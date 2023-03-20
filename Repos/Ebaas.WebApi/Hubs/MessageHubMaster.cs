using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Data;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Text.RegularExpressions;

using Microsoft.AspNet.SignalR;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Ebaas.WebApi.Infrastructure;
using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Server.UsrMgr;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Principal;
using Newtera.WorkflowServices;

namespace Ebaas.WebApi.Hubs
{
    /// <summary>
    /// The single instance of the MessageHub which can be called anywhere in the server
    /// </summary>
    public class MessageHubMaster : IMessageService
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME=COMMON;SCHEMA_VERSION=1.0";
        private const string GROUP_ATTRIBUTE_NAME = "Name";
        private const string GROUP_CLASS_NAME = "Groups";
        private const string MEMBER_ATTRIBUTE_NAME = "Name";
        private const string MEMBER_CLASS_NAME = "Members";
        private const string MEMBER_TO_GROUP_CLASS_NAME = "GroupToMember";
        private const string MESSAGES_TYPE = "msgs";
        private const string TASKS_TYPE = "tasks";
        private const int PAGE_SIZE = 20;
        private const int MAX_PAGE_SIZE = 100;

        #region Implementation of singleton

        private static readonly MessageHubMaster _instance = new MessageHubMaster(GlobalHost.ConnectionManager.GetHubContext<MessageHub>());

        private readonly IHubContext _hubContext;

        private HashSet<string> _connectedIds = new HashSet<string>();

        private MessageHubMaster(IHubContext context)
        {
            _hubContext = context;

            Newtera.Server.Engine.Workflow.NewteraWorkflowRuntime.Instance.GetWorkflowRunTime().AddService(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public IHubContext GlobalContext
        {
            get { return _hubContext; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static MessageHubMaster Instance
        {
            get { return _instance; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Add a connected id
        /// </summary>
        /// <param name="connectedId"></param>
        public void AddConnectedId(string connectedId)
        {
            _connectedIds.Add(connectedId);
        }

        /// <summary>
        /// Remove a connected id
        /// </summary>
        /// <param name="connectedId"></param>
        public void RemoveConnectedId(string connectedId)
        {
            _connectedIds.Remove(connectedId);
        }

        /// <summary>
        /// Check if the number of client connection exceeds the limit specified by the license?
        /// </summary>
        /// <returns>true if it exceed the limit, false, otherwise</returns>
        public bool ExceedConnectionLimit()
        {
            int num = _connectedIds.Count;

            return false;
        }

        /// <summary>
        /// Get an unique group name
        /// </summary>
        /// <param name="schemaId">schema name plus version number</param>
        /// <param name="className">class name</param>
        /// <param name="objId">instance id</param>
        public string GetGroupName(string schemaId, string className, string objId)
        {
            string groupName = "";

            string[] schemaInfo = schemaId.Split(' ');

            // groupName is schemaName-className-objId
            if (schemaInfo.Length > 1)
            {
                groupName = schemaInfo[0] + "-" + className + "-" + objId;
            }

            return groupName;
        }

        /// <summary>
        /// Send a message to a hub group
        /// </summary>
        /// <param name="groupName">The group name</param>
        /// <param name="message">The message to be sent</param>
        public void SendMessageToGroup(string groupName, Newtera.WFModel.MessageInfo message)
        {
            // persist the messages for each user in the group
            PersistGroupMessage(groupName, message);

            _hubContext.Clients.Group(groupName).addMessage(MESSAGES_TYPE, message);
        }

        /// <summary>
        /// Send a message to a list of users
        /// </summary>
        /// <param name="userNames">The user names</param>
        /// <param name="message">The message to be sent</param>
        public void SendMessageToUsers(List<string> userNames, Newtera.WFModel.MessageInfo message)
        {
            // persist the messages for each user in the group
            PersistUsersMessage(userNames, message);

            _hubContext.Clients.Users(userNames).addMessage(MESSAGES_TYPE, message);
        }

        /// <summary>
        /// Send a task notice to an user
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="message">The message to be sent</param>
        public void SendTaskNoticeToUser(string userName, Newtera.WFModel.MessageInfo message)
        {
            _hubContext.Clients.User(userName).addMessage(TASKS_TYPE, message);
        }

        /// <summary>
        /// Add a connecting user to a group
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="schemaName"></param>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public Task AddToGroup(string connectionId, string schemaName, string userName, string groupName)
        {
            SaveUserToGroupRelationship(schemaName, userName, groupName);

            return _hubContext.Groups.Add(connectionId, groupName);
        }

        /// <summary>
        /// Remove a connection user from a group
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="schemaName"></param>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public Task RemoveFromGroup(string connectionId, string schemaName, string userName, string groupName)
        {
            RemoveUserToGroupRelationship(schemaName, userName, groupName);

            TryRemoveUser(schemaName, userName); // remove the user from the member class if the user is no longer in any groups

            TryRemoveGroup(schemaName, groupName); // remove the group from the Group class if the group has no members

            return _hubContext.Groups.Remove(connectionId, groupName);
        }

        public List<string> GetUserGroups(string schemaName, string userName)
        {
            try
            {
                List<string> groups = new List<string>();
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView userInstanceView = GetUserInstanceView(con, MEMBER_CLASS_NAME, userName);

                    if (userInstanceView != null)
                    {
                        // user exsts
                        DataViewModel dataView = con.MetaDataModel.GetRelatedDetailedDataView(userInstanceView, GROUP_CLASS_NAME);

                        if (dataView != null)
                        {
                            int pageSize = 1000; // Assume an user can have less 1000 groups
                            int pageIndex = 0;

                            dataView.PageIndex = pageIndex;
                            dataView.PageSize = pageSize;

                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            XmlReader reader = cmd.ExecuteXMLReader();
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                            {
                                InstanceView instanceView = new InstanceView(dataView, ds);

                                count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                                for (int row = 0; row < count; row++)
                                {
                                    instanceView.SelectedIndex = row; // set the cursor

                                    string group = instanceView.InstanceData.GetAttributeStringValue(GROUP_ATTRIBUTE_NAME);

                                    groups.Add(group);
                                }
                            }
                        }
                    }
                }

                return groups;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        public List<string> GetUserGroups(string schemaName, string className, string userName, int startIndex, int pageSize)
        {
            CMDataReader dataReader = null;

            try
            {
                List<string> groups = new List<string>();
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView userInstanceView = GetUserInstanceView(con, MEMBER_CLASS_NAME, userName);

                    if (userInstanceView != null)
                    {
                        // user exsts
                        DataViewModel dataView = con.MetaDataModel.GetRelatedDetailedDataView(userInstanceView, GROUP_CLASS_NAME);

                        if (dataView != null)
                        {
                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.PageSize = PAGE_SIZE;
                            cmd.CommandText = query;

                            // use Default behavior so that when closing CMDataReader, the
                            // connection won't be closed
                            dataReader = cmd.ExecuteReader();
                            XmlDocument doc;
                            XmlReader xmlReader;
                            DataSet ds;

                            if (pageSize > MAX_PAGE_SIZE)
                            {
                                pageSize = MAX_PAGE_SIZE;
                            }
                            int count = 0;
                            int from = startIndex;
                            int to = startIndex + pageSize;
                            int instanceCount;

                            // read data one page per time
                            while (dataReader.Read())
                            {
                                doc = dataReader.GetXmlDocument();

                                xmlReader = new XmlNodeReader(doc);
                                ds = new DataSet();
                                ds.ReadXml(xmlReader);

                                if (DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                                {
                                    // got an empty result
                                    break;
                                }

                                InstanceView instanceView = new InstanceView(dataView, ds);

                                instanceCount = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                                for (int row = 0; row < instanceCount; row++)
                                {
                                    instanceView.SelectedIndex = row; // set the cursor

                                    string group = instanceView.InstanceData.GetAttributeStringValue(GROUP_ATTRIBUTE_NAME);

                                    // check if the group name matches the criteria
                                    if (IsTheGroup(schemaName, className, group))
                                    {
                                        if (count < from)
                                        {
                                            // move the cursor of result set to the position indicated by the from
                                            count++;
                                        }
                                        else if (count < to)
                                        {
                                            count++;
                                            groups.Add(group);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (groups.Count == pageSize)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                return groups;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
            }
        }

        public bool IsUserInGroup(string schemaName, string userName, string groupName)
        {
            try
            {
                bool status = false;

                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView userInstanceView = GetUserInstanceView(con, MEMBER_CLASS_NAME, userName);

                    if (userInstanceView != null)
                    {
                        // user exsts
                        DataViewModel dataView = con.MetaDataModel.GetRelatedDetailedDataView(userInstanceView, GROUP_CLASS_NAME);

                        if (dataView != null)
                        {
                            int pageSize = 1000; // Assume an user can have less 1000 groups
                            int pageIndex = 0;

                            dataView.PageIndex = pageIndex;
                            dataView.PageSize = pageSize;

                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            XmlReader reader = cmd.ExecuteXMLReader();
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                            {
                                InstanceView instanceView = new InstanceView(dataView, ds);
                                count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                                for (int row = 0; row < count; row++)
                                {
                                    instanceView.SelectedIndex = row; // set the cursor

                                    string group = instanceView.InstanceData.GetAttributeStringValue(GROUP_ATTRIBUTE_NAME);

                                    if (group == groupName)
                                    {
                                        status = true;

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                return status;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        #endregion

        #region private methods

        private bool IsTheGroup(string schemaName, string className, string groupName)
        {
            bool status = false;

            // groupName is schemaName-className-objId
            string groupNamePrefix = schemaName + "-" + className + "-";
            if (groupName.StartsWith(groupNamePrefix))
            {
                status = true;
            }

            return status;
        }

        private void PersistGroupMessage(string groupName, Newtera.WFModel.MessageInfo messageInfo)
        {
            List<string> users = GetGroupMembers(messageInfo.SchemaName, groupName);
            MessageManager messageManager = new MessageManager();

            foreach (string userName in users)
            {
                messageManager.AddMessage(userName, messageInfo);
            }
        }

        private void PersistUsersMessage(List<string> userNames, Newtera.WFModel.MessageInfo messageInfo)
        {
            MessageManager messageManager = new MessageManager();

            foreach (string userName in userNames)
            {
                messageManager.AddMessage(userName, messageInfo);
            }
        }

        private void SaveUserToGroupRelationship(string schemaName, string userName, string groupName)
        {
            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                QueryHelper queryHelper = new QueryHelper();

                // execute the query as a super user
                Thread.CurrentPrincipal = superUser;

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView userInstanceView = GetUserInstanceView(con, MEMBER_CLASS_NAME, userName);

                    string userObjId;
                    if (userInstanceView == null)
                    {
                        // add the user
                        userObjId = AddUser(con, MEMBER_CLASS_NAME, userName);
                    }
                    else
                    {
                        userObjId = userInstanceView.InstanceData.ObjId;
                    }

                    InstanceView groupInstanceView = GetGroupInstanceView(con, GROUP_CLASS_NAME, groupName);
                    string groupObjId;
                    if (groupInstanceView == null)
                    {
                        // add the group
                        groupObjId = AddGroup(con, GROUP_CLASS_NAME, groupName);
                    }
                    else
                    {
                        groupObjId = groupInstanceView.InstanceData.ObjId;
                    }

                    // there is a many-to-many relationship between master class and related class
                    CreateManyToManyRelationship(con, MEMBER_CLASS_NAME, userObjId, GROUP_CLASS_NAME, groupObjId, MEMBER_TO_GROUP_CLASS_NAME);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        private void RemoveUserToGroupRelationship(string schemaName, string userName, string groupName)
        {
            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                QueryHelper queryHelper = new QueryHelper();

                // execute the query as a super user
                Thread.CurrentPrincipal = superUser;

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView userInstanceView = GetUserInstanceView(con, MEMBER_CLASS_NAME, userName);

                    string userObjId = null;
                    if (userInstanceView != null)
                    {
                        userObjId = userInstanceView.InstanceData.ObjId;
                    }

                    InstanceView groupInstanceView = GetGroupInstanceView(con, GROUP_CLASS_NAME, groupName);
                    string groupObjId = null;
                    if (groupInstanceView != null)
                    {
                        groupObjId = groupInstanceView.InstanceData.ObjId;
                    }

                    if (userObjId != null && groupObjId != null)
                    {
                        RemoveManyToManyRelationship(con, MEMBER_CLASS_NAME, userObjId, GROUP_CLASS_NAME, groupObjId, MEMBER_TO_GROUP_CLASS_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        private void RemoveManyToManyRelationship(CMConnection connection, string masterClassName, string masterInstanceId, string relatedClassName, string relatedInstanceId, string junctionClassName)
        {
            // first make sure the many-to-many link doesn't exist (it could add by another user)
            DataViewModel junctionClassDataView = connection.MetaDataModel.GetDefaultDataView(junctionClassName);
            junctionClassDataView.ClearSearchExpression();

            DataClass leftSideClass = null;
            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == masterClassName)
                {
                    if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
                    {
                        leftSideClass = relatedClass;
                        break;
                    }
                }
            }

            DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias);
            Newtera.Common.MetaData.DataView.Parameter right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias, DataType.String);
            right.ParameterValue = masterInstanceId;
            RelationalExpr expr = new RelationalExpr(ElementType.Equals, left, right);

            // add search expression to the dataview
            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

            DataClass rightSideClass = null;
            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == relatedClassName)
                {
                    if (relatedClass != leftSideClass && relatedClass.ReferringRelationship.IsForeignKeyRequired)
                    {
                        rightSideClass = relatedClass;
                        break;
                    }
                }
            }

            left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias);
            right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias, DataType.String);
            right.ParameterValue = relatedInstanceId;
            expr = new RelationalExpr(ElementType.Equals, left, right);

            // add search expression to the dataview
            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

            // search the relationship
            string query = junctionClassDataView.SearchQuery;
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader(CMCommandBehavior.CheckReadPermissionOnly);
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, junctionClassDataView.BaseClass.ClassName))
            {
                InstanceView instanceView = new InstanceView(junctionClassDataView, ds);
                string instanceId = instanceView.InstanceData.ObjId;

                // delete the relationship
                junctionClassDataView.CurrentObjId = instanceId;

                query = junctionClassDataView.DeleteQuery;

                cmd = connection.CreateCommand();
                cmd.CommandText = query;

                cmd.ExecuteXMLDoc(); // delete
            }
        }

        private void TryRemoveUser(string schemaName, string userName)
        {
            int groupCount = GetUserGroupCount(schemaName, userName);
            if (groupCount == 0)
            {
                // user no longer belongs to any groups, delete from Member class
                CMUserManager userMgr = new CMUserManager();
                IPrincipal superUser = userMgr.SuperUser;

                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    QueryHelper queryHelper = new QueryHelper();

                    // execute the query as a super user
                    Thread.CurrentPrincipal = superUser;

                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        InstanceView userInstanceView = GetUserInstanceView(con, MEMBER_CLASS_NAME, userName);

                        string userObjId = null;
                        if (userInstanceView != null)
                        {
                            userObjId = userInstanceView.InstanceData.ObjId;
                        }

                        if (userObjId != null)
                        {
                            userInstanceView.DataView.CurrentObjId = userObjId;

                            string query = userInstanceView.DataView.DeleteQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            cmd.ExecuteXMLDoc(); // delete
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                    throw ex;
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
        }

        private void TryRemoveGroup(string schemaName, string groupName)
        {
            int memberCount = GetGroupMemberCount(schemaName, groupName);
            if (memberCount == 0)
            {
                // the group no longer has any users, delete from Group class
                CMUserManager userMgr = new CMUserManager();
                IPrincipal superUser = userMgr.SuperUser;

                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    QueryHelper queryHelper = new QueryHelper();

                    // execute the query as a super user
                    Thread.CurrentPrincipal = superUser;

                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        InstanceView groupInstanceView = GetGroupInstanceView(con, GROUP_CLASS_NAME, groupName);

                        string groupObjId = null;
                        if (groupInstanceView != null)
                        {
                            groupObjId = groupInstanceView.InstanceData.ObjId;
                        }

                        if (groupObjId != null)
                        {
                            groupInstanceView.DataView.CurrentObjId = groupObjId;

                            string query = groupInstanceView.DataView.DeleteQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            cmd.ExecuteXMLDoc(); // delete
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                    throw ex;
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
        }

        /// <summary>
        /// Get an user InstanceView object
        /// </summary>
        /// <param name="connection">DB connection</param>
        /// <param name="className">Class name of the instance view</param>
        /// <param name="userName">User's name, unique in the Memebers class</param>
        /// <returns>An InstanceView object</returns>
        private InstanceView GetUserInstanceView(CMConnection connection, string className, string userName)
        {
            InstanceView instanceView = null;

            MetaDataModel metaData = connection.MetaDataModel;

            DataViewModel dataView = metaData.GetDetailedDataView(className);

            string query = dataView.GetInstanceByAttributeValueQuery(MEMBER_ATTRIBUTE_NAME, userName);

            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                instanceView = new InstanceView(dataView, ds);
            }

            return instanceView;
        }

        private string AddUser(CMConnection connection, string className, string userName)
        {
            MetaDataModel metaData = connection.MetaDataModel;

            DataViewModel dataView = metaData.GetDetailedDataView(className);

            InstanceView instanceView = new InstanceView(dataView);

            instanceView.InstanceData.SetAttributeStringValue(MEMBER_ATTRIBUTE_NAME, userName);

            string query = instanceView.DataView.InsertQuery;

            CMCommand cmd = connection.CreateCommand();

            cmd.CommandText = query;

            XmlDocument doc = cmd.ExecuteXMLDoc();// insert the instance to database

            return doc.DocumentElement.InnerText;
        }

        private string AddGroup(CMConnection connection, string className, string groupName)
        {
            MetaDataModel metaData = connection.MetaDataModel;

            DataViewModel dataView = metaData.GetDetailedDataView(className);

            InstanceView instanceView = new InstanceView(dataView);

            instanceView.InstanceData.SetAttributeStringValue(GROUP_ATTRIBUTE_NAME, groupName);

            string query = instanceView.DataView.InsertQuery;

            CMCommand cmd = connection.CreateCommand();

            cmd.CommandText = query;

            XmlDocument doc = cmd.ExecuteXMLDoc();// insert the instance to database

            return doc.DocumentElement.InnerText;
        }

        private int GetUserGroupCount(string schemaName, string userName)
        {
            try
            {
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView userInstanceView = GetUserInstanceView(con, MEMBER_CLASS_NAME, userName);

                    if (userInstanceView != null)
                    {
                        // user exsts
                        DataViewModel dataView = con.MetaDataModel.GetRelatedDetailedDataView(userInstanceView, GROUP_CLASS_NAME);

                        if (dataView != null)
                        {
                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            count = cmd.ExecuteCount();
                        }
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        private List<string> GetGroupMembers(string schemaName, string groupName)
        {
            try
            {
                List<string> members = new List<string>();
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView groupInstanceView = GetUserInstanceView(con, GROUP_CLASS_NAME, groupName);

                    if (groupInstanceView != null)
                    {
                        // user exsts
                        DataViewModel dataView = con.MetaDataModel.GetRelatedDetailedDataView(groupInstanceView, MEMBER_CLASS_NAME);

                        if (dataView != null)
                        {
                            int pageSize = 1000; // Assume a group can have less 1000 members
                            int pageIndex = 0;

                            dataView.PageIndex = pageIndex;
                            dataView.PageSize = pageSize;

                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            XmlReader reader = cmd.ExecuteXMLReader();
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                            {
                                InstanceView instanceView = new InstanceView(dataView, ds);
                                count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                                for (int row = 0; row < count; row++)
                                {
                                    instanceView.SelectedIndex = row; // set the cursor

                                    string member = instanceView.InstanceData.GetAttributeStringValue(MEMBER_ATTRIBUTE_NAME);

                                    members.Add(member);
                                }
                            }
                        }
                    }
                }

                return members;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        private int GetGroupMemberCount(string schemaName, string groupName)
        {
            try
            {
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    InstanceView groupInstanceView = GetUserInstanceView(con, GROUP_CLASS_NAME, groupName);

                    if (groupInstanceView != null)
                    {
                        // user exsts
                        DataViewModel dataView = con.MetaDataModel.GetRelatedDetailedDataView(groupInstanceView, MEMBER_CLASS_NAME);

                        if (dataView != null)
                        {
                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            count = cmd.ExecuteCount();
                        }
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }


        /// <summary>
        /// Get a group InstanceView object
        /// </summary>
        /// <param name="connection">DB connection</param>
        /// <param name="className">Class name of the instance view</param>
        /// <param name="groupName">Group's name, unique in the Groups class</param>
        /// <returns>An InstanceView object</returns>
        private InstanceView GetGroupInstanceView(CMConnection connection, string className, string groupName)
        {
            InstanceView instanceView = null;

            MetaDataModel metaData = connection.MetaDataModel;

            DataViewModel dataView = metaData.GetDetailedDataView(className);

            string query = dataView.GetInstanceByAttributeValueQuery(GROUP_ATTRIBUTE_NAME, groupName);

            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                instanceView = new InstanceView(dataView, ds);
            }

            return instanceView;
        }

        private void CreateManyToManyRelationship(CMConnection connection, string masterClassName, string masterInstanceId, string relatedClassName, string relatedInstanceId, string junctionClassName)
        {
            // first make sure the many-to-many link doesn't exist (it could add by another user)
            DataViewModel junctionClassDataView = connection.MetaDataModel.GetDefaultDataView(junctionClassName);
            junctionClassDataView.ClearSearchExpression();

            DataClass leftSideClass = null;
            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == masterClassName)
                {
                    if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
                    {

                        leftSideClass = relatedClass;
                        break;
                    }
                }
            }

            DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias);
            Newtera.Common.MetaData.DataView.Parameter right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias, DataType.String);
            right.ParameterValue = masterInstanceId;
            RelationalExpr expr = new RelationalExpr(ElementType.Equals, left, right);

            // add search expression to the dataview
            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

            DataClass rightSideClass = null;
            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == relatedClassName)
                {
                    if (relatedClass != leftSideClass && relatedClass.ReferringRelationship.IsForeignKeyRequired)
                    {
                        rightSideClass = relatedClass;
                        break;
                    }
                }
            }

            left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias);
            right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias, DataType.String);
            right.ParameterValue = relatedInstanceId;
            expr = new RelationalExpr(ElementType.Equals, left, right);

            // add search expression to the dataview
            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

            // search the relationship
            string query = junctionClassDataView.SearchQuery;
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            // Since the result will be displayed on DataGridView, we don't need to check
            // write permissions on each attribute, use CMCommandBehavior.CheckReadPermissionOnly
            XmlReader reader = cmd.ExecuteXMLReader(CMCommandBehavior.CheckReadPermissionOnly);
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (DataSetHelper.IsEmptyDataSet(ds, junctionClassDataView.BaseClass.ClassName))
            {
                InstanceView instanceView = new InstanceView(junctionClassDataView);
                DataRelationshipAttribute leftRelationshipAttribute = null;
                foreach (IDataViewElement resultAttribute in junctionClassDataView.ResultAttributes)
                {
                    leftRelationshipAttribute = resultAttribute as DataRelationshipAttribute;
                    if (leftRelationshipAttribute != null &&
                        leftRelationshipAttribute.IsForeignKeyRequired)
                    {
                        if (leftRelationshipAttribute.LinkedClassName == masterClassName)
                        {
                            DataViewModel masterClassDataView = connection.MetaDataModel.GetDetailedDataView(masterClassName);

                            SetRelationshipValue(connection, instanceView, leftRelationshipAttribute, masterClassDataView, masterInstanceId);

                            break;
                        }
                    }
                }

                DataRelationshipAttribute rightRelationshipAttribute;
                foreach (IDataViewElement resultAttribute in junctionClassDataView.ResultAttributes)
                {
                    rightRelationshipAttribute = resultAttribute as DataRelationshipAttribute;
                    if (rightRelationshipAttribute != null &&
                        rightRelationshipAttribute != leftRelationshipAttribute &&
                        rightRelationshipAttribute.IsForeignKeyRequired)
                    {
                        if (rightRelationshipAttribute.LinkedClassName == relatedClassName)
                        {
                            DataViewModel relatedClassDataView = connection.MetaDataModel.GetDetailedDataView(relatedClassName);

                            SetRelationshipValue(connection, instanceView, rightRelationshipAttribute, relatedClassDataView, relatedInstanceId);

                            break;

                        }
                    }
                }

                query = junctionClassDataView.InsertQuery;

                cmd = connection.CreateCommand();
                cmd.CommandText = query;

                cmd.ExecuteXMLDoc(); // insert
            }
        }

        private void SetRelationshipValue(CMConnection connection, InstanceView instanceView,
            DataRelationshipAttribute relationshipAttribute, DataViewModel relatedDataView, string relatedInstanceId)
        {
            // first, we need get the primary key values of the related instance
            string query = relatedDataView.GetInstanceQuery(relatedInstanceId);
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;
            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            InstanceView relatedInstanceView = new InstanceView(relatedDataView, ds);
            string[] pkValues = relatedInstanceView.InstanceData.PrimaryKeyValues.Split('&');
            // then set the primary key value as foreign key values to the instance view
            DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
            if (primaryKeys != null)
            {
                int index = 0;
                foreach (DataSimpleAttribute pk in primaryKeys)
                {
                    if (index < pkValues.Length && pkValues[index] != null)
                    {
                        // to set a pk value, the name combines that of relationship attribute and primary key
                        instanceView.InstanceData.SetAttributeValue(relationshipAttribute.Name + "_" + pk.Name, pkValues[index].Trim());
                    }
                    index++;
                }
            }
        }

        #endregion
    }
}  
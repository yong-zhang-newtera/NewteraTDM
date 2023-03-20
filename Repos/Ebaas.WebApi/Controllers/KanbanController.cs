using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;

using Swashbuckle.Swagger.Annotations;

using Ebaas.WebApi.Infrastructure;
using Ebaas.WebApi.Models;
using Newtera.Common.Core;
using Newtera.Common.MetaData.SiteMap;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Data;
using Newtera.WFModel;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.DataView;
using Newtera.Server.Engine.Workflow;
using Newtera.Server.Engine.Cache;
using Newtera.Server.Engine.Interpreter;
using Newtera.Common.MetaData.XaclModel.Processor;
using Ebaas.WebApi.Hubs;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// Kanban is Japanese for “visual signal” or “card.” The Kanban’s highly visual nature allowed teams to
    /// communicate more easily on what work needed to be done and when.
    /// The Kanban service allows you to build a Kanban user interface component.
    /// </summary>
    [RoutePrefix("api/kanban")]
    public class KanbanController : ApiController
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string KANBAN_CONNECTION_STRING = @"SCHEMA_NAME=COMMON;SCHEMA_VERSION=1.0";
        private const string TEMPLATE_XQUERY = "let $this := getCurrentInstance() return <flag>{if ($$) then 1 else 0}</flag>";
        private const string KANBAN_CLASS = "Kanban";
        private const string KANBAN_NAME = "Name";
        private const string KANBAN_TEXT = "DisplayText";
        private const string KANBAN_GROUP_CLASS = "GroupClass";
        private const string KANBAN_ITEM_CLASS = "ItemClass";
        private const string KANBAN_DATA_VIEW = "KanbanDataView";
        private const string STATE_CLASS = "KanbanStates";
        private const string STATE_ID = "StateID";
        private const string STATE_NAME = "Name";
        private const string STATE_ALLOW_ADD = "AllowAdd";
        private const string GROUP_ID_PROPERTY = "GroupIDProperty";
        private const string GROUP_NAME_PROPERTY = "GroupNameProperty";
        private const string GROUP_STATE_PROPERTY = "GroupStateProperty";
        private const string GROUP_PROGRESS_PROPERTY = "GroupProgressProperty";
        private const string ITEM_OBJ_ID_PROPERTY = "ItemObjIDProperty";
        private const string ITEM_ID_PROPERTY = "ItemIDProperty";
        private const string ITEM_NAME_PROPERTY = "ItemNameProperty";
        private const string ITEM_STATE_PROPERTY = "ItemStateProperty";
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";
        private const string FILTER = "filter";
        private const string OBJ_ID = "objId";

        /// <summary>
        /// Gets the kanban data for a specified kanban for the requesting user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="kanbanName">The name of a kanban</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("data/{schemaName}/{kanbanName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(KanbanModel), Description = "A kanban data model for the connecting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetData(string schemaName, string kanbanName)
        {
            QueryHelper queryHelper = new QueryHelper();
            List<KanbanGroupModel> groups;
            List<KanbanItemModel> items;
            List<KanbanCommandModel> groupCommands;
            List<KanbanCommandModel> itemCommands;
            KanbanModel kanbanModel = new KanbanModel();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    InstanceView kanbanInstance = GetKanbanInstance(kanbanName);
                    List<KanbanStateModel> states = GetKanbanStates(kanbanInstance, schemaName, kanbanName);

                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        if (kanbanInstance != null)
                        {
                            string kanbanDataView = kanbanInstance.InstanceData.GetAttributeStringValue(KANBAN_DATA_VIEW);
                            string groupClass = kanbanInstance.InstanceData.GetAttributeStringValue(KANBAN_GROUP_CLASS);
                            string itemClass = kanbanInstance.InstanceData.GetAttributeStringValue(KANBAN_ITEM_CLASS);

                            DataViewModel dataView = con.MetaDataModel.GetDataView(kanbanDataView);

                            if (dataView == null)
                            {
                                throw new Exception("Unable to find a kanban data view with name " + kanbanDataView);
                            }

                            NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                            string objId = GetParamValue(parameters, OBJ_ID, "");
                            int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 5));
                            int pageIndex = int.Parse(GetParamValue(parameters, START_ROW, 0)) / pageSize;
                            StringCollection groupIds;

                            if (!string.IsNullOrEmpty(objId))
                            {
                                // get single group using the parameter
                                groupIds = new StringCollection();
                                groupIds.Add(objId);
                            }
                            else
                            {
                                // get obj_ids of the groups which the connecting user is tracking on
                                groupIds = GetKanbanGroupIds(schemaName, groupClass, pageIndex, pageSize);
                            }

                            if (groupIds.Count > 0)
                            {
                                XmlDocument kanbanDataXmlDcoument = GetKanbanDataXmlDocument(con, dataView, groupIds);

                                // convert doc to instance view
                                XmlReader reader = new XmlNodeReader(kanbanDataXmlDcoument);
                                DataSet ds = new DataSet();
                                ds.ReadXml(reader);

                                InstanceView kanbanDataInstanceView = null;
                                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                                {
                                    kanbanDataInstanceView = new InstanceView(dataView, ds);


                                    groups = GetKanbanGroups(kanbanInstance, schemaName, kanbanDataInstanceView, kanbanDataXmlDcoument);
                                    items = GetKanbanItems(kanbanInstance, schemaName, states, kanbanDataInstanceView, kanbanDataXmlDcoument);
                                    groupCommands = GetCustomCommands(schemaName, groupClass);
                                    itemCommands = GetCustomCommands(schemaName, itemClass);

                                    //var jsonString = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
                                    //ErrorLog.Instance.WriteLine(jsonString);
                                }
                                else
                                {
                                    groups = new List<KanbanGroupModel>();
                                    items = new List<KanbanItemModel>();
                                    groupCommands = new List<KanbanCommandModel>();
                                    itemCommands = new List<KanbanCommandModel>();
                                }
                            }
                            else
                            {
                                groups = new List<KanbanGroupModel>();
                                items = new List<KanbanItemModel>();
                                groupCommands = new List<KanbanCommandModel>();
                                itemCommands = new List<KanbanCommandModel>();
                            }

                            kanbanModel.text = kanbanInstance.InstanceData.GetAttributeStringValue(KANBAN_TEXT);
                            kanbanModel.states = states;
                            kanbanModel.groups = groups;
                            kanbanModel.items = items;
                            kanbanModel.groupCommands = groupCommands;
                            kanbanModel.itemCommands = itemCommands;

                        }
                        else
                        {
                            throw new Exception("Unable to find a kanban instance with name " + kanbanName);
                        }
                    }
                });

                return Ok(kanbanModel);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets kanban items of a given group and kanban name
        /// </summary>
        private List<KanbanItemModel> GetKanbanItems(InstanceView kanbanInstance, string schemaName, List<KanbanStateModel> states, InstanceView kanbanDataInstanceView, XmlDocument kanbanDataXmlDcoument)
        {
            List<KanbanItemModel> items = new List<KanbanItemModel>();
            
            string groupClass = kanbanInstance.InstanceData.GetAttributeStringValue(KANBAN_GROUP_CLASS);
            string groupIDProperty = kanbanInstance.InstanceData.GetAttributeStringValue(GROUP_ID_PROPERTY);
            string itemClass = kanbanInstance.InstanceData.GetAttributeStringValue(KANBAN_ITEM_CLASS);
            string itemObjIDPropertyName = kanbanInstance.InstanceData.GetAttributeStringValue(ITEM_OBJ_ID_PROPERTY);
            string itemIDPropertyName = kanbanInstance.InstanceData.GetAttributeStringValue(ITEM_ID_PROPERTY);
            string itemNamePropertyName = kanbanInstance.InstanceData.GetAttributeStringValue(ITEM_NAME_PROPERTY);
            string itemStatePropertyName = kanbanInstance.InstanceData.GetAttributeStringValue(ITEM_STATE_PROPERTY);

            KanbanItemModel item;
            string groupId, stateName;
            int count = DataSetHelper.GetRowCount(kanbanDataInstanceView.InstanceData.DataSet, kanbanDataInstanceView.DataView.BaseClass.ClassName);
            for (int row = 0; row < count; row++)
            {
                kanbanDataInstanceView.SelectedIndex = row;

                groupId = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(groupIDProperty);

                item = new KanbanItemModel();
                item.id = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(itemIDPropertyName);
                item.objId = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(itemObjIDPropertyName);
                item.className = itemClass;
                item.name = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(itemNamePropertyName);
                item.groupId = groupId;
                stateName = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(itemStatePropertyName);
                KanbanStateModel foundState = FindState(stateName, states);
                if (foundState != null)
                {
                    item.stateId = foundState.id;
                }
                else
                {
                    item.stateId = null;
                }

                item.allowWrite = false;

                item.commands = GetCustomCommandNames(schemaName, itemClass, GetXmlInstance(kanbanDataXmlDcoument, row));

                items.Add(item);
            }
               
            return items;
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }

        private StringCollection GetKanbanGroupIds(string schemaName, string className, int pageIndex, int pageSize)
        {
            StringCollection groupIds = new StringCollection();

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            string userName = principal.Identity.Name;

            MessageHubMaster messageHubMaster = MessageHubMaster.Instance;

            List<string> groupNames = messageHubMaster.GetUserGroups(schemaName, className, userName, pageIndex * pageSize, pageSize);

            int pos;
            foreach (string groupName in groupNames)
            {
                // groupName is in form of schemaName-className-objId
                pos = groupName.LastIndexOf('-');
                if (pos > 0)
                {
                    groupIds.Add(groupName.Substring(pos + 1)); // get obj_id from the string
                }
            }

            return groupIds;
        }

        private List<KanbanStateModel> GetKanbanStates(InstanceView kanbanInstance, string schemaName, string kanbanName)
        {
            using (CMConnection connection = new CMConnection(KANBAN_CONNECTION_STRING))
            {
                connection.Open();

                List<KanbanStateModel> states = new List<KanbanStateModel>();
                int count = 0;

                DataViewModel dataView = connection.MetaDataModel.GetRelatedDefaultDataView(kanbanInstance, STATE_CLASS);

                string query = dataView.SearchQuery;

                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    InstanceView instanceView = new InstanceView(dataView, ds);

                    count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                    KanbanStateModel state;
                    for (int row = 0; row < count; row++)
                    {
                        instanceView.SelectedIndex = row;

                        state = new KanbanStateModel();
                        state.id = instanceView.InstanceData.GetAttributeStringValue(STATE_ID);
                        state.name = instanceView.InstanceData.GetAttributeStringValue(STATE_NAME);
                        state.areNewItemButtonsHidden = true;
                        string str = instanceView.InstanceData.GetAttributeStringValue(STATE_ALLOW_ADD);
                        if (!string.IsNullOrEmpty(str))
                        {
                            try
                            {
                                state.areNewItemButtonsHidden = bool.Parse(str);
                            }
                            catch (Exception)
                            { }
                        }

                        states.Add(state);
                    }
                }

                return states;
            }
        }

        private XmlDocument GetKanbanDataXmlDocument(CMConnection con, DataViewModel dataView, StringCollection groupIds)
        {
            XmlDocument doc = null;
            QueryHelper queryHelper = new QueryHelper();

            string query = null;
            if (groupIds != null && groupIds.Count == 1)
            {
                // get a single instance by the objId
                query = dataView.GetInstanceQuery(groupIds[0]);
            }
            else if (groupIds != null && groupIds.Count > 1)
            {
                query = dataView.GetInstancesQuery(groupIds);
            }
            else
            {
                query = dataView.SearchQuery;
            }

            if (query != null)
            {
                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                doc = cmd.ExecuteXMLDoc();
 
            }

            return doc;
        }

        private List<KanbanGroupModel> GetKanbanGroups(InstanceView kanbanInstance, string schemaName, InstanceView kanbanDataInstanceView, XmlDocument kanbanDataXmlDcoument)
        {
            List<KanbanGroupModel> groups = new List<KanbanGroupModel>();
            int count = 0;
            QueryHelper queryHelper = new QueryHelper();

            string groupClass = kanbanInstance.InstanceData.GetAttributeStringValue(KANBAN_GROUP_CLASS);
            string groupIDPropertyName = kanbanInstance.InstanceData.GetAttributeStringValue(GROUP_ID_PROPERTY);
            string groupNamePropertyName = kanbanInstance.InstanceData.GetAttributeStringValue(GROUP_NAME_PROPERTY);
            string groupProgressPropertyName = kanbanInstance.InstanceData.GetAttributeStringValue(GROUP_PROGRESS_PROPERTY);

            count = DataSetHelper.GetRowCount(kanbanDataInstanceView.InstanceData.DataSet, kanbanDataInstanceView.DataView.BaseClass.ClassName);
            KanbanGroupModel group;
            string groupId;
            for (int row = 0; row < count; row++)
            {
                kanbanDataInstanceView.SelectedIndex = row;

                groupId = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(groupIDPropertyName);
                if (!IsGroupExist(groups, groupId))
                {
                    group = new KanbanGroupModel();
                    group.id = groupId;
                    group.objId = kanbanDataInstanceView.InstanceData.ObjId;
                    group.className = groupClass;
                    group.name = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(groupNamePropertyName);
                    group.progress = 0;
                    group.track = true;
                    group.allowWrite = false;
                    /*
                    if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, entity, XaclActionType.Write, _originalInstance))
                    {
                        throw new PermissionViolationException("Do not have permissions to write to the attribute " + entity.Name);
                    }
                    */
                    string str = kanbanDataInstanceView.InstanceData.GetAttributeStringValue(groupProgressPropertyName);
                    if (!string.IsNullOrEmpty(str))
                    {
                        try
                        {
                            group.progress = int.Parse(str);
                        }
                        catch (Exception)
                        {
                            group.progress = 0;
                        }
                    }

                    group.commands = GetCustomCommandNames(schemaName, groupClass, GetXmlInstance(kanbanDataXmlDcoument, row));
                    groups.Add(group);
                }
            }
 
            return groups;
        }

        private bool IsGroupExist(List<KanbanGroupModel> groups, string groupId)
        {
            bool status = false;

            foreach (KanbanGroupModel group in groups)
            {
                if (group.id == groupId)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }

        private KanbanStateModel FindState(string stateName, List<KanbanStateModel> states)
        {
            KanbanStateModel found = null;

            foreach (KanbanStateModel state in states)
            {
                if (state.name == stateName)
                {
                    found = state;

                    break;
                }
            }

            return found;
        }

        private string GetParamValue(NameValueCollection parameters, string key, object defaultValue)
        {
            string val = null;

            if (defaultValue != null)
            {
                val = defaultValue.ToString();
            }

            if (parameters[key] != null)
            {
                val = parameters[key];
            }

            return val;
        }

        /// <summary>
        /// Get a kanban InstanceView object
        /// </summary>
        /// <param name="kanbanName">Name of the kanban</param>
        /// <returns>An InstanceView object</returns>
        private InstanceView GetKanbanInstance(string kanbanName)
        {
            using (CMConnection connection = new CMConnection(KANBAN_CONNECTION_STRING))
            {
                connection.Open();

                InstanceView instanceView = null;

                MetaDataModel metaData = connection.MetaDataModel;

                DataViewModel dataView = metaData.GetDefaultDataView(KANBAN_CLASS);

                string query = dataView.GetInstanceByAttributeValueQuery(KANBAN_NAME, kanbanName);

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
        }

        // get commands names associted with an object
        private List<string> GetCustomCommandNames(string schemaName, string className, XmlElement currentInstance)
        {
            List<string> commands = new List<string>();

            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
            schemaInfo.Name = schemaName;
            string schemaId = schemaInfo.NameAndVersion;

            CustomCommandGroup customCommandGroup = SiteMapManager.Instance.GetCustomCommandGroup(schemaId, className);

            if (customCommandGroup != null)
            {
                XaclPolicy policy = SiteMapManager.Instance.Policy;

                Hashtable customCommandPermissions = new Hashtable();

                // check the custom command access permissions
                foreach (CustomCommand customCommand in customCommandGroup.ChildNodes)
                {
                    if (!PermissionChecker.Instance.HasPermission(policy, customCommand, XaclActionType.Read))
                    {
                        customCommandPermissions[customCommand] = false;
                    }
                    else
                    {
                        customCommandPermissions[customCommand] = true;
                    }

                    if (customCommand.CommandScope == CommandScope.Instance &&
                        IsVisible(customCommand, policy, schemaName, className, currentInstance))
                    {
                        commands.Add(customCommand.Name);
                    }
                }
            }

            return commands;
        }

        // get commands associted with a class
        private List<KanbanCommandModel> GetCustomCommands(string schemaName, string className)
        {
            List<KanbanCommandModel> commands = new List<KanbanCommandModel>();

            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
            schemaInfo.Name = schemaName;
            string schemaId = schemaInfo.NameAndVersion;

            CustomCommandGroup customCommandGroup = SiteMapManager.Instance.GetCustomCommandGroup(schemaId, className);

            if (customCommandGroup != null)
            {
                XaclPolicy policy = SiteMapManager.Instance.Policy;

                Hashtable customCommandPermissions = new Hashtable();
                KanbanCommandModel command;

                // check the custom command access permissions
                foreach (CustomCommand customCommand in customCommandGroup.ChildNodes)
                {
                    if (!PermissionChecker.Instance.HasPermission(policy, customCommand, XaclActionType.Read))
                    {
                        customCommandPermissions[customCommand] = false;
                    }
                    else
                    {
                        customCommandPermissions[customCommand] = true;
                    }

                    if (customCommand.CommandScope == CommandScope.Instance)
                    {
                        command = new KanbanCommandModel();
                        command.id = customCommand.Name;
                        command.title = customCommand.Title;
                        command.icon = customCommand.ImageUrl;
                        command.url = customCommand.NavigationUrl;

                        List<CommandParameterModel> parameters = new List<CommandParameterModel>();
                        if (customCommand.StateParameters != null)
                        {
                            CommandParameterModel parameterModel;
                            foreach (StateParameter parameter in customCommand.StateParameters)
                            {
                                parameterModel = new CommandParameterModel();
                                parameterModel.name = parameter.Name;
                                parameterModel.value = parameter.Value;

                                parameters.Add(parameterModel);
                            }
                        }
                        command.parameters = parameters;

                        commands.Add(command);
                    }
                }
            }

            return commands;
        }

        // Check permission of a custom command
        private bool IsVisible(CustomCommand customCommand, XaclPolicy policy, string schemaName, string className, XmlElement currentInstance)
        {
            bool status = PermissionChecker.Instance.HasPermission(policy, customCommand, XaclActionType.Read);

            if (status &&
                !string.IsNullOrEmpty(customCommand.VisibleCondition) &&
                !MeetVisibleCondition(customCommand.VisibleCondition, schemaName, className, currentInstance))
            {
                status = false;
            }

            return status;
        }

        private bool MeetVisibleCondition(string visibleCondition, string schemaName, string className, XmlElement currentInstance)
        {
            bool status = true;
            if (currentInstance != null)
            {
                CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
                XmlElement originalInstance = principal.CurrentInstance;
                try
                {
                    if (principal != null)
                    {
                        if (currentInstance != null)
                        {
                            principal.CurrentInstance = currentInstance;

                            // build a complete xquery
                            string finalCondition = TEMPLATE_XQUERY.Replace("$$", visibleCondition);

                            IConditionRunner conditionRunner = PermissionChecker.Instance.ConditionRunner;

                            status = conditionRunner.IsConditionMet(finalCondition);
                        }
                    }
                }
                finally
                {
                    // unset the current instance as a context for condition evaluation
                    principal.CurrentInstance = originalInstance;
                }
            }

            return status;
        }

        private XmlElement GetXmlInstance(XmlDocument kanbanDataXmlDcoument, int row)
        {
            XmlElement xmlInstance = null;

            if (kanbanDataXmlDcoument != null && row < kanbanDataXmlDcoument.DocumentElement.ChildNodes.Count)
            {
                xmlInstance = (XmlElement)kanbanDataXmlDcoument.DocumentElement.ChildNodes[row];
            }

            return xmlInstance;
        }

        private XmlElement GetDataInstance(string schemaName, string className, string oid)
        {
            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                string query = dataView.GetInstanceQuery(oid);

                Interpreter interpreter = new Interpreter();
                XmlDocument doc = interpreter.Query(query);

                if (doc != null && doc.DocumentElement.ChildNodes.Count > 0)
                {
                    return (XmlElement)doc.DocumentElement.ChildNodes[0]; // return the first element to evaluate the expression
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;
using Newtera.WebForm;
using Newtera.Common.Wrapper;
using Newtera.Common.MetaData.Schema;
using Newtera.Server.Engine.Workflow;
using Newtera.WebApi.Models;

namespace Newtera.WebApi.Infrastructure
{
    /// <summary>
    /// Report template manager that provides services for gettting, adding, or delete the report templates in a local disk for data classes
    /// </summary>
    public class MessageManager
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME=COMMON;SCHEMA_VERSION=1.0";
        private const string MESSAGE_CLASS = "Messages";
        private const string MESSAGE_ID = "ID";
        private const string MESSAGE_SUBJECT = "Subject";
        private const string MESSAGE_CONTENT = "Content";
        private const string MESSAGE_POSTER = "Poster";
        private const string MESSAGE_POSTER_NAME = "PosterName";
        private const string MESSAGE_POST_TIME = "PostTime";
        private const string MESSAGE_RECEIVER = "Receiver";
        private const string MESSAGE_STATUS = "Status";
        private const string MESSAGE_URL = "url";
        private const string MESSAGE_URL_PARAMS = "Params";
        private const string MESSAGE_OWNER_SCHEMA = "Schema";
        private const string MESSAGE_OWNER_CLASS = "DBClass";
        private const string MESSAGE_OWNER_OID = "ObjId";

        /// <summary>
        /// Message manager that provides services for posting gettting, adding, and delete the messages.
        /// </summary>
        public MessageManager()
        {
        }

        /// <summary>
        /// Get a list of MessageModel objects for the calling user
        /// </summary>
        /// <returns>A list of ImageInfo objects</returns>
        public async Task<IEnumerable<MessageModel>> GetMessages()
        {
            List<MessageModel> messages = new List<MessageModel>();

            await Task.Factory.StartNew(() =>
            {
                messages = GetUserMessages();
            });

            return messages;
        }

        /// <summary>
        /// Get count of MessageModel objects for the calling user
        /// </summary>
        /// <returns>An integer</returns>
        public async Task<Int32> GetMessageCount()
        {
            int count = 0;

            await Task.Factory.StartNew(() =>
            {
                count = GetUserMessageCount();
            });

            return count;
        }

        /// <summary>
        /// Add a MessageModel object for the calling user
        /// </summary>
        /// <param name="message">the message object</param>
        /// <returns>An integer</returns>
        public async Task<string> AddMessage(Models.MessageModel message)
        {
            string id = null;

            await Task.Factory.StartNew(() =>
            {
                id = CreateMessage(message);
            });

            return id;
        }

        /// <summary>
        /// Remove a message of given obj_id
        /// </summary>
        /// <param name="oid">the message obj_id</param>
        public async void RemoveMessage(string oid)
        {
            await Task.Factory.StartNew(() =>
            {
                DeleteMessage(oid);
            });
        }

        /// <summary>
        /// Clear all messages of the calling user
        /// </summary>
        public async void ClearMessages()
        {
            List<MessageModel> messages = GetUserMessages();
            await Task.Factory.StartNew(() =>
            {
                RemoveUserMessages(messages);
            });
        }

        /// <summary>
        /// Create a message in database for an user
        /// </summary>
        /// <param name="userName">the receiver of the message</param>
        /// <param name="messageInfo">MessageInfo object</param>
        public void AddMessage(string userName,  Newtera.WFModel.MessageInfo messageInfo)
        {
            // convert a MessageInfo to JObject
            JObject message = new JObject();

            message["Receiver"] = userName;
            message["Subject"] = messageInfo.Subject;
            message["Content"] = messageInfo.Content;
            message["Poster"] = messageInfo.SenderName;
            message["PostTime"] = messageInfo.SendTime;
            message["Status"] = "0"; //unread
            message["url"] = messageInfo.Url;
            message["Params"] = messageInfo.UrlParams;
            message["Schema"] = messageInfo.SchemaName;
            message["DBClass"] = messageInfo.ClassName;
            message["ObjId"] = messageInfo.ObjId;

            CreateMessage(message);
        }

        /// <summary>
        /// Return a list of MessageModel of the calling user
        /// </summary>
        /// <returns>List of MessageModel objects</returns>
        private List<MessageModel> GetUserMessages()
        {
            List<MessageModel> messageModels = new List<MessageModel>();

            ServerSideUserManager userManager = new ServerSideUserManager();

            using (CMConnection con = new CMConnection(CONNECTION_STRING))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(MESSAGE_CLASS);

                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                if (principal == null)
                {
                    throw new InvalidOperationException("The user has not been authenticated");
                }

                string userName = principal.Identity.Name;

                dataView.SetSearchValue(dataView.BaseClass.Alias, MESSAGE_RECEIVER, userName);

                dataView.PageSize = 100; // get first 100 messages for a given user

                string query = dataView.SearchQuery;

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    InstanceView instanceView = new InstanceView(dataView, ds);

                    int count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                    MessageModel message;
                    for (int row = 0; row < count; row++)
                    {
                        instanceView.SelectedIndex = row;

                        message = new MessageModel();
                        message.id = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_ID);
                        message.objId = instanceView.InstanceData.ObjId;
                        message.subject = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_SUBJECT); ;
                        message.content = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_CONTENT);
                        message.posterId = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_POSTER);
                        message.poster = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_POSTER_NAME);
                        message.postTime = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_POST_TIME);
                        message.status = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_STATUS);
                        message.url = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_URL);
                        message.urlparams = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_URL_PARAMS);
                        message.dbschema = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_OWNER_SCHEMA);
                        message.dbclass = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_OWNER_CLASS);
                        message.oid = instanceView.InstanceData.GetAttributeStringValue(MESSAGE_OWNER_OID);

                        messageModels.Add(message);
                    }
                }
            }

            return messageModels;
        }

        /// <summary>
        /// Return count of MessageModel of the calling user
        /// </summary>
        /// <returns>An integer</returns>
        private int GetUserMessageCount()
        {
            int count = 0;

            using (CMConnection con = new CMConnection(CONNECTION_STRING))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(MESSAGE_CLASS);

                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                if (principal == null)
                {
                    throw new InvalidOperationException("The user has not been authenticated");
                }

                string userName = principal.Identity.Name;

                dataView.SetSearchValue(dataView.BaseClass.Alias, MESSAGE_RECEIVER, userName);

                dataView.PageSize = 100; // get first 100 messages for a given user

                string query = dataView.SearchQuery;

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                count = cmd.ExecuteCount();
            }

            return count;
        }

        /// <summary>
        /// Create a message in database for calling user
        /// </summary>
        /// <param name="messageModel">json message object</param>
        /// <returns>Message's obj_id</returns>
        private string CreateMessage(Models.MessageModel messageModel)
        {
            JObject message = new JObject();

            message["Receiver"] = messageModel.receiver;
            message["Subject"] = messageModel.subject;
            message["Content"] = messageModel.content;
            message["Poster"] = messageModel.poster;
            message["PostTime"] = messageModel.postTime;
            message["Status"] = messageModel.status;
            message["url"] = messageModel.url;
            message["Params"] = messageModel.urlparams;
            message["Schema"] = messageModel.dbschema;
            message["DBClass"] = messageModel.dbclass;
            message["ObjId"] = messageModel.objId;

            return CreateMessage(message);
        }

        /// <summary>
        /// Create a message in database
        /// </summary>
        /// <param name="message">json message object</param>
        /// <returns>Message's obj_id</returns>
        private string CreateMessage(JObject message)
        {
            string oid = "";
            string subject = null;

            if (message.GetValue("Subject") != null)
            {
                subject = message.GetValue("Subject").ToString();
            }

            //var jsonString = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented);
            //ErrorLog.Instance.WriteLine(jsonString);

            if (!string.IsNullOrEmpty(subject))
            {
                using (CMConnection con = new CMConnection(CONNECTION_STRING))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(MESSAGE_CLASS);

                    // Create an instance view
                    InstanceView instanceView = new InstanceView(dataView);

                    InstanceEditor instanceEditor = new InstanceEditor();
                    instanceEditor.EditInstance = instanceView;
                    instanceEditor.ConvertToModel(message); // // translate the JSON instance data to InstanceView instance data

                    // run initialzing code
                    RunBeforeInsertCode(con, instanceView, message);

                    string query = instanceView.DataView.InsertQuery;

                    CMCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    XmlDocument doc = cmd.ExecuteXMLDoc(); // execute insert query

                    oid = doc.DocumentElement.InnerText;
                }
            }

            return oid;
        }

        /// <summary>
        /// Run the before insert code on the instance
        /// </summary>
        private void RunBeforeInsertCode(CMConnection connection, InstanceView instanceView, dynamic instance)
        {
            ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.Name);

            // Execute the before insert code
            if (!string.IsNullOrEmpty(classElement.BeforeInsertCode))
            {
                // Execute the before updare code
                IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView, connection.ConnectionString, instance); ;

                ActionCodeRunner.Instance.ExecuteActionCode("ClassInsert" + classElement.ID, classElement.BeforeInsertCode, instanceWrapper);
            }
        }

        /// <summary>
        /// delete a message in database
        /// </summary>
        /// <param name="oid">message's obj_id</param>
        private void DeleteMessage(string oid)
        {
            using (CMConnection con = new CMConnection(CONNECTION_STRING))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(MESSAGE_CLASS);

                // create a delete query
                dataView.CurrentObjId = oid;

                string query = dataView.DeleteQuery;

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                cmd.ExecuteXMLDoc();
            }
        }

        /// <summary>
        /// remove an user's messages in database
        /// </summary>
        /// <param name="messages">User's messages</param>
        private void RemoveUserMessages(List<MessageModel> messages)
        {
            using (CMConnection con = new CMConnection(CONNECTION_STRING))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(MESSAGE_CLASS);
                string query;

                foreach (MessageModel message in messages)
                {
                    // create a delete query
                    dataView.CurrentObjId = message.objId;

                    query = dataView.DeleteQuery;

                    CMCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    cmd.ExecuteXMLDoc();
                }
            }
        }
    }
}
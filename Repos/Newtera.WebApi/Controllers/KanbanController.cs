using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;

using Swashbuckle.Swagger.Annotations;

using Newtera.WebApi.Infrastructure;
using Newtera.WebApi.Models;
using Newtera.Common.Core;
using Newtera.Common.MetaData.SiteMap;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Data;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.DataView;
using Newtera.Server.Engine.Cache;
using Newtera.Server.Engine.Interpreter;
using Newtera.Common.MetaData.XaclModel.Processor;
using Newtera.WebApi.Hubs;

namespace Newtera.WebApi.Controllers
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

        /// <summary>
        /// Gets data for a kanban for given requesting user
        /// </summary>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("data/{schemaName}/{dbclass}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(KanbanModel), Description = "A kanban data model for the request user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetKanbanDataModel(string schemaName, string dbclass)
        {
            QueryHelper queryHelper = new QueryHelper();
            KanbanModel kanbanModel = new KanbanModel();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(dbclass);

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        var groupAttribute = GetParamValue(parameters, "group", null);
                        var stateAttribute = GetParamValue(parameters, "state", null);
                        var idAttribute = GetParamValue(parameters, "ID", null);
                        var titleAttribute = GetParamValue(parameters, "title", null);
                        // stateMapping is used to make data instance's state to the display state
                        var stateMapping = GetParamValue(parameters, "stateMapping", null);

                        // get obj_ids of the groups which the connecting user is tracking on
                        var itemObjIds = GetKanbanItemObjIds(schemaName, dbclass);

                        if (itemObjIds.Count > 0)
                        {
                            XmlDocument itemDataXmlDcoument = GetKanbanDataXmlDocument(con, dataView, itemObjIds);

                            // convert doc to instance view
                            XmlReader reader = new XmlNodeReader(itemDataXmlDcoument);
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                            {
                                var itemInstanceView = new InstanceView(dataView, ds);

                                BuildKanbanModel(kanbanModel, itemInstanceView, groupAttribute, stateAttribute, idAttribute, titleAttribute, stateMapping);
                            }
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

        private void BuildKanbanModel(KanbanModel kanbanModel, InstanceView itemInstanceView, string groupAttribute, string stateAttribute, string idAttribute, string titleAttribute, string stateMapping)
        {
            var stateLookup = BuildKanbanStates(stateMapping);

            foreach (var key in stateLookup.Keys)
            {
                var stateModel = new KanbanStateModel(key.GetHashCode().ToString(), key);
                kanbanModel.States.Add(stateModel);
            }

            for (int i = 0; i < itemInstanceView.InstanceCount; i++)
            {
                itemInstanceView.SelectedIndex = i;

                var item = BuildKanbanItemModel(itemInstanceView, groupAttribute, stateAttribute, idAttribute, titleAttribute);
                var groupModel = kanbanModel.Groups.Where(g => g.Name == item.GroupName).FirstOrDefault();
                if (groupModel == null)
                {
                    groupModel = new KanbanGroupModel(item.GroupName.GetHashCode().ToString(), item.GroupName);
                    kanbanModel.Groups.Add(groupModel);
                }

                item.GroupId = groupModel.Id; // assign a group id to item
                kanbanModel.Items.Add(item);

                foreach (var key in stateLookup.Keys)
                {
                    var actualStates = stateLookup[key];
                    if (actualStates.Contains(item.StateName))
                    {
                        var stateModel = kanbanModel.States.Where(s => s.Name == key).FirstOrDefault();
                        if (stateModel != null)
                        {
                            item.StateId = stateModel.Id; // assign a state id to item
                        }
                    }
                }
            }
        }

        private IDictionary<string, IList<string>> BuildKanbanStates(string stateMapping)
        {
            var stateLookup = new Dictionary<string, IList<string>>();

            // Parse stateMapping to build state lookup table
            // e.g. ToDo:New,Submitted;In Progress:Processing;Completed:Done,Resolved;Others:default
            if (!string.IsNullOrEmpty(stateMapping))
            {
                var keyValuePairs = stateMapping.Split(';');
                foreach (string keyValue in keyValuePairs)
                {
                    var kv = keyValue.Trim();
                    var index = kv.IndexOf(':');
                    if (index > 0)
                    {
                        var displayStateName = kv.Substring(0, index);
                        var stateValue = kv.Substring(index + 1);
                        if (!stateLookup.ContainsKey(displayStateName))
                        {
                            stateLookup[displayStateName] = new List<string>();
                        }
                        var actualStates = stateValue.Split(',');
                        foreach (var actualState in actualStates)
                        {
                            stateLookup[displayStateName].Add(actualState);
                        }
                    }
                }
            }

            return stateLookup;
        }

        private KanbanItemModel BuildKanbanItemModel(InstanceView kanbanInstanceView, string groupAttribute, string stateAttribute, string idAttribute, string titleAttribute)
        {
            var item = new KanbanItemModel();
            item.ObjId = kanbanInstanceView.InstanceData.ObjId;
            item.Name = kanbanInstanceView.InstanceData.GetAttributeStringValue(idAttribute);
            item.Title = kanbanInstanceView.InstanceData.GetAttributeStringValue(titleAttribute);
            item.GroupName = kanbanInstanceView.InstanceData.GetAttributeStringValue(groupAttribute);
            item.StateName = kanbanInstanceView.InstanceData.GetAttributeStringValue(stateAttribute);
            item.Track = true;
            return item;
        }

        private StringCollection GetKanbanItemObjIds(string schemaName, string className)
        {
            StringCollection itemObjIds = new StringCollection();

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            string userName = principal.Identity.Name;

            MessageHubMaster messageHubMaster = MessageHubMaster.Instance;

            List<string> itemNames = messageHubMaster.GetUserGroups(schemaName, className, userName, 0, 200);

            int pos;
            foreach (string itemName in itemNames)
            {
                // itemName is in form of schemaName-className-objId
                pos = itemName.LastIndexOf('-');
                if (pos > 0)
                {
                    itemObjIds.Add(itemName.Substring(pos + 1)); // get obj_id from the string
                }
            }

            return itemObjIds;
        }

        private XmlDocument GetKanbanDataXmlDocument(CMConnection con, DataViewModel dataView, StringCollection itemIds)
        {
            XmlDocument doc = null;

            string query = null;
            if (itemIds != null && itemIds.Count == 1)
            {
                // get a single instance by the objId
                query = dataView.GetInstanceQuery(itemIds[0]);
            }
            else if (itemIds != null && itemIds.Count > 1)
            {
                query = dataView.GetInstancesQuery(itemIds);
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
    }
}
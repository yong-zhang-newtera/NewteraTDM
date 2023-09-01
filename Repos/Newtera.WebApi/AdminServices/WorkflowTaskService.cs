using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Xml;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Description;

using Newtera.Data;
using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;
using Newtera.WFModel;
using Newtera.WorkflowServices;
using Newtera.Server.Engine.Workflow;
using Newtera.Server.UsrMgr;
using Newtera.WebApi.Infrastructure;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// Represents a service that perform meta-data related tasks for admin tools
    /// </summary>
    /// <version>  	1.0.0 01 April 2016 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/workflowTaskService")]
    public class WorkflowTaskServiceController : ApiController
    {
        /// <summary>
        /// Get count of tasks that are associated with an user
        /// </summary>
        /// <param name="connectionStr">The connection string, contains user id</param>
        /// <param name="userName">User name</param>
        /// <returns>A count</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetTaskCount/{userName}")]
        public HttpResponseMessage GetTaskCount(string userName)
        {
            try
            {
                int totalCount = 0;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                    schemaInfo.Name = con.SchemaName;
                    if (!string.IsNullOrEmpty(con.SchemaVersion))
                    {
                        schemaInfo.Version = con.SchemaVersion;
                    }
                    else
                    {
                        schemaInfo.Version = "1.0";
                    }
                    string schemaId = schemaInfo.NameAndVersion;

                    ITaskService taskService = new NewteraTaskService();

                    List<TaskInfo> tasks = taskService.GetUserTasks(schemaId, userName, new ServerSideUserManager());

                    totalCount = tasks.Count;

                    return Request.CreateResponse(HttpStatusCode.OK, totalCount);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Get a xml string representing a list of TaskInfo objects that are associated with an user
        /// </summary>
        /// <param name="connectionStr">The connection string, contains user id</param>
        /// <param name="userName">The suer name</param>
        /// <returns>a xml string</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetTaskInfos/{userName}")]
        public HttpResponseMessage GetTaskInfos(string userName)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    int count;

                    Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                    schemaInfo.Name = con.SchemaName;
                    schemaInfo.Version = con.SchemaVersion;
                    string schemaId = schemaInfo.NameAndVersion;

                    ITaskService taskService = new NewteraTaskService();

                    List<TaskInfo> tasks = taskService.GetUserTasks(schemaId, userName, new ServerSideUserManager());

                    // convert to xml
                    DataTable dt = new DataTable();

                    // create columns of datatable
                    dt.Columns.Add("TaskID");
                    dt.Columns.Add("Subject");
                    dt.Columns.Add("CreateTime");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("WorkflowInstanceId");

                    DataRow row;

                    foreach (TaskInfo task in tasks)
                    {
                        row = dt.NewRow();
                        row["TaskID"] = task.TaskId;
                        row["Subject"] = task.Subject;
                        row["CreateTime"] = task.CreateTime;
                        row["Description"] = task.Description;
                        row["WorkflowInstanceId"] = task.WorkflowInstanceId;

                        dt.Rows.Add(row);
                    }

                    string xml = null;
                    using (StringWriter sw = new StringWriter())
                    {
                        dt.WriteXml(sw);
                        xml = sw.ToString();
                    }

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                    }
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets information indicating whether the given schema it a valid schema
        /// </summary>
        /// <param name="schemaName">The given schema name</param>
        /// <param name="schemaVersion">The given schema version, can be null, default is 1.0</param>
        /// <returns>True if it is a valid schema, false otherwise.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("IsValidSchema/{schemaName}/{schemaVersion}")]
        public HttpResponseMessage IsValidSchema(string schemaName, string schemaVersion)
        {
            try
            {
                bool status = false;

                if (!string.IsNullOrEmpty(schemaName))
                {
                    using (CMConnection con = new CMConnection())
                    {
                        SchemaInfo[] allSchemas = con.AllSchemas;

                        foreach (SchemaInfo existSchema in allSchemas)
                        {
                            if (schemaName.Trim() == existSchema.Name)
                            {
                                if (!string.IsNullOrEmpty(schemaVersion) && schemaVersion == existSchema.Version)
                                {
                                    status = true;
                                    break;
                                }
                                else if (string.IsNullOrEmpty(schemaVersion) &&
                                    (existSchema.Version == "1.0" || string.IsNullOrEmpty(existSchema.Version)))
                                {
                                    status = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, status);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }
    }
}
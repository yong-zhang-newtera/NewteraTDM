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
    /// The service that provides caching, id generation and other api if the server is a master server
    /// </summary>
    /// <version>  	1.0.0 11 Nov 2018 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/mastercache")]
    public class MasterServerServiceController : ApiController
    {
        /// <summary>
        /// Get a xml string representing a list of TaskInfo objects that are associated with an user
        /// </summary>
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
    }
}
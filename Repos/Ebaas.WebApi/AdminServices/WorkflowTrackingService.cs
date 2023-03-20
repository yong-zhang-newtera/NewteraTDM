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

using Newtera.WFModel;
using Newtera.Data;
using Newtera.Common.Core;
using Ebaas.WebApi.Infrastructure;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// Represents a service that perform meta-data related tasks for admin tools
    /// </summary>
    /// <version>  	1.0.0 01 April 2016 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/workflowTrackingService")]
    public class WorkflowTrackingServiceController : ApiController
    {
        /// <summary>
        /// Gets count of for NewteraTrackingWorkflowInstance a given workflow type from tracking service located
        /// at the server
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="workflowTypeId">The unique workflow type internal id</param>
        /// <param name="workflowEvent">The type of workflow event</param>
        /// <param name="from">Start time</param>
        /// <param name="until">Until time</param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <returns>The workflow instance count</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("GetTrackingWorkflowInstanceCount/{workflowTypeId}")]
        public HttpResponseMessage GetTrackingWorkflowInstanceCount(string workflowTypeId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string content = Request.Content.ReadAsStringAsync().Result;
                string[] apiParams = JsonConvert.DeserializeObject<string[]>(content);
                string workflowEvent = apiParams[0];
                DateTime from;
                try
                {
                    from = DateTime.Parse(apiParams[1]);
                }
                catch (Exception)
                {
                    from = DateTime.Now;
                }

                DateTime until;
                try
                {
                    until = DateTime.Parse(apiParams[2]);
                }
                catch (Exception)
                {
                    until = DateTime.Now;
                }

                bool useCondition;
                try
                {
                    useCondition = bool.Parse(apiParams[3]);
                }
                catch (Exception)
                {
                    useCondition = false;
                }

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    int count = con.GetTrackingWorkflowInstanceCount(workflowTypeId, workflowEvent, from, until, useCondition);

                    return Request.CreateResponse(HttpStatusCode.OK, count);
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
        /// Gets a collection of NewteraTrackingWorkflowInstance in xml for a given workflow type from tracking service
        /// </summary>
        /// <param name="workflowTypeId">The unique workflow type internal id</param>
        /// <param name="from">Start time</param>
        /// <param name="until">Until time</param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>A xml string that can be restored as a NewteraTrackingWorkflowInstanceCollection object</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("GetTrackingWorkflowInstances/{workflowTypeId}")]
        public HttpResponseMessage GetTrackingWorkflowInstances(string workflowTypeId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string content = Request.Content.ReadAsStringAsync().Result;
                string[] apiParams = JsonConvert.DeserializeObject<string[]>(content);
                string workflowEvent = apiParams[0];
                DateTime from;
                try
                {
                    from = DateTime.Parse(apiParams[1]);
                }
                catch (Exception)
                {
                    from = DateTime.Now;
                }

                DateTime until;
                try
                {
                    until = DateTime.Parse(apiParams[2]);
                }
                catch (Exception)
                {
                    until = DateTime.Now;
                }

                bool useCondition;
                try
                {
                    useCondition = bool.Parse(apiParams[3]);
                }
                catch (Exception)
                {
                    useCondition = false;
                }

                int pageIndex;
                try
                {
                    pageIndex = int.Parse(apiParams[4]);
                }
                catch (Exception)
                {
                    pageIndex = 0;
                }

                int pageSize;
                try
                {
                    pageSize = int.Parse(apiParams[5]);
                }
                catch (Exception)
                {
                    pageSize = 10;
                }

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);

                    NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = con.GetTrackingWorkflowInstances(workflowTypeId, workflowEvent, from, until, useCondition, pageIndex, pageSize);
                    trackingWorkflowInstances.Write(writer);

                    // return the xml string
                    string xml = builder.ToString();

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
        /// Gets a NewteraTrackingWorkflowInstance in xml for a given workflow instance from the tracking service.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>A xml string that can be restored as a NewteraTrackingWorkflowInstanceCollection object</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetTrackingWorkflowInstance/{workflowInstanceId}")]
        public HttpResponseMessage GetTrackingWorkflowInstance(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);

                    NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = con.GetTrackingWorkflowInstances(workflowInstanceId);
                    trackingWorkflowInstances.Write(writer);

                    // return the xml string
                    string xml = builder.ToString();

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
        /// Cancel a workflow instance that is awaiting an event.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("CancelWorkflowInstance/{workflowInstanceId}")]
        public HttpResponseMessage CancelWorkflowInstance(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.CancelWorkflowInstance(workflowInstanceId);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Cancel a workflow activity that is awaiting an event.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <param name="activityName">Name of an activity to be cancelled.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("CancelActivity/{workflowInstanceId}/{activityName}")]
        public HttpResponseMessage CancelActivity(string workflowInstanceId, string activityName)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.CancelActivity(workflowInstanceId, activityName);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Delete tracking data from the database of a workflow instance.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("DeleteTrackingWorkflowInstance/{workflowInstanceId}")]
        public HttpResponseMessage DeleteTrackingWorkflowInstance(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.DeleteTrackingWorkflowInstance(workflowInstanceId);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Get a list of TaskInfo objects that are associated with a workflow instance
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>A list of TaskInfo objects</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetTaskInfos/{workflowInstanceId}")]
        public HttpResponseMessage GetTaskInfos(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    List<TaskInfo> taskInfos = con.GetTaskInfos(workflowInstanceId);

                    return Request.CreateResponse(HttpStatusCode.OK, taskInfos);
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
        /// Get information of the data instance that has been bound to a workflow instance.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The binding info, null if no binding data instance exists.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetBindingDataInstanceInfo/{workflowInstanceId}")]
        public HttpResponseMessage GetBindingDataInstanceInfo(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    WorkflowInstanceBindingInfo bindingInfo = con.GetBindingDataInstanceInfo(workflowInstanceId);

                    return Request.CreateResponse(HttpStatusCode.OK, bindingInfo);
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
        /// Get ids of the data instances that has been bound to workflow instances.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="schemaId"> Indicate the database where the data instances exist</param>
        /// <param name="pageSize"> The number of ids returned each call</param>
        /// <param name="pageIndex"> The index of current page</param>
        /// <returns>A string array of data instance ids, null if it reaches the end of result.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetBindingDataInstanceIds/{schemaId}/{pageSize}/{pageIndex}")]
        public HttpResponseMessage GetBindingDataInstanceIds(string schemaId, int pageSize, int pageIndex)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string[] objIds = con.GetBindingDataInstanceIds(schemaId, pageSize, pageIndex);

                    return Request.CreateResponse(HttpStatusCode.OK, objIds);
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
        /// Replace the old id of a binding data instance with a new id. This method is used when
        /// restore a database from a backup file in which new ids are created for each data instance.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="oldInstanceId"> The old instance id</param>
        /// <param name="newInstanceId"> The new instance id</param>        
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ReplaceBindingDataInstanceId/{oldInstanceId}/{newInstanceId}")]
        public HttpResponseMessage ReplaceBindingDataInstanceId(string oldInstanceId, string newInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.ReplaceBindingDataInstanceId(oldInstanceId, newInstanceId);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Get the state information of a workflow instance.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The state info.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetWorkflowInstanceStateInfo/{workflowInstanceId}")]
        public HttpResponseMessage GetWorkflowInstanceStateInfo(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    WorkflowInstanceStateInfo stateInfo = con.GetWorkflowInstanceStateInfo(workflowInstanceId);

                    return Request.CreateResponse(HttpStatusCode.OK, stateInfo);
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
        /// Get a collection of database event subscriptions of a workflow instance.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The state info.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetDBEventSubscriptions/{workflowInstanceId}")]
        public HttpResponseMessage GetDBEventSubscriptions(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    DBEventSubscriptionCollection subscriptions = con.GetDBEventSubscriptions(workflowInstanceId);

                    return Request.CreateResponse(HttpStatusCode.OK, subscriptions);
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
        /// Get a collection of workflow event subscriptions of a workflow instance.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The state info.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetWorkflowEventSubscriptions/{workflowInstanceId}")]
        public HttpResponseMessage GetWorkflowEventSubscriptions(string workflowInstanceId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    WorkflowEventSubscriptionCollection subscriptions = con.GetWorkflowEventSubscriptions(workflowInstanceId);

                    return Request.CreateResponse(HttpStatusCode.OK, subscriptions);
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
        /// Sets a xml string representing a collection of NewteraTrackingWorkflowInstance
        /// for a given workflow type. Used by the project restore tool
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="workflowTypeId">The unique workflow internal type id</param>
        /// <param name="xmlString">a xml string representing a collection of NewteraTrackingWorkflowInstance.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetTrackingWorkflowInstances/{workflowTypeId}")]
        public HttpResponseMessage SetTrackingWorkflowInstances(string workflowTypeId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    NewteraTrackingWorkflowInstanceCollection workflowTrackingInfos = new NewteraTrackingWorkflowInstanceCollection();
                    workflowTrackingInfos.Load(xmlString);

                    con.SetTrackingWorkflowInstances(workflowTypeId, workflowTrackingInfos);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Sets a workflow instance state info to the database. Used by the project restore tool
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="stateInfo">A workflow instance state info.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetWorkflowInstanceStateInfo")]
        public HttpResponseMessage SetWorkflowInstanceStateInfo()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string content = Request.Content.ReadAsStringAsync().Result;
                WorkflowInstanceStateInfo stateInfo = JsonConvert.DeserializeObject<WorkflowInstanceStateInfo>(content);

                //WorkflowInstanceStateInfo stateInfo = Request.Content.ReadAsStringAsync().Result;
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.SetWorkflowInstanceStateInfo(stateInfo);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Sets a collection of a workflow instance's subscriptions to workflow events. Used by the project restore tool
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="xmlString">A xml string representing a collection of subscriptions to workflow events.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetWorkflowEventSubscriptions")]
        public HttpResponseMessage SetWorkflowEventSubscriptions()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    WorkflowEventSubscriptionCollection subscriptions = new WorkflowEventSubscriptionCollection();
                    subscriptions.Load(xmlString);

                    con.SetWorkflowEventSubscriptions(subscriptions);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Sets a collection of a workflow instance's subscriptions to database events. Used by the project restore tool
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="xmlString">A xml string representing a collection of subscriptions to database events.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetDBEventSubscriptions")]
        public HttpResponseMessage SetDBEventSubscriptions()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    DBEventSubscriptionCollection subscriptions = new DBEventSubscriptionCollection();
                    subscriptions.Load(xmlString);

                    con.SetDBEventSubscriptions(subscriptions);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Sets a collection of a workflow instance's binding to database instances. Used by the project restore tool
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="workflowTypeId">The unique workflow internal type id</param>
        /// <param name="xmlString">A xml string representing a collection of a workflow instance's binding to database instances.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetBindingDataInstanceInfos/{workflowTypeId}")]
        public HttpResponseMessage SetBindingDataInstanceInfos(string workflowTypeId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    WorkflowInstanceBindingInfoCollection bindingInfos = new WorkflowInstanceBindingInfoCollection();
                    bindingInfos.Load(xmlString);

                    con.SetBindingDataInstanceInfos(workflowTypeId, bindingInfos);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Sets a collection of a workflow instance's task infos. Used by the project restore tool
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="taskInfos">A xml string representing a collection of a workflow instance's task infos.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetTaskInfos")]
        public HttpResponseMessage SetTaskInfos()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    TaskInfoCollection taskInfos = new TaskInfoCollection();
                    taskInfos.Load(xmlString);

                    con.SetTaskInfos(taskInfos);

                    return Request.CreateResponse(HttpStatusCode.OK);
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
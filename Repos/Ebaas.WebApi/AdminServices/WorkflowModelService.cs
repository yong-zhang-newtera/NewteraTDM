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
    [RoutePrefix("api/workflowModelService")]
    public class WorkflowModelServiceController : ApiController
    {
        /// <summary>
        /// Gets project infos of the existing workflow projects that exist in the database.
        /// </summary>
        /// <returns>An array of ProjectInfo</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetExistingProjectInfos")]
        public HttpResponseMessage GetExistingProjectInfos()
        {
            try
            {
                using (WFConnection con = new WFConnection())
                {
                    ProjectInfo[] projects = con.WorkflowProjectInfos;

                    return Request.CreateResponse(HttpStatusCode.OK, projects);
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
        /// get the project model from database as xml string
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetProject/{projectName}/{projectVersion}")]
        public HttpResponseMessage GetProject(string projectName, string projectVersion)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string xml = con.GetProject(projectName, projectVersion);

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
        /// Get a project's access control policy from database as xml string
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetProjectPolicy/{projectName}/{projectVersion}")]
        public HttpResponseMessage GetProjectPolicy(string projectName, string projectVersion)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string xml = con.GetProjectPolicy(projectName, projectVersion);

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
        /// Save the project model to database
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <returns>The modified time of projetc model</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SaveProject/{projectName}/{projectVersion}")]
        public HttpResponseMessage SaveProject(string projectName, string projectVersion)
        {
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();

            string connectionStr = parameters["connectionStr"];
            string content = Request.Content.ReadAsStringAsync().Result;
            string[] apiParams = JsonConvert.DeserializeObject<string[]>(content);
            string projectXml = apiParams[0];
            string policyXml = apiParams[1];

            using (WFConnection con = new WFConnection(connectionStr))
            {
                con.Open();

                try
                {
                    DateTime modifiedTime = con.UpdateProject(projectName, projectVersion, projectXml, policyXml);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    resp.Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
            }
        }

        /// <summary>
        /// Delete a project model from database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The name of project to be deleted</param>
        /// <param name="projectVersion">The project version</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("DeleteProject/{projectName}/{projectVersion}")]
        public HttpResponseMessage DeleteProject(string projectName, string projectVersion)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.DeleteProject(projectName, projectVersion);
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
        /// Gets the information indicating whether a workflow has rules data
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">Project version</param>
        /// <param name="workflowName">The workflow name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("HasRules/{projectName}/{projectVersion}/{workflowName}")]
        public HttpResponseMessage HasRules(string projectName, string projectVersion, string workflowName)
        {
            try
            {
                bool status = true;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string dataStr = con.GetWorkflowData(projectName, projectVersion, workflowName, "Rules");
                    if (dataStr == null || dataStr.Length == 0)
                    {
                        status = false;
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

        /// <summary>
        /// Gets the information indicating whether a workflow has layout data
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("HasLayout/{projectName}/{projectVersion}/{workflowName}")]
        public HttpResponseMessage HasLayout(string projectName, string projectVersion, string workflowName)
        {
            try
            {
                bool status = true;
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string data = con.GetWorkflowData(projectName, projectVersion, workflowName, "Layout");
                    if (data == null || data.Length == 0)
                    {
                        status = false;
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

        /// <summary>
        /// Gets the indicated data of the given workflow from database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataTypestr">Data type string</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetWorkflowData/{projectName}/{projectVersion}/{workflowName}/{dataTypestr}")]
        public HttpResponseMessage GetWorkflowData(string projectName, string projectVersion,
            string workflowName, string dataTypestr)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string data = con.GetWorkflowData(projectName, projectVersion, workflowName, dataTypestr);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(data))
                    {
                        resp.Content = new StringContent(data, System.Text.Encoding.UTF8, "text/plain");
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
        /// Sets the indicated data of the given workflow to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataTypestr">Data type string indicate whether it is a xoml, layout, rules, or code</param>
        /// <param name="dataString">The data string</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetWorkflowData/{projectName}/{projectVersion}/{workflowName}/{dataTypestr}")]
        public HttpResponseMessage SetWorkflowData(string projectName, string projectVersion,
            string workflowName, string dataTypestr)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string dataString = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.SetWorkflowData(projectName, projectVersion, workflowName, dataTypestr, dataString);
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
        /// Start a workflow instance
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <returns>The id of the started workflow instance</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("StartWorkflow/{projectName}/{projectVersion}/{workflowName}")]
        public HttpResponseMessage StartWorkflow(string projectName, string projectVersion, string workflowName)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string wfId = con.StartWorkflow(projectName, projectVersion, workflowName).ToString();

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(wfId))
                    {
                        resp.Content = new StringContent(wfId, System.Text.Encoding.UTF8, "text/plain");
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
        /// Lock specified project for update.
        /// </summary>
        /// <param name="projectName">Name of the project to be locked</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="connectionStr">The connection string</param>
        /// <exception cref="LockProjectException">Thrown if the project has been locked by another user</exception>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("LockProject/{projectName}/{projectVersion}")]
        public HttpResponseMessage LockProject(string projectName, string projectVersion)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.LockProject(projectName, projectVersion);
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
        /// Unlock the specified project.
        /// </summary>
        /// <param name="projectName">Name of the project to be unlocked</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        /// <exception cref="LockProjectException">Thrown if the user does have the permission to unlock the project.</exception>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("UnlockProject/{projectName}/{projectVersion}/{forceUnlock}")]
        public HttpResponseMessage UnlockProject(string projectName, string projectVersion, bool forceUnlock)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.UnlockProject(projectName, projectVersion, forceUnlock);
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
        /// Gets authorized schema infos.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <returns>An array of SchemaInfo instances</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetAuthorizedSchemaInfos")]
        public HttpResponseMessage GetAuthorizedSchemaInfos()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    SchemaInfoCollection schemas = con.AuthorizedSchemas;

                    SchemaInfo[] authorizedSchemas = new SchemaInfo[schemas.Count];
                    int index = 0;
                    foreach (SchemaInfo schemaInfo in schemas)
                    {
                        authorizedSchemas[index] = schemaInfo;
                        index++;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, authorizedSchemas);
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
        /// Gets name of the role that has permission to modify the project
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetDBARole/{projectName}/{projectVersion}")]
        public HttpResponseMessage GetDBARole(string projectName, string projectVersion)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string role = con.GetDBARole(projectName, projectVersion);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(role))
                    {
                        resp.Content = new StringContent(role, System.Text.Encoding.UTF8, "text/plain");
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
        /// Sets name of the role that has permission to modify the project
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="role">The role</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetDBARole/{projectName}/{projectVersion}")]
        public HttpResponseMessage SetDBARole(string projectName, string projectVersion)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string role = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.SetDBARole(projectName, projectVersion, role);
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
        /// Gets the information indicating whether a workflow model has running instances.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowId">The workflow Id</param>
        /// <returns>true if it has running instances, false otherwise.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("HasRunningInstances/{projectName}/{projectVersion}/{workflowId}")]
        public HttpResponseMessage HasRunningInstances(string projectName, string projectVersion, string workflowId)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    bool status = con.HasRunningInstances(projectName, projectVersion, workflowId);

                    return Request.CreateResponse(HttpStatusCode.OK, status);
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
        /// Gets the id of a workflow model given the name.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow Name</param>
        /// <returns>The id of the found workflow model, null if the workflow model does not exist.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetWorkflowModelID/{projectName}/{projectVersion}/{workflowName}")]
        public HttpResponseMessage GetWorkflowModelID(string projectName, string projectVersion, string workflowName)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string id = con.GetWorkflowModelID(projectName, projectVersion, workflowName);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(id))
                    {
                        resp.Content = new StringContent(id, System.Text.Encoding.UTF8, "text/plain");
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
        /// Gets the information indicating whether a project is the latest version.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <returns>true if it has running instances, false otherwise.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("IsLatestVersion/{projectName}/{projectVersion}")]
        public HttpResponseMessage IsLatestVersion(string projectName, string projectVersion)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    bool status = con.IsLatestVersion(projectName, projectVersion);

                    return Request.CreateResponse(HttpStatusCode.OK, status);
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
        /// Gets the information indicating whether a XQuery is valid.
        /// An xquery kept in a workflow activity may become invalid if the database schema is changed.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="query">The xquery</param>
        /// <returns>true if it is valid, false otherwise.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("IsQueryValid")]
        public HttpResponseMessage IsQueryValid()
        {
            try
            {
                bool status = true;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string query = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    status = con.IsQueryValid(query);
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

        /// <summary>
        /// Gets the information indicating whether the format of a custom function definition is correct.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="functionDefinition">A custom function definition</param>
        /// <returns>true if it is valid, false otherwise.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("IsValidCustomFunctionDefinition")]
        public HttpResponseMessage IsValidCustomFunctionDefinition()
        {
            try
            {
                bool status = true;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string functionDefinition = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    status = con.IsValidCustomFunctionDefinition(functionDefinition);
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

        /// <summary>
        /// Validate the action code.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="actionCode">The action code</param>
        /// <param name="schemaId">The schema id indicates the schema where the instance class resides</param>
        /// <param name="instanceClassName">The class name of the instance to which the action code is run against</param>
        /// <returns>Error message if the action code is invalid, null if the action code is valid.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ValidateActionCode/{schemaId}/{instanceClassName}")]
        public HttpResponseMessage ValidateActionCode(string schemaId, string instanceClassName)
        {
            try
            {
                string msg = null;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string actionCode = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    msg = con.ValidateActionCode(actionCode, schemaId, instanceClassName);
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(msg))
                {
                    resp.Content = new StringContent(msg, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
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
        /// get the task substitute model from database as xml string
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetTaskSubstituteModel")]
        public HttpResponseMessage GetTaskSubstituteModel()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    string modelXml = con.GetTaskSubstuteModel();

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(modelXml))
                    {
                        resp.Content = new StringContent(modelXml, System.Text.Encoding.UTF8, "application/xml");
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
        /// Update the task substitute model in database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="xml">The task substitute xml string</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("UpdateTaskSubstituteModel")]
        public HttpResponseMessage UpdateTaskSubstituteModel()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xml = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.UpdateTaskSubstituteModel(xml);
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
        /// Lock the task substitute model for update.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <exception cref="LockProjectException">Thrown if the project has been locked by another user</exception>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("LockTaskSubstituteModel")]
        public HttpResponseMessage LockTaskSubstituteModel()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.LockTaskSubstituteModel();
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
        /// Unlock the task substitute model.
        /// </summary>
        /// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("UnlockTaskSubstituteModel/{forceUnlock}")]
        public HttpResponseMessage UnlockTaskSubstituteModel(bool forceUnlock)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    con.UnlockTaskSubstituteModel(forceUnlock);
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
    }
}
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Http.Description;
using System.Collections.Specialized;

using Swashbuckle.Swagger.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.WebApi.Infrastructure;
using Newtera.WebApi.Models;
using Newtera.Common.Core;
using Newtera.Data;
using Newtera.WFModel;
using Newtera.Server.DB;
using Newtera.Server.Engine.Workflow;
using Newtera.Server.Engine.Cache;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The Tasks Service allows you to manage workflow tasks so that your application can support business processes. 
    /// </summary>
    public class TasksController : ApiController
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";

        /// <summary>
        /// Get the tasks created by workflows for the requesting user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <returns>A collection of TaskInfo objects</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/{schemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TaskModel>), Description = "All tasks for the requesting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetUserTasks(string schemaName)
        {
            try
            {
                List<TaskModel> tasks = new List<TaskModel>();
                TaskModel taskModel;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);
                        List<TaskInfo> allTasks = wfConnection.GetAllTasks(schemaId);
                        foreach (TaskInfo taskInfo in allTasks)
                        {
                            //if (taskInfo.IsVisible)
                            {
                                taskModel = new TaskModel();
                                taskModel.TaskId = taskInfo.TaskId;
                                taskModel.Subject = taskInfo.Subject;
                                taskModel.Description = taskInfo.Description;
                                taskModel.CreateTime = taskInfo.CreateTime;
                                taskModel.BindingSchemaId = taskInfo.BindingSchemaId;
                                taskModel.BindingClassName = taskInfo.BindingClassName;

                                tasks.Add(taskModel);
                            }
                        }
                    }
                });

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets the count of the tasks created by the workflows for the requesting user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <returns>Total count</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/{schemaName}/count")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int), Description = "Count of tasks for the requesting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetUserTasksCount(string schemaName)
        {
            try
            {
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);
                        List<TaskInfo> allTasks = wfConnection.GetAllTasks(schemaId);

                        count = allTasks.Count;
                    }
                });

                return Ok(count);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the task of the given id
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="taskId">the task id</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/{schemaName}/{taskId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TaskModel), Description = "The task model")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetUserTaskById(string schemaName, string taskId)
        {
            try
            {
                TaskInfo taskInfo = null;
                TaskModel taskModel = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);
                        taskInfo = wfConnection.GetTask(taskId);

                        if (taskInfo != null)
                        {
                            taskModel = new TaskModel();
                            taskModel.TaskId = taskInfo.TaskId;
                            taskModel.CreateTime = taskInfo.CreateTime;
                            taskModel.Subject = taskInfo.Subject;
                            taskModel.Description = taskInfo.Description;
                            taskModel.BindingSchemaId = taskInfo.BindingSchemaId;
                            taskModel.BindingClassName = taskInfo.BindingClassName;

                            // get binding instance id
                            NewteraWorkflowInstance workflowInstance = wfConnection.GetWorkflowInstance(new Guid(taskInfo.WorkflowInstanceId));

                            if (workflowInstance != null)
                            {
                                if (!string.IsNullOrEmpty(workflowInstance.ClassName) &&
                                    !string.IsNullOrEmpty(workflowInstance.ObjId))
                                {
                                    taskModel.BindingInstanceId = workflowInstance.ObjId;
                                }
                            }

                            if (!string.IsNullOrEmpty(taskInfo.CustomFormUrl))
                            {
                                // CustomFormUrl is {param1: value1, param2: value2} by default
                                // It may also in form of app.xxx.yyy({param1: value1, param2: value2}), we need to parse the url
                                int pos = taskInfo.CustomFormUrl.IndexOf('(');
                                if (pos > 0)
                                {
                                    taskModel.FormUrl = taskInfo.CustomFormUrl.Substring(0, pos);
                                    int len = taskInfo.CustomFormUrl.Length - pos - 2;
                                    taskModel.FormParams = taskInfo.CustomFormUrl.Substring(pos + 1, len); // extract {param1: value1, param2: value2}
                                }
                                else
                                {
                                    taskModel.FormParams = taskInfo.CustomFormUrl;
                                    taskModel.FormUrl = null;
                                }
                            }

                            // create custom action string in json format
                            if (taskInfo.HasCustomActions)
                            {
                                // convert xml string to a collection of CustomAction objects
                                StringReader reader = new StringReader(taskInfo.CustomActionsXml);
                                CustomActionCollection customActions = new CustomActionCollection();
                                customActions.Read(reader);

                                List<ActionModel> actionModels = new List<ActionModel>();
                                ActionModel actionModel;

                                foreach (CustomAction customAction in customActions)
                                {
                                    actionModel = new ActionModel();
                                    actionModel.ID = customAction.ID;
                                    actionModel.Name = customAction.Name;
                                    actionModel.Api = customAction.ApiName;
                                    actionModel.FormAction = Enum.GetName(typeof(FormAction), customAction.FormAction);

                                    actionModels.Add(actionModel);
                                }

                                taskModel.CustomActions = actionModels;
                            }
                            else
                            {
                                taskModel.CustomActions = null;
                            }
                        }
                    }
                });

                return Ok(taskModel);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Transfer a task from an orignal user to a target user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="taskId">the task id, such as 22233</param>
        /// <param name="originalUser">The name of orininal task user such as demo1</param>
        /// <param name="targetUser">The name of target task user such as demo2</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/reassign/{schemaName}/{taskId}/{originalUser}/{targetUser}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TaskModel), Description = "The task model")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ReassignTask(string schemaName, string taskId, string originalUser, string targetUser)
        {
            try
            {
                TaskInfo taskInfo = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);
                        taskInfo = wfConnection.GetTask(taskId);

                        if (taskInfo != null)
                        {
                            // if there is an substitue entry for the orignal user, it means the orignal user
                            // is a reassigned user too, replace with the new target user
                            // otherwise, add a new target user
                            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
                            if (!WorkflowModelCache.Instance.IsEntryForSubstituteExist(taskId, originalUser, dataProvider))
                            {
                                string workflowInstanceId = taskInfo.WorkflowInstanceId;
                                // assign this task to the selected user
                                WorkflowModelCache.Instance.AddTaskSubstitute(taskId, schemaId, workflowInstanceId, originalUser, targetUser, dataProvider);
                            }
                            else
                            {
                                WorkflowModelCache.Instance.ReplaceTaskSubstitute(taskId, schemaId, originalUser, targetUser, dataProvider);
                            }
                        }
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the rules for transfering tasks to other users defined for a user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="userName">The name of an user such as demo1</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/substitute/{schemaName}/{userName}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SubjectEntry), Description = "The task substitution rule set or null")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetTaskSubstitutionRuleSet(string schemaName, string userName)
        {
            try
            {
                IDataProvider dataProvider = DataProviderFactory.Instance.Create();
                TaskSubstituteModel model = WorkflowModelCache.Instance.GetTaskSubstituteModel(dataProvider);
                SubjectEntry subjectEntry = (SubjectEntry)model.SubjectEntries[userName];

                if (subjectEntry != null)
                {
                    return Ok(subjectEntry);
                }
                else
                {
                    return Ok(new SubjectEntry("empty", "empty"));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Update the rules for transfering tasks to other users defined for a user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="userName">The name of an user such as demo1</param>
        /// <param name="postObj">The task substitute rule set to be updated</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/substitute/{schemaName}/{userName}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "The task substitution rule set updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> UpdateTaskSubstitutionRuleSet(string schemaName, string userName, dynamic postObj)
        {
            bool isLockObtained = false;

            try
            {
                if (postObj == null)
                {
                    throw new Exception("postObj is null");
                }

                await Task.Factory.StartNew(() =>
                {
                    // lock the model before modify it
                    WorkflowModelCache.Instance.LockTaskSubstituteModel();
                    isLockObtained = true;

                    IDataProvider dataProvider = DataProviderFactory.Instance.Create();
                    TaskSubstituteModel model = WorkflowModelCache.Instance.GetTaskSubstituteModel(dataProvider);
                    SubjectEntry subjectEntry = (SubjectEntry)model.SubjectEntries[userName];

                    bool isUpdated = false;
                    if (subjectEntry != null)
                    {
                        isUpdated = UpdateSubjectEntry(subjectEntry, postObj);
                    }

                    if (isUpdated)
                    {
                        StringBuilder builder = new StringBuilder();
                        StringWriter writer = new StringWriter(builder);
                        model.Write(writer);
                        string xml = builder.ToString();

                        WorkflowModelCache.Instance.UpdateTaskSubstituteModel(xml, dataProvider);

                        // clear the user's task cache so that the task list will be regenerated
                        UserTaskCache.Instance.ClearUserTasks(userName, null);

                        SubstituteEntryCollection substituteEntries = subjectEntry.SubstituteEntries;
                        foreach (SubstituteEntry substituteEntry in substituteEntries)
                        {
                            foreach (string substituteUser in substituteEntry.SubstituteUsers)
                            {
                                // clear substitute's task list so that it will be re-generated
                                UserTaskCache.Instance.ClearUserTasks(substituteUser, null);
                            }
                        }
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
            finally
            {
                if (isLockObtained)
                {
                    WorkflowModelCache.Instance.UnlockTaskSubstituteModel(false);
                }
            }
        }

        /// <summary>
        /// Get a finished task by a id
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="taskId">the task id</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/finished/{schemaName}/{taskId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TaskModel), Description = "The task model for the finished task")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetFinishedTaskById(string schemaName, string taskId)
        {
            try
            {
                TaskInfo taskInfo = null;
                TaskModel taskModel = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);
                        taskInfo = wfConnection.GetFinishedTask(taskId);

                        if (taskInfo != null)
                        {
                            taskModel = new TaskModel();
                            taskModel.TaskId = taskInfo.TaskId;
                            taskModel.Subject = taskInfo.Subject;
                            taskModel.Description = taskInfo.Description;
                            taskModel.CreateTime = taskInfo.CreateTime;
                            taskModel.FinishTime = taskInfo.FinishTime;
                            taskModel.Owners = taskInfo.Users;
                            taskModel.BindingSchemaId = taskInfo.BindingSchemaId;
                            taskModel.BindingClassName = taskInfo.BindingClassName;
                            taskModel.BindingInstanceId = taskInfo.BindingObjId;

                            if (!string.IsNullOrEmpty(taskInfo.CustomFormUrl))
                            {
                                // CustomFormUrl is {param1: value1, param2: value2} by default
                                // It may also in form of app.xxx.yyy({param1: value1, param2: value2}), we need to parse the url
                                int pos = taskInfo.CustomFormUrl.IndexOf('(');
                                if (pos > 0)
                                {
                                    taskModel.FormUrl = taskInfo.CustomFormUrl.Substring(0, pos);
                                    int len = taskInfo.CustomFormUrl.Length - pos - 2;
                                    taskModel.FormParams = taskInfo.CustomFormUrl.Substring(pos + 1, len); // extract {param1: value1, param2: value2}
                                }
                                else
                                {
                                    taskModel.FormParams = taskInfo.CustomFormUrl;
                                    taskModel.FormUrl = null;
                                }
                            }
                        }
                    }
                });

                return Ok(taskModel);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets finished tasks for a workflow instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="wfInstanceId">A workflow instance id</param>
        /// <returns>A collection of TaskInfo objects</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/finished/worklfow/{schemaName}/{wfInstanceId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TaskModel>), Description = "Finished tasks of the workflow instance")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetWorkflowFinishedTasks(string schemaName, string wfInstanceId)
        {
            try
            {
                List<TaskModel> tasks = new List<TaskModel>();
                TaskModel taskModel;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);
                        List<TaskInfo> allTasks = wfConnection.GetWorkflowFinishedTasks(schemaId, wfInstanceId);
                        foreach (TaskInfo taskInfo in allTasks)
                        {
                            taskModel = new TaskModel();
                            taskModel.TaskId = taskInfo.TaskId;
                            taskModel.Subject = taskInfo.Subject;
                            taskModel.Description = taskInfo.Description;
                            taskModel.CreateTime = taskInfo.CreateTime;
                            taskModel.FinishTime = taskInfo.FinishTime;
                            taskModel.Owners = taskInfo.Users;
                            taskModel.BindingSchemaId = taskInfo.BindingSchemaId;
                            taskModel.BindingClassName = taskInfo.BindingClassName;
                            taskModel.BindingInstanceId = taskInfo.BindingObjId;

                            tasks.Add(taskModel);
                        }
                    }
                });

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets the finished tasks for the requesting user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <returns>A collection of TaskInfo objects</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/finished/user/{schemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TaskModel>), Description = "Finished tasks of the requesting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetUserFinishedTasks(string schemaName)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 20));
                int pageIndex = int.Parse(GetParamValue(parameters, START_ROW, 0)) / pageSize;

                List<TaskModel> tasks = new List<TaskModel>();
                TaskModel taskModel;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);
                        List<TaskInfo> finishedTasks = wfConnection.GetFinishedTasks(schemaId, pageIndex, pageSize);
                        foreach (TaskInfo taskInfo in finishedTasks)
                        {
                            taskModel = new TaskModel();
                            taskModel.TaskId = taskInfo.TaskId;
                            taskModel.Subject = taskInfo.Subject;
                            taskModel.Description = taskInfo.Description;
                            taskModel.CreateTime = taskInfo.CreateTime;
                            taskModel.FinishTime = taskInfo.FinishTime;
                            taskModel.Owners = taskInfo.Users;
                            taskModel.BindingSchemaId = taskInfo.BindingSchemaId;
                            taskModel.BindingClassName = taskInfo.BindingClassName;
                            taskModel.BindingInstanceId = taskInfo.BindingObjId;

                            tasks.Add(taskModel);
                        }
                    }
                });

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets the count of all finished tasks for the requesting user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <returns>An integer as count</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/finished/user/{schemaName}/count")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TaskModel>), Description = "The count of all finished tasks of the requesting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetUserFinishedTaskCount(string schemaName)
        {
            try
            {
                int count = 0;

                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        string schemaId = queryHelper.GetSchemaId(connectionString);

                        count = wfConnection.GetFinishedCount(schemaId);
                    }
                });

                return Ok(count);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Clear the finished tasks for the requesting user
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/tasks/finished/{schemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TaskModel>), Description = "Finished tasks of the requesting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ClearFinishedTasks(string schemaName)
        {
            try
            {
                string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    string schemaId = queryHelper.GetSchemaId(connectionString);
                    using (WFConnection wfConnection = new WFConnection(connectionString))
                    {
                        wfConnection.Open();

                        wfConnection.ClearAllFinishedTasks(schemaId);
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        private bool UpdateSubjectEntry(SubjectEntry subjectEntry, dynamic newSubjectEntry)
        {
            bool isUpdated = false;
            int rowIndex = 0;
            foreach (SubstituteEntry substitute in subjectEntry.SubstituteEntries)
            {
                JObject newSubstitueEntry = (JObject) newSubjectEntry["substituteEntries"][rowIndex];
                if (newSubstitueEntry != null)
                {
                    switch(newSubstitueEntry["effectiveStatus"].ToString())
                    {
                        case "0":
                            if (substitute.EffectiveStatus != EffecitiveStatus.Disabled)
                            {
                                isUpdated = true;
                                substitute.EffectiveStatus = EffecitiveStatus.Disabled;
                            }
                            break;
                        case "1":

                            if (substitute.EffectiveStatus != EffecitiveStatus.Immediately)
                            {
                                isUpdated = true;
                                substitute.EffectiveStatus = EffecitiveStatus.Immediately;
                            }
                           
                            break;

                        case "2":

                            if (substitute.EffectiveStatus != EffecitiveStatus.Duration)
                            {
                                isUpdated = true;
                                substitute.EffectiveStatus = EffecitiveStatus.Duration;
                            }
  
                            break;

                    }

                    rowIndex++;
                }
            }

            return isUpdated;
        }

        private string GetParamValue(NameValueCollection parameters, string key, object defaultValue)
        {
            string val = null;

            if (defaultValue != null)
            {
                val = defaultValue.ToString();
            }

            if (parameters != null && parameters[key] != null)
            {
                val = parameters[key];
            }

            return val;
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}
/*
* @(#)NewteraTaskService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Workflow.Runtime;
using System.Threading;
using System.Security.Principal;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Security;

using Newtera.Common.Core;
using Newtera.WorkflowServices;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;
using Newtera.Server.DB;
using Newtera.Server.Engine.Interpreter;
using Newtera.Server.Engine.Cache;
using Newtera.WFModel;
using Newtera.Common.Wrapper;
using Newtera.Common.MetaData.DataView;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary>
    /// Provide the service for workflow task management based on Newtera Database.
    /// </summary>
    public class NewteraTaskService : ITaskService
    {
        private const string AddTaskQuery = "let $task := [[<Task xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\"  xsi:type=\"Task\"><TaskId xsi:nil=\"true\"/><WorkflowInstanceId>param10</WorkflowInstanceId><CreateTime>param11</CreateTime><Subject>param12</Subject><Description>param13</Description><Instruction>param14</Instruction><Users>param15</Users><Roles>param16</Roles><ActivityName>param17</ActivityName><BindingSchemaId>param18</BindingSchemaId><BindingClassName>param19</BindingClassName><CustomActions>param20</CustomActions><CustomFormUrl>param21</CustomFormUrl><FormProperties>param22</FormProperties><Visible>param23</Visible></Task>]] return addInstance(document(\"db://workflowinfo.xml\"), $task)";
        private const string DeleteTaskQuery = "for $t in document(\"db://WorkflowInfo.xml\")/TaskList/Task where $t/WorkflowInstanceId=\"param1\" and $t/ActivityName=\"param2\" return deleteInstance(document(\"db://WorkflowInfo.xml\"), $t)";
        private const string DeleteTaskQueryById = "for $t in document(\"db://WorkflowInfo.xml\")/TaskList/Task where $t/TaskId=\"param1\" return deleteInstance(document(\"db://WorkflowInfo.xml\"), $t)";
        private const string DeleteTasksQuery = "for $t in document(\"db://WorkflowInfo.xml\")/TaskList/Task where $t/WorkflowInstanceId=\"param1\" return deleteInstance(document(\"db://WorkflowInfo.xml\"), $t)";
        private const string GetTasksQuery = "for $task in document(\"db://workflowinfo.xml\")/TaskList/Task where $task/BindingSchemaId=\"param1\" return <Task {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/Users} {$task/Roles} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties} {$task/Visible}</Task> sortby ($task/TaskId descending)";
        private const string GetWorkflowInstanceTasksQuery = "for $task in document(\"db://workflowinfo.xml\")/TaskList/Task where $task/WorkflowInstanceId=\"param1\" return <Task {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/Users} {$task/Roles} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties} {$task/Visible}</Task> sortby ($task/CreateTime descending)";
        private const string GetTaskByIdQuery = "for $task in document(\"db://workflowinfo.xml\")/TaskList/Task where $task/TaskId=\"param1\" return <Task {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/Users} {$task/Roles} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties} {$task/Visible}</Task>";
        private const string GetTaskByObjIdQuery = "for $task in document(\"db://workflowinfo.xml\")/TaskList/Task[@obj_id = \"(obj_id)\"] return <Task {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/Users} {$task/Roles} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties} {$task/Visible}</Task>";
        private const string GetTaskByWfIdAndActivityQuery = "for $task in document(\"db://workflowinfo.xml\")/TaskList/Task where $task/WorkflowInstanceId=\"param1\" and $task/ActivityName=\"param2\" return <Task {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/Users} {$task/Roles} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties} {$task/Visible}</Task>";
        private const string GetBindingValueQuery = "for $i in document(\"db://(Schema).xml?Version=(Version)\")/(Class)List/(Class)[@obj_id = \"(obj_id)\"] return <(Class)> {$i/(Attribute)}</(Class)>";
        private const string AddTaskLogQuery = "let $log := [[<TaskExecuteLog xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\"  xsi:type=\"TaskExecuteLog\"><LogID xsi:nil=\"true\"/><BindingInstanceKey>param10</BindingInstanceKey><BindingInstanceDesc>param11</BindingInstanceDesc><WorkflowInstanceId>param12</WorkflowInstanceId><TaskName>param13</TaskName><ProjectName>param14</ProjectName><ProjectVersion>param15</ProjectVersion><WorkflowName>param16</WorkflowName><StartTime>param17</StartTime><ExpectedFinishTime>param18</ExpectedFinishTime><TaskTakers>param19</TaskTakers><TaskID>param20</TaskID></TaskExecuteLog>]] return addInstance(document(\"db://workflowinfo.xml\"), $log)";
        private const string UpdateTaskLogQuery = "for $log in document(\"db://WorkflowInfo.xml\")/TaskExecuteLogList/TaskExecuteLog where $log/TaskID = \"param10\" return (setText($log/FinishTime, \"param11\"), updateInstance(document(\"db://WorkflowInfo.xml\"), $log))";
        private const string GetTaskLogsQuery = "for $log in document(\"db://WorkflowInfo.xml\")/TaskExecuteLogList/TaskExecuteLog where $log/BindingInstanceKey = \"param10\" and $log/TaskName = \"param11\" return $log";
        private const string GetCompletedTasksQuery = "for $task in document(\"db://workflowinfo.xml\")/CompletedTaskList/CompletedTask[start to finish] where $task/BindingSchemaId=\"param1\" and $task/User=\"param2\" return <CompletedTask {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/FinishTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/User} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/BindingObjId} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties}</CompletedTask> sortby ($task/@obj_id descending )";
        private const string GetCompletedTaskCountQuery = "for $task in document(\"db://workflowinfo.xml\")/CompletedTaskList/CompletedTask where $task/BindingSchemaId=\"param1\" and $task/User=\"param2\" return <CompletedTask {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/FinishTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/User} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/BindingObjId} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties}</CompletedTask> sortby ($task/@obj_id descending)";
        private const string GetCompletedTasksByWorkflowQuery = "for $task in document(\"db://workflowinfo.xml\")/CompletedTaskList/CompletedTask where $task/BindingSchemaId=\"param1\" and $task/User=\"param2\" return <CompletedTask {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/FinishTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/User} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/BindingObjId} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties}</CompletedTask> sortby ($task/obj_id descending)";
        private const string AddCompletedTaskQuery = "let $task := [[<CompletedTask xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\"  xsi:type=\"CompletedTask\"><TaskId >param10</TaskId><WorkflowInstanceId>param11</WorkflowInstanceId><CreateTime>param12</CreateTime><FinishTime>param13</FinishTime><Subject>param14</Subject><Description>param15</Description><Instruction>param16</Instruction><User>param17</User><ActivityName>param18</ActivityName><BindingSchemaId>param19</BindingSchemaId><BindingClassName>param20</BindingClassName><BindingObjId>param21</BindingObjId><CustomActions>param22</CustomActions><CustomFormUrl>param23</CustomFormUrl><FormProperties>param24</FormProperties></CompletedTask>]] return addInstance(document(\"db://workflowinfo.xml\"), $task)";
        private const string GetCompletedTaskByIdQuery = "for $task in document(\"db://workflowinfo.xml\")/CompletedTaskList/CompletedTask where $task/TaskId=\"param1\" and $task/User=\"param2\" return <CompletedTask {$task/@obj_id}> {$task/TaskId} {$task/WorkflowInstanceId} {$task/CreateTime} {$task/FinishTime} {$task/Subject} {$task/Description} {$task/Instruction} {$task/User} {$task/ActivityName} {$task/BindingSchemaId} {$task/BindingClassName} {$task/BindingObjId} {$task/CustomActions} {$task/CustomFormUrl} {$task/FormProperties}</CompletedTask>";
        private const string DeleteCompletedTasksQuery = "for $t in document(\"db://WorkflowInfo.xml\")/CompletedTaskList/CompletedTask where $t/BindingSchemaId=\"param1\" and $t/User=\"param2\" return deleteInstance(document(\"db://WorkflowInfo.xml\"), $t)";

        private const string VariablePattern = @"\{[^\}]+\}";
        private const string BODY_VARIABLE = @"${body}";
        public const string EMAIL_TEMPLATES_DIR = @"Templates\Emails\";
        private const string DEFAULT_EMAIL_TEMPLATE = "EmailTemplate.txt";

        private IPrincipal _superUser = null;
        private IDataProvider _dataProvider;
        private IUserManager _userManager;

        public NewteraTaskService()
        {
            CMUserManager userMgr = new CMUserManager();
            _superUser = userMgr.SuperUser;
            _dataProvider = DataProviderFactory.Instance.Create();
            _userManager = new ServerSideUserManager();
        }

        /// <summary>
        /// Get the super user principle
        /// </summary>
        public IPrincipal SuperUser
        {
            get
            {
                return _superUser;
            }
        }

        /// <summary>
        /// Create a task for each of specified users
        /// </summary>
        /// <param name="subject">The task subject</param>
        /// <param name="description">The task description</param>
        /// <param name="instruction">The task instruction</param>
        /// <param name="url">The custom form url, could be null</param>
        /// <param name="formProperties">The properties to be displayed in the form</param>
        /// <param name="users">A list of users who are assigned to the task</param>
        /// <param name="roles">A roles that a users must belong to in order to receive the assignment</param>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        /// <param name="bindingSchemaId">The schema id of the binding data instance, can be empty.</param>
        /// <param name="bindingClassName">The class name of the binding data instance, can be empty.</param>
        /// <param name="customActionsXml">The xml represents a collection of CustomAction objects</param>
        /// <param name="isVisible">Is the task visible on the my task list</param>
        /// <returns>The task unique id</returns>
        public string CreateTask(string subject, string description, string instruction, string url,
            StringCollection formProperties, StringCollection users, StringCollection roles, string activityName,
            string bindingSchemaId, string bindingClassName, string customActionsXml, bool isVisible)
        {
            string userString = ConvertToArrayString(users);
            string roleString = ConvertToArrayString(roles);
            string formPropertyString = ConvertToArrayString(formProperties);
            return CreateTask(null, subject, description, instruction, url, formPropertyString, userString, roleString,
                activityName, bindingSchemaId, bindingClassName, customActionsXml, isVisible);
        }

        /// <summary>
        /// Create a task for each of specified users
        /// </summary>
        /// <param name="workflowInstanceId">A workflow instance id</param>
        /// <param name="subject">The task subject</param>
        /// <param name="description">The task description</param>
        /// <param name="instruction">The task instruction</param>
        /// <param name="formProperties">The properties to be displayed in the form</param>
        /// <param name="url">The custom form url, could be null</param>
        /// <param name="users">A string of users who are assigned to the task</param>
        /// <param name="roles">A string of roles that a users must belong to in order to receive the assignment</param>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        /// <param name="bindingSchemaId">The schema id of the binding data instance, can be empty.</param>
        /// <param name="bindingClassName">The class name of the binding data instance, can be empty.</param>
        /// <param name="customActionsXml">The xml represents a collection of CustomAction objects</param>
        /// <param name="isVisible"> Is task visible on the my task list</param>
        /// <returns>The task unique id </returns>
        public string CreateTask(string workflowInstanceId, string subject, string description, string instruction,
            string url, string formProperties, string users, string roles, string activityName,
            string bindingSchemaId, string bindingClassName, string customActionsXml, bool isVisible)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            string taskId = null;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string wfInstanceId = workflowInstanceId;
                if (wfInstanceId == null)
                {
                    // called by the workflow runtime
                    wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();
                }

                // replace the variables contained in Subject or Description with actual values
                // from the binding data instance
                IInstanceWrapper instance = this.GetBindingInstance(wfInstanceId);
                string subject1 = ReplaceVariables(subject, instance);
                string description1 = ReplaceVariables(description, instance);
                string instruction1 = ReplaceVariables(instruction, instance);

                string query = AddTaskQuery;

                query = query.Replace("param10", wfInstanceId);
                query = query.Replace("param11", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
                query = query.Replace("param12", SecurityElement.Escape(subject1));
                query = query.Replace("param13", SecurityElement.Escape(description1));
                query = query.Replace("param14", SecurityElement.Escape(instruction1));
                query = query.Replace("param15", users);
                query = query.Replace("param16", roles);
                query = query.Replace("param17", activityName);
                query = query.Replace("param18", (bindingSchemaId != null? bindingSchemaId : ""));
                query = query.Replace("param19", (bindingClassName != null ? bindingClassName : ""));
                query = query.Replace("param20", (customActionsXml != null ? customActionsXml : ""));
                query = query.Replace("param21", (url != null ? System.Security.SecurityElement.Escape(url): ""));
                query = query.Replace("param22", (formProperties != null ? formProperties : ""));
                query = query.Replace("param23", (isVisible? "true" : "false"));

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                string objId = "";

                XmlDocument doc = interpreter.Query(query);

                if (doc.DocumentElement.InnerText != null)
                {
                    objId = doc.DocumentElement.InnerText;

                    // retrive the newly created task info
                    TaskInfo taskInfo = GetTaskByObjId(objId);
                    if (taskInfo != null)
                    {
                        taskId = taskInfo.TaskId;
                        AddTaskInfoToCache(bindingSchemaId, taskInfo);
                    }
                    else
                    {
                        throw new Exception("Unable to find a task info object with id " + objId);
                    }
                }

                return taskId;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Gets the user's tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user manager</param>
        /// <returns>A list of user tasks.</returns>
        public List<TaskInfo> GetUserTasks(string schemaId, string userName, IUserManager userManager)
        {
            List<TaskInfo> userTasks = null;
            List<TaskInfo> allTasks1 = null;
            List<TaskInfo> allTasks2 = null;

            userTasks = UserTaskCache.Instance.GetUserTasks(userName, schemaId);
            if (userTasks == null)
            {
                lock (UserTaskCache.Instance)
                {
                    allTasks1 = UserTaskCache.Instance.GetSchemaTasks(schemaId);
                    if (allTasks1 == null)
                    {
                        // first time access, initialize the cache
                        allTasks1 = GetAllTasks(schemaId);

                        UserTaskCache.Instance.SetSchemaTasks(schemaId, allTasks1);
                    }

                    // make a new list so that the original list can be modified while looping through the new list
                    allTasks2 = new List<TaskInfo>();
                    foreach (TaskInfo taskInfo in allTasks1)
                    {
                        allTasks2.Add(taskInfo);
                    }
                }

                // get the user's tasks which may take a little bi time to complete and allTasks1 may be modofied during this time
                userTasks = GetUserTasksFromAllTasks(userName, userManager, allTasks2); 

                lock (UserTaskCache.Instance)
                {
                    // if the allTasks1 has been modified, the userTasks may be obsolete, do not save it in the cache,
                    // rebuild the user task list at the next call
                    if (!IsTaskListModified(allTasks1, allTasks2))
                    {
                        // remember in the cache
                        UserTaskCache.Instance.SetUserTasks(userName, schemaId, userTasks);

                        // get a copy of user's tasks to avoid memory leak
                        userTasks = UserTaskCache.Instance.GetUserTasks(userName, schemaId);
                    }
                }
            }

            return userTasks;
        }

        /// <summary>
        /// Gets a workflow instance's tasks
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <returns>A list of task infos.</returns>
        public List<TaskInfo> GetWorkflowInstanceTasks(string workflowInstanceId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            QueryReader reader = null;

            try
            {
                List<TaskInfo> taskInfos = new List<TaskInfo>();

                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetWorkflowInstanceTasksQuery;

                query = query.Replace("param1", workflowInstanceId);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                // tell the interpreter that we want a reader that gets the query result in paging mode.
                interpreter.IsPaging = true;
                reader = interpreter.GetQueryReader(query);

                XmlDocument doc = reader.GetNextPage();
                DataSet ds;
                XmlReader xmlReader;
                DataTable allTasks;
                TaskInfo taskInfo;

                while (doc.DocumentElement.ChildNodes.Count > 0)
                {
                    ds = new DataSet();
                    xmlReader = new XmlNodeReader(doc);
                    ds.ReadXml(xmlReader);
                    allTasks = ds.Tables["Task"];

                    if (allTasks != null)
                    {
                        foreach (DataRow dataRow in allTasks.Rows)
                        {
                            taskInfo = new TaskInfo();
                            taskInfo.TaskId = dataRow["TaskId"].ToString();
                            taskInfo.WorkflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                            taskInfo.CreateTime = dataRow["CreateTime"].ToString();
                            taskInfo.Subject = dataRow["Subject"].ToString();
                            taskInfo.Description = dataRow["Description"].ToString();
                            taskInfo.Instruction = dataRow["Instruction"].ToString();
                            taskInfo.BindingSchemaId = dataRow["BindingSchemaId"].ToString();
                            taskInfo.BindingClassName = dataRow["BindingClassName"].ToString();
                            taskInfo.Users = dataRow["Users"].ToString();
                            taskInfo.Roles = dataRow["Roles"].ToString();
                            if (!dataRow.IsNull("ActivityName"))
                            {
                                taskInfo.ActivityName = dataRow["ActivityName"].ToString();
                            }
                            if (!dataRow.IsNull("CustomActions"))
                            {
                                taskInfo.CustomActionsXml = dataRow["CustomActions"].ToString();
                            }
                            if (!dataRow.IsNull("CustomFormUrl"))
                            {
                                taskInfo.CustomFormUrl = dataRow["CustomFormUrl"].ToString();
                            }
                            if (!dataRow.IsNull("FormProperties"))
                            {
                                taskInfo.FormProperties = dataRow["FormProperties"].ToString();
                            }
                            if (!dataRow.IsNull("Visible") && !string.IsNullOrEmpty(dataRow["Visible"].ToString()))
                            {
                                taskInfo.IsVisible = bool.Parse(dataRow["Visible"].ToString());
                            }

                            taskInfos.Add(taskInfo);
                        }
                    }

                    // get next page of records
                    doc = reader.GetNextPage();
                }

                return taskInfos;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Gets a task by id
        /// </summary>
        /// <param name="taskId">task id</param>
        /// <returns>A TaskInfo object</returns>
        public TaskInfo GetTask(string taskId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            TaskInfo taskInfo = null;
            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetTaskByIdQuery;

                query = query.Replace("param1", taskId);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(query);

                DataSet ds = new DataSet();
                XmlReader xmlReader = new XmlNodeReader(doc);
                ds.ReadXml(xmlReader);
                DataTable taskTable = ds.Tables["Task"];

                if (taskTable != null && taskTable.Rows.Count == 1)
                {
                    taskInfo = new TaskInfo();
                    DataRow dataRow = taskTable.Rows[0];
                    taskInfo.TaskId = dataRow["TaskId"].ToString();
                    taskInfo.WorkflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                    taskInfo.CreateTime = dataRow["CreateTime"].ToString();
                    taskInfo.Subject = dataRow["Subject"].ToString();
                    taskInfo.Description = dataRow["Description"].ToString();
                    taskInfo.Instruction = dataRow["Instruction"].ToString();
                    taskInfo.BindingSchemaId = dataRow["BindingSchemaId"].ToString();
                    taskInfo.BindingClassName = dataRow["BindingClassName"].ToString();
                    taskInfo.Users = dataRow["Users"].ToString();
                    taskInfo.Roles = dataRow["Roles"].ToString();
                    if (!dataRow.IsNull("ActivityName"))
                    {
                        taskInfo.ActivityName = dataRow["ActivityName"].ToString();
                    }
                    if (!dataRow.IsNull("CustomActions"))
                    {
                        taskInfo.CustomActionsXml = dataRow["CustomActions"].ToString();
                    }
                    if (!dataRow.IsNull("CustomFormUrl"))
                    {
                        taskInfo.CustomFormUrl = dataRow["CustomFormUrl"].ToString();
                    }
                    if (!dataRow.IsNull("FormProperties"))
                    {
                        taskInfo.FormProperties = dataRow["FormProperties"].ToString();
                    }
                    if (!dataRow.IsNull("Visible") && !string.IsNullOrEmpty(dataRow["Visible"].ToString()))
                    {
                        taskInfo.IsVisible = bool.Parse(dataRow["Visible"].ToString());
                    }
                }

                return taskInfo;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Gets a finished task by id belong to an user
        /// </summary>
        /// <param name="taskId">task id</param>
        /// <param name="userName">The user name</param>
        /// <returns>A TaskInfo object</returns>
        public TaskInfo GetFinishedTask(string taskId, string userName)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            TaskInfo taskInfo = null;
            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetCompletedTaskByIdQuery;

                query = query.Replace("param1", taskId);
                query = query.Replace("param2", userName);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(query);

                DataSet ds = new DataSet();
                XmlReader xmlReader = new XmlNodeReader(doc);
                ds.ReadXml(xmlReader);
                DataTable taskTable = ds.Tables["CompletedTask"];

                if (taskTable != null && taskTable.Rows.Count == 1)
                {
                    taskInfo = new TaskInfo();
                    DataRow dataRow = taskTable.Rows[0];
                    taskInfo.TaskId = dataRow["TaskId"].ToString();
                    taskInfo.WorkflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                    taskInfo.CreateTime = dataRow["CreateTime"].ToString();
                    taskInfo.FinishTime = dataRow["FinishTime"].ToString();
                    taskInfo.Subject = dataRow["Subject"].ToString();
                    taskInfo.Description = dataRow["Description"].ToString();
                    taskInfo.Instruction = dataRow["Instruction"].ToString();
                    taskInfo.BindingSchemaId = dataRow["BindingSchemaId"].ToString();
                    taskInfo.BindingClassName = dataRow["BindingClassName"].ToString();
                    taskInfo.BindingObjId = dataRow["BindingObjId"].ToString();
                    taskInfo.Users = dataRow["User"].ToString();
                    if (!dataRow.IsNull("ActivityName"))
                    {
                        taskInfo.ActivityName = dataRow["ActivityName"].ToString();
                    }
                    taskInfo.CustomActionsXml = "";
                    if (!dataRow.IsNull("CustomFormUrl"))
                    {
                        taskInfo.CustomFormUrl = dataRow["CustomFormUrl"].ToString();
                    }
                    if (!dataRow.IsNull("FormProperties"))
                    {
                        taskInfo.FormProperties = dataRow["FormProperties"].ToString();
                    }
                }

                return taskInfo;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Gets values of an attribute from a binding data instance indicated by the parameters
        /// </summary>
        /// <param name="schemaId">the schema of the data instance</param>
        /// <param name="className">The class of the data instance</param>
        /// <param name="attributeName">The attibute name of the data instance</param>
        /// <returns>A collection of value strings</returns>
        public StringCollection GetBindingValues(string schemaId, string className, string attributeName)
        {
            StringCollection values = null;

            // get the id of the data instance bound to the workflow instance
            string wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();
            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
            WorkflowInstanceBindingInfo binding = workflowModelAdapter.GetBindingInfoByWorkflowInstanceId(wfInstanceId);
            string objId = null;

            if (binding != null)
            {
                objId = binding.DataInstanceId;
            }

            if (objId != null)
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    string query = GetBindingValueQuery;

                    string[] strings = schemaId.Split(' ');
                    query = query.Replace("(Schema)", strings[0].Trim());
                    query = query.Replace("(Version)", strings[1].Trim());
                    query = query.Replace("(Class)", className);
                    query = query.Replace("(Attribute)", attributeName);
                    query = query.Replace("(obj_id)", objId);

                    Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                    XmlDocument doc = interpreter.Query(query);

                    DataSet ds = new DataSet();
                    XmlReader xmlReader = new XmlNodeReader(doc);
                    ds.ReadXml(xmlReader);
                    DataTable dataTable = ds.Tables[className];

                    if (dataTable != null && dataTable.Rows.Count == 1)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        string bindingValue = dataRow[attributeName].ToString();
                        if (bindingValue != null && bindingValue.Length > 0)
                        {
                            values = new StringCollection();
                            string[] splitValues = bindingValue.Split(';');
                            for (int i = 0; i < splitValues.Length; i++)
                            {
                                values.Add(splitValues[i]);
                            }
                        }
                    }
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }

            return values;
        }

        /// <summary>
        /// Gets user ids from an attribute's value representing user's display names from a binding data instance indicated by the parameters
        /// </summary>
        /// <param name="schemaId">the schema of the data instance</param>
        /// <param name="className">The class of the data instance</param>
        /// <param name="attributeName">The attibute name of the binding data instance</param>
        /// <returns>A collection of user ids strings</returns>
        public StringCollection GetBindingUsers(string schemaId, string className, string attributeName)
        {
            StringCollection userIds = new StringCollection();
            StringCollection userDisplayNames = GetBindingValues(schemaId, className, attributeName);
            if (userDisplayNames != null && userDisplayNames.Count > 0)
            {
                UsersListHandler usersHandler = new UsersListHandler();

                EnumValueCollection enumValues = usersHandler.GetValues(new ServerSideUserManager());

                // convert fom user display name to the user id
                foreach (string userDisplayName in userDisplayNames)
                {
                    foreach (EnumValue enumValue in enumValues)
                    {
                        if (enumValue.DisplayText == userDisplayName)
                        {
                            userIds.Add(enumValue.Value);
                            break;
                        }
                    }
                }
            }

            return userIds;
        }

        /// <summary>
        /// Gets role's names from an attribute's value representing role's display names from a binding data instance indicated by the parameters
        /// </summary>
        /// <param name="schemaId">the schema of the data instance</param>
        /// <param name="className">The class of the data instance</param>
        /// <param name="attributeName">The attibute name of the binding data instance</param>
        /// <returns>A collection of role's names</returns>
        public StringCollection GetBindingRoles(string schemaId, string className, string attributeName)
        {
            StringCollection roleNames = new StringCollection();
            StringCollection roleDisplayNames = GetBindingValues(schemaId, className, attributeName);
            if (roleDisplayNames != null && roleDisplayNames.Count > 0)
            {
                RolesListHandler rolesHandler = new RolesListHandler();

                EnumValueCollection enumValues = rolesHandler.GetValues(new ServerSideUserManager());

                // convert fom role display name to role name
                foreach (string roleDisplayName in roleDisplayNames)
                {
                    foreach (EnumValue enumValue in enumValues)
                    {
                        if (enumValue.DisplayText == roleDisplayName)
                        {
                            roleNames.Add(enumValue.Value);
                            break;
                        }
                    }
                }
            }

            return roleNames;
        }

        /// <summary>
        /// Gets dynamic assigned roles from a bound data instance indicated by the parameters
        /// </summary>
        /// <param name="schemaId">the schema of the data instance</param>
        /// <param name="className">The class of the data instance</param>
        /// <param name="attributeName">The attibute name of the data instance</param>
        /// <returns>A collection of roles</returns>
        public StringCollection GetDynamicAssignedRoles(string schemaId, string className, string attributeName)
        {
            StringCollection roles = null;

            string wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();

            // TODO
            return roles;
        }

        /// <summary>
        /// An utility method that returns a list of distintc qualified users specified by the users
        /// and roles.
        /// </summary>
        /// <param name="users">The explicit user collection</param>
        /// <param name="roles">The roles that a qualified users must belong to</param>
        /// <param name="userManager">IUserManager object</param>
        /// <returns>A list of distinct user list</returns>
        public StringCollection GetQualifiedUsers(StringCollection users, StringCollection roles, IUserManager userManager)
        {
            StringCollection qualifierUsers = new StringCollection();

            if (users != null)
            {
                foreach (string user in users)
                {
                    AddUser(qualifierUsers, user);
                }
            }

            // get list of users who belong to all the specified roles
            if (roles != null && roles.Count > 0)
            {
                Hashtable userTable = new Hashtable();
                foreach (string role in roles)
                {
                    string[] roleUsers = userManager.GetUsers(role);
                    if (roleUsers != null)
                    {
                        for (int i = 0; i < roleUsers.Length; i++)
                        {
                            string user = roleUsers[i];
                            if (userTable[user] == null)
                            {
                                userTable[user] = 1; // one occurence
                            }
                            else
                            {
                                // assuming GetUsers method returns a list of distinct users, otherwise, it will break
                                userTable[user] = ((int)userTable[user]) + 1; // increment by one
                            }
                        }
                    }
                }

                // pick the qualified users
                int roleCount = roles.Count;
                foreach (string user in userTable.Keys)
                {
                    if (((int)userTable[user]) == roleCount)
                    {
                        AddUser(qualifierUsers, user);
                    }
                }
            }

            return qualifierUsers;
        }

        /// <summary>
        /// Get a list of users who are actually responsible for performing the task, by 
        /// substituting users according to the substitute rules
        /// </summary>
        /// <param name="taskId">The task Id</param>
        /// <param name="assigneUsers">The users who have been assigned the task.</param>
        /// <returns>A list of actual users</returns>
        public StringCollection GetActualUsers(string taskId, StringCollection assigneUsers, string workflowProjectName, string workflowProjectVersion)
        {
            StringCollection actualUsers = new StringCollection();
            StringCollection substituteUsers;
            TaskSubstituteModel taskSubstituteModel = WorkflowModelCache.Instance.GetTaskSubstituteModel(_dataProvider);
            string substituteUser;

            foreach (string user in assigneUsers)
            {
                // First get the substitute user from the cache which is selected by an original
                // task owner on a task-basis
                substituteUser = WorkflowModelCache.Instance.GetTaskSubstitute(taskId, user, _dataProvider);
                if (!string.IsNullOrEmpty(substituteUser) && !actualUsers.Contains(substituteUser))
                {
                    actualUsers.Add(substituteUser);
                }
                else
                {
                    // Next, get substitute users based on the rules which are pre-defined by administrator
                    substituteUsers = taskSubstituteModel.GetSubstituteUsers(user, workflowProjectName, workflowProjectVersion);

                    // if the user doesn't have appointed any substitute uses, it will be null collection
                    if (substituteUsers == null || substituteUsers.Count == 0)
                    {
                        actualUsers.Add(user);
                    }
                    else
                    {
                        // add the substitute users to the actual user list
                        foreach (string substitute in substituteUsers)
                        {
                            if (!actualUsers.Contains(substitute))
                            {
                                actualUsers.Add(substitute);
                            }
                        }
                    }
                }
            }

            return actualUsers;
        }

        /// <summary>
        /// Cancel the task that has been assigned to the users.
        /// </summary>
        /// <param name="activityName">The name of the CreateTaskActivity which created the task</param>
        /// <returns>Task Id</returns>
        public string CloseTask(string activityName)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            string taskId = null;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();

                // remove the substitue user entries from the cache
                TaskInfo taskInfo = GetTaskInfo(wfInstanceId, activityName);
                if (taskInfo != null)
                {

                    // delete the task from database
                    string query = DeleteTaskQuery;

                    query = query.Replace("param1", wfInstanceId);
                    query = query.Replace("param2", activityName);

                    Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                    taskId = taskInfo.TaskId;

                    XmlDocument doc = interpreter.Query(query);

                    // remove the task info from the caceh and clear corresponding user's cache
                    RemoveTaskInfoToCache(taskInfo.BindingSchemaId, taskInfo);

                    WorkflowModelCache.Instance.RemoveTaskSubstitutes(taskInfo.TaskId, _dataProvider);
                }

                return taskId;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Remove the task of the given id that has been assigned to the users from DB and server cache
        /// </summary>
        /// <param name="taskId">The id of the task to be cancelled</param>
        public void CloseTaskById(string taskId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                TaskInfo taskInfo = GetTask(taskId);
                if (taskInfo != null)
                {
                    // remember the requesting user in the taskInfo
                    taskInfo.UserId = originalPrincipal.Identity.Name;

                    // delete the task from database
                    string query = DeleteTaskQueryById;

                    query = query.Replace("param1", taskId);

                    Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                    XmlDocument doc = interpreter.Query(query);

                    // remove the task info from the caceh and clear corresponding user's cache
                    RemoveTaskInfoToCache(taskInfo.BindingSchemaId, taskInfo);

                    WorkflowModelCache.Instance.RemoveTaskSubstitutes(taskInfo.TaskId, _dataProvider);
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Delete tasks from database that are associated with a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        public void DeleteTasks(string workflowInstanceId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                // remove the substitue user entries and task info from the cache
                List<TaskInfo> taskInfos = GetWorkflowInstanceTasks(workflowInstanceId);

                // delete the task from database
                string query = DeleteTasksQuery;

                query = query.Replace("param1", workflowInstanceId);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                XmlDocument doc = interpreter.Query(query);

                foreach (TaskInfo taskInfo in taskInfos)
                {
                    WorkflowModelCache.Instance.RemoveTaskSubstitutes(taskInfo.TaskId, _dataProvider);

                    RemoveTaskInfoToCache(taskInfo.BindingSchemaId, taskInfo);
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Send a notification to the specified users
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="emailAddresses">A list of email addresses</param>
        /// <param name="users">the users to be noticed</param>
        public void SendNotice(string subject, string body, StringCollection emailAddresses, StringCollection users)
        {
            SendNotice(subject, body, emailAddresses, users, true);
        }

        /// <summary>
        /// Send a notification to the specified users
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="emailAddresses">A list of email addresses</param>
        /// <param name="users">the users to be noticed</param>
        /// <param name="hasVariables">Information indicate whether the subject and body contain variables that need to be replaced with data from the binding instance</param>
        public void SendNotice(string subject, string body, StringCollection emailAddresses, StringCollection users, bool hasVariables)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string wfInstanceId;
                IInstanceWrapper instance = null;

                try
                {
                    if (WorkflowEnvironment.WorkflowInstanceId != null)
                        wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();
                    else
                        wfInstanceId = null;
                }
                catch (Exception)
                {
                    wfInstanceId = null;
                }

                string subject1 = subject;
                string body1 = body;
                if (hasVariables && wfInstanceId != null)
                {
                    instance = this.GetBindingInstance(wfInstanceId);

                    if (!string.IsNullOrEmpty(subject))
                    {
                        subject1 = ReplaceVariables(subject, instance);
                    }

                    if (!string.IsNullOrEmpty(body))
                    {
                        body1 = ReplaceVariables(body, instance);
                    }
                }

                if (!string.IsNullOrEmpty(body1))
                {
                    // get email message using a template if exists
                    body1 = GetTemplatedBody(body1);
                }

                if (emailAddresses != null && emailAddresses.Count > 0)
                {
                    // Create a new SmtpClient for sending the email
                    string fromAddress = SmtpConfig.Instance.GetFromAddress();
                    string smtpHost = SmtpConfig.Instance.GetSmtpHost();
                    int smtpPort = SmtpConfig.Instance.GetSmtpPort();
                    string emailUser = SmtpConfig.Instance.GetEmailUser();
                    string emailPassword = SmtpConfig.Instance.GetEmailPassword();

                    SmtpClient sc;

                    System.Text.Encoding emailEncoding = GetEmailEncoding(SmtpConfig.Instance.GetEmailEncoding());

                    foreach (string emailAddress in emailAddresses)
                    {
                        // Use the properties of the activity to construct a new MailMessage
                        try
                        {
                            sc = new SmtpClient(smtpHost, smtpPort);
                            sc.UseDefaultCredentials = true;
                            sc.Credentials = new System.Net.NetworkCredential(emailUser, emailPassword);

                            MailMessage message = new MailMessage();
                            message.From = new MailAddress(fromAddress);
                            message.To.Add(emailAddress);
                            message.SubjectEncoding = emailEncoding;
                            message.BodyEncoding = emailEncoding;
                            message.Priority = MailPriority.Normal;

                            if (!String.IsNullOrEmpty(subject1))
                            {
                                message.Subject = subject1;
                            }
                            else
                            {
                                message.Subject = "No subject";
                            }

                            if (!String.IsNullOrEmpty(body1))
                            {
                                message.Body = body1;
                            }
                            else
                            {
                                message.Body = "No content";
                            }

                            // Set the SMTP host and send the mail asynchronously
                            string userState = "email message";

                            sc.SendAsync(message, userState);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.WriteLine("Failed to send email to " + emailAddress + ". Error message is " + ex.Message);
                        }
                    }
                }

                // send message to users
                if (users != null && users.Count > 0)
                {
                    IMessageService messageService = Newtera.Server.Engine.Workflow.NewteraWorkflowRuntime.Instance.GetWorkflowRunTime().GetService<IMessageService>();

                    MessageInfo messageInfo = new MessageInfo();
                    messageInfo.Subject = subject1;
                    messageInfo.Content = body1;
                    messageInfo.SenderName = "admin";
                    messageInfo.SendTime = DateTime.Now.ToShortDateString();
                    if (instance != null)
                    {
                        messageInfo.ObjId = instance.ObjId;
                        messageInfo.ClassName = instance.OwnerClassName;
                        messageInfo.SchemaName = instance.SchemaId;
                    }

                    foreach (string user in users)
                    {
                        messageService.SendTaskNoticeToUser(user, messageInfo);
                    }
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Save an user's finsished task into database
        /// </summary>
        /// <param name="taskInfo">The task info</param>
        /// <param name="userName">The user name</param>
        public void AddUserFinishedTask(TaskInfo taskInfo, string userName)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
                WorkflowInstanceBindingInfo binding = workflowModelAdapter.GetBindingInfoByWorkflowInstanceId(taskInfo.WorkflowInstanceId);

                string query = AddCompletedTaskQuery;

                query = query.Replace("param10", taskInfo.TaskId);
                query = query.Replace("param11", taskInfo.WorkflowInstanceId);
                query = query.Replace("param12", taskInfo.CreateTime);
                query = query.Replace("param13", DateTime.Now.ToString());
                query = query.Replace("param14", taskInfo.Subject);
                query = query.Replace("param15", taskInfo.Description);
                query = query.Replace("param16", taskInfo.Instruction);
                query = query.Replace("param17", userName);
                query = query.Replace("param18", taskInfo.ActivityName);
                query = query.Replace("param19", taskInfo.BindingSchemaId);
                query = query.Replace("param20", taskInfo.BindingClassName);
                query = query.Replace("param21", binding.DataInstanceId);
                query = query.Replace("param22", "");
                query = query.Replace("param23", (taskInfo.CustomFormUrl != null ? System.Security.SecurityElement.Escape(taskInfo.CustomFormUrl) : ""));
                query = query.Replace("param24", (taskInfo.FormProperties != null ? taskInfo.FormProperties : ""));

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                interpreter.Query(query);
            }
            catch (Exception ex)
            {
                // igonore the error
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Gets the user's finished tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user mananger</param>
        /// <param name="from">from index</param>
        /// <param name="size">page size</param>
        /// <returns></returns>
        public List<TaskInfo> GetUserFinishedTasks(string schemaId, string userName, int from, int size, IUserManager userManager)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                List<TaskInfo> taskList = new List<TaskInfo>();

                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetCompletedTasksQuery;
 
                query = GetPagedQuery(query, from, size); // replace the variables in the query
                query = query.Replace("param1", schemaId);
                query = query.Replace("param2", userName);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(query);
                TaskInfo taskInfo;

                foreach (XmlElement element in doc.DocumentElement.ChildNodes)
                {
                    taskInfo = new TaskInfo();

                    taskInfo.TaskId = element["TaskId"].InnerText; ;
                    taskInfo.WorkflowInstanceId = element["WorkflowInstanceId"].InnerText;
                    taskInfo.CreateTime = element["CreateTime"].InnerText;
                    taskInfo.FinishTime = element["FinishTime"].InnerText;
                    taskInfo.Subject = element["Subject"].InnerText;
                    taskInfo.Description = element["Description"].InnerText;
                    taskInfo.Instruction = element["Instruction"].InnerText;
                    taskInfo.BindingSchemaId = element["BindingSchemaId"].InnerText;
                    taskInfo.BindingClassName = element["BindingClassName"].InnerText;
                    taskInfo.BindingObjId = element["BindingObjId"].InnerText;
                    taskInfo.Users = element["User"].InnerText;
                    if (element["ActivityName"] != null)
                    {
                        taskInfo.ActivityName = element["ActivityName"].InnerText;
                    }
                    if (element["CustomActions"] != null)
                    {
                        taskInfo.CustomActionsXml = element["CustomActions"].InnerText;
                    }
                    if (element["CustomFormUrl"] != null)
                    {
                        taskInfo.CustomFormUrl = element["CustomFormUrl"].InnerText;
                    }

                    if (element["FormProperties"] != null)
                    {
                        taskInfo.FormProperties = element["FormProperties"].InnerText;
                    }


                    taskList.Add(taskInfo);
                }

                return taskList;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Gets the count of user's finished tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user mananger</param>
        /// <returns>count integer</returns>
        public int GetUserFinishedTaskCount(string schemaId, string userName, IUserManager userManager)
        {
            int count = 0;

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                List<TaskInfo> taskList = new List<TaskInfo>();

                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetCompletedTaskCountQuery;
                query = query.Replace("param1", schemaId);
                query = query.Replace("param2", userName);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                XmlDocument doc = interpreter.Count(query);

                // The content of document root is the count result
                string countStr = doc.DocumentElement.InnerText;

                count = System.Convert.ToInt32(countStr);

                return count;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Gets the finished tasks for a worklfow instance
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="wfInstanceId">The worklfow instance id</param>
        /// <param name="userManager">The user mananger</param>
        /// <returns></returns>
        public List<TaskInfo> GetWorkflowFinishedTasks(string schemaId, string wfInstanceId, IUserManager userManager)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            QueryReader reader = null;

            try
            {
                List<TaskInfo> taskList = new List<TaskInfo>();

                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetCompletedTasksByWorkflowQuery;
                query = query.Replace("param1", schemaId);
                query = query.Replace("param2", wfInstanceId);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                // tell the interpreter that we want a reader that gets the query result in paging mode.
                interpreter.IsPaging = true;
                reader = interpreter.GetQueryReader(query);

                XmlDocument doc = reader.GetNextPage();
                DataSet ds;
                XmlReader xmlReader;
                DataTable allTasks;
                TaskInfo taskInfo;

                while (doc.DocumentElement.ChildNodes.Count > 0)
                {
                    ds = new DataSet();
                    xmlReader = new XmlNodeReader(doc);
                    ds.ReadXml(xmlReader);
                    allTasks = ds.Tables["CompletedTask"];

                    // create a task list from database records
                    if (allTasks != null)
                    {
                        foreach (DataRow dataRow in allTasks.Rows)
                        { 
                            taskInfo = new TaskInfo();

                            taskInfo.TaskId = dataRow["TaskId"].ToString(); ;
                            taskInfo.WorkflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                            taskInfo.CreateTime = dataRow["CreateTime"].ToString();
                            taskInfo.FinishTime = dataRow["FinishTime"].ToString();
                            taskInfo.Subject = dataRow["Subject"].ToString();
                            taskInfo.Description = dataRow["Description"].ToString();
                            taskInfo.Instruction = dataRow["Instruction"].ToString();
                            taskInfo.BindingSchemaId = dataRow["BindingSchemaId"].ToString();
                            taskInfo.BindingClassName = dataRow["BindingClassName"].ToString();
                            taskInfo.BindingObjId = dataRow["BindingObjId"].ToString();
                            taskInfo.Users = dataRow["User"].ToString();
                            if (!dataRow.IsNull("ActivityName"))
                            {
                                taskInfo.ActivityName = dataRow["ActivityName"].ToString();
                            }
                            if (!dataRow.IsNull("CustomActions"))
                            {
                                taskInfo.CustomActionsXml = dataRow["CustomActions"].ToString();
                            }
                            if (!dataRow.IsNull("CustomFormUrl"))
                            {
                                taskInfo.CustomFormUrl = dataRow["CustomFormUrl"].ToString();
                            }

                            if (!dataRow.IsNull("FormProperties"))
                            {
                                taskInfo.FormProperties = dataRow["FormProperties"].ToString();
                            }

                            taskList.Add(taskInfo);
                        }
                    }

                    // get next page of records
                    doc = reader.GetNextPage();
                }

                return taskList;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Clear the user's finished tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user mananger</param>
        /// <returns></returns>
        public void ClearUserFinishedTasks(string schemaId, string userName, IUserManager userManager)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                // delete the task from database
                string query = DeleteCompletedTasksQuery;

                query = query.Replace("param1", schemaId);
                query = query.Replace("param2", userName);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                XmlDocument doc = interpreter.Query(query);
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Create a log entry for a workflow task
        /// </summary>
        /// <param name="instanceKey">The binding data instance key</param>
        /// <param name="instanceDescription">The binding data instance key description</param>
        /// <param name="expectedFinishTime">The expected finish time of the task</param>
        /// <param name="taskTakers">A list of users separaed by semicolon who are assigned to the task</param>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        /// <param name="workflowInstanceId">The id string of the workflow instance</param>
        public void WriteTaskLog(string instanceKey, string instanceDescription, string expectedFinishTime,
            string taskTakers, string activityName, string workflowInstanceId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string wfInstanceId = workflowInstanceId;
                if (string.IsNullOrEmpty(wfInstanceId))
                {
                    wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();
                }
                NewteraWorkflowInstance wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(new Guid(wfInstanceId));
                string projectName = "";
                string projectVersion = "";
                string workflowName = "";
                if (wfInstance != null)
                {
                    projectName = wfInstance.ProjectName;
                    projectVersion = wfInstance.ProjectVersion;
                    workflowName = wfInstance.WorkflowName;
                }

                TaskInfo taskInfo = GetTaskInfo(wfInstanceId, activityName);

                // replace the variables contained in instanceKey, instanceDescription, 
                // or expectedFinishTime with actual values from the binding data instance
                IInstanceWrapper instance = this.GetBindingInstance(wfInstanceId);
                string instanceKey1 = ReplaceVariables(instanceKey, instance);
                string instanceDescription1 = ReplaceVariables(instanceDescription, instance);
                string expectedFinishTime1 = ReplaceVariables(expectedFinishTime, instance);

                string query = AddTaskLogQuery;

                query = query.Replace("param10", (instanceKey1 != null ? instanceKey1 : ""));
                query = query.Replace("param11", (instanceDescription1 != null ? instanceDescription1 : ""));
                query = query.Replace("param12", wfInstanceId);
                query = query.Replace("param13", activityName);
                query = query.Replace("param14", projectName);
                query = query.Replace("param15", projectVersion);
                query = query.Replace("param16", workflowName);
                query = query.Replace("param17", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
                query = query.Replace("param18", (expectedFinishTime1 != null ? expectedFinishTime1 : ""));
                query = query.Replace("param19", (taskTakers != null ? taskTakers : ""));
                query = query.Replace("param20", (taskInfo != null ? taskInfo.TaskId : ""));

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(query);
            }
            catch (Exception ex)
            {
                // do not throw an exception for failure of logging
                ErrorLog.Instance.WriteLine("Write to the log:" + ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Update the corresponding task log's finish time
        /// </summary>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        /// <param name="taskId">The task id</param>
        public void UpdateTaskLog(string activityName, string taskId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string wfInstanceId = WorkflowEnvironment.WorkflowInstanceId.ToString();
                string query = UpdateTaskLogQuery;

                // set the finish time of a task
                query = query.Replace("param10", taskId);
                query = query.Replace("param11", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(query);
            }
            catch (Exception)
            {
                // do not throw an exception for failure of logging
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Get a collection of TaskLogInfo objects of given criteria
        /// </summary>
        /// <param name="instanceId">The binding data instance id or key</param>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        public List<TaskLogInfo> GetTaskLogs(string instanceId, string activityName)
        {
            List<TaskLogInfo> taskLogs = new List<TaskLogInfo>();
            QueryReader reader = null;

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetTaskLogsQuery;

                // set the finish time of a task
                query = query.Replace("param10", instanceId);
                query = query.Replace("param11", activityName);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();

                // tell the interpreter that we want a reader that gets the query result in paging mode.
                interpreter.IsPaging = true;
                reader = interpreter.GetQueryReader(query);

                XmlDocument doc = reader.GetNextPage();
                DataSet ds;
                XmlReader xmlReader;
                DataTable allTaskLogTable;
                TaskLogInfo taskLogInfo;

                while (doc.DocumentElement.ChildNodes.Count > 0)
                {
                    ds = new DataSet();
                    xmlReader = new XmlNodeReader(doc);
                    ds.ReadXml(xmlReader);
                    allTaskLogTable = ds.Tables["TaskExecuteLog"];

                    if (allTaskLogTable != null)
                    {
                        foreach (DataRow dataRow in allTaskLogTable.Rows)
                        {
                            taskLogInfo = new TaskLogInfo();
                            taskLogInfo.LogID = dataRow["LogID"].ToString();
                            taskLogInfo.WorkflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                            taskLogInfo.TaskName = dataRow["TaskName"].ToString();
                            if (!dataRow.IsNull("BindingInstanceDesc"))
                            {
                                taskLogInfo.BindingInstanceDesc = dataRow["BindingInstanceDesc"].ToString();
                            }

                            if (!dataRow.IsNull("ProjectName"))
                            {
                                taskLogInfo.ProjectName = dataRow["ProjectName"].ToString();
                            }

                            if (!dataRow.IsNull("ProjectVersion"))
                            {
                                taskLogInfo.ProjectVersion = dataRow["ProjectVersion"].ToString();
                            }

                            if (!dataRow.IsNull("StartTime"))
                            {
                                taskLogInfo.StartTime = dataRow["StartTime"].ToString();
                            }

                            if (!dataRow.IsNull("TaskTakers"))
                            {
                                taskLogInfo.TaskTakers = dataRow["TaskTakers"].ToString();
                            }

                            if (!dataRow.IsNull("TaskID"))
                            {
                                taskLogInfo.TaskID = dataRow["TaskID"].ToString();
                            }

                            taskLogs.Add(taskLogInfo);
                        }
                    }

                    // get next page of records
                    doc = reader.GetNextPage();
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }

            return taskLogs;
        }

        /// <summary>
        /// Remove a TaskInfo from the cache
        /// </summary>
        /// <param name="taskId">The id of the task to be removed</param>
        public void DeleteTaskFromCache(string schemaId, string taskId, string userName)
        {
            TaskInfo foundTaskInfo = null;

            lock (UserTaskCache.Instance)
            {
                List<TaskInfo> allTasks = UserTaskCache.Instance.GetSchemaTasks(schemaId);
                if (allTasks != null)
                {
                    // remove the task info from the cached list
                    foreach (TaskInfo cachedTaskInfo in allTasks)
                    {
                        if (cachedTaskInfo.TaskId == taskId)
                        {
                            foundTaskInfo = cachedTaskInfo;
                            break;
                        }
                    }

                    if (foundTaskInfo != null)
                    {
                        allTasks.Remove(foundTaskInfo);

                        // remove it from the user's cache
                        UserTaskCache.Instance.RemoveUserTask(userName, schemaId, foundTaskInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all tasks of a schema from DB
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A list of tasks for a schema.</returns>
        private List<TaskInfo> GetAllTasks(string schemaId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            QueryReader reader = null;

            try
            {
                List<TaskInfo> taskList = new List<TaskInfo>();

                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetTasksQuery;
                query = query.Replace("param1", schemaId);

                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                // tell the interpreter that we want a reader that gets the query result in paging mode.
                interpreter.IsPaging = true;
                reader = interpreter.GetQueryReader(query);

                XmlDocument doc = reader.GetNextPage();
                DataSet ds;
                XmlReader xmlReader;
                DataTable allTasks;
                TaskInfo taskInfo;

                while (doc.DocumentElement.ChildNodes.Count > 0)
                {
                    ds = new DataSet();
                    xmlReader = new XmlNodeReader(doc);
                    ds.ReadXml(xmlReader);
                    allTasks = ds.Tables["Task"];

                    string workflowInstanceId;
                    NewteraWorkflowInstance wfInstance;

                    // create a task list from database records
                    if (allTasks != null)
                    {
                        foreach (DataRow dataRow in allTasks.Rows)
                        {
                            workflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                            wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(new Guid(workflowInstanceId));
                            // do not add tasks that do not have a live workflow instance
                            if (wfInstance != null)
                            {
                                taskInfo = new TaskInfo();

                                taskInfo.TaskId = dataRow["TaskId"].ToString(); ;
                                taskInfo.WorkflowInstanceId = workflowInstanceId;
                                taskInfo.CreateTime = dataRow["CreateTime"].ToString();
                                taskInfo.Subject = dataRow["Subject"].ToString();
                                taskInfo.Description = dataRow["Description"].ToString();
                                taskInfo.Instruction = dataRow["Instruction"].ToString();
                                taskInfo.BindingSchemaId = dataRow["BindingSchemaId"].ToString();
                                taskInfo.BindingClassName = dataRow["BindingClassName"].ToString();
                                taskInfo.Users = dataRow["Users"].ToString();
                                taskInfo.Roles = dataRow["Roles"].ToString();
                                if (!dataRow.IsNull("ActivityName"))
                                {
                                    taskInfo.ActivityName = dataRow["ActivityName"].ToString();
                                }
                                if (!dataRow.IsNull("CustomActions"))
                                {
                                    taskInfo.CustomActionsXml = dataRow["CustomActions"].ToString();
                                }
                                if (!dataRow.IsNull("CustomFormUrl"))
                                {
                                    taskInfo.CustomFormUrl = dataRow["CustomFormUrl"].ToString();
                                }

                                if (!dataRow.IsNull("FormProperties"))
                                {
                                    taskInfo.FormProperties = dataRow["FormProperties"].ToString();
                                }
                                if (!dataRow.IsNull("Visible") && !string.IsNullOrEmpty(dataRow["Visible"].ToString()))
                                {
                                    taskInfo.IsVisible = bool.Parse(dataRow["Visible"].ToString());
                                }

                                taskList.Add(taskInfo);
                            }
                        }
                    }

                    // get next page of records
                    doc = reader.GetNextPage();
                }

                return taskList;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        private List<TaskInfo> GetUserTasksFromAllTasks(string userName, IUserManager userManager, List<TaskInfo> allTasks)
        {
            List<TaskInfo> userTasks = new List<TaskInfo>();
            string workflowInstanceId;
            NewteraWorkflowInstance wfInstance;
            Hashtable taskUsersTable = new Hashtable();
            Hashtable taskSubstituteTable = new Hashtable();

            // filter the tasks that do not belong to the given user
            if (allTasks != null)
            {
                StringCollection reassignedTaskIds = WorkflowModelCache.Instance.GetReassignedTaskIds(userName, _dataProvider);
                foreach (TaskInfo taskInfo in allTasks)
                {
                    if (reassignedTaskIds.Contains(taskInfo.TaskId))
                    {
                        // create a copy of the cached taskInfo to avoid memory leak of List<>
                        userTasks.Add(taskInfo.Clone());
                    }
                    else if (!string.IsNullOrEmpty(taskInfo.WorkflowInstanceId))
                    {
                        workflowInstanceId = taskInfo.WorkflowInstanceId;
                        wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(new Guid(workflowInstanceId));
                        if (wfInstance != null &&
                            IsUserTask(taskInfo, userName, userManager, wfInstance.ProjectName, wfInstance.ProjectVersion, wfInstance.WorkflowName, taskUsersTable, taskSubstituteTable))
                        {
                            // check if the task has been reassigned to other user
                            string substituteUser = WorkflowModelCache.Instance.GetTaskSubstitute(taskInfo.TaskId, userName, _dataProvider);
                            if (string.IsNullOrEmpty(substituteUser))
                            {
                                // create a copy of the cached taskInfo to avoid memory leak of List<>
                                userTasks.Add(taskInfo.Clone());
                            }
                        }
                    }
                }
            }

            return userTasks;
        }

        /// <summary>
        /// Add a new task to the cache so that the data in the cache is in sync with that in database
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="taskInfo">The new task info object</param>
        private void AddTaskInfoToCache(string schemaId, TaskInfo taskInfo)
        {
            TaskInfo foundTaskInfo = null;

            lock (UserTaskCache.Instance)
            {
                List<TaskInfo> allTasks = UserTaskCache.Instance.GetSchemaTasks(schemaId);
                if (allTasks == null)
                {
                    // build a list of task infos which should already include the new task info
                    allTasks = GetAllTasks(schemaId);

                    UserTaskCache.Instance.SetSchemaTasks(schemaId, allTasks);
                }

                // do not insert duplicated taskinfo
                foreach (TaskInfo cachedTaskInfo in allTasks)
                {
                    if (cachedTaskInfo.TaskId == taskInfo.TaskId)
                    {
                        foundTaskInfo = cachedTaskInfo;
                        break;
                    }
                }

                if (foundTaskInfo == null)
                {
                    // add the new task info to the head of the cached list
                    allTasks.Insert(0, taskInfo);
                }
            }

            if (foundTaskInfo == null)
            {
                StringCollection taskReceivers = GetTaskReceivers(taskInfo);
                foreach (string userName in taskReceivers)
                {
                    UserTaskCache.Instance.AddUserTask(userName, schemaId, taskInfo); // add to the existing tasklist
                }
            }
        }

        /// <summary>
        /// Remove a task from the cache so that the data in the cache is in sync with that in database
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="taskInfo">The new task info object</param>
        private void RemoveTaskInfoToCache(string schemaId, TaskInfo taskInfo)
        {
            TaskInfo foundTaskInfo = null;

            lock (UserTaskCache.Instance)
            {
                List<TaskInfo> allTasks = UserTaskCache.Instance.GetSchemaTasks(schemaId);
                if (allTasks != null)
                {
                    // remove the new task info from the cached list
                    foreach (TaskInfo cachedTaskInfo in allTasks)
                    {
                        if (cachedTaskInfo.TaskId == taskInfo.TaskId)
                        {
                            foundTaskInfo = cachedTaskInfo;
                            break;
                        }
                    }

                    if (foundTaskInfo != null)
                    {
                        allTasks.Remove(foundTaskInfo);

                        // remember the task executor name if exists
                        if (string.IsNullOrEmpty(foundTaskInfo.UserId) && !string.IsNullOrEmpty(taskInfo.UserId))
                        {
                            foundTaskInfo.UserId = taskInfo.UserId;
                        }
                    }
                }
            }

            if (foundTaskInfo != null)
            {
                // Get all receivers of the task
                StringCollection taskReceivers = GetTaskReceivers(foundTaskInfo);
                StringCollection taskExecutors = null;
                if (!string.IsNullOrEmpty(foundTaskInfo.UserId))
                {
                    // if the user who executed the task is remembered in the task info
                    // make the task as a completed task for the user
                    taskExecutors = new StringCollection();
                    taskExecutors.Add(foundTaskInfo.UserId);
                }
                else
                {
                    // otherwise, make the task as a completed task for all task receivers
                    taskExecutors = taskReceivers;
                }

                foreach (string userName in taskReceivers)
                {
                    // clear the user's cache so that the task list will be regenerated later
                    UserTaskCache.Instance.RemoveUserTask(userName, schemaId, foundTaskInfo);
                }

                // save finished tasks async
                CompletedTaskSaver taskSaver = new CompletedTaskSaver(foundTaskInfo, taskExecutors);
                taskSaver.Run(); // return right away
            }
        }

        /// <summary>
        /// Gets the information indicating whether a TaskInfo object represents a task for the given user.
        /// </summary>
        /// <param name="taskInfo">Task Info object</param>
        /// <param name="userName">User name</param>
        /// <param name="userManager"></param>
        /// <param name="projectName"></param>
        /// <param name="projectVersion"></param>
        /// <returns>true if it belongs to the user, false otherwise.</returns>
        private bool IsUserTask(TaskInfo taskInfo, string userName, IUserManager userManager,
            string projectName, string projectVersion, string workflowName, Hashtable taskUsersTable, Hashtable taskSubstituteTable)
        {
            bool status = false;
            StringCollection substituteUsers;

            // Check if the task has only one user and doesn't have any roles, if so, compare the task user
            // with the given user name directly
            if (IsSingleUserTask(taskInfo))
            {
                string taskUser = taskInfo.Users.Trim();

                // check if the user has enabled task reassignment rules
                string key = projectName + projectVersion + taskUser; // single user

                if (!taskSubstituteTable.ContainsKey(key))
                {
                    TaskSubstituteModel taskSubstituteModel = WorkflowModelCache.Instance.GetTaskSubstituteModel(_dataProvider);

                    // Next, get substitute users based on the rules which are pre-defined by administrator
                    substituteUsers = taskSubstituteModel.GetSubstituteUsers(taskUser, projectName, projectVersion);

                    if (substituteUsers != null && substituteUsers.Count > 0)
                    {
                        taskSubstituteTable[key] = substituteUsers; // save it in hashtable for better performance
                    }
                    else
                    {
                        taskSubstituteTable[key] = new StringCollection();
                    }
                }

                substituteUsers = taskSubstituteTable[key] as StringCollection;

                if (substituteUsers.Contains(userName))
                {
                    status = true;
                }
                else if (substituteUsers.Count == 0 &&
                    taskUser == userName)
                {
                    status = true; // no substitute, compare the name
                }
            }
            else
            {
                // first get user collection for a workflow activity from the hashtable
                string activityId = projectName + projectVersion + workflowName + taskInfo.ActivityName + taskInfo.Users + taskInfo.Roles;

                StringCollection actualUsers = taskUsersTable[activityId] as StringCollection;

                if (actualUsers == null)
                {
                    StringCollection users = new StringCollection();
                    StringCollection roles = new StringCollection();

                    users = ConvertToStringCollection(taskInfo.Users);
                    roles = ConvertToStringCollection(taskInfo.Roles);

                    StringCollection assignedUsers = this.GetQualifiedUsers(users, roles, userManager);

                    // replace the users with substitute users according to the substitute rules
                    actualUsers = new StringCollection();
                    TaskSubstituteModel taskSubstituteModel = WorkflowModelCache.Instance.GetTaskSubstituteModel(_dataProvider);
                    foreach (string user in assignedUsers)
                    {
                        // Next, get substitute users based on the rules which are pre-defined by administrator
                        substituteUsers = taskSubstituteModel.GetSubstituteUsers(user, projectName, projectVersion);

                        // if the user doesn't have appointed any substitute uses, it will be null collection
                        if (substituteUsers == null || substituteUsers.Count == 0)
                        {
                            actualUsers.Add(user);
                        }
                        else
                        {
                            // add the substitute users to the actual user list
                            foreach (string substitute in substituteUsers)
                            {
                                if (!actualUsers.Contains(substitute))
                                {
                                    actualUsers.Add(substitute);
                                }
                            }
                        }
                    }

                    // save the collection in the table
                    taskUsersTable[activityId] = actualUsers;
                }

                if (actualUsers.Contains(userName))
                {
                    status = true;
                }
            }

            return status;
        }

        private bool IsSingleUserTask(TaskInfo taskInfo)
        {
            bool status = false;

            if (!string.IsNullOrEmpty(taskInfo.Users) && string.IsNullOrEmpty(taskInfo.Roles))
            {
                if (!taskInfo.Users.Contains(";"))
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets a task by objId
        /// </summary>
        /// <param name="objId">obj id of a task</param>
        /// <returns>A TaskInfo object</returns>
        private TaskInfo GetTaskByObjId(string objId)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;
            TaskInfo taskInfo = null;
            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetTaskByObjIdQuery;

                query = query.Replace("(obj_id)", objId);
                
                Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
                XmlDocument doc = interpreter.Query(query);

                DataSet ds = new DataSet();
                XmlReader xmlReader = new XmlNodeReader(doc);
                ds.ReadXml(xmlReader);
                DataTable taskTable = ds.Tables["Task"];

                if (taskTable != null && taskTable.Rows.Count == 1)
                {
                    taskInfo = new TaskInfo();
                    DataRow dataRow = taskTable.Rows[0];
                    taskInfo.TaskId = dataRow["TaskId"].ToString();
                    taskInfo.WorkflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                    taskInfo.CreateTime = dataRow["CreateTime"].ToString();
                    taskInfo.Subject = dataRow["Subject"].ToString();
                    taskInfo.Description = dataRow["Description"].ToString();
                    taskInfo.Instruction = dataRow["Instruction"].ToString();
                    taskInfo.BindingSchemaId = dataRow["BindingSchemaId"].ToString();
                    taskInfo.BindingClassName = dataRow["BindingClassName"].ToString();
                    taskInfo.Users = dataRow["Users"].ToString();
                    taskInfo.Roles = dataRow["Roles"].ToString();
                    if (!dataRow.IsNull("ActivityName"))
                    {
                        taskInfo.ActivityName = dataRow["ActivityName"].ToString();
                    }
                    if (!dataRow.IsNull("CustomActions"))
                    {
                        taskInfo.CustomActionsXml = dataRow["CustomActions"].ToString();
                    }
                    if (!dataRow.IsNull("CustomFormUrl"))
                    {
                        taskInfo.CustomFormUrl = dataRow["CustomFormUrl"].ToString();
                    }
                    if (!dataRow.IsNull("FormProperties"))
                    {
                        taskInfo.FormProperties = dataRow["FormProperties"].ToString();
                    }
                    if (!dataRow.IsNull("Visible") && !string.IsNullOrEmpty(dataRow["Visible"].ToString()))
                    {
                        taskInfo.IsVisible = bool.Parse(dataRow["Visible"].ToString());
                    }
                }

                return taskInfo;
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Get a collection of user names who are actual owners of the given task
        /// </summary>
        /// <param name="taskId">The id of the task</param>
        /// <param name="users">The user collection sepcified for the task</param>
        /// <param name="roles">The role collection specified  for the task</param>
        /// <param name="workflowInstanceId">the workflow instance id that the task is associated</param>
        /// <returns>A list of users who receive the task</returns>
        public StringCollection GetTaskReceivers(string taskId, StringCollection users, StringCollection roles, string workflowInstanceId)
        {
            NewteraWorkflowInstance wfInstance;

            StringCollection tempTaskReceivers = GetQualifiedUsers(users, roles, _userManager);
            StringCollection taskReceivers = new StringCollection();

            StringCollection substitudeUsers = new StringCollection();
            foreach (string userName in tempTaskReceivers)
            {
                // add substitute user as a task receiver using the rules created dynamically
                string substituteUser = WorkflowModelCache.Instance.GetTaskSubstitute(taskId, userName, _dataProvider);
                if (!string.IsNullOrEmpty(substituteUser))
                {
                    substitudeUsers.Add(substituteUser);
                }
            }

            // add substitude users to the task receiver collection
            foreach (string substituteUser in substitudeUsers)
            {
                AddUser(tempTaskReceivers, substituteUser);
            }

            wfInstance = NewteraWorkflowRuntime.Instance.FindWorkflowInstance(new Guid(workflowInstanceId));

            if (wfInstance != null)
            {
                // replace the users with substitute users according to the substitute rules
                taskReceivers = GetActualUsers(taskId, tempTaskReceivers, wfInstance.ProjectName, wfInstance.ProjectVersion);
            }

            return taskReceivers;
        }

        /// <summary>
        /// Get a collection of user names who are owners of the given task
        /// </summary>
        /// <param name="taskInfo">The task info</param>
        /// <returns>A collection of the user names</returns>
        public StringCollection GetTaskReceivers(TaskInfo taskInfo)
        {
            StringCollection users = ConvertToStringCollection(taskInfo.Users);
            StringCollection roles = ConvertToStringCollection(taskInfo.Roles);

            return GetTaskReceivers(taskInfo.TaskId, users, roles, taskInfo.WorkflowInstanceId);
        }

        private string ConvertToArrayString(StringCollection items)
        {
            StringBuilder builder = new StringBuilder();

            if (items != null)
            {
                int index = 0;
                foreach (string item in items)
                {
                    if (index == 0)
                    {
                        builder.Append(item);
                    }
                    else
                    {
                        builder.Append(";").Append(item);
                    }

                    index++;
                }
            }

            return builder.ToString();
        }

        private StringCollection ConvertToStringCollection(string stringArray)
        {
            StringCollection itemCollection = null;

            if (stringArray != null && stringArray.Length > 0)
            {
                itemCollection = new StringCollection();
                string[] items = stringArray.Split(';');
                for (int i = 0; i < items.Length; i++)
                {
                    itemCollection.Add(items[i]);
                }
            }

            return itemCollection;
        }

        // add a distinct user to the collection
        private void AddUser(StringCollection users, string user)
        {
            bool status = false;

            foreach (string usr in users)
            {
                if (usr == user)
                {
                    status = true;
                    break;
                }
            }

            if (!status)
            {
                users.Add(user);
            }
        }

        /// <summary>
        /// Gets the binding data instance of a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        /// <returns>IInstanceWrapper object</returns>
        private IInstanceWrapper GetBindingInstance(string workflowInstanceId)
        {
            IInstanceWrapper wrapped = null;

            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();

            WorkflowInstanceBindingInfo binding = workflowModelAdapter.GetBindingInfoByWorkflowInstanceId(workflowInstanceId);

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            SchemaInfo[] schemaInfos = MetaDataCache.Instance.GetSchemaInfos(dataProvider);
            SchemaInfo theSchemaInfo = null;
            foreach (SchemaInfo schemaInfo in schemaInfos)
            {
                if (schemaInfo.NameAndVersion == binding.SchemaId)
                {
                    theSchemaInfo = schemaInfo;
                    break;
                }
            }

            if (theSchemaInfo == null)
            {
                throw new InvalidDataException("The schema " + binding.SchemaId + " doesn't exist in the database anymore.");
            }

            // build a query for getting the binding instance
            MetaDataModel metaData = MetaDataCache.Instance.GetMetaData(theSchemaInfo, dataProvider);
            DataViewModel dataView = metaData.GetDetailedDataView(binding.DataClassName);
            if (dataView == null)
            {
                throw new InvalidDataException("The class " + binding.DataClassName + " doesn't exist in the schema " + binding.SchemaId + " anymore.");
            }

            // get the instance
            string query = dataView.GetInstanceQuery(binding.DataInstanceId);
            Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
            XmlDocument doc = interpreter.Query(query);
            XmlReader reader = new XmlNodeReader(doc);
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (DataSetHelper.IsEmptyDataSet(ds, binding.DataClassName))
            {
                throw new InvalidDataException("The data instance with id " + binding.DataInstanceId + " doesn't exist. It may be deleted.");
            }

            // Create an instance view
            InstanceView instanceView = new InstanceView(dataView, ds);

            wrapped = new InstanceWrapper(instanceView);

            return wrapped;
        }

        private string ReplaceVariables(string pattern, IInstanceWrapper instance)
        {
            string text = pattern;

            if (!string.IsNullOrEmpty(text))
            {
                Regex patternExp = new Regex(NewteraTaskService.VariablePattern);

                MatchCollection matches = patternExp.Matches(pattern);
                if (matches.Count > 0)
                {
                    // contains variables
                    string propertyName;
                    string propertyValue;
                    if (instance != null)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Value.Length > 2)
                            {
                                // variable is in form of {propertyName}
                                propertyName = match.Value.Substring(1, (match.Value.Length - 2));
                                try
                                {
                                    propertyValue = instance.GetString(propertyName);
                                    if (!string.IsNullOrEmpty(propertyValue))
                                    {
                                        // replace the variable with value
                                        text = text.Replace(match.Value, propertyValue);
                                    }
                                }
                                catch (Exception)
                                {
                                    // ignore
                                }
                            }
                        }
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Gets a task by workflow instance id and activity name
        /// </summary>
        /// <param name="wfInstanceId">The workflow instance id</param>
        /// <param name="activityName">The activity name</param>
        /// <returns>A TaskInfo object, null if not exist</returns>
        private TaskInfo GetTaskInfo(string wfInstanceId, string activityName)
        {
            TaskInfo taskInfo = null;

            string query = GetTaskByWfIdAndActivityQuery;

            query = query.Replace("param1", wfInstanceId);
            query = query.Replace("param2", activityName);

            Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
            XmlDocument doc = interpreter.Query(query);

            DataSet ds = new DataSet();
            XmlReader xmlReader = new XmlNodeReader(doc);
            ds.ReadXml(xmlReader);
            DataTable taskTable = ds.Tables["Task"];

            if (taskTable != null && taskTable.Rows.Count == 1)
            {
                taskInfo = new TaskInfo();
                DataRow dataRow = taskTable.Rows[0];
                taskInfo.TaskId = dataRow["TaskId"].ToString();
                taskInfo.WorkflowInstanceId = dataRow["WorkflowInstanceId"].ToString();
                taskInfo.CreateTime = dataRow["CreateTime"].ToString();
                taskInfo.Subject = dataRow["Subject"].ToString();
                taskInfo.Description = dataRow["Description"].ToString();
                taskInfo.Instruction = dataRow["Instruction"].ToString();
                taskInfo.BindingSchemaId = dataRow["BindingSchemaId"].ToString();
                taskInfo.BindingClassName = dataRow["BindingClassName"].ToString();
                taskInfo.Users = dataRow["Users"].ToString();
                taskInfo.Roles = dataRow["Roles"].ToString();
                if (!dataRow.IsNull("ActivityName"))
                {
                    taskInfo.ActivityName = dataRow["ActivityName"].ToString();
                }
                if (!dataRow.IsNull("CustomActions"))
                {
                    taskInfo.CustomActionsXml = dataRow["CustomActions"].ToString();
                }
                if (!dataRow.IsNull("CustomFormUrl"))
                {
                    taskInfo.CustomFormUrl = dataRow["CustomFormUrl"].ToString();
                }
                if (!dataRow.IsNull("FormProperties"))
                {
                    taskInfo.FormProperties = dataRow["FormProperties"].ToString();
                }
                if (!dataRow.IsNull("Visible") && !string.IsNullOrEmpty(dataRow["Visible"].ToString()))
                {
                    taskInfo.IsVisible = bool.Parse(dataRow["Visible"].ToString());
                }
            }

            return taskInfo;
        }

        // get the information whether the first task list contains the same number of TaskInfo objects and the second one does
        private bool IsTaskListModified(List<TaskInfo> firstTaskList, List<TaskInfo> secondTaskList)
        {
            bool status = false;

            if (firstTaskList.Count != secondTaskList.Count)
            {
                status = true; // lazy check
            }

            return status;
        }

        /// <summary>
        /// Replace the start and finish parameters in a query to get a paged query
        /// </summary>
        /// <param name="originalQuery">The original query</param>
        /// <param name="pageIndex">The page index</param>
        /// <returns>The paged query</returns>
        private string GetPagedQuery(string originalQuery, int pageIndex, int pageSize)
        {
            int start = pageSize * pageIndex + 1;
            int end = pageSize * (pageIndex + 1);

            string query = originalQuery.Replace("start", start.ToString());
            query = query.Replace("finish", end.ToString());

            return query;
        }

        private string GetTemplatedBody(string body)
        {
            string newBody = body;

            // for web client
            string templateDir = NewteraNameSpace.GetAppHomeDir();
            if (templateDir.EndsWith(@"\"))
            {
                templateDir += EMAIL_TEMPLATES_DIR;
            }
            else
            {
                templateDir +=  @"\" + EMAIL_TEMPLATES_DIR;
            }

            if (Directory.Exists(templateDir))
            {
                string templatePath = templateDir + DEFAULT_EMAIL_TEMPLATE;

                if (File.Exists(templatePath))
                {
                    string templateText = File.ReadAllText(templatePath);
                    if (!string.IsNullOrEmpty(templateText) && body != null)
                    {
                        newBody = templateText.Replace(BODY_VARIABLE, body);
                    }
                }
            }

            return newBody;
        }

        private System.Text.Encoding GetEmailEncoding(string encodingString)
        {
            System.Text.Encoding encoding = System.Text.Encoding.Default;

            switch (encodingString)
            {
                case "UTF8":
                case "utf-8":
                    encoding = System.Text.Encoding.UTF8;
                    break;
                case "Unicode":
                case "UNICODE":
                    encoding = System.Text.Encoding.Unicode;
                    break;
                case "ASCII":
                case "ascii":
                    encoding = System.Text.Encoding.ASCII;
                    break;
                case "UTF7":
                case "utf-7":
                    encoding = System.Text.Encoding.UTF7;
                    break;
                case "GB2312":
                    encoding = System.Text.Encoding.GetEncoding("GB2312");
                    if (encoding == null)
                    {
                        encoding = System.Text.Encoding.Default;
                    }
                    break;
            }

            return encoding;
        }
    }
}

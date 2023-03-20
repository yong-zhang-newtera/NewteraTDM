/*
* @(#)WFConnection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;
    using System.Text;
    using System.IO;
	using System.Threading;
	using System.Resources;
    using System.Collections.Generic;
	using System.Security.Principal;
	using System.ComponentModel;
    using System.Runtime.Remoting;
    using System.Security;

	using Newtera.Common.Core;
    using Newtera.Common.Wrapper;
	using Newtera.Server.DB;
    using Newtera.Server.Engine.Workflow;
	using Newtera.Server.Engine.Cache;
    using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.UsrMgr;
    using Newtera.WorkflowServices;
    using Newtera.Common.MetaData.Principal;
    using Newtera.WFModel;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a connection to the Workflow server.
	/// </summary>
	/// <version>  	1.0.0 15 Dec 2006 </version>
	public class WFConnection : ConnectionBase, IDisposable
	{
        #region common

        private DateTime _modifiedTime;

        /// <summary> 
		/// Default constructor
		/// </summary>
		public WFConnection() : base()
		{
		}
		
		/// <summary>
		/// The constructor that takes a connection string
		/// </summary>
		/// <param name="connectionString">the connection string </param>
		public WFConnection(string connectionString) : base()
		{
            _properties = GetProperties(connectionString);

            if (!string.IsNullOrEmpty((string)_properties[TIMESTAMP]))
            {
                _modifiedTime = DateTime.Parse((string)_properties[TIMESTAMP]);
            }
		}

		/// <summary> close the connection in case that the application forgets to do so.
		/// This method will be called by GC
		/// </summary>
		~WFConnection()
		{
			this.Close();
		}

        /// <summary>
        /// Implementing IDisposal interface
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            /*
             * Dispose of the object and perform any cleanup.
             */
            this.Close();
        }

        #endregion Common

        #region Workflow

        /// <summary>
        /// Get ProjectInfos of existing workflow projects.
        /// </summary>
        /// <value>
        /// An array of project infos
        /// </value>
        public ProjectInfo[] WorkflowProjectInfos
        {
            get
            {
                return WorkflowModelCache.Instance.GetProjectInfos(_dataProvider);
            }
        }

        /// <summary>
        /// Gets the project model from database as xml string
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        public string GetProject(string projectName, string projectVersion)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            ProjectModel projectModel = WorkflowModelCache.Instance.GetProjectModel(projectName, projectVersion, _dataProvider);
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            projectModel.Write(writer);

            return builder.ToString();
        }

        /// <summary>
        /// Gets the project's access policy from database as xml string
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        public string GetProjectPolicy(string projectName, string projectVersion)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            ProjectModel projectModel = WorkflowModelCache.Instance.GetProjectModel(projectName, projectVersion, _dataProvider);
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            projectModel.Policy.Write(writer);

            return builder.ToString();
        }

        /// <summary>
        /// Update the project model saved in database
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="projectXml">The project xml</param>
        /// <param name="policyXml">The xacl policy xml</param>
        /// <returns>The project model modified time</returns>
        public DateTime UpdateProject(string projectName, string projectVersion, string projectXml, string policyXml)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            return WorkflowModelCache.Instance.UpdateProjectModel(projectName, projectVersion, _modifiedTime, projectXml, policyXml, _dataProvider);
        }

        /// <summary>
        /// Delete a project model from the database
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        public void DeleteProject(string projectName, string projectVersion)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelCache.Instance.DeleteProjectModel(projectName, projectVersion, _dataProvider);
        }

        /// <summary>
        /// Gets the indicated data of the given workflow from database
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataTypeStr">Data type string</param>
        public string GetWorkflowData(string projectName, string projectVersion, string workflowName, string dataTypeStr)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            // dataType string has to match with the one defined in WorkflowDataType enum
            WorkflowDataType dataType = (WorkflowDataType) Enum.Parse(typeof(WorkflowDataType), dataTypeStr);
            return GetWorkflowData(projectName, projectVersion, workflowName, dataType);
        }

        /// <summary>
        /// Gets the indicated data of the given workflow from database
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataType">Data type</param>
        private string GetWorkflowData(string projectName, string projectVersion, string workflowName, WorkflowDataType dataType)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            return WorkflowModelCache.Instance.GetWorkflowData(projectName, projectVersion, workflowName, dataType, _dataProvider);
        }

        /// <summary>
        /// Sets the indicated data of the given workflow to database
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataTypeStr">Data type string</param>
        /// <param name="dataString">The data string</param>
        public void SetWorkflowData(string projectName, string projectVersion, string workflowName,
            string dataTypeStr, string dataString)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            // dataType string has to match with the one defined in WorkflowDataType enum
            WorkflowDataType dataType = (WorkflowDataType) Enum.Parse(typeof(WorkflowDataType), dataTypeStr);
            SetWorkflowData(projectName, projectVersion, workflowName, dataType, dataString);
        }

        /// <summary>
        /// Sets the indicated data of the given workflow to database
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataType">Data type string</param>
        /// <param name="dataString">The data string</param>
        private void SetWorkflowData(string projectName, string projectVersion, string workflowName,
            WorkflowDataType dataType, string dataString)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelCache.Instance.SetWorkflowData(projectName, projectVersion, workflowName, dataType, dataString, _dataProvider);
        }

        /// <summary>
        /// Starte a workflow instance
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <returns>The workflow instance id</returns>
        public Guid StartWorkflow(string projectName, string projectVersion, string workflowName)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            return NewteraWorkflowRuntime.Instance.StartWorkflowInstance(projectName, projectVersion, workflowName, null, _dataProvider);
        }

        /// <summary>
        /// Gets the information indicating whether a workflow model has running instances.
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>true if it has running instances, false otherwise.</returns>
        public bool HasRunningInstances(string projectName, string projectVersion, string workflowId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.HasRunningInstances(projectName, projectVersion, workflowId);
        }

        /// <summary>
        /// Gets the id of a workflow model given the name.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow Name</param>
        /// <returns>The id of the found workflow model, null if the workflow model does not exist.</returns>
        public string GetWorkflowModelID(string projectName, string projectVersion, string workflowName)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.GetWorkflowModelID(projectName, projectVersion, workflowName);
        }

        /// <summary>
        /// Gets the information indicating whether a project is the latest version.
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <returns>true if it is the latest version, false otherwise.</returns>
        public bool IsLatestVersion(string projectName, string projectVersion)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            return WorkflowModelCache.Instance.IsLatestVersion(projectName, projectVersion, _dataProvider);
        }

        /// <summary>
        /// Gets the information indicating whether a XQuery is valid.
        /// An xquery kept in a workflow activity may become invalid if the database schema is changed.
        /// </summary>
        /// <param name="query">The xquery</param>
        /// <returns>true if it is valid, false otherwise.</returns>
        public bool IsQueryValid(string query)
        {
            bool status = true;

            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.GetQueryReader(query); // parse the query without executing it
            }
            catch (Exception )
            {
                // something wrong with the query
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the format of a custom function definition is correct.
        /// </summary>
        /// <param name="functionDefinition">A custom function definition</param>
        /// <returns>true if it is valid, false otherwise.</returns>
        public bool IsValidCustomFunctionDefinition(string functionDefinition)
        {
            bool status = false;

            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            ICustomFunction function = null;

            if (!string.IsNullOrEmpty(functionDefinition))
            {
                int index = functionDefinition.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = functionDefinition.Substring(0, index).Trim();
                    assemblyName = functionDefinition.Substring(index + 1).Trim();
                }
                else
                {
                    className = functionDefinition.Trim();
                }

                try
                {

                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    function = (ICustomFunction)obj.Unwrap();
                    if (function != null)
                    {
                        status = true;
                    }
                }
                catch
                {
                }
            }

            return status;
        }

        /// <summary>
        /// Validate the action code.
        /// </summary>
        /// <param name="actionCode">The action code</param>
        /// <param name="schemaId">The schema id indicates the schema where the instance class resides</param>
        /// <param name="instanceClassName">The class name of the instance to which the action code is run against</param>
        /// <returns>Error message if the action code is invalid, null if the action code is valid.</returns>
        public string ValidateActionCode(string actionCode, string schemaId, string instanceClassName)
        {
            return ActionCodeRunner.Instance.CompileActionCode(actionCode, schemaId, instanceClassName);
        }

        /// <summary>
        /// Lock a project for update
        /// </summary>
        /// <param name="projectName">Name of the project to be locked</param>
        /// <param name="projectVersion">The project version</param>
        /// <exception cref="LockProjectException">Thrown when the project has been locked by another user.</exception>
        public void LockProject(string projectName, string projectVersion)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelCache.Instance.LockProjectModel(projectName, projectVersion, _modifiedTime, _dataProvider);
        }

        /// <summary>
        /// Unlock the project
        /// </summary>
        /// <param name="projectName">Name of the project to be unlocked.</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        /// <exception cref="LockProjectException">Thrown when the user doesn't have a right to unlock the project.</exception>
        public void UnlockProject(string projectName, string projectVersion, bool forceUnlock)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelCache.Instance.UnlockProjectModel(projectName, projectVersion, _dataProvider, forceUnlock);
        }

        /// <summary>
        /// Gets a count of workflow tracking instances from Newtera Tracking store
        /// </summary>
        /// <param name="workflowTypeId">The unique id of workflow type to be matched.</param>
        /// <param name="workflowEvent">The workflow event to be matched</param>
        /// <param name="from">The from time to be matched</param>
        /// <param name="until">The until time to be matched</param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <returns>Total count</returns>
        public int GetTrackingWorkflowInstanceCount(string workflowTypeId,
            string workflowEvent, DateTime from, DateTime until, bool useCondition)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraTrackingQuery trackingQuery = new NewteraTrackingQuery();

            return trackingQuery.GetTrackingWorkflowInstanceCount(workflowTypeId, workflowEvent, from, until, useCondition);
        }

        /// <summary>
        /// Gets a collection of NewteraTrackingWorkflowInstance from Newtera Tracking store
        /// </summary>
        /// <param name="workflowTypeId">The unique id of workflow type to be matched.</param>
        /// <param name="workflowEvent">The workflow event to be matched</param>
        /// <param name="from">The from time to be matched</param>
        /// <param name="until">The until time to be matched</param>
        /// <param name="useCondition">Information indicating whether to use the provided condition for querying.</param>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>NewteraTrackingWorkflowInstanceCollection object</returns>
        public NewteraTrackingWorkflowInstanceCollection GetTrackingWorkflowInstances(string workflowTypeId,
            string workflowEvent, DateTime from, DateTime until, bool useCondition, int pageIndex, int pageSize)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraTrackingQuery trackingQuery = new NewteraTrackingQuery();

            return trackingQuery.GetTrackingWorkflowInstances(workflowTypeId, workflowEvent, from, until, useCondition, pageIndex, pageSize);
        }

        /// <summary>
        /// Sets a collection of NewteraTrackingWorkflowInstance
        /// for a given workflow type to database. Used by the project restore tool
        /// </summary>
        /// <param name="workflowTypeId">The unique workflow internal type id</param>
        /// <param name="workflowTrackingInfos">A collection of NewteraTrackingWorkflowInstance.</param>
        public void SetTrackingWorkflowInstances(string workflowTypeId,
            NewteraTrackingWorkflowInstanceCollection workflowTrackingInfos)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraTrackingQuery trackingQuery = new NewteraTrackingQuery();

            trackingQuery.SetTrackingWorkflowInstances(workflowTypeId, workflowTrackingInfos);
        }

        /// <summary>
        /// Gets a collection of NewteraTrackingWorkflowInstance from Newtera Tracking store
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id to be matched</param>
        /// <returns>NewteraTrackingWorkflowInstanceCollection instance</returns>
        public NewteraTrackingWorkflowInstanceCollection GetTrackingWorkflowInstances(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraTrackingQuery trackingQuery = new NewteraTrackingQuery();

            return trackingQuery.GetTrackingWorkflowInstances(workflowInstanceId);
        }

        /// <summary>
        /// Gets a list of the tasks created by the workflows for the current user associated with
        /// a particular schema.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="startRow">The start index of task list to return.</param>
        /// <param name="count">The number of task to return</param>
        /// <param name="totalCount">The total count of task</param>
        /// <returns>A list of tasks</returns>
        public List<TaskInfo> GetTasks(string schemaId, int startRow, int count, out int totalCount)
        {
            List<TaskInfo> returnTasks = new List<TaskInfo>();

            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            ITaskService taskService = new NewteraTaskService();

            List<TaskInfo> allTasks = taskService.GetUserTasks(schemaId, principal.Identity.Name, new ServerSideUserManager());

            List<TaskInfo> visibleTasks = new List<TaskInfo>();
            foreach (TaskInfo taskInfo in allTasks)
            {
                if (taskInfo.IsVisible)
                {
                    visibleTasks.Add(taskInfo);
                }
            }

            totalCount = visibleTasks.Count;

            for (int i = startRow; i < startRow + count; i++)
            {
                if (i < visibleTasks.Count)
                {
                    returnTasks.Add(visibleTasks[i]);
                }
                else
                {
                    // no more tasks
                    break;
                }
            }

            return returnTasks;
        }


        /// <summary>
        /// Gets all of the tasks created by the workflows for the current user associated with
        /// a particular schema.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A list of tasks</returns>
        public List<TaskInfo> GetAllTasks(string schemaId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            ITaskService taskService = new NewteraTaskService();

            return taskService.GetUserTasks(schemaId, principal.Identity.Name, new ServerSideUserManager());
        }

        /// <summary>
        /// Gets the tasks finished by the current user associated with a particular schema.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A list of tasks</returns>
        public List<TaskInfo> GetFinishedTasks(string schemaId, int from, int size)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            ITaskService taskService = new NewteraTaskService();

            return taskService.GetUserFinishedTasks(schemaId, principal.Identity.Name, from, size, new ServerSideUserManager());
        }

        /// <summary>
        /// Gets count of all the tasks finished by the current user associated with a particular schema.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>The count of the finished tasks</returns>
        public int GetFinishedCount(string schemaId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            ITaskService taskService = new NewteraTaskService();

            return taskService.GetUserFinishedTaskCount(schemaId, principal.Identity.Name, new ServerSideUserManager());
        }

        /// <summary>
        /// Gets all of the tasks finished by the current user associated with a particular schema.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="wfInstanceId">A workflow instance id</param>
        /// <returns>A list of tasks</returns>
        public List<TaskInfo> GetWorkflowFinishedTasks(string schemaId, string wfInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            ITaskService taskService = new NewteraTaskService();

            return taskService.GetWorkflowFinishedTasks(schemaId, wfInstanceId, new ServerSideUserManager());
        }

        /// <summary>
        /// Delete all of the tasks finished by the current user associated with a particular schema.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A list of tasks</returns>
        public void ClearAllFinishedTasks(string schemaId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            ITaskService taskService = new NewteraTaskService();

            taskService.ClearUserFinishedTasks(schemaId, principal.Identity.Name, new ServerSideUserManager());
        }

        /// <summary>
        /// Gets a task by id
        /// </summary>
        /// <param name="taskId">task id</param>
        /// <returns>A TaskInfo object</returns>
        public TaskInfo GetTask(string taskId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            ITaskService taskService = new NewteraTaskService();

            return taskService.GetTask(taskId);
        }

        /// <summary>
        /// Gets a finished task by id
        /// </summary>
        /// <param name="taskId">task id</param>
        /// <returns>A TaskInfo object</returns>
        public TaskInfo GetFinishedTask(string taskId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                throw new InvalidOperationException("The user has not been authenticated");
            }

            ITaskService taskService = new NewteraTaskService();

            return taskService.GetFinishedTask(taskId, principal.Identity.Name);
        }

        /// <summary>
        /// Gets the workflow instance of the given workflow instance id
        /// </summary>
        /// <param name="wfInstanceId">The workflow instance id</param>
        /// <returns>A NewteraWorkflowInstance object, null if there isn't a binding exist.</returns>
        public NewteraWorkflowInstance GetWorkflowInstance(Guid wfInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            return NewteraWorkflowRuntime.Instance.FindWorkflowInstance(wfInstanceId);
        }

        /// <summary>
        /// Cancel a workflow instance that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        public void CancelWorkflowInstance(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraWorkflowRuntime.Instance.CancelWorkflow(new Guid(workflowInstanceId));
        }

        /// <summary>
        /// Cancel an activity of a workflow instance that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <param name="activityName">Name of the activity</param>
        public void CancelActivity(string workflowInstanceId, string activityName)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraWorkflowRuntime.Instance.CancelActivity(new Guid(workflowInstanceId), activityName);
        }

        /// <summary>
        /// Delete tracking data from the database of a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        public void DeleteTrackingWorkflowInstance(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraTrackingQuery trackingQuery = new NewteraTrackingQuery();

            trackingQuery.DeleteTrackingWorkflowInstance(workflowInstanceId);
        }

        /// <summary>
        /// Get a list of TaskInfo objects that are associated with a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        public List<TaskInfo> GetTaskInfos(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraTaskService taskService = new NewteraTaskService();

            return taskService.GetWorkflowInstanceTasks(workflowInstanceId);
        }

        /// <summary>
        /// Sets a collection of a workflow instance's task infos. Used by the project restore tool
        /// </summary>
        /// <param name="taskInfos">A collection of a workflow instance's task infos.</param>
        public void SetTaskInfos(TaskInfoCollection taskInfos)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            NewteraTaskService taskService = new NewteraTaskService();

            string escapedCustomActionsXml;
            foreach (TaskInfo taskInfo in taskInfos)
            {
                if (!string.IsNullOrEmpty(taskInfo.CustomActionsXml))
                {
                    // esacpe the xml characters since it will be inserted into a xquery
                    escapedCustomActionsXml = SecurityElement.Escape(taskInfo.CustomActionsXml);
                }
                else
                {
                    escapedCustomActionsXml = null;
                }

                taskService.CreateTask(taskInfo.WorkflowInstanceId, taskInfo.Subject, taskInfo.Description,
                    taskInfo.Instruction, taskInfo.CustomFormUrl, taskInfo.FormProperties, taskInfo.Users, taskInfo.Roles,
                    taskInfo.ActivityName, taskInfo.BindingSchemaId, taskInfo.BindingClassName,
                    escapedCustomActionsXml, taskInfo.IsVisible);
            }
        }

        /// <summary>
        /// Get information of the data instance that has been bound to a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The binding info, null if no binding data instance exists.</returns>
        public WorkflowInstanceBindingInfo GetBindingDataInstanceInfo(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.GetBindingInfoByWorkflowInstanceId(workflowInstanceId);
        }

        /// <summary>
        /// Gets the name of role that has permission to modify the project
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <returns>The name of role, null for non-protected mode.</returns>
        public string GetDBARole(string projectName, string projectVersion)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.GetDBARole(projectName, projectVersion);
        }

        /// <summary>
        /// Sets the name of role that has permission to modify the project
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="role">The name of role, null to set non-protected mode.</param>
        public void SetDBARole(string projectName, string projectVersion, string role)
        {
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            adapter.SetDBARole(projectName, projectVersion, role);
        }

        /// <summary>
        /// Get ids of the data instances that has been bound to workflow instances.
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="pageSize"> The number of ids returned each call</param>
        /// <param name="pageIndex"> The index of current page</param>
        /// <returns>A string array of data instance ids, null if it reaches the end of result.</returns>
        public string[] GetBindingDataInstanceIds(string schemaId, int pageSize, int pageIndex)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.GetBindingDataInstanceIds(schemaId, pageSize, pageIndex);
        }

        /// <summary>
        /// Replace the old id of a binding data instance with a new id. This method is used when
        /// restore a database from a backup file in which new ids are created for each data instance.
        /// </summary>
        /// <param name="oldInstanceId"> The old instance id</param>
        /// <param name="newInstanceId"> The new instance id</param>        
        public void ReplaceBindingDataInstanceId(string oldInstanceId, string newInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            adapter.ReplaceBindingDataInstanceId(oldInstanceId, newInstanceId);
        }

        /// <summary>
        /// Gets the task substitute model from database as xml string
        /// </summary>
        public string GetTaskSubstuteModel()
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            TaskSubstituteModel taskSubstituteModel = WorkflowModelCache.Instance.GetTaskSubstituteModel(_dataProvider);
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            taskSubstituteModel.Write(writer);

            return builder.ToString();
        }

        /// <summary>
        /// Update the task substitute model saved in database
        /// </summary>
        /// <param name="xml">The task substitute xml</param>
        public void UpdateTaskSubstituteModel(string xml)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelCache.Instance.UpdateTaskSubstituteModel(xml, _dataProvider);
        }

        /// <summary>
        /// Lock the task substitute model for update
        /// </summary>
        /// <exception cref="LockProjectException">Thrown when the task substitute model has been locked by another user.</exception>
        public void LockTaskSubstituteModel()
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelCache.Instance.LockTaskSubstituteModel();
        }

        /// <summary>
        /// Unlock the task substitute model
        /// </summary>
        /// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        /// <exception cref="LockProjectException">Thrown when the user doesn't have a right to unlock the project.</exception>
        public void UnlockTaskSubstituteModel(bool forceUnlock)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelCache.Instance.UnlockTaskSubstituteModel(forceUnlock);
        }

        /// <summary>
        /// Get the state information of a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>The binding info, null if no binding data instance exists.</returns>
        public WorkflowInstanceStateInfo GetWorkflowInstanceStateInfo(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.GetStateInfoByWorkflowInstanceId(workflowInstanceId);
        }

        /// <summary>
        /// Sets a workflow instance state info to the database. Used by the project restore tool
        /// </summary>
        /// <param name="connectionStr">connection string</param>
        /// <param name="stateInfo">A workflow instance state info.</param>
        public void SetWorkflowInstanceStateInfo(WorkflowInstanceStateInfo stateInfo)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            adapter.WriteInstanceState(stateInfo.WorkflowInstanceId,
                stateInfo.State, stateInfo.Unlocked);
        }

        /// <summary>
        /// Get a collection of DBEventSubscription objects of a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>A collection of DBEventSubscription objects.</returns>
        public DBEventSubscriptionCollection GetDBEventSubscriptions(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.GetEventSubscriptionsByWorkflowInstanceId(workflowInstanceId);
        }

        /// <summary>
        /// Sets a collection of a workflow instance's subscriptions to database events. Used by the project restore tool
        /// </summary>
        /// <param name="subscriptions">A collection of subscriptions to database events.</param>
        public void SetDBEventSubscriptions(DBEventSubscriptionCollection subscriptions)
        {
             if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            foreach (DBEventSubscription subscription in subscriptions)
            {
                Guid subscriptionId = new Guid(subscription.SubscriptionId);
                adapter.AddEventSubscription(subscriptionId, subscription);
            }
        }

        /// <summary>
        /// Get a collection of WorkflowEventSubscription objects of a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <returns>A collection of WorkflowEventSubscription objects.</returns>
        public WorkflowEventSubscriptionCollection GetWorkflowEventSubscriptions(string workflowInstanceId)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            return adapter.GetWorkflowEventSubscriptionsByWFInstanceId(workflowInstanceId);
        }

        /// <summary>
        /// Sets a collection of a workflow instance's subscriptions to workflow events. Used by the project restore tool
        /// </summary>
        /// <param name="subscriptions">A collection of subscriptions to workflow events.</param>
        public void SetWorkflowEventSubscriptions(WorkflowEventSubscriptionCollection subscriptions)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            foreach (WorkflowEventSubscription subscription in subscriptions)
            {
                Guid subscriptionId = new Guid(subscription.SubscriptionId);
                adapter.AddWorkflowEventSubscription(subscriptionId, subscription);
            }
        }

        /// <summary>
        /// Sets a collection of a workflow instance's binding to database instances. Used by the project restore tool
        /// </summary>
        /// <param name="workflowTypeId">The unique workflow internal type id</param>
        /// <param name="bindingInfos">A collection of a workflow instance's binding to database instances.</param>
        public void SetBindingDataInstanceInfos(string workflowTypeId, WorkflowInstanceBindingInfoCollection bindingInfos)
        {
            if (this.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();

            foreach (WorkflowInstanceBindingInfo bindingInfo in bindingInfos)
            {
                adapter.SetWorkflowInstanceBinding(bindingInfo.DataInstanceId,
                    bindingInfo.DataClassName, bindingInfo.SchemaId,
                    bindingInfo.WorkflowInstanceId, workflowTypeId);
            }
        }

        #endregion Workflow
    }
}
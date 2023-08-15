/*
* @(#) WorkflowModelCache.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using System.IO;
	using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
	using System.Resources;
	using System.Threading;

	using Newtera.Server.DB;
    using Newtera.Server.DB.WorkflowModel;
	using Newtera.Common.MetaData.Principal;
    using Newtera.WFModel;
    using Newtera.Server.Engine.Workflow;
    using Newtera.Server.Util;

    /// <summary>
    /// This is the single cache of workflow model data for the server.
    /// </summary>
    /// <version> 	1.0.0	15 Dec 2006 </version>
    public class WorkflowModelCache
	{
        private const string TaskSubstituteModelName = "taskSubstituteModelName";

		// Static cache object, all invokers will use this cache object.
		private static WorkflowModelCache theCache;
		
		private IKeyValueStore _table;
		private IKeyValueStore _locks;
        private IKeyValueStore _dataTable;
        private IKeyValueStore _wizardStepTable;
		private ResourceManager _resources;
        private ProjectInfoCollection _projectInfos = null;
        private ProjectInfoCollection _latestProjectInfos = null;
        private TaskSubstituteModel _taskSubstituteModel = null;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private WorkflowModelCache()
		{
			_table = KeyValueStoreFactory.TheInstance.Create("WorkflowModelCache.Table");
			_locks = KeyValueStoreFactory.TheInstance.Create("WorkflowModelCache.Locks");
            _dataTable = KeyValueStoreFactory.TheInstance.Create("WorkflowModelCache.DataTable");
            _wizardStepTable = KeyValueStoreFactory.TheInstance.Create("WorkflowModelCache.WizardStepTable");
			_resources = new ResourceManager(this.GetType());
		}

		/// <summary>
		/// Gets the WorkflowModelCache instance.
		/// </summary>
		/// <returns> The WorkflowModelCache instance.</returns>
		static public WorkflowModelCache Instance
		{
			get
			{
				return theCache;
			}
		}

        /// <summary>
        /// Gets project infos from the database
        /// </summary>
        /// <returns>An array of existing project infos</returns>
        public ProjectInfo[] GetProjectInfos(IDataProvider dataProvider)
        {
            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter(dataProvider);

            // this method never return null
            ProjectInfoCollection projectInfoCollection = workflowModelAdapter.GetProjectInfos();

            ProjectInfo[] projectInfos = new ProjectInfo[projectInfoCollection.Count];
            int idx = 0;
            foreach (ProjectInfo projectInfo in projectInfoCollection)
            {
                projectInfos[idx] = new ProjectInfo();
                projectInfos[idx].ID = projectInfo.ID;
                projectInfos[idx].Name = projectInfo.Name;
                projectInfos[idx].Version = projectInfo.Version;
                projectInfos[idx].Description = projectInfo.Description;
                projectInfos[idx].ModifiedTime = projectInfo.ModifiedTime;
                idx++;
            }

            return projectInfos;
        }

        /// <summary>
        /// Gets the project infos which are the latest version
        /// </summary>
        /// <param name="allProjectInfos">All project infos</param>
        /// <returns>A collection of project infos</returns>
        public ProjectInfoCollection GetProjectsOfLatestVersion(IDataProvider dataProvider)
        {
            lock (this)
            {
                if (_latestProjectInfos == null)
                {
                    _latestProjectInfos = new ProjectInfoCollection();

                    if (_projectInfos == null)
                    {
                        WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);

                        _projectInfos = adapter.GetProjectInfos();
                    }

                    bool exist;
                    int index;
                    foreach (ProjectInfo projectInfo in _projectInfos)
                    {
                        index = 0;
                        exist = false;
                        foreach (ProjectInfo latestProjectInfo in _latestProjectInfos)
                        {
                            if (latestProjectInfo.Name.ToUpper() == projectInfo.Name.ToUpper())
                            {
                                exist = true;
                                break;
                            }

                            index++;
                        }

                        if (!exist)
                        {
                            _latestProjectInfos.Add(projectInfo);
                        }
                        else if (string.Compare(projectInfo.Version, _latestProjectInfos[index].Version) > 0)
                        {
                            // remove the lower version, and add one with higher version
                            _latestProjectInfos.RemoveAt(index);
                            _latestProjectInfos.Add(projectInfo);
                        }
                    }
                }
            }

            return _latestProjectInfos;
        }

		/// <summary>
		/// Get a ProjectModel for the given name and version
		/// </summary>
		/// <param name="projectName">project name.</param>
        /// <param name="projectVersion">The project version</param>
		/// <returns>A ProjectModel.</returns>
        /// <remarks>GetProjectModel method is synchronized.</remarks>
		public ProjectModel GetProjectModel(string projectName, string projectVersion, IDataProvider dataProvider)
		{
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            return GetProjectModel(projectInfo, dataProvider);
		}

        /// <summary>
        /// Get a ProjectModel for the given project info
        /// </summary>
        /// <param name="projectInfo">project info.</param>
        /// <returns>A ProjectModel.</returns>
        /// <remarks>GetProjectModel method is synchronized.</remarks>
        public ProjectModel GetProjectModel(ProjectInfo projectInfo, IDataProvider dataProvider)
        {
            lock (this)
            {
                ProjectModel projectModel = _table.Get<ProjectModel>(projectInfo.NameAndVersion);

                if (projectModel == null)
                {
                    // reload the project model from database
                    if (!IsProjectExisted(projectInfo.NameAndVersion, dataProvider))
                    {
                        throw new Exception("The workflow project with name and version " + projectInfo.NameAndVersion + " does not exists.");
                    }

                    ProjectInfo projectInfoFromDb = GetProjectInfo(projectInfo.NameAndVersion, dataProvider);
                    projectModel = new ProjectModel(projectInfo.Name);
                    WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                    adapter.Fill(projectInfoFromDb.ID, projectModel);

                    projectModel.ModifiedTime = projectInfoFromDb.ModifiedTime; // set the timestamp to the project model
                    projectModel.Version = projectInfoFromDb.Version;

                    _table.Add(projectInfoFromDb.NameAndVersion, projectModel);
                }

                return projectModel;
            }
        }

        /// <summary>
		/// Updates the project model in the database and invalidates
		/// that chached project model so that it will be reloaded upon the next
		/// request.
		/// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion"> The project version</param>
        /// <param name="timestamp">The timestamp of the project model</param>
        /// <param name="projectXml">The xml string representing the project model</param>
        /// <param name="policyXml">The xml string representing the project access policy</param>
		/// <param name="dataProvider">The data provider</param>
        /// <returns>The latest modified time of the project model</returns>
        public DateTime UpdateProjectModel(string projectName, string projectVersion, DateTime timestamp, string projectXml, string policyXml, IDataProvider dataProvider)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            return UpdateProjectModel(projectInfo, timestamp, projectXml, policyXml, dataProvider);
        }

		/// <summary>
		/// Updates the project model in the database and invalidates
		/// that chached project model so that it will be reloaded upon the next
		/// request.
		/// </summary>
        /// <param name="projectInfo">The project info</param>
        /// <param name="timestamp">The timestamp of the project model</param>
        /// <param name="projectXml">The xml string representing the project model</param>
        /// <param name="policyXml">The xml string representing the project access policy</param>
		/// <param name="dataProvider">The data provider</param>
        /// <returns>The latest modified time of the project model</returns>
        public DateTime UpdateProjectModel(ProjectInfo projectInfo, DateTime timestamp, string projectXml, string policyXml, IDataProvider dataProvider)
		{
            ProjectModel newProjectModel = new ProjectModel(projectInfo.Name);
            newProjectModel.ModifiedTime = timestamp;
            newProjectModel.Version = projectInfo.Version;
            ProjectModel oldProjectModel;
            DateTime modifiedTime = DateTime.Now;

            if (IsProjectExisted(projectInfo.NameAndVersion, dataProvider))
            {
                if (IsUpdateAllowed(projectInfo.NameAndVersion))
                {
                    // get the old project model
                    oldProjectModel = GetProjectModel(projectInfo, dataProvider);

                    // Compare the timestamps of the newer project model and older
                    // project model, make sure that the two timestamps are the same.
                    if (oldProjectModel.ModifiedTime > newProjectModel.ModifiedTime)
                    {
                        // meta-data model stored in database is more updated then the new one.
                        // reject the updates
                        throw new UpdateMetaDataException(_resources.GetString("WFProjectOutdated"));
                    }
                }
                else
                {
                    throw new Exception(_resources.GetString("UpdateNotAllowed"));
                }
            }
            else
            {
                // the given project dose not exists
                oldProjectModel = null;
            }

            // convert project model from string to its model
            StringReader reader = new StringReader(projectXml);
            newProjectModel.Read(reader);
            reader = new StringReader(policyXml);
            newProjectModel.Policy.Read(reader);

            // compare the old and new project models to get the result of
            // comparison
            ProjectModelComparator comparator = new ProjectModelComparator(dataProvider);
            ProjectModelCompareResult result = comparator.Compare(newProjectModel,
                oldProjectModel);

            // lock the cache during updating the project model
            lock (this)
            {
                ProjectModelUpdateExecutor executor = new ProjectModelUpdateExecutor(result, dataProvider);

                InvalidateCache(projectInfo.NameAndVersion); // make the cached schema invalid

                // make sure to execute the update as the last step
                executor.Execute(); // Perform the updates to project model in database

                modifiedTime = UpdateProjectTimestamp(projectInfo.Name, projectInfo.Version, dataProvider); // set the modified time
            }

            // clear the assemblies created for custom action code for tasks so they will be recreated
            ActionCodeRunner.Instance.ClearAssemblies();

            return modifiedTime;
		}

        /// <summary>
		/// Delete a project model in the database and clear the cache
		/// </summary>
		/// <param name="projectName">The name of a project model is to be deleted</param>
        /// <param name="projectVersion">The project version</param>
		/// <param name="dataProvider">The data provider</param>
        public void DeleteProjectModel(string projectName, string projectVersion, IDataProvider dataProvider)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            DeleteProjectModel(projectInfo, dataProvider);
        }

		/// <summary>
		/// Delete a project model in the database and clear the cache
		/// </summary>
		/// <param name="projectInfo">The project info of a project model is to be deleted</param>
		/// <param name="dataProvider">The data provider</param>
		public void DeleteProjectModel(ProjectInfo projectInfo, IDataProvider dataProvider)
		{
			ProjectModel oldProjectModel;

            if (IsProjectExisted(projectInfo.NameAndVersion, dataProvider))
			{
				// get the old project model
                oldProjectModel = this.GetProjectModel(projectInfo, dataProvider);
			}
			else
			{
				return; // the project model does not exist.
			}

			ProjectModelComparator comparator = new ProjectModelComparator(dataProvider);
			ProjectModelCompareResult result = comparator.Compare(null, oldProjectModel);

			// lock the cache during deleting of the project model
			lock(this)
			{
				ProjectModelUpdateExecutor executor = new ProjectModelUpdateExecutor(result, dataProvider);

				// clear the cache
                _table.Remove(projectInfo.NameAndVersion);
                _locks.Remove(projectInfo.NameAndVersion);
                _wizardStepTable.Clear();
                _projectInfos = null;
                _latestProjectInfos = null;

				// make sure to execute the update as the last step
				executor.Execute(); // Perform the updates to project model in database
			}

            // clear the assemblies created for custom action code for tasks so they will be recreated
            ActionCodeRunner.Instance.ClearAssemblies();

            // clear the cache workflow instances in the runtime so that they have to be reloaded
            NewteraWorkflowRuntime.Instance.ClearCache();
		}

        /// <summary>
		/// Lock the ProjectModel for update so that other users are unable to update
		/// the same project model concurrently.
		/// </summary>
		/// <param name="projectName">The name of the project model.</param>
        /// <param name="projectVersion">The version of the project</param>
        /// <param name="modifiedTime">The timestamp of the client-side project model.</param>
		/// <param name="dataProvider">Data provider</param>
        public void LockProjectModel(string projectName, string projectVersion, DateTime modifiedTime, IDataProvider dataProvider)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            LockProjectModel(projectInfo, modifiedTime, dataProvider);
        }

		/// <summary>
		/// Lock the ProjectModel for update so that other users are unable to update
		/// the same project model concurrently.
		/// </summary>
		/// <param name="projectInfo">The project info of the project model.</param>
        /// <param name="modifiedTime">The timestamp of the client-side project model.</param>
		/// <param name="dataProvider">Data provider</param>
		public void LockProjectModel(ProjectInfo projectInfo, DateTime modifiedTime, IDataProvider dataProvider)
		{
			lock (this)
			{
                ProjectModel projectModel = GetProjectModel(projectInfo, dataProvider);

                // Make sure that project model has not been modified since the user loaded
                // project model into the client side
                if (modifiedTime < projectModel.ModifiedTime)
                {
                    throw new LockMetaDataException(_resources.GetString("LockFailed"));
                }

                string lockingUser = _locks.Get<string>(projectInfo.NameAndVersion);
				string requestUser = null;
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				if (principal != null)
				{
					requestUser = principal.Identity.Name;
				}
				
				if (requestUser == null)
				{
					throw new Exception(_resources.GetString("UnknownUser"));
				}
				else if	(lockingUser != null && lockingUser != requestUser)
				{
					string msg = String.Format(_resources.GetString("HasLocked"), lockingUser);
					throw new Exception(msg);
				}
				else if (lockingUser == null)
				{
					// set the lock
                    _locks.Add(projectInfo.NameAndVersion, requestUser);
				}
			}
		}

        /// <summary>
		/// Unlock the ProjectModel so that other users can obtain the lock for update.
		/// </summary>
		/// <param name="projectName">name of the project model to be unlocked.</param>
        /// <param name="projectVersion">The version of the project</param>
		/// <param name="dataProvider">Data provider</param>
		/// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        public void UnlockProjectModel(string projectName, string projectVersion, IDataProvider dataProvider,
            bool forceUnlock)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            UnlockProjectModel(projectInfo, dataProvider, forceUnlock);
        }

		/// <summary>
		/// Unlock the ProjectModel so that other users can obtain the lock for update.
		/// </summary>
		/// <param name="projectInfo">Project info.</param>
		/// <param name="dataProvider">Data provider</param>
		/// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
		public void UnlockProjectModel(ProjectInfo projectInfo, IDataProvider dataProvider,
			bool forceUnlock)
		{
			lock (this)
			{
                string lockingUser = _locks.Get<string>(projectInfo.NameAndVersion);
				string requestUser = null;
				string[] roles = null;
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				if (principal != null)
				{
					requestUser = principal.Identity.Name;
					roles = principal.Roles;
				}

				if (requestUser != null)
				{
					if (lockingUser != null)
					{
						// the project model is locked
						if (requestUser == lockingUser)
						{
                            _locks.Remove(projectInfo.NameAndVersion); // remove the lock
						}
						else if (forceUnlock)
						{
							// check if the requesting user is the super user
							bool isSuperUser = false;

							if (roles != null)
							{
								foreach (string role in roles)
								{
									if (role == Newtera.Common.Core.NewteraNameSpace.CM_SUPER_USER_ROLE)
									{
										isSuperUser = true;
										break;
									}
								}
							}

							if (isSuperUser)
							{
								// the lock can be removed by super-user
                                _locks.Remove(projectInfo.NameAndVersion);
							}
							else
							{
								// not super user, throw an exception
								throw new Exception(_resources.GetString("UnlockFailed"));
							}
						}
					}
				}
				else
				{
					throw new LockMetaDataException(_resources.GetString("UnknownUser"));
				}
			}
		}

        /// <summary>
        /// Gets the information indicating whether a project is the latest version.
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="dataProvider">The data provider</param>
        /// <returns>true if it is the latest version, false otherwise.</returns>
        public bool IsLatestVersion(string projectName, string projectVersion, IDataProvider dataProvider)
        {
            lock (this)
            {
                bool status = true;

                WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter(dataProvider);

                // this method never return null
                ProjectInfoCollection projectInfoCollection = workflowModelAdapter.GetProjectInfos();

                foreach (ProjectInfo projectInfo in projectInfoCollection)
                {
                    // check if the project has higher version
                    if (projectInfo.Name.ToUpper() == projectName.ToUpper() &&
                        string.Compare(projectInfo.Version, projectVersion) > 0)
                    {
                        status = false;
                        break;
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the unique type id of the specified workflow.
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataProvider">IDataProvider instance</param>
        public string GetWorkflowTypeId(string projectName, string projectVersion, string workflowName, IDataProvider dataProvider)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            return GetWorkflowTypeId(projectInfo, workflowName, dataProvider);
        }

        /// <summary>
        /// Gets the unique type id of the specified workflow.
        /// </summary>
        /// <param name="projectInfo">The project info</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataProvider">IDataProvider instance</param>
        public string GetWorkflowTypeId(ProjectInfo projectInfo, string workflowName, IDataProvider dataProvider)
        {
            ProjectModel projectModel = this.GetProjectModel(projectInfo, dataProvider);
            WorkflowModel workflowModel = (WorkflowModel)projectModel.Workflows[workflowName];
            string workflowType = null;

            if (workflowModel != null)
            {
                workflowType = workflowModel.ID;
            }
            else
            {
                throw new Exception("Unable to find the workflow model with name " + workflowName);
            }

            return workflowType;
        }

        /// <summary>
        /// Gets the specified data of the given workflow
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataType">One of WorkflowDataType enum</param>
        /// <param name="dataProvider">IDataProvider instance</param>
        public string GetWorkflowData(string projectName, string projectVersion, string workflowName,
            WorkflowDataType dataType, IDataProvider dataProvider)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            return GetWorkflowData(projectInfo, workflowName, dataType, dataProvider);
        }

        /// <summary>
        /// Gets the specified data of the given workflow
        /// </summary>
        /// <param name="projectInfo">The project info</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataType">One of WorkflowDataType enum</param>
        /// <param name="dataProvider">IDataProvider instance</param>
        public string GetWorkflowData(ProjectInfo projectInfo, string workflowName,
            WorkflowDataType dataType, IDataProvider dataProvider)
        {
            ProjectModel projectModel = this.GetProjectModel(projectInfo, dataProvider);
            WorkflowModel workflowModel = (WorkflowModel)projectModel.Workflows[workflowName];
            string dataString = null;

            if (workflowModel != null)
            {
                dataString = GetWorkflowData(workflowModel.ID, dataType, dataProvider);
            }
            else
            {
                throw new Exception("Unable to find the workflow model with name " + workflowName);
            }

            return dataString;
        }

        /// <summary>
        /// Gets the workflow data by the workflow id
        /// </summary>
        /// <param name="worflowId">The id of the workflow</param>
        /// <param name="dataType">One of WorkflowDataType enum</param>
        /// <param name="dataProvider">IDataProvider instance</param>
        public string GetWorkflowData(string workflowId, WorkflowDataType dataType, IDataProvider dataProvider)
        {
            string dataString = null;

            if (workflowId != null)
            {
                // first try to get the data from the cache
                WorkflowDataCacheEntry entry = _dataTable.Get<WorkflowDataCacheEntry>(workflowId);

                if (entry == null)
                {
                    entry = new WorkflowDataCacheEntry();
                    WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                    adapter.FillWorkflowData(workflowId, entry);

                    _dataTable.Add(workflowId, entry);
                }

                switch (dataType)
                {
                    case WorkflowDataType.Xoml:
                        dataString = entry.Xoml;
                        break;

                    case WorkflowDataType.Layout:
                        dataString = entry.Layout;
                        break;

                    case WorkflowDataType.Rules:
                        dataString = entry.Rules;
                        break;

                    case WorkflowDataType.Code:
                        dataString = entry.Code;
                        break;
                }
            }
            else
            {
                throw new Exception("Unable to find the workflow model with id as " + workflowId);
            }

            return dataString;
        }

        /// <summary>
        /// Sets the specified data of the given workflow to database
        /// </summary>
        /// <param name="projectName">The name of project</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataType">Data type string</param>
        /// <param name="dataString">The data string</param>
        /// <param name="dataProvider">IDataProvider instance</param>
        public void SetWorkflowData(string projectName, string projectVersion, string workflowName,
            WorkflowDataType dataType, string dataString, IDataProvider dataProvider)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            SetWorkflowData(projectInfo, workflowName, dataType, dataString, dataProvider);
        }

        /// <summary>
        /// Sets the specified data of the given workflow to database
        /// </summary>
        /// <param name="projectInfo">The info of project</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataType">Data type string</param>
        /// <param name="dataString">The data string</param>
        /// <param name="dataProvider">IDataProvider instance</param>
        public void SetWorkflowData(ProjectInfo projectInfo, string workflowName,
            WorkflowDataType dataType, string dataString, IDataProvider dataProvider)
        {
            if (IsUpdateAllowed(projectInfo.NameAndVersion))
            {
                ProjectModel projectModel = this.GetProjectModel(projectInfo, dataProvider);
                WorkflowModel workflowModel = (WorkflowModel) projectModel.Workflows[workflowName];

                if (workflowModel != null)
                {
                    WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                    adapter.WriteWorkflowData(workflowModel.ID, dataType, dataString);

                    // remove the entry from the cache
                    _dataTable.Remove(workflowModel.ID);
                }
                else
                {
                    throw new Exception("Unable to find the workflow model with name " + workflowName);
                }
            }
            else
            {
                throw new Exception(_resources.GetString("UpdateNotAllowed"));
            }
        }

        /// <summary>
        /// Get the TaskSubstituteModel
        /// </summary>
        /// <returns>A TaskSubstituteModel.</returns>
        /// <remarks>GetTaskSubstituteModel method is synchronized.</remarks>
        public TaskSubstituteModel GetTaskSubstituteModel(IDataProvider dataProvider)
        {
            lock (this)
            {
                if (_taskSubstituteModel == null)
                {
                    _taskSubstituteModel = new TaskSubstituteModel();
                    WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                    adapter.FillTaskSubstituteModel(_taskSubstituteModel);
                }

                return _taskSubstituteModel;
            }
        }

        /// <summary>
        /// Updates the task substitute model in the database and invalidates
        /// that chached task substitute model
        /// </summary>
        /// <param name="xml">The xml string representing the task substitute model</param>
        /// <param name="dataProvider">The data provider</param>
        public void UpdateTaskSubstituteModel(string xml, IDataProvider dataProvider)
        {
            // lock the cache during updating the task substitute model
            lock (this)
            {
                if (IsUpdateAllowed(TaskSubstituteModelName))
                {
                    WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                    adapter.WriteTaskSubstituteModel(xml);

                    _taskSubstituteModel = null;
                }
                else
                {
                    throw new Exception(_resources.GetString("UpdateTaskSubstituteModelNotAllowed"));
                }
            }
        }

        /// <summary>
        /// Lock the task substitute model for update so that other users are unable to update
        /// the model concurrently.
        /// </summary>
        public void LockTaskSubstituteModel()
        {
            lock (this)
            {
                string lockingUser = _locks.Get<string>(TaskSubstituteModelName);
                string requestUser = null;
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                if (principal != null)
                {
                    requestUser = principal.Identity.Name;
                }

                if (requestUser == null)
                {
                    throw new Exception(_resources.GetString("UnknownUser"));
                }
                else if (lockingUser != null && lockingUser != requestUser)
                {
                    string msg = String.Format(_resources.GetString("TaskSubstituteModelHasLocked"), lockingUser);
                    throw new Exception(msg);
                }
                else if (lockingUser == null)
                {
                    // set the lock
                    _locks.Add(TaskSubstituteModelName, requestUser);
                }
            }
        }

        /// <summary>
        /// Unlock the TaskSubstituteModel so that other users can obtain the lock for update.
        /// </summary>
        /// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        public void UnlockTaskSubstituteModel(bool forceUnlock)
        {
            lock (this)
            {
                string lockingUser = _locks.Get<string>(TaskSubstituteModelName);
                string requestUser = null;
                string[] roles = null;
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                if (principal != null)
                {
                    requestUser = principal.Identity.Name;
                    roles = principal.Roles;
                }

                if (requestUser != null)
                {
                    if (lockingUser != null)
                    {
                        // the project model is locked
                        if (requestUser == lockingUser)
                        {
                            _locks.Remove(TaskSubstituteModelName); // remove the lock
                        }
                        else if (forceUnlock)
                        {
                            // check if the requesting user is the super user
                            bool isSuperUser = false;

                            if (roles != null)
                            {
                                foreach (string role in roles)
                                {
                                    if (role == Newtera.Common.Core.NewteraNameSpace.CM_SUPER_USER_ROLE)
                                    {
                                        isSuperUser = true;
                                        break;
                                    }
                                }
                            }

                            if (isSuperUser)
                            {
                                // the lock can be removed by super-user
                                _locks.Remove(TaskSubstituteModelName);
                            }
                            else
                            {
                                // not super user, throw an exception
                                throw new Exception(_resources.GetString("TaskSubstituteModelUnlockFailed"));
                            }
                        }
                    }
                }
                else
                {
                    throw new LockMetaDataException(_resources.GetString("UnknownUser"));
                }
            }
        }

        /// <summary>
        /// Gets a collection of Wizard Steps represented by a Wizard Workflow
        /// </summary>
        /// <param name="projectName">The name of project that contains the workflow.</param>
        /// <param name="workflowName">The name of the wizard workflow</param>
        /// <returns>A collection of IWizardStep objects, or null if something went wrong</returns>
        public List<IWizardStep> GetWizardSteps(string projectName, string projectVersion, string workflowName)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.Version = projectVersion;

            List<IWizardStep> wizardSteps = _wizardStepTable.Get<List<IWizardStep>>(projectInfo.NameAndVersion);
            if (wizardSteps == null)
            {
            }

            return wizardSteps;
        }

		/// <summary>
		/// Make the chached project model of the given name invalid to force it
		/// to reload from database at next request.
		/// </summary>
        /// <param name="nameAndVersion">The name and version of project model to be invalidated</param>
		private void InvalidateCache(string nameAndVersion)
		{
            // remove from the cache
            _table.Remove(nameAndVersion);

            // clear the project infos in case they are updated
            this._projectInfos = null;
            _latestProjectInfos = null;

            _wizardStepTable.Clear();
		}

		/// <summary>
		/// Gets the info indicating if the given project is existed in database
		/// </summary>
        /// <param name="nameAndVersion">The id of the project consists of name and version</param>
		/// <param name="dataProvider">The data provider</param>
		/// <returns>true if it exists, false otherwise</returns>
		public bool IsProjectExisted(string nameAndVersion, IDataProvider dataProvider)
		{
            if (_projectInfos == null)
            {
                WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);

                _projectInfos = adapter.GetProjectInfos();
            }

            if (_projectInfos[nameAndVersion] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
		}

        /// <summary>
        /// Gets a task substitute user from the cache
        /// </summary>
        /// <param name="taskId">The task Id</param>
        /// <param name="user"> A assigned task user</param>
        /// <returns>A collection of substitute user names, null if not exist. </returns>
        public string GetTaskSubstitute(string taskId, string user, IDataProvider dataProvider)
        {
            string substitute = null;

            WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);

            List<ReassignedTaskInfo> taskInfos = adapter.GetReassignedTaskInfosByTaskId(taskId);

            foreach (ReassignedTaskInfo taskInfo in taskInfos)
            {
                if (taskInfo.OriginalOwner == user)
                {
                    substitute = taskInfo.CurrentOwner;
                    break;
                }
            }

            return substitute;
        }

        /// <summary>
        /// Gets a collection of task ids that have been reassigned to an user
        /// </summary>
        /// <param name="taskId">The task Id</param>
        /// <param name="user"> A assigned task user</param>
        /// <returns>A collection of substitute user names, null if not exist. </returns>
        public StringCollection GetReassignedTaskIds(string user, IDataProvider dataProvider)
        {
            StringCollection taskIds = new StringCollection();

            WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);

            List<ReassignedTaskInfo> taskInfos = adapter.GetReassignedTaskInfos();

            foreach (ReassignedTaskInfo taskInfo in taskInfos)
            {
                if (taskInfo.CurrentOwner == user)
                {
                    taskIds.Add(taskInfo.TaskId);
                }
            }

            return taskIds;
        }

        /// <summary>
        /// Add an entry in the hashtable for a task substitute
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="workflowInstanceId">Workflow instance id</param>
        /// <param name="originalOwner">The original assigned user</param>
        /// <param name="currentOwner">Task substitue user</param>
        public void AddTaskSubstitute(string taskId, string schemaId, string workflowInstanceId, string originalOwner, string currentOwner, IDataProvider dataProvider)
        {
            lock (this)
            {
                ReassignedTaskInfo taskInfo = new ReassignedTaskInfo();
                taskInfo.TaskId = taskId;
                taskInfo.WorkflowInstanceId = workflowInstanceId;
                taskInfo.OriginalOwner = originalOwner;
                taskInfo.CurrentOwner = currentOwner;
                WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                adapter.AddReassignedTaskInfo(taskInfo);
            }

            // Clear original's and transfered user's cached task list
            UserTaskCache.Instance.ClearUserTasks(originalOwner, schemaId);
            UserTaskCache.Instance.ClearUserTasks(currentOwner, schemaId);
        }

        /// <summary>
        /// Get information indicating whether there is an entry existing for the given user
        /// as substitute.
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="substituteUser">Task substitue user</param>
        public bool IsEntryForSubstituteExist(string taskId, string substituteUser, IDataProvider dataProvider)
        {
            bool status = false;

            lock (this)
            {
                WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);

                List<ReassignedTaskInfo> taskInfos = adapter.GetReassignedTaskInfosByTaskId(taskId);

                foreach (ReassignedTaskInfo taskInfo in taskInfos)
                {
                    if (taskInfo.CurrentOwner == substituteUser)
                    {
                        status = true;
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Relace the substitute user of an entry with a new user
        /// </summary>
        /// <param name="taskId">The task id</param>
        /// <param name="schemaId">The schema id</param>
        /// <param name="oldUser">The old substitue user</param>
        /// <param name="newUser">The new substitute user</param>
        public void ReplaceTaskSubstitute(string taskId, string schemaId, string oldUser, string newUser, IDataProvider dataProvider)
        {
            lock (this)
            {
                WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                adapter.ReplaceReassignedTaskCurrentOwner(taskId, oldUser, newUser);
            }

            // clear the orignal and new task owner's cached task list
            UserTaskCache.Instance.ClearUserTasks(oldUser, schemaId);
            UserTaskCache.Instance.ClearUserTasks(newUser, schemaId);
        }

        /// <summary>
        /// Remove all substitute users of a task
        /// </summary>
        /// <param name="taskId">The task id</param>
        public void RemoveTaskSubstitutes(string taskId, IDataProvider dataProvider)
        {
            lock (this)
            {
                WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);
                adapter.DeleteReassignedTaskInfoByTaskId(taskId);
            }
        }

        /// <summary>
        /// Gets the ProjectInfo instance of a given name
        /// </summary>
        /// <param name="namendVersion">The project name and version</param>
        /// <param name="dataProvider">The data access provider</param>
        /// <returns>ProjectInfo</returns>
        private ProjectInfo GetProjectInfo(string nameAndVersion, IDataProvider dataProvider)
        {
            if (_projectInfos == null)
            {
                WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);

                _projectInfos = adapter.GetProjectInfos();
            }

            return _projectInfos[nameAndVersion];
        }

		/// <summary>
		/// Gets the information indicating whether the current user is allowed to
		/// update the project model
		/// </summary>
        /// <param name="modelId">The unqiue id of the model to be updated</param>
		/// <returns>true if it is allowed, false, otherwise.</returns>
		private bool IsUpdateAllowed(string modelId)
		{
			bool status = true;

			lock (this)
			{
                string lockingUser = _locks.Get<string>(modelId);
				string requestUser = null;
				string[] roles = null;
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				if (principal != null)
				{
					requestUser = principal.Identity.Name;
					roles = principal.Roles;
				}

				if (requestUser != null)
				{
					bool isSuperUser = false;

					if (roles != null)
					{
						foreach (string role in roles)
						{
							if (role == Newtera.Common.Core.NewteraNameSpace.CM_SUPER_USER_ROLE)
							{
								isSuperUser = true;
								break;
							}
						}
					}
				
					// make sure that the project model model is locked by the same user
					if (lockingUser != null)
					{
						if	(requestUser != lockingUser && !isSuperUser)
						{
							status = false;
						}
					}
					else
					{
						// the model has not been locked, lock it automatically
						// set the lock
                        _locks.Add(modelId, requestUser);
					}
				}
				else
				{
					status = false;
				}
			}

			return status;
		}

        /// <summary>
        /// Set the project model's modified time to the current time
        /// </summary>
        /// <param name="name">The name of project.</param>
        /// <param name="version">The version of project</param>
        /// <param name="dataProvider">The data provider</param>
        /// <returns>The current time when the project model is modified.</returns>
        private DateTime UpdateProjectTimestamp(string name, string version, IDataProvider dataProvider)
        {
            DateTime modifiedTime = DateTime.Now;
            WorkflowModelAdapter adapter = new WorkflowModelAdapter(dataProvider);

            adapter.SetModifiedTime(name, version, modifiedTime);

            return modifiedTime;
        }

		static WorkflowModelCache()
		{
			// Initializing the cache.
			{
				theCache = new WorkflowModelCache();
			}
		}
	}
}
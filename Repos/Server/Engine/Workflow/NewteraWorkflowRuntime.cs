/*
* @(#) NewteraWorkflowRuntime.cs
*
* Copyright (c) 2006-2012 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
    using System.Xml;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Workflow.Runtime;
    using System.Workflow.Runtime.Hosting;
    using System.Workflow.Activities;
    using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Compiler;
    using System.Workflow.ComponentModel.Serialization;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    using Newtera.Common.Core;
    using Newtera.Common.Wrapper;
    using Newtera.WFModel;
    using Newtera.Server.DB;
    using Newtera.Server.Engine.Cache;
    using Newtera.Server.Engine.Workflow;
    using Newtera.WorkflowServices;
    using Newtera.Server.UsrMgr;
    //using Newtera.Activities;

	/// <summary>
    /// A wrapper around System.Workflow.WorkflowRuntime so that it guarentees a single
    /// WorkflowRuntime instance is used in the server, and provide custmized access to
    /// the workflow runtime functionalities.
	/// </summary>
	/// <version> 	1.0.0 19 Dec 2006 </version>
	public class NewteraWorkflowRuntime
	{		
		// Static factory object, all invokers will use this factory object.
		private static NewteraWorkflowRuntime theRuntime;

        private System.Workflow.Runtime.WorkflowRuntime _workflowRuntime;
        private Dictionary<Guid, NewteraWorkflowInstance> _workflowInstances;
        private ManualWorkflowSchedulerService _manualScheduler;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private NewteraWorkflowRuntime()
		{
            _workflowRuntime = new WorkflowRuntime();

            // register the workflow runtime event handlers to the running thread
            this.RegisterEventHandler();

            // add customized persistent service to the runtime
            _workflowRuntime.AddService(new NewteraPersistenceService(true));

            // add Newtera Event Service
            _workflowRuntime.AddService(DBEventServiceSingleton.Instance);

            // add customized tracking service
            _workflowRuntime.AddService(new NewteraTrackingService());

            // add customized task service
            _workflowRuntime.AddService(new NewteraTaskService());

            //add data exchange service
            ExternalDataExchangeService dataExchangeService = new ExternalDataExchangeService();
            _workflowRuntime.AddService(dataExchangeService);

            //add Wizard workflow Service
            WizardWorkflowService wizardWorkflowService = new WizardWorkflowService();
            dataExchangeService.AddService(wizardWorkflowService);

            ////add manual scheduler for host donated thread (for single threaded operation like in ASP.NET or Web Services)
            //_manualScheduler = new ManualWorkflowSchedulerService();
            //_workflowRuntime.AddService(_manualScheduler);

            // add customized data service
            IDataService dataService = new NewteraDataService();
            _workflowRuntime.AddService(dataService);
            InstanceWrapperFactory.Instance.BindingInstanceService = (IBindingInstanceService) dataService; 

            // add newtera workflow service
            _workflowRuntime.AddService(WorkflowEventServiceSingleton.Instance);

            // add UserManager
            _workflowRuntime.AddService(new ServerSideUserManager());

            // add custom types
            TypeProvider typeProvider = new TypeProvider(null);
            //typeProvider.AddAssemblyReference(typeof(NewteraStateMachineWorkflowActivity).Assembly.Location);
            typeProvider.AddAssemblyReference("Newtera.Activities.dll");

            _workflowRuntime.AddService(typeProvider);

            // get the current workflow instances from the database
            _workflowInstances = GetWorkflowInstances();

            _workflowRuntime.StartRuntime();
		}

        ~NewteraWorkflowRuntime()
        {
            _workflowRuntime.StopRuntime();
        }

		/// <summary>
		/// Gets the NewteraWorkflowRuntime instance.
		/// </summary>
		/// <returns> The NewteraWorkflowRuntime instance.</returns>
		static public NewteraWorkflowRuntime Instance
		{
			get
			{
				return theRuntime;
			}
		}

        /// <summary>
        /// Register event handlers
        /// </summary>
        public void RegisterEventHandler()
        {
            // Create EventHandlers for the WorkflowRuntime
            _workflowRuntime.WorkflowCompleted +=new EventHandler<WorkflowCompletedEventArgs>(this.WorkflowCompleted);
            _workflowRuntime.WorkflowTerminated += new EventHandler<WorkflowTerminatedEventArgs>(this.WorkflowTerminated);
            _workflowRuntime.WorkflowAborted += new EventHandler<WorkflowEventArgs>(this.WorkflowAborted);
            _workflowRuntime.WorkflowIdled += new EventHandler<WorkflowEventArgs>(this.WorkflowIdled); 
            //_workflowRuntime.WorkflowLoaded +=new EventHandler<WorkflowEventArgs>(this.WorkflowLoaded);
            //_workflowRuntime.WorkflowCreated +=new EventHandler<WorkflowEventArgs>(this.WorkflowCreated);
            _workflowRuntime.WorkflowPersisted +=new EventHandler<WorkflowEventArgs>(this.WorkflowPersisted);
            //_workflowRuntime.WorkflowStarted +=new EventHandler<WorkflowEventArgs>(this.WorkflowStarted);
            //_workflowRuntime.WorkflowUnloaded +=new EventHandler<WorkflowEventArgs>(this.WorkflowUnloaded);
            //_workflowRuntime.WorkflowSuspended += new EventHandler<WorkflowSuspendedEventArgs>(this.WorkflowSuspended);
        }

        /// <summary>
        /// Clear the cache to start over
        /// </summary>
        public void ClearCache()
        {
            _workflowInstances.Clear();
        }

        /// <summary>
        /// Find the workflow instance by id.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        /// <returns>A WorkflowInstance instance.</returns>
        public NewteraWorkflowInstance FindWorkflowInstance(Guid workflowInstanceId)
        {
            NewteraWorkflowInstance result = null;
            if (_workflowInstances.ContainsKey(workflowInstanceId))
            {
                result = _workflowInstances[workflowInstanceId];
            }
            else
            {
                // try to get it from the database to support multiple servers environment
                WorkflowModelAdapter adapter = new WorkflowModelAdapter();
                WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByWorkflowInstanceId(workflowInstanceId.ToString());
                if (bindingInfo != null)
                {
                    result = new NewteraWorkflowInstance(workflowInstanceId,
                        bindingInfo.ProjectName,
                        bindingInfo.ProjectVersion,
                        bindingInfo.WorkflowName,
                        bindingInfo.WorkflowTypeId);
                    result.SchemaId = bindingInfo.SchemaId;
                    result.ClassName = bindingInfo.DataClassName;
                    result.ObjId = bindingInfo.DataInstanceId;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a NewteraWorkflowInstance instance, given the project name and workflow name
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataProvider">The data provider</param>
        /// <returns>A NewteraWorkflowInstance instance.</returns>
        /// <remarks>The project name and workflow name can uniquely identify a workflow in
        /// database.</remarks>
        public NewteraWorkflowInstance CreateWorkflowInstance(string projectName, string projectVersion, string workflowName, IDataProvider dataProvider)
        {
            string workflowTypeId = WorkflowModelCache.Instance.GetWorkflowTypeId(projectName, projectVersion, workflowName, dataProvider);

            return CreateWorkflowInstanceByTypeId(projectName, projectVersion, workflowName, workflowTypeId, dataProvider);
        }

        /// <summary>
        /// Remove a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public void RemoveWorkflowInstance(Guid workflowInstanceId)
        {
            lock (this._workflowInstances)
            {
                if (_workflowInstances.ContainsKey(workflowInstanceId))
                {
                    _workflowInstances.Remove(workflowInstanceId);
                }
            }

            // remove the subscription from the database
            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
            workflowModelAdapter.DeleteWorkflowInstanceBindings(workflowInstanceId.ToString());
        }

        /// <summary>
        /// Start an instance, given the project info and event info
        /// </summary>
        /// <param name="projectInfo">The project info object</param>
        /// <param name="args">The event that causes the workflow to start</param>
        /// <returns>A Workflow Instance id.</returns>
        public Guid StartWorkflowInstance(ProjectInfo projectInfo, string workflowName, NewteraEventArgs args)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            NewteraWorkflowInstance wfInstance = CreateWorkflowInstance(projectInfo.Name, projectInfo.Version, workflowName, dataProvider);

            // establish a binding between the data instance that generates start event and workflow instance started.
            wfInstance.SchemaId = args.SchemaId;
            wfInstance.ClassName = args.ClassName;
            wfInstance.ObjId = args.ObjId;
            // save the updates to database
            wfInstance.Save();

            // trace it
            if (TraceLog.Instance.Enabled)
            {
                string[] messages = { wfInstance.WorkflowName + " workflow is started by event.",
                                        "Event Name: " + args.EventName,
                                        "Binding Data Instance: " + args.ObjId,
                                        "Project Name: " + wfInstance.ProjectName,
                                        "Project Version: " + wfInstance.ProjectVersion};
                TraceLog.Instance.WriteLines(messages);
            }

            // start the worklfow instance
            wfInstance.WorkflowInstance.Start();

            ErrorLog.Instance.WriteLine("An workflow instance of " + workflowName + " is started " + wfInstance.WorkflowInstanceId.ToString());

            return wfInstance.WorkflowInstanceId;
        }

        /// <summary>
        /// Start an instance, given the project name and workflow name
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="inputParameters">The input parameters to the workflow</param>
        /// <returns>A Workflow Instance id.</returns>
        /// <remarks>The project name and workflow name can uniquely identify a workflow in
        /// database.</remarks>
        public Guid StartWorkflowInstance(string projectName, string projectVersion, string workflowName, IList inputParameters)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            return StartWorkflowInstance(projectName, projectVersion, workflowName, inputParameters, dataProvider);
        }

        /// <summary>
        /// Start an instance, given the project name and workflow name
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="dataProvider">The Data provider</param>
        /// <returns>A Workflow Instance id.</returns>
        /// <remarks>The project name and workflow name can uniquely identify a workflow in
        /// database.</remarks>
        public Guid StartWorkflowInstance(string projectName, string projectVersion, string workflowName, IList inputParameters, IDataProvider dataProvider)
        {
            try
            {
                IWorkflowService workflowService = _workflowRuntime.GetService<IWorkflowService>();
                NewteraWorkflowInstance wfInstance = CreateWorkflowInstance(projectName, projectVersion, workflowName, dataProvider);
                
                // set the parameter values to the cache in workflow service
                if (inputParameters != null)
                {
                    string wfInstanceId = wfInstance.WorkflowInstanceId.ToString();
                    foreach (InputParameter passedParameter in inputParameters)
                    {
                        workflowService.SetInputParameterValue(wfInstanceId, passedParameter.Name, passedParameter.Value);
                    }
                }

                // start the workflow
                wfInstance.WorkflowInstance.Start();

                return wfInstance.WorkflowInstanceId;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Runs a workflow based on the instance Id using manual schedular
        /// </summary>
        /// <param name="instanceId">A valid instance Id</param>
        /// <returns>A boolean indicating result</returns>
        public bool RunWorkflowInstance(Guid instanceId)
        {
            return _manualScheduler.RunWorkflow(instanceId);
        }

        /// <summary>
        /// Gets a WorkflowInstance of the given instance id
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <returns>A WorkflowInstance</returns>
        public WorkflowInstance GetWorkflow(Guid instanceId)
        {
            return this._workflowRuntime.GetWorkflow(instanceId);
        }

        /// <summary>
        /// Cancel a workflow instance that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        public void CancelWorkflow(Guid workflowInstanceId)
        {
            try
            {
                WorkflowInstance workflow = this.GetWorkflow(workflowInstanceId);

                // what activity is blocking the workflow
                ReadOnlyCollection<WorkflowQueueInfo> wqi = workflow.GetWorkflowQueueData();
                foreach (WorkflowQueueInfo q in wqi)
                {
                    if (q != null)
                    {
                        //ErrorLog.Instance.WriteLine("CancelWorkflow find the queue info with name " + q.QueueName  + " and deliver the CancelWorkflowException to " + workflowInstanceId.ToString());

                        // it will either be handled by exception handler that was modeled in workflow
                        // or the runtime will unwind running compensation handlers and exit the workflow
                        workflow.EnqueueItem(q.QueueName, new CancelWorkflowException(workflowInstanceId.ToString()), null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                // do not throw the exception
                //throw ex;

                try
                {
                    Cleanup(workflowInstanceId); // something wrong in cancelling the workflow, just perform the clean up for the workflow
                }
                catch (Exception ex1)
                {
                    ErrorLog.Instance.WriteLine(ex1.Message + "\n" + ex1.StackTrace);
                }
            }
        }

        /// <summary>
        /// Cancel execution of an workflow activity that is awaiting an event.
        /// </summary>
        /// <param name="workflowInstanceId"> The unique workflow instance id</param>
        /// <param name="activityName">The name of the activity</param>
        public void CancelActivity(Guid workflowInstanceId, string activityName)
        {
            try
            {
                WorkflowInstance workflow = this.GetWorkflow(workflowInstanceId);

                // what activity is blocking the workflow
                ReadOnlyCollection<WorkflowQueueInfo> wqi = workflow.GetWorkflowQueueData();
                bool found = false;
                foreach (WorkflowQueueInfo q in wqi)
                {
                    if (q != null && q.SubscribedActivityNames.Contains(activityName))
                    {
                        found = true;

                        // Deliver the cancel activity event to the workflow
                        workflow.EnqueueItem(q.QueueName, new CancelActivityEventArgs(workflowInstanceId.ToString(), activityName), null, null);
                    }
                }

                if (!found)
                {
                    // unable to cancel an activity, terminate the workflow
                    workflow.Terminate("Unable to cancel the activity " + activityName);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("CancelActivity got an exception:" + ex.Message + "\n" + ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ErrorLog.Instance.WriteLine("Inner Exception is \n" + ex.InnerException.Message + "\n" + ex.InnerException.StackTrace);
                }

                // ignore the error
            }
        }

        /// <summary>
        /// Raise the event to start workflows
        /// </summary>
        /// <param name="eventDef">The event</param>
        /// <returns>true if there is a workflow started by the event, false otherwise.</returns>
        internal bool StartWorkflowEventHandler(object sender, NewteraEventArgs args)
        {
            bool isFound = false;

            ProjectInfoCollection projectInfos = null;

            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();

            // only the workflows in the project of the latest version can be started by
            // an event, therefore, get a collection of the projects of the latest version
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            projectInfos = WorkflowModelCache.Instance.GetProjectsOfLatestVersion(dataProvider);

            foreach (ProjectInfo projectInfo in projectInfos)
            {
                if (!isFound)
                {
                    ProjectModel projectModel = WorkflowModelCache.Instance.GetProjectModel(projectInfo.Name, projectInfo.Version, dataProvider);
                    foreach (WorkflowModel workflowModel in projectModel.Workflows)
                    {
                        if (workflowModel.StartEvent.SchemaID != null &&
                            workflowModel.StartEvent.SchemaID == args.SchemaId &&
                            workflowModel.StartEvent.ClassName != null &&
                            workflowModel.StartEvent.ClassName == args.ClassName &&
                            workflowModel.StartEvent.EventName != null &&
                            workflowModel.StartEvent.EventName == args.EventName)
                        {
                            WorkflowInstanceBindingInfo bindingInfo = workflowModelAdapter.GetBindingInfoByObjId(args.ObjId);
                            // make sure that each data instance can only be bound to one workflow instance
                            if (bindingInfo == null)
                            {
                                StartWorkflowInstance(projectInfo, workflowModel.Name, args);
                            }
                            else
                            {
                                ErrorLog.Instance.WriteLine("An instance with obj_id " + args.ObjId + " in class " + args.ClassName + " has been bound to a workflow instance.");
                            }

                            // since a data instance can be only bound to one workflow instance,
                            // other workflows in this project and other projects using the same event
                            // as the start event will not be started
                            isFound = true;
                            break;
                        }
                    }
                }
            }

            return isFound;
        }

        public WorkflowRuntime GetWorkflowRunTime()
        {
            return this._workflowRuntime;
        }

        /// <summary>
        /// Creates a CreateWorkflowInstance instance given workflow type id.
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName">The workflow name</param>
        /// <param name="workflowTypeId">The unique workflow id.</param>
        /// <param name="dataProvider">A DataProvider to access database to get workflow data</param>
        /// <returns>A NewteraWorkflowInstance instance.</returns>
        private NewteraWorkflowInstance CreateWorkflowInstanceByTypeId(string projectName, string projectVersion, string workflowName, string workflowTypeId, IDataProvider dataProvider)
        {
            string xoml = WorkflowModelCache.Instance.GetWorkflowData(workflowTypeId,
                WorkflowDataType.Xoml, dataProvider);
            string rules = WorkflowModelCache.Instance.GetWorkflowData(workflowTypeId,
                WorkflowDataType.Rules, dataProvider);

            XmlTextReader xomlReader = new XmlTextReader(new StringReader(xoml));
            WorkflowInstance workflowInstance;

            XmlTextReader rulesReader = null;
            if (rules != null && rules.Length > 0)
            {
                rulesReader = new XmlTextReader(new StringReader(rules));
            }

            try
            {
                if (rulesReader != null)
                {
                    workflowInstance = _workflowRuntime.CreateWorkflow(xomlReader, rulesReader, null);
                }
                else
                {
                    workflowInstance = _workflowRuntime.CreateWorkflow(xomlReader);
                }

                return AddWorkflowInstance(projectName, projectVersion, workflowName, workflowTypeId, workflowInstance);
            }
            catch (WorkflowValidationFailedException exp)
            {
                StringBuilder errors = new StringBuilder();

                foreach (ValidationError error in exp.Errors)
                {

                    errors.AppendLine(error.ToString());

                }

                string msg = errors.ToString();

                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Add a new workflow instance to the dictionary
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="workflowName"></param>
        /// <param name="workflowTypeId"></param>
        /// <param name="workflowInstance"></param>
        /// <returns>A NewteraWorkflowInstance that wraps the workflow instance</returns>
        private NewteraWorkflowInstance AddWorkflowInstance(string projectName, string projectVersion,
            string workflowName, string workflowTypeId, WorkflowInstance workflowInstance)
        {
            NewteraWorkflowInstance wrapper = this.FindWorkflowInstance(workflowInstance.InstanceId);
            if (wrapper == null)
            {
                lock (_workflowInstances)
                {
                    wrapper = new NewteraWorkflowInstance(workflowInstance.InstanceId, projectName, projectVersion, workflowName, workflowTypeId);
                    wrapper.WorkflowInstance = workflowInstance;

                    _workflowInstances.Add(wrapper.WorkflowInstanceId, wrapper);

                    // write the record to the database for recovery
                    WorkflowModelAdapter adapter = new WorkflowModelAdapter();
                    adapter.SetWorkflowInstanceBinding(wrapper.ObjId, wrapper.ClassName,
                        wrapper.SchemaId, workflowInstance.InstanceId.ToString(),
                        wrapper.WorkflowTypeId);
                }
            }

            return wrapper;
        }

        /// <summary>
        /// Cleanup after a workflow instance is completed, terminated, or aborted
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        private void Cleanup(Guid workflowInstanceId)
        {
            // get services from runtime
            DBEventService dbEventService = _workflowRuntime.GetService<DBEventService>();
            NewteraWorkflowService workflowService = _workflowRuntime.GetService<NewteraWorkflowService>();

            try
            {
                // raise an event to tell its invoking workflow that life of this workflow instance is over
                NewteraEventArgs eventArgs = new NewteraEventArgs();
                eventArgs.WorkflowInstanceId = workflowInstanceId.ToString();
                WorkflowEventServiceSingleton.Instance.RaiseWorkflowEvent(workflowInstanceId,
                    NewteraWorkflowRuntime.Instance.GetWorkflowRunTime(),
                    eventArgs);
            }
            catch (Exception ex)
            {
                // in case there are error in informing parent workflow, continue the cleanup process
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            // remove the event subscriptions
            dbEventService.RemoveSubscriptions(workflowInstanceId);
            workflowService.RemoveSubscriptions(workflowInstanceId);

            // clear the cached data instance
            InstanceWrapperFactory.Instance.BindingInstanceService.ClearBindingInstance(workflowInstanceId);

            // cleanup workflow instance persisted in the database
            WorkflowModelAdapter workflowModelAdapter = new WorkflowModelAdapter();
            workflowModelAdapter.DeleteInstanceState(workflowInstanceId.ToString());

            // clean up the reassigned tasks associated with workflow instance
            workflowModelAdapter.DeleteReassignedTaskInfosByWFInstanceId(workflowInstanceId.ToString());

            // delete the tasks that may have been created for the workflow instance
            ITaskService taskService = _workflowRuntime.GetService<ITaskService>();
            taskService.DeleteTasks(workflowInstanceId.ToString());

            // remove the newtera workflow instance at the end since other cleanups like cleaning up tasks needs to access the workflow instance
            RemoveWorkflowInstance(workflowInstanceId);
        }

        /// <summary>
        /// Workflow completed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkflowCompleted(object sender, WorkflowCompletedEventArgs e)
        {
            try
            {
                string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

                // perform cleanup
                Cleanup(e.WorkflowInstance.InstanceId);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Workflow terminated event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkflowTerminated(object sender, WorkflowTerminatedEventArgs e)
        {
            try
            {
                // cleanup
                string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

                // get the inner exception for the workflow instance if it is available
                Exception ex = e.Exception;
                NewteraWorkflowInstance workflowInstance = this.FindWorkflowInstance(e.WorkflowInstance.InstanceId);
                if (workflowInstance != null && workflowInstance.Exception != null)
                {
                    ex = workflowInstance.Exception;
                }

                if (ex != null)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("Workflow " + workflowInstanceId + " is terminated with reason:" + ex.Message).Append("\n");
                    sb.Append("The stack trace is: " + ex.StackTrace).Append("\n");
                    if (ex.InnerException != null && ex.InnerException.StackTrace != null)
                    {
                        sb.Append("The inner exception stack trace is:" + ex.InnerException.StackTrace);
                    }

                    // write the error into the workflow log
                    ITaskService taskService = _workflowRuntime.GetService<ITaskService>();
                    WorkflowModelAdapter adapter = new WorkflowModelAdapter();
                    WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByWorkflowInstanceId(workflowInstanceId);
                    string bindingInstanceId = "Unknown";
                    if (bindingInfo != null)
                    {
                        bindingInstanceId = bindingInfo.DataInstanceId;
                    }
                    taskService.WriteTaskLog(bindingInstanceId, ex.Message, "", "", "Terminated", workflowInstanceId);

                    ErrorLog.Instance.WriteLine(sb.ToString());

                    // perform cleanup
                    Cleanup(e.WorkflowInstance.InstanceId);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Workflow abortd event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkflowAborted(object sender, WorkflowEventArgs e)
        {
            try
            {
                // cleanup
                string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

                ErrorLog.Instance.WriteLine("Workflow " + workflowInstanceId + " is aborted:");

                // perform cleanup
                Cleanup(e.WorkflowInstance.InstanceId);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Workflow idled event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkflowIdled(object sender, WorkflowEventArgs e)
        {
            try
            {
                string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

                //ErrorLog.Instance.WriteLine("Workflow " + workflowInstanceId + " is idled:");

                // persists the workflow into database
                e.WorkflowInstance.TryUnload();
                return;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        private void WorkflowLoaded(object sender, WorkflowEventArgs e)
        {
            string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();
        }

        private void WorkflowCreated(object sender, WorkflowEventArgs e)
        {
            string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();
        }

        private void WorkflowPersisted(object sender, WorkflowEventArgs e)
        {
            string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

            // workflow instance will be persisted in the event of workflow complete, idled, and etc.
            // therefore it is a good time to do two things
            // First, save the changes that may have been made to the
            // instance by rules to the database.
            // 
            // And second clear the cached binding data instance that may have been created for the workflow instance
            // at the time of unloading, so that it will be created again from the database
            // next time it is requested
            IBindingInstanceService bindingService = InstanceWrapperFactory.Instance.BindingInstanceService;
            bindingService.SaveBindingInstance(e.WorkflowInstance.InstanceId);
            bindingService.ClearBindingInstance(e.WorkflowInstance.InstanceId);
        }

        private void WorkflowStarted(object sender, WorkflowEventArgs e)
        {
            string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

            ErrorLog.Instance.WriteLine("Workflow " + workflowInstanceId + " is started:");
        }

        private void WorkflowUnloaded(object sender, WorkflowEventArgs e)
        {
            string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

        }

        private void WorkflowSuspended(object sender, WorkflowSuspendedEventArgs e)
        {
            string workflowInstanceId = e.WorkflowInstance.InstanceId.ToString();

        }

        /// <summary>
        /// Gets current workflow instances from database
        /// </summary>
        /// <returns>Dictionary<Guid, NewteraWorkflowInstance></returns>
        private Dictionary<Guid, NewteraWorkflowInstance> GetWorkflowInstances()
        {
            Dictionary<Guid, NewteraWorkflowInstance> workflows = new Dictionary<Guid, NewteraWorkflowInstance>();
            try
            {
                WorkflowModelAdapter workflowAdaptor = new WorkflowModelAdapter();
                List<WorkflowInstanceBindingInfo> bindingInfos = workflowAdaptor.GetWorkflowInstances();
                NewteraWorkflowInstance wfInstance;
                Guid wfInstanceId;
                foreach (WorkflowInstanceBindingInfo bindingInfo in bindingInfos)
                {
                    // get the workflow instance from persistent store
                    wfInstanceId = new Guid(bindingInfo.WorkflowInstanceId);
                    wfInstance = new NewteraWorkflowInstance(wfInstanceId,
                        bindingInfo.ProjectName,
                        bindingInfo.ProjectVersion,
                        bindingInfo.WorkflowName,
                        bindingInfo.WorkflowTypeId);
                    wfInstance.SchemaId = bindingInfo.SchemaId;
                    wfInstance.ClassName = bindingInfo.DataClassName;
                    wfInstance.ObjId = bindingInfo.DataInstanceId;

                    workflows.Add(wfInstanceId, wfInstance);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            return workflows;
        }

		static NewteraWorkflowRuntime()
		{
			// Initializing the factory.
			{
				theRuntime = new NewteraWorkflowRuntime();
			}
		}
	}
}
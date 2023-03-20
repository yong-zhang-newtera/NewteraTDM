/*
* @(#)HandleGroupTaskEventActivity.cs
*
* Copyright (c) 2004-2014 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Text;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.WorkflowServices;
using Newtera.WFModel;
using Newtera.Common.Wrapper;

namespace Newtera.Activities
{
    /// <summary>
    /// An activity that works with CreateGroupTaskActivity to suspend the workflow until all tasks created by CreateGroupTaskActivity
    /// are completed.
    /// </summary>
    [ActivityValidator(typeof(HandleGroupTaskEventValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    public partial class HandleGroupTaskEventActivity : Activity, IEventActivity, IActivityEventListener<QueueEventArgs>, ICustomTypeDescriptor
    {
        #region Default values
        
        #endregion

        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        private List<Guid> _subscriptionIds = null;
        private IComparable _queueName = null;
       
        #endregion

        // Define private constants for the Validation Errors 
        private const int InvalidActivityName = 1;
        
        #region Dependency Properties

        public static DependencyProperty ActivityNameProperty = DependencyProperty.Register("ActivityName", typeof(string), typeof(HandleGroupTaskEventActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        #endregion

        public HandleGroupTaskEventActivity()
        {
        }

        #region Activity Properties

        // Required by IEventActivity
        [Browsable(false)]
        public IComparable QueueName
        {
            get { return this._queueName; }
        }

        [Browsable(false)]
        public IComparable EventName
        {
            get { return "CloseTask"; }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The ActivityName property is used to specify the name of the CreateGroupTaskActivityNamePropertyEditor that creates a group of tasks..")]
        [CategoryAttribute("EventSource")]
        [DefaultValueAttribute(null)]
        [EditorAttribute("WorkflowStudio.CreateGroupTaskActivityNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public string ActivityName
        {
            get
            {
                return ((string)(base.GetValue(HandleGroupTaskEventActivity.ActivityNameProperty)));
            }
            set
            {
                base.SetValue(HandleGroupTaskEventActivity.ActivityNameProperty, value);
            }
        }


        #endregion

        #region Activity Execution Logic

        /// <summary>
        /// Initialize the activity
        /// </summary>
        /// <param name="provider"></param>
        protected override void Initialize(IServiceProvider provider)
        {
            if (this._queueName == null)
            {
                this._queueName = this.Name + Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// Execute the activity
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            try
            {
                if (this.ProcessQueueItem(context))
                {
                    return ActivityExecutionStatus.Closed;
                }

                this.DoSubscribe(context, this);

                // this block the activity
                return ActivityExecutionStatus.Executing;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                //throw ex;

                return ActivityExecutionStatus.Faulting;
            }
        }

        /// <summary>
        /// Subscribe the newtera event
        /// </summary>
        /// <param name="parentContext"></param>
        /// <param name="parentEventHandler"></param>
        void IEventActivity.Subscribe(ActivityExecutionContext parentContext, IActivityEventListener<QueueEventArgs> parentEventHandler)
        {
            try
            {
                DoSubscribe(parentContext, parentEventHandler);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;   
            }
        }

        /// <summary>
        /// Cancel the activity
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Cancel(ActivityExecutionContext context)
        {
            StringCollection taskIds = GetCreatedGroupTaskIds(context);

            foreach (string taskId in taskIds)
            {
                try
                {
                    DoCancel(context, taskId);
                }
                catch (Exception)
                {
                }
            }

            // unsubscriptions will be done by ProcessQueueItem method

            return ActivityExecutionStatus.Closed;
        }

        /// <summary>
        /// Unscribe the event
        /// </summary>
        /// <param name="parentContext"></param>
        /// <param name="parentEventHandler"></param>
        void IEventActivity.Unsubscribe(ActivityExecutionContext parentContext, IActivityEventListener<QueueEventArgs> parentEventHandler)
        {
            try
            {
                DoUnsubscribe(parentContext, parentEventHandler);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Create a workflow queue if it dosn't exist.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The queue</returns>
        private WorkflowQueue CreateQueue(ActivityExecutionContext context)
        {
            WorkflowQueuingService qService = context.GetService<WorkflowQueuingService>();

            if (!qService.Exists(this.QueueName))
            {
                WorkflowQueue queue = qService.CreateWorkflowQueue(this.QueueName, true);
            }

            return qService.GetWorkflowQueue(this.QueueName);
        }

        /// <summary>
        /// Subscribe to an newtera event
        /// </summary>
        /// <param name="context"></param>
        /// <param name="listener"></param>
        /// <returns>true if subscription is done.</returns>
        private Boolean DoSubscribe(ActivityExecutionContext context, IActivityEventListener<QueueEventArgs> listener)
        {
            bool status = true;
            WorkflowQueue queue = CreateQueue(context);
            queue.RegisterForQueueItemAvailable(listener, QualifiedName);

            if (_subscriptionIds == null)
            {
                _subscriptionIds = new List<Guid>();
            }

            IDBEventService dbEventService = context.GetService<IDBEventService>();
            StringCollection taskIds = GetCreatedGroupTaskIds(context);
            INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);
            foreach (string taskId in taskIds)
            {
                // register an event listener for event with name "CloseTask_XXXXXXXX" where the XXXXXXXX is the task id
                Guid subscriptionId = dbEventService.RegisterListener(this.QueueName, rootActivity.SchemaId, rootActivity.ClassName, this.EventName + "_" + taskId, false);

                _subscriptionIds.Add(subscriptionId);
            }

            return status;
        }
  
        private void DoUnsubscribe(ActivityExecutionContext context, IActivityEventListener<QueueEventArgs> listener)
        {
            if (this._subscriptionIds != null)
            {
                IDBEventService dbEventService = context.GetService<IDBEventService>();
                foreach (Guid subscriptionId in _subscriptionIds)
                {
                    dbEventService.UnregisterListener(subscriptionId);
                }

                this._subscriptionIds = null;
            }

            WorkflowQueuingService qService = context.GetService<WorkflowQueuingService>();
            if (qService.Exists(this.QueueName))
            {
                WorkflowQueue queue = qService.GetWorkflowQueue(this.QueueName);

                queue.UnregisterForQueueItemAvailable(listener);
            }
        }

        private void DoCancel(ActivityExecutionContext context, string taskId)
        {
            // the workflow instance is cancelled, therefore, cancel the all tasks created
            if (this.ExecutionStatus == ActivityExecutionStatus.Executing && 
                taskId != null)
            {
                ITaskService taskService = context.GetService<ITaskService>();
                taskService.CloseTaskById(taskId);
            }
        }

        private void ClearTasks(ActivityExecutionContext context)
        {
            StringCollection taskIds = GetCreatedGroupTaskIds(context);
            ITaskService taskService = context.GetService<ITaskService>();

            foreach (string taskId in taskIds)
            {
                try
                {
                    taskService.CloseTaskById(taskId);
                }
                catch (Exception)
                {
                }
            }
        }

        void IActivityEventListener<QueueEventArgs>.OnEvent(object sender, QueueEventArgs e)
        {
            try
            {
                // If activity is not scheduled for execution, do nothing
                if (this.ExecutionStatus == ActivityExecutionStatus.Executing)
                {
                    ActivityExecutionContext context = sender as ActivityExecutionContext;
                    if (this.ProcessQueueItem(context))
                    {
                        context.CloseActivity();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Process the item from workflow queue
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if an item has been processed, false othwise.</returns>
        private bool ProcessQueueItem(ActivityExecutionContext context)
        {
            WorkflowQueuingService qService = context.GetService<WorkflowQueuingService>();
            if (!qService.Exists(this.QueueName))
            {
                return false;
            }

            WorkflowQueue queue = qService.GetWorkflowQueue(this.QueueName);

            // If the queue has messages, then process the first one, otherwise return
            if (queue.Count == 0)
            {
                return false;
            }

            // dequeue an item
            object item = queue.Dequeue();

            string completedTaskId = null;
            bool isCancelled = false;
            if (item is NewteraEventArgs)
            {
                completedTaskId = ((NewteraEventArgs)item).TaskId;

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = {"Task with id " + completedTaskId + " is completed",
                            "Activity Name: " + this.Name,
                            "Workflow Instance Id: " + this.WorkflowInstanceId.ToString(),
                            "Queue Name:" + this.QueueName};
                    TraceLog.Instance.WriteLines(messages);
                }
            }
            else if (item is CancelActivityEventArgs)
            {
                CancelActivityEventArgs eventArgs = (CancelActivityEventArgs)item;

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = {" Cancel activity event is received by activity.",
                            "Activity Name: " + this.Name,
                            "Workflow Instance Id: " + this.WorkflowInstanceId.ToString(),
                            "Queue Name:" + this.QueueName};
                    TraceLog.Instance.WriteLines(messages);
                }

                isCancelled = true;

                ClearTasks(context); // Clear the remaining tasks associated with the activity
            }

            if (AllCreatedTasksDone(context) || isCancelled)
            {
                // remove the subscription from the service
                DoUnsubscribe(context, this);

                // remove the queue
                DeleteQueue(context);

                InstanceWrapperFactory.Instance.BindingInstanceService.ClearBindingInstance(this.WorkflowInstanceId);

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AllCreatedTasksDone(ActivityExecutionContext context)
        {
            bool status = false;

            StringCollection taskIds = GetCreatedGroupTaskIds(context);

            if (taskIds == null || taskIds.Count == 0)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Must delete queue as a separate action, rather than as part of unsubscription of
        /// event. Otherwise, this activity won't work correctly in a ListenActivity
        /// </summary>
        /// <param name="context"></param>
        private void DeleteQueue(ActivityExecutionContext context)
        {
            WorkflowQueuingService qService = context.GetService<WorkflowQueuingService>();
            qService.DeleteWorkflowQueue(this.QueueName);
        }

        private StringCollection GetCreatedGroupTaskIds(ActivityExecutionContext context)
        {
            StringCollection taskIds = new StringCollection();
            ITaskService taskService = context.GetService<ITaskService>();

            List<TaskInfo> taskInfos = taskService.GetWorkflowInstanceTasks(this.WorkflowInstanceId.ToString());

            // make sure the task is created by the CreateGroupTaskActivity
            foreach (TaskInfo taskInfo in taskInfos)
            {
                if (taskInfo.ActivityName == this.ActivityName)
                {
                    taskIds.Add(taskInfo.TaskId);
                }
            }

            return taskIds;
        }

        #endregion

        #region ICustomTypeDescriptor

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// Called to get the properties of a type.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to be globalized
                foreach (PropertyDescriptor property in baseProps)
                {
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Our implementation overrides GetProperties() only and creates a
        /// collection of custom property descriptors of type GlobalizedPropertyDescriptor
        /// and returns them to the caller instead of the default ones.
        /// </summary>
        /// <returns>A collection of Property Descriptors.</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            // Only do once
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProperties = TypeDescriptor.GetProperties(this, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to 
                // be globalized
                foreach (PropertyDescriptor property in baseProperties)
                {
                    // create our custom property descriptor and add it to the collection
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        /// <summary>
        /// Custom validator for HandleGroupTaskEventActivity
        /// </summary>
        public class HandleGroupTaskEventValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                HandleGroupTaskEventActivity activity = obj as HandleGroupTaskEventActivity;
                if (activity != null)
                {
                    // Validate the ActivityName property
                    if (String.IsNullOrEmpty(activity.ActivityName))
                    {
                        validationErrors.Add(ValidationError.GetNotSetValidationError(HandleGroupTaskEventActivity.ActivityNameProperty.Name));

                    }
                    else if (!IsValidActivityName(activity))
                    {
                        validationErrors.Add(new ValidationError(activity.ActivityName + " is not a valid CreateGroupTaskActivity name.", InvalidActivityName));
                    }
                }

                return validationErrors;
            }

            private bool IsValidActivityName(HandleGroupTaskEventActivity activity)
            {
                Activity temp = activity.GetActivityByName(activity.ActivityName);
                if (temp != null && temp is CreateGroupTaskActivity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
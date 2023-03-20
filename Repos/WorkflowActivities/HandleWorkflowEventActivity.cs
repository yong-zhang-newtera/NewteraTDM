/*
* @(#)HandleWorkflowEventActivity.cs
*
* Copyright (c) 2010-2012 Newtera, Inc. All rights reserved.
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

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.WorkflowServices;
using Newtera.WFModel;
using Newtera.Common.Wrapper;

namespace Newtera.Activities
{
    [ActivityValidator(typeof(HandleWorkflowEventValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    public partial class HandleWorkflowEventActivity : Activity, IEventActivity, IActivityEventListener<QueueEventArgs>, ICustomTypeDescriptor
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

        public static DependencyProperty ActivityNameProperty = DependencyProperty.Register("ActivityName", typeof(string), typeof(HandleWorkflowEventActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        #endregion

        public HandleWorkflowEventActivity()
        {
        }

        #region Activity Properties

        // Required by IEventActivity
        [Browsable(false)]
        public IComparable QueueName
        {
            get { return this._queueName; }
        }

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The ActivityName property is used to specify the name of the InvokeAsyncNewteraWorkflowActivity that starts a workflow.")]
        [CategoryAttribute("EventSource")]
        [DefaultValueAttribute(null)]
        [EditorAttribute("WorkflowStudio.InvokedWorkflowActivityNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public string ActivityName
        {
            get
            {
                return ((string)(base.GetValue(HandleWorkflowEventActivity.ActivityNameProperty)));
            }
            set
            {
                base.SetValue(HandleWorkflowEventActivity.ActivityNameProperty, value);
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
                List<Guid> invokedWrokflowInstanceIds = GetInvokedWorkflowInstanceIds(context);

                if (this.ProcessQueueItem(context))
                {
                    return ActivityExecutionStatus.Closed;
                }

                this.DoSubscribe(context, this, invokedWrokflowInstanceIds);

                // this block the activity
                return ActivityExecutionStatus.Executing;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
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
                List<Guid> invokedWorkflowInstanceIds = GetInvokedWorkflowInstanceIds(parentContext);

                DoSubscribe(parentContext, parentEventHandler, invokedWorkflowInstanceIds);
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
            List<Guid> invokedWorklflowInstanceIds = GetInvokedWorkflowInstanceIds(context);

            foreach (Guid workflowInstanceId in invokedWorklflowInstanceIds)
            {
                DoCancel(context, workflowInstanceId);
            }

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
        private Boolean DoSubscribe(ActivityExecutionContext context, IActivityEventListener<QueueEventArgs> listener, List<Guid> invokedWorklflowInstanceIds)
        {
            bool status = true;
            WorkflowQueue queue = CreateQueue(context);
            queue.RegisterForQueueItemAvailable(listener, QualifiedName);

            if (_subscriptionIds == null)
            {
                _subscriptionIds = new List<Guid>();
            }

            IWorkflowService workflowService = context.GetService<IWorkflowService>();
            foreach (Guid invokedWorkflowInstanceId in invokedWorklflowInstanceIds)
            {
                Guid subscriptionId = workflowService.RegisterListener(this.QueueName, invokedWorkflowInstanceId);

                _subscriptionIds.Add(subscriptionId);

            }
            return status;
        }
  
        private void DoUnsubscribe(ActivityExecutionContext context, IActivityEventListener<QueueEventArgs> listener)
        {
            if (this._subscriptionIds != null)
            {
                IWorkflowService workflowService = context.GetService<IWorkflowService>();
                foreach (Guid subscriptionId in _subscriptionIds)
                {
                    workflowService.UnregisterListener(subscriptionId);
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

        private void DoCancel(ActivityExecutionContext context, Guid workflowInstanceId)
        {
            try
            {
                // the master workflow instance is cancelled, therefore,
                // cancel the workflow instance invoked
                if (this.ExecutionStatus == ActivityExecutionStatus.Executing &&
                    workflowInstanceId != Guid.Empty)
                {
                    IWorkflowService workflowService = context.GetService<IWorkflowService>();
                    workflowService.CancelWorkflow(workflowInstanceId);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                //throw ex;
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

            // If the queue has messages, then process the first one
            if (queue.Count == 0)
            {
                return false;
            }

            // dequeue an item
            object item = queue.Dequeue();

            string invokedWorkflowInstanceId = null;
            if (item is NewteraEventArgs)
            {
                invokedWorkflowInstanceId = ((NewteraEventArgs)item).WorkflowInstanceId;

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = {"Child workflow with id " + invokedWorkflowInstanceId + " is completed or terminated.",
                            "Activity Name: " + this.Name,
                            "Workflow Instance Id: " + this.WorkflowInstanceId.ToString(),
                            "Queue Name:" + this.QueueName};
                    TraceLog.Instance.WriteLines(messages);
                }
            }

            // cancel the invoked workflow instance if the queued item is CancelWorkflowException
            if (item is CancelWorkflowException)
            {
                invokedWorkflowInstanceId = ((CancelWorkflowException)item).Message;
                DoCancel(context, new Guid(invokedWorkflowInstanceId));
            }

            if (AllInvokedWorkflowInstancesDone(context))
            {
                // remove the subscription from the service
                DoUnsubscribe(context, this);

                // remove the queue
                DeleteQueue(context);

                return true;
            }
            else
            {
                return false;
            }

            /*
            if (item is CancelWorkflowException)
            {
                // throw the exception to terminate the workflow instance
                throw (CancelWorkflowException)item;
            }
            */
        }

        private bool AllInvokedWorkflowInstancesDone(ActivityExecutionContext context)
        {
            bool status = false;

            List<Guid> invokedWorkflowInstanceIds = GetInvokedWorkflowInstanceIds(context);

            if (invokedWorkflowInstanceIds.Count == 0)
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

        private List<Guid> GetInvokedWorkflowInstanceIds(ActivityExecutionContext context)
        {
            IWorkflowService workflowService = context.GetService<IWorkflowService>();

            // get all child workflow instance ids invoked by the master workflow with the given activity
            List<Guid> childWorkflowInstanceIds = workflowService.GetChildWorkflowInstanceIds(this.WorkflowInstanceId.ToString(), this.ActivityName);

            return childWorkflowInstanceIds;
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
        /// Custom validator for HandleWorkflowEventActivity
        /// </summary>
        public class HandleWorkflowEventValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                HandleWorkflowEventActivity activity = obj as HandleWorkflowEventActivity;
                if (activity != null)
                {
                    // Validate the ActivityName property
                    if (String.IsNullOrEmpty(activity.ActivityName))
                    {
                        validationErrors.Add(ValidationError.GetNotSetValidationError(HandleWorkflowEventActivity.ActivityNameProperty.Name));

                    }
                    else if (!IsValidActivityName(activity))
                    {
                        validationErrors.Add(new ValidationError(activity.ActivityName + " is not a valid InvokeAsyncNewteraWorkflowActivity name.", InvalidActivityName));
                    }
                }

                return validationErrors;
            }

            private bool IsValidActivityName(HandleWorkflowEventActivity activity)
            {
                Activity temp = activity.GetActivityByName(activity.ActivityName);
                if (temp != null && temp is InvokeAsyncNewteraWorkflowActivity)
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
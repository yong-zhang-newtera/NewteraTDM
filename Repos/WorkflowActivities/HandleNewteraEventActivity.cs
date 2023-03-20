/*
* @(#)HandleNewteraEventActivity.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections;
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

namespace Newtera.Activities
{
    [ActivityValidator(typeof(HandleNewteraEventValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    public partial class HandleNewteraEventActivity : Activity, IEventActivity, IActivityEventListener<QueueEventArgs>, ICustomTypeDescriptor
    {
        #region Default values
        
        #endregion

        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        private Guid _subscriptionId = Guid.Empty;
        private IComparable _queueName = null;
       
        #endregion

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;
        
        #region Dependency Properties

        public static DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(HandleNewteraEventActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty CreateDataBindingProperty =
            DependencyProperty.Register("CreateDataBinding", typeof(bool), typeof(HandleNewteraEventActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        #endregion

        public HandleNewteraEventActivity()
        {
            this.CreateDataBinding = true;
        }

        #region Activity Properties

        // Required by IEventActivity
        [Browsable(false)]
        public IComparable QueueName
        {
            get { return this._queueName; }
        }

        [
            CategoryAttribute("EventSource"),
            DescriptionAttribute("Specify a event to listen to."),
		    DefaultValueAttribute(null),
            EditorAttribute("WorkflowStudio.EventNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public string EventName
        {
            get
            {
                return ((string)(base.GetValue(HandleNewteraEventActivity.EventNameProperty)));
            }
            set
            {
                base.SetValue(HandleNewteraEventActivity.EventNameProperty, value);
            }
        }

        [
         CategoryAttribute("DataBinding"),
         DescriptionAttribute("Specify whether to create a binding between this workflow instance and the data instance where the event comes from."),
         DefaultValueAttribute(true),
        ]
        public bool CreateDataBinding
        {
            get
            {
                return ((bool)(base.GetValue(HandleNewteraEventActivity.CreateDataBindingProperty)));
            }
            set
            {
                base.SetValue(HandleNewteraEventActivity.CreateDataBindingProperty, value);
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
            return DoCancel(context);
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
            WorkflowQueue queue = CreateQueue(context);
            queue.RegisterForQueueItemAvailable(listener, QualifiedName);

            IDBEventService dbEventService = context.GetService<IDBEventService>();
            INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);
            this._subscriptionId = dbEventService.RegisterListener(this.QueueName, rootActivity.SchemaId, rootActivity.ClassName, this.EventName, this.CreateDataBinding);
            return (_subscriptionId != Guid.Empty);
        }
  
        private void DoUnsubscribe(ActivityExecutionContext context, IActivityEventListener<QueueEventArgs> listener)
        {
            if (!this._subscriptionId.Equals(Guid.Empty))
            {
                IDBEventService dbEventService = context.GetService<IDBEventService>();
                dbEventService.UnregisterListener(this._subscriptionId);
                this._subscriptionId = Guid.Empty;
            }

            WorkflowQueuingService qService = context.GetService<WorkflowQueuingService>();
            if (qService.Exists(this.QueueName))
            {
                WorkflowQueue queue = qService.GetWorkflowQueue(this.QueueName);

                queue.UnregisterForQueueItemAvailable(listener);
            }
        }

        private ActivityExecutionStatus DoCancel(ActivityExecutionContext context)
        {
            try
            {
                DoUnsubscribe(context, this);
                DeleteQueue(context);
                return ActivityExecutionStatus.Closed;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
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

            // write trace info
            if (TraceLog.Instance.Enabled && item is NewteraEventArgs)
            {
                NewteraEventArgs eventArgs = (NewteraEventArgs)item;

                string[] messages = {eventArgs.EventName + " event is received by activity.",
                            "Activity Name: " + this.Name,
                            "Event Class Name: " + eventArgs.ClassName,
                            "Event Schema:" + eventArgs.SchemaId,
                            "Binding Data Instance: " + eventArgs.ObjId,
                            "Workflow Instance Id: " + this.WorkflowInstanceId.ToString(),
                            "Queue Name:" + this.QueueName};
                TraceLog.Instance.WriteLines(messages);
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
            }

            // remove the subscription from the service and delete queue
            DoUnsubscribe(context, this);

            DeleteQueue(context); // remove the queue

            if (item is CancelWorkflowException)
            {
                // the activity is cancelled, throw the exception to terminate the workflow
                throw (CancelWorkflowException) item;
            }

            return true;
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
        /// Custom validator for HandleNewteraEventActivity
        /// </summary>
        public class HandleNewteraEventValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                HandleNewteraEventActivity activity = obj as HandleNewteraEventActivity;

                if (activity != null)
                {
                    // Validate the SchemaId, ClassName and EventName Properties
                    this.ValidateEventSourceProperties(validationErrors, activity);
                }
                return validationErrors;
            }

            private void ValidateEventSourceProperties(ValidationErrorCollection validationErrors, HandleNewteraEventActivity activity)
            {
                // Validate the EventName property
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);
                if (String.IsNullOrEmpty(activity.EventName))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(HandleNewteraEventActivity.EventNameProperty.Name));
                }
                else if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidEventName(rootActivity.SchemaId, rootActivity.ClassName, activity.EventName))
                {
                    validationErrors.Add(new ValidationError(activity.EventName + " event doesn't exist in the class " + rootActivity.ClassName + " of database schema " + rootActivity.SchemaId, UnknownAssignment));
                }
            }
        }
    }
}
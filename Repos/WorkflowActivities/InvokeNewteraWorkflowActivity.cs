/*
* @(#)InvokeNewteraWorkflowActivity.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections;
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
using System.Reflection;

using Newtera.Common.Core;
using Newtera.WFModel;
using Newtera.Common.MetaData;
using Newtera.WorkflowServices;

namespace Newtera.Activities
{
    #region InvokeMethod enum

    public enum InvokeMethod
    {
        // synchronous
        Synchronous,
        // asynchronous
        Asynchronous
    }

    #endregion InvokeMethod

    [ActivityValidator(typeof(InvokeNewteraWorkflowValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    [DefaultProperty("WorkflowName")]
    public partial class InvokeNewteraWorkflowActivity : Activity, IInvokeWorkflowActivity, IEventActivity, IActivityEventListener<QueueEventArgs>, IEnumerableActivity, ICustomTypeDescriptor
    {
        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        private Guid _subscriptionId = Guid.Empty;
        private IComparable _queueName = null;
        private Guid _worklfowInstanceId = Guid.Empty;

        private object _currentItem = null;

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;
       
        #endregion
        
        #region Dependency Properties

        public static DependencyProperty ProjectNameProperty =
            DependencyProperty.Register("ProjectName", typeof(string), typeof(InvokeNewteraWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty WorkflowNameProperty =
            DependencyProperty.Register("WorkflowName", typeof(string), typeof(InvokeNewteraWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty InvokeMethodProperty =
            DependencyProperty.Register("InvokeMethod", typeof(InvokeMethod), typeof(InvokeNewteraWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty ParameterBindingsProperty =
            DependencyProperty.Register("ParameterBindings", typeof(IList), typeof(InvokeNewteraWorkflowActivity)); // run-time update

        #endregion

        public InvokeNewteraWorkflowActivity()
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
        // IInvokeWorkflowActivity
        public string ProjectName
        {
            get
            {
                return ((string)(base.GetValue(InvokeNewteraWorkflowActivity.ProjectNameProperty)));
            }
            set
            {
                base.SetValue(InvokeNewteraWorkflowActivity.ProjectNameProperty, value);
            }
        }
       
        [
            CategoryAttribute("Workflow"),
            DescriptionAttribute("Specify a workflow to be invoked."),
		    DefaultValueAttribute(null),
            EditorAttribute("WorkflowStudio.WorkflowNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        // IInvokeWorkflowActivity
        public string WorkflowName
        {
            get
            {
                return ((string)(base.GetValue(InvokeNewteraWorkflowActivity.WorkflowNameProperty)));
            }
            set
            {
                if (value == null || value != WorkflowName)
                {
                    ParameterBindings = null; // clear the parameter bidnings
                }

                base.SetValue(InvokeNewteraWorkflowActivity.WorkflowNameProperty, value);
            }
        }

        [
            CategoryAttribute("Workflow"),
            DefaultValueAttribute(InvokeMethod.Synchronous),
            DescriptionAttribute("Specify the invoke method."),
        ]
        public InvokeMethod InvokeMethod
        {
            get
            {
                return ((InvokeMethod)(base.GetValue(InvokeNewteraWorkflowActivity.InvokeMethodProperty)));
            }
            set
            {
                base.SetValue(InvokeNewteraWorkflowActivity.InvokeMethodProperty, value);
            }
        }

        [
            CategoryAttribute("Parameters"),
            DescriptionAttribute("Specify input parameter bindings of the workflow to be invoked."),
            DefaultValueAttribute(null),    
            EditorAttribute("WorkflowStudio.ParameterBindingsPropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        // IInvokeWorkflowActivity
        public IList ParameterBindings
        {
            get
            {

                IList bindings = ((IList)(base.GetValue(InvokeNewteraWorkflowActivity.ParameterBindingsProperty)));
                
                // create a list of parameter bindings at the first time or when the
                // input parameters of the invoking workflow has been changed
                if (this.DesignMode &&
                    !string.IsNullOrEmpty(this.WorkflowName) &&
                    (bindings == null || bindings.Count == 0))
                {
                    bindings = GetParameterBindings(this.WorkflowName);

                    ParameterBindings = bindings;
                }

                return bindings;
            }
            set
            {
                base.SetValue(InvokeNewteraWorkflowActivity.ParameterBindingsProperty, value);
            }
        }

        #endregion

        #region IEnumerableActivity

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object CurrentItem
        {
            get
            {
                return _currentItem;
            }
            set
            {
                _currentItem = value;
            }
        }

        #endregion

        #region Activity Execution Logic

        /// <summary>
        /// Initialize the activity, always create a new queue name
        /// </summary>
        /// <param name="provider"></param>
        protected override void Initialize(IServiceProvider provider)
        {
            this._queueName = this.Name + Guid.NewGuid().ToString();
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
                // obtains values from the parameter bindings
                ObtainParameterBindingValues();

                string workflowName = this.WorkflowName;

                // invoke the given workflow in the same project
                this._worklfowInstanceId = InvokeWorkflow(context, workflowName, this.ParameterBindings);

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = {workflowName + " workflow is invoked.",
                            "By Activity Name: " + this.Name,
                            "Invoked workflow Instance Id: " + _worklfowInstanceId.ToString()};
                    TraceLog.Instance.WriteLines(messages);
                }

                if (this.InvokeMethod == InvokeMethod.Synchronous)
                {
                    // block for the events generated from workflow instance completion or termination
                    if (this.ProcessQueueItem(context))
                    {
                        return ActivityExecutionStatus.Closed;
                    }

                    this.DoSubscribe(context, this);

                    return ActivityExecutionStatus.Executing;
                }
                else
                {
                    // asynchronous call, do not block
                    return ActivityExecutionStatus.Closed;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Obtain the values for the specified parameter bindings
        /// </summary>
        private void ObtainParameterBindingValues()
        {
            if (this.ParameterBindings != null)
            {
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);

                foreach (InputParameter parameter in this.ParameterBindings)
                {
                    parameter.Value = ActivityUtil.GetParameterValue(parameter.ParameterBinding, rootActivity, this);
                }
            }
        }

        /// <summary>
        /// Invoke an instance of the specified workflow in a project
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="subWorkflowName">The name of the workflow to be invoked.</param>
        /// <param name="inputParameters">The input parameters</param>
        /// <returns>The workflow instance id</returns>
        private Guid InvokeWorkflow(ActivityExecutionContext context, string subWorkflowName,
            IList inputParameters)
        {
            INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);

            IWorkflowService workflowService = context.GetService<IWorkflowService>();
            return workflowService.StartWorkflowInstance(subWorkflowName, inputParameters);

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
        /// Cancel the activity, called by the workflow runtime
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
        /// Delete the workflow queue
        /// </summary>
        /// <param name="context"></param>
        private void DeleteQueue(ActivityExecutionContext context)
        {
            WorkflowQueuingService qService = context.GetService<WorkflowQueuingService>();
            qService.DeleteWorkflowQueue(this.QueueName);
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

            IWorkflowService workflowService = context.GetService<IWorkflowService>();
            this._subscriptionId = workflowService.RegisterListener(this.QueueName, this._worklfowInstanceId);
            return (_subscriptionId != Guid.Empty);
        }
  
        private void DoUnsubscribe(ActivityExecutionContext context, IActivityEventListener<QueueEventArgs> listener)
        {
            if (!this._subscriptionId.Equals(Guid.Empty))
            {
                IWorkflowService workflowService = context.GetService<IWorkflowService>();
                workflowService.UnregisterListener(this._subscriptionId);
                this._subscriptionId = Guid.Empty;
            }

            WorkflowQueuingService qService = context.GetService<WorkflowQueuingService>();
            if (qService.Exists(this.QueueName))
            {
                WorkflowQueue queue = qService.GetWorkflowQueue(this.QueueName);

                queue.UnregisterForQueueItemAvailable(listener);
            }
        }

        /// <summary>
        /// Cancel the the invoked workflow instance
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private ActivityExecutionStatus DoCancel(ActivityExecutionContext context)
        {
            try
            {
                // the master workflow instance is cancelled, therefore,
                // cancel the workflow instance invoked by this activity
                if (this.ExecutionStatus == ActivityExecutionStatus.Executing &&
                    this._worklfowInstanceId != null)
                {
                    IWorkflowService workflowService = context.GetService<IWorkflowService>();
                    workflowService.CancelWorkflow(_worklfowInstanceId);
                }

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

            if (TraceLog.Instance.Enabled)
            {
                string[] messages = {"The invoked workflow " + this.WorkflowName + " is completed or terminated.",
                            "Activity Name: " + this.Name,
                            "Workflow Instance Id: " + this.WorkflowInstanceId.ToString(),
                            "Queue Name:" + this.QueueName};
                TraceLog.Instance.WriteLines(messages);
            }

            // cancel the invoked workflow instance if the queued item is CancelWorkflowException
            if (item is CancelWorkflowException)
            {
                DoCancel(context);
            }
 
            // remove the subscription from the service
            DoUnsubscribe(context, this);

            // remove the queue
            DeleteQueue(context);

            if (item is CancelWorkflowException)
            {
                // throw the exception to terminate the workflow instance
                throw (CancelWorkflowException)item;
            }

            return true;
        }

        /// <summary>
        /// Gets the input parameters of the invoking workflow
        /// </summary>
        /// <param name="worklflowName">The workflow name</param>
        /// <returns>The input parameters of the invoking workflow</returns>
        private IList GetInputParameters(string workflowName)
        {
            IList inputParameters = null;
            INewteraWorkflow invokedRootActivity = null;

            // it is a design time, get root activity from the workflow model
            ProjectModel projectModel = ProjectModelContext.Instance.Project;
            if (projectModel != null)
            {
                WorkflowModel workflowModel = (WorkflowModel)projectModel.Workflows[workflowName];
                if (workflowModel != null)
                {
                    // the workflow model's root activity hasn't been instantiated
                    invokedRootActivity = workflowModel.CreateRootActivity() as INewteraWorkflow;
                }

                if (invokedRootActivity != null)
                {
                    inputParameters = invokedRootActivity.InputParameters;
                }
            }

            return inputParameters;
        }

        /// <summary>
        /// Get the parameter bindings, called at the design time
        /// </summary>
        /// <param name="workflowName">The workflow name</param>
        /// <returns>A list of parameter bindings</returns>
        private IList GetParameterBindings(string workflowName)
        {
            IList parameterBindings = null;
            IList inputParameters = GetInputParameters(workflowName);
            InputParameter binding;

            if (inputParameters != null)
            {
                parameterBindings = new ArrayList();
                foreach (InputParameter param in inputParameters)
                {
                    binding = param.Clone();

                    parameterBindings.Add(binding);
                }
            }

            return parameterBindings;
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

        #region InvokeNewteraWorkflowValidator

        /// <summary>
        /// Custom validator for InvokeNewteraWorkflowActivity
        /// </summary>
        public class InvokeNewteraWorkflowValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                InvokeNewteraWorkflowActivity activity = obj as InvokeNewteraWorkflowActivity;
                if (activity != null)
                {
                    // Validate the WorkflowName Properties
                    this.ValidateWorkflowNameProperties(validationErrors, activity);
                }
                return validationErrors;
            }

            /// <summary>
            /// Gets the information indicating whether there are difference between the existing
            /// bindings and the input parameters of the invoking workflow
            /// </summary>
            /// <param name="inputParameters">The input parameters</param>
            /// <param name="existingBindings">The existing bindings</param>
            /// <returns></returns>
            private bool IsParametersChanged(IList inputParameters, IList existingBindings)
            {
                bool status = false;

                if (inputParameters == null && existingBindings != null)
                {
                    status = true;
                }
                else if (inputParameters != null && existingBindings == null)
                {
                    status = true;
                }
                else if (inputParameters.Count != existingBindings.Count)
                {
                    status = true;
                }
                else
                {
                    bool found;
                    // make sure all input parameters has corresponding bindings
                    foreach (InputParameter inputParameter in inputParameters)
                    {
                        found = false;
                        foreach (InputParameter binding in existingBindings)
                        {
                            if (inputParameter.Name == binding.Name)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            status = true;
                            break;
                        }
                    }
                }

                return status;
            }

            private void ValidateWorkflowNameProperties(ValidationErrorCollection validationErrors, InvokeNewteraWorkflowActivity activity)
            {
                // Validate the WorkflowName property

                if (String.IsNullOrEmpty(activity.WorkflowName))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(InvokeNewteraWorkflowActivity.WorkflowNameProperty.Name));

                }
                else if (!String.IsNullOrEmpty(activity.WorkflowName)  && 
                    ProjectModelContext.Instance.Project != null &&
                    ProjectModelContext.Instance.Project.Workflows[activity.WorkflowName] == null)
                {
                    validationErrors.Add(new ValidationError("The workflow with name " + activity.WorkflowName + " doesn't exist in the project " + ProjectModelContext.Instance.Project.Name, UnknownAssignment));
                }
                else if (!String.IsNullOrEmpty(activity.WorkflowName) &&
                    activity.ParameterBindings != null && activity.ParameterBindings.Count > 0)
                {
                    // make sure the parameters is up to date
                    Activity rootActivity = null;
                    ProjectModel projectModel = ProjectModelContext.Instance.Project;
                    if (projectModel != null)
                    {
                        // this works at the design time but not at run-time
                        WorkflowModel workflowModel = (WorkflowModel)projectModel.Workflows[activity.WorkflowName];
                        rootActivity = workflowModel.CreateRootActivity();
                    }
                    
                    if (rootActivity != null)
                    {
                        INewteraWorkflow workflowActivity = rootActivity as INewteraWorkflow;
                        if (workflowActivity != null)
                        {
                            IList inputParameters = workflowActivity.InputParameters;

                            if (IsParametersChanged(inputParameters, activity.ParameterBindings))
                            {
                                activity.ParameterBindings = null; // clear the obsolete bindings
                                validationErrors.Add(new ValidationError("The parameter bindings do not match the input parameters of the workflow " + activity.WorkflowName + ", please specify the bindings again.", UnknownAssignment));
                            }
                            else if (activity.ParameterBindings != null)
                            {
                                // make sure that all the parameter binding are still valid

                                foreach (InputParameter binding in activity.ParameterBindings)
                                {
                                    if (binding.ParameterBinding.SourceType == SourceType.Unknown)
                                    {
                                        validationErrors.Add(new ValidationError("The parameter with name " + binding.Name + " has an unknown binding source.", UnknownAssignment));
                                    }
                                    else
                                    {
                                        switch (binding.ParameterBinding.SourceType)
                                        {
                                            case SourceType.DataInstance:
                                                if (!ActivityUtil.IsAttributeExist(binding.ParameterBinding.Path, activity))
                                                {
                                                    validationErrors.Add(new ValidationError("Unable to find the attribute with name " + binding.ParameterBinding.Path + " in the binding data class, make sure the data class hasn't been changed.", UnknownAssignment));
                                                }
                                                break;

                                            case SourceType.Parameter:
                                                if (!ActivityUtil.IsParameterExist(binding.ParameterBinding.Path, activity))
                                                {
                                                    validationErrors.Add(new ValidationError("Unable to find the parameter with name " + binding.ParameterBinding.Path + " among the input parameters of the workflow.", UnknownAssignment));
                                                }
                                                break;

                                            case SourceType.Activity:
                                                if (!ActivityUtil.IsActivityPropertyExist(binding.ParameterBinding.Path, activity))
                                                {
                                                    validationErrors.Add(new ValidationError("Unable to find the " + binding.ParameterBinding.Path + " as activity/property specification, make sure that the activity name hasn't been changed.", UnknownAssignment));
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion InvokeNewteraWorkflowValidator
    }
}
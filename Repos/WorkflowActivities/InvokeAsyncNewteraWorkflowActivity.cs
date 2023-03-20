/*
* @(#)InvokeAsyncNewteraWorkflowActivity.cs
*
* Copyright (c) 2010-2012 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections.Specialized;
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
using System.Reflection;

using Newtera.Common.Core;
using Newtera.WFModel;
using Newtera.Common.MetaData;
using Newtera.WorkflowServices;

namespace Newtera.Activities
{
    [ActivityValidator(typeof(InvokeAsyncNewteraWorkflowValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    [DefaultProperty("WorkflowName")]
    public partial class InvokeAsyncNewteraWorkflowActivity : Activity, IInvokeWorkflowActivity, IEnumerableActivity, ICustomTypeDescriptor
    {
        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        private Guid _worklfowInstanceId = Guid.Empty;

        private object _currentItem = null;

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;
       
        #endregion
        
        #region Dependency Properties

        public static DependencyProperty ProjectNameProperty =
            DependencyProperty.Register("ProjectName", typeof(string), typeof(InvokeAsyncNewteraWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty WorkflowNameProperty =
            DependencyProperty.Register("WorkflowName", typeof(string), typeof(InvokeAsyncNewteraWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty ParameterBindingsProperty =
            DependencyProperty.Register("ParameterBindings", typeof(IList), typeof(InvokeAsyncNewteraWorkflowActivity)); // run-time update

        #endregion

        public InvokeAsyncNewteraWorkflowActivity()
        {
        }

        #region Activity Properties


        [Browsable(false)]
        // IInvokeWorkflowActivity
        public string ProjectName
        {
            get
            {
                return ((string)(base.GetValue(InvokeAsyncNewteraWorkflowActivity.ProjectNameProperty)));
            }
            set
            {
                base.SetValue(InvokeAsyncNewteraWorkflowActivity.ProjectNameProperty, value);
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
                return ((string)(base.GetValue(InvokeAsyncNewteraWorkflowActivity.WorkflowNameProperty)));
            }
            set
            {
                if (value == null || value != WorkflowName)
                {
                    ParameterBindings = null; // clear the parameter bidnings
                }

                base.SetValue(InvokeAsyncNewteraWorkflowActivity.WorkflowNameProperty, value);
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

                IList bindings = ((IList)(base.GetValue(InvokeAsyncNewteraWorkflowActivity.ParameterBindingsProperty)));
                
                // create a list of parameter bindings at the first time or when the
                // input parameters of the invoking workflow has been changed
                if (this.DesignMode &&
                    !string.IsNullOrEmpty(WorkflowName) &&
                    (bindings == null || bindings.Count == 0))
                {
                    bindings = GetParameterBindings(WorkflowName);

                    ParameterBindings = bindings;
                }

                return bindings;
            }
            set
            {
                base.SetValue(InvokeAsyncNewteraWorkflowActivity.ParameterBindingsProperty, value);
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
                Guid childWorklfowInstanceId = InvokeWorkflow(context, workflowName, this.ParameterBindings);

                // add a subscription to db so that the child workflow is asscociated with the parent workflow 
                AddChildWorkflowEventSubscription(context, childWorklfowInstanceId);
               
                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = {workflowName + " workflow is invoked.",
                            "By Activity Name: " + this.Name,
                            "Invoked workflow Instance Id: " + childWorklfowInstanceId};
                    TraceLog.Instance.WriteLines(messages);
                }

                // asynchronous call, do not block
                return ActivityExecutionStatus.Closed;
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
        /// Add event subscription to the invoked workflow instance to data base
        /// </summary>
        /// <param name="context"></param>
        /// <param name="childWorklflowInstanceId">The child workflow instance id</param>
        private void AddChildWorkflowEventSubscription(ActivityExecutionContext context, Guid childWorklflowInstanceId)
        {
            IWorkflowService workflowService = context.GetService<IWorkflowService>();
            Guid subscriptionId = workflowService.AddChildWorkflowEventSubscription(childWorklflowInstanceId);
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

        #region InvokeAsyncNewteraWorkflowValidator

        /// <summary>
        /// Custom validator for InvokeAsyncNewteraWorkflowActivity
        /// </summary>
        public class InvokeAsyncNewteraWorkflowValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                InvokeAsyncNewteraWorkflowActivity activity = obj as InvokeAsyncNewteraWorkflowActivity;
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

            private void ValidateWorkflowNameProperties(ValidationErrorCollection validationErrors, InvokeAsyncNewteraWorkflowActivity activity)
            {
                // Validate the WorkflowName property
                if (String.IsNullOrEmpty(activity.WorkflowName))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(InvokeAsyncNewteraWorkflowActivity.WorkflowNameProperty.Name));

                }
                else if (ProjectModelContext.Instance.Project != null &&
                    ProjectModelContext.Instance.Project.Workflows[activity.WorkflowName] == null)
                {
                    validationErrors.Add(new ValidationError("The workflow with name " + activity.WorkflowName + " doesn't exist in the project " + ProjectModelContext.Instance.Project.Name, UnknownAssignment));
                }
                else if (activity.ParameterBindings != null && activity.ParameterBindings.Count > 0)
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

        #endregion InvokeAsyncNewteraWorkflowValidator
    }
}
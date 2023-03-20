/*
* @(#)BindDataInstanceActivity.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Data;
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
    [ActivityValidator(typeof(BindDataInstanceValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    public partial class BindDataInstanceActivity : System.Workflow.ComponentModel.Activity, ICustomTypeDescriptor
    {
        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;

        #endregion
        
        #region Dependency Properties

        public static DependencyProperty SearchStatementProperty =
            DependencyProperty.Register("SearchStatement", typeof(string), typeof(BindDataInstanceActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty SearchExprXmlProperty =
            DependencyProperty.Register("SearchExprXml", typeof(string), typeof(BindDataInstanceActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        #endregion

        public BindDataInstanceActivity()
        {
        }

        #region Activity Properties

        [
             CategoryAttribute("Activity"),
             DescriptionAttribute("Specify a search statement that retrive the data instance to be bound."),
		     DefaultValueAttribute(null),
             EditorAttribute("WorkflowStudio.SearchStatementPropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public string SearchStatement
        {
            get
            {
                return ((string)(base.GetValue(BindDataInstanceActivity.SearchStatementProperty)));
            }
            set
            {
                base.SetValue(BindDataInstanceActivity.SearchStatementProperty, value);
            }
        }

        [Browsable(false)]
        public string SearchExprXml
        {
            get
            {
                return ((string)(base.GetValue(BindDataInstanceActivity.SearchExprXmlProperty)));
            }
            set
            {
                base.SetValue(BindDataInstanceActivity.SearchExprXmlProperty, value);
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
        }

        /// During execution the CreateTask activity should create a task for each of the
        /// specified users in the database and send the email using SMTP if required.  
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            try
            {
                IWorkflowService workflowService = context.GetService<IWorkflowService>();
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);

                // if the workflow instance already has a binding, throw an exception
                string objId = workflowService.GetBindingDataInstanceId(rootActivity.ClassName,
                     rootActivity.SchemaId, this.WorkflowInstanceId);

                if (string.IsNullOrEmpty(objId))
                {

                    // Get a data instance
                    objId = GetDataInstanceId(context, rootActivity);

                    if (!string.IsNullOrEmpty(objId))
                    {
                        // create a binding between data instance created with the workflow instance
                        workflowService.SetWorkflowInstanceBinding(objId, rootActivity.ClassName,
                             rootActivity.SchemaId, this.WorkflowInstanceId);
                    }
                    else
                    {
                        throw new Exception("Unable to find a data instance using the specified search statement.");
                    }

                    // Retun the closed status indicating that this activity is complete.
                    return ActivityExecutionStatus.Closed;
                }
                else
                {
                    throw new Exception("The workflow instance " + this.WorkflowInstanceId.ToString() + " has already been bound to a data instance, therefore, can not bind to another data instance again.");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        private string GetDataInstanceId(ActivityExecutionContext context, INewteraWorkflow rootActivity)
        {
            string objId;
            string searchQuery = SubstituteParameterValues(rootActivity);

            IDataService dataService = context.GetService<IDataService>();
            DataSet ds = dataService.ExecuteQuery(rootActivity.SchemaId, rootActivity.ClassName, searchQuery);

            objId = ActivityUtil.GetObjId(ds, rootActivity.ClassName);

            return objId;
        }

        /// <summary>
        /// Substitute any parameter variables embedded in the insert statement with parameter values
        /// </summary>
        /// <returns>The query with parameters substituted with the values</returns>
        private string SubstituteParameterValues(INewteraWorkflow rootActivity)
        {
            string xquery = this.SearchStatement;
            if (rootActivity.InputParameters != null)
            {
                string variable;
                foreach (InputParameter inputParameter in rootActivity.InputParameters)
                {
                    variable = "{" + inputParameter.Name + "}";
                    if (xquery.Contains(variable))
                    {
                        if (inputParameter.Value != null)
                        {
                            // at run time, input parameter has value
                            xquery = xquery.Replace(variable, inputParameter.Value.ToString());
                        }
                        else
                        {
                            // Something is wrong, replace the variable with zero
                            xquery = xquery.Replace(variable, "0");
                        }
                    }
                }
            }

            return xquery;
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
        /// Custom validator for BindDataInstanceActivity
        /// </summary>
        public class BindDataInstanceValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                BindDataInstanceActivity activity = obj as BindDataInstanceActivity;
                if (activity != null)
                {
                    // Validate the Properties
                    this.ValidateProperties(validationErrors, activity);
                }
                return validationErrors;
            }

            private void ValidateProperties(ValidationErrorCollection validationErrors, BindDataInstanceActivity activity)
            {
                // Validate the EventName property
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);
                if (String.IsNullOrEmpty(activity.SearchStatement))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(BindDataInstanceActivity.SearchStatementProperty.Name));
                }
                else
                {
                    string searchQuery = SubstituteVariables(rootActivity, activity);
                    if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSearchQuery(rootActivity.SchemaId, rootActivity.ClassName, searchQuery))
                    {
                        validationErrors.Add(new ValidationError("Search query is invalid, please verify the query parameter or specify the search expression again.", UnknownAssignment));
                    }
                }
            }

            /// <summary>
            /// Substitute any parameter variables embedded in the search statement with parameter values
            /// </summary>
            /// <returns>The query with parameters substituted with the values</returns>
            private string SubstituteVariables(INewteraWorkflow rootActivity, BindDataInstanceActivity activity)
            {
                string xquery = activity.SearchStatement;
                if (rootActivity.InputParameters != null)
                {
                    string variable;
                    foreach (InputParameter inputParameter in rootActivity.InputParameters)
                    {
                        variable = "{" + inputParameter.Name + "}";
                        if (xquery.Contains(variable))
                        {
                            // at design time, input parameter deosn't have any value, replace the 
                            // variable with a constant
                            xquery = xquery.Replace(variable, "0");
                        }
                    }
                }

                return xquery;
            }
        }
    }
}
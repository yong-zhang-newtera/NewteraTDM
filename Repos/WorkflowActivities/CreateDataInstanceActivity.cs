/*
* @(#)CreateDataInstanceActivity.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
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
    [ActivityValidator(typeof(CreateDataInstanceValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    public partial class CreateDataInstanceActivity : System.Workflow.ComponentModel.Activity, ICustomTypeDescriptor
    {
        #region Default values
        
        #endregion

        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;

        #endregion
        
        #region Dependency Properties

        public static DependencyProperty InsertStatementProperty =
            DependencyProperty.Register("InsertStatement", typeof(string), typeof(CreateDataInstanceActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        #endregion

        public CreateDataInstanceActivity()
        {
        }

        #region Activity Properties

        [
         CategoryAttribute("DataSource"),
         DescriptionAttribute("Specify an insert statement."),
		 DefaultValueAttribute(null),
         EditorAttribute("WorkflowStudio.InsertStatementPropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public string InsertStatement
        {
            get
            {
                return ((string)(base.GetValue(CreateDataInstanceActivity.InsertStatementProperty)));
            }
            set
            {
                base.SetValue(CreateDataInstanceActivity.InsertStatementProperty, value);
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
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);

                // create a data instance
                string objId = CreateDataInstance(context, rootActivity);

                IWorkflowService workflowService = context.GetService<IWorkflowService>();

                // create a binding between data instance created with the workflow instance
                workflowService.SetWorkflowInstanceBinding(objId, rootActivity.ClassName,
                     rootActivity.SchemaId, this.WorkflowInstanceId);

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        private string CreateDataInstance(ActivityExecutionContext context, INewteraWorkflow rootActivity)
        {
            string insertQuery = SubstituteParameterValues(rootActivity);

            IDataService dataService = context.GetService<IDataService>();
            return dataService.ExecuteNonQuery(rootActivity.SchemaId, rootActivity.ClassName, insertQuery);
        }

        /// <summary>
        /// Substitute any parameter variables embedded in the insert statement with parameter values
        /// </summary>
        /// <returns>The query with parameters substituted with the values</returns>
        private string SubstituteParameterValues(INewteraWorkflow rootActivity)
        {
            string xquery = this.InsertStatement;
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
                            xquery = xquery.Replace(variable, inputParameter.Value.ToString());
                        }
                        else
                        {
                            // replace with an empty string
                            xquery = xquery.Replace(variable, string.Empty);
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
        /// Custom validator for CreateDataInstanceActivity
        /// </summary>
        public class CreateDataInstanceValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                CreateDataInstanceActivity activity = obj as CreateDataInstanceActivity;
                if (activity != null)
                {
                    // Validate the Properties
                    this.ValidateProperties(validationErrors, activity);
                }
                return validationErrors;
            }

            private void ValidateProperties(ValidationErrorCollection validationErrors, CreateDataInstanceActivity activity)
            {
                // Validate the EventName property
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);
                if (String.IsNullOrEmpty(activity.InsertStatement))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(CreateDataInstanceActivity.InsertStatementProperty.Name));
                }
                else if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidInsertQuery(rootActivity.SchemaId, rootActivity.ClassName, activity.InsertStatement))
                {
                    validationErrors.Add(new ValidationError("Insert query is invalid, please specify it again", UnknownAssignment));
                }
            }
        }
    }
}
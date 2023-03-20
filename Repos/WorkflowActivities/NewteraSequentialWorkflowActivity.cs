using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities;
using System.ComponentModel;
using System.Drawing.Design;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;

using Newtera.Common.Core;
using Newtera.Common.Wrapper;
using Newtera.WFModel;
using Newtera.Common.MetaData;
using Newtera.WorkflowServices;

namespace Newtera.Activities
{
    [ActivityValidator(typeof(NewteraSequentialWorkflowValidator))]
    [DefaultProperty("SchemaId")]
    public partial class NewteraSequentialWorkflowActivity : SequentialWorkflowActivity, INewteraWorkflow, ICustomTypeDescriptor
    {
        #region public event

        [Browsable(false)]
        public event EventHandler StartEventChanged;

        #endregion

        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        #endregion

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;

        #region Dependency Properties

        public static DependencyProperty SchemaIdProperty =
            DependencyProperty.Register("SchemaId", typeof(string), typeof(NewteraSequentialWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty ClassNameProperty =
            DependencyProperty.Register("ClassName", typeof(string), typeof(NewteraSequentialWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty ClassCaptionProperty =
            DependencyProperty.Register("ClassCaption", typeof(string), typeof(NewteraSequentialWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(NewteraSequentialWorkflowActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty InputParametersProperty =
            DependencyProperty.Register("InputParameters", typeof(IList), typeof(NewteraSequentialWorkflowActivity)); // run-time update

        #endregion

        public NewteraSequentialWorkflowActivity()
        {
        }

        #region INewteraWorkflow

        /// <summary>
        /// Gets or sets the schema id for the binding instance
        /// </summary>
        [
            CategoryAttribute("DataBinding"),
            DescriptionAttribute("Specify a schema of the bindig instance."),
            DefaultValueAttribute(null),
            EditorAttribute("WorkflowStudio.SchemaIdPropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public string SchemaId
        {
            get
            {
                return ((string)(base.GetValue(NewteraSequentialWorkflowActivity.SchemaIdProperty)));
            }
            set
            {
                base.SetValue(NewteraSequentialWorkflowActivity.SchemaIdProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the class name for the binding instance
        /// </summary>
        [
            CategoryAttribute("DataBinding"),
            DescriptionAttribute("Specify a class name for the binding instance."),
            DefaultValueAttribute(null),
            TypeConverterAttribute("WorkflowStudio.ClassNameConverter, WorkflowStudio"),
            EditorAttribute("WorkflowStudio.ClassNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public string ClassName
        {
            get
            {
                return ((string)(base.GetValue(NewteraSequentialWorkflowActivity.ClassNameProperty)));
            }
            set
            {
                base.SetValue(NewteraSequentialWorkflowActivity.ClassNameProperty, value);
            }
        }

        [Browsable(false)]
        public string ClassCaption
        {
            get
            {
                return ((string)(base.GetValue(NewteraSequentialWorkflowActivity.ClassCaptionProperty)));
            }
            set
            {
                base.SetValue(NewteraSequentialWorkflowActivity.ClassCaptionProperty, value);
            }
        }

        /// <summary>
        /// get the binding instance
        /// </summary>
        /// <remarks>The property is used at execution only, not available at design time</remarks>
        [Browsable(false)]
        public IInstanceWrapper Instance
        {
            get
            {
                if (!this.DesignMode)
                {
                    try
                    {
                        return InstanceWrapperFactory.Instance.GetInstanceWrapper(this.WorkflowInstanceId);
                    }
                    catch (Exception )
                    {
                        return new DummyInstance();
                    }
                }
                else
                {
                    return new DummyInstance();
                }
            }
        }

        [
            CategoryAttribute("StartEvent"),
            DescriptionAttribute("Specify an event that starts the workflow."),
            DefaultValueAttribute(null),
            EditorAttribute("WorkflowStudio.EventNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public string EventName
        {
            get
            {
                return ((string)(base.GetValue(NewteraSequentialWorkflowActivity.EventNameProperty)));
            }
            set
            {
                string oldVal = ((string)(base.GetValue(NewteraSequentialWorkflowActivity.EventNameProperty)));
                base.SetValue(NewteraSequentialWorkflowActivity.EventNameProperty, value);

                if (oldVal != value && this.StartEventChanged != null)
                {
                    StartEventChanged(this, new EventArgs());
                }
            }
        }

        [
            CategoryAttribute("Parameters"),
            DescriptionAttribute("Specify input parameters of the workflow."),
            DefaultValueAttribute(null),
            EditorAttribute("WorkflowStudio.InputParametersPropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public IList InputParameters
        {
            get
            {
                return ((IList)(base.GetValue(NewteraSequentialWorkflowActivity.InputParametersProperty)));
            }
            set
            {
                base.SetValue(NewteraSequentialWorkflowActivity.InputParametersProperty, value);
            }
        }

        /// <summary>
        /// override the execute method to obtain the input parameter values from a service
        /// </summary>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            if (InputParameters != null)
            {
                // get input parameter values
                IWorkflowService workflowService = executionContext.GetService<IWorkflowService>();

                foreach (InputParameter inputParameter in InputParameters)
                {
                    inputParameter.Value = workflowService.GetInputParameterValue(this.WorkflowInstanceId.ToString(), inputParameter.Name);

                    workflowService.RemoveInputParameterValue(this.WorkflowInstanceId.ToString(), inputParameter.Name); // clear the cache
                }
            }

            return base.Execute(executionContext);
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
        /// Custom validator for NewteraSequentialWorkflowActivity
        /// </summary>
        public class NewteraSequentialWorkflowValidator : System.Workflow.ComponentModel.Compiler.CompositeActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                NewteraSequentialWorkflowActivity activity = obj as NewteraSequentialWorkflowActivity;

                if (activity != null)
                {
                    // Validate the SchemaId, and ClassName Properties
                    this.ValidateProperties(validationErrors, activity);
                }

                return validationErrors;
            }

            private void ValidateProperties(ValidationErrorCollection validationErrors, NewteraSequentialWorkflowActivity activity)
            {
                // Validate the SchemaId property
                if (!String.IsNullOrEmpty(activity.SchemaId) &&
                    !ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(activity.SchemaId))
                {
                    validationErrors.Add(new ValidationError(activity.SchemaId + " is an unknown database schema or denied to access.", UnknownAssignment));
                }

                // Validate the ClassName property
                if (!String.IsNullOrEmpty(activity.ClassName) &&
                    !ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(activity.SchemaId, activity.ClassName))
                {
                    validationErrors.Add(new ValidationError(activity.ClassName + " class doesn't exist in database schema " + activity.SchemaId, UnknownAssignment));
                }

                // Validate the EventName property
                if (!String.IsNullOrEmpty(activity.EventName))
                {
                    if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidEventName(activity.SchemaId, activity.ClassName, activity.EventName))
                    {
                        validationErrors.Add(new ValidationError(activity.EventName + " event doesn't exist in the class " + activity.ClassName + " of database schema " + activity.SchemaId, UnknownAssignment));
                    }
                }
            }
        }
    }
}
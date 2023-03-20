/*
* @(#)InvokeCustomFunctionActivity.cs
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
using System.Threading;
using System.Drawing.Design;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Principal;

using Newtera.Common.Core;
using Newtera.WFModel;
using Newtera.Common.MetaData;
using Newtera.Common.Wrapper;
using Newtera.WorkflowServices;
using Newtera.Common.MetaData.Principal;

namespace Newtera.Activities
{
    [ActivityValidator(typeof(InvokeCustomFunctionValidator))]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    [DefaultProperty("FunctionDefinition")]
    public partial class InvokeCustomFunctionActivity : Activity, ICustomTypeDescriptor
    {
        #region Private data

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;
       
        #endregion
        
        #region Dependency Properties

        public static DependencyProperty FunctionDefinitionProperty =
            DependencyProperty.Register("FunctionDefinition", typeof(string), typeof(InvokeCustomFunctionActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty InvokeMethodProperty =
            DependencyProperty.Register("InvokeMethod", typeof(InvokeMethod), typeof(InvokeCustomFunctionActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static readonly DependencyProperty ReturnValueProperty = DependencyProperty.Register("ReturnValue", typeof(string), typeof(InvokeCustomFunctionActivity));

        #endregion

        public InvokeCustomFunctionActivity()
        {
        }

        #region Activity Properties
       
        [
            CategoryAttribute("Activity"),
            DescriptionAttribute("Specify a custom function to be invoked."),
		    DefaultValueAttribute(null),
        ]
        public string FunctionDefinition
        {
            get
            {
                return ((string)(base.GetValue(InvokeCustomFunctionActivity.FunctionDefinitionProperty)));
            }
            set
            {
                base.SetValue(InvokeCustomFunctionActivity.FunctionDefinitionProperty, value);
            }
        }

        [
            CategoryAttribute("Activity"),
            DefaultValueAttribute(InvokeMethod.Synchronous),
            DescriptionAttribute("Specify the invoke method."),
        ]
        public InvokeMethod InvokeMethod
        {
            get
            {
                return ((InvokeMethod)(base.GetValue(InvokeCustomFunctionActivity.InvokeMethodProperty)));
            }
            set
            {
                base.SetValue(InvokeCustomFunctionActivity.InvokeMethodProperty, value);
            }
        }

        // This is a property that returns the the value as result of custom function execution at runtime.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ReturnValue
        {
            get
            {
                return base.GetValue(ReturnValueProperty) as string;
            }
            set
            {
                base.SetValue(ReturnValueProperty, value);
            }
        }

        #endregion

        #region Activity Execution Logic

        private delegate string ExecuteCustomFunctionDelegate(IInstanceWrapper instance);

        /// <summary>
        /// Execute the activity
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            IPrincipal originalPrinciple = Thread.CurrentPrincipal;
            try
            {
                ITaskService taskService = context.GetService<ITaskService>();
                Thread.CurrentPrincipal = taskService.SuperUser; // run the custom function using super user rights

                // invoke the given workflow in the same project
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);

                if (this.InvokeMethod == InvokeMethod.Synchronous)
                {
                    ICustomFunction function = CreateCustomFunction(this.FunctionDefinition);
                    if (function != null)
                    {
                        // save the return value in the property
                        ReturnValue = function.Execute(rootActivity.Instance);
                    }
                }
                else
                {
                    // asynchronous call, do not block on custom function call
                    ICustomFunction function = CreateCustomFunction(this.FunctionDefinition);

                    ExecuteCustomFunctionDelegate funcCall = new ExecuteCustomFunctionDelegate(function.Execute);
                    // TODO, save the return value in the property in callback method
                    funcCall.BeginInvoke(rootActivity.Instance, null, null);
                }

                return ActivityExecutionStatus.Closed;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("Invoke custom function " + this.FunctionDefinition + " failed with error " + ex.Message + "\n" + ex.StackTrace);
                //throw ex;
                return ActivityExecutionStatus.Closed;
            }
            finally
            {
                Thread.CurrentPrincipal = originalPrinciple;
            }
        }

        /// <summary>
        /// Create an instance of the ICustomFunction as specified.
        /// </summary>
        /// <param name="functionDefinition">The definition of the custom function</param>
        /// <returns>An instance of the ICustomFunction, null if failed to create the instance.</returns>
        private ICustomFunction CreateCustomFunction(string functionDefinition)
        {
            ICustomFunction function = null;

            if (!string.IsNullOrEmpty(functionDefinition))
            {
                int index = functionDefinition.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = functionDefinition.Substring(0, index).Trim();
                    assemblyName = functionDefinition.Substring(index + 1).Trim();
                }
                else
                {
                    className = functionDefinition.Trim();
                }

                try
                {

                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    function = (ICustomFunction)obj.Unwrap();
                }
                catch
                {
                    function = null;
                }
            }

            return function;
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

        #region InvokeCustomFunctionValidator

        /// <summary>
        /// Custom validator for InvokeCustomFunctionActivity
        /// </summary>
        public class InvokeCustomFunctionValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager, object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                InvokeCustomFunctionActivity activity = obj as InvokeCustomFunctionActivity;
                if (activity != null)
                {
                    // Validate the FunctionDefinition Properties
                    this.ValidateFunctionProperties(validationErrors, activity);
                }
                return validationErrors;
            }

            private void ValidateFunctionProperties(ValidationErrorCollection validationErrors, InvokeCustomFunctionActivity activity)
            {
                // Validate the FunctionDefinition property
                if (String.IsNullOrEmpty(activity.FunctionDefinition))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(InvokeCustomFunctionActivity.FunctionDefinitionProperty.Name));

                }
                else if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidCustomFunction(activity.FunctionDefinition))
                {
                    validationErrors.Add(new ValidationError("The custom funcion definition is incorrect or the library doesn't exist.", UnknownAssignment));
                }
            }
        }

        #endregion InvokeCustomFunctionValidator
    }
}
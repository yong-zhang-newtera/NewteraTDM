/*
* @(#)CloseTaskActivity.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

using Newtera.Common.Core;
using Newtera.WorkflowServices;
using Newtera.Common.MetaData;

namespace Newtera.Activities
{
    [ActivityValidator(typeof(CloseTaskValidator))]
    [DefaultProperty("ActivityName")]
    public partial class CloseTaskActivity : System.Workflow.ComponentModel.Activity, ICustomTypeDescriptor
    {
        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;
  
        // Define the DependencyProperty objects for all of the Properties 
        // ...and Events exposed by this activity
        public static DependencyProperty ActivityNameProperty = DependencyProperty.Register("ActivityName", typeof(string), typeof(CloseTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty WriteLogProperty = DependencyProperty.Register("WriteLog", typeof(bool), typeof(CloseTaskActivity), new PropertyMetadata(false, DependencyPropertyOptions.Metadata));

        // Define constant values for the Property categories.  
        private const string ActivityPropertiesCategory = "Task";
        private const string LogPropertiesCategory = "Log";

        // Define private constants for the Validation Errors 
        private const int InvalidActivityName = 1;

        public CloseTaskActivity()
        {
        }

        #region public Properties

        [BrowsableAttribute(true)]
        [DescriptionAttribute("The ActivityName property is used to specify the name of the CreateTaskActivity that created the task.")]
        [CategoryAttribute(ActivityPropertiesCategory)]
        [DefaultValueAttribute(null)]
        [EditorAttribute("WorkflowStudio.TaskNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public string ActivityName
        {
            get
            {
                return ((string)(base.GetValue(CloseTaskActivity.ActivityNameProperty)));
            }
            set
            {
                base.SetValue(CloseTaskActivity.ActivityNameProperty, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("The WriteLog property is used to specify whether to update a log entry when a task is closed")]
        [Category(LogPropertiesCategory)]
        [BrowsableAttribute(true)]
        [DefaultValue(false)]
        public bool WriteLog
        {
            get
            {
                return ((bool)(base.GetValue(CloseTaskActivity.WriteLogProperty)));
            }
            set
            {
                base.SetValue(CloseTaskActivity.WriteLogProperty, value);
            }
        }

        #endregion

        #region Activity Execution Logic

        /// During execution the CreateTask activity should create a task for each of the
        /// specified users in the database and send the email using SMTP if required.  
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            try
            {
                // Close the task
                string taskId = CloseTask(context);

                if (this.WriteLog)
                {
                    // update the log before closeTask remove the task info
                    UpdateTaskLog(context, taskId);
                }

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        private string CloseTask(ActivityExecutionContext context)
        {
            ITaskService taskService = context.GetService<ITaskService>();
            return taskService.CloseTask(this.ActivityName);
        }

        private void UpdateTaskLog(ActivityExecutionContext context, string taskId)
        {
            ITaskService taskService = context.GetService<ITaskService>();
            taskService.UpdateTaskLog(this.ActivityName, taskId);
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

        #region CloseTaskValidator

        public class CloseTaskValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager,object obj)
            {

                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                CloseTaskActivity activity = obj as CloseTaskActivity;
                if (activity != null)
                {
                    // Validate the ActivityName property
                    if (String.IsNullOrEmpty(activity.ActivityName))
                    {
                        validationErrors.Add(ValidationError.GetNotSetValidationError(CloseTaskActivity.ActivityNameProperty.Name));

                    }
                    else if (!IsValidActivityName(activity))
                    {
                        validationErrors.Add(new ValidationError(activity.ActivityName + " is not a valid CreateTaskActivity name.", InvalidActivityName));
                    }
                }

                return validationErrors;
            }

            private bool IsValidActivityName(CloseTaskActivity activity)
            {
                Activity temp = activity.GetActivityByName(activity.ActivityName);
                if (temp != null && temp is CreateTaskActivity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
         }

        #endregion
    }
}

/*
* @(#)CreateGroupTaskActivity.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Collections;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Security;

using Newtera.Common.Core;
using Newtera.WorkflowServices;
using Newtera.Common.MetaData;
using Newtera.WFModel;
using Newtera.Common.MetaData.Principal;

namespace Newtera.Activities
{
    /// <summary>
    /// Create a same task for a group of users, usually used in an approval process where it requires multiple people to approve or
    /// disapprove a task
    /// </summary>
    /// 
    [ActivityValidator(typeof(CreateGroupTaskValidator))]
    [ToolboxBitmap(typeof(CreateGroupTaskActivity), "Resources.EmailMessage.png")]
    [DefaultProperty("Subject")]
    public partial class CreateGroupTaskActivity : System.Workflow.ComponentModel.Activity, ICustomTypeDescriptor
    {
        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        // Define the DependencyProperty objects for all of the Properties 
        // ...and Events exposed by this activity
        public static DependencyProperty SubjectProperty = DependencyProperty.Register("Subject", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata("New Task", DependencyPropertyOptions.Metadata));
        public static DependencyProperty TaskDescriptionProperty = DependencyProperty.Register("TaskDescription", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty InstructionProperty = DependencyProperty.Register("Instruction", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty CustomActionsProperty = DependencyProperty.Register("CustomActions", typeof(IList), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty FormTypeProperty = DependencyProperty.Register("FormType", typeof(FormTypeEnum), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty CustomFormUrlProperty = DependencyProperty.Register("CustomFormUrl", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty FormPropertiesProperty = DependencyProperty.Register("FormProperties", typeof(StringCollection), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty UsersProperty = DependencyProperty.Register("Users", typeof(StringCollection), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty UsersBindingAttributeProperty = DependencyProperty.Register("UsersBindingAttribute", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty UsersBindingAttributeCaptionProperty = DependencyProperty.Register("UsersBindingAttributeCaption", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
               
        public static DependencyProperty SendEmailProperty = DependencyProperty.Register("SendEmail", typeof(bool), typeof(CreateGroupTaskActivity), new PropertyMetadata(false, DependencyPropertyOptions.Metadata));

        // Task execution log properties
        public static DependencyProperty WriteLogProperty = DependencyProperty.Register("WriteLog", typeof(bool), typeof(CreateGroupTaskActivity), new PropertyMetadata(false, DependencyPropertyOptions.Metadata));
        public static DependencyProperty BindingInstanceKeyProperty = DependencyProperty.Register("BindingInstanceKey", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty BindingInstanceDescProperty = DependencyProperty.Register("BindingInstanceDesc", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty ExpectedFinishTimeProperty = DependencyProperty.Register("ExpectedFinishTime", typeof(string), typeof(CreateGroupTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        // runtime used properties
        public static DependencyProperty TaskIdsProperty = DependencyProperty.Register("TaskIds", typeof(StringCollection), typeof(CreateGroupTaskActivity));

        // Define constant values for the Property categories.  
        private const string TaskPropertiesCategory = "Task";
        private const string DataBindingPropertiesCategory = "DataInstanceBinding";
        private const string StaticAssignmentPropertiesCategory = "StaticAssignment";
        private const string DynamicAssignmentPropertiesCategory = "DynamicAssignment";
        private const string NoticePropertiesCategory = "Notice";
        private const string FormPropertiesCategory = "Form";
        private const string LogPropertiesCategory = "Log";

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;
        private const int InvalidAssignment = 2;
        
        public CreateGroupTaskActivity()
        {
        }

        #region public Properties

        //[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The Subject property is used to specify the subject of the task.")]
        [CategoryAttribute(TaskPropertiesCategory)]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string Subject
        {
            get
            {
                return ((string)(base.GetValue(CreateGroupTaskActivity.SubjectProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.SubjectProperty, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The description property is used to specify the description of the task. This is not a required property")]
        [CategoryAttribute(TaskPropertiesCategory)]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string TaskDescription
        {

            get
            {
                return (string)base.GetValue(CreateGroupTaskActivity.TaskDescriptionProperty);
            }
            set
            {

                base.SetValue(CreateGroupTaskActivity.TaskDescriptionProperty, value);
            }

        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The instruction property is used to specify the instruction of the task. This is not a required property")]
        [CategoryAttribute(TaskPropertiesCategory)]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string Instruction
        {

            get
            {
                return (string)base.GetValue(CreateGroupTaskActivity.InstructionProperty);
            }
            set
            {

                base.SetValue(CreateGroupTaskActivity.InstructionProperty, value);
            }

        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The CustomActions property is used to specify the custom actions available for the task. This is not a required property")]
        [CategoryAttribute(TaskPropertiesCategory)]
        [EditorAttribute("WorkflowStudio.CustomActionsPropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public IList CustomActions
        {
            get
            {
                return ((IList)(base.GetValue(CreateGroupTaskActivity.CustomActionsProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.CustomActionsProperty, value);
            }
        }

        [CategoryAttribute(FormPropertiesCategory)]
        [BrowsableAttribute(true)]
        [DefaultValueAttribute(FormTypeEnum.Auto)]
        [DescriptionAttribute("Specify whether the form is auto-genrated or custom one.")]
        public FormTypeEnum FormType
        {
            get
            {
                return ((FormTypeEnum)(base.GetValue(CreateGroupTaskActivity.FormTypeProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.FormTypeProperty, value);
            }
        }

        [CategoryAttribute(FormPropertiesCategory)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("Specify url of a custom form when the form type is Custom.")]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string CustomFormUrl
        {
            get
            {
                return ((string)(base.GetValue(CreateGroupTaskActivity.CustomFormUrlProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.CustomFormUrlProperty, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The properties to display in the auto-generated form if specified, otherwise all properties of the bound class are displayed.")]
        [CategoryAttribute(FormPropertiesCategory)]
        [EditorAttribute("WorkflowStudio.FormPropertiesPropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public StringCollection FormProperties
        {

            get
            {
                return (StringCollection)base.GetValue(CreateGroupTaskActivity.FormPropertiesProperty);
            }
            set
            {

                base.SetValue(CreateGroupTaskActivity.FormPropertiesProperty, value);
            }

        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The users property is used to specify users who are assigned to the task.")]
        [CategoryAttribute(StaticAssignmentPropertiesCategory)]
        [EditorAttribute("WorkflowStudio.UsersPropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public StringCollection Users
        {

            get
            {
                return (StringCollection)base.GetValue(CreateGroupTaskActivity.UsersProperty);
            }
            set
            {

                base.SetValue(CreateGroupTaskActivity.UsersProperty, value);
            }

        }


        [CategoryAttribute(DynamicAssignmentPropertiesCategory)]
        [DescriptionAttribute("Specify an array attribute whose value represents the users who are assigned to the task.")]
        [DefaultValueAttribute(null)]
        [TypeConverterAttribute("WorkflowStudio.ArrayAttributeNameConverter, WorkflowStudio")]
        [EditorAttribute("WorkflowStudio.AttributeNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public string UsersBindingAttribute
        {
            get
            {
                return ((string)(base.GetValue(CreateGroupTaskActivity.UsersBindingAttributeProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.UsersBindingAttributeProperty, value);
            }
        }

        [Browsable(false)]
        public string UsersBindingAttributeCaption
        {
            get
            {
                return ((string)(base.GetValue(CreateGroupTaskActivity.UsersBindingAttributeCaptionProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.UsersBindingAttributeCaptionProperty, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("The SendEmail property is used to specify whether an email is sent to the assigned users")]
        [Category(NoticePropertiesCategory)]
        [BrowsableAttribute(true)]
        [DefaultValue(false)]
        public bool SendEmail
        {
            get
            {
                return ((bool)(base.GetValue(CreateGroupTaskActivity.SendEmailProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.SendEmailProperty, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("The WriteLog property is used to specify whether to create a log entry when a task is created")]
        [Category(LogPropertiesCategory)]
        [BrowsableAttribute(true)]
        [DefaultValue(false)]
        public bool WriteLog
        {
            get
            {
                return ((bool)(base.GetValue(CreateGroupTaskActivity.WriteLogProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.WriteLogProperty, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The BindingInstanceKey property is used to specify the primay key of the binding data instance to the worklfow instance to be logged.")]
        [CategoryAttribute(LogPropertiesCategory)]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string BindingInstanceKey
        {

            get
            {
                return (string)base.GetValue(CreateGroupTaskActivity.BindingInstanceKeyProperty);
            }
            set
            {

                base.SetValue(CreateGroupTaskActivity.BindingInstanceKeyProperty, value);
            }

        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The BindingInstanceDesc property is used to specify the descriptive info of the binding data instance to the worklfow instance to be logged.")]
        [CategoryAttribute(LogPropertiesCategory)]
        [EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))]
        public string BindingInstanceDesc
        {

            get
            {
                return (string)base.GetValue(CreateGroupTaskActivity.BindingInstanceDescProperty);
            }
            set
            {

                base.SetValue(CreateGroupTaskActivity.BindingInstanceDescProperty, value);
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The ExpectedFinishTimeProperty property is used to specify the descriptive info of the binding data instance to the worklfow instance to be logged.")]
        [CategoryAttribute(LogPropertiesCategory)]
        public string ExpectedFinishTime
        {

            get
            {
                return (string)base.GetValue(CreateGroupTaskActivity.ExpectedFinishTimeProperty);
            }
            set
            {

                base.SetValue(CreateGroupTaskActivity.ExpectedFinishTimeProperty, value);
            }
        }

        [Description("The collection of ids of tasks created by the activity")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        public StringCollection TaskIds
        {
            get
            {
                return ((StringCollection)(base.GetValue(CreateGroupTaskActivity.TaskIdsProperty)));
            }
            set
            {
                base.SetValue(CreateGroupTaskActivity.TaskIdsProperty, value);
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
                StringCollection users = this.GetCombinedUsers(context);

                // create a task for each of specified users
                StringCollection taskIds = CreateTasks(context, users);

                this.TaskIds = taskIds;

                if (this.SendEmail)
                {
                    // Send the email to assigned users now
                    this.SendEmailToUsers(context, users);
                }

                if (this.WriteLog)
                {
                    // write a log entry for the task
                    this.WriteTaskLog(context, users);
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

        private StringCollection CreateTasks(ActivityExecutionContext context, StringCollection users)
        {
            ITaskService taskService = context.GetService<ITaskService>();
            INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);

            string customActionsXml = null;
            if (this.CustomActions != null && this.CustomActions.Count > 0)
            {
                customActionsXml = GetCustomActionXml((CustomActionCollection)this.CustomActions);
            }

            string url = null;
            if (this.FormType == FormTypeEnum.Custom)
            {
                url = this.CustomFormUrl;
            }

            StringCollection taskIds = new StringCollection();
            StringCollection singleUser;
            foreach (string user in users)
            {
                // create a task for each user in the collection
                singleUser = new StringCollection();
                singleUser.Add(user);

                string taskId = taskService.CreateTask(this.Subject, this.TaskDescription, this.Instruction, url, this.FormProperties, singleUser,
                    null, this.QualifiedName, rootActivity.SchemaId, rootActivity.ClassName, customActionsXml, true);
                taskIds.Add(taskId);
            }

            return taskIds;
        }

        private void SendEmailToUsers(ActivityExecutionContext context, StringCollection users)
        {
            StringCollection emailAddresses = new StringCollection();

            IUserManager userManager = context.GetService<IUserManager>();
            ITaskService taskService = context.GetService<ITaskService>();

            StringCollection qualifiedUsers = taskService.GetQualifiedUsers(users, null, userManager);
            
            string[] usrEmails;
            foreach (string user in qualifiedUsers)
            {
                usrEmails = userManager.GetUserEmails(user);
                for (int i = 0; i < usrEmails.Length; i++)
                {
                    if (usrEmails[i].Length > 0)
                    {
                        AddEmailAddress(emailAddresses, usrEmails[i]);
                    }
                }
            }

            taskService.SendNotice(this.Subject, this.TaskDescription, emailAddresses, qualifiedUsers);
        }

        private void WriteTaskLog(ActivityExecutionContext context, StringCollection users)
        {
            IUserManager userManager = context.GetService<IUserManager>();
            ITaskService taskService = context.GetService<ITaskService>();

            StringBuilder builder = new StringBuilder();
            if (users != null)
            {
                string displayText;
                string[] userData;
                foreach (string user in users)
                {
                    userManager.GetUserData(user);
                    userData = userManager.GetUserData(user);
                    displayText = GetUserDisplayText(user, userData);
                    if (builder.Length > 0)
                    {
                        builder.Append(";").Append(displayText);
                    }
                    else
                    {
                        builder.Append(displayText);
                    }
                }
            }

            taskService.WriteTaskLog(this.BindingInstanceKey, this.BindingInstanceDesc, this.ExpectedFinishTime, builder.ToString(),
              this.QualifiedName, null);
        }

        private string GetUserDisplayText(string user, string[] userData)
        {
            string displayText;
            if (userData == null || string.IsNullOrEmpty(userData[0]) &&
                string.IsNullOrEmpty(userData[1]))
            {
                displayText = user;
            }
            else
            {
                displayText = UsersListHandler.GetFormatedName(userData[0], userData[1]);
            }

            return displayText;
        }

        // add email address without duplication
        private void AddEmailAddress(StringCollection emails, string email)
        {
            bool status = false;

            foreach (string addr in emails)
            {
                if (addr == email)
                {
                    status = true;
                    break;
                }
            }

            if (!status)
            {
                emails.Add(email);
            }
        }

        /// <summary>
        /// Gets user names combined from static and dynamic assignment properties
        /// </summary>
        /// <returns></returns>
        private StringCollection GetCombinedUsers(ActivityExecutionContext context)
        {
            StringCollection users = new StringCollection();

            // get static assigned users
            if (this.Users != null)
            {
                foreach (string user in this.Users)
                {
                    users.Add(user);
                }
            }

            // get dynamic assigned users
            if (this.UsersBindingAttribute != null &&
                this.UsersBindingAttribute.Length > 0)
            {
                ITaskService taskService = context.GetService<ITaskService>();
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);
                StringCollection dynamicUsers = taskService.GetBindingUsers(rootActivity.SchemaId,
                    rootActivity.ClassName, this.UsersBindingAttribute);
                if (dynamicUsers != null)
                {
                    foreach (string user in dynamicUsers)
                    {
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        // convert a collection of custom actions to xml
        private string GetCustomActionXml(CustomActionCollection customActions)
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            customActions.Write(writer);
            
            // esacpe the xml characters since it will be inserted into a xquery
            return SecurityElement.Escape(builder.ToString());
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

        #region CreateGroupTaskValidator

        public class CreateGroupTaskValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager,object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                CreateGroupTaskActivity activity = obj as CreateGroupTaskActivity;
                if (activity != null)
                {
                    // Validate the task and assignment Properties
                    this.ValidateTaskProperties(validationErrors, activity);
                    this.ValidateAssignmentProperties(validationErrors, activity);
                    this.ValidateFormProperties(validationErrors, activity);
                    this.ValidateLogProperties(validationErrors, activity);
                }
                return validationErrors;
            }
       
            private void ValidateTaskProperties(ValidationErrorCollection validationErrors, CreateGroupTaskActivity activity)
            {
                // Validate the Subject property
                if (String.IsNullOrEmpty(activity.Subject))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(CreateGroupTaskActivity.SubjectProperty.Name));

                }
                else if (activity.Subject.Length > 100)
                {
                    validationErrors.Add(new ValidationError("The length of Subject has to be less than 100", InvalidAssignment));
                }

                if (!string.IsNullOrEmpty(activity.Description) && activity.Description.Length > 500)
                {
                    validationErrors.Add(new ValidationError("The length of Description has to be less than 500", InvalidAssignment));
                }

                if (!string.IsNullOrEmpty(activity.Instruction) && activity.Instruction.Length > 500)
                {
                    validationErrors.Add(new ValidationError("The length of Instruction has to be less than 500", InvalidAssignment));
                }
            }

            private void ValidateFormProperties(ValidationErrorCollection validationErrors, CreateGroupTaskActivity activity)
            {
                // Validate the Subject property
                if (activity.FormType == FormTypeEnum.Custom &&
                    string.IsNullOrEmpty(activity.CustomFormUrl))
                {
                    validationErrors.Add(new ValidationError("The Custom Form Url has to be specified when FormType is set to Custom", UnknownAssignment));
                }
            }

            private void ValidateAssignmentProperties(ValidationErrorCollection validationErrors, CreateGroupTaskActivity activity)
            {
                // Validate the Users or roles property
                if ((activity.Users == null || activity.Users.Count == 0) &&
                    (activity.UsersBindingAttribute == null || activity.UsersBindingAttribute.Length == 0))
                {
                    validationErrors.Add(new ValidationError("The task has not been assigned to any users.", UnknownAssignment));
                }

                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                // validate if the user binding attribute name is valid
                if (activity.UsersBindingAttribute != null && activity.UsersBindingAttribute.Length > 0 &&
                    !ActivityValidatingServiceProvider.Instance.ValidateService.IsValidAttributeName(rootActivity.SchemaId, rootActivity.ClassName, activity.UsersBindingAttribute))
                {
                    validationErrors.Add(new ValidationError(activity.UsersBindingAttribute + " attribute doesn't exist in the class " + rootActivity.ClassName + " of database schema " + rootActivity.SchemaId, UnknownAssignment));
                }

                // validate if static assigned users are still existing
                if (activity.Users != null && activity.Users.Count > 0)
                {
                    foreach (string user in activity.Users)
                    {
                        if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidUser(user))
                        {
                            validationErrors.Add(new ValidationError(user + " user doesn't exist any more", UnknownAssignment));
                        }
                    }
                }
            }

            private void ValidateLogProperties(ValidationErrorCollection validationErrors, CreateGroupTaskActivity activity)
            {
                if (activity.WriteLog)
                {
                    // Validate the BindingInstanceKey property
                    if (String.IsNullOrEmpty(activity.BindingInstanceKey))
                    {
                        validationErrors.Add(ValidationError.GetNotSetValidationError(CreateGroupTaskActivity.BindingInstanceKeyProperty.Name));
                    }
                }
            }
         }

        #endregion
    }
}

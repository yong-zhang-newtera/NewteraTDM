/*
* @(#)CreateTaskActivity.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
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
    #region FormTypeEnum

    public enum FormTypeEnum
    {
        // Auto generated form
        Auto,
        // Custom made form
        Custom
    }

    #endregion FormTypeEnum

    [ActivityValidator(typeof(CreateTaskValidator))]
    [ToolboxBitmap(typeof(CreateTaskActivity), "Resources.EmailMessage.png")]
    [DefaultProperty("Subject")]
    public partial class CreateTaskActivity : System.Workflow.ComponentModel.Activity, ICustomTypeDescriptor
    {
        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        // Define the DependencyProperty objects for all of the Properties 
        // ...and Events exposed by this activity
        public static DependencyProperty SubjectProperty = DependencyProperty.Register("Subject", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata("New Task", DependencyPropertyOptions.Metadata));
        public static DependencyProperty TaskDescriptionProperty = DependencyProperty.Register("TaskDescription", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty InstructionProperty = DependencyProperty.Register("Instruction", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty CustomActionsProperty = DependencyProperty.Register("CustomActions", typeof(IList), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty FormTypeProperty = DependencyProperty.Register("FormType", typeof(FormTypeEnum), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty CustomFormUrlProperty = DependencyProperty.Register("CustomFormUrl", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty FormPropertiesProperty = DependencyProperty.Register("FormProperties", typeof(StringCollection), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty UsersProperty = DependencyProperty.Register("Users", typeof(StringCollection), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty RolesProperty = DependencyProperty.Register("Roles", typeof(StringCollection), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        public static DependencyProperty UsersBindingAttributeProperty = DependencyProperty.Register("UsersBindingAttribute", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty UsersBindingAttributeCaptionProperty = DependencyProperty.Register("UsersBindingAttributeCaption", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        
        public static DependencyProperty RolesBindingAttributeProperty = DependencyProperty.Register("RolesBindingAttribute", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty RolesBindingAttributeCaptionProperty = DependencyProperty.Register("RolesBindingAttributeCaption", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
       
        public static DependencyProperty SendEmailProperty = DependencyProperty.Register("SendEmail", typeof(bool), typeof(CreateTaskActivity), new PropertyMetadata(false, DependencyPropertyOptions.Metadata));

        // Task execution log properties
        public static DependencyProperty WriteLogProperty = DependencyProperty.Register("WriteLog", typeof(bool), typeof(CreateTaskActivity), new PropertyMetadata(false, DependencyPropertyOptions.Metadata));
        public static DependencyProperty BindingInstanceKeyProperty = DependencyProperty.Register("BindingInstanceKey", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty BindingInstanceDescProperty = DependencyProperty.Register("BindingInstanceDesc", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));
        public static DependencyProperty ExpectedFinishTimeProperty = DependencyProperty.Register("ExpectedFinishTime", typeof(string), typeof(CreateTaskActivity), new PropertyMetadata(DependencyPropertyOptions.Metadata));

        // Task Display Properties
        public static DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(CreateTaskActivity), new PropertyMetadata(true, DependencyPropertyOptions.Metadata));

        // Define constant values for the Property categories.  
        private const string TaskPropertiesCategory = "Task";
        private const string DataBindingPropertiesCategory = "DataInstanceBinding";
        private const string StaticAssignmentPropertiesCategory = "StaticAssignment";
        private const string DynamicAssignmentPropertiesCategory = "DynamicAssignment";
        private const string NoticePropertiesCategory = "Notice";
        private const string FormPropertiesCategory = "Form";
        private const string LogPropertiesCategory = "Log";
        private const string DisplayPropertiesCategory = "Display";

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;
        private const int InvalidAssignment = 2;

        //private string _taskObjId = null; // NOTE: Newver change the member names, otheriwse, it will break the workflow instances that are running, you can delete members, but not add members
        
        public CreateTaskActivity()
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
                return ((string)(base.GetValue(CreateTaskActivity.SubjectProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.SubjectProperty, value);
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
                return (string)base.GetValue(CreateTaskActivity.TaskDescriptionProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.TaskDescriptionProperty, value);
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
                return (string)base.GetValue(CreateTaskActivity.InstructionProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.InstructionProperty, value);
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
                return ((IList)(base.GetValue(CreateTaskActivity.CustomActionsProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.CustomActionsProperty, value);
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
                return ((FormTypeEnum)(base.GetValue(CreateTaskActivity.FormTypeProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.FormTypeProperty, value);
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
                return ((string)(base.GetValue(CreateTaskActivity.CustomFormUrlProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.CustomFormUrlProperty, value);
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
                return (StringCollection)base.GetValue(CreateTaskActivity.FormPropertiesProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.FormPropertiesProperty, value);
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
                return (StringCollection)base.GetValue(CreateTaskActivity.UsersProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.UsersProperty, value);
            }

        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [DescriptionAttribute("The roles property is used to specify roles of the users who are assigned of the task. Only those users who belong to all the specified roles will be assigned.")]
        [CategoryAttribute(StaticAssignmentPropertiesCategory)]
        [EditorAttribute("WorkflowStudio.RolesPropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public StringCollection Roles
        {

            get
            {
                return (StringCollection)base.GetValue(CreateTaskActivity.RolesProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.RolesProperty, value);
            }
        }

        [CategoryAttribute(DynamicAssignmentPropertiesCategory)]
        [DescriptionAttribute("Specify an attribute whose value represents the users who are assigned to the task.")]
        [DefaultValueAttribute(null)]
        [TypeConverterAttribute("WorkflowStudio.AttributeNameConverter, WorkflowStudio")]
        [EditorAttribute("WorkflowStudio.AttributeNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public string UsersBindingAttribute
        {
            get
            {
                return ((string)(base.GetValue(CreateTaskActivity.UsersBindingAttributeProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.UsersBindingAttributeProperty, value);
            }
        }

        [Browsable(false)]
        public string UsersBindingAttributeCaption
        {
            get
            {
                return ((string)(base.GetValue(CreateTaskActivity.UsersBindingAttributeCaptionProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.UsersBindingAttributeCaptionProperty, value);
            }
        }

        [CategoryAttribute(DynamicAssignmentPropertiesCategory)]
        [DescriptionAttribute("Specify an attribute whose value represents the roles who are assigned to the task.")]
        [DefaultValueAttribute(null)]
        [TypeConverterAttribute("WorkflowStudio.AttributeNameConverter, WorkflowStudio")]
        [EditorAttribute("WorkflowStudio.AttributeNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor))]
        public string RolesBindingAttribute
        {
            get
            {
                return ((string)(base.GetValue(CreateTaskActivity.RolesBindingAttributeProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.RolesBindingAttributeProperty, value);
            }
        }

        [Browsable(false)]
        public string RolesBindingAttributeCaption
        {
            get
            {
                return ((string)(base.GetValue(CreateTaskActivity.RolesBindingAttributeCaptionProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.RolesBindingAttributeCaptionProperty, value);
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
                return ((bool)(base.GetValue(CreateTaskActivity.SendEmailProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.SendEmailProperty, value);
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
                return ((bool)(base.GetValue(CreateTaskActivity.WriteLogProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.WriteLogProperty, value);
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
                return (string)base.GetValue(CreateTaskActivity.BindingInstanceKeyProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.BindingInstanceKeyProperty, value);
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
                return (string)base.GetValue(CreateTaskActivity.BindingInstanceDescProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.BindingInstanceDescProperty, value);
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
                return (string)base.GetValue(CreateTaskActivity.ExpectedFinishTimeProperty);
            }
            set
            {

                base.SetValue(CreateTaskActivity.ExpectedFinishTimeProperty, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("The Visible property is used to specify whether the task display in the my task list")]
        [Category(DisplayPropertiesCategory)]
        [BrowsableAttribute(true)]
        [DefaultValue(true)]
        public bool Visible
        {
            get
            {
                return ((bool)(base.GetValue(CreateTaskActivity.VisibleProperty)));
            }
            set
            {
                base.SetValue(CreateTaskActivity.VisibleProperty, value);
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
                StringCollection roles = this.GetCombinedRoles(context);

                // create a task for each of specified users
                string taskjId = CreateTask(context, users, roles);

                // notify assigned users
                this.NotifyUsers(context, taskjId, users, roles);

                if (this.WriteLog)
                {
                    // write a log entry for the task
                    this.WriteTaskLog(context, users, roles);
                }

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return ActivityExecutionStatus.Closed; // exception may be caused by sending email, close the activity to prevent it stuck the workflow instance
                //throw ex;
            }
        }

        private string CreateTask(ActivityExecutionContext context, StringCollection users, StringCollection roles)
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

            return taskService.CreateTask(this.Subject, this.TaskDescription, this.Instruction, url, this.FormProperties, users,
                roles, this.QualifiedName, rootActivity.SchemaId, rootActivity.ClassName, customActionsXml, this.Visible);
        }

        private void NotifyUsers(ActivityExecutionContext context, string taskId, StringCollection users, StringCollection roles)
        {
            try
            {
                StringCollection emailAddresses = new StringCollection();

                ITaskService taskService = context.GetService<ITaskService>();

                //StringCollection taskReceivers = taskService.GetQualifiedUsers(users, roles, userManager);
                StringCollection taskReceivers = taskService.GetTaskReceivers(taskId, users, roles, WorkflowInstanceId.ToString());

                if (this.SendEmail)
                {
                    IUserManager userManager = context.GetService<IUserManager>();

                    string[] usrEmails;
                    foreach (string user in taskReceivers)
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
                }
 
                taskService.SendNotice(this.Subject, this.TaskDescription, emailAddresses, taskReceivers);
            }
            catch (Exception ex)
            {
                // we don't want to throw an exception for failing to send emails
                ErrorLog.Instance.WriteLine("Failed to send emails due to " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void WriteTaskLog(ActivityExecutionContext context, StringCollection users, 
            StringCollection roles)
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

            if (roles != null)
            {
                foreach (string role in roles)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(";").Append(role);
                    }
                    else
                    {
                        builder.Append(role);
                    }
                }
            }

            taskService.WriteTaskLog(this.BindingInstanceKey, this.BindingInstanceDesc, this.ExpectedFinishTime, builder.ToString(),
              this.QualifiedName, null);
        }

        private string GetUserDisplayText(string user, string[] userData)
        {
            string displayText;
            if (userData == null ||
                string.IsNullOrEmpty(userData[0]) &&
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

        /// <summary>
        /// Gets role names combined from static and dynamic assignment properties
        /// </summary>
        /// <returns></returns>
        private StringCollection GetCombinedRoles(ActivityExecutionContext context)
        {
            StringCollection roles = new StringCollection();

            // get static assigned roles
            if (this.Roles != null)
            {
                foreach (string role in this.Roles)
                {
                    roles.Add(role);
                }
            }

            // get dynamic assigned roles
            if (this.RolesBindingAttribute != null &&
                this.RolesBindingAttribute.Length > 0)
            {
                ITaskService taskService = context.GetService<ITaskService>();
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);
                StringCollection dynamicRoles = taskService.GetBindingRoles(rootActivity.SchemaId,
                    rootActivity.ClassName, this.RolesBindingAttribute);
                if (dynamicRoles != null)
                {
                    IUserManager userManager = context.GetService<IUserManager>();
                    foreach (string role in dynamicRoles)
                    {
                        // convert from role display name to role name
                        //userManager.G
                        roles.Add(role);
                    }
                }
            }

            return roles;
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

        #region CreateTaskValidator

        public class CreateTaskValidator : System.Workflow.ComponentModel.Compiler.ActivityValidator
        {
            //customizing the default activity validation
            public override ValidationErrorCollection ValidateProperties(ValidationManager manager,object obj)
            {
                // Create a new collection for storing the validation errors
                ValidationErrorCollection validationErrors = base.ValidateProperties(manager, obj);

                CreateTaskActivity activity = obj as CreateTaskActivity;
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
       
            private void ValidateTaskProperties(ValidationErrorCollection validationErrors, CreateTaskActivity activity)
            {
                // Validate the Subject property
                if (String.IsNullOrEmpty(activity.Subject))
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(CreateTaskActivity.SubjectProperty.Name));

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

            private void ValidateFormProperties(ValidationErrorCollection validationErrors, CreateTaskActivity activity)
            {
                // Validate the Subject property
                if (activity.FormType == FormTypeEnum.Custom &&
                    string.IsNullOrEmpty(activity.CustomFormUrl))
                {
                    validationErrors.Add(new ValidationError("The Custom Form Url has to be specified when FormType is set to Custom", UnknownAssignment));
                }
            }

            private void ValidateAssignmentProperties(ValidationErrorCollection validationErrors, CreateTaskActivity activity)
            {
                // Validate the Users or roles property
                if ((activity.Users == null || activity.Users.Count == 0) &&
                    (activity.Roles == null || activity.Roles.Count == 0) &&
                    (activity.UsersBindingAttribute == null || activity.UsersBindingAttribute.Length == 0) &&
                    (activity.RolesBindingAttribute == null || activity.RolesBindingAttribute.Length == 0))
                {
                    validationErrors.Add(new ValidationError("The task has not been assigned to any users or roles.", UnknownAssignment));
                }

                // validate that static roles and dynamic roles are mutually exclusive
                /* comment out to allow combination of static roles and dynamic roles to form a and-roles
                if (activity.Roles != null && activity.Roles.Count > 0 &&
                    activity.RolesBindingAttribute != null && activity.RolesBindingAttribute.Length > 0)
                {
                    validationErrors.Add(new ValidationError("The task cannot be assigned to static roles and dynamical roles at the same time.", InvalidAssignment));
                }
                */

                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                // validate if the user binding attribute name is valid
                if (activity.UsersBindingAttribute != null && activity.UsersBindingAttribute.Length > 0 &&
                    !ActivityValidatingServiceProvider.Instance.ValidateService.IsValidAttributeName(rootActivity.SchemaId, rootActivity.ClassName, activity.UsersBindingAttribute))
                {
                    validationErrors.Add(new ValidationError(activity.UsersBindingAttribute + " attribute doesn't exist in the class " + rootActivity.ClassName + " of database schema " + rootActivity.SchemaId, UnknownAssignment));
                }

                // validate if the role binding attribute name is valid
                if (activity.RolesBindingAttribute != null && activity.RolesBindingAttribute.Length > 0 &&
                    !ActivityValidatingServiceProvider.Instance.ValidateService.IsValidAttributeName(rootActivity.SchemaId, rootActivity.ClassName, activity.RolesBindingAttribute))
                {
                    validationErrors.Add(new ValidationError(activity.RolesBindingAttribute + " attribute doesn't exist in the class " + rootActivity.ClassName + " of database schema " + rootActivity.SchemaId, UnknownAssignment));
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

                // validate if static assigned roles are still existing
                if (activity.Roles != null && activity.Roles.Count > 0)
                {
                    foreach (string role in activity.Roles)
                    {
                        if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidRole(role))
                        {
                            validationErrors.Add(new ValidationError(role + " role doesn't exist any more", UnknownAssignment));
                        }
                    }
                }
            }

            private void ValidateLogProperties(ValidationErrorCollection validationErrors, CreateTaskActivity activity)
            {
                if (activity.WriteLog)
                {
                    // Validate the BindingInstanceKey property
                    if (String.IsNullOrEmpty(activity.BindingInstanceKey))
                    {
                        validationErrors.Add(ValidationError.GetNotSetValidationError(CreateTaskActivity.BindingInstanceKeyProperty.Name));
                    }
                }
            }
         }

        #endregion
    }
}

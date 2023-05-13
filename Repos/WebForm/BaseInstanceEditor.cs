using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;
using Newtonsoft.Json.Linq;

using HtmlAgilityPack;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.Wrapper;
using Newtera.Server.Engine.Workflow;

namespace Newtera.WebForm
{
/// <remarks>
/// BaseInstanceEditor is a base class that implements the IInstanceEditor interface, it provide common properties and methods
/// to be inherited by the subclasses
/// It builds:
/// * default ui componnents  for each property
/// * default validating rule for each property
/// * translate model data to view model data for each property
/// * translate view model data to model data for each property
/// 
/// </remarks>
    public abstract class BaseInstanceEditor : IInstanceEditor
	{
        protected const char NAME_SEPATATOR_CHAR = ';';
        protected const char NAME_PAIR_SEPATATOR_CHAR = ':';
        protected const string DEFAULT_FORM_NAME = "ebaasform";
        protected const string DEFAULT_VIEW_MODEL_PATH = "model.";
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        private HtmlDocument _doc;

        private InstanceView _instanceView = null;   // storage for InstanceView

        private dynamic _instance = null;

        private string _displayedPropertyNames = null; // properties to be displayed, names are separated with ';'

        private List<PropertyPlaceHolder> _propertyPlaceHolders = new List<PropertyPlaceHolder>();

        private string _instanceId = null; // id of instance to be shown in editor

        private string _connectionString = null; // the connection string to the current databas

        private bool _isViewOnly = false;

        private string _formName = null;

        private bool _isInlineForm = false;

        private bool _isRemoved = false;

        private string _formId = null;

        private string _taskId = null;

        private string _actionId = null;

        protected Hashtable _propertyReadOnlyStatusTable = null;

        public BaseInstanceEditor()
        {
            EditorId = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Substring(0, 22)
                .Replace("/", "_")
                .Replace("+", "-");
        }

        public string EditorId { get; }

        /// <summary>
        /// Get the html document for the editor
        /// </summary>
        public HtmlDocument Document
        {
            get
            {
                return _doc;
            }
            set
            {
                _doc = value;
            }
        }

        /// <summary>
        /// Gets or sets a JObject instance submitted by the client
        /// </summary>
        public dynamic Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Gets or sets a html form name of the editor
        /// </summary>
        public string FormName
        {
            get
            {
                if (!string.IsNullOrEmpty(_formName))
                {
                    return _formName;
                }
                else
                {
                    return DEFAULT_FORM_NAME;
                }
            }
            set
            {
                _formName = value;
            }
        }

        /// <summary>
        /// Gets a root path of the view model for the editor
        /// </summary>
        public virtual string ViewModelPath {
            get
            {
                return DEFAULT_VIEW_MODEL_PATH;
            }
        }

        /// <summary>
        /// Gets or set the id of a form (post by a client using headers), This property is used to identify a client when running BeforeInsert or BefoerUpdate scripts
        /// </summary>
        public string FormId
        {
            get
            {
                return _formId;
            }
            set
            {
                _formId = value;
            }
        }

        /// <summary>
        /// Gets or set the id of a workflow task, This property is used to identify a wf task when running custom action script
        /// </summary>
        public string TaskId
        {
            get
            {
                return _taskId;
            }
            set
            {
                _taskId = value;
            }
        }

        /// <summary>
        /// Gets or set the id of a workflow action, This property is used to identify a wf action when running custom action script
        /// </summary>
        public string ActionId
        {
            get
            {
                return _actionId;
            }
            set
            {
                _actionId = value;
            }
        }

        /// <summary>
        /// Gets a root path of accessing an array in the view model
        /// </summary>
        public virtual string ArrayBasePath
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether a instance represented by the editor has been removed from UI
        /// </summary>
        public virtual bool IsRemoved
        {
            get
            {
                return _isRemoved;
            }
            set
            {
                _isRemoved = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether the properties are displayed in a inline form
        /// </summary>
        public bool IsInlineForm
        {
            get
            {
                return _isInlineForm;
            }
            set
            {
                _isInlineForm = value;
            }
        }

        /// <summary>
        /// InstanceView is the instance we are viewing. Null is permitted but the table will be empty.
        /// </summary>
        public InstanceView EditInstance
		{
			get
			{
				return _instanceView; 
			}
			set 
			{
				_instanceView = value;
			}  // permits null
		}

        /// <summary>
        /// The editor that is a container of this editor
        /// </summary>
        public IInstanceEditor ParentEditor { get; set; }

        /// <summary>
        /// Gets or sets a connection string to the current database
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// A coolection of property holders representing properties appeared in this editor
        /// </summary>
        public List<PropertyPlaceHolder> PropertyPlaceHolders
        {
            get
            {
                return _propertyPlaceHolders;
            }
        }

        /// <summary>
        /// Define a list of properties to be displayed. If not provided, display all properties.
        /// The value shou
        /// </summary>
        public string DisplayedPropertyNames
        {
            get
            {
                return _displayedPropertyNames;
            }
            set 
            {
                _displayedPropertyNames = value; 
            }
        }

        /// <summary>
        /// Gets or sets id of the instance to be shown in property editor
        /// </summary>
        public string InstanceId
        {
            get
            {
                string instanceId = null;
                if (EditInstance != null && EditInstance.InstanceData != null)
                {
                    instanceId = EditInstance.InstanceData.ObjId;
                }

                return instanceId;
            }
            set
            {
                if (EditInstance != null && EditInstance.InstanceData != null)
                {
                    EditInstance.InstanceData.ObjId = value;
                }
            }
        }


        /// <summary>
        /// Gets class name of the edit instance
        /// </summary>
        public string InstanceClassName
        {
            get
            {
                string className = null;
                if (EditInstance != null)
                {
                    className = EditInstance.DataView.BaseClass.ClassName;
                }

                return className;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the given instance has values
        /// </summary>
        public bool HasValues(JObject instance)
        {
            bool status = false;
            InstanceAttributePropertyDescriptor vPropertyInfo;

            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);

            // iterate through the JSON instance's properties and set the value to the corresponding property in the InstanceView object
            foreach (JProperty jproperty in ((JToken)instance))
            {
                BaseProperty vPropertyControl = null;
                vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[jproperty.Name];

                if (vPropertyInfo != null)
                {
                    vPropertyControl = GetPropertyControl(vPropertyInfo);

                    if (vPropertyControl != null)
                    {
                        if (vPropertyControl.HasValue(jproperty))
                        {
                            status = true;
                            break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the given instance has values
        /// </summary>
        public bool HasValues(XmlElement instance)
        {
            bool status = false;
            InstanceAttributePropertyDescriptor vPropertyInfo;

            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);

            // iterate through the XmlElement's properties and set the value to the corresponding property in the InstanceView object
            foreach (XmlElement childElement in instance.ChildNodes)
            {
                BaseProperty vPropertyControl = null;
                vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[childElement.Name];

                if (vPropertyInfo != null)
                {
                    vPropertyControl = GetPropertyControl(vPropertyInfo);

                    if (vPropertyControl != null)
                    {
                        if (vPropertyControl.HasValue(childElement))
                        {
                            status = true;
                            break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether th editor is for a related class to the master class
        /// </summary>
        public abstract bool IsRelatedClass { get; }

        /// <summary>
        /// IsViewOnly determines if the InstanceEditor allows editing or works simply as a viewer 
        /// When true, all fields appear as read only. 
        /// </summary>
        public virtual bool IsViewOnly
		{
			get 
			{
                return _isViewOnly; 
			}
			set
            {
                _isViewOnly = value;
            } 
		}

        /// <summary>
        /// Run the action code of an action
        /// </summary>
        /// <param name="connection">db connection</param>
        /// <param name="schemaName">The schema name</param>
        /// <param name="taskId"> the id of a workflow task sent by the client</param>
        /// <param name="actionId">The id of workflow action sent by a client</param>
        /// <param name="instanceView">The instance to run the code.</param>
        public void RunCustomAction(CMConnection connection, string schemaName, string taskId, string actionId, InstanceView instanceView, dynamic instance)
        {
            Newtera.WFModel.TaskInfo taskInfo = GetTaskInfo(schemaName, taskId);
            if (taskInfo != null &&
                taskInfo.BindingClassName == instanceView.DataView.BaseClass.ClassName &&
                !string.IsNullOrEmpty(taskInfo.CustomActionsXml))
            {
                // convert xml string to a collection of CustomAction objects
                StringReader reader = new StringReader(taskInfo.CustomActionsXml);
                Newtera.WFModel.CustomActionCollection customActions = new Newtera.WFModel.CustomActionCollection();
                customActions.Read(reader);

                Newtera.WFModel.CustomAction customAction = null;
                foreach (Newtera.WFModel.CustomAction ca in customActions)
                {
                    if (ca.ID == actionId)
                    {
                        customAction = ca;
                        break;
                    }
                }

                if (customAction != null && !string.IsNullOrEmpty(customAction.ActionCode))
                {
                    IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView, connection.ConnectionString, instance, true);

                    ActionCodeRunner.Instance.ExecuteActionCode(customAction.ID, customAction.ActionCode, instanceWrapper);
                }
            }
        }

        /// <summary>
        /// Close the task and raise event to inform HnadleGroupTaskActivity
        /// </summary>
        /// <param name="schemaId">The id of database</param>
        /// <param name="className">The class name of the instance</param>
        /// <param name="taskId">The task id</param>
        /// <param name="actionName">The name of action</param>
        /// <param name="instanceView">The instance view</param>
        public void RaiseCloseTaskEvent(CMConnection connection, string schemaName, string taskId, string actionId, InstanceView instanceView)
        {
            Newtera.WFModel.TaskInfo taskInfo = GetTaskInfo(schemaName, taskId);
            if (taskInfo != null &&
                taskInfo.BindingClassName == instanceView.DataView.BaseClass.ClassName &&
                !string.IsNullOrEmpty(taskInfo.CustomActionsXml))
            {
                // convert xml string to a collection of CustomAction objects
                StringReader reader = new StringReader(taskInfo.CustomActionsXml);
                Newtera.WFModel.CustomActionCollection customActions = new Newtera.WFModel.CustomActionCollection();
                customActions.Read(reader);

                Newtera.WFModel.CustomAction customAction = null;
                foreach (Newtera.WFModel.CustomAction ca in customActions)
                {
                    if (ca.ID == actionId)
                    {
                        customAction = ca;
                        break;
                    }
                }

                if (customAction != null)
                {
                    if (customAction.CloseTask && !string.IsNullOrEmpty(taskId))
                    {
                        Newtera.WorkflowServices.ITaskService taskService = NewteraWorkflowRuntime.Instance.GetWorkflowRunTime().GetService<Newtera.WorkflowServices.ITaskService>();

                        taskService.CloseTaskById(taskId);

                        // raise a task completed event to the HandleGroupTaskActivity of the workflow if any
                        EventRaiser eventRaiser = new EventRaiser();
                        eventRaiser.RaiseTaskCompletedEventToWorkflowActivity(connection.SchemaInfo.NameAndVersion,
                            instanceView.DataView.BaseClass.ClassName,
                            instanceView.InstanceData.ObjId,
                            taskId);
                    }
                }
            }
        }

        protected bool IsPropertyReadOnly(string propertyName)
        {
            if (_propertyReadOnlyStatusTable != null &&
                _propertyReadOnlyStatusTable[propertyName] != null)
            {
                return (bool)_propertyReadOnlyStatusTable[propertyName];
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get a hashtable of property readonly status definition defined in Workflow Model for a task
        /// </summary>
        /// <param name="connection">db connection</param>
        /// <param name="schemaName">The schema name</param>
        /// <param name="taskId"> the id of a workflow task sent by the client</param>
        /// <param name="actionId">The id of workflow action sent by a client</param>
        /// <param name="instanceView">The instance to run the code.</param>
        protected Hashtable GetPropertyReadOnlyStatusTable(string schemaName, string taskId)
        {
            Hashtable table = new Hashtable();
            if (!string.IsNullOrEmpty(taskId))
            {
                Newtera.WFModel.TaskInfo taskInfo = GetTaskInfo(schemaName, taskId);
                if (taskInfo != null)
                {
                    string formProperties = taskInfo.FormProperties;

                    if (!string.IsNullOrEmpty(formProperties))
                    {
                        // The form properties string is in form of "property1:r;property2:r;property3"
                        string[] propertyDefs = formProperties.Split(';');
                        foreach (string propertyDef in propertyDefs)
                        {
                            string[] eq = propertyDef.Split(':');
                            if (eq.Length > 1)
                            {
                                if (eq[1] == "r")
                                {
                                    table[eq[0]] = true; // the property is readonly
                                }
                                else
                                {
                                    table[eq[0]] = false; // the property is not readonly
                                }
                            }
                            else if (eq.Length > 0)
                            {
                                table[eq[0]] = false; // the property is not readonly
                            }
                        }
                    }
                }
            }

            return table;
        }

        private Newtera.WFModel.TaskInfo GetTaskInfo(string schemaName, string taskId)
        {
            Newtera.WFModel.TaskInfo taskInfo = null;

            string connectionString = GetConnectionString(CONNECTION_STRING, schemaName);
            using (WFConnection wfConnection = new WFConnection(connectionString))
            {
                wfConnection.Open();

                string schemaId = GetSchemaId(connectionString);
                taskInfo = wfConnection.GetTask(taskId);
            }

            return taskInfo;
        }

        private string GetSchemaId(string connectionStr)
        {
            Hashtable properties = GetProperties(connectionStr);
            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
            schemaInfo.Name = (string)properties["SCHEMA_NAME"];
            schemaInfo.Version = (string)properties["SCHEMA_VERSION"];

            return schemaInfo.NameAndVersion;
        }

        /// <summary>
        /// Get key/value pairs from the connectionString and save them in a hashtable
        /// </summary>
        /// <param name="connectionString">The connectionString</param>
        /// <returns>The hashtable</returns>
        /// <exception cref="InvalidConnectionStringException">
        /// Thrown if missing some critical key/value pairs in the connection string.
        /// </exception>
        private Hashtable GetProperties(string connectionString)
        {
            Hashtable properties = new Hashtable();

            // Compile regular expression to find "name = value" pairs
            Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

            MatchCollection matches = regex.Matches(connectionString);
            foreach (Match match in matches)
            {
                int pos = match.Value.IndexOf("=");
                string key = match.Value.Substring(0, pos).TrimEnd();
                string val = match.Value.Substring(pos + 1).TrimStart();
                properties[key] = val;
            }

            return properties;
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }

        /// <summary>
        /// It builds and returns a SortedList of both public and non-public properties for this class type. 
        /// Each element of the list has a name and value.
        /// The name is the InstanceAttributePropertyDescriptor.Name and this sorts the list.
        /// The value is the InstanceAttributePropertyDescriptor.
        /// </summary>
        protected virtual PropertyDescriptorCollection GetPropertyDescriptorList(InstanceView instanceView)
		{
			PropertyDescriptorCollection properties = instanceView.GetProperties(null);

            if (!string.IsNullOrEmpty(_displayedPropertyNames))
            {
                string[] propertyNames = _displayedPropertyNames.Split(NAME_SEPATATOR_CHAR);
                PropertyDescriptorCollection selectedProperties = new PropertyDescriptorCollection(null);
                string pName;
                int pos;
                bool allowManualUpdate;
                foreach (string propertyName in propertyNames)
                {
                    // if propertyName consists of name:r, it means that the property should be shown as
                    // non-editable text
                    pos = propertyName.IndexOf(NAME_PAIR_SEPATATOR_CHAR);
                    if (pos > 0)
                    {
                        pName = propertyName.Substring(0, pos);
                        if (propertyName.Substring(pos + 1).Trim() == "r")
                        {
                            allowManualUpdate = false;
                        }
                        else
                        {
                            allowManualUpdate = true;
                        }
                    }
                    else
                    {
                        pName = propertyName;
                        allowManualUpdate = true;
                    }

                    InstanceAttributePropertyDescriptor selectedProperty = properties[pName] as InstanceAttributePropertyDescriptor;
                    if (selectedProperty != null)
                    {
                        if (!allowManualUpdate)
                        {
                            selectedProperty.AllowManualUpdate = allowManualUpdate;
                        }

                        selectedProperties.Add(selectedProperty);
                    }
                }

                properties = selectedProperties;
            }

			return properties;
		}  // GetPropertyDescriptorList()


        /// <summary>
        /// Create a form template in html
        /// </summary>
        /// <param name="taskId">A workflow task id to which the form is created</param>
        public abstract string CreateFormTemplate(string taskId);

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public abstract List<IInstanceEditor> ConvertToModel(JObject instance);

        /// <summary>
        /// Translate an xml element to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="element">An XmlElement object</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public abstract List<IInstanceEditor> ConvertToModel(XmlElement element);

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <param name="isFormFormat">true if the json values are in form format where enum values are internal values</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public abstract List<IInstanceEditor> ConvertToModel(JObject instance, bool isFormFormat);

        /// <summary>
        /// Validate the model represented by the InstanceView instance against the schema.
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>Return if the instance model is valid. It will throw an exception if a validating error occures </returns>
        public abstract void Validate();

        /// <summary>
        /// Save the the master instance and related instnaces to the database 
        /// </summary>
        /// <param name="">A list of related instances</param>
        /// <returns>The obj_id of the saved instance</returns>
        public abstract string SaveToDatabase(List<IInstanceEditor> relatedEditInstances);

        /// <summary>
        /// ConvertToViewModel converts values from the InstanceView instance (Model) to a JSON instance (ViewModel)
        /// the EditInstance
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>The JSON instance</returns>
        public abstract JObject ConvertToViewModel(bool convert);

        /// <summary>
        /// ConvertToIndexingDocument converts values from the InstanceView instance (Model) to a JSON instance as a document
        /// for full-text search indexing
        /// </summary>
        /// <returns>The JSON instance</returns>
        public abstract JObject ConvertToIndexingDocument();

        /// <summary>
        /// Create a IPropertyControl object for the given property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>IPropertyControl object, can be null</returns>
        public virtual IPropertyControl CreatePropertyControl(string propertyName)
        {
            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);
            InstanceAttributePropertyDescriptor vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[propertyName];

            if (vPropertyInfo != null)
            {
                return (IPropertyControl) GetPropertyControl(vPropertyInfo);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create a IPropertyControl object for the given property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="isReadOnly">true to create a readonly property control, false otherwise</param>
        /// <param name="defaultValue">the property default value</param>
        /// <returns>IPropertyControl object, can be null</returns>
        public IPropertyControl CreatePropertyControl(string propertyName, bool isReadOnly, string defaultValue)
        {
            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);
            InstanceAttributePropertyDescriptor vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[propertyName];

            if (vPropertyInfo != null)
            {
                bool readOnly = isReadOnly;

                // determine if the control should be readonly
                if (IsViewOnly || vPropertyInfo.IsReadOnly || !vPropertyInfo.AllowManualUpdate)
                {
                    readOnly = true;
                }

                return (IPropertyControl)CreatePropertyControl(vPropertyInfo, readOnly, defaultValue);
            }
            else
            {
                return null;
            }
        }

        protected BaseProperty GetPropertyControl(InstanceAttributePropertyDescriptor vPropertyInfo)
        {
            BaseProperty vPropertyControl = null;
            bool isReadOnly = false;

            // Filter out any inappropriate or undesirable properties
            if (vPropertyInfo == null || FilterOut(vPropertyInfo))
                return null;

            // determine if the control should be readonly
            if (IsViewOnly || vPropertyInfo.IsReadOnly || !vPropertyInfo.AllowManualUpdate)
            {
                isReadOnly = true;
            }

            string defaultValue = GetPropertyDefaultValue(vPropertyInfo);

            vPropertyControl = CreatePropertyControl(vPropertyInfo, isReadOnly, defaultValue);

            return vPropertyControl;
        }

        /// <summary>
        /// CreatePropertyControl creates a BaseProperty control based on information within
        /// pPropertyInfo. This method is virtual, anticipating additional user-defined BaseProperty classes.
        /// It does not support ReadOnlyProperty. CreatePropertySections will first check in case the control
        /// should be ReadOnlyProperty. If not, it calls this. This permits you to override all but
        /// ReadOnlyProperty by calling base.CreatePropertyControl after evaluating pPropertyInfo for
        /// custom classes.
        /// </summary>
        protected BaseProperty CreatePropertyControl(InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly, string defaultValue)
		{
            // NOTE: This could be written to use the TypeConverter associated with
            // each type. Perhaps an upgrade should support TypeConverter's translation
            // between strings and the actual datatype in any custom TypeConverter.
            // Imagine a textbox control associated with the TypeConverter.
            BaseProperty vPropertyControl = null;
            InstanceView instanceView = this.EditInstance;

            if (pPropertyInfo.IsRelationship)
            {
                if (pPropertyInfo.IsForeignKeyRequired)
                {
                    if (!string.IsNullOrEmpty(pPropertyInfo.UIControlCreator) &&
                        this.IsRelatedClass)
                    {
                        // has a customized UI control creator, create a customized control
                        vPropertyControl = new CustomizedUIProperty(this, instanceView, pPropertyInfo, isReadOnly);
                    }
                    else
                    {
                        // many-to-one or one-to-one with joined manager, popup selection
                        vPropertyControl = new RelationshipProperty(this, instanceView, pPropertyInfo, isReadOnly);
                    }
                }
                else
                {
                    // many-to-many relationship
                    vPropertyControl = new ManyToManyRelationshipProperty(this, instanceView, pPropertyInfo, isReadOnly);
                }
            }
            else if (pPropertyInfo.IsRichText)
            {
                vPropertyControl = new RichTextProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else if (pPropertyInfo.IsHistoryEdit)
            {
                vPropertyControl = new HistoryEditProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else if (!string.IsNullOrEmpty(pPropertyInfo.UIControlCreator))
            {
                // has a customized UI control creator, create a customized control
                vPropertyControl = new CustomizedUIProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else if (pPropertyInfo.PropertyType.IsEnum)
            {
                if (pPropertyInfo.IsMultipleChoice)
                {
                    vPropertyControl = new MultipleChoiceProperty(this, instanceView, pPropertyInfo, isReadOnly);
                }
                else if (pPropertyInfo.HasConstraint &&
                    (((IEnumConstraint)pPropertyInfo.Constraint).IsConditionBased || !string.IsNullOrEmpty(pPropertyInfo.CascadedPropertyNames)))
                {
                    // Values of the enum constraint are static
                    vPropertyControl = new CascadingListProperty(this, instanceView, pPropertyInfo, isReadOnly);
                }
                else
                {
                    // Values of the enum constraint are static
                    vPropertyControl = new EnumProperty(this, instanceView, pPropertyInfo, isReadOnly);
                }
            }
            else if (pPropertyInfo.IsImage)
            {
                vPropertyControl = new ImageProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else if (pPropertyInfo.PropertyType == typeof(string))
            {
                if (pPropertyInfo.IsVirtual)
                {
                    vPropertyControl = new VirtualProperty(this, instanceView, pPropertyInfo, isReadOnly);
                }
                else if (string.IsNullOrEmpty(pPropertyInfo.DataSourceName))
                {
                    vPropertyControl = new StringProperty(this, instanceView, pPropertyInfo, isReadOnly, defaultValue);
                }
                else
                {
                    // MaskedInputProperty handle picking data from a data source
                    vPropertyControl = new MaskedInputProperty(this, instanceView, pPropertyInfo, isReadOnly);
                }
            }
            else if (pPropertyInfo.PropertyType == typeof(bool))
            {
                vPropertyControl = new BoolProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else if (pPropertyInfo.PropertyType == typeof(DateTime))
            {
                if (pPropertyInfo.DataType == DataType.Date)
                    vPropertyControl = new DateProperty(this, instanceView, pPropertyInfo, isReadOnly);
                else
                    vPropertyControl = new DateTimeProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else if (pPropertyInfo.PropertyType == typeof(DataTable))
            {
                vPropertyControl = new ArrayProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else if (pPropertyInfo.PropertyType.IsPrimitive ||
                BaseInstanceEditor.IsNumericType(pPropertyInfo.PropertyType))   // captures anything that is supported by NumericProperty
            {
                vPropertyControl = new NumericProperty(this, instanceView, pPropertyInfo, isReadOnly);
            }
            else
            {
                // get StringProperty for any other type of objects
                vPropertyControl = new StringProperty(this, instanceView, pPropertyInfo, isReadOnly, defaultValue);
            }

			return vPropertyControl;
		}  // CreatePropertyControl()

		/// <summary>
		/// FilterOut determines if the property should be shown or not.
		/// It returns true when the property should be hidden.
		/// There are numerous rules:
		/// 1. Write Only.
		/// 2. Read Only (various cases)
		/// 3. Arrays
		/// 5. Apply the filters by Name and Type
		/// </summary>
		protected virtual bool FilterOut(InstanceAttributePropertyDescriptor pPropertyInfo)
		{
			// omit properties that have their Browsable Attribute set to false.
			if (!pPropertyInfo.IsBrowsable)
				return true;

			return false;  // no problems. Passed all tests
		}  // FilterOut()


        protected bool IsParentOf(string parentClassName, string childClassName)
        {
            bool status = false;

            ClassElement childClassElement = EditInstance.DataView.SchemaModel.FindClass(childClassName);
            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                status = true;
            }

            return status;
        }

        protected bool IsParentOf(MetaDataModel metaData, string parentClassName, string childClassName)
        {
            bool status = false;

            ClassElement childClassElement = metaData.SchemaModel.FindClass(childClassName);
            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                status = true;
            }

            return status;
        }

        protected void AddPermissionProperties(JObject instance, string permissionStr)
        {
            if (!string.IsNullOrEmpty(permissionStr))
            {
                XaclPermissionFlag flags = (XaclPermissionFlag)Enum.Parse(typeof(XaclPermissionFlag), permissionStr);

                if (HasPermission(XaclActionType.Read, flags))
                {
                    instance.Add("allowRead", true);
                }
                else
                {
                    instance.Add("allowRead", false);
                }

                if (HasPermission(XaclActionType.Write, flags))
                {
                    instance.Add("allowWrite", true);
                }
                else
                {
                    instance.Add("allowWrite", false);
                }

                if (HasPermission(XaclActionType.Create, flags))
                {
                    instance.Add("allowCreate", true);
                }
                else
                {
                    instance.Add("allowCreate", false);
                }

                if (HasPermission(XaclActionType.Delete, flags))
                {
                    instance.Add("allowDelete", true);
                }
                else
                {
                    instance.Add("allowDelete", false);
                }

                if (HasPermission(XaclActionType.Upload, flags))
                {
                    instance.Add("allowUpload", true);
                }
                else
                {
                    instance.Add("allowUpload", false);
                }

                if (HasPermission(XaclActionType.Download, flags))
                {
                    instance.Add("allowDownload", true);
                }
                else
                {
                    instance.Add("allowDownload", false);
                }
            }
            else
            {
                // add defaults

                instance.Add("allowRead", true);
                instance.Add("allowWrite", true);
                instance.Add("allowCreate", true);
                instance.Add("allowDelete", true);
                instance.Add("allowUpload", true);
                instance.Add("allowDownload", true);
            }
        }

        public bool HasPermission(XaclActionType action, XaclPermissionFlag flags)
        {
            bool hasPermission = true;

            switch (action)
            {
                case XaclActionType.Read:
                    if ((flags & XaclPermissionFlag.GrantRead) == 0)
                    {
                        hasPermission = false;
                    }
                    break;
                case XaclActionType.Write:
                    if ((flags & XaclPermissionFlag.GrantWrite) == 0)
                    {
                        hasPermission = false;
                    }
                    break;
                case XaclActionType.Create:
                    if ((flags & XaclPermissionFlag.GrantCreate) == 0)
                    {
                        hasPermission = false;
                    }
                    break;
                case XaclActionType.Delete:
                    if ((flags & XaclPermissionFlag.GrantDelete) == 0)
                    {
                        hasPermission = false;
                    }
                    break;
                case XaclActionType.Upload:
                    if ((flags & XaclPermissionFlag.GrantUpload) == 0)
                    {
                        hasPermission = false;
                    }
                    break;
                case XaclActionType.Download:
                    if ((flags & XaclPermissionFlag.GrantDownload) == 0)
                    {
                        hasPermission = false;
                    }
                    break;
            }

            return hasPermission;
        }

        private string GetPropertyDefaultValue(InstanceAttributePropertyDescriptor vPropertyInfo)
        {
            string defaultValue = null;

            PropertyPlaceHolder propertyPlaceHolder = null;

            foreach (PropertyPlaceHolder ph in this.PropertyPlaceHolders)
            {
                if (ph.PropertyName == vPropertyInfo.Name)
                {
                    propertyPlaceHolder = ph;
                    break;
                }
            }

            if (propertyPlaceHolder != null)
            {
                defaultValue = propertyPlaceHolder.DefaultValue;
            }

            return defaultValue;
        }

        static internal bool IsNumericType(Type type)
        {
            if (type == typeof(Int32) ||
                type == typeof(Int64) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal))
                return true;
            else
                return false;
        }
    }


	//----CLASS PropertySection -----------------------------------------------------------
	/// <remarks>
	/// PropertySection is a section element that includes references
	/// to key controls in the section: the Label in column 1, the BaseProperty subclass, and the error label.
	/// In this sense, it is specialized to work with other classes here.
	/// </remarks>
	public class PropertySection
	{
        public HtmlNode HtmlNode = null;
        public BaseProperty fPropertyControl = null;
        public bool Visible = true;

        public PropertySection(HtmlDocument doc)
        {
            HtmlNode = doc.CreateElement("section");
        }
	}  // class PropertySection

	//----CLASS BaseProperty --------------------------------------------------------------
	/// <remarks>
	/// BaseProperty and its subclasses build controls to view and edit data for specific datatypes.
	/// The InstanceEditor class uses these to build its html elements
	/// BaseProperty references the InstanceAttributePropertyDescriptor of the property and the control that owns it.
	/// BaseProperty includes several static and support methods for use by its child controls.
	/// </remarks>   
	abstract public class BaseProperty : IPropertyControl
    {
        protected InstanceView fInstanceView; // the instance view bound to the property editor
        protected InstanceAttributePropertyDescriptor fPropertyInfo;  // the info for the property. Some subclasses don't use this and its null.
        protected IInstanceEditor fOwnerEditor = null;   // The PpropertyEditor which owns this.
        protected string fHint = "";   // if the InstanceAttributePropertyDescriptor offers a DescriptionAttribute, this is its text.
        protected bool bReadOnly = false;
        protected string fControlName = null;

        protected const string SELECTED_VALUE = "selectedValue";
        protected const string OPTIONS = "options";

        public BaseProperty(IInstanceEditor pOwnerControl, InstanceView instanceView, InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
		{
            fInstanceView = instanceView;
			fPropertyInfo = pPropertyInfo;
			fOwnerEditor = pOwnerControl;

			// get the hint (tooltip) from the Description of the property
			if (fPropertyInfo != null)
			{
				fHint = pPropertyInfo.Description;
			}

            bReadOnly = isReadOnly;

        }  // Constructor

        /// <summary>
        /// Gets or set the Instance Editor on which the property control lives
        /// </summary>
        public IInstanceEditor InstanceEditor
        {
            get
            {
                return fOwnerEditor;
            }
            set
            {
                fOwnerEditor = value;
            }
        }

        /// <summary>
        /// Gets or set the object that contains information about the property
        /// </summary>
        public InstanceAttributePropertyDescriptor PropertyInfo
        {
            get
            {
                return fPropertyInfo;
            }
            set
            {
                fPropertyInfo = value;
            }
        }

        /// <summary>
        /// Gets an unique name for the property control in a form
        /// </summary>
        public string PropertyControlName
        {
            get
            {
                if (string.IsNullOrEmpty(fControlName))
                {
                    if (this.fOwnerEditor.IsRelatedClass)
                    {
                        fControlName = this.fOwnerEditor.InstanceClassName + "_" + this.PropertyInfo.Name;
                    }
                    else
                    {
                        fControlName = this.PropertyInfo.Name;
                    }
                }

                return fControlName;
            }
        }

        /// <summary>
        /// Gets an instance view of the base class of the property
        /// </summary>
        public InstanceView BaseInstanceView
        {
            get
            {
                return fInstanceView;
            }
            set
            {
                fInstanceView = value;
            }
        }

        /// <summary>
        /// Gets or set the readonly status
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return bReadOnly || !fPropertyInfo.AllowManualUpdate;
            }
            set
            {
                bReadOnly = value;
            }
        }

        /// <summary>
        /// Check if the jproperty contains non-empty value for the property
        /// </summary>
        /// <param name="jProperty"></param>
        /// <returns></returns>
        public virtual bool HasValue(JProperty jProperty)
        {
            if (jProperty.Value != null &&
                !string.IsNullOrEmpty(jProperty.Value.ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if an xml element contains non-empty value for the property
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public virtual bool HasValue(XmlElement element)
        {
            if (element != null &&
                !string.IsNullOrEmpty(element.InnerText))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validate the property value and throw an exception if there is an error
        /// </summary>
        public virtual void Validate()
        {
            // do nothing
        }

        /// <summary>
        /// create html nodes for the property
        /// </summary>
        public virtual HtmlNode CreatePropertyNode()
        {
            // create a property container for all types of properties
            HtmlNode container = this.fOwnerEditor.Document.CreateElement("div");
            container.SetAttributeValue("ng-class", "getCssClasses(" + fOwnerEditor.FormName  + "." + this.PropertyControlName + ")");
            if (this.InstanceEditor.IsInlineForm)
            {
                //container.SetAttributeValue("class", "col-sm-3");
            }
            return container;
        }

        /// <summary>
        /// Convert the value in the json property(ViewModel) to model value, can be overrided by subclass to convert JProperty value to string
        /// </summary>
        /// <returns></returns>
        public virtual void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            if (jProperty.Value != null)
            {
                fPropertyInfo.SetStringValue(jProperty.Value.ToString());
            }
        }

        /// <summary>
        /// Update the instance with the value in a XmlElement object
        /// </summary>
        /// <param name="jProperty">the JProperty object</param>
        /// <param name="isFormFormat">whether the value is in the form format</param>
        public virtual void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (element != null)
            {
                fPropertyInfo.SetStringValue(element.InnerText);
            }
        }

        /// <summary>
        /// Gets model instances from an array of ViewModel instances when jProperty;s value is an JArray.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public virtual List<IInstanceEditor> GetInstanceEditors(JProperty jProperty)
        {
            return null;
        }

        /// <summary>
        /// Gets model instances from the child elements of an xml element representing a wrapper of related instances.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public virtual List<IInstanceEditor> GetInstanceEditors(XmlElement element)
        {
            return null;
        }

        public virtual JToken GetPropertyViewModel()
        {
            string val = fPropertyInfo.GetStringValue();
            if (val != null)
            {
                return val;
            }
            else
            {
                return "";
            }
        }

		/// <summary>
		/// ToolTipToStatus is a Utility function to attach the fHint to the parent control.
		/// </summary>
		public void ToolTipToStatus(HtmlNode pControl)
		{
            return;

            if ((pControl != null))
            {
                string tipText = fHint;
                if (string.IsNullOrEmpty(tipText))
                {
                    tipText = "This a string type";
                }
  
                HtmlNode tip = this.fOwnerEditor.Document.CreateElement("b");
                tip.SetAttributeValue("class", "tooltip tooltip-bottom-right");
                tip.InnerHtml = HtmlDocument.HtmlEncode(tipText);
                pControl.AppendChild(tip);
            }
		}  // ToolTipToStatus()

        /// <summary>
        /// GetEnumValue is a static method to retrieve an enum value from the pEnumControl ASPxComboBox.
        /// pEnumControl must have each ListEditItem's Value set to the enumerated type's value.
        /// </summary>
        protected Enum GetEnumValue(string enumValue, Type pEnumType)
        {
            if (!string.IsNullOrEmpty(enumValue))
            {
                int vValue = Convert.ToInt32(enumValue);
                Enum vEnum = (Enum)(Enum.ToObject(pEnumType, vValue));

                return vEnum;
            }
            else
            {
                return null;
            }
        }  // GetEnumValue()

        /// <summary>
        /// Create validations for the property
        /// </summary>
        /// <param name="propertyContainer"></param>
        protected void CreateValidators(HtmlNode propertyContainer)
        {
            if (fPropertyInfo.IsRequired)
            {
                HtmlNode requiredNode = this.fOwnerEditor.Document.CreateElement("span");
                requiredNode.SetAttributeValue("class", "help-block");
                requiredNode.SetAttributeValue("ng-show", "showError(" + fOwnerEditor.FormName + ", " + fOwnerEditor.FormName + "." + PropertyControlName + ", 'required')");
                propertyContainer.AppendChild(requiredNode);

                HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                iNode.SetAttributeValue("class", "fa fa-warning");
                requiredNode.AppendChild(iNode);

                HtmlTextNode textNode = this.fOwnerEditor.Document.CreateTextNode(fPropertyInfo.DisplayName + "{{getWord('Required')}}");
                requiredNode.AppendChild(textNode);
            }

            if (fPropertyInfo.PropertyType == typeof(string))
            {
                HtmlNode maxLengthNode = this.fOwnerEditor.Document.CreateElement("span");
                maxLengthNode.SetAttributeValue("class", "help-block");
                maxLengthNode.SetAttributeValue("ng-show", "showError(" + fOwnerEditor.FormName + ", "  + fOwnerEditor.FormName + "." + PropertyControlName + ", 'maxlength')");
                propertyContainer.AppendChild(maxLengthNode);

                HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                iNode.SetAttributeValue("class", "fa fa-warning");
                maxLengthNode.AppendChild(iNode);

                HtmlTextNode textNode = this.fOwnerEditor.Document.CreateTextNode(fPropertyInfo.DisplayName + "{{getWord('MaxLength')}}" + fPropertyInfo.MaxLength);
                maxLengthNode.AppendChild(textNode);
            }

            if (fPropertyInfo.PropertyType == typeof(byte))
            {
                HtmlNode maxValueNode = this.fOwnerEditor.Document.CreateElement("span");
                maxValueNode.SetAttributeValue("class", "help-block");
                maxValueNode.SetAttributeValue("ng-show", "showError(" + fOwnerEditor.FormName + ", " + fOwnerEditor.FormName + "." + PropertyControlName + ", 'max')");
                propertyContainer.AppendChild(maxValueNode);

                HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                iNode.SetAttributeValue("class", "fa fa-warning");
                maxValueNode.AppendChild(iNode);

                HtmlTextNode textNode = this.fOwnerEditor.Document.CreateTextNode(fPropertyInfo.DisplayName + "{{getWord('MaxValue')}}" + Byte.MaxValue);
                maxValueNode.AppendChild(textNode);

                HtmlNode minValueNode = this.fOwnerEditor.Document.CreateElement("span");
                minValueNode.SetAttributeValue("class", "help-block");
                minValueNode.SetAttributeValue("ng-show", "showError(" + fOwnerEditor.FormName + ", " + fOwnerEditor.FormName + "." + PropertyControlName + ", 'min')");
                propertyContainer.AppendChild(minValueNode);

                iNode = this.fOwnerEditor.Document.CreateElement("i");
                iNode.SetAttributeValue("class", "fa fa-warning");
                minValueNode.AppendChild(iNode);

                textNode = this.fOwnerEditor.Document.CreateTextNode(fPropertyInfo.DisplayName + "{{getWord('MinValue')}}" + Byte.MinValue);
                minValueNode.AppendChild(textNode);
            }

            if (fPropertyInfo.HasConstraint)
            {
                if (fPropertyInfo.Constraint is RangeElement)
                {
                    HtmlNode maxValueNode = this.fOwnerEditor.Document.CreateElement("span");
                    maxValueNode.SetAttributeValue("class", "help-block");
                    maxValueNode.SetAttributeValue("ng-show", "showError(" + fOwnerEditor.FormName + ", " + fOwnerEditor.FormName + "." + PropertyControlName + ", 'max')");
                    propertyContainer.AppendChild(maxValueNode);

                    HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                    iNode.SetAttributeValue("class", "fa fa-warning");
                    maxValueNode.AppendChild(iNode);

                    HtmlTextNode textNode = this.fOwnerEditor.Document.CreateTextNode(fPropertyInfo.DisplayName + "{{getWord('MaxValue')}}" + ((RangeElement)fPropertyInfo.Constraint).MaxValue);
                    maxValueNode.AppendChild(textNode);

                    HtmlNode minValueNode = this.fOwnerEditor.Document.CreateElement("span");
                    minValueNode.SetAttributeValue("class", "help-block");
                    minValueNode.SetAttributeValue("ng-show", "showError(" + fOwnerEditor.FormName + ", " + fOwnerEditor.FormName + "." + PropertyControlName + ", 'min')");
                    propertyContainer.AppendChild(minValueNode);

                    iNode = this.fOwnerEditor.Document.CreateElement("i");
                    iNode.SetAttributeValue("class", "fa fa-warning");
                    minValueNode.AppendChild(iNode);

                    textNode = this.fOwnerEditor.Document.CreateTextNode(fPropertyInfo.DisplayName + "{{getWord('MinValue')}}" + ((RangeElement)fPropertyInfo.Constraint).MinValue);
                    minValueNode.AppendChild(textNode);
                }
                else if (fPropertyInfo.Constraint is PatternElement)
                {
                    HtmlNode patternValueNode = this.fOwnerEditor.Document.CreateElement("span");
                    patternValueNode.SetAttributeValue("class", "help-block");
                    patternValueNode.SetAttributeValue("ng-show", "showError("  + fOwnerEditor.FormName + ", " + fOwnerEditor.FormName + "." + PropertyControlName + ", 'pattern')");
                    propertyContainer.AppendChild(patternValueNode);

                    HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                    iNode.SetAttributeValue("class", "fa fa-warning");
                    patternValueNode.AppendChild(iNode);

                    string message;
                    if (fPropertyInfo.Constraint.ErrorMessage != null)
                    {
                        message = fPropertyInfo.DisplayName + fPropertyInfo.Constraint.ErrorMessage;
                    }
                    else
                    {
                        message = fPropertyInfo.DisplayName + "{{getWord('PatternValue')}}" + ((PatternElement)fPropertyInfo.Constraint).PatternValue;
                    }

                    HtmlTextNode textNode = this.fOwnerEditor.Document.CreateTextNode(message);
                    patternValueNode.AppendChild(textNode);
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the property is a one-to-one relatioship
        /// </summary>
        /// <param name="pd"></param>
        /// <returns>true if it is part of an unique constrain keys, false otherwise.</returns>
        private bool NeedToValidateUniqueReference(InstanceView instanceView, InstanceAttributePropertyDescriptor pd)
        {
            bool status = false;

            IDataViewElement element = instanceView.DataView.ResultAttributes[pd.Name];
            if (element != null)
            {
                RelationshipAttributeElement relationship = element.GetSchemaModelElement() as RelationshipAttributeElement;

                // if it is a one-to-one relationship, check the one-to-one constraint
                if (relationship != null &&
                    relationship.Type == RelationshipType.OneToOne &&
                    !relationship.IsJoinManager)
                {
                    // requires validating the reference
                    status = true;
                }
            }

            return status;
        }

        protected HtmlNode CreateReadOnlyControl()
        {
            HtmlNode rootNode;
            HtmlNode fTextControl;

            if (fPropertyInfo.IsMultipleLined)
            {
                fTextControl = this.fOwnerEditor.Document.CreateElement("textarea");
                fTextControl.SetAttributeValue("class", "form-control");
                fTextControl.SetAttributeValue("rows", fPropertyInfo.Rows.ToString());
                rootNode = fTextControl;
            }
            else
            {
                fTextControl = this.fOwnerEditor.Document.CreateElement("input");
                fTextControl.SetAttributeValue("class", "form-control");
                fTextControl.SetAttributeValue("type", "text");

                rootNode = fTextControl;
            }

            // set the angularjs  model binding
            fTextControl.SetAttributeValue("name", PropertyControlName);
            fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
            fTextControl.SetAttributeValue("readonly", "readonly");
            fTextControl.SetAttributeValue("style", "background-color:#f7f9f9;");
            fTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);


            return rootNode;
        } 


        /// <summary>
        /// Gets the information indicating whether the property is for view
        /// information only
        /// </summary>
        protected bool IsViewOnly
		{
			get
			{
				if (fOwnerEditor is IInstanceEditor)
				{
					return ((IInstanceEditor) fOwnerEditor).IsViewOnly;
				}
				else
				{
					return false;
				}
			}
		}

	}  // class BaseProperty

	//---- CLASS BaseTextBoxProperty ----------------------------------------------------------------
	/// <remarks>
	/// BaseTextBoxProperty class is a base class for values that use TextBoxes.
	/// It supplies a default ApplyValueInternal() method which converts using value.ToString().
	/// Subclasses must supply their own UpdateValueInternal to convert from string to the correct type.
	/// </remarks>
	abstract public class BaseTextBoxProperty : BaseProperty
	{
        public HtmlNode fTextControl = null;


        public BaseTextBoxProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
		}

        /// <summary>
        /// Validate the property value and throw an exception if there is an error
        /// </summary>
        public override void Validate()
        {
            // do nothing
            // set the angularjs  model binding
            if (fPropertyInfo.IsRequired)
            {
                object vValue = fPropertyInfo.GetValue();

                if (vValue != null)
                {
                    string value = vValue.ToString();

                    if (string.IsNullOrEmpty(value))
                    {
                        throw new Exception(fPropertyInfo.DisplayName + " value is required");
                    }
                }
                else
                {
                    throw new Exception(fPropertyInfo.DisplayName + " value is required");
                }
            }
        }

        public override JToken GetPropertyViewModel()
        {
            string value = "";

            // most controls simply use the value.ToString() to get the data.
            // Override this method only when that is not the case.
            object vValue = fPropertyInfo.GetValue();

            if (vValue != null)
                value = vValue.ToString();

            return value;
        }

    }  // class BaseTextBoxProperty

	//---- CLASS StringProperty ---------------------------------------------------------------------
	/// <remarks>
	/// StringProperty class is for string properties.
	/// </remarks>
	public class StringProperty : BaseTextBoxProperty
	{
        private string _defaultValue;

        public StringProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly, string defaultValue)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Validate the property value and throw an exception if there is an error
        /// </summary>
        public override void Validate()
        {
            // do nothing
            // set the angularjs  model binding
            if (fPropertyInfo.IsRequired)
            {
                object vValue = fPropertyInfo.GetValue();

                if (vValue != null)
                {
                    string value = vValue.ToString();

                    if (string.IsNullOrEmpty(value) )
                    {
                        throw new Exception(fPropertyInfo.DisplayName + " value is required");
                    }
                }
                else
                {
                    throw new Exception(fPropertyInfo.DisplayName + " value is required");
                }
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            // don't update when the string is empty if the original value was null.
            if (jProperty.Value != null)
                fPropertyInfo.SetValue(null, jProperty.Value.ToString().Trim());
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (element != null)
                fPropertyInfo.SetValue(null, element.InnerText);
        }

        public override JToken GetPropertyViewModel()
        {
            string value = "";

            // most controls simply use the value.ToString() to get the data.
            // Override this method only when that is not the case.
            object vValue = fPropertyInfo.GetValue();

            if (vValue != null)
            {
                value = vValue.ToString();

                if (string.IsNullOrEmpty(value) &&
                    !string.IsNullOrEmpty(_defaultValue))
                {
                    // use the default value defined in the value attribute of the input control
                    value = _defaultValue;
                }
            }

            return value;
        }

        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode() ;
            HtmlNode propertyNode;
            if (IsReadOnly)
            {
                propertyNode = CreateReadOnlyControl();

                container.AppendChild(propertyNode);
            }
            else
            {
                // label element

                if (fPropertyInfo.IsMultipleLined)
                {
                    if (fPropertyInfo.ShowUpdateHistory)
                    {
                        // label element
                        HtmlNode groupNode = this.fOwnerEditor.Document.CreateElement("div");
                        groupNode.SetAttributeValue("class", "input-group");
                        container.AppendChild(groupNode);

                        fTextControl = this.fOwnerEditor.Document.CreateElement("textarea");
                        fTextControl.SetAttributeValue("class", "form-control");
                        fTextControl.SetAttributeValue("rows", fPropertyInfo.Rows.ToString());
                        groupNode.AppendChild(fTextControl);

                        // appending an selectable icon at end

                        HtmlNode addonNode = this.fOwnerEditor.Document.CreateElement("span");
                        addonNode.SetAttributeValue("class", "input-group-addon");
                        groupNode.AppendChild(addonNode);

                        HtmlNode linkNode = this.fOwnerEditor.Document.CreateElement("a");
                        linkNode.SetAttributeValue("data-ui-sref", GetViewChangeLogURL());
                        addonNode.AppendChild(linkNode);

                        HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                        iNode.SetAttributeValue("class", "icon-append fa fa-ellipsis-h");

                        linkNode.AppendChild(iNode);

                        ToolTipToStatus(container);
                    }
                    else
                    {
                        fTextControl = this.fOwnerEditor.Document.CreateElement("textarea");
                        fTextControl.SetAttributeValue("class", "form-control");
                        fTextControl.SetAttributeValue("rows", fPropertyInfo.Rows.ToString());
                        container.AppendChild(fTextControl);
                    }
                }
                else if (fPropertyInfo.ShowUpdateHistory)
                {
                    // label element
                    HtmlNode groupNode = this.fOwnerEditor.Document.CreateElement("div");
                    groupNode.SetAttributeValue("class", "input-group");
                    container.AppendChild(groupNode);

                    fTextControl = this.fOwnerEditor.Document.CreateElement("input");
                    fTextControl.SetAttributeValue("class", "form-control");
                    fTextControl.SetAttributeValue("type", "text");
                    fTextControl.SetAttributeValue("maxlength", fPropertyInfo.MaxLength.ToString());
                    groupNode.AppendChild(fTextControl);

                    if (fPropertyInfo.HasConstraint)
                    {
                        if (fPropertyInfo.Constraint is PatternElement)
                        {
                            fTextControl.SetAttributeValue("pattern", ((PatternElement)fPropertyInfo.Constraint).PatternValue);
                        }
                    }

                    // appending an selectable icon at end

                    HtmlNode addonNode = this.fOwnerEditor.Document.CreateElement("span");
                    addonNode.SetAttributeValue("class", "input-group-addon");
                    groupNode.AppendChild(addonNode);

                    HtmlNode linkNode = this.fOwnerEditor.Document.CreateElement("a");
                    linkNode.SetAttributeValue("data-ui-sref", GetViewChangeLogURL());
                    addonNode.AppendChild(linkNode);

                    HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                    iNode.SetAttributeValue("class", "icon-append fa fa-ellipsis-h");

                    linkNode.AppendChild(iNode);

                    ToolTipToStatus(container);
                }
                else
                {
                    fTextControl = this.fOwnerEditor.Document.CreateElement("input");
                    fTextControl.SetAttributeValue("class", "form-control");
                    fTextControl.SetAttributeValue("type", "text");
                    fTextControl.SetAttributeValue("maxlength", fPropertyInfo.MaxLength.ToString());

                    if (fPropertyInfo.HasConstraint)
                    {
                        if (fPropertyInfo.Constraint is PatternElement)
                        {
                            fTextControl.SetAttributeValue("pattern", ((PatternElement)fPropertyInfo.Constraint).PatternValue);
                        }
                    }

                    container.AppendChild(fTextControl);

                    ToolTipToStatus(container);
                }

                // set the angularjs  model binding
                if (fPropertyInfo.IsRequired)
                {
                    if (!fOwnerEditor.IsInlineForm)
                    {
                        fTextControl.SetAttributeValue("required", "");
                    }
                    else
                    {
                        fTextControl.SetAttributeValue("ng-required", "!$last");
                    }
                }

                fTextControl.SetAttributeValue("name", PropertyControlName);
                fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
                fTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                if (fPropertyInfo.InvokeCallback)
                {
                    fTextControl.SetAttributeValue("ng-blur", "reloadInstance('" + fPropertyInfo.Name + "')");
                }

                this.CreateValidators(container);
            }

            return container;

        }  // CreateChilidControls

        private string GetViewChangeLogURL()
        {
            string url = ".viewlog";

            string schemaName = this.fOwnerEditor.EditInstance.DataView.SchemaInfo.Name;
            string className = this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName;
            string oid = this.fOwnerEditor.EditInstance.InstanceData.ObjId;
            if (string.IsNullOrEmpty(oid))
            {
                oid = "unknown";
            }
    
            url += "({logschema: '" + schemaName + "', logclass: '" + className + "', logoid: '" + oid + "', logproperty: '" + fPropertyInfo.Name + "'})";
            return url;
        }

    }  // class StringProperty

    //---- CLASS RichTextProperty ---------------------------------------------------------------------
    /// <remarks>
    /// RichTextProperty class is for rich text properties.
    /// </remarks>
    public class RichTextProperty : BaseTextBoxProperty
    {
        public RichTextProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            // don't update when the string is empty if the original value was null.
            if (jProperty.Value != null)
                fPropertyInfo.SetValue(null, jProperty.Value.ToString().Trim());
        }

        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();
            HtmlNode propertyNode;
            if (IsReadOnly)
            {
                propertyNode = this.fOwnerEditor.Document.CreateElement("div");
                propertyNode.SetAttributeValue("class", "well");
                object vValue = PropertyInfo.GetValue();

                if (vValue != null)
                {              
                    propertyNode.SetAttributeValue("ng-bind-html", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                    propertyNode.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                }
                else
                {
                    propertyNode.SetAttributeValue("ng-bind-html", "");
                }
                container.AppendChild(propertyNode);
            }
            else
            {
                propertyNode = this.fOwnerEditor.Document.CreateElement("summernote");
                propertyNode.SetAttributeValue("config", "options");
                if (fPropertyInfo.IsRequired)
                {
                    propertyNode.SetAttributeValue("required", "");
                }

                propertyNode.SetAttributeValue("name", PropertyControlName);
                propertyNode.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
                propertyNode.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);

                container.AppendChild(propertyNode);

                this.CreateValidators(container);
            }

            return container;

        }  // CreateChilidControls

    }  // class RichTextProperty

    //---- CLASS NumericProperty--------------------------------------------------------------------
    /// <remarks>
    /// NumericProperty class is supports all intrinsic numeric types:
    /// Byte, Int16, Int32, Int64, SByte, UInt16, UInt32, UInt64,
    /// Decimal, Single, Double.
    /// It also handles the type "Char".
    /// It determines the type from fPropertyInfo.PropertyType and uses that to establish
    /// translation between value and control in ApplyValueInternal() and UpdateValueInteral().
    /// All use a TextBox for an editor. The textbox is formatted with a limited width and MaxChars
    /// depending on the type.
    /// </remarks>
    public class NumericProperty : BaseTextBoxProperty
	{
        public NumericProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
		}

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            if (jProperty.Value != null && !string.IsNullOrEmpty(jProperty.Value.ToString()) &&
                jProperty.Value.ToString() != "null") // numbers must have a value assigned first
            {
                string pValue = jProperty.Value.ToString().Trim();
                object vValue = ConvertToNumeric(pValue);

                fPropertyInfo.SetValue(null, vValue);
            }  // if 
            else
            {
                fPropertyInfo.SetValue(null, "");
            }
        }

        /// <summary>
        /// Update the instance with the value in a XmlElement object
        /// </summary>
        /// <param name="jProperty">the JProperty object</param>
        /// <param name="isFormFormat">whether the value is in the form format</param>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (element != null && !string.IsNullOrEmpty(element.InnerText))
            { 
                object vValue = ConvertToNumeric(element.InnerText);

                fPropertyInfo.SetValue(null, vValue);
            }
            else
            {
                fPropertyInfo.SetValue(null, "");
            }
        }

        public override JToken GetPropertyViewModel()
        {
            // most controls simply use the value.ToString() to get the data.
            // Override this method only when that is not the case.
            object pValue = fPropertyInfo.GetValue();
            if (pValue != null && !string.IsNullOrEmpty(pValue.ToString()) &&
                pValue.ToString() != "null")
            {
                if (fPropertyInfo.PropertyType == typeof(int))
                {
                    return Convert.ToInt32(pValue);
                }
                else if (fPropertyInfo.PropertyType == typeof(Int64))
                {
                    return Convert.ToInt64(pValue);
                }
                else if (fPropertyInfo.PropertyType == typeof(float))
                {
                    return Convert.ToSingle(pValue);
                }
                else if (fPropertyInfo.PropertyType == typeof(double))
                {
                    return Convert.ToDouble(pValue);
                }
                else if (fPropertyInfo.PropertyType == typeof(decimal))
                {
                    return Convert.ToDecimal(pValue);
                }
                else if (fPropertyInfo.PropertyType == typeof(byte))
                {
                    return Convert.ToByte(pValue);
                }
                else
                    throw new Exception("Unknown data type " + fPropertyInfo.PropertyType.Name);
            }
            else
                return "";
        }

        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();
            HtmlNode propertyNode;

            if (IsReadOnly)
            {
                propertyNode = CreateReadOnlyControl();
                container.AppendChild(propertyNode);
            }
            else
            {
                fTextControl = this.fOwnerEditor.Document.CreateElement("input");
                fTextControl.SetAttributeValue("class", "form-control");
                fTextControl.SetAttributeValue("type", "number");
                fTextControl.SetAttributeValue("step", "any");

                if (fPropertyInfo.HasConstraint)
                {
                    if (fPropertyInfo.Constraint is RangeElement)
                    {
                        fTextControl.SetAttributeValue("min", ((RangeElement)fPropertyInfo.Constraint).MinValue);
                        fTextControl.SetAttributeValue("max", ((RangeElement)fPropertyInfo.Constraint).MaxValue);
                    }
                }

                container.AppendChild(fTextControl);

                // set the angularjs  model binding
                if (fPropertyInfo.IsRequired)
                {
                    if (!fOwnerEditor.IsInlineForm)
                    {
                        fTextControl.SetAttributeValue("required", "");
                    }
                    else
                    {
                        fTextControl.SetAttributeValue("ng-required", "!$last");
                    }
                }

                // set the angularjs  model binding
                fTextControl.SetAttributeValue("name", PropertyControlName);
                fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
                //fTextControl.SetAttributeValue("string-to-number", "");
                fTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                if (fPropertyInfo.InvokeCallback)
                {
                    fTextControl.SetAttributeValue("ng-blur", "reloadInstance('" + fPropertyInfo.Name + "')");
                }

                //ToolTipToStatus(container);
            }

            this.CreateValidators(container);
            return container;
        }  // CreateChilidControls

        private object ConvertToNumeric(string pValue)
        {
            object vValue = null;

            if (fPropertyInfo.PropertyType == typeof(int))
            {
                vValue = (object)Convert.ToInt32(pValue);
            }
            else if (fPropertyInfo.PropertyType == typeof(Int64))
            {
                vValue = (object)Convert.ToInt64(pValue);
            }
            else if (fPropertyInfo.PropertyType == typeof(float))
            {
                vValue = (object)Convert.ToSingle(pValue);
            }
            else if (fPropertyInfo.PropertyType == typeof(double))
            {
                vValue = (object)Convert.ToDouble(pValue);
            }
            else if (fPropertyInfo.PropertyType == typeof(decimal))
            {
                vValue = (object)Convert.ToDecimal(pValue);
            }
            else if (fPropertyInfo.PropertyType == typeof(byte))
            {
                vValue = (object)Convert.ToByte(pValue);
            }

            return vValue;
        }

	}  // class NumericProperty

	//---- CLASS DateProperty --------------------------------------------------------------------
	/// <remarks>
	/// DateProperty class supports the Date type
	/// </remarks>
	public class DateProperty : BaseTextBoxProperty
	{
        public DateProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
		}

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            try
            {
                if (jProperty.Value != null && jProperty.Value.ToString() != "")
                {
                    string sDateTime = jProperty.Value.ToString();

                    // SYS_TIME is a special value for the DateProperty which
                    // means property value will be set to the current system time.
                    if (sDateTime != SimpleAttributeElement.SYS_TIME)
                    {
                        fPropertyInfo.SetValue(null, Convert.ToDateTime(sDateTime));
                    }
                    else
                    {
                        fPropertyInfo.SetValue(null, DateTime.Now);
                    }
                }
                else
                {
                    fPropertyInfo.SetValue(null, null);
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            try
            {
                if (element != null && !string.IsNullOrEmpty(element.InnerText))
                {
                    string sDateTime = element.InnerText;

                    // SYS_TIME is a special value for the DateProperty which
                    // means property value will be set to the current system time.
                    if (sDateTime != SimpleAttributeElement.SYS_TIME)
                    {
                        fPropertyInfo.SetValue(null, Convert.ToDateTime(sDateTime));
                    }
                    else
                    {
                        fPropertyInfo.SetValue(null, DateTime.Now);
                    }
                }
                else
                {
                    fPropertyInfo.SetValue(null, null);
                }
            }
            catch (Exception)
            { }
        }

        public override JToken GetPropertyViewModel()
        {
            string jValue = "";
            object val = fPropertyInfo.GetValue();
            if (val != null && val is DateTime)
            {
                DateTime vDateTime = (DateTime)val;

                jValue = vDateTime.ToString("yyyy-MM-dd");
            }

            return jValue;
        }

        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();
            HtmlNode propertyNode;

            if (IsReadOnly)
            {
                propertyNode = CreateReadOnlyControl();
                container.AppendChild(propertyNode);
            }
            else
            {
                HtmlNode groupNode = null;
                bool hasIcon = false;
                if (!this.fOwnerEditor.IsInlineForm && hasIcon)
                {
                    groupNode = this.fOwnerEditor.Document.CreateElement("div");
                    groupNode.SetAttributeValue("class", "input-group");
                    container.AppendChild(groupNode);
                }

                fTextControl = this.fOwnerEditor.Document.CreateElement("input");
                fTextControl.SetAttributeValue("class", "form-control datepicker");
                fTextControl.SetAttributeValue("type", "text");
                //fTextControl.SetAttributeValue("data-smart-datepicker", "");
                //fTextControl.SetAttributeValue("data-date-format", "yy-mm-dd");
                fTextControl.SetAttributeValue("data-datetimepicker", "");
                fTextControl.SetAttributeValue("datetimepicker-options", "{format: 'YYYY-MM-DD', useCurrent: true, showTodayButton: true, showClose:true}");
                if (!this.fOwnerEditor.IsInlineForm && hasIcon)
                {
                    groupNode.AppendChild(fTextControl);
                }
                else
                {
                    container.AppendChild(fTextControl);
                }

                // appending an icon at end if it is not inline form
                if (!this.fOwnerEditor.IsInlineForm && hasIcon)
                {
                    HtmlNode iconNode = this.fOwnerEditor.Document.CreateElement("span");
                    iconNode.SetAttributeValue("class", "input-group-addon");
                    groupNode.AppendChild(iconNode);

                    HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                    iNode.SetAttributeValue("class", "icon-append fa fa-calendar");
                    iconNode.AppendChild(iNode);
                }

                // set the angularjs  model binding
                if (fPropertyInfo.IsRequired)
                {
                    if (!fOwnerEditor.IsInlineForm)
                    {
                        fTextControl.SetAttributeValue("required", "");
                    }
                    else
                    {
                        fTextControl.SetAttributeValue("ng-required", "!$last");
                    }
                }

                fTextControl.SetAttributeValue("name", PropertyControlName);
                fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
                fTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                if (fPropertyInfo.InvokeCallback)
                {
                    fTextControl.SetAttributeValue("ng-change", "reloadInstance('" + fPropertyInfo.Name + "')");
                }

                ToolTipToStatus(container);
            }

            this.CreateValidators(container);
            return container;

        }  // CreateChilidControls
    }  // class DateProperty

    //---- CLASS DateTimeProperty --------------------------------------------------------------------
    /// <remarks>
    /// DateTimeProperty class supports the DateTime class.
    /// </remarks>
    public class DateTimeProperty : BaseTextBoxProperty
    {
        public DateTimeProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            try
            {
                if (jProperty.Value != null && jProperty.Value.ToString() != "")
                {
                    string sDateTime = jProperty.Value.ToString();

                    // SYS_TIME is a special value for the DateProperty which
                    // means property value will be set to the current system time.
                    if (sDateTime != SimpleAttributeElement.SYS_TIME)
                    {
                        fPropertyInfo.SetValue(null, Convert.ToDateTime(sDateTime));
                    }
                    else
                    {
                        fPropertyInfo.SetValue(null, DateTime.Now);
                    }
                }
                else
                {
                    fPropertyInfo.SetValue(null, null);
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            try
            {
                if (element != null && !string.IsNullOrEmpty(element.InnerText))
                {
                    string sDateTime = element.InnerText;

                    // SYS_TIME is a special value for the DateProperty which
                    // means property value will be set to the current system time.
                    if (sDateTime != SimpleAttributeElement.SYS_TIME)
                    {
                        fPropertyInfo.SetValue(null, Convert.ToDateTime(sDateTime));
                    }
                    else
                    {
                        fPropertyInfo.SetValue(null, DateTime.Now);
                    }
                }
                else
                {
                    fPropertyInfo.SetValue(null, null);
                }
            }
            catch (Exception)
            { }
        }

        public override JToken GetPropertyViewModel()
        {
            string jValue = "";
            object val = fPropertyInfo.GetValue();
            if (val != null && val is DateTime)
            {
                DateTime vDateTime = (DateTime)val;

                jValue = vDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return jValue;
        }

        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();
            HtmlNode propertyNode;

            if (IsReadOnly)
            {
                propertyNode = CreateReadOnlyControl();
                container.AppendChild(propertyNode);
            }
            else
            {
                HtmlNode groupNode = null;
                bool hasIcon = false;
                if (!this.fOwnerEditor.IsInlineForm && hasIcon)
                {
                    groupNode = this.fOwnerEditor.Document.CreateElement("div");
                    groupNode.SetAttributeValue("class", "input-group");
                    container.AppendChild(groupNode);
                }

                fTextControl = this.fOwnerEditor.Document.CreateElement("input");
                fTextControl.SetAttributeValue("class", "form-control");
                fTextControl.SetAttributeValue("type", "text");
                fTextControl.SetAttributeValue("data-datetimepicker", "");
                fTextControl.SetAttributeValue("datetimepicker-options", "{format: 'YYYY-MM-DD HH:mm:ss', useCurrent: true, showTodayButton: true, showClose:true}");
                if (!this.fOwnerEditor.IsInlineForm && hasIcon)
                {
                    groupNode.AppendChild(fTextControl);
                }
                else
                {
                    container.AppendChild(fTextControl);
                }

                // appending an icon at end if it is not inline form
                if (!this.fOwnerEditor.IsInlineForm && hasIcon)
                {
                    HtmlNode iconNode = this.fOwnerEditor.Document.CreateElement("span");
                    iconNode.SetAttributeValue("class", "input-group-addon");
                    groupNode.AppendChild(iconNode);

                    HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                    iNode.SetAttributeValue("class", "icon-append fa fa-calendar");
                    iconNode.AppendChild(iNode);
                }

                // set the angularjs  model binding
                if (fPropertyInfo.IsRequired)
                {
                    if (!fOwnerEditor.IsInlineForm)
                    {
                        fTextControl.SetAttributeValue("required", "");
                    }
                    else
                    {
                        fTextControl.SetAttributeValue("ng-required", "!$last");
                    }
                }

                fTextControl.SetAttributeValue("name", PropertyControlName);
                fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
                fTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                if (fPropertyInfo.InvokeCallback)
                {
                    fTextControl.SetAttributeValue("ng-blur", "reloadInstance('" + fPropertyInfo.Name + "')");
                }

                ToolTipToStatus(container);
            }

            this.CreateValidators(container);
            return container;

        }  // CreateChilidControls
    }  // class DateTimeProperty

    //---- CLASS VirtualProperty ---------------------------------------------------------------------
    /// <remarks>
    /// VirtualProperty class is for Virtual attibute with string type.
    /// </remarks>
    public class VirtualProperty : BaseProperty
    {
        public VirtualProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
        }

        public override JToken GetPropertyViewModel()
        {
            string value = "";

            // most controls simply use the value.ToString() to get the data.
            // Override this method only when that is not the case.
            object vValue = fPropertyInfo.GetValue();

            if (vValue != null)
                value = vValue.ToString();

            return value;
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            // doing nothing for virtual property
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            // doing nothing for virtual property
        }

        public override HtmlNode CreatePropertyNode()
        {
            // create a property container for all types of properties
            HtmlNode container = this.fOwnerEditor.Document.CreateElement("div");

            Regex tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
            // if the value of virtual property contains html, create a form-control-static control
            // otherwise, create a readonly input control
            object vValue = fPropertyInfo.GetValue();
            bool hasTag = false;
            if (vValue != null)
            {
                string value = vValue.ToString();

                hasTag = tagRegex.IsMatch(value);
            }

            if (hasTag || fPropertyInfo.IsHtmlValue)
            {
                // compile the value either it cantains tag or the property value is html
                HtmlNode pNode = this.fOwnerEditor.Document.CreateElement("p");
                pNode.SetAttributeValue("class", "form-control-static");
                //pNode.SetAttributeValue("name", PropertyControlName);

                pNode.SetAttributeValue("dynamic", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                container.AppendChild(pNode);
            }
            else
            {
                // create a readonly input control
                HtmlNode inputNode = CreateReadOnlyControl();
                container.AppendChild(inputNode);
            }

            return container;

        }  // CreateChilidControls

    }  // class VirtualProperty

    //---- CLASS RelationshipProperty --------------------------------------------------------------------
    /// <remarks>
    /// RelationshipProperty class supports the relationship attribute. This will raise exceptions
    /// when the user incorrectly formats the text. (Exceptions are expected to be handled by the caller.)
    /// </remarks>
    public class RelationshipProperty : BaseTextBoxProperty
    {
        private const string ParameterPattern = @"\{[^\}]+\}";
        private IInstanceEditor _ownerControl;

        public RelationshipProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
            _ownerControl = (IInstanceEditor)pOwnerControl;
        }

        public override JToken GetPropertyViewModel()
        {
            string value = "";

            // most controls simply use the value.ToString() to get the data.
            // Override this method only when that is not the case.
            object vValue = fPropertyInfo.GetValue();

            if (vValue != null)
                value = vValue.ToString();

            return value;
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            if (jProperty.Value != null && !string.IsNullOrEmpty(jProperty.Value.ToString()))
            {
                string pkValue = jProperty.Value.ToString();

                DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute)_ownerControl.EditInstance.DataView.ResultAttributes[fPropertyInfo.Name];
                string[] pkValues = pkValue.Split('&');
                DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
                if (primaryKeys != null)
                {
                    int index = 0;
                    foreach (DataSimpleAttribute pk in primaryKeys)
                    {
                        if (index < pkValues.Length && pkValues[index] != null)
                        {
                            // to set a pk value, the name combines that of relationship attribute and primary key
                            _ownerControl.EditInstance.InstanceData.SetAttributeValue(fPropertyInfo.Name + "_" + pk.Name, pkValues[index].Trim());
                        }
                        index++;
                    }
                }
            }
        }  // UpdateValueInternal()

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (element != null && !string.IsNullOrEmpty(element.InnerText))
            {
                string pkValue = element.InnerText;

                DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute)_ownerControl.EditInstance.DataView.ResultAttributes[fPropertyInfo.Name];
                string[] pkValues = pkValue.Split('&');
                DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
                if (primaryKeys != null)
                {
                    int index = 0;
                    foreach (DataSimpleAttribute pk in primaryKeys)
                    {
                        if (index < pkValues.Length && pkValues[index] != null)
                        {
                            // to set a pk value, the name combines that of relationship attribute and primary key
                            _ownerControl.EditInstance.InstanceData.SetAttributeValue(fPropertyInfo.Name + "_" + pk.Name, pkValues[index].Trim());
                        }
                        index++;
                    }
                }
            }
        }  // UpdateValueInternal()


        // create a text box and an image link to launch an instance picker
        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();
            HtmlNode propertyNode;

            if (IsReadOnly)
            {
                propertyNode = CreateReadOnlyControl();
                container.AppendChild(propertyNode);
            }
            else
            {
                // label element
                HtmlNode groupNode = this.fOwnerEditor.Document.CreateElement("div");
                groupNode.SetAttributeValue("class", "input-group");
                container.AppendChild(groupNode);

                fTextControl = this.fOwnerEditor.Document.CreateElement("input");
                fTextControl.SetAttributeValue("class", "form-control");
                fTextControl.SetAttributeValue("type", "text");
                groupNode.AppendChild(fTextControl);

                // appending an selectable icon at end

                HtmlNode addonNode = this.fOwnerEditor.Document.CreateElement("span");
                addonNode.SetAttributeValue("class", "input-group-addon");
                groupNode.AppendChild(addonNode);

                HtmlNode linkNode = this.fOwnerEditor.Document.CreateElement("a");
                linkNode.SetAttributeValue("data-ui-sref", GetRelatedClassURL());
                //linkNode.SetAttributeValue("href-void", "");
                addonNode.AppendChild(linkNode);

                HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
                iNode.SetAttributeValue("class", "icon-append fa fa-external-link");

                linkNode.AppendChild(iNode);

                // set the angularjs  model binding
                if (fPropertyInfo.IsRequired)
                {
                    if (!fOwnerEditor.IsInlineForm)
                    {
                        fTextControl.SetAttributeValue("required", "");
                    }
                    else
                    {
                        fTextControl.SetAttributeValue("ng-required", "!$last");
                    }
                }

                fTextControl.SetAttributeValue("name", PropertyControlName);
                fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
                fTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
                if (fPropertyInfo.InvokeCallback)
                {
                    fTextControl.SetAttributeValue("ng-blur", "reloadInstance('" + fPropertyInfo.Name + "')");
                }

                //ToolTipToStatus(container);
            }

            this.CreateValidators(container);
            return container;
        }  // CreatePropertyNode


        private string GetRelatedClassURL()
        {
            string url = ".pickpk";

            // Find out the class related by the relationship attribute
            SchemaModel schemaModel = this._ownerControl.EditInstance.DataView.SchemaModel;
            string ownerClassName = this._ownerControl.EditInstance.DataView.BaseClass.ClassName;
            ClassElement classElement = this._ownerControl.EditInstance.DataView.SchemaModel.FindClass(ownerClassName);
            RelationshipAttributeElement relationshipElement = classElement.FindInheritedRelationshipAttribute(fPropertyInfo.Name);
            string relatedClassName = relationshipElement.LinkedClass.Name;

            string filterExpression = GetFilterExpression(relationshipElement);

            url += "({pkclass: '" + relatedClassName + "', property: '" + fPropertyInfo.Name + "', filter: '" + filterExpression + "', callback: " + (fPropertyInfo.InvokeCallback? "true" : "false") + "})";
            return url;
        }

        private string GetFilterExpression(RelationshipAttributeElement relationshipElement)
        {
            string filterExpression = null;
            if (!string.IsNullOrEmpty(relationshipElement.FilterExpression))
            {
                filterExpression = relationshipElement.FilterExpression;
                string pattern = relationshipElement.FilterExpression;
                Regex patternExp = new Regex(RelationshipProperty.ParameterPattern);

                MatchCollection matches = patternExp.Matches(pattern);
                if (matches.Count > 0)
                {
                    // contains variables
                    string propertyName;
                    string propertyValue;

                    foreach (Match match in matches)
                    {
                        if (match.Value.Length > 2)
                        {
                            // parameter is in form of {propertyName} or {class:propertyName}
                            propertyName = match.Value.Substring(1, (match.Value.Length - 2));
                            try
                            {
                                if (propertyName == "class:Description")
                                {
                                    // property name represents a property of a class
                                    string className = this._ownerControl.EditInstance.DataView.BaseClass.ClassName;
                                    ClassElement classElement = this._ownerControl.EditInstance.DataView.SchemaModel.FindClass(className);
                                    if (classElement != null && !string.IsNullOrEmpty(classElement.Description))
                                    {
                                        // replace the parameter with value
                                        filterExpression = filterExpression.Replace(match.Value, classElement.Description);
                                    }
                                    else
                                    {
                                        // replace the parameter with empty string
                                        filterExpression = filterExpression.Replace(match.Value, "");
                                    }
                                }
                                else
                                {
                                    // property name represents a property of an instance
                                    propertyValue = this._ownerControl.EditInstance.InstanceData.GetAttributeStringValue(propertyName);
                                    if (!string.IsNullOrEmpty(propertyValue))
                                    {
                                        // replace the parameter with value
                                        filterExpression = filterExpression.Replace(match.Value, propertyValue);
                                    }
                                    else
                                    {
                                        // replace the parameter with empty string
                                        filterExpression = filterExpression.Replace(match.Value, "");
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // ignore
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(filterExpression))
            {
                // esacpe special chars in filter

                filterExpression = filterExpression.Replace("'", "%");
                return filterExpression;
            }
            else
            {
                return "";
            }
        }
    }  // class RelationshipProperty

    //---- CLASS CustomizedUIProperty --------------------------------------------------------------------
    /// <remarks>
    /// CustomizedUIProperty class is used for implementing a customized web control for a property
    /// </remarks>
    public class CustomizedUIProperty : BaseProperty
    {
        private IInstanceEditor _ownerControl;
        private IPropertyControl _customizedControl;

        public CustomizedUIProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
            _ownerControl = pOwnerControl;
            
            _customizedControl = CreateCustomizedControl(instanceView, pPropertyInfo);
            _customizedControl.InstanceEditor = _ownerControl;
            _customizedControl.BaseInstanceView = this.BaseInstanceView;
            _customizedControl.PropertyInfo = pPropertyInfo;
            _customizedControl.IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// Check if the jproperty contains non-empty value for the property
        /// </summary>
        /// <param name="jProperty"></param>
        /// <returns></returns>
        public override bool HasValue(JProperty jProperty)
        {
            return _customizedControl.HasValue(jProperty);
        }

        /// <summary>
        /// Check if an xml element contains non-empty value for the property
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool HasValue(XmlElement element)
        {
            return _customizedControl.HasValue(element);
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            _customizedControl.UpdateValueInternal(jProperty, true);
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement elememnt, bool isFormFormat)
        {
            _customizedControl.UpdateValueInternal(elememnt, false);
        }

        /// <summary>
        /// Gets model instances from an array of ViewModel instances when jProperty's value is an JArray.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public override List<IInstanceEditor> GetInstanceEditors(JProperty jProperty)
        {
            return _customizedControl.GetInstanceEditors(jProperty);
        }

        /// <summary>
        /// Gets model instances from the child elements of an xml element representing a wrapper of related instances.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public override List<IInstanceEditor> GetInstanceEditors(XmlElement element)
        {
            return _customizedControl.GetInstanceEditors(element);
        }

        public override JToken GetPropertyViewModel()
        {
            return _customizedControl.GetPropertyViewModel();
        }

        public override HtmlNode CreatePropertyNode()
        {
            return _customizedControl.CreatePropertyNode();
        }  // CreatePropertyNode()

        private IPropertyControl CreateCustomizedControl(InstanceView instanceView, InstanceAttributePropertyDescriptor propertyInfo)
        {
            IPropertyControl customizedControl = null;
            string libName = propertyInfo.UIControlCreator;

            if (!string.IsNullOrEmpty(libName))
            {
                int index = libName.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = libName.Substring(0, index).Trim();
                    assemblyName = libName.Substring(index + 1).Trim();
                }
                else
                {
                    className = libName.Trim();
                }

                try
                {

                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    customizedControl = (IPropertyControl)obj.Unwrap();
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to create a control for peroperty " + propertyInfo.DisplayName + " in the class " + instanceView.DataView.BaseClass.Caption + " with the lib " + libName + " with error " + ex.Message);
                }
            }

            return customizedControl;
        }

    }  // class CustomizedUIProperty

    //---- CLASS ImageProperty --------------------------------------------------------------------
    /// <remarks>
    /// ImageProperty class supports the image attribute.
    /// </remarks>
    public class ImageProperty : BaseProperty
    {
        private const string BlankImageUrl = @"styles/img/blank.jpg";
        private IInstanceEditor _ownerControl;

        public ImageProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
            _ownerControl = (IInstanceEditor)pOwnerControl;
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            // Save the image id
            if (jProperty.Value != null)
            {
                fPropertyInfo.SetValue(null, jProperty.Value.ToString().Trim());
            }
        }  // UpdateValueInternal()

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            // Save the image id
            if (element != null && !string.IsNullOrEmpty(element.InnerText))
            {
                fPropertyInfo.SetValue(null, element.InnerText.Trim());
            }
        }  // UpdateValueInternal()

        // create a image panel and buttons to upload and clear image
        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();

            HtmlNode tableNode = this.fOwnerEditor.Document.CreateElement("table");
            tableNode.SetAttributeValue("class", "table table-bordered");
            container.AppendChild(tableNode);

            // table header
            HtmlNode theadNode = this.fOwnerEditor.Document.CreateElement("tbody");
            tableNode.AppendChild(theadNode);

            HtmlNode trNode = this.fOwnerEditor.Document.CreateElement("tr");
            theadNode.AppendChild(trNode);

            HtmlNode tdNode = this.fOwnerEditor.Document.CreateElement("td");
            tdNode.SetAttributeValue("align", "center");

            trNode.AppendChild(tdNode);

            HtmlNode imgNode = this.fOwnerEditor.Document.CreateElement("img");
            imgNode.SetAttributeValue("width", fPropertyInfo.ImageMaximumWidth.ToString());
            imgNode.SetAttributeValue("height", fPropertyInfo.ImageMaximumHeight.ToString());
            imgNode.SetAttributeValue("ng-src", "{{getImageUrl('" + fPropertyInfo.Name + "')}}");
            imgNode.SetAttributeValue("id", $"Image_{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
            tdNode.AppendChild(imgNode);

            if (!this.IsReadOnly)
            {
                trNode = this.InstanceEditor.Document.CreateElement("tr");
                trNode.SetAttributeValue("ng-show", "showImageEditButtons()");
                theadNode.AppendChild(trNode);

                tdNode = this.InstanceEditor.Document.CreateElement("td");
                tdNode.SetAttributeValue("align", "center");
                trNode.AppendChild(tdNode);

                string text = WebControlsResourceManager.GetString("PropertyEditor.UploadImageText");

                HtmlNode btnNode = CreateUploadBtnElement(text);

                tdNode.AppendChild(btnNode);

                HtmlNode textNode = this.InstanceEditor.Document.CreateTextNode("&nbsp;");

                tdNode.AppendChild(textNode);

                text = WebControlsResourceManager.GetString("PropertyEditor.ClearImageText");

                btnNode = CreateClearBtnElement(text);

                tdNode.AppendChild(btnNode);
            }

            return container;
        }  // CreatePropertyNode

        private HtmlNode CreateUploadBtnElement(string text)
        {
            HtmlNode btn = this.InstanceEditor.Document.CreateElement("a");
            string url = ".uploadimage";
            url += "({property: '" + fPropertyInfo.Name + "', imageid: getImageId('" + fPropertyInfo.Name + "')})";

            btn.SetAttributeValue("data-ui-sref", url);
            btn.SetAttributeValue("class", "btn btn-primary");
            btn.InnerHtml = text;

            return btn;
        }

        private HtmlNode CreateClearBtnElement(string text)
        {
            HtmlNode btn = this.InstanceEditor.Document.CreateElement("a");
            btn.SetAttributeValue("ng-click", "ClearImage('" + fPropertyInfo.Name + "')");
            btn.SetAttributeValue("class", "btn btn-danger");
            btn.InnerHtml = text;

            return btn;
        }

        /// <summary>
        /// Gets image url
        /// </summary>
        private string GetImageUrl()
        {
            string imageId = fPropertyInfo.GetStringValue();

            if (!string.IsNullOrEmpty(imageId))
            {
                string imagePath = NewteraNameSpace.GetUserImageDir() + imageId;
                if (File.Exists(imagePath))
                {
                    // create an url that binds img control to the model
                    return NewteraNameSpace.USER_IMAGE_BASE_URL + "{{" + this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + "}}";
                }
                else
                {
                    // image not found
                    return BlankImageUrl;
                }
            }
            else
            {
                // no image
                return BlankImageUrl;
            }
        }
    }  // class ImageProperty


	//---- CLASS BoolProperty -----------------------------------------------------------------------
	/// <remarks>
	/// BoolProperty class supports boolean properties.
	/// It uses a drop list with the labels "Yes" and "No".
	/// </remarks>
	public class BoolProperty : BaseProperty
	{
        public BoolProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
		}

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            if (jProperty.Value != null && !string.IsNullOrEmpty(jProperty.Value.ToString()))
            {
                bool vBool = false;
                if (jProperty.Value.ToString() == "true")
                {
                    vBool = true;
                }

                fPropertyInfo.SetValue(null, vBool);
            }
            else
            {
                fPropertyInfo.SetValue(null, null);
            }
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (element != null && !string.IsNullOrEmpty(element.InnerText))
            {
                bool vBool = false;
                if (element.InnerText == "true")
                {
                    vBool = true;
                }

                fPropertyInfo.SetValue(null, vBool);
            }
            else
            {
                fPropertyInfo.SetValue(null, null);
            }
        }

        public override JToken GetPropertyViewModel()
        {
            string jValue = "";
            object val = fPropertyInfo.GetValue();

            if (val != null && val is Enum)
            {
                jValue = val.ToString();
            }

            return jValue;
        }

        public override HtmlNode CreatePropertyNode()
		{
            HtmlNode container = base.CreatePropertyNode();

            // appending a radio element for true
            HtmlNode labelNode = this.fOwnerEditor.Document.CreateElement("label");
            labelNode.SetAttributeValue("class", "radio radio-inline");
            container.AppendChild(labelNode);

            // an input element
            HtmlNode input = this.fOwnerEditor.Document.CreateElement("input");
            input.SetAttributeValue("type", "radio");
            input.SetAttributeValue("class", "radiobox style-0");
            input.SetAttributeValue("name", PropertyControlName);
            input.SetAttributeValue("value", "true");
            if (IsReadOnly)
            {
                input.SetAttributeValue("disabled", "disabled");
            }

            // set the angularjs  model binding
            input.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
            labelNode.AppendChild(input);

            HtmlNode spanNode = this.fOwnerEditor.Document.CreateElement("span");
            spanNode.InnerHtml = HtmlDocument.HtmlEncode(LocaleInfo.Instance.True);
            labelNode.AppendChild(spanNode);

            // appending a radio element for false
            labelNode = this.fOwnerEditor.Document.CreateElement("label");
            labelNode.SetAttributeValue("class", "radio radio-inline");
            container.AppendChild(labelNode);

            // an input element
            input = this.fOwnerEditor.Document.CreateElement("input");
            input.SetAttributeValue("type", "radio");
            input.SetAttributeValue("class", "radiobox style-0");
            input.SetAttributeValue("name", PropertyControlName);
            input.SetAttributeValue("value", "false");
            if (IsReadOnly)
            {
                input.SetAttributeValue("disabled", "disabled");
            }

            // set the angularjs  model binding
            input.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
            labelNode.AppendChild(input);

            spanNode = this.fOwnerEditor.Document.CreateElement("span");
            spanNode.InnerHtml = HtmlDocument.HtmlEncode(LocaleInfo.Instance.False);
            labelNode.AppendChild(spanNode);

            return container;

        }  // CreatePropertyNode

    }  // class BoolProperty

	//----- CLASS EnumProperty ----------------------------------------------------------------------
	/// <remarks>
	/// EnumProperty supports any enumerated type (based on the Enum class).
	/// It uses the power of the class System.Enum which defines all enumerated types.
	/// EnumProperty builds a drop list of all names and values in the class supplied in fPropertyInfo.PropertyType.
	/// The drop list will contain Name and Values, both taken from the Enum.System class.
	/// </remarks>
	public class EnumProperty : BaseProperty
	{
		private HtmlNode fEnumControl;

        public EnumProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
		}

        /// <summary>
        /// Validate the property value and throw an exception if there is an error
        /// </summary>
        public override void Validate()
        {
            object vValue = fPropertyInfo.GetValue();
            string value = null;

            if (vValue != null)
            {
                value = vValue.ToString();
            }

            // do nothing
            // set the angularjs  model binding
            if (fPropertyInfo.IsRequired)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(fPropertyInfo.DisplayName + " value is required");
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    object enumValue = Enum.Parse(fPropertyInfo.PropertyType, value);
                    if (enumValue == null)
                    {
                        throw new Exception(value + " is not a valid value for " + fPropertyInfo.DisplayName);
                    }
                }
                catch (Exception)
                {
                    throw new Exception(value + " is not a valid value for " + fPropertyInfo.DisplayName);
                }
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">the property in json data</param>
        /// <param name="isFormFormat">true if the json values are in form format where enum values are internal values</param>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            string val = jProperty.Value.ToString();

            try
            {
                Enum vEnum;
                if (isFormFormat)
                {
                    // convert integer to an enum value
                    vEnum = GetEnumValue(val, fPropertyInfo.PropertyType);
                }
                else
                {
                    // convert the string to an enum value
                    vEnum = Enum.Parse(fPropertyInfo.PropertyType, val) as Enum;
                }

                if (vEnum != null)
                {
                    fPropertyInfo.SetValue(null, vEnum);
                }
            }
            catch (Exception)
            {
                // may be caused by legacy values
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="element">an XmlElement object</param>
        /// <param name="isFormFormat">true if the json values are in form format where enum values are internal values</param>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            string val = element.InnerText;

            try
            {
                Enum vEnum;
                if (isFormFormat)
                {
                    // convert integer to an enum value
                    vEnum = GetEnumValue(val, fPropertyInfo.PropertyType);
                }
                else
                {
                    // convert the string to an enum value
                    vEnum = Enum.Parse(fPropertyInfo.PropertyType, val) as Enum;
                }

                if (vEnum != null)
                {
                    fPropertyInfo.SetValue(null, vEnum);
                }
            }
            catch (Exception)
            {
                // may be caused by legacy values
            }
        }

        /// <summary>
        /// Convert the model value to ViewModel value
        /// </summary>
        /// <returns></returns>
        public override JToken GetPropertyViewModel()
        {
            string jValue = null;
            object val = fPropertyInfo.GetValue();
            if (val != null && val is Enum)
            {
                Enum vEnum = (Enum) val;

                jValue = Convert.ChangeType(vEnum, vEnum.GetTypeCode()).ToString();

                if (jValue == "0")
                    jValue = ""; // first option is unknown, change the value from "0" to ''
            }

            return jValue;
        }

        public override HtmlNode CreatePropertyNode()
		{
            HtmlNode container = base.CreatePropertyNode();

            // label element
            HtmlNode groupNode = this.fOwnerEditor.Document.CreateElement("div");
            container.AppendChild(groupNode);

            fEnumControl = CreateEnumDropList(this.fOwnerEditor.Document, fPropertyInfo);

            // set the angularjs  model binding
            fEnumControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name);
            groupNode.AppendChild(fEnumControl);

            // the actual property value may not be one of the enum values. This could happen
            // if the Enum Constraint has been modified or values of a List Constraint is changed,
            // since the data instance have been saved to the database. In this case, we have to
            // create a dropdown list item for the property value so that it can display properly
            object val = fPropertyInfo.GetValue();
            if (val != null && val is string)
            {
                string valStr = (string)val;
                HtmlNode option = this.fOwnerEditor.Document.CreateElement("option");
                option.SetAttributeValue("value", valStr);
                option.SetAttributeValue("selected", "true"); // make the value selected
                option.InnerHtml = HtmlDocument.HtmlEncode(valStr);
                fEnumControl.AppendChild(option);
            }

            if (fPropertyInfo.InvokeCallback)
            {
                fEnumControl.SetAttributeValue("ng-change", "reloadInstance('" + fPropertyInfo.Name + "')");
            }

            if (!IsReadOnly)
            {
                this.CreateValidators(container);
            }

            return container;
		}  // CreatePropertyNode()

        /// <summary>
        /// CreateEnumDropList creates and fills in a select list with values from an Enum type.
        /// The ListEditItem.Name will be the Enum's name and the ListEditItem.Value will be its numeric value.
        /// It returns the HtmlNode.
        /// Its up to the caller to add it to the controls list (because its a Static method).
        /// </summary>
        private HtmlNode CreateEnumDropList(HtmlDocument doc, InstanceAttributePropertyDescriptor fPropertyInfo)
        {
            HtmlNode vEnumControl = doc.CreateElement("select");
            vEnumControl.SetAttributeValue("name", PropertyControlName);
            vEnumControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
            vEnumControl.SetAttributeValue("class", "form-control");
            if (IsReadOnly)
            {
                vEnumControl.SetAttributeValue("disabled", "disabled");
                vEnumControl.SetAttributeValue("style", "background-color:#f7f9f9;");

            }
            if (this.fOwnerEditor.IsInlineForm)
            {
                vEnumControl.SetAttributeValue("style", "width:auto;");
            }

            // set the angularjs  model binding
            if (fPropertyInfo.IsRequired)
            {
                if (!fOwnerEditor.IsInlineForm)
                {
                    vEnumControl.SetAttributeValue("required", "");
                }
                else
                {
                    vEnumControl.SetAttributeValue("ng-required", "!$last");
                }
            }

            FillInEnumDropList(doc, vEnumControl, fPropertyInfo.PropertyType);
            return vEnumControl;
        }  // CreateEnumDropList()


        /// <summary>
        /// FillInEnumDropList is a static method that takes a ASPxComboBox and fills in its
        /// items list with all values from the passed in Enum Type. The Text of each value
        /// will be from the Enum names; the values will be the integer value for the enum.
        /// This is the heart of CreateEnumDropList().
        /// </summary>
        private void FillInEnumDropList(HtmlDocument doc, HtmlNode pEnumControl, Type pEnumType)
        {
            Array vEnumValues = Enum.GetValues(pEnumType);  // .NET's tool for giving us all of an enum type's values

            HtmlNode option;
            for (int vI = 0; vI < vEnumValues.Length; vI++)
            {
                int vValue = Convert.ToInt32(vEnumValues.GetValue(vI));
                string vName = HtmlDocument.HtmlEncode(Enum.GetName(pEnumType, vValue));
                option = doc.CreateElement("option");
   
                option.InnerHtml = vName;
                if (vI == 0)
                {
                    // first option is for unknown, make it no selectable
                    option.SetAttributeValue("value", "");
                    option.SetAttributeValue("selected", "");
                }
                else
                {
                    option.SetAttributeValue("value", vValue.ToString());
                }
                pEnumControl.AppendChild(option);
            }  // for
        }  // FillInEnumDropList

    }  // class EnumProperty

    //----- CLASS CascadingListProperty ----------------------------------------------------------------------
    /// <remarks>
    /// CascadingListProperty is similar to EnumProperty except that it use client-side
    /// script to make a callback to server to update the values of another cascade combobox
    /// property when its selected value is changed
    /// </remarks>
    public class CascadingListProperty : BaseProperty
    {
        private HtmlNode fSelectControl;
        private IInstanceEditor _ownerControl;

        public CascadingListProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
            _ownerControl = pOwnerControl;

        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            string val = jProperty.Value[SELECTED_VALUE].ToString();

            try
            {
                Enum vEnum = GetEnumValue(val, fPropertyInfo.PropertyType);

                fPropertyInfo.SetValue(null, vEnum);

                // set the filter value if the property has a parent property defined, otherwise, the model will have empty options
                SetListFilterValue(vEnum.ToString());
            }
            catch (Exception)
            {
                // may be caused by legacy values
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            string val = element.Attributes[SELECTED_VALUE].ToString(); // TODO
            try
            {
                Enum vEnum = GetEnumValue(val, fPropertyInfo.PropertyType);

                fPropertyInfo.SetValue(null, vEnum);

                // set the filter value if the property has a parent property defined, otherwise, the model will have empty options
                SetListFilterValue(vEnum.ToString());
            }
            catch (Exception)
            {
                // may be caused by legacy values
            }
        }

        /// <summary>
        /// Convert the model value to ViewModel value
        /// </summary>
        /// <returns>A JObject with "selectedValue" and "options" properties, options are JArray object </returns>
        public override JToken GetPropertyViewModel()
        {
            JObject jPropertyValue = new JObject();
            object val = fPropertyInfo.GetValue();
            if (val != null && val is Enum)
            {
                Enum vEnum = (Enum)val;

                string selectedValue = Convert.ChangeType(vEnum, vEnum.GetTypeCode()).ToString();

                if (selectedValue == "0")
                    selectedValue = ""; // first option is unknown, change the value from "0" to ''

                jPropertyValue.Add(SELECTED_VALUE, selectedValue);

                // set the filter value if the property has a parent property defined, otherwise, the model will have empty options
                SetListFilterValue(vEnum.ToString());
            }
            else
            {
                jPropertyValue.Add(SELECTED_VALUE, "");
            }

            jPropertyValue.Add(OPTIONS, GetListOptions());

            return jPropertyValue;
        }

        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();

            // label element
            HtmlNode groupNode = this.fOwnerEditor.Document.CreateElement("div");
            container.AppendChild(groupNode);

            fSelectControl = CreateSelectControl(this.fOwnerEditor.Document, fPropertyInfo);
            groupNode.AppendChild(fSelectControl);

            if (!IsReadOnly)
            {
                this.CreateValidators(container);
            }

            return container;

        }  // CreatePropertyNode()

        /// <summary>
        /// CreateSelectControl creates a select html control with dynamic options. The options will be
        /// generated on the client side
        /// </summary>
        private HtmlNode CreateSelectControl(HtmlDocument doc, InstanceAttributePropertyDescriptor fPropertyInfo)
        {
            HtmlNode vSelectControl = doc.CreateElement("select");
            vSelectControl.SetAttributeValue("class", "form-control");
            vSelectControl.SetAttributeValue("name", PropertyControlName);
            vSelectControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");

            // set the angularjs  model binding
            if (fPropertyInfo.IsRequired)
            {
                if (!fOwnerEditor.IsInlineForm)
                {
                    vSelectControl.SetAttributeValue("required", "");
                }
                else
                {
                    vSelectControl.SetAttributeValue("ng-required", "!$last");
                }
            }

            if (IsReadOnly)
            {
                vSelectControl.SetAttributeValue("disabled", "disabled");
                vSelectControl.SetAttributeValue("style", "background-color:#f7f9f9;");
            }
            // set the angularjs  model binding
            vSelectControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + "." + SELECTED_VALUE);

            if (!string.IsNullOrEmpty(fPropertyInfo.CascadedPropertyNames))
            {
                // if the property has some cascading properties, it will call a service to reload the options
                // of the cascading properties when its value is changed
                StringBuilder javascriptFunc = new StringBuilder();
                javascriptFunc.Append("updateListOptions([");

                string[] cascadedAttributeNames = fPropertyInfo.CascadedPropertyNames.Split(';');

                int i = 0;
                foreach (string cascadedAttributeName in cascadedAttributeNames)
                {
                    if (!string.IsNullOrEmpty(cascadedAttributeName))
                    {
                        if (i > 0)
                        {
                            javascriptFunc.Append(",");
                        }

                        javascriptFunc.Append("'").Append(cascadedAttributeName).Append("'");

                        i++;
                    }
                }

                javascriptFunc.Append("], ").Append($"getSelectedText('{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}'))");

                AddResetScripts(cascadedAttributeNames, javascriptFunc);

                vSelectControl.SetAttributeValue("ng-change", javascriptFunc.ToString());
            }

            HtmlNode option = doc.CreateElement("option");
            option.SetAttributeValue("ng-repeat", "item in " + this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + "." + OPTIONS);
            option.SetAttributeValue("value", "{{item.value}}");
            option.InnerHtml = "{{item.name}}";
            
            vSelectControl.AppendChild(option);

            return vSelectControl;
        }  // CreateEnumDropList()

        private void AddResetScripts(string[] cascadedAttributeNames, StringBuilder javascriptFunc)
        {
            // when the selection changed, we want to reset the values of the all cascaded properties
            foreach (string cascadedAttributeName in cascadedAttributeNames)
            {
                if (!string.IsNullOrEmpty(cascadedAttributeName))
                {
                    javascriptFunc.Append("; ");

                    javascriptFunc.Append(this.fOwnerEditor.ViewModelPath).Append(cascadedAttributeName).Append(".selectedValue=''");

                    InstanceAttributePropertyDescriptor childPropertyInfo = (InstanceAttributePropertyDescriptor)_ownerControl.EditInstance.GetProperties(null)[cascadedAttributeName];

                    // add reset script if the attribute has cascaded attributes too
                    if (childPropertyInfo != null && !string.IsNullOrEmpty(childPropertyInfo.CascadedPropertyNames))
                    {
                        string[] nextLevelCascadedAttributeNames = childPropertyInfo.CascadedPropertyNames.Split(';');
                        AddResetScripts(nextLevelCascadedAttributeNames, javascriptFunc);
                    }
                }
            }
        }

        private  JArray GetListOptions()
        {
            JArray options = new JArray();

            Type enumType = fPropertyInfo.PropertyType;

            Array enumValues = Enum.GetValues(enumType);  // .NET's tool for giving us all of an enum type's values

            JObject option;
            for (int i = 0; i < enumValues.Length; i++)
            {
                int value = Convert.ToInt32(enumValues.GetValue(i));
                string name = Enum.GetName(enumType, value);

                option = new JObject();
                option.Add("value", value);
                option.Add("name", name);

                options.Add(option);
            }  // for

            // the actual property value may not be one of the enum values. This could happen
            // if the Enum Constraint has been modified or values of a List Constraint is changed,
            // since the data instance have been saved to the database. In this case, we have to
            // create a dropdown list item for the property value so that it can display properly
            /*
            object val = fPropertyInfo.GetValue();
            if (val != null && val is string)
            {
                string valStr = (string)val;
                HtmlNode option = this.fOwnerEditor.Document.CreateElement("option");
                option.SetAttributeValue("value", valStr);
                option.SetAttributeValue("selected", "true"); // make the value selected
                option.InnerHtml = HtmlDocument.HtmlEncode(valStr);
                fSelectControl.AppendChild(option);
            }
            */

            return options;
        }

        private void SetListFilterValue(string filterValue)
        {
            if (!string.IsNullOrEmpty(fPropertyInfo.CascadedPropertyNames))
            {
                string[] cascadedAttributeNames = fPropertyInfo.CascadedPropertyNames.Split(';');
                foreach (string cascadedAttributeName in cascadedAttributeNames)
                {
                    InstanceAttributePropertyDescriptor childPropertyInfo = (InstanceAttributePropertyDescriptor)_ownerControl.EditInstance.GetProperties(null)[cascadedAttributeName];

                    if (childPropertyInfo != null)
                    {
                        childPropertyInfo.SetListFilterValue(filterValue);
                    }
                }
            }
        } // SetListFilterValue

    }  // class CascadingListProperty

    //----- CLASS MaskedInputProperty ----------------------------------------------------------------------
    /// <remarks>
    /// MaskedInputProperty is similar to StringProperty except that it use a input mask
    /// to format the input text. If there is a datasource, provide a button to select a value from the specified data source
    /// </remarks>
    public class MaskedInputProperty : BaseProperty
    {
        private IInstanceEditor _ownerControl;

        public MaskedInputProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
            _ownerControl = pOwnerControl;
        }

        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();

            HtmlNode textNode = this.fOwnerEditor.Document.CreateTextNode("MaskedInputProperty");

            container.AppendChild(textNode);

            return container;
        }
    }  // class MaskedInputProperty


	//----- CLASS MultipleChoiceProperty ----------------------------------------------------------------------
	/// <remarks>
	/// MultipleChoiceProperty supports any enumerated type (based on the Enum class).
	/// It uses the power of the class System.Enum which defines all enumerated types.
	/// It has the capability of returning an array of Names and Values for any type.
	/// MultipleChoiceProperty builds a checkbox list of all names and values in the class supplied in fPropertyInfo.PropertyType.
	/// The checkbox list will contain Name and Values, both taken from the Enum.System class.
	/// </remarks>
	public class MultipleChoiceProperty : BaseProperty
	{
        public MultipleChoiceProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
		}

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            object[] vEnums = GetCheckboxValues(jProperty, fPropertyInfo.PropertyType);
            fPropertyInfo.SetValue(null, vEnums);
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            object[] vEnums = GetCheckboxValues(element, fPropertyInfo.PropertyType);
            fPropertyInfo.SetValue(null, vEnums);
        }

        /// <summary>
        /// Convert the model value to ViewModel value
        /// </summary>
        /// <returns></returns>
        public override JToken GetPropertyViewModel()
        {
            object[] vEnums = (object[])fPropertyInfo.GetValue();
            return GetCheckboxJSonValues(fPropertyInfo.PropertyType, vEnums);
        }

        public override HtmlNode CreatePropertyNode()
		{
            HtmlNode container = base.CreatePropertyNode();

            string[] enumNames = Enum.GetNames(fPropertyInfo.PropertyType);
            // the first enum is "None", do not show this option as a checkbox
            for (int i = 1; i < enumNames.Length; i++)
            {
                // appending a radio element for true
                HtmlNode child = this.fOwnerEditor.Document.CreateElement("label");
                child.SetAttributeValue("class", "checkbox-inline");
                container.AppendChild(child);

                // an input element
                HtmlNode input = this.fOwnerEditor.Document.CreateElement("input");
                input.SetAttributeValue("type", "checkbox");
                input.SetAttributeValue("name", PropertyControlName + i);
                input.SetAttributeValue("class", "checkbox style-0");
                // set the angularjs  model binding
                input.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + "[" + i + "]");

                if (fPropertyInfo.InvokeCallback)
                {
                    input.SetAttributeValue("ng-change", "reloadInstance('" + fPropertyInfo.Name + "')");
                }

                child.AppendChild(input);

                HtmlNode spanNode = this.fOwnerEditor.Document.CreateElement("span");
                spanNode.InnerHtml = HtmlDocument.HtmlEncode(enumNames[i]);
                child.AppendChild(spanNode);
            }

            this.CreateValidators(container);
            return container;
		}  // CreatePropertyNode()


        /// <summary>
        /// GetCheckboxValues is a static method to convert an array of Enum values to a JSON values
        /// </summary>
        private JToken GetCheckboxJSonValues(Type pEnumType, object[] pEnumValues)
        {
            JArray jArray = new JArray();

            string[] enumNames = Enum.GetNames(pEnumType);

            int i = 0;
            foreach (string enumName in enumNames)
            {
                object enumValue = Enum.Parse(pEnumType, enumName);
                bool found = false;
                for (int j = 0; j < pEnumValues.Length; j++)
                {
                    if (Enum.Equals(enumValue, pEnumValues[j]))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    jArray.Add(true);
                }
                else
                {
                    jArray.Add(false);
                }

                i++;
            }

            return jArray;
        }  // SetCheckboxValues()

        /// <summary>
        /// GetCheckboxValues is a method to retrieve an array of enum value from a JSON property's value.
        /// and convert them to Enum object array
        /// </summary>
        private object[] GetCheckboxValues(JProperty jProperty, Type pEnumType)
        {
            ArrayList checkedItems = new ArrayList();

            if (jProperty.Value != null &&
                jProperty.Value is JArray)
            {
                JArray jArray = (JArray)jProperty.Value;
                string[] enumNames = Enum.GetNames(pEnumType);

                int i = 0;
                foreach (var item in jArray.Children())
                {
                    if ((bool)item)
                    {
                        checkedItems.Add(enumNames[i]);
                    }
                    i++;
                }
            }

            object[] vEnums = new object[checkedItems.Count];
            for (int i = 0; i < vEnums.Length; i++)
            {
                vEnums[i] = Enum.Parse(pEnumType, (string)checkedItems[i]);
            }

            return vEnums;
        }  // GetCheckboxValues()

        /// <summary>
        /// GetCheckboxValues is a method to convert a XmlElement's value to Enum object array
        /// </summary>
        private object[] GetCheckboxValues(XmlElement element, Type pEnumType)
        {
            ArrayList checkedItems = new ArrayList();

            if (element != null && !string.IsNullOrEmpty(element.InnerText))
            {
                string[] values = element.InnerText.Split(';');
                string[] enumNames = Enum.GetNames(pEnumType);

                foreach (string val in values)
                {
                    bool found = false;
                    foreach (string enumName in enumNames)
                    {
                        if (val == enumName)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        checkedItems.Add(val);
                    }
                }
            }

            object[] vEnums = new object[checkedItems.Count];
            for (int i = 0; i < vEnums.Length; i++)
            {
                vEnums[i] = Enum.Parse(pEnumType, (string)checkedItems[i]);
            }

            return vEnums;
        }  // GetCheckboxValues()

    }  // class MultipleChoiceProperty

    //---- CLASS HistoryEditProperty ---------------------------------------------------------------------
    /// <remarks>
    /// HistoryEditProperty class is for HistoryEdit properties. It only allow to append text to the
    /// property's existing value with editor name and time of editing 
    /// </remarks>
    public class HistoryEditProperty : BaseProperty
    {
        private HtmlNode fUneditableTextControl = null;
        private HtmlNode fTextControl = null;

        public HistoryEditProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            // don't update when the field is empty if the original value was null.
            if (jProperty.Value["text"] != null && !string.IsNullOrEmpty(jProperty.Value["text"].ToString()))
            {
                string val = jProperty.Value["text"].ToString();

                fPropertyInfo.SetValue(null, val);
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <returns>Model value of the property</returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            // don't update when the field is empty if the original value was null.
            if (element != null && !string.IsNullOrEmpty(element.InnerText))
            {
                fPropertyInfo.SetValue(null, element.InnerText);
            }
        }

        /// <summary>
        /// Convert the model value to ViewModel value
        /// </summary>
        /// <returns></returns>
        public override JToken GetPropertyViewModel()
        {
            JObject jObject = new JObject();

            // most controls simply use the value.ToString() to get the data.
            // Override this method only when that is not the case.
            object vValue = fPropertyInfo.GetValue();
            if (vValue != null)
                jObject.Add("logs", vValue.ToString());
            else
                jObject.Add("logs", "");

            jObject.Add("text", "");

            return jObject;
        }

        // create a read-only textbox for viewing history and a textbox for appending text to the history
        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();

            fUneditableTextControl = this.fOwnerEditor.Document.CreateElement("textarea");
            fUneditableTextControl.SetAttributeValue("class", "form-control");
            fUneditableTextControl.SetAttributeValue("readonly", "");
            fUneditableTextControl.SetAttributeValue("style", "background-color:#f7f9f9;");
            fUneditableTextControl.SetAttributeValue("rows", fPropertyInfo.Rows.ToString());
            fUneditableTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + ".logs");
            container.AppendChild(fUneditableTextControl);

            if (!IsReadOnly)
            {

                fTextControl = this.fOwnerEditor.Document.CreateElement("textarea");
                fTextControl.SetAttributeValue("class", "form-control");
                fTextControl.SetAttributeValue("rows", "3");
                container.AppendChild(fTextControl);

                // set the angularjs  model binding
                fTextControl.SetAttributeValue("name", PropertyControlName);
                fTextControl.SetAttributeValue("placeholder", WebControlsResourceManager.GetString("PropertyEditor.InputComment"));
                fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
                // set the angularjs  model binding
                fTextControl.SetAttributeValue("ng-model", this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + ".text");
            }


            ToolTipToStatus(container);

            return container;
        }

    }  // class HistoryEditProperty

	//---- CLASS ArrayProperty ---------------------------------------------------------------------
	/// <remarks>
	/// ArrayProperty class supports properties of array type. It uses a DataGrid to display and
	/// edit array values.
	/// </remarks>
	public class ArrayProperty : BaseProperty
	{
        private List<PropertySetting> fSettings;
 
        public ArrayProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
		{
            if (!string.IsNullOrEmpty(pPropertyInfo.Settings))
            {
                fSettings = ConvertSettings(pPropertyInfo.Settings);
            }
            else
            {
                fSettings = null;
            }
		}

        /// <summary>
        /// Check if the jproperty contains non-empty value for the property
        /// </summary>
        /// <param name="jProperty"></param>
        /// <returns></returns>
        public override bool HasValue(JProperty jProperty)
        {
            if (jProperty.Value != null)
            {
                bool hasValue = false;

                JArray rows = jProperty.Value["rows"] as JArray; // The values of rows property is a JArray
                DataTable existingDataTable = ((ArrayDataTableView)fPropertyInfo.GetValue()).ArrayAttributeValue;

                for (int row = 0; row < rows.Count; row++)
                {
                    for (int col = 0; col < existingDataTable.Columns.Count; col++)
                    {
                        if (rows[row]["col" + col] != null &&
                            !string.IsNullOrEmpty(rows[row]["col" + col].ToString()))
                        {
                            hasValue = true;
                            break;
                        }
                    }

                    if (hasValue)
                    {
                        break;
                    }
                }

                return hasValue;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if an xml element contains non-empty value for the property
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool HasValue(XmlElement element)
        {
            if (element != null)
            {
                bool hasValue = false;

                XmlNodeList rows = element.ChildNodes; // The child elements are data of array rows
                DataTable existingDataTable = ((ArrayDataTableView)fPropertyInfo.GetValue()).ArrayAttributeValue;

                for (int row = 0; row < rows.Count; row++)
                {
                    XmlElement rowElement = rows[row] as XmlElement;
                    for (int col = 0; col < existingDataTable.Columns.Count; col++)
                    {
                        XmlElement colElement = rowElement[existingDataTable.Columns[col].ColumnName];
                        if (colElement != null &&
                            !string.IsNullOrEmpty(colElement.InnerText))
                        {
                            hasValue = true;
                            break;
                        }
                    }

                    if (hasValue)
                    {
                        break;
                    }
                }

                return hasValue;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            if (!IsReadOnly)
            {
                DataTable dataTable = GetDataTable(jProperty);

                ArrayDataTableView arrayDataTableView = (ArrayDataTableView)fPropertyInfo.GetValue();

                arrayDataTableView.ArrayAttributeValue = dataTable;
            }
        }

        /// <summary>
        /// Covert ViewModel value to Model value
        /// </summary>
        /// <param name="jProperty">ViewModel property</param>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (!IsReadOnly)
            {
                DataTable dataTable = GetDataTable(element);

                ArrayDataTableView arrayDataTableView = (ArrayDataTableView)fPropertyInfo.GetValue();

                arrayDataTableView.ArrayAttributeValue = dataTable;
            }
        }

        /// <summary>
        /// Get a DataTable from the ViewModel value
        /// </summary>
        /// <returns>A DataTable object</returns>
        private DataTable GetDataTable(JProperty jProperty)
        {
            DataTable dataTable = new DataTable(fPropertyInfo.Name);
            DataColumn dataColumn;
            DataRow dataRow;

            JArray rows = jProperty.Value["rows"] as JArray; // The values of rows property is a JArray
            DataTable existingDataTable = ((ArrayDataTableView)fPropertyInfo.GetValue()).ArrayAttributeValue;

            for (int col = 0; col < existingDataTable.Columns.Count; col++)
            {
                // Create new DataColumn for each of columns in the array.    
                dataColumn = new DataColumn();
                dataColumn.ColumnName = existingDataTable.Columns[col].ColumnName;
                // Add the Column to the DataColumnCollection.
                dataTable.Columns.Add(dataColumn);
            }

            bool hasValues;
            for (int row = 0; row < rows.Count; row++)
            {
                hasValues = false;

                dataRow = dataTable.NewRow();

                for (int col = 0; col < existingDataTable.Columns.Count; col++)
                {
                    if (rows[row]["col" + col] != null &&
                        !string.IsNullOrEmpty(rows[row]["col" + col].ToString()))
                    {
                        hasValues = true;
                        dataRow[col] = rows[row]["col" + col];
                    }
                }

                if (hasValues)
                {
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Get a DataTable from the XmlElememnt object
        /// </summary>
        /// <returns>A DataTable object</returns>
        private DataTable GetDataTable(XmlElement element)
        {
            DataTable dataTable = new DataTable(fPropertyInfo.Name);
            DataColumn dataColumn;
            DataRow dataRow;

            XmlNodeList rows = element.ChildNodes; // Child elements are array rows
            DataTable existingDataTable = ((ArrayDataTableView)fPropertyInfo.GetValue()).ArrayAttributeValue;

            for (int col = 0; col < existingDataTable.Columns.Count; col++)
            {
                // Create new DataColumn for each of columns in the array.    
                dataColumn = new DataColumn();
                dataColumn.ColumnName = existingDataTable.Columns[col].ColumnName;
                // Add the Column to the DataColumnCollection.
                dataTable.Columns.Add(dataColumn);
            }

            bool hasValues;
            XmlElement rowElement;
            XmlElement colElement;
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                hasValues = false;

                rowElement = rows[rowIndex] as XmlElement;

                dataRow = dataTable.NewRow();

                for (int col = 0; col < existingDataTable.Columns.Count; col++)
                {
                    colElement = rowElement[existingDataTable.Columns[col].ColumnName];
                    if (colElement != null && !string.IsNullOrEmpty(colElement.InnerText))
                    {
                        hasValues = true;
                        dataRow[col] = GetCellValue(colElement);
                    }
                }

                if (hasValues)
                {
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        private string GetCellValue(XmlElement element)
        {
            if (element.ChildNodes == null || element.ChildNodes.Count == 0)
                return element.InnerText;
            else
            {
                string val = "";
                foreach (XmlNode child in element.ChildNodes)
                {
                    if (!string.IsNullOrEmpty(val))
                        val += ",";

                    val += child.InnerText;
                }

                return val;
            }
        }

        /// <summary>
        /// Convert the model value to ViewModel value
        /// </summary>
        /// <returns></returns>
        public override JToken GetPropertyViewModel()
        {
            DataTable dataTable = ((ArrayDataTableView)fPropertyInfo.GetValue()).ArrayAttributeValue;
            JObject arrayObj = new JObject();
            JArray rows = new JArray();

            arrayObj.Add("rows", rows);

            JObject rowObj;
            string val;

            if (dataTable.Rows.Count > 0)
            {
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    rowObj = new JObject();
                    rows.Add(rowObj);

                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        if (!dataTable.Rows[row].IsNull(col) &&
                            dataTable.Rows[row][col].ToString().Length > 0)
                        {
                            val = dataTable.Rows[row][col].ToString();
                        }
                        else
                        {
                            val = "";
                        }
                        rowObj.Add("col" + col, val); // name of a cell is "col0", "col1", etc.
                    }
                }
            }

            // append an empty row at end
            rowObj = new JObject();
            rows.Add(rowObj);

            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                rowObj.Add("col" + col, ""); // name of a cell is "col0", "col1", etc.
            }

            return arrayObj;
        }


        /// Create a table of text boxes for the array data	
		public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();

            HtmlNode divNode = this.fOwnerEditor.Document.CreateElement("div");
            divNode.SetAttributeValue("class", "table-responsive");

            container.AppendChild(divNode);

            HtmlNode tableNode = this.fOwnerEditor.Document.CreateElement("table");
            tableNode.SetAttributeValue("class", "table table-striped");
            divNode.AppendChild(tableNode);

            // table header
            HtmlNode theadNode = this.fOwnerEditor.Document.CreateElement("thead");
            tableNode.AppendChild(theadNode);

            HtmlNode trNode = this.fOwnerEditor.Document.CreateElement("tr");
            theadNode.AppendChild(trNode);

            DataTable dataTable = ((ArrayDataTableView)fPropertyInfo.GetValue()).ArrayAttributeValue;
            // create a table of text boxes for each value in the datatable
            HtmlNode thNode;
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                thNode = this.fOwnerEditor.Document.CreateElement("td");
                if (!string.IsNullOrEmpty(dataTable.Columns[col].Caption))
                {
                    thNode.InnerHtml = HtmlDocument.HtmlEncode(dataTable.Columns[col].Caption);
                }
                else
                {
                    thNode.InnerHtml = "Column_" + col;
                }
                trNode.AppendChild(thNode);
            }

            // add and delete row command
            if (!IsReadOnly)
            {
                thNode = this.InstanceEditor.Document.CreateElement("td");
                thNode.InnerHtml = "#";
                trNode.AppendChild(thNode);

                thNode = this.InstanceEditor.Document.CreateElement("td");
                thNode.InnerHtml = "#";
                trNode.AppendChild(thNode);
            }

            HtmlNode tbodyNode = this.fOwnerEditor.Document.CreateElement("tbody");
            tbodyNode.SetAttributeValue("ng-repeat", "row in " + this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + ".rows" + " track by $index"); // create tr with imput elements for array
            if (IsReadOnly)
                tbodyNode.SetAttributeValue("ng-if", "!$last");
            tableNode.AppendChild(tbodyNode);

            HtmlNode tdNode;
            trNode = this.fOwnerEditor.Document.CreateElement("tr");
            tbodyNode.AppendChild(trNode);
            HtmlNode inputNode;
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                tdNode = this.fOwnerEditor.Document.CreateElement("td");
                trNode.AppendChild(tdNode);

                inputNode = CreateInputElement(col);

                tdNode.AppendChild(inputNode);
            }

            //string arrayName = this.fOwnerEditor.ViewModelPath + fPropertyInfo.Name + ".rows";
            string arrayPath = fPropertyInfo.Name + ".rows";

            // delete row command
            if (!IsReadOnly)
            {
                // Add btn
                tdNode = this.InstanceEditor.Document.CreateElement("td");
                tdNode.SetAttributeValue("class", "text-align-center");
                trNode.AppendChild(tdNode);
                HtmlNode addNode = CreateAddBtnElement(arrayPath);
                tdNode.AppendChild(addNode);

                tdNode = this.fOwnerEditor.Document.CreateElement("td");
                tdNode.SetAttributeValue("class", "text-align-center");
                trNode.AppendChild(tdNode);
                HtmlNode deleteNode = CreateDeleteBtnElement(arrayPath);
                deleteNode.SetAttributeValue("ng-hide", "$last");
                tdNode.AppendChild(deleteNode);
            }

            return container;
        }  // CreatePropertyNode()

        private HtmlNode CreateInputElement(int colIndex)
        {
            HtmlNode container = this.fOwnerEditor.Document.CreateElement("label");
            container.SetAttributeValue("class", "input");

            HtmlNode inputNode = this.fOwnerEditor.Document.CreateElement("input");
            inputNode.SetAttributeValue("class", "form-control");
            inputNode.SetAttributeValue("type", "text");

            if (IsReadOnly)
            {
                inputNode.SetAttributeValue("readonly", "readonly");
                inputNode.SetAttributeValue("style", "background-color:#f7f9f9;");
            }

            // set the angularjs  model binding
            inputNode.SetAttributeValue("ng-model", "row.col" + colIndex);

            container.AppendChild(inputNode);

            return container;
        }

        private HtmlNode CreateAddBtnElement(string arrayPath)
        {
            HtmlNode btn = this.InstanceEditor.Document.CreateElement("span");
            btn.SetAttributeValue("class", "btn btn-sm btn-success");
            btn.SetAttributeValue("ng-click", "copyArrayRow('" + arrayPath + "', $index)");

            HtmlNode iconNode = this.InstanceEditor.Document.CreateElement("i");
            iconNode.SetAttributeValue("class", "glyphicon glyphicon-plus");
            btn.AppendChild(iconNode);

            return btn;
        }

        private HtmlNode CreateDeleteBtnElement(string path)
        {
            HtmlNode container = this.fOwnerEditor.Document.CreateElement("span");
            container.SetAttributeValue("class", "btn btn-sm btn-danger");
            container.SetAttributeValue("ng-click", "removeArrayRow('" + path + "', $index)");

            HtmlNode iconNode = this.fOwnerEditor.Document.CreateElement("i");
            iconNode.SetAttributeValue("class", "glyphicon glyphicon-remove");

            container.AppendChild(iconNode);

            return container;
        }

        private List<PropertySetting> ConvertSettings(string settings)
        {
            List<PropertySetting> settingCollection = new List<PropertySetting>();
            PropertySetting propertySetting;

            string[] nameValues = settings.Split(','); // name value paires are separated by comma
            foreach (string nameValue in nameValues)
            {
                propertySetting = new PropertySetting();

                // nameValue pair can be name=value or name only
                int pos = nameValue.IndexOf('=');
                if (pos > 0)
                {
                    // name=value
                    propertySetting.ParameterName = nameValue.Substring(0, pos).Trim();
                    propertySetting.ParameterValue = nameValue.Substring(pos + 1).Trim();
                }
                else
                {
                    // name only
                    propertySetting.ParameterName = nameValue.Trim();
                    propertySetting.ParameterValue = null;
                }

                settingCollection.Add(propertySetting);
            }

            return settingCollection;
        }

    }  // class ArrayProperty

    ///---- CLASS ManyToManyRelationshipProperty ---------------------------------------------------------------------
    /// <remarks>
    /// ManyToManyRelationshipProperty class is for many-to-many relationship properties. 
    /// It shows the number of the related instances and allow to add relationships to the other
    /// side of many-to-many relationships
    /// </remarks>
    public class ManyToManyRelationshipProperty : BaseProperty
    {
        private IInstanceEditor _ownerControl;

        public override JToken GetPropertyViewModel()
        {
            return null;
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
        
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {

        }

        public ManyToManyRelationshipProperty(IInstanceEditor pOwnerControl, InstanceView instanceView,
            InstanceAttributePropertyDescriptor pPropertyInfo, bool isReadOnly)
            : base(pOwnerControl, instanceView, pPropertyInfo, isReadOnly)
        {
            _ownerControl = (IInstanceEditor)pOwnerControl;
        }

        // create a text box and an image link to launch an instance picker
        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container = base.CreatePropertyNode();
  
            // label element
            HtmlNode groupNode = this.fOwnerEditor.Document.CreateElement("div");
            groupNode.SetAttributeValue("class", "input-group");
            container.AppendChild(groupNode);

            string editInstanceId = this._ownerControl.EditInstance.InstanceData.ObjId;

            HtmlNode fTextControl = this.fOwnerEditor.Document.CreateElement("input");
            fTextControl.SetAttributeValue("class", "form-control");
            fTextControl.SetAttributeValue("type", "text");
            fTextControl.SetAttributeValue("readonly", "");
            fTextControl.SetAttributeValue("style", "background-color:#f7f9f9;");
            fTextControl.SetAttributeValue("name", PropertyControlName);
            fTextControl.SetAttributeValue("id", $"{this.fOwnerEditor.EditorId}-{this.fOwnerEditor.EditInstance.DataView.BaseClass.ClassName}-{PropertyControlName}");
            groupNode.AppendChild(fTextControl);

            HtmlNode addonNode = this.fOwnerEditor.Document.CreateElement("span");
            addonNode.SetAttributeValue("class", "input-group-addon");
            groupNode.AppendChild(addonNode);

            HtmlNode linkNode = this.fOwnerEditor.Document.CreateElement("a");
            linkNode.SetAttributeValue("data-ui-sref", GetManyToManyModelURL(editInstanceId));
            //linkNode.SetAttributeValue("href-void", "");
            addonNode.AppendChild(linkNode);

            HtmlNode iNode = this.fOwnerEditor.Document.CreateElement("i");
            iNode.SetAttributeValue("class", "icon-append fa fa-share-alt");

            linkNode.AppendChild(iNode);

            ToolTipToStatus(container);
            return container;

        }  // CreatePropertyNode


        private string GetManyToManyModelURL(string instanceId)
        {
            string url = ".viewmanytomany";

            // Find out the class related by the relationship attribute
            SchemaModel schemaModel = this._ownerControl.EditInstance.DataView.SchemaModel;
            string ownerClassName = this._ownerControl.EditInstance.DataView.BaseClass.ClassName;
            ClassElement classElement = this._ownerControl.EditInstance.DataView.SchemaModel.FindClass(ownerClassName);
            RelationshipAttributeElement relationshipElement = classElement.FindInheritedRelationshipAttribute(fPropertyInfo.Name);
            // find the many-to-one relationship attribute in the junction class that connects to
            // the referenced class on the otherside of relationship
            RelationshipAttributeElement toReferencedClsRelationshipAttribute = relationshipElement.LinkedClass.FindPairedRelationshipAttribute(relationshipElement);
            string linkedClassName = toReferencedClsRelationshipAttribute.LinkedClassName;

            url += "({masterclass: '" + ownerClassName + "', relatedclass:'" + linkedClassName + "', masterid: '" + instanceId + "'})";

            return url;
        }
    }  // class ManyToManyRelationshipProperty

    internal class PropertySetting
    {
        public string ParameterName = null;
        public string ParameterValue = null;
    }

	//-------------------------------------------------------------------------------------------

    public interface IInstanceEditor
    {
        /// <summary>
        /// An unique id for this editor instance
        /// </summary>
        string EditorId { get; }

        /// <summary>
        /// Get the html document for the editor
        /// </summary>
        HtmlDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the JObject instance submitted by the client
        /// </summary>
        dynamic Instance { get; set; }

        /// <summary>
        /// Gets or sets a html form name of the editor
        /// </summary>
        string FormName { get; set; }

        /// <summary>
        /// Gets or sets information indicating whether the properties are displayed in a inline form
        /// </summary>
        bool IsInlineForm { get; set; }

        /// <summary>
        /// Gets or sets the information indicating whether the editor is view only
        /// </summary>
        bool IsViewOnly {get; set;}

        /// <summary>
        /// The main instance being edited
        /// </summary>
        InstanceView EditInstance { get; set;}

        /// <summary>
        /// The id if the instance being edited
        /// </summary>
        string InstanceId { get; set;}

        /// <summary>
        /// Gets class name of the edit instance
        /// </summary>
        string InstanceClassName { get; }

        /// <summary>
        /// Gets or set the id of a form, This property is used to identify a client when running BeforeInsert or BefoerUpdate scripts
        /// </summary>
        string FormId { get; set; }

        /// <summary>
        /// Gets or set the id of a workflow task, This property is used to identify a wf task when running custom action script
        /// </summary>
        string TaskId { get; set; }

        /// <summary>
        /// Gets or set the id of a workflow action, This property is used to identify a wf action when running custom action script
        /// </summary>
        string ActionId { get; set; }

        /// <summary>
        /// Gets a root path of the view model for the editor
        /// </summary>
        string ViewModelPath { get; }

        /// <summary>
        /// Gets a root path of accessing an array in the view model
        /// </summary>
        string ArrayBasePath {get;}

        /// <summary>
        /// Gets the information indicating whether th editor is for a related class to the master class
        /// </summary>
        bool IsRelatedClass { get; }

        /// <summary>
        /// Gets or sets the information indicating whether a instance represented by the editor has been removed from UI
        /// </summary>
        bool IsRemoved { get; set; }

        /// <summary>
        /// Gets or sets a connection string to the current database
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// The editor that is a container of this editor
        /// </summary>
        IInstanceEditor ParentEditor { get; set; }


        /// <summary>
        /// A coolection of property holders representing properties appeared in this editor
        /// </summary>
        List<PropertyPlaceHolder> PropertyPlaceHolders {get;}

        /// <summary>
        /// Gets the information indicating whether the instance has values
        /// </summary>
        bool HasValues(JObject instance);

        /// <summary>
        /// Gets the information indicating whether the instance has values
        /// </summary>
        bool HasValues(XmlElement instance);

        /// <summary>
        /// Validate the model represented by the InstanceView instance against the schema.
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>Return if the instance model is valid. It will throw an exception if a validating error occures </returns>
        void Validate();

        /// <summary>
        /// ConvertToViewModel converts values from the InstanceView instance (Model) to a JSON instance (ViewModel)
        /// the EditInstance
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>The JSON instance</returns>
        JObject ConvertToViewModel(bool convert);

        /// <summary>
        /// ConvertToIndexingDocument converts values from the InstanceView instance (Model) to a JSON instance as a document
        /// for full-text search indexing
        /// </summary>
        /// <returns>The JSON instance</returns>
        JObject ConvertToIndexingDocument();

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        List<IInstanceEditor> ConvertToModel(JObject instance);

        /// <summary>
        /// Translate an XmlElement object to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="element">An XmlElement object</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        List<IInstanceEditor> ConvertToModel(XmlElement element);

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <param name="isFormFormat">true if the json values are in form format where enum values are internal values</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        List<IInstanceEditor> ConvertToModel(JObject instance, bool isFormFormat);

        /// <summary>
        /// Save the the master instance and related instnaces to the database 
        /// </summary>
        /// <param name="">A list of related instances</param>
        /// <returns>The obj_id of the saved instance</returns>
        string SaveToDatabase(List<IInstanceEditor> relatedEditInstances);

        /// <summary>
        /// Create a IPropertyControl object for the given property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>IPropertyControl object, can be null</returns>
        IPropertyControl CreatePropertyControl(string propertyName);

        /// <summary>
        /// Create a IPropertyControl object for the given property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>IPropertyControl object, can be null</returns>
        IPropertyControl CreatePropertyControl(string propertyName, bool isReadOnly, string defaultValue);

        /// <summary>
        /// Run the action code of an action
        /// </summary>
        /// <param name="connection">db connection</param>
        /// <param name="schemaName">The schema name</param>
        /// <param name="taskId"> the id of a workflow task sent by the client</param>
        /// <param name="actionId">The id of workflow action sent by a client</param>
        /// <param name="instanceView">The instance to run the code.</param>
        void RunCustomAction(CMConnection connection, string schemaName, string taskId, string actionId, InstanceView instanceView, dynamic instance);
    }

    public interface IPropertyControl
    {
        /// <summary>
        /// Gets or set the Instance Editor on which the property control lives
        /// </summary>
        IInstanceEditor InstanceEditor { get; set;}

        /// <summary>
        /// Gets or set the instance view on which the PropertyInfo object belongs
        /// </summary>
        InstanceView BaseInstanceView { get; set; }

        /// <summary>
        /// Gets or set the object that contains information about the property
        /// </summary>
        InstanceAttributePropertyDescriptor PropertyInfo { get; set;}

        /// <summary>
        /// Gets an unique name for the property control in a form
        /// </summary>
        string PropertyControlName { get;}

        /// <summary>
        /// Gets or set the readonly status
        /// </summary>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Check if the jproperty contains non-empty value for the property
        /// </summary>
        /// <param name="jProperty"></param>
        /// <returns></returns>
        bool HasValue(JProperty jProperty);

        /// <summary>
        /// Check if an xml element contains non-empty value for the property
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool HasValue(XmlElement element);

        /// <summary>
        /// Validate the property value and throw an exception if there is an error
        /// </summary>
        void Validate();

        /// <summary>
        /// Convert the model value to ViewModel value
        /// </summary>
        /// <returns></returns>
        JToken GetPropertyViewModel();

        /// <summary>
        /// Update the instance with the value in a JProperty object
        /// </summary>
        /// <param name="jProperty">the JProperty object</param>
        /// <param name="isFormFormat">whether the value is in the form format</param>
        void UpdateValueInternal(JProperty jProperty, bool isFormFormat);

        /// <summary>
        /// Update the instance with the value in a XmlElement object
        /// </summary>
        /// <param name="jProperty">the JProperty object</param>
        /// <param name="isFormFormat">whether the value is in the form format</param>
        void UpdateValueInternal(XmlElement element, bool isFormFormat);

        /// <summary>
        /// Gets model instances from an array of ViewModel instances
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        List<IInstanceEditor> GetInstanceEditors(JProperty jProperty);

        /// <summary>
        /// Gets model instances from the child elements of an xml element representing a wrapper of related instances.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        List<IInstanceEditor> GetInstanceEditors(XmlElement element);

        /// <summary>
        /// Create the customized UI
        /// </summary>
        /// <returns>Root control of the customized UI</returns>
        HtmlNode CreatePropertyNode();
    }

    //----- ENUM ApplyInstanceType -----------------------------------------------------------
    /// <remarks>
    /// ApplyInstanceType assists the ApplyInstance method as it updates the values assigned to controls.
    /// Each row in the table may be visible or invisible. When visible, post back code assigns
    /// values. When invisible, it does not.
    /// This determines which rows get updated based on visibility.
    /// </remarks>
    public enum ApplyInstanceType
    {
        Visible,
        Invisible,  // used on post back
        All   // used on databind
    }  // ApplyInstanceType

    /// <summary>
    /// A base class for customized property UI
    /// </summary>
    public abstract class PropertyControlBase : IPropertyControl
    {
        private InstanceView _baseInstanceView;
        private InstanceAttributePropertyDescriptor _propertyInfo;
        private IInstanceEditor _ownerEditor;
        private bool _isReadOnly;
        private string _controlName;

        public PropertyControlBase()
        {
            _ownerEditor = null;
            _propertyInfo = null;
            _baseInstanceView = null;
            _isReadOnly = false;
            _controlName = null;
        }


        /// <summary>
        /// Gets or set the Instance Editor on which the property control resides
        /// </summary>
        public IInstanceEditor InstanceEditor
        {
            get
            {
                return _ownerEditor;
            }
            set
            {
                _ownerEditor = value;
            }
        }

        /// <summary>
        /// Gets or set the instance view which the base instance of an editor form
        /// </summary>
        public InstanceView BaseInstanceView
        {
            get
            {
                return _baseInstanceView;
            }
            set
            {
                _baseInstanceView = value;
            }
        }

        /// <summary>
        /// Gets or set the object that contains information about the property
        /// </summary>
        public InstanceAttributePropertyDescriptor PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
            set
            {
                _propertyInfo = value;
            }
        }

        /// <summary>
        /// Gets an unique name for the property control in a form
        /// </summary>
        public string PropertyControlName
        {
            get
            {
                if (string.IsNullOrEmpty(_controlName))
                {
                    if (this._ownerEditor.IsRelatedClass)
                    {
                        _controlName = this._ownerEditor.InstanceClassName + "-" + this.PropertyInfo.Name;
                    }
                    else
                    {
                        _controlName = this.PropertyInfo.Name;
                    }
                }

                return _controlName;
            }
        }

        /// <summary>
        /// Gets or set the readonly status
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly || !this.PropertyInfo.AllowManualUpdate;
            }
            set
            {
                _isReadOnly = value;
            }
        }

        /// <summary>
        /// Check if the jproperty contains non-empty value for the property
        /// </summary>
        /// <param name="jProperty"></param>
        /// <returns></returns>
        public virtual bool HasValue(JProperty jProperty)
        {
            if (jProperty.Value != null &&
                !string.IsNullOrEmpty(jProperty.Value.ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if an xml element contains non-empty value for the property
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public virtual bool HasValue(XmlElement element)
        {
            if (element != null &&
                !string.IsNullOrEmpty(element.InnerText))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validate the property value and throw an exception if there is an error
        /// </summary>
        public virtual void Validate()
        {
            // do nothing
        }

        /// <summary>
        /// create html nodes for the property
        /// </summary>
        public virtual HtmlNode CreatePropertyNode()
        {
            // create a property container for all types of properties
            HtmlNode container = this.InstanceEditor.Document.CreateElement("div");
            container.SetAttributeValue("ng-class", "getCssClasses(" + this.InstanceEditor.FormName + "." + this.PropertyControlName + ")");
            return container;
        }


        /// <summary>
        /// Gets the value in the json property, can be overrided by subclass to convert JProperty value to string
        /// </summary>
        /// <returns></returns>
        public virtual void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
            if (jProperty.Value != null)
            {
                _propertyInfo.SetStringValue(jProperty.Value.ToString());
            }
        }

        /// <summary>
        /// Update the instance with the value in a XmlElement object
        /// </summary>
        /// <param name="jProperty">the JProperty object</param>
        /// <param name="isFormFormat">whether the value is in the form format</param>
        public virtual void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {
            if (element != null)
            {
                _propertyInfo.SetStringValue(element.InnerText);
            }
        }

        /// <summary>
        /// Gets model instances from an array of ViewModel instances when jProperty's value is an JArray.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public virtual List<IInstanceEditor> GetInstanceEditors(JProperty jProperty)
        {
            return null;
        }

        /// <summary>
        /// Gets model instances from the child elements of an xml element representing a wrapper of related instances.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public virtual List<IInstanceEditor> GetInstanceEditors(XmlElement element)
        {
            return null;
        }

        public virtual JToken GetPropertyViewModel()
        {
            return _propertyInfo.GetStringValue();
        }
    }

}  // namespace PropEdControls


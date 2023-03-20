using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Threading;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

using HtmlAgilityPack;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Common.Wrapper;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.Engine.Workflow;

namespace Newtera.WebForm
{
    /// <remarks>
    /// CustomFormEditor is an editor whose layout is based on a html template.
    /// The CustomFormEditor is specialized to shows properties of an InstanceView, defined
    /// in Newtera.Common.MetaDataModel.DataView package, object representing
    /// a master data instance.
    /// 
    /// The CustomFormEditor is a composite editor which may include a set of InstanceEditor objects
    /// representing data instances that are related to the master instance.
    ///
    /// It builds a free formed layout with a html template with property placeholder embedded.
    ///
    /// This control uses Angularjs DataBinding to map the master InstanceView object into its properties and
    /// related InstanceView objects into its InstanceEditor objects.
    /// It builds a view model for Angularjs Client Side DataBinding
    /// 
    /// </remarks>
    public class CustomFormEditor : BaseInstanceEditor
	{
		private List<PropertyPlaceHolder> _propertyPlaceHolders = new List<PropertyPlaceHolder>();  // will hold references to any PropertyPlaceHolder associated with the top level properties

        private string _templatePath = null; // template file

        private List<IInstanceEditor> _instanceEditors = new List<IInstanceEditor>();

		//------ PROPERTIES ---------------------------------------------------------------------

        /// <summary>
        /// The owner of this editor which is usually a CustomFormEditor
        /// </summary>
        public CustomFormEditor EditorOwner
        {
            get
            {
                return this;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets the information indicating whether th editor is for a related class to the master class
        /// </summary>
        public override bool IsRelatedClass
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets name of a html template file
        /// </summary>
        public string TemplatePath
        {
            get
            {
                return _templatePath;
            }
            set
            {
                _templatePath = value;
            }
        }

        /// <summary>
        /// Create a form template in html
        /// </summary>
        /// <param name="taskId">A workflow task id to which the form is created</param>
        public override string CreateFormTemplate(string taskId)
        {
            if (EditInstance != null) // post back. Create controls. If not, DataBind does the work.
            {
                // get property read only status defined for a workflow task
                string schemaName = EditInstance.DataView.SchemaInfo.Name;
                _propertyReadOnlyStatusTable = GetPropertyReadOnlyStatusTable(schemaName, taskId);

                HtmlDocument doc = CreateControlHierarchy();

                return doc.DocumentNode.OuterHtml;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public override List<IInstanceEditor> ConvertToModel(JObject instance)
        {
            return ConvertToModel(instance, true);
        }

        /// <summary>
        /// Translate an XmlElement object to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="element">An XmlElement object</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public override List<IInstanceEditor> ConvertToModel(XmlElement element)
        {
            List<IInstanceEditor> instanceEditors = GetInstanceEditors();
            List<IInstanceEditor> relatedEditInstances = new List<IInstanceEditor>();

            // first conver the xml element representing the master instance to model
            foreach (IInstanceEditor instanceEditor in instanceEditors)
            {
                if (!instanceEditor.IsRelatedClass)
                {
                    instanceEditor.FormId = this.FormId;
                    instanceEditor.TaskId = this.TaskId;
                    instanceEditor.ActionId = this.ActionId;
                    instanceEditor.Instance = element;
                    instanceEditor.ConvertToModel(element);
                    break;
                }
            }

            // then convert xml elements representing the related instances to model instances
            XmlElement relatedInstanceWrapper;
            foreach (IInstanceEditor instanceEditor in instanceEditors)
            {
                if (instanceEditor.IsRelatedClass)
                {
                    relatedInstanceWrapper = null;

                    // find the name of relationship from the master to the related class, and use it to locate
                    // the child element of the master element
                    string relationshipName = FindRelationshipName(instanceEditor.InstanceClassName);

                    if (element[instanceEditor.InstanceClassName] != null)
                    {
                        // this is an element representing a class that is related to the master class through a many-to-one relationship
                        relatedInstanceWrapper = element[instanceEditor.InstanceClassName];
                    }
                    else if (!string.IsNullOrEmpty(relationshipName))
                    {
                        relatedInstanceWrapper = element[relationshipName];
                    }

                    if (relatedInstanceWrapper != null)
                    {
                        List<IInstanceEditor> convertedInstances = instanceEditor.ConvertToModel(relatedInstanceWrapper);
                        if (convertedInstances != null)
                        {
                            instanceEditor.Instance = relatedInstanceWrapper;

                            foreach (IInstanceEditor convertedInstance in convertedInstances)
                            {
                                relatedEditInstances.Add(convertedInstance); // collect it so that we will process the list with all instances later
                            }
                        }
                    }
                }
            }

            return relatedEditInstances;
        }

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <param name="isFormFormat">true if the json values are in form format where enum values are internal values</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public override List<IInstanceEditor> ConvertToModel(JObject instance, bool isFormFormat)
        {
            List<IInstanceEditor> instanceEditors = GetInstanceEditors();
            List<IInstanceEditor> relatedEditInstances = new List<IInstanceEditor>();

            // first conver view data in the master instance to model
            foreach (IInstanceEditor instanceEditor in instanceEditors)
            {
                if (!instanceEditor.IsRelatedClass)
                {
                    instanceEditor.FormId = this.FormId;
                    instanceEditor.TaskId = this.TaskId;
                    instanceEditor.ActionId = this.ActionId;
                    instanceEditor.Instance = instance;
                    instanceEditor.ConvertToModel(instance);
                    break;
                }
            }

            // then convert view model of related instances to model instances
            JObject relatedInstance;
            foreach (IInstanceEditor instanceEditor in instanceEditors)
            {
                if (instanceEditor.IsRelatedClass)
                {
                    relatedInstance = instance.GetValue(instanceEditor.InstanceClassName) as JObject;
                    if (relatedInstance != null)
                    {
                        List<IInstanceEditor> convertedInstances = instanceEditor.ConvertToModel(relatedInstance);
                        instanceEditor.Instance = relatedInstance;

                        foreach (IInstanceEditor convertedInstance in convertedInstances)
                        {
                            relatedEditInstances.Add(convertedInstance); // collect it so that we will process the list with all instances later
                        }
                    }
                }
            }

            return relatedEditInstances;
        }

        /// <summary>
        /// Validate the model represented by the InstanceView instance against the schema.
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>Return if the instance model is valid. It will throw an exception if a validating error occures </returns>
        public override void Validate()
        {

        }

        /// <summary>
        /// Save the the master instance and related instances to the database 
        /// </summary>
        /// <param name="">A list of related instances</param>
        /// <returns>The obj_id of the saved instance</returns>
        public override string SaveToDatabase(List<IInstanceEditor> relatedEditInstances)
        {
            List<EditInstanceDef> editInstanceDefs = BuildChainedEditInstances(relatedEditInstances);
            foreach (EditInstanceDef editInstanceDef in editInstanceDefs)
            {
                if (!PersistChangesToDataBase(editInstanceDef))
                {
                    // there is an error, stop the saving rest of the instance
                    break;
                }
            }

            return this.EditInstance.InstanceData.ObjId;
        }

        /// <summary>
        /// ConvertToViewModel converts values from the InstanceView instance (Model) to a JSON instance (ViewModel)
        /// the EditInstance
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>The JSON instance</returns>
        public override JObject ConvertToViewModel(bool convert)
        {
            return ConvertToViewModel(convert, false);
        }

        /// <summary>
        /// ConvertToIndexingDocument converts values from the InstanceView instance (Model) to a JSON instance as a document
        /// for full-text search indexing
        /// </summary>
        /// <returns>The JSON instance</returns>
        public override JObject ConvertToIndexingDocument()
        {
            return ConvertToViewModel(true, true);
        }

        private JObject ConvertToViewModel(bool convert, bool forFullTextSearch)
        {
            JObject instance = new JObject();

            List<IInstanceEditor> instanceEditors = GetInstanceEditors();

            // first conver data in the master instance to view model
            foreach (IInstanceEditor instanceEditor in instanceEditors)
            {
                if (!instanceEditor.IsRelatedClass)
                {
                    if (!forFullTextSearch)
                        instance = instanceEditor.ConvertToViewModel(convert);
                    else
                        instance = instanceEditor.ConvertToIndexingDocument();
                    break;
                }
            }

            // then convert related instances to view model and add them to the view model of the master instance
            JObject relatedInstance;
            foreach (IInstanceEditor instanceEditor in instanceEditors)
            {
                if (instanceEditor.IsRelatedClass)
                {
                    if (!forFullTextSearch)
                        relatedInstance = instanceEditor.ConvertToViewModel(convert);
                    else
                        relatedInstance = instanceEditor.ConvertToIndexingDocument();

                    if (relatedInstance != null)
                    {
                        // use instance class name as an property name to make related instance as part of master instance view model
                        // The related instance is in form of { property1: value1, property2: value2, ...}
                        instance.Add(instanceEditor.InstanceClassName, relatedInstance);
                    }
                }
            }

            return instance;
        }

		//---- METHODS ---------------------------------------------------------------
		
		/// <summary>
		/// CreateControlHierarchy creates all controls using a html template, when encoutering
        /// a property place holder, create a control for the property by calling CreatePropertyControl
		/// with InstanceView's properties.
		/// </summary>
		private HtmlDocument CreateControlHierarchy()
		{
            HtmlDocument doc = null;
            string templateFileName = TemplatePath;

            // read html template
            if (File.Exists(templateFileName))
            {
                doc = new HtmlDocument();
                doc.Load(templateFileName);
                string html = doc.DocumentNode.OuterHtml;
                html = html.Replace("\n", " ").Replace("\r", " ").Replace("\t", " "); // get rid of carriage returns and tab
                doc = new HtmlDocument();
                doc.LoadHtml(html);

                // create placeholder controls from html
                _propertyPlaceHolders = CreatePlaceHolders(doc);

                // Create an InstanceEditor for all properties of the master class and
                // an InstanceEditor for all properties in each of the related class
                List<IInstanceEditor> instanceEditors = CreateInstanceEditors(_propertyPlaceHolders, doc);

                // Replace the input nodes in the doc with the node generated by its corresponsing control
                HtmlNode propertyRootNode;
                IPropertyControl propertyControl;
                bool isReadOnly;
                foreach (IInstanceEditor instanceEditor in instanceEditors)
                {
                    foreach (PropertyPlaceHolder placeHolder in instanceEditor.PropertyPlaceHolders)
                    {
                        if (placeHolder.IsReadOnly ||
                            IsPropertyReadOnly(placeHolder.PropertyName))
                        {
                            isReadOnly = true;
                        }
                        else
                        {
                            isReadOnly = false;
                        }

                        propertyControl = instanceEditor.CreatePropertyControl(placeHolder.PropertyName, isReadOnly, placeHolder.DefaultValue);

                        if (propertyControl != null)
                        {
                            propertyRootNode = propertyControl.CreatePropertyNode();

                            if (propertyRootNode != null)
                            {
                                // replace the input node 
                                placeHolder.ParentNode.ReplaceChild(propertyRootNode, placeHolder.InputNode);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Unable to find a file at " + templateFileName);
            }

            return doc;
		}  // CreateControlHierarchy()


        private InstanceView GetRelatedInstanceView(string relatedClassName)
        {
            InstanceView instanceView = null;

            using (CMConnection connection = new CMConnection(this.ConnectionString))
            {
                connection.Open();

                DataViewModel dataView = connection.MetaDataModel.GetRelatedDetailedDataView(this.EditInstance, relatedClassName);

                if (dataView != null)
                {
                    dataView.PageIndex = 0;
                    dataView.PageSize = 1000; // assuming there is no more 1000 related instances in the nested form

                    string query = dataView.SearchQueryWithPKValues;

                    CMCommand cmd = connection.CreateCommand();
                    cmd.CommandText = query;

                    XmlReader reader = cmd.ExecuteXMLReader();
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                    {
                        // Create an instance view with instance data
                        instanceView = new InstanceView(dataView, ds);
                    }
                    else
                    {
                        instanceView = new InstanceView(dataView);
                    }
                }
                else
                {
                    throw new Exception("Unable to find a dataview for class " + relatedClassName);
                }
            }

            return instanceView;
        }

        /// <summary>
        /// GetInstanceEditors parse the input fields in a html template and create an IInstanceEditor for each
        /// of the instance class appeared in the custom form
        /// </summary>
        private List<IInstanceEditor> GetInstanceEditors()
        {
            List<IInstanceEditor> instanceEditors = null;
            HtmlDocument doc = null;
            string templateFileName = TemplatePath;

            // read html template
            if (File.Exists(templateFileName))
            {
                doc = new HtmlDocument();
                doc.Load(templateFileName);

                // create placeholder controls from html
                _propertyPlaceHolders = CreatePlaceHolders(doc);

                // Create an InstanceEditor for all properties of the master class and
                // an InstanceEditor for all properties in each of the related class
                instanceEditors = CreateInstanceEditors(_propertyPlaceHolders, doc);
            }
            else
            {
                throw new Exception("Unable to find a file at " + templateFileName);
            }

            return instanceEditors;
        }  // CreateControlHierarchy()

        /// <summary>
        /// Parse the html template and create placeholder controls for each input tag
        /// appeared in the html document.
        /// </summary>
        /// <returns>A list of property placeholder controls.</returns>
        private List<PropertyPlaceHolder> GetPropertyPlaceHolders()
        {
            List<PropertyPlaceHolder> propertyPlaceHolders;
            HtmlDocument doc;

            // read html template
            if (File.Exists(TemplatePath))
            {
                doc = new HtmlDocument();
                doc.Load(TemplatePath);


                // create placeholder controls from html
                propertyPlaceHolders = CreatePlaceHolders(doc);
            }
            else
            {
                throw new Exception("Unable to find the file " + TemplatePath);
            }

            return propertyPlaceHolders;
        }


        private List<IInstanceEditor> CreateInstanceEditors(List<PropertyPlaceHolder> propertyPlaceHolders, HtmlDocument doc)
        {
            List<IInstanceEditor> instanceEditors = new List<IInstanceEditor>();
            IInstanceEditor editor;

            // Create a InstanceEditor object for the master class and each related class appeared in the form
            foreach (PropertyPlaceHolder propertyPlaceHolder in propertyPlaceHolders)
            {
                editor = null;

                // find the editor if exists
                foreach (InstanceEditor instanceEditor in instanceEditors)
                {
                    if (propertyPlaceHolder.ClassName == instanceEditor.EditInstance.DataView.BaseClass.ClassName ||
                        IsParentOf(propertyPlaceHolder.ClassName, instanceEditor.EditInstance.DataView.BaseClass.ClassName))
                    {
                        editor = instanceEditor;
                        break;
                    }
                }

                if (editor == null)
                {
                    if (this.EditInstance.DataView.BaseClass.ClassName == propertyPlaceHolder.ClassName ||
                        IsParentOf(propertyPlaceHolder.ClassName, this.EditInstance.DataView.BaseClass.ClassName))
                    {
                        // it is a property of the master instance for the form, create an InstanceEditor object
                        editor = new InstanceEditor();
                        editor.EditInstance = this.EditInstance;
                        editor.IsViewOnly = this.IsViewOnly;
                        editor.ConnectionString = this.ConnectionString;
                        editor.Document = doc;
                        editor.ParentEditor = this;
                    }
                    else
                    {
                        // It is a property of a related instance(s) to the master instance, create a RelatedInstanceEditor object
                        editor = new RelatedInstanceEditor();
                        editor.EditInstance = GetRelatedInstanceView(propertyPlaceHolder.ClassName);
                        editor.ConnectionString = this.ConnectionString;
                        editor.Document = doc;
                        editor.ParentEditor = this;
                        editor.IsViewOnly = this.IsViewOnly;
                        ((RelatedInstanceEditor)editor).IsForeignKeyRequired = GetForeignKeyRequiredStatus(editor.EditInstance, propertyPlaceHolder.PropertyName);
                    }

                    // add the place holder to the editor
                    editor.PropertyPlaceHolders.Add(propertyPlaceHolder);

                    instanceEditors.Add(editor);
                }
                else
                {
                    // add the place holder to the editor
                    editor.PropertyPlaceHolders.Add(propertyPlaceHolder);
                }
            }
    
            return instanceEditors;
        }


        private bool GetForeignKeyRequiredStatus(InstanceView instanceView, string propertyName)
        {
            bool status = false;

            InstanceAttributePropertyDescriptor pd = instanceView.GetProperties(null)[propertyName] as InstanceAttributePropertyDescriptor;

            if (pd != null)
            {
                status = pd.IsForeignKeyRequired;
            }

            return status;
        }

        /// <summary>
        /// Create place holder controls by parsing the html of a template
        /// </summary>
        /// <param name="dco">Html document</param>
        /// <returns>A list of place holder controls.</returns>
        protected List<PropertyPlaceHolder> CreatePlaceHolders(HtmlDocument doc)
        {
            List < PropertyPlaceHolder > propertyPlaceHolders = new List<PropertyPlaceHolder>();
            Hashtable table = new Hashtable();

            PropertyPlaceHolder placeHolder;
            bool isReadOnly;
            int pos;
            string inputId, sizeStr, inputSettings, defaultValueStr;
            HtmlNodeCollection inputElements = doc.DocumentNode.SelectNodes("//input");
            if (inputElements != null && inputElements.Count > 0)
            {
                foreach (HtmlNode input in doc.DocumentNode.SelectNodes("//input"))
                {
                    inputId = null;
                    sizeStr = null;
                    inputSettings = null;
                    defaultValueStr = null;
                    isReadOnly = false;
                    if (input.Attributes["name"] != null)
                    {
                        inputId = input.Attributes["name"].Value;
                    }

                    if (input.Attributes["size"] != null)
                    {
                        sizeStr = input.Attributes["size"].Value;
                    }

                    if (input.Attributes["settings"] != null)
                    {
                        inputSettings = input.Attributes["settings"].Value;
                    }

                    if (input.Attributes["value"] != null)
                    {
                        defaultValueStr = input.Attributes["value"].Value;
                    }

                    if (input.Attributes["read"] != null)
                    {
                        try
                        {
                            isReadOnly = bool.Parse(input.Attributes["read"].Value);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    if (!string.IsNullOrEmpty(inputId))
                    {
                        pos = inputId.IndexOf('_'); // format of input id is of ClassName_PropertyName
                        if (pos > 0)
                        {
                            placeHolder = new PropertyPlaceHolder();

                            placeHolder.ClassName = inputId.Substring(0, pos);
                            placeHolder.PropertyName = inputId.Substring(pos + 1);
                            placeHolder.Size = sizeStr;
                            placeHolder.IsReadOnly = isReadOnly;
                            placeHolder.DefaultValue = defaultValueStr;
                            placeHolder.Settings = inputSettings;
                            placeHolder.InputNode = input;
                            placeHolder.ParentNode = input.ParentNode;

                            propertyPlaceHolders.Add(placeHolder);
                        }
                    }
                }
            }

            return propertyPlaceHolders;
        }

        /// <summary>
        /// Perform the action of saving the instance to database, either insert or update the
        /// instance.
        /// </summary>
        /// <param name="editInstanceDef">The defintion of the instance to be saved</param>
        /// <returns>true if the action succeeded, false otherwise</returns>
        private bool PersistChangesToDataBase(EditInstanceDef editInstanceDef)
        {
            bool status = true;

            if (editInstanceDef.Action == EditAction.Insert && !editInstanceDef.IsReadOnly)
            {
                status = AddInstance(editInstanceDef);
            }
            else if (editInstanceDef.Action == EditAction.Update && !editInstanceDef.IsReadOnly)
            {
                status = UpdateInstance(editInstanceDef);
            }
            else if (editInstanceDef.Action == EditAction.Delete && !editInstanceDef.IsReadOnly)
            {
                status = DeleteInstance(editInstanceDef);
            }

            return status;
        }

        /// <summary>
        /// Add an instance to database.
        /// </summary>
        /// <param name="editInstanceDef">The defintion of the instance and its context required to perform add action.</param>
        /// <returns>return status of adding, true if add succeeded, false otherwise</returns>
        private bool AddInstance(EditInstanceDef editInstanceDef)
        {
            InstanceView instanceView = editInstanceDef.EditInstance;
            string connectionString = this.ConnectionString;

            using (CMConnection connection = new CMConnection(connectionString))
            {
                connection.Open();

                ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.Name);

                // set the relationship to the referenced instances if any
                if (editInstanceDef.InstanceRelationships.Count > 0)
                {
                    foreach (InstanceRelationship instanceRelationship in editInstanceDef.InstanceRelationships)
                    {
                        string referencedInstanceId = instanceRelationship.ReferencedInstanceDef.EditInstance.InstanceData.ObjId;
                        instanceView.InstanceData.SetAttributeValue(instanceRelationship.ReferencedRelationshipName, referencedInstanceId);
                    }
                }

                // Execute the before insert code
                string beforeInsertCode = GetClassCustomCode(classElement, ClassElement.CLASS_BEFORE_INSERT_CODE);
                if (!string.IsNullOrEmpty(beforeInsertCode))
                {
                    // Execute the before updare code
                    IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView, connectionString, editInstanceDef.Instance); ;

                    if (!string.IsNullOrEmpty(FormId))
                    {
                        // script may use FormId to run different part of code
                        ActionCodeRunner.Instance.ExecuteActionCode(FormId, "ClassInsert" + classElement.ID, beforeInsertCode, instanceWrapper);
                    }
                    else
                    {
                        ActionCodeRunner.Instance.ExecuteActionCode("ClassInsert" + classElement.ID, beforeInsertCode, instanceWrapper);
                    }
                }

                string query = instanceView.DataView.InsertQuery;

                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = query;

                try
                {
                    XmlDocument doc = cmd.ExecuteXMLDoc();
                    instanceView.InstanceData.ObjId = doc.DocumentElement.InnerText;
                }
                catch (Exception ex)
                {
                    //ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    //return false;
                    throw new Exception("Failed to insert an instance in class " + classElement.Caption + " for the reason " + ex.Message);
                }
            }

            return true; // succeeded
        }

        // return status of updating, true if update succeeded, false otherwise
        private bool UpdateInstance(EditInstanceDef editInstanceDef)
        {
            string connectionString = this.ConnectionString;

            InstanceView instanceView = editInstanceDef.EditInstance;

            // set the relationship to the referenced instances if any
            if (editInstanceDef.InstanceRelationships.Count > 0)
            {
                foreach (InstanceRelationship instanceRelationship in editInstanceDef.InstanceRelationships)
                {
                    if (instanceRelationship.ReferencedInstanceDef.Action == EditAction.Insert)
                    {
                        // The referenced instance is newly inserted, we need to retrieve its primary key value(s)
                        // and establish the relationship
                        if (!string.IsNullOrEmpty(instanceRelationship.ReferencedInstanceDef.EditInstance.InstanceData.ObjId))
                        {
                            string pkValues = RetriveInstancePKValues(instanceRelationship.ReferencedInstanceDef.EditInstance);
                            instanceView.InstanceData.SetAttributeStringValue(instanceRelationship.ReferencedRelationshipName, pkValues);
                        }
                    }
                }
            }

            // run the workflow custom action code or BeforeUpdateCode on the instance if any
            using (CMConnection connection = new CMConnection(connectionString))
            {
                connection.Open();

                if (!string.IsNullOrEmpty(this.TaskId) && !string.IsNullOrEmpty(this.ActionId))
                {
                    // execute the script specified in a workflow custom action
                    RunCustomAction(connection, instanceView.DataView.SchemaInfo.Name, this.TaskId, this.ActionId, instanceView, editInstanceDef.Instance);

                    // Raise Close Task Event to the HanldeGroupTaskActivity
                    RaiseCloseTaskEvent(connection, instanceView.DataView.SchemaInfo.Name, this.TaskId, this.ActionId, instanceView);
                }
                else
                {
                    ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.Name);

                    // Execute the before update code
                    string beforeUpdateCode = GetClassCustomCode(classElement, ClassElement.CLASS_BEFORE_UPDATE_CODE);
                    if (!string.IsNullOrEmpty(beforeUpdateCode))
                    {
                        // Execute the before updare code
                        IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView, connectionString, editInstanceDef.Instance); ;

                        if (!string.IsNullOrEmpty(FormId))
                        {
                            // Script may use formid to run different part of code
                            ActionCodeRunner.Instance.ExecuteActionCode(FormId, "ClassUpdate" + classElement.ID, beforeUpdateCode, instanceWrapper);
                        }
                        else
                        {
                            ActionCodeRunner.Instance.ExecuteActionCode("ClassUpdate" + classElement.ID, beforeUpdateCode, instanceWrapper);
                        }
                    }
                }

                bool status = instanceView.IsDataChanged; // run IsDataChanged method so that the values set by RunBeforeUpdate and BeforeUpdate handler will be included in the UpdateQuery

                if (status)
                {
                    string query = instanceView.DataView.UpdateQuery;

                    CMCommand cmd = connection.CreateCommand();
                    cmd.CommandText = query;

                    try
                    {
                        XmlDocument doc = cmd.ExecuteXMLDoc();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                        throw new Exception("Update " + editInstanceDef.EditInstance.DataView.BaseClass.Caption + " got error due to " + ex.Message);
                    }
                }
            }

            return true; // succeeded
        }

        // return status of deleting, true if deletion succeeded, false otherwise
        private bool DeleteInstance(EditInstanceDef editInstanceDef)
        {
            string connectionString = this.ConnectionString;

            InstanceView instanceView = editInstanceDef.EditInstance;

            if (instanceView != null)
            {
                using (CMConnection connection = new CMConnection(connectionString))
                {
                    connection.Open();

                    string query = instanceView.DataView.DeleteQuery;

                    CMCommand cmd = connection.CreateCommand();
                    cmd.CommandText = query;

                    try
                    {
                        XmlDocument doc = cmd.ExecuteXMLDoc();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                        return false;
                    }
                }
            }

            return true; // succeeded
        }

        private string RetriveInstancePKValues(InstanceView instanceView)
        {
            string pkValues = null;

            using (CMConnection connection = new CMConnection(this.ConnectionString))
            {
                connection.Open();

                DataViewModel dataView = instanceView.DataView;

                string query = dataView.GetInstanceQuery(instanceView.InstanceData.ObjId);

                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                // Create an instance view
                InstanceView theInstanceView = new InstanceView(dataView, ds);

                pkValues = theInstanceView.InstanceData.PrimaryKeyValues;

            }

            return pkValues;
        }

        private List<IInstanceEditor> GetEditInstances()
        {
            List<IInstanceEditor> editInstances = new List<IInstanceEditor>();

            return editInstances;
        }

        /// <summary>
        /// Build a chain of actions to perform to the database according to the edit types and relationships
        /// among the data instances to be edited
        /// </summary>
        /// <returns>A colection of EditInstanceDef objects</returns>
        private List<EditInstanceDef> BuildChainedEditInstances(List<IInstanceEditor> relatedEditInstances)
        {
            List<EditInstanceDef> editInstanceDefs = new List<EditInstanceDef>();

            using (CMConnection connection = new CMConnection(this.ConnectionString))
            {
                connection.Open();

                // create an EditInstanceDef object for the base instance
                EditInstanceDef baseEditInstanceDef = new EditInstanceDef();
                baseEditInstanceDef.EditInstance = this.EditInstance;
                baseEditInstanceDef.Instance = this.Instance;
                if (string.IsNullOrEmpty(this.InstanceId))
                {
                    baseEditInstanceDef.Action = EditAction.Insert;
                }
                else
                {
                    baseEditInstanceDef.Action = EditAction.Update;
                }

                // find the instances that the base instance is referenced to
                InstanceView relatedInstanceView;
                string relatedClassName;
                EditInstanceDef editInstanceDef;
                foreach (IInstanceEditor relatedInstanceEditor in relatedEditInstances)
                {
                    if (!relatedInstanceEditor.IsRemoved &&
                        relatedInstanceEditor.PropertyPlaceHolders.Count > 0)
                    {
                        relatedInstanceView = relatedInstanceEditor.EditInstance;
                        relatedClassName = relatedInstanceView.DataView.BaseClass.ClassName;

                        foreach (DataClass relatedClass in this.EditInstance.DataView.BaseClass.RelatedClasses)
                        {
                            if (relatedClass.ClassName == relatedClassName || IsParentOf(connection.MetaDataModel, relatedClass.ClassName, relatedClassName))
                            {
                                if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
                                {
                                    // it is a many-to-one relationship between the base class and related class,
                                    // therefore, we have to make the base instance depend on the referenced instance by
                                    // putting the referenced instance in front of the base instance in the chain
                                    editInstanceDef = new EditInstanceDef();
                                    editInstanceDef.EditInstance = relatedInstanceEditor.EditInstance;
                                    editInstanceDef.Instance = relatedInstanceEditor.Instance;
                                    if (string.IsNullOrEmpty(relatedInstanceEditor.InstanceId))
                                    {
                                        // the instance hasn't been created yet
                                        editInstanceDef.Action = EditAction.Insert;
                                    }
                                    else
                                    {
                                        // the instance has been created
                                        editInstanceDef.Action = EditAction.Update;
                                    }

                                    // If Ownership of the relationship in the base class isn't Owned, the  the related instance
                                    // is referenced by the base instance only, therefore, we are not supposed to add or update the referenced
                                    // instance
                                    if (relatedClass.ReferringRelationship.BackwardRelationship.Ownership != RelationshipOwnership.Owned)
                                    {
                                        editInstanceDef.IsReadOnly = true;
                                    }

                                    editInstanceDefs.Add(editInstanceDef);

                                    // add instance relationship to the base instance
                                    InstanceRelationship instanceRelationship = new InstanceRelationship();
                                    instanceRelationship.ReferencedInstanceDef = editInstanceDef;
                                    instanceRelationship.ReferencedRelationshipName = relatedClass.ReferringRelationship.Name;
                                    baseEditInstanceDef.InstanceRelationships.Add(instanceRelationship);
                                }

                                break;
                            }
                        }
                    }
                }

                // add the base instance to the chain
                editInstanceDefs.Add(baseEditInstanceDef);

                // add the instances that reference the base instance to the chain
                foreach (IInstanceEditor relatedInstanceEditor in relatedEditInstances)
                {
                    if (!relatedInstanceEditor.IsRemoved)
                    {
                        relatedInstanceView = relatedInstanceEditor.EditInstance;
                        relatedClassName = relatedInstanceView.DataView.BaseClass.ClassName;
                        foreach (DataClass relatedClass in this.EditInstance.DataView.BaseClass.RelatedClasses)
                        {
                            if (relatedClass.ClassName == relatedClassName || IsParentOf(connection.MetaDataModel, relatedClass.ClassName, relatedClassName))
                            {
                                if (!relatedClass.ReferringRelationship.IsForeignKeyRequired)
                                {
                                    // it is a one-to-many relationship between the base class and related class,
                                    // therefore, we have to make the referenced instance depend on the base instance by
                                    // putting the referenced instance after of the base instance in the chain
                                    editInstanceDef = new EditInstanceDef();
                                    editInstanceDef.EditInstance = relatedInstanceEditor.EditInstance;
                                    editInstanceDef.Instance = relatedInstanceEditor.Instance;
                                    if (string.IsNullOrEmpty(relatedInstanceEditor.InstanceId))
                                    {
                                        // the instance hasn't been created yet
                                        editInstanceDef.Action = EditAction.Insert;
                                    }
                                    else
                                    {
                                        // the instance has been created
                                        editInstanceDef.Action = EditAction.Update;
                                    }

                                    editInstanceDefs.Add(editInstanceDef);

                                    // add instance relationship to the referenced instance
                                    InstanceRelationship instanceRelationship = new InstanceRelationship();
                                    instanceRelationship.ReferencedInstanceDef = baseEditInstanceDef;
                                    instanceRelationship.ReferencedRelationshipName = relatedClass.ReferringRelationship.BackwardRelationship.Name;
                                    editInstanceDef.InstanceRelationships.Add(instanceRelationship);
                                }

                                break;
                            }
                        }
                    }
                }

                // add the instances that to be deleted to the chain
                foreach (IInstanceEditor relatedInstanceEditor in relatedEditInstances)
                {
                    if (relatedInstanceEditor.IsRemoved &&
                        !string.IsNullOrEmpty(relatedInstanceEditor.InstanceId))
                    {
                        editInstanceDef = new EditInstanceDef();
                        editInstanceDef.EditInstance = relatedInstanceEditor.EditInstance;
                        editInstanceDef.Instance = relatedInstanceEditor.Instance;

                        // Delete the instance
                        editInstanceDef.Action = EditAction.Delete;

                        editInstanceDefs.Add(editInstanceDef);
                    }
                }
            }

            return editInstanceDefs;
        }

        private string GetClassCustomCode(ClassElement classElement, string codeBlock)
        {
            string code = null;

            code = GetCustomCode(classElement, codeBlock);

            if (string.IsNullOrEmpty(code) &&
                classElement.ParentClass != null)
            {
                // use the code defined in the parent class
                code = GetClassCustomCode(classElement.ParentClass, codeBlock);
            }

            return code;
        }

        private string GetCustomCode(ClassElement classElement, string codeBlock)
        {
            string code = null;

            switch (codeBlock)
            {
                case ClassElement.CLASS_INITIALIZATION_CODE:
                    code = classElement.InitializationCode;
                    break;

                case ClassElement.CLASS_BEFORE_INSERT_CODE:

                    code = classElement.BeforeInsertCode;
                    break;

                case ClassElement.CLASS_BEFORE_UPDATE_CODE:

                    code = classElement.BeforeUpdateCode;
                    break;

                case ClassElement.CLASS_CALLBACK_CODE:

                    code = classElement.CallbackFunctionCode;

                    break;
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = code.Trim();
            }

            return code;
        }

        private string FindRelationshipName(string relatedClassName)
        {
            DataClass relatedDataClass = null;
            foreach (DataClass relatedClass in this.EditInstance.DataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == relatedClassName ||
                    IsParentOf(relatedClass.ClassName, relatedClassName))
                {
                    relatedDataClass = relatedClass;
                    break;
                }
            }

            if (relatedDataClass == null)
            {
                // try to get from referenced Classes
                foreach (DataClass relatedClass in this.EditInstance.DataView.ReferencedClasses)
                {
                    if (relatedClass.ClassName == relatedClassName ||
                        IsParentOf(relatedClass.ClassName, relatedClassName))
                    {
                        relatedDataClass = relatedClass;
                        break;
                    }
                }

                if (relatedDataClass == null)
                {
                    throw new Exception("Unable to find a related data class " + relatedClassName + " for " + this.EditInstance.DataView.BaseClass.Name);
                }
            }

            return relatedDataClass.ReferringRelationship.Name;
        }
    }

	//----CLASS PropertyPlaceHolder -----------------------------------------------------------
	/// <remarks>
	/// PropertyPlaceHolder includes references to key controls in the place holder
	/// </remarks>
	public class PropertyPlaceHolder
	{
        public int RowIndex = 0; // An index of row for the instance
        public string InstanceId = null; // instance's id
        public string ClassName = null; // property's class name
        public string PropertyName = null; // property name
		public BaseProperty fPropertyControl = null;  // field associated with a BaseProperty subclass
        public string Size = null; // property UI control size
        public bool IsReadOnly = false; // whether the property is readonly
        public string Settings = null; // extra property settings
        public string DefaultValue = null; // default value
        public HtmlNode InputNode = null; // the input node to be replaced by the node generated by the property control
        public HtmlNode ParentNode = null; // the HTML node which is the parent node of an input node
 
	}  // class PropertyPlaceHolder

    /// <summary>
    /// Definition of the instance to be saved to the database
    /// </summary>
    public class EditInstanceDef
    {
        public InstanceView EditInstance;
        public dynamic Instance;
        public EditAction Action;
        public bool IsReadOnly = false;
        public List<InstanceRelationship> InstanceRelationships = new List<InstanceRelationship>();
    }

    public class InstanceRelationship
    {
        public EditInstanceDef ReferencedInstanceDef;
        public string ReferencedRelationshipName;
    }

    /// <summary>
    /// Definition of the database action
    /// </summary>
    public enum EditAction
    {
        Update,
        Insert,
        Delete
    }

    public class RowActionInfo
    {
        public int RowIndex = 0;
        public int CopyRowIndex = 0;
        public string TableId;
        public RowActionType Action = RowActionType.AddRow;
    }

    public enum RowActionType
    {
        AddRow,
        DelRow
    }
}  // namespace FreeFormEditorControls


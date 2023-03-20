/*
* @(#) RelationshipViewOnlyGridViewControl.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
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

namespace Newtera.WebForm
{
    /// <summary>
    /// An IPropertyControl control that supports viewing and editing related instances in a gridview for a one-to-many relationship
    /// </summary>
    public class RelationshipViewOnlyGridViewControl : PropertyControlBase
    {
        public const string ROW_ACTION_INFOS = "TableRowActionInfo";
        private int _defaultRows = 1;
        private InstanceView _relatedInstanceView = null;
        private bool _isReadOnly = true;

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(JProperty jProperty, bool isFormFormat)
        {
 
        }

        /// <summary>
        /// Update model value with the ViewModel value
        /// </summary>
        /// <returns></returns>
        public override void UpdateValueInternal(XmlElement element, bool isFormFormat)
        {

        }

        /// <summary>
        /// Gets model instances from an array of ViewModel instances when jProperty;s value is an JArray.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public override List<IInstanceEditor> GetInstanceEditors(JProperty jProperty)
        {
            List<IInstanceEditor> instanceEditors = new List<IInstanceEditor>();

            if (_relatedInstanceView == null)
            {
                _relatedInstanceView = GetRelatedInstanceView(this.BaseInstanceView.DataView.BaseClass.ClassName); // BaseInstanceView owns the relationship attribute
            }

            int count = 0;

            if (!DataSetHelper.IsEmptyDataSet(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName))
            {
                count = DataSetHelper.GetRowCount(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName);
            }

            // build the instances for edit, start with updates, inserts, and deletes
            IInstanceEditor editor;
            JObject relatedInstance;

            for (int row = 0; row < count; row++)
            {
                _relatedInstanceView.SelectedIndex = row;

                relatedInstance = FindViewModelInstance(jProperty, _relatedInstanceView.InstanceData.ObjId);
                if (relatedInstance != null)
                {
                    // found instances to update
                    editor = new InstanceEditor();
                    // Get an individual instance view with all property values loaded. 
                    // Because it will run BeforeInsert or BeforeUpdate script on the instance view which requires
                    // all property values are present in the instance view. 
                    editor.EditInstance = GetDetailedInstanceView(_relatedInstanceView.InstanceData.ObjId);
                    editor.IsRemoved = false;
                    editor.ConnectionString = this.InstanceEditor.ConnectionString;
                    editor.Document = this.InstanceEditor.Document;
                    editor.ParentEditor = this.InstanceEditor.ParentEditor;

                    // convert view model data to model
                    editor.ConvertToModel(relatedInstance);

                    instanceEditors.Add(editor);
                }
                else
                {
                    // the instance has been removed on view, create an InstanceEditor to remember it
                    editor = new InstanceEditor();
                    editor.EditInstance = GetDetailedInstanceView(_relatedInstanceView.InstanceData.ObjId);
                    editor.IsRemoved = true;
                    editor.ConnectionString = this.InstanceEditor.ConnectionString;
                    editor.Document = this.InstanceEditor.Document;
                    editor.ParentEditor = this.InstanceEditor.ParentEditor;

                    instanceEditors.Add(editor);
                }
            }

            // found new instances to be inserted
            foreach (JObject instance in jProperty.Value)
            {
                if (instance.GetValue("obj_id") == null ||
                    string.IsNullOrEmpty(instance.GetValue("obj_id").ToString()))
                {
                    // found instances to insert
                    editor = new InstanceEditor();
                    editor.EditInstance = GetDetailedInstanceView(null);
                    editor.IsRemoved = false;
                    editor.ConnectionString = this.InstanceEditor.ConnectionString;
                    editor.Document = this.InstanceEditor.Document;
                    editor.ParentEditor = this.InstanceEditor.ParentEditor;

                    if (editor.HasValues(instance))
                    {
                        // convert view model data to model
                        editor.ConvertToModel(instance);

                        instanceEditors.Add(editor);
                    }
                }
            }

            return instanceEditors;
        }

        /// <summary>
        /// Gets model instances from the child elements of an xml element representing a wrapper of related instances.
        /// </summary>
        /// <returns>A collection of IInstanceEditor objects</returns>
        public override List<IInstanceEditor> GetInstanceEditors(XmlElement element)
        {
            List<IInstanceEditor> instanceEditors = new List<IInstanceEditor>();

            if (_relatedInstanceView == null)
            {
                _relatedInstanceView = GetRelatedInstanceView(this.BaseInstanceView.DataView.BaseClass.ClassName); // BaseInstanceView owns the relationship attribute
            }

            int count = 0;

            if (!DataSetHelper.IsEmptyDataSet(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName))
            {
                count = DataSetHelper.GetRowCount(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName);
            }

            // build the instances for edit, start with updates, inserts, and deletes
            IInstanceEditor editor;
            XmlElement relatedInstance;

            for (int row = 0; row < count; row++)
            {
                _relatedInstanceView.SelectedIndex = row;

                relatedInstance = FindViewModelInstance(element, _relatedInstanceView.InstanceData.ObjId);
                if (relatedInstance != null)
                {
                    // found instances to update
                    editor = new InstanceEditor();
                    // Get an individual instance view with all property values loaded. 
                    // Because it will run BeforeInsert or BeforeUpdate script on the instance view which requires
                    // all property values are present in the instance view. 
                    editor.EditInstance = GetDetailedInstanceView(_relatedInstanceView.InstanceData.ObjId);
                    editor.IsRemoved = false;
                    editor.ConnectionString = this.InstanceEditor.ConnectionString;
                    editor.Document = this.InstanceEditor.Document;
                    editor.ParentEditor = this.InstanceEditor.ParentEditor;

                    // convert view model data to model
                    editor.ConvertToModel(relatedInstance);

                    instanceEditors.Add(editor);
                }
                else
                {
                    // the instance has been removed on view, create an InstanceEditor to remember it
                    editor = new InstanceEditor();
                    editor.EditInstance = GetDetailedInstanceView(_relatedInstanceView.InstanceData.ObjId);
                    editor.IsRemoved = true;
                    editor.ConnectionString = this.InstanceEditor.ConnectionString;
                    editor.Document = this.InstanceEditor.Document;
                    editor.ParentEditor = this.InstanceEditor.ParentEditor;

                    instanceEditors.Add(editor);
                }
            }

            // found new instances to be inserted
            foreach (XmlElement instance in element.ChildNodes)
            {
                if (instance["obj_id"] == null ||
                    string.IsNullOrEmpty(instance["obj_id"].InnerText))
                {
                    // found instances to insert
                    editor = new InstanceEditor();
                    editor.EditInstance = GetDetailedInstanceView(null);
                    editor.IsRemoved = false;
                    editor.ConnectionString = this.InstanceEditor.ConnectionString;
                    editor.Document = this.InstanceEditor.Document;
                    editor.ParentEditor = this.InstanceEditor.ParentEditor;

                    if (editor.HasValues(instance))
                    {
                        // convert view model data to model
                        editor.ConvertToModel(instance);

                        instanceEditors.Add(editor);
                    }
                }
            }

            return instanceEditors;
        }

        public override JToken GetPropertyViewModel()
        {
            JArray rows = new JArray();

            if (_relatedInstanceView == null)
            {
                _relatedInstanceView = GetRelatedInstanceView(this.BaseInstanceView.DataView.BaseClass.ClassName); // BaseInstanceView owns the relationship attribute
            }

            int count = 0;

            if (!DataSetHelper.IsEmptyDataSet(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName))
            {
                count = DataSetHelper.GetRowCount(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName);
            }

            // convert data of each related instance to a view model instance
            JObject rowObj = null;
            IInstanceEditor editor;
            if (count > 0)
            {
                for (int row = 0; row < count; row++)
                {
                    _relatedInstanceView.SelectedIndex = row;

                    editor = new InstanceEditor();
                    editor.EditInstance = _relatedInstanceView;
                    editor.ConnectionString = this.InstanceEditor.ConnectionString;
                    editor.Document = this.InstanceEditor.Document;
                    editor.ParentEditor = this.InstanceEditor.ParentEditor;

                    rowObj = editor.ConvertToViewModel(true);

                    //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(rowObj, Newtonsoft.Json.Formatting.Indented);
                    //ErrorLog.Instance.WriteLine("related obj=" + jsonString);

                    rows.Add(rowObj);
                }
            }

            return rows;
        }

        /// <summary>
        /// use ng-repeat to create a table for the related instances
        /// </summary>
        /// <returns></returns>
        public override HtmlNode CreatePropertyNode()
        {
            HtmlNode container;

            if (_relatedInstanceView == null)
            {
                _relatedInstanceView = GetRelatedInstanceView(this.BaseInstanceView.DataView.BaseClass.ClassName); // BaseInstanceView owns the relationship attribute
            }

            int count = 0;

            if (!DataSetHelper.IsEmptyDataSet(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName))
            {
                count = DataSetHelper.GetRowCount(_relatedInstanceView.DataSet, _relatedInstanceView.DataView.BaseClass.ClassName);
            }

            if (_isReadOnly && count == 0)
            {
                container = this.InstanceEditor.Document.CreateElement("div");
                // hide the table when there is no data
                //container = this.InstanceEditor.Document.CreateElement("div ng-show='" + this.InstanceEditor.ParentEditor.ViewModelPath + this.InstanceEditor.InstanceClassName + "." + this.PropertyInfo.Name + ".length > 0'");
            }
            else
            {
                container = this.InstanceEditor.Document.CreateElement("div");
            }

            //container.SetAttributeValue("class", "table-responsive");

            HtmlNode tableNode = this.InstanceEditor.Document.CreateElement("table");
            tableNode.SetAttributeValue("class", "table table-striped");
            container.AppendChild(tableNode);

            // table header
            HtmlNode theadNode = this.InstanceEditor.Document.CreateElement("thead");
            tableNode.AppendChild(theadNode);

            HtmlNode trNode = this.InstanceEditor.Document.CreateElement("tr");
            theadNode.AppendChild(trNode);

            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = _relatedInstanceView.GetProperties(null);

            // create a table with properties in the related data view
            HtmlNode thNode;
            foreach (InstanceAttributePropertyDescriptor pd in properties)
            {
                thNode = this.InstanceEditor.Document.CreateElement("td");
                thNode.InnerHtml = HtmlDocument.HtmlEncode(pd.DisplayName);
                trNode.AppendChild(thNode);
            }

            string nestedFormName = this.PropertyInfo.Name + "Form";

            HtmlNode tbodyNode = this.InstanceEditor.Document.CreateElement("tbody");
            tbodyNode.SetAttributeValue("ng-repeat", "model in " + this.InstanceEditor.ParentEditor.ViewModelPath + this.InstanceEditor.InstanceClassName + "." + this.PropertyInfo.Name + " track by $index");
            // each instance field is contained in a ng-form so that angularjs validation can work
            //tbodyNode.SetAttributeValue("ng-form", nestedFormName);
            tableNode.AppendChild(tbodyNode);

            IInstanceEditor editor = new InstanceEditor();
            editor.EditInstance = _relatedInstanceView;
            editor.ConnectionString = this.InstanceEditor.ConnectionString;
            editor.Document = this.InstanceEditor.Document;
            editor.ParentEditor = this.InstanceEditor.ParentEditor;
            editor.FormName = nestedFormName;  // make sure to set this to make validation work, the controls are contained in a nested ng-form 
            editor.IsInlineForm = true; // the properties are placed in an inline form

            IPropertyControl vPropertyControl = null;
            HtmlNode propertyRoot;

            // create a row foreach related instance
            HtmlNode tdNode;
            trNode = this.InstanceEditor.Document.CreateElement("tr");
            trNode.SetAttributeValue("ng-form", nestedFormName);
            tbodyNode.AppendChild(trNode);

            for (int i = 0; i < properties.Count; i++)
            {
                InstanceAttributePropertyDescriptor vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[i];

                vPropertyControl = editor.CreatePropertyControl(vPropertyInfo.Name, _isReadOnly, null);

                tdNode = this.InstanceEditor.Document.CreateElement("td");
                trNode.AppendChild(tdNode);
                if (vPropertyControl != null)
                {
                    propertyRoot = vPropertyControl.CreatePropertyNode();

                    tdNode.AppendChild(propertyRoot);
                }
            }

            return container;
        }  // CreatePropertyNode()

        private InstanceView GetRelatedInstanceView(string relatedClassName)
        {
            InstanceView instanceView;
            string dataViewName = GetDataViewName();

            using (CMConnection connection = new CMConnection(this.InstanceEditor.ConnectionString))
            {
                connection.Open();

                DataViewModel dataView;

                // this.PropertiesEditor.EditInstance is the master instance view to which to get the related instances
                if (!string.IsNullOrEmpty(dataViewName))
                {
                    // Instance of the ParentEditor is the master instance
                    dataView = connection.MetaDataModel.GetRelatedDataView(dataViewName, this.InstanceEditor.ParentEditor.EditInstance, relatedClassName);
                }
                else
                {
                    // use default data view
                    dataView = connection.MetaDataModel.GetRelatedDefaultDataView(this.InstanceEditor.ParentEditor.EditInstance, relatedClassName);
                }

                dataView.PageSize = 500; // get maximum 500 records

                string query = dataView.SearchQuery;

                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                instanceView = new InstanceView(dataView, ds);
            }

            return instanceView;
        }

        // DataViewName is stored in NestedDataViewName attribute of the ClassElement
        private string GetDataViewName()
        {
            string dataViewName = null;
            string baseClassName = this.BaseInstanceView.DataView.BaseClass.ClassName;
            ClassElement baseClassElement = this.BaseInstanceView.DataView.SchemaModel.FindClass(baseClassName);

            _defaultRows = GetDefaultRowCount(baseClassElement);

            if (!string.IsNullOrEmpty(baseClassElement.NestedDataViewName))
            {
                dataViewName = baseClassElement.NestedDataViewName;
            }

            return dataViewName;
        }

        private int GetDefaultRowCount(ClassElement baseClassElement)
        {
            int rows = 1;

            try
            {
                if (!string.IsNullOrEmpty(baseClassElement.Description))
                {
                    Hashtable settings = InstanceView.ParseSettingString(baseClassElement.Description);

                    string str = (string)settings["rows"];
                    if (str != null)
                    {
                        rows = int.Parse(str);
                    }
                }
            }
            catch (Exception)
            {
                rows = 1;
            }

            return rows;
        }

        private JObject FindViewModelInstance(JProperty jProperty, string objId)
        {
            JObject relatedInstance = null;

            foreach (JObject instance in jProperty.Value)
            {
                if (instance.GetValue("obj_id") != null &&
                    instance.GetValue("obj_id").ToString() == objId)
                {
                    relatedInstance = instance;
                    break;
                }
            }

            return relatedInstance;
        }

        private InstanceView GetDetailedInstanceView(string oid)
        {
            using (CMConnection con = new CMConnection(this.InstanceEditor.ConnectionString))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(this.InstanceEditor.InstanceClassName);

                InstanceView instanceView;

                if (!string.IsNullOrEmpty(oid))
                {
                    // create an instance query
                    string query = dataView.GetInstanceQuery(oid);

                    CMCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    XmlReader reader = cmd.ExecuteXMLReader();
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                    {
                        instanceView = new InstanceView(dataView, ds);
                    }
                    else
                    {
                        instanceView = new InstanceView(dataView);
                    }
                }
                else
                {
                    instanceView = new InstanceView(dataView);
                }

                return instanceView;
            }
        }

        private HtmlNode CreateDeleteBtnElement(string arrayPath)
        {
            HtmlNode container = this.InstanceEditor.Document.CreateElement("span");
            container.SetAttributeValue("class", "btn btn-sm btn-danger");
            container.SetAttributeValue("ng-click", "removeArrayRow('" + arrayPath + "', $index)");

            HtmlNode iconNode = this.InstanceEditor.Document.CreateElement("i");
            iconNode.SetAttributeValue("class", "glyphicon glyphicon-remove");

            container.AppendChild(iconNode);

            return container;
        }

        private XmlElement FindViewModelInstance(XmlElement element, string objId)
        {
            XmlElement relatedInstance = null;

            foreach (XmlElement instance in element.ChildNodes)
            {
                if (instance["obj_id"] != null &&
                    instance["obj_id"].InnerText == objId)
                {
                    relatedInstance = instance;
                    break;
                }
            }

            return relatedInstance;
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
    }
}
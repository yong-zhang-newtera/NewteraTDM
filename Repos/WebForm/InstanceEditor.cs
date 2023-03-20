using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;
using Newtonsoft.Json.Linq;

using HtmlAgilityPack;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Logging;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Server.Logging;

namespace Newtera.WebForm
{
    /// <remarks>
    /// InstanceEditor is an utility class as a bridge between class model and view model.
    /// It builds:
    /// * default ui component for each property
    /// * default alidating rule for each property
    /// * translate model data to view model data for each property
    /// * translate view model data to model data for each property
    /// 
    /// </remarks>
    public class InstanceEditor : BaseInstanceEditor
    {
        private const string PK_PROPERTY = "obj_pk";
        private const string SUGGEST_PROPERTY = "suggest";

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
        /// Create a form template in html
        /// </summary>
        /// <param name="taskId">A workflow task id to which the form is created</param>
        public override string CreateFormTemplate(string taskId)
        {
            if (EditInstance != null) 
            {
                Document = new HtmlDocument();
                HtmlNode node = Document.CreateElement("fieldset");
                Document.DocumentNode.AppendChild(node);

                // get property read only status defined for a workflow task
                string schemaName = EditInstance.DataView.SchemaInfo.Name;
                _propertyReadOnlyStatusTable = GetPropertyReadOnlyStatusTable(schemaName, taskId);

                CreatePropertySections(node, EditInstance);

                return node.OuterHtml;
            }
            else
            {
                return "";
            }
        }  // CreateEditor()

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
            InstanceAttributePropertyDescriptor vPropertyInfo;
            List<IInstanceEditor> convertedInstances = new List<IInstanceEditor>();

            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);

            // iterate through the XML instance's properties and set the value to the corresponding property in the InstanceView object
            foreach (XmlElement childElement in element.ChildNodes)
            {
                BaseProperty vPropertyControl = null;
                vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[childElement.Name];

                if (vPropertyInfo != null)
                {
                    vPropertyControl = GetPropertyControl(vPropertyInfo);

                    if (vPropertyControl != null)
                    {
                        // Update EditInstance's property value with ViewModel value in childElement
                        vPropertyControl.UpdateValueInternal(childElement, false);
                    }  // if (vPropertyControl != null)
                }
                else if (childElement.ChildNodes != null && childElement.ChildNodes.Count > 0)
                {
                    // The xml element may represent a category, check if the nested elements represents an instance attribute property
                    foreach (InstanceAttributePropertyDescriptor pd in properties)
                    {
                        if (pd.Category == childElement.Name)
                        {
                            foreach (XmlElement grandChildElement in childElement.ChildNodes)
                            {
                                if (grandChildElement.Name == pd.Name)
                                {
                                    vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[grandChildElement.Name];

                                    if (vPropertyInfo != null)
                                    {
                                        vPropertyControl = GetPropertyControl(vPropertyInfo);

                                        if (vPropertyControl != null)
                                        {
                                            // Update EditInstance's property value with ViewModel value in grandChildElement
                                            vPropertyControl.UpdateValueInternal(grandChildElement, false);
                                        }  // if (vPropertyControl != null)
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return convertedInstances;
        }

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <param name="isFormFormat">true if the json values are in form format where enum values are internal values</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public override List<IInstanceEditor> ConvertToModel(JObject instance, bool isFormFormat)
        {
            InstanceAttributePropertyDescriptor vPropertyInfo;
            List<IInstanceEditor> convertedInstances = new List<IInstanceEditor>();

            // get the properties for this instance in a specified order
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
                        // Update EditInstance's property value with ViewModel value in jProperty
                        vPropertyControl.UpdateValueInternal(jproperty, isFormFormat);
                    }  // if (vPropertyControl != null)
                }
            }

            return convertedInstances;
        }

        /// <summary>
        /// Validate the model represented by the InstanceView instance against the schema.
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>Return if the instance model is valid. It will throw an exception if a validating error occures </returns>
        public override void Validate()
        {
            InstanceAttributePropertyDescriptor vPropertyInfo;
            BaseProperty vPropertyControl;

            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);

            if (this.EditInstance.InstanceCount > 0)
            {
                DataTable dt = this.EditInstance.DataSet.Tables[this.EditInstance.DataView.BaseClass.ClassName];

                if (dt != null)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[column.ColumnName];

                        if (vPropertyInfo != null)
                        {
                            // Get corresponding control to convert the value
                            vPropertyControl = GetPropertyControl(vPropertyInfo);

                            if (vPropertyControl != null)
                            {
                                // An exception is thrown if there is a validating error
                                vPropertyControl.Validate();
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (InstanceAttributePropertyDescriptor pd in properties)
                {
                    // Get corresponding control to convert the value
                    vPropertyControl = GetPropertyControl(pd);

                    if (vPropertyControl != null)
                    {
                    // An exception is thrown if there is a validating error
                        vPropertyControl.Validate();
                    }
                }
            }
        }

        /// <summary>
        /// Save the the master instance and related instances to the database 
        /// </summary>
        /// <param name="">A list of related instances</param>
        /// <returns>The obj_id of the saved instance</returns>
        public override string SaveToDatabase(List<IInstanceEditor> relatedEditInstances)
        {
            return null;
        }

        /// <summary>
        /// ConvertToViewModel converts values from the InstanceView instance (Model) to a JSON instance (ViewModel)
        /// the EditInstance
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>The JSON instance</returns>
        public override JObject ConvertToViewModel(bool convert)
        {
            JObject instance = new JObject();
            InstanceAttributePropertyDescriptor vPropertyInfo;
            BaseProperty vPropertyControl;

            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);

            if (this.EditInstance.InstanceCount > 0)
            {
                DataTable dt = this.EditInstance.DataSet.Tables[this.EditInstance.DataView.BaseClass.ClassName];

                if (dt != null)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName == NewteraNameSpace.PERMISSION)
                        {
                            // add permission properties to the view model
                            AddPermissionProperties(instance, EditInstance.InstanceData.GetPermissionValue());
                        }
                        if (column.ColumnName == NewteraNameSpace.ATTACHMENTS)
                        {
                            instance.Add("attachments", EditInstance.InstanceData.GetANUM());
                        }
                        else if (convert)
                        {
                            vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[column.ColumnName];
                           
                            if (vPropertyInfo != null)
                            {
                                // Get corresponding control to convert the value
                                vPropertyControl = GetPropertyControl(vPropertyInfo);

                                if (vPropertyControl != null)
                                {
                                    // each property control implements GetPropertyViewModel method
                                    JToken pViewModel = vPropertyControl.GetPropertyViewModel();
                                    if (pViewModel != null)
                                        instance.Add(vPropertyInfo.Name, pViewModel);

                                }  // if (vPropertyControl != null)
                                else
                                {
                                    // unable to get a control, no conversion
                                    instance.Add(column.ColumnName, dt.Rows[EditInstance.SelectedIndex][column.ColumnName].ToString());
                                }
                            }
                            else
                            {
                                // it is one of the internal attributes, such as obj_id, type, attachments
                                // add it to the JObject
                                instance.Add(column.ColumnName, dt.Rows[EditInstance.SelectedIndex][column.ColumnName].ToString());
                            }
                        }
                        else
                        {
                            vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[column.ColumnName];
                            if (vPropertyInfo != null &&
                                vPropertyInfo.HasConstraint &&
                                vPropertyInfo.Constraint is EnumElement &&
                                ((EnumElement)vPropertyInfo.Constraint).DisplayMode == EnumDisplayMode.Image)
                            {
                                // convert the value to image name
                                string imageName = GetImageName(vPropertyInfo, dt.Rows[EditInstance.SelectedIndex][column.ColumnName].ToString());
                                instance.Add(column.ColumnName, imageName);
                            }
                            else
                            {
                                instance.Add(column.ColumnName, dt.Rows[EditInstance.SelectedIndex][column.ColumnName].ToString());
                            }
                        }
                    }

                    AddPrimaryKeyValueProperty(instance); // add a property to provide the primary key value if any, for convinient purpose

                    AddRelationshipPrimaryKeyProperties(instance, properties, this.EditInstance.DataView);
                }
            }
            else
            {
                if (convert)
                {
                    // empty instance, generate JObject with initial values
                    foreach (InstanceAttributePropertyDescriptor pd in properties)
                    {
                        // Get corresponding control to convert the value
                        vPropertyControl = GetPropertyControl(pd);

                        if (vPropertyControl != null)
                        {
                            // each property control implements GetPropertyViewModel method
                            JToken val = vPropertyControl.GetPropertyViewModel();
                            if (val != null)
                                instance.Add(pd.Name, val);
                        } 
                        else
                        {
                            // unable to get a control, add empty string
                            instance.Add(pd.Name, "");
                        }
                    }

                    ClassElement baseClassElement = EditInstance.DataView.BaseClass.GetSchemaModelElement() as ClassElement;
                    if (baseClassElement != null &&
                        PermissionChecker.Instance.HasPermission(EditInstance.DataView.SchemaModel.MetaData.XaclPolicy,
                        baseClassElement, XaclActionType.Create))
                    {
                        instance.Add("allowCreate", true);
                    }
                    else
                    {
                        instance.Add("allowCreate", false);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// ConvertToIndexingDocument converts values from the InstanceView instance (Model) to a JSON instance as a document
        /// for full-text search indexing
        /// </summary>
        /// <returns>The JSON instance</returns>
        public override JObject ConvertToIndexingDocument()
        {
            JObject instance = new JObject();
            InstanceAttributePropertyDescriptor vPropertyInfo;

            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = GetPropertyDescriptorList(EditInstance);

            if (this.EditInstance.InstanceCount > 0)
            {
                DataTable dt = this.EditInstance.DataSet.Tables[this.EditInstance.DataView.BaseClass.ClassName];

                if (dt != null)
                {
                    JArray suggesters = new JArray();

                    foreach (DataColumn column in dt.Columns)
                    {
                        vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[column.ColumnName];
                        if (vPropertyInfo != null)
                        {
                            if (!vPropertyInfo.IsForFullTextSearch &&
                                !vPropertyInfo.IsVirtual &&
                                !vPropertyInfo.IsImage &&
                                !vPropertyInfo.IsArray &&
                                !vPropertyInfo.IsHistoryEdit &&
                                !vPropertyInfo.IsRichText &&
                                !vPropertyInfo.IsHidden)
                            {
                                string val = dt.Rows[EditInstance.SelectedIndex][column.ColumnName].ToString();
                                if (vPropertyInfo.DataType == DataType.Date ||
                                    vPropertyInfo.DataType == DataType.DateTime)
                                {
                                    if (string.IsNullOrEmpty(val))
                                        val = null;
                                    else if (vPropertyInfo.DataType == DataType.DateTime)
                                        val = val.Replace("T", " ");
                                }
                                else if (vPropertyInfo.DataType == DataType.Boolean)
                                {
                                    if (LocaleInfo.Instance.IsTrue(val))
                                        val = "true";
                                    else if (LocaleInfo.Instance.IsFalse(val))
                                        val = "false";
                                    else
                                        val = null;
                                }

                                if (!string.IsNullOrEmpty(val))
                                {
                                    instance.Add(column.ColumnName, val);

                                    // add to suggester array
                                    if (vPropertyInfo.IsGoodForSearchSuggester)
                                        suggesters.Add(val);
                                }
                            }
                        }
                        else if (column.ColumnName == NewteraNameSpace.OBJ_ID)
                        {
                            instance.Add(column.ColumnName, dt.Rows[EditInstance.SelectedIndex][column.ColumnName].ToString());
                        }
                    }

                    if (suggesters.Count > 0)
                        instance.Add(SUGGEST_PROPERTY, suggesters);
                }
            }

            return instance;
        }

        /// <summary>
        /// Add properties to the JSON instance to display the primary key values of the related class
        /// for the relationships that required a foreign key,such as, many-to-one, one-to-one on the joined manager side
        /// </summary>
        /// <param name="dataView">Meta model of the data instance</param>
        private void AddRelationshipPrimaryKeyProperties(JObject instance, PropertyDescriptorCollection properties, DataViewModel dataView)
        {
            BaseProperty vPropertyControl;

            foreach (IDataViewElement resultAttribute in dataView.ResultAttributes)
            {
                if (resultAttribute is DataRelationshipAttribute)
                {
                    RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)resultAttribute.GetSchemaModelElement();

                    // add a property to display primary key value(s) of the relationship
                    //AddPKColumn(instance, dataView, relationshipAttribute);

                    InstanceAttributePropertyDescriptor vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[resultAttribute.Name];

                    if (vPropertyInfo != null)
                    {
                        // Get corresponding control to convert the value
                        vPropertyControl = GetPropertyControl(vPropertyInfo);

                        if (vPropertyControl != null)
                        {
                            // each property control implements GetPropertyViewModel method
                            JToken val = vPropertyControl.GetPropertyViewModel();
                            if (val != null)
                                instance.Add(vPropertyInfo.Name, val);
                        }  // if (vPropertyControl != null)
                    }
                }
            }
        }

        /// <summary>
        /// Add primary key property to the JSON instance for convinient purpose
        /// </summary>
        private void AddPrimaryKeyValueProperty(JObject instance)
        {
            instance.Add(PK_PROPERTY, this.EditInstance.InstanceData.PrimaryKeyValues);
        }

        private string GetImageName(InstanceAttributePropertyDescriptor vPropertyInfo, string propertyValue)
        {
            string imageName = "";

            EnumElement enumElement = vPropertyInfo.Constraint as EnumElement;
            if (enumElement != null)
            {
                imageName = enumElement.GetImageNameByText(propertyValue);
            }
            return imageName;
        }

        /// <summary>
        /// Add a property to the instance to store values of primary keys for
        /// a relationship attributes
        /// </summary>
        /// <param name="instance">JSON instance</param>
        /// <param name="dataView">Meta-data</param>
        /// <param name="relationshipElement">The relationship element</param>
        /*
        private void AddPKColumn(JObject instance, DataViewModel dataView, RelationshipAttributeElement relationshipElement)
        {
            DataSet ds = this.EditInstance.DataSet;

            DataTable baseTable = ds.Tables[dataView.BaseClass.ClassName];
            // Note: do not change dataView.BaseClass.Name to dataView.BaseClass.ClassName,
            // it will fail if the ClassName of the data view is set to a leaf class
            DataTable relatedTable = ds.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(dataView.BaseClass.Name, relationshipElement.Name)];
            if (relatedTable != null)
            {
                // add a new property to the instance to for the primary key values of the class
                // referenced by the relatonship.
                string propertyName = relationshipElement.Name;
                string propertyValue = GetPrimaryKeyValues(relatedTable, relationshipElement, EditInstance.SelectedIndex);

                if (propertyName == "TestProcedure")
                {
                    //ErrorLog.Instance.WriteLine("Add relationship value for " + propertyName + " with value " + propertyValue);
                }

                instance.Add(propertyName, propertyValue);
            }
        }
        */

        /// <summary>
        /// Gets the concatenated values of primary keys of a class referenced by a relationship attribute.
        /// </summary>
        /// <param name="relatedTable">The DataTable</param>
        /// <param name="relationshipElement">The relationship attribute</param>
        /// <param name="rowIndex">The current row index.</param>
        /// <returns>The concatenated primary key values</returns>
        /*
        private string GetPrimaryKeyValues(DataTable relatedTable, RelationshipAttributeElement relationshipElement,
            int rowIndex)
        {
            string val = "";

            SchemaModelElementCollection primaryKeys = null;
            ClassElement currentClassElement = relationshipElement.LinkedClass;
            while (currentClassElement != null)
            {
                primaryKeys = currentClassElement.PrimaryKeys;
                if (primaryKeys != null && primaryKeys.Count > 0)
                {
                    break;
                }

                currentClassElement = currentClassElement.ParentClass;
            }

            if (primaryKeys != null && primaryKeys.Count > 0)
            {
                foreach (SimpleAttributeElement pk in primaryKeys)
                {
                    if (relatedTable.Columns[pk.Name] != null &&
                        rowIndex < relatedTable.Rows.Count &&
                        !relatedTable.Rows[rowIndex].IsNull(pk.Name))
                    {
                        if (string.IsNullOrEmpty(val))
                        {
                            val = relatedTable.Rows[rowIndex][pk.Name].ToString();
                        }
                        else
                        {
                            // separate the primary keys with semicolumn
                            val += ";" + relatedTable.Rows[rowIndex][pk.Name].ToString();
                        }
                    }
                }
            }

            return val;
        }
        */

        /// <summary>
        /// CreatePropertySections iterates through the properties for the supplied instanceView
        /// and builds elements of the section for those that pass the filtering (via FilterOut()). 
        /// The property's data type and access determines which BaseProperty control is used for viewing and editing
        /// the data.
        /// <param name="parentNode">Add html nodes to the parent node as children</param>
        /// <param name="instanceView">If its a new page, pass a real instance that we are processing. On postback, pass null.</param>
        /// </summary>
        protected void CreatePropertySections(HtmlNode parentNode, InstanceView instanceView)
        {
            bool isReadOnly;

            // get the properties for this instance in a specified order
            PropertyDescriptorCollection properties = GetPropertyDescriptorList(instanceView);

            for (int i = 0; i < properties.Count; i++)
            {
                // investigate one property. We'll:
                // 1. Determine its attribs: Name and value
                // 2. See if it can be filtered out
                // 3. Create a BaseProperty subclass based on its data type and access.
                // 4. Build a PropertySection with the name label, BaseProperty subclass, and type label
                BaseProperty vPropertyControl = null;
                InstanceAttributePropertyDescriptor vPropertyInfo = (InstanceAttributePropertyDescriptor)properties[i];

                string vPropName = HtmlDocument.HtmlEncode(vPropertyInfo.DisplayName);

                // Filter out any inappropriate or undesirable properties
                if (FilterOut(vPropertyInfo))
                    continue;

                // create the appropriate BaseProperty subclass
                if (IsViewOnly ||
                    vPropertyInfo.IsReadOnly ||
                    IsPropertyReadOnly(vPropertyInfo.Name))
                {
                    isReadOnly = true;
                }
                else
                {
                    isReadOnly = false;
                }

                vPropertyControl = CreatePropertyControl(vPropertyInfo, isReadOnly, null);

                if (vPropertyControl != null)
                {
                    // add a new section to the table with a label, vPropertyControl and the class name in the third column
                    PropertySection vPropertySection = CreateOneSection(parentNode, instanceView,
                        true, vPropName, vPropertyControl, vPropertyInfo);

                }  // if (vPropertyControl != null)
            }  // foreach
        }  // CreatePropertySections()


        /// <summary>
        /// CreateOneSection creates one section in the template using the BaseProperty control subclass pPropertyControl.
        /// A section is based on PropertySection,
        /// It returns the new section.
        /// </summary>
        protected virtual PropertySection CreateOneSection(HtmlNode parentNode, InstanceView instanceView,
            bool pVisibleB, string pLeadText, BaseProperty pPropertyControl,
            InstanceAttributePropertyDescriptor pPropertyInfo)
        {
            // Throughout this area, we are very selective to what gets its ViewState enabled
            // to avoid excessive ViewState's on the page.
            PropertySection vPropertySection = new PropertySection(this.Document, pPropertyInfo);
            vPropertySection.fPropertyControl = pPropertyControl;

            // fill the first element with the Property Name (with any heading names)
            HtmlNode node = this.Document.CreateElement("label");
            node.InnerHtml = HtmlDocument.HtmlEncode(pLeadText);
            //node.SetAttributeValue("class", "control-label");
            vPropertySection.HtmlNode.AppendChild(node);
            parentNode.AppendChild(vPropertySection.HtmlNode);

            // fill in the child nodes created for the property
            HtmlNode propertyNode = pPropertyControl.CreatePropertyNode();
            if (propertyNode != null)
            {
                vPropertySection.HtmlNode.AppendChild(propertyNode);
            }

            return vPropertySection;
        }  // CreateOneSection()


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

            public PropertySection(HtmlDocument doc, InstanceAttributePropertyDescriptor propertyInfo)
            {

                HtmlNode = doc.CreateElement("div");
                HtmlNode.SetAttributeValue("class", "form-group");
            }
        }  // class PropertySection

    }
}


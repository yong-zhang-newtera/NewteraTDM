using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections;
using Word = Microsoft.Office.Interop.Word;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;

namespace Newtera.SmartWordUtil
{
    public abstract class DocDataSourceBase : IDocDataSource
    {
        protected const int PopulateLineLimit = 10000;

        private MetaDataModel _metaData;
        private string _baseClassName;

        public DocDataSourceBase()
        {
            _metaData = null;
            _baseClassName = null;
        }

        /// <summary>
        /// Gets or sets the meta data model of the data source.
        /// </summary>
        public virtual MetaDataModel MetaData
        {
            get
            {
                return _metaData;
            }
            set
            {
                _metaData = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the selected base class in the data source.
        /// </summary>
        public abstract string BaseClassName {get;set;}

        /// <summary>
        /// Gets information indicating whether the given class is an inherited class to the base class
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public abstract bool IsInheitedClass(string className);

        /// <summary>
        /// Gets an InstanceView representing a data instance that is related to the data instances
        /// selected in the result grid.
        /// </summary>
        /// <param name="familyNode">The family node that contains the related view node</param>
        /// <param name="viewNode">The view node presenting a related view</param>
        /// <returns>An InstanceView representing a data instance</returns>
        public virtual InstanceView GetInstanceView(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            return null;
        }

        /// <summary>
        /// Gets an InstanceView representing the selected instance of the base class.
        /// </summary>
        /// <returns>An InstanceView representing the data instance for the specified row</returns>
        public abstract InstanceView GetInstanceView();

        // Get the meta data element represented by attribute values of the view node
        public IMetaDataElement GetViewNodeMetaDataElement(string elementName, string elementType, string taxonomyName)
        {
            IMetaDataElement metaDataElement = null;

            switch (elementType)
            {
                case WordPopulator.ClassType:
                    // elementName is a class name
                    metaDataElement = MetaData.SchemaModel.FindClass(elementName);

                    break;

                case WordPopulator.DataViewType:
                    // elementName is a data view name
                    metaDataElement = (DataViewModel)MetaData.DataViews[elementName];

                    break;

                case WordPopulator.TaxonType:
                    // elementName is a taxon name
                    TaxonomyModel taxonomy = (TaxonomyModel)MetaData.Taxonomies[taxonomyName];
                    if (taxonomy != null)
                    {
                        metaDataElement = taxonomy.FindNode(elementName);
                    }

                    break;
            }

            return metaDataElement;
        }

        /// <summary>
        /// Gets the base class name that is associated with a family node
        /// </summary>
        /// <param name="familyNode">The family node</param>
        /// <returns>The base class name of family node</returns>
        public string GetViewClassName(Word.XMLNode familyNode, out string baseClassCaption)
        {
            string className = null;
            DataViewModel dataView = null;
            baseClassCaption = "";

            string elementName = GetAttributeValue(familyNode, WordPopulator.ElementAttribute, false);
            string elementType = GetAttributeValue(familyNode, WordPopulator.TypeAttribute, true);
            string taxonomyName = GetAttributeValue(familyNode, WordPopulator.TaxonomyAttribute, false);

            switch (elementType)
            {
                case WordPopulator.ClassType:
                    // elementName is a class name
                    ClassElement classElement = MetaData.SchemaModel.FindClass(elementName);
                    if (classElement != null)
                    {
                        className = classElement.Name;
                        baseClassCaption = classElement.Caption;
                    }

                    break;

                case WordPopulator.DataViewType:
                    // elementName is a data view name
                    dataView = (DataViewModel)MetaData.DataViews[elementName];
                    if (dataView != null)
                    {
                        className = dataView.BaseClass.Name;
                        baseClassCaption = dataView.BaseClass.Caption;
                    }

                    break;

                case WordPopulator.TaxonType:
                    // elementName is a taxon name
                    TaxonomyModel taxonomy = (TaxonomyModel)MetaData.Taxonomies[taxonomyName];
                    if (taxonomy != null)
                    {
                        TaxonNode taxon = taxonomy.FindNode(elementName);
                        if (taxon != null)
                        {
                            dataView = taxon.GetDataView(null);
                            if (dataView != null)
                            {
                                className = dataView.BaseClass.Name;
                                baseClassCaption = dataView.BaseClass.Caption;
                            }
                        }
                    }

                    break;
            }

            return className;
        }

        /// <summary>
        /// Gets a DataView representing a view node that is related to the base class.
        /// </summary>
        /// <param name="familyNode">The family node that contains the related view node</param>
        /// <param name="viewNode">The view node presenting a related view</param>
        /// <returns>A DataViewModel object representing a view node that is related to the base class</returns>
        public DataViewModel GetDataView(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            DataViewModel dataView = null;

            // get the data view associated with a view node
            dataView = GetViewNodeDataView(viewNode);

            if (dataView != null)
            {
                // add relationship path and search value to the data view so that it will yield the
                // data instances that are related to the selected data instance
                string baseClassCaption = null;
                string baseClassName = GetViewClassName(familyNode, out baseClassCaption);
                string path = GetAttributeValue(viewNode, WordPopulator.PathAttribute, false);
                AddDataViewConstraint(dataView, baseClassName, path);
            }

            return dataView;
        }

        /// <summary>
        /// Add necessary referenced classes and search criteria to the data view so that it can retrieve
        /// the data instances that are related to the selected data instances.
        /// </summary>
        /// <param name="dataView">A data view for a view node within a family node</param>
        /// <param name="baseClassName">The base class name represented by a family node</param>
        /// <param name="path">Path of relationships between the data view and the base class</param>
        private void AddDataViewConstraint(DataViewModel dataView, string baseClassName, string path)
        {
            // Get relationship path between the data view and the base class,
            // starting from the data view side
            string[] relationshipNames = null;
            if (path != null && path.Length > 0)
            {
                relationshipNames = path.Split(';');
            }

            if (relationshipNames.Length > 0)
            {
                ClassElement classElement = MetaData.SchemaModel.FindClass(dataView.BaseClass.Name);
                DataClass referringClass = dataView.BaseClass;

                // add the referenced classes to the data view according to the path
                bool isCompleted = true;
                bool found = false;
                for (int i = 0; i < relationshipNames.Length; i++)
                {
                    int pos = relationshipNames[i].IndexOf(":");
                    string ownerClassName = relationshipNames[i].Substring(0, pos); // class that owns the relationship
                    string relationshipName = relationshipNames[i].Substring(pos + 1);

                    found = false;
                    while (classElement != null)
                    {
                        foreach (RelationshipAttributeElement relationship in classElement.RelationshipAttributes)
                        {
                            if (relationship.BackwardRelationshipName == relationshipName &&
                                relationship.LinkedClassName == ownerClassName)
                            {
                                found = true; // found relationship

                                referringClass = AddReferencedClass(dataView, relationship, referringClass);

                                // be ready to move to match the next relationship in the path
                                classElement = relationship.LinkedClass;

                                break;
                            }
                        }

                        if (!found)
                        {
                            // go up class hierarchy
                            classElement = classElement.ParentClass;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                    else if (classElement == null)
                    {
                        // unable to find the relationship
                        isCompleted = false;
                        break;
                    }
                }

                if (isCompleted && referringClass != null)
                {
                    // if the process above is completed, the referringClass should represent
                    // the base class, set the corresponding obj_id as search value
                    RelationshipAttributeElement relationshipAttribute = referringClass.ReferringRelationship.BackwardRelationship;

                    // build a search expression that retrieve the data instances of the related class that
                    // are associated with the selected data instance
                    string objId = null;
                    InstanceView instanceView = GetInstanceView(); // the instance view represent the currently selected data instance
                    IDataViewElement expr = null;
                    DataSimpleAttribute left;
                    Parameter right;

                    if (relationshipAttribute.IsForeignKeyRequired)
                    {
                        // it is a many-to-one relationship between base class and related class,
                        // gets the obj_id of the related instance from the table
                        if (instanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(instanceView.DataView.BaseClass.Name, relationshipAttribute.Name)] != null)
                        {
                            if (instanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(instanceView.DataView.BaseClass.Name, relationshipAttribute.Name)].Rows[0].IsNull(NewteraNameSpace.OBJ_ID) == false)
                            {
                                objId = instanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(instanceView.DataView.BaseClass.Name, relationshipAttribute.Name)].Rows[0][NewteraNameSpace.OBJ_ID].ToString();
                            }

                            if (string.IsNullOrEmpty(objId))
                            {
                                objId = "0";
                            }
                        }

                        left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.ReferringClassAlias);
                        right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.ReferringClassAlias, DataType.String);

                        right.ParameterValue = objId;
                        expr = new RelationalExpr(ElementType.Equals, left, right);
                    }
                    else
                    {
                        // it is a one-to-many relationship between base class and related class,
                        // get the obj_id of the selected data instance
                        objId = instanceView.InstanceData.ObjId;

                        left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.Alias);
                        right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.Alias, DataType.String);

                        right.ParameterValue = objId;
                        expr = new RelationalExpr(ElementType.Equals, left, right);
                    }

                    if (expr != null)
                    {
                        // add search expression to the dataview
                        dataView.AddSearchExpr(expr, ElementType.And);
                    }
                }
            }
        }

        private DataClass AddReferencedClass(DataViewModel dataView, RelationshipAttributeElement relationship, DataClass referringClass)
        {
            DataClass referencedClass = null;
            // check to see if the class has been added as a referenced class
            bool existed = false;
            foreach (DataClass refClass in dataView.ReferencedClasses)
            {
                if (refClass.ReferringClassAlias == referringClass.Alias &&
                    refClass.ReferringRelationshipName == relationship.Name)
                {
                    existed = true;
                    referencedClass = refClass;
                    break;
                }
            }

            if (!existed)
            {
                // create a referenced class and add it to data view
                // Add the linked class as a referenced class to data view
                referencedClass = new DataClass(relationship.LinkedClassAlias,
                    relationship.LinkedClassName, DataClassType.ReferencedClass);
                referencedClass.ReferringClassAlias = referringClass.Alias;
                referencedClass.ReferringRelationshipName = relationship.Name;
                referencedClass.Caption = relationship.LinkedClass.Caption;
                referencedClass.ReferringRelationship = relationship;
                dataView.ReferencedClasses.Add(referencedClass);
            }

            return referencedClass;
        }

        /// <summary>
        /// Gets the data view object associted with a view node
        /// </summary>
        /// <param name="viewNode">The view node</param>
        /// <returns>The data view associated with a view node</returns>
        private DataViewModel GetViewNodeDataView(Word.XMLNode viewNode)
        {
            DataViewModel dataView = null;

            string elementName = GetAttributeValue(viewNode, WordPopulator.ElementAttribute, false);
            string elementType = GetAttributeValue(viewNode, WordPopulator.TypeAttribute, true);
            string taxonomyName = GetAttributeValue(viewNode, WordPopulator.TaxonomyAttribute, false);

            IMetaDataElement metaDataElement = GetViewNodeMetaDataElement(elementName, elementType, taxonomyName);

            if (metaDataElement != null)
            {

                if (!PermissionChecker.Instance.HasPermission(MetaData.XaclPolicy, (IXaclObject)metaDataElement, XaclActionType.Read))
                {
                    string msg = string.Format(Newtera.SmartWordUtil.MessageResourceManager.GetString("SmartWord.NoReadPermission"), metaDataElement.Caption);
                    throw new Exception(msg);
                }

                switch (elementType)
                {
                    case WordPopulator.ClassType:
                        // elementName is a class name
                        dataView = MetaData.GetDetailedDataView(elementName);

                        break;

                    case WordPopulator.DataViewType:
                        // elementName is a data view name
                        dataView = (DataViewModel)MetaData.DataViews[elementName];

                        break;

                    case WordPopulator.TaxonType:
                        // elementName is a taxon name
                        TaxonomyModel taxonomy = (TaxonomyModel)MetaData.Taxonomies[taxonomyName];
                        if (taxonomy != null)
                        {
                            TaxonNode taxon = taxonomy.FindNode(elementName);
                            if (taxon != null)
                            {
                                dataView = taxon.GetDataView(null);
                            }
                        }

                        break;
                }
            }

            return dataView;
        }

        /// <summary>
        /// Get value of the specified attribute from a XMLNode
        /// </summary>
        /// <param name="xmlNode">The XMLNode</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="isRequired">indicating whether the attribute is a required attribute</param>
        /// <returns>attribute value</returns>
        /// <exception cref="Exception">Thrown if it is unable to find a required attribute</exception>
        protected string GetAttributeValue(Word.XMLNode xmlNode, string attributeName, bool isRequired)
        {
            string val = null;
            foreach (Word.XMLNode attr in xmlNode.Attributes)
            {
                if (attr.BaseName == attributeName)
                {
                    val = attr.NodeValue;
                }
            }

            if (val == null && isRequired)
            {
                throw new Exception("Unable to find attribute " + attributeName + " in xml node " + xmlNode.BaseName);
            }

            return val;
        }
    }
}
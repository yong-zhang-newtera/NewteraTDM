/*
* @(#)XMLSchemaElement.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Xml;
    using System.Xml.Schema;
	using System.Data;
    using System.Text;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A XMLSchemaElement is an object-representation of a specific xml schema element
	/// </summary>
	/// 
    /// <version>  	1.0.0 10 Aug 2014</version>
	public class XMLSchemaElement : XMLSchemaNodeBase, IXaclObject
	{
        private string _elementType;
        private string _minOccurs;
        private string _maxOccurs;
        private StringCollection _importScriptNames;
        private XMLElementDataSourceOption _dataSourceOption;
        private bool _isMergingInstances;
        private string _instanceIdentifyAttribute;
        private string _validateCondition;
        private string _filterCondition;
        private bool _isSortEnabled;
        private int _sortOrder;
        private bool _isSortAscending;
        private bool _isXAxis;
        private bool _isCategoryAxis;
        private MachineLearningCategory _mlCategory;

        /// <summary>
        /// Initiating an instance of XMLSchemaElement class
        /// </summary>
        /// <param name="name">Name of the data view</param>
        public XMLSchemaElement(string name) : base(name)
		{
            _elementType = "xs:string"; // default type
            _minOccurs = "0";
            _maxOccurs = "1";
            _importScriptNames = null;
            _dataSourceOption = XMLElementDataSourceOption.Database; // default
            _isMergingInstances = false;
            _instanceIdentifyAttribute = null;
            _validateCondition = null;
            _filterCondition = null;
            _isSortEnabled = false;
            _sortOrder = 0;
            _isSortAscending = true;
            _isXAxis = false;
            _isCategoryAxis = false;
            _mlCategory = MachineLearningCategory.None;
        }

		/// <summary>
		/// Initiating an instance of XMLSchemaElement class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XMLSchemaElement(XmlElement xmlElement) : base()
        {
            Unmarshal(xmlElement);
        }

		/// <summary>
		/// Gets the type of Node
		/// </summary>
        /// <value>One of XMLSchemaNodeType values</value>
		[BrowsableAttribute(false)]
        public override XMLSchemaNodeType NodeType 
		{
			get
			{
                return XMLSchemaNodeType.XMLSchemaElement;
			}
		}

        /// <summary>
        /// Gets or set the type of the element
        /// </summary>
        /// <value>One of xml schema element types or name of a complex type included in the xml schema</value>
        [
             CategoryAttribute("System"),
             DescriptionAttribute("The element type"),
             ReadOnlyAttribute(true),
        ]	
        public string ElementType
        {
            get
            {
                return _elementType;
            }
            set
            {
                _elementType = value;
            }
        }

        /// <summary>
        /// Gets or set the minOccurs of the element
        /// </summary>
        /// <value>One or zero</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The min accurs"),
            ReadOnlyAttribute(true),
        ]	
        public string MinOccurs
        {
            get
            {
                return _minOccurs;
            }
            set
            {
                _minOccurs = value;
            }
        }

        /// <summary>
        /// Gets or set the maxOccurs of the element
        /// </summary>
        /// <value>One or unbounded</value>
        [
           CategoryAttribute("System"),
           DescriptionAttribute("The max accurs"),
           ReadOnlyAttribute(true),
        ]	
        public string MaxOccurs
        {
            get
            {
                return _maxOccurs;
            }
            set
            {
                _maxOccurs = value;
            }
        }

        /// <summary>
        /// Gets or set a collection of import script names that are responsible for import data from other data sources to create the xml elements
        /// </summary>
        /// <value>A StringCollection object</value>
        [
           CategoryAttribute("System"),
           DescriptionAttribute("Names of import scripts used to import data from data sources other than database"),
           ReadOnlyAttribute(true),
        ]
        public StringCollection ImportScriptNames
        {
            get
            {
                return _importScriptNames;
            }
            set
            {
                _importScriptNames = value;
            }
        }

        /// <summary>
        /// Gets or set data source option
        /// </summary>
        /// <value>One of the XMLElementDataSourceOption values, default is Database</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Specify the data source for xml element values"),
            ReadOnlyAttribute(true),
        ]
        public XMLElementDataSourceOption DataSourceOption
        {
            get
            {
                return _dataSourceOption;
            }
            set
            {
                _dataSourceOption = value;
            }
        }

        /// <summary>
        /// Gets or set information indicating whether to merge the xml element instances when data come from the multiple data sources
        /// </summary>
        /// <value>True or false, default is false</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("whether to merge the xml element instances when data come from the multiple data sources"),
            ReadOnlyAttribute(true),
        ]
        public bool IsMergingInstances
        {
            get
            {
                return _isMergingInstances;
            }
            set
            {
                _isMergingInstances = value;
            }
        }

        /// <summary>
        /// Gets or set the xml element attribute whose value uniquely identifies an instance
        /// </summary>
        /// <value>One of the elemnet name</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("the xml element attribute whose value uniquely identifies an instance"),
            ReadOnlyAttribute(true),
        ]
        public string InstanceIdentifyAttribute
        {
            get
            {
                return _instanceIdentifyAttribute;
            }
            set
            {
                _instanceIdentifyAttribute = value;
            }
        }

        /// <summary>
        /// Gets or set the condition that validates the data imported from specified data sources.
        /// </summary>
        /// <value>XQuery expression</value>
        /// <remarks>when specified, the condition is used to run against the dataset generated by the import scripts to determine if the import data is valid or not</remarks>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("the condition that validates the data imported from specified data sources"),
            ReadOnlyAttribute(true),
        ]
        public string ValidateCondition
        {
            get
            {
                return _validateCondition;
            }
            set
            {
                _validateCondition = value;
            }
        }

        /// <summary>
        /// Gets or set the condition that filters qualifies instances from the specified data sources
        /// </summary>
        /// <value>XQuery expression</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("the condition that filters qualifies instances from the specified data sources"),
            ReadOnlyAttribute(true),
        ]
        public string FilterCondition
        {
            get
            {
                return _filterCondition;
            }
            set
            {
                _filterCondition = value;
            }
        }

        /// <summary>
        /// Gets or set information indicating whether the value of this element is used for sorting
        /// </summary>
        /// <value>True or false, default is false</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("whether to sort xml elements using this element value"),
            ReadOnlyAttribute(true)
        ]
        public bool IsSortEnabled
        {
            get
            {
                return _isSortEnabled;
            }
            set
            {
                _isSortEnabled = value;
            }
        }

        /// <summary>
        /// Gets or set sort order when there are multiple elements participating in the sort
        /// </summary>
        /// <value>An integer number, default is 0</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("sort order when there are multipl elements used in sort"),
            ReadOnlyAttribute(true)
        ]
        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
            }
        }

        /// <summary>
        /// Gets or set information indicating whether sort direction is ascending or desceding
        /// </summary>
        /// <value>True for ascending or false for descending, default is True</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("whether to sort direction is ascending"),
            ReadOnlyAttribute(true)
        ]
        public bool IsSortAscending
        {
            get
            {
                return _isSortAscending;
            }
            set
            {
                _isSortAscending = value;
            }
        }

        [Browsable(false)]
        public bool IsXAxis
        {
            get
            {
                return _isXAxis;
            }
            set
            {
                _isXAxis = value;
            }
        }

        [Browsable(false)]
        public bool IsCategoryAxis
        {
            get
            {
                return _isCategoryAxis;
            }
            set
            {
                _isCategoryAxis = value;
            }
        }

        /// <summary>
        /// Gets or set information indicating if the element is used as feature or label for machine learning
        /// </summary>
        /// <value>One of the MachineLearningCategory values, default is None</value>
        [Browsable(false)]
        public MachineLearningCategory MLCategory
        {
            get
            {
                return _mlCategory;
            }
            set
            {
                _mlCategory = value;
            }
        }

        /// <summary>
        /// Accept a visitor of IXMLSchemaNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXMLSchemaNodeVisitor visitor)
		{
			visitor.VisitXMLSchemaElement(this);
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            // set value of  the _elementtype member
            string text;
            text = parent.GetAttribute("etype");
            if (!string.IsNullOrEmpty(text))
            {
                _elementType = text;
            }
            else
            {
                _elementType = "xs:string";
            }

            text = parent.GetAttribute("minOccurs");
            if (!string.IsNullOrEmpty(text))
            {
                _minOccurs = text;
            }
            else
            {
                _minOccurs = "1";
            }

            text = parent.GetAttribute("maxOccurs");
            if (!string.IsNullOrEmpty(text))
            {
                _maxOccurs = text;
            }
            else
            {
                _maxOccurs = "1";
            }

            string str = parent.GetAttribute("ImportScripts");
            this._importScriptNames = new StringCollection();
            if (!string.IsNullOrEmpty(str))
            {
                string[] scriptNames = str.Split(';');
                for (int i = 0; i < scriptNames.Length; i++)
                {
                    this._importScriptNames.Add(scriptNames[i]);
                }
            }

            str = parent.GetAttribute("DataSource");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    _dataSourceOption = (XMLElementDataSourceOption)Enum.Parse(typeof(XMLElementDataSourceOption), str);
                }
                catch (Exception)
                {
                    _dataSourceOption = XMLElementDataSourceOption.Database;
                }
            }
            else
            {
                _dataSourceOption = XMLElementDataSourceOption.Database; // default
            }

            str = parent.GetAttribute("Merge");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _isMergingInstances = true;
            }
            else
            {
                _isMergingInstances = false;
            }

            _instanceIdentifyAttribute = parent.GetAttribute("IdentifyAttr");

            _validateCondition = parent.GetAttribute("Validate");

            _filterCondition = parent.GetAttribute("Condition");

            str = parent.GetAttribute("SortEnabled");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _isSortEnabled = true;
            }
            else
            {
                _isSortEnabled = false;
            }

            str = parent.GetAttribute("SortOrder");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    _sortOrder = int.Parse(str);
                }
                catch (Exception)
                {
                    _sortOrder = 0;
                }
            }
            else
            {
                _sortOrder = 0;
            }

            str = parent.GetAttribute("SortAscending");
            if (!string.IsNullOrEmpty(str) && str == "false")
            {
                _isSortAscending = false;
            }
            else
            {
                _isSortAscending = true;
            }

            str = parent.GetAttribute("TimeAxis");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _isXAxis = true;
            }
            else
            {
                _isXAxis = false;
            }

            str = parent.GetAttribute("CategoryAxis");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _isCategoryAxis = true;
            }
            else
            {
                _isCategoryAxis = false;
            }

            str = parent.GetAttribute("MLCategory");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    _mlCategory = (MachineLearningCategory)Enum.Parse(typeof(MachineLearningCategory), str);
                }
                catch (Exception)
                {
                    _mlCategory = MachineLearningCategory.None;
                }
            }
            else
            {
                _mlCategory = MachineLearningCategory.None; // default
            }
        }

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            // write the element type member
            if (!string.IsNullOrEmpty(_elementType))
            {
                parent.SetAttribute("etype", _elementType);
            }

            // write the _minOccurs member
            if (!string.IsNullOrEmpty(_minOccurs))
            {
                parent.SetAttribute("minOccurs", _minOccurs);
            }

            // write the _maxOccurs member
            if (!string.IsNullOrEmpty(_maxOccurs))
            {
                parent.SetAttribute("maxOccurs", _maxOccurs);
            }

            if (this._importScriptNames != null && this._importScriptNames.Count > 0)
            {
                StringBuilder buffer = new StringBuilder();
                int index = 0;
                foreach (string str in _importScriptNames)
                {
                    if (index == 0)
                    {
                        buffer.Append(str);
                    }
                    else
                    {
                        buffer.Append(";").Append(str);
                    }

                    index++;
                }

                parent.SetAttribute("ImportScripts", buffer.ToString());
            }

            if (_dataSourceOption != XMLElementDataSourceOption.Database)
            {
                parent.SetAttribute("DataSource", Enum.GetName(typeof(XMLElementDataSourceOption), _dataSourceOption));
            }

            if (_isMergingInstances)
            {
                parent.SetAttribute("Merge", "true");
            }

            if (!string.IsNullOrEmpty(_instanceIdentifyAttribute))
            {
                parent.SetAttribute("IdentifyAttr", _instanceIdentifyAttribute);
            }

            if (!string.IsNullOrEmpty(_validateCondition))
            {
                parent.SetAttribute("Validate", _validateCondition);
            }

            if (!string.IsNullOrEmpty(_filterCondition))
            {
                parent.SetAttribute("Condition", _filterCondition);
            }

            if (_isSortEnabled)
            {
                parent.SetAttribute("SortEnabled", "true");
            }

            if (_sortOrder > 0)
            {
                parent.SetAttribute("SortOrder", _sortOrder.ToString());
            }

            if (!_isSortAscending)
            {
                parent.SetAttribute("SortAscending", "false");
            }

            if (_isXAxis)
            {
                parent.SetAttribute("TimeAxis", "true");
            }

            if (_isCategoryAxis)
            {
                parent.SetAttribute("CategoryAxis", "true");
            }

            if (_mlCategory != MachineLearningCategory.None)
            {
                parent.SetAttribute("MLCategory", Enum.GetName(typeof(MachineLearningCategory), _mlCategory));
            }
        }

        /// <summary>
        /// Create a Xml Schema types that have been referenced by the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaType(XMLSchemaModel xmlSchemaModel)
        {
            XmlSchemaSimpleType simpleType = null;

            if (!IsComplexType(xmlSchemaModel))
            {
                simpleType = new XmlSchemaSimpleType();
                XmlSchemaSimpleTypeRestriction typeRes = new XmlSchemaSimpleTypeRestriction();
                typeRes.BaseTypeName = new XmlQualifiedName(ElementType, @"http://www.w3.org/2001/XMLSchema");
                simpleType.Content = typeRes;
            }

            return simpleType;
        }

        /// <summary>
        /// Create a Xml Schema Element that represents the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaElement(XMLSchemaModel xmlSchemaModel)
        {
            XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
            //xmlSchemaElement.MinOccursString = this.MinOccurs;
            //xmlSchemaElement.MaxOccursString = this.MaxOccurs;
            xmlSchemaElement.Name = EsacapeXMLElementName(Caption);

            if (IsComplexType(xmlSchemaModel))
            {
                // it's type is one of the complex type names
                XMLSchemaComplexType complexType = (XMLSchemaComplexType) xmlSchemaModel.ComplexTypes[Caption];
                xmlSchemaElement.SchemaType = (XmlSchemaComplexType)complexType.CreateXmlSchemaType(xmlSchemaModel);
            }
            else
            {
                // it is a simple type, first try to see if it has been created
                XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) CreateXmlSchemaType(xmlSchemaModel);

                xmlSchemaElement.SchemaType = simpleType;
            }

            return xmlSchemaElement;
        }

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
        public override string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/" + this.Name;
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
		[BrowsableAttribute(false)]
        public override IXaclObject Parent
		{
			get
			{
				return _parentNode;
			}
		}

		/// <summary>
		/// Return a  of children of the Taxonomy node
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
		{
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

		#endregion
	}

    public enum XMLElementDataSourceOption
    {
        Database,
        File,
        DatabaseAndFile
    }
}
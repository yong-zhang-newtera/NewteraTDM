/*
* @(#)TaxonNode.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary> 
	/// The class represent a node in a toxonomy tree
	/// </summary>
	/// <version> 1.0.0 23 Feb 2004</version>
	/// <author> Yong Zhang </author>
	/// <remarks>
	/// A Taxon node is allowed to add additional search
	/// filter to the DataViewModel inherited from its root node and it can
	/// also change the class name of the inherited DataViewModel except
	/// that the new class must be a subclass of the class of the
	/// TaxonomyModel; If the search filter contains search expresions that
	/// refer to referenced classes that do not exist in the inherited
	/// DataViewModel, they have to be added to the inherited DataViewModel.
	/// </remarks>
	public class TaxonNode : DataViewElementBase, ITaxonomy
	{
		private ITaxonomy _parentNode;
		private TaxonNodeCollection _childrenNodes;
		private string _className;
		private string _dataViewName;
		private ReferencedClassCollection _referencedClasses;
		private Filter _filter;
		private bool _isEnabled;
		private string _baseUrl;
        private AutoClassifyDef _autoClassifyDef; // defintion of auto-generated hierarchy

		// The name of a large image of the taxon node
		private string _largeImage = null;

		// The name of a median image of the taxon node
		private string _medianImage = null;

		// The name of a small image of the taxon node
		private string _smallImage = null;

		// The text of the taxon node
		private string _text = null;

		/// <summary>
		/// Initiate an instance of TaxonNode class
		/// </summary>
		public TaxonNode(string name) : base(name)
		{
			_parentNode = null;
			_childrenNodes = new TaxonNodeCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _childrenNodes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_className = null;
			_dataViewName = null;
			_referencedClasses = new ReferencedClassCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _referencedClasses.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_filter = new Filter();
			_isEnabled = true;
			_baseUrl = null;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _filter.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_xpath = null;
            _autoClassifyDef = new AutoClassifyDef();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _autoClassifyDef.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			if (DataView != null)
			{
				_childrenNodes.DataView = DataView;
				_referencedClasses.DataView = DataView;
				_filter.DataView = DataView;
                _autoClassifyDef.DataView = DataView;
			}
		}

		/// <summary>
		/// Initiating an instance of TaxonNode class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal TaxonNode(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

			// register the value change handlers
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _referencedClasses.ValueChanged += new EventHandler(ValueChangedHandler);
                _childrenNodes.ValueChanged += new EventHandler(ValueChangedHandler);
                _filter.ValueChanged += new EventHandler(ValueChangedHandler);
                _autoClassifyDef.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			if (DataView != null)
			{
				_referencedClasses.DataView = DataView;
				_childrenNodes.DataView = DataView;
				_filter.DataView = DataView;
                _autoClassifyDef.DataView = DataView;
			}
		}

		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		/// <value>DataViewModel object</value>
		[BrowsableAttribute(false)]		
		public override DataViewModel DataView
		{
			get
			{
				return base.DataView;
			}
			set
			{
				base.DataView = value;
				if (value != null)
				{
					if (_referencedClasses != null)
					{
						_referencedClasses.DataView = value;
					}

					if (_childrenNodes != null)
					{
						_childrenNodes.DataView = value;
					}

					if (_filter != null)
					{
						_filter.DataView = value;
					}

                    _autoClassifyDef.DataView = value;
				}
			}
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		[BrowsableAttribute(false)]		
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.TaxonNode;
			}
		}

		/// <summary>
		/// Gets the name of owner taxonomy model
		/// </summary>
		/// <value>Taxonomy name</value>
		[BrowsableAttribute(false)]		
		public string TaxonomyName 
		{
			get
			{
				return OwnerTaxonomy.Name;
			}
		}

		/// <summary>
		/// Gets the owner taxonomy model
		/// </summary>
		/// <value>A owner TaxonomyModel</value>
		[BrowsableAttribute(false)]		
		public TaxonomyModel OwnerTaxonomy 
		{
			get
			{
				TaxonomyModel taxonomy = null;
				ITaxonomy parent = ParentNode;
				while (taxonomy == null)
				{
					if (parent == null)
					{
						break;
					}

					if (parent is TaxonomyModel)
					{
						taxonomy = (TaxonomyModel) parent;
					}
					else
					{
						parent = parent.ParentNode;
					}
				}

				return taxonomy;
			}
		}

		/// <summary>
		/// Gets or sets information to indicate whether the node is enabled.
		/// </summary>
		/// <value>
		/// true if it is enabled; otherwise false. default is true.
		/// </value>
		/// <remarks>An enabled node can have an action associated with it</remarks>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Is the node enabled?"),
		DefaultValueAttribute(true)
		]		
		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets the base url associated with the node.
		/// </summary>
		/// <value>
		/// a string representing a base url
		/// </value>
		/// <remarks>This value is used by web applications</remarks>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("A base url associated with the node")
		]		
		public string BaseUrl
		{
			get
			{
				return _baseUrl;
			}
			set
			{
				_baseUrl = value;
                FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets a name of the large image for the taxon node.
		/// </summary>
		/// <value>An image name</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The name of a large image that can be displayed on UI."),
		EditorAttribute("Newtera.Studio.ImageNamePropertyEditor, Studio", typeof(UITypeEditor))
		]
		public string LargeImage
		{
			get
			{
				if (_largeImage != null && _largeImage.Length > 0)
				{
					return _largeImage;
				}
				else if (_className != null && _className.Length > 0)
				{
					ClassElement classElement = MetaDataModel.SchemaModel.FindClass(_className);
					
					return classElement.LargeImage;
				}
				else
				{
					return null;
				}
			}
			set
			{
				_largeImage = value;
                FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets a name of the median image for the taxon node.
		/// </summary>
		/// <value>An image name</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The name of a median image that can be displayed on UI."),
		EditorAttribute("Newtera.Studio.ImageNamePropertyEditor, Studio", typeof(UITypeEditor))
		]
		public string MedianImage
		{
			get
			{
				if (_medianImage != null && _medianImage.Length > 0)
				{
					return _medianImage;
				}
				else if (_className != null && _className.Length > 0)
				{
					ClassElement classElement = MetaDataModel.SchemaModel.FindClass(_className);
					
					return classElement.MedianImage;
				}
				else
				{
					return null;
				}
			}
			set
			{
				_medianImage = value;
                FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets a name of the small image for the taxon node.
		/// </summary>
		/// <value>An image name</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The name of a small image that can be displayed on UI."),
		EditorAttribute("Newtera.Studio.ImageNamePropertyEditor, Studio", typeof(UITypeEditor))
		]
		public string SmallImage
		{
			get
			{
				if (_smallImage != null && _smallImage.Length > 0)
				{
					return _smallImage;
				}
				else if (_className != null && _className.Length > 0)
				{
					ClassElement classElement = MetaDataModel.SchemaModel.FindClass(_className);
					
					return classElement.SmallImage;
				}
				else
				{
					return null;
				}
			}
			set
			{
				_smallImage = value;
                FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets a detailed text of the taxon node.
		/// </summary>
		/// <value>An image name</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The detailed text of the taxon node that is displayed on UI."),
        EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
		public string DetailedText
		{
			get
			{
				if (_text != null && _text.Length > 0)
				{
					return _text;
				}
				else if (_className != null && _className.Length > 0)
				{
					ClassElement classElement = MetaDataModel.SchemaModel.FindClass(_className);
					
					return classElement.DetailedText;
				}
				else
				{
					return null;
				}
			}
			set
			{
				_text = value;
                FireValueChangedEvent(value);
			}
		}

        /// <summary>
        /// Gets or sets the definition for auto-generated hierarchy.
        /// </summary>
        /// <value>A AutoClassifyDef object</value>
        [BrowsableAttribute(false)]
        public AutoClassifyDef AutoClassifyDef
        {
            get
            {
                return _autoClassifyDef;
            }
            set
            {
                _autoClassifyDef = value;

                FireValueChangedEvent(value);
            }
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			if (visitor.VisitTaxonNode(this))
			{
				_referencedClasses.Accept(visitor);

				_filter.Accept(visitor);

				foreach (TaxonNode taxon in _childrenNodes)
				{
					taxon.Accept(visitor);
				}

                _autoClassifyDef.Accept(visitor);
			}
		}

		/// <summary>
		/// Gets the search filter defined for this node
		/// </summary>
		/// <value>A Filter object</value>
		[
		CategoryAttribute("Filter"),
		DescriptionAttribute("A search expression as a filter of the node"),
		DefaultValueAttribute(null),
		EditorAttribute("Newtera.Studio.SearchFilterPropertyEditor, Studio", typeof(UITypeEditor)),
		TypeConverterAttribute("Newtera.Studio.SearchFilterPropertyConverter, Studio")
		]	
		public IDataViewElement SearchFilter
		{
			get
			{
				return _filter.Expression;
			}
			set
			{
				_filter.Expression = value;
                FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets a collection of referenced classes used by the search filter
		/// </summary>
		/// <value>A ReferencedClassCollection</value>
		[BrowsableAttribute(false)]	
		public ReferencedClassCollection ReferencedClasses
		{
			get
			{
				return _referencedClasses;
			}
		}

		#region ITaxonomy

		/// <summary>
		/// Gets the meta data model that owns the ITaxonomy object
		/// </summary>
		/// <value>A MetaDataModel</value>
		[BrowsableAttribute(false)]				
		public MetaDataModel MetaDataModel 
		{
			get
			{
				return _parentNode.MetaDataModel;
			}
		}

		/// <summary>
		/// Gets or sets the class name for this node
		/// </summary>
		/// <value>A class name, null if this node does not define a new class.</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Specify a class of the node with restriction that it must be a subclass of the class inherited from the closest parent node, if exists."),
		DefaultValueAttribute(null),
		TypeConverterAttribute("Newtera.Common.MetaData.DataView.Taxonomy.ClassNameConverter"),		
		EditorAttribute("Newtera.Studio.ClassNamePropertyEditor, Studio", typeof(UITypeEditor)),
		]		
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the data view name for the taxon node.
		/// </summary>
		/// <value>The data view name, can be null.</value>		
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The data view of the taxon node. The default data view of the specified class is used if this value is not specified."),
		DefaultValueAttribute(null),
		TypeConverterAttribute("Newtera.Common.MetaData.DataView.Taxonomy.DataViewNameConverter"),
		EditorAttribute("Newtera.Studio.DataViewNamePropertyEditor, Studio", typeof(UITypeEditor)),
		]			
		public string DataViewName
		{
			get
			{
				return _dataViewName;
			}
			set
			{
				_dataViewName = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the parent node of this node
		/// </summary>
		/// <value>A IDataViewElement object.</value>
		[BrowsableAttribute(false)]		
		public ITaxonomy ParentNode
		{
			get
			{
				return _parentNode;
			}
			set
			{
				_parentNode = value;
			}
		}

		/// <summary>
		/// Gets the children nodes of this node
		/// </summary>
		/// <value>A TaxonNodeCollection</value>
		[BrowsableAttribute(false)]		
		public TaxonNodeCollection ChildrenNodes
		{
			get
			{
				return _childrenNodes;
			}
		}

		/// <summary>
		/// Gets the DataViewModel for the ITaxonomy object
		/// </summary>
		/// <param name="sectionString">Specify the sections whose attributes are included
		/// in the result list of the generated data view, or null to include all attributes.</param>		
		public DataViewModel GetDataView(string sectionString)
		{
			// the data view of this taxon node is generated dynamicaly
			// according to the following rules.
			// if a data view is generated by its parent node, this node
			// will not generate a new data view but simply add the local search
			// filter to the data view from the parent. If there is no data view
			// generated by its parent node, it will create a data view with the
			// local search filter. Once a data view is generated, it is cached
			// to improve the performance
			DataViewModel dataView = GenerateDataView(sectionString);

			this.DataView = dataView;

            return dataView;
		}

		#endregion

		/// <summary>
		/// Get information indicating whether a class is referenced by the node or its
		/// children nodes.
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>true if the class is referenced by the node or its subtree, false otherwise.</returns>
		public bool IsClassReferenced(string className)
		{
			bool status = false;

			if (_className != null && _className == className)
			{
				status = true;
			}
			else
			{
				foreach (TaxonNode taxon in _childrenNodes)
				{
					if (taxon.IsClassReferenced(className))
					{
						status = true;
						break;
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Get information indicating whether a data view is referenced by the node or its
		/// children nodes.
		/// </summary>
		/// <param name="dataViewName">The data view name</param>
		/// <returns>true if the data view is referenced by the node or its subtree, false otherwise.</returns>
		public bool IsDataViewReferenced(string dataViewName)
		{
			bool status = false;

			if (_dataViewName != null && _dataViewName == dataViewName)
			{
				status = true;
			}
			else
			{
				foreach (TaxonNode taxon in _childrenNodes)
				{
					if (taxon.IsDataViewReferenced(dataViewName))
					{
						status = true;
						break;
					}
				}
			}

			return status;
		}

        /// <summary>
        /// Get information indicating whether an attribute is referenced by the node or its
        /// children nodes.
        /// </summary>
        /// <param name="className">The owner class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>true if the attribute is referenced by the node or its subtree, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName)
        {
            bool status = false;

            // check in the filter
            if (_filter != null)
            {
                FindSearchAttributeVisitor visitor = new FindSearchAttributeVisitor(className, attributeName);
                _filter.Accept(visitor);
                if (visitor.IsFound)
                {
                    status = true;
                }
            }

            if (!status)
            {
                if (_autoClassifyDef != null && _autoClassifyDef.HasDefinition)
                {
                    if (_autoClassifyDef.IsAttributeReferenced(this, className, attributeName))
                    {
                        status = true;
                    }
                }
            }
            
            if (!status)
            {
                foreach (TaxonNode taxon in _childrenNodes)
                {
                    if (taxon.IsAttributeReferenced(className, attributeName))
                    {
                        status = true;
                        break;
                    }
                }
            }

            if (!status)
            {
                // checking the referenced class for relationships
                foreach (DataClass refClass in _referencedClasses)
                {
                    if (refClass.ReferringRelationshipName == attributeName)
                    {
                        DataClass referringClass = this.FindClass(refClass.ReferringClassAlias);
                        if (referringClass != null)
                        {
                            ClassElement schemaModelElement = referringClass.GetSchemaModelElement() as ClassElement;
                            if (schemaModelElement != null &&
                                schemaModelElement.Name == className)
                            {
                                status = true;
                                break;
                            }
                        }
                    }
                }
            }

            return status;
        }

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of  the _className member
			string text = parent.GetAttribute("ClassName");
			if (text != null && text.Length > 0)
			{
				_className = text;
			}

			// set value of  the _dataViewName member
			text = parent.GetAttribute("DataView");
			if (text != null && text.Length > 0)
			{
				_dataViewName = text;
			}

			// Set _isEnabled member
			string status = parent.GetAttribute(NewteraNameSpace.ENABLED);
			this._isEnabled = (status != null && status == "true" ? true : false);

			// set value of  the _baseUrl member
			text = parent.GetAttribute("BaseUrl");
			if (text != null && text.Length > 0)
			{
				_baseUrl = text;
			}

			// Set _largeImage member
			text = parent.GetAttribute(NewteraNameSpace.LARGE_IMAGE);
			if (text != null && text.Length > 0)
			{
				_largeImage = text;
			}

			// Set _medianImage member
			text = parent.GetAttribute(NewteraNameSpace.MEDIAN_IMAGE);
			if (text != null && text.Length > 0)
			{
				_medianImage = text;
			}

			// Set _smallImage member
			text = parent.GetAttribute(NewteraNameSpace.SMALL_IMAGE);
			if (text != null && text.Length > 0)
			{
				_smallImage = text;
			}

			// Set _text member
			text = parent.GetAttribute(NewteraNameSpace.TEXT);
			if (text != null && text.Length > 0)
			{
				_text = text;
			}

			// restore a collection of  referenced classes
			_referencedClasses = (ReferencedClassCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);

			// restore the search filter
			_filter = (Filter) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);

			// restore the children Taxon nodes
			_childrenNodes = (TaxonNodeCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[2]);

			// set the parent node of children nodes
			foreach (TaxonNode child in _childrenNodes)
			{
				child.ParentNode = this;
			}

            // the old schema model may not have this element
            if (parent.ChildNodes.Count > 3)
            {
                _autoClassifyDef = (AutoClassifyDef)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[3]);
            }
            else
            {
                // this is an old schema, create a new one
                _autoClassifyDef = new AutoClassifyDef();
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the baseClassName member
			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("ClassName", _className);
			}

			// write the _dataViewName member
			if (_dataViewName != null && _dataViewName.Length > 0)
			{
				parent.SetAttribute("DataView", _dataViewName);
			}

			// write the _referencedClasses
			XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_referencedClasses.ElementType));
			_referencedClasses.Marshal(child);
			parent.AppendChild(child);

			// write the _filter
			child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_filter.ElementType));
			_filter.Marshal(child);
			parent.AppendChild(child);

			// write the _childrenNodes
			child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_childrenNodes.ElementType));
			_childrenNodes.Marshal(child);
			parent.AppendChild(child);

			// Write _isEnabled member, default is true
			if (this._isEnabled)
			{
				// write the _className member
				parent.SetAttribute(NewteraNameSpace.ENABLED, "true");                	
			}

			// write the _baseUrl member
			if (_baseUrl != null && _baseUrl.Length > 0)
			{
				parent.SetAttribute("BaseUrl", _baseUrl);
			}

			// Write _largeImage member
			if (_largeImage != null && _largeImage.Length > 0)
			{
				parent.SetAttribute(NewteraNameSpace.LARGE_IMAGE, _largeImage);	
			}

			// Write _medianImage member
			if (_medianImage != null && _medianImage.Length > 0)
			{
				parent.SetAttribute(NewteraNameSpace.MEDIAN_IMAGE, _medianImage);	
			}

			// Write _smallImage member
			if (_smallImage != null && _smallImage.Length > 0)
			{
				parent.SetAttribute(NewteraNameSpace.SMALL_IMAGE, _smallImage);	
			}

			// Write _text member
			if (_text != null && _text.Length > 0)
			{
				parent.SetAttribute(NewteraNameSpace.TEXT, _text);	
			}

            // write the _autoClassifyDef
            child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_autoClassifyDef.ElementType));
            _autoClassifyDef.Marshal(child);
            parent.AppendChild(child);
		}

		/// <summary>
		/// Generate a local copy of DataViewModel based on its parent node's
		/// DataViewModel and local search filter
		/// </summary>
		/// <returns></returns>
		private DataViewModel GenerateDataView(string sectionString)
		{
            bool isLocalDataView = false; // true if the data view is created locally

			DataViewModel dataView = _parentNode.GetDataView(sectionString);
			if (dataView != null)
			{
				dataView.SchemaInfo = _parentNode.GetDataView(sectionString).SchemaInfo;
				dataView.SchemaModel = _parentNode.GetDataView(sectionString).SchemaModel;
			}
			else if (_dataViewName != null && _dataViewName.Length > 0)
			{
				// clone the data view so that it can be modified without affecting
				// the global one
				dataView = ((DataViewModel) MetaDataModel.DataViews[_dataViewName]).Clone();
				if (dataView == null)
				{
					throw new TaxonomyException("The DataView " + _dataViewName + " does not exist. It may have been deleted");
				}
				else
				{
					dataView.SchemaModel = MetaDataModel.SchemaModel;
					dataView.SchemaInfo = MetaDataModel.SchemaInfo;

                    // add related classes to the dataview
                    dataView.BaseClass.RelatedClasses = MetaDataModel.GetRelatedClasses(dataView.BaseClass);
				}
			}
			else if (_className != null && _className.Length > 0)
			{
				if (sectionString != null && sectionString.Length > 0)
				{
					if (sectionString.ToUpper() == "DETAILED")
					{
						dataView = MetaDataModel.GetDetailedDataView(_className);
					}
					else
					{
						dataView = MetaDataModel.GetDefaultDataView(_className, sectionString, false);
					}
				}
				else
				{
					dataView = MetaDataModel.GetDefaultDataView(_className);
				}

                isLocalDataView = true;
			}

			if (dataView != null)
			{
				// change the base class of DataViewModel to the specified base class
				// defined for this node
				if (_className != null && _className.Length > 0 &&
					dataView.BaseClass.ClassName != _className)
				{
					ClassElement classElement = MetaDataModel.SchemaModel.FindClass(_className);
					dataView.BaseClass.ClassName = _className;
					dataView.BaseClass.Caption = classElement.Caption;
				}

				// add the referenced class that are used by the local search filter
				foreach (DataClass refClass in ReferencedClasses)
				{
                    // TODO, compare class alias since alias is unique among the referenced classes
                    if (dataView.ReferencedClasses[refClass.Name] == null)
                    {
                        dataView.ReferencedClasses.Add(refClass);
                    }
				}

				// add the local search expression to the data view
				if (_filter.Expression != null)
				{
                    if (isLocalDataView)
                    {
                        // clear the search expression so that the data view contains only the
                        // expression defined for the filter
                        dataView.ClearSearchExpression();
                    }

					dataView.AddSearchExpr(_filter.Expression, ElementType.And);
				}
			}

			return dataView;
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
			return _childrenNodes.GetEnumerator();
		}

		#endregion

        /// <summary>
        /// Find the class element in the data view given a class alias
        /// </summary>
        /// <param name="alias">The class alias</param>
        /// <returns>A DataClass element</returns>
        public DataClass FindClass(string alias)
        {
            DataClass found = null;

            if (!string.IsNullOrEmpty(alias))
            {
                foreach (DataClass dataClass in _referencedClasses)
                {
                    if (dataClass.Alias == alias)
                    {
                        found = dataClass;
                        break;
                    }
                }
            }

            return found;
        }
	}
}
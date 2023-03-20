/*
* @(#)TaxonomyModel.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Xml;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.DataView.Validate;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A TaxonomyModel contains a taxonomy tree that
	/// provided a logical classification scheme.
	/// 
	/// A TaxonomyModel is used in two areas: to generate queries that fetches
	/// data instances that satisfy the definition of taxonomy nodes, and
	/// to provide information for user interface creation.
	/// 
	/// A TaxonomyModel can be constructed programatically or from an XML data. It can be
	/// saved as an XML data too.
	/// </summary>
	/// 
	/// <version>1.0.1 12 Feb 2004</version>
	/// 
	/// <author>Yong Zhang</author>
	/// <remarks>
	/// A non-leaf node in a taxonomy tree can be just a place holder to provide
	/// a navigation path to its subnodes. This kind of node is called dummy node.
	/// A dummy node does not have either class name, data view name, or search
	/// filter specified. Leaf nodes can not be dummy nodes.
	/// For a non-root node with class name or data view name defined, if its parent
	/// node is associated a class, the class name itself or the base class of
	/// the data view must be one of subclasses of the class associated with parent
	/// node. A node in taxonomy tree can add a search filter to the data view
	/// inherited from parent node or created locally to define the set of instances
	/// associated with the node.
	/// 
	/// If a node has a class name, but not data view name, the
	/// default data view of the class is used. Othewise, the specified
	/// data view is used.
	/// </remarks>
	public class TaxonomyModel : DataViewElementBase, ITaxonomy
	{
		private string _className;
		private string _dataViewName;
		private TaxonNodeCollection _childrenNodes;
        private AutoClassifyDef _autoClassifyDef; // defintion of auto-generated hierarchy
		private ITaxonomy _parent; // run-time use only
		private MetaDataModel _metaData; // run-time use only

		/// <summary>
		/// Initiating an instance of TaxonomyModel class
		/// </summary>
		/// <param name="name">Name of the taxonomy</param>
		public TaxonomyModel(string name) : base(name)
		{
			_className = null;
			_dataViewName = null;
			_childrenNodes = new TaxonNodeCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
			    _childrenNodes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _autoClassifyDef = new AutoClassifyDef();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _autoClassifyDef.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_xpath = null;

			if (DataView != null)
			{
				_childrenNodes.DataView = DataView;
                _autoClassifyDef.DataView = DataView;
			}
		}

		/// <summary>
		/// Initiating an instance of TaxonomyModel class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal TaxonomyModel(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

            if (GlobalSettings.Instance.IsWindowClient)
            {
                _childrenNodes.ValueChanged += new EventHandler(ValueChangedHandler);
                _autoClassifyDef.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_xpath = null;

			if (DataView != null)
			{
				_childrenNodes.DataView = DataView;
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
				if (_childrenNodes != null && value != null)
				{
					_childrenNodes.DataView = value;
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
				return ElementType.Taxonomy;
			}
		}

		/// <summary>
		/// Gets or sets the meta data
		/// </summary>
		/// <returns> A MetaDataModel object</returns>
		[BrowsableAttribute(false)]
		public MetaDataModel MetaData
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
				return _parent.MetaDataModel;
			}
		}

		/// <summary>
		/// Gets or sets the base class name for the taxonomy.
		/// </summary>
		/// <value>The base class name</value>
		[
		CategoryAttribute("System"),
		DefaultValueAttribute(null),
		DescriptionAttribute("Specify the class of the taxonomy "),
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
		/// Gets the data view name for the taxonomy.
		/// </summary>
		/// <value>The data view name, can be null.</value>		
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The data view of the taxonomy. The default data view of the specified class is used if this value is not specified."),
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
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// Gets the first level of children Taxon nodes.
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
		/// Gets the DataViewModel for the ITaxonomy object
		/// </summary>
		/// <param name="sectionString">Specify the sections whose attributes are included
		/// in the result list of the generated data view, or null to include all attributes.</param>
		public DataViewModel GetDataView(string sectionString)
		{
			DataViewModel dataView = GenerateDataView(sectionString);

			this.DataView = dataView;

            return dataView;
		}

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = _parent.ToXPath() + "/" + this.Name;
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
				return _parent;
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
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			if (visitor.VisitTaxonomyModel(this))
			{
				foreach (TaxonNode taxon in _childrenNodes)
				{
					taxon.Accept(visitor);
				}

                _autoClassifyDef.Accept(visitor);
			}
		}
		
		/// <summary>
		/// Find a TaxonNode of a specified name
		/// </summary>
		/// <param name="name">The name</param>
		/// <returns>The TaxonNode found, null if it does not exist.</returns>
		public TaxonNode FindNode(string name)
		{
			return FindNodeFromChildren(name, ChildrenNodes);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of  the _className member
			_className = parent.GetAttribute("BaseClassName");

			// set value of  the _dataViewName member
			_dataViewName = parent.GetAttribute("DataView");

			// restore the children nodes
			_childrenNodes = (TaxonNodeCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
		
			// set the parent node of children nodes
			foreach (TaxonNode child in _childrenNodes)
			{
				child.ParentNode = this;
			}

            // the old schema model may not have this element
            if (parent.ChildNodes.Count > 1)
            {
                _autoClassifyDef = (AutoClassifyDef)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[1]);
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

			// write the _className member
			if (_className != null && _className.Length > 0)
			{
				parent.SetAttribute("BaseClassName", _className);
			}

			// write the _dataViewName member
			if (_dataViewName != null && _dataViewName.Length > 0)
			{
				parent.SetAttribute("DataView", _dataViewName);
			}

			// write the _childrenNodes
			XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_childrenNodes.ElementType));
			_childrenNodes.Marshal(child);
			parent.AppendChild(child);

            // write the _autoClassifyDef
            child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_autoClassifyDef.ElementType));
            _autoClassifyDef.Marshal(child);
            parent.AppendChild(child);
		}

		/// <summary>
		/// Validate the taxonomy to see whether it is in a valid state.
		/// </summary>
		/// <returns>A DataViewValidateResult</returns>
		public DataViewValidateResult Validate()
		{
			// we share the DataViewValidateVistor since taxonomy model contains
			// many of DataView elements
			DataViewValidateVisitor visitor = new DataViewValidateVisitor();

			this.Accept(visitor);

			return visitor.ValidateResult;
		}

		/// <summary>
		/// Get information indicating whether a class is referenced by any of nodes
		/// in the taxonomy.
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>true if the class is referenced by the taxonomy, false otherwise.</returns>
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
		/// Get information indicating whether a data view is referenced by any of nodes
		/// in the taxonomy.
		/// </summary>
		/// <param name="dataViewName">The data view name</param>
		/// <returns>true if the data view is referenced by any of nodes in taxonomy, false otherwise.</returns>
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
        /// Get information indicating whether an attribute is referenced by any of nodes
        /// in the taxonomy.
        /// </summary>
        /// <param name="className">The owner class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>true if the attribute is referenced by the taxonomy, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName)
        {
            bool status = false;

            if (_autoClassifyDef != null && _autoClassifyDef.HasDefinition)
            {
                if (_autoClassifyDef.IsAttributeReferenced(this, className, attributeName))
                {
                    status = true;
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

            return status;
        }

		/// <summary>
		/// Get a DataViewModel for the specified data view name. If not
		/// specified, generate a Default DataViewModel of the class.
		/// </summary>
		/// <returns>A DataViewModel</returns>
		private DataViewModel GenerateDataView(string sectionString)
		{
			DataViewModel dataView = null;
			if (_dataViewName != null && _dataViewName.Length > 0)
			{
				// clone the data view so that it can be freely modified
				dataView = ((DataViewModel) MetaDataModel.DataViews[_dataViewName]).Clone();
				if (dataView == null)
				{
					throw new TaxonomyException("The DataView " + _dataViewName + " does not exist. It may have been deleted");
				}
				else
				{
					dataView.SchemaModel = MetaDataModel.SchemaModel;
					dataView.SchemaInfo = MetaDataModel.SchemaInfo;
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
			}

			return dataView;
		}

		/// <summary>
		/// A resursive function that finds a TaxonNode of the specified name from
		/// a collection of TaxonNode objects
		/// </summary>
		/// <param name="name">The name</param>
		/// <param name="childNodes">A collection of TaxonNode objects</param>
		/// <returns></returns>
		private TaxonNode FindNodeFromChildren(string name, TaxonNodeCollection childNodes)
		{
			TaxonNode found = null;

			foreach (TaxonNode child in childNodes)
			{
				if (child.Name == name)
				{
					found = child;
					break;
				}
				else
				{
					found = FindNodeFromChildren(name, child.ChildrenNodes);

					if (found != null)
					{
						break;
					}
				}
			}

			return found;
		}
	}
}
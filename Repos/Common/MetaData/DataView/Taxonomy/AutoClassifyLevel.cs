/*
* @(#)AutoClassifyLevel.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
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

	/// <summary> 
	/// The class is used to represent information that is necessary for generating a level
    /// of nodes in an auto-generated hierarchy
	/// </summary>
	/// <version> 1.0.0 13 June 2008</version>
	/// <remarks>
    /// Each level needs to define the following:
    /// 1. A classifying attribute whose distinct values determine number of nodes of this level
    /// 2. base url is the url associated with each generated node
    ///
	/// </remarks>
	public class AutoClassifyLevel : DataViewElementBase
	{
		private string _classifyingAttribute;
        private string _ownerClassAlias;
		private string _baseUrl;
        private string _nodeValue; // run-time variable

		/// <summary>
		/// Initiate an instance of AutoClassifyLevel class
		/// </summary>
		public AutoClassifyLevel() : base("")
		{
			_classifyingAttribute = null;
            _ownerClassAlias = null;
			_baseUrl = null;
            _nodeValue = null;
		}

		/// <summary>
		/// Initiating an instance of AutoClassifyLevel class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AutoClassifyLevel(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
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
				return ElementType.AutoClassifyLevel;
			}
		}

        /// <summary>
        /// Override the property so that it won't show up on the PropertyEditor
        /// </summary>
        [BrowsableAttribute(false)]	
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Override the property so that it won't show up on the PropertyEditor
        /// </summary>
        [BrowsableAttribute(false)]	
        public virtual string Caption
        {
            get
            {
                return base.Caption;
            }
            set
            {
                base.Caption = value;
            }
        }

        /// <summary>
        /// Override the property so that it won't show up on the PropertyEditor
        /// </summary>
        [BrowsableAttribute(false)]	
        public virtual string Description
        {
            get
            {
                return base.Description;
            }
            set
            {
                base.Description = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of attribute whose distinct values determine dimension of the level.
        /// </summary>
        /// <value>
        /// a attribute name
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("the name of attribute whose distinct values determine dimension of the level."),
            DefaultValueAttribute(null),
            TypeConverterAttribute("Newtera.Studio.ClassifyingAttributeConverter, Studio"),
            EditorAttribute("Newtera.Studio.ClassifyingAttributePropertyEditor, Studio", typeof(UITypeEditor))
        ]
        public string ClassifyingAttribute
        {
            get
            {
                return _classifyingAttribute;
            }
            set
            {
                _classifyingAttribute = value;
                FireValueChangedEvent(value);
            }
        }

        /// <summary>
        /// The alias of the class that owns the classifiying attribute
        /// </summary>
        [BrowsableAttribute(false)]
        public string OwnerClassAlias
        {
            get
            {
                return _ownerClassAlias;
            }
            set
            {
                _ownerClassAlias = value;
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
        /// Gets or sets the value of a node currently representing this level.
        /// This property is mainly used by node generating algorithm
        /// </summary>
        [BrowsableAttribute(false)]
        public string NodeValue
        {
            get
            {
                return _nodeValue;
            }
            set
            {
                _nodeValue = value;
            }
        }

        /// <summary>
        /// Get information indicating whether an attribute is referenced by definition of
        /// this level.
        /// </summary>
        /// <param name="ownerNode">The node that owns the definition</param>
        /// <param name="className">The owner class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>true if the attribute is referenced definition of
        /// this level, false otherwise.</returns>
        public bool IsAttributeReferenced(ITaxonomy ownerNode, string className, string attributeName)
        {
            bool status = false;

            DataViewModel dataView = ownerNode.GetDataView(null);
            if (dataView != null &&
                !string.IsNullOrEmpty(_ownerClassAlias) &&
                !string.IsNullOrEmpty(_classifyingAttribute))
            {
                DataClass dataClass = dataView.FindClass(_ownerClassAlias);
                if (dataClass != null &&
                    dataClass.ClassName == className &&
                    _classifyingAttribute == attributeName)
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Accept a visitor of IDataViewElementVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IDataViewElementVisitor visitor)
        {
            visitor.VisitClassifyLevel(this);
        }

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of  the _classifyingAttribute member
			string text = parent.GetAttribute("classify");
			if (text != null && text.Length > 0)
			{
				_classifyingAttribute = text;
			}

            // set value of  the _ownerClassAlias member
            text = parent.GetAttribute("owner");
            if (text != null && text.Length > 0)
            {
                _ownerClassAlias = text;
            }

			// set value of  the _baseUrl member
			text = parent.GetAttribute("BaseUrl");
			if (text != null && text.Length > 0)
			{
				_baseUrl = text;
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
			if (!string.IsNullOrEmpty(_classifyingAttribute))
			{
				parent.SetAttribute("classify", _classifyingAttribute);
			}

            // write the _ownerClassAlias member
            if (!string.IsNullOrEmpty(_ownerClassAlias))
            {
                parent.SetAttribute("owner", _ownerClassAlias);
            }

            // write the _baseUrl member
            if (_baseUrl != null && _baseUrl.Length > 0)
            {
                parent.SetAttribute("BaseUrl", _baseUrl);
            }
		}
	}
}
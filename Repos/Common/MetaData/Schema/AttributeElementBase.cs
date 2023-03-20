/*
* @(#)AttributeElementBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml.Schema;
	using System.ComponentModel;
	using Newtera.Common.Core;

	/// <summary>
	/// Provides the base functionality for creating an Attribute Element
	/// 
	/// </summary>
	/// <version> 1.0.1 26 Jun 2003 </version>
	/// <author> Yong Zhang</author>
	abstract public class AttributeElementBase : SchemaModelElement
	{
		/* private instance variables */
		private ClassElement _ownerClass = null;
		
		// The column name correspond to this attribute element.
		private string _columnName = null;

        private string _uiControlCreator = null;

        private bool _invokeCallback = false;
		
		private bool _isBrowsable = true;

		/// <summary>
		/// Initializing AttributeElementBase object
		/// </summary>
		/// <param name="name">Name of attribute</param>
		public AttributeElementBase(string name) : base(name)
		{
			_ownerClass = null;
			_columnName = null;
		}

		/// <summary>
		/// Initializing AttributeElementBase object
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal AttributeElementBase(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
			_ownerClass = null;
			_columnName = null;
		}

		/// <summary>
		/// Gets or sets the information indicating whether the attribute is browsable.
		/// </summary>
		/// <value>true if it is browsable, false otherwise, default is true</value>
		/// <remarks>If browsable is false, it won't appear as result field of a data view,
		/// but it still can be used as a search field</remarks>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("Is the attribute browsable?"),
		DefaultValueAttribute(true)
		]	
		public virtual bool IsBrowsable
		{
			get
			{
				return _isBrowsable;
			}
			set
			{
				_isBrowsable = value;
			}
		}

		/// <summary>
		/// Gets or sets the owner class of the attribute.
		/// </summary>
		/// <value>The owner class of the attribute.</value>
		[BrowsableAttribute(false)]		
		public ClassElement OwnerClass
		{
			get
			{
				return _ownerClass;
			}
			set
			{
				_ownerClass = value;
			}
		}

		/// <summary>
		/// Gets or sets the DB column name of attribute.
		/// </summary>
		/// <value>The database column name for the attribute</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The corresponding table column name"),
            ReadOnlyAttribute(true),
        ]	
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
			set
			{
				_columnName = value;
			}
		}

        /// <summary>
        /// Gets or sets the creator that creates a customized web control for the attribute
        /// </summary>
        /// <value>
        /// A fully-qualified creator name, including namespace and class name, and dll name
        /// for example, MyLib.RelationshipDropdownList,MyLib
        /// </value>
        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("Gets or sets the creator that creates a customized web control for the attribute"),
        DefaultValueAttribute(null)
        ]
        public virtual string UIControlCreator
        {
            get
            {
                return _uiControlCreator;
            }
            set
            {
                _uiControlCreator = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether to invoke call back function code defined for the class
        /// </summary>
        /// <value> return true if to invoke callback, false otherwise. The default is false.</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Invoke the callback function when the attribute values is changed at user interface?"),
            DefaultValueAttribute(false),
            BrowsableAttribute(true)
        ]
        public virtual bool InvokeCallback
        {
            get
            {
                return _invokeCallback;
            }
            set
            {
                _invokeCallback = value;
            }
        }

		/// <summary>
		/// Gets or sets data type of the attribute.
		/// </summary>
		/// <value>DataType.Integer</value>
		public abstract DataType DataType {get; set;}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			// first give the base a chance to do its own unmarshalling
			base.Unmarshal();

			// Set ColumnName member
			_columnName = GetNewteraAttributeValue(NewteraNameSpace.COLUMN_NAME);

            // Set _uiControlCreator member
            _uiControlCreator = this.GetNewteraAttributeValue(NewteraNameSpace.UI_CREATOR);

            // Set _invokeCallback member
            string status = GetNewteraAttributeValue(NewteraNameSpace.INVOKE_CALLBACK);
            this._invokeCallback = (status != null && status == "true" ? true : false);

			// set IsBrowsable member
			status = GetNewteraAttributeValue(NewteraNameSpace.BROWSABLE);
			_isBrowsable = (status != null && status == "false" ? false : true);
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		internal override void Marshal()
		{
			// write ColumnName member to xml schema
			if (ColumnName != null && ColumnName.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.COLUMN_NAME, _columnName);	
			}

            // Write _uiControlCreator member
            if (!string.IsNullOrEmpty(_uiControlCreator))
            {
                SetNewteraAttributeValue(NewteraNameSpace.UI_CREATOR, _uiControlCreator);
            }

            // Set _invokeCallback member
            if (_invokeCallback)
            {
                SetNewteraAttributeValue(NewteraNameSpace.INVOKE_CALLBACK, "true");
            }

			// Write IsBrowsable member
			if (!_isBrowsable)
			{
				SetNewteraAttributeValue(NewteraNameSpace.BROWSABLE, "false");
			}

			// Always to call this last
			base.Marshal();
		}
	}
}
/*
* @(#)CustomPageElement.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Collections.Specialized;
	using System.Collections;
	using System.Xml.Schema;
	using System.ComponentModel;
	using System.Drawing.Design;
    using System.Security;

	using Newtera.Common.Core;

	/// <summary>
	/// CustomPageElement represents info for a cusotm page on web client.
	/// </summary>
	/// <version>1.0.1 24 Jul 2009</version>
    public class CustomPageElement : SchemaModelElement
	{
		private string _url = null;
        private string _queryString = null;
        private string _relatedClassName = null;
        private string _visibleCondition = null;
        private string _masterClassName = null; // run-time use

		/// <summary>
		/// Initializing CustomPageElement object.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		public CustomPageElement(string name) : base(name)
		{
		}

		/// <summary>
		/// Initializing CustomPageElement object.
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal CustomPageElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
		}

		/// <summary>
		/// Gets or sets url of the custom page.
		/// </summary>
		[
		    CategoryAttribute("Appearance"),
		    DescriptionAttribute("Specify an url of the custom page, it is usally a .ascx page"),
		    DefaultValueAttribute(null)
		]		
		public string URL
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

        /// <summary>
        /// Gets or sets query string of the custom page.
        /// </summary>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Specify a query string as part of the url, e.g. Name=AA&Type=LLL"),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor)),
            DefaultValueAttribute(null)
        ]
        public string QueryString
        {
            get
            {
                return _queryString;
            }
            set
            {
                _queryString = value;
            }
        }

        /// <summary>
        /// Gets or sets the condition which is in form of XQuery that determines if the custom page should be visible or not.
        /// </summary>
        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("The visible condition for the page"),
        EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string VisibleCondition
        {
            get
            {
                return _visibleCondition;
            }
            set
            {
                _visibleCondition = value;
            }
        }

        /// <summary>
        /// Gets or sets name of a related class that the custom page represents, null if the class the custom
        /// page represents is the same as the master class.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Gets or sets name of a related class that the custom page represents"),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.RelatedClassNamePropertyEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.RelatedClassNamePropertyConverter, Studio")
        ]
        public string RelatedClassName
        {
            get
            {
                return _relatedClassName;
            }
            set
            {
                _relatedClassName = value;
            }
        }

        /// <summary>
        /// The name of a master class the custom pages are associated with
        /// </summary>
        [BrowsableAttribute(false)]
        public string MasetrClassName
        {
            get
            {
                return _masterClassName;
            }
            set
            {
                _masterClassName = value;
            }
        }

		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitCustomPageElement(this);
		}
		
		/// <summary>
		/// Return the name of the CustomPageElement
		/// </summary>
		protected override string ElementName
		{
			get
			{
				return ((XmlSchemaElement) XmlSchemaElement).Name;
			}
		}
		
		/// <summary>
		/// Create xml schema element as an internal representation
		/// of Schema Model element.
		/// </summary>
		/// <returns> Return an XmlSchemaAnnotated object</returns>
		protected override XmlSchemaAnnotated CreateXmlSchemaElement(string name)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = name;

			return xmlSchemaElement;
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		/// <remarks>
		/// Example of simple attributes in xml schema
		/// 
		/// </remarks>
		internal override void Unmarshal()
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			// Set _url member
			_url = this.GetNewteraAttributeValue(NewteraNameSpace.URL);

            // Set _queryString member
            _queryString = this.GetNewteraAttributeValue(NewteraNameSpace.QUERY_STRING);

            // Set _relatedClassName member
            _relatedClassName = this.GetNewteraAttributeValue(NewteraNameSpace.RELATED_CLASS);

            _visibleCondition = this.GetNewteraAttributeValue(NewteraNameSpace.VISIBLE_CONDITION);
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		internal override void Marshal()
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) XmlSchemaElement;

			// Write _url member
			if (!string.IsNullOrEmpty(_url))
			{
				SetNewteraAttributeValue(NewteraNameSpace.URL, _url);	
			}

            // Write _queryString member
            if (!string.IsNullOrEmpty(_queryString))
            {
                SetNewteraAttributeValue(NewteraNameSpace.QUERY_STRING, System.Security.SecurityElement.Escape(_queryString));
            }

            // Write _relatedClassName member
            if (!string.IsNullOrEmpty(_relatedClassName))
            {
                SetNewteraAttributeValue(NewteraNameSpace.RELATED_CLASS, _relatedClassName);
            }

            if (!string.IsNullOrEmpty(_visibleCondition))
            {
                SetNewteraAttributeValue(NewteraNameSpace.VISIBLE_CONDITION, SecurityElement.Escape(_visibleCondition));
            }

			// Always to call this last
			base.Marshal();
		}
    }
}
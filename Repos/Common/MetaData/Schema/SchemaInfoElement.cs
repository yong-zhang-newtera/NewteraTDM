/*
* @(#)SchemaInfoElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
	using System.Xml.Schema;
	using System.ComponentModel;
	using Newtera.Common.Core;

	/// <summary>
	/// The SchemaInfoElement represents information about a schema.
	/// </summary>
	/// <version>  1.0.1 26 Jun 2003</version>
	/// <author>  Yong Zhang</author>
	public class SchemaInfoElement : SchemaModelElement
	{
		private string _version;
        private DateTime _modifiedTime;
        private bool _isBrowsable = true;

		/// <summary>
		/// Initializing a SchemaInfoElement
		/// </summary>
		/// <param name="name">The name of schema</param>
		public SchemaInfoElement(string name) : base(name)
		{
		}
		
		/// <summary>
		/// Initializing a SchemaInfoElement
		/// </summary>
		/// <param name="xmlSchemaElement">xml schema element</param>
		internal SchemaInfoElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
		}

		/// <summary>
		/// Gets or sets version of schema.
		/// </summary>
		/// <value>
		/// The version of schema
		/// </value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The version number of the schema")
		]		
		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

        /// <summary>
        /// Gets or sets modified time of schema.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The modified time of the schema"),
            ReadOnly(true)
        ]	
        public DateTime ModifiedTime
        {
            get
            {
                return _modifiedTime;
            }
            set
            {
                _modifiedTime = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether classes of the schema are browsable.
        /// </summary>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Is classes browsable?"),
            DefaultValueAttribute(true)
        ]
        public bool IsBrowsable
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
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitSchemaInfoElement(this);
		}

		/// <summary>
		/// Return the name of the RangeElement
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
		/// of Schema Info element.
		/// </summary>
		/// <returns> Return an XmlSchemaAnnotated object</returns>
		protected override XmlSchemaAnnotated CreateXmlSchemaElement(string name)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = name;
			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			complexType.Particle = new XmlSchemaAll();
			xmlSchemaElement.SchemaType = complexType;

			return xmlSchemaElement;
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			XmlSchemaElement element = (XmlSchemaElement) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();
			
			_version = GetNewteraAttributeValue(NewteraNameSpace.VERSION);

            string val = GetNewteraAttributeValue(NewteraNameSpace.MODIFIED_TIME);
            if (!string.IsNullOrEmpty(val))
            {
                _modifiedTime = DateTime.Parse(val);
            }

            // Set _isBrowsable member
            if (this.GetNewteraAttributeValue(NewteraNameSpace.BROWSABLE) != null)
            {
                _isBrowsable = false;
            }
            else
            {
                _isBrowsable = true; // default
            }
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		internal override void Marshal()
		{
			SetNewteraAttributeValue(NewteraNameSpace.VERSION, _version);
            if (_modifiedTime != null)
            {
                SetNewteraAttributeValue(NewteraNameSpace.MODIFIED_TIME, _modifiedTime.ToString("s"));
            }

            // write _isBrowsable member
            if (!_isBrowsable)
            {
                SetNewteraAttributeValue(NewteraNameSpace.BROWSABLE, "false");
            }

			base.Marshal();
		}

		/// <summary>
		/// Return a xpath representation of the SchemaModelElement
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
			return "/metadata"; // representing the root of an xpath for a mete data
		}
	}
}
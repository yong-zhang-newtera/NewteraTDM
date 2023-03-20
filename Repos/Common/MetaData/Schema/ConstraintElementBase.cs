/*
* @(#)ConstraintElementBase.cs
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
    using System.Text.RegularExpressions;

	using Newtera.Common.Core;

	/// <summary>
	/// Provides the base functionality for creating a constraint Element
	/// 
	/// </summary>
	/// <version> 1.0.1 26 Jun 2003 </version>
	/// <author> Yong Zhang</author>
	abstract public class ConstraintElementBase : SchemaModelElement
	{
		private DataType _type = DataType.Unknown;
		private string _errorMsg = null;
		private int _index = 0; // run-time use only, for creating a unique enum type name purpose

		/// <summary>
		/// Initializing ConstraintElementBase object
		/// </summary>
		/// <param name="name">Name of attribute</param>
		internal ConstraintElementBase(string name) : base(name)
		{
		}

		/// <summary>
		/// Initializing ConstraintElementBase object
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal ConstraintElementBase(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
		}

		/// <summary>
		/// Gets or sets the data type of the constraint.
		/// </summary>
		/// <value>One of DataType enum values. default is String</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The data type of constraint value(s)"),
			DefaultValueAttribute(DataType.Unknown)
		]		
		public virtual DataType DataType
		{
			get
			{
				return _type;
			}
			set
			{
                _type = value;
			}
		}

		/// <summary>
		/// Gets or sets the data type of the constraint.
		/// </summary>
		/// <value>One of DataType enum values. default is String</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The error message to display when data validatation fails"),
		DefaultValueAttribute(null)
		]		
		public virtual string ErrorMessage
		{
			get
			{
				return _errorMsg;
			}
			set
			{
				_errorMsg = value;
			}
		}

		/// <summary>
		/// Gets a name for creating an enum type of this constraint
		/// </summary>
		[BrowsableAttribute(false)]		
		public string EnumTypeName
		{
			get
			{
                lock (this)
                {
                    return this.Name + _index;
                }
			}
		}

		/// <summary>
		/// Return the name of the Element
		/// </summary>
		[BrowsableAttribute(false)]		
		protected override string ElementName
		{
			get
			{
				return ((XmlSchemaSimpleType) XmlSchemaElement).Name;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the given value is valid
		/// based on the constraint.
		/// </summary>
		/// <param name="value">The given value</param>
		/// <returns>true if it is valid, false otherwise</returns>
		public abstract bool IsValueValid(string value);

        /// <summary>
        /// convert a string of "|' separated enum values into an array of strings.
        /// </summary>
        /// <param name="val">a string of "|" separated enum values</param>
        /// <returns>An array of strings</returns>
        public string[] GetStringArray(string val)
        {
            if (val.Length > 0)
            {
                Regex exp = new Regex(EnumElement.SEPARATOR);

                return exp.Split(val);
            }
            else
            {
                return new string[0];
            }
        }

		/// <summary>
		/// Change the enum type name
		/// </summary>
		protected void ChangeEnumTypeName()
		{
            lock (this)
            {
                _index++;
            }
		} 

		/// <summary>
		/// Create xml schema element as an internal representation
		/// of Schema Model element.
		/// </summary>
		/// <returns> Return an XmlSchemaAnnotated object</returns>
		protected override XmlSchemaAnnotated CreateXmlSchemaElement(string name)
		{
			XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
			simpleType.Name = name;

			// <xs:restriction base="xs:string">
			XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
			
			simpleType.Content = restriction;

			return simpleType;
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) XmlSchemaElement).Content;
			
			_type = DataTypeConverter.ConvertToTypeEnum(restriction.BaseTypeName.Name);

			_errorMsg = this.GetNewteraAttributeValue(NewteraNameSpace.ERROR_MESSAGE);
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		internal override void Marshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) XmlSchemaElement).Content;
			restriction.BaseTypeName = new XmlQualifiedName(DataTypeConverter.ConvertToTypeString(_type), "http://www.w3.org/2003/XMLSchema");

			// Write _errorMsg member
			if (_errorMsg != null && _errorMsg.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.ERROR_MESSAGE, _errorMsg);	
			}

			base.Marshal(); // make sure to call this at the end so that Newtera Namespace attributes will write xml
		}
	}

	/// <summary>
	/// Describes the types for constraint
	/// </summary>
	public enum ConstraintType
	{
		/// <summary>
		/// Enumeration
		/// </summary>
		Enumeration,
		/// <summary>
		/// Pattern
		/// </summary>
		Pattern,
		/// <summary>
		/// Range
		/// </summary>
		Range,
		/// <summary>
		/// List
		/// </summary>
		List
	}
}

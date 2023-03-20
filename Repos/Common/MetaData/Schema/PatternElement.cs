/*
* @(#)PatternElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Text.RegularExpressions;
	using System.Xml;
	using System.Xml.Schema;
	using System.ComponentModel;
	using System.Drawing.Design;
	using Newtera.Common.Core;

	/// <summary>
	/// The PatternElement represents a range constraint.
	/// </summary>
	/// <version>  	1.0.0 26 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class PatternElement : ConstraintElementBase
	{		
		/// <summary>
		/// Get info if the xmlSchemaElemet represents a pattern constraint
		/// </summary>
		/// <param name="xmlSchemaElement">the XmlSchemaAnnotated object
		/// </param>
		/// <returns>
		/// return true if the element represents a patern constraint, 
		/// otherwise, false.
		/// </returns>
		static public bool isPattern(XmlSchemaAnnotated xmlSchemaElement)
		{
			bool status = false;
			
			if (xmlSchemaElement is XmlSchemaSimpleType)
			{
				XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) xmlSchemaElement).Content;
				
				if (restriction != null)
				{

					// looking for MaxInclusive facet, minInclusive is optional
					foreach (XmlSchemaFacet facet in restriction.Facets)
					{
						if (facet is XmlSchemaPatternFacet)
						{
							status = true;
							break;
						}
					}
				}
			}
			
			return status;
		}

        /// <summary>
        /// Compare the values of two pattern
        /// </summary>
        /// <param name="firstPattern"></param>
        /// <param name="secondPattern"></param>
        /// <returns>return true if the two patterns are the same, false if different</returns>
        static public bool Compare(PatternElement firstPattern, PatternElement secondPattern)
        {
            bool result = true;

            if (firstPattern.PatternValue != firstPattern.PatternValue)
            {
                result = false;
            }

            return result;
        }

		private string _patternValue;

		/// <summary>
		/// Initializing a PatternElement object
		/// </summary>
		/// <param name="name">Name of element</param>
		public PatternElement(string name): base(name)
		{
			_patternValue = null;

			DataType = DataType.String; // default type for Pattern
		}

		/// <summary>
		/// Initializing a PatternElement object
		/// </summary>
		/// <param name="xmlSchemaElement">The XmlSchemaAnnotated object</param>
		internal PatternElement(XmlSchemaAnnotated xmlSchemaElement): base(xmlSchemaElement)
		{
			_patternValue = null;
		}

		/// <summary>
		/// Gets or sets the pattern value.
		/// </summary>
		/// <value>Value of the pattern, empty if not set</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("A regular expression representing the pattern."),
			DefaultValueAttribute(null)
		]		
		public virtual string PatternValue
		{
			get
			{
				return _patternValue;
			}
			set
			{
				_patternValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the data type of the constraint.
		/// </summary>
		/// <value>Alwayse String</value>
		public override DataType DataType
		{
			get
			{
				return DataType.String;
			}
			set
			{
				base.DataType = DataType.String;
			}
		}

		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitPatternElement(this);
		}

		/// <summary>
		/// Gets the information indicating whether the given value is valid
		/// based on the constraint.
		/// </summary>
		/// <param name="value">The given value</param>
		/// <returns>true if it is valid, false otherwise</returns>
		public override bool IsValueValid(string value)
		{
			Regex regex = new Regex(_patternValue);

			return regex.IsMatch(value);
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;
			foreach (XmlSchemaFacet facet in restriction.Facets)
			{
				if (facet is XmlSchemaPatternFacet)
				{
					_patternValue = facet.Value;
				}
			}
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <!--
		/// The sample XML code for a Pattern.
		/// 
		/// <code>
		/// <xsd:simpleType name="partNum">
		/// <xsd:restriction base="xsd:string">
		/// <xsd:maxLength value="6"/>
		/// <xsd:pattern value="\d{3}-[A-Z]{2}"/>
		/// </xsd:restriction>
		/// </xsd:simpleType>
		/// </code>
		/// -->
		internal override void Marshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) XmlSchemaElement).Content;
			XmlSchemaPatternFacet pattern = new XmlSchemaPatternFacet();
			pattern.Value = _patternValue;
			restriction.Facets.Add(pattern);

			base.Marshal();
		}
	}
}
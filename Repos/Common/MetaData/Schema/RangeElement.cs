/*
* @(#)RangeElement.cs
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
	/// The PatternElement represents a range constraint. 
	/// </summary>
	/// <version>  	1.0.1 26 Jun 2003
	/// </version>
	/// <author> Yong Zhang</author>
	public class RangeElement : ConstraintElementBase
	{
		/// <summary>
		/// Get info if the xmlSchemaElemet represents a range constraint
		/// </summary>
		/// <param name="xmlSchemaElement">the XmlSchemaAnnotated object
		/// </param>
		/// <returns>
		/// return true if the element represents a range constraint, 
		/// otherwise, false.
		/// </returns>
		static public bool isRange(XmlSchemaAnnotated xmlSchemaElement)
		{
			bool status = false;
			
			if (xmlSchemaElement is XmlSchemaSimpleType)
			{
				XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) ((XmlSchemaSimpleType) xmlSchemaElement).Content;
				
				if (restriction != null)
				{
					if (restriction.Facets.Count > 0 )
					{
						// looking for MaxInclusive facet, minInclusive is optional
						foreach (XmlSchemaFacet facet in restriction.Facets)
						{
							if (facet is XmlSchemaMaxInclusiveFacet)
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
        /// Compare the values of two range
        /// </summary>
        /// <param name="firstRange"></param>
        /// <param name="secondRange"></param>
        /// <returns>return true if the two range are the same, false if different</returns>
        static public bool Compare(RangeElement firstRange, RangeElement secondRange)
        {
            bool result = true;

            if (firstRange.MinValue != secondRange.MinValue || firstRange.MaxValue != secondRange.MaxValue)
            {
                result = false;
            }

            return result;
        }

		private string _minValue;
		private string _maxValue;

		/// <summary>
		/// Initializing a RangeElement object
		/// </summary>
		/// <param name="name">Name of element</param>
		public RangeElement(string name): base(name)
		{
			_minValue = null;
			_maxValue = null;

			DataType = DataType.Double; // default type for Range
		}

		/// <summary>
		/// Initializing a RangeElement object
		/// </summary>
		/// <param name="xmlSchemaElement">The XmlSchemaAnnotated object</param>
		internal RangeElement(XmlSchemaAnnotated xmlSchemaElement): base(xmlSchemaElement)
		{
			_minValue = null;
			_maxValue = null;
		}

		/// <summary>
		/// Gets or sets the min value.
		/// </summary>
		/// <value>Min value of the range, 0 if not set</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The minimum value of a range")
		]			
		public virtual string MinValue
		{
			get
			{	
				return (_minValue != null && _minValue.Length > 0? _minValue : "0");
			}
			set
			{
				_minValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the max value.
		/// </summary>
		/// <value>Max value of the range, 0 if not set</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The maximum value of a range")
		]		
		public virtual string MaxValue
		{
			get
			{
				return (_maxValue != null && _maxValue.Length > 0? _maxValue : "0");
			}
			set
			{
				_maxValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the data type of the constraint.
		/// </summary>
		/// <value>One of DataType enum values. default is Integer</value>
		public override DataType DataType
		{
			get
			{
				if (base.DataType == DataType.Unknown)
				{
					return DataType.Integer;
				}
				else
				{
					return base.DataType;
				}
			}
			set
			{
				base.DataType = value;
			}
		}

		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitRangeElement(this);
		}

		/// <summary>
		/// Gets the information indicating whether the given value is valid
		/// based on the constraint.
		/// </summary>
		/// <param name="value">The given value</param>
		/// <returns>true if it is valid, false otherwise</returns>
		public override bool IsValueValid(string value)
		{
			bool status = true;

			switch (DataType)
			{
				case DataType.Integer:
					int minIntVal = System.Convert.ToInt32(MinValue);
					int maxIntVal = System.Convert.ToInt32(MaxValue);
					int theIntValue = System.Convert.ToInt32(value);
					if (theIntValue < minIntVal || theIntValue > maxIntVal)
					{
						status = false;
					}

					break;

				case DataType.BigInteger:
					long minLongVal = System.Convert.ToInt64(MinValue);
					long maxLongVal = System.Convert.ToInt64(MaxValue);
					long theLongValue = System.Convert.ToInt64(value);
					if (theLongValue < minLongVal || theLongValue > maxLongVal)
					{
						status = false;
					}

					break;

				case DataType.Float:
					float minFloatVal = System.Convert.ToSingle(MinValue);
					float maxFloatVal = System.Convert.ToSingle(MaxValue);
					float theFloatValue = System.Convert.ToSingle(value);
					if (theFloatValue < minFloatVal || theFloatValue > maxFloatVal)
					{
						status = false;
					}

					break;
				case DataType.Double:
					double minDoubleVal = System.Convert.ToDouble(MinValue);
					double maxDoubleVal = System.Convert.ToDouble(MaxValue);
					double theDoubleValue = System.Convert.ToDouble(value);
					if (theDoubleValue < minDoubleVal || theDoubleValue > maxDoubleVal)
					{
						status = false;
					}

					break;
				case DataType.Decimal:
					decimal minDecimalVal = System.Convert.ToDecimal(MinValue);
					decimal maxDecimalVal = System.Convert.ToDecimal(MaxValue);
					decimal theDecimalValue = System.Convert.ToDecimal(value);
					if (theDecimalValue < minDecimalVal || theDecimalValue > maxDecimalVal)
					{
						status = false;
					}

					break;
				case DataType.Date:
				case DataType.DateTime:
					DateTime minDateTimeVal = System.Convert.ToDateTime(MinValue);
					DateTime maxDateTimeVal = System.Convert.ToDateTime(MaxValue);
					DateTime theDateTimeValue = System.Convert.ToDateTime(value);
					if (theDateTimeValue < minDateTimeVal || theDateTimeValue > maxDateTimeVal)
					{
						status = false;
					}

					break;
				default:
					// for other types
					if (value.CompareTo(MinValue) < 0 || value.CompareTo(MaxValue) > 0)
					{
						status = false;
					}

					break;
			}
			return status;
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
				if (facet is XmlSchemaMinInclusiveFacet)
				{
					_minValue = facet.Value;
				}
				else if (facet is XmlSchemaMaxInclusiveFacet)
				{
					_maxValue = facet.Value;
				}
			}

			// Set type member
			DataType = DataTypeConverter.ConvertToTypeEnum(restriction.BaseTypeName.Name);
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <!--
		/// The sample XML code for a Range:
		/// 
		/// <xsd:simpleType name="quantity">
		/// <xsd:restriction base="xsd:integer">
		/// <xsd:maxInclusive value="10"/>
		/// <xsd:maxInclusive value="100"/>
		/// </xsd:restriction>
		/// </xsd:simpleType>
		/// -->
		internal override void Marshal()
		{
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) XmlSchemaElement;

			XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;

			XmlSchemaMinInclusiveFacet minInclusive = new XmlSchemaMinInclusiveFacet();
			minInclusive.Value = _minValue;
			restriction.Facets.Add(minInclusive);

			XmlSchemaMaxInclusiveFacet maxInclusive = new XmlSchemaMaxInclusiveFacet();
			maxInclusive.Value = _maxValue;
			restriction.Facets.Add(maxInclusive);

			// Write type member
			restriction.BaseTypeName = new XmlQualifiedName(DataTypeConverter.ConvertToTypeString(DataType), "http://www.w3.org/2003/XMLSchema");
		
			base.Marshal();
		}
	}
}
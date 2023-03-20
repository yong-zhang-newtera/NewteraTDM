/*
* @(#)BackwardRelationshipPropertyConverter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// Conversion between a RelationshipAttributeElement object and string
	/// </summary>
	/// <version>  1.0.1 29 Sept 2003</version>
	/// <author> Yong Zhang</author>
	public class BackwardRelationshipPropertyConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert a RelationshipAttributeElement to a string type
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			// we can convert from a RelationshipAttributeElement object to a string type
			if (sourceType == typeof(RelationshipAttributeElement))
			{
				return true;
			}

			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Convert from a RelationshipAttributeElement object to string
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns>a string</returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			// if the source value is a RelationshipAttributeElement type
			if (value is RelationshipAttributeElement)
			{
				string converted = null;

				// Convert to string
				if (destinationType == typeof(string))
				{
					RelationshipAttributeElement relationship = (RelationshipAttributeElement) value;
					converted =  relationship.Caption + " (" + relationship.Name + ")";
				}

				return converted;
			}

			return "(None)";
		}

		/// <summary>
		/// Convert from string to a ClassElement
		/// </summary>
		/// <param name="context"></param>
		/// <param name="info"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo info, object value)
		{
			return base.ConvertFrom(context, info, value);
		}
	}
}
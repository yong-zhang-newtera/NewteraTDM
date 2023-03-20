/*
* @(#)ConstraintPropertyConverter.cs
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
	/// Conversion between a constraint object and string
	/// </summary>
	/// <version>  1.0.1 29 Sept 2003</version>
	/// <author> Yong Zhang</author>
	public class ConstraintPropertyConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert a constraint to a string type
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			// we can convert from a constraint object to a string type
			if (sourceType == typeof(ConstraintElementBase))
			{
				return true;
			}
			else if (sourceType == typeof(string))
			{
				return false;
			}

			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Convert from a constraint to string
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			// if the source value is a constraint type
			if (value is ConstraintElementBase)
			{
				string converted = null;

				// Convert to string
				if (destinationType == typeof(string))
				{
					if (value is EnumElement)
					{
						converted = ((EnumElement) value).Caption + " (Enum)";
					}
					else if (value is RangeElement)
					{
						converted = ((RangeElement) value).Caption + " (Range)";
					}
					else if (value is PatternElement)
					{
						converted = ((PatternElement) value).Caption + " (Pattern)";
					}
					else if (value is ListElement)
					{
						converted = ((ListElement) value).Caption + " (List)";
					}
				}

				return converted;
			}

			return "(None)";
			//return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// Convert from string to a constraint
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
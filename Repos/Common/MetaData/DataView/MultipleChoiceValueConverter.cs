/*
* @(#)MultipleChoiceValueConverter.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text.RegularExpressions;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Conversion between an array of enum values and string
	/// </summary>
	/// <version>  1.0.1 07 Jul. 2004</version>
	/// <author> Yong Zhang</author>
	public class MultipleChoiceValueConverter : TypeConverter
	{
		private Type _enumType;

		/// <summary>
		/// Initiating an instance of MultipleChoiceValueConverter class
		/// </summary>
		public MultipleChoiceValueConverter(Type enumType) : base()
		{
			_enumType = enumType;
		}

		/// <summary> 
		/// Let clients know it can convert an array of enum values to a string type
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			// we can convert from an array of enum object to a string type
			// but not from string to an array of enum
			if (sourceType == typeof(Array))
			{
				return true;
			}
			else if (sourceType == typeof(string))
			{
				return false;
			}
			else
			{
				return base.CanConvertFrom(context, sourceType);
			}
		}

		/// <summary>
		/// Convert from an array of enum object to string
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			string converted = "";

			// if the source value is an array of enum object type
			if (value is Array)
			{
				object[] values = (object[]) value;

				// Convert to string
				if (destinationType == typeof(string))
				{
					for (int i = 0; i < values.Length; i++)
					{
						if (converted.Length > 0)
						{
							converted = converted + EnumElement.SEPARATOR + Enum.GetName(_enumType, values[i]);
						}
						else
						{
							converted = Enum.GetName(_enumType, values[i]);
						}
					}
				}
			}

			return converted;
		}
	}
}
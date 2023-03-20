/*
* @(#)ArrayDataRowViewConverter.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Data;
	using System.ComponentModel;
	using System.Globalization;

	/// <summary>
	/// Conversion between a ArrayDataRowView object and string
	/// </summary>
	/// <version>  1.0.1 29 Sep. 2004</version>
	/// <author> Yong Zhang</author>
	public class ArrayDataRowViewConverter : ExpandableObjectConverter
	{
		internal const int DISPLAY_LENGHTH = 200;

		/// <summary>
		/// Initiating an instance of ArrayDataRowViewConverter class
		/// </summary>
		public ArrayDataRowViewConverter() : base()
		{
		}

		/// <summary> 
		/// Let clients know it can convert a ArrayDataRowView to a string type
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			// we can convert from a ArrayDataRowView object to a string type
			// but not from string to a ArrayDataRowView
			if (sourceType == typeof(ArrayDataRowView))
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
		/// Convert from a ArrayDataRowView object to string
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

			// if the source value is a ArrayDataRowView type
			if (value is ArrayDataRowView)
			{
				// Convert to string
				if (destinationType == typeof(string))
				{
					converted = ((ArrayDataRowView) value).ToString();

					// the converted string is used to display at property grid,
					// since the property grid is slow to display a long string,
					// we have to truncate the string for display purpose.
					if (converted.Length > ArrayDataTableViewConverter.DISPLAY_LENGHTH)
					{
						converted = converted.Substring(0, ArrayDataTableViewConverter.DISPLAY_LENGHTH);
					}
				}
			}

			return converted;
		}
	}
}
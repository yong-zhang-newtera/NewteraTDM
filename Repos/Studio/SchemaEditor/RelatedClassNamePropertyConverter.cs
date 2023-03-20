/*
* @(#)RelatedClassNamePropertyConverter.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData;

	/// <summary>
	/// Conversion between a class name and the class caption
	/// </summary>
	/// <version>  1.0.1 26 Jul. 2009</version>
	public class RelatedClassNamePropertyConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert bewtween a class name to a class caption
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			else
			{
				return base.CanConvertFrom(context, sourceType);
			}
		}

		/// <summary>
		/// Convert from a class name to a class caption
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			object converted = "";

			// if the source value is a string type
			if (value != null && value is String)
			{
				if (context.Instance is CustomPageElement)
				{
                    CustomPageElement customPage = (CustomPageElement)context.Instance;

					// Convert to caption
                    ClassElement classElement = customPage.SchemaModel.FindClass((string)value);

                    if (classElement != null)
                    {
                        converted = classElement.Caption;
                    }
                    else
                    {
                        converted = "Unknown";
                    }
				}
			}

			return converted;
		}

		/// <summary>
		/// Convert from a data view caption to a class name
		/// </summary>
		/// <param name="context"></param>
		/// <param name="info"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo info, object value)
		{
			object converted = "";

			return converted;
		}
	}
}
/*
* @(#)ClassNameConverter.cs
*
* Copyright (c) 2011 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Conversion between a class name and the class caption
	/// </summary>
	/// <version>  1.0.1 15 June. 2011</version>
	public class ClassNameConverter : TypeConverter
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
				if (context.Instance is SimpleAttributeElement)
				{
                    SimpleAttributeElement node = (SimpleAttributeElement)context.Instance;

					// Convert to caption
					ClassElement classElement = node.OwnerClass.SchemaModel.FindClass((string) value);

					if (classElement != null)
					{
						converted = classElement.Caption;
					}
				}
			}

			return converted;
		}

		/// <summary>
		/// Convert from a class caption to a class name
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
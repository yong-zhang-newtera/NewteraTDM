/*
* @(#)CascadedAttributePropertyConverter.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
    using System.Text;
	using System.ComponentModel;
	using System.Globalization;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;

	/// <summary>
	/// Conversion between a cascaded attribute name and caption
	/// </summary>
	/// <version>  1.0.1 14 Nov. 2009</version>
	public class CascadedAttributePropertyConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert bewtween a cascade attribute name to display caption
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
		/// Convert from an attribute name to an attribute caption
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
            object converted = NewteraNameSpace.NONE;

			// if the source value is a string type
            if (value != null && value is String)
            {
                if (context.Instance is SimpleAttributeElement || context.Instance is RelationshipAttributeElement)
                {
                    AttributeElementBase attributeElement = (AttributeElementBase)context.Instance;
                    ClassElement classElement = attributeElement.OwnerClass;

                    // Convert to display text
                    string attributeNameStr = (string)value;
                    if (attributeNameStr == NewteraNameSpace.NONE)
                    {
                        converted = NewteraNameSpace.NONE;
                    }
                    else
                    {
                        string[] attributeNames = attributeNameStr.Split(';');
                        StringBuilder builder = new StringBuilder();
                        foreach (string attributeName in attributeNames)
                        {
                            foreach (SimpleAttributeElement attr in classElement.SimpleAttributes)
                            {
                                if (attr.Name == attributeName)
                                {
                                    if (builder.Length > 0)
                                    {
                                        builder.Append(";");
                                    }

                                    builder.Append(attr.Caption);
                                    break;
                                }
                            }
                        }

                        converted = builder.ToString();
                    }
                }
            }

			return converted;
		}

		/// <summary>
		/// Convert from an attribute caption to the attribute name
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
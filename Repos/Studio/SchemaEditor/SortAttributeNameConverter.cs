/*
* @(#)SortAttributeNameConverter.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;

	/// <summary>
	/// Conversion between an sort attribute name and caption
	/// </summary>
	/// <version>  1.0.1 28 Jul. 2008</version>
	public class SortAttributeNameConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert bewtween a sort attribute name to display caption
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
                if (context.Instance is ClassElement)
                {
                    ClassElement classElement = (ClassElement)context.Instance;

                    // Convert to display text
                    string attributeName = (string)value;
                    if (attributeName == NewteraNameSpace.NONE)
                    {
                        converted = NewteraNameSpace.NONE;
                    }
                    else if (attributeName == NewteraNameSpace.OBJ_ID)
                    {
                        converted = MessageResourceManager.GetString("SchemaEditor.ObjId");
                    }
                    else
                    {
                        foreach (SimpleAttributeElement attr in classElement.SimpleAttributes)
                        {
                            if (attr.Name == attributeName)
                            {
                                converted = attr.Caption;
                                break;
                            }
                        }
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
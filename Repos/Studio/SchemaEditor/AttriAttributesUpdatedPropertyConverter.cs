/*
* @(#)AttributesUpdatedPropertyConverter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.ComponentModel;
    using System.Text;
    using System.Collections.Specialized;
	using System.Globalization;

	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Events;

	/// <summary>
	/// Conversion between a StringCollection and A string of concatenated attribute captions
	/// </summary>
	/// <version>  1.0.1 09 Jan 2007</version>
	public class AttributesUpdatedPropertyConverter : TypeConverter
	{
		/// <summary> 
        /// Let clients know it can convert bewtween a a StringCollection to a string
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(StringCollection))
			{
				return true;
			}
			else
			{
				return base.CanConvertFrom(context, sourceType);
			}
		}

		/// <summary>
        /// Convert from a StringCollection to a string of comcatenated attribute captions
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			StringBuilder builder = new StringBuilder();

			// if the source value is a string type
			if (value != null && value is StringCollection)
			{
				if (context.Instance is EventDef)
				{
                    
                    EventDef eventDef = (EventDef)context.Instance;
                    MetaDataModel metaData = eventDef.MetaData;
                    ClassElement theClass = metaData.SchemaModel.FindClass(eventDef.ClassName);
                    SimpleAttributeElement simpleAttribute;

                    if (theClass != null)
                    {
                        int index = 0;
                        StringCollection names = (StringCollection)value;
                        foreach (string name in names)
                        {
                            simpleAttribute = theClass.FindInheritedSimpleAttribute(name);
                            if (simpleAttribute != null)
                            {
                                if (index == 0)
                                {
                                    builder.Append(simpleAttribute.Caption);
                                }
                                else
                                {
                                    builder.Append(";").Append(simpleAttribute.Caption);
                                }
                            }
                        }
                    }
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Convert from a data view caption to a data view name
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
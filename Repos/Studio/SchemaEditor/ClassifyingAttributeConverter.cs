/*
* @(#)ClassifyingAttributeConverter.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Conversion between an attribute name and caption
	/// </summary>
	/// <version>  1.0.1 14 June 2008</version>
	public class ClassifyingAttributeConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert bewtween an attribute name to attribute caption
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
			object converted = "";

			// if the source value is a string type
			if (value != null && value is String)
			{
				if (context.Instance is AutoClassifyLevel)
				{
                    AutoClassifyLevel autoClassifyLevel = (AutoClassifyLevel)context.Instance;

                    DataViewModel dataView = autoClassifyLevel.DataView;
                    if (dataView != null)
                    {
                        ResultAttributeCollection attributes = dataView.ResultAttributes;

                        foreach (IDataViewElement attr in attributes)
                        {
                            if (attr is DataSimpleAttribute)
                            {
                                DataSimpleAttribute simpleAttribute = (DataSimpleAttribute)attr;
                                if (simpleAttribute.Name == (string)value &&
                                    simpleAttribute.OwnerClassAlias == autoClassifyLevel.OwnerClassAlias)
                                {
                                    converted = attr.Caption;
                                    break;
                                }
                            }
                            else if (attr is DataRelationshipAttribute)
                            {
                                DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute)attr;

                                // only the relationships of many-to-one and one-to-one can be used as classifying attribute
                                if (relationshipAttribute.IsForeignKeyRequired)
                                {
                                    if (relationshipAttribute.Name == (string)value &&
                                        relationshipAttribute.OwnerClassAlias == autoClassifyLevel.OwnerClassAlias)
                                    {
                                        converted = attr.Caption;
                                        break;
                                    }
                                }
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
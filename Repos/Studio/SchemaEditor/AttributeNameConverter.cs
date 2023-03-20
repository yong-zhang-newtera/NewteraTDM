/*
* @(#)AttributeNameConverter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Subscribers;

	/// <summary>
	/// Conversion between an attribute name and caption
	/// </summary>
	/// <version>  1.0.1 10 Aug. 2007</version>
	public class AttributeNameConverter : TypeConverter
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
			string converted = "";

			// if the source value is a string type
			if (value != null && value is String)
			{
				if (context.Instance is ClassElement)
				{
                    ClassElement classElement = (ClassElement)context.Instance;

                    // Convert to caption
                    string attributeName = (string) value;
                    foreach (SimpleAttributeElement attr in classElement.SimpleAttributes)
                    {
                        if (attr.Name == attributeName)
                        {
                            converted = attr.Caption;
                            break;
                        }
                    }
				}
                else if (context.Instance is Subscriber)
                {
                    Subscriber subscriber = (Subscriber)context.Instance;

                    MetaDataModel metaData = subscriber.MetaData;
                    ClassElement classElement = metaData.SchemaModel.FindClass(subscriber.ClassName);

                    // Convert to caption
                    string attributeName = (string)value;
                    SimpleAttributeElement simpleAttribute = classElement.FindInheritedSimpleAttribute(attributeName);
                    if (simpleAttribute != null)
                    {
                        converted = simpleAttribute.Caption;
                    }
                }
                else if (context.Instance is SimpleAttributeElement)
                {
                    SimpleAttributeElement simpleAttributeElement = (SimpleAttributeElement)context.Instance;

                    SchemaModel schemaModel = simpleAttributeElement.SchemaModel;
                    ClassElement classElement = schemaModel.FindClass(simpleAttributeElement.OwnerClass.Name);

                    // Convert to caption
                    string attributeName = (string)value;
                    SimpleAttributeElement simpleAttribute = classElement.FindInheritedSimpleAttribute(attributeName);
                    RelationshipAttributeElement relationshipAttribute = classElement.FindInheritedRelationshipAttribute(attributeName);
                    if (simpleAttribute != null)
                    {
                        converted = simpleAttribute.Caption;
                    }
                    else if (relationshipAttribute != null)
                    {
                        converted = relationshipAttribute.Caption;
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
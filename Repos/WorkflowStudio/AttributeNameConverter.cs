/*
* @(#)AttributeNameConverter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

    using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Activities;
    using Newtera.WFModel;

	/// <summary>
	/// Conversion between an attribute name and the caption
	/// </summary>
	/// <version>  1.0.1 18 Jan 2007</version>
	public class AttributeNameConverter : TypeConverter
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
                if (context.Instance is CreateTaskActivity)
				{
                    CreateTaskActivity activity = (CreateTaskActivity)context.Instance;
                    INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                    // Convert an attribute name to its caption
                    ClassElement clsElement = null;
                    MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(rootActivity.SchemaId);
                    if (metaData != null && rootActivity.ClassName != null)
                    {
                        clsElement = metaData.SchemaModel.FindClass(rootActivity.ClassName);
                    }

                    string propertyName = context.PropertyDescriptor.Name;
                    if (clsElement != null)
                    {
                        SimpleAttributeElement attributeElement = clsElement.FindInheritedSimpleAttribute((string)value);
                        if (attributeElement != null)
                        {
                            converted = attributeElement.Caption;

                            if (propertyName == "UsersBindingAttribute")
                            {
                                activity.UsersBindingAttributeCaption = attributeElement.Caption;
                            }
                            else
                            {
                                activity.RolesBindingAttributeCaption = attributeElement.Caption;
                            }
                        }
                        else
                        {
                            if (propertyName == "UsersBindingAttribute")
                            {
                                converted = activity.UsersBindingAttributeCaption; // when attribute doesn't exist
                            }
                            else
                            {
                                converted = activity.RolesBindingAttributeCaption; // when attribute doesn't exist
                            }
                        }
                    }
                    else
                    {
                        if (propertyName == "UsersBindingAttribute")
                        {
                            converted = activity.UsersBindingAttributeCaption; // when meta-data isn't available
                        }
                        else
                        {
                            converted = activity.RolesBindingAttributeCaption; // when meta-data isn't available
                        }
                    }
				}
			}

			return converted;
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
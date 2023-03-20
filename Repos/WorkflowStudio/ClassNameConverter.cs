/*
* @(#)ClassNameConverter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
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
	/// Conversion between a class name and the class caption
	/// </summary>
	/// <version>  1.0.1 02 Jan 2007</version>
	public class ClassNameConverter : TypeConverter
	{
        //public int 转换器名称
        public int ConverterName
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

		/// <summary> 
		/// Let clients know it can convert bewtween a class name to a class caption
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
                //this.转换器名称 = 0;
                this.ConverterName = 0;
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
				if (context.Instance is INewteraWorkflow)
				{
                    INewteraWorkflow activity = (INewteraWorkflow)context.Instance;

                    // Convert a class name to its caption
                    ClassElement clsElement = null;
                    MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(activity.SchemaId);
                    if (metaData != null)
                    {
                        clsElement = metaData.SchemaModel.FindClass((string)value);
                    }

                    if (clsElement != null)
                    {
                        converted = clsElement.Caption;
                        activity.ClassCaption = clsElement.Caption;
                    }
                    else
                    {
                        converted = activity.ClassCaption; // when meta-data isn't available
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
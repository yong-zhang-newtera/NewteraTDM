/*
* @(#)DataViewNameConverter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// Conversion between a data view name and the data view caption
	/// </summary>
	/// <version>  1.0.1 05 Nov. 2004</version>
	/// <author> Yong Zhang</author>
	public class DataViewNameConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert bewtween a data view name to a data view caption
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
		/// Convert from a data view name to a data view caption
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
				if (context.Instance is ITaxonomy)
				{
					ITaxonomy node = (ITaxonomy) context.Instance;

					// Convert to caption
					DataViewModel dataView = (DataViewModel) node.MetaDataModel.DataViews[(string) value];

					if (dataView != null)
					{
						converted = dataView.Caption;
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
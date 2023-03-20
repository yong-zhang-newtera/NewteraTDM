/*
* @(#)CurrentSiteMapNameConverter.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.SiteMap;
    using Newtera.WinClientCommon;

	/// <summary>
	/// Conversion between a class name and the class caption
	/// </summary>
	/// <version>  1.0.0 19 Jul. 2012</version>
	public class CurrentSiteMapNameConverter : TypeConverter
	{
		/// <summary> 
		/// Let clients know it can convert bewtween a sitemap name to a sitemap caption
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
		/// Convert from a sitemap title to the sitemap name
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
                string siteMapName = (string) value;
				// Convert to caption
                if (context.Instance is SiteMapModelSet)
                {
                    SiteMapModelSet siteMapModelSet = (SiteMapModelSet)context.Instance;
                    foreach (SiteMapModel siteMapModel in siteMapModelSet.ChildNodes)
                    {
                        if (siteMapModel.Name == siteMapName)
                        {
                            converted = siteMapModel.Title;
                            break;
                        }
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
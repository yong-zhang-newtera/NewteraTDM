/*
* @(#)ClassNameConverter.cs
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
	/// <version>  1.0.1 27 Nov. 2010</version>
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
				// Convert to caption
                ClassElement classElement = null;
                string database = null;
                if (context.Instance is CustomCommandGroup)
                {
                    CustomCommandGroup customCommandGroup = (CustomCommandGroup)context.Instance;
                    database = customCommandGroup.Database;
                }
                else if (context.Instance is SiteMapNode)
                {
                    SiteMapNode siteMapNode = (SiteMapNode)context.Instance;
                    database = siteMapNode.Database;
                }

                if (!string.IsNullOrEmpty(database))
                {
                    MetaDataModel metaData = GetMetaDataModel(database);

                    if (metaData != null)
                    {
                        classElement = metaData.SchemaModel.FindClass((string)value);

                        if (classElement != null)
                        {
                            converted = classElement.Caption;
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

        /// <summary>
        /// Gets the meta data model indicated by the schemaId
        /// </summary>
        /// <param name="schemaId">The schema info</param>
        /// <returns>The MetaDataModel object</returns>
        private MetaDataModel GetMetaDataModel(string schemaId)
        {
            MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(schemaId);
            if (metaData == null)
            {
                System.Windows.Forms.Cursor preCursor = System.Windows.Forms.Cursor.Current;
                try
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                    MetaDataServiceStub metaDataService = new MetaDataServiceStub();

                    // Create an meta data object to hold the schema model
                    metaData = new MetaDataModel();
                    string[] strings = schemaId.Split(' ');
                    Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                    schemaInfo.Name = strings[0];
                    schemaInfo.Version = strings[1];
                    metaData.SchemaInfo = schemaInfo;

                    // create a MetaDataModel instance from the xml strings retrieved from the database
                    string[] xmlStrings = metaDataService.GetMetaData(ConnectionStringBuilder.Instance.Create(metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
                    metaData.Load(xmlStrings);

                    // save it for future reference
                    MetaDataStore.Instance.PutMetaData(metaData);
                }
                finally
                {
                    System.Windows.Forms.Cursor.Current = preCursor;
                }
            }

            return metaData;
        }
	}
}
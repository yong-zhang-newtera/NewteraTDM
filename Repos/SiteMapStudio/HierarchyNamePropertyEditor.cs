/*
* @(#)HierarchyNamePropertyEditor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	using System;
    using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;
    using System.Threading;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

	using Newtera.WinClientCommon;
    using Newtera.Common.Core;
	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.DataView.Taxonomy;
    using Newtera.Common.MetaData.SiteMap;
	
	/// <summary>
	/// A DropDown UI editor for the Database property of MenuGroup in
	/// the namespace of Newtera.Common.MetaData.SiteMap.
	/// </summary>
	/// <version>  1.0.1 18 Jun 2009</version>
	public class HierarchyNamePropertyEditor : UITypeEditor
	{
        private const string DASHBOARD = "DASHBOARD";

		/// <summary>
		/// Initializes a new instance of the HierarchyNamePropertyEditor class.
		/// </summary>
		public HierarchyNamePropertyEditor() : base()
		{
		}

		/// <summary> 
		/// Overrides the inherited method to return a Modal style
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}

			return base.GetEditStyle(context);
		}

		/// <summary>
		/// Override the method to show a dropdown list of schema ids
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen schema id</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (editorService != null)
				{
					// Create an instance of UI editor control
					DropDownListControl listPicker = new DropDownListControl(editorService);
                    Newtera.Common.MetaData.SiteMap.Menu menu = null;
                    List<IDataViewElement> taxonomies = null;

                    if (context.Instance is Newtera.Common.MetaData.SiteMap.Menu)
					{
                        menu = (Newtera.Common.MetaData.SiteMap.Menu)context.Instance;

                        taxonomies = GetTaxonomies(menu);

						// display the available hierarchy names in the dropdown list
                        if (taxonomies != null)
                        {
                            string[] captions = new string[taxonomies.Count];
                            int index = 0;
                            foreach (TaxonomyModel taxonomy in taxonomies)
                            {
                                captions[index++] = taxonomy.Caption;
                            }
                            listPicker.DataSource = captions;
                        }
					}

					editorService.DropDownControl(listPicker);

                    if (menu != null && taxonomies != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < taxonomies.Count)
					{
                        return taxonomies[listPicker.SelectedIndex].Name;
					}
					else
					{
						return "";
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        /// <summary>
        /// Gets available hierarchy names
        /// </summary>
        /// <returns></returns>
        private List<IDataViewElement> GetTaxonomies(Newtera.Common.MetaData.SiteMap.Menu menu)
        {
            List<IDataViewElement> taxonomies = null;

            SideMenuGroup menuGroup = (SideMenuGroup)menu.ParentNode;

            if (!string.IsNullOrEmpty(menuGroup.Database) &&
                (menu.Type == MenuType.Dashboard || menu.Type == MenuType.Tree))
            {

                Cursor preCursor = Cursor.Current;

                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    MetaDataServiceStub metaDataService = new MetaDataServiceStub();
                    MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(menuGroup.Database);
                    if (metaData == null)
                    {
                        metaData = new MetaDataModel();
                        string[] strings = menuGroup.Database.Split(' ');
                        Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                        schemaInfo.Name = strings[0];
                        schemaInfo.Version = strings[1];
                        metaData.SchemaInfo = schemaInfo;

                        // invoke the web service synchronously
                        string[] xmlStrings = metaDataService.GetMetaData(ConnectionStringBuilder.Instance.Create(menuGroup.Database));

                        // read mete-data from xml strings
                        metaData.Load(xmlStrings);

                        // Replace the meta data in a meta-data store as the current meta-data
                        MetaDataStore.Instance.PutMetaData(metaData);
                    }

                    taxonomies = new List<IDataViewElement>();
                    foreach (TaxonomyModel taxonomy in metaData.Taxonomies)
                    {
                        if (menu.Type == MenuType.Dashboard)
                        {
                            if (taxonomy.Description != null &&
                                taxonomy.Description.ToUpper() == DASHBOARD)
                            {
                                taxonomies.Add(taxonomy);
                            }
                        }
                        else if (menu.Type == MenuType.Tree)
                        {
                            if (taxonomy.Description == null ||
                                taxonomy.Description.ToUpper() != DASHBOARD)
                            {
                                taxonomies.Add(taxonomy);
                            }
                        }
                    }
                }
                finally
                {
                    Cursor.Current = preCursor;
                }
            }

            return taxonomies;
        }
	}
}
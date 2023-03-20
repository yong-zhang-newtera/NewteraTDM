/*
* @(#)CurrentSiteMapNamePropertyEditor.cs
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
	/// A DropDown UI editor for the CurrentSiteMapName property of SiteMapModelSet in
	/// the namespace of Newtera.Common.MetaData.SiteMap.
	/// </summary>
	/// <version>  1.0.1 18 Jul 2012</version>
	public class CurrentSiteMapNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the CurrentSiteMapNamePropertyEditor class.
		/// </summary>
		public CurrentSiteMapNamePropertyEditor() : base()
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
                    SiteMapModelSet siteMapModelSet = null;
                    if (context.Instance is Newtera.Common.MetaData.SiteMap.SiteMapModelSet)
					{
                        siteMapModelSet = (Newtera.Common.MetaData.SiteMap.SiteMapModelSet)context.Instance;

						// display the available sitemap models in the dropdown list
                        if (siteMapModelSet.ChildNodes != null &&
                            siteMapModelSet.ChildNodes.Count > 0)
                        {
                            string[] captions = new string[siteMapModelSet.ChildNodes.Count];
                            int index = 0;
                            foreach (SiteMapModel siteMapModel in siteMapModelSet.ChildNodes)
                            {
                                captions[index++] = siteMapModel.Title;
                            }
                            listPicker.DataSource = captions;
                        }
					}

					editorService.DropDownControl(listPicker);

                    if (siteMapModelSet != null &&
                        siteMapModelSet.ChildNodes != null &&
                        siteMapModelSet.ChildNodes.Count > 0 &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < siteMapModelSet.ChildNodes.Count)
					{
                        return siteMapModelSet.ChildNodes[listPicker.SelectedIndex].Name;
					}
					else
					{
						return "";
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
/*
* @(#)SideMenuPathPropertyEditor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

    using Newtera.WinClientCommon;
	using Newtera.Common.MetaData.SiteMap;
	
	/// <summary>
	/// A Modal UI editor for the SideMenuPath property of SiteMapNode in
	/// the namespace of Newtera.Common.MetaData.SiteMap.
	/// </summary>
	/// <version>  1.0.1 18 Jun 2009</version>
	public class SideMenuPathPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the SideMenuPathPropertyEditor class.
		/// </summary>
		public SideMenuPathPropertyEditor() : base()
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
		/// Override the method to launch a ChooseConstraintDialog modal dialog
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen constraints</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
                // Access the property grid's UI display service
                IWindowsFormsEditorService editorService =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (editorService != null)
                {
                    // Create an instance of UI editor control
                    DropDownListControl listPicker = new DropDownListControl(editorService);
                    SiteMapNodeCollection menuGroups = null;
                    ISiteMapNode siteMapNode = null;

                    if (context.Instance is ISiteMapNode)
                    {
                        siteMapNode = (ISiteMapNode)context.Instance;

                        menuGroups = GetMenuGroups();

                        // display the menu group names in the dropdown list
                        if (menuGroups != null)
                        {
                            listPicker.DataSource = menuGroups;
                            listPicker.DisplayMember = "Title";
                        }
                    }

                    editorService.DropDownControl(listPicker);
                    string converted;
                    if (siteMapNode != null && menuGroups != null &&
                        listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < menuGroups.Count)
                    {
                        converted = @"/SideMenu/SideMenuGroup[@Name='" + menuGroups[listPicker.SelectedIndex].Name + "']/*";

                        return converted;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

			return base.EditValue(context, provider, value);
		}

        private SiteMapNodeCollection GetMenuGroups()
        {
            SideMenu sideMenu = SiteMapModelManager.Instance.SelectedSiteMapModel.SideMenu;

            return sideMenu.ChildNodes;
        }
	}
}
/*
* @(#)DatabasePropertyEditor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
    using System.Threading;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

	using Newtera.WinClientCommon;
    using Newtera.Common.Core;
	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.SiteMap;
	
	/// <summary>
	/// A DropDown UI editor for the Database property of MenuGroup in
	/// the namespace of Newtera.Common.MetaData.SiteMap.
	/// </summary>
	/// <version>  1.0.1 18 Jun 2009</version>
	public class DatabasePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the DatabasePropertyEditor class.
		/// </summary>
		public DatabasePropertyEditor() : base()
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
                    string[] schemaIds = null;
                    Newtera.Common.Core.SchemaInfo[] schemaInfos = null;
                    ISiteMapNode siteMapNode = null;

                    if (context.Instance is ISiteMapNode)
					{
                        siteMapNode = (ISiteMapNode)context.Instance;

                        // Get the authorized schema infos
                        schemaInfos = GetSchemaInfos();

						// display the schemas in the dropdown list
                        if (schemaInfos != null)
                        {
                            schemaIds = new string[schemaInfos.Length];
                            int index = 0;
                            foreach (Newtera.Common.Core.SchemaInfo schemaInfo in schemaInfos)
                            {
                                schemaIds[index++] = schemaInfo.NameAndVersion;
                            }

                            listPicker.DataSource = schemaIds;
                        }
					}

					editorService.DropDownControl(listPicker);

                    if (siteMapNode != null && schemaInfos != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < schemaInfos.Length)
					{
                        return schemaIds[listPicker.SelectedIndex];
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
        /// Gets the schema infos that the current user is authroized to access
        /// </summary>
        /// <returns></returns>
        private Newtera.Common.Core.SchemaInfo[] GetSchemaInfos()
        {
            Newtera.Common.Core.SchemaInfo[] schemaInfos = null;

            Cursor preCursor = Cursor.Current;

            // gets schema infos from the server
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                MetaDataServiceStub metaDataService = new MetaDataServiceStub();
                schemaInfos = metaDataService.GetSchemaInfos();
            }
            finally
            {
                Cursor.Current = preCursor;
            }

            return schemaInfos;
        }
	}
}
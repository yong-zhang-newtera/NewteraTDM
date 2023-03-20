/*
* @(#)SchemaIdPropertyEditor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
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
    using Newtera.Common.MetaData.Principal;
	using Newtera.Activities;
    using Newtera.WFModel;
	
	/// <summary>
	/// A DropDown UI editor for the SchemaId property of HandleNewteraEventActivity in
	/// the namespace of Newtear.Activities.
	/// </summary>
	/// <version>  1.0.1 02 Jan 2006</version>
	public class SchemaIdPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the SchemaIdPropertyEditor class.
		/// </summary>
		public SchemaIdPropertyEditor() : base()
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
                    INewteraWorkflow activity = null;
                    string[] schemaIds = null;
                    Newtera.Common.Core.SchemaInfo[] schemaInfos = null;

                    if (context.Instance is INewteraWorkflow)
					{
                        activity = (INewteraWorkflow)context.Instance;

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

                    if (activity != null && schemaInfos != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < schemaInfos.Length)
					{
                        if (MetaDataStore.Instance.GetMetaData(schemaInfos[listPicker.SelectedIndex]) == null)
                        {
                            MetaDataStore.Instance.PutMetaData(GetMetaData(schemaInfos[listPicker.SelectedIndex]));
                        }

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
            bool isAuthenticated = false;

            Newtera.Common.Core.SchemaInfo[] schemaInfos = null;

            // make sure the current user has been authenticated, if not, show the login
            // dialog
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                // not autheticated yet, ask user to login
                UserLoginDialog loginDialog = new UserLoginDialog();
                if (loginDialog.ShowDialog() == DialogResult.OK)
                {
                    isAuthenticated = true;
                }
            }
            else
            {
                isAuthenticated = true;
            }

            if (isAuthenticated)
            {
                Cursor preCursor = Cursor.Current;

                // gets schema infos from the server
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    ActivityValidateService service = new ActivityValidateService();
                    schemaInfos = service.GetAuthorizedSchemaInfos();
                }
                finally
                {
                    Cursor.Current = preCursor;
                }
            }

            return schemaInfos;
        }

        /// <summary>
        /// Gets the meta data of the selected schema
        /// </summary>
        /// <param name="schemaInfo"></param>
        /// <returns>A MetaDataModel instance.</returns>
        private MetaDataModel GetMetaData(Newtera.Common.Core.SchemaInfo schemaInfo)
        {
            Cursor preCursor = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ActivityValidateService service = new ActivityValidateService();

                return service.GetMetaData(schemaInfo);
            }
            finally
            {
                Cursor.Current = preCursor;
            }
        }
	}
}
/*
* @(#)ClassNamePropertyEditor.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
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
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.SiteMap;
	
	/// <summary>
	/// A Modal UI editor for the Class Name property of CustomCommandGroup in
	/// the namespace of Newtear.Coomon.MetaData.SiteMap
	/// </summary>
	/// <version>  1.0.1 27 Nov 2010</version>
	public class ClassNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ClassNamePropertyEditor class.
		/// </summary>
		public ClassNamePropertyEditor() : base()
		{
		}

		/// <summary> 
		/// Overrides the inherited method to return a Modal style
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				return UITypeEditorEditStyle.Modal;
			}

			return base.GetEditStyle(context);
		}

		/// <summary>
		/// Override the method to launch a ChooseClassDialog modal dialog
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
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (editorService != null)
				{
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
                            // Create an instance of ChooseClassDialog
                            ChooseClassDialog dialog = new ChooseClassDialog();

                            dialog.MetaData = metaData;

                            // set the current selected class if any
                            if (value != null)
                            {
                                ClassElement selectedClass = dialog.MetaData.SchemaModel.FindClass((string)value);
                                dialog.SelectedClass = selectedClass;
                            }

                            // show all classes
                            dialog.RootClass = "ALL";

                            // Display the dialog
                            if (editorService.ShowDialog(dialog) == DialogResult.OK)
                            {
                                // make sure that class name is used only once
                                string className = dialog.SelectedClassName;
                                if (context.Instance is CustomCommandGroup)
                                {
                                    CustomCommandGroup customCommandGroup = (CustomCommandGroup)context.Instance;
                                    if (!IsClassNameExist(className, customCommandGroup, (CustomCommandSet)customCommandGroup.ParentNode))
                                    {
                                        return className;
                                    }
                                    else
                                    {
                                        MessageBox.Show(MessageResourceManager.GetString("SiteMapStudioApp.InvalidClassName"), "Error Dialog", MessageBoxButtons.OK,
                                                        MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    return className;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("SiteMapStudioApp.SelectDatabase"), "Error Dialog", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}

        private bool IsClassNameExist(string className, CustomCommandGroup currentCustomGroup, CustomCommandSet customCommandSet)
        {
            bool status = false;

            foreach (CustomCommandGroup customGroup in customCommandSet.ChildNodes)
            {
                if (customGroup != currentCustomGroup &&
                    customGroup.Database == currentCustomGroup.Database &&
                    customGroup.ClassName == className)
                {
                    status = true;
                    break;
                }
            }

            return status;
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
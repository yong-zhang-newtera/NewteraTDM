/*
* @(#)ClassNamePropertyEditor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

    using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
	/// A Modal UI editor for the Class Name property of HandleNewteraEventActivity in
	/// the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 02 Jan 2007</version>
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
		/// <returns>The chosen class name</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (editorService != null)
				{
					// Create an instance of ChooseClassDialog
					ChooseClassDialog dialog = new ChooseClassDialog();

					// Set the meta data model to the dialog
                    INewteraWorkflow activity = (INewteraWorkflow)context.Instance;
                    if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(activity.SchemaId))
                    {
                        MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(activity.SchemaId);

                        dialog.MetaData = metaData;
                        dialog.RootClass = "ALL";

                        // set the current selected class if any
                        if (value != null)
                        {
                            ClassElement selectedClass = dialog.MetaData.SchemaModel.FindClass((string)value);
                            dialog.SelectedClass = selectedClass;

                        }

                        // Display the dialog
                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            if (dialog.SelectedClass.IsLeaf)
                            {
                                return dialog.SelectedClassName;
                            }
                            else
                            {
                                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.BindLeafClass"),
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.SetSchemaIdFirst"),
                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
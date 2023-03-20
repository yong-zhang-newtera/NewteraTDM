/*
* @(#)InsertStatementPropertyEditor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
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
	using Newtera.Common.MetaData.DataView;
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
	/// A Modal UI editor for the InsertStatement property of CreateDataInstanceActivity in
	/// the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 09 Feb 2007</version>
	public class InsertStatementPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the InsertStatementPropertyEditor class.
		/// </summary>
		public InsertStatementPropertyEditor() : base()
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
		/// Override the method to launch a InsertStatement modal dialog
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
					// Create an EditDataInstanceDialog to specify the data instance to be inserted
                    EditDataInstanceDialog dialog = new EditDataInstanceDialog();

                    CreateDataInstanceActivity activity = (CreateDataInstanceActivity)context.Instance;
                    INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                    if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(rootActivity.SchemaId))
                    {
                        if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(rootActivity.SchemaId, rootActivity.ClassName))
                        {
                            MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(rootActivity.SchemaId);
                  
                            DataViewModel instanceDataView = metaData.GetDetailedDataView(rootActivity.ClassName);
                            InstanceView instanceView = new InstanceView(instanceDataView);
                            dialog.InstanceView = instanceView;
                            dialog.ExistingInsertStatement = activity.InsertStatement;

                            // Display the dialog
                            if (editorService.ShowDialog(dialog) == DialogResult.OK)
                            {
                                return instanceDataView.InsertQuery; ;
                            }
                        }
                        else
                        {
                            MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.SetClassNameFirst"),
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
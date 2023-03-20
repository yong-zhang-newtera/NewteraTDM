/*
* @(#)FormPropertiesPropertyEditor.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
    using System.Collections.Specialized;
	using System.Windows.Forms.Design;

    using Newtera.WinClientCommon;
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
     using Newtera.WFModel;
	
	/// <summary>
	/// A Modal UI editor for the FormProperties property of CreateTaskActivity in
	/// the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 17 May 2010</version>
	public class FormPropertiesPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the FormPropertiesPropertyEditor class.
		/// </summary>
		public FormPropertiesPropertyEditor() : base()
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
		/// Override the method to launch a SelectPropertiesDialog modal dialog
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
                    // Create an instance of SelectPropertiesDialog
                    SelectPropertiesDialog dialog = new SelectPropertiesDialog();
                    StringCollection formProperties = null;
                    INewteraWorkflow rootActivity = null;

                    CreateTaskActivity taskActivity = context.Instance as CreateTaskActivity;
                    CreateGroupTaskActivity groupTaskActivity = context.Instance as CreateGroupTaskActivity;
                    if (taskActivity != null)
                    {
                        formProperties = taskActivity.FormProperties;
                        rootActivity = ActivityUtil.GetRootActivity(taskActivity);
                    }
                    else if (groupTaskActivity != null)
                    {
                        formProperties = groupTaskActivity.FormProperties;
                        rootActivity = ActivityUtil.GetRootActivity(groupTaskActivity);
                    }

                    if (rootActivity != null &&
                        ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(rootActivity.SchemaId))
                     {
                         if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(rootActivity.SchemaId, rootActivity.ClassName))
                         {
                             if (formProperties != null)
                             {
                                 dialog.FormProperties = formProperties;
                             }

                             // Display the dialog
                             if (editorService.ShowDialog(dialog) == DialogResult.OK)
                             {
                                 return dialog.FormProperties;
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
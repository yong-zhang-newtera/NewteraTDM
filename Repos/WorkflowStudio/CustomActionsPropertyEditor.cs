/*
* @(#)CustomActionsPropertyEditor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
    using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
    using System.Workflow.ComponentModel;

    using Newtera.WinClientCommon;
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
    /// A Modal UI editor for the CustomActions property of CreateTaskActivity or
    /// in the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 26 Nov 2007</version>
	public class CustomActionsPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the CustomActionsPropertyEditor class.
		/// </summary>
		public CustomActionsPropertyEditor() : base()
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
		/// Override the method to launch a DefineCustomActionDialog modal dialog
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
                    if (context.Instance is CreateTaskActivity ||
                        context.Instance is CreateGroupTaskActivity)
                    {
                        Activity activity = (Activity)context.Instance;
                        INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                        if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(rootActivity.SchemaId))
                        {
                            if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(rootActivity.SchemaId, rootActivity.ClassName))
                            {
                                // Create an instance of DefineCustomActionDialog
                                DefineCustomActionDialog dialog = new DefineCustomActionDialog();
                                dialog.BindingSchemaId = rootActivity.SchemaId;
                                dialog.BindingClassName = rootActivity.ClassName;

                                if (value != null)
                                {
                                    // creat a copy of CustomActionCollection
                                    CustomActionCollection customActions = new CustomActionCollection();
                                    foreach (CustomAction customAction in ((CustomActionCollection)value))
                                    {
                                        customActions.Add(customAction.Clone());
                                    }

                                    dialog.CustomActions = customActions;
                                }

                                // Display the dialog
                                if (editorService.ShowDialog(dialog) == DialogResult.OK)
                                {
                                    return dialog.CustomActions;
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
			}

			return base.EditValue(context, provider, value);
		}
	}
}
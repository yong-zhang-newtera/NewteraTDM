/*
* @(#)ActionCodeEditor.cs
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
    /// A Modal UI editor for the ActionCode of CustomAction in the namespace of Newtear.WFModel
	/// </summary>
	/// <version>  1.0.0 27 Aug 2008</version>
	public class ActionCodeEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ActionCodeEditor class.
		/// </summary>
		public ActionCodeEditor() : base()
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
		/// Override the method to launch a ActionCodeEditDialog modal dialog
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
                    if (context.Instance is CustomAction)
                    {
                        CustomAction customAction = (CustomAction)context.Instance;

                        // Create an instance of ActionCodeEditDialog
                        ActionCodeEditDialog dialog = new ActionCodeEditDialog();

                        dialog.BindingSchemaId = customAction.SchemaId;
                        dialog.BindingClassName = customAction.ClassName;

                        if (value != null)
                        {
                            dialog.ActionCode = (string)value;
                        }

                        // Display the dialog
                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            return dialog.ActionCode;
                        }
                    }
                }
			}

			return base.EditValue(context, provider, value);
		}
	}
}
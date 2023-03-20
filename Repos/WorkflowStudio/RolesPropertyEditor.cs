/*
* @(#)RolesPropertyEditor.cs
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
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
	
	/// <summary>
	/// A Modal UI editor for the Roles property of CreateTaskActivity in
	/// the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 10 Jan 2007</version>
	public class RolesPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the RolesPropertyEditor class.
		/// </summary>
		public RolesPropertyEditor() : base()
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
		/// Override the method to launch a SelectRolesDialog modal dialog
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
                    // Create an instance of SelectRolesDialog
                    SelectRolesDialog dialog = new SelectRolesDialog();

                    CreateTaskActivity activity = (CreateTaskActivity)context.Instance;

                    if (activity.Roles != null)
                    {
                        dialog.Roles = activity.Roles;
                    }

                    // Display the dialog
                    if (editorService.ShowDialog(dialog) == DialogResult.OK)
                    {
                        return dialog.Roles;
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
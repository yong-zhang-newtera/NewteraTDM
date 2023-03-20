/*
* @(#)UsersPropertyEditor.cs
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

    using Newtera.WFModel;
    using Newtera.WinClientCommon;
    using Newtera.WorkflowStudioControl;
	
	/// <summary>
	/// A Modal UI editor for the SubstituteUsers property of SubstituteEntry in
	/// the namespace of Newtear.WFModel
	/// </summary>
	/// <version>  1.0.0 27 OCT 2008</version>
	public class SubstituteUsersPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the SubstituteUsersPropertyEditor class.
		/// </summary>
        public SubstituteUsersPropertyEditor()
            : base()
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
		/// Override the method to launch a SelectUsersDialog modal dialog
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
                    // Create an instance of SelectUsersDialog
                    SelectUsersDialog dialog = new SelectUsersDialog();

                    SubstituteEntry substitute = (SubstituteEntry)context.Instance;

                    if (substitute.SubstituteUsers != null)
                    {
                        dialog.Users = substitute.SubstituteUsers;
                    }

                    // Display the dialog
                    if (editorService.ShowDialog(dialog) == DialogResult.OK)
                    {
                        return dialog.Users;
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
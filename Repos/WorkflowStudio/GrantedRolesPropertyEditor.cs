/*
* @(#)GrantedRolesPropertyEditor.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
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
    using Newtera.WFModel;
    using Newtera.WorkflowStudioControl;
	
	/// <summary>
	/// A Modal UI editor for the GrantedRoles property of CustomAction in
	/// the namespace of Newtear.WFModel
	/// </summary>
	/// <version>  1.0.0 28 April 2015</version>
	public class GrantedRolesPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the GrantedRolesPropertyEditor class.
		/// </summary>
		public GrantedRolesPropertyEditor() : base()
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
                    dialog.IsLogicalAnd = false;

                    CustomAction customAction = (CustomAction)context.Instance;

                    if (customAction.GrantedRoles != null)
                    {
                        dialog.Roles = customAction.GrantedRoles;
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
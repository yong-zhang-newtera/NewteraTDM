/*
* @(#)RolesPropertyEditor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

    using Newtera.WinClientCommon;
    using Newtera.Common.MetaData.Subscribers;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Modal UI editor for the Roles property of Subscriber in
	/// the namespace of Newtear.Common.MetaData.Subscribers
	/// </summary>
	/// <version>  1.0.0 17 Sep 2013</version>
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

                    Subscriber subscriber = (Subscriber)context.Instance;

                    if (subscriber.Roles != null)
                    {
                        dialog.Roles = subscriber.Roles;
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
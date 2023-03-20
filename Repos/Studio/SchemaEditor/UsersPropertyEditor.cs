/*
* @(#)UsersPropertyEditor.cs
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
	/// A Modal UI editor for the Users property of Subscriber in
	/// the namespace of Newtear.Common.MetaData.Subscribers
	/// </summary>
	/// <version>  1.0.0 17 Sep 2013</version>
	public class UsersPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the UsersPropertyEditor class.
		/// </summary>
		public UsersPropertyEditor() : base()
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

                    Subscriber subscriber = (Subscriber)context.Instance;

                    if (subscriber.Users != null)
                    {
                        dialog.Users = subscriber.Users;
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
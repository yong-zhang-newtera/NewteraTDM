/*
* @(#)PrimaryKeysPropertyEditor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

	using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Modal UI editor for the PrimaryKeys property of ClassElement in
	/// the namespace of MetaData.Schema.
	/// </summary>
	/// <version>  1.0.1 29 Sept 2003</version>
	/// <author> Yong Zhang</author>
	public class PrimaryKeysPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ConstraintPropertyEditor class.
		/// </summary>
		public PrimaryKeysPropertyEditor() : base()
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
		/// Override the method to launch a ChoosePrimaryKeysDialog modal dialog
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen constraints</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				ClassElement theClass = (ClassElement) context.Instance;

				if (editorService != null)
				{
					// only root class allows to define primary keys
					if (theClass.IsRoot)
					{
						// Create an instance of ChoosePrimaryKeysDialog
						ChoosePrimaryKeysDialog dialog = new ChoosePrimaryKeysDialog();

						// Set the classElement to the dialog
						dialog.ClassElement = theClass;

						// Display the dialog
						if (editorService.ShowDialog(dialog) == DialogResult.OK)
						{
							return dialog.PrimaryKeys;
						}
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.InvalidPKClass"));
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

	}
}
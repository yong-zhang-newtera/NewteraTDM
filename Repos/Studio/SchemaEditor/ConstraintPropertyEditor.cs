/*
* @(#)ConstraintPropertyEditor.cs
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
	
	/// <summary>
	/// A Modal UI editor for the Constraint property of SimpleAttributeElement in
	/// the namespace of MetaData.Schema.
	/// </summary>
	/// <version>  1.0.1 29 Sept 2003</version>
	/// <author> Yong Zhang</author>
	public class ConstraintPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ConstraintPropertyEditor class.
		/// </summary>
		public ConstraintPropertyEditor() : base()
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
		/// Override the method to launch a ChooseConstraintDialog modal dialog
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

				if (editorService != null)
				{
					// Create an instance of ChooseConstraintDialog
					ChooseConstraintDialog dialog = new ChooseConstraintDialog();

					// Set the schema model to the dialog
					dialog.SchemaModel = ((SchemaModelElement) context.Instance).SchemaModel;

					// set the current selected constraint if any
					if (value != null)
					{
						dialog.SelectedConstraint = (ConstraintElementBase) value;
					}

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
						return dialog.SelectedConstraint;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

	}
}
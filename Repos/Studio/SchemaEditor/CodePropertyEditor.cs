/*
* @(#)CodePropertyEditor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
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
	/// A Modal UI editor for the Code property of VirtualAttributeElement in
	/// the namespace of Newtera.Common.MetaData.Schema.
	/// </summary>
	/// <version>  1.0.1 26 May 2007</version>
	public class CodePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the CodePropertyEditor class.
		/// </summary>
		public CodePropertyEditor() : base()
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
		/// Override the method to launch a WriteVirtualAttributeCodeDialog modal dialog
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The the C# code for calculatig value of a virtual attribute</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (editorService != null)
				{
                    // Create an instance of WriteVirtualAttributeCodeDialog
                    WriteVirtualAttributeCodeDialog dialog = new WriteVirtualAttributeCodeDialog();

                    VirtualAttributeElement attribute = (VirtualAttributeElement)context.Instance;
					
					// Set the data view model to the dialog
                    dialog.Attribute = attribute;

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
						return dialog.Code;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
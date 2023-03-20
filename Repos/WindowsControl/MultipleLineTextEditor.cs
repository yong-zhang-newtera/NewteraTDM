/*
* @(#)MultipleLineTextEditor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A drop-down UI editor for the value of SimpleAttribute with multiple lines.
	/// </summary>
	/// <version>  1.0.0 28 Jul. 2004</version>
	/// <author> Yong Zhang</author>
	public class MultipleLineTextEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the MultipleLineTextEditor class.
		/// </summary>
		public MultipleLineTextEditor() : base()
		{
		}

		/// <summary> 
		/// Overrides the inherited method to return a drop-down style
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}

			return base.GetEditStyle(context);
		}

		/// <summary>
		/// Override the method to show a drop-down editor with multiple choices
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen constraints</returns>
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (editorService != null)
				{
					// create an instance of UI editor control
					MultipleLineTextControl dropDownEditor = new MultipleLineTextControl(editorService,
						context.PropertyDescriptor.PropertyType);

					// pass the UI editor control the current property value
					dropDownEditor.PropertyText = (string) value;

					// display the UI editor control
					editorService.DropDownControl(dropDownEditor);

					// return the new property value from the UI editor control
					return dropDownEditor.PropertyText;
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
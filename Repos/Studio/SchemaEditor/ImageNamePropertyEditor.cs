/*
* @(#)ImageNamePropertyEditor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
	
	/// <summary>
	/// A Model UI editor for the Image Name property of ClassElement in
	/// the namespace of Newtear.Common.MetaData.Schema
	/// </summary>
	/// <version>  1.0.1 19 Jul 2004</version>
	/// <author> Yong Zhang</author>
	public class ImageNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ImageNamePropertyEditor class.
		/// </summary>
		public ImageNamePropertyEditor() : base()
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
		/// Override the method to launch a ChooseFileDialog modal dialog
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen image file name</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (editorService != null)
				{
					OpenFileDialog dialog = new OpenFileDialog();
					dialog.InitialDirectory = "c:\\" ;
					dialog.FilterIndex = 1 ;
					dialog.RestoreDirectory = false ;

					// Display the dialog
					if (dialog.ShowDialog() == DialogResult.OK)
					{
						// only need the file name, not a path
						string fileName = dialog.FileName;
						int pos = fileName.LastIndexOf(@"\");
						if (pos >= 0)
						{
							fileName = fileName.Substring(pos + 1);
						}

						return fileName;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
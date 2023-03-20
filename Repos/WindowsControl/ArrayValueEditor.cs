/*
* @(#)ArrayValueEditor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Data;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A Modal UI editor for the value of DataRelationshipAttribute in
	/// a Instance View.
	/// </summary>
	/// <version>  1.0.1 15 Nov. 2003</version>
	/// <author> Yong Zhang</author>
	public class ArrayValueEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ArrayValueEditor class.
		/// </summary>
		public ArrayValueEditor() : base()
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
		/// Override the method to launch a EditArrayValueDialog modal dialog
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
					string arrayAttributeName = context.PropertyDescriptor.Name;
					InstanceView instanceView = (InstanceView) context.Instance;

					// Create an instance of EditArrayValueDialog
					EditArrayValueDialog dialog = new EditArrayValueDialog();

					// pass the UI editor control the current property value
					dialog.DataTable = ((ArrayDataTableView) value).ArrayAttributeValue;

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
						// The property grid expects a ArrayDataTableView as
						// an value, therefore, create a new ArrayDataTavleView instance. 
						ArrayDataTableView newValue = new ArrayDataTableView(arrayAttributeName,
							instanceView.InstanceData);

						// This will set the updated value in the DataTable from
						// the dialog to the instanceData
						newValue.ArrayAttributeValue = dialog.DataTable;

						return newValue;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
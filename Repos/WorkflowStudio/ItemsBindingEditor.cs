/*
* @(#)ItemsBindingEditor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
    using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
    using System.Workflow.ComponentModel;

    using Newtera.WinClientCommon;
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
    /// A Modal UI editor for the ItemsBinding property of ForEachActivity in the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 20 Nov 2007</version>
	public class ItemsBindingEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ItemsBindingEditor class.
		/// </summary>
		public ItemsBindingEditor() : base()
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
		/// Override the method to launch a ParameterBindingsDialog modal dialog
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
                    // Create an instance of SelectParameterBindingDialog
                    SelectParameterBindingDialog dialog = new SelectParameterBindingDialog();

                    if (value != null)
                    {
                        dialog.ParameterBinding = ((ParameterBindingInfo)value).Clone();
                    }
                    else
                    {
                        dialog.ParameterBinding = new ParameterBindingInfo();
                    }

                    // expecting the parameter type is List
                    dialog.DataType = ParameterDataType.Array;

                    // Display the dialog
                    if (editorService.ShowDialog(dialog) == DialogResult.OK)
                    {
                        return dialog.ParameterBinding;
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
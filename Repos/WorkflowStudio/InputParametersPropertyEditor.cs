/*
* @(#)InputParametersPropertyEditor.cs
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

    using Newtera.WinClientCommon;
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
    /// A Modal UI editor for the InputParameters property of NewteraSequentialWorkflowActivity or
    /// NewteraStateMachineWorkflowActivity in the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 15 Nov 2007</version>
	public class InputParametersPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the InputParametersPropertyEditor class.
		/// </summary>
		public InputParametersPropertyEditor() : base()
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
		/// Override the method to launch a DefineInputParameterDialog modal dialog
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
					// Create an instance of DefineInputParameterDialog
					DefineInputParameterDialog dialog = new DefineInputParameterDialog();

                    if (value != null)
                    {
                        // creat a copy of input parameter list
                        ArrayList inputParameters = new ArrayList();
                        foreach (InputParameter param in ((IList)value))
                        {
                            inputParameters.Add(param.Clone());
                        }

                        dialog.InputParameters = inputParameters;
                    }

                    // Display the dialog
                    if (editorService.ShowDialog(dialog) == DialogResult.OK)
                    {
                        return dialog.InputParameters;
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
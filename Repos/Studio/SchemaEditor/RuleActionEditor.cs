/*
* @(#)RuleActionEditor.cs
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

    using Newtera.WinClientCommon;
	using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Rules;
	
	/// <summary>
	/// A DropDown UI editor for the ThenAction and ElseAction properties of RuleDef class in
	/// the namespace of Newtear.Common.MetaData.Rules
	/// </summary>
	/// <version>  1.0.0 16 Oct 2007</version>
	public class RuleActionEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the RuleActionEditor class.
		/// </summary>
		public RuleActionEditor() : base()
		{
		}

		/// <summary> 
        /// Overrides the inherited method to return a DropDown style
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
		/// Override the method to show a drop-down list of functions available for rule actions
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
					// Create an instance of UI editor control
					DropDownListControl listPicker = new DropDownListControl(editorService);

                    if (context.Instance is RuleDef)
                    {
                        RuleDef ruleDef = (RuleDef)context.Instance;

                        listPicker.DataSource = FunctionFactory.ACTION_FUNCTIONS;

                        editorService.DropDownControl(listPicker);

                        if (listPicker.SelectedIndex >= 0 &&
                            listPicker.SelectedIndex < FunctionFactory.ACTION_FUNCTIONS.Length)
                        {
                            string functionName = FunctionFactory.ACTION_FUNCTIONS[listPicker.SelectedIndex];
                            return FunctionFactory.Instance.Create(functionName);
                        }
                        else
                        {
                            return null;
                        }
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
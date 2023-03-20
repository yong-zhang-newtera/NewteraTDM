/*
* @(#)RuleConditionEditor.cs
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

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Rules;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Modal UI editor for the Condition, ThenAction, and ElseAction properties of
    /// RuleDef in the namespace of Newtera.Common.MetaData.Rules.
	/// </summary>
	/// <version>  1.0.0 13 Oct 2007</version>
	public class RuleConditionEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the RuleConditionEditor class.
		/// </summary>
		public RuleConditionEditor() : base()
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
		/// Override the method to launch a CreateSearchExprDialog modal dialog
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
					// Create an instance of CreateSearchExprDialog
					CreateSearchExprDialog dialog = new CreateSearchExprDialog();
                    dialog.EnableReferences = false; // do not support using referenced classes in a rule for now

					// get the data view associated with the RuleDef object
                    RuleDef ruleDef = (RuleDef)context.Instance;
                    DataViewModel dataView = ruleDef.DataView;
					
					if (dataView != null)
					{
						// set the data view's search filter to the local search filter
						// because we only want to edit the search expression local to
						// the current taxon node
						dataView.ClearSearchExpression();
						// set the current search expression if any
						if (value != null)
						{
							dataView.AddSearchExpr((IDataViewElement) value,
								ElementType.And);
						}

						// Set the data view model to the dialog
						dialog.DataView = dataView;

						// Display the dialog
						if (editorService.ShowDialog(dialog) == DialogResult.OK)
						{
							// save the referenced class used by the local search expression
							// to the TaxonNode
							//UpdateReferencedClasses(taxon, dataView.ReferencedClasses);

							return dialog.DataView.FilterExpr;
						}
					}
					else
					{
                        throw new Exception("Unable to find a data view for class " + ruleDef.ClassName);
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
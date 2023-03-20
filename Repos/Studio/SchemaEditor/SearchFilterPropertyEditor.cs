/*
* @(#)SearchFilterPropertyEditor.cs
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

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.DataView.Taxonomy;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Modal UI editor for the SearchFilter property of TaxonNode in
	/// the namespace of Newtera.Common.MetaData.DataView.Taxonomy.
	/// </summary>
	/// <version>  1.0.1 15 Feb 2004</version>
	/// <author> Yong Zhang</author>
	public class SearchFilterPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the SearchFilterPropertyEditor class.
		/// </summary>
		public SearchFilterPropertyEditor() : base()
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

					// get the data view created by the TaxonNode
					TaxonNode taxon = (TaxonNode) context.Instance;
                    DataViewModel dataView = taxon.GetDataView(null);
					
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
							UpdateReferencedClasses(taxon, dataView.ReferencedClasses);

							return dialog.DataView.FilterExpr;
						}
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.MissingClassSpec"),
							"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

		/// <summary>
		/// Add newly added or delete referenced classes to the TaxonNode object
		/// </summary>
		/// <param name="taxon">The TaxonNode object</param>
		/// <param name="updatedClasses">The updated list of referenced classes.</param>
		private void UpdateReferencedClasses(TaxonNode taxon, ReferencedClassCollection updatedClasses)
		{
            taxon.ReferencedClasses.Clear();

			// add the new referenced classes to taxon node
            foreach (DataClass refClass in updatedClasses)
			{
                taxon.ReferencedClasses.Add(refClass);
			}
		}
	}
}
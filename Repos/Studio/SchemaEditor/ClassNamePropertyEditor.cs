/*
* @(#)ClassNamePropertyEditor.cs
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

    using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView.Taxonomy;
	
	/// <summary>
	/// A Modal UI editor for the Class Name property of TaxonomyModel and TaxonNode in
	/// the namespace of Newtear.Coomon.MetaData.DataView.Taxonomy
	/// </summary>
	/// <version>  1.0.1 15 Feb 2004</version>
	/// <author> Yong Zhang</author>
	public class ClassNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ClassNamePropertyEditor class.
		/// </summary>
		public ClassNamePropertyEditor() : base()
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
		/// Override the method to launch a ChooseClassDialog modal dialog
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
					// Create an instance of ChooseClassDialog
					ChooseClassDialog dialog = new ChooseClassDialog();
                    bool found = false;

					// Set the meta data model to the dialog
                    ITaxonomy node = context.Instance as ITaxonomy;
                    SimpleAttributeElement attribute = context.Instance as SimpleAttributeElement;
                    if (node != null)
                    {
                        dialog.MetaData = node.MetaDataModel;
                        found = true;
                    }
                    else if (attribute != null)
                    {
                        dialog.MetaData = attribute.OwnerClass.SchemaModel.MetaData;
                        found = true;
                    }

					// set the current selected class if any
                    if (found && value != null)
					{
						ClassElement selectedClass = dialog.MetaData.SchemaModel.FindClass((string) value);
						dialog.SelectedClass = selectedClass;
					}

					// set the root class of a class subtree to be displayed
					if (context.Instance is TaxonNode)
					{
						// get the class defined for its nearest parent
						string rootClassName = null;
						ITaxonomy parent = ((ITaxonomy) context.Instance);
						while (rootClassName == null && parent != null)
						{
							rootClassName = parent.ClassName;
							parent = parent.ParentNode;
						}

						if (rootClassName != null && rootClassName.Length > 0)
						{
							dialog.RootClass = rootClassName;
						}
						else
						{
							dialog.RootClass = "ALL";
						}
					}
					else if (context.Instance is TaxonomyModel)
					{
						dialog.RootClass = "ALL";
					}
                    else if (context.Instance is SimpleAttributeElement)
                    {
                        dialog.RootClass = "ALL";
                    }

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
						return dialog.SelectedClassName;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
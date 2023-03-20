/*
* @(#)ClassCategoryPropertyEditor.cs
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
    using System.Collections.Specialized;
	using System.Windows.Forms.Design;

	using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Modal UI editor for the Category property of ClassElement in
	/// the namespace of MetaData.Schema.
	/// </summary>
	/// <version>  1.0.0 15 June 2007</version>
	public class ClassCategoryPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ConstraintPropertyEditor class.
		/// </summary>
		public ClassCategoryPropertyEditor() : base()
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
		/// Override the method to launch a ChooseCategoryDialog modal dialog
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The choosen category</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				ClassElement theClass = (ClassElement) context.Instance;

				if (editorService != null)
				{
					// Create an instance of ChoosePrimaryKeysDialog
                    ChooseCategoryDialog dialog = new ChooseCategoryDialog();

					// Set the existing categories to the dialog
                    StringCollection existingCategories = GetExistingCategories(theClass);
                    dialog.ExistingCategories = existingCategories;

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
						return dialog.Category;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        /// <summary>
        /// Gets the existing categories among the sibling classes.
        /// </summary>
        /// <param name="theClass">The class element</param>
        /// <returns>A collection of category names.</returns>
        private StringCollection GetExistingCategories(ClassElement theClass)
        {
            StringCollection existingCategories = new StringCollection();
            SchemaModelElementCollection classes;
            ClassElement parent = theClass.ParentClass;
            if (parent == null)
            {
                // it is a root class, gets all root classes from the SchemaModel
                classes = theClass.SchemaModel.RootClasses;
            }
            else
            {
                classes = parent.Subclasses;
            }

            if (classes != null)
            {
                foreach (ClassElement classElement in classes)
                {
                    if (!string.IsNullOrEmpty(classElement.Category))
                    {
                        bool found = false;
                        foreach (string category in existingCategories)
                        {
                            if (category == classElement.Category)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            existingCategories.Add(classElement.Category);
                        }
                    }
                }
            }

            return existingCategories;
        }
	}
}
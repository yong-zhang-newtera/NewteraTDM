/*
* @(#)CustomPageCollectionEditor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
	
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A Modal UI editor for editing CustomPages property of ClassElement. It inherites
	/// from UITypeEditor class.
	/// </summary>
	/// <version>  1.0.1 24 Jul 2009</version>
	public class CustomPageCollectionEditor : UITypeEditor
	{
		public CustomPageCollectionEditor() : base()
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
		/// Override the method to launch a DefineCustomPagesDialog modal dialog
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

                ClassElement classElement = (ClassElement)context.Instance;

				if (editorService != null)
				{
                    // Create an instance of DefineCustomPagesDialog
                    DefineCustomPagesDialog dialog = new DefineCustomPagesDialog();
                    dialog.ClassName = classElement.Name;
                    dialog.SchemaModel = classElement.SchemaModel;
                    dialog.CustomPages = GetClonedCollection(classElement.CustomPages);

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
                        return dialog.CustomPages;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        private SchemaModelElementCollection GetClonedCollection(SchemaModelElementCollection originalCollection)
        {
            SchemaModelElementCollection clonedCollection = new SchemaModelElementCollection();

            CustomPageElement cloned;
            foreach (CustomPageElement original in originalCollection)
            {
                cloned = new CustomPageElement(original.Name);
                cloned.Caption = original.Caption;
                cloned.URL = original.URL;
                cloned.QueryString = original.QueryString;
                cloned.Description = original.Description;
                cloned.RelatedClassName = original.RelatedClassName;
                cloned.MasetrClassName = original.MasetrClassName;
                cloned.VisibleCondition = original.VisibleCondition;
                cloned.SchemaModel = original.SchemaModel;
                clonedCollection.Add(cloned);
            }

            return clonedCollection;
        }
	}
}
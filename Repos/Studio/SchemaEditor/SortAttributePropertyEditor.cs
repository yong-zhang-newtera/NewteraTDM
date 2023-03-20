/*
* @(#)SortAttributePropertyEditor.cs
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
    using Newtera.WindowsControl;
    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A DropDown UI editor for the SortAttribute property of ClassElement in
	/// the namespace of Newtear.Coomon.MetaData.Schema
	/// </summary>
	/// <version>  1.0.0 10 Aug 2007</version>
	public class SortAttributePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the SortAttributePropertyEditor class.
		/// </summary>
		public SortAttributePropertyEditor() : base()
		{
		}

		/// <summary> 
		/// Overrides the inherited method to return a Modal style
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
		/// Override the method to show a pulldown menu
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

                    if (context.Instance is ClassElement)
                    {
                        ClassElement classElement = (ClassElement)context.Instance;

                        int count = 0;
                        foreach (SimpleAttributeElement attr in classElement.SimpleAttributes)
                        {
                            // attributes with Text type cannot be used for sort
                            if (attr.DataType != DataType.Text)
                            {
                                count++;
                            }
                        }

                        // get list of sortable attributes
                        count = count + 2;
                        string[] sortableAttributeCaptions = new string[count];
                        // default items
                        sortableAttributeCaptions[0] = NewteraNameSpace.NONE;
                        sortableAttributeCaptions[1] = MessageResourceManager.GetString("SchemaEditor.ObjId");
                        string[] sortableAttributeNames = new string[count];

                        sortableAttributeNames[0] = NewteraNameSpace.NONE;
                        sortableAttributeNames[1] = NewteraNameSpace.OBJ_ID;
                        int index = 2; // starts from position 2
                        foreach (SimpleAttributeElement attr in classElement.SimpleAttributes)
                        {
                            if (attr.DataType != DataType.Text)
                            {
                                sortableAttributeCaptions[index] = attr.Caption;
                                sortableAttributeNames[index++] = attr.Name;
                            }
                        }

                        listPicker.DataSource = sortableAttributeCaptions;

                        editorService.DropDownControl(listPicker);

                        if (listPicker.SelectedIndex >= 0 &&
                            listPicker.SelectedIndex < count)
                        {
                            return sortableAttributeNames[listPicker.SelectedIndex];
                        }
                        else
                        {
                            return "";
                        }
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
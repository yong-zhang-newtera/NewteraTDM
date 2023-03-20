/*
* @(#)CascadedAttributePropertyEditor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
    using System.Text;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

	using Newtera.WinClientCommon;
    using Newtera.WindowsControl;
    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A DropDown UI editor for the CascadedAttributes property of SimpleAttributeElement in
	/// the namespace of Newtear.Coomon.MetaData.Schema
	/// </summary>
	/// <version>  1.0.0 14 Nov 2009</version>
	public class CascadedAttributePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the CascadedAttributePropertyEditor class.
		/// </summary>
		public CascadedAttributePropertyEditor() : base()
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
					DropDownCheckListControl listPicker = new DropDownCheckListControl(editorService);

                    string[] checkedAttributes = new string[0];
                    if (value != null)
                    {
                        checkedAttributes = ((string)value).Split(';');
                    }

                    if (context.Instance is SimpleAttributeElement || context.Instance is RelationshipAttributeElement)
                    {
                        AttributeElementBase attributeElement = (AttributeElementBase)context.Instance;

                        ClassElement classElement = attributeElement.OwnerClass;

                        int count = 0;
                        foreach (SimpleAttributeElement attr in classElement.SimpleAttributes)
                        {
                            // attributes with Text type cannot be used as a cascaded attribute
                            if (attr.Name != attributeElement.Name &&
                                attr.DataType != DataType.Text)
                            {
                                count++;
                            }
                        }

                        // get list of sortable attributes
                        string[] attributeNames = new string[count];

                        int index = 0; // starts from position 0
                        bool isChecked;
                        foreach (SimpleAttributeElement attr in classElement.SimpleAttributes)
                        {
                            if (attr.Name != attributeElement.Name && 
                                attr.DataType != DataType.Text)
                            {
                                attributeNames[index++] = attr.Name;
                                isChecked = false;
                                foreach (string checkedAttribute in checkedAttributes)
                                {
                                    if (checkedAttribute == attr.Name)
                                    {
                                        isChecked = true;
                                        break;
                                    }
                                }

                                listPicker.AddItem(attr.Caption, isChecked);
                            }
                        }

                        editorService.DropDownControl(listPicker);

                        if (listPicker.SelectedIndices.Count >= 0 )
                        {
                            StringBuilder builder = new StringBuilder();
                            for (int i = 0; i < listPicker.SelectedIndices.Count; i++)
                            {
                                if (builder.Length > 0)
                                {
                                    builder.Append(";");
                                }

                                builder.Append(attributeNames[listPicker.SelectedIndices[i]]);
                            }
                            return builder.ToString();
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
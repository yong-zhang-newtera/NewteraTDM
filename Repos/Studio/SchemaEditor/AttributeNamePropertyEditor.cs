/*
* @(#)AttributeNamePropertyEditor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
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
    using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Subscribers;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A DropDown UI editor for the UsersBindingAttribute property of
    /// Subscriber in the namespace of Newtear.Common.MetaData.Subscribers.
	/// </summary>
	/// <version>  1.0.1 17 Sep 2013</version>
	public class AttributeNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the AttributeNamePropertyEditor class.
		/// </summary>
		public AttributeNamePropertyEditor() : base()
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
		/// Override the method to show a dropdown list of simple attribute names
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen attribute name</returns>
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
                    string[] attributeNames = null;
                    string[] attributeCaptions = null;

					if (context.Instance is Subscriber || context.Instance is SimpleAttributeElement)
					{
                        Subscriber subscriber = context.Instance as Subscriber;
                        SimpleAttributeElement simpleAttributeElement = context.Instance as SimpleAttributeElement;
                        string className = null;
                        MetaDataModel metaData;
                        if (subscriber != null)
                        {
                            className = subscriber.ClassName;
                            metaData = subscriber.MetaData;
                        }
                        else
                        {
                            className = simpleAttributeElement.OwnerClass.Name;
                            metaData = simpleAttributeElement.SchemaModel.MetaData;
                        }

                        DataViewModel dataView = metaData.GetDetailedDataView(className);
                        if (dataView != null)
                        {
                            ResultAttributeCollection attributes = dataView.ResultAttributes;

                            attributeNames = new string[attributes.Count];
                            attributeCaptions = new string[attributes.Count];
                            int index = 0;
                            foreach (IDataViewElement attr in attributes)
                            {
                                attributeNames[index] = attr.Name;
                                attributeCaptions[index++] = attr.Caption;
                            }

                            listPicker.DataSource = attributeCaptions;
                        }
					}

					editorService.DropDownControl(listPicker);

                    if (attributeNames != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < attributeNames.Length)
					{
						return attributeNames[listPicker.SelectedIndex];
					}
					else
					{
						return "";
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
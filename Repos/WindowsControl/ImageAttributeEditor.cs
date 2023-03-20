/*
* @(#)ImageAttributeEditor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Data;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A Modal UI editor for the value of ImageAttributeElement in
	/// a Instance View.
	/// </summary>
	/// <version>  1.0.1 07 Jul. 2008</version>
	public class ImageAttributeEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ImageAttributeEditor class.
		/// </summary>
		public ImageAttributeEditor() : base()
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
		/// Override the method to launch a EditArrayValueDialog modal dialog
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
                    InstanceView instanceView = (InstanceView)context.Instance;

					// Create an instance of EditImageValueDialog
                    EditImageValueDialog dialog = new EditImageValueDialog();

					// pass the UI editor control the current property value
                    if (value != null)
                    {
                        dialog.ImageId = (string)value;
                    }

                    string baseClassName = instanceView.DataView.BaseClass.ClassName;
                    ClassElement baseClassElement = instanceView.DataView.SchemaModel.FindClass(baseClassName);
                    ImageAttributeElement imageAttributeElement = baseClassElement.FindInheritedImageAttribute(context.PropertyDescriptor.Name);
                    InstanceAttributePropertyDescriptor pd = context.PropertyDescriptor as InstanceAttributePropertyDescriptor;
                    if (imageAttributeElement != null)
                    {
                        if (instanceView.InstanceData.ObjId != null && !instanceView.InstanceData.IsDuplicated)
                        {
                            // the data instance exists
                            dialog.InstanceId = instanceView.InstanceData.ObjId;
                        }
                        else
                        {
                            dialog.InstanceId = null;
                        }

                        if (pd != null && pd.IsReadOnly)
                        {
                            dialog.IsReadOnly = true;
                        }

                        dialog.AttributeName = imageAttributeElement.Name;
                        dialog.SchemaInfo = instanceView.DataView.SchemaInfo;
                        dialog.ClassName = imageAttributeElement.OwnerClass.Name;

                        // Display the dialog
                        editorService.ShowDialog(dialog);
                    }

                    return dialog.ImageId;
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
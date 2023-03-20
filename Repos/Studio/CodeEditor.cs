/*
* @(#)CodeEditor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
    using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

    using Newtera.WinClientCommon;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Subscribers;
    using Newtera.Common.MetaData.Api;
	
	/// <summary>
    /// A Modal UI editor for the Code for the class methods of ClassElement
	/// </summary>
	/// <version>  1.0.0 11 Nov 2009</version>
	public class CodeEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the CodeEditor class.
		/// </summary>
		public CodeEditor() : base()
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
		/// Override the method to launch a ActionCodeEditDialog modal dialog
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen class name</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

                if (editorService != null)
                {
                    if (context.Instance is ClassElement)
                    {
                        ClassElement classElement = (ClassElement)context.Instance;

                        // Create an instance of CodeEditDialog
                        CodeEditDialog dialog = new CodeEditDialog();

                        dialog.BindingSchemaId = classElement.SchemaModel.MetaData.SchemaInfo.NameAndVersion;
                        dialog.BindingClassName = classElement.Name;

                        if (value != null)
                        {
                            dialog.Code = (string)value;
                        }

                        // Display the dialog
                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            return dialog.Code;
                        }
                    }
                    else if (context.Instance is Subscriber)
                    {
                        Subscriber subscriber = (Subscriber)context.Instance;

                        // Create an instance of CodeEditDialog
                        CodeEditDialog dialog = new CodeEditDialog();

                        dialog.BindingSchemaId = subscriber.MetaData.SchemaInfo.NameAndVersion;
                        ClassElement classElement = subscriber.MetaData.SchemaModel.FindClass(subscriber.ClassName);
                        dialog.BindingClassName = classElement.Name;

                        if (value != null)
                        {
                            dialog.Code = (string)value;
                        }

                        // Display the dialog
                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            return dialog.Code;
                        }
                    }
                    else if (context.Instance is Api)
                    {
                        Api api = (Api)context.Instance;

                        // Create an instance of CodeEditDialog
                        CodeEditDialog dialog = new CodeEditDialog();

                        dialog.BindingSchemaId = api.MetaData.SchemaInfo.NameAndVersion;
                        ClassElement classElement = api.MetaData.SchemaModel.FindClass(api.ClassName);
                        dialog.BindingClassName = classElement.Name;

                        if (value != null)
                        {
                            dialog.Code = (string)value;
                        }

                        // Display the dialog
                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            return dialog.Code;
                        }
                    }
                }
			}

			return base.EditValue(context, provider, value);
		}
	}
}
/*
* @(#)AttributesUpdatedPropertyEditor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
    using System.Collections.Specialized;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Modal UI editor for the AttributesUpdated property of EventDef in
	/// the namespace of Newtera.Common.MetaData.Events.
	/// </summary>
	/// <version>  1.0.1 9 Jan 2007</version>
	public class AttributesUpdatedPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the AttributesUpdatedPropertyEditor class.
		/// </summary>
		public AttributesUpdatedPropertyEditor() : base()
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
		/// Override the method to launch a ChooseAttributesUpdateDialog modal dialog
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
                    EventDef eventDef = (EventDef)context.Instance;
                    if (eventDef.OperationType == OperationType.Update)
                    {
                        // Create an instance of ChooseAttributesUpdateDialog
                        ChooseAttributesUpdatedDialog dialog = new ChooseAttributesUpdatedDialog();

                        if (value != null)
                        {
                            dialog.AttributesUpdated = (StringCollection)value;
                        }

                        MetaDataModel metaData = eventDef.MetaData;
                        ClassElement theClass = metaData.SchemaModel.FindClass(eventDef.ClassName);
                        // Set the classElement to the dialog
                        dialog.ClassElement = theClass;

                        // Display the dialog
                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            return dialog.AttributesUpdated;
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.ForUpdateOperation"),
                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
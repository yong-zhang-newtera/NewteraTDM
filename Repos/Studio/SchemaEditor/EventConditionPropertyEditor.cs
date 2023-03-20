/*
* @(#)EventConditionPropertyEditor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;

    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.DataView;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Modal UI editor for the BeforeCondition and AfterCondition properties of EventDef in
	/// the namespace of Newtera.Common.MetaData.Events.
	/// </summary>
	/// <version>  1.0.1 22 Dec 2006</version>
	public class EventConditionPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the EventConditionPropertyEditor class.
		/// </summary>
		public EventConditionPropertyEditor() : base()
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
                    EventDef eventDef = (EventDef)context.Instance;

                    if (eventDef.OperationType == OperationType.Insert ||
                        eventDef.OperationType == OperationType.Update ||
                        eventDef.OperationType == OperationType.Timer)
                    {
                        // Create an instance of CreateSearchExprDialog
                        CreateSearchExprDialog dialog = new CreateSearchExprDialog();

                        // get the data view created by the EventDef
                        MetaDataModel metaData = eventDef.MetaData;
                        DataViewModel dataView = metaData.GetDetailedDataView(eventDef.ClassName);

                        if (dataView != null)
                        {
                            // set the data view's search filter to the local search filter
                            // because we only want to edit the search expression local to
                            // the current taxon node
                            dataView.ClearSearchExpression();
                            // set the current search expression if any
                            if (value != null)
                            {
                                dataView.AddSearchExpr((IDataViewElement)value,
                                    ElementType.And);
                            }

                            // Set the data view model to the dialog
                            dialog.DataView = dataView;

                            // Display the dialog
                            if (editorService.ShowDialog(dialog) == DialogResult.OK)
                            {
                                // clone the data view model so that the Expression object is
                                // different from the one that has been set, therefore,
                                // cause the ValueChange event to happen in the PropertyEditor
                                DataViewModel newDataView = dataView;
                                if (value != null)
                                {
                                    newDataView = dataView.Clone();
                                }
                                return newDataView.FilterExpr;
                            }
                        }
                        else
                        {
                            MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.MissingClassSpec"),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.ForUpdateAndInsert"),
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
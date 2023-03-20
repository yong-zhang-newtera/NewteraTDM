/*
* @(#)EventNamePropertyEditor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
    using System.Workflow.ComponentModel;

	using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Events;
	using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
	/// A DropDown UI editor for the EventName property of HandleNewteraEventActivity in
	/// the namespace of Newtear.Activities.
	/// </summary>
	/// <version>  1.0.1 02 Jan 2006</version>
	public class EventNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the EventNamePropertyEditor class.
		/// </summary>
		public EventNamePropertyEditor() : base()
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
		/// Override the method to show a dropdown list of event names
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen event name</returns>
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
                    string[] eventsNames = null;

					if (context.Instance is Activity)
					{
                        Activity activity = (Activity)context.Instance;
                        INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                        if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(rootActivity.SchemaId))
                        {
                            if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(rootActivity.SchemaId, rootActivity.ClassName))
                            {
						        // display the events in the dropdown list
                                MetaDataModel metadata = MetaDataStore.Instance.GetMetaData(rootActivity.SchemaId);
                                ClassElement clsElement = metadata.SchemaModel.FindClass(rootActivity.ClassName);
                                if (clsElement != null)
                                {
                                    EventCollection events = metadata.EventManager.GetClassEvents(clsElement);

                                    eventsNames = new string[events.Count];
                                    int index = 0;
                                    foreach (EventDef eventDef in events)
                                    {
                                        eventsNames[index++] = eventDef.Name;
                                    }

                                    listPicker.DataSource = eventsNames;
                                }
                            }
                            else
                            {
                                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.SetClassNameFirst"),
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.SetSchemaIdFirst"),
                                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
					}

					editorService.DropDownControl(listPicker);

                    if (eventsNames != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < eventsNames.Length)
					{
						return eventsNames[listPicker.SelectedIndex];
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
/*
* @(#)EventNamePropertyEditor.cs
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
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Subscribers;
	
	/// <summary>
	/// A DropDown UI editor for the EventName property of Subscriber in
	/// the namespace of Newtear.Common.Subscribers
	/// </summary>
	/// <version>  1.0.1 17 Sep 2013</version>
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

                    if (context.Instance is Subscriber)
                    {
                        Subscriber subscriber = (Subscriber)context.Instance;

                        // display the events in the dropdown list
                        MetaDataModel metadata = subscriber.MetaData;
                        ClassElement clsElement = metadata.SchemaModel.FindClass(subscriber.ClassName);
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
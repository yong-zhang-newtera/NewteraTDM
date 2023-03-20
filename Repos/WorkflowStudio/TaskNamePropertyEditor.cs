/*
* @(#)TaskNamePropertyEditor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
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
    using System.Collections.Specialized;

	using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
	/// A DropDown UI editor for the ActivityName property of CloseTaskActivity in
	/// the namespace of Newtear.Activities.
	/// </summary>
	/// <version>  1.0.1 18 May 2007</version>
	public class TaskNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the TaskNamePropertyEditor class.
		/// </summary>
		public TaskNamePropertyEditor() : base()
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
		/// Override the method to show a dropdown list of names of CreateTaskActivity in the workflow
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
                    string[] taskNames = null;
                    StringCollection taskNameCollection = new StringCollection();

					if (context.Instance is CloseTaskActivity)
					{
                        Activity activity = (Activity)context.Instance;
                        CompositeActivity rootActivity = (CompositeActivity) ActivityUtil.GetRootActivity(activity);
                        
                        // travel all the child activities, and collect the names of CreateTaskActivity
                        // and display the names in the dropdown list
                        FindCreateTaskActivities(rootActivity, taskNameCollection);
                        taskNames = new string[taskNameCollection.Count];
                        int index = 0;
                        foreach (string taskName in taskNameCollection)
                        {
                            taskNames[index++] = taskName;
                        }

                        listPicker.DataSource = taskNames;
					}

					editorService.DropDownControl(listPicker);

                    if (taskNames != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < taskNames.Length)
					{
                        return taskNames[listPicker.SelectedIndex];
					}
					else
					{
						return "";
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        /// <summary>
        /// Find all CreateTaskActivity children in the composite activity, and keep the name
        /// in the StringCollection. This method is called recursively.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="taskNameCollection"></param>
        private void FindCreateTaskActivities(CompositeActivity parent, StringCollection taskNameCollection)
        {
            foreach (Activity child in parent.Activities)
            {
                if (child is CompositeActivity)
                {
                    // call it recursively
                    FindCreateTaskActivities((CompositeActivity)child, taskNameCollection);
                }
                else if (child is CreateTaskActivity)
                {
                    taskNameCollection.Add(child.Name);
                }
            }
        }
	}
}
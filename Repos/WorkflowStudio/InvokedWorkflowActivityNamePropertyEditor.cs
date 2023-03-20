/*
* @(#)InvokedWorkflowActivityNamePropertyEditor.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
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
    /// A DropDown UI editor for the ActivityName property of HandleWorkflowEventActivity in
	/// the namespace of Newtear.Activities.
	/// </summary>
	/// <version>  1.0.1 18 Dec 2010</version>
	public class InvokedWorkflowActivityNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the InvokedWorkflowActivityNamePropertyEditor class.
		/// </summary>
		public InvokedWorkflowActivityNamePropertyEditor() : base()
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
		/// Override the method to show a dropdown list of names of InvokeAsyncNewteraWorkflowActivity in the workflow
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
                    string[] activityNames = null;
                    StringCollection activityNameCollection = new StringCollection();

					if (context.Instance is HandleWorkflowEventActivity)
					{
                        Activity activity = (Activity)context.Instance;
                        CompositeActivity rootActivity = (CompositeActivity) ActivityUtil.GetRootActivity(activity);

                        // travel all the child activities, and collect the names of InvokeAsyncNewteraWorkflowActivity
                        // and display the names in the dropdown list
                        FindInvokeWorkflowActivities(rootActivity, activityNameCollection);
                        activityNames = new string[activityNameCollection.Count];
                        int index = 0;
                        foreach (string activityName in activityNameCollection)
                        {
                            activityNames[index++] = activityName;
                        }

                        listPicker.DataSource = activityNames;
					}

					editorService.DropDownControl(listPicker);

                    if (activityNames != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < activityNames.Length)
					{
                        return activityNames[listPicker.SelectedIndex];
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
        /// Find all InvokeAsyncNewteraWorkflowActivity children in the composite activity, and keep the name
        /// in the StringCollection. This method is called recursively.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="taskNameCollection"></param>
        private void FindInvokeWorkflowActivities(CompositeActivity parent, StringCollection activityNameCollection)
        {
            foreach (Activity child in parent.Activities)
            {
                if (child is CompositeActivity)
                {
                    // call it recursively
                    FindInvokeWorkflowActivities((CompositeActivity)child, activityNameCollection);
                }
                else if (child is InvokeAsyncNewteraWorkflowActivity)
                {
                    activityNameCollection.Add(child.Name);
                }
            }
        }
	}
}
/*
* @(#)WorkflowNamePropertyEditor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
    using System.Threading;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
    using System.Workflow.ComponentModel;

	using Newtera.WinClientCommon;
    using Newtera.Common.Core;
    using Newtera.WFModel;
	using Newtera.Activities;
	
	/// <summary>
	/// A DropDown UI editor for the workflow name property of IInvokeWorkflowActivity in
	/// the namespace of Newtear.Activities.
	/// </summary>
	/// <version>  1.0.1 21 Mar 2007</version>
	public class WorkflowNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the WorkflowNamePropertyEditor class.
		/// </summary>
		public WorkflowNamePropertyEditor() : base()
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
		/// Override the method to show a dropdown list of workflow names in a project
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen schema id</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

                IInvokeWorkflowActivity activity = null;
                string[] workflowNames = new string[0];

				if (editorService != null)
				{
					// Create an instance of UI editor control
					DropDownListControl listPicker = new DropDownListControl(editorService);

                    if (context.Instance is IInvokeWorkflowActivity)
					{
                        activity = (IInvokeWorkflowActivity)context.Instance;
                        CompositeActivity rootActivity = (CompositeActivity)ActivityUtil.GetRootActivity((Activity)activity);

                        // get the current workflow project from a global context
                        if (ProjectModelContext.Instance.Project != null)
                        {
                            // Get the list of workflow name
                            workflowNames = GetWorkflowNamess(ProjectModelContext.Instance.Project, rootActivity.Name);
                        }
         
						// display the workflow names in the dropdown list
                        if (workflowNames != null)
                        {
                            listPicker.DataSource = workflowNames;
                        }
					}

					editorService.DropDownControl(listPicker);

                    if (listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < workflowNames.Length)
					{
                        return workflowNames[listPicker.SelectedIndex];
					}
					else
					{
						return "";
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        private string[] GetWorkflowNamess(ProjectModel project, string hostingWorkflowName)
        {
            string[] workflowNames = new string[0];
            if (project.Workflows.Count > 0)
            {
                workflowNames = new string[project.Workflows.Count];
                int index = 0;
                foreach (WorkflowModel workflow in project.Workflows)
                {
                    workflowNames[index] = workflow.Name;
                    index++;
                }
            }

            return workflowNames;
        }
    }
}
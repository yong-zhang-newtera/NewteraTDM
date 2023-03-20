/*
* @(#)ProjectNamePropertyEditor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
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
	
	/// <summary>
	/// A DropDown UI editor for the project name property of SubstituteEntry in
	/// the namespace of Newtear.WFModel.
	/// </summary>
	/// <version>  1.0.1 27 Oct 2008</version>
	public class ProjectNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ProjectNamePropertyEditor class.
		/// </summary>
		public ProjectNamePropertyEditor() : base()
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

                string[] projectNames = null;

				if (editorService != null)
				{
					// Create an instance of UI editor control
					DropDownListControl listPicker = new DropDownListControl(editorService);

					if (context.Instance is SubstituteEntry)
					{
                        // Get the list of project names
                        projectNames = GetProjectNames();
         
						// display the project names in the dropdown list
                        listPicker.DataSource = projectNames;
					}

					editorService.DropDownControl(listPicker);

                    if (listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < projectNames.Length)
					{
                        return projectNames[listPicker.SelectedIndex];
					}
					else
					{
						return "";
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        private string[] GetProjectNames()
        {
            WorkflowModelServiceStub service = new WorkflowModelServiceStub();
            ProjectInfo[] projectInfos = service.GetExistingProjectInfos();

            string[] projectNames = new string[0];

            if (projectInfos != null && projectInfos.Length > 0)
            {
                projectNames = new string[projectInfos.Length];
                int index = 0;
                foreach (ProjectInfo projectInfo in projectInfos)
                {
                    projectNames[index++] = projectInfo.Name + " " + projectInfo.Version;
                }
            }

            return projectNames;
        }
    }
}
/*
* @(#)AttributeNamePropertyEditor.cs
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

	using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
	using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
	/// A DropDown UI editor for the UsersBindingAttribute and RolesBindingAttribute properties of
    /// CreateTaskActivity in the namespace of Newtear.Activities.
	/// </summary>
	/// <version>  1.0.1 18 Jan 2007</version>
	public class AttributeNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the AttributeNamePropertyEditor class.
		/// </summary>
		public AttributeNamePropertyEditor() : base()
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
                    string[] attributeNames = null;
                    string[] attributeCaptions = null;

					if (context.Instance is CreateTaskActivity || context.Instance is CreateGroupTaskActivity)
					{
                        System.Workflow.ComponentModel.Activity activity = (System.Workflow.ComponentModel.Activity)context.Instance;
                        INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                        if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(rootActivity.SchemaId))
                        {
                            if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(rootActivity.SchemaId, rootActivity.ClassName))
                            {
                                string bindingClassName = rootActivity.ClassName;
                                MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(rootActivity.SchemaId);

                                DataViewModel dataView = metaData.GetDetailedDataView(bindingClassName);
                                if (dataView != null)
                                {
                                    ResultAttributeCollection attributes = dataView.ResultAttributes;

                                    attributeNames = new string[attributes.Count];
                                    attributeCaptions = new string[attributes.Count];
                                    int index = 0;
                                    foreach (IDataViewElement attr in attributes)
                                    {
                                        if (context.Instance is CreateTaskActivity)
                                        {
                                            attributeNames[index] = attr.Name;
                                            attributeCaptions[index++] = attr.Caption;
                                        }
                                        else if (context.Instance is CreateGroupTaskActivity && attr is DataArrayAttribute)
                                        {
                                            attributeNames[index] = attr.Name;
                                            attributeCaptions[index++] = attr.Caption;
                                        }
                                    }

                                    listPicker.DataSource = attributeCaptions;
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

                    if (attributeNames != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < attributeNames.Length)
					{
						return attributeNames[listPicker.SelectedIndex];
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
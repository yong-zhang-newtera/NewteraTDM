/*
* @(#)ApiNamePropertyEditor.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
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
    using Newtera.WindowsControl;
    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Api;
    using Newtera.WFModel;

    /// <summary>
    /// A DropDown UI editor for the ApiName property of CustomAction in
    /// the namespace of Newtera.WFModel
    /// </summary>
    /// <version>  1.0.0 27 Oct 2015</version>
    public class ApiNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ApiNamePropertyEditor class.
		/// </summary>
		public ApiNamePropertyEditor() : base()
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
		/// Override the method to show a pulldown menu
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
					// Create an instance of UI editor control
					DropDownListControl listPicker = new DropDownListControl(editorService);

                    if (context.Instance is CustomAction)
                    {
                        CustomAction customAction = (CustomAction)context.Instance;

                        MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(customAction.SchemaId);

                        ApiCollection apis = metaData.ApiManager.GetClassApis(customAction.ClassName);

                        if (apis != null)
                        {

                            listPicker.DataSource = apis;
                            listPicker.DisplayMember = "Name";

                            editorService.DropDownControl(listPicker);

                            if (listPicker.SelectedIndex >= 0)
                            {
                                return apis[listPicker.SelectedIndex].Name;
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
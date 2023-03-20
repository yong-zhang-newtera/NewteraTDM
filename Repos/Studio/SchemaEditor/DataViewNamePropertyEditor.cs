/*
* @(#)DataViewNamePropertyEditor.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
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
	using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView.Taxonomy;
	
	/// <summary>
	/// A DropDown UI editor for the DataViewName property of TaxonomyModel in
	/// the namespace of Newtear.Coomon.MetaData.DataView.Taxonomy
	/// </summary>
	/// <version>  1.0.1 15 Feb 2004</version>
	/// <author> Yong Zhang</author>
	public class DataViewNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the DataViewNamePropertyEditor class.
		/// </summary>
		public DataViewNamePropertyEditor() : base()
		{
		}

		/// <summary> 
        /// Overrides the inherited method to return a DropDown style
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
		/// Override the method to show a drop-down menu of data views
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

				DataViewModelCollection dataViews = null;

				if (editorService != null)
				{
					// Create an instance of UI editor control
					DropDownListControl listPicker = new DropDownListControl(editorService);

					if (context.Instance is ITaxonomy ||
                        context.Instance is ClassElement)
					{
                        string className = null;
                        MetaDataModel metaData = null;
      
						ITaxonomy node = context.Instance as ITaxonomy;
                        ClassElement classElement = context.Instance as ClassElement;

                        if (node != null)
                        {
                            className = node.ClassName;
                            metaData = node.MetaDataModel;
                        }
                        else if (classElement != null)
                        {
                            className = classElement.Name;
                            metaData = classElement.SchemaModel.MetaData;
                        }

						// get list of data views
						if (!string.IsNullOrEmpty(className))
						{
                            dataViews = metaData.DataViews.GetDataViewsForClass(className);

                            if (dataViews != null)
                            {
                                string[] dataViewCaptions = new string[dataViews.Count];
                                int index = 0;
                                foreach (DataViewModel dataView in dataViews)
                                {
                                    dataViewCaptions[index++] = dataView.Caption;
                                }

                                listPicker.DataSource = dataViewCaptions;
                            }
						}
					}

					editorService.DropDownControl(listPicker);

					if (dataViews != null &&
						listPicker.SelectedIndex >= 0 &&
						listPicker.SelectedIndex < dataViews.Count)
					{
						return ((DataViewModel) dataViews[listPicker.SelectedIndex]).Name;
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
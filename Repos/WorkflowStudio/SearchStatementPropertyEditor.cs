/*
* @(#)SearchStatementPropertyEditor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
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
	using Newtera.Common.MetaData.DataView;
    using Newtera.Activities;
    using Newtera.WorkflowStudioControl;
    using Newtera.WFModel;
	
	/// <summary>
	/// A Modal UI editor for the SearchStatement property of BindDataInstanceActivity in
	/// the namespace of Newtear.Activities
	/// </summary>
	/// <version>  1.0.0 09 Jan 2008</version>
	public class SearchStatementPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the SearchStatementPropertyEditor class.
		/// </summary>
		public SearchStatementPropertyEditor() : base()
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
		/// Override the method to launch a SearchStatement modal dialog
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns>The chosen class name</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null)
			{
				// Access the property grid's UI display service
				IWindowsFormsEditorService editorService =
					(IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (editorService != null)
				{
                    BindDataInstanceActivity activity = (BindDataInstanceActivity)context.Instance;
                    INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);

                    if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(rootActivity.SchemaId))
                    {
                        if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(rootActivity.SchemaId, rootActivity.ClassName))
                        {
                            // Create an CreateSearchExprDialog to specify the data instance to be found
                            CreateSearchExprDialog dialog = new CreateSearchExprDialog();

                            MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(rootActivity.SchemaId);
                  
                            DataViewModel dataView = metaData.GetDefaultDataView(rootActivity.ClassName);

                            if (!string.IsNullOrEmpty(activity.SearchExprXml))
                            {
                                // there is previously saved search expression, restore it
                                try
                                {
                                    IDataViewElement searchExpr = QueryStatementUtil.CreateSearchExprFromXml(activity.SearchExprXml);
                                    searchExpr.DataView = dataView;
                                    dataView.FilterExpr = searchExpr;
                                }
                                catch (Exception)
                                {
                                    // something wrong, just clear it
                                    dataView.ClearSearchExpression();
                                }
                            }
                            else
                            {
                                dataView.ClearSearchExpression();
                            }

                            dialog.DataView = dataView;

                            // Display the dialog
                            if (editorService.ShowDialog(dialog) == DialogResult.OK)
                            {
                                // save the search expression as xml in the activity so that we can restore it
                                // next time the CreateSearchExprDialog is opened
                                string xml = QueryStatementUtil.GenerateSearchExprXml(dataView.FilterExpr);
                                activity.SearchExprXml = xml;
                                return dataView.SearchQuery; ;
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
			}

			return base.EditValue(context, provider, value);
		}
	}
}
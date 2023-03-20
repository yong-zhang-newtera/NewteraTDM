/*
* @(#)StateParameterCollectionEditor.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
    using System.Collections.Generic;
	using System.Windows.Forms.Design;
	
	using Newtera.Common.MetaData.SiteMap;

	/// <summary>
	/// A Modal UI editor for editing StateParameters property of SiteMapNode. It inherites
	/// from UITypeEditor class.
	/// </summary>
	/// <version>  1.0.0 15 Mar 2016</version>
	public class StateParameterCollectionEditor : UITypeEditor
	{
		public StateParameterCollectionEditor() : base()
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
		/// Override the method to launch a DefineStateParameterDialog modal dialog
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

                SiteMapNode siteMapNode = context.Instance as SiteMapNode;
                CustomCommand customCommand = context.Instance as CustomCommand;

                SiteMapNodeCollection stateParameters = null;
                if (siteMapNode != null)
                {
                    stateParameters = siteMapNode.StateParameters;
                }
                else
                {
                    stateParameters = customCommand.StateParameters;
                }

                if (editorService != null)
				{
                    // Create an instance of DefineStateParametersDialog
                    DefineStateParametersDialog dialog = new DefineStateParametersDialog();
                    dialog.StateParameters = GetClonedCollection(stateParameters);

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
                        return dialog.StateParameters;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        private SiteMapNodeCollection GetClonedCollection(SiteMapNodeCollection originalCollection)
        {
            SiteMapNodeCollection clonedCollection = new SiteMapNodeCollection();

            StateParameter cloned;
            foreach (StateParameter original in originalCollection)
            {
                cloned = new StateParameter();
                cloned.Name = original.Name;
                cloned.Value = original.Value;
                cloned.Description = original.Description;
                clonedCollection.Add(cloned);
            }

            return clonedCollection;
        }
	}
}
/*
* @(#)EnumValueCollectionEditor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
	
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A Modal UI editor for editing enum value EnumValueCollection instance. It inherites
	/// from UITypeEditor class.
	/// </summary>
	/// <version>  1.0.1 16 Aug 2006</version>
	/// <author> Yong Zhang</author>
	public class EnumValueCollectionEditor : UITypeEditor
	{
		public EnumValueCollectionEditor() : base()
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
		/// Override the method to launch a DefineEnumValuesDialog modal dialog
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

				EnumElement enumElement = (EnumElement) context.Instance;

				if (editorService != null)
				{
					// Create an instance of DefineEnumValuesDialog
					DefineEnumValuesDialog dialog = new DefineEnumValuesDialog();
					dialog.EnumElement = enumElement;

					dialog.Values = enumElement.Values.Clone();

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
						return dialog.Values;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

		/// <summary>
		/// Gets the information indicating whether two string array are equal
		/// </summary>
		/// <param name="oldValues">old array</param>
		/// <param name="newValues">new array</param>
		/// <returns></returns>
		private bool IsSame(string[] oldValues, string[] newValues)
		{
			bool status = true;

			if (oldValues.Length != newValues.Length)
			{
				status = false;
			}
			else
			{
				for (int i = 0; i < oldValues.Length; i++)
				{
					if (oldValues[i] != newValues[i])
					{
						status = false;
						break;
					}
				}
			}

			return status;
		}
	}
}
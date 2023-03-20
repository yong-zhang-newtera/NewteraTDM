/*
* @(#)OperatorPropertyEditor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing.Design;
	using System.Windows.Forms.Design;
    using System.Collections.Specialized;

	using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A DropDown UI editor for the Operator property of SimpleAttributeElement in
	/// the namespace of Newtear.Coomon.MetaData.Schema
	/// </summary>
	/// <version>  1.0.1 06 May 2008</version>
	public class OperatorPropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the OperatorPropertyEditor class.
		/// </summary>
		public OperatorPropertyEditor() : base()
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

                StringCollection operators = new StringCollection();

				if (editorService != null)
				{
					// Create an instance of UI editor control
					DropDownListControl listPicker = new DropDownListControl(editorService);

					if (context.Instance is SimpleAttributeElement)
					{
                        SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)context.Instance;

                        // add common operators
                        operators.Add(MetaDataModel.OPT_EQUALS);
                        operators.Add(MetaDataModel.OPT_NOT_EQUALS);

						// add operators appropriate for the data type of the simple attribute
						if (simpleAttribute.DataType != DataType.Unknown)
						{
                            switch (simpleAttribute.DataType)
                            {
                                case DataType.String:
                                case DataType.Text:
                                    operators.Add(MetaDataModel.OPT_LIKE);
                                    break;
                                case DataType.BigInteger:
                                case DataType.Byte:
                                case DataType.Date:
                                case DataType.DateTime:
                                case DataType.Decimal:
                                case DataType.Double:
                                case DataType.Float:
                                case DataType.Integer:
                                    operators.Add(MetaDataModel.OPT_LESS_THAN);
                                    operators.Add(MetaDataModel.OPT_GREATER_THAN);
                                    operators.Add(MetaDataModel.OPT_LESS_THAN_EQUALS);
                                    operators.Add(MetaDataModel.OPT_GREATER_THAN_EQUALS);
                                    break;
                            }
						}

                        listPicker.DataSource = operators;
					}

					editorService.DropDownControl(listPicker);

					if (operators.Count > 0 &&
						listPicker.SelectedIndex >= 0 &&
						listPicker.SelectedIndex < operators.Count)
					{
						return operators[listPicker.SelectedIndex];
					}
					else
					{
						return MetaDataModel.OPT_EQUALS;
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
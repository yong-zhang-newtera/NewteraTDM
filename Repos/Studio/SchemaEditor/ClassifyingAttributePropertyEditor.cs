/*
* @(#)ClassifyingAttributePropertyEditor.cs
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

	using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.DataView.Taxonomy;
	
	/// <summary>
	/// A DropDown UI editor for the ClassifyingAttribute property of
    /// AutoClassifyLevel class in the namespace of Newtear.Common.MetaData.DataView.Taxonomy.
	/// </summary>
	/// <version>  1.0.1 14 June 2008</version>
	public class ClassifyingAttributePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the ClassifyingAttributePropertyEditor class.
		/// </summary>
		public ClassifyingAttributePropertyEditor() : base()
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
                    string[] ownerClassAliases = null;

                    AutoClassifyLevel autoClassifyLevel = null;
					if (context.Instance is AutoClassifyLevel)
					{
                        autoClassifyLevel = (AutoClassifyLevel)context.Instance;

                        DataViewModel dataView = autoClassifyLevel.DataView;
                        if (dataView != null)
                        {
                            ResultAttributeCollection attributes = dataView.ResultAttributes;

                            // only the simple and relationship attributes can be used as classifying attributes
                            ResultAttributeCollection classifyingAttributes = new ResultAttributeCollection();
                            foreach (IDataViewElement attr in attributes)
                            {
                                if (attr is DataSimpleAttribute)
                                {
                                    SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)attr.GetSchemaModelElement();
                                    if (!simpleAttribute.IsMultipleChoice &&
                                        !simpleAttribute.IsHistoryEdit &&
                                        !simpleAttribute.IsRichText)
                                    {
                                        classifyingAttributes.Add(attr);
                                    }
                                }
                                /*
                                else if (attr is DataRelationshipAttribute)
                                {
                                    // only the relationships of many-to-one and one-to-one can be used as classifying attribute
                                    if (((DataRelationshipAttribute)attr).IsForeignKeyRequired)
                                    {
                                        classifyingAttributes.Add(attr);
                                    }
                                }
                                 */
                            }

                            attributeNames = new string[classifyingAttributes.Count];
                            attributeCaptions = new string[classifyingAttributes.Count];
                            ownerClassAliases = new string[classifyingAttributes.Count];
                            int index = 0;
                            string ownerClassAlias = null;
                            foreach (IDataViewElement classifyingAttribute in classifyingAttributes)
                            {
                                if (classifyingAttribute is DataSimpleAttribute)
                                {
                                    ownerClassAlias = ((DataSimpleAttribute)classifyingAttribute).OwnerClassAlias;
                                }
                                else if (classifyingAttribute is DataRelationshipAttribute)
                                {
                                    ownerClassAlias = ((DataRelationshipAttribute)classifyingAttribute).OwnerClassAlias;
                                }

                                attributeNames[index] = classifyingAttribute.Name;
                                attributeCaptions[index] = classifyingAttribute.Caption;
                                ownerClassAliases[index++] = ownerClassAlias;
                            }

                            listPicker.DataSource = attributeCaptions;
                        }

                        editorService.DropDownControl(listPicker);

                        if (attributeNames != null &&
                            listPicker.SelectedIndex >= 0 &&
                            listPicker.SelectedIndex < attributeNames.Length)
                        {
                            // attribute name may not be unique in a dataview, therefore,
                            // we have to keep the owner class alias in the level definition
                            autoClassifyLevel.OwnerClassAlias = ownerClassAliases[listPicker.SelectedIndex];
                            return attributeNames[listPicker.SelectedIndex];
                        }
                        else
                        {
                            return "";
                        }
					}
				}
			}

			return base.EditValue(context, provider, value);
		}
	}
}
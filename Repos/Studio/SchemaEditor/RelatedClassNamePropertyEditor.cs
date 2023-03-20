/*
* @(#)RelatedClassNamePropertyEditor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
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
	
	/// <summary>
	/// A DropDown UI editor for the RelatedClassName property of CustomPageElement in
	/// the namespace of Newtear.Coomon.MetaData.Schema
	/// </summary>
	/// <version>  1.0.1 27 Jul 2009</version>
	public class RelatedClassNamePropertyEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the RelatedClassNamePropertyEditor class.
		/// </summary>
		public RelatedClassNamePropertyEditor() : base()
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
		/// Override the method to show a drop-down menu of related class
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

                    ReferencedClassCollection relatedClasses = null;

					if (context.Instance is CustomPageElement)
					{
                        CustomPageElement customPage = (CustomPageElement)context.Instance;

						// get list of related classes of the master class
                        if (!string.IsNullOrEmpty(customPage.MasetrClassName))
						{
                            DataViewModel dataView = customPage.SchemaModel.MetaData.GetDefaultDataView(customPage.MasetrClassName);
                            relatedClasses = new ReferencedClassCollection();
                            foreach (DataClass relatedClass in dataView.BaseClass.RelatedClasses)
                            {
                                relatedClasses.Add(relatedClass);
                                if (!relatedClass.IsLeafClass)
                                {
                                    // add the leaf classes to the list
                                    ReferencedClassCollection rleafClasses = GetLeafClasses(relatedClass, customPage.SchemaModel.MetaData);
                                    foreach (DataClass leafClass in rleafClasses)
                                    {
                                        if (!IsRelatedClassExist(leafClass, relatedClasses))
                                        {
                                            relatedClasses.Add(leafClass);
                                        }
                                    }
                                }
                            }

                            string[] relatedClassCaptions = new string[relatedClasses.Count];
							int index = 0;
                            ClassElement relatedClassElement;
                            foreach (DataClass relatedClass in relatedClasses)
							{
                                relatedClassElement = customPage.SchemaModel.FindClass(relatedClass.ClassName);
                                relatedClassCaptions[index++] = relatedClassElement.Caption;
							}

                            listPicker.DataSource = relatedClassCaptions;
						}
					}

					editorService.DropDownControl(listPicker);

                    if (relatedClasses != null &&
						listPicker.SelectedIndex >= 0 &&
                        listPicker.SelectedIndex < relatedClasses.Count)
					{
                        return ((DataClass)relatedClasses[listPicker.SelectedIndex]).ClassName;
					}
					else
					{
						return "";
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

        private bool IsRelatedClassExist(DataClass relatedClass, ReferencedClassCollection relatedClasses)
        {
            bool isExist = false;

            foreach (DataClass rClass in relatedClasses)
            {
                if (rClass.ClassName == relatedClass.ClassName)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }

        private ReferencedClassCollection GetLeafClasses(DataClass relatedClass, MetaDataModel metaData)
        {
            ReferencedClassCollection relatedLeafClasses = new ReferencedClassCollection();
            DataClass relatedLeafClass;
            SchemaModelElementCollection leafClassElements;

            leafClassElements = metaData.GetBottomClasses(relatedClass.ClassName);

            foreach (ClassElement leafClassElement in leafClassElements)
            {
                // create a DataClass using leaf class name
                relatedLeafClass = new DataClass(relatedClass.Name, leafClassElement.Name, DataClassType.ReferencedClass);
                relatedLeafClass.ReferringClassAlias = relatedClass.ReferringClassAlias;
                relatedLeafClass.ReferringRelationshipName = relatedClass.ReferringRelationshipName;
                relatedLeafClass.Caption = leafClassElement.Caption;
                relatedLeafClass.ReferringRelationship = relatedClass.ReferringRelationship;

                relatedLeafClasses.Add(relatedLeafClass);
            }

            return relatedLeafClasses;
        }
	}
}
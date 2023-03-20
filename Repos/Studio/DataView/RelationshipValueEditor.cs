/*
* @(#)RelationshipValueEditor.cs
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

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A Modal UI editor for the value of DataRelationshipAttribute in
	/// a Instance View.
	/// </summary>
	/// <version>  1.0.1 15 Nov. 2003</version>
	/// <author> Yong Zhang</author>
	public class RelationshipValueEditor : UITypeEditor
	{
		/// <summary>
		/// Initializes a new instance of the RelationshipValueEditor class.
		/// </summary>
		public RelationshipValueEditor() : base()
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
		/// Override the method to launch a ChooseInstanceDialog modal dialog
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
					// Create an instance of ChooseInstanceDialog
					ChooseInstanceDialog dialog = new ChooseInstanceDialog();

					// Get the linked class of the relationship
					string relationshipName = context.PropertyDescriptor.Name;
					InstanceView instanceView = (InstanceView) context.Instance;
					SchemaModel schemaModel = instanceView.DataView.SchemaModel;
					string ownerClassName = instanceView.DataView.BaseClass.ClassName;
					ClassElement classElement = instanceView.DataView.SchemaModel.FindClass(ownerClassName);
					RelationshipAttributeElement relationshipElement = classElement.FindInheritedRelationshipAttribute(relationshipName);
					ClassElement linkedClass = relationshipElement.LinkedClass;

					dialog.SchemaModel = schemaModel;
					dialog.LinkedClass = linkedClass;

					// Display the dialog
					if (editorService.ShowDialog(dialog) == DialogResult.OK)
					{
						InstanceData referencedInstance = dialog.SelectedInstance;
						InstanceData instanceData = instanceView.InstanceData;
						DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute) instanceView.DataView.ResultAttributes[relationshipName];

						// Copy the primary key values from the referenced instance data
						// to primary keys of the relationship attribute being edited
						CopyPrimaryKeyValues(relationshipAttribute, instanceData, referencedInstance);

						// The property grid expects a RelationshipPrimaryKeyView as
						// an value, therefore, create one with new values in instance data
						return new RelationshipPrimaryKeyView(relationshipAttribute,
								relationshipElement, instanceData);
					}
				}
			}

			return base.EditValue(context, provider, value);
		}

		/// <summary>
		/// Copy the primary key values of a referenced instance to the primary keys
		/// of a corresponding relationship in the targted instance data
		/// </summary>
		/// <param name="relationshipElement">The relationship attribute element</param>
		/// <param name="targetInstanceData">The targeted instance data</param>
		/// <param name="referencedInstance">The referenced instance data</param>
		private void CopyPrimaryKeyValues(DataRelationshipAttribute relationshipAttribute,
			InstanceData targetInstanceData, InstanceData referencedInstanceData)
		{
			DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
			if (primaryKeys != null)
			{
				foreach (DataSimpleAttribute pk in primaryKeys)
				{
					string pkValue = (string) referencedInstanceData.GetAttributeValue(pk.Name);

					// to set a pk value, the name combines that of relationship attribute and primary key
					targetInstanceData.SetAttributeValue(relationshipAttribute.Name + "_" + pk.Name, pkValue);
				}
			}
		}
	}
}
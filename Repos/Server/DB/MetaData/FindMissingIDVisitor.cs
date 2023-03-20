/*
* @(#)FindMissingIDVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Traverse the new meta data and Find those elements in the new meta data
	/// that do not have ID filled but they have the corresponding elements in
	/// the old mete data that have IDs. Then copy the IDs from elements in the
	/// old schema to those in the new one. This is to handle the situation when a 
	/// meta data coming from a meta data editor tool and the meta data
	/// has not been refreshed.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class FindMissingIDVisitor : ISchemaModelElementVisitor
	{
		private MetaDataModel _newMetaDataModel;
		private MetaDataModel _oldMetaDataModel;
		private IDataProvider _dataProvider;

		/// <summary>
		/// Instantiate an instance of FindMissingIDVisitor class
		/// </summary>
		/// <param name="newMetaDataModel">The new meta data model</param>
		/// <param name="oldMetaDataModel">The old meta data model</param>
		public FindMissingIDVisitor(MetaDataModel newMetaDataModel,
			MetaDataModel oldMetaDataModel,
			IDataProvider dataProvider)
		{
			_newMetaDataModel = newMetaDataModel;
			_oldMetaDataModel = oldMetaDataModel;
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		public void VisitClassElement(ClassElement element)
		{
			ClassElement oldClassElement;

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.Name)) != null)
			{
				if (element.ID == null || element.ID.Length == 0)
				{
					// copy id and table name
					element.ID = oldClassElement.ID;
					element.TableName = oldClassElement.TableName;
				}
			}
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{	
			ClassElement oldClassElement;
			SimpleAttributeElement oldAttribute;

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) != null &&
				(oldAttribute = oldClassElement.FindSimpleAttribute(element.Name)) != null)
			{
				if (element.ID == null || element.ID.Length == 0)
				{
					// copy id and column name
					element.ID = oldAttribute.ID;
					element.ColumnName = oldAttribute.ColumnName;
				}
			}
		}

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		public void VisitRelationshipAttributeElement(RelationshipAttributeElement element)
		{
			ClassElement oldClassElement;
			RelationshipAttributeElement oldAttribute;

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) != null &&
				(oldAttribute = oldClassElement.FindRelationshipAttribute(element.Name)) != null)
			{
				if (element.ID == null || element.ID.Length == 0)
				{
					// copy id and column name
					element.ID = oldAttribute.ID;
					element.ColumnName = oldAttribute.ColumnName;
				}
			}
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
			ClassElement oldClassElement;
			ArrayAttributeElement oldAttribute;

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) != null &&
				(oldAttribute = oldClassElement.FindArrayAttribute(element.Name)) != null)
			{
				if (element.ID == null || element.ID.Length == 0)
				{
					// copy id and column name
					element.ID = oldAttribute.ID;
					element.ColumnName = oldAttribute.ColumnName;
				}
			}
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A VirtualAttributeElement instance</param>
        public void VisitVirtualAttributeElement(VirtualAttributeElement element)
        {
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A ImageAttributeElement instance</param>
        public void VisitImageAttributeElement(ImageAttributeElement element)
        {
            ClassElement oldClassElement;
            ImageAttributeElement oldAttribute;

            if (_oldMetaDataModel != null &&
                (oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) != null &&
                (oldAttribute = oldClassElement.FindImageAttribute(element.Name)) != null)
            {
                if (element.ID == null || element.ID.Length == 0)
                {
                    // copy id and column name
                    element.ID = oldAttribute.ID;
                    element.ColumnName = oldAttribute.ColumnName;
                }
            }
        }

        /// <summary>
        /// Viste an custom page element.
        /// </summary>
        /// <param name="element">A CustomPageElement instance</param>
        public void VisitCustomPageElement(CustomPageElement element)
        {
        }

		/// <summary>
		/// Viste a schema info element.
		/// </summary>
		/// <param name="element">A SchemaInfoElement instance</param>
		public void VisitSchemaInfoElement(SchemaInfoElement element)
		{
			if (_oldMetaDataModel != null)
			{
				if (element.ID == null || element.ID.Length == 0)
				{
					// copy id
					element.ID = _oldMetaDataModel.SchemaModel.SchemaInfo.ID;
				}
			}
		}

		/// <summary>
		/// Viste an enum constraint element.
		/// </summary>
		/// <param name="element">A EnumElement instance</param>
		public void VisitEnumElement(EnumElement element)
		{
		}

		/// <summary>
		/// Viste a range constraint element.
		/// </summary>
		/// <param name="element">A RangeElement instance</param>
		public void VisitRangeElement(RangeElement element)
		{
		}

		/// <summary>
		/// Viste a pattern constraint element.
		/// </summary>
		/// <param name="element">A PatternElement instance</param>
		public void VisitPatternElement(PatternElement element)
		{
		}

		/// <summary>
		/// Viste a list constraint element.
		/// </summary>
		/// <param name="element">A ListElement instance</param>
		public void VisitListElement(ListElement element)
		{
		}
	}
}
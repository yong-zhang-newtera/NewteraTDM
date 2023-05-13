/*
* @(#)FindDeletionVisitor.cs
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
	/// Traverse the old meta data and find deleted elements, such as
	/// ClassElement, SimpleAttributeElement, RelationshipAttributeElement, and
	/// EnumElement etc.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class FindDeletionVisitor : ISchemaModelElementVisitor
	{
		private MetaDataModel _newMetaDataModel;
		private MetaDataModel _oldMetaDataModel;
		private MetaDataCompareResult _result;
		private IDataProvider _dataProvider;
        private bool _nonConditionalCompare;

		/// <summary>
		/// Instantiate an instance of FindDeletionVisitor class
		/// </summary>
		/// <param name="newMetaDataModel">The new meta data model</param>
		/// <param name="oldMetaDataModel">The old meta data model</param>
		/// <param name="result">The compare result</param>
		public FindDeletionVisitor(MetaDataModel newMetaDataModel,
			MetaDataModel oldMetaDataModel,
			MetaDataCompareResult result,
			IDataProvider dataProvider,
            bool nonConditionalCompare)
		{
			_newMetaDataModel = newMetaDataModel;
			_oldMetaDataModel = oldMetaDataModel;
			_result = result;
			_dataProvider = dataProvider;
            _nonConditionalCompare = nonConditionalCompare;
		}

        /// <summary>
        /// Instantiate an instance of FindDeletionVisitor class
        /// </summary>
        /// <param name="newMetaDataModel">The new meta data model</param>
        /// <param name="oldMetaDataModel">The old meta data model</param>
        /// <param name="result">The compare result</param>
        public FindDeletionVisitor(MetaDataModel newMetaDataModel,
            MetaDataModel oldMetaDataModel,
            MetaDataCompareResult result,
            IDataProvider dataProvider) 
        {
            _newMetaDataModel = newMetaDataModel;
            _oldMetaDataModel = oldMetaDataModel;
            _result = result;
            _dataProvider = dataProvider;
            _nonConditionalCompare = true;
        }

		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		public void VisitClassElement(ClassElement element)
		{
			IMetaDataAction action;

            if (!_nonConditionalCompare)
            {
                // do not allow deleting class elements in this mode
                return;
            }

			if (_newMetaDataModel == null ||
				_newMetaDataModel.SchemaModel.FindClass(element.Name) == null)
			{
				if (!element.IsRoot)
				{
					action = new DeleteClassInheritanceAction(_oldMetaDataModel, element, _dataProvider);
					_result.AddAlterClassAction(action);
				}

				// delete a class
				action = new DeleteClassAction(_oldMetaDataModel, element, _dataProvider);
				_result.AddDeleteClassAction(action);
			}
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{
			ClassElement newClassElement = null;
			bool skipDDLExecution = false;
			IMetaDataAction action;

			if (_newMetaDataModel == null ||
				(newClassElement = _newMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
				newClassElement.FindSimpleAttribute(element.Name) == null)
			{
				if (newClassElement == null)
				{
					// the class of the attribute is being deleted, there must be a DeleteClassAction
					// created which will take care of deleting this attributes
					skipDDLExecution = true;
				}

                if (!_nonConditionalCompare && (newClassElement == null || !newClassElement.NeedToAlter))
                {
                    // no need to delete the attribute
                    return;
                }

                if (element.IsAutoIncrement && !element.HasCustomValueGenerator)
				{
					action = new DeleteSimpleAttributeAutoIncrementAction(_oldMetaDataModel, element, _dataProvider);
					_result.AddAlterRelationshipAttributeAction(action);
				}

                if (element.IsHistoryEdit && element.DataType == DataType.Text)
                {
                    // history edit works on the attribute of Text type
                    action = new DeleteSimpleAttributeHistoryEditAction(_oldMetaDataModel, element, _dataProvider);
                    _result.AddDeleteSimpleAttributeAction(action); 
                }

                if (element.IsRichText && element.DataType == DataType.Text)
                {
                    // rich text works on the attribute of Text type
                    action = new DeleteSimpleAttributeRichTextAction(_oldMetaDataModel, element, _dataProvider);
                    _result.AddDeleteSimpleAttributeAction(action);
                }

                if (element.IsIndexed && !element.IsUnique && !element.IsPrimaryKey)
				{
					action = new DeleteSimpleAttributeIndexAction(_oldMetaDataModel, element, _dataProvider);
					_result.AddAlterRelationshipAttributeAction(action);
				}

				if (element.IsUnique && !element.IsPrimaryKey)
				{
					// when the element is part of primary key, it's been uniquely
					// constrainted.
					action = new DeleteSimpleAttributeUniqueConstraintAction(_oldMetaDataModel, element, _dataProvider);
					_result.AddAlterRelationshipAttributeAction(action);
				}

                if (!string.IsNullOrEmpty(element.DefaultValue))
                {
                    action = new DeleteSimpleAttributeDefaultValueAction(_oldMetaDataModel, element, _dataProvider);
                    _result.AddAlterRelationshipAttributeAction(action);
                }

				// it is a deleted simple attribute
				action = new DeleteSimpleAttributeAction(_oldMetaDataModel, element, _dataProvider);
				action.SkipDDLExecution = skipDDLExecution;
				_result.AddDeleteSimpleAttributeAction(action);
			}
		}

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		public void VisitRelationshipAttributeElement(RelationshipAttributeElement element)
		{
			ClassElement newClassElement = null;
			IMetaDataAction action;

			if (_newMetaDataModel == null ||
				(newClassElement = _newMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
				newClassElement.FindRelationshipAttribute(element.Name) == null)
			{
                if (!_nonConditionalCompare && (newClassElement == null || !newClassElement.NeedToAlter))
                {
                    // no need to delete the attribute
                    return;
                }

				if (element.IsForeignKeyRequired)
				{
                    if (element.IsIndexed)
                    {
                        action = new DeleteRelationshipAttributeIndexAction(_oldMetaDataModel, element, _dataProvider);
                        _result.AddDeleteRelationshipAttributeAction(action);
                    }

					// delete the FK constraint
					action = new DeleteRelationshipAttributeFKConstraintAction(_oldMetaDataModel, element, _dataProvider);
					_result.AddDeleteRelationshipAttributeAction(action);
				}

				// delete relationship attribute
				action = new DeleteRelationshipAttributeAction(_oldMetaDataModel, element, _dataProvider);
				_result.AddDeleteRelationshipAttributeAction(action);
			}
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
			ClassElement newClassElement = null;
			bool skipDDLExecution = false;
			IMetaDataAction action;

			if (_newMetaDataModel == null ||
				(newClassElement = _newMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
				newClassElement.FindArrayAttribute(element.Name) == null)
			{
				if (newClassElement == null)
				{
					// the class of the attribute is being deleted, there must be a DeleteClassAction
					// created which will take care of deleting this attributes
					skipDDLExecution = true;
				}

                if (!_nonConditionalCompare && (newClassElement == null || !newClassElement.NeedToAlter))
                {
                    // no need to delete the attribute
                    return;
                }

				// it is a deleted array attribute
				action = new DeleteArrayAttributeAction(_oldMetaDataModel, element, _dataProvider);
				action.SkipDDLExecution = skipDDLExecution;
				_result.AddDeleteArrayAttributeAction(action);
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
            ClassElement newClassElement = null;
            bool skipDDLExecution = false;
            IMetaDataAction action;

            if (_newMetaDataModel == null ||
                (newClassElement = _newMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
                newClassElement.FindImageAttribute(element.Name) == null)
            {
                if (newClassElement == null)
                {
                    // the class of the attribute is being deleted, there must be a DeleteClassAction
                    // created which will take care of deleting this attributes
                    skipDDLExecution = true;
                }

                if (!_nonConditionalCompare && (newClassElement == null || !newClassElement.NeedToAlter))
                {
                    // no need to delete the attribute
                    return;
                }

                // it is a deleted image attribute
                action = new DeleteImageAttributeAction(_oldMetaDataModel, element, _dataProvider);
                action.SkipDDLExecution = skipDDLExecution;
                _result.AddDeleteImageAttributeAction(action);
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
			if (_newMetaDataModel == null)
			{
				// it is new schema
				IMetaDataAction action = new DeleteSchemaAction(_oldMetaDataModel, element, _dataProvider);
				_result.DeleteSchema = action;
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
/*
* @(#)FindAdditionVisitor.cs
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
	/// Traverse the new meta data and find added elements, such as
	/// ClassElement, SimpleAttributeElement, RelationshipAttributeElement, and
	/// EnumElement etc, and create corresponding actions in the result.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class FindAdditionVisitor : ISchemaModelElementVisitor
	{
		private MetaDataModel _newMetaDataModel;
		private MetaDataModel _oldMetaDataModel;
		private MetaDataCompareResult _result;
		private IDataProvider _dataProvider;
        private bool _nonConditionalCompare;

		/// <summary>
		/// Instantiate an instance of FindAdditionVisitor class
		/// </summary>
		/// <param name="newMetaDataModel">The new meta data model</param>
		/// <param name="oldMetaDataModel">The old meta data model</param>
		/// <param name="result">The compare result</param>
		public FindAdditionVisitor(MetaDataModel newMetaDataModel,
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
        /// Instantiate an instance of FindAdditionVisitor class
        /// </summary>
        /// <param name="newMetaDataModel">The new meta data model</param>
        /// <param name="oldMetaDataModel">The old meta data model</param>
        /// <param name="result">The compare result</param>
        public FindAdditionVisitor(MetaDataModel newMetaDataModel,
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
            if (!_nonConditionalCompare && !element.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

            if (_oldMetaDataModel == null ||
                _oldMetaDataModel.SchemaModel.FindClass(element.Name) == null)
            {
                // it is an added class
                IMetaDataAction action = new AddClassAction(_newMetaDataModel, element, _dataProvider);
                _result.AddAddClassAction(action);

                if (!element.IsRoot)
                {
                    action = new AddClassInheritanceAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAddClassAction(action);
                }
            }
            else
            {
                // set the TableName of old class element to the new Class element
                ClassElement oldElement = _oldMetaDataModel.SchemaModel.FindClass(element.Name);
                element.TableName = oldElement.TableName;
                element.ID = oldElement.ID;
            }
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{
            if (!_nonConditionalCompare && !element.OwnerClass.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

			ClassElement oldClassElement = null;
			bool skipDDLExecution = false;

            if (_oldMetaDataModel == null ||
                (oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
                oldClassElement.FindSimpleAttribute(element.Name) == null)
            {
                if (oldClassElement == null)
                {
                    // this is a brand new class, there must be a AddClassAction
                    // created which will take care of adding simple attributes
                    skipDDLExecution = true;
                }

                // it is an added simple attribute
                IMetaDataAction action = new AddSimpleAttributeAction(_newMetaDataModel, element, _dataProvider);
                action.SkipDDLExecution = skipDDLExecution;
                _result.AddAddSimpleAttributeAction(action);

                if (element.IsUnique && !element.IsPrimaryKey)
                {
                    // when the element is part of primary key, it's been uniquely
                    // constrainted.
                    action = new AddSimpleAttributeUniqueConstraintAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }

                if (element.IsAutoIncrement && !element.HasCustomValueGenerator)
                {
                    // use database default auto-increment value generator
                    action = new AddSimpleAttributeAutoIncrementAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }

                if (element.IsFullTextSearchable && element.DataType == DataType.Text)
                {
                    // full text search works on the attribute of Text type
                    action = new AddSimpleAttributeFullTextSearchAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }

                if (element.IsHistoryEdit && element.DataType == DataType.Text)
                {
                    // history edit works on the attribute of Text type
                    action = new AddSimpleAttributeHistoryEditAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }

                if (element.IsRichText && element.DataType == DataType.Text)
                {
                    // history edit works on the attribute of Text type
                    action = new AddSimpleAttributeRichTextAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }

                if (element.IsIndexed && !element.IsUnique && !element.IsPrimaryKey)
                {
                    // when a element is unique or primary key, it will be indexed automatically
                    action = new AddSimpleAttributeIndexAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }

                if (!element.IsAutoIncrement && !string.IsNullOrEmpty(element.DefaultValue))
                {
                    // when a element has default value, add a database default value constraint
                    action = new AddSimpleAttributeDefaultValueAction(_newMetaDataModel, element, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }
            }
            else
            {
                oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name);
                SimpleAttributeElement oldSimpleAttribute = oldClassElement.FindSimpleAttribute(element.Name);
                element.ColumnName = oldSimpleAttribute.ColumnName;
                element.ID = oldSimpleAttribute.ID;
            }
		}

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		public void VisitRelationshipAttributeElement(RelationshipAttributeElement element)
		{
            if (!_nonConditionalCompare && !element.OwnerClass.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

			ClassElement oldClassElement = null;
			IMetaDataAction action;
			bool isFKAdded = false;

			if (_oldMetaDataModel == null ||
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
				oldClassElement.FindRelationshipAttribute(element.Name) == null)
			{
				if (oldClassElement == null)
				{
					// this is a brand new class, there must be a AddClassAction
					// created which will take care of adding foreign key
					// for the relationship attributes
					isFKAdded = true;
				}

				action = new AddRelationshipAttributeAction(_newMetaDataModel, element, _dataProvider);
				_result.AddAddRelationshipAttributeAction(action);

				if (element.IsForeignKeyRequired)
				{
					if (!isFKAdded)
					{
						// Add a foreign key column if the relationship is many-to-one
						// or one-to-one and not join manager
						action = new AddRelationshipAttributeFKAction(_newMetaDataModel, element, _dataProvider);
						_result.AddAlterRelationshipAttributeAction(action);
					}

					action = new AddRelationshipAttributeFKConstraintAction(_newMetaDataModel, element, _dataProvider);
					_result.AddAlterRelationshipAttributeAction(action);

                    if (element.IsIndexed)
                    {
                        // Add a index if the relationship is many-to-one
                        // or one-to-one and not join manager
                        action = new AddRelationshipAttributeIndexAction(_newMetaDataModel, element, _dataProvider);
                        _result.AddAlterRelationshipAttributeAction(action);
                    }
				}
			}
            else
            {
                oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name);
                RelationshipAttributeElement oldElement = oldClassElement.FindRelationshipAttribute(element.Name);
                element.ColumnName = oldElement.ColumnName;
                element.ID = oldElement.ID;
            }
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
            if (!_nonConditionalCompare && !element.OwnerClass.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

			ClassElement oldClassElement = null;
			bool skipDDLExecution = false;

			if (_oldMetaDataModel == null ||
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
				oldClassElement.FindArrayAttribute(element.Name) == null)
			{
				if (oldClassElement == null)
				{
					// this is a brand new class, there must be a AddClassAction
					// created which will take care of adding simple attributes
					skipDDLExecution = true;
				}

				// it is an added array attribute
				IMetaDataAction action = new AddArrayAttributeAction(_newMetaDataModel, element, _dataProvider);
				action.SkipDDLExecution = skipDDLExecution;
				_result.AddAddArrayAttributeAction(action);
			}
            else
            {
                oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name);
                ArrayAttributeElement oldElement = oldClassElement.FindArrayAttribute(element.Name);
                element.ColumnName = oldElement.ColumnName;
                element.ID = oldElement.ID;
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
            if (!_nonConditionalCompare && !element.OwnerClass.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

            ClassElement oldClassElement = null;
            bool skipDDLExecution = false;

            if (_oldMetaDataModel == null ||
                (oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null ||
                oldClassElement.FindImageAttribute(element.Name) == null)
            {
                if (oldClassElement == null)
                {
                    // this is a brand new class, there must be a AddClassAction
                    // created which will take care of adding simple attributes
                    skipDDLExecution = true;
                }

                // it is an added array attribute
                IMetaDataAction action = new AddImageAttributeAction(_newMetaDataModel, element, _dataProvider);
                action.SkipDDLExecution = skipDDLExecution;
                _result.AddAddImageAttributeAction(action);
            }
            else
            {
                oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name);
                ImageAttributeElement oldElement = oldClassElement.FindImageAttribute(element.Name);
                element.ColumnName = oldElement.ColumnName;
                element.ID = oldElement.ID;
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
            if (_oldMetaDataModel == null)
            {
                // it is new schema
                IMetaDataAction action = new AddSchemaAction(_newMetaDataModel, element, _dataProvider);
                _result.AddSchema = action;
            }
            else
            {
                element.ID = _oldMetaDataModel.SchemaModel.SchemaInfo.ID;
            }
		}

		/// <summary>
		/// Viste an enum constraint element.
		/// </summary>
		/// <param name="element">A EnumElement instance</param>
		public void VisitEnumElement(EnumElement element)
		{
            if (_oldMetaDataModel != null)
            {
                // make sure that new enum constraint contains all values in the old enum constraint
                ConstraintElementBase constraint = _oldMetaDataModel.SchemaModel.FindConstraint(element.Name);
                if (constraint != null && constraint is EnumElement)
                {
                    EnumElement oldEnumElement = (EnumElement)constraint;
                    // if new enum does not have all the values of the old enum element,
                    // the Compare method will return -1
                    if (EnumElement.Compare(element, oldEnumElement) < 0)
                    {
                        _result.AddMismatchedEnumElement(element);
                    }
                }
            }
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
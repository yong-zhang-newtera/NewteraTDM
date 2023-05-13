/*
* @(#)FindAlterationVisitor.cs
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
	/// Traverse the new meta data and find altered elements, such as
	/// ClassElement, SimpleAttributeElement, RelationshipAttributeElement, and
	/// EnumElement etc.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class FindAlterationVisitor : ISchemaModelElementVisitor
	{
		private MetaDataModel _newMetaDataModel;
		private MetaDataModel _oldMetaDataModel;
		private MetaDataCompareResult _result;
		private IDataProvider _dataProvider;
        private bool _nonConditionalCompare;

		/// <summary>
		/// Instantiate an instance of FindAlterationVisitor class
		/// </summary>
		/// <param name="newMetaDataModel">The new meta data model</param>
		/// <param name="oldMetaDataModel">The old meta data model</param>
		/// <param name="result">The compare result</param>
		public FindAlterationVisitor(MetaDataModel newMetaDataModel,
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
        /// Instantiate an instance of FindAlterationVisitor class
        /// </summary>
        /// <param name="newMetaDataModel">The new meta data model</param>
        /// <param name="oldMetaDataModel">The old meta data model</param>
        /// <param name="result">The compare result</param>
        public FindAlterationVisitor(MetaDataModel newMetaDataModel,
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
			ClassElement oldClassElement;

            if (!_nonConditionalCompare && !element.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.Name)) != null)
			{
				IMetaDataAction action;

				if (IsInheritanceDifferent(element, oldClassElement))
				{
					if (!oldClassElement.IsRoot)
					{
						// delete old inheritance first
						action = new DeleteClassInheritanceAction(_oldMetaDataModel, oldClassElement, _dataProvider);
						_result.AddAlterClassAction(action);
					}

					if (!element.IsRoot)
					{
						action = new AddClassInheritanceAction(_newMetaDataModel, element, _dataProvider);
						_result.AddAlterClassAction(action);
					}
				}

				// check to see if there is a difference in user-defined
				// primary keys
				if (IsPKDifferent(element, oldClassElement))
				{
					if (oldClassElement.PrimaryKeys.Count > 0)
					{
						// drop the old primary key constraint first
						action = new DeleteClassPKConstraintAction(_oldMetaDataModel, oldClassElement, _dataProvider);
						_result.AddAlterClassAction(action);
					}

					if (element.PrimaryKeys.Count > 0)
					{
						// add the new primary key constraint
						action = new AddClassPKConstraintAction(_newMetaDataModel, element, _dataProvider);
						
                        // add the action to the alter class list so that it wil
						// be executed after the primary key(s) is added and constraints are deleted.
						_result.AddAlterClassAction(action);
					}
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
			IMetaDataAction action;
			bool needModify = false;
			ModifyFlag flag = ModifyFlag.None;

            if (!_nonConditionalCompare && !element.OwnerClass.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) != null &&
				(oldAttribute = oldClassElement.FindSimpleAttribute(element.Name)) != null)
			{
				// A set of changes are taken care by ModifySimpleAttributeAction
				if (element.DataType != oldAttribute.DataType)
				{
					needModify = true;
					flag = flag | ModifyFlag.DataType;
				}

				if (element.MaxLength != oldAttribute.MaxLength)
				{
					needModify = true;
					flag = flag | ModifyFlag.MaxLength;
				}

				if (element.MinLength != oldAttribute.MinLength)
				{
					needModify = true;
					flag = flag | ModifyFlag.MinLength;
				}

                /*
				if (element.DefaultValue != oldAttribute.DefaultValue)
				{
					needModify = true;
					flag = flag | ModifyFlag.DefaultValue;
				}
                */

				if (needModify)
				{
					action = new ModifySimpleAttributeAction(_newMetaDataModel, element, _dataProvider, flag);
					_result.AddAlterSimpleAttributeAction(action);
				}

				if (element.IsAutoIncrement != oldAttribute.IsAutoIncrement)
				{
                    if (element.IsAutoIncrement && !element.HasCustomValueGenerator)
					{
						action = new AddSimpleAttributeAutoIncrementAction(_newMetaDataModel, element, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}
					else if (!element.IsAutoIncrement && !element.HasCustomValueGenerator)
					{
						action = new DeleteSimpleAttributeAutoIncrementAction(_oldMetaDataModel, oldAttribute, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}
				}

                if (element.IsRichText != oldAttribute.IsRichText)
                {
                    if (element.IsRichText && element.DataType == DataType.Text)
                    {
                        // rich text works on the attribute of Text type
                        action = new AddSimpleAttributeRichTextAction(_newMetaDataModel, element, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }
                    else if (oldAttribute.DataType == DataType.Text)
                    {
                        action = new DeleteSimpleAttributeRichTextAction(_oldMetaDataModel, oldAttribute, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }
                }

                if (element.IsHistoryEdit != oldAttribute.IsHistoryEdit)
                {
                    if (element.IsHistoryEdit && element.DataType == DataType.Text)
                    {
                        // history edit works on the attribute of Text type
                        action = new AddSimpleAttributeHistoryEditAction(_newMetaDataModel, element, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }
                    else if (oldAttribute.DataType == DataType.Text)
                    {
                        action = new DeleteSimpleAttributeHistoryEditAction(_oldMetaDataModel, oldAttribute, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }
                }

				if (element.IsIndexed != oldAttribute.IsIndexed)
				{
					if (element.IsIndexed && !element.IsUnique && !element.IsPrimaryKey)
					{
						// If the element is unique or primary key, it is indexed automatically
						action = new AddSimpleAttributeIndexAction(_newMetaDataModel, element, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}
					else if (!element.IsIndexed && !oldAttribute.IsUnique && !oldAttribute.IsPrimaryKey)
					{
						// delete an index
						action = new DeleteSimpleAttributeIndexAction(_oldMetaDataModel, oldAttribute, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}
				}

				if (element.IsUnique != oldAttribute.IsUnique)
				{
					if (element.IsUnique && !element.IsPrimaryKey)
					{
						// when the element is part of primary key, it's been uniquely
						// constrainted.
						action = new AddSimpleAttributeUniqueConstraintAction(_newMetaDataModel, element, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}
					else if (!element.IsUnique && !oldAttribute.IsPrimaryKey)
					{
						action = new DeleteSimpleAttributeUniqueConstraintAction(_oldMetaDataModel, oldAttribute, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}
				}

                // delete the unique constraint of an attribute if it is part of primary key(s) and
                // it was set to be unique
                if (element.IsPrimaryKey && oldAttribute.IsUnique && !oldAttribute.IsPrimaryKey)
                {
                    action = new DeleteSimpleAttributeUniqueConstraintAction(_oldMetaDataModel, oldAttribute, _dataProvider);
                    _result.AddAlterSimpleAttributeAction(action);
                }

                if (element.DefaultValue != oldAttribute.DefaultValue)
                {
                    if (string.IsNullOrEmpty(oldAttribute.DefaultValue))
                    {
                        // add a default value constraint to the column
                        action = new AddSimpleAttributeDefaultValueAction(_newMetaDataModel, element, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }
                    else if (!element.IsAutoIncrement && string.IsNullOrEmpty(element.DefaultValue))
                    {
                        // drop the default value constraint
                        action = new DeleteSimpleAttributeDefaultValueAction(_oldMetaDataModel, oldAttribute, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }
                    else
                    {
                        // drop the old default value and add the new default value
                        action = new DeleteSimpleAttributeDefaultValueAction(_oldMetaDataModel, oldAttribute, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);

                        if (!element.IsAutoIncrement)
                        {
                            action = new AddSimpleAttributeDefaultValueAction(_newMetaDataModel, element, _dataProvider);
                            _result.AddAlterSimpleAttributeAction(action);
                        }
                    }
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
			IMetaDataAction action;
			bool needModify = false;
			ModifyFlag flag = ModifyFlag.None;

            if (!_nonConditionalCompare && !element.OwnerClass.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) != null &&
				(oldAttribute = oldClassElement.FindRelationshipAttribute(element.Name)) != null)
			{
                /* IsRequired is enforced at UI level, therefore, there is no need to modify at database
				if (element.IsRequired != oldAttribute.IsRequired)
				{
					// isRequired status is changed
					needModify = true;
					flag = flag | ModifyFlag.IsRequired;
				}
                 */

				if (needModify)
				{
					action = new ModifyRelationshipAttributeAction(_newMetaDataModel, element, _dataProvider, flag);
					_result.AddAlterRelationshipAttributeAction(action);
				}

				if (element.LinkedClass.Name != oldAttribute.LinkedClass.Name || element.Type != oldAttribute.Type)
				{
					// Linked class is changed or the relationship type is changed
					ChangeRelationship(element, oldAttribute);
				}
				else if (element.Ownership != oldAttribute.Ownership)
				{
					// Only ownership is changed
					ChangeRelationshipOwnership(element, oldAttribute);
				}

                if (element.IsForeignKeyRequired && element.IsIndexed != oldAttribute.IsIndexed)
                {
                    if (element.IsIndexed)
                    {
                        // add an index
                        action = new AddRelationshipAttributeIndexAction(_newMetaDataModel, element, _dataProvider);
                        _result.AddAlterRelationshipAttributeAction(action);
                    }
                    else if (!element.IsIndexed)
                    {
                        // delete an index
                        action = new DeleteRelationshipAttributeIndexAction(_oldMetaDataModel, oldAttribute, _dataProvider);
                        _result.AddAlterRelationshipAttributeAction(action);
                    }
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
			IMetaDataAction action;
			bool needModify = false;
			ModifyFlag flag = ModifyFlag.None;

            if (!_nonConditionalCompare && !element.OwnerClass.NeedToAlter)
            {
                // elemnt is in new meta data
                return;
            }

			if (_oldMetaDataModel != null &&
				(oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) != null &&
				(oldAttribute = oldClassElement.FindArrayAttribute(element.Name)) != null)
			{
				// A set of changes are taken care by ModifySimpleAttributeAction
				if (element.IsRequired != oldAttribute.IsRequired)
				{
					needModify = true;
					flag = flag | ModifyFlag.IsRequired;
				}

				if (needModify)
				{
					action = new ModifyArrayAttributeAction(_newMetaDataModel, element, _dataProvider, flag);
					_result.AddAlterArrayAttributeAction(action);
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

		/// <summary>
		/// Gets the information indicating whether a two class has different
		/// parent classes.
		/// </summary>
		/// <param name="newClass">The new class</param>
		/// <param name="oldClass">The old class</param>
		/// <returns>true if two classes have different parent classes, false otherwise.</returns>
		private bool IsInheritanceDifferent(ClassElement newClass, ClassElement oldClass)
		{
			bool status = false;

			if (newClass.IsRoot != oldClass.IsRoot)
			{
				// one of them is root, another is not
				status = true;
			}
			else if (!newClass.IsRoot && newClass.ParentClass.Name != oldClass.ParentClass.Name)
			{
				// two classes have different parents.
				status = true;
			}

			return status;
		}

		/// <summary>
		/// Gets the information indicating whether a two class has different
		/// primary keys.
		/// </summary>
		/// <param name="newClass">The new class</param>
		/// <param name="oldClass">The old class</param>
		/// <returns>true if two classes have different primary keys, false otherwise.</returns>
		private bool IsPKDifferent(ClassElement newClass, ClassElement oldClass)
		{
			bool status = false;

			if (newClass.PrimaryKeys.Count != oldClass.PrimaryKeys.Count)
			{
				// two classes have different number of primary keys
				status = true;
			}
			else if (newClass.PrimaryKeys.Count > 0)
			{
				// compare each primary keys
				foreach (SimpleAttributeElement pk in newClass.PrimaryKeys)
				{
					if (!IsPKExist(pk, oldClass.PrimaryKeys))
					{
						status = true;
						break;
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Gets the information indicating whether a primary key exists in a
		/// collection.
		/// </summary>
		/// <param name="pk">The primary key</param>
		/// <param name="primaryKeys">The collection of primary keys</param>
		/// <returns>true if it exists, false otherwise.</returns>
		private bool IsPKExist(SimpleAttributeElement pk, SchemaModelElementCollection primaryKeys)
		{
			bool status = false;

			foreach (SimpleAttributeElement primaryKey in primaryKeys)
			{
				if (pk.Name == primaryKey.Name)
				{
					status = true;
					break;
				}
			}

			return status;
		}

		/// <summary>
		/// Change the relationship.
		/// </summary>
		/// <param name="newRelationship">The new relationship</param>
		/// <param name="oldRelationship">The old relationship</param>
		private void ChangeRelationship(RelationshipAttributeElement newRelationship,
			RelationshipAttributeElement oldRelationship)
		{
			IMetaDataAction action;

			if (oldRelationship.IsForeignKeyRequired)
			{
				// delete existing FK constraint
				action = new DeleteRelationshipAttributeFKConstraintAction(_oldMetaDataModel, oldRelationship, _dataProvider);
				_result.AddAlterRelationshipAttributeAction(action);

				// delete existing FK column
				action = new DeleteRelationshipAttributeFKAction(_oldMetaDataModel, oldRelationship, _dataProvider);
				_result.AddAlterRelationshipAttributeAction(action);
			}

			if (newRelationship.IsForeignKeyRequired)
			{
				// add an FK column to the table
				action = new AddRelationshipAttributeFKAction(_newMetaDataModel, newRelationship, _dataProvider);
				_result.AddAlterRelationshipAttributeAction(action);

				// add a FK constraint
				action = new AddRelationshipAttributeFKConstraintAction(_newMetaDataModel, newRelationship, _dataProvider);
				_result.AddAlterRelationshipAttributeAction(action);
			}
		}

		/// <summary>
		/// Change the ownership of a relationship.
		/// </summary>
		/// <param name="newRelationship">The new relationship</param>
		/// <param name="oldRelationship">The old relationship</param>
		private void ChangeRelationshipOwnership(RelationshipAttributeElement newRelationship,
			RelationshipAttributeElement oldRelationship)
		{
			IMetaDataAction action;

			if (newRelationship.IsForeignKeyRequired)
			{
				action = new DeleteRelationshipAttributeFKConstraintAction(_oldMetaDataModel, newRelationship, _dataProvider);
				_result.AddAlterRelationshipAttributeAction(action);

				action = new AddRelationshipAttributeFKConstraintAction(_newMetaDataModel, newRelationship, _dataProvider);
				_result.AddAlterRelationshipAttributeAction(action);
			}
		}
	}
}
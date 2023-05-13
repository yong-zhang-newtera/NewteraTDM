/*
* @(#)SchemaModelValidateVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Validate
{
	using System;
	using System.Resources;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Traverse a schema model and validate each element to check if it confirms
	/// to some schema model integrity rules.
	/// </summary>
	/// <version> 1.0.0 23 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class SchemaModelValidateVisitor : ISchemaModelElementVisitor
	{
		private const int STRING_MAX_LENGTH = 2000;
        private const int STRING_MIN_LENGTH = 1;
		private const int CAPTION_MAX_LENGTH = 100;
		private SchemaModel _schemaModel;
		private ValidateResult _result;
		private ResourceManager _resources;

		/// <summary>
		/// Instantiate an instance of SchemaModelValidateVisitor class
		/// </summary>
		/// <param name="schemaModel">The schema model being validated.</param>
		public SchemaModelValidateVisitor(SchemaModel schemaModel)
		{
			_schemaModel = schemaModel;
			_result = new ValidateResult();
			_resources = new ResourceManager(this.GetType());
		}

		/// <summary>
		/// Gets the validate result.
		/// </summary>
		/// <value>The validate result in ValidateResult object</value>
		public ValidateResult ValidateResult
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Get a localized error message for the given message id
		/// </summary>
		/// <param name="msgId">The message id</param>
		/// <returns>The localized error message</returns>
		public string GetMessage(string msgId)
		{
			return _resources.GetString(msgId);
		} 

		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		public void VisitClassElement(ClassElement element)
		{
			ValidateResultEntry entry;

			if (element.Caption.Length > SchemaModelValidateVisitor.CAPTION_MAX_LENGTH)
			{
				entry = new ValidateResultEntry(_resources.GetString("Class.CaptionTooLong"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

            if (element.IsJunction)
            {
                if (element.ParentClass != null || element.Subclasses.Count > 0)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Class.NoRootJunction"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (element.RelationshipAttributes.Count != 2 ||
                    ((RelationshipAttributeElement)element.RelationshipAttributes[0]).Type != RelationshipType.ManyToOne ||
                    ((RelationshipAttributeElement)element.RelationshipAttributes[0]).Type != RelationshipType.ManyToOne)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Class.TwoManyToOneRelationship"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (element.PrimaryKeys.Count > 0)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Class.JunctionNoPK"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (element.UniqueKeys.Count > 0)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Class.JunctionNoUC"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
            }

			// ensure there is one or zero auto-increment attribute
			int autoAttributeCount = 0;
            int resultAttributeCount = 0;
			ClassElement currentCls = element;
			while (currentCls != null)
			{
				foreach (SimpleAttributeElement sAttribute in currentCls.SimpleAttributes)
				{
					if (sAttribute.IsAutoIncrement && !sAttribute.HasCustomValueGenerator)
					{
                        autoAttributeCount++;
					}

                    if (sAttribute.Usage == DefaultViewUsage.Included)
                    {
                        resultAttributeCount++;
                    }
				}

                foreach (RelationshipAttributeElement rAttribute in currentCls.RelationshipAttributes)
                {
                    if (rAttribute.Usage == DefaultViewUsage.Included)
                    {
                        resultAttributeCount++;
                    }
                }

                foreach (VirtualAttributeElement vAttribute in currentCls.VirtualAttributes)
                {
                    if (vAttribute.Usage == DefaultViewUsage.Included)
                    {
                        resultAttributeCount++;
                    }
                }

                foreach (RelationshipAttributeElement rAttribute in currentCls.RelationshipAttributes)
                {
                    if (rAttribute.IsForeignKeyRequired)
                    {
                        resultAttributeCount++; // for the joined class for many-to-many relatioship
                    }
                }

				currentCls = currentCls.ParentClass;
			}

            if (autoAttributeCount > 1)
			{
                entry = new ValidateResultEntry(_resources.GetString("Class.TooManyAutoIncAttributes"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

            if (resultAttributeCount == 0)
            {
                entry = new ValidateResultEntry(_resources.GetString("Class.NoResultAttributes"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            /*
            if (element.UniqueKeys.Count > 0)
            {
                // make sure that attributes defined for unique keys have proper usage
                foreach (SimpleAttributeElement simpleAttribute in element.UniqueKeys)
                {
                    if (simpleAttribute.Usage != AttributeUsage.Both && simpleAttribute.Usage != AttributeUsage.Search)
                    {
                        entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidUsageForUniqueKey"), MetaDataValidateHelper.Instance.GetSource(simpleAttribute), EntryType.Error, simpleAttribute);
                        _result.AddError(entry);
                    }
                }
            }
             */
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{
			ValidateResultEntry entry;
            bool isMultilpleEnum = false;

			if (element.DataType == DataType.Unknown)
			{
                entry = new ValidateResultEntry(_resources.GetString("Simple.UnknownDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

            if (element.IsFullTextSearchAttribute)
            {
                if (element.DataType != DataType.String)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.WrongFullTextSearchType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
            }

            if ((element.DataType == DataType.Date || element.DataType == DataType.DateTime) &&
                !string.IsNullOrEmpty(element.DefaultValue))
            {
                if (!element.IsSystemTimeDefault)
                {
                    try
                    {
                        DateTime parsedValue = DateTime.Parse(element.DefaultValue);
                    }
                    catch (Exception)
                    {
                        entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidDefaultDateTimeValue"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                        _result.AddError(entry);
                    }
                }
            }

            if (element.IsHistoryEdit && element.Usage != DefaultViewUsage.Excluded)
            {
                entry = new ValidateResultEntry(_resources.GetString("Simple.HistoryEditInvalidForSearch"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            if (element.IsRichText && element.Usage != DefaultViewUsage.Excluded)
            {
                entry = new ValidateResultEntry(_resources.GetString("Simple.RichTextInvalidForSearch"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            if (element.Caption.Length > SchemaModelValidateVisitor.CAPTION_MAX_LENGTH)
			{
                entry = new ValidateResultEntry(_resources.GetString("Simple.CaptionTooLong"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

			if (element.Constraint != null)
			{
				ConstraintElementBase constraint = _schemaModel.FindConstraint(element.Constraint.Name);
				if (constraint == null)
				{
                    entry = new ValidateResultEntry(_resources.GetString("Simple.UnknownConstraint"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
					_result.AddError(entry);
				}
				else if (constraint is EnumElement)
				{
					EnumElement enumConstraint = (EnumElement) constraint;
                    if (enumConstraint.IsMultipleSelection)
                    {
                        isMultilpleEnum = true;
                        if (enumConstraint.DataType != DataType.Integer)
                        {
                            entry = new ValidateResultEntry(_resources.GetString("Simple.IncorrectEnumDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                            _result.AddError(entry);
                        }
                    }
				}
			}

            if (element.IsAutoIncrement)
            {
                // the data type of default auto-value generator must be integer
                if (!element.HasCustomValueGenerator && element.DataType != DataType.Integer)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidAutoIncType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (element.HasCustomValueGenerator && element.DataType != DataType.String)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidCustomAutoIncType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (element.HasCustomValueGenerator)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidValueGenerator"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddDoubt(entry);
                }
            }

			if (element.DataType == DataType.String)
			{
                if (element.MaxLength < STRING_MIN_LENGTH)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.StringLengthTooShort") + STRING_MIN_LENGTH, MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (element.MaxLength > STRING_MAX_LENGTH)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.StringLengthTooBig") + STRING_MAX_LENGTH, MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }

                if (element.MinLength < 0)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidStringMinLength"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (element.MinLength >= element.MaxLength)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidStringMinLength"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
			}

            if (!string.IsNullOrEmpty(element.InputMask) &&
                element.IsEncrypted)
            {
                entry = new ValidateResultEntry(_resources.GetString("Simple.InputMaskNotAllowedEncrypted"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            if (element.IsHistoryEdit)
            {
                if (element.DataType != DataType.Text)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.WrongHistoryEditType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Warning, element);
                    _result.AddError(entry);
                }

                // make sure that the attribute is not auto-incremental
                if (element.IsAutoIncrement)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidAutoIncForHistoryEdit"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }

                // make sure that the attribute is not unqiue
                if (element.IsUnique)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidUniqueForHistoryEdit"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }

                // make sure that the attribute has no default value
                if (!string.IsNullOrEmpty(element.DefaultValue))
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidDefaultForHistoryEdit"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
            }

            if (element.InlineEditEnabled)
            {
                // attribute with unique values cannot be used in inline editing
                if (element.IsUnique)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InlineEditInvalidUniqueAttribute"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }

                // attribute with auto-incremental values cannot be used in inline editing
                if (element.IsAutoIncrement)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InlineEditInvalidAutoAttribute"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }

                // attribute with enum constraint of multiple choices cannot be used in inline editing
                if (isMultilpleEnum)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.InlineEditInvalidMultiEnumAttribute"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
            }

            if (!string.IsNullOrEmpty(element.CascadedAttributes))
            {
                // verify that the cascaded attributes has conditional List constraint
                string[] cascadedAttributeNames = element.CascadedAttributes.Split(';');

                ClassElement ownerClass = element.OwnerClass;
                foreach (string cascadedAttributeName in cascadedAttributeNames)
                {
                    SimpleAttributeElement cascadedAttribute = ownerClass.FindInheritedSimpleAttribute(cascadedAttributeName);
                    /*
                    if (cascadedAttribute == null)
                    {
                        entry = new ValidateResultEntry(_resources.GetString("Simple.CascadedAttributeNotFound") + "" + cascadedAttributeName, MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                        _result.AddError(entry);
                    }
                     */
                    if (cascadedAttribute != null &&
                        (cascadedAttribute.Constraint == null ||
                        !(cascadedAttribute.Constraint is ListElement) ||
                        !((ListElement) cascadedAttribute.Constraint).IsConditionBased))
                    {
                        entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidCascadedAttributeConstraint") + "" + cascadedAttributeName, MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                        _result.AddError(entry);
                    }

                    if (cascadedAttribute != null &&
                        cascadedAttribute.Constraint != null &&
                        cascadedAttribute.Constraint is ListElement &&
                        ((ListElement)cascadedAttribute.Constraint).IsConditionBased &&
                        !string.IsNullOrEmpty(cascadedAttribute.ParentAttribute))
                    {
                        entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidUseOfCascadeAttribute") + "" + cascadedAttributeName, MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                        _result.AddError(entry);
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
			ValidateResultEntry entry;

			if (element.Caption.Length > SchemaModelValidateVisitor.CAPTION_MAX_LENGTH)
			{
                entry = new ValidateResultEntry(_resources.GetString("Relationship.CaptionTooLong"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

			if (element.IsForeignKeyRequired && element.LinkedClass.RootClass.PrimaryKeys.Count == 0)
			{
                entry = new ValidateResultEntry(_resources.GetString("Relationship.MissingPrimaryKeys"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

			// make sure the linked class exists
			if (_schemaModel.FindClass(element.LinkedClass.Name) == null)
			{
                entry = new ValidateResultEntry(_resources.GetString("Relationship.MissingLinkedClass"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
			ValidateResultEntry entry;

			if (element.ElementDataType == DataType.Unknown)
			{
                entry = new ValidateResultEntry(_resources.GetString("Array.UnknownElementDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

			if (element.Caption.Length > SchemaModelValidateVisitor.CAPTION_MAX_LENGTH)
			{
                entry = new ValidateResultEntry(_resources.GetString("Array.CaptionTooLong"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

			if (element.Dimension < 1 && element.Dimension > 2)
			{
                entry = new ValidateResultEntry(_resources.GetString("Array.IncorrectDimension"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

            if (element.ColumnTitles.Count > 1)
            {
                bool isDuplicate = false;

                // make sure the column names are different
                for (int i = 0; i < element.ColumnTitles.Count; i++)
                {
                    for (int j = i + 1; j < element.ColumnTitles.Count; j++)
                    {
                        if (element.ColumnTitles[i] == element.ColumnTitles[j])
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                }

                if (isDuplicate)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Array.DuplicatedColumnTitle"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
            }
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A VirtualAttributeElement instance</param>
        public void VisitVirtualAttributeElement(VirtualAttributeElement element)
        {
            ValidateResultEntry entry;

            if (string.IsNullOrEmpty(element.Code))
            {
                entry = new ValidateResultEntry(_resources.GetString("Virtual.UnspecifiedCode"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            if (element.DataType == DataType.Unknown)
            {
                entry = new ValidateResultEntry(_resources.GetString("Virtual.UnknownDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            if (element.Caption.Length > SchemaModelValidateVisitor.CAPTION_MAX_LENGTH)
            {
                entry = new ValidateResultEntry(_resources.GetString("Simple.CaptionTooLong"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            if (element.Constraint != null)
            {
                ConstraintElementBase constraint = _schemaModel.FindConstraint(element.Constraint.Name);
                if (constraint == null)
                {
                    entry = new ValidateResultEntry(_resources.GetString("Simple.UnknownConstraint"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                    _result.AddError(entry);
                }
                else if (constraint is EnumElement)
                {
                    EnumElement enumConstraint = (EnumElement)constraint;
                    if (enumConstraint.IsMultipleSelection &&
                        enumConstraint.DataType != DataType.Integer)
                    {
                        entry = new ValidateResultEntry(_resources.GetString("Simple.IncorrectEnumDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                        _result.AddError(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A ImageAttributeElement instance</param>
        public void VisitImageAttributeElement(ImageAttributeElement element)
        {
        }

        /// <summary>
        /// Viste a custom page element.
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
			ValidateResultEntry entry;

			if (element.Values.Count == 0)
			{
                entry = new ValidateResultEntry(_resources.GetString("Enum.MissingValues"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

			if (element.DataType == DataType.Unknown)
			{
                entry = new ValidateResultEntry(_resources.GetString("Simple.UnknownDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

            if (element.IsMultipleSelection && element.DataType != DataType.Integer)
            {
                entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidEnumDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }

            if (!element.IsMultipleSelection && element.DataType != DataType.String)
            {
                entry = new ValidateResultEntry(_resources.GetString("Simple.InvalidSingleEnumDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
                _result.AddError(entry);
            }
		}

		/// <summary>
		/// Viste a range constraint element.
		/// </summary>
		/// <param name="element">A RangeElement instance</param>
		public void VisitRangeElement(RangeElement element)
		{
			ValidateResultEntry entry;

			if (System.Convert.ToDouble(element.MinValue) > System.Convert.ToDouble(element.MaxValue))
			{
                entry = new ValidateResultEntry(_resources.GetString("Range.InvalidMinMax"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}
		}

		/// <summary>
		/// Viste a pattern constraint element.
		/// </summary>
		/// <param name="element">A PatternElement instance</param>
		public void VisitPatternElement(PatternElement element)
		{
			ValidateResultEntry entry;

			if (element.PatternValue == null)
			{
                entry = new ValidateResultEntry(_resources.GetString("Pattern.MissingValue"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}
		}

		/// <summary>
		/// Viste a list constraint element.
		/// </summary>
		/// <param name="element">A ListElement instance</param>
		public void VisitListElement(ListElement element)
		{
			ValidateResultEntry entry;

			if (element.ListHandlerName == null)
			{
                entry = new ValidateResultEntry(_resources.GetString("List.MissingValue"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}

			if (element.DataType == DataType.Unknown)
			{
                entry = new ValidateResultEntry(_resources.GetString("Simple.UnknownDataType"), MetaDataValidateHelper.Instance.GetSource(element), EntryType.Error, element);
				_result.AddError(entry);
			}
		}

		/// <summary>
		/// Gets the information indicating whether the given type is one of
		/// the numeric type
		/// </summary>
		/// <param name="type">The data type</param>
		/// <returns>true if it is a numeric type, false otherwise.</returns>
		private bool IsNumericType(DataType type)
		{
			bool status = false;

			switch (type)
			{
				case DataType.Decimal:
				case DataType.Double:
				case DataType.Float:
				case DataType.Integer:
				case DataType.BigInteger:
					status = true;
					break;
			}

			return status;
		}
	}
}
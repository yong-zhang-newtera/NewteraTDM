/*
* @(#)InstanceDataValidateVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Validate
{
	using System;
	using System.Resources;
    using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData.Schema.Generator;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Represents a visitor that validate an instance data kept in result attributes
	/// of a data view. The validating rules are based on that provided by the
	/// instance's schema. 
	/// </summary>
	/// <version> 1.0.0 04 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	public class InstanceDataValidateVisitor : IDataViewElementVisitor
	{
		private DataViewModel _dataView;
		private DataViewValidateResult _result;
		private ResourceManager _resources;
        private Hashtable _formulaTable;

		/// <summary>
		/// Initiate an instance of InstanceDataValidateVisitor class
		/// </summary>
		public InstanceDataValidateVisitor(DataViewModel dataView)
		{
			_dataView = dataView;
			_result = new DataViewValidateResult();
			_resources = new ResourceManager(this.GetType());
            _formulaTable = new Hashtable();
		}

		/// <summary>
		/// Gets the validate result
		/// </summary>
		/// <value>A DataViewValidateResult</value>
		public DataViewValidateResult ValidateResult
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Viste a data view element.
		/// </summary>
		/// <param name="element">A DataViewModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitDataView(DataViewModel element)
		{
			return true;
		}

		/// <summary>
		/// Viste a data class element.
		/// </summary>
		/// <param name="element">A DataClass instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitDataClass(DataClass element)
		{
            DataValidateResultEntry entry = null;
            ClassElement currentClass = (ClassElement)element.GetSchemaModelElement();

            // search the unique constraints including inherited ones
            while (currentClass != null)
            {
                if (currentClass.UniqueKeys.Count > 0)
                {
                    string attributeCaptions = null;
                    foreach (AttributeElementBase uk in currentClass.UniqueKeys)
                    {
                        if (attributeCaptions == null)
                        {
                            attributeCaptions = uk.Caption;
                        }
                        else
                        {
                            attributeCaptions += ", " + uk.Caption;
                        }
                    }

                    string msg = string.Format(_resources.GetString("UniqueValuesRequired"), attributeCaptions);
                    entry = new DataValidateResultEntry(msg, GetSource(element), element, EntryType.UniqueValues);
                    entry.ClassName = currentClass.Name;
                    _result.AddDoubt(entry);
                }

                currentClass = currentClass.ParentClass;
            }
			
			return true;
		}

		/// <summary>
		/// Viste a filter element.
		/// </summary>
		/// <param name="element">A Filter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitFilter(Filter element)
		{
			return true;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A DataSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(DataSimpleAttribute element)
		{
			DataValidateResultEntry entry = null;
			SimpleAttributeElement schemaModelElement = (SimpleAttributeElement) element.GetSchemaModelElement();
			
			// check the IsRequired rule
			if (schemaModelElement.IsRequired &&
				!schemaModelElement.IsAutoIncrement &&
				!element.HasValue)
			{
				entry = new DataValidateResultEntry(_resources.GetString("ValueRequired"), GetSource(element), element);
				_result.AddError(entry);
			}

            if (IsAnUniqueKey(schemaModelElement) &&
                !element.HasValue)
            {
                entry = new DataValidateResultEntry(_resources.GetString("UniqueKeyValueRequired"), GetSource(element), element);
                _result.AddError(entry);
            }

            if (schemaModelElement.IsUnique &&
                !schemaModelElement.IsAutoIncrement &&
                !schemaModelElement.IsPrimaryKey &&
                !schemaModelElement.IsHistoryEdit &&
                !schemaModelElement.IsRichText &&
                schemaModelElement.IsRequired)
            {
                entry = new DataValidateResultEntry(_resources.GetString("UniqueValueRequired"), GetSource(element), element, EntryType.UniqueValue);
                _result.AddDoubt(entry);
            }

			// Check max length rule
			if (schemaModelElement.DataType == DataType.String && element.HasValue && element.AttributeValue.Length > schemaModelElement.MaxLength)
			{
                entry = new DataValidateResultEntry(_resources.GetString("ValueLengthTooLong") + schemaModelElement.MaxLength, GetSource(element), element);
				_result.AddError(entry);
			}

            // Check min length rule
            if (schemaModelElement.DataType == DataType.String && element.HasValue && element.AttributeValue.Length < schemaModelElement.MinLength)
            {
                entry = new DataValidateResultEntry(_resources.GetString("ValueLengthTooShort") + schemaModelElement.MinLength, GetSource(element), element);
                _result.AddError(entry);
            }

            // if primary is not an auto-incremental attribute
            // we need to make sure that the primary key values are unique
            // in the database. Adding a doubt so that the host can perform
            // a database query to check the uniqueness constraint
            if (schemaModelElement.IsPrimaryKey && !schemaModelElement.IsAutoIncrement)
            {
                entry = new DataValidateResultEntry(_resources.GetString("PKValueExists"), GetSource(element), element, EntryType.PrimaryKey);
                _result.AddDoubt(entry);
            }

			bool CheckValueType = true;
            if (schemaModelElement.IsAutoIncrement)
            {
                CheckValueType = false;
            }

			// check constraint rule
			if (schemaModelElement.Constraint != null)
			{
				if (element.HasValue)
				{
					if (!schemaModelElement.Constraint.IsValueValid(element.AttributeValue))
					{
						string errMsg = null;
						if (schemaModelElement.Constraint.ErrorMessage != null &&
							schemaModelElement.Constraint.ErrorMessage.Length > 0)
						{
							// get the customized error message
							errMsg = schemaModelElement.Constraint.ErrorMessage;
						}
						
						if (schemaModelElement.Constraint is RangeElement)
						{
							entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("ValueOutOfRange")), GetSource(element), element);
						}
						else if (schemaModelElement.Constraint is PatternElement)
						{
							entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("BadPatternValue")), GetSource(element), element);
						}
						else if (schemaModelElement.Constraint is EnumElement)
						{
							entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("BadEnumValue")), GetSource(element), element);
						}
						else if (schemaModelElement.Constraint is ListElement)
						{
							entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("BadListValue")), GetSource(element), element);
						}

						if (entry != null)
						{
							_result.AddError(entry);
						}
					}
				}

				if (schemaModelElement.Constraint is EnumElement &&
					((EnumElement) schemaModelElement.Constraint).IsMultipleSelection)
				{
					CheckValueType = false; // Multiple selection is stored as integer value
				}
			}

			// Check to see if the value is valid for the data type of attribute
			if (CheckValueType &&
				!IsValidValueFormat(element.AttributeValue, schemaModelElement.DataType))
			{
				entry = new DataValidateResultEntry(_resources.GetString("InvalidValueFormat") + schemaModelElement.DataType.ToString(), GetSource(element), element);
				_result.AddError(entry);
			}

			// TODO, check uniqueness, this requires a database operation

			return true;
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A DataArrayAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitArrayAttribute(DataArrayAttribute element)
		{
			DataValidateResultEntry entry = null;
			ArrayAttributeElement schemaModelElement = (ArrayAttributeElement) element.GetSchemaModelElement();
			
			// check the IsRequired rule
			if (schemaModelElement.IsRequired &&
				!element.HasValue)
			{
				entry = new DataValidateResultEntry(_resources.GetString("ValueRequired"), GetSource(element), element);
				_result.AddError(entry);
			}

			// Check max length rule
			if (element.HasValue &&
				schemaModelElement.ArraySize == ArraySizeType.NormalSize &&
				element.AttributeValue.Length > ArrayAttributeElement.MAX_COLUMN_LENGTH)
			{
				entry = new DataValidateResultEntry(_resources.GetString("ValueLengthTooLong"), GetSource(element), element);
				_result.AddError(entry);
			}

			// check to see if the array values have correct format
			string[] arrayValues = element.AttributeValues;
			if (arrayValues != null)
			{
				for (int i = 0; i < arrayValues.Length; i++)
				{
					if (!IsValidValueFormat(arrayValues[i], schemaModelElement.ElementDataType))
					{
						entry = new DataValidateResultEntry(_resources.GetString("InvalidArrayValueFormat") + schemaModelElement.ElementDataType.ToString(), GetSource(element), element);
						_result.AddError(entry);
						break;
					}
				}
			}

			return true;
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A DataVirtualAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitVirtualAttribute(DataVirtualAttribute element)
        {
            DataValidateResultEntry entry = null;
            VirtualAttributeElement schemaModelElement = (VirtualAttributeElement)element.GetSchemaModelElement();

            // generate the value of the virtual attribute at the time of evaluation.
            // stop validation if there is an error while generating the value
            try
            {
                element.AttributeValue = this.GenerateValue(schemaModelElement);
            }
            catch (Exception)
            {
                return true;
            }

            bool CheckValueType = true;

            // check constraint rule
            if (schemaModelElement.Constraint != null)
            {
                if (element.HasValue)
                {
                    if (!schemaModelElement.Constraint.IsValueValid(element.AttributeValue))
                    {
                        string errMsg = null;
                        if (schemaModelElement.Constraint.ErrorMessage != null &&
                            schemaModelElement.Constraint.ErrorMessage.Length > 0)
                        {
                            // get the customized error message
                            errMsg = schemaModelElement.Constraint.ErrorMessage;
                        }

                        if (schemaModelElement.Constraint is RangeElement)
                        {
                            entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("ValueOutOfRange")), GetSource(element), element);
                        }
                        else if (schemaModelElement.Constraint is PatternElement)
                        {
                            entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("BadPatternValue")), GetSource(element), element);
                        }
                        else if (schemaModelElement.Constraint is EnumElement)
                        {
                            entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("BadEnumValue")), GetSource(element), element);
                        }
                        else if (schemaModelElement.Constraint is ListElement)
                        {
                            entry = new DataValidateResultEntry((errMsg != null ? errMsg : _resources.GetString("BadListValue")), GetSource(element), element);
                        }

                        if (entry != null)
                        {
                            _result.AddError(entry);
                        }
                    }
                }

                if (schemaModelElement.Constraint is EnumElement &&
                    ((EnumElement)schemaModelElement.Constraint).IsMultipleSelection)
                {
                    CheckValueType = false; // Multiple selection is stored as integer value
                }
            }

            // Check to see if the value is valid for the data type of attribute
            if (CheckValueType &&
                !IsValidValueFormat(element.AttributeValue, schemaModelElement.DataType))
            {
                entry = new DataValidateResultEntry(_resources.GetString("InvalidValueFormat") + schemaModelElement.DataType.ToString(), GetSource(element), element);
                _result.AddError(entry);
            }

            // TODO, check uniqueness, this requires a database operation

            return true;
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A DataImageAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitImageAttribute(DataImageAttribute element)
        {
            DataValidateResultEntry entry = null;
            ImageAttributeElement schemaModelElement = (ImageAttributeElement)element.GetSchemaModelElement();

            return true;
        }

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A DataRelationshipAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipAttribute(DataRelationshipAttribute element)
		{
			DataValidateResultEntry entry = null;
			RelationshipAttributeElement schemaModelElement = (RelationshipAttributeElement) element.GetSchemaModelElement();

			// check the IsRequired rule
			if (schemaModelElement.IsRequired && !element.HasValue)
			{
				entry = new DataValidateResultEntry(_resources.GetString("ValueRequired"), GetSource(element), element);
				_result.AddError(entry);
			}

            // if it is a one-to-one relationship, check the one-to-one constraint
            if (schemaModelElement.Type == RelationshipType.OneToOne &&
                !schemaModelElement.IsJoinManager)
            {
                // requires database operation, add as a doubt
                entry = new DataValidateResultEntry(_resources.GetString("UniqueReferenceRequired"), GetSource(element), element, EntryType.UniqueReference);
                _result.AddDoubt(entry);
            }

			return false;
		}

		/// <summary>
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ResultAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitResultAttributes(ResultAttributeCollection element)
		{
			return true;
		}

		/// <summary>
		/// Viste a referenced class Collection.
		/// </summary>
		/// <param name="element">A ReferencedClassCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitReferencedClasses(ReferencedClassCollection element)
		{
			return true;
		}

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A BinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(BinaryExpr element)
		{
			return true;
		}

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ParenthesizedExpr element)
		{
			return true;
		}

		/// <summary>
		/// Start visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipBegin(DataRelationshipAttribute element)
		{
			return true;
		}

		/// <summary>
		/// End visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipEnd(DataRelationshipAttribute element)
		{
			return true;
		}

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ParenthesizedExpr element)
		{
			return true;
		}

		/// <summary>
		/// Viste a search parameter.
		/// </summary>
		/// <param name="element">A Parameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(Parameter element)
		{
			return true;
		}

		/// <summary>
		/// Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ParameterCollection element)
		{
			return true;
		}

		/// <summary>
		/// End visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ParameterCollection element)
		{
			return true;
		}

		/// <summary>
		/// Visit a TaxonomyModel.
		/// </summary>
		/// <param name="element">A TaxonomyModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonomyModel(TaxonomyModel element)
		{
			return true;
		}

		/// <summary>
		/// Visit a TaxonNode.
		/// </summary>
		/// <param name="element">A TaxonNode instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonNode(TaxonNode element)
		{
			return true;
		}

		/// <summary>
		/// Visit a SortBy.
		/// </summary>
		/// <param name="element">A SortBy instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitSortBy(SortBy element)
		{
			return false;
		}

        /// <summary>
        /// Visit a SortAttribute.
        /// </summary>
        /// <param name="element">A SortAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSortAttribute(SortAttribute element)
        {
            return false;
        }

        /// <summary>
        /// Visit a function element.
        /// </summary>
        /// <param name="element">A function instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitFunction(IFunctionElement element)
        {
            return false;
        }

        /// <summary>
        /// Visit a AutoClassifyDef element.
        /// </summary>
        /// <param name="element">A AutoClassifyDef instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitClassifyDef(AutoClassifyDef element)
        {
            return false;
        }

        /// <summary>
        /// Visit a AutoClassifyLevel element.
        /// </summary>
        /// <param name="element">A AutoClassifyLevel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitClassifyLevel(AutoClassifyLevel element)
        {
            return false;
        }

		/// <summary>
		/// Gets the source string
		/// </summary>
		/// <param name="element">The data view element</param>
		/// <returns>A source string</returns>
		private string GetSource(IDataViewElement element)
		{
			string source = "";

			switch (element.ElementType)
			{
				case ElementType.SimpleAttribute:
				case ElementType.ArrayAttribute:
                case ElementType.VirtualAttribute:
                case ElementType.ImageAttribute:
					source = element.Caption;
					break;
				case ElementType.RelationshipAttribute:
					source = element.Caption;
					break;
                case ElementType.Class:
                    source = element.Caption;
                    break;
			}

			return source;
		}

        /// <summary>
        /// Generate value of a virtual attribute
        /// </summary>
        /// <param name="attributeEntity">The attribute entity</param>
        /// <returns>The generated value.</returns>
        private string GenerateValue(VirtualAttributeElement attributeElement)
        {
            string val = null;
            IFormula formula = (IFormula)_formulaTable[attributeElement];

            if (formula == null)
            {
                // create a formula instance from the source code
                formula = attributeElement.CreateFormula();

                if (formula == null)
                {
                    throw new Exception(_resources.GetString("UnableToGetAssembly"));
                }

                // keep it in the hashtable for reuse
                _formulaTable[attributeElement] = formula;
            }

            try
            {
                InstanceWrapper wrapper = new InstanceWrapper(_dataView);
                ExecutionContext context = new DefaultExecutionContext();
                context.Attribute = attributeElement;
                val = formula.Execute(wrapper, context);
            }
            catch (Exception ex)
            {
                string msg = String.Format(_resources.GetString("GetVirtualAttributeValue"), ex.Message);
                throw new SchemaModelException(msg);
            }

            return val;
        }

		/// <summary>
		/// Gets information indicating whether the attribute value is valid
		/// for the given data type
		/// </summary>
		/// <param name="val">The attribute value</param>
		/// <param name="dataType">One of the DataType enum</param>
		/// <returns>true if the value is valid, false otherwise</returns>
		private bool IsValidValueFormat(string val, DataType dataType)
		{
			bool status = true;

			if (val != null && val.Length > 0)
			{
				try
				{
					switch (dataType)
					{
						case DataType.BigInteger:
							Int64.Parse(val);
							break;
						case DataType.Boolean:
							if (val != LocaleInfo.Instance.True &&
								val != LocaleInfo.Instance.False)
							{
								throw new FormatException("Invalid boolean value");
							}
							break;
						case DataType.Byte:
							Byte.Parse(val);
							break;
						case DataType.Date:
						case DataType.DateTime:
							DateTime.Parse(val);
							break;
						case DataType.Decimal:
							Decimal.Parse(val);
							break;
						case DataType.Double:
							Double.Parse(val);
							break;
						case DataType.Float:
							Single.Parse(val);
							break;
						case DataType.Integer:
							Int32.Parse(val);
							break;
					}
				}
				catch (Exception)
				{
					status = false;
				}
			}

			return status;
		}

        /// <summary>
        /// Gets the information indicating whether the attribute is part of an
        /// unique constraint keys
        /// </summary>
        /// <param name="element">The simple attribute</param>
        /// <returns>true if it is part of an unique constrain keys, false otherwise.</returns>
        private bool IsAnUniqueKey(SimpleAttributeElement element)
        {
            bool status = false;

            ClassElement currentClass = (ClassElement) _dataView.BaseClass.GetSchemaModelElement();
            while (currentClass != null)
            {
                if (currentClass.UniqueKeys.Count > 0)
                {
                    foreach (AttributeElementBase uk in currentClass.UniqueKeys)
                    {
                        if (uk.Name == element.Name)
                        {
                            status = true;
                            break;
                        }
                    }
                }

                if (status)
                {
                    break;
                }
                else
                {
                    currentClass = currentClass.ParentClass;
                }
            }

            return status;
        }
	}
}
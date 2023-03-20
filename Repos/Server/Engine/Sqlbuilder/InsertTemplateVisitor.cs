/*
* @(#)InsertTemplateVisitor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
    using System.Data;

    using Newtera.Common.Core;
    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView.Taxonomy;
    using Newtera.Server.Engine.Sqlbuilder.Sql;
    using Newtera.Server.DB;
    using Newtera.Server.Engine.Vdom.Dbimp;

	/// <summary>
	/// Represents a DataView visitor that generates executable SQLs for inserts from a sql template and
    /// instance data stored in a data view elements
	/// </summary>
	/// <version> 1.0.0 03 Aug 2008 </version>
	public class InsertTemplateVisitor : IDataViewElementVisitor
	{
		private SQLActionCollection _sqlActions;
        private bool _visitingResultAttributes = false;
        private KeyGenerator _idGenerator; // the obj id generator
        private MetaDataModel _metaData;
        private DataViewModel _instanceDataView;
        private IDataProvider _dataProvider;
        private SQLBuilder _builder; // the sqlbuilder

		/// <summary>
		/// Initiate an instance of InsertTemplateVisitor class
		/// </summary>
		/// <param name="dataView">The data view</param>
        public InsertTemplateVisitor(MetaDataModel metaData, DataViewModel instanceDataView, SQLActionCollection sqlActions, IDataProvider dataProvider)
		{
            _metaData = metaData;
            _instanceDataView = instanceDataView;
            _sqlActions = sqlActions;
            _visitingResultAttributes = false;
            _dataProvider = dataProvider;
            _builder = new SQLBuilder(_metaData, _dataProvider);
            _idGenerator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.ObjId, _metaData.SchemaInfo);
		}

		/// <summary>
		/// Viste a data view element.
		/// </summary>
		/// <param name="element">A DataViewModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitDataView(DataViewModel element)
		{
            _visitingResultAttributes = false;

            // create an unique id for the data instance
            string objId = _idGenerator.NextKey().ToString();

            // replace the obj_id variable in SQL templates
            foreach (SQLAction sqlAction in _sqlActions)
            {
                if (sqlAction.Type == SQLActionType.Insert)
                {
                    sqlAction.ExecutableSQL = sqlAction.SQLTemplate.Replace(GetVariable(NewteraNameSpace.OBJ_ID), objId);
                }
                else if (sqlAction.Type == SQLActionType.WriteClob)
                {
                    sqlAction.ObjId = objId;
                }
            }

			return true;
		}

		/// <summary>
		/// Viste a data class element.
		/// </summary>
		/// <param name="element">A DataClass instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitDataClass(DataClass element)
		{
			return true;
		}

		/// <summary>
		/// Viste a filter element.
		/// </summary>
		/// <param name="element">A Filter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitFilter(Filter element)
		{
            _visitingResultAttributes = false;

			return true;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A DataSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(DataSimpleAttribute element)
		{
            if (_visitingResultAttributes)
            {
                string val = null;
                SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)element.GetSchemaModelElement();
                SimpleAttributeEntity entity = new SimpleAttributeEntity(simpleAttribute);

                if (entity.IsHistoryEdit || entity.IsRichText)
                {
                    // the array data is stored in a clob column
                    SQLAction action = FindWriteClobAction(entity.OwnerClass.Name, entity.Name);
                    if (action != null)
                    {
                        action.Data = element.AttributeValue;
                    }
                }
                else
                {
                    if (entity.IsAutoIncrement)
                    {
                        if (simpleAttribute.HasCustomValueGenerator)
                        {
                            val = GetAttributeValue(entity, element.AttributeValue);
                            // only generate the value using custom key generator if the value is empty
                            if (val == SQLElement.VALUE_NULL)
                            {
                                // generate value using custom value generator
                                IInstanceWrapper instanceWrapper = new Newtera.Common.MetaData.DataView.Validate.InstanceWrapper(_instanceDataView);
                                val = GenerateAttributeValue(simpleAttribute, instanceWrapper);
                            }
                        }
                    }

                    // Get value of the attribute to be inserted
                    if (val == null)
                    {
                        val = GetAttributeValue(entity, element.AttributeValue);
                    }

                    /*
                     * do not create an entry in the insert SQL if the value is null and the
                     * attribute has a default value defined.
                     */
                    if (val == SQLElement.VALUE_NULL && entity.HasDefaultValue())
                    {
                        val = entity.DefaultValue;
                    }

                    // use SearchValue to convert xquery value to a database specific sql value
                    SearchValue fieldValue = new SearchValue(val, entity.Type, _dataProvider);

                    string sqlValue = fieldValue.ToSQL();

                    SQLAction sqlAction = FindActionByOwnerClass(simpleAttribute.OwnerClass.Name, SQLActionType.Insert);
                    if (sqlAction != null)
                    {
                        // replace the variable with sql value
                        sqlAction.ExecutableSQL = sqlAction.ExecutableSQL.Replace(GetVariable(simpleAttribute.Name), sqlValue);
                    }
                }
            }

			return true;
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A DataArrayAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitArrayAttribute(DataArrayAttribute element)
		{
			if (_visitingResultAttributes)
			{
                ArrayAttributeElement arrayAttribute = (ArrayAttributeElement)element.GetSchemaModelElement();
                ArrayAttributeEntity entity = new ArrayAttributeEntity(arrayAttribute);
                if (entity.ArraySize == ArraySizeType.OverSize)
                {
                    // the array data is stored in a clob column
                    SQLAction action = FindWriteClobAction(entity.OwnerClass.Name, entity.Name);
                    if (action != null)
                    {
                        action.Data = element.AttributeValue;
                    }
                }
                else
                {
                    string val = element.AttributeValue;
                    if (string.IsNullOrEmpty(val))
                    {
                        val = SQLElement.VALUE_NULL;
                    }

                    SearchValue fieldValue = new SearchValue(val, entity.Type, _dataProvider);
                    string sqlValue = fieldValue.ToSQL();

                    SQLAction sqlAction = FindActionByOwnerClass(arrayAttribute.OwnerClass.Name, SQLActionType.Insert);
                    if (sqlAction != null)
                    {
                        // replace the variable with sql value
                        sqlAction.ExecutableSQL = sqlAction.ExecutableSQL.Replace(GetVariable(arrayAttribute.Name), sqlValue);
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
            // virtual attribute doesn't have database column, so need to insert

            return true;
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A DataImageAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitImageAttribute(DataImageAttribute element)
        {
            if (_visitingResultAttributes)
            {
                ImageAttributeElement imageAttribute = (ImageAttributeElement)element.GetSchemaModelElement();
                ImageAttributeEntity entity = new ImageAttributeEntity(imageAttribute);

                /*
                 * insert a NULL value for image column
                 */
                string val = SQLElement.VALUE_NULL;

                // use SearchValue to convert a database specific null value
                SearchValue fieldValue = new SearchValue(val, entity.Type, _dataProvider);

                string sqlValue = fieldValue.ToSQL();

                SQLAction sqlAction = FindActionByOwnerClass(imageAttribute.OwnerClass.Name, SQLActionType.Insert);
                if (sqlAction != null)
                {
                    // replace the variable with sql value
                    sqlAction.ExecutableSQL = sqlAction.ExecutableSQL.Replace(GetVariable(imageAttribute.Name), sqlValue);
                }
            }

            return true;
        }

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A DataRelationshipAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipAttribute(DataRelationshipAttribute element)
		{
            if (_visitingResultAttributes)
            {
                RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)element.GetSchemaModelElement();
                RelationshipEntity entity = new RelationshipEntity(relationshipAttribute);

                /*
			     * Set the obj_id of referenced object to the foreign key column. This is
			     * the case when the relationship direction is FORWARD.
			     */
                if (entity.Direction == RelationshipDirection.Forward)
                {
                    string referencedId = RetrieveReferencedId(entity, element);


                    // use SearchValue to convert xquery value to a database specific sql value
                    SearchValue fieldValue = new SearchValue(referencedId, entity.Type, _dataProvider);
                    string sqlValue = fieldValue.ToSQL();

                    SQLAction sqlAction = FindActionByOwnerClass(relationshipAttribute.OwnerClass.Name, SQLActionType.Insert);
                    if (sqlAction != null)
                    {
                        // replace the variable with sql value
                        sqlAction.ExecutableSQL = sqlAction.ExecutableSQL.Replace(GetVariable(relationshipAttribute.Name), sqlValue);
                    }
                }
            }

			return false; // stop visting its sub elements
		}

		/// <summary>
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ResultAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitResultAttributes(ResultAttributeCollection element)
		{
			_visitingResultAttributes = true;
 
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
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ParenthesizedExpr element)
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
            _visitingResultAttributes = false;

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
        /// Generate an attribute value by invoking a custom value generator.
        /// </summary>
        /// <param name="instanceWrapper">the instance to be inserted</param>
        /// <returns>The generated value.</returns>
        private string GenerateAttributeValue(SimpleAttributeElement simpleAttribute, IInstanceWrapper instanceWrapper)
        {
            string val = null;

            IAttributeValueGenerator generator = simpleAttribute.GetAutoValueGenerator();

            if (generator != null)
            {
                // generate an unique id which can be used as part of the generated value to
                // ensure the uniqueness of the generated value
                KeyGenerator idGenerator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.ValueId, _metaData.SchemaInfo);
                string valueId = idGenerator.NextKey().ToString();
                val = generator.GetValue(valueId, instanceWrapper, _metaData);
            }

            return val;
        }

        /// <summary>
        /// Get an inserting value of an attribute.
        /// </summary>
        /// <param name="entity">the attribute entity object.</param>
        /// <param name="val">Unconverted attribute value</param>
        /// <returns> the attribute value in string.</returns>
        private string GetAttributeValue(AttributeEntity entity, string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                val = SQLElement.VALUE_NULL;
            }
            else if (entity.IsEnum)
            {
                if (entity.IsMultipleChoice)
                {
                    // convert the attribute value from string representation
                    // to integer representation
                    val = entity.ConvertToEnumInteger(val).ToString();
                }
                else
                {
                    // convert the enum display text to the enum value.
                    // we store enum value in the database so that user can be
                    // free to change enum display text of an enum constraint
                    val = entity.ConvertToEnumValue(val);
                }
            }
            else if (entity.HasInputMask)
            {
                // remove the mask
                val = entity.ConvertToUnmaskedString(val);
            }
            else if (entity.IsEncrypted)
            {
                val = entity.ConvertToEncrytedString(val);
            }

            return val;
        }

        /// <summary>
        /// Find the SQLAction object by an owner class name
        /// </summary>
        /// <param name="ownerClassName"></param>
        /// <returns></returns>
        private SQLAction FindActionByOwnerClass(string ownerClassName, SQLActionType actionType)
        {
            SQLAction found = null;

            foreach (SQLAction sqlAction in _sqlActions)
            {
                if (sqlAction.Type == actionType && sqlAction.OwnerClassName == ownerClassName)
                {
                    found = sqlAction;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Find the SQLAction of WriteClob type by an owner class name and attribute name
        /// </summary>
        /// <param name="ownerClassName">The attribute owner class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <returns></returns>
        private SQLAction FindWriteClobAction(string ownerClassName, string attributeName)
        {
            SQLAction found = null;

            foreach (SQLAction sqlAction in _sqlActions)
            {
                if (sqlAction.Type == SQLActionType.WriteClob &&
                    sqlAction.OwnerClassName == ownerClassName &&
                    sqlAction.AttributeName == attributeName)
                {
                    found = sqlAction;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Retrieve the id of a referenced object.
        /// </summary>
        /// <param name="relationship">the relationship attribute for the referenced object.</param>
        /// <param name="element">the DataRelationshipAttribute object to get primary key value(s).</param>
        private string RetrieveReferencedId(RelationshipEntity relationship, DataRelationshipAttribute element)
        {
            string referencedObjId = SQLElement.VALUE_NULL;

            // first check if the relationship value is null
            if (element.HasValue)
            {
                // get the primary key value from instance and use them to retrieve the object id 		

                // build a SQL for retrieving the id of referenced object
                ClassEntity referencedClass = new ClassEntity(relationship.LinkedClass.SchemaElement);
                // the referenced class may have inherited classes, create them
                referencedClass.CreateEmptyAncestorClasses();

                DBEntityCollection keys = referencedClass.CreatePrimaryKeys();
                if (keys.Count == 0)
                {
                    throw new NoPrimaryKeyException("Class " + referencedClass.Name + " dose not have any primary keys.");
                }

                foreach (AttributeEntity key in keys)
                {
                    string keyValue = GetPrimaryKeyValue(element, key.Name);
                    if (keyValue == null)
                    {
                        throw new MissingPrimaryKeyValueException("Missing primary key value for key " + key.Name);
                    }
                    key.SearchValue = keyValue;
                }

                IDbConnection con = _dataProvider.Connection;
                IDataReader dataReader = null;
                try
                {
                    string sql = _builder.GenerateSelect(referencedClass);

                    IDbCommand cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    dataReader = cmd.ExecuteReader();

                    if (!dataReader.Read())
                    {
                        throw new NonexistReferencedObjException("Unable to find the referenced instances for " + relationship.Name);
                    }

                    referencedObjId = System.Convert.ToString(dataReader.GetValue(referencedClass.ObjIdEntity.ColumnIndex - 1));

                    // only one row is expected
                    if (dataReader.Read())
                    {
                        throw new MultiReferencedObjException("More than one instances have the same primary key value(s) in the class " + relationship.LinkedClass.SchemaElement.Caption);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (dataReader != null && !dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                    con.Close();
                }
            }

            return referencedObjId;
        }

        /// <summary>
        /// Gets the value of a primary key
        /// </summary>
        /// <param name="element">The DataRelationshipAttribute object</param>
        /// <param name="keyName">The name of primary key</param>
        /// <returns>The primary key value</returns>
        private string GetPrimaryKeyValue(DataRelationshipAttribute element, string keyName)
        {
            string val = null;
            if (element.PrimaryKeys != null)
            {
                foreach (DataSimpleAttribute pk in element.PrimaryKeys)
                {
                    if (pk.Name == keyName)
                    {
                        val = pk.AttributeValue;
                        break;
                    }
                }
            }

            return val;
        }

        private string GetVariable(string attributeName)
        {
            return "{" + attributeName + "}";
        }
	}
}
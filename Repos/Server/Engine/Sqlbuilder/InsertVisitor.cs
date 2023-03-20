/*
* @(#)InsertVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;
    using System.Text;

	using Newtera.Common.Core;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.Wrapper;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Cache;

	/// <summary>
	/// A InsertVisitor object visite the entities and create SQLStatement object(s) for inserting
	/// an instance.
	/// </summary>
	/// <version>  	1.0.0 08 Jul 2003</version>
	/// <author> Yong Zhang</author>
	class InsertVisitor : EntityVisitor
	{		
		// private instance members
		private SQLActionCollection _insertActions;
		private Hashtable _createdStatements;
		private Instance _data;
		private ClassEntity _baseClass;
		private MetaDataModel _meteData;
		private IDataProvider _dataProvider;
        private bool _isSQLTemplate;
        private StringBuilder _actionData;

        /// <summary>
        /// Initiating an instance of a InsertVistor object
        /// </summary>
        /// <param name="baseClass">the base class of inserting instance.</param>
        /// <param name="date">the instance data to be inserted.</param>
        /// <param name="actionData">Logging data</param>
        public InsertVisitor(ClassEntity baseClass, Instance data, MetaDataModel metaData, IDataProvider dataProvider, StringBuilder actionData) :
            this(baseClass, data, metaData, dataProvider, false, actionData)
        {
        }
		
		/// <summary>
		/// Initiating an instance of a InsertVistor object
		/// </summary>
		/// <param name="baseClass">the base class of inserting instance.</param>
		/// <param name="date">the instance data to be inserted.</param>
        /// <param name="actionData">Logging data</param>
        public InsertVisitor(ClassEntity baseClass, Instance data, MetaDataModel metaData, IDataProvider dataProvider, bool isTemplate, StringBuilder actionData)
		{
			_insertActions = new SQLActionCollection();
			_createdStatements = new Hashtable();
			_data = data;
			_baseClass = baseClass;
			_meteData = metaData;
			_dataProvider = dataProvider;
            _isSQLTemplate = isTemplate;
            _actionData = actionData;
		}

		/// <summary>
		/// Gets insert action collection.
		/// </summary>
		/// <value> the insert action collection </value>
		public SQLActionCollection InsertActions
		{
			get
			{
				return _insertActions;
			}
		}
		
		/// <summary>
		/// Visit a ClassEntity object and create an InsertStatement object.
		/// </summary>
		/// <param name="entity">the ClassEntity object to be visited</param>
		public bool VisitClass(ClassEntity entity)
		{
			SQLStatement statement;
			SQLElement clause;
						
			/*
			* If the entity represents a class that is related to the base class through
			* the relationship, ignore it.
			*/
			if (_baseClass == entity || _baseClass.IsChildOf(entity))
			{
				statement = new SQLStatement();
				
				clause = new InsertClause(new TableName(entity.TableName));
				statement.Add(clause); // add INSERT clause
				
				// An INSERT clause always has OBJ_ID, CLS_ID & ATTACHMENT_COUNT columns
				FieldName fieldName = new FieldName(entity.ObjIdEntity.ColumnName);
				clause.Add(fieldName);
				fieldName = new FieldName(entity.ClsIdEntity.ColumnName);
				clause.Add(fieldName);
				fieldName = new FieldName(entity.AttachmentEntity.ColumnName);
				clause.Add(fieldName);
				
				clause = new ValuesClause();
				statement.Add(clause); // add VALUES clause 
				
				// A Value clause always has a OBJ_ID, CLS_ID & ATTACHMENT_COUNT values
				SearchValue fieldValue = new SearchValue(_data.ObjId, entity.ObjIdEntity.Type, _dataProvider);
				clause.Add(fieldValue);
				fieldValue = new SearchValue(ConvertToClassId(_data.ClsName), entity.ClsIdEntity.Type, _dataProvider);
				clause.Add(fieldValue);
				// initialize attachment count with zero
				fieldValue = new SearchValue("0", entity.AttachmentEntity.Type, _dataProvider);
				clause.Add(fieldValue);
				
				SQLAction action = new SQLAction();
				action.Type = SQLActionType.Insert;
				action.Statement = statement;
                action.OwnerClassName = entity.Name;
				_insertActions.Insert(0, action);
				_createdStatements[entity] = statement;

                // log obj_id
                if (_actionData.Length > 0)
                {
                    _actionData.Append(";");
                }

                _actionData.Append("OID = ").Append(_data.ObjId);
				
				if (entity.IsRoot)
				{
					// create an Insert statement for inserting data into the CM_ROOT
					CreateInsertForRoot();
				}
			}

			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object and add the SQLElements to the insert statement for
		/// this attribute.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
            string val = null;
            SQLAction action;

			if (entity.IsAutoIncrement)
			{
                SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)entity.SchemaModelElement;
                if (!simpleAttribute.HasCustomValueGenerator)
                {
                    string keyValue = GetAttributeValue(entity);
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        // if the auto attribute contains a value, it may be generated by the application.
                        // associated with value with id of instance to be inserted for future reference
                        QueryDataCache.Instance.AddSessionObject(keyValue, _data.ObjId);
                    }

                    // excluding the auto increment attribute with default value generator,
                    // because the database is responsible to generate a value
                    return true;
                }
                else if (!_isSQLTemplate)
                {
                    val = GetAttributeValue(entity);
                    // only generate the value using custom key generator if the value is empty
                    if (val == SQLElement.VALUE_NULL)
                    {
                        DataViewModel instanceDataView = _meteData.GetDetailedDataView(_baseClass.Name);
                        InstanceView instanceView = new InstanceView(instanceDataView);
                        IInstanceWrapper instanceWrapper = new InstanceWrapper(_data, instanceView);
                        val = GenerateAttributeValue(simpleAttribute, instanceWrapper);
                    }
                }
			}

            // excluding virtual attribute from the insert statement
            if (entity is VirtualAttributeEntity)
            {
                return true;
            }

            if (!_isSQLTemplate)
            {
                // Get value of the attribute to be inserted
                if (val == null)
                {
                    val = GetAttributeValue(entity);

                    if ((val == null || val == SQLElement.VALUE_NULL)
                        && entity.HasUidAsDefault())
                    {
                        val = "uid_" + Guid.NewGuid().ToString(); // generate an unique id as value
                    }
                }
            }
            else
            {
                // create a variable instead of value
                val = entity.Name;
            }

            /*
             * do not create an entry in the insert SQL if the value is null and the
             * attribute has a default value defined.
             */
            if (val == SQLElement.VALUE_NULL && entity.HasDefaultValue())
            {
                return true;
            }

            // Add a write clob action for Text Data type
            if (entity.IsHistoryEdit || entity.IsRichText || entity.Type == DataType.Text)
            {
                // the value is stored in a clob column, we need to create
                // special action to perform the clob data writting
                action = new SQLAction();
                action.Type = SQLActionType.WriteClob;
                action.TableName = entity.OwnerClass.TableName;
                action.ColumnName = entity.ColumnName;
                action.Data = val;
                action.OwnerClassName = entity.OwnerClass.Name;
                action.AttributeName = entity.Name;

                _insertActions.Add(action); // add it to the end
                return true;
            }
            else if (entity is ArrayAttributeEntity &&
                ((ArrayAttributeEntity)entity).ArraySize == ArraySizeType.OverSize)
            {
                // the array data is stored in a clob column, we need to create
                // special action to perform the clob data writting
                action = new SQLAction();
                action.Type = SQLActionType.WriteClob;
                action.TableName = entity.OwnerClass.TableName;
                action.ColumnName = entity.ColumnName;
                action.Data = val;
                action.OwnerClassName = entity.OwnerClass.Name;
                action.AttributeName = entity.Name;

                _insertActions.Add(action); // add it to the end
            }
            else
            {
                /*
                * Find the insert statement for this attribute.
                */
                SQLElement statement = FindInsertStatement(entity.OwnerClass);

                // create a FieldName element for this attribute
                SQLElementCollection children = statement.Children;

                // First element is InsertClause
                SQLElement clause = children[0];

                // add the FieldName object to Insert Clause
                FieldName fieldName = new FieldName(entity.ColumnName);
                clause.Add(fieldName);

                // The second element is a ValuesClause
                clause = children[1];
                SQLElement fieldValue;
                if (!_isSQLTemplate)
                {
                    fieldValue = new SearchValue(val, entity.Type, _dataProvider);
                }
                else
                {
                    fieldValue = new Variable(val);
                }

                clause.Add(fieldValue);
            }
			
			return true;
		}
		
		/// <summary>
		/// Visit a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{		
			RelationshipDirection direction = entity.Direction;

			/*
			* Set the obj_id of referenced object to the foreign key column. This is
			* the case when the relationship direction is FORWARD.
			*/
			if (direction == RelationshipDirection.Forward)
			{
				/*
				* Find the insert statement for this relationship attribute.
				*/
				SQLElement statement = FindInsertStatement(entity.OwnerClass);
				
				// create a FieldName element for this attribute
				SQLElementCollection children = statement.Children;

				// First element is InsertClause
				SQLElement clause = children[0];
				
				// entity.getColumnName() return the DB column name of a foreign key
				FieldName fieldName = new FieldName(entity.ColumnName);
				clause.Add(fieldName);
				
				// add the referenced id to the value clause of insert statement
				// The second element is a ValuesClause       
				clause = children[1];
				string referencedId = _data.GetReferencedObjId(entity.Name);
				if (referencedId == null)
				{
					referencedId = SQLElement.VALUE_NULL;
				}
				SearchValue fieldValue = new SearchValue(referencedId, entity.Type, _dataProvider);
				clause.Add(fieldValue);
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visit a SchemaEntity object representing a query schema. 
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public bool VisitSchema(SchemaEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visit a ScoreEntity object.
		/// </summary>
		/// <param name="entity">the score entity to be visited.</param>
		public bool VisitScore(ScoreEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Find the insert statement for a given ClassEntity object.
		/// </summary>
		/// <returns> the found insert statement</returns>
		public SQLElement FindInsertStatement(ClassEntity classEntity)
		{
			return (SQLElement) _createdStatements[classEntity];
		}
		
		/// <summary>
		/// Create an insert statement for inserting an instance data into
		/// CM_ROOT table.
		/// </summary>
		/// <remarks>Every instnace will have an entry in CM_ROOT table</remarks>
		public void CreateInsertForRoot()
		{
			SQLStatement statement;
			SQLElement insertClause;
			SQLElement valuesClause;
			
			statement = new SQLStatement();
			
			insertClause = new InsertClause(new TableName(NewteraNameSpace.CM_ROOT_TABLE));
			statement.Add(insertClause); // add INSERT clause
			
			// Insert the obj_id of an instance
			FieldName fieldName = new FieldName("OID");
			insertClause.Add(fieldName);
			
			valuesClause = new ValuesClause();
			statement.Add(valuesClause); // add VALUES clause 
			
			SearchValue fieldValue = new SearchValue(_data.ObjId, SQLElement.OBJ_ID_TYPE, _dataProvider);
			valuesClause.Add(fieldValue);

			// CID column is added to CM_ROOT since version 2.9.1, we need to insert
			// value for CID (Class ID)

			// Insert the cid of an instance
			fieldName = new FieldName("CID");
			insertClause.Add(fieldName);

			fieldValue = new SearchValue(ConvertToClassId(_data.ClsName), SQLElement.OBJ_ID_TYPE, _dataProvider);
			valuesClause.Add(fieldValue);
			
			SQLAction action = new SQLAction();
			action.Type = SQLActionType.Insert;
			action.Statement = statement;
			_insertActions.Insert(0, action);
		}

		/// <summary>
		/// Convert a class name to its id.
		/// </summary>
		/// <param name="className">the class name.</param>
		/// <returns> the class id</returns>
		private string ConvertToClassId(string className)
		{
			if (className != null)
			{
				ClassElement classElement = _meteData.SchemaModel.FindClass(className);
				
				if (classElement == null)
				{
					throw new SQLBuilderException("Failed to find a class with name: " + className);
				}
				else
				{
					return classElement.ID;
				}
			}
			else
			{
				throw new SQLBuilderException("No class name found in the instance");
			}
		}
		
		/// <summary>
		/// Get an inserting value of an attribute.
		/// </summary>
		/// <param name="attributeEntity">the attribute entity object.</param>
		/// <returns> the attribute value in string.</returns>
		private string GetAttributeValue(AttributeEntity entity)
		{
			string val = null;

            val = _data.GetValue(entity.Name);

            // log value pair
            AttributeElementBase attributeElement = entity.SchemaModelElement;
            if (val != SQLElement.VALUE_NULL)
            {
                _actionData.Append(";").Append(attributeElement.Name).Append(" = ").Append(val);
            }
            else
            {
                _actionData.Append(";").Append(attributeElement.Name).Append(" = ").Append("");
            }

            if (val == null)
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
                KeyGenerator idGenerator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.ValueId, _meteData.SchemaInfo);
                string valueId = idGenerator.NextKey().ToString();
                val = generator.GetValue(valueId, instanceWrapper, _meteData);
            }

            return val;
        }
	}
}
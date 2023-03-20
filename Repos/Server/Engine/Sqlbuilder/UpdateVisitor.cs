/*
* @(#)UpdateVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Xml;
	using System.Collections;
    using System.Resources;
    using System.Text;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Server.DB;

	/// <summary>
	/// A UpdateVisitor object visite the entities and create SET fields and a WhereClause object
	/// for an UPDATE SQL statement.
	/// </summary>
	/// <version>  	1.0.0 08 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	class UpdateVisitor : EntityVisitor
	{
		// private instance members
		private SQLActionCollection _updateActions;
		private Hashtable _createdStatements;
		private Instance _data;
		private ClassEntity _baseClass;
		private MetaDataModel _metaData;
		private IDataProvider _dataProvider;
		private XmlElement _originalInstance;
        private StringBuilder _actionData;

		/// <summary>
		/// Initiating an instance of UpdateVistor class.
		/// </summary>
		/// <param name="baseClass">the base class of inserting instance.</param>
		/// <param name="date">the instance data to be inserted.</param>
		/// <param name="localeInfo">the locale info.</param>
        /// <param name="actionData">Logging data</param>
		public UpdateVisitor(ClassEntity baseClass, Instance data, XmlElement originalInstance, IDataProvider dataProvider, MetaDataModel metaData, StringBuilder actionData)
		{
			_updateActions = new SQLActionCollection();
			_createdStatements = new Hashtable();
			_data = data;
			_originalInstance = originalInstance;
			_baseClass = baseClass;
			_dataProvider = dataProvider;
			_metaData = metaData;
            _actionData = actionData;
		}
		
		/// <summary>
		/// Get update SQLAction object(s).
		/// </summary>
		/// <value> the collection of SQLAction objects.</value>
		public SQLActionCollection UpdateActions
		{
			get
			{
				return _updateActions;
			}
		}
		
		/// <summary>
		/// Visits a ClassEntity object and create an Update Statement object.
		/// </summary>
		/// <param name="entity">the ClassEntity object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{
			SQLStatement statement;
			SQLElement clause;

			/*
			* Generate an UPDATE statement for the base class and its parent classes
			* only. If the entity represents a class that is related to the base class
			* through the relationship, ignore it.
			*/
			if (_baseClass != entity && !_baseClass.IsChildOf(entity))
			{
				return true;
			}
			
			/*
			* Create an Update statement only if the class has attributes and the some of
			* the attributes need to be updated.
			*/
			if ((entity.HasLocalAttributes() || entity.HasLocalRelationships()) && _data.ContainsDataFor(entity))
			{
				statement = new SQLStatement();
				
				clause = new UpdateClause(new TableName(entity.TableName));
				statement.Add(clause); // add Update clause
				
				// Create the WHERE clause of the Update Statement
				clause = new WhereClause();
				
				// Use OBJ_ID = value as a condition
				SQLElement leftExp = new FieldName(entity.ObjIdEntity.ColumnName);
				SQLElement rightExp = new SearchValue(_data.ObjId, entity.ObjIdEntity.Type, _dataProvider);
				clause.Add(new Condition(leftExp, "=", rightExp));
				statement.Add(clause); // add WHERE clause

				
				SQLAction action = new SQLAction();
				action.Type = SQLActionType.Update;
				action.Statement = statement;
                action.OwnerClassName = entity.Name;
				_updateActions.Add(action);

				_createdStatements[entity] = statement;

                // log obj_id
                _actionData.Append("OID = ").Append(_data.ObjId);

                if (entity.IsRoot)
                {
                    // create an Update statement for updating UPDATETIME attribute into the CM_ROOT
                    CreateUpdateForRoot(entity);
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
            // do nothing for virtual attribute since it does not have a corresponding column
			if (!(entity is VirtualAttributeEntity) && _data.ContainsDataFor(entity) && !entity.IsAutoIncrement)
			{
				/// make sure it has permission to update this attribute.
				/// 
				/// Note: class level write permission has been checked at UpdateExcutor.cs
				/// therefore, there is no need to check it again in the visitClass method
				/// of UpdateVisitor.cs
				if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, entity, XaclActionType.Write, _originalInstance))
				{
					throw new PermissionViolationException("Do not have permissions to write to the attribute " + entity.Name);
				}
				
				string val = _data.GetValue(entity.Name);
                SQLAction action;

                if (entity is SimpleAttributeEntity &&
                    (entity.IsHistoryEdit || entity.IsRichText || entity.Type == DataType.Text))
                {
                    // the history edit data is stored in a clob column, we need to create
                    // special action to perform the clob data writting
                    action = new SQLAction();
                    action.Type = SQLActionType.WriteClob;
                    action.TableName = entity.OwnerClass.TableName;
                    action.ColumnName = entity.ColumnName;
                    action.Data = val;
                    action.OwnerClassName = entity.OwnerClass.Name;
                    action.AttributeName = entity.Name;

                    _updateActions.Add(action); // add it to the end
                }
				else if (entity is ArrayAttributeEntity &&
					((ArrayAttributeEntity) entity).ArraySize == ArraySizeType.OverSize)
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

					_updateActions.Add(action); // add it to the end
				}
				else
				{
                    // log value pair
                    AttributeElementBase attributeElement = entity.SchemaModelElement;
                    _actionData.Append(";").Append(attributeElement.Name).Append(" = ").Append(val);

					/*
					* Find the update statement for this attribute.
					*/
					SQLElement statement = FindUpdateStatement(entity.OwnerClass);
					
					// create a FieldName element for this attribute
					SQLElementCollection children = statement.Children;
					// First element is an UpdateClause
					SQLElement clause = (SQLElement) children[0];
					
					SQLElement leftExp = new FieldName(entity.ColumnName);

					if (entity.IsEnum)
					{
						if (entity.IsMultipleChoice)
						{
							// convert the attribute value from string representation
							// to integer representation
							val = entity.ConvertToEnumInteger(val).ToString();
						}
						else
						{
							// convert from display text to it enum value
							val = entity.ConvertToEnumValue(val);
						}
					}
                    else if (entity.HasInputMask)
                    {
                        // unmask the string
                        val = entity.ConvertToUnmaskedString(val);
                    }
                    else if (entity.IsEncrypted)
                    {
                        val = entity.ConvertToEncrytedString(val);
                    }

					SQLElement rightExp = new SearchValue(val, entity.Type, _dataProvider);
					clause.Add(new Condition(leftExp, "=", rightExp));


				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Visits a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{
			/*
			* Set the obj_id of referenced object to the foreign key column. This is
			* the case when the relationship direction is FORWARD.
			*/
			if (entity.Direction == RelationshipDirection.Forward && _data.ContainsDataFor(entity))
			{
				// make sure it has permission to update this relationship
				if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, entity, XaclActionType.Write, _originalInstance))
				{
                    throw new PermissionViolationException("Do not have permission to write to the class " + entity.Name);
				}
				
				/*
				* Find the update statement for this attribute.
				*/
				SQLElement statement = FindUpdateStatement(entity.OwnerClass);
				
				// create a FieldName element for this attribute
				// First element is an UpdateClause
				SQLElement clause = (SQLElement) statement.Children[0];
				
				// entity.getColumnName() return the DB column name of a foreign key            
				SQLElement leftExp = new FieldName(entity.ColumnName);
				SQLElement rightExp = new SearchValue(_data.GetReferencedObjId(entity.Name), entity.LinkedClass.ObjIdEntity.Type, _dataProvider);
				clause.Add(new Condition(leftExp, "=", rightExp));
			}
			
			return true;
		}
		
		/// <summary>
		/// Visits a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visits a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public bool VisitSchema(SchemaEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visits a ScoreEntity object.
		/// </summary>
		/// <param name="entity">the score entity to be visited.</param>
		public bool VisitScore(ScoreEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Finds the update statement for a given ClassEntity object.
		/// </summary>
		/// <returns> the found update statement.</returns>
		public SQLElement FindUpdateStatement(ClassEntity classEntity)
		{
			return (SQLElement) _createdStatements[classEntity];
		}

        /// <summary>
        /// Create an update statement for updating UPDATETIME attribute of the instance in  CM_ROOT table.
        /// </summary>
        /// <remarks>Every instnace will have an entry in CM_ROOT table</remarks>
        private void CreateUpdateForRoot(ClassEntity entity)
        {
            SQLStatement statement;
            SQLElement updateClause;
            SQLElement whereClause;

            statement = new SQLStatement();

            updateClause = new UpdateClause(new TableName(NewteraNameSpace.CM_ROOT_TABLE));
            statement.Add(updateClause); // add UPDATE clause

            // Create the WHERE clause of the Update Statement
            whereClause = new WhereClause();

            // Use OBJ_ID = value as a condition
            SQLElement leftExp = new FieldName(entity.ObjIdEntity.ColumnName);
            SQLElement rightExp = new SearchValue(_data.ObjId, entity.ObjIdEntity.Type, _dataProvider);
            whereClause.Add(new Condition(leftExp, "=", rightExp));
            statement.Add(whereClause); // add WHERE clause

            leftExp = new FieldName("UPDATETIME");
            // get the system time
            string val = DateTime.Now.ToString("s");

            rightExp  = new SearchValue(val, DataType.DateTime, _dataProvider);
            updateClause.Add(new Condition(leftExp, "=", rightExp));

            SQLAction action = new SQLAction();
            action.Type = SQLActionType.Update;
            action.Statement = statement;
            action.OwnerClassName = entity.Name;
            _updateActions.Insert(0, action);
        }
	}
}
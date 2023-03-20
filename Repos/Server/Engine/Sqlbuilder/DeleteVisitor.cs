/*
* @(#)DeleteVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;

	using Newtera.Common.Core;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// A DeleteVisitor object visite the entities and create SQLStatement object(s) for delete
	/// an instance.
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003</version>
	/// <author>  Yong Zhang </author>
	class DeleteVisitor : EntityVisitor
	{
		// private instance members
		private SQLElementCollection _deleteStatements;
		private Instance _data;
		private ClassEntity _baseClass;
		private IDataProvider _dataProvider;
		
		/// <summary>
		/// Initiating a DeleteVisitor object
		/// </summary>
		/// <param name="baseClass">is the base class of inserting instance.</param>
		/// <param name="date">the instance data to be inserted.</param>
		public DeleteVisitor(ClassEntity baseClass, Instance data, IDataProvider dataProvider)
		{
			_deleteStatements = new SQLElementCollection();
			_data = data;
			_baseClass = baseClass;
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Gets delete statement collection.
		/// </summary>
		/// <returns> the delete statement collection</returns>
		public SQLElementCollection DeleteStatements
		{
			get
			{
				return _deleteStatements;
			}
		}
		
		/// <summary>
		/// Visit a ClassEntity object and create a Delete Statement object.
		/// </summary>
		/// <param name="entity">the ClassEntity object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{
			SQLStatement statement;
			SQLElement clause;
			SQLElement leftExp, rightExp;
			
			/*
			* the class entity must be either the base class itself or a parent of
			* the base class. Visitor generate a delete Statement for CM_ROOT table,
			* base class, and its inherited classes.
			*/
			if (_baseClass == entity)
			{
				// create a Delete statement for deleting data into the CM_ROOT
				CreateDeleteForRoot();
			}
			else if (!_baseClass.IsChildOf(entity))
			{
				return true;
			}
			
			statement = new SQLStatement();
			
			clause = new DeleteClause(new TableName(entity.TableName));
			statement.Add(clause); // add DELETE clause
			
			// Use OBJ_ID attribute to identify the deleting instance
			clause = new WhereClause();
			leftExp = new SearchFieldName(entity.ObjIdEntity.ColumnName, _dataProvider);
			rightExp = new SearchValue(_data.ObjId, entity.ObjIdEntity.Type, _dataProvider);
			clause.Add(new Condition(leftExp, "=", rightExp));
			statement.Add(clause); // add WHERE clause        
			
			_deleteStatements.Add(statement);
			
			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visit a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{
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
		/// Create a delete statement for deleting an instance data from
		/// CM_ROOT table.
		/// </summary>
		/// <remarks>Every instnace will have an entry in CM_ROOT table</remarks>
		private void CreateDeleteForRoot()
		{
			SQLStatement statement;
			SQLElement clause;
			SQLElement leftExp, rightExp;
			
			statement = new SQLStatement();
			
			clause = new DeleteClause(new TableName(NewteraNameSpace.CM_ROOT_TABLE));
			statement.Add(clause); // add DELETE clause
			
			// Use OBJ_ID attribute to identify the deleting instance
			clause = new WhereClause();
			leftExp = new SearchFieldName("OID", _dataProvider);
			rightExp = new SearchValue(_data.ObjId, SQLElement.OBJ_ID_TYPE, _dataProvider);
			clause.Add(new Condition(leftExp, "=", rightExp));
			statement.Add(clause); // add WHERE clause        
			
			_deleteStatements.Add(statement);
		}
	}
}
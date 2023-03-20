/*
* @(#)UpdateANUMVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// A UpdateANUMVisitor object visite the entities and create SQLStatement object(s) for delete
	/// an instance.
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003</version>
	/// <author>  Yong Zhang </author>
	class UpdateANUMVisitor : EntityVisitor
	{
		// private instance members
		private SQLElementCollection _updateStatements;
		private string _instanceId;
		private ClassEntity _baseClass;
		private IDataProvider _dataProvider;
		private bool _isIncreament;
		
		/// <summary>
		/// Initiating a UpdateANUMVisitor object
		/// </summary>
		/// <param name="baseClass">the base class of the instance</param>
		/// <param name="instanceId">the id of instance</param>
		/// <param name="isIncreament">true to increament ANUM value of instance by one,
		/// false to decreament the ANUM value by one</param>
		/// <param name="dataProvider">DataProvider</param>
		public UpdateANUMVisitor(ClassEntity baseClass, string instanceId, bool isIncreament, IDataProvider dataProvider)
		{
			_updateStatements = new SQLElementCollection();
			_instanceId = instanceId;
			_baseClass = baseClass;
			_isIncreament = isIncreament;
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Gets delete statement collection.
		/// </summary>
		/// <returns> the delete statement collection</returns>
		public SQLElementCollection UpdateStatements
		{
			get
			{
				return _updateStatements;
			}
		}
		
		/// <summary>
		/// Visit a ClassEntity object and create an Update Statement object that
		/// increament or decreament the ANUM column of an instance.
		/// </summary>
		/// <param name="entity">the ClassEntity object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{
			SQLStatement statement;
			SQLElement clause;
			
			/*
			* the class entity must be either the base class itself or a parent of
			* the base class. Visitor generate an update Statement for each class in the
			* inheritance path.
			*/
			if (_baseClass != entity && !_baseClass.IsChildOf(entity))
			{
				return true;
			}
			
			statement = new SQLStatement();
			
			clause = new UpdateClause(new TableName(entity.TableName));
			statement.Add(clause); // add Update clause

			// set new value to ANUM column
			SQLElement leftExp = new FieldName(entity.AttachmentEntity.ColumnName);
			SQLElement rightExp;
			if (_isIncreament)
			{
				rightExp = new SearchValue(entity.AttachmentEntity.ColumnName + " + 1",
					entity.AttachmentEntity.Type, _dataProvider);
			}
			else
			{
				rightExp = new SearchValue(entity.AttachmentEntity.ColumnName + " - 1",
					entity.AttachmentEntity.Type, _dataProvider);
			}

			clause.Add(new Condition(leftExp, "=", rightExp));
			
			// Use OBJ_ID attribute to identify the deleting instance
			clause = new WhereClause();
			leftExp = new SearchFieldName(entity.ObjIdEntity.ColumnName, _dataProvider);
			rightExp = new SearchValue(_instanceId, entity.ObjIdEntity.Type, _dataProvider);
			clause.Add(new Condition(leftExp, "=", rightExp));
			statement.Add(clause); // add WHERE clause        
			
			_updateStatements.Add(statement);
			
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
	}
}
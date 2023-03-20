/*
* @(#)FromVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;

	using Newtera.Server.DB;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// A FromVisitor creates the FROM clause for a SQL Statement.
	/// </summary>
	/// <version>  	1.0.0 06 Jul 2003</version>
	/// <author> Yong Zhang </author>
	class FromVisitor : EntityVisitor
	{		
		// private instance members
		private IDataProvider _dataProvider;
		private SQLBuilder _builder;
		private IList _existingJoins;
		
		/// <summary>
		/// Initiating an instance of a FromVisitor
		/// </summary>
		/// <param name="builder">the SQLBuilder</param>
		/// <param name="dataProvider">the database provider</param>
		public FromVisitor(SQLBuilder builder, IDataProvider dataProvider,
			IList joinStatements)
		{
			_builder = builder;
			_dataProvider = dataProvider;
			_existingJoins = joinStatements;
		}

		/// <summary>
		/// Gets the join statements created by the visitor.
		/// </summary>
		/// <returns> A SQLElement </returns>
		public IList JoinStatements
		{
			get
			{
				return _existingJoins;
			}
		}
		
		/// <summary>
		/// Visit a ClassEntity object.
		/// </summary>
		/// <param name="entity">the class entity object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{	
			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object and add TABLE_ALIAS property to the object.
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
			SQLElement leftOperand, rightOperand, joinCondition, leftElement, rightElement;
			
			// create a join only if the referenced class is non-empty
			if (entity.LinkedClass.HasInheritedAttributes() || entity.LinkedClass.HasInheritedRelationships())
			{
				RelationshipDirection direction = entity.Direction;
				if (direction == RelationshipDirection.Forward)
				{
					// find the join to which the class with the relationship attribute 
					// participated.
					leftElement = FindJoin(entity.OwnerClass.Alias);
					// entity.getColumnName() return the DB column name of a foreign key
					leftOperand = new FieldName(entity.ColumnName, entity.OwnerClass.Alias);
					leftOperand.ClassEntity = entity.OwnerClass; // make reference to its owner class
					
					// find the join to which the referenced class participated.
					rightElement = FindJoin(entity.LinkedClass.Alias);

					rightOperand = new FieldName(entity.LinkedClass.ObjIdEntity.ColumnName, entity.LinkedClass.Alias);
					rightOperand.ClassEntity = entity.LinkedClass; // make reference to its referenced class

					// Right outer join
					joinCondition = new OuterJoinCondition(leftElement, rightElement, leftOperand, rightOperand, JoinType.LeftJoin, _dataProvider);
				}
				else
				{			
					leftElement = FindJoin(entity.OwnerClass.Alias);

					leftOperand = new FieldName(entity.OwnerClass.ObjIdEntity.ColumnName, entity.OwnerClass.Alias);
					leftOperand.ClassEntity = entity.OwnerClass; // make reference to its owner class
					
					rightElement = FindJoin(entity.LinkedClass.Alias);

					/*
					* find the for column name of a foreign key from the corresponding
					* relationshipEntity object of the referenced ClassEntity object
					*/
					//rightOperand = new FieldName(entity.ReferencedRelationship.ColumnName, entity.LinkedClass.Alias);
                    rightOperand = new FieldName(entity.ReferencedRelationship.ColumnName, entity.ReferencedRelationship.OwnerClass.Alias);
					rightOperand.ClassEntity = entity.LinkedClass; // make reference to its referenced class
					
					// Left outer join
					joinCondition = new OuterJoinCondition(leftElement, rightElement, leftOperand, rightOperand, JoinType.LeftJoin, _dataProvider);
				}

				// replace joins with the outer join
				_existingJoins.Remove(leftElement);
				_existingJoins.Remove(rightElement);

				_existingJoins.Add(joinCondition);
			}

			return true;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{	
			/*
			* Create a select statement as a subquery for the aggregate function as part of
			* the from clause.
			*/
			if (entity.OwnerClass != null)
			{
				this._existingJoins.Add(new ViewTable(entity.GetSQLElement(_builder), entity.Alias));
			}
			
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
		/// Find the join statement in which the class of given alias partipates
		/// </summary>
		/// <param name="classAlias">The class alias</param>
		/// <returns>The join statement</returns>
		private SQLElement FindJoin(string classAlias)
		{
			SQLElement foundJoin = null;

			foreach (IJoinElement joinElement in this._existingJoins)
			{
				if (joinElement.ContainsAlias(classAlias))
				{
					foundJoin = (SQLElement) joinElement;
					break;
				}
			}

			if (foundJoin == null)
			{
				throw new SQLBuilderException("Unable to find a Join that consists of a class with alias " + classAlias);
			}

			return foundJoin;
		}
	}
}
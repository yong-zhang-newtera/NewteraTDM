/*
* @(#)JoinVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;

	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// A JoinVisitor object visite the entities and create inner join statements for the
	/// inheritance relationships between classes.
	/// </summary>
	/// <version>  	1.0.0 08 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	class JoinVisitor : EntityVisitor
	{
		// private instance members
		private ArrayList _visitedClasses;
		private ArrayList _existingJoins;
		private IDataProvider _dataProvider;
		
		/// <summary>
		/// Initiating an instance of a JoinVistor class.
		/// </summary>
		/// <param name="dataProvider">the database provider</param>
		public JoinVisitor(IDataProvider dataProvider)
		{
			_existingJoins = new ArrayList();
			_visitedClasses = new ArrayList();
			_dataProvider = dataProvider;
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
		/// Visit a ClassEntity object and create a join condition for parent-child relationship.
		/// </summary>
		/// <param name="attribute">the attribute object to be visited</param>
		public bool VisitClass(ClassEntity entity)
		{
			// the class may have been visited through inheritance
			if (!_visitedClasses.Contains(entity) && entity.Alias != null)
			{
				// build a list of inherited classes in top-down order
				ArrayList inheritedClasses = new ArrayList();

				ClassEntity currentClass = entity;
				while (currentClass != null)
				{
					if (inheritedClasses.Count == 0)
					{
						// this is the base class,
						// make sure the class is needed in the joins
						if (currentClass.HasInheritedAttributes() || currentClass.HasInheritedRelationships())
						{
							// Append the class so that child classes are placed before parent classes
							inheritedClasses.Add(currentClass);
						}
					}
					else
					{
						// it is a parent class, make sure it is needed in the join
						if (currentClass.HasLocalAttributes())
						{
							// Append the class so that child classes are placed before parent classes
							inheritedClasses.Add(currentClass);
						}
					}

					_visitedClasses.Add(currentClass); // mark this class has been visited

                    currentClass = currentClass.ParentEntity;
				}

				// build a SQLElement to represent this inherited relationship
				SQLElement leftOperand, rightOperand, joinCondition, leftElement, rightElement;
				rightOperand = null;
				rightElement = null;
				joinCondition = null;
				foreach (ClassEntity classEntity in inheritedClasses)
				{
					leftElement = new TableName(classEntity.TableName, classEntity.Alias);
					leftElement.ClassEntity = classEntity;
					leftOperand = new FieldName(classEntity.ObjIdEntity.ColumnName, classEntity.Alias);
					leftOperand.ClassEntity = classEntity; // make reference to its owner class
					
					// create an inner join element
					joinCondition = new InnerJoinCondition(leftElement, rightElement, leftOperand, rightOperand, _dataProvider);
				
					// prepare for recursive call
					rightElement = joinCondition;
					rightOperand = leftOperand;
				}

				// remember the inner join
				if (joinCondition != null)
				{
					_existingJoins.Add(joinCondition);
				}
			}

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
		/// <param name="entity">the relationship entity to be visited .</param>
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
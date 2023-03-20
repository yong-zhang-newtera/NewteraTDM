/*
* @(#)ResultVisitor.cs
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
	/// A ResultVisitor object creates a SELECT clause.
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	class ResultVisitor : EntityVisitor
	{		
		/* private instance members */
		private SQLElement _selectClause;
		private bool _isDistinct;
		private int _colIndex;
		private DBEntityCollection _bottomClasses;
		private IDataProvider _dataProvider;
		
		/// <summary>
		/// Initiating an instance of ResultVistor class.
		/// </summary>
		/// <param name="dataProvider">the database provider.</param>
		public ResultVisitor(IDataProvider dataProvider)
		{
			_selectClause = new SelectClause();
			_isDistinct = false;
			_colIndex = 1; // column index starts from 1
			_bottomClasses = new DBEntityCollection();
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Gets the select clause element.
		/// </summary>
		/// <value> the SelectClause element.</value>
		public SQLElement SelectClause
		{
			get
			{
				return _selectClause;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the result is distinct.
		/// </summary>
		/// <value> true if it is distinct, false otherwise.</value>
		public bool IsDistinct
		{
			get
			{
				return _isDistinct;
			}
		}
		
		/// <summary>
		/// Visit a ClassEntity object. Create an OBJ_ID column as a result field
		/// for each bottom class.
		/// </summary>
		/// <param name="entity">the class attribute object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{
			/*
			* Create OBJ_ID & CLS_ID columns for a class in select clause only if the class
			* is the bottom class and has existing attributes or relationships.
			*/
			if (IsBottomClass(entity) && (entity.HasInheritedAttributes() || entity.HasInheritedRelationships()))
			{
				/*
				* Create a result field for the OBJ_ID.
				*/
				ResultFieldName resultField = new ResultFieldName(entity.ObjIdEntity.ColumnName, entity.Alias, entity.ObjIdEntity.Type, _dataProvider);
				resultField.ClassEntity = entity; // Make reference to its owner class
				
				
				// add the result field to the select clause
				_selectClause.Add(resultField);
				
				// remember the colum index in the entity
				entity.ObjIdEntity.ColumnIndex = _colIndex++;
				
				/*
				* Create a result field for the CLS_ID.
				*/
				resultField = new ResultFieldName(entity.ClsIdEntity.ColumnName, entity.Alias, entity.ClsIdEntity.Type, _dataProvider);
				resultField.ClassEntity = entity; // Make reference to its owner class
				
				
				// add the result field to the select clause
				_selectClause.Add(resultField);
				
				// remember the colum index in the entity for a class id
				entity.ClsIdEntity.ColumnIndex = _colIndex++;
				
				/*
				* Create a result field for the ATTACHMENT_COUNT.
				*/
				resultField = new ResultFieldName(entity.AttachmentEntity.ColumnName, entity.Alias, entity.AttachmentEntity.Type, _dataProvider);
				resultField.ClassEntity = entity; // Make reference to its owner class
				
				
				// add the result field to the select clause
				_selectClause.Add(resultField);
				
				// remember the colum index in the attachment count entity
				entity.AttachmentEntity.ColumnIndex = _colIndex++;
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object and create a ResultField for SelectClause.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			if (entity is ArrayAttributeEntity &&
				!entity.IsReferenced)
			{
				// do not include an un-used array attribute in the SQL to save
				// memory space
				return true;
			}
            else if (entity is VirtualAttributeEntity)
            {
                return true; // do not create a result field for virtual attribute
            }

			/*
			* Create a result field name inside select clause.
			* This may include adjustments (a surrounding expr) for unit
			* conversion.
			*/
			ResultFieldName resultField = new ResultFieldName(entity.ColumnName, entity.OwnerClass.Alias, entity.Type, _dataProvider);
			resultField.ClassEntity = entity.OwnerClass; // Make reference to its owner class
			
			
			// add the result field to the select clause
			_selectClause.Add(resultField);
			
			// remember the colum index in the entity
			entity.ColumnIndex = _colIndex++;
			
			return true;
		}
		
		/// <summary>
		/// Visit a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{
			if (entity.Direction == RelationshipDirection.Forward)
			{
				/*
				* Create a result field for the foreign key column.
				*/
				ResultFieldName resultField = new ResultFieldName(entity.ColumnName, entity.OwnerClass.Alias, entity.Type, _dataProvider);
				resultField.ClassEntity = entity.OwnerClass; // Make reference to its owner class
				
				// add the result field to the select clause
				_selectClause.Add(resultField);
				
				// remember the colum index in the entity
				entity.ColumnIndex = _colIndex++;
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{
			SQLElement functionField;
			
			functionField = new FieldName(entity.Name, ((AggregateFuncEntity) entity).Alias);
			
			// add the function field to the select clause
			_selectClause.Add(functionField);
			
			// remember the colum index in the entity
			entity.ColumnIndex = _colIndex++;
			
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
			/*
			* Create a score function as a field in the select clause
			*/
			SQLElement functionField = new ScoreFunc(((ScoreEntity) entity).ColumnName);
			
			// add the function field to the select clause
			_selectClause.Add(functionField);
			
			// remember the colum index in the entity
			entity.ColumnIndex = _colIndex++;
			
			return true;
		}
		
		/// <summary> 
		/// return the information indicating whether the given class is a bottom class.
		/// </summary>
		/// <param name="entity">the class entity object.</param>
		/// <returns>true if the class is a bottom class, otherwise return false status.</returns>
		private bool IsBottomClass(ClassEntity entity)
		{
			bool status = true;
			
			foreach (ClassEntity bottomClass in _bottomClasses)
			{
				if (bottomClass.IsChildOf(entity))
				{
					status = false;
					break;
				}
			}
			
			if (status)
			{
				_bottomClasses.Add(entity);
			}
			
			return status;
		}
	}
}
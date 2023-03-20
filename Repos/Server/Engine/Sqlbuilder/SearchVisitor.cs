/*
* @(#)SearchVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.XaclModel.Processor;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// A SearchVisitor object creates a WHERE clause of SQL statement.
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003</version>
	/// <author> Yong Zhang </author>
	class SearchVisitor : EntityVisitor
	{
        private const string UNKNOWN_ENUM_VALUE = "unknown";
		// private instance members
        private MetaDataModel _metaData;
		private SQLElement _whereClause;
		private IDataProvider _dataProvider;
        private QueryInfo _queryInfo;
		
		/// <summary>
		/// Initiating an instance of SearchVistor class.
		/// </summary>
		/// <param name="dataProvider">the database provider.</param>
		public SearchVisitor(IDataProvider dataProvider, MetaDataModel metaData, QueryInfo queryInfo)
		{
			_whereClause = new WhereClause();
			_dataProvider = dataProvider;
            _metaData = metaData;
            _queryInfo = queryInfo;
		}

		/// <summary>
		/// Gets the where clause element.
		/// </summary>
		/// <value> the WhereClause element</value>
		public SQLElement WhereClause
		{
			get
			{
				return _whereClause;
			}
		}
		
		/// <summary>
		/// Visit a ClassEntity object.
		/// </summary>
		/// <param name="attribute">the attribute object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{
            // check those base class entities if they need to add exclusion conditions
            // for the classes that the current principle does not have permission to read
            if (entity.IsLeaf && !string.IsNullOrEmpty(entity.Alias))
            {
                // when alias of a ClassEntity is null, it is not supposed to be part of query
                AddExcludingClassCondition(entity);
            }

			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object and create a condition for WhereClause.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			string val;
			SQLElement expression;
			SQLElement leftExp, rightExp;
			string relOperator;

            if (entity is ArrayAttributeEntity ||
                entity is VirtualAttributeEntity)
            {
                // array attribute and virtual attribute can not be used as search fields
                return true;
            }
						
			val = entity.SearchValue;
			
			if (val != null && val !=SQLElement.NULL_STRING)
			{
				if (entity.IsEnum)
				{
					if (entity.IsMultipleChoice)
					{
						// convert search value from string representation to integer representation
						val = entity.ConvertToEnumInteger(val).ToString();

						leftExp = new BitwiseAndFunc(entity.ColumnName, entity.OwnerClass.Alias, val, _dataProvider);
					}
					else
					{
						// convert from enum display text to its value
						val = entity.ConvertToEnumValue(val);
                        if (string.IsNullOrEmpty(val))
                        {
                            // it's an unknown enum value, replace with unknown
                            val = UNKNOWN_ENUM_VALUE;
                        }

						leftExp = new SearchFieldName(entity.ColumnName, entity.OwnerClass.Alias, entity.CaseStyle, _dataProvider);
					}
				}
                else if (entity.HasInputMask)
                {
                    val = entity.ConvertToUnmaskedString(val); // unmask the string

                    leftExp = new SearchFieldName(entity.ColumnName, entity.OwnerClass.Alias, entity.CaseStyle, _dataProvider);
                }
                else if (entity.IsEncrypted)
                {
                    val = entity.ConvertToEncrytedString(val); // encrypted the string

                    leftExp = new SearchFieldName(entity.ColumnName, entity.OwnerClass.Alias, entity.CaseStyle, _dataProvider);
                }
                else
                {
                    leftExp = new SearchFieldName(entity.ColumnName, entity.OwnerClass.Alias, entity.CaseStyle, _dataProvider);
                }
				leftExp.ClassEntity = entity.OwnerClass; // reference to its owner class
				
				rightExp = new SearchValue(val, entity.Type, entity.CaseStyle, _dataProvider);
				relOperator = entity.Operator;
				if (relOperator == null)
				{
                    relOperator = SQLElement.OPT_EQUALS;
				}

                expression = new Condition(leftExp, relOperator, rightExp, _dataProvider);
				
				_whereClause.Add(expression);
			}
			
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
			SQLElement leftField, rightField, joinCondition;
						
			// create a join with its owning parent if the root entity is for a relationship
			DBEntity root = entity.RootEntity;
			if (root is RelationshipEntity && entity.OwnerClass != null)
			{
				leftField = new FieldName(entity.OwnerClass.ObjIdEntity.ColumnName, entity.OwnerClass.Alias);
				leftField.ClassEntity = entity.OwnerClass; // make reference to its owner class
				
				/*
				* the root entity of the contained path is used as the right operand of the
				* joined condition.
				*/
				rightField = new FieldName(((RelationshipEntity) root).ReferencedRelationship.ColumnName, entity.Alias);
				
				joinCondition = new Condition(leftField, "=", rightField);
				_whereClause.Add(joinCondition);
			}

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
        /// Add conditions to exclude the instances in those leaf classes which the principle
        /// does not have permission to read  
        /// </summary>
        private void AddExcludingClassCondition(ClassEntity classEntity)
        {
            SQLElement leftField, rightExp, excludeCondition;

            ClassElement classElement = (ClassElement)classEntity.SchemaElement;
            // if the classElement is already leaf class, ignore it
            if (!classElement.IsLeaf)
            {
                SchemaModelElementCollection leafClasses = classElement.GetLeafClasses();
                foreach (ClassElement leafClass in leafClasses)
                {
                    Conclusion conclusion = PermissionChecker.Instance.GetConclusion(_metaData.XaclPolicy, leafClass, XaclActionType.Read);
                    if (conclusion.Permission == XaclPermissionType.Deny)
                    {
                        // add an exclusion condition to the where clause
                        leftField = new FieldName(classEntity.ClsIdEntity.ColumnName, classEntity.Alias);
                        leftField.ClassEntity = classEntity.OwnerClass; // make reference to its owner class

                        /*
                        * the root entity of the contained path is used as the right operand of the
                        * joined condition.
                        */
                        rightExp = new SearchValue(leafClass.ID, DataType.BigInteger, _dataProvider);

                        excludeCondition = new Condition(leftField, "!=", rightExp);
                        _whereClause.Add(excludeCondition);
                    }
                    else if (conclusion.Permission != XaclPermissionType.Grant)
                    {
                        // it is questionable if the current principle has read permission to
                        // all the data instances in the leaf class, add the id of leaf class to
                        // the query info so that we can check it later
                        _queryInfo.QuestionableLeafClassIds.Add(leafClass.ID);
                    }
                }
            }
        }
	}
}
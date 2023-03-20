/*
* @(#)TreeManager.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Xml;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Vdom.Common;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// This class takes care of building a DBEntity object tree.
	/// </summary>
	/// <version>  	1.0.0 18 Jul 2003 </version>
	/// <author>  		Yong Zhang </author>
	public class TreeManager
	{
		private MetaDataModel _metaData; // The meta data model
		private QueryInfo _queryInfo; // TODO, handle mutipl queries
		private VDocument _document; // the document
		
		/// <summary>
		/// Initiating an instance of TreeManager class.
		/// </summary>
		public TreeManager(MetaDataModel metaData, VDocument document)
		{
			_metaData = metaData;
			_document = document;
			_queryInfo = null;
		}

		/// <summary>
		/// Gets or setsthe query info.
		/// </summary>
		/// <value> a query info.</value>
		public QueryInfo QueryInfo
		{
			get
			{
				return _queryInfo;
			}
			set
			{
				_queryInfo = value;
			}
		}

		/// <summary>
		/// Create a new tree with DBEntity objects as nodes based on the path,
		/// or add the new brance to the existing tree.
		/// </summary>
		/// <param name="pathEnumerator">the path enumerator.</param>
		public void GrowTree(PathEnumerator pathEnumerator)
		{	
			QueryInfo queryInfo = QueryInfo;
			
			if (queryInfo == null)
			{
				// It is the first path created, therefore, create a SchemaEntity as the tree root
				SchemaEntity schema = new SchemaEntity();
				CreateTree(schema, pathEnumerator);
				
				bool isForFunction = false;
				DBEntityCollection roots = schema.RootEntities;

                ClassElement baseClass = null;
				if (roots.Count == 1) 
				{
                    if (roots[0] is AggregateFuncEntity)
                    {
                        isForFunction = true;
                    }
                    else if (roots[0] is ClassEntity)
                    {
                        baseClass = ((ClassEntity)roots[0]).SchemaElement;
                    }
				}
				
				QueryInfo = new QueryInfo(schema, isForFunction, baseClass);
			}
			else
			{
				AddBranch(queryInfo, pathEnumerator);
			}
			
			/*
			* If the path represents an attribute entity, make sure there is a read permission
			* to the attribute.
			*/
			pathEnumerator.Reset();
			DBEntity referencedEntity = FindEntity(pathEnumerator);
			if (referencedEntity is AttributeEntity)
			{
				// check the read permission to the attribute entity
				if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, referencedEntity, XaclActionType.Read))
				{
					throw new PermissionViolationException("Do not have read permission to attribute " + referencedEntity.Name);
				}
			}
		}
		
		/// <summary>
		/// prepare a qualifier to build a SQLElement object for it.
		/// </summary>
		/// <param name="qualifier">the qualifier expression to be prepared.</param>
		public void PrepareQualifier(IExpr qualifier)
		{
			PrepareQualifier(qualifier, null);
		}
		
		/// <summary>
		/// Prepare a qualifier to build a SQLElement object for it.
		/// </summary>
		/// <param name="qualifier">the qualifier expression to be prepared.</param>
		/// <param name="funcEntity">
		/// the entity reprenting a function whose path contains the qualifier,
		/// if this is null, the qualifier belongs to the main query.
		/// </param>
		public void PrepareQualifier(IExpr qualifier, AggregateFuncEntity funcEntity)
		{
			QueryInfo queryInfo;
			/*
			* For an aggregate function that is embedded in a query, it will be a separate
			* Select statement built for it, therefore, we need to save the corresponding
			* condition in a separate QueryInfo object.
			*/
			if (funcEntity == null || QueryInfo.IsForFunction)
			{
				queryInfo = QueryInfo;
			}
			else
			{
				queryInfo = funcEntity.QueryInfo;
			}
			
			QualifierVisitor visitor = new QualifierVisitor(this, _document, _document.DataProvider);
			
			// Let visitor to visit each node in the qualifier expression tree
			((ITraversable) qualifier).Accept(visitor);
			
			// Get the SQLElement for the condition
			SQLElement condition = visitor.ConditionElement;
			if (condition != null)
			{
				queryInfo.AddCondition(condition);
			}
			
			if (visitor.Range != null)
			{
				queryInfo.Range = visitor.Range; // Get the query result range
			}
			
			if (visitor.Variables != null)
			{
				queryInfo.AddVariables(visitor.Variables); // Get the query variables
			}
		}
		
		/// <summary>
		/// prepare a function by creating an entity and add it to the tree. 
		/// </summary>
		/// <param name="func">the function.</param>
		/// <param name="params">the list of parameters.</param>
		public void PrepareFunction(IDBFunction func, ExprCollection parameters)
		{
			// set the environment to the function
			func.TreeManager = this;
			func.DataProvider = _document.DataProvider;
			
			switch (func.Type)
			{
				case SQLFunction.Avg: 
				case SQLFunction.Count: 
                case SQLFunction.Distinct:
				case SQLFunction.Max: 
				case SQLFunction.Min: 
				case SQLFunction.Sum: 
					PathEnumerator pathEnumerator = ((Path) parameters[0]).GetAbsolutePathEnumerator();
					
					// grow the tree with the complete function path
					//GrowTree(pathEnumerator);
					
					// Find the last entity of the path contained by the function
					//pathEnumerator.Reset();
					DBEntity lastEntity = FindLastEntity(pathEnumerator);
					
					pathEnumerator = ((Path) parameters[0]).GetBasePathEnumerator();
					AggregateFuncEntity funcEntity = (AggregateFuncEntity) FindEntity(pathEnumerator);
					
					funcEntity.LastEntity = lastEntity;
					
					break;
				
				case SQLFunction.Score: 
					
					DBEntity entity = FindEntity(((Path) parameters[0]).GetAbsolutePathEnumerator());
					if (entity is AttributeEntity)
					{
						// add a ScoreEntity for the score function to the owner class
						entity.OwnerClass.AddFunction(new ScoreEntity(((IExpr) parameters[2]).Eval().ToString(), _document.DataProvider));
					}
					else
					{
						throw new VDOMException("The first parameter is an invalid path for the contains function");
					}
					
					break;
				
				default: 
					throw new VDOMException("Unsupported function :" + func.Type);
			}
		}
		
		/// <summary>
		/// prepare a sortby to build a GROUP BY SQLElement.
		/// </summary>
		/// <param name="sortBy">the sortby object to be prepared.</param>
		public void PrepareSortBy(SortbySpec sortBy)
		{
			SQLElement sortByField;
			
			QueryInfo queryInfo = QueryInfo;
			
			// Find the entity in the tree for the sort by column
			DBEntity entity = FindEntity(sortBy.SortPath.GetAbsolutePathEnumerator());
			if (entity is AttributeEntity)
			{
				sortByField = new OrderByField(((AttributeEntity) entity).ColumnName, ((AttributeEntity) entity).OwnerClass.Alias, sortBy.IsAscending);
				sortByField.ClassEntity = ((AttributeEntity) entity).OwnerClass;
				
				queryInfo.AddSortBy(sortByField);
			}
			else if (entity is ScoreEntity)
			{
				sortByField = new OrderByField(((ScoreEntity) entity).ColumnName, null, sortBy.IsAscending);
				queryInfo.AddSortBy(sortByField);
			}
            else if (entity is ObjIdEntity)
            {
                sortByField = new OrderByField(((ObjIdEntity)entity).ColumnName, ((ObjIdEntity)entity).OwnerClass.Alias, sortBy.IsAscending);
                sortByField.ClassEntity = ((ObjIdEntity)entity).OwnerClass;

                queryInfo.AddSortBy(sortByField);
            }
            else
            {
                throw new VDOMException("SortbySpec path does not specify an attribute");
            }
		}
		
		/// <summary>
		/// Find a DBEntity object from the tree based on a path.
		/// </summary>
		/// <param name="pathEnumerator">the path enumerator.</param>
		/// <returns> the DBEntity object.</returns>
		public DBEntity FindEntity(PathEnumerator pathEnumerator)
		{
			FindEntityVisitor visitor = new FindEntityVisitor(_metaData);
			pathEnumerator.Reset(); // begin iteration from the first step
			PathIterator iterator = new PathIterator(pathEnumerator, QueryInfo.SchemaEntity, visitor);
			iterator.Iterate(); // apply the action to entity tree
			
			return visitor.CurrentEntity; // return the found entity
		}
		
		/// <summary>
		/// Create a new tree.
		/// </summary>
		/// <param name="pathEnumerator">the path enumerator. </param>
		/// <param name="root">the root entity.</param>
		/// <returns> the root of the tree.</returns>
		private DBEntity CreateTree(DBEntity root, PathEnumerator pathEnumerator)
		{
			pathEnumerator.Reset(); // extend from the beginning of the path
			return ExtendBranch(root, pathEnumerator);
		}
		
		/// <summary>
		/// Add a branch to a tree.
		/// </summary>
		/// <param name="queryInfo">the query info.</param>
		/// <param name="pathEnumerator">the path enumerator. </param>
		private void AddBranch(QueryInfo queryInfo, PathEnumerator pathEnumerator)
		{
			/*
			* The front part of path may already have corresponding entities created in the
			* tree. Therefore, first we need to locate the extending entity that
			* represents the first part of the path whose entities have been created.
			* Then adding a new branch for the remaining steps in the path.
			*/
			DBEntity extendingEntity;
			FindExtendingEntityVisitor visitor = new FindExtendingEntityVisitor(_metaData);
			pathEnumerator.Reset(); // begin iteration from the first step
			PathIterator iterator = new PathIterator(pathEnumerator, QueryInfo.SchemaEntity, visitor);
			iterator.Iterate(); // apply the action to entity tree
			
			extendingEntity = visitor.CurrentEntity;
			
			/*
			* If there are steps at the end of the path that do not have entities existed
			* in the tree, ExtendBranch will create new entities for these steps add them
			* to the end of a tree brance.
			*/
			if (!pathEnumerator.IsEnd)
			{
				// Move the enumerator one step back
				pathEnumerator.MovePrevious();
				ExtendBranch(extendingEntity, pathEnumerator);
			}
		}
		
		/// <summary>
		/// Create DB entities for the steps in the new path, and
		/// connect them together based on the relations specified by the corresponding
		/// meta model.
		/// </summary>
		/// <param name="startEntity">the entity to be extended.</param>
		/// <param name="path">the iterator that iterates steps in a path.</param>
		/// <returns> the root entity of the newly created path.</returns>
		private DBEntity ExtendBranch(DBEntity startEntity, PathEnumerator pathEnumerator)
		{
			ExtendBranchVisitor visitor = new ExtendBranchVisitor(_metaData, _document.Interpreter, _document.DataProvider);
			PathIterator iterator = new PathIterator(pathEnumerator, startEntity, visitor);
			iterator.Iterate(); // apply the action to entity tree
			
			return startEntity; // return the root entity		
		}
		
		/// <summary>
		/// Find the last entity of the path contained by a function.
		/// </summary>
		/// <param name="path">
		/// the absolute path that refers the entity at the end of the path.
		/// </param>
		/// <returns> the entity found</returns>
		private DBEntity FindLastEntity(PathEnumerator pathEnumerator)
		{
			DBEntity lastEntity = FindEntity(pathEnumerator);
			if (lastEntity is ClassEntity)
			{
				// get the obj id entity from the class entity
				lastEntity = ((ClassEntity) lastEntity).ObjIdEntity;
			}
			else if (!(lastEntity is AttributeEntity))
			{
				throw new VDOMException("The last step in the path contained by an aggregate function has to refer to eithe a class or an attribute");
			}
			
			return lastEntity;
		}
	}
}
/*
* @(#)SQLBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Data;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
    using System.Text;


	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.DB;

	/// <summary>
	/// A SQLBuilder object generates a SQL statement based on information stored in the given
	/// objects of DBEntity types. The DBEntity objects and their relationships represent
	/// a view of Newtera's object-relational data model. The generated SQL will retrieve from
	/// or add/delete/update data in a relational database according to spec in the view.
	/// </summary>
	/// <version>  	1.0.0 08 Sep 2003</version>
	/// <author> Yong Zhang </author>
	public class SQLBuilder
	{		
		public const int DEFAULT_PAGE_SIZE = 100; // Default page size
		
		// private instance variable declarartion
		private MetaDataModel _metaData;
		private IDataProvider _dataProvider;
		private SymbolLookup _lookup;
		
		/// <summary>
		/// Initiating an instance of SQLBuilder class.
		/// </summary>
		/// <param name="metaData">the meta model</param>
		/// <param name="dataProvider">the database provider </param>
		public SQLBuilder(MetaDataModel metaData, IDataProvider dataProvider)
		{
			_metaData = metaData;
			_dataProvider = dataProvider;

			_lookup = SymbolLookupFactory.Instance.Create(dataProvider.DatabaseType);
		}
		
		/// <summary>
		/// Generates a Select SQL statement that joins related table through object id and
		/// foreign keys.
		/// </summary>
		/// <param name="baseClass">the base class for a query.</param>
		/// <returns> generated SELECT SQL statement </returns>
		public string GenerateSelect(ClassEntity baseClass)
		{
			SchemaEntity schema = new SchemaEntity();
			schema.AddRootEntity(baseClass);
			return GenerateSelect(new QueryInfo(schema, false, baseClass.SchemaElement));
		}
		
		/// <summary>
		/// Generates a Select SQL statement that joins related table through object id and
		/// foreign keys.
		/// </summary>
		/// <param name="queryInfo">the query info.</param>
		/// <returns> the generated SELECT SQL statement.</returns>
		public string GenerateSelect(QueryInfo queryInfo)
		{
			SchemaEntity schema = queryInfo.SchemaEntity;
			
			// Validate the query info
			ValidateQueryInfo(queryInfo);
			
			// Create a new SQL statement object
			SQLStatement statement = new SQLStatement();
			
			// Resolve variables by evaluate them
			ResolveVariables(queryInfo);
			
			// generate table alias first and a FROM clause
			GenerateFromClause(statement, schema);
			
			// process result attributes and generate SELECT
			GenerateSelectClause(statement, schema);
			
			// process conditions and generate WHERE clause
			GenerateWhereClause(statement, schema, queryInfo);
			
			// generate ORDER BY clause
			GenerateOrderByClause(statement, queryInfo);
			
			// validate the generated statement
			//statement.Accept(new ValidateVisitor());
			
			// rearrange the order of clauses
			statement.SortChildren();
			
			string sql = statement.ToSQL();
			
			return sql;
		}
		
		/// <summary>
		/// Generates UPDATE SQL action(s) for a given base class. If the class is a subclass
		/// and the mapping is vertical. This method return a list of UPDATE SQL actions,
		/// one for each of ancestor classes, or a clob column.
		/// </summary>
		/// <param name="baseClass">the base class for update.</param>
		/// <param name="data">the update data for updating.</param>
		/// <param name="originalInstance">the originalInstance data for permission checking.</param>
        /// <param name="actionData">To Keep the updated attributes and values for logging purpose</param>
		/// <returns> the string array of SQL UPDATE statements</returns>
		public SQLActionCollection GenerateUpdates(ClassEntity baseClass, Instance data, XmlElement originalInstance, StringBuilder actionData)
		{
			
			StringCollection updateSQLs = new StringCollection();
			
			UpdateVisitor visitor = new UpdateVisitor(baseClass, data, originalInstance, _dataProvider, _metaData, actionData);
			baseClass.Accept(visitor);
			
			/*
			 * UpdateVisitor generates multiple UPDATE SQLAction objects,
			 * one for each physical table in the database, or a clob column.
             * make sure the update statment has at least one field
			 */
            foreach (SQLAction sqlAction in visitor.UpdateActions)
            {
                if (sqlAction.Type == SQLActionType.Update)
                {
                    SQLElementCollection children = sqlAction.Statement.Children;
                    // First element is an UpdateClause
                    SQLElement clause = (SQLElement)children[0];
                    if (clause.Children.Count == 0)
                    {
                        // no column specified in update clause
                        sqlAction.Type = SQLActionType.Invalid;
                    }
                }
            }

			return visitor.UpdateActions;
		}

        /// <summary>
        /// Generates INSERT SQL action(s) for each of involved table in an inheritance.
        /// </summary>
        /// <param name="baseClass">is the base class into which to insert an instance data.</param>
        /// <param name="data">the instance data to be inserted.</param>
        /// <param name="actionData">Logging data</param>
        /// <returns>A collection of SQLAction instances.</returns>
        public SQLActionCollection GenerateInserts(ClassEntity baseClass, Instance data, StringBuilder actionData)
        {
            return GenerateInserts(baseClass, data, false, actionData);
        }
		
		/// <summary>
		/// Generates INSERT SQL action(s) for each of involved table in an inheritance.
		/// </summary>
		/// <param name="baseClass">is the base class into which to insert an instance data.</param>
		/// <param name="data">the instance data to be inserted.</param>
        /// <param name="isTemplate">true to generate insert sqls as templates containing variables rather than values. false otherwise.</param>
        /// <param name="actionData">To Keep the updated attributes and values for logging purpose</param>
		/// <returns>A collection of SQLAction instances.</returns>
		public SQLActionCollection GenerateInserts(ClassEntity baseClass, Instance data, bool isTemplate, StringBuilder actionDate)
		{
            InsertVisitor visitor = new InsertVisitor(baseClass, data, _metaData, _dataProvider, isTemplate, actionDate);
			baseClass.Accept(visitor);
			
			/*
			* InsertVisitor generates multiple INSERT SQLElement objects,
			* one for each physical table in the database.
			*/
			return visitor.InsertActions;
		}
		
		/// <summary>
		/// Generates a DELETE SQL statement(s).
		/// </summary>
		/// <param name="baseClass">is the base class from which to delete instance(s).</param>
		/// <param name="instance">the instance to be deleted.</param>
		/// <returns>Array of delete statements</returns>
		public StringCollection GenerateDeletes(ClassEntity baseClass, Instance data)
		{
			StringCollection deleteSQLs = new StringCollection();
			
			DeleteVisitor visitor = new DeleteVisitor(baseClass, data, _dataProvider);
			baseClass.Accept(visitor);
			
			/*
			* DeleteVisitor generates multiple DELETE SQLElement objects,
			* one for each physical table in the database.
			*/
			SQLElementCollection statements = visitor.DeleteStatements;
			foreach (SQLElement statement in statements)
			{
				deleteSQLs.Add(statement.ToSQL());
			}
			
			return deleteSQLs;
		}
		
		/// <summary>
		/// Generates a Select SQL statement for an aggregate function
		/// </summary>
		/// <param name="queryInfo">the queryInfo for the function.</param>
		/// <returns> a SQLElement object representing the generated SQL statement </returns>
		public SQLElement GenerateFunctionSQL(QueryInfo queryInfo)
		{
			DBEntityCollection rootEntities = queryInfo.SchemaEntity.RootEntities;
			
			/*
			* there must be a single root which is AggregateFuncEntity type
			*/
			if (rootEntities.Count != 1 && !(rootEntities[0] is AggregateFuncEntity))
			{
				throw new SQLBuilderException("Mismatched type of entity for generateFunctionSQL");
			}
			
			AggregateFuncEntity funcEntity = (AggregateFuncEntity) rootEntities[0];
			ClassEntity baseClass;
			if (funcEntity.RootEntity is ClassEntity)
			{
				baseClass = (ClassEntity) funcEntity.RootEntity;
			}
			else
			{
				baseClass = ((RelationshipEntity) funcEntity.RootEntity).LinkedClass;
			}
			
			// Create a new SQL statement object
			SQLStatement statement = new SQLStatement();
			
			// Resolve variables by evaluate them
			ResolveVariables(queryInfo);
			
			// generate table alias first and a FROM clause
			GenerateFromClause(statement, baseClass);
			
			// generate SELECT for the function
			GenerateFunctionSelectClause(statement, funcEntity);
			
			// process conditions and generate WHERE clause
			GenerateWhereClause(statement, baseClass, queryInfo);
			
			// generate GROUP BY clause
			GenerateGroupByClause(statement, funcEntity);
			
			// rearrange the order of clauses
			statement.SortChildren();
			
			return statement;
		}
		
		/// <summary>
		/// Generates a Select SQL statement for count.
		/// </summary>
		/// <param name="queryInfo">the queryInfo for the function.</param>
		/// <returns> the generated SQL statement.</returns>
		public string GenerateCountSQL(QueryInfo queryInfo)
		{
			SchemaEntity schema = queryInfo.SchemaEntity;
			
			// Validate the query info
			ValidateQueryInfo(queryInfo);
			
			// Create a new SQL statement object
			SQLStatement statement = new SQLStatement();
			
			// Resolve variables by evaluate them
			ResolveVariables(queryInfo);
			
			// generate table alias first and a FROM clause
			GenerateFromClause(statement, schema);
			
			// generate count function for SELECT clause
			GenerateCountClause(statement);
			
			// process conditions and generate WHERE clause
			GenerateWhereClause(statement, schema, queryInfo);
			
			// generate ORDER BY clause, not need to process order by for count
			//generateOrderByClause(statement, queryInfo);
			
			// rearrange the order of clauses
			statement.SortChildren();
			
			string sql = statement.ToSQL();
			
			return sql;
		}

        /// <summary>
        /// Generates a Select SQL statement for getting distinct class ids.
        /// </summary>
        /// <param name="queryInfo">the queryInfo for the function.</param>
        /// <returns> the generated SQL statement.</returns>
        public string GenerateDistinctClassIdSQL(QueryInfo queryInfo)
        {
            SchemaEntity schema = queryInfo.SchemaEntity;

            // Validate the query info
            ValidateQueryInfo(queryInfo);

            // Create a new SQL statement object
            SQLStatement statement = new SQLStatement();

            // Resolve variables by evaluate them
            ResolveVariables(queryInfo);

            // generate table alias first and a FROM clause
            GenerateFromClause(statement, schema);

            // generate SELECT clause that returns distinct class ids
            GenerateDistinctClassIdClause(statement, schema);

            // process conditions and generate WHERE clause
            GenerateWhereClause(statement, schema, queryInfo);

            // generate ORDER BY clause, not need to process order by for count
            //generateOrderByClause(statement, queryInfo);

            // rearrange the order of clauses
            statement.SortChildren();

            string sql = statement.ToSQL();

            return sql;
        }
		
		/// <summary>
		/// Generates SQL statement(s) for updating ANUM (Attachment Number) column of an instance
		/// </summary>
		/// <param name="baseClass">the base class of the instance</param>
		/// <param name="instanceId">the id of instance</param>
		/// <param name="isIncreament">true to increament ANUM value of instance by one,
		/// false to decreament the ANUM value by one</param>
		/// <returns>A collection of update statements</returns>
		public StringCollection GenerateANUMUpdates(ClassEntity baseClass, string instanceId, bool isIncreament)
		{
			StringCollection updateSQLs = new StringCollection();
			
			UpdateANUMVisitor visitor = new UpdateANUMVisitor(baseClass, instanceId, isIncreament, _dataProvider);
			baseClass.Accept(visitor);
			
			/*
			* UpdateANUMVisitor can generate multiple UPDATE SQLElements,
			* one for each of physical tables if the base class has inherited classes.
			*/
			SQLElementCollection statements = visitor.UpdateStatements;
			foreach (SQLElement statement in statements)
			{
				updateSQLs.Add(statement.ToSQL());
			}
			
			return updateSQLs;
		}

		/// <summary>
		/// Converts a query result set of given range into xml elements and add them to a xml document.
		/// </summary>
		/// <param name="queryInfo">the information about the query.</param>
		/// <param name="doc">the xml document. </param>
		/// <param name="resultSet">the query result set </param>
        /// <param name="omitArrayData">Whether to omit array data</param>
		/// <returns> the number of rows processed in the result set.</returns>
		public int ConvertResultSet(QueryInfo queryInfo, VDocument doc, IDataReader dataReader, bool omitArrayData, bool calculateVirtualValue)
		{
			
			ConvertVisitor visitor = new ConvertVisitor(doc, dataReader, _metaData, _dataProvider, omitArrayData, calculateVirtualValue);
			
			/*
			* convert each query result row in the set once a time
			*/
			try
			{
				/*
				* move the cursor of result set to the position indicated by the from
				* of range
				*/
				int count = 1;
				int from = queryInfo.Range.From;
				int to = queryInfo.Range.To;
                ServerExecutionContext context = new ServerExecutionContext();
				
				/*
				* Page size can not exceed the maximum page size for the sake of performance
				* and memeory usage
				*/
				if ((to - from) > Range.MAX_PAGE_SIZE)
				{
					to = from + Range.MAX_PAGE_SIZE;
				}
				
				while (count < from && dataReader.Read())
				{
					count++;
				}
				
				// Now process the rows fall in the range
				while (count <= to && dataReader.Read())
				{
					
					// visitor process the current row in the result set
					queryInfo.SchemaEntity.Accept(visitor);

                    // Generate values of any virtual attributes exists
                    visitor.GenerateVirtualAttributeValues(context);
					
					// clear the visitor for next row
					visitor.Clear();
					
					count++;
				}
								
				return count;
			}
			catch (Exception e)
			{
				throw new SQLBuilderException(e.Message, e);
			}
		}

		/// <summary>
		/// Converts a query result set of next page into xml elements and add them to a xml document.
		/// </summary>
		/// <param name="queryInfo">the information about the query.</param>
		/// <param name="doc">the xml document. </param>
		/// <param name="resultSet">the query result set </param>
		/// <param name="pageSize">The page size</param>
        /// <param name="omitArrayData">Whether to convert array data</param>
		/// <returns> the number of rows processed in the result set.</returns>
		public int ConvertResultSet(QueryInfo queryInfo, VDocument doc, IDataReader dataReader, int pageSize, bool omitArrayData, bool calculateVirtualValue)
		{
			
			ConvertVisitor visitor = new ConvertVisitor(doc, dataReader, _metaData, _dataProvider, omitArrayData, calculateVirtualValue);
            ServerExecutionContext context = new ServerExecutionContext();

			/*
			* convert each query result row in the set once a time
			*/
			try
			{
				int count = 0;
				
				while (count < pageSize && dataReader.Read())
				{
					
					// visitor process the current row in the result set
					queryInfo.SchemaEntity.Accept(visitor);

                    // Generate values of any virtual attributes exists
                    visitor.GenerateVirtualAttributeValues(context);

                    // clear the visitor for next row
                    visitor.Clear();
					
					count++;
				}
								
				return count;
			}
			catch (Exception e)
			{
				throw new SQLBuilderException(e.Message, e);
			}
		}
		
		/// <summary>
		/// Personalizes the result based on the user's read permissions.
		/// </summary>
		/// <param name="queryInfo">the information about the query.</param>
		/// <param name="personalizedDoc">the xml document to be personalized.</param>
		/// <param name="entityTable">the hash table for associating xml elements with their entities.</param>
		public void PersonalizeResult(QueryInfo queryInfo, XmlDocument personalizedDoc, Hashtable entityTable,
            bool checkReadPermissionOnly, bool showEncryptedData)
		{
            PersonalizeVisitor visitor = new PersonalizeVisitor(personalizedDoc, entityTable, _metaData, queryInfo, checkReadPermissionOnly, showEncryptedData);
			
			// visitor process the result stored in xml document
			queryInfo.SchemaEntity.Accept(visitor);

			visitor.CommitChanges();
		}
		
		/// <summary>
		/// Print out structure information about the base class
		/// </summary>
		/// <param name="baseClass">is the base class from which to delete instance(s).</param>
		public void OutputDebuggingInfo(ClassEntity baseClass)
		{
			PrintVisitor visitor = new PrintVisitor();
			baseClass.Accept(visitor);
		}
		
		/// <summary>
		/// Generate a FROM clause of the SQL.
		/// </summary>
		/// <param name="the">SQL statement object.</param>
		/// <param name="rootEntity">the root entity of the query.</param>
		private void GenerateFromClause(SQLStatement statement, DBEntity rootEntity)
		{
			FromClause clause;
			
			// create table alias for each class
			AliasVisitor aliasVisitor = new AliasVisitor();
			rootEntity.Accept(aliasVisitor);
			
			// Generate inner joins for inheritances
			JoinVisitor joinVisitor = new JoinVisitor(this._dataProvider);
			rootEntity.Accept(joinVisitor);

			/*
			* Creates a FROM clause
			*/
			clause = new FromClause();
			clause.Position = 2; // Sencond clause in a SQL
			FromVisitor fromVisitor = new FromVisitor(this, _dataProvider, joinVisitor.JoinStatements);
			rootEntity.Accept(fromVisitor);
			IList joins = fromVisitor.JoinStatements;
			foreach (SQLElement sqlElement in joins)
			{
				clause.Add(sqlElement);
			}
			
			statement.AddFromClause(clause); // add FROM clause
		}
		
		/*
		* generate SELECT clause
		*/
		private void GenerateSelectClause(SQLStatement statement, SchemaEntity schema)
		{
			ResultVisitor visitor = new ResultVisitor(_dataProvider);
			schema.Accept(visitor);
			
			/*
			* After visiting field, the result visitor creates a SELECT clause
			*/
			SQLElement clause = visitor.SelectClause;
			clause.Position = 1; // SELECT is the first clause in a SQL
			
			statement.Add(clause); // add SELECT clause
		}
		
		/*
		* create WHERE clause
		*/
		private void GenerateWhereClause(SQLStatement statement, DBEntity rootEntity, QueryInfo queryInfo)
		{			
			EntityVisitor visitor = new SearchVisitor(_dataProvider, _metaData, queryInfo);
			rootEntity.Accept(visitor);
			
			/*
			* get the where clause from the visitor
			*/
			SQLElement whereClause = ((SearchVisitor) visitor).WhereClause;
			whereClause.Position = 3; // WHERE is the third clause
			
			/*
			* build conditions according to qualifier expressions.
			*/
			if (queryInfo != null && queryInfo.Condition != null)
			{
				whereClause.Add(queryInfo.Condition);
			}
			
			// WHERE clause is optional
			if (whereClause.Children.Count > 0)
			{
				statement.Add(whereClause); // add WHERE clause
			}
		}
		
		/*
		* creates ORDER BY clause
		*/
		private void GenerateOrderByClause(SQLStatement statement, QueryInfo queryInfo)
		{
			if (queryInfo.SortByElement != null)
			{
				SQLElement orderByClause = queryInfo.SortByElement;
				orderByClause.Position = 7; // Order BY is the last clause
				statement.Add(orderByClause); // add OrderBy clause
			}
		}
		
		/*
		* generates SELECT clause for an aggregate function
		*/
		private void GenerateFunctionSelectClause(SQLStatement statement, AggregateFuncEntity funcEntity)
		{
			ResultFieldName fieldName;
			int colIndex = 1; // index of column in the returned result of a SQL
			
			SelectClause clause = new SelectClause();
			clause.Position = 1; // SELECT is the first clause in a SQL
			
			/*
			* when the root entity is a RelationshipEntity, the aggregate function is supposed
			* to be grouped by the relationship value. Create a column for the group clause.
			*/
			if (funcEntity.RootEntity is RelationshipEntity)
			{
				RelationshipEntity relationship = (RelationshipEntity) funcEntity.RootEntity;
				fieldName = new ResultFieldName(relationship.ReferencedRelationship.ColumnName, relationship.LinkedClass.Alias, relationship.ReferencedRelationship.Type, _dataProvider);
				fieldName.ClassEntity = relationship.LinkedClass; // Make reference to its owner class
				
				
				relationship.ColumnIndex = colIndex++;
				clause.Add(fieldName);
			}
			
			// Create a function result field for the aggregate function
			DBEntity lastEntity = funcEntity.LastEntity;
			fieldName = new ResultFieldName(lastEntity.ColumnName, lastEntity.OwnerClass.Alias, lastEntity.Type, funcEntity.ColumnName, funcEntity.Name, _dataProvider);
			fieldName.ClassEntity = lastEntity.OwnerClass; // Make reference to its owner class
			
			
			funcEntity.ColumnIndex = colIndex++;
			clause.Add(fieldName);
			
			statement.Add(clause); // add SELECT clause
		}
		
		/*
		* creates GROUP BY clause And/or HAVING clause for an aggregate function SQL
		*/
		private void GenerateGroupByClause(SQLStatement statement, AggregateFuncEntity entity)
		{
			/*
			* when the root entity is a RelationshipEntity, the aggregate function is supposed
			* to be grouped by the relationship value. Create a group clause according to it.
			*/
			if (entity.RootEntity is RelationshipEntity)
			{
				RelationshipEntity relationship = (RelationshipEntity) entity.RootEntity;
				SQLElement groupByClause = new GroupByClause();
				
				// Use the referenced relationship as the base for grouping
				groupByClause.Add(new FieldName(relationship.ReferencedRelationship.ColumnName, relationship.LinkedClass.Alias));
				groupByClause.Position = 5; // GROUP BY is at fifth position
				statement.Add(groupByClause); // add GROUP BY clause
				
				if (entity.Condition != null)
				{
					SQLElement havingClause = new HavingClause(entity.Condition);
					havingClause.Position = 6;
					statement.Add(havingClause);
				}
			}
		}
		
		/*
		* generates SELECT clause with a count function
		*/
		private void GenerateCountClause(SQLStatement statement)
		{
			SQLElement clause = new SelectClause();
			string funcName = _lookup.CountFunc;
			clause.Add(new CountField(funcName));
			
			clause.Position = 1; // SELECT is the first clause in a SQL
			
			statement.Add(clause); // add SELECT clause
		}

        /// <summary>
        /// Generates SELECT clause that returns distinct class ids
        /// </summary>
        /// <param name="statement"></param>
        private void GenerateDistinctClassIdClause(SQLStatement statement, SchemaEntity schemaEntity)
        {
            SQLElement clause = new SelectClause();
            string keyword = _lookup.DistinctFunc;
            string tableAlias = null;
            if (schemaEntity.RootEntities.Count > 0 &&
                schemaEntity.RootEntities[0] is ClassEntity)
            {
                // first entity represents the base class
                ClassEntity baseEntity = (ClassEntity)schemaEntity.RootEntities[0];
                clause.Add(new ClassIdField(baseEntity.Alias, keyword));
            }   

            clause.Position = 1; // SELECT is the first clause in a SQL

            statement.Add(clause); // add SELECT clause
        }
		
		/// <summary>
		///  Resolves the variables by calling eval on each of them and set the resolved value to
		/// its corresponding Search Value element
		/// </summary>
		private void ResolveVariables(QueryInfo queryInfo)
		{
			Hashtable variables = queryInfo.Variables;
			if (variables != null)
			{
				ICollection keys = variables.Keys;
				foreach (IExpr expr in keys)
				{
					// set the variable value to its SearchValue element
					SearchValue element = (SearchValue) variables[expr];
					element.Value = expr.Eval().ToString();
				}
			}
		}
		
		/// <summary>
		/// Validate the QueryInfo object.
		/// </summary>
		/// <param name="the">query info.</param>
		private void ValidateQueryInfo(QueryInfo queryInfo)
		{
			DBEntityCollection roots = queryInfo.SchemaEntity.RootEntities;
			
			foreach (ClassEntity baseClass in roots)
			{
				if (!baseClass.HasInheritedAttributes() && !baseClass.HasInheritedRelationships())
				{
					throw new MissingAttributesException("The class " + baseClass.Name + " doesn't have any attributes");
				}
			}
		}
		
		/// <summary>
		/// Add outer-join condition to the from clause for DB2 database.
		/// </summary>
		/// <param name="fromClause">the from clause object.</param>
		/// <param name="outerJoin">the outer join statement.</param>
		private void AddOuterJoinToFromClause(SQLElement fromClause, OuterJoinCondition outerJoin)
		{
			//Remove the tables from the from clause that have shown up in the outer join
			//equition
			SQLElement leftTableFound = null;
			SQLElement rightTableFound = null;
			SQLElementCollection elements = fromClause.Children;

			foreach (SQLElement element in elements)
			{
				// Stop the iteration if both left and right tables are found
				if (leftTableFound != null && rightTableFound != null)
				{
					break;
				}
				
				if (leftTableFound == null && element.ToSQL() == outerJoin.LeftTable.ToSQL())
				{
					leftTableFound = element;
				}
				
				if (rightTableFound == null && element.ToSQL() == outerJoin.RightTable.ToSQL())
				{
					rightTableFound = element;
				}
			}
			
			// Remove the elements from the from clause
			if (leftTableFound != null)
			{
				fromClause.Remove(leftTableFound);
			}
			
			if (rightTableFound != null)
			{
				fromClause.Remove(rightTableFound);
			}

			/*
			 * Microsoft does not allow same table alias to appear more than once in
			 * a FROM clause, therefore, we have to change the alias if it has already
			 * appeared in previous outer join conditions
			 */
			foreach (SQLElement element in elements)
			{
				if (element is OuterJoinCondition)
				{
					OuterJoinCondition existingOuterJoin = (OuterJoinCondition) element;

					if (existingOuterJoin.LeftTable.ToSQL() == outerJoin.LeftTable.ToSQL() ||
						existingOuterJoin.RightTable.ToSQL() == outerJoin.LeftTable.ToSQL())
					{
						ChangeTableAlias(outerJoin.LeftTable, outerJoin.LeftOperand);
					}
					else if (existingOuterJoin.LeftTable.ToSQL() == outerJoin.RightTable.ToSQL() ||
						existingOuterJoin.RightTable.ToSQL() == outerJoin.RightTable.ToSQL())
					{
						ChangeTableAlias(outerJoin.RightTable, outerJoin.RightOperand);
					}
				}
			}
			
			fromClause.Add(outerJoin);
		}

		/// <summary>
		/// Change the alias of the given table and operand elements
		/// </summary>
		/// <param name="tableName">The table element</param>
		/// <param name="fieldName">The field element</param>
		private void ChangeTableAlias(SQLElement tableElement, SQLElement operandElement)
		{
			TableName tableName = (TableName) tableElement;
			FieldName fieldName = (FieldName) operandElement;

			if (tableName.Alias != null)
			{
				// append an "a" to the alias
				tableName.Alias = tableName.Alias + "a";
			}

			if (fieldName.TableAlias != null)
			{
				// append an "a" to the alias
				fieldName.TableAlias = fieldName.TableAlias + "a";
			}
		}
	}
}
/*
* @(#)AggregateFuncEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// An AggregateFuncEntity object represents the one of aggregate functions, such as
	/// avg, count, min, max, and sum.
	/// </summary>
	/// <version>  1.0.0 25 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class AggregateFuncEntity : DBEntity
	{
		/* private instance variables */
		private DBEntity _last = null; // The last entity in the path contained by the function
		private DBEntity _root = null; // The root entity in the path contained by the function
		private string _name; // xquery function name
		private string _dbName; // database-specific function name
		private string _alias = null; // alias for the view created for the function
		private SQLElement _condition = null; // set when function is involved in condition
		private QueryInfo _queryInfo; // Query info for the function
		
		/// <summary>
		/// Convert a function type to a generic aggregate function name 
		/// </summary>
		/// <param name="type">one of SQLFunction enum values</param>
		/// <returns>function name</returns>
		static public string GetFunctionName(SQLFunction type)
		{
			string name = "";
			
			switch (type)
			{
				
				case SQLFunction.Avg: 
					name = SQLElement.FUNCTION_AVG_STR;
					break;
				
				case SQLFunction.Count: 
					name = SQLElement.FUNCTION_COUNT_STR;
					break;

                case SQLFunction.Distinct:
                    name = SQLElement.FUNCTION_DISTINCT_STR;
                    break;
				
				case SQLFunction.Min: 
					name = SQLElement.FUNCTION_MIN_STR;
					break;
				
				case SQLFunction.Max: 
					name = SQLElement.FUNCTION_MAX_STR;
					break;
				
				case SQLFunction.Sum: 
					name = SQLElement.FUNCTION_SUM_STR;
					break;
			}
			
			return name;
		}
		
		/// <summary>
		/// Initiating an AggregateFuncEntity object
		/// </summary>
		/// <param name="name">the generic function name</param>
		/// <param name="dataProvider">data provider</param>
		public AggregateFuncEntity(string name, IDataProvider dataProvider) : base()
		{
			SchemaEntity schema = new SchemaEntity();
			
			_name = name;
			_dbName = GetDBFuncNameByName(name, dataProvider);
			
			schema.AddRootEntity(this);
			_queryInfo = new QueryInfo(schema, true);
		}

		/// <summary>
		/// Gets the queryinfo for the function.
		/// </summary>
		/// <value> the query info</value>
		public QueryInfo QueryInfo
		{
			get
			{
				return _queryInfo;
			}
		}

		/// <summary>
		/// Gets or sets the root entity of the path contained by the function.
		/// </summary>
		/// <value>the root entity object</value>
		public DBEntity RootEntity
		{
			get
			{
				return _root;
			}
			set
			{
				_root = value;
			}
		}

		/// <summary>
		/// Gets or sets the last entity of the path contained by the function.
		/// </summary>
		/// <value> the last entity object</value>
		public DBEntity LastEntity
		{
			get
			{
				return _last;
			}
			set
			{
				_last = value;
			}
		}

		/// <summary>
		/// Gets or sets the class alias
		/// </summary>
		/// <value> the class alias</value>
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
			}
		}

		/// <summary>
		/// Gets or sets the condition element
		/// </summary>
		/// <value> condition element </value>
		public SQLElement Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		/// <summary>
		/// Gets the name of aggregate function name.
		/// </summary>
		/// <returns> the function name</returns>
		public override string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the function type.
		/// </summary>
		/// <value> one of DataType enum values</value>
		/// <remarks>the function type is the same as that of the enclosed entity</remarks>
		public override DataType Type
		{
			get
			{
				return _last.Type;
			}
		}

		/// <summary>
		/// Gets the Database name for the function.
		/// </summary>
		/// <value> the database-specific name of the function</value>
		public override string ColumnName
		{
			get
			{
				return _dbName;
			}
		}
		
		/// <summary>
		/// Accept a EntityVistor to visit itself.
		/// </summary>
		/// <param name="visitor">the visiting visitor</param>
		public override void Accept(EntityVisitor visitor)
		{
			visitor.VisitFunction(this);
		}
				
		/// <summary>
		/// Create a SQLStatement object for retrieving the function result from the database
		/// </summary>
		/// <param name="builder">The SQLBuilder object</param>
		public SQLElement GetSQLElement(SQLBuilder builder)
		{
			return builder.GenerateFunctionSQL(_queryInfo);
		}
		
		/// <summary>
		/// Convert a function name to a database-specific aggregate function name 
		/// </summary>
		/// <param name="name">the generic function name</param>
		/// <param name="dataProvider">the database provider</param>
		/// <returns> the db-specific function name</returns>
		private string GetDBFuncNameByName(string funcName, IDataProvider dataProvider)
		{
			string name = "";
			
			DatabaseType source = dataProvider.DatabaseType;
			switch (funcName)
			{
				case SQLElement.FUNCTION_AVG_STR:
					name = SymbolLookupFactory.Instance.Create(source).AvgFunc;
					break;
				case SQLElement.FUNCTION_COUNT_STR:
					name = SymbolLookupFactory.Instance.Create(source).CountFunc;
					break;
                case SQLElement.FUNCTION_DISTINCT_STR:
                    name = SymbolLookupFactory.Instance.Create(source).DistinctFunc;
                    break;
				case SQLElement.FUNCTION_MIN_STR:
					name = SymbolLookupFactory.Instance.Create(source).MinFunc;
					break;
				case SQLElement.FUNCTION_MAX_STR:
					name = SymbolLookupFactory.Instance.Create(source).MaxFunc;
					break;
				case SQLElement.FUNCTION_SUM_STR:
					name = SymbolLookupFactory.Instance.Create(source).SumFunc;
					break;
			}
			
			return name;
		}
	}
}
/*
* @(#)QueryInfo.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;

    using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary> This class stores and provides all necessary information about a query and ready
	/// for the sqlbuilder to generate a SQL based them.
	/// 
	/// </summary>
	/// <version>  	1.0.0 29 Jan 2003
	/// </version>
	/// <author> Yong Zhang </author>
	public class QueryInfo
	{		
		private SchemaEntity _schema; // the schema entity of a query
		private Range _range = null; // Specifying the range of the result
		private SQLElementCollection _conditions = null;
		private SQLElementCollection _sortBys = null;
		private SQLElementCollection _groupBys = null;
		private Hashtable _variables = null;
		private bool _isForFunction = false;
        private bool _isNestedQuery = false;
        private ArrayList _questionableLeafClassIds = null;
        private ClassElement _baseClass = null; // base class of the query
		
		/// <summary>
		/// Initiating an instance of QueryInfo class.
		/// </summary>
		/// <param name="schema">the schema entity of a query.</param>
		public QueryInfo(SchemaEntity schema)
		{
			_schema = schema;
		}
		
		/// <summary>
		/// Initiating an instance of QueryInfo class.
		/// </summary>
		/// <param name="schema">the schema entity of a query.</param>
		/// <param name="isForFunction">indicates whether this query is a function query</param>
		public QueryInfo(SchemaEntity schema, bool isForFunction)
		{
			_schema = schema;
			_isForFunction = isForFunction;
		}

        /// <summary>
        /// Initiating an instance of QueryInfo class.
        /// </summary>
        /// <param name="schema">the schema entity of a query.</param>
        /// <param name="isForFunction">indicates whether this query is a function query</param>
        /// <param name="baseClass">The base class of the query</param>
        public QueryInfo(SchemaEntity schema, bool isForFunction, ClassElement baseClass)
        {
            _schema = schema;
            _isForFunction = isForFunction;
            _baseClass = baseClass;
        }
		
		/// <summary>
		/// Gets or sets schema entity.
		/// </summary>
		/// <value> the schema entity</value>
		public SchemaEntity SchemaEntity
		{
			get
			{
				return _schema;
			}
			set
			{
				_schema = value;
			}
		}

		/// <summary>
		/// Gets or sets range of the result.
		/// </summary>
		/// <value> the Range object</value>
		public Range Range
		{
			get
			{
				if (_range != null)
				{
					return _range;
				}
				else
				{
					return new Range(); // return a default range
				}
			}
			set
			{
				_range = value;
			}
		}

        /// <summary>
        /// Gets the condition of the query.
        /// </summary>
        /// <value> the Condition object.</returns>
        public SQLElement Condition
		{
			get
			{
				SQLElement condition = null;
				
				if (_conditions != null)
				{
					if (_conditions.Count == 1)
					{
						// a single condition
						condition = (SQLElement) _conditions[0];
					}
					else if (_conditions.Count > 1)
					{
						condition = new ANDExpression();
						foreach (SQLElement element in _conditions)
						{
							// add a parenthesis if the condition is an ORExpression
							if (element is ORExpression)
							{
								condition.Add(new EnclosedExpression(element));
							}
							else
							{
								condition.Add(element);
							}
						}
					}
				}
				
				return condition;
			}
		}

		/// <summary>
		/// Gets the sort-by element.
		/// </summary>
		/// <value> the sort-by element.</value>
		public SQLElement SortByElement
		{
			get
			{
				SQLElement sortby = null;
				
				if (_sortBys != null)
				{
					if (_sortBys.Count > 0)
					{
						sortby = new OrderByClause();
						foreach (SQLElement element in _sortBys)
						{
							sortby.Add(element);
						}
					}
				}
				
				return sortby;
			}
		}

		/// <summary>
		/// Gets the group-by element
		/// </summary>
		/// <value> the group by object.</value>
		public SQLElement GroupByElement
		{
			get
			{
				SQLElement groupBy = null;
				
				if (_groupBys != null)
				{
					if (_groupBys.Count > 0)
					{
						groupBy = new GroupByClause();
						foreach (SQLElement element in _groupBys)
						{
							groupBy.Add(element);
						}
					}
				}
				
				return groupBy;
			}
		}

		/// <summary>
		/// Gets the hashtable that associates for the variables with SQLElement objects
		/// </summary>
		/// <returns> the hashtable object</returns>
		public Hashtable Variables
		{
			get
			{
				return _variables;
			}
		}

		/// <summary>
		/// Gets information indicating whether the query is for count
		/// </summary>
		/// <value> true if it is for count, false otherwise. </value>
		public bool IsForFunction
		{
			get
			{
				return _isForFunction;
			}
		}

        /// <summary>
        /// Gets the base class of the query, could be null.
        /// </summary>
        public ClassElement BaseClassElement
        {
            get
            {
                return _baseClass;
            }
        }

        /// <summary>
        /// Gets a list of ids of the leaf classes that the current principle may not have read
        /// permission to all the data instances.
        /// </summary>
        public ArrayList QuestionableLeafClassIds
        {
            get
            {
                if (_questionableLeafClassIds == null)
                {
                    _questionableLeafClassIds = new ArrayList();
                }

                return _questionableLeafClassIds;
            }
        }
		
		/// <summary>
		/// Adds a condition to the query info.
		/// </summary>
		/// <param name="condition">the condition object.</param>
		public void AddCondition(SQLElement condition)
		{
			if (_conditions == null)
			{
				_conditions = new SQLElementCollection();
			}
			
			_conditions.Add(condition);
		}
		
		/// <summary>
		/// Adds a sortby to the query info.
		/// </summary>
		/// <param name="sortby">a sortby field SQLElement.</param>
		public void AddSortBy(SQLElement sortBy)
		{
			if (_sortBys == null)
			{
				_sortBys = new SQLElementCollection();
			}
			
			_sortBys.Add(sortBy);
		}
		
		/// <summary>
		/// Add a groupby to the query info.
		/// </summary>
		/// <param name="groupby">a groupby SQLElement.</param>
		public void AddGroupBy(SQLElement groupBy)
		{
			if (_groupBys == null)
			{
				_groupBys = new SQLElementCollection();
			}
			
			_groupBys.Add(groupBy);
		}
		
		/// <summary>
		/// Add variables to the query info
		/// </summary>
		/// <param name="hashtable">that contains variables and their associated SQLElements.
		/// 
		/// </param>
		public void AddVariables(Hashtable variables)
		{
			if (_variables == null)
			{
				_variables = new Hashtable();
			}
			
			ICollection keys = variables.Keys;
		
			foreach (object key in keys)
			{
				_variables[key] = variables[key]; // add an entry to it
			}
		}
	}
}
/*
* @Interpreter.cs 	1.0  2003-10-29
*
* Development version 1.0
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.IO;
	using System.Xml;
    using System.Data;
	using System.Collections;
    using System.Collections.Specialized;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Interpreter.Parser;
    using Newtera.Server.Engine.Sqlbuilder;
	using antlr;

	/// 
	/// <summary> This is the main class that directs a query. 
	///
	/// The query is evaluated in 4 passes. Pass 1 builds an expression
	/// tree out of the query, pass 2 traverses the tree to optimize, which
	/// mainly means preparing for the database calls, pass 3 is a second
	/// optimize pass also required for the database optimization, and pass
	/// 4 is the actual evaluation. 
	///
	/// The Interpreter object runs the 4 passes (parse, prepare, restrict,
	/// eval), and stores the environment information associated with it
	/// (symbol table, errors, function table, etc.)
	///
	/// </summary>
	/// <version>  1.0.0 17 Aug 2003 </version>
	public class Interpreter
	{		
		// These 3 variables are documented below, in the functions where
		// they are manipulated.
		private SymbolTable _symbolTable;

		private XmlDocument _doc;
		
		/*
		* This is a hack to get round the problem of getting count for a FLWR
		* query. When this flag is true, it tells Document object (Document.cs) to
		* build a special VDocument object that is dedicated to retrieve count of a query
		* rather than get result. This is added by Yong Zhang
		*/
		private bool _forCount = false;
		private bool _isPaging = false;
		private int _pageSize = 50;
		private int _countValue = 0;
        private bool _omitArrayData = false;
        private bool _delayVirtualAttributeCalculation = true;
        private bool _isForClassNames = false;
        private bool _isForSQLActions = false;
        private bool _needToRaiseEvents = true;
        private bool _isNestedQuery = false;
        private bool _checkReadPermissionOnly = false;
        private bool _showEncryptedData = false;
        private bool _obtainCachedObjId = false;
        private StringCollection _classNames;
        private SQLActionCollection _sqlActions;
        private string _extraCondition;
        private IDbConnection _globalConnection = null;
        private IDbTransaction _globalTransaction = null;
		
		/// <summary>
		/// Initiating an instance of Interpreter
		/// </summary>
		public Interpreter()
		{
			Reset();
		}

		/// <summary> 
		/// Gets or sets the xml document to be returned by the interpreter
		/// </summary>
		/// <returns> the xml document</returns>
		public XmlDocument Document
		{
			get
			{
				return _doc;
			}
			set
			{
				_doc = value;
			}
		}

		/// <summary> 
		/// Gets or sets the information indicating whether the interpreter
		/// is used for getting a query count.
		/// </summary>
		/// <returns> true if it is, false otherwise</returns>
		public bool IsForCount
		{
			get
			{
				return _forCount;
			}
			set
			{
				_forCount = value;
			}
		}

        /// <summary> 
        /// Gets or sets the information indicating whether the interpreter
        /// is used for getting a list of class names as search result.
        /// </summary>
        /// <returns> true if it is, false otherwise</returns>
        public bool IsForClassNames
        {
            get
            {
                return _isForClassNames;
            }
            set
            {
                _isForClassNames = value;
            }
        }

        /// <summary> 
        /// Gets or sets the information indicating whether the interpreter
        /// is used for executing a nested query for a virtual attribute, for example
        /// </summary>
        /// <returns> true if it is executing a nested query , false otherwise</returns>
        public bool IsNestedQuery
        {
            get
            {
                return _isNestedQuery;
            }
            set
            {
                _isNestedQuery = value;
            }
        }

		/// <summary> 
		/// Gets or sets the information indicating whether the result of a query
		/// is obtained in paging mode.
		/// </summary>
		/// <returns> true if it is, false otherwise</returns>
		public bool IsPaging
		{
			get
			{
				return _isPaging;
			}
			set
			{
				_isPaging = value;
			}
		}

        /// <summary> 
        /// Gets or sets the information indicating whether to raise events for the update queries
        /// such as Insert, Update, and Delete.
        /// </summary>
        /// <returns> true if it is, false otherwise</returns>
        public bool NeedToRaiseEvents
        {
            get
            {
                return _needToRaiseEvents;
            }
            set
            {
                _needToRaiseEvents = value;
            }
        }

		/// <summary>
		/// Gets or sets the page size
		/// </summary>
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
			}
		}

        /// <summary>
        /// Gets or sets the information indicate whether to omit array data in return result.
        /// </summary>
        /// <value>True if omitting array data. false otherwise. default is false</value>
        public bool OmitArrayData
        {
            get
            {
                return _omitArrayData;
            }
            set
            {
                _omitArrayData = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to delay calculating values of virtual attributes
        /// </summary>
        /// <value>True to postpone the virtual value calculation at the select node stage. default is True</value>
        /// <remarks>Delay calculations to improve the query speed by avoiding calculating unneccessary virtual attributes</remarks>
        public bool DelayVirtualAttributeCalculation
        {
            get
            {
                return _delayVirtualAttributeCalculation;
            }
            set
            {
                _delayVirtualAttributeCalculation = value;
            }
        }

        /// <summary> 
        /// Gets or sets the count value
        /// </summary>
        /// <returns> count value</returns>
        public int CountValue
		{
			get
			{
				return _countValue;
			}
			set
			{
				_countValue = value;
			}
		}

        /// <summary>
        /// Gets or sets the class names as of a search result
        /// </summary>
        public StringCollection ClassNames
        {
            get
            {
                return _classNames;
            }
            set
            {
                _classNames = value;
            }
        }

        /// <summary> 
        /// Gets or sets the information indicating whether the interpreter
        /// is used for getting sql actions created for an xquery. When it is true,
        /// it will return the corresponding SQL Actions without really execute the query,
        /// in other words, no changes to database will take place.
        /// </summary>
        /// <returns> true if it is for getting sql actions, false otherwise</returns>
        public bool IsForSQLActions
        {
            get
            {
                return _isForSQLActions;
            }
            set
            {
                _isForSQLActions = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to check permissions other than read
        /// </summary>
        /// <value>True to check read permission only. false to check all permissions. default is false</value>
        public bool CheckReadPermissionOnly
        {
            get
            {
                return _checkReadPermissionOnly;
            }
            set
            {
                _checkReadPermissionOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to show encrypted data in the query result
        /// </summary>
        /// <value>True to show encrypted data in the result. false otherwise, default is false</value>
        public bool ShowEncryptedData
        {
            get
            {
                return _showEncryptedData;
            }
            set
            {
                _showEncryptedData = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to obtain cached objId for a primary key
        /// </summary>
        /// <value>True to obtain a cached objId, false to get objId from database. default is false</value>
        public bool ObtainCachedObjId
        {
            get
            {
                return _obtainCachedObjId;
            }
            set
            {
                _obtainCachedObjId = value;
            }
        }


        /// <summary>
        /// Gets or sets an exta condition to be added to the query passed to the interpreter
        /// </summary>
        /// <value>The condition in xquery syntax. The default value is a null reference </value>
        public string ExtraCondition
        {
            get
            {
                return _extraCondition;
            }
            set
            {
                _extraCondition = value;
            }
        }

        /// <summary>
        /// Gets the SQL Actions resulting from a query
        /// </summary>
        public SQLActionCollection SQLActions
        {
            get
            {
                return _sqlActions;
            }
            set
            {
                _sqlActions = value;
            }
        }

        // Gets or sets an IDbConnection for a global transaction
        public IDbConnection IDbConnection
        {
            get
            {
                return _globalConnection;
            }
            set
            {
                _globalConnection = value;
            }
        }

        // Gets or sets an IDbTransaction which is a global transaction
        public IDbTransaction IDbTransaction
        {
            get
            {
                return _globalTransaction;
            }
            set
            {
                _globalTransaction = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether there is a global transaction
        /// </summary>
        public bool HasGlobalTransaction
        {
            get
            {
                if (_globalTransaction != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary> resets the current interpreter so it can be reused
		/// 
		/// </summary>
		/// <exception cref=""> InterpreterException
		/// 
		/// </exception>
		// Should this be automatic, rather than required?
		public virtual void Reset()
		{
			_symbolTable = new SymbolTable(this);

			_doc = null;
		}
		
		/// <summary>
		/// this is the normal entry point to the interpreter
		/// </summary>
		/// <param name="query">XQuery to evaluate </param>
		/// <exception cref="InterpreterException">thrown when there is a problem in executing the query</exception>
		/// <returns> an XML Document</returns>
		public XmlDocument Query(string query)
		{
			IExpr tree = Parse(query);

			tree.Prepare();
 
            tree.Restrict();
  
            XNode result = (XNode) tree.Eval();

            return (XmlDocument) result.Content;
		}

        /// <summary>
        /// Get a query reader that reads the result in pages in a forward-only manner.
        /// </summary>
        /// <param name="query">XQuery to evaluate </param>
        /// <exception cref="InterpreterException">thrown when there is a problem in parsing and preparing the query</exception>
        /// <returns>A QueryReader object.</returns>
        public QueryReader GetQueryReader(string query)
		{
			IExpr tree = Parse(query);
			
			tree.Prepare();
			
			tree.Restrict();
						
			return new QueryReader(tree, this);
		}
		
		/// <summary>
		/// this is another entry point to the interpreter for getting the count of
		/// a query.
		/// </summary>
		/// <param name="query">XQuery to evaluate </param>
		/// <exception cref="InterpreterException">thrown when there is a problem in executing the query</exception>
		/// <returns> an XML Document</returns>
		public XmlDocument Count(string query)
		{
			_forCount = true;
            _isForClassNames = false;
            _isForSQLActions = false;
			return Query(query);
		}

        /// <summary>
        /// this is another entry point to the interpreter for getting the leaf class names as query result
        /// </summary>
        /// <param name="query">XQuery to evaluate </param>
        /// <exception cref="InterpreterException">thrown when there is a problem in executing the query</exception>
        /// <returns> An XML Document</returns>
        public XmlDocument GetClassNames(string query)
        {
            _isForClassNames = true;
            _forCount = false;
            _isForSQLActions = false;
            return Query(query);
        }

        /// <summary>
        /// this is another entry point to the interpreter for getting the SQL Actions without executing query.
        /// </summary>
        /// <param name="query">XQuery to evaluate </param>
        /// <exception cref="InterpreterException">thrown when there is a problem in parsing and preparing the query</exception>
        /// <returns> an XML Document</returns>
        /// <remarks>This method only works for query that insert instances for now. For other kinds of query,
        /// it will execute as usual.</remarks>
        public XmlDocument GetSQLActions(string query)
        {
            _isForClassNames = false;
            _forCount = false;
            _isForSQLActions = true;

            return Query(query);
        }
		
		/// <summary>
		/// Parsing an xquery
		/// </summary>
		/// <param name="query">The xquery string</param>
		/// <returns>The root of parse tree</returns>
		internal IExpr Parse(string query)
		{
			return Parse(query, null);
		}

		/// <summary>
		/// Parsing an xquery
		/// </summary>
		/// <param name="query">The xquery string</param>
		/// <param name="context">A context for parsing the query, can be null</param>
		/// <returns>The root of parse tree</returns>
		internal IExpr Parse(string query, IExpr context)
		{
			try
			{
				XQueryLexer lexer = new XQueryLexer(new StringReader(query));
				XQueryParser parser = new XQueryParser(this, lexer);
                
				if (context != null)
				{
					return parser.Parse(context); // get the tree root
				}
				else
				{
					return parser.Parse(); // get the tree root
				}
			}
			catch (ANTLRException ex)
			{
				throw new InterpreterException(ex.Message, ex);
			}
			catch (Exception other)
			{
				throw new InterpreterException("Got an error in parsing query: \n" + query + " \n with error:" + other.Message, other);
			}
		}

		internal SymbolTable SymbolTable
		{
			get
			{
				return _symbolTable;
			}
		}
	}
}
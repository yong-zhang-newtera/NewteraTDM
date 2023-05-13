/*
* @(#)ContainsFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin.Db
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Vdom.Dbimp;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// Class ContainsFunction is a builtin function to perform a full-text search on a
	/// text value.
	/// </summary>
	/// <version>  1.0 2003-9-21</version>
	/// <author>  Yong Zhang </author>
	public class ContainsFunction : FunctionImpBase, IDBFunction
	{
		private TreeManager _treeManager = null;
		private SQLElement _containsElement = null;
		private DBEntity _functionEntity = null;
		private IDataProvider _dataProvider = null;
		
		/// <summary>
		/// Constructor: Functions are instanciated by Class.forName().
		/// This means its real constructor cannot have any arguments. This
		/// constructor is fairly useless, the real work is done in the 
		/// method Constructor()
		/// </summary>
		public ContainsFunction() : base()
		{
		}

		/// <summary>
		/// Set the tree manager for retrieving an entity using a path.
		/// </summary>
		/// <value>the tree manager.</value>
		public TreeManager TreeManager
		{
			set
			{
				_treeManager = value;
			}
		}

		/// <summary>
		/// Set the data provider
		/// </summary>
		/// <value>the data provider.</value>
		public IDataProvider DataProvider
		{
			set
			{
				_dataProvider = value;
			}
		}

		/// <summary>
		/// Used to get an SQL equivalent element created for the function.
		/// </summary>
		/// <value> an SQLElement </value>
		public SQLElement SQLElement
		{
			get
			{
				return _containsElement;
			}
		}

		/// <summary>
		/// Gets the DBEntity for the function.
		/// </summary>
		/// <value> a DBEntity</value>
		public DBEntity FunctionEntity
		{
			get
			{
				return _functionEntity;
			}
		}
			
		/// <summary>
		/// Get the function type, defined in SQLElement.
		/// </summary>
		/// <value> one of SQLFunction enum values.</value>
		public SQLFunction Type
		{
			get
			{
				return SQLFunction.Score;
			}
		}

		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			// There must be one argument
			if (arguments.Count != 2 && arguments.Count != 3)
			{
				throw new InterpreterException("Usage: Contains(path, keywords) or Contains(path, keywords, label)");
			}
			
			if (!(arguments[0] is Path))
			{
				throw new InterpreterException("Contains function expects the first argument to be a path");
			}
		}
	
		/// <summary>
		/// prepare the function.
		/// </summary>
		public override void Prepare()
		{			
			// Prepare a score function entity if the third parameter, label, is provided
			if (_arguments.Count == 3)
			{
				//We know that the first argument is of Path type
                Value val = ((Path)_arguments[0]).TraceDocument();
                if (val != null)
                {
                    VDocument doc = val.ToNode() as VDocument;
                    if (doc != null)
                    {
                        doc.PrepareFunction(this, _arguments);
                    }
                }
			}
		}
		
		/// <summary>
		/// This is called during visiting qualifiers in a query and allows to build
		/// a SQL equivalent function to contains function.
		/// </summary>
		public override void Restrict()
		{
			DBEntity entity = _treeManager.FindEntity(((Path) _arguments[0]).GetAbsolutePathEnumerator());
			if (entity is AttributeEntity)
			{
				AttributeEntity attrEntity = (AttributeEntity) entity;
				SQLElement element = new SearchFieldName(attrEntity.ColumnName, attrEntity.OwnerClass.Alias, attrEntity.CaseStyle, _dataProvider);
				element.ClassEntity = attrEntity.OwnerClass; // reference to its owner class
				
				if (_arguments.Count == 3)
				{
					_containsElement = new ContainsFunc(element, ((IExpr) _arguments[1]).Eval().ToString(), ((IExpr) _arguments[2]).Eval().ToString(), _dataProvider, (attrEntity.IsHistoryEdit || attrEntity.IsRichText));
				}
				else
				{
					_containsElement = new ContainsFunc(element, ((IExpr) _arguments[1]).Eval().ToString(), _dataProvider, (attrEntity.IsHistoryEdit || attrEntity.IsRichText));
				}
			}
			else
			{
				throw new InterpreterException("The first argument is an invalid path");
			}
		}

        /// <summary>
        /// Contains function is executed by a database, therefore, this function does not need to
        /// perform anything except just returning a true, since all the records have been selected
        /// by the data base according to the search criteria.
        /// </summary>
		public override Value Eval()
		{
            return new XBoolean(true);
		}
	}
}
/// <summary> @AggregateFunction.cs     2003-12-19
///
/// Development version 1.0
///
/// Copyright (c) 2003 by Newtera, Inc.
/// 
/// </summary>
namespace Newtera.Server.Engine.Interpreter.Builtin.Db
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Data;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Vdom.Dbimp;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary> This abstract class <code>OneArgFunction</code> is used to facilitate 
	/// the single argument function's creation. It contains of the common 
	/// methods for such functions.
	/// </summary>
	/// <version>  1.0 2003-9-10 </version>
	/// <author>  Yong Zhang </author>
	public abstract class AggregateFunction : FunctionImpBase, IDBFunction, ITraceable
	{
		private TreeManager _treeManager = null;
		private SQLElement _functionElement = null;
		private AggregateFuncEntity _functionEntity = null;
		private IDataProvider _dataProvider = null;
		private IExpr _context = null;
		private PathEnumerator _funcPathEnumerator = null;

		/// <summary>
		/// The default constructor
		/// </summary>
		public AggregateFunction() : base()
		{
		}

		/// <summary>
		/// Set the tree manager for retrieving an entity using a path.
		/// </summary>
		/// <value>the tree manager </value>
		public TreeManager TreeManager
		{
			set
			{
				_treeManager = value;
			}
		}

		/// <summary>
		/// Set the data provider.
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
		/// <value> an SQLElement.</value>
		public SQLElement SQLElement
		{
			get
			{
				// Create a function field for the aggregate function
				DBEntity lastEntity = _functionEntity.LastEntity;
				_functionElement = new ResultFieldName(lastEntity.ColumnName, lastEntity.OwnerClass.Alias, lastEntity.Type, _functionEntity.Name, _dataProvider);
				_functionElement.ClassEntity = lastEntity.OwnerClass; //Make reference to its owner class
				
				return _functionElement;
			}
		}

		/// <summary>
		/// Gets the DBEntity for the function
		/// </summary>
		/// <value> a DBEntity </value>
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
		public abstract SQLFunction Type
		{
			get;
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			// There must be one argument
			if (arguments.Count != 1)
			{
				throw new InterpreterException("Aggregate function expects one argument, but got " + arguments.Count);
			}
			
			if (!(arguments[0] is Path))
			{
				throw new InterpreterException("Aggregate function expects the argument to be a path");
			}			
		}
		
		/// <summary>
		/// prepare the function.
		/// </summary>
		public override void Prepare()
		{		
			Path path = ((Path) _arguments[0]);

			/*
			 * The aggregate function is treated as a function step that is
			 * inserted into the absolute path enumerator. For example:
			 * 
			 * for $c in document("db://schema.xml")/CustomerList/Customer where avg($c/@orders=>Order/Price) > 300 return $c
			 * 
			 * The absolute path enumerator for the parameter of avg function will be:
			 * 
			 * //Customer/function::avg/@orders=>Order/Price
			 * 
			 * The base path enumerator for the parameter of avg function will be:
			 * 
			 * //Customer/function::avg
			 * 
			 * instead of //Customer
			 * 
			 * To do so, we make the original context of the path paramter as the context
			 * of this aggregate function and then make the aggregate function as the
			 * context of the path paramter, therefore, when getting an absolute path
			 * enumerator on the path parameter, the aggregate function will add a new step
			 * in between.
			 */
			this._context = path.Context;
			path.Context = this;

			// Prepare the argument
			_arguments.Prepare();

			// prepare the function
            Value val = path.TraceDocument();
            if (val != null)
            {
                VDocument doc = val.ToNode() as VDocument;
                if (doc != null)
                {
                    doc.PrepareFunction(this, _arguments);
                }
            }
		}
		
		/// <summary>
		/// This is called during visiting qualifiers in a query and allows to build
		/// a SQL equivalent function for the aggregate function.
		/// </summary>
		public override void Restrict()
		{
			Path path = ((Path) _arguments[0]);

			/*
			* Find the function entity in the tree that was created during the prepare
			* phase.
			*/
			PathEnumerator basePathEnumerator = path.GetBasePathEnumerator();
			_functionEntity = (AggregateFuncEntity) _treeManager.FindEntity(basePathEnumerator);
			
			// prepare the qualifiers contained in the path
			PathEnumerator pathEnumerator = path.GetLocalPathEnumerator();
			
			pathEnumerator.Reset();
			while (pathEnumerator.MoveNext())
			{
				Step step = (Step) pathEnumerator.Current;
				IExpr qualifier = step.Qualifier;
				if (qualifier != null)
				{
					// add the qualifier to the subquery indicated by the func entity
					_treeManager.PrepareQualifier(qualifier, _functionEntity);
				}
			}
		}
		
		/// <summary>
		/// The result should be one of the interpreter Value types, see Value
		/// and its subclasses.
		/// </summary>
		/// <returns> a Value object as result of aggregate function.</returns>
		public override Value Eval()
		{
			Value val = _context.Eval();

			XCollection nodes = (XCollection) ((ISelectable) val).SelectNodes(_funcPathEnumerator);
			
			if (nodes.ToCollection().Count == 1)
			{
				return new XString(nodes.ToCollection()[0].ToString());
			}
			else
			{
				throw new InterpreterException("Expecting 1 row result for an aggregate function, but got " + nodes.ToCollection().Count);
			}
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the aggregate function.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			if (_context != null)
			{
				return ((ITraceable) _context).TraceDocument();
			}
			else
			{
				throw new InterpreterException("The context does not exist. The aggregate function needs to be prepared first.");
			}
		}

		/// <summary>
		/// Get an enumerator of the absolute path that includes a step for the function
		/// </summary>
		/// <returns>The PathEnumerator</returns>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			if (_context != null)
			{
				PathEnumerator pathEnumerator = ((ITraceable) _context).GetAbsolutePathEnumerator();

				// add a step representing the function
				
				if (_funcPathEnumerator == null)
				{
					_funcPathEnumerator = this.GetFunctionPathEnumerator();
				}

				pathEnumerator.Append(_funcPathEnumerator);

				return pathEnumerator;
			}
			else
			{
				throw new InterpreterException("The context does not exist. The aggregate function needs to be prepared first.");
			}
		}

		#endregion

		/// <summary>
		/// Gets a PathEnumerator for the function step
		/// </summary>
		/// <returns>A PathEnumerator</returns>
		private PathEnumerator GetFunctionPathEnumerator()
		{
			string stepName = AggregateFuncEntity.GetFunctionName(this.Type);
			Step step = new Step(this.Interpreter, stepName, null, "/function::");
			Path path = new Path(this.Interpreter, step, this._context);
			return path.GetLocalPathEnumerator();
		}
	}
}
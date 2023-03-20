/*
* @(#)Flwr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Represents a FLWR expression.
	/// </summary>
	/// <version> 1.0.0 15 Aug 2003 </version>
	/// <author> Yong Zhang </author>
	public class Flwr : ExprBase, ITraceable
	{
		private IExpr _flClause;
		private IExpr _where;
		private IExpr _return;
		private IExpr _sortBy;
        private UseWhereEnum _useWhere;

		/// <summary>
		/// Initiate an instance of Flwr object.
		/// </summary>
		/// <param name="flClause">The FLClause</param>
		/// <param name="where">Optional where clause</param>
		/// <param name="returnClause">The return clause</param>
		/// <param name="sortByClause">The optional sortby clause</param> 
		public Flwr(Interpreter interpreter, IExpr flClause, IExpr where, IExpr returnClause, IExpr sortby) : base(interpreter)
		{
			_flClause = flClause;
			_where = where;
			_return = returnClause;
			_sortBy = sortby;
            _useWhere = UseWhereEnum.Unknow;
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.FLWR;
			}
		}

		/// <summary>
		/// Prepare the flwr expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_flClause != null)
			{
				_flClause.Prepare();
			}

			if (_where != null)
			{
				// Prepare qualifier here
				_where.Prepare();
			}

			if (_return != null)
			{
				_return.Prepare();
			}

			if (_sortBy != null)
			{
				_sortBy.Prepare();
			}
		}

		/// <summary>
		/// Restrict the flwr expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_flClause != null)
			{
				_flClause.Restrict();
			}

			if (_where != null)
			{
				// invoke prepareQualifier method on each dictinct document traced from
				// it for or/and let clauses.
				ValueCollection docs = ((ITraceable) _flClause).TraceDocument().ToCollection();

				// TODO, this may have problems with multiple documents
				foreach (XNode node in docs)
				{
					((VDocument) node.ToNode()).PrepareQualifier(_where);
				}
			}

			if (_return != null)
			{
				_return.Restrict();	
			}
		}

		/// <summary>
		/// Evaluate the flwr expression in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			ValueCollection returnValues = new ValueCollection();

			// start iteration
			((FLClause) _flClause).Reset();
			while (_flClause.Eval().ToBoolean())
			{
                // the vdocument will be loaded after the first call of SelectNodes method,
                // UseWhere is called here after vdocument is loaded
				if (_where == null ||
                    !UseWhere ||
                    _where.Eval().ToBoolean())
				{
					returnValues.Add(_return.Eval());
				}
			}

			// TODO, sortby has been taken care by SQL if the data source is a
			// database. For non-database source, we need to take care of sort by here
			//_sortBy.Eval()

			return new XCollection(returnValues);
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			//System.Console.WriteLine(_left.Eval().ToString() + " " + _operator + " " + _right.Eval().ToString());
		}

		/// <summary>
		/// Gets the information indicating whether the whereClause should be used
		/// in the evaluation of flwr. For the DocumentDB, the returned nodes have been
		/// pre-selected using SQL statement(s), therefore, using whereClause to filter
		/// these nodes are redundant, unless the document contains more than one ClassNameList
        /// elements for the same class as a result of a self-referenced relationships
		/// </summary>
		/// <value></value>
		private bool UseWhere
		{
			get
			{
				bool status = false;

                switch (_useWhere)
                {
                    case UseWhereEnum.Unknow:

                        ValueCollection docs = ((ITraceable)_flClause).TraceDocument().ToCollection();

                        foreach (XNode node in docs)
                        {
                            VDocument vdoc = (VDocument)node.ToNode();
                            if (!vdoc.IsDB)
                            {
                                // non db type document found, use where during evaluation
                                _useWhere = UseWhereEnum.Yes;
                                status = true;
                                break;
                            }
                            else if (vdoc.IsLoaded && vdoc.HasDuplicatedRootElements)
                            {
                                // has duplicated root elements as result of a self-referenced
                                // relationship, use Where clause during the evaluation
                                _useWhere = UseWhereEnum.Yes;
                                status = true;
                                break;
                            }
                        }

                        if (!status)
                        {
                            // avoid computation next time UseWhere is called
                            _useWhere = UseWhereEnum.No;
                        }

                        break;

                    case UseWhereEnum.Yes:
                        status = true;
                        break;

                    case UseWhereEnum.No:
                        status = false;
                        break;
                }

				return status;
			}
		}

		#region ITraceable Members

		public Value TraceDocument()
		{
			if (_flClause is ITraceable)
			{
				return ((ITraceable) _flClause).TraceDocument();
			}
			else
			{
				return null;
			}
		}

		public PathEnumerator GetAbsolutePathEnumerator()
		{
			throw new InterpreterException("Invalid method call");
		}

		#endregion
	}

    internal enum UseWhereEnum
    {
        Unknow,
        Yes,
        No
    }
}
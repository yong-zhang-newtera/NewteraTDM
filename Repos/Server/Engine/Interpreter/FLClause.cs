/*
* @(#)FLClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a combination of For and/or Let clause which is part of a FLWR statement.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class FLClause : ExprBase, ITraceable
	{
		private Stack _stack;
		private IList _exprs;

		/// <summary>
		/// Initiate an instance of FLClause object.
		/// </summary>
		public FLClause(Interpreter interpreter) : base(interpreter)
		{
			_stack = new Stack();
			_exprs = new ArrayList();
		}

		/// <summary>
		/// Add a for/let expression in order of outermost to innermost
		/// </summary>
		/// <param name="index">The index to which to insert the expression</param>
		/// <param name="expr"></param>
		public void InsertExpr(int index, IExpr expr)
		{
			_exprs.Insert(index, expr);
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.FL;
			}
		}

		/// <summary>
		/// Prepare the FLClause in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			foreach(IExpr expr in _exprs)
			{
				expr.Prepare();
			}
		}

		/// <summary>
		/// Restrict the For expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			foreach(IExpr expr in _exprs)
			{
				expr.Restrict();
			}
		}

		/// <summary>
		/// Evaluate each for or let expr in the list. The side effect is that the identifiers
		/// declared in each for or let expr are assigned a new Value object so that the
		/// corresponding variables in symbol table is updated.
		/// 
		/// It also takes care of nested loops by using a stack
		/// </summary>
		/// <returns>An XBoolean contains false if the loop is ended, true otherwise. </returns>
		public override Value Eval()
		{			
			// start looping
			bool status = MoveNext();

			return new XBoolean(status);
		}

		/// <summary>
		/// Reset the stack for iterating from beginning.
		/// </summary>
		public void Reset()
		{
			// initialize the stack by pushing expr from outermost to innermost
			int i;
			for (i = 0; i < _exprs.Count - 1; i++)
			{
				IExpr expr = (IExpr) _exprs[i];
				// eval the expr so that the identifier in expr is initialized
				expr.Eval();
				_stack.Push(expr);
			}

			// push the inner-most expr without eval
			_stack.Push(_exprs[i]);
		}

		/// <summary>
		/// loop through the expressions on the stack
		/// </summary>
		/// <returns>true if it has not reached the end of the loop, false otherwise.</returns>
		private bool MoveNext()
		{
			bool status = false;

			while (_stack.Count > 0)
			{
				IExpr expr = (IExpr) _stack.Peek();
				
				if (expr.Eval().ToBoolean())
				{
					if (_stack.Count < _exprs.Count)
					{
						// it isn't a full stack, push the inner loop onto stack
						_stack.Push(_exprs[_stack.Count]);
					}
					else
					{
						// move to next successfully
						status = true;
						break;
					}
				}
				else
				{
					_stack.Pop();
				}
			}

			return status;
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{

			System.Console.WriteLine("FLClause");
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document(s) of the identifiers defined by for and let clauses.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			ValueCollection values = new ValueCollection();

			// go through each for clause to get owner document of each variable
			// of the clause. Because the variables of for clauses could have
			// different owner documents. Only collect the distinct owner documents and
			// return them.
			foreach (IExpr expr in _exprs)
			{
				if (expr is For)
				{
					XNode docNode = (XNode) ((ITraceable) expr).TraceDocument();

					if (docNode != null)
					{
						// make sure the document has not been added
						bool existing = false;
						foreach(XNode node in values)
						{
							if (node.ToNode() == docNode.ToNode())
							{
								existing = true;
								break;
							}
						}

						if (!existing)
						{
							values.Add(docNode);
						}
					}
				}
			}

			return new XCollection(values);
		}

		/// <summary>
		/// Get an enumerator of the absolute path, including the base path referenced
		/// by the context.
		/// </summary>
		/// <exception cref="InterpreterException">This method shouldn't be called</exception>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			throw new InterpreterException("Invalid method call");
		}

		#endregion
	}
}
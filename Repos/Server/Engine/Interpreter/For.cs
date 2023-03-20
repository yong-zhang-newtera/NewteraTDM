/*
* @(#)For.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a For expression which is part of a FLWR statement.
	/// 
	/// FOR takes a sequence and assigns the sequence values to the variables in order,
	/// evaluating once per variable assignment.
	///
	/// There is only a single variable per For expression - see explanation
	/// in FLWR.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	/// <remarks>Also see FLClause</remarks>
	public class For : ExprBase, ITraceable
	{
		private IExpr _ident;
		private IExpr _expr;
		private IEnumerator _valuesEnum;

		/// <summary>
		/// Initiate an instance of For object.
		/// </summary>
		/// <param name="ident">The identifier of For</param>
		/// <param name="expr"> The expression of For</param> 
		public For(Interpreter interpreter, IExpr ident, IExpr expr) : base(interpreter)
		{
			_ident = ident;
			_expr = expr;
			_valuesEnum = null;
			((Ident) _ident).Expression = _expr; // assign the expression to the identifier
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.FOR;
			}
		}

		/// <summary>
		/// Prepare the For expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_ident != null)
			{
				_ident.Prepare();
			}

			if (_expr != null)
			{
				_expr.Prepare();
			}
		}

		/// <summary>
		/// Restrict the For expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_ident != null)
			{
				_ident.Restrict();
			}

			if (_expr != null)
			{
				_expr.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the For expression in the phase three of interpreting.
		/// It first evaluate the right-side expression to get a collection of Value objects
		/// , then assign a Value object to the identifier each time the Eval is called until
		/// the end of collection is reached.
		/// </summary>
		/// <returns>An XBoolean contains false if it reaches the end of iteration, true otherwise. </returns>
		public override Value Eval()
		{
			bool status = false;

			if (_valuesEnum == null)
			{
				// evaluate the right-sided expr to get the collection of values
				Value val = _expr.Eval();
				if (val.DataType.IsCollection)
				{
					_valuesEnum = val.ToCollection().GetEnumerator();
				}
				else
				{
					ValueCollection collection = new ValueCollection();
					collection.Add(val);
					_valuesEnum = collection.GetEnumerator();
				}
			}

			// assign a value to the indentifier
			if (_valuesEnum.MoveNext())
			{
				status = true;
				_ident.Value = (Value) _valuesEnum.Current;
			}
			else
			{
				_valuesEnum = null;
                _ident.Value = null; // end of for loop, clear up the identifier value
			}

			return new XBoolean(status);
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("for $" + ((Ident) _ident).Name + " in " + _expr.Eval().ToString());
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the variable of the for clause.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			return ((ITraceable) _ident).TraceDocument();
		}

		/// <summary>
		/// Get an enumerator of the absolute path represented by the variable of the for clause.
		/// </summary>
		/// <returns>The PathEnumerator</returns>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			return ((ITraceable) _ident).GetAbsolutePathEnumerator();
		}

		#endregion
	}
}
/*
* @(#)Let.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a Let expression which is part of a FLWR statement.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	/// <remarks>Also see FLClause</remarks>
	public class Let : ExprBase, ITraceable
	{
		private IExpr _ident;
		private IExpr _expr;
		private bool _isInitialized;

		/// <summary>
		/// Initiate an instance of Let object.
		/// </summary>
		/// <param name="ident">The identifier of Let</param>
		/// <param name="expr"> The expression of Let</param> 
		public Let(Interpreter interpreter, IExpr ident, IExpr expr) : base(interpreter)
		{
			_ident = ident;
			_expr = expr;
			_isInitialized = false;
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
				return ExprType.LET;
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
		/// Evaluate the Let expression in the phase three of interpreting.
		/// It first evaluate the right-side expression to get a Value object.
		/// Then assign a Value object to the identifier.
		/// </summary>
		/// <returns>An XBoolean contains false if the value has been assigned, true otherwise. </returns>
		public override Value Eval()
		{
			if (!_isInitialized)
			{
				// evaluate the right-sided expr to get Value object
				Value val = _expr.Eval();
				_ident.Value = val;
				_isInitialized = true;

				return new XBoolean(true);
			}
			else
			{
				_isInitialized = false;

				return new XBoolean(false);
			}
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("let $" + ((Ident) _ident).Name + " in " + _expr.Eval().ToString());
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the variable of the let clause.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			return ((ITraceable) _ident).TraceDocument();
		}

		/// <summary>
		/// Get an enumerator of the absolute path represented by the variable of the let clause.
		/// </summary>
		/// <returns>The PathEnumerator</returns>
		public PathEnumerator GetAbsolutePathEnumerator()
		{
			return ((ITraceable) _ident).GetAbsolutePathEnumerator();
		}

		#endregion
	}
}
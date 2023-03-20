/*
* @(#)If.cs
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
	/// Represents an If expression.
	/// </summary>
	/// <version> 1.0.0 15 Aug 2003 </version>
	/// <author> Yong Zhang </author>
	public class If : ExprBase
	{
		private IExpr _condition;
		private IExpr _thenClause;
		private IExpr _elseClause;

		/// <summary>
		/// Initiate an instance of If object.
		/// </summary>
		/// <param name="condition">the condition if if expression</param>
		/// <param name="thenClause">The then clause</param>
		/// <param name="elseClause">The else clause</param> 
		public If(Interpreter interpreter, IExpr condition, IExpr thenClause, IExpr elseClause) : base(interpreter)
		{
			_condition = condition;
			_thenClause = thenClause;
			_elseClause = elseClause;
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.IF;
			}
		}

		/// <summary>
		/// Prepare the flwr expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_condition != null)
			{
				_condition.Prepare();
			}

			if (_thenClause != null)
			{
				// Prepare then clause here
				_thenClause.Prepare();
			}

			if (_elseClause != null)
			{
				_elseClause.Prepare();
			}
		}

		/// <summary>
		/// Restrict the flwr expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_condition != null)
			{
				_condition.Restrict();
			}

			if (_thenClause != null)
			{
				_thenClause.Restrict();
			}

			if (_elseClause != null)
			{
				_elseClause.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the if expression in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			if (_condition.Eval().ToBoolean())
			{
				return _thenClause.Eval();
			}
			else
			{
				return _elseClause.Eval();
			}
		}

		/// <summary>
		/// Print the information about the if expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			//System.Console.WriteLine(_left.Eval().ToString() + " " + _operator + " " + _right.Eval().ToString());
		}
	}
}
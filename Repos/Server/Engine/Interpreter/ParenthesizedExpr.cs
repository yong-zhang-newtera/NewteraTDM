/*
* @(#)ParenthesizedExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an unary minus expression.
	/// </summary>
	/// <version> 1.0.0 16 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class ParenthesizedExpr : Unary
	{
		/// <summary>
		/// Initiate an instance of ParenthesizedExpr object.
		/// </summary>
		/// <param name="term">the unary operand</param> 
		public ParenthesizedExpr(Interpreter interpreter, IExpr term) : base(interpreter, term)
		{
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.PARENTHESIZED;
			}
		}

		/// <summary>
		/// Prepare the additive expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_term != null)
			{
				_term.Prepare();
			}
		}

		/// <summary>
		/// Restrict the additive expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_term != null)
			{
				_term.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the additive expression in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			return _term.Eval();
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("(" + _term.Eval().ToString() + ")");
		}
	}
}
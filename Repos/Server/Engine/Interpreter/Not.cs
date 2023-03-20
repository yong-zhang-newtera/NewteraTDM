/*
* @(#)Not.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an unary not expression.
	/// </summary>
	/// <version> 1.0.0 16 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class Not : Unary
	{
		/// <summary>
		/// Initiate an instance of Not class which operates on a XBoolean value
		/// </summary>
		/// <param name="term">the unary operand</param> 
		public Not(Interpreter interpreter, IExpr term) : base(interpreter, term)
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
				return ExprType.NOT;
			}
		}

		/// <summary>
		/// Evaluate the not expression in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			Value val = _term.Eval();
			DataType type = val.DataType;

			return type.Not(val);
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("!" + _term.Eval().ToString());
		}
	}
}
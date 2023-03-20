/*
* @(#)Mod.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an Mod expression
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class Mod : BinaryExpr
	{
		/// <summary>
		/// Initiate an instance of Mod object.
		/// </summary>
		public Mod(Interpreter interpreter, IExpr left, IExpr right) : base(interpreter, left, right)
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
				return ExprType.MOD;
			}
		}

		/// <summary>
		/// Perform the mod operation
		/// </summary>
		/// <param name="left">The expr for left operand</param>
		/// <param name="right">The expr for right operand</param>
		/// <returns>The Value object</returns>
		protected override Value DoOp(IExpr left, IExpr right)
		{
			Value lval = left.Eval();
			Value rval = right.Eval();
			DataType dataType = rval.DataType;

			return dataType.Mod(lval,  rval);
		}

		/// <summary>
		/// Get the operator string
		/// </summary>
		/// <value>The string representation of the operator</value>
		protected override string Operator
		{
			get
			{
				return "%";
			}
		}
	}
}
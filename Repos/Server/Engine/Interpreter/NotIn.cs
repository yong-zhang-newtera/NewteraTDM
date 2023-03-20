/*
* @(#)NotIn.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a NotIn expression
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class NotIn : BinaryExpr
	{
		/// <summary>
		/// Initiate an instance of NotIn object.
		/// </summary>
		public NotIn(Interpreter interpreter, IExpr left, IExpr right) : base(interpreter, left, right)
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
				return ExprType.NOTIN;
			}
		}

		/// <summary>
		/// Perform the relational operation
		/// </summary>
		/// <param name="left">The expr for left operand</param>
		/// <param name="right">The expr for right operand</param>
		/// <returns>The XBoolean object</returns>
		protected override Value DoOp(IExpr left, IExpr right)
		{
			Value lval = left.Eval();
			Value rval = right.Eval();
			DataType dataType = rval.DataType;

			return dataType.Ni(lval,  rval);
		}

		/// <summary>
		/// Get the operator string
		/// </summary>
		/// <value>The string representation of the operator</value>
		protected override string Operator
		{
			get
			{
				return "ni";
			}
		}
	}
}
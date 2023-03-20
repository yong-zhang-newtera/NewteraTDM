/*
* @(#)UTrue.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an unary expression that always evals to boolean true.
	/// </summary>
	/// <version> 1.0.0 16 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class UTrue : Unary
	{
		/// <summary>
		/// Initiate an instance of UTrue object.
		/// </summary>
		public UTrue(Interpreter interpreter, IExpr term) : base(interpreter, term)
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
				return ExprType.TRUE;
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
		/// Evaluate the UTrue expression in the phase three of interpreting
		/// </summary>
		/// <returns>Always return a true</returns>
		public override Value Eval()
		{
			return new XBoolean(true);
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("true");
		}
	}
}
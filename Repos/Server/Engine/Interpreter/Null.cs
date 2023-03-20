/*
* @(#)Null.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a null keyword discovered by parser. Null is a special literal
	/// </summary>
	/// <version> 1.0.0 02 Sep 2003 </version>
	/// <author> Yong Zhang</author>
	public class Null : ExprBase
	{
		/// <summary>
		/// Initiate an instance of Null object.
		/// </summary>
		public Null(Interpreter interpreter) : base(interpreter)
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
				return ExprType.NULL;
			}
		}

		/// <summary>
		/// Prepare the literal in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
		}

		/// <summary>
		/// Restrict the literal in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
		}

		/// <summary>
		/// Evaluate the null in the phase three of interpreting
		/// </summary>
		/// <returns>A null value</returns>
		public override Value Eval()
		{
			return new XString("");
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Null");
		}
	}
}
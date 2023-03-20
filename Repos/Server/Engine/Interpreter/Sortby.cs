/*
* @(#)Sortby.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a Sortby expression which is part of a FLWR statement.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	/// <remarks>Also see Flwr</remarks>
	public class Sortby : ExprBase
	{
		private ExprCollection _sortbyFields;

		/// <summary>
		/// Initiate an instance of Sortby object.
		/// </summary>
		/// <param name="sortbyFields"></param>
		public Sortby(Interpreter interpreter, ExprCollection sortbyFields) : base(interpreter)
		{
			_sortbyFields = sortbyFields;
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.SORTBY;
			}
		}

		/// <summary>
		/// Prepare the For expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_sortbyFields != null)
			{
				_sortbyFields.Prepare();
			}
		}

		/// <summary>
		/// Restrict the For expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_sortbyFields != null)
			{
				_sortbyFields.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the sortby expression in the phase three of interpretin
		/// </summary>
		/// <returns></returns>
		public override Value Eval()
		{
			// Not supposed to be called, throw an exception
			throw new InvalidOperationException("Eval on sortby expression is invalid");
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("sortby(" +  _sortbyFields.Eval().ToString() + ")");
		}
	}
}
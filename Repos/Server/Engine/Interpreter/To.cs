/*
* @(#)To.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a range expression created by (n TO m), where n and m is an integer.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class To : ExprBase, ITraversable
	{
		protected IExpr _left;
		protected IExpr _right;
		protected string _operator;

		/// <summary>
		/// Initiate an instance of AddExpr object.
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param> 
		public To(Interpreter interpreter, IExpr left, IExpr right) : base(interpreter)
		{
			_left = left;
			_right = right;
			_operator = "to";
		}

		public bool IsDefinedFor(DataType left, DataType right)
		{
			if (left == null || right == null)
			{
				throw new InterpreterException("Invalid operands for additive expression");
			}

			return true;
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public override ExprType ExprType
		{
			get
			{
				return ExprType.TO;
			}
		}

		/// <summary>
		/// Prepare the additive expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_left != null)
			{
				_left.Prepare();
			}

			if (_right != null)
			{
				_right.Prepare();
			}
		}

		/// <summary>
		/// Restrict the additive expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_left != null)
			{
				_left.Restrict();
			}

			if (_right != null)
			{
				_right.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the additive expression in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			if (_left == null || _right == null)
			{
				throw new InterpreterException("Null values are not allowed for additive op " + _operator);
			}

			// This expression is usually part of path predicate and is used by SQL builder
			// to get a range of records. therefore, we make it return a true value here
			return new XBoolean(true);
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine(_left.Eval().ToString() + " to " + _right.Eval().ToString());
		}

		/// <summary>
		/// Gets the start of range
		/// </summary>
		/// <returns>An integer value</returns>
		public int GetFromValue()
		{
			return _left.Eval().ToInt();
		}

		/// <summary>
		/// Gets the end of range
		/// </summary>
		/// <returns>An integer value</returns>
		public int GetToValue()
		{
			return _right.Eval().ToInt();
		}

		#region ITraversable Members

		/// <summary> 
		/// Gets child count the expression.
		/// </summary>
		/// <value> Child count </returns>
		public int ChildCount
		{
			get
			{
				return 2;
			}
		}

		/// <summary> 
		/// Accept an ExprVisitor that will traverse the expression
		/// </summary>
		/// <param name="visitor">The ExprVisitor</param>
		public void Accept(IExprVisitor visitor)
		{
			if (visitor.Visit(this))
			{
				visitor.Visit(_left);

				visitor.Visit(_right);
			}
		}

		#endregion
	}
}
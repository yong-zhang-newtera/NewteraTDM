/*
* @(#)Unary.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a base class for all unary expressions, such as, UMinus, Not, etc.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	abstract public class Unary : ExprBase, ITraversable
	{
		protected IExpr _term;

		/// <summary>
		/// Initiate an instance of Unary object.
		/// </summary>
		/// <param name="term">the unary operand</param> 
		public Unary(Interpreter interpreter, IExpr operand) : base(interpreter)
		{
			_term = operand;
		}

		/// <summary>
		/// Check the type of expression
		/// </summary>
		/// <param name="type">The type of the operand</param>
		/// <returns>true if it is a right type, false otherwise</returns>
		public virtual bool IsDefinedFor(DataType type)
		{
			if (type == null)
			{
				throw new InterpreterException("Invalid operands for unary minus expression");
			}

			return true;
		}

		/// <summary>
		/// Prepare the not expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (_term != null)
			{
				_term.Prepare();
			}
		}

		/// <summary>
		/// Restrict the not expression in the phase two of interpreting
		/// </summary>
		public override void Restrict()
		{
			if (_term != null)
			{
				_term.Restrict();
			}
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
				return 1;
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
				if (_term is ITraversable)
				{
					((ITraversable) _term).Accept(visitor);
				}
				else
				{
					visitor.Visit(_term);
				}
			}
		}

		#endregion
	}
}
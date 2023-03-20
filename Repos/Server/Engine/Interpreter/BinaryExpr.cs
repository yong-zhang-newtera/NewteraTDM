/*
* @(#)BinaryExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a base class for all binary expressions which has left and right operands.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	abstract public class BinaryExpr : ExprBase, ITraversable
	{
		protected IExpr _left = null;
		protected IExpr _right = null;

		/// <summary>
		/// Initiate an instance of BinaryExpr object.
		/// </summary>
		public BinaryExpr(Interpreter interpreter, IExpr left, IExpr right) : base(interpreter)
		{
			_left = left;
			_right = right;
		}

		public bool IsDefinedFor(DataType left, DataType right)
		{
			if (left == null || right == null)
			{
				throw new InterpreterException("Invalid operands for relational expression");
			}

			return true;
		}

		/// <summary>
		/// Gets the left operand of the binary expression
		/// </summary>
		public IExpr Left
		{
			get
			{				
				return _left;
			}
		}

		/// <summary>
		/// Gets the right operand of the binary expression
		/// </summary>
		public IExpr Right
		{
			get
			{
				return _right;
			}
		}

		/// <summary>
		/// Prepare the relational expression in the phase one of interpreting
		/// </summary>
		public override void Prepare()
		{
			if (Left != null)
			{
				Left.Prepare();
			}

			if (Right != null)
			{
				Right.Prepare();
			}
		}

		/// <summary>
		/// Restrict the relational expression in the phase two of interpreting
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
		/// Evaluate the relational expression in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			if (Left == null || Right == null)
			{
				throw new InterpreterException("Null values are not allowed for relational op " + Operator);
			}

			return DoOp(Left, Right);
		}

		/// <summary>
		/// Print the information about the rel expression for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine(_left.Eval().ToString() + " " + Operator + " " + _right.Eval().ToString());
		}

		/// <summary>
		/// Perform the relational operation
		/// </summary>
		/// <param name="left">The expr for left operand</param>
		/// <param name="right">The expr for right operand</param>
		/// <returns>The XBoolean object</returns>
		abstract protected Value DoOp(IExpr left, IExpr right);

		abstract protected string Operator { get;}

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
				if (_left is ITraversable)
				{
					((ITraversable) _left).Accept(visitor);
				}
				else
				{
					visitor.Visit(_left);
				}

				if (_right is ITraversable)
				{
					((ITraversable) _right).Accept(visitor);
				}
				else
				{
					visitor.Visit(_right);
				}
			}
		}

		#endregion
	}
}
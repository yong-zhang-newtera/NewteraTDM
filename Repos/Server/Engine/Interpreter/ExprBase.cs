/*
* @(#)ExprBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;

	/// <summary>
	/// This a base class for all tree nodes created by ANTLR parser. It implements
	/// AST interface so that the parser can treat the all tree nodes uniformly
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang </author>
	abstract public class ExprBase : IExpr, ICloneable
	{
		private Interpreter _interpreter = null;
		private DataType _type = null;
		private Value _value = null;
		
		/// <summary>
		/// Initiate an instance of ExprBase object.
		/// </summary>
		public ExprBase() : base()
		{
		}
	
		/// <summary>
		/// Initiate an instance of ExprBase object.
		/// </summary>
		public ExprBase(Interpreter interpreter) : base()
		{
			_interpreter = interpreter;
		}

		/// <summary>
		/// Initiate an instance of ExprBase object.
		/// </summary>
		/// <param name="type">The data type</param>
		public ExprBase(Interpreter interpreter, DataType type) : base()
		{
			_interpreter = interpreter;
			_type = type;
			_value = null;
		}

		/// <summary>
		/// Initiate an instance of ExprBase object.
		/// </summary>
		/// <param name="type">The data type</param>
		/// <param name="val">The value of expression</param>
		public ExprBase(Interpreter interpreter, DataType type, Value val) : base()
		{
			_interpreter = interpreter;
			_type = type;
			_value = val;
		}

		#region IExpr Members

		/// <summary>
		/// Gets the interpreter that stores the symbol table
		/// </summary>
		public Interpreter Interpreter
		{
			get
			{
				return _interpreter;
			}
			set
			{
				_interpreter = value;
			}
		}

		/// <summary>
		/// Gets or sets the data type of the expression.
		/// </summary>
		/// <value>The DataType object</value>
		public DataType DataType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Gets or sets the data value of the expression.
		/// </summary>
		/// <value>The Value object</value>
		public virtual Value Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public abstract ExprType ExprType
		{
			get;
		}

		/// <summary>
		/// Prepare the expression in the phase one of interpreting
		/// </summary>
		public abstract void Prepare();

		/// <summary>
		/// Restrict the expression in the phase two of interpreting
		/// </summary>
		public abstract void Restrict();

		/// <summary>
		/// Evaluate the expression in the phase three of interpreting
		/// </summary>
		public abstract Value Eval();

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public abstract void Print();

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return null;
		}

		#endregion
	}
}
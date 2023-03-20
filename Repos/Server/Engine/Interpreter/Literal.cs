/*
* @(#)Literal.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a variable discovered by parser. It is stored as an entry in the symbol
	/// table. A variable is defined using let expression of XQuery (let $var := expression).
	/// . Before evaluation, it is associated with parser tree node representing the
	/// expression. After evalutation it is associated with a Value object resulting from
	/// the associated expression.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class Literal : ExprBase
	{
		/// <summary>
		/// Initiate an instance of Literal object, and infer the DataType and Value
		/// object based on the literal value.
		/// </summary>
		/// <param name="interpreter">The interpreter</param>
		/// <param name="literalVal"></param>
		public Literal(Interpreter interpreter, string literalVal) : base(interpreter)
		{
			SetDataType(literalVal);
		}

		/// <summary>
		/// Initiate an instance of Literal object.
		/// </summary>
		/// <param name="interpreter">The interpreter.</param>
		/// <param name="name">The data type of the Literal</param>
		public Literal(Interpreter interpreter, DataType type, Value val) : base(interpreter, type, val)
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
				return ExprType.LITERAL;
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
		/// Evaluate the literal in the phase three of interpreting
		/// </summary>
		public override Value Eval()
		{
			return this.Value;
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Literal with value " + Eval().ToString());
		}

		/// <summary>
		/// Set the data type according to the literal value
		/// </summary>
		/// <param name="literalValue"></param>
		private void SetDataType(string literalValue)
		{
			bool completed = false;

			try
			{
				bool boolVal = Boolean.Parse(literalValue);
				this.DataType = new BooleanType();
				this.Value = new XBoolean(boolVal);
				completed = true;
			}
			catch (Exception)
			{
				completed = false;
			}

			if (!completed)
			{
				try
				{
					int intVal = Int32.Parse(literalValue);
					this.DataType = new IntegerType();
					this.Value = new XInteger(intVal);
					completed = true;
				}
				catch (Exception)
				{
					completed = false;
				}
			}

			if (!completed)
			{
				try
				{
					float floatVal = Single.Parse(literalValue);
					this.DataType = new FloatType();
					this.Value = new XFloat(floatVal);
					completed = true;
				}
				catch (Exception)
				{
					completed = false;
				}
			}

			if (!completed)
			{
				try
				{
					double doubleVal = Double.Parse(literalValue);
					this.DataType = new DoubleType();
					this.Value = new XDouble(doubleVal);
					completed = true;
				}
				catch (Exception)
				{
					completed = false;
				}
			}
		}
	}
}
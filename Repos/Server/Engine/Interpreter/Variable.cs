/*
* @(#)Variable.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	// Declare a delegate for variable's value change event notification
	public delegate void ValueChangeEventHandler(string varName, Value newVal);

	/// <summary>
	/// Represents a variable discovered by parser. It is stored as an entry in the symbol
	/// table. A variable is defined using let expression of XQuery (let $var := expression).
	/// . Before evaluation, it is associated with parser tree node representing the
	/// expression. After evalutation it is associated with a Value object resulting from
	/// the associated expression.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class Variable : SymbolTableEntry
	{
		public event ValueChangeEventHandler ValueChange = null;

		/// <summary>
		/// Initiate an instance of Variable object.
		/// </summary>
		/// <param name="name">The name of a variable</param>
		/// <remarks>The $ sign is not included in the variable name</remarks>
		public Variable(string name) : base(name)
		{
		}

		/// <summary>
		/// Gets the information indicating whether the symbol is a variable.
		/// </summary>
		/// <value>True</value>
		public override bool IsVariable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets or sets the value of the symbol.
		/// </summary>
		public override Value Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;

				// Raise the event for value change
				if (ValueChange != null)
				{
					ValueChange(Name, value);
				}
			}
		}

		/// <summary>
		/// Gets the information indicating whether the symbol is a function.
		/// </summary>
		/// <value>False</value>
		public override bool IsFunction
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Variable symbol " + this.Name);
		}
	}
}
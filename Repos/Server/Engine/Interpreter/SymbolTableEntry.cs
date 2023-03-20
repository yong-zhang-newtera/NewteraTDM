/*
* @(#)SymbolTableEntry.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// The base class for all symbol table entry classes. It provides common data members
	/// and access methods.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang </author>
	abstract public class SymbolTableEntry
	{
		private string _name;
		private Value _value;
		private IExpr _expr;

		/// <summary>
		/// Initiate an instance of SymbolTableEntry object.
		/// </summary>
		/// <param name="name">The symbol entry name</param>
		protected SymbolTableEntry(string name)
		{
			_name = name;
			_expr = null;
		}

		/// <summary>
		/// Initiate an instance of SymbolTableEntry object.
		/// </summary>
		/// <param name="name">The symbol entry name</param>
		/// <param name="expr">The expression object associated with the symbol</param>
		protected SymbolTableEntry(string name, IExpr expr)
		{
			_name = name;
			_expr = expr;
		}

		/// <summary>
		/// Gets the symbol entry name
		/// </summary>
		/// <value>The symbol entry name</value>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets or sets the expression associated with symbol
		/// </summary>
		/// <value>The associated expression</value>
		public IExpr Expression
		{
			get
			{
				return _expr;
			}
			set
			{
				_expr = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the symbol.
		/// </summary>
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
		/// Get the data type of the symbol
		/// </summary>
		public DataType Type
		{
			get
			{
				if (_value != null)
				{
					return _value.DataType;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the information indicating whether the symbol is a variable.
		/// </summary>
		/// <value>True if it is, false otherwise</value>
		public abstract bool IsVariable
		{
			get;
		}

		/// <summary>
		/// Gets the information indicating whether the symbol is a function.
		/// </summary>
		/// <value>True if it is, false otherwise</value>
		public abstract bool IsFunction
		{
			get;
		}

		/// <summary>
		/// Evaluate the expression associated with symbol to result in a value
		/// </summary>
		/// <returns>The Value object</returns>
		public Value Eval()
		{
			_value = _expr.Eval();
			return _value;
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public abstract void Print();
	}
}
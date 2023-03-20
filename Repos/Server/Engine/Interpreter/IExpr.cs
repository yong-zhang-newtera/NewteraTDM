/*
* @(#)IExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an interface for all expressions. An expression has a type and can return
	/// a value.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public interface IExpr
	{
		/// <summary>
		/// Gets the interpreter that stores the symbol table
		/// </summary>
		Interpreter Interpreter
		{
			get;
		}

		/// <summary>
		/// Gets or sets the data type of the expression.
		/// </summary>
		/// <value>The DataType object</value>
		DataType DataType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the data value of the expression.
		/// </summary>
		/// <value>The Value object</value>
		Value Value
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		ExprType ExprType
		{
			get;
		}

		/// <summary>
		/// Prepare the expression in the phase one of interpreting
		/// </summary>
		void Prepare();

		/// <summary>
		/// Restrict the expression in the phase two of interpreting
		/// </summary>
		void Restrict();

		/// <summary>
		/// Evaluate the expression in the phase three of interpreting
		/// </summary>
		Value Eval();

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		void Print();
	}

	/// <summary>
	/// All types of concrete expressions
	/// </summary>
	public enum ExprType
	{
		UNKNOWN = 0,

		// Types
		XINTEGER,
		XDECIMAL,
		XDOUBLE,
		XBOOLEAN,
		XFLOAT,
		XSTRING,
		XNODE,
		XCOLLECTION,
		IDENT,
		FUNCTION,
		FUNCTIONIMP,
		ELEMENT,
		ATTRIBUTE,
		LITERAL,
		NULL,
		
		// Arithmetic operations
		PLUS,
		SUB,
		TIMES,
		DIVIDE,
		MOD,
		UMINUS,
		
		// logical expressions
		NOT,
		EQUALS,
		NEQ,
		LT,
		GT,
		LEQ,
		GEQ,
        LIKE,
		AND,
		OR,
		IN,
		NOTIN,
		TRUE,

		// other
		QUERY,
		FLWR,
		FOR,
		LET,
		FL,
		SORTBY,
		SORTBYFIELD,
		IF,
		TO,
		DOCUMENT,
		PATH,
		STEP,
		COLLECTION,
		PARENTHESIZED,
		INLINE,
		DUMMY
	}
}
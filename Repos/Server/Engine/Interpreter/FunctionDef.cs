/*
* @(#)FunctionDef.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a builtin function stored as an entry in the symbol
	/// table.
	/// . Before evaluation, it is associated with parser tree node representing the
	/// expression. After evalutation it is associated with a Value object resulting from
	/// the associated expression.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class FunctionDef : SymbolTableEntry
	{
		/// <summary>
		/// Initiate an instance of FunctionDef object.
		/// </summary>
		/// <param name="name">The name of a variable</param>
		/// <param name="expr">The expression object associated with the symbol</param>
		/// <remarks>The $ sign is not included in the variable name</remarks>
		public FunctionDef(string name, IExpr expr) : base(name, expr)
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
				return false;
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
				return true;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the value of a function is cacheable.
		/// </summary>
		/// <value>true if the value is cacheable, false otherwise</value>
		public bool IsValueCacheable
		{
			get
			{
				return ((IFunctionImp) Expression).IsValueCacheable;
			}
		}

		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public void CheckArgs(ExprCollection arguments)
		{
			((IFunctionImp) Expression).CheckArgs(arguments);		
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("FunctionDef symbol " + this.Name);
		}
	}
}
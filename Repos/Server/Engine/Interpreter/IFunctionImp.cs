/*
* @(#)IFunctionImp.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an interface for function implementation classes
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public interface IFunctionImp
	{
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		void CheckArgs(ExprCollection arguments);

		/// <summary>
		/// Initialize the function implementation instance
		/// </summary>
		/// <param name="interpreter">The interpreter</param>
		/// <param name="name">The function name</param>
		/// <param name="arguments">The invoking arguments</param>
		void Initialize(Interpreter interpreter, string name, ExprCollection arguments);

		/// <summary>
		/// Gets the information indicating whether the value of a function is cacheable.
		/// </summary>
		/// <value>true if the value is cacheable, false otherwise.</value>
		bool IsValueCacheable
		{
			get;
		}
	}
}
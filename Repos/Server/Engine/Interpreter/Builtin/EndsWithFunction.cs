/*
* @(#)EndsWithFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;

	/// <summary> 
	/// verify wether the first given string ends with the second given string. Using a 
	/// XBoolean to indicate the result
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang</author>
	public class EndsWithFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public EndsWithFunction() : base()
		{
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 2)
			{
				throw new InterpreterException("EndsWithFunction expectes two argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			bool result = false;

			// Get the arguments.
			string s1 = ((IExpr) _arguments[0]).Eval().ToString();
			string s2 = ((IExpr) _arguments[1]).Eval().ToString();
			
			result = s1.EndsWith(s2);
			
			return new XBoolean(result);
		}
	}
}
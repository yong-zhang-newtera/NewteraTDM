/*
* @(#)DistinctFunction.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	
	/// <summary> 
	/// Gets a collection of distinct values of the given sequence's item values.
	/// </summary>
	/// <version>  1.0 19 June 2008</version>
	public class DistinctFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public DistinctFunction() : base()
		{
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 1)
			{
				throw new InterpreterException("DistinctFunction expectes one argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
            ValueCollection distinctValues = new ValueCollection();
			ValueCollection values = ((IExpr) _arguments[0]).Eval().ToCollection();

			foreach (Value val in values)
			{
                if (!distinctValues.Contains(val))
                {
                    distinctValues.Add(val);
                }
			}

            return new XCollection(distinctValues);
		}
	}
}
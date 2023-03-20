/*
* @(#)CurrentHourFunction.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Interpreter;

	/// 
	/// <summary>
    /// Gets the number represent the current hour of the system time.
    /// </summary>
	public class CurrentHourFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public CurrentHourFunction() : base()
		{
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 0)
			{
				throw new InterpreterException("CurrentHourFunction expectes zero argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
            int currentHour = DateTime.Now.Hour;

            return new XInteger(currentHour);
		}
	}
}
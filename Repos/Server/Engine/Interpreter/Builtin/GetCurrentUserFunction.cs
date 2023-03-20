/*
* @(#)GetCurrentUserFunction.cs
*
* Copyright (c) 2003-2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Xml;
    using System.Threading;
	using System.Collections;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Principal;
 
	/// <summary>
	/// Gets id of the current loggined user.
	/// </summary>
	/// <version>  1.0.0 23 Feb 2008</version>
	public class GetCurrentUserFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public GetCurrentUserFunction() : base()
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
				throw new InterpreterException("GetCurrentUserFunction expectes no argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
            string userName = "";
            CustomPrincipal principal = null;

            principal = (CustomPrincipal)Thread.CurrentPrincipal;
            if (principal != null)
            {
                //userId = principal.Identity.Name;
                userName = principal.DisplayText;
            }

            return new XString(userName);
		}
	}
}
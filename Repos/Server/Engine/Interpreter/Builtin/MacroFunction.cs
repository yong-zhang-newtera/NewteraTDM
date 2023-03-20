/*
* @(#)MacroFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Collections.Specialized;
	using Newtera.Server.Engine.Interpreter;
	
	/// <summary>
	/// A function representing the macros.
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang</author>
	public class MacroFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public MacroFunction() : base()
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
				throw new InterpreterException("MacroFunction expectes one argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			string macroName = ((IExpr) _arguments[0]).Eval().ToString();
			
			IMacroDefinition macroDef = MacroMapper.Instance.GetMacroDefination(macroName);
			
			object result = macroDef.MacroResult;
			
			if (result is string)
			{
				return new XString((string) result);
			}
			else if (result is StringCollection)
			{
				StringCollection list = (StringCollection) result;
					
				ValueCollection values = new ValueCollection();
				
				foreach (string str in list)
				{
					values.Add(new XString(str));
				}
										
				return new XCollection(values);
			}
			else
			{
				throw new InterpreterException("Invalid macro result. It has to be a string or collection of strings");
			}
		}
	}
}
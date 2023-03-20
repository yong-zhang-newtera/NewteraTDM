/*
* @(#)AddInstanceFunction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin.Db
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;

	/// 
	/// <summary>
	/// Add an data instance represented by an XmlElement to the spcified VDocument.
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang </author>
	public class AddInstanceFunction : EditFunctionBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public AddInstanceFunction()
		{
		}
	
		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			EvalArgs();

			string result = Add();
			
			return new XString(result);
		}
	}
}
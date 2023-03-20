/*
* @(#)IMacroDefinition.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Collections;

	/// <summary>
	/// An interface for macros, mainly used for xacl purpose
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang </author>
	public interface IMacroDefinition
	{
		/// <summary> 
		/// Gets the macro's result.
		/// </summary>
		/// <value> result of macro</returns>
		object MacroResult
		{
			get;
		}

		/// <summary>
		/// Gets the name of macro defination
		/// </summary>
		/// <value> name of macro </value>
		string Name
		{
			get;
		}
	}
}
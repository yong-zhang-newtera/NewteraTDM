/*
* @(#)ITraceable.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an interface for the expressions that can be traced back to owner document,
	/// such as Path, Document, and other XML node related functins
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public interface ITraceable
	{
		/// <summary>
		/// Trace the owner document of the expression.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		Value TraceDocument();

		/// <summary>
		/// Get an enumerator of the absolute path of the traceable object.
		/// </summary>
		/// <returns></returns>
		PathEnumerator GetAbsolutePathEnumerator();
	}
}
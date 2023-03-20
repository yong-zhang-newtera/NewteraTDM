/*
* @(#)ISelectable.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents an interface for the expressions whose nodes can be selected by a
	/// path
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public interface ISelectable
	{
		/// <summary>
		/// Select nodes using a path.
		/// </summary>
		/// <param name="enumerator">The path enumerator</param>
		/// <returns>The XCollection containing a ValueCollection of selected nodes</returns>
		XCollection SelectNodes(PathEnumerator enumerator);
	}
}
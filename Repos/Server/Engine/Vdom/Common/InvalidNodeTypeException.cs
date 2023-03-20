/*
* @(#)InvalidNodeTypeException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Common
{
	using System;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Interpreter;

	/// <summary>
	/// The exception is thrown for invalid node type
	/// </summary>
	public class InvalidNodeTypeException : VDOMException
	{
		/// <summary>
		/// Initializing a InvalidNodeTypeException
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public InvalidNodeTypeException(string reason) : base(reason)
		{
		}
	}
}
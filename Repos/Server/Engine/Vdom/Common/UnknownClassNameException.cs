/*
* @(#)UnknownClassNameException.cs
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
	/// The exception is thrown for unknown class name
	/// </summary>
	public class UnknownClassNameException : VDOMException
	{
		
		/// <summary>
		/// Initiating a UnknownClassNameException.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public UnknownClassNameException(string reason) : base(reason)
		{
		}
	}
}
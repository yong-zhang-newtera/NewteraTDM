/*
* @(#)UnknownJDOMObjectException.cs
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
	/// The exception is thrown for unknown XML object
	/// </summary>
	public class UnknownJDOMObjectException:VDOMException
	{
		
		/// <summary>
		/// Initialize a UnknownJDOMObjectException object
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public UnknownJDOMObjectException(string reason) : base(reason)
		{
		}
	}
}
/*
* @(#)UnknownAttributeNameException.cs
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
	/// The exception is thrown for unknown attribute (simple or relationship) name
	/// </summary>
	public class UnknownAttributeNameException : VDOMException
	{
		
		/// <summary>
		/// Initiating a UnknownAttributeNameException.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public UnknownAttributeNameException(string reason) : base(reason)
		{
		}
	}
}
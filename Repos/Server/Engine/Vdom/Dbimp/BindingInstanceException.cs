/*
* @(#)BindingInstanceException.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// The exception is thrown for deleting an instance that has been bound to some running
    /// workflow instances.
	/// </summary>
	/// <version>  	1.0.0 21 Jul 2007 </version>
	public class BindingInstanceException : VDOMException
	{
		/// <summary>
		/// Initiating an instance of BindingInstanceException class.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public BindingInstanceException(string reason) : base(reason)
		{
		}
	}
}
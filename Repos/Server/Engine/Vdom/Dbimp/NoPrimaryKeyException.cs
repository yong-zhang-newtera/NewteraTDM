/*
* @(#)NoPrimaryKeyException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Interpreter;

	/// <summary>
	///  The exception is thrown for missing primary key(s)in a class.
	/// </summary>
	/// <version>  	1.0.0 02 July 2003 </version>
	/// <author>  		Yong Zhang  </author>
	public class NoPrimaryKeyException:VDOMException
	{
		/// <summary>
		/// Initiating an instance of NoPrimaryKeyException class.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public NoPrimaryKeyException(string reason) : base(reason)
		{
		}
	}
}
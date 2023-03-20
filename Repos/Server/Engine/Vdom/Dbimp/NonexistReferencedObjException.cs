/*
* @(#)NonexistReferencedObjException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Interpreter;

	/// <summary> The exception is thrown for non-exist referenced object.
	/// 
	/// </summary>
	/// <version>  	1.0.0 22 Jul 2003 </version>
	/// <author>  		Yong Zhang  </author>
	public class NonexistReferencedObjException:VDOMException
	{
		/// <summary>
		/// Initiating an instance of NonexistReferencedObjException class.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public NonexistReferencedObjException(string reason) : base(reason)
		{
		}
	}
}
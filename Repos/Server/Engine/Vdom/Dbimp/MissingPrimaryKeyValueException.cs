/*
* @(#)MissingPrimaryKeyValueException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Interpreter;

	/// <summary> The exception is thrown for missing primary key values in an instance element.
	/// 
	/// </summary>
	/// <version>  	1.0.0 22 Jul 2003 </version>
	/// <author>  		Yong Zhang </author>
	public class MissingPrimaryKeyValueException:VDOMException
	{
		/// <summary>
		/// Initiating an instance of MissingPrimaryKeyValueException class.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public MissingPrimaryKeyValueException(string reason) : base(reason)
		{
		}
	}
}
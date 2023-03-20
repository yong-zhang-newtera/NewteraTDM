/*
* @(#)UnsupportedFeatureException.cs
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
	/// The exception is thrown for unsupported features
	/// </summary>
	public class UnsupportedFeatureException : VDOMException
	{
		/// <summary>
		/// Initialize a UnsupportedFeatureException object
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public UnsupportedFeatureException(string reason) : base(reason)
		{
		}
	}
}
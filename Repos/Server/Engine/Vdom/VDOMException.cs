/*
* @(#)VDOMException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom
{
	using System;
	using Newtera.Common.Core;
	using Newtera.Server.Engine.Interpreter;

	/// <summary>
	/// The VDOMException class is the default exception type for vdom 
	/// package. It is highly recommended that a new exception class is defined 
	/// and subclassed from this exception class for each specific error that 
	/// might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 11 Nov 2003
	/// </version>
	/// <author>  		Yong Zhang</author>
    [Serializable]
	public class VDOMException : NewteraException
	{
		/// <summary>
		/// Initiating a VDOMException object
		/// </summary>
		/// <param name="reason">
		/// a description of the error
		/// </param>
		public VDOMException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initiating a VDOMException object
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		/// <param name="ex">the root exception</param>
		public VDOMException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
}
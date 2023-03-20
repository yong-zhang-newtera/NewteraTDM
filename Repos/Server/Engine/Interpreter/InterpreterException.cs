/*
* @(#)InterpreterException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The InterpreterException class is the default exception type for interpreter 
	/// package. It is highly recommended that a new exception class is defined 
	/// and subclassed from this exception class for each specific error that 
	/// might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 11 Aug 2003</version>
    [Serializable]
    public class InterpreterException : NewteraException
	{
		/// <summary>
		/// Initiating a InterpreterException object
		/// </summary>
		/// <param name="reason">
		/// a description of the error
		/// </param>
		public InterpreterException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initiating a InterpreterException object
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		/// <param name="ex">the root exception</param>
		public InterpreterException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
}
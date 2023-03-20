/*
* @(#)ParseTreeException.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;

	/// <summary> 
	/// The ParseTreeException class is the default exception type for ParseTree
	/// package. It is highly recommended that a new exception class 
	/// is defined and subclassed from this exception class for each 
	/// specific error that might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 03 Dec 2005</version>
	/// <author> Yong Zhang </author>
	public class ParseTreeException : Exception
	{
		/// <summary> 
		/// Constructor of a ParseTreeException without an object
		/// </summary>
		/// <param name="reason">a description of the exception </param>
		public ParseTreeException(string reason):base(reason)
		{
		}
		
		/// <summary>
		/// Use this constructor when you wish to wrap an Exception.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">The exception to translate; is stored as
		/// next exception in chain.  Since Throwables have no
		/// chain, ex will be the last exception in the chain.
		/// </param>
		public ParseTreeException(string reason, System.Exception ex) : base(reason, ex)
		{
		}
	}
}
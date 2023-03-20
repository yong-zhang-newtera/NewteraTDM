/*
* @(#)ScriptException.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;

	using Newtera.Common.Core;

	/// <summary> 
	/// The ScriptException class is the default exception type for Scripts
	/// package. It is highly recommended that a new exception class 
	/// is defined and subclassed from this exception class for each 
	/// specific error that might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 23 Sep 2004</version>
	/// <author> Yong Zhang </author>
    public class ScriptException : NewteraException
	{
		/// <summary> Constructor of a ScriptException without an object
		///
		/// </summary>
		/// <param name="reason">a description of the exception 
		/// 
		/// </param>
		public ScriptException(string reason):base(reason)
		{
		}
		
		/// <summary>
		/// Use this constructor when you wish to wrap an Exception.
		/// </summary>
		/// <param name="reason">a description of the exception
		/// </param>
		/// <param name="ex">The exception to translate; is stored as
		/// next exception in chain.  Since Throwables have no
		/// chain, ex will be the last exception in the chain.
		/// </param>
		public ScriptException(string reason, System.Exception ex):base(reason, ex)
		{
		}
	}
}
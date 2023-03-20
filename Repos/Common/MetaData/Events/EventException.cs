/*
* @(#)EventException.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using Newtera.Common.Core;

	/// <summary> 
	/// The EventException class is the default exception type for Events
	/// package. It is highly recommended that a new exception class 
	/// is defined and subclassed from this exception class for each 
	/// specific error that might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 22 Dec 2006</version>
    public class EventException : NewteraException
	{
		/// <summary>
        /// Create an instance of a EventException
		/// </summary>
		/// <param name="reason">a description of the exception 
		/// </param>
		public EventException(string reason):base(reason)
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
		public EventException(string reason, System.Exception ex):base(reason, ex)
		{
		}
	}
}
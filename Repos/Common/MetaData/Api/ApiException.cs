/*
* @(#)ApiException.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using Newtera.Common.Core;

	/// <summary> 
	/// The ApiException class is the default exception type for Apis
	/// package. It is highly recommended that a new exception class 
	/// is defined and subclassed from this exception class for each 
	/// specific error that might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 16 Oct 2015</version>
    public class ApiException : NewteraException
	{
		/// <summary>
        /// Create an instance of a ApiException
		/// </summary>
		/// <param name="reason">a description of the exception 
		/// </param>
		public ApiException(string reason):base(reason)
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
		public ApiException(string reason, System.Exception ex):base(reason, ex)
		{
		}
	}
}
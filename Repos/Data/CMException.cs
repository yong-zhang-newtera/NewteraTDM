/*
* @(#) CMException.cs	1.1.0		2001-11-19
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// A common exception type for all exceptions thrown from CM data provider implementation.
	/// It is highly recommended that a new exception class is defined 
	/// and subclassed from this exception class for each specific error that 
	/// might occur in program of this module.
	/// 
	/// An exception instance include an error description and the causing exception.
	/// </summary>
	/// <version> 	1.1.0	08 May 2003</version>
	/// <author> 	Yong Zhang </author>
	public class CMException: NewteraException
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="desc">a description of the exception.</param>
		public CMException(string desc) : base(desc)
		{
		}
		
		/// <summary>
		/// Use this constructor for situation of translating an Exception.
		/// </summary>
		/// <param name="desc">a description of the exception</param>
		/// <param name="ex">the exception that got translated and chained.</param>
		public CMException(string desc, Exception ex) : base(desc, ex)
		{
		}
	}

	/// <summary>
	/// The exception thrown when there is a problem in a schema
	/// </summary>
	/// <version> 	1.1.0	08 Aug 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class InvalidSchemaException : CMException
	{
		/// <summary>
		/// Use this constructor for situation of translating an Exception
		/// </summary>
		/// <param name="ex">the exception that got translated and chained
		/// </param>
		/// <param name="desc">a description of the exception
		/// 
		/// </param>
		public InvalidSchemaException(string desc, Exception ex) : base(desc, ex)
		{
		}
	}

	/// <summary>
	/// The exception Thrown if missing some critical key/value pairs in the connection string.
	/// </summary>
	/// <version> 	1.1.0	08 Aug 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class InvalidConnectionStringException : CMException
	{
		/// <summary>
		/// initiating a new instance of InvalidConnectionStringException
		/// </summary>
		/// <param name="desc">a description of the exception</param>
		public InvalidConnectionStringException(string desc) : base(desc)
		{
		}
	}
}
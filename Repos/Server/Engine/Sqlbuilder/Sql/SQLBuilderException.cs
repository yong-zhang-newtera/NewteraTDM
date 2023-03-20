/*
* @(#)SQLBuilderException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The SQLBuilderException class is the default exception type for sql 
	/// package. It is highly recommended that a new exception class is defined 
	/// and subclassed from this exception class for each specific error that 
	/// might occur in program of this module.
	/// </summary>
	/// <version>  	1.0.0 14 Jul 2003 </version>
    [Serializable]
    public class SQLBuilderException : NewteraException
	{
		/// <summary>
		/// Initializing a SQLBuilderException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public SQLBuilderException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a SQLBuilderException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		///  
		public SQLBuilderException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
	
	/// <summary>
	/// The DateTimeFormatterException class for date/time related errors
	/// </summary>
	class DateTimeFormatterException:SQLBuilderException
	{
		/// <summary>
		/// Initializing a DateTimeFormatterException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public DateTimeFormatterException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a DateTimeFormatterException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		///  
		public DateTimeFormatterException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
}
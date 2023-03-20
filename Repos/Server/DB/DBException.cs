/*
* @(#)DatabaseConfigException.cs	1.0.0		2003-01-16
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using Newtera.Common.Core;
	
	/// <summary>
	/// Common exception class for db related exception
	/// </summary>
	/// <version>  1.0.0	24 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
    public class DBException : NewteraException
	{
		/// <summary>
		/// Initializing a DBException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public DBException(string reason):base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a DBException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		///  
		public DBException(string reason, Exception ex):base(reason, ex)
		{
		}
	}
	
	/// <summary>
	/// Exception is thrown when failed in generating an identifier.
	/// </summary>
	internal class GeneratIDFailException : DBException
	{
		/// <summary>
		/// Initializing a DBException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		///  
		public GeneratIDFailException(string reason, Exception ex):base(reason, ex)
		{
		}
	}

	/// <summary>
	/// Exception is thrown when failed to find a canned sql.
	/// </summary>	
	internal class SQLNotFoundException : DBException
	{
		/// <summary>
		/// Initializing a SQLNotFoundException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public SQLNotFoundException(string reason) : base(reason)
		{
		}
	}
	
	/// <summary>
	/// Thrown if format of a SQL configuration file is incorrect.
	/// </summary>
	internal class InvalidConfigurationException : DBException
	{
		/// <summary>
		/// Initializing a InvalidConfigurationException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public InvalidConfigurationException(string reason):base(reason)
		{
		}
	}
	
	/// <summary>
	/// Thrown when there are not enough parameter to replace
	/// varaible in a SQL string.
	/// </summary>
	class MissingParameterException : DBException
	{
		/// <summary>
		/// Initializing a MissingParameterException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public MissingParameterException(string reason) : base(reason)
		{
		}
	}
}
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
	/// Exception is thrown when a data source name gives an invalid connection.
	/// </summary>
	public class InvalidDataSourceException : DBException
	{
		/// <summary>
		/// Initiating an instance of InvalidDataSourceException class.
		/// </summary>
		/// <param name="reason">exception description</param>
		public InvalidDataSourceException(string reason) : base(reason)
		{
		}
	}
}
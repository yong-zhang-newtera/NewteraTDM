/*
* @(#)MetaDataUpdateException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The exception is thrown for error during the update of meta data model.
	/// </summary>
	/// <version>  	1.0.0 19 Oct 2003</version>
	/// <author> Yong Zhang </author>
    public class MetaDataUpdateException : NewteraException
	{
		/// <summary>
		/// Initiating an instance of MetaDataUpdateException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public MetaDataUpdateException(string reason) : base(reason)
		{
		}
		
		/// <summary>
		/// Initiating an instance of MetaDataUpdateException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">An inner exception</param>
		public MetaDataUpdateException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}
}
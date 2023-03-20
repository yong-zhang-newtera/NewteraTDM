/*
* @(#)LockMetaDataException.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The exception is thrown for failing to lock a meta-data model.
	/// </summary>
	/// <version>  	1.0.0 02 Oct 2006</version>
    public class LockMetaDataException : NewteraException
	{
		
		/// <summary>
		/// Initiating an instance of LockMetaDataException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public LockMetaDataException(string reason) : base(reason)
		{
		}
	}
}
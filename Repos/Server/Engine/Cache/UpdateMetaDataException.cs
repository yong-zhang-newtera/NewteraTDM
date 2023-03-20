/*
* @(#)UpdateMetaDataException.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The exception is thrown for failing to update a meta-data model.
	/// </summary>
	/// <version>  	1.0.0 15 Jul 2007</version>
    public class UpdateMetaDataException : NewteraException
	{
		/// <summary>
		/// Initiating an instance of UpdateMetaDataException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public UpdateMetaDataException(string reason) : base(reason)
		{
		}
	}
}
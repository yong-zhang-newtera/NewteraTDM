/*
* @(#)UnknownSchemaException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The exception is thrown for an unknow schema.
	/// </summary>
	/// <version>  	1.0.0 19 Oct 2003</version>
	/// <author> Yong Zhang </author>
    public class UnknownSchemaException : NewteraException
	{
		
		/// <summary>
		/// Initiating an instance of UnknownSchemaException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public UnknownSchemaException(string reason) : base(reason)
		{
		}
	}
}
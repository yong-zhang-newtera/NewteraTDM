/*
* @(#)NoDeletePermissionException.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The exception is thrown for an unknow schema.
	/// </summary>
	/// <version>  	1.0.0 02 Jan 2005</version>
	/// <author> Yong Zhang </author>
    public class NoDeletePermissionException : NewteraException
	{
		
		/// <summary>
		/// Initiating an instance of NoDeletePermissionException class.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public NoDeletePermissionException(string reason) : base(reason)
		{
		}
	}
}
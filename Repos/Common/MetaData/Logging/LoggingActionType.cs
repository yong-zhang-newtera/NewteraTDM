/*
* @(#)LoggingActionType.cs 
*
* Copyright (c) 2009 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	
	/// <summary>
	/// Specify the possible action types
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009 </version>
	[Flags]
	public enum LoggingActionType : short
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Read
		/// </summary>
		Read = 1,
		/// <summary>
		/// Write
		/// </summary>
		Write = 2,
		/// <summary>
		/// Create
		/// </summary>
		Create = 4,
		/// <summary>
		/// Delete
		/// </summary>
		Delete = 8,
		/// <summary>
		/// Upload
		/// </summary>
		Upload = 16,
		/// <summary>
		/// Download
		/// </summary>
		Download = 32,
        /// <summary>
        /// Import
        /// </summary>
        Import = 64,
        /// <summary>
        /// Export
        /// </summary>
        Export = 128,
        /// <summary>
        /// Login for an user log in a system, treated specially
        /// </summary>
        Login = 256,
        /// <summary>
        /// Logout for an user log out a system, treated specially
        /// </summary>
        Logout = 512
	}
}
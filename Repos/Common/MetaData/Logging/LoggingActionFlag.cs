/*
* @(#)LoggingActionFlag.cs 
*
* Copyright (c) 2003 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	
	/// <summary>
	/// Specify the flags used to indicate permissions to an object
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009</version>
	[Flags]
	public enum LoggingActionFlag : byte
	{
		/// <summary>
		/// LogRead
		/// </summary>
		LogRead = 1,
		/// <summary>
		/// LogWrite
		/// </summary>
		LogWrite = 2,
		/// <summary>
		/// LogCreate
		/// </summary>
		LogCreate = 4,
		/// <summary>
		/// LogDelete
		/// </summary>
		LogDelete = 8,
		/// <summary>
		/// LogUpload
		/// </summary>
		LogUpload = 16,
		/// <summary>
		/// LogDownload
		/// </summary>
		LogDownload = 32,
        /// <summary>
        /// LogImport
        /// </summary>
        LogImport = 64,
        /// <summary>
        /// LogExport
        /// </summary>
        LogExport = 128,
	}
}
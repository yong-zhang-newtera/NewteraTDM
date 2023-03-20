/*
* @(#)XaclPermissionFlag.cs 
*
* Copyright (c) 2003 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	
	/// <summary>
	/// Specify the flags used to indicate permissions to an object
	/// </summary>
	/// <version> 1.0.0 04 Jan 2004</version>
	/// <author> Yong Zhang </author>
	[Flags]
	public enum XaclPermissionFlag : byte
	{
		/// <summary>
		/// GrantRead
		/// </summary>
		GrantRead = 1,
		/// <summary>
		/// GrantWrite
		/// </summary>
		GrantWrite = 2,
		/// <summary>
		/// GrantCreate
		/// </summary>
		GrantCreate = 4,
		/// <summary>
		/// GrantDelete
		/// </summary>
		GrantDelete = 8,
		/// <summary>
		/// GrantUpload
		/// </summary>
		GrantUpload = 16,
		/// <summary>
		/// GrantDownload
		/// </summary>
		GrantDownload = 32
	}
}
/*
* @(#)XaclActionType.cs 
*
* Copyright (c) 2003 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	
	/// <summary>
	/// Specify the possible action types
	/// </summary>
	/// <version> 1.0.0 11 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	[Flags]
	public enum XaclActionType : byte
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
		Download = 32
	}
}
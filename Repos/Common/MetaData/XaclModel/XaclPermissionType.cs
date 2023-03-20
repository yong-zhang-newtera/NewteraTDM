/*
* @(#)XaclPermissionType.cs 
*
* Copyright (c) 2003 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	
	/// <summary>
	/// Specify the possible type of permissions
	/// </summary>
	/// <version> 1.0.0 10 Dec 2003</version>
	/// <author> Yong Zhang </author>
	public enum XaclPermissionType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// Grant access
		/// </summary>
		Grant,
		/// <summary>
		/// Deny access
		/// </summary>
		Deny,
		/// <summary>
		/// Conditionally Grant access
		/// </summary>
		ConditionalGrant,
		/// <summary>
		/// Conditionally Deny access
		/// </summary>
		ConditionalDeny
	}
}
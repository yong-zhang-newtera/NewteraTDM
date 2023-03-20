/*
* @(#)XaclConflictResolutionType.cs 
*
* Copyright (c) 2003 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	
	/// <summary>
	/// Specify the options of conflict resolution
	/// </summary>
	/// <version> 1.0.0 11 Dec 2003</version>
	/// <author> Yong Zhang </author>
	public enum XaclConflictResolutionType
	{
		/// <summary>
		/// Deny take precedence
		/// </summary>
		Dtp,
		/// <summary>
		/// Grant take precedence
		/// </summary>
		Gtp,
		/// <summary>
		/// Nothing tale precedence
		/// </summary>
		Ntp
	}
}
/*
* @(#)LoggingConflictResolutionType.cs 
*
* Copyright (c) 2009 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	
	/// <summary>
	/// Specify the options of conflict resolution
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009</version>
	public enum LoggingConflictResolutionType
	{
		/// <summary>
		/// Off take precedence
		/// </summary>
		Offtp,
		/// <summary>
		/// On take precedence
		/// </summary>
		Ontp,
		/// <summary>
		/// Nothing take precedence
		/// </summary>
		Ntp
	}
}
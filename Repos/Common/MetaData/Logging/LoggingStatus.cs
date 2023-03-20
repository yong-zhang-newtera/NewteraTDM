/*
* @(#)LoggingStatus.cs 
*
* Copyright (c) 2009 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	
	/// <summary>
	/// Specify the status of logging
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009</version>
	public enum LoggingStatus
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// Logging on
		/// </summary>
		On,
		/// <summary>
		/// Logging off
		/// </summary>
		Off
	}
}
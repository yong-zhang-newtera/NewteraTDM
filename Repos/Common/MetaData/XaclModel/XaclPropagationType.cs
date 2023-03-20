/*
* @(#)XaclPropagationType.cs 
*
* Copyright (c) 2003 by Newtera, Inc. All Rights Reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Specify the possible options of PropagationType enum
	/// </summary>
	/// <version> 1.0.0 11 Dec 2003 </version>
	/// <author>  Yong Zhang </author>
	public enum XaclPropagationType
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Downward
		/// </summary>
		Downward
	}
}
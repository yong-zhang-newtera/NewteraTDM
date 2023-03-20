/*
* @(#)IEnumerableActivity.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Activities
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the activities that can be set to a current item when used
    /// as a child activity ofthe ForEachActivity
	/// </summary>
	/// <version> 1.0.0 10 Jan 2008</version>
    /// <remarks>Deprecated in 4.0.0</remarks>
	public interface IEnumerableActivity
	{
        /// <summary>
        /// Gets or sets the current item of a ForEachActivity in which the activity is the child.
        /// </summary>
        object CurrentItem {get; set;}
	}
}
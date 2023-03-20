/*
* @(#)IXaclObject.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the objects to be managed with access control
	/// components.
	/// </summary>
	/// <version>  	1.0.0 26 Jul 2003
	/// </version>
	/// <author>  Yong Zhang </author>
	public interface IXaclObject
	{
		/// <summary>
		/// Return a xpath representation of the object
		/// </summary>
		/// <returns>a xapth representation</returns>
		string ToXPath();

		/// <summary>
		/// Return a  parent of the object
		/// </summary>
		/// <returns>The parent of the object</returns>
		IXaclObject Parent
		{
			get;
		}

		/// <summary>
		/// Return a  of children of the object
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
		IEnumerator GetChildren();
	}
}
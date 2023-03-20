/*
* @(#)IGridItemProxy.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Xml;
	using System.Windows.Forms;


	/// <summary>
	/// An interface implemented by the map components that serves as proxy
	/// for an GridItem in PropertyGrid
	/// </summary>
	/// <version>1.0.0 10 Nov 2004</version>
	/// <author>Yong Zhang </author>
	public interface IGridItemProxy
	{
		/// <summary>
		/// Gets or sets the GridItem associated with the componnet
		/// </summary>
		/// <value>GridItem object</value>
		GridItem GridItem
		{
			get;
			set;
		}
	}
}
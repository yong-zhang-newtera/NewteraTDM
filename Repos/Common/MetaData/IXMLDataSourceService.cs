/*
* @(#)IXMLDataSourceService.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
    using System.Xml;

	using Newtera.Common.Core;

	/// <summary>
	/// Represents an interface for accessing a XML data source. This interface will have different implementation on
	/// the window client side and server side.
	/// </summary>
	/// <version>  	1.0.0 15 Nov. 2009</version>
	public interface IXMLDataSourceService
	{
		/// <summary>
		/// Execute an xquery to get result
		/// </summary>
		/// <param name="query">A xquery</param>
		/// <returns>A XmlDocument</returns>
		XmlDocument Execute(string query);
	}
}
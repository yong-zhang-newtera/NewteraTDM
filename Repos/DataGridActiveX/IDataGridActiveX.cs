/*
* @(#)IDataGridActiveX.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a COM interface for an ActiveX control.
	/// </summary>
	/// <version>  	1.0.0 12 April 2006</version>
	/// <author>  Yong Zhang </author>
	public interface IDataGridActiveX
	{
		/// <summary>
		/// Gets or sets the type of view to be associated with the DataGrid control
		/// the values are Class, Taxon, or Array
		/// </summary>
		string ViewType {get; set;}

		/// <summary>
		/// Gets or sets the Caption of the class that DataGrid is linked with
		/// </summary>
		string ClassCaption {get; set;}

		/// <summary>
		/// Gets or sets the name of the class that DataGrid is linked with
		/// </summary>
		string ClassName {get; set;}

		/// <summary>
		/// Gets or sets the name of taxonomy tree
		/// </summary>
		string TaxonomyName {get; set;}

		/// <summary>
		/// Gets or sets the taxon node name
		/// </summary>
		string TaxonName {get; set;}

		/// <summary>
		/// Gets or sets the connection string that is used to connect the server
		/// </summary>
		string ConnectionString {get; set;}

		/// <summary>
		/// Gets or sets the instance count of result
		/// </summary>
		int TotalCount {get; set;}

		/// <summary>
		/// Gets or sets the XQuery for searching the results
		/// </summary>
		string XQuery {get; set;}
	}
}
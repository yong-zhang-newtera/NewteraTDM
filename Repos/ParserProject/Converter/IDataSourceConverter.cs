/*
* @(#)IDataSourceConverter.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.Converter
{
	using System;
	using System.Data;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for all data source parser implementations.
	/// </summary>
	/// <version> 1.0.0 07 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public interface IDataSourceConverter
	{
		/// <summary>
		/// Gets or sets the name of converter
		/// </summary>
		/// <value>A string representing an unique name of the converter.</value>
		string Name {get; set;}

		/// <summary>
		/// Gets the information indicating whether the converter supports reading data in
		/// pages
		/// </summary>
		/// <value>true if it supports paging, false otherwise.</value>
		bool SupportPaging {get;}

		/// <summary>
		/// Convert data of a data source into a DataSet instance.
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies a data source</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		DataSet Convert(string dataSourceName);

		/// <summary>
		/// Used in converting data in a large file, convert the first page of data into a DataSet instance.
		/// To be called first.
		/// </summary>
		/// <param name="dataSourceName">The name of the data source file.</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data, or NULL it reaches the end of file.</returns>
		DataSet ConvertFirstPage(string dataSourceName, int pageSize);

		/// <summary>
		/// Used in converting data in a large file, convert data of a next page
		/// to a DataSet instance. To be called subsequently after ConvertFirstPage is called.
		/// </summary>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data, or NULL it reaches the end of file.</returns>
		/// <exception cref="Exception">Thrown if the ConvertFirstPage isn't called.</exception>
		DataSet ConvertNextPage();

		/// <summary>
		/// Used in converting data in a large file, close the file opened at ConvertFirstPage
		/// method call.
		/// </summary>
		void Close();
	}
}
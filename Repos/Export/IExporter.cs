/*
* @(#)IExporter.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Export
{
	using System;
	using System.Xml;
	using System.Data;

	/// <summary>
	/// Represents a common interface for all data exporter implementations.
	/// </summary>
	/// <version> 1.0.0 01 June 2006</version>
	public interface IExporter
	{
		/// <summary>
		/// Called at the beginning of the exporting data, allow the exporter to
		/// perform the initialization necessay for exporting process, such as
		/// open the file to write data.
		/// </summary>
		void BeginExport(string filePath);

		/// <summary>
		/// Export the data in the TableTable to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for exporting</param>
		void ExportData(DataTable dataTable);

        /// <summary>
        /// Export the data in the Xml to a file.
        /// </summary>
        /// <param name="xmlString">The xmlstring for exporting</param>
        void ExportXml(string xmlString);

		/// <summary>
		/// Called at the end of the exporting data, allow the exporter to
		/// free up the resources used by the exporter, such as closing the file
		/// </summary>
		void EndExport();
	}
}
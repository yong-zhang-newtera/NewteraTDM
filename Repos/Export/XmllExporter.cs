/*
* @(#)XmlExporter.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Export
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Data;

	/// <summary>
	/// Export data in DataTable into a xml file
	/// </summary>
	/// <version> 1.0.0 28 Jul 2009</version>
	public class XmlExporter : IExporter
	{
		private StreamWriter _writer;

		/// <summary>
		/// Initialize the exporter
		/// </summary>
		public XmlExporter()
		{
			_writer = null;
		}

		/// <summary>
		/// Called at the beginning of the exporting data, allow the exporter to
		/// perform the initialization necessay for exporting process, such as
		/// open the file to write data.
		/// </summary>
		public void BeginExport(string filePath)
		{
            // use UTF8 encoding to avoid problem of exporting Chinses
            _writer = new StreamWriter(filePath, false, Encoding.UTF8);
		}

		/// <summary>
		/// Export the data in the TableTable to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for exporting</param>
		public void ExportData(DataTable dataTable)
		{
		}

		/// <summary>
		/// Called at the end of the exporting data, allow the exporter to
		/// free up the resources used by the exporter, such as closing the file
		/// </summary>
		public void EndExport()
		{
			if (_writer != null)
			{
				_writer.Close(); // close the file
			}
		}

        /// <summary>
        /// Export the data in the Xml to a file.
        /// </summary>
        /// <param name="xmlString">The xmlstring for exporting</param>
        public void ExportXml(string xmlString)
        {
            _writer.WriteLine(xmlString);
        }
	}
}
/*
* @(#)OldExcelExporter.cs
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
	/// Export data in DataTable into an Excel file
	/// </summary>
	/// <version> 1.0.0 01 June 2006</version>
	public class OldExcelExporter : IExporter
	{
		private StreamWriter _writer;
		private bool _isFirstCall = true;
		private bool _showExcelTableBorder = true;
		private bool _hasHeaders = false;

		/// <summary>
		/// Initialize the exporter
		/// </summary>
		public OldExcelExporter()
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
			_isFirstCall = true;

			//_writer = File.CreateText(filePath);

            // use UTF8 encoding to avoid problem of exporting Chinses
            _writer = new StreamWriter(filePath, false, Encoding.UTF8);
		}

		/// <summary>
		/// Export the data in the TableTable to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for exporting</param>
		public void ExportData(DataTable dataTable)
		{
			StringBuilder builder = new StringBuilder();

			if (_isFirstCall)
			{
				// export the headers
				ExportHeaders(builder, dataTable);
				_isFirstCall = false;
				if (builder.Length > 0)
				{
					_hasHeaders = true;
				}
			}

			// Creating rows
			foreach(DataRow dataRow in dataTable.Rows)	
			{
				builder.Append("<TR>");

				foreach(DataColumn dataColumn in dataTable.Columns)	
				{
					if (dataRow[dataColumn.ColumnName] is long || 
						dataRow[dataColumn.ColumnName] is float ||
						dataRow[dataColumn.ColumnName] is int)	
					{
						builder.Append("<TD align=right>" + dataRow[dataColumn.ColumnName] + "</TD>");
					}
					else	
					{
						builder.Append("<TD>" + dataRow[dataColumn.ColumnName] + "</TD>");
					}
				}

				builder.Append("</TR>");
			}

			// write to the file
			_writer.Write(builder.ToString());
		}

		/// <summary>
		/// Called at the end of the exporting data, allow the exporter to
		/// free up the resources used by the exporter, such as closing the file
		/// </summary>
		public void EndExport()
		{
			if (_hasHeaders)
			{
				_writer.WriteLine("</Table>"); // end of excel data
			}

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
        }

		/// <summary>
		/// Export Excel headers
		/// </summary>
		/// <param name="builder">The StringBuilder</param>
		/// <param name="dataTable">The DataTable</param>
		private void ExportHeaders(StringBuilder builder, DataTable dataTable)
		{
			 // Start, check for border width
			int borderWidth = 0;

			if (_showExcelTableBorder)	
			{
				borderWidth = 1;
			}

			builder.Append("<Table border=" + borderWidth + ">");

			// Creating table header
			builder.Append("<TR>");

			foreach(DataColumn dataColumn in dataTable.Columns)	
			{
				builder.Append("<TD>" + dataColumn.Caption + "</TD>");
			}

			builder.Append("</TR>");
		}
	}
}
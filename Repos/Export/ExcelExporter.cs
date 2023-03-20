/*
* @(#)ExcelExporter.cs
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
	/// <version> 1.0.0 01 Nov 2013</version>
	public class ExcelExporter : IExporter
	{
		private StreamWriter _writer;
		private bool _isFirstCall = true;
		private bool _showExcelTableBorder = true;

		/// <summary>
		/// Initialize the exporter
		/// </summary>
		public ExcelExporter()
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

            // use gb2312 encoding to avoid problem of exporting Chinses
            _writer = new StreamWriter(filePath, false, System.Text.Encoding.GetEncoding("gb2312"));
		}

		/// <summary>
		/// Export the data in the TableTable to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for exporting</param>
		public void ExportData(DataTable dataTable)
		{
			if (_isFirstCall)
			{
				// export the headers
				ExportHeaders(dataTable);
				_isFirstCall = false;
			}

			// Creating rows
			foreach(DataRow dataRow in dataTable.Rows)	
			{
                StringBuilder builder = new StringBuilder();

				foreach(DataColumn dataColumn in dataTable.Columns)	
				{
                    if (builder.Length > 0)
                    {
                        builder.Append("\t");
                    }

		            builder.Append(dataRow[dataColumn.ColumnName].ToString());
				}

                // write to the file
                _writer.WriteLine(builder.ToString());
			}
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
        }

		/// <summary>
		/// Export Excel headers
		/// </summary>
		/// <param name="dataTable">The DataTable</param>
		private void ExportHeaders(DataTable dataTable)
		{
            StringBuilder builder = new StringBuilder();

			foreach(DataColumn dataColumn in dataTable.Columns)	
			{
                if (builder.Length > 0)
                {
                    builder.Append("\t");
                }
				builder.Append(dataColumn.Caption);
			}

            _writer.WriteLine(builder.ToString());
		}
	}
}
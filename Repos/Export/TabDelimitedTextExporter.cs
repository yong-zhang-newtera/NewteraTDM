/*
* @(#)TabDelimitedTextExporter.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
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
	/// Export data in DataTable into a Tab-delimited text file
	/// </summary>
	/// <version> 1.0.0 01 June 2006</version>
	public class TabDelimitedTextExporter : IExporter
	{
		private StreamWriter _writer;
		private bool _exportHeader = true;
        private string _tableName;

		/// <summary>
		/// Initialize the exporter
		/// </summary>
		public TabDelimitedTextExporter()
		{
			_writer = null;
            _tableName = null;
		}

		/// <summary>
		/// Called at the beginning of the exporting data, allow the exporter to
		/// perform the initialization necessay for exporting process, such as
		/// open the file to write data.
		/// </summary>
		public void BeginExport(string filePath)
		{
			_exportHeader = true;
			_writer = new StreamWriter(filePath, false, System.Text.Encoding.Default);
		}

		/// <summary>
		/// Export the data in the TableTable to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for exporting</param>
		public void ExportData(DataTable dataTable)
		{
			StringBuilder builder = new StringBuilder();

            if (_tableName == null || _tableName != dataTable.TableName)
            {
                if (!string.IsNullOrEmpty(_tableName))
                {
                    // write a separator before append a datatable
                    ExportSeparator();
                }

                _exportHeader = true;
                _tableName = dataTable.TableName;
            }

			if (_exportHeader)
			{
				// export the headers
				ExportHeaders(builder, dataTable);
				_exportHeader = false;
			}

			foreach(DataRow dataRow in dataTable.Rows)	
			{
				bool isFirstColumn = true;
				foreach(DataColumn dataColumn in dataTable.Columns)	
				{
					if (!isFirstColumn)
					{
						builder.Append("\t");
					}
					else
					{
						isFirstColumn = false;
					}

					builder.Append(dataRow[dataColumn.ColumnName]);
				}

				_writer.WriteLine(builder.ToString());
				builder.Length = 0;
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
		/// Export column headers
		/// </summary>
		/// <param name="builder">The StringBuilder</param>
		/// <param name="dataTable">The DataTable</param>
		private void ExportHeaders(StringBuilder builder, DataTable dataTable)
		{
			bool isFirstColumn = true;

			foreach(DataColumn dataColumn in dataTable.Columns)	
			{
				if (!isFirstColumn)
				{
					builder.Append("\t");
				}
				else
				{
					isFirstColumn = false;
				}
				builder.Append(dataColumn.Caption);
			}

			_writer.WriteLine(builder.ToString());
			builder.Length = 0;
		}

        /// <summary>
        /// Export an empty line to the file
        /// </summary>
        private void ExportSeparator()
        {
            _writer.WriteLine();
        }
	}
}
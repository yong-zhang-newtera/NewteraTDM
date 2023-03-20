/*
* @(#)DelimitedTextFileConverter.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Conveters
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Data;
	using System.Text;
	using System.Text.RegularExpressions;

	using Newtera.ParserGen.Converter;

	/// <summary> 
	/// The class implements a converter for delimited text file formats.
	/// </summary>
	/// <version> 1.0.0 07 Sep 2004</version>
	public class DelimitedTextFileConverter : DataSourceConverterBase
	{
		private string _rowDelimiter;
		private string _colDelimiter = "\t";
		private bool _isFirstRowColumns = false;
		private StringBuilder _rowDataBuffer = null;
		private DataSet _dataSet = null;
		private StreamReader _reader = null;
		private int _pageSize = 100; // default
		private string[] _srcRows = null;
		private int _srcRowIndex = 0;
        private int _startingDataRow = 1;

		/// <summary>
		/// Initiate an instance of DelimitedTextFileConverter class
		/// </summary>
		public DelimitedTextFileConverter() : base()
		{
		}

		/// <summary>
		/// Initiate an instance of DelimitedTextFileConverter class
		/// </summary>
		/// <param name="name">The parser name</param>
		public DelimitedTextFileConverter(string name) : base(name)
		{
		}

		/// <summary>
		/// Gets or sets the row delimiter
		/// </summary>
		/// <value>A string representing row delimiter.</value>
		public string RowDelimiter
		{
			get
			{
				return _rowDelimiter;
			}
			set
			{
				_rowDelimiter = value;
			}
		}

		/// <summary>
		/// Gets or sets the column delimiter
		/// </summary>
		/// <value>A string representing column delimiter.</value>
		public string ColumnDelimiter
		{
			get
			{
				return _colDelimiter;
			}
			set
			{
				_colDelimiter = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating if the first row representing
		/// column names
		/// </summary>
		/// <value>true if the first row represents the column names, false otherwise.</value>
		public bool IsFirstRowColumns
		{
			get
			{
				return _isFirstRowColumns;
			}
			set
			{
				_isFirstRowColumns = value;
			}
		}

        /// <summary>
        /// Gets or sets the starting data row.
        /// </summary>
        /// <value>A one-based integer representing row of the first data row in a file.</value>
        public int StartingDataRow
        {
            get
            {
                return _startingDataRow;
            }
            set
            {
                _startingDataRow = value;
            }
        }

		/// <summary>
		/// Gets the information indicating whether the converter supports reading data in
		/// pages
		/// </summary>
		/// <value>true if it supports paging, false otherwise. Default is false.</value>
		public override bool SupportPaging
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Overriding the method to implement operation of parsing a text data separated
		/// with delimiters. 
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies a data source</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		public override DataSet Convert(string dataSourceName)
		{
            string fileName = GetFileName(dataSourceName);

            // The DataSet to return
            DataSet dataSet = new DataSet(fileName);

			DataTable dataTable = dataSet.Tables.Add(fileName);

			this._rowDataBuffer = null;

			// open the file as stream reader
			using (StreamReader reader = new StreamReader(dataSourceName, Encoding.Default))
			{
				// add columns to the DataTable
				AddColumns(reader, dataTable);

				if (RowDelimiter != null && ColumnDelimiter != null && ColumnDelimiter.Length > 0)
				{
					string[] rows = GetAllRows(reader); // get all rows
		
					if (rows != null)
					{
						AddRows(dataTable, rows, GetSkippedRows());
					}
				}
			}

			return dataSet;
		}

		/// <summary>
		/// Override the method to convert the first page of data into a DataSet instance.
		/// To be called first.
		/// </summary>
		/// <param name="dataSourceName">The name of the data source file.</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data, or NULL it reaches the end of file.</returns>
		public override DataSet ConvertFirstPage(string dataSourceName, int pageSize)
		{
            string fileName = GetFileName(dataSourceName);

            // The DataSet to return
            _dataSet = new DataSet(fileName);

			DataTable dataTable = _dataSet.Tables.Add(fileName);

			_rowDataBuffer = null;
			_pageSize = pageSize;

			// open the file as stream reader
			_reader = new StreamReader(dataSourceName, Encoding.Default);

			// add columns to the DataTable
			AddColumns(_reader, dataTable);

			if (RowDelimiter != null && ColumnDelimiter != null && ColumnDelimiter.Length > 0)
			{
				string[] rows = GetPagedRows(_reader);
		
				if (rows != null)
				{
					AddRows(dataTable, rows, GetSkippedRows());
				}
				else
				{
					_dataSet = null;
				}
			}

			return _dataSet;
		}

		/// <summary>
		/// Override the method to convert data of a next page
		/// to a DataSet instance. To be called subsequently after ConvertFirstPage is called.
		/// </summary>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data, or NULL it reaches the end of file.</returns>
		/// <exception cref="Exception">Thrown if the ConvertFirstPage isn't called.</exception>
		public override DataSet ConvertNextPage()
		{
			if (_reader == null)
			{
				throw new Exception("Please call ConvertFirstPage method before calling ConvertNextPage.");
			}

			// Reuse the dataset
			DataTable dataTable = _dataSet.Tables[0];
			dataTable.Rows.Clear(); // clear the previous page

			if (RowDelimiter != null && ColumnDelimiter != null && ColumnDelimiter.Length > 0)
			{
				string[] rows = GetPagedRows(_reader);
		
				if (rows != null)
				{
					AddRows(dataTable, rows);
				}
				else
				{
					_dataSet = null;
				}
			}

			return _dataSet;
		}

		/// <summary>
		/// Override the method to close the file opened at ConvertFirstPage
		/// method call.
		/// </summary>
		public override void Close()
		{
			this._dataSet = null;

			this._srcRows = null;

			this._rowDataBuffer = null;
			
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}
		}

		/// <summary>
		/// Add columns to the DataTable
		/// </summary>
		/// <param name="reader">The reader to read column data</param>
		/// <param name="dataTable">The DataTable</param>
		private void AddColumns(StreamReader reader, DataTable dataTable)
		{
			if (IsFirstRowColumns && ColumnDelimiter != null && ColumnDelimiter.Length > 0)
			{
				//Split the first line into the columns 
				string firstLine = reader.ReadLine();
				
				if (ColumnDelimiter[0] == ' ')
				{
					// remove extra spaces between column names
					firstLine = StripSpaces(firstLine);
				}

				string[] columns = firstLine.Split(ColumnDelimiter.ToCharArray());
	    
				//Cycle the colums, adding those that don't exist yet 
				//and sequencing the one that do.
				foreach (string col in columns)
				{
					bool added = false;
					string next = "";
					int i = 0;
					while (!added)        
					{
						//Build the column name and remove any unwanted characters.
						string columnName = col + next;
						columnName = columnName.Replace("#", "");
						columnName = columnName.Replace("'", "");
						columnName = columnName.Replace("&", "");
	        
						//See if the column already exists
						if(!dataTable.Columns.Contains(columnName))
						{
							//if it doesn't then we add it here and mark it as added
							dataTable.Columns.Add(columnName);
							added = true;
						}
						else
						{
							//if it did exist then we increment the sequencer and try again.
							i++;  
							next = "_" + i.ToString();
						}         
					}
				}
			}
		}

        /// <summary>
		/// Add rows to the DataTable
		/// </summary>
		/// <param name="dataTable">The data table</param>
		/// <param name="rows">The data rows</param>
        private void AddRows(DataTable dataTable, string[] rows)
        {
            AddRows(dataTable, rows, 0);
        }

		/// <summary>
		/// Add rows to the DataTable
		/// </summary>
		/// <param name="dataTable">The data table</param>
		/// <param name="rows">The data rows</param>
        /// <param name="skipRows">Number of rows to skip first</param>
		private void AddRows(DataTable dataTable, string[] rows, int skipRows)
		{
			//Now add each row to the DataSet 
			//int i = 0;
            int remainedSkipRows = skipRows;
			foreach(string row in rows)
			{
				if (row != null && row.Length > 0)
				{
					string temp = row;
					if (ColumnDelimiter[0] == ' ')
					{
						// strip the extra spaces between values
						temp = StripSpaces(temp);
					}
								
					string[] items = temp.Split(ColumnDelimiter.ToCharArray());

                    if (remainedSkipRows > 0)
                    {
                        // skip this row
                        remainedSkipRows--;
                        continue;
                    }

                    if (!IsFirstRowColumns && dataTable.Columns.Count == 0)
                    {
                        // add default columns to data table
                        AddDefaultColumns(dataTable, items.Length);
                    }

					// add to the table
					if (items.Length == dataTable.Columns.Count)
					{
						dataTable.Rows.Add(items); 
					}
					else
					{
						// add a new row
						DataRow dataRow = dataTable.NewRow();
						for (int col = 0; col < dataTable.Columns.Count; col++)
						{
							if (col < items.Length)
							{
								dataRow[col] = items[col];
							}
						}
						dataTable.Rows.Add(dataRow);
					}
				}
			}
		}

		/// <summary>
		/// Get the all rows
		/// </summary>
		/// <param name="reader">The stream reader</param>
		/// <returns>The rows in string array.</returns>
		private string[] GetAllRows(StreamReader reader)
		{
			string[] rows = null;
			string rowData;

			//Read the rest of the data in the file.
			rowData = reader.ReadToEnd();

			//Split off each row at the RowDelimiter
			//Default line ending in most windows exports.  
			//You may have to edit this to match your particular file.
			//This will work for Excel, Access, etc. default exports.
			rows = rowData.Split(RowDelimiter.ToCharArray());
			
			return rows;
		}

		/// <summary>
		/// Get data rows of the next page
		/// </summary>
		/// <param name="reader">The stream reader</param>
		/// <returns>The rows in string array.</returns>
		private string[] GetPagedRows(StreamReader reader)
		{
			string[] rows = new string[_pageSize];

			int currentRowIndex = 0; // 0 based index
			
			while (currentRowIndex < _pageSize)
			{
				if (_srcRows == null)
				{
					_srcRows = GetRowsInBlock(reader);
					if (_srcRows == null)
					{
						// end of file reached
						break;
					}

					_srcRowIndex = 0;
				}

				// srcRow may be empty string, skip the empty string
				if (_srcRows[_srcRowIndex] != null && _srcRows[_srcRowIndex].Length > 0)
				{
					rows[currentRowIndex++] = _srcRows[_srcRowIndex++];
				}
				else
				{
					_srcRowIndex++;
				}

				if (_srcRowIndex >= _srcRows.Length)
				{
					_srcRows = null;
				}
			}
			
			if (currentRowIndex > 0)
			{
				return rows;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Read rows from the stream reader one block at a time.
		/// </summary>
		/// <param name="reader">The stream reader.</param>
		/// <returns>The rows of a block in string array.</returns>
		private string[] GetRowsInBlock(StreamReader reader)
		{
			int blockSize = 50000;
			string[] rows = null;
			char[] buffer = new char[blockSize];
			int count;
			string rowData;

			count = reader.ReadBlock(buffer, 0, blockSize);

			if (_rowDataBuffer == null)
			{
				_rowDataBuffer = new StringBuilder();
			}

			if (count > 0)
			{
				// convert to a string
				_rowDataBuffer.Append(buffer, 0, count);
				rowData = _rowDataBuffer.ToString();

				rows = rowData.Split(RowDelimiter.ToCharArray());
				
				if (count == blockSize)
				{
					// the last row is incomplete, save it to the buffer
					_rowDataBuffer = new StringBuilder();
					_rowDataBuffer.Append(rows[rows.Length - 1]);
					rows[rows.Length - 1] = null;
				}

			}

			return rows;
		}

		/// <summary>
		/// Add default columns to the data table
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="count"></param>
		private void AddDefaultColumns(DataTable dataTable, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dataTable.Columns.Add("Column_" + i);
			}
		}

		/// <summary>
		/// Strip the etra spaces between words to keep just one
		/// </summary>
		/// <param name="old">The old string</param>
		/// <returns>A space-stripped string</returns>
		private string StripSpaces(string old)
		{
			StringBuilder builder = new StringBuilder();
			bool firstSpace = true;

			for (int i = 0; i < old.Length; i++)
			{
				if (old[i] == ' ' || old[i] == '\t')
				{
					if (firstSpace)
					{
						firstSpace = false;
						builder.Append(' ');
					}
				}
				else
				{
					builder.Append(old[i]);
					firstSpace = true;
				}
			}

			return builder.ToString().Trim();
		}

		/// <summary>
		/// Get the file name from a file path
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns>The file name</returns>
		private new string GetFileName(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);

			return fileInfo.Name;
		}

        /// <summary>
        /// Gets number of rows that should be skipped at the beginning of a file
        /// </summary>
        /// <returns>number of rows that should be skipped</returns>
        private int GetSkippedRows()
        {
            int skipRows = 0;

            if (!_isFirstRowColumns)
            {
                if (_startingDataRow > 1)
                {
                    skipRows = _startingDataRow - 1; // _startingDataRow is one-based index
                }
            }
            else
            {
                if (_startingDataRow > 2)
                {
                    skipRows = _startingDataRow - 2; // _startingDataRow is one-based index
                }
            }

            return skipRows;
        }
	}
}
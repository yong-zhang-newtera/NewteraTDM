/*
* @(#)For001InputDataConverter.cs
*
* Copyright (c) 2003-2005 Newtera, Inc. All rights reserved.
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
	using System.Collections.Specialized;

	using Newtera.ParserGen.Converter;

	/// <summary> 
	/// The class implements a converter for input data to design system.
	/// </summary>
	/// <version> 1.0.0 15 Nov 2005</version>
	public class For001InputDataConverter : DataSourceConverterBase
	{
		private string[] COMMENTS = {"//", "-1", "CODEND"};
		private const string ASSIGNMENT = "=";
		private const string ARRAY_ELEMENT_SEPARATOR = ";";
		private const string NAME_VALUE_EXP = @"[^\s]+[=]\s*[^\s]*";

		private Regex _nameValueExp;
		private char[] _delimiters;

		private int _startSequence;
		private bool _isParameterLine;

		/// <summary>
		/// Initiate an instance of For001InputDataConverter class
		/// </summary>
		public For001InputDataConverter() : base()
		{
			// Compile regular expression to find "name = value" or "name : value" pairs
			_nameValueExp = new Regex(For001InputDataConverter.NAME_VALUE_EXP);

			// the delimiters that separate name and value pair
			_delimiters = new char[] {'='};
			_startSequence = 1;
			_isParameterLine = true;
		}

		/// <summary>
		/// Initiate an instance of For001InputDataConverter class
		/// </summary>
		/// <param name="name">The parser name</param>
		public For001InputDataConverter(string name) : base(name)
		{
		}

		/// <summary>
		/// Overriding the method to implement operation of parsing a free formed text
		/// data conforming to some regualar expressions
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies a data source</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		public override DataSet Convert(string dataSourceName)
		{
			bool columnCreated = false;

			// The DataSet to return
			DataSet dataSet = new DataSet();

			string fileName = GetFileName(dataSourceName);

			DataTable dataTable = dataSet.Tables.Add(fileName);

			// open the file as stream reader
			using (StreamReader reader = new StreamReader(dataSourceName, Encoding.Default))
			{

				string rowData;
				while ((rowData = GetRowData(reader)) != null)
				{
					NameValueCollection columnValues = GetColumnValues(rowData);

					if (!columnCreated)
					{
						// add columns to the data table
						CreateColumns(dataTable, columnValues);

						columnCreated = true; // only do it once
					}

					// add a row to the data table
					AddDataRow(dataTable, columnValues);
				}
			}

			return dataSet;
		}

		/// <summary>
		/// Get a string representing a row data from the text file
		/// </summary>
		/// <param name="reader">The reader from which to read data</param>
		/// <returns>A string containing data of a row, null if it reaches the end of the file.</returns>
		private string GetRowData(StreamReader reader)
		{
			bool beginLastArray = false;
			StringBuilder builder = new StringBuilder();

			// read lines from the file once at a time
			string line = null;
			bool beginArray = false;
			int columnCount = 0;
			while ((line = reader.ReadLine()) != null)
			{
				line = line.Trim();

				// skip empty line
				if (line == null || line.Length == 0)
				{
					continue;
				}

				// skip comment
				if (IsComment(line))
				{
					beginArray = false;
					continue;
				}

				// paramter line
				if (IsParamters(line))
				{
					if (builder.Length > 0)
					{
						builder.Append(" "); // add a space between value pairs
					}

					// add column name
					builder.Append("Col_").Append(columnCount++).Append(For001InputDataConverter.ASSIGNMENT);

					// data line, replace spaces with ,
					line = ReplaceSpaceWithComma(line);
					builder.Append(line);
				}
				else if (IsLettersLeadingLine(line))
				{
					if (!beginArray)
					{
						if (builder.Length > 0)
						{
							builder.Append(" "); // add a space between value pairs
						}

						// add column name
						builder.Append("Col_").Append(columnCount++).Append(For001InputDataConverter.ASSIGNMENT);
						
						beginArray = true;
					}
					else
					{
						builder.Append(For001InputDataConverter.ARRAY_ELEMENT_SEPARATOR);
					}

					// filling zero values
					line = FillInZeros(line);

					builder.Append(line);
				}
				else if (IsSequnceLine(line))
				{
					if (!beginArray)
					{
						if (builder.Length > 0)
						{
							builder.Append(" "); // add a space between value pairs
						}

						// add column name
						builder.Append("Col_").Append(columnCount++).Append(For001InputDataConverter.ASSIGNMENT);
						
						beginArray = true;
					}
					else
					{
						builder.Append(For001InputDataConverter.ARRAY_ELEMENT_SEPARATOR);
					}

					// remove the sequence number
					int index = line.IndexOf(" ");
					if (index > 0)
					{
						line = line.Substring(index + 1);
					}

					builder.Append(line);
				}
				else if (IsMoreThanTwoColumns(line))
				{
					if (!beginArray)
					{
						if (builder.Length > 0)
						{
							builder.Append(" "); // add a space between value pairs
						}

						// add column name
						builder.Append("Col_").Append(columnCount++).Append(For001InputDataConverter.ASSIGNMENT);
						
						beginArray = true;
					}
					else
					{
						builder.Append(For001InputDataConverter.ARRAY_ELEMENT_SEPARATOR);
					}

					// data line, replace spaces with ,
					line = ReplaceSpaceWithComma(line);
					builder.Append(line);
				}
				else
				{
					if (!beginLastArray)
					{
						_startSequence = 1; // reset the sequence for next use.
						_isParameterLine = true; // reset for the next use

						if (builder.Length > 0)
						{
							builder.Append(" "); // add a space between value pairs
						}

						// add column name
						builder.Append("Col_").Append(columnCount++).Append(For001InputDataConverter.ASSIGNMENT);
						
						beginLastArray = true;
					}
					else
					{
						builder.Append(For001InputDataConverter.ARRAY_ELEMENT_SEPARATOR);
					}

					// data line, replace spaces with ,
					line = ReplaceSpaceWithComma(line);
					builder.Append(line);
				}
			}

			if (builder.Length > 0)
			{
				return builder.ToString();
			}
			else
			{
				return null; // reached the end of file
			}
		}

		private bool IsComment(string line)
		{
			bool status = false;

			for (int i = 0; i < this.COMMENTS.Length; i++)
			{
				if (line.Length >= this.COMMENTS[i].Length &&
					line.StartsWith(this.COMMENTS[i]))
				{
					status = true;
					break;
				}
			}

			return status;
		}

		private bool IsParamters(string line)
		{
			bool status = false;

			if (_isParameterLine)
			{
				_isParameterLine = false;
				int index = line.IndexOf(" ");

				if (index > 0)
				{
					string sub = line.Substring(0, index);
					if (sub.Length > 1)
					{
						Regex regex = new Regex(@"^[a-zA-Z]+[0-9]*[a-zA-Z]*[0-9]*$");
						status = regex.IsMatch(sub);
					}
				}
			}
			else
			{
				// parameter line is one line only
				status = false;
			}

			return status;
		}

		private bool IsLettersLeadingLine(string line)
		{
			bool status = false;
			int index = line.IndexOf(" ");

			if (index > 0)
			{
				string sub = line.Substring(0, index);
				if (sub.Length == 6)
				{
					Regex regex = new Regex(@"^[a-zA-Z]+$");
					status = regex.IsMatch(sub);
				}
			}

			return status;
		}

		private bool IsSequnceLine(string line)
		{
			bool status = false;
			int index = line.IndexOf(" ");

			if (index > 0)
			{
				string sub = line.Substring(0, index);
				if (sub.Length > 0 && sub.Length < 3)
				{
					try
					{
						int sequenceNo = int.Parse(sub);

						if (sequenceNo == this._startSequence)
						{
							status = true;
							this._startSequence++;
						}
					}
					catch (Exception)
					{
					}
				}
			}

			return status;
		}

		private bool IsMoreThanTwoColumns(string line)
		{
			string delimiters = " ";
			bool status = false;
			
			string[] cols = line.Split(delimiters.ToCharArray());

			if (cols.Length > 2)
			{
				status = true;
			}

			return status;
		}

		private string FillInZeros(string line)
		{
			StringBuilder builder = new StringBuilder();
			string delimiters = " ";
			int colIndex = 0;
			int index = 0;
			string[] cols = line.Split(delimiters.ToCharArray());

			// add first col
			builder.Append(cols[index++]).Append(ARRAY_ELEMENT_SEPARATOR);
			colIndex++;

			// add empty fields if there is no trailing ,
			if (cols[index].EndsWith(","))
			{
				string val = cols[index++];
				builder.Append(val).Append(ARRAY_ELEMENT_SEPARATOR);
				colIndex++;

				// 3rd col non-empty
				builder.Append(cols[index++]).Append(ARRAY_ELEMENT_SEPARATOR);
				colIndex++;

				// 4th col is empty
				builder.Append(ARRAY_ELEMENT_SEPARATOR);
				colIndex++;
			}
			else
			{
				builder.Append(cols[index++]).Append(ARRAY_ELEMENT_SEPARATOR);
				colIndex++;

				// 3rd and 4th col are empty
				builder.Append(ARRAY_ELEMENT_SEPARATOR);
				colIndex++;
				builder.Append(ARRAY_ELEMENT_SEPARATOR);
				colIndex++;
			}

			while (colIndex < 9)
			{
				if (index < cols.Length)
				{
					builder.Append(cols[index++]).Append(ARRAY_ELEMENT_SEPARATOR);
				}
				else if (colIndex < 8)
				{
					// emty val
					builder.Append(ARRAY_ELEMENT_SEPARATOR);
				}

				colIndex++;
			}

			return builder.ToString();
		}

		/// <summary>
		/// Create columns in the data table
		/// </summary>
		/// <param name="dataTable">The DataTable instance to which the columns are added.</param>
		/// <param name="columnValues">A NameValueCollection of column names and values</param>
		private void CreateColumns(DataTable dataTable, NameValueCollection columnValues)
		{
			// create column information in the data table
			for (int i = 0; i < columnValues.Count; i++)
			{
				string columnName = columnValues.GetKey(i);
				      
				dataTable.Columns.Add(columnName);
			}
		}

		/// <summary>
		/// Parse a string containing column names and values and return a
		/// Name and Value collection.
		/// </summary>
		/// <param name="rowData">The string containing row data</param>
		/// <returns>A NameValueCollection instance</returns>
		private NameValueCollection GetColumnValues(string rowData)
		{
			NameValueCollection columnValues = new NameValueCollection();

			// parse the row data to find matches according to the expression
			MatchCollection matches = _nameValueExp.Matches(rowData);
			foreach (Match match in matches)
			{
				int pos = match.Value.IndexOfAny(_delimiters);

				if (pos > 0)
				{
					// extract the name and value
					string key = match.Value.Substring(0, pos).TrimEnd();
					string val = match.Value.Substring(pos + 1).TrimStart();
					
					columnValues.Add(key, val);
				}
			}

			return columnValues;
		}

		/// <summary>
		/// Add a row of data to the data table
		/// </summary>
		/// <param name="dataTable">The DataTable instance</param>
		/// <param name="columnValues">A NameValueCollection of column names and values</param>
		private void AddDataRow(DataTable dataTable, NameValueCollection columnValues)
		{
			// create a new DataRow instance
			DataRow dataRow = dataTable.NewRow();

			// fill the data row with data        
			for (int col = 0; col < columnValues.Count; col++)
			{
				dataRow[col] = columnValues[col];
			}

			// add the data row to the data table
			dataTable.Rows.Add(dataRow);
		}

		/// <summary>
		/// Replace the spaces in the a string with comma
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		private string ReplaceSpaceWithComma(string original)
		{
			StringBuilder builder = new StringBuilder();
			bool firstSpace = true;

			for (int i = 0; i < original.Length; i++)
			{
				if (original[i] == ' ')
				{
					if (firstSpace)
					{
						builder.Append(For001InputDataConverter.ARRAY_ELEMENT_SEPARATOR);
						firstSpace = false;
					}
				}
				else
				{
					builder.Append(original[i]);
					firstSpace = true;
				}
			}

			return builder.ToString();
		}
	}
}
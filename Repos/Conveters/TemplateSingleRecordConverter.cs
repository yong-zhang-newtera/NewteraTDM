/*
* @(#)TemplateSingleRecordConverter.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
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
	/// The class implements a converter for a free formed text file data.
	/// </summary>
	/// <version> 1.0.0 08 Dec 2004</version>
	public class TemplateSingleRecordConverter : DataSourceConverterBase
	{
		private const string ARRAY_ELEMENT_SEPARATOR = ";";
		private const string NAME_VALUE_EXP = @"[^\s]+[£º|:]\s*[^\s]*";

		private Regex _nameValueExp;
		private char[] _delimiters;

		/// <summary>
		/// Initiate an instance of TemplateSingleRecordConverter class
		/// </summary>
		public TemplateSingleRecordConverter() : base()
		{
			// Compile regular expression to find "name = value" or "name : value" pairs
			_nameValueExp = new Regex(TemplateSingleRecordConverter.NAME_VALUE_EXP);

			// the delimiters that separate name and value pair
			_delimiters = new char[] {':'};
		}

		/// <summary>
		/// Initiate an instance of TemplateSingleRecordConverter class
		/// </summary>
		/// <param name="name">The parser name</param>
		public TemplateSingleRecordConverter(string name) : base(name)
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
			StringBuilder builder = new StringBuilder();
			bool isColName = true;

			// read lines from the file once at a time
			string line = null;

			// skip the beginning empty lines
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Length > 0)
				{
					line = line.Replace("\t", " ");
					line = line.Trim();
					if (line.EndsWith(":"))
					{
						// column name line
						builder.Append(" ").Append(line.Trim()).Append(" ");
						isColName = true;
					}
					else
					{
						if (isColName)
						{
							isColName = false;
						}
						else
						{
							builder.Append(TemplateSingleRecordConverter.ARRAY_ELEMENT_SEPARATOR);
						}
						// data line, replace spaces with ,
						line = ReplaceSpaceWithComma(line);
						builder.Append(line);
					}
				}
				else 
				{
					// an empty line
					if (isColName)
					{
						builder.Append(" ").Append("0"); // a fake value
						isColName = false;
					}
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
				string col = columnValues.GetKey(i);
				bool added = false;
				string next = "";
				int colId = 0;
				while (!added)        
				{
					//Build the column name and remove any unwanted characters.
					string columnName = col + next;
					columnName = columnName.Replace("#", "");
					columnName = columnName.Replace("'", "");
					columnName = columnName.Replace("&", "");
        
					//See if the column already exists in the table
					if(!dataTable.Columns.Contains(columnName))
					{
						//if it doesn't then we add it here and mark it as added
						dataTable.Columns.Add(columnName);
						added = true;
					}
					else
					{
						//if it did exist then we increment the sequencer and try again.
						colId++;  
						next = "_" + colId.ToString();
					}         
				}
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
						builder.Append(TemplateSingleRecordConverter.ARRAY_ELEMENT_SEPARATOR);
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
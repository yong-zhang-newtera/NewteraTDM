/*
* @(#)ExcelFileConverter.cs
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
	using System.Data.OleDb;

	using Newtera.ParserGen.Converter;

	/// <summary> 
	/// The class implements a converter for Microsoft excel file.
	/// </summary>
	/// <version> 1.0.0 07 Sep 2004</version>
	internal class ExcelFileConverter : DataSourceConverterBase
	{
		/// <summary>
		/// Initiate an instance of ExcelFileConverter class
		/// </summary>
		public ExcelFileConverter() : base()
		{
		}

		/// <summary>
		/// Initiate an instance of ExcelFileConverter class
		/// </summary>
		/// <param name="name">The parser name</param>
		public ExcelFileConverter(string name) : base(name)
		{
		}

		/// <summary>
		/// Overriding the method to implement operation of converting an excel file. 
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies an excel file</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		public override DataSet Convert(string dataSourceName)
		{
			DataSet dataSet = null;
			DataTable dt = null;
			OleDbConnection objConn = null;

            string connectionStr = connectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dataSourceName + ";Extended Properties=  \"Excel 12.0;HDR=YES;IMEX=1\"";

			try
			{
				// Create connection object by using the preceding connection string.
                try
                {
                    objConn = new OleDbConnection(connectionStr);

                    // Open connection with the database.
                    objConn.Open();
                }
                catch (Exception)
                {
                    // incase 32 bit PC deosn't support Microsoft.ACE.OLEDB.12.0;
                    connectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataSourceName + ";Extended Properties=  \"Excel 8.0;HDR=YES;IMEX=1\"";
                    objConn = new OleDbConnection(connectionStr);

                    // Open connection with the database.
                    objConn.Open();
                }

				// Get the data table containg the schema guid.
				dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

				if (dt != null)
				{
                    string fileName = GetFileName(dataSourceName);

                    dataSet = new DataSet(fileName);

					String[] excelSheets = new String[dt.Rows.Count];
					int i = 0;

					// Add the sheet name to the string array.
					foreach(DataRow row in dt.Rows)
					{
						excelSheets[i] = row["TABLE_NAME"].ToString();
						i++;
					}

					for (int j = 0; j < excelSheets.Length; j++)
					{
						GetSheetData(excelSheets[j], dataSet, objConn);
					}
				}

				return dataSet;
			}
			finally
			{
				// Clean up.
				if(objConn != null)
				{
					objConn.Close();
					objConn.Dispose();
				}

				if(dt != null)
				{
					dt.Dispose();
				}
			}
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
        /// Query data from an Excel file for a given Sheet and fill the
        /// result in a data set
        /// </summary>
        /// <param name="sheetName">The Excel sheet name</param>
        /// <param name="dataSet">The data set</param>
        /// <param name="conn">The connection to the Excel file</param>
        private void GetSheetData(string sheetName, DataSet dataSet, OleDbConnection conn)
		{
			//You must use the $ after the object you reference in the spreadsheet
			string stmt;

			// strip away the quotes around the sheet name
			sheetName =	sheetName.Trim('\'');

            stmt = "SELECT * FROM [" + sheetName + "]";

			OleDbDataAdapter cmd = new OleDbDataAdapter(stmt, conn);

            string tmpSheetName = sheetName;
             if (sheetName.EndsWith("$"))
             {
                 tmpSheetName = sheetName.Substring(0, sheetName.Length - 1);
             }
            
			try
			{
                cmd.Fill(dataSet, tmpSheetName);
			}
			catch (Exception)
			{
				// The sheet name may be invalid for some reason, which prevent
				// the query from executing, ignore it
				return;

			}
		}
	}
}
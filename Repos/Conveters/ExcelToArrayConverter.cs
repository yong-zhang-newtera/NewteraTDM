/*
* @(#)ExcelToArrayConverter.cs
*
*/
namespace Newtera.Conveters
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Data;
    using System.Text;
	using System.Data.OleDb;

    using Newtera.Common.Core;
	using Newtera.ParserGen.Converter;

	/// <summary> 
	/// The class implements a converter for Microsoft excel file.
	/// </summary>
	/// <version> 1.0.0 07 Sep 2004</version>
	internal class ExcelToArrayConverter : DataSourceConverterBase
	{
		/// <summary>
		/// Initiate an instance of ExcelToArrayConverter class
		/// </summary>
		public ExcelToArrayConverter() : base()
		{
		}

		/// <summary>
		/// Initiate an instance of ExcelToArrayConverter class
		/// </summary>
		/// <param name="name">The parser name</param>
		public ExcelToArrayConverter(string name) : base(name)
		{
		}

		/// <summary>
		/// Overriding the method to implement operation of converting an excel file. 
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies an excel file</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		public override DataSet Convert(string dataSourceName)
		{
			DataTable dt = null;
			OleDbConnection objConn = null;

            DataSet dataSet = new DataSet();
            string headerId;

            DataTable headerDataTable = dataSet.Tables.Add(NewteraNameSpace.HEADER_TABLE_NAME);
            headerDataTable.Columns.Add(NewteraNameSpace.GROUP_ID); // 表头的ＩＤ
            headerDataTable.Columns.Add(NewteraNameSpace.HEADER_NAME_ARRAY); // 表头参数数组
            headerDataTable.Columns.Add(NewteraNameSpace.HEADER_VALUE_ARRAY); // 表头参数值数组
            headerDataTable.Columns.Add(NewteraNameSpace.CHANNEL_NAME_ARRAY); // 通道参数名数组

            DataTable channelDataTable = dataSet.Tables.Add(NewteraNameSpace.CHANNEL_TABLE_NAME);
            channelDataTable.Columns.Add(NewteraNameSpace.RECORD_ID); // 主键键
            channelDataTable.Columns.Add(NewteraNameSpace.GROUP_ID); // 外键
            channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_VALUE_ARRAY); // 通道参数值数组

            // 创建两个DataTable之间的关系
            dataSet.Relations.Add(NewteraNameSpace.GROUP_CHANNEL_RELATION, headerDataTable.Columns[NewteraNameSpace.GROUP_ID],
                channelDataTable.Columns[NewteraNameSpace.GROUP_ID]);

			string connectionStr;
            string fileExtension = ".XLS";
            FileInfo fileInfo = new FileInfo(dataSourceName);
            if (fileInfo.Extension != null)
            {
                fileExtension = fileInfo.Extension.ToUpper();
            }

            connectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dataSourceName + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

			try
			{
                try
                {
                    // Create connection object by using the preceding connection string.
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
						DataSet excelDataSet = GetSheetData(excelSheets[j], objConn);

                        if (excelDataSet.Tables.Count > 0)
                        {
                            // 从DataSet中读取文件表头数据并形成组记录
                            DataRow headerRow = ReadHeaderData(excelDataSet, headerDataTable);

                            if (headerRow != null)
                            {
                                // 从DataSet中读取通道数据记录
                                headerId = headerRow[NewteraNameSpace.GROUP_ID].ToString();
                                ReadChannelData(excelDataSet, channelDataTable, headerId);
                            }
                        }
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
		/// Query data from an Excel file for a given Sheet and fill the
		/// result in a data set
		/// </summary>
		/// <param name="sheetName">The Excel sheet name</param>
		/// <param name="conn">The connection to the Excel file</param>
		private DataSet GetSheetData(string sheetName, OleDbConnection conn)
		{
            DataSet dataSet = new DataSet("Excel Data");

			//You must use the $ after the object you reference in the spreadsheet
			string stmt;

			// strip away the quotes around the sheet name
			sheetName =	sheetName.Trim('\'');

            stmt = "SELECT * FROM [" + sheetName + "]";

			OleDbDataAdapter cmd = new OleDbDataAdapter(stmt, conn);
            
			try
			{
				cmd.Fill(dataSet, sheetName);

                return dataSet;
			}
			catch (Exception)
			{
				// The sheet name may be invalid for some reason, which prevent
				// the query from executing, ignore it
				return null;
			}
		}

        /// <summary>
        /// 从DataSet读取表头数据,并作为DataTable中的唯一记录
        /// </summary>
        private DataRow ReadHeaderData(DataSet excelDataSet, DataTable headerDataTable)
        {
            DataRow dataRow = null;
            StringBuilder namesBuilder = new StringBuilder();

            string rowId = null;

            // 创建通道名称数组
            int index = 0;
            DataTable excelDataTable = excelDataSet.Tables[0];

            // 调过空的工作簿
            if (excelDataTable.Columns.Count > 1)
            {
                dataRow = headerDataTable.NewRow();

                // 设置主键
                rowId = CreateRowId();
                dataRow[NewteraNameSpace.GROUP_ID] = rowId;

                foreach (DataColumn dataColumn in excelDataTable.Columns)
                {
                    if (index > 0)
                    {
                        namesBuilder.Append(";");
                    }

                    namesBuilder.Append(dataColumn.ColumnName);

                    index++;
                }

                dataRow[NewteraNameSpace.CHANNEL_NAME_ARRAY] = namesBuilder.ToString();
                dataRow[NewteraNameSpace.HEADER_NAME_ARRAY] = "";
                dataRow[NewteraNameSpace.HEADER_VALUE_ARRAY] = "";

                headerDataTable.Rows.Add(dataRow);
            }

            return dataRow;
        }

        /// <summary>
        /// 读取通道数据记录
        /// </summary>
        private void ReadChannelData(DataSet excelDataSet, DataTable channelDataTable, string groupId)
        {
            DataRow dataRow;
            StringBuilder channelValues;

            // 读取通道数据记录
            DataTable excelDataTable = excelDataSet.Tables[0];
            foreach (DataRow excelDataRow in excelDataTable.Rows)
            {
                dataRow = channelDataTable.NewRow();
                channelValues = new StringBuilder();

                // 设置外键
                dataRow[NewteraNameSpace.GROUP_ID] = groupId;
                dataRow[NewteraNameSpace.RECORD_ID] = CreateRowId();

                // add the data row to the data table
                channelDataTable.Rows.Add(dataRow);

                for (int index = 0; index < excelDataTable.Columns.Count; index++)
                {
                    if (index > 0)
                    {
                        channelValues.Append(";");
                    }
                    if (!excelDataRow.IsNull(index))
                    {
                        channelValues.Append(excelDataRow[index].ToString());
                    }
                    else
                    {
                        channelValues.Append("");
                    }
                }

                dataRow[NewteraNameSpace.CHANNEL_VALUE_ARRAY] = channelValues.ToString();
            }
        }

        /// <summary>
        /// 生成一个行记录ＩＤ值
        /// </summary>
        private string CreateRowId()
        {
            string id = "R_" + Guid.NewGuid();

            return id;
        }
	}
}
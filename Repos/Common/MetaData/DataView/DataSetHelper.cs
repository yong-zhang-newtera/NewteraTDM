/*
* @(#)DataSetHelper.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Data;
	
	using Newtera.Common.Core;

	/// <summary>
	/// A helper class that provide some convinient access methods to data set
	/// </summary>
	/// <version>  1.0.1 13 Nov 2003</version>
	/// <author>  Yong Zhang</author>
	public class DataSetHelper
	{
		/// <summary>
		/// Gets the value indicating whether the data set contains data
		/// for a table
		/// </summary>
		/// <param name="dataSet">The data set</param>
		/// <param name="tableName">The table name</param>
		/// <returns>true if it contains no data, false otherwise.</returns>
		public static bool IsEmptyDataSet(DataSet dataSet, string tableName)
		{
			bool status = true;

			if (dataSet != null)
			{
				// find the table , check if it is empty
				DataTable dataTable = dataSet.Tables[tableName];
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					// Check the obj_id field
					if (dataTable.Rows.Count != 1 ||
						(dataTable.Columns[NewteraNameSpace.OBJ_ID] != null &&
						!dataTable.Rows[0].IsNull(NewteraNameSpace.OBJ_ID) &&
						  dataTable.Rows[0][NewteraNameSpace.OBJ_ID].ToString().Length > 0))
					{						
						status = false;
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Gets the value of a cell in data set
		/// </summary>
		/// <param name="dataSet">The DataSet</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <param name="rowIndex">The row index</param>
		/// <returns>The value of a cell, or null if the cell doesn't exist.</returns>
		public static string GetCellValue(DataSet dataSet, string tableName, string columnName, int rowIndex)
		{
			string val = null;

			if (dataSet != null)
			{
				DataTable dataTable = dataSet.Tables[tableName];

				if (dataTable != null && rowIndex < dataTable.Rows.Count &&
					dataTable.Columns.Contains(columnName))
				{
					if (!dataTable.Rows[rowIndex].IsNull(columnName))
					{
						val = dataTable.Rows[rowIndex][columnName].ToString();
					}
					else
					{
						val = "";
					}
				}
			}

			return val;
		}

		/// <summary>
		/// Sets a value to a specified cell in data set
		/// </summary>
		/// <param name="dataSet">The DataSet</param>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <param name="rowIndex">The row index</param>
		/// <param name="val">The value to be set.</param>
		/// <remarks>If the specified cell doen't exists, nothing is done.</remarks>
		public static void SetCellValue(DataSet dataSet, string tableName, string columnName, int rowIndex, string val)
		{
			if (dataSet != null)
			{
				DataTable dataTable = dataSet.Tables[tableName];

				if (dataTable != null && rowIndex < dataTable.Rows.Count &&
					dataTable.Columns.Contains(columnName))
				{
					dataTable.Rows[rowIndex][columnName] = val;
				}
			}
		}

		/// <summary>
		/// Get count of rows of a given table in data set.
		/// </summary>
		/// <param name="dataSet">The data set</param>
		/// <param name="tableName">The table name</param>
		/// <returns></returns>
		public static int GetRowCount(DataSet dataSet, string tableName)
		{
			int count = 0;

			if (dataSet != null && dataSet.Tables[tableName] != null)
			{
				count = dataSet.Tables[tableName].Rows.Count;
			}

			return count;
		}
	}
}
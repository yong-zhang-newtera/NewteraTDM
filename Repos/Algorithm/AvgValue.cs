/*
* @(#)AvgValue.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Algorithm
{
	using System;
	using System.Xml;
	using System.Data;
    using System.Text;

	/// <summary>
	/// Implementation of an algorithm that calculate the average value of each column in the data table
	/// </summary>
	/// <version> 1.0.0 26 Aug 2007</version>
    public class AvgValue : AlgorithmBase, IAlgorithm 
	{
		/// <summary>
		/// Get average value of each column in the datatable
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for algorithm.</param>
        /// <returns>The avg values separated by semicolor.</returns>
        public string Execute(DataTable dataTable)
        {
            StringBuilder builder = new StringBuilder();
            if (dataTable != null)
            {
                int col = 0;
                string val;
                int count;
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    string avgValue = "0";
                    count = 0;
                    string dataType = (string) dataColumn.ExtendedProperties["DataType"];
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        if (!dataTable.Rows[row].IsNull(col))
                        {
                            val = dataTable.Rows[row][col].ToString();

                            avgValue = AddValues(avgValue, val, dataType);

                            count++;
                        }
                    }

                    avgValue = DivideValues(avgValue, count.ToString(), dataType);

                    // keep the avg value of a column
                    if (col == 0)
                    {
                        // first avg value
                        builder.Append(avgValue);
                    }
                    else
                    {
                        builder.Append(";").Append(avgValue);
                    }

                    col++;
                }
            }

            return builder.ToString();
        }
	}
}
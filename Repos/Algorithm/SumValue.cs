/*
* @(#)SumValue.cs
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
	/// Implementation of an algorithm that calculate the sum value of each column in the data table
	/// </summary>
	/// <version> 1.0.0 26 Aug 2007</version>
    public class SumValue : AlgorithmBase, IAlgorithm 
	{
		/// <summary>
		/// Get sum value of each column in the datatable
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
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    string sumValue = "0";
                    string dataType = (string) dataColumn.ExtendedProperties["DataType"];
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        if (!dataTable.Rows[row].IsNull(col))
                        {
                            val = dataTable.Rows[row][col].ToString();

                            sumValue = AddValues(sumValue, val, dataType);
                        }
                    }

                    // keep the sum value of a column
                    if (col == 0)
                    {
                        // first sum value
                        builder.Append(sumValue);
                    }
                    else
                    {
                        builder.Append(";").Append(sumValue);
                    }

                    col++;
                }
            }

            return builder.ToString();
        }
	}
}
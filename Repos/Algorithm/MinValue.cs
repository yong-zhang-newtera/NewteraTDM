/*
* @(#)MinValue.cs
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
    /// Implementation of an algorithm that calculate the min value of each column in the data table
    /// </summary>
	/// <version> 1.0.0 20 Aug 2007</version>
	public class MinValue : AlgorithmBase, IAlgorithm 
	{
		/// <summary>
		/// Get max value of each column in the datatable
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for algorithm.</param>
        /// <returns>The max values separated by semicolor.</returns>
        public string Execute(DataTable dataTable)
        {
            StringBuilder builder = new StringBuilder();
            if (dataTable != null)
            {
                int col = 0;
                string val;
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    string maxValue = null;
                    string dataType = (string)dataColumn.ExtendedProperties["DataType"];
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        if (!dataTable.Rows[row].IsNull(col))
                        {
                            val = dataTable.Rows[row][col].ToString();
                            if (maxValue == null)
                            {
                                maxValue = val;
                            }
                            else if (CompareValues(maxValue, val, dataType) > 0)
                            {
                                maxValue = val;
                            }
                        }
                    }

                    // keep the max value of a column
                    if (col == 0)
                    {
                        // first max value
                        builder.Append(maxValue);
                    }
                    else
                    {
                        builder.Append(";").Append(maxValue);
                    }

                    col++;
                }
            }

            return builder.ToString();
        }
	}
}
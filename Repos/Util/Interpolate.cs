/*
* @(#)Interpolate.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Util
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Data;
	using System.Text;
	using System.ComponentModel;
	using System.Collections;

	using Newtera.Common.MetaData.DataView;

	/// <summary> 
	/// The class implements the Interpolate algorithm (曲线插值)
	/// </summary>
	/// <version> 1.0.0 09 Apr 2005</version>
	public class Interpolate
	{
		/// <summary>
		/// Initiate an instance of Interpolate class
		/// </summary>
		public Interpolate()
		{
		}

		/// <summary>
		/// Perform the algorithm and insert points into the data array
		/// </summary>
		/// <param name="beforeData">The original array data</param>
		/// <param name="xAxisIndex">X Axis Index.</param>
		/// <param name="type">One of the InterpolateType enum values.</param>
		/// <param name="points">The points to be inserted in the array data.</param>
		/// <returns>The interpolated array data.</returns>
		public DataTable Compute(DataTable beforeData, int xAxisIndex, InterpolateType type, double[] points)
		{
			DataTable afterData = null;

			switch (type)
			{
				case InterpolateType.Linear:

					afterData = ComputeLinear(beforeData, xAxisIndex, points);

					break;

				case InterpolateType.Paraboloidal:

					afterData = ComputeParaboloidal(beforeData, xAxisIndex, points);

					break;
			}

			return afterData;
		}

		/// <summary>
		/// Perform Linear interpolate algorithm.(线性插值)
		/// </summary>
		/// <param name="beforeData">The original array data</param>
		/// <param name="xAxisIndex">X Axis Index.</param>
		/// <param name="points">The points to be inserted in the array data.</param>
		/// <returns>The interpolated array data.</returns>
		private DataTable ComputeLinear(DataTable beforeData, int xAxisIndex, double[] points)
		{
			DataTable afterData = beforeData.Copy(); // make a copy
			int rowIndex;
			DataRow newRow;
			double y;

			if (afterData.Rows.Count > 1)
			{
				// insert a new row for each interpolated point
				foreach (double point in points)
				{
					rowIndex = FindInsertRowIndex(afterData, xAxisIndex, point);

					// instantiate a new row with calculated values
					newRow = afterData.NewRow();

					newRow[xAxisIndex] = point;
					
					double x1, y1, x2, y2;
					int rowCount = afterData.Rows.Count;
					for(int i = 0; i < afterData.Columns.Count; i++)
					{
						if (i != xAxisIndex)
						{
							try
							{
								if (rowIndex == 0)
								{
									// insert a point at the beginning, take the first two points
									// to calculate.
									x1 = double.Parse(afterData.Rows[0][xAxisIndex].ToString()); /* x1 */
									y1 = double.Parse(afterData.Rows[0][i].ToString()); /* y1 */
									x2 = double.Parse(afterData.Rows[1][xAxisIndex].ToString()); /* x2 */
									y2 = double.Parse(afterData.Rows[1][i].ToString()); /* y2 */
								}
								else if (rowIndex == rowCount)
								{
									// add a point at the end, take the last two points
									x1 = double.Parse(afterData.Rows[rowCount - 2][xAxisIndex].ToString()); /* x1 */
									y1 = double.Parse(afterData.Rows[rowCount - 2][i].ToString()); /* y1 */
									x2 = double.Parse(afterData.Rows[rowCount - 1][xAxisIndex].ToString()); /* x2 */
									y2 = double.Parse(afterData.Rows[rowCount - 1][i].ToString()); /* y2 */
								}
								else
								{
									// insert a new point in between, take sibling points
									x1 = double.Parse(afterData.Rows[rowIndex - 1][xAxisIndex].ToString()); /* x1 */
									y1 = double.Parse(afterData.Rows[rowIndex - 1][i].ToString()); /* y1 */
									x2 = double.Parse(afterData.Rows[rowIndex][xAxisIndex].ToString()); /* x2 */
									y2 = double.Parse(afterData.Rows[rowIndex][i].ToString()); /* y2 */
								}

								y = CalculateLinearValue(point, /* x */
														  x1, y1, x2, y2);
							}
							catch (Exception)
							{
								y = 0;
							}

							newRow[i] = y;
						}
					}

					// insert the new row
					if (rowIndex < afterData.Rows.Count)
					{
						afterData.Rows.InsertAt(newRow, rowIndex);
					}
					else
					{
						afterData.Rows.Add(newRow);
					}
				}
			}

			return afterData;
		}

		/// <summary>
		/// Perform Paraboloidal interpolate algorithm. (抛物插值)
		/// </summary>
		/// <param name="beforeData">The original array data</param>
		/// <param name="xAxisIndex">X Axis Index.</param>
		/// <param name="points">The points to be inserted in the array data.</param>
		/// <returns>The interpolated array data.</returns>
		private DataTable ComputeParaboloidal(DataTable beforeData, int xAxisIndex, double[] points)
		{
			DataTable afterData = beforeData.Copy(); // make a copy
			int rowIndex;
			DataRow newRow;
			double y;

			// Paraboloidal requires at least three x values
			if (beforeData.Rows.Count > 2)
			{
				// insert a new row for each interpolated point
				foreach (double x in points)
				{
					// Note here, afterData is the one containing new inserted rows
					rowIndex = FindInsertRowIndex(afterData, xAxisIndex, x);

					// instantiate a new row with calculated values
					newRow = afterData.NewRow();

					newRow[xAxisIndex] = x;

					for(int i = 0; i < afterData.Columns.Count; i++)
					{
						if (i != xAxisIndex)
						{
							// determine the index of a middle row of three rows where 
							// x(i-1), x(i), x(i+1) are closest to the interpolated x value.
							int middleRowIndex = FindMiddleXRowIndex(afterData, xAxisIndex, x);

							y = CalculateParaboloidalValue(afterData, xAxisIndex, i, x, middleRowIndex);
						

							newRow[i] = y;
						}
					}

					// insert the new row
					if (rowIndex < afterData.Rows.Count)
					{
						afterData.Rows.InsertAt(newRow, rowIndex);
					}
					else
					{
						afterData.Rows.Add(newRow);
					}
				}
			}

			return afterData;
		}

		/// <summary>
		/// Find the insert position where to add a new row in the datatable
		/// </summary>
		/// <param name="dataTable">The data table</param>
		/// <param name="xAxisIndex">The x axis index</param>
		/// <param name="point">The point to be inserted</param>
		/// <returns>The row index where the new row to be inserted</returns>
		/// <remarks>Find the row index that satisfy xi-1 < point < xi, assuming that
		/// x0 < x1 < .... < xn</remarks>
		private int FindInsertRowIndex(DataTable dataTable, int xAxisIndex, double point)
		{
			int rowIndex = dataTable.Rows.Count; // add a row to the end by default
	
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				try
				{
					double x = double.Parse(dataTable.Rows[i][xAxisIndex].ToString());
					if (x >= point)
					{
						// found
						rowIndex = i;
						break;
					}
				}
				catch(Exception)
				{
				}
			}

			return rowIndex;
		}

		/// <summary>
		/// Determine the row index where x value is in the middle of three x values
		/// that are closest to the interpolated x value.
		/// </summary>
		/// <param name="dataTable">The data table</param>
		/// <param name="xAxisIndex">The x axis index</param>
		/// <param name="x">The x value to be inserted</param>
		/// <returns>The row index for the middle x among the three points for Paraboloidal formular.</returns>
		private int FindMiddleXRowIndex(DataTable dataTable, int xAxisIndex, double x)
		{
			int rowIndex = dataTable.Rows.Count - 2; // default
			int rowCount = dataTable.Rows.Count;
	
			double x1, x2, x3, mx1, mx2;
			// looping from 1 to n-2
			for (int j = 1; j < rowCount - 1; j++)
			{
				try
				{
					x1 = double.Parse(dataTable.Rows[j-1][xAxisIndex].ToString());
					x2 = double.Parse(dataTable.Rows[j][xAxisIndex].ToString());
					x3 = double.Parse(dataTable.Rows[j+1][xAxisIndex].ToString());
					mx1 = 0.5 * (x1 + x2);
					mx2 = 0.5 * (x1 + x2);

					if (j == 1 && x < mx2)
					{
						rowIndex = 1;
						break;
					}
					else if (j == (rowCount - 2) && x >= mx1)
					{
						rowIndex = dataTable.Rows.Count - 2;
						break;
					}
					else if (x >= mx1 && x < mx2)
					{
						rowIndex = j;
						break;
					}
				}
				catch(Exception)
				{
				}
			}

			return rowIndex;
		}

		/// <summary>
		/// Calculate a Y value for a X value using linear interpolate formula
		/// </summary>
		/// <param name="x">The X value</param>
		/// <param name="x1">The X value of the first point</param>
		/// <param name="y1">The Y value of the first point</param>
		/// <param name="x2">The X value of the second point</param>
		/// <param name="y2">The Y value of the second point</param>
		/// <returns>The calculated Y value</returns>
		private double CalculateLinearValue(double x, double x1, double y1, double x2, double y2)
		{

			return y1 * (x - x2) / (x1 - x2) + y2 * (x - x1) / (x2 - x1);
		}

		/// <summary>
		/// Calculate a Y value using the Paraboloidal interpolate formula
		/// </summary>
		/// <param name="dataArray">The DataTable instance</param>
		/// <param name="xAxisIndex">The x axis index in the DataTable instance</param>
		/// <param name="yAxisIndex">The y axis index in the DataTable instance</param>
		/// <param name="x">The x axis value to be inserted in the DataTable instance</param>
		/// <param name="middleRowIndex">The row index of an x value that is the middle x among the three pints</param>
		/// <returns>The calculated Y value</returns>
		private double CalculateParaboloidalValue(DataTable dataArray,
			int xAxisIndex, int yAxisIndex, double x, int middleRowIndex)
		{
			double y = 0;

			try
			{
				double x1 = double.Parse(dataArray.Rows[middleRowIndex - 1][xAxisIndex].ToString());
				double x2 = double.Parse(dataArray.Rows[middleRowIndex][xAxisIndex].ToString());
				double x3 = double.Parse(dataArray.Rows[middleRowIndex + 1][xAxisIndex].ToString());

				double y1 = double.Parse(dataArray.Rows[middleRowIndex - 1][yAxisIndex].ToString());
				double y2 = double.Parse(dataArray.Rows[middleRowIndex][yAxisIndex].ToString());
				double y3 = double.Parse(dataArray.Rows[middleRowIndex + 1][yAxisIndex].ToString());

				y = y1 * ((x - x2) / (x1 - x2)) * ((x - x3) / (x1 - x3)) +
					y2 * ((x - x1) / (x2 - x1)) * ((x - x3) / (x2 - x3)) +
					y3 * ((x - x1) / (x3 - x1)) * ((x - x2) / (x3 - x2));
			}
			catch (Exception)
			{
			}

			return y;
		}
	}

	public enum InterpolateType
	{
		/// <summary>
		/// 线性插值
		/// </summary>
		Linear,
		/// <summary>
		/// 抛物线插值
		/// </summary>
		Paraboloidal
	}
}
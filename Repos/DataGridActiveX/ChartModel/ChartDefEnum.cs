/*
* @(#)ChartDefEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	/// <summary>
	/// Specify the types of charts
	/// </summary>
	public enum ChartType
	{
		/// <summary>
		/// Line Chart
		/// </summary>
		Line = 0,
		/// <summary>
		/// Contour chart
		/// </summary>
		Contour = 1,
        /// <summary>
		/// Contour chart
		/// </summary>
		Bar = 2,
        /// <summary>
        /// Data Grid
        /// </summary>
        Grid =3
	}

	/// <summary>
	/// Specify the orientation of data series
	/// </summary>
	public enum DataSeriesOrientation
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// ChartManager
		/// </summary>
		ByRow,
		/// <summary>
		/// ChartDef
		/// </summary>
		ByColumn
	}

	/// <summary>
	/// Specify the various display methods for Line Chart
	/// </summary>
	public enum LineChartDisplayMethod
	{
		/// <summary>
		/// OneXOneY
		/// </summary>
		OneXOneY,
		/// <summary>
		/// OneXManyY
		/// </summary>
		OneXManyY,
		/// <summary>
		/// ManyXOneY
		/// </summary>
		ManyXOneY,
		/// <summary>
		/// ManyXManyY
		/// </summary>
		ManyXManyY,
		/// <summary>
		/// ManyCoordinates
		/// </summary>
		ManyCoordinates
	}

	/// <summary>
	/// Specify the location of legend
	/// </summary>
	public enum LegendLocation
	{
		/// <summary>
		/// Right of chart
		/// </summary>
		Right,
		/// <summary>
		/// Left of chart
		/// </summary>
		Left,
		/// <summary>
		/// Top of chart
		/// </summary>
		Top,
		/// <summary>
		/// Bottom of chart
		/// </summary>
		Bottom
	}
}
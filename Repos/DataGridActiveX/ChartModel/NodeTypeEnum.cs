/*
* @(#)NodeTypeEnum.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	/// <summary>
	/// Specify the types of nodes in ChartModel package.
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// ChartManager
		/// </summary>
		ChartModelManager,
		/// <summary>
		/// ChartDef
		/// </summary>
		ChartDef,
		/// <summary>
		/// LineChartDef
		/// </summary>
		LineChartDef,
		/// <summary>
		/// ChartDefCollection
		/// </summary>
		ChartDefCollection,
		/// <summary>
		/// AxisDef
		/// </summary>
		AxisDef,
		/// <summary>
		/// LineDef
		/// </summary>
		LineDef,
		/// <summary>
		/// LineDefCollection
		/// </summary>
		LineDefCollection,
		/// <summary>
		/// DataPoint
		/// </summary>
		DataPoint,
		/// <summary>
		/// DataPointCollection 
		/// </summary>
		DataPointCollection,
		/// <summary>
		/// ChartInfo
		/// </summary>
		ChartInfo,
		/// <summary>
		/// ChartInfoCollection
		/// </summary>
		ChartInfoCollection,
		/// <summary>
		/// ContourChartDef
		/// </summary>
		ContourChartDef,
		/// <summary>
		/// DataSeries
		/// </summary>
		DataSeries
	}
}
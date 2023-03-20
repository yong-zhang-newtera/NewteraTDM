/*
* @(#)NodeFactory.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IChartNode based on a xml element
	/// </summary>
	/// <version>1.0.0 24 Apr 2006 </version>
	public class NodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static NodeFactory theFactory;
		
		static NodeFactory()
		{
			theFactory = new NodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private NodeFactory()
		{
		}

		/// <summary>
		/// Gets the NodeFactory instance.
		/// </summary>
		/// <returns> The NodeFactory instance.</returns>
		static public NodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IChartNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IChartNode instance</returns>
		public IChartNode Create(XmlElement xmlElement)
		{
			IChartNode obj = null;

			string elemntName = xmlElement.Name;

			NodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case NodeType.ChartDef:
					obj = new ChartDef(xmlElement);
					break;
				case NodeType.LineChartDef:
					obj = new LineChartDef(xmlElement);
					break;
				case NodeType.ChartDefCollection:
					obj = new ChartDefCollection(xmlElement);
					break;
				case NodeType.AxisDef:
					obj = new AxisDef(xmlElement);
					break;
				case NodeType.LineDef:
					obj = new LineDef(xmlElement);
					break;
				case NodeType.LineDefCollection:
					obj = new LineDefCollection(xmlElement);
					break;
				case NodeType.DataPoint:
					obj = new DataPoint(xmlElement);
					break;
				case NodeType.DataPointCollection:
					obj = new DataPointCollection(xmlElement);
					break;
				case NodeType.ChartInfo:
					obj = new ChartInfo(xmlElement);
					break;
				case NodeType.ContourChartDef:
					obj = new ContourChartDef(xmlElement);
					break;
				case NodeType.DataSeries:
					obj = new DataSeries(xmlElement);
					break;
			}
			
			return obj;
		}

		/// <summary>
		/// Convert a NodeType value to a string
		/// </summary>
		/// <param name="type">One of NodeType values</param>
		/// <returns>The corresponding string</returns>
		internal static string ConvertTypeToString(NodeType type)
		{
			string str = "Unknown";

			switch (type)
			{
				case NodeType.ChartModelManager:
					str = "Manager";
					break;
				case NodeType.ChartDef:
					str = "Chart";
					break;
				case NodeType.LineChartDef:
					str = "LineChart";
					break;
				case NodeType.ChartDefCollection:
					str = "Charts";
					break;
				case NodeType.AxisDef:
					str = "Axis";
					break;
				case NodeType.LineDef:
					str = "Line";
					break;
				case NodeType.LineDefCollection:
					str = "Lines";
					break;
				case NodeType.DataPoint:
					str = "P"; // make it short since there will be a lots of DataPoint nodes
					break;
				case NodeType.DataPointCollection:
					str = "Points";
					break;
				case NodeType.ChartInfo:
					str = "ChartInfo";
					break;
				case NodeType.ContourChartDef:
					str = "ContourChart";
					break;
				case NodeType.DataSeries:
					str = "DataSeries";
					break;
			}

			return str;
		}

		/// <summary>
		/// Convert a type string to a NodeType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of NodeType values</returns>
		internal static NodeType ConvertStringToType(string str)
		{
			NodeType type = NodeType.Unknown;

			switch (str)
			{
				case "Manager":
					type = NodeType.ChartModelManager;
					break;
				case "Chart":
					type = NodeType.ChartDef;
					break;
				case "LineChart":
					type = NodeType.LineChartDef;
					break;
				case "Charts":
					type = NodeType.ChartDefCollection;
					break;
				case "Axis":
					type = NodeType.AxisDef;
					break;
				case "Line":
					type = NodeType.LineDef;
					break;
				case "Lines":
					type = NodeType.LineDefCollection;
					break;
				case "P":
					type = NodeType.DataPoint;
					break;
				case "Points":
					type = NodeType.DataPointCollection;
					break;
				case "ChartInfo":
					type = NodeType.ChartInfo;
					break;
				case "ContourChart":
					type = NodeType.ContourChartDef;
					break;
				case "DataSeries":
					type = NodeType.DataSeries;
					break;
			}

			return type;
		}
	}
}
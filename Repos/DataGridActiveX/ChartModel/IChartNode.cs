/*
* @(#)IChartNode.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for the nodes in ChartModel package.
	/// </summary>
	/// <version> 1.0.0 24 April 2006</version>
	public interface IChartNode
	{
		/// <summary>
		/// Value changed handler
		/// </summary>
		event EventHandler ValueChanged;

		/// <summary>
		/// Gets or sets the information indicating whether the content of the Node
		/// has been altered or not
		/// </summary>
		/// <value>True when it is altered, false otherwise.</value>
		bool IsAltered {get; set;}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		NodeType NodeType {get;}

		/// <summary>
		/// create objects from xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		void Unmarshal(XmlElement parent);

		/// <summary>
		/// write object to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		void Marshal(XmlElement parent);
	}
}
/*
* @(#)IMappingNode.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for the nodes in Mappings package.
	/// </summary>
	/// <version> 1.0.0 02 Sep 2004</version>
	/// <author>  Yong Zhang </author>
	public interface IMappingNode : IMetaDataElement
	{
		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		NodeType NodeType {get;}

		/// <summary>
		/// Make a copy of the IMappingNode instance
		/// </summary>
		/// <returns>A copy of IMappingNode instance</returns>
		IMappingNode Copy();

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
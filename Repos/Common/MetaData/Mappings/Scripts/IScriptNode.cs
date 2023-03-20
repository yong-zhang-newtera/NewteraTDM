/*
* @(#)IScriptNode.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for the nodes in Scripts package.
	/// </summary>
	/// <version> 1.0.0 23 Sep 2004</version>
	/// <author>  Yong Zhang </author>
	public interface IScriptNode
	{
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
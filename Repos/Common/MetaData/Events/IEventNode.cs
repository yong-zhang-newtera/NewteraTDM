/*
* @(#)IEventNode.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for the nodes in Events package.
	/// </summary>
	/// <version> 1.0.0 21 Dec 2006</version>
	public interface IEventNode
	{
		/// <summary>
		/// Value changed handler
		/// </summary>
		event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets the name of event
        /// </summary>
        string Name { get; set;}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		EventNodeType NodeType {get;}

        /// <summary>
        /// Accept a visitor of IEventNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        void Accept(IEventNodeVisitor visitor);

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
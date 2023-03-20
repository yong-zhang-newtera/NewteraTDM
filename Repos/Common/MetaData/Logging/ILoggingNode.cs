/*
* @(#)ILoggingNode.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for the nodes in logging namespac.
	/// </summary>
	/// <version>  	1.0.0 04 Jan 2009</version>
	public interface ILoggingNode
	{
		/// <summary>
		/// Value change handler
		/// </summary>
		event EventHandler ValueChanged;

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		NodeType NodeType {get;}

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        void Accept(ILoggingNodeVisitor visitor);

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
/*
* @(#)ISiteMapNode.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a common interface for the site map and side menu node.
	/// </summary>
	/// <version>  	1.0.0 14 Jun 2009</version>
	public interface ISiteMapNode : IXaclObject
	{
        /// <summary>
        /// Title changed event handler
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Gets or sets the parent node, can be null
        /// </summary>
        ISiteMapNode ParentNode { get; set;}

        /// <summary>
        /// Gets the child nodes, can be null
        /// </summary>
        SiteMapNodeCollection ChildNodes { get;}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		NodeType NodeType {get;}

        /// <summary>
        /// Gets or sets name of node
        /// </summary>
        string Name { get; set;}

        /// <summary>
        /// Gets or sets title of node
        /// </summary>
        string Title {get; set;}

        /// <summary>
        /// Gets or sets description of node
        /// </summary>
        string Description { get; set;}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        void Accept(ISiteMapNodeVisitor visitor);

        /// <summary>
        /// Add a node as a child
        /// </summary>
        /// <param name="child"></param>
        void AddChildNode(ISiteMapNode child);

        /// <summary>
        /// Delete a child node
        /// </summary>
        /// <param name="child"></param>
        void DeleteChildNode(ISiteMapNode child);

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

        /// <summary>
        /// Gets the node path that consists of node's displayed titles
        /// </summary>
        /// <returns>A title path</returns>
        string ToDisplayPath();

        /// <summary>
        /// Gets the node 's unique hash code
        /// </summary>
        /// <returns>A hashcode</returns>
        int GetNodeHashCode();
    }
}
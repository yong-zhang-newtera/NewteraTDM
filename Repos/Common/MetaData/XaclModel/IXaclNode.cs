/*
* @(#)IXaclNode.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a common interface for the xacl nodes.
	/// </summary>
	/// <version>  	1.0.0 08 Dec 2003</version>
	/// <author>  Yong Zhang </author>
	public interface IXaclNode
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
        /// Accept a visitor of IXaclNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        void Accept(IXaclNodeVisitor visitor);

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
/*
* @(#)IModelElement.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the model element in WFModel namespace.
	/// </summary>
	/// <version> 1.0.0 7 Dec. 2006</version>
	public interface IWFModelElement
	{
		/// <summary>
		/// Value changed event handler
		/// </summary>
		event EventHandler ValueChanged;

        /// <summary>
        /// Gets database generated ID of the element
        /// </summary>
        /// <returns>An unique ID</returns>
        string ID { get; set;}

		/// <summary>
		/// Gets name of the element
		/// </summary>
		/// <returns>The element name</returns>
        string Name { get; set;}

		/// <summary>
		/// Gets or sets description of the element
		/// </summary>
		/// <returns>The element description</returns>
		string Description { get; set;}	
	
		/// <summary>
		/// Gets or sets display position of the element
		/// </summary>
		/// <returns>The display position</returns>
		int Position { get; set;}

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        ElementType ElementType { get;}

        /// <summary>
        /// Accept a visitor of IWFModelElementVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A IWFModelElementVisitor visitor</param>
        void Accept(IWFModelElementVisitor visitor);

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        void Unmarshal(XmlElement parent);

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        void Marshal(XmlElement parent);
	}
}
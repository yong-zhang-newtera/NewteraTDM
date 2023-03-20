/*
* @(#)IProjectModelElement.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the elements for parser project model.
	/// </summary>
	/// <version> 1.0.0 11 Nov. 2004</version>
	/// <author>  Yong Zhang </author>
	public interface IProjectModelElement
	{
		/// <summary>
		/// Value changed event handler
		/// </summary>
		event EventHandler ValueChanged;

		/// <summary>
		/// Gets or sets the status of value changed
		/// </summary>
		bool IsValueChanged {get; set;}

		/// <summary>
		/// Gets name of the element
		/// </summary>
		/// <returns>The element name</returns>
		string Name {get;}

		/// <summary>
		/// Gets or sets Caption of the element
		/// </summary>
		/// <returns>The element caption</returns>
		string Caption { get; set;}

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
		/// Gets or sets the parent element of the element
		/// </summary>
		IProjectModelElement ParentElement {get; set;}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		ElementType ElementType {get;}

		/// <summary>
		/// Reset the value change status in the project model
		/// </summary>
		void Reset();

		/// <summary>
		/// Accept a visitor of IProjectModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		void Accept(IProjectModelElementVisitor visitor);

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
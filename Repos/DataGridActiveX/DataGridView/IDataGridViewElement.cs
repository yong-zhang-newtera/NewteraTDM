/*
* @(#)IDataGridViewElement.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the elements in ClassViewLite name space.
	/// </summary>
	/// <version>  	1.0.0 28 May 2006</version>
	/// <author>  Yong Zhang </author>
	public interface IDataGridViewElement
	{
		/// <summary>
		/// Gets or sets the name of the element
		/// </summary>
		string Name {get; set;}

		/// <summary>
		/// Gets or sets the caption of the element
		/// </summary>
		string Caption {get; set;}

		/// <summary>
		/// Gets or sets the DataGridViewModel that owns this element
		/// </summary>
		DataGridViewModel DataGridView {get; set;}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		ViewElementType ElementType {get;}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		void Accept(IDataGridViewElementVisitor visitor);

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
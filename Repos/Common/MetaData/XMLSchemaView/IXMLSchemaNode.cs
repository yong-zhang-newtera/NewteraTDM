/*
* @(#)IXMLSchemaNode.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a common interface for the elements in XMLSchemaView namespace.
	/// </summary>
	/// <version>  	1.0.0 10 Aug 2014</version>
	public interface IXMLSchemaNode : IMetaDataElement, IXaclObject
	{
		/// <summary>
		/// Gets or sets the XMLSchemaModel that owns this element
		/// </summary>
		XMLSchemaModel XMLSchemaView {get; set;}
          
		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of XMLSchemaNodeType values</value>
		XMLSchemaNodeType NodeType {get;}

        /// <summary>
        /// Gets or sets the data view element that is the parent element in expression tree
        /// </summary>
        /// <value>The data view element</value>
        IXMLSchemaNode ParentNode { get; set;}

		/// <summary>
		/// Gets or sets the information indicating whether the value of element
		/// is changed or not
		/// </summary>
		/// <value>true if it is changed, false otherwise.</value>
		bool IsValueChanged {get; set;}

		/// <summary>
		/// Accept a visitor of IXMLSchemaNodeVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		void Accept(IXMLSchemaNodeVisitor visitor);

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

        /// <summary>
        /// Create a Xml Schema types that have been referenced by the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaType(XMLSchemaModel xmlSchemaModel);

        /// <summary>
        /// Create a Xml Schema Element that represents the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaElement(XMLSchemaModel xmlSchemaModel);
	}
}
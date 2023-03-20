/*
* @(#)IDataViewElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a common interface for the elements in DataView name space.
	/// </summary>
	/// <version>  	1.0.0 28 Oct 2003</version>
	/// <author>  Yong Zhang </author>
	public interface IDataViewElement : IMetaDataElement, IXaclObject
	{
		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		DataViewModel DataView {get; set;}
          
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		ElementType ElementType {get;}

        /// <summary>
        /// Gets or sets the data view element that is the parent element in expression tree
        /// </summary>
        /// <value>The data view element</value>
        IDataViewElement ParentElement { get; set;}

		/// <summary>
		/// Gets or sets the information indicating whether the value of element
		/// is changed or not
		/// </summary>
		/// <value>true if it is changed, false otherwise.</value>
		bool IsValueChanged {get; set;}

		/// <summary>
		/// Gets or sets the information indicating whether the DataViewElement is
		/// readonly
		/// </summary>
		/// <value>true if it is read-only, false otherwise</value>
		bool IsReadOnly {get; set;}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		void Accept(IDataViewElementVisitor visitor);

		/// <summary>
		/// Gets the schema model element that the data view element associates with.
		/// </summary>
		/// <value>The SchemaModelElement.</value>
		SchemaModelElement GetSchemaModelElement();

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
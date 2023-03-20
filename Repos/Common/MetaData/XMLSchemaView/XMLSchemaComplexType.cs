/*
* @(#)XMLSchemaComplexType.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Xml;
	using System.Data;
	using System.ComponentModel;
	using System.Drawing.Design;
    using System.Xml.Schema;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A XMLSchemaComplexType is an object-representation of a specific xml schema view
	/// </summary>
	/// 
    /// <version>  	1.0.0 10 Aug 2014</version>
	public class XMLSchemaComplexType : XMLSchemaNodeBase, IXaclObject
	{
        private XMLSchemaElementCollection _elements;

		/// <summary>
		/// Initiating an instance of XMLSchemaComplexType class
		/// </summary>
		/// <param name="name">Name of the data view</param>
		public XMLSchemaComplexType(string name) : base(name)
		{
            _elements = new XMLSchemaElementCollection();
            _elements.ParentNode = this;
		}

		/// <summary>
		/// Initiating an instance of XMLSchemaComplexType class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XMLSchemaComplexType(XmlElement xmlElement) : base()
        {
            Unmarshal(xmlElement);
        }

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of XMLSchemaNodeType values</value>
		[BrowsableAttribute(false)]
        public override XMLSchemaNodeType NodeType 
		{
			get
			{
                return XMLSchemaNodeType.XMLSchemaComplexType;
			}
		}

        /// <summary>
        /// Gets or set the elements of the complex type
        /// </summary>
        /// <value>XMLSchemaElementCollection</value>
        [BrowsableAttribute(false)]
        public XMLSchemaElementCollection Elements
        {
            get
            {
                return _elements;
            }
            set
            {
                _elements = value;
            }
        }

		/// <summary>
		/// Accept a visitor of IXMLSchemaNodeVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IXMLSchemaNodeVisitor visitor)
		{
			visitor.VisitXMLSchemaComplexType(this);
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            // the first child is  a collection of xml schema elements
            _elements = (XMLSchemaElementCollection)XMLSchemaNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            _elements.ParentNode = this;
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            // write the _elements
            XmlElement child = parent.OwnerDocument.CreateElement(XMLSchemaNodeFactory.ConvertTypeToString(_elements.NodeType));
            _elements.Marshal(child);
            parent.AppendChild(child);
		}

        /// <summary>
        /// Create a Xml Schema types that have been referenced by the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaType(XMLSchemaModel xmlSchemaModel)
        {
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            XmlSchemaSequence sequence = new XmlSchemaSequence();
            complexType.Particle = sequence;

            // Add xml schema element to the complex type
            foreach (XMLSchemaElement xmlSchemaElement in _elements)
            {
                XmlSchemaElement schemaElement = (XmlSchemaElement)xmlSchemaElement.CreateXmlSchemaElement(xmlSchemaModel);
                schemaElement.MinOccursString = xmlSchemaElement.MinOccurs;
                schemaElement.MaxOccursString = xmlSchemaElement.MaxOccurs;

                sequence.Items.Add(schemaElement);
            }

            return complexType;
        }

        /// <summary>
        /// Create a Xml Schema Element that represents the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaElement(XMLSchemaModel xmlSchemaModel)
        {
            return null;
        }

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/" + this.Name;
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
		[BrowsableAttribute(false)]
        public override IXaclObject Parent
		{
			get
			{
				return _parentNode;
			}
		}

		/// <summary>
		/// Return a  of children of the Taxonomy node
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
		{
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

		#endregion
	}
}
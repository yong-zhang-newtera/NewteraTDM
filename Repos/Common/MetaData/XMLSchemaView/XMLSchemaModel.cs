/*
* @(#)XMLSchemaModel.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
    using System.IO;
	using System.Collections;
	using System.Collections.Specialized;
    using System.Collections.Generic;
	using System.Xml;
    using System.Xml.Schema;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A XMLSchemaModel is an object-representation of a specific xml schema view
	/// 
	/// A XMLSchemaModel can be constructed programatically or from an XML data. It can be
	/// saved as an XML data too.
	/// </summary>
	/// 
    /// <version>  	1.0.0 10 Aug 2014</version>
	public class XMLSchemaModel : XMLSchemaNodeBase, IXaclObject
	{
        private XMLSchemaElement _rootElement;
        private XMLSchemaComplexTypeCollection _complexTypes;
        private Hashtable _schemaTypesTable = new Hashtable();
        private string _postProcessor = null;

		/// <summary>
		/// Initiating an instance of XMLSchemaModel class
		/// </summary>
		/// <param name="name">Name of the data view</param>
		public XMLSchemaModel(string name) : base(name)
		{
			_xpath = null;
            _rootElement = null;
            _complexTypes = new XMLSchemaComplexTypeCollection();
            _complexTypes.ParentNode = this;
		}

		/// <summary>
		/// Initiating an instance of XMLSchemaModel class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XMLSchemaModel(XmlElement xmlElement) : base()
        {
            Unmarshal(xmlElement);

            _xpath = null;
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
                return XMLSchemaNodeType.XMLSchemaView;
			}
		}

        /// <summary>
        /// Gets or set the complex type for the root element
        /// </summary>
        /// <value>One of XMLSchemaNodeType values</value>
        [BrowsableAttribute(false)]
        public XMLSchemaElement RootElement
        {
            get
            {
                return _rootElement;
            }
            set
            {
                _rootElement = value;
            }
        }

        /// <summary>
        /// Gets or set a post processor definition.The post processor will be called on the data table
        /// generated based on the xml schema
        /// </summary>
        /// <value>In the format of ClassName,DLLName</value>
        [BrowsableAttribute(false)]
        public string PostProcessor
        {
            get
            {
                return _postProcessor;
            }
            set
            {
                _postProcessor = value;
            }
        }

        /// <summary>
        /// Gets or set the complex type for the xml schema model
        /// </summary>
        /// <value>XMLSchemaComplexTypeCollection</value>
        [BrowsableAttribute(false)]
        public XMLSchemaComplexTypeCollection ComplexTypes
        {
            get
            {
                return _complexTypes;
            }
            set
            {
                _complexTypes = value;
            }
        }

        /// <summary>
        /// Set a schema type in the table
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="schemaType"></param>
        public void SetSchemaType(string typeName, XmlSchemaAnnotated schemaType)
        {
            _schemaTypesTable[typeName] = schemaType;
        }

        /// <summary>
        /// Get a schema type from the table
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public XmlSchemaAnnotated GetSchemaType(string typeName)
        {
            return (XmlSchemaAnnotated)_schemaTypesTable[typeName];
        }

        public XMLSchemaElement GetXAxisElement()
        {
            XMLSchemaElement xAxisElement = null;

            foreach (XMLSchemaComplexType complexType in this.ComplexTypes)
            {
                foreach (XMLSchemaElement schemaElement in complexType.Elements)
                {
                    if (schemaElement.IsXAxis)
                    {
                        xAxisElement = schemaElement;
                        break;
                    }
                }

                if (xAxisElement != null)
                {
                    break;
                }
            }

            return xAxisElement;
        }

        public XMLSchemaElement GetCategoryAxisElement()
        {
            XMLSchemaElement cAxisElement = null;

            foreach (XMLSchemaComplexType complexType in this.ComplexTypes)
            {
                foreach (XMLSchemaElement schemaElement in complexType.Elements)
                {
                    if (schemaElement.IsCategoryAxis)
                    {
                        cAxisElement = schemaElement;
                        break;
                    }
                }

                if (cAxisElement != null)
                {
                    break;
                }
            }

            return cAxisElement;
        }

        public List<XMLSchemaElement> GetSimpleElements()
        {
            List<XMLSchemaElement> simpleElements = new List<XMLSchemaElement>();

            foreach (XMLSchemaComplexType complexType in this.ComplexTypes)
            {
                foreach (XMLSchemaElement schemaElement in complexType.Elements)
                {
                    if (this.ComplexTypes[schemaElement.Caption] == null)
                    {
                        // do not include complextype element
                        simpleElements.Add(schemaElement);
                    }
                }
            }

            return simpleElements;
        }

        /// <summary>
        /// Accept a visitor of IXMLSchemaNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXMLSchemaNodeVisitor visitor)
		{
			visitor.VisitXMLSchemaModel(this);
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            // the first child is for root element
            _rootElement = (XMLSchemaElement)XMLSchemaNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
            _rootElement.ParentNode = this;

            // then a collection of  complex types
            _complexTypes = (XMLSchemaComplexTypeCollection)XMLSchemaNodeFactory.Instance.Create((XmlElement)parent.ChildNodes[1]);
            _complexTypes.ParentNode = this;

            // set value of the description
            string text = parent.GetAttribute("postprocessor");
            if (!string.IsNullOrEmpty(text))
            {
                _postProcessor = text;
            }
        }

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            // write the _rootElement
            XmlElement child = parent.OwnerDocument.CreateElement(XMLSchemaNodeFactory.ConvertTypeToString(_rootElement.NodeType));
            _rootElement.Marshal(child);
            parent.AppendChild(child);

            // write the _complexTypes
            child = parent.OwnerDocument.CreateElement(XMLSchemaNodeFactory.ConvertTypeToString(_complexTypes.NodeType));
            _complexTypes.Marshal(child);
            parent.AppendChild(child);

            if (!string.IsNullOrEmpty(_postProcessor))
            {
                parent.SetAttribute("postprocessor", _postProcessor);
            }
        }

        /// <summary>
        /// Create a new XMLSchemaModel by cloning this XMLSchemaModel
        /// </summary>
        /// <returns>A cloned XMLSchemaModel</returns>
        public XMLSchemaModel Clone()
        {
            // use Marshal and Unmarshal to clone a XMLSchemaModel
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("SchemaViews");
            doc.AppendChild(root);
            XmlElement child = doc.CreateElement(XMLSchemaNodeFactory.ConvertTypeToString(this.NodeType));
            this.Marshal(child);
            root.AppendChild(child);

            // create a new XMLSchemaModel and unmarshal from the xml element as source
            XMLSchemaModel newSchemaModel = new XMLSchemaModel(child);

            return newSchemaModel;
        }

        /// <summary>
        /// Return xpath strings for all leaf nodes in the xml schema model
        /// </summary>
        /// <returns>xpath strings of all leaf nodes</returns>
        public List<string> GetAllLeafNodeXPath()
        {
            List<string> allLeafNodeXParhs = new List<string>();

            string rootElementPath = @"/" + RootElement.Caption;
            bool isRepeatNode = false;
            if (RootElement.MaxOccurs == "unbounded")
            {
                isRepeatNode = true;
            }

            XMLSchemaComplexType complexType = (XMLSchemaComplexType) ComplexTypes[RootElement.Caption];
            if (complexType != null)
            {
                AddComplexTypeLeafNodeXPaths(allLeafNodeXParhs, rootElementPath, complexType, isRepeatNode);
            }

            return allLeafNodeXParhs;
        }

        /// <summary>
        /// Write the schema as a XML Schema to a file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        public void Write(string fileName)
        {
            try
            {
                //Open the stream and read XSD from it.
                using (FileStream fs = File.Open(fileName, FileMode.Create))
                {
                    Write(fs);
                    fs.Flush();
                }
            }
            catch (System.IO.IOException ex)
            {
                throw new ReadSchemaException("XMLSchemaModel FailedToWriteFile" + fileName, ex);
            }
        }

        /// <summary>
        /// Write the schema as a XML Schema to a Stream.
        /// </summary>
        /// <param name="stream">the stream object to which to write a XML schema
        /// </param>
        /// 
        /// <exception cref="SchemaModelException"> SchemaModelException is thrown when it fails to write
        /// the XML Schema file
        /// </exception>
        public void Write(Stream stream)
        {
            try
            {
                XmlSchema xmlSchema = GetXmlSchema();

                xmlSchema.Write(stream);
            }
            catch (System.IO.IOException ex)
            {
                throw new WriteSchemaException("XMLSchemaModel FailedToWriteModel", ex);
            }
        }

        /// <summary>
        /// Write the schema as a XML Schema to a TextWriter.
        /// </summary>
        /// <param name="writer">the TextWriter instance to which to write a XML schema
        /// </param>
        /// <exception cref="SchemaModelException"> SchemaModelException is thrown when it fails to write
        /// the XML Schema file
        /// </exception>
        public void Write(TextWriter writer)
        {
            try
            {
                XmlSchema xmlSchema = GetXmlSchema();

                xmlSchema.Write(writer);
            }
            catch (System.IO.IOException ex)
            {
                throw new WriteSchemaException("XMLSchemaModel FailedToWriteModel", ex);
            }
        }

        /// <summary>
        /// Gets the xml schema that represents the schema model
        /// </summary>
        /// <returns>A XmlSchema instance</returns>
        public XmlSchema GetXmlSchema()
        {
            // Marshal the objects to xml schema
            XmlSchema xmlSchema = new XmlSchema();

            BuildXMLSchema(xmlSchema);

            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(xmlSchema);
            set.Compile();

            return xmlSchema;
        }
 
        /// <summary>
        /// Write objects to XML Schema Model
        /// </summary>
        /// <param name="xmlSchema">The xml schema model</param>
        private void BuildXMLSchema(XmlSchema xmlSchema)
        {
            if (_rootElement.MaxOccurs != "unbounded")
            {
                // Root element occures once, use root element as the root of the XML Schema
                xmlSchema.Items.Add(_rootElement.CreateXmlSchemaElement(this));
            }
            else
            {
                // Root element occures multiple times, create a pseudo-element as a root element
                xmlSchema.Items.Add(this.CreateXmlSchemaElement(this));
            }
        }

        /// <summary>
        /// Create a Xml Schema types that have been referenced by the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaType(XMLSchemaModel xmlSchemaModel)
        {
            return null;
        }

        /// <summary>
        /// Create a Xml Schema Element as a pseudo-element as a root element
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaElement(XMLSchemaModel xmlSchemaModel)
        {
            XmlSchemaElement xmlSchemaElement = null;

            if (this._rootElement.MaxOccurs != "1" &&
                IsComplexType(xmlSchemaModel))
            {
                xmlSchemaElement = new XmlSchemaElement();
                xmlSchemaElement.Name = EsacapeXMLElementName(Caption);

                // it's type is one of the complex type names
                XMLSchemaComplexType complexType = (XMLSchemaComplexType)xmlSchemaModel.ComplexTypes[xmlSchemaModel.Caption];
                complexType.Elements.Add(_rootElement);
                xmlSchemaElement.SchemaType = (XmlSchemaComplexType)complexType.CreateXmlSchemaType(xmlSchemaModel);
            }
            else
            {
                throw new Exception("The root element's max occurs isn't defined as unbounded.");
            }

            return xmlSchemaElement;
        }

        /// <summary>
        /// Add xapths of leaf nodes of a complex type into a list. This method
        /// can be called recursively.
        /// </summary>
        /// <param name="allLeafNodeXParhs">A list of leaf node xpaths</param>
        /// <param name="parentPath">The xpath of the parent node</param>
        /// <param name="complexType">The complex type whose nodes to be traveled</param>
        /// <param name="isRepeatingNode">true indicates the xapth represents a repeating node</param>
        private void AddComplexTypeLeafNodeXPaths(List<string> allLeafNodeXParhs, string parentPath, XMLSchemaComplexType complexType, bool isRepeatingNode)
        {
            XMLSchemaComplexType childComplexType;
            string xpath;
            bool isRepeatingNodeTemp = isRepeatingNode;

            foreach (XMLSchemaElement schemaElement in complexType.Elements)
            {
                childComplexType = (XMLSchemaComplexType)ComplexTypes[schemaElement.Caption];
                xpath = parentPath + @"/" + schemaElement.Caption;

                // both caption and name have to be the same
                if (childComplexType != null &&
                    childComplexType.Name != schemaElement.Name)
                {
                    childComplexType = null;
                }

                if (childComplexType == null)
                {
                    if (isRepeatingNode)
                    {
                        // non-repeating node can be placed in xml list to become a repeating node
                        // therefore, commen it out
                        //xpath = "r:" + xpath; // to indicate the xpath represents a repeating node
                    }
                    // it is a leaf node, add the xapth
                    allLeafNodeXParhs.Add(xpath);
                }
                else if (childComplexType != null)
                {
                    if (!isRepeatingNodeTemp && schemaElement.MaxOccurs == "unbounded")
                    {
                        isRepeatingNodeTemp = true;
                    }

                    // call the method recursively to add xpaths of the leaf nodes in the child complex type
                    AddComplexTypeLeafNodeXPaths(allLeafNodeXParhs, xpath, childComplexType, isRepeatingNodeTemp);
                }
            }
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
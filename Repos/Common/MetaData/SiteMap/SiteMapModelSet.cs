/*
* @(#)SiteMapModelSet.cs
*
* Copyright (c) 2012 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.SiteMap.Validate;

	/// <summary>
	/// Represents a set of SiteMap Models, allowing multiple sitemaps exist at the same time. Usually
    /// each sitemap represents a different application. There is only one sitemap model is active at
    /// any time, indicating by CurrentSiteMap property.
	/// </summary>
	/// <version> 1.0.0 16 Jul 2012 </version>
    public class SiteMapModelSet : ISiteMapNode, ICustomTypeDescriptor
	{
        private string _currentModelName;
        private SiteMapModel _currentSiteMapModel;

        private PropertyDescriptorCollection _globalizedProperties = null;

        /// <summary>
        /// Title changed handler
        /// </summary>
        public event EventHandler TitleChanged;

        private SiteMapNodeCollection _models = new SiteMapNodeCollection();

		/// <summary>
		///  Initializes a new instance of the SiteMapModelSet class.
		/// </summary>
		public SiteMapModelSet() : base()
		{
            _currentModelName = null;
            _currentSiteMapModel = null;
		}

		/// <summary>
		/// Initiating an instance of SiteMapModelSet class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SiteMapModelSet(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
            _currentSiteMapModel = null;
		}

        /// <summary>
        /// Gets or sets the currently used sitemap name
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Specify the name of a site map model to be the currently used sitemap model."),
            TypeConverterAttribute("Newtera.SiteMapStudio.CurrentSiteMapNameConverter, SiteMapStudio"),
            EditorAttribute("Newtera.SiteMapStudio.CurrentSiteMapNamePropertyEditor, SiteMapStudio", typeof(UITypeEditor))
        ]
        public string CurrentSiteMapName
        {
            get
            {
                if (!string.IsNullOrEmpty(_currentModelName))
                {
                    return _currentModelName;
                }
                else
                {
                    return SiteMapModel.DEFAULT_MODEL_NAME;
                }
            }
            set
            {
                _currentModelName = value;
                _currentSiteMapModel = null;
            }
        }

        /// <summary>
        /// Gets the current site map model
        /// </summary>
        [BrowsableAttribute(false)]	
        public SiteMapModel CurrentSiteMapModel
        {
            get
            {
                if (_currentSiteMapModel == null)
                {
                    if (!string.IsNullOrEmpty(_currentModelName))
                    {
                        _currentSiteMapModel = FindSiteMapModel(_currentModelName);
                        if (_currentSiteMapModel == null)
                        {
                            throw new Exception("Uanble to find a site map model with name " + _currentModelName);
                        }
                    }
                    else
                    {
                        // get the default sitemap model
                        _currentSiteMapModel = FindSiteMapModel(SiteMapModel.DEFAULT_MODEL_NAME);
                        if (_currentSiteMapModel == null)
                        {
                            throw new Exception("Uanble to find a site map model with name " + SiteMapModel.DEFAULT_MODEL_NAME);
                        }
                    }
                }

                return _currentSiteMapModel;
            }
        }

        /// <summary>
        /// Find a SiteMapModel of a given name
        /// </summary>
        /// <param name="name">The model name</param>
        /// <returns>The SiteMapModel object</returns>
        public SiteMapModel FindSiteMapModel(string name)
        {
            SiteMapModel found = null;

            if (_models != null)
            {
                foreach (SiteMapModel model in this._models)
                {
                    if (model.Name == name)
                    {
                        found = model;
                        break;
                    }
                }
            }

            return found;
        }

		/// <summary>
		/// Construct a SiteMapModelSet object from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="SiteMapException">SiteMapException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.OpenRead(fileName)) 
				{
					Read(fs);					
				}
			}
			catch (Exception ex)
			{
                throw new SiteMapException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
        /// Construct a SiteMapModelSet object from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="SiteMapException">SiteMapException is thrown when it fails to
		/// read the stream
		/// </exception>
		public void Read(Stream stream)
		{
			if (stream != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(stream);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new SiteMapException(e.Message, e);
				}
			}
		}

		/// <summary>
        /// Construct a SiteMapModelSet object from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="SiteMapException">SiteMapException is thrown when it fails to
		/// read the text reader
		/// </exception>
		public void Read(TextReader reader)
		{
			if (reader != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(reader);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new SiteMapException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write the SiteMapModelSet object to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="SiteMapException">SiteMapException is thrown when it fails to
		/// write to the file.
		/// </exception> 
		public void Write(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.OpenWrite(fileName)) 
				{
					Write(fs);
					fs.Flush();
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new SiteMapException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
        /// Write the SiteMapModelSet object as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="SiteMapException">SiteMapException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException ex)
			{
				throw new SiteMapException("Failed to write the SiteMapModelSet object", ex);
			}
		}

		/// <summary>
        /// Write the SiteMapModelSet object as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="SiteMapException">SiteMapException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException ex)
			{
				throw new SiteMapException("Failed to write the SiteMapModelSet object", ex);
			}
		}

		#region ISiteMapNode members

        [BrowsableAttribute(false)]	
        public string Name
        {
            get
            {
                return "SiteMapModelSet";
            }
            set
            {
            }
        }

        [BrowsableAttribute(false)]	
        public string Title
        {
            get
            {
                return "SiteMapModelSet";
            }
            set
            {
            }
        }

        [BrowsableAttribute(false)]	
        public string Description
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the parent node
        /// </summary>
        [BrowsableAttribute(false)]	
        public ISiteMapNode ParentNode
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        /// <value>A SiteMapNodeCollection</value>
        [BrowsableAttribute(false)]	
        public SiteMapNodeCollection ChildNodes
        {
            get
            {
                return _models;
            }
        }

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
        [BrowsableAttribute(false)]	
		public NodeType NodeType	{
			get
			{
				return NodeType.SiteMapModelSet;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitSiteMapModelSet(this))
            {
                foreach (ISiteMapNode child in ChildNodes)
                {
                    child.Accept(visitor);
                }
            }
        }

        /// <summary>
        /// Add a node as a child
        /// </summary>
        /// <param name="child"></param>
        public void AddChildNode(ISiteMapNode child)
        {
            if (this.ChildNodes != null)
            {
                this.ChildNodes.Add(child);
                child.ParentNode = this;
            }
        }

        /// <summary>
        /// Delete a child node
        /// </summary>
        /// <param name="child"></param>
        public void DeleteChildNode(ISiteMapNode child)
        {
            if (this.ChildNodes != null)
            {
                this.ChildNodes.Remove(child);
                child.ParentNode = null;
            }
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        public virtual void Unmarshal(XmlElement parent)
        {
            if (parent.GetAttribute("sitemap") != null)
            {
                _currentModelName = parent.GetAttribute("sitemap");
            }

            foreach (XmlElement xmlElement in parent.ChildNodes)
            {
                ISiteMapNode element = NodeFactory.Instance.Create(xmlElement);
                element.ParentNode = this;
                _models.Add(element);
            }
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public virtual void Marshal(XmlElement parent)
        {
            if (!string.IsNullOrEmpty(_currentModelName))
            {
                parent.SetAttribute("sitemap", _currentModelName);
            }

            XmlElement child;

            foreach (ISiteMapNode node in _models)
            {
                child = parent.OwnerDocument.CreateElement(NodeFactory.Instance.ConvertTypeToString(node.NodeType));
                node.Marshal(child);
                parent.AppendChild(child);
            }
        }

        /// <summary>
        /// Validate all site map model to see if it confirm to schema model integrity
        /// rules.
        /// </summary>
        /// <returns>The result in ValidateResult object</returns>
        public ValidateResult Validate()
        {
            ValidateResult validateResult = null;

            foreach (SiteMapModel siteMapModel in this.ChildNodes)
            {
                validateResult = siteMapModel.Validate();
                if (validateResult.HasError)
                {
                    break;
                }
            }

            return validateResult;
        }

		#endregion

		/// <summary>
		/// Gets the xml document that represents the custom command set
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
            XmlDocument doc = new XmlDocument();
            XmlDeclaration nodeDec = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            doc.AppendChild(nodeDec);   

			XmlElement element = doc.CreateElement(NodeFactory.Instance.ConvertTypeToString(NodeType));

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

        /// <summary>
        /// Return a displayed title path representation of the ISiteMapNode
        /// </summary>
        /// <returns>a title path representation</returns>
        public virtual string ToDisplayPath()
        {
            return "/SiteMapModelSet";
        }

        /// <summary>
        /// Gets the node 's unique hash code
        /// </summary>
        /// <returns>A hashcode</returns>
        public virtual int GetNodeHashCode()
        {
            return ToXPath().GetHashCode();
        }

        #region IXaclObject

        /// <summary>
        /// Return a xpath representation of the object
        /// </summary>
        /// <returns>a xapth representation</returns>
        public virtual string ToXPath()
        {
            return "/SiteMapModelSet";
        }

        /// <summary>
        /// Return a  parent of the object
        /// </summary>
        /// <returns>The parent of the object</returns>
        [BrowsableAttribute(false)]
        public IXaclObject Parent
        {
            get
            {
                return (IXaclObject)this.ParentNode;
            }
        }

        /// <summary>
        /// Return a  of children of the object
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public IEnumerator GetChildren()
        {
            return this.ChildNodes.GetEnumerator();
        }

        #endregion

        #region ICustomTypeDescriptor

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// Called to get the properties of a type.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to be globalized
                foreach (PropertyDescriptor property in baseProps)
                {
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Our implementation overrides GetProperties() only and creates a
        /// collection of custom property descriptors of type GlobalizedPropertyDescriptor
        /// and returns them to the caller instead of the default ones.
        /// </summary>
        /// <returns>A collection of Property Descriptors.</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            // Only do once
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProperties = TypeDescriptor.GetProperties(this, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to 
                // be globalized
                foreach (PropertyDescriptor property in baseProperties)
                {
                    // create our custom property descriptor and add it to the collection
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
	}
}
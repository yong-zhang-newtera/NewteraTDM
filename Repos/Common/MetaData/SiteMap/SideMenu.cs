/*
* @(#)SideMenu.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
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

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a sitemap object
	/// </summary>
	/// <version> 1.0.0 14 Jun 2009 </version>
	public class SideMenu : ISiteMapNode
	{
        private ISiteMapNode _parent;
        private SiteMapNodeCollection _groups = new SiteMapNodeCollection();

        /// Title changed handler
        /// </summary>
        public event EventHandler TitleChanged;

		/// <summary>
		///  Initializes a new instance of the SideMenu class.
		/// </summary>
		public SideMenu() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of SideMenu class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SideMenu(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets the SideMenuGroup of the specified path
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The SideMenuGroup object, null if no exist</returns>
        public ISiteMapNode FindMenuGroup(string path)
        {
            ISiteMapNode found = null;

            foreach (SideMenuGroup group in _groups)
            {
                if (group.ToPath() == path)
                {
                    found = group;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Gets the ISiteMapNode of the specified path
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The ISiteMapNode object, null if no exist</returns>
        public ISiteMapNode FindNode(string path)
        {
            return FindNode(this, path);
        }

        private ISiteMapNode FindNode(ISiteMapNode parentNode, string path)
        {
            ISiteMapNode found = null;

            foreach (ISiteMapNode child in parentNode.ChildNodes)
            {
                if (child.ToXPath() == path)
                {
                    found = child;
                    break;
                }
                else
                {
                    found = FindNode(child, path);
                    if (found != null)
                    {
                        break;
                    }
                }
            }

            return found;
        }

		/// <summary>
		/// Constrauct a attachment info collection from an XML file.
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
		/// Constrauct a attachment info from an stream.
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
		/// Constrauct a attachment info from a text reader.
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
		/// Write the attachment info to an XML file.
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
		/// Write the data view as a XML data to a Stream.
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
				throw new SiteMapException("Failed to write the SideMenu object", ex);
			}
		}

		/// <summary>
		/// Write the data view as a XML data to a TextWriter.
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
				throw new SiteMapException("Failed to write the SideMenu object", ex);
			}
		}

		#region ISiteMapNode members

        public string Name
        {
            get
            {
                return "SideMenu";
            }
            set
            {
            }
        }

        public string Title
        {
            get
            {
                return "SideMenu";
            }
            set
            {
            }
        }

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
        public ISiteMapNode ParentNode
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        /// <value>A SiteMapNodeCollection</value>
        public SiteMapNodeCollection ChildNodes
        {
            get
            {
                return _groups;
            }
        }

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public NodeType NodeType	{
			get
			{
				return NodeType.SideMenu;
			}
		}

        /// <summary>
        /// Accept a visitor of ISiteMapNodeVisitor type to traverse itself and its
        /// children nodes.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public void Accept(ISiteMapNodeVisitor visitor)
        {
            if (visitor.VisitSideMenu(this))
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
            foreach (XmlElement xmlElement in parent.ChildNodes)
            {
                ISiteMapNode element = NodeFactory.Instance.Create(xmlElement);
                element.ParentNode = this;
                _groups.Add(element);
            }
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        public virtual void Marshal(XmlElement parent)
        {
            XmlElement child;

            foreach (ISiteMapNode node in _groups)
            {
                child = parent.OwnerDocument.CreateElement(NodeFactory.Instance.ConvertTypeToString(node.NodeType));
                node.Marshal(child);
                parent.AppendChild(child);
            }
        }

		#endregion

		/// <summary>
		/// Gets the xml document that represents the sitemap
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
            if (ParentNode != null)
            {
                return ParentNode.ToDisplayPath() + "/SideMenu";
            }
            else
            {
                return "/Model/SideMenu";
            }
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
            if (ParentNode != null)
            {
                return ParentNode.ToXPath() + "/SideMenu";
            }
            else
            {
                return "/Model/SideMenu";
            }
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
	}
}
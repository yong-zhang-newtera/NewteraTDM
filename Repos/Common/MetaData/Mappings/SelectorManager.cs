/*
* @(#)SelectorManager.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// This is the top level class that manages all Selectors for a
	/// schema. It also provides methods to allow easy accesses, addition, and 
	/// deletion of Selectors.
	/// </summary>
	/// <version> 1.0.0 09 Jan 2005 </version>
	/// <author> Yong Zhang </author>
	public class SelectorManager : MappingNodeBase, IXaclObject
	{
		private MetaDataModel _metaData; // run-time only
		private string _xpath; // run-time only
		private bool _isAltered; // run-time only
		
		private SelectorCollection _selectors;

		/// <summary>
		/// Initiate an instance of SelectorManager class
		/// </summary>
		public SelectorManager(): base()
		{
			_isAltered = false;
			_metaData = null;
			_xpath = null;
			_selectors = new SelectorCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _selectors.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// Gets or sets the information indicating whether selector has been
		/// altered.
		/// </summary>
		/// <value>true if it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether it is an empty selector collection
		/// </summary>
		/// <value>true if it is an empty selector collection, false otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				if (this._selectors.Count == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets a collection of Selector instances held by the manager
		/// </summary>
		public SelectorCollection Selectors
		{
			get
			{
				return _selectors;
			}
		}

		/// <summary>
		/// Gets or sets the meta data that owns the SelectorManager
		/// </summary>
		/// <returns> A MetaDataModel object</returns>
		public MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
			}
		}
		
		/// <summary>
		/// Gets a selector of given name
		/// </summary>
		/// <param name="name">The selector name</param>
		/// <returns>A Selector instance, null if not found.</returns>
		public Selector GetSelector(string name)
		{
			Selector found = null;

			foreach (Selector selector in _selectors)
			{
				if (selector.Name == name)
				{
					found = selector;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// Gets a collection selectors for a given destination class.
		/// </summary>
		/// <param name="dstClassName">The destination class name.</param>
		/// <returns>A SelectorCollection instance.</returns>
		public SelectorCollection GetSelectors(string dstClassName)
		{
			SelectorCollection selectors = new SelectorCollection();

			foreach (Selector selector in _selectors)
			{
				if (selector.DestinationClassName == dstClassName)
				{
					selectors.Add(selector);
				}
			}

			return selectors;
		}

		/// <summary>
		/// Gets the information indicating whether a selector of the given name has
		/// existed.
		/// </summary>
		/// <param name="name">The selector name</param>
		/// <returns>true if it exists, false otherwise.</returns>
		public bool IsSelectorExist(string name)
		{
			bool found = false;

			foreach (Selector selector in _selectors)
			{
				if (selector.Name == name)
				{
					found = true;
					break;
				}
			}

			return found;
		}


		/// <summary>
		/// Add a selector to the collection
		/// </summary>
		/// <param name="selector">The Selector to be added</param>
		public void AddSelector(Selector selector)
		{	
			selector.SelectorManager = this;
			_selectors.Add(selector);
		}

		/// <summary>
		/// Remove a selector from the collection.
		/// </summary>
		/// <param name="selector">The selector instance to be removed</param>
		public void RemoveSelector(Selector selector)
		{
			_selectors.Remove(selector);
		}

		/// <summary>
		/// Read selectors from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				if (File.Exists(fileName))
				{
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						Read(fs);					
					}
				}
			}
			catch (Exception ex)
			{
                throw new MappingException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Read selectors from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// read the stream.</exception>
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
					throw new MappingException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Read selectors from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// read the text reader</exception>
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
					throw new MappingException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write selectors to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// write to the file.</exception> 
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
				throw new MappingException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write selectors as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (Exception ex)
			{
				throw new MappingException("Failed to write the selectors", ex);
			}
		}

		/// <summary>
		/// Write selectors as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (Exception ex)
			{
				throw new MappingException("Failed to write the selectors", ex);
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType 
		{
			get
			{
				return NodeType.SelectorManager;
			}
		}

		/// <summary>
		/// create selectors from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of Selector instances
			_selectors = (SelectorCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			// set SelectorManager
			foreach (Selector selector in _selectors)
			{
				selector.SelectorManager = this;
			}
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _selectors.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write selectors to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the rule defs
			XmlElement child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_selectors.NodeType));
			_selectors.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an selectors
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("SelectorManager");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// A handler to call when a value of the selectors changed
		/// </summary>
		/// <param name="sender">the IXaclNode that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the SelectorManager node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/selector";
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the SelectorManager node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
		public IXaclObject Parent
		{
			get
			{
				return _metaData;
			}
		}

		/// <summary>
		/// Return a  of children of the Taxonomy node
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
		public IEnumerator GetChildren()
		{
			return this._selectors.GetEnumerator();
		}

		#endregion
	}
}
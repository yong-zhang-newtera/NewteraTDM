/*
* @(#)XMLSchemaModelCollection.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
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
	/// Represents a collection of XMLSchemaModel objects.
	/// </summary>
    /// <version>  	1.0.0 10 Aug 2014</version>
	public class XMLSchemaModelCollection : XMLSchemaNodeCollection
	{
		private bool _isAltered;
		private MetaDataModel _metaData; // run-time only

		/// <summary>
		/// Initiating an instance of XMLSchemaModelCollection class
		/// </summary>
		public XMLSchemaModelCollection() : base()
		{
			_isAltered = false;
			_xpath = null;
			_metaData = null;

            if (GlobalSettings.Instance.IsWindowClient)
            {
                // listen to the value changed event from the data views
                this.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// Gets or sets the information indicating whether the data views has been altered
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
		/// Gets the type of node
		/// </summary>
        /// <value>One of XMLSchemaNodeType values</value>
        public override XMLSchemaNodeType NodeType 
		{
			get
			{
                return XMLSchemaNodeType.XMLSchemaViews;
			}
		}

        /// <summary>
		/// Adds a XMLSchemaModel to the XMLSchemaModelCollection.
		/// </summary>
		/// <param name="value">the XMLSchemaModel to be added</param>
		/// <returns>The position into which the new element was added</returns>
        public override int Add(IXMLSchemaNode value)
        {
            int pos = 0;
            if (value is XMLSchemaModel)
            {
                pos = base.Add(value);
                ((XMLSchemaModel)value).ParentNode = this;
            }
            else
            {
                throw new ArgumentException("value must be of type XMLSchemaModel.", "value");
            }

            return pos;
        }

        /// <summary>
		/// inserts a XMLSchemaModel to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The XMLSchemaModel to insert into collection</param>
        public override void Insert(int index, IXMLSchemaNode value)
        {
            if (value is XMLSchemaModel)
            {
                base.Insert(index, value);
                ((XMLSchemaModel)value).ParentNode = this;
            }
            else
            {
                throw new ArgumentException("value must be of type XMLSchemaModel.", "value");
            }
        }

		/// <summary>
		/// Accept a visitor of IXMLSchemaNodeVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IXMLSchemaNodeVisitor visitor)
		{
			foreach (IXMLSchemaNode element in List)
			{
				element.Accept(visitor);
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
        /// Create a Xml Schema Element that represents the XMLSchema node.
        /// The method must be override by the subclass.
        /// </summary>
        /// <returns>The created XmlSchemaAnnotated object</returns>
        public override System.Xml.Schema.XmlSchemaAnnotated CreateXmlSchemaElement(XMLSchemaModel xmlSchemaModel)
        {
            return null;
        }

		/// <summary>
		/// Constrauct a xml schema view model collection from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="XMLSchemaViewException">XMLSchemaViewException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					//Open the stream and read XSD from it.
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						Read(fs);					
					}
				}
			}
			catch (Exception ex)
			{
                throw new XMLSchemaViewException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a data view model from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="XMLSchemaViewException">XMLSchemaViewException is thrown when it fails to
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
					throw new XMLSchemaViewException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a data view model from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="XMLSchemaViewException">XMLSchemaViewException is thrown when it fails to
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
					throw new XMLSchemaViewException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write the data view model to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="XMLSchemaViewException">XMLSchemaViewException is thrown when it fails to
		/// write to the file.
		/// </exception> 
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
				throw new XMLSchemaViewException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write the data view as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="XMLSchemaViewException">XMLSchemaViewException is thrown when it fails to
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
				throw new XMLSchemaViewException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Write the data view as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="XMLSchemaViewException">XMLSchemaViewException is thrown when it fails to
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
				throw new XMLSchemaViewException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Unmarshal an element representing data view collection
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			this.List.Clear();

			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				XMLSchemaModel xmlSchemaView = (XMLSchemaModel) XMLSchemaNodeFactory.Instance.Create(xmlElement);

				this.Add(xmlSchemaView);
			}
		}

		/// <summary>
		/// Gets the xml document that represents the data view
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("SchemaViews");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// A handler to call when a value of the data views changed
		/// </summary>
		/// <param name="sender">the IXMLSchemaNode that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
            if (_xpath == null && this.Parent != null)
			{
				_xpath = this.Parent.ToXPath() + "/schemaview";
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
        public override IXaclObject Parent
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
        public override IEnumerator GetChildren()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}
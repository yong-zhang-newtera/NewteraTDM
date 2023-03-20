/*
* @(#)NewteraTrackingWorkflowInstanceCollection.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.WFModel
{
	using System;
	using System.Text;
	using System.Xml;
    using System.IO;

	/// <summary>
    /// Represents a collection of NewteraTrackingWorkflowInstance instances.
	/// </summary>
	/// <version>1.0.0 3 Jan 2007</version>
	/// <author>Yong Zhang</author>
	public class NewteraTrackingWorkflowInstanceCollection : WFModelElementCollection
	{
		/// <summary>
		/// Initiating an instance of NewteraTrackingWorkflowInstanceCollection class
		/// </summary>
		public NewteraTrackingWorkflowInstanceCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of NewteraTrackingWorkflowInstanceCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal NewteraTrackingWorkflowInstanceCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.TrackingInstances;
			}
		}

		/// <summary>
        /// Accept a visitor of IWFModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
            if (visitor.VisitTrackingWorkflowInstanceCollection(this))
			{
				foreach (IWFModelElement element in List)
				{
					element.Accept(visitor);
				}
			}
		}

        /// <summary>
        /// Load data from a xml string
        /// </summary>
        /// <param name="xmlString">The xml string represents the collection</param>
        public void Load(string xmlString)
        {
            // read the xml string for schema
            StringReader reader = new StringReader(xmlString);
            this.Read(reader);
        }

        /// <summary>
        /// Constrauct a NewteraTrackingWorkflowInstanceCollection from an XML file.
        /// </summary>
        /// <param name="fileName">the name of the XML file</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
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
                throw new WFModelException("Failed to read the file :" + fileName, ex);
            }
        }

        /// <summary>
        /// Constrauct a NewteraTrackingWorkflowInstanceCollection from an stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
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
                    throw new WFModelException(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Constrauct a NewteraTrackingWorkflowInstanceCollection from a text reader.
        /// </summary>
        /// <param name="reader">the text reader</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
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
                    throw new WFModelException(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Write the NewteraTrackingWorkflowInstanceCollection to an XML file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
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
                throw new WFModelException("Failed to write to file :" + fileName, ex);
            }
        }

        /// <summary>
        /// Write the NewteraTrackingWorkflowInstanceCollection as a XML data to a Stream.
        /// </summary>
        /// <param name="stream">the stream object to which to write a XML data</param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
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
                throw new WFModelException("Failed to write the SchemaModel object", ex);
            }
        }

        /// <summary>
        /// Write the data view as a XML data to a TextWriter.
        /// </summary>
        /// <param name="writer">the TextWriter instance to which to write a XML schema
        /// </param>
        /// <exception cref="WFModelException">WFModelException is thrown when it fails to
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
                throw new WFModelException("Failed to write the SchemaModel object", ex);
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
                NewteraTrackingWorkflowInstance trackingInstance = (NewteraTrackingWorkflowInstance)ElementFactory.Instance.Create(xmlElement);

                this.Add(trackingInstance);
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

            XmlElement element = doc.CreateElement("TrackingInstances");

            doc.AppendChild(element);

            Marshal(element);

            return doc;
        }
	}
}
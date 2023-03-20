/*
* @(#)WorkflowInstanceStateInfoCollection.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.WFModel
{
	using System;
	using System.Text;
	using System.Xml;
    using System.IO;
    using System.Collections;

	/// <summary>
    /// Represents a collection of WorkflowInstanceStateInfo instances.
	/// </summary>
	/// <version>1.0.0 28 Dec 2008</version>
    public class WorkflowInstanceStateInfoCollection : CollectionBase
	{
		/// <summary>
		/// Initiating an instance of WorkflowInstanceStateInfoCollection class
		/// </summary>
		public WorkflowInstanceStateInfoCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of WorkflowInstanceStateInfoCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal WorkflowInstanceStateInfoCollection(XmlElement xmlElement) : base()
		{
		}

        /// <summary>
        /// Implemention of Indexer member using attribute index
        /// </summary>
        public WorkflowInstanceStateInfo this[int index]
        {
            get
            {
                return ((WorkflowInstanceStateInfo)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Adds an WorkflowInstanceStateInfo to the Collection.
        /// </summary>
        /// <param name="value">the object to be added</param>
        /// <returns>The position into which the new element was inserted</returns>
        public void Add(WorkflowInstanceStateInfo value)
        {
            // append at the end
            List.Add(value);
        }

        /// <summary>
        /// removes the first occurrence of a specific object from the collection
        /// </summary>
        /// <param name="value">The Object to remove from the collection.</param>
        public void Remove(WorkflowInstanceStateInfo value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// determines whether the collection contains a specific value
        /// </summary>
        /// <param name="value">The Object to locate in the collection.</param>
        /// <returns>true if the Object is found in the List; otherwise, false.</returns>
        public bool Contains(WorkflowInstanceStateInfo value)
        {
            // If value is not of type WorkflowInstanceStateInfo, this will return false.
            return (List.Contains(value));
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
        /// Constrauct a WorkflowInstanceStateInfoCollection from an XML file.
        /// </summary>
        /// <param name="fileName">the name of the XML file</param>
        public void Read(string fileName)
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

        /// <summary>
        /// Constrauct a WorkflowInstanceStateInfoCollection from an stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        public void Read(Stream stream)
        {
            if (stream != null)
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(stream);

                // Initializing the objects from the xml document
                Unmarshal(doc.DocumentElement);
            }
        }

        /// <summary>
        /// Constrauct a WorkflowInstanceStateInfoCollection from a text reader.
        /// </summary>
        /// <param name="reader">the text reader</param>
        public void Read(TextReader reader)
        {
            if (reader != null)
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(reader);

                // Initializing the objects from the xml document
                Unmarshal(doc.DocumentElement);
            }
        }

        /// <summary>
        /// Write the WorkflowInstanceStateInfoCollection to an XML file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        public void Write(string fileName)
        {
            //Open the stream and read XSD from it.
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            {
                Write(fs);
                fs.Flush();
            }
        }

        /// <summary>
        /// Write the WorkflowInstanceStateInfoCollection as a XML data to a Stream.
        /// </summary>
        /// <param name="stream">the stream object to which to write a XML data</param>
        public void Write(Stream stream)
        {
            XmlDocument doc = GetXmlDocument();

            doc.Save(stream);
        }

        /// <summary>
        /// Write the data view as a XML data to a TextWriter.
        /// </summary>
        /// <param name="writer">the TextWriter instance to which to write a XML schema
        /// </param>
        public void Write(TextWriter writer)
        {
            XmlDocument doc = GetXmlDocument();

            doc.Save(writer);
        }

        /// <summary>
        /// Unmarshal an element representing data view collection
        /// </summary>
        /// <param name="parent">An xml element</param>
        public void Unmarshal(XmlElement parent)
        {
            this.List.Clear();

            foreach (XmlElement xmlElement in parent.ChildNodes)
            {
                WorkflowInstanceStateInfo workflowInstanceStateInfo = new WorkflowInstanceStateInfo(xmlElement);

                this.List.Add(workflowInstanceStateInfo);
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

            XmlElement parent = doc.CreateElement("WFInstanceStateInfos");

            doc.AppendChild(parent);

            XmlElement child;

            foreach (WorkflowInstanceStateInfo workflowInstanceStateInfo in List)
            {
                child = parent.OwnerDocument.CreateElement("WFInstanceStateInfo");
                workflowInstanceStateInfo.Marshal(child);
                parent.AppendChild(child);
            }

            return doc;
        }
	}
}
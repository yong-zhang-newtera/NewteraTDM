/*
* @(#)CustomActionCollection.cs
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
	/// An collection of CustomAction objects.
	/// </summary>
	/// <version> 1.0.0 26 Aug 2008 </version>
	public class CustomActionCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the CustomActionCollection class.
		/// </summary>
		public CustomActionCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of CustomActionCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal CustomActionCollection(XmlElement xmlElement) : base()
		{
			//this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public CustomAction this[int index]  
		{
			get  
			{
				return ((CustomAction) List[index]);
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member using attribute name
		/// </summary>
		public CustomAction this[string name]  
		{
			get  
			{
				CustomAction found = null;

				foreach (CustomAction element in List)
				{
					if (element.Name == name)
					{
						found = element;
						break;
					}
				}

				return found;
			}
			set  
			{
			}
		}

		/// <summary>
		/// Adds an CustomAction to the CustomActionCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(CustomAction value)  
		{
			// append at the end
	        return List.Add(value);
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(CustomAction value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, CustomAction value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(CustomAction value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(CustomAction value )  
		{
			// If value is not of type CustomAction, this will return false.
			return (List.Contains( value ));
		}

        /// <summary>
        /// Build a custom action collection from an XML file.
        /// </summary>
        /// <param name="fileName">the name of the XML file</param>
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
                throw new Exception("Failed to read the file :" + fileName, ex);
            }
        }

        /// <summary>
        /// Build a custom action collection from an stream.
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
        /// Build a custom action collection from a text reader.
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
        /// Write a collection of custom actions to an XML file.
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
                throw new Exception("Failed to write to file :" + fileName, ex);
            }
        }

        /// <summary>
        /// Write a collection of custom actions to a Stream.
        /// </summary>
        /// <param name="stream">the stream object to which to write a XML data</param>
        public void Write(Stream stream)
        {
            XmlDocument doc = GetXmlDocument();

            doc.Save(stream);
        }

        /// <summary>
        /// Write a collection of custom actions to a TextWriter.
        /// </summary>
        /// <param name="writer">the TextWriter instance to which to write a XML schema
        /// </param>
        public void Write(TextWriter writer)
        {
            XmlDocument doc = GetXmlDocument();

            doc.Save(writer);
        }

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is CustomAction))
			{
				throw new ArgumentException( "value must be of type CustomAction.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is CustomAction))
			{
				throw new ArgumentException( "value must be of type CustomAction.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes before setting a value in the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSet(int index, Object oldValue, Object newValue)  
		{
			if (!(newValue is CustomAction))
			{
				throw new ArgumentException( "newValue must be of type CustomAction.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is CustomAction))
			{
				throw new ArgumentException( "value must be of type CustomAction." );
			}
		}

        /// <summary>
        /// Unmarshal the custom action collection
        /// </summary>
        /// <param name="parent">An xml element</param>
        internal void Unmarshal(XmlElement parent)
        {
            this.List.Clear();

            foreach (XmlElement xmlElement in parent.ChildNodes)
            {
                CustomAction customAction = new CustomAction(xmlElement);

                this.Add(customAction);
            }
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        internal void Marshal(XmlElement parent)
        {
            XmlElement child;

            foreach (CustomAction customAction in List)
            {
                child = parent.OwnerDocument.CreateElement("CustomAction");
                customAction.Marshal(child);
                parent.AppendChild(child);
            }
        }

        /// <summary>
        /// Gets the xml document that represents a collection of custom actions
        /// </summary>
        /// <returns>A XmlDocument instance</returns>
        private XmlDocument GetXmlDocument()
        {
            // Marshal the objects to xml document
            XmlDocument doc = new XmlDocument();

            XmlElement element = doc.CreateElement("CustomActions");

            doc.AppendChild(element);

            Marshal(element);

            return doc;
        }
	}
}
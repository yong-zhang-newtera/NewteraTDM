/*
* @(#)PivotLayoutCollection.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.Pivot
{
	using System;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle PivotLayout objects when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 16 Oct 2008 </version>
	public class PivotLayoutCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the PivotLayoutCollection class.
		/// </summary>
		public PivotLayoutCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public PivotLayout this[int index]  
		{
			get  
			{
				return( (PivotLayout) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an PivotLayout to the PivotLayoutCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(PivotLayout value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(PivotLayout value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, PivotLayout value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(PivotLayout value)  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(PivotLayout value )  
		{
			// If value is not of type PivotLayout, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is PivotLayout))
			{
				throw new ArgumentException( "value must be of type PivotLayout.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is PivotLayout))
			{
				throw new ArgumentException( "value must be of type PivotLayout.", "value" );
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
			if (!(newValue is PivotLayout))
			{
				throw new ArgumentException( "newValue must be of type PivotLayout.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is PivotLayout))
			{
				throw new ArgumentException( "value must be of type PivotLayout." );
			}
		}

		/// <summary>
		/// Constrauct a PivotLayout collection from an XML file.
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
		/// Constrauct a PivotLayout collection from an stream.
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
		/// Constrauct a PivotLayout collection from a text reader.
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
		/// Write the PivotLayout collection to an XML file.
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
		/// Write the pivot layouts as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		public void Write(Stream stream)
		{
			XmlDocument doc = GetXmlDocument();

			doc.Save(stream);
		}

		/// <summary>
        /// Write the pivot layouts as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		public void Write(TextWriter writer)
		{
			XmlDocument doc = GetXmlDocument();

			doc.Save(writer);
		}

		/// <summary>
        /// Unmarshal an element representing pivot layout collection
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			this.List.Clear();

			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				PivotLayout layout = new PivotLayout(xmlElement);

                this.Add(layout);
			}
		}

		/// <summary>
		/// write object to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (PivotLayout layout in List)
			{
				child = parent.OwnerDocument.CreateElement("PivotLayout");
                layout.Marshal(child);
				parent.AppendChild(child);
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

			XmlElement element = doc.CreateElement("PivotLayouts");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}
	}
}
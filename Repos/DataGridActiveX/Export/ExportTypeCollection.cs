/*
* @(#)ExportTypeCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.Export
{
	using System;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle ExportType when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 14 Apr 2006 </version>
	/// <author> Yong Zhang</author>
	public class ExportTypeCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the ExportTypeCollection class.
		/// </summary>
		public ExportTypeCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public ExportType this[int index]  
		{
			get  
			{
				return( (ExportType) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member using attribute name
		/// </summary>
		public ExportType this[string name]  
		{
			get  
			{
				ExportType found = null;

				foreach (ExportType element in List)
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
		/// Adds an ExportType to the ExportTypeCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(ExportType value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(ExportType value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, ExportType value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(ExportType value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(ExportType value )  
		{
			// If value is not of type ExportType, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Read model from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// read the stream.</exception>
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
		/// Read collection from a text reader.
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
		/// Write model as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			XmlDocument doc = GetXmlDocument();

			doc.Save(stream);
		}

		/// <summary>
		/// Write model as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema.</param>
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
			ExportType node;
			this.List.Clear();

			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				node = new ExportType();
				node.Unmarshal(xmlElement);

				this.Add((ExportType) node);
			}
		}

		/// <summary>
		/// write object to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (ExportType node in List)
			{
				child = parent.OwnerDocument.CreateElement("ExportType");
				node.Marshal(child);
				parent.AppendChild(child);
			}		
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is ExportType))
			{
				throw new ArgumentException( "value must be of type ExportType.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is ExportType))
			{
				throw new ArgumentException( "value must be of type ExportType.", "value" );
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
			if (!(newValue is ExportType))
			{
				throw new ArgumentException( "newValue must be of type ExportType.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is ExportType))
			{
				throw new ArgumentException( "value must be of type ExportType." );
			}
		}

		/// <summary>
		/// Gets the xml document that represents an export policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("ExportTypes");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}
	}
}
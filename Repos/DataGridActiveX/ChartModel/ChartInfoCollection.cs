/*
* @(#)ChartInfoCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle ChartInfo objects when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 30 Apr 2006 </version>
	public class ChartInfoCollection : CollectionBase
	{
		/// <summary>
		///  Initializes a new instance of the ChartInfoCollection class.
		/// </summary>
		public ChartInfoCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public ChartInfo this[int index]  
		{
			get  
			{
				return( (ChartInfo) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an ChartInfo to the ChartInfoCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(ChartInfo value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(ChartInfo value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, ChartInfo value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(ChartInfo value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(ChartInfo value )  
		{
			// If value is not of type ChartInfo, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is ChartInfo))
			{
				throw new ArgumentException( "value must be of type ChartInfo.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is ChartInfo))
			{
				throw new ArgumentException( "value must be of type ChartInfo.", "value" );
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
			if (!(newValue is ChartInfo))
			{
				throw new ArgumentException( "newValue must be of type ChartInfo.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is ChartInfo))
			{
				throw new ArgumentException( "value must be of type ChartInfo." );
			}
		}

		/// <summary>
		/// Constrauct a ChartInfo collection from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
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
			catch (Exception)
			{
				throw new ChartModelException("Failed to read the file :" + fileName);
			}
		}
		
		/// <summary>
		/// Constrauct a ChartInfo collection from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
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
					throw new ChartModelException(e.Message);
				}
			}
		}

		/// <summary>
		/// Constrauct a ChartInfo collection from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
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
					throw new ChartModelException(e.Message);
				}
			}
		}

		/// <summary>
		/// Write the ChartInfo collection to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
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
			catch (System.IO.IOException)
			{
				throw new ChartModelException("Failed to write to file :" + fileName);
			}
		}
		
		/// <summary>
		/// Write the data view as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException)
			{
				throw new ChartModelException("Failed to write the ChartInfoCollection object");
			}
		}

		/// <summary>
		/// Write the data view as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException)
			{
				throw new ChartModelException("Failed to write the ChartInfoCollection object");
			}
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
				IChartNode node = NodeFactory.Instance.Create(xmlElement);

				this.Add((ChartInfo) node);
			}
		}

		/// <summary>
		/// write object to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (ChartInfo node in List)
			{
				child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(node.NodeType));
				node.Marshal(child);
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

			XmlElement element = doc.CreateElement("ChartInfos");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}
	}
}
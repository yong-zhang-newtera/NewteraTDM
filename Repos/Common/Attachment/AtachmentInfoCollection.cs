/*
* @(#)AttachmentInfoCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Attachment
{
	using System;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IAttachmentInfo when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 08 Jan 2004 </version>
	/// <author> Yong Zhang</author>
	public class AttachmentInfoCollection : CollectionBase, IAttachmentInfo
	{
		/// <summary>
		///  Initializes a new instance of the AttachmentInfoCollection class.
		/// </summary>
		public AttachmentInfoCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of AttachmentInfoCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AttachmentInfoCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public IAttachmentInfo this[int index]  
		{
			get  
			{
				return( (IAttachmentInfo) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds an IAttachmentInfo to the AttachmentInfoCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IAttachmentInfo value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IAttachmentInfo value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IAttachmentInfo value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IAttachmentInfo value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IAttachmentInfo value )  
		{
			// If value is not of type IAttachmentInfo, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is IAttachmentInfo))
			{
				throw new ArgumentException( "value must be of type IAttachmentInfo.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IAttachmentInfo))
			{
				throw new ArgumentException( "value must be of type IAttachmentInfo.", "value" );
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
			if (!(newValue is IAttachmentInfo))
			{
				throw new ArgumentException( "newValue must be of type IAttachmentInfo.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IAttachmentInfo))
			{
				throw new ArgumentException( "value must be of type IAttachmentInfo." );
			}
		}

		/// <summary>
		/// Constrauct a attachment info collection from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="AttachmentException">AttachmentException is thrown when it fails to
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
                throw new AttachmentException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a attachment info from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="AttachmentException">AttachmentException is thrown when it fails to
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
					throw new AttachmentException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a attachment info from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="AttachmentException">AttachmentException is thrown when it fails to
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
					throw new AttachmentException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write the attachment info to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="AttachmentException">AttachmentException is thrown when it fails to
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
				throw new AttachmentException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write the data view as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="AttachmentException">AttachmentException is thrown when it fails to
		/// write to the stream.
		/// </exception>
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
		/// <exception cref="AttachmentException">AttachmentException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(TextWriter writer)
		{
			XmlDocument doc = GetXmlDocument();

			doc.Save(writer);
		}

		#region IAttachmentInfo members

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public virtual NodeType NodeType	{
			get
			{
				return NodeType.Collection;
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
				IAttachmentInfo element = NodeFactory.Instance.Create(xmlElement);

				this.Add(element);
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (IAttachmentInfo info in List)
			{
				child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(info.NodeType));
				info.Marshal(child);
				parent.AppendChild(child);
			}
		}

		#endregion

		/// <summary>
		/// Gets the xml document that represents the attachment infos
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement(NodeFactory.ConvertTypeToString(NodeType));

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}
	}
}
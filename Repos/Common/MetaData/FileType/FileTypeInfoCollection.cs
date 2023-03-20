/*
* @(#)FileTypeInfoCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.FileType
{
	using System;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle FileTypeInfo when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.0 14 Jan 2004 </version>
	/// <author> Yong Zhang</author>
	public class FileTypeInfoCollection : FileTypeNodeCollection
	{
		/// <summary>
		///  Initializes a new instance of the FileTypeInfoCollection class.
		/// </summary>
		public FileTypeInfoCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of FileTypeInfoCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal FileTypeInfoCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Constrauct a attachment info collection from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="FileTypeException">FileTypeException is thrown when it fails to
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
                throw new FileTypeException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a attachment info from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="FileTypeException">FileTypeException is thrown when it fails to
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
					throw new FileTypeException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a attachment info from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="FileTypeException">FileTypeException is thrown when it fails to
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
					throw new FileTypeException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write the attachment info to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="FileTypeException">FileTypeException is thrown when it fails to
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
				throw new FileTypeException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write the data view as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="FileTypeException">FileTypeException is thrown when it fails to
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
				throw new FileTypeException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Write the data view as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="FileTypeException">FileTypeException is thrown when it fails to
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
				throw new FileTypeException("Failed to write the SchemaModel object", ex);
			}
		}

		#region IFileTypeInfo members

		/// <summary>
		/// Gets the type of Node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType	{
			get
			{
				return NodeType.TypeCollection;
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
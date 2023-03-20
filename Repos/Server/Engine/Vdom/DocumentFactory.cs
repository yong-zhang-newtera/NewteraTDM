/*
* @(#)DocumentFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom
{
	using System;
	using System.Xml;
	using System.IO;
	using Newtera.Common.Core;
	using Newtera.Server.Engine.Vdom.Common;
	using Newtera.Server.DB;

	/// <summary>
	/// A singleton class that creates a concrete Document object according to an URL
	/// of document. For example, document("db://PurchaseOrder.xml") will cause it to create
	/// and return a DocumentDB object; document("invoice.xml") will return a DocumentText. 
	/// </summary>
	/// <version>  	1.0.0 11 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class DocumentFactory
	{
		/* constants definitions */
		internal const string DB_SOURCE = "db://";
		internal const string URL_SOURCE = "http://";
		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static DocumentFactory theDocumentFactory;
		
		static DocumentFactory()
		{
			theDocumentFactory = new DocumentFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private DocumentFactory()
		{
		}

		/// <summary>
		/// Gets the DocumentFactory instance.
		/// </summary>
		/// <returns> The DocumentFactory instance.</returns>
		static public DocumentFactory Instance
		{
			get
			{
				return theDocumentFactory;
			}
		}
		
		/// <summary>
		/// Creates a concrete document object from the supplied URL. Currently, two type
		/// of documents are supported. The textual XML document and database-based document.
		/// </summary>
		/// <param name="url">the url of the document.</param>
		/// <returns> a VDocument object </returns>
		public VDocument Create(string url)
		{
			VDocument doc = null;
			
			if (url.StartsWith(DB_SOURCE))
			{
				//Get a data provider for database				
				IDataProvider dataProvider = DataProviderFactory.Instance.Create();
				
				doc = new DocumentDB(dataProvider);
			}
			else
			{
				doc = new DocumentText();
			}
			
			// load the document from the source specified by the url
			doc.Load(url);
			
			return doc;
		}
		
		/// <summary>
		/// Creates an empty DocumentJDOM object for constructing a VDOM
		/// document from scratch.
		/// 
		/// </summary>
		/// <returns> an empty VDocument object.
		/// 
		/// </returns>
		public virtual VDocument Create()
		{
			VDocument doc = null;
			
			doc = new DocumentText();
			
			return doc;
		}
		
		/// <summary>
		/// Creates a DocumentText object from a XML string.
		/// </summary>
		/// <param name="xmlString">the xml string</param>
		/// <returns> a VDocument object</returns>
		public virtual VDocument CreateFromXMLString(string xmlString)
		{
			DocumentText doc = new DocumentText();
			
			doc.LoadXml(xmlString);
			
			return doc;
		}
		
		/// <summary>
		/// Get the schema info from the specified url.
		/// </summary>
		/// <param name="url">the url string</param>
		/// <returns>SchemaInfo object</returns>
		/// <remarks>the url is in the form of db://MySchema.xml?version=1.2</remarks>
		internal SchemaInfo GetSchemaInfo(string url)
		{
			
			int startPos = url.IndexOf("//");
			if (startPos < 0)
			{
				startPos = 0;
			}
			else
			{
				startPos = startPos + 2;
			}
			int endPos = url.IndexOf(".");
			if (endPos < 0)
			{
				endPos = url.Length;
			}
			string schemaName = url.Substring(startPos, (endPos) - (startPos));
			
			string version = "1.0";
			startPos = url.IndexOf("version=");
			if (startPos > 0)
			{
				version = url.Substring(startPos + 8);
			}
			
			SchemaInfo schemaInfo = new SchemaInfo();

			schemaInfo.Name = schemaName;
			schemaInfo.Version = version;
			
			return schemaInfo;
		}
	}
}
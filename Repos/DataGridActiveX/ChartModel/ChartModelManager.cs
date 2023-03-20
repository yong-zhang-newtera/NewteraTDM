/*
* @(#)ChartModelManager.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// This is the top level class that manages all ChartDef instances of a
	/// schema model.
	/// </summary>
	/// <version> 1.0.0 24 Apr 2006 </version>
	public class ChartModelManager : ChartNodeBase
	{
		private bool _serializeDataPoints = true; // runtime use
		private ChartDefCollection _chartDefs;

		/// <summary>
		/// Initiate an instance of ChartModelManager class
		/// </summary>
		public ChartModelManager(): base()
		{
			_chartDefs = new ChartDefCollection();
			_chartDefs.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// Gets a collection of ChartDef objects managed
		/// </summary>
		public ChartDefCollection ChartDefs
		{
			get
			{
				return this._chartDefs;
			}
		}

		/// <summary>
		/// Gets or sets information indicating whether to serialize data points
		/// when the chart objects are serialized.
		/// </summary>
		/// <value>True to serialize data points, false not to serialize data points</value>
		public virtual bool SerializeDataPoints
		{
			get
			{
				return _serializeDataPoints;
			}
			set
			{
				_serializeDataPoints = value;
				foreach (ChartDef chart in _chartDefs)
				{
					chart.SerializeDataPoints = value;
				}
			}
		}

		/// <summary>
		/// Add a ChartDef instance to the collection
		/// </summary>
		/// <param name="chartDef">The ChartDef instance</param>
		public void AddChartDef(ChartDef chartDef)
		{
			_chartDefs.Add(chartDef);
		}

		/// <summary>
		/// Remove a chartDef from the collection.
		/// </summary>
		/// <param name="chartDef">The ChartDef to be removed</param>
		public void RemoveChartDef(ChartDef chartDef)
		{
			_chartDefs.Remove(chartDef);
		}

		/// <summary>
		/// Read model from an XML file.
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
		/// Read model from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// read the stream.</exception>
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
		/// Read model from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// read the text reader</exception>
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
		/// Write model to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// write to the file.</exception> 
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
		/// Write model as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException)
			{
				throw new ChartModelException("Failed to write the model");
			}
		}

		/// <summary>
		/// Write model as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="ChartModelException">ChartModelException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException)
			{
				throw new ChartModelException("Failed to write the model");
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType 
		{
			get
			{
				return NodeType.ChartModelManager;
			}
		}

		/// <summary>
		/// create model from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of RuleSet instances
			_chartDefs = (ChartDefCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			_chartDefs.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// write model to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the chartDef defs
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_chartDefs.NodeType));
			_chartDefs.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an xacl policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("ChartModel");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}
	}
}
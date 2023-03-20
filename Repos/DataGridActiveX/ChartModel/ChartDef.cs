/*
* @(#)ChartDef.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Data;
	using System.IO;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// The class represents definition of a chart.
	/// </summary>
	/// <version>1.0.0 24 Apr 2006</version>
	public class ChartDef : ChartNodeBase
	{
		private string _name;
		private string _desc;
		private string _title;
		private bool _useSelectedRows;
		private DataSeriesOrientation _orientaion;
		private LegendLocation _legendLocation = LegendLocation.Right;
		private bool _serializeDataPoints = true; // run-time use
		
		/// <summary>
		/// Initiate an instance of ChartDef class.
		/// </summary>
		public ChartDef() : base()
		{
			_name = null;
			_desc = null;
			_title = null;
			_orientaion = DataSeriesOrientation.ByColumn; // default
			_useSelectedRows = false; // default
		}

		/// <summary>
		/// Initiating an instance of ChartDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ChartDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Convert xml string to a corresponding ChartDef instance
		/// </summary>
		/// <param name="xml">The xml string</param>
		/// <returns>The ChartDef instance</returns>
		public static ChartDef ConvertToChartDef(string chartType, string xml)
		{
			ChartDef chartDef = null;
			StringReader reader;
			ChartType chartTypeEnum = (ChartType) Enum.Parse(typeof(ChartType), chartType);

			switch (chartTypeEnum)
			{
				case ChartType.Line:
					chartDef = new LineChartDef();
					reader = new StringReader(xml);
					chartDef.Read(reader);
                    break;

                case ChartType.Bar:
                    chartDef = new LineChartDef(ChartType.Bar);
                    reader = new StringReader(xml);
                    chartDef.Read(reader);

					break;
				case ChartType.Contour:
					chartDef = new ContourChartDef();
					reader = new StringReader(xml);
					chartDef.Read(reader);
					break;
			}

			return chartDef;
		}

		/// <summary>
		/// Gets or sets name of the chart definition
		/// </summary>
		/// <value> The unique chart name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets description of the chart def.
		/// </summary>
		/// <value> The chart description.</value>
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets title of the chart def.
		/// </summary>
		/// <value> The chart title.</value>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets information indicating whether to serialize data points
		/// when the chart object is serialized.
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
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets orientation of the data series.
		/// </summary>
		/// <value> One of DataSeriesOrientation enum values. The default is by column</value>
		public DataSeriesOrientation Orientation
		{
			get
			{
				return _orientaion;
			}
			set
			{
				_orientaion = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets location of the chart legend.
		/// </summary>
		/// <value> One of LegendLocation enum values. The default is at Right</value>
		public LegendLocation LegendLocation
		{
			get
			{
				return this._legendLocation;
			}
			set
			{
				_legendLocation = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets information indicating whether data range is from
		/// selected rows.
		/// </summary>
		/// <value> true if the data range is from selected rows, false is from all rows. Default is false.</value>
		public bool UseSelectedRows
		{
			get
			{
				return _useSelectedRows;
			}
			set
			{
				_useSelectedRows = value;
				FireValueChangedEvent(value);
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
				return NodeType.ChartDef;
			}
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
		/// create an ChartDef from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("name");
			if (str != null && str.Length > 0)
			{
				_name = str;
			}
			else
			{
				_name = null;
			}

			str = parent.GetAttribute("desc");
			if (str != null && str.Length > 0)
			{
				_desc = str;
			}
			else
			{
				_desc = null;
			}

			str = parent.GetAttribute("title");
			if (str != null && str.Length > 0)
			{
				_title = str;
			}
			else
			{
				_title = null;
			}


			// Set _useSelectedRows member
			str = parent.GetAttribute("selected");
			_useSelectedRows = (str != null && str == "true" ? true : false);

			// set _orientaion member
			str = parent.GetAttribute("orient");
			if (str != null && str.Length > 0)
			{
				this._orientaion = (DataSeriesOrientation) Enum.Parse(typeof(DataSeriesOrientation), str);
			}
			else
			{
				this._orientaion = DataSeriesOrientation.ByColumn;
			}

			// set _legendLocation member
			str = parent.GetAttribute("legendLoc");
			if (str != null && str.Length > 0)
			{
				this._legendLocation = (LegendLocation) Enum.Parse(typeof(LegendLocation), str);
			}
			else
			{
				this._legendLocation = LegendLocation.Right;
			}
		}

		/// <summary>
		/// write ChartDef to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_name != null && _name.Length > 0)
			{
				parent.SetAttribute("name", _name);
			}

			if (_desc != null && _desc.Length > 0)
			{
				parent.SetAttribute("desc", _desc);
			}

			if (_title != null && _title.Length > 0)
			{
				parent.SetAttribute("title", _title);
			}

			// Write _useSelectedRows member, default is false
			if (_useSelectedRows)
			{
				parent.SetAttribute("selected", "true");                	
			}

			// write _orientaion
			parent.SetAttribute("orient", Enum.GetName(typeof(DataSeriesOrientation), _orientaion));

			// write _legendLocation
			parent.SetAttribute("legendLoc", Enum.GetName(typeof(LegendLocation), _legendLocation));
		}

		/// <summary>
		/// Gets the xml document that represents an xacl policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		protected virtual XmlDocument GetXmlDocument()
		{
			return null;
		}
	}
}
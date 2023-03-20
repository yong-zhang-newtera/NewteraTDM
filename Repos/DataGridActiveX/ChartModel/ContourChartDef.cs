/*
* @(#)ContourChartDef.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Data;
	using System.Xml;

	/// <summary>
	/// The class represents definition of a contour chart.
	/// </summary>
	/// <version>1.0.0 3 May 2006</version>
	public class ContourChartDef : ChartDef
	{
		private int _xPoints;
		private float _xStart;
		private float _xStep;
		private int _yPoints;
		private float _yStart;
		private float _yStep;
		private string _xAxisLabel;
		private string _yAxisLabel;
		private string _zAxisLabel;
		private DataSeries _zDataSeries;

		/// <summary>
		/// Initiate an instance of ContourChartDef class.
		/// </summary>
		public ContourChartDef() : base()
		{
			_xPoints = 0;
			_xStart = 1;
			_xStep = 1;
			_yPoints = 0;
			_yStart = 1;
			_yStep = 1;
			_xAxisLabel = null;
			_yAxisLabel = null;
			_zAxisLabel = null;
			_zDataSeries = new DataSeries();
			_zDataSeries.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// Initiating an instance of ContourChartDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ContourChartDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the number of X data points.
		/// </summary>
		public int XPoints
		{
			get
			{
				return _xPoints;
			}
			set
			{
				_xPoints = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the number of Y data points.
		/// </summary>
		public int YPoints
		{
			get
			{
				return _yPoints;
			}
			set
			{
				_yPoints = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the start value of XAxis.
		/// </summary>
		public float XStart
		{
			get
			{
				return _xStart;
			}
			set
			{
				_xStart = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the start value of YAxis.
		/// </summary>
		public float YStart
		{
			get
			{
				return _yStart;
			}
			set
			{
				_yStart = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the increment value of XAxis.
		/// </summary>
		public float XStep
		{
			get
			{
				return _xStep;
			}
			set
			{
				_xStep = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the increment value of YAxis.
		/// </summary>
		public float YStep
		{
			get
			{
				return _yStep;
			}
			set
			{
				_yStep = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the label of XAxis.
		/// </summary>
		public string XAxisLabel
		{
			get
			{
				return _xAxisLabel;
			}
			set
			{
				_xAxisLabel = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the label of YAxis.
		/// </summary>
		public string YAxisLabel
		{
			get
			{
				return _yAxisLabel;
			}
			set
			{
				_yAxisLabel = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the label of ZAxis.
		/// </summary>
		public string ZAxisLabel
		{
			get
			{
				return _zAxisLabel;
			}
			set
			{
				_zAxisLabel = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the data series for z axis, z data series is a flattened two-dimensional data.
		/// </summary>
		public DataSeries ZDataSeries
		{
			get
			{
				return this._zDataSeries;
			}
		}

		/// <summary>
		/// Gets or sets information indicating whether to serialize data points
		/// when the LineDef object is serialized.
		/// </summary>
		/// <value>True to serialize data points, false not to serialize data points</value>
		public override bool SerializeDataPoints
		{
			get
			{
				return base.SerializeDataPoints;
			}
			set
			{
				base.SerializeDataPoints = value;
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
				return NodeType.ContourChartDef;
			}
		}

		/// <summary>
		/// create an ContourChartDef from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set _xPoints member
			string str = parent.GetAttribute("xpoints");
			if (str != null && str.Length > 0)
			{
				_xPoints = int.Parse(str);
			}
			else
			{
				_xPoints = 0;
			}

			str = parent.GetAttribute("ypoints");
			if (str != null && str.Length > 0)
			{
				_yPoints = int.Parse(str);
			}
			else
			{
				_yPoints = 0;
			}

			// set _xStart member
			str = parent.GetAttribute("xstart");
			if (str != null && str.Length > 0)
			{
				_xStart = float.Parse(str);
			}
			else
			{
				_xStart = 0;
			}

			// set _yStart member
			str = parent.GetAttribute("ystart");
			if (str != null && str.Length > 0)
			{
				_yStart = float.Parse(str);
			}
			else
			{
				_yStart = 0;
			}

			// set _xStep member
			str = parent.GetAttribute("xstep");
			if (str != null && str.Length > 0)
			{
				_xStep = float.Parse(str);
			}
			else
			{
				_xStep = 1;
			}

			// set _yStep member
			str = parent.GetAttribute("ystep");
			if (str != null && str.Length > 0)
			{
				_yStep = float.Parse(str);
			}
			else
			{
				_yStep = 1;
			}

			// set _xAxisLabel member
			str = parent.GetAttribute("xlabel");
			if (str != null && str.Length > 0)
			{
				 _xAxisLabel = str;
			}
			else
			{
				_xAxisLabel = null;
			}

			// set _yAxisLabel member
			str = parent.GetAttribute("ylabel");
			if (str != null && str.Length > 0)
			{
				_yAxisLabel = str;
			}
			else
			{
				_yAxisLabel = null;
			}

			// set _zAxisLabel member
			str = parent.GetAttribute("zlabel");
			if (str != null && str.Length > 0)
			{
				_zAxisLabel = str;
			}
			else
			{
				_zAxisLabel = null;
			}

			// z data series
			if (SerializeDataPoints)
			{
				_zDataSeries = (DataSeries) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
				_zDataSeries.ValueChanged += new EventHandler(this.ValueChangedHandler);
			}
		}

		/// <summary>
		/// write ContourChartDef to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write _xPoints
			parent.SetAttribute("xpoints", _xPoints.ToString());

			// write _yPoints
			parent.SetAttribute("ypoints", _yPoints.ToString());

			// write _xStart
			parent.SetAttribute("xstart", _xStart.ToString());

			// write _yStart
			parent.SetAttribute("ystart", _yStart.ToString());

			// write _xStep
			parent.SetAttribute("xstep", _xStep.ToString());

			// write _yStep
			parent.SetAttribute("ystep", _yStep.ToString());

			// write _xAxisLabel
			if (_xAxisLabel != null && _xAxisLabel.Length > 0)
			{
				parent.SetAttribute("xlabel", _xAxisLabel);
			}

			// write _yAxisLabel
			if (_yAxisLabel != null && _yAxisLabel.Length > 0)
			{
				parent.SetAttribute("ylabel", _yAxisLabel);
			}

			// write _zAxisLabel
			if (_zAxisLabel != null && _zAxisLabel.Length > 0)
			{
				parent.SetAttribute("zlabel", _zAxisLabel);
			}

			if (SerializeDataPoints)
			{
				// write the _zDataSeries
				XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_zDataSeries.NodeType));
				_zDataSeries.Marshal(child);
				parent.AppendChild(child);
			}
		}

		/// <summary>
		/// Gets the xml document that represents contour chart object
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		protected override XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("Contour");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

	}
}
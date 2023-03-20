/*
* @(#)LineDef.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// The class represents definition of a axis.
	/// </summary>
	/// <version>1.0.0 24 Apr 2006</version>
	public class LineDef : ChartNodeBase
	{
		private string _name;
		private AxisDef _xAxis;
		private AxisDef _yAxis;
		private DataPointCollection _dataPoints;
		private bool _serializeDataPoints = true; // runtime use
		private bool _isNew = true; // runtime use
		
		/// <summary>
		/// Initiate an instance of LineDef class.
		/// </summary>
		public LineDef() : base()
		{
			_name = null;
			_xAxis = new AxisDef();
			_xAxis.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_yAxis = new AxisDef();
			_yAxis.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_dataPoints = new DataPointCollection();
			_dataPoints.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// Initiating an instance of LineDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LineDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets name of the axis
		/// </summary>
		/// <value> The axis name.</value>
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
		/// Gets the X Axis.
		/// </summary>
		/// <value> AxisDef instance.</value>
		public AxisDef XAxis
		{
			get
			{
				return _xAxis;
			}
		}

		/// <summary>
		/// Gets the Y Axis.
		/// </summary>
		/// <value> AxisDef instance.</value>
		public AxisDef YAxis
		{
			get
			{
				return _yAxis;
			}
		}

		/// <summary>
		/// Gets the data points of the line
		/// </summary>
		public DataPointCollection DataPoints
		{
			get
			{
				return _dataPoints;
			}
		}

		/// <summary>
		/// Gets or sets information indicating whether to serialize data points
		/// when the LineDef object is serialized.
		/// </summary>
		/// <value>True to serialize data points, false not to serialize data points</value>
		public bool SerializeDataPoints
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
		/// Gets or sets information indicating whether to the line is newly added.
		/// </summary>
		/// <value>True if it is newly added, false otherwise</value>
		public bool IsNew
		{
			get
			{
				return _isNew;
			}
			set
			{
				_isNew = value;
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
				return NodeType.LineDef;
			}
		}

		/// <summary>
		/// Gets the minimum value of X data series
		/// </summary>
		public float MinX
		{
			get
			{
				float min = 0.0F;

				if (this.DataPoints.Count > 0)
				{
					min = ((DataPoint) this.DataPoints[0]).FloatX;
				}
				
				foreach (DataPoint point in this.DataPoints)
				{
					float val = point.FloatX;
					if (val < min)
					{
						min = val;
					}
				}

				return min;
			}
		}

		/// <summary>
		/// Gets the maximum value of X data series
		/// </summary>
		public float MaxX
		{
			get
			{
				float max = 0.0F;

				if (this.DataPoints.Count > 0)
				{
					max = ((DataPoint) this.DataPoints[0]).FloatX;
				}
				
				foreach (DataPoint point in this.DataPoints)
				{
					float val = point.FloatX;
					if (val > max)
					{
						max = val;
					}
				}

				return max;
			}
		}

		/// <summary>
		/// Gets the minimum value of Y data series
		/// </summary>
		public float MinY
		{
			get
			{
				float min = 0.0F;

				if (this.DataPoints.Count > 0)
				{
					min = ((DataPoint) this.DataPoints[0]).FloatY;
				}
				
				foreach (DataPoint point in this.DataPoints)
				{
					float val = point.FloatY;
					if (val < min)
					{
						min = val;
					}
				}

				return min;
			}
		}

		/// <summary>
		/// Gets the maximum value of Y data series
		/// </summary>
		public float MaxY
		{
			get
			{
				float max = 0.0F;

				if (this.DataPoints.Count > 0)
				{
					max = ((DataPoint) this.DataPoints[0]).FloatY;
				}
				
				foreach (DataPoint point in this.DataPoints)
				{
					float val = point.FloatY;
					if (val > max)
					{
						max = val;
					}
				}

				return max;
			}
		}

		/// <summary>
		/// create an LineDef from a xml document.
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

			// x axis
			_xAxis = (AxisDef) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			_xAxis.ValueChanged += new EventHandler(this.ValueChangedHandler);

			// y axis
			_yAxis = (AxisDef) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
			_yAxis.ValueChanged += new EventHandler(this.ValueChangedHandler);

			if (_serializeDataPoints)
			{
				// a collection of DataPoint instances
				_dataPoints = (DataPointCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[2]);
				_dataPoints.ValueChanged += new EventHandler(this.ValueChangedHandler);
			}
		}

		/// <summary>
		/// write LineDef to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_name != null && _name.Length > 0)
			{
				parent.SetAttribute("name", _name);
			}

			// write the _xAxis
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_xAxis.NodeType));
			_xAxis.Marshal(child);
			parent.AppendChild(child);

			// write the _yAxis
			child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_xAxis.NodeType));
			_yAxis.Marshal(child);
			parent.AppendChild(child);

			if (_serializeDataPoints)
			{
				// write a collection of DataPoint instances
				child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_dataPoints.NodeType));
				_dataPoints.Marshal(child);
				parent.AppendChild(child);
			}
		}
	}
}
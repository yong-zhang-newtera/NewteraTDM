/*
* @(#)LineChartDef.cs
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
	/// The class represents definition of a line chart.
	/// </summary>
	/// <version>1.0.0 24 Apr 2006</version>
	public class LineChartDef : ChartDef
	{
		private LineDefCollection _lines;
		private LineChartDisplayMethod _displayMethod;
        private ChartType _chartType = ChartType.Line;

		/// <summary>
		/// Initiate an instance of LineChartDef class.
		/// </summary>
		public LineChartDef() : base()
		{
			_lines = new LineDefCollection();
			_lines.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_displayMethod = LineChartDisplayMethod.OneXOneY; // default
		}

        		/// <summary>
		/// Initiate an instance of LineChartDef class.
		/// </summary>
		public LineChartDef(ChartType chartType) : base()
		{
			_lines = new LineDefCollection();
			_lines.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_displayMethod = LineChartDisplayMethod.OneXOneY; // default
            _chartType = chartType;
		}

		/// <summary>
		/// Initiating an instance of LineChartDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LineChartDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Chart types can be Line or Bar
        /// </summary>
        public ChartType ChartType
        {
            get
            {
                return _chartType;
            }
            set
            {
                _chartType = value;
            }
        }

        /// <summary>
        /// Gets the data of lines contained in the line chart
        /// </summary>
		public LineDefCollection Lines
		{
			get
			{
				return _lines;
			}
		}

		/// <summary>
		/// Gets or sets the display method for line chart
		/// </summary>
        /// <value>One of the LineChartDisplayMethod enum values </value>
		public LineChartDisplayMethod DisplayMethod
		{
			get
			{
				return _displayMethod;
			}
			set
			{
				_displayMethod = value;
				FireValueChangedEvent(value);
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
				foreach (LineDef lineDef in this.Lines)
				{
					lineDef.SerializeDataPoints = value;
				}
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
				return NodeType.LineChartDef;
			}
		}

		/// <summary>
		/// create an LineChartDef from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set _displayMethod member
			string str = parent.GetAttribute("display");
			if (str != null && str.Length > 0)
			{
				this._displayMethod = (LineChartDisplayMethod) Enum.Parse(typeof(LineChartDisplayMethod), str);
			}
			else
			{
				this._displayMethod = LineChartDisplayMethod.OneXOneY; // default
			}

			// a collection of LineDef instances
			_lines = (LineDefCollection) NodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			_lines.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// write LineChartDef to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write _displayMethod
			parent.SetAttribute("display", Enum.GetName(typeof(LineChartDisplayMethod), _displayMethod));

			// write a collection of LineDef instances
			XmlElement child = parent.OwnerDocument.CreateElement(NodeFactory.ConvertTypeToString(_lines.NodeType));
			_lines.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents line chart object
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		protected override XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("Line");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

	}
}
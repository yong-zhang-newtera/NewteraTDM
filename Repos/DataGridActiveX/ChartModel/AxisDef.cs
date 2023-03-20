/*
* @(#)AxisDef.cs
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
	public class AxisDef : ChartNodeBase
	{
		private string _seriesName;
		private string _seriesCaption;
		private string _label;
		private string _function;
		
		/// <summary>
		/// Initiate an instance of AxisDef class.
		/// </summary>
		public AxisDef() : base()
		{
			_seriesName = null;
			_seriesCaption = null;
			_label = null;
			_function = null;
		}

		/// <summary>
		/// Initiating an instance of AxisDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AxisDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets data series name of the axis
		/// </summary>
		/// <value> The data series name.</value>
		public string SeriesName
		{
			get
			{
				return _seriesName;
			}
			set
			{
				_seriesName = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets data series caption
		/// </summary>
		/// <value> The data series caption.</value>
		public string SeriesCaption
		{
			get
			{
				return _seriesCaption;
			}
			set
			{
				_seriesCaption = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets lable of the axis.
		/// </summary>
		/// <value> The axis label.</value>
		public string Label
		{
			get
			{
				return _label;
			}
			set
			{
				_label = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets function applied to the axis.
		/// </summary>
		/// <value> The function name.</value>
		public string Function
		{
			get
			{
				return _function;
			}
			set
			{
				_function = value;
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
				return NodeType.AxisDef;
			}
		}

		/// <summary>
		/// create an AxisDef from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("name");
			if (str != null && str.Length > 0)
			{
				_seriesName = str;
			}
			else
			{
				_seriesName = null;
			}

			str = parent.GetAttribute("caption");
			if (str != null && str.Length > 0)
			{
				_seriesCaption = str;
			}
			else
			{
				_seriesCaption = null;
			}

			str = parent.GetAttribute("label");
			if (str != null && str.Length > 0)
			{
				_label = str;
			}
			else
			{
				_label = null;
			}

			str = parent.GetAttribute("func");
			if (str != null && str.Length > 0)
			{
				_function = str;
			}
			else
			{
				_function = null;
			}
		}

		/// <summary>
		/// write AxisDef to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_seriesName != null && _seriesName.Length > 0)
			{
				parent.SetAttribute("name", _seriesName);
			}

			if (_seriesCaption != null && _seriesCaption.Length > 0)
			{
				parent.SetAttribute("caption", _seriesCaption);
			}

			if (_label != null && _label.Length > 0)
			{
				parent.SetAttribute("label", _label);
			}

			if (_function != null && _function.Length > 0)
			{
				parent.SetAttribute("func", _function);
			}
		}
	}
}
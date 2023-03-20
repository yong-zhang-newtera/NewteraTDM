/*
* @(#)DataSeries.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Text;
	using System.Collections.Specialized;
	using System.IO;
	using System.Xml;

	/// <summary>
	/// The class represents a data series in chart.
	/// </summary>
	/// <version>1.0.0 24 Apr 2006</version>
	public class DataSeries : ChartNodeBase
	{
		private StringCollection _dataValues;
		
		/// <summary>
		/// Initiate an instance of DataSeries class.
		/// </summary>
		public DataSeries() : base()
		{
			_dataValues = new StringCollection();
		}

		/// <summary>
		/// Initiating an instance of DataSeries class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataSeries(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Get x value of float type
		/// </summary>
		public StringCollection DataValues
		{
			get
			{
				return _dataValues;
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
				return NodeType.DataSeries;
			}
		}

		/// <summary>
		/// create an DataSeries from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			if (parent.InnerText != null && parent.InnerText.Length > 0)
			{
				string[] strArray = ConvertToArray(parent.InnerText);
				_dataValues = new StringCollection();
				_dataValues.AddRange(strArray);
			}
		}

		/// <summary>
		/// write DataSeries to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			string str = ConvertToString(_dataValues);
			parent.InnerText = str;
		}

		/// <summary>
		/// Convert a string collection to a semi-colon separated string
		/// </summary>
		/// <param name="values">A StringCollection object</param>
		/// <returns>A semi-colon separated string</returns>
		private string ConvertToString(StringCollection values)
		{
			StringBuilder builder = new StringBuilder(values.Count);
			foreach (string val in values)
			{
                if (builder.Length > 0)
                {
                    builder.Append(";");
                }

				builder.Append(val);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Convert a semi-colon separated string in to a string array
		/// </summary>
		/// <param name="valueStr">The string value</param>
		/// <returns>A string array</returns>
		private string[] ConvertToArray(string valueStr)
		{
			return valueStr.Split(';');
		}
	}
}
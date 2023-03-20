/*
* @(#)ChartFormat.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.Export
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// A class represents infomation about a column in a datagrid
	/// </summary>
	/// <version>  	1.0.0 1 June 2006</version>
	public class ChartFormat
	{
		private string _name;
		private string _suffix;
		private string _formatter;

		/// <summary>
		/// Initiate an instance of ChartFormat
		/// </summary>
		public ChartFormat()
		{
			_name = null;
			_suffix = null;
			_formatter = null;
		}

		/// <summary>
		/// Gets or sets the name of the Export Type
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the suffix of export file
		/// </summary>
		public string Suffix
		{
			get
			{
				return _suffix;
			}
			set
			{
				_suffix = value;
			}
		}

		/// <summary>
		/// Gets or sets class definition of formatter
		/// </summary>
		public string Formatter
		{
			get
			{
				return _formatter;
			}
			set
			{
				_formatter = value;
			}
		}

		/// <summary>
		/// create an ChartFormat from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			string str = parent.GetAttribute("name");
			if (str != null && str.Length > 0)
			{
				_name = str;
			}
			else
			{
				_name = null;
			}

			str = parent.GetAttribute("suffix");
			if (str != null && str.Length > 0)
			{
				_suffix = str;
			}
			else
			{
				_suffix = null;
			}

			str = parent.GetAttribute("formatter");
			if (str != null && str.Length > 0)
			{
				_formatter = str;
			}
			else
			{
				_formatter = null;
			}
		}

		/// <summary>
		/// write ChartFormat to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			if (_name != null && _name.Length > 0)
			{
				parent.SetAttribute("name", _name);
			}

			if (_suffix != null && _suffix.Length > 0)
			{
				parent.SetAttribute("suffix", _suffix);
			}

			if (_formatter != null && _formatter.Length > 0)
			{
				parent.SetAttribute("formatter", _formatter);
			}
		}
	}
}
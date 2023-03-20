/*
* @(#)ExportType.cs
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
	public class ExportType
	{
		private string _name;
		private string _suffix;
		private string _exporter;
        private ExportFormat _format;

		/// <summary>
		/// Initiate an instance of ExportType
		/// </summary>
		public ExportType()
		{
			_name = null;
			_suffix = null;
			_exporter = null;
            _format = ExportFormat.Grid; // default
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
        /// Gets or sets the format of export file, it can be Grid or Hierarchical
        /// </summary>
        public ExportFormat Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }

		/// <summary>
		/// Gets or sets class definition of exporter
		/// </summary>
		public string Exporter
		{
			get
			{
				return _exporter;
			}
			set
			{
				_exporter = value;
			}
		}

		/// <summary>
		/// create an ExportType from a xml document.
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

            str = parent.GetAttribute("format");
            if (str != null && str.Length > 0)
            {
                try
                {
                    _format = (ExportFormat)Enum.Parse(typeof(ExportFormat), str);
                }
                catch(Exception)
                {
                    _format = ExportFormat.Grid;
                }
            }
            else
            {
                _format = ExportFormat.Grid; // defauly
            }

			str = parent.GetAttribute("exporter");
			if (str != null && str.Length > 0)
			{
				_exporter = str;
			}
			else
			{
				_exporter = null;
			}
		}

		/// <summary>
		/// write DataPoint to xml document
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

            if (_format != ExportFormat.Grid)
            {
                parent.SetAttribute("format", Enum.GetName(typeof(ExportFormat), _format));
            }

			if (_exporter != null && _exporter.Length > 0)
			{
				parent.SetAttribute("exporter", _exporter);
			}
		}
	}

    public enum ExportFormat
    {
        Grid,
        Hierarchical
    }
}
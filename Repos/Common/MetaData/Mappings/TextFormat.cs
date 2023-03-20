/*
* @(#)TextFormat.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// The class represents the text format definition or text converter definition
	/// for a text file.
	/// </summary>
	/// <version>1.0.0 04 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class TextFormat : MappingNodeBase
	{
		private string _rowDelimiter;
		private bool _isFirstRowColumns;
		private string _columnDelimiter;
        private int _startingDataRow;
		
		/// <summary>
		/// Initiate an instance of TextFormat class.
		/// </summary>
		public TextFormat() : base()
		{
			_rowDelimiter = null;
			_columnDelimiter = null;
			_isFirstRowColumns = false;
            _startingDataRow = 1;
		}

		/// <summary>
		/// Initiating an instance of TextFormat class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal TextFormat(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets row delimiter.
		/// </summary>
		/// <value> A string represents row delimiter.</value>
		public string RowDelimiter
		{
			get
			{
				return _rowDelimiter;
			}
			set
			{
				_rowDelimiter = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the first row of a text
		/// file has column names.
		/// </summary>
		/// <value> true if the first row has columns, false otherwise.</value>
		public bool IsFirstRowColumns
		{
			get
			{
				return _isFirstRowColumns;
			}
			set
			{
				_isFirstRowColumns = value;
			}
		}

		/// <summary>
		/// Gets or sets column delimiter.
		/// </summary>
		/// <value> A string represents column delimiter. </value>
		public string ColumnDelimiter
		{
			get
			{
				return _columnDelimiter;
			}
			set
			{
				_columnDelimiter = value;
			}
		}

        /// <summary>
        /// Gets or sets starting data row.
        /// </summary>
        /// <value> A one-based index </value>
        public int StartingDataRow
        {
            get
            {
                return _startingDataRow;
            }
            set
            {
                _startingDataRow = value;
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
				return NodeType.TextFormat;
			}
		}

		/// <summary>
		/// create an TextFormat from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("rowDelim");
			if (str != null && str.Length > 0)
			{
				_rowDelimiter = str;
			}
			else
			{
				_rowDelimiter = null;
			}

			str = parent.GetAttribute("colDelim");
			if (str != null && str.Length > 0)
			{
				str = UnescapeSpace(str);
				_columnDelimiter = str;
			}
			else
			{
				_columnDelimiter = null;
			}

			str = parent.GetAttribute("firstRowCols");
			if (str != null && str.Length > 0)
			{
				_isFirstRowColumns = true;
			}
			else
			{
				_isFirstRowColumns = false;
			}

            str = parent.GetAttribute("startDataRow");
            if (!String.IsNullOrEmpty(str))
            {
                _startingDataRow = Int32.Parse(str);
            }
            else
            {
                if (_isFirstRowColumns)
                {
                    _startingDataRow = 2;
                }
                else
                {
                    _startingDataRow = 1;
                }
            }
		}

		/// <summary>
		/// write TextFormat to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (_rowDelimiter != null && _rowDelimiter.Length > 0)
			{
				parent.SetAttribute("rowDelim", _rowDelimiter);
			}

			if (_columnDelimiter != null && _columnDelimiter.Length > 0)
			{
				string str = EscapeSpace(_columnDelimiter);

				parent.SetAttribute("colDelim", str);
			}

			if (_isFirstRowColumns)
			{
				parent.SetAttribute("firstRowCols", "true");
			}

            parent.SetAttribute("startDataRow", _startingDataRow.ToString());
		}

		/// <summary>
		/// Escape the spaces contained in a string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private string EscapeSpace(string str)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] != ' ')
				{
					builder.Append(str[i]);
				}
				else
				{
					builder.Append("&nbsp;");
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Unescape the spaces contained in a string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private string UnescapeSpace(string str)
		{
			return str.Replace("&nbsp;", " ");
		}
	}
}
/*
* @(#)ColumnInfo.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// A class represents infomation about a column in a datagrid
	/// </summary>
	/// <version>  	1.0.0 12 April 2006</version>
	public class ColumnInfo
	{
		private string _name;
		private string _caption;
		private bool _isChecked;
        private bool _isArray;

		/// <summary>
		/// Initiate an instance of ColumnInfo
		/// </summary>
		public ColumnInfo()
		{
			_name = null;
			_caption = null;
			_isChecked = true;
            _isArray = false;
		}

		/// <summary>
		/// Gets or sets the name of the column
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
		/// Gets or sets the caption of the column
		/// </summary>
		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the column is checked or not, default is checked.
		/// </summary>
		public bool IsChecked
		{
			get
			{
				return _isChecked;
			}
			set
			{
				_isChecked = value;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the column representing an array, default is false.
        /// </summary>
        public bool IsArray
        {
            get
            {
                return _isArray;
            }
            set
            {
                _isArray = value;
            }
        }
	}
}
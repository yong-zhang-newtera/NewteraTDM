/*
* @(#)MasterServerDef.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.Export
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// A class represents infomation about a master server
	/// </summary>
	/// <version>  	1.0.0 19 April 2013</version>
	public class MasterServerDef
	{
		private string _url;
        private string _schemaName;
        private string _schemaVersion;

		/// <summary>
		/// Initiate an instance of MasterServerDef
		/// </summary>
		public MasterServerDef()
		{
			_url = null;
            _schemaName = null;
            _schemaVersion = null;
		}

		/// <summary>
		/// Gets or sets the url of a master server
		/// </summary>
        public string URL
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        /// <summary>
        /// Gets or sets the scherma name at a master server
        /// </summary>
        public string SchemaName
        {
            get
            {
                return _schemaName;
            }
            set
            {
                _schemaName = value;
            }
        }

        /// <summary>
        /// Gets or sets the scherma version at a master server
        /// </summary>
        public string SchemaVersion
        {
            get
            {
                return _schemaVersion;
            }
            set
            {
                _schemaVersion = value;
            }
        }

		/// <summary>
		/// create an MasterServerDef from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			string str = parent.GetAttribute("url");
			if (!string.IsNullOrEmpty(str))
			{
				_url = str;
			}
			else
			{
				_url = null;
			}

            str = parent.GetAttribute("scehemaName");
            if (!string.IsNullOrEmpty(str))
            {
                _schemaName = str;
            }
            else
            {
                _schemaName = null;
            }

            str = parent.GetAttribute("scehemaVersion");
            if (!string.IsNullOrEmpty(str))
            {
                _schemaVersion = str;
            }
            else
            {
                _schemaVersion = null;
            }
		}

		/// <summary>
		/// write master server def to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			if (!string.IsNullOrEmpty(_url))
			{
				parent.SetAttribute("url", _url);
			}

            if (!string.IsNullOrEmpty(_schemaName))
            {
                parent.SetAttribute("scehemaName", _schemaName);
            }

            if (!string.IsNullOrEmpty(_schemaVersion))
            {
                parent.SetAttribute("scehemaVersion", _schemaVersion);
            }
		}
	}
}
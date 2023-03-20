/*
* @(#)AlgorithmType.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.Export
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// A class represents infomation about an algorithm used to compute results
	/// </summary>
	/// <version>  	1.0.0 19 Aug 2007</version>
	public class AlgorithmType
	{
		private string _name;
		private string _algorithm;
        private string _displayFormat;
        private string _ownerClass;
        private string _displayUrl;

		/// <summary>
		/// Initiate an instance of AlgorithmType
		/// </summary>
		public AlgorithmType()
		{
			_name = null;
            _algorithm = null;
            _displayFormat = null;
            _ownerClass = null;
            _displayUrl = null;
		}

		/// <summary>
		/// Gets or sets the name of the Algorithm
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
		/// Gets or sets definition of algorithm
		/// </summary>
		public string Algorithm
		{
			get
			{
                return _algorithm;
			}
			set
			{
                _algorithm = value;
			}
		}

        /// <summary>
        /// Gets or sets display format of result
        /// </summary>
        public string DisplayFormat
        {
            get
            {
                return _displayFormat;
            }
            set
            {
                _displayFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the owner class name of the algorithm
        /// </summary>
        public string OwnerClass
        {
            get
            {
                return _ownerClass;
            }
            set
            {
                _ownerClass = value;
            }
        }

        /// <summary>
        /// Gets or sets the url that is used to display the result of the algorithm
        /// </summary>
        public string DisplayURL
        {
            get
            {
                return _displayUrl;
            }
            set
            {
                _displayUrl = value;
            }
        }

		/// <summary>
		/// create an AlgorithmType from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public void Unmarshal(XmlElement parent)
		{
			string str = parent.GetAttribute("name");
			if (!string.IsNullOrEmpty(str))
			{
				_name = str;
			}
			else
			{
				_name = null;
			}

			str = parent.GetAttribute("algorithm");
            if (!string.IsNullOrEmpty(str))
			{
                _algorithm = str;
			}
			else
			{
                _algorithm = null;
			}

            str = parent.GetAttribute("format");
            if (!string.IsNullOrEmpty(str))
            {
                _displayFormat = str;
            }
            else
            {
                _displayFormat = null;
            }

			str = parent.GetAttribute("class");
            if (!string.IsNullOrEmpty(str))
			{
                _ownerClass = str;
			}
			else
			{
                _ownerClass = null;
			}

            str = parent.GetAttribute("url");
            if (!string.IsNullOrEmpty(str))
            {
                _displayUrl = str;
            }
            else
            {
                _displayUrl = null;
            }
		}

		/// <summary>
		/// write DataPoint to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public void Marshal(XmlElement parent)
		{
			if (!string.IsNullOrEmpty(_name))
			{
				parent.SetAttribute("name", _name);
			}

            if (!string.IsNullOrEmpty(_algorithm))
			{
                parent.SetAttribute("algorithm", _algorithm);
			}

            if (!string.IsNullOrEmpty(_displayFormat))
            {
                parent.SetAttribute("format", _displayFormat);
            }

            if (!string.IsNullOrEmpty(_ownerClass))
            {
                parent.SetAttribute("class", _ownerClass);
            }

            if (!string.IsNullOrEmpty(_displayUrl))
            {
                parent.SetAttribute("url", _displayUrl);
            }
		}
	}
}
/*
* @(#)ElasticsearchConfig.cs
*
* Copyright (c) 2018 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Config
{
	using System;
	using System.IO;
	using System.Xml;

	using Newtera.Common.Core;

	/// <summary> 
	/// A class that reads/writes the full-text index settings of a full-text index configuration file.
	/// </summary>
	/// <version> 1.0.0 29 Apr 2014 </version>
	public class ElasticsearchConfig
	{
        private const string CONFIG_FILE = "elasticsearch_config.xml";
        private const string CONFIG_DIR = @"\Config\";
		private const string KEY_NAME = "key";
		private const string VALUE_NAME = "value";
		private const string ADD = "add";
        private const string SERVER_URL = "ElasticsearchURL";
        private const string USER = "User";
        private const string PASSWORD = "Password";
        private const string ANALYZER = "analyzer";
        private const string SEARCH_ANALYSER = "search_analyzer";

        private string _configFileName;
		private XmlDocument _doc;
        private string _url;
        private string _user;
        private string _password;
        private string _analyzer;
        private string _searchAnalyzer;

        /// <summary>
        /// Singleton's private instance.
        /// </summary>
        private static ElasticsearchConfig theConfig;

        static ElasticsearchConfig()
        {
            theConfig = new ElasticsearchConfig();
        }

        /// <summary>
        /// The private constructor.
        /// </summary>
        private ElasticsearchConfig()
        {
            _configFileName = NewteraNameSpace.GetAppHomeDir() + ElasticsearchConfig.CONFIG_DIR + ElasticsearchConfig.CONFIG_FILE;
            _url = null;
            _analyzer = null;
            _searchAnalyzer = null;

            if (File.Exists(_configFileName))
            {
                _doc = new XmlDocument();
                _doc.Load(_configFileName);
            }
            else
            {
                _doc = null;
            }
        }

        /// <summary>
        /// Gets the ElasticsearchConfig instance.
        /// </summary>
        /// <returns> The ElasticsearchConfig instance.</returns>
        static public ElasticsearchConfig Instance
        {
            get
            {
                return theConfig;
            }
        }

        public bool IsElasticsearchEnabled
        {
            get
            {
                bool status = false;

                if (_doc != null)
                {
                    if (_url == null)
                    {
                        _url = GetAppSetting(SERVER_URL);
                        if (!string.IsNullOrEmpty(_url))
                            status = true;
                    }
                    else
                        status = true;
                }

                return status;
            }
        }

        public string ElasticsearchURL
        {
            get
            {
                if (_doc != null && _url == null)
                {
                    _url = GetAppSetting(SERVER_URL);
                }

                return _url;
            }
        }

        public string User
        {
            get
            {
                if (_doc != null && _user == null)
                {
                    _user = GetAppSetting(USER);
                }

                return _user;
            }
        }

        public string Password
        {
            get
            {
                if (_doc != null && _password == null)
                {
                    _password = GetAppSetting(PASSWORD);
                }

                return _password;
            }
        }

        public string Analyzer
        {
            get
            {
                if (_doc != null && _analyzer == null)
                {
                    _analyzer = GetAppSetting(ANALYZER);
                }

                return _analyzer;
            }
        }

        public string SearchAnalyzer
        {
            get
            {
                if (_doc != null && _searchAnalyzer == null)
                {
                    _searchAnalyzer = GetAppSetting(SEARCH_ANALYSER);
                }

                return _searchAnalyzer;
            }
        }

        /// <summary>
        /// Gets the configurations as a key-value collection
        /// </summary>
        /// <param name="section">The section name</param>
        /// <returns>A ConfigKeyValueCollection instance</returns>
        public ConfigKeyValueCollection GetConfig(string section)
		{
			ConfigKeyValueCollection keyValues = new ConfigKeyValueCollection();

			if (_doc != null)
			{
				XmlElement appSettings = _doc.DocumentElement; // root element is settings element
				if (appSettings != null)
				{
					foreach (XmlNode node in appSettings.ChildNodes)
					{
						if (node is XmlElement)
						{
							XmlElement element = (XmlElement) node;
							keyValues.Add(element.GetAttribute(KEY_NAME),
								element.GetAttribute(VALUE_NAME));
						}
					}
				}
			}

			return keyValues;
		}
		
		/// <summary>
		/// Gets value of a setting of a given key
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The value of the given key, null if the key does not exists.</returns>
		public string GetAppSetting(string key)
		{
			string val = null;

			if (_doc != null)
			{
				XmlElement appSettings = _doc.DocumentElement;
				if (appSettings != null)
				{
					foreach (XmlNode node in appSettings.ChildNodes)
					{
						if (node is XmlElement)
						{
							XmlElement element = (XmlElement) node;
							if (element.GetAttribute(KEY_NAME) == key)
							{
								val = element.GetAttribute(VALUE_NAME);
                                if (val != null && string.IsNullOrEmpty(val.Trim()))
                                    val = null;
								break;
							}
						}
					}
				}
			}

			return val;
		}

		/// <summary>
		/// Sets a setting value of a given key
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		public void SetAppSetting(string key, string value)
		{
			if (_doc != null)
			{
				XmlElement appSettings = _doc.DocumentElement;
                if (appSettings != null)
                {
                    XmlElement appSetting = null;
                    foreach (XmlNode node in appSettings.ChildNodes)
                    {
                        if (node is XmlElement)
                        {
                            XmlElement element = (XmlElement)node;
                            if (element.GetAttribute(KEY_NAME) == key)
                            {
                                appSetting = element;
                                break;
                            }
                        }
                    }

                    if (appSetting == null)
                    {
                        appSetting = _doc.CreateElement(ADD);
                        appSetting.SetAttribute(KEY_NAME, key);
                        appSetting.SetAttribute(VALUE_NAME, value);
                        appSettings.AppendChild(appSetting);
                    }
                    else
                    {
                        appSetting.SetAttribute(VALUE_NAME, value);
                    }
                }
                else
                {
                    throw new Exception("The format of file " + CONFIG_FILE + " is incorrect.");
                }
			}		
		}

        /// <summary>
        /// Remove a setting item of a given key
        /// </summary>
        /// <param name="key">The key</param>
        public void RemoveAppSetting(string key)
        {
            if (_doc != null)
            {
                XmlElement appSettings = _doc.DocumentElement;
                if (appSettings != null)
                {
                    XmlElement appSetting = null;
                    foreach (XmlNode node in appSettings.ChildNodes)
                    {
                        if (node is XmlElement)
                        {
                            XmlElement element = (XmlElement)node;
                            if (element.GetAttribute(KEY_NAME) == key)
                            {
                                appSetting = element;
                                break;
                            }
                        }
                    }
                    if (appSetting != null)
                    {
                        appSettings.RemoveChild(appSetting);
                    }
                }
                else
                {
                    throw new Exception("The format of file " + CONFIG_FILE + " is incorrect.");
                }
            }
        }

        public string ToXML()
        {
            string xmlString = null;
            if (_doc != null)
            {
                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                _doc.WriteTo(tx);
                xmlString = sw.ToString();
            }

            return xmlString;
        }

		/// <summary>
		/// Flush the changes to app settings by writing them back to
		/// the config file
		/// </summary>
		public void Flush()
		{			
			XmlTextWriter writer = new XmlTextWriter(_configFileName , null);
			writer.Formatting = Formatting.Indented;					
			_doc.WriteTo(writer);					
			writer.Flush();
			writer.Close();
		}
	}
}
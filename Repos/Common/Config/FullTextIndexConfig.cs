/*
* @(#)FullTextIndexConfig.cs
*
* Copyright (c) 2003-2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Config
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Web;
	using System.Threading;
	using System.Text;
	using System.Reflection;
	using System.Collections.Specialized;

	using Newtera.Common.Core;

	/// <summary> 
	/// A class that reads/writes the full-text index settings of a full-text index configuration file.
	/// </summary>
	/// <version> 1.0.0 29 Apr 2014 </version>
	public class FullTextIndexConfig
	{
        private const string CONFIG_FILE = "fulltext_defs.xml";
        private const string CONFIG_DIR = @"\Config\";
		private const string KEY_NAME = "key";
		private const string VALUE_NAME = "value";
		private const string ADD = "add";

		private string _configFileName;
		private XmlDocument _doc;
		
		/// <summary>
		/// Initiate an instance of FullTextIndexConfig from a xml file
		/// </summary>
		/// <remarks>It will locate the config file automatically</remarks>
		public FullTextIndexConfig()
		{
            _configFileName = NewteraNameSpace.GetAppHomeDir() + FullTextIndexConfig.CONFIG_DIR + FullTextIndexConfig.CONFIG_FILE;
			
			if (File.Exists(_configFileName))
			{
				_doc = new XmlDocument();
				_doc.Load(_configFileName);
			}
		}

        /// <summary>
        /// Initiate an instance of FullTextIndexConfig from a xml string
        /// </summary>
        /// <remarks>It will locate the config file automatically</remarks>
        public FullTextIndexConfig(string xmlString)
        {
            _doc = new XmlDocument();
            _doc.LoadXml(xmlString);
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
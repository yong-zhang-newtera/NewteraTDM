/*
* @(#)AppConfig.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
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
	/// A class that reads/writes the AppSettings of an application's configuration file.
	/// </summary>
	/// <version> 1.0.0 30 Jan 2004 </version>
	/// <author> Yong Zhang </author>
	/// <remarks>
	/// Since System.Configuration provided by .net framework is read-only,
	/// this class provides the write ability to an application's config file.
    /// 
    /// Deprecated class, REMOVE
	/// </remarks>
	public class AppConfig
	{
		private const string WEB_CONFIG = "NewteraServer.exe.config";
		private const string APP_SETTINGS = "appSettings";
		private const string KEY_NAME = "key";
		private const string VALUE_NAME = "value";
		private const string ADD = "add";

		private string _configFileName;
		private XmlDocument _doc;
		
		/// <summary>
		/// Initiate an instance of AppConfig
		/// </summary>
		/// <remarks>It will locate the config file automatically</remarks>
		public AppConfig()
		{
			Newtera.Common.MetaData.Principal.CustomPrincipal customPrincipal = Thread.CurrentPrincipal as Newtera.Common.MetaData.Principal.CustomPrincipal;

			if (customPrincipal != null && customPrincipal.IsServerSide)
			{
				// The client is a web app or web service
				_configFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), WEB_CONFIG);
			}
			else
			{
				// it is a Window form or console application, get name of executable for the application
				_configFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ((Assembly.GetEntryAssembly()).GetName()).Name + ".exe.config");
			}
			
			if (File.Exists(_configFileName))
			{
				_doc = new XmlDocument();
				_doc.Load(_configFileName);
			}
			else
            {
				throw new Exception($"Unable to find a config file at {_configFileName}");
            }
		}

		/// <summary>
		/// Initiate an instance of AppConfig
		/// </summary>
		/// <param name="filePath">The config file path.</param>
		public AppConfig(string filePath)
		{			
			_configFileName = filePath;

			if (File.Exists(_configFileName))
			{
				_doc = new XmlDocument();
				_doc.Load(_configFileName);
			}
		}

		/// <summary>
		/// Gets the configurations of a given section
		/// </summary>
		/// <param name="section">The section name</param>
		/// <returns>A ConfigKeyValueCollection instance</returns>
		public ConfigKeyValueCollection GetConfig(string section)
		{
			ConfigKeyValueCollection keyValues = new ConfigKeyValueCollection();

			if (_doc != null)
			{
				XmlElement appSettings = _doc.DocumentElement[section];
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
		/// Gets value of an app setting of a given key
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The value of the given key, null if the key does not exists.</returns>
		public string GetAppSetting(string key)
		{
			string val = null;

			if (_doc != null)
			{
				XmlElement appSettings = _doc.DocumentElement[APP_SETTINGS];
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
		/// Sets an AppSetting value of a given key
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		public void SetAppSetting(string key, string value)
		{
			if (_doc != null)
			{
				XmlElement appSettings = _doc.DocumentElement[APP_SETTINGS];
				if (appSettings == null)
				{
					appSettings = _doc.CreateElement(APP_SETTINGS);
					_doc.DocumentElement.AppendChild(appSettings);
				}

				XmlElement appSetting = null;
				foreach (XmlNode node in appSettings.ChildNodes)
				{
					if (node is XmlElement)
					{
						XmlElement element = (XmlElement) node;
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
		}

		/// <summary>
		/// Gets value of a given key in a given section
		/// </summary>
		/// <param name="section">The section</param>
		/// <param name="key">The key</param>
		/// <returns>The value of the given key, null if the key does not exists.</returns>
		public string GetSetting(string section, string key)
		{
			string val = null;

			if (_doc != null)
			{
				XmlElement settings = _doc.DocumentElement[section];
				if (settings != null)
				{
					foreach (XmlNode node in settings.ChildNodes)
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
		/// Sets a value of a given key in a given section.
		/// </summary>
		/// <param name="section">The section</param>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		public void SetSetting(string section, string key, string value)
		{
			if (_doc != null)
			{
				XmlElement settings = _doc.DocumentElement[section];
				if (settings == null)
				{
					settings = _doc.CreateElement(section);
					_doc.DocumentElement.AppendChild(settings);
				}

				XmlElement setting = null;
				foreach (XmlNode node in settings.ChildNodes)
				{
					if (node is XmlElement)
					{
						XmlElement element = (XmlElement) node;
						if (element.GetAttribute(KEY_NAME) == key)
						{
							setting = element;
							break;
						}
					}
				}

				if (setting == null)
				{
					setting = _doc.CreateElement(ADD);
					setting.SetAttribute(KEY_NAME, key);
					setting.SetAttribute(VALUE_NAME, value);
					settings.AppendChild(setting);
				}
				else
				{
					setting.SetAttribute(VALUE_NAME, value);
				}
			}		
		}

        /// <summary>
        /// Remove a setting from a given section
        /// </summary>
        /// <param name="section">The section name</param>
        /// <param name="key">The key</param>
        public void RemoveSetting(string section, string key)
        {
            if (_doc != null)
            {
                XmlElement settings = _doc.DocumentElement[section];
                if (settings != null)
                {
                    XmlElement setting = null;
                    foreach (XmlNode node in settings.ChildNodes)
                    {
                        if (node is XmlElement)
                        {
                            XmlElement element = (XmlElement)node;
                            if (element.GetAttribute(KEY_NAME) == key)
                            {
                                setting = element;
                                break;
                            }
                        }
                    }

                    if (setting != null)
                    {
                        settings.RemoveChild(setting);
                    }
                }
            }	
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
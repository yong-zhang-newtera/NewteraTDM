/*
* @(#)ClusterServerConfig.cs
*
* Copyright (c) 2018 Newtera, Inc. All rights reserved.
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
	/// A class that manages the cluster server configuration file.
	/// </summary>
	/// <version> 1.0.0 4 Dec 2018 </version>
	public class ClusterServerConfig
	{
        private const string CONFIG_FILE = "cluster_servers.xml";
        private const string CONFIG_DIR = @"\Config\";
		private const string KEY_NAME = "key";
		private const string VALUE_NAME = "value";

        private string _configFileName;
		private XmlDocument _doc;
        private ConfigKeyValueCollection _serverDefs;

        /// <summary>
        /// Singleton's private instance.
        /// </summary>
        private static ClusterServerConfig theConfig;

        static ClusterServerConfig()
        {
            theConfig = new ClusterServerConfig();
        }

        /// <summary>
        /// The private constructor.
        /// </summary>
        private ClusterServerConfig()
        {
            _configFileName = NewteraNameSpace.GetAppHomeDir() + ClusterServerConfig.CONFIG_DIR + ClusterServerConfig.CONFIG_FILE;
            _serverDefs = null;

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
        /// Gets the ClusterServerConfig instance.
        /// </summary>
        /// <returns> The ClusterServerConfig instance.</returns>
        static public ClusterServerConfig Instance
        {
            get
            {
                return theConfig;
            }
        }

        public bool MoreThanOneServers
        {
            get
            {
                if (this.GetServerDefs().Count > 1)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets the server definitions as a key-value collection
        /// </summary>
        /// <returns>A ConfigKeyValueCollection instance</returns>
        public ConfigKeyValueCollection GetServerDefs()
		{
            if (this._serverDefs == null)
            {
                this._serverDefs = new ConfigKeyValueCollection();

                if (_doc != null)
                {
                    XmlElement appSettings = _doc.DocumentElement; // root element is settings element
                    if (appSettings != null)
                    {
                        foreach (XmlNode node in appSettings.ChildNodes)
                        {
                            if (node is XmlElement)
                            {
                                XmlElement element = (XmlElement)node;
                                this._serverDefs.Add(element.GetAttribute(KEY_NAME),
                                    element.GetAttribute(VALUE_NAME));
                            }
                        }
                    }
                }
            }

			return _serverDefs;
		}
	}
}
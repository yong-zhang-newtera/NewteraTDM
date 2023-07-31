/*
* @(#)RedisConfig.cs
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
    /// A class that manages the redis config.
    /// </summary>
    public class RedisConfig
    {
        private const string CONFIG_FILE = "redis_config.xml";
        private const string CONFIG_DIR = @"Config\";
        private const string KEY_NAME = "key";
        private const string VALUE_NAME = "value";
        private const string ENABLED = "Enabled";
        private const string CONNECTION_STRING = "ConnectionString";

        private ConfigKeyValueCollection _configSettings;

        /// <summary>
        /// Singleton's private instance.
        /// </summary>
        private static RedisConfig theConfig;

        static RedisConfig()
        {
            theConfig = new RedisConfig();
        }

        /// <summary>
        /// The private constructor.
        /// </summary>
        private RedisConfig()
        {
        }

        /// <summary>
        /// Gets the RedisConfig instance.
        /// </summary>
        /// <returns> The RedisConfig instance.</returns>
        static public RedisConfig Instance
        {
            get
            {
                return theConfig;
            }
        }

        public bool DistributedCacheEnabled
        {
            get
            {
                var settings = this.GetConfigSettings();
                if (!string.IsNullOrEmpty(settings[ENABLED]) &&
                    settings[ENABLED].ToUpper() == "TRUE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets redis connection string
        /// </summary>
        /// <returns>A string for connecting to redis server</returns>
        public string ConnectionString
        {
            get
            {
                var configSettings = GetConfigSettings();

                return configSettings.Get(CONNECTION_STRING);
            }
        }

        private ConfigKeyValueCollection GetConfigSettings()
        {
            if (this._configSettings == null)
            {
                this._configSettings = new ConfigKeyValueCollection();

                string configFileName = NewteraNameSpace.GetAppHomeDir() + RedisConfig.CONFIG_DIR + RedisConfig.CONFIG_FILE;

                XmlDocument doc = new XmlDocument();
                if (File.Exists(configFileName))
                {
                    doc.Load(configFileName);
                    XmlElement appSettings = doc.DocumentElement; // root element is settings element
                    if (appSettings != null)
                    {
                        foreach (XmlNode node in appSettings.ChildNodes)
                        {
                            if (node is XmlElement)
                            {
                                XmlElement element = (XmlElement)node;
                                this._configSettings.Add(element.GetAttribute(KEY_NAME),
                                    element.GetAttribute(VALUE_NAME));
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception($"Unable to find redis configuration file at {configFileName}.");
                }
            }

            return this._configSettings;
        }
    }
}


/*
* @(#)BlobStorageConfig.cs
*
* Copyright (c) 2023 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Newtera.Common.Core;

namespace Newtera.Common.Config
{
	/// <summary> 
	/// A class that reads/writes the options of a blob storage configuration file.
	/// </summary>
	/// <version> 1.0.0 11 Apr 2023 </version>
	public class BlobStorageConfig
	{
        private const string CONFIG_FILE = "blob_storage_config.xml";
        private const string CONFIG_DIR = @"\Config\";

        private List<BucketConfig> _bucketConfigs = null;

        /// <summary>
        /// Singleton's private instance.
        /// </summary>
        private static BlobStorageConfig theConfig;

        static BlobStorageConfig()
        {
            theConfig = new BlobStorageConfig();
        }

        /// <summary>
        /// The private constructor.
        /// </summary>
        private BlobStorageConfig()
        {
            var configFileName = NewteraNameSpace.GetAppHomeDir() + BlobStorageConfig.CONFIG_DIR + BlobStorageConfig.CONFIG_FILE;

            if (File.Exists(configFileName))
            {
                var doc = new XmlDocument();
                doc.Load(configFileName);
                _bucketConfigs = BuildBucketConfigs(doc);
            }
            else
            {
                ErrorLog.Instance.WriteLine($"The file {configFileName} doesn't exist.");
            }
        }

        /// <summary>
        /// Gets the BlobStorageConfig instance.
        /// </summary>
        /// <returns> The BlobStorageConfig instance.</returns>
        static public BlobStorageConfig Instance
        {
            get
            {
                return theConfig;
            }
        }

        public List<BucketConfig> BucketConfigs
		{
            get
            {
                return this._bucketConfigs;
            }
		}

        private List<BucketConfig> BuildBucketConfigs(XmlDocument doc)
        {
            List<BucketConfig> bucketConfigs = new List<BucketConfig>();

            foreach (XmlNode node in doc?.DocumentElement?.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    var bucketConfig = new BucketConfig();
                    bucketConfig.Name = element.GetAttribute("Name");
                    bucketConfig.Type = element.GetAttribute("Type");
                    bucketConfig.ServiceUrl = element.GetAttribute("ServiceUrl");
                    bucketConfig.Path = element.GetAttribute("Path");
                    bucketConfig.PublicKey = element.GetAttribute("PublicKey");
                    bucketConfig.SecretKey = element.GetAttribute("SecretKey");
                    bucketConfig.SecretKey = element.GetAttribute("SecretKey");
                    bucketConfig.ForDBSchema = element.GetAttribute("ForDBSchema");
                    bucketConfig.ForDBClass = element.GetAttribute("ForDBClass");

                    bucketConfigs.Add(bucketConfig);
                }
            }

            return bucketConfigs;
        }
	}

    public class BucketConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string ServiceUrl { get; set; }
        public string Path { get; set; }
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public TimeSpan Timeout { get; set; }
        public string ForDBSchema { get; set; }
        public string ForDBClass { get; set; }
    }
}
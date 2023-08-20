/*
* @(#)StorageProviderFactory.cs
*
* Copyright (c) 2023 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Linq;
using System.Collections.Generic;
using Newtera.BlobStorage.LocalStorage;
using Newtera.BlobStorage.S3Storage;
using Newtera.Common.Config;

namespace Newtera.BlobStorage
{
	/// <summary> 
	/// A singleton class that create and return a storage provider
	/// </summary>
	/// <version> 1.0.0 11 Apr 2023 </version>
	public class StorageProviderFactory
    {
        private IDictionary<string, IStorageProvider> _mappings;

        /// <summary>
        /// Singleton's private instance.
        /// </summary>
        private static StorageProviderFactory theFactory;

        static StorageProviderFactory()
        {
            theFactory = new StorageProviderFactory();
        }

        /// <summary>
        /// The private constructor.
        /// </summary>
        private StorageProviderFactory()
        {
            this._mappings = new Dictionary<string, IStorageProvider>();
        }

        /// <summary>
        /// Gets the BlobStorageConfig instance.
        /// </summary>
        /// <returns> The BlobStorageConfig instance.</returns>
        static public StorageProviderFactory Instance
        {
            get
            {
                return theFactory;
            }
        }

        public IStorageProvider Create(string schemaName, string className)
        {
            lock (this)
            {
                var key = schemaName + className;
                if (!this._mappings.ContainsKey(key))
                {
                    var storageProvider = CreateStorageProvider(schemaName, className);
                    if (storageProvider != null)
                    {
                        this._mappings[key] = storageProvider;
                    }
                    else
                    {
                        throw new Exception($"Unable to create a storage provider for {schemaName} and {className}.");
                    }
                }

                return this._mappings[key];
            }
        }

        public IStorageProvider Create(BucketConfig bucket)
        {
            var storageProvider = CreateStorageProvider(bucket);
            if (storageProvider != null)
            {
                return storageProvider;
            }
            else
            {
                throw new Exception($"Unable to create a storage provider for {bucket.Name} and {bucket.Type}.");
            }
        }

        private IStorageProvider CreateStorageProvider(string schemaName, string className)
        {
            var buckets = BlobStorageConfig.Instance.BucketConfigs;

            BucketConfig bucket = MatchBucket(buckets, schemaName, className);
            if (bucket == null)
            {
                throw new Exception($"Unable to match a bucket config for database {schemaName} and class {className}.");
            }

            var storageProvider = CreateStorageProvider(bucket);

            return storageProvider;
        }

        private IStorageProvider CreateStorageProvider(BucketConfig bucket)
        {
            IStorageProvider storageProvider;

            switch (bucket.Type)
            {
                case "Local":
                    storageProvider = new LocalStorageProvider(bucket.Path);

                    break;

                case "S3":
                    S3ProviderOptions options = new S3ProviderOptions();
                    options.Bucket = bucket.Name;
                    options.ServiceUrl = bucket.ServiceUrl;
                    options.PublicKey = bucket.PublicKey;
                    options.SecretKey = bucket.SecretKey;

                    storageProvider = new S3StorageProvider(options);

                    break;

                default:
                    throw new Exception($"Unknown bucket type {bucket.Type}");
            }

            return storageProvider;
        }

        private BucketConfig MatchBucket(List<BucketConfig> buckets, string schemaName, string className)
        {
            // matching rules:
            // 1. match the bucket with the same database name or bucket with empty database
            // 2. match the bucket with the same class name or bucket with empty class name
            // 3. if there are more than one matched buckets, pick the first one in the list
            BucketConfig bucket = buckets?.OrderBy(b => String.IsNullOrEmpty(b.ForDBSchema))
                .ThenBy(b => String.IsNullOrEmpty(b.ForDBClass))
                .Where(b => (b.ForDBSchema == schemaName || string.IsNullOrEmpty(b.ForDBSchema)))
                .Where(b => b.ForDBClass == className || string.IsNullOrEmpty(b.ForDBClass))
                .FirstOrDefault();

            return bucket;
        }
    }
}
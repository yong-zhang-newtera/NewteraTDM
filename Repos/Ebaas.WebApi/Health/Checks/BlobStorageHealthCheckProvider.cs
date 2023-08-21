using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtera.BlobStorage;
using Newtera.Common.Config;

namespace Ebaas.WebApi.Health
{
    /// <summary>
    /// Provides health info about the blob storage.
    /// </summary>
    public class BlobStorageHealthCheckProvider : IHealthCheckProvider
    {
        /// <summary>
        /// Tries to connect to the application's main database.
        /// </summary>
        public async Task<List<HealthCheckItemResult>> GetHealthCheckAsync()
        {
            var results = new List<HealthCheckItemResult>();
            HealthCheckItemResult result = null;
            BucketConfig currentBucket = null;
            try
            {
                var bucketConfigs = BlobStorageConfig.Instance.BucketConfigs;
                int index = 0;
                foreach (BucketConfig bucketConfig in bucketConfigs)
                {
                    currentBucket = bucketConfig;
                    result = new HealthCheckItemResult($"Blob Storage:({bucketConfig.Name})", (SortOrder + index++), "Checks Blob Storage", "Checks whether Blob storage can be accessed.", bucketConfig.Type);
                    results.Add(result);
                    var storageProvider = StorageProviderFactory.Instance.Create(bucketConfig);
                    bool exist = await storageProvider.DoesBlobExistAsync("Test", "Test");

                    result.HealthState = HealthState.Healthy;
                    result.Messages.Add($"Successfully connecting to a {currentBucket.Type} Blob Storage at {GetStorageLocation(currentBucket)}.");
                }
            }
            catch (Exception ex)
            {
                if (result == null)
                {
                    result = new HealthCheckItemResult(nameof(BlobStorageHealthCheckProvider), SortOrder, "Checks Blob Storage", "Checks whether Blob storage can be accessed.", "Unknown");
                    results.Add(result);
                }
                result.HealthState = HealthState.Unhealthy;
                result.Messages.Add($"Error connecting to a {currentBucket.Type} Blob Storage at {GetStorageLocation(currentBucket)} due to {ex.Message} with detail {ex.StackTrace}.");
            }

            return results;
        }

        /// <summary>
        /// Defines the order of this provider in the results.
        /// </summary>
        public int SortOrder { get; } = 10;

        private string GetStorageLocation(BucketConfig bucketConfig)
        {
            if (bucketConfig.Type == "Local")
            {
                return bucketConfig.Path;
            }
            else
            {
                return bucketConfig.ServiceUrl;
            }
        }
    }
}
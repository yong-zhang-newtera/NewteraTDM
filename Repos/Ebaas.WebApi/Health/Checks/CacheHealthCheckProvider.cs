using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtera.Common.Config;
using Newtera.Server.Util;

namespace Ebaas.WebApi.Health.Checks
{
    /// <summary>
    /// Provides health info about the Cache.
    /// </summary>
    public class CacheHealthCheckProvider : IHealthCheckProvider
    {
        /// <summary>
        /// Tries to connect to the application's main database.
        /// </summary>
        public Task<List<HealthCheckItemResult>> GetHealthCheckAsync()
        {
            var redisEnabled = RedisConfig.Instance.DistributedCacheEnabled;
            var results = new List<HealthCheckItemResult>();
            var result = new HealthCheckItemResult("Data Cache", SortOrder, "Checks Cache", "Checks whether Cache can be accessed.", redisEnabled ? "Redis" : "In-Memory");
            results.Add(result);

            try
            {
                if (redisEnabled)
                {
                    var cache = KeyValueStoreFactory.TheInstance.Create("Check");
                    cache.Add("foo", "bar");
                    cache.Remove("foo");
                    result.HealthState = HealthState.Healthy;
                    result.Messages.Add($"Successfully connecting to Redis at {RedisConfig.Instance.ConnectionString}.");
                }
                else
                {
                    result.HealthState = HealthState.Healthy;
                    result.Messages.Add($"Distributed cache isn't enabled. In-Memory cache is used.");
                }
            }
            catch (Exception ex)
            {
                result.HealthState = HealthState.Unhealthy;
                result.Messages.Add($"Error connecting to Redis at location {RedisConfig.Instance.ConnectionString} due to {ex.Message} with detail {ex.StackTrace}.");
            }

            return Task.FromResult(results);
        }

        /// <summary>
        /// Defines the order of this provider in the results.
        /// </summary>
        public int SortOrder { get; } = 30;
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtera.Common.Config;
using Newtera.ElasticSearchIndexer;

namespace Newtera.WebApi.Health
{
    /// <summary>
    /// Provides health info about the full text search.
    /// </summary>
    public class FullTextSearchHealthCheckProvider : IHealthCheckProvider
    {
        /// <summary>
        /// Tries to connect to the application's main database.
        /// </summary>
        public Task<List<HealthCheckItemResult>> GetHealthCheckAsync()
        {
            var elasticSearchEnabled = ElasticsearchConfig.Instance.IsElasticsearchEnabled;
            var results = new List<HealthCheckItemResult>();
            var result = new HealthCheckItemResult("Full-Text Search", SortOrder, "Checks full-text search", "Checks whether full-text search can be accessed.", elasticSearchEnabled ? "Elastic Search" : "Disabled");
            results.Add(result);

            try
            {
                if (elasticSearchEnabled)
                {
                    var status = ElasticSearchWrapper.Ping();
                    if (!status)
                    {
                        throw new Exception("Elastic Search can not be accessed.");
                    }
                    result.HealthState = HealthState.Healthy;
                    result.Messages.Add($"Successfully connecting to Elastic Search at {ElasticsearchConfig.Instance.ElasticsearchURL}.");
                }
                else
                {
                    result.HealthState = HealthState.Healthy;
                    result.Messages.Add($"Full-text search is disabled. To enable full-text search, please setup Elastic Search.");
                }
            }
            catch (Exception ex)
            {
                result.HealthState = HealthState.Unhealthy;
                result.Messages.Add($"Error connecting to Elastic Search at {ElasticsearchConfig.Instance.ElasticsearchURL} due to {ex.Message} with detail {ex.StackTrace}.");
            }

            return Task.FromResult(results);
        }

        /// <summary>
        /// Defines the order of this provider in the results.
        /// </summary>
        public int SortOrder { get; } = 40;
    }
}
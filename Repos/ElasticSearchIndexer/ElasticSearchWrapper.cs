using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Nest;
using Nest.JsonNetSerializer;

using Newtera.Common.Config;
using Newtera.Common.Core;
using Elasticsearch.Net;

namespace Newtera.ElasticSearchIndexer
{
    /// <summary>
    /// wrapper for ElasticSearch API
    /// </summary>
    public class ElasticSearchWrapper
    {
        private const string INDEX_PREFIX = "ebass";
        private const string OBJ_ID = "obj_id";

        static private ElasticClient _client;
        static private object _lock = new object();

        static private ElasticClient GetClient()
        {
            lock (_lock)
            {
                if (_client == null)
                {
                    var pool = new SingleNodeConnectionPool(new Uri(ElasticsearchConfig.Instance.ElasticsearchURL));
                    var connectionSettings = new ConnectionSettings(pool, JsonNetSerializer.Default)
                        .BasicAuthentication(ElasticsearchConfig.Instance.User, ElasticsearchConfig.Instance.Password)
                        .RequestTimeout(TimeSpan.FromMinutes(2));
                    _client = new ElasticClient(connectionSettings);
                }

                return _client;
            }
        }

        private static SearchRequest BuildSearchRequest(string indexName, string searchText, int? startRow = null, int? pageSize = null)
        {
            var request = new SearchRequest(indexName)
            {
                //Query = new TermQuery("catch_all") { Value = searchText }
            };

            if (startRow != null)
            {
                request.From = startRow;
                request.Size = pageSize ?? 100;
            }

            return request;
        }

        static private string GetIndexName(string schemaName, string className)
        {
            return ElasticSearchWrapper.INDEX_PREFIX + "_" + schemaName.ToLower() + "_" + className.ToLower();
        }

        public static bool IsIndexExist(string schemaName, string className)
        {
            var client = GetClient();
            string indexName = GetIndexName(schemaName, className);
            return client.Indices.Exists(indexName).Exists;
        }

        public static async Task DeleteIndex(string schemaName, string className)
        {
            var client = GetClient();

            string indexName = GetIndexName(schemaName, className);

            var response = await client.Indices.DeleteAsync(indexName);

            if (!response.IsValid)
            {
                ErrorLog.Instance.WriteLine($"Delete Index {indexName} failed with error {response.ServerError}.");
            }
        }

        public static async Task CreateDocumentIndexes(string schemaName, string className, List<JObject> documents)
        {
            foreach (JObject document in documents)
            {
                var documentId = document.GetValue(OBJ_ID).ToString();
                await CreateDocumentIndex(schemaName, className, documentId, document);
            }
        }

        public static async Task CreateDocumentIndex(string schemaName, string className,  string documentId, JObject document)
        {
            var client = GetClient();

            string indexName = GetIndexName(schemaName, className);

            var indexRequest = new IndexRequest<JObject>(document, indexName, documentId);

            var response = await client.IndexAsync(indexRequest);

            if (!response.IsValid)
            {
                ErrorLog.Instance.WriteLine($"Index document with id {documentId} failed with error {response.ServerError}.");
            }
        }

        public static async Task UpdateDocumentIndex(string schemaName, string className, string documentId, JObject document)
        {
            var client = GetClient();

            string indexName = GetIndexName(schemaName, className);

            var response = await client.UpdateAsync<JObject>(documentId, u => u
                .Index(indexName)
                .Doc(document));

            if (!response.IsValid)
            {
                ErrorLog.Instance.WriteLine($"Update document with id {documentId} failed with error {response.ServerError}.");
            }
        }

        public static async Task DeleteDocumentIndex(string schemaName, string className, string documentId)
        {
            var client = GetClient();

            string indexName = GetIndexName(schemaName, className);

            var response = await client.DeleteAsync<JObject>(documentId);
 
            if (!response.IsValid)
            {
                ErrorLog.Instance.WriteLine($"Delete document with id {documentId} failed with error {response.ServerError}.");
            }
        }

        public static async Task<long> GetSearchCount(string schemaName, string className, string searchText)
        {
            long count = 0;

            if (IsIndexExist(schemaName, className))
            {
                var client = GetClient();

                string indexName = GetIndexName(schemaName, className);

                var request = BuildSearchRequest(indexName, searchText);

                var response = await client.SearchAsync<JObject>(request);

                if (response.IsValid)
                {
                    count = response.Total;
                }
                else
                {
                    ErrorLog.Instance.WriteLine($"Search documents failed with error {response.ServerError}.");
                }
            }

            return count;
        }

        public static async Task<IReadOnlyCollection<JObject>> GetSearchResult(string schemaName, string className, string searchText, int? startRow = null, int? pageSize = null)
        {
            IReadOnlyCollection<JObject> result = new List<JObject>();

            if (IsIndexExist(schemaName, className))
            {
                var client = GetClient();

                string indexName = GetIndexName(schemaName, className);

                var request = BuildSearchRequest(indexName, searchText, startRow, pageSize);

                try
                {
                    var response = await client.SearchAsync<object>(request);

                    if (response.IsValid)
                    {
                        //result = response.Documents;
                    }
                    else
                    {
                        ErrorLog.Instance.WriteLine($"Search documents failed with error {response.ServerError}.");
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine($"Search documents failed with error {ex.Message}.");
                }
            }

            return result;
        }
    }
}

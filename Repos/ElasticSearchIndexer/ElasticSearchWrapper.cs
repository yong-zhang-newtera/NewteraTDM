using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Common.Config;

namespace Newtera.ElasticSearchIndexer
{
    /// <summary>
    /// wrapper for ElasticSearch API
    /// </summary>
    public class ElasticSearchWrapper
    {
        private const string INDEX_PREFIX = "ebass";
        private const string OBJ_ID = "obj_id";

        // port 9200

        static private JObject PostAPICall(string apiUrl, JObject body)
        {
            JObject apiResult = null;

            string baseUri =  ElasticsearchConfig.Instance.ElasticsearchURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                client.MaxResponseContentBufferSize = 556000;

                StringContent postContent = null;

                if (body != null)
                {
                    var jsonString = JsonConvert.SerializeObject(body, Newtonsoft.Json.Formatting.Indented);
                    //ErrorLog.Instance.WriteLine(jsonString);
                    postContent = new StringContent(jsonString, Encoding.UTF8);

                    postContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                var response = client.PostAsync(apiUrl, postContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string json = responseContent.ReadAsStringAsync().Result;

                    apiResult = JsonConvert.DeserializeObject<JObject>(json);
                }
                else
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
            }

            return apiResult;
        }

        static private string PutAPICall(string apiUrl, JObject document)
        {
            string apiResult = null;

            string baseUri = ElasticsearchConfig.Instance.ElasticsearchURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                client.MaxResponseContentBufferSize = 556000;

                StringContent postContent = null;

                if (document != null)
                {
                    var jsonString = JsonConvert.SerializeObject(document, Newtonsoft.Json.Formatting.Indented);
                    //ErrorLog.Instance.WriteLine(jsonString);
                    postContent = new StringContent(jsonString, Encoding.UTF8);
                    
                    postContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                var response = client.PutAsync(apiUrl, postContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    apiResult = responseContent.ReadAsStringAsync().Result;
                }
                else
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
            }

            return apiResult;
        }

  
        static private string DeleteAPICall(string apiUrl)
        {
            string apiResult = null;

            string baseUri = ElasticsearchConfig.Instance.ElasticsearchURL;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                client.MaxResponseContentBufferSize = 556000;

                var response = client.DeleteAsync(apiUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    apiResult = responseContent.ReadAsStringAsync().Result;
                }
                else
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
            }

            return apiResult;
        }

        static private string GetIndexName(string schemaName, string className)
        {
            return ElasticSearchWrapper.INDEX_PREFIX + "_" + schemaName.ToLower() + "_" + className.ToLower();
        }

        public static bool IsIndexExist(string schemaName, string className)
        {
            try
            {
                string indexName = GetIndexName(schemaName, className);

                string baseUri = ElasticsearchConfig.Instance.ElasticsearchURL;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUri);
                    client.Timeout = new TimeSpan(TimeSpan.TicksPerHour);
                    client.MaxResponseContentBufferSize = 556000;

                    var response = client.GetAsync(indexName).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DeleteIndex(string schemaName, string className)
        {
            // DELETE /twitter
            string indexName = GetIndexName(schemaName, className);

            DeleteAPICall(indexName);
        }

        public static void CreateDocumentMapping(string schemaName, string className, JObject documentMappping)
        {
            string indexName = GetIndexName(schemaName, className);

            string apiUrl = indexName;

            // call elastic search
            string res = PutAPICall(apiUrl, documentMappping);
        }

        public static void CreateDocumentIndexes(string schemaName, string className, List<JObject> documents)
        {
            // POST /customer/_doc/_bulk?pretty
            // { "index":{ "_id":"1"} }
            // { "name": "John Doe" }
            // { "index":{ "_id":"2"} }
            // { "name": "Jane Doe" }

            string documentId = null;

            foreach (JObject document in documents)
            {
                documentId = document.GetValue(OBJ_ID).ToString();
                CreateDocumentIndex(schemaName, className, documentId, document);
            }
        }

        public static void CreateDocumentIndex(string schemaName, string className,  string documentId, JObject document)
        {
            // PUT twitter/_doc/1
            // {
            //    "user" : "kimchy",
            // "post_date" : "2009-11-15T14:12:12",
            // "message" : "trying out Elasticsearch"
            // }

            string indexName = GetIndexName(schemaName, className);

            string apiUrl = indexName + @"/_doc/" + documentId;

            // call elastic search
            string res = PutAPICall(apiUrl, document);
        }

        public static void UpdateDocumentIndex(string schemaName, string className, string documentId, JObject document)
        {
            // PUT /customer/doc/1?pretty
            // {
            //    "name": "Jane Doe"
            // }

            string indexName = GetIndexName(schemaName, className);

            string apiUrl = indexName + @"/_doc/" + documentId;

            // call elastic search
            string res = PutAPICall(apiUrl, document);
        }

        public static void DeleteDocumentIndex(string schemaName, string className, string documentId)
        {
            // DELETE /twitter/_doc/1

            string indexName = GetIndexName(schemaName, className);

            string apiUrl = indexName + @"/_doc/" + documentId;

            // call elastic search
            string res = DeleteAPICall(apiUrl);
        }

        public static int GetSearchCount(string schemaName, string className, JObject queryBody)
        {
            int count = 0;

            if (IsIndexExist(schemaName, className))
            {
                string indexName = GetIndexName(schemaName, className);

                string apiUrl = indexName + @"/_doc/_count";

                // call elastic search
                JObject jsonObj = PostAPICall(apiUrl, queryBody);

                count = jsonObj["count"].Value<int>();
            }

            return count;
        }

        public static JObject GetSearchResult(string schemaName, string className, JObject queryBody)
        {
            JObject result = new JObject();

            if (IsIndexExist(schemaName, className))
            {
                string indexName = GetIndexName(schemaName, className);

                string apiUrl = indexName + @"/_doc/_search";

                // call elastic search
                result = PostAPICall(apiUrl, queryBody);
            }

            return result;
        }

        public static JObject GetSuggestions(string schemaName, string className, JObject queryBody)
        {
            JObject result = new JObject();

            if (IsIndexExist(schemaName, className))
            {
                string indexName = GetIndexName(schemaName, className);

                string apiUrl = indexName + @"/_search";

                // call elastic search
                result = PostAPICall(apiUrl, queryBody);
            }

            return result;
        }
    }
}

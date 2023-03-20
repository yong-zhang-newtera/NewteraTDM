using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Net;
using System.Text;
using System.Data;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Dynamic;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Web.Http.Description;

using Swashbuckle.Swagger.Annotations;

using Ebaas.WebApi.Infrastructure;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Ebaas.WebApi.Models;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Data;
using Newtera.ElasticSearchIndexer;
using Newtera.WebForm;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// The Full-text search Service is Elasticsearch to perform full-text search and return the search result in JSON
    /// </summary>
    public class FullTextSearchController : ApiController
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string SEARCH_TEXT = "searchtext";
        private const string COPY_TO_PROPERTY = "catch_all";
        private const string COMPLETION_PROPERTY = "completion_all";
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";
        private const string FILTER = "filter";
        private const string SORT_FIELD = "sortfield";
        private const string SORT_REVERSE = "sortreverse";

        /// <summary>
        /// Get information indicating whether the server uses an external full text search engine (e.g. ElasticSearch) or not
        /// </summary>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/search/enabled")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool), Description = "return true if Elasticsearch is enabled, false otherwise.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public HttpResponseMessage HasExternalFullTextSearch()
        {
            try
            {
                bool status = Newtera.Common.Config.ElasticsearchConfig.Instance.IsElasticsearchEnabled;

                return Request.CreateResponse(HttpStatusCode.OK, status); ;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                resp.Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }

        /// <summary>
        /// Get suggestions for a typed search text
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="typedText">The text an user typed</param>
        /// <param name="size">Size of the suggestions, default to 20</param>
        /// <returns>A json array of suggestion strings</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/search/{schemaName}/suggestions")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A json array of suggestion strings")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> getSuggestions(string schemaName, string typedText = null, int? size = null)
        {
            try
            {
                List<string> suggestions = new List<string>();
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string searchText = GetParamValue(parameters, SEARCH_TEXT, "");
                int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 20));
                JObject queryBody = FullTextSearchHelper.BuildGetSuggestionsBody(searchText, 5);

                await Task.Factory.StartNew(() =>
                {
                    SchemaModelElementCollection classElements = FullTextSearchHelper.GetAccessibleClasses(schemaName);

                    foreach (SchemaModelElement classElement in classElements)
                    {
                        StringCollection tmpList = FullTextSearchHelper.GetCompletionSuggestionsSearchEngine(schemaName, classElement.Name, queryBody);

                        if (tmpList.Count > 0)
                        {
                            foreach (string suggestion in tmpList)
                            {
                                if (!suggestions.Contains(suggestion) && suggestions.Count < pageSize)
                                    suggestions.Add(suggestion);
                            }

                            if (suggestions.Count >= pageSize)
                                break;
                        }
                    }
                    
                });

                return Ok(suggestions);

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Search the indexes of Elasticsearch created for the classes in a given schema and return a collection of
        /// json objects with hit count information.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="searchtext">The search text user entered</param>
        /// <returns>A collection of search count info objects in json</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/search/{schemaName}/counts")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A collection of SearchCountModel objects in json format")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> getSearchCounts(string schemaName, string searchtext = null)
        {
            try
            {
                List<SearchCountModel> countModels = new List<SearchCountModel>();
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string searchText = GetParamValue(parameters, SEARCH_TEXT, "");
                JObject queryBody = FullTextSearchHelper.BuildQueryBody(searchText, -1, -1);

                await Task.Factory.StartNew(() =>
                {
                    SchemaModelElementCollection classElements = FullTextSearchHelper.GetAccessibleClasses(schemaName);

                    foreach (SchemaModelElement classElement in classElements)
                    {
                        int count = ElasticSearchWrapper.GetSearchCount(schemaName, classElement.Name, queryBody);

                        if (count > 0)
                        {
                            SearchCountModel searchModel = new SearchCountModel();
                            searchModel.schemaName = schemaName;
                            searchModel.className = classElement.Name;
                            searchModel.classDisplayName = classElement.Caption;
                            searchModel.count = count;

                            countModels.Add(searchModel);
                        }
                    }
                });

                return Ok(countModels);
                
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Search the index of Elasticsearch created for the given class and given schema and return the result in a json object
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className" > A data class name such as Issues</param>
        /// <param name="searchtext">The search text user entered</param>
        /// <returns>A json object representing the search result</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/search/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A collection of data instances in json format")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> getSearchResult(string schemaName, string className, string view = null, int? from = null, int? size = null, string filter = null, string sortfield = null, string sortreverse = null)
        {
            try
            {
                List<JObject> instances = null;
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                        DataViewModel dataView;

                        dataView = con.MetaDataModel.GetDefaultDataView(className);

                        if (dataView != null)
                        {
                            int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 20));
                            int startRow = int.Parse(GetParamValue(parameters, START_ROW, 0));
                            int pageIndex = startRow / pageSize;
                            string filters = GetParamValue(parameters, FILTER, "");
                            string sortField = GetParamValue(parameters, SORT_FIELD, "");
                            bool sortReverse = bool.Parse(GetParamValue(parameters, SORT_REVERSE, "false"));

                            StringCollection instanceIds;
                            if (!string.IsNullOrEmpty(filters))
                            {
                                string searchText = queryHelper.GetSearchText(filters);
                                instanceIds = FullTextSearchHelper.GetInstanceIdsFromSearchEngine(schemaName, className, searchText, startRow, pageSize);

                                queryHelper.SetInstanceIds(dataView, instanceIds);
                            }
                            else
                                instanceIds = new StringCollection();

                            if (!string.IsNullOrEmpty(sortField))
                            {
                                queryHelper.SetSort(dataView, sortField, sortReverse);
                            }

                            string query = dataView.GetInstancesQuery(instanceIds);

                            //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                            //timer.Start();

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            XmlReader reader = cmd.ExecuteXMLReader();
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            //timer.Stop();
                            //ErrorLog.Instance.WriteLine("Query elapsed time is " + timer.Elapsed);

                            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                            {
                                InstanceView instanceView = new InstanceView(dataView, ds);

                                InstanceEditor instanceEditor = new InstanceEditor();
                                instanceEditor.EditInstance = instanceView;

                                count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);

                                instances = new List<JObject>();
                                JObject instance;
                                for (int row = 0; row < count; row++)
                                {
                                    instanceEditor.EditInstance.SelectedIndex = row; // set the cursor

                                    instance = instanceEditor.ConvertToViewModel(false); // returned instances will be displayed in grid, therefore, we want to get displayed values instead of internal value

                                    if (instance != null)
                                    {
                                        instances.Add(instance);
                                    }
                                }
                            }
                        }
                    }
                });

                return Ok(instances);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Search the index of Elasticsearch created for the given class and given schema and return count of hits
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className" > A data class name such as Issues</param>
        /// <param name="searchtext">The search text user entered</param>
        /// <returns>A integer representing count of the hits</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/search/{schemaName}/{className}/count")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "An integer representing count of the hits")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> getSearchResultCount(string schemaName, string className, string filter = null)
        {
            try
            {
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                        string filters = GetParamValue(parameters, FILTER, "");

                        string searchText = queryHelper.GetSearchText(filters);

                        JObject queryBody = FullTextSearchHelper.BuildQueryBody(searchText, -1, -1);

                        count = ElasticSearchWrapper.GetSearchCount(schemaName, className, queryBody);
                    }
                });

                return Ok(count.ToString());
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        private string GetParamValue(NameValueCollection parameters, string key, object defaultValue)
        {
            string val = null;

            if (defaultValue != null)
            {
                val = defaultValue.ToString();
            }

            if (parameters != null && parameters[key] != null)
            {
                val = parameters[key];
            }

            return val;
        }
    }
}

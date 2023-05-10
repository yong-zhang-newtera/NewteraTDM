using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Net;
using System.Text;
using System.Data;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Threading.Tasks;

using Swashbuckle.Swagger.Annotations;

using Ebaas.WebApi.Infrastructure;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Ebaas.WebApi.Models;
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
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";
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
        /// Search the indexes of Elasticsearch created for the classes in a given schema and return a collection of
        /// json objects with hit count information.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
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

                SchemaModelElementCollection classElements = FullTextSearchHelper.GetAccessibleClasses(schemaName);

                foreach (SchemaModelElement classElement in classElements)
                {
                    long count = await ElasticSearchWrapper.GetSearchCount(schemaName, classElement.Name, searchText);

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
        /// <returns>A json object representing the search result</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/search/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A collection of data instances in json format")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> getSearchResult(string schemaName, string className)
        {
            try
            {
                List<JObject> instances = new List<JObject>();
                int count = 0;
                QueryHelper queryHelper = new QueryHelper();

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
                        string searchText = GetParamValue(parameters, SEARCH_TEXT, "");
                        string sortField = GetParamValue(parameters, SORT_FIELD, "");
                        bool sortReverse = bool.Parse(GetParamValue(parameters, SORT_REVERSE, "false"));

                        StringCollection instanceIds = new StringCollection();
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            IReadOnlyCollection<JObject> result = await ElasticSearchWrapper.GetSearchResult(schemaName, className, searchText, startRow, pageSize);
                            instanceIds = FullTextSearchHelper.GetInstanceIdsFromQueryResult(result);
                        }

                        if (instanceIds.Count == 0)
                        {
                            return Ok(instances);
                        }

                        if (!string.IsNullOrEmpty(sortField))
                        {
                            queryHelper.SetSort(dataView, sortField, sortReverse);
                        }

                        string query = dataView.GetInstancesQuery(instanceIds);

                        CMCommand cmd = con.CreateCommand();
                        cmd.CommandText = query;

                        XmlReader reader = cmd.ExecuteXMLReader();
                        DataSet ds = new DataSet();
                        ds.ReadXml(reader);

                        if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                        {
                            InstanceView instanceView = new InstanceView(dataView, ds);

                            InstanceEditor instanceEditor = new InstanceEditor();
                            instanceEditor.EditInstance = instanceView;

                            count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);

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
                long count = 0;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                    string searchText = GetParamValue(parameters, SEARCH_TEXT, "");

                    count = await ElasticSearchWrapper.GetSearchCount(schemaName, className, searchText);

                }

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

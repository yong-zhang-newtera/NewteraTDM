using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Xml;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Description;
using System.Threading.Tasks;

using Newtera.Data;
using Newtera.Server.DB;
using Newtera.Server.Engine.Cache;
using Newtera.Server.Logging;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Logging;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Server.FullText;
using Newtera.Common.MetaData.Principal;
using Ebaas.WebApi.Infrastructure;
using Newtera.ElasticSearchIndexer;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// Represents a service that perform application data related tasks for admin tools
    /// </summary>
    /// <version>  	1.0.0 01 April 2016 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/appDataService")]
    public class AppDataServiceController : ApiController
    {
        /// <summary>
        /// Execute a query and return the result in XmlDocument
        /// </summary>
        /// <param name="query">An XQuery to be executed.</param>
        /// <returns>The query result in XmlDocument</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteQuery")]
        public HttpResponseMessage ExecuteQuery()
        {
            try {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string query = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    // make sure the meta-data model has not been modified
                    if (con.IsMetaDataModelModified)
                    {
                        throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                    }

                    CMCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    XmlDocument doc = cmd.ExecuteXMLDoc();

                    string xml = null;

                    if (doc != null)
                    {
                        StringWriter sw;
                        XmlTextWriter tx;
                        // convert xml doc into string
                        sw = new StringWriter();
                        tx = new XmlTextWriter(sw);
                        doc.WriteTo(tx);
                        xml = sw.ToString();
                    }

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                    }
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);
                
                return resp;
            }
        }

        /// <summary>
        /// Prepare to execute a lenghty query. 
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="query">An XQuery to be executed.</param>
        /// <param name="pageSize">The size of each return page of result.</param>
        /// <returns>An unique query id to identify the CMDataReder.</returns>
        /// <remarks>Begin a query by creating a CMDataReader, generating an unique query id,
        /// save the CMDataReader in the cache along with the id, return the id to
        /// the client.</remarks>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("BeginQuery/{pageSize}")]
        public HttpResponseMessage BeginQuery(int pageSize)
        {
            CMDataReader reader = null;

            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string query = Request.Content.ReadAsStringAsync().Result;

                // the connection will be closed at EndQuery call, therefore,
                // do not use using statement here
                CMConnection con = new CMConnection(connectionStr);
                con.Open();

                // make sure the meta-data model has not been modified
                if (con.IsMetaDataModelModified)
                {
                    throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                }

                // Create a CMDataReder object for the query
                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;
                cmd.PageSize = pageSize;
                // close the reader will close the connection
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                // NOTE: THIS APPROACH MAY NOT WORK PROPERLY IN A WEB-FARM OR WEB-GARDEN
                // ENVIRONMENT, SINCE NEXT WEB SERVICE REQUEST MAY BE DIRECTED TO AN
                // SERVER THAT DOESN'T HAVE THE CMDATAREADER CACHED.

                // create an unique id
                string id = "qid_" + Guid.NewGuid().ToString();

                // save the reader in the cache identified by the id
                QueryDataCache.Instance.AddObject(id, reader);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(id, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Get a page of result in XmlDocument. 
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="queryId">The id to identify the CMDataReader in the cache.</param>
        /// <returns>The query result in XmlDocument</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetNextResult/{queryId}")]
        public HttpResponseMessage GetNextResult(string queryId)
        {
            try
            {
                XmlDocument doc = null;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open(); // just for attaching a CustomPrinciple to the Thread

                    // Get the reader in the cache identified by the id
                    CMDataReader reader = (CMDataReader)QueryDataCache.Instance.GetObject(queryId);
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            doc = reader.GetXmlDocument();
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Unable to get the reader for query " + queryId);
                    }
                }

                string xml = null;

                if (doc != null)
                {
                    StringWriter sw;
                    XmlTextWriter tx;

                    // convert xml doc into string
                    sw = new StringWriter();
                    tx = new XmlTextWriter(sw);
                    doc.WriteTo(tx);
                    xml = sw.ToString();
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(xml))
                {
                    resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                }
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Finish a lengthy query by closing the reader.
        /// </summary>
        /// <param name="queryId">The id to identify the CMDataReader in the cache.</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("EndQuery/{queryId}")]
        public HttpResponseMessage EndQuery(string queryId)
        {
            try {
                // Remove the reader in the cache identified by the id
                CMDataReader reader = (CMDataReader)QueryDataCache.Instance.RemoveObject(queryId);
                if (reader != null)
                {
                    reader.Close(); // this will close the CMConnection associated with the reader.
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                resp.Content = new StringContent("");

                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Execute an update statement and return obj_id(s) of the instance(s) that have
        /// been involved. This method can be used to execute insert, update, and
        /// delete queries.
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="query">An XQuery to be executed.</param>
        /// <param name="needToRaiseEvents">indicates whether to raise events for the update</param>
        /// <returns>The comma separated obj_id(s)</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteUpdateQuery/{needToRaiseEvents}")]
        public  HttpResponseMessage ExecuteUpdateQuery(bool needToRaiseEvents)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string query = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    // make sure the meta-data model has not been modified
                    if (con.IsMetaDataModelModified)
                    {
                        throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                    }

                    CMCommand cmd = con.CreateCommand();
                    cmd.NeedToRaiseEvents = needToRaiseEvents;
                    cmd.CommandText = query;

                    XmlDocument doc = cmd.ExecuteXMLDoc();

                    string result;
                    if (doc.DocumentElement.InnerText != null)
                    {
                        result = doc.DocumentElement.InnerText;
                    }
                    else
                    {
                        result = "";
                    }

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(result))
                    {
                        resp.Content = new StringContent(result);
                    }
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Execute a validating query and return the validate result in xml string.
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="query">An XQuery to be executed.</param>
        /// <returns>The validating result in xml string.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteValidatingQuery")]
        public HttpResponseMessage ExecuteValidatingQuery()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string query = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    // make sure the meta-data model has not been modified
                    if (con.IsMetaDataModelModified)
                    {
                        throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                    }

                    CMCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    XmlDocument doc = cmd.ExecuteXMLDoc();

                    string result;
                    if (doc.DocumentElement.InnerText != null)
                    {
                        result = doc.DocumentElement.InnerText;
                    }
                    else
                    {
                        result = "";
                    }

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(result))
                    {
                        resp.Content = new StringContent(result);
                    }
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Delete all instances of a given class.
        /// </summary>
        /// <param name="className">The given class name.</param>
        /// <returns>The number of instances deleted.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("DeleteAllInstances/{className}")]
        public HttpResponseMessage DeleteAllInstances(string className)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    CMCommand cmd = con.CreateCommand();

                    int count = cmd.DeleteAllInstances(className);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(count.ToString());
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Get a xml document generated based on a xml schema for the instance(s) of a base class retrived by a query
        /// </summary>
        /// <param name="xmlSchemaName">The name of  xml schema in the meta data.</param>
        /// <param name="baseClassName">The base class name of the instance(s)</param>
        /// <returns>The generated xml in XmlDocument</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("GenerateXmlDoc/{xmlSchemaName}/{baseClassName}")]
        public HttpResponseMessage GenerateXmlDoc(string xmlSchemaName, string baseClassName)
        {
            try
            {
                XmlDocument doc = null;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string query = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open(); // just for attaching a CustomPrinciple to the Thread

                    XMLSchemaModel xmlSchemaModel = (XMLSchemaModel)con.MetaDataModel.XMLSchemaViews[xmlSchemaName];
                    DataViewModel instanceDataView = con.MetaDataModel.GetDetailedDataView(baseClassName);
                    if (xmlSchemaModel != null)
                    {
                        if (instanceDataView != null)
                        {
                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            doc = cmd.GenerateXmlDoc(xmlSchemaModel, instanceDataView);
                        }
                        else
                        {
                            throw new Exception("Unable to craete a data view model for the class " + baseClassName);
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find xml schema model with name " + xmlSchemaName);
                    }
                }

                string xml = null;

                if (doc != null)
                {
                    StringWriter sw;
                    XmlTextWriter tx;

                    // convert xml doc into string
                    sw = new StringWriter();
                    tx = new XmlTextWriter(sw);
                    doc.WriteTo(tx);
                    xml = sw.ToString();
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(xml))
                {
                    resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                }
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Build full-text index for a given class.
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("IsExternalFullTextSearch")]
        public bool IsExternalFullTextSearch()
        {
            return Newtera.Common.Config.ElasticsearchConfig.Instance.IsElasticsearchEnabled;
        }

        /// <summary>
        /// Build full-text index for a given class.
        /// </summary>
        /// <param name="className">The given class name.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("BuildFullTextIndex/{className}")]
        public async Task<HttpResponseMessage> BuildFullTextIndex(string className)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string connectionStr = parameters["connectionStr"];

                // use ElasticSearch for full-text search
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                    if (classElement != null)
                    {
                        IndexingContext context = new IndexingContext(con.MetaDataModel, classElement, null, Newtera.Common.MetaData.Events.OperationType.Insert);
                        BatchDocumentsRunner runner = new BatchDocumentsRunner();

                        // create document indexes for the instances in the class
                        await runner.Execute(context);
                    }
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Execute multiple update statements and return obj_id(s) of the instance(s) that have
        /// been involved. This method can be used to execute insert, update, and
        /// delete queries.
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="queries">An array of update quries to be executed.</param>
        /// <returns>The comma separated obj_id(s)</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteUpdateQueries")]
        public HttpResponseMessage ExecuteUpdateQueries()
        {
            try
            {
                string objIds = "";
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string content = Request.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(content))
                {
                    string[] queries = JsonConvert.DeserializeObject<string[]>(content);

                    using (CMConnection con = new CMConnection(connectionStr))
                    {
                        con.Open();

                        // make sure the meta-data model has not been modified
                        if (con.IsMetaDataModelModified)
                        {
                            throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                        }

                        CMCommand cmd;

                        foreach (string query in queries)
                        {
                            cmd = con.CreateCommand(); // reuse command for more one query has a bug now

                            cmd.CommandText = query;

                            XmlDocument doc = cmd.ExecuteXMLDoc();

                            if (doc.DocumentElement.InnerText != null)
                            {
                                if (objIds.Length > 0)
                                {
                                    objIds += ",";
                                }

                                objIds += doc.DocumentElement.InnerText;
                            }
                        }
                    }
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(objIds))
                {
                    resp.Content = new StringContent(objIds);
                }
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Execute a query and return the count of resulting instances.
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="query">An XQuery to be executed.</param>
        /// <returns>The count of resulting instances</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteCount")]
        public HttpResponseMessage ExecuteCount()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string query = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    // make sure the meta-data model has not been modified
                    if (con.IsMetaDataModelModified)
                    {
                        throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                    }

                    CMCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    int count = cmd.ExecuteCount();

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
 
                    resp.Content = new StringContent(count.ToString());

                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Get the number of instances in a class.
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="className">The class name.</param>
        /// <returns>The count of instances in a class</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetInstanceCount/{className}")]
        public HttpResponseMessage GetInstanceCount(string className)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    CMCommand cmd = con.CreateCommand();

                    int count = cmd.GetInstanceCount(className);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);

                    resp.Content = new StringContent(count.ToString());

                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Reset data cache
        /// </summary>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ResetDataCache")]
        public HttpResponseMessage ResetDataCache()
        {
            try
            {
                QueryDataCache.Instance.ClearExpiringObjects(); // clear the key-objId cached previously

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Execute import scripts
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="xmlString">A XML string containing import scripts</param>
        /// <param name="chunkIndex">The index of the chunked data to be imported</param>
        /// <returns>A XML string containing script execution status</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteImportScripts/{chunkIndex}")]
        public HttpResponseMessage ExecuteImportScripts(int chunkIndex)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    // make sure the meta-data model has not been modified
                    if (con.IsMetaDataModelModified)
                    {
                        throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                    }

                    CMCommand cmd = con.CreateCommand();

                    string result = cmd.ExecuteScripts(xmlString, chunkIndex);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(result))
                    {
                        resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
                    }
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Begine to import by generating sql actions from a xquery, save the actions in the cache, and return an id
        /// to identify the actions. 
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="xquery">An xquery that is used to generate sql actions.</param>
        /// <returns>An unique id used when calling ImportNext method to identify the SQL actions.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("BeginImport")]
        public HttpResponseMessage BeginImport()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xquery = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    // make sure the meta-data model has not been modified
                    if (con.IsMetaDataModelModified)
                    {
                        throw new Exception("MetaData has been modified, please restart the server to reload meta data");
                    }

                    CMCommand cmd = con.CreateCommand();

                    cmd.CommandText = xquery;

                    object sqlActions = cmd.GetSQLActions();

                    // NOTE: THIS APPROACH MAY NOT WORK PROPERLY IN A WEB-FARM OR WEB-GARDEN
                    // ENVIRONMENT, SINCE NEXT WEB SERVICE REQUEST MAY BE DIRECTED TO AN
                    // SERVER THAT DOESN'T HAVE THE CMDATAREADER CACHED.

                    // create an unique id
                    string id = "aid_" + Guid.NewGuid().ToString();

                    // save the reader in the cache identified by the id
                    QueryDataCache.Instance.AddObject(id, sqlActions);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(id, System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Import a block of data instances to the specified class. 
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the schema to query against</param>
        /// <param name="importId">The id to identify the sql actions in the cache.</param>
        /// <param name="className">Name of the class to which to import data instances.</param>
        /// <param name="xmlString">The xml string generated from a DataSet.</param>
        /// <param name="chunkIndex">The index of the chunked data to be imported</param>
        /// <returns>A XML string containing import status</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ImportNext/{importId}/{className}/{chunkIndex}")]
        public HttpResponseMessage ImportNext(string importId, string className, int chunkIndex)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string xmlString = Request.Content.ReadAsStringAsync().Result;

                string results = "";
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open(); // just for attaching a CustomPrinciple to the Thread

                    // Get the sql actions in the cache identified by the id
                    object sqlActions = QueryDataCache.Instance.GetObject(importId);
                    if (sqlActions != null)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xmlString);
                        XmlNodeReader reader = new XmlNodeReader(xmlDoc);
                        DataSet ds = new DataSet();
                        ds.ReadXml(reader);

                        CMCommand cmd = con.CreateCommand();

                        // import the first block of data instances in the dataset
                        results = cmd.ImportDataSet(className, ds, sqlActions, chunkIndex);

                        if (LoggingChecker.Instance.IsLoggingOn(con.MetaDataModel, className, LoggingActionType.Import))
                        {
                            ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                            string classCaption = (classElement != null ? classElement.Caption : null);
                            LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Import, con.MetaDataModel.SchemaInfo.NameAndVersion,
                                className, classCaption, "Import");

                            LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Unable to get the sql actions with id " + importId);
                    }
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(results))
                {
                    resp.Content = new StringContent(results, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Finish an import task.
        /// </summary>
        /// <param name="importId">The id to identify the import template in the cache.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("EndImport/{importId}")]
        public HttpResponseMessage EndImport(string importId)
        {
            try
            {
                // Remove the sql template in the cache identified by the id
                QueryDataCache.Instance.RemoveObject(importId);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }
    }
}
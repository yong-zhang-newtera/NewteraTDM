using System;
using System.Collections.Generic;
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
using System.Collections.Specialized;
using System.Web.Http.Description;

using Newtera.Data;
using Newtera.Server.DB;
using Newtera.Server.Engine.Cache;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Server.UsrMgr;
using Newtera.Common.MetaData.Principal;
using Ebaas.WebApi.Infrastructure;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// Represents a service that perform meta-data related tasks for admin tools
    /// </summary>
    /// <version>  	1.0.0 01 April 2016 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/metaDataService")]
    public class MetaDataServiceController : ApiController
    {

        /// <summary>
        /// Gets schema infos.
        /// </summary>
        /// <returns>An array of SchemaInfo instances</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSchemaInfos")]
        public SchemaInfo[] GetSchemaInfos()
        {
            using (CMConnection con = new CMConnection())
            {
                return con.AllSchemas;
            }
        }

        /// <summary>
        /// Gets meta data in xml string array
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetMetaData")]
        public  HttpResponseMessage GetMetaData()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string[] xmlStrings = new string[12];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);

                    con.MetaDataModel.SchemaModel.Write(writer);

                    // the first xml string represents schema info
                    xmlStrings[0] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.DataViews.Write(writer);

                    // the second xml string represents data views
                    xmlStrings[1] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.XaclPolicy.Write(writer);

                    // the third xml string represents xacl policy
                    xmlStrings[2] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.Taxonomies.Write(writer);

                    // the forth xml string represents taxonomies
                    xmlStrings[3] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.RuleManager.Write(writer);

                    // the fifth xml string represents rules
                    xmlStrings[4] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.MappingManager.Write(writer);

                    // the sixth xml string represents mappings
                    xmlStrings[5] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.SelectorManager.Write(writer);

                    // the seventh xml string represents selectors
                    xmlStrings[6] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.EventManager.Write(writer);

                    // the eighth xml string represents events
                    xmlStrings[7] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.LoggingPolicy.Write(writer);

                    // the ninth xml string represents logging policy
                    xmlStrings[8] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.SubscriberManager.Write(writer);

                    // the eighth xml string represents subscribers
                    xmlStrings[9] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.XMLSchemaViews.Write(writer);

                    // the eleventh xml string represents xml schema views
                    xmlStrings[10] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.ApiManager.Write(writer);

                    // the twelveth xml string represents apis
                    xmlStrings[11] = builder.ToString();

                    return Request.CreateResponse(HttpStatusCode.OK, xmlStrings);

                    //return xmlStrings;
                }
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
        /// Gets schema model in xml schema string
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSchemaModel")]
        public HttpResponseMessage GetSchemaModel()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);

                    con.MetaDataModel.SchemaModel.Write(writer);

                    string xml = builder.ToString();

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
                resp.Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }

        /// <summary>
        /// Gets a class tree as JObject
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetClassTree")]
        public HttpResponseMessage GetClassTree()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    JArray classTreeRoots = TreeNodeBuilder.GetClassTree(con.MetaDataModel);

                    if (classTreeRoots != null &&
                        classTreeRoots.Count > 0)
                    {

                        var resp = new HttpResponseMessage(HttpStatusCode.OK);
                        string str = JsonConvert.SerializeObject(classTreeRoots[0]);
                        resp.Content = new StringContent(str, System.Text.Encoding.UTF8, "text/plain");
                        return resp;
                    }
                    else
                    {
                        ErrorLog.Instance.WriteLine("class tree is empty");

                        var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        resp.Content = new StringContent("class tree is empty", System.Text.Encoding.UTF8, "text/plain");
                        return resp;
                    }
                }
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
        /// Write schema model in xml schema string to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="xmlStrings">Xml strings representing meta data.</param>
        /// <returns>The time when the meta-data model is modified.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetMetaData")]
        public HttpResponseMessage SetMetaData()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                DateTime modifiedTime = DateTime.Now;

                string content = Request.Content.ReadAsStringAsync().Result;

                string[] xmlStrings = JsonConvert.DeserializeObject<string[]>(content);

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    if (xmlStrings[0] != "")
                    {
                        // The first xml string represents a schema model
                        modifiedTime = con.UpdateMetaData(MetaDataType.Schema, xmlStrings[0]);
                    }

                    if (xmlStrings[1] != "")
                    {
                        // The second xml string represents data views of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.DataViews, xmlStrings[1]);
                    }

                    if (xmlStrings[2] != "")
                    {
                        // The third xml string represents a xacl policy of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.XaclPolicy, xmlStrings[2]);
                    }

                    if (xmlStrings[3] != "")
                    {
                        // The forth xml string represents taxonomies of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.Taxonomies, xmlStrings[3]);
                    }

                    if (xmlStrings[4] != "")
                    {
                        // The fifth xml string represents rules of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.Rules, xmlStrings[4]);
                    }

                    if (xmlStrings[5] != "")
                    {
                        // The sixth xml string represents mappings of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.Mappings, xmlStrings[5]);
                    }

                    if (xmlStrings[6] != "")
                    {
                        // The seventh xml string represents selectors of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.Selectors, xmlStrings[6]);
                    }

                    if (xmlStrings[7] != "")
                    {
                        // The eighth xml string represents events of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.Events, xmlStrings[7]);
                    }

                    if (xmlStrings[8] != "")
                    {
                        // The ninth xml string represents logging policy of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.LoggingPolicy, xmlStrings[8]);
                    }

                    if (xmlStrings[9] != "")
                    {
                        // The tenth xml string represents subscribers of a schema
                        modifiedTime = con.UpdateMetaData(MetaDataType.Subscribers, xmlStrings[9]);
                    }

                    if (xmlStrings[10] != "")
                    {
                        // The eleventh xml string represents xml schema views
                        modifiedTime = con.UpdateMetaData(MetaDataType.XMLSchemaViews, xmlStrings[10]);
                    }

                    if (xmlStrings[11] != "")
                    {
                        // The twelveth xml string represents apis
                        modifiedTime = con.UpdateMetaData(MetaDataType.Apis, xmlStrings[11]);
                    }
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
                return resp;
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
        /// Write schema model in xml schema string to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <returns>The time when the meta-data model is modified.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetSchemaModel")]
        public HttpResponseMessage SetSchemaModel()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlSchema = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    DateTime modifiedTime = con.UpdateMetaData(MetaDataType.Schema, xmlSchema);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
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
        /// Fix an schema model by detecting the discrepancies between the schema
        /// model definitions and its corresponding database definitions, and then
        /// try to fix the discrepancies on the database side.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("FixSchemaModel")]
        public HttpResponseMessage FixSchemaModel()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    con.FixSchemaModel();
                }

                return Request.CreateResponse(HttpStatusCode.OK); ;
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
        /// Gets data views in a xml string
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetDataViews")]
        public HttpResponseMessage GetDataViews()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);

                    con.MetaDataModel.DataViews.Write(writer);

                    string xml = builder.ToString();

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
                resp.Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }

        /// <summary>
        /// Write data views in xml string to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="xmlString">An xml string representing data views of a schema</param>
        /// <returns>The time when the meta-data model is modified.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetDataViews")]
        public HttpResponseMessage SetDataViews()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    DateTime modifiedTime = con.UpdateMetaData(MetaDataType.DataViews, xmlString);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
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
        /// Write xacl policy in xml string to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="xmlString">An xml string representing xacl policy of a schema</param>
        /// <returns>The time when the meta-data model is modified.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetXaclPolicy")]
        public HttpResponseMessage SetXaclPolicy()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    DateTime modifiedTime = con.UpdateMetaData(MetaDataType.XaclPolicy, xmlString);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
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
        /// Write taxonomies in xml string to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="xmlString">An xml string representing taxonomies of a schema</param>
        /// <returns>The time when the meta-data model is modified.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetTaxonomies")]
        public HttpResponseMessage SetTaxonomies()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    DateTime modifiedTime = con.UpdateMetaData(MetaDataType.Taxonomies, xmlString);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
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
        /// Write xml schema views in xml string to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="xmlString">An xml string representing xml schema views</param>
        /// <returns>The time when the meta-data model is modified.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetXMLSchemaViews")]
        public HttpResponseMessage SetXMLSchemaViews()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    DateTime modifiedTime = con.UpdateMetaData(MetaDataType.XMLSchemaViews, xmlString);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
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
        /// Write rules in xml string to database
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="xmlString">An xml string representing rules of a schema</param>
        /// <returns>The time when the meta-data model is modified.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetRules")]
        public HttpResponseMessage SetRules()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string xmlString = Request.Content.ReadAsStringAsync().Result;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    DateTime modifiedTime = con.UpdateMetaData(MetaDataType.Rules, xmlString);

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(modifiedTime.ToString("s"), System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
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
        /// Delete a meta data in database
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the meta data</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("DeleteMetaData")]
        public HttpResponseMessage DeleteMetaData()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    string schemaName = con.SchemaInfo.Name;
                    string schemaVersion = con.SchemaInfo.Version;

                    con.DeleteMetaData();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Get the latest log of updating a meta data in database
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the meta data</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetMetaDataUpdateLog")]
        public HttpResponseMessage GetMetaDataUpdateLog()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    string log = con.MetaDataUpdateLog;

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(log))
                    {
                        resp.Content = new StringContent(log, System.Text.Encoding.UTF8, "text/plain");
                    }
                    return resp;
                }
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
        /// Get an unique id for transformer
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the meta data</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetTransformerId")]
        public HttpResponseMessage GetTransformerId()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    KeyGenerator generator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.TransformerId, con.MetaDataModel.SchemaInfo);
                    string transformerId = generator.NextKey().ToString();

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    if (!string.IsNullOrEmpty(transformerId))
                    {
                        resp.Content = new StringContent(transformerId, System.Text.Encoding.UTF8, "text/plain");
                    }
                    return resp;
                }
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
        /// Get information indicating whether a given value generator exists on server side or not
        /// </summary>
        /// <param name="valueGeneratorDef">The vaue generator definition string in form of "class,lib"</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("IsValueGeneratorExist")]
        public HttpResponseMessage IsValueGeneratorExist()
        {
            try
            {
                bool status = false;

                string valueGeneratorDef = Request.Content.ReadAsStringAsync().Result;

                IAttributeValueGenerator generator = SimpleAttributeElement.CreateGenerator(valueGeneratorDef);
                if (generator != null)
                {
                    status = true;
                }

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
        /// Gets the information indicating whether a xquery condition is valid or not .
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="className">The class name that the condition is defined</param>
        /// <param name="condition">A xquery condition string</param>
        /// <returns>error mesage if it is invalid.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ValidateXQueryCondition/{className}")]
        public HttpResponseMessage ValidateXQueryCondition(string className)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string condition = Request.Content.ReadAsStringAsync().Result;

                string msg = null;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    msg = con.ValidateXQueryCondition(condition, className);
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(msg))
                {
                    resp.Content = new StringContent(msg, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
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
        /// Gets the information indicating whether the format of a custom function definition is correct.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="functionDefinition">A custom function definition</param>
        /// <returns>true if it is valid, false otherwise.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("IsValidCustomFunctionDefinition")]
        public HttpResponseMessage IsValidCustomFunctionDefinition()
        {
            try
            {
                bool status = true;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string functionDefinition = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    status = con.IsValidCustomFunctionDefinition(functionDefinition);
                }

                return Request.CreateResponse(HttpStatusCode.OK, status);
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
        /// Get a result in form of XMLDocument by executing a XQuery on server side XMLDataSourceService
        /// </summary>
        /// <param name="xquery">The xquery</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SearchXmlDataSource")]
        public HttpResponseMessage SearchXmlDataSource()
        {
            try
            {
                string xquery = Request.Content.ReadAsStringAsync().Result;

                Newtera.Server.Util.XmlDataSourceService service = new Newtera.Server.Util.XmlDataSourceService();

                XmlDocument doc = service.Execute(xquery);

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
                resp.Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }

        /// <summary>
        /// Validate a class method code.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="code">The method code</param>
        /// <param name="schemaId">The schema id indicates the schema where the instance class resides</param>
        /// <param name="instanceClassName">The class name of the instance to which the code is run against</param>
        /// <returns>Error message if the method code is invalid, null if the action code is valid.</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ValidateMethodCode/{schemaId}/{instanceClassName}")]
        public HttpResponseMessage ValidateMethodCode(string schemaId, string instanceClassName)
        {
            try
            {
                string msg = null;

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string code = Request.Content.ReadAsStringAsync().Result;

                using (WFConnection con = new WFConnection(connectionStr))
                {
                    con.Open();

                    msg = con.ValidateActionCode(code, schemaId, instanceClassName);
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(msg))
                {
                    resp.Content = new StringContent(msg, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
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
        /// Gets name of the role that has permission to modify the meta data
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the meta data</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetDBARole")]
        public HttpResponseMessage GetDBARole()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];

                string role = null;

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    role = con.GetDBARole();
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(role))
                {
                    resp.Content = new StringContent(role, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
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
        /// Sets name of the role that has permission to modify the meta data
        /// </summary>
        /// <param name="connectionStr">The connection string indicating the meta data</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("SetDBARole/{role}")]
        public HttpResponseMessage SetDBARole(string role)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    con.SetDBARole(role);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Lock the meta data specified in the connection string for update.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <exception cref="LockMetaDataException">Thrown if the meta data has been locked by another user</exception>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("LockMetaData")]
        public HttpResponseMessage LockMetaData()
        {
            try {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    con.LockMetaData();


                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    return resp;
                }
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
        /// Unlock the meta data specified in the connection string.
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        /// <exception cref="LockMetaDataException">Thrown if the user does have the permission to unlock the meta-data.</exception>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("UnlockMetaData/{forceUnlock}")]
        public HttpResponseMessage UnlockMetaData(bool forceUnlock)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    con.UnlockMetaData(forceUnlock);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Gets all sitemap models in a xml string
        /// </summary>
        /// <returns>All sitemap model specs in xml string</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSiteMapModels")]
        public HttpResponseMessage GetSiteMapModels()
        {
            try
            {
                string xml = SiteMapManager.Instance.GetSiteMapModels();

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Write specs of sitemap models in xml string.
        /// </summary>
        /// <param name="data">An xml string representing specs of site map models.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetSiteMapModels")]
        public HttpResponseMessage SetSiteMapModels()
        {
            try
            {
                var xmlString = Request.Content.ReadAsStringAsync().Result;

                SiteMapManager.Instance.SetSiteMapModels(xmlString);

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Gets the sitemap in a xml string
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSiteMap/{modelName}")]
        public HttpResponseMessage GetSiteMap(string modelName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetSiteMap(modelName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Gets the sitemap in a xml string from a file, return null if the file doesn't exist
        /// </summary>
        /// <param name="fileName">The name of a sitemap file</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSiteMapFromFile/{fileName}")]
        public HttpResponseMessage GetSiteMapFromFile(string fileName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetSiteMapFromFile(fileName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Write sitemap in xml string.
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        /// <param name="xmlString">An xml string representing site map definition.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetSiteMap/{modelName}")]
        public HttpResponseMessage SetSiteMap(string modelName)
        {
            try
            {
                var xmlString = Request.Content.ReadAsStringAsync().Result;

                SiteMapManager.Instance.SetSiteMap(modelName, xmlString);

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Gets the sidemenu in a xml string
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSideMenu/{modelName}")]
        public HttpResponseMessage GetSideMenu(string modelName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetSideMenu(modelName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Gets the sidemenu in a xml string from a file, return null if the file doesn't exist
        /// </summary>
        /// <param name="fileName">The name of a side menu file</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSideMenuFromFile/{fileName}")]
        public HttpResponseMessage GetSideMenuFromFile(string fileName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetSideMenuFromFile(fileName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Write sidemenu in xml string.
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        /// <param name="xmlString">An xml string representing side menu definition.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetSideMenu/{modelName}")]
        public HttpResponseMessage SetSideMenu(string modelName)
        {
            try
            {
                var xmlString = Request.Content.ReadAsStringAsync().Result;

                SiteMapManager.Instance.SetSideMenu(modelName, xmlString);

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Gets the custom command set in a xml string
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetCustomCommandSet/{modelName}")]
        public HttpResponseMessage GetCustomCommandSet(string modelName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetCustomCommandSet(modelName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Gets the custom command set in a xml string from a file, return null if the file doesn't exist
        /// </summary>
        /// <param name="fileName">The name of a side menu file</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetCustomCommandSetFromFile/{fileName}")]
        public HttpResponseMessage GetCustomCommandSetFromFile(string fileName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetCustomCommandSetFromFile(fileName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Write custom command set in xml string.
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        /// <param name="xmlString">An xml string representing custom command definitions.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetCustomCommandSet/{modelName}")]
        public HttpResponseMessage SetCustomCommandSet(string modelName)
        {
            try
            {
                var xmlString = Request.Content.ReadAsStringAsync().Result;

                SiteMapManager.Instance.SetCustomCommandSet(modelName, xmlString);

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Gets the sitemap access policy in a xml string.
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSiteMapAccessPolicy/{modelName}")]
        public HttpResponseMessage GetSiteMapAccessPolicy(string modelName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetSiteMapPolicy(modelName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Gets the sitemap access policy in a xml string from a file, return null if the file doesn't exist
        /// </summary>
        /// <param name="fileName">The name of a side menu file</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSiteMapAccessPolicyFromFile/{fileName}")]
        public HttpResponseMessage GetSiteMapAccessPolicyFromFile(string fileName)
        {
            try
            {
                string xml = SiteMapManager.Instance.GetSiteMapPolicyFromFile(fileName);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                return resp;
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
        /// Write sitemap access policy in xml string.
        /// </summary>
        /// <param name="modelName">The name of a sitemap model, null indicate the default sitemap</param>
        /// <param name="xmlString">An xml string representing side menu definition.</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetSiteMapAccessPolicy/{modelName}")]
        public HttpResponseMessage SetSiteMapAccessPolicy(string modelName)
        {
            try
            {
                var xmlString = Request.Content.ReadAsStringAsync().Result;

                SiteMapManager.Instance.SetSiteMapPolicy(modelName, xmlString);

                return Request.CreateResponse(HttpStatusCode.OK);
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
        /// Gets the names of form templates.
        /// </summary>
        /// <param name="schemaId">Indicates the schema that form templates belong to </param>
        /// <param name="className">Indicates the class that form templates belong to .</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetFormTemplatesFileNames/{schemaId}/{className}")]
        public HttpResponseMessage GetFormTemplatesFileNames(string schemaId, string className)
        {
            try
            {
                ServerSideServerProxy serverProxy = new ServerSideServerProxy();

                string[] templates = serverProxy.GetFormTemplatesFileNames(schemaId, className);

                return Request.CreateResponse(HttpStatusCode.OK, templates);
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
        /// Gets the names of Word report templates.
        /// </summary>
        /// <param name="schemaId">Indicates the schema that Word templates belong to </param>
        /// <param name="className">Indicates the class that Word templates belong to .</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetReportTemplatesFileNames/{schemaId}/{className}")]
        public HttpResponseMessage GetReportTemplatesFileNames(string schemaId, string className)
        {
            try
            {
                ServerSideServerProxy serverProxy = new ServerSideServerProxy();

                string[] templates = serverProxy.GetReportTemplatesFileNames(schemaId, className);

                return Request.CreateResponse(HttpStatusCode.OK, templates);
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
        /// Gets the names of worfklows.
        /// </summary>
        /// <param name="projectName">The project that the workflows belong to</param>
        /// <param name="projectVersion">The project version that workflows belong to</param>
        /// <param name="schemaId">Indicates the schema that workflows are bound to </param>
        /// <param name="className">Indicates the class that workflows are bound to </param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetWorklowNames/{projectName}/{projectVersion}/{schemaId}/{className}")]
        public HttpResponseMessage GetWorklowNames(string projectName, string projectVersion, string schemaId, string className)
        {
            try
            {
                ServerSideServerProxy serverProxy = new ServerSideServerProxy();

                string[] names = serverProxy.GetWorkflowNames(projectName, projectVersion, schemaId, className);

                return Request.CreateResponse(HttpStatusCode.OK, names);
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
        /// Get names of images for enum values in a specific directory
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetImageNames")]
        public HttpResponseMessage GetImageNames()
        {
            try
            {

                ImageManager imageManager = new ImageManager();

                List<ImageModel> images = imageManager.GetImageModels(EbaasNameSpace.USER_ICON_DIR);

                int num = 0;
                if (images != null)
                {
                    num = images.Count;
                }

                string[] imageNames = new string[num];
                int index = 0;
                foreach (ImageModel imageModel in images)
                {
                    imageNames[index] = imageModel.Name;

                    index++;
                }

                return Request.CreateResponse(HttpStatusCode.OK, imageNames);
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
        /// Get bytes of an image
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetImageBytes/{imageName}")]
        public HttpResponseMessage GetImageBytes(string imageName)
        {
            try
            {
                string staticFilesDir = NewteraNameSpace.GetStaticFilesDir();

                string imagePath = staticFilesDir + EbaasNameSpace.USER_ICON_DIR + @"\" + imageName;

                using (FileStream fs = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    Byte[] image = new Byte[fs.Length];

                    fs.Read(image, 0, Convert.ToInt32(fs.Length));

                    return Request.CreateResponse(HttpStatusCode.OK, image);
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
        /// Gets an array of xml schema basic infos of a class
        /// </summary>
        /// <param name="connectionStr">The connection string</param>
        /// <param name="className">The class name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetXMLSchemas")]
        public HttpResponseMessage GetXMLSchemas()
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string connectionStr = parameters["connectionStr"];
                string className = parameters["className"];

                using (CMConnection con = new CMConnection(connectionStr))
                {
                    con.Open();

                    JArray xmlSchemas = new JArray();

                    foreach (XMLSchemaModel xmlSchemaModel in con.MetaDataModel.XMLSchemaViews)
                    {
                        // root element type is the base class name
                        if (xmlSchemaModel.RootElement.ElementType == className)
                        {
                            JObject schemaModelInfo = new JObject();
                            schemaModelInfo["title"] = xmlSchemaModel.Caption;
                            schemaModelInfo["name"] = xmlSchemaModel.Name;

                            xmlSchemas.Add(schemaModelInfo);
                        }
                    }

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    string str = JsonConvert.SerializeObject(xmlSchemas);
                    resp.Content = new StringContent(str, System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                resp.Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }
    }
}
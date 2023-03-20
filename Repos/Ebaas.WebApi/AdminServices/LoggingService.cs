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
using System.Security.Principal;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Description;

using Newtera.Data;
using Newtera.Server.DB;
using Newtera.Server.UsrMgr;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Ebaas.WebApi.Infrastructure;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// Represents a service that perform application data related tasks for admin tools
    /// </summary>
    /// <version>  	1.0.0 01 April 2016 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/loggingService")]
    public class LoggingServiceController : ApiController
    {
        // Temporary solution. It will require the caller to pass a key in order to invoke
        // the web service
        private const string ConnectionString = "SCHEMA_NAME=LOGGINGINFO;SCHEMA_VERSION=1.0;USER_ID=admin";

        /// <summary>
        /// Gets meta data in xml string array for the LoggingInfo schema
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetLoggingMetaData")]
        public HttpResponseMessage GetLoggingMetaData()
        {
            try
            {
                string[] xmlStrings = new string[12];

                using (CMConnection con = new CMConnection(ConnectionString))
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

                    // the eleventh xml string represents xml schema view
                    xmlStrings[10] = builder.ToString();

                    builder = new StringBuilder();
                    writer = new StringWriter(builder);

                    con.MetaDataModel.ApiManager.Write(writer);

                    // the twelvth xml string represents apis
                    xmlStrings[11] = builder.ToString();

                    return Request.CreateResponse(HttpStatusCode.OK, xmlStrings);
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
        /// Execute a query and return the logging messages in XmlDocument
        /// </summary>
        /// <param name="query">An XQuery to be executed.</param>
        /// <returns>The query result in XmlDocument</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteLoggingQuery")]
        public HttpResponseMessage ExecuteLoggingQuery()
        {
            string query = Request.Content.ReadAsStringAsync().Result;

            using (CMConnection con = new CMConnection(ConnectionString))
            {
                con.Open();

                CMUserManager userMgr = new CMUserManager();
                IPrincipal superUser = userMgr.SuperUser;

                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the query as a super user since only administrator can access LoggingInfo database
                    Thread.CurrentPrincipal = superUser;

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
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                    resp.Content = new StringContent(ex.Message);

                    return resp;
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
        }

        /// <summary>
        /// Execute a query and return the count of resulting logging messages.
        /// </summary>
        /// <param name="query">An XQuery to be executed.</param>
        /// <returns>The count of resulting instances</returns>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ExecuteLoggingCount")]
        public HttpResponseMessage ExecuteLoggingCount()
        {
            string query = Request.Content.ReadAsStringAsync().Result;

            using (CMConnection con = new CMConnection(ConnectionString))
            {
                con.Open();

                CMUserManager userMgr = new CMUserManager();
                IPrincipal superUser = userMgr.SuperUser;

                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the query as a super user since only administrator can access LoggingInfo database
                    Thread.CurrentPrincipal = superUser;

                    CMCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    int count = cmd.ExecuteCount();

                    return Request.CreateResponse(HttpStatusCode.OK, count);
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
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
        }
    }
}
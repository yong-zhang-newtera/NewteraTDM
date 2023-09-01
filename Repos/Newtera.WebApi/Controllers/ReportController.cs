using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Swashbuckle.Swagger.Annotations;

using Newtera.Common.Core;
using Newtera.WebApi.Models;
using Newtera.WebApi.Infrastructure;
using Newtera.Data;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// You can use the Reporting Service to generate a report using a template and download it in Word or Excel format.
    /// </summary>
    public class ReportController : ApiController
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private string TEMPLATE_SOURCE = "templateSource";
        private string FILE_SOURCE = "file";
        private string PROPERTY_SOURCE = "property";
        private string TEMPLATE_NAME = "template";
        private string TEMPLATE_PROPERTY = "property";
        private string XML_SCHEMA = "xmlschema";
        private string OBJ_IDS = "oids";
        private string FILTER = "filter";
        private const string DATA_VIEW = "view";
        private int PAGE_SIZE = 50;

        /// <summary>
        /// Get infos of the report templates defined for a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/report/templates/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FileViewModel>), Description = "A report template infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetReportTemplates(string schemaName, string className)
        {
            try
            {
                ReportTemplateManager templateManager = new ReportTemplateManager(); 
                var results = await templateManager.Get(schemaName, className);
                return Ok(results);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Generate and download a report representing for an instance using a specified (word or excel) template
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of a data instance</param>
        /// <param name="templateSource">Indicating the source of a report template with options of file or property. Default to null</param>
        /// <param name="template">When templateSource is file, providing the name of a report template file</param>
        /// <param name="property">When templateSource is property, providing the name of a property that stores the name of a report template file</param>
        /// <param name="xmlschema">When generating a pdf report, xmlschema is used to generate xml data</param>
        /// <remarks>A report is downloaded as a result</remarks>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/report/{schemaName}/{className}/{oid}/")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "A report is generated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetReport(string schemaName, string className, string oid,
            string templateSource=null, string template=null, string property=null, string xmlschema=null)
        {
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();

            string templateSrc = GetParamValue(parameters, TEMPLATE_SOURCE, null);
            string xmlSchemaName = GetParamValue(parameters, XML_SCHEMA, null);
            string templateName = null;
            string propertyName = null;

            if (!string.IsNullOrEmpty(templateSrc))
            {
                if (templateSrc == FILE_SOURCE)
                {
                    templateName = GetParamValue(parameters, TEMPLATE_NAME, null);
                    if (string.IsNullOrEmpty(templateName))
                    {
                        throw new Exception("Missing parameter " + TEMPLATE_NAME + " in api/report/{schemaName}/{className}/{oid} api call.");
                    }
                }
                else if (templateSrc == PROPERTY_SOURCE)
                {
                    propertyName = GetParamValue(parameters, TEMPLATE_PROPERTY, null);

                    if (string.IsNullOrEmpty(propertyName))
                    {
                        throw new Exception("Missing parameter " + TEMPLATE_PROPERTY + " in api/report/{schemaName}/{className}/{oid} api call.");
                    }
                }
                else
                {
                    throw new Exception("Unknown template source type " + templateSrc);
                }

                ReportGenerator reportrGenerator = new ReportGenerator();
                
                var result = await reportrGenerator.GenerateReport(schemaName, className, xmlSchemaName, oid, templateName, propertyName, null);

                return result;
            }
            else
            {
                throw new Exception("Missing parameter " + TEMPLATE_SOURCE + " in api/report/{schemaName}/{className}/{oid} api call.");
            }
        }

        /// <summary>
        /// Generate and download a report representing for a set of data instances using a specified (word or excel) template
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="template">When templateSource is file, providing the name of a report template file</param>
        /// <param name="oids">The obj_id of data instances such as 22999222,2882992,23492999</param>
        /// <param name="filter">The filter used to get a set of data instances to generate a report.</param>
        /// <remarks>This api is used to generate report that compares data from multiple datat instances. A report is downloaded as a result</remarks>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/report/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "A report is generated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetReportMultipleInstances(string schemaName, string className,
            string template=null, string oids=null, string filter=null)
        {
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();

            string templateName = GetParamValue(parameters, TEMPLATE_NAME, null);
            if (string.IsNullOrEmpty(templateName))
            {
                throw new Exception("Missing parameter " + TEMPLATE_NAME + " in api/report/{schemaName}/{className} api call.");
            }

            StringCollection objIdCollection = null;

            string viewName = GetParamValue(parameters, DATA_VIEW, null);

            // get obj_ids of data instances in form of 22999222,2882992,292999
            string objIds = GetParamValue(parameters, OBJ_IDS, null);

            // filter used to select instances
            string filterStr = GetParamValue(parameters, FILTER, null);
            if (!string.IsNullOrEmpty(objIds))
            {
                // conver to string collection
                string[] objIdArray = objIds.Split(',');
                objIdCollection = new StringCollection();

                for (int i = 0; i < objIdArray.Length; i++)
                {
                    objIdCollection.Add(objIdArray[i]);
                }
            }
            else
            {
                objIdCollection = GetInstanceIds(schemaName, className, viewName, filterStr);
            }

            ReportGenerator reportGenerator = new ReportGenerator();
            
            var result = await reportGenerator.GenerateReport(schemaName, className, objIdCollection, templateName, null);

            return result;
        }

        private string GetParamValue(NameValueCollection parameters, string key, object defaultValue)
        {
            string val = null;

            if (defaultValue != null)
            {
                val = defaultValue.ToString();
            }

            if (parameters[key] != null)
            {
                val = parameters[key];
            }

            return val;
        }

        private StringCollection GetInstanceIds(string schemaName, string className, string viewName, string filter)
        {
            StringCollection objIds = new StringCollection();
            QueryHelper queryHelper = new QueryHelper();

            using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView;

                if (!string.IsNullOrEmpty(viewName))
                {
                    dataView = con.MetaDataModel.DataViews[viewName] as DataViewModel;
                    if (dataView != null)
                    {
                        // get a copy of the dataview
                        dataView = dataView.Clone();
                    }
                    else
                    {
                        dataView = con.MetaDataModel.GetDefaultDataView(className);
                    }
                }
                else
                {
                    dataView = con.MetaDataModel.GetDefaultDataView(className);
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    queryHelper.SetFilters(dataView, filter, true);
                }

                CMDataReader dataReader = null;
                XmlReader xmlReader;
                XmlDocument doc;
                DataSet ds;
                int count;

                try
                {
                    string query = dataView.SearchQuery;

                    CMCommand cmd = con.CreateCommand();
                    cmd.PageSize = PAGE_SIZE;
                    cmd.CommandText = query;

                    // use Default behavior so that when closing CMDataReader, the
                    // connection won't be closed
                    dataReader = cmd.ExecuteReader();

                    // read the data in pages
                    while (dataReader.Read())
                    {
                        doc = dataReader.GetXmlDocument();

                        xmlReader = new XmlNodeReader(doc);
                        ds = new DataSet();
                        ds.ReadXml(xmlReader);

                        if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                        {
                            InstanceView instanceView = new InstanceView(dataView, ds);

                            count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                            for (int row = 0; row < count; row++)
                            {
                                instanceView.SelectedIndex = row; // set the cursor
                                objIds.Add(instanceView.InstanceData.ObjId);
                            }
                        }
                        else
                        {
                            // got an empty result
                            break;
                        }
                    }
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
                }
            }

            return objIds;
        }
    }
}

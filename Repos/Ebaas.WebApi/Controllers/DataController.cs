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
using Ebaas.WebApi.Models;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Api;
using Newtera.Common.MetaData.Rules;
using Newtera.Common.Wrapper;
using Newtera.Data;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Server.Engine.Workflow;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.WebForm;
using System.Threading;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// The Data Service is a model-driven web service. You can use the APIs of the service to perform CRUD operations
    /// on any of the classes in your application data model without coding. The service stores the data in a
    /// relational database, such as SQL Server or Oracle.
    /// </summary>
    public class DataController : ApiController
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string DATA_VIEW = "view";
        private const string FORM_TEMPLATE = "template";
        private const string FORM_ATTRIBUTE = "formAttribute";
        private const string FORM_FORMAT = "formformat";
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";
        private const string FILTER = "filter";
        private const string SORT_FIELD = "sortfield";
        private const string SORT_REVERSE = "sortreverse";
        private const string FULL_VIEW = "full";
        private const string SIMPLE_VIEW = "simple";
        private const string DEEP_CLONE = "deep";
        private const string TREE_NAME = "tree";
        private const string TREE_NODE = "node";
        private const string PARAM_PAIR = "parameters";
        private const string DEFAULT_FORM = "default.htm";
        private const string VALIDATE = "validate";

        /// <summary>
        /// Get data instances in a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="view">Indicate the data view used for getting data, such as simple, full, or a specific name. Default to simple.</param>
        /// <param name="from">Indicate the start row index such as 0. Default to 0.</param>
        /// <param name="size">Indicate a page size such as 20. Default to 20.</param>
        /// <param name="filter">Getting the data using the filter such as [["Category","contains","bug"],"and",[["Status","=","Closed"],"or",["Status","=","Fixed"]]]. Default to null.</param>
        /// <param name="sortfield">Indicate the sort field of the data such as StartTime. Default to null</param>
        /// <param name="sortreverse">Indicate the sort direction such as true or false where true means descending and false means acending. Default to false.</param>
        /// <returns>A collection of data instance in json</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(List<Object>), Description = "A collection of data instances in json format")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetMany(string schemaName, string className, string view=null, int? from=null, int? size=null, string filter=null, string sortfield= null, string sortreverse=null)
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
                        string treeName = GetParamValue(parameters, TREE_NAME, null);
                        string nodeName = GetParamValue(parameters, TREE_NODE, null);
                        string viewName = GetParamValue(parameters, DATA_VIEW, SIMPLE_VIEW);

                        if (!string.IsNullOrEmpty(treeName) &&
                            !string.IsNullOrEmpty(nodeName))
                        {
                            // get data view for a taxonomy tree node
                            TaxonomyModel taxonomy = (TaxonomyModel)con.MetaDataModel.Taxonomies[treeName];
                            if (taxonomy != null)
                            {
                                TaxonNode taxonNode = taxonomy.FindNode(nodeName);
                                if (taxonNode != null)
                                {
                                    dataView = taxonNode.GetDataView("DETAILED");
                                }
                                else if (con.MetaDataModel.Taxonomies[nodeName] != null)
                                {
                                    // it is a root node
                                    dataView = taxonomy.GetDataView("DETAILED");
                                }
                                else
                                {
                                    throw new NotImplementedException("Unable to find a node with name " + nodeName + " in the Taxonomy " + treeName);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException("Taxonomy " + treeName + " does not exist.");
                            }
                        }
                        else if (viewName == SIMPLE_VIEW)
                        {
                            dataView = con.MetaDataModel.GetDefaultDataView(className);
                        }
                        else if (viewName == FULL_VIEW)
                        {
                            dataView = con.MetaDataModel.GetDetailedDataView(className);
                        }
                        else
                        {
                            dataView = con.MetaDataModel.DataViews[viewName] as DataViewModel;
                            if (dataView != null)
                            {
                                // get a copy of the dataview
                                dataView = dataView.Clone();
                            }
                        }

                        if (dataView != null)
                        {
                            int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 20));
                            int pageIndex = int.Parse(GetParamValue(parameters, START_ROW, 0)) / pageSize;
                            string filters = GetParamValue(parameters, FILTER, "");
                            string sortField = GetParamValue(parameters, SORT_FIELD, "");
                            bool sortReverse = bool.Parse(GetParamValue(parameters, SORT_REVERSE, "false"));

                            dataView.PageIndex = pageIndex;
                            dataView.PageSize = pageSize;
                            if (!string.IsNullOrEmpty(filters))
                            {
                                queryHelper.SetFilters(dataView, filters);
                            }

                            if (!string.IsNullOrEmpty(sortField))
                            {
                                queryHelper.SetSort(dataView, sortField, sortReverse);
                            }

                            string query = dataView.SearchQueryWithPKValues;

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

                                        //var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
                                        //ErrorLog.Instance.WriteLine(jsonString);
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
        /// Gets count of returning data using a filter
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="filter">The filter used to get the data, such as [["Category","contains","bug"],"and",[["Status","=","Closed"],"or",["Status","=","Fixed"]]]. Default to null.</param>
        /// <returns>The data count</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/count/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int), Description = "Data instance count")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetCount(string schemaName, string className, string filter = null)
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
                        string treeName = GetParamValue(parameters, TREE_NAME, null);
                        string nodeName = GetParamValue(parameters, TREE_NODE, null);
                        string viewName = GetParamValue(parameters, DATA_VIEW, SIMPLE_VIEW);

                        DataViewModel dataView = null;

                        if (!string.IsNullOrEmpty(treeName) &&
                            !string.IsNullOrEmpty(nodeName))
                        {
                            // get data view for a taxonomy tree node
                            TaxonomyModel taxonomy = (TaxonomyModel)con.MetaDataModel.Taxonomies[treeName];
                            if (taxonomy != null)
                            {
                                TaxonNode taxonNode = taxonomy.FindNode(nodeName);
                                if (taxonNode != null)
                                {
                                    dataView = taxonNode.GetDataView("DETAILED");
                                }
                                else if (con.MetaDataModel.Taxonomies[nodeName] != null)
                                {
                                    // it is a root node
                                    dataView = taxonomy.GetDataView("DETAILED");
                                }
                                else
                                {
                                    throw new NotImplementedException("Unable to find a node with name " + nodeName + " in the Taxonomy " + treeName);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException("Taxonomy " + treeName + " does not exist.");
                            }
                        }
                        else if (viewName == SIMPLE_VIEW)
                        {
                            dataView = con.MetaDataModel.GetDefaultDataView(className);
                        }
                        else if (viewName == FULL_VIEW)
                        {
                            dataView = con.MetaDataModel.GetDetailedDataView(className);
                        }
                        else
                        {
                            dataView = con.MetaDataModel.DataViews[viewName] as DataViewModel;
                            if (dataView != null)
                            {
                                // get a copy of the dataview
                                dataView = dataView.Clone();
                            }
                        }

                        if (dataView != null)
                        {
                            string filters = GetParamValue(parameters, FILTER, "");

                            if (!string.IsNullOrEmpty(filters))
                            {
                                queryHelper.SetFilters(dataView, filters);
                            }

                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            count = cmd.ExecuteCount();
                        }
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

        /// <summary>
        /// Executing a custom API on a set of instances of a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="method">A name of a cusotm api such as GetRequestCountByMonth</param>
        /// <param name="parameters">A parameter expected by the method in form of "name1:value1,name2:value2,name3:value3"</param>
        /// <returns>The JObject return by the cusotm api</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/custom/{method}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "An object in JSON as result of executing a cusotm API")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetManyCustom(string schemaName, string className, string method, string parameters = null)
        {
            try
            {
                JObject result = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameter = Request.RequestUri.ParseQueryString();

                        DataViewModel dataView = con.MetaDataModel.GetDefaultDataView(className);
                        string pair = GetParamValue(parameter, PARAM_PAIR, null);
                        NameValueCollection arguments = null;
                        if (!string.IsNullOrEmpty(pair))
                        {
                            arguments = ParseParameters(pair);
                        }

                        // run the specified api on the instance
                        Api api = con.MetaDataModel.ApiManager.GetClassApi(className, method);
                        if (api != null &&
                            !string.IsNullOrEmpty(api.ExternalHanlder) &&
                            api.MethodType == MethodType.GetMany)
                        {
                            ApiExecutionContext context = new ApiExecutionContext();
                            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                            schemaInfo.Name = schemaName;
                            schemaInfo.Version = "1.0";
                            context.SchemaId = schemaInfo.NameAndVersion; // schemaName1.0
                            context.ClassName = className;
                            context.DataView = dataView;
                            context.Parameters = arguments;
                            context.Connection = con;
                            context.AcceptType = "application/json";
                            context.ContentType = "application/json";

                            IApiFunction function = queryHelper.CreateExternalHandler(api.ExternalHanlder);
                            if (function != null)
                            {
                                result = function.Execute(context);

                                //var jsonString = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
                                //ErrorLog.Instance.WriteLine(jsonString);
                            }
                            else
                            {
                                throw new Exception("Unable to create the custom function with definition " + api.ExternalHanlder);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException("Api " + method + " does not exist or has wrong definitions.");
                        }
                    }
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Executing a custom api on a given instance of a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">the obj_id of an instance such as 2883988822</param>
        /// <param name="method">A name of a cusotm api such as GetRequestCountByMonth</param>
        /// <returns>The JObject return by the cusotm api</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}/custom/{method}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "An object in JSON as result of executing a cusotm API")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetOneCustom(string schemaName, string className, string oid, string method)
        {
            try
            {
                JObject result = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {

                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameter = Request.RequestUri.ParseQueryString();

                        DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                        // run the specified api on the instance
                        Api api = con.MetaDataModel.ApiManager.GetClassApi(className, method);
                        if (api != null &&
                            !string.IsNullOrEmpty(api.ExternalHanlder) &&
                            api.MethodType == MethodType.GetOne)
                        {
                            ApiExecutionContext context = new ApiExecutionContext();
                            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                            schemaInfo.Name = schemaName;
                            schemaInfo.Version = "1.0";
                            context.SchemaId = schemaInfo.NameAndVersion; // schemaName1.0
                            context.ClassName = className;
                            context.DataView = dataView;
                            context.ObjID = oid;
                            context.Connection = con;
                            context.Parameters = parameter;
                            context.AcceptType = "application/json";
                            context.ContentType = "application/json";

                            IApiFunction function = queryHelper.CreateExternalHandler(api.ExternalHanlder);
                            if (function != null)
                            {
                                result = function.Execute(context);

                                //var jsonString = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
                                //ErrorLog.Instance.WriteLine(jsonString);
                            }
                            else
                            {
                                throw new Exception("Unable to create the custom function with definition " + api.ExternalHanlder);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException("Api " + method + " does not exist or has wrong definitions.");
                        }
                    }
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get one instance of a class given the instance id
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">the obj_id of an instance such as 2883988822</param>
        /// <returns>The instance</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "An instance in json")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetOne(string schemaName, string className, string oid)
        {
            try
            {
                JObject instance = null;

                await Task.Factory.StartNew(() =>
                {
                    instance = GetOneInstance(schemaName, className, oid);

                });

                return Ok(instance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Clone a data instance and return the cloned instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">the obj_id of an instance to be duplicated such as 2883988822</param>
        /// <param name="deep">true to deep clone an instance by cloning its related instances following predefined paths; false to shallow clone the instance without cloning its related instances</param>
        /// <returns>The duplicated instance</returns>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}/clone")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "The cloned instance in JSON")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> CloneInstance(string schemaName, string className, string oid, bool? deep = null)
        {
            try
            {
                JObject instance = null;

                await Task.Factory.StartNew(() =>
                {
                    instance = GetClonedInstance(schemaName, className, oid);
                });

                return Ok(instance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get a new instance loaded with initial values
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="template">When getting a new hierarchical instance for a web form, provide a form template name such as RequestForm.htm. Default to null</param>
        /// <param name="formformat">Indicate the format of the returned instance. True means data in form format and false means in normal format. Default to false</param>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/new")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A new instance object in JSON")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetNew(string schemaName, string className, string template = null, string formformat = null)
        {
            try
            {
                JObject instance = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        string templateName = GetParamValue(parameters, FORM_TEMPLATE, null);
                        bool formFormat = bool.Parse(GetParamValue(parameters, FORM_FORMAT, "false"));

                        if (!string.IsNullOrEmpty(templateName))
                        {
                            // get instance using form template
                            instance = GetCustomFormDataUsingFile(schemaName, className, null, templateName);
                        }
                        else
                        {
                            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                            InstanceView instanceView = new InstanceView(dataView);
                            ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                            string initCode = GetClassCustomCode(classElement, ClassElement.CLASS_INITIALIZATION_CODE);
                            if (!string.IsNullOrEmpty(initCode))
                            {
                                // initialize the new instance with initialization code

                                // Execute the initialization code using the same connection so that changes are made within a same transaction
                                IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView);

                                // run the initialization code on the instance
                                ActionCodeRunner.Instance.ExecuteActionCode("GetNewAPI", "ClassInit" + classElement.ID, initCode, instanceWrapper);
                            }

                            InstanceEditor instanceEditor = new InstanceEditor();
                            instanceEditor.EditInstance = instanceView;
                            instance = instanceEditor.ConvertToViewModel(true); // translate the instance of InstanceView (Model) to an instance of JSOM (ViewModel)
                        }
                    }
                });

                return Ok(instance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets data instances of a class that is related to a data instance in a master class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="relatedClassName">The name of a class related to the master class such as ATestItemInstance. The class can be related to the master class through one-to-one, one-to-many, many-to-one, or many-to-many relationhsip, and can be a subclass of a class that has the relationship.</param>
        /// <param name="view">Return the data using the data view such as simple, full, or a specific name. Default to simple.</param>
        /// <param name="from">Return the data starting from the row index such as 0. Default to 0.</param>
        /// <param name="size">Return the data with a page size such as 20. Default to 20.</param>
        /// <param name="filter">Search the data using the filter such as [["Category","contains","bug"],"and",[["Status","=","Closed"],"or",["Status","=","Fixed"]]]. Default to null.</param>
        /// <param name="sortfield">Sort returning data by a sort field such as StartTime. Default to null</param>
        /// <param name="sortreverse">Sort returning datat with the direction such as true or false where true means descending and false means acending. Default to false.</param>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}/{relatedClassName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A collection of the related data instances in JSON")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetRelatedMany(string schemaName, string className, string oid, string relatedClassName,
            string view = null, int? from = null, int? size = null, string filter = null, string sortfield = null, string sortreverse = null)
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

                        InstanceView masterInstanceView = GetInstanceView(con, className, oid);

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                        DataViewModel dataView = null;
                        string viewName = GetParamValue(parameters, DATA_VIEW, SIMPLE_VIEW);

                        if (viewName == SIMPLE_VIEW)
                        {
                            dataView = con.MetaDataModel.GetRelatedDefaultDataView(masterInstanceView, relatedClassName);
                        }
                        else if (viewName == FULL_VIEW)
                        {
                            dataView = con.MetaDataModel.GetRelatedDetailedDataView(masterInstanceView, relatedClassName);
                        }
                        else
                        {
                            dataView = con.MetaDataModel.GetRelatedDataView(viewName, masterInstanceView, relatedClassName);
                        }

                        if (dataView != null)
                        {
                            int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 20));
                            int pageIndex = int.Parse(GetParamValue(parameters, START_ROW, 0)) / pageSize;
                            string filters = GetParamValue(parameters, FILTER, "");
                            string sortField = GetParamValue(parameters, SORT_FIELD, "");
                            bool sortReverse = bool.Parse(GetParamValue(parameters, SORT_REVERSE, "false"));

                            dataView.PageIndex = pageIndex;
                            dataView.PageSize = pageSize;
                            if (!string.IsNullOrEmpty(filters))
                            {
                                queryHelper.AddFilters(dataView, filters); // add search filter to the existing filter
                            }

                            if (!string.IsNullOrEmpty(sortField))
                            {
                                queryHelper.SetSort(dataView, sortField, sortReverse);
                            }

                            string query = dataView.SearchQueryWithPKValues;

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
                                instances = new List<JObject>();
                                JObject instance;
                                for (int row = 0; row < count; row++)
                                {
                                    instanceEditor.EditInstance.SelectedIndex = row; // set the cursor

                                    instance = instanceEditor.ConvertToViewModel(false); // Related instances are displayed in a grid, therefore, do not convert instance values into model values in which enum property values are indeces.

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
        /// Gets a count of data instances of a class that is related to a data instance in a master class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="relatedClassName">The name of a class related to the master class such as ATestItemInstance. The class can be related to the master class through one-to-one, one-to-many, many-to-one, or many-to-many relationhsip, and can be a subclass of a class that has the relationship.</param>
        /// <param name="filter">Search the data using the filter such as [["Category","contains","bug"],"and",[["Status","=","Closed"],"or",["Status","=","Fixed"]]]. Default to null.</param>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/count/{schemaName}/{className}/{oid:long}/{relatedClassName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int), Description = "The related instance count")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetRelatedCount(string schemaName, string className, string oid, string relatedClassName, string filter = null)
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

                        InstanceView masterInstanceView = GetInstanceView(con, className, oid);

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        string viewName = GetParamValue(parameters, DATA_VIEW, SIMPLE_VIEW);

                        DataViewModel dataView = null;
                        if (viewName == SIMPLE_VIEW)
                        {
                            dataView = con.MetaDataModel.GetRelatedDefaultDataView(masterInstanceView, relatedClassName);
                        }
                        else if (viewName == FULL_VIEW)
                        {
                            dataView = con.MetaDataModel.GetRelatedDetailedDataView(masterInstanceView, relatedClassName);
                        }
                        else
                        {
                            dataView = con.MetaDataModel.GetRelatedDataView(viewName, masterInstanceView, relatedClassName);
                        }

                        if (dataView != null)
                        {
                            string filters = GetParamValue(parameters, FILTER, "");

                            if (!string.IsNullOrEmpty(filters))
                            {
                                queryHelper.AddFilters(dataView, filters);
                            }

                            string query = dataView.SearchQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            count = cmd.ExecuteCount();
                        }
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


        /// <summary>
        /// Gets a specific data instance in a class related to a data instance in a master class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="relatedClassName">The name of a class related to the master class such as ATestItemInstance. The class can be related to the master class through one-to-one, one-to-many, many-to-one, or many-to-many relationhsip, and can be a subclass of a class that has the relationship.</param>
        /// <param name="roid">the obj_id of a data instance in the related class</param>
        /// <param name="view">Return the data using the data view such as simple, full, or a specific name. Default to simple.</param>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}/{relatedClassName}/{roid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A related data instance in JSON")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetRelatedOne(string schemaName, string className, string oid, string relatedClassName, string roid, string view=null)
        {
            try
            {
                JObject instance = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        InstanceView masterInstanceView = GetInstanceView(con, className, oid);

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        string viewName = GetParamValue(parameters, DATA_VIEW, SIMPLE_VIEW);

                        DataViewModel dataView = null;
                        if (viewName == SIMPLE_VIEW)
                        {
                            dataView = con.MetaDataModel.GetRelatedDefaultDataView(masterInstanceView, relatedClassName);
                        }
                        else if (viewName == FULL_VIEW)
                        {
                            dataView = con.MetaDataModel.GetRelatedDetailedDataView(masterInstanceView, relatedClassName);
                        }
                        else
                        {
                            dataView = con.MetaDataModel.GetRelatedDataView(viewName, masterInstanceView, relatedClassName);
                        }

                        if (dataView != null)
                        {

                            // create an instance query
                            string query = dataView.GetInstanceQuery(oid);

                            //ErrorLog.Instance.WriteLine("GetRelatedOne Search Query is " + query);

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
                                instance = instanceEditor.ConvertToViewModel(false); // translate the instance of InstanceView (Model) to an instance of JSOM (ViewModel)
                            }
                        }
                    }
                });

                return Ok(instance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Add an instance to a class. If the posted instance contains data for the related classes, it creates the related data instances.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="instance">A json object to be added as data instance</param>
        /// <param name="template">When posting from a web form, providing a name of the form template such as RequestForm.htm. Default to null</param>
        /// <param name="validate">If validate is true, it will validate the instance before adding the instance to database and return an error message if there is a validating error. Otherwise, it won't validate the instance. Default value is false.</param>
        /// <param name="formformat">Indicate if the instance posted is in a form format? The true means data in form format and false means in regular format. Default to true</param>
        [HttpPost]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object as an added instance with obj_id")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> Create(string schemaName, string className, dynamic instance, string template=null, string validate=null, string formformat=null)
        {
            try
            {
                JObject newInstance = null;
                string oid = "";
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        //var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
                        //ErrorLog.Instance.WriteLine(jsonString);

                        ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                        if (classElement == null)
                        {
                            throw new Exception("Unable to find a class with name " + className);
                        }
                        else if (!classElement.IsLeaf)
                        {
                            throw new Exception("It is not allowed to add an instance to an abstract class " + className);
                        }

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        string templateName = GetParamValue(parameters, FORM_TEMPLATE, null);

                        bool needValidate = false; // default value
                        string validateStr = GetParamValue(parameters, VALIDATE, null);
                        if (!string.IsNullOrEmpty(validateStr) && validateStr.Trim() == "true")
                        {
                            needValidate = true;
                        }

                        bool isFormFormat = true; // default value
                        string formformatStr = GetParamValue(parameters, FORM_FORMAT, null);
                        if (!string.IsNullOrEmpty(formformatStr) && formformatStr.Trim() == "false")
                        {
                            isFormFormat = false;
                        }

                        if (!string.IsNullOrEmpty(templateName))
                        {
                            // update instance using form template
                            newInstance = UpdateCustomFormDataUsingFile(schemaName, className, null, templateName, instance, needValidate);
                        }
                        else
                        {

                            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                            // Create an instance view
                            InstanceView instanceView = new InstanceView(dataView);

                            InstanceEditor instanceEditor = new InstanceEditor();
                            instanceEditor.EditInstance = instanceView;
                            instanceEditor.ConvertToModel(instance, isFormFormat); // translate the JSON instance to InstanceView instance

                            // run initialzing code
                            RunBeforeInsertCode(con, instanceView, instance);

                            if (needValidate)
                            {
                                Validate(con, instanceEditor, classElement, dataView); // validate the instance before inserting. This method throw an error on validate error
                            }

                            string query = instanceView.DataView.InsertQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            XmlDocument doc = cmd.ExecuteXMLDoc(); // execute insert query

                            oid = doc.DocumentElement.InnerText;

                            // get new instance and return back to caller
                            newInstance = GetOneInstance(schemaName, className, oid);
                        }
                    }
                });

                return Ok(newInstance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Create a relatiosnhip between a master data instance and a related data instance. The relationship is one of many-to-many or one-to-many between master and related data instances.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="rClassName">The name of a class related to the master class such as ATestItemInstance. The class can be related to the master class through one-to-one, one-to-many, many-to-one, or many-to-many relationhsip, and can be a subclass of a class that has the relationship.</param>
        /// <param name="roid">the obj_id of a data instance in the related class such as 883939992</param>
        [HttpPost]
        [AuthorizeByMetaDataAttribute]
        [Route("api/relationship/{schemaName}/{className}/{oid}/{rClassName}/{roid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "The relationship created successfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> CreateRelated(string schemaName, string className, string oid, string rClassName, string roid)
        {
            try {
                JObject instance = new JObject();

                string relatedClassName = rClassName;
                string masterInstanceId = oid;
                string masterClassName = className;
                string relatedInstanceId = roid;

                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection connection = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        connection.Open();

                        MetaDataModel metaData = connection.MetaDataModel;

                        string junctionClassName = GetManyToManyClassName(metaData, className, rClassName);

                        if (junctionClassName != null)
                        {
                            // there is a many-to-many relationship between master class and related class
                            CreateManyToManyRelationship(connection, metaData, masterClassName, masterInstanceId, relatedClassName, relatedInstanceId, junctionClassName);
                        }
                        else
                        {
                            // create a one-to-many relationhsip between master class and related class
                            CreateOneToManyRelationship(connection, metaData, masterClassName, masterInstanceId, relatedClassName, relatedInstanceId);
                        }
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Update an instance in a class, If the posted instance contains data for the related classes, it also updates the related data instances.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of a data instance to be updated</param>
        /// <param name="instance">A json object with updated values</param>
        /// <param name="template">When posting from a web form, providing a name of the form template such as RequestForm.htm. Default to null</param>
        /// <param name="formAttribute">When posting from a web form, providing a name of the attribute that stores a name of the form template such as InfoForm. Default to null</param>
        /// <param name="validate">If validate is true, it will validate the instance before updating the instance to database and return an error message if there is a validating error. Otherwise, it won't validate the instance. Default value is false.</param>
        /// <param name="formformat">Indicate if the instance posted is in a form format? The true means data in form format and false means in regular format. Default to true</param>
        /// <remarks>The "template" and "formAttribute" parameters are mutually-exclusive. They can be all null or only one should be provided.</remarks>
        [HttpPost]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object as an updated instance with obj_id")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> Update(string schemaName, string className, string oid, dynamic instance, string template = null, string formAttribute = null, string validate = null, string formformat = null)
        {
            try
            {
                JObject updatedInstance = null;

                QueryHelper queryHelper = new QueryHelper();

                //var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
                //ErrorLog.Instance.WriteLine(jsonString);

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        string templateName = GetParamValue(parameters, FORM_TEMPLATE, null);
                        string formAttributeName = GetParamValue(parameters, FORM_ATTRIBUTE, null);

                        bool needValidate = false;
                        string validateStr = GetParamValue(parameters, VALIDATE, null);
                        if (!string.IsNullOrEmpty(validateStr) && validateStr.Trim() == "true")
                        {
                            needValidate = true;
                        }

                        bool isFormFormat = true; // default value
                        string formformatStr = GetParamValue(parameters, FORM_FORMAT, null);
                        if (!string.IsNullOrEmpty(formformatStr) && formformatStr.Trim() == "false")
                        {
                            isFormFormat = false;
                        }

                        if (!string.IsNullOrEmpty(templateName))
                        {
                            // update instance using form template
                            updatedInstance = UpdateCustomFormDataUsingFile(schemaName, className, oid, templateName, instance, needValidate);
                        }
                        else if (!string.IsNullOrEmpty(formAttributeName))
                        {
                            // update instance using form template specified by the value of an attribute
                            updatedInstance = UpdateCustomFormDataUsingAttribute(schemaName, className, oid, formAttributeName, instance, needValidate);
                        }
                        else if (isFormFormat && HasDefaultFormTemplate(schemaName, className))
                        {
                            // update instance using the deafult form template
                            updatedInstance = UpdateCustomFormDataUsingFile(schemaName, className, oid, DEFAULT_FORM, instance, needValidate);
                        }
                        else
                        {
                            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                            // get formId to be used when running BeforeUpdateCode or taskId to run custom action
                            IEnumerable<string> headerValues;
                            var taskId = string.Empty;
                            if (Request.Headers.TryGetValues("taskId", out headerValues))
                            {
                                taskId = headerValues.FirstOrDefault();
                            }

                            var actionId = string.Empty;
                            if (Request.Headers.TryGetValues("actionId", out headerValues))
                            {
                                actionId = headerValues.FirstOrDefault();
                            }

                            string existingObjId = oid;

                            string query = null;
                            CMCommand cmd;
                            XmlDocument doc;
                            InstanceView instanceView = null;
                            bool isUpdate = false;

                            if (!string.IsNullOrEmpty(existingObjId))
                            {
                                // get the existing instance
                                query = dataView.GetInstanceQuery(existingObjId);

                                cmd = con.CreateCommand();
                                cmd.CommandText = query;

                                XmlReader reader = cmd.ExecuteXMLReader();
                                DataSet ds = new DataSet();
                                ds.ReadXml(reader);

                                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                                {
                                    // the instance with the obj_id exists
                                    instanceView = new InstanceView(dataView, ds);
                                    isUpdate = true;
                                }
                                else
                                {
                                    // Create an instance view
                                    instanceView = new InstanceView(dataView);
                                }
                            }
                            else
                            {
                                // Create an instance view
                                instanceView = new InstanceView(dataView);
                            }

                            InstanceEditor instanceEditor = new InstanceEditor();
                            instanceEditor.EditInstance = instanceView;
                            instanceEditor.ConvertToModel(instance, isFormFormat); // // translate the JSON instance data to InstanceView instance data

                            if (isUpdate)
                            {
                                if (!string.IsNullOrEmpty(taskId) && !string.IsNullOrEmpty(actionId))
                                {
                                    // execute the script specified in the custom action
                                    instanceEditor.RunCustomAction(con, schemaName, taskId, actionId, instanceView, instance);

                                    // Raise Close Task Event to the HanldeGroupTaskActivity
                                    instanceEditor.RaiseCloseTaskEvent(con, schemaName, taskId, actionId, instanceView);
                                }
                                else
                                {
                                    // execute the before update script
                                    RunBeforeUpdateCode(con, instanceView, instance);
                                }

                                if (needValidate)
                                {
                                    instanceEditor.Validate(); // validate the instance before inserting. This method throw an error on validate error
                                }

                                // make sure that instance's value(s) have been changed, otheriwse, no update is performed
                                if (instanceView.InstanceData.IsChanged)
                                {
                                    query = instanceView.DataView.UpdateQuery;
                                }
                                else
                                {
                                    query = null;
                                }
                            }
                            else
                            {
                                RunBeforeInsertCode(con, instanceView, instance);

                                if (needValidate)
                                {
                                    ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                                    Validate(con, instanceEditor, classElement, dataView); // validate the instance before inserting. This method throw an error on validate error
                                }

                                // generate insert query
                                query = instanceView.DataView.InsertQuery;
                            }

                            if (query != null)
                            {
                                cmd = con.CreateCommand();
                                cmd.CommandText = query;

                                doc = cmd.ExecuteXMLDoc(); // execute insert or update query

                                string id = doc.DocumentElement.InnerText;

                                updatedInstance = GetOneInstance(schemaName, className, id);
                            }
                        }
                    }
                });

                return Ok(updatedInstance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Call the callback function defined for a class on the data instance which doesn't exist in the class.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="property">The name of a field in a web form invoke this api with its value change</param>
        /// <param name="instance">The json object to be updated by the callback</param>
        /// <param name="template">When posting from a web form, providing a name of the form template such as RequestForm.htm. Default to null</param>
        /// <param name="formAttribute">When posting from a web form, providing a name of the attribute that stores a name of the form template such as InfoForm. Default to null</param>
        /// <remarks>This api is normally called when a field in a web form is changed. The "template" and "formAttribute" parameters are mutually-exclusive. They can be all null or only one should be provided.</remarks>
        [HttpPost]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{property}/refresh")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object with updated values")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> RefreshNew(string schemaName, string className, string property, dynamic instance, string template = null, string formAttribute = null)
        {
            try
            {
                JObject updatedInstance = null;

                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        string templateName = GetParamValue(parameters, FORM_TEMPLATE, null);
                        string formAttributeName = GetParamValue(parameters, FORM_ATTRIBUTE, null);

                        if (!string.IsNullOrEmpty(templateName))
                        {
                            // refresh instance using form template without updating database
                            updatedInstance = RefreshCustomFormDataUsingFile(schemaName, className, null, templateName, property, instance);
                        }
                        else if (!string.IsNullOrEmpty(formAttributeName))
                        {
                            // refresh instance using form template specified by the value of an attribute without updating database
                            updatedInstance = RefreshCustomFormDataUsingAttribute(schemaName, className, null, formAttributeName, property, instance);

                            //ErrorLog.Instance.WriteLine("****After convert****");
                            //var jsonString = JsonConvert.SerializeObject(updatedInstance, Newtonsoft.Json.Formatting.Indented);
                            //ErrorLog.Instance.WriteLine(jsonString);
                        }
                        else
                        {
                            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                            InstanceView instanceView = new InstanceView(dataView);

                            InstanceEditor instanceEditor = new InstanceEditor();
                            instanceEditor.EditInstance = instanceView;
                            instanceEditor.ConvertToModel(instance); // // translate the JSON instance data to InstanceView instance data

                            // execute the callback script
                            RunCallbackCode(con, instanceView, property, instance);

                            updatedInstance = instanceEditor.ConvertToViewModel(true); // convert to view model, so enum property's values are internal values
                        }
                    }
                });

                return Ok(updatedInstance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Call the callback function defined for a class on an existing data instance.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="property">The name of a field in a web form invoke this api with its value change</param>
        /// <param name="instance">The json object to be updated by the callback</param>
        /// <param name="template">When posting from a web form, providing a name of the form template such as RequestForm.htm. Default to null</param>
        /// <param name="formAttribute">When posting from a web form, providing a name of the attribute that stores a name of the form template such as InfoForm. Default to null</param>
        /// <remarks>This api is normally called when a field in a web form is changed. The "template" and "formAttribute" parameters are mutually-exclusive. They can be all null or only one should be provided.</remarks>
        [HttpPost]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}/{property}/refresh")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object with updated values")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> RefreshExisting(string schemaName, string className, string oid, string property, dynamic instance, string template = null, string formAttribute = null)
        {
            try
            {
                JObject updatedInstance = null;

                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                        string templateName = GetParamValue(parameters, FORM_TEMPLATE, null);
                        string formAttributeName = GetParamValue(parameters, FORM_ATTRIBUTE, null);

                        if (!string.IsNullOrEmpty(templateName))
                        {
                            // refresh instance using form template without updating database
                            updatedInstance = RefreshCustomFormDataUsingFile(schemaName, className, oid, templateName, property, instance);
                        }
                        else if (!string.IsNullOrEmpty(formAttributeName))
                        {
                            // refresh instance using form template specified by the value of an attribute without updating database
                            updatedInstance = RefreshCustomFormDataUsingAttribute(schemaName, className, oid, formAttributeName, property, instance);

                            //ErrorLog.Instance.WriteLine("****After convert****");
                            //var jsonString = JsonConvert.SerializeObject(updatedInstance, Newtonsoft.Json.Formatting.Indented);
                            //ErrorLog.Instance.WriteLine(jsonString);
                        }
                        else
                        {
                            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                            string query = null;
                            CMCommand cmd;
                            InstanceView instanceView = null;

                            // get the existing instance
                            query = dataView.GetInstanceQuery(oid);

                            cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            XmlReader reader = cmd.ExecuteXMLReader();
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                            {
                                // the instance with the obj_id exists
                                instanceView = new InstanceView(dataView, ds);
                            }
                            else
                            {
                                throw new Exception("Unable to find a data instance with obj_id " + oid);
                            }

                            InstanceEditor instanceEditor = new InstanceEditor();
                            instanceEditor.EditInstance = instanceView;
                            instanceEditor.ConvertToModel(instance); // // translate the JSON instance data to InstanceView instance data

                            // execute the callback script
                            RunCallbackCode(con, instanceView, property, instance);

                            updatedInstance = instanceEditor.ConvertToViewModel(true); // convert to view model, so enum property's values are internal values
                        }
                    }
                });

                return Ok(updatedInstance);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Execute a cusotm API to update an exsting data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">the obj_id of a data instance in a class</param>
        /// <param name="method">A name of a cusotm api such as UpdateOrder</param>
        [HttpPost]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}/custom/{method}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object as result of executing a custom api")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public HttpResponseMessage UpdateCustom(string schemaName, string className, string oid, string method)
        {
            try
            {
                JObject result = null;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    NameValueCollection parameter = Request.RequestUri.ParseQueryString();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    // run the specified api on the instance
                    Api api = con.MetaDataModel.ApiManager.GetClassApi(className, method);
                    if (api != null &&
                        !string.IsNullOrEmpty(api.ExternalHanlder) &&
                        api.MethodType == MethodType.Update)
                    {
                        string contentType = Request.Content.Headers.ContentType.ToString();
                        string postData = Request.Content.ReadAsStringAsync().Result;
                        dynamic instanceObject = queryHelper.ConvertToObject(postData, contentType);

                        ApiExecutionContext context = new ApiExecutionContext();
                        Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                        schemaInfo.Name = schemaName;
                        schemaInfo.Version = "1.0";
                        context.SchemaId = schemaInfo.NameAndVersion; // schemaName1.0
                        context.ClassName = className;
                        context.DataView = dataView;
                        context.ObjID = oid;
                        context.InstanceData = instanceObject;
                        context.Connection = con;
                        context.Parameters = parameter;
                        context.AcceptType = "application/json";
                        context.ContentType = contentType;

                        IApiFunction function = queryHelper.CreateExternalHandler(api.ExternalHanlder);
                        if (function != null)
                        {
                            result = function.Execute(context);

                            //var jsonString = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
                            //ErrorLog.Instance.WriteLine(jsonString);
                        }
                        else
                        {
                            throw new Exception("Unable to create the custom function with definition " + api.ExternalHanlder);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("Api " + method + " does not exist or has wrong definitions.");
                    }
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                string str = JsonConvert.SerializeObject(result);

                resp.Content = new StringContent(str, System.Text.Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return resp;
            }
        }

        /// <summary>
        /// Execute a cusotm API to create a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="method">A name of a cusotm api such as UpdateOrder</param>
        [HttpPost]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/custom/{method}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object as result of executing a custom api")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public HttpResponseMessage CreateCustom(string schemaName, string className, string method)
        {
            try
            {
                JObject result = new JObject();
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    NameValueCollection parameter = Request.RequestUri.ParseQueryString();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    // run the specified api on the instance
                    Api api = con.MetaDataModel.ApiManager.GetClassApi(className, method);
                    if (api != null &&
                        !string.IsNullOrEmpty(api.ExternalHanlder) &&
                        api.MethodType == MethodType.Create)
                    {
                        string postData = Request.Content.ReadAsStringAsync().Result;
                        string contentType = Request.Content.Headers.ContentType.ToString();
                        dynamic instanceObject = queryHelper.ConvertToObject(postData, contentType);

                        ApiExecutionContext context = new ApiExecutionContext();
                        Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                        schemaInfo.Name = schemaName;
                        schemaInfo.Version = "1.0";
                        context.SchemaId = schemaInfo.NameAndVersion; // schemaName1.0
                        context.ClassName = className;
                        context.DataView = dataView;
                        context.Parameters = parameter;
                        context.InstanceData = instanceObject;
                        context.Connection = con;
                        context.AcceptType = "application/json";
                        context.ContentType = contentType;

                        IApiFunction function = queryHelper.CreateExternalHandler(api.ExternalHanlder);
                        if (function != null)
                        {
                            result = function.Execute(context);

                            //var jsonString = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
                            //ErrorLog.Instance.WriteLine(jsonString);
                        }
                        else
                        {
                            throw new Exception("Unable to create the custom function with definition " + api.ExternalHanlder);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("Api " + method + " does not exist or has wrong definitions.");
                    }
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                string str = JsonConvert.SerializeObject(result);

                resp.Content = new StringContent(str, System.Text.Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return resp;
            }
        }

        /// <summary>
        /// Delete an instance from a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">the obj_id of a data instance to be deleted</param>
        [HttpDelete]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "The data instance has been deleted successfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> Delete(string schemaName, string className, string oid)
        {
            try
            {
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        DataViewModel dataView = con.MetaDataModel.GetDefaultDataView(className);

                        if (dataView != null)
                        {
                            // create a delete query
                            dataView.CurrentObjId = oid;

                            string query = dataView.DeleteQuery;

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            cmd.ExecuteXMLDoc();
                        }
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Delete a relatiosnhip between a master and related instances
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="rClassName">The name of a class related to the master class such as ATestItemInstance. The class can be related to the master class through one-to-one, one-to-many, many-to-one, or many-to-many relationhsip, and can be a subclass of a class that has the relationship.</param>
        /// <param name="roid">the obj_id of a data instance in the related class such as 883939992</param>
        [HttpDelete]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/{schemaName}/{className}/{oid:long}/{rClassName}/{roid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "The relationship has been deleted successfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DeleteRelated(string schemaName, string className, string oid, string rClassName, string roid)
        {
            try
            {
                QueryHelper queryHelper = new QueryHelper();
                string relatedClassName = rClassName;
                string masterInstanceId = oid;
                string masterClassName = className;
                string relatedInstanceId = roid;

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection connection = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        connection.Open();

                        MetaDataModel metaData = connection.MetaDataModel;

                        string junctionClassName = GetManyToManyClassName(metaData, className, rClassName);

                        if (junctionClassName != null)
                        {
                            // first make sure the many-to-many link doesn't exist (it could add by another user)
                            DataViewModel junctionClassDataView = metaData.GetDefaultDataView(junctionClassName);
                            junctionClassDataView.ClearSearchExpression();

                            DataClass leftSideClass = null;
                            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
                            {
                                if (relatedClass.ClassName == masterClassName || IsParentOf(metaData, relatedClass.ClassName, masterClassName))
                                {
                                    if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
                                    {

                                        leftSideClass = relatedClass;
                                        break;
                                    }
                                }
                            }

                            DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias);
                            Newtera.Common.MetaData.DataView.Parameter right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias, DataType.String);
                            right.ParameterValue = masterInstanceId;
                            RelationalExpr expr = new RelationalExpr(ElementType.Equals, left, right);

                            // add search expression to the dataview
                            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

                            DataClass rightSideClass = null;
                            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
                            {
                                if (relatedClass.ClassName == relatedClassName || IsParentOf(metaData, relatedClass.ClassName, relatedClassName))
                                {
                                    if (relatedClass != leftSideClass && relatedClass.ReferringRelationship.IsForeignKeyRequired)
                                    {
                                        rightSideClass = relatedClass;
                                        break;
                                    }
                                }
                            }

                            left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias);
                            right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias, DataType.String);
                            right.ParameterValue = relatedInstanceId;
                            expr = new RelationalExpr(ElementType.Equals, left, right);

                            // add search expression to the dataview
                            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

                            // search the relationship
                            string query = junctionClassDataView.SearchQuery;
                            CMCommand cmd = connection.CreateCommand();
                            cmd.CommandText = query;

                            // Since the result will be displayed on DataGridView, we don't need to check
                            // write permissions on each attribute, use CMCommandBehavior.CheckReadPermissionOnly
                            XmlReader reader = cmd.ExecuteXMLReader(CMCommandBehavior.CheckReadPermissionOnly);
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            if (!DataSetHelper.IsEmptyDataSet(ds, junctionClassDataView.BaseClass.ClassName))
                            {
                                InstanceView instanceView = new InstanceView(junctionClassDataView, ds);
                                string instanceId = instanceView.InstanceData.ObjId;

                                // delete the relationship
                                junctionClassDataView.CurrentObjId = instanceId;

                                query = junctionClassDataView.DeleteQuery;

                                cmd = connection.CreateCommand();
                                cmd.CommandText = query;

                                cmd.ExecuteXMLDoc(); // delete
                            }
                            else
                            {
                                throw new Exception("Unable to find the relationship to be deleted.");
                            }
                        }
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Execute a data extraction process for a data instance defined by a xml schema definition.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="xmlSchemaName">The name of an xml schema that defines a program of the extraction</param>
        /// <remarks>This api is used by SmartExcel or other tools to extract data for generating a report</remarks>
        [HttpGet]
        [AuthorizeByMetaDataAttribute]
        [Route("api/data/extract/{schemaName}/{className}/{oid}/{xmlSchemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A hierarchical json object(s) representing the extracted data based on the xml schema")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ExtractInstanceData(string schemaName, string className, string oid, string xmlSchemaName)
        {
            try
            {
                string jsonStr = null;
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection connection = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        connection.Open();

                        XMLSchemaModel xmlSchemaModel = (XMLSchemaModel)connection.MetaDataModel.XMLSchemaViews[xmlSchemaName];

                        // make sure the xmlSchemaModel is defined for the given base class
                        if (xmlSchemaModel != null &&
                            xmlSchemaModel.RootElement.ElementType == className)
                        {
                            DataViewModel dataView = connection.MetaDataModel.GetDetailedDataView(className);
                            string query = dataView.GetInstanceQuery(oid);

                            CMCommand cmd = connection.CreateCommand();
                            cmd.CommandText = query;

                            XmlDocument doc;

                            // Get a hierarchical data structure in XML
                            doc = cmd.GenerateXmlDoc(xmlSchemaModel, dataView);

                            StringWriter sw;
                            XmlTextWriter tx;
                            // convert xml doc into text
                            sw = new StringWriter();
                            tx = new XmlTextWriter(sw);
                            doc.WriteTo(tx);

                            // Convert xmldocument to json string and json
                            jsonStr = JsonConvert.SerializeXmlNode(doc);

                            //ErrorLog.Instance.WriteLine("Extracted JSON string=" + jsonStr);
                        }
                        else
                        {
                            throw new Exception("Unable to find a xmls schema " + xmlSchemaName + " for the class " + className);
                        }
                    }
                });

                return Ok(JObject.Parse(jsonStr));
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get a InstanceView object
        /// </summary>
        /// <param name="connection">DB connection</param>
        /// <param name="className">Class name of the instance view</param>
        /// <param name="oid">ObjId of the instance</param>
        /// <returns>An InstanceView object</returns>
        private InstanceView GetInstanceView(CMConnection connection, string className, string oid)
        {
            InstanceView instanceView;

            MetaDataModel metaData = connection.MetaDataModel;

            DataViewModel dataView = metaData.GetDetailedDataView(className);

            string query = dataView.GetInstanceQuery(oid);

            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                instanceView = new InstanceView(dataView, ds);
            }
            else
            {
                throw new Exception("Unable to find the data instance for id " + oid);
            }

            return instanceView;
        }

        private bool IsParentOf(MetaDataModel metaData, string parentClassName, string childClassName)
        {
            bool status = false;

            ClassElement childClassElement = metaData.SchemaModel.FindClass(childClassName);
            if (childClassElement == null)
            {
                throw new Exception("Unable to find class element for " + childClassName);
            }

            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                status = true;
            }

            return status;
        }

        private string GetManyToManyClassName(MetaDataModel metaData, string masterClassName, string relatedClassName)
        {
            ClassElement masterClassElement = metaData.SchemaModel.FindClass(masterClassName);
            RelationshipAttributeElement relationshipElement = null;

            ClassElement currentClassElement = masterClassElement;
            while (currentClassElement != null)
            {
                foreach (RelationshipAttributeElement relationshipAttribute in currentClassElement.RelationshipAttributes)
                {
                    if (relationshipAttribute.IsBrowsable && relationshipAttribute.LinkedClass.IsJunction)
                    {
                        RelationshipAttributeElement toReferencedClsRelationshipAttribute = relationshipAttribute.LinkedClass.FindPairedRelationshipAttribute(relationshipAttribute);
                        if (toReferencedClsRelationshipAttribute.LinkedClassName == relatedClassName ||
                            IsParentOf(metaData, toReferencedClsRelationshipAttribute.LinkedClassName, relatedClassName))
                        {
                            relationshipElement = relationshipAttribute;
                            break;
                        }
                    }
                }

                if (relationshipElement != null)
                {
                    break;
                }

                currentClassElement = currentClassElement.ParentClass; // go to parent class element
            }

            if (relationshipElement != null)
            {
                return relationshipElement.LinkedClassName;
            }
            else
            {
                return null;
            }
        }

        private string GetManyToOneRelationshipName(MetaDataModel metaData, string masterClassName, string relatedClassName)
        {
            ClassElement masterClassElement = metaData.SchemaModel.FindClass(masterClassName);
            RelationshipAttributeElement relationshipElement = null;

            ClassElement currentClassElement = masterClassElement;
            while (currentClassElement != null)
            {
                foreach (RelationshipAttributeElement relationshipAttribute in currentClassElement.RelationshipAttributes)
                {
                    if (relationshipAttribute.IsBrowsable && relationshipAttribute.LinkedClass.Name == relatedClassName)
                    {
                        RelationshipAttributeElement toReferencedClsRelationshipAttribute = relationshipAttribute.LinkedClass.FindPairedRelationshipAttribute(relationshipAttribute);
                        if (toReferencedClsRelationshipAttribute.LinkedClassName == relatedClassName ||
                            IsParentOf(metaData, toReferencedClsRelationshipAttribute.LinkedClassName, relatedClassName))
                        {
                            relationshipElement = relationshipAttribute;
                            break;
                        }
                    }
                }

                if (relationshipElement != null)
                {
                    break;
                }

                currentClassElement = currentClassElement.ParentClass; // go to parent class element
            }

            if (relationshipElement != null)
            {
                return relationshipElement.LinkedClassName;
            }
            else
            {
                return null;
            }
        }

        private void SetRelationshipValue(CMConnection connection, InstanceView instanceView,
            DataRelationshipAttribute relationshipAttribute, DataViewModel relatedDataView, string relatedInstanceId)
        {
            // first, we need get the primary key values of the related instance
            string query = relatedDataView.GetInstanceQuery(relatedInstanceId);
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;
            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            InstanceView relatedInstanceView = new InstanceView(relatedDataView, ds);
            string[] pkValues = relatedInstanceView.InstanceData.PrimaryKeyValues.Split('&');
            // then set the primary key value as foreign key values to the instance view
            DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
            if (primaryKeys != null)
            {
                int index = 0;
                foreach (DataSimpleAttribute pk in primaryKeys)
                {
                    if (index < pkValues.Length && pkValues[index] != null)
                    {
                        // to set a pk value, the name combines that of relationship attribute and primary key
                        instanceView.InstanceData.SetAttributeValue(relationshipAttribute.Name + "_" + pk.Name, pkValues[index].Trim());
                    }
                    index++;
                }
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

        /// <summary>
        /// Run the before insert code on the instance
        /// </summary>
        private void RunBeforeInsertCode(CMConnection connection, InstanceView instanceView, dynamic instance)
        {
            ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.Name);

            // Execute the before insert code
            string beforeInsertCode = GetClassCustomCode(classElement, ClassElement.CLASS_BEFORE_INSERT_CODE);
            if (!string.IsNullOrEmpty(beforeInsertCode))
            {
                // Execute the before updare code
                IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView, connection.ConnectionString, instance); ;

                ActionCodeRunner.Instance.ExecuteActionCode("ClassInsert" + classElement.ID, beforeInsertCode, instanceWrapper);
            }
        }

        /// <summary>
        /// Run the before update code on the instance
        /// </summary>
        private void RunBeforeUpdateCode(CMConnection connection, InstanceView instanceView, dynamic instance)
        {
            ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.Name);

            // Execute the before update code
            string beforeUpdateCode = GetClassCustomCode(classElement, ClassElement.CLASS_BEFORE_UPDATE_CODE);
            if (!string.IsNullOrEmpty(beforeUpdateCode))
            {
                // Execute the before updare code
                IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView, connection.ConnectionString, instance); ;

                ActionCodeRunner.Instance.ExecuteActionCode("ClassUpdate" + classElement.ID, beforeUpdateCode, instanceWrapper);
            }
        }

        /// <summary>
        /// Run the callback code on the instance
        /// </summary>
        private void RunCallbackCode(CMConnection connection, InstanceView instanceView, string propertyName, dynamic instance)
        {
            ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.Name);

            // Execute the callback code
            if (!string.IsNullOrEmpty(classElement.CallbackFunctionCode))
            {
                // Execute the callback code
                IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView, connection.ConnectionString, instance); ;

                ActionCodeRunner.Instance.ExecuteActionCode("ClassCallback" + classElement.ID, classElement.CallbackFunctionCode, instanceWrapper, propertyName);
            }
        }

        /// <summary>
        /// Gets data for a cusotm form using a template specified by an attribute value of an existing instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="formAttributeName">the custom template name</param>
        /// <returns>A JObject instance</returns>
        private JObject GetCustomFormDataUsingAttribute(string schemaName, string className, string oid, string formAttributeName)
        {
            JObject instance = null;

            string templateName = GetTemplateNameFromAttribute(schemaName, className, oid, formAttributeName);

            if (!string.IsNullOrEmpty(templateName))
            {
                instance = GetCustomFormDataUsingFile(schemaName, className, oid, templateName);
            }
            else
            {
                ErrorLog.Instance.WriteLine("Unable to find a template in the attribute " + formAttributeName + " of the instance " + oid + " in the class " + className);
            }

            return instance;
        }

        private string GetTemplateNameFromAttribute(string schemaName, string className, string oid, string formAttributeName)
        {
            string templateName = null;

            QueryHelper queryHelper = new QueryHelper();

            using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                // create an instance query
                string query = dataView.GetInstanceQuery(oid);

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    InstanceView instanceView = new InstanceView(dataView, ds);

                    templateName = instanceView.InstanceData.GetAttributeStringValue(formAttributeName);
                }
            }

            return templateName;
        }

        private JObject GetDataForFullView(string schemaName, string className, string oid, bool formFormat)
        {
            return GetDataForView(schemaName, className, FULL_VIEW, oid, formFormat);
        }

        private JObject GetDataForView(string schemaName, string className, string viewName, string oid, bool formFormat)
        {
            JObject instance = null;
            QueryHelper queryHelper = new QueryHelper();

            using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = null;

                if (viewName == SIMPLE_VIEW)
                {
                    dataView = con.MetaDataModel.GetDefaultDataView(className);
                }
                else if (viewName == FULL_VIEW)
                {
                    dataView = con.MetaDataModel.GetDetailedDataView(className);
                }
                else
                {
                    dataView = con.MetaDataModel.DataViews[viewName] as DataViewModel;
                    if (dataView != null)
                    {
                        dataView = dataView.Clone();
                    }
                }

                // create an instance query
                string query = dataView.GetInstanceQuery(oid);

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
                    if (formFormat)
                    {
                        instance = instanceEditor.ConvertToViewModel(true); // convert to view model, so enum property's values are internal values
                    }
                    else
                    {
                        instance = instanceEditor.ConvertToViewModel(false); // do not convert to view model, so enum property's values are displayed values
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Gets data for a cusotm form using a file-based template for an existing instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="templateName">the custom template name</param>
        /// <returns>A JObject instance</returns>
        private JObject GetCustomFormDataUsingFile(string schemaName, string className, string oid, string templateName)
        {
            try
            {
                JObject instance = null;
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    if (dataView != null)
                    {
                        InstanceView instanceView;

                        if (!string.IsNullOrEmpty(oid))
                        {
                            // create an instance query
                            string query = dataView.GetInstanceQuery(oid);

                            CMCommand cmd = con.CreateCommand();
                            cmd.CommandText = query;

                            XmlReader reader = cmd.ExecuteXMLReader();
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                            {
                                instanceView = new InstanceView(dataView, ds);
                            }
                            else
                            {
                                instanceView = new InstanceView(dataView);
                            }
                        }
                        else
                        {
                            instanceView = new InstanceView(dataView);

                            // execute initialization code on the new instance
                            ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                            string initializationCode = GetClassCustomCode(classElement, ClassElement.CLASS_INITIALIZATION_CODE);
 
                            if (!string.IsNullOrEmpty(initializationCode))
                            {
                                // initialize the new instance with initialization code

                                // Execute the initialization code using the same connection so that changes are made within a same transaction
                                IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView);

                                // run the initialization code on the instance
                                ActionCodeRunner.Instance.ExecuteActionCode("GetCustomFormNew", "ClassInit" + classElement.ID, initializationCode, instanceWrapper);
                            }
                        }

                        CustomFormEditor formEditor = new CustomFormEditor();
                        formEditor.EditInstance = instanceView;
                        formEditor.ConnectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
                        SetTemplateToEditor(con, formEditor, className, templateName);

                        instance = formEditor.ConvertToViewModel(true);
                    }
                    else
                    {
                        throw new Exception("Unable to create a data view for class " + className);
                    }
                }

                //var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
                //ErrorLog.Instance.WriteLine(jsonString);

                return instance;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        private void SetTemplateToEditor(CMConnection con, CustomFormEditor formEditor, string className, string templateName)
        {
            bool foundTemplate = false;
            string templateBaseClass = className;
            string schemaId = con.MetaDataModel.SchemaInfo.NameAndVersion;

            while (true)
            {
                string formTemplatePath = NewteraNameSpace.GetFormTemplateDir(schemaId, templateBaseClass) + templateName;
                if (File.Exists(formTemplatePath))
                {
                    formEditor.TemplatePath = formTemplatePath;
                    foundTemplate = true;
                    break;
                }
                else
                {
                    // try to see if the template is defined for one of the parent classes
                    templateBaseClass = GetParentClassName(con, templateBaseClass);
                    if (templateBaseClass == null)
                    {
                        // no parent class
                        break;
                    }
                }
            }

            if (!foundTemplate)
            {
                throw new Exception("Unable to find a template with name " + templateName);
            }
        }

        private string GetParentClassName(CMConnection con, string childClassName)
        {
            ClassElement childClassElement = con.MetaDataModel.SchemaModel.FindClass(childClassName);
            if (childClassElement != null && childClassElement.ParentClass != null)
            {
                return childClassElement.ParentClass.Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Update data for a cusotm form using a template specified by an attribute value of an existing instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="formAttributeName">the custom template name</param>
        /// <param name="instance">The instance to be updated</param>
        /// <param name="needValidate">true to validate the instance first</param>
        /// <returns>The updated instance in json object</returns>
        private JObject UpdateCustomFormDataUsingAttribute(string schemaName, string className, string oid, string formAttributeName, dynamic instance, bool needValidate)
        {
            JObject updatedInstance = null;

            string templateName = GetTemplateNameFromAttribute(schemaName, className, oid, formAttributeName);

            if (!string.IsNullOrEmpty(templateName))
            {
                updatedInstance = UpdateCustomFormDataUsingFile(schemaName, className, oid, templateName, instance, needValidate);
            }
            else
            {
                throw new Exception("Unable to find a template in the attribute " + formAttributeName + " of the instance " + oid + " in the class " + className);
            }

            return updatedInstance;
        }

        /// <summary>
        /// Refresh data for a cusotm form using a template specified by an attribute value of an existing instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="formAttributeName">the custom template name</param>
        /// <param name="propertyName">The property whose value change invokes callback</param>
        /// <param name="instance">The instance to be updated</param>
        /// <returns>The refreshed instance in json object</returns>
        private JObject RefreshCustomFormDataUsingAttribute(string schemaName, string className, string oid, string formAttributeName, string propertyName, dynamic instance)
        {
            JObject updatedInstance = null;

            string templateName = GetTemplateNameFromAttribute(schemaName, className, oid, formAttributeName);

            if (!string.IsNullOrEmpty(templateName))
            {
                updatedInstance = RefreshCustomFormDataUsingFile(schemaName, className, oid, templateName, propertyName, instance);
            }
            else
            {
                throw new Exception("Unable to find a template in the attribute " + formAttributeName + " of the instance " + oid + " in the class " + className);
            }

            return updatedInstance;
        }

        /// <summary>
        /// Update a data instance for a cusotm form using a file-based template
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="templateName">the custom template name</param>
        /// <param name="instance">The instance to be updated</param>
        /// <param name="needValidate">true to validate the instance first</param>
        /// <returns>The updated instance in json object</returns>
        private JObject UpdateCustomFormDataUsingFile(string schemaName, string className, string oid, string templateName, dynamic instance, bool needValidate)
        {
            try
            {
                JObject updatedInstance = null;

                InstanceView instanceView;

                QueryHelper queryHelper = new QueryHelper();

                // get formId to be used when running BeforeUpdateCode
                IEnumerable<string> headerValues;
                var formId = string.Empty;
                if (Request.Headers.TryGetValues("formId", out headerValues))
                {
                    formId = headerValues.FirstOrDefault();
                }

                // get taskid and action name to be used when running CustomActionCode
                var taskId = string.Empty;
                if (Request.Headers.TryGetValues("taskId", out headerValues))
                {
                    taskId = headerValues.FirstOrDefault();
                }

                var actionId = string.Empty;
                if (Request.Headers.TryGetValues("actionId", out headerValues))
                {
                    actionId = headerValues.FirstOrDefault();
                }

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    if (!string.IsNullOrEmpty(oid))
                    {
                        // create an instance query
                        string query = dataView.GetInstanceQuery(oid);

                        CMCommand cmd = con.CreateCommand();
                        cmd.CommandText = query;

                        XmlReader reader = cmd.ExecuteXMLReader();
                        DataSet ds = new DataSet();
                        ds.ReadXml(reader);

                        if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                        {
                            instanceView = new InstanceView(dataView, ds);
                        }
                        else
                        {
                            instanceView = new InstanceView(dataView);
                        }
                    }
                    else
                    {
                        instanceView = new InstanceView(dataView);
                    }

                    CustomFormEditor formEditor = new CustomFormEditor();
                    formEditor.FormId = formId;
                    formEditor.TaskId = taskId;
                    formEditor.ActionId = actionId;
                    formEditor.EditInstance = instanceView;
                    formEditor.Instance = instance;
                    formEditor.ConnectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
                    SetTemplateToEditor(con, formEditor, className, templateName);

                    List<IInstanceEditor> relatedInstances = formEditor.ConvertToModel(instance);

                    // save the data in form to the database
                    formEditor.SaveToDatabase(relatedInstances);

                    updatedInstance = GetCustomFormDataUsingFile(schemaName, className, formEditor.EditInstance.InstanceData.ObjId, templateName);
                }

                return updatedInstance; // the added or updated instance in json
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        /// <summary>
        /// REfresh a data instance for a cusotm form using a file-based template
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="templateName">the custom template name</param>
        /// <param name="propertyName">The property whose value change invokes the callback</param>
        /// <param name="instance">The instance to be refreshed</param>
        /// <returns>The refreshed instance in json object</returns>
        private JObject RefreshCustomFormDataUsingFile(string schemaName, string className, string oid, string templateName, string propertyName, dynamic instance)
        {
            try
            {
                JObject updatedInstance = null;

                InstanceView instanceView;

                QueryHelper queryHelper = new QueryHelper();

                // get formId to be used when running BeforeUpdateCode
                IEnumerable<string> headerValues;
                var formId = string.Empty;
                if (Request.Headers.TryGetValues("formId", out headerValues))
                {
                    formId = headerValues.FirstOrDefault();
                }

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    if (!string.IsNullOrEmpty(oid))
                    {
                        // create an instance query
                        string query = dataView.GetInstanceQuery(oid);

                        CMCommand cmd = con.CreateCommand();
                        cmd.CommandText = query;

                        XmlReader reader = cmd.ExecuteXMLReader();
                        DataSet ds = new DataSet();
                        ds.ReadXml(reader);

                        if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                        {
                            instanceView = new InstanceView(dataView, ds);
                        }
                        else
                        {
                            instanceView = new InstanceView(dataView);
                        }
                    }
                    else
                    {
                        instanceView = new InstanceView(dataView);
                    }

                    CustomFormEditor formEditor = new CustomFormEditor();
                    formEditor.FormId = formId;
                    formEditor.EditInstance = instanceView;
                    formEditor.ConnectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
                    SetTemplateToEditor(con, formEditor, className, templateName);

                    // convert view model to model
                    formEditor.ConvertToModel(instance);

                    // execute the callback script
                    RunCallbackCode(con, instanceView, propertyName, instance);

                    // convert refreshed model back to view model
                    updatedInstance = formEditor.ConvertToViewModel(true);
                }

                return updatedInstance; // the added or updated instance in json
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        private void CreateManyToManyRelationship(CMConnection connection, MetaDataModel metaData, string masterClassName, string masterInstanceId, string relatedClassName, string relatedInstanceId, string junctionClassName)
        {
            // first make sure the many-to-many link doesn't exist (it could add by another user)
            DataViewModel junctionClassDataView = metaData.GetDefaultDataView(junctionClassName);
            junctionClassDataView.ClearSearchExpression();

            DataClass leftSideClass = null;
            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == masterClassName || IsParentOf(metaData, relatedClass.ClassName, masterClassName))
                {
                    if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
                    {

                        leftSideClass = relatedClass;
                        break;
                    }
                }
            }

            DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias);
            Newtera.Common.MetaData.DataView.Parameter right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, leftSideClass.Alias, DataType.String);
            right.ParameterValue = masterInstanceId;
            RelationalExpr expr = new RelationalExpr(ElementType.Equals, left, right);

            // add search expression to the dataview
            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

            DataClass rightSideClass = null;
            foreach (DataClass relatedClass in junctionClassDataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == relatedClassName || IsParentOf(metaData, relatedClass.ClassName, relatedClassName))
                {
                    if (relatedClass != leftSideClass && relatedClass.ReferringRelationship.IsForeignKeyRequired)
                    {
                        rightSideClass = relatedClass;
                        break;
                    }
                }
            }

            left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias);
            right = new Newtera.Common.MetaData.DataView.Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, rightSideClass.Alias, DataType.String);
            right.ParameterValue = relatedInstanceId;
            expr = new RelationalExpr(ElementType.Equals, left, right);

            // add search expression to the dataview
            junctionClassDataView.AddSearchExpr(expr, ElementType.And);

            // search the relationship
            string query = junctionClassDataView.SearchQuery;
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            // Since the result will be displayed on DataGridView, we don't need to check
            // write permissions on each attribute, use CMCommandBehavior.CheckReadPermissionOnly
            XmlReader reader = cmd.ExecuteXMLReader(CMCommandBehavior.CheckReadPermissionOnly);
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (DataSetHelper.IsEmptyDataSet(ds, junctionClassDataView.BaseClass.ClassName))
            {
                InstanceView instanceView = new InstanceView(junctionClassDataView);
                DataRelationshipAttribute leftRelationshipAttribute = null;
                foreach (IDataViewElement resultAttribute in junctionClassDataView.ResultAttributes)
                {
                    leftRelationshipAttribute = resultAttribute as DataRelationshipAttribute;
                    if (leftRelationshipAttribute != null &&
                        leftRelationshipAttribute.IsForeignKeyRequired)
                    {
                        if (leftRelationshipAttribute.LinkedClassName == masterClassName ||
                            IsParentOf(metaData, leftRelationshipAttribute.LinkedClassName, masterClassName))
                        {
                            DataViewModel masterClassDataView = metaData.GetDetailedDataView(masterClassName);

                            SetRelationshipValue(connection, instanceView, leftRelationshipAttribute, masterClassDataView, masterInstanceId);

                            break;
                        }
                    }
                }

                DataRelationshipAttribute rightRelationshipAttribute;
                foreach (IDataViewElement resultAttribute in junctionClassDataView.ResultAttributes)
                {
                    rightRelationshipAttribute = resultAttribute as DataRelationshipAttribute;
                    if (rightRelationshipAttribute != null &&
                        rightRelationshipAttribute != leftRelationshipAttribute &&
                        rightRelationshipAttribute.IsForeignKeyRequired)
                    {
                        if (rightRelationshipAttribute.LinkedClassName == relatedClassName ||
                            IsParentOf(metaData, rightRelationshipAttribute.LinkedClassName, relatedClassName))
                        {
                            DataViewModel relatedClassDataView = metaData.GetDetailedDataView(relatedClassName);

                            SetRelationshipValue(connection, instanceView, rightRelationshipAttribute, relatedClassDataView, relatedInstanceId);

                            break;

                        }
                    }
                }

                query = junctionClassDataView.InsertQuery;

                cmd = connection.CreateCommand();
                cmd.CommandText = query;

                cmd.ExecuteXMLDoc(); // insert
            }
        }

        private JObject GetOneInstance(string schemaName, string className, string oid)
        {
            JObject instance = null;
            QueryHelper queryHelper = new QueryHelper();

            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string templateName = GetParamValue(parameters, FORM_TEMPLATE, null);
            string formAttributeName = GetParamValue(parameters, FORM_ATTRIBUTE, null);
            string viewName = GetParamValue(parameters, DATA_VIEW, FULL_VIEW);

            bool formFormat = bool.Parse(GetParamValue(parameters, FORM_FORMAT, "false"));

            if (!string.IsNullOrEmpty(templateName))
            {
                // get instance using form template
                instance = GetCustomFormDataUsingFile(schemaName, className, oid, templateName);

                if (instance == null)
                {
                    // unable to find the form template, get a default instance
                    instance = GetDataForFullView(schemaName, className, oid, formFormat);
                }
            }
            else if (!string.IsNullOrEmpty(formAttributeName))
            {
                // get instance using form template specified by the value of an attribute
                instance = GetCustomFormDataUsingAttribute(schemaName, className, oid, formAttributeName);
                if (instance == null)
                {
                    // unable to find the form template from the formAttributeName, get a default instance
                    instance = GetDataForFullView(schemaName, className, oid, formFormat);
                }
            }
            else if (HasDefaultFormTemplate(schemaName, className))
            {
                // get instance using form template
                instance = GetCustomFormDataUsingFile(schemaName, className, oid, DEFAULT_FORM);

                if (instance == null)
                {
                    // unable to find the form template, get a default instance
                    instance = GetDataForFullView(schemaName, className, oid, formFormat);
                }
            }
            else if (!string.IsNullOrEmpty(viewName))
            {
                instance = GetDataForView(schemaName, className, viewName, oid, formFormat);
            }
            else
            {
                //ErrorLog.Instance.WriteLine("Do not have default form ");
                instance = GetDataForFullView(schemaName, className, oid, formFormat);
            }

            //var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
            //ErrorLog.Instance.WriteLine(jsonString);

            return instance;
        }

        private JObject GetClonedInstance(string schemaName, string className, string oid)
        {
            JObject instance = null;
            QueryHelper queryHelper = new QueryHelper();

            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string templateName = GetParamValue(parameters, FORM_TEMPLATE, null);
            string formAttributeName = GetParamValue(parameters, FORM_ATTRIBUTE, null);
            string viewName = GetParamValue(parameters, DATA_VIEW, FULL_VIEW);
            bool deepClone = bool.Parse(GetParamValue(parameters, DEEP_CLONE, "false"));
            bool formFormat = bool.Parse(GetParamValue(parameters, FORM_FORMAT, "false"));

            string clonedId = CloneInstance(schemaName, className, oid, deepClone);

            // convert the cloned instance according to a form requirements
            if (!string.IsNullOrEmpty(templateName))
            {
                // get instance using form template
                instance = GetCustomFormDataUsingFile(schemaName, className, clonedId, templateName);

                if (instance == null)
                {
                    // unable to find the form template, get a default instance
                    instance = GetDataForFullView(schemaName, className, clonedId, formFormat);
                }
            }
            else if (!string.IsNullOrEmpty(formAttributeName))
            {
                // get instance using form template specified by the value of an attribute
                instance = GetCustomFormDataUsingAttribute(schemaName, className, clonedId, formAttributeName);
                if (instance == null)
                {
                    // unable to find the form template from the formAttributeName, get a default instance
                    instance = GetDataForFullView(schemaName, className, clonedId, formFormat);
                }
            }
            else if (!string.IsNullOrEmpty(viewName))
            {
                instance = GetDataForView(schemaName, className, viewName, clonedId, formFormat);
            }
            else
            {
                instance = GetDataForFullView(schemaName, className, clonedId, formFormat);
            }

            return instance;
        }

        private string CloneInstance(string schemaName, string className, string oid, bool deepClone)
        {
            QueryHelper queryHelper = new QueryHelper();
            InstanceView orignalInstanceView;
            InstanceView clonedInstanceView;

            using (CMConnection connection = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                connection.Open();

                // get original instance
                DataViewModel dataView = connection.MetaDataModel.GetDetailedDataView(className);

                CMCommand cmd = connection.CreateCommand();

                // create an instance query
                string query = dataView.GetInstanceQuery(oid);
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    orignalInstanceView = new InstanceView(dataView, ds);

                    clonedInstanceView = cmd.DeepCloneInstance(orignalInstanceView, deepClone);

                    return clonedInstanceView.InstanceData.ObjId;
                }
                else
                {
                    return "";
                }
            }
        }

        private bool HasDefaultFormTemplate(string schemaName, string className)
        {
            bool foundTemplate = false;
            string templateBaseClass = className;
            QueryHelper queryHelper = new QueryHelper();

            using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                string schemaId = con.MetaDataModel.SchemaInfo.NameAndVersion;

                while (true)
                {
                    string formTemplatePath = NewteraNameSpace.GetFormTemplateDir(schemaId, templateBaseClass) + DEFAULT_FORM;
                    if (File.Exists(formTemplatePath))
                    {
                        foundTemplate = true;
                        break;
                    }
                    else
                    {
                        // try to see if the template is defined for one of the parent classes
                        templateBaseClass = GetParentClassName(con, templateBaseClass);
                        if (templateBaseClass == null)
                        {
                            // no parent class
                            break;
                        }
                    }
                }

                if (foundTemplate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private NameValueCollection ParseParameters(string pair)
        {
            NameValueCollection parameters = new NameValueCollection();
            // pair is in form of name1=value1,name2=value2
            string[] nameValues = pair.Split(',');
            foreach (string nameValue in nameValues)
            {
                string[] eq = nameValue.Split(':');
                if (eq.Length == 2)
                {
                    parameters.Add(eq[0], eq[1]);
                }
            }

            return parameters;
        }

        private string GetClassCustomCode(ClassElement classElement, string codeBlock)
        {
            string code = null;

            code = GetCustomCode(classElement, codeBlock);

            if (string.IsNullOrEmpty(code) &&
                classElement.ParentClass != null)
            {
                // use the code defined in the parent class
                code = GetClassCustomCode(classElement.ParentClass, codeBlock);
            }
            
            return code;
        }

        private string GetCustomCode(ClassElement classElement, string codeBlock)
        {
            string code = null;

            switch (codeBlock)
            {
                case ClassElement.CLASS_INITIALIZATION_CODE:
                    code = classElement.InitializationCode;
                    break;

                case ClassElement.CLASS_BEFORE_INSERT_CODE:

                    code = classElement.BeforeInsertCode;
                    break;

                case ClassElement.CLASS_BEFORE_UPDATE_CODE:

                    code = classElement.BeforeUpdateCode;
                    break;

                case ClassElement.CLASS_CALLBACK_CODE:

                    code = classElement.CallbackFunctionCode;

                    break;
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = code.Trim();
            }

            return code;
        }

        private void CreateOneToManyRelationship(CMConnection connection, MetaDataModel metaData, string masterClassName, string masterInstanceId, string relatedClassName, string relatedInstanceId)
        {

            // find the relationship attribute that is a mant-to-one relationship from related class to the master class
            DataViewModel relatedClassDataView = metaData.GetDetailedDataView(relatedClassName);
            ClassElement relatedClassElement = metaData.SchemaModel.FindClass(relatedClassName);

            // Get an InstanceView for the related instance
            string query = relatedClassDataView.GetInstanceQuery(relatedInstanceId);
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;
            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, relatedClassDataView.BaseClass.ClassName))
            {
                InstanceView relatedInstanceView = new InstanceView(relatedClassDataView, ds);

                DataRelationshipAttribute relationshipAttribute = null;
                foreach (IDataViewElement resultAttribute in relatedClassDataView.ResultAttributes)
                {
                    relationshipAttribute = resultAttribute as DataRelationshipAttribute;
                    if (relationshipAttribute != null &&
                        relationshipAttribute.IsForeignKeyRequired)
                    {
                        if (relationshipAttribute.LinkedClassName == masterClassName ||
                            IsParentOf(metaData, relationshipAttribute.LinkedClassName, masterClassName))
                        {
                            DataViewModel masterClassDataView = metaData.GetDetailedDataView(masterClassName);

                            SetRelationshipValue(connection, relatedInstanceView, relationshipAttribute, masterClassDataView, masterInstanceId);

                            break;
                        }
                    }
                }

                // create a relationship
                if (relatedInstanceView.IsDataChanged)
                {
                    query = relatedClassDataView.UpdateQuery;

                    cmd = connection.CreateCommand();
                    cmd.CommandText = query;
                    cmd.NeedToRaiseEvents = false; // do not raise event for updating relationship 

                    cmd.ExecuteXMLDoc(); // create relationship

                    // Post an Insert Event after creating the relationship, so that subscriber defined in the related class can get the referenced master instance
                    EventContext eventContext = new EventContext(Guid.NewGuid().ToString(), metaData, relatedClassElement,
                        relatedInstanceId, Newtera.Common.MetaData.Events.OperationType.Insert);
                    DBEventQueueManager.Instance.PostEvent(eventContext);
                }
            }
        }

        private void Validate(CMConnection con, InstanceEditor instanceEditor,  ClassElement classElement, DataViewModel dataView)
        {
            instanceEditor.Validate(); // validate the instance before inserting. This method throw an error on validate error

            // validate the data instance using validating rules if any.
            RuleCollection validatingRules = con.MetaDataModel.RuleManager.GetPrioritizedRules(classElement).Rules;
            string msg = "";
            if (validatingRules != null && validatingRules.Count > 0)
            {
                if (!ValidateUsingRules(con, dataView, validatingRules, out msg))
                {
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// Validate the data instance using the validating rules
        /// </summary>
        /// <param name="connection">The CMConnection object</param>
        /// <param name="dataView">The data view that holds an instance data to be validated against.</param>
        /// <param name="rules">The validating rules.</param>
        /// <param name="message">The validating message if there is a validating error.</param>
        /// <returns>true if the data instance is valid, false otherwise.</returns>
        private bool ValidateUsingRules(CMConnection connection, DataViewModel dataView, RuleCollection rules, out string message)
        {
            bool isValid = true;
            message = "";
            string query;
            XmlDocument doc;
            CMCommand cmd;

            foreach (RuleDef ruleDef in rules)
            {
                // generating a validating query based on the rule definition
                query = dataView.GetRuleValidatingQuery(ruleDef);
                cmd = connection.CreateCommand();
                cmd.CommandText = query;

                doc = cmd.ExecuteXMLDoc();
                if (doc.DocumentElement.InnerText != null)
                {
                    message = doc.DocumentElement.InnerText;
                }

                RuleValidateResult result = new RuleValidateResult(message);
                if (result.HasError)
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }
    }
}

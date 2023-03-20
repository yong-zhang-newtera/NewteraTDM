using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Xml;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;

using HtmlAgilityPack;
using Swashbuckle.Swagger.Annotations;

using Ebaas.WebApi.Infrastructure;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Rules;
using Newtera.Data;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.Wrapper;
using Newtera.Server.Engine.Workflow;
using Newtera.WebForm;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// The Forms Service is a model-driven web service. It supports creating dynamic forms using data model and templates.
    /// It allows you to offer various forms in your business application without coding.
    /// </summary>
    [RoutePrefix("api/form")]
    public class FormController : ApiController
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string TEMPLATE_SOURCE = "templateSource";
        private const string READ_ONLY = "readOnly";
        private const string FILE_SOURCE = "file";
        private const string PROPERTY_SOURCE = "property";
        private const string TEMPLATE_NAME = "template";
        private const string TEMPLATE_PROPERTY = "property";
        private const string TASK_ID = "taskId";
        private const string DEFAULT_FORM = "default.htm";

        /// <summary>
        /// Get a form template of a class for creating a new instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="templateSource">Indicating the source of a template with options of file or property. Default to null</param>
        /// <param name="template">When templateSource is file, providing the name of a form template file</param>
        /// <param name="readOnly">true to get a read-onlu form template, false otherwise. Deafult to false</param>
        /// <remarks>If templateSource parameter is null, it returns an auto-generated form template based on meta-data; This api doesn't support it if templateSource is property</remarks>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("template/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "A form template in html")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetTemplateForNewInstance(string schemaName, string className, string templateSource = null, string template = null,  string readOnly = null)
        {
            try
            {
                string templateStr = "";

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string templateSrc = GetParamValue(parameters, TEMPLATE_SOURCE, null);
                bool isReadOnly = bool.Parse(GetParamValue(parameters, READ_ONLY, false));

                if (!string.IsNullOrEmpty(templateSrc))
                {
                    if (templateSrc == FILE_SOURCE)
                    {
                        string templateName = GetParamValue(parameters, TEMPLATE_NAME, null);

                        if (!string.IsNullOrEmpty(templateName))
                        {
                            templateStr = GetCustomFormFromFile(schemaName, className, null, templateName, isReadOnly, null);
                        }
                        else
                        {
                            throw new Exception("Missing parameter " + TEMPLATE_NAME + " in template/{schemaName}/{className} api call.");
                        }
                    }
                    else if (templateSrc == PROPERTY_SOURCE)
                    {
                        throw new Exception(PROPERTY_SOURCE + " source is not supported by this api");
                    }
                    else
                    {
                        throw new Exception("Unknown template source type " + templateSrc);
                    }
                }
                else
                {
                    string defaultFormTemplate = GetCustomFormFromFile(schemaName, className, null, DEFAULT_FORM, isReadOnly, null);
                    if (!String.IsNullOrEmpty(defaultFormTemplate))
                    {
                        templateStr = defaultFormTemplate;
                    }
                    else
                    {
                        // return the auto-generated form template
                        IHttpActionResult result = await GetAutoFormTemplate(schemaName, className, null, isReadOnly, null);

                        return result;
                    }
                }

                return Ok(templateStr);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get a form template of a class for updating or viewing an existing data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="templateSource">Indicating the source of a template with options of file or property. Default to null</param>
        /// <param name="oid">The obj_id of an data instance such as 37728282</param>
        /// <param name="template">When templateSource is file, providing the name of a form template file</param>
        /// <param name="property">When templateSource is property, providing the name of a property that stores the name of a form template file</param>
        /// <param name="readOnly">true to get a read-onlu form template, false otherwise. Deafult to false</param>
        /// <param name="taskId">the id of a workflow task that contains settings for the form, such as which properties are readonly for the task </param>
        /// <remarks>If templateSource parameter is null, it returns an auto-generated form template based on meta-data.</remarks>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("template/{schemaName}/{className}/{oid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "A form template in html")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetTemplateForExistingInstance(string schemaName, string className, string oid, string templateSource = null, string template = null, string property= null, string readOnly = null, string taskId = null)
        {
            try
            {
                string templateStr = "";
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string templateSrc = GetParamValue(parameters, TEMPLATE_SOURCE, null);
                bool isReadOnly = bool.Parse(GetParamValue(parameters, READ_ONLY, false));

                string taskIdStr = GetParamValue(parameters, TASK_ID, null);

                if (!string.IsNullOrEmpty(templateSrc))
                {
                    if (templateSrc == FILE_SOURCE)
                    {
                        string templateName = GetParamValue(parameters, TEMPLATE_NAME, null);
                        if (!string.IsNullOrEmpty(templateName))
                        {
                            templateStr = GetCustomFormFromFile(schemaName, className, oid, templateName, isReadOnly, taskIdStr);
                        }
                        else
                        {
                            throw new Exception("Missing parameter " + TEMPLATE_NAME + " in template/{schemaName}/{className}/{oid} api call.");
                        }
                    }
                    else if (templateSrc == PROPERTY_SOURCE)
                    {
                        string propertyName = GetParamValue(parameters, TEMPLATE_PROPERTY, null);

                        if (!string.IsNullOrEmpty(propertyName))
                        {
                            templateStr = GetCustomFormFromProperty(schemaName, className, oid, propertyName, isReadOnly, taskIdStr);
                        }
                        else
                        {
                            throw new Exception("Missing parameter " + TEMPLATE_PROPERTY + " in template/{schemaName}/{className}/{oid} api call.");
                        }
                    }
                    else
                    {
                        throw new Exception("Unknown template source type " + templateSrc);
                    }
                }
                else
                {
                    string defaultFormTemplate = GetCustomFormFromFile(schemaName, className, oid, DEFAULT_FORM, isReadOnly, taskIdStr);
                    if (!String.IsNullOrEmpty(defaultFormTemplate))
                    {
                        templateStr = defaultFormTemplate;
                    }
                    else
                    {
                        // return the auto-generated form template
                        IHttpActionResult result = await GetAutoFormTemplate(schemaName, className, oid, isReadOnly, taskIdStr);

                        return result;
                    }
                }

                return Ok(templateStr);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Validate data in a form with the server-side validating rules
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="instance">A json object representing a form data</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("validate/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string[]), Description = "empty string if the form data is valid or a message if it is invalid")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ValidateFormData(string schemaName, string className, dynamic instance)
        {
            QueryHelper queryHelper = new QueryHelper();
            string msg = "";
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);
                        InstanceView instanceView = new InstanceView(dataView);

                        InstanceEditor instanceEditor = new InstanceEditor();
                        instanceEditor.EditInstance = instanceView;
                        instanceEditor.ConvertToModel(instance); // // translate the JSON instance data to InstanceView instance data

                        // validate the data instance using validating rules if any.
                        ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                        RuleCollection validatingRules = con.MetaDataModel.RuleManager.GetPrioritizedRules(classElement).Rules;
                        if (validatingRules != null && validatingRules.Count > 0)
                        {
                            if (ValidateUsingRules(con, instanceView.DataView, validatingRules, out msg))
                            {
                                msg = "";
                            }
                        }
                    }
                });

                return Ok(msg);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the names of the form templates defined for a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("layouts/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string[]), Description = "A array of form template names")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetFormTemplateNames(string schemaName, string className)
        {
            string[] formNames = null;
            QueryHelper queryHelper = new QueryHelper();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        string formDir = NewteraNameSpace.GetFormTemplateDir(con.MetaDataModel.SchemaInfo.NameAndVersion, className);

                        if (Directory.Exists(formDir))
                        {
                            string[] files = Directory.GetFiles(formDir);
                            if (files.Length > 0)
                            {
                                formNames = new string[files.Length];
                                int pos;
                                FileInfo fileInfo;
                                int index = 0;
                                foreach (string file in files)
                                {
                                    fileInfo = new FileInfo(file);
                                    pos = fileInfo.Name.LastIndexOf('.');
                                    if (pos > 0)
                                    {
                                        // remove the file extension
                                        formNames[index++] = fileInfo.Name.Substring(0, pos);
                                    }
                                    else
                                    {
                                        formNames[index++] = fileInfo.Name;
                                    }
                                }
                            }
                            else
                            {
                                formNames = new string[0];
                            }
                        }
                    }
                });

                return Ok(formNames);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets a html string of a form given a form name
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="formName">the name of the form without extension such as MyForm</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("layout/{schemaName}/{className}/{formName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "A html string of a form")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetFormLayout(string schemaName, string className, string formName)
        {
            string layoutContent = "";
            QueryHelper queryHelper = new QueryHelper();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        string formDir = NewteraNameSpace.GetFormTemplateDir(con.MetaDataModel.SchemaInfo.NameAndVersion, className);

                        if (Directory.Exists(formDir))
                        {
                            string formFilePath = formDir + formName + ".htm";
                            if (File.Exists(formFilePath))
                            {
                                HtmlDocument doc = new HtmlDocument();
                                doc.Load(formFilePath);
                                layoutContent = doc.DocumentNode.OuterHtml;
                            }
                        }
                    }
                });

                return Ok(layoutContent);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Save a html string as a form template file
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="formName">the name of the form without extension such as MyForm</param>
        /// <param name="content">The html string representing a form</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("layout/{schemaName}/{className}/{formName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "A html string saved as a file")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> SetFormLayout(string schemaName, string className, string formName, [FromBody]string content)
        {
            QueryHelper queryHelper = new QueryHelper();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        string formDir = NewteraNameSpace.GetFormTemplateDir(con.MetaDataModel.SchemaInfo.NameAndVersion, className);

                        string formFilePath = formDir + formName + ".htm";
                        if (File.Exists(formFilePath))
                        {
                            File.Delete(formFilePath);
                        }

                        if (!Directory.Exists(formDir))
                        {
                            Directory.CreateDirectory(formDir);
                        }

                        using (StreamWriter sw = new StreamWriter(formFilePath, false, System.Text.Encoding.UTF8))
                        {
                            string templateHtml = content;
                            sw.Write(templateHtml);
                        }

                        // clear enum types so that new form name will appear in the dropdown list
                        Newtera.Common.MetaData.EnumTypeFactory.Instance.ClearEnumTypes();
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
        /// Get the default form template of a class for an existing instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="readOnly">true to get a readonly form, false otherwise</param>
        /// <param name="taskId">A workflow task id</param>
        /// <returns>A template in html</returns>
        private async Task<IHttpActionResult> GetAutoFormTemplate(string schemaName, string className, string oid, bool readOnly, string taskId)
        {
            try
            {
                string template = "";
                QueryHelper queryHelper = new QueryHelper();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

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
                        }

                        InstanceEditor instanceEditor = new InstanceEditor();
                        instanceEditor.EditInstance = instanceView;
                        instanceEditor.IsViewOnly = readOnly;
                        instanceEditor.ConnectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);

                        template = instanceEditor.CreateFormTemplate(taskId);

                        //ErrorLog.Instance.WriteLine(template);
                    }
                });

                return Ok(template);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get a cusotm form template of a given name for updating an instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="templateName">the custom template name</param>
        /// <param name="readOnly">true to get a readonly form, false otherwise</param>
        /// <param name="taskId">a workflow task id, default to null</param>
        /// <returns>A custom template in html</returns>
        private string GetCustomFormFromFile(string schemaName, string className, string oid, string templateName, bool readOnly, string taskId)
        {
            try
            {
                string template = "";
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

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
                    }

                    CustomFormEditor formEditor = new CustomFormEditor();
                    formEditor.EditInstance = instanceView;
                    formEditor.IsViewOnly = readOnly;
                    formEditor.ConnectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
                    SetTemplateToEditor(con, formEditor, className, templateName);

                    template = formEditor.CreateFormTemplate(taskId);

                    //ErrorLog.Instance.WriteLine(template);
                }

                return template;
            }
            catch (Exception ex)
            {
                if (templateName != DEFAULT_FORM)
                {
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }

                return null;
            }
        }


        /// <summary>
        /// Get a cusotm form template from a property of an instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <param name="propertyName">the property name</param>
        /// <param name="readOnly">true to get a readonly form, false otherwise</param>
        /// <param name="taskId">A workflow task id</param>
        /// <returns>A custom template in html</returns>
        private string GetCustomFormFromProperty(string schemaName, string className, string oid, string propertyName, bool readOnly, string taskId)
        {
            try
            {
                string template = "";
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    InstanceView instanceView;

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
                        throw new Exception("Failed to get an instance with id " + oid + " in class " + className);
                    }

                    CustomFormEditor formEditor = new CustomFormEditor();
                    formEditor.EditInstance = instanceView;
                    formEditor.IsViewOnly = readOnly;
                    formEditor.ConnectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);

                    string templateName = GetTemplateName(instanceView, propertyName);
                    SetTemplateToEditor(con, formEditor, className, templateName);

                    template = formEditor.CreateFormTemplate(taskId);

                    //ErrorLog.Instance.WriteLine(template);
                }

                return template;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        /// <summary>
        /// Get the primary key of an instance
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="oid">the instance key</param>
        /// <returns>A primary key string</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("primarykey/{schemaName}/{className}/{oid}")]
        public string GetPrimaryKey(string schemaName, string className, string oid)
        {
            try
            {
                string pkValues = "";
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    if (dataView != null)
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
                            InstanceView instanceView = new InstanceView(dataView, ds);
                            pkValues = instanceView.InstanceData.PrimaryKeyValues;
                        }
                    }
                }

                return pkValues;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        /// <summary>
        /// Get the options of a property of a class
        /// </summary>
        /// <param name="schemaName">the schema name</param>
        /// <param name="className">the class name</param>
        /// <param name="property">the property of a list</param>
        /// <param name="filterValue">the value that filters the list options</param>
        /// <returns>A collection of option objects, each object has a name aand value</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("listoptions/{schemaName}/{className}/{property}/{filterValue}")]
        public IEnumerable<JObject> GetListOptions(string schemaName, string className, string property, string filterValue)
        {
            try
            {
                List<JObject> options = new List<JObject>();
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    con.Open();

                    DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                    if (dataView != null)
                    {
                        InstanceView instanceView = new InstanceView(dataView);

                        InstanceAttributePropertyDescriptor pd = instanceView.GetProperties(null)[property] as InstanceAttributePropertyDescriptor;
                        if (pd != null)
                        {
                            pd.SetListFilterValue(filterValue);

                            Type enumType = pd.PropertyType; // return the filtered enum values for this session

                            Array enumValues = Enum.GetValues(enumType);  // .NET's tool for giving us all of an enum type's values

                            JObject option;
                            for (int i = 0; i < enumValues.Length; i++)
                            {
                                int value = Convert.ToInt32(enumValues.GetValue(i));
                                string name = Enum.GetName(enumType, value);

                                option = new JObject();
                                if (i == 0)
                                {
                                    option.Add("value", ""); // make unknown as empty value
                                }
                                else
                                {
                                    option.Add("value", value);
                                }
                                option.Add("name", name);

                                options.Add(option);
                            }  // for

                        }
                    }
                }

                return options;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
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

        private string GetTemplateName(InstanceView instanceView, string propertyName)
        {
            string templateName = null;

            // set the form template
            if (!string.IsNullOrEmpty(propertyName))
            {
                templateName = instanceView.InstanceData.GetAttributeStringValue(propertyName);
            }

            if (templateName == null)
            {
                throw new Exception("Unable to find a form template name stored in the property " + propertyName);
            }

            return templateName;
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
    }
}

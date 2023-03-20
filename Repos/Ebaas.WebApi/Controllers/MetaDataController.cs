using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Dynamic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Specialized;

using Swashbuckle.Swagger.Annotations;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Data;
using Ebaas.WebApi.Infrastructure;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// The Metadata Service has a set of operations of getting metadata in JSON formats. 
    /// It allows you to develop metadata-driven user interface components that are adaptive and customizable.
    /// </summary>
    public class MetaDataController : ApiController
    {
        private string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string DATA_VIEW = "view";
        private const string FULL_VIEW = "full";
        private const string SIMPLE_VIEW = "simple";

        /// <summary>
        /// Gets infos of the database schemas
        /// </summary>
        [HttpGet]
        [Route("api/metadata/schemas")]
        [NormalAuthorizeAttribute]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SchemaInfo[]), Description = "A collection of available database schema objects")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetSchemas()
        {
            try
            {
                SchemaInfo[] schemas = null;

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection())
                    {
                        schemas = con.AllSchemas;
                    }
                });

                return Ok(schemas);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets the basic info of a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        [HttpGet]
        [Route("api/metadata/class/{schemaName}/{className}")]
        [NormalAuthorizeAttribute]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object for a class info")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetClassInfo(string schemaName, string className)
        {
            try
            {
                JObject info = new JObject();

                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
                        
                        if (classElement != null)
                        {
                            info.Add("name", classElement.Name);
                            info.Add("title", classElement.Caption);
                        }
                        else
                        {
                            throw new Exception("Unable to find a class with name " + className + " in schema " + schemaName);
                        }
                    }
                });

                return Ok(info);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get a string in JSON Schema format representing the metadata of a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="view">Return a json schema using the data view such as simple, full, or a specific name. Default to simple.</param>
        [HttpGet]
        [Route("api/metadata/view/{schemaName}/{className}")]
        [NormalAuthorizeAttribute]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json schema object as class meta-data")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetSimpleJSonScehma(string schemaName, string className, string view=null)
        {
            try
            {
                JObject jsonSchema = new JObject();
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                        DataViewModel dataView;
                        string viewName = GetParamValue(parameters, DATA_VIEW, SIMPLE_VIEW);
                        if (viewName == SIMPLE_VIEW)
                        {
                            dataView = con.MetaDataModel.GetDefaultDataView(className);
                            dataView.Caption = dataView.BaseClass.Caption;
                        }
                        else if (viewName == FULL_VIEW)
                        {
                            dataView = con.MetaDataModel.GetDetailedDataView(className);
                            dataView.Caption = dataView.BaseClass.Caption;
                        }
                        else
                        {
                            dataView = con.MetaDataModel.DataViews[viewName] as DataViewModel;
                        }

                        if (dataView != null)
                        {

                            JsonSchema jschema = BuildJSonScehma(dataView);

                            string schemaStr = jschema.ToString();

                            jsonSchema = JObject.Parse(schemaStr);
                        }
                    }
                });

                return Ok(jsonSchema);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get infos of the bottom classes of a data class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as TestItem</param>
        [HttpGet]
        [Route("api/metadata/leafclasses/{schemaName}/{className}")]
        [NormalAuthorizeAttribute]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JArray), Description = "An array of json object for bottom class infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetLeafClasses(string schemaName, string className)
        {
            JArray leafClasses = new JArray();
            JObject leafClass = null;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        SchemaModelElementCollection leafClassElements = con.MetaDataModel.GetBottomClasses(className);

                        foreach (SchemaModelElement leafClassElement in leafClassElements)
                        {
                            leafClass = new JObject();

                            leafClass.Add("name", leafClassElement.Name);
                            leafClass.Add("title", leafClassElement.Caption);

                            leafClasses.Add(leafClass);
                        }
                    }
                });

                return Ok(leafClasses);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets a tree representing the roots classes of a schema
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <returns>A Tree of json object with a tree root</returns>
        [HttpGet]
        [Route("api/metadata/schematree/{schemaName}")]
        [NormalAuthorizeAttribute]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JArray), Description = "An array of hierarchical json object representing root classes ")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetClassTree(string schemaName)
        {
            JArray treeRoots = null;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        treeRoots = TreeNodeBuilder.GetClassTree(con.MetaDataModel);
                    }
                });

                return Ok(treeRoots);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get a tree in JSON representing the relationships from a class to its related classes
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A name of the base data class such as TestItem</param>
        /// <returns>A Tree of json object with a single tree root</returns>
        [HttpGet]
        [Route("api/metadata/relationtree/{schemaName}/{className}")]
        [NormalAuthorizeAttribute]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A hierarchical json object representing relationships of a base class to its related classes")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetRelationshipTree(string schemaName, string className)
        {
            JObject treeRoot = null;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        treeRoot = TreeNodeBuilder.GetRelationshipTree(con.MetaDataModel, className, 1);
                    }
                });

                return Ok(treeRoot);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }


        /// <summary>
        /// Gets a tree in JSON representing a taxonomy defined in a schema
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="treeName">A name of the taxonomy defined in the schema such as ProductCatalog</param>
        /// <returns>A Tree of json object with a tree root</returns>
        [HttpGet]
        [Route("api/metadata/classificationtree/{schemaName}/{treeName}")]
        [NormalAuthorizeAttribute]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A hierarchical json object representing a taxonomy")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetTaxonomyTree(string schemaName, string treeName)
        {
            JObject treeRoot = null;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
                    {
                        con.Open();

                        TaxonomyModel taxonomy = (TaxonomyModel)con.MetaDataModel.Taxonomies[treeName];
                        treeRoot = TreeNodeBuilder.GetTaxonomyTree(con.MetaDataModel, taxonomy);
                    }
                });

                return Ok(treeRoot);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Build a JSON Schema object based on a DataViewModel object
        /// </summary>
        /// <param name="dataView">A DataViewModel object</param>
        /// <returns>A JObject representing a JSON schema</returns>
        private JsonSchema BuildJSonScehma(DataViewModel dataView)
        {
            JsonSchema jschema = new JsonSchema();
            jschema.Type = JsonSchemaType.Object;
            jschema.Id = dataView.BaseClass.ClassName;
            jschema.Title = dataView.Caption;

            // add schema properties
            jschema.Properties = new Dictionary<string, JsonSchema>();
            InstanceView instanceView = new InstanceView(dataView);
            JsonSchema propertySchema;
            System.Collections.Hashtable cascadedProperties = new System.Collections.Hashtable();
            foreach (InstanceAttributePropertyDescriptor pd in instanceView.GetProperties(null))
            {
                if (!pd.IsBrowsable)
                    continue;

                propertySchema = new JsonSchema();
                propertySchema.Type = GetPropertyType(pd);
                propertySchema.Title = pd.DisplayName;
                
                if (pd.IsReadOnly || pd.IsVirtual || pd.IsRelationship)
                {
                    propertySchema.ReadOnly = true; // do not allow filter row
                }

                propertySchema.Required = pd.IsRequired;
                if (propertySchema.Type == JsonSchemaType.String)
                {
                    propertySchema.MaximumLength = pd.MaxLength;
                    propertySchema.MinimumLength = pd.MinLength;
                }

                if (pd.ShowAsProgressBar)
                {
                    propertySchema.Format = "progress";
                }

                if (pd.IsImage)
                {
                    propertySchema.Format = "image";
                    propertySchema.MinimumLength = pd.ImageThumbnailWidth; // use minLength for thumnail width of image property
                    propertySchema.MaximumLength = pd.ImageThumbnailHeight; // // use maxLength for thumnail width of image property
                }

                if (pd.DataType == DataType.Date)
                {
                    propertySchema.Format = "date";
                }
                else if (pd.DataType == DataType.DateTime)
                {
                    propertySchema.Format = "datetime";
                }
                else if (pd.DataType == DataType.Boolean)
                {
                    // add enum values to the property
                    propertySchema.Enum = new List<JToken>();

                    Type pEnumType = pd.PropertyType;
                    Array vEnumValues = Enum.GetValues(pEnumType);  // .NET's tool for giving us all of an enum type's values
                    string enumName;
                    for (int index = 0; index < vEnumValues.Length; index++)
                    {
                        int vValue = Convert.ToInt32(vEnumValues.GetValue(index));
                        enumName = Enum.GetName(pEnumType, vValue);

                        propertySchema.Enum.Add(enumName);
                    }  // for
                }
                else if (pd.IsMultipleLined)
                {
                    propertySchema.Format = "text";
                }

                if (pd.HasConstraint)
                {
                    if (pd.Constraint is EnumElement ||
                        pd.Constraint is ListElement)
                    {
                        // TODO, handle the cascaded properties
                        if (cascadedProperties[pd.Name] == null)
                        {
                            // add enum values to the property
                            propertySchema.Enum = new List<JToken>();

                            Type pEnumType = pd.PropertyType;
                            Array vEnumValues = Enum.GetValues(pEnumType);  // .NET's tool for giving us all of an enum type's values
                            string enumName;
                            for (int index = 0; index < vEnumValues.Length; index++)
                            {
                                int vValue = Convert.ToInt32(vEnumValues.GetValue(index));
                                enumName = Enum.GetName(pEnumType, vValue);

                                propertySchema.Enum.Add(enumName);
                              
                            }  // for
                        }

                        string cascadedPropertyNames = pd.CascadedPropertyNames;
                        if (!string.IsNullOrEmpty(pd.CascadedPropertyNames))
                        {
                            string[] propertieStrings = pd.CascadedPropertyNames.Split(';');
                            JArray properties = new JArray();
                            foreach (string propertyString in propertieStrings)
                            {
                                properties.Add(propertyString);
                                cascadedProperties[propertyString] = propertyString;
                            }
                            //propertySchema.Default = properties;
                        }

                        if (pd.Constraint is EnumElement &&
                            ((EnumElement)pd.Constraint).DisplayMode == EnumDisplayMode.Image)
                        {
                            propertySchema.Format = "icon";
                        }
                    }
                    else if (pd.Constraint is PatternElement)
                    {
                        propertySchema.Pattern = ((PatternElement)pd.Constraint).PatternValue;
                    }
                }

                if (pd.IsHidden)
                {
                    propertySchema.Hidden = true;
                }

                // use description to keep attribute's type
                if (pd.IsRelationship)
                {
                    propertySchema.Description = "R";
                }
                else if (pd.IsVirtual)
                {
                    propertySchema.Description = "V";
                }
                else if (pd.IsArray)
                {
                    propertySchema.Description = "A";
                }
                else if (pd.IsImage)
                {
                    propertySchema.Description = "I";
                }
                else
                {
                    // simple attribute
                    propertySchema.Description = "";
                }

                jschema.Properties.Add(pd.Name, propertySchema);
            }

            return jschema;
        }

        private JsonSchemaType GetPropertyType(InstanceAttributePropertyDescriptor pd)
        {
            JsonSchemaType jType = JsonSchemaType.String;

            switch (pd.DataType)
            {
                case DataType.BigInteger:
                case DataType.Integer:
                case DataType.Byte:

                    if (pd.IsMultipleChoice)
                    {
                        jType = JsonSchemaType.String; // Multiple choice property treats as string
                    }
                    else
                    {
                        jType = JsonSchemaType.Integer;
                    }
                    break;

                case DataType.Float:
                case DataType.Double:
                case DataType.Decimal:
                    jType = JsonSchemaType.Float;
                    break;

                case DataType.Boolean:
                    jType = JsonSchemaType.Boolean;
                    break;

                case DataType.Date:
                case DataType.DateTime:
                    jType = JsonSchemaType.String;
                    break;

                case DataType.String:
                    jType = JsonSchemaType.String;
                    break;

                case DataType.Text:
                    jType = JsonSchemaType.Object;
                    break;

                default:
                    jType = JsonSchemaType.String;
                    break;
            }

            return jType;
        }

        private string GetPropertyEnum(InstanceAttributePropertyDescriptor pd)
        {
            return "";
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
    }
}

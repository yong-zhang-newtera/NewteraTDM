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
using Newtera.Data;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.MLServer.TimeSeries;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// The Time Series Service is a particular web service that has a set of operations of reading time series data
    /// from files, forecasting time series based on a machine learning model, and publishing predictive machine learning models.
    /// </summary>
    public class TimeSeriesController : ApiController
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";
        private const string CATEGORY = "category";
        private const string FREQUENCY = "frequency";
        private const string OPERATION = "operation";
        private const string RELOAD = "reload";
        private const string FIELD = "field";
        private const string MAX_FORECAST = "maxforecast";

        /// <summary>
        /// Get a time series metric which consists of a timestamp and multiple fields
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="xmlSchemaName">The name of an xml schema that defines a program of the extraction</param>
        /// <param name="category">Return the time series of the category, return a complete time series if null</param>
        /// <param name="fieldName">The name of a series to be returned, if null, return all time series</param>
        /// <param name="frequency">Time sereis resampleing frequencysuch as, Second, Minute, Hour, Day, and Month, default is null for returning all points</param>
        /// <param name="operation">Tiime series resampleing operation, such as Min, Max, Mean, Median, FirstValue, LastValue of resampling range</param>
        /// <param name="from">Return the data starting from the row index such as 0. Default to -1 for all data.</param>
        /// <param name="size">Return the data with a page size such as 30. Default to 50.</param>
        /// <param name="reload">Indicate whether to reload the cache, true to reload, default is false</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/metric/{schemaName}/{className}/{oid}/{xmlSchemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A time series metric which consists of a timestamp and multiple fields or an empty array if there is no timeseries available.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured.")]
        public async Task<IHttpActionResult> GetTimeSeries(string schemaName, string className, string oid, string xmlSchemaName, string category = null, string fieldName = null, string frequency = null, string operation = null, int? from = null, int? size = null, string reload = null)
        {
            try
            {
                QueryHelper queryHelper = new QueryHelper();
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                TimeSeriesFrequency frequencyType = GetTimeSeriesFrequency(GetParamValue(parameters, FREQUENCY, null)); // time series frequency
                TimeSeriesOperation opType = GetTimeSeriesOperation(GetParamValue(parameters, OPERATION, null));
                int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 50)); // page size,default to 50
                int startRow = int.Parse(GetParamValue(parameters, START_ROW, -1)); // startRow row, -1 for all data
                string field = GetParamValue(parameters, FIELD, null);
                string categoryName = GetParamValue(parameters, CATEGORY, null);
                bool reloadFlag = bool.Parse(GetParamValue(parameters, RELOAD, "false"));

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

                        DataTable timeSeries = await cmd.GetTimeSeries(xmlSchemaModel, dataView, categoryName, field, frequencyType, opType, startRow, pageSize, reloadFlag);

                        if (timeSeries != null)
                        {
                            // convert to string to keep the case of property names
                            string jsonString = JsonConvert.SerializeObject(timeSeries);
                            //ErrorLog.Instance.WriteLine(jsonString);
                            return Ok(jsonString);
                        }
                        else
                        {
                            return Ok("[]");
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find a xmls schema " + xmlSchemaName + " for the class " + className);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the count of data points in a time series metric.
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="xmlSchemaName">The name of an xml schema that defines a program of the extraction</param>
        /// <param name="category">return count of a time series of the category, return count of a complete time series if null</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/metric/count/{schemaName}/{className}/{oid}/{xmlSchemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "data point count of a time series metric")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured.")]
        public async Task<IHttpActionResult> GetTimeSeriesCount(string schemaName, string className, string oid, string xmlSchemaName, string category = null)
        {
            try
            {
                QueryHelper queryHelper = new QueryHelper();
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string categoryName = GetParamValue(parameters, CATEGORY, null);

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

                        int count = await cmd.GetTimeSeriesPointCount(xmlSchemaModel, dataView, categoryName);

                        return Ok(count);
                    }
                    else
                    {
                        throw new Exception("Unable to find a xmls schema " + xmlSchemaName + " for the class " + className);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the categories in a time series metric grouped by a category field defined for the matrix
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="xmlSchemaName">The name of an xml schema that defines a program of the extraction</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/metric/categories/{schemaName}/{className}/{oid}/{xmlSchemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "A list of categories of the time series")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetTimeSeriesCategories(string schemaName, string className, string oid, string xmlSchemaName)
        {
            try
            {
                QueryHelper queryHelper = new QueryHelper();

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

                        IList<string> categories = await cmd.GetTimeSeriesCategories(xmlSchemaModel, dataView);

                        if (categories != null)
                            return Ok(categories);
                        else
                        {
                            return Ok();
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find a xmls schema " + xmlSchemaName + " for the class " + className);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the fields in a time series metric
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="xmlSchemaName">The name of an xml schema that defines a program of the extraction</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/metric/fields/{schemaName}/{className}/{xmlSchemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Object>), Description = "Fields in a time series metric including the timestamp")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public HttpResponseMessage GetTimeSeriesFields(string schemaName, string className, string xmlSchemaName)
        {
            try
            {
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection connection = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    connection.Open();

                    JArray columnInfos = new JArray();

                    XMLSchemaModel xmlSchemaModel = connection.MetaDataModel.XMLSchemaViews[xmlSchemaName] as XMLSchemaModel;

                    // make sure the xmlSchemaModel is defined for the given base class
                    if (xmlSchemaModel != null &&
                        xmlSchemaModel.RootElement.ElementType == className)
                    {
                        List<XMLSchemaElement> simpleElements = xmlSchemaModel.GetSimpleElements();
                        XMLSchemaElement xAxisElement = xmlSchemaModel.GetXAxisElement();
                        string xAxisName = null;
                        if (xAxisElement != null)
                        {
                            xAxisName = xAxisElement.Name;
                        }

                        foreach (XMLSchemaElement simpleElement in simpleElements)
                        {
                            JObject columnInfo = new JObject();
                            columnInfo["title"] = simpleElement.Caption;
                            columnInfo["name"] = simpleElement.Name;
                            columnInfo["type"] = simpleElement.ElementType;
                            if (xAxisName != null && simpleElement.Name == xAxisName)
                            {
                                columnInfo["xaxis"] = true;
                            }
                            else
                            {
                                columnInfo["xaxis"] = false;
                            }
                            columnInfos.Add(columnInfo);
                        }

                        var resp = new HttpResponseMessage(HttpStatusCode.OK);
                        string str = JsonConvert.SerializeObject(columnInfos);
                        resp.Content = new StringContent(str, System.Text.Encoding.UTF8, "text/plain");
                        return resp;
                    }
                    else
                    {
                        throw new Exception("Unable to find an xml schema with name " + schemaName);
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
        /// Download the data of a time series frame as a text file
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A master data class name such as CLTestRequest</param>
        /// <param name="oid">The obj_id of a data instance in the master class such as 2778388292</param>
        /// <param name="xmlSchemaName">The name of an xml schema that defines a program of the extraction</param>
        /// <param name="category">download a file of the time series of the category, download a file of the complete time series if null</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/metric/file/{schemaName}/{className}/{oid}/{xmlSchemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "File downloaded")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetFile(string schemaName, string className, string oid, string xmlSchemaName, string category)
        {
            QueryHelper queryHelper = new QueryHelper();
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string categoryName = GetParamValue(parameters, CATEGORY, null);

            LocalFileManager fileManager = new LocalFileManager();

            var result = await fileManager.GetFile(schemaName, className, oid, TimeSeriesGenerator.TIME_SERIES_FILE, null);

            return result;
        }

        /// <summary>
        /// Forecast a time series for a field using a machine learning model
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as BateryMeasurements</param>
        /// <param name="modelId">The unique id of a time series forecast model</param>
        /// <param name="timeSeries">The time series collection</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/field/forecast/{schemaName}/{className}/{modelId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "The forecasted time series of a field")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> Forecast(string schemaName, string className, string modelId, dynamic timeSeries)
        {
            try
            {         
                QueryHelper queryHelper = new QueryHelper();

                using (CMConnection connection = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
                {
                    connection.Open();

                    TimeSeriesForecaster forecaster = new TimeSeriesForecaster();

                    ModelInfo modelInfo = MLModelManager.Instance.GetModelInfo(schemaName, className, modelId);
                    if (modelInfo != null)
                    {
                        string modelFileDir = MLModelManager.Instance.GetModelFileDir(schemaName, className, modelId);
                        string preprocess = null;
                        if (!string.IsNullOrEmpty(modelInfo.Preprocess))
                        {
                            preprocess = modelFileDir + @"\" + modelInfo.Preprocess;
                        }
                        string postprocess = null;
                        if (!string.IsNullOrEmpty(modelInfo.Postprocess))
                        {
                            postprocess = modelFileDir + @"\" + modelInfo.Postprocess;
                        }

                        string fieldName = GetTimeSeriesName(connection, modelInfo); // get display name for the time series

                        JArray forecated = await forecaster.GetForecastTimeSeries(modelFileDir, fieldName, modelInfo.Frequency, preprocess, postprocess, timeSeries);

                        //var jstr = JsonConvert.SerializeObject(forecated, Newtonsoft.Json.Formatting.Indented);
                        //ErrorLog.Instance.WriteLine(jstr);

                        // convert to string to keep the case sensitivity of property names
                        var jsonString = JsonConvert.SerializeObject(forecated);
                        return Ok(jsonString);
                    }
                    else
                    {
                        throw new Exception("Unable to find a model with id " + modelId + " for the class " + className + " in the schema " + schemaName);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Publish a machine learning model for a field in a time sereis matrix.
        /// </summary>
        /// <param name="schemaName">Database schema name such as DEMO</param>
        /// <param name="className">The name of a data class that the model applies to</param>
        /// <param name="xmlSchemaName">The name of an xml schema that produces data for the model</param>
        /// <param name="fieldName">The name of a time series that the model applies to.</param>
        /// <param name="frequency">The frequency of time series that the model is forecating</param>
        /// <param name="maxForecast">The max forecast horizon, such as 1, 6, 12</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/field/model/{schemaName}/{className}/{xmlSchemaName}/{fieldName}/{frequency}/{maxForecast}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "Result of publishing a model")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PublishModel(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                var files = await MLModelManager.Instance.AddModel(Request, schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);
                return Ok(new { Message = "model published ok", files = files });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the predictive models created for a time series field that meets a criteria.
        /// </summary>
        /// <param name="schemaName">Database schema name such as DEMO</param>
        /// <param name="className">The name of a data class that the model applies to</param>
        /// <param name="xmlSchemaName">The name of an xml schema that produces data for the model</param>
        /// <param name="fieldName">The name of a time series that the model applies to.</param>
        /// <param name="frequency">The frequency of time series that the model is forecating</param>
        /// <param name="maxForecast">The max forecast horizon, such as 1, 6, 12</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/field/model/{schemaName}/{className}/{xmlSchemaName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "A model info object, null if the model desn't exist")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public HttpResponseMessage GetModelInfos(string schemaName, string className, string xmlSchemaName, string fieldName = null, string frequency = null, string maxForecast = null)
        {
            QueryHelper queryHelper = new QueryHelper();
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string field = GetParamValue(parameters, FIELD, null);
            string tsFrequency = GetParamValue(parameters, FREQUENCY, null); // time series frequency
            string maxForecastHorizon = GetParamValue(parameters, MAX_FORECAST, null);

            List<ModelInfo> modelInfos = MLModelManager.Instance.GetMatchedModelInfos(schemaName, className,
                xmlSchemaName, field, tsFrequency, maxForecastHorizon);

            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            string str = JsonConvert.SerializeObject(modelInfos.ToArray(), Newtonsoft.Json.Formatting.Indented);
           
            resp.Content = new StringContent(str, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        /// <summary>
        /// Publish a preprocessing program file for a predictive model created for a time series field.
        /// </summary>
        /// <param name="schemaName">Database schema name such as DEMO</param>
        /// <param name="className">The name of a data class that the model applies to</param>
        /// <param name="xmlSchemaName">The name of an xml schema that produces data for the model</param>
        /// <param name="fieldName">The name of a time series that the model applies to.</param>
        /// <param name="frequency">The frequency of time series that the model is forecating</param>
        /// <param name="maxForecast">The max forecast horizon, such as 1, 6, 12</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/field/preprocess/{schemaName}/{className}/{xmlSchemaName}/{fieldName}/{frequency}/{maxForecast}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "Result of posting a preprocess program")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PublishPreprocess(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                var files = await MLModelManager.Instance.AddPreprocess(Request, schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);
                return Ok(new { Message = "program published ok", files = files });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Publish a post-processing program file for a predictive model created for a time series field.
        /// </summary>
        /// <param name="schemaName">Database schema name such as DEMO</param>
        /// <param name="className">The name of a data class that the model applies to</param>
        /// <param name="xmlSchemaName">The name of an xml schema that produces data for the model</param>
        /// <param name="fieldName">The name of a time series that the model applies to.</param>
        /// <param name="frequency">The frequency of time series that the model is forecating</param>
        /// <param name="maxForecast">The max forecast horizon, such as 1, 6, 12</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/field/postprocess/{schemaName}/{className}/{xmlSchemaName}/{fieldName}/{frequency}/{maxForecast}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "Result of posting a postprocess program")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PublishPostprocess(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                var files = await MLModelManager.Instance.AddPostprocess(Request, schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);
                return Ok(new { Message = "program published ok", files = files });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Delete a predictive model and its associated programs created for a time series field
        /// </summary>
        /// <param name="schemaName">Database schema name such as DEMO</param>
        /// <param name="className">The name of a data class that the model applies to</param>
        /// <param name="xmlSchemaName">The name of an xml schema that produces data for the model</param>
        /// <param name="fieldName">The name of a time series that the model applies to.</param>
        /// <param name="frequency">The frequency of time series that the model is forecating</param>
        /// <param name="maxForecast">The max forecast horizon, such as 1, 6, 12</param>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/timeseries/field/model/{schemaName}/{className}/{xmlSchemaName}/{fieldName}/{frequency}/{maxForecast}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "Model deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(void), Description = "Model not found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DeleteModel(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            try
            {
                var result = await MLModelManager.Instance.DeleteModel(schemaName, className, xmlSchemaName, fieldName, frequency, maxForecast);

                if (result.Successful)
                {
                    return Ok(new { message = result.Message });
                }
                if (result.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

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

            if (parameters[key] != null)
            {
                val = parameters[key];
            }

            return val;
        }

        private string GetTimeSeriesName(CMConnection connection, ModelInfo modelInfo)
        {
            string fieldName = modelInfo.FieldName;

            XMLSchemaModel xmlSchemaModel = (XMLSchemaModel) connection.MetaDataModel.XMLSchemaViews[modelInfo.XMLSchemaName];
            if (xmlSchemaModel != null)
            {
                List<XMLSchemaElement> simpleElements = xmlSchemaModel.GetSimpleElements();
                
                foreach (XMLSchemaElement simpleElement in simpleElements)
                {
                    if (simpleElement.Name == fieldName)
                    {
                        fieldName = simpleElement.Caption;
                        break;
                    }
                }
            }

            return fieldName;
        }

        private TimeSeriesFrequency GetTimeSeriesFrequency(string frequencyStr)
        {
            TimeSeriesFrequency frequency = TimeSeriesFrequency.None;

            try
            {
                frequency = (TimeSeriesFrequency) Enum.Parse(typeof(TimeSeriesFrequency), frequencyStr);
            }
            catch (Exception)
            {
                frequency = TimeSeriesFrequency.None;
            }
            return frequency;
        }

        private TimeSeriesOperation GetTimeSeriesOperation(string opStr)
        {
            TimeSeriesOperation op = TimeSeriesOperation.None;

            try
            {
                op = (TimeSeriesOperation)Enum.Parse(typeof(TimeSeriesOperation), opStr);
            }
            catch (Exception)
            {
                op = TimeSeriesOperation.None;
            }
            return op;
        }
    }
}

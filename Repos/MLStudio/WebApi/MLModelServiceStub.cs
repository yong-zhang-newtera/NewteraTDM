using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Newtera.MLStudio.WebApi
{
    public class MLModelServiceStub : WebApiServiceBase
    {
        public MLModelServiceStub()
        {
        }

        /// <summary>
        /// Gets the version of the server
        /// </summary>
        /// <returns></returns>
        public string GetServerVersion()
        {
            return GetAPICall("api/adminService/GetServerVersion");
        }

        public JArray GetTimeSeriesModelInfos(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            string url = "api/timeseries/field/model/" + schemaName + "/" + className + "/" + xmlSchemaName;
            if (!string.IsNullOrEmpty(fieldName))
            {
                url += "?field=" + fieldName;
            }

            if (!string.IsNullOrEmpty(frequency))
            {
                url += "&frequency=" + frequency;
            }

            if (!string.IsNullOrEmpty(maxForecast))
            {
                url += "&maxforecast=" + maxForecast;
            }

            string json = GetAPICall(url);

            return JsonConvert.DeserializeObject<JArray>(json);
        }

        public void PublishTimeSeriesModel(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast, string modelFilePath)
        {
            PostFile("api/timeseries/field/model/" + schemaName + "/" + className + "/" + xmlSchemaName + "/" + fieldName + "/" + frequency + "/" + maxForecast, modelFilePath);
        }

        public void PublishPreprocessProgram(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast, string modelFilePath)
        {
            PostFile("api/timeseries/field/preprocess/" + schemaName + "/" + className + "/" + xmlSchemaName + "/" + fieldName + "/" + frequency + "/" + maxForecast, modelFilePath);
        }

        public void PublishPostprocessProgram(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast, string modelFilePath)
        {
            PostFile("api/timeseries/field/postprocess/" + schemaName + "/" + className + "/" + xmlSchemaName + "/" + fieldName + "/" + frequency + "/" + maxForecast, modelFilePath);
        }

        public void DeleteTimeSeriesModel(string schemaName, string className, string xmlSchemaName, string fieldName, string frequency, string maxForecast)
        {
            DeleteAPICall("api/timeseries/field/model/" + schemaName + "/" + className + "/" + xmlSchemaName + "/" + fieldName + "/" + frequency + "/" + maxForecast);
        }

        public string GetTimeSeriesFields(string schemaName, string className, string xmlSchemaName)
        {
            string result = GetAPICall("api/timeseries/metric/fields/" + schemaName + "/" + className + "/" + xmlSchemaName);
            
            return result;
        }
    }
}

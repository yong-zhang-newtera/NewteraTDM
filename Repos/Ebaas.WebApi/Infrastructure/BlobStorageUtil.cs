using System;
using System.Xml;
using System.Data;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Common.MetaData.DataView;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Net.Http.Headers;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Utility class for BlobStorageController
    /// </summary>
    public class BlobStorageUtil
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        public BlobStorageUtil()
        {
        }

        public string GetContainerName(string schemaName, string className, string oid, string prefix, string prefixProperty)
        {
            string containerName = prefix;
            if (string.IsNullOrEmpty(containerName))
            {
                // use the property value as the container name
                if (!string.IsNullOrEmpty(prefixProperty))
                {
                    containerName = GetContainerNameFromProperty(schemaName, className, oid, prefixProperty);
                }

                if (string.IsNullOrEmpty(containerName))
                {
                    containerName = className; // use class name as container name by default
                }
            }
            return containerName;
        }

        public async Task<HttpResponseMessage> CreateHttpResponse(string blobName, Stream stream)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(blobName, Encoding.UTF8)
            };

            return response;
        }

        private string GetContainerNameFromProperty(string schemaName, string className, string oid, string prefixProperty)
        {
            string containerName = null;

            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
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

                    containerName = instanceView.InstanceData.GetAttributeStringValue(prefixProperty);
                }
            }

            return containerName;
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}
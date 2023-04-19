using System;
using System.Xml;
using System.Data;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;

using Ebaas.WebApi.Models;

using Newtera.Data;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.Core;
using Newtera.BlobStorage;

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
            // HttpResponseMessage will dispose its Content when it is disposed, therefore, the file stream will be disposed
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Headers.Add("x-filename", HttpUtility.UrlEncode(blobName, Encoding.UTF8));
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(blobName, Encoding.UTF8)
            };

            return response;
        }

        public async Task<IEnumerable<MultipartFileData>> GetMultipartFileData(HttpRequestMessage request, string prefix)
        {
            // save the uploaded files in a temp dir
            var basePath = NewteraNameSpace.GetUserFilesDir();
            string baseDir = Path.Combine(basePath, prefix ?? string.Empty);
            if (!baseDir.EndsWith(@"\"))
            {
                baseDir += @"\";
            }

            if (!Directory.Exists(baseDir))
            {
                // create the directory
                Directory.CreateDirectory(baseDir);
            }

            var provider = new FileMultipartFormDataStreamProvider(baseDir);

            // create file infos in db and save files in a local disk
            await request.Content.ReadAsMultipartAsync(provider);

            return provider.FileData;
        }

        public IEnumerable<FileViewModel> ConvertToFileViewModels(IList<BlobDescriptor> blobDescriptors)
        {
            string creator = string.Empty;
            var fileViewModels = blobDescriptors
                .Where(x => !x.Name.ToUpper().EndsWith("-META.JSON"))
                .Select(x => new FileViewModel() {
                    ID = x.Name,
                    Name = x.Name,
                    Size = BytesToString(x.Length),
                    Modified = x.LastModified?.LocalDateTime ?? DateTime.Now,
                    Type = x.ContentType,
                    Creator = x.Metadata?.TryGetValue("creator", out creator) ?? false ? creator : "Unknown"
                })
                .ToList();
            
            return fileViewModels;
        }

        private String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
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
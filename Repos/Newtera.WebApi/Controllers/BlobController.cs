using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;

using Swashbuckle.Swagger.Annotations;

using Newtera.Common.Core;
using Newtera.BlobStorage;
using Newtera.WebApi.Models;
using Newtera.WebApi.Infrastructure;
using Newtera.Common.Config;
using Newtonsoft.Json;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The Blob Service stores a blob to varouse object storage storages based on the configuration,
    /// including local file directory, MinIO storage, or cloud storage. 
    /// It has set of operations for uploading, downloading, and deleting blob associated with a data instance.
    /// </summary>
    public class BlobController : ApiController
    {
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";
        private const string PREFIX = "prefix";
        private const string PREFIX_PROPERTY = "prefixProperty";

        public BlobController()
        {
        }

        /// <summary>
        /// Get files associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <remarks>Files associated with a data instance are stored in a specified directory by application</remarks>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/blob/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FileViewModel>), Description = "A collection of attachment infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetBlobs(string schemaName, string className, string oid)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 100));
                int start = int.Parse(GetParamValue(parameters, START_ROW, 0));
                
                string containerName = GetContainerName(parameters, schemaName, className, oid);

                IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(schemaName, className);

                var results = await storageProvider.ListBlobsAsync(containerName);

                BlobStorageUtil util = new BlobStorageUtil();
                var fileViewModels = util.ConvertToFileViewModels(results);
                return Ok(new { files = fileViewModels });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Upload files associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <remarks>The posted files are part of formdata</remarks>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/blob/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "Result of uploading files")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PostBlob(string schemaName, string className, string oid)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            IEnumerable<MultipartFileData> multipartFileData = null;
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string containerName = GetContainerName(parameters, schemaName, className, oid);

                string userName = GetParamValue(parameters, "user", null);
                IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(schemaName, className);

                BlobStorageUtil util = new BlobStorageUtil();
                multipartFileData = await util.GetMultipartFileData(Request, containerName);
                var blobProperties = new BlobProperties()
                {
                    Metadata = new Dictionary<string, string>() { { "creator", userName ?? "Unknown" } }
                };

                int index = 0;
                foreach (var fileData in multipartFileData)
                {
                    var filename = fileData.Headers?.ContentDisposition?.FileName ?? $"File-{index++}";
                    filename = filename.Trim(new char[] { '"' }).Replace("&", "and");
                    await storageProvider.SaveBlobStreamAsync(containerName,
                         filename,
                         File.OpenRead(fileData.LocalFileName),
                         blobProperties,
                         true);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
            finally
            {
                // cleanup temp files
                if (multipartFileData != null)
                {
                    foreach (var fileData in multipartFileData)
                    {
                        if (File.Exists(fileData.LocalFileName))
                        {
                            File.Delete(fileData.LocalFileName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete a blob associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="blobName">The name of a blob to deleted</param>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/blob/{schemaName}/{className}/{oid:long}/{blobName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "File deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(void), Description = "File not found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DeleteBlob(string schemaName, string className, string oid, string blobName)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string containerName = GetContainerName(parameters, schemaName, className, oid);

                IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(schemaName, className);

                await storageProvider.DeleteBlobAsync(containerName, blobName);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Download a blob
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="blobName">The name of a blob to download</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/blob/{schemaName}/{className}/{oid:long}/{blobName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "File downloaded")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetBlob(string schemaName, string className, string oid, string blobName)
        {
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string containerName = GetContainerName(parameters, schemaName, className, oid);

            IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(schemaName, className);
            var stream = await storageProvider.GetBlobStreamAsync(containerName, blobName);
            
            BlobStorageUtil util = new BlobStorageUtil();
            return await util.CreateHttpResponse(blobName, stream);
        }

        /// <summary>
        /// Get info of the bucket configured for the schema and class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/blob/buckets/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FileViewModel>), Description = "A collection of attachment infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetBucketInfo(string schemaName, string className)
        {
            try
            {
                IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(schemaName, className);
                var bucketModel = new BucketModel
                {
                    Name = storageProvider.BucketConfig?.Name,
                    Type = storageProvider.BucketConfig?.Type,
                    ServiceUrl = storageProvider.BucketConfig?.ServiceUrl,
                    Path = storageProvider.BucketConfig?.Path,
                    ForDBClass = storageProvider.BucketConfig?.ForDBClass,
                    ForDBSchema = storageProvider.BucketConfig?.ForDBSchema
                };
                return Ok(bucketModel);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Check if a bucket exists
        /// </summary>
        /// <param name="bucketName">A bucket name</param>
        [HttpGet]
        [Route("api/blob/buckets/{bucketName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool), Description = "Boolean value")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DoesBucketExist(string bucketName)
        {
            try
            {
                var bucket = GetBucketConfig(bucketName);
                return bucket != null ? Ok(true) : Ok(false);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }
        /// <summary>
        /// Get file objectsin a bucket with a prefix 
        /// </summary>
        /// <param name="bucketName">A bucket name</param>
        /// <remarks>File objects</remarks>
        [HttpGet]
        [Route("api/blob/objects/{bucketName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FileViewModel>), Description = "A collection of blob objects")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetBlobObjects(string bucketName)
        {
            try
            {
                var bucket = GetBucketConfig(bucketName);
                if (bucket == null)
                {
                    return NotFound();
                }

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                string prefix = GetParamValue(parameters, PREFIX, null);

                IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(bucket);

                var results = await storageProvider.ListBlobsAsync(prefix);

                BlobStorageUtil util = new BlobStorageUtil();
                var fileViewModels = util.ConvertToFileViewModels(results);
                return Ok(new { files = fileViewModels });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Upload an object
        /// </summary>
        /// <param name="bucketName">A bucket name</param>
        /// <param name="objectName">An object name</param>
        [HttpPut]
        [Route("api/blob/objects/{bucketName}/{objectName}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PutSingleBlobObject(string bucketName, string objectName)
        {
            var bucket = GetBucketConfig(bucketName);
            if (bucket == null)
            {
                return NotFound();
            }

            if (Request.Content.Headers.ContentType.MediaType != "application/octet-stream")
            {
                return BadRequest(@"Unsupported media type {Request.Content.Headers.ContentType.MediaType}");
            }

            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string prefix = GetParamValue(parameters, PREFIX, null);
            string userName = GetParamValue(parameters, "user", null);
            IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(bucket);

            try
            {
                var filename = objectName;
                filename = filename.Trim(new char[] { '"' }).Replace("&", "and");
                var blobProperties = new BlobProperties()
                {
                    Metadata = new Dictionary<string, string>() { { "creator", userName ?? "Unknown" } }
                };
                var stream = await Request.Content.ReadAsStreamAsync();
                await storageProvider.SaveBlobStreamAsync(prefix,
                     filename,
                     stream,
                     blobProperties,
                     true);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Upload an object
        /// </summary>
        /// <param name="bucketName">A bucket name</param>
        /// <param name="objectName">An object name</param>
        [HttpPost]
        [Route("api/blob/objects/{bucketName}/{objectName}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PostMultiPartBlobObject(string bucketName, string objectName)
        {
            var bucket = GetBucketConfig(bucketName);
            if (bucket == null)
            {
                return NotFound();
            }

            if (Request.Content.Headers.ContentType.MediaType != "application/octet-stream")
            {
                return BadRequest(@"Unsupported media type {Request.Content.Headers.ContentType.MediaType}");
            }

            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string prefix = GetParamValue(parameters, PREFIX, null);
            string userName = GetParamValue(parameters, "user", null);
            IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(bucket);

            try
            {
                var filename = objectName;
                filename = filename.Trim(new char[] { '"' }).Replace("&", "and");
                var blobProperties = new BlobProperties()
                {
                    Metadata = new Dictionary<string, string>() { { "creator", userName ?? "Unknown" } }
                };
                var stream = await Request.Content.ReadAsStreamAsync();
                await storageProvider.SaveBlobStreamAsync(prefix,
                     filename,
                     stream,
                     blobProperties,
                     true);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get descriptor of an object
        /// </summary>
        /// <param name="bucketName">A bucket name</param>
        /// <param name="objectName">An object name</param>
        [HttpHead]
        [Route("api/blob/objects/{bucketName}/{objectName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "object")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetBlobObjectDescriptor(string bucketName, string objectName)
        {
            var bucket = GetBucketConfig(bucketName);
            if (bucket == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string prefix = GetParamValue(parameters, PREFIX, null);
            IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(bucket);

            var descriptor = await storageProvider.GetBlobDescriptorAsync(prefix, objectName);

            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Headers.Add("ContentType", descriptor.ContentType);
            response.Headers.Add("Length", descriptor.Length.ToString());
            response.Headers.Add("Name", descriptor.Name);
            response.Headers.Add("Url", descriptor.Url);
            if (descriptor.Metadata != null)
            {
                response.Headers.Add("Metadata", JsonConvert.SerializeObject(descriptor.Metadata));
            }

            return response;
        }

        /// <summary>
        /// Get an object
        /// </summary>
        /// <param name="bucketName">A bucket name</param>
        /// <param name="objectName">An object name</param>
        [HttpGet]
        [Route("api/blob/objects/{bucketName}/{objectName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "object")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetBlobObject(string bucketName, string objectName)
        {
            var bucket = GetBucketConfig(bucketName);
            if (bucket == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string prefix = GetParamValue(parameters, PREFIX, null);
            IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(bucket);

            var stream = await storageProvider.GetBlobStreamAsync(prefix, objectName);

            BlobStorageUtil util = new BlobStorageUtil();
            return await util.CreateHttpResponse(objectName, stream);
        }

        /// <summary>
        /// Delete a blob associated with a data instance
        /// </summary>
        [HttpDelete]
        [Route("api/blob/objects/{bucketName}/{objectName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "File deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(void), Description = "File not found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DeleteBlobObject(string bucketName, string objectName)
        {
            try
            {
                var bucket = GetBucketConfig(bucketName);
                if (bucket == null)
                {
                    return NotFound();
                }

                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string prefix = GetParamValue(parameters, PREFIX, null);
                IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(bucket);

                await storageProvider.DeleteBlobAsync(prefix, objectName);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        private string GetContainerName(NameValueCollection parameters, string schemaName, string className, string oid)
        {
            // The prefix to filter the list of blobs. In terms of Local storage, prefix is a subdirectory.
            // If prefix is empty, the value of a property called prefixProperty is used as prefix.
            string prefix = GetParamValue(parameters, PREFIX, null);
            prefix = !string.IsNullOrEmpty(prefix) ? System.Web.HttpUtility.UrlDecode(prefix) : null;
            string prefixProperty = GetParamValue(parameters, PREFIX_PROPERTY, null);

            BlobStorageUtil util = new BlobStorageUtil();
            return util.GetContainerName(schemaName, className, oid, prefix, prefixProperty);
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

        private BucketConfig GetBucketConfig(string bucketName)
        {
            var buckets = BlobStorageConfig.Instance.BucketConfigs;
            return buckets.Where(x => string.Equals(x.Name, bucketName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }
}

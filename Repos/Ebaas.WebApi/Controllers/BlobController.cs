using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Swashbuckle.Swagger.Annotations;

using Newtera.Common.Core;
using Newtera.BlobStorage;
using Ebaas.WebApi.Models;
using Ebaas.WebApi.Infrastructure;

namespace Ebaas.WebApi.Controllers
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
        /// <param name="from">Return attachment infos from the row index such as 0. Default to 0.</param>
        /// <param name="size">Return attachment infos with a page size such as 20. Default to 20.</param>
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
                
                return Ok(new { blobs = results });
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
        /// <param name="postedFile">The posted file object</param>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <remarks>The posted files are part of formdata</remarks>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/blob/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "Result of uploading files")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PostBlob(System.Web.HttpPostedFileBase postedFile,
            string schemaName, string className, string oid)
        {
            try
            {
                if (postedFile.ContentLength > 0)
                {
                    NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                    string containerName = GetContainerName(parameters, schemaName, className, oid);

                    IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(schemaName, className);

                    await storageProvider.SaveBlobStreamAsync(containerName,
                        postedFile.FileName,
                        postedFile.InputStream);
                }
  
                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }

        }

        /// <summary>
        /// Delete a blob associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="blobName">The name of a blob to download</param>
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
        public async Task<HttpResponseMessage> GetBlob(string schemaName, string className, string oid, string blobName, string dirPath = null)
        {
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string containerName = GetContainerName(parameters, schemaName, className, oid);

            IStorageProvider storageProvider = StorageProviderFactory.Instance.Create(schemaName, className);
            var stream = await storageProvider.GetBlobStreamAsync(containerName, blobName);
            BlobStorageUtil util = new BlobStorageUtil();
            return await util.CreateHttpResponse(blobName, stream);
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
    }
}

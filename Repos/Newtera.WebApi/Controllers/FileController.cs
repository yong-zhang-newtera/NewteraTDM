using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using Swashbuckle.Swagger.Annotations;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using Newtera.Common.Core;
using Newtera.WebApi.Models;
using Newtera.WebApi.Infrastructure;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The Files Service stores files that your application needs to store on a file server. 
    /// It has set of operations for uploading, downloading, and deleting files associated with a data instance.
    /// </summary>
    public class FileController : ApiController
    {
        private const string START_ROW = "from";
        private const string PAGE_SIZE = "size";
        private const string DIR_PATH = "path";

        private IFileManager attachmentManager;
        private IFileManager fileManager;

        public FileController()
            : this(new AttachmentManager())
        {            
        }

        public FileController(IFileManager attachmentManager)
        {
            this.attachmentManager = attachmentManager;
            this.fileManager = new LocalFileManager();
        }

        /// <summary>
        /// Get attachment infos associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="from">Return attachment infos from the row index such as 0. Default to 0.</param>
        /// <param name="size">Return attachment infos with a page size such as 20. Default to 20.</param>
        /// <remarks>Attachments associated with a data instance are stored in a dedicated directory for attachments. Each attachment has a corresponsing data record in database.</remarks>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/attachment/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FileViewModel>), Description = "A collection of attachment infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetAttachments(string schemaName, string className, string oid, int? from=null, int? size=null)
        {
            try {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 30));
                int start = int.Parse(GetParamValue(parameters, START_ROW, 0));

                var results = await attachmentManager.Get(schemaName, className, oid, start, pageSize, null);
                return Ok(new { files = results });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get count of attachments associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <returns>count</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/attachment/{schemaName}/{className}/{oid:long}/count")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int), Description = "Total count of attachment infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> AttachmentCount(string schemaName, string className, string oid)
        {
            try {
                var result = await attachmentManager.Count(schemaName, className, oid, null);
                return Ok(new { count = result });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Upload attachments associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <remarks>The attachment files are part of formdata</remarks>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/attachment/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "Result of uploading attachments")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PostAttachment(string schemaName, string className, string oid)
        {
            // Check if the request contains multipart/form-data.
            if(!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                var files = await attachmentManager.Add(Request, schemaName, className, oid, null);
                var uploadResult = new UploadFileResultModel();
                uploadResult.message = "files uploaded ok";
                uploadResult.files = files;
                return Ok(uploadResult);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n"  + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
            
        }

        /// <summary>
        /// Delete an attachment associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="attachmentId">The id of an attachmenet to be deleted</param>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/attachment/{schemaName}/{className}/{oid}/{attachmentId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "Attachment deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(void), Description = "Attachment not found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DeleteAttachment(string schemaName, string className, string oid, string attachmentId)
        {
            try
            {
                var result = await this.attachmentManager.Delete(schemaName, null, null, attachmentId, null);

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

        /// <summary>
        /// Download an attachment associated with an instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="attachmentId">The id of an attachmenet to download</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/attachment/{schemaName}/{className}/{oid}/{attachmentId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "Attachment downloaded")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetAttachment(string schemaName, string className, string oid, string attachmentId)
        {
            var result = await attachmentManager.GetFile(schemaName, className, oid, attachmentId, null);

            return result;
        }

        /// <summary>
        /// Get files associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="from">Return attachment infos from the row index such as 0. Default to 0.</param>
        /// <param name="size">Return attachment infos with a page size such as 20. Default to 20.</param>
        /// <param name="dirPath">Indicate the sub directory path (such as dir1%5Cdir2) where the files are located. If null, get files from the base directory</param>
        /// <remarks>Files associated with a data instance are stored in a specified directory by application</remarks>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/file/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FileViewModel>), Description = "A collection of attachment infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetFiles(string schemaName, string className, string oid, int? from = null, int? size = null, string dirPath = null)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();

                int pageSize = int.Parse(GetParamValue(parameters, PAGE_SIZE, 100));
                int start = int.Parse(GetParamValue(parameters, START_ROW, 0));
                string path = GetParamValue(parameters, DIR_PATH, null);

                path = System.Web.HttpUtility.UrlDecode(path);

                var results = await fileManager.Get(schemaName, className, oid, start, pageSize, path);
                return Ok(new { files = results });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get count of files associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="dirPath">Indicate the sub directory path (such as dir1%5Cdir2) where the files are located. If null, get files from the base directory</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/file/{schemaName}/{className}/{oid:long}/count")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int), Description = "Total count of files")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> FileCount(string schemaName, string className, string oid, string dirPath = null)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string path = GetParamValue(parameters, DIR_PATH, null);

                path = System.Web.HttpUtility.UrlDecode(path);

                var result = await fileManager.Count(schemaName, className, oid, path);
                return Ok(new { count = result });
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
        /// <param name="dirPath">The subdirectory path such as dir1%5Cdir2</param>
        /// <remarks>The posted files are part of formdata</remarks>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/file/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UploadFileResultModel), Description = "Result of uploading files")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PostFile(string schemaName, string className, string oid, string dirPath = null)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string path = GetParamValue(parameters, DIR_PATH, null);

                path = System.Web.HttpUtility.UrlDecode(path);

                var files = await fileManager.Add(Request, schemaName, className, oid, path);
                return Ok(new { Message = "files uploaded ok", files = files });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }

        }

        /// <summary>
        /// Delete a file associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="fileName">The name of a file to download</param>
        /// <param name="dirPath">The subdirectory path such as dir1%5Cdir2</param>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/file/{schemaName}/{className}/{oid:long}/{fileName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "File deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(void), Description = "File not found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DeleteFile(string schemaName, string className, string oid, string fileName, string dirPath=null)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string path = GetParamValue(parameters, DIR_PATH, null);

                path = System.Web.HttpUtility.UrlDecode(path);

                var result = await this.fileManager.Delete(schemaName, className, oid, fileName, path);

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

        /// <summary>
        /// Download a file associated with a data instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <param name="fileName">The name of a file to download</param>
        /// <param name="dirPath">The subdirectory path such as dir1%5Cdir2</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/file/{schemaName}/{className}/{oid:long}/{fileName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "File downloaded")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetFile(string schemaName, string className, string oid, string fileName, string dirPath = null)
        {
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();
            string path = GetParamValue(parameters, DIR_PATH, null);

            path = System.Web.HttpUtility.UrlDecode(path);

            var result = await fileManager.GetFile(schemaName, className, oid, fileName, path);

            return result;
        }

        /// <summary>
        /// Get a file content in json format from a file under UserFiles\\{schemaName} directory
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="file">The name of a file such as lineChart.json</param>
        /// <param name="dirPath">The subdirectory path such as dir1%5Cdir2</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/file/content/{schemaName}/{file}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "A json data returned")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetJSONDataFromFile(string schemaName, string file, string dirPath = null)
        {
            try
            {
                NameValueCollection parameters = Request.RequestUri.ParseQueryString();
                string path = GetParamValue(parameters, DIR_PATH, null);

                path = System.Web.HttpUtility.UrlDecode(path);

                Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                schemaInfo.Name = schemaName;
                schemaInfo.Version = "1.0";
                string schemaId = schemaInfo.NameAndVersion; // schemaName1.0

                var jsondata = await fileManager.GetJSonData(schemaId, file, path);
                return Ok(jsondata);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the roots of class trees
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of an data instance such as 377382882</param>
        /// <returns>A Tree of json object with a tree root</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/file/directory/{schemaName}/{className}/{oid:long}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DirectoryViewModel), Description = "Root of the directory tree ")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetFileDirectory(string schemaName, string className, string oid)
        {
            try
            {
                var result = await fileManager.GetDirectoryTree(schemaName, className, oid);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

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
    }
}

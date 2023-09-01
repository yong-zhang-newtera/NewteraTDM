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

using Newtera.Common.Core;
using Newtera.WebApi.Models;
using Newtera.WebApi.Infrastructure;
using Newtera.WebApi.Utils;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The ImportExport service allows you to import data from files using a canned import process and export a set of data as a data package.
    /// </summary>
    public class ImportExportController : ApiController
    {
        private IImportExportManager importExportManager;
        private string OBJ_IDS = "oids";
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        /// <summary>
        /// Constructor
        /// </summary>
        public ImportExportController()
        {
            importExportManager = new ImportExportManager();
        }

        /// <summary>
        /// Get infos of the import scripts defined for a specific file type and a given class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="fileType">Indicating the file type that scripts are created for, valid options are All, Excel, Text, or Other. Default to All</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/import/scripts/{schemaName}/{className}/{fileType}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ScriptViewModel>), Description = "The existing import script infos for a certain file type ")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetScripts(string schemaName, string className, string fileType)
        {
            try
            {
                var result = await importExportManager.GetScripts(schemaName, className, fileType);
                return Ok(new { scripts = result });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Import data from files into a given class using the specified import script
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="script">The name of a script to import a file</param>
        /// <remarks>The data files are part of formdata</remarks>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/import/files/{schemaName}/{className}/{script}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "Data import completed successfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ImportFile(string schemaName, string className, string script)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                var result = await importExportManager.ImportFiles(Request, schemaName, className, script);
                if (string.IsNullOrEmpty(result))
                {
                    return Ok(new { Message = "files imported ok" });
                }
                else
                {
                    // something wrong with the action
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Import data from files into a class that is related to an data instance of a master class using an import script
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A name of the master data class</param>
        /// <param name="oid">The obj_id of a master instance</param>
        /// <param name="relatedClass">The name of a class that is related to the master class via a many-to-one relationship</param>
        /// <param name="script">The name of a script to import a file. The script is defined for the related class</param>
        /// <remarks>The data files are part of formdata. The imported data instances will have relationship to the master data instance.</remarks>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/import/files/{schemaName}/{className}/{oid}/{relatedClass}/{script}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "related data import completed successfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ImportFile(string schemaName, string className, string oid, string relatedClass, string script)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                var result = await importExportManager.ImportFiles(Request, schemaName, className, oid, relatedClass, script);
                if (string.IsNullOrEmpty(result))
                {
                    return Ok(new { Message = "files imported ok" });
                }
                else
                {
                    // something wrong with the action
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Download a data package of an instance
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A name of the master data class</param>
        /// <param name="oids">The obj_id of data instances such as 22999222,2882992,23492999, required</param>
        /// <returns></returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/export/datapackage/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "Data package downloaded")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<HttpResponseMessage> GetDataPackage(string schemaName, string className, string oids = null)
        {
            NameValueCollection parameters = Request.RequestUri.ParseQueryString();

            // get obj_ids of data instances in form of 22999222,2882992,292999
            string objIds = GetParamValue(parameters, OBJ_IDS, null);
            if (string.IsNullOrEmpty(objIds))
            {
                throw new Exception("Missing parameter " + OBJ_IDS + " in api/export/datapackage/{schemaName}/{className} api call.");
            }
            QueryHelper queryHelper = new QueryHelper();
            string connectionStr = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
            string schemaId = GetSchemaId(schemaName);
            PackDataPackageUtil util = new PackDataPackageUtil(connectionStr,
                                                             null,
                                                             schemaId,
                                                             className,
                                                             objIds,
                                                             null,
                                                             null);

            var result = await util.DownloaddDataPackage();

            return result;
        }

        /// <summary>
        /// Import a data package
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A name of the master data class</param>
        /// <returns></returns>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/import/datapackage/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "Data package imported")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ImportDataPackage(string schemaName, string className)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                QueryHelper queryHelper = new QueryHelper();
                string connectionStr = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
                string schemaId = GetSchemaId(schemaName);
                UnpackDataPackageUtil util = new UnpackDataPackageUtil(connectionStr,
                                                                 schemaId,
                                                                  false);

                var result = await util.ImportDataPackage(Request);

                if (string.IsNullOrEmpty(result))
                {
                    return Ok(new { Message = "Data package imported ok" });
                }
                else
                {
                    // something wrong with the action
                    return BadRequest(result);
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

        private string GetSchemaId(string schemaName)
        {
            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
            schemaInfo.Name = schemaName;
            schemaInfo.Version = "1.0";
            
            return schemaInfo.NameAndVersion;
        }
    }
}

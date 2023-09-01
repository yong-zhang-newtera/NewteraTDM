using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Http.Description;

using Swashbuckle.Swagger.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.WebApi.Infrastructure;
using Newtera.WebApi.Models;
using Newtera.Common.Core;
using Newtera.Server.Logging;


namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The Log Service provides the APIs for getting logging information about actions performed on the data.
    /// </summary>
    public class LogController : ApiController
    {
        /// <summary>
        /// Get logging records of updating a property of a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A class name such as TestItem</param>
        /// <param name="oid">An instance id such as 2882992</param>
        /// <param name="propertyName">A property name such as ProductCode</param>
        /// <returns>A collection of Log instances</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/log/{schemaName}/{className}/{oid}/{propertyName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<LoggingMessage>), Description = "logging messages for the property")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetPropertyLogs(string schemaName, string className, string oid, string propertyName)
        {
            try
            {
                Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                schemaInfo.Name = schemaName;
                schemaInfo.Version = "1.0";
                List<LoggingMessage> logs = null;

                string schemaId = schemaInfo.NameAndVersion;

                await Task.Factory.StartNew(() =>
                {
                    logs = LoggingManager.Instance.GetMatchedLoggingMessages(schemaId, className, propertyName, oid);
                });

                return Ok(logs);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
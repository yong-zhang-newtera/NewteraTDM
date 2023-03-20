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

using Swashbuckle.Swagger.Annotations;

using Ebaas.WebApi.Infrastructure;
using Newtera.Common.Core;
using Ebaas.WebApi.Hubs;
using Newtera.WFModel;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// Using the Message Service for sending push notifications to your applications removes the complexity of
    /// integrating with multiple vendor-specific notification services and gives you an easy to use push portal.
    /// </summary>
    public class MessageController : ApiController
    {
        /// <summary>
        /// Get messages of the requesting user
        /// </summary>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/messages")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Models.MessageModel>), Description = "All messages for the requesting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetUserMessages()
        {
            try
            {
                MessageManager messageManager = new MessageManager();
                var results = await messageManager.GetMessages();
                return Ok(results);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets count of messages of the requesting user
        /// </summary>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/messages/count")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int), Description = "Count of messages for the requesting user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetUserMessageCount()
        {
            try
            {
                MessageManager messageManager = new MessageManager();
                var results = await messageManager.GetMessageCount();
                return Ok(results);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Post a message to a group
        /// </summary>
        /// <param name="groupName">A message group name in form of SchemaName-ClassName-ObjId</param>
        /// <param name="message">A json object as a message</param>
        /// <returns>The message id</returns>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/messages/{groupName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string), Description = "The obj_id of the posted message")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> AddMessage(string groupName, Models.MessageModel message)
        {
            try
            {                
                await Task.Factory.StartNew(() =>
                {
                    MessageInfo messageInfo = new MessageInfo();
                    messageInfo.SchemaName = message.dbschema;
                    messageInfo.SenderName = message.posterId;
                    messageInfo.SendTime = message.postTime;
                    messageInfo.Subject = message.subject;
                    messageInfo.ClassName = message.dbclass;
                    messageInfo.ObjId = message.oid;
                    messageInfo.Content = message.content;
                    messageInfo.Url = message.url;
                    messageInfo.UrlParams = message.urlparams;

                    MessageHubMaster.Instance.SendMessageToGroup(groupName, messageInfo);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Delete a message
        /// </summary>
        /// <param name="oid">the obj_id of a message</param>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/messages/{oid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "The message removed")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> RemoveMessage(string oid)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    MessageManager messageManager = new MessageManager();
                    messageManager.RemoveMessage(oid);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Clear all messages of the requesting user
        /// </summary>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/messages")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "The messages cleared")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> ClearMessages()
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    MessageManager messageManager = new MessageManager();
                    messageManager.ClearMessages();
                });

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
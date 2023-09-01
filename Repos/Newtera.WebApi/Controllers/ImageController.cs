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
using Newtera.WebApi.Models;
using Newtera.WebApi.Infrastructure;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The Image Service stores image files that your application needs to display. It has set of operations for reading, uploading, and deleting images.
    /// </summary>
    public class ImageController : ApiController
    {
        /// <summary>
        /// Get infos of the images stored in a directory
        /// </summary>
        /// <param name="subDir">A sub directory of the image directory on server</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("api/images/{subdir}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ImageModel>), Description = "A image infos")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetImageInfos(string subDir)
        {
            try
            {
                ImageManager imageManager = new ImageManager();
                string imagedir = WebApiNameSpace.CUSTOM_IMAGE_BASE_DIR + subDir;
                var results = await imageManager.GetImages(imagedir);
                return Ok(results);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Post images to a directory under the system's image directory
        /// </summary>
        /// <param name="subDir">A sub directory of the image directory on server</param>
        [HttpPost]
        [NormalAuthorizeAttribute]
        [Route("api/images/{subdir}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ImageModel>), Description = "Images saved")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> PostImages(string subDir)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                ImageManager imageManager = new ImageManager();
                string imagedir = WebApiNameSpace.CUSTOM_IMAGE_BASE_DIR + subDir;
                var results = await imageManager.AddImages(Request, imagedir);
                return Ok(results);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Delete an image in a directory under the images directory
        /// </summary>
        /// <param name="subDir">A sub directory of the image directory on server</param>
        /// <param name="imageName">The image name</param>
        [HttpDelete]
        [NormalAuthorizeAttribute]
        [Route("api/images/{subdir}/{imageName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ImageModel>), Description = "Images deleted")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> DeleteImage(string subDir, string imageName)
        {
            try
            {
                ImageManager imageManager = new ImageManager();
                string imagedir = WebApiNameSpace.CUSTOM_IMAGE_BASE_DIR + subDir;
                var results = await imageManager.DeleteImage(imagedir, imageName);
                return Ok(results);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}

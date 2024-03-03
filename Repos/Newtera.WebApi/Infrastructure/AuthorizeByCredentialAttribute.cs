using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Security.Principal;

using Newtera.Server.UsrMgr;
using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WebApi.Infrastructure
{
    public class AuthorizeByCredentialAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = false;

            IEnumerable<string> headerValues;
            var userId = string.Empty;
            var userPwd = string.Empty;

            if (actionContext.Request.Headers.TryGetValues("accesskey", out headerValues))
            {
                userId = headerValues.FirstOrDefault();
            }

            if (actionContext.Request.Headers.TryGetValues("secretkey", out headerValues))
            {
                userPwd = headerValues.FirstOrDefault();
            }

            IUserManager userManager = new ServerSideUserManager();

            isAuthorized = userManager.Authenticate(userId, userPwd);

            if (isAuthorized)
            {
                // attach a custom principal to the thread for userid
                CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy(), userId);
            }

            return isAuthorized;
        }
    }
}
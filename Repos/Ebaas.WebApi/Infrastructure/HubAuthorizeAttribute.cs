using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;

using Newtera.Server.UsrMgr;
using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;

namespace Ebaas.WebApi.Infrastructure
{
    public class HubAuthorizeAttribute : AuthorizeAttribute
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = false;

            try {

                string userName = System.Web.HttpContext.Current.Request.QueryString["user"];
                if (string.IsNullOrEmpty(userName))
                {
                    isAuthorized = false;
                }
                else
                {
                    // attach a custom principle to the thread
                    CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy());

                    isAuthorized = true;
                }
               
                return isAuthorized;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
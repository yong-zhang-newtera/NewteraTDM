using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;

using Newtera.Server.UsrMgr;
using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WebApi.Infrastructure
{
    public class NormalAuthorizeAttribute : AuthorizeAttribute
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = true;

            try {
                var identity = Thread.CurrentPrincipal.Identity;

                // require authorization by default
                if (identity != null &&
                    identity.IsAuthenticated)
                {
                    // attach a custom principle to the thread
                    CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy());

                    SetContextInfo(actionContext);

                    isAuthorized = true;
                }
                else if (identity != null && !identity.IsAuthenticated)
                {
                    isAuthorized = true;
                }
                else
                {
                    isAuthorized = false; // unauthrozed
                }

                return isAuthorized;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private void SetContextInfo(HttpActionContext actionContext)
        {
            try
            {
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;

                if (customPrincipal != null)
                {
                    var routeData = actionContext.ControllerContext.RouteData;

                    object val;
                    routeData.Values.TryGetValue("schemaName", out val);
                    if (val != null)
                    {
                        string schemaName = val.ToString();
                        QueryHelper queryHelper = new QueryHelper();
                        string connectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
                        customPrincipal.SetUserData(NewteraNameSpace.CURRENT_CONNECTION, connectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
        }
    }
}
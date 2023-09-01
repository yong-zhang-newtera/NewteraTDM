using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Collections.Specialized;

using Newtera.Server.UsrMgr;
using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WebApi.Infrastructure
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = true;

            try {

                CMUserManager userMgr = new CMUserManager();
                // HACK, assign the user as a super user to execute the api dedicated for winform tools
                Thread.CurrentPrincipal = userMgr.SuperUser;

                SetContextInfo(actionContext);
            }
            catch (Exception)
            {
                // ignore since some of admin apis do not depends on the database connection
                isAuthorized = true;
            }

            return isAuthorized;
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
                    routeData.Values.TryGetValue("schemaId", out val);
                    if (val != null)
                    {
                        string schemaId = val.ToString();
                        if (!string.IsNullOrEmpty(schemaId))
                        {
                            string[] values = schemaId.Split(' ');
                            string schemaName = values[0];
                            QueryHelper queryHelper = new QueryHelper();
                            string connectionString = queryHelper.GetConnectionString(CONNECTION_STRING, schemaName);
                            customPrincipal.SetUserData(NewteraNameSpace.CURRENT_CONNECTION, connectionString);
                        }
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
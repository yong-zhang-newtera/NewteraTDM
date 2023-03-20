using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Api;
using Newtera.Server.DB;
using Newtera.Server.Engine.Cache;
using Newtera.Common.MetaData;
using Newtera.Server.UsrMgr;
using Newtera.Common.MetaData.Principal;

namespace Ebaas.WebApi.Infrastructure
{
    public class AuthorizeByMetaDataAttribute : AuthorizeAttribute
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                bool isAuthorized = true;

                var routeData = actionContext.ControllerContext.RouteData;

                string schemaName = null;
                object val = null;

                routeData.Values.TryGetValue("schemaName", out val);
                if (val != null)
                {
                    schemaName = val.ToString();
                }
                else
                {
                    throw new MissingFieldException("Missing schemaName in the route data.");
                }

                routeData.Values.TryGetValue("className", out val);
                string className = null;
                if (val != null)
                {
                    className = val.ToString();
                }
                string apiName = actionContext.ActionDescriptor.ActionName;

                IDataProvider dataProvider = DataProviderFactory.Instance.Create();
                SchemaInfo schemaInfo = new SchemaInfo();
                schemaInfo.Name = schemaName;
                schemaInfo.Version = "1.0";

                MetaDataModel metaData = MetaDataCache.Instance.GetMetaData(schemaInfo, dataProvider);

                var identity = Thread.CurrentPrincipal.Identity;


                if (metaData != null &&
                    !string.IsNullOrEmpty(className) &&
                    metaData.ApiManager.HasApis(className))
                {
                    Api api = metaData.ApiManager.GetClassApi(className, apiName);

                    if (api != null &&
                        api.IsAuthorized)
                    {
                        if (identity != null &&
                            identity.IsAuthenticated)
                        {
                            // attach a custom principle to the thread
                            CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy());

                            SetContextInfo(actionContext);

                            isAuthorized = true;
                        }
                        else
                        {
                            isAuthorized = false; // unauthrozed
                        }
                    }
                    else
                    {
                        // require authorization by default
                        if (identity != null &&
                            identity.IsAuthenticated)
                        {
                            // attach a custom principle to the thread
                            CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy());

                            SetContextInfo(actionContext);

                            isAuthorized = true;
                        }
                        else
                        {
                            isAuthorized = false; // unauthrozed
                        }
                    }
                }
                else
                {
                    // require authorization by default
                    if (identity != null &&
                        identity.IsAuthenticated)
                    {
                        // attach a custom principle to the thread
                        CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy());

                        SetContextInfo(actionContext);

                        isAuthorized = true;
                    }
                    else
                    {
                        isAuthorized = false; // unauthrozed
                    }
                }

                return isAuthorized;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                //throw ex;
                // TODO
                return true;
            }
        }

        private void SetContextInfo(HttpActionContext actionContext)
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
    }
}
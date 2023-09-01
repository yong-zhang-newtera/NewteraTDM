using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Dynamic;
using System.Xml;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Swashbuckle.Swagger.Annotations;

using Newtera.WebApi.Infrastructure;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.SiteMap;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Server.Engine.Cache;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Data;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.XaclModel.Processor;
using Newtera.Server.Engine.Interpreter;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The SiteMap Service allows you to develop a framework of your frontend driven by a sitemap model so that users with different roles can have personalized user interface.
    /// </summary>
    [RoutePrefix("api/sitemap")]
    public class SiteMapController : ApiController
    {
        private string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string TEMPLATE_XQUERY = "let $this := getCurrentInstance() return <flag>{if ($$) then 1 else 0}</flag>";
        private const string MENU_ITEM_REF = "ref";
        private const string PARAMETER_HASH = "hash";
        private const string DEFAULT_HELP = "DefaultHelp.pdf";

        /// <summary>
        /// Get the basic settings of the site map
        /// </summary>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("settings")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object for sitemap general settings")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetSiteMapSettings()
        {
            try
            {
                JObject settings = new JObject();

                await Task.Factory.StartNew(() =>
                {

                    Newtera.Common.MetaData.SiteMap.SiteMapModel siteMapModel = SiteMapManager.Instance.SiteMapModel;
                    if (siteMapModel != null)
                    {
                        settings.Add("database", siteMapModel.Database);
                        settings.Add("language", siteMapModel.Language);
                    }
                });

                return Ok(settings);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets a tree of the menu items personalized for the requesting user
        /// </summary>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("menu")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A hierarchical json object representing a menu item tree")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetMenuItems()
        {
            try {
                JObject rootMenuItem = null;

                await Task.Factory.StartNew(() =>
                {
                    XaclPolicy policy = SiteMapManager.Instance.Policy;
                    Newtera.Common.MetaData.SiteMap.SiteMap siteMap = SiteMapManager.Instance.SiteMap;
                    if (siteMap != null)
                    {
                        GenerateMenuItemsVisitor visitor = new GenerateMenuItemsVisitor(policy);

                        siteMap.Accept(visitor);

                        rootMenuItem = visitor.RootMenuItem;
                    }
                });

                if (rootMenuItem != null)
                {
                    return Ok(rootMenuItem);
                }
                else
                {
                    throw new Exception("Unable to get menu items");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the custom commands defined for a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("commands/{schemaName}/{className}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<JObject>), Description = "A array of json object representing custom commands of a class ")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetClassCommands(string schemaName, string className)
        {
            try
            {
                List<JObject> commands = new List<JObject>();

                await Task.Factory.StartNew(() =>
                {
                    AddCustomCommands(commands, schemaName, className, null); // add custom commands defined in the sitemap studio for the class
                });

                return Ok(commands);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets custom commands of a data instance in a class
        /// </summary>
        /// <param name="schemaName">A database schema name such as DEMO</param>
        /// <param name="className">A data class name such as ATestItemInstance</param>
        /// <param name="oid">The obj_id of a data instance</param>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("commands/{schemaName}/{className}/{oid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<JObject>), Description = "A array of json object representing custom commands of a data instance ")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetInstanceCommands(string schemaName, string className, string oid)
        {
            try {
                List<JObject> commands = new List<JObject>();

                await Task.Factory.StartNew(() =>
                {
                    AddCustomCommands(commands, schemaName, className, oid); // add custom commands defined in the sitemap studio for the class
                });

                return Ok(commands);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets the parameters of a menu item in the sitemap
        /// </summary>
        /// <param name="hash">A hashcode identifying the menu item</param>
        /// <returns>A JSON object with parameter names and values, example {param1:xxx, param2: xxxx, param3: xxxx}</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("parameters/{hash}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A json object representing parameters of a sitemap menu item ")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetNodeParameters(string hash)
        {
            try
            {
                JObject jobject = new JObject();

                SiteMapNode found = null;

                await Task.Factory.StartNew(() =>
                {
                    Newtera.Common.MetaData.SiteMap.SiteMap siteMap = SiteMapManager.Instance.SiteMap;
                    if (siteMap != null && !string.IsNullOrEmpty(hash))
                    {
                        int hashCodeInt = int.Parse(hash);
                        found = siteMap.FindSiteMapNode(hashCodeInt) as SiteMapNode;
                    }

                    if (found != null && found.StateParameters != null)
                    {
                        foreach (StateParameter parameter in found.StateParameters)
                        {
                            jobject.Add(parameter.Name, parameter.Value);
                        }
                    }
                });

                return Ok(jobject);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Gets the url of the help document for a menu item in the sitemap
        /// </summary>
        /// <param name="hash">A hashcode identifying the menu item</param>
        /// <returns>A document url string such as myspace,html</returns>
        [HttpGet]
        [NormalAuthorizeAttribute]
        [Route("help/{hash}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject), Description = "A document url string such as myspace,html ")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in data.message")]
        public async Task<IHttpActionResult> GetNodeHelpDocUrl(string hash)
        {
            try
            {
                string helpDoc = DEFAULT_HELP;

                SiteMapNode found = null;

                await Task.Factory.StartNew(() =>
                {
                    Newtera.Common.MetaData.SiteMap.SiteMap siteMap = SiteMapManager.Instance.SiteMap;
                    if (siteMap != null && !string.IsNullOrEmpty(hash))
                    {
                        int hashCodeInt = int.Parse(hash);
                        found = siteMap.FindSiteMapNode(hashCodeInt) as SiteMapNode;
                    }

                    if (found != null && !string.IsNullOrEmpty(found.HelpDoc))
                    {
                        helpDoc = found.HelpDoc;
                    }
                });

                return Ok(helpDoc);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        private void AddCustomCommands(List<JObject> commands, string schemaName, string className, string oid)
        {
            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
            schemaInfo.Name = schemaName;
            string schemaId = schemaInfo.NameAndVersion;
           
            CustomCommandGroup customCommandGroup = SiteMapManager.Instance.GetCustomCommandGroup(schemaId, className);
            Newtera.Common.MetaData.SiteMap.SiteMap siteMap = SiteMapManager.Instance.SiteMap;

            if (customCommandGroup != null)
            {
                XaclPolicy policy = SiteMapManager.Instance.Policy;

                Hashtable customCommandPermissions = new Hashtable();
                JObject item;

                // check the custom command access permissions
                foreach (CustomCommand customCommand in customCommandGroup.ChildNodes)
                {
                    if (!PermissionChecker.Instance.HasPermission(policy, customCommand, XaclActionType.Read))
                    {
                        customCommandPermissions[customCommand] = false;
                    }
                    else
                    {
                        customCommandPermissions[customCommand] = true;
                    }

                    if (customCommand.CommandScope == CommandScope.Instance &&
                        IsVisible(customCommand, policy, schemaName, className, oid))
                    {
                        item = new JObject();
                        item.Add("title", customCommand.Title);
                        item.Add("icon", customCommand.ImageUrl);
                        item.Add("url", customCommand.NavigationUrl);
                        item.Add("name", customCommand.Name);
                        item.Add("baseUrl", customCommand.BaseUrl);

                        var parameters = new JObject();
                        if (customCommand.StateParameters != null)
                        {
                            foreach (StateParameter parameter in customCommand.StateParameters)
                            {
                                if (parameter.Name == MENU_ITEM_REF)
                                {
                                    parameters.Add(PARAMETER_HASH, GetHashForReferencedMenu(siteMap, parameter.Value));
                                }
                                else
                                {
                                    parameters.Add(parameter.Name, parameter.Value);
                                }
                            }
                        }

                        item.Add("parameters", parameters);
                        item.Add("hash", customCommand.GetNodeHashCode());
                        commands.Add(item);
                    }
                }
            }
        }

        // Check permission of a custom command
        private bool IsVisible(CustomCommand customCommand, XaclPolicy policy, string schemaName, string className, string oid)
        {
            bool status = PermissionChecker.Instance.HasPermission(policy, customCommand, XaclActionType.Read);

            if (status &&
                oid != null &&
                !string.IsNullOrEmpty(customCommand.VisibleCondition) &&
                !MeetVisibleCondition(customCommand.VisibleCondition, schemaName, className, oid))
            {
                status = false;
            }

            return status;
        }

        private bool MeetVisibleCondition(string visibleCondition, string schemaName, string className, string oid)
        {
            bool status = true;
            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            XmlElement originalInstance = principal.CurrentInstance;
            try
            {
                if (principal != null)
                {
                    //XmlElement currentInstance = GetCurrentInstance();
                    XmlElement currentInstance = GetDataInstance(schemaName, className, oid);

                    if (currentInstance != null)
                    {
                        principal.CurrentInstance = currentInstance;

                        // build a complete xquery
                        string finalCondition = TEMPLATE_XQUERY.Replace("$$", visibleCondition);

                        IConditionRunner conditionRunner = PermissionChecker.Instance.ConditionRunner;

                        status = conditionRunner.IsConditionMet(finalCondition);
                    }
                }

                return status;
            }
            finally
            {
                // unset the current instance as a context for condition evaluation
                principal.CurrentInstance = originalInstance;
            }
        }

        private XmlElement GetDataInstance(string schemaName, string className, string oid)
        {
            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                string query = dataView.GetInstanceQuery(oid);

                Interpreter interpreter = new Interpreter();
                XmlDocument doc = interpreter.Query(query);

                if (doc != null && doc.DocumentElement.ChildNodes.Count > 0)
                {
                    return (XmlElement)doc.DocumentElement.ChildNodes[0]; // return the first element to evaluate the expression
                }
                else
                {
                    return null;
                }
            }
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }

        private string GetHashForReferencedMenu(Newtera.Common.MetaData.SiteMap.SiteMap siteMap, string path)
        {
            string hash = "";

            string menuItemPath = siteMap.BuildMenuItemPath(path); // convert nodeName1/nodeName2 to MenuItem[@Name='nodeName1']/MenuItem[@Name='nodeName2']

            ISiteMapNode found = siteMap.FindSiteMapNode(menuItemPath); // 
            if (found != null)
            {
                hash = found.GetNodeHashCode().ToString();
            }

            return hash;
        }
    }
}

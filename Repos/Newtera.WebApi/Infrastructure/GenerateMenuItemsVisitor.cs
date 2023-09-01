/*
* @(#)GenerateMenuItemsVisitor.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WebApi.Infrastructure
{
	using System;
	using System.Resources;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.SiteMap;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Traverse a site map model and generates a menu items hirarchy in json format for the current user
	/// </summary>
	/// <version> 1.0.0 11 Jan 2016 </version>
	public class GenerateMenuItemsVisitor : ISiteMapNodeVisitor
	{
        private const string PARAMETER_SCHEMA = "schema";
        private const string PARAMETER_CLASS = "class";
        private const string PARAMETER_HASH = "hash";
        private const string PARAMETER_OID = "oid";

		private XaclPolicy _policy;
        private JObject _rootItem;
        private Hashtable _parentItemTable;

        /// <summary>
        /// Instantiate an instance of GenerateMenuItemsVisitor class
        /// </summary>
        /// <param name="policy">The access control policy for the menu items</param>
        public GenerateMenuItemsVisitor(XaclPolicy policy)
		{
            _policy = policy;
            _parentItemTable = new Hashtable();
        }

        /// <summary>
        /// Viste a site map model.
        /// </summary>
        /// <param name="node">A SiteMapModel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSiteMapModel(SiteMapModel node)
        {
            return true;
        }

        /// <summary>
        /// Gets the user-specific root menu item
        /// </summary>
        public JObject RootMenuItem
        {
            get
            {
                return _rootItem;
            }
        }

        /// <summary>
        /// Viste a site map.
        /// </summary>
        /// <param name="node">A SiteMap instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSiteMap(SiteMap node)
        {
            _rootItem = new JObject();
            _rootItem.Add("items", new JArray());

            return true;
        }

        /// <summary>
        /// Viste a SideMenu.
        /// </summary>
        /// <param name="node">A Filter instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSideMenu(SideMenu node)
        {

            return true;
        }

        /// <summary>
        /// Viste a site map node.
        /// </summary>
        /// <param name="node">A SiteMapNode instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSiteMapNode(SiteMapNode node)
        {
            if (IsAccessibleToUser(node))
            {
                JObject item = new JObject();

                item.Add("name", node.Name);
                item.Add("title", node.Title);
                if (!string.IsNullOrEmpty(node.Url))
                {
                    string sref = BuildSRef(node);
                    item.Add("sref", sref);
                }
                else
                {
                    item.Add("href", "#");
                }

                item.Add("icon", node.ImageUrl);

                item.Add("visible", node.IsVisible);

                item.Add("desc", node.Description);

                item.Add("baseUrl", node.BaseUrl);

                if (!string.IsNullOrEmpty(node.Database))
                {
                    string[] strs = node.Database.Split(' '); // "schemaname 1.0"
                    // only keep the schema name without version
                    if (strs.Length > 0)
                    {
                        item.Add("schema", strs[0]);
                    }
                }

                if (!string.IsNullOrEmpty(node.ClassName))
                {
                    item.Add("class", node.ClassName);
                }

                JObject parentItem = (JObject)_parentItemTable[node.ParentNode];
                if (parentItem != null)
                {
                    if (parentItem["items"] == null)
                    {
                        parentItem.Add("items", new JArray());
                    }
                    ((JArray)parentItem["items"]).Add(item);
                }
                else
                {
                    ((JArray)_rootItem["items"]).Add(item);
                }

                if (node.StateParameters != null && node.StateParameters.Count > 0)
                {
                    JObject parameters = new JObject();
                    item.Add("parameters", parameters);
                    foreach (StateParameter parameter in node.StateParameters)
                    {
                        parameters.Add(parameter.Name, parameter.Value);
                    }
                }

                _parentItemTable[node] = item;
            }

            return true;
        }

        /// <summary>
        /// Viste a SideMenuGroup node.
        /// </summary>
        /// <param name="node">A SideMenuGroup instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSideMenuGroup(SideMenuGroup node)
        {

            return true;
        }

        /// <summary>
        /// Viste a menu node.
        /// </summary>
        /// <param name="node">A Menu instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitMenu(Menu node)
        {

            return true;
        }

        /// <summary>
        /// Viste a MenuItem node.
        /// </summary>
        /// <param name="node">A MenuItem instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitMenuItem(MenuItem node)
        {
            return true;
        }

        /// <summary>
        /// Viste a CustomCommandSet node.
        /// </summary>
        /// <param name="node">A CustomCommandSet instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitCustomCommandSet(CustomCommandSet node)
        {
            return true;
        }


        /// <summary>
        /// Viste a CustomCommandGroup node.
        /// </summary>
        /// <param name="node">A CustomCommandGroup instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitCustomCommandGroup(CustomCommandGroup node)
        {

            return true;
        }

        /// <summary>
        /// Viste a SiteMapModelSet node.
        /// </summary>
        /// <param name="node">A SiteMapModelSet instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSiteMapModelSet(SiteMapModelSet node)
        {

            return true;
        }

        /// <summary>
        /// Viste a CustomCommand node.
        /// </summary>
        /// <param name="node">A CustomCommand instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitCustomCommand(CustomCommand node)
        {

            return true;
        }

        private bool IsAccessibleToUser(SiteMapNode node)
        {
            bool status = true;

            if (_policy != null)
            {
                if (!PermissionChecker.Instance.HasPermission(_policy, node, XaclActionType.Read))
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Build a sref string with necessay parameters for the UI
        /// </summary>
        /// <param name="node">The sitemap node</param>
        /// <returns>A sref string</returns>
        private string BuildSRef(SiteMapNode node)
        {
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(node.Url))
            {
                string url = node.Url.Trim();
                string existingParameters = "";

                // get state url without parameters
                int pos = url.IndexOf('(');

                if (pos > 0)
                {
                    existingParameters = url.Substring(pos); // get ({param1:xxx,param2:xxx})
                    try {
                        existingParameters = existingParameters.Trim().Substring(1, existingParameters.Length - 2); // get rid of ()
                    }
                    catch (Exception)
                    {
                        // in case of bad format
                        existingParameters = "";
                    }

                    url = url.Substring(0, pos); // get only url
                }

                builder.Append(url);

                JObject jParameters = null;

                if (!string.IsNullOrEmpty(existingParameters))
                {
                    try
                    {
                        jParameters = JObject.Parse(existingParameters);
                    }
                    catch (Exception)
                    {
                        // in case of bad format
                        jParameters = null;
                    }
                }

                if (jParameters == null)
                {
                    // no existing parameters or existing parameter is in bad format
                    jParameters = new JObject();
                }

                // add schema parameter if non exist
                if (!string.IsNullOrEmpty(node.Database) && jParameters[PARAMETER_SCHEMA] == null)
                {
                    string schemaName = node.Database;
                    pos = schemaName.IndexOf(' ');
                    schemaName = schemaName.Substring(0, pos); // get ride of version in the database name

                    jParameters.Add(PARAMETER_SCHEMA, schemaName);
                }

                // add class parameter if non exist
                if (!string.IsNullOrEmpty(node.ClassName) && jParameters[PARAMETER_CLASS] == null)
                {
                    jParameters.Add(PARAMETER_CLASS, node.ClassName);
                }

                // add hash parameter
                jParameters.Add(PARAMETER_HASH, node.GetNodeHashCode());

                builder.Append("(").Append(JsonConvert.SerializeObject(jParameters)).Append(")");
            }

            return builder.ToString();
        }
    }
}
/*
* @(#)GetAccessibleClassesVisitor.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
namespace Ebaas.WebApi.Infrastructure
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
	public class GetAccessibleClassesVisitor : ISiteMapNodeVisitor
	{
        private string _schemaName;
		private XaclPolicy _policy;
        private Hashtable _classTable;

        /// <summary>
        /// Instantiate an instance of GetAccessibleClassesVisitor class
        /// </summary>
        /// <param name="policy">The access control policy for the menu items</param>
        public GetAccessibleClassesVisitor(XaclPolicy policy, string schemaName)
		{
            _schemaName = schemaName;
            _policy = policy;
            _classTable = new Hashtable();
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
        public Hashtable AccessibleClasses
        {
            get
            {
                return _classTable;
            }
        }

        /// <summary>
        /// Viste a site map.
        /// </summary>
        /// <param name="node">A SiteMap instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSiteMap(SiteMap node)
        { 
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
                if (!string.IsNullOrEmpty(node.Database) && node.Database.StartsWith(_schemaName))
                {
                    if (!string.IsNullOrEmpty(node.ClassName) && _classTable[node.ClassName] == null)
                    {
                        _classTable.Add(node.ClassName, "1");
                    }
                }
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

            if (!node.IsVisible)
            {
                status = false;
            }
            else if (_policy != null)
            {
                if (!PermissionChecker.Instance.HasPermission(_policy, node, XaclActionType.Read))
                {
                    status = false;
                }
            }

            return status;
        }
    }
}
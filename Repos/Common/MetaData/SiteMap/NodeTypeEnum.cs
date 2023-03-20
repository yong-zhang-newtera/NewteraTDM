/*
* @(#)NodeTypeEnum.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	/// <summary>
	/// Specify the types of nodes in SiteMap package
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Unknown node type
		/// </summary>
		Unknown,
        /// <summary>
        /// Model
        /// </summary>
        Model,
		/// <summary>
		/// Collection type
		/// </summary>
		Collection,
        /// <summary>
        /// SiteMap
        /// </summary>
        siteMap,
		/// <summary>
		/// Sitemap node collection
		/// </summary>
		SiteMapNodeCollection,
		/// <summary>
		/// SiteMapNode
		/// </summary>
        siteMapNode,
        /// <summary>
        /// SideMenu
        /// </summary>
        SideMenu,
        /// <summary>
        /// SideMenuGroup
        /// </summary>
        SideMenuGroup,
        /// <summary>
        /// Menu
        /// </summary>
        Menu,
        /// <summary>
        /// MenuItem
        /// </summary>
        MenuItem,
        /// <summary>
        /// CustomCommandSet
        /// </summary>
        CustomCommandSet,
        /// <summary>
        /// CustomCommandGroup
        /// </summary>
        CustomCommandGroup,
        /// <summary>
        /// CustomCommand
        /// </summary>
        CustomCommand,
        /// <summary>
        /// SiteMapModelSet
        /// </summary>
        SiteMapModelSet,
        /// <summary>
        /// Parameter
        /// </summary>
        Parameter
	}
}
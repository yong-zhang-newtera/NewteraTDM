/*
* @(#)TreeNodeType.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	/// <summary>
	/// Specify the types of tree nodes
	/// </summary>
	public enum TreeNodeType
	{
		Unknown = 0,
        SiteMapRootNode,
		SiteMapFolder,
		SideMenuFolder,
		SiteMapMenu,
        SideMenuGroup,
        SideMenuSearch,
        SideMenuDashboard,
        SideMenuTrees,
        SideMenuTree,
        SideMenuActions,
        SideMenuItem,
        CustomCommandFolder,
        CustomCommandGroup,
        CustomCommand,
        SiteMapSetNode
	}
}
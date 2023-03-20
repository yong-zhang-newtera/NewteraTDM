/*
* @(#)ISiteMapNodeVisitor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap
{
	using System;

	using Newtera.Common.MetaData.SiteMap;

	/// <summary>
	/// Represents an interface for visitors that traverse nodes in a site map model.
	/// </summary>
	/// <version> 1.0.0 24 Jun 2009 </version>
	public interface ISiteMapNodeVisitor
	{
		/// <summary>
		/// Viste a site map model.
		/// </summary>
        /// <param name="node">A SiteMapModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitSiteMapModel(SiteMapModel node);

		/// <summary>
		/// Viste a site map.
		/// </summary>
		/// <param name="node">A SiteMap instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitSiteMap(SiteMap node);

		/// <summary>
		/// Viste a SideMenu.
		/// </summary>
		/// <param name="node">A Filter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitSideMenu(SideMenu node);

		/// <summary>
		/// Viste a site map node.
		/// </summary>
		/// <param name="node">A SiteMapNode instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitSiteMapNode(SiteMapNode node);

		/// <summary>
		/// Viste a SideMenuGroup node.
		/// </summary>
		/// <param name="node">A SideMenuGroup instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitSideMenuGroup(SideMenuGroup node);

        /// <summary>
        /// Viste a menu node.
        /// </summary>
        /// <param name="node">A Menu instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitMenu(Menu node);

        /// <summary>
        /// Viste a MenuItem node.
        /// </summary>
        /// <param name="node">A MenuItem instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitMenuItem(MenuItem node);

        /// <summary>
        /// Viste a CustomCommandSet node.
        /// </summary>
        /// <param name="node">A CustomCommandSet instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitCustomCommandSet(CustomCommandSet node);

        /// <summary>
        /// Viste a CustomCommandGroup node.
        /// </summary>
        /// <param name="node">A CustomCommandGroup instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitCustomCommandGroup(CustomCommandGroup node);

        /// <summary>
        /// Viste a CustomCommand node.
        /// </summary>
        /// <param name="node">A CustomCommand instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitCustomCommand(CustomCommand node);

        /// <summary>
        /// Viste a SiteMapModelSet node.
        /// </summary>
        /// <param name="node">A SiteMapModelSet instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitSiteMapModelSet(SiteMapModelSet node);
	}
}
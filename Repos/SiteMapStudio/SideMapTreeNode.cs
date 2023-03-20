/*
* @(#)SiteMapTreeNode.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	using System;
	using System.Windows.Forms;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.SiteMap;
	
	/// <summary>
	/// Represents a tree node for a ISiteMapNode instance
	/// </summary>
	/// <version>  1.0.1 18 Jun 2009</version>
	public class SiteMapTreeNode : TreeNode
	{
		private TreeNodeType _type;
		private ISiteMapNode _siteMapNode;

		/// <summary>
		/// Initializes a new instance of the SiteMapTreeNode class.
		/// </summary>
		/// <param name="type">The type of node</param>
		public SiteMapTreeNode(TreeNodeType type)
		{
			_type = type;
			_siteMapNode = null;
		}

		/// <summary>
		/// Initializes a new instance of the SiteMapTreeNode class.
		/// </summary>
		/// <param name="name">The site map node</param>
		public SiteMapTreeNode(ISiteMapNode siteMapNode)
		{
			_type = GetTreeNodeType(siteMapNode);
			_siteMapNode = siteMapNode;

            // listen to the title changed event from the sitemap node
            _siteMapNode.TitleChanged += new EventHandler(this.TitleChangedHandler);
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of Newtera.Common.SiteMap.TreeNodeType enumeration</value>
		public TreeNodeType Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary> 
		/// Gets or sets the sitemap node
		/// </summary>
		/// <value> The name of the site map node</value>
		public ISiteMapNode SiteMapNode
		{
			get
			{
				return _siteMapNode;
			}
			set
			{
				_siteMapNode = value;
			}
		}

        /// <summary>
        /// Get tree node type of ISiteMapNode object
        /// </summary>
        /// <param name="node">ISiteMapNode object</param>
        /// <returns>the corresponding tree node type</returns>
        private TreeNodeType GetTreeNodeType(ISiteMapNode node)
        {
            TreeNodeType type = TreeNodeType.Unknown;

            if (node is SiteMapModelSet)
            {
                type = TreeNodeType.SiteMapSetNode;
            }
            else if (node is SiteMapModel)
            {
                type = TreeNodeType.SiteMapRootNode;
            }
            else if (node is SiteMapNode)
            {
                type = TreeNodeType.SiteMapMenu;
            }
            else if (node is SideMenuGroup)
            {
                type = TreeNodeType.SideMenuGroup;
            }
            else if (node is Newtera.Common.MetaData.SiteMap.Menu)
            {
                Newtera.Common.MetaData.SiteMap.Menu menu = (Newtera.Common.MetaData.SiteMap.Menu)node;
                switch (menu.Type)
                {
                    case MenuType.Keywords:
                        type = TreeNodeType.SideMenuSearch;
                        break;

                    case MenuType.Dashboard:
                        type = TreeNodeType.SideMenuDashboard;
                        break;

                    case MenuType.Actions:
                        type = TreeNodeType.SideMenuActions;
                        break;

                    case MenuType.Trees:
                        type = TreeNodeType.SideMenuTrees;
                        break;
                    case MenuType.Tree:
                        type = TreeNodeType.SideMenuTree;
                        break;
                }
            }
            else if (node is Newtera.Common.MetaData.SiteMap.MenuItem)
            {
                type = TreeNodeType.SideMenuItem;
            }
            else if (node is CustomCommandGroup)
            {
                type = TreeNodeType.CustomCommandGroup;
            }
            else if (node is CustomCommand)
            {
                type = TreeNodeType.CustomCommand;
            }

            return type;
        }

        /// <summary>
        /// A handler to call when a title of the site map node changed
        /// </summary>
        /// <param name="sender">the site map node that cause the event</param>
        /// <param name="e">the arguments</param>
        private void TitleChangedHandler(object sender, EventArgs e)
        {
            ValueChangedEventArgs args = (ValueChangedEventArgs)e;

            this.Text= (string)args.NewValue;
        }
	}
}
/*
* @(#)SiteMapModelValidateVisitor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.SiteMap.Validate
{
	using System;
	using System.Resources;
    using System.Collections;

	using Newtera.Common.MetaData.SiteMap;

	/// <summary>
	/// Traverse a site map model and validate each node to check if it has no validating errors.
	/// </summary>
	/// <version> 1.0.0 23 Jun 2009 </version>
	public class SiteMapModelValidateVisitor : ISiteMapNodeVisitor
	{
        private const string DefaultSideMenuName = "Default";
		private SiteMapModel _model;
		private ValidateResult _result;
		private ResourceManager _resources;

		/// <summary>
		/// Instantiate an instance of SiteMapModelValidateVisitor class
		/// </summary>
		/// <param name="model">The site map model being validated.</param>
		public SiteMapModelValidateVisitor(SiteMapModel model)
		{
			_model = model;
			_result = new ValidateResult();
			_resources = new ResourceManager(this.GetType());
		}

		/// <summary>
		/// Gets the validate result.
		/// </summary>
		/// <value>The validate result in ValidateResult object</value>
		public ValidateResult ValidateResult
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Get a localized error message for the given message id
		/// </summary>
		/// <param name="msgId">The message id</param>
		/// <returns>The localized error message</returns>
		public string GetMessage(string msgId)
		{
			return _resources.GetString(msgId);
		}

        /// <summary>
        /// Viste a site map model.
        /// </summary>
        /// <param name="node">A SiteMapModel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSiteMapModel(SiteMapModel node)
        {
            ValidateResultEntry entry;

            if (string.IsNullOrEmpty(node.SideMenuFile))
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMapFile.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            if (string.IsNullOrEmpty(node.SideMenuFile))
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMenuFile.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            if (string.IsNullOrEmpty(node.CustomCommandSetFile))
            {
                entry = new ValidateResultEntry(_resources.GetString("CustomCommandSetFile.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            if (string.IsNullOrEmpty(node.PolicyFile))
            {
                entry = new ValidateResultEntry(_resources.GetString("PolicyFile.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            if (string.IsNullOrEmpty(node.Database))
            {
                entry = new ValidateResultEntry(_resources.GetString("DataBase.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            if (node.SiteMap == null)
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMap.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            /*
            if (node.SideMenu == null)
            {
                entry = new ValidateResultEntry(_resources.GetString("SideMenu.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }
            */

            return true;
        }

        /// <summary>
        /// Viste a site map.
        /// </summary>
        /// <param name="node">A SiteMap instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSiteMap(SiteMap node)
        {
            ValidateResultEntry entry;

            if (node.ChildNodes.Count == 0)
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMap.Empty"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            if ( HasChildrenNodesWithSameName(node))
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMapNode.SameChildTitle"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

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
            ValidateResultEntry entry;

            if (node.IsMainMenuItem)
            {
                /*
                if (node.ChildNodes.Count == 0)
                {
                    entry = new ValidateResultEntry(_resources.GetString("SiteMapNode.NoChildren"), GetSource(node), EntryType.Error, node);
                    _result.AddError(entry);
                }
                */

                if ( HasChildrenNodesWithSameName(node))
                {
                    entry = new ValidateResultEntry(_resources.GetString("SiteMapNode.SameChildTitle"), GetSource(node), EntryType.Error, node);
                    _result.AddError(entry);

                    return false;
                }
            }

            if (string.IsNullOrEmpty(node.Url) && node.ChildNodes.Count == 0)
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMapNode.MissingURL"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);
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
            ValidateResultEntry entry;

            if (string.IsNullOrEmpty(node.NavigationUrl))
            {
                entry = new ValidateResultEntry(_resources.GetString("MenuItem.MissNavigationURL"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);
            }

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
            ValidateResultEntry entry;

            if (node.ChildNodes.Count == 0)
            {
                entry = new ValidateResultEntry(_resources.GetString("CustomCommandGroup.NoChildren"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);
            }

            if ( HasChildrenNodesWithSameName(node))
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMapNode.SameChildTitle"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);

                return false;
            }

            if (string.IsNullOrEmpty(node.Database))
            {
                entry = new ValidateResultEntry(_resources.GetString("CustomCommandGroup.MissDatabase"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);
            }

            if (string.IsNullOrEmpty(node.ClassName))
            {
                entry = new ValidateResultEntry(_resources.GetString("CustomCommandGroup.MissClassName"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);
            }

            return true;
        }

        /// <summary>
        /// Viste a SiteMapModelSet node.
        /// </summary>
        /// <param name="node">A SiteMapModelSet instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSiteMapModelSet(SiteMapModelSet node)
        {
            ValidateResultEntry entry;

            if (node.ChildNodes.Count <= 0)
            {
                entry = new ValidateResultEntry(_resources.GetString("SiteMapModelSet.AtLeastOneSiteMap"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);
            }

            return true;
        }

        /// <summary>
        /// Viste a CustomCommand node.
        /// </summary>
        /// <param name="node">A CustomCommand instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitCustomCommand(CustomCommand node)
        {
            ValidateResultEntry entry;

            if (string.IsNullOrEmpty(node.NavigationUrl))
            {
                entry = new ValidateResultEntry(_resources.GetString("CustomCommand.MissNavigationURL"), GetSource(node), EntryType.Error, node);
                _result.AddError(entry);
            }

            return true;
        }

        /// <summary>
        /// Gets the source string
        /// </summary>
        /// <param name="node">The ISiteMapNode instance</param>
        /// <returns>A source string</returns>
        public string GetSource(ISiteMapNode node)
        {
            if (_resources == null)
            {
                _resources = new ResourceManager(this.GetType());
            }

            string source = "";

            if (node is SiteMap)
            {
                source = _resources.GetString("SiteMap.Text") + "=>" + node.Title;
            }
            else if (node is SideMenu)
            {
                source = _resources.GetString("SideMenu.Text") + "=>" + node.Title;
            }
            else if (node is SiteMapNode)
            {
                source = _resources.GetString("SiteMapNode.Text") + "=>" + node.Title;
            }
            else if (node is SideMenuGroup)
            {
                source = _resources.GetString("SideMenuGroup.Text") + "=>" + node.Title;
            }
            else if (node is Menu)
            {
                source = _resources.GetString("Menu.Text") + "=>" + node.Title;
            }
            else if (node is MenuItem)
            {
                source = _resources.GetString("MenuItem.Text") + "=>" + node.Title;
            }
            else if (node is CustomCommandGroup)
            {
                source = _resources.GetString("CustomCommandGroup.Text") + "=>" + node.Title;
            }
            else if (node is CustomCommand)
            {
                source = _resources.GetString("CustomCommand.Text") + "=>" + node.Title;
            }

            return source;
        }

        private bool  HasChildrenNodesWithSameName(ISiteMapNode parent)
        {
            bool status = false;
            Hashtable nameTable = new Hashtable();

            foreach (ISiteMapNode child in parent.ChildNodes)
            {
                if (nameTable[child.Name] != null)
                {
                    status = true;
                    break;
                }
                else
                {
                    nameTable[child.Name] = child.Name;
                }
            }

            return status;
        }
	}
}
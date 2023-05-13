using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.SiteMap;
using Newtera.Common.MetaData.SiteMap.Validate;
using Newtera.Common.MetaData.XaclModel;
using Newtera.WinClientCommon;

namespace Newtera.SiteMapStudio
{
    public partial class SiteMapStudioApp : Form
    {
        private MenuItemStates _menuItemStates;

        public SiteMapStudioApp()
        {
            InitializeComponent();

            _menuItemStates = new MenuItemStates();

            _menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

            InitializeMenuItemStates();

            // set windows client flag
            GlobalSettings.Instance.IsWindowClient = true;
        }

        /// <summary>
        /// Gets the menu item states
        /// </summary>
        public MenuItemStates MenuItemStates
        {
            get
            {
                return this._menuItemStates;
            }
        }

        /// <summary>
        /// Get the site maps from server and display at treeview 
        /// </summary>
        private void OpenSiteMaps()
        {
            // only system administrator is allowed to open a site map
            AdminLoginDialog dialog = new AdminLoginDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MetaDataServiceStub webService = new MetaDataServiceStub();
                string siteMapModelsXml = webService.GetSiteMapModels();
                // build site map model set
                SiteMapModelSet siteMapModelSet = new SiteMapModelSet();
                StringReader reader = new StringReader(siteMapModelsXml);
                siteMapModelSet.Read(reader);

                SiteMapModelManager.Instance.ModelSet = siteMapModelSet; // keep in the singleton

                TreeNode root = CreateTreeNode(siteMapModelSet);
                // build tree node for each site map model
                foreach (SiteMapModel siteMapModel in siteMapModelSet.ChildNodes)
                {
                    SiteMapModelManager.Instance.SelectedSiteMapModel = siteMapModel;

                    string siteMapXml = webService.GetSiteMap(siteMapModel.Name);
                    string sideMenuXml = webService.GetSideMenu(siteMapModel.Name);
                    string customCommandSetXml = webService.GetCustomCommandSet(siteMapModel.Name);
                    string xaclPolicyXml = webService.GetSiteMapAccessPolicy(siteMapModel.Name);

                    TreeNode siteMapRoot = BuildSiteMapTree(siteMapModel, siteMapXml, sideMenuXml, customCommandSetXml, xaclPolicyXml);

                    root.Nodes.Add(siteMapRoot);
                }

                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(root);
                // Make the deafult sitemap selected initially
                treeView1.SelectedNode = root.Nodes[0];
                treeView1.EndUpdate();

                _menuItemStates.SetState(MenuItemID.FileSave, true);
            }
        }

        /// <summary>
        /// Builds a tree to show nodes associated with a sitemap
        /// </summary>
        /// <param name="siteMapModel">SiteMap model object</param>
        /// <param name="siteMapXml">SiteMap xml</param>
        /// <param name="sideMenuXml">SideMenu xml</param>
        /// <param name="customCommandSetXml">Custom Command Set xml</param>
        /// <param name="xaclPolicyXml">XaclPolidy xml</param>
        /// <returns>The root of the tree</returns>
        private TreeNode BuildSiteMapTree(SiteMapModel siteMapModel, string siteMapXml, string sideMenuXml, string customCommandSetXml, string xaclPolicyXml)
        {
            TreeNode root = BuildSiteMapTreeFrame();
            StringReader reader;

            // build site map node
            SiteMap siteMap = new SiteMap();
            if (!string.IsNullOrEmpty(siteMapXml))
            {
                reader = new StringReader(siteMapXml);
                siteMap.Read(reader);
            }

            siteMapModel.SiteMap = siteMap; // keep in the model

            TreeNode siteMapRootNode = root.Nodes[0];

            // display site map nodes without displaying the root node
            AddTreeChildrenNodes(siteMapRootNode, siteMap);

            // display side menu nodes
            SideMenu sideMenu = new SideMenu();
            if (!string.IsNullOrEmpty(sideMenuXml))
            {
                reader = new StringReader(sideMenuXml);
                sideMenu.Read(reader);
            }

            siteMapModel.SideMenu = sideMenu; // keep in the singleton

            TreeNode sideMenuRootNode = root.Nodes[1];

            // display side menu nodes without displaying the root node
            AddTreeChildrenNodes(sideMenuRootNode, sideMenu);

            // display custom command nodes
            CustomCommandSet customCommandSet = new CustomCommandSet();
            if (!string.IsNullOrEmpty(customCommandSetXml))
            {
                reader = new StringReader(customCommandSetXml);
                customCommandSet.Read(reader);
            }

            siteMapModel.CustomCommandSet = customCommandSet; // keep in the singleton

            TreeNode customCommandRootNode = root.Nodes[1];

            // display add custom command nodes without displaying the root node
            AddTreeChildrenNodes(customCommandRootNode, customCommandSet);

            XaclPolicy policy = new XaclPolicy();
            siteMapModel.Policy = policy;
            if (!string.IsNullOrEmpty(xaclPolicyXml))
            {
                reader = new StringReader(xaclPolicyXml);
                policy.Read(reader);
            }
            else
            {
                SetDefaultPermissions(siteMapModel);
            }


            return root;
        }

        /// <summary>
        /// Build a tree frame that represents a sitemap nodes
        /// </summary>
        /// <returns>The root of the tree</returns>
        private SiteMapTreeNode BuildSiteMapTreeFrame()
        {
            SiteMapTreeNode root = CreateTreeNode(SiteMapModelManager.Instance.SelectedSiteMapModel);

            SiteMapTreeNode siteMapFolder = CreateTreeNode(TreeNodeType.SiteMapFolder);
            siteMapFolder.Expand();
            root.Nodes.Add(siteMapFolder);

            /*
            SiteMapTreeNode sideMenuFolder = CreateTreeNode(TreeNodeType.SideMenuFolder);
            sideMenuFolder.Expand();
            root.Nodes.Add(sideMenuFolder);
            */

            SiteMapTreeNode customCommandFolder = CreateTreeNode(TreeNodeType.CustomCommandFolder);
            customCommandFolder.Expand();
            root.Nodes.Add(customCommandFolder);

            return root;
        }

        /// <summary>
        /// Add a child nodes to the tree recursively
        /// </summary>
        /// <param name="parentNode">The parent tree node.</param>
        /// <returns>The created tree node</returns>
        private void AddTreeChildrenNodes(TreeNode parentTreeNode, ISiteMapNode parentSiteMapNode)
        {
            SiteMapTreeNode childTreeNode;
            foreach (ISiteMapNode child in parentSiteMapNode.ChildNodes)
            {
                if (child.NodeType != Common.MetaData.SiteMap.NodeType.Parameter)
                {
                    // parameter nodes are special nodes
                    childTreeNode = CreateTreeNode(child);
                    parentTreeNode.Nodes.Add(childTreeNode);

                    AddTreeChildrenNodes(childTreeNode, child);
                }
            }
        }

        /// <summary>
        /// Create a SiteMapTreeNode
        /// </summary>
        /// <param name="siteMapNode">The node.</param>
        /// <returns>The created tree node</returns>
        private SiteMapTreeNode CreateTreeNode(TreeNodeType treeNodeType)
        {
            SiteMapTreeNode treeNode = new SiteMapTreeNode(treeNodeType);

            SetTreeNodeProperties(treeNode);

            return treeNode;
        }

        /// <summary>
        /// Create a SiteMapTreeNode
        /// </summary>
        /// <param name="siteMapNode">The node.</param>
        /// <returns>The created tree node</returns>
        private SiteMapTreeNode CreateTreeNode(ISiteMapNode siteMapNode)
        {
            SiteMapTreeNode treeNode = new SiteMapTreeNode(siteMapNode);
            SetTreeNodeProperties(treeNode);

            return treeNode;
        }

        /// <summary>
        /// Set other properties of the tree node, such as imageindex
        /// </summary>
        /// <param name="treeNode"></param>
        private void SetTreeNodeProperties(SiteMapTreeNode treeNode)
        {
            switch (treeNode.Type)
            {
                case TreeNodeType.SiteMapSetNode:
                    treeNode.Text = MessageResourceManager.GetString("SiteMapStudioApp.SiteMapSetFolder");
                    treeNode.ImageIndex = 1;
                    treeNode.SelectedImageIndex = 2;
                    break;
                case TreeNodeType.SiteMapRootNode:
                    //treeNode.Text = MessageResourceManager.GetString("SiteMapStudioApp.SiteMapRootNode");
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 0;
                    break;
                case TreeNodeType.SiteMapFolder:
                    treeNode.Text = MessageResourceManager.GetString("SiteMapStudioApp.SiteMapFolder");
                    treeNode.ImageIndex = 1;
                    treeNode.SelectedImageIndex = 2;
                    break;
                case TreeNodeType.SideMenuFolder:
                    treeNode.Text = MessageResourceManager.GetString("SiteMapStudioApp.SideMenuFolder");
                    treeNode.ImageIndex = 1;
                    treeNode.SelectedImageIndex = 2;
                    break;
                case TreeNodeType.SiteMapMenu:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    if (((SiteMapNode)treeNode.SiteMapNode).IsMainMenuItem)
                    {
                        treeNode.ImageIndex = 3;
                        treeNode.SelectedImageIndex = 3;
                    }
                    else
                    {
                        treeNode.ImageIndex = 4;
                        treeNode.SelectedImageIndex = 4;
                    }

                    break;
                case TreeNodeType.SideMenuGroup:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 3;
                    treeNode.SelectedImageIndex = 3;
                    break;
                case TreeNodeType.SideMenuSearch:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                    break;
                case TreeNodeType.SideMenuDashboard:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                    break;
                case TreeNodeType.SideMenuTrees:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                    break;
                case TreeNodeType.SideMenuTree:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                    break;
                case TreeNodeType.SideMenuActions:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                    break;
                case TreeNodeType.SideMenuItem:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                    break;
                case TreeNodeType.CustomCommandFolder:
                    treeNode.Text = MessageResourceManager.GetString("SiteMapStudioApp.CustomCommandFolder");
                    treeNode.ImageIndex = 1;
                    treeNode.SelectedImageIndex = 2;
                    break;
                case TreeNodeType.CustomCommandGroup:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 3;
                    treeNode.SelectedImageIndex = 3;
                    break;
                case TreeNodeType.CustomCommand:
                    treeNode.Text = treeNode.SiteMapNode.Title;
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                    break;
            }
        }

        private void ShowSelectionStatus(string text)
        {
            this.toolStripStatusLabel1.Text = text;
        }

        private void SaveSiteMaps()
        {
            // validate all the model first, if the model has validate errors,
            // stop saving it to server.
            ValidateResult result = SiteMapModelManager.Instance.ModelSet.Validate();

            if (result.HasError)
            {
                if (MessageBox.Show(MessageResourceManager.GetString("SiteMapStudio.ValidatingError"),
                    "Error Dialog", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error) == DialogResult.OK)
                {
                    ShowValidateErrorDialog(result);
                }

                return;
            }

            if (MessageBox.Show(MessageResourceManager.GetString("SiteMapStudio.ConfirmSave"),
                "Confirm Dialog", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    MetaDataServiceStub webService = new MetaDataServiceStub();

                    SiteMapModelSet siteMapModelSet = SiteMapModelManager.Instance.ModelSet;
                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);
                    siteMapModelSet.Write(writer);
                    string siteMapModelSetXml = builder.ToString();
                    webService.SetSiteMapModels(siteMapModelSetXml);

                    foreach (SiteMapModel siteMapModel in siteMapModelSet.ChildNodes)
                    {
                        SiteMap siteMap = siteMapModel.SiteMap;
                        builder = new StringBuilder();
                        writer = new StringWriter(builder);
                        siteMap.Write(writer);
                        string siteMapXml = builder.ToString();

                        SideMenu sideMenu = siteMapModel.SideMenu;
                        builder = new StringBuilder();
                        writer = new StringWriter(builder);
                        sideMenu.Write(writer);
                        string sideMenuXml = builder.ToString();

                        CustomCommandSet customCommandSet = siteMapModel.CustomCommandSet;
                        builder = new StringBuilder();
                        writer = new StringWriter(builder);
                        customCommandSet.Write(writer);
                        string customCommandXml = builder.ToString();

                        XaclPolicy policy = siteMapModel.Policy;
                        builder = new StringBuilder();
                        writer = new StringWriter(builder);
                        policy.Write(writer);
                        string policyXml = builder.ToString();

                        webService.SetSiteMap(siteMapModel.Name, siteMapXml);
                        webService.SetSideMenu(siteMapModel.Name, sideMenuXml);
                        webService.SetCustomCommandSet(siteMapModel.Name, customCommandXml);
                        webService.SetSiteMapAccessPolicy(siteMapModel.Name, policyXml);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Display the validating errors
        /// </summary>
        /// <param name="result">The result containing validating errors</param>
        private void ShowValidateErrorDialog(ValidateResult result)
        {
            string summary = result.Errors.Count + " Errors; ";

            ValidateErrorDialog dialog = new ValidateErrorDialog();
            dialog.Entries = result.Errors;
            dialog.Show();
        }

        /// <summary>
        /// event handler for menu state change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemStateChanged(object sender, System.EventArgs e)
        {
            StateChangedEventArgs args = (StateChangedEventArgs)e;

            // set the toolbar button states
            switch (args.ID)
            {
                case MenuItemID.FileOpen:
                    this.openToolStripButton.Enabled = args.State;
                    this.openToolStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.FileSave:
                    this.saveToolStripButton.Enabled = args.State;
                    this.saveToolStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditAdd:
                    this.addToolStripButton.Enabled = args.State;
                    this.addContextMenuStripItem.Enabled = args.State;
                    this.addToolStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditDelete:
                    this.deleteToolStripButton.Enabled = args.State;
                    this.deleteToolStripMenuItem.Enabled = args.State;
                    this.deleteContextMenuStripItem.Enabled = args.State;
                    break;
                case MenuItemID.EditUp:
                    this.upContextMenuItem.Enabled = args.State;
                    this.upToolStripMenuItem.Enabled = args.State;
                    this.uptoolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.EditDown:
                    this.downContextMenuItem.Enabled = args.State;
                    this.downToolStripMenuItem.Enabled = args.State;
                    this.downToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.EditRefresh:
                    this.refreshContextMenuItem.Enabled = args.State;
                    this.refreshToolStripButton.Enabled = args.State;
                    this.refreshToolStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditOptions:
                    this.optionsToolStripMenuItem.Enabled = args.State;
                    break;
            }
        }

        /// <summary>
        /// Set the menu item states based on the selected tree node
        /// </summary>
        /// <param name="selectedNode"></param>
        private void SetMenuItemStates(SiteMapTreeNode selectedNode)
        {
            switch (selectedNode.Type)
            {
                case TreeNodeType.SiteMapSetNode:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SiteMapRootNode:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    if (((SiteMapModel)selectedNode.SiteMapNode).AllowDelete)
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, true);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, true);

                    break;
                case TreeNodeType.SiteMapFolder:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuFolder:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SiteMapMenu:
                    SiteMapNode siteMapNode = (SiteMapNode)selectedNode.SiteMapNode;
                    if (siteMapNode.IsMainMenuItem)
                    {
                        this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                        this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                        this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    }

                    if (IsFirstItemInCollection(siteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(siteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuGroup:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuSearch:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);

                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuDashboard:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);

                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuTrees:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuTree:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuActions:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.SideMenuItem:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;

                case TreeNodeType.CustomCommandFolder:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;

                case TreeNodeType.CustomCommandGroup:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
                case TreeNodeType.CustomCommand:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    if (IsFirstItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditUp, true);
                    }

                    if (IsLastItemInCollection(selectedNode.SiteMapNode))
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, false);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditDown, true);
                    }
                    this._menuItemStates.SetState(MenuItemID.EditRefresh, false);
                    this._menuItemStates.SetState(MenuItemID.EditOptions, false);

                    break;
            }
        }

        /// <summary>
        /// Initialize the menu item states
        /// </summary>
        private void InitializeMenuItemStates()
        {
            _menuItemStates.SetState(MenuItemID.FileSave, false);
            _menuItemStates.SetState(MenuItemID.EditAdd, false);
            _menuItemStates.SetState(MenuItemID.EditDelete, false);
            _menuItemStates.SetState(MenuItemID.EditUp, false);
            _menuItemStates.SetState(MenuItemID.EditDown, false);
            _menuItemStates.SetState(MenuItemID.EditRefresh, false);
            _menuItemStates.SetState(MenuItemID.EditOptions, false);
        }

        /// <summary>
        /// Add an item as a child to the selected tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewItem()
        {
            // Create an instance of AddNewItemDialog
            AddNewItemDialog dialog = new AddNewItemDialog();

            // Set the currently selected tree node
            dialog.SelectedTreeNode = (SiteMapTreeNode)this.treeView1.SelectedNode;

            // Display the dialog
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddToModel(dialog.SelectedTreeNode, dialog.NewSiteMapNode);

                // Add the new node to the tree view
                SiteMapTreeNode newTreeNode;
                if (dialog.NewSiteMapNode is SiteMapModel)
                {
                    SiteMapModelManager.Instance.SelectedSiteMapModel = (SiteMapModel)dialog.NewSiteMapNode;

                    newTreeNode = BuildSiteMapTreeFrame();

                }
                else
                {
                    newTreeNode = CreateTreeNode(dialog.NewSiteMapNode);
                }

                dialog.SelectedTreeNode.Nodes.Add(newTreeNode);

            }
        }

        private void AddToModel(SiteMapTreeNode selectedTreeNode, ISiteMapNode newNode)
        {
            switch (selectedTreeNode.Type)
            {
                case TreeNodeType.SiteMapSetNode:
                    SiteMapModelManager.Instance.ModelSet.AddChildNode(newNode);

                    break;
                case TreeNodeType.SiteMapFolder:
                    SiteMapModelManager.Instance.SelectedSiteMapModel.SiteMap.AddChildNode(newNode);
                    break;

                case TreeNodeType.SiteMapMenu:
                    selectedTreeNode.SiteMapNode.AddChildNode(newNode);

                    break;

                case TreeNodeType.SideMenuFolder:
                    SiteMapModelManager.Instance.SelectedSiteMapModel.SideMenu.AddChildNode(newNode);

                    break;
                case TreeNodeType.SideMenuGroup:
                    selectedTreeNode.SiteMapNode.AddChildNode(newNode);

                    break;

                case TreeNodeType.SideMenuActions:
                    selectedTreeNode.SiteMapNode.AddChildNode(newNode);

                    break;

                case TreeNodeType.CustomCommandFolder:
                    SiteMapModelManager.Instance.SelectedSiteMapModel.CustomCommandSet.AddChildNode(newNode);

                    break;

                case TreeNodeType.CustomCommandGroup:
                    selectedTreeNode.SiteMapNode.AddChildNode(newNode);

                    break;
            }
        }

        /// <summary>
        /// Delete an item which is currently being selected
        /// </summary>
        private void DeleteItem()
        {
            SiteMapTreeNode treeNode = null;

            if (MessageBox.Show(MessageResourceManager.GetString("SiteMapStudio.DeleteItem"),
                "Confirm Dialog", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // delete the node selected in the tree view
                    treeNode = (SiteMapTreeNode)this.treeView1.SelectedNode;

                    // delete the corresponding node from the model
                    DeleteSiteMapNode(treeNode);

                    this.treeView1.BeginUpdate();

                    TreeNode parent = treeNode.Parent;

                    // clear the selection to avoid AfterSelect event of tree view to be
                    // fired when removing the currently selected tree node
                    this.treeView1.SelectedNode = null;

                    parent.Nodes.Remove(treeNode);

                    // switch the selection to its parent
                    this.treeView1.SelectedNode = parent;

                    this.treeView1.EndUpdate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Delete a node from the model.
        /// </summary>
        /// <param name="treeNode">The tree node that contains the site map node.</param>
        private void DeleteSiteMapNode(SiteMapTreeNode treeNode)
        {
            ISiteMapNode siteMapNode = (ISiteMapNode)treeNode.SiteMapNode;

            if (siteMapNode.ParentNode != null)
            {
                siteMapNode.ParentNode.DeleteChildNode(siteMapNode);
            }
        }

        private bool IsFirstItemInCollection(ISiteMapNode node)
        {
            bool status = true;

            if (node.ParentNode != null &&
                node.ParentNode.ChildNodes != null &&
                node.ParentNode.ChildNodes.Count > 1)
            {
                int index = 0;
                foreach (ISiteMapNode child in node.ParentNode.ChildNodes)
                {
                    if (child == node)
                    {
                        break;
                    }
                    index++;
                }

                if (index > 0)
                {
                    status = false;
                }
            }
            return status;
        }

        private bool IsLastItemInCollection(ISiteMapNode node)
        {
            bool status = true;

            if (node.ParentNode != null &&
                node.ParentNode.ChildNodes != null &&
                node.ParentNode.ChildNodes.Count > 1)
            {
                int index = 0;
                foreach (ISiteMapNode child in node.ParentNode.ChildNodes)
                {
                    if (child == node)
                    {
                        break;
                    }
                    index++;
                }

                if (index < (node.ParentNode.ChildNodes.Count - 1))
                {
                    status = false;
                }
            }
            return status;
        }

        private void MoveUp()
        {
            SiteMapTreeNode treeNode = (SiteMapTreeNode) this.treeView1.SelectedNode;
            TreeNode treeNodeParent = treeNode.Parent;
            ISiteMapNode modelNode = treeNode.SiteMapNode;

            SiteMapNodeCollection modelNodes = modelNode.ParentNode.ChildNodes;
            int pos = modelNodes.IndexOf(modelNode);
            if (pos > 0)
            {
                modelNodes.RemoveAt(pos);
                modelNodes.Insert(pos - 1, modelNode);

                treeNodeParent.Nodes.RemoveAt(pos);
                treeNodeParent.Nodes.Insert(pos - 1, treeNode);

                this.treeView1.SelectedNode = treeNode;
            }
        }

        private void MoveDown()
        {
            SiteMapTreeNode treeNode = (SiteMapTreeNode)this.treeView1.SelectedNode;
            TreeNode treeNodeParent = treeNode.Parent;
            ISiteMapNode modelNode = treeNode.SiteMapNode;

            SiteMapNodeCollection modelNodes = modelNode.ParentNode.ChildNodes;
            int pos = modelNodes.IndexOf(modelNode);
            if (pos >= 0)
            {
                modelNodes.RemoveAt(pos);
                modelNodes.Insert(pos + 1, modelNode);

                treeNodeParent.Nodes.RemoveAt(pos);
                treeNodeParent.Nodes.Insert(pos + 1, treeNode);

                this.treeView1.SelectedNode = treeNode;
            }
        }

        private void AddXaclRule(string user, string[] roles)
        {
            IXaclObject xaclItem = this.propertyGrid1.SelectedObject as IXaclObject;

            if (xaclItem != null)
            {
                XaclObject obj = new XaclObject(xaclItem.ToXPath());

                XaclSubject subject = new XaclSubject();
                if (user != null)
                {
                    subject.Uid = user;
                }
                else if (roles != null)
                {
                    foreach (string role in roles)
                    {
                        subject.AddRole(role);
                    }
                }

                XaclRule rule = new XaclRule(subject, obj.Href);
                if (!SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.IsRuleExist(obj, rule))
                {
                    SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.AddRule(obj, rule);
                    XaclRuleListViewItem listViewItem = new XaclRuleListViewItem(obj, rule);
                    listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                    listViewItem.Selected = true;
                    this.ruleListView.Items.Add(listViewItem);
                }
                else if (!SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.IsLocalRuleExist(obj, rule))
                {
                    // there is a propagated rule with the same subject exist,
                    // ask user if he/she wants to override it
                    if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRuleExists"),
                        "Confirm Dialog", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        // remove the propagated rule from display in the list view
                        XaclRule propagatedRule = SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.GetPropagatedRule(obj, rule);
                        foreach (XaclRuleListViewItem item in this.ruleListView.Items)
                        {
                            if (item.Rule == propagatedRule)
                            {
                                this.ruleListView.Items.Remove(item);
                                break;
                            }
                        }

                        // add the overrided rule
                        rule.IsOverrided = true;
                        SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.AddRule(obj, rule);
                        XaclRuleListViewItem listViewItem = new XaclRuleListViewItem(obj, rule);
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                        listViewItem.Selected = true;
                        this.ruleListView.Items.Add(listViewItem);
                    }
                    else
                    {
                        // add as a local rule
                        SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.AddRule(obj, rule);
                        XaclRuleListViewItem listViewItem = new XaclRuleListViewItem(obj, rule);
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                        listViewItem.Selected = true;
                        this.ruleListView.Items.Add(listViewItem);
                    }
                }
                else
                {
                    MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRuleExists"),
                        "Information Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Remove an XaclRule from currently displayed rules
        /// </summary>
        /// <param name="listViewItem">The list view item for the rule</param>
        private void RemoveXaclRule(XaclRuleListViewItem listViewItem)
        {
            SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.RemoveRule(listViewItem.Object, listViewItem.Rule);

            // refresh the rules in case there is a propagated rule
            this.ShowXaclRules((ISiteMapNode)this.propertyGrid1.SelectedObject);
        }

        /// <summary>
        /// Show the xacl rules associated with a ISiteMapNode object
        /// </summary>
        /// <param name="siteMapNode">ISiteMapNode object</param>
        private void ShowXaclRules(ISiteMapNode siteMapNode)
        {
            IXaclObject xaclItem = siteMapNode as IXaclObject;

            this.ruleListView.SuspendLayout();
            this.ruleListView.Items.Clear();

            InitializeXaclRuleControls();

            if (xaclItem != null && !(siteMapNode is SiteMapModelSet))
            {
                XaclObject obj = new XaclObject(xaclItem.ToXPath());

                XaclRuleListViewItem listViewItem;
                XaclRuleCollection rules = SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.GetRules(obj);
                foreach (XaclRule rule in rules)
                {
                    listViewItem = new XaclRuleListViewItem(obj, rule);
                    if (SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.IsLocalRuleExist(obj, rule))
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                    }
                    else
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRule"));
                    }
                    this.ruleListView.Items.Add(listViewItem);
                }

                this.addButton.Enabled = true;
            }

            this.ruleListView.ResumeLayout();
        }

        /// <summary>
        /// if the selected node is a SiteMapModel object, set it as the current sitemap model in
        /// the singleton.
        /// </summary>
        /// <param name="siteMapNode">ISiteMapNode object</param>
        private void SetCurrentSiteMapModel(SiteMapTreeNode treeNode)
        {
            SiteMapTreeNode theTreeNode = treeNode;
            ISiteMapNode siteMapNode;

            while (true)
            {
                siteMapNode = theTreeNode.SiteMapNode;

                if (siteMapNode != null && siteMapNode is SiteMapModel)
                {
                    SiteMapModelManager.Instance.SelectedSiteMapModel = (SiteMapModel)siteMapNode;
                    break;
                }
                else
                {
                    theTreeNode = theTreeNode.Parent as SiteMapTreeNode;
                    if (theTreeNode == null)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the states of xacl rule related controls
        /// </summary>
        private void InitializeXaclRuleControls()
        {
            this.addButton.Enabled = false;
            this.deleteButton.Enabled = false;
            this.grantReadRadioButton.DataBindings.Clear();
            this.grantReadRadioButton.Checked = false;
            this.grantReadRadioButton.Enabled = false;
            this.denyReadRadioButton.DataBindings.Clear();
            this.denyReadRadioButton.Checked = false;
            this.denyReadRadioButton.Enabled = false;
        }

        /// <summary>
        /// Display and bind the detail of a rule to UI controls
        /// </summary>
        /// <param name="rule">The rule</param>
        private void ShowXaclRuleDetail(XaclRule rule)
        {
            XaclActionCollection actions = rule.Actions;
            string href = ((IXaclObject)this.propertyGrid1.SelectedObject).ToXPath();
            bool isLocalRule;
            if (rule.ObjectHref != href)
            {
                isLocalRule = false;
                this.deleteButton.Enabled = false;
            }
            else
            {
                isLocalRule = true;
                this.deleteButton.Enabled = true;
            }

            foreach (XaclAction action in actions)
            {
                switch (action.ActionType)
                {
                    case XaclActionType.Read:
                        this.grantReadRadioButton.DataBindings.Clear();
                        this.grantReadRadioButton.Enabled = isLocalRule;
                        this.grantReadRadioButton.DataBindings.Add("Checked", rule, "IsReadGranted");
                        this.denyReadRadioButton.DataBindings.Clear();
                        this.denyReadRadioButton.Enabled = isLocalRule;
                        this.denyReadRadioButton.DataBindings.Add("Checked", rule, "IsReadDenied");
                        break;
                }
            }
        }

        /// <summary>
        /// Create default rules at model level, which will be inherited by all nodes
        /// </summary>
        private void SetDefaultPermissions(SiteMapModel siteMapModel)
        {
            // create a rule to allow EveryOne to have full access to meta data model
            XaclObject obj = new XaclObject(siteMapModel.ToXPath());
            XaclSubject subject = new XaclSubject();
            subject.AddRole(XaclSubject.EveryOne);
            XaclRule rule = new XaclRule(subject);
            siteMapModel.Policy.AddRule(obj, rule);
        }

        /// <summary>
        /// Refresh the currently selected SiteMapModel content from the server
        /// </summary>
        private void RefreshSiteMapModel()
        {
            MetaDataServiceStub webService = new MetaDataServiceStub();

            TreeNode root = treeView1.SelectedNode.Parent; // the selected node is for SiteMapModel object

            // build tree node for the selected SiteMapModel object
            SiteMapModel siteMapModel = SiteMapModelManager.Instance.SelectedSiteMapModel;

            string siteMapXml = webService.GetSiteMapFromFile(siteMapModel.SiteMapFile);
            string sideMenuXml = webService.GetSideMenuFromFile(siteMapModel.SideMenuFile);
            string customCommandSetXml = webService.GetCustomCommandSetFromFile(siteMapModel.CustomCommandSetFile);
            string xaclPolicyXml = webService.GetSiteMapAccessPolicyFromFile(siteMapModel.PolicyFile);

            TreeNode siteMapRoot = BuildSiteMapTree(siteMapModel, siteMapXml, sideMenuXml, customCommandSetXml, xaclPolicyXml);

            // replace the sitemap model tree node with newly create tree node
            int index = 0;
            foreach (SiteMapTreeNode childNode in root.Nodes)
            {
                if (childNode.SiteMapNode == siteMapModel)
                {
                    break;
                }

                index++;
            }

            treeView1.BeginUpdate();
            root.Nodes.RemoveAt(index);
            root.Nodes.Insert(index, siteMapRoot);
            // Make the refreshed sitemap model selected initially
            treeView1.SelectedNode = siteMapRoot;
            treeView1.EndUpdate();
        }

        private void OpenPermissionDialog()
        {
            AccessControlOptionDialog dialog = new AccessControlOptionDialog();
            dialog.XaclPolicy = SiteMapModelManager.Instance.SelectedSiteMapModel.Policy;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SiteMapModelManager.Instance.SelectedSiteMapModel.Policy.Setting.ConflictResolutionType = dialog.ConflictResolution;
            }
        }

        #region event handlers

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSiteMaps();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSiteMaps();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewItem();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        private void aboutSiteMapStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog();

            aboutDialog.ShowDialog();
        }

        private void onlineDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpFile = AppDomain.CurrentDomain.BaseDirectory + MessageResourceManager.GetString("SiteMapStudioApp.OnlineHelp");
            Help.ShowHelp(this, helpFile);
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenSiteMaps();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveSiteMaps();
        }

        private void addToolStripButton_Click(object sender, EventArgs e)
        {
            AddNewItem();
        }

        private void deleteToolStripButton_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get the selected node
            SiteMapTreeNode node = (SiteMapTreeNode)e.Node;

            if (node != null)
            {
                this.propertyGrid1.SelectedObject = node.SiteMapNode;

                // set the current selected site map model
                SetCurrentSiteMapModel(node);

                // show the xacl rules for the ISiteMapNode
                ShowXaclRules(node.SiteMapNode);

                // set the menu item states
                SetMenuItemStates(node);

                // show caption of the selected object in the status bar
                string text = "";
                if (node.SiteMapNode != null)
                {
                    text = node.SiteMapNode.Title;
                }

                ShowSelectionStatus(text);
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }

        private void addContextMenuStripItem_Click(object sender, EventArgs e)
        {
            AddNewItem();
        }

        private void deleteContextMenuStripItem_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // add an xacl rule
            SelectUserOrRoleDialog dialog = new SelectUserOrRoleDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddXaclRule(dialog.SelectedUser, dialog.SelectedRoles);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // delete an xacl rule
            if (this.ruleListView.SelectedItems.Count == 1)
            {
                XaclRuleListViewItem selectedItem = (XaclRuleListViewItem)this.ruleListView.SelectedItems[0];

                RemoveXaclRule(selectedItem);

                this.deleteButton.Enabled = false;
            }	
        }

        private void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveUp();
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveDown();
        }

        private void uptoolStripButton_Click(object sender, EventArgs e)
        {
            MoveUp();
        }

        private void downToolStripButton_Click(object sender, EventArgs e)
        {
            MoveDown();
        }

        private void upContextMenuItem_Click(object sender, EventArgs e)
        {
            MoveUp();
        }

        private void downContextMenuItem_Click(object sender, EventArgs e)
        {
            MoveDown();
        }

        private void SiteMapStudioApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(MessageResourceManager.GetString("SiteMapStudio.Exit"),
                "Confirm Dialog", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void ruleListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ruleListView.SelectedItems.Count == 1)
            {
                XaclRuleListViewItem selectedItem = (XaclRuleListViewItem)this.ruleListView.SelectedItems[0];

                ShowXaclRuleDetail(selectedItem.Rule);
            }
        }

        private void permissionRadioButton_Click(object sender, EventArgs e)
        {
            this.permissionGroupBox.Focus();
            this.permissionGroupBox.Refresh();
        }

        private void descriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // make the help area visible if it is not, otherwise, invisible
            this.propertyGrid1.HelpVisible = !(this.descriptionToolStripMenuItem.Checked);
            this.descriptionToolStripMenuItem.Checked = !(this.descriptionToolStripMenuItem.Checked);
        }

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            RefreshSiteMapModel();
        }

        private void refreshContextMenuItem_Click(object sender, EventArgs e)
        {
            RefreshSiteMapModel();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshSiteMapModel();
        }

        private void accessControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPermissionDialog();
        }

        #endregion

    }
}
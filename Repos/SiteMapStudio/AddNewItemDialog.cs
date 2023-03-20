using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.SiteMap;

namespace Newtera.SiteMapStudio
{
    public partial class AddNewItemDialog : Form
    {
        private SiteMapTreeNode _selectedTreeNode = null;
        private ISiteMapNode _siteMapNode = null;
        private SiteMapListViewItem _selectedListViewItem = null;
        private ISiteMapNode _parentNode = null;

        public SiteMapTreeNode SelectedTreeNode
        {
            get
            {
                return _selectedTreeNode;
            }
            set
            {
                _selectedTreeNode = value;
            }
        }

        public ISiteMapNode NewSiteMapNode
        {
            get
            {
                return _siteMapNode;
            }
        }

        public AddNewItemDialog()
        {
            InitializeComponent();
        }

        private ISiteMapNode CreateItem()
        {
            ISiteMapNode item = null;

            switch (_selectedListViewItem.Type)
            {
                case TreeNodeType.SiteMapMenu:
                    item = new SiteMapNode();
                    break;

                case TreeNodeType.SideMenuGroup:
                    item = new SideMenuGroup();
                    break;

                case TreeNodeType.SideMenuSearch:
                    item = new Newtera.Common.MetaData.SiteMap.Menu();
                    ((Newtera.Common.MetaData.SiteMap.Menu)item).Type = MenuType.Keywords;
                    break;

                case TreeNodeType.SideMenuDashboard:
                    item = new Newtera.Common.MetaData.SiteMap.Menu();
                    ((Newtera.Common.MetaData.SiteMap.Menu)item).Type = MenuType.Dashboard;
                    break;

                case TreeNodeType.SideMenuTree:
                    item = new Newtera.Common.MetaData.SiteMap.Menu();
                    ((Newtera.Common.MetaData.SiteMap.Menu)item).Type = MenuType.Tree;
                    break;

                case TreeNodeType.SideMenuTrees:
                    item = new Newtera.Common.MetaData.SiteMap.Menu();
                    ((Newtera.Common.MetaData.SiteMap.Menu)item).Type = MenuType.Trees;
                    break;

                case TreeNodeType.SideMenuActions:
                    item = new Newtera.Common.MetaData.SiteMap.Menu();
                    ((Newtera.Common.MetaData.SiteMap.Menu)item).Type = MenuType.Actions;
                    break;

                case TreeNodeType.SideMenuItem:
                    item = new Newtera.Common.MetaData.SiteMap.MenuItem();
                    break;

                case TreeNodeType.CustomCommandGroup:
                    item = new CustomCommandGroup();
                    break;

                case TreeNodeType.CustomCommand:
                    item = new CustomCommand();
                    break;
                case TreeNodeType.SiteMapRootNode:
                    item = new SiteMapModel();
                    break;

            }

            if (item != null)
            {
                // for both name and title. title can be changed later
                item.Name = this.enterTextBox1.Text.Trim();
                item.Title = item.Name;
            }

            return item;
        }

        private bool ValidateNameUniqueness(string name)
        {
            bool status = true;

            if (_parentNode != null)
            {
                foreach (ISiteMapNode child in _parentNode.ChildNodes)
                {
                    if (child.Name == name)
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }

        #region event handlers

        private void okButton_Click(object sender, EventArgs e)
        {
            this.enterTextBox1.Focus();
            if (!this.Validate())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            this._siteMapNode = CreateItem();
        }

        private void AddNewItemDialog_Load(object sender, EventArgs e)
        {
            this.listView1.BeginUpdate();
            this.listView1.Items.Clear();

            SiteMapListViewItem item;
            if (SelectedTreeNode != null)
            {
                switch (SelectedTreeNode.Type)
                {
                    case TreeNodeType.SiteMapSetNode:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SiteMapModel.Text"), TreeNodeType.SiteMapRootNode);
                        item.ImageIndex = 1;
                        item.Selected = true;
                        this.listView1.Items.Add(item);
                        this._parentNode = SiteMapModelManager.Instance.ModelSet;
                        break;
                    case TreeNodeType.SiteMapFolder:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SiteMapMenu.Text"), TreeNodeType.SiteMapMenu);
                        item.ImageIndex = 3;
                        item.Selected = true;
                        this.listView1.Items.Add(item);
                        this._parentNode = SiteMapModelManager.Instance.SelectedSiteMapModel.SiteMap;
                        break;

                    case TreeNodeType.SiteMapMenu:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SiteMapMenu.Text"), TreeNodeType.SiteMapMenu);
                        item.ImageIndex = 4;
                        item.Selected = true;
                        this.listView1.Items.Add(item);
                        this._parentNode = _selectedTreeNode.SiteMapNode;
                        break;

                    case TreeNodeType.SideMenuFolder:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SideMenuGroup.Text"), TreeNodeType.SideMenuGroup);
                        item.ImageIndex = 3;
                        item.Selected = true;
                        this.listView1.Items.Add(item);
                        this._parentNode = SiteMapModelManager.Instance.SelectedSiteMapModel.SideMenu;

                        break;
                    case TreeNodeType.SideMenuGroup:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SideMenuSearch.Text"), TreeNodeType.SideMenuSearch);
                        item.ImageIndex = 4;
                        item.Selected = true;
                        this.listView1.Items.Add(item);

                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SideMenuDashboard.Text"), TreeNodeType.SideMenuDashboard);
                        item.ImageIndex = 4;
                        this.listView1.Items.Add(item);

                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SideMenuTree.Text"), TreeNodeType.SideMenuTree);
                        item.ImageIndex = 4;
                        this.listView1.Items.Add(item);

                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SideMenuTrees.Text"), TreeNodeType.SideMenuTrees);
                        item.ImageIndex = 4;
                        this.listView1.Items.Add(item);

                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SideMenuActions.Text"), TreeNodeType.SideMenuActions);
                        item.ImageIndex = 4;
                        this.listView1.Items.Add(item);

                        this._parentNode = _selectedTreeNode.SiteMapNode;

                        break;

                    case TreeNodeType.SideMenuActions:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("SideMenuItem.Text"), TreeNodeType.SideMenuItem);
                        item.ImageIndex = 4;
                        item.Selected = true;
                        this.listView1.Items.Add(item);

                        this._parentNode = _selectedTreeNode.SiteMapNode;

                        break;

                    case TreeNodeType.CustomCommandFolder:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("CustomCommandGroup.Text"), TreeNodeType.CustomCommandGroup);
                        item.ImageIndex = 3;
                        item.Selected = true;
                        this.listView1.Items.Add(item);
                        this._parentNode = SiteMapModelManager.Instance.SelectedSiteMapModel.CustomCommandSet;
                        break;

                    case TreeNodeType.CustomCommandGroup:
                        item = new SiteMapListViewItem(MessageResourceManager.GetString("CustomCommand.Text"), TreeNodeType.CustomCommand);
                        item.ImageIndex = 4;
                        item.Selected = true;
                        this.listView1.Items.Add(item);

                        this._parentNode = _selectedTreeNode.SiteMapNode;

                        break;
                }
            }

            this.listView1.EndUpdate();
        }

        private void enterTextBox1_Validating(object sender, CancelEventArgs e)
        {
            // the text box cannot be null
            if (this.enterTextBox1.Text.Length == 0)
            {
                e.Cancel = true;

                this.errorProvider.SetError(this.enterTextBox1, MessageResourceManager.GetString("SiteMapStudio.InvalidTitle"));
            }
            else if (!ValidateNameUniqueness(this.enterTextBox1.Text))
            {
                this.errorProvider.SetError(this.enterTextBox1, MessageResourceManager.GetString("SiteMapStudio.ExistName"));
                e.Cancel = true;
            }
            else
            {
                this.errorProvider.SetError((Control)sender, null);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                this._selectedListViewItem = (SiteMapListViewItem)this.listView1.SelectedItems[0];
            }
        }

        private void enterTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.enterTextBox1.Focus();
                if (!this.Validate())
                {
                    return;
                }

                this._siteMapNode = CreateItem();

                this.DialogResult = DialogResult.OK;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represent an item in the list view
    /// </summary>
    internal class SiteMapListViewItem : ListViewItem
    {
        private TreeNodeType _nodeType;

        public SiteMapListViewItem(string text, TreeNodeType nodeType)
            : base(text)
        {
            _nodeType = nodeType;
        }

        public TreeNodeType Type
        {
            get
            {
                return _nodeType;
            }
            set
            {
                _nodeType = value;
            }
        }
    }
}
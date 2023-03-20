using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;

namespace Newtera.Studio
{
    public partial class DefineAutoHierarchyDialog : Form
    {
        private AutoClassifyDef _autoClassifyDef;

        public DefineAutoHierarchyDialog(AutoClassifyDef autoClassifyDef)
        {
            InitializeComponent();
            _autoClassifyDef = autoClassifyDef;
        }

        /// <summary>
        /// Gets or sets the display caption of the node which will be the root of
        /// generated hierarchy.
        /// </summary>
        public string RootNodeCaption
        {
            get
            {
                return _autoClassifyDef.RootNodeCaption;
            }
        }

        /// <summary>
        /// Gets or sets the defintion of the auto-generated hierarchy
        /// </summary>
        public AutoClassifyDef AutoClassifyDef
        {
            get
            {
                return _autoClassifyDef;
            }
            set
            {
                _autoClassifyDef = value;
            }
        }

        #region private methods

        /// <summary>
        /// Create a single branch tree to represent the level of hierarchy
        /// </summary>
        /// <returns>The root of the tree</returns>
        private TreeNode CreateTree()
        {
            TreeNode root = new TreeNode();
            root.Text = RootNodeCaption;
            root.ImageIndex = 0;
            root.SelectedImageIndex = 1;

            TreeNode currentNode = root;
            TreeNode childNode;
            int level = 1;
            foreach (AutoClassifyLevel levelDef in _autoClassifyDef.AutoClassifyLevels)
            {
                childNode = new TreeNode();
                if (string.IsNullOrEmpty(levelDef.Name))
                {
                    levelDef.Name = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LevelNodeText"), level);
                }
                childNode.Text = levelDef.Name;
                childNode.ImageIndex = 0;
                childNode.SelectedImageIndex = 1;
                childNode.Expand();

                // add to parent node
                currentNode.Nodes.Add(childNode);
                currentNode = childNode;
                level++;
            }

            return root;
        }

        /// <summary>
        /// show the hierarchy levels in the tree view
        /// </summary>
        private void ShowHierachyLevels()
        {
            TreeNode treeRoot = CreateTree();

            hierarchyLevelTreeView.BeginUpdate();
            hierarchyLevelTreeView.Nodes.Clear();
            hierarchyLevelTreeView.Nodes.Add(treeRoot);
            // Make the first level selected initially
            if (treeRoot.Nodes.Count > 0)
            {
                hierarchyLevelTreeView.SelectedNode = treeRoot.Nodes[0];
            }
            hierarchyLevelTreeView.EndUpdate();

            if (_autoClassifyDef.HasDefinition)
            {
                this.previewButton.Enabled = true;
            }
            else
            {
                this.previewButton.Enabled = false;
            }
        }

        /// <summary>
        /// Validate the level specs and display errors if there exist
        /// </summary>
        /// <returns>true if the validation succeed, false if fails</returns>
        private bool ValidateLevels()
        {
            bool status = true;

            foreach (AutoClassifyLevel levelDef in _autoClassifyDef.AutoClassifyLevels)
            {
                if (string.IsNullOrEmpty(levelDef.ClassifyingAttribute))
                {
                    string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.MissingClassifyAttribute"), levelDef.Name);
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                    status = false;
                    break;
                }
            }

            return status;
        }

        #endregion

        private void DefineAutoHierarchyDialog_Load(object sender, EventArgs e)
        {
            // create one level by default
            if (_autoClassifyDef != null)
            {
                if (!_autoClassifyDef.HasDefinition)
                {
                    AutoClassifyLevel level = new AutoClassifyLevel();
                    _autoClassifyDef.AddLevel(level); // use this method to add so it gets the data view assigned

                    // default number of level
                    this.levelNumericUpDown.Value = 2;
                }
                else
                {
                    // show the existing levels
                    this.levelNumericUpDown.Value = _autoClassifyDef.AutoClassifyLevels.Count + 1;
                }
            }
        }

        private void hierarchyLevelTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get the selected node
            TreeNode selectedNode = (TreeNode)e.Node;
            if (selectedNode.Level > 0)
            {
                // level 0 is root node
                levelPropertyGrid.SelectedObject = _autoClassifyDef.AutoClassifyLevels[selectedNode.Level - 1];
            }
            else
            {
                levelPropertyGrid.SelectedObject = null;
            }
        }

        private void levelNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            int levelValue = ((int)this.levelNumericUpDown.Value) - 1;

            if (levelValue > _autoClassifyDef.AutoClassifyLevels.Count)
            {
                // add new level
                int addLevels = levelValue - _autoClassifyDef.AutoClassifyLevels.Count;
                for (int i = 0; i < addLevels; i++)
                {
                    AutoClassifyLevel levelDef = new AutoClassifyLevel();
                    _autoClassifyDef.AddLevel(levelDef); // use this method to add so that it gets the data view assigned
                }
            }
            else if (levelValue < _autoClassifyDef.AutoClassifyLevels.Count)
            {
                // remove level
                int deleteLevels = _autoClassifyDef.AutoClassifyLevels.Count - levelValue;
                for (int i = 0; i < deleteLevels; i++)
                {
                    // remove the last level
                    _autoClassifyDef.RemoveLastLevel();
                }
            }

            // show the levels
            ShowHierachyLevels();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            levelNumericUpDown.Value = 1; // only root is displayed
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            // validate the all level definition
            if (this.ValidateLevels())
            {
                PreviewHierarchyDialog dialog = new PreviewHierarchyDialog();
                dialog.AutoClassifyDef = _autoClassifyDef;

                dialog.ShowDialog();
            }
        }

        private void levelPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // validate the all level definition
            if (!this.ValidateLevels())
            {
                this.DialogResult = DialogResult.None;
                return;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO, implement auto-classifying for dynamic levels
            this.tabControl1.SelectedIndex = 0;
        }
    }
}
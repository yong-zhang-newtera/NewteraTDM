using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
    public partial class PreviewHierarchyDialog : Form
    {
        private AutoClassifyDef _autoClassifyDef;
        private CMDataServiceStub _dataService;
        private bool _isRequestComplete;
        private WorkInProgressDialog _workInProgressDialog;
        private bool _isCancelled;
        private MethodInvoker _createTreeMethod;
        private TreeNode _treeRoot;
        private string _errorMessage;
        private HierarchyGenerationUtil _util;

        public PreviewHierarchyDialog()
        {
            InitializeComponent();

            _dataService = new CMDataServiceStub();
            _createTreeMethod = null;
            _treeRoot = null;
            _workInProgressDialog = new WorkInProgressDialog();
        }

        ~PreviewHierarchyDialog()
		{
            if (_workInProgressDialog != null)
            {
                _workInProgressDialog.Dispose();
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
        /// Show the working dialog
        /// </summary>
        /// <remarks>It has to deal with multi-threading issue</remarks>
        private void ShowWorkingDialog()
        {
            ShowWorkingDialog(false, null);
        }

        /// <summary>
        /// Show the working dialog
        /// </summary>
        /// <remarks>It has to deal with multi-threading issue</remarks>
        private void ShowWorkingDialog(bool cancellable, MethodInvoker cancellCallback)
        {
            lock (_workInProgressDialog)
            {

                _workInProgressDialog.EnableCancel = cancellable;
                _workInProgressDialog.CancelCallback = cancellCallback;

                // check _isRequestComplete flag in case the worker thread
                // completes the request before the working dialog is shown
                if (!_isRequestComplete && !_workInProgressDialog.Visible)
                {
                    _workInProgressDialog.DisplayText = null;
                    _workInProgressDialog.ShowDialog();
                }
            }
        }

        private delegate void HideWorkingDialogDelegate();

        /// <summary>
        /// Hide the working dialog
        /// </summary>
        /// <remarks>Has to condider multi-threading issue</remarks>
        private void HideWorkingDialog()
        {
            if (this.InvokeRequired == false)
            {
                // It is the UI thread, go ahead to close the working dialog
                // lock it while updating _isRequestComplete
                lock (_workInProgressDialog)
                {
                    if (_workInProgressDialog.Visible)
                    {
                        _workInProgressDialog.Close();
                    }
                    _isRequestComplete = true;
                }
            }
            else
            {
                // It is a worker thread, pass the control to the UI thread
                HideWorkingDialogDelegate hideWorkingDialog = new HideWorkingDialogDelegate(HideWorkingDialog);
                this.BeginInvoke(hideWorkingDialog);
            }
        }

        private void GenerateHierarchy()
        {
            // run the generating process on a worker thread since it may take a long time
            _createTreeMethod = new MethodInvoker(CreateTree);

            _isCancelled = false;

            _createTreeMethod.BeginInvoke(new AsyncCallback(CreateTreeDone), null);

            _isRequestComplete = false;

            // launch a work in progress dialog
            ShowWorkingDialog(true, new MethodInvoker(CancelGeneratingHierarchy));
        }

        /// <summary>
        /// The AsyncCallback event handler for CreateTree method.
        /// </summary>
        /// <param name="res">The result</param>
        private void CreateTreeDone(IAsyncResult res)
        {
            try
            {
                _createTreeMethod.EndInvoke(res);

                ShowTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
            }
        }

        private delegate void ShowTreeDelegate();

        /// <summary>
        /// Display the generated tree
        /// </summary>
        private void ShowTree()
        {
            if (this.InvokeRequired == false)
            {
                // it is the UI thread, continue

                if (_treeRoot != null)
                {
                    hierarchyTreeView.BeginUpdate();
                    hierarchyTreeView.Nodes.Clear();
                    hierarchyTreeView.Nodes.Add(_treeRoot);
                    hierarchyTreeView.EndUpdate();
                }

                // show the error which occured during construction of the tree
                if (_errorMessage != null)
                {
                    MessageBox.Show(_errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                ShowTreeDelegate showTree = new ShowTreeDelegate(ShowTree);

                this.BeginInvoke(showTree, null);
            }
        }

        /// <summary>
        /// Cancel the lengthy in a worker thread
        /// </summary>
        private void CancelGeneratingHierarchy()
        {
            this._isCancelled = true;
            this._util.IsCancelled = true;
        }

        /// <summary>
        /// Create a tree based on the definitions
        /// </summary>
        /// <returns>The root of the tree</returns>
        private void CreateTree()
        {
            try
            {
                _errorMessage = null;
                _treeRoot = new TreeNode();
                _treeRoot.Text = _autoClassifyDef.RootNodeCaption;
                _treeRoot.ImageIndex = 0;
                _treeRoot.SelectedImageIndex = 1;
                _treeRoot.Expand();

                // check the size of returned result, confirm if the class has too many instances
                DataViewModel dataView = _autoClassifyDef.DataView;
                string query = _autoClassifyDef.DataView.SearchQuery;
                int count = _dataService.ExecuteCount(ConnectionStringBuilder.Instance.Create(dataView.SchemaModel.SchemaInfo), query);
                if (count > HierarchyGenerationUtil.MaxInstanceNumber)
                {
                    string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.TooBigSize"), count);

                    if (MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        // abort generation process
                        return;
                    }
                }

                // create the hierarchy generation util
                _util = new HierarchyGenerationUtil(_autoClassifyDef,
                    _dataService,
                    _autoClassifyDef.DataView.Clone());

                // grow the first level of child nodes
                CreateTreeLevel(_treeRoot, 0);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Create a collection of child nodes and add them to the parent node. This method is called
        /// recursively
        /// </summary>
        /// <param name="parentNode">The parent tree node</param>
        /// <param name="childLevelIndex">The index of current child level, starting from 0 for first level of child and so on.</param>
        private void CreateTreeLevel(TreeNode parentNode, int childLevelIndex)
        {
            if (_isCancelled)
            {
                // the process is cancelled by the user
                throw new Exception(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.HierarchyGenerationCancelled"));
            }

            // first, get number of child nodes for this level. raise an error if the number is 
            // too big
            AutoClassifyLevel currentAutoLevelDef = _autoClassifyDef.GetClassifyLevelAt(childLevelIndex);

            StringCollection distinctNodeValues = this._util.GetDistinctLevelValue(currentAutoLevelDef, childLevelIndex);

            // throw an exception when number of child nodes is too big
            if (distinctNodeValues.Count > HierarchyGenerationUtil.MaxChildNumber)
            {
                string path = _autoClassifyDef.RootNodeCaption;
                for (int i = 0; i < childLevelIndex; i++)
                {
                    path += "/" + _autoClassifyDef.GetClassifyLevelAt(i).NodeValue;
                }

                string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.TooManyChildNodes"), path, HierarchyGenerationUtil.MaxChildNumber);
                throw new Exception(msg);
            }

            // create a tree node for each distinct values
            TreeNode childNode;
            foreach (string childNodeValue in distinctNodeValues)
            {
                // create the child tree node and add to the parent node
                childNode = new TreeNode();
                childNode.Text = childNodeValue;
                childNode.ImageIndex = 0;
                childNode.SelectedImageIndex = 1;
                childNode.Expand();

                parentNode.Nodes.Add(childNode);

                // grow the child node by calling CreateTreeLevel method recursivly
                if (childLevelIndex < (_autoClassifyDef.AutoClassifyLevels.Count - 1))
                {
                    currentAutoLevelDef.NodeValue = childNodeValue;
                    CreateTreeLevel(childNode, childLevelIndex + 1);
                }
            }
        }

        #endregion

        private void PreviewHierarchyDialog_Load(object sender, EventArgs e)
        {
            if (_autoClassifyDef != null && _autoClassifyDef.HasDefinition)
            {
                GenerateHierarchy();
            }
        }
    }
}
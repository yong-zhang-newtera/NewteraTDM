using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Web.Services.Dime;
using Word = Microsoft.Office.Interop.Word;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;
using Newtera.WindowsControl.Chart;
using Newtera.DataGridActiveX;
using SmartWord.WorkflowModelWebService;
using SmartWord.MetaDataWebService;
using SmartWord.CMDataWebService;
using SmartWord.AttachmentWebService;
using Newtera.DataGridActiveX.ActiveXControlWebService;

namespace SmartWord
{
    public partial class NavigationControl : UserControl, IDataGridControl
    {
        private const int DataThreshhold = 10000;
        private const int PopulateLineLimit = 5000;
        private const int LevelLimit = 5; // limit level of finding a related class

        private Newtera.Common.Core.SchemaInfo[] _schemaInfos = null;
        private MetaDataModel _metaData = null;
        private string _rootClass = "ALL";
        private IMetaDataElement _selectedMetaDataElement = null;
        private DataViewModel _dataView;
        private DataViewElementCollection _classAttributes;
        private CMDataService _dataService;
        private MenuItemStates _menuItemStates;
        private bool _isAuthenticated = false;
        private string _userName = null;
        private MetaDataTreeNode _treeRoot = null;
        private string _selectedAttributeName = null;
        private Word.XMLNode _selectedXmlNode = null;
        private GraphWizard _graphWizard;
        private string _graphFileName = null;
        private bool _isRequestComplete;
        private bool _isCancelled;
        private WorkInProgressDialog _workInProgressDialog;
        private MethodInvoker _populateFamilyMethod;
        private string _errorMessage;
        private Word.XMLNode _familyNode;

        private object _selectedRange = System.Type.Missing;
        private object objMissing = System.Type.Missing;
        private int _pageSize = 100;

        public NavigationControl()
        {
            InitializeComponent();

            _menuItemStates = new MenuItemStates();
            this.resultDataControl.MenuItemStates = _menuItemStates;
            this.resultDataControl.UserManager = new WindowClientUserManager();
            _workInProgressDialog = null;

            // Register the menu item state change event handler
            _menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);
        }

        #region internal methods

        /// <summary>
        /// Gets or sets the selected range in the Word document
        /// </summary>
        internal object SelectedRange
        {
            get
            {
                return _selectedRange;
            }
            set
            {
                _selectedRange = value;
            }
        }

        internal void EnableInsertButton()
        {
            if (_selectedXmlNode == null)
            {
                // to enable the button for inserting a database node
                insertXmlNodeButton.Text = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertDatabaseNode");
                insertXmlNodeButton.Enabled = true;
            }
        }

        /// <summary>
        /// Select the specified database
        /// </summary>
        /// <param name="xmlNode">The selected xml node</param>
        /// <param name="database">The database name</param>
        internal void SelectDatabase(Word.XMLNode xmlNode, string database)
        {
            if (_userName != null)
            {
                // attach a custom principal to the thread
                AttachCustomPrincipal();

                _selectedXmlNode = xmlNode;
                insertXmlNodeButton.Text = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertViewNode");

                if (database != null && database != this._metaData.SchemaInfo.NameAndVersion)
                {
                    int databaseIndex = 0;
                    Newtera.Common.Core.SchemaInfo selectedSchemaInfo = null;
                    foreach (Newtera.Common.Core.SchemaInfo schemaInfo in this._schemaInfos)
                    {
                        if (schemaInfo.NameAndVersion == database)
                        {
                            selectedSchemaInfo = schemaInfo;
                            break;
                        }

                        databaseIndex++;
                    }

                    if (selectedSchemaInfo != null)
                    {
                        // change the database combobox selection to trigger the database switching
                        this.databaseComboBox.SelectedIndex = databaseIndex;
                    }
                }

                if (_selectedMetaDataElement != null)
                {
                    this.insertXmlNodeButton.Enabled = true;
                }
                else
                {
                    this.insertXmlNodeButton.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Select the specified meta data element
        /// </summary>
        /// <param name="xmlNode">The selected xml node</param>
        /// <param name="database">The database name</param>
        /// <param name="elementName">The meta data element name</param>
        /// <param name="elementType">The type of the meta data element, it can be class element, data view element, or taxon element</param>
        /// <param name="taxonomyName">Provide the taxonomy name when the element is a taxon element, null otherwise.</param>
        internal void SelectElement(Word.XMLNode xmlNode, string database, string elementName, string elementType, string taxonomyName)
        {
            if (_userName != null)
            {
                // attach a custom principal to the thread
                AttachCustomPrincipal();

                _selectedXmlNode = xmlNode;
                if (xmlNode.BaseName == ThisDocument.ViewNodeName)
                {
                    insertXmlNodeButton.Text = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertPropertyNode");
                }
                else
                {
                    insertXmlNodeButton.Text = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertViewNode");
                }

                if ((_selectedMetaDataElement == null || _selectedMetaDataElement.Name != elementName))
                {
                    DoSelectElement(elementName, elementType, taxonomyName);
                }

                this.insertXmlNodeButton.Enabled = true;
            }
        }

        /// <summary>
        /// Select the specified attribute
        /// </summary>
        /// <param name="xmlNode">The selected xml node</param>
        /// <param name="database">The database name</param>
        /// <param name="elementName">The meta data name</param>
        /// <param name="elementType">The type of the meta data element, it can be class element, data view element, or taxon element.</param>
        /// <param name="taxonomyName">Provide the taxonomy name when the element is a taxon element, null otherwise.</param>
        /// <param name="attributeName">The attribute name</param>
        internal void SelectAttribute(Word.XMLNode xmlNode, string database, string elementName, string elementType, string taxonomyName, string attributeName)
        {
            if (_userName != null)
            {
                // attach a custom principal to the thread
                AttachCustomPrincipal();

                _selectedXmlNode = xmlNode;

                if ((_selectedMetaDataElement == null || _selectedMetaDataElement.Name != elementName))
                {
                    _selectedAttributeName = attributeName;

                    DoSelectElement(elementName, elementType, taxonomyName);
                }
                else if (_selectedAttributeName == null || _selectedAttributeName != attributeName)
                {
                    _selectedAttributeName = attributeName;

                    foreach (ResultAttributeListViewItem item in this.attributeListView.Items)
                    {
                        if (item.Selected)
                        {
                            item.Selected = false;
                        }

                        if (item.ResultAttribute.Name == attributeName)
                        {
                            item.Selected = true;
                            item.EnsureVisible();
                        }
                    }
                }

                // no node can be added under property node, disable the button
                this.insertXmlNodeButton.Enabled = false;
            }
        }

        internal void Clear()
        {
            if (this._selectedXmlNode != null)
            {
                this._selectedXmlNode = null;
                this.insertXmlNodeButton.Enabled = false;
            }
        }

        #endregion internal methods

        #region private methods

        /// <summary>
        /// Get a web service for retrieving data instance
        /// </summary>
        /// <returns></returns>
        private CMDataService GetCMDataWebService()
        {
            if (_dataService == null)
            {
                _dataService = new CMDataService();
                _dataService.BeginQueryCompleted += BeginQueryDone;
            }

            return _dataService;
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
                    _workInProgressDialog.Close();
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

        private delegate void SetWorkingDialogDisplayTextDelegate(string msg);

        /// <summary>
        /// Set the working dialog display text
        /// </summary>
        /// <param name="msg"></param>
        private void SetWorkingDialogDisplayText(string msg)
        {
            if (this.InvokeRequired == false)
            {
                // It is the UI thread
                _workInProgressDialog.DisplayText = msg;
            }
            else
            {
                // It is a worker thread, pass the control to the UI thread
                SetWorkingDialogDisplayTextDelegate setWorkingDialogDisplayText = new SetWorkingDialogDisplayTextDelegate(SetWorkingDialogDisplayText);
                this.BeginInvoke(setWorkingDialogDisplayText, new object[] { msg });
            }
        }

        /// <summary>
        /// Perform actions of selecting a meta data element.
        /// </summary>
        /// <param name="elementName">The meta data element name</param>
        /// <param name="elementType">The type of the meta data element, it can be class element, data view element, or taxon element</param>
        /// <param name="taxonomyName">Provide the taxonomy name when the element is a taxon element, null otherwise.</param>
        private void DoSelectElement(string elementName, string elementType, string taxonomyName)
        {
            // select the tree node that representing the selected element node
            IMetaDataElement selectedElement = GetViewNodeMetaDataElement(elementName, elementType, taxonomyName);

            if (selectedElement != null)
            {
                TreeNode selectedNode = FindTreeNode(_treeRoot, selectedElement);
                if (selectedNode != null && !selectedNode.IsSelected)
                {
                    classTreeView.SelectedNode = selectedNode;
                }
            }
            else
            {
                throw new Exception(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.UnknownElement") + ":" + elementName);
            }
        }

        /// <summary>
        /// Gets the schema infos that the current user is authroized to access
        /// </summary>
        /// <returns></returns>
        private Newtera.Common.Core.SchemaInfo[] GetSchemaInfos()
        {
            _isAuthenticated = false;

            Newtera.Common.Core.SchemaInfo[] schemaInfos = null;

            // make sure the current user has been authenticated, if not, show the login
            // dialog
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                // not autheticated yet, ask user to login
                UserLoginDialog loginDialog = new UserLoginDialog();
                if (loginDialog.ShowDialog() == DialogResult.OK)
                {
                    _isAuthenticated = true;
                    _userName = loginDialog.UserName;
                    this.resultDataControl.UserName = _userName;
                }
            }
            else
            {
                _isAuthenticated = false;
            }

            if (_isAuthenticated)
            {
                Cursor preCursor = Cursor.Current;

                // gets schema infos from the server
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    WorkflowModelService workflowModelServic = new WorkflowModelService();

                    SmartWord.WorkflowModelWebService.SchemaInfo[] schemas = workflowModelServic.GetAuthorizedSchemaInfos(ConnectionStringBuilder.Instance.Create());
                    schemaInfos = new Newtera.Common.Core.SchemaInfo[schemas.Length];
                    for (int i = 0; i < schemas.Length; i++)
                    {
                        schemaInfos[i] = new Newtera.Common.Core.SchemaInfo();
                        schemaInfos[i].Name = schemas[i].Name;
                        schemaInfos[i].Version = schemas[i].Version;
                    }
                }
                finally
                {
                    Cursor.Current = preCursor;
                }
            }

            return schemaInfos;
        }

        private MetaDataModel GetMetaDataModel(Newtera.Common.Core.SchemaInfo schemaInfo)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // invoke the web service synchronously
                MetaDataService metaDataService = new MetaDataService();
                string[] xmlStrings = metaDataService.GetMetaData(ConnectionStringBuilder.Instance.Create(schemaInfo));

                // Create an meta data object to hold the schema model
                MetaDataModel metaData = new MetaDataModel();
                metaData.SchemaInfo = schemaInfo;

                // read mete-data from xml strings
                metaData.Load(xmlStrings);

                return metaData;
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// Display the database schema in form of a tree
        /// </summary>
        private void ShowSchemaTree()
        {
            if (_metaData != null && _rootClass != null)
            {
                MetaDataTreeBuilder builder = new MetaDataTreeBuilder();

                // do not show attributes and constraints nodes
                builder.IsAttributesShown = false;
                builder.IsConstraintsShown = false;
                builder.IsTaxonomiesShown = true;
                builder.IsDataViewsShown = true;

                _treeRoot = (MetaDataTreeNode) builder.BuildTree(_metaData);

                classTreeView.BeginUpdate();
                classTreeView.Nodes.Clear();
                classTreeView.Nodes.Add(_treeRoot);

                if (_selectedMetaDataElement != null)
                {
                    // make the initial selection
                    TreeNode selectedNode = FindTreeNode(_treeRoot, _selectedMetaDataElement);
                    if (selectedNode != null)
                    {
                        classTreeView.SelectedNode = selectedNode;
                    }
                }
                else
                {
                    // expand the root node
                    if (classTreeView.Nodes.Count > 0)
                    {
                        classTreeView.Nodes[0].Expand();
                    }
                }

                classTreeView.EndUpdate();

                _selectedAttributeName = null;
            }
        }

        private MetaDataTreeNode FindTreeNode(MetaDataTreeNode root, IMetaDataElement theElement)
        {
            foreach (MetaDataTreeNode node in root.Nodes)
            {
                if (theElement == node.MetaDataElement)
                {
                    return node;
                }
                else
                {
                    MetaDataTreeNode found = FindTreeNode(node, theElement);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        private void ShowDataViewAttributes(IMetaDataElement theElement)
        {
            if (theElement != _selectedMetaDataElement)
            {
                _selectedMetaDataElement = theElement;

                // clear the data grid
                this.resultDataControl.ClearDataGrids();
                this.insertInstanceButton.Enabled = false;
                
                ResultAttributeListViewItem item;
                ResultAttributeListViewItem selectedItem = null;
                this.attributeListView.SuspendLayout();
                this.attributeListView.Items.Clear();

                _classAttributes = new DataViewElementCollection();
                int index = 0;
                foreach (IDataViewElement result in _dataView.ResultAttributes)
                {
                    _classAttributes.Add(result);

                    item = new ResultAttributeListViewItem(result.Caption, result);
                    if (index == 0)
                    {
                        selectedItem = item;
                    }
                    index++;

                    if (result.ElementType == ElementType.SimpleAttribute)
                    {
                        item.ImageIndex = 0;
                    }
                    else if (result.ElementType == ElementType.ArrayAttribute)
                    {
                        item.ImageIndex = 2;
                    }
                    else if (result.ElementType == ElementType.VirtualAttribute)
                    {
                        item.ImageIndex = 3;
                    }
                    else if (result.ElementType == ElementType.RelationshipAttribute)
                    {
                        item.ImageIndex = 1;
                    }
                    else
                    {
                        item.ImageIndex = 0;
                    }
                    item.SubItems.Add(result.Name);

                    if (_selectedAttributeName != null &&
                        _selectedAttributeName == result.Name)
                    {
                        selectedItem = item;
                    }

                    this.attributeListView.Items.Add(item);
                }

                // select the item and make sure the selected item is visible
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;
                    selectedItem.EnsureVisible();
                }

                this.attributeListView.ResumeLayout();
            }

            this.showInstanceButton.Enabled = true;
            if (this._selectedXmlNode != null &&
                this._selectedXmlNode.BaseName == ThisDocument.DatabaseNodeName)
            {
                this.insertXmlNodeButton.Enabled = true;
            }
        }

        private void ExecuteQuery()
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                if (this.resultDataControl.CurrentSlide.DataView != null)
                {
                    string query = this.resultDataControl.CurrentSlide.DataView.SearchQuery;

                    // invoke the web service synchronously
                    XmlNode xmlNode = GetCMDataWebService().ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        query);

                    DataSet ds = new DataSet();
                    XmlReader xmlReader = new XmlNodeReader(xmlNode);
                    ds.ReadXml(xmlReader);

                    this.resultDataControl.ShowQueryResult(ds);
                    // clear the InstanceView which may contains the old DataSet
                    this.resultDataControl.CurrentSlide.InstanceView = null;
                    if (this.resultDataControl.CurrentSlide != null && !this.resultDataControl.CurrentSlide.IsEmptyResult)
                    {
                        _menuItemStates.SetState(MenuItemID.EditSearch, true);
                        _menuItemStates.SetState(MenuItemID.EditShowAllInstances, true);
                        _menuItemStates.SetState(MenuItemID.ViewRowCount, true);
                        _menuItemStates.SetState(MenuItemID.ViewDetail, true);
                        _menuItemStates.SetState(MenuItemID.ViewChart, true);

                        this.insertInstanceButton.Enabled = true;
                    }
                    else
                    {
                        _menuItemStates.SetState(MenuItemID.EditSearch, false);
                        _menuItemStates.SetState(MenuItemID.EditShowAllInstances, false);
                        _menuItemStates.SetState(MenuItemID.ViewRowCount, false);
                        _menuItemStates.SetState(MenuItemID.ViewDetail, false);
                        _menuItemStates.SetState(MenuItemID.ViewChart, false);

                        this.insertInstanceButton.Enabled = false;
                    }
                }
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// Initialize the menu item states
        /// </summary>
        private void InitializeMenuItemStates()
        {
            _menuItemStates.SetState(MenuItemID.EditCopy, false);
            _menuItemStates.SetState(MenuItemID.EditCut, false);
            _menuItemStates.SetState(MenuItemID.EditPaste, false);
            _menuItemStates.SetState(MenuItemID.FileExport, false);
            _menuItemStates.SetState(MenuItemID.EditSearch, false);
            _menuItemStates.SetState(MenuItemID.ViewDetail, false);
            _menuItemStates.SetState(MenuItemID.EditShowAllInstances, false);
            _menuItemStates.SetState(MenuItemID.ViewDefault, false);
            _menuItemStates.SetState(MenuItemID.ViewDetailed, false);
            _menuItemStates.SetState(MenuItemID.ViewChart, false);
            _menuItemStates.SetState(MenuItemID.ViewNextRow, false);
            _menuItemStates.SetState(MenuItemID.ViewPrevRow, false);
            _menuItemStates.SetState(MenuItemID.ViewNextPage, false);
            _menuItemStates.SetState(MenuItemID.ViewPrevPage, false);
            _menuItemStates.SetState(MenuItemID.ViewRowCount, false);
        }

        private void AttachCustomPrincipal()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal == null && _isAuthenticated)
            {
                // attach a custom principal object to the thread
                CustomPrincipal.Attach(new WindowClientUserManager(), _userName);
            }
        }

        /// <summary>
        /// Gets and display the total instance count of the current query.
        /// </summary>
        private void ExecuteQueryCount()
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                if (this.resultDataControl.CurrentSlide.DataView != null)
                {
                    string query = this.resultDataControl.CurrentSlide.DataView.SearchQuery;

                    // invoke the web service synchronously

                    int count = GetCMDataWebService().ExecuteCount(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        query);

                    this.resultDataControl.ShowInstanceCount(count);
                }
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void ShowSearchDialog()
        {
            CreateSearchExprDialog dialog = new CreateSearchExprDialog();
            try
            {
                dialog.DataView = this.resultDataControl.CurrentSlide.DataView;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ExecuteQuery();
                }
            }
            catch
            {
            }
        }

        private void MenuItemStateChanged(object sender, System.EventArgs e)
        {
            StateChangedEventArgs args = (StateChangedEventArgs)e;

            // set the toolbar button states
            switch (args.ID)
            {
                case MenuItemID.EditSearch:
                    //this.SearchButton.Enabled = args.State;
                    break;
                case MenuItemID.EditShowAllInstances:
                    this.getAllInstancesToolStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.ViewDefault:
                    break;
                case MenuItemID.ViewDetail:
                    this.detailButton.Enabled = args.State;
                    break;
                case MenuItemID.ViewChart:
                    this.chartButton.Enabled = args.State;
                    break;
            }
        }

        /// <summary>
        /// Insert a corresponding XML node into the Word Document according to the
        /// attached xml schema and current position
        /// </summary>
        private void InsertXMLNode()
        {
            Word.XMLNode xmlNode = null;
            Word.XMLNode attributeNode;

            if (this._selectedXmlNode != null)
            {
                if (this._selectedXmlNode.BaseName == ThisDocument.DatabaseNodeName)
                {
                    bool containTable = false;
                    int numOfTables = 0;
                    Word.Range range = (Word.Range)_selectedRange;
                    // make sure that the class node does not contains more than one table
                    numOfTables = range.Tables.Count;
                    if (numOfTables == 1)
                    {
                        // find the table contained by the class node
                        //Remember: arrays through interop in Word are 1 based.
                        Word.Table table = range.Tables[1];
                        // make sure the table only has two rows
                        if (table.Rows.Count < 2)
                        {
                            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InvalidTable"),
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        containTable = true;
                    }

                    // ask for user whether to insert a view or a family
                    SelectViewOrFamilyDialog dialog = new SelectViewOrFamilyDialog();
                    string nodeName = ThisDocument.ViewNodeName;
                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        nodeName = dialog.InsertNodeName;
                    }

                    // insert a class node or family node
                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in _selectedXmlNode.ChildNodeSuggestions)
                    {
                        if (nodeName == ThisDocument.ViewNodeName && nodeSuggestion.BaseName == ThisDocument.ViewNodeName)
                        {
                            if (numOfTables > 1)
                            {
                                // A class node can only contain one table
                                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.TooManyTables"),
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                break;
                            }
                            else
                            {
                                // insert a view node only
                                xmlNode = nodeSuggestion.Insert(ref _selectedRange);
                                SetViewNodeAttributes(xmlNode, containTable);
                            }
                            break;
                        }
                        else if (nodeSuggestion.BaseName == ThisDocument.FamilyNodeName)
                        {
                            // insert a family node
                            xmlNode = nodeSuggestion.Insert(ref _selectedRange);
                            SetViewNodeAttributes(xmlNode, containTable);
                            break;
                        }
                    }
                }
                else if (this._selectedXmlNode.BaseName == ThisDocument.FamilyNodeName)
                {
                    // make sure the view inserting into family node is the same as the base class associated with
                    // the family node or is related to the base class
                    // of the family through some relationship chain
                    string baseClassCaption = "";
                    Stack<string> path = new Stack<string>();
                    if (!IsFamilyRelated(this._selectedXmlNode, _selectedMetaDataElement, out baseClassCaption, path))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.NotRelatedClass"), baseClassCaption);
                        MessageBox.Show(msg,
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                    bool containTable = false;
                    Word.Range range = (Word.Range)_selectedRange;
                    // make sure that the class node does not contains more than one table
                    if (range.Tables.Count == 1)
                    {
                        // find the table contained by the class node
                        //Remember: arrays through interop in Word are 1 based.
                        Word.Table table = range.Tables[1];
                        // make sure the table only has two rows
                        if (table.Rows.Count < 2)
                        {
                            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InvalidTable"),
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        containTable = true;
                    }
                    else if (range.Tables.Count > 1)
                    {
                        MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.TooManyTables"),
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    // insert a view node
                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in _selectedXmlNode.ChildNodeSuggestions)
                    {
                        // insert a family node that contains a view node
                        xmlNode = nodeSuggestion.Insert(ref _selectedRange);
                        SetViewNodeAttributes(xmlNode, containTable, path);
                    }
                }
                else if (this._selectedXmlNode.BaseName == ThisDocument.ViewNodeName)
                {
                    // insert attribute nodes
                    if (this.attributeListView.SelectedItems.Count > 0)
                    {
                        for (int i = 0; i < this.attributeListView.SelectedIndices.Count; i++)
                        {
                            int index = this.attributeListView.SelectedIndices[i];
                            IDataViewElement attribute = this._classAttributes[index];
                            // do not insert relationship attribute
                            if (!(attribute is DataRelationshipAttribute))
                            {
                                if (attribute is DataArrayAttribute)
                                {
                                    // insert an array tage with an embedded table
                                    InsertArrayNode(attribute);
                                }
                                else
                                {
                                    // insert a Property tag
                                    InsertPropertyNode(attribute);
                                }
                            }
                        }
                    }
                }
                else if (this._selectedXmlNode.BaseName == ThisDocument.PropertyNodeName ||
                    this._selectedXmlNode.BaseName == ThisDocument.ArrayNodeName)
                {
                }
            }
            else
            {
                // there is no currently selected node, insert a Database node if it doesn't exist.
                if (Globals.ThisDocument.XMLNodes.Count == 0)
                {
                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in Globals.ThisDocument.ChildNodeSuggestions)
                    {
                        xmlNode = nodeSuggestion.Insert(ref _selectedRange);
                    }
                    if (xmlNode != null)
                    {
                        attributeNode = xmlNode.Attributes.Add(ThisDocument.DatabaseAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                        attributeNode.NodeValue = this._metaData.SchemaInfo.NameAndVersion;
                    }
                }
            }
        }

        // Insert a Property XML Tag
        private void InsertPropertyNode(IDataViewElement attribute)
        {
            Word.XMLNode xmlNode = null;
            Word.XMLNode attributeNode;

            foreach (Word.XMLChildNodeSuggestion nodeSuggestion in _selectedXmlNode.ChildNodeSuggestions)
            {
                if (nodeSuggestion.BaseName == ThisDocument.PropertyNodeName)
                {
                    xmlNode = nodeSuggestion.Insert(ref _selectedRange);
                    break;
                }
            }
            if (xmlNode != null)
            {
                attributeNode = xmlNode.Attributes.Add(ThisDocument.PropertyNameAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                attributeNode.NodeValue = attribute.Name;
            }
        }

        // Insert a Array XML Tag with an embedded table
        private void InsertArrayNode(IDataViewElement attribute)
        {
            Word.XMLNode xmlNode = null;
            Word.XMLNode attributeNode;

            foreach (Word.XMLChildNodeSuggestion nodeSuggestion in _selectedXmlNode.ChildNodeSuggestions)
            {
                if (nodeSuggestion.BaseName == ThisDocument.ArrayNodeName)
                {
                    xmlNode = nodeSuggestion.Insert(ref _selectedRange);
                    break;
                }
            }

            if (xmlNode != null)
            {
                attributeNode = xmlNode.Attributes.Add(ThisDocument.PropertyNameAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                attributeNode.NodeValue = attribute.Name;

                ArrayAttributeElement arrayAttributeElement = (ArrayAttributeElement) attribute.GetSchemaModelElement();
                int cols = 1;
                if (arrayAttributeElement.Dimension == 2)
                {
                    // two dimension array
                    cols = arrayAttributeElement.ColumnTitles.Count;
                }

                // Insert a table with two rows inside Array Xml Node
                xmlNode.Range.Text = "Array";
                xmlNode.Range.SetRange(1, 5);
                Word.Table table = xmlNode.Range.Tables.Add(xmlNode.Range, 2, cols, ref objMissing, ref objMissing);

                // Format the table.
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Word.Borders borders = table.Columns[i + 1].Cells.Borders; // index starts from 1
                    borders[Word.WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    borders[Word.WdBorderType.wdBorderLeft].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    borders[Word.WdBorderType.wdBorderRight].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    borders[Word.WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    borders[Word.WdBorderType.wdBorderHorizontal].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                }

                // add array column titles to the first row of the table
                // Word table index starts from 1
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (i < arrayAttributeElement.ColumnTitles.Count &&
                        !string.IsNullOrEmpty(arrayAttributeElement.ColumnTitles[i]))
                    {
                        table.Cell(1, i + 1).Range.Text = arrayAttributeElement.ColumnTitles[i];
                    }
                    else
                    {
                        table.Cell(1, i + 1).Range.Text = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.UnknownTitle");
                    }
                }

                // add Column tags to the second row of the table
                Word.XMLNode columnNode;
                string colName;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in xmlNode.ChildNodeSuggestions)
                    {
                        object range = table.Cell(2, i + 1).Range;
                        if (nodeSuggestion.BaseName == ThisDocument.ColumnNodeName)
                        {
                            columnNode = nodeSuggestion.Insert(ref range);
                            if (i < arrayAttributeElement.ColumnTitles.Count &&
                                !string.IsNullOrEmpty(arrayAttributeElement.ColumnTitles[i]))
                            {
                                colName = arrayAttributeElement.ColumnTitles[i];
                            }
                            else
                            {
                                colName = "Unknown";
                            }
                            attributeNode = columnNode.Attributes.Add(ThisDocument.ColumnNameAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                            attributeNode.NodeValue = colName;
                            break;
                        }
                    }
                }
            }
        }

        private void SetViewNodeAttributes(Word.XMLNode xmlNode, bool containTable)
        {
            SetViewNodeAttributes(xmlNode, containTable, null);
        }

        private void SetViewNodeAttributes(Word.XMLNode xmlNode, bool containTable, Stack<string> path)
        {
            Word.XMLNode attributeNode;

            if (xmlNode != null)
            {
                attributeNode = xmlNode.Attributes.Add(ThisDocument.ElementAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                attributeNode.NodeValue = this._selectedMetaDataElement.Name;
                if (containTable)
                {
                    // remember the table name as an attribute
                    attributeNode = xmlNode.Attributes.Add(ThisDocument.TableAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                    attributeNode.NodeValue = this._selectedMetaDataElement.Name;
                }

                // remember the path in the attribute
                if (path != null && path.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    while (path.Count > 0)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(";");
                        }

                        builder.Append(path.Pop());
                    }

                    attributeNode = xmlNode.Attributes.Add(ThisDocument.PathAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                    attributeNode.NodeValue = builder.ToString();
                }

                // a class node can represent several kinds of meta data elements,
                // remember the type of the element in an attribute
                attributeNode = xmlNode.Attributes.Add(ThisDocument.TypeAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                if (_selectedMetaDataElement is ClassElement)
                {
                    attributeNode.NodeValue = ThisDocument.ClassType;
                }
                else if (_selectedMetaDataElement is DataViewModel)
                {
                    attributeNode.NodeValue = ThisDocument.DataViewType;
                }
                else if (_selectedMetaDataElement is TaxonNode)
                {
                    attributeNode.NodeValue = ThisDocument.TaxonType;
                    // remember the taxonomy that owns the taxon node
                    attributeNode = xmlNode.Attributes.Add(ThisDocument.TaxonomyAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                    attributeNode.NodeValue = ((TaxonNode)_selectedMetaDataElement).TaxonomyName;
                }
            }
        }

        /// <summary>
        /// Populate the selected instances in the datagrid to the selected xml node in the document
        /// </summary>
        private void PopulateToSelectedNode()
        {
            if (_selectedXmlNode != null && this.resultDataControl.CurrentSlide != null)
            {
                if (_selectedXmlNode.BaseName == ThisDocument.DatabaseNodeName)
                {
                    PopulateDatabaseNode(_selectedXmlNode);
                }
                else if (_selectedXmlNode.BaseName == ThisDocument.FamilyNodeName)
                {
                    PopulateFamilyNode(_selectedXmlNode);
                }
                else if (_selectedXmlNode.BaseName == ThisDocument.ViewNodeName)
                {
                    PopulateViewNode(_selectedXmlNode);
                }
                else if (_selectedXmlNode.BaseName == ThisDocument.PropertyNodeName &&
                    this._selectedAttributeName != null)
                {
                    // get currently selected data instance 
                    SelectedIndexCollection selectedIndices = this.resultDataControl.SelectedIndices;
                    if (selectedIndices.Count > 0)
                    {
                        InstanceView instanceView = this.GetInstanceView(selectedIndices[0]);

                        PopulatePropertyNode(_selectedXmlNode, _selectedAttributeName, instanceView);
                    }
                }
                else if (_selectedXmlNode.BaseName == ThisDocument.ArrayNodeName &&
                    this._selectedAttributeName != null)
                {
                    // get currently selected data instance 
                    SelectedIndexCollection selectedIndices = this.resultDataControl.SelectedIndices;
                    if (selectedIndices.Count > 0)
                    {
                        InstanceView instanceView = this.GetInstanceView(selectedIndices[0]);

                        PopulateArrayNode(_selectedXmlNode, _selectedAttributeName, instanceView);
                    }
                }
            }
        }

        // To see if the view node and familiy node have the same attribute values
        private bool IsFamilyBaseView(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            bool status = true;

            if (familyNode.BaseName == ThisDocument.FamilyNodeName &&
                viewNode.BaseName == ThisDocument.ViewNodeName)
            {
                if (GetAttributeValue(familyNode, ThisDocument.ElementAttribute, false) !=
                    GetAttributeValue(viewNode, ThisDocument.ElementAttribute, false))
                {
                    status = false;
                }
                else if (GetAttributeValue(familyNode, ThisDocument.TypeAttribute, false) !=
                    GetAttributeValue(viewNode, ThisDocument.TypeAttribute, false))
                {
                    status = false;
                }
                else if (GetAttributeValue(familyNode, ThisDocument.TaxonomyAttribute, false) !=
                    GetAttributeValue(viewNode, ThisDocument.TaxonomyAttribute, false))
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Populate the selected data instances in result grid to all xml nodes in the document that belong to the selected class.
        /// </summary>
        /// <param name="databaseNode">The database node</param>
        private void PopulateDatabaseNode(Word.XMLNode databaseNode)
        {
            string database = GetAttributeValue(databaseNode, ThisDocument.DatabaseAttribute, true);
            if (database == _metaData.SchemaInfo.NameAndVersion)
            {
                foreach (Word.XMLNode viewNode in databaseNode.ChildNodes)
                {
                    string elementName = GetAttributeValue(viewNode, ThisDocument.ElementAttribute, true);
                    // make sure to populate the matched node
                    if (elementName == _selectedMetaDataElement.Name)
                    {
                        if (viewNode.BaseName == ThisDocument.FamilyNodeName)
                        {
                            PopulateFamilyNode(viewNode);
                        }
                        else if (viewNode.BaseName == ThisDocument.ViewNodeName)
                        {
                            PopulateViewNode(viewNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populate the selected data instance and its related data instances to view nodes
        /// within a family node in the document.
        /// </summary>
        /// <param name="familyNode">The family node</param>
        private void PopulateFamilyNode(Word.XMLNode familyNode)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                _isCancelled = false;

                _familyNode = familyNode; // MethodInvoker takes no paramter

                // run the populating family nodes process on a worker thread since it may take a long time
                _populateFamilyMethod = new MethodInvoker(DoPopulateFamilyNode);

                _populateFamilyMethod.BeginInvoke(new AsyncCallback(DoPopulateFamilyNodeDone), null);

                _isRequestComplete = false;

                // launch a work in progress dialog
                _workInProgressDialog = new WorkInProgressDialog();
                ShowWorkingDialog(true, new MethodInvoker(CancelPopulateFamily));
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for DoPopulateFamilyNode method.
        /// </summary>
        /// <param name="res">The result</param>
        private void DoPopulateFamilyNodeDone(IAsyncResult res)
        {
            try
            {
                _populateFamilyMethod.EndInvoke(res);

                CompletePopulation();
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

        private delegate void CompletePopulationDelegate();

        /// <summary>
        /// Complete family node population
        /// </summary>
        private void CompletePopulation()
        {
            if (this.InvokeRequired == false)
            {
                // it is the UI thread, continue

                // show the error which occured during construction of the hierarchy
                if (_errorMessage != null)
                {
                    MessageBox.Show(_errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                CompletePopulationDelegate completePopulation = new CompletePopulationDelegate(CompletePopulation);

                this.BeginInvoke(completePopulation, null);
            }
        }

        /// <summary>
        /// Cancel the lengthy in a worker thread
        /// </summary>
        private void CancelPopulateFamily()
        {
            this._isCancelled = true;
        }

        private void DoPopulateFamilyNode()
        {
            _errorMessage = null;

            try
            {
                if (_familyNode != null)
                {
                    foreach (Word.XMLNode viewNode in _familyNode.ChildNodes)
                    {
                        if (_isCancelled)
                        {
                            break;
                        }

                        if (IsFamilyBaseView(_familyNode, viewNode))
                        {
                            // populate the base view node
                            PopulateViewNode(viewNode);
                        }
                        else if (IsFamilyRelated(_familyNode, viewNode))
                        {
                            // populate the view node within a family node
                            PopulateFamilyViewNode(_familyNode, viewNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Populate all the property nodes under a view node
        /// </summary>
        /// <param name="viewNode">The view node</param>
        private void PopulateViewNode(Word.XMLNode viewNode)
        {
            string tableName = GetAttributeValue(viewNode, ThisDocument.TableAttribute, false);
            if (tableName == null)
            {
                // populate each individual property nodes under the class node with the currently
                // selected data instance in the result grid
                InstanceView instanceView = this.GetInstanceView();
                foreach (Word.XMLNode childNode in viewNode.ChildNodes)
                {
                    string propertyName = GetAttributeValue(childNode, ThisDocument.PropertyNameAttribute, true);
                    if (childNode.BaseName == ThisDocument.PropertyNodeName)
                    {
                        PopulatePropertyNode(childNode, propertyName, instanceView);
                    }
                    else if (childNode.BaseName == ThisDocument.ArrayNodeName)
                    {
                        PopulateArrayNode(childNode, propertyName, instanceView);
                    }
                }
            }
            else
            {
                // populate the table rows with all selected data instances in the result grid
                PopulateTable(viewNode);
            }
        }

        /// <summary>
        /// Populate all the property nodes under a view node which is related to a family node
        /// </summary>
        /// <param name="familyNode">The family node</param>
        /// <param name="viewNode">The view node</param>
        private void PopulateFamilyViewNode(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            // populate each individual property nodes under the view node with the data instance
            // that is related to the data instances selected in the result grid
            InstanceView instanceView = GetInstanceView(familyNode, viewNode);
 
            string tableName = GetAttributeValue(viewNode, ThisDocument.TableAttribute, false);
            if (tableName == null)
            {
                foreach (Word.XMLNode childNode in viewNode.ChildNodes)
                {
                    if (instanceView != null)
                    {
                        string propertyName = GetAttributeValue(childNode, ThisDocument.PropertyNameAttribute, true);
                        if (childNode.BaseName == ThisDocument.PropertyNodeName)
                        {
                            PopulatePropertyNode(childNode, propertyName, instanceView);
                        }
                        else if (childNode.BaseName == ThisDocument.ArrayNodeName)
                        {
                            PopulateArrayNode(childNode, propertyName, instanceView);
                        }
                    }
                    else
                    {
                        childNode.Text = "";
                    }
                }
            }
            else
            {
                // populate the table rows with with the data instance
                // that are related to the data instances selected in the data grid
                PopulateTable(familyNode, viewNode, instanceView);
            }
        }

        /// <summary>
        /// Populate a property node
        /// </summary>
        /// <param name="propertyNode">The property node</param>
        /// <param name="propertyName"></param>
        /// <param name="instanceView"></param>
        private void PopulatePropertyNode(Word.XMLNode propertyNode,
            string propertyName, InstanceView instanceView)
        {
            PropertyDescriptorCollection properties = instanceView.GetProperties(null);
            InstanceAttributePropertyDescriptor ipd = (InstanceAttributePropertyDescriptor) properties[propertyName];
            if (ipd != null)
            {
                if (ipd.PropertyType.IsEnum && ipd.IsMultipleChoice)
                {
                    object[] vEnums = (object[])ipd.GetValue();
                    if (vEnums != null && vEnums.Length > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < vEnums.Length; i++)
                        {
                            if (i > 0)
                            {
                                builder.Append(",");
                            }

                            builder.Append(vEnums[i].ToString());
                        }

                        propertyNode.Text = builder.ToString();
                    }
                    else
                    {
                        propertyNode.Text = "";
                    }
                }
                else
                {
                    object val = ipd.GetValue();
                    if (val != null)
                    {
                        propertyNode.Text = val.ToString();
                    }
                    else
                    {
                        propertyNode.Text = "";
                    }
                }
            }
            else
            {
                // unable to find the corresponding property from the instance view, clear the propertyNode value
                propertyNode.Text = "";
            }
        }

        /// <summary>
        /// Populate a column node
        /// </summary>
        /// <param name="columnNode">The column node</param>
        /// <param name="columnName">The column name</param>
        /// <param name="dataRow">The row data</param>
        private void PopulateColumnNode(Word.XMLNode columnNode,
            string columnName, DataRow dataRow)
        {
            string val = "";
            if (columnName != null)
            {
                try
                {
                    val = dataRow[columnName].ToString();
                }
                catch (Exception)
                {
                    // for one-dimension array, the column name is Unknown which does not exist in data row
                    // get the value from the column zero
                    val = dataRow[0].ToString();
                }

                columnNode.Text = val;
            }
            else
            {
                columnNode.Text = "";
            }
        }

        /// <summary>
        /// Populate an array node
        /// </summary>
        /// <param name="arrayNode">The array node</param>
        /// <param name="arrayName">The array name</param>
        /// <param name="instanceView"></param>
        private void PopulateArrayNode(Word.XMLNode arrayNode,
            string arrayName, InstanceView instanceView)
        {
            PropertyDescriptorCollection properties = instanceView.GetProperties(null);
            InstanceAttributePropertyDescriptor ipd = (InstanceAttributePropertyDescriptor)properties[arrayName];
            if (ipd != null)
            {
                if (ipd.IsArray)
                {
                    DataTable arrayData = null;
                    ArrayDataTableView arrayDataTableView = (ArrayDataTableView)ipd.GetValue();
                    if (arrayDataTableView != null)
                    {
                        arrayData = arrayDataTableView.ArrayAttributeValue;
                    }

                    if (arrayData != null && arrayData.Columns.Count > 0)
                    {
                        // find the table contained by the array node
                        Word.Table arraytable = arrayNode.Range.Tables[1];
                        if (!IsArrayTable(arraytable, arrayData))
                        {
                            // for some reason, when the array table is nested table
                            // arrayNode.Range.Tables[1] return the root table,
                            // therefore, we have to search for the nested table
                            arraytable = FindNestedTable(arraytable, arrayData);
                            if (arraytable == null)
                            {
                                throw new Exception("Unable to find table for array " + arrayName);
                            }
                        }

                        // Remove any data in the table.
                        if (arraytable.Rows.Count > 2 && NeedDel(arrayNode, arraytable))
                        {
                            RemoveDataRows(arraytable);
                        }

                        // clear the old content
                        foreach (Word.XMLNode columnNode in arrayNode.ChildNodes)
                        {
                            columnNode.Text = "";
                        }

                        // populate the array table with the data in DataTable
                        PopulateArrayTableRows(arrayNode, arraytable, arrayData);
                    }
                }
            }
        }

        /// <summary>
        /// Populate the rows in a table
        /// </summary>
        /// <param name="viewNode">Word XmlNode representing a view</param>
        private void PopulateTable(Word.XMLNode viewNode)
        {
            if (viewNode.Range.Tables.Count == 1)
            {
                // find the table contained by the view node
                //Remember: arrays through interop in Word are 1 based.
                Word.Table table = viewNode.Range.Tables[1];
                // Remove any data in the table.
                if (table.Rows.Count > 2 && NeedDel(viewNode, table))
                {
                    RemoveDataRows(table);
                }

                // populate the table with the data instances selected in result grid
                PopulateTableRows(viewNode, table);
            }
        }

        /// <summary>
        /// Populate the rows in a table with the provided InstanceView
        /// </summary>
        /// <param name="familyNode">The family node that contains the view node, can be null</param>
        /// <param name="viewNode">Word XmlNode representing a view</param>
        private void PopulateTable(Word.XMLNode familyNode, Word.XMLNode viewNode, InstanceView instanceView)
        {
            if (viewNode.Range.Tables.Count == 1)
            {
                // find the table contained by the view node
                //Remember: arrays through interop in Word are 1 based.
                Word.Table table = viewNode.Range.Tables[1];
                // Remove any data in the table.
                if (table.Rows.Count > 2 && NeedDel(viewNode, table))
                {
                    RemoveDataRows(table);
                }

                // clear the old content
                foreach (Word.XMLNode propertyNode in viewNode.ChildNodes)
                {
                    propertyNode.Text = "";
                }

                // populate the table with the data instances with data in the instanceView
                PopulateTableRows(familyNode, viewNode, table, instanceView);
            }
        }

        /// <summary>
        /// Deletes all except the two template rows in the table.
        /// </summary>
        /// <param name="table"></param>
        private void RemoveDataRows(Word.Table table)
        {
            if (table.Rows.Count < 2)
            {
                throw new ApplicationException(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InvalidTable"));
            }
            try
            {
                for (int i = table.Rows.Count; i > 2; i--)
                {
                    table.Rows[i].Delete();
                }
            }
            catch
            {
            }
                
        }

        /// <summary>
        /// Populates the rows of the table in the document with the data instances selected in result grid.
        /// </summary>
        /// <param name="viewNode">The view node</param>
        /// <param name="table">Word table</param>
        private void PopulateTableRows(Word.XMLNode viewNode, Word.Table table)
        {
            int rowId = 0;
            int propertiesPerRow = viewNode.ChildNodes.Count;
            InstanceView instanceView;

            SelectedIndexCollection selectedIndices = this.resultDataControl.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                for (int i = 0; i < selectedIndices.Count; i++)
                {
                    // Add item rows only after the first template row.
                    if (rowId > 0)
                    {
                        table.Rows.Add(ref objMissing);
                    }

                    instanceView = this.GetInstanceView(selectedIndices[i]);

                    // populate a table row
                    PopulateTableRow(viewNode, rowId, instanceView, propertiesPerRow);

                    // Move to the next item row in the table.
                    rowId++;
                }
            }
        }

        /// <summary>
        /// Populates the rows of the table in the document with the data instances retrived from database.
        /// </summary>
        /// <param name="familyNode">The family node that contains the view node.</param>
        /// <param name="viewNode">The view node</param>
        /// <param name="table">Word table</param>
        /// <param name="instanceView">The instanceview</param>
        private void PopulateTableRows(Word.XMLNode familyNode, Word.XMLNode viewNode, Word.Table table, InstanceView instanceView)
        {
            int rowId = 0;
            int propertiesPerRow = viewNode.ChildNodes.Count;

            if (instanceView != null)
            {
                int count = instanceView.InstanceData.DataSet.Tables[instanceView.DataView.BaseClass.Name].Rows.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (_isCancelled)
                        {
                            break;
                        }

                        // Add item rows only after the first template row.
                        if (rowId > 0)
                        {
                            table.Rows.Add(ref objMissing);
                        }

                        instanceView.SelectedIndex = i; ;

                        // populate a table row
                        PopulateTableRow(viewNode, rowId, instanceView, propertiesPerRow);

                        // Move to the next item row in the table.
                        rowId++;
                    }
                }
            }
        }

        /// <summary>
        /// Populates the rows of the table representing an array with the data in a DataTable
        /// </summary>
        /// <param name="arrayNode">The array node</param>
        /// <param name="table">Word table</param>
        /// <param name="arrayData">The array data</param>
        private void PopulateArrayTableRows(Word.XMLNode arrayNode, Word.Table table, DataTable arrayData)
        {
            int rowId = 0;
            int columnsPerRow = arrayNode.ChildNodes.Count;

            if (arrayData.Rows.Count > 0)
            {
                for (int i = 0; i < arrayData.Rows.Count; i++)
                {
                    // Add item rows only after the first template row.
                    if (rowId > 0)
                    {
                        table.Rows.Add(ref objMissing);
                    }

                    DataRow dataRow = arrayData.Rows[i];

                    // populate an array table row
                    PopulateArrayTableRow(arrayNode, rowId, dataRow, columnsPerRow);

                    // Move to the next item row in the table.
                    rowId++;
                }
            }
        }

        // Get the meta data element represented by attribute values of the view node
        private IMetaDataElement GetViewNodeMetaDataElement(string elementName, string elementType, string taxonomyName)
        {
            IMetaDataElement metaDataElement = null;

            switch (elementType)
            {
                case ThisDocument.ClassType:
                    // elementName is a class name
                    metaDataElement = _metaData.SchemaModel.FindClass(elementName);

                    break;

                case ThisDocument.DataViewType:
                    // elementName is a data view name
                    metaDataElement = (DataViewModel)_metaData.DataViews[elementName];

                    break;

                case ThisDocument.TaxonType:
                    // elementName is a taxon name
                    TaxonomyModel taxonomy = (TaxonomyModel)_metaData.Taxonomies[taxonomyName];
                    if (taxonomy != null)
                    {
                        metaDataElement = taxonomy.FindNode(elementName);
                    }

                    break;
            }

            return metaDataElement;
        }

        /// <summary>
        /// Gets the information indicating whether a view node represents a class that is related to the
        /// class represented by a family node
        /// </summary>
        /// <param name="familyNode">The family xml node</param>
        /// <param name="viewNode">A view node</param>
        /// <returns>true if it is related, false otherwise.</returns>
        private bool IsFamilyRelated(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            bool status = false;
            string caption = null;

            string elementName = GetAttributeValue(viewNode, ThisDocument.ElementAttribute, false);
            string elementType = GetAttributeValue(viewNode, ThisDocument.TypeAttribute, true);
            string taxonomyName = GetAttributeValue(viewNode, ThisDocument.TaxonomyAttribute, false);

            IMetaDataElement metaDataElement = GetViewNodeMetaDataElement(elementName, elementType, taxonomyName);

            Stack<string> path = new Stack<string>();
            status = IsFamilyRelated(familyNode, metaDataElement, out caption, path);

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the class to be inserted is related to
        /// the base class of the family node through relationship chain.
        /// </summary>
        /// <param name="familyNode">The family xml node</param>
        /// <param name="selectedMetaDataElement">A meta data element representing the class to be inserted</param>
        /// <returns>true if it is related, false otherwise.</returns>
        private bool IsFamilyRelated(Word.XMLNode familyNode, IMetaDataElement selectedMetaDataElement, out string baseClassCaption, Stack<string> path)
        {
            bool status = false;
            string insertingClassName = null;
            string familyBaseClassName = GetViewClassName(familyNode, out baseClassCaption);

            if (selectedMetaDataElement != null)
            {
                if (selectedMetaDataElement is ClassElement)
                {
                    insertingClassName = ((ClassElement)selectedMetaDataElement).Name;
                }
                else if (selectedMetaDataElement is DataViewModel)
                {
                    insertingClassName = ((DataViewModel)selectedMetaDataElement).BaseClass.Name;
                }
                else if (selectedMetaDataElement is TaxonNode)
                {
                    DataViewModel dataView = ((TaxonNode)selectedMetaDataElement).GetDataView(null);
                    if (dataView != null)
                    {
                        insertingClassName = dataView.BaseClass.Name;
                    }
                }
            }

            if (familyBaseClassName != null && insertingClassName != null)
            {
                if (familyBaseClassName != insertingClassName)
                {
                    // check to see if the inserting class is a related class through relationship attributes
                    // this method is called recursively to travel down the relationship chain
                    status = IsRelatedClass(familyBaseClassName, insertingClassName, null, 0, path);
                }
                else
                {
                    // inserting class is the same as the base class of the family node
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets information indicating whether two classes (base and related classes) are related through relationships.
        /// This method is called recursively to travel down the relationship chain until the related class in question
        /// is reached, or exhausted all the possible paths, or max length of travelling is reached
        /// </summary>
        /// <param name="currentClassName">The class name which is currently used as a base class</param>
        /// <param name="relatedClassName">The class name to see if it is related to the current base class</param>
        /// <param name="parentClassName">The name of class where it comes from</param>
        /// <param name="level">The level of recursive method call</param>
        /// <returns></returns>
        private bool IsRelatedClass(string baseClassName, string theClassName, string parentClassName, int level, Stack<string> path)
        {
            bool status = false;

            ClassElement classElement = _metaData.SchemaModel.FindClass(baseClassName);

            while (classElement != null)
            {
                foreach (RelationshipAttributeElement relationship in classElement.RelationshipAttributes)
                {
                    // do not trace the relationship that linked back to class where we come from
                    if (parentClassName == null ||
                        parentClassName != relationship.LinkedClass.Name)
                    {
                        path.Push(classElement.Name + ":" + relationship.Name);
                        ClassElement linkedClass = relationship.LinkedClass;
                        if (linkedClass.Name == theClassName || IsParentOf(linkedClass.Name, theClassName))
                        {
                            status = true;
                            break;
                        }
                        else if (level < LevelLimit) // to prevent dead loop in case there is a circular relationship
                        {
                            // travel down the relaionship chain
                            status = IsRelatedClass(linkedClass.Name, theClassName, baseClassName, level + 1, path);
                            if (status)
                            {
                                break;
                            }
                        }
                    }
                }

                // go up class hierarchy
                classElement = classElement.ParentClass;
            }

            if (!status && path.Count > 0)
            {
                // it's a wrong path, pop the relationhsip
                path.Pop();
            }

            return status;
        }

        private bool IsParentOf(string parentClassName, string childClassName)
        {
            bool status = false;

            ClassElement childClassElement = _metaData.SchemaModel.FindClass(childClassName);
            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                status = true;
            }

            return status;
        }

        private Word.Table FindNestedTable(Word.Table parentTable, DataTable arrayData)
        {
            Word.Table foundTable = null;
            for (int col = 0; col < parentTable.Columns.Count; col++)
            {
                for (int row = 0; row < parentTable.Rows.Count; row++)
                {
                    if (parentTable.Cell(row + 1, col + 1).Tables.Count == 1)
                    {
                        Word.Table nestedTable = parentTable.Cell(row + 1, col + 1).Tables[1];
                        if (IsArrayTable(nestedTable, arrayData))
                        {
                            foundTable = nestedTable;
                            break;
                        }
                        else
                        {
                            foundTable = FindNestedTable(nestedTable, arrayData);
                        }
                    }

                    if (foundTable != null)
                    {
                        break;
                    }
                }
            }

            return foundTable;
        }

        private bool IsArrayTable(Word.Table table, DataTable arrayData)
        {
            bool status = true;

            for (int col = 0; col < table.Columns.Count; col++)
            {
                string colName = table.Cell(1, col + 1).Range.Text;
                // colname may end with \r\a, trim it
                int pos = colName.IndexOf("\r");
                if (pos > 0)
                {
                    colName = colName.Substring(0, pos);
                }

                // if a table column doesn't exist in array data's column, then it is not table for the array attribute
                if (arrayData.Columns[colName] == null)
                {
                    status = false;
                    break;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the base class name that is associated with a family node
        /// </summary>
        /// <param name="familyNode">The family node</param>
        /// <returns>The base class name of family node</returns>
        private string GetViewClassName(Word.XMLNode familyNode, out string baseClassCaption)
        {
            string className = null;
            DataViewModel dataView = null;
            baseClassCaption = "";

            string elementName = GetAttributeValue(familyNode, ThisDocument.ElementAttribute, false);
            string elementType = GetAttributeValue(familyNode, ThisDocument.TypeAttribute, true);
            string taxonomyName = GetAttributeValue(familyNode, ThisDocument.TaxonomyAttribute, false);

            switch (elementType)
            {
                case ThisDocument.ClassType:
                    // elementName is a class name
                    ClassElement classElement = _metaData.SchemaModel.FindClass(elementName);
                    if (classElement != null)
                    {
                        className = classElement.Name;
                        baseClassCaption = classElement.Caption;
                    }

                    break;

                case ThisDocument.DataViewType:
                    // elementName is a data view name
                    dataView = (DataViewModel)_metaData.DataViews[elementName];
                    if (dataView != null)
                    {
                        className = dataView.BaseClass.Name;
                        baseClassCaption = dataView.BaseClass.Caption;
                    }

                    break;

                case ThisDocument.TaxonType:
                    // elementName is a taxon name
                    TaxonomyModel taxonomy = (TaxonomyModel)_metaData.Taxonomies[taxonomyName];
                    if (taxonomy != null)
                    {
                        TaxonNode taxon = taxonomy.FindNode(elementName);
                        if (taxon != null)
                        {
                            dataView = taxon.GetDataView(null);
                            if (dataView != null)
                            {
                                className = dataView.BaseClass.Name;
                                baseClassCaption = dataView.BaseClass.Caption;
                            }
                        }
                    }

                    break;
            }

            return className;
        }

        /// <summary>
        /// Gets the data view object associted with a view node
        /// </summary>
        /// <param name="viewNode">The view node</param>
        /// <returns>The data view associated with a view node</returns>
        private DataViewModel GetViewNodeDataView(Word.XMLNode viewNode)
        {
            DataViewModel dataView = null;

            string elementName = GetAttributeValue(viewNode, ThisDocument.ElementAttribute, false);
            string elementType = GetAttributeValue(viewNode, ThisDocument.TypeAttribute, true);
            string taxonomyName = GetAttributeValue(viewNode, ThisDocument.TaxonomyAttribute, false);

            IMetaDataElement metaDataElement = GetViewNodeMetaDataElement(elementName, elementType, taxonomyName);

            if (metaDataElement != null)
            {

                if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, (IXaclObject)metaDataElement, XaclActionType.Read))
                {
                    string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.NoReadPermission"), metaDataElement.Caption);
                    throw new Exception(msg);
                }

                switch (elementType)
                {
                    case ThisDocument.ClassType:
                        // elementName is a class name
                        dataView = _metaData.GetDetailedDataView(elementName);

                        break;

                    case ThisDocument.DataViewType:
                        // elementName is a data view name
                        dataView = (DataViewModel)_metaData.DataViews[elementName];

                        break;

                    case ThisDocument.TaxonType:
                        // elementName is a taxon name
                        TaxonomyModel taxonomy = (TaxonomyModel)_metaData.Taxonomies[taxonomyName];
                        if (taxonomy != null)
                        {
                            TaxonNode taxon = taxonomy.FindNode(elementName);
                            if (taxon != null)
                            {
                                dataView = taxon.GetDataView(null);
                            }
                        }

                        break;
                }
            }

            return dataView;
        }

        //by zhang.jingyuan
        private bool NeedDel(Word.XMLNode viewNode, Word.Table table)
        {
            bool _isNeedDel=true;

            try
            {
                int colCount = table.Rows[1].Cells.Count;
                int rowsCount = table.Rows.Count;
                int nodesCount = 0;

                for (int row = 2; row <= rowsCount; row++)
                {
                    if (table.Rows[row].Cells.Count != table.Rows[1].Cells.Count)
                    {
                        _isNeedDel = false;
                    }
                }

                foreach (Word.XMLNode propertyNode in viewNode.ChildNodes)
                {
                    nodesCount++;

                }

                if (nodesCount == (rowsCount - 1) * colCount)
                {
                    _isNeedDel = true;
                }
                else
                {
                    _isNeedDel = false;
                }
            }
            catch
            {
                _isNeedDel = false;
            }

            return _isNeedDel;
        }

        /// <summary>
        /// Populates a row in the table.
        /// </summary>
        /// <param name="viewNode">The view node that contains the table</param>
        /// <param name="rowId">row id</param>
        /// <param name="instanceView"></param>
        /// <param name="propertiesPerRow">Number of properties per row</param>
        private void PopulateTableRow(Word.XMLNode viewNode, int rowId, InstanceView instanceView, int propertiesPerRow)
        {
            int startPos = rowId * propertiesPerRow;
            int endPos = startPos + propertiesPerRow;
            int position = 0;
            string propertyName = null;
            StringCollection propertyNames = new StringCollection();

            // get the property names for XML node in the first row, since
            // the xml nodes in the new rows are created automatically, hence
            // do not have property name attribute
            int colIndex = 0;
            foreach (Word.XMLNode propertyNode in viewNode.ChildNodes)
            {
                propertyName = GetAttributeValue(propertyNode, ThisDocument.PropertyNameAttribute, true);
                propertyNames.Add(propertyName);
                colIndex++;
                if (colIndex >= propertiesPerRow)
                {
                    break;
                }
            }

            // populate each individual property
            colIndex = 0;
            Word.XMLNode attributeNode;
            foreach (Word.XMLNode childNode in viewNode.ChildNodes)
            {
                if (position >= startPos && position < endPos)
                {
                    propertyName = null;
                    try
                    {
                        propertyName = GetAttributeValue(childNode, ThisDocument.PropertyNameAttribute, true);
                    }
                    catch
                    {
                        // add property name attribute to the property node
                        propertyName = propertyNames[colIndex];
                        attributeNode = childNode.Attributes.Add(ThisDocument.PropertyNameAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                        attributeNode.NodeValue = propertyName;
                    }

                    if (childNode.BaseName == ThisDocument.PropertyNodeName)
                    {
                        PopulatePropertyNode(childNode, propertyName, instanceView);
                    }
                    else if (childNode.BaseName == ThisDocument.ArrayNodeName)
                    {
                        PopulateArrayNode(childNode, propertyName, instanceView);
                    }

                    colIndex++;
                }

                position++;
            }
        }

        /// <summary>
        /// Populates a row in the array table.
        /// </summary>
        /// <param name="arrayNode">The array node that contains the table</param>
        /// <param name="rowId">row id</param>
        /// <param name="dataRow">An array data row</param>
        /// <param name="columnsPerRow">Number of columns per row</param>
        private void PopulateArrayTableRow(Word.XMLNode arrayNode, int rowId, DataRow dataRow, int columnsPerRow)
        {
            int startPos = rowId * columnsPerRow;
            int endPos = startPos + columnsPerRow;
            int position = 0;
            string columnName = null;
            StringCollection columnNames = new StringCollection();

            // get the column names for XML node in the first row, since
            // the xml nodes in the new rows are created automatically, hence
            // do not have column name attribute
            int colIndex = 0;
            foreach (Word.XMLNode columnNode in arrayNode.ChildNodes)
            {
                columnName = GetAttributeValue(columnNode, ThisDocument.ColumnNameAttribute, true);
                columnNames.Add(columnName);
                colIndex++;
                if (colIndex >= columnsPerRow)
                {
                    break;
                }
            }

            // populate each individual property
            colIndex = 0;
            Word.XMLNode attributeNode;
            foreach (Word.XMLNode childNode in arrayNode.ChildNodes)
            {
                if (position >= startPos && position < endPos)
                {
                    columnName = null;
                    try
                    {
                        columnName = GetAttributeValue(childNode, ThisDocument.ColumnNameAttribute, true);
                    }
                    catch
                    {
                        // add column name attribute to the property node
                        columnName = columnNames[colIndex];
                        attributeNode = childNode.Attributes.Add(ThisDocument.ColumnNameAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                        attributeNode.NodeValue = columnName;
                    }

                    if (childNode.BaseName == ThisDocument.ColumnNodeName)
                    {
                        PopulateColumnNode(childNode, columnName, dataRow);
                    }

                    colIndex++;
                }

                position++;
            }
        }

        /// <summary>
        /// Gets an InstanceView representing a data instance that is related to the data instances
        /// selected in the result grid.
        /// </summary>
        /// <param name="familyNode">The family node that contains the related view node</param>
        /// <param name="viewNode">The view node presenting a related view</param>
        /// <returns>An InstanceView representing a data instance</returns>
        private InstanceView GetInstanceView(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            InstanceView instanceView = null;

            DataViewModel instanceDataView = GetDataView(familyNode, viewNode);
            if (instanceDataView != null)
            {

                string query = instanceDataView.SearchQuery;

                int totalCount = GetCMDataWebService().ExecuteCount(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                    query);

                if (totalCount > PopulateLineLimit)
                {
                    string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.TooManyInstances"), instanceView.DataView.BaseClass.Caption, totalCount, PopulateLineLimit);
                    throw new Exception(msg);
                }

                // get the result in paging mode, the database connection is released at BeginQueryDone method when done
                string queryId = GetCMDataWebService().BeginQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), query, _pageSize);

                DataSet masterDataSet = null;

                DataSet slaveDataSet;
                int currentPageIndex = 0;
                int instanceCount = 0;
                int start, end;
                XmlReader xmlReader;
                XmlNode xmlNode;
                while (instanceCount < totalCount && !_isCancelled)
                {
                    start = currentPageIndex * _pageSize + 1;
                    end = start + this._pageSize - 1;
                    if (end > totalCount)
                    {
                        end = totalCount;
                    }

                    // invoke the web service synchronously to get data in pages
                    xmlNode = GetCMDataWebService().GetNextResult(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        queryId);

                    if (xmlNode == null)
                    {
                        // end of result
                        break;
                    }

                    slaveDataSet = new DataSet();

                    xmlReader = new XmlNodeReader(xmlNode);
                    slaveDataSet.ReadXml(xmlReader);

                    instanceCount += slaveDataSet.Tables[instanceDataView.BaseClass.Name].Rows.Count;

                    if (masterDataSet == null)
                    {
                        // first page
                        masterDataSet = slaveDataSet;
                        masterDataSet.EnforceConstraints = false;
                    }
                    else
                    {
                        // append to the master dataset
                        AppendDataSet(instanceDataView.BaseClass.Name, masterDataSet, slaveDataSet);
                    }

                    currentPageIndex++;
                }

                if (!this._isCancelled &&
                    masterDataSet != null &&
                    !DataSetHelper.IsEmptyDataSet(masterDataSet, instanceDataView.BaseClass.ClassName))
                {
                    instanceView = new InstanceView(instanceDataView, masterDataSet);
                    instanceView.SelectedIndex = 0;
                }
                else
                {
                    instanceView = null;
                }
            }

            return instanceView;
        }

        /// <summary>
        /// Gets an InstanceView representing the detailed instance data for
        /// the currently selected row in the datagrid.
        /// </summary>
        /// <returns>An InstanceView representing the detailed instance for the currently selected row in the datagrid</returns>
        private InstanceView GetInstanceView()
        {
            return GetInstanceView(-1);
        }

        /// <summary>
        /// Gets an InstanceView representing the instance data of
        /// the given row index in the datagrid.
        /// </summary>
        /// <param name="rowIndex">The row index, -1 for the currently selected index.</param>
        /// <returns>An InstanceView representing the data instance for the specified row</returns>
        private InstanceView GetInstanceView(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                this.resultDataControl.SetCurrentSlideSelectedRowIndex(rowIndex);
            }

            if (this.resultDataControl.CurrentSlide.InstanceView == null)
            {
                if (this.resultDataControl.CurrentSlide.InstanceDataView == null)
                {
                    // uses the same dataview of the data slide
                    this.resultDataControl.CurrentSlide.InstanceDataView = this.resultDataControl.CurrentSlide.DataView;
                }

                this.resultDataControl.CurrentSlide.InstanceView = new InstanceView(this.resultDataControl.CurrentSlide.InstanceDataView,
                    this.resultDataControl.CurrentSlide.DataSet);
            }

            // this assign the selected instance to the InstanceView object
            this.resultDataControl.CurrentSlide.InstanceView.SelectedIndex = this.resultDataControl.CurrentSlide.SelectedRowIndex;

            return this.resultDataControl.CurrentSlide.InstanceView;
        }

        /// <summary>
        /// Gets a DataView representing a view node that is related to the base class.
        /// </summary>
        /// <param name="familyNode">The family node that contains the related view node</param>
        /// <param name="viewNode">The view node presenting a related view</param>
        /// <returns>A DataViewModel object representing a view node that is related to the base class</returns>
        private DataViewModel GetDataView(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            DataViewModel dataView = null;

            // get the data view associated with a view node
            dataView = GetViewNodeDataView(viewNode);

            if (dataView != null)
            {
                // add relationship path and search value to the data view so that it will yield the
                // data instances that are related to the selected data instance in the result grid
                string baseClassCaption = null;
                string baseClassName = GetViewClassName(familyNode, out baseClassCaption);
                string path = GetAttributeValue(viewNode, ThisDocument.PathAttribute, false);
                AddDataViewConstraint(dataView, baseClassName, path);
            }

            return dataView;
        }

        /// <summary>
        /// Add necessary referenced classes and search criteria to the data view so that it can retrieve
        /// the data instances that are related to the selected data instances in the result grid.
        /// </summary>
        /// <param name="dataView">A data view for a view node within a family node</param>
        /// <param name="baseClassName">The base class name represented by a family node</param>
        /// <param name="path">Path of relationships between the data view and the base class</param>
        private void AddDataViewConstraint(DataViewModel dataView, string baseClassName, string path)
        {
            // Get relationship path between the data view and the base class,
            // starting from the data view side
            string[] relationshipNames = null;
            if (path != null && path.Length > 0)
            {
                relationshipNames = path.Split(';');
            }

            if (relationshipNames.Length > 0)
            {
                ClassElement classElement = _metaData.SchemaModel.FindClass(dataView.BaseClass.Name);
                DataClass referringClass = dataView.BaseClass;

                // add the referenced classes to the data view according to the path
                bool isCompleted = true;
                bool found = false;
                for (int i = 0; i < relationshipNames.Length; i++)
                {
                    int pos = relationshipNames[i].IndexOf(":");
                    string ownerClassName = relationshipNames[i].Substring(0, pos);
                    string relationshipName = relationshipNames[i].Substring(pos + 1);

                    found = false;
                    while (classElement != null)
                    {
                        foreach (RelationshipAttributeElement relationship in classElement.RelationshipAttributes)
                        {
                            if (relationship.BackwardRelationshipName == relationshipName &&
                                relationship.LinkedClassName == ownerClassName)
                            {
                                found = true; // found relationship

                                referringClass = AddReferencedClass(dataView, relationship, referringClass);

                                // be ready to move to match the next relationship in the path
                                classElement = relationship.LinkedClass;

                                break;
                            }
                        }

                        if (!found)
                        {
                            // go up class hierarchy
                            classElement = classElement.ParentClass;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                    else if (classElement == null)
                    {
                        // unable to find the relationship
                        isCompleted = false;
                        break;
                    }
                }

                if (isCompleted && referringClass != null)
                {
                    // if the process above is completed, the referringClass should represent
                    // the base class, set the corresponding obj_id as search value
                    RelationshipAttributeElement relationshipAttribute = referringClass.ReferringRelationship.BackwardRelationship;

                    // build a search expression that retrieve the data instances of the related class that
                    // are associated with the selected data instance in the result grid
                    string objId = null;
                    InstanceView instanceView = GetInstanceView(); // the instance view represent the currently selected data instance
                    IDataViewElement expr = null;
                    DataSimpleAttribute left;
                    Parameter right;

                    if (relationshipAttribute.IsForeignKeyRequired)
                    {
                        // many-to-one relationship, gets the obj_id of the referenced instance from the table
                        if (instanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(dataView.BaseClass.Name, relationshipAttribute.Name)] != null)
                        {
                            if (instanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(dataView.BaseClass.Name, relationshipAttribute.Name)].Rows[this.resultDataControl.CurrentSlide.SelectedRowIndex].IsNull(NewteraNameSpace.OBJ_ID) == false)
                            {
                                objId = instanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(dataView.BaseClass.Name, relationshipAttribute.Name)].Rows[this.resultDataControl.CurrentSlide.SelectedRowIndex][NewteraNameSpace.OBJ_ID].ToString();
                            }

                            if (string.IsNullOrEmpty(objId))
                            {
                                objId = "0";
                            }
                        }

                        left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.ReferringClassAlias);
                        right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.ReferringClassAlias, DataType.String);

                        right.ParameterValue = objId;
                        expr = new RelationalExpr(ElementType.Equals, left, right);
                    }
                    else
                    {
                        // one-to-many relationship, get the obj_id of the selected data instance
                        objId = instanceView.InstanceData.ObjId;

                        left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.Alias);
                        right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.Alias, DataType.String);

                        right.ParameterValue = objId;
                        expr = new RelationalExpr(ElementType.Equals, left, right);
                    }

                    if (expr != null)
                    {
                        // add search expression to the dataview
                        dataView.AddSearchExpr(expr, ElementType.And);
                    }
                }
            }
        }

        private DataClass AddReferencedClass(DataViewModel dataView, RelationshipAttributeElement relationship, DataClass referringClass)
        {
            DataClass referencedClass = null;
            // check to see if the class has been added as a referenced class
            bool existed = false;
            foreach (DataClass refClass in dataView.ReferencedClasses)
            {
                if (refClass.ReferringClassAlias == referringClass.Alias &&
                    refClass.ReferringRelationshipName == relationship.Name)
                {
                    existed = true;
                    referencedClass = refClass;
                    break;
                }
            }

            if (!existed)
            {
                // create a referenced class and add it to data view
                // Add the linked class as a referenced class to data view
                referencedClass = new DataClass(relationship.LinkedClassAlias,
                    relationship.LinkedClassName, DataClassType.ReferencedClass);
                referencedClass.ReferringClassAlias = referringClass.Alias;
                referencedClass.ReferringRelationshipName = relationship.Name;
                referencedClass.Caption = relationship.LinkedClass.Caption;
                referencedClass.ReferringRelationship = relationship;
                dataView.ReferencedClasses.Add(referencedClass);
            }

            return referencedClass;
        }

        /// <summary>
        /// Get value of the specified attribute from a XMLNode
        /// </summary>
        /// <param name="xmlNode">The XMLNode</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="isRequired">indicating whether the attribute is a required attribute</param>
        /// <returns>attribute value</returns>
        /// <exception cref="Exception">Thrown if it is unable to find a required attribute</exception>
        private string GetAttributeValue(Word.XMLNode xmlNode, string attributeName, bool isRequired)
        {
            string val = null;
            foreach (Word.XMLNode attr in xmlNode.Attributes)
            {
                if (attr.BaseName == attributeName)
                {
                    val = attr.NodeValue;
                }
            }

            if (val == null && isRequired)
            {
                throw new Exception("Unable to find attribute " + attributeName + " in xml node " + xmlNode.BaseName);
            }

            return val;
        }

        #endregion private methods

        #region IDataGridControl members

        /// <summary>
        /// Fire the graph event
        /// </summary>
        public void FireGraphEvent()
        {
            ScientificGraphDialog dialog = new ScientificGraphDialog();
            dialog.WorkingChartDef = _graphWizard.WorkingChartDef;

            dialog.ShowDialog();
        }

        /// <summary>
        /// Fire the download graph event
        /// </summary>
        public void FireDownloadGraphEvent(string formatName, string suffix)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = formatName + "|*." + suffix;
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = false;
            saveFileDialog.FileName = "graph." + suffix;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _graphFileName = saveFileDialog.FileName;

                DownloadGraphFile(formatName);
            }
        }

        /// <summary>
        /// Create a web service proxy for chart related services
        /// </summary>
        /// <returns></returns>
        public Newtera.DataGridActiveX.ActiveXControlWebService.ActiveXControlService CreateActiveXControlWebService()
        {
            ActiveXControlService webService = new ActiveXControlService();

            // get the web service url from the user's config file
            string url = SmartWord.Properties.Settings.Default.SmartWord_ActiveXControlWebService_ActiveXControlService;

            webService.Url = url;

            webService.Timeout = -1; // wait infinitly

            return webService;
        }

        /// <summary>
        /// Gets the base class name of data instances currently displayed in the datagrid 
        /// </summary>
        public string BaseClassName
        {
            get
            {

                if (this.resultDataControl.CurrentSlide != null)
                {
                    return this.resultDataControl.CurrentSlide.DataView.BaseClass.Name;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the name of Table indicating which table in the DataSet is of interesting
        /// </summary>
        public string TableName
        {
            get
            {

                if (this.resultDataControl.CurrentSlide != null)
                {
                    return this.resultDataControl.CurrentSlide.DataView.BaseClass.Name;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the DataGrid control
        /// </summary>
        public DataGrid TheDataGrid
        {
            get
            {
                return this.resultDataControl.TheDataGrid;
            }
        }

        /// <summary>
        /// Gets the DataView that is in the same order as what displayed on the datagrid
        /// </summary>
        public DataView DataView
        {
            get
            {
                if (this.resultDataControl.CurrentSlide != null &&
                    this.resultDataControl.CurrentSlide.DataSet != null &&
                    this.resultDataControl.CurrentSlide.DataSet.Tables[TableName] != null)
                {
                    return this.resultDataControl.CurrentSlide.DataSet.Tables[TableName].DefaultView;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a collection of ColumnInfo objects that describe the columns in the datagrid
        /// </summary>
        /// <value>A ColumnInfoCollection</value>
        public ColumnInfoCollection ColumnInfos
        {
            get
            {
                ColumnInfoCollection columnInfos = null;
                string tableName = this.TableName;
                if (this.resultDataControl.CurrentSlide != null)
                {
                    ColumnInfo columnInfo;
                    columnInfos = new ColumnInfoCollection();

                    DataGridTableStyle tableStyle = this.resultDataControl.TheDataGrid.TableStyles[tableName];
                    foreach (DataGridColumnStyle columnStyle in tableStyle.GridColumnStyles)
                    {
                        if (columnStyle.HeaderText != null &&
                            columnStyle.HeaderText.Length > 0)
                        {
                            columnInfo = new ColumnInfo();
                            columnInfo.Caption = columnStyle.HeaderText;
                            columnInfo.Name = columnStyle.MappingName;
                            columnInfos.Add(columnInfo);
                        }
                    }
                }

                return columnInfos;
            }
        }

        /// <summary>
        /// Download a graph file
        /// </summary>
        private void DownloadGraphFile(string formatName)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                // invoke the web service synchronously
                AttachmentService attachmentService = new AttachmentService();
                attachmentService.GetChartFile(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                    formatName, null);

                if (attachmentService.ResponseSoapContext.Attachments.Count == 1)
                {
                    DimeAttachment dime = attachmentService.ResponseSoapContext.Attachments[0];

                    FileStream oStream = File.OpenWrite(this._graphFileName);
                    using (BinaryWriter streamWriter = new BinaryWriter(oStream))
                    {
                        BinaryReader streamReader = new BinaryReader(dime.Stream);
                        int actual = 0;
                        byte[] cbuffer = new byte[1000];
                        while ((actual = streamReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                        {
                            streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
                            streamWriter.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// Show all instances of the currently displayed view in the datagrid
        /// </summary>
        private void ShowAllInstances()
        {
            // to get all instances, we need to get the total count
            if (this.resultDataControl.CurrentSlide.TotalInstanceCount < 0)
            {
                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataView.MissingInstanceCount"),
                    "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                GetAllInstances();
            }
        }

        /// <summary>
        /// Gets all instances for the currently displayed data view
        /// </summary>
        private void GetAllInstances()
        {
            if (this.resultDataControl.CurrentSlide.TotalInstanceCount > DataThreshhold)
            {
                string msg = Newtera.WindowsControl.MessageResourceManager.GetString("DataView.LargeData");
                int totalCount = this.resultDataControl.CurrentSlide.TotalInstanceCount;
                msg = String.Format(msg, totalCount, totalCount / 1000);
                if (MessageBox.Show(msg,
                    "Confirm", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    // user do not want to load the data for now
                    return;
                }
            }

            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                if (this.resultDataControl.CurrentSlide.DataView != null)
                {
                    string query = this.resultDataControl.CurrentSlide.DataView.SearchQuery;

                    _isRequestComplete = false;
                    _isCancelled = false;

                    // invoke the web service asynchronously
                    GetCMDataWebService().BeginQueryAsync(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        query,
                        _pageSize);

                    // launch a work in progress dialog
                    _workInProgressDialog = new WorkInProgressDialog();
                    this._workInProgressDialog.MaximumSteps = this.resultDataControl.CurrentSlide.TotalInstanceCount / _pageSize + 1;
                    this._workInProgressDialog.Value = 1;
                    this._workInProgressDialog.IsTimerEnabled = false;
                    ShowWorkingDialog(true, new MethodInvoker(CancelGettingAllInstances));
                }
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for BeginQuery web service method.
        /// </summary>
        /// <param name="res">The result</param>
        private void BeginQueryDone(Object source, SmartWord.CMDataWebService.BeginQueryCompletedEventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            string queryId = null;
            try
            {
                // Get the query id
                queryId = e.Result;

                DataSet masterDataSet = null;

                this._workInProgressDialog.PerformStep(); // move one step forward

                // It is a Worker thread, continue getting rest of data
                DataSet slaveDataSet;
                int currentPageIndex = 0;
                int instanceCount = 0;
                int start, end;
                XmlReader xmlReader;
                XmlNode xmlNode;
                while (instanceCount < this.resultDataControl.CurrentSlide.TotalInstanceCount && !_isCancelled)
                {
                    start = currentPageIndex * _pageSize + 1;
                    end = start + this._pageSize - 1;
                    if (end > this.resultDataControl.CurrentSlide.TotalInstanceCount)
                    {
                        end = this.resultDataControl.CurrentSlide.TotalInstanceCount;
                    }

                    //this._workInProgressDialog.DisplayText = Newtera.WindowsControl.MessageResourceManager.GetString("DataView.GettingData") + " " + start + ":" + end;
                    SetWorkingDialogDisplayText(Newtera.WindowsControl.MessageResourceManager.GetString("DataView.GettingData") + " " + start + ":" + end);
                    this._workInProgressDialog.PerformStep(); // move one step forward

                    // invoke the web service synchronously to get data in pages
                    xmlNode = GetCMDataWebService().GetNextResult(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        queryId);

                    if (xmlNode == null)
                    {
                        // end of result
                        break;
                    }

                    slaveDataSet = new DataSet();

                    xmlReader = new XmlNodeReader(xmlNode);
                    slaveDataSet.ReadXml(xmlReader);

                    instanceCount += slaveDataSet.Tables[this.resultDataControl.CurrentSlide.DataView.BaseClass.Name].Rows.Count;

                    if (masterDataSet == null)
                    {
                        // first page
                        masterDataSet = slaveDataSet;
                        masterDataSet.EnforceConstraints = false;
                    }
                    else
                    {
                        // append to the master dataset
                        AppendDataSet(this.resultDataControl.CurrentSlide.DataView.BaseClass.Name, masterDataSet, slaveDataSet);
                    }

                    currentPageIndex++;
                }

                ShowQueryResult(masterDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // finish the query
                if (queryId != null)
                {
                    GetCMDataWebService().EndQuery(queryId);
                }

                //Bring down the work in progress dialog
                HideWorkingDialog();
            }
        }

        /// <summary>
        /// Append a slave DataSet to a master dataset
        /// </summary>
        /// <param name="master">The master DataSet</param>
        /// <param name="slave">The slave DataSet</param>
        private void AppendDataSet(string tableName, DataSet master, DataSet slave)
        {
            DataTable masterTable = master.Tables[tableName];
            DataTable slaveTable = slave.Tables[tableName];

            foreach (DataRow row in slaveTable.Rows)
            {
                masterTable.ImportRow(row);
            }
        }

        private delegate void ShowQueryResultDelegate(DataSet dataSet);

        /// <summary>
        /// Display the query result data set in a data grid
        /// </summary>
        /// <param name="dataSet">The query result in DataSet</param>
        private void ShowQueryResult(DataSet dataSet)
        {
            if (this.InvokeRequired == false)
            {
                // it is the UI thread, continue

                this.resultDataControl.ShowQueryResult(dataSet);
                this._menuItemStates.SetState(MenuItemID.ViewNextPage, false);

                // clear the InstanceView which may contains the previous DataSet
                this.resultDataControl.CurrentSlide.InstanceView = null;
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                ShowQueryResultDelegate showQueryResult = new ShowQueryResultDelegate(ShowQueryResult);

                this.BeginInvoke(showQueryResult, new object[] { dataSet });
            }
        }

        /// <summary>
        /// Cancel the lengthy in a worker thread
        /// </summary>
        private void CancelGettingAllInstances()
        {
            this._isCancelled = true;
        }

        /// <summary>
        /// Clear the data that is currently displayed
        /// </summary>
        private void ClearDisplay()
        {
            _selectedMetaDataElement = null;
            _dataView = null;
            this.resultDataControl.ClearDataGrids();
            this.attributeListView.Items.Clear();
            this.showInstanceButton.Enabled = false;
            this.insertXmlNodeButton.Enabled = false;
            this.insertInstanceButton.Enabled = false;
        }

        #endregion

        #region event handlers

        private void databaseComboBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_schemaInfos == null)
            {
                _schemaInfos = GetSchemaInfos();

                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

                // display the schemas in the dropdown list
                if (_schemaInfos != null)
                {
                    string[] schemaIds = new string[_schemaInfos.Length];
                    int index = 0;
                    foreach (Newtera.Common.Core.SchemaInfo schemaInfo in _schemaInfos)
                    {
                        schemaIds[index++] = schemaInfo.NameAndVersion;
                    }

                    this.databaseComboBox.DataSource = schemaIds;
                }
            }
        }

        private void databaseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            if (this.databaseComboBox.SelectedIndex >= 0)
            {
                Newtera.Common.Core.SchemaInfo selectedSchemaInfo = _schemaInfos[this.databaseComboBox.SelectedIndex];

                _metaData = GetMetaDataModel(selectedSchemaInfo);

                _selectedMetaDataElement = null;
                _dataView = null;

                ShowSchemaTree();

                InitializeMenuItemStates();

                this.resultDataControl.MetaData = _metaData;
                this.resultDataControl.ClearDataGrids();
                this.attributeListView.Items.Clear();
                this.showInstanceButton.Enabled = false;
                this.insertInstanceButton.Enabled = false;
                this.insertXmlNodeButton.Enabled = true;
            }
        }

        private void classTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            // Get the selected node
            MetaDataTreeNode node = (MetaDataTreeNode)e.Node;

            if (node.MetaDataElement != null)
            {
                if (node.MetaDataElement is ClassElement)
                {
                    ClassElement theClass = node.MetaDataElement as ClassElement;

                    if (theClass.Name != _rootClass)
                    {
                        _dataView = _metaData.GetDetailedDataView(theClass.Name);
                        if (_dataView != null)
                        {
                            // Show existing attributes of a data view
                            ShowDataViewAttributes(theClass);
                        }
                    }
                    _dataView.ClearSearchExpression();
                }
                else if (node.MetaDataElement is DataViewModel)
                {
                    _dataView = (DataViewModel)node.MetaDataElement;

                    // Show existing attributes of the data view
                    ShowDataViewAttributes(node.MetaDataElement);
                }
                else if (node.MetaDataElement is ITaxonomy)
                {
                    _dataView = ((ITaxonomy)node.MetaDataElement).GetDataView("Detailed");
                    if (_dataView != null)
                    {
                        // Show existing attributes of a data view
                        ShowDataViewAttributes(node.MetaDataElement);
                    }
                    else
                    {
                        // selected node doesn't have an associated data view, clear the previous data
                        ClearDisplay();
                    }
                }
                else
                {
                    // selected node doesn't have an associated data view, clear the previous data
                    ClearDisplay();
                }
            }
            else
            {
                // selected node doesn't have an associated data view, clear the previous data
                ClearDisplay();
            }
        }

        private void insertXmlNodeButton_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            InsertXMLNode();
        }

        private void showInstanceButton_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            if (_selectedMetaDataElement != null && _dataView != null)
            {
                this.resultDataControl.Slides.Clear();
                this.resultDataControl.CurrentSlide = new DataViewSlide(_menuItemStates);
                if(this.resultDataControl.CurrentSlide.DataView!=null)
                    this.resultDataControl.CurrentSlide.DataView.ClearSearchExpression();
                this.resultDataControl.CurrentSlide.DataView = _dataView;
                this.resultDataControl.CurrentSlide.InstanceDataView = _dataView;
                // clear the search expression in the default data view, so that
                // users can build their own search expression using search
                // expression builder from the scratch
                //this.resultDataControl.CurrentSlide.DataView.ClearSearchExpression();
                // get the instance data
                ExecuteQuery();
            }
        }

        private void insertInstanceButton_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            PopulateToSelectedNode();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            ShowSearchDialog();
        }

        /// Get data of an instance of the selected row in 
		/// and display it in the instance detail dialog
        private void detailButton_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            InstanceView instanceView = GetInstanceView();
            if (instanceView != null)
			{
                // show the instance data in a dialog
                InstanceDetailDialog dialog = new InstanceDetailDialog();
                dialog.InstanceView = instanceView;
                dialog.ShowDialog();
			}
        }

        private void resultDataControl_RequestForCountEvent(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            ExecuteQueryCount();
        }

        private void resultDataControl_RequestForDataEvent(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            ExecuteQuery();
        }

        private void resultDataControl_RowSelectedIndexChangedEvent(object sender, EventArgs e)
        {
        }

        private void chartButton_Click(object sender, EventArgs e)
        {
            _graphWizard = new GraphWizard();
            _graphWizard.DataGridControl = this;
            _graphWizard.IsForWindowsClient = true;
            _graphWizard.ConnectionString = ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo);

            _graphWizard.ShowDialog();
        }

        private void setupServerURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupServerURLDialog dialog = new SetupServerURLDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // the URL may be changed, clear the old web service so that
                // it will be created again using the new URL
                _dataService = null;
            }
        }

        private void getAllInstancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            ShowAllInstances();
        }

        private void smartWordLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            ClientLicenseInfoDialog dialog = new ClientLicenseInfoDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void attributeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.attributeListView.SelectedIndices.Count == 1)
            {
                int index = this.attributeListView.SelectedIndices[0];
                IDataViewElement attribute = this._classAttributes[index];
                if (attribute is DataRelationshipAttribute &&
                    this._selectedXmlNode != null &&
                    this._selectedXmlNode.BaseName == ThisDocument.ViewNodeName)
                {
                    this.insertXmlNodeButton.Enabled = false;
                }
                else
                {
                    this.insertXmlNodeButton.Enabled = true;
                }
            }
        }

        private void pivotButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                PivotGridDialog pivotGridDialog = new PivotGridDialog();

                pivotGridDialog.DataGridControl = this;
                pivotGridDialog.PivotLayoutManager = new PivotLayoutManager(_metaData);

                pivotGridDialog.ShowDialog();
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        #endregion event handlers
    }
}
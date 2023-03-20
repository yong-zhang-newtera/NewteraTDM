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
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;
using Newtera.WindowsControl.Chart;
using Newtera.DataGridActiveX;
using Newtera.SmartWordUtil;
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
        private string _xmlSchemaViewName = null;
        private WorkInProgressDialog _workInProgressDialog;
        private DocDataSourceClient _dataSource;
        private XMLSchemaDialog _xmlSchemaDialog;
        private string _insertXMLNodeButtonText;

        internal object _selectedRange = System.Type.Missing;
        private object objMissing = System.Type.Missing;
        private int _pageSize = 100;

        public NavigationControl()
        {
            InitializeComponent();

            _menuItemStates = new MenuItemStates();
            this.resultDataControl.MenuItemStates = _menuItemStates;
            this.resultDataControl.UserManager = new WindowClientUserManager();
            _workInProgressDialog = null;
            _xmlSchemaDialog = null;
            _insertXMLNodeButtonText = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertDatabaseNode");

            _dataSource = new DocDataSourceClient(this.resultDataControl);

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

        /// <summary>
        /// Gets the currently selected xml node in the word doc 
        /// </summary>
        internal Word.XMLNode SelectedXmlNode
        {
            get
            {
                return _selectedXmlNode;
            }
        }

        // Get the text that displays on the insert xml node button
        internal string InsertXMLNodeButtonText
        {
            get
            {
                return _insertXMLNodeButtonText;
            }
        }

        internal void EnableInsertButton()
        {
            if (_selectedXmlNode == null)
            {
                // to enable the button for inserting a database node
                _insertXMLNodeButtonText = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertDatabaseNode");
                insertXmlNodeButton.Text = _insertXMLNodeButtonText;
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
                _insertXMLNodeButtonText = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertViewNode");;
                insertXmlNodeButton.Text = _insertXMLNodeButtonText;

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
                    _insertXMLNodeButtonText = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertPropertyNode");
                    insertXmlNodeButton.Text = _insertXMLNodeButtonText;
                }
                else
                {
                    _insertXMLNodeButtonText = Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InsertViewNode");
                    insertXmlNodeButton.Text = _insertXMLNodeButtonText;
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
            IMetaDataElement selectedElement = _dataSource.GetViewNodeMetaDataElement(elementName, elementType, taxonomyName);

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

        internal void AttachCustomPrincipal()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal == null && _isAuthenticated)
            {
                // attach a custom principal object to the thread
                CustomPrincipal.Attach(new WindowClientUserManager(), new WindowClientServerProxy(), _userName);
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
                    WordPopulator wordPopulator = new WordPopulator(_dataSource);
                    if (!wordPopulator.IsFamilyRelated(this._selectedXmlNode, _selectedMetaDataElement, out baseClassCaption, path))
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
                        if (table.Rows.Count == 2)
                        {
                            /*
                            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InvalidTable"),
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                             */
                            containTable = true;
                        }
                    }
                    else if (range.Tables.Count > 1)
                    {
                        MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.TooManyTables"),
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    // ask for user whether to insert a view or a property
                    string nodeName = ThisDocument.ViewNodeName;
                    if (this.attributeListView.SelectedItems.Count == 1)
                    {
                        SelectViewOrPropertyDialog dialog = new SelectViewOrPropertyDialog();
                        if (dialog.ShowDialog() == DialogResult.Cancel)
                        {
                            return;
                        }
                        else
                        {
                            nodeName = dialog.InsertNodeName;
                        }
                    }

                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in _selectedXmlNode.ChildNodeSuggestions)
                    {
                        if (nodeSuggestion.BaseName == ThisDocument.ViewNodeName)
                        {
                            // insert a view node
                            xmlNode = nodeSuggestion.Insert(ref _selectedRange);
                            SetViewNodeAttributes(xmlNode, containTable, path);

                            if (nodeName == ThisDocument.PropertyNodeName &&
                                this.attributeListView.SelectedItems.Count == 1)
                            {
                                int index = this.attributeListView.SelectedIndices[0];
                                IDataViewElement attribute = this._classAttributes[index];
                                // do not insert relationship attribute
                                if (!(attribute is DataRelationshipAttribute))
                                {
                                    // in order to insert array or property node inside the view node
                                    // we need to change the selected range
                                    _selectedRange = xmlNode.Range;
                                    if (attribute is DataArrayAttribute)
                                    {
                                        // insert an array tage with an embedded table
                                        InsertArrayNode(xmlNode, attribute);
                                    }
                                    else
                                    {
                                        // insert a Property tag
                                        InsertPropertyNode(xmlNode, attribute);
                                    }
                                }
                            }
                        }
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
                                    InsertArrayNode(_selectedXmlNode, attribute);
                                }
                                else
                                {
                                    // insert a Property tag
                                    InsertPropertyNode(_selectedXmlNode, attribute);
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
        private void InsertPropertyNode(Word.XMLNode parentXmlNode, IDataViewElement attribute)
        {
            Word.XMLNode xmlNode = null;
            Word.XMLNode attributeNode;

            foreach (Word.XMLChildNodeSuggestion nodeSuggestion in parentXmlNode.ChildNodeSuggestions)
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
        private void InsertArrayNode(Word.XMLNode parentNode, IDataViewElement attribute)
        {
            Word.XMLNode xmlNode = null;
            Word.XMLNode attributeNode;

            foreach (Word.XMLChildNodeSuggestion nodeSuggestion in parentNode.ChildNodeSuggestions)
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
                //xmlNode.Range.SetRange(1, 5);
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
                        table.Cell(1, i + 1).Range.Text = "Column_" + i;
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
                WordPopulator wordPopulator = new WordPopulator(_dataSource);

                Cursor preCursor = Cursor.Current;

                try
                {
                    if (_selectedXmlNode.BaseName == ThisDocument.DatabaseNodeName)
                    {
                        wordPopulator.PopulateDatabaseNode(_selectedXmlNode);
                    }
                    else if (_selectedXmlNode.BaseName == ThisDocument.FamilyNodeName)
                    {
                        wordPopulator.PopulateFamilyNode(_selectedXmlNode);
                    }
                    else if (_selectedXmlNode.BaseName == ThisDocument.ViewNodeName)
                    {
                        wordPopulator.PopulateViewNode(_selectedXmlNode);
                    }
                }
                finally
                {
                    Cursor.Current = preCursor;
                }
            }
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

                            // Show existing xml schemas of the class
                            ShowXMLSchemaViews(theClass);
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

            InstanceView instanceView = _dataSource.GetInstanceView();
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

        private void showXMLSchemaButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_xmlSchemaViewName))
            {
                _xmlSchemaDialog = new XMLSchemaDialog();
                _xmlSchemaDialog.MetaData = _metaData;
                _xmlSchemaDialog.NavigationControl = this;

                _xmlSchemaDialog.SelectedXMLSchemaName = _xmlSchemaViewName;

                _xmlSchemaDialog.Show(this.FindForm());
            }
        }

        private void xmlSchemaListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.xmlSchemaListView.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)this.xmlSchemaListView.SelectedItems[0];

                _xmlSchemaViewName = item.SubItems[1].Text;

                this.showXMLSchemaButton.Enabled = true;
            }
            else
            {
                this._xmlSchemaViewName = null;
                this.showXMLSchemaButton.Enabled = false;
            }
        }

        #endregion event handlers

        private void ShowXMLSchemaViews(ClassElement baseClass)
        {
            // clear the data grid
            this.resultDataControl.ClearDataGrids();
            this.showXMLSchemaButton.Enabled = false;

            ListViewItem item;
            this.xmlSchemaListView.SuspendLayout();
            this.xmlSchemaListView.Items.Clear();

            int count = 0;
            foreach (XMLSchemaModel xmlSchemaModel in _metaData.XMLSchemaViews)
            {
                if (xmlSchemaModel.RootElement.ElementType == baseClass.Name)
                {
                    item = new ListViewItem(xmlSchemaModel.Caption);
                    item.SubItems.Add(xmlSchemaModel.Name);
                    item.ImageIndex = 0;
                    this.xmlSchemaListView.Items.Add(item);
                    count++;
                }
            }

            this.xmlSchemaListView.ResumeLayout();

            this.showXMLSchemaButton.Enabled = false;
        }
    }
}
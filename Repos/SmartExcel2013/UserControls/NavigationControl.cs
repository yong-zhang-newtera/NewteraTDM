using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using Excel = Microsoft.Office.Interop.Excel;
using Missing = System.Reflection.Missing;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Common.MetaData.Principal;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;
using Newtera.SmartWordUtil;

namespace SmartExcel2013
{
    public partial class NavigationControl : UserControl
    {
        private const int DataThreshhold = 10000;

        private Newtera.Common.Core.SchemaInfo[] _schemaInfos = null;
        private MetaDataModel _metaData = null;
        private string _rootClass = "ALL";
        private IMetaDataElement _selectedMetaDataElement = null;
        private DataViewModel _dataView;
        private CMDataServiceStub _dataService;
        private MenuItemStates _menuItemStates;
        private bool _isAuthenticated = false;
        private string _userName = null;
        private MetaDataTreeNode _treeRoot = null;
        private bool _isRequestComplete;
        private bool _isCancelled;
        private string _xmlSchemaViewName = null;
        private WorkInProgressDialog _workInProgressDialog;
        private DocDataSourceClient _dataSource;

        private int _pageSize = 100;

        public NavigationControl()
        {
            InitializeComponent();

            _menuItemStates = new MenuItemStates();
            this.resultDataControl.MenuItemStates = _menuItemStates;
            this.resultDataControl.UserManager = new WindowClientUserManager();
            _workInProgressDialog = null;

            _dataSource = new DocDataSourceClient(this.resultDataControl);

            // Register the menu item state change event handler
            _menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);
        }

        #region private methods

        /// <summary>
        /// Get a web service for retrieving data instance
        /// </summary>
        /// <returns></returns>
        private CMDataServiceStub GetCMDataWebService()
        {
            if (_dataService == null)
            {
                _dataService = new CMDataServiceStub();
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

                    WorkflowModelServiceStub workflowModelServic = new WorkflowModelServiceStub();

                    SchemaInfo[] schemas = workflowModelServic.GetAuthorizedSchemaInfos(ConnectionStringBuilder.Instance.Create());
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
                MetaDataServiceStub metaDataService = new MetaDataServiceStub();
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
                builder.IsTaxonomiesShown = false;
                builder.IsDataViewsShown = false;
                builder.IsXMLSchemaViewsShown = false;
                builder.IsSelectorShown = false;

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

                        this.loadXMLButton.Enabled = true;
                    }
                    else
                    {
                        _menuItemStates.SetState(MenuItemID.EditSearch, false);
                        _menuItemStates.SetState(MenuItemID.EditShowAllInstances, false);
                        _menuItemStates.SetState(MenuItemID.ViewRowCount, false);
                        _menuItemStates.SetState(MenuItemID.ViewDetail, false);
                        _menuItemStates.SetState(MenuItemID.ViewChart, false);

                        this.loadXMLButton.Enabled = false;
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

        #endregion private methods

        #region IDataGridControl members

        /// <summary>
        /// Fire the graph event
        /// </summary>
        public void FireGraphEvent()
        {
        }

        /// <summary>
        /// Fire the download graph event
        /// </summary>
        public void FireDownloadGraphEvent(string formatName, string suffix)
        {
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

            string queryId = null;
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
                    queryId = GetCMDataWebService().BeginQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        query,
                        _pageSize);

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
            }
            finally
            {
                // finish the query
                if (queryId != null)
                {
                    GetCMDataWebService().EndQuery(queryId);
                }

                // Restore the cursor
                Cursor.Current = this.Cursor;
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

        /// <summary>
        /// Display the query result data set in a data grid
        /// </summary>
        /// <param name="dataSet">The query result in DataSet</param>
        private void ShowQueryResult(DataSet dataSet)
        {
            this.resultDataControl.ShowQueryResult(dataSet);
            this._menuItemStates.SetState(MenuItemID.ViewNextPage, false);

            // clear the InstanceView which may contains the previous DataSet
            this.resultDataControl.CurrentSlide.InstanceView = null;
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
            this.xmlSchemaListView.Items.Clear();
            this.showInstanceButton.Enabled = false;
            this.loadSchemaViewButton.Enabled = false;
            this.loadXMLButton.Enabled = false;
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
                this.xmlSchemaListView.Items.Clear();
                this.showInstanceButton.Enabled = false;
                this.loadXMLButton.Enabled = false;
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
                            // Show existing xml schemas of the class
                            ShowXMLSchemaViews(theClass);
                        }
                    }
                    _dataView.ClearSearchExpression();
                }
                else
                {
                    // selected node doesn't have an associated xml schema view, clear the previous data
                    ClearDisplay();
                }
            }
            else
            {
                // selected node doesn't have an associated data view, clear the previous data
                ClearDisplay();
            }
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
            if (this.xmlSchemaListView.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)this.xmlSchemaListView.SelectedItems[0];

                _xmlSchemaViewName = item.SubItems[1].Text;

                this.loadSchemaViewButton.Enabled = true;
            }
            else
            {
                this._xmlSchemaViewName = null;
                this.loadSchemaViewButton.Enabled = false;
            }
        }

        #endregion event handlers

        private void ShowXMLSchemaViews(ClassElement baseClass)
        {
            if (baseClass != _selectedMetaDataElement)
            {
                _selectedMetaDataElement = baseClass;

                // clear the data grid
                this.resultDataControl.ClearDataGrids();
                this.loadXMLButton.Enabled = false;

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

                this.loadSchemaViewButton.Enabled = false;
            }

            this.showInstanceButton.Enabled = true;
        }

        private void loadSchemaViewButton_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            if (!string.IsNullOrEmpty(_xmlSchemaViewName))
            {
                try
                {
                    // Change the cursor to indicate that we are waiting
                    Cursor.Current = Cursors.WaitCursor;

                    XMLSchemaModel xmlSchemaModel = (XMLSchemaModel)_metaData.XMLSchemaViews[_xmlSchemaViewName];

                    if (Globals.ThisWorkbook.Application.ActiveWorkbook.XmlMaps.Count > 0 &&
                        IsXmlSchemaExists(_xmlSchemaViewName, Globals.ThisWorkbook.Application.ActiveWorkbook.XmlMaps))
                    {
                        if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartExcel.XMLSchemaExisted"),
                            "Confirm", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            // keep the xml mappings and delete the existing xml schema
                            Dictionary<string, ExcelMappedNodeInfo> existingMappings = DeleteXMLSchema(xmlSchemaModel);

                            Excel.XmlMap xmlMap = LoadXMLSchema(xmlSchemaModel);

                            // Resore the previous xml mappings
                            RestoreXMLMapping(xmlMap, existingMappings);
                        }
                    }
                    else
                    {
                        Excel.XmlMap xmlMap = LoadXMLSchema(xmlSchemaModel);
                    }

                    // show the xml schema mappings in the action panel
                    string version = Globals.ThisWorkbook.Application.Version;
                    if (version == "11.0")
                    {
                        // Excel 2003
                        Globals.ThisWorkbook.Application.SendKeys("%DXX", true);
                    }
                    else if (version == "12.0")
                    {
                        // Excel 2007 or Excel 2013
                        Globals.ThisWorkbook.Application.SendKeys("%LX", true);
                    }
                    else if (version == "16.0")
                    {
                        // Excel 2007 or Excel 2013
                        Globals.ThisWorkbook.Application.SendKeys("%LX", true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                        "Server Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    // Restore the cursor
                    Cursor.Current = this.Cursor;
                }
            }
        }

        private bool IsXmlSchemaExists(string xmlSchemaName, Excel.XmlMaps xmlMaps)
        {
            bool status = false;

            foreach (Excel.XmlMap xmlMap in xmlMaps)
            {
                if (xmlMap.Name == xmlSchemaName)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        private void RemoveCustomization()
        {
            Globals.ThisWorkbook.RemoveCustomization();
        }

        private Excel.XmlMap LoadXMLSchema(XMLSchemaModel xmlSchemaModel)
        {
            // convert xml schema into text
            StringBuilder builder = new StringBuilder();
            StringWriter strWriter = new StringWriter(builder);
            xmlSchemaModel.Write(strWriter);

            string schemaStr = builder.ToString();

            string schemaRootElementName = xmlSchemaModel.RootElement.Caption;
            if (xmlSchemaModel.RootElement.MaxOccurs == "unbounded")
            {
                // if the max occurs of the root element is unbounded, the schema has a pseudo-element as a root element with schema model's caption as name
                schemaRootElementName = xmlSchemaModel.Caption;
            }

            Excel.XmlMap xmlMap = Globals.ThisWorkbook.Application.ActiveWorkbook.XmlMaps.Add(schemaStr, schemaRootElementName);

            // Use the xml schema model's name as the name of xml map
            xmlMap.Name = xmlSchemaModel.Name;

            return xmlMap;
        }

        private Dictionary<string, ExcelMappedNodeInfo> DeleteXMLSchema(XMLSchemaModel xmlSchemaModel)
        {
            Excel.XmlMap xmlMap = FindXmlMap(xmlSchemaModel.Name, Globals.ThisWorkbook.Application.ActiveWorkbook.XmlMaps);

            if (xmlMap == null)
            {
                throw new Exception("Unable to find an xml map with name " + xmlSchemaModel.Name);
            }

            Dictionary<string, ExcelMappedNodeInfo> existingMappings = new Dictionary<string, ExcelMappedNodeInfo>();

            List<string> xpathList = xmlSchemaModel.GetAllLeafNodeXPath(); // get xpaths for all the nodes in the existing xml schema

            ExcelMappedNodeInfo mappedNodeInfo;

            foreach (Excel.Worksheet ws in Globals.ThisWorkbook.Worksheets)
            {
                foreach (string xpathString in xpathList)
                {
                    Excel.Range rngFound = ws.XmlMapQuery(xpathString, Type.Missing, xmlMap);

                    if (rngFound != null)
                    {
                        mappedNodeInfo = new ExcelMappedNodeInfo();
                        mappedNodeInfo.IsRepeating = rngFound.XPath.Repeating;
                        mappedNodeInfo.nodeRange = rngFound;

                        existingMappings.Add(xpathString, mappedNodeInfo);
                    }
                }
            }

            // delete the xml map from the workbook
            xmlMap.Delete();

            return existingMappings;
        }

        private Excel.XmlMap FindXmlMap(string xmlSchemaName, Excel.XmlMaps xmlMaps)
        {
            Excel.XmlMap found = null;

            foreach (Excel.XmlMap xmlMap in xmlMaps)
            {
                if (xmlMap.Name == xmlSchemaName)
                {
                    found = xmlMap;
                    break;
                }
            }

            return found;
        }

        private void RestoreXMLMapping(Excel.XmlMap xmlMap, Dictionary<string, ExcelMappedNodeInfo> existingMappings)
        {
            ExcelMappedNodeInfo mappedNodeInfo;
            string xpathStr;

            foreach (KeyValuePair<string, ExcelMappedNodeInfo> keyValuePair in existingMappings)
            {
                mappedNodeInfo = (ExcelMappedNodeInfo)keyValuePair.Value;
                xpathStr = keyValuePair.Key;

                Excel.XPath xpath = mappedNodeInfo.nodeRange.XPath;

                xpath.SetValue(xmlMap, xpathStr, Missing.Value, mappedNodeInfo.IsRepeating);
            }
        }

        private void OpenExcelFile(object sender, System.EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = "c:\\";
            dialog.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = false;

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dialog.FileName;

                Globals.ThisWorkbook.Sheets.Add(Missing.Value, Missing.Value, Missing.Value, fileName);
            }
        }

        private void loadXMLButton_Click(object sender, EventArgs e)
        {
            // attach a custom principal to the thread
            AttachCustomPrincipal();

            //Globals.ThisWorkbook.Application.DisplayAlerts = false; // disable the error or alert dialog

            if (!string.IsNullOrEmpty(_xmlSchemaViewName))
            {
                InstanceView instanceView = _dataSource.GetInstanceView();
                if (instanceView != null)
                {
                    try
                    {
                        // Change the cursor to indicate that we are waiting
                        Cursor.Current = Cursors.WaitCursor;

                        // create a search query for the selected instance
                        string query = instanceView.DataView.GetInstanceQuery(instanceView.InstanceData.ObjId);

                        // invoke the web service synchronously
                        XmlDocument xmlDoc = (XmlDocument)_dataService.GenerateXmlDoc(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                            _xmlSchemaViewName, query, instanceView.DataView.BaseClass.ClassName);

                        XmlDocument doc = new XmlDocument();
                        doc.AppendChild(doc.ImportNode(xmlDoc.DocumentElement, true));

                        if (Globals.ThisWorkbook.Application.ActiveWorkbook.XmlMaps.Count > 0)
                        {
                            // convert xml doc into text
                            StringWriter sw = new StringWriter();
                            XmlTextWriter tx = new XmlTextWriter(sw);
                            doc.WriteTo(tx);
                            string xml = sw.ToString();

                            Excel.XmlMap xmlMap = FindXmlMap(_xmlSchemaViewName, Globals.ThisWorkbook.Application.ActiveWorkbook.XmlMaps);

                            if (xmlMap != null)
                            {
                                xmlMap.ImportXml(xml, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Restore the cursor
                        Cursor.Current = this.Cursor;

                        Globals.ThisWorkbook.Application.DisplayAlerts = true;
                    }
                }
            }
            else
            {
                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartExcel.XMLSchemaNotSelected"),
                "Information", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
        }

        private string GetSaveXMLFileName()
        {
            string fileName = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "xml files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = false;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                if (!fileName.EndsWith(".xml"))
                {
                    fileName += ".xml";
                }
            }

            return fileName;
        }

        private void removeCusomizationButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                RemoveCustomization();
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void showSchemaButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                // show the xml schema mappings in the action panel
                string version = Globals.ThisWorkbook.Application.Version;
                if (version == "11.0")
                {
                    // Excel 2003
                    Globals.ThisWorkbook.Application.SendKeys("%DXX", true);
                }
                else if (version == "12.0")
                {
                    // Excel 2007
                    Globals.ThisWorkbook.Application.SendKeys("%LX", true);
                }
                else if (version == "16.0")
                {
                    // Excel 2007 or Excel 2013
                    Globals.ThisWorkbook.Application.SendKeys("%LX", false);
                }
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void openBbutton_Click(object sender, EventArgs e)
        {
            OpenExcelFile(sender, e);
        }
    }

    public class ExcelMappedNodeInfo
    {
        public bool IsRepeating;
        public Excel.Range nodeRange;

    }
}
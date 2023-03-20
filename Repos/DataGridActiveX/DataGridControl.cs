using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Globalization;

using Newtera.DataGridActiveX.ActiveXControlWebService;
using Newtera.DataGridActiveX.DataGridView;
using Newtera.DataGridActiveX.Export;

namespace Newtera.DataGridActiveX
{
	public delegate void GraphCallback(int graphType);
	public delegate void DownloadFileCallback(string fileName);
	public delegate void DownloadGraphCallback(string formatName);
    public delegate void RunAlgorithmCallback(string displayUrl);
    public delegate void ShowPivotGridCallback(string dataSourceId);

	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IDataGridControlEvenets 
	{
		[DispId(1)] void GraphEvent(int graphType);
		[DispId(2)] void DownloadFileEvent(string fileName);
		[DispId(3)] void DownloadGraphEvent(string formatName);
        [DispId(4)] void RunAlgorithmEvent(string displayUrl);
        [DispId(5)] void ShowPivotGridEvent(string dataSourceId);
	}

	/// <summary>
	/// DataGridControl 的摘要说明。
	/// </summary>
	[ComSourceInterfaces(typeof(IDataGridControlEvenets))]
	public class DataGridControl : System.Windows.Forms.UserControl, IDataGridActiveX, IDataGridControl
	{
        /// <summary>
        /// The constant definition representing the database column name for
        /// internally created obj id.
        /// </summary>
        public const string OBJ_ID = "obj_id";

		private const int DATA_THRESHHOLD = 20000;

		private bool _isDataLoaded;
		private string _viewType;
		private string _taxonomy;
		private string _taxonName;
        private string _taxonCaption;
		private string _className;
		private string _classCaption;
		private string _arrayName;
		private string _arrayCaption;
        private string _relatedClassAlias;
        private string _relatedClassName;
        private string _relatedClassCaption;
		private string _instanceId;
		private string _connectionString;
		private string _baseURL;
		private string _basePhysicalPath;
		private int _totalCount;
		private string _xquery;
		private ActiveXControlService _webService;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private DataGridViewModel _dataGridView;
		private ColumnInfoCollection _columnInfos;
		private bool _isCancelled;
		private int _pageSize = 100;
		private DataSet _masterDataSet;
		private string _tableName;
		private ExportType _exportType;
		private bool _bFullScreenMode = false;
		private Control _theParent;
        private bool _webhosted = true;
        private bool _hasEditableColumns = false;
        private ComputeResultDialog _computeResultDialog;
        private string _dataSourceId = null;

		private string _downLoadfile;
		private System.Windows.Forms.ImageList toolBarImageList;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem chartWizardMenuItem;
		private System.Windows.Forms.MenuItem configColumnMenuItem;
		private System.Windows.Forms.MenuItem refreshMenuItem;
		private Newtera.DataGridActiveX.SelectionDataGrid dataGrid;
		private System.Windows.Forms.MenuItem searchMenuItem;
		private System.Windows.Forms.MenuItem exportDataMenuItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button chartButton;
		private System.Windows.Forms.Button searchButton;
		private System.Windows.Forms.Button exportButton;
		private System.Windows.Forms.Button configButton;
		private System.Windows.Forms.Button refreshButton;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button computeButton;
		private System.Windows.Forms.MenuItem maximizeMenuItem;
		private System.Windows.Forms.Panel mainPanel;
        private Button maximizeButton;
        private MenuItem computeMenuItem;
        private MenuItem quickChartMenuItem;
        private Button quickChartButton;
        private Button saveButton;
        private MenuItem saveMenuItem;
        private Button pivotButton;
		private System.ComponentModel.IContainer components;

		public event GraphCallback GraphEvent;
		public event DownloadFileCallback DownloadFileEvent;
		public event DownloadGraphCallback DownloadGraphEvent;
        public event RunAlgorithmCallback RunAlgorithmEvent;
        public event ShowPivotGridCallback ShowPivotGridEvent;

		public DataGridControl()
		{
			// 该调用是 Windows.Forms 窗体设计器所必需的。
			InitializeComponent();

			// TODO: 在 InitializeComponent 调用后添加任何初始化
			_isDataLoaded = false;
			_viewType = "CLASS";
			_taxonomy = null;
			_taxonName = null;
			_classCaption = null;
			_className = null;
			_arrayName = null;
			_arrayCaption = null;
			_connectionString = null;
			_totalCount = 0;
			_xquery = null;
			_webService = null;
			_workInProgressDialog = new WorkInProgressDialog();
			_isRequestComplete = false;
			_dataGridView = null;
			_isCancelled = false;
			_columnInfos = null;
			_baseURL = null;
			_basePhysicalPath = null;
			_exportType = null;
			_downLoadfile = null;
            _computeResultDialog = null;
		}

		#region IDataGridControl

		/// <summary>
		/// Gets the column infos of the data series currently displayed on
		/// the data grid
		/// </summary>
		public ColumnInfoCollection ColumnInfos
		{
			get
			{
				return _columnInfos;
			}
		}

		/// <summary>
		/// Gets the DataView that is in the same order as what displayed on the datagrid
		/// </summary>
		public DataView DataView
		{
			get
			{
				if (_masterDataSet != null && _masterDataSet.Tables[TableName] != null)
				{
					DataView dataView = _masterDataSet.Tables[TableName].DefaultView;

					if (this.dataGrid.SortColumnIndex >= 0)
					{
						ColumnInfo sortedColumnInfo = GetSortingColumnInfo();
						
						// set the sort expression to the DataView
						dataView.Sort = sortedColumnInfo.Name + " " + (this.dataGrid.IsAscending? "ASC" : "DESC");
					}

					return dataView;
				}
				else
				{
					return null;
				}
			}
		}

        /// <summary>
        /// Gets the base class name of data instances currently displayed in the datagrid 
        /// </summary>
        public string BaseClassName
        {
            get
            {
                string className = ClassName;
                if (this._viewType == "ARRAY")
                {
                    className = className + "_" + ArrayName;
                }

                return className;
            }
        }

		/// <summary>
		/// Gets the name of table in the DataSet
		/// </summary>
		public string TableName
		{
			get
			{
				string tableName = ClassName;
				if (this._viewType == "ARRAY")
				{
					tableName = ArrayName;
				}
                else if (this._viewType == "RELATED")
                {
                    tableName = RelatedClassName;
                }

				return tableName;
			}
		}

		/// <summary>
		/// Get Windows form DataGrid control
		/// </summary>
		public DataGrid TheDataGrid
		{
			get
			{
				return dataGrid;
			}
		}

        /// <summary>
        /// Gets the information indicated whether the DataGridControl is hosted in a web browser
        /// </summary>
        public bool IsWebHosted
        {
            get
            {
                return _webhosted;
            }
            set
            {
                _webhosted = value;
            }
        }

		/// <summary>
		/// Create a web service proxy for the activeX control
		/// </summary>
		/// <returns>A ActiveXControlService instance</returns>
		public ActiveXControlService CreateActiveXControlWebService()
		{
			ActiveXControlService webService = new ActiveXControlService();

			string url = this.BaseURL;
            if (url != null)
            {
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
            }
            else
            {
                url = "http://localhost/Newtera/";
            }

			url += @"WebService/ActiveXControlWebService.asmx";

			webService.Url = url;

			webService.Timeout = -1; // wait infinitly

			return webService;
		}

		public void FireGraphEvent()
		{
            if (IsWebHosted)
            {
                // Assume assembly granted unmanaged code permissions
                SecurityPermissionFlag flag = SecurityPermissionFlag.UnmanagedCode;
                SecurityPermission perm = new SecurityPermission(flag);
                perm.Assert(); // Danger!
                if (GraphEvent != null)
                {
                    GraphEvent(0);
                }
            }
            else
            {
                if (GraphEvent != null)
                {
                    GraphEvent(0);
                }
            }
		}

		public void FireDownloadGraphEvent(string formatName, string suffix)
		{
            if (IsWebHosted)
            {
                // Assume assembly granted unmanaged code permissions
                SecurityPermissionFlag flag = SecurityPermissionFlag.UnmanagedCode;
                SecurityPermission perm = new SecurityPermission(flag);
                perm.Assert(); // Danger!
                if (DownloadGraphEvent != null)
                {
                    DownloadGraphEvent(formatName);
                }
            }
            else
            {
                if (DownloadGraphEvent != null)
                {
                    DownloadGraphEvent(formatName);
                }
            }
		}

        public void FireRunAlgorithmEvent(string displayUrl)
        {
            if (IsWebHosted)
            {
                // Assume assembly granted unmanaged code permissions
                SecurityPermissionFlag flag = SecurityPermissionFlag.UnmanagedCode;
                SecurityPermission perm = new SecurityPermission(flag);
                perm.Assert(); // Danger!
                if (RunAlgorithmEvent != null)
                {
                    RunAlgorithmEvent(displayUrl);
                }
            }
            else
            {
                if (RunAlgorithmEvent != null)
                {
                    RunAlgorithmEvent(displayUrl);
                }
            }
        }

        private delegate void FireShowPivotGridEventDelegate();

        public void FireShowPivotGridEvent()
        {
            if (this.InvokeRequired == false)
            {
                // make sure to run ShowPivotGridEvent in the UI thread, otherwise
                // it will get "object doesn't match target type" exeception
                if (IsWebHosted)
                {
                    // Assume assembly granted unmanaged code permissions
                    SecurityPermissionFlag flag = SecurityPermissionFlag.UnmanagedCode;
                    SecurityPermission perm = new SecurityPermission(flag);
                    perm.Assert(); // Danger!
                    if (ShowPivotGridEvent != null)
                    {
                        ShowPivotGridEvent(_dataSourceId);
                    }
                }
                else
                {
                    if (ShowPivotGridEvent != null)
                    {
                        ShowPivotGridEvent(_dataSourceId);
                    }
                }
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                FireShowPivotGridEventDelegate fireShowPivotGridEvent = new FireShowPivotGridEventDelegate(FireShowPivotGridEvent);

                this.panel2.BeginInvoke(fireShowPivotGridEvent);
            }
        }

		#endregion IDataGridControl

		public void LoadData()
		{
			if (this._viewType != "ARRAY")
			{
				// load the class data
				LoadClassData();
			}
			else
			{
				// load array data
				LoadArrayData();
			}
		}

		/// <summary>
		/// Load the class data into the data grid
		/// </summary>
		private void LoadClassData()
		{
			if (!_isDataLoaded)
			{
				if (this._totalCount > DATA_THRESHHOLD)
				{
					string msg = MessageResourceManager.GetString("DataGrid.LargeData");
					msg = String.Format(msg, this._totalCount, this._totalCount/1000);
					if (MessageBox.Show(msg,
						"Confirm", MessageBoxButtons.YesNo,
						MessageBoxIcon.Question) == DialogResult.No)
					{
						// user do not want to load the data for now
						_isDataLoaded = true;
						return;
					}
				}

				if (_webService == null)
				{
					_webService = CreateActiveXControlWebService();
				}

				try
				{
					// Change the cursor to indicate that we are waiting
					Cursor.Current = Cursors.WaitCursor;

					// Get attribute information first
					if (this._viewType == "CLASS")
					{
						this._dataGridView = GetDataGridViewForClass();
						this._dataGridView.ViewType = "CLASS";
					}
					else if (this._viewType == "TAXONOMY")
					{
						this._dataGridView = GetDataGridViewForTaxon();
						this._dataGridView.ViewType = "TAXONOMY";
						this._dataGridView.TaxonomyName = this.TaxonomyName;
						this._dataGridView.TaxonName = this.TaxonName;
					}
                    else if (this._viewType == "RELATED")
                    {
                        this._dataGridView = GetDataGridViewForRelatedClass();
                        this._dataGridView.ViewType = "CLASS";
                    }

					if (XQuery == null)
					{
                        // Create a query from the dataview
                        StringBuilder builder = new StringBuilder();
                        StringWriter writer = new StringWriter(builder);
                        _dataGridView.Write(writer);

                        string xml = builder.ToString();

                        XQuery = _webService.GetXQueryForDataGridView(ConnectionString, xml);
                    }

                    if (!string.IsNullOrEmpty(XQuery))
                    {
                        _isRequestComplete = false;
                        _isCancelled = false;

                        string query = XQuery;

                        // invoke the web service asynchronously
                        _webService.BeginBeginQuery(ConnectionString,
                            query,
                            _pageSize,
                            new AsyncCallback(BeginQueryDone), null);

                        _isDataLoaded = true;

                        // launch a work in progress dialog
                        this._workInProgressDialog.MaximumSteps = this._totalCount / this._pageSize + 1;
                        this._workInProgressDialog.Value = 1;
                        ShowWorkingDialog(true, new MethodInvoker(CancelGettingData));
                    }
				}
				finally
				{
					// Restore the cursor
					Cursor.Current = this.Cursor;
				}
			}
		}

		/// <summary>
		/// Load the array data into the data grid
		/// </summary>
		private void LoadArrayData()
		{
			if (!_isDataLoaded)
			{
				if (_webService == null)
				{
					_webService = CreateActiveXControlWebService();
				}

				try
				{
					// Change the cursor to indicate that we are waiting
					Cursor.Current = Cursors.WaitCursor;

					// Get DataGridViewModel for array first
					this._dataGridView = GetDataGridViewForArray();

					if (_dataGridView != null)
					{
						_isRequestComplete = false;
						_isCancelled = false;

						// invoke the web service asynchronously
						_webService.BeginGetArrayData(ConnectionString,
							ClassName, ArrayName, InstanceId,
							new AsyncCallback(GetArrayDataDone), null);

						_isDataLoaded = true;

						// launch a work in progress dialog
						this._workInProgressDialog.MaximumSteps = 10;
						this._workInProgressDialog.Value = 1;
						this._workInProgressDialog.IsTimerEnabled = true;
						ShowWorkingDialog();
					}
				}
				finally
				{
					// Restore the cursor
					Cursor.Current = this.Cursor;
				}
			}
		}

		/// <summary>
		/// Config table display
		/// </summary>
		private void ConfigTable()
		{
			ConfigTableDialog dialog = new ConfigTableDialog();
			dialog.ColumnInfos = _columnInfos;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_columnInfos = dialog.ColumnInfos;

				foreach (ColumnInfo columnInfo in _columnInfos)
				{
					((ViewAttribute) _dataGridView.ResultAttributes[columnInfo.Name]).Visible = columnInfo.IsChecked;
				}

				DataGridTableStyle tableStyle = CreateTableStyle(_columnInfos, TableName);
				this.dataGrid.TableStyles.Clear();
				this.dataGrid.TableStyles.Add(tableStyle);
				this.dataGrid.Refresh();
			}
		}

		private void SetButtonEnableStates(bool state)
		{
			this.chartButton.Enabled = state;
			this.chartWizardMenuItem.Enabled = state;
			this.exportButton.Enabled = state;
			this.exportDataMenuItem.Enabled = state;

            if (_hasEditableColumns)
            {
                this.saveButton.Enabled = state;
                this.saveMenuItem.Enabled = state;
            }
            else
            {
                this.saveButton.Enabled = false;
                this.saveMenuItem.Enabled = false;
            }
		}

		/// <summary>
		/// Gets or sets the information indicating whether to cancel the data retrieval process.
		/// </summary>
		internal bool IsCancelled
		{
			get
			{
				return _isCancelled;
			}
			set
			{
				_isCancelled = value;
			}
		}

		/// <summary>
		/// Get a DataGridViewModel object for a given class from the server
		/// </summary>
		/// <returns>A DataGridViewModel object</returns>
		private DataGridViewModel GetDataGridViewForClass()
		{
			DataGridViewModel dataGridView = null;

			string xml = _webService.GetDataGridViewForClass(ConnectionString, ClassName);

			dataGridView = new DataGridViewModel();
			StringReader reader = new StringReader(xml);
			dataGridView.Read(reader);

			return dataGridView;
		}

		/// <summary>
		/// Get a DataGridViewModel object for a given taxon node from the server
		/// </summary>
		/// <returns>A DataGridViewModel object</returns>
		private DataGridViewModel GetDataGridViewForTaxon()
		{
			DataGridViewModel dataGridView = null;

			string xml = _webService.GetDataGridViewForTaxon(ConnectionString, TaxonomyName, TaxonName);

			dataGridView = new DataGridViewModel();
			StringReader reader = new StringReader(xml);
			dataGridView.Read(reader);

			return dataGridView;
		}

        /// <sumary>
        /// Get a DataGridViewModel object for a collection of related data instances to 
        /// a given instance from the server
        /// </summary>
        /// <returns>A DataGridViewModel object</returns>
        private DataGridViewModel GetDataGridViewForRelatedClass()
        {
            DataGridViewModel dataGridView = null;

            string xml = _webService.GetDataGridViewForRelatedClass(ConnectionString, ClassName, RelatedClassAlias, RelatedClassName, InstanceId);

            if (xml != null)
            {
                dataGridView = new DataGridViewModel();
                StringReader reader = new StringReader(xml);
                dataGridView.Read(reader);
            }
            else
            {
                throw new Exception("Failed to get data view for related class " + RelatedClassName + " with objId " + InstanceId);
            }

            return dataGridView;
        }

		/// <summary>
		/// Gets a DataGridViewModel object for a given class from the server
		/// </summary>
		/// <returns>A DataGridViewModel object</returns>
		private DataGridViewModel GetDataGridViewForArray()
		{
			DataGridViewModel dataGridView = null;

            string xml = _webService.GetDataGridViewForArray(ConnectionString, ClassName, ArrayName);

			dataGridView = new DataGridViewModel();
			StringReader reader = new StringReader(xml);
			dataGridView.Read(reader);

			return dataGridView;
		}

		/// <summary>
		/// The AsyncCallback event handler for BeginQuery web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void BeginQueryDone(IAsyncResult res)
		{
			string queryId = null;
			try
			{						
				// Get the query id
				queryId = _webService.EndBeginQuery(res);
				XmlNode xmlNode;
				
				_masterDataSet = null;
				XmlReader xmlReader;

				this._workInProgressDialog.PerformStep(); // move one step forward

				// It is a Worker thread, continue getting rest of data
				DataSet slaveDataSet;
				int currentPageIndex = 0;
				int instanceCount = 0;
				int start, end;
				while (instanceCount < _totalCount && !_isCancelled)
				{
					start = currentPageIndex * _pageSize + 1;
					end = start + this._pageSize - 1;
					if (end > _totalCount)
					{
						end = _totalCount;
					}

                    SetDisplayText(MessageResourceManager.GetString("DataGrid.GettingData") + " " + start + ":" + end);
					
					this._workInProgressDialog.PerformStep(); // move one step forward

					// invoke the web service synchronously to get data in pages
					xmlNode = _webService.GetNextResult(ConnectionString,
						queryId);

					if (xmlNode == null)
					{
						// end of result
						break;
					}

					slaveDataSet = new DataSet();

					xmlReader = new XmlNodeReader(xmlNode);
					slaveDataSet.ReadXml(xmlReader);

                    if (IsEmptyDataSet(slaveDataSet, this.TableName))
                    {
                        // got an empty result
                        break;
                    }

					instanceCount += slaveDataSet.Tables[this.TableName].Rows.Count;

					if (_masterDataSet == null)
					{
						// first page
                        _masterDataSet = CreateMasterDataSet(slaveDataSet, TableName);
						_masterDataSet.EnforceConstraints = false;
					}
					
					// append to the master dataset
					AppendDataSet(_masterDataSet, slaveDataSet, TableName);

					currentPageIndex++;
				}

				_tableName = TableName;
				ShowQueryResult();
			}
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
			finally
			{
				// end of query
				if (queryId != null)
				{
					try
					{
						_webService.EndQuery(queryId);
					}
					catch (Exception)
					{
					}
				}

				//Bring down the work in progress dialog
				HideWorkingDialog();
			}
		}

        private delegate void SetWorkingDialogDisplayTextDelegate(string text);

        private void SetDisplayText(string text)
        {
            if (this.InvokeRequired == false)
            {
                // It is the UI thread
                this._workInProgressDialog.DisplayText = text;
            }
            else
            {
                // It is a worker thread, pass the control to the UI thread
                SetWorkingDialogDisplayTextDelegate setDisplayTextDelegate = new SetWorkingDialogDisplayTextDelegate(SetDisplayText);
                this.panel2.BeginInvoke(setDisplayTextDelegate, new object[] { text });
            }
        }

		/// <summary>
		/// The AsyncCallback event handler for GetArrayData web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void GetArrayDataDone(IAsyncResult res)
		{
			try
			{						
				// Get the query result in DataSet
				string xml = _webService.EndGetArrayData(res);

				_masterDataSet = new DataSet();

				StringReader strReader = new StringReader(xml);
				_masterDataSet.ReadXml(strReader);
				_masterDataSet.EnforceConstraints = false;

				//this._workInProgressDialog.PerformStep(); // move one step forward

				this._tableName = ArrayName;
				ShowQueryResult();
			}
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
			finally
			{
				//Bring down the work in progress dialog
				HideWorkingDialog();
				this._workInProgressDialog.IsTimerEnabled = false;
			}
		}

		private delegate void ShowQueryResultDelegate();

		/// <summary>
		/// Display the query result data set in a data grid
		/// </summary>
		private void ShowQueryResult()
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, display the dataset in the datagrid
                if (!IsEmptyDataSet(_masterDataSet, _tableName))
				{
					// The data grid need a table style to display the data
					this.dataGrid.TableStyles.Clear();
					if (_columnInfos == null)
					{
						_columnInfos = this.CreateColumnInfos(_masterDataSet, _tableName);
					}

                    _hasEditableColumns = false;
					DataGridTableStyle tableStyle = CreateTableStyle(this._columnInfos, _tableName);

					this.dataGrid.TableStyles.Add(tableStyle);

                    if (_hasEditableColumns)
                    {
                        // clear the has changed flas
                        _masterDataSet.AcceptChanges();
                    }

					this.dataGrid.SuspendLayout();
					if (_viewType == "ARRAY")
					{
						this.dataGrid.CaptionText = ArrayCaption + " (" + this.TotalCount + ")";
					}
                    else if (_viewType == "TAXONOMY")
                    {
                        this.dataGrid.CaptionText = TaxonCaption + " (" + _masterDataSet.Tables[_tableName].Rows.Count + ")";
                    }
                    else if (_viewType == "CLASS")
                    {
                        this.dataGrid.CaptionText = ClassCaption + " (" + _masterDataSet.Tables[_tableName].Rows.Count + ")";
                    }
                    else if (_viewType == "RELATED")
                    {
                        this.dataGrid.CaptionText = RelatedClassCaption + " (" + _masterDataSet.Tables[_tableName].Rows.Count + ")";
                    }

					this.dataGrid.SetDataBinding(_masterDataSet, _tableName);
					this.dataGrid.ResumeLayout(true);

					// select the first row automatically
					this.dataGrid.Select(0);

					// set button states to true
					this.SetButtonEnableStates(true);
				}
				else
				{
					this.dataGrid.DataSource = null;

					// set button states to false
					this.SetButtonEnableStates(false);

					MessageBox.Show(MessageResourceManager.GetString("DataGrid.NoResult"), "Information",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}

                if (this._viewType == "ARRAY")
                {
                    // disable the search buttons and export for array data
                    this.searchButton.Enabled = false;
                    this.searchMenuItem.Enabled = false;
                }
			}
			else
			{
				//pass the control to UI thread
				ShowQueryResultDelegate showQueryResult = new ShowQueryResultDelegate(ShowQueryResult);

				this.panel2.BeginInvoke(showQueryResult);
			}
		}

        /// <summary>
        /// Gets the value indicating whether the data set contains data
        /// for a table
        /// </summary>
        /// <param name="dataSet">The data set</param>
        /// <param name="tableName">The table name</param>
        /// <returns>true if it contains no data, false otherwise.</returns>
        private bool IsEmptyDataSet(DataSet dataSet, string tableName)
        {
            bool status = true;

            if (dataSet != null)
            {
                // find the table , check if it is empty
                DataTable dataTable = dataSet.Tables[tableName];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    if (this._viewType != "ARRAY")
                    {
                        // Check the obj_id field
                        if (dataTable.Rows.Count != 1 ||
                            (dataTable.Columns[OBJ_ID] != null &&
                            !dataTable.Rows[0].IsNull(OBJ_ID) &&
                              dataTable.Rows[0][OBJ_ID].ToString().Length > 0))
                        {
                            status = false;
                        }
                    }
                    else
                    {
                        // it is for array
                        status = false;
                    }
                }
            }

            return status;
        }

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
		private void ShowWorkingDialog(string msg)
		{
			ShowWorkingDialog(msg, false, null);
		}

		/// <summary>
		/// Show the working dialog
		/// </summary>
		/// <remarks>It has to deal with multi-threading issue</remarks>
		private void ShowWorkingDialog(bool cancellable, MethodInvoker cancellCallback)
		{
			ShowWorkingDialog(null, cancellable, cancellCallback);
		}

		/// <summary>
		/// Show the working dialog
		/// </summary>
		/// <remarks>It has to deal with multi-threading issue</remarks>
		private void ShowWorkingDialog(string msg, bool cancellable, MethodInvoker cancellCallback)
		{
			lock (_workInProgressDialog)
			{

				_workInProgressDialog.EnableCancel = cancellable;
				_workInProgressDialog.CancelCallback = cancellCallback;

				// check _isRequestComplete flag in case the worker thread
				// completes the request before the working dialog is shown
				if (!_isRequestComplete && !_workInProgressDialog.Visible)
				{
					_workInProgressDialog.DisplayText = msg;
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
				lock(_workInProgressDialog)
				{
					_workInProgressDialog.Close();
					_isRequestComplete = true;
				}
			}
			else
			{
				// It is a worker thread, pass the control to the UI thread
				HideWorkingDialogDelegate hideWorkingDialog = new HideWorkingDialogDelegate(HideWorkingDialog);
				this.panel2.BeginInvoke(hideWorkingDialog);
			}
		}

        private delegate void ShowErrorMessageDelegate(string msg);

        /// <summary>
        /// Show error message
        /// </summary>
        /// <remarks>Has to condider multi-threading issue</remarks>
        private void ShowErrorMessage(string msg)
        {
            if (this.InvokeRequired == false)
            {
                // It is the UI thread, go ahead to close the working dialog
                // lock it while updating _isRequestComplete
                MessageBox.Show(msg);
            }
            else
            {
                // It is a worker thread, pass the control to the UI thread
                ShowErrorMessageDelegate showErrorDelegate = new ShowErrorMessageDelegate(ShowErrorMessage);
                this.panel2.BeginInvoke(showErrorDelegate, new object[] { msg });
            }
        }

		/// <summary>
		/// Create a DataGridTableStyle based a collection of ColumnInfo objects
		/// </summary>
		/// <param name="columnInfos">A collection of ColumnInfo objects</param>
		/// <param name="tableName">The name of the Table in DataSet</param>
		/// <returns>A DataGridTableStyle</returns>
		private DataGridTableStyle CreateTableStyle(ColumnInfoCollection columnInfos, string tableName)
		{
			DataGridTableStyle tableStyle = new DataGridTableStyle();

			// mapping the table style to the name of class
			tableStyle.MappingName = tableName;
			tableStyle.AlternatingBackColor = Color.LightGray;
			DataGridColumnStyle columnStyle;

			if (columnInfos != null)
			{
				foreach (ColumnInfo columnInfo in columnInfos)
				{
					if (columnInfo.IsChecked)
					{
                        ViewAttribute attribute = (ViewAttribute)_dataGridView.ResultAttributes[columnInfo.Name];
                        ViewSimpleAttribute simpleAttribute = attribute as ViewSimpleAttribute;
                        ViewVirtualAttribute virtualAttribute = attribute as ViewVirtualAttribute;
                        ViewRelationshipAttribute relationshipAttribute = attribute as ViewRelationshipAttribute;

                        if (simpleAttribute != null)
                        {
                            if (simpleAttribute.EnumConstraint != null &&
                                simpleAttribute.EnumConstraint.DisplayMode == ViewEnumDisplayMode.Image)
                            {
                                // show images
                                columnStyle = new DataGridControlImageColumn();
                                ((DataGridControlImageColumn)columnStyle).ImageGetter = new DataGridEnumColumnImageGetter(simpleAttribute.EnumConstraint);
                            }
                            else if (simpleAttribute.InlineEditEnabled &&
                                this._viewType == "CLASS" &&
                                simpleAttribute.EnumConstraint != null)
                            {
                                // show combobox with enum values
                                DataGridControlComboBoxColumn comboBoxColumn = new DataGridControlComboBoxColumn();
                                comboBoxColumn.ComboBoxControl.DataSource = simpleAttribute.EnumConstraint.Values;
                                comboBoxColumn.ComboBoxControl.DisplayMember = "DisplayText";
                                comboBoxColumn.ComboBoxControl.ValueMember = "DisplayText";
                                tableStyle.PreferredRowHeight = comboBoxColumn.ComboBoxControl.Height;
                                columnStyle = comboBoxColumn;
                            }
                            else if (simpleAttribute.InlineEditEnabled &&
                                this._viewType == "CLASS" &&
                                simpleAttribute.DataType == ViewDataType.Boolean)
                            {
                                // show combobox with boolean values
                                DataGridControlComboBoxColumn comboBoxColumn = new DataGridControlComboBoxColumn();
                                comboBoxColumn.ComboBoxControl.DataSource = _dataGridView.LocaleInfo.GetBooleanValues();
                                comboBoxColumn.ComboBoxControl.DisplayMember = "DisplayText";
                                comboBoxColumn.ComboBoxControl.ValueMember = "DisplayText";
                                tableStyle.PreferredRowHeight = comboBoxColumn.ComboBoxControl.Height;
                                columnStyle = comboBoxColumn;
                            }
                            else
                            {
                                // show text box
                                columnStyle = new DataGridTextBoxColumn();
                            }

                            if (simpleAttribute.InlineEditEnabled &&
                                _viewType == "CLASS")
                            {
                                _hasEditableColumns = true;
                            }
                        }
                        else if (virtualAttribute != null &&
                            virtualAttribute.EnumConstraint != null &&
                            virtualAttribute.EnumConstraint.DisplayMode == ViewEnumDisplayMode.Image)
                        {
                            // show images
                            columnStyle = new DataGridControlImageColumn();
                            ((DataGridControlImageColumn)columnStyle).ImageGetter = new DataGridEnumColumnImageGetter(virtualAttribute.EnumConstraint);
                        }
                        else if (relationshipAttribute != null)
                        {
                            // do not show relationship attribute in result grid
                            columnStyle = null;
                        }
                        else
                        {
                            // show text
                            columnStyle = new DataGridTextBoxColumn();
                        }

                        if (columnStyle != null)
                        {
                            columnStyle.MappingName = columnInfo.Name;
                            columnStyle.HeaderText = columnInfo.Caption;
                            if (columnInfo.IsArray)
                            {
                                columnStyle.NullText = "...";
                                columnStyle.Alignment = HorizontalAlignment.Center;
                            }
                            else
                            {
                                columnStyle.NullText = "";
                            }

                            // InlineEditEnabled is also set to false if the user doesn't have permission to modify the attribute
                            if (simpleAttribute != null && simpleAttribute.InlineEditEnabled &&
                                _viewType == "CLASS")
                            {
                                columnStyle.ReadOnly = false;
                            }
                            else
                            {
                                columnStyle.ReadOnly = true;
                            }

                            tableStyle.GridColumnStyles.Add(columnStyle);
                        }
					}
				}
			}

			return tableStyle;
		}

		/// <summary>
		/// Create a ColumnInfoCollection object from a dataset
		/// </summary>
		/// <param name="dataSet">The data set</param>
		/// <param name="tableName">The name of the table in DataSet</param>
		/// <returns>A ColumnInfoCollection</returns>
		private ColumnInfoCollection CreateColumnInfos(DataSet dataSet, string tableName)
		{
			ColumnInfoCollection columnInfos = new ColumnInfoCollection();
			
			ColumnInfo columnInfo;

			DataTable dataTable = dataSet.Tables[tableName];
			if (dataTable != null)
			{
				foreach (DataColumn column in dataTable.Columns)
				{
					// Create a column info for each column is included in attributeInfos
					ViewAttribute attribute = (ViewAttribute) this._dataGridView.ResultAttributes[column.ColumnName];
                    if (attribute != null)
					{
						columnInfo = new ColumnInfo();

						columnInfo.Name = column.ColumnName;
						columnInfo.Caption = attribute.Caption;
                        if (attribute is ViewArrayAttribute)
                        {
                            columnInfo.IsArray = true;
                        }
                        else
                        {
                            columnInfo.IsArray = false;
                        }
					
						columnInfos.Add(columnInfo);
					}
				}
			}

			return columnInfos;
		}

		/// <summary>
		/// Append a slave DataSet to a master dataset
		/// </summary>
		/// <param name="master">The master DataSet</param>
		/// <param name="slave">The slave DataSet</param>
		private void AppendDataSet(DataSet master, DataSet slave, string tableName)
		{
			DataTable masterTable = master.Tables[tableName];
			DataTable slaveTable = slave.Tables[tableName];
			DataRow newRow;
			int cols = slaveTable.Columns.Count;
			string val;

			if (slaveTable != null)
			{
				foreach (DataRow row in slaveTable.Rows)
				{
					newRow = masterTable.NewRow();
					masterTable.Rows.Add(newRow);
					for (int i = 0 ; i < cols; i++)
					{
						if (!row.IsNull(i))
						{
							val = row[i].ToString();
							if (val.Length > 0)
							{
								newRow[i] = row[i];
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Launch the search dialog
		/// </summary>
		private void LaunchSearchDialog()
		{
			SearchDialog dialog = new SearchDialog();
			dialog.DataGridView = this._dataGridView;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				// convert the data grid view to xml
				StringBuilder builder = new StringBuilder();
				StringWriter writer = new StringWriter(builder);
				_dataGridView.Write(writer);

				string xml = builder.ToString();

				if (_webService == null)
				{
					_webService = CreateActiveXControlWebService();
				}

				try
				{
					// Change the cursor to indicate that we are waiting
					Cursor.Current = Cursors.WaitCursor;

					// Get XQuery build for the DataGridView
					string query = _webService.GetXQueryForDataGridView(ConnectionString, xml);

					if (_dataGridView != null && XQuery != null)
					{
						_isRequestComplete = false;
						_isCancelled = false;

						// invoke the web service asynchronously
						_webService.BeginBeginQuery(ConnectionString,
							query,
							_pageSize,
							new AsyncCallback(BeginQueryDone), null);

						_isDataLoaded = true;

						// launch a work in progress dialog
						this._workInProgressDialog.MaximumSteps = this._totalCount/this._pageSize + 1;
						this._workInProgressDialog.Value = 1;
						ShowWorkingDialog(true, new MethodInvoker(CancelGettingData));
					}
				}
				finally
				{
					// Restore the cursor
					Cursor.Current = this.Cursor;
				}
			}
		}

		/// <summary>
		/// Cancel the lengthy in a worker thread
		/// </summary>
		private void CancelGettingData()
		{
			this._isCancelled = true;
		}

		/// <summary>
		/// Export data that is currently loaded in the data grid control
		/// </summary>
		private void ExportData()
		{
			if (this._totalCount > DATA_THRESHHOLD)
			{
				string msg = MessageResourceManager.GetString("DataGrid.ExportLargeData");
				msg = String.Format(msg, this._totalCount);
				if (MessageBox.Show(msg,
					"Confirm", MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
			}

			ExportDialog dialog = new ExportDialog();

			if (_webService == null)
			{
				_webService = CreateActiveXControlWebService();
			}

			dialog.WebService = _webService;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_exportType = dialog.SelectedExportType;

				string selectedRows = null;

				try
				{
					// Change the cursor to indicate that we are waiting
					Cursor.Current = Cursors.WaitCursor;

					// use User name as the file name so that it won't
					// conflict with other users
					_downLoadfile = GetUserName() + "." + _exportType.Suffix;

					if (!dialog.ExportAll)
					{
						selectedRows = GetSelectedRows();
					}

					_isRequestComplete = false;

					if (this._viewType != "ARRAY")
					{
						// export class data

						if (this.dataGrid.SortColumnIndex > 0)
						{
							// make sure to export the data records in the same order as displayed
							_dataGridView.ClearSortBy();
							ViewAttribute sortedAttribute = GetSortingAttribute();
							if (sortedAttribute != null)
							{
								_dataGridView.SortBy.SortAttributes.Add(sortedAttribute);
							}

							if (this.dataGrid.IsAscending)
							{
								_dataGridView.SortBy.SortDirection = ViewSortDirection.Ascending;
							}
							else
							{
								_dataGridView.SortBy.SortDirection = ViewSortDirection.Descending;
							}
						}
						
						// convert the data grid view to xml
						StringBuilder builder = new StringBuilder();
						StringWriter writer = new StringWriter(builder);
						_dataGridView.Write(writer);
						string xml = builder.ToString();

						// invoke the web service asynchronously
						_webService.BeginExportDataToFile(ConnectionString, xml,
							BasePath + @"temp\" + _downLoadfile, _exportType.Exporter, selectedRows,
							new AsyncCallback(ExportDataDone), null);
					}
					else
					{
						// export array data
						_webService.BeginExportArrayDataToFile(ConnectionString, ClassName,
							ArrayName, InstanceId,
							BasePath + @"temp\" + _downLoadfile, _exportType.Exporter, selectedRows,
							new AsyncCallback(ExportArrayDataDone), null);
					}

					// launch a work in progress dialog
					this._workInProgressDialog.IsTimerEnabled = true;
					this._workInProgressDialog.MaximumSteps = 10;
					ShowWorkingDialog(MessageResourceManager.GetString("DataGrid.Exporting"));
				}
				finally
				{
					// Restore the cursor
					Cursor.Current = this.Cursor;
				}
			}
		}

		/// <summary>
		/// The AsyncCallback event handler for ExportData web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void ExportDataDone(IAsyncResult res)
		{
            try
            {
                _webService.EndExportDataToFile(res);

                FireDownloadFileEvent();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
			finally
			{
				//Bring down the work in progress dialog
				HideWorkingDialog();
				this._workInProgressDialog.IsTimerEnabled = false;
			}
		}

		/// <summary>
		/// The AsyncCallback event handler for ExportArrayData web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void ExportArrayDataDone(IAsyncResult res)
		{
            try
            {
                _webService.EndExportArrayDataToFile(res);

                FireDownloadFileEvent();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
			finally
			{
				//Bring down the work in progress dialog
				HideWorkingDialog();
				this._workInProgressDialog.IsTimerEnabled = false;
			}
		}

        internal void ShowPivotGrid()
        {
            if (this._totalCount > DATA_THRESHHOLD)
            {
                string msg = MessageResourceManager.GetString("DataGrid.PivotOnLargeData");
                msg = String.Format(msg, this._totalCount);
                if (MessageBox.Show(msg,
                    "Confirm", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            if (_webService == null)
            {
                _webService = CreateActiveXControlWebService();
            }

            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                _isRequestComplete = false;

                if (this._viewType != "ARRAY")
                {
                    // do pivot on class data

                    // convert the data grid view to xml
                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);
                    _dataGridView.Write(writer);
                    string xml = builder.ToString();

                    // invoke the web service asynchronously
                   _webService.BeginGetClassDataForPivotGrid(ConnectionString, xml,
                        new AsyncCallback(GetClassDataForPivotGridDone), null);
                }
                else
                {
                    // pivot on array data
                    _webService.BeginGetArrayDataForPivotGrid(ConnectionString, ClassName,
                        ArrayName, InstanceId,
                        new AsyncCallback(GetArrayDataForPivotGridDone), null);
                }

                // launch a work in progress dialog
                this._workInProgressDialog.IsTimerEnabled = true;
                this._workInProgressDialog.MaximumSteps = 10;
                ShowWorkingDialog(MessageResourceManager.GetString("DataGrid.GetDataForPivot"));
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for GetClassDataForPivotGrid web service method.
        /// </summary>
        /// <param name="res">The result</param>
        private void GetClassDataForPivotGridDone(IAsyncResult res)
        {
            try
            {
                _dataSourceId = _webService.EndGetClassDataForPivotGrid(res);

                FireShowPivotGridEvent();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
                this._workInProgressDialog.IsTimerEnabled = false;
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for GetArrayDataForPivotGrid web service method.
        /// </summary>
        /// <param name="res">The result</param>
        private void GetArrayDataForPivotGridDone(IAsyncResult res)
        {
            try
            {
                _dataSourceId = _webService.EndGetArrayDataForPivotGrid(res);

                FireShowPivotGridEvent();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
                this._workInProgressDialog.IsTimerEnabled = false;
            }
        }

        /// <summary>
        /// Execute an algorithm on the selected data rows and columns
        /// </summary>
        /// <returns>The result of executing the algorithm</returns>
        internal string ExecuteAlgorithm(AlgorithmType algorithmType, bool allRows)
        {
            string selectedRows = null;
            string result = null;

            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                if (!allRows)
                {
                    selectedRows = GetSelectedRows();
                }

                _isRequestComplete = false;

                if (this._viewType != "ARRAY")
                {
                    // compute the class data

                    if (this.dataGrid.SortColumnIndex > 0)
                    {
                        // make sure to compute the data records in the same order as displayed
                        _dataGridView.ClearSortBy();
                        ViewAttribute sortedAttribute = GetSortingAttribute();
                        if (sortedAttribute != null)
                        {
                            _dataGridView.SortBy.SortAttributes.Add(sortedAttribute);
                        }

                        if (this.dataGrid.IsAscending)
                        {
                            _dataGridView.SortBy.SortDirection = ViewSortDirection.Ascending;
                        }
                        else
                        {
                            _dataGridView.SortBy.SortDirection = ViewSortDirection.Descending;
                        }
                    }

                    // select the attributes in the data grid view according to the column
                    // selection in the ComputeResultDialog
                    if (_computeResultDialog != null)
                    {
                        foreach (ColumnInfo columnInfo in _computeResultDialog.ColumnInfos)
                        {
                            ((ViewAttribute)_dataGridView.ResultAttributes[columnInfo.Name]).Visible = columnInfo.IsChecked;
                        }
                    }

                    // convert the data grid view to xml
                    StringBuilder builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);
                    _dataGridView.Write(writer);
                    string xml = builder.ToString();

                    // Now we got the data grid view in xml, reset the data grid view
                    // back to its original state
                    foreach (ColumnInfo columnInfo in _columnInfos)
                    {
                        ((ViewAttribute)_dataGridView.ResultAttributes[columnInfo.Name]).Visible = columnInfo.IsChecked;
                    }

                    // invoke the web service synchronously
                    result = _webService.RunAlgorithmOnClassData(ConnectionString, xml,
                        algorithmType.Name, selectedRows);
                }
                else
                {
                    StringBuilder builder = new StringBuilder();

                    // select the array columns according to the column
                    // selection in the ComputeResultDialog
                    if (_computeResultDialog != null)
                    {
                        foreach (ColumnInfo columnInfo in _computeResultDialog.ColumnInfos)
                        {
                            if (columnInfo.IsChecked)
                            {
                                if (builder.Length == 0)
                                {
                                    builder.Append(columnInfo.Name);
                                }
                                else
                                {
                                    builder.Append(";").Append(columnInfo.Name);
                                }
                            }
                        }
                    }

                    // execute array data
                    result = _webService.RunAlgorithmOnArrayData(ConnectionString, ClassName,
                        ArrayName, InstanceId,
                        algorithmType.Name, builder.ToString(), selectedRows);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }

            return result;
        }

		private delegate void FireDownloadFileEventDelegate();

		/// <summary>
		/// Fire the event of downloading a file
		/// </summary>
		private void FireDownloadFileEvent()
		{
			if (this.InvokeRequired == false)
			{
                if (IsWebHosted)
                {
                    // fire the event to the hosting web browser
                    // Assume assembly granted unmanaged code permissions
                    SecurityPermissionFlag flag = SecurityPermissionFlag.UnmanagedCode;
                    SecurityPermission perm = new SecurityPermission(flag);
                    perm.Assert(); // Danger!

                    if (DownloadFileEvent != null)
                    {
                        DownloadFileEvent(_downLoadfile);
                    }
                }
                else
                {
                    // fire the event to the hosting windows dialog
                    if (DownloadFileEvent != null)
                    {
                        DownloadFileEvent(_downLoadfile);
                    }
                }
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				FireDownloadFileEventDelegate fireDownloadEvent = new FireDownloadFileEventDelegate(FireDownloadFileEvent);

                this.panel2.BeginInvoke(fireDownloadEvent);
			}
		}

		/// <summary>
		/// Gets the user name from the ConnectionString
		/// </summary>
		/// <returns>The user name</returns>
		private string GetUserName()
		{
			string userName = "User";

			// Compile regular expression to find "name = value" pairs
			Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

			MatchCollection matches = regex.Matches(ConnectionString);
			foreach (Match match in matches)
			{
				int pos = match.Value.IndexOf("=");
				string key = match.Value.Substring(0, pos).TrimEnd();
				string val = match.Value.Substring(pos + 1).TrimStart();
				if (key == "USER_ID")
				{
					userName = val;
					break;
				}
			}

			return userName;
		}

		/// <summary>
		/// Gets the indecies of the selected rows as space separated string
		/// </summary>
		/// <returns></returns>
		private string GetSelectedRows()
		{
			StringBuilder builder = new StringBuilder();

			DataTable dataTable = this.DataView.Table;

			int index = 0;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (this.TheDataGrid.IsSelected(i))
				{
					if (index > 0)
					{
						builder.Append(" ");
					}

					builder.Append(i);
					index++;
				}
			}

			if (builder.Length > 0)
			{
				return builder.ToString();
			}
			else
			{
				return null;
			}
		}

        /// <summary>
        /// Update data instances whose attribute values have been changed in the grid control
        /// </summary>
        private void UpdateData()
        {
            if (_webService == null)
            {
                _webService = CreateActiveXControlWebService();
            }

            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                _isRequestComplete = false;

                // convert the data grid view to xml
                DataSet changedDataSet = _masterDataSet.GetChanges(DataRowState.Modified);

                if (changedDataSet != null)
                {
                    string xml = changedDataSet.GetXml();

                    // invoke the web service asynchronously
                    _webService.BeginUpdateData(ConnectionString,
                        this._dataGridView.BaseClass.ClassName,
                        xml,
                        new AsyncCallback(UpdateDataDone), null);

                    // launch a work in progress dialog
                    this._workInProgressDialog.IsTimerEnabled = true;
                    this._workInProgressDialog.MaximumSteps = 10;
                    ShowWorkingDialog(MessageResourceManager.GetString("DataGrid.Updating"));
                }
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for UpdateData web service method.
        /// </summary>
        /// <param name="res">The result</param>
        private void UpdateDataDone(IAsyncResult res)
        {
            try
            {
                string msg = _webService.EndUpdateData(res);

                AcceptChanges(msg);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
                this._workInProgressDialog.IsTimerEnabled = false;
            }
        }

        private delegate void AcceptChangesDelegate(string msg);

        /// <summary>
        /// Accept changes
        /// </summary>
        private void AcceptChanges(string msg)
        {
            if (this.InvokeRequired == false)
            {
                string displayMsg = MessageResourceManager.GetString("DataGrid.UpdateDone");
                if (!string.IsNullOrEmpty(msg))
                {
                    displayMsg = msg;
                }
                MessageBox.Show(displayMsg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _masterDataSet.AcceptChanges();
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                AcceptChangesDelegate acceptChanges = new AcceptChangesDelegate(AcceptChanges);

                this.panel2.BeginInvoke(acceptChanges, new object[] { msg });
            }
        }

		private void RefreshData()
		{
			// clear the content, search criteria and sortby, load data again
			this._isDataLoaded = false;
			this.dataGrid.DataSource = null;
            if (_dataGridView != null)
            {
                _dataGridView.ClearSortBy();
            }
			this.dataGrid.SortColumnIndex = -1;
			LoadData();
		}

		/// <summary>
		/// Show the control in full screen
		/// </summary>
		private void ShowFullScreen()
		{
			if (_bFullScreenMode == false)
			{
				FullScreenForm f = new FullScreenForm();
				f.DataGrid = this;

				_theParent = this.mainPanel.Parent;

				this.mainPanel.Parent = f;

				// change the button states
				this.maximizeMenuItem.Checked = true;
                this.maximizeButton.ImageIndex = 6;

				_bFullScreenMode = true;

				f.Show();
			}
			else
			{
				Form f = (Form) this.mainPanel.Parent;
				this.mainPanel.Parent = this._theParent;

				_bFullScreenMode = false;

				// change the button states
				this.maximizeMenuItem.Checked = false;
                this.maximizeButton.ImageIndex = 5;

				f.Close();
			}
		}

        private void ComputeResult(int selectedColumnIndex)
        {
            _computeResultDialog = new ComputeResultDialog();
            // register event handler
            _computeResultDialog.RunAlgorithmEvent += new RunAlgorithmCallback(ComputeResultDialog_RunAlgorithmEvent);

            if (_webService == null)
            {
                _webService = CreateActiveXControlWebService();
            }

            _computeResultDialog.WebService = _webService;
            _computeResultDialog.DataGridControl = this;
            _computeResultDialog.ColumnInfos = _columnInfos;
            _computeResultDialog.BaseClassName = ClassName;
            if (selectedColumnIndex >= 0)
            {
                _computeResultDialog.SelectedColumnName = GetColumnNameByIndex(selectedColumnIndex);
            }
            _computeResultDialog.ShowDialog();

            // the dialog is cancelled, set it to null so that it can be collected
            _computeResultDialog = null;
        }

		internal void ShowRegularScreen()
		{
			if (_bFullScreenMode == true)
			{
				Form f = (Form) this.mainPanel.Parent;
				this.mainPanel.Parent = this._theParent;

				_bFullScreenMode = false;

				// change the button states
				this.maximizeMenuItem.Checked = false;
				this.maximizeButton.ImageIndex = 5;
			}
		}

        /// <summary>
        /// Show a dialog of showing templates
        /// </summary>
        private void ShowTemplates()
        {
            ShowTemplatesDialog dialog = new ShowTemplatesDialog();
            dialog.DataGridControl = this;
            dialog.ConnectionString = this.ConnectionString;

            dialog.Show(); // we cann't use ShowDialog because it will prevent downloading a graph file
        }

		/// <summary>
		/// Create a DataSet as the master dataset
		/// </summary>
		/// <param name="slave">The slave DataSet</param>
		/// <param name="tableName">The table name</param>
		/// <returns>The msater dataset</returns>
		private DataSet CreateMasterDataSet(DataSet slave, string tableName)
		{
			DataSet master = new DataSet();
			DataTable masterTable = new DataTable(tableName);
			master.Tables.Add(masterTable);
			DataTable slaveTable = slave.Tables[tableName];
			DataColumn masterColumn;

			if (slaveTable != null)
			{
				foreach (DataColumn dataColumn in slaveTable.Columns)
				{
					masterColumn = masterTable.Columns.Add(dataColumn.ColumnName,
						GetDataType(dataColumn.ColumnName));
					masterColumn.AllowDBNull = true;
				}
			}

			return master;
		}

		/// <summary>
		/// Gets the datatype of the given attribute
		/// </summary>
		/// <param name="attributeName">Attribute name</param>
		/// <returns>System.Type</returns>
		private System.Type GetDataType(string attributeName)
		{
			System.Type dataType;

			// get the ViewAttribute
			ViewAttribute attribute = (ViewAttribute) this._dataGridView.ResultAttributes[attributeName];
			if (attribute != null && attribute is ViewSimpleAttribute)
            {
                ViewSimpleAttribute simpleAttribute = (ViewSimpleAttribute) attribute;
                if (!simpleAttribute.HasEnumConstraint)
                {
                    dataType = ConvertDataType(attribute.DataType);
                }
                else
                {
                    dataType = ConvertDataType(ViewDataType.String);
                }
            }
			else
			{
				dataType = ConvertDataType(ViewDataType.String);
			}

			return dataType;
		}

		/// <summary>
		/// Convert a data type from ViewDataType enum to a System.Type
		/// </summary>
		/// <param name="dataType">One of ViewDataType enum values.</param>
		/// <returns>A System.Type object.</returns>
		private System.Type ConvertDataType(ViewDataType dataType)
		{
			System.Type sysType = null;

			switch (dataType)
			{
				case ViewDataType.BigInteger:
					sysType = System.Type.GetType("System.Int64");
					break;

				case ViewDataType.Boolean:
					sysType = System.Type.GetType("System.String");
					break;

				case ViewDataType.Byte:
					sysType = System.Type.GetType("System.String");
					break;

				case ViewDataType.Date:
					sysType = System.Type.GetType("System.String");
					break;

				case ViewDataType.DateTime:
					sysType = System.Type.GetType("System.String");
					break;

				case ViewDataType.Decimal:
					sysType = System.Type.GetType("System.Decimal");
					break;

				case ViewDataType.Double:
					sysType = System.Type.GetType("System.Double");
					break;

				case ViewDataType.Float:
					sysType = System.Type.GetType("System.Single");
					break;

				case ViewDataType.Integer:
					sysType = System.Type.GetType("System.Int32");
					break;

				case ViewDataType.String:
					sysType = System.Type.GetType("System.String");
					break;

				case ViewDataType.Text:
					sysType = System.Type.GetType("System.String");
					break;

				default:
					sysType = System.Type.GetType("System.String");
					break;
			}

			return sysType;
		}

		/// <summary>
		/// Gets the ViewAttribute object representing the sorting column in the datagrid
		/// </summary>
		/// <returns>A ViewAttribute object</returns>
		private ViewAttribute GetSortingAttribute()
		{
			ColumnInfo sortedColumnInfo = GetSortingColumnInfo();

			ViewAttribute sortedAttribute = (ViewAttribute) _dataGridView.ResultAttributes[sortedColumnInfo.Name];

            if (sortedAttribute != null && sortedAttribute is ViewSimpleAttribute)
			{
                ViewSimpleAttribute simpleAttribute = (ViewSimpleAttribute) sortedAttribute;
                return (ViewAttribute) simpleAttribute.Clone();
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Get the ColumnInfo representing the sorting column in the data grid
		/// </summary>
		/// <returns>The ColumnInfo object, null if not found</returns>
		private ColumnInfo GetSortingColumnInfo()
		{
			ColumnInfo sortedColumnInfo = null;
			int colIndex = 0;
			foreach (ColumnInfo columnInfo in _columnInfos)
			{
				if (columnInfo.IsChecked)
				{
					if (colIndex == dataGrid.SortColumnIndex)
					{
						sortedColumnInfo = columnInfo;
						break;
					}

					colIndex++;
				}
			}

			return sortedColumnInfo;
		}

        /// <summary>
        /// Get the name of a column based on a given column index
        /// </summary>
        /// <returns>The name of the column, null if not found</returns>
        private string GetColumnNameByIndex(int index)
        {
            string colName = null;
            int colIndex = 0;
            foreach (ColumnInfo columnInfo in _columnInfos)
            {
                if (columnInfo.IsChecked)
                {
                    if (colIndex == index)
                    {
                        colName = columnInfo.Name;
                        break;
                    }

                    colIndex++;
                }
            }

            return colName;
        }

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region 组件设计器生成的代码
		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器 
		/// 修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataGridControl));
            Newtera.DataGridActiveX.GridRange gridRange1 = new Newtera.DataGridActiveX.GridRange();
            this.toolBarImageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.searchMenuItem = new System.Windows.Forms.MenuItem();
            this.chartWizardMenuItem = new System.Windows.Forms.MenuItem();
            this.quickChartMenuItem = new System.Windows.Forms.MenuItem();
            this.exportDataMenuItem = new System.Windows.Forms.MenuItem();
            this.computeMenuItem = new System.Windows.Forms.MenuItem();
            this.saveMenuItem = new System.Windows.Forms.MenuItem();
            this.configColumnMenuItem = new System.Windows.Forms.MenuItem();
            this.refreshMenuItem = new System.Windows.Forms.MenuItem();
            this.maximizeMenuItem = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.maximizeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.quickChartButton = new System.Windows.Forms.Button();
            this.pivotButton = new System.Windows.Forms.Button();
            this.computeButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.configButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.chartButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGrid = new Newtera.DataGridActiveX.SelectionDataGrid();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mainPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBarImageList
            // 
            this.toolBarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolBarImageList.ImageStream")));
            this.toolBarImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.toolBarImageList.Images.SetKeyName(0, "");
            this.toolBarImageList.Images.SetKeyName(1, "");
            this.toolBarImageList.Images.SetKeyName(2, "");
            this.toolBarImageList.Images.SetKeyName(3, "");
            this.toolBarImageList.Images.SetKeyName(4, "");
            this.toolBarImageList.Images.SetKeyName(5, "");
            this.toolBarImageList.Images.SetKeyName(6, "");
            this.toolBarImageList.Images.SetKeyName(7, "123.ico");
            this.toolBarImageList.Images.SetKeyName(8, "template_16x16.bmp");
            this.toolBarImageList.Images.SetKeyName(9, "save.jpg");
            this.toolBarImageList.Images.SetKeyName(10, "PivotGrid.bmp");
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.searchMenuItem,
            this.chartWizardMenuItem,
            this.quickChartMenuItem,
            this.exportDataMenuItem,
            this.computeMenuItem,
            this.saveMenuItem,
            this.configColumnMenuItem,
            this.refreshMenuItem,
            this.maximizeMenuItem});
            // 
            // searchMenuItem
            // 
            this.searchMenuItem.Index = 0;
            resources.ApplyResources(this.searchMenuItem, "searchMenuItem");
            this.searchMenuItem.Click += new System.EventHandler(this.searchMenuItem_Click);
            // 
            // chartWizardMenuItem
            // 
            this.chartWizardMenuItem.Index = 1;
            this.chartWizardMenuItem.MergeOrder = 1;
            resources.ApplyResources(this.chartWizardMenuItem, "chartWizardMenuItem");
            this.chartWizardMenuItem.Click += new System.EventHandler(this.chartWizardMenuItem_Click);
            // 
            // quickChartMenuItem
            // 
            this.quickChartMenuItem.Index = 2;
            this.quickChartMenuItem.MergeOrder = 2;
            resources.ApplyResources(this.quickChartMenuItem, "quickChartMenuItem");
            this.quickChartMenuItem.Click += new System.EventHandler(this.quickChartMenuItem_Click);
            // 
            // exportDataMenuItem
            // 
            this.exportDataMenuItem.Index = 3;
            this.exportDataMenuItem.MergeOrder = 3;
            resources.ApplyResources(this.exportDataMenuItem, "exportDataMenuItem");
            this.exportDataMenuItem.Click += new System.EventHandler(this.exportDataMenuItem_Click);
            // 
            // computeMenuItem
            // 
            this.computeMenuItem.Index = 4;
            this.computeMenuItem.MergeOrder = 4;
            resources.ApplyResources(this.computeMenuItem, "computeMenuItem");
            this.computeMenuItem.Click += new System.EventHandler(this.computeMenuItem_Click);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Index = 5;
            this.saveMenuItem.MergeOrder = 5;
            resources.ApplyResources(this.saveMenuItem, "saveMenuItem");
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // configColumnMenuItem
            // 
            this.configColumnMenuItem.Index = 6;
            this.configColumnMenuItem.MergeOrder = 6;
            resources.ApplyResources(this.configColumnMenuItem, "configColumnMenuItem");
            this.configColumnMenuItem.Click += new System.EventHandler(this.configColumnMenuItem_Click);
            // 
            // refreshMenuItem
            // 
            this.refreshMenuItem.Index = 7;
            this.refreshMenuItem.MergeOrder = 7;
            resources.ApplyResources(this.refreshMenuItem, "refreshMenuItem");
            this.refreshMenuItem.Click += new System.EventHandler(this.refreshMenuItem_Click);
            // 
            // maximizeMenuItem
            // 
            this.maximizeMenuItem.Index = 8;
            this.maximizeMenuItem.MergeOrder = 8;
            resources.ApplyResources(this.maximizeMenuItem, "maximizeMenuItem");
            this.maximizeMenuItem.Click += new System.EventHandler(this.maximizeMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.maximizeButton);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.quickChartButton);
            this.panel1.Controls.Add(this.pivotButton);
            this.panel1.Controls.Add(this.computeButton);
            this.panel1.Controls.Add(this.refreshButton);
            this.panel1.Controls.Add(this.configButton);
            this.panel1.Controls.Add(this.exportButton);
            this.panel1.Controls.Add(this.chartButton);
            this.panel1.Controls.Add(this.searchButton);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // maximizeButton
            // 
            resources.ApplyResources(this.maximizeButton, "maximizeButton");
            this.maximizeButton.ImageList = this.toolBarImageList;
            this.maximizeButton.Name = "maximizeButton";
            this.toolTip.SetToolTip(this.maximizeButton, resources.GetString("maximizeButton.ToolTip"));
            this.maximizeButton.Click += new System.EventHandler(this.maximizeButton_Click);
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.ImageList = this.toolBarImageList;
            this.saveButton.Name = "saveButton";
            this.toolTip.SetToolTip(this.saveButton, resources.GetString("saveButton.ToolTip"));
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // quickChartButton
            // 
            resources.ApplyResources(this.quickChartButton, "quickChartButton");
            this.quickChartButton.ImageList = this.toolBarImageList;
            this.quickChartButton.Name = "quickChartButton";
            this.toolTip.SetToolTip(this.quickChartButton, resources.GetString("quickChartButton.ToolTip"));
            this.quickChartButton.Click += new System.EventHandler(this.quickChartButton_Click);
            // 
            // pivotButton
            // 
            resources.ApplyResources(this.pivotButton, "pivotButton");
            this.pivotButton.ImageList = this.toolBarImageList;
            this.pivotButton.Name = "pivotButton";
            this.toolTip.SetToolTip(this.pivotButton, resources.GetString("pivotButton.ToolTip"));
            this.pivotButton.UseVisualStyleBackColor = true;
            this.pivotButton.Click += new System.EventHandler(this.pivotButton_Click);
            // 
            // computeButton
            // 
            resources.ApplyResources(this.computeButton, "computeButton");
            this.computeButton.ImageList = this.toolBarImageList;
            this.computeButton.Name = "computeButton";
            this.toolTip.SetToolTip(this.computeButton, resources.GetString("computeButton.ToolTip"));
            this.computeButton.Click += new System.EventHandler(this.computeButton_Click);
            // 
            // refreshButton
            // 
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.refreshButton.ImageList = this.toolBarImageList;
            this.refreshButton.Name = "refreshButton";
            this.toolTip.SetToolTip(this.refreshButton, resources.GetString("refreshButton.ToolTip"));
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // configButton
            // 
            resources.ApplyResources(this.configButton, "configButton");
            this.configButton.ImageList = this.toolBarImageList;
            this.configButton.Name = "configButton";
            this.toolTip.SetToolTip(this.configButton, resources.GetString("configButton.ToolTip"));
            this.configButton.Click += new System.EventHandler(this.configButton_Click);
            // 
            // exportButton
            // 
            resources.ApplyResources(this.exportButton, "exportButton");
            this.exportButton.ImageList = this.toolBarImageList;
            this.exportButton.Name = "exportButton";
            this.toolTip.SetToolTip(this.exportButton, resources.GetString("exportButton.ToolTip"));
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // chartButton
            // 
            resources.ApplyResources(this.chartButton, "chartButton");
            this.chartButton.ImageList = this.toolBarImageList;
            this.chartButton.Name = "chartButton";
            this.toolTip.SetToolTip(this.chartButton, resources.GetString("chartButton.ToolTip"));
            this.chartButton.Click += new System.EventHandler(this.chartButton_Click);
            // 
            // searchButton
            // 
            resources.ApplyResources(this.searchButton, "searchButton");
            this.searchButton.ImageList = this.toolBarImageList;
            this.searchButton.Name = "searchButton";
            this.toolTip.SetToolTip(this.searchButton, resources.GetString("searchButton.ToolTip"));
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGrid);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // dataGrid
            // 
            this.dataGrid.AllowNavigation = false;
            this.dataGrid.DataMember = "";
            resources.ApplyResources(this.dataGrid, "dataGrid");
            this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid.Name = "dataGrid";
            gridRange1.Bottom = -1;
            gridRange1.Left = -1;
            gridRange1.Right = -1;
            gridRange1.Top = -1;
            this.dataGrid.SelectedRange = gridRange1;
            this.dataGrid.SortColumnIndex = -1;
            this.dataGrid.CellClick += new Newtera.DataGridActiveX.DataGridCellClickEventHandler(this.dataGrid_CellClick);
            // 
            // mainPanel
            // 
            this.mainPanel.ContextMenu = this.contextMenu;
            this.mainPanel.Controls.Add(this.panel2);
            this.mainPanel.Controls.Add(this.panel1);
            resources.ApplyResources(this.mainPanel, "mainPanel");
            this.mainPanel.Name = "mainPanel";
            // 
            // DataGridControl
            // 
            this.Controls.Add(this.mainPanel);
            resources.ApplyResources(this, "$this");
            this.Name = "DataGridControl";
            this.Load += new System.EventHandler(this.DataGridControl_Load);
            this.VisibleChanged += new System.EventHandler(this.DataGridControl_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void DataGridControl_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible && !this.DesignMode)
			{
				LoadData();
			}
		}

		private void DataGridControl_Load(object sender, System.EventArgs e)
		{
			// TODO, select a culture based on the IE culture
			//System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN");
			this.searchButton.Focus();

            // set the web service to the image info cache
            ImageInfoCache.Instance.WebService = CreateActiveXControlWebService();

            // clear XQuery
            XQuery = null;
		}

		private void chartWizardMenuItem_Click(object sender, System.EventArgs e)
		{
		}

		private void configColumnMenuItem_Click(object sender, System.EventArgs e)
		{
			ConfigTable();
		}

		private void refreshMenuItem_Click(object sender, System.EventArgs e)
		{
			RefreshData();
		}

		private void searchMenuItem_Click(object sender, System.EventArgs e)
		{
			LaunchSearchDialog();
		}

		private void exportDataMenuItem_Click(object sender, System.EventArgs e)
		{
			ExportData();
		}

		private void searchButton_Click(object sender, System.EventArgs e)
		{
			LaunchSearchDialog();
		}

		private void exportButton_Click(object sender, System.EventArgs e)
		{
			ExportData();
		}

		private void refreshButton_Click(object sender, System.EventArgs e)
		{
			RefreshData();
		}

		private void chartButton_Click(object sender, System.EventArgs e)
		{
		}

		private void configButton_Click(object sender, System.EventArgs e)
		{
			ConfigTable();
		}

		private void maximizeButton_Click(object sender, System.EventArgs e)
		{
			ShowFullScreen();
		}

		private void maximizeMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowFullScreen();
		}

        private void computeMenuItem_Click(object sender, EventArgs e)
        {
            ComputeResult(dataGrid.HitColumnIndex);
        }

        private void computeButton_Click(object sender, EventArgs e)
        {
            ComputeResult(-1);
        }

        private void quickChartMenuItem_Click(object sender, EventArgs e)
        {
            ShowTemplates();
        }

        private void quickChartButton_Click(object sender, EventArgs e)
        {
            ShowTemplates();
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            if (_masterDataSet.HasChanges())
            {
                UpdateData();
            }
        }

        private void pivotButton_Click(object sender, EventArgs e)
        {
            ShowPivotGrid();
        }

		#region IDataGridActiveX 成员

		/// <summary>
		/// Gets or sets the type of view to be associated with the DataGrid control
		/// the values are Class, Taxon, or Array
		/// </summary>
		public string ViewType
		{
			get
			{
				return _viewType;
			}
			set
			{
                if (!string.IsNullOrEmpty(value))
                {
                    _viewType = value.ToUpper();
                }
			}
		}

		/// <summary>
		/// Gets or sets the caption of the class that DataGrid is linked with
		/// </summary>
		public string ClassCaption
		{
			get
			{
				return _classCaption;
			}
			set
			{
				_classCaption = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the class that DataGrid is linked with
		/// </summary>
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the taxonomy tree. it could be null
		/// </summary>
		public string TaxonomyName
		{
			get
			{
				return _taxonomy;
			}
			set
			{
				_taxonomy = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the taxon node. it could be null
		/// </summary>
		public string TaxonName
		{
			get
			{
				return _taxonName;
			}
			set
			{
				_taxonName = value;
			}
		}

        /// <summary>
        /// Gets or sets the caption of the taxon node. it could be null
        /// </summary>
        public string TaxonCaption
        {
            get
            {
                return _taxonCaption;
            }
            set
            {
                _taxonCaption = value;
            }
        }

		/// <summary>
		/// Gets or sets the name of the array whose data the DataGrid displays
		/// </summary>
		public string ArrayName
		{
			get
			{
				return _arrayName;
			}
			set
			{
				_arrayName = value;
			}
		}

		/// <summary>
		/// Gets or sets the caption of the array whose data the DataGrid displays
		/// </summary>
		public string ArrayCaption
		{
			get
			{
				return _arrayCaption;
			}
			set
			{
				_arrayCaption = value;
			}
		}

        /// <summary>
        /// Gets or sets the alias of the related class when the view type is "RELATED".
        /// A related class alias is different from a related class name, since a class can
        /// be related to a same class through more than one relationship, therefore,
        /// a alias consists of the related class name and the relationship name
        /// </summary>
        public string RelatedClassAlias
        {
            get
            {
                return _relatedClassAlias;
            }
            set
            {
                _relatedClassAlias = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the related class when the view type is "RELATED".
        /// </summary>
        public string RelatedClassName
        {
            get
            {
                return _relatedClassName;
            }
            set
            {
                _relatedClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets the caption of the related class when the view type is "RELATED".
        /// </summary>
        public string RelatedClassCaption
        {
            get
            {
                return _relatedClassCaption;
            }
            set
            {
                _relatedClassCaption = value;
            }
        }

		/// <summary>
		/// Gets or sets the id of the data instance when view type is "ARRAY" or "RELATED".
		/// </summary>
		public string InstanceId
		{
			get
			{
				return _instanceId;
			}
			set
			{
				if (value != null)
				{
					_instanceId = value.Trim();
				}
				else
				{
					_instanceId = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the connection string that is used to connect the server
		/// </summary>
		public string ConnectionString 
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
			}
		}

		/// <summary>
		/// Gets or sets the instance count of result
		/// </summary>
		public int TotalCount
		{
			get
			{
				return _totalCount;
			}
			set
			{
				_totalCount = value;
			}
		}

		/// <summary>
		/// Gets or sets the XQuery for searching the results
		/// </summary>
		public string XQuery
		{
			get
			{
				return _xquery;
			}
			set
			{
				_xquery = value;
			}
		}

		/// <summary>
		/// Gets or sets the base URL of the web service
		/// </summary>
		public string BaseURL 
		{
			get
			{
				return _baseURL;
			}
			set
			{
				_baseURL = value;
			}
		}

		/// <summary>
		/// Gets or sets the base Path of the web virtual directory
		/// </summary>
		public string BasePath
		{
			get
			{
				return this._basePhysicalPath;
			}
			set
			{
				_basePhysicalPath = value;
			}
		}

        /// <summary>
        /// Convert a row index of displayed row into the row index of corresponding data table row
        /// </summary>
        /// <param name="displayedRowIndex"></param>
        /// <returns></returns>
        private int ConvertToDataTableRowIndex(int displayedRowIndex)
        {
            CurrencyManager cm = (CurrencyManager)dataGrid.BindingContext[this._masterDataSet, this._tableName];
            DataView dv = (DataView)cm.List;
            DataRowView dataRowView = dv[displayedRowIndex];
            DataRow dataRow = dataRowView.Row;

            // convert to the row index in the datatable
            int selectedRowIndex = 0;
            DataTable dataTable = dataRow.Table;
            foreach (DataRow dr in dataTable.Rows)
            {
                if (dr == dataRow)
                {
                    break;
                }

                selectedRowIndex++;
            }

            return selectedRowIndex;
        }

        private void LaucheArrayViewer(ColumnInfo columnInfo, int displayedRow)
        {
            // get corresponding row index in datatable
            int rowIndex = ConvertToDataTableRowIndex(displayedRow);

            DataTable dt = _masterDataSet.Tables[_tableName];
            string instanceId = dt.Rows[rowIndex][OBJ_ID].ToString();

            ViewArrayDataDialog dialog = new ViewArrayDataDialog();
            dialog.ViewType = "Array";
            dialog.BaseURL = this.BaseURL;
            dialog.BasePath = this.BasePath;
            if (this._viewType != "RELATED")
            {
                dialog.ClassName = this.ClassName;
            }
            else
            {
                dialog.ClassName = this.RelatedClassName;
            }
            dialog.ArrayName = columnInfo.Name;
            dialog.ArrayCaption = columnInfo.Caption;
            dialog.ConnectionString = this.ConnectionString;
            dialog.InstanceId = instanceId;

            // register event handler
            dialog.GraphEvent += new GraphCallback(ViewArrayDataDialog_GraphEvent);
            dialog.DownloadFileEvent += new DownloadFileCallback(ViewArrayDataDialog_DownloadFileEvent);
            dialog.DownloadGraphEvent += new DownloadGraphCallback(ViewArrayDataDialog_DownloadGraphEvent);
            dialog.RunAlgorithmEvent += new RunAlgorithmCallback(ViewArrayDataDialog_RunAlgorithmEvent);
            dialog.ShowPivotGridEvent += new ShowPivotGridCallback(ViewArrayDataDialog_ShowPivotGridEvent);

            dialog.Show(); // cannot use ShowDialog because model dialog will prevent downloading file from working.
        }

        private void dataGrid_CellClick(object sender, DataGridCellClickEventArgs e)
        {
            if (this._viewType != "ARRAY" && e.Row >=0 && e.Col >= 0)
            {
                ColumnInfo selectedColumn = null;
                int index = 0;
                foreach (ColumnInfo colInfo in _columnInfos)
                {
                    if (colInfo.IsChecked)
                    {
                        if (index == e.Col)
                        {
                            selectedColumn = colInfo;
                            break;
                        }

                        index++;
                    }
                }

                if (selectedColumn != null && selectedColumn.IsArray)
                {
                    LaucheArrayViewer(selectedColumn, e.Row);
                }
            }
        }

        private void ViewArrayDataDialog_GraphEvent(int graphType)
        {
            FireGraphEvent();
        }

        private void ViewArrayDataDialog_DownloadFileEvent(string fileName)
        {
            this._downLoadfile = fileName;
            FireDownloadFileEvent();
        }

        private void ViewArrayDataDialog_DownloadGraphEvent(string formatName)
        {
            FireDownloadGraphEvent(formatName, null);
        }

        private void ViewArrayDataDialog_RunAlgorithmEvent(string displayUrl)
        {
            FireRunAlgorithmEvent(displayUrl);
        }

        private void ComputeResultDialog_RunAlgorithmEvent(string displayUrl)
        {
            FireRunAlgorithmEvent(displayUrl);
        }

        private void ViewArrayDataDialog_ShowPivotGridEvent(string dataSourceId)
        {
            _dataSourceId = dataSourceId;
            FireShowPivotGridEvent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_masterDataSet.HasChanges())
            {
                UpdateData();
            }
        }

		#endregion

    }
}

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Web.Services.Dime;

using Newtera.WinClientCommon;
using Newtera.WindowsControl;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.DataView.Validate;
using Newtera.Common.MetaData.Rules;
using Newtera.Common.MetaData.FileType;
using Newtera.Common.Attachment;
using Newtera.Export;
using Newtera.DataGridActiveX;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DataEditor.
	/// </summary>
	public class DataViewer : System.Windows.Forms.Form, IDataGridControl
	{
		private const int DATA_THRESHHOLD = 10000;

		private MetaDataModel _metaData = null;
		private MetaDataTreeBuilder _treeBuilder;
		private MenuItemStates _menuItemStates;
		private CMDataServiceStub _dataService;
		private AttachmentServiceStub _attachmentService;
        private ActiveXControlServiceStub _activeXService;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private bool _isCancelled;
		private int _pageSize = 100;
		private bool _isDefaultDataView;
        private bool _isDBA;

		private ResultViewType _currentSubView;
		private string _attachmentFileName;
		private string _graphFileName;
		private bool _isAttachmentChanged;
		private SchemaModelElementCollection _fullTextIndexClasses;
		private int _currentIndexClassPosition;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ImageList treeViewImageList;
        private System.Windows.Forms.Panel resultPanel;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem searchMenuItem;
        private System.Windows.Forms.ContextMenu treeViewContextMenu;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TabControl resultTabControl;
		private System.Windows.Forms.TabPage instanceViewTabPage;
		private System.Windows.Forms.PropertyGrid instanceViewGrid;
		private System.Windows.Forms.TabPage attachmentTabPage;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem newInstanceMenuItem;
		private System.Windows.Forms.MenuItem saveInstanceMenuItem;
		private System.Windows.Forms.MenuItem saveInstanceAsMenuItem;
        private System.Windows.Forms.MenuItem deleteInstanceMenuItem;
		private System.Windows.Forms.MenuItem nextRowMenuItem;
		private System.Windows.Forms.MenuItem prevRowMenuItem;
		private System.Windows.Forms.MenuItem nextPageMenuItem;
        private System.Windows.Forms.MenuItem prevPageMenuItem;
		private System.Windows.Forms.MenuItem tvNewDataViewMenuItem;
		private System.Windows.Forms.MenuItem tvModifyDataViewMenuItem;
		private System.Windows.Forms.MenuItem tvDeleteDataViewMenuItem;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem newDataViewMenuItem;
		private System.Windows.Forms.MenuItem modifyDataViewMenuItem;
		private System.Windows.Forms.MenuItem deleteDataViewMenuItem;
		private System.Windows.Forms.MenuItem saveDataViewMenuItem;
		private System.Windows.Forms.MenuItem runXQueryMenuItem;
		private System.Windows.Forms.ListView attachmentListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button addAttachmentButton;
		private System.Windows.Forms.Button deleteAttachmentButton;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ImageList fileTypeSmallImageList;
        private System.Windows.Forms.ImageList fileTypeLargeImageList;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem deleteAllMenuItem;
		private System.Windows.Forms.Button downloadButton;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem tvBuildFullTextIndexMenuItem;
		private System.Windows.Forms.MenuItem buildFullTextIndexMenuItem;
		private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem showAllInstancesMenuItem;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem chartMenuItem;
        private System.Windows.Forms.MenuItem exportMenuItem;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.MenuItem defaultViewMenuItem;
        private System.Windows.Forms.MenuItem detailedViewMenuItem;
        private Newtera.WindowsControl.ResultDataControl resultDataControl1;
        private ContextMenuStrip dataGridContextMenuStrip;
        private ToolStripMenuItem dgNewToolStripMenuItem;
        private ToolStripMenuItem dgDuplicateInstanceToolStripMenuItem;
        private ToolStripMenuItem dgDeleteToolStripMenuItem;
        private ToolStripMenuItem dgDeleteAllToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem dgDefaultViewToolStripMenuItem;
        private ToolStripMenuItem dgDetailedViewToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem dgShowAllInstancesToolStripMenuItem;
        private ToolStripMenuItem dgExportDataToolStripMenuItem;
        private MenuItem pivotTableMenuItem;
        private MenuItem menuItem8;
        private MenuItem loggingMenuItem;
        private MenuItem menuItem13;
        private MenuItem tvLoggingMenuItem;
        private ToolStripMenuItem exportXMLToolStripMenuItem;
		private System.ComponentModel.IContainer components;

		public DataViewer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_treeBuilder = new MetaDataTreeBuilder();
			_treeBuilder.IsAttributesShown = false;
			_treeBuilder.IsConstraintsShown = false;
			_treeBuilder.IsDataViewsShown = true;
			_menuItemStates = new MenuItemStates();
			_dataService = new CMDataServiceStub();
			_attachmentService = new AttachmentServiceStub();
			_currentSubView = ResultViewType.InstanceView;
			_attachmentFileName = null;
			_graphFileName = null;
			_isAttachmentChanged = false;
			_isDefaultDataView = true;
			_workInProgressDialog = new WorkInProgressDialog();
            _isDBA = true;

            this.resultDataControl1.MenuItemStates = _menuItemStates;
		}

		~DataViewer()
		{
            if (_workInProgressDialog != null)
            {
                _workInProgressDialog.Dispose();
            }
		}

		/// <summary>
		/// Clean up any resources being used.
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

		/// <summary>
		/// Gets or sets the meta data being loaded in Data Viewer
		/// </summary>
		public MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;

                this.resultDataControl1.MetaData = value;

				// listen to the value changed event from the data views
				_metaData.DataViews.ValueChanged += new EventHandler(this.DataViewChangedHandler);
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the currently loggin user
        /// has DBA role.
        /// </summary>
        public bool IsDBAUser
        {
            get
            {
                return _isDBA;
            }
            set
            {
                _isDBA = value;
            }
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
				lock(_workInProgressDialog)
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataViewer));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.exportMenuItem = new System.Windows.Forms.MenuItem();
            this.saveDataViewMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.searchMenuItem = new System.Windows.Forms.MenuItem();
            this.showAllInstancesMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.newInstanceMenuItem = new System.Windows.Forms.MenuItem();
            this.saveInstanceMenuItem = new System.Windows.Forms.MenuItem();
            this.saveInstanceAsMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteInstanceMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteAllMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.newDataViewMenuItem = new System.Windows.Forms.MenuItem();
            this.modifyDataViewMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteDataViewMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.chartMenuItem = new System.Windows.Forms.MenuItem();
            this.pivotTableMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.defaultViewMenuItem = new System.Windows.Forms.MenuItem();
            this.detailedViewMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem16 = new System.Windows.Forms.MenuItem();
            this.nextRowMenuItem = new System.Windows.Forms.MenuItem();
            this.prevRowMenuItem = new System.Windows.Forms.MenuItem();
            this.nextPageMenuItem = new System.Windows.Forms.MenuItem();
            this.prevPageMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.loggingMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.runXQueryMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.buildFullTextIndexMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.treeView = new System.Windows.Forms.TreeView();
            this.treeViewContextMenu = new System.Windows.Forms.ContextMenu();
            this.tvNewDataViewMenuItem = new System.Windows.Forms.MenuItem();
            this.tvModifyDataViewMenuItem = new System.Windows.Forms.MenuItem();
            this.tvDeleteDataViewMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.tvBuildFullTextIndexMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.tvLoggingMenuItem = new System.Windows.Forms.MenuItem();
            this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.resultPanel = new System.Windows.Forms.Panel();
            this.resultDataControl1 = new Newtera.WindowsControl.ResultDataControl();
            this.dataGridContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dgNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgDuplicateInstanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgDeleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.dgDefaultViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgDetailedViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.dgShowAllInstancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgExportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.resultTabControl = new System.Windows.Forms.TabControl();
            this.instanceViewTabPage = new System.Windows.Forms.TabPage();
            this.instanceViewGrid = new System.Windows.Forms.PropertyGrid();
            this.attachmentTabPage = new System.Windows.Forms.TabPage();
            this.deleteAttachmentButton = new System.Windows.Forms.Button();
            this.addAttachmentButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.attachmentListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileTypeLargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.fileTypeSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.resultPanel.SuspendLayout();
            this.dataGridContextMenuStrip.SuspendLayout();
            this.resultTabControl.SuspendLayout();
            this.instanceViewTabPage.SuspendLayout();
            this.attachmentTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem3,
            this.menuItem4});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.exportMenuItem,
            this.saveDataViewMenuItem,
            this.menuItem10});
            this.menuItem1.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // exportMenuItem
            // 
            resources.ApplyResources(this.exportMenuItem, "exportMenuItem");
            this.exportMenuItem.Index = 0;
            this.exportMenuItem.MergeOrder = 10;
            this.exportMenuItem.Click += new System.EventHandler(this.exportMenuItem_Click);
            // 
            // saveDataViewMenuItem
            // 
            resources.ApplyResources(this.saveDataViewMenuItem, "saveDataViewMenuItem");
            this.saveDataViewMenuItem.Index = 1;
            this.saveDataViewMenuItem.MergeOrder = 11;
            this.saveDataViewMenuItem.Click += new System.EventHandler(this.saveDataViewMenuItem_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 2;
            this.menuItem10.MergeOrder = 12;
            resources.ApplyResources(this.menuItem10, "menuItem10");
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem5,
            this.searchMenuItem,
            this.showAllInstancesMenuItem,
            this.menuItem6,
            this.newInstanceMenuItem,
            this.saveInstanceMenuItem,
            this.saveInstanceAsMenuItem,
            this.deleteInstanceMenuItem,
            this.deleteAllMenuItem,
            this.menuItem7,
            this.newDataViewMenuItem,
            this.modifyDataViewMenuItem,
            this.deleteDataViewMenuItem});
            this.menuItem2.MergeOrder = 1;
            this.menuItem2.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 0;
            this.menuItem5.MergeOrder = 7;
            resources.ApplyResources(this.menuItem5, "menuItem5");
            // 
            // searchMenuItem
            // 
            resources.ApplyResources(this.searchMenuItem, "searchMenuItem");
            this.searchMenuItem.Index = 1;
            this.searchMenuItem.MergeOrder = 8;
            this.searchMenuItem.Click += new System.EventHandler(this.searchMenuItem_Click);
            // 
            // showAllInstancesMenuItem
            // 
            resources.ApplyResources(this.showAllInstancesMenuItem, "showAllInstancesMenuItem");
            this.showAllInstancesMenuItem.Index = 2;
            this.showAllInstancesMenuItem.MergeOrder = 9;
            this.showAllInstancesMenuItem.Click += new System.EventHandler(this.showAllInstancesMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 3;
            this.menuItem6.MergeOrder = 10;
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // newInstanceMenuItem
            // 
            this.newInstanceMenuItem.Index = 4;
            this.newInstanceMenuItem.MergeOrder = 11;
            resources.ApplyResources(this.newInstanceMenuItem, "newInstanceMenuItem");
            this.newInstanceMenuItem.Click += new System.EventHandler(this.newInstanceMenuItem_Click);
            // 
            // saveInstanceMenuItem
            // 
            this.saveInstanceMenuItem.Index = 5;
            this.saveInstanceMenuItem.MergeOrder = 12;
            resources.ApplyResources(this.saveInstanceMenuItem, "saveInstanceMenuItem");
            this.saveInstanceMenuItem.Click += new System.EventHandler(this.saveInstanceMenuItem_Click);
            // 
            // saveInstanceAsMenuItem
            // 
            this.saveInstanceAsMenuItem.Index = 6;
            this.saveInstanceAsMenuItem.MergeOrder = 13;
            resources.ApplyResources(this.saveInstanceAsMenuItem, "saveInstanceAsMenuItem");
            this.saveInstanceAsMenuItem.Click += new System.EventHandler(this.saveInstanceAsMenuItem_Click);
            // 
            // deleteInstanceMenuItem
            // 
            this.deleteInstanceMenuItem.Index = 7;
            this.deleteInstanceMenuItem.MergeOrder = 14;
            resources.ApplyResources(this.deleteInstanceMenuItem, "deleteInstanceMenuItem");
            this.deleteInstanceMenuItem.Click += new System.EventHandler(this.deleteInstanceMenuItem_Click);
            // 
            // deleteAllMenuItem
            // 
            this.deleteAllMenuItem.Index = 8;
            this.deleteAllMenuItem.MergeOrder = 15;
            resources.ApplyResources(this.deleteAllMenuItem, "deleteAllMenuItem");
            this.deleteAllMenuItem.Click += new System.EventHandler(this.deleteAllMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 9;
            this.menuItem7.MergeOrder = 20;
            resources.ApplyResources(this.menuItem7, "menuItem7");
            // 
            // newDataViewMenuItem
            // 
            resources.ApplyResources(this.newDataViewMenuItem, "newDataViewMenuItem");
            this.newDataViewMenuItem.Index = 10;
            this.newDataViewMenuItem.MergeOrder = 31;
            this.newDataViewMenuItem.Click += new System.EventHandler(this.newDataViewMenuItem_Click);
            // 
            // modifyDataViewMenuItem
            // 
            resources.ApplyResources(this.modifyDataViewMenuItem, "modifyDataViewMenuItem");
            this.modifyDataViewMenuItem.Index = 11;
            this.modifyDataViewMenuItem.MergeOrder = 32;
            this.modifyDataViewMenuItem.Click += new System.EventHandler(this.modifyDataViewMenuItem_Click);
            // 
            // deleteDataViewMenuItem
            // 
            resources.ApplyResources(this.deleteDataViewMenuItem, "deleteDataViewMenuItem");
            this.deleteDataViewMenuItem.Index = 12;
            this.deleteDataViewMenuItem.MergeOrder = 33;
            this.deleteDataViewMenuItem.Click += new System.EventHandler(this.deleteDataViewMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.chartMenuItem,
            this.pivotTableMenuItem,
            this.menuItem14,
            this.defaultViewMenuItem,
            this.detailedViewMenuItem,
            this.menuItem16,
            this.nextRowMenuItem,
            this.prevRowMenuItem,
            this.nextPageMenuItem,
            this.prevPageMenuItem,
            this.menuItem8,
            this.loggingMenuItem});
            this.menuItem3.MergeOrder = 2;
            this.menuItem3.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // chartMenuItem
            // 
            this.chartMenuItem.Index = 0;
            this.chartMenuItem.MergeOrder = 10;
            resources.ApplyResources(this.chartMenuItem, "chartMenuItem");
            this.chartMenuItem.Click += new System.EventHandler(this.chartMenuItem_Click);
            // 
            // pivotTableMenuItem
            // 
            resources.ApplyResources(this.pivotTableMenuItem, "pivotTableMenuItem");
            this.pivotTableMenuItem.Index = 1;
            this.pivotTableMenuItem.MergeOrder = 11;
            this.pivotTableMenuItem.Click += new System.EventHandler(this.pivotTableMenuItem_Click);
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 2;
            this.menuItem14.MergeOrder = 12;
            resources.ApplyResources(this.menuItem14, "menuItem14");
            // 
            // defaultViewMenuItem
            // 
            this.defaultViewMenuItem.Checked = true;
            this.defaultViewMenuItem.Index = 3;
            this.defaultViewMenuItem.MergeOrder = 13;
            resources.ApplyResources(this.defaultViewMenuItem, "defaultViewMenuItem");
            this.defaultViewMenuItem.Click += new System.EventHandler(this.defaultViewMenuItem_Click);
            // 
            // detailedViewMenuItem
            // 
            this.detailedViewMenuItem.Index = 4;
            this.detailedViewMenuItem.MergeOrder = 14;
            resources.ApplyResources(this.detailedViewMenuItem, "detailedViewMenuItem");
            this.detailedViewMenuItem.Click += new System.EventHandler(this.detailedViewMenuItem_Click);
            // 
            // menuItem16
            // 
            this.menuItem16.Index = 5;
            this.menuItem16.MergeOrder = 15;
            resources.ApplyResources(this.menuItem16, "menuItem16");
            // 
            // nextRowMenuItem
            // 
            resources.ApplyResources(this.nextRowMenuItem, "nextRowMenuItem");
            this.nextRowMenuItem.Index = 6;
            this.nextRowMenuItem.MergeOrder = 16;
            this.nextRowMenuItem.Click += new System.EventHandler(this.nextRowMenuItem_Click);
            // 
            // prevRowMenuItem
            // 
            resources.ApplyResources(this.prevRowMenuItem, "prevRowMenuItem");
            this.prevRowMenuItem.Index = 7;
            this.prevRowMenuItem.MergeOrder = 17;
            this.prevRowMenuItem.Click += new System.EventHandler(this.prevRowMenuItem_Click);
            // 
            // nextPageMenuItem
            // 
            resources.ApplyResources(this.nextPageMenuItem, "nextPageMenuItem");
            this.nextPageMenuItem.Index = 8;
            this.nextPageMenuItem.MergeOrder = 18;
            this.nextPageMenuItem.Click += new System.EventHandler(this.nextPageMenuItem_Click);
            // 
            // prevPageMenuItem
            // 
            resources.ApplyResources(this.prevPageMenuItem, "prevPageMenuItem");
            this.prevPageMenuItem.Index = 9;
            this.prevPageMenuItem.MergeOrder = 19;
            this.prevPageMenuItem.Click += new System.EventHandler(this.prevPageMenuItem_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 10;
            this.menuItem8.MergeOrder = 20;
            resources.ApplyResources(this.menuItem8, "menuItem8");
            // 
            // loggingMenuItem
            // 
            this.loggingMenuItem.Index = 11;
            this.loggingMenuItem.MergeOrder = 21;
            resources.ApplyResources(this.loggingMenuItem, "loggingMenuItem");
            this.loggingMenuItem.Click += new System.EventHandler(this.loggingMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.runXQueryMenuItem,
            this.menuItem9,
            this.buildFullTextIndexMenuItem,
            this.menuItem11});
            this.menuItem4.MergeOrder = 3;
            this.menuItem4.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // runXQueryMenuItem
            // 
            this.runXQueryMenuItem.Index = 0;
            resources.ApplyResources(this.runXQueryMenuItem, "runXQueryMenuItem");
            this.runXQueryMenuItem.Click += new System.EventHandler(this.runXQueryMenuItem_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 1;
            this.menuItem9.MergeOrder = 1;
            resources.ApplyResources(this.menuItem9, "menuItem9");
            // 
            // buildFullTextIndexMenuItem
            // 
            this.buildFullTextIndexMenuItem.Index = 2;
            this.buildFullTextIndexMenuItem.MergeOrder = 2;
            resources.ApplyResources(this.buildFullTextIndexMenuItem, "buildFullTextIndexMenuItem");
            this.buildFullTextIndexMenuItem.Click += new System.EventHandler(this.builFullTextIndexMenuItem_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 3;
            this.menuItem11.MergeOrder = 3;
            resources.ApplyResources(this.menuItem11, "menuItem11");
            // 
            // treeView
            // 
            this.treeView.ContextMenu = this.treeViewContextMenu;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.HideSelection = false;
            this.treeView.ImageList = this.treeViewImageList;
            this.treeView.ItemHeight = 16;
            this.treeView.Name = "treeView";
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // treeViewContextMenu
            // 
            this.treeViewContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.tvNewDataViewMenuItem,
            this.tvModifyDataViewMenuItem,
            this.tvDeleteDataViewMenuItem,
            this.menuItem12,
            this.tvBuildFullTextIndexMenuItem,
            this.menuItem13,
            this.tvLoggingMenuItem});
            // 
            // tvNewDataViewMenuItem
            // 
            resources.ApplyResources(this.tvNewDataViewMenuItem, "tvNewDataViewMenuItem");
            this.tvNewDataViewMenuItem.Index = 0;
            this.tvNewDataViewMenuItem.Click += new System.EventHandler(this.tvNewDataViewMenuItem_Click);
            // 
            // tvModifyDataViewMenuItem
            // 
            resources.ApplyResources(this.tvModifyDataViewMenuItem, "tvModifyDataViewMenuItem");
            this.tvModifyDataViewMenuItem.Index = 1;
            this.tvModifyDataViewMenuItem.Click += new System.EventHandler(this.tvModifyDataViewMenuItem_Click);
            // 
            // tvDeleteDataViewMenuItem
            // 
            resources.ApplyResources(this.tvDeleteDataViewMenuItem, "tvDeleteDataViewMenuItem");
            this.tvDeleteDataViewMenuItem.Index = 2;
            this.tvDeleteDataViewMenuItem.Click += new System.EventHandler(this.tvDeleteDataViewMenuItem_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 3;
            resources.ApplyResources(this.menuItem12, "menuItem12");
            // 
            // tvBuildFullTextIndexMenuItem
            // 
            this.tvBuildFullTextIndexMenuItem.Index = 4;
            resources.ApplyResources(this.tvBuildFullTextIndexMenuItem, "tvBuildFullTextIndexMenuItem");
            this.tvBuildFullTextIndexMenuItem.Click += new System.EventHandler(this.tvBuildFullTextIndexMenuItem_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 5;
            resources.ApplyResources(this.menuItem13, "menuItem13");
            // 
            // tvLoggingMenuItem
            // 
            this.tvLoggingMenuItem.Index = 6;
            resources.ApplyResources(this.tvLoggingMenuItem, "tvLoggingMenuItem");
            this.tvLoggingMenuItem.Click += new System.EventHandler(this.tvLoggingMenuItem_Click);
            // 
            // treeViewImageList
            // 
            this.treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImageList.ImageStream")));
            this.treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewImageList.Images.SetKeyName(0, "");
            this.treeViewImageList.Images.SetKeyName(1, "");
            this.treeViewImageList.Images.SetKeyName(2, "");
            this.treeViewImageList.Images.SetKeyName(3, "");
            this.treeViewImageList.Images.SetKeyName(4, "");
            this.treeViewImageList.Images.SetKeyName(5, "");
            this.treeViewImageList.Images.SetKeyName(6, "");
            this.treeViewImageList.Images.SetKeyName(7, "");
            this.treeViewImageList.Images.SetKeyName(8, "");
            this.treeViewImageList.Images.SetKeyName(9, "");
            this.treeViewImageList.Images.SetKeyName(10, "");
            this.treeViewImageList.Images.SetKeyName(11, "");
            this.treeViewImageList.Images.SetKeyName(12, "");
            this.treeViewImageList.Images.SetKeyName(13, "");
            this.treeViewImageList.Images.SetKeyName(14, "");
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // resultPanel
            // 
            this.resultPanel.Controls.Add(this.resultDataControl1);
            resources.ApplyResources(this.resultPanel, "resultPanel");
            this.resultPanel.Name = "resultPanel";
            // 
            // resultDataControl1
            // 
            this.resultDataControl1.ContextMenuStrip = this.dataGridContextMenuStrip;
            this.resultDataControl1.CurrentSlide = null;
            resources.ApplyResources(this.resultDataControl1, "resultDataControl1");
            this.resultDataControl1.MenuItemStates = null;
            this.resultDataControl1.MetaData = null;
            this.resultDataControl1.Name = "resultDataControl1";
            this.resultDataControl1.ServerProxy = null;
            this.resultDataControl1.UserManager = null;
            this.resultDataControl1.UserName = null;
            this.resultDataControl1.RowSelectedIndexChangedEvent += new System.EventHandler(this.resultDataControl1_RowSelectedIndexChangedEvent);
            this.resultDataControl1.RequestForCountEvent += new System.EventHandler(this.resultDataControl1_RequestForCountEvent);
            this.resultDataControl1.RequestForDataEvent += new System.EventHandler(this.resultDataControl1_RequestForDataEvent);
            this.resultDataControl1.DataGridClearedEvent += new System.EventHandler(this.resultDataControl1_DataGridClearedEvent);
            this.resultDataControl1.RequestForSaveInstancesEvent += new System.EventHandler(this.resultDataControl1_RequestForSaveInstancesEvent);
            // 
            // dataGridContextMenuStrip
            // 
            this.dataGridContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dgNewToolStripMenuItem,
            this.dgDuplicateInstanceToolStripMenuItem,
            this.dgDeleteToolStripMenuItem,
            this.dgDeleteAllToolStripMenuItem,
            this.toolStripMenuItem1,
            this.dgDefaultViewToolStripMenuItem,
            this.dgDetailedViewToolStripMenuItem,
            this.toolStripMenuItem2,
            this.dgShowAllInstancesToolStripMenuItem,
            this.dgExportDataToolStripMenuItem,
            this.exportXMLToolStripMenuItem});
            this.dataGridContextMenuStrip.Name = "dataGridContextMenuStrip";
            resources.ApplyResources(this.dataGridContextMenuStrip, "dataGridContextMenuStrip");
            // 
            // dgNewToolStripMenuItem
            // 
            this.dgNewToolStripMenuItem.Name = "dgNewToolStripMenuItem";
            resources.ApplyResources(this.dgNewToolStripMenuItem, "dgNewToolStripMenuItem");
            this.dgNewToolStripMenuItem.Click += new System.EventHandler(this.dgNewToolStripMenuItem_Click);
            // 
            // dgDuplicateInstanceToolStripMenuItem
            // 
            this.dgDuplicateInstanceToolStripMenuItem.Name = "dgDuplicateInstanceToolStripMenuItem";
            resources.ApplyResources(this.dgDuplicateInstanceToolStripMenuItem, "dgDuplicateInstanceToolStripMenuItem");
            this.dgDuplicateInstanceToolStripMenuItem.Click += new System.EventHandler(this.dgDuplicateInstanceToolStripMenuItem_Click);
            // 
            // dgDeleteToolStripMenuItem
            // 
            this.dgDeleteToolStripMenuItem.Name = "dgDeleteToolStripMenuItem";
            resources.ApplyResources(this.dgDeleteToolStripMenuItem, "dgDeleteToolStripMenuItem");
            this.dgDeleteToolStripMenuItem.Click += new System.EventHandler(this.dgDeleteToolStripMenuItem_Click);
            // 
            // dgDeleteAllToolStripMenuItem
            // 
            this.dgDeleteAllToolStripMenuItem.Name = "dgDeleteAllToolStripMenuItem";
            resources.ApplyResources(this.dgDeleteAllToolStripMenuItem, "dgDeleteAllToolStripMenuItem");
            this.dgDeleteAllToolStripMenuItem.Click += new System.EventHandler(this.dgDeleteAllToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // dgDefaultViewToolStripMenuItem
            // 
            this.dgDefaultViewToolStripMenuItem.Checked = true;
            this.dgDefaultViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dgDefaultViewToolStripMenuItem.Name = "dgDefaultViewToolStripMenuItem";
            resources.ApplyResources(this.dgDefaultViewToolStripMenuItem, "dgDefaultViewToolStripMenuItem");
            this.dgDefaultViewToolStripMenuItem.Click += new System.EventHandler(this.dgDefaultViewToolStripMenuItem_Click);
            // 
            // dgDetailedViewToolStripMenuItem
            // 
            this.dgDetailedViewToolStripMenuItem.Name = "dgDetailedViewToolStripMenuItem";
            resources.ApplyResources(this.dgDetailedViewToolStripMenuItem, "dgDetailedViewToolStripMenuItem");
            this.dgDetailedViewToolStripMenuItem.Click += new System.EventHandler(this.dgDetailedViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // dgShowAllInstancesToolStripMenuItem
            // 
            this.dgShowAllInstancesToolStripMenuItem.Name = "dgShowAllInstancesToolStripMenuItem";
            resources.ApplyResources(this.dgShowAllInstancesToolStripMenuItem, "dgShowAllInstancesToolStripMenuItem");
            this.dgShowAllInstancesToolStripMenuItem.Click += new System.EventHandler(this.dgShowAllInstancesToolStripMenuItem_Click);
            // 
            // dgExportDataToolStripMenuItem
            // 
            this.dgExportDataToolStripMenuItem.Name = "dgExportDataToolStripMenuItem";
            resources.ApplyResources(this.dgExportDataToolStripMenuItem, "dgExportDataToolStripMenuItem");
            this.dgExportDataToolStripMenuItem.Click += new System.EventHandler(this.dgExportDataToolStripMenuItem_Click);
            // 
            // exportXMLToolStripMenuItem
            // 
            this.exportXMLToolStripMenuItem.Name = "exportXMLToolStripMenuItem";
            resources.ApplyResources(this.exportXMLToolStripMenuItem, "exportXMLToolStripMenuItem");
            this.exportXMLToolStripMenuItem.Click += new System.EventHandler(this.dgExportXMLToolStripMenuItem_Click);
            // 
            // splitter2
            // 
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            // 
            // resultTabControl
            // 
            this.resultTabControl.Controls.Add(this.instanceViewTabPage);
            this.resultTabControl.Controls.Add(this.attachmentTabPage);
            resources.ApplyResources(this.resultTabControl, "resultTabControl");
            this.resultTabControl.Name = "resultTabControl";
            this.resultTabControl.SelectedIndex = 0;
            this.resultTabControl.SelectedIndexChanged += new System.EventHandler(this.resultTabControl_SelectedIndexChanged);
            // 
            // instanceViewTabPage
            // 
            this.instanceViewTabPage.Controls.Add(this.instanceViewGrid);
            resources.ApplyResources(this.instanceViewTabPage, "instanceViewTabPage");
            this.instanceViewTabPage.Name = "instanceViewTabPage";
            this.instanceViewTabPage.UseVisualStyleBackColor = true;
            // 
            // instanceViewGrid
            // 
            resources.ApplyResources(this.instanceViewGrid, "instanceViewGrid");
            this.instanceViewGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.instanceViewGrid.Name = "instanceViewGrid";
            this.instanceViewGrid.ToolbarVisible = false;
            // 
            // attachmentTabPage
            // 
            this.attachmentTabPage.Controls.Add(this.deleteAttachmentButton);
            this.attachmentTabPage.Controls.Add(this.addAttachmentButton);
            this.attachmentTabPage.Controls.Add(this.downloadButton);
            this.attachmentTabPage.Controls.Add(this.attachmentListView);
            resources.ApplyResources(this.attachmentTabPage, "attachmentTabPage");
            this.attachmentTabPage.Name = "attachmentTabPage";
            this.attachmentTabPage.UseVisualStyleBackColor = true;
            // 
            // deleteAttachmentButton
            // 
            resources.ApplyResources(this.deleteAttachmentButton, "deleteAttachmentButton");
            this.deleteAttachmentButton.Name = "deleteAttachmentButton";
            this.toolTip1.SetToolTip(this.deleteAttachmentButton, resources.GetString("deleteAttachmentButton.ToolTip"));
            this.deleteAttachmentButton.Click += new System.EventHandler(this.deleteAttachmentButton_Click);
            // 
            // addAttachmentButton
            // 
            resources.ApplyResources(this.addAttachmentButton, "addAttachmentButton");
            this.addAttachmentButton.Name = "addAttachmentButton";
            this.toolTip1.SetToolTip(this.addAttachmentButton, resources.GetString("addAttachmentButton.ToolTip"));
            this.addAttachmentButton.Click += new System.EventHandler(this.addAttachmentButton_Click);
            // 
            // downloadButton
            // 
            resources.ApplyResources(this.downloadButton, "downloadButton");
            this.downloadButton.Name = "downloadButton";
            this.toolTip1.SetToolTip(this.downloadButton, resources.GetString("downloadButton.ToolTip"));
            this.downloadButton.Click += new System.EventHandler(this.dowloadButton_Click);
            // 
            // attachmentListView
            // 
            resources.ApplyResources(this.attachmentListView, "attachmentListView");
            this.attachmentListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.attachmentListView.FullRowSelect = true;
            this.attachmentListView.HideSelection = false;
            this.attachmentListView.LargeImageList = this.fileTypeLargeImageList;
            this.attachmentListView.MultiSelect = false;
            this.attachmentListView.Name = "attachmentListView";
            this.attachmentListView.SmallImageList = this.fileTypeSmallImageList;
            this.attachmentListView.UseCompatibleStateImageBehavior = false;
            this.attachmentListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // fileTypeLargeImageList
            // 
            this.fileTypeLargeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.fileTypeLargeImageList, "fileTypeLargeImageList");
            this.fileTypeLargeImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // fileTypeSmallImageList
            // 
            this.fileTypeSmallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.fileTypeSmallImageList, "fileTypeSmallImageList");
            this.fileTypeSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // DataViewer
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.resultTabControl);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.resultPanel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView);
            this.Menu = this.mainMenu1;
            this.Name = "DataViewer";
            this.Activated += new System.EventHandler(this.DataViewer_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DataViewer_Closing);
            this.Deactivate += new System.EventHandler(this.DataViewer_Deactivate);
            this.Load += new System.EventHandler(this.DataViewer_Load);
            this.resultPanel.ResumeLayout(false);
            this.dataGridContextMenuStrip.ResumeLayout(false);
            this.resultTabControl.ResumeLayout(false);
            this.instanceViewTabPage.ResumeLayout(false);
            this.attachmentTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region IDataGridControl ≥…‘±

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
			SaveFileDialog saveFileDialog = new SaveFileDialog();
	 
			saveFileDialog.InitialDirectory = "c:\\" ;
			saveFileDialog.Filter = formatName + "|*." + suffix;
			saveFileDialog.FilterIndex = 2 ;
			saveFileDialog.RestoreDirectory = false ;
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
            return DesignStudio.CreateActiveXControlWebService();
		}

        /// <summary>
        /// Gets the base class name of data instances currently displayed in the datagrid 
        /// </summary>
        public string BaseClassName
        {
            get
            {
                if (this.resultDataControl1.CurrentSlide != null)
                {
                    return this.resultDataControl1.CurrentSlide.DataView.BaseClass.Name;
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
				if (this.resultDataControl1.CurrentSlide != null)
				{
                    return this.resultDataControl1.CurrentSlide.DataView.BaseClass.Name;
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
				return this.resultDataControl1.TheDataGrid;
			}
		}

		/// <summary>
		/// Gets the DataView that is in the same order as what displayed on the datagrid
		/// </summary>
		public System.Data.DataView DataView
		{
			get
			{
                if (this.resultDataControl1.CurrentSlide != null &&
                    this.resultDataControl1.CurrentSlide.DataSet != null &&
                    this.resultDataControl1.CurrentSlide.DataSet.Tables[TableName] != null)
				{
                    return this.resultDataControl1.CurrentSlide.DataSet.Tables[TableName].DefaultView;
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
                if (this.resultDataControl1.CurrentSlide != null)
				{
					ColumnInfo columnInfo;
					columnInfos = new ColumnInfoCollection();

					DataGridTableStyle tableStyle = this.resultDataControl1.TheDataGrid.TableStyles[tableName];
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

		#endregion

		#region Data Viewer controller code

		/// <summary>
		/// Display the data of the seletced tree node
		/// </summary>
		internal void ShowSelectedTreeNode()
		{
			MetaDataTreeNode treeNode = (MetaDataTreeNode) this.treeView.SelectedNode;
	
			// set the menu item states
			SetMenuItemStates(treeNode);

			if (treeNode.MetaDataElement != null)
			{
				if (treeNode.MetaDataElement is ClassElement)
				{
					ClassElement clsElement = (ClassElement) treeNode.MetaDataElement;
					if (clsElement.TableName != null && clsElement.TableName.Length > 0)
					{
						this.resultDataControl1.Slides.Clear();
                        this.resultDataControl1.CurrentSlide = new DataViewSlide(_menuItemStates);
                        this.resultDataControl1.Slides.Add(this.resultDataControl1.CurrentSlide);
						if (this._isDefaultDataView)
						{
                            this.resultDataControl1.CurrentSlide.DataView = _metaData.GetDefaultDataView(clsElement.Name);
						}
						else
						{
                            this.resultDataControl1.CurrentSlide.DataView = _metaData.GetDetailedDataView(clsElement.Name);
						}

                        // the class element is good for inline edit
                        this.resultDataControl1.CurrentSlide.GoodForInlineEdit = true;

						// clear the search expression in the default data view, so that
						// users can build their own search expression using search
						// expression builder from the scratch
                        this.resultDataControl1.CurrentSlide.DataView.ClearSearchExpression();
                        DataViewValidateResult validateResult = this.resultDataControl1.CurrentSlide.DataView.ValidateDataView();
						if (validateResult.HasError)
						{
							if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.InvalidDataView"),
								"Error Dialog", MessageBoxButtons.YesNo,
								MessageBoxIcon.Error) == DialogResult.Yes)
							{
								DataValidateErrorDialog dialog = new DataValidateErrorDialog();
								dialog.Entries = validateResult.Errors;
								dialog.ShowDialog();
							}
						}
						else
						{
							// set button state for building full-text index
							SetBuildFullTextIndexButtonState(clsElement.Name);

							ExecuteQuery();
						}
					}
					else
					{
						// the class has not been saved to the databae
                        MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataView.NonExistClass"),
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
				}
				else if (treeNode.MetaDataElement is DataViewModel)
				{
					// make sure the data view is in sync with schema model
					DataViewValidateResult validateResult = ((DataViewModel) treeNode.MetaDataElement).ValidateDataView();
					if (validateResult.HasError)
					{
                        if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.InvalidDataView"),
							"Error Dialog", MessageBoxButtons.YesNo,
							MessageBoxIcon.Error) == DialogResult.Yes)
						{
							DataValidateErrorDialog dialog = new DataValidateErrorDialog();
							dialog.Entries = validateResult.Errors;
							dialog.ShowDialog();
						}
					}
					else
					{
                        this.resultDataControl1.Slides.Clear();
                        this.resultDataControl1.CurrentSlide = new DataViewSlide(_menuItemStates);
                        this.resultDataControl1.Slides.Add(this.resultDataControl1.CurrentSlide);
                        this.resultDataControl1.CurrentSlide.DataView = (DataViewModel)treeNode.MetaDataElement;
						ExecuteQuery();
					}
				}
				else if (treeNode.MetaDataElement is ITaxonomy)
				{
					DataViewModel dataView = null;
					if (this._isDefaultDataView)
					{
						dataView = ((ITaxonomy) treeNode.MetaDataElement).GetDataView(null);
					}
					else
					{
						dataView = ((ITaxonomy) treeNode.MetaDataElement).GetDataView("Detailed");
					}

					if (dataView != null)
					{

                        TaxonNode taxon = treeNode.MetaDataElement as TaxonNode;
                        if (taxon != null &&
                            taxon.SearchFilter == null &&
                            string.IsNullOrEmpty(taxon.DataViewName))
                        {
                            // the dataview's search expression contains no search parameter, therefore,
                            // clear the search expression in the default class data view, so that
                            // users can build their own search expression using search
                            // expression builder from the scratch
                            dataView.ClearSearchExpression();
                        }

						// make sure the data view is in sync with schema model
						DataViewValidateResult validateResult = dataView.ValidateDataView();
						if (validateResult.HasError)
						{
                            if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.InvalidTaxonomy"),
								"Error Dialog", MessageBoxButtons.YesNo,
								MessageBoxIcon.Error) == DialogResult.Yes)
							{
								DataValidateErrorDialog dialog = new DataValidateErrorDialog();
								dialog.Entries = validateResult.Errors;
								dialog.ShowDialog();
							}
						}
						else
						{
                            this.resultDataControl1.Slides.Clear();
                            this.resultDataControl1.CurrentSlide = new DataViewSlide(_menuItemStates);
                            this.resultDataControl1.Slides.Add(this.resultDataControl1.CurrentSlide);
                            this.resultDataControl1.CurrentSlide.DataView = dataView;
							ExecuteQuery();
						}
					}
				}
			}
		}

		internal void ShowSearchDialog(object sender, EventArgs e)
		{
			CreateSearchExprDialog dialog = new CreateSearchExprDialog();
            dialog.DataView = this.resultDataControl1.CurrentSlide.DataView;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				ExecuteQuery();
			}
		}

		/// <summary>
		/// Show a dialog for creating or modifying a data view
		/// </summary>
		/// <param name="dataView">If not null, then it is a data view to be modified.</param>
		internal void ShowDataViewDialog(DataViewModel dataView)
		{
			CreateDataViewDialog dialog = new CreateDataViewDialog();
			dialog.MetaData = this._metaData;
            bool isCreate = true;

            if (dataView != null)
            {
                // Modify the data view model, so we need to clone it
                isCreate = false;
                // make a clone
                dialog.DataView = dataView.Clone();
                dialog.DataView.SchemaModel = this._metaData.SchemaModel;
                dialog.DataView.SchemaInfo = this._metaData.SchemaInfo;
            }
            else
            {
                dialog.DataView = null;
            }


			if (dialog.ShowDialog() == DialogResult.OK)
			{
                if (isCreate)
				{
					// create a new data view
					_metaData.DataViews.Add(dialog.DataView);

					// add a new tree node under DataViews tree node for the newly added
					// data view
					MetaDataTreeNode dataViewFolderNode = (MetaDataTreeNode) this.treeView.SelectedNode;
					MetaDataTreeNode dataViewNode = _treeBuilder.CreateTreeNode(dialog.DataView);
					dataViewFolderNode.Nodes.Add(dataViewNode);
				}
				else
				{
					// change the caption of current selected data view node
					MetaDataTreeNode selectedNode = (MetaDataTreeNode) this.treeView.SelectedNode;
					selectedNode.Text = dialog.DataView.Caption;

                    // replace the data view with modified one
                    this._metaData.DataViews.Add(dialog.DataView);
                    this._metaData.DataViews.Remove(dataView);
                    selectedNode.MetaDataElement = dialog.DataView;
				}
			}
		}

		internal void DeleteDataView(object sender, EventArgs e)
		{
			MetaDataTreeNode selectedTreeNode = (MetaDataTreeNode) this.treeView.SelectedNode;

			// remove the data view from the meta data
			_metaData.DataViews.Remove((DataViewModel) selectedTreeNode.MetaDataElement);

			// Remove the corresponding tree node
			TreeNode parent = selectedTreeNode.Parent;

			parent.Nodes.Remove(selectedTreeNode);
		}

		internal void ShowModifyDataViewDialog()
		{
			MetaDataTreeNode selectedTreeNode = (MetaDataTreeNode) this.treeView.SelectedNode;
			DataViewModel dataView = selectedTreeNode.MetaDataElement as DataViewModel;

			ShowDataViewDialog(dataView);
		}

		/// <summary>
		/// Initialize the menu item states
		/// </summary>
		private void InitializeMenuItemStates()
		{
			_menuItemStates.SetState(MenuItemID.EditAdd, false);
			_menuItemStates.SetState(MenuItemID.EditCopy, false);
			_menuItemStates.SetState(MenuItemID.EditCut, false);
			_menuItemStates.SetState(MenuItemID.EditPaste, false);
			_menuItemStates.SetState(MenuItemID.FileExport, false);
			_menuItemStates.SetState(MenuItemID.EditSearch, false);
			_menuItemStates.SetState(MenuItemID.EditShowAllInstances, false);
			_menuItemStates.SetState(MenuItemID.EditNewInstance, false);
			_menuItemStates.SetState(MenuItemID.EditSaveInstance, false);
			_menuItemStates.SetState(MenuItemID.EditSaveInstanceAs, false);
			_menuItemStates.SetState(MenuItemID.EditDeleteInstance, false);
			_menuItemStates.SetState(MenuItemID.EditDeleteAllInstances, false);
			_menuItemStates.SetState(MenuItemID.EditAdd, false);
			_menuItemStates.SetState(MenuItemID.EditModify, false);
			_menuItemStates.SetState(MenuItemID.EditDelete, false);
			_menuItemStates.SetState(MenuItemID.ViewDefault, false);
			_menuItemStates.SetState(MenuItemID.ViewDetailed, false);
			_menuItemStates.SetState(MenuItemID.ViewChart, false);
            _menuItemStates.SetState(MenuItemID.ViewPivot, false);
            _menuItemStates.SetState(MenuItemID.ViewLogging, false);
			_menuItemStates.SetState(MenuItemID.ViewNextRow, false);
			_menuItemStates.SetState(MenuItemID.ViewPrevRow, false);
			_menuItemStates.SetState(MenuItemID.ViewNextPage, false);
			_menuItemStates.SetState(MenuItemID.ViewPrevPage, false);
			_menuItemStates.SetState(MenuItemID.ViewRowCount, false);
			_menuItemStates.SetState(MenuItemID.FileSaveAsFile, false);
			_menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
			_menuItemStates.SetState(MenuItemID.FileSaveFile, false);
			_menuItemStates.SetState(MenuItemID.ToolValidate, false);
			_menuItemStates.SetState(MenuItemID.ToolBuildFullTextIndex, false);

            if (_metaData != null && _metaData.SchemaInfo.ID != null)
            {
                // meta-data model is loaded from database
                _menuItemStates.SetState(MenuItemID.ViewRefresh, true);
            }
            else
            {
                _menuItemStates.SetState(MenuItemID.ViewRefresh, false);
            }
		}

		/// <summary>
		/// Set the menu item states based on the selected tree node
		/// </summary>
		/// <param name="selectedNode"></param>
		private void SetMenuItemStates(MetaDataTreeNode selectedNode)
		{
			if (selectedNode.Nodes.Count > 0)
			{
				if (selectedNode.IsExpanded)
				{
					this._menuItemStates.SetState(MenuItemID.CollapseNode, true);
					this._menuItemStates.SetState(MenuItemID.ExpandNode, false);
				}
				else
				{
					this._menuItemStates.SetState(MenuItemID.CollapseNode, false);
					this._menuItemStates.SetState(MenuItemID.ExpandNode, true);
				}
			}

            if (selectedNode.Type == Newtera.WinClientCommon.NodeType.ClassesFolder ||
                selectedNode.Type == Newtera.WinClientCommon.NodeType.SchemaInfoNode)
			{
				SetBuildFullTextIndexButtonState(null);
			}
			else
			{
				_menuItemStates.SetState(MenuItemID.ToolBuildFullTextIndex, false);
			}

            /* disable adding dataview in the dataviewer
            if (selectedNode.Type == Newtera.WinClientCommon.NodeType.DataViewsFolder)
			{
				_menuItemStates.SetState(MenuItemID.EditAdd, true);
			}
			else
			{
				_menuItemStates.SetState(MenuItemID.EditAdd, false);
			}
             */

            /* disable the modifying dataviews in the dataviewer
            if (selectedNode.Type == Newtera.WinClientCommon.NodeType.DataViewNode)
			{
				_menuItemStates.SetState(MenuItemID.EditModify, true);
				_menuItemStates.SetState(MenuItemID.EditDelete, true);
			}
			else
			{
				_menuItemStates.SetState(MenuItemID.EditModify, false);
				_menuItemStates.SetState(MenuItemID.EditDelete, false);
			}
            */

            if (selectedNode.Type == Newtera.WinClientCommon.NodeType.ClassNode ||
                selectedNode.Type == Newtera.WinClientCommon.NodeType.DataViewNode)
			{
				_menuItemStates.SetState(MenuItemID.EditSearch, true);
				_menuItemStates.SetState(MenuItemID.EditShowAllInstances, true);
				_menuItemStates.SetState(MenuItemID.ViewDefault, true);
				_menuItemStates.SetState(MenuItemID.ViewDetailed, true);
				_menuItemStates.SetState(MenuItemID.ViewChart, true);
                _menuItemStates.SetState(MenuItemID.ViewPivot, false);
				_menuItemStates.SetState(MenuItemID.ViewRowCount, true);

                if (IsDBAUser)
                {
                    // only dba user can view logging info
                    _menuItemStates.SetState(MenuItemID.ViewLogging, true);
                }
                else
                {
                    _menuItemStates.SetState(MenuItemID.ViewLogging, false);
                }
			}

			if (selectedNode.MetaDataElement is DataViewModel)
			{
				DataViewModel dataView = (DataViewModel) selectedNode.MetaDataElement;

				if (Newtera.Common.MetaData.XaclModel.PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
					(ClassElement) dataView.BaseClass.GetSchemaModelElement(), XaclActionType.Download))
				{
					this._menuItemStates.SetState(MenuItemID.FileExport, true);
				}
			}
			else if (selectedNode.MetaDataElement is ITaxonomy)
			{
				DataViewModel dataView = ((ITaxonomy) selectedNode.MetaDataElement).GetDataView(null);
				
				if (dataView != null &&
					Newtera.Common.MetaData.XaclModel.PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
					(ClassElement) dataView.BaseClass.GetSchemaModelElement(), XaclActionType.Download))
				{
					this._menuItemStates.SetState(MenuItemID.FileExport, true);
				}
			}
		}

		/// <summary>
		/// Enable or disable instance edit buttons
		/// </summary>
		/// <param name="state">true to enable, false to disable</param>
		private void EnableInstanceEditButtons(bool state)
		{
			this._menuItemStates.SetState(MenuItemID.EditNewInstance, state);
			this._menuItemStates.SetState(MenuItemID.EditSaveInstance, state);
			this._menuItemStates.SetState(MenuItemID.EditSaveInstanceAs, state);
			this._menuItemStates.SetState(MenuItemID.EditDeleteInstance, state);
		}

		/// <summary>
		/// Display the query result data set in a data grid
		/// </summary>
		/// <param name="dataSet">The query result in DataSet</param>
		private void ShowQueryResult(DataSet dataSet)
		{
            this.resultDataControl1.ShowQueryResult(dataSet);

            if (this.resultDataControl1.CurrentSlide != null &&
                this.resultDataControl1.CurrentSlide.IsEmptyResult)
            {
                this.addAttachmentButton.Enabled = false;
            }
		}

		private void ExecuteQuery()
		{
			if (this.resultDataControl1.CurrentSlide.DataView != null)
			{
                string query = this.resultDataControl1.CurrentSlide.DataView.SearchQueryWithPKValues;

				//MessageBox.Show(query, "Confirm Dialog", MessageBoxButtons.OK, MessageBoxIcon.Question);

				_isRequestComplete = false;

                // invoke the web service
                XmlNode xmlNode = _dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
					query);

                ((DesignStudio)this.MdiParent).ShowReadyStatus();

                DataSet ds = new DataSet();

                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                ShowQueryResult(ds);
			}
		}

		/// <summary>
		/// Get the relationship attribute of a given name
		/// </summary>
		/// <param name="classAlias">The unique class alias</param>
		/// <param name="relationshipName">The relationship name</param>
		/// <returns>The relationship attribute</returns>
		private RelationshipAttributeElement GetRelationshipAttribute(string classAlias, string relationshipName)
		{
			// first find the class in data view using class alias
            DataClass dataClass = this.resultDataControl1.CurrentSlide.DataView.FindClass(classAlias);
			// Then get the corresponding class element from schema model
            ClassElement classElement = this.resultDataControl1.CurrentSlide.DataView.SchemaModel.FindClass(dataClass.ClassName);

			// lastly, find the relationship attribute element from the class element
			RelationshipAttributeElement relationshipAttribute = classElement.FindInheritedRelationshipAttribute(relationshipName);

			return relationshipAttribute;
		}

		/// <summary>
		/// Display the additional view about the selected row in the main data grid
		/// depending on which sub view is currently active.
		/// </summary>
		private void ShowSubView()
		{
            if (this.resultDataControl1.CurrentSlide != null && !this.resultDataControl1.CurrentSlide.IsEmptyResult)
			{
				switch (_currentSubView)
				{
					case ResultViewType.InstanceView:
						if (IsNewInstanceNeeded())
						{
							ShowInstanceView();
						}
						else
						{
							this.instanceViewGrid.Refresh();
						}

						break;
					case ResultViewType.AttachmentView:
						if (IsUpdateAttachmentViewNeeded())
						{
							ShowAttachmentView();
						}

						break;
				}
			}
		}

		/// <summary>
		/// Gets the value indicating if a new instance data is needed
		/// </summary>
		/// <returns>True if it is needed, false otherwise</returns>
		private bool IsNewInstanceNeeded()
		{
			bool isNeeded = false;

			if (this.resultDataControl1.CurrentSlide.InstanceDataView == null ||
                this.resultDataControl1.CurrentSlide.SelectedRowObjId != this.resultDataControl1.CurrentSlide.InstanceDataView.CurrentObjId)
			{
				isNeeded = true;
			}
		
			return isNeeded;
		}

		/// <summary>
		/// Gets the value indicating whether the attachment view needs to be updated.
		/// </summary>
		/// <returns>True if it is needed, false otherwise</returns>
		private bool IsUpdateAttachmentViewNeeded()
		{
			bool isNeeded = false;

            if (this.resultDataControl1.CurrentSlide.AttachmentViewObjId == null ||
                this.resultDataControl1.CurrentSlide.SelectedRowObjId != this.resultDataControl1.CurrentSlide.AttachmentViewObjId)
			{
				isNeeded = true;
			}
			else if (_isAttachmentChanged)
			{
				isNeeded = true;
				_isAttachmentChanged = false;
			}

			return isNeeded;
		}

		/// <summary>
		/// Get data of an instance of the selected row in 
		/// and display it in the instance view
		/// </summary>
		private void ShowInstanceView()
		{
			try
			{
                String classType = this.resultDataControl1.CurrentSlide.SelectedRowClassType;
				if (classType != null)
				{
                    if (this.resultDataControl1.CurrentSlide.InstanceDataView == null ||
                        this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.ClassName != classType)
					{
						DataViewModel instanceDataView = _metaData.GetDetailedDataView(classType);
                        this.resultDataControl1.CurrentSlide.InstanceDataView = instanceDataView;
					}

                    string query = this.resultDataControl1.CurrentSlide.InstanceDataView.GetInstanceQuery(this.resultDataControl1.CurrentSlide.SelectedRowObjId);

                    // Show the status
                    ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.GettingData"));

					// invoke the web service synchronously
					XmlNode xmlNode = _dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
						query);

                    DataSet ds = new DataSet();

                    XmlReader xmlReader = new XmlNodeReader(xmlNode);
                    ds.ReadXml(xmlReader);

                    ShowInstanceData(ds);
				}
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.instanceViewGrid.SelectedObject = null;
            }		
		}

		private delegate void ShowInstanceDataDelegate(DataSet dataSet);

		/// <summary>
		/// Display the query result data set in a data grid
		/// </summary>
		/// <param name="dataSet">The query result in DataSet</param>
		private void ShowInstanceData(DataSet dataSet)
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue
   
				// Create an row view for the row data
                if (this.resultDataControl1.CurrentSlide.InstanceDataView != null)
				{
                    if (!DataSetHelper.IsEmptyDataSet(dataSet, this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.ClassName))
                    {
                        InstanceView instanceView = new InstanceView(this.resultDataControl1.CurrentSlide.InstanceDataView, dataSet);

                        this.resultDataControl1.CurrentSlide.InstanceView = instanceView;

                        this.instanceViewGrid.SelectedObject = instanceView;

                        // set the enabling states of instance editing buttons accordingly
                        this.resultDataControl1.SetInstanceEditButtonStates(instanceView.InstanceData);
                    }
                    else
                    {
                        // didn'get the instance, maybe due to lack of permission
                        this.instanceViewGrid.SelectedObject = null;
                        //this.resultDataControl1.EnableInstanceEditButtons(false);
                    }
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowInstanceDataDelegate showInstanceData = new ShowInstanceDataDelegate(ShowInstanceData);

				this.BeginInvoke(showInstanceData, new object[] {dataSet});
			}
		}

		/// <summary>
		/// Display the attachement data of an instance of the selected row in 
		/// the attachment view
		/// </summary>
		private void ShowAttachmentView()
		{
			// the attachment'e permission follows that of instances.
            SetAttachmentEditButtonStates(this.resultDataControl1.CurrentSlide.RowView.InstanceData);

            int anum = this.resultDataControl1.CurrentSlide.SelectedRowANUM;
            string objId = this.resultDataControl1.CurrentSlide.SelectedRowObjId;
            this.resultDataControl1.CurrentSlide.AttachmentViewObjId = objId;
			if (anum > 0)
			{
                // Show the status
                ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.GettingAttachment"));

                string connectionString = ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo);
                int count = _attachmentService.GetAttachmentInfosCount(connectionString, objId);
                AttachmentInfoCollection allAttachmentInfos = new AttachmentInfoCollection();
                int startRow = 0;
                int pageSize = 50;
                while (startRow < count)
                {
                    // invoke the web service synchronously
                    string xml = _attachmentService.GetAttachmentInfos(connectionString, objId, startRow, pageSize);

                    // convert xml to AttachmentInfoCollection
                    StringReader reader = new StringReader(xml);
                    AttachmentInfoCollection infos = new AttachmentInfoCollection();
                    infos.Read(reader);

                    foreach (AttachmentInfo attachmentInfo in infos)
                    {
                        allAttachmentInfos.Add(attachmentInfo);
                    }

                    startRow += pageSize;
                }

                ShowAttachmentInfos(allAttachmentInfos);
			}
			else
			{
				this.attachmentListView.Items.Clear();
				this.downloadButton.Enabled = false;
				this.deleteAttachmentButton.Enabled = false;
			}
		}

		private delegate void ShowAttachmentInfosDelegate(AttachmentInfoCollection infos);

		/// <summary>
		/// Display the attachment infos
		/// </summary>
		/// <param name="infos">A collection of attachment infos</param>
		private void ShowAttachmentInfos(AttachmentInfoCollection infos)
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue

				this.attachmentListView.Items.Clear();

				AttachmentListViewItem item;
				
				FileTypeInfo fileTypeInfo;
				int index = 0;
				foreach (AttachmentInfo info in infos)
				{
					fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Type);

					item = new AttachmentListViewItem(info);
					item.ImageIndex = fileTypeInfo.ImageIndex;
					item.SubItems.Add(GetSizeInKB(info.Size));
					item.SubItems.Add(fileTypeInfo.Description);
					item.SubItems.Add(info.CreateTime.ToString());

					if (index == 0)
					{
						// select the first item
						item.Selected = true;
					}

					this.attachmentListView.Items.Add(item);
					index++;
				}

				// disable download and delete buttons if the attachment list is empty
				if (index == 0)
				{
					this.downloadButton.Enabled = false;
					this.deleteAttachmentButton.Enabled = false;
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowAttachmentInfosDelegate showAttachmentInfo = new ShowAttachmentInfosDelegate(ShowAttachmentInfos);

				this.BeginInvoke(showAttachmentInfo, new object[] {infos});
			}
		}

		/// <summary>
		/// Get the size represented using KB as unit
		/// </summary>
		/// <param name="size">size in bytes</param>
		/// <returns>A string representing size in KB</returns>
		private string GetSizeInKB(long size)
		{
			string sizeInKB;

			if (size == 0)
			{
				sizeInKB = "0 KB";
			}
			else
			{
				sizeInKB = (size/1024 + 1) + " KB";
			}

			return sizeInKB;
		}

		/// <summary>
		/// Delete an attachment
		/// </summary>
		private void DeleteAttachment(AttachmentInfo info)
		{
			try
			{
                // Show the status
                ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.DeletingAttachment"));

				// invoke the web service asynchronously
				_attachmentService.DeleteAttachment(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
					info.ItemId, info.Name);

                RefreshAttachmentListView();
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
			finally
			{
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
			}
		}

		private delegate void RefreshAttachmentListViewDelegate();

		/// <summary>
		/// Refresh the attachment list view
		/// </summary>
		private void RefreshAttachmentListView()
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue

				// Decrease the attachment number by one to the cached result
				// so that we don't have to fetch the result from server
                this.resultDataControl1.CurrentSlide.RowView.InstanceData.DecreamentANUM();

				_isAttachmentChanged = true;

				this.resultDataControl1.ShowDataSlide(); // refresh the result window
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				RefreshAttachmentListViewDelegate refreshAttachmentListView = new RefreshAttachmentListViewDelegate(RefreshAttachmentListView);

				this.BeginInvoke(refreshAttachmentListView);
			}
		}

		/// <summary>
		/// Get an attachment and save it to a file
		/// </summary>
		private void GetAttachment(AttachmentInfo info)
		{
			_isRequestComplete = false;

			// invoke the web service asynchronously
			_attachmentService.GetAttachment(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
				info.ItemId, info.Name);
		}

		/// <summary>
		/// Download a graph file
		/// </summary>
		private void DownloadGraphFile(string formatName)
		{
		}

		/// <summary>
		/// Show the dialog of creating a new instance
		/// </summary>
		internal void ShowNewInstanceDialog()
		{
			// first try to get class type of the selected row as the type of
			// a new instance, if there isn't a selected row, fall back to
			// the base class of the dataview, but make sure the base class is
			// a bottom class.
            String classType = this.resultDataControl1.CurrentSlide.SelectedRowClassType;

			if (classType == null)
			{
                ClassElement classElement = this.resultDataControl1.CurrentSlide.DataView.SchemaModel.FindClass(this.resultDataControl1.CurrentSlide.DataView.BaseClass.ClassName);
				if (classElement.IsLeaf)
				{
					classType = classElement.Name;
				}
				else
				{
                    MessageBox.Show(this.resultDataControl1.CurrentSlide.DataView.BaseClass.Caption + Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.NotLeafClass"),
						"Message Dialog", MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}

			if (classType != null)
			{
                if (this.resultDataControl1.CurrentSlide.InstanceDataView == null ||
                    this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.ClassName != classType)
				{
					DataViewModel instanceDataView = _metaData.GetDetailedDataView(classType);
                    this.resultDataControl1.CurrentSlide.InstanceDataView = instanceDataView;
				}

                NewInstanceDialog newInstanceDialog = new NewInstanceDialog(_metaData);

                newInstanceDialog.InstanceView = new InstanceView(this.resultDataControl1.CurrentSlide.InstanceDataView);

				if (newInstanceDialog.ShowDialog() == DialogResult.OK)
				{
					AddNewInstance(newInstanceDialog.InstanceView.DataView);
				}
			}
		}

		/// <summary>
		/// Generate and execute a query of adding a new instance to database
		/// </summary>
		/// <param name="instanceDataView">The instance data view</param>
		private void AddNewInstance(DataViewModel instanceDataView)
		{
			try
			{
				string query = instanceDataView.InsertQuery;

				//MessageBox.Show(query, "Confirm Dialog", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                // Show the status
                ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.AddingInstance"));

				// invoke the web service asynchronously
				string xml = _dataService.ExecuteUpdateQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
					query, true);

                ShowQueryExecutionStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.InstanceAdded"));
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
			finally
			{
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
			}
		}

		private delegate void ShowQueryExecutionStatusDelegate(string msg);

		/// <summary>
		/// Display the query execution status
		/// </summary>
		/// <param name="msg">The message to display.</param>
		private void ShowQueryExecutionStatus(string msg)
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue
				if (MessageBox.Show(msg,
					"Message Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Information) == DialogResult.OK)
				{
					ExecuteQuery();
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowQueryExecutionStatusDelegate showStatus = new ShowQueryExecutionStatusDelegate(ShowQueryExecutionStatus);

				this.BeginInvoke(showStatus, new object[] {msg});
			}
		}

		/// <summary>
		/// Save the instance currently being edited in an instance view
		/// </summary>
		internal void SaveInstance()
		{
            DataValidateErrorDialog dialog;

			try
			{

				// make sure there are data changes in the selected instance
                if (this.resultDataControl1.CurrentSlide.InstanceView != null &&
                    this.resultDataControl1.CurrentSlide.InstanceView.IsDataChanged)
				{
                    // validate the instance data
                    DataViewValidateResult validateResult = this.resultDataControl1.CurrentSlide.InstanceDataView.ValidateData();
                    if (validateResult.HasError)
                    {
                        dialog = new DataValidateErrorDialog();
                        dialog.Entries = validateResult.Errors;
                        dialog.ShowDialog();
                        return;
                    }
                    else if (validateResult.HasDoubt)
                    {
                        // determine if the doubts are errors or not
                        ValidateDoubts(validateResult);

                        // ValidateDoubts method call may turn doubts into an errors
                        if (validateResult.HasError)
                        {
                            dialog = new DataValidateErrorDialog();
                            dialog.Entries = validateResult.Errors;
                            dialog.ShowDialog();
                            return;
                        }
                    }
                    
                    // finally verify the instance data using validating rules if exist.
                    ClassElement classElement = _metaData.SchemaModel.FindClass(this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.Name);
                    RuleCollection validatingRules = _metaData.RuleManager.GetPrioritizedRules(classElement).Rules;
                    if (validatingRules != null && validatingRules.Count > 0)
                    {
                        ValidateUsingRules(this.resultDataControl1.CurrentSlide.InstanceDataView, validateResult, validatingRules, classElement);
                        if (validateResult.HasError)
                        {
                            dialog = new DataValidateErrorDialog();
                            dialog.Entries = validateResult.Errors;
                            dialog.ShowDialog();
                            return;
                        }
                    }

                    string query = this.resultDataControl1.CurrentSlide.InstanceDataView.UpdateQuery;

					///MessageBox.Show(query, "Confirm Dialog", MessageBoxButtons.OKCancel,	MessageBoxIcon.Question);

                    // Show the status
                    ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.SavingInstance"));

					// invoke the web service asynchronously
					XmlNode xmlNode  = _dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
						query);

                    ShowQueryExecutionStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.InstanceUpdated"));
				}
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
			finally
			{
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
			}
		}

        /// <summary>
        /// validate the doubts raised by validating data process
        /// </summary>
        /// <param name="validateResult"></param>
        private void ValidateDoubts(DataViewValidateResult validateResult)
        {
            foreach (DataValidateResultEntry doubt in validateResult.Doubts)
            {
                if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.PrimaryKey)
                {
                    if (this.resultDataControl1.CurrentSlide.InstanceDataView.ResultAttributes[doubt.DataViewElement.Name].IsValueChanged)
                    {
                        if (IsPKValueExists())
                        {
                            // the primary key value exists, change the doubt into error
                            doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                            validateResult.AddError(doubt);
                        }
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValue)
                {
                    if (this.resultDataControl1.CurrentSlide.InstanceDataView.ResultAttributes[doubt.DataViewElement.Name].IsValueChanged)
                    {
                        if (!IsValueUnique())
                        {
                            // the value isn't unique, change the doubt into error
                            doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                            validateResult.AddError(doubt);
                        }
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueReference)
                {
                    if (this.resultDataControl1.CurrentSlide.InstanceDataView.ResultAttributes[doubt.DataViewElement.Name].IsValueChanged)
                    {
                        if (!IsReferenceUnique(doubt.DataViewElement))
                        {
                            // the reference isn't unique, change the doubt into error
                            doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                            validateResult.AddError(doubt);
                        }
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValues)
                {
                    if (this.resultDataControl1.CurrentSlide.InstanceDataView.IsUniqueKeyValuesChanged(doubt.ClassName))
                    {
                        if (!IsCombinedValuesUnique(doubt.ClassName))
                        {
                            // the combination of values isn't unique, change the doubt into error
                            doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                            validateResult.AddError(doubt);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validate the instance data using the validating rules
        /// </summary>
        /// <param name="dataView">The data view that holds an instance data to be validated against.</param>
        /// <param name="validateResult">The validating results.</param>
        /// <param name="rules">The validating rules.</param>
        /// <param name="classElement">The rule owner class.</param>
        private void ValidateUsingRules(DataViewModel dataView, DataViewValidateResult validateResult, RuleCollection rules, ClassElement classElement)
        {
            DataValidateResultEntry entry = null;
            string message;
            string query;

            foreach (RuleDef ruleDef in rules)
            {
                // generating a validating query based on the rule definition
                query = dataView.GetRuleValidatingQuery(ruleDef);

                // invoke the web service synchronously
                message = _dataService.ExecuteValidatingQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), query);

                RuleValidateResult result = new RuleValidateResult(message);
                if (result.HasError)
                {
                    entry = new DataValidateResultEntry(result.Message, classElement.Caption, null);
                    validateResult.AddError(entry);
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the primary key value are already used by
        /// another instance
        /// </summary>
        /// <returns>True if it's been used, false otherwise</returns>
        private bool IsPKValueExists()
        {
            bool status = false;

            try
            {
                CMDataServiceStub dataService = new CMDataServiceStub();

                string query = this.resultDataControl1.CurrentSlide.InstanceDataView.GetInstanceByPKQuery();
                if (query != null)
                {

                    // invoke the web service synchronously
                    XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        query);

                    DataSet ds = new DataSet();

                    XmlReader xmlReader = new XmlNodeReader(xmlNode);
                    ds.ReadXml(xmlReader);

                    // if the result isn't empty, the instance with the same primary key value exists
                    if (!DataSetHelper.IsEmptyDataSet(ds, this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.ClassName))
                    {
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the indivated value are unique among the
        /// same class.
        /// </summary>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsValueUnique()
        {
            bool status = true;

            // to be implemented

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether a combination of values is unique among the
        /// same class.
        /// </summary>
        /// <param name="className">The name of the class uniquely constrainted.</param>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsCombinedValuesUnique(string className)
        {
            bool status = true;

            CMDataServiceStub dataService = new CMDataServiceStub();

            string query = this.resultDataControl1.CurrentSlide.InstanceDataView.GetInstanceByUniqueKeysQuery(className);
            if (query != null)
            {

                // invoke the web service synchronously
                XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                    query);

                DataSet ds = new DataSet();

                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                // if the result isn't empty, the instance with the same primary key value exists
                if (!DataSetHelper.IsEmptyDataSet(ds, this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.ClassName))
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether a reference to an instance is unique among the
        /// same class.
        /// </summary>
        /// <param name="element">IDataViewElement representing a relationship element</param>
        /// <returns>True if the reference is unque, false otherwise</returns>
        private bool IsReferenceUnique(IDataViewElement element)
        {
            bool status = true;

            DataRelationshipAttribute relationshipAttribute = element as DataRelationshipAttribute;

            if (relationshipAttribute != null && relationshipAttribute.HasValue)
            {
                try
                {
                    CMDataServiceStub dataService = new CMDataServiceStub();

                    string query = this.resultDataControl1.CurrentSlide.InstanceDataView.GetInstancesQuery(element);
                    if (query != null)
                    {

                        // invoke the web service synchronously
                        XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                            query);

                        DataSet ds = new DataSet();

                        XmlReader xmlReader = new XmlNodeReader(xmlNode);
                        ds.ReadXml(xmlReader);

                        // if the result isn't empty, there are instances that have reference to the same instance
                        if (!DataSetHelper.IsEmptyDataSet(ds, this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.ClassName))
                        {
                            status = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Server Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            return status;
        }

		/// <summary>
		/// Display a new instance dialog filled with values of the selected instance
		/// except for the attributes that have uniqueness constraint.
		/// </summary>
		internal void ShowNewInstanceAsDialog()
		{
			// make sure that there is a selected instance
            String classType = this.resultDataControl1.CurrentSlide.SelectedRowClassType;

			if (classType != null)
			{
                if (this.resultDataControl1.CurrentSlide.InstanceDataView == null ||
                    this.resultDataControl1.CurrentSlide.InstanceDataView.BaseClass.ClassName != classType)
				{
					DataViewModel instanceDataView = _metaData.GetDetailedDataView(classType);
                    this.resultDataControl1.CurrentSlide.InstanceDataView = instanceDataView;
				}

                NewInstanceDialog newInstanceDialog = new NewInstanceDialog(_metaData);

				// Create an instance view of the currently selected instance in that values of
				// those attributes that have to be unique among instances in a class,
				// such as primary keys and attributes with uniqueness, are empty.
                newInstanceDialog.InstanceView = new InstanceView(this.resultDataControl1.CurrentSlide.InstanceDataView,
                    this.resultDataControl1.CurrentSlide.InstanceView.DataSet, false);

				if (newInstanceDialog.ShowDialog() == DialogResult.OK)
				{
					AddNewInstance(newInstanceDialog.InstanceView.DataView);
				}
			}
			else
			{
                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.SelectInstance"));
			}
		}

		/// <summary>
		/// Delete the selected instances.
		/// </summary>
		internal void DeleteInstances()
		{
			try
			{
                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.DeleteInstance"),
					"Confirm Dialog", MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) == DialogResult.Yes)
				{
					// count number of selected rows
					int selectedRowCount = 0;
                    for (int row = 0; row < this.resultDataControl1.CurrentSlide.RowCount; row++)
					{
                        if (this.resultDataControl1.IsRowSelected(row))
						{
							selectedRowCount++;
						}
					}

					// create a delete query for each selected row
					if (selectedRowCount > 0)
					{
						string[] queries = new string[selectedRowCount];
						int index = 0;
						string classType;
						DataViewModel instanceDataView = null;
                        for (int row = 0; row < this.resultDataControl1.CurrentSlide.RowCount; row++)
						{
                            if (this.resultDataControl1.IsRowSelected(row))
							{
                                this.resultDataControl1.SetCurrentSlideSelectedRowIndex(row);
                                classType = this.resultDataControl1.CurrentSlide.SelectedRowClassType;
								if (instanceDataView == null ||
									instanceDataView.BaseClass.Name != classType)
								{
									instanceDataView = _metaData.GetDetailedDataView(classType);
								}

                                instanceDataView.CurrentObjId = this.resultDataControl1.CurrentSlide.SelectedRowObjId;

								queries[index++] =  instanceDataView.DeleteQuery;
							}
						}

                        // Show the status
                        ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.DeletingInstance"));

						// invoke the web service synchronously
						string deletedInstanceIds = _dataService.ExecuteUpdateQueries(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
							queries);

                        // The current selected row index may be invalid after delete
                        // set it to the first row
                        this.resultDataControl1.SetCurrentSlideSelectedRowIndex(0);
                        this.resultDataControl1.CurrentSlide.IsRowExpanded = false;

                        ShowQueryExecutionStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.InstanceDeleted"));
					}
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
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
			}
		}

		/// <summary>
		/// Delete all instances in the seleted class.
		/// </summary>
		internal void DeleteAllInstances()
		{
			try
			{
                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.DeleteAllInstances"),
					"Confirm Dialog", MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) == DialogResult.Yes)
				{
                    string className = this.resultDataControl1.CurrentSlide.DataView.BaseClass.Name;

                    // Show the status
                    ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.DeletingAllInstances"));

					// invoke the web service synchronously
                    CMDataServiceStub dataService = new CMDataServiceStub();
                    
                    dataService.DeleteAllInstances(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
						className);
	
                    // The current selected row index may be invalid after delete
                    // set it to the first row
                    this.resultDataControl1.SetCurrentSlideSelectedRowIndex(0);
                    this.resultDataControl1.CurrentSlide.IsRowExpanded = false;

                    _menuItemStates.SetState(MenuItemID.EditDeleteAllInstances, false);

                    ShowQueryExecutionStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.InstanceDeleted"));
				}
			}
			finally
			{
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
			}
		}

		/// <summary>
		/// Build full-text index for the seleted class.
		/// </summary>
		internal void BuildFullTextIndex()
		{
			MetaDataTreeNode treeNode = (MetaDataTreeNode) this.treeView.SelectedNode;
			_fullTextIndexClasses = new SchemaModelElementCollection();
			_currentIndexClassPosition = 0;

            CMDataServiceStub dataService = new CMDataServiceStub();

            bool isExternalFullTextSearch = dataService.IsExternalFullTextSearch(); // if true, server uses external full text search, such as ElasticSearch

            if (treeNode.Type == Newtera.WinClientCommon.NodeType.ClassNode)
			{
                ClassElement classElement = (ClassElement)treeNode.MetaDataElement;
                if (isExternalFullTextSearch)
                {
                    // add full-text search enabled bottom classes
                    AddFullTextSearchEnabledBottomClasses(_fullTextIndexClasses, classElement);
                }
			}
            else if (treeNode.Type == Newtera.WinClientCommon.NodeType.ClassesFolder ||
                treeNode.Type == Newtera.WinClientCommon.NodeType.SchemaInfoNode)
			{
				// build indexes for the schema, find all classes that have
				// full-text definitions
				foreach (ClassElement root in this._metaData.SchemaModel.RootClasses)
				{
                    if (!_treeBuilder.CheckPermission ||
                        PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, root, XaclActionType.Read))
                    {
                        if (isExternalFullTextSearch)
                        {
                            // add full-text search enabled bottom classes
                            AddFullTextSearchEnabledBottomClasses(_fullTextIndexClasses, root);
                        }
                    }
				}
			}

			if (_fullTextIndexClasses.Count > 0)
			{
				ClassElement classElement = (ClassElement) _fullTextIndexClasses[_currentIndexClassPosition];
	
				_isRequestComplete = false;

				// invoke the web service synchronously

                // Show the status
                ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.BuildFullTextIndex"));

                dataService.BuildFullTextIndex(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
					classElement.Name);

                BuildFullTextIndexDone();
			}
		}

		/// <summary>
		/// Show all instances of the currently displayed class in the datagrid
		/// </summary>
		internal void ShowAllInstances()
		{
			// to get all instances, we need to get the total count
			if (this.resultDataControl1.CurrentSlide.TotalInstanceCount < 0)
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
		internal void GetAllInstances()
		{
            if (this.resultDataControl1.CurrentSlide.TotalInstanceCount > DATA_THRESHHOLD)
			{
                string msg = Newtera.WindowsControl.MessageResourceManager.GetString("DataView.LargeData");
                int totalCount = this.resultDataControl1.CurrentSlide.TotalInstanceCount;
				msg = String.Format(msg, totalCount, totalCount/1000);
				if (MessageBox.Show(msg,
					"Confirm", MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) == DialogResult.No)
				{
					// user do not want to load the data for now
					return;
				}
			}

            if (this.resultDataControl1.CurrentSlide.DataView != null)
			{
                string query = this.resultDataControl1.CurrentSlide.DataView.SearchQueryWithPKValues;

				_isRequestComplete = false;
				_isCancelled = false;

                // Show the status
                ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.GettingData"));

                // invoke the web service asynchronously
                string queryId = _dataService.BeginQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
					query,
					_pageSize);

                BeginQueryDone(queryId);
		
			}
		}

        /// <summary>
        /// Export a xml document by choosing data instance(s) and a xml schema
        /// </summary>
        internal void ExportXmlDoc()
        {
            try
            {
                string xmlSchemaName = "";

                // count number of selected rows
                int selectedRowCount = 0;
                for (int row = 0; row < this.resultDataControl1.CurrentSlide.RowCount; row++)
                {
                    if (this.resultDataControl1.IsRowSelected(row))
                    {
                        selectedRowCount++;
                    }
                }

                // create a search query for the single selected instance
                if (selectedRowCount == 1)
                {
                    string query = null;
                    string classType;
                    DataViewModel instanceDataView = null;
                    for (int row = 0; row < this.resultDataControl1.CurrentSlide.RowCount; row++)
                    {
                        if (this.resultDataControl1.IsRowSelected(row))
                        {
                            this.resultDataControl1.SetCurrentSlideSelectedRowIndex(row);
                            classType = this.resultDataControl1.CurrentSlide.SelectedRowClassType;
                            if (instanceDataView == null ||
                                instanceDataView.BaseClass.Name != classType)
                            {
                                instanceDataView = _metaData.GetDetailedDataView(classType);
                            }

                            string instanceId = this.resultDataControl1.CurrentSlide.SelectedRowObjId;

                            query = instanceDataView.GetInstanceQuery(instanceId);

                            break;
                        }
                    }

                    // select a xml schema view model
                    XmlSchemaViewsDialog dialog = new XmlSchemaViewsDialog();
                    dialog.BaseClassName = instanceDataView.BaseClass.ClassName;
                    dialog.MetaData = _metaData;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        xmlSchemaName = dialog.SelectedXMLSchemaName;

                        // Show the status
                        ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.GeneratingXML"));

                        // invoke the web service synchronously
                        XmlDocument xmlDoc = (XmlDocument)_dataService.GenerateXmlDoc(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                            xmlSchemaName, query, instanceDataView.BaseClass.ClassName);

                        string xmlFileName = GetSaveXMLFileName();

                        if (xmlFileName != null)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.AppendChild(doc.ImportNode(xmlDoc.DocumentElement, true));

                            doc.Save(xmlFileName);
                        }
                    }

                    ShowQueryExecutionStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.GeneratingXMLCompleted"));
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
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
            }
        }

        private void AddFullTextSearchEnabledBottomClasses(SchemaModelElementCollection indexClasses, ClassElement classElement)
        {
			// get the bottom classes and check each of them if it has an attribute with "IsFullTextSearchAttribute" enabled
			SchemaModelElementCollection bottomClasses;

            if (classElement.IsLeaf)
            {
                bottomClasses = new SchemaModelElementCollection();
                bottomClasses.Add(classElement);
            }
            else
            {
				// get the bottom classes and check each of them if it has an attribute with "IsFullTextSearchAttribute" enabled
				bottomClasses = _metaData.GetBottomClasses(classElement.Name);
            }

            bool hasFullTextSearchableAttribute;
            foreach (ClassElement bottomClass in bottomClasses)
            {
                hasFullTextSearchableAttribute = false;
                ClassElement currentClass = bottomClass;
                while (currentClass != null)
                {
                    foreach (SimpleAttributeElement simpleAttribute in currentClass.SimpleAttributes)
                    {
                        if (simpleAttribute.IsFullTextSearchAttribute)
                        {
                            hasFullTextSearchableAttribute = true;
                            break;
                        }
                    }
                    
                    if (hasFullTextSearchableAttribute)
                    {
                        break;
                    }
                    
                    currentClass = currentClass.ParentClass;
                }
                
                if (hasFullTextSearchableAttribute)
                {
                    indexClasses.Add(bottomClass);
                }
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

		/// <summary>
		/// The AsyncCallback event handler for BeginQuery web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void BeginQueryDone(string queryId)
		{
			try
			{						
				DataSet masterDataSet = null;

				// It is a Worker thread, continue getting rest of data
				DataSet slaveDataSet;
				int currentPageIndex = 0;
				int instanceCount = 0;
				int start, end;
				XmlReader xmlReader;
				XmlNode xmlNode;
                while (instanceCount < this.resultDataControl1.CurrentSlide.TotalInstanceCount && !_isCancelled)
				{
					start = currentPageIndex * _pageSize + 1;
					end = start + this._pageSize - 1;
                    if (end > this.resultDataControl1.CurrentSlide.TotalInstanceCount)
					{
                        end = this.resultDataControl1.CurrentSlide.TotalInstanceCount;
					}

					// invoke the web service synchronously to get data in pages
					xmlNode = _dataService.GetNextResult(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
						queryId);

					if (xmlNode == null)
					{
						// end of result
						break;
					}

					slaveDataSet = new DataSet();

					xmlReader = new XmlNodeReader(xmlNode);
					slaveDataSet.ReadXml(xmlReader);

                    instanceCount += slaveDataSet.Tables[this.resultDataControl1.CurrentSlide.DataView.BaseClass.Name].Rows.Count;

					if (masterDataSet == null)
					{
						// first page
						masterDataSet = slaveDataSet;
						masterDataSet.EnforceConstraints = false;
					}
					else
					{
						// append to the master dataset
						AppendDataSet(masterDataSet, slaveDataSet);
					}

					currentPageIndex++;
				}

				// disable the next page button
				this._menuItemStates.SetState(MenuItemID.ViewNextPage, false);

				ShowQueryResult(masterDataSet);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			finally
			{
				// finish the query
				if (queryId != null)
				{
					_dataService.EndQuery(queryId);
				}
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
		/// Append a slave DataSet to a master dataset
		/// </summary>
		/// <param name="master">The master DataSet</param>
		/// <param name="slave">The slave DataSet</param>
		private void AppendDataSet(DataSet master, DataSet slave)
		{
            string tableName = this.resultDataControl1.CurrentSlide.DataView.BaseClass.Name;
			DataTable masterTable = master.Tables[tableName];
			DataTable slaveTable = slave.Tables[tableName];

			foreach (DataRow row in slaveTable.Rows)
			{
				masterTable.ImportRow(row);
			}
		}

		/// <summary>
		/// Launch a chart wizard
		/// </summary>
		internal void LaunchChartWizard()
		{
		}

		/// <summary>
		/// Export the instances currently shown in the datagrid
		/// </summary>
		internal void ExportData()
		{
			string fileName = null;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
 
			saveFileDialog.InitialDirectory = "c:\\" ;
			//saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Filter = Newtera.WindowsControl.MessageResourceManager.GetString("DataView.ExportFileFilter");
			saveFileDialog.FilterIndex = 1 ;
			saveFileDialog.RestoreDirectory = false ;
 
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					fileName = saveFileDialog.FileName;
					int index = saveFileDialog.FilterIndex;
					SaveDataToFile(fileName, index);
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
		/// Set the Default view as default DataView
		/// </summary>
		internal void SetDefaultView()
		{
			this.defaultViewMenuItem.Checked = true;
			this.dgDefaultViewToolStripMenuItem.Checked = true;
			this.detailedViewMenuItem.Checked = false;
			this.dgDetailedViewToolStripMenuItem.Checked = false;

			this._isDefaultDataView = true;

			// show default view
			ShowSelectedTreeNode();
		}

		/// <summary>
		/// Set the Detailed view as default DataView
		/// </summary>
		internal void SetDetailedView()
		{
			this.defaultViewMenuItem.Checked = false;
            this.dgDefaultViewToolStripMenuItem.Checked = false;
			this.detailedViewMenuItem.Checked = true;
            this.dgDetailedViewToolStripMenuItem.Checked = true;

			this._isDefaultDataView = false;

			// show detailed view
			ShowSelectedTreeNode();
		}

		/// <summary>
		/// Save the data views to database
		/// </summary>
		internal void SaveDataViewsToDatabase()
		{
			// display a text in the status bar
            ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.SavingDataView"));

			StringBuilder builder = new StringBuilder();
			StringWriter writer = new StringWriter(builder);
		
			_metaData.DataViews.Write(writer);

			string xmlString = builder.ToString();

            try
            {
                // invoke the web service asynchronously
                ((DesignStudio)this.MdiParent).MetaDataService.SetDataViews(
                    ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                    xmlString);

                _metaData.DataViews.IsAltered = false;
                _menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
            }
            finally
            {
                // set the status back to ready message
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
            }
		}

		/// <summary>
		/// Replace the range expression in the query with empty bracket []
		/// </summary>
		/// <param name="originalQuery">The original query</param>
		/// <returns>The fixed query</returns>
		private string fixQuery(string originalQuery)
		{
			int start = originalQuery.IndexOf("[");
			int end = originalQuery.IndexOf("]");

			if (start > 0 && end > 0 && start < end)
			{
				return originalQuery.Substring(0, start + 1) + originalQuery.Substring(end);
			}
			else
			{
				return originalQuery;
			}
		}

		/// <summary>
		/// Get a query that returns results for the given page
		/// </summary>
		/// <param name="originalQuery">The original query</param>
		/// <param name="pageIndex">The current page index</param>
		/// <returns>The paged query</returns>
		private string GetPagedQuery(string originalQuery, int pageIndex)
		{
            int totalCount = this.resultDataControl1.CurrentSlide.TotalInstanceCount;
			int from = pageIndex * _pageSize + 1;
			int to = (pageIndex + 1) * _pageSize;
			if (to > totalCount)
			{
				to = totalCount;
			}

			return originalQuery.Replace("[]", "[" + from + " to " + to + "]");
		}

		/// <summary>
		/// The AsyncCallback event handler for build full-text index.
		/// </summary>
		/// <param name="res">The result</param>
		private void BuildFullTextIndexDone()
		{
			try
			{
				_currentIndexClassPosition++; // move to next class in the list

				while (_currentIndexClassPosition < _fullTextIndexClasses.Count)
				{
					ClassElement classElement = (ClassElement) _fullTextIndexClasses[_currentIndexClassPosition];

					CMDataServiceStub dataService = new CMDataServiceStub();
					

					// invoke the web service synchronously
					dataService.BuildFullTextIndex(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
						classElement.Name);

					_currentIndexClassPosition++; // move to next class in the list
				}

				((DesignStudio) this.MdiParent).ShowReadyStatus();
						
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			finally
			{
			}
		}

		/// <summary>
		/// Gets and display the total instance count of the current query.
		/// </summary>
		internal void ExecuteQueryCount()
		{
            if (this.resultDataControl1.CurrentSlide.DataView != null)
			{

                string query = this.resultDataControl1.CurrentSlide.DataView.SearchQuery;

				_isRequestComplete = false;

				// invoke the web service
				int count = _dataService.ExecuteCount(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
					query);

                ExecuteCountDone(count);
            }
		}

		/// <summary>
		/// The AsyncCallback event handler for ExecuteCount web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void ExecuteCountDone(int count)
		{
			try
			{
				((DesignStudio) this.MdiParent).ShowReadyStatus();

				ShowInstanceCount(count);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			finally
			{
			}
		}

		private delegate void ShowInstanceCountDelegate(int count);

		/// <summary>
		/// Display the instance count of current query
		/// </summary>
		/// <param name="count">The count.</param>
		private void ShowInstanceCount(int count)
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue
                this.resultDataControl1.ShowInstanceCount(count);
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowInstanceCountDelegate showCount = new ShowInstanceCountDelegate(ShowInstanceCount);

				this.BeginInvoke(showCount, new object[] {count});
			}
		}

        /// <summary>
        /// Update data instances whose attribute values have been changed in the data grid control
        /// </summary>
        private void UpdateInstances()
        {
            if (_activeXService == null)
            {
                _activeXService = new ActiveXControlServiceStub();
            }

            try
            {
                _isRequestComplete = false;

                // convert the data grid view to xml
                DataSet changedDataSet = this.resultDataControl1.CurrentSlide.DataSet.GetChanges(DataRowState.Modified);

                if (changedDataSet != null)
                {
                    string xml = changedDataSet.GetXml();

                    // invoke the web service asynchronously
                    string msg = _activeXService.UpdateData(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        this.resultDataControl1.CurrentSlide.DataView.BaseClass.ClassName,
                        xml);

                    AcceptChanges(msg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Accept changes
        /// </summary>
        private void AcceptChanges(string msg)
        {
            string displayMsg = Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.UpdateDone");
            if (!string.IsNullOrEmpty(msg))
            {
                displayMsg = msg;
            }
            MessageBox.Show(displayMsg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.resultDataControl1.CurrentSlide.DataSet.AcceptChanges();
 
        }

		/// <summary>
		/// A handler to call when a value of the data views changed
		/// </summary>
		/// <param name="sender">the IDataViewElement that cause the event</param>
		/// <param name="e">the arguments</param>
		private void DataViewChangedHandler(object sender, EventArgs e)
		{
			ValueChangedEventArgs args = (ValueChangedEventArgs) e;

			// set the menu item state
			if (_metaData.IsLockObtained)
			{
				_menuItemStates.SetState(MenuItemID.FileSaveDatabase, true);
			}
		}

		/// <summary>
		/// Enable/disable the buttons for adding/deleting attachments of an instance
		/// according to its permissions
		/// </summary>
		/// <param name="instanceData">The instance data</param>
		private void SetAttachmentEditButtonStates(InstanceData instanceData)
		{
			if (instanceData.HasPermission(XaclActionType.Upload))
			{
				this.addAttachmentButton.Enabled = true;
			}
			else
			{
				this.addAttachmentButton.Enabled = false;
			}

			if (instanceData.HasPermission(XaclActionType.Delete))
			{
				this.deleteAttachmentButton.Enabled = true;
			}
			else
			{
				this.deleteAttachmentButton.Enabled = false;
			}

			if (instanceData.HasPermission(XaclActionType.Download))
			{
				this.downloadButton.Enabled = true;
			}
			else
			{
				this.downloadButton.Enabled = false;
			}
		}

		/// <summary>
		/// set the button state of build full-text index to true if the class
		/// contains the full-text enabled attributes, false otherwise.
		/// </summary>
		/// <param name="className">The class name</param>
		private void SetBuildFullTextIndexButtonState(string className)
		{
			bool hasFullTextAttribute = false;

			if (className != null)
			{
				hasFullTextAttribute = HasFullTextAttribute(className);
			}
			else
			{
				// check if any of root classes have full-text search attribute
				foreach (ClassElement root in _metaData.SchemaModel.RootClasses)
				{
                    if (!_treeBuilder.CheckPermission ||
                        PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, root, XaclActionType.Read))
                    {
                        if (HasFullTextAttribute(root.Name))
                        {
                            hasFullTextAttribute = true;
                            break;
                        }
                    }
				}
			}

			if (hasFullTextAttribute)
			{
				_menuItemStates.SetState(MenuItemID.ToolBuildFullTextIndex, true);
			}
			else
			{
				_menuItemStates.SetState(MenuItemID.ToolBuildFullTextIndex, false);
			}
		}

		/// <summary>
		/// Get information indicating whether a class has full-text search attribute
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>true if it has full-text search attribute, false otherwise.</returns>
		private bool HasFullTextAttribute(string className)
		{
			bool hasFullTextAttribute = false;

			DataViewModel dataView = this._metaData.GetCompleteDataView(className);
				
			InstanceView instanceView = new InstanceView(dataView);

			foreach (InstanceAttributePropertyDescriptor property in instanceView.GetProperties(null))
			{
				if (property.IsFullTextSearchAttribute)
				{
					hasFullTextAttribute = true;
					break;
				}
			}

			return hasFullTextAttribute;
		}

		/// <summary>
		/// Export data to the given file
		/// </summary>
		/// <param name="filePath">The file path</param>
		/// <param name="exportTypeIndex">The export type index</param>
		private void SaveDataToFile(string filePath, int exportTypeIndex)
		{
			// create the specified exporter
			IExporter exporter = null;
			
			switch (exportTypeIndex)
			{
				case 1: // excel type
					exporter = ExporterFactory.Instance.Create("Newtera.Export.ExcelExporter, Newtera.Export");
					break;

				case 2: // tab delimited text
					exporter = ExporterFactory.Instance.Create("Newtera.Export.TabDelimitedTextExporter, Newtera.Export");
					break;
			}

			if (exporter != null)
			{
				try
				{
					// begine export
					exporter.BeginExport(filePath);

					int totalCount = 0;
					DataTable dataTable = null;
                    if (this.resultDataControl1.CurrentSlide.DataSet.Tables[this.resultDataControl1.CurrentSlide.DataView.BaseClass.ClassName] != null)
					{
                        dataTable = this.resultDataControl1.CurrentSlide.DataSet.Tables[this.resultDataControl1.CurrentSlide.DataView.BaseClass.ClassName];
						totalCount = dataTable.Rows.Count;
					}

					// export data to the file in blocks
					DataTable dt;
					int currentRowIndex = 0;
					while (currentRowIndex < totalCount)
					{
                        dt = TransformDataTable(dataTable, this.resultDataControl1.CurrentSlide.DataView, currentRowIndex);
						exporter.ExportData(dt);

						currentRowIndex += dt.Rows.Count;
					}
				}
				finally
				{
					exporter.EndExport();
				}
			}
		}

		private DataTable TransformDataTable(DataTable dataTable, DataViewModel dataView, int currentRowIndex)
		{
			int pageSize = 100;
			DataTable dt = new DataTable(dataView.BaseClass.Caption);

			// create columns
			foreach (IDataViewElement resultAttribute in dataView.ResultAttributes)
			{
                // Add a column style for each result of DataSimpleAttribute or DataVirtualAttribute
                // that has corresponding column in the data table
				if ((resultAttribute is DataSimpleAttribute ||
                    resultAttribute is DataVirtualAttribute) &&
                    dataTable.Columns[resultAttribute.Name] != null)
				{
					dt.Columns.Add(resultAttribute.Name);
				}
			}

			// create rows
			DataRow newRow;
			for (int row = currentRowIndex; row < (currentRowIndex + pageSize); row ++)
			{
				if (row < dataTable.Rows.Count)
				{
					newRow = dt.NewRow();
					foreach (IDataViewElement resultAttribute in dataView.ResultAttributes)
					{
						if ((resultAttribute is DataSimpleAttribute ||
                            resultAttribute is DataVirtualAttribute) &&
                            dt.Columns[resultAttribute.Name] != null)
						{
							// TODO, handle Enum values
							newRow[resultAttribute.Name] = dataTable.Rows[row][resultAttribute.Name].ToString();
						}
					}

					dt.Rows.Add(newRow);
				}
				else
				{
					break;
				}
			}

			// replace the column name with attribute captions
			foreach (IDataViewElement resultAttribute in dataView.ResultAttributes)
			{
				// Add a column style for each result of DataSimpleAttribute type
				if ((resultAttribute is DataSimpleAttribute ||
                    resultAttribute is DataVirtualAttribute) &&
                    dt.Columns[resultAttribute.Name] != null)
				{
					dt.Columns[resultAttribute.Name].ColumnName = resultAttribute.Caption;
				}
			}

			return dt;
		}

		/// <summary>
		/// Refresh the display tree based on the meta data
		/// </summary>
		internal void RefreshMetaDataTree()
		{
			TreeNode root;
			if (_metaData != null)
			{
				root = _treeBuilder.BuildTree(_metaData);

				treeView.BeginUpdate();
				treeView.Nodes.Clear();
				treeView.Nodes.Add(root);
				root.Expand();

				// Make the ClassNode selected initially
				//treeView.SelectedNode = root.Nodes[0];
				treeView.EndUpdate();

				InitializeMenuItemStates();

				// clear the data grid and property editor
                this.resultDataControl1.ClearDataGrids();
				
				this.instanceViewGrid.SelectedObject = null;
			}
		}

        private void ViewLoggingMessage()
        {
            ViewLoggingMessageDialog dialog = new ViewLoggingMessageDialog();
            dialog.SchemaId = this.resultDataControl1.MetaData.SchemaInfo.NameAndVersion;
            dialog.ClassName = this.resultDataControl1.CurrentSlide.DataView.BaseClass.Name;

            dialog.ShowDialog();
        }

		#endregion

		private void DataViewer_Load(object sender, System.EventArgs e)
		{
			// Register the menu item state change event handler
			_menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

			// Initialize the FileTypeInfoManager
			FileTypeInfoManager.Instance.Initialize();

			// set small and large image lists to the attachment list view
			FileTypeInfoManager.Instance.LoadSmallImages(this.fileTypeSmallImageList);
			FileTypeInfoManager.Instance.LoadLargeImages(this.fileTypeLargeImageList);
		}

		private void DataViewer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_metaData.DataViews.IsAltered && _metaData.IsLockObtained)
			{
				if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DataViewer.DataViewChanged"),
					"Confirm Dialog", MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) == DialogResult.Yes)
				{
					SaveDataViewsToDatabase();
				}
			}

			return;
		}

		private void MenuItemStateChanged(object sender, System.EventArgs e)
		{
			StateChangedEventArgs args = (StateChangedEventArgs) e;

			// set the toolbar button states
			switch (args.ID)
			{
				case MenuItemID.FileSaveDatabase:
					this.saveDataViewMenuItem.Enabled = args.State;
					break;
				case MenuItemID.FileExport:
					this.exportMenuItem.Enabled = args.State;
					this.dgExportDataToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditSearch:
					this.searchMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditShowAllInstances:
					this.showAllInstancesMenuItem.Enabled = args.State;
					this.dgShowAllInstancesToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditNewInstance:
					this.newInstanceMenuItem.Enabled = args.State;
					this.dgNewToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditSaveInstance:
					this.saveInstanceMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditSaveInstanceAs:
					this.saveInstanceAsMenuItem.Enabled = args.State;
					this.dgDuplicateInstanceToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditDeleteInstance:
					this.deleteInstanceMenuItem.Enabled = args.State;
					this.dgDeleteToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditDeleteAllInstances:
					this.deleteAllMenuItem.Enabled = args.State;
					this.dgDeleteAllToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditAdd:
					this.newDataViewMenuItem.Enabled = args.State;
					this.tvNewDataViewMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditModify:
					this.modifyDataViewMenuItem.Enabled = args.State;
					this.tvModifyDataViewMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditDelete:
					this.deleteDataViewMenuItem.Enabled = args.State;
					this.tvDeleteDataViewMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewDefault:
					this.defaultViewMenuItem.Enabled = args.State;
					this.dgDefaultViewToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewDetailed:
					this.detailedViewMenuItem.Enabled = args.State;
					this.dgDetailedViewToolStripMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewChart:
					this.chartMenuItem.Enabled = args.State;
					break;
                case MenuItemID.ViewPivot:
                    this.pivotTableMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.ViewLogging:
                    this.loggingMenuItem.Enabled = args.State;
                    this.tvLoggingMenuItem.Enabled = args.State;
                    break;
				case MenuItemID.ViewNextRow:
					this.nextRowMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewPrevRow:
					this.prevRowMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewPrevPage:
					this.prevPageMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewNextPage:
					this.nextPageMenuItem.Enabled = args.State;
					break;
				case MenuItemID.CollapseNode:
					//this.tvCollapseMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ExpandNode:
					//this.tvExpandMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ToolBuildFullTextIndex:
					this.buildFullTextIndexMenuItem.Enabled = args.State;
					this.tvBuildFullTextIndexMenuItem.Enabled = args.State;
					break;
			}
		}

		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			ShowSelectedTreeNode();
		}

		private void searchMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowSearchDialog(sender, e);
		}

		private void resultTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selectedIndex = this.resultTabControl.SelectedIndex;

            if (this.resultDataControl1.CurrentSlide != null)
			{
				switch (selectedIndex)
				{
					case 0:
						_currentSubView = ResultViewType.InstanceView;
                        this.resultDataControl1.SetInstanceEditButtonStates(this.resultDataControl1.CurrentSlide.RowView.InstanceData);
						break;
					case 1:
						_currentSubView = ResultViewType.AttachmentView;
						EnableInstanceEditButtons(false);
						break;
				}
			}

			ShowSubView();
		}

		private void newInstanceMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowNewInstanceDialog();
		}

		private void saveInstanceMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveInstance();
		}

		private void saveInstanceAsMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowNewInstanceAsDialog();		
		}

		private void deleteInstanceMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteInstances();
		}

		private void countRowButton_Click(object sender, System.EventArgs e)
		{
			ExecuteQueryCount();
		}

		private void newDataViewMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowDataViewDialog(null);
		}

		private void tvNewDataViewMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowDataViewDialog(null);	
		}

		private void deleteDataViewMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteDataView(sender, e);
		}

		private void tvDeleteDataViewMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteDataView(sender, e);		
		}

		private void tvModifyDataViewMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowModifyDataViewDialog();
		}

		private void modifyDataViewMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowModifyDataViewDialog();
		}

		private void saveDataViewMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveDataViewsToDatabase();
		}

		private void runXQueryMenuItem_Click(object sender, System.EventArgs e)
		{
			RunXQueryDialog dialog = new RunXQueryDialog();
			dialog.SchemaInfo = _metaData.SchemaInfo;
			dialog.DataService = _dataService;
			dialog.Show();
		}

		private void addAttachmentButton_Click(object sender, System.EventArgs e)
		{
			AddAttachmentDialog dialog = new AddAttachmentDialog();
			
			dialog.SchemaInfo = this._metaData.SchemaInfo;
            dialog.InstanceId = this.resultDataControl1.CurrentSlide.SelectedRowObjId;
            dialog.ClassName = this.resultDataControl1.CurrentSlide.SelectedRowClassType;
			dialog.AttachmentItems = this.attachmentListView.Items;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				// increase the attachment number by one to the cached result
				// so that we don't have to fetch the result from server
                this.resultDataControl1.CurrentSlide.RowView.InstanceData.IncreamentANUM();

				_isAttachmentChanged = true;

				this.resultDataControl1.ShowDataSlide(); // refresh the result window
			}
		}

		private void deleteAttachmentButton_Click(object sender, System.EventArgs e)
		{
			if (this.attachmentListView.SelectedItems.Count == 1)
			{
				AttachmentListViewItem item = (AttachmentListViewItem) this.attachmentListView.SelectedItems[0];

				DeleteAttachment(item.AttachmentInfo);
			}
		}

		private void dowloadButton_Click(object sender, System.EventArgs e)
		{
			if (this.attachmentListView.SelectedItems.Count == 1)
			{
				AttachmentListViewItem item = (AttachmentListViewItem) this.attachmentListView.SelectedItems[0];

				SaveFileDialog saveFileDialog = new SaveFileDialog();
	 
				FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(item.AttachmentInfo.Type);
				saveFileDialog.InitialDirectory = "c:\\" ;
				saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
				saveFileDialog.FilterIndex = 2 ;
				saveFileDialog.RestoreDirectory = false ;
				saveFileDialog.FileName = item.AttachmentInfo.Name;
	 
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					_attachmentFileName = saveFileDialog.FileName;

					GetAttachment(item.AttachmentInfo);
				}
			}
		}

		private void DataViewer_Activated(object sender, System.EventArgs e)
		{
			// Refresh the display in case the meta data has been changed
			RefreshMetaDataTree();

            // only one dataviewer for a database
            _menuItemStates.SetState(MenuItemID.FileOpenDataViewer, false);
		}

		private void deleteAllMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteAllInstances();
		}

		private void builFullTextIndexMenuItem_Click(object sender, System.EventArgs e)
		{
			BuildFullTextIndex();
		}

		private void tvBuildFullTextIndexMenuItem_Click(object sender, System.EventArgs e)
		{
			BuildFullTextIndex();
		}

		private void showAllInstancesMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowAllInstances();
		}

		private void chartMenuItem_Click(object sender, System.EventArgs e)
		{
			LaunchChartWizard();
		}

		private void exportMenuItem_Click(object sender, System.EventArgs e)
		{
			ExportData();
		}

		private void defaultViewMenuItem_Click(object sender, System.EventArgs e)
		{
			SetDefaultView();
		}

		private void detailedViewMenuItem_Click(object sender, System.EventArgs e)
		{
			SetDetailedView();	
		}

        private void resultDataControl1_RowSelectedIndexChangedEvent(object sender, EventArgs e)
        {
            ShowSubView();
        }

        private void resultDataControl1_RequestForCountEvent(object sender, EventArgs e)
        {
            ExecuteQueryCount();
        }

        private void resultDataControl1_RequestForDataEvent(object sender, EventArgs e)
        {
            ExecuteQuery();
        }

        private void dgNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewInstanceDialog();
        }

        private void dgDuplicateInstanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewInstanceAsDialog();
        }

        private void dgDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteInstances();
        }

        private void dgDeleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllInstances();
        }

        private void dgDefaultViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDefaultView();
        }

        private void dgDetailedViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDetailedView();
        }

        private void dgShowAllInstancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAllInstances();
        }

        private void dgExportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportData();
        }

        private void resultDataControl1_DataGridClearedEvent(object sender, EventArgs e)
        {
            this.instanceViewGrid.SelectedObject = null;
        }

        private void nextRowMenuItem_Click(object sender, EventArgs e)
        {
            this.resultDataControl1.MoveToNextRow();
        }

        private void prevRowMenuItem_Click(object sender, EventArgs e)
        {
            this.resultDataControl1.MoveToPrevRow();
        }

        private void nextPageMenuItem_Click(object sender, EventArgs e)
        {
            this.resultDataControl1.MoveToNextPage();
        }

        private void prevPageMenuItem_Click(object sender, EventArgs e)
        {
            this.resultDataControl1.MoveToPrevPage();
        }

        private void DataViewer_Deactivate(object sender, EventArgs e)
        {
            _menuItemStates.SetState(MenuItemID.FileOpenDataViewer, true);
        }

        private void resultDataControl1_RequestForSaveInstancesEvent(object sender, EventArgs e)
        {
            UpdateInstances();
        }

        private void pivotTableMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void loggingMenuItem_Click(object sender, EventArgs e)
        {
            ViewLoggingMessage();
        }

        private void tvLoggingMenuItem_Click(object sender, EventArgs e)
        {
            ViewLoggingMessage();
        }

        private void dgExportXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportXmlDoc();
        }
	}
}

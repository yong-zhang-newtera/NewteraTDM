using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Security.Principal;
using System.Globalization;

using Newtera.Common.Core;
using Newtera.Studio.PackUnpack;
using Newtera.DataGridActiveX.ActiveXControlWebService;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Studio.ImportExport;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DesignStudio.
	/// </summary>
	public class DesignStudio : System.Windows.Forms.Form
	{
		private const string DATA_DIR = "data";

		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private MetaDataServiceStub _metaDataService;
		private MDIChildType _openningChild;
		private Newtera.Common.Core.SchemaInfo _selectedSchemaInfo;
		private Hashtable _schemaEditors;
		private Hashtable _dataViewers;
		private PackData _packer;
		private MethodInvoker _backupMethod;
		private UnpackData _unpacker;
		private MethodInvoker _restoreMethod;
		private bool _isDBAUser;
		private bool _layoutCalled = false;
		private bool _isLockObtained = false;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.MenuItem fileNewMenuItem;
		private System.Windows.Forms.MenuItem editMenuItem;
		private System.Windows.Forms.MenuItem viewMenuItem;
		private System.Windows.Forms.MenuItem toolMenuItem;
		private System.Windows.Forms.MenuItem windowMenuItem;
		private System.Windows.Forms.MenuItem helpMenuItem;
		private System.Windows.Forms.MenuItem fileOpenMenuItem;
		private System.Windows.Forms.MenuItem fileOpenFileMenuItem;
		private System.Windows.Forms.MenuItem fileOpenDatabaseMenuItem;
		private System.Windows.Forms.MenuItem fileCloseMenuItem;
		private System.Windows.Forms.MenuItem fileExitMenuItem;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ImageList toolBarImageList;
		private System.Windows.Forms.ToolBarButton newToolBarButton;
		private System.Windows.Forms.ToolBarButton openFileToolBarButton;
		private System.Windows.Forms.ToolBarButton saveToolBarButton;
		private System.Windows.Forms.ToolBarButton cutToolBarButton;
		private System.Windows.Forms.ToolBarButton separator1;
		private System.Windows.Forms.ToolBarButton copyToolBarButton;
		private System.Windows.Forms.ToolBarButton openDBSchemaToolBarButton;
		private System.Windows.Forms.ToolBarButton saveDBSchemaToolBarButton;
		private System.Windows.Forms.ToolBarButton addItemToolBarButton;
		private System.Windows.Forms.ToolBarButton deleteItemToolBarButton;
		private System.Windows.Forms.ToolBarButton helpToolBarButton;
		private System.Windows.Forms.ToolBarButton exitToolBarButton;
		private System.Windows.Forms.ToolBarButton pasteToolBarButton;
		private System.Windows.Forms.MenuItem cutMenuItem;
		private System.Windows.Forms.MenuItem copyMenuItem;
		private System.Windows.Forms.MenuItem pasteMenuItem;
		private System.Windows.Forms.MenuItem aboutMenuItem;
		private System.Windows.Forms.ToolBarButton validateToolBarButton;
		private System.Windows.Forms.StatusBarPanel selectionPanel;
		private System.Windows.Forms.StatusBarPanel statusPanel;
		private System.Windows.Forms.StatusBarPanel progressPanel;
		private System.Windows.Forms.MenuItem fileOpenDataViewerMenuItem;
		private System.Windows.Forms.ToolBarButton openDataViewerBarButton;
		private System.Windows.Forms.ToolBarButton separator3;
		private System.Windows.Forms.ToolBarButton separator4;
		private System.Windows.Forms.ToolBarButton separator5;
		private System.Windows.Forms.ToolBarButton separator2;
		private System.Windows.Forms.ToolBarButton searchToolBarButton;
		private System.Windows.Forms.ToolBarButton newInstanceToolBarButton;
		private System.Windows.Forms.ToolBarButton saveInstanceAsToolBarButton;
		private System.Windows.Forms.ToolBarButton saveInstanceToolBarButton;
		private System.Windows.Forms.ToolBarButton deleteInstanceToolBarButton;
		private System.Windows.Forms.ToolBarButton modifyItemToolBarButton;
		private System.Windows.Forms.MenuItem changePasswordMenuItem;
		private System.Windows.Forms.MenuItem setupDatabaseMenuItem;
		private System.Windows.Forms.MenuItem setupServerURLMenuItem;
		private System.Windows.Forms.MenuItem createUserRoleMenuItem;
		private System.Windows.Forms.MenuItem fileImportMenuItem;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem fileDisconnectMenuItem;
		private System.Windows.Forms.MenuItem fileConnectMenuItem;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem userManualMenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem backupMenuItem;
		private System.Windows.Forms.MenuItem restoreMenuItem;
		private System.Windows.Forms.MenuItem deleteSchemaMenuItem;
		private System.Windows.Forms.MenuItem protectSchemaMenuItem;
		private System.Windows.Forms.ToolBarButton chartToolBarButton;
		private System.Windows.Forms.ToolBarButton separator6;
		private System.Windows.Forms.ToolBarButton exportToolBarButton;
        private MenuItem refreshSchemaMenuItem;
        private MenuItem menuItem4;
        private StatusBarPanel userPanel;
        private ToolBarButton pivotToolBarButton;
        private MenuItem setupServerIPandPortMenuItem;
        private System.ComponentModel.IContainer components;

		public DesignStudio()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			AppDomain currentDomain = Thread.GetDomain();
			currentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

			_schemaEditors = new Hashtable();
			_dataViewers = new Hashtable();

			_metaDataService = new MetaDataServiceStub();
			_workInProgressDialog = new WorkInProgressDialog();

			_backupMethod = null;
			_packer = null;
			_restoreMethod = null;
			_unpacker = null;

			_isDBAUser = true;

            // set the image getter factory
            TableStyleFactory.Instance.ImageGetterFactory = ImageGetterFactory.Instance;

            // set the image web service proxy globaly
            ImageWebServiceManager.Instance.ImageWebService = new ImageWebServiceProxy();

            XmlDataSourceListHandler.XmlDataSourceService = new Studio.XmlDataSourceService();

            // set windows client flag
            GlobalSettings.Instance.IsWindowClient = true;
		}

		/// <summary>
		/// Gets or sets the information indicating whether the currently loggin user
		/// has DBA role.
		/// </summary>
		public bool IsDBAUser
		{
			get
			{
				return _isDBAUser;
			}
			set
			{
				_isDBAUser = value;
			}
		}

		/// <summary>
		/// Gets the web service for mete data
		/// </summary>
		internal MetaDataServiceStub MetaDataService
		{
			get
			{
				return _metaDataService;
			}
		}

		/// <summary>
		/// Show the working status in the status panel of status bar
		/// </summary>
		/// <param name="text">Status text</param>
		internal void ShowWorkingStatus(string text)
		{
			this.statusPanel.Text = text;
		}

		/// <summary>
		/// Show the ready status in the status panel of status bar
		/// </summary>
		internal void ShowReadyStatus()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DesignStudio));

			this.statusPanel.Text = resources.GetString("statusPanel.Text");
		}

		/// <summary>
		/// Show the current selection text in the selection panel of status bar
		/// </summary>
		/// <param name="text">Showing text</param>
		internal void ShowSelectionStatus(string text)
		{
			this.selectionPanel.Text = text;
		}

        /// <summary>
        /// Display current loggin user at the status bar
        /// </summary>
        private void DisplayCurrentUser()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal != null)
            {
                this.userPanel.Text = principal.DisplayText;
            }
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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

		/// <summary>
		/// Disconnect the database if all of its clients have been closed
		/// </summary>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		private void DisconnectDatabase(MetaDataModel metaData)
		{
			bool allClosed = true;

			if (_schemaEditors[metaData.SchemaInfo.NameAndVersion] != null)
			{
				allClosed = false;
			}
			else if (_dataViewers[metaData.SchemaInfo.NameAndVersion] != null)
			{
				allClosed = false;
			}

			if (allClosed)
			{
				if (metaData.IsLockObtained)
				{
					// unlock the meta data on the server
					try
					{
						this._metaDataService.UnlockMetaData(ConnectionStringBuilder.Instance.Create(metaData.SchemaInfo), false);
					}
					catch (Exception)
					{
					}
				}

				MetaDataStore.Instance.RemoveMetaData(metaData.SchemaInfo);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignStudio));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.fileMenuItem = new System.Windows.Forms.MenuItem();
            this.fileNewMenuItem = new System.Windows.Forms.MenuItem();
            this.fileOpenMenuItem = new System.Windows.Forms.MenuItem();
            this.fileOpenFileMenuItem = new System.Windows.Forms.MenuItem();
            this.fileOpenDatabaseMenuItem = new System.Windows.Forms.MenuItem();
            this.fileOpenDataViewerMenuItem = new System.Windows.Forms.MenuItem();
            this.fileCloseMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.fileImportMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.fileConnectMenuItem = new System.Windows.Forms.MenuItem();
            this.fileDisconnectMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.fileExitMenuItem = new System.Windows.Forms.MenuItem();
            this.editMenuItem = new System.Windows.Forms.MenuItem();
            this.cutMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.pasteMenuItem = new System.Windows.Forms.MenuItem();
            this.viewMenuItem = new System.Windows.Forms.MenuItem();
            this.refreshSchemaMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.toolMenuItem = new System.Windows.Forms.MenuItem();
            this.setupServerIPandPortMenuItem = new System.Windows.Forms.MenuItem();
            this.setupServerURLMenuItem = new System.Windows.Forms.MenuItem();
            this.setupDatabaseMenuItem = new System.Windows.Forms.MenuItem();
            this.createUserRoleMenuItem = new System.Windows.Forms.MenuItem();
            this.protectSchemaMenuItem = new System.Windows.Forms.MenuItem();
            this.changePasswordMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.backupMenuItem = new System.Windows.Forms.MenuItem();
            this.restoreMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteSchemaMenuItem = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.userManualMenuItem = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusPanel = new System.Windows.Forms.StatusBarPanel();
            this.selectionPanel = new System.Windows.Forms.StatusBarPanel();
            this.progressPanel = new System.Windows.Forms.StatusBarPanel();
            this.userPanel = new System.Windows.Forms.StatusBarPanel();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.newToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.openFileToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.openDBSchemaToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.openDataViewerBarButton = new System.Windows.Forms.ToolBarButton();
            this.separator1 = new System.Windows.Forms.ToolBarButton();
            this.saveToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.saveDBSchemaToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.separator2 = new System.Windows.Forms.ToolBarButton();
            this.cutToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.copyToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.pasteToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.separator3 = new System.Windows.Forms.ToolBarButton();
            this.addItemToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.deleteItemToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.modifyItemToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.validateToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.separator4 = new System.Windows.Forms.ToolBarButton();
            this.searchToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.chartToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.exportToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.pivotToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.separator5 = new System.Windows.Forms.ToolBarButton();
            this.newInstanceToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.saveInstanceAsToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.saveInstanceToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.deleteInstanceToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.separator6 = new System.Windows.Forms.ToolBarButton();
            this.helpToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.exitToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.toolBarImageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.statusPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectionPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.viewMenuItem,
            this.toolMenuItem,
            this.windowMenuItem,
            this.helpMenuItem});
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.Index = 0;
            this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.fileNewMenuItem,
            this.fileOpenMenuItem,
            this.fileCloseMenuItem,
            this.menuItem2,
            this.fileImportMenuItem,
            this.menuItem11,
            this.fileConnectMenuItem,
            this.fileDisconnectMenuItem,
            this.menuItem3,
            this.fileExitMenuItem});
            this.fileMenuItem.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.fileMenuItem, "fileMenuItem");
            // 
            // fileNewMenuItem
            // 
            this.fileNewMenuItem.Index = 0;
            resources.ApplyResources(this.fileNewMenuItem, "fileNewMenuItem");
            this.fileNewMenuItem.Click += new System.EventHandler(this.fileNewMenuItem_Click);
            // 
            // fileOpenMenuItem
            // 
            this.fileOpenMenuItem.Index = 1;
            this.fileOpenMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.fileOpenFileMenuItem,
            this.fileOpenDatabaseMenuItem,
            this.fileOpenDataViewerMenuItem});
            this.fileOpenMenuItem.MergeOrder = 1;
            resources.ApplyResources(this.fileOpenMenuItem, "fileOpenMenuItem");
            // 
            // fileOpenFileMenuItem
            // 
            this.fileOpenFileMenuItem.Index = 0;
            resources.ApplyResources(this.fileOpenFileMenuItem, "fileOpenFileMenuItem");
            this.fileOpenFileMenuItem.Click += new System.EventHandler(this.fileOpenFileMenuItem_Click);
            // 
            // fileOpenDatabaseMenuItem
            // 
            this.fileOpenDatabaseMenuItem.Index = 1;
            resources.ApplyResources(this.fileOpenDatabaseMenuItem, "fileOpenDatabaseMenuItem");
            this.fileOpenDatabaseMenuItem.Click += new System.EventHandler(this.fileOpenDatabaseMenuItem_Click);
            // 
            // fileOpenDataViewerMenuItem
            // 
            this.fileOpenDataViewerMenuItem.Index = 2;
            resources.ApplyResources(this.fileOpenDataViewerMenuItem, "fileOpenDataViewerMenuItem");
            this.fileOpenDataViewerMenuItem.Click += new System.EventHandler(this.fileOpenDataViewerMenuItem_Click_1);
            // 
            // fileCloseMenuItem
            // 
            this.fileCloseMenuItem.Index = 2;
            this.fileCloseMenuItem.MergeOrder = 2;
            resources.ApplyResources(this.fileCloseMenuItem, "fileCloseMenuItem");
            this.fileCloseMenuItem.Click += new System.EventHandler(this.fileCloseMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 3;
            this.menuItem2.MergeOrder = 3;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // fileImportMenuItem
            // 
            this.fileImportMenuItem.Index = 4;
            this.fileImportMenuItem.MergeOrder = 4;
            resources.ApplyResources(this.fileImportMenuItem, "fileImportMenuItem");
            this.fileImportMenuItem.Click += new System.EventHandler(this.fileImportMenuItem_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 5;
            this.menuItem11.MergeOrder = 5;
            resources.ApplyResources(this.menuItem11, "menuItem11");
            // 
            // fileConnectMenuItem
            // 
            this.fileConnectMenuItem.Index = 6;
            this.fileConnectMenuItem.MergeOrder = 98;
            resources.ApplyResources(this.fileConnectMenuItem, "fileConnectMenuItem");
            this.fileConnectMenuItem.Click += new System.EventHandler(this.fileConnectMenuItem_Click);
            // 
            // fileDisconnectMenuItem
            // 
            resources.ApplyResources(this.fileDisconnectMenuItem, "fileDisconnectMenuItem");
            this.fileDisconnectMenuItem.Index = 7;
            this.fileDisconnectMenuItem.MergeOrder = 99;
            this.fileDisconnectMenuItem.Click += new System.EventHandler(this.fileDisconnectMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 8;
            this.menuItem3.MergeOrder = 99;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // fileExitMenuItem
            // 
            this.fileExitMenuItem.Index = 9;
            this.fileExitMenuItem.MergeOrder = 100;
            resources.ApplyResources(this.fileExitMenuItem, "fileExitMenuItem");
            this.fileExitMenuItem.Click += new System.EventHandler(this.fileExitMenuItem_Click);
            // 
            // editMenuItem
            // 
            this.editMenuItem.Index = 1;
            this.editMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem});
            this.editMenuItem.MergeOrder = 1;
            this.editMenuItem.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.editMenuItem, "editMenuItem");
            // 
            // cutMenuItem
            // 
            resources.ApplyResources(this.cutMenuItem, "cutMenuItem");
            this.cutMenuItem.Index = 0;
            // 
            // copyMenuItem
            // 
            resources.ApplyResources(this.copyMenuItem, "copyMenuItem");
            this.copyMenuItem.Index = 1;
            this.copyMenuItem.MergeOrder = 1;
            // 
            // pasteMenuItem
            // 
            resources.ApplyResources(this.pasteMenuItem, "pasteMenuItem");
            this.pasteMenuItem.Index = 2;
            this.pasteMenuItem.MergeOrder = 2;
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.Index = 2;
            this.viewMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.refreshSchemaMenuItem,
            this.menuItem4});
            this.viewMenuItem.MergeOrder = 2;
            this.viewMenuItem.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.viewMenuItem, "viewMenuItem");
            // 
            // refreshSchemaMenuItem
            // 
            resources.ApplyResources(this.refreshSchemaMenuItem, "refreshSchemaMenuItem");
            this.refreshSchemaMenuItem.Index = 0;
            this.refreshSchemaMenuItem.MergeOrder = 1;
            this.refreshSchemaMenuItem.Click += new System.EventHandler(this.refreshSchemaMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            this.menuItem4.MergeOrder = 2;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // toolMenuItem
            // 
            this.toolMenuItem.Index = 3;
            this.toolMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.setupServerIPandPortMenuItem,
            this.setupServerURLMenuItem,
            this.setupDatabaseMenuItem,
            this.createUserRoleMenuItem,
            this.protectSchemaMenuItem,
            this.changePasswordMenuItem,
            this.menuItem1,
            this.backupMenuItem,
            this.restoreMenuItem,
            this.deleteSchemaMenuItem});
            this.toolMenuItem.MergeOrder = 3;
            this.toolMenuItem.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.toolMenuItem, "toolMenuItem");
            // 
            // setupServerIPandPortMenuItem
            // 
            this.setupServerIPandPortMenuItem.Index = 0;
            resources.ApplyResources(this.setupServerIPandPortMenuItem, "setupServerIPandPortMenuItem");
            this.setupServerIPandPortMenuItem.Click += new System.EventHandler(this.setupServerIPandPortMenuItem_Click);
            // 
            // setupServerURLMenuItem
            // 
            this.setupServerURLMenuItem.Index = 1;
            this.setupServerURLMenuItem.MergeOrder = 8;
            resources.ApplyResources(this.setupServerURLMenuItem, "setupServerURLMenuItem");
            this.setupServerURLMenuItem.Click += new System.EventHandler(this.setupServerURLMenuItem_Click);
            // 
            // setupDatabaseMenuItem
            // 
            this.setupDatabaseMenuItem.Index = 2;
            this.setupDatabaseMenuItem.MergeOrder = 9;
            resources.ApplyResources(this.setupDatabaseMenuItem, "setupDatabaseMenuItem");
            this.setupDatabaseMenuItem.Click += new System.EventHandler(this.setupDatabaseMenuItem_Click);
            // 
            // createUserRoleMenuItem
            // 
            this.createUserRoleMenuItem.Index = 3;
            this.createUserRoleMenuItem.MergeOrder = 10;
            resources.ApplyResources(this.createUserRoleMenuItem, "createUserRoleMenuItem");
            this.createUserRoleMenuItem.Click += new System.EventHandler(this.createUserRoleMenuItem_Click);
            // 
            // protectSchemaMenuItem
            // 
            this.protectSchemaMenuItem.Index = 4;
            this.protectSchemaMenuItem.MergeOrder = 11;
            resources.ApplyResources(this.protectSchemaMenuItem, "protectSchemaMenuItem");
            this.protectSchemaMenuItem.Click += new System.EventHandler(this.protectSchemaMenuItem_Click);
            // 
            // changePasswordMenuItem
            // 
            this.changePasswordMenuItem.Index = 5;
            this.changePasswordMenuItem.MergeOrder = 12;
            resources.ApplyResources(this.changePasswordMenuItem, "changePasswordMenuItem");
            this.changePasswordMenuItem.Click += new System.EventHandler(this.changePasswordMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 6;
            this.menuItem1.MergeOrder = 13;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // backupMenuItem
            // 
            this.backupMenuItem.Index = 7;
            this.backupMenuItem.MergeOrder = 14;
            resources.ApplyResources(this.backupMenuItem, "backupMenuItem");
            this.backupMenuItem.Click += new System.EventHandler(this.backupMenuItem_Click);
            // 
            // restoreMenuItem
            // 
            this.restoreMenuItem.Index = 8;
            this.restoreMenuItem.MergeOrder = 15;
            this.restoreMenuItem.MergeType = System.Windows.Forms.MenuMerge.Replace;
            resources.ApplyResources(this.restoreMenuItem, "restoreMenuItem");
            this.restoreMenuItem.Click += new System.EventHandler(this.restoreMenuItem_Click);
            // 
            // deleteSchemaMenuItem
            // 
            this.deleteSchemaMenuItem.Index = 9;
            this.deleteSchemaMenuItem.MergeOrder = 16;
            resources.ApplyResources(this.deleteSchemaMenuItem, "deleteSchemaMenuItem");
            this.deleteSchemaMenuItem.Click += new System.EventHandler(this.deleteSchemaMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 4;
            this.windowMenuItem.MdiList = true;
            this.windowMenuItem.MergeOrder = 4;
            resources.ApplyResources(this.windowMenuItem, "windowMenuItem");
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.Index = 5;
            this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.userManualMenuItem,
            this.aboutMenuItem});
            this.helpMenuItem.MergeOrder = 5;
            resources.ApplyResources(this.helpMenuItem, "helpMenuItem");
            // 
            // userManualMenuItem
            // 
            this.userManualMenuItem.Index = 0;
            resources.ApplyResources(this.userManualMenuItem, "userManualMenuItem");
            this.userManualMenuItem.Click += new System.EventHandler(this.userManualMenuItem_Click);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Index = 1;
            this.aboutMenuItem.MergeOrder = 1;
            resources.ApplyResources(this.aboutMenuItem, "aboutMenuItem");
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // statusBar1
            // 
            resources.ApplyResources(this.statusBar1, "statusBar1");
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusPanel,
            this.selectionPanel,
            this.progressPanel,
            this.userPanel});
            this.statusBar1.ShowPanels = true;
            // 
            // statusPanel
            // 
            this.statusPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            resources.ApplyResources(this.statusPanel, "statusPanel");
            // 
            // selectionPanel
            // 
            this.selectionPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            resources.ApplyResources(this.selectionPanel, "selectionPanel");
            // 
            // progressPanel
            // 
            resources.ApplyResources(this.progressPanel, "progressPanel");
            this.progressPanel.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
            // 
            // userPanel
            // 
            resources.ApplyResources(this.userPanel, "userPanel");
            // 
            // toolBar1
            // 
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.newToolBarButton,
            this.openFileToolBarButton,
            this.openDBSchemaToolBarButton,
            this.openDataViewerBarButton,
            this.separator1,
            this.saveToolBarButton,
            this.saveDBSchemaToolBarButton,
            this.separator2,
            this.cutToolBarButton,
            this.copyToolBarButton,
            this.pasteToolBarButton,
            this.separator3,
            this.addItemToolBarButton,
            this.deleteItemToolBarButton,
            this.modifyItemToolBarButton,
            this.validateToolBarButton,
            this.separator4,
            this.searchToolBarButton,
            this.chartToolBarButton,
            this.exportToolBarButton,
            this.pivotToolBarButton,
            this.separator5,
            this.newInstanceToolBarButton,
            this.saveInstanceAsToolBarButton,
            this.saveInstanceToolBarButton,
            this.deleteInstanceToolBarButton,
            this.separator6,
            this.helpToolBarButton,
            this.exitToolBarButton});
            this.toolBar1.ImageList = this.toolBarImageList;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick_1);
            // 
            // newToolBarButton
            // 
            resources.ApplyResources(this.newToolBarButton, "newToolBarButton");
            this.newToolBarButton.Name = "newToolBarButton";
            // 
            // openFileToolBarButton
            // 
            resources.ApplyResources(this.openFileToolBarButton, "openFileToolBarButton");
            this.openFileToolBarButton.Name = "openFileToolBarButton";
            // 
            // openDBSchemaToolBarButton
            // 
            resources.ApplyResources(this.openDBSchemaToolBarButton, "openDBSchemaToolBarButton");
            this.openDBSchemaToolBarButton.Name = "openDBSchemaToolBarButton";
            // 
            // openDataViewerBarButton
            // 
            resources.ApplyResources(this.openDataViewerBarButton, "openDataViewerBarButton");
            this.openDataViewerBarButton.Name = "openDataViewerBarButton";
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            this.separator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // saveToolBarButton
            // 
            resources.ApplyResources(this.saveToolBarButton, "saveToolBarButton");
            this.saveToolBarButton.Name = "saveToolBarButton";
            // 
            // saveDBSchemaToolBarButton
            // 
            resources.ApplyResources(this.saveDBSchemaToolBarButton, "saveDBSchemaToolBarButton");
            this.saveDBSchemaToolBarButton.Name = "saveDBSchemaToolBarButton";
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            this.separator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // cutToolBarButton
            // 
            resources.ApplyResources(this.cutToolBarButton, "cutToolBarButton");
            this.cutToolBarButton.Name = "cutToolBarButton";
            // 
            // copyToolBarButton
            // 
            resources.ApplyResources(this.copyToolBarButton, "copyToolBarButton");
            this.copyToolBarButton.Name = "copyToolBarButton";
            // 
            // pasteToolBarButton
            // 
            resources.ApplyResources(this.pasteToolBarButton, "pasteToolBarButton");
            this.pasteToolBarButton.Name = "pasteToolBarButton";
            // 
            // separator3
            // 
            this.separator3.Name = "separator3";
            this.separator3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // addItemToolBarButton
            // 
            resources.ApplyResources(this.addItemToolBarButton, "addItemToolBarButton");
            this.addItemToolBarButton.Name = "addItemToolBarButton";
            // 
            // deleteItemToolBarButton
            // 
            resources.ApplyResources(this.deleteItemToolBarButton, "deleteItemToolBarButton");
            this.deleteItemToolBarButton.Name = "deleteItemToolBarButton";
            // 
            // modifyItemToolBarButton
            // 
            resources.ApplyResources(this.modifyItemToolBarButton, "modifyItemToolBarButton");
            this.modifyItemToolBarButton.Name = "modifyItemToolBarButton";
            // 
            // validateToolBarButton
            // 
            resources.ApplyResources(this.validateToolBarButton, "validateToolBarButton");
            this.validateToolBarButton.Name = "validateToolBarButton";
            // 
            // separator4
            // 
            this.separator4.Name = "separator4";
            this.separator4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // searchToolBarButton
            // 
            resources.ApplyResources(this.searchToolBarButton, "searchToolBarButton");
            this.searchToolBarButton.Name = "searchToolBarButton";
            // 
            // chartToolBarButton
            // 
            resources.ApplyResources(this.chartToolBarButton, "chartToolBarButton");
            this.chartToolBarButton.Name = "chartToolBarButton";
            // 
            // exportToolBarButton
            // 
            resources.ApplyResources(this.exportToolBarButton, "exportToolBarButton");
            this.exportToolBarButton.Name = "exportToolBarButton";
            // 
            // pivotToolBarButton
            // 
            resources.ApplyResources(this.pivotToolBarButton, "pivotToolBarButton");
            this.pivotToolBarButton.Name = "pivotToolBarButton";
            // 
            // separator5
            // 
            this.separator5.Name = "separator5";
            this.separator5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // newInstanceToolBarButton
            // 
            resources.ApplyResources(this.newInstanceToolBarButton, "newInstanceToolBarButton");
            this.newInstanceToolBarButton.Name = "newInstanceToolBarButton";
            // 
            // saveInstanceAsToolBarButton
            // 
            resources.ApplyResources(this.saveInstanceAsToolBarButton, "saveInstanceAsToolBarButton");
            this.saveInstanceAsToolBarButton.Name = "saveInstanceAsToolBarButton";
            // 
            // saveInstanceToolBarButton
            // 
            resources.ApplyResources(this.saveInstanceToolBarButton, "saveInstanceToolBarButton");
            this.saveInstanceToolBarButton.Name = "saveInstanceToolBarButton";
            // 
            // deleteInstanceToolBarButton
            // 
            resources.ApplyResources(this.deleteInstanceToolBarButton, "deleteInstanceToolBarButton");
            this.deleteInstanceToolBarButton.Name = "deleteInstanceToolBarButton";
            // 
            // separator6
            // 
            this.separator6.Name = "separator6";
            this.separator6.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // helpToolBarButton
            // 
            resources.ApplyResources(this.helpToolBarButton, "helpToolBarButton");
            this.helpToolBarButton.Name = "helpToolBarButton";
            // 
            // exitToolBarButton
            // 
            resources.ApplyResources(this.exitToolBarButton, "exitToolBarButton");
            this.exitToolBarButton.Name = "exitToolBarButton";
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
            this.toolBarImageList.Images.SetKeyName(7, "");
            this.toolBarImageList.Images.SetKeyName(8, "");
            this.toolBarImageList.Images.SetKeyName(9, "");
            this.toolBarImageList.Images.SetKeyName(10, "");
            this.toolBarImageList.Images.SetKeyName(11, "");
            this.toolBarImageList.Images.SetKeyName(12, "");
            this.toolBarImageList.Images.SetKeyName(13, "");
            this.toolBarImageList.Images.SetKeyName(14, "");
            this.toolBarImageList.Images.SetKeyName(15, "");
            this.toolBarImageList.Images.SetKeyName(16, "");
            this.toolBarImageList.Images.SetKeyName(17, "");
            this.toolBarImageList.Images.SetKeyName(18, "");
            this.toolBarImageList.Images.SetKeyName(19, "");
            this.toolBarImageList.Images.SetKeyName(20, "");
            this.toolBarImageList.Images.SetKeyName(21, "");
            this.toolBarImageList.Images.SetKeyName(22, "PivotGrid.bmp");
            // 
            // DesignStudio
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.statusBar1);
            this.IsMdiContainer = true;
            this.Menu = this.mainMenu1;
            this.Name = "DesignStudio";
            this.Load += new System.EventHandler(this.DesignStudio_Load);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.DesignStudio_Layout);
            ((System.ComponentModel.ISupportInitialize)(this.statusPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectionPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main(string[] args) 
		{
			try 
			{
                string language = (args.Length > 0) ? args[0] : null;
                if (language != null)
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
                }

                /*
                SplashScreen.SetStatus(NewteraNameSpace.RELEASE_VERSION);

                if (Thread.CurrentThread.CurrentUICulture.Name.ToUpper() == "ZH-CN")
                {
                    SplashScreen.ShowSplashScreen();

                    System.Threading.Thread.Sleep(3500);
                }
                */

				Application.Run(new DesignStudio());
			}
            catch (System.AggregateException)
            {
                MessageBox.Show("Unable to connect to server, please make the server is running and try again.");
            }
			catch (Exception e)
			{
				string msg = e.Message;
			}
		}

		#region Controller code

        /// <summary>
        /// Create a web service proxy for chart related services
        /// </summary>
        /// <returns></returns>
        internal static Newtera.DataGridActiveX.ActiveXControlWebService.ActiveXControlService CreateActiveXControlWebService()
        {

            return null;
        }

		/// <summary>
		/// event handler for menu state change event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemStateChanged(object sender, System.EventArgs e)
		{
			StateChangedEventArgs args = (StateChangedEventArgs) e;

			// set the toolbar button states
			switch (args.ID)
			{
                case MenuItemID.FileOpenDatabase:
                    this.openDBSchemaToolBarButton.Enabled = args.State;
                    this.fileOpenDatabaseMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.FileOpenDataViewer:
                    this.openDataViewerBarButton.Enabled = args.State;
                    this.fileOpenDataViewerMenuItem.Enabled = args.State;
                    break;
				case MenuItemID.EditAdd:
					this.addItemToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.EditCopy:
					this.copyToolBarButton.Enabled = args.State;
					this.copyMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditCut:
					this.cutToolBarButton.Enabled = args.State;
					this.cutMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditModify:
					this.modifyItemToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.EditDelete:
					this.deleteItemToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.EditPaste:
					this.pasteToolBarButton.Enabled = args.State;
					this.pasteMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditSearch:
					this.searchToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.ViewChart:
					this.chartToolBarButton.Enabled = args.State;
					break;
                case MenuItemID.ViewPivot:
                    this.pivotToolBarButton.Enabled = args.State;
                    break;
				case MenuItemID.FileExport:
					this.exportToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.EditNewInstance:
					this.newInstanceToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.EditSaveInstance:
					this.saveInstanceToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.EditSaveInstanceAs:
					this.saveInstanceAsToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.EditDeleteInstance:
					this.deleteInstanceToolBarButton.Enabled = args.State;
					break;
				case MenuItemID.FileSaveDatabase:
					this.saveDBSchemaToolBarButton.Enabled = args.State;
					break;
                case MenuItemID.FileSaveFile:
                    this.saveToolBarButton.Enabled = args.State;
                    break;
				case MenuItemID.ToolValidate:
					this.validateToolBarButton.Enabled = args.State;
					break;
                case MenuItemID.ViewRefresh:
                    this.refreshSchemaMenuItem.Enabled = args.State;
                    break;
			}
		}

		private void schemaEditor_Closed(object sender, System.EventArgs e)
		{
			SchemaEditor schemaEditor = (SchemaEditor) sender;

			// schema editor is closed, remove it from the hashtable
			_schemaEditors.Remove(schemaEditor.MetaData.SchemaInfo.NameAndVersion);

			DisconnectDatabase(schemaEditor.MetaData);
		}

		private void dataViewer_Closed(object sender, System.EventArgs e)
		{
			DataViewer dataViewer = (DataViewer) sender;

			// data viewer is closed, remove it from the hashtable
			_dataViewers.Remove(dataViewer.MetaData.SchemaInfo.NameAndVersion);

			DisconnectDatabase(dataViewer.MetaData);
		}

		private SchemaEditor CreateSchemaEditor()
		{
			//
			// Create a MDI child for SchemaEditor
			//
			SchemaEditor schemaEditor = new SchemaEditor();
			schemaEditor.MdiParent = this;
			schemaEditor.IsDBAUser = this.IsDBAUser;
			schemaEditor.WindowState = FormWindowState.Maximized;

			// register the menu item state change deleget
			schemaEditor.MenuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

			schemaEditor.Closed += new EventHandler(this.schemaEditor_Closed);

			return schemaEditor;
		}

		private DataViewer CreateDataViewer()
		{
			//
			// Create a MDI child for DataViewer
			//
			DataViewer dataViewer = new DataViewer();
			dataViewer.MdiParent = this;
			dataViewer.WindowState = FormWindowState.Maximized;
            dataViewer.IsDBAUser = this.IsDBAUser;

			// register the menu item state change deleget
			dataViewer.MenuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

			dataViewer.Closed += new EventHandler(this.dataViewer_Closed);

			return dataViewer;
		}

		private void ExitApplication(object sender, System.EventArgs e)
		{
			if (MessageBox.Show(MessageResourceManager.GetString("DesignStudio.Exit"),
				"Confirm Dialog", MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.Yes)
			{
				this.Close();
				Application.Exit();
			}
		}

		private void CreateNewSchema(object sender, System.EventArgs e)
		{
			SchemaEditor schemaEditor = CreateSchemaEditor();

			schemaEditor.Show();
		}

		private void OpenDataViewer(object sender, System.EventArgs e)
		{
			DataViewer dataViewer = CreateDataViewer();

			dataViewer.Show();
		}

		/// <summary>
		/// The AsyncCallback event handler for GetSchemaModel web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void GetMetaDataDone(string[] xmlStrings)
		{
			try
			{
				this.ShowReadyStatus();
					
				// Create an meta data object to hold the schema model
				MetaDataModel metaData = new MetaDataModel();

                // Note, make sure to set SchemaInfo to meta-data model before
                // loading xml strings
				metaData.SchemaInfo = _selectedSchemaInfo;

				// read mete-data from xml strings
                metaData.Load(xmlStrings);
			
				// save the meta data in a global store
				MetaDataStore.Instance.PutMetaData(metaData);

				// enable the disconnect database menu item
				this.fileDisconnectMenuItem.Enabled = true;

				metaData.NeedToSave = false;

				metaData.IsLockObtained = this._isLockObtained;
				_isLockObtained = false; // reset

				ShowSchemaModel(metaData);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace,
					"Error Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			finally
			{
				//Bring down the work in progress dialog
				HideWorkingDialog();
			}
		}

		private delegate void ShowSchemaModelDelegate(MetaDataModel metaData);

		/// <summary>
		/// Create a schema editor loaded with a schema model
		/// </summary>
		/// <param name="metaData">The meta data represents a schema model</param>
		private void ShowSchemaModel(MetaDataModel metaData)
		{
			if (this.InvokeRequired == false)
			{
                if (PermissionChecker.Instance.HasPermission(metaData.XaclPolicy, metaData, XaclActionType.Read))
				{
					// it is the UI thread, continue
					if (_openningChild == MDIChildType.SchemaEditor)
					{
						SchemaEditor schemaEditor = CreateSchemaEditor();
						schemaEditor.MetaData = metaData;

						// Set IsDBLoaded flag true indicating the meta data is loaded
						// from database
						schemaEditor.IsDBLoaded = true;

						// only one schema editor is created for each database, keep it in a hashtable
						_schemaEditors[metaData.SchemaInfo.NameAndVersion] = schemaEditor;

						// open the schema editor
						schemaEditor.Show();
					}
					else if (_openningChild == MDIChildType.DataViewer)
					{
						DataViewer dataViewer = CreateDataViewer();
						dataViewer.MetaData = metaData;

						// only one data viewer is created for each database, keep it in a hashtable
						_dataViewers[metaData.SchemaInfo.NameAndVersion] = dataViewer;
						
						dataViewer.Show();
					}
					else if (_openningChild == MDIChildType.ImportWizard)
					{
						ImportWizard importWizard = new ImportWizard();

						importWizard.MetaData = metaData;

						importWizard.DesignStudio = this;

						importWizard.IsDBAUser = this.IsDBAUser;

						DialogResult result = importWizard.ShowDialog();
					}

                    // the user has loggin successfully, display the loggin user at status bar
                    DisplayCurrentUser();
				}
				else
				{
					// remove the meta data from the store since the user doesn't
					// have permission to access it.
					MetaDataStore.Instance.RemoveMetaData(metaData.SchemaInfo);

					if (metaData.IsLockObtained)
					{
						// unlock the meta data model on the server
						try
						{
							_metaDataService.UnlockMetaData(ConnectionStringBuilder.Instance.Create(metaData.SchemaInfo), false);
						}
						catch (Exception)
						{
						}
					}

					MessageBox.Show(MessageResourceManager.GetString("DesignStudio.NoDatabasePermission"));
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowSchemaModelDelegate showSchemaModel = new ShowSchemaModelDelegate(ShowSchemaModel);

				this.BeginInvoke(showSchemaModel, new object[] {metaData});
			}
		}

		private void OpenDatabaseMetaData(object sender, System.EventArgs e)
		{
			if (ValidateVersion())
			{
				CheckEvaluationDuration();
					
				OpenSchemaDialog dialog = new OpenSchemaDialog();
				dialog.Owner = this;
				DialogResult result = dialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					IsDBAUser = dialog.IsDBAUser;
					this._isLockObtained = dialog.IsLockObtained;

					if (dialog.SelectedSchema != null)
					{
						_selectedSchemaInfo = new Newtera.Common.Core.SchemaInfo();
						_selectedSchemaInfo.ID = dialog.SelectedSchema.ID;
						_selectedSchemaInfo.Name = dialog.SelectedSchema.Name;
						_selectedSchemaInfo.Version = dialog.SelectedSchema.Version;
							
						_isRequestComplete = false;

                        // Show the status
                        this.ShowWorkingStatus(MessageResourceManager.GetString("DesignStudio.GettingSchema"));

                        // invoke the web service asynchronously
                        string[] xmlStrings = _metaDataService.GetMetaData(ConnectionStringBuilder.Instance.Create(_selectedSchemaInfo));

                        GetMetaDataDone(xmlStrings);
                    }
				}
			}
		}

		/// <summary>
		/// Validate if the design studio and the server has the same version
		/// </summary>
		/// <returns>True if it has same version, false, otherwise.</returns>
		private bool ValidateVersion()
		{
			bool isEqual = true;
			AdminServiceStub adminService = new AdminServiceStub();

			string serverVersion = adminService.GetServerVersion();

			string clientVersion = NewteraNameSpace.RELEASE_VERSION;

            // compare the first two numbers in the version number
            // for example, 3.0.0 and 3.0.1 are considered the same
			int pos = serverVersion.LastIndexOf(".");
			string serverMainVersion = serverVersion.Substring(0, pos);

			pos = clientVersion.LastIndexOf(".");
			string clientMainVersion = clientVersion.Substring(0, pos);

			int result = serverMainVersion.CompareTo(clientMainVersion);
			if (result > 0)
			{
				isEqual = false;
				MessageBox.Show(MessageResourceManager.GetString("DesignStudio.OlderClientVersion") + " " + serverVersion);
			}
			else if (result < 0)
			{
				isEqual = false;
				MessageBox.Show(MessageResourceManager.GetString("DesignStudio.NewerClientVersion") + " " + clientVersion);
			}

			return isEqual;
		}

		/// <summary>
		/// Check how long the evaluation time remains, if the remained evaluation time
		/// is less than 10 days, show a warning message.
		/// </summary>
		private void CheckEvaluationDuration()
		{
			int daysRemained = 0;

			try
			{
				AdminServiceStub adminService = new AdminServiceStub();

				daysRemained = adminService.GetRemainingEvaluationDays();

				// daysRemained is -1, then it is a permenant license
				if (daysRemained >= 0 && daysRemained < 10)
				{
					MessageBox.Show(String.Format(MessageResourceManager.GetString("DesignStudio.RemainedDays"), daysRemained),
						"Warning Dialog", MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
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
		/// Delete a database schema.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteDatabaseMetaData(object sender, System.EventArgs e)
		{
			try
			{
				LoginDialog loginDialog = new LoginDialog();

				if (loginDialog.ShowDialog() == DialogResult.OK)
				{
					SelectSchemaDialog dialog = new SelectSchemaDialog();
					DialogResult result = dialog.ShowDialog();
					if (result == DialogResult.OK)
					{
						if (dialog.SelectedSchema != null)
						{
							Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
							schemaInfo.Name = dialog.SelectedSchema.Name;
							schemaInfo.Version = dialog.SelectedSchema.Version;
                            schemaInfo.ModifiedTime = dialog.SelectedSchema.ModifiedTime;

							_isRequestComplete = false;

							// invoke the web service asynchronously
							_metaDataService.DeleteMetaData(ConnectionStringBuilder.Instance.Create(schemaInfo));

                            this.ShowReadyStatus();

                            MessageBox.Show(MessageResourceManager.GetString("DesignStudio.DeleteDatabaseDone"),
                                "Info",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
					}
				}
			}
			catch(Exception ex)
			{
                string msg = WinClientUtil.GetOriginalMessage(ex.Message);
                MessageBox.Show(msg,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void OpenFileSchema(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.InitialDirectory = "c:\\" ;
			dialog.Filter = "schema files (*.schema)|*.schema"  ;
			dialog.FilterIndex = 1 ;
			dialog.RestoreDirectory = false ;

			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				SchemaEditor schemaEditor = CreateSchemaEditor();
				
				string fileName = dialog.FileName;
				schemaEditor.SchemaFileName = fileName;

				try
				{
					MetaDataModel metaData = new MetaDataModel();
					metaData.SchemaModel.Read(fileName);
					metaData.SchemaModel.IsAltered = true;
					string dataViewFileName = fileName.Replace(".schema", ".dataview");
					metaData.DataViews.Read(dataViewFileName);
					metaData.DataViews.IsAltered = true;
					string taxonomyFileName = fileName.Replace(".schema", ".taxonomy");
					metaData.Taxonomies.Read(taxonomyFileName);
					metaData.Taxonomies.IsAltered = true;
					string xaclFileName = fileName.Replace(".schema", ".xacl");
					metaData.XaclPolicy.Read(xaclFileName);
					metaData.XaclPolicy.IsAltered = true;
					string ruleFileName = fileName.Replace(".schema", ".rules");
					metaData.RuleManager.Read(ruleFileName);
					metaData.RuleManager.IsAltered = true;
					string mappingFileName = fileName.Replace(".schema", ".mappings");
					metaData.MappingManager.Read(mappingFileName);
					metaData.MappingManager.IsAltered = true;
					string selectorFileName = fileName.Replace(".schema", ".selectors");
					if (File.Exists(selectorFileName))
					{
						metaData.SelectorManager.Read(selectorFileName);
						metaData.SelectorManager.IsAltered = true;
					}
                    string eventFileName = fileName.Replace(".schema", ".events");
                    if (File.Exists(eventFileName))
                    {
                        metaData.EventManager.Read(eventFileName);
                        metaData.EventManager.IsAltered = true;
                    }

                    string subscriberFileName = fileName.Replace(".schema", ".subscribers");
                    if (File.Exists(subscriberFileName))
                    {
                        metaData.SubscriberManager.Read(subscriberFileName);
                        metaData.SubscriberManager.IsAltered = true;
                    }

                    string loggingFileName = fileName.Replace(".schema", ".logging");
                    if (File.Exists(loggingFileName))
                    {
                        metaData.LoggingPolicy.Read(loggingFileName);
                        metaData.LoggingPolicy.IsAltered = true;
                    }

                    string xmlSchemaViewFileName = fileName.Replace(".schema", ".schemaviews");
                    if (File.Exists(xmlSchemaViewFileName))
                    {
                        metaData.XMLSchemaViews.Read(xmlSchemaViewFileName);
                        metaData.XMLSchemaViews.IsAltered = true;
                    }

                    string apiFileName = fileName.Replace(".schema", ".apis");
                    if (File.Exists(apiFileName))
                    {
                        metaData.ApiManager.Read(apiFileName);
                        metaData.ApiManager.IsAltered = true;
                    }

                    // clean up any post-data left in the saved schema
                    CleanupSchemaVisitor visitor = new CleanupSchemaVisitor();
					metaData.SchemaModel.Accept(visitor);

					schemaEditor.MetaData = metaData;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}

				// open a schema editor with tree frame
				schemaEditor.Show();
			}		
		}

		private void OpenImportWizard(object sender, System.EventArgs e)
		{
			// Import wizard needs to load Meta Data of a schema to process.
			// Since loading a meta data is a common procedure, it is better to
			// reuse the code. In order to distinguish what to do after the
			// meta data is loaded, here, we decides to use a flag for the purpose.
			_openningChild = MDIChildType.ImportWizard;
			if (MetaDataStore.Instance.CurrentMetaData == null)
			{
				OpenDatabaseMetaData(sender, e);
			}
			else
			{
				MetaDataModel metaData = MetaDataStore.Instance.CurrentMetaData;
				ShowSchemaModel(MetaDataStore.Instance.CurrentMetaData);
			}
		}

		/// <summary>
		/// backup a database data, including metadata, instance data, and attachments
		/// into a zip file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PackDatabaseData(object sender, System.EventArgs e)
		{
			try
			{
				OpenSchemaDialog dialog = new OpenSchemaDialog();
				dialog.Owner = this;
				dialog.LockMetaData = false; // read data only
				DialogResult result = dialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					if (dialog.IsDBAUser)
					{
						if (dialog.SelectedSchema != null)
						{
							Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
							schemaInfo.Name = dialog.SelectedSchema.Name;
							schemaInfo.Version = dialog.SelectedSchema.Version;
                            schemaInfo.ModifiedTime = dialog.SelectedSchema.ModifiedTime;

							string fileName = null;
							SaveFileDialog saveFileDialog = new SaveFileDialog();
	 
							saveFileDialog.InitialDirectory = "c:\\" ;
							saveFileDialog.Filter = "Backup files (*.bkp)|*.bkp"  ;
							saveFileDialog.FilterIndex = 1 ;
							saveFileDialog.RestoreDirectory = false ;
	 
							if (saveFileDialog.ShowDialog() == DialogResult.OK)
							{
								fileName = saveFileDialog.FileName;

								// pack the data of the selected schema into a zip file
                                string dataDir = GetToolTempDir() + DesignStudio.DATA_DIR;

								bool backupAttachments = false;
								if (MessageBox.Show(MessageResourceManager.GetString("DesignStudio.BackupAttachments"),
									"Confirm", MessageBoxButtons.YesNo,
									MessageBoxIcon.Question) == DialogResult.Yes)
								{
									backupAttachments = true;
								}
								else
								{
									backupAttachments = false;
								}

								_packer = new PackData(fileName, schemaInfo, dataDir, backupAttachments);
								_packer.WorkingDialog = _workInProgressDialog;

								_isRequestComplete = false;

								// export and pack is a lengthy job, therefore, run the task
								// on a worker thread so that UI thread won't be blocked.
								_backupMethod = new MethodInvoker(RunBackupJob);

								_backupMethod.BeginInvoke(new AsyncCallback(RunBackupJobDone), null);

								// Show the status
								this.ShowWorkingStatus(MessageResourceManager.GetString("DesignStudio.BackupData"));

								// launch a work in progress dialog
								ShowWorkingDialog(true, new MethodInvoker(CancelBackupJob));
							}
						}
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("DesignStudio.DBARequired"));
					}
				}
			}
			catch(Exception ex)
			{
                string msg = WinClientUtil.GetOriginalMessage(ex.Message);
                MessageBox.Show(msg,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
			}
		}

        /// <summary>
        /// Get the temp dir for saving tempory files
        /// </summary>
        /// <returns></returns>
        private string GetToolTempDir()
        {
            string tempToolDir = NewteraNameSpace.GetAppToolDir();

            tempToolDir += @"temp\";

            if (!Directory.Exists(tempToolDir))
            {
                Directory.CreateDirectory(tempToolDir);
            }

            return tempToolDir;
        }

        /// <summary>
        /// Reload the meta-data model for the current active SchemaEditor or DataViewer
        /// </summary>
        private void ReloadCurrentMetaData()
        {
            MetaDataModel metaData = MetaDataStore.Instance.CurrentMetaData;
            if (metaData != null)
            {
                if (metaData.IsAltered)
                {
                    if (MessageBox.Show(MessageResourceManager.GetString("DesignStudio.RefreshMetaData"),
                        "Confirm", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }

                Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                schemaInfo.ID = metaData.SchemaInfo.ID;
                schemaInfo.Name = metaData.SchemaInfo.Name;
                schemaInfo.Version = metaData.SchemaInfo.Version;

                _isRequestComplete = false;

                // Show the status
                this.ShowWorkingStatus(MessageResourceManager.GetString("DesignStudio.GettingSchema"));

                // invoke the web service asynchronously
                string[] xmlStrings = _metaDataService.GetMetaData(ConnectionStringBuilder.Instance.Create(schemaInfo));

                ReloadMetaDataDone(xmlStrings);
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for ReloadCurrentMetaData web service method.
        /// </summary>
        /// <param name="res">The result</param>
        private void ReloadMetaDataDone(string[] xmlStrings)
        {
            try
            {
                this.ShowReadyStatus();

                MetaDataModel metaData = new MetaDataModel();

                // Note, we need to set the SchemaInfo to the meta-data model before
                // loading from the xml strings
                Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
                schemaInfo.ID = MetaDataStore.Instance.CurrentMetaData.SchemaInfo.ID;
                schemaInfo.Name = MetaDataStore.Instance.CurrentMetaData.SchemaInfo.Name;
                schemaInfo.Version = MetaDataStore.Instance.CurrentMetaData.SchemaInfo.Version;
                metaData.SchemaInfo = schemaInfo;

                // read mete-data from xml strings
                metaData.Load(xmlStrings);

                // reset the meta-data model's modified time
                schemaInfo.ModifiedTime = metaData.SchemaModel.SchemaInfo.ModifiedTime;

                metaData.NeedToSave = false;
                metaData.IsLockObtained = MetaDataStore.Instance.CurrentMetaData.IsLockObtained;

                // Replace the meta data in a meta-data store as the current meta-data
                MetaDataStore.Instance.PutMetaData(metaData);

                RefreshDisplay(metaData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error Dialog", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
            }
        }

        private delegate void RefreshMetaDataModelDelegate(MetaDataModel metaData);

        /// <summary>
        /// Refresh the meta-data display on the schema editor or data viewer
        /// </summary>
        /// <param name="metaData">The new meta data model</param>
        private void RefreshDisplay(MetaDataModel metaData)
        {
            if (this.InvokeRequired == false)
            {
                SchemaEditor schemaEditor = (SchemaEditor)_schemaEditors[metaData.SchemaInfo.NameAndVersion];
                if (schemaEditor != null)
                {
                    schemaEditor.MetaData = metaData;
                    schemaEditor.RefreshMetaDataTree();
                }

                DataViewer dataViewer = (DataViewer)_dataViewers[metaData.SchemaInfo.NameAndVersion];
                if (dataViewer != null)
                {
                    dataViewer.MetaData = metaData;
                    dataViewer.RefreshMetaDataTree();
                }
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                RefreshMetaDataModelDelegate refreshMetaData = new RefreshMetaDataModelDelegate(RefreshDisplay);

                this.BeginInvoke(refreshMetaData, new object[] { metaData });
            }
        }

		/// <summary>
		/// Run the backup job in a worker thread
		/// </summary>
		private void RunBackupJob()
		{
			if (_packer != null)
			{
				_packer.Pack();
			}
		}

		/// <summary>
		/// Cancel the backup job in a worker thread
		/// </summary>
		private void CancelBackupJob()
		{
			if (_packer != null)
			{
				_packer.IsCancelled = true;
			}
		}

		/// <summary>
		/// The AsyncCallback event handler for RunBackupJob method.
		/// </summary>
		/// <param name="res">The result</param>
		private void RunBackupJobDone(IAsyncResult res)
		{
			try
			{
				this.ShowReadyStatus();

				_backupMethod.EndInvoke(res);

				MessageBox.Show(MessageResourceManager.GetString("DesignStudio.BackupComplete"));

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

		/// <summary>
		/// Restore a database from a zipped backup file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RestoreDatabaseData(object sender, System.EventArgs e)
		{
			try
			{
				LoginDialog loginDialog = new LoginDialog();

				if (loginDialog.ShowDialog() == DialogResult.OK)
				{
					string fileName = null;
					OpenFileDialog openFileDialog = new OpenFileDialog();

					openFileDialog.InitialDirectory = "c:\\" ;
					openFileDialog.Filter = "Backup files (*.bkp)|*.bkp"  ;
					openFileDialog.FilterIndex = 1 ;
					openFileDialog.RestoreDirectory = false ;

					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						fileName = openFileDialog.FileName;

						// pack the data of the selected schema into a zip file
                        string dataDir = GetToolTempDir() + DesignStudio.DATA_DIR;

						_unpacker = new UnpackData(fileName, dataDir);
						_unpacker.ConfirmCallback = new MethodInvoker(ConfirmOverrideDatabase);
						_unpacker.WorkingDialog = this._workInProgressDialog;

						_isRequestComplete = false;

						// unpack and import database data is a lengthy job, therefore, run the task
						// on a worker thread so that UI thread won't be blocked.
						_restoreMethod = new MethodInvoker(RunRestoreJob);

						_restoreMethod.BeginInvoke(new AsyncCallback(RunRestoreJobDone), null);

						// Show the status
						this.ShowWorkingStatus(MessageResourceManager.GetString("DesignStudio.RestoreData"));

						// launch a work in progress dialog
						ShowWorkingDialog(false, null);
					}
				}
			}
			catch(Exception ex)
			{
                string msg = WinClientUtil.GetOriginalMessage(ex.Message);
                MessageBox.Show(msg,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Gets the information indicating whether the server's license is an
		/// evaluation license.
		/// </summary>
		/// <returns>True if it is an evaluation license, false if it is a permenant license.</returns>
		private bool IsInEvaluation()
		{
			bool isEvaluation = true;

			try
			{
				AdminServiceStub adminService = new AdminServiceStub();

				isEvaluation = adminService.IsEvaluationLicense();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}

			return isEvaluation;
		}

		/// <summary>
		/// The AsyncCallback event handler for RunRestoreJob method.
		/// </summary>
		/// <param name="res">The result</param>
		private void RunRestoreJobDone(IAsyncResult res)
		{
			try
			{
				this.ShowReadyStatus();

				_restoreMethod.EndInvoke(res);

				MessageBox.Show(MessageResourceManager.GetString("DesignStudio.RestoreComplete"));

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

		/// <summary>
		/// Run the restore database job in a worker thread
		/// </summary>
		private void RunRestoreJob()
		{
			if (_unpacker != null)
			{
				_unpacker.Restore();
			}
		}

		/// <summary>
		/// Confirm with user whether to override an existing database
		/// </summary>
		internal void ConfirmOverrideDatabase()
		{
			if (MessageBox.Show(MessageResourceManager.GetString("DesignStudio.OverrideDatabase"),
				"Confirm Dialog", MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_unpacker.IsOverride = true;
			}
			else
			{
				_unpacker.IsOverride = false;
			}
		}

		/// <summary>
		/// Refresh the MDI children, such as SchemaEditor and DataViewers, that
		/// are associated with the given schema, so that their display are up to date
		/// with the metadata changes
		/// </summary>
		/// <param name="schemaNameAndVersion">The combination of a schema name and version.</param>
		internal void RefreshMDIChildMetaDataTree(string schemaNameAndVersion)
		{
            SchemaEditor schemaEditor = (SchemaEditor)_schemaEditors[schemaNameAndVersion];

			if (schemaEditor != null)
			{
				schemaEditor.RefreshMetaDataTree();
			}

            DataViewer dataViewer = (DataViewer)_dataViewers[schemaNameAndVersion];

			if (dataViewer != null)
			{
				dataViewer.RefreshMetaDataTree();
			}
		}

		#endregion

		private void fileNewMenuItem_Click(object sender, System.EventArgs e)
		{
			CreateNewSchema(sender, e);
		}

		private void fileCloseMenuItem_Click(object sender, System.EventArgs e)
		{
			Form activeMdiChild = this.ActiveMdiChild;

			if (activeMdiChild != null)
			{
				activeMdiChild.Close();
			}
		}

		private void fileOpenDatabaseMenuItem_Click(object sender, System.EventArgs e)
		{
			_openningChild = MDIChildType.SchemaEditor;
			if (MetaDataStore.Instance.CurrentMetaData == null)
			{
				OpenDatabaseMetaData(sender, e);
			}
			else
			{
				// Only one schema editor is allowed for each database.
				// Therefore, first find it from the hashtable
				MetaDataModel metaData = MetaDataStore.Instance.CurrentMetaData;
				SchemaEditor schemaEditor = (SchemaEditor) _schemaEditors[metaData.SchemaInfo.NameAndVersion];
				if (schemaEditor == null)
				{
					ShowSchemaModel(MetaDataStore.Instance.CurrentMetaData);
				}
				else
				{
					schemaEditor.Activate();
				}
			}
		}

		private void fileOpenFileMenuItem_Click(object sender, System.EventArgs e)
		{
			OpenFileSchema(sender, e);
		}

		private void fileOpenDataViewerMenuItem_Click_1(object sender, System.EventArgs e)
		{
			_openningChild = MDIChildType.DataViewer;
			if (MetaDataStore.Instance.CurrentMetaData == null)
			{
				OpenDatabaseMetaData(sender, e);
			}
			else
			{
				// Only one data viewer is allowed for each database.
				// Therefore, first find it from the hashtable
				MetaDataModel metaData = MetaDataStore.Instance.CurrentMetaData;
				DataViewer dataViewer = (DataViewer) _dataViewers[metaData.SchemaInfo.NameAndVersion];
				if (dataViewer == null)
				{
					ShowSchemaModel(MetaDataStore.Instance.CurrentMetaData);
				}
				else
				{
					dataViewer.Activate();
				}
			}		
		}

		private void fileExitMenuItem_Click(object sender, System.EventArgs e)
		{
			ExitApplication(sender, e);
		}

		private void toolBar1_ButtonClick_1(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			Form activeMdiChild = this.ActiveMdiChild;

			if (e.Button == this.newToolBarButton)
			{
				CreateNewSchema(sender, e);
			}
			else if (e.Button == this.openFileToolBarButton)
			{
				OpenFileSchema(sender, e);
			}
			else if (e.Button == this.saveToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is SchemaEditor)
				{
					((SchemaEditor) activeMdiChild).SaveMetaDataToFile(sender, e);
				}
			}
			else if (e.Button == this.openDBSchemaToolBarButton)
			{
				this._openningChild = MDIChildType.SchemaEditor;
				if (MetaDataStore.Instance.CurrentMetaData == null)
				{
					OpenDatabaseMetaData(sender, e);
				}
				else
				{
					// Only one schema editor is allowed for each database.
					// Therefore, first find it from the hashtable
					MetaDataModel metaData = MetaDataStore.Instance.CurrentMetaData;
					SchemaEditor schemaEditor = (SchemaEditor) _schemaEditors[metaData.SchemaInfo.NameAndVersion];
					if (schemaEditor == null)
					{
						ShowSchemaModel(MetaDataStore.Instance.CurrentMetaData);
					}
					else
					{
						schemaEditor.Activate();
					}
				}
			}
			else if (e.Button == this.openDataViewerBarButton)
			{
				this._openningChild = MDIChildType.DataViewer;
				if (MetaDataStore.Instance.CurrentMetaData == null)
				{
					OpenDatabaseMetaData(sender, e);
				}
				else
				{
					// Only one data viewer is allowed for each database.
					// Therefore, first find it from the hashtable
					MetaDataModel metaData = MetaDataStore.Instance.CurrentMetaData;
					DataViewer dataViewer = (DataViewer) _dataViewers[metaData.SchemaInfo.NameAndVersion];
					if (dataViewer == null)
					{
						ShowSchemaModel(MetaDataStore.Instance.CurrentMetaData);
					}
					else
					{
						dataViewer.Activate();
					}
				}
			}
			else if (e.Button == this.searchToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is DataViewer)
				{
					((DataViewer) activeMdiChild).ShowSearchDialog(sender, e);
				}
			}
			else if (e.Button == this.chartToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is DataViewer)
				{
					((DataViewer) activeMdiChild).LaunchChartWizard();
				}
			}
			else if (e.Button == this.exportToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is DataViewer)
				{
					((DataViewer) activeMdiChild).ExportData();
				}
			}
            else if (e.Button == this.pivotToolBarButton)
            {
                if (activeMdiChild != null && activeMdiChild is DataViewer)
                {
                }
            }
			else if (e.Button == this.newInstanceToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is DataViewer)
				{
					((DataViewer) activeMdiChild).ShowNewInstanceDialog();
				}
			}
			else if (e.Button == this.saveInstanceToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is DataViewer)
				{
					((DataViewer) activeMdiChild).SaveInstance();
				}
			}
			else if (e.Button == this.saveInstanceAsToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is DataViewer)
				{
					((DataViewer) activeMdiChild).ShowNewInstanceAsDialog();
				}
			}
			else if (e.Button == this.deleteInstanceToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is DataViewer)
				{
					((DataViewer) activeMdiChild).DeleteInstances();
				}
			}
			else if (e.Button == this.saveDBSchemaToolBarButton)
			{
				if (activeMdiChild != null)
				{
					if (activeMdiChild is SchemaEditor)
					{
						((SchemaEditor) activeMdiChild).SaveMetaDataToDatabase(sender, e);
					}
					else if (activeMdiChild is DataViewer)
					{
						((DataViewer) activeMdiChild).SaveDataViewsToDatabase();
					}
				}
			}
			else if (e.Button == this.validateToolBarButton)
			{
				if (activeMdiChild != null && activeMdiChild is SchemaEditor)
				{
					((SchemaEditor) activeMdiChild).ValidateMetaModel();
				}
			}
			else if (e.Button == this.cutToolBarButton)
			{
			}
			else if (e.Button == this.copyToolBarButton)
			{
			}
			else if (e.Button == this.pasteToolBarButton)
			{
			}
			else if (e.Button == this.addItemToolBarButton)
			{
				if (activeMdiChild != null)
				{
					if (activeMdiChild is SchemaEditor)
					{
						((SchemaEditor) activeMdiChild).AddNewItem(sender, e);
					}
					else if (activeMdiChild is DataViewer)
					{
						((DataViewer) activeMdiChild).ShowDataViewDialog(null);
					}
				}
			}
			else if (e.Button == this.deleteItemToolBarButton)
			{
				if (activeMdiChild != null)
				{
					if (activeMdiChild is SchemaEditor)
					{
						((SchemaEditor) activeMdiChild).DeleteItem(sender, e);
					}
					else if (activeMdiChild is DataViewer)
					{
						((DataViewer) activeMdiChild).DeleteDataView(sender, e);
					}
				}
			}
			else if (e.Button == this.modifyItemToolBarButton)
			{
				if (activeMdiChild != null)
				{
					if (activeMdiChild is DataViewer)
					{
						((DataViewer) activeMdiChild).ShowModifyDataViewDialog();
					}
					else if (activeMdiChild is SchemaEditor)
					{
						((SchemaEditor) activeMdiChild).ModifyItem(sender, e);
					}
				}
			}
			else if (e.Button == this.helpToolBarButton)
			{
				string helpFile = AppDomain.CurrentDomain.BaseDirectory + MessageResourceManager.GetString("DesignStudio.OnlineHelp");

				Help.ShowHelp(this, helpFile);
			}
			else if (e.Button == this.exitToolBarButton)
			{
				ExitApplication(sender, e);
			}
		}

		private void deleteSchemaMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteDatabaseMetaData(sender, e);
		}

		private void changePasswordMenuItem_Click(object sender, System.EventArgs e)
		{
			if (!IsInEvaluation())
			{
				ChangePasswordDialog dialog = new ChangePasswordDialog();
				dialog.ShowDialog();
			}
			else
			{
				MessageBox.Show(MessageResourceManager.GetString("DesignStudio.DisabledFeature"),
					"Error Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void setupDatabaseMenuItem_Click(object sender, System.EventArgs e)
		{
            AdminServiceStub adminService = new AdminServiceStub();

            bool isStandardLicense = adminService.IsStandardLicense();
            if (isStandardLicense)
            {
                DBSetupWizard wizard = new DBSetupWizard();

                wizard.Launch();
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("DesignStudio.AdvancedFeature"),
                    "Error Dialog", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
		}

		private void setupServerURLMenuItem_Click(object sender, System.EventArgs e)
		{
			SetupServerURLDialog dialog = new SetupServerURLDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(MessageResourceManager.GetString("DesignStudio.CorrectURL"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
		}

		private void createUserRoleMenuItem_Click(object sender, System.EventArgs e)
		{
			LoginDialog loginDialog = new LoginDialog();

			if (loginDialog.ShowDialog() == DialogResult.OK)
			{
				bool isReadOnly = false;
				WindowClientUserManager userManager = new WindowClientUserManager();
				isReadOnly = userManager.IsReadOnly;

				CreateUserOrRoleDialog userDialog = new CreateUserOrRoleDialog();

				// set IsReadOnly property according to the user manager
				userDialog.IsReadOnly = isReadOnly;

				userDialog.ShowDialog();
			}
		}

		private void fileImportMenuItem_Click(object sender, System.EventArgs e)
		{
			OpenImportWizard(sender, e);
		}

		private void fileConnectMenuItem_Click(object sender, System.EventArgs e)
		{
			// open a new database connection and load the meta data into
			// a new schema editor
			_openningChild = MDIChildType.SchemaEditor;
			OpenDatabaseMetaData(sender, e);
		}

		private void fileDisconnectMenuItem_Click(object sender, System.EventArgs e)
		{
			DisconnectDatabasesDialog dialog = new DisconnectDatabasesDialog();

			dialog.SchemaInfos = MetaDataStore.Instance.ConnectedSchemas;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.SelectedSchema != null)
				{
					// close the child windows that are connected to the given
					// database indicated by the schema
					foreach (Form mdiChild in this.MdiChildren)
					{
						if (mdiChild is SchemaEditor)
						{
							SchemaEditor schemaEditor = (SchemaEditor) mdiChild;
							if (schemaEditor.MetaData.SchemaInfo.Name == dialog.SelectedSchema.Name &&
								schemaEditor.MetaData.SchemaInfo.Version == dialog.SelectedSchema.Version)
							{
								schemaEditor.Close();
							}
						}
						else if (mdiChild is DataViewer)
						{
							DataViewer dataViewer = (DataViewer) mdiChild;
							if (dataViewer.MetaData.SchemaInfo.Name == dialog.SelectedSchema.Name &&
								dataViewer.MetaData.SchemaInfo.Version == dialog.SelectedSchema.Version)
							{
								dataViewer.Close();
							}
						}
					}

					// remove the meta data from the store
					MetaDataStore.Instance.RemoveMetaData(dialog.SelectedSchema);
				}

				// disable the disconnect database menu item if there is no more
				// currently connected databases
				if (dialog.SchemaInfos.Length == 1)
				{
					this.fileDisconnectMenuItem.Enabled = false;
				}
			}
		}

		private void aboutMenuItem_Click(object sender, System.EventArgs e)
		{
			AboutDialog aboutDialog = new AboutDialog();

			aboutDialog.ShowDialog();
		}

		private void userManualMenuItem_Click(object sender, System.EventArgs e)
		{
			string helpFile = AppDomain.CurrentDomain.BaseDirectory + MessageResourceManager.GetString("DesignStudio.OnlineHelp");
			Help.ShowHelp(this, helpFile);
		}

		private void backupMenuItem_Click(object sender, System.EventArgs e)
		{
			PackDatabaseData(sender, e);
		}

		private void restoreMenuItem_Click(object sender, System.EventArgs e)
		{
			RestoreDatabaseData(sender, e);
		}

		private void protectSchemaMenuItem_Click(object sender, System.EventArgs e)
		{
			if (!IsInEvaluation())
			{
				LoginDialog loginDialog = new LoginDialog();

				if (loginDialog.ShowDialog() == DialogResult.OK)
				{
					ProtectDatabaseDialog dialog = new ProtectDatabaseDialog();

					dialog.ShowDialog();
				}
			}
			else
			{
				MessageBox.Show(MessageResourceManager.GetString("DesignStudio.DisabledFeature"),
					"Error Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void DesignStudio_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			if( _layoutCalled == false )
			{
				_layoutCalled = true;
				if( SplashScreen.SplashForm != null )
					SplashScreen.SplashForm.Owner = this;
				this.Activate();
				SplashScreen.CloseForm();
			}
		}

		private void DesignStudio_Load(object sender, System.EventArgs e)
		{
			// disable the "Setup Server..." menu item if the Design Studio
			// is running on the client side
			string homeDir = NewteraNameSpace.GetAppHomeDir();
			if (string.IsNullOrEmpty(homeDir))
			{
				// only the server installation has HOME_DIR defined,
				// so it is the client side installation
				this.setupDatabaseMenuItem.Enabled = false;
			}
		}

        private void refreshSchemaMenuItem_Click(object sender, EventArgs e)
        {
            ReloadCurrentMetaData();
        }

        private void setupServerIPandPortMenuItem_Click(object sender, EventArgs e)
        {
            SetupServerIPandPortDialog dialog = new SetupServerIPandPortDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string displayMsg = Newtera.WindowsControl.MessageResourceManager.GetString("DesignStudio.SetServerUrlDone");
                MessageBox.Show(displayMsg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    internal enum MDIChildType
	{
		SchemaEditor,
		DataViewer,
		ImportWizard
	}
}

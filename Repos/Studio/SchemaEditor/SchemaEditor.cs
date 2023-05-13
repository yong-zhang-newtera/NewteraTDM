using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

using Newtera.WinClientCommon;
using Newtera.WindowsControl;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Schema.Validate;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Validate;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.Mappings;
using Newtera.Common.MetaData.Rules;
using Newtera.Common.MetaData.Events;
using Newtera.Common.MetaData.Logging;
using Newtera.Common.MetaData.Subscribers;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Common.MetaData.Api;
using Newtera.Common.MetaData.Principal;
using Newtera.DataGridActiveX;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for SchemaEditor.
	/// </summary>
	public class SchemaEditor : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private string _schemaFileName = null;
		private bool _isDBLoaded = false;
		private MetaDataListViewBuilder _listViewBuilder = null;
		private MetaDataTreeBuilder _treeBuilder;
		private MenuItemStates _menuItemStates;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private bool _isDBAUser;
        private MethodInvoker _createHierarchyMethod;
        private HierarchyGenerationUtil _util;
        private bool _isCancelled;
        private string _errorMessage;
        private CMDataServiceStub _dataService;
        private DataViewElementCollection _autoClassifyingNodes;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ImageList treeViewImageList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ImageList listViewLargeImageList;
		private System.Windows.Forms.ImageList listViewSmallImageList;
		private System.Windows.Forms.MenuItem viewLargeIconMenuItem;
		private System.Windows.Forms.MenuItem viewSmallIcomMenuItem;
		private System.Windows.Forms.MenuItem viewListMenuItem;
		private System.Windows.Forms.MenuItem viewDetailMenuItem;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.ContextMenu propertyContextMenu;
		private System.Windows.Forms.MenuItem Reset;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem description;
		private System.Windows.Forms.ContextMenu treeViewContextMenu;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.ContextMenu listViewContextMenu;
		private System.Windows.Forms.MenuItem tvCollapseMenuItem;
		private System.Windows.Forms.MenuItem tvExpandMenuItem;
		private System.Windows.Forms.MenuItem tvDeleteMenuItem;
		private System.Windows.Forms.MenuItem tvAddMenuItem;
		private System.Windows.Forms.MenuItem lvAddMenuItem;
		private System.Windows.Forms.MenuItem lvDeleteMenuItem;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem lvLargeIconMenuItem;
		private System.Windows.Forms.MenuItem lvListMenuItem;
		private System.Windows.Forms.MenuItem lvDetailMenuItem;
		private System.Windows.Forms.MenuItem saveMenuItem;
		private System.Windows.Forms.MenuItem saveAsMenuItem;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem addMenuItem;
		private System.Windows.Forms.MenuItem deleteMenuItem;
		private System.Windows.Forms.MenuItem validateMenuItem;
		private System.Windows.Forms.MenuItem lvSmallIconMenuItem;
		private System.Windows.Forms.MenuItem saveToDatabaseMenuItem;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem updateLogMenuItem;
		private System.Windows.Forms.MenuItem saveToDatabaseAsMenuItem;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem optionsMenuItem;
		private System.Windows.Forms.MenuItem aclOptionMenuItem;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button addRuleButton;
		private System.Windows.Forms.Button removeRuleButton;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton grantReadRadioButton;
		private System.Windows.Forms.RadioButton denyReadRadioButton;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton denyWriteRadioButton;
		private System.Windows.Forms.RadioButton grantWriteRadioButton;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.RadioButton denyCreateRadioButton;
		private System.Windows.Forms.RadioButton grantCreateRadioButton;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.RadioButton denyDeleteRadioButton;
		private System.Windows.Forms.RadioButton grantDeleteRadioButton;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox conditionTextBox;
		private System.Windows.Forms.ListView ruleListView;
		private System.Windows.Forms.CheckBox allowPropagateCheckBox;
		private System.Windows.Forms.GroupBox permissionGroupBox;
		private System.Windows.Forms.MenuItem lvModifyMenuItem;
		private System.Windows.Forms.MenuItem modifyMenuItem;
		private System.Windows.Forms.MenuItem tvModifyMenuItem;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ListView validateRuleListView;
		private System.Windows.Forms.Button addValidateRuleBtn;
		private System.Windows.Forms.Button modifyValidateRuleBtn;
		private System.Windows.Forms.Button removeValidateRuleBtn;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem fixDBMenuItem;
		private System.Windows.Forms.MenuItem tvArrangeMenuItem;
		private System.Windows.Forms.MenuItem lvArrangeMenuItem;
		private System.Windows.Forms.MenuItem arrangeMenuItem;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.RadioButton denyDownloadRadioButton;
		private System.Windows.Forms.RadioButton grantDownloadRadioButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.RadioButton denyUploadRadioButton;
		private System.Windows.Forms.RadioButton grantUploadRadioButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.MenuItem unlockMenuItem;
		private System.Windows.Forms.MenuItem lockMenuItem;
        private TabPage eventTabPage;
        private ListView eventListView;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private PropertyGrid eventPropertyGrid;
        private Button deleteEventButton;
        private Button addEventButton;
        private ColumnHeader columnHeader10;
        private MenuItem tvUpdateMenuItem;
        private MenuItem tvDefineMenuItem;
        private MenuItem updateMenuItem;
        private MenuItem defineMenuItem;
        private MenuItem lvUpdateMenuItem;
        private MenuItem lvDefineMenuItem;
        private MenuItem menuItem13;
        private MenuItem menuItem12;
        private MenuItem menuItem15;
        private TabPage loggingPage;
        private ListView loggingRuleListView;
        private ColumnHeader columnHeader11;
        private ColumnHeader columnHeader12;
        private GroupBox logStatusGroupBox;
        private Panel panel7;
        private RadioButton uploadLogOffRadioButton;
        private RadioButton uploadLogOnRadioButton;
        private Label label3;
        private Panel panel8;
        private RadioButton downloadLogOffRadioButton;
        private RadioButton downloadLogOnRadioButton;
        private Label label11;
        private Label label12;
        private Label label13;
        private Panel panel9;
        private RadioButton readLogOffRadioButton;
        private RadioButton readLogOnRadioButton;
        private Label label14;
        private Panel panel10;
        private RadioButton modifyLogOffRadioButton;
        private RadioButton modifyLogOnRadioButton;
        private Label label15;
        private Panel panel11;
        private RadioButton createLogOffRadioButton;
        private RadioButton createLogOnRadioButton;
        private Label label16;
        private Panel panel12;
        private RadioButton deleteLogOffRadioButton;
        private RadioButton deleteLogOnRadioButton;
        private Label label17;
        private CheckBox allowLogingRulePassDownCheckBox;
        private Button removeLoggingRuleButton;
        private Button addLoggingRuleButton;
        private Panel panel14;
        private RadioButton exportLogOffRadioButton;
        private RadioButton exportLogOnRadioButton;
        private Label label19;
        private Panel panel13;
        private RadioButton importLogOffRadioButton;
        private RadioButton importLogOnRadioButton;
        private Label label18;
        private MenuItem loggingOptionMenuItem;
        private TabPage subscriberPage;
        private ListView subscribersListView;
        private ColumnHeader columnHeader13;
        private ColumnHeader columnHeader14;
        private PrintPreviewDialog printPreviewDialog1;
        private Button addSubscriberButton;
        private PropertyGrid subscriberPropertyGrid;
        private Button delSubscriberButton;
        private MenuItem menuItem16;
        private MenuItem tvExportMenuItem;
        private TabPage apiPage;
        private PropertyGrid apiPropertyGrid;
        private Button deleteApiButton;
        private ListView apiListView;
        private ColumnHeader columnHeader15;
        private ColumnHeader columnHeader16;
        private Button addCustomButton;
        private System.ComponentModel.IContainer components;

		public SchemaEditor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_treeBuilder = new MetaDataTreeBuilder();
			_listViewBuilder = new MetaDataListViewBuilder();
			_menuItemStates = new MenuItemStates();
			_workInProgressDialog = new WorkInProgressDialog();
            _dataService = new CMDataServiceStub();
			_isDBAUser = true;
		}

		~SchemaEditor()
		{
			_workInProgressDialog.Dispose();
		}

		/// <summary>
		/// Gets or sets the meta data being edited by the Editor
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

                if (_metaData != null)
                {
                    _metaData.SchemaModel.CategoryChanged += new EventHandler(this.CategoryChangedHandler);
                    _metaData.SchemaModel.ValueChanged += new EventHandler(this.ValueChangedHandler);
                }
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
				return _isDBAUser;
			}
			set
			{
				_isDBAUser = value;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemaEditor));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.saveMenuItem = new System.Windows.Forms.MenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.MenuItem();
            this.saveToDatabaseMenuItem = new System.Windows.Forms.MenuItem();
            this.saveToDatabaseAsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.addMenuItem = new System.Windows.Forms.MenuItem();
            this.modifyMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteMenuItem = new System.Windows.Forms.MenuItem();
            this.arrangeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.updateMenuItem = new System.Windows.Forms.MenuItem();
            this.defineMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.viewLargeIconMenuItem = new System.Windows.Forms.MenuItem();
            this.viewSmallIcomMenuItem = new System.Windows.Forms.MenuItem();
            this.viewListMenuItem = new System.Windows.Forms.MenuItem();
            this.viewDetailMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.updateLogMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.validateMenuItem = new System.Windows.Forms.MenuItem();
            this.lockMenuItem = new System.Windows.Forms.MenuItem();
            this.unlockMenuItem = new System.Windows.Forms.MenuItem();
            this.fixDBMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.optionsMenuItem = new System.Windows.Forms.MenuItem();
            this.aclOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.loggingOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeViewContextMenu = new System.Windows.Forms.ContextMenu();
            this.tvCollapseMenuItem = new System.Windows.Forms.MenuItem();
            this.tvExpandMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.tvAddMenuItem = new System.Windows.Forms.MenuItem();
            this.tvModifyMenuItem = new System.Windows.Forms.MenuItem();
            this.tvDeleteMenuItem = new System.Windows.Forms.MenuItem();
            this.tvArrangeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.tvUpdateMenuItem = new System.Windows.Forms.MenuItem();
            this.tvDefineMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem16 = new System.Windows.Forms.MenuItem();
            this.tvExportMenuItem = new System.Windows.Forms.MenuItem();
            this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewContextMenu = new System.Windows.Forms.ContextMenu();
            this.lvAddMenuItem = new System.Windows.Forms.MenuItem();
            this.lvModifyMenuItem = new System.Windows.Forms.MenuItem();
            this.lvDeleteMenuItem = new System.Windows.Forms.MenuItem();
            this.lvArrangeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.lvUpdateMenuItem = new System.Windows.Forms.MenuItem();
            this.lvDefineMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.lvLargeIconMenuItem = new System.Windows.Forms.MenuItem();
            this.lvSmallIconMenuItem = new System.Windows.Forms.MenuItem();
            this.lvListMenuItem = new System.Windows.Forms.MenuItem();
            this.lvDetailMenuItem = new System.Windows.Forms.MenuItem();
            this.listViewLargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.listViewSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.propertyContextMenu = new System.Windows.Forms.ContextMenu();
            this.Reset = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.description = new System.Windows.Forms.MenuItem();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.permissionGroupBox = new System.Windows.Forms.GroupBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.denyUploadRadioButton = new System.Windows.Forms.RadioButton();
            this.grantUploadRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.denyDownloadRadioButton = new System.Windows.Forms.RadioButton();
            this.grantDownloadRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.denyReadRadioButton = new System.Windows.Forms.RadioButton();
            this.grantReadRadioButton = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.denyWriteRadioButton = new System.Windows.Forms.RadioButton();
            this.grantWriteRadioButton = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.denyCreateRadioButton = new System.Windows.Forms.RadioButton();
            this.grantCreateRadioButton = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.denyDeleteRadioButton = new System.Windows.Forms.RadioButton();
            this.grantDeleteRadioButton = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.conditionTextBox = new System.Windows.Forms.TextBox();
            this.allowPropagateCheckBox = new System.Windows.Forms.CheckBox();
            this.removeRuleButton = new System.Windows.Forms.Button();
            this.addRuleButton = new System.Windows.Forms.Button();
            this.ruleListView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.apiPage = new System.Windows.Forms.TabPage();
            this.addCustomButton = new System.Windows.Forms.Button();
            this.apiPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.deleteApiButton = new System.Windows.Forms.Button();
            this.apiListView = new System.Windows.Forms.ListView();
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.modifyValidateRuleBtn = new System.Windows.Forms.Button();
            this.removeValidateRuleBtn = new System.Windows.Forms.Button();
            this.addValidateRuleBtn = new System.Windows.Forms.Button();
            this.validateRuleListView = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eventTabPage = new System.Windows.Forms.TabPage();
            this.eventPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.deleteEventButton = new System.Windows.Forms.Button();
            this.addEventButton = new System.Windows.Forms.Button();
            this.eventListView = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loggingPage = new System.Windows.Forms.TabPage();
            this.logStatusGroupBox = new System.Windows.Forms.GroupBox();
            this.panel14 = new System.Windows.Forms.Panel();
            this.exportLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.exportLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label19 = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.importLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.importLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label18 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.uploadLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.uploadLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.downloadLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.downloadLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.readLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.readLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label14 = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.modifyLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.modifyLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.createLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.createLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.deleteLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.deleteLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label17 = new System.Windows.Forms.Label();
            this.allowLogingRulePassDownCheckBox = new System.Windows.Forms.CheckBox();
            this.removeLoggingRuleButton = new System.Windows.Forms.Button();
            this.addLoggingRuleButton = new System.Windows.Forms.Button();
            this.loggingRuleListView = new System.Windows.Forms.ListView();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.subscriberPage = new System.Windows.Forms.TabPage();
            this.delSubscriberButton = new System.Windows.Forms.Button();
            this.addSubscriberButton = new System.Windows.Forms.Button();
            this.subscriberPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.subscribersListView = new System.Windows.Forms.ListView();
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.permissionGroupBox.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.apiPage.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.eventTabPage.SuspendLayout();
            this.loggingPage.SuspendLayout();
            this.logStatusGroupBox.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel12.SuspendLayout();
            this.subscriberPage.SuspendLayout();
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
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.saveToDatabaseMenuItem,
            this.saveToDatabaseAsMenuItem,
            this.menuItem11});
            this.menuItem1.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Index = 0;
            this.saveMenuItem.MergeOrder = 10;
            resources.ApplyResources(this.saveMenuItem, "saveMenuItem");
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.Index = 1;
            this.saveAsMenuItem.MergeOrder = 11;
            resources.ApplyResources(this.saveAsMenuItem, "saveAsMenuItem");
            this.saveAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
            // 
            // saveToDatabaseMenuItem
            // 
            this.saveToDatabaseMenuItem.Index = 2;
            this.saveToDatabaseMenuItem.MergeOrder = 12;
            resources.ApplyResources(this.saveToDatabaseMenuItem, "saveToDatabaseMenuItem");
            this.saveToDatabaseMenuItem.Click += new System.EventHandler(this.saveToDatabaseMenuItem_Click);
            // 
            // saveToDatabaseAsMenuItem
            // 
            this.saveToDatabaseAsMenuItem.Index = 3;
            this.saveToDatabaseAsMenuItem.MergeOrder = 13;
            resources.ApplyResources(this.saveToDatabaseAsMenuItem, "saveToDatabaseAsMenuItem");
            this.saveToDatabaseAsMenuItem.Click += new System.EventHandler(this.saveToDatabaseAsMenuItem_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 4;
            this.menuItem11.MergeOrder = 14;
            resources.ApplyResources(this.menuItem11, "menuItem11");
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem5,
            this.addMenuItem,
            this.modifyMenuItem,
            this.deleteMenuItem,
            this.arrangeMenuItem,
            this.menuItem13,
            this.updateMenuItem,
            this.defineMenuItem});
            this.menuItem2.MergeOrder = 1;
            this.menuItem2.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 0;
            this.menuItem5.MergeOrder = 4;
            resources.ApplyResources(this.menuItem5, "menuItem5");
            // 
            // addMenuItem
            // 
            this.addMenuItem.Index = 1;
            this.addMenuItem.MergeOrder = 5;
            resources.ApplyResources(this.addMenuItem, "addMenuItem");
            this.addMenuItem.Click += new System.EventHandler(this.addMenuItem_Click);
            // 
            // modifyMenuItem
            // 
            this.modifyMenuItem.Index = 2;
            this.modifyMenuItem.MergeOrder = 6;
            resources.ApplyResources(this.modifyMenuItem, "modifyMenuItem");
            this.modifyMenuItem.Click += new System.EventHandler(this.modifyMenuItem_Click);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Index = 3;
            this.deleteMenuItem.MergeOrder = 7;
            resources.ApplyResources(this.deleteMenuItem, "deleteMenuItem");
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // arrangeMenuItem
            // 
            this.arrangeMenuItem.Index = 4;
            this.arrangeMenuItem.MergeOrder = 8;
            resources.ApplyResources(this.arrangeMenuItem, "arrangeMenuItem");
            this.arrangeMenuItem.Click += new System.EventHandler(this.arrangeMenuItem_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 5;
            this.menuItem13.MergeOrder = 9;
            resources.ApplyResources(this.menuItem13, "menuItem13");
            // 
            // updateMenuItem
            // 
            this.updateMenuItem.Index = 6;
            this.updateMenuItem.MergeOrder = 10;
            resources.ApplyResources(this.updateMenuItem, "updateMenuItem");
            this.updateMenuItem.Click += new System.EventHandler(this.updateMenuItem_Click);
            // 
            // defineMenuItem
            // 
            this.defineMenuItem.Index = 7;
            this.defineMenuItem.MergeOrder = 11;
            resources.ApplyResources(this.defineMenuItem, "defineMenuItem");
            this.defineMenuItem.Click += new System.EventHandler(this.defineMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.viewLargeIconMenuItem,
            this.viewSmallIcomMenuItem,
            this.viewListMenuItem,
            this.viewDetailMenuItem,
            this.menuItem6,
            this.updateLogMenuItem});
            this.menuItem3.MergeOrder = 2;
            this.menuItem3.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // viewLargeIconMenuItem
            // 
            this.viewLargeIconMenuItem.Index = 0;
            this.viewLargeIconMenuItem.MergeOrder = 4;
            resources.ApplyResources(this.viewLargeIconMenuItem, "viewLargeIconMenuItem");
            this.viewLargeIconMenuItem.Click += new System.EventHandler(this.viewLargeIconMenuItem_Click);
            // 
            // viewSmallIcomMenuItem
            // 
            this.viewSmallIcomMenuItem.Index = 1;
            this.viewSmallIcomMenuItem.MergeOrder = 5;
            resources.ApplyResources(this.viewSmallIcomMenuItem, "viewSmallIcomMenuItem");
            this.viewSmallIcomMenuItem.Click += new System.EventHandler(this.viewSmallIcomMenuItem_Click);
            // 
            // viewListMenuItem
            // 
            this.viewListMenuItem.Index = 2;
            this.viewListMenuItem.MergeOrder = 6;
            resources.ApplyResources(this.viewListMenuItem, "viewListMenuItem");
            this.viewListMenuItem.Click += new System.EventHandler(this.viewListMenuItem_Click);
            // 
            // viewDetailMenuItem
            // 
            this.viewDetailMenuItem.Checked = true;
            this.viewDetailMenuItem.Index = 3;
            this.viewDetailMenuItem.MergeOrder = 7;
            resources.ApplyResources(this.viewDetailMenuItem, "viewDetailMenuItem");
            this.viewDetailMenuItem.Click += new System.EventHandler(this.viewDetailMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 4;
            this.menuItem6.MergeOrder = 8;
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // updateLogMenuItem
            // 
            this.updateLogMenuItem.Index = 5;
            this.updateLogMenuItem.MergeOrder = 9;
            resources.ApplyResources(this.updateLogMenuItem, "updateLogMenuItem");
            this.updateLogMenuItem.Click += new System.EventHandler(this.updateLogMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.validateMenuItem,
            this.lockMenuItem,
            this.unlockMenuItem,
            this.fixDBMenuItem,
            this.menuItem7,
            this.optionsMenuItem,
            this.menuItem9});
            this.menuItem4.MergeOrder = 3;
            this.menuItem4.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // validateMenuItem
            // 
            this.validateMenuItem.Index = 0;
            resources.ApplyResources(this.validateMenuItem, "validateMenuItem");
            this.validateMenuItem.Click += new System.EventHandler(this.validateMenuItem_Click);
            // 
            // lockMenuItem
            // 
            resources.ApplyResources(this.lockMenuItem, "lockMenuItem");
            this.lockMenuItem.Index = 1;
            this.lockMenuItem.MergeOrder = 1;
            this.lockMenuItem.Click += new System.EventHandler(this.lockMenuItem_Click);
            // 
            // unlockMenuItem
            // 
            resources.ApplyResources(this.unlockMenuItem, "unlockMenuItem");
            this.unlockMenuItem.Index = 2;
            this.unlockMenuItem.MergeOrder = 2;
            this.unlockMenuItem.Click += new System.EventHandler(this.unlockMenuItem_Click);
            // 
            // fixDBMenuItem
            // 
            this.fixDBMenuItem.Index = 3;
            this.fixDBMenuItem.MergeOrder = 3;
            resources.ApplyResources(this.fixDBMenuItem, "fixDBMenuItem");
            this.fixDBMenuItem.Click += new System.EventHandler(this.fixDBMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 4;
            this.menuItem7.MergeOrder = 4;
            resources.ApplyResources(this.menuItem7, "menuItem7");
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Index = 5;
            this.optionsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.aclOptionMenuItem,
            this.loggingOptionMenuItem});
            this.optionsMenuItem.MergeOrder = 5;
            resources.ApplyResources(this.optionsMenuItem, "optionsMenuItem");
            // 
            // aclOptionMenuItem
            // 
            this.aclOptionMenuItem.Index = 0;
            resources.ApplyResources(this.aclOptionMenuItem, "aclOptionMenuItem");
            this.aclOptionMenuItem.Click += new System.EventHandler(this.aclOptionMenuItem_Click);
            // 
            // loggingOptionMenuItem
            // 
            this.loggingOptionMenuItem.Index = 1;
            resources.ApplyResources(this.loggingOptionMenuItem, "loggingOptionMenuItem");
            this.loggingOptionMenuItem.Click += new System.EventHandler(this.loggingOptionMenuItem_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 6;
            this.menuItem9.MergeOrder = 6;
            resources.ApplyResources(this.menuItem9, "menuItem9");
            // 
            // treeView1
            // 
            this.treeView1.ContextMenu = this.treeViewContextMenu;
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.HideSelection = false;
            this.treeView1.ImageList = this.treeViewImageList;
            this.treeView1.ItemHeight = 16;
            this.treeView1.Name = "treeView1";
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // treeViewContextMenu
            // 
            this.treeViewContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.tvCollapseMenuItem,
            this.tvExpandMenuItem,
            this.menuItem10,
            this.tvAddMenuItem,
            this.tvModifyMenuItem,
            this.tvDeleteMenuItem,
            this.tvArrangeMenuItem,
            this.menuItem12,
            this.tvUpdateMenuItem,
            this.tvDefineMenuItem,
            this.menuItem16,
            this.tvExportMenuItem});
            // 
            // tvCollapseMenuItem
            // 
            this.tvCollapseMenuItem.Index = 0;
            resources.ApplyResources(this.tvCollapseMenuItem, "tvCollapseMenuItem");
            this.tvCollapseMenuItem.Click += new System.EventHandler(this.tvCollapseMenuItem_Click);
            // 
            // tvExpandMenuItem
            // 
            this.tvExpandMenuItem.Index = 1;
            resources.ApplyResources(this.tvExpandMenuItem, "tvExpandMenuItem");
            this.tvExpandMenuItem.Click += new System.EventHandler(this.tvExpandMenuItem_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 2;
            resources.ApplyResources(this.menuItem10, "menuItem10");
            // 
            // tvAddMenuItem
            // 
            this.tvAddMenuItem.Index = 3;
            resources.ApplyResources(this.tvAddMenuItem, "tvAddMenuItem");
            this.tvAddMenuItem.Click += new System.EventHandler(this.tvAddMenuItem_Click);
            // 
            // tvModifyMenuItem
            // 
            this.tvModifyMenuItem.Index = 4;
            resources.ApplyResources(this.tvModifyMenuItem, "tvModifyMenuItem");
            this.tvModifyMenuItem.Click += new System.EventHandler(this.tvModifyMenuItem_Click);
            // 
            // tvDeleteMenuItem
            // 
            this.tvDeleteMenuItem.Index = 5;
            resources.ApplyResources(this.tvDeleteMenuItem, "tvDeleteMenuItem");
            this.tvDeleteMenuItem.Click += new System.EventHandler(this.tvDeleteMenuItem_Click);
            // 
            // tvArrangeMenuItem
            // 
            this.tvArrangeMenuItem.Index = 6;
            resources.ApplyResources(this.tvArrangeMenuItem, "tvArrangeMenuItem");
            this.tvArrangeMenuItem.Click += new System.EventHandler(this.tvArrangeMenuItem_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 7;
            resources.ApplyResources(this.menuItem12, "menuItem12");
            // 
            // tvUpdateMenuItem
            // 
            this.tvUpdateMenuItem.Index = 8;
            resources.ApplyResources(this.tvUpdateMenuItem, "tvUpdateMenuItem");
            this.tvUpdateMenuItem.Click += new System.EventHandler(this.tvUpdateMenuItem_Click);
            // 
            // tvDefineMenuItem
            // 
            this.tvDefineMenuItem.Index = 9;
            resources.ApplyResources(this.tvDefineMenuItem, "tvDefineMenuItem");
            this.tvDefineMenuItem.Click += new System.EventHandler(this.tvDefineMenuItem_Click);
            // 
            // menuItem16
            // 
            this.menuItem16.Index = 10;
            resources.ApplyResources(this.menuItem16, "menuItem16");
            // 
            // tvExportMenuItem
            // 
            this.tvExportMenuItem.Index = 11;
            resources.ApplyResources(this.tvExportMenuItem, "tvExportMenuItem");
            this.tvExportMenuItem.Click += new System.EventHandler(this.tvExportMenuItem_Click);
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
            this.treeViewImageList.Images.SetKeyName(15, "virtualproperty.GIF");
            this.treeViewImageList.Images.SetKeyName(16, "imageicon.gif");
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.ContextMenu = this.listViewContextMenu;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.FullRowSelect = true;
            this.listView1.LargeImageList = this.listViewLargeImageList;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.SmallImageList = this.listViewSmallImageList;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
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
            // listViewContextMenu
            // 
            this.listViewContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.lvAddMenuItem,
            this.lvModifyMenuItem,
            this.lvDeleteMenuItem,
            this.lvArrangeMenuItem,
            this.menuItem15,
            this.lvUpdateMenuItem,
            this.lvDefineMenuItem,
            this.menuItem14,
            this.lvLargeIconMenuItem,
            this.lvSmallIconMenuItem,
            this.lvListMenuItem,
            this.lvDetailMenuItem});
            // 
            // lvAddMenuItem
            // 
            this.lvAddMenuItem.Index = 0;
            resources.ApplyResources(this.lvAddMenuItem, "lvAddMenuItem");
            this.lvAddMenuItem.Click += new System.EventHandler(this.lvAddMenuItem_Click);
            // 
            // lvModifyMenuItem
            // 
            this.lvModifyMenuItem.Index = 1;
            resources.ApplyResources(this.lvModifyMenuItem, "lvModifyMenuItem");
            this.lvModifyMenuItem.Click += new System.EventHandler(this.lvModifyMenuItem_Click);
            // 
            // lvDeleteMenuItem
            // 
            this.lvDeleteMenuItem.Index = 2;
            resources.ApplyResources(this.lvDeleteMenuItem, "lvDeleteMenuItem");
            this.lvDeleteMenuItem.Click += new System.EventHandler(this.lvDeleteMenuItem_Click);
            // 
            // lvArrangeMenuItem
            // 
            this.lvArrangeMenuItem.Index = 3;
            resources.ApplyResources(this.lvArrangeMenuItem, "lvArrangeMenuItem");
            this.lvArrangeMenuItem.Click += new System.EventHandler(this.lvArrangeMenuItem_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 4;
            resources.ApplyResources(this.menuItem15, "menuItem15");
            // 
            // lvUpdateMenuItem
            // 
            this.lvUpdateMenuItem.Index = 5;
            resources.ApplyResources(this.lvUpdateMenuItem, "lvUpdateMenuItem");
            this.lvUpdateMenuItem.Click += new System.EventHandler(this.lvUpdateMenuItem_Click);
            // 
            // lvDefineMenuItem
            // 
            this.lvDefineMenuItem.Index = 6;
            resources.ApplyResources(this.lvDefineMenuItem, "lvDefineMenuItem");
            this.lvDefineMenuItem.Click += new System.EventHandler(this.lvDefineMenuItem_Click);
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 7;
            resources.ApplyResources(this.menuItem14, "menuItem14");
            // 
            // lvLargeIconMenuItem
            // 
            this.lvLargeIconMenuItem.Index = 8;
            resources.ApplyResources(this.lvLargeIconMenuItem, "lvLargeIconMenuItem");
            this.lvLargeIconMenuItem.Click += new System.EventHandler(this.lvLargeIconMenuItem_Click);
            // 
            // lvSmallIconMenuItem
            // 
            this.lvSmallIconMenuItem.Index = 9;
            resources.ApplyResources(this.lvSmallIconMenuItem, "lvSmallIconMenuItem");
            this.lvSmallIconMenuItem.Click += new System.EventHandler(this.lvSamllIconMenuItem_Click);
            // 
            // lvListMenuItem
            // 
            this.lvListMenuItem.Index = 10;
            resources.ApplyResources(this.lvListMenuItem, "lvListMenuItem");
            this.lvListMenuItem.Click += new System.EventHandler(this.lvListMenuItem_Click);
            // 
            // lvDetailMenuItem
            // 
            this.lvDetailMenuItem.Index = 11;
            resources.ApplyResources(this.lvDetailMenuItem, "lvDetailMenuItem");
            this.lvDetailMenuItem.Click += new System.EventHandler(this.lvDetailMenuItem_Click);
            // 
            // listViewLargeImageList
            // 
            this.listViewLargeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewLargeImageList.ImageStream")));
            this.listViewLargeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewLargeImageList.Images.SetKeyName(0, "");
            this.listViewLargeImageList.Images.SetKeyName(1, "");
            this.listViewLargeImageList.Images.SetKeyName(2, "");
            this.listViewLargeImageList.Images.SetKeyName(3, "");
            this.listViewLargeImageList.Images.SetKeyName(4, "");
            this.listViewLargeImageList.Images.SetKeyName(5, "");
            this.listViewLargeImageList.Images.SetKeyName(6, "");
            this.listViewLargeImageList.Images.SetKeyName(7, "");
            this.listViewLargeImageList.Images.SetKeyName(8, "");
            this.listViewLargeImageList.Images.SetKeyName(9, "");
            this.listViewLargeImageList.Images.SetKeyName(10, "");
            this.listViewLargeImageList.Images.SetKeyName(11, "");
            this.listViewLargeImageList.Images.SetKeyName(12, "");
            this.listViewLargeImageList.Images.SetKeyName(13, "");
            this.listViewLargeImageList.Images.SetKeyName(14, "");
            this.listViewLargeImageList.Images.SetKeyName(15, "");
            this.listViewLargeImageList.Images.SetKeyName(16, "virtualproperty.GIF");
            this.listViewLargeImageList.Images.SetKeyName(17, "imageicon.gif");
            // 
            // listViewSmallImageList
            // 
            this.listViewSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewSmallImageList.ImageStream")));
            this.listViewSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewSmallImageList.Images.SetKeyName(0, "");
            this.listViewSmallImageList.Images.SetKeyName(1, "");
            this.listViewSmallImageList.Images.SetKeyName(2, "");
            this.listViewSmallImageList.Images.SetKeyName(3, "");
            this.listViewSmallImageList.Images.SetKeyName(4, "");
            this.listViewSmallImageList.Images.SetKeyName(5, "");
            this.listViewSmallImageList.Images.SetKeyName(6, "");
            this.listViewSmallImageList.Images.SetKeyName(7, "");
            this.listViewSmallImageList.Images.SetKeyName(8, "");
            this.listViewSmallImageList.Images.SetKeyName(9, "");
            this.listViewSmallImageList.Images.SetKeyName(10, "");
            this.listViewSmallImageList.Images.SetKeyName(11, "");
            this.listViewSmallImageList.Images.SetKeyName(12, "");
            this.listViewSmallImageList.Images.SetKeyName(13, "");
            this.listViewSmallImageList.Images.SetKeyName(14, "");
            this.listViewSmallImageList.Images.SetKeyName(15, "");
            this.listViewSmallImageList.Images.SetKeyName(16, "");
            this.listViewSmallImageList.Images.SetKeyName(17, "imageicon.gif");
            // 
            // splitter2
            // 
            this.splitter2.Cursor = System.Windows.Forms.Cursors.HSplit;
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.apiPage);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.eventTabPage);
            this.tabControl1.Controls.Add(this.loggingPage);
            this.tabControl1.Controls.Add(this.subscriberPage);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propertyGrid1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.ContextMenu = this.propertyContextMenu;
            resources.ApplyResources(this.propertyGrid1, "propertyGrid1");
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // propertyContextMenu
            // 
            this.propertyContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Reset,
            this.menuItem8,
            this.description});
            this.propertyContextMenu.Popup += new System.EventHandler(this.propertyContextMenu_Popup);
            // 
            // Reset
            // 
            this.Reset.Index = 0;
            resources.ApplyResources(this.Reset, "Reset");
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 1;
            resources.ApplyResources(this.menuItem8, "menuItem8");
            // 
            // description
            // 
            this.description.Checked = true;
            this.description.Index = 2;
            resources.ApplyResources(this.description, "description");
            this.description.Click += new System.EventHandler(this.description_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.permissionGroupBox);
            this.tabPage3.Controls.Add(this.removeRuleButton);
            this.tabPage3.Controls.Add(this.addRuleButton);
            this.tabPage3.Controls.Add(this.ruleListView);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // permissionGroupBox
            // 
            resources.ApplyResources(this.permissionGroupBox, "permissionGroupBox");
            this.permissionGroupBox.Controls.Add(this.panel6);
            this.permissionGroupBox.Controls.Add(this.panel5);
            this.permissionGroupBox.Controls.Add(this.label4);
            this.permissionGroupBox.Controls.Add(this.label5);
            this.permissionGroupBox.Controls.Add(this.panel1);
            this.permissionGroupBox.Controls.Add(this.panel2);
            this.permissionGroupBox.Controls.Add(this.panel3);
            this.permissionGroupBox.Controls.Add(this.panel4);
            this.permissionGroupBox.Controls.Add(this.label10);
            this.permissionGroupBox.Controls.Add(this.conditionTextBox);
            this.permissionGroupBox.Controls.Add(this.allowPropagateCheckBox);
            this.permissionGroupBox.Name = "permissionGroupBox";
            this.permissionGroupBox.TabStop = false;
            // 
            // panel6
            // 
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Controls.Add(this.denyUploadRadioButton);
            this.panel6.Controls.Add(this.grantUploadRadioButton);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Name = "panel6";
            // 
            // denyUploadRadioButton
            // 
            resources.ApplyResources(this.denyUploadRadioButton, "denyUploadRadioButton");
            this.denyUploadRadioButton.Name = "denyUploadRadioButton";
            this.denyUploadRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // grantUploadRadioButton
            // 
            resources.ApplyResources(this.grantUploadRadioButton, "grantUploadRadioButton");
            this.grantUploadRadioButton.Name = "grantUploadRadioButton";
            this.grantUploadRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Controls.Add(this.denyDownloadRadioButton);
            this.panel5.Controls.Add(this.grantDownloadRadioButton);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Name = "panel5";
            // 
            // denyDownloadRadioButton
            // 
            resources.ApplyResources(this.denyDownloadRadioButton, "denyDownloadRadioButton");
            this.denyDownloadRadioButton.Name = "denyDownloadRadioButton";
            this.denyDownloadRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // grantDownloadRadioButton
            // 
            resources.ApplyResources(this.grantDownloadRadioButton, "grantDownloadRadioButton");
            this.grantDownloadRadioButton.Name = "grantDownloadRadioButton";
            this.grantDownloadRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.denyReadRadioButton);
            this.panel1.Controls.Add(this.grantReadRadioButton);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Name = "panel1";
            // 
            // denyReadRadioButton
            // 
            resources.ApplyResources(this.denyReadRadioButton, "denyReadRadioButton");
            this.denyReadRadioButton.Name = "denyReadRadioButton";
            this.denyReadRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // grantReadRadioButton
            // 
            resources.ApplyResources(this.grantReadRadioButton, "grantReadRadioButton");
            this.grantReadRadioButton.Name = "grantReadRadioButton";
            this.grantReadRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.denyWriteRadioButton);
            this.panel2.Controls.Add(this.grantWriteRadioButton);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Name = "panel2";
            // 
            // denyWriteRadioButton
            // 
            resources.ApplyResources(this.denyWriteRadioButton, "denyWriteRadioButton");
            this.denyWriteRadioButton.Name = "denyWriteRadioButton";
            this.denyWriteRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // grantWriteRadioButton
            // 
            resources.ApplyResources(this.grantWriteRadioButton, "grantWriteRadioButton");
            this.grantWriteRadioButton.Name = "grantWriteRadioButton";
            this.grantWriteRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.denyCreateRadioButton);
            this.panel3.Controls.Add(this.grantCreateRadioButton);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Name = "panel3";
            // 
            // denyCreateRadioButton
            // 
            resources.ApplyResources(this.denyCreateRadioButton, "denyCreateRadioButton");
            this.denyCreateRadioButton.Name = "denyCreateRadioButton";
            this.denyCreateRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // grantCreateRadioButton
            // 
            resources.ApplyResources(this.grantCreateRadioButton, "grantCreateRadioButton");
            this.grantCreateRadioButton.Name = "grantCreateRadioButton";
            this.grantCreateRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Controls.Add(this.denyDeleteRadioButton);
            this.panel4.Controls.Add(this.grantDeleteRadioButton);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Name = "panel4";
            // 
            // denyDeleteRadioButton
            // 
            resources.ApplyResources(this.denyDeleteRadioButton, "denyDeleteRadioButton");
            this.denyDeleteRadioButton.Name = "denyDeleteRadioButton";
            this.denyDeleteRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // grantDeleteRadioButton
            // 
            resources.ApplyResources(this.grantDeleteRadioButton, "grantDeleteRadioButton");
            this.grantDeleteRadioButton.Name = "grantDeleteRadioButton";
            this.grantDeleteRadioButton.Click += new System.EventHandler(this.permissionRadioButton_Click);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // conditionTextBox
            // 
            resources.ApplyResources(this.conditionTextBox, "conditionTextBox");
            this.conditionTextBox.Name = "conditionTextBox";
            this.conditionTextBox.TextChanged += new System.EventHandler(this.conditionTextBox_TextChanged);
            // 
            // allowPropagateCheckBox
            // 
            resources.ApplyResources(this.allowPropagateCheckBox, "allowPropagateCheckBox");
            this.allowPropagateCheckBox.Name = "allowPropagateCheckBox";
            this.allowPropagateCheckBox.CheckStateChanged += new System.EventHandler(this.allowPropagateCheckBox_CheckStateChanged);
            // 
            // removeRuleButton
            // 
            resources.ApplyResources(this.removeRuleButton, "removeRuleButton");
            this.removeRuleButton.Name = "removeRuleButton";
            this.removeRuleButton.Click += new System.EventHandler(this.removeRuleButton_Click);
            // 
            // addRuleButton
            // 
            resources.ApplyResources(this.addRuleButton, "addRuleButton");
            this.addRuleButton.Name = "addRuleButton";
            this.addRuleButton.Click += new System.EventHandler(this.addRuleButton_Click);
            // 
            // ruleListView
            // 
            resources.ApplyResources(this.ruleListView, "ruleListView");
            this.ruleListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader7});
            this.ruleListView.FullRowSelect = true;
            this.ruleListView.HideSelection = false;
            this.ruleListView.LargeImageList = this.listViewLargeImageList;
            this.ruleListView.MultiSelect = false;
            this.ruleListView.Name = "ruleListView";
            this.ruleListView.SmallImageList = this.listViewSmallImageList;
            this.ruleListView.UseCompatibleStateImageBehavior = false;
            this.ruleListView.View = System.Windows.Forms.View.Details;
            this.ruleListView.SelectedIndexChanged += new System.EventHandler(this.ruleListView_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // apiPage
            // 
            this.apiPage.Controls.Add(this.addCustomButton);
            this.apiPage.Controls.Add(this.apiPropertyGrid);
            this.apiPage.Controls.Add(this.deleteApiButton);
            this.apiPage.Controls.Add(this.apiListView);
            resources.ApplyResources(this.apiPage, "apiPage");
            this.apiPage.Name = "apiPage";
            this.apiPage.UseVisualStyleBackColor = true;
            // 
            // addCustomButton
            // 
            resources.ApplyResources(this.addCustomButton, "addCustomButton");
            this.addCustomButton.Name = "addCustomButton";
            this.addCustomButton.UseVisualStyleBackColor = true;
            this.addCustomButton.Click += new System.EventHandler(this.addCustomButton_Click);
            // 
            // apiPropertyGrid
            // 
            resources.ApplyResources(this.apiPropertyGrid, "apiPropertyGrid");
            this.apiPropertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.apiPropertyGrid.Name = "apiPropertyGrid";
            this.apiPropertyGrid.ToolbarVisible = false;
            this.apiPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.apiPropertyGrid_PropertyValueChanged);
            // 
            // deleteApiButton
            // 
            resources.ApplyResources(this.deleteApiButton, "deleteApiButton");
            this.deleteApiButton.Name = "deleteApiButton";
            this.deleteApiButton.UseVisualStyleBackColor = true;
            this.deleteApiButton.Click += new System.EventHandler(this.deleteApiButton_Click);
            // 
            // apiListView
            // 
            resources.ApplyResources(this.apiListView, "apiListView");
            this.apiListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader15,
            this.columnHeader16});
            this.apiListView.FullRowSelect = true;
            this.apiListView.HideSelection = false;
            this.apiListView.MultiSelect = false;
            this.apiListView.Name = "apiListView";
            this.apiListView.UseCompatibleStateImageBehavior = false;
            this.apiListView.View = System.Windows.Forms.View.Details;
            this.apiListView.SelectedIndexChanged += new System.EventHandler(this.apiListView_SelectedIndexChanged);
            // 
            // columnHeader15
            // 
            resources.ApplyResources(this.columnHeader15, "columnHeader15");
            // 
            // columnHeader16
            // 
            resources.ApplyResources(this.columnHeader16, "columnHeader16");
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.modifyValidateRuleBtn);
            this.tabPage2.Controls.Add(this.removeValidateRuleBtn);
            this.tabPage2.Controls.Add(this.addValidateRuleBtn);
            this.tabPage2.Controls.Add(this.validateRuleListView);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // modifyValidateRuleBtn
            // 
            resources.ApplyResources(this.modifyValidateRuleBtn, "modifyValidateRuleBtn");
            this.modifyValidateRuleBtn.Name = "modifyValidateRuleBtn";
            this.modifyValidateRuleBtn.Click += new System.EventHandler(this.modifyValidateRuleBtn_Click);
            // 
            // removeValidateRuleBtn
            // 
            resources.ApplyResources(this.removeValidateRuleBtn, "removeValidateRuleBtn");
            this.removeValidateRuleBtn.Name = "removeValidateRuleBtn";
            this.removeValidateRuleBtn.Click += new System.EventHandler(this.removeValidateRuleBtn_Click);
            // 
            // addValidateRuleBtn
            // 
            resources.ApplyResources(this.addValidateRuleBtn, "addValidateRuleBtn");
            this.addValidateRuleBtn.Name = "addValidateRuleBtn";
            this.addValidateRuleBtn.Click += new System.EventHandler(this.addValidateRuleBtn_Click);
            // 
            // validateRuleListView
            // 
            resources.ApplyResources(this.validateRuleListView, "validateRuleListView");
            this.validateRuleListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader10});
            this.validateRuleListView.FullRowSelect = true;
            this.validateRuleListView.HideSelection = false;
            this.validateRuleListView.LargeImageList = this.listViewLargeImageList;
            this.validateRuleListView.MultiSelect = false;
            this.validateRuleListView.Name = "validateRuleListView";
            this.validateRuleListView.ShowItemToolTips = true;
            this.validateRuleListView.SmallImageList = this.listViewSmallImageList;
            this.validateRuleListView.UseCompatibleStateImageBehavior = false;
            this.validateRuleListView.View = System.Windows.Forms.View.Details;
            this.validateRuleListView.SelectedIndexChanged += new System.EventHandler(this.validateRuleListView_SelectedIndexChanged);
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // columnHeader10
            // 
            resources.ApplyResources(this.columnHeader10, "columnHeader10");
            // 
            // eventTabPage
            // 
            this.eventTabPage.Controls.Add(this.eventPropertyGrid);
            this.eventTabPage.Controls.Add(this.deleteEventButton);
            this.eventTabPage.Controls.Add(this.addEventButton);
            this.eventTabPage.Controls.Add(this.eventListView);
            resources.ApplyResources(this.eventTabPage, "eventTabPage");
            this.eventTabPage.Name = "eventTabPage";
            this.eventTabPage.UseVisualStyleBackColor = true;
            // 
            // eventPropertyGrid
            // 
            resources.ApplyResources(this.eventPropertyGrid, "eventPropertyGrid");
            this.eventPropertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.eventPropertyGrid.Name = "eventPropertyGrid";
            this.eventPropertyGrid.ToolbarVisible = false;
            this.eventPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.eventPropertyGrid_PropertyValueChanged);
            // 
            // deleteEventButton
            // 
            resources.ApplyResources(this.deleteEventButton, "deleteEventButton");
            this.deleteEventButton.Name = "deleteEventButton";
            this.deleteEventButton.UseVisualStyleBackColor = true;
            this.deleteEventButton.Click += new System.EventHandler(this.deleteEventButton_Click);
            // 
            // addEventButton
            // 
            resources.ApplyResources(this.addEventButton, "addEventButton");
            this.addEventButton.Name = "addEventButton";
            this.addEventButton.UseVisualStyleBackColor = true;
            this.addEventButton.Click += new System.EventHandler(this.addEventButton_Click);
            // 
            // eventListView
            // 
            resources.ApplyResources(this.eventListView, "eventListView");
            this.eventListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9});
            this.eventListView.FullRowSelect = true;
            this.eventListView.HideSelection = false;
            this.eventListView.MultiSelect = false;
            this.eventListView.Name = "eventListView";
            this.eventListView.UseCompatibleStateImageBehavior = false;
            this.eventListView.View = System.Windows.Forms.View.Details;
            this.eventListView.SelectedIndexChanged += new System.EventHandler(this.eventListView_SelectedIndexChanged);
            // 
            // columnHeader8
            // 
            resources.ApplyResources(this.columnHeader8, "columnHeader8");
            // 
            // columnHeader9
            // 
            resources.ApplyResources(this.columnHeader9, "columnHeader9");
            // 
            // loggingPage
            // 
            this.loggingPage.Controls.Add(this.logStatusGroupBox);
            this.loggingPage.Controls.Add(this.removeLoggingRuleButton);
            this.loggingPage.Controls.Add(this.addLoggingRuleButton);
            this.loggingPage.Controls.Add(this.loggingRuleListView);
            resources.ApplyResources(this.loggingPage, "loggingPage");
            this.loggingPage.Name = "loggingPage";
            this.loggingPage.UseVisualStyleBackColor = true;
            // 
            // logStatusGroupBox
            // 
            this.logStatusGroupBox.Controls.Add(this.panel14);
            this.logStatusGroupBox.Controls.Add(this.panel13);
            this.logStatusGroupBox.Controls.Add(this.panel7);
            this.logStatusGroupBox.Controls.Add(this.panel8);
            this.logStatusGroupBox.Controls.Add(this.label12);
            this.logStatusGroupBox.Controls.Add(this.label13);
            this.logStatusGroupBox.Controls.Add(this.panel9);
            this.logStatusGroupBox.Controls.Add(this.panel10);
            this.logStatusGroupBox.Controls.Add(this.panel11);
            this.logStatusGroupBox.Controls.Add(this.panel12);
            this.logStatusGroupBox.Controls.Add(this.allowLogingRulePassDownCheckBox);
            resources.ApplyResources(this.logStatusGroupBox, "logStatusGroupBox");
            this.logStatusGroupBox.Name = "logStatusGroupBox";
            this.logStatusGroupBox.TabStop = false;
            // 
            // panel14
            // 
            resources.ApplyResources(this.panel14, "panel14");
            this.panel14.Controls.Add(this.exportLogOffRadioButton);
            this.panel14.Controls.Add(this.exportLogOnRadioButton);
            this.panel14.Controls.Add(this.label19);
            this.panel14.Name = "panel14";
            // 
            // exportLogOffRadioButton
            // 
            resources.ApplyResources(this.exportLogOffRadioButton, "exportLogOffRadioButton");
            this.exportLogOffRadioButton.Name = "exportLogOffRadioButton";
            // 
            // exportLogOnRadioButton
            // 
            resources.ApplyResources(this.exportLogOnRadioButton, "exportLogOnRadioButton");
            this.exportLogOnRadioButton.Name = "exportLogOnRadioButton";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // panel13
            // 
            resources.ApplyResources(this.panel13, "panel13");
            this.panel13.Controls.Add(this.importLogOffRadioButton);
            this.panel13.Controls.Add(this.importLogOnRadioButton);
            this.panel13.Controls.Add(this.label18);
            this.panel13.Name = "panel13";
            // 
            // importLogOffRadioButton
            // 
            resources.ApplyResources(this.importLogOffRadioButton, "importLogOffRadioButton");
            this.importLogOffRadioButton.Name = "importLogOffRadioButton";
            // 
            // importLogOnRadioButton
            // 
            resources.ApplyResources(this.importLogOnRadioButton, "importLogOnRadioButton");
            this.importLogOnRadioButton.Name = "importLogOnRadioButton";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // panel7
            // 
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Controls.Add(this.uploadLogOffRadioButton);
            this.panel7.Controls.Add(this.uploadLogOnRadioButton);
            this.panel7.Controls.Add(this.label3);
            this.panel7.Name = "panel7";
            // 
            // uploadLogOffRadioButton
            // 
            resources.ApplyResources(this.uploadLogOffRadioButton, "uploadLogOffRadioButton");
            this.uploadLogOffRadioButton.Name = "uploadLogOffRadioButton";
            this.uploadLogOffRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // uploadLogOnRadioButton
            // 
            resources.ApplyResources(this.uploadLogOnRadioButton, "uploadLogOnRadioButton");
            this.uploadLogOnRadioButton.Name = "uploadLogOnRadioButton";
            this.uploadLogOnRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // panel8
            // 
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.Controls.Add(this.downloadLogOffRadioButton);
            this.panel8.Controls.Add(this.downloadLogOnRadioButton);
            this.panel8.Controls.Add(this.label11);
            this.panel8.Name = "panel8";
            // 
            // downloadLogOffRadioButton
            // 
            resources.ApplyResources(this.downloadLogOffRadioButton, "downloadLogOffRadioButton");
            this.downloadLogOffRadioButton.Name = "downloadLogOffRadioButton";
            this.downloadLogOffRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // downloadLogOnRadioButton
            // 
            resources.ApplyResources(this.downloadLogOnRadioButton, "downloadLogOnRadioButton");
            this.downloadLogOnRadioButton.Name = "downloadLogOnRadioButton";
            this.downloadLogOnRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // panel9
            // 
            resources.ApplyResources(this.panel9, "panel9");
            this.panel9.Controls.Add(this.readLogOffRadioButton);
            this.panel9.Controls.Add(this.readLogOnRadioButton);
            this.panel9.Controls.Add(this.label14);
            this.panel9.Name = "panel9";
            // 
            // readLogOffRadioButton
            // 
            resources.ApplyResources(this.readLogOffRadioButton, "readLogOffRadioButton");
            this.readLogOffRadioButton.Name = "readLogOffRadioButton";
            this.readLogOffRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // readLogOnRadioButton
            // 
            resources.ApplyResources(this.readLogOnRadioButton, "readLogOnRadioButton");
            this.readLogOnRadioButton.Name = "readLogOnRadioButton";
            this.readLogOnRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // panel10
            // 
            resources.ApplyResources(this.panel10, "panel10");
            this.panel10.Controls.Add(this.modifyLogOffRadioButton);
            this.panel10.Controls.Add(this.modifyLogOnRadioButton);
            this.panel10.Controls.Add(this.label15);
            this.panel10.Name = "panel10";
            // 
            // modifyLogOffRadioButton
            // 
            resources.ApplyResources(this.modifyLogOffRadioButton, "modifyLogOffRadioButton");
            this.modifyLogOffRadioButton.Name = "modifyLogOffRadioButton";
            this.modifyLogOffRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // modifyLogOnRadioButton
            // 
            resources.ApplyResources(this.modifyLogOnRadioButton, "modifyLogOnRadioButton");
            this.modifyLogOnRadioButton.Name = "modifyLogOnRadioButton";
            this.modifyLogOnRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // panel11
            // 
            resources.ApplyResources(this.panel11, "panel11");
            this.panel11.Controls.Add(this.createLogOffRadioButton);
            this.panel11.Controls.Add(this.createLogOnRadioButton);
            this.panel11.Controls.Add(this.label16);
            this.panel11.Name = "panel11";
            // 
            // createLogOffRadioButton
            // 
            resources.ApplyResources(this.createLogOffRadioButton, "createLogOffRadioButton");
            this.createLogOffRadioButton.Name = "createLogOffRadioButton";
            this.createLogOffRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // createLogOnRadioButton
            // 
            resources.ApplyResources(this.createLogOnRadioButton, "createLogOnRadioButton");
            this.createLogOnRadioButton.Name = "createLogOnRadioButton";
            this.createLogOnRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // panel12
            // 
            resources.ApplyResources(this.panel12, "panel12");
            this.panel12.Controls.Add(this.deleteLogOffRadioButton);
            this.panel12.Controls.Add(this.deleteLogOnRadioButton);
            this.panel12.Controls.Add(this.label17);
            this.panel12.Name = "panel12";
            // 
            // deleteLogOffRadioButton
            // 
            resources.ApplyResources(this.deleteLogOffRadioButton, "deleteLogOffRadioButton");
            this.deleteLogOffRadioButton.Name = "deleteLogOffRadioButton";
            this.deleteLogOffRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // deleteLogOnRadioButton
            // 
            resources.ApplyResources(this.deleteLogOnRadioButton, "deleteLogOnRadioButton");
            this.deleteLogOnRadioButton.Name = "deleteLogOnRadioButton";
            this.deleteLogOnRadioButton.Click += new System.EventHandler(this.logStatusRadioButton_Click);
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // allowLogingRulePassDownCheckBox
            // 
            resources.ApplyResources(this.allowLogingRulePassDownCheckBox, "allowLogingRulePassDownCheckBox");
            this.allowLogingRulePassDownCheckBox.Name = "allowLogingRulePassDownCheckBox";
            this.allowLogingRulePassDownCheckBox.CheckStateChanged += new System.EventHandler(this.allowLogingRulePassDownCheckBox_CheckStateChanged);
            // 
            // removeLoggingRuleButton
            // 
            resources.ApplyResources(this.removeLoggingRuleButton, "removeLoggingRuleButton");
            this.removeLoggingRuleButton.Name = "removeLoggingRuleButton";
            this.removeLoggingRuleButton.Click += new System.EventHandler(this.removeLoggingRuleButton_Click);
            // 
            // addLoggingRuleButton
            // 
            resources.ApplyResources(this.addLoggingRuleButton, "addLoggingRuleButton");
            this.addLoggingRuleButton.Name = "addLoggingRuleButton";
            this.addLoggingRuleButton.Click += new System.EventHandler(this.addLoggingRuleButton_Click);
            // 
            // loggingRuleListView
            // 
            resources.ApplyResources(this.loggingRuleListView, "loggingRuleListView");
            this.loggingRuleListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12});
            this.loggingRuleListView.FullRowSelect = true;
            this.loggingRuleListView.HideSelection = false;
            this.loggingRuleListView.LargeImageList = this.listViewLargeImageList;
            this.loggingRuleListView.MultiSelect = false;
            this.loggingRuleListView.Name = "loggingRuleListView";
            this.loggingRuleListView.SmallImageList = this.listViewSmallImageList;
            this.loggingRuleListView.UseCompatibleStateImageBehavior = false;
            this.loggingRuleListView.View = System.Windows.Forms.View.Details;
            this.loggingRuleListView.SelectedIndexChanged += new System.EventHandler(this.loggingRuleListView_SelectedIndexChanged);
            // 
            // columnHeader11
            // 
            resources.ApplyResources(this.columnHeader11, "columnHeader11");
            // 
            // columnHeader12
            // 
            resources.ApplyResources(this.columnHeader12, "columnHeader12");
            // 
            // subscriberPage
            // 
            this.subscriberPage.Controls.Add(this.delSubscriberButton);
            this.subscriberPage.Controls.Add(this.addSubscriberButton);
            this.subscriberPage.Controls.Add(this.subscriberPropertyGrid);
            this.subscriberPage.Controls.Add(this.subscribersListView);
            resources.ApplyResources(this.subscriberPage, "subscriberPage");
            this.subscriberPage.Name = "subscriberPage";
            this.subscriberPage.UseVisualStyleBackColor = true;
            // 
            // delSubscriberButton
            // 
            resources.ApplyResources(this.delSubscriberButton, "delSubscriberButton");
            this.delSubscriberButton.Name = "delSubscriberButton";
            this.delSubscriberButton.UseVisualStyleBackColor = true;
            this.delSubscriberButton.Click += new System.EventHandler(this.delSubscriberButton_Click);
            // 
            // addSubscriberButton
            // 
            resources.ApplyResources(this.addSubscriberButton, "addSubscriberButton");
            this.addSubscriberButton.Name = "addSubscriberButton";
            this.addSubscriberButton.UseVisualStyleBackColor = true;
            this.addSubscriberButton.Click += new System.EventHandler(this.addSubscriberButton_Click);
            // 
            // subscriberPropertyGrid
            // 
            resources.ApplyResources(this.subscriberPropertyGrid, "subscriberPropertyGrid");
            this.subscriberPropertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.subscriberPropertyGrid.Name = "subscriberPropertyGrid";
            this.subscriberPropertyGrid.ToolbarVisible = false;
            this.subscriberPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.subscriberPropertyGrid_PropertyValueChanged);
            // 
            // subscribersListView
            // 
            resources.ApplyResources(this.subscribersListView, "subscribersListView");
            this.subscribersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader13,
            this.columnHeader14});
            this.subscribersListView.FullRowSelect = true;
            this.subscribersListView.HideSelection = false;
            this.subscribersListView.MultiSelect = false;
            this.subscribersListView.Name = "subscribersListView";
            this.subscribersListView.UseCompatibleStateImageBehavior = false;
            this.subscribersListView.View = System.Windows.Forms.View.Details;
            this.subscribersListView.SelectedIndexChanged += new System.EventHandler(this.subscribersListView_SelectedIndexChanged);
            // 
            // columnHeader13
            // 
            resources.ApplyResources(this.columnHeader13, "columnHeader13");
            // 
            // columnHeader14
            // 
            resources.ApplyResources(this.columnHeader14, "columnHeader14");
            // 
            // printPreviewDialog1
            // 
            resources.ApplyResources(this.printPreviewDialog1, "printPreviewDialog1");
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            // 
            // SchemaEditor
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Menu = this.mainMenu1;
            this.Name = "SchemaEditor";
            this.Activated += new System.EventHandler(this.SchemaEditor_Activated);
            this.Deactivate += new System.EventHandler(this.SchemaEditor_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SchemaEditor_FormClosing);
            this.Load += new System.EventHandler(this.SchemaEditor_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.permissionGroupBox.ResumeLayout(false);
            this.permissionGroupBox.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.apiPage.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.eventTabPage.ResumeLayout(false);
            this.loggingPage.ResumeLayout(false);
            this.logStatusGroupBox.ResumeLayout(false);
            this.panel14.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.subscriberPage.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region exposed methods

		/// <summary>
		/// Gets or sets the schema file name
		/// </summary>
		public string SchemaFileName
		{
			get
			{
				return _schemaFileName;
			}
			set
			{
				_schemaFileName = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the current editing
		/// meta data schema is loaded from database
		/// </summary>
		public bool IsDBLoaded
		{
			get
			{
				return _isDBLoaded;
			}
			set
			{
				_isDBLoaded = value;
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

		/// <summary>
		/// Save the currently editing schema to the file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void SaveMetaDataToFile(object sender, System.EventArgs e)
		{
			if (_schemaFileName == null)
			{
				_schemaFileName = GetSaveFileName();
			}

			if (_schemaFileName != null)
			{
				SaveMetaData();
			}
		}

		/// <summary>
		/// Save the currently editing schema to the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void SaveMetaDataToDatabase(object sender, System.EventArgs e)
		{
			if (!_isDBLoaded)
			{
				SaveAsSchemaToDatabase(sender, e);
			}
			else
			{
				if (this.IsDBAUser)
				{
					if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.SaveSchema"),
						"Confirm Dialog", MessageBoxButtons.YesNo,
						MessageBoxIcon.Question) == DialogResult.Yes)
					{
						DoSaveMetaDataToDatabase(false);
					}
				}
				else
				{
                    MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("DesignStudio.DBARequired"));
				}
			}
		}

		/// <summary>
		/// Save the currently editing schema to database with the
		/// name and version that the users give.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void SaveAsSchemaToDatabase(object sender, System.EventArgs e)
		{
			// only system administrator is allowed to save a new schema to
			// database
			LoginDialog dialog = new LoginDialog();
            bool isExistingSchema;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
                Newtera.Common.Core.SchemaInfo schemaInfo = GetSaveSchemaInfo(out isExistingSchema);
		
				if (schemaInfo != null)
				{
					// Update the schema info in the meta data
					_metaData.SchemaInfo = schemaInfo;

					// set IsAltered flag to true to trigger the action
					_metaData.IsAltered = true;

                    // to override an existing schema in a safe mode
                    DoSaveMetaDataToDatabase(isExistingSchema);
				}
			}
		}

		/// <summary>
		/// The AsyncCallback event handler for Web service method SetMetaData.
		/// </summary>
		/// <param name="res">The result</param>
		private void SetMetaDataDone(DateTime modifiedTime)
		{
			try
			{
				// set the status back to ready message
				((DesignStudio) this.MdiParent).ShowReadyStatus();

				_metaData.IsAltered = false;
                _metaData.SchemaInfo.ModifiedTime = modifiedTime;
                _metaData.SchemaModel.SchemaInfo.ModifiedTime = modifiedTime;

				// since the meta data has been saved to the database, there is no
				// need to save it to the files
				_metaData.NeedToSave = false;
				_isDBLoaded = true;

                _menuItemStates.SetState(MenuItemID.ViewRefresh, true);

				// set a fake table name to the newly added class so that
				// they can be viewed by DataViewer
				foreach (ClassElement clsElement in _metaData.SchemaModel.RootClasses)
				{
					SetFakeTableName(clsElement);
				}

                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.UpdateSchema"));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Set a fake table name to a class element and its subclasses to
		/// indicated that it has been saved to database.
		/// </summary>
		/// <param name="clsElement">The class element</param>
		private void SetFakeTableName(ClassElement clsElement)
		{
			if (clsElement.TableName == null || clsElement.TableName.Length == 0)
			{
				clsElement.TableName = "New";
			}

			foreach (ClassElement subClass in clsElement.Subclasses)
			{
				SetFakeTableName(subClass);
			}
		}

		/// <summary>
		/// Perform the action of saving the currently editing schema to the database
		/// </summary>
        /// <param name="isSafeMode">Indicate whether to update the meta data in a safe mode</param>
		internal void DoSaveMetaDataToDatabase(bool isSafeMode)
		{
            // validate the schema first, if the schema has validate errors,
            // stop saving it to database.
            ValidateResult result = _metaData.SchemaModel.Validate();

            // verify if the result contains some doubts
            if (result.HasDoubt)
            {
                VerifyDoubts(result);
            }

            // validate the xacl rules, append the errors to the validating result
            _metaData.XaclPolicy.Validate(_metaData, new WindowClientUserManager(), result);

            if (result.HasError)
            {
                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                    "Error Dialog", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error) == DialogResult.OK)
                {
                    ShowValidateErrorDialog(result);
                }

                return;
            }

            // validate the event definitions, append the errors to the validating result
            _metaData.EventManager.Validate(_metaData, new WindowClientUserManager(), result);

            // verify if the result contains some doubts
            if (result.HasDoubt)
            {
                VerifyDoubts(result);
            }

            if (result.HasError)
            {
                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                    "Error Dialog", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error) == DialogResult.OK)
                {
                    ShowValidateErrorDialog(result);
                }

                return;
            }

            // validate the subscriber definitions, append the errors to the validating result
            _metaData.SubscriberManager.Validate(_metaData, new WindowClientUserManager(), result);

            // verify if the result contains some doubts
            if (result.HasDoubt)
            {
                VerifyDoubts(result);
            }

            if (result.HasError)
            {
                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                    "Error Dialog", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error) == DialogResult.OK)
                {
                    ShowValidateErrorDialog(result);
                }

                return;
            }

            // validate the api definitions, append the errors to the validating result
            _metaData.ApiManager.Validate(_metaData, new WindowClientUserManager(), result);

            // verify if the result contains some doubts
            if (result.HasDoubt)
            {
                VerifyDoubts(result);
            }

            if (result.HasError)
            {
                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                    "Error Dialog", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error) == DialogResult.OK)
                {
                    ShowValidateErrorDialog(result);
                }

                return;
            }

            // validate the logging rules, append the errors to the validating result
            _metaData.LoggingPolicy.Validate(_metaData, new WindowClientUserManager(), result);

            if (result.HasError)
            {
                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                    "Error Dialog", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error) == DialogResult.OK)
                {
                    ShowValidateErrorDialog(result);
                }

                return;
            }

            // validate the taxonomies, if there are errors, stop saving to database
            DataViewValidateResult validateResult = null;
            foreach (TaxonomyModel taxonomy in _metaData.Taxonomies)
            {
                validateResult = taxonomy.Validate();
                if (validateResult.HasError)
                {
                    break;
                }
            }

            if (validateResult != null && validateResult.HasError)
            {
                ShowTaxonomyValidateErrorDialog(validateResult);

                return;
            }

            // validate the data views, if there are errors, stop saving to database
            validateResult = null;
            foreach (DataViewModel dataView in _metaData.DataViews)
            {
                validateResult = dataView.ValidateDataView();
                if (validateResult.HasError)
                {
                    break;
                }
            }

            if (validateResult != null && validateResult.HasError)
            {
                ShowDataViewValidateErrorDialog(validateResult);

                return;
            }


            // display a text in the status bar
            ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.SavingSchema"));

            StringBuilder builder;
            StringWriter writer;
            string[] xmlStrings = new string[12];

            if (_metaData.SchemaModel.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.SchemaModel.Write(writer);
                // the first string is a xml string for schema model
                xmlStrings[0] = builder.ToString();
            }
            else
            {
                xmlStrings[0] = "";
            }

            if (_metaData.DataViews.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.DataViews.Write(writer);
                // the second string is a xml string for dataviews
                xmlStrings[1] = builder.ToString();
            }
            else
            {
                xmlStrings[1] = "";
            }

            if (_metaData.XaclPolicy.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.XaclPolicy.Write(writer);
                // the third string is a xml string for xacl policy
                xmlStrings[2] = builder.ToString();
            }
            else
            {
                xmlStrings[2] = "";
            }

            if (_metaData.Taxonomies.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.Taxonomies.Write(writer);
                // the forth string is a xml string for taxonomies
                xmlStrings[3] = builder.ToString();
            }
            else
            {
                xmlStrings[3] = "";
            }

            if (_metaData.RuleManager.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.RuleManager.Write(writer);
                // the fifth string is a xml string for rules
                xmlStrings[4] = builder.ToString();
            }
            else
            {
                xmlStrings[4] = "";
            }

            if (_metaData.MappingManager.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.MappingManager.Write(writer);
                // the sixth string is a xml string for mappings
                xmlStrings[5] = builder.ToString();
            }
            else
            {
                xmlStrings[5] = "";
            }

            if (_metaData.SelectorManager.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.SelectorManager.Write(writer);
                // the seventh string is a xml string for selectors
                xmlStrings[6] = builder.ToString();
            }
            else
            {
                xmlStrings[6] = "";
            }

            if (_metaData.EventManager.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.EventManager.Write(writer);
                // the eighth string is a xml string for events
                xmlStrings[7] = builder.ToString();
            }
            else
            {
                xmlStrings[7] = "";
            }

            if (_metaData.LoggingPolicy.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.LoggingPolicy.Write(writer);
                // the eighth string is a xml string for logging policy
                xmlStrings[8] = builder.ToString();
            }
            else
            {
                xmlStrings[8] = "";
            }

            if (_metaData.SubscriberManager.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.SubscriberManager.Write(writer);
                // the eighth string is a xml string for subscribers
                xmlStrings[9] = builder.ToString();
            }
            else
            {
                xmlStrings[9] = "";
            }

            if (_metaData.XMLSchemaViews.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.XMLSchemaViews.Write(writer);
                // the eleventh string is a xml string for xml schema views
                xmlStrings[10] = builder.ToString();
            }
            else
            {
                xmlStrings[10] = "";
            }

            if (_metaData.ApiManager.IsAltered)
            {
                builder = new StringBuilder();
                writer = new StringWriter(builder);
                _metaData.ApiManager.Write(writer);
                // the eighth string is a xml string for apis
                xmlStrings[11] = builder.ToString();
            }
            else
            {
                xmlStrings[11] = "";
            }

            _isRequestComplete = false;

            // invoke the web service asynchronously
            DateTime modifiedTime = ((DesignStudio)this.MdiParent).MetaDataService.SetMetaData(
                ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo, isSafeMode),
                xmlStrings);

            SetMetaDataDone(modifiedTime);
        }

		/// <summary>
		/// Perform the action of fix any discrepancies in the database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void FixDatabase(object sender, System.EventArgs e)
		{
			LoginDialog dialog = new LoginDialog();

			if (dialog.ShowDialog() == DialogResult.OK)
			{
                // display a text in the status bar
                ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.FixDatabase"));

                _isRequestComplete = false;

                // invoke the web service asynchronously
                ((DesignStudio)this.MdiParent).MetaDataService.FixSchemaModel(
                    ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo));

                // set the status back to ready message
                ((DesignStudio)this.MdiParent).ShowReadyStatus();

                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.FixDatabaseDone"));
            }
		}

		/// <summary>
		/// Load and Show a log of updating a meta data in database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void ShowUpdateLog(object sender, System.EventArgs e)
		{
			// display a text in the status bar
            ((DesignStudio)this.MdiParent).ShowWorkingStatus(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LoadingUpdateLog"));

            try
            {
                // invoke the web service synchronously
                string log =  ((DesignStudio)this.MdiParent).MetaDataService.GetMetaDataUpdateLog(
                    ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo));

                UpdateLogDialog dialog = new UpdateLogDialog();

                dialog.SetUpdateLog(log);

                dialog.ShowDialog();
            }
            finally
            {
                // set the status back to ready message
                ((DesignStudio)this.MdiParent).ShowReadyStatus();
            }
		}

		/// <summary>
		/// Save the currently editing meta data to a file that the users pick.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void SaveAsMetaDataToFile(object sender, System.EventArgs e)
		{
			_schemaFileName = GetSaveFileName();
		
			if (_schemaFileName != null)
			{
				SaveMetaData();
			}
		}

        /// <summary>
        /// Save the selected xml schema to a file that the users pick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SaveAsXMLSchemaToFile(object sender, System.EventArgs e)
        {
            string xmlSchemaFileName = GetSaveXMLSchemaFileName();

            if (xmlSchemaFileName != null)
            {
                SaveXMLSchema(xmlSchemaFileName);
            }
        }

		/// <summary>
		/// Add an item as a child to the selected tree node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void AddNewItem(object sender, System.EventArgs e)
		{
			// Create an instance of AddNewItemDialog
			AddNewItemDialog dialog = new AddNewItemDialog();

			dialog.MetaDataModel = _metaData;

			// Set the currently selected tree node
			dialog.SelectedTreeNode = (MetaDataTreeNode) this.treeView1.SelectedNode;

			// Display the dialog
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				DoAddNewItem(dialog.SelectedTreeNode, dialog.NewMetaDataElement);
			}		
		}

		/// <summary>
		/// Delete an item which is currently being selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void DeleteItem(object sender, System.EventArgs e)
		{
			MetaDataTreeNode treeNode = null;

            if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.DeleteItem"),
				"Confirm Dialog", MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.Yes)
			{
				try
				{
					MetaDataListViewItem listItem = null;

					if (this.listView1.SelectedItems.Count == 1)
					{
						// delete the node selected in the list view
						listItem = (MetaDataListViewItem) this.listView1.SelectedItems[0];

						// remove the corresponding tree node from tree view
						treeNode = listItem.TreeNode;
					}
					else
					{
						// delete the node selected in the tree view
						treeNode = (MetaDataTreeNode) this.treeView1.SelectedNode;
					}
				
					// delete the corresponding meta data element from the meta data
					DeleteMetaDataElement(treeNode);

					if (listItem != null)
					{
						// remove the item from the list view
						this.listView1.Items.Remove(listItem);
					}

					this.treeView1.BeginUpdate();

					TreeNode parent = treeNode.Parent;

					// clear the selection to avoid AfterSelect event of tree view to be
					// fired when removing the currently selected tree node
					this.treeView1.SelectedNode = null;

					parent.Nodes.Remove(treeNode);

					// switch the selection to its parent
					this.treeView1.SelectedNode = parent;

					if (treeNode.MetaDataElement is RelationshipAttributeElement)
					{
						// remove the tree node for the backward relationship attribute
						// at the linked class side
						RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement) treeNode.MetaDataElement;

						TreeNode backwardRelationshipTreeNode = _treeBuilder.GetTreeNode(relationshipAttribute.BackwardRelationship);
						parent = backwardRelationshipTreeNode.Parent;
						parent.Nodes.Remove(backwardRelationshipTreeNode);
					}

					// reset the display position of remaining children
					int position = 0;
					foreach (MetaDataTreeNode metaDataTreeNode in parent.Nodes)
					{
                        // some tree node, like CategoryNode, doesn't have a MetaDataElement
                        if (metaDataTreeNode.MetaDataElement != null)
                        {
                            metaDataTreeNode.MetaDataElement.Position = position++;
                        }
					}

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
		/// Arrange order of children items of the selected tree node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void ArrangeItems(object sender, System.EventArgs e)
		{
			// Create an instance of ArrangeElementDialog
			ArrangeElementDialog dialog = new ArrangeElementDialog();

			// Set the currently selected tree node
			MetaDataTreeNode selectedTreeNode = (MetaDataTreeNode) this.treeView1.SelectedNode;

			dialog.SelectedTreeNode = selectedTreeNode;

			// Display the dialog
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				// show the arranged items in the list view
				this.listView1.BeginUpdate();

				_listViewBuilder.BuildItems(this.listView1, selectedTreeNode);

				this.listView1.EndUpdate();

				// adjust the display position of tree nodes
				ArrayList childNodes = new ArrayList();
				foreach (MetaDataTreeNode child in selectedTreeNode.Nodes)
				{
					childNodes.Add(child);
				}

				// clear child tree nodes and add the child tree node based on display positions
				selectedTreeNode.Nodes.Clear();
				int index;
				foreach (MetaDataTreeNode child in childNodes)
				{
					index = 0;
					foreach (MetaDataTreeNode treeNode in selectedTreeNode.Nodes)
					{
                        // For node that serves as Folder Node, it doesn't have an associated
                        // MetaDataElement, we will look as the position of its first child node that
                        // has an associated MetaDataElement
                        int position1 = GetPosition(treeNode);
                        int position2 = GetPosition(child);
                        if (position1 > position2)
						{
							break;
						}
						else
						{
							index++;
						}
					}

					if (index < selectedTreeNode.Nodes.Count)
					{
						selectedTreeNode.Nodes.Insert(index, child);
					}
					else
					{
						selectedTreeNode.Nodes.Add(child);
					}
				}
			}		
		}

        /// <summary>
        /// Gets position of a tree node.
        /// For node that serves as Folder Node, it doesn't have an associated
        /// MetaDataElement, we will look as the position of its first child node that
        /// has an associated MetaDataElement
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns>Position</returns>
        private int GetPosition(MetaDataTreeNode treeNode)
        {
            int position = 0;

            if (treeNode.MetaDataElement != null)
            {
                position = treeNode.MetaDataElement.Position;
            }
            else if (treeNode.Nodes.Count > 0)
            {
                position = GetPosition((MetaDataTreeNode)treeNode.Nodes[0]);
            }

            return position;
        }

		/// <summary>
		/// Modify an item which is currently being selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void ModifyItem(object sender, System.EventArgs e)
		{
			MetaDataTreeNode treeNode = null;
			MetaDataListViewItem listItem = null;

			if (this.listView1.SelectedItems.Count == 1)
			{
				// get the node selected in the list view
				listItem = (MetaDataListViewItem) this.listView1.SelectedItems[0];

				// get the corresponding tree node from tree view
				treeNode = listItem.TreeNode;
			}
			else
			{
				// get the node selected in the tree view
				treeNode = (MetaDataTreeNode) this.treeView1.SelectedNode;
			}

            if (treeNode.Type == Newtera.WinClientCommon.NodeType.DataViewNode)
			{
				CreateDataViewDialog dialog = new CreateDataViewDialog();
				dialog.MetaData = this._metaData;
                DataViewModel dataView = (DataViewModel) treeNode.MetaDataElement;
                if (dataView != null)
                {
                    // make a clone
                    dataView = dataView.Clone();
                    dataView.SchemaModel = this._metaData.SchemaModel;
                    dataView.SchemaInfo = this._metaData.SchemaInfo;
                }
				dialog.DataView = dataView;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					// the caption of the data view may have been changed
					treeNode.Text = dialog.DataView.Caption;
					if (listItem != null)
					{
						listItem.Text = dialog.DataView.Caption;
					}

                    // replace the data view with modified one
                    this._metaData.DataViews.Add(dialog.DataView);
                    this._metaData.DataViews.Remove((DataViewModel)treeNode.MetaDataElement);
                    treeNode.MetaDataElement = dialog.DataView;
				}
			}
            else if (treeNode.Type == Newtera.WinClientCommon.NodeType.SelectorNode)
			{
				SpecifySelectorDialog selectorDialog = new SpecifySelectorDialog();
				selectorDialog.MetaData = _metaData;
				selectorDialog.Selector = (Selector) treeNode.MetaDataElement;;
				if (selectorDialog.ShowDialog() == DialogResult.OK)
				{
					// TODO, realize the changes to the selector made in dialog
				}
			}
            else if (treeNode.Type == Newtera.WinClientCommon.NodeType.XMLSchemaView)
            {
                CreateXMLSchemaViewDialog dialog = new CreateXMLSchemaViewDialog();
                dialog.MetaData = this._metaData;
                XMLSchemaModel model = (XMLSchemaModel)treeNode.MetaDataElement;
                if (model != null)
                {
                    // make a clone
                    model = model.Clone();
                }
                dialog.XMLSchemaModel = model;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // the caption of the xml schema model may have been changed
                    treeNode.Text = dialog.XMLSchemaModel.Caption;
                    if (listItem != null)
                    {
                        listItem.Text = dialog.XMLSchemaModel.Caption;
                    }

                    // replace the xml schema model with modified one
                    this._metaData.XMLSchemaViews.Add(dialog.XMLSchemaModel);
                    this._metaData.XMLSchemaViews.Remove((XMLSchemaModel)treeNode.MetaDataElement);
                    treeNode.MetaDataElement = dialog.XMLSchemaModel;
                }
            }
		}

        /// <summary>
		/// Update an item based on the definitions that describe the item, such as Taxonomy node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        internal void UpdateHierarchy(object sender, System.EventArgs e)
        {
            MetaDataTreeNode treeNode = null;
            MetaDataListViewItem listItem = null;

            if (this.listView1.SelectedItems.Count == 1)
            {
                // get the node selected in the list view
                listItem = (MetaDataListViewItem)this.listView1.SelectedItems[0];

                // get the corresponding tree node from tree view
                treeNode = listItem.TreeNode;
            }
            else
            {
                // get the node selected in the tree view
                treeNode = (MetaDataTreeNode)this.treeView1.SelectedNode;
            }

            if (treeNode.MetaDataElement is ITaxonomy)
            {
                // only node of ITaxonomy type support auto-generating hierarchy
                ITaxonomy node = (ITaxonomy)treeNode.MetaDataElement;

                // collect all the nodes under this node that have auto-generation hierarchy definitions
                _autoClassifyingNodes = new DataViewElementCollection();
                CollectAutoClassifyingNodes(node, _autoClassifyingNodes);

                GenerateHierarchy();
            }
        }

        /// <summary>
        /// To be invoked recursively to collect the nodes that have auto-generation hierarchy definitions
        /// </summary>
        /// <param name="node">The current node</param>
        private void CollectAutoClassifyingNodes(ITaxonomy node, DataViewElementCollection autoClassifyingNodes)
        {
            if (node.AutoClassifyDef.HasDefinition)
            {
                autoClassifyingNodes.Add((IDataViewElement)node);
            }
            else
            {
                foreach (ITaxonomy child in node.ChildrenNodes)
                {
                    CollectAutoClassifyingNodes(child, autoClassifyingNodes);
                }
            }
        }

        /// <summary>
        /// Generate a hierarchy based on the definition of auto generation
        /// </summary>
        private void GenerateHierarchy()
        {
            _isCancelled = false;

            // run the generating process on a worker thread since it may take a long time
            _createHierarchyMethod = new MethodInvoker(CreateHierarchies);

            _createHierarchyMethod.BeginInvoke(new AsyncCallback(CreateHierarchiesDone), null);

            _isRequestComplete = false;

            // launch a work in progress dialog
            ShowWorkingDialog(true, new MethodInvoker(CancelGeneratingHierarchy));
        }

        /// <summary>
        /// The AsyncCallback event handler for CreateHierarchy method.
        /// </summary>
        /// <param name="res">The result</param>
        private void CreateHierarchiesDone(IAsyncResult res)
        {
            try
            {
                _createHierarchyMethod.EndInvoke(res);

                ShowHierarchy();
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

        private delegate void ShowHierarchyDelegate();

        /// <summary>
        /// Show the generated hierarchy
        /// </summary>
        private void ShowHierarchy()
        {
            if (this.InvokeRequired == false)
            {
                // it is the UI thread, continue
                foreach (ITaxonomy node in _autoClassifyingNodes)
                {
                    MetaDataTreeNode treeRoot = FindTreeNode((MetaDataTreeNode)treeView1.SelectedNode, node);

                    // remove the children of the tree root, since we will add new ones
                    treeRoot.Nodes.Clear();

                    // Rebuild the tree to represent the hierarchy
                    _treeBuilder.BuildTaxonomyTree(treeRoot, node);

                    if (treeRoot == treeView1.SelectedNode)
                    {
                        // show the items under the selected node in the list view
                        this.listView1.BeginUpdate();

                        _listViewBuilder.BuildItems(this.listView1, treeRoot);

                        this.listView1.EndUpdate();
                    }
                }

                // show the error which occured during construction of the hierarchy
                if (_errorMessage != null)
                {
                    MessageBox.Show(_errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                ShowHierarchyDelegate showHierarchy = new ShowHierarchyDelegate(ShowHierarchy);

                this.BeginInvoke(showHierarchy, null);
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

        private void CreateHierarchies()
        {
            _errorMessage = null;

            try
            {
                foreach (ITaxonomy node in _autoClassifyingNodes)
                {
                    DataViewModel dataView = node.GetDataView(null);
                    if (dataView == null)
                    {
                        MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.NodeWithoutDataView"),
                                        "Info", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    node.AutoClassifyDef.DataView = dataView;
                    CreateHierarchy(node); // generate the hierarchy based on definition of the node
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Create a hierarchy based on the definitions
        /// </summary>
        private void CreateHierarchy(ITaxonomy root)
        {
            AutoClassifyDef autoClassifyDef = root.AutoClassifyDef;

            // check the size of returned result, confirm if the class has too many instances
            DataViewModel dataView = autoClassifyDef.DataView;

            string query = dataView.SearchQuery;
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
            _util = new HierarchyGenerationUtil(autoClassifyDef,
                _dataService,
                autoClassifyDef.DataView.Clone());

            // build a new tree from scratch
            root.ChildrenNodes.Clear();

            // grow the first level of child nodes
            CreateHierarchyLevel(autoClassifyDef, root, 0);
        }

        /// <summary>
        /// Create a collection of child nodes and add them to the parent node. This method is called
        /// recursively
        /// </summary>
        /// <param name="parentNode">The parent node of ITaxonomy type</param>
        /// <param name="childLevelIndex">The index of current child level, starting from 0 for first level of child and so on.</param>
        private void CreateHierarchyLevel(AutoClassifyDef autoClassifyDef, ITaxonomy parentNode, int childLevelIndex)
        {
            if (_isCancelled)
            {
                // the process is cancelled by the user
                throw new Exception(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.HierarchyGenerationCancelled"));
            }

            // first, get number of child nodes for this level. raise an error if the number is 
            // too big
            AutoClassifyLevel currentAutoLevelDef = autoClassifyDef.GetClassifyLevelAt(childLevelIndex);

            StringCollection distinctNodeValues = this._util.GetDistinctLevelValue(currentAutoLevelDef, childLevelIndex);

            // throw an exception when number of child nodes is too big
            if (distinctNodeValues.Count > HierarchyGenerationUtil.MaxChildNumber)
            {
                string path = autoClassifyDef.RootNodeCaption;
                for (int i = 0; i < childLevelIndex; i++)
                {
                    path += "/" + autoClassifyDef.GetClassifyLevelAt(i).NodeValue;
                }

                string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.TooManyChildNodes"), path, HierarchyGenerationUtil.MaxChildNumber);
                throw new Exception(msg);
            }

            // create a taxon node for each distinct values
            TaxonNode childNode;
            foreach (string childNodeValue in distinctNodeValues)
            {
                // create the child tree node and add to the parent node
                string nodeName = _util.CreateUniqueTaxonName(parentNode);
                childNode = new TaxonNode(nodeName);
                childNode.Caption = childNodeValue;
                parentNode.ChildrenNodes.Add(childNode);
                childNode.ParentNode = parentNode;
                childNode.BaseUrl = currentAutoLevelDef.BaseUrl;
                childNode.SearchFilter = _util.CreateSearchFilter(currentAutoLevelDef, childNodeValue);

                // grow the child node by calling CreateTreeLevel method recursivly
                if (childLevelIndex < (autoClassifyDef.AutoClassifyLevels.Count - 1))
                {
                    currentAutoLevelDef.NodeValue = childNodeValue;
                    CreateHierarchyLevel(autoClassifyDef, childNode, childLevelIndex + 1);
                }
            }
        }

        /// <summary>
        /// Define the hierarchical structure for a Taxonomy node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void DefineHierarchy(object sender, System.EventArgs e)
        {
            MetaDataTreeNode treeNode = null;
            MetaDataListViewItem listItem = null;

            if (this.listView1.SelectedItems.Count == 1)
            {
                // get the node selected in the list view
                listItem = (MetaDataListViewItem)this.listView1.SelectedItems[0];

                // get the corresponding tree node from tree view
                treeNode = listItem.TreeNode;
            }
            else
            {
                // get the node selected in the tree view
                treeNode = (MetaDataTreeNode)this.treeView1.SelectedNode;
            }

            if (treeNode.MetaDataElement is ITaxonomy)
            {
                // only node of ITaxonomy type support auto-generating hierarchy
                ITaxonomy node = (ITaxonomy)treeNode.MetaDataElement;

                DataViewModel dataView = node.GetDataView(null);
                if (dataView == null)
                {
                    MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.NodeWithoutDataView"),
                                    "Info", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                // clone the exitsing auto classifying definition so that the changes made
                // in the dialog won't affect the original one
                AutoClassifyDef autoClassifyDef = node.AutoClassifyDef.Clone();
                autoClassifyDef.DataView = dataView;
                autoClassifyDef.RootNodeCaption = ((IDataViewElement)node).Caption;

                DefineAutoHierarchyDialog dialog = new DefineAutoHierarchyDialog(autoClassifyDef);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    node.AutoClassifyDef = dialog.AutoClassifyDef;
                }
            }
        }

		/// <summary>
		/// Refresh the meta-data model tree display.
		/// </summary>
		internal void RefreshMetaDataTree()
		{
			InitializeMenuItemStates();

			TreeNode root;
			if (_metaData != null)
			{
				root = _treeBuilder.BuildTree(_metaData);
			}
			else
			{
				root = _treeBuilder.BuildTree();
				_metaData = new MetaDataModel();
                _metaData.SchemaModel.CategoryChanged += new EventHandler(this.CategoryChangedHandler);
				SetDefaultPermissions(_metaData); // Set the default permission
			}

			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			treeView1.Nodes.Add(root);
			// Make the ClassNode selected initially
			treeView1.SelectedNode = root.Nodes[0];
			treeView1.EndUpdate();

			// Register the menu item state change event handler
			_menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);
        }

        /// <summary>
        /// Relocate the tree node in the tree based on category of the class it represents.
        /// </summary>
        private void RelocateTreeNode(ClassElement theClass)
        {
            // refresh the display of the meta data tree to reflect the change of
            // a class category
            TreeNode root = _treeBuilder.BuildTree(_metaData);

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(root);

            // Make the treeNode representing the class selected initially
            // make the initial selection
            MetaDataTreeNode selectedNode = FindTreeNode(root, theClass);
            if (selectedNode != null)
            {
                treeView1.SelectedNode = selectedNode;
            }

            treeView1.EndUpdate();
        }

        private MetaDataTreeNode FindTreeNode(TreeNode root, IMetaDataElement classElement)
        {
            foreach (MetaDataTreeNode node in root.Nodes)
            {
                if (classElement == node.MetaDataElement)
                {
                    return node;
                }
                else
                {
                    MetaDataTreeNode found = FindTreeNode(node, classElement);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        private MetaDataTreeNode FindTreeNode(MetaDataTreeNode root, ITaxonomy node)
        {
            MetaDataTreeNode found = null;

            if (root.MetaDataElement == node)
            {
                found = root;
            }
            else
            {
                foreach (MetaDataTreeNode treeNode in root.Nodes)
                {
                    found = FindTreeNode(treeNode, node);
                    if (found != null)
                    {
                        break;
                    }
                }
            }

            return found;
        }

        #endregion Controller code

        #region Event handlers

        private void SchemaEditor_Load(object sender, System.EventArgs e)
		{
			RefreshMetaDataTree();
		}

		/// <summary>
		/// Initialize the menu item states
		/// </summary>
		private void InitializeMenuItemStates()
		{
			_menuItemStates.SetState(MenuItemID.EditAdd, false);
			_menuItemStates.SetState(MenuItemID.EditCopy, false);
			_menuItemStates.SetState(MenuItemID.EditCut, false);
			_menuItemStates.SetState(MenuItemID.EditModify, false);
			_menuItemStates.SetState(MenuItemID.EditDelete, false);
			_menuItemStates.SetState(MenuItemID.EditArrange, false);
			_menuItemStates.SetState(MenuItemID.EditPaste, false);
			_menuItemStates.SetState(MenuItemID.EditSearch, false);
            _menuItemStates.SetState(MenuItemID.EditUpdate, false);
            _menuItemStates.SetState(MenuItemID.EditDefine, false);
            _menuItemStates.SetState(MenuItemID.EditExport, false);
			_menuItemStates.SetState(MenuItemID.FileExport, false);
			_menuItemStates.SetState(MenuItemID.ViewChart, false);
            _menuItemStates.SetState(MenuItemID.ViewPivot, false);
			_menuItemStates.SetState(MenuItemID.FileSaveAsFile, true);
			_menuItemStates.SetState(MenuItemID.FileSaveFile, true);
			_menuItemStates.SetState(MenuItemID.ToolValidate, true);

            if (_isDBLoaded)
            {
                _menuItemStates.SetState(MenuItemID.ViewRefresh, true);
            }
            else
            {
                _menuItemStates.SetState(MenuItemID.ViewRefresh, false);
            }

			// enable SaveToDatabase button according to the lock status
			if (_metaData != null)
			{
				if (_metaData.IsLockObtained)
				{
                    // metadata is locked
					_menuItemStates.SetState(MenuItemID.FileSaveDatabase, true);
					_menuItemStates.SetState(MenuItemID.ToolLock, false);
                    _menuItemStates.SetState(MenuItemID.ToolUnlock, true);
				}
				else
				{
					_menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
                    if (IsDBAUser)
                    {
                        _menuItemStates.SetState(MenuItemID.ToolLock, true);
                    }
                    else
                    {
                        _menuItemStates.SetState(MenuItemID.ToolLock, false);
                    }
                    // enable unlock only for super user
                    CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                    if (principal != null && principal.IsInRole(Newtera.Common.Core.NewteraNameSpace.CM_SUPER_USER_ROLE))
                    {
                        _menuItemStates.SetState(MenuItemID.ToolUnlock, true);
                    }
                    else
                    {
                        _menuItemStates.SetState(MenuItemID.ToolUnlock, false);
                    }
				}
			}
			else
			{
				_menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
				_menuItemStates.SetState(MenuItemID.ToolLock, false);
				_menuItemStates.SetState(MenuItemID.ToolUnlock, false);
			}
		}

		private void MenuItemStateChanged(object sender, System.EventArgs e)
		{
			StateChangedEventArgs args = (StateChangedEventArgs) e;

			// set the toolbar button states
			switch (args.ID)
			{
				case MenuItemID.EditAdd:
					this.addMenuItem.Enabled = args.State;
					this.tvAddMenuItem.Enabled = args.State;
					this.lvAddMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditDelete:
					this.deleteMenuItem.Enabled = args.State;
					this.tvDeleteMenuItem.Enabled = args.State;
					this.lvDeleteMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditArrange:
					this.arrangeMenuItem.Enabled = args.State;
					this.tvArrangeMenuItem.Enabled = args.State;
					this.lvArrangeMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditModify:
					this.modifyMenuItem.Enabled = args.State;
					this.tvModifyMenuItem.Enabled = args.State;
					this.lvModifyMenuItem.Enabled = args.State;
					break;
                case MenuItemID.EditUpdate:
                    this.updateMenuItem.Enabled = args.State;
                    this.tvUpdateMenuItem.Enabled = args.State;
                    this.lvUpdateMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditDefine:
                    this.defineMenuItem.Enabled = args.State;
                    this.tvDefineMenuItem.Enabled = args.State;
                    this.lvDefineMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditExport:
                    this.tvExportMenuItem.Enabled = args.State;
                    break;
				case MenuItemID.FileSaveFile:
					this.saveMenuItem.Enabled = args.State;
					break;
				case MenuItemID.FileSaveDatabase:
					this.saveToDatabaseMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ToolValidate:
					this.validateMenuItem.Enabled = args.State;
					break;
				case MenuItemID.CollapseNode:
					this.tvCollapseMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ExpandNode:
					this.tvExpandMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ToolLock:
					this.lockMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ToolUnlock:
					this.unlockMenuItem.Enabled = args.State;
					break;
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

			switch (selectedNode.Type)
			{
                case Newtera.WinClientCommon.NodeType.SchemaInfoNode:
					this._menuItemStates.SetState(MenuItemID.EditAdd, false);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditDelete, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, false);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;
                case Newtera.WinClientCommon.NodeType.ClassNode:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, true);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;
                case Newtera.WinClientCommon.NodeType.ClassesFolder:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, false);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;
                case Newtera.WinClientCommon.NodeType.ConstraintsFolder:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, false);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;

                case Newtera.WinClientCommon.NodeType.CategoryFolder:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
                    this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

                    break;

                case Newtera.WinClientCommon.NodeType.TaxonomiesFolder:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, false);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;
                case Newtera.WinClientCommon.NodeType.TaxonomyNode:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, true);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, true);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, true);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;

                case Newtera.WinClientCommon.NodeType.TaxonNode:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, true);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, true);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, true);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;

                case Newtera.WinClientCommon.NodeType.DataViewsFolder:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, false);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;
                case Newtera.WinClientCommon.NodeType.RelationshipAttributeNode:
                case Newtera.WinClientCommon.NodeType.SimpleAttributeNode:
                case Newtera.WinClientCommon.NodeType.ArrayAttributeNode:
                case Newtera.WinClientCommon.NodeType.VirtualAttributeNode:
                case Newtera.WinClientCommon.NodeType.ImageAttributeNode:
                case Newtera.WinClientCommon.NodeType.EnumConstraintNode:
                case Newtera.WinClientCommon.NodeType.RangeConstraintNode:
                case Newtera.WinClientCommon.NodeType.PatternConstraintNode:
                case Newtera.WinClientCommon.NodeType.ListConstraintNode:
					this._menuItemStates.SetState(MenuItemID.EditAdd, false);
					this._menuItemStates.SetState(MenuItemID.EditDelete, true);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, false);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;

                case Newtera.WinClientCommon.NodeType.DataViewNode:
					this._menuItemStates.SetState(MenuItemID.EditAdd, false);
					this._menuItemStates.SetState(MenuItemID.EditDelete, true);
					this._menuItemStates.SetState(MenuItemID.EditModify, true);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, false);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;

                case Newtera.WinClientCommon.NodeType.SelectorsFolder:
					this._menuItemStates.SetState(MenuItemID.EditAdd, true);
					this._menuItemStates.SetState(MenuItemID.EditDelete, false);
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;

                case Newtera.WinClientCommon.NodeType.SelectorNode:
					this._menuItemStates.SetState(MenuItemID.EditAdd, false);
					this._menuItemStates.SetState(MenuItemID.EditDelete, true);
					this._menuItemStates.SetState(MenuItemID.EditModify, true);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
					this._menuItemStates.SetState(MenuItemID.EditArrange, false);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

					break;

                case Newtera.WinClientCommon.NodeType.XMLSchemaViewsFolder:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, true);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
                    this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

                    break;

                case Newtera.WinClientCommon.NodeType.XMLSchemaView:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                    this._menuItemStates.SetState(MenuItemID.EditModify, true);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
                    this._menuItemStates.SetState(MenuItemID.EditArrange, true);
                    this._menuItemStates.SetState(MenuItemID.EditExport, true);

                    break;

                case Newtera.WinClientCommon.NodeType.XMLSchemaComplexType:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
                    this._menuItemStates.SetState(MenuItemID.EditArrange, false);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

                    break;

                case Newtera.WinClientCommon.NodeType.XMLSchemaElement:
                    this._menuItemStates.SetState(MenuItemID.EditAdd, false);
                    this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                    this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
                    this._menuItemStates.SetState(MenuItemID.EditArrange, false);
                    this._menuItemStates.SetState(MenuItemID.EditExport, false);

                    break;
			}
		}

		/// <summary>
		/// Perform the action of adding a new item.
		/// </summary>
		/// <param name="selectedTreeNode">The selected tree node</param>
		/// <param name="metaDataElement">The meta data element to be added to the meta data model</param>
		private void DoAddNewItem(MetaDataTreeNode selectedTreeNode, IMetaDataElement metaDataElement)
		{
			bool isAdded = true;
			MetaDataType type = MetaDataType.Schema;
            ClassElement ownerClass;

			switch (selectedTreeNode.Type)
			{
                case Newtera.WinClientCommon.NodeType.ClassesFolder:
					// add the new item as a root class
					_metaData.SchemaModel.AddRootClass((ClassElement) metaDataElement);

					break;

                case Newtera.WinClientCommon.NodeType.CategoryFolder:

                    // Set the category to the new class element
                    ClassElement newClassElement = (ClassElement)metaDataElement;

                    MetaDataTreeNode parentNode = (MetaDataTreeNode)selectedTreeNode.Parent;
                    if (parentNode.Type == Newtera.WinClientCommon.NodeType.ClassesFolder)
                    {
                        // add the new class as a root class
                        _metaData.SchemaModel.AddRootClass(newClassElement);
                    }
                    else if (parentNode.Type == Newtera.WinClientCommon.NodeType.ClassNode)
                    {
                        // then it is a class tree node
                        // add a new class as a child to the class represented by the tree node
                        ownerClass = (ClassElement) parentNode.MetaDataElement;

                        if (ownerClass.IsLeaf && !IsClassEmpty(ownerClass))
                        {
                            // do not allow adding a subclass to a leaf class that already
                            // has instances
                            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InvalidSubclass"),
                                "Info", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            isAdded = false;
                        }
                        else
                        {
                            // no errors found, the meta model is valid
                            ownerClass.AddSubclass(newClassElement);
                        }
                    }

                    if (isAdded)
                    {
                        // set the category to the class element
                        newClassElement.Category = selectedTreeNode.Text;
                    }

                    break;

                case Newtera.WinClientCommon.NodeType.ClassNode:
			        ownerClass = (ClassElement) selectedTreeNode.MetaDataElement;
					if (metaDataElement is SimpleAttributeElement)
					{
						ownerClass.AddSimpleAttribute((SimpleAttributeElement) metaDataElement);
					}
					else if (metaDataElement is ArrayAttributeElement)
					{
						SpecifyArraySizeDialog dialog = new SpecifyArraySizeDialog();
						if (dialog.ShowDialog() == DialogResult.OK)
						{
							ArrayAttributeElement arrayAttribute = (ArrayAttributeElement) metaDataElement;
							arrayAttribute.ArraySize = dialog.ArraySize;
							ownerClass.AddArrayAttribute(arrayAttribute);
						}
						else
						{
							isAdded = false;
						}
					}
                    else if (metaDataElement is VirtualAttributeElement)
                    {
                        ownerClass.AddVirtualAttribute((VirtualAttributeElement)metaDataElement);
                    }
                    else if (metaDataElement is ImageAttributeElement)
                    {
                        ownerClass.AddImageAttribute((ImageAttributeElement)metaDataElement);
                    }
					else if (metaDataElement is RelationshipAttributeElement)
					{
						// set the rest of information for the relationship and create
						// a backward relationship automatically on the linked class
						SpecifyRelationshipDialog dialog = new SpecifyRelationshipDialog();
						dialog.MetaData = _metaData;
						dialog.RelationshipAttribute = (RelationshipAttributeElement) metaDataElement;
						dialog.RelationshipAttribute.OwnerClass = ownerClass;
						if (dialog.ShowDialog() == DialogResult.OK)
						{
							// add the relationship to the owner class
							ownerClass.AddRelationshipAttribute((RelationshipAttributeElement) metaDataElement);
						}
						else
						{
							isAdded = false;
						}
					}
					else if (metaDataElement is ClassElement)
					{
                        if (ownerClass.IsLeaf && !IsClassEmpty(ownerClass))
                        {
                            // do not allow adding a subclass to a leaf class that already
                            // has instances
                            // no errors found, the meta model is valid
                            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InvalidSubclass"),
                                "Info", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            isAdded = false;
                        }
                        else
                        {
                            ownerClass.AddSubclass((ClassElement)metaDataElement);
                        }
					}

					break;
                case Newtera.WinClientCommon.NodeType.ConstraintsFolder:
					
					if (metaDataElement is EnumElement)
					{
						_metaData.SchemaModel.AddEnumConstraint((EnumElement) metaDataElement);
					}
					else if (metaDataElement is RangeElement)
					{
						_metaData.SchemaModel.AddRangeConstraint((RangeElement) metaDataElement);
					}
					else if (metaDataElement is PatternElement)
					{
						_metaData.SchemaModel.AddPatternConstraint((PatternElement) metaDataElement);
					}
					else if (metaDataElement is ListElement)
					{
						_metaData.SchemaModel.AddListConstraint((ListElement) metaDataElement);
					}

					break;

                case Newtera.WinClientCommon.NodeType.TaxonomiesFolder:
					// add a new taxonomy
					_metaData.Taxonomies.Add((TaxonomyModel) metaDataElement);
					((TaxonomyModel) metaDataElement).ParentNode = _metaData.Taxonomies;

					break;

                case Newtera.WinClientCommon.NodeType.TaxonomyNode:
					// add a new Taxon node as child
					TaxonomyModel taxonomy = (TaxonomyModel) selectedTreeNode.MetaDataElement;
					taxonomy.ChildrenNodes.Add((TaxonNode) metaDataElement);
					((TaxonNode) metaDataElement).ParentNode = taxonomy;

					break;

                case Newtera.WinClientCommon.NodeType.TaxonNode:
					// add a new Taxon node as child
					TaxonNode parent = (TaxonNode) selectedTreeNode.MetaDataElement;
					parent.ChildrenNodes.Add((TaxonNode) metaDataElement);
					((TaxonNode) metaDataElement).ParentNode = parent;

					break;

                case Newtera.WinClientCommon.NodeType.DataViewsFolder:

					// Set content of a dataview through a dialog that makes it easy
					// to specify a data view
					DataViewModel dataView = (DataViewModel) metaDataElement;
					CreateDataViewDialog dvDialog = new CreateDataViewDialog();
					dvDialog.MetaData = _metaData;
					dvDialog.DataView = dataView;
					if (dvDialog.ShowDialog() == DialogResult.OK)
					{
						// add a new data view
						_metaData.DataViews.Add(dataView);					
					}
					else
					{
						isAdded = false;
					}

					break;

                case Newtera.WinClientCommon.NodeType.SelectorsFolder:

					// Set content of a selector through a dialog that is specialized
					// for selector
					Selector selector = (Selector) metaDataElement;
					SpecifySelectorDialog selectorDialog = new SpecifySelectorDialog();
					selectorDialog.MetaData = _metaData;
					selectorDialog.Selector = selector;
					if (selectorDialog.ShowDialog() == DialogResult.OK)
					{
						// add the new selector
						_metaData.SelectorManager.AddSelector(selector);
					}
					else
					{
						isAdded = false;
					}

					break;

                case Newtera.WinClientCommon.NodeType.XMLSchemaViewsFolder:
 
                    // Set content of a xml schema view through a dialog that makes it easy
                    // to specify a xml schema view
                    XMLSchemaModel xmlSchemaModel = (XMLSchemaModel)metaDataElement;
                    CreateXMLSchemaViewDialog svDialog = new CreateXMLSchemaViewDialog();
                    svDialog.MetaData = _metaData;
                    svDialog.XMLSchemaModel = xmlSchemaModel;
                    if (svDialog.ShowDialog() == DialogResult.OK)
                    {
                        // add a new xml schema view
                        _metaData.XMLSchemaViews.Add(xmlSchemaModel);
                    }
                    else
                    {
                        isAdded = false;
                    }

                    type = MetaDataType.XMLSchemaViews;

                    break;
			}

			if (isAdded)
			{
                // Add the new node to the tree view and list view
                MetaDataTreeNode newNode = _treeBuilder.CreateTreeNode(metaDataElement);

                ListViewItem item = _listViewBuilder.BuildItem(newNode);
                item.Selected = true;

				switch (type)
				{
					case MetaDataType.Schema:
						// mark the schema model as altered
						_metaData.SchemaModel.IsAltered = true;

						if (metaDataElement is RelationshipAttributeElement)
						{
							RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement) metaDataElement;

							// add a tree node for the backward relationship attribute too
							TreeNode backwardRelationshipNode = _treeBuilder.CreateTreeNode(relationshipAttribute.BackwardRelationship);
							TreeNode linkedClassNode = _treeBuilder.GetTreeNode(relationshipAttribute.LinkedClass);
							linkedClassNode.Nodes.Add(backwardRelationshipNode);
						}
						break;
					case MetaDataType.Taxonomies:
						// mark taxonomy as altered
						_metaData.Taxonomies.IsAltered = true;
						break;

                    case MetaDataType.XMLSchemaViews:

                        // create a new tree node with children nodes
                        newNode = (MetaDataTreeNode)_treeBuilder.BuildXMLSchemaTree((XMLSchemaModel)metaDataElement);

                        break;
				}

				// add the new tree node to a position in tree view, a list view item
				// to list view, based on type of metaDataElement
				AddChildNode(selectedTreeNode, newNode, item);

				this.listView1.Focus();
			}
		}

		/// <summary>
		/// Delete the meta data element (contained by the tree node) from
		/// the meta data.
		/// </summary>
		/// <param name="treeNode">The tree node that contains the meta data element.</param>
		private void DeleteMetaDataElement(MetaDataTreeNode treeNode)
		{
			ClassElement ownerClass;
            string caption;

			switch (treeNode.Type)
			{
                case Newtera.WinClientCommon.NodeType.ClassNode:
					ClassElement classElement = (ClassElement) treeNode.MetaDataElement;

					// Make sure it is an empty class.
					if (!IsClassEmpty(classElement))
					{
                        throw new Exception(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.UnableDeleteClass"));
					}

					// make sure the class has not been referenced by taxonomies or
					// data views
                    if (_metaData.DataViews.IsClassReferenced(classElement.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ClassReferencedByDataView"), caption);
                        throw new Exception(msg);
                    }
                    
                    if (this._metaData.Taxonomies.IsClassReferenced(classElement.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ClassReferencedByTaxonomy"), caption);
                        throw new Exception(msg);
                    }

                    // make sure that all subclasses have been removed
                    if (classElement.Subclasses != null && classElement.Subclasses.Count > 0)
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.SubclassesExist"), caption);
                        throw new Exception(msg);
                    }

                    // make sure that all relationsnhips of the class have been removed
                    if (classElement.RelationshipAttributes != null &&
                        classElement.RelationshipAttributes.Count > 0)
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.RelationshipsExist"), caption);
                        throw new Exception(msg);
                    }

					if (classElement.IsRoot)
					{
						_metaData.SchemaModel.RemoveRootClass(classElement);
					}
					else
					{
						classElement.ParentClass.RemoveSubclass(classElement);
					}

                    // Remove events associated with the class
                    _metaData.EventManager.RemoveEvents(classElement);

                    // Remove subscribers associated with the class
                    _metaData.SubscriberManager.RemoveSubscribers(classElement);

                    // Remove apis associated with the class
                    _metaData.ApiManager.RemoveApis(classElement.Name);

                    // Remove subscribers associated with the class
                    _metaData.SubscriberManager.RemoveSubscribers(classElement);

                    break;
				case Newtera.WinClientCommon.NodeType.SimpleAttributeNode:

                    SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)treeNode.MetaDataElement;
                    ownerClass = simpleAttribute.OwnerClass;

                    // make sure the attribute has not been referenced by taxonomies or
                    // data views
                    if (_metaData.DataViews.IsAttributeReferenced(ownerClass.Name, simpleAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByDataView"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.Taxonomies.IsAttributeReferenced(ownerClass.Name, simpleAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByTaxonomy"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.EventManager.IsAttributeReferenced(ownerClass.Name, simpleAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByEvent"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.RuleManager.IsAttributeReferenced(ownerClass.Name, simpleAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByRules"), caption);
                        throw new Exception(msg);
                    }

                    // if the attribute is used in default sorting, clear the sorting
                    if (simpleAttribute.OwnerClass.SortAttribute == simpleAttribute.Name)
                    {
                        simpleAttribute.OwnerClass.SortAttribute = null;
                    }

                    ownerClass.RemoveSimpleAttribute((SimpleAttributeElement)treeNode.MetaDataElement);

					break;
                case Newtera.WinClientCommon.NodeType.RelationshipAttributeNode:
					RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement) treeNode.MetaDataElement;
					ownerClass = relationshipAttribute.OwnerClass;

                    // make sure the relationship has not been referenced by taxonomies or
                    // data views
                    if (_metaData.DataViews.IsAttributeReferenced(ownerClass.Name, relationshipAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByDataView"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.Taxonomies.IsAttributeReferenced(ownerClass.Name, relationshipAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByTaxonomy"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.RuleManager.IsAttributeReferenced(ownerClass.Name, relationshipAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByRules"), caption);
                        throw new Exception(msg);
                    }

                    // make sure the backward relationship has not been referenced by taxonomies or
                    // data views
                    ClassElement linkedClass = relationshipAttribute.LinkedClass;
                    if (_metaData.DataViews.IsAttributeReferenced(linkedClass.Name, relationshipAttribute.BackwardRelationship.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByDataView"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.Taxonomies.IsAttributeReferenced(linkedClass.Name, relationshipAttribute.BackwardRelationship.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByTaxonomy"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.RuleManager.IsAttributeReferenced(linkedClass.Name, relationshipAttribute.BackwardRelationship.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByRules"), caption);
                        throw new Exception(msg);
                    }

					ownerClass.RemoveRelationshipAttribute(relationshipAttribute);
					// remove the backward relationship too
					linkedClass.RemoveRelationshipAttribute(relationshipAttribute.BackwardRelationship);
				
					break;
                case Newtera.WinClientCommon.NodeType.ArrayAttributeNode:
                    ArrayAttributeElement arrayAttribute = (ArrayAttributeElement)treeNode.MetaDataElement;
                    ownerClass = arrayAttribute.OwnerClass;

                    // make sure the attribute has not been referenced by taxonomies or
                    // data views
                    if (_metaData.DataViews.IsAttributeReferenced(ownerClass.Name, arrayAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByDataView"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.Taxonomies.IsAttributeReferenced(ownerClass.Name, arrayAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByTaxonomy"), caption);
                        throw new Exception(msg);
                    }


                    ownerClass.RemoveArrayAttribute(arrayAttribute);

					break;
                case Newtera.WinClientCommon.NodeType.VirtualAttributeNode:
                    VirtualAttributeElement virtualAttribute = (VirtualAttributeElement)treeNode.MetaDataElement;
                    ownerClass = virtualAttribute.OwnerClass;

                    // make sure the attribute has not been referenced by taxonomies or
                    // data views
                    if (_metaData.DataViews.IsAttributeReferenced(ownerClass.Name, virtualAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByDataView"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.Taxonomies.IsAttributeReferenced(ownerClass.Name, virtualAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByTaxonomy"), caption);
                        throw new Exception(msg);
                    }
                    ownerClass.RemoveVirtualAttribute(virtualAttribute);
                    break;
                case Newtera.WinClientCommon.NodeType.ImageAttributeNode:
                    ImageAttributeElement imageAttribute = (ImageAttributeElement)treeNode.MetaDataElement;
                    ownerClass = imageAttribute.OwnerClass;

                    // make sure the attribute has not been referenced by taxonomies or
                    // data views
                    if (_metaData.DataViews.IsAttributeReferenced(ownerClass.Name, imageAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByDataView"), caption);
                        throw new Exception(msg);
                    }

                    if (this._metaData.Taxonomies.IsAttributeReferenced(ownerClass.Name, imageAttribute.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.AttributeReferencedByTaxonomy"), caption);
                        throw new Exception(msg);
                    }
                    ownerClass.RemoveImageAttribute(imageAttribute);
                    break;
                case Newtera.WinClientCommon.NodeType.EnumConstraintNode:
					_metaData.SchemaModel.RemoveEnumConstraint(treeNode.MetaDataElement.Name);
					break;
                case Newtera.WinClientCommon.NodeType.RangeConstraintNode:
					_metaData.SchemaModel.RemoveRangeConstraint(treeNode.MetaDataElement.Name);
					break;
                case Newtera.WinClientCommon.NodeType.PatternConstraintNode:
					_metaData.SchemaModel.RemovePatternConstraint(treeNode.MetaDataElement.Name);
					break;
                case Newtera.WinClientCommon.NodeType.ListConstraintNode:
					_metaData.SchemaModel.RemoveListConstraint(treeNode.MetaDataElement.Name);
					break;
                case Newtera.WinClientCommon.NodeType.TaxonomyNode:
					_metaData.Taxonomies.Remove((TaxonomyModel) treeNode.MetaDataElement);
					break;
                case Newtera.WinClientCommon.NodeType.TaxonNode:
					TaxonNode taxon = (TaxonNode) treeNode.MetaDataElement;
					ITaxonomy parent = taxon.ParentNode;
					parent.ChildrenNodes.Remove(taxon);
					break;
                case Newtera.WinClientCommon.NodeType.DataViewNode:

					DataViewModel dataView = (DataViewModel) treeNode.MetaDataElement;

					// make sure the data view has not been referenced by any taxonomies
                    if (this._metaData.Taxonomies.IsDataViewReferenced(dataView.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.DataViewReferenced"), caption);
                        throw new Exception(msg);
                    }

                    // make sure the data view has not been referenced by any class elements
                    if (this._metaData.SchemaModel.IsDataViewReferenced(dataView.Name, out caption))
                    {
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.DataViewReferencedByClass"), caption);
                        throw new Exception(msg);
                    }

					_metaData.DataViews.Remove(dataView);
					break;
                case Newtera.WinClientCommon.NodeType.SelectorNode:
					Selector selector = (Selector) treeNode.MetaDataElement;
					_metaData.SelectorManager.RemoveSelector(selector);
					break;
                case Newtera.WinClientCommon.NodeType.XMLSchemaView:
                    _metaData.XMLSchemaViews.Remove((XMLSchemaModel)treeNode.MetaDataElement);
                    break;
			}

			// remove the schema model element to tree node mapping
			if (treeNode.MetaDataElement != null)
			{
				_treeBuilder.RemoveMapping(treeNode.MetaDataElement);
			}
		}

		private string GetSaveFileName()
		{
			string fileName = null;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
 
			saveFileDialog.InitialDirectory = "c:\\" ;
			saveFileDialog.Filter = "schema files (*.schema)|*.schema"  ;
			saveFileDialog.FilterIndex = 1 ;
			saveFileDialog.RestoreDirectory = false;
 
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				fileName = saveFileDialog.FileName;
				if (!fileName.EndsWith(".schema"))
				{
					fileName += ".schema";
				}
			}

			return fileName;
		}

        private string GetSaveXMLSchemaFileName()
        {
            string fileName = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "schema files (*.xsd)|*.xsd";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = false;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                if (!fileName.EndsWith(".xsd"))
                {
                    fileName += ".xsd";
                }
            }

            return fileName;
        }

		private Newtera.Common.Core.SchemaInfo GetSaveSchemaInfo(out bool isExistingSchema)
		{
			Newtera.Common.Core.SchemaInfo schemaInfo = null;
			SaveToDatabaseDialog saveToDatabaseDialog = new SaveToDatabaseDialog();
			saveToDatabaseDialog.Owner = this.MdiParent; // allow to access status bar
            isExistingSchema = false;
 
			if (saveToDatabaseDialog.ShowDialog() == DialogResult.OK)
			{
				// Note, we have to copy the values because webservice
				// created its own SchemaInfo type which is different from
				// the one defined in Newtera.Common.Core
				schemaInfo = new Newtera.Common.Core.SchemaInfo();
				schemaInfo.ID = saveToDatabaseDialog.SelectedSchema.ID;
				schemaInfo.Name = saveToDatabaseDialog.SelectedSchema.Name;
				schemaInfo.Version = saveToDatabaseDialog.SelectedSchema.Version;
                schemaInfo.ModifiedTime = saveToDatabaseDialog.SelectedSchema.ModifiedTime;
                isExistingSchema = saveToDatabaseDialog.IsExistingSchema;
			}

			return schemaInfo;
		}

		private void SaveMetaData()
		{
			try
			{
				_metaData.SchemaModel.Write(_schemaFileName);
				string xaclPolicyFileName = _schemaFileName.Replace(".schema", ".xacl");
				_metaData.XaclPolicy.Write(xaclPolicyFileName);
				string taxonomyFileName = _schemaFileName.Replace(".schema", ".taxonomy");
				_metaData.Taxonomies.Write(taxonomyFileName);
				string dataViewFileName = _schemaFileName.Replace(".schema", ".dataview");
				_metaData.DataViews.Write(dataViewFileName);
				string ruleFileName = _schemaFileName.Replace(".schema", ".rules");
				_metaData.RuleManager.Write(ruleFileName);
				string mappingFileName = _schemaFileName.Replace(".schema", ".mappings");
				_metaData.MappingManager.Write(mappingFileName);
				string selectorFileName = _schemaFileName.Replace(".schema", ".selectors");
				_metaData.SelectorManager.Write(selectorFileName);
                string eventFileName = _schemaFileName.Replace(".schema", ".events");
                _metaData.EventManager.Write(eventFileName);
                string loggingFileName = _schemaFileName.Replace(".schema", ".logging");
                _metaData.LoggingPolicy.Write(loggingFileName);
                string subscriberFileName = _schemaFileName.Replace(".schema", ".subscribers");
                _metaData.SubscriberManager.Write(subscriberFileName);
                string xmlSchemaViewsFileName = _schemaFileName.Replace(".schema", ".schemaviews");
                _metaData.XMLSchemaViews.Write(xmlSchemaViewsFileName);
                string apiFileName = _schemaFileName.Replace(".schema", ".apis");
                _metaData.ApiManager.Write(apiFileName);
                _metaData.NeedToSave = false;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

        private void SaveXMLSchema(string schemaFileName)
        {
            try
            {
                MetaDataTreeNode treeNode = null;

                // get the node selected in the tree view
                treeNode = (MetaDataTreeNode)this.treeView1.SelectedNode;
                if (treeNode != null && treeNode.MetaDataElement is XMLSchemaModel)
                {
                    XMLSchemaModel xmlSchemaModel = treeNode.MetaDataElement as XMLSchemaModel;
                    xmlSchemaModel.Write(schemaFileName);
                }
                else
                {
                    throw new Exception("Unable to get a XMLSchemaModel object from the selected tree node");
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.StackTrace);
            }
        }

		/// <summary>
		/// Validate the meta model
		/// </summary>
		internal void ValidateMetaModel()
		{
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                ValidateResult result = _metaData.SchemaModel.Validate();

                // verify if the result contains some doubts, given that the schema
                // has been created and loaded from database
                if (result.HasDoubt && IsDBLoaded)
                {
                    VerifyDoubts(result);
                }

                // validate the xacl rules, append the errors to the validating result
                _metaData.XaclPolicy.Validate(_metaData, new WindowClientUserManager(), result);

                if (result.HasError || result.HasWarning)
                {
                    ShowValidateErrorDialog(result);

                    return;
                }

                // validate the taxonomies, if there are errors, stop saving to database
                DataViewValidateResult validateResult = null;
                foreach (TaxonomyModel taxonomy in _metaData.Taxonomies)
                {
                    validateResult = taxonomy.Validate();
                    if (validateResult.HasError)
                    {
                        break;
                    }
                }

                if (validateResult != null && validateResult.HasError)
                {
                    ShowTaxonomyValidateErrorDialog(validateResult);

                    return;
                }

                // validate the data views, if there are errors, stop saving to database
                validateResult = null;
                foreach (DataViewModel dataView in _metaData.DataViews)
                {
                    validateResult = dataView.ValidateDataView();
                    if (validateResult.HasError)
                    {
                        break;
                    }
                }

                if (validateResult != null && validateResult.HasError)
                {
                    ShowDataViewValidateErrorDialog(validateResult);

                    return;
                }

                // validate the event definitions, append the errors to the validating result
                _metaData.EventManager.Validate(_metaData, new WindowClientUserManager(), result);

                // verify if the result contains some doubts
                if (result.HasDoubt)
                {
                    VerifyDoubts(result);
                }

                if (result.HasError)
                {
                    if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                        "Error Dialog", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        ShowValidateErrorDialog(result);
                    }

                    return;
                }

                // validate the subscriber definitions, append the errors to the validating result
                _metaData.SubscriberManager.Validate(_metaData, new WindowClientUserManager(), result);

                // verify if the result contains some doubts
                if (result.HasDoubt)
                {
                    VerifyDoubts(result);
                }

                if (result.HasError)
                {
                    if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                        "Error Dialog", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        ShowValidateErrorDialog(result);
                    }

                    return;
                }

                // validate the api definitions, append the errors to the validating result
                _metaData.ApiManager.Validate(_metaData, new WindowClientUserManager(), result);

                // verify if the result contains some doubts
                if (result.HasDoubt)
                {
                    VerifyDoubts(result);
                }

                if (result.HasError)
                {
                    if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidatingError"),
                        "Error Dialog", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        ShowValidateErrorDialog(result);
                    }

                    return;
                }
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }

			// no errors found, the meta model is valid
            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ValidSchema"),
				"Message Dialog", MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		/// <summary>
		/// Verify the doubts contained in the validating result. If they are
		/// errors, change them into errors/warnings.
		/// </summary>
		/// <param name="result"></param>
		internal void VerifyDoubts(ValidateResult result)
		{
			foreach (ValidateResultEntry doubt in result.Doubts)
			{
                if (doubt.MetaDataElement is SimpleAttributeElement)
                {
                    SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)doubt.MetaDataElement;

                    if (simpleAttribute.IsAutoIncrement && simpleAttribute.HasCustomValueGenerator)
                    {
                        // verify if the auto value generator spec is correct or not
                        bool status = true;
                        try
                        {
                            ((DesignStudio)this.MdiParent).MetaDataService.IsValueGeneratorExist(simpleAttribute.AutoValueGenerator);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        if (!status)
                        {
                            result.AddError(doubt);
                        }
                    }
                }

                if (doubt.MetaDataElement is ClassElement)
                {
                    ClassElement classElement = (ClassElement)doubt.MetaDataElement;

                    if (doubt.EventDef != null && doubt.EventDef.OperationType == OperationType.Timer)
                    {
                        // verify if the timer condition is correct or not
                        string msg = ((DesignStudio)this.MdiParent).MetaDataService.ValidateXQueryCondition(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), classElement.Name, doubt.EventDef.TimerCondition);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            doubt.Message = string.Format(doubt.Message, msg); // add server side error
                            result.AddError(doubt);
                        }
                    }

                    if (doubt.Subscriber != null && !string.IsNullOrEmpty(doubt.Subscriber.ExternalHanlder))
                    {
                        // verify if external handler exists or its defintion correct
                        bool status = true;
                        try {
                           ((DesignStudio)this.MdiParent).MetaDataService.IsValidCustomFunctionDefinition(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), doubt.Subscriber.ExternalHanlder);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        if (!status)
                        {
                            result.AddError(doubt);
                        }
                    }
                }
			}
		}

		/// <summary>
		/// Verify if the owner class of the schemaModelElement is empty class
		/// or not.
		/// </summary>
		/// <param name="schemaModelElement">The schemaModel element</param>
		/// <returns>true if it is empty, false otherwise.</returns>
		internal bool IsOwnerClassEmpty(IMetaDataElement schemaModelElement)
		{
			bool status = true;

			if (schemaModelElement is AttributeElementBase)
			{
				ClassElement ownerClass = ((AttributeElementBase) schemaModelElement).OwnerClass;
				status = IsClassEmpty(ownerClass);
			}

			return status;
		}

		/// <summary>
		/// Gets the information indicating if the class is an empty class
		/// (no instances).
		/// </summary>
		/// <param name="classElement">The class element</param>
		/// <returns>true if the class is empty, false if it has instances</returns>
		internal bool IsClassEmpty(ClassElement classElement)
		{
			bool status = true;

			// if the class does not have a TableName, then it is just created and
			// has not been exported to the database, consider it is empty
			if (classElement.TableName != null && classElement.TableName.Length > 0)
			{
				try
				{
					CMDataServiceStub dataService = new CMDataServiceStub();

					// invoke the web service synchronously to get instance count
					int count = dataService.GetInstanceCount(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
						classElement.Name);

					if (count > 0)
					{
						status = false; // not empty
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
		/// Validate if the change to a property of a SchemaModelElment is allowed
		/// </summary>
		/// <param name="element">The SchemaModelElement instance in question.</param>
		/// <param name="propertyName">The property name.</param>
		/// <param name="newValue">The new value</param>
		/// <param name="oldValue">The old value before change</param>
		/// <returns>A localized error message</returns>
		internal string ValidateValueChange(SchemaModelElement element,
			string propertyName, object newValue, object oldValue)
		{
			string msg = null;
			SchemaModelValidateVisitor validator = new SchemaModelValidateVisitor(_metaData.SchemaModel);
			bool newBool, oldBool;
            ClassElement refClass;
            AttributeElementBase refAttribute;

			if (element is SimpleAttributeElement)
			{
				SimpleAttributeElement simpleAttribute = (SimpleAttributeElement) element;
				switch (propertyName)
				{
					case "IsRequired":
						newBool = (bool) newValue;
                        if (newBool)
                        {
                            if (simpleAttribute.IsHistoryEdit)
                            {
                                // the change is invalid, change it back to old value
                                simpleAttribute.IsRequired = (bool)oldValue;

                                msg = validator.GetMessage("Simple.RequiredAttributeInvalidForHistoryEdit");
                            }
                        }

						break;

					case "IsUnique":
						newBool = (bool) newValue;
						if (newBool &&
                            simpleAttribute.ColumnName != null &&
							simpleAttribute.OwnerClass.TableName != null &&
							!IsOwnerClassEmpty(element))
						{
							// the change is invalid, change it back to old value
							simpleAttribute.IsUnique = (bool) oldValue;

							msg = validator.GetMessage("Simple.IllegalUniqueAttribute");
						}

						break;

					case "IsAutoIncrement":
						if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null)
						{
							// the change is invalid, change it back to old value
							simpleAttribute.IsAutoIncrement = (bool) oldValue;

                            // The DataType has been changed to Integer from other data type,
                            // Since we don't know what was the old data type is, we just change
                            // to String type because it is a safe type to change to.
                            //simpleAttribute.ResetDataType();

							msg = validator.GetMessage("Simple.IllegalAutoIncrementAttribute");
						}

						break;

                    case "IsHistoryEdit":
                        if (simpleAttribute.IsIndexed)
                        {
                            simpleAttribute.IsHistoryEdit = false;

                            // The DataType has been changed to Text from other data type,
                            // Since we need to reset the data type
                            simpleAttribute.ResetDataType();

                            msg = validator.GetMessage("Simple.NormalIndexExisted");
                        }
                        else if (simpleAttribute.IsRequired)
                        {
                            // required attribute, such as primary key can not be used as history edit attribute
                            simpleAttribute.IsHistoryEdit = false;

                            // The DataType has been changed to Text from other data type,
                            // Since we need to reset the data type
                            simpleAttribute.ResetDataType();

                            msg = validator.GetMessage("Simple.RequiredAttributeInvalidForHistoryEdit");
                        }
                        else if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null)
                        {
                            oldBool = (bool)oldValue;
                            if (!IsOwnerClassEmpty(element))
                            {
                                // the change is invalid, change it back to old value
                                simpleAttribute.IsHistoryEdit = oldBool;

                                // The DataType has been changed to Text from other data type,
                                // Since we need to reset the data type
                                simpleAttribute.ResetDataType();

                                msg = validator.GetMessage("Simple.InvalidHistoryEditChange");
                            }
                            else if (oldBool)
                            {
                                // unable to change the is history edit back once it has been
                                // saved to database
                                simpleAttribute.IsHistoryEdit = oldBool;

                                msg = validator.GetMessage("Simple.UnableToChangeBackHistoryEdit");
                            }
                        }

                        break;

                    case "IsRichText":
                        if (simpleAttribute.IsIndexed)
                        {
                            simpleAttribute.IsRichText = false;

                            // The DataType has been changed to Text from other data type,
                            // Since we need to reset the data type
                            simpleAttribute.ResetDataType();

                            msg = validator.GetMessage("Simple.NormalIndexExisted");
                        }
                        else if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null)
                        {
                            oldBool = (bool)oldValue;
                            if (!IsOwnerClassEmpty(element))
                            {
                                // the change is invalid, change it back to old value
                                simpleAttribute.IsRichText = oldBool;

                                // The DataType has been changed to Text from other data type,
                                // Since we need to reset the data type
                                simpleAttribute.ResetDataType();

                                msg = validator.GetMessage("Simple.InvalidRichTextChange");
                            }
                            else if (oldBool)
                            {
                                // unable to change the is history edit back once it has been
                                // saved to database
                                simpleAttribute.IsRichText = oldBool;

                                msg = validator.GetMessage("Simple.UnableToChangeBackRichText");
                            }
                        }

                        break;

                    case "Constraint":

                        // changes with pattern and range constraints are allowed
                        ConstraintElementBase oldConstraint = (ConstraintElementBase)oldValue;
                        if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null &&
                            newValue is IEnumConstraint &&
                            !IsOwnerClassEmpty(element))
                        {
                            // the change is invalid, change it back to old value
                            simpleAttribute.Constraint = oldConstraint;

                            msg = validator.GetMessage("Simple.InvalidConstraintChange");
                        }

                        break;

                    case "IsIndexed":

                        // Index is not allowed when the data type is Text
                        if (((bool) newValue) &&
                            simpleAttribute.DataType == DataType.Text)
                        {
                            simpleAttribute.IsIndexed = false;

                            // index is not allowed for Text type
                            msg = validator.GetMessage("Simple.InvalidIndexType");
                        }

                        break;

                    case "DataType":
                        DataType oldType = (DataType)oldValue;
                        DataType newType = (DataType)newValue;

                        if (simpleAttribute.IsPrimaryKey &&
                            simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null)
                        {
                            // do not allow data type changes for the primary key attribute for
                            // a saved schema
                            simpleAttribute.DataType = oldType;

                            msg = validator.GetMessage("Simple.InvalidPKDataTypeChange");
                        }
                        else if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null)
                        {
                            if (oldType == DataType.Text)
                            {
                                // do not allow data type changes when the column has been created with Text type
                                simpleAttribute.DataType = oldType;

                                msg = validator.GetMessage("Simple.UnableToChangeTextBack");
                            }
                            else if (!IsClassEmpty(simpleAttribute.OwnerClass))
                            {
                                // do not allow data type changes when the class already has data instances
                                // exists
                                simpleAttribute.DataType = oldType;

                                msg = validator.GetMessage("Simple.InvalidTypeChange");
                            }
                        }


                        // invalidate the operator since previously selected operator may not be
                        // appropriate for the new data type, change to equals which is universal
                        // to all data type
                        simpleAttribute.Operator = null; // change to equals operator

                        break;

                    case "DefaultValue":

                        string oldDefaultValue = (string)oldValue;
                        string newDefaultValue = (string)newValue;

                        if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null &&
                            !string.IsNullOrEmpty(oldDefaultValue) &&
                            !IsClassEmpty(simpleAttribute.OwnerClass))
                        {
                            // do not allow default value changes when a default value exists and the class already has data instances exists
                            simpleAttribute.DefaultValue = oldDefaultValue;

                            msg = validator.GetMessage("Simple.InvalidDefaultChange");
                        }

                        break;

                    case "MaxLength":

                        int oldMaxLengthValue = (int)oldValue;
                        int newMaxLengthVlaue = (int)newValue;

                        if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null &&
                            newMaxLengthVlaue <= oldMaxLengthValue &&
                            !IsClassEmpty(simpleAttribute.OwnerClass))
                        {
                            // do not allow max length value changes when new max length is leass than the old max length and
                            // the class already has data instances exists
                            simpleAttribute.MaxLength = oldMaxLengthValue;

                            msg = validator.GetMessage("Simple.InvalidMaxLengthChange");
                        }

                        break;

                    case "MinLength":

                        int oldMinLengthValue = (int)oldValue;
                        int newMinLengthValue = (int)newValue;

                        if (simpleAttribute.ColumnName != null &&
                            simpleAttribute.OwnerClass.TableName != null &&
                            newMinLengthValue >= oldMinLengthValue &&
                            !IsClassEmpty(simpleAttribute.OwnerClass))
                        {
                            // do not allow min length value changes when new min length is greater than the old min length and
                            // the class already has data instances exists
                            simpleAttribute.MinLength = oldMinLengthValue;

                            msg = validator.GetMessage("Simple.InvalidMinLengthChange");
                        }

                        break;
				}
			}
            else if (element is RelationshipAttributeElement)
            {
                RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement)element;
                switch (propertyName)
                {
                    case "IsBrowsable":

                        // set the value to the backward relationship
                        relationshipAttribute.BackwardRelationship.IsBrowsable = (bool)newValue;

                        break;

                    case "IsIndexed":

                        if (!relationshipAttribute.IsForeignKeyRequired && relationshipAttribute.IsIndexed)
                        {
                            relationshipAttribute.IsIndexed = false;

                            msg = validator.GetMessage("Relationship.InvalidForIndex");
                        }

                        break;
                }
            }
            else if (element is ArrayAttributeElement)
            {
                ArrayAttributeElement arrayAttribute = (ArrayAttributeElement)element;
                switch (propertyName)
                {
                    case "IsRequired":
                        /* allow to change IsRequired when a class has instances
                        newBool = (bool)newValue;
                        if (newBool &&
                            arrayAttribute.ColumnName != null &&
                            arrayAttribute.OwnerClass.TableName != null &&
                            !IsOwnerClassEmpty(element))
                        {
                            // the change is invalid, change it back to old value
                            arrayAttribute.IsRequired = (bool)oldValue;

                            msg = validator.GetMessage("Simple.IllegalRequiredAttribute");
                        }
                         */

                        break;
                }
            }
            else if (element is ClassElement)
            {
                ClassElement classElement = (ClassElement)element;
                switch (propertyName)
                {
                    case "PrimaryKeys":
                        SchemaModelElementCollection oldPrimaryKeys = (SchemaModelElementCollection)oldValue;
                        if (classElement.TableName != null)
                        {
                            // the class has been created in the database, check the constraints
                            // with regard to the primary keys
                            if (!IsClassEmpty(classElement))
                            {
                                // the change is invalid, change it back to old value
                                classElement.PrimaryKeys = oldPrimaryKeys;

                                msg = validator.GetMessage("Class.IllegalPrimaryKey");
                            }
                            else
                            {
                                // clear the IsUnique setting of the attribute that becomes
                                // part of primary key(s) since the existing unique constraint
                                // will be deleted as part of meta data update
                                foreach (SimpleAttributeElement pk in classElement.PrimaryKeys)
                                {
                                    pk.IsUnique = false;
                                }
                            }
                        }

                        break;
                    case "UniqueKeys":
                        SchemaModelElementCollection uniqueKeys = (SchemaModelElementCollection)oldValue;
                        if (classElement.TableName != null)
                        {
                            // Disallow change of unique keys if the class is not empty
                            /*
                            if (!IsClassEmpty(classElement))
                            {
                                // the change is invalid, change it back to old value
                                classElement.UniqueKeys = uniqueKeys;

                                msg = validator.GetMessage("Class.IllegalUniqueKey");
                            }
                             */
                        }

                        break;
                }
            }
            else if (element is ConstraintElementBase)
            {
                ConstraintElementBase constraint = (ConstraintElementBase)element;
                RangeElement rangeConstraint;

                switch (propertyName)
                {
                    case "DataType":
                        DataType oldType = (DataType)oldValue;
                        DataType newType = (DataType)newValue;

                        if (newType == DataType.Text)
                        {
                            // Text type is invalid for constraint
                            constraint.DataType = oldType;

                            msg = validator.GetMessage("Constraint.InvalidDataType");
                        }
                        else if (newType != DataType.String &&
                            IsConstraintUsedByNonEmptyClasses(constraint, out refClass, out refAttribute))
                        {
                            // do not allow data type changes from any types to a non-string
                            // type when the constraint is referenced by any attributes whose class
                            // has already has data instances exists
                            constraint.DataType = oldType;

                            msg = string.Format(validator.GetMessage("Constraint.InvalidTypeChange"),
                                refClass.Caption, refAttribute.Caption);
                        }

                        break;

                    case "IsMultipleSelection":

                        oldBool = (bool)oldValue;
                        newBool = (bool)newValue;

                        EnumElement enumConstraint = (EnumElement)constraint;

                        if (IsConstraintUsedByNonEmptyClasses(enumConstraint, out refClass, out refAttribute))
                        {
                            // do not allow change of selection mode when the constraint is
                            // referenced by any attributes whose class
                            // has already has data instances exists
                            enumConstraint.IsMultipleSelection = oldBool;
                            if (newBool)
                            {
                                enumConstraint.DataType = DataType.String;
                            }

                            msg = string.Format(validator.GetMessage("Constraint.InvalidSelectionModeChange"), refClass.Caption, refAttribute.Caption);
                        }

                        break;

                    case "MinValue":

                        rangeConstraint = (RangeElement)constraint;

                        if (IsConstraintUsedByNonEmptyClasses(rangeConstraint, out refClass, out refAttribute))
                        {
                            // give a warning that the constraint is
                            // referenced by any attributes whose class
                            // has already has data instances exists

                            msg = string.Format(validator.GetMessage("Constraint.InvalidMinMaxValueChange"), refClass.Caption, refAttribute.Caption);
                        }

                        break;
                    case "MaxValue":

                        rangeConstraint = (RangeElement)constraint;

                        if (IsConstraintUsedByNonEmptyClasses(rangeConstraint, out refClass, out refAttribute))
                        {
                            // give a warning that the constraint is
                            // referenced by any attributes whose class
                            // has already has data instances exists

                            msg = string.Format(validator.GetMessage("Constraint.InvalidMinMaxValueChange"), refClass.Caption, refAttribute.Caption);
                        }

                        break;
                }
            }

			return msg;
		}

        /// <summary>
        /// Validate if the change to a property of a IDataViewElement is allowed
        /// </summary>
        /// <param name="element">The IDataViewElement instance in question.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="newValue">The new value</param>
        /// <param name="oldValue">The old value before change</param>
        /// <returns>A localized error message</returns>
        private string ValidateValueChange(IDataViewElement element,
            string propertyName, object newValue, object oldValue)
        {
            string msg = null;

            if (element is ITaxonomy)
            {
                ITaxonomy node = (ITaxonomy)element;
                switch (propertyName)
                {
                    case "ClassName":
                        // clear the auto classifying definition that has been associated with
                        // the node
                        if (node.AutoClassifyDef.HasDefinition)
                        {
                            if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ClearAutoClassify"),
                                "Confirm Dialog", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.No)
                            {
                                if (oldValue != null)
                                {
                                    node.ClassName = (string)oldValue;
                                }
                                else
                                {
                                    node.ClassName = null;
                                }
                            }
                            else
                            {
                                // clear the definitions
                                node.AutoClassifyDef.Clear();
                            }
                        }

                        break;
                    case "DataViewName":
                        // clear the auto classifying definition that has been associated with
                        // the node
                        if (node.AutoClassifyDef.HasDefinition)
                        {
                            if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ClearAutoClassify"),
                                "Confirm Dialog", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.No)
                            {
                                if (oldValue != null)
                                {
                                    node.DataViewName = (string)oldValue;
                                }
                                else
                                {
                                    node.DataViewName = null;
                                }
                            }
                            else
                            {
                                // clear the definitions
                                node.AutoClassifyDef.Clear();
                            }
                        }

                        break;
                }
            }

            return msg;
        }

		/// <summary>
		/// Add a child tree node at the end of children list
		/// </summary>
		/// <param name="parent">The parent tree node.</param>
		/// <param name="child">The child tree node.</param>
		/// <param name="listViewItem">The list view item</param>
		internal void AddChildNode(MetaDataTreeNode parent,
			MetaDataTreeNode child,
			ListViewItem listViewItem)
		{
            if (child.MetaDataElement != null)
            {
                child.MetaDataElement.Position = parent.Nodes.Count;
            }
			parent.Nodes.Add(child);

			this.listView1.Items.Add(listViewItem);
		}

        /// <summary>
        /// Gets the information indicating whether two primary key collections are the same
        /// </summary>
        /// <param name="newPrimaryKeys"></param>
        /// <param name="oldPrimaryKeys"></param>
        /// <returns>true if they are the same, false, otherwise.</returns>
        private bool IsSamePK(SchemaModelElementCollection newPrimaryKeys,
            SchemaModelElementCollection oldPrimaryKeys)
        {
            bool status = false;

            if (newPrimaryKeys.Count == oldPrimaryKeys.Count)
            {
                bool found = false;
                foreach (SchemaModelElement newPk in newPrimaryKeys)
                {
                    found = false;
                    foreach (SchemaModelElement oldPk in oldPrimaryKeys)
                    {
                        if (newPk.Name == oldPk.Name)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        break;
                    }
                }

                if (found)
                {
                    // two collection contains the same set of primary keys
                    status = true;
                }
            }

            return status;
        }

		/// <summary>
		/// Display the validating errors
		/// </summary>
		/// <param name="result">The result containing validating errors</param>
		private void ShowValidateErrorDialog(ValidateResult result)
		{
			string summary = result.Errors.Count + " Errors; " + result.Warnings.Count + " Warnings;";

			ValidateErrorDialog dialog = new ValidateErrorDialog();
			dialog.Entries = result.AllEntries;
			dialog.Accept += new EventHandler(validateErrorDialog_Accept);
			dialog.Show();
		}

		private void ShowTaxonomyValidateErrorDialog(DataViewValidateResult validateResult)
		{
			DataValidateErrorDialog dialog = new DataValidateErrorDialog();
			dialog.Entries = validateResult.Errors;
			dialog.ShowDialog();
		}

		private void ShowDataViewValidateErrorDialog(DataViewValidateResult validateResult)
		{
			DataValidateErrorDialog dialog = new DataValidateErrorDialog();
			dialog.Entries = validateResult.Errors;
			dialog.ShowDialog();
		}

		/// <summary>
		/// Show the data validate rules associated with the a class element,
		/// otherwise, hide the tab for data validate rules
		/// </summary>
		/// <param name="metaDataElement">The mete data element</param>
		private void ShowValidateRules(IMetaDataElement metaDataElement)
		{
			if (metaDataElement != null && metaDataElement is ClassElement)
			{
				this.validateRuleListView.SuspendLayout();
				this.validateRuleListView.Items.Clear();

				ValidateRuleListViewItem listViewItem;
				RuleCollection rules = _metaData.RuleManager.GetPrioritizedRules((ClassElement) metaDataElement).Rules;
                ClassElement ownerClass;
                string ownerClassCaption;
                foreach (RuleDef rule in rules)
				{
                    ownerClass = _metaData.SchemaModel.FindClass(rule.ClassName);
                    if (ownerClass != null)
                    {
                        ownerClassCaption = ownerClass.Caption;
                    }
                    else
                    {
                        ownerClassCaption = "Unknown";
                    }
                    listViewItem = new ValidateRuleListViewItem((ClassElement)metaDataElement, rule, ownerClassCaption);
					this.validateRuleListView.Items.Add(listViewItem);
				}

				this.validateRuleListView.ResumeLayout();

				this.addValidateRuleBtn.Enabled = true;
			}
			else
			{
				this.validateRuleListView.Items.Clear();

				// rules are only associated with class
				this.addValidateRuleBtn.Enabled = false;
			}

			this.modifyValidateRuleBtn.Enabled = false;
			this.removeValidateRuleBtn.Enabled = false;
		}

		/// <summary>
		/// Show the xacl rules associated with the meta data element
		/// </summary>
		/// <param name="metaDataElement">The mete data element</param>
		private void ShowXaclRules(IMetaDataElement metaDataElement)
		{
			IXaclObject xaclItem = metaDataElement as IXaclObject;

			// TODO, shift focus so that controls with databinding can generate an event
			// to write data back to its source. Figure out a better way to do this.
			//this.permissionGroupBox.Focus();

			this.ruleListView.SuspendLayout();
			this.ruleListView.Items.Clear();

			InitializeXaclRuleControls();

			if (xaclItem != null)
			{
				XaclObject obj = new XaclObject(xaclItem.ToXPath());

				XaclRuleListViewItem listViewItem;
				XaclRuleCollection rules = _metaData.XaclPolicy.GetRules(obj);
				foreach (XaclRule rule in rules)
				{
					listViewItem = new XaclRuleListViewItem(obj, rule);
					if (this._metaData.XaclPolicy.IsLocalRuleExist(obj, rule))
					{
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
					}
					else
					{
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRule"));
					}
					this.ruleListView.Items.Add(listViewItem);
				}

				this.addRuleButton.Enabled = true;
			}

			this.ruleListView.ResumeLayout();
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
				if (!_metaData.XaclPolicy.IsRuleExist(obj, rule))
				{
					_metaData.XaclPolicy.AddRule(obj, rule);
					XaclRuleListViewItem listViewItem = new XaclRuleListViewItem(obj, rule);
                    listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
					listViewItem.Selected = true;
					this.ruleListView.Items.Add(listViewItem);
				}
				else if (!_metaData.XaclPolicy.IsLocalRuleExist(obj, rule))
				{
					// there is a propagated rule with the same subject exist,
					// ask user if he/she wants to override it
                    if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRuleExists"),
						"Confirm Dialog", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
						== DialogResult.Yes)
					{
						// remove the propagated rule from display in the list view
						XaclRule propagatedRule = _metaData.XaclPolicy.GetPropagatedRule(obj, rule);
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
						_metaData.XaclPolicy.AddRule(obj, rule);
						XaclRuleListViewItem listViewItem = new XaclRuleListViewItem(obj, rule);
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
						listViewItem.Selected = true;
						this.ruleListView.Items.Add(listViewItem);
					}
					else
					{
						// add as a local rule
						_metaData.XaclPolicy.AddRule(obj, rule);
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
			_metaData.XaclPolicy.RemoveRule(listViewItem.Object, listViewItem.Rule);

			// refresh the rules in case there is a propagated rule
			this.ShowXaclRules((IMetaDataElement) this.propertyGrid1.SelectedObject);
		}

		/// <summary>
		/// Display and bind the detail of a rule to UI controls
		/// </summary>
		/// <param name="rule">The rule</param>
		private void ShowXaclRuleDetail(XaclRule rule)
		{
			XaclActionCollection actions = rule.Actions;
			string href = ((IXaclObject) this.propertyGrid1.SelectedObject).ToXPath();
			bool isLocalRule;
			if (rule.ObjectHref != href)
			{
				isLocalRule = false;
				this.removeRuleButton.Enabled = false;
			}
			else
			{
				isLocalRule = true;
				this.removeRuleButton.Enabled = true;
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
					case XaclActionType.Write:
						this.grantWriteRadioButton.DataBindings.Clear();
						this.grantWriteRadioButton.Enabled = isLocalRule;
						this.grantWriteRadioButton.DataBindings.Add("Checked", rule, "IsWriteGranted");
						this.denyWriteRadioButton.DataBindings.Clear();
						this.denyWriteRadioButton.Enabled = isLocalRule;
						this.denyWriteRadioButton.DataBindings.Add("Checked", rule, "IsWriteDenied");
						break;
					case XaclActionType.Create:
						this.grantCreateRadioButton.DataBindings.Clear();
						this.grantCreateRadioButton.Enabled = isLocalRule;
						this.grantCreateRadioButton.DataBindings.Add("Checked", rule, "IsCreateGranted");
						this.denyCreateRadioButton.DataBindings.Clear();
						this.denyCreateRadioButton.Enabled = isLocalRule;
						this.denyCreateRadioButton.DataBindings.Add("Checked", rule, "IsCreateDenied");
						break;
					case XaclActionType.Delete:
						this.grantDeleteRadioButton.DataBindings.Clear();
						this.grantDeleteRadioButton.Enabled = isLocalRule;
						this.grantDeleteRadioButton.DataBindings.Add("Checked", rule, "IsDeleteGranted");
						this.denyDeleteRadioButton.DataBindings.Clear();
						this.denyDeleteRadioButton.Enabled = isLocalRule;
						this.denyDeleteRadioButton.DataBindings.Add("Checked", rule, "IsDeleteDenied");
						break;
					case XaclActionType.Upload:
						this.grantUploadRadioButton.DataBindings.Clear();
						this.grantUploadRadioButton.Enabled = isLocalRule;
						this.grantUploadRadioButton.DataBindings.Add("Checked", rule, "IsUploadGranted");
						this.denyUploadRadioButton.DataBindings.Clear();
						this.denyUploadRadioButton.Enabled = isLocalRule;
						this.denyUploadRadioButton.DataBindings.Add("Checked", rule, "IsUploadDenied");
						break;
					case XaclActionType.Download:
						this.grantDownloadRadioButton.DataBindings.Clear();
						this.grantDownloadRadioButton.Enabled = isLocalRule;
						this.grantDownloadRadioButton.DataBindings.Add("Checked", rule, "IsDownloadGranted");
						this.denyDownloadRadioButton.DataBindings.Clear();
						this.denyDownloadRadioButton.Enabled = isLocalRule;
						this.denyDownloadRadioButton.DataBindings.Add("Checked", rule, "IsDownloadDenied");
						break;
				}
			}

			this.conditionTextBox.DataBindings.Clear();
			this.conditionTextBox.Enabled = isLocalRule;
			this.conditionTextBox.DataBindings.Add("Text", rule.Condition, "Condition");

            this.allowPropagateCheckBox.Enabled = isLocalRule;
			this.allowPropagateCheckBox.DataBindings.Clear();
			this.allowPropagateCheckBox.DataBindings.Add("Checked", rule, "AllowPropagation");
		}

		/// <summary>
		/// Create default rules at meta data model level, which will be inherited
		/// by sub-models
		/// </summary>
		/// <param name="metaData">The meta data</param>
		private void SetDefaultPermissions(MetaDataModel metaData)
		{
			// create a rule to allow EveryOne to have full access to meta data model
			XaclObject obj = new XaclObject(metaData.ToXPath());
			XaclSubject subject = new XaclSubject();
			subject.AddRole(XaclSubject.EveryOne);
			XaclRule rule = new XaclRule(subject);
			metaData.XaclPolicy.AddRule(obj, rule);

			// create a rule to allow Anonymous to have read permission to meta data model
            /*
			obj = new XaclObject(metaData.ToXPath());
			subject = new XaclSubject();
			subject.Uid = XaclSubject.AnonymousUser;
			rule = new XaclRule(subject);
			rule.SetPermission(XaclActionType.Write, XaclPermissionType.Deny);
			rule.SetPermission(XaclActionType.Create, XaclPermissionType.Deny);
			rule.SetPermission(XaclActionType.Delete, XaclPermissionType.Deny);
            rule.SetPermission(XaclActionType.Upload, XaclPermissionType.Deny);
			metaData.XaclPolicy.AddRule(obj, rule);
            */
		}

		/// <summary>
		/// Initialize the states of xacl rule related controls
		/// </summary>
		private void InitializeXaclRuleControls()
		{
			this.addRuleButton.Enabled = false;
			this.removeRuleButton.Enabled = false;
			this.grantReadRadioButton.DataBindings.Clear();
			this.grantReadRadioButton.Checked = false;
			this.grantReadRadioButton.Enabled = false;
			this.denyReadRadioButton.DataBindings.Clear();
			this.denyReadRadioButton.Checked = false;
			this.denyReadRadioButton.Enabled = false;
			this.grantWriteRadioButton.DataBindings.Clear();
			this.grantWriteRadioButton.Checked = false;
			this.grantWriteRadioButton.Enabled = false;
			this.denyWriteRadioButton.DataBindings.Clear();
			this.denyWriteRadioButton.Checked = false;
			this.denyWriteRadioButton.Enabled = false;
			this.grantCreateRadioButton.DataBindings.Clear();
			this.grantCreateRadioButton.Checked = false;
			this.grantCreateRadioButton.Enabled = false;
			this.denyCreateRadioButton.DataBindings.Clear();
			this.denyCreateRadioButton.Checked = false;
			this.denyCreateRadioButton.Enabled = false;
			this.grantDeleteRadioButton.DataBindings.Clear();
			this.grantDeleteRadioButton.Checked = false;
			this.grantDeleteRadioButton.Enabled = false;
			this.denyDeleteRadioButton.DataBindings.Clear();
			this.denyDeleteRadioButton.Checked = false;
			this.denyDeleteRadioButton.Enabled = false;
			this.grantUploadRadioButton.DataBindings.Clear();
			this.grantUploadRadioButton.Checked = false;
			this.grantUploadRadioButton.Enabled = false;
			this.denyUploadRadioButton.DataBindings.Clear();
			this.denyUploadRadioButton.Checked = false;
			this.denyUploadRadioButton.Enabled = false;
			this.grantDownloadRadioButton.DataBindings.Clear();
			this.grantDownloadRadioButton.Checked = false;
			this.grantDownloadRadioButton.Enabled = false;
			this.denyDownloadRadioButton.DataBindings.Clear();
			this.denyDownloadRadioButton.Checked = false;
			this.denyDownloadRadioButton.Enabled = false;
			this.conditionTextBox.DataBindings.Clear();
			this.conditionTextBox.Text = "";
			this.conditionTextBox.Enabled = false;
			this.allowPropagateCheckBox.DataBindings.Clear();
			this.allowPropagateCheckBox.Checked = false;
			this.allowPropagateCheckBox.Enabled = false;
		}

		/// <summary>
		/// Add a data validate rule to the currently selected class
		/// </summary>
        /// <param name="ruleDef">The rule to be added</param>
		private void AddValidateRule(RuleDef ruleDef)
		{
			ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

			if (classElement != null)
			{
				_metaData.RuleManager.AddRule(classElement, ruleDef);
                ValidateRuleListViewItem listViewItem = new ValidateRuleListViewItem(classElement, ruleDef, classElement.Caption);
				listViewItem.Selected = true;
				this.validateRuleListView.Items.Add(listViewItem);
			}
		}

		/// <summary>
		/// Modify a data validate rule
		/// </summary>
		/// <param name="listViewItem">The list view item for the rule</param>
		private void ModifyValidateRule(ValidateRuleListViewItem listViewItem)
		{
			DefineValidatingRuleDialog dialog = new DefineValidatingRuleDialog();
			dialog.RuleDef = listViewItem.Rule.Clone();

			if (dialog.ShowDialog() == DialogResult.OK)
			{
                // replace the rule in the rule set
                RuleCollection rules = _metaData.RuleManager.GetLocalRules(listViewItem.Rule.ClassName);
                int pos = rules.Remove(listViewItem.Rule);
                if (pos >= 0)
                {
                    rules.Insert(pos, dialog.RuleDef);
                }
                else
                {
                    rules.Add(dialog.RuleDef);
                }
                listViewItem.Rule = dialog.RuleDef;
				listViewItem.Text = dialog.RuleDef.ToString();
                listViewItem.SubItems[1].Text = dialog.RuleDef.Priority.ToString();
			}
		}

		/// <summary>
		/// Remove a RuleDef from currently displayed validating rules
		/// </summary>
		/// <param name="listViewItem">The list view item for the rule</param>
		private void RemoveValidateRule(ValidateRuleListViewItem listViewItem)
		{
			_metaData.RuleManager.RemoveRule(listViewItem.ClassElement, listViewItem.Rule);

			// refresh the rules in display
			this.ShowValidateRules((IMetaDataElement) this.propertyGrid1.SelectedObject);
		}

        /// <summary>
        /// Show the events associated with a class element
        /// </summary>
        /// <param name="metaDataElement">The mete data element</param>
        private void ShowEvents(IMetaDataElement metaDataElement)
        {
            ClassElement clsElement = metaDataElement as ClassElement;

            this.eventListView.SuspendLayout();
            this.eventListView.Items.Clear();
            this.eventPropertyGrid.SelectedObject = null;

            this.deleteEventButton.Enabled = false;

            if (clsElement != null)
            {
                EventListViewItem listViewItem;
                EventCollection events = _metaData.EventManager.GetClassEvents(clsElement);
                int index = 0;
                bool isLocalEvent;
                foreach (EventDef eventDef in events)
                {
                    listViewItem = new EventListViewItem(clsElement, eventDef);
                    eventDef.MetaData = this._metaData;
                    isLocalEvent = this._metaData.EventManager.IsLocalEventExist(clsElement, eventDef);
                    if (isLocalEvent)
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                    }
                    else
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRule"));
                    }

                    // select the first node by default
                    if (index == 0)
                    {
                        listViewItem.Selected = true;
                        if (isLocalEvent)
                        {
                            this.deleteEventButton.Enabled = true;
                        }
                    }
                    index++;
                    this.eventListView.Items.Add(listViewItem);
                }

                this.addEventButton.Enabled = true;
            }
            else
            {
                this.addEventButton.Enabled = false;
            }

            this.eventListView.ResumeLayout();
        }

        /// <summary>
        /// Add an event to the selected class
        /// </summary>
        private void AddClassEvent()
        {
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

			if (classElement != null)
			{
				EventDef eventDef = new EventDef();
                eventDef.ClassName = classElement.Name;
                eventDef.MetaData = this._metaData;
                // make sure the event name is unique
                int index = 2;
                while (_metaData.EventManager.IsEventExist(classElement, eventDef))
				{
                    eventDef.Name = "Event" + index;
                    index++;
                }

                _metaData.EventManager.AddEvent(classElement, eventDef);
                EventListViewItem listViewItem = new EventListViewItem(classElement, eventDef);
                listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
				listViewItem.Selected = true;
				this.eventListView.Items.Add(listViewItem);
			}
        }

        /// <summary>
        /// Remove an event from currently displayed class events
        /// </summary>
        /// <param name="listViewItem">The list view item for the event</param>
        private void RemoveClassEvent(EventListViewItem listViewItem)
        {
            _metaData.EventManager.RemoveEvent(listViewItem.ClassElement, listViewItem.Event);

            // refresh the event in display
            this.ShowEvents(listViewItem.ClassElement);
        }

        /// <summary>
        /// Show the event properties
        /// </summary>
        /// <param name="eventDef">The EventDef object</param>
        private void ShowEventDetail(EventDef eventDef)
        {
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

            if (classElement != null)
            {
                if (classElement.Name != eventDef.ClassName)
                {
                    // is an inherited event, no remove and no editing
                    this.deleteEventButton.Enabled = false;
                    this.eventPropertyGrid.SelectedObject = eventDef;
                    this.eventPropertyGrid.Enabled = false;
                }
                else
                {
                    // it is a local event;
                    this.deleteEventButton.Enabled = true;
                    this.eventPropertyGrid.SelectedObject = eventDef;
                    this.eventPropertyGrid.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Show the logging rules associated with the meta data element
        /// </summary>
        /// <param name="metaDataElement">The mete data element</param>
        private void ShowLoggingRules(IMetaDataElement metaDataElement)
        {
            IXaclObject loggingItem = metaDataElement as IXaclObject;

            // TODO, shift focus so that controls with databinding can generate an event
            // to write data back to its source. Figure out a better way to do this.
            //this.logStatusGroupBox.Focus();

            this.loggingRuleListView.SuspendLayout();
            this.loggingRuleListView.Items.Clear();

            InitializeLoggingRuleControls();

            if (loggingItem != null &&
                !(loggingItem is DataViewModel) &&
                !(loggingItem is TaxonNode) &&
                !(loggingItem is TaxonomyModel) &&
                !(loggingItem is AttributeElementBase))
            {
                LoggingObject obj = new LoggingObject(loggingItem.ToXPath());

                LoggingRuleListViewItem listViewItem;
                LoggingRuleCollection rules = _metaData.LoggingPolicy.GetRules(obj);
                foreach (LoggingRule rule in rules)
                {
                    listViewItem = new LoggingRuleListViewItem(obj, rule);
                    if (this._metaData.LoggingPolicy.IsLocalRuleExist(obj, rule))
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                    }
                    else
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRule"));
                    }
                    this.loggingRuleListView.Items.Add(listViewItem);
                }

                this.addLoggingRuleButton.Enabled = true;
            }

            this.loggingRuleListView.ResumeLayout();
        }

        private void AddLoggingRule(string user, string[] roles)
        {
            IXaclObject loggingItem = this.propertyGrid1.SelectedObject as IXaclObject;

            if (loggingItem != null)
            {
                LoggingObject obj = new LoggingObject(loggingItem.ToXPath());

                LoggingSubject subject = new LoggingSubject();
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

                LoggingRule rule = new LoggingRule(subject, obj.Href);
                if (!_metaData.LoggingPolicy.IsRuleExist(obj, rule))
                {
                    _metaData.LoggingPolicy.AddRule(obj, rule);
                    LoggingRuleListViewItem listViewItem = new LoggingRuleListViewItem(obj, rule);
                    listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                    listViewItem.Selected = true;
                    this.loggingRuleListView.Items.Add(listViewItem);
                }
                else if (!_metaData.LoggingPolicy.IsLocalRuleExist(obj, rule))
                {
                    // there is a propagated rule with the same subject exist,
                    // ask user if he/she wants to override it
                    if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRuleExists"),
                        "Confirm Dialog", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        // remove the propagated rule from display in the list view
                        LoggingRule propagatedRule = _metaData.LoggingPolicy.GetPropagatedRule(obj, rule);
                        foreach (LoggingRuleListViewItem item in this.loggingRuleListView.Items)
                        {
                            if (item.Rule == propagatedRule)
                            {
                                this.loggingRuleListView.Items.Remove(item);
                                break;
                            }
                        }

                        // add the overrided rule
                        rule.IsOverrided = true;
                        _metaData.LoggingPolicy.AddRule(obj, rule);
                        LoggingRuleListViewItem listViewItem = new LoggingRuleListViewItem(obj, rule);
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                        listViewItem.Selected = true;
                        this.loggingRuleListView.Items.Add(listViewItem);
                    }
                    else
                    {
                        // add as a local rule
                        _metaData.LoggingPolicy.AddRule(obj, rule);
                        LoggingRuleListViewItem listViewItem = new LoggingRuleListViewItem(obj, rule);
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                        listViewItem.Selected = true;
                        this.loggingRuleListView.Items.Add(listViewItem);
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
        /// Remove a LoggingRule from currently displayed rules
        /// </summary>
        /// <param name="listViewItem">The list view item for the rule</param>
        private void RemoveLoggingRule(LoggingRuleListViewItem listViewItem)
        {
            _metaData.LoggingPolicy.RemoveRule(listViewItem.Object, listViewItem.Rule);

            // refresh the rules in case there is a propagated rule
            this.ShowLoggingRules((IMetaDataElement)this.propertyGrid1.SelectedObject);
        }

        /// <summary>
        /// Display and bind the detail of a rule to UI controls
        /// </summary>
        /// <param name="rule">The rule</param>
        private void ShowLoggingRuleDetail(LoggingRule rule)
        {
            LoggingActionCollection actions = rule.Actions;
            string href = ((IXaclObject)this.propertyGrid1.SelectedObject).ToXPath();
            bool isLocalRule;
            if (rule.ObjectHref != href)
            {
                isLocalRule = false;
                this.removeLoggingRuleButton.Enabled = false;
            }
            else
            {
                isLocalRule = true;
                this.removeLoggingRuleButton.Enabled = true;
            }

            foreach (LoggingAction action in actions)
            {
                switch (action.ActionType)
                {
                    case LoggingActionType.Read:
                        this.readLogOnRadioButton.DataBindings.Clear();
                        this.readLogOnRadioButton.Enabled = isLocalRule;
                        this.readLogOnRadioButton.DataBindings.Add("Checked", rule, "IsReadLogOn");
                        this.readLogOffRadioButton.DataBindings.Clear();
                        this.readLogOffRadioButton.Enabled = isLocalRule;
                        this.readLogOffRadioButton.DataBindings.Add("Checked", rule, "IsReadLogOff");
                        break;
                    case LoggingActionType.Write:
                        this.modifyLogOnRadioButton.DataBindings.Clear();
                        this.modifyLogOnRadioButton.Enabled = isLocalRule;
                        this.modifyLogOnRadioButton.DataBindings.Add("Checked", rule, "IsWriteLogOn");
                        this.modifyLogOffRadioButton.DataBindings.Clear();
                        this.modifyLogOffRadioButton.Enabled = isLocalRule;
                        this.modifyLogOffRadioButton.DataBindings.Add("Checked", rule, "IsWriteLogOff");
                        break;
                    case LoggingActionType.Create:
                        this.createLogOnRadioButton.DataBindings.Clear();
                        this.createLogOnRadioButton.Enabled = isLocalRule;
                        this.createLogOnRadioButton.DataBindings.Add("Checked", rule, "IsCreateLogOn");
                        this.createLogOffRadioButton.DataBindings.Clear();
                        this.createLogOffRadioButton.Enabled = isLocalRule;
                        this.createLogOffRadioButton.DataBindings.Add("Checked", rule, "IsCreateLogOff");
                        break;
                    case LoggingActionType.Delete:
                        this.deleteLogOnRadioButton.DataBindings.Clear();
                        this.deleteLogOnRadioButton.Enabled = isLocalRule;
                        this.deleteLogOnRadioButton.DataBindings.Add("Checked", rule, "IsDeleteLogOn");
                        this.deleteLogOffRadioButton.DataBindings.Clear();
                        this.deleteLogOffRadioButton.Enabled = isLocalRule;
                        this.deleteLogOffRadioButton.DataBindings.Add("Checked", rule, "IsDeleteLogOff");
                        break;
                    case LoggingActionType.Upload:
                        this.uploadLogOnRadioButton.DataBindings.Clear();
                        this.uploadLogOnRadioButton.Enabled = isLocalRule;
                        this.uploadLogOnRadioButton.DataBindings.Add("Checked", rule, "IsUploadLogOn");
                        this.uploadLogOffRadioButton.DataBindings.Clear();
                        this.uploadLogOffRadioButton.Enabled = isLocalRule;
                        this.uploadLogOffRadioButton.DataBindings.Add("Checked", rule, "IsUploadLogOff");
                        break;
                    case LoggingActionType.Download:
                        this.downloadLogOnRadioButton.DataBindings.Clear();
                        this.downloadLogOnRadioButton.Enabled = isLocalRule;
                        this.downloadLogOnRadioButton.DataBindings.Add("Checked", rule, "IsDownloadLogOn");
                        this.downloadLogOffRadioButton.DataBindings.Clear();
                        this.downloadLogOffRadioButton.Enabled = isLocalRule;
                        this.downloadLogOffRadioButton.DataBindings.Add("Checked", rule, "IsDownloadLogOff");
                        break;
                    case LoggingActionType.Import:
                        this.importLogOnRadioButton.DataBindings.Clear();
                        this.importLogOnRadioButton.Enabled = isLocalRule;
                        this.importLogOnRadioButton.DataBindings.Add("Checked", rule, "IsImportLogOn");
                        this.importLogOffRadioButton.DataBindings.Clear();
                        this.importLogOffRadioButton.Enabled = isLocalRule;
                        this.importLogOffRadioButton.DataBindings.Add("Checked", rule, "IsImportLogOff");
                        break;
                    case LoggingActionType.Export:
                        this.exportLogOnRadioButton.DataBindings.Clear();
                        this.exportLogOnRadioButton.Enabled = isLocalRule;
                        this.exportLogOnRadioButton.DataBindings.Add("Checked", rule, "IsExportLogOn");
                        this.exportLogOffRadioButton.DataBindings.Clear();
                        this.exportLogOffRadioButton.Enabled = isLocalRule;
                        this.exportLogOffRadioButton.DataBindings.Add("Checked", rule, "IsExportLogOff");
                        break;
                }
            }

            this.allowLogingRulePassDownCheckBox.Enabled = isLocalRule;
            this.allowLogingRulePassDownCheckBox.DataBindings.Clear();
            this.allowLogingRulePassDownCheckBox.DataBindings.Add("Checked", rule, "AllowPropagation");
        }

        /// <summary>
        /// Show the subscribers associated with a class element
        /// </summary>
        /// <param name="metaDataElement">The mete data element</param>
        private void ShowSubscribers(IMetaDataElement metaDataElement)
        {
            ClassElement clsElement = metaDataElement as ClassElement;

            this.subscribersListView.SuspendLayout();
            this.subscribersListView.Items.Clear();
            this.subscriberPropertyGrid.SelectedObject = null;

            this.delSubscriberButton.Enabled = false;

            if (clsElement != null)
            {
                SubscriberListViewItem listViewItem;
                SubscriberCollection subscribers = _metaData.SubscriberManager.GetClassSubscribers(clsElement);
                int index = 0;
                bool isLocalSubscriber;
                foreach (Subscriber subscriber in subscribers)
                {
                    listViewItem = new SubscriberListViewItem(clsElement, subscriber);
                    subscriber.MetaData = this._metaData;
                    isLocalSubscriber = this._metaData.SubscriberManager.IsLocalSubscriberExist(clsElement, subscriber);
                    if (isLocalSubscriber)
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                    }
                    else
                    {
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InheritedRule"));
                    }

                    // select the first node by default
                    if (index == 0)
                    {
                        listViewItem.Selected = true;
                        if (isLocalSubscriber)
                        {
                            this.delSubscriberButton.Enabled = true;
                        }
                    }
                    index++;
                    this.subscribersListView.Items.Add(listViewItem);
                }

                this.addSubscriberButton.Enabled = true;
            }
            else
            {
                this.addSubscriberButton.Enabled = false;
            }

            this.subscribersListView.ResumeLayout();
        }

        /// <summary>
        /// Add a subscriber to the selected class
        /// </summary>
        private void AddClassSubscriber()
        {
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

            if (classElement != null)
            {
                Subscriber subscriber = new Subscriber();
                subscriber.ClassName = classElement.Name;
                subscriber.MetaData = this._metaData;
                // make sure the subscriber name is unique
                int index = 2;
                while (_metaData.SubscriberManager.IsSubscriberExist(classElement, subscriber))
                {
                    subscriber.Name = "Subscriber" + index;
                    index++;
                }

                _metaData.SubscriberManager.AddSubscriber(classElement, subscriber);
                SubscriberListViewItem listViewItem = new SubscriberListViewItem(classElement, subscriber);
                listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LocalRule"));
                listViewItem.Selected = true;
                this.subscribersListView.Items.Add(listViewItem);
            }
        }

        /// <summary>
        /// Remove a subscriber from currently displayed subscriber list
        /// </summary>
        /// <param name="listViewItem">The list view item for the subscriber</param>
        private void RemoveClassSubscriber(SubscriberListViewItem listViewItem)
        {
            _metaData.SubscriberManager.RemoveSubscriber(listViewItem.ClassElement, listViewItem.Subscriber);

            // refresh the subscribers in display
            this.ShowSubscribers(listViewItem.ClassElement);
        }

        /// <summary>
        /// Show the subscriber properties
        /// </summary>
        /// <param name="subscriber">The Subscriber object</param>
        private void ShowSubscriberDetail(Subscriber subscriber)
        {
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

            if (classElement != null)
            {
                if (classElement.Name != subscriber.ClassName)
                {
                    // is an inherited subscriber, no remove and no editing
                    this.delSubscriberButton.Enabled = false;
                    this.subscriberPropertyGrid.SelectedObject = subscriber;
                    this.subscriberPropertyGrid.Enabled = false;
                }
                else
                {
                    // it is a local subscriber;
                    this.delSubscriberButton.Enabled = true;
                    this.subscriberPropertyGrid.SelectedObject = subscriber;
                    this.subscriberPropertyGrid.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Show the apis associated with a class element
        /// </summary>
        /// <param name="metaDataElement">The mete data element</param>
        private void ShowApis(IMetaDataElement metaDataElement)
        {
            ClassElement clsElement = metaDataElement as ClassElement;

            this.apiListView.SuspendLayout();
            this.apiListView.Items.Clear();
            this.apiPropertyGrid.SelectedObject = null;

            this.deleteApiButton.Enabled = false;

            if (clsElement != null)
            {
                ApiListViewItem listViewItem;
                ApiCollection apis = _metaData.ApiManager.GetClassApis(clsElement.Name);
                int index = 0;
                if (apis != null)
                {
                    foreach (Api api in apis)
                    {
                        listViewItem = new ApiListViewItem(clsElement, api);
                        api.MetaData = this._metaData;
  
                        listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.Custom"));

                        // select the first node by default
                        if (index == 0)
                        {
                            listViewItem.Selected = true;
                     
                            this.deleteApiButton.Enabled = true;
                        }
                        index++;
                        this.apiListView.Items.Add(listViewItem);
                    }
                }

                //this.addApiButton.Enabled = true;
            }
            else
            {
                //this.addApiButton.Enabled = false;
            }

            this.apiListView.ResumeLayout();
        }

        /// <summary>
        /// Add a cusotm api to the selected class
        /// </summary>
        private void AddClassCustomApi()
        {
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

            if (classElement != null)
            {
                Api api = new Api();
                api.ClassName = classElement.Name;
                api.MetaData = this._metaData;
                api.MethodType = MethodType.GetMany;

                // make sure the api name is unique
                int index = 2;
                while (_metaData.ApiManager.IsApiExist(classElement.Name, api))
                {
                    api.Name = "Api" + index;
                    index++;
                }

                _metaData.ApiManager.AddApi(classElement.Name, api);
                ApiListViewItem listViewItem = new ApiListViewItem(classElement, api);
                listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.Standard"));
                listViewItem.Selected = true;
                this.apiListView.Items.Add(listViewItem);
            }
        }

        /// <summary>
        /// Add all standard apis to the selected class
        /// </summary>
        private void AddClassStandardApis()
        {
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

            if (classElement != null)
            {
                string[] apiNames = Enum.GetNames(typeof(MethodType));
                foreach (string apiName in apiNames)
                {
                    if (apiName != "Custom")
                    {
                        Api api = new Api();
                        api.Name = apiName;
                        api.ClassName = classElement.Name;
                        api.MetaData = this._metaData;
                        api.MethodType = (MethodType)Enum.Parse(typeof(MethodType), apiName);
                        api.IsAuthorized = true;
                        api.HttpMethod = GetHttpMethod(api.MethodType);

                        // make sure the api has not been added
                        if (!_metaData.ApiManager.IsApiExist(classElement.Name, api))
                        {
                            _metaData.ApiManager.AddApi(classElement.Name, api);
                            ApiListViewItem listViewItem = new ApiListViewItem(classElement, api);
                            listViewItem.SubItems.Add(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.Standard"));
                            this.apiListView.Items.Add(listViewItem);
                        }
                    }
                }
            }
        }

        private HttpMethod GetHttpMethod(MethodType methodType)
        {
            HttpMethod httpMethod = HttpMethod.GET;

            switch (methodType)
            {
                case MethodType.GetMany:
                case MethodType.GetOne:
                    httpMethod = HttpMethod.GET;
                    break;

                case MethodType.Create:
                    httpMethod = HttpMethod.POST;
                    break;
            }

            return httpMethod;
        }

        /// <summary>
        /// Remove an api from currently displayed api list
        /// </summary>
        /// <param name="listViewItem">The list view item for the api</param>
        private void RemoveClassApi(ApiListViewItem listViewItem)
        {
            _metaData.ApiManager.RemoveApi(listViewItem.ClassElement.Name, listViewItem.Api);

            // refresh the api  in display
            this.ShowApis(listViewItem.ClassElement);
        }

        /// <summary>
        /// Show the api properties
        /// </summary>
        /// <param name="api">The Api object</param>
        private void ShowApiDetail(Api api)
        {
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

            if (classElement != null)
            {
                if (classElement.Name != api.ClassName)
                {
                    // is an inherited api, no remove and no editing
                    this.deleteApiButton.Enabled = false;
                    this.apiPropertyGrid.SelectedObject = api;
                    this.apiPropertyGrid.Enabled = false;
                }
                else
                {
                    // it is a local subscriber;
                    this.deleteApiButton.Enabled = true;
                    this.apiPropertyGrid.SelectedObject = api;
                    this.apiPropertyGrid.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Initialize the states of logging rule related controls
        /// </summary>
        private void InitializeLoggingRuleControls()
        {
            this.addLoggingRuleButton.Enabled = false;
            this.removeLoggingRuleButton.Enabled = false;
            this.readLogOnRadioButton.DataBindings.Clear();
            this.readLogOnRadioButton.Checked = false;
            this.readLogOnRadioButton.Enabled = false;
            this.readLogOffRadioButton.DataBindings.Clear();
            this.readLogOffRadioButton.Checked = false;
            this.readLogOffRadioButton.Enabled = false;
            this.modifyLogOnRadioButton.DataBindings.Clear();
            this.modifyLogOnRadioButton.Checked = false;
            this.modifyLogOnRadioButton.Enabled = false;
            this.modifyLogOffRadioButton.DataBindings.Clear();
            this.modifyLogOffRadioButton.Checked = false;
            this.modifyLogOffRadioButton.Enabled = false;
            this.createLogOnRadioButton.DataBindings.Clear();
            this.createLogOnRadioButton.Checked = false;
            this.createLogOnRadioButton.Enabled = false;
            this.createLogOffRadioButton.DataBindings.Clear();
            this.createLogOffRadioButton.Checked = false;
            this.createLogOffRadioButton.Enabled = false;
            this.deleteLogOnRadioButton.DataBindings.Clear();
            this.deleteLogOnRadioButton.Checked = false;
            this.deleteLogOnRadioButton.Enabled = false;
            this.deleteLogOffRadioButton.DataBindings.Clear();
            this.deleteLogOffRadioButton.Checked = false;
            this.deleteLogOffRadioButton.Enabled = false;
            this.uploadLogOnRadioButton.DataBindings.Clear();
            this.uploadLogOnRadioButton.Checked = false;
            this.uploadLogOnRadioButton.Enabled = false;
            this.uploadLogOffRadioButton.DataBindings.Clear();
            this.uploadLogOffRadioButton.Checked = false;
            this.uploadLogOffRadioButton.Enabled = false;
            this.downloadLogOnRadioButton.DataBindings.Clear();
            this.downloadLogOnRadioButton.Checked = false;
            this.downloadLogOnRadioButton.Enabled = false;
            this.downloadLogOffRadioButton.DataBindings.Clear();
            this.downloadLogOffRadioButton.Checked = false;
            this.downloadLogOffRadioButton.Enabled = false;
            this.importLogOnRadioButton.DataBindings.Clear();
            this.importLogOnRadioButton.Checked = false;
            this.importLogOnRadioButton.Enabled = false;
            this.importLogOffRadioButton.DataBindings.Clear();
            this.importLogOffRadioButton.Checked = false;
            this.importLogOffRadioButton.Enabled = false;
            this.exportLogOnRadioButton.DataBindings.Clear();
            this.exportLogOnRadioButton.Checked = false;
            this.exportLogOnRadioButton.Enabled = false;
            this.exportLogOffRadioButton.DataBindings.Clear();
            this.exportLogOffRadioButton.Checked = false;
            this.exportLogOffRadioButton.Enabled = false;
            this.allowLogingRulePassDownCheckBox.DataBindings.Clear();
            this.allowLogingRulePassDownCheckBox.Checked = false;
            this.allowLogingRulePassDownCheckBox.Enabled = false;
        }

		/// <summary>
		/// Obtain a lock to the meta data
		/// </summary>
		private void ObtainMetaDataLock()
		{
            try
            {
                ((DesignStudio)this.MdiParent).MetaDataService.LockMetaData(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo));

                // lock is obtained
                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LockObtained"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // obtain a lock successfully
                _metaData.IsLockObtained = true;
                _menuItemStates.SetState(MenuItemID.FileSaveDatabase, true);
                _menuItemStates.SetState(MenuItemID.ToolLock, false);
            }
            catch (Exception ex)
            {
                // locking failed
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                _metaData.IsLockObtained = false;
                _menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
                _menuItemStates.SetState(MenuItemID.ToolLock, true);
            }
		}

		/// <summary>
		/// Release the meta data lock
		/// </summary>
		private void ReleaseMetaDataLock()
		{
			try
			{
				((DesignStudio) this.MdiParent).MetaDataService.UnlockMetaData(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), true);

                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LockReleased"),
					"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

				// release the lock successfully
				_metaData.IsLockObtained = false;
				_menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
				_menuItemStates.SetState(MenuItemID.ToolLock, true);
			}
			catch (Exception ex)
			{
				// locking failed
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

        /// <summary>
        /// gets the information indicating whether a constraint has been referenced by an non-empty
        /// class or not.
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="refClass"></param>
        /// <param name="refAttribute"></param>
        /// <returns></returns>
        private bool IsConstraintUsedByNonEmptyClasses(ConstraintElementBase constraint,
            out ClassElement refClass, out AttributeElementBase refAttribute)
        {
            bool status = false;
            refClass = null;
            refAttribute = null;

            IEnumerator classEnumerator = _metaData.SchemaModel.GetClassEnumerator();
            while (classEnumerator.MoveNext())
            {
                ClassElement classElement = (ClassElement)classEnumerator.Current;
                refClass = classElement;

                foreach (SimpleAttributeElement attribute in classElement.SimpleAttributes)
                {
                    if (attribute.Constraint == constraint && !IsClassEmpty(classElement))
                    {
                        refAttribute = attribute;
                        status = true;
                        break;
                    }
                }

                if (status)
                {
                    break;
                }

                foreach (VirtualAttributeElement attribute in classElement.VirtualAttributes)
                {
                    if (attribute.Constraint == constraint && !IsClassEmpty(classElement))
                    {
                        refAttribute = attribute;
                        status = true;
                        break;
                    }
                }

                if (status)
                {
                    break;
                }
            }

            return status;
        }

        // disable the buttons associated with DataEditor
        private void DisableDataEditorMenuItems()
        {
            _menuItemStates.SetState(MenuItemID.EditShowAllInstances, false);
		    _menuItemStates.SetState(MenuItemID.EditNewInstance, false);
		    _menuItemStates.SetState(MenuItemID.EditSaveInstance, false);
		    _menuItemStates.SetState(MenuItemID.EditSaveInstanceAs, false);
		    _menuItemStates.SetState(MenuItemID.EditDeleteInstance, false);
		    _menuItemStates.SetState(MenuItemID.EditDeleteAllInstances, false);
		    _menuItemStates.SetState(MenuItemID.ViewDefault, false);
		    _menuItemStates.SetState(MenuItemID.ViewDetailed, false);
		    _menuItemStates.SetState(MenuItemID.ViewNextRow, false);
		    _menuItemStates.SetState(MenuItemID.ViewPrevRow, false);
		    _menuItemStates.SetState(MenuItemID.ViewNextPage, false);
		    _menuItemStates.SetState(MenuItemID.ViewPrevPage, false);
            _menuItemStates.SetState(MenuItemID.ViewRowCount, false);
        }

		#endregion

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Get the selected node
			MetaDataTreeNode node = (MetaDataTreeNode) e.Node;

			if (node != null)
			{
				// show the items under the selected node in the list view
				this.listView1.BeginUpdate();

				_listViewBuilder.BuildItems(this.listView1, node);

				this.listView1.EndUpdate();

				// show the selected schema model element in the property grid
				this.propertyGrid1.SelectedObject = node.MetaDataElement;

				// show the xacl rules for the meta data element
				ShowXaclRules(node.MetaDataElement);

				// show data validate rules if the selected item is a class
				ShowValidateRules(node.MetaDataElement);

                // show events if the selected item is a class
                ShowEvents(node.MetaDataElement);

                // show the logging rules for the meta data element
                ShowLoggingRules(node.MetaDataElement);

                // show subscribers if the selected item is a class
                ShowSubscribers(node.MetaDataElement);

                // show apis if the selected item is a class
                ShowApis(node.MetaDataElement);

                // set the menu item states
                SetMenuItemStates(node);

				// show caption of the selected object in the status bar
				string text = "";
				if (node.MetaDataElement != null)
				{
					text = node.MetaDataElement.Caption;
				}

				((DesignStudio) this.MdiParent).ShowSelectionStatus(text);
			}
		}

		private void viewLargeIconMenuItem_Click(object sender, System.EventArgs e)
		{
			this.viewDetailMenuItem.Checked = false;
			this.viewLargeIconMenuItem.Checked = true;
			this.viewListMenuItem.Checked = false;
			this.viewSmallIcomMenuItem.Checked = false;

			this.listView1.View = View.LargeIcon;
		}

		private void viewSmallIcomMenuItem_Click(object sender, System.EventArgs e)
		{
			this.viewDetailMenuItem.Checked = false;
			this.viewLargeIconMenuItem.Checked = false;
			this.viewListMenuItem.Checked = false;
			this.viewSmallIcomMenuItem.Checked = true;

			this.listView1.View = View.SmallIcon;		
		}

		private void viewListMenuItem_Click(object sender, System.EventArgs e)
		{
			this.viewDetailMenuItem.Checked = false;
			this.viewLargeIconMenuItem.Checked = false;
			this.viewListMenuItem.Checked = true;
			this.viewSmallIcomMenuItem.Checked = false;

			this.listView1.View = View.List;		
		}

		private void viewDetailMenuItem_Click(object sender, System.EventArgs e)
		{
			this.viewDetailMenuItem.Checked = true;
			this.viewLargeIconMenuItem.Checked = false;
			this.viewListMenuItem.Checked = false;
			this.viewSmallIcomMenuItem.Checked = false;

			this.listView1.View = View.Details;		
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			ListView.SelectedListViewItemCollection items = this.listView1.SelectedItems;

			if (items.Count == 1)
			{
				MetaDataListViewItem item = (MetaDataListViewItem) items[0];

				// select the tree node
                if (item.Type == Newtera.WinClientCommon.NodeType.ClassesFolder ||
                    item.Type == Newtera.WinClientCommon.NodeType.ConstraintsFolder ||
                    item.Type == Newtera.WinClientCommon.NodeType.ClassNode ||
                    item.Type == Newtera.WinClientCommon.NodeType.TaxonomyNode ||
                    item.Type == Newtera.WinClientCommon.NodeType.TaxonNode ||
                    item.Type == Newtera.WinClientCommon.NodeType.DataViewsFolder ||
                    item.Type == Newtera.WinClientCommon.NodeType.SelectorsFolder ||
                    item.Type == Newtera.WinClientCommon.NodeType.XMLSchemaViewsFolder ||
                    item.Type == Newtera.WinClientCommon.NodeType.XMLSchemaView ||
                    item.Type == Newtera.WinClientCommon.NodeType.CategoryFolder)
				{
					treeView1.SelectedNode = item.TreeNode;
				}
			}
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ListView.SelectedListViewItemCollection items = this.listView1.SelectedItems;

			if (items.Count == 1)
			{
				MetaDataListViewItem item = (MetaDataListViewItem) items[0];

				this.propertyGrid1.SelectedObject = item.MetaDataElement;

				// show the xacl rules for the meta data element
				ShowXaclRules(item.MetaDataElement);

				// show data validate rules if the selected item is a class
				ShowValidateRules(item.MetaDataElement);

                // show events if the selected item is a class
                ShowEvents(item.MetaDataElement);

                // show the logging rules for the meta data element
                ShowLoggingRules(item.MetaDataElement);

                // show subscribers if the selected item is a class
                ShowSubscribers(item.MetaDataElement);

                // show apis if the selected item is a class
                ShowApis(item.MetaDataElement);

				// show the caption of selected object in the status bar
				if (item.MetaDataElement != null)
				{
					((DesignStudio) this.MdiParent).ShowSelectionStatus(item.MetaDataElement.Caption);
				}
			}		
		}

		private void Reset_Click(object sender, System.EventArgs e)
		{
			object theInstance = this.propertyGrid1.SelectedObject;
			if (theInstance != null)
			{
                if (theInstance is SchemaModelElement)
                {
                    // reset on schema model element may have side effects, therefore, disallow
                    // on reset in any schema model elements
                    MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.DisallowReset"),
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    PropertyDescriptor descriptor = this.propertyGrid1.SelectedGridItem.PropertyDescriptor;
                    descriptor.ResetValue(theInstance);
                    this.propertyGrid1.Refresh();
                }
			}
		}

		private void propertyContextMenu_Popup(object sender, System.EventArgs e)
		{
			object theInstance = this.propertyGrid1.SelectedObject;
			if (theInstance != null)
			{
				PropertyDescriptor descriptor = this.propertyGrid1.SelectedGridItem.PropertyDescriptor;
				bool canReset = false;
				if (descriptor != null)
				{
					canReset = descriptor.CanResetValue(theInstance);
				}
		
				// disable the menu item if the value is already a default value
				this.Reset.Enabled = canReset;

				// check the description menu item if the description area is visible
				this.description.Checked = this.propertyGrid1.HelpVisible;
			}
		}

		private void description_Click(object sender, System.EventArgs e)
		{
			// make the help area visible if it is not, otherwise, invisible
			this.propertyGrid1.HelpVisible = ! (this.description.Checked);
			this.description.Checked = ! (this.description.Checked);
		}

		private void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			MetaDataTreeNode clickedTreeNode = (MetaDataTreeNode) treeView1.GetNodeAt(e.X, e.Y);
			if (clickedTreeNode != null)
			{
				// so that After_select event is fired even the same node is clicked
				this.treeView1.SelectedNode = null;
				this.treeView1.SelectedNode = clickedTreeNode;
			}
		}

		private void listView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			MetaDataListViewItem clickedListViewItem = (MetaDataListViewItem) listView1.GetItemAt(e.X, e.Y);
			if (clickedListViewItem != null)
			{
				this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                if (clickedListViewItem.Type == Newtera.WinClientCommon.NodeType.DataViewNode ||
                    clickedListViewItem.Type == Newtera.WinClientCommon.NodeType.SelectorNode)
				{
					this._menuItemStates.SetState(MenuItemID.EditModify, true);
				}
				else
				{
					this._menuItemStates.SetState(MenuItemID.EditModify, false);
				}

                if (clickedListViewItem.Type == Newtera.WinClientCommon.NodeType.TaxonomyNode ||
                    clickedListViewItem.Type == Newtera.WinClientCommon.NodeType.TaxonNode)
                {
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, true);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, true);
                }
                else
                {
                    this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                    this._menuItemStates.SetState(MenuItemID.EditDefine, false);
                }
			}
			else
			{
				this._menuItemStates.SetState(MenuItemID.EditDelete, false);
				this._menuItemStates.SetState(MenuItemID.EditModify, false);
                this._menuItemStates.SetState(MenuItemID.EditUpdate, false);
                this._menuItemStates.SetState(MenuItemID.EditDefine, false);
			}
		}

		private void lvAddMenuItem_Click(object sender, System.EventArgs e)
		{
			AddNewItem(sender, e);
		}

		private void tvAddMenuItem_Click(object sender, System.EventArgs e)
		{
			AddNewItem(sender, e);	
		}

		private void tvCollapseMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.treeView1.SelectedNode != null)
			{
				this.treeView1.SelectedNode.Collapse();
			}
		}

		private void tvExpandMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.treeView1.SelectedNode != null)
			{
				this.treeView1.SelectedNode.Expand();
			}		
		}

		private void lvLargeIconMenuItem_Click(object sender, System.EventArgs e)
		{
			this.listView1.View = View.LargeIcon;
		}

		private void lvSamllIconMenuItem_Click(object sender, System.EventArgs e)
		{
			this.listView1.View = View.SmallIcon;		
		}

		private void lvListMenuItem_Click(object sender, System.EventArgs e)
		{
			this.listView1.View = View.List;		
		}

		private void lvDetailMenuItem_Click(object sender, System.EventArgs e)
		{
			this.listView1.View = View.Details;		
		}

		private void lvDeleteMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteItem(sender, e);	
		}

		private void tvDeleteMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteItem(sender, e);
		}

		private void tvModifyMenuItem_Click(object sender, System.EventArgs e)
		{
			ModifyItem(sender, e);
		}

		private void lvModifyMenuItem_Click(object sender, System.EventArgs e)
		{
			ModifyItem(sender, e);		
		}

		private void modifyMenuItem_Click(object sender, System.EventArgs e)
		{
			ModifyItem(sender, e);		
		}

        private void tvUpdateMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHierarchy(sender, e);
        }

        private void tvDefineMenuItem_Click(object sender, EventArgs e)
        {
            DefineHierarchy(sender, e);
        }

        private void updateMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHierarchy(sender, e);
        }

        private void lvUpdateMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHierarchy(sender, e);
        }

        private void lvDefineMenuItem_Click(object sender, EventArgs e)
        {
            DefineHierarchy(sender, e);
        }

        private void defineMenuItem_Click(object sender, EventArgs e)
        {
            DefineHierarchy(sender, e);
        }

		private void saveMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveMetaDataToFile(sender, e);
		}

		private void saveAsMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveAsMetaDataToFile(sender, e);
		}

		private void addMenuItem_Click(object sender, System.EventArgs e)
		{
			AddNewItem(sender, e);		
		}

		private void deleteMenuItem_Click(object sender, System.EventArgs e)
		{
			DeleteItem(sender, e);		
		}

		private void saveToDatabaseMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveMetaDataToDatabase(sender, e);
		}

		private void saveToDatabaseAsMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveAsSchemaToDatabase(sender, e);
		}

		private void updateLogMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowUpdateLog(sender, e);
		}

		private void propertyGrid1_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
            string msg;
			if (this.propertyGrid1.SelectedObject is SchemaModelElement)
			{
				_metaData.SchemaModel.IsAltered = true;

				// verify if the value change is valid
				msg = ValidateValueChange((SchemaModelElement) this.propertyGrid1.SelectedObject,
					e.ChangedItem.PropertyDescriptor.Name,
					e.ChangedItem.Value, e.OldValue);
				if (msg != null)
				{
					// something is wrong, display the error
					MessageBox.Show(msg);
				}
			}
			else if (this.propertyGrid1.SelectedObject is ITaxonomy)
			{
				_metaData.Taxonomies.IsAltered = true;

                // verify if the value change is valid
                msg = ValidateValueChange((IDataViewElement)this.propertyGrid1.SelectedObject,
                    e.ChangedItem.PropertyDescriptor.Name,
                    e.ChangedItem.Value, e.OldValue);
                if (msg != null)
                {
                    // something is wrong, display the error
                    MessageBox.Show(msg);
                }
			}
		}

		private void validateMenuItem_Click(object sender, System.EventArgs e)
		{
			ValidateMetaModel();
		}

		private void fixDBMenuItem_Click(object sender, System.EventArgs e)
		{
			FixDatabase(sender, e);
		}

		private void validateErrorDialog_Accept(object sender, System.EventArgs e)
		{
			ShowEntryEventArgs args = (ShowEntryEventArgs) e;

			// select the corresponding tree node in tree view
            if (args.Element != null)
            {
                TreeNode treeNode = _treeBuilder.GetTreeNode(args.Element);
                if (treeNode != null)
                {
                    this.treeView1.SelectedNode = treeNode;
                }
            }
		}

		private void aclOptionMenuItem_Click(object sender, System.EventArgs e)
		{
			AccessControlOptionDialog dialog = new AccessControlOptionDialog();
			dialog.XaclPolicy = _metaData.XaclPolicy;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_metaData.XaclPolicy.Setting.ConflictResolutionType = dialog.ConflictResolution;
				_metaData.XaclPolicy.Setting.DefaultReadPermission = dialog.DefaultReadPermission;
				_metaData.XaclPolicy.Setting.DefaultWritePermission = dialog.DefaultWritePermission;
				_metaData.XaclPolicy.Setting.DefaultCreatePermission = dialog.DefaultCreatePermission;
				_metaData.XaclPolicy.Setting.DefaultDeletePermission = dialog.DefaultDeletePermission;
                _metaData.XaclPolicy.Setting.DefaultDownloadPermission = dialog.DefaultDownloadPermission;
                _metaData.XaclPolicy.Setting.DefaultUploadPermission = dialog.DefaultUploadPermission;
			}
		}

		private void addRuleButton_Click(object sender, System.EventArgs e)
		{
			CreateUserOrRoleDialog dialog = new CreateUserOrRoleDialog();
			dialog.IsReadOnly = true;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				AddXaclRule(dialog.SelectedUser, dialog.SelectedRoles);
			}
		}

		private void ruleListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.ruleListView.SelectedItems.Count == 1)
			{
				XaclRuleListViewItem selectedItem = (XaclRuleListViewItem) this.ruleListView.SelectedItems[0];

				// TODO, shift focus so that controls with databinding can generate an event
				// to write data back to its source. Figure out a better way to do this.
				//this.permissionGroupBox.Focus();

				ShowXaclRuleDetail(selectedItem.Rule);
			}
		}

		private void removeRuleButton_Click(object sender, System.EventArgs e)
		{
			if (this.ruleListView.SelectedItems.Count == 1)
			{
				XaclRuleListViewItem selectedItem = (XaclRuleListViewItem) this.ruleListView.SelectedItems[0];

				RemoveXaclRule(selectedItem);

				this.removeRuleButton.Enabled = false;
			}		
		}

		private void permissionRadioButton_Click(object sender, System.EventArgs e)
		{
			this.permissionGroupBox.Focus();
			this.permissionGroupBox.Refresh();
		}

		private void allowPropagateCheckBox_CheckStateChanged(object sender, System.EventArgs e)
		{
			if (this.ruleListView.SelectedItems.Count == 1)
			{
				XaclRuleListViewItem selectedItem = (XaclRuleListViewItem) this.ruleListView.SelectedItems[0];

				string href = ((IXaclObject) this.propertyGrid1.SelectedObject).ToXPath();
				bool isLocalRule;
				if (selectedItem.Rule.ObjectHref != href)
				{
					isLocalRule = false;
				}
				else
				{
					isLocalRule = true;
				}

                if (isLocalRule &&
                    selectedItem.Rule.AllowPropagation != this.allowPropagateCheckBox.Checked)
                {
                    selectedItem.Rule.AllowPropagation = this.allowPropagateCheckBox.Checked;
                }
		    }
		}

		private void validateRuleListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.modifyValidateRuleBtn.Enabled = false;
			this.removeValidateRuleBtn.Enabled = false;

			if (this.validateRuleListView.SelectedItems.Count == 1)
			{
                ClassElement selectedClass = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;

				if (selectedClass != null)
				{
					// enable the modify and remove buttons for local rule
					ValidateRuleListViewItem selectedItem = (ValidateRuleListViewItem) this.validateRuleListView.SelectedItems[0];
					if (selectedItem.Rule.ClassName == selectedClass.Name)
					{
						this.modifyValidateRuleBtn.Enabled = true;
						this.removeValidateRuleBtn.Enabled = true;
					}
				}
			}
		}

		private void addValidateRuleBtn_Click(object sender, System.EventArgs e)
		{
			DefineValidatingRuleDialog dialog = new DefineValidatingRuleDialog();
            ClassElement classElement = ((MetaDataTreeNode)this.treeView1.SelectedNode).MetaDataElement as ClassElement;
            if (classElement != null)
            {
                RuleDef ruleDef = new RuleDef();
                ruleDef.ClassName = classElement.Name;
                ruleDef.DataView = _metaData.GetDetailedDataView(classElement.Name);
                dialog.RuleDef = ruleDef;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AddValidateRule(dialog.RuleDef);
                }
            }
		}

		private void modifyValidateRuleBtn_Click(object sender, System.EventArgs e)
		{
			if (this.validateRuleListView.SelectedItems.Count == 1)
			{
				ValidateRuleListViewItem selectedItem = (ValidateRuleListViewItem) this.validateRuleListView.SelectedItems[0];

				ModifyValidateRule(selectedItem);
			}		
		}

		private void removeValidateRuleBtn_Click(object sender, System.EventArgs e)
		{
			if (this.validateRuleListView.SelectedItems.Count == 1)
			{
				ValidateRuleListViewItem selectedItem = (ValidateRuleListViewItem) this.validateRuleListView.SelectedItems[0];

				RemoveValidateRule(selectedItem);

				this.modifyValidateRuleBtn.Enabled = false;
				this.removeValidateRuleBtn.Enabled = false;
			}		
		}

		private void SchemaEditor_Activated(object sender, System.EventArgs e)
		{
			_menuItemStates.RestoreMenuStates();

            // only one schema editor for a database
            _menuItemStates.SetState(MenuItemID.FileOpenDatabase, false);

            // disable the tool bar items for DataEditor
            DisableDataEditorMenuItems();

			// set the meta data as the current meta data
			MetaDataStore.Instance.SelectMetaData(_metaData.SchemaInfo);
		}

		private void tvArrangeMenuItem_Click(object sender, System.EventArgs e)
		{
			ArrangeItems(sender, e);
		}

		private void lvArrangeMenuItem_Click(object sender, System.EventArgs e)
		{
			ArrangeItems(sender, e);		
		}

		private void arrangeMenuItem_Click(object sender, System.EventArgs e)
		{
			ArrangeItems(sender, e);
		}

		private void conditionTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (this.ruleListView.SelectedItems.Count == 1)
			{
				XaclRuleListViewItem selectedItem = (XaclRuleListViewItem) this.ruleListView.SelectedItems[0];


				selectedItem.Rule.Condition.Condition = this.conditionTextBox.Text;
			}
		}

		private void unlockMenuItem_Click(object sender, System.EventArgs e)
		{
			ReleaseMetaDataLock();
		}

		private void lockMenuItem_Click(object sender, System.EventArgs e)
		{
			ObtainMetaDataLock();
		}

        private void eventListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.eventListView.SelectedItems.Count == 1)
            {
                EventListViewItem selectedItem = (EventListViewItem)this.eventListView.SelectedItems[0];

                ShowEventDetail(selectedItem.Event);

                bool isLocalEvent = this._metaData.EventManager.IsLocalEventExist(selectedItem.ClassElement, selectedItem.Event);

                if (isLocalEvent)
                {
                    this.deleteEventButton.Enabled = true;
                }
                else
                {
                    this.deleteEventButton.Enabled = false;
                }
            }
        }

        private void addEventButton_Click(object sender, EventArgs e)
        {
            AddClassEvent();
        }

        private void deleteEventButton_Click(object sender, EventArgs e)
        {
            if (this.eventListView.SelectedItems.Count == 1)
            {
                EventListViewItem selectedItem = (EventListViewItem)this.eventListView.SelectedItems[0];

                RemoveClassEvent(selectedItem);
            }	
        }

        private void eventPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (this.eventPropertyGrid.SelectedObject is EventDef &&
                e.ChangedItem.PropertyDescriptor.Name == "Name")
            {
                this.eventListView.SelectedItems[0].Text = (string) e.ChangedItem.Value;
            }

            // mark the event manager as altered
            this._metaData.EventManager.IsAltered = true;
        }

        private void SchemaEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_metaData.NeedToSave)
            {
                DialogResult result = MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.SchemaChanged"),
                    "Confirm Dialog", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveMetaDataToFile(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            return;
        }

        private void SchemaEditor_Deactivate(object sender, EventArgs e)
        {
            _menuItemStates.SetState(MenuItemID.FileOpenDatabase, true);
        }

        /// <summary>
        /// A handler to call when a category changed event is raised from a schema model element
        /// </summary>
        /// <param name="sender">the MetaModelElement object that raises the event.</param>
        /// <param name="e">the arguments</param>
        private void CategoryChangedHandler(object sender, EventArgs e)
        {
            if (sender is ClassElement)
            {
                RelocateTreeNode((ClassElement) sender);
            }
        }

        /// <summary>
        /// A handler to call when a value changed event is raised from a schema model element
        /// </summary>
        /// <param name="sender">the MetaModelElement object that raises the event.</param>
        /// <param name="e">the arguments</param>
        private void ValueChangedHandler(object sender, EventArgs e)
        {
        }

        private void loggingRuleListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.loggingRuleListView.SelectedItems.Count == 1)
            {
                LoggingRuleListViewItem selectedItem = (LoggingRuleListViewItem)this.loggingRuleListView.SelectedItems[0];

                // TODO, shift focus so that controls with databinding can generate an event
                // to write data back to its source. Figure out a better way to do this.
                //this.logStatusGroupBox.Focus();

                ShowLoggingRuleDetail(selectedItem.Rule);
            }
        }

        private void addLoggingRuleButton_Click(object sender, EventArgs e)
        {
            CreateUserOrRoleDialog dialog = new CreateUserOrRoleDialog();
            dialog.IsReadOnly = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddLoggingRule(dialog.SelectedUser, dialog.SelectedRoles);
            }
        }

        private void removeLoggingRuleButton_Click(object sender, EventArgs e)
        {
            if (this.loggingRuleListView.SelectedItems.Count == 1)
            {
                LoggingRuleListViewItem selectedItem = (LoggingRuleListViewItem)this.loggingRuleListView.SelectedItems[0];

                RemoveLoggingRule(selectedItem);

                this.removeLoggingRuleButton.Enabled = false;
            }	
        }

        private void logStatusRadioButton_Click(object sender, System.EventArgs e)
        {
            this.logStatusGroupBox.Focus();
            this.logStatusGroupBox.Refresh();
        }

        private void allowLogingRulePassDownCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.loggingRuleListView.SelectedItems.Count == 1)
            {
                LoggingRuleListViewItem selectedItem = (LoggingRuleListViewItem)this.loggingRuleListView.SelectedItems[0];

                string href = ((IXaclObject)this.propertyGrid1.SelectedObject).ToXPath();
                bool isLocalRule;
                if (selectedItem.Rule.ObjectHref != href)
                {
                    isLocalRule = false;
                }
                else
                {
                    isLocalRule = true;
                }

                if (isLocalRule &&
                    selectedItem.Rule.AllowPropagation != this.allowLogingRulePassDownCheckBox.Checked)
                {
                    selectedItem.Rule.AllowPropagation = this.allowLogingRulePassDownCheckBox.Checked;
                }
            }
        }

        private void loggingOptionMenuItem_Click(object sender, EventArgs e)
        {
            LoggingOptionDialog dialog = new LoggingOptionDialog();
            dialog.LoggingPolicy = _metaData.LoggingPolicy;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _metaData.LoggingPolicy.Setting.ConflictResolutionType = dialog.ConflictResolution;
                _metaData.LoggingPolicy.Setting.DefaultReadLogStatus = dialog.DefaultReadLogStatus;
                _metaData.LoggingPolicy.Setting.DefaultWriteLogStatus = dialog.DefaultWriteLogStatus;
                _metaData.LoggingPolicy.Setting.DefaultCreateLogStatus = dialog.DefaultCreateLogStatus;
                _metaData.LoggingPolicy.Setting.DefaultDeleteLogStatus = dialog.DefaultDeleteLogStatus;
                _metaData.LoggingPolicy.Setting.DefaultDownloadLogStatus = dialog.DefaultDownloadLogStatus;
                _metaData.LoggingPolicy.Setting.DefaultUploadLogStatus = dialog.DefaultUploadLogStatus;
            }
        }

        private void addSubscriberButton_Click(object sender, EventArgs e)
        {
            AddClassSubscriber();
        }

        private void delSubscriberButton_Click(object sender, EventArgs e)
        {
            if (this.subscribersListView.SelectedItems.Count == 1)
            {
                SubscriberListViewItem selectedItem = (SubscriberListViewItem)this.subscribersListView.SelectedItems[0];

                RemoveClassSubscriber(selectedItem);
            }
        }

        private void subscriberPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (this.subscriberPropertyGrid.SelectedObject is Subscriber &&
                e.ChangedItem.PropertyDescriptor.Name == "Name")
            {
                this.subscribersListView.SelectedItems[0].Text = (string)e.ChangedItem.Value;
            }

            // mark the subscriber manager as altered
            this._metaData.SubscriberManager.IsAltered = true;
        }

        private void subscribersListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.subscribersListView.SelectedItems.Count == 1)
            {
                SubscriberListViewItem selectedItem = (SubscriberListViewItem)this.subscribersListView.SelectedItems[0];

                ShowSubscriberDetail(selectedItem.Subscriber);

                bool isLocalSubscriber = this._metaData.SubscriberManager.IsLocalSubscriberExist(selectedItem.ClassElement, selectedItem.Subscriber);

                if (isLocalSubscriber)
                {
                    this.delSubscriberButton.Enabled = true;
                }
                else
                {
                    this.delSubscriberButton.Enabled = false;
                }
            }
        }

        private void tvExportMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsXMLSchemaToFile(sender, e);
        }

        private void apiListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.apiListView.SelectedItems.Count == 1)
            {
                ApiListViewItem selectedItem = (ApiListViewItem)this.apiListView.SelectedItems[0];

                ShowApiDetail(selectedItem.Api);

                this.deleteApiButton.Enabled = true;
            }
        }

        private void deleteApiButton_Click(object sender, EventArgs e)
        {
            if (this.apiListView.SelectedItems.Count == 1)
            {
                ApiListViewItem selectedItem = (ApiListViewItem)this.apiListView.SelectedItems[0];

                RemoveClassApi(selectedItem);
            }
        }

        private void apiPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (this.apiPropertyGrid.SelectedObject is Api &&
                    e.ChangedItem.PropertyDescriptor.Name == "Name")
            {
                this.apiListView.SelectedItems[0].Text = (string)e.ChangedItem.Value;
            }

            // mark the api manager as altered
            this._metaData.ApiManager.IsAltered = true;
        }

        private void addCustomButton_Click(object sender, EventArgs e)
        {
            AddClassCustomApi();
        }
    }
}

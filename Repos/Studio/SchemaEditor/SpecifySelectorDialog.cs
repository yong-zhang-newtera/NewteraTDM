using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Mappings;
using Newtera.Studio.ImportExport;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for SpecifySelectorDialog.
	/// </summary>
	public class SpecifySelectorDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData;
		private Selector _selector;
		private ClassMapping _currentClassMapping;
		private InstanceView _srcInstanceView = null;
		private InstanceView _dstInstanceView = null;
		private DefaultValueCollection _defaultValues;
		private ConnectLineAction _connectionAction = null;
		private CMDataServiceStub _dataService;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.Button prevRowButton;
		private System.Windows.Forms.Button nextRowButton;
		private Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx destinationPropertyGrid;
		private System.Windows.Forms.Splitter splitter2;
		private Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx sourcePropertyGrid;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.MenuItem deleteMenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem propertyMenuItem;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ImageList toolBarImageList;
		private System.Windows.Forms.ToolBarButton oneToOneButton;
		private System.Windows.Forms.ToolBarButton manyToOneButton;
		private System.Windows.Forms.ToolBarButton oneToManyButton;
		private System.Windows.Forms.ToolBarButton manyToManyButton;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton propertyButton;
		private System.Windows.Forms.ToolBarButton delButton;
		private System.Windows.Forms.ToolBarButton delAllButton;
		private System.Windows.Forms.MenuItem deleteAllMenuItem;
		private System.Windows.Forms.ToolBarButton arrowButton;
		private Newtera.Studio.ImportExport.MapPanel _mapPanel;
		private System.Windows.Forms.Button autoMappingButton;
		private System.Windows.Forms.ComboBox srcClassComboBox;
		private System.Windows.Forms.TextBox dstClassTextBox;
		private System.Windows.Forms.Button dstClassButton;
		private System.ComponentModel.IContainer components;

		public SpecifySelectorDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_defaultValues = new DefaultValueCollection();
			_dataService = new CMDataServiceStub();
		}

		/// <summary>
		/// Gets or sets the MetaDataModel instance
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
			}
		}

		/// <summary>
		/// Gets or sets the Selector instance
		/// </summary>
		public Selector Selector
		{
			get
			{
				return _selector;
			}
			set
			{
				_selector = value;
			}
		}

		/// <summary>
		/// Gets the attribute mappings created by the dialog
		/// </summary>
		/// <value> A AttributeMappingCollection instance</value>
		private AttributeMappingCollection AttributeMappings
		{
			get
			{
				return this._mapPanel.AttributeMappings;
			}
		}

		/// <summary>
		/// Gets the attribute default values created by the dialog
		/// </summary>
		/// <value> A AttributeMappingCollection instance</value>
		private DefaultValueCollection DefaultValues
		{
			get
			{
				return _defaultValues;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SpecifySelectorDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this._mapPanel = new Newtera.Studio.ImportExport.MapPanel();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.deleteMenuItem = new System.Windows.Forms.MenuItem();
			this.deleteAllMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.propertyMenuItem = new System.Windows.Forms.MenuItem();
			this.destinationPropertyGrid = new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx();
			this.toolBarImageList = new System.Windows.Forms.ImageList(this.components);
			this.sourcePropertyGrid = new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.prevRowButton = new System.Windows.Forms.Button();
			this.nextRowButton = new System.Windows.Forms.Button();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.arrowButton = new System.Windows.Forms.ToolBarButton();
			this.oneToOneButton = new System.Windows.Forms.ToolBarButton();
			this.manyToOneButton = new System.Windows.Forms.ToolBarButton();
			this.oneToManyButton = new System.Windows.Forms.ToolBarButton();
			this.manyToManyButton = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.propertyButton = new System.Windows.Forms.ToolBarButton();
			this.delButton = new System.Windows.Forms.ToolBarButton();
			this.delAllButton = new System.Windows.Forms.ToolBarButton();
			this.autoMappingButton = new System.Windows.Forms.Button();
			this.srcClassComboBox = new System.Windows.Forms.ComboBox();
			this.dstClassTextBox = new System.Windows.Forms.TextBox();
			this.dstClassButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.AccessibleDescription = resources.GetString("panel1.AccessibleDescription");
			this.panel1.AccessibleName = resources.GetString("panel1.AccessibleName");
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel1.Anchor")));
			this.panel1.AutoScroll = ((bool)(resources.GetObject("panel1.AutoScroll")));
			this.panel1.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel1.AutoScrollMargin")));
			this.panel1.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel1.AutoScrollMinSize")));
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this._mapPanel);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.sourcePropertyGrid);
			this.panel1.Controls.Add(this.splitter2);
			this.panel1.Controls.Add(this.destinationPropertyGrid);
			this.panel1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel1.Dock")));
			this.panel1.Enabled = ((bool)(resources.GetObject("panel1.Enabled")));
			this.panel1.Font = ((System.Drawing.Font)(resources.GetObject("panel1.Font")));
			this.panel1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel1.ImeMode")));
			this.panel1.Location = ((System.Drawing.Point)(resources.GetObject("panel1.Location")));
			this.panel1.Name = "panel1";
			this.panel1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel1.RightToLeft")));
			this.panel1.Size = ((System.Drawing.Size)(resources.GetObject("panel1.Size")));
			this.panel1.TabIndex = ((int)(resources.GetObject("panel1.TabIndex")));
			this.panel1.Text = resources.GetString("panel1.Text");
			this.panel1.Visible = ((bool)(resources.GetObject("panel1.Visible")));
			// 
			// _mapPanel
			// 
			this._mapPanel.AccessibleDescription = resources.GetString("_mapPanel.AccessibleDescription");
			this._mapPanel.AccessibleName = resources.GetString("_mapPanel.AccessibleName");
			this._mapPanel.Action = null;
			this._mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("_mapPanel.Anchor")));
			this._mapPanel.AutoScroll = ((bool)(resources.GetObject("_mapPanel.AutoScroll")));
			this._mapPanel.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("_mapPanel.AutoScrollMargin")));
			this._mapPanel.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("_mapPanel.AutoScrollMinSize")));
			this._mapPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this._mapPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_mapPanel.BackgroundImage")));
			this._mapPanel.ClassMapping = null;
			this._mapPanel.ContextMenu = this.contextMenu1;
			this._mapPanel.DestinationPropertyGrid = this.destinationPropertyGrid;
			this._mapPanel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("_mapPanel.Dock")));
			this._mapPanel.Enabled = ((bool)(resources.GetObject("_mapPanel.Enabled")));
			this._mapPanel.Font = ((System.Drawing.Font)(resources.GetObject("_mapPanel.Font")));
			this._mapPanel.ImageList = this.toolBarImageList;
			this._mapPanel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("_mapPanel.ImeMode")));
			this._mapPanel.Location = ((System.Drawing.Point)(resources.GetObject("_mapPanel.Location")));
			this._mapPanel.Name = "_mapPanel";
			this._mapPanel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("_mapPanel.RightToLeft")));
			this._mapPanel.Size = ((System.Drawing.Size)(resources.GetObject("_mapPanel.Size")));
			this._mapPanel.SourcePropertyGrid = this.sourcePropertyGrid;
			this._mapPanel.TabIndex = ((int)(resources.GetObject("_mapPanel.TabIndex")));
			this._mapPanel.Text = resources.GetString("_mapPanel.Text");
			this._mapPanel.Visible = ((bool)(resources.GetObject("_mapPanel.Visible")));
			this._mapPanel.SelectedComponentChanged += new Newtera.Studio.ImportExport.MapPanel.SelectedComponentChangedEventHandler(this.mapPanel_SelectedComponentChanged);
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.deleteMenuItem,
																						 this.deleteAllMenuItem,
																						 this.menuItem1,
																						 this.propertyMenuItem});
			this.contextMenu1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("contextMenu1.RightToLeft")));
			// 
			// deleteMenuItem
			// 
			this.deleteMenuItem.Enabled = ((bool)(resources.GetObject("deleteMenuItem.Enabled")));
			this.deleteMenuItem.Index = 0;
			this.deleteMenuItem.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("deleteMenuItem.Shortcut")));
			this.deleteMenuItem.ShowShortcut = ((bool)(resources.GetObject("deleteMenuItem.ShowShortcut")));
			this.deleteMenuItem.Text = resources.GetString("deleteMenuItem.Text");
			this.deleteMenuItem.Visible = ((bool)(resources.GetObject("deleteMenuItem.Visible")));
			this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
			// 
			// deleteAllMenuItem
			// 
			this.deleteAllMenuItem.Enabled = ((bool)(resources.GetObject("deleteAllMenuItem.Enabled")));
			this.deleteAllMenuItem.Index = 1;
			this.deleteAllMenuItem.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("deleteAllMenuItem.Shortcut")));
			this.deleteAllMenuItem.ShowShortcut = ((bool)(resources.GetObject("deleteAllMenuItem.ShowShortcut")));
			this.deleteAllMenuItem.Text = resources.GetString("deleteAllMenuItem.Text");
			this.deleteAllMenuItem.Visible = ((bool)(resources.GetObject("deleteAllMenuItem.Visible")));
			this.deleteAllMenuItem.Click += new System.EventHandler(this.deleteAllMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Enabled = ((bool)(resources.GetObject("menuItem1.Enabled")));
			this.menuItem1.Index = 2;
			this.menuItem1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem1.Shortcut")));
			this.menuItem1.ShowShortcut = ((bool)(resources.GetObject("menuItem1.ShowShortcut")));
			this.menuItem1.Text = resources.GetString("menuItem1.Text");
			this.menuItem1.Visible = ((bool)(resources.GetObject("menuItem1.Visible")));
			// 
			// propertyMenuItem
			// 
			this.propertyMenuItem.Enabled = ((bool)(resources.GetObject("propertyMenuItem.Enabled")));
			this.propertyMenuItem.Index = 3;
			this.propertyMenuItem.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("propertyMenuItem.Shortcut")));
			this.propertyMenuItem.ShowShortcut = ((bool)(resources.GetObject("propertyMenuItem.ShowShortcut")));
			this.propertyMenuItem.Text = resources.GetString("propertyMenuItem.Text");
			this.propertyMenuItem.Visible = ((bool)(resources.GetObject("propertyMenuItem.Visible")));
			this.propertyMenuItem.Click += new System.EventHandler(this.propertyMenuItem_Click);
			// 
			// destinationPropertyGrid
			// 
			this.destinationPropertyGrid.AccessibleDescription = resources.GetString("destinationPropertyGrid.AccessibleDescription");
			this.destinationPropertyGrid.AccessibleName = resources.GetString("destinationPropertyGrid.AccessibleName");
			this.destinationPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("destinationPropertyGrid.Anchor")));
			this.destinationPropertyGrid.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("destinationPropertyGrid.BackgroundImage")));
			this.destinationPropertyGrid.CommandsVisibleIfAvailable = true;
			this.destinationPropertyGrid.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("destinationPropertyGrid.Dock")));
			this.destinationPropertyGrid.Enabled = ((bool)(resources.GetObject("destinationPropertyGrid.Enabled")));
			this.destinationPropertyGrid.Font = ((System.Drawing.Font)(resources.GetObject("destinationPropertyGrid.Font")));
			this.destinationPropertyGrid.HelpVisible = ((bool)(resources.GetObject("destinationPropertyGrid.HelpVisible")));
			this.destinationPropertyGrid.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("destinationPropertyGrid.ImeMode")));
			this.destinationPropertyGrid.LargeButtons = false;
			this.destinationPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.destinationPropertyGrid.Location = ((System.Drawing.Point)(resources.GetObject("destinationPropertyGrid.Location")));
			this.destinationPropertyGrid.Name = "destinationPropertyGrid";
			this.destinationPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.destinationPropertyGrid.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("destinationPropertyGrid.RightToLeft")));
			this.destinationPropertyGrid.Size = ((System.Drawing.Size)(resources.GetObject("destinationPropertyGrid.Size")));
			this.destinationPropertyGrid.TabIndex = ((int)(resources.GetObject("destinationPropertyGrid.TabIndex")));
			this.destinationPropertyGrid.Text = resources.GetString("destinationPropertyGrid.Text");
			this.destinationPropertyGrid.ToolbarVisible = false;
			this.destinationPropertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.destinationPropertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.destinationPropertyGrid.Visible = ((bool)(resources.GetObject("destinationPropertyGrid.Visible")));
			this.destinationPropertyGrid.GridItemExpanded += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemExpandedEventHandler(this.destinationPropertyGrid_GridItemExpanded);
			this.destinationPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.destinationPropertyGrid_PropertyValueChanged);
			this.destinationPropertyGrid.GridItemCollapsed += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemCollapsedEventHandler(this.destinationPropertyGrid_GridItemCollapsed);
			// 
			// toolBarImageList
			// 
			this.toolBarImageList.ImageSize = ((System.Drawing.Size)(resources.GetObject("toolBarImageList.ImageSize")));
			this.toolBarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolBarImageList.ImageStream")));
			this.toolBarImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// sourcePropertyGrid
			// 
			this.sourcePropertyGrid.AccessibleDescription = resources.GetString("sourcePropertyGrid.AccessibleDescription");
			this.sourcePropertyGrid.AccessibleName = resources.GetString("sourcePropertyGrid.AccessibleName");
			this.sourcePropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("sourcePropertyGrid.Anchor")));
			this.sourcePropertyGrid.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("sourcePropertyGrid.BackgroundImage")));
			this.sourcePropertyGrid.CommandsVisibleIfAvailable = true;
			this.sourcePropertyGrid.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("sourcePropertyGrid.Dock")));
			this.sourcePropertyGrid.Enabled = ((bool)(resources.GetObject("sourcePropertyGrid.Enabled")));
			this.sourcePropertyGrid.Font = ((System.Drawing.Font)(resources.GetObject("sourcePropertyGrid.Font")));
			this.sourcePropertyGrid.HelpVisible = ((bool)(resources.GetObject("sourcePropertyGrid.HelpVisible")));
			this.sourcePropertyGrid.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("sourcePropertyGrid.ImeMode")));
			this.sourcePropertyGrid.LargeButtons = false;
			this.sourcePropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.sourcePropertyGrid.Location = ((System.Drawing.Point)(resources.GetObject("sourcePropertyGrid.Location")));
			this.sourcePropertyGrid.Name = "sourcePropertyGrid";
			this.sourcePropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.sourcePropertyGrid.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("sourcePropertyGrid.RightToLeft")));
			this.sourcePropertyGrid.Size = ((System.Drawing.Size)(resources.GetObject("sourcePropertyGrid.Size")));
			this.sourcePropertyGrid.TabIndex = ((int)(resources.GetObject("sourcePropertyGrid.TabIndex")));
			this.sourcePropertyGrid.Text = resources.GetString("sourcePropertyGrid.Text");
			this.sourcePropertyGrid.ToolbarVisible = false;
			this.sourcePropertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.sourcePropertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.sourcePropertyGrid.Visible = ((bool)(resources.GetObject("sourcePropertyGrid.Visible")));
			this.sourcePropertyGrid.GridItemExpanded += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemExpandedEventHandler(this.sourcePropertyGrid_GridItemExpanded);
			this.sourcePropertyGrid.GridItemCollapsed += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemCollapsedEventHandler(this.sourcePropertyGrid_GridItemCollapsed);
			// 
			// splitter1
			// 
			this.splitter1.AccessibleDescription = resources.GetString("splitter1.AccessibleDescription");
			this.splitter1.AccessibleName = resources.GetString("splitter1.AccessibleName");
			this.splitter1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitter1.Anchor")));
			this.splitter1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitter1.BackgroundImage")));
			this.splitter1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitter1.Dock")));
			this.splitter1.Enabled = ((bool)(resources.GetObject("splitter1.Enabled")));
			this.splitter1.Font = ((System.Drawing.Font)(resources.GetObject("splitter1.Font")));
			this.splitter1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitter1.ImeMode")));
			this.splitter1.Location = ((System.Drawing.Point)(resources.GetObject("splitter1.Location")));
			this.splitter1.MinExtra = ((int)(resources.GetObject("splitter1.MinExtra")));
			this.splitter1.MinSize = ((int)(resources.GetObject("splitter1.MinSize")));
			this.splitter1.Name = "splitter1";
			this.splitter1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitter1.RightToLeft")));
			this.splitter1.Size = ((System.Drawing.Size)(resources.GetObject("splitter1.Size")));
			this.splitter1.TabIndex = ((int)(resources.GetObject("splitter1.TabIndex")));
			this.splitter1.TabStop = false;
			this.splitter1.Visible = ((bool)(resources.GetObject("splitter1.Visible")));
			// 
			// splitter2
			// 
			this.splitter2.AccessibleDescription = resources.GetString("splitter2.AccessibleDescription");
			this.splitter2.AccessibleName = resources.GetString("splitter2.AccessibleName");
			this.splitter2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitter2.Anchor")));
			this.splitter2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitter2.BackgroundImage")));
			this.splitter2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitter2.Dock")));
			this.splitter2.Enabled = ((bool)(resources.GetObject("splitter2.Enabled")));
			this.splitter2.Font = ((System.Drawing.Font)(resources.GetObject("splitter2.Font")));
			this.splitter2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitter2.ImeMode")));
			this.splitter2.Location = ((System.Drawing.Point)(resources.GetObject("splitter2.Location")));
			this.splitter2.MinExtra = ((int)(resources.GetObject("splitter2.MinExtra")));
			this.splitter2.MinSize = ((int)(resources.GetObject("splitter2.MinSize")));
			this.splitter2.Name = "splitter2";
			this.splitter2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitter2.RightToLeft")));
			this.splitter2.Size = ((System.Drawing.Size)(resources.GetObject("splitter2.Size")));
			this.splitter2.TabIndex = ((int)(resources.GetObject("splitter2.TabIndex")));
			this.splitter2.TabStop = false;
			this.splitter2.Visible = ((bool)(resources.GetObject("splitter2.Visible")));
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			// 
			// okButton
			// 
			this.okButton.AccessibleDescription = resources.GetString("okButton.AccessibleDescription");
			this.okButton.AccessibleName = resources.GetString("okButton.AccessibleName");
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("okButton.Anchor")));
			this.okButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("okButton.BackgroundImage")));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("okButton.Dock")));
			this.okButton.Enabled = ((bool)(resources.GetObject("okButton.Enabled")));
			this.okButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("okButton.FlatStyle")));
			this.okButton.Font = ((System.Drawing.Font)(resources.GetObject("okButton.Font")));
			this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
			this.okButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.ImageAlign")));
			this.okButton.ImageIndex = ((int)(resources.GetObject("okButton.ImageIndex")));
			this.okButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("okButton.ImeMode")));
			this.okButton.Location = ((System.Drawing.Point)(resources.GetObject("okButton.Location")));
			this.okButton.Name = "okButton";
			this.okButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("okButton.RightToLeft")));
			this.okButton.Size = ((System.Drawing.Size)(resources.GetObject("okButton.Size")));
			this.okButton.TabIndex = ((int)(resources.GetObject("okButton.TabIndex")));
			this.okButton.Text = resources.GetString("okButton.Text");
			this.okButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.TextAlign")));
			this.okButton.Visible = ((bool)(resources.GetObject("okButton.Visible")));
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.AccessibleDescription = resources.GetString("cancelButton.AccessibleDescription");
			this.cancelButton.AccessibleName = resources.GetString("cancelButton.AccessibleName");
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("cancelButton.Anchor")));
			this.cancelButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cancelButton.BackgroundImage")));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("cancelButton.Dock")));
			this.cancelButton.Enabled = ((bool)(resources.GetObject("cancelButton.Enabled")));
			this.cancelButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("cancelButton.FlatStyle")));
			this.cancelButton.Font = ((System.Drawing.Font)(resources.GetObject("cancelButton.Font")));
			this.cancelButton.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton.Image")));
			this.cancelButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("cancelButton.ImageAlign")));
			this.cancelButton.ImageIndex = ((int)(resources.GetObject("cancelButton.ImageIndex")));
			this.cancelButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("cancelButton.ImeMode")));
			this.cancelButton.Location = ((System.Drawing.Point)(resources.GetObject("cancelButton.Location")));
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("cancelButton.RightToLeft")));
			this.cancelButton.Size = ((System.Drawing.Size)(resources.GetObject("cancelButton.Size")));
			this.cancelButton.TabIndex = ((int)(resources.GetObject("cancelButton.TabIndex")));
			this.cancelButton.Text = resources.GetString("cancelButton.Text");
			this.cancelButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("cancelButton.TextAlign")));
			this.cancelButton.Visible = ((bool)(resources.GetObject("cancelButton.Visible")));
			// 
			// prevRowButton
			// 
			this.prevRowButton.AccessibleDescription = resources.GetString("prevRowButton.AccessibleDescription");
			this.prevRowButton.AccessibleName = resources.GetString("prevRowButton.AccessibleName");
			this.prevRowButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("prevRowButton.Anchor")));
			this.prevRowButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("prevRowButton.BackgroundImage")));
			this.prevRowButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("prevRowButton.Dock")));
			this.prevRowButton.Enabled = ((bool)(resources.GetObject("prevRowButton.Enabled")));
			this.prevRowButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("prevRowButton.FlatStyle")));
			this.prevRowButton.Font = ((System.Drawing.Font)(resources.GetObject("prevRowButton.Font")));
			this.prevRowButton.Image = ((System.Drawing.Image)(resources.GetObject("prevRowButton.Image")));
			this.prevRowButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("prevRowButton.ImageAlign")));
			this.prevRowButton.ImageIndex = ((int)(resources.GetObject("prevRowButton.ImageIndex")));
			this.prevRowButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("prevRowButton.ImeMode")));
			this.prevRowButton.Location = ((System.Drawing.Point)(resources.GetObject("prevRowButton.Location")));
			this.prevRowButton.Name = "prevRowButton";
			this.prevRowButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("prevRowButton.RightToLeft")));
			this.prevRowButton.Size = ((System.Drawing.Size)(resources.GetObject("prevRowButton.Size")));
			this.prevRowButton.TabIndex = ((int)(resources.GetObject("prevRowButton.TabIndex")));
			this.prevRowButton.Text = resources.GetString("prevRowButton.Text");
			this.prevRowButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("prevRowButton.TextAlign")));
			this.prevRowButton.Visible = ((bool)(resources.GetObject("prevRowButton.Visible")));
			this.prevRowButton.Click += new System.EventHandler(this.prevRowButton_Click);
			// 
			// nextRowButton
			// 
			this.nextRowButton.AccessibleDescription = resources.GetString("nextRowButton.AccessibleDescription");
			this.nextRowButton.AccessibleName = resources.GetString("nextRowButton.AccessibleName");
			this.nextRowButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("nextRowButton.Anchor")));
			this.nextRowButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("nextRowButton.BackgroundImage")));
			this.nextRowButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("nextRowButton.Dock")));
			this.nextRowButton.Enabled = ((bool)(resources.GetObject("nextRowButton.Enabled")));
			this.nextRowButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("nextRowButton.FlatStyle")));
			this.nextRowButton.Font = ((System.Drawing.Font)(resources.GetObject("nextRowButton.Font")));
			this.nextRowButton.Image = ((System.Drawing.Image)(resources.GetObject("nextRowButton.Image")));
			this.nextRowButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("nextRowButton.ImageAlign")));
			this.nextRowButton.ImageIndex = ((int)(resources.GetObject("nextRowButton.ImageIndex")));
			this.nextRowButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("nextRowButton.ImeMode")));
			this.nextRowButton.Location = ((System.Drawing.Point)(resources.GetObject("nextRowButton.Location")));
			this.nextRowButton.Name = "nextRowButton";
			this.nextRowButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("nextRowButton.RightToLeft")));
			this.nextRowButton.Size = ((System.Drawing.Size)(resources.GetObject("nextRowButton.Size")));
			this.nextRowButton.TabIndex = ((int)(resources.GetObject("nextRowButton.TabIndex")));
			this.nextRowButton.Text = resources.GetString("nextRowButton.Text");
			this.nextRowButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("nextRowButton.TextAlign")));
			this.nextRowButton.Visible = ((bool)(resources.GetObject("nextRowButton.Visible")));
			this.nextRowButton.Click += new System.EventHandler(this.nextRowButton_Click);
			// 
			// toolBar
			// 
			this.toolBar.AccessibleDescription = resources.GetString("toolBar.AccessibleDescription");
			this.toolBar.AccessibleName = resources.GetString("toolBar.AccessibleName");
			this.toolBar.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("toolBar.Anchor")));
			this.toolBar.Appearance = ((System.Windows.Forms.ToolBarAppearance)(resources.GetObject("toolBar.Appearance")));
			this.toolBar.AutoSize = ((bool)(resources.GetObject("toolBar.AutoSize")));
			this.toolBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolBar.BackgroundImage")));
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.arrowButton,
																					   this.oneToOneButton,
																					   this.manyToOneButton,
																					   this.oneToManyButton,
																					   this.manyToManyButton,
																					   this.toolBarButton1,
																					   this.propertyButton,
																					   this.delButton,
																					   this.delAllButton});
			this.toolBar.ButtonSize = ((System.Drawing.Size)(resources.GetObject("toolBar.ButtonSize")));
			this.toolBar.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("toolBar.Dock")));
			this.toolBar.DropDownArrows = ((bool)(resources.GetObject("toolBar.DropDownArrows")));
			this.toolBar.Enabled = ((bool)(resources.GetObject("toolBar.Enabled")));
			this.toolBar.Font = ((System.Drawing.Font)(resources.GetObject("toolBar.Font")));
			this.toolBar.ImageList = this.toolBarImageList;
			this.toolBar.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("toolBar.ImeMode")));
			this.toolBar.Location = ((System.Drawing.Point)(resources.GetObject("toolBar.Location")));
			this.toolBar.Name = "toolBar";
			this.toolBar.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("toolBar.RightToLeft")));
			this.toolBar.ShowToolTips = ((bool)(resources.GetObject("toolBar.ShowToolTips")));
			this.toolBar.Size = ((System.Drawing.Size)(resources.GetObject("toolBar.Size")));
			this.toolBar.TabIndex = ((int)(resources.GetObject("toolBar.TabIndex")));
			this.toolBar.TextAlign = ((System.Windows.Forms.ToolBarTextAlign)(resources.GetObject("toolBar.TextAlign")));
			this.toolBar.Visible = ((bool)(resources.GetObject("toolBar.Visible")));
			this.toolBar.Wrappable = ((bool)(resources.GetObject("toolBar.Wrappable")));
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// arrowButton
			// 
			this.arrowButton.Enabled = ((bool)(resources.GetObject("arrowButton.Enabled")));
			this.arrowButton.ImageIndex = ((int)(resources.GetObject("arrowButton.ImageIndex")));
			this.arrowButton.Pushed = true;
			this.arrowButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.arrowButton.Text = resources.GetString("arrowButton.Text");
			this.arrowButton.ToolTipText = resources.GetString("arrowButton.ToolTipText");
			this.arrowButton.Visible = ((bool)(resources.GetObject("arrowButton.Visible")));
			// 
			// oneToOneButton
			// 
			this.oneToOneButton.Enabled = ((bool)(resources.GetObject("oneToOneButton.Enabled")));
			this.oneToOneButton.ImageIndex = ((int)(resources.GetObject("oneToOneButton.ImageIndex")));
			this.oneToOneButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.oneToOneButton.Text = resources.GetString("oneToOneButton.Text");
			this.oneToOneButton.ToolTipText = resources.GetString("oneToOneButton.ToolTipText");
			this.oneToOneButton.Visible = ((bool)(resources.GetObject("oneToOneButton.Visible")));
			// 
			// manyToOneButton
			// 
			this.manyToOneButton.Enabled = ((bool)(resources.GetObject("manyToOneButton.Enabled")));
			this.manyToOneButton.ImageIndex = ((int)(resources.GetObject("manyToOneButton.ImageIndex")));
			this.manyToOneButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.manyToOneButton.Text = resources.GetString("manyToOneButton.Text");
			this.manyToOneButton.ToolTipText = resources.GetString("manyToOneButton.ToolTipText");
			this.manyToOneButton.Visible = ((bool)(resources.GetObject("manyToOneButton.Visible")));
			// 
			// oneToManyButton
			// 
			this.oneToManyButton.Enabled = ((bool)(resources.GetObject("oneToManyButton.Enabled")));
			this.oneToManyButton.ImageIndex = ((int)(resources.GetObject("oneToManyButton.ImageIndex")));
			this.oneToManyButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.oneToManyButton.Text = resources.GetString("oneToManyButton.Text");
			this.oneToManyButton.ToolTipText = resources.GetString("oneToManyButton.ToolTipText");
			this.oneToManyButton.Visible = ((bool)(resources.GetObject("oneToManyButton.Visible")));
			// 
			// manyToManyButton
			// 
			this.manyToManyButton.Enabled = ((bool)(resources.GetObject("manyToManyButton.Enabled")));
			this.manyToManyButton.ImageIndex = ((int)(resources.GetObject("manyToManyButton.ImageIndex")));
			this.manyToManyButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.manyToManyButton.Text = resources.GetString("manyToManyButton.Text");
			this.manyToManyButton.ToolTipText = resources.GetString("manyToManyButton.ToolTipText");
			this.manyToManyButton.Visible = ((bool)(resources.GetObject("manyToManyButton.Visible")));
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Enabled = ((bool)(resources.GetObject("toolBarButton1.Enabled")));
			this.toolBarButton1.ImageIndex = ((int)(resources.GetObject("toolBarButton1.ImageIndex")));
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.toolBarButton1.Text = resources.GetString("toolBarButton1.Text");
			this.toolBarButton1.ToolTipText = resources.GetString("toolBarButton1.ToolTipText");
			this.toolBarButton1.Visible = ((bool)(resources.GetObject("toolBarButton1.Visible")));
			// 
			// propertyButton
			// 
			this.propertyButton.Enabled = ((bool)(resources.GetObject("propertyButton.Enabled")));
			this.propertyButton.ImageIndex = ((int)(resources.GetObject("propertyButton.ImageIndex")));
			this.propertyButton.Text = resources.GetString("propertyButton.Text");
			this.propertyButton.ToolTipText = resources.GetString("propertyButton.ToolTipText");
			this.propertyButton.Visible = ((bool)(resources.GetObject("propertyButton.Visible")));
			// 
			// delButton
			// 
			this.delButton.Enabled = ((bool)(resources.GetObject("delButton.Enabled")));
			this.delButton.ImageIndex = ((int)(resources.GetObject("delButton.ImageIndex")));
			this.delButton.Text = resources.GetString("delButton.Text");
			this.delButton.ToolTipText = resources.GetString("delButton.ToolTipText");
			this.delButton.Visible = ((bool)(resources.GetObject("delButton.Visible")));
			// 
			// delAllButton
			// 
			this.delAllButton.Enabled = ((bool)(resources.GetObject("delAllButton.Enabled")));
			this.delAllButton.ImageIndex = ((int)(resources.GetObject("delAllButton.ImageIndex")));
			this.delAllButton.Text = resources.GetString("delAllButton.Text");
			this.delAllButton.ToolTipText = resources.GetString("delAllButton.ToolTipText");
			this.delAllButton.Visible = ((bool)(resources.GetObject("delAllButton.Visible")));
			// 
			// autoMappingButton
			// 
			this.autoMappingButton.AccessibleDescription = resources.GetString("autoMappingButton.AccessibleDescription");
			this.autoMappingButton.AccessibleName = resources.GetString("autoMappingButton.AccessibleName");
			this.autoMappingButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("autoMappingButton.Anchor")));
			this.autoMappingButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("autoMappingButton.BackgroundImage")));
			this.autoMappingButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("autoMappingButton.Dock")));
			this.autoMappingButton.Enabled = ((bool)(resources.GetObject("autoMappingButton.Enabled")));
			this.autoMappingButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("autoMappingButton.FlatStyle")));
			this.autoMappingButton.Font = ((System.Drawing.Font)(resources.GetObject("autoMappingButton.Font")));
			this.autoMappingButton.Image = ((System.Drawing.Image)(resources.GetObject("autoMappingButton.Image")));
			this.autoMappingButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("autoMappingButton.ImageAlign")));
			this.autoMappingButton.ImageIndex = ((int)(resources.GetObject("autoMappingButton.ImageIndex")));
			this.autoMappingButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("autoMappingButton.ImeMode")));
			this.autoMappingButton.Location = ((System.Drawing.Point)(resources.GetObject("autoMappingButton.Location")));
			this.autoMappingButton.Name = "autoMappingButton";
			this.autoMappingButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("autoMappingButton.RightToLeft")));
			this.autoMappingButton.Size = ((System.Drawing.Size)(resources.GetObject("autoMappingButton.Size")));
			this.autoMappingButton.TabIndex = ((int)(resources.GetObject("autoMappingButton.TabIndex")));
			this.autoMappingButton.Text = resources.GetString("autoMappingButton.Text");
			this.autoMappingButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("autoMappingButton.TextAlign")));
			this.autoMappingButton.Visible = ((bool)(resources.GetObject("autoMappingButton.Visible")));
			this.autoMappingButton.Click += new System.EventHandler(this.autoMappingButton_Click);
			// 
			// srcClassComboBox
			// 
			this.srcClassComboBox.AccessibleDescription = resources.GetString("srcClassComboBox.AccessibleDescription");
			this.srcClassComboBox.AccessibleName = resources.GetString("srcClassComboBox.AccessibleName");
			this.srcClassComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("srcClassComboBox.Anchor")));
			this.srcClassComboBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("srcClassComboBox.BackgroundImage")));
			this.srcClassComboBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("srcClassComboBox.Dock")));
			this.srcClassComboBox.Enabled = ((bool)(resources.GetObject("srcClassComboBox.Enabled")));
			this.srcClassComboBox.Font = ((System.Drawing.Font)(resources.GetObject("srcClassComboBox.Font")));
			this.srcClassComboBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("srcClassComboBox.ImeMode")));
			this.srcClassComboBox.IntegralHeight = ((bool)(resources.GetObject("srcClassComboBox.IntegralHeight")));
			this.srcClassComboBox.ItemHeight = ((int)(resources.GetObject("srcClassComboBox.ItemHeight")));
			this.srcClassComboBox.Location = ((System.Drawing.Point)(resources.GetObject("srcClassComboBox.Location")));
			this.srcClassComboBox.MaxDropDownItems = ((int)(resources.GetObject("srcClassComboBox.MaxDropDownItems")));
			this.srcClassComboBox.MaxLength = ((int)(resources.GetObject("srcClassComboBox.MaxLength")));
			this.srcClassComboBox.Name = "srcClassComboBox";
			this.srcClassComboBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("srcClassComboBox.RightToLeft")));
			this.srcClassComboBox.Size = ((System.Drawing.Size)(resources.GetObject("srcClassComboBox.Size")));
			this.srcClassComboBox.TabIndex = ((int)(resources.GetObject("srcClassComboBox.TabIndex")));
			this.srcClassComboBox.Text = resources.GetString("srcClassComboBox.Text");
			this.srcClassComboBox.Visible = ((bool)(resources.GetObject("srcClassComboBox.Visible")));
			this.srcClassComboBox.SelectedIndexChanged += new System.EventHandler(this.srcClassComboBox_SelectedIndexChanged);
			// 
			// dstClassTextBox
			// 
			this.dstClassTextBox.AccessibleDescription = resources.GetString("dstClassTextBox.AccessibleDescription");
			this.dstClassTextBox.AccessibleName = resources.GetString("dstClassTextBox.AccessibleName");
			this.dstClassTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("dstClassTextBox.Anchor")));
			this.dstClassTextBox.AutoSize = ((bool)(resources.GetObject("dstClassTextBox.AutoSize")));
			this.dstClassTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("dstClassTextBox.BackgroundImage")));
			this.dstClassTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("dstClassTextBox.Dock")));
			this.dstClassTextBox.Enabled = ((bool)(resources.GetObject("dstClassTextBox.Enabled")));
			this.dstClassTextBox.Font = ((System.Drawing.Font)(resources.GetObject("dstClassTextBox.Font")));
			this.dstClassTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("dstClassTextBox.ImeMode")));
			this.dstClassTextBox.Location = ((System.Drawing.Point)(resources.GetObject("dstClassTextBox.Location")));
			this.dstClassTextBox.MaxLength = ((int)(resources.GetObject("dstClassTextBox.MaxLength")));
			this.dstClassTextBox.Multiline = ((bool)(resources.GetObject("dstClassTextBox.Multiline")));
			this.dstClassTextBox.Name = "dstClassTextBox";
			this.dstClassTextBox.PasswordChar = ((char)(resources.GetObject("dstClassTextBox.PasswordChar")));
			this.dstClassTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("dstClassTextBox.RightToLeft")));
			this.dstClassTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("dstClassTextBox.ScrollBars")));
			this.dstClassTextBox.Size = ((System.Drawing.Size)(resources.GetObject("dstClassTextBox.Size")));
			this.dstClassTextBox.TabIndex = ((int)(resources.GetObject("dstClassTextBox.TabIndex")));
			this.dstClassTextBox.Text = resources.GetString("dstClassTextBox.Text");
			this.dstClassTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("dstClassTextBox.TextAlign")));
			this.dstClassTextBox.Visible = ((bool)(resources.GetObject("dstClassTextBox.Visible")));
			this.dstClassTextBox.WordWrap = ((bool)(resources.GetObject("dstClassTextBox.WordWrap")));
			// 
			// dstClassButton
			// 
			this.dstClassButton.AccessibleDescription = resources.GetString("dstClassButton.AccessibleDescription");
			this.dstClassButton.AccessibleName = resources.GetString("dstClassButton.AccessibleName");
			this.dstClassButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("dstClassButton.Anchor")));
			this.dstClassButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("dstClassButton.BackgroundImage")));
			this.dstClassButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("dstClassButton.Dock")));
			this.dstClassButton.Enabled = ((bool)(resources.GetObject("dstClassButton.Enabled")));
			this.dstClassButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("dstClassButton.FlatStyle")));
			this.dstClassButton.Font = ((System.Drawing.Font)(resources.GetObject("dstClassButton.Font")));
			this.dstClassButton.Image = ((System.Drawing.Image)(resources.GetObject("dstClassButton.Image")));
			this.dstClassButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("dstClassButton.ImageAlign")));
			this.dstClassButton.ImageIndex = ((int)(resources.GetObject("dstClassButton.ImageIndex")));
			this.dstClassButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("dstClassButton.ImeMode")));
			this.dstClassButton.Location = ((System.Drawing.Point)(resources.GetObject("dstClassButton.Location")));
			this.dstClassButton.Name = "dstClassButton";
			this.dstClassButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("dstClassButton.RightToLeft")));
			this.dstClassButton.Size = ((System.Drawing.Size)(resources.GetObject("dstClassButton.Size")));
			this.dstClassButton.TabIndex = ((int)(resources.GetObject("dstClassButton.TabIndex")));
			this.dstClassButton.Text = resources.GetString("dstClassButton.Text");
			this.dstClassButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("dstClassButton.TextAlign")));
			this.dstClassButton.Visible = ((bool)(resources.GetObject("dstClassButton.Visible")));
			this.dstClassButton.Click += new System.EventHandler(this.dstClassButton_Click);
			// 
			// SpecifySelectorDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.dstClassButton);
			this.Controls.Add(this.dstClassTextBox);
			this.Controls.Add(this.srcClassComboBox);
			this.Controls.Add(this.autoMappingButton);
			this.Controls.Add(this.toolBar);
			this.Controls.Add(this.nextRowButton);
			this.Controls.Add(this.prevRowButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "SpecifySelectorDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.SpecifySelectorDialog_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SpecifySelectorDialog_MouseUp);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Controller Code

		/// <summary>
		/// Set the enabling status of Prev and Next buttons
		/// </summary>
		private void SetPrevNextButtonStatus()
		{
			if (_srcInstanceView.SelectedIndex == 0)
			{
				this.prevRowButton.Enabled = false;
			}
			else
			{
				this.prevRowButton.Enabled = true;
			}

			int rows = DataSetHelper.GetRowCount(this._srcInstanceView.DataSet, _currentClassMapping.SourceClassName);
			if (_srcInstanceView.SelectedIndex < rows - 1)
			{
				this.nextRowButton.Enabled = true;
			}
			else
			{
				this.nextRowButton.Enabled = false;
			}
		}

		/// <summary>
		/// Set the default values specified for the destination attributes
		/// </summary>
		private void SetDefaultValues(InstanceView instanceView)
		{
			if (_currentClassMapping != null)
			{
				foreach (DefaultValue defaultValue in _currentClassMapping.DefaultValues)
				{
					// keep a local copy of default values
					_defaultValues.Add(defaultValue.Copy());
					instanceView.InstanceData.SetAttributeStringValue(defaultValue.DestinationAttributeName,
						defaultValue.Value);
				}

				_defaultValues.IsAltered = false; // reset the flag
			}
		}

		/// <summary>
		/// set tool bar toggle button status so that only one of toggle buttons
		/// is pushed in
		/// </summary>
		/// <param name="e">The event</param>
		private void SetToolBarToggleButtonStatus(System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			MapComponentType type = MapComponentType.Pointer;

			if (e.Button == this.arrowButton)
			{
				type = MapComponentType.Pointer;
			}
			else if (e.Button == this.oneToOneButton)
			{
				type = MapComponentType.OneToOne;
			}
			else if (e.Button == this.oneToManyButton)
			{
				type = MapComponentType.OneToMany;
			}
			else if (e.Button == this.manyToOneButton)
			{
				type = MapComponentType.ManyToOne;
			}
			else if (e.Button == this.manyToManyButton)
			{
				type = MapComponentType.ManyToMany;
			}
			
			SetToolBarToggleButtonStatus(type);
		}

		private void SetToolBarToggleButtonStatus(MapComponentType type)
		{
			if (type == MapComponentType.Pointer)
			{
				this.arrowButton.Pushed = true;
			}
			else
			{
				this.arrowButton.Pushed = false;
			}

			if (type == MapComponentType.OneToOne)
			{
				this.oneToOneButton.Pushed = true;
			}
			else
			{
				this.oneToOneButton.Pushed = false;
			}

			if (type == MapComponentType.OneToMany)
			{
				this.oneToManyButton.Pushed = true;
				this._mapPanel.Action = new InsertRectAction(_mapPanel, type);
			}
			else
			{
				this.oneToManyButton.Pushed = false;
			}

			if (type == MapComponentType.ManyToOne)
			{
				this.manyToOneButton.Pushed = true;
				this._mapPanel.Action = new InsertRectAction(_mapPanel, type);
			}
			else
			{
				this.manyToOneButton.Pushed = false;
			}

			if (type == MapComponentType.ManyToMany)
			{
				this.manyToManyButton.Pushed = true;
				this._mapPanel.Action = new InsertRectAction(_mapPanel, type);
			}
			else
			{
				this.manyToManyButton.Pushed = false;
			}
		}

		/// <summary>
		/// Perform actions according to the selected component
		/// </summary>
		/// <param name="component">The selected component</param>
		private void PerformActions(IMapComponent component, MapComponentEventArgs e)
		{
			switch (component.ComponentType)
			{
				case MapComponentType.SourceEnd:
				case MapComponentType.OutputEnd:

					if (component.IsValid)
					{
						_mapPanel.Cursor = Cursors.Cross;

						_connectionAction = new ConnectLineAction(_mapPanel);
						
						_connectionAction.SrcComponent = component;
		
						// set the toolbar toggle button status
						SetToolBarToggleButtonStatus(MapComponentType.OneToOne);
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.InvalidConnection"));
					}

					break;

				case MapComponentType.InputEnd:
				case MapComponentType.DestinationEnd:

					_mapPanel.Cursor = Cursors.Default;

					if (_connectionAction != null &&
						_connectionAction.ActionType == MapActionType.Connect)
					{
						_connectionAction.DstComponent = component;

						// make sure that two components can be connected
						if (!_connectionAction.IsValid)
						{
							MessageBox.Show(_connectionAction.ErrorMessage);
						}
						else
						{
							_mapPanel.Action = _connectionAction;
						}

						// connect action will be performed when mouse up event fired
					}
		
					// set the toolbar toggle button status
					SetToolBarToggleButtonStatus(MapComponentType.Pointer);

					break;

				case MapComponentType.Composite:

					if (component.Principal.ComponentType == MapComponentType.ManyToOne ||
						component.Principal.ComponentType == MapComponentType.OneToMany ||
						component.Principal.ComponentType == MapComponentType.ManyToMany)
					{
						_mapPanel.Cursor = Cursors.SizeAll;

						MoveRectAction moveAction = new MoveRectAction(_mapPanel,
							(MapRect) component.Principal);

						_mapPanel.Action = moveAction;
					}

					break;

				default:
					_mapPanel.Cursor = Cursors.Default;

					// set the toolbar toggle button status
					SetToolBarToggleButtonStatus(MapComponentType.Pointer);
					break;
			}
		}

		private void ShowMappingProperty(IMapComponent component)
		{
			if (component is MapComposite &&
				((MapComposite) component).Principal.MappingNode != null &&
				((MapComposite) component).Principal.MappingNode is ITransformable)
			{
				ITransformable pricipalComponent = (ITransformable) ((MapComposite) component).Principal.MappingNode;
				MappingScriptDialog dialog = new MappingScriptDialog();
				dialog.SchemaInfo = _metaData.SchemaInfo;
				dialog.Mapping = pricipalComponent;
				dialog.ScriptEnabled = pricipalComponent.ScriptEnabled;
				dialog.ScriptLanguage = pricipalComponent.ScriptLanguage;
				dialog.ClassType = pricipalComponent.ClassType;
				dialog.Script = pricipalComponent.Script;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					pricipalComponent.ScriptEnabled = dialog.ScriptEnabled;
					pricipalComponent.ScriptLanguage = dialog.ScriptLanguage;
					pricipalComponent.ClassType = dialog.ClassType;
					pricipalComponent.Script = dialog.Script;
				}
			}
		}

		/// <summary>
		/// Show the given source class in the left property grid
		/// </summary>
		/// <param name="srcClassName">The name of source class.</param>
		private void ShowSourceProperties(string srcClassName)
		{
			if (_metaData != null && srcClassName != null)
			{
				DataViewModel instanceDataView = _metaData.GetDetailedDataView(srcClassName);

				// get instance data in source class to display in the source property grid
				string query = instanceDataView.SearchQuery;

				int rows;
				DataSet ds = null;
				try
				{
					// invoke the web service synchronously
					XmlNode xmlNode = _dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
						query);

					ds = new DataSet();
					XmlReader xmlReader = new XmlNodeReader(xmlNode);
					ds.ReadXml(xmlReader);

					rows = DataSetHelper.GetRowCount(ds, srcClassName);
				}
				catch (Exception)
				{
					rows = 0; // studio may be offline
				}

				// display the source class in the property grid
				if (rows > 0)
				{
					_srcInstanceView = new InstanceView(instanceDataView, ds);
				}
				else
				{
					_srcInstanceView = new InstanceView(instanceDataView);
				}

				sourcePropertyGrid.SelectedObject = _srcInstanceView;
                sourcePropertyGrid.CalculateGridItemPositions();

				// set status of next button
				if (rows > 1)
				{
					this.nextRowButton.Enabled = true;
				}
				else
				{
					this.nextRowButton.Enabled = false;
				}
				this.prevRowButton.Enabled = false;
			}
		}

		/// <summary>
		/// Show the given destination class in the right property grid
		/// </summary>
		/// <param name="dstClassName">The name of destination class.</param>
		private void ShowDestinationProperties(string dstClassName)
		{
			if (_metaData != null && dstClassName != null)
			{
				DataViewModel instanceDataView = _metaData.GetDetailedDataView(dstClassName);

				// display the destination class in the property grid
				_dstInstanceView = new InstanceView(instanceDataView);

				// set the default values to corresponding destination attributes
				SetDefaultValues(_dstInstanceView);

				// create an empty row for each array attribute so that property
				// grid can create GridItem for array sub-element
				InitArrayAttributes(_dstInstanceView);

				destinationPropertyGrid.SelectedObject = _dstInstanceView;
                destinationPropertyGrid.CalculateGridItemPositions();

				this.okButton.Enabled = true;
			}
		}

		/// <summary>
		/// Show a list of source classes in the srcClassComboBox and display one
		/// of the source class in the left property grid.
		/// </summary>
		private void ShowSourceClasses()
		{
			this.srcClassComboBox.Text = "";
			this.srcClassComboBox.Items.Clear();

			foreach (ClassMapping classMapping in _selector.ClassMappings)
			{
				ClassElement srcClassElement = _metaData.SchemaModel.FindClass(classMapping.SourceClassName);

				if (srcClassElement != null)
				{
					this.srcClassComboBox.Items.Add(srcClassElement.Caption);
				}
			}

			// the last item in the combo box is a command to launch ChooseClassDialog
			// to add a new source class.
			this.srcClassComboBox.Items.Add(MessageResourceManager.GetString("SchemaEditor.AddClass"));

			// make the first class mapping as the current class mapping if any
			if (_selector.ClassMappings.Count > 0)
			{
				this.srcClassComboBox.SelectedIndex = 0;
			}
		}

		/// <summary>
		/// Create a new ClassMapping in the selector for the provided source
		/// class, add an item in srcClassComboBox, display the source class in the
		/// left property grid, and display the existing mappings in map panel.
		/// </summary>
		/// <param name="srcClassElement">The ClassElement instance.</param>
		private void CreateClassMapping(ClassElement srcClassElement)
		{
			// insert the source class caption into the combo box
			int insertPos = _selector.ClassMappings.Count;
			this.srcClassComboBox.Items.Insert(insertPos, srcClassElement.Caption);

			_selector.AddClassMapping(srcClassElement.Name, _selector.DestinationClassName);

			// select the newly added class
			this.srcClassComboBox.SelectedIndex = insertPos;
		}

		/// <summary>
		/// Save changes made in the map panel, including attribute mappings and
		/// default values.
		/// </summary>
		private void SaveChanges()
		{
			// save the changes to the attribute mappings
			if (AttributeMappings.IsAltered)
			{
				_currentClassMapping.AttributeMappings = this.AttributeMappings;
			}

			// save the changes to the default values for destination class
			if (DefaultValues.IsAltered)
			{
				_currentClassMapping.DefaultValues = this.DefaultValues;
			}

			// we have got the mappings and default values,
			// initialize the collections that maintains mapping infos
			// so that any future changes to the collections won't affect
			// those contained by the selector
			_mapPanel.InitAttributeCollection();
			_defaultValues = new DefaultValueCollection();
		}

		/// <summary>
		/// Create an empty row for each array attribute so that the PropertyGrid
		/// can display subitems of array attributes.
		/// </summary>
		/// <param name="instanceView">The InstanceView</param>
		private void InitArrayAttributes(InstanceView instanceView)
		{
			foreach (IDataViewElement element in instanceView.DataView.ResultAttributes)
			{
				if (element is DataArrayAttribute)
				{
					DataArrayAttribute arrayAttr = (DataArrayAttribute) element;
					// create an empty row if it is an empty array
					if (!arrayAttr.HasValue)
					{
						arrayAttr.AddEmptyRow();
					}
				}
			}
		}

		/// <summary>
		/// Adjust heights of the map panel so that the scroll bars of both
		/// PropertyGrid are not visible.
		/// </summary>
		private void AdjustPanelSize()
		{
			// Set the AutoScrollMinSize to the max of heights of sourcePropertyGrid
			// and destinationPropertyGrid, so that the scroll bars of both
			// PropertyGrid are not visible.
			int srcHeight = this.sourcePropertyGrid.GetActualHeight();
			int desHeight = this.destinationPropertyGrid.GetActualHeight();
			int panelMinHeight = (srcHeight > desHeight ? srcHeight : desHeight);
			Size size = new Size(this.panel1.AutoScrollMinSize.Width, panelMinHeight);
			this.panel1.AutoScrollMinSize = size;

			this.panel1.Refresh();
		}

		/// <summary>
		/// Clear the selector to have a fresh start.
		/// </summary>
		private void ClearSelector()
		{
			_selector.Clear();
			ShowSourceClasses(); // empty the srcClasses in combox
			this.sourcePropertyGrid.SelectedObject = null;
			this.destinationPropertyGrid.SelectedObject = null;
			_mapPanel.ClearMappings();
			_currentClassMapping = null;
		}

		#endregion

		private void SpecifySelectorDialog_Load(object sender, System.EventArgs e)
		{
			if (_selector.DestinationClassName != null)
			{
				ClassElement dstClassElement = _metaData.SchemaModel.FindClass(_selector.DestinationClassName);
				this.dstClassTextBox.Text = dstClassElement.Caption;

				// show the destination class on the right property grid
				ShowDestinationProperties(_selector.DestinationClassName);
			}

			// display source classes on the left property grid
			ShowSourceClasses();
		}

		private void mapPanel_SelectedComponentChanged(object sender, Newtera.Studio.ImportExport.MapComponentEventArgs e)
		{
			if (e.SelectedComponent != null)
			{
				this.delButton.Enabled = true;
				this.deleteMenuItem.Enabled = true;
				if (e.SelectedComponent is MapComposite &&
					((MapComposite) e.SelectedComponent).Principal.MappingNode != null)
				{
					this.propertyButton.Enabled = true;
					this.propertyMenuItem.Enabled = true;
				}
				else
				{
					this.propertyButton.Enabled = false;
					this.propertyMenuItem.Enabled = false;
				}

				PerformActions(e.SelectedComponent, e);
			}
			else
			{
				// nothing selected
				this.delButton.Enabled = false;
				this.deleteMenuItem.Enabled = false;
				this.propertyButton.Enabled = false;
				this.propertyMenuItem.Enabled = false;

				this._connectionAction = null;

				// Restore the Cursor and discard the src item selection
				_mapPanel.Cursor = Cursors.Default;
		
				// set the toolbar toggle button status
				SetToolBarToggleButtonStatus(MapComponentType.Pointer);
			}
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.DeleteComponent(_mapPanel.SelectedComponent);
		}

		private void deleteAllButton_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.ClearMappings();
		}

		private void destinationPropertyGrid_GridItemCollapsed(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
			this._mapPanel.Redraw();
		}

		private void destinationPropertyGrid_GridItemExpanded(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
			this._mapPanel.Redraw();		
		}

		private void sourcePropertyGrid_GridItemCollapsed(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
			this._mapPanel.Redraw();	
		}

		private void sourcePropertyGrid_GridItemExpanded(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
			this._mapPanel.Redraw();		
		}

		private void SpecifySelectorDialog_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Cursor.Current = Cursors.Default;	
		}

		private void prevRowButton_Click(object sender, System.EventArgs e)
		{
			_srcInstanceView.SelectedIndex = _srcInstanceView.SelectedIndex - 1;
			this.sourcePropertyGrid.Refresh();

			SetPrevNextButtonStatus();
		}

		private void nextRowButton_Click(object sender, System.EventArgs e)
		{
			_srcInstanceView.SelectedIndex = _srcInstanceView.SelectedIndex + 1;
			this.sourcePropertyGrid.Refresh();
		
			SetPrevNextButtonStatus();
		}

		private void destinationPropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.GridItemType == GridItemType.Property)
			{
				string attributeName = e.ChangedItem.PropertyDescriptor.Name;
				string attributeValue = _dstInstanceView.InstanceData.GetAttributeStringValue(attributeName);
				
				InstanceAttributePropertyDescriptor ipd = e.ChangedItem.PropertyDescriptor as InstanceAttributePropertyDescriptor;

				if (ipd != null && ipd.IsRelationship)
				{
					DataRelationshipAttribute relationshipAttr = (DataRelationshipAttribute) ipd.DataViewElement;

					// the value of a relationship attribite consists of values of its
					// primary keys separated by &. Splits the pk values and set them
					// to each primary keys
					string[] pkValues = relationshipAttr.SplitPKValues(attributeValue);

					if (relationshipAttr.PrimaryKeys != null)
					{
						int index = 0;
						foreach (DataSimpleAttribute pk in relationshipAttr.PrimaryKeys)
						{
							if (pkValues != null && pkValues.Length > index)
							{
								SetDefaultValue(relationshipAttr.GetUniquePKName(pk.Name), pkValues[index]);
							}
							else
							{
								// remove the default value for the primary key
								SetDefaultValue(relationshipAttr.GetUniquePKName(pk.Name), null);
							}

							index ++;
						}
					}
				}
				else
				{
					SetDefaultValue(attributeName, attributeValue);
				}

				if (ipd != null && ipd.IsArray)
				{
					// the number of grid items in destination property grid that
					// represent array cells might have been changed, we need to
					// redraw the dots in the map panel that represents the
					// grid items
					this._mapPanel.Redraw();
				}
			}
		}

		/// <summary>
		/// Create, modify or delete a DefaultValue instance in the ClassMapping.
		/// </summary>
		/// <param name="attributeName">The attribute name.</param>
		/// <param name="value">The default value.</param>
		private void SetDefaultValue(string attributeName, string attributeValue)
		{
			DefaultValue defaultValue = null;
			foreach (DefaultValue dv in this._defaultValues)
			{
				if (dv.DestinationAttributeName == attributeName)
				{
					defaultValue = dv;
					break;
				}
			}

			if (defaultValue == null)
			{
				// create a Default Value instance
				defaultValue = new DefaultValue(attributeName, attributeValue);
				_defaultValues.Add(defaultValue);
			}
			else
			{
				if (attributeValue != null && attributeValue.Length > 0)
				{
					// change the default value
					defaultValue.Value = attributeValue;
				}
				else
				{
					// remove the default value
					_defaultValues.Remove(defaultValue);
				}
			}
		}

		private void deleteMenuItem_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.DeleteComponent(_mapPanel.SelectedComponent);		
		}

		private void propertyMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowMappingProperty(_mapPanel.SelectedComponent);
		}

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.delButton)
			{
				this._mapPanel.DeleteComponent(_mapPanel.SelectedComponent);
			}
			else if (e.Button == this.delAllButton)
			{
				this._mapPanel.ClearMappings();
			}
			else if (e.Button == this.propertyButton)
			{
				ShowMappingProperty(_mapPanel.SelectedComponent);
			}
			else
			{
				SetToolBarToggleButtonStatus(e); // set the tool bar toggle button status
			}
		}

		private void deleteAllMenuItem_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.ClearMappings();
		}

		private void autoMappingButton_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.AutoMappings();
		}

		private void dstClassButton_Click(object sender, System.EventArgs e)
		{
			ChooseClassDialog dialog = new ChooseClassDialog();
			dialog.RootClass = "ALL";
			dialog.MetaData = _metaData;
			if (_selector.DestinationClassName != null)
			{
				dialog.SelectedClass = (ClassElement) _metaData.SchemaModel.FindClass(_selector.DestinationClassName);
			}

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				ClassElement dstClassElement = dialog.SelectedClass;
				
				if (dstClassElement.IsLeaf)
				{
					if (dstClassElement.Name != _selector.DestinationClassName)
					{
						ClearSelector(); // destination changed, clear existing selector

						_selector.DestinationClassName = dstClassElement.Name;

						this.dstClassTextBox.Text = dstClassElement.Caption;

						// show the destination class on the right panel
						ShowDestinationProperties(_selector.DestinationClassName);

						AdjustPanelSize(); // adjust panel size
					}
				}
				else
				{
					MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.LeafClassRequired"));
				}
			}
		}

		private void srcClassComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.srcClassComboBox.SelectedIndex >= 0)
			{
				if (this.srcClassComboBox.SelectedIndex < _selector.ClassMappings.Count)
				{
					// show the selected class
					ClassMapping selected = (ClassMapping) _selector.ClassMappings[this.srcClassComboBox.SelectedIndex];
					if (_currentClassMapping != selected)
					{
						if (_currentClassMapping != null)
						{
							SaveChanges(); // save the changes
						}

						_currentClassMapping = selected;

						ShowSourceProperties(_currentClassMapping.SourceClassName);

						_mapPanel.ClassMapping = _currentClassMapping;

						AdjustPanelSize(); // adjust panel size
					}
				}
				else
				{
					// "Add class" item is selected, launch a ChooseClassDialog
					if (_selector.DestinationClassName != null)
					{
						ChooseClassDialog dialog = new ChooseClassDialog();
						dialog.RootClass = "ALL";
						dialog.MetaData = _metaData;

						if (dialog.ShowDialog() == DialogResult.OK)
						{
							ClassElement srcClassElement = dialog.SelectedClass;
					
							if (srcClassElement.IsLeaf)
							{
								CreateClassMapping(srcClassElement);
							}
							else
							{
								MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.LeafClassRequired"));
							}
						}
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.UnknowDestinationClass"));
					}
				}
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (_currentClassMapping != null)
			{
				SaveChanges();
			}
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Mappings;
using Newtera.Studio.UserControls;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for CreateXMLSchemaViewDialog.
	/// </summary>
	public class CreateXMLSchemaViewDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private XMLSchemaModel _xmlSchemaModel;
        private MetaDataTreeNode _schemaRootNode = null;
        private MetaDataTreeNode _currentSchemaTreeNode = null;
        private XMLSchemaComplexType _currentComplexType = null;
        private XMLSchemaElement _currentSelectedAttribute = null;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.Button nextStepButton;
		private System.Windows.Forms.Button prevStepButton;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ErrorProvider infoProvider;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ImageList smallIconImageList;
        private TabControl schemaViewTabControl;
        private TabPage basicInfoTabPage;
        private Button baseClassButton;
        private Label label11;
        private TextBox schemaViewDescriptionTextBox;
        private TextBox baseClassTextBox;
        private TextBox schemaViewCaptionTextBox;
        private TextBox schemaViewNameTextBox;
        private Label label6;
        private Label label5;
        private Label label4;
        private TabPage classesTabPage;
        private GroupBox classesGroupBox;
        private Label msgLabel;
        private TabPage attributesTabPage;
        private TreeView schemaTreeView;
        private Button removeClassButton;
        private Button addButton;
        private ListView linkedClassesListView;
        private ColumnHeader linkedClassCaptionColumnHeader;
        private ColumnHeader relationshipCaptionColumnHeader;
        private ColumnHeader relationshipTypeColumnHeader;
        private Label label2;
        private Button showLeafClassButton;
        private Label label1;
        private ImageList treeImageList;
        private Label label3;
        private TreeView schemaTreeView1;
        private ListView resultsListView;
        private ColumnHeader resultCaptionColumnHeader;
        private ColumnHeader resultNameColumnHeader;
        private Button addResultsButton;
        private Button refreshButton;
        private Button removeResultButton;
        private TabPage ImportScripts;
        private TreeView schemaTreeView2;
        private Label label8;
        private ListView importScriptListView;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Button removeScriptButton;
        private Button addScriptButton;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private RadioButton dbOnlyRadioButton;
        private RadioButton dbAndFilesRadioButton;
        private RadioButton fileOnlyRadioButton;
        private GroupBox groupBox3;
        private CheckBox mergingCheckBox;
        private TextBox matchAttributeTextBox;
        private Label label9;
        private GroupBox groupBox4;
        private TextBox conditionTextBox;
        private Label label10;
        private GroupBox groupBox5;
        private TextBox validateConditionTextBox;
        private Label label12;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label14;
        private Label label13;
        private Label label7;
        private ComboBox sortDirectionComboBox;
        private TextBox sortOrderTextBox;
        private ComboBox sortEnabledComboBox;
        private Label attributeNameLabel;
        private Label label15;
        private CheckBox multipleOccurCheckBox;
        private Button renameButton;
        private Button renameResult;
        private GroupBox groupBox7;
        private GroupBox groupBox6;
        private CheckBox xAxisCheckBox;
        private GroupBox groupBox8;
        private RadioButton labelRadioButton;
        private RadioButton featureRadioButton;
        private CheckBox categoryAxisCheckBox;
        private Label label16;
        private TextBox postProcessorTextBox;
        private System.ComponentModel.IContainer components;

		public CreateXMLSchemaViewDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
		/// Gets or sets the meta data model from which to get all
		/// available information needed to specify a relationship
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
		/// Gets or sets the xml schema model
		/// is being chosen.
		/// </summary>
		public XMLSchemaModel XMLSchemaModel
		{
			get
			{
				return _xmlSchemaModel;
			}
			set
			{
				if (value != null)
				{
					_xmlSchemaModel = value;
				}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateXMLSchemaViewDialog));
            this.nextStepButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.smallIconImageList = new System.Windows.Forms.ImageList(this.components);
            this.doneButton = new System.Windows.Forms.Button();
            this.prevStepButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.validateConditionTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.classesGroupBox = new System.Windows.Forms.GroupBox();
            this.renameButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.showLeafClassButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.linkedClassesListView = new System.Windows.Forms.ListView();
            this.linkedClassCaptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.relationshipCaptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.relationshipTypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.addButton = new System.Windows.Forms.Button();
            this.removeClassButton = new System.Windows.Forms.Button();
            this.schemaTreeView = new System.Windows.Forms.TreeView();
            this.msgLabel = new System.Windows.Forms.Label();
            this.baseClassTextBox = new System.Windows.Forms.TextBox();
            this.schemaViewNameTextBox = new System.Windows.Forms.TextBox();
            this.schemaViewCaptionTextBox = new System.Windows.Forms.TextBox();
            this.schemaViewTabControl = new System.Windows.Forms.TabControl();
            this.basicInfoTabPage = new System.Windows.Forms.TabPage();
            this.postProcessorTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.multipleOccurCheckBox = new System.Windows.Forms.CheckBox();
            this.baseClassButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.schemaViewDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.classesTabPage = new System.Windows.Forms.TabPage();
            this.attributesTabPage = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.renameResult = new System.Windows.Forms.Button();
            this.resultsListView = new System.Windows.Forms.ListView();
            this.resultCaptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.resultNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.refreshButton = new System.Windows.Forms.Button();
            this.addResultsButton = new System.Windows.Forms.Button();
            this.removeResultButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.labelRadioButton = new System.Windows.Forms.RadioButton();
            this.featureRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.categoryAxisCheckBox = new System.Windows.Forms.CheckBox();
            this.xAxisCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.sortEnabledComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.sortDirectionComboBox = new System.Windows.Forms.ComboBox();
            this.sortOrderTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.attributeNameLabel = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.schemaTreeView1 = new System.Windows.Forms.TreeView();
            this.ImportScripts = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.conditionTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.matchAttributeTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.mergingCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dbAndFilesRadioButton = new System.Windows.Forms.RadioButton();
            this.fileOnlyRadioButton = new System.Windows.Forms.RadioButton();
            this.dbOnlyRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.importScriptListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addScriptButton = new System.Windows.Forms.Button();
            this.removeScriptButton = new System.Windows.Forms.Button();
            this.schemaTreeView2 = new System.Windows.Forms.TreeView();
            this.label8 = new System.Windows.Forms.Label();
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.classesGroupBox.SuspendLayout();
            this.schemaViewTabControl.SuspendLayout();
            this.basicInfoTabPage.SuspendLayout();
            this.classesTabPage.SuspendLayout();
            this.attributesTabPage.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.ImportScripts.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // nextStepButton
            // 
            resources.ApplyResources(this.nextStepButton, "nextStepButton");
            this.infoProvider.SetError(this.nextStepButton, resources.GetString("nextStepButton.Error"));
            this.errorProvider.SetError(this.nextStepButton, resources.GetString("nextStepButton.Error1"));
            this.infoProvider.SetIconAlignment(this.nextStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("nextStepButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.nextStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("nextStepButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.nextStepButton, ((int)(resources.GetObject("nextStepButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.nextStepButton, ((int)(resources.GetObject("nextStepButton.IconPadding1"))));
            this.nextStepButton.Name = "nextStepButton";
            this.toolTip.SetToolTip(this.nextStepButton, resources.GetString("nextStepButton.ToolTip"));
            this.nextStepButton.Click += new System.EventHandler(this.nextStepButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.infoProvider.SetError(this.cancelButton, resources.GetString("cancelButton.Error"));
            this.errorProvider.SetError(this.cancelButton, resources.GetString("cancelButton.Error1"));
            this.infoProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.cancelButton, ((int)(resources.GetObject("cancelButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.cancelButton, ((int)(resources.GetObject("cancelButton.IconPadding1"))));
            this.cancelButton.Name = "cancelButton";
            this.toolTip.SetToolTip(this.cancelButton, resources.GetString("cancelButton.ToolTip"));
            // 
            // smallIconImageList
            // 
            this.smallIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallIconImageList.ImageStream")));
            this.smallIconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.smallIconImageList.Images.SetKeyName(0, "");
            this.smallIconImageList.Images.SetKeyName(1, "");
            this.smallIconImageList.Images.SetKeyName(2, "");
            this.smallIconImageList.Images.SetKeyName(3, "");
            // 
            // doneButton
            // 
            resources.ApplyResources(this.doneButton, "doneButton");
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.infoProvider.SetError(this.doneButton, resources.GetString("doneButton.Error"));
            this.errorProvider.SetError(this.doneButton, resources.GetString("doneButton.Error1"));
            this.infoProvider.SetIconAlignment(this.doneButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("doneButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.doneButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("doneButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.doneButton, ((int)(resources.GetObject("doneButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.doneButton, ((int)(resources.GetObject("doneButton.IconPadding1"))));
            this.doneButton.Name = "doneButton";
            this.toolTip.SetToolTip(this.doneButton, resources.GetString("doneButton.ToolTip"));
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // prevStepButton
            // 
            resources.ApplyResources(this.prevStepButton, "prevStepButton");
            this.infoProvider.SetError(this.prevStepButton, resources.GetString("prevStepButton.Error"));
            this.errorProvider.SetError(this.prevStepButton, resources.GetString("prevStepButton.Error1"));
            this.infoProvider.SetIconAlignment(this.prevStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("prevStepButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.prevStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("prevStepButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.prevStepButton, ((int)(resources.GetObject("prevStepButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.prevStepButton, ((int)(resources.GetObject("prevStepButton.IconPadding1"))));
            this.prevStepButton.Name = "prevStepButton";
            this.toolTip.SetToolTip(this.prevStepButton, resources.GetString("prevStepButton.ToolTip"));
            this.prevStepButton.Click += new System.EventHandler(this.prevStepButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.validateConditionTextBox);
            this.groupBox5.Controls.Add(this.label12);
            this.errorProvider.SetError(this.groupBox5, resources.GetString("groupBox5.Error"));
            this.infoProvider.SetError(this.groupBox5, resources.GetString("groupBox5.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox5.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox5.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox5, ((int)(resources.GetObject("groupBox5.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox5, ((int)(resources.GetObject("groupBox5.IconPadding1"))));
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox5, resources.GetString("groupBox5.ToolTip"));
            // 
            // validateConditionTextBox
            // 
            resources.ApplyResources(this.validateConditionTextBox, "validateConditionTextBox");
            this.errorProvider.SetError(this.validateConditionTextBox, resources.GetString("validateConditionTextBox.Error"));
            this.infoProvider.SetError(this.validateConditionTextBox, resources.GetString("validateConditionTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.validateConditionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("validateConditionTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.validateConditionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("validateConditionTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.validateConditionTextBox, ((int)(resources.GetObject("validateConditionTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.validateConditionTextBox, ((int)(resources.GetObject("validateConditionTextBox.IconPadding1"))));
            this.validateConditionTextBox.Name = "validateConditionTextBox";
            this.toolTip.SetToolTip(this.validateConditionTextBox, resources.GetString("validateConditionTextBox.ToolTip"));
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.infoProvider.SetError(this.label12, resources.GetString("label12.Error"));
            this.errorProvider.SetError(this.label12, resources.GetString("label12.Error1"));
            this.infoProvider.SetIconAlignment(this.label12, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label12.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label12, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label12.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label12, ((int)(resources.GetObject("label12.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label12, ((int)(resources.GetObject("label12.IconPadding1"))));
            this.label12.Name = "label12";
            this.toolTip.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // classesGroupBox
            // 
            resources.ApplyResources(this.classesGroupBox, "classesGroupBox");
            this.classesGroupBox.Controls.Add(this.renameButton);
            this.classesGroupBox.Controls.Add(this.label1);
            this.classesGroupBox.Controls.Add(this.showLeafClassButton);
            this.classesGroupBox.Controls.Add(this.label2);
            this.classesGroupBox.Controls.Add(this.linkedClassesListView);
            this.classesGroupBox.Controls.Add(this.addButton);
            this.classesGroupBox.Controls.Add(this.removeClassButton);
            this.classesGroupBox.Controls.Add(this.schemaTreeView);
            this.classesGroupBox.Controls.Add(this.msgLabel);
            this.errorProvider.SetError(this.classesGroupBox, resources.GetString("classesGroupBox.Error"));
            this.infoProvider.SetError(this.classesGroupBox, resources.GetString("classesGroupBox.Error1"));
            this.errorProvider.SetIconAlignment(this.classesGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("classesGroupBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.classesGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("classesGroupBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.classesGroupBox, ((int)(resources.GetObject("classesGroupBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.classesGroupBox, ((int)(resources.GetObject("classesGroupBox.IconPadding1"))));
            this.classesGroupBox.Name = "classesGroupBox";
            this.classesGroupBox.TabStop = false;
            this.toolTip.SetToolTip(this.classesGroupBox, resources.GetString("classesGroupBox.ToolTip"));
            // 
            // renameButton
            // 
            resources.ApplyResources(this.renameButton, "renameButton");
            this.infoProvider.SetError(this.renameButton, resources.GetString("renameButton.Error"));
            this.errorProvider.SetError(this.renameButton, resources.GetString("renameButton.Error1"));
            this.infoProvider.SetIconAlignment(this.renameButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("renameButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.renameButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("renameButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.renameButton, ((int)(resources.GetObject("renameButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.renameButton, ((int)(resources.GetObject("renameButton.IconPadding1"))));
            this.renameButton.Name = "renameButton";
            this.toolTip.SetToolTip(this.renameButton, resources.GetString("renameButton.ToolTip"));
            this.renameButton.Click += new System.EventHandler(this.renameButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.infoProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.errorProvider.SetError(this.label1, resources.GetString("label1.Error1"));
            this.infoProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding1"))));
            this.label1.Name = "label1";
            this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // showLeafClassButton
            // 
            resources.ApplyResources(this.showLeafClassButton, "showLeafClassButton");
            this.infoProvider.SetError(this.showLeafClassButton, resources.GetString("showLeafClassButton.Error"));
            this.errorProvider.SetError(this.showLeafClassButton, resources.GetString("showLeafClassButton.Error1"));
            this.infoProvider.SetIconAlignment(this.showLeafClassButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("showLeafClassButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.showLeafClassButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("showLeafClassButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.showLeafClassButton, ((int)(resources.GetObject("showLeafClassButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.showLeafClassButton, ((int)(resources.GetObject("showLeafClassButton.IconPadding1"))));
            this.showLeafClassButton.Name = "showLeafClassButton";
            this.toolTip.SetToolTip(this.showLeafClassButton, resources.GetString("showLeafClassButton.ToolTip"));
            this.showLeafClassButton.UseVisualStyleBackColor = true;
            this.showLeafClassButton.Click += new System.EventHandler(this.showSubClassesButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.infoProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error1"));
            this.infoProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding1"))));
            this.label2.Name = "label2";
            this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // linkedClassesListView
            // 
            resources.ApplyResources(this.linkedClassesListView, "linkedClassesListView");
            this.linkedClassesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.linkedClassCaptionColumnHeader,
            this.relationshipCaptionColumnHeader,
            this.relationshipTypeColumnHeader});
            this.infoProvider.SetError(this.linkedClassesListView, resources.GetString("linkedClassesListView.Error"));
            this.errorProvider.SetError(this.linkedClassesListView, resources.GetString("linkedClassesListView.Error1"));
            this.linkedClassesListView.FullRowSelect = true;
            this.infoProvider.SetIconAlignment(this.linkedClassesListView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("linkedClassesListView.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.linkedClassesListView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("linkedClassesListView.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.linkedClassesListView, ((int)(resources.GetObject("linkedClassesListView.IconPadding"))));
            this.errorProvider.SetIconPadding(this.linkedClassesListView, ((int)(resources.GetObject("linkedClassesListView.IconPadding1"))));
            this.linkedClassesListView.MultiSelect = false;
            this.linkedClassesListView.Name = "linkedClassesListView";
            this.linkedClassesListView.SmallImageList = this.treeImageList;
            this.toolTip.SetToolTip(this.linkedClassesListView, resources.GetString("linkedClassesListView.ToolTip"));
            this.linkedClassesListView.UseCompatibleStateImageBehavior = false;
            this.linkedClassesListView.View = System.Windows.Forms.View.Details;
            this.linkedClassesListView.SelectedIndexChanged += new System.EventHandler(this.linkedClassesListView_SelectedIndexChanged);
            // 
            // linkedClassCaptionColumnHeader
            // 
            resources.ApplyResources(this.linkedClassCaptionColumnHeader, "linkedClassCaptionColumnHeader");
            // 
            // relationshipCaptionColumnHeader
            // 
            resources.ApplyResources(this.relationshipCaptionColumnHeader, "relationshipCaptionColumnHeader");
            // 
            // relationshipTypeColumnHeader
            // 
            resources.ApplyResources(this.relationshipTypeColumnHeader, "relationshipTypeColumnHeader");
            // 
            // treeImageList
            // 
            this.treeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImageList.ImageStream")));
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeImageList.Images.SetKeyName(0, "");
            this.treeImageList.Images.SetKeyName(1, "");
            this.treeImageList.Images.SetKeyName(2, "");
            this.treeImageList.Images.SetKeyName(3, "");
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.infoProvider.SetError(this.addButton, resources.GetString("addButton.Error"));
            this.errorProvider.SetError(this.addButton, resources.GetString("addButton.Error1"));
            this.infoProvider.SetIconAlignment(this.addButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.addButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.addButton, ((int)(resources.GetObject("addButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.addButton, ((int)(resources.GetObject("addButton.IconPadding1"))));
            this.addButton.Name = "addButton";
            this.toolTip.SetToolTip(this.addButton, resources.GetString("addButton.ToolTip"));
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeClassButton
            // 
            resources.ApplyResources(this.removeClassButton, "removeClassButton");
            this.infoProvider.SetError(this.removeClassButton, resources.GetString("removeClassButton.Error"));
            this.errorProvider.SetError(this.removeClassButton, resources.GetString("removeClassButton.Error1"));
            this.infoProvider.SetIconAlignment(this.removeClassButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("removeClassButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.removeClassButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("removeClassButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.removeClassButton, ((int)(resources.GetObject("removeClassButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.removeClassButton, ((int)(resources.GetObject("removeClassButton.IconPadding1"))));
            this.removeClassButton.Name = "removeClassButton";
            this.toolTip.SetToolTip(this.removeClassButton, resources.GetString("removeClassButton.ToolTip"));
            this.removeClassButton.Click += new System.EventHandler(this.removeClassButton_Click);
            // 
            // schemaTreeView
            // 
            resources.ApplyResources(this.schemaTreeView, "schemaTreeView");
            this.errorProvider.SetError(this.schemaTreeView, resources.GetString("schemaTreeView.Error"));
            this.infoProvider.SetError(this.schemaTreeView, resources.GetString("schemaTreeView.Error1"));
            this.schemaTreeView.HideSelection = false;
            this.errorProvider.SetIconAlignment(this.schemaTreeView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaTreeView.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.schemaTreeView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaTreeView.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.schemaTreeView, ((int)(resources.GetObject("schemaTreeView.IconPadding"))));
            this.infoProvider.SetIconPadding(this.schemaTreeView, ((int)(resources.GetObject("schemaTreeView.IconPadding1"))));
            this.schemaTreeView.ImageList = this.treeImageList;
            this.schemaTreeView.ItemHeight = 16;
            this.schemaTreeView.Name = "schemaTreeView";
            this.toolTip.SetToolTip(this.schemaTreeView, resources.GetString("schemaTreeView.ToolTip"));
            this.schemaTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.schemaTreeView_AfterSelect);
            // 
            // msgLabel
            // 
            resources.ApplyResources(this.msgLabel, "msgLabel");
            this.infoProvider.SetError(this.msgLabel, resources.GetString("msgLabel.Error"));
            this.errorProvider.SetError(this.msgLabel, resources.GetString("msgLabel.Error1"));
            this.msgLabel.ForeColor = System.Drawing.Color.Red;
            this.infoProvider.SetIconAlignment(this.msgLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("msgLabel.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.msgLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("msgLabel.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.msgLabel, ((int)(resources.GetObject("msgLabel.IconPadding"))));
            this.infoProvider.SetIconPadding(this.msgLabel, ((int)(resources.GetObject("msgLabel.IconPadding1"))));
            this.msgLabel.Name = "msgLabel";
            this.toolTip.SetToolTip(this.msgLabel, resources.GetString("msgLabel.ToolTip"));
            // 
            // baseClassTextBox
            // 
            resources.ApplyResources(this.baseClassTextBox, "baseClassTextBox");
            this.errorProvider.SetError(this.baseClassTextBox, resources.GetString("baseClassTextBox.Error"));
            this.infoProvider.SetError(this.baseClassTextBox, resources.GetString("baseClassTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.baseClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("baseClassTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.baseClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("baseClassTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.baseClassTextBox, ((int)(resources.GetObject("baseClassTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.baseClassTextBox, ((int)(resources.GetObject("baseClassTextBox.IconPadding1"))));
            this.baseClassTextBox.Name = "baseClassTextBox";
            this.baseClassTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.baseClassTextBox, resources.GetString("baseClassTextBox.ToolTip"));
            // 
            // schemaViewNameTextBox
            // 
            resources.ApplyResources(this.schemaViewNameTextBox, "schemaViewNameTextBox");
            this.errorProvider.SetError(this.schemaViewNameTextBox, resources.GetString("schemaViewNameTextBox.Error"));
            this.infoProvider.SetError(this.schemaViewNameTextBox, resources.GetString("schemaViewNameTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.schemaViewNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewNameTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.schemaViewNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewNameTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.schemaViewNameTextBox, ((int)(resources.GetObject("schemaViewNameTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.schemaViewNameTextBox, ((int)(resources.GetObject("schemaViewNameTextBox.IconPadding1"))));
            this.schemaViewNameTextBox.Name = "schemaViewNameTextBox";
            this.toolTip.SetToolTip(this.schemaViewNameTextBox, resources.GetString("schemaViewNameTextBox.ToolTip"));
            this.schemaViewNameTextBox.TextChanged += new System.EventHandler(this.schemaViewNameTextBox_TextChanged);
            this.schemaViewNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.dataViewNameTextBox_Validating);
            // 
            // schemaViewCaptionTextBox
            // 
            resources.ApplyResources(this.schemaViewCaptionTextBox, "schemaViewCaptionTextBox");
            this.errorProvider.SetError(this.schemaViewCaptionTextBox, resources.GetString("schemaViewCaptionTextBox.Error"));
            this.infoProvider.SetError(this.schemaViewCaptionTextBox, resources.GetString("schemaViewCaptionTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.schemaViewCaptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewCaptionTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.schemaViewCaptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewCaptionTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.schemaViewCaptionTextBox, ((int)(resources.GetObject("schemaViewCaptionTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.schemaViewCaptionTextBox, ((int)(resources.GetObject("schemaViewCaptionTextBox.IconPadding1"))));
            this.schemaViewCaptionTextBox.Name = "schemaViewCaptionTextBox";
            this.toolTip.SetToolTip(this.schemaViewCaptionTextBox, resources.GetString("schemaViewCaptionTextBox.ToolTip"));
            this.schemaViewCaptionTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.dataViewCaptionTextBox_Validating);
            // 
            // schemaViewTabControl
            // 
            resources.ApplyResources(this.schemaViewTabControl, "schemaViewTabControl");
            this.schemaViewTabControl.Controls.Add(this.basicInfoTabPage);
            this.schemaViewTabControl.Controls.Add(this.classesTabPage);
            this.schemaViewTabControl.Controls.Add(this.attributesTabPage);
            this.schemaViewTabControl.Controls.Add(this.ImportScripts);
            this.errorProvider.SetError(this.schemaViewTabControl, resources.GetString("schemaViewTabControl.Error"));
            this.infoProvider.SetError(this.schemaViewTabControl, resources.GetString("schemaViewTabControl.Error1"));
            this.infoProvider.SetIconAlignment(this.schemaViewTabControl, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewTabControl.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.schemaViewTabControl, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewTabControl.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.schemaViewTabControl, ((int)(resources.GetObject("schemaViewTabControl.IconPadding"))));
            this.errorProvider.SetIconPadding(this.schemaViewTabControl, ((int)(resources.GetObject("schemaViewTabControl.IconPadding1"))));
            this.schemaViewTabControl.Name = "schemaViewTabControl";
            this.schemaViewTabControl.SelectedIndex = 0;
            this.toolTip.SetToolTip(this.schemaViewTabControl, resources.GetString("schemaViewTabControl.ToolTip"));
            this.schemaViewTabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // basicInfoTabPage
            // 
            resources.ApplyResources(this.basicInfoTabPage, "basicInfoTabPage");
            this.basicInfoTabPage.Controls.Add(this.postProcessorTextBox);
            this.basicInfoTabPage.Controls.Add(this.label16);
            this.basicInfoTabPage.Controls.Add(this.multipleOccurCheckBox);
            this.basicInfoTabPage.Controls.Add(this.baseClassButton);
            this.basicInfoTabPage.Controls.Add(this.label11);
            this.basicInfoTabPage.Controls.Add(this.schemaViewDescriptionTextBox);
            this.basicInfoTabPage.Controls.Add(this.baseClassTextBox);
            this.basicInfoTabPage.Controls.Add(this.schemaViewCaptionTextBox);
            this.basicInfoTabPage.Controls.Add(this.schemaViewNameTextBox);
            this.basicInfoTabPage.Controls.Add(this.label6);
            this.basicInfoTabPage.Controls.Add(this.label5);
            this.basicInfoTabPage.Controls.Add(this.label4);
            this.errorProvider.SetError(this.basicInfoTabPage, resources.GetString("basicInfoTabPage.Error"));
            this.infoProvider.SetError(this.basicInfoTabPage, resources.GetString("basicInfoTabPage.Error1"));
            this.infoProvider.SetIconAlignment(this.basicInfoTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("basicInfoTabPage.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.basicInfoTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("basicInfoTabPage.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.basicInfoTabPage, ((int)(resources.GetObject("basicInfoTabPage.IconPadding"))));
            this.infoProvider.SetIconPadding(this.basicInfoTabPage, ((int)(resources.GetObject("basicInfoTabPage.IconPadding1"))));
            this.basicInfoTabPage.Name = "basicInfoTabPage";
            this.toolTip.SetToolTip(this.basicInfoTabPage, resources.GetString("basicInfoTabPage.ToolTip"));
            this.basicInfoTabPage.UseVisualStyleBackColor = true;
            // 
            // postProcessorTextBox
            // 
            resources.ApplyResources(this.postProcessorTextBox, "postProcessorTextBox");
            this.errorProvider.SetError(this.postProcessorTextBox, resources.GetString("postProcessorTextBox.Error"));
            this.infoProvider.SetError(this.postProcessorTextBox, resources.GetString("postProcessorTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.postProcessorTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("postProcessorTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.postProcessorTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("postProcessorTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.postProcessorTextBox, ((int)(resources.GetObject("postProcessorTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.postProcessorTextBox, ((int)(resources.GetObject("postProcessorTextBox.IconPadding1"))));
            this.postProcessorTextBox.Name = "postProcessorTextBox";
            this.toolTip.SetToolTip(this.postProcessorTextBox, resources.GetString("postProcessorTextBox.ToolTip"));
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.infoProvider.SetError(this.label16, resources.GetString("label16.Error"));
            this.errorProvider.SetError(this.label16, resources.GetString("label16.Error1"));
            this.infoProvider.SetIconAlignment(this.label16, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label16.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label16, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label16.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label16, ((int)(resources.GetObject("label16.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label16, ((int)(resources.GetObject("label16.IconPadding1"))));
            this.label16.Name = "label16";
            this.toolTip.SetToolTip(this.label16, resources.GetString("label16.ToolTip"));
            // 
            // multipleOccurCheckBox
            // 
            resources.ApplyResources(this.multipleOccurCheckBox, "multipleOccurCheckBox");
            this.infoProvider.SetError(this.multipleOccurCheckBox, resources.GetString("multipleOccurCheckBox.Error"));
            this.errorProvider.SetError(this.multipleOccurCheckBox, resources.GetString("multipleOccurCheckBox.Error1"));
            this.infoProvider.SetIconAlignment(this.multipleOccurCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("multipleOccurCheckBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.multipleOccurCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("multipleOccurCheckBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.multipleOccurCheckBox, ((int)(resources.GetObject("multipleOccurCheckBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.multipleOccurCheckBox, ((int)(resources.GetObject("multipleOccurCheckBox.IconPadding1"))));
            this.multipleOccurCheckBox.Name = "multipleOccurCheckBox";
            this.toolTip.SetToolTip(this.multipleOccurCheckBox, resources.GetString("multipleOccurCheckBox.ToolTip"));
            this.multipleOccurCheckBox.UseVisualStyleBackColor = true;
            this.multipleOccurCheckBox.CheckedChanged += new System.EventHandler(this.multipleOccurCheckBox_CheckedChanged);
            // 
            // baseClassButton
            // 
            resources.ApplyResources(this.baseClassButton, "baseClassButton");
            this.infoProvider.SetError(this.baseClassButton, resources.GetString("baseClassButton.Error"));
            this.errorProvider.SetError(this.baseClassButton, resources.GetString("baseClassButton.Error1"));
            this.infoProvider.SetIconAlignment(this.baseClassButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("baseClassButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.baseClassButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("baseClassButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.baseClassButton, ((int)(resources.GetObject("baseClassButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.baseClassButton, ((int)(resources.GetObject("baseClassButton.IconPadding1"))));
            this.baseClassButton.Name = "baseClassButton";
            this.toolTip.SetToolTip(this.baseClassButton, resources.GetString("baseClassButton.ToolTip"));
            this.baseClassButton.Click += new System.EventHandler(this.baseClassButton_Click);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.infoProvider.SetError(this.label11, resources.GetString("label11.Error"));
            this.errorProvider.SetError(this.label11, resources.GetString("label11.Error1"));
            this.infoProvider.SetIconAlignment(this.label11, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label11.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label11, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label11.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label11, ((int)(resources.GetObject("label11.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label11, ((int)(resources.GetObject("label11.IconPadding1"))));
            this.label11.Name = "label11";
            this.toolTip.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // schemaViewDescriptionTextBox
            // 
            resources.ApplyResources(this.schemaViewDescriptionTextBox, "schemaViewDescriptionTextBox");
            this.errorProvider.SetError(this.schemaViewDescriptionTextBox, resources.GetString("schemaViewDescriptionTextBox.Error"));
            this.infoProvider.SetError(this.schemaViewDescriptionTextBox, resources.GetString("schemaViewDescriptionTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.schemaViewDescriptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewDescriptionTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.schemaViewDescriptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaViewDescriptionTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.schemaViewDescriptionTextBox, ((int)(resources.GetObject("schemaViewDescriptionTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.schemaViewDescriptionTextBox, ((int)(resources.GetObject("schemaViewDescriptionTextBox.IconPadding1"))));
            this.schemaViewDescriptionTextBox.Name = "schemaViewDescriptionTextBox";
            this.toolTip.SetToolTip(this.schemaViewDescriptionTextBox, resources.GetString("schemaViewDescriptionTextBox.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.infoProvider.SetError(this.label6, resources.GetString("label6.Error"));
            this.errorProvider.SetError(this.label6, resources.GetString("label6.Error1"));
            this.infoProvider.SetIconAlignment(this.label6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label6.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label6.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label6, ((int)(resources.GetObject("label6.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label6, ((int)(resources.GetObject("label6.IconPadding1"))));
            this.label6.Name = "label6";
            this.toolTip.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.infoProvider.SetError(this.label5, resources.GetString("label5.Error"));
            this.errorProvider.SetError(this.label5, resources.GetString("label5.Error1"));
            this.infoProvider.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding1"))));
            this.label5.Name = "label5";
            this.toolTip.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.infoProvider.SetError(this.label4, resources.GetString("label4.Error"));
            this.errorProvider.SetError(this.label4, resources.GetString("label4.Error1"));
            this.infoProvider.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding1"))));
            this.label4.Name = "label4";
            this.toolTip.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // classesTabPage
            // 
            resources.ApplyResources(this.classesTabPage, "classesTabPage");
            this.classesTabPage.Controls.Add(this.classesGroupBox);
            this.errorProvider.SetError(this.classesTabPage, resources.GetString("classesTabPage.Error"));
            this.infoProvider.SetError(this.classesTabPage, resources.GetString("classesTabPage.Error1"));
            this.infoProvider.SetIconAlignment(this.classesTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("classesTabPage.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.classesTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("classesTabPage.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.classesTabPage, ((int)(resources.GetObject("classesTabPage.IconPadding"))));
            this.infoProvider.SetIconPadding(this.classesTabPage, ((int)(resources.GetObject("classesTabPage.IconPadding1"))));
            this.classesTabPage.Name = "classesTabPage";
            this.toolTip.SetToolTip(this.classesTabPage, resources.GetString("classesTabPage.ToolTip"));
            this.classesTabPage.UseVisualStyleBackColor = true;
            // 
            // attributesTabPage
            // 
            resources.ApplyResources(this.attributesTabPage, "attributesTabPage");
            this.attributesTabPage.Controls.Add(this.tabControl1);
            this.attributesTabPage.Controls.Add(this.label3);
            this.attributesTabPage.Controls.Add(this.schemaTreeView1);
            this.errorProvider.SetError(this.attributesTabPage, resources.GetString("attributesTabPage.Error"));
            this.infoProvider.SetError(this.attributesTabPage, resources.GetString("attributesTabPage.Error1"));
            this.infoProvider.SetIconAlignment(this.attributesTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("attributesTabPage.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.attributesTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("attributesTabPage.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.attributesTabPage, ((int)(resources.GetObject("attributesTabPage.IconPadding"))));
            this.infoProvider.SetIconPadding(this.attributesTabPage, ((int)(resources.GetObject("attributesTabPage.IconPadding1"))));
            this.attributesTabPage.Name = "attributesTabPage";
            this.toolTip.SetToolTip(this.attributesTabPage, resources.GetString("attributesTabPage.ToolTip"));
            this.attributesTabPage.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.errorProvider.SetError(this.tabControl1, resources.GetString("tabControl1.Error"));
            this.infoProvider.SetError(this.tabControl1, resources.GetString("tabControl1.Error1"));
            this.infoProvider.SetIconAlignment(this.tabControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl1.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.tabControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl1.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.tabControl1, ((int)(resources.GetObject("tabControl1.IconPadding"))));
            this.errorProvider.SetIconPadding(this.tabControl1, ((int)(resources.GetObject("tabControl1.IconPadding1"))));
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.renameResult);
            this.tabPage1.Controls.Add(this.resultsListView);
            this.tabPage1.Controls.Add(this.refreshButton);
            this.tabPage1.Controls.Add(this.addResultsButton);
            this.tabPage1.Controls.Add(this.removeResultButton);
            this.errorProvider.SetError(this.tabPage1, resources.GetString("tabPage1.Error"));
            this.infoProvider.SetError(this.tabPage1, resources.GetString("tabPage1.Error1"));
            this.infoProvider.SetIconAlignment(this.tabPage1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPage1.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.tabPage1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPage1.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.tabPage1, ((int)(resources.GetObject("tabPage1.IconPadding"))));
            this.infoProvider.SetIconPadding(this.tabPage1, ((int)(resources.GetObject("tabPage1.IconPadding1"))));
            this.tabPage1.Name = "tabPage1";
            this.toolTip.SetToolTip(this.tabPage1, resources.GetString("tabPage1.ToolTip"));
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // renameResult
            // 
            resources.ApplyResources(this.renameResult, "renameResult");
            this.infoProvider.SetError(this.renameResult, resources.GetString("renameResult.Error"));
            this.errorProvider.SetError(this.renameResult, resources.GetString("renameResult.Error1"));
            this.infoProvider.SetIconAlignment(this.renameResult, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("renameResult.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.renameResult, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("renameResult.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.renameResult, ((int)(resources.GetObject("renameResult.IconPadding"))));
            this.infoProvider.SetIconPadding(this.renameResult, ((int)(resources.GetObject("renameResult.IconPadding1"))));
            this.renameResult.Name = "renameResult";
            this.toolTip.SetToolTip(this.renameResult, resources.GetString("renameResult.ToolTip"));
            this.renameResult.Click += new System.EventHandler(this.renameResult_Click);
            // 
            // resultsListView
            // 
            resources.ApplyResources(this.resultsListView, "resultsListView");
            this.resultsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.resultCaptionColumnHeader,
            this.resultNameColumnHeader});
            this.infoProvider.SetError(this.resultsListView, resources.GetString("resultsListView.Error"));
            this.errorProvider.SetError(this.resultsListView, resources.GetString("resultsListView.Error1"));
            this.resultsListView.FullRowSelect = true;
            this.infoProvider.SetIconAlignment(this.resultsListView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("resultsListView.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.resultsListView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("resultsListView.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.resultsListView, ((int)(resources.GetObject("resultsListView.IconPadding"))));
            this.errorProvider.SetIconPadding(this.resultsListView, ((int)(resources.GetObject("resultsListView.IconPadding1"))));
            this.resultsListView.LargeImageList = this.smallIconImageList;
            this.resultsListView.MultiSelect = false;
            this.resultsListView.Name = "resultsListView";
            this.resultsListView.SmallImageList = this.smallIconImageList;
            this.toolTip.SetToolTip(this.resultsListView, resources.GetString("resultsListView.ToolTip"));
            this.resultsListView.UseCompatibleStateImageBehavior = false;
            this.resultsListView.View = System.Windows.Forms.View.Details;
            this.resultsListView.SelectedIndexChanged += new System.EventHandler(this.resultsListView_SelectedIndexChanged);
            // 
            // resultCaptionColumnHeader
            // 
            resources.ApplyResources(this.resultCaptionColumnHeader, "resultCaptionColumnHeader");
            // 
            // resultNameColumnHeader
            // 
            resources.ApplyResources(this.resultNameColumnHeader, "resultNameColumnHeader");
            // 
            // refreshButton
            // 
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.infoProvider.SetError(this.refreshButton, resources.GetString("refreshButton.Error"));
            this.errorProvider.SetError(this.refreshButton, resources.GetString("refreshButton.Error1"));
            this.infoProvider.SetIconAlignment(this.refreshButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("refreshButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.refreshButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("refreshButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.refreshButton, ((int)(resources.GetObject("refreshButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.refreshButton, ((int)(resources.GetObject("refreshButton.IconPadding1"))));
            this.refreshButton.Name = "refreshButton";
            this.toolTip.SetToolTip(this.refreshButton, resources.GetString("refreshButton.ToolTip"));
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // addResultsButton
            // 
            resources.ApplyResources(this.addResultsButton, "addResultsButton");
            this.infoProvider.SetError(this.addResultsButton, resources.GetString("addResultsButton.Error"));
            this.errorProvider.SetError(this.addResultsButton, resources.GetString("addResultsButton.Error1"));
            this.infoProvider.SetIconAlignment(this.addResultsButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addResultsButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.addResultsButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addResultsButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.addResultsButton, ((int)(resources.GetObject("addResultsButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.addResultsButton, ((int)(resources.GetObject("addResultsButton.IconPadding1"))));
            this.addResultsButton.Name = "addResultsButton";
            this.toolTip.SetToolTip(this.addResultsButton, resources.GetString("addResultsButton.ToolTip"));
            this.addResultsButton.Click += new System.EventHandler(this.addResultsButton_Click);
            // 
            // removeResultButton
            // 
            resources.ApplyResources(this.removeResultButton, "removeResultButton");
            this.infoProvider.SetError(this.removeResultButton, resources.GetString("removeResultButton.Error"));
            this.errorProvider.SetError(this.removeResultButton, resources.GetString("removeResultButton.Error1"));
            this.infoProvider.SetIconAlignment(this.removeResultButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("removeResultButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.removeResultButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("removeResultButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.removeResultButton, ((int)(resources.GetObject("removeResultButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.removeResultButton, ((int)(resources.GetObject("removeResultButton.IconPadding1"))));
            this.removeResultButton.Name = "removeResultButton";
            this.toolTip.SetToolTip(this.removeResultButton, resources.GetString("removeResultButton.ToolTip"));
            this.removeResultButton.Click += new System.EventHandler(this.removeResultButton_Click);
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.attributeNameLabel);
            this.tabPage2.Controls.Add(this.label15);
            this.errorProvider.SetError(this.tabPage2, resources.GetString("tabPage2.Error"));
            this.infoProvider.SetError(this.tabPage2, resources.GetString("tabPage2.Error1"));
            this.infoProvider.SetIconAlignment(this.tabPage2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPage2.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.tabPage2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPage2.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.tabPage2, ((int)(resources.GetObject("tabPage2.IconPadding"))));
            this.infoProvider.SetIconPadding(this.tabPage2, ((int)(resources.GetObject("tabPage2.IconPadding1"))));
            this.tabPage2.Name = "tabPage2";
            this.toolTip.SetToolTip(this.tabPage2, resources.GetString("tabPage2.ToolTip"));
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Controls.Add(this.labelRadioButton);
            this.groupBox8.Controls.Add(this.featureRadioButton);
            this.errorProvider.SetError(this.groupBox8, resources.GetString("groupBox8.Error"));
            this.infoProvider.SetError(this.groupBox8, resources.GetString("groupBox8.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox8.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox8.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox8, ((int)(resources.GetObject("groupBox8.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox8, ((int)(resources.GetObject("groupBox8.IconPadding1"))));
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox8, resources.GetString("groupBox8.ToolTip"));
            // 
            // labelRadioButton
            // 
            resources.ApplyResources(this.labelRadioButton, "labelRadioButton");
            this.infoProvider.SetError(this.labelRadioButton, resources.GetString("labelRadioButton.Error"));
            this.errorProvider.SetError(this.labelRadioButton, resources.GetString("labelRadioButton.Error1"));
            this.errorProvider.SetIconAlignment(this.labelRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelRadioButton.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.labelRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelRadioButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.labelRadioButton, ((int)(resources.GetObject("labelRadioButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.labelRadioButton, ((int)(resources.GetObject("labelRadioButton.IconPadding1"))));
            this.labelRadioButton.Name = "labelRadioButton";
            this.labelRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.labelRadioButton, resources.GetString("labelRadioButton.ToolTip"));
            this.labelRadioButton.UseVisualStyleBackColor = true;
            // 
            // featureRadioButton
            // 
            resources.ApplyResources(this.featureRadioButton, "featureRadioButton");
            this.infoProvider.SetError(this.featureRadioButton, resources.GetString("featureRadioButton.Error"));
            this.errorProvider.SetError(this.featureRadioButton, resources.GetString("featureRadioButton.Error1"));
            this.errorProvider.SetIconAlignment(this.featureRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("featureRadioButton.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.featureRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("featureRadioButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.featureRadioButton, ((int)(resources.GetObject("featureRadioButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.featureRadioButton, ((int)(resources.GetObject("featureRadioButton.IconPadding1"))));
            this.featureRadioButton.Name = "featureRadioButton";
            this.featureRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.featureRadioButton, resources.GetString("featureRadioButton.ToolTip"));
            this.featureRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.categoryAxisCheckBox);
            this.groupBox7.Controls.Add(this.xAxisCheckBox);
            this.errorProvider.SetError(this.groupBox7, resources.GetString("groupBox7.Error"));
            this.infoProvider.SetError(this.groupBox7, resources.GetString("groupBox7.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox7, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox7.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox7, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox7.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox7, ((int)(resources.GetObject("groupBox7.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox7, ((int)(resources.GetObject("groupBox7.IconPadding1"))));
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox7, resources.GetString("groupBox7.ToolTip"));
            // 
            // categoryAxisCheckBox
            // 
            resources.ApplyResources(this.categoryAxisCheckBox, "categoryAxisCheckBox");
            this.infoProvider.SetError(this.categoryAxisCheckBox, resources.GetString("categoryAxisCheckBox.Error"));
            this.errorProvider.SetError(this.categoryAxisCheckBox, resources.GetString("categoryAxisCheckBox.Error1"));
            this.infoProvider.SetIconAlignment(this.categoryAxisCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("categoryAxisCheckBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.categoryAxisCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("categoryAxisCheckBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.categoryAxisCheckBox, ((int)(resources.GetObject("categoryAxisCheckBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.categoryAxisCheckBox, ((int)(resources.GetObject("categoryAxisCheckBox.IconPadding1"))));
            this.categoryAxisCheckBox.Name = "categoryAxisCheckBox";
            this.toolTip.SetToolTip(this.categoryAxisCheckBox, resources.GetString("categoryAxisCheckBox.ToolTip"));
            this.categoryAxisCheckBox.UseVisualStyleBackColor = true;
            this.categoryAxisCheckBox.CheckedChanged += new System.EventHandler(this.categoryAxisCheckBox_CheckedChanged);
            // 
            // xAxisCheckBox
            // 
            resources.ApplyResources(this.xAxisCheckBox, "xAxisCheckBox");
            this.infoProvider.SetError(this.xAxisCheckBox, resources.GetString("xAxisCheckBox.Error"));
            this.errorProvider.SetError(this.xAxisCheckBox, resources.GetString("xAxisCheckBox.Error1"));
            this.infoProvider.SetIconAlignment(this.xAxisCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("xAxisCheckBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.xAxisCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("xAxisCheckBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.xAxisCheckBox, ((int)(resources.GetObject("xAxisCheckBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.xAxisCheckBox, ((int)(resources.GetObject("xAxisCheckBox.IconPadding1"))));
            this.xAxisCheckBox.Name = "xAxisCheckBox";
            this.toolTip.SetToolTip(this.xAxisCheckBox, resources.GetString("xAxisCheckBox.ToolTip"));
            this.xAxisCheckBox.UseVisualStyleBackColor = true;
            this.xAxisCheckBox.CheckedChanged += new System.EventHandler(this.xAxisCheckBox_CheckedChanged);
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.sortEnabledComboBox);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.sortDirectionComboBox);
            this.groupBox6.Controls.Add(this.sortOrderTextBox);
            this.groupBox6.Controls.Add(this.label14);
            this.errorProvider.SetError(this.groupBox6, resources.GetString("groupBox6.Error"));
            this.infoProvider.SetError(this.groupBox6, resources.GetString("groupBox6.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox6.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox6.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox6, ((int)(resources.GetObject("groupBox6.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox6, ((int)(resources.GetObject("groupBox6.IconPadding1"))));
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.infoProvider.SetError(this.label7, resources.GetString("label7.Error"));
            this.errorProvider.SetError(this.label7, resources.GetString("label7.Error1"));
            this.infoProvider.SetIconAlignment(this.label7, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label7.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label7, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label7.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label7, ((int)(resources.GetObject("label7.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label7, ((int)(resources.GetObject("label7.IconPadding1"))));
            this.label7.Name = "label7";
            this.toolTip.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // sortEnabledComboBox
            // 
            resources.ApplyResources(this.sortEnabledComboBox, "sortEnabledComboBox");
            this.infoProvider.SetError(this.sortEnabledComboBox, resources.GetString("sortEnabledComboBox.Error"));
            this.errorProvider.SetError(this.sortEnabledComboBox, resources.GetString("sortEnabledComboBox.Error1"));
            this.sortEnabledComboBox.FormattingEnabled = true;
            this.infoProvider.SetIconAlignment(this.sortEnabledComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("sortEnabledComboBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.sortEnabledComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("sortEnabledComboBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.sortEnabledComboBox, ((int)(resources.GetObject("sortEnabledComboBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.sortEnabledComboBox, ((int)(resources.GetObject("sortEnabledComboBox.IconPadding1"))));
            this.sortEnabledComboBox.Items.AddRange(new object[] {
            resources.GetString("sortEnabledComboBox.Items"),
            resources.GetString("sortEnabledComboBox.Items1")});
            this.sortEnabledComboBox.Name = "sortEnabledComboBox";
            this.toolTip.SetToolTip(this.sortEnabledComboBox, resources.GetString("sortEnabledComboBox.ToolTip"));
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.infoProvider.SetError(this.label13, resources.GetString("label13.Error"));
            this.errorProvider.SetError(this.label13, resources.GetString("label13.Error1"));
            this.infoProvider.SetIconAlignment(this.label13, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label13.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label13, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label13.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label13, ((int)(resources.GetObject("label13.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label13, ((int)(resources.GetObject("label13.IconPadding1"))));
            this.label13.Name = "label13";
            this.toolTip.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
            // 
            // sortDirectionComboBox
            // 
            resources.ApplyResources(this.sortDirectionComboBox, "sortDirectionComboBox");
            this.infoProvider.SetError(this.sortDirectionComboBox, resources.GetString("sortDirectionComboBox.Error"));
            this.errorProvider.SetError(this.sortDirectionComboBox, resources.GetString("sortDirectionComboBox.Error1"));
            this.sortDirectionComboBox.FormattingEnabled = true;
            this.infoProvider.SetIconAlignment(this.sortDirectionComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("sortDirectionComboBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.sortDirectionComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("sortDirectionComboBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.sortDirectionComboBox, ((int)(resources.GetObject("sortDirectionComboBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.sortDirectionComboBox, ((int)(resources.GetObject("sortDirectionComboBox.IconPadding1"))));
            this.sortDirectionComboBox.Items.AddRange(new object[] {
            resources.GetString("sortDirectionComboBox.Items"),
            resources.GetString("sortDirectionComboBox.Items1")});
            this.sortDirectionComboBox.Name = "sortDirectionComboBox";
            this.toolTip.SetToolTip(this.sortDirectionComboBox, resources.GetString("sortDirectionComboBox.ToolTip"));
            // 
            // sortOrderTextBox
            // 
            resources.ApplyResources(this.sortOrderTextBox, "sortOrderTextBox");
            this.errorProvider.SetError(this.sortOrderTextBox, resources.GetString("sortOrderTextBox.Error"));
            this.infoProvider.SetError(this.sortOrderTextBox, resources.GetString("sortOrderTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.sortOrderTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("sortOrderTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.sortOrderTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("sortOrderTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.sortOrderTextBox, ((int)(resources.GetObject("sortOrderTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.sortOrderTextBox, ((int)(resources.GetObject("sortOrderTextBox.IconPadding1"))));
            this.sortOrderTextBox.Name = "sortOrderTextBox";
            this.toolTip.SetToolTip(this.sortOrderTextBox, resources.GetString("sortOrderTextBox.ToolTip"));
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.infoProvider.SetError(this.label14, resources.GetString("label14.Error"));
            this.errorProvider.SetError(this.label14, resources.GetString("label14.Error1"));
            this.infoProvider.SetIconAlignment(this.label14, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label14.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label14, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label14.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label14, ((int)(resources.GetObject("label14.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label14, ((int)(resources.GetObject("label14.IconPadding1"))));
            this.label14.Name = "label14";
            this.toolTip.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
            // 
            // attributeNameLabel
            // 
            resources.ApplyResources(this.attributeNameLabel, "attributeNameLabel");
            this.infoProvider.SetError(this.attributeNameLabel, resources.GetString("attributeNameLabel.Error"));
            this.errorProvider.SetError(this.attributeNameLabel, resources.GetString("attributeNameLabel.Error1"));
            this.infoProvider.SetIconAlignment(this.attributeNameLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("attributeNameLabel.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.attributeNameLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("attributeNameLabel.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.attributeNameLabel, ((int)(resources.GetObject("attributeNameLabel.IconPadding"))));
            this.infoProvider.SetIconPadding(this.attributeNameLabel, ((int)(resources.GetObject("attributeNameLabel.IconPadding1"))));
            this.attributeNameLabel.Name = "attributeNameLabel";
            this.toolTip.SetToolTip(this.attributeNameLabel, resources.GetString("attributeNameLabel.ToolTip"));
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.infoProvider.SetError(this.label15, resources.GetString("label15.Error"));
            this.errorProvider.SetError(this.label15, resources.GetString("label15.Error1"));
            this.infoProvider.SetIconAlignment(this.label15, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label15.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label15, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label15.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label15, ((int)(resources.GetObject("label15.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label15, ((int)(resources.GetObject("label15.IconPadding1"))));
            this.label15.Name = "label15";
            this.toolTip.SetToolTip(this.label15, resources.GetString("label15.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.infoProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error1"));
            this.infoProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding1"))));
            this.label3.Name = "label3";
            this.toolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // schemaTreeView1
            // 
            resources.ApplyResources(this.schemaTreeView1, "schemaTreeView1");
            this.errorProvider.SetError(this.schemaTreeView1, resources.GetString("schemaTreeView1.Error"));
            this.infoProvider.SetError(this.schemaTreeView1, resources.GetString("schemaTreeView1.Error1"));
            this.schemaTreeView1.HideSelection = false;
            this.errorProvider.SetIconAlignment(this.schemaTreeView1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaTreeView1.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.schemaTreeView1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaTreeView1.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.schemaTreeView1, ((int)(resources.GetObject("schemaTreeView1.IconPadding"))));
            this.infoProvider.SetIconPadding(this.schemaTreeView1, ((int)(resources.GetObject("schemaTreeView1.IconPadding1"))));
            this.schemaTreeView1.ImageList = this.treeImageList;
            this.schemaTreeView1.ItemHeight = 16;
            this.schemaTreeView1.Name = "schemaTreeView1";
            this.toolTip.SetToolTip(this.schemaTreeView1, resources.GetString("schemaTreeView1.ToolTip"));
            this.schemaTreeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.schemaTreeView1_AfterSelect);
            // 
            // ImportScripts
            // 
            resources.ApplyResources(this.ImportScripts, "ImportScripts");
            this.ImportScripts.Controls.Add(this.groupBox5);
            this.ImportScripts.Controls.Add(this.groupBox4);
            this.ImportScripts.Controls.Add(this.groupBox3);
            this.ImportScripts.Controls.Add(this.groupBox2);
            this.ImportScripts.Controls.Add(this.groupBox1);
            this.ImportScripts.Controls.Add(this.schemaTreeView2);
            this.ImportScripts.Controls.Add(this.label8);
            this.errorProvider.SetError(this.ImportScripts, resources.GetString("ImportScripts.Error"));
            this.infoProvider.SetError(this.ImportScripts, resources.GetString("ImportScripts.Error1"));
            this.infoProvider.SetIconAlignment(this.ImportScripts, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("ImportScripts.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.ImportScripts, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("ImportScripts.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.ImportScripts, ((int)(resources.GetObject("ImportScripts.IconPadding"))));
            this.infoProvider.SetIconPadding(this.ImportScripts, ((int)(resources.GetObject("ImportScripts.IconPadding1"))));
            this.ImportScripts.Name = "ImportScripts";
            this.toolTip.SetToolTip(this.ImportScripts, resources.GetString("ImportScripts.ToolTip"));
            this.ImportScripts.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.conditionTextBox);
            this.groupBox4.Controls.Add(this.label10);
            this.errorProvider.SetError(this.groupBox4, resources.GetString("groupBox4.Error"));
            this.infoProvider.SetError(this.groupBox4, resources.GetString("groupBox4.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox4.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox4.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox4, ((int)(resources.GetObject("groupBox4.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox4, ((int)(resources.GetObject("groupBox4.IconPadding1"))));
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
            // 
            // conditionTextBox
            // 
            resources.ApplyResources(this.conditionTextBox, "conditionTextBox");
            this.errorProvider.SetError(this.conditionTextBox, resources.GetString("conditionTextBox.Error"));
            this.infoProvider.SetError(this.conditionTextBox, resources.GetString("conditionTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.conditionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("conditionTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.conditionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("conditionTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.conditionTextBox, ((int)(resources.GetObject("conditionTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.conditionTextBox, ((int)(resources.GetObject("conditionTextBox.IconPadding1"))));
            this.conditionTextBox.Name = "conditionTextBox";
            this.toolTip.SetToolTip(this.conditionTextBox, resources.GetString("conditionTextBox.ToolTip"));
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.infoProvider.SetError(this.label10, resources.GetString("label10.Error"));
            this.errorProvider.SetError(this.label10, resources.GetString("label10.Error1"));
            this.infoProvider.SetIconAlignment(this.label10, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label10.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label10, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label10.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label10, ((int)(resources.GetObject("label10.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label10, ((int)(resources.GetObject("label10.IconPadding1"))));
            this.label10.Name = "label10";
            this.toolTip.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.matchAttributeTextBox);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.mergingCheckBox);
            this.errorProvider.SetError(this.groupBox3, resources.GetString("groupBox3.Error"));
            this.infoProvider.SetError(this.groupBox3, resources.GetString("groupBox3.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox3.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox3.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox3, ((int)(resources.GetObject("groupBox3.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox3, ((int)(resources.GetObject("groupBox3.IconPadding1"))));
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // matchAttributeTextBox
            // 
            resources.ApplyResources(this.matchAttributeTextBox, "matchAttributeTextBox");
            this.errorProvider.SetError(this.matchAttributeTextBox, resources.GetString("matchAttributeTextBox.Error"));
            this.infoProvider.SetError(this.matchAttributeTextBox, resources.GetString("matchAttributeTextBox.Error1"));
            this.errorProvider.SetIconAlignment(this.matchAttributeTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("matchAttributeTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.matchAttributeTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("matchAttributeTextBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.matchAttributeTextBox, ((int)(resources.GetObject("matchAttributeTextBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.matchAttributeTextBox, ((int)(resources.GetObject("matchAttributeTextBox.IconPadding1"))));
            this.matchAttributeTextBox.Name = "matchAttributeTextBox";
            this.toolTip.SetToolTip(this.matchAttributeTextBox, resources.GetString("matchAttributeTextBox.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.infoProvider.SetError(this.label9, resources.GetString("label9.Error"));
            this.errorProvider.SetError(this.label9, resources.GetString("label9.Error1"));
            this.infoProvider.SetIconAlignment(this.label9, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label9.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label9, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label9.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label9, ((int)(resources.GetObject("label9.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label9, ((int)(resources.GetObject("label9.IconPadding1"))));
            this.label9.Name = "label9";
            this.toolTip.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // mergingCheckBox
            // 
            resources.ApplyResources(this.mergingCheckBox, "mergingCheckBox");
            this.infoProvider.SetError(this.mergingCheckBox, resources.GetString("mergingCheckBox.Error"));
            this.errorProvider.SetError(this.mergingCheckBox, resources.GetString("mergingCheckBox.Error1"));
            this.infoProvider.SetIconAlignment(this.mergingCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mergingCheckBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.mergingCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mergingCheckBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.mergingCheckBox, ((int)(resources.GetObject("mergingCheckBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.mergingCheckBox, ((int)(resources.GetObject("mergingCheckBox.IconPadding1"))));
            this.mergingCheckBox.Name = "mergingCheckBox";
            this.toolTip.SetToolTip(this.mergingCheckBox, resources.GetString("mergingCheckBox.ToolTip"));
            this.mergingCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.dbAndFilesRadioButton);
            this.groupBox2.Controls.Add(this.fileOnlyRadioButton);
            this.groupBox2.Controls.Add(this.dbOnlyRadioButton);
            this.errorProvider.SetError(this.groupBox2, resources.GetString("groupBox2.Error"));
            this.infoProvider.SetError(this.groupBox2, resources.GetString("groupBox2.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox2.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox2.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox2, ((int)(resources.GetObject("groupBox2.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox2, ((int)(resources.GetObject("groupBox2.IconPadding1"))));
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // dbAndFilesRadioButton
            // 
            resources.ApplyResources(this.dbAndFilesRadioButton, "dbAndFilesRadioButton");
            this.infoProvider.SetError(this.dbAndFilesRadioButton, resources.GetString("dbAndFilesRadioButton.Error"));
            this.errorProvider.SetError(this.dbAndFilesRadioButton, resources.GetString("dbAndFilesRadioButton.Error1"));
            this.errorProvider.SetIconAlignment(this.dbAndFilesRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dbAndFilesRadioButton.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.dbAndFilesRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dbAndFilesRadioButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.dbAndFilesRadioButton, ((int)(resources.GetObject("dbAndFilesRadioButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.dbAndFilesRadioButton, ((int)(resources.GetObject("dbAndFilesRadioButton.IconPadding1"))));
            this.dbAndFilesRadioButton.Name = "dbAndFilesRadioButton";
            this.dbAndFilesRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.dbAndFilesRadioButton, resources.GetString("dbAndFilesRadioButton.ToolTip"));
            this.dbAndFilesRadioButton.UseVisualStyleBackColor = true;
            // 
            // fileOnlyRadioButton
            // 
            resources.ApplyResources(this.fileOnlyRadioButton, "fileOnlyRadioButton");
            this.infoProvider.SetError(this.fileOnlyRadioButton, resources.GetString("fileOnlyRadioButton.Error"));
            this.errorProvider.SetError(this.fileOnlyRadioButton, resources.GetString("fileOnlyRadioButton.Error1"));
            this.errorProvider.SetIconAlignment(this.fileOnlyRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("fileOnlyRadioButton.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.fileOnlyRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("fileOnlyRadioButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.fileOnlyRadioButton, ((int)(resources.GetObject("fileOnlyRadioButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.fileOnlyRadioButton, ((int)(resources.GetObject("fileOnlyRadioButton.IconPadding1"))));
            this.fileOnlyRadioButton.Name = "fileOnlyRadioButton";
            this.fileOnlyRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.fileOnlyRadioButton, resources.GetString("fileOnlyRadioButton.ToolTip"));
            this.fileOnlyRadioButton.UseVisualStyleBackColor = true;
            // 
            // dbOnlyRadioButton
            // 
            resources.ApplyResources(this.dbOnlyRadioButton, "dbOnlyRadioButton");
            this.dbOnlyRadioButton.Checked = true;
            this.infoProvider.SetError(this.dbOnlyRadioButton, resources.GetString("dbOnlyRadioButton.Error"));
            this.errorProvider.SetError(this.dbOnlyRadioButton, resources.GetString("dbOnlyRadioButton.Error1"));
            this.errorProvider.SetIconAlignment(this.dbOnlyRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dbOnlyRadioButton.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.dbOnlyRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dbOnlyRadioButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.dbOnlyRadioButton, ((int)(resources.GetObject("dbOnlyRadioButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.dbOnlyRadioButton, ((int)(resources.GetObject("dbOnlyRadioButton.IconPadding1"))));
            this.dbOnlyRadioButton.Name = "dbOnlyRadioButton";
            this.dbOnlyRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.dbOnlyRadioButton, resources.GetString("dbOnlyRadioButton.ToolTip"));
            this.dbOnlyRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.importScriptListView);
            this.groupBox1.Controls.Add(this.addScriptButton);
            this.groupBox1.Controls.Add(this.removeScriptButton);
            this.errorProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
            this.infoProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error1"));
            this.errorProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding1"))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // importScriptListView
            // 
            resources.ApplyResources(this.importScriptListView, "importScriptListView");
            this.importScriptListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.infoProvider.SetError(this.importScriptListView, resources.GetString("importScriptListView.Error"));
            this.errorProvider.SetError(this.importScriptListView, resources.GetString("importScriptListView.Error1"));
            this.importScriptListView.FullRowSelect = true;
            this.infoProvider.SetIconAlignment(this.importScriptListView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("importScriptListView.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.importScriptListView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("importScriptListView.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.importScriptListView, ((int)(resources.GetObject("importScriptListView.IconPadding"))));
            this.errorProvider.SetIconPadding(this.importScriptListView, ((int)(resources.GetObject("importScriptListView.IconPadding1"))));
            this.importScriptListView.LargeImageList = this.smallIconImageList;
            this.importScriptListView.MultiSelect = false;
            this.importScriptListView.Name = "importScriptListView";
            this.importScriptListView.SmallImageList = this.smallIconImageList;
            this.toolTip.SetToolTip(this.importScriptListView, resources.GetString("importScriptListView.ToolTip"));
            this.importScriptListView.UseCompatibleStateImageBehavior = false;
            this.importScriptListView.View = System.Windows.Forms.View.Details;
            this.importScriptListView.SelectedIndexChanged += new System.EventHandler(this.importScriptListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // addScriptButton
            // 
            resources.ApplyResources(this.addScriptButton, "addScriptButton");
            this.infoProvider.SetError(this.addScriptButton, resources.GetString("addScriptButton.Error"));
            this.errorProvider.SetError(this.addScriptButton, resources.GetString("addScriptButton.Error1"));
            this.infoProvider.SetIconAlignment(this.addScriptButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addScriptButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.addScriptButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addScriptButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.addScriptButton, ((int)(resources.GetObject("addScriptButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.addScriptButton, ((int)(resources.GetObject("addScriptButton.IconPadding1"))));
            this.addScriptButton.Name = "addScriptButton";
            this.toolTip.SetToolTip(this.addScriptButton, resources.GetString("addScriptButton.ToolTip"));
            this.addScriptButton.Click += new System.EventHandler(this.addScriptButton_Click);
            // 
            // removeScriptButton
            // 
            resources.ApplyResources(this.removeScriptButton, "removeScriptButton");
            this.infoProvider.SetError(this.removeScriptButton, resources.GetString("removeScriptButton.Error"));
            this.errorProvider.SetError(this.removeScriptButton, resources.GetString("removeScriptButton.Error1"));
            this.infoProvider.SetIconAlignment(this.removeScriptButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("removeScriptButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.removeScriptButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("removeScriptButton.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.removeScriptButton, ((int)(resources.GetObject("removeScriptButton.IconPadding"))));
            this.infoProvider.SetIconPadding(this.removeScriptButton, ((int)(resources.GetObject("removeScriptButton.IconPadding1"))));
            this.removeScriptButton.Name = "removeScriptButton";
            this.toolTip.SetToolTip(this.removeScriptButton, resources.GetString("removeScriptButton.ToolTip"));
            this.removeScriptButton.Click += new System.EventHandler(this.removeScriptButton_Click);
            // 
            // schemaTreeView2
            // 
            resources.ApplyResources(this.schemaTreeView2, "schemaTreeView2");
            this.errorProvider.SetError(this.schemaTreeView2, resources.GetString("schemaTreeView2.Error"));
            this.infoProvider.SetError(this.schemaTreeView2, resources.GetString("schemaTreeView2.Error1"));
            this.schemaTreeView2.HideSelection = false;
            this.errorProvider.SetIconAlignment(this.schemaTreeView2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaTreeView2.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.schemaTreeView2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("schemaTreeView2.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.schemaTreeView2, ((int)(resources.GetObject("schemaTreeView2.IconPadding"))));
            this.infoProvider.SetIconPadding(this.schemaTreeView2, ((int)(resources.GetObject("schemaTreeView2.IconPadding1"))));
            this.schemaTreeView2.ImageList = this.treeImageList;
            this.schemaTreeView2.ItemHeight = 16;
            this.schemaTreeView2.Name = "schemaTreeView2";
            this.toolTip.SetToolTip(this.schemaTreeView2, resources.GetString("schemaTreeView2.ToolTip"));
            this.schemaTreeView2.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.schemaTreeView2_AfterSelect);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.infoProvider.SetError(this.label8, resources.GetString("label8.Error"));
            this.errorProvider.SetError(this.label8, resources.GetString("label8.Error1"));
            this.infoProvider.SetIconAlignment(this.label8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label8.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label8.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.label8, ((int)(resources.GetObject("label8.IconPadding"))));
            this.infoProvider.SetIconPadding(this.label8, ((int)(resources.GetObject("label8.IconPadding1"))));
            this.label8.Name = "label8";
            this.toolTip.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // infoProvider
            // 
            this.infoProvider.ContainerControl = this;
            resources.ApplyResources(this.infoProvider, "infoProvider");
            // 
            // CreateXMLSchemaViewDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.prevStepButton);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.schemaViewTabControl);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.nextStepButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CreateXMLSchemaViewDialog";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.CreateDataViewDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.classesGroupBox.ResumeLayout(false);
            this.classesGroupBox.PerformLayout();
            this.schemaViewTabControl.ResumeLayout(false);
            this.basicInfoTabPage.ResumeLayout(false);
            this.basicInfoTabPage.PerformLayout();
            this.classesTabPage.ResumeLayout(false);
            this.classesTabPage.PerformLayout();
            this.attributesTabPage.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ImportScripts.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		private void dataViewNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null and has to be unique
			if (this.schemaViewNameTextBox.Text.Length == 0)
			{
                this.errorProvider.SetError(this.schemaViewNameTextBox, MessageResourceManager.GetString("SchemaEditor.EnterName"));
				this.infoProvider.SetError(this.schemaViewNameTextBox, null);
				e.Cancel = true;
			}
			else if (!ValidateNameUniqueness(this.schemaViewNameTextBox.Text))
			{
				this.infoProvider.SetError(this.schemaViewNameTextBox, null);
				e.Cancel = true;
			}
			else
			{
				// data binding did not work properly for name and caption
				// box, so we set the values manually
				_xmlSchemaModel.Name = this.schemaViewNameTextBox.Text;
				_xmlSchemaModel.Caption = this.schemaViewCaptionTextBox.Text;
                _xmlSchemaModel.PostProcessor = this.postProcessorTextBox.Text;

				string tip = this.toolTip.GetToolTip((Control) sender);
				// show the info when there is text in text box
				this.errorProvider.SetError((Control) sender, null);
				this.infoProvider.SetError((Control) sender, tip);
			}
		}

		private void dataViewCaptionTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the caption cannot be null
			if (this.schemaViewCaptionTextBox.Text.Length == 0)
			{
				e.Cancel = true;

                this.errorProvider.SetError(this.schemaViewCaptionTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidCaption"));
			}		
		}

		private bool ValidateNameUniqueness(string name)
		{
			bool status = true;

			if (_metaData.XMLSchemaViews[name] != null)
			{
				status = false;
				//this.errorProvider.SetError(this.schemaViewNameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateDataViewName"));
			}

			return status;
		}

        private void ShowXMLSchemaClasses(TreeView treeView)
        {
            if (_xmlSchemaModel.RootElement != null)
            {
                XMLSchemaTreeBuilder builder = new XMLSchemaTreeBuilder(_metaData, _xmlSchemaModel);
                builder.IsAttributesShown = false;

                _schemaRootNode = (MetaDataTreeNode)builder.BuildTree();

                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                treeView.Nodes.Add(_schemaRootNode);

                // expand the root node
                if (treeView.Nodes.Count > 0)
                {
                    treeView.ExpandAll();
                }

                treeView.EndUpdate();

                treeView.SelectedNode = _schemaRootNode;
            }		
        }

        /// <summary>
        /// Create a XMLSchemaComplexType object based on a ClassElememnt object
        /// </summary>
        private XMLSchemaComplexType CreateXMLSchemaComplexType(ClassElement classElement)
        {
            XMLSchemaComplexType complexType = new XMLSchemaComplexType(classElement.Name);
            complexType.Caption = classElement.Caption;

            AddSimpleElements(complexType, classElement);

            return complexType;
        }

        /// <summary>
        /// Create a XMLSchemaComplexType object representing the xml schema model
        /// </summary>
        private XMLSchemaComplexType CreateXMLSchemaComplexType(XMLSchemaModel xmlSchemaModel)
        {
            XMLSchemaComplexType complexType = new XMLSchemaComplexType(xmlSchemaModel.Name);
            complexType.Caption = xmlSchemaModel.Caption;

            return complexType;
        }

        private void AddSimpleElements(XMLSchemaComplexType complexType, ClassElement classElement)
        {
            // display order of attributes of class hierarchy from top-down order
            ClassElement currentClassElement = classElement;
            XMLSchemaElement xmlSchemaElement;

            while (currentClassElement != null)
            {
                int index = 0;
                foreach (SimpleAttributeElement att in currentClassElement.SimpleAttributes)
                {
                    if (att.Usage == DefaultViewUsage.Included)
                    {
                        xmlSchemaElement = new XMLSchemaElement(att.Name);
                        xmlSchemaElement.Caption = att.Caption;
                        xmlSchemaElement.ElementType = DataTypeConverter.ConvertToXMLSchemaSimpleType(att.DataType);

                        complexType.Elements.Insert(index, xmlSchemaElement);
                        index++;
                    }
                }

                foreach (ArrayAttributeElement att in currentClassElement.ArrayAttributes)
                {
                        xmlSchemaElement = new XMLSchemaElement(att.Name);
                        xmlSchemaElement.Caption = att.Caption;
                        xmlSchemaElement.ElementType = DataTypeConverter.ConvertToXMLSchemaSimpleType(att.DataType);

                        complexType.Elements.Insert(index, xmlSchemaElement);
                        index++;
                }

                foreach (VirtualAttributeElement att in currentClassElement.VirtualAttributes)
                {
                    if (att.Usage == DefaultViewUsage.Included)
                    {
                        xmlSchemaElement = new XMLSchemaElement(att.Name);
                        xmlSchemaElement.Caption = att.Caption;
                        xmlSchemaElement.ElementType = DataTypeConverter.ConvertToXMLSchemaSimpleType(att.DataType);

                        complexType.Elements.Insert(index, xmlSchemaElement);
                        index++;
                    }
                }

                foreach (ImageAttributeElement att in currentClassElement.ImageAttributes)
                {
                    if (att.Usage == DefaultViewUsage.Included)
                    {
                        xmlSchemaElement = new XMLSchemaElement(att.Name);
                        xmlSchemaElement.Caption = att.Caption;
                        xmlSchemaElement.ElementType = DataTypeConverter.ConvertToXMLSchemaSimpleType(att.DataType);

                        complexType.Elements.Insert(index, xmlSchemaElement);
                        index++;
                    }
                }

                currentClassElement = currentClassElement.ParentClass;
            }
        }

        private bool IsChildClassExist(string className)
        {
            bool status = false;

            foreach (XMLSchemaComplexType complexType in _xmlSchemaModel.ComplexTypes)
            {
                if (IsParentOf(className, complexType.Name))
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        private bool IsParentClassExist(string className)
        {
            bool status = false;

            foreach (XMLSchemaComplexType complexType in _xmlSchemaModel.ComplexTypes)
            {
                if (IsParentOf(complexType.Name, className))
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        private bool IsParentOf(string parent, string child)
        {
            bool status = false;

            ClassElement childClassElement = this._metaData.SchemaModel.FindClass(child);
            if (childClassElement.FindParentClass(parent) != null)
            {
                status = true;
            }

            return status;
        }

        private void ShowComplextTypeElements(XMLSchemaComplexType complexType)
        {
            this.resultsListView.SuspendLayout();
            this.resultsListView.Items.Clear();

            XMLSchemaElementListViewItem elementItem;
            foreach (XMLSchemaElement xmlSchemaElement in complexType.Elements)
            {
                // show only elements with simple type
                if (_xmlSchemaModel.ComplexTypes[xmlSchemaElement.Caption] == null)
                {
                    elementItem = new XMLSchemaElementListViewItem(xmlSchemaElement.Caption, xmlSchemaElement);
                    elementItem.ImageIndex = 1;
                    elementItem.SubItems.Add(xmlSchemaElement.Name);

                    this.resultsListView.Items.Add(elementItem);
                }
            }

            this.resultsListView.ResumeLayout();

            this.addResultsButton.Enabled = true;
            this.refreshButton.Enabled = true;
        }

        private void ShowComplexTypeDataSourceSettings(XMLSchemaElement xmlSchemaElement)
        {
            this.importScriptListView.SuspendLayout();
            this.importScriptListView.Items.Clear();

            ListViewItem listViewItem;

            if (xmlSchemaElement.ImportScriptNames != null)
            {
                string tartgetClassName = xmlSchemaElement.ElementType;

                MappingPackageCollection mappingPackages = _metaData.MappingManager.GetMappingPackagesByClass(DataSourceType.Unknown, tartgetClassName);
                MappingPackage mappingPackage;

                foreach (string importScriptName in xmlSchemaElement.ImportScriptNames)
                {
                    // show the import script
                    mappingPackage = GetMappingPackage(mappingPackages, importScriptName);
                    listViewItem = new ListViewItem(importScriptName);
                    listViewItem.ImageIndex = 1;
                    if (mappingPackage != null)
                    {
                        listViewItem.SubItems.Add(mappingPackage.DataSourceType.ToString());
                    }

                    this.importScriptListView.Items.Add(listViewItem);
                }
            }

            this.importScriptListView.ResumeLayout();

            // set other seeting
            switch (xmlSchemaElement.DataSourceOption)
            {
                case XMLElementDataSourceOption.Database:
                    this.dbOnlyRadioButton.Checked = true;
                    break;

                case XMLElementDataSourceOption.File:
                    this.fileOnlyRadioButton.Checked = true;
                    break;

                case XMLElementDataSourceOption.DatabaseAndFile:
                    this.dbAndFilesRadioButton.Checked = true;
                    break;
            }

            if (xmlSchemaElement.IsMergingInstances)
            {
                this.mergingCheckBox.Checked = true;
            }
            else
            {
                this.mergingCheckBox.Checked = false;
            }

            this.matchAttributeTextBox.Text = xmlSchemaElement.InstanceIdentifyAttribute;

            this.conditionTextBox.Text = xmlSchemaElement.FilterCondition;

            this.validateConditionTextBox.Text = xmlSchemaElement.ValidateCondition;

            this.addScriptButton.Enabled = true;
            this.removeScriptButton.Enabled = false;
        }

        private void SaveComplexTypeDataSourceSettings(XMLSchemaElement xmlSchemaElement)
        {
            if (this.dbOnlyRadioButton.Checked)
            {
                xmlSchemaElement.DataSourceOption = XMLElementDataSourceOption.Database;
            }
            else if (this.fileOnlyRadioButton.Checked)
            {
                xmlSchemaElement.DataSourceOption = XMLElementDataSourceOption.File;
            }
            else if (this.dbAndFilesRadioButton.Checked)
            {
                xmlSchemaElement.DataSourceOption = XMLElementDataSourceOption.DatabaseAndFile;
            }

            if (this.mergingCheckBox.Checked)
            {
                xmlSchemaElement.IsMergingInstances = true;
            }
            else
            {
                xmlSchemaElement.IsMergingInstances = false;
            }

            if (!string.IsNullOrEmpty(this.matchAttributeTextBox.Text))
            {
                xmlSchemaElement.InstanceIdentifyAttribute = this.matchAttributeTextBox.Text.Trim();
            }
            else
            {
                xmlSchemaElement.InstanceIdentifyAttribute = null;
            }

            if (!string.IsNullOrEmpty(this.conditionTextBox.Text))
            {
                xmlSchemaElement.FilterCondition = this.conditionTextBox.Text.Trim();
            }
            else
            {
                xmlSchemaElement.FilterCondition = null;
            }

            if (!string.IsNullOrEmpty(this.validateConditionTextBox.Text))
            {
                xmlSchemaElement.ValidateCondition = this.validateConditionTextBox.Text.Trim();
            }
            else
            {
                xmlSchemaElement.ValidateCondition = null;
            }
        }

        private MappingPackage GetMappingPackage(MappingPackageCollection mappingPackages, string scriptName)
        {
            MappingPackage mappingPackage = null;

            foreach (MappingPackage mp in mappingPackages)
            {
                if (mp.Name == scriptName)
                {
                    mappingPackage = mp;
                    break;
                }
            }

            return mappingPackage;
        }

        private void ShowAttributeSpecs(XMLSchemaElement xmlSchemaElement)
        {
            if (xmlSchemaElement != null)
            {
                this.attributeNameLabel.Text = xmlSchemaElement.Caption;

                if (xmlSchemaElement.IsSortEnabled)
                {
                    this.sortEnabledComboBox.SelectedIndex = 1; // true
                }
                else
                {
                    this.sortEnabledComboBox.SelectedIndex = 0; // false
                }
                this.sortOrderTextBox.Text = xmlSchemaElement.SortOrder.ToString();

                if (xmlSchemaElement.IsSortAscending)
                {
                    this.sortDirectionComboBox.SelectedIndex = 0; // Ascending
                }
                else
                {
                    this.sortDirectionComboBox.SelectedIndex = 1; // Descending
                }

                if (xmlSchemaElement.IsXAxis)
                {
                    this.xAxisCheckBox.Checked = true;
                }
                else
                {
                    this.xAxisCheckBox.Checked = false;
                }

                if (xmlSchemaElement.IsCategoryAxis)
                {
                    this.categoryAxisCheckBox.Checked = true;
                }
                else
                {
                    this.categoryAxisCheckBox.Checked = false;
                }

                if (xmlSchemaElement.MLCategory == MachineLearningCategory.Feature)
                {
                    this.featureRadioButton.Checked = true;
                    this.labelRadioButton.Checked = false;
                }
                else if (xmlSchemaElement.MLCategory == MachineLearningCategory.Label)
                {
                    this.featureRadioButton.Checked = false;
                    this.labelRadioButton.Checked = true;
                }
            }
            else
            {
                this.attributeNameLabel.Text = "";
                this.sortEnabledComboBox.SelectedIndex = 0; // false
                this.sortOrderTextBox.Text = "0";
                this.sortDirectionComboBox.SelectedIndex = 0; // Ascending
                this.xAxisCheckBox.Checked = false;
                this.featureRadioButton.Checked = false;
                this.labelRadioButton.Checked = false;
            }
        }

        private void SaveAttributeSpecs(XMLSchemaElement xmlSchemaElement)
        {
            if (xmlSchemaElement != null)
            {
                if (this.sortEnabledComboBox.SelectedIndex == 1)
                {
                    xmlSchemaElement.IsSortEnabled = true;
                }
                else
                {
                    xmlSchemaElement.IsSortEnabled = false;
                }

                try
                {
                    xmlSchemaElement.SortOrder = int.Parse(this.sortOrderTextBox.Text);
                }
                catch (Exception)
                {
                    xmlSchemaElement.SortOrder = 0;
                }

                if (this.sortDirectionComboBox.SelectedIndex == 0)
                {
                    xmlSchemaElement.IsSortAscending = true;
                }
                else
                {
                    xmlSchemaElement.IsSortAscending = false;
                }

                if (this.xAxisCheckBox.Checked)
                {
                    xmlSchemaElement.IsXAxis = true;
                }
                else
                {
                    xmlSchemaElement.IsXAxis = false;
                }

                if (this.categoryAxisCheckBox.Checked)
                {
                    xmlSchemaElement.IsCategoryAxis = true;
                }
                else
                {
                    xmlSchemaElement.IsCategoryAxis = false;
                }

                if (this.featureRadioButton.Checked)
                {
                    xmlSchemaElement.MLCategory = MachineLearningCategory.Feature;
                }
                else if (this.labelRadioButton.Checked)
                {
                    xmlSchemaElement.MLCategory = MachineLearningCategory.Label;
                }
                else
                {
                    xmlSchemaElement.MLCategory = MachineLearningCategory.None;
                }
            }
        }

		#endregion

		private void CreateDataViewDialog_Load(object sender, System.EventArgs e)
		{
			if (_xmlSchemaModel != null)
			{
				// set the data bindings
				this.schemaViewNameTextBox.DataBindings.Add("Text", _xmlSchemaModel, "Name");
				this.schemaViewCaptionTextBox.DataBindings.Add("Text", _xmlSchemaModel, "Caption");
				this.schemaViewDescriptionTextBox.DataBindings.Add("Text", _xmlSchemaModel, "Description");
                this.postProcessorTextBox.DataBindings.Add("Text", _xmlSchemaModel, "PostProcessor");
                if (_xmlSchemaModel.RootElement != null)
                {
                    this.multipleOccurCheckBox.Enabled = true;

                    if (_xmlSchemaModel.RootElement.MaxOccurs == "unbounded")
                    {
                        this.multipleOccurCheckBox.Checked = true;
                    }
                    else
                    {
                        this.multipleOccurCheckBox.Checked = false;
                    }
                }

				// if the root elememnt exist, then this is an existing schema model
				// do not allow change the name of the schema model
				if (_xmlSchemaModel.RootElement != null)
				{
					this.schemaViewNameTextBox.ReadOnly = true;
					this.schemaViewNameTextBox.Validating -= new System.ComponentModel.CancelEventHandler(this.dataViewNameTextBox_Validating);
				}

				// display help providers to some text boxes
				string tip = toolTip.GetToolTip(this.schemaViewNameTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.schemaViewNameTextBox, tip);
				}

				tip = toolTip.GetToolTip(this.schemaViewCaptionTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.schemaViewCaptionTextBox, tip);
				}

				tip = toolTip.GetToolTip(this.baseClassTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.baseClassTextBox, tip);
				}

				if (_xmlSchemaModel.RootElement != null)
				{
					this.baseClassTextBox.Text = _xmlSchemaModel.RootElement.Caption;
					this.nextStepButton.Enabled = true;
				}
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int tabIndex = this.schemaViewTabControl.SelectedIndex;

			// do not allow switching to new tab if the root element isn't exist
			if (_xmlSchemaModel.RootElement != null)
			{
				if (tabIndex == 0)
				{
					this.prevStepButton.Enabled = false;
					this.nextStepButton.Enabled = true;
                    this.doneButton.Enabled = false;
				}
				else if (tabIndex == 1)
				{
					this.prevStepButton.Enabled = true;
					this.nextStepButton.Enabled = true;
                    this.doneButton.Enabled = true;

                    // it is a tab for xml schema classes
                    ShowXMLSchemaClasses(this.schemaTreeView);
				}
                else if (tabIndex == 2)
                {
                    this.prevStepButton.Enabled = true;
                    this.nextStepButton.Enabled = true;
                    this.doneButton.Enabled = true;

                    // it is a tab for xml schema attributes
                    ShowXMLSchemaClasses(this.schemaTreeView1);
                }
                else
                {
                    // it is a tab for import scripts
                    ShowXMLSchemaClasses(this.schemaTreeView2);

                    _currentSchemaTreeNode = null;

                    this.prevStepButton.Enabled = true;
                    this.nextStepButton.Enabled = false;
                    this.doneButton.Enabled = true;
                }
			}
			else if (tabIndex > 0)
			{
				// stay in the first tab page
				this.schemaViewTabControl.SelectedIndex = 0;
			}
		}

		private void nextStepButton_Click(object sender, System.EventArgs e)
		{
			int tabIndex = this.schemaViewTabControl.SelectedIndex;
			if (tabIndex < 3)
			{
				this.schemaViewTabControl.SelectedIndex = tabIndex + 1;

                if (this.schemaViewTabControl.SelectedIndex == 1)
                {
                    // it is a tab for xml schema classes
                    ShowXMLSchemaClasses(this.schemaTreeView);
                }
                else if (this.schemaViewTabControl.SelectedIndex == 2)
                {
                    // it is a tab for xml schema attributes
                    ShowXMLSchemaClasses(this.schemaTreeView1);
                }
                else if (this.schemaViewTabControl.SelectedIndex == 3)
                {
                    if (_currentSelectedAttribute != null)
                    {
                        SaveAttributeSpecs(_currentSelectedAttribute);
                    }

                    // it is a tab for import scripts
                    ShowXMLSchemaClasses(this.schemaTreeView2);
                }
			}
		}

		private void schemaViewNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// keep the caption text in sync with the name
			this.schemaViewCaptionTextBox.Text = this.schemaViewNameTextBox.Text;
		}

		private void prevStepButton_Click(object sender, System.EventArgs e)
		{
			int tabIndex = this.schemaViewTabControl.SelectedIndex;
			if (tabIndex > 0)
			{
				this.schemaViewTabControl.SelectedIndex = tabIndex - 1;

                if (this.schemaViewTabControl.SelectedIndex == 1)
                {
                    if (_currentSelectedAttribute != null)
                    {
                        SaveAttributeSpecs(_currentSelectedAttribute);
                    }

                    // it is a tab for xml schema classes
                    ShowXMLSchemaClasses(this.schemaTreeView);
                }
			}
		}

		private void doneButton_Click(object sender, System.EventArgs e)
		{
            int tabIndex = this.schemaViewTabControl.SelectedIndex;
            if (tabIndex == 2)
            {
                // class attributes tab, save the current sort attribute info
                if (_currentSelectedAttribute != null)
                {
                    SaveAttributeSpecs(_currentSelectedAttribute);
                }
            }
            else if (tabIndex == 3)
            {
                if (_currentSchemaTreeNode != null &&
                    _currentSchemaTreeNode.MetaDataElement is XMLSchemaElement)
                {
                    XMLSchemaElement theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;
                    // save the xml schema element settings
                    SaveComplexTypeDataSourceSettings(theElement);
                }
            }
		}

		private void baseClassButton_Click(object sender, System.EventArgs e)
		{
			ChooseClassDialog dialog = new ChooseClassDialog();
			dialog.RootClass = "ALL";
			dialog.MetaData = _metaData;
			if (_xmlSchemaModel.RootElement != null)
			{
				//dialog.SelectedClass = (ClassElement) _xmlSchemaModel.RootElement.ElementType;
			}

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				ClassElement baseClassElement = dialog.SelectedClass;

                XMLSchemaElement rootElement = new XMLSchemaElement(baseClassElement.Name);
                rootElement.Caption = baseClassElement.Caption;
                rootElement.ElementType = baseClassElement.Name;
                rootElement.ParentNode = _xmlSchemaModel;

				if (_xmlSchemaModel.RootElement == null ||
                    _xmlSchemaModel.RootElement.Name != rootElement.Name)
				{
					// clear old info for a new base class
					_xmlSchemaModel.ComplexTypes.Clear();

                    _xmlSchemaModel.RootElement = rootElement;

                    // add a complex type referenced by the root element to the schema
                    _xmlSchemaModel.ComplexTypes.Add(CreateXMLSchemaComplexType(baseClassElement));

                    this.baseClassTextBox.Text = rootElement.Caption;

					this.nextStepButton.Enabled = true;

                    this.multipleOccurCheckBox.Enabled = true;
				}
			}
		}

        private void schemaTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get the selected node
            _currentSchemaTreeNode = (MetaDataTreeNode)e.Node;

            XMLSchemaElement theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;

            if (theElement == _xmlSchemaModel.RootElement)
            {
                this.removeClassButton.Enabled = false;
                this.renameButton.Enabled = false;
            }
            else
            {
                this.removeClassButton.Enabled = true;
                this.renameButton.Enabled = true;
            }

            if (theElement != null)
            {
                // display the related classes of the selected class represented by the element
                ClassElement classElement = _metaData.SchemaModel.FindClass(theElement.ElementType);
                ClassElement relatedClassElement;
                if (classElement != null)
                {
                    DataViewModel dataView = _metaData.GetDefaultDataView(classElement.Name);

                    _currentComplexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[theElement.Caption];

                    this.linkedClassesListView.SuspendLayout();
                    this.linkedClassesListView.Items.Clear();

                    foreach (DataClass relatedClass in dataView.BaseClass.RelatedClasses)
                    {
                        relatedClassElement = _metaData.SchemaModel.FindClass(relatedClass.ClassName);
                        LinkedClassListViewItem item1 = new LinkedClassListViewItem(relatedClassElement.Caption, relatedClassElement, null, relatedClass.ReferringRelationship);
                        item1.ImageIndex = 0;
                        item1.SubItems.Add(relatedClass.ReferringRelationship.Caption);
                        item1.SubItems.Add(Enum.GetName(typeof(RelationshipType), relatedClass.ReferringRelationship.Type));

                        this.linkedClassesListView.Items.Add(item1);
                    }

                    this.linkedClassesListView.ResumeLayout();
                }
                else
                {
                    throw new Exception("Unable to find a class with name " + theElement.ElementType);
                }
            }
        }

        private void linkedClassesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.linkedClassesListView.SelectedItems.Count == 1)
            {
                this.addButton.Enabled = true;

                // check to see if the linked class has leaf classes
                LinkedClassListViewItem listViewItem = (LinkedClassListViewItem)this.linkedClassesListView.SelectedItems[0];
                ClassElement linkedClass = listViewItem.LinkedClassElement;
                if (!linkedClass.IsLeaf)
                {
                    this.showLeafClassButton.Enabled = true;
                }
                else
                {
                    this.showLeafClassButton.Enabled = false;
                }
            }	
        }

        private void showSubClassesButton_Click(object sender, EventArgs e)
        {
            if (this.linkedClassesListView.SelectedItems.Count == 1)
            {
                LinkedClassListViewItem listViewItem = (LinkedClassListViewItem)this.linkedClassesListView.SelectedItems[0];
                RelationshipAttributeElement relationship = listViewItem.RelationshipAttribute;

                ChooseClassDialog dialog = new ChooseClassDialog();
                dialog.MetaData = _metaData;
                dialog.RootClass = listViewItem.LinkedClassElement.Name;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ClassElement selectedClassElement = dialog.SelectedClass;

                    string selectedClassName = selectedClassElement.Name;
                    string selectedClassCaption = selectedClassElement.Caption;

                    // check to see if the class has been added as a complex type
                    if (_xmlSchemaModel.ComplexTypes[selectedClassCaption] == null)
                    {
                        if (!IsParentClassExist(selectedClassName))
                        {
                            // create xml schema element and a complex type and add it to the xml schema
                            XMLSchemaElement xmlSchemaElement = new XMLSchemaElement(selectedClassElement.Name);
                            xmlSchemaElement.Caption = selectedClassElement.Caption;
                            xmlSchemaElement.ElementType = selectedClassElement.Name;
                            if (relationship.Type == RelationshipType.ManyToOne)
                            {
                                // master type to the selected type has many-to-one relationship
                                xmlSchemaElement.MaxOccurs = "1";
                            }
                            else
                            {
                                // master type to the selected type has one-to-many relationship
                                xmlSchemaElement.MaxOccurs = "unbounded";
                            }

                            _currentComplexType.Elements.Add(xmlSchemaElement);

                            // add a complex type referenced by the newly added element to the schema
                            _xmlSchemaModel.ComplexTypes.Add(CreateXMLSchemaComplexType(selectedClassElement));

                            // refresh the xml schema tree
                            ShowXMLSchemaClasses(this.schemaTreeView);
                        }
                        else
                        {
                            MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.ParentClassExistInXMLSchema"), "Notify Dialog",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.TheClassExistInXMLSchema"), "Notify Dialog",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (this.linkedClassesListView.SelectedItems.Count == 1)
            {
                LinkedClassListViewItem listViewItem = (LinkedClassListViewItem)this.linkedClassesListView.SelectedItems[0];
                ClassElement classElement = listViewItem.LinkedClassElement;
                RelationshipAttributeElement relationship = listViewItem.RelationshipAttribute;

                // check to see if the class has been added as a complex type
                if (_xmlSchemaModel.ComplexTypes[classElement.Caption] == null)
                {
                    if (!IsChildClassExist(classElement.Name))
                    {
                        // create xml schema element and a complex type and add it to the xml schema
                        XMLSchemaElement xmlSchemaElement = new XMLSchemaElement(classElement.Name);
                        xmlSchemaElement.Caption = classElement.Caption;
                        xmlSchemaElement.ElementType = classElement.Name;
                        if (relationship.Type == RelationshipType.ManyToOne)
                        {
                            // master type to the selected type has many-to-one relationship
                            xmlSchemaElement.MaxOccurs = "1";
                        }
                        else
                        {
                            // master type to the selected type has one-to-many relationship
                            xmlSchemaElement.MaxOccurs = "unbounded";
                        }

                        _currentComplexType.Elements.Add(xmlSchemaElement);

                        // add a complex type referenced by the newly added element to the schema
                        _xmlSchemaModel.ComplexTypes.Add(CreateXMLSchemaComplexType(classElement));

                        // refresh the xml schema tree
                        ShowXMLSchemaClasses(this.schemaTreeView);
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.ChildLeafClassExistInXMLSchema"), "Notify Dialog",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.TheClassExistInXMLSchema"), "Notify Dialog",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void removeClassButton_Click(object sender, EventArgs e)
        {
            if (_currentSchemaTreeNode != null && _currentComplexType != null)
            {
                XMLSchemaElement theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;

                XMLSchemaElementCollection parentNode = theElement.ParentNode as XMLSchemaElementCollection;
                if (parentNode != null)
                {
                    parentNode.Remove(theElement);
                    _xmlSchemaModel.ComplexTypes.Remove(_currentComplexType);
                }

                // refresh the existing class list view
                ShowXMLSchemaClasses(this.schemaTreeView);

                this.schemaTreeView.SelectedNode = _schemaRootNode;
            }
        }

        private void schemaTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get the selected node
            _currentSchemaTreeNode = (MetaDataTreeNode)e.Node;

            XMLSchemaElement theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;

            if (theElement != null)
            {
                // display the attributes of the selected class represented by the element
                ClassElement classElement = _metaData.SchemaModel.FindClass(theElement.ElementType);
                if (classElement != null)
                {
                    _currentComplexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[theElement.Caption];

                    ShowComplextTypeElements(_currentComplexType);
                }
                else
                {
                    throw new Exception("Unable to find a class with name " + theElement.ElementType);
                }
            }
        }

        private void addResultsButton_Click(object sender, EventArgs e)
        {
            ClassElement classElement = _metaData.SchemaModel.FindClass(_currentComplexType.Name);
            if (classElement != null)
            {
                SelectClassAttributeDialog dialog = new SelectClassAttributeDialog(_metaData, classElement);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.SelectedAttributeName != null)
                    {
                        XMLSchemaElement xmlSchemaElement = new XMLSchemaElement(dialog.SelectedAttributeName);
                        xmlSchemaElement.Caption = dialog.SelectedAttributeCaption;
                        xmlSchemaElement.ElementType = DataTypeConverter.ConvertToXMLSchemaSimpleType(dialog.SelectedAttributeDataType);

                        this._currentComplexType.Elements.Add(xmlSchemaElement);

                        ShowComplextTypeElements(_currentComplexType);
                    }
                }
            }
        }

        private void removeResultButton_Click(object sender, EventArgs e)
        {
            if (this.resultsListView.SelectedItems.Count == 1)
            {
                XMLSchemaElementListViewItem item = (XMLSchemaElementListViewItem)this.resultsListView.SelectedItems[0];

                _currentComplexType.Elements.Remove(item.TheElememnt);

                this.resultsListView.Items.Remove(item);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (_currentComplexType != null)
            {
                // remove simple type elements from the complex type
                XMLSchemaElementCollection removedElements = new XMLSchemaElementCollection();
                foreach (XMLSchemaElement xmlSchemaElement in _currentComplexType.Elements)
                {
                    if (_xmlSchemaModel.ComplexTypes[xmlSchemaElement.Caption] == null)
                    {
                        removedElements.Add(xmlSchemaElement);
                    }
                }

                foreach (XMLSchemaElement removedElement in removedElements)
                {
                    _currentComplexType.Elements.Remove(removedElement);
                }

                ClassElement classElement = _metaData.SchemaModel.FindClass(_currentComplexType.Name);

                AddSimpleElements(_currentComplexType, classElement);

                this.resultsListView.SuspendLayout();
                this.resultsListView.Items.Clear();

                XMLSchemaElementListViewItem elementItem;
                foreach (XMLSchemaElement xmlSchemaElement in _currentComplexType.Elements)
                {
                    // show only elements with simple type
                    if (_xmlSchemaModel.ComplexTypes[xmlSchemaElement.Caption] == null)
                    {
                        elementItem = new XMLSchemaElementListViewItem(xmlSchemaElement.Caption, xmlSchemaElement);
                        elementItem.ImageIndex = 1;
                        elementItem.SubItems.Add(xmlSchemaElement.Name);

                        this.resultsListView.Items.Add(elementItem);
                    }
                }

                this.resultsListView.ResumeLayout();

                this.addResultsButton.Enabled = true;
                this.refreshButton.Enabled = true;
            }
        }

        private void resultsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // save the current sort setting before switching
            if (_currentSelectedAttribute != null)
            {
                SaveAttributeSpecs(_currentSelectedAttribute);
            }

            if (resultsListView.SelectedIndices.Count > 0)
            {
                XMLSchemaElementListViewItem xmlSchemaElementListViewItem = (XMLSchemaElementListViewItem)resultsListView.Items[resultsListView.SelectedIndices[0]];

                XMLSchemaElement xmlSchemaElement = xmlSchemaElementListViewItem.TheElememnt;

                _currentSelectedAttribute = xmlSchemaElement;

                ShowAttributeSpecs(xmlSchemaElement);

                this.removeResultButton.Enabled = true;

                this.renameResult.Enabled = true;
            }
            else
            {
                _currentSelectedAttribute = null;

                ShowAttributeSpecs(null);

                this.removeResultButton.Enabled = false;
            }

        }

        private void schemaTreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            XMLSchemaElement theElement;

            if (_currentSchemaTreeNode != null &&
                _currentSchemaTreeNode.MetaDataElement is XMLSchemaElement)
            {
                theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;

                //SaveComplexTypeDataSourceSettings(theElement);
            }

            // Get the selected node
            _currentSchemaTreeNode = (MetaDataTreeNode)e.Node;

            theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;

            if (theElement != null)
            {
                // display the attributes of the selected class represented by the element
                ClassElement classElement = _metaData.SchemaModel.FindClass(theElement.ElementType);
                if (classElement != null)
                {
                    ShowComplexTypeDataSourceSettings(theElement);
                }
                else
                {
                    throw new Exception("Unable to find a class with name " + theElement.ElementType);
                }
            }
        }

        private void addScriptButton_Click(object sender, EventArgs e)
        {
            XMLSchemaElement theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;

            if (theElement != null)
            {
                string tartgetClassName = theElement.ElementType;

                MappingPackageCollection mappingPackages = _metaData.MappingManager.GetMappingPackagesByClass(DataSourceType.Unknown, tartgetClassName);

                SelectMappingPackageDialog dialog = new SelectMappingPackageDialog(mappingPackages);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.SelectedMappingPackage != null)
                    {
                        bool exist = false;
                        if (theElement.ImportScriptNames != null)
                        {
                            foreach (string scriptName in theElement.ImportScriptNames)
                            {
                                if (scriptName == dialog.SelectedMappingPackage.Name)
                                {
                                    exist = true;
                                    break;
                                }
                            }

                            if (!exist)
                            {
                                theElement.ImportScriptNames.Add(dialog.SelectedMappingPackage.Name);
                            }
                        }
                        else
                        {
                            StringCollection scriptNames = new StringCollection();
                            scriptNames.Add(dialog.SelectedMappingPackage.Name);
                            theElement.ImportScriptNames = scriptNames;
                        }

                        ShowComplexTypeDataSourceSettings(theElement);
                    }
                }
            }
        }

        private void removeScriptButton_Click(object sender, EventArgs e)
        {
            if (this.importScriptListView.SelectedItems.Count == 1)
            {
                ListViewItem item = (ListViewItem)this.importScriptListView.SelectedItems[0];

                XMLSchemaElement theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;

                if (theElement != null)
                {
                    theElement.ImportScriptNames.Remove(item.Text);
                }

                this.importScriptListView.Items.Remove(item);
            }
        }

        private void importScriptListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (importScriptListView.SelectedIndices.Count > 0)
            {
                this.removeScriptButton.Enabled = true;
            }
            else
            {
                this.removeScriptButton.Enabled = false;
            }
        }

        private void multipleOccurCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.multipleOccurCheckBox.Checked)
            {
                _xmlSchemaModel.RootElement.MaxOccurs = "unbounded";

                // add the pseudo-element as a complex type to the schema
                _xmlSchemaModel.ComplexTypes.Add(CreateXMLSchemaComplexType(_xmlSchemaModel));
            }
            else
            {
                _xmlSchemaModel.RootElement.MaxOccurs = "1";
                // remove the root element as a complex type
                if (_xmlSchemaModel.ComplexTypes[_xmlSchemaModel.Caption] != null)
                {
                    _xmlSchemaModel.ComplexTypes.Remove(_xmlSchemaModel.Caption);
                }
            }
        }

        private void renameButton_Click(object sender, EventArgs e)
        {
            if (_currentSchemaTreeNode != null && _currentComplexType != null)
            {
                XMLSchemaElement theElement = _currentSchemaTreeNode.MetaDataElement as XMLSchemaElement;
                XMLSchemaComplexType theComplexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[theElement.Caption];

                RenameXMLSchemaElementDialog dialog = new RenameXMLSchemaElementDialog();
                dialog.ElementName = theElement.Caption;
                dialog.SchemaModel = _xmlSchemaModel;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    theElement.Caption = dialog.ElementName;
                    theComplexType.Caption = dialog.ElementName;

                    // refresh the existing class list view
                    ShowXMLSchemaClasses(this.schemaTreeView);
                }

            }
        }

        private void renameResult_Click(object sender, EventArgs e)
        {
            if (this.resultsListView.SelectedItems.Count == 1)
            {
                XMLSchemaElementListViewItem item = (XMLSchemaElementListViewItem)this.resultsListView.SelectedItems[0];

                RenameXMLSchemaElementDialog dialog = new RenameXMLSchemaElementDialog();
                dialog.ElementName = item.TheElememnt.Caption;
                dialog.SchemaModel = _xmlSchemaModel;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    item.TheElememnt.Caption = dialog.ElementName;

                    // refresh the result list
                    this.resultsListView.SuspendLayout();
                    this.resultsListView.Items.Clear();

                    XMLSchemaElementListViewItem elementItem;
                    foreach (XMLSchemaElement xmlSchemaElement in _currentComplexType.Elements)
                    {
                        // show only elements with simple type
                        if (_xmlSchemaModel.ComplexTypes[xmlSchemaElement.Caption] == null)
                        {
                            elementItem = new XMLSchemaElementListViewItem(xmlSchemaElement.Caption, xmlSchemaElement);
                            elementItem.ImageIndex = 1;
                            elementItem.SubItems.Add(xmlSchemaElement.Name);

                            this.resultsListView.Items.Add(elementItem);
                        }
                    }

                    this.resultsListView.ResumeLayout();

                }
            }
        }

        private void xAxisCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool status = false;

            if (_currentSelectedAttribute != null && xAxisCheckBox.Checked)
            {
                foreach (XMLSchemaComplexType complexType in _xmlSchemaModel.ComplexTypes)
                {
                    foreach (XMLSchemaElement schemaElement in complexType.Elements)
                    {
                        if (schemaElement.IsXAxis &&
                            schemaElement != _currentSelectedAttribute)
                        {
                            status = true;
                            break;
                        }
                    }

                    if (status)
                    {
                        break;
                    }
                }
            }
           
            if (status)
            {
                MessageBox.Show("Only one x axis is allowed for a xml schema.");
                this.xAxisCheckBox.Checked = false;
            }
        }

        private void categoryAxisCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool status = false;

            if (_currentSelectedAttribute != null && this.categoryAxisCheckBox.Checked)
            {
                foreach (XMLSchemaComplexType complexType in _xmlSchemaModel.ComplexTypes)
                {
                    foreach (XMLSchemaElement schemaElement in complexType.Elements)
                    {
                        if (schemaElement.IsCategoryAxis &&
                            schemaElement != _currentSelectedAttribute)
                        {
                            status = true;
                            break;
                        }
                    }

                    if (status)
                    {
                        break;
                    }
                }
            }

            if (status)
            {
                MessageBox.Show("Only one category property is allowed for a xml schema.");
                this.categoryAxisCheckBox.Checked = false;
            }
        }
    }
}

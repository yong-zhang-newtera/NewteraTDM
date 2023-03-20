using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Mappings;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Validate;
using Newtera.ParserGen.Converter;
using Newtera.Conveters;
using Newtera.Common.MetaData.Mappings.Scripts;
using Newtera.ParserGen.ProjectModel;
using Newtera.Common.Config;
using Newtera.DataGridActiveX;
using Newtera.WindowsControl;
using Newtera.WinClientCommon;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// Summary description for ImportWizard.
	/// 
	///		Escape Sequences for Formatting Escape Sequence
	///		
	///		\a		bell (alert)
	///		\b		backspace
	///		\f		form feed
	///		\n		new line/carriage return
	///		\r		carriage return
	///		\t		horizontal tab
	///		\v		vertical tab
	///		\'		single quotation mark
	///		\"		double quotation mark
	///		\\		backslash
	///		\?		literal question mark
	///		\ooo	ASCII character shown in octal notation
	///		\xhh	ASCII character shown in hexadecimal notation
	///		\xhhhh	-UNICODE character shown in hexadecimal notation when this escape sequence is used in a wide-character constant or a UNICODE string literal
	///
	/// </summary>
	public class ImportWizard : System.Windows.Forms.Form, IDataGridControl
	{
		internal const int CHUNK_SIZE = 50;
		internal const int FILE_SIZE_WARNING_THRESHHOLD = 100; // 100 MB
		internal const int BLOCK_SIZE = 1000; // 1000 records per block
        internal const int MAX_INSTANCE_COUNT = 10000; // 10000 instance per data table

		private string[] rowDelimiters = new string[] {"\r\n", "\n", "\r", ";", ",", "\t", "|"};
		private string[] colDelimiters = new string[] {"\t", ",", ";"};

		private CMDataServiceStub _dataService;
		private MetaDataServiceStub _metaDataService;
		private WorkInProgressDialog _workInProgressDialog;
		private MetaDataModel _metaData;
		private MappingPackage _package;
		private DataSourceType _dataSourceType;
		private DataSet[] _sourceDataSets;
		private DataSet[] _destinationDataSets;
		private int _currentDstRowCount;
		private bool _isRequestComplete;
		private ConfigKeyValueCollection _textConverters;
        private ConfigKeyValueCollection _excelConverters;
        private ConfigKeyValueCollection _otherConverters;
		private OpenFileDialog _openFileDialog;
		private ScriptNodeCollection _scriptResults;
		private int _currentClassMappingIndex;
		private int _currentRowIndex;
		private int _currentChunkIndex;
		private AppConfig _config;
		private bool _isDBAUser;
		private int _currentBlock;
		private DesignStudio _designStudio;
		private bool _isImportCancelled;
		private IDataSourceConverter[] _srcConverters;
		private string[] _fileNames;
		private int _currentFileIndex;
        private bool _hasMissingColumns;

		private Newtera.Studio.UserControls.Wizards.WizardWelcomePage wizardWelcomePage1;
		private Newtera.Studio.UserControls.Wizards.WizardPageBase chooseDataSourcePage;
		private Newtera.Studio.UserControls.Wizards.WizardPageBase determineFileFormatPage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox dataSourceComboBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ImageList dataSourceTypeImageList;
		private System.Windows.Forms.Label selectedDataSourceImage;
		private System.Windows.Forms.Label selectedDataSourceText;
		private System.Windows.Forms.Label dataSourceFileNameLabel;
		private System.Windows.Forms.TextBox dataSourceFileNameTextBox;
		private System.Windows.Forms.Button openFileButton;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton delimitedRadioButton;
		private System.Windows.Forms.RadioButton freeFormedRadioButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.DataGrid previewDataGrid;
		private Newtera.Studio.UserControls.Wizards.WizardForm importWizardForm;
		private Newtera.Studio.UserControls.Wizards.WizardPageBase previewDataPage;
		private Newtera.Studio.UserControls.Wizards.WizardPageBase specifyMappingPage;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.CheckBox runImmediatlyCheckBox;
		private System.Windows.Forms.CheckBox saveImportPackageCheckBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox importPackageNameTextBox;
		private Newtera.Studio.UserControls.Wizards.WizardFinalPage wizardFinalPage1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox rowDelimiterComboBox;
		private System.Windows.Forms.TextBox otherColumnDelimiterTextBox;
		private System.Windows.Forms.CheckBox firstRowColumnNameCheckBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton columnDelimiterRadioButton1;
		private System.Windows.Forms.RadioButton columnDelimiterRadioButton2;
		private System.Windows.Forms.RadioButton columnDelimiterRadioButton3;
		private System.Windows.Forms.RadioButton columnDelimiterRadioButton4;
		private System.Windows.Forms.ColumnHeader sourceColumnHeader;
		private System.Windows.Forms.ColumnHeader destinationColumnHeader;
		private System.Windows.Forms.ColumnHeader transformColumnHeader;
        private Newtera.WindowsControl.ListViewEx classMapListView;
		private Newtera.Studio.UserControls.Wizards.WizardPageBase previewPostTransformPage;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox classComboBox;
		private System.Windows.Forms.Button editInstanceButton;
		private System.Windows.Forms.Button validateDataButton;
		private Newtera.Studio.UserControls.Wizards.WizardPageBase runSaveImportPage;
		private System.Windows.Forms.ComboBox textConverterComboBox;
		private System.Windows.Forms.ImageList dataGridImageList;
		private System.Windows.Forms.Button pickConverterButton;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox importPackageDescTextBox;
		private System.Windows.Forms.TextBox choosenImportPackageTextBox;
		private System.Windows.Forms.Button selectImportPackageBtn;
		private System.Windows.Forms.CheckBox includeUpdateCheckBox;
		private System.Windows.Forms.Button addConverterButton;
		private System.Windows.Forms.Button chartPreviewButton;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton dataRangeRadioButton1;
		private System.Windows.Forms.RadioButton dataRangeRadioButton2;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox blockSizeTextBox;
		private System.Windows.Forms.ToolTip toolTip1;
		private Newtera.DataGridActiveX.SelectionDataGrid classDataGrid;
		private System.Windows.Forms.Label textFileNameLabel;
		private System.Windows.Forms.ComboBox previewTextFileComboBox;
		private System.Windows.Forms.Button addEntryButton;
		private System.Windows.Forms.Button viewSourceButton;
        private Button delConveterButton;
        private Newtera.Studio.UserControls.Wizards.WizardPageBase excelFormatPage;
        private RadioButton standardConverterRadioButton;
        private RadioButton specialConveterRadioButton;
        private GroupBox groupBox9;
        private Button delExcelConverterButton;
        private Button AddExcelConverterButton;
        private Button autoPickExcelConverterButton;
        private ComboBox excelConverterComboBox;
        private Label label12;
        private GroupBox groupBox10;
        private TextBox readExcelBlockSizeTextBox;
        private Label label13;
        private Panel panel3;
        private RadioButton readExcelAllRadioButton;
        private RadioButton readExcelInBlockRadioButton;
        private Label label14;
        private Newtera.Studio.UserControls.Wizards.WizardPageBase otherFormatPage;
        private GroupBox groupBox11;
        private TextBox readOtherBlockSizeTextBox;
        private Label label16;
        private Panel panel4;
        private RadioButton readOtherAllRadioButton;
        private RadioButton readOtherInBlockRadioButton;
        private Label label17;
        private GroupBox groupBox8;
        private Button delOtherConverterButton;
        private Button addOtherConverterButton;
        private Button autoPickOtherConverterButton;
        private ComboBox otherConverterComboBox;
        private Label label15;
        private Label label18;
        private TextBox startingDataRowTextBox;
        private CheckBox noValidationCheckBox;
        private GroupBox groupBox12;
        private RadioButton pkRadioButton;
        private RadioButton ukRadioButton;
		private System.ComponentModel.IContainer components;

		public ImportWizard()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_workInProgressDialog = new WorkInProgressDialog();
			_currentDstRowCount = 0;
			_dataService = null;
			_textConverters = null;
            _excelConverters = null;
            _otherConverters = null;
			_openFileDialog = null;
			_scriptResults = null;
			_config = null;
			_isDBAUser = false;
			_currentBlock = 0;
			_designStudio = null;
			_isImportCancelled = false;
			_srcConverters = null;
		}

		~ImportWizard()
		{
			_workInProgressDialog.Dispose();
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
		/// Gets or sets the DesignStudio that launches the ImportWizard
		/// </summary>
		public DesignStudio DesignStudio
		{
			get
			{
				return _designStudio;
			}
			set
			{
				_designStudio = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportWizard));
            Newtera.DataGridActiveX.GridRange gridRange1 = new Newtera.DataGridActiveX.GridRange();
            this.dataSourceTypeImageList = new System.Windows.Forms.ImageList(this.components);
            this.dataGridImageList = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dataRangeRadioButton1 = new System.Windows.Forms.RadioButton();
            this.dataRangeRadioButton2 = new System.Windows.Forms.RadioButton();
            this.delConveterButton = new System.Windows.Forms.Button();
            this.addConverterButton = new System.Windows.Forms.Button();
            this.pickConverterButton = new System.Windows.Forms.Button();
            this.readExcelAllRadioButton = new System.Windows.Forms.RadioButton();
            this.readExcelInBlockRadioButton = new System.Windows.Forms.RadioButton();
            this.delExcelConverterButton = new System.Windows.Forms.Button();
            this.AddExcelConverterButton = new System.Windows.Forms.Button();
            this.autoPickExcelConverterButton = new System.Windows.Forms.Button();
            this.readOtherAllRadioButton = new System.Windows.Forms.RadioButton();
            this.readOtherInBlockRadioButton = new System.Windows.Forms.RadioButton();
            this.delOtherConverterButton = new System.Windows.Forms.Button();
            this.addOtherConverterButton = new System.Windows.Forms.Button();
            this.autoPickOtherConverterButton = new System.Windows.Forms.Button();
            this.previewTextFileComboBox = new System.Windows.Forms.ComboBox();
            this.addEntryButton = new System.Windows.Forms.Button();
            this.noValidationCheckBox = new System.Windows.Forms.CheckBox();
            this.includeUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.importWizardForm = new Newtera.Studio.UserControls.Wizards.WizardForm();
            this.wizardWelcomePage1 = new Newtera.Studio.UserControls.Wizards.WizardWelcomePage();
            this.chooseDataSourcePage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.selectImportPackageBtn = new System.Windows.Forms.Button();
            this.choosenImportPackageTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.openFileButton = new System.Windows.Forms.Button();
            this.dataSourceFileNameTextBox = new System.Windows.Forms.TextBox();
            this.dataSourceFileNameLabel = new System.Windows.Forms.Label();
            this.selectedDataSourceText = new System.Windows.Forms.Label();
            this.selectedDataSourceImage = new System.Windows.Forms.Label();
            this.dataSourceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.determineFileFormatPage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.blockSizeTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textConverterComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.startingDataRowTextBox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.columnDelimiterRadioButton4 = new System.Windows.Forms.RadioButton();
            this.columnDelimiterRadioButton3 = new System.Windows.Forms.RadioButton();
            this.columnDelimiterRadioButton2 = new System.Windows.Forms.RadioButton();
            this.columnDelimiterRadioButton1 = new System.Windows.Forms.RadioButton();
            this.firstRowColumnNameCheckBox = new System.Windows.Forms.CheckBox();
            this.otherColumnDelimiterTextBox = new System.Windows.Forms.TextBox();
            this.rowDelimiterComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.freeFormedRadioButton = new System.Windows.Forms.RadioButton();
            this.delimitedRadioButton = new System.Windows.Forms.RadioButton();
            this.excelFormatPage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.readExcelBlockSizeTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.excelConverterComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.specialConveterRadioButton = new System.Windows.Forms.RadioButton();
            this.standardConverterRadioButton = new System.Windows.Forms.RadioButton();
            this.otherFormatPage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.readOtherBlockSizeTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.otherConverterComboBox = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.previewDataPage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.textFileNameLabel = new System.Windows.Forms.Label();
            this.previewDataGrid = new System.Windows.Forms.DataGrid();
            this.specifyMappingPage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.viewSourceButton = new System.Windows.Forms.Button();
            this.classMapListView = new Newtera.WindowsControl.ListViewEx();
            this.sourceColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.destinationColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.transformColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.previewPostTransformPage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.classDataGrid = new Newtera.DataGridActiveX.SelectionDataGrid();
            this.chartPreviewButton = new System.Windows.Forms.Button();
            this.validateDataButton = new System.Windows.Forms.Button();
            this.editInstanceButton = new System.Windows.Forms.Button();
            this.classComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.runSaveImportPage = new Newtera.Studio.UserControls.Wizards.WizardPageBase();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.ukRadioButton = new System.Windows.Forms.RadioButton();
            this.pkRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.importPackageDescTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.importPackageNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.saveImportPackageCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.runImmediatlyCheckBox = new System.Windows.Forms.CheckBox();
            this.wizardFinalPage1 = new Newtera.Studio.UserControls.Wizards.WizardFinalPage();
            this.chooseDataSourcePage.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.determineFileFormatPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.excelFormatPage.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.otherFormatPage.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.previewDataPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewDataGrid)).BeginInit();
            this.specifyMappingPage.SuspendLayout();
            this.previewPostTransformPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.classDataGrid)).BeginInit();
            this.runSaveImportPage.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataSourceTypeImageList
            // 
            this.dataSourceTypeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dataSourceTypeImageList.ImageStream")));
            this.dataSourceTypeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.dataSourceTypeImageList.Images.SetKeyName(0, "");
            this.dataSourceTypeImageList.Images.SetKeyName(1, "");
            this.dataSourceTypeImageList.Images.SetKeyName(2, "");
            // 
            // dataGridImageList
            // 
            this.dataGridImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dataGridImageList.ImageStream")));
            this.dataGridImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.dataGridImageList.Images.SetKeyName(0, "");
            // 
            // dataRangeRadioButton1
            // 
            resources.ApplyResources(this.dataRangeRadioButton1, "dataRangeRadioButton1");
            this.dataRangeRadioButton1.Checked = true;
            this.dataRangeRadioButton1.Name = "dataRangeRadioButton1";
            this.dataRangeRadioButton1.TabStop = true;
            this.toolTip1.SetToolTip(this.dataRangeRadioButton1, resources.GetString("dataRangeRadioButton1.ToolTip"));
            this.dataRangeRadioButton1.CheckedChanged += new System.EventHandler(this.dataRangeRadioButton1_CheckedChanged);
            // 
            // dataRangeRadioButton2
            // 
            resources.ApplyResources(this.dataRangeRadioButton2, "dataRangeRadioButton2");
            this.dataRangeRadioButton2.Name = "dataRangeRadioButton2";
            this.toolTip1.SetToolTip(this.dataRangeRadioButton2, resources.GetString("dataRangeRadioButton2.ToolTip"));
            this.dataRangeRadioButton2.CheckedChanged += new System.EventHandler(this.dataRangeRadioButton2_CheckedChanged);
            // 
            // delConveterButton
            // 
            resources.ApplyResources(this.delConveterButton, "delConveterButton");
            this.delConveterButton.Name = "delConveterButton";
            this.toolTip1.SetToolTip(this.delConveterButton, resources.GetString("delConveterButton.ToolTip"));
            this.delConveterButton.Click += new System.EventHandler(this.delConveterButton_Click);
            // 
            // addConverterButton
            // 
            resources.ApplyResources(this.addConverterButton, "addConverterButton");
            this.addConverterButton.Name = "addConverterButton";
            this.toolTip1.SetToolTip(this.addConverterButton, resources.GetString("addConverterButton.ToolTip"));
            this.addConverterButton.Click += new System.EventHandler(this.addConverterButton_Click);
            // 
            // pickConverterButton
            // 
            resources.ApplyResources(this.pickConverterButton, "pickConverterButton");
            this.pickConverterButton.Name = "pickConverterButton";
            this.toolTip1.SetToolTip(this.pickConverterButton, resources.GetString("pickConverterButton.ToolTip"));
            this.pickConverterButton.Click += new System.EventHandler(this.pickConverterButton_Click);
            // 
            // readExcelAllRadioButton
            // 
            resources.ApplyResources(this.readExcelAllRadioButton, "readExcelAllRadioButton");
            this.readExcelAllRadioButton.Checked = true;
            this.readExcelAllRadioButton.Name = "readExcelAllRadioButton";
            this.readExcelAllRadioButton.TabStop = true;
            this.toolTip1.SetToolTip(this.readExcelAllRadioButton, resources.GetString("readExcelAllRadioButton.ToolTip"));
            this.readExcelAllRadioButton.CheckedChanged += new System.EventHandler(this.readExcelAllRadioButton_CheckedChanged);
            // 
            // readExcelInBlockRadioButton
            // 
            resources.ApplyResources(this.readExcelInBlockRadioButton, "readExcelInBlockRadioButton");
            this.readExcelInBlockRadioButton.Name = "readExcelInBlockRadioButton";
            this.toolTip1.SetToolTip(this.readExcelInBlockRadioButton, resources.GetString("readExcelInBlockRadioButton.ToolTip"));
            this.readExcelInBlockRadioButton.CheckedChanged += new System.EventHandler(this.readExcelInBlockRadioButton_CheckedChanged);
            // 
            // delExcelConverterButton
            // 
            resources.ApplyResources(this.delExcelConverterButton, "delExcelConverterButton");
            this.delExcelConverterButton.Name = "delExcelConverterButton";
            this.toolTip1.SetToolTip(this.delExcelConverterButton, resources.GetString("delExcelConverterButton.ToolTip"));
            this.delExcelConverterButton.Click += new System.EventHandler(this.delExcelConverterButton_Click);
            // 
            // AddExcelConverterButton
            // 
            resources.ApplyResources(this.AddExcelConverterButton, "AddExcelConverterButton");
            this.AddExcelConverterButton.Name = "AddExcelConverterButton";
            this.toolTip1.SetToolTip(this.AddExcelConverterButton, resources.GetString("AddExcelConverterButton.ToolTip"));
            this.AddExcelConverterButton.Click += new System.EventHandler(this.AddExcelConverterButton_Click);
            // 
            // autoPickExcelConverterButton
            // 
            resources.ApplyResources(this.autoPickExcelConverterButton, "autoPickExcelConverterButton");
            this.autoPickExcelConverterButton.Name = "autoPickExcelConverterButton";
            this.toolTip1.SetToolTip(this.autoPickExcelConverterButton, resources.GetString("autoPickExcelConverterButton.ToolTip"));
            this.autoPickExcelConverterButton.Click += new System.EventHandler(this.autoPickExcelConverterButton_Click);
            // 
            // readOtherAllRadioButton
            // 
            resources.ApplyResources(this.readOtherAllRadioButton, "readOtherAllRadioButton");
            this.readOtherAllRadioButton.Checked = true;
            this.readOtherAllRadioButton.Name = "readOtherAllRadioButton";
            this.readOtherAllRadioButton.TabStop = true;
            this.toolTip1.SetToolTip(this.readOtherAllRadioButton, resources.GetString("readOtherAllRadioButton.ToolTip"));
            this.readOtherAllRadioButton.CheckedChanged += new System.EventHandler(this.readOtherAllRadioButton_CheckedChanged);
            // 
            // readOtherInBlockRadioButton
            // 
            resources.ApplyResources(this.readOtherInBlockRadioButton, "readOtherInBlockRadioButton");
            this.readOtherInBlockRadioButton.Name = "readOtherInBlockRadioButton";
            this.toolTip1.SetToolTip(this.readOtherInBlockRadioButton, resources.GetString("readOtherInBlockRadioButton.ToolTip"));
            this.readOtherInBlockRadioButton.CheckedChanged += new System.EventHandler(this.readOtherInBlockRadioButton_CheckedChanged);
            // 
            // delOtherConverterButton
            // 
            resources.ApplyResources(this.delOtherConverterButton, "delOtherConverterButton");
            this.delOtherConverterButton.Name = "delOtherConverterButton";
            this.toolTip1.SetToolTip(this.delOtherConverterButton, resources.GetString("delOtherConverterButton.ToolTip"));
            this.delOtherConverterButton.Click += new System.EventHandler(this.delOtherConverterButton_Click);
            // 
            // addOtherConverterButton
            // 
            resources.ApplyResources(this.addOtherConverterButton, "addOtherConverterButton");
            this.addOtherConverterButton.Name = "addOtherConverterButton";
            this.toolTip1.SetToolTip(this.addOtherConverterButton, resources.GetString("addOtherConverterButton.ToolTip"));
            this.addOtherConverterButton.Click += new System.EventHandler(this.addOtherConverterButton_Click);
            // 
            // autoPickOtherConverterButton
            // 
            resources.ApplyResources(this.autoPickOtherConverterButton, "autoPickOtherConverterButton");
            this.autoPickOtherConverterButton.Name = "autoPickOtherConverterButton";
            this.toolTip1.SetToolTip(this.autoPickOtherConverterButton, resources.GetString("autoPickOtherConverterButton.ToolTip"));
            this.autoPickOtherConverterButton.Click += new System.EventHandler(this.autoPickOtherConverterButton_Click);
            // 
            // previewTextFileComboBox
            // 
            resources.ApplyResources(this.previewTextFileComboBox, "previewTextFileComboBox");
            this.previewTextFileComboBox.Name = "previewTextFileComboBox";
            this.toolTip1.SetToolTip(this.previewTextFileComboBox, resources.GetString("previewTextFileComboBox.ToolTip"));
            this.previewTextFileComboBox.SelectedIndexChanged += new System.EventHandler(this.previewTextFileComboBox_SelectedIndexChanged);
            // 
            // addEntryButton
            // 
            resources.ApplyResources(this.addEntryButton, "addEntryButton");
            this.addEntryButton.Name = "addEntryButton";
            this.toolTip1.SetToolTip(this.addEntryButton, resources.GetString("addEntryButton.ToolTip"));
            this.addEntryButton.Click += new System.EventHandler(this.addEntryButton_Click);
            // 
            // noValidationCheckBox
            // 
            resources.ApplyResources(this.noValidationCheckBox, "noValidationCheckBox");
            this.noValidationCheckBox.Name = "noValidationCheckBox";
            this.toolTip1.SetToolTip(this.noValidationCheckBox, resources.GetString("noValidationCheckBox.ToolTip"));
            // 
            // includeUpdateCheckBox
            // 
            resources.ApplyResources(this.includeUpdateCheckBox, "includeUpdateCheckBox");
            this.includeUpdateCheckBox.Name = "includeUpdateCheckBox";
            this.toolTip1.SetToolTip(this.includeUpdateCheckBox, resources.GetString("includeUpdateCheckBox.ToolTip"));
            // 
            // importWizardForm
            // 
            resources.ApplyResources(this.importWizardForm, "importWizardForm");
            this.importWizardForm.Name = "importWizardForm";
            this.importWizardForm.PageIndex = 9;
            this.importWizardForm.Pages.AddRange(new Newtera.Studio.UserControls.Wizards.WizardPageBase[] {
            this.wizardWelcomePage1,
            this.chooseDataSourcePage,
            this.determineFileFormatPage,
            this.excelFormatPage,
            this.otherFormatPage,
            this.previewDataPage,
            this.specifyMappingPage,
            this.previewPostTransformPage,
            this.runSaveImportPage,
            this.wizardFinalPage1});
            this.toolTip1.SetToolTip(this.importWizardForm, resources.GetString("importWizardForm.ToolTip"));
            this.importWizardForm.Next += new Newtera.Studio.UserControls.Wizards.WizardForm.WizardNextEventHandler(this.importWizardForm_Next);
            this.importWizardForm.Back += new Newtera.Studio.UserControls.Wizards.WizardForm.WizardNextEventHandler(this.importWizardForm_Back);
            this.importWizardForm.Continue += new Newtera.Studio.UserControls.Wizards.WizardForm.WizardNextEventHandler(this.importWizardForm_Continue);
            this.importWizardForm.Load += new System.EventHandler(this.importWizardForm_Load);
            // 
            // wizardWelcomePage1
            // 
            resources.ApplyResources(this.wizardWelcomePage1, "wizardWelcomePage1");
            this.wizardWelcomePage1.BackColor = System.Drawing.Color.White;
            this.wizardWelcomePage1.HeaderImage = ((System.Drawing.Image)(resources.GetObject("wizardWelcomePage1.HeaderImage")));
            this.wizardWelcomePage1.Index = 0;
            this.wizardWelcomePage1.Name = "wizardWelcomePage1";
            this.toolTip1.SetToolTip(this.wizardWelcomePage1, resources.GetString("wizardWelcomePage1.ToolTip"));
            this.wizardWelcomePage1.WizardPageParent = this.importWizardForm;
            this.wizardWelcomePage1.Load += new System.EventHandler(this.wizardWelcomePage1_Load);
            // 
            // chooseDataSourcePage
            // 
            resources.ApplyResources(this.chooseDataSourcePage, "chooseDataSourcePage");
            this.chooseDataSourcePage.Controls.Add(this.groupBox6);
            this.chooseDataSourcePage.Controls.Add(this.groupBox1);
            this.chooseDataSourcePage.Controls.Add(this.dataSourceComboBox);
            this.chooseDataSourcePage.Controls.Add(this.label1);
            this.chooseDataSourcePage.HeaderImage = ((System.Drawing.Image)(resources.GetObject("chooseDataSourcePage.HeaderImage")));
            this.chooseDataSourcePage.Index = 1;
            this.chooseDataSourcePage.Name = "chooseDataSourcePage";
            this.toolTip1.SetToolTip(this.chooseDataSourcePage, resources.GetString("chooseDataSourcePage.ToolTip"));
            this.chooseDataSourcePage.WizardPageParent = this.importWizardForm;
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.selectImportPackageBtn);
            this.groupBox6.Controls.Add(this.choosenImportPackageTextBox);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
            // 
            // selectImportPackageBtn
            // 
            resources.ApplyResources(this.selectImportPackageBtn, "selectImportPackageBtn");
            this.selectImportPackageBtn.Name = "selectImportPackageBtn";
            this.toolTip1.SetToolTip(this.selectImportPackageBtn, resources.GetString("selectImportPackageBtn.ToolTip"));
            this.selectImportPackageBtn.Click += new System.EventHandler(this.selectImportPackageBtn_Click);
            // 
            // choosenImportPackageTextBox
            // 
            resources.ApplyResources(this.choosenImportPackageTextBox, "choosenImportPackageTextBox");
            this.choosenImportPackageTextBox.Name = "choosenImportPackageTextBox";
            this.toolTip1.SetToolTip(this.choosenImportPackageTextBox, resources.GetString("choosenImportPackageTextBox.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.openFileButton);
            this.groupBox1.Controls.Add(this.dataSourceFileNameTextBox);
            this.groupBox1.Controls.Add(this.dataSourceFileNameLabel);
            this.groupBox1.Controls.Add(this.selectedDataSourceText);
            this.groupBox1.Controls.Add(this.selectedDataSourceImage);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // openFileButton
            // 
            resources.ApplyResources(this.openFileButton, "openFileButton");
            this.openFileButton.Name = "openFileButton";
            this.toolTip1.SetToolTip(this.openFileButton, resources.GetString("openFileButton.ToolTip"));
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // dataSourceFileNameTextBox
            // 
            resources.ApplyResources(this.dataSourceFileNameTextBox, "dataSourceFileNameTextBox");
            this.dataSourceFileNameTextBox.Name = "dataSourceFileNameTextBox";
            this.toolTip1.SetToolTip(this.dataSourceFileNameTextBox, resources.GetString("dataSourceFileNameTextBox.ToolTip"));
            // 
            // dataSourceFileNameLabel
            // 
            resources.ApplyResources(this.dataSourceFileNameLabel, "dataSourceFileNameLabel");
            this.dataSourceFileNameLabel.Name = "dataSourceFileNameLabel";
            this.toolTip1.SetToolTip(this.dataSourceFileNameLabel, resources.GetString("dataSourceFileNameLabel.ToolTip"));
            // 
            // selectedDataSourceText
            // 
            resources.ApplyResources(this.selectedDataSourceText, "selectedDataSourceText");
            this.selectedDataSourceText.Name = "selectedDataSourceText";
            this.toolTip1.SetToolTip(this.selectedDataSourceText, resources.GetString("selectedDataSourceText.ToolTip"));
            // 
            // selectedDataSourceImage
            // 
            resources.ApplyResources(this.selectedDataSourceImage, "selectedDataSourceImage");
            this.selectedDataSourceImage.ImageList = this.dataSourceTypeImageList;
            this.selectedDataSourceImage.Name = "selectedDataSourceImage";
            this.toolTip1.SetToolTip(this.selectedDataSourceImage, resources.GetString("selectedDataSourceImage.ToolTip"));
            // 
            // dataSourceComboBox
            // 
            resources.ApplyResources(this.dataSourceComboBox, "dataSourceComboBox");
            this.dataSourceComboBox.Name = "dataSourceComboBox";
            this.toolTip1.SetToolTip(this.dataSourceComboBox, resources.GetString("dataSourceComboBox.ToolTip"));
            this.dataSourceComboBox.SelectedIndexChanged += new System.EventHandler(this.dataSourceComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // determineFileFormatPage
            // 
            resources.ApplyResources(this.determineFileFormatPage, "determineFileFormatPage");
            this.determineFileFormatPage.Controls.Add(this.panel1);
            this.determineFileFormatPage.HeaderImage = ((System.Drawing.Image)(resources.GetObject("determineFileFormatPage.HeaderImage")));
            this.determineFileFormatPage.Index = 2;
            this.determineFileFormatPage.Name = "determineFileFormatPage";
            this.toolTip1.SetToolTip(this.determineFileFormatPage, resources.GetString("determineFileFormatPage.ToolTip"));
            this.determineFileFormatPage.WizardPageParent = this.importWizardForm;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.groupBox7);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.freeFormedRadioButton);
            this.panel1.Controls.Add(this.delimitedRadioButton);
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.blockSizeTextBox);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.panel2);
            this.groupBox7.Controls.Add(this.label10);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox7, resources.GetString("groupBox7.ToolTip"));
            // 
            // blockSizeTextBox
            // 
            resources.ApplyResources(this.blockSizeTextBox, "blockSizeTextBox");
            this.blockSizeTextBox.Name = "blockSizeTextBox";
            this.blockSizeTextBox.ReadOnly = true;
            this.toolTip1.SetToolTip(this.blockSizeTextBox, resources.GetString("blockSizeTextBox.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.dataRangeRadioButton1);
            this.panel2.Controls.Add(this.dataRangeRadioButton2);
            this.panel2.Name = "panel2";
            this.toolTip1.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.delConveterButton);
            this.groupBox3.Controls.Add(this.addConverterButton);
            this.groupBox3.Controls.Add(this.pickConverterButton);
            this.groupBox3.Controls.Add(this.textConverterComboBox);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // textConverterComboBox
            // 
            resources.ApplyResources(this.textConverterComboBox, "textConverterComboBox");
            this.textConverterComboBox.Name = "textConverterComboBox";
            this.toolTip1.SetToolTip(this.textConverterComboBox, resources.GetString("textConverterComboBox.ToolTip"));
            this.textConverterComboBox.SelectedIndexChanged += new System.EventHandler(this.textConverterComboBox_SelectedIndexChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.startingDataRowTextBox);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.columnDelimiterRadioButton4);
            this.groupBox2.Controls.Add(this.columnDelimiterRadioButton3);
            this.groupBox2.Controls.Add(this.columnDelimiterRadioButton2);
            this.groupBox2.Controls.Add(this.columnDelimiterRadioButton1);
            this.groupBox2.Controls.Add(this.firstRowColumnNameCheckBox);
            this.groupBox2.Controls.Add(this.otherColumnDelimiterTextBox);
            this.groupBox2.Controls.Add(this.rowDelimiterComboBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // startingDataRowTextBox
            // 
            resources.ApplyResources(this.startingDataRowTextBox, "startingDataRowTextBox");
            this.startingDataRowTextBox.Name = "startingDataRowTextBox";
            this.toolTip1.SetToolTip(this.startingDataRowTextBox, resources.GetString("startingDataRowTextBox.ToolTip"));
            this.startingDataRowTextBox.TextChanged += new System.EventHandler(this.startingDataRowTextBox_TextChanged);
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            this.toolTip1.SetToolTip(this.label18, resources.GetString("label18.ToolTip"));
            // 
            // columnDelimiterRadioButton4
            // 
            resources.ApplyResources(this.columnDelimiterRadioButton4, "columnDelimiterRadioButton4");
            this.columnDelimiterRadioButton4.Name = "columnDelimiterRadioButton4";
            this.toolTip1.SetToolTip(this.columnDelimiterRadioButton4, resources.GetString("columnDelimiterRadioButton4.ToolTip"));
            this.columnDelimiterRadioButton4.CheckedChanged += new System.EventHandler(this.columnDelimiterRadioButton4_CheckedChanged);
            // 
            // columnDelimiterRadioButton3
            // 
            resources.ApplyResources(this.columnDelimiterRadioButton3, "columnDelimiterRadioButton3");
            this.columnDelimiterRadioButton3.Name = "columnDelimiterRadioButton3";
            this.toolTip1.SetToolTip(this.columnDelimiterRadioButton3, resources.GetString("columnDelimiterRadioButton3.ToolTip"));
            this.columnDelimiterRadioButton3.CheckedChanged += new System.EventHandler(this.columnDelimiterRadioButton3_CheckedChanged);
            // 
            // columnDelimiterRadioButton2
            // 
            resources.ApplyResources(this.columnDelimiterRadioButton2, "columnDelimiterRadioButton2");
            this.columnDelimiterRadioButton2.Name = "columnDelimiterRadioButton2";
            this.toolTip1.SetToolTip(this.columnDelimiterRadioButton2, resources.GetString("columnDelimiterRadioButton2.ToolTip"));
            this.columnDelimiterRadioButton2.CheckedChanged += new System.EventHandler(this.columnDelimiterRadioButton2_CheckedChanged);
            // 
            // columnDelimiterRadioButton1
            // 
            resources.ApplyResources(this.columnDelimiterRadioButton1, "columnDelimiterRadioButton1");
            this.columnDelimiterRadioButton1.Checked = true;
            this.columnDelimiterRadioButton1.Name = "columnDelimiterRadioButton1";
            this.columnDelimiterRadioButton1.TabStop = true;
            this.toolTip1.SetToolTip(this.columnDelimiterRadioButton1, resources.GetString("columnDelimiterRadioButton1.ToolTip"));
            this.columnDelimiterRadioButton1.CheckedChanged += new System.EventHandler(this.columnDelimiterRadioButton1_CheckedChanged);
            // 
            // firstRowColumnNameCheckBox
            // 
            resources.ApplyResources(this.firstRowColumnNameCheckBox, "firstRowColumnNameCheckBox");
            this.firstRowColumnNameCheckBox.Name = "firstRowColumnNameCheckBox";
            this.toolTip1.SetToolTip(this.firstRowColumnNameCheckBox, resources.GetString("firstRowColumnNameCheckBox.ToolTip"));
            this.firstRowColumnNameCheckBox.CheckedChanged += new System.EventHandler(this.firstRowColumnNameCheckBox_CheckedChanged);
            // 
            // otherColumnDelimiterTextBox
            // 
            resources.ApplyResources(this.otherColumnDelimiterTextBox, "otherColumnDelimiterTextBox");
            this.otherColumnDelimiterTextBox.Name = "otherColumnDelimiterTextBox";
            this.toolTip1.SetToolTip(this.otherColumnDelimiterTextBox, resources.GetString("otherColumnDelimiterTextBox.ToolTip"));
            this.otherColumnDelimiterTextBox.TextChanged += new System.EventHandler(this.otherColumnDelimiterTextBox_TextChanged);
            // 
            // rowDelimiterComboBox
            // 
            resources.ApplyResources(this.rowDelimiterComboBox, "rowDelimiterComboBox");
            this.rowDelimiterComboBox.Items.AddRange(new object[] {
            resources.GetString("rowDelimiterComboBox.Items"),
            resources.GetString("rowDelimiterComboBox.Items1"),
            resources.GetString("rowDelimiterComboBox.Items2"),
            resources.GetString("rowDelimiterComboBox.Items3"),
            resources.GetString("rowDelimiterComboBox.Items4"),
            resources.GetString("rowDelimiterComboBox.Items5"),
            resources.GetString("rowDelimiterComboBox.Items6"),
            resources.GetString("rowDelimiterComboBox.Items7")});
            this.rowDelimiterComboBox.Name = "rowDelimiterComboBox";
            this.toolTip1.SetToolTip(this.rowDelimiterComboBox, resources.GetString("rowDelimiterComboBox.ToolTip"));
            this.rowDelimiterComboBox.SelectedIndexChanged += new System.EventHandler(this.rowDelimiterComboBox_SelectedIndexChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // freeFormedRadioButton
            // 
            resources.ApplyResources(this.freeFormedRadioButton, "freeFormedRadioButton");
            this.freeFormedRadioButton.Name = "freeFormedRadioButton";
            this.toolTip1.SetToolTip(this.freeFormedRadioButton, resources.GetString("freeFormedRadioButton.ToolTip"));
            this.freeFormedRadioButton.CheckedChanged += new System.EventHandler(this.freeFormedRadioButton_CheckedChanged);
            // 
            // delimitedRadioButton
            // 
            resources.ApplyResources(this.delimitedRadioButton, "delimitedRadioButton");
            this.delimitedRadioButton.Checked = true;
            this.delimitedRadioButton.Name = "delimitedRadioButton";
            this.delimitedRadioButton.TabStop = true;
            this.toolTip1.SetToolTip(this.delimitedRadioButton, resources.GetString("delimitedRadioButton.ToolTip"));
            this.delimitedRadioButton.CheckedChanged += new System.EventHandler(this.delimitedRadioButton_CheckedChanged);
            // 
            // excelFormatPage
            // 
            resources.ApplyResources(this.excelFormatPage, "excelFormatPage");
            this.excelFormatPage.Controls.Add(this.groupBox10);
            this.excelFormatPage.Controls.Add(this.groupBox9);
            this.excelFormatPage.Controls.Add(this.specialConveterRadioButton);
            this.excelFormatPage.Controls.Add(this.standardConverterRadioButton);
            this.excelFormatPage.Index = 3;
            this.excelFormatPage.Name = "excelFormatPage";
            this.toolTip1.SetToolTip(this.excelFormatPage, resources.GetString("excelFormatPage.ToolTip"));
            this.excelFormatPage.WizardPageParent = this.importWizardForm;
            // 
            // groupBox10
            // 
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Controls.Add(this.readExcelBlockSizeTextBox);
            this.groupBox10.Controls.Add(this.label13);
            this.groupBox10.Controls.Add(this.panel3);
            this.groupBox10.Controls.Add(this.label14);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox10, resources.GetString("groupBox10.ToolTip"));
            // 
            // readExcelBlockSizeTextBox
            // 
            resources.ApplyResources(this.readExcelBlockSizeTextBox, "readExcelBlockSizeTextBox");
            this.readExcelBlockSizeTextBox.Name = "readExcelBlockSizeTextBox";
            this.readExcelBlockSizeTextBox.ReadOnly = true;
            this.toolTip1.SetToolTip(this.readExcelBlockSizeTextBox, resources.GetString("readExcelBlockSizeTextBox.ToolTip"));
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            this.toolTip1.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.readExcelAllRadioButton);
            this.panel3.Controls.Add(this.readExcelInBlockRadioButton);
            this.panel3.Name = "panel3";
            this.toolTip1.SetToolTip(this.panel3, resources.GetString("panel3.ToolTip"));
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            this.toolTip1.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
            // 
            // groupBox9
            // 
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Controls.Add(this.delExcelConverterButton);
            this.groupBox9.Controls.Add(this.AddExcelConverterButton);
            this.groupBox9.Controls.Add(this.autoPickExcelConverterButton);
            this.groupBox9.Controls.Add(this.excelConverterComboBox);
            this.groupBox9.Controls.Add(this.label12);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox9, resources.GetString("groupBox9.ToolTip"));
            // 
            // excelConverterComboBox
            // 
            resources.ApplyResources(this.excelConverterComboBox, "excelConverterComboBox");
            this.excelConverterComboBox.Name = "excelConverterComboBox";
            this.toolTip1.SetToolTip(this.excelConverterComboBox, resources.GetString("excelConverterComboBox.ToolTip"));
            this.excelConverterComboBox.SelectedIndexChanged += new System.EventHandler(this.excelConveterComboBox_SelectedIndexChanged);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // specialConveterRadioButton
            // 
            resources.ApplyResources(this.specialConveterRadioButton, "specialConveterRadioButton");
            this.specialConveterRadioButton.Name = "specialConveterRadioButton";
            this.toolTip1.SetToolTip(this.specialConveterRadioButton, resources.GetString("specialConveterRadioButton.ToolTip"));
            this.specialConveterRadioButton.CheckedChanged += new System.EventHandler(this.specialConveterRadioButton_CheckedChanged);
            // 
            // standardConverterRadioButton
            // 
            resources.ApplyResources(this.standardConverterRadioButton, "standardConverterRadioButton");
            this.standardConverterRadioButton.Checked = true;
            this.standardConverterRadioButton.Name = "standardConverterRadioButton";
            this.standardConverterRadioButton.TabStop = true;
            this.toolTip1.SetToolTip(this.standardConverterRadioButton, resources.GetString("standardConverterRadioButton.ToolTip"));
            this.standardConverterRadioButton.CheckedChanged += new System.EventHandler(this.standardConverterRadioButton_CheckedChanged);
            // 
            // otherFormatPage
            // 
            resources.ApplyResources(this.otherFormatPage, "otherFormatPage");
            this.otherFormatPage.Controls.Add(this.groupBox11);
            this.otherFormatPage.Controls.Add(this.groupBox8);
            this.otherFormatPage.Index = 4;
            this.otherFormatPage.Name = "otherFormatPage";
            this.toolTip1.SetToolTip(this.otherFormatPage, resources.GetString("otherFormatPage.ToolTip"));
            this.otherFormatPage.WizardPageParent = this.importWizardForm;
            // 
            // groupBox11
            // 
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Controls.Add(this.readOtherBlockSizeTextBox);
            this.groupBox11.Controls.Add(this.label16);
            this.groupBox11.Controls.Add(this.panel4);
            this.groupBox11.Controls.Add(this.label17);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox11, resources.GetString("groupBox11.ToolTip"));
            // 
            // readOtherBlockSizeTextBox
            // 
            resources.ApplyResources(this.readOtherBlockSizeTextBox, "readOtherBlockSizeTextBox");
            this.readOtherBlockSizeTextBox.Name = "readOtherBlockSizeTextBox";
            this.readOtherBlockSizeTextBox.ReadOnly = true;
            this.toolTip1.SetToolTip(this.readOtherBlockSizeTextBox, resources.GetString("readOtherBlockSizeTextBox.ToolTip"));
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            this.toolTip1.SetToolTip(this.label16, resources.GetString("label16.ToolTip"));
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Controls.Add(this.readOtherAllRadioButton);
            this.panel4.Controls.Add(this.readOtherInBlockRadioButton);
            this.panel4.Name = "panel4";
            this.toolTip1.SetToolTip(this.panel4, resources.GetString("panel4.ToolTip"));
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            this.toolTip1.SetToolTip(this.label17, resources.GetString("label17.ToolTip"));
            // 
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Controls.Add(this.delOtherConverterButton);
            this.groupBox8.Controls.Add(this.addOtherConverterButton);
            this.groupBox8.Controls.Add(this.autoPickOtherConverterButton);
            this.groupBox8.Controls.Add(this.otherConverterComboBox);
            this.groupBox8.Controls.Add(this.label15);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox8, resources.GetString("groupBox8.ToolTip"));
            // 
            // otherConverterComboBox
            // 
            resources.ApplyResources(this.otherConverterComboBox, "otherConverterComboBox");
            this.otherConverterComboBox.Name = "otherConverterComboBox";
            this.toolTip1.SetToolTip(this.otherConverterComboBox, resources.GetString("otherConverterComboBox.ToolTip"));
            this.otherConverterComboBox.SelectedIndexChanged += new System.EventHandler(this.otherConverterComboBox_SelectedIndexChanged);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            this.toolTip1.SetToolTip(this.label15, resources.GetString("label15.ToolTip"));
            // 
            // previewDataPage
            // 
            resources.ApplyResources(this.previewDataPage, "previewDataPage");
            this.previewDataPage.Controls.Add(this.previewTextFileComboBox);
            this.previewDataPage.Controls.Add(this.textFileNameLabel);
            this.previewDataPage.Controls.Add(this.previewDataGrid);
            this.previewDataPage.HeaderImage = ((System.Drawing.Image)(resources.GetObject("previewDataPage.HeaderImage")));
            this.previewDataPage.Index = 5;
            this.previewDataPage.Name = "previewDataPage";
            this.toolTip1.SetToolTip(this.previewDataPage, resources.GetString("previewDataPage.ToolTip"));
            this.previewDataPage.WizardPageParent = this.importWizardForm;
            // 
            // textFileNameLabel
            // 
            resources.ApplyResources(this.textFileNameLabel, "textFileNameLabel");
            this.textFileNameLabel.Name = "textFileNameLabel";
            this.toolTip1.SetToolTip(this.textFileNameLabel, resources.GetString("textFileNameLabel.ToolTip"));
            // 
            // previewDataGrid
            // 
            resources.ApplyResources(this.previewDataGrid, "previewDataGrid");
            this.previewDataGrid.CaptionVisible = false;
            this.previewDataGrid.DataMember = "";
            this.previewDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.previewDataGrid.Name = "previewDataGrid";
            this.previewDataGrid.ReadOnly = true;
            this.previewDataGrid.RowHeadersVisible = false;
            this.toolTip1.SetToolTip(this.previewDataGrid, resources.GetString("previewDataGrid.ToolTip"));
            // 
            // specifyMappingPage
            // 
            resources.ApplyResources(this.specifyMappingPage, "specifyMappingPage");
            this.specifyMappingPage.Controls.Add(this.viewSourceButton);
            this.specifyMappingPage.Controls.Add(this.addEntryButton);
            this.specifyMappingPage.Controls.Add(this.classMapListView);
            this.specifyMappingPage.HeaderImage = ((System.Drawing.Image)(resources.GetObject("specifyMappingPage.HeaderImage")));
            this.specifyMappingPage.Index = 6;
            this.specifyMappingPage.Name = "specifyMappingPage";
            this.toolTip1.SetToolTip(this.specifyMappingPage, resources.GetString("specifyMappingPage.ToolTip"));
            this.specifyMappingPage.WizardPageParent = this.importWizardForm;
            this.specifyMappingPage.Enter += new System.EventHandler(this.specifyMappingPage_Enter);
            // 
            // viewSourceButton
            // 
            resources.ApplyResources(this.viewSourceButton, "viewSourceButton");
            this.viewSourceButton.Name = "viewSourceButton";
            this.toolTip1.SetToolTip(this.viewSourceButton, resources.GetString("viewSourceButton.ToolTip"));
            this.viewSourceButton.Click += new System.EventHandler(this.viewSourceButton_Click);
            // 
            // classMapListView
            // 
            resources.ApplyResources(this.classMapListView, "classMapListView");
            this.classMapListView.AllowColumnReorder = true;
            this.classMapListView.CheckBoxes = true;
            this.classMapListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sourceColumnHeader,
            this.destinationColumnHeader,
            this.transformColumnHeader});
            this.classMapListView.DoubleClickActivation = false;
            this.classMapListView.FullRowSelect = true;
            this.classMapListView.GridLines = true;
            this.classMapListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.classMapListView.MultiSelect = false;
            this.classMapListView.Name = "classMapListView";
            this.toolTip1.SetToolTip(this.classMapListView, resources.GetString("classMapListView.ToolTip"));
            this.classMapListView.UseCompatibleStateImageBehavior = false;
            this.classMapListView.View = System.Windows.Forms.View.Details;
            this.classMapListView.SubItemClicked += new Newtera.WindowsControl.SubItemEventHandler(this.classMaplistView_SubItemClicked);
            // 
            // sourceColumnHeader
            // 
            resources.ApplyResources(this.sourceColumnHeader, "sourceColumnHeader");
            // 
            // destinationColumnHeader
            // 
            resources.ApplyResources(this.destinationColumnHeader, "destinationColumnHeader");
            // 
            // transformColumnHeader
            // 
            resources.ApplyResources(this.transformColumnHeader, "transformColumnHeader");
            // 
            // previewPostTransformPage
            // 
            resources.ApplyResources(this.previewPostTransformPage, "previewPostTransformPage");
            this.previewPostTransformPage.Controls.Add(this.classDataGrid);
            this.previewPostTransformPage.Controls.Add(this.chartPreviewButton);
            this.previewPostTransformPage.Controls.Add(this.validateDataButton);
            this.previewPostTransformPage.Controls.Add(this.editInstanceButton);
            this.previewPostTransformPage.Controls.Add(this.classComboBox);
            this.previewPostTransformPage.Controls.Add(this.label4);
            this.previewPostTransformPage.Index = 7;
            this.previewPostTransformPage.Name = "previewPostTransformPage";
            this.toolTip1.SetToolTip(this.previewPostTransformPage, resources.GetString("previewPostTransformPage.ToolTip"));
            this.previewPostTransformPage.WizardPageParent = this.importWizardForm;
            // 
            // classDataGrid
            // 
            resources.ApplyResources(this.classDataGrid, "classDataGrid");
            this.classDataGrid.AllowNavigation = false;
            this.classDataGrid.CaptionVisible = false;
            this.classDataGrid.DataMember = "";
            this.classDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.classDataGrid.Name = "classDataGrid";
            this.classDataGrid.ReadOnly = true;
            gridRange1.Bottom = -1;
            gridRange1.Left = -1;
            gridRange1.Right = -1;
            gridRange1.Top = -1;
            this.classDataGrid.SelectedRange = gridRange1;
            this.classDataGrid.SortColumnIndex = -1;
            this.toolTip1.SetToolTip(this.classDataGrid, resources.GetString("classDataGrid.ToolTip"));
            this.classDataGrid.Click += new System.EventHandler(this.classDataGrid_Click);
            this.classDataGrid.DoubleClick += new System.EventHandler(this.classDataGrid_DoubleClick);
            // 
            // chartPreviewButton
            // 
            resources.ApplyResources(this.chartPreviewButton, "chartPreviewButton");
            this.chartPreviewButton.Name = "chartPreviewButton";
            this.toolTip1.SetToolTip(this.chartPreviewButton, resources.GetString("chartPreviewButton.ToolTip"));
            this.chartPreviewButton.Click += new System.EventHandler(this.chartPreviewButton_Click);
            // 
            // validateDataButton
            // 
            resources.ApplyResources(this.validateDataButton, "validateDataButton");
            this.validateDataButton.Name = "validateDataButton";
            this.toolTip1.SetToolTip(this.validateDataButton, resources.GetString("validateDataButton.ToolTip"));
            this.validateDataButton.Click += new System.EventHandler(this.validateDataButton_Click);
            // 
            // editInstanceButton
            // 
            resources.ApplyResources(this.editInstanceButton, "editInstanceButton");
            this.editInstanceButton.Name = "editInstanceButton";
            this.toolTip1.SetToolTip(this.editInstanceButton, resources.GetString("editInstanceButton.ToolTip"));
            this.editInstanceButton.Click += new System.EventHandler(this.editInstanceButton_Click);
            // 
            // classComboBox
            // 
            resources.ApplyResources(this.classComboBox, "classComboBox");
            this.classComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.classComboBox.Name = "classComboBox";
            this.toolTip1.SetToolTip(this.classComboBox, resources.GetString("classComboBox.ToolTip"));
            this.classComboBox.SelectedIndexChanged += new System.EventHandler(this.classComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // runSaveImportPage
            // 
            resources.ApplyResources(this.runSaveImportPage, "runSaveImportPage");
            this.runSaveImportPage.Controls.Add(this.groupBox12);
            this.runSaveImportPage.Controls.Add(this.groupBox5);
            this.runSaveImportPage.Controls.Add(this.groupBox4);
            this.runSaveImportPage.HeaderImage = ((System.Drawing.Image)(resources.GetObject("runSaveImportPage.HeaderImage")));
            this.runSaveImportPage.Index = 8;
            this.runSaveImportPage.Name = "runSaveImportPage";
            this.toolTip1.SetToolTip(this.runSaveImportPage, resources.GetString("runSaveImportPage.ToolTip"));
            this.runSaveImportPage.WizardPageParent = this.importWizardForm;
            // 
            // groupBox12
            // 
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.Controls.Add(this.ukRadioButton);
            this.groupBox12.Controls.Add(this.pkRadioButton);
            this.groupBox12.Controls.Add(this.includeUpdateCheckBox);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox12, resources.GetString("groupBox12.ToolTip"));
            // 
            // ukRadioButton
            // 
            resources.ApplyResources(this.ukRadioButton, "ukRadioButton");
            this.ukRadioButton.Name = "ukRadioButton";
            this.toolTip1.SetToolTip(this.ukRadioButton, resources.GetString("ukRadioButton.ToolTip"));
            this.ukRadioButton.UseVisualStyleBackColor = true;
            this.ukRadioButton.Click += new System.EventHandler(this.ukRadioButton_Click);
            // 
            // pkRadioButton
            // 
            resources.ApplyResources(this.pkRadioButton, "pkRadioButton");
            this.pkRadioButton.Checked = true;
            this.pkRadioButton.Name = "pkRadioButton";
            this.pkRadioButton.TabStop = true;
            this.toolTip1.SetToolTip(this.pkRadioButton, resources.GetString("pkRadioButton.ToolTip"));
            this.pkRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.importPackageDescTextBox);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.importPackageNameTextBox);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.saveImportPackageCheckBox);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox5, resources.GetString("groupBox5.ToolTip"));
            // 
            // importPackageDescTextBox
            // 
            resources.ApplyResources(this.importPackageDescTextBox, "importPackageDescTextBox");
            this.importPackageDescTextBox.Name = "importPackageDescTextBox";
            this.toolTip1.SetToolTip(this.importPackageDescTextBox, resources.GetString("importPackageDescTextBox.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            this.toolTip1.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // importPackageNameTextBox
            // 
            resources.ApplyResources(this.importPackageNameTextBox, "importPackageNameTextBox");
            this.importPackageNameTextBox.Name = "importPackageNameTextBox";
            this.toolTip1.SetToolTip(this.importPackageNameTextBox, resources.GetString("importPackageNameTextBox.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // saveImportPackageCheckBox
            // 
            resources.ApplyResources(this.saveImportPackageCheckBox, "saveImportPackageCheckBox");
            this.saveImportPackageCheckBox.Name = "saveImportPackageCheckBox";
            this.toolTip1.SetToolTip(this.saveImportPackageCheckBox, resources.GetString("saveImportPackageCheckBox.ToolTip"));
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.noValidationCheckBox);
            this.groupBox4.Controls.Add(this.runImmediatlyCheckBox);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
            // 
            // runImmediatlyCheckBox
            // 
            resources.ApplyResources(this.runImmediatlyCheckBox, "runImmediatlyCheckBox");
            this.runImmediatlyCheckBox.Checked = true;
            this.runImmediatlyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.runImmediatlyCheckBox.Name = "runImmediatlyCheckBox";
            this.toolTip1.SetToolTip(this.runImmediatlyCheckBox, resources.GetString("runImmediatlyCheckBox.ToolTip"));
            // 
            // wizardFinalPage1
            // 
            resources.ApplyResources(this.wizardFinalPage1, "wizardFinalPage1");
            this.wizardFinalPage1.BackColor = System.Drawing.Color.White;
            this.wizardFinalPage1.FinishPage = true;
            this.wizardFinalPage1.HeaderImage = ((System.Drawing.Image)(resources.GetObject("wizardFinalPage1.HeaderImage")));
            this.wizardFinalPage1.Index = 9;
            this.wizardFinalPage1.Name = "wizardFinalPage1";
            this.toolTip1.SetToolTip(this.wizardFinalPage1, resources.GetString("wizardFinalPage1.ToolTip"));
            this.wizardFinalPage1.WelcomePage = true;
            this.wizardFinalPage1.WizardPageParent = this.importWizardForm;
            // 
            // ImportWizard
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.importWizardForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImportWizard";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.chooseDataSourcePage.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.determineFileFormatPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.excelFormatPage.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.otherFormatPage.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.previewDataPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewDataGrid)).EndInit();
            this.specifyMappingPage.ResumeLayout(false);
            this.previewPostTransformPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.classDataGrid)).EndInit();
            this.runSaveImportPage.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion


		#region Controller code

		/// <summary>
		/// Gets the name for the source data file that is currently being processed
		/// </summary>
		private string CurrentSourceFile
		{
			get
			{
                if (this._fileNames != null && this._fileNames.Length > this._currentFileIndex)
				{
					return this._fileNames[this._currentFileIndex];
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the source data converter that is currently being used
		/// </summary>
		private IDataSourceConverter CurrentConverter
		{
			get
			{
				if (this._srcConverters.Length > 0)
				{
					return _srcConverters[this._currentFileIndex];
				}
				else
				{
					throw new Exception("No converter available");
				}
			}
		}

		/// <summary>
		/// Gets the source dataset that is currently being processed
		/// </summary>
		private DataSet CurrentSourceDataSet
		{
			get
			{
				if (this._sourceDataSets.Length > 0)
				{
					return this._sourceDataSets[this._currentFileIndex];
				}
				else
				{
					throw new Exception("No source datasets available");
				}
			}
		}

		/// <summary>
		/// Gets the destination that is currently being processed
		/// </summary>
		private DataSet CurrentDestinationDataSet
		{
			get
			{
				if (this._destinationDataSets.Length > 0)
				{
					return this._destinationDataSets[this._currentFileIndex];
				}
				else
				{
					throw new Exception("No destination datasets available");
				}
			}
		}

		/// <summary>
		/// Show the class mappings
		/// </summary>
		private void DisplayClassMappings()
		{
			ListViewItem listViewItem;
			classMapListView.Items.Clear();
			ClassMapping classMapping;
            _hasMissingColumns = false;

			if (this._package.ClassMappings.Count == 0)
			{
				// this is a brand new mapping package, therefore, we need to
				// create the class mappings according to the source data set.
				DataTable dt;
				// this is not a saved package, create new class mappings
				for (int i = 0; i < CurrentSourceDataSet.Tables.Count; i++)
				{
					dt = CurrentSourceDataSet.Tables[i];
					listViewItem = new ListViewItem(dt.TableName);
					listViewItem.Checked = true;
					listViewItem.SubItems.Add(ImportExportResourceManager.GetString("ImportWizard.SelectDstClass"));

					// add a ClassMapping instance to the MappingPackage
					classMapping = _package.AddClassMapping(dt.TableName, "");
					classMapping.SourceClassIndex = i;
					listViewItem.SubItems.Add("...");
					this.classMapListView.Items.Add(listViewItem);
				}
			}
			else
			{
				// the mapping package is one of the saved ones, simply
				// display the class mappings
				// this is a saved package, show the class mapping(s)
				for (int i = 0; i < _package.ClassMappings.Count; i++)
				{
					classMapping = (ClassMapping) _package.ClassMappings[i];
					listViewItem = new ListViewItem(classMapping.SourceClassName);
                    listViewItem.Checked = classMapping.IsChecked;
		
					string destinationClassName = classMapping.DestinationClassName;
					ClassElement destinationClass = _metaData.SchemaModel.FindClass(destinationClassName);
					if (destinationClass != null)
					{
						listViewItem.SubItems.Add(destinationClass.Caption);
					}

					listViewItem.SubItems.Add("...");
					this.classMapListView.Items.Add(listViewItem);

                    // the source file may contains partial columns as specified in the class
                    // mapping. In this case, we will simply add the missing columns to the DataTable
                    // for the source file, so that the import can contibue
                    _hasMissingColumns = AddMissingColumns(classMapping, CurrentSourceDataSet);
				}
			}
		}

		/// <summary>
		/// Transform the data from source to destination according to the transform
		/// mappings, display the transformed data for preview.
		/// </summary>
		private void DisplayTransformedData()
		{
			foreach (ClassMapping classMapping in _package.ClassMappings)
			{
				classMapping.IsChecked = false;
			}

			// set the checked status of each class mapping package
			for (int i = 0; i < classMapListView.Items.Count; i++)
			{
				if (classMapListView.Items[i].Checked)
				{
					((ClassMapping) _package.ClassMappings[i]).IsChecked = true;
				}
				else
				{
					((ClassMapping) _package.ClassMappings[i]).IsChecked = false;
				}
			}

			if (_package.CheckedClassMappings.Count == 0)
			{
				throw new Exception(ImportExportResourceManager.GetString("Error.NoClassMappings"));
			}
				
			ClassMappingCollection checkedClassMappings = _package.CheckedClassMappings;
			// validate if there is a destination class for each chekced class mapping
			for (int i = 0; i < checkedClassMappings.Count; i++)
			{
				ClassMapping classMapping = (ClassMapping) checkedClassMappings[i];
				if (classMapping.DestinationClassName == null ||
					classMapping.DestinationClassName.Length == 0)
				{
					throw new Exception(ImportExportResourceManager.GetString("Error.MissingDestination"));
				}
			}

			// validate the class mapping against the source data to make sure
			// that it is a correct package for the selected data source
			foreach (ClassMapping classMapping in checkedClassMappings)
			{
				// it will throw an exception if the mapping is invalid
				ValidMapping(classMapping, CurrentSourceDataSet);
			}

			// gets the detailed data view for the destination class in each of
			// the class mappings
			foreach (ClassMapping classMapping in checkedClassMappings)
			{
				classMapping.DestinationDataView = _metaData.GetDetailedDataView(classMapping.DestinationClassName);
			}

			// transform the data from src to destination
			_destinationDataSets = new DataSet[_fileNames.Length];
			for (int i = 0; i < _destinationDataSets.Length; i++)
			{
                _destinationDataSets[i] = _package.Transform(_sourceDataSets[i], GetToolBinDir());
			}

			classComboBox.Items.Clear();

			// display the destination classes
			foreach (ClassMapping classMapping in checkedClassMappings)
			{
				string classCaption = _metaData.SchemaModel.FindClass(classMapping.DestinationClassName).Caption;
				classComboBox.Items.Add(classCaption);
			}

			if (checkedClassMappings.Count > 0)
			{
				classComboBox.SelectedIndex = 0;
			}
		}

        /// <summary>
        /// Get the lib dir
        /// </summary>
        /// <returns></returns>
        private string GetToolBinDir()
        {
            string toolBinDir = NewteraNameSpace.GetAppToolDir();

            if (!toolBinDir.EndsWith(@"\"))
            {
                toolBinDir += @"\bin\";
            }
            else
            {
                toolBinDir += @"bin\";
            }

            if (!File.Exists(toolBinDir + "Newtera.Common.dll"))
            {
                // development enviroment
                toolBinDir += @"Debug\";
            }

            return toolBinDir;
        }

		/// <summary>
		/// Validate if the mapping is valid for the given source data
		/// </summary>
		/// <param name="classMapping">The class mapping</param>
		/// <param name="srcDataSet">The source dataset</param>
		/// <exception cref="Exception">If the mapping is invalid</exception>
		private void ValidMapping(ClassMapping classMapping, DataSet srcDataSet)
		{
			DataTable srcTable = srcDataSet.Tables[classMapping.SourceClassIndex];

			if (srcTable == null)
			{
				throw new Exception(ImportExportResourceManager.GetString("ImportWizard.MissingSrcTable" + classMapping.SourceClassName));
			}

			foreach (IMappingNode mapping in classMapping.AttributeMappings)
			{
				switch (mapping.NodeType)
				{
					case Newtera.Common.MetaData.Mappings.NodeType.ArrayDataCellMapping:
					case Newtera.Common.MetaData.Mappings.NodeType.AttributeMapping:
					case Newtera.Common.MetaData.Mappings.NodeType.PrimaryKeyMapping:
						
						AttributeMapping attrMapping = (AttributeMapping) mapping;

						if (srcTable.Columns[attrMapping.SourceAttributeName] == null)
						{
							throw new Exception(ImportExportResourceManager.GetString("ImportWizard.MissingSrcAttribute") + " " + attrMapping.SourceAttributeName);
						}

						break;

					case Newtera.Common.MetaData.Mappings.NodeType.OneToManyMapping:
					case Newtera.Common.MetaData.Mappings.NodeType.ManyToOneMapping:
					case Newtera.Common.MetaData.Mappings.NodeType.ManyToManyMapping:

						MultiAttributeMappingBase multiMapping = (MultiAttributeMappingBase) mapping;

						foreach (InputOutputAttribute inputAttribute in multiMapping.InputAttributes)
						{
							if (srcTable.Columns[inputAttribute.AttributeName] == null)
							{
								throw new Exception(ImportExportResourceManager.GetString("ImportWizard.MissingSrcAttribute") + " " + inputAttribute.AttributeName);
							}
						}

						break;
				}
			}
		}

        /// <summary>
        /// Add the missing columns to the DataTable as specified in the class mapping
        /// </summary>
        /// <param name="classMapping">The class mapping</param>
        /// <param name="srcDataSet">The source dataset</param>
        /// <returns>True if there are missing columns, false, otherwise</returns>
        private bool AddMissingColumns(ClassMapping classMapping, DataSet srcDataSet)
        {
            bool status = false;
            DataTable srcTable = srcDataSet.Tables[classMapping.SourceClassIndex];

            if (srcTable == null)
            {
                throw new Exception(ImportExportResourceManager.GetString("ImportWizard.MissingSrcTable" + classMapping.SourceClassName));
            }

            foreach (IMappingNode mapping in classMapping.AttributeMappings)
            {
                switch (mapping.NodeType)
                {
                    case Newtera.Common.MetaData.Mappings.NodeType.ArrayDataCellMapping:
                    case Newtera.Common.MetaData.Mappings.NodeType.AttributeMapping:
                    case Newtera.Common.MetaData.Mappings.NodeType.PrimaryKeyMapping:

                        AttributeMapping attrMapping = (AttributeMapping)mapping;

                        if (srcTable.Columns[attrMapping.SourceAttributeName] == null)
                        {
                            srcTable.Columns.Add(attrMapping.SourceAttributeName);
                            status = true;
                        }

                        break;

                    case Newtera.Common.MetaData.Mappings.NodeType.OneToManyMapping:
                    case Newtera.Common.MetaData.Mappings.NodeType.ManyToOneMapping:
                    case Newtera.Common.MetaData.Mappings.NodeType.ManyToManyMapping:

                        MultiAttributeMappingBase multiMapping = (MultiAttributeMappingBase)mapping;

                        foreach (InputOutputAttribute inputAttribute in multiMapping.InputAttributes)
                        {
                            if (srcTable.Columns[inputAttribute.AttributeName] == null)
                            {
                                srcTable.Columns.Add(inputAttribute.AttributeName);
                                status = true;
                            }
                        }

                        break;
                }
            }

            return status;
        }

		/// <summary>
		/// Save the transformed data to database, and/or, save the specified
		/// import package to the database.
		/// </summary>
		private void SaveDataOrImportPackage()
		{
			if (this.runImmediatlyCheckBox.Checked)
			{
				SaveTransformedData();
			}
			else if (this.saveImportPackageCheckBox.Checked)
			{
				if (!_metaData.MappingManager.IsPackageExist(_package.Name))
				{
					// add the package to the mapping manager
					_metaData.MappingManager.AddMappingPackage(_package);
				}

				SaveImportPackage();
			}
		}

		/// <summary>
		/// Cancel the import job in a worker thread
		/// </summary>
		private void CancelImportJob()
		{
			this._isImportCancelled = true;
		}

		/// <summary>
		/// Save the transformed data to database.
		/// </summary>
		private void SaveTransformedData()
		{
            if (_package.NeedValidation)
            {
                PerformNormalImports();
            }
            else
            {
                PerformFastImports();
            }
		}

        /// <summary>
        /// Perform normal imports by generating import scripts consisting of XQuery statements
        /// so that it go through the validation provided by the server
        /// </summary>
        private void PerformNormalImports()
        {
            _scriptResults = new ScriptNodeCollection();

            ScriptManager scriptManager = new ScriptManager();
            StringBuilder builder = null;

            if (_package.CheckedClassMappings.Count == 0)
            {
                return;
            }

            ClassMapping classMapping = (ClassMapping)_package.CheckedClassMappings[0];

            _currentChunkIndex = 0;

            int actual = GenerateScripts(scriptManager, classMapping, _package.InstanceIdentifier);

            // initialize the counters
            _currentClassMappingIndex = 0;
            _currentRowIndex = actual; // next row index
            _currentChunkIndex++; // increase to next chunk

            // convert the import scripts to a xml string to be sent to
            // the server
            builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            scriptManager.Write(writer);

            // execute the first chunk of import script
            _isRequestComplete = false;
            _isImportCancelled = false;

            _dataService = new CMDataServiceStub();

            // clear the cache which may have data left over from the last import action
            _dataService.ResetDataCache();

            // invoke the web service asynchronously
            string xml = _dataService.ExecuteImportScripts(
                ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                builder.ToString(), 0);

            ExecuteImportScriptsDone(xml);
        }

		/// <summary>
		/// The AsyncCallback event handler for ExecuteImportScriptsDone web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void ExecuteImportScriptsDone(string xml)
		{
			try
			{						
				// Get the query result in DataSet

				// read the xml that contains script execution status and detail messages
				StringReader reader = new StringReader(xml);
				ScriptManager scriptResult = new ScriptManager();
				scriptResult.Read(reader);

				// If there are errors, stop the importing process
				if (scriptResult.IsSucceeded)
				{
					// this is the worker process, execute the rest of import task
					// synchronously

					_dataService = new CMDataServiceStub();

					// import all source files until it is done or cancelled
					while (_currentFileIndex < _fileNames.Length)
					{
						// import the data in the current source file
						ImportDataOfCurrentFile();
						
						if (_isImportCancelled)
						{
							// process is cancelled, stop now
							break;
						}
						else
						{
							// finishing data in one file, continue on
							// the next file if it exists
							_currentChunkIndex = 0;
							_currentRowIndex = 0;
							_currentBlock = 0;
							_currentClassMappingIndex = 0;
							_currentFileIndex++; // move to the next file
						}
					}
				}
				else
				{
					_scriptResults.Add(scriptResult);
				}

				ShowScriptResult(_scriptResults);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			finally
			{
				// always Close the converters
				CleanupSrcConverters();
			}
		}

		/// <summary>
		/// Import the data converted from the source file that is being currently
		/// processed. It method will exit either when all data have been imported
		/// or the process is cancelled by the user.
		/// </summary>
		private void ImportDataOfCurrentFile()
		{
			string xml;
			StringReader reader;
			ScriptManager scriptResult;
			StringBuilder builder;
			StringWriter writer;
			ScriptManager scriptManager;
			ClassMapping classMapping;
			int actual;

			while (!_isImportCancelled && _currentClassMappingIndex < _package.CheckedClassMappings.Count)
			{
				classMapping = (ClassMapping) _package.CheckedClassMappings[_currentClassMappingIndex];

				scriptManager = new ScriptManager();

				// generate the scripts
				if ((actual = GenerateScripts(scriptManager, classMapping, _package.InstanceIdentifier)) > 0)
				{
					// convert the import scripts to a xml string to be sent to
					// the server
					builder = new StringBuilder();
					writer = new StringWriter(builder);
					scriptManager.Write(writer);

					int start = _currentRowIndex + 1;
					int end = start + actual - 1;
                    _workInProgressDialog.DisplayText = Newtera.WindowsControl.MessageResourceManager.GetString("DesignStudio.ImportRange") + " " + start + ":" + end;

					// invoke the web service synchronously
					xml = _dataService.ExecuteImportScripts(
						ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                        builder.ToString(), _currentBlock + _currentChunkIndex);

					reader = new StringReader(xml);
					scriptResult = new ScriptManager();
					scriptResult.Read(reader);
					// if there are errors, stop the import process and report error
					if (!scriptResult.IsSucceeded)
					{
						_scriptResults.Add(scriptResult);
						break;
					}

					_currentRowIndex += actual; // add up the current row index

					_currentChunkIndex++; // exporting next chunk
				}
				else
				{
					// finishing import all instances in a class, continue on
					// the next class in the package
					_currentChunkIndex = 0;
					_currentRowIndex = 0;
					_currentBlock = 0;
					_currentClassMappingIndex++;
				}
			}
		}

		/// <summary>
		/// Generate data import scripts for the given class and given chunk index
		/// </summary>
		/// <param name="scriptManager">The script manager</param>
		/// <param name="classMapping">The class mapping</param>
		/// <returns>Actual number of rows that have been generated in scripts</returns>
        private int GenerateScripts(ScriptManager scriptManager, ClassMapping classMapping, InstanceIdentifier instanceIdentifier)
		{
			ClassScript classScript;
			InstanceScript instanceScript;
			string query;

			int instanceCount = GetInstanceCount(classMapping.DestinationTableName, CurrentDestinationDataSet);
			int start = _currentChunkIndex * ImportWizard.CHUNK_SIZE;
			if (start >= instanceCount)
			{
				// finish importing a block of data rows, check to see if there are more blocks of data rows
				// need to be imported
				if (_package.IsPaging && CurrentConverter.SupportPaging)
				{
					this._currentBlock++; // read next block for the given class
					if (!ConvertSrcDataToDst(CurrentConverter, classMapping, _currentBlock))
					{
						return 0; // no more data blocks to import
					}

					instanceCount = GetInstanceCount(classMapping.DestinationTableName, CurrentDestinationDataSet);
					_currentChunkIndex = 0;
					start = 0;
				}
				else
				{
					return 0; // finish the importing data rows for the current class
				}
			}

			InstanceData instanceData = new InstanceData(classMapping.DestinationDataView,
				CurrentDestinationDataSet, true);

			classScript = new ClassScript(classMapping.DestinationClassName);
			scriptManager.AddClassScript(classScript);

			int end = start + ImportWizard.CHUNK_SIZE;
			if (end > instanceCount)
			{
				end = instanceCount;
			}

			for (int i = start; i < end; i++)
			{
				// Set the row index to InstanceData instance will cause
				// it to copy values of the DataRow of the DataSet to the
				// contained DataViewModel instance
				instanceData.RowIndex = i;

				instanceScript = new InstanceScript();
				
				// build the insert query using the DataViewModel instance
				query = classMapping.DestinationDataView.InsertQuery;
				instanceScript.InsertQuery = query;

				if (_package.ModifyExistingInstances)
				{
                    if (instanceIdentifier == InstanceIdentifier.PrimaryKeys)
                    {
                        // build the search query that can retrieve the instance
                        // based on primary key(s)
                        query = classMapping.DestinationDataView.GetInstanceByPKQuery();
                    }
                    else
                    {
                        // build the search query that can retrieve the instance
                        // based on unique key(s)
                        query = classMapping.DestinationDataView.GetInstanceByUniqueKeysQuery(classMapping.DestinationDataView.BaseClass.ClassName);
                    }

                    instanceScript.SearchQuery = query;

					// build the update query using the DataViewModel instance
					// since the obj_id is unknown at this time, we will place
					// a variable @obj_id in the update query for the time being,
					// the variable will be replaced at server side with an obj_id
					// retrieve by the GetInstanceByPKQuery.
					classMapping.DestinationDataView.CurrentObjId = InstanceScript.OBJ_ID;
					query = classMapping.DestinationDataView.UpdateQuery;
					instanceScript.UpdateQuery = query;
				}

				classScript.InstanceScripts.Add(instanceScript);
			}

			return (end - start);
		}

        /// <summary>
        /// Gets an xquery for inserting a data instance to the current destination class.
        /// This xquery is used to generate reusable sql actions in order to perform fast imports
        /// </summary>
        /// <param name="classMapping">The class mapping</param>
        /// <returns>An xquery</returns>
        private string GetInsertQuery(ClassMapping classMapping)
        {
            InstanceData instanceData = new InstanceData(classMapping.DestinationDataView,
                CurrentDestinationDataSet, true);

            // Set the row index to InstanceData instance will cause
            // it to copy values of the DataRow of the DataSet to the
            // contained DataViewModel instance
            instanceData.RowIndex = 0;

            // build the insert query using the DataViewModel instance
            return classMapping.DestinationDataView.InsertQuery;
        }

        /// <summary>
        /// Get a dataset of the next block
        /// </summary>
        /// <param name="classMapping">The class mapping</param>
        /// <returns>A DataSet object</returns>
        private DataSet GetNextDataSet(ClassMapping classMapping)
        {
            // Check to see if there are more blocks of data rows
            // need to be imported
            if (_package.IsPaging && CurrentConverter.SupportPaging)
            {
                this._currentBlock++; // read next block for the given class
                if (!ConvertSrcDataToDst(CurrentConverter, classMapping, _currentBlock))
                {
                    return null; // no more data blocks to import
                }
            }
            else
            {
                return null; // finish the importing data rows for the current class
            }

            return CurrentDestinationDataSet;
        }

		private delegate void ShowScriptResultDelegate(ScriptNodeCollection scriptResults);

		/// <summary>
		/// Display the scripts execution result
		/// </summary>
		/// <param name="scriptResults">A collection of script executing results</param>
		private void ShowScriptResult(ScriptNodeCollection scriptResults)
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue
				if (!HasErrors(scriptResults))
				{
					if (this.saveImportPackageCheckBox.Checked)
					{
						// finish the data import, now take care of saving package
						if (!_metaData.MappingManager.IsPackageExist(_package.Name))
						{
							// add the package to the mapping manager
							_metaData.MappingManager.AddMappingPackage(_package);
						}

						SaveImportPackage();
					}
				}
				else
				{
					// show the error messages
					ShowImportErrorMessages(scriptResults);
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowScriptResultDelegate showScriptResult = new ShowScriptResultDelegate(ShowScriptResult);

				this.BeginInvoke(showScriptResult, new object[] {scriptResults});
			}
		}

        /// <summary>
        /// Perform fast imports by generating SQL statements directly. It will skip the process
        /// of parsing XQuery statements, and also the validations during the paring stage, therefore
        /// is much faster. Recommend only for importing a huge data file.
        /// </summary>
        private void PerformFastImports()
        {
            string xquery;
            _scriptResults = new ScriptNodeCollection();

            if (_package.CheckedClassMappings.Count == 0)
            {
                return;
            }

            ClassMapping classMapping = (ClassMapping)_package.CheckedClassMappings[0];

            // initialize the counters
            _currentRowIndex = 0;
            _currentClassMappingIndex = 0;
            _currentBlock = 0;

            xquery = GetInsertQuery(classMapping);

            // execute the first chunk of import script
            _isRequestComplete = false;
            _isImportCancelled = false;

            _dataService = new CMDataServiceStub();

            // invoke the web service asynchronously
            string importId = _dataService.BeginImport(
                ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                xquery);

            ExecuteFastImportsDone(importId);
        }

        /// <summary>
        /// The AsyncCallback event handler for ExecuteFastImportDone web service method.
        /// </summary>
        /// <param name="res">The result</param>
        private void ExecuteFastImportsDone(string importId)
        {
            try
            {
                // Get the id of sql actions ached on the server side

                // import the first block of data instances to the destination class
                _dataService = new CMDataServiceStub();

                ScriptManager scriptResult = ImportInitialDataSet(importId);

                // If there are errors, stop the importing process
                if (scriptResult.IsSucceeded)
                {
                    // this is the worker process, execute the rest of import task
                    // synchronously

                    // import all source files until it is done or cancelled
                    while (_currentFileIndex < _fileNames.Length)
                    {
                        // continue to import the data in the current source file
                        FastImportDataOfCurrentFile(importId);

                        if (_isImportCancelled)
                        {
                            // process is cancelled, stop now
                            break;
                        }
                        else
                        {
                            // finishing data in one file, continue on
                            // the next file if it exists
                            _currentBlock = 0;
                            _currentClassMappingIndex = 0;
                            _currentRowIndex = 0;
                            _currentFileIndex++; // move to the next file

                            if (_currentFileIndex < _fileNames.Length)
                            {
                                // create a new import id for the data instances from current file
                                importId = GetImportId();

                                scriptResult = ImportInitialDataSet(importId);
                                if (!scriptResult.IsSucceeded)
                                {
                                    _scriptResults.Add(scriptResult);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    _scriptResults.Add(scriptResult);
                }

                ShowScriptResult(_scriptResults);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // always Close the converters
                CleanupSrcConverters();
            }
        }

        private ScriptManager ImportInitialDataSet(string importId)
        {
            ClassMapping classMapping = (ClassMapping)_package.CheckedClassMappings[_currentClassMappingIndex];

            int count = GetInstanceCount(classMapping.DestinationTableName, CurrentDestinationDataSet);
            if (count > MAX_INSTANCE_COUNT)
            {
                throw new Exception(string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("DesignStudio.TooBigForFastImport"), MAX_INSTANCE_COUNT));
            }

            _workInProgressDialog.DisplayText = Newtera.WindowsControl.MessageResourceManager.GetString("DesignStudio.ImportRange") + " " + 1 + ":" + count;

            string xml = _dataService.ImportNext(ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                importId, classMapping.DestinationClassName, CurrentDestinationDataSet.GetXml(), _currentBlock);

            _currentRowIndex += count;

            // read the xml that contains script execution status and detail messages
            StringReader reader = new StringReader(xml);
            ScriptManager scriptResult = new ScriptManager();
            scriptResult.Read(reader);

            return scriptResult;
        }

        /// <summary>
        /// Import the data instances converted from the source file that is being currently
        /// processed using fast import method. It method will exit either when all data have
        /// been imported or the process is cancelled by the user.
        /// </summary>
        /// <param name="importId">The import id</param>
        private void FastImportDataOfCurrentFile(string importId)
        {
            string xml;
            StringReader reader;
            ScriptManager scriptResult;
            ClassMapping classMapping;
            int count;
            string currentImportId = importId;

            while (!_isImportCancelled && _currentClassMappingIndex < _package.CheckedClassMappings.Count)
            {
                classMapping = (ClassMapping)_package.CheckedClassMappings[_currentClassMappingIndex];

                DataSet ds = GetNextDataSet(classMapping);
                if (ds != null)
                {
                    count = GetInstanceCount(classMapping.DestinationTableName, ds);
                    if (count > MAX_INSTANCE_COUNT)
                    {
                        throw new Exception(string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("DesignStudio.TooBigForFastImport"), MAX_INSTANCE_COUNT));
                    }

                    int start = _currentRowIndex + 1;
                    int end = start + count - 1;
                    _workInProgressDialog.DisplayText = Newtera.WindowsControl.MessageResourceManager.GetString("DesignStudio.ImportRange") + " " + start + ":" + end;

                    // invoke the web service synchronously
                    xml = _dataService.ImportNext(
                        ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                        currentImportId, classMapping.DestinationClassName, ds.GetXml(), _currentBlock);

                    reader = new StringReader(xml);
                    scriptResult = new ScriptManager();
                    scriptResult.Read(reader);
                    // if there are errors, stop the import process and report error
                    if (!scriptResult.IsSucceeded)
                    {
                        _scriptResults.Add(scriptResult);
                        break;
                    }

                    _currentRowIndex += count; // add up the current row index
                }
                else
                {
                    // finishing import all instances in a class, continue on
                    // the next class in the package
                    _dataService.EndImport(currentImportId);

                    _currentRowIndex = 0;
                    _currentBlock = 0;
                    _currentClassMappingIndex++;

                    if (_currentClassMappingIndex < _package.CheckedClassMappings.Count)
                    {
                        // invoke the web service synchronously
                        currentImportId = GetImportId();
                    }
                }
            }
        }

        private string GetImportId()
        {
            // Destination class changed, we need to get a different import id
            ClassMapping classMapping = (ClassMapping)_package.CheckedClassMappings[_currentClassMappingIndex];

            string xquery = GetInsertQuery(classMapping);
            // invoke the web service synchronously
            return _dataService.BeginImport(
                ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                xquery);
        }

		/// <summary>
		/// Show an dialog that display import errors
		/// </summary>
		/// <param name="scriptResults">A collection of script results that contains error messages.</param>
		private void ShowImportErrorMessages(ScriptNodeCollection scriptResults)
		{
			ImportErrorDialog dialog = new ImportErrorDialog();

			dialog.MetaData = this._metaData;
			dialog.ScriptResults = scriptResults;

			dialog.ShowDialog();

			// go back to the previous page
			this.importWizardForm.PageIndex = this.importWizardForm.PageIndex - 1;
		}

		/// <summary>
		/// Gets information indicating whether the script results contains any errors
		/// </summary>
		/// <param name="scriptResults">A collection of script results</param>
		/// <returns>true if there are errors, false otherwise.</returns>
		private bool HasErrors(ScriptNodeCollection scriptResults)
		{
			bool status = false;

			foreach (ScriptManager scriptManager in scriptResults)
			{
				if (!scriptManager.IsSucceeded)
				{
					status = true;
					break;
				}
			}

			return status;
		}

		/// <summary>
		/// Convert source data of the given block to destination data of a given class
		/// </summary>
		/// <param name="converter">The data converter</param>
		/// <param name="classMapping">The class mapping</param>
		/// <param name="currentBlock">The current block index</param>
		/// <returns>true if a block of data has been convetered, false if there no more data blocks left.</returns>
		private bool ConvertSrcDataToDst(IDataSourceConverter converter, ClassMapping classMapping, int currentBlock)
		{
			bool status = true;

			string msg = String.Format(ImportExportResourceManager.GetString("Import.NextBlock"), this._currentBlock + 1);
			this._workInProgressDialog.DisplayText = msg;

			// convert source data of given block to DataSet
			int start = currentBlock * _package.BlockSize;
			DataSet sourceDataSet = converter.ConvertNextPage();

			if (sourceDataSet != null)
			{
                // add missing columns to the data table in case the source file contains a subset
                // of the parameters required by the mapping.
                if (_hasMissingColumns)
                {
                    AddMissingColumns(classMapping, sourceDataSet);
                }

				// transform the source data to destination data
                DataSet destinationDataSet = _package.Transform(classMapping, sourceDataSet, GetToolBinDir());

				// replace the DataTable in _detinationDataSet
				CurrentDestinationDataSet.Tables.Remove(classMapping.DestinationTableName);
				DataTable copiedTable = destinationDataSet.Tables[classMapping.DestinationTableName].Copy();
				CurrentDestinationDataSet.Tables.Add(copiedTable);
			}
			else
			{
				status = false;
			}

			return status;
		}

        /// <summary>
        /// pick the converter automatically
        /// </summary>
        /// <param name="dataSourceType">One of the DataSourceType enum values.</param>
        private void AutoPickConverter(DataSourceType dataSourceType)
        {
            IDataSourceConverter converter;
            DataSet srcDataSet;
            bool found = false;
            int maxCols = 0;
            int index = 0;
            ConfigKeyValueCollection converters = null;
            if (dataSourceType == DataSourceType.Text)
            {
                converters = _textConverters;
            }
            else if (dataSourceType == DataSourceType.Excel)
            {
                converters = _excelConverters;
            }
            else if (dataSourceType == DataSourceType.Other)
            {
                converters = _otherConverters;
            }

            // Search for the converter that is suitable for the source file
            for (int i = 0; i < converters.Count; i++)
            {
                string converterTypeName = converters.Get(i);

                // convert the data and show the converted data in a datagrid
                try
                {
                    converter = ConverterFactory.Instance.Create(converterTypeName);
                }
                catch (Exception)
                {
                    converter = null;
                }

                if (converter != null)
                {
                    srcDataSet = converter.Convert(CurrentSourceFile);

                    if (srcDataSet.Tables.Count > 0 &&
                        srcDataSet.Tables[0].Columns.Count > 0)
                    {
                        found = true;

                        if (srcDataSet.Tables[0].Columns.Count > maxCols)
                        {
                            maxCols = srcDataSet.Tables[0].Columns.Count;
                            index = i;
                        }
                    }
                }
            }


            if (!found)
            {
                MessageBox.Show(ImportExportResourceManager.GetString("Info.NoMatchedConverter"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // make converter combo box display the selected converter
                if (dataSourceType == DataSourceType.Text)
                {
                    this.textConverterComboBox.SelectedIndex = index;
                }
                else if (dataSourceType == DataSourceType.Excel)
                {
                    this.excelConverterComboBox.SelectedIndex = index;
                }
                else if (dataSourceType == DataSourceType.Other)
                {
                    this.otherConverterComboBox.SelectedIndex = index;
                }
            }
        }

        private void DeleteConverter(DataSourceType dataSourceType)
        {
            ComboBox converterComboBox = null;
            ConfigKeyValueCollection converters = null;
            string configSection = null;
            if (dataSourceType == DataSourceType.Text)
            {
                converterComboBox = this.textConverterComboBox;
                converters = this._textConverters;
                configSection = "textConverters";
            }
            else if (dataSourceType == DataSourceType.Excel)
            {
                converterComboBox = this.excelConverterComboBox;
                converters = this._excelConverters;
                configSection = "excelConverters";
            }
            else if (dataSourceType == DataSourceType.Other)
            {
                converterComboBox = this.otherConverterComboBox;
                converters = this._otherConverters;
                configSection = "otherConverters";
            }

            // confirm
            if (MessageBox.Show(ImportExportResourceManager.GetString("ImportWizard.DeleteConverter"),
                "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                if (converterComboBox.SelectedIndex >= 0)
                {
                    string converterName = (string)converterComboBox.Items[converterComboBox.SelectedIndex];
                    string converterTypeName = null;

                    if (converters != null)
                    {
                        // remove the converter from the cached object
                        converterTypeName = (string)converters.Get(converterName);
                        converters.Remove(converterName);
                    }

                    // clear the package reference
                    if (_package.ConverterTypeName != null &&
                        _package.ConverterTypeName == converterTypeName)
                    {
                        _package.ConverterTypeName = null;
                    }

                    // remove the converter from the combobox
                    converterComboBox.Items.Remove(converterName);
                    converterComboBox.Text = "";

                    // Remove the dll and xml associated with the converter
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string assemblyName = null;
                    if (converterTypeName != null)
                    {
                        int index = converterTypeName.IndexOf(",");
                        if (index > 0)
                        {
                            assemblyName = converterTypeName.Substring(index + 1).Trim();
                        }
                    }

                    if (assemblyName != null)
                    {
                        // delete the dll and xml files from the current dir
                        if (File.Exists(baseDir + assemblyName + ".dll"))
                        {
                            try
                            {
                                File.Delete(baseDir + assemblyName + ".dll");
                            }
                            catch (Exception)
                            {
                                // ignore if it failed to delete the dll
                                // may be the dll is running
                            }
                        }

                        if (File.Exists(baseDir + assemblyName + ".xml"))
                        {
                            try
                            {
                                File.Delete(baseDir + assemblyName + ".xml");
                            }
                            catch (Exception)
                            {
                                // ignore if it failed to delete the xml
                            }
                        }
                    }

                    // remove the converter from the config file
                    if (_config != null)
                    {
                        _config.RemoveSetting(configSection, converterName);

                        _config.Flush(); // write changes back to config file
                    }
                }
            }
        }

		/// <summary>
		/// Save the specified import package to database.
		/// </summary>
		internal void SaveImportPackage()
		{
			if (_metaData.MappingManager.IsAltered)
			{
				StringBuilder builder;
				StringWriter writer;
				string[] xmlStrings = new string[12];

				// schema 
				xmlStrings[0] = "";

				// data views
				xmlStrings[1] = "";

				// xacl policy
				xmlStrings[2] = "";

				// taxonomies
				xmlStrings[3] = "";

				// rules
				xmlStrings[4] = "";

				builder = new StringBuilder();
				writer = new StringWriter(builder);
				_metaData.MappingManager.Write(writer);
				// the sixth string is a xml string for mappings
				xmlStrings[5] = builder.ToString();

				// selectors
				xmlStrings[6] = "";

                // events
                xmlStrings[7] = "";

                // logging info
                xmlStrings[8] = "";

                // subscriber info
                xmlStrings[9] = "";

                // xml schema views
                xmlStrings[10] = "";

                // apis
                xmlStrings[11] = "";

                _isRequestComplete = false;

				if (_metaDataService == null)
				{
					_metaDataService = new MetaDataServiceStub();
				}

                // invoke the web service asynchronously
                DateTime modifiedTime = _metaDataService.SetMetaData(
					ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
					xmlStrings);

                _metaData.MappingManager.IsAltered = false;
                _metaData.SchemaInfo.ModifiedTime = modifiedTime;
                _metaData.SchemaModel.SchemaInfo.ModifiedTime = modifiedTime;
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
		/// Gets number of instances in a DataSet for a given class.
		/// </summary>
		/// <param name="className">The class name</param>
		/// <param name="dataSet">The DataSet instance</param>
		/// <returns>The number of instances</returns>
		private int GetInstanceCount(string className, DataSet dataSet)
		{
			DataTable dataTable = dataSet.Tables[className];

			if (dataTable != null)
			{
				return dataTable.Rows.Count;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Launch an instance dialog to edit instance of selected row
		/// </summary>
		private void EditInstance()
		{
			int rowIndex = classDataGrid.CurrentRowIndex;
			if (rowIndex >= 0)
			{
                NewInstanceDialog dialog = new NewInstanceDialog(_metaData);

				ClassMapping classMapping = (ClassMapping) _package.CheckedClassMappings[this.classComboBox.SelectedIndex];

				// Show a dialog to display the data of the currently selected row in the classDataGrid.
				dialog.InstanceView = new InstanceView(classMapping.DestinationDataView, CurrentDestinationDataSet, true);
				dialog.InstanceView.SelectedIndex = rowIndex;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					// save the values changed in the dialog back to the DataSet
					dialog.InstanceView.InstanceData.SaveValuesToDataSet();

					// Refresh the DataGrid to show the updated values
					classDataGrid.Refresh();
				}
			}
		}

		/// <summary>
		/// Validate the destination data
		/// </summary>
		/// <returns>true if the data is in good state, false otherwise.</returns>
		private bool ValidateDestinationData()
		{
			bool hasError = false;
			int errorLineIndex = -1;

			if (CurrentDestinationDataSet != null && classComboBox.SelectedIndex >= 0)
			{
				ClassMapping classMapping = (ClassMapping) _package.CheckedClassMappings[this.classComboBox.SelectedIndex];

				if (CurrentDestinationDataSet.Tables[classMapping.DestinationTableName] != null)
				{
					int instanceCount = CurrentDestinationDataSet.Tables[classMapping.DestinationTableName].Rows.Count;

					InstanceData instanceData = new InstanceData(classMapping.DestinationDataView,
						CurrentDestinationDataSet, true);

					// validate the instances in the currently displayed destination class
					for (int i = 0; i < instanceCount; i++)
					{
						// setting the row index will cause the instance data for
						// the row copied to the result attributes of the data view model
						instanceData.RowIndex = i;

						DataViewValidateResult result = classMapping.DestinationDataView.ValidateData();

                        // verify the validating doubts if any
                        if (result.HasDoubt)
                        {
                            // turn the doubts into errors if any doubts turn out to be errors
                            //ValidateDoubts(result, classMapping.DestinationDataView);
                        }

						int count = instanceData.GetANUM();
						if (result.HasError)
						{
							// HACK, we use the column of Attachment to indicate
							// there are validating error for the instance so that
							// the datagrid for the row will have an error icon
							// displayed.
							if (count == 0)
							{
								instanceData.IncreamentANUM();
							}

							hasError = true;
							if (errorLineIndex < 0)
							{
								errorLineIndex = i;
							}
						}
						else
						{
							if (count > 0)
							{
								// remove the error icon for the row
								instanceData.DecreamentANUM();
							}
						}
					}

					// refresh to display or remove the error icons on the corresponding rows
					classDataGrid.Refresh();

					if (hasError)
					{
						MessageBox.Show(ImportExportResourceManager.GetString("Error.InvalidData"),
							"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

						this.classDataGrid.ScrollToRow(errorLineIndex); // show the first error line in the data grid
					}
				}
			}
	
			return !hasError;
		}

		/// <summary>
		/// Display the settings for the default converter.
		/// </summary>
		private void DisplayDefaultConverterSettings()
		{
            if (_package.DataSourceType == DataSourceType.Text)
            {
                this.delimitedRadioButton.Checked = true;

                // show the settings defined in the package
                int index = 0;
                foreach (string str in rowDelimiters)
                {
                    if (_package.TextFormat.RowDelimiter == str)
                    {
                        this.rowDelimiterComboBox.SelectedIndex = index;
                        break;
                    }

                    index++;
                }

                this.startingDataRowTextBox.Text = _package.TextFormat.StartingDataRow.ToString();

                if (_package.TextFormat.IsFirstRowColumns)
                {
                    this.firstRowColumnNameCheckBox.Checked = true;
                }
                else
                {
                    this.firstRowColumnNameCheckBox.Checked = false;
                }

                index = 0;
                foreach (string str in colDelimiters)
                {
                    if (_package.TextFormat.ColumnDelimiter == str)
                    {
                        switch (index)
                        {
                            case 0:
                                // Tab
                                this.columnDelimiterRadioButton1.Checked = true;
                                break;
                            case 1:
                                // Comma
                                this.columnDelimiterRadioButton2.Checked = true;
                                break;
                            case 2:
                                // Semicolon
                                this.columnDelimiterRadioButton3.Checked = true;
                                break;
                            default:
                                break;
                        }

                        break;
                    }
                    index++;
                }

                if (index >= colDelimiters.Length)
                {
                    // other column delimiter
                    this.otherColumnDelimiterTextBox.Text = _package.TextFormat.ColumnDelimiter; // Note: do not change the order
                    this.columnDelimiterRadioButton4.Checked = true;
                }
            }
		}

		/// <summary>
		/// Show the custom converter used in an import package.
		/// </summary>
		private void DisplayCustomConverter()
		{
            string converterName;
            if (_package.DataSourceType == DataSourceType.Text)
            {
			    for (int i = 0; i < _textConverters.Count; i++)
			    {
				    converterName = _textConverters[i];
				    if (converterName == _package.ConverterTypeName)
				    {
					    // make converter combo box display the corresponding
					    // converter's name
                        this.textConverterComboBox.SelectedIndex = i;
                    }
			    }

                this.freeFormedRadioButton.Checked = true;
			}
            else if (_package.DataSourceType == DataSourceType.Excel)
            {
                for (int i = 0; i < _excelConverters.Count; i++)
                {
                    converterName = _excelConverters[i];
                    if (converterName == _package.ConverterTypeName)
                    {
                        // make converter combo box display the corresponding
                        // converter's name
                        this.excelConverterComboBox.SelectedIndex = i;
                    }
                }

                this.specialConveterRadioButton.Checked = true;
            }
            else if (_package.DataSourceType == DataSourceType.Other)
            {
                for (int i = 0; i < _otherConverters.Count; i++)
                {
                    converterName = _otherConverters[i];
                    if (converterName == _package.ConverterTypeName)
                    {
                        // make converter combo box display the corresponding
                        // converter's name
                        this.otherConverterComboBox.SelectedIndex = i;
                    }
                }
            }
		}

		/// <summary>
		/// Show the paging definitions defined in a package.
		/// </summary>
		private void SetPagingMode()
		{
			if (_package.IsPaging)
			{
				this.dataRangeRadioButton2.Checked = true;
                this.readExcelInBlockRadioButton.Checked = true;
			}
			else
			{
				this.dataRangeRadioButton1.Checked = true;
                this.readExcelAllRadioButton.Checked = true;
			}

			this.blockSizeTextBox.Text = _package.BlockSize.ToString();
            this.readExcelBlockSizeTextBox.Text = _package.BlockSize.ToString();
		}

		/// <summary>
		/// Create a new import package
		/// </summary>
		private void CreateNewImportPackage()
		{
			// create a new package and restore to default settings
			_package = new MappingPackage(_dataSourceType);

			// set the default converter type for the given data source type
			_package.ConverterTypeName = _package.GetDefaultConverterTypeName(_dataSourceType);
			_package.TextFormat.RowDelimiter = rowDelimiters[0];
			_package.TextFormat.ColumnDelimiter = colDelimiters[0];
			_package.TextFormat.IsFirstRowColumns = false;
            _package.TextFormat.StartingDataRow = 1;

			this.delimitedRadioButton.Checked = true;
			this.textConverterComboBox.Text = null;
			this.rowDelimiterComboBox.SelectedIndex = 0;
			this.firstRowColumnNameCheckBox.Checked = false;
			this.columnDelimiterRadioButton1.Checked = true;
			this.importPackageNameTextBox.Text = "";
			this.importPackageDescTextBox.Text = "";
			this.dataRangeRadioButton1.Checked = true;
			this.otherColumnDelimiterTextBox.Text = "";
			this.blockSizeTextBox.Text = BLOCK_SIZE.ToString();
			if (_metaData.IsLockObtained)
			{
				this.saveImportPackageCheckBox.Checked = true;
			}
			_currentBlock = 0;
		}

		// add converters to the available list
		private void AddConverters(ArrayList converterInfos, DataSourceType dataSourceType)
		{
			ArrayList addedConverters = new ArrayList();
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			bool added = true;
            ConfigKeyValueCollection converters = null;
            if (dataSourceType == DataSourceType.Text)
            {
                converters = _textConverters;
            }
            else if (dataSourceType == DataSourceType.Excel)
            {
                converters = _excelConverters;
            }
            else if (dataSourceType == DataSourceType.Other)
            {
                converters = _otherConverters;
            }

			// check if the assembly alreay exist in the current directory
            foreach (ConverterInfo info in converterInfos)
			{
				added = true;
				// check if a converter with the same name has already exists
				if (converters.Get(info.ConverterName) != null)
				{
                    if (MessageBox.Show(info.ConverterName + " " + Newtera.WindowsControl.MessageResourceManager.GetString("Import.ConverterExists"),
						"Info Diaog",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Information) == DialogResult.No)
					{
						added = false;
					}
					else
					{
						info.IsAdd = false;
						info.IsUpdate = true;
					}
				}
				else if (File.Exists(baseDir + info.AssemblyDLLName))
				{
					// check if the assembly has already exists
                    if (MessageBox.Show(info.AssemblyDLLName + " " + Newtera.WindowsControl.MessageResourceManager.GetString("Import.AssemblyExists"),
						"Info Diaog",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Information) == DialogResult.No)
					{
						added = false;
					}
				}
				
				if (added)
				{
					addedConverters.Add(info);
				}
			}

			foreach (ConverterInfo info in addedConverters)
			{
				// copy the dll and xml files to the current dir
				if (File.Exists(info.AssemblyDLLFullPath))
				{
					File.Copy(info.AssemblyDLLFullPath,
						baseDir + info.AssemblyDLLName,
						true);
				}

				if (File.Exists(info.SettingXMLFileFullPath))
				{
					File.Copy(info.SettingXMLFileFullPath,
						baseDir + info.SettingXMLFileName,
						true);
				}

				// add the converter name to the combo box list
				if (info.IsAdd)
				{
                    if (dataSourceType == DataSourceType.Text)
                    {
                        this.textConverterComboBox.Items.Add(info.ConverterName);
                    }
                    else if (dataSourceType == DataSourceType.Excel)
                    {
                        this.excelConverterComboBox.Items.Add(info.ConverterName);
                    }
                    else if (dataSourceType == DataSourceType.Other)
                    {
                        this.otherConverterComboBox.Items.Add(info.ConverterName);
                    }
				}

				// add the converter to the _textConverters list
				string converterString = info.ClassName + "," + info.AssemlyName;

				if (info.IsAdd)
				{
					converters.Add(info.ConverterName, converterString);
				}
				else if (info.IsUpdate)
				{
					converters[info.ConverterName] = converterString;
				}

				// update the configuration file with the newly added converters
                if (dataSourceType == DataSourceType.Text)
                {
                    _config.SetSetting("textConverters", info.ConverterName, converterString);
                }
                else if (dataSourceType == DataSourceType.Excel)
                {
                    _config.SetSetting("excelConverters", info.ConverterName, converterString);
                }
                else if (dataSourceType == DataSourceType.Other)
                {
                    _config.SetSetting("otherConverters", info.ConverterName, converterString);
                }
			}

			if (addedConverters.Count > 0)
			{
				_config.Flush(); // write the changes to the config file
			}
		}

		/// <summary>
		/// gets the information indicating whether all the source data set have
		/// the same format
		/// </summary>
		/// <returns>true if they have the same format. False otherwise.</returns>
		private bool AreSourceFormatSame()
		{
			bool status = true;

			if (this._sourceDataSets.Length > 1)
			{
				DataSet dataSet1 = this._sourceDataSets[0];
				DataSet dataSet2;

				if (dataSet1 == null || dataSet1.Tables.Count == 0 ||
					dataSet1.Tables[0].Columns.Count == 0)
				{
					status = false;
				}
				else
				{
					// compare using the first DataSet as the base
					for (int i = 1; i < this._sourceDataSets.Length; i++)
					{
						dataSet2 = this._sourceDataSets[i];

						if (dataSet2 == null)
						{
							status = false;
							break;
						}
						else if (dataSet2.Tables.Count != 1)
						{
							status = false;
							break;
						}
						else if (dataSet1.Tables[0].Columns.Count != dataSet2.Tables[0].Columns.Count)
						{
							status = false;
							break;
						}
						else
						{
							// compare the column name
							for (int j = 0; j < dataSet1.Tables[0].Columns.Count; j++)
							{
								if (dataSet1.Tables[0].Columns[j].ColumnName !=
									dataSet2.Tables[0].Columns[j].ColumnName)
								{
									status = false;
									break;
								}
							}

							if (!status)
							{
								break;
							}
						}
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Since the converters that support paging may have held file open,
		/// we need to close the converters
		/// </summary>
		private void CleanupSrcConverters()
		{
			if (_srcConverters != null)
			{
				for (int i = 0; i < _srcConverters.Length; i++)
				{
					if (_srcConverters[i] != null)
					{
						_srcConverters[i].Close();
						_srcConverters[i] = null;
					}
				}

				_srcConverters = null;
			}
		}

		/// <summary>
		/// Create an class mapping to the package that has the same source data
		/// as the selected class mapping
		/// </summary>
		/// <param name="selectedIndex">The index of the selected class mapping shown in the class map list view.</param>
		private void CreateClassMapping(int selectedIndex)
		{
			ListViewItem listViewItem;

			ClassMapping selectedClassMapping = (ClassMapping) this._package.ClassMappings[selectedIndex];

			// create a new class mapping with the same source class name as the selected one's
			ClassMapping clsMapping = _package.AddClassMapping(selectedClassMapping.SourceClassName, "");
			clsMapping.SourceClassIndex = selectedClassMapping.SourceClassIndex; // point to the same source data table

			// Create list view item to show the new mapping.
			listViewItem = new ListViewItem(selectedClassMapping.SourceClassName);
			listViewItem.Checked = true;
			listViewItem.SubItems.Add(ImportExportResourceManager.GetString("ImportWizard.SelectDstClass"));
			listViewItem.SubItems.Add("...");
			this.classMapListView.Items.Add(listViewItem);
		}

        /// <summary>
        /// validate the doubts raised by validating data process
        /// </summary>
        /// <param name="validateResult"></param>
        /// <param name="destinationDataView"></param>
        private void ValidateDoubts(DataViewValidateResult validateResult, DataViewModel destinationDataView)
        {
            foreach (DataValidateResultEntry doubt in validateResult.Doubts)
            {
                if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.PrimaryKey)
                {
                    if (IsPKValueExists(destinationDataView))
                    {
                        // the primary key value exists, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValue)
                {
                    if (!IsValueUnique(destinationDataView))
                    {
                        // the value isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueReference)
                {
                    if (!IsReferenceUnique(destinationDataView, doubt.DataViewElement))
                    {
                        // the value isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValues)
                {
                    if (!IsCombinedValuesUnique(destinationDataView, doubt.ClassName))
                    {
                        // the combination of values isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the primary key value are already used by
        /// another instance in the database
        /// </summary>
        /// <returns>True if it's been used, false otherwise</returns>
        private bool IsPKValueExists(DataViewModel destinationDataView)
        {
            bool status = false;

            CMDataServiceStub dataService = new CMDataServiceStub();

            string query = destinationDataView.GetInstanceByPKQuery();
            if (query != null)
            {
                // invoke the web service synchronously
                XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
                    query);

                DataSet ds = new DataSet();

                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                // if the result isn't empty, the instance with the same primary key value exists
                if (!DataSetHelper.IsEmptyDataSet(ds, destinationDataView.BaseClass.ClassName))
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the indivated value are unique among the
        /// same class.
        /// </summary>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsValueUnique(DataViewModel destinationDataView)
        {
            bool status = true;

            // to be implemented

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether a combination of values is unique among the
        /// same class.
        /// </summary>
        /// <param name="destinationDataView">The destination data view</param>
        /// <param name="className">The name of the class uniquely constrainted.</param>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsCombinedValuesUnique(DataViewModel destinationDataView, string className)
        {
            bool status = true;

            CMDataServiceStub dataService = new CMDataServiceStub();

            string query = destinationDataView.GetInstanceByUniqueKeysQuery(className);
            if (query != null)
            {

                // invoke the web service synchronously
                XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                    query);

                DataSet ds = new DataSet();

                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                // if the result isn't empty, the instance with the same primary key value exists
                if (!DataSetHelper.IsEmptyDataSet(ds, destinationDataView.BaseClass.ClassName))
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
        private bool IsReferenceUnique(DataViewModel destinationDataView, IDataViewElement element)
        {
            bool status = true;

            DataRelationshipAttribute relationshipAttribute = element as DataRelationshipAttribute;

            if (relationshipAttribute != null && relationshipAttribute.HasValue)
            {
                CMDataServiceStub dataService = new CMDataServiceStub();

                string query = destinationDataView.GetInstancesQuery(element);
                if (query != null)
                {

                    // invoke the web service synchronously
                    XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        query);

                    DataSet ds = new DataSet();

                    XmlReader xmlReader = new XmlNodeReader(xmlNode);
                    ds.ReadXml(xmlReader);

                    // if the result isn't empty, there are instances that have reference to the same instance
                    if (!DataSetHelper.IsEmptyDataSet(ds, destinationDataView.BaseClass.ClassName))
                    {
                        status = false;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the converter specified by an import package
        /// still exist or not.
        /// </summary>
        /// <param name="importPackage">The import package</param>
        /// <returns>true if it exists, false otherwise.</returns>
        private bool IsConverterExist(MappingPackage package)
        {
            bool status = false;
            ConfigKeyValueCollection converters = null;

            if (package != null)
            {
                // compare with the default converter first
                if (package.GetDefaultConverterTypeName(_dataSourceType) == package.ConverterTypeName)
                {
                    status = true;
                }
                else
                {
                    // compare with the custom converters
                    if (package.DataSourceType == DataSourceType.Text)
                    {
                        converters = this._textConverters;
                    }
                    else if (package.DataSourceType == DataSourceType.Excel)
                    {
                        converters = this._excelConverters;
                    }
                    else if (package.DataSourceType == DataSourceType.Other)
                    {
                        converters = this._otherConverters;
                    }

                    if (converters != null)
                    {
                        string converterName;
                        for (int i = 0; i < converters.Count; i++)
                        {
                            converterName = converters[i];
                            if (converterName == package.ConverterTypeName)
                            {
                                status = true;
                                break;
                            }
                        }
                    }
                }
            }

            return status;
        }

		#endregion

		private void importWizardForm_Load(object sender, System.EventArgs e)
		{
			// Display localized text for data sources
			string[] dsTypes = Enum.GetNames(typeof(DataSourceType));
			if (dsTypes.Length > 1)
			{
				// start from index 1 to skip "Unknown" entry
				for (int i = 1; i < dsTypes.Length; i++)
				{
					string typeText = ImportExportResourceManager.GetString("DataSourceType." + dsTypes[i]);
					dataSourceComboBox.Items.Add(typeText);
				}

				// select the first entry by default
				dataSourceComboBox.SelectedIndex = 0;
			}
		
			// Load the text converters defined in the configuration file
			_config = new AppConfig();
			_textConverters = _config.GetConfig("textConverters");
            _excelConverters = _config.GetConfig("excelConverters");
            _otherConverters = _config.GetConfig("otherConverters");
			
			for (int i = 0; i < _textConverters.Count; i++)
			{
				this.textConverterComboBox.Items.Add(_textConverters.GetKey(i));
			}

            for (int i = 0; i < _excelConverters.Count; i++)
            {
                this.excelConverterComboBox.Items.Add(_excelConverters.GetKey(i));
            }

            for (int i = 0; i < _otherConverters.Count; i++)
            {
                this.otherConverterComboBox.Items.Add(_otherConverters.GetKey(i));
            }

			// disable the save import package check box if the lock to the 
			// meta-data has not been obtained
			if (_metaData.IsLockObtained)
			{
				saveImportPackageCheckBox.Enabled = true;
			}
			else
			{
				saveImportPackageCheckBox.Enabled = false;
				saveImportPackageCheckBox.Checked = false;
			}
		}

		private void dataSourceComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selectedIndex = dataSourceComboBox.SelectedIndex;
			if (selectedIndex >= 0)
			{
				// clear the file path
				this.dataSourceFileNameTextBox.Text = "";

				// display the select data source image and localized description
				this.selectedDataSourceImage.Image = this.dataSourceTypeImageList.Images[selectedIndex];
				
				string[] dsTypes = Enum.GetNames(typeof(DataSourceType));
				string typeName = dsTypes[selectedIndex + 1]; // skip the first entry (Unknown)
				string typeDesc = ImportExportResourceManager.GetString("DataSourceDescription." + typeName);
				this.selectedDataSourceText.Text = typeDesc;

				// display the predefined import packages for the selected data type
				_dataSourceType = (DataSourceType) Enum.Parse(typeof(DataSourceType), typeName);
			}
		}

		private void openFileButton_Click(object sender, System.EventArgs e)
		{
			int selectedIndex = dataSourceComboBox.SelectedIndex;
			if (selectedIndex >= 0)
			{		
				string[] dsTypes = Enum.GetNames(typeof(DataSourceType));
				string typeName = dsTypes[selectedIndex + 1]; // skip the first entry (Unknown)
				string typeFilter = ImportExportResourceManager.GetString("DataSourceFilter." + typeName);

				if (_openFileDialog == null)
				{
					_openFileDialog = new OpenFileDialog();
					//_openFileDialog.InitialDirectory = "c:\\" ;
					_openFileDialog.Filter = typeFilter;
					_openFileDialog.FilterIndex = 1;
					_openFileDialog.RestoreDirectory = true;
					_openFileDialog.Multiselect = true;
				}
				else
				{
					_openFileDialog.Filter = typeFilter;
				}

				DialogResult result = _openFileDialog.ShowDialog();
				if (result == DialogResult.OK)
				{				
					_fileNames = _openFileDialog.FileNames;
					_currentFileIndex = 0;

					string fileName;
					for (int i = 0; i < _fileNames.Length; i++)
					{
						fileName = _fileNames[i];
						if (File.Exists(fileName))
						{
							if (i == 0)
							{
								this.dataSourceFileNameTextBox.Text = fileName;
							}
							else
							{
								// separate the file names with comma
								this.dataSourceFileNameTextBox.Text += ";" + fileName;
							}
						}
						else
						{
							MessageBox.Show(ImportExportResourceManager.GetString("Error.UnknownFile") + ":" + fileName,
								"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}		
		}

		private void columnDelimiterRadioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			this._package.TextFormat.ColumnDelimiter = colDelimiters[0];
			this.otherColumnDelimiterTextBox.Enabled = false;
		}

		private void columnDelimiterRadioButton2_CheckedChanged(object sender, System.EventArgs e)
		{
			this._package.TextFormat.ColumnDelimiter = colDelimiters[1];
			this.otherColumnDelimiterTextBox.Enabled = false;
		}

		private void columnDelimiterRadioButton3_CheckedChanged(object sender, System.EventArgs e)
		{
			this._package.TextFormat.ColumnDelimiter = colDelimiters[2];
			this.otherColumnDelimiterTextBox.Enabled = false;
		}

		private void columnDelimiterRadioButton4_CheckedChanged(object sender, System.EventArgs e)
		{
			this._package.TextFormat.ColumnDelimiter = otherColumnDelimiterTextBox.Text;
			this.otherColumnDelimiterTextBox.Enabled = true;		
		}

		private void rowDelimiterComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.rowDelimiterComboBox.SelectedIndex >= 0)
			{
				this._package.TextFormat.RowDelimiter = rowDelimiters[this.rowDelimiterComboBox.SelectedIndex];
			}
		}

		private void firstRowColumnNameCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.firstRowColumnNameCheckBox.Checked)
			{
				this._package.TextFormat.IsFirstRowColumns = true;
                if (this.startingDataRowTextBox.Text.Trim() == "1")
                {
                    this.startingDataRowTextBox.Text = "2";
                }
			}
			else
			{
				this._package.TextFormat.IsFirstRowColumns = false;
                if (this.startingDataRowTextBox.Text.Trim() == "2")
                {
                    this.startingDataRowTextBox.Text = "1";
                }
			}
		}

		private void otherColumnDelimiterTextBox_TextChanged(object sender, System.EventArgs e)
		{
			this._package.TextFormat.ColumnDelimiter = this.otherColumnDelimiterTextBox.Text;
		}

		private void delimitedRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.delimitedRadioButton.Checked)
			{
				this._package.ConverterTypeName = _package.GetDefaultConverterTypeName(_dataSourceType);
			}
		}

		private void freeFormedRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
            if (this.freeFormedRadioButton.Checked)
            {
                if (this.textConverterComboBox.SelectedIndex >= 0)
                {
                    // get the selected converter name
                    this._package.ConverterTypeName = _textConverters[this.textConverterComboBox.SelectedIndex];
                }
                else
                {
                    this._package.ConverterTypeName = null;
                }
            }
		}

		private void importWizardForm_Next(object sender, Newtera.Studio.UserControls.Wizards.WizardForm.EventNextArgs e)
		{
			Newtera.Studio.UserControls.Wizards.WizardPageBase page =
				(Newtera.Studio.UserControls.Wizards.WizardPageBase) sender;

			if (page.Equals(this.determineFileFormatPage) == true ||
                page.Equals(this.excelFormatPage) == true ||
                page.Equals(this.otherFormatPage) == true)
			{
				// convert the data and show the converted data in a datagrid
				try
				{
					if (_package.ConverterTypeName != null)
					{
						// close the previous converters if they have not been closed
						CleanupSrcConverters();
						
						// we need a separate converter for each source file since
						// each converter may keep states
						_srcConverters = new IDataSourceConverter[this._fileNames.Length];
						for (int i = 0; i < _srcConverters.Length; i++)
						{
							_srcConverters[i] = ConverterFactory.Instance.Create(_package.ConverterTypeName);
				
							if (_srcConverters[i] is DelimitedTextFileConverter)
							{
								((DelimitedTextFileConverter) _srcConverters[i]).RowDelimiter = _package.TextFormat.RowDelimiter;
								((DelimitedTextFileConverter) _srcConverters[i]).ColumnDelimiter = _package.TextFormat.ColumnDelimiter;
								((DelimitedTextFileConverter) _srcConverters[i]).IsFirstRowColumns = _package.TextFormat.IsFirstRowColumns;
                                ((DelimitedTextFileConverter)_srcConverters[i]).StartingDataRow = _package.TextFormat.StartingDataRow;
							}
						}

						_sourceDataSets = new DataSet[_fileNames.Length];
						if (_package.IsPaging && CurrentConverter.SupportPaging)
						{
							// convert the data in paging mode, convert the first page of data
							// remember the converter
							try
							{
								_package.BlockSize = int.Parse(this.blockSizeTextBox.Text);
							}
							catch (Exception)
							{
							}

							_currentBlock = 0;
							for (int i = 0; i < _fileNames.Length; i++)
							{
								//_sourceDataSets[i] = CurrentConverter.ConvertFirstPage(_fileNames[i], _package.BlockSize);
                                _sourceDataSets[i] = this._srcConverters[i].ConvertFirstPage(_fileNames[i], _package.BlockSize);
							}
						}
						else
						{
							for (int i = 0; i < _fileNames.Length; i++)
							{
								// read all rows
								//_sourceDataSets[i] = CurrentConverter.Convert(_fileNames[i]);
                                _sourceDataSets[i] = this._srcConverters[i].Convert(_fileNames[i]);
							}
						}

						// show the selected text files in the combo box
						this.previewTextFileComboBox.DataSource = this._fileNames;
						this.previewTextFileComboBox.SelectedIndex = -1; // make sure SelectedIndexChange event fired
						this.previewTextFileComboBox.SelectedIndex = 0;

                        if (_package.DataSourceType == DataSourceType.Text)
                        {
                            e.Step = 3; // go to the preview page
                        }
                        else if (_package.DataSourceType == DataSourceType.Excel)
                        {
                            e.Step = 2; // go to the preview page
                        }
					}
					else
					{
						MessageBox.Show(ImportExportResourceManager.GetString("Error.NoConverterSelected"),
							"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						e.Step = 0;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ImportExportResourceManager.GetString("Error.InvalidConverter") + ":" + ex.Message,
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Step = 0;
				}
			}
			else if (page.Equals(this.chooseDataSourcePage) == true)
			{
				string packageName = this.choosenImportPackageTextBox.Text;
				if (packageName == null || packageName.Length == 0)
				{
					CreateNewImportPackage();
				}
				else if (_metaData.MappingManager.GetMappingPackage(packageName) != null)
				{
                    // make a copy since it may be modified
					_package = (MappingPackage)_metaData.MappingManager.GetMappingPackage(packageName).Copy();

					this.importPackageNameTextBox.Text = _package.Name;
					this.importPackageDescTextBox.Text = _package.Description;
					this.saveImportPackageCheckBox.Checked = false;
                    this.noValidationCheckBox.Checked = !_package.NeedValidation;

					if (_package.ConverterTypeName == _package.GetDefaultConverterTypeName(_dataSourceType))
					{
						// the package is using the default converter of the current data source type
						// set the default converter's settings
						DisplayDefaultConverterSettings();
					}
					else
					{
						// the package is using a custom converter
						// show the converter name
						DisplayCustomConverter();
					}

					SetPagingMode();
				}
				else
				{
					MessageBox.Show(ImportExportResourceManager.GetString("Error.UnknownPackage") + packageName,
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Step = 0; // goback to select a data source				
				}

				if (CurrentSourceFile != null && CurrentSourceFile.Length > 0)
				{
				    // check the size of the data files, if it is a large file
				    // ask to use the paging mode to process the data
				    foreach (string fileName in this._fileNames)
				    {
					    FileInfo fileInfo = new FileInfo(fileName);
					    long fileSize = fileInfo.Length / (1024 * 1000);
					    if (fileSize > FILE_SIZE_WARNING_THRESHHOLD)
					    {
						    string msg = String.Format(ImportExportResourceManager.GetString("Import.LargeFile"), fileSize);
						    MessageBox.Show(msg,
							    "Warning",
							    MessageBoxButtons.OK,
							    MessageBoxIcon.Warning);
					    }
				    }

                    if (_package.DataSourceType == DataSourceType.Excel)
                    {
                        e.Step = 2; // go to the excel conveter selection page
                    }
                    else if (_package.DataSourceType == DataSourceType.Other)
                    {
                        e.Step = 3; // go to the converter selection for other file types
                    }
				}
				else
				{
					MessageBox.Show(ImportExportResourceManager.GetString("Error.UnspecifiedFile"),
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Step = 0; // goback to select a data source				
				}
			}
			else if (page.Equals(this.previewDataPage) == true)
			{
				if (AreSourceFormatSame())
				{
					DisplayClassMappings();
				}
				else
				{
					MessageBox.Show(ImportExportResourceManager.GetString("Error.SourceFormatDifferent"),
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Step = 0; // goback to preview source	data
				}
			}
			else if (page.Equals(this.specifyMappingPage) == true)
			{
				try
				{
					DisplayTransformedData();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

					e.Step = 0; // goback to specifyMappingPage
				}
			}
			else if (page.Equals(this.previewPostTransformPage) == true)
			{
				bool hasErrors = false;

				// validate the transformed data first
				if (!ValidateDestinationData())
				{
					// some instances have data validating errors
					hasErrors = true;
				}

				if (hasErrors)
				{
					// there are validating errors, go back to preview transformed data page
					e.Step = 0;
				}
				else
				{
					if (_metaData.MappingManager.IsAltered &&
						!this.saveImportPackageCheckBox.Checked &&
						_metaData.IsLockObtained)
					{
						this.saveImportPackageCheckBox.Checked = true;
					}

                    if (_package.ModifyExistingInstances)
                    {
                        this.includeUpdateCheckBox.Checked = true;
                    }
                    else
                    {
                        this.includeUpdateCheckBox.Checked = false;
                    }

                    if (_package.InstanceIdentifier == InstanceIdentifier.PrimaryKeys)
                    {
                        this.pkRadioButton.Checked = true;
                        this.ukRadioButton.Checked = false;
                    }
                    else
                    {
                        this.pkRadioButton.Checked = false;
                        this.ukRadioButton.Checked = true;
                    }
				}
			}
			else if (page.Equals(this.runSaveImportPage) == true)
			{
				try
				{
					// verify the import package name
					if (this.saveImportPackageCheckBox.Checked)
					{
						string packageName = null;
                        if (this.importPackageNameTextBox.Text != null &&
                            this.importPackageNameTextBox.Text.Trim().Length > 0)
                        {
                            packageName = this.importPackageNameTextBox.Text.Trim();
                        }

						if (packageName == null)
						{
							MessageBox.Show(ImportExportResourceManager.GetString("Error.EmptyPackageName"),
								"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							e.Step = 0; // go back to runSaveImportPage
                            return;
						}
						else if (_metaData.MappingManager.IsPackageExist(packageName))
						{
                            // confirm
                            if (MessageBox.Show(ImportExportResourceManager.GetString("Error.DuplicatePackageName"),
                                "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                // Delete the existing package
                                _metaData.MappingManager.RemoveMappingPackage(packageName);
                            }
                            else
                            {
                                e.Step = 0; // go back to runSaveImportPage
                                return;
                            }
						}

						if (string.IsNullOrEmpty(_package.Name) || _package.Name != packageName)
						{
							_package.Name = packageName;
							_package.Description = this.importPackageDescTextBox.Text;
						}
					}

                    // save the flag of "no validation" in the packe
                    if (this.noValidationCheckBox.Checked)
                    {
                        _package.NeedValidation = false;
                    }
                    else
                    {
                        _package.NeedValidation = true;
                    }

                    // save the flag of "modify existing instances" in the packe
                    if (this.includeUpdateCheckBox.Checked)
                    {
                        _package.ModifyExistingInstances = true;
                    }
                    else
                    {
                        _package.ModifyExistingInstances = false;
                    }

                    // save the instance identifier type in the packe
                    if (this.pkRadioButton.Checked)
                    {
                        _package.InstanceIdentifier = InstanceIdentifier.PrimaryKeys;
                    }
                    else
                    {
                        _package.InstanceIdentifier = InstanceIdentifier.UniqueKeys;
                    }

					SaveDataOrImportPackage();
				}
				catch (Exception)
				{
					e.Step = 0; // go back to runSaveImportPage
				}
			}
		}

        private void classMaplistView_SubItemClicked(object sender, Newtera.WindowsControl.SubItemEventArgs e)
		{
			switch (e.SubItem)
			{
				case 1: // destination
					// select a destination class using a class tree dialog
					if (classMapListView.SelectedIndices.Count == 1)
					{
						int selectedIndex = classMapListView.SelectedIndices[0];
						ClassMapping selectedClassMapping = (ClassMapping) _package.ClassMappings[selectedIndex]; 
					
						ChooseImportClassDialog dialog = new ChooseImportClassDialog();
						dialog.RootClass = "ALL";
						dialog.MetaData = _metaData;
						dialog.IsDBAUser = this.IsDBAUser;
						dialog.Wizard = this;
						dialog.SourceDataTable = CurrentSourceDataSet.Tables[selectedClassMapping.SourceClassIndex];

						if (selectedClassMapping.DestinationClassName != null &&
							selectedClassMapping.DestinationClassName.Length > 0)
						{
							dialog.SelectedClass = (ClassElement) _metaData.SchemaModel.FindClass(selectedClassMapping.DestinationClassName);
						}

						if (dialog.ShowDialog() == DialogResult.OK)
						{
							ClassElement dstClassElement = dialog.SelectedClass;
				
							if (dstClassElement.IsLeaf)
							{
								if (dstClassElement.Name != selectedClassMapping.DestinationClassName)
								{					
									// it's better to call SetDesitinationName method to change the destination
									// because it also clears attribute mappings.
									_package.SetClassMappingDestination(selectedClassMapping, dstClassElement.Name);

									ListViewItem selectedListViewItem = classMapListView.Items[selectedIndex];
									selectedListViewItem.SubItems[1].Text = dstClassElement.Caption;
								}
							}
							else
							{
                                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.LeafClassRequired"),
									"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
							}
						}
					}
					
					break;
				case 2: // transform
					if (classMapListView.SelectedIndices.Count == 1)
					{
						int selectedIndex = classMapListView.SelectedIndices[0];
						ClassMapping selectedClassMapping = (ClassMapping) _package.ClassMappings[selectedIndex]; 
						if ( selectedClassMapping.DestinationClassName != null &&
							selectedClassMapping.DestinationClassName.Length > 0)
						{
							TransformDialog dialog = new TransformDialog();
							dialog.MetaData = _metaData;
							dialog.SourceDataTable = CurrentSourceDataSet.Tables[selectedClassMapping.SourceClassIndex];
							dialog.ClassMapping = selectedClassMapping;

							if (dialog.ShowDialog() == DialogResult.OK)
							{
								// get attribute mappings created by the dialog
								if (dialog.AttributeMappings.IsAltered)
								{
									selectedClassMapping.AttributeMappings = dialog.AttributeMappings;
								}

								// get default values created by the dialog
								if (dialog.DefaultValues.IsAltered)
								{
									selectedClassMapping.DefaultValues = dialog.DefaultValues;
								}
							}
						}
						else
						{
							string errMsg = ImportExportResourceManager.GetString("Error.MissingDestination");
							MessageBox.Show(errMsg,
								"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}

					break;
			}

			// an entry is selected, enable the "add entry"button
			this.addEntryButton.Enabled = true;
			this.viewSourceButton.Enabled = true;
		}


		private void classComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (CurrentDestinationDataSet != null && classComboBox.SelectedIndex >= 0)
			{
				ClassMapping classMapping = (ClassMapping) _package.CheckedClassMappings[classComboBox.SelectedIndex];

				if (CurrentDestinationDataSet.Tables[classMapping.DestinationTableName] != null)
				{
					// The data grid need a table style to display the data, check
					// if a table style has alreadt existed for the class, 
					// if no, create a new table style
					DataGridTableStyle tableStyle = classDataGrid.TableStyles[classMapping.DestinationClassName];
					if (tableStyle == null)
					{
						tableStyle = TableStyleFactory.Instance.Create(classMapping.DestinationDataView, this.dataGridImageList.Images[0]);
						classDataGrid.TableStyles.Add(tableStyle);					
					}

					DataTable dataTable = CurrentDestinationDataSet.Tables[classMapping.DestinationTableName];
					classDataGrid.DataSource = dataTable;
					_currentDstRowCount = dataTable.Rows.Count;
				}
			}
		}

		private void classDataGrid_Click(object sender, System.EventArgs e)
		{
			int rowIndex = classDataGrid.CurrentRowIndex;

			if (rowIndex >= 0)
			{
				this.editInstanceButton.Enabled = true;
			}
			else
			{
				this.editInstanceButton.Enabled = false;
			}
		}

		private void editInstanceButton_Click(object sender, System.EventArgs e)
		{
			EditInstance();
		}

		private void classDataGrid_DoubleClick(object sender, System.EventArgs e)
		{
			EditInstance();
		}

		private void textConverterComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            if (this.textConverterComboBox.SelectedIndex >= 0)
            {
                this.delConveterButton.Enabled = true;
                if (this.freeFormedRadioButton.Checked)
                {
                    _package.ConverterTypeName = _textConverters.Get(this.textConverterComboBox.SelectedIndex);
                }
            }
            else
            {
                this.delConveterButton.Enabled = false;
            }
		}

		private void validateDataButton_Click(object sender, System.EventArgs e)
		{
			if (ValidateDestinationData())
			{
				MessageBox.Show(ImportExportResourceManager.GetString("Info.DataIsValid"),
					"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void importWizardForm_Continue(object sender, Newtera.Studio.UserControls.Wizards.WizardForm.EventNextArgs e)
		{
			// clear the data source file text field
			this.dataSourceFileNameTextBox.Text = null;
		}

		private void pickConverterButton_Click(object sender, System.EventArgs e)
		{
            AutoPickConverter(DataSourceType.Text);
		}

		private void selectImportPackageBtn_Click(object sender, System.EventArgs e)
		{
			SelectImportPackageDialog dialog = new SelectImportPackageDialog();

			if (this.choosenImportPackageTextBox.Text != null &&
				this.choosenImportPackageTextBox.Text.Length > 0)
			{
				dialog.SelectedPackage = this.choosenImportPackageTextBox.Text;
			}

			dialog.DataSourceType = _dataSourceType;
			dialog.MappingManager = _metaData.MappingManager;
			dialog.ImportWizard = this;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
                MappingPackage package = _metaData.MappingManager.GetMappingPackage(dialog.SelectedPackage);

                if (IsConverterExist(package))
                {
                    this.choosenImportPackageTextBox.Text = dialog.SelectedPackage;
                }
                else
                {
                    // the required converter has been deleted on the local computer
                    string msg = string.Format(ImportExportResourceManager.GetString("Info.ConverterNotExist"), package.ConverterTypeName);
                    MessageBox.Show(msg,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.choosenImportPackageTextBox.Text = null;
                }
			}
		}

		private void wizardWelcomePage1_Load(object sender, System.EventArgs e)
		{
		
		}

		private void addConverterButton_Click(object sender, System.EventArgs e)
		{
			AddConverterDialog dialog = new AddConverterDialog();
            dialog.DataSourceType = DataSourceType.Text;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				AddConverters(dialog.SelectedConverters, DataSourceType.Text);
			}
		}

		private void dataRangeRadioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.dataRangeRadioButton1.Checked)
			{
				this._package.IsPaging = false;
				this.blockSizeTextBox.ReadOnly = true;
			}
			else
			{
				this._package.IsPaging = true;
				this.blockSizeTextBox.ReadOnly = false;
			}
		}

		private void dataRangeRadioButton2_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.dataRangeRadioButton1.Checked)
			{
				this.blockSizeTextBox.ReadOnly = true;
			}
			else
			{
				this.blockSizeTextBox.ReadOnly = false;
			}
		}

		private void chartPreviewButton_Click(object sender, System.EventArgs e)
		{
		}

        private void delConveterButton_Click(object sender, EventArgs e)
        {
            this.DeleteConverter(DataSourceType.Text);
        }

        private void importWizardForm_Back(object sender, Newtera.Studio.UserControls.Wizards.WizardForm.EventNextArgs e)
        {
            Newtera.Studio.UserControls.Wizards.WizardPageBase page =
				(Newtera.Studio.UserControls.Wizards.WizardPageBase) sender;

            if (page.Equals(this.previewDataPage) == true)
            {
                if (_package.DataSourceType == DataSourceType.Text)
                {
                    e.Step = 3; // skip excel and other format pages
                }
                else if (_package.DataSourceType == DataSourceType.Excel)
                {
                    e.Step = 2; // skip other format page
                }
            }
            else if (page.Equals(this.excelFormatPage) == true)
            {
                e.Step = 2;
            }
            else if (page.Equals(this.otherFormatPage) == true)
            {
                e.Step = 3;
            }
        }

        private void standardConverterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.standardConverterRadioButton.Checked)
            {
                this._package.ConverterTypeName = _package.GetDefaultConverterTypeName(_dataSourceType);
            }
        }

        private void specialConveterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.specialConveterRadioButton.Checked)
            {
                if (this.excelConverterComboBox.SelectedIndex >= 0)
                {
                    // get the selected converter name
                    this._package.ConverterTypeName = _excelConverters[this.excelConverterComboBox.SelectedIndex];
                }
                else
                {
                    this._package.ConverterTypeName = null;
                }
            }
        }

        private void autoPickExcelConverterButton_Click(object sender, EventArgs e)
        {
            AutoPickConverter(DataSourceType.Excel);
        }

        private void AddExcelConverterButton_Click(object sender, EventArgs e)
        {
            AddConverterDialog dialog = new AddConverterDialog();
            dialog.DataSourceType = DataSourceType.Excel;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddConverters(dialog.SelectedConverters, DataSourceType.Excel);
            }
        }

        private void delExcelConverterButton_Click(object sender, EventArgs e)
        {
            this.DeleteConverter(DataSourceType.Excel);
        }

        private void readExcelAllRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.readExcelAllRadioButton.Checked)
            {
                this._package.IsPaging = false;
                this.readExcelBlockSizeTextBox.ReadOnly = true;
            }
            else
            {
                this._package.IsPaging = true;
                this.readExcelBlockSizeTextBox.ReadOnly = false;
            }
        }

        private void readExcelInBlockRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.readExcelAllRadioButton.Checked)
            {
                this.readExcelBlockSizeTextBox.ReadOnly = true;
            }
            else
            {
                this.readExcelBlockSizeTextBox.ReadOnly = false;
            }
        }

        private void excelConveterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.excelConverterComboBox.SelectedIndex >= 0)
            {
                this.delExcelConverterButton.Enabled = true;
                if (this.specialConveterRadioButton.Checked)
                {
                    _package.ConverterTypeName = _excelConverters.Get(this.excelConverterComboBox.SelectedIndex);
                }
            }
            else
            {
                this.delExcelConverterButton.Enabled = false;
            }
        }

        private void otherConverterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.otherConverterComboBox.SelectedIndex >= 0)
            {
                this.delOtherConverterButton.Enabled = true;
                _package.ConverterTypeName = _otherConverters.Get(this.otherConverterComboBox.SelectedIndex);
            }
            else
            {
                this.delOtherConverterButton.Enabled = false;
            }
        }

        private void autoPickOtherConverterButton_Click(object sender, EventArgs e)
        {
            AutoPickConverter(DataSourceType.Other);
        }

        private void readOtherAllRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.readOtherAllRadioButton.Checked)
            {
                this._package.IsPaging = false;
                this.readOtherBlockSizeTextBox.ReadOnly = true;
            }
            else
            {
                this._package.IsPaging = true;
                this.readOtherBlockSizeTextBox.ReadOnly = false;
            }
        }

        private void readOtherInBlockRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.readOtherAllRadioButton.Checked)
            {
                this.readOtherBlockSizeTextBox.ReadOnly = true;
            }
            else
            {
                this.readOtherBlockSizeTextBox.ReadOnly = false;
            }
        }

        private void addOtherConverterButton_Click(object sender, EventArgs e)
        {
            AddConverterDialog dialog = new AddConverterDialog();
            dialog.DataSourceType = DataSourceType.Other;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddConverters(dialog.SelectedConverters, DataSourceType.Other);
            }
        }

        private void delOtherConverterButton_Click(object sender, EventArgs e)
        {
            this.DeleteConverter(DataSourceType.Other);
        }

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
		/// <param name="formatName">Format Name</param>
		public void FireDownloadGraphEvent(string formatName, string fileSuffix)
		{
			MessageBox.Show(ImportExportResourceManager.GetString("Info.NotImplementedYet"),
				"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		/// Fire the download graph event
		/// </summary>
		/// <param name="formatName">download file format</param>
		public void FireDownloadGraphEvent(string formatName)
		{
		}

		/// <summary>
		/// Create a web service proxy for chart related services
		/// </summary>
		/// <returns></returns>
		public Newtera.DataGridActiveX.ActiveXControlWebService.ActiveXControlService CreateActiveXControlWebService()
		{
            return DesignStudio.CreateActiveXControlWebService();
		}

		private void previewTextFileComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.previewTextFileComboBox.SelectedIndex >= 0)
			{
				DataSet selectedDataSet = this._sourceDataSets[this.previewTextFileComboBox.SelectedIndex];
				if (selectedDataSet != null && selectedDataSet.Tables.Count > 0)
				{
					// show the converted data in preview data grid
					this.previewDataGrid.DataSource = selectedDataSet.Tables[0];
				}
			}
		}

		private void addEntryButton_Click(object sender, System.EventArgs e)
		{
			if (this.classMapListView.SelectedIndices.Count == 1)
			{
				CreateClassMapping(this.classMapListView.SelectedIndices[0]);
			}
		}

		private void specifyMappingPage_Enter(object sender, System.EventArgs e)
		{
			if (this.classMapListView.SelectedIndices.Count == 1)
			{
				this.addEntryButton.Enabled = true;
				this.viewSourceButton.Enabled = true;
			}
			else
			{
				this.addEntryButton.Enabled = false;
				this.viewSourceButton.Enabled = false;
			}
		}

		private void viewSourceButton_Click(object sender, System.EventArgs e)
		{
			if (classMapListView.SelectedIndices.Count == 1)
			{
				int selectedIndex = classMapListView.SelectedIndices[0];
				ClassMapping selectedClassMapping = (ClassMapping) _package.ClassMappings[selectedIndex]; 
					
				ShowSourceTableDialog dialog = new ShowSourceTableDialog();
				dialog.SourceDataTable = CurrentSourceDataSet.Tables[selectedClassMapping.SourceClassIndex];

				dialog.ShowDialog();
			}
		}

        /// <summary>
        /// Gets the base class name of data instances currently displayed in the datagrid 
        /// </summary>
        public string BaseClassName
        {
            get
            {
                ClassMapping classMapping = (ClassMapping)_package.CheckedClassMappings[this.classComboBox.SelectedIndex];

                if (classMapping != null)
                {
                    return classMapping.DestinationClassName;
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
				ClassMapping classMapping = (ClassMapping) _package.CheckedClassMappings[this.classComboBox.SelectedIndex];

				if (classMapping != null)
				{
					//return classMapping.DestinationClassName;
                    return classMapping.DestinationTableName;
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
				return classDataGrid;
			}
		}

		/// <summary>
		/// Gets the DataView that is in the same order as what displayed on the datagrid
		/// </summary>
        public System.Data.DataView DataView
		{
			get
			{
				if (CurrentDestinationDataSet != null &&
					CurrentDestinationDataSet.Tables[TableName] != null)
				{
					return CurrentDestinationDataSet.Tables[TableName].DefaultView;
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
				if (tableName != null)
				{
					ColumnInfo columnInfo;
					columnInfos = new ColumnInfoCollection();

					DataGridTableStyle tableStyle = classDataGrid.TableStyles[tableName];
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

        private void startingDataRowTextBox_TextChanged(object sender, EventArgs e)
        {
            int dataStartingRow = 1;

            try
            {
                dataStartingRow = Int32.Parse(this.startingDataRowTextBox.Text);
            }
            catch (Exception)
            {
                if (this.firstRowColumnNameCheckBox.Checked)
                {
                    dataStartingRow = 2;
                }
                else
                {
                    dataStartingRow = 1;
                }
                this.startingDataRowTextBox.Text = dataStartingRow.ToString();
            }

            this._package.TextFormat.StartingDataRow = dataStartingRow;
        }

        private void ukRadioButton_Click(object sender, EventArgs e)
        {
            ClassElement classElement;

            // check if the destination classes have the unique key(s) defined
            foreach (ClassMapping classMapping in _package.CheckedClassMappings)
            {
                classElement = _metaData.SchemaModel.FindClass(classMapping.DestinationClassName);
                if (classElement.UniqueKeys == null || classElement.UniqueKeys.Count == 0)
                {
                    MessageBox.Show(ImportExportResourceManager.GetString("Warning.NoUniqueKeys") + classElement.Caption,
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
	}
}

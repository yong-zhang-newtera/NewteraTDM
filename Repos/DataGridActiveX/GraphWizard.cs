using System;
using System.Data;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.DataGridActiveX.Export;
using Newtera.DataGridActiveX.ChartModel;
using Newtera.DataGridActiveX.ActiveXControlWebService;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// GraphWizard 的摘要说明。
	/// </summary>
	public class GraphWizard : System.Windows.Forms.Form
	{
		private const int XML_BLOCK_SIZE = 50000; // xml block size to be sent over intranet

		private bool _isForWindowsClient;
		private ChartDef _chartDef;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private string _connectionString;
		private ActiveXControlService _service;
		private ChartInfoCollection _chartInfos;
        private ChartInfoCollection _chartTemplates;
		private ChartType _chartType;
		private ChartInfo _selectedChartInfo;
		private StringBuilder _currentXMLBuffer;
		private int _currentBlockNum;
		private EventType _eventType = EventType.Unknown;
		private ChartFormat _chartFormat;
		private ChartFormatCollection _chartFormats;
		private IDataGridControl _dataGridControl;
        private ChartInfoType _selectedChartInfoType;
        private bool _isDBA;

		private Newtera.DataGridActiveX.Wizard wizard1;
		private Newtera.DataGridActiveX.WizardPage chartTypeWizardPage;
		private System.Windows.Forms.TabControl chartTypeTabControl;
		private System.Windows.Forms.TabPage standardTabPage;
		private System.Windows.Forms.TabControl lineChartDataSettingTabControl;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton allRowsRadioButton;
		private System.Windows.Forms.RadioButton selectedRowsRadioButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton byColumnsRadioButton;
		private System.Windows.Forms.RadioButton byRowsRadioButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TabPage lineDataSourceTabPage;
		private System.Windows.Forms.TabPage lineChartTabPage;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox lineChartTitleTextBox;
		private System.Windows.Forms.GroupBox groupBox5;
		private Newtera.DataGridActiveX.WizardPage lineChartWizardPage;
		private Newtera.DataGridActiveX.WizardPage viewChartWizardPage;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Button viewChartButton;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox savedChartDescTextBox;
		private System.Windows.Forms.Button saveChartButton;
		private System.ComponentModel.IContainer components;

		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.RadioButton lineRadioButton;
		private System.Windows.Forms.RadioButton contourRadioButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TabPage lineTabPage;
		private System.Windows.Forms.ListBox lineListBox;
		private System.Windows.Forms.Button selectXAxisButton;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RadioButton oneXManyYRadioButton;
		private System.Windows.Forms.RadioButton oneXOneYRadioButton;
		private System.Windows.Forms.RadioButton manyXOneYRadioButton;
        private System.Windows.Forms.RadioButton separateCoordinatesRadioButton;
		private System.Windows.Forms.ListBox existingChartsListBox;
		private System.Windows.Forms.TextBox chartDescTextBox;
		private System.Windows.Forms.Button selectYAxisButton;
		private System.Windows.Forms.Button removeLineButton;
		private System.Windows.Forms.Button addLineButton;
		private System.Windows.Forms.TextBox lineYDataSeriesTextBox;
		private System.Windows.Forms.TextBox lineXDataSeriesTextBox;
		private Newtera.DataGridActiveX.EnterTextBox lineNameTextBox;
		private System.Windows.Forms.TextBox lineXAxisLabelTextBox;
		private System.Windows.Forms.TextBox lineYAxisLabelTextBox;
		private System.Windows.Forms.ToolTip toolTip1;
		private Newtera.DataGridActiveX.WizardPage contourChartWizardPage;
		private System.Windows.Forms.TabPage contourAxesTabPage;
		private System.Windows.Forms.TabPage contourChartTabPage;
		private System.Windows.Forms.TabControl contourChartTabControl;
		private System.Windows.Forms.GroupBox groupBox10;
		private System.Windows.Forms.GroupBox groupBox11;
		private System.Windows.Forms.GroupBox groupBox12;
		private System.Windows.Forms.TabPage contourDataSourceTabPage;
		private System.Windows.Forms.GroupBox groupBox13;
		private System.Windows.Forms.GroupBox groupBox14;
		private System.Windows.Forms.RadioButton contourByRowRadioButton;
		private System.Windows.Forms.RadioButton contourByColumnRadioButton;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox countourZAxisLabelTextBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox contourYAxisLabelTextBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox contourXAxisLabelTextBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox contourXAxisStartValueTextBox;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox contourXAxisStepValueTextBox;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox contourYAxisStartValueTextBox;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox contourYAxisStepValueTextBox;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox contourChartTitleTextBox;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.ComboBox contourLegengLocationComboBox;
		private System.Windows.Forms.RadioButton contourSelectedAreaRadioButton;
		private System.Windows.Forms.RadioButton contourWholeAreaRadioButton;
		private System.Windows.Forms.GroupBox groupBox15;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Button downloadButton;
		private System.Windows.Forms.ComboBox chartFormatComboBox;
        private TabControl tabControl1;
        private TabPage templateTabPage;
        private TabPage chartTabPage;
        private ListBox existingChartTemplateListBox;
        private Button deleteTemplateButton;
        private TextBox templateDescTextBox;
        private Button saveTemplateButton;
		private System.Windows.Forms.TextBox savedChartNameTextBox;

		public GraphWizard()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_isForWindowsClient = false;
			_dataGridControl = null;
			_chartDef = null;
			_isRequestComplete = false;
			_workInProgressDialog = new WorkInProgressDialog();
			_service = null;
			_connectionString = null;
			_chartInfos = null;
			_chartType = ChartType.Line;
			_selectedChartInfo = null;
            _selectedChartInfoType = ChartInfoType.Unknown;
            _isDBA = false;
		}

		/// <summary>
		/// Gets or sets the DataGridControl instance
		/// </summary>
		public IDataGridControl DataGridControl
		{
			get
			{
				return _dataGridControl;
			}
			set
			{
				_dataGridControl = value;
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
		/// Get the working chart def object
		/// </summary>
		public ChartDef WorkingChartDef
		{
			get
			{
				return _chartDef;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the wizard is launched
		/// from the Windows Client or not
		/// </summary>
		public bool IsForWindowsClient
		{
			get
			{
				return _isForWindowsClient;
			}
			set
			{
				_isForWindowsClient = value;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether current user is a DBA
        /// </summary>
        public bool IsDBA
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphWizard));
            this.wizard1 = new Newtera.DataGridActiveX.Wizard();
            this.chartTypeWizardPage = new Newtera.DataGridActiveX.WizardPage();
            this.chartTypeTabControl = new System.Windows.Forms.TabControl();
            this.standardTabPage = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.templateTabPage = new System.Windows.Forms.TabPage();
            this.deleteTemplateButton = new System.Windows.Forms.Button();
            this.templateDescTextBox = new System.Windows.Forms.TextBox();
            this.existingChartTemplateListBox = new System.Windows.Forms.ListBox();
            this.chartTabPage = new System.Windows.Forms.TabPage();
            this.chartDescTextBox = new System.Windows.Forms.TextBox();
            this.existingChartsListBox = new System.Windows.Forms.ListBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.contourRadioButton = new System.Windows.Forms.RadioButton();
            this.lineRadioButton = new System.Windows.Forms.RadioButton();
            this.lineChartWizardPage = new Newtera.DataGridActiveX.WizardPage();
            this.lineChartDataSettingTabControl = new System.Windows.Forms.TabControl();
            this.lineDataSourceTabPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.byRowsRadioButton = new System.Windows.Forms.RadioButton();
            this.byColumnsRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectedRowsRadioButton = new System.Windows.Forms.RadioButton();
            this.allRowsRadioButton = new System.Windows.Forms.RadioButton();
            this.lineTabPage = new System.Windows.Forms.TabPage();
            this.lineNameTextBox = new Newtera.DataGridActiveX.EnterTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.selectYAxisButton = new System.Windows.Forms.Button();
            this.lineYDataSeriesTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lineYAxisLabelTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.selectXAxisButton = new System.Windows.Forms.Button();
            this.lineXDataSeriesTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lineXAxisLabelTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.removeLineButton = new System.Windows.Forms.Button();
            this.addLineButton = new System.Windows.Forms.Button();
            this.lineListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lineChartTabPage = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.separateCoordinatesRadioButton = new System.Windows.Forms.RadioButton();
            this.manyXOneYRadioButton = new System.Windows.Forms.RadioButton();
            this.oneXManyYRadioButton = new System.Windows.Forms.RadioButton();
            this.oneXOneYRadioButton = new System.Windows.Forms.RadioButton();
            this.lineChartTitleTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.contourChartWizardPage = new Newtera.DataGridActiveX.WizardPage();
            this.contourChartTabControl = new System.Windows.Forms.TabControl();
            this.contourDataSourceTabPage = new System.Windows.Forms.TabPage();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.contourByRowRadioButton = new System.Windows.Forms.RadioButton();
            this.contourByColumnRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.contourSelectedAreaRadioButton = new System.Windows.Forms.RadioButton();
            this.contourWholeAreaRadioButton = new System.Windows.Forms.RadioButton();
            this.contourAxesTabPage = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.countourZAxisLabelTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.contourYAxisStepValueTextBox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.contourYAxisStartValueTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.contourYAxisLabelTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.contourXAxisStepValueTextBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.contourXAxisStartValueTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.contourXAxisLabelTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.contourChartTabPage = new System.Windows.Forms.TabPage();
            this.contourLegengLocationComboBox = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.contourChartTitleTextBox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.viewChartWizardPage = new Newtera.DataGridActiveX.WizardPage();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.downloadButton = new System.Windows.Forms.Button();
            this.chartFormatComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.saveTemplateButton = new System.Windows.Forms.Button();
            this.saveChartButton = new System.Windows.Forms.Button();
            this.savedChartDescTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.savedChartNameTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.viewChartButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.wizard1.SuspendLayout();
            this.chartTypeWizardPage.SuspendLayout();
            this.chartTypeTabControl.SuspendLayout();
            this.standardTabPage.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.templateTabPage.SuspendLayout();
            this.chartTabPage.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.lineChartWizardPage.SuspendLayout();
            this.lineChartDataSettingTabControl.SuspendLayout();
            this.lineDataSourceTabPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.lineTabPage.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.lineChartTabPage.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.contourChartWizardPage.SuspendLayout();
            this.contourChartTabControl.SuspendLayout();
            this.contourDataSourceTabPage.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.contourAxesTabPage.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.contourChartTabPage.SuspendLayout();
            this.viewChartWizardPage.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard1
            // 
            resources.ApplyResources(this.wizard1, "wizard1");
            this.wizard1.Controls.Add(this.viewChartWizardPage);
            this.wizard1.Controls.Add(this.contourChartWizardPage);
            this.wizard1.Controls.Add(this.lineChartWizardPage);
            this.wizard1.Controls.Add(this.chartTypeWizardPage);
            this.wizard1.Name = "wizard1";
            this.wizard1.Pages.AddRange(new Newtera.DataGridActiveX.WizardPage[] {
            this.chartTypeWizardPage,
            this.lineChartWizardPage,
            this.contourChartWizardPage,
            this.viewChartWizardPage});
            this.wizard1.AfterSwitchPages += new Newtera.DataGridActiveX.Wizard.AfterSwitchPagesEventHandler(this.wizard1_AfterSwitchPages);
            this.wizard1.BeforeSwitchPages += new Newtera.DataGridActiveX.Wizard.BeforeSwitchPagesEventHandler(this.wizard1_BeforeSwitchPages);
            this.wizard1.Cancel += new System.ComponentModel.CancelEventHandler(this.wizard1_Cancel);
            // 
            // chartTypeWizardPage
            // 
            resources.ApplyResources(this.chartTypeWizardPage, "chartTypeWizardPage");
            this.chartTypeWizardPage.Controls.Add(this.chartTypeTabControl);
            this.chartTypeWizardPage.Description = "Select a chart type";
            this.chartTypeWizardPage.Name = "chartTypeWizardPage";
            this.chartTypeWizardPage.Style = Newtera.DataGridActiveX.WizardPageStyle.Custom;
            this.chartTypeWizardPage.Title = "Chart Type";
            // 
            // chartTypeTabControl
            // 
            resources.ApplyResources(this.chartTypeTabControl, "chartTypeTabControl");
            this.chartTypeTabControl.Controls.Add(this.standardTabPage);
            this.chartTypeTabControl.Name = "chartTypeTabControl";
            this.chartTypeTabControl.SelectedIndex = 0;
            // 
            // standardTabPage
            // 
            this.standardTabPage.Controls.Add(this.tabControl1);
            this.standardTabPage.Controls.Add(this.groupBox9);
            resources.ApplyResources(this.standardTabPage, "standardTabPage");
            this.standardTabPage.Name = "standardTabPage";
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.templateTabPage);
            this.tabControl1.Controls.Add(this.chartTabPage);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // templateTabPage
            // 
            this.templateTabPage.Controls.Add(this.deleteTemplateButton);
            this.templateTabPage.Controls.Add(this.templateDescTextBox);
            this.templateTabPage.Controls.Add(this.existingChartTemplateListBox);
            resources.ApplyResources(this.templateTabPage, "templateTabPage");
            this.templateTabPage.Name = "templateTabPage";
            this.toolTip1.SetToolTip(this.templateTabPage, resources.GetString("templateTabPage.ToolTip"));
            this.templateTabPage.UseVisualStyleBackColor = true;
            // 
            // deleteTemplateButton
            // 
            resources.ApplyResources(this.deleteTemplateButton, "deleteTemplateButton");
            this.deleteTemplateButton.Name = "deleteTemplateButton";
            this.deleteTemplateButton.UseVisualStyleBackColor = true;
            this.deleteTemplateButton.Click += new System.EventHandler(this.deleteTemplateButton_Click);
            // 
            // templateDescTextBox
            // 
            resources.ApplyResources(this.templateDescTextBox, "templateDescTextBox");
            this.templateDescTextBox.Name = "templateDescTextBox";
            this.templateDescTextBox.ReadOnly = true;
            // 
            // existingChartTemplateListBox
            // 
            resources.ApplyResources(this.existingChartTemplateListBox, "existingChartTemplateListBox");
            this.existingChartTemplateListBox.FormattingEnabled = true;
            this.existingChartTemplateListBox.Name = "existingChartTemplateListBox";
            this.existingChartTemplateListBox.SelectedIndexChanged += new System.EventHandler(this.existingChartTemplateListBox_SelectedIndexChanged);
            // 
            // chartTabPage
            // 
            this.chartTabPage.Controls.Add(this.chartDescTextBox);
            this.chartTabPage.Controls.Add(this.existingChartsListBox);
            resources.ApplyResources(this.chartTabPage, "chartTabPage");
            this.chartTabPage.Name = "chartTabPage";
            this.toolTip1.SetToolTip(this.chartTabPage, resources.GetString("chartTabPage.ToolTip"));
            this.chartTabPage.UseVisualStyleBackColor = true;
            // 
            // chartDescTextBox
            // 
            resources.ApplyResources(this.chartDescTextBox, "chartDescTextBox");
            this.chartDescTextBox.Name = "chartDescTextBox";
            this.chartDescTextBox.ReadOnly = true;
            // 
            // existingChartsListBox
            // 
            resources.ApplyResources(this.existingChartsListBox, "existingChartsListBox");
            this.existingChartsListBox.Name = "existingChartsListBox";
            this.existingChartsListBox.SelectedIndexChanged += new System.EventHandler(this.existingChartsListBox_SelectedIndexChanged);
            // 
            // groupBox9
            // 
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Controls.Add(this.label2);
            this.groupBox9.Controls.Add(this.label1);
            this.groupBox9.Controls.Add(this.contourRadioButton);
            this.groupBox9.Controls.Add(this.lineRadioButton);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ImageList = this.imageList;
            this.label2.Name = "label2";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ImageList = this.imageList;
            this.label1.Name = "label1";
            // 
            // contourRadioButton
            // 
            resources.ApplyResources(this.contourRadioButton, "contourRadioButton");
            this.contourRadioButton.Name = "contourRadioButton";
            this.contourRadioButton.CheckedChanged += new System.EventHandler(this.contourRadioButton_CheckedChanged);
            // 
            // lineRadioButton
            // 
            resources.ApplyResources(this.lineRadioButton, "lineRadioButton");
            this.lineRadioButton.Name = "lineRadioButton";
            this.lineRadioButton.CheckedChanged += new System.EventHandler(this.lineRadioButton_CheckedChanged);
            // 
            // lineChartWizardPage
            // 
            resources.ApplyResources(this.lineChartWizardPage, "lineChartWizardPage");
            this.lineChartWizardPage.Controls.Add(this.lineChartDataSettingTabControl);
            this.lineChartWizardPage.Description = "Line Chart Setting";
            this.lineChartWizardPage.Name = "lineChartWizardPage";
            this.lineChartWizardPage.Style = Newtera.DataGridActiveX.WizardPageStyle.Custom;
            this.lineChartWizardPage.Title = "Line Chart";
            // 
            // lineChartDataSettingTabControl
            // 
            resources.ApplyResources(this.lineChartDataSettingTabControl, "lineChartDataSettingTabControl");
            this.lineChartDataSettingTabControl.Controls.Add(this.lineDataSourceTabPage);
            this.lineChartDataSettingTabControl.Controls.Add(this.lineTabPage);
            this.lineChartDataSettingTabControl.Controls.Add(this.lineChartTabPage);
            this.lineChartDataSettingTabControl.Name = "lineChartDataSettingTabControl";
            this.lineChartDataSettingTabControl.SelectedIndex = 0;
            // 
            // lineDataSourceTabPage
            // 
            this.lineDataSourceTabPage.Controls.Add(this.groupBox2);
            this.lineDataSourceTabPage.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.lineDataSourceTabPage, "lineDataSourceTabPage");
            this.lineDataSourceTabPage.Name = "lineDataSourceTabPage";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.byRowsRadioButton);
            this.groupBox2.Controls.Add(this.byColumnsRadioButton);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // byRowsRadioButton
            // 
            resources.ApplyResources(this.byRowsRadioButton, "byRowsRadioButton");
            this.byRowsRadioButton.Name = "byRowsRadioButton";
            // 
            // byColumnsRadioButton
            // 
            this.byColumnsRadioButton.Checked = true;
            resources.ApplyResources(this.byColumnsRadioButton, "byColumnsRadioButton");
            this.byColumnsRadioButton.Name = "byColumnsRadioButton";
            this.byColumnsRadioButton.TabStop = true;
            this.byColumnsRadioButton.CheckedChanged += new System.EventHandler(this.byColumnsRadioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.selectedRowsRadioButton);
            this.groupBox1.Controls.Add(this.allRowsRadioButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // selectedRowsRadioButton
            // 
            resources.ApplyResources(this.selectedRowsRadioButton, "selectedRowsRadioButton");
            this.selectedRowsRadioButton.Name = "selectedRowsRadioButton";
            // 
            // allRowsRadioButton
            // 
            this.allRowsRadioButton.Checked = true;
            resources.ApplyResources(this.allRowsRadioButton, "allRowsRadioButton");
            this.allRowsRadioButton.Name = "allRowsRadioButton";
            this.allRowsRadioButton.TabStop = true;
            this.allRowsRadioButton.CheckedChanged += new System.EventHandler(this.allRowsRadioButton_CheckedChanged);
            // 
            // lineTabPage
            // 
            this.lineTabPage.Controls.Add(this.lineNameTextBox);
            this.lineTabPage.Controls.Add(this.groupBox4);
            this.lineTabPage.Controls.Add(this.groupBox3);
            this.lineTabPage.Controls.Add(this.removeLineButton);
            this.lineTabPage.Controls.Add(this.addLineButton);
            this.lineTabPage.Controls.Add(this.lineListBox);
            this.lineTabPage.Controls.Add(this.label3);
            resources.ApplyResources(this.lineTabPage, "lineTabPage");
            this.lineTabPage.Name = "lineTabPage";
            // 
            // lineNameTextBox
            // 
            resources.ApplyResources(this.lineNameTextBox, "lineNameTextBox");
            this.lineNameTextBox.Name = "lineNameTextBox";
            this.lineNameTextBox.Leave += new System.EventHandler(this.lineNameTextBox_Leave);
            this.lineNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lineNameTextBox_KeyDown);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.selectYAxisButton);
            this.groupBox4.Controls.Add(this.lineYDataSeriesTextBox);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.lineYAxisLabelTextBox);
            this.groupBox4.Controls.Add(this.label9);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // selectYAxisButton
            // 
            resources.ApplyResources(this.selectYAxisButton, "selectYAxisButton");
            this.selectYAxisButton.Name = "selectYAxisButton";
            this.toolTip1.SetToolTip(this.selectYAxisButton, resources.GetString("selectYAxisButton.ToolTip"));
            this.selectYAxisButton.Click += new System.EventHandler(this.selectYAxisButton_Click);
            // 
            // lineYDataSeriesTextBox
            // 
            resources.ApplyResources(this.lineYDataSeriesTextBox, "lineYDataSeriesTextBox");
            this.lineYDataSeriesTextBox.Name = "lineYDataSeriesTextBox";
            this.lineYDataSeriesTextBox.ReadOnly = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // lineYAxisLabelTextBox
            // 
            resources.ApplyResources(this.lineYAxisLabelTextBox, "lineYAxisLabelTextBox");
            this.lineYAxisLabelTextBox.Name = "lineYAxisLabelTextBox";
            this.toolTip1.SetToolTip(this.lineYAxisLabelTextBox, resources.GetString("lineYAxisLabelTextBox.ToolTip"));
            this.lineYAxisLabelTextBox.Leave += new System.EventHandler(this.lineYAxisLabelTextBox_Leave);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.selectXAxisButton);
            this.groupBox3.Controls.Add(this.lineXDataSeriesTextBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.lineXAxisLabelTextBox);
            this.groupBox3.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // selectXAxisButton
            // 
            resources.ApplyResources(this.selectXAxisButton, "selectXAxisButton");
            this.selectXAxisButton.Name = "selectXAxisButton";
            this.toolTip1.SetToolTip(this.selectXAxisButton, resources.GetString("selectXAxisButton.ToolTip"));
            this.selectXAxisButton.Click += new System.EventHandler(this.selectXAxisButton_Click);
            // 
            // lineXDataSeriesTextBox
            // 
            resources.ApplyResources(this.lineXDataSeriesTextBox, "lineXDataSeriesTextBox");
            this.lineXDataSeriesTextBox.Name = "lineXDataSeriesTextBox";
            this.lineXDataSeriesTextBox.ReadOnly = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lineXAxisLabelTextBox
            // 
            resources.ApplyResources(this.lineXAxisLabelTextBox, "lineXAxisLabelTextBox");
            this.lineXAxisLabelTextBox.Name = "lineXAxisLabelTextBox";
            this.toolTip1.SetToolTip(this.lineXAxisLabelTextBox, resources.GetString("lineXAxisLabelTextBox.ToolTip"));
            this.lineXAxisLabelTextBox.Leave += new System.EventHandler(this.lineXAxisLabelTextBox_Leave);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // removeLineButton
            // 
            resources.ApplyResources(this.removeLineButton, "removeLineButton");
            this.removeLineButton.Name = "removeLineButton";
            this.removeLineButton.Click += new System.EventHandler(this.removeLineButton_Click);
            // 
            // addLineButton
            // 
            resources.ApplyResources(this.addLineButton, "addLineButton");
            this.addLineButton.Name = "addLineButton";
            this.addLineButton.Click += new System.EventHandler(this.addLineButton_Click);
            // 
            // lineListBox
            // 
            resources.ApplyResources(this.lineListBox, "lineListBox");
            this.lineListBox.Name = "lineListBox";
            this.lineListBox.SelectedIndexChanged += new System.EventHandler(this.lineListBox_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lineChartTabPage
            // 
            this.lineChartTabPage.Controls.Add(this.groupBox5);
            this.lineChartTabPage.Controls.Add(this.lineChartTitleTextBox);
            this.lineChartTabPage.Controls.Add(this.label10);
            resources.ApplyResources(this.lineChartTabPage, "lineChartTabPage");
            this.lineChartTabPage.Name = "lineChartTabPage";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.separateCoordinatesRadioButton);
            this.groupBox5.Controls.Add(this.manyXOneYRadioButton);
            this.groupBox5.Controls.Add(this.oneXManyYRadioButton);
            this.groupBox5.Controls.Add(this.oneXOneYRadioButton);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // separateCoordinatesRadioButton
            // 
            resources.ApplyResources(this.separateCoordinatesRadioButton, "separateCoordinatesRadioButton");
            this.separateCoordinatesRadioButton.Name = "separateCoordinatesRadioButton";
            this.separateCoordinatesRadioButton.CheckedChanged += new System.EventHandler(this.separateCoordinatesRadioButton_CheckedChanged);
            // 
            // manyXOneYRadioButton
            // 
            resources.ApplyResources(this.manyXOneYRadioButton, "manyXOneYRadioButton");
            this.manyXOneYRadioButton.Name = "manyXOneYRadioButton";
            this.manyXOneYRadioButton.CheckedChanged += new System.EventHandler(this.manyXOneYRadioButton_CheckedChanged);
            // 
            // oneXManyYRadioButton
            // 
            resources.ApplyResources(this.oneXManyYRadioButton, "oneXManyYRadioButton");
            this.oneXManyYRadioButton.Name = "oneXManyYRadioButton";
            this.oneXManyYRadioButton.CheckedChanged += new System.EventHandler(this.oneXManyYRadioButton_CheckedChanged);
            // 
            // oneXOneYRadioButton
            // 
            this.oneXOneYRadioButton.Checked = true;
            resources.ApplyResources(this.oneXOneYRadioButton, "oneXOneYRadioButton");
            this.oneXOneYRadioButton.Name = "oneXOneYRadioButton";
            this.oneXOneYRadioButton.TabStop = true;
            this.oneXOneYRadioButton.CheckedChanged += new System.EventHandler(this.oneXOneYRadioButton_CheckedChanged);
            // 
            // lineChartTitleTextBox
            // 
            resources.ApplyResources(this.lineChartTitleTextBox, "lineChartTitleTextBox");
            this.lineChartTitleTextBox.Name = "lineChartTitleTextBox";
            this.lineChartTitleTextBox.Leave += new System.EventHandler(this.lineChartTitleTextBox_Leave);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // contourChartWizardPage
            // 
            resources.ApplyResources(this.contourChartWizardPage, "contourChartWizardPage");
            this.contourChartWizardPage.Controls.Add(this.contourChartTabControl);
            this.contourChartWizardPage.Name = "contourChartWizardPage";
            this.contourChartWizardPage.Style = Newtera.DataGridActiveX.WizardPageStyle.Custom;
            this.contourChartWizardPage.Title = "Contour Chart";
            // 
            // contourChartTabControl
            // 
            resources.ApplyResources(this.contourChartTabControl, "contourChartTabControl");
            this.contourChartTabControl.Controls.Add(this.contourDataSourceTabPage);
            this.contourChartTabControl.Controls.Add(this.contourAxesTabPage);
            this.contourChartTabControl.Controls.Add(this.contourChartTabPage);
            this.contourChartTabControl.Name = "contourChartTabControl";
            this.contourChartTabControl.SelectedIndex = 0;
            // 
            // contourDataSourceTabPage
            // 
            this.contourDataSourceTabPage.Controls.Add(this.groupBox14);
            this.contourDataSourceTabPage.Controls.Add(this.groupBox13);
            resources.ApplyResources(this.contourDataSourceTabPage, "contourDataSourceTabPage");
            this.contourDataSourceTabPage.Name = "contourDataSourceTabPage";
            // 
            // groupBox14
            // 
            resources.ApplyResources(this.groupBox14, "groupBox14");
            this.groupBox14.Controls.Add(this.contourByRowRadioButton);
            this.groupBox14.Controls.Add(this.contourByColumnRadioButton);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            // 
            // contourByRowRadioButton
            // 
            this.contourByRowRadioButton.Checked = true;
            resources.ApplyResources(this.contourByRowRadioButton, "contourByRowRadioButton");
            this.contourByRowRadioButton.Name = "contourByRowRadioButton";
            this.contourByRowRadioButton.TabStop = true;
            // 
            // contourByColumnRadioButton
            // 
            resources.ApplyResources(this.contourByColumnRadioButton, "contourByColumnRadioButton");
            this.contourByColumnRadioButton.Name = "contourByColumnRadioButton";
            this.contourByColumnRadioButton.CheckedChanged += new System.EventHandler(this.contourByColumnRadioButton_CheckedChanged);
            // 
            // groupBox13
            // 
            resources.ApplyResources(this.groupBox13, "groupBox13");
            this.groupBox13.Controls.Add(this.contourSelectedAreaRadioButton);
            this.groupBox13.Controls.Add(this.contourWholeAreaRadioButton);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.TabStop = false;
            // 
            // contourSelectedAreaRadioButton
            // 
            resources.ApplyResources(this.contourSelectedAreaRadioButton, "contourSelectedAreaRadioButton");
            this.contourSelectedAreaRadioButton.Name = "contourSelectedAreaRadioButton";
            // 
            // contourWholeAreaRadioButton
            // 
            this.contourWholeAreaRadioButton.Checked = true;
            resources.ApplyResources(this.contourWholeAreaRadioButton, "contourWholeAreaRadioButton");
            this.contourWholeAreaRadioButton.Name = "contourWholeAreaRadioButton";
            this.contourWholeAreaRadioButton.TabStop = true;
            this.contourWholeAreaRadioButton.CheckedChanged += new System.EventHandler(this.contourWholeAreaRadioButton_CheckedChanged);
            // 
            // contourAxesTabPage
            // 
            this.contourAxesTabPage.Controls.Add(this.groupBox12);
            this.contourAxesTabPage.Controls.Add(this.groupBox11);
            this.contourAxesTabPage.Controls.Add(this.groupBox10);
            resources.ApplyResources(this.contourAxesTabPage, "contourAxesTabPage");
            this.contourAxesTabPage.Name = "contourAxesTabPage";
            // 
            // groupBox12
            // 
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.Controls.Add(this.countourZAxisLabelTextBox);
            this.groupBox12.Controls.Add(this.label6);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            // 
            // countourZAxisLabelTextBox
            // 
            resources.ApplyResources(this.countourZAxisLabelTextBox, "countourZAxisLabelTextBox");
            this.countourZAxisLabelTextBox.Name = "countourZAxisLabelTextBox";
            this.countourZAxisLabelTextBox.Leave += new System.EventHandler(this.countourZAxisLabelTextBox_Leave);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // groupBox11
            // 
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Controls.Add(this.contourYAxisStepValueTextBox);
            this.groupBox11.Controls.Add(this.label17);
            this.groupBox11.Controls.Add(this.contourYAxisStartValueTextBox);
            this.groupBox11.Controls.Add(this.label16);
            this.groupBox11.Controls.Add(this.contourYAxisLabelTextBox);
            this.groupBox11.Controls.Add(this.label7);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // contourYAxisStepValueTextBox
            // 
            resources.ApplyResources(this.contourYAxisStepValueTextBox, "contourYAxisStepValueTextBox");
            this.contourYAxisStepValueTextBox.Name = "contourYAxisStepValueTextBox";
            this.contourYAxisStepValueTextBox.Leave += new System.EventHandler(this.contourYAxisStepValueTextBox_Leave);
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // contourYAxisStartValueTextBox
            // 
            resources.ApplyResources(this.contourYAxisStartValueTextBox, "contourYAxisStartValueTextBox");
            this.contourYAxisStartValueTextBox.Name = "contourYAxisStartValueTextBox";
            this.contourYAxisStartValueTextBox.Leave += new System.EventHandler(this.contourYAxisStartValueTextBox_Leave);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // contourYAxisLabelTextBox
            // 
            resources.ApplyResources(this.contourYAxisLabelTextBox, "contourYAxisLabelTextBox");
            this.contourYAxisLabelTextBox.Name = "contourYAxisLabelTextBox";
            this.contourYAxisLabelTextBox.Leave += new System.EventHandler(this.contourYAxisLabelTextBox_Leave);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // groupBox10
            // 
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Controls.Add(this.contourXAxisStepValueTextBox);
            this.groupBox10.Controls.Add(this.label15);
            this.groupBox10.Controls.Add(this.contourXAxisStartValueTextBox);
            this.groupBox10.Controls.Add(this.label12);
            this.groupBox10.Controls.Add(this.contourXAxisLabelTextBox);
            this.groupBox10.Controls.Add(this.label11);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // contourXAxisStepValueTextBox
            // 
            resources.ApplyResources(this.contourXAxisStepValueTextBox, "contourXAxisStepValueTextBox");
            this.contourXAxisStepValueTextBox.Name = "contourXAxisStepValueTextBox";
            this.contourXAxisStepValueTextBox.Leave += new System.EventHandler(this.contourXAxisStepValueTextBox_Leave);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // contourXAxisStartValueTextBox
            // 
            resources.ApplyResources(this.contourXAxisStartValueTextBox, "contourXAxisStartValueTextBox");
            this.contourXAxisStartValueTextBox.Name = "contourXAxisStartValueTextBox";
            this.contourXAxisStartValueTextBox.Leave += new System.EventHandler(this.contourXAxisStartValueTextBox_Leave);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // contourXAxisLabelTextBox
            // 
            resources.ApplyResources(this.contourXAxisLabelTextBox, "contourXAxisLabelTextBox");
            this.contourXAxisLabelTextBox.Name = "contourXAxisLabelTextBox";
            this.contourXAxisLabelTextBox.Leave += new System.EventHandler(this.contourXAxisLabelTextBox_Leave);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // contourChartTabPage
            // 
            this.contourChartTabPage.Controls.Add(this.contourLegengLocationComboBox);
            this.contourChartTabPage.Controls.Add(this.label19);
            this.contourChartTabPage.Controls.Add(this.contourChartTitleTextBox);
            this.contourChartTabPage.Controls.Add(this.label18);
            resources.ApplyResources(this.contourChartTabPage, "contourChartTabPage");
            this.contourChartTabPage.Name = "contourChartTabPage";
            // 
            // contourLegengLocationComboBox
            // 
            resources.ApplyResources(this.contourLegengLocationComboBox, "contourLegengLocationComboBox");
            this.contourLegengLocationComboBox.Items.AddRange(new object[] {
            resources.GetString("contourLegengLocationComboBox.Items"),
            resources.GetString("contourLegengLocationComboBox.Items1")});
            this.contourLegengLocationComboBox.Name = "contourLegengLocationComboBox";
            this.contourLegengLocationComboBox.SelectedIndexChanged += new System.EventHandler(this.contourLegengLocationComboBox_SelectedIndexChanged);
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // contourChartTitleTextBox
            // 
            resources.ApplyResources(this.contourChartTitleTextBox, "contourChartTitleTextBox");
            this.contourChartTitleTextBox.Name = "contourChartTitleTextBox";
            this.contourChartTitleTextBox.Leave += new System.EventHandler(this.contourChartTitleTextBox_Leave);
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // viewChartWizardPage
            // 
            resources.ApplyResources(this.viewChartWizardPage, "viewChartWizardPage");
            this.viewChartWizardPage.Controls.Add(this.groupBox15);
            this.viewChartWizardPage.Controls.Add(this.groupBox8);
            this.viewChartWizardPage.Controls.Add(this.groupBox7);
            this.viewChartWizardPage.Description = "View chart";
            this.viewChartWizardPage.Name = "viewChartWizardPage";
            this.viewChartWizardPage.Style = Newtera.DataGridActiveX.WizardPageStyle.Custom;
            this.viewChartWizardPage.Title = "View Chart";
            // 
            // groupBox15
            // 
            resources.ApplyResources(this.groupBox15, "groupBox15");
            this.groupBox15.Controls.Add(this.label20);
            this.groupBox15.Controls.Add(this.downloadButton);
            this.groupBox15.Controls.Add(this.chartFormatComboBox);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.TabStop = false;
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // downloadButton
            // 
            resources.ApplyResources(this.downloadButton, "downloadButton");
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // chartFormatComboBox
            // 
            resources.ApplyResources(this.chartFormatComboBox, "chartFormatComboBox");
            this.chartFormatComboBox.Name = "chartFormatComboBox";
            // 
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Controls.Add(this.saveTemplateButton);
            this.groupBox8.Controls.Add(this.saveChartButton);
            this.groupBox8.Controls.Add(this.savedChartDescTextBox);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.savedChartNameTextBox);
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // saveTemplateButton
            // 
            resources.ApplyResources(this.saveTemplateButton, "saveTemplateButton");
            this.saveTemplateButton.Name = "saveTemplateButton";
            this.saveTemplateButton.Click += new System.EventHandler(this.saveTemplateButton_Click);
            // 
            // saveChartButton
            // 
            resources.ApplyResources(this.saveChartButton, "saveChartButton");
            this.saveChartButton.Name = "saveChartButton";
            this.saveChartButton.Click += new System.EventHandler(this.saveChartButton_Click);
            // 
            // savedChartDescTextBox
            // 
            resources.ApplyResources(this.savedChartDescTextBox, "savedChartDescTextBox");
            this.savedChartDescTextBox.Name = "savedChartDescTextBox";
            this.savedChartDescTextBox.Leave += new System.EventHandler(this.savedChartDescTextBox_Leave);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // savedChartNameTextBox
            // 
            resources.ApplyResources(this.savedChartNameTextBox, "savedChartNameTextBox");
            this.savedChartNameTextBox.Name = "savedChartNameTextBox";
            this.savedChartNameTextBox.Leave += new System.EventHandler(this.savedChartTitleTextBox_Leave);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.viewChartButton);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // viewChartButton
            // 
            resources.ApplyResources(this.viewChartButton, "viewChartButton");
            this.viewChartButton.Name = "viewChartButton";
            this.viewChartButton.Click += new System.EventHandler(this.viewChartButton_Click);
            // 
            // GraphWizard
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.wizard1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphWizard";
            this.Load += new System.EventHandler(this.GraphWizard_Load);
            this.wizard1.ResumeLayout(false);
            this.chartTypeWizardPage.ResumeLayout(false);
            this.chartTypeTabControl.ResumeLayout(false);
            this.standardTabPage.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.templateTabPage.ResumeLayout(false);
            this.templateTabPage.PerformLayout();
            this.chartTabPage.ResumeLayout(false);
            this.chartTabPage.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.lineChartWizardPage.ResumeLayout(false);
            this.lineChartDataSettingTabControl.ResumeLayout(false);
            this.lineDataSourceTabPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.lineTabPage.ResumeLayout(false);
            this.lineTabPage.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.lineChartTabPage.ResumeLayout(false);
            this.lineChartTabPage.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.contourChartWizardPage.ResumeLayout(false);
            this.contourChartTabControl.ResumeLayout(false);
            this.contourDataSourceTabPage.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.contourAxesTabPage.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.contourChartTabPage.ResumeLayout(false);
            this.contourChartTabPage.PerformLayout();
            this.viewChartWizardPage.ResumeLayout(false);
            this.groupBox15.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region internal methods

		/// <summary>
		/// show the LineChartDef on the Line Chart Page
		/// </summary>
		private void ShowLineChartDef()
		{
			// show the selections
			LineChartDef lineChart = (LineChartDef) _chartDef;

			if (lineChart.Title != null)
			{
				this.lineChartTitleTextBox.Text = lineChart.Title;
			}

			if (lineChart.Name != null)
			{
				this.savedChartNameTextBox.Text = lineChart.Name;
			}

			if (lineChart.Description != null)
			{
				this.savedChartDescTextBox.Text = lineChart.Description;
			}

			// display the lines of the line chart def
			this.lineListBox.Items.Clear();
			LineDefCollection lines = lineChart.Lines;
			this.lineListBox.DisplayMember = "Name";
			foreach (LineDef lineDef in lines)
			{
				this.lineListBox.Items.Add(lineDef);
			}

			if (this.lineListBox.Items.Count > 0)
			{
				this.lineListBox.SelectedIndex = 0;
			}
			else
			{
				this.lineListBox.SelectedIndex = -1;
			}
		}

		/// <summary>
		/// show the ShowContourChartDef on the contour Chart Page
		/// </summary>
		private void ShowContourChartDef()
		{
			// show the selections
			ContourChartDef contourChart = (ContourChartDef) _chartDef;

			if (contourChart.Title != null)
			{
				this.contourChartTitleTextBox.Text = contourChart.Title;
			}

			this.contourXAxisStartValueTextBox.Text = contourChart.XStart + "";
			this.contourYAxisStartValueTextBox.Text = contourChart.YStart + "";
			this.contourXAxisStepValueTextBox.Text = contourChart.XStep + "";
			this.contourYAxisStepValueTextBox.Text = contourChart.YStep + "";

			if (contourChart.XAxisLabel != null)
			{
				this.contourXAxisLabelTextBox.Text = contourChart.XAxisLabel;
			}

			if (contourChart.YAxisLabel != null)
			{
				this.contourYAxisLabelTextBox.Text = contourChart.YAxisLabel;
			}

			if (contourChart.ZAxisLabel != null)
			{
				this.countourZAxisLabelTextBox.Text = contourChart.ZAxisLabel;
			}

			switch (contourChart.LegendLocation)
			{
				case LegendLocation.Right:
					this.contourLegengLocationComboBox.SelectedIndex = 0;
					break;

				case LegendLocation.Left:
					this.contourLegengLocationComboBox.SelectedIndex = 1;
					break;
			}
		}

		/// <summary>
		/// Gets the existing ChartDef instance from the server
		/// </summary>
		private void GetExistingChartDef()
		{
            if (_service == null)
            {
                _service = this._dataGridControl.CreateActiveXControlWebService();
            }

            if (_selectedChartInfoType == ChartInfoType.SavedTemplate)
            {
                _chartDef = HandleChartUtil.GetChartDef(_service, _selectedChartInfo);
                _chartDef.IsAltered = true;

                // it is the UI thread, show the chartDef
                switch (this._chartType)
                {
                    case ChartType.Line:
                    case ChartType.Bar:
                        ShowLineChartDef();
                        break;

                    case ChartType.Contour:
                        ShowContourChartDef();
                        break;
                }
            }
            else if (_selectedChartInfoType == ChartInfoType.SavedChart)
            {
                _isRequestComplete = false;

                this._currentBlockNum = 0;

                // The size of saved chart def may be very large, therefore, we will get
                // it using async method
                _service.BeginGetChartDefXmlById(_selectedChartInfo.ID,
                    0,
                    new AsyncCallback(GetExistingChartDefDone),
                    null);

                // launch a work in progress dialog
                this._workInProgressDialog.MaximumSteps = 10;
                this._workInProgressDialog.Value = 1;
                ShowWorkingDialog(MessageResourceManager.GetString("GraphWiard.GetChartInfo"));
            }
		}

		/// <summary>
		/// The AsyncCallback event handler for Web service method BeginGetChartDefXmlById.
		/// </summary>
		/// <param name="res">The result</param>
		private void GetExistingChartDefDone(IAsyncResult res)
		{
			try
			{
				string xml = _service.EndGetChartDefXmlById(res);

				this._workInProgressDialog.PerformStep(); // move one step forward

				StringBuilder builder = new StringBuilder(xml);

				// get xml in blocks
				while (true)
				{
					this._currentBlockNum++;

					xml = _service.GetChartDefXmlById(_selectedChartInfo.ID,
						_currentBlockNum);

					this._workInProgressDialog.PerformStep(); // move one step forward

					if (xml != null)
					{
						builder.Append(xml);
					}
					else
					{
						break;
					}
				}

				_chartDef = ChartDef.ConvertToChartDef(_selectedChartInfo.Type, builder.ToString());
				_chartDef.IsAltered = true;

				ShowChartDef();
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

		private delegate void ShowChartDefDelegate();

		/// <summary>
		/// Display the query result data set in a data grid
		/// </summary>
		/// <param name="dataSet">The query result in DataSet</param>
		private void ShowChartDef()
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, show the chartDef
				switch (this._chartType)
				{
					case ChartType.Line:
                    case ChartType.Bar:
						// set the lines in the chart as existing lines so that they
						// won't be modified
						LineChartDef lineChart = (LineChartDef) this._chartDef;
						foreach (LineDef lineDef in lineChart.Lines)
						{
							lineDef.IsNew = false;
						}
						ShowLineChartDef();
						break;

					case ChartType.Contour:
						ShowContourChartDef();
						break;
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowChartDefDelegate showChartDef = new ShowChartDefDelegate(ShowChartDef);

				this.BeginInvoke(showChartDef);
			}
		}

		private void ShowSelectedLine(LineDef lineDef)
		{
			if (lineDef != null)
			{
				this.lineNameTextBox.Text = lineDef.Name;

				this.lineXDataSeriesTextBox.Text = lineDef.XAxis.SeriesCaption;
				this.lineXAxisLabelTextBox.Text = lineDef.XAxis.Label;

				this.lineYDataSeriesTextBox.Text = lineDef.YAxis.SeriesCaption;
				this.lineYAxisLabelTextBox.Text = lineDef.YAxis.Label;
			}
			else
			{
				this.lineNameTextBox.Text = null;

				this.lineXDataSeriesTextBox.Text = null;
				this.lineXAxisLabelTextBox.Text = null;

				this.lineYDataSeriesTextBox.Text = null;
				this.lineYAxisLabelTextBox.Text = null;

			}
		}

		private void RenameLineName()
		{
			if (this.lineListBox.SelectedIndex >= 0)
			{
				int selectedIndex = this.lineListBox.SelectedIndex;
				if (lineNameTextBox.Text != null && lineNameTextBox.Text.Length > 0)
				{
					LineDef selectedLine = (LineDef) this.lineListBox.SelectedItem;
					string lineName = lineNameTextBox.Text;

					// make sure the name is unique
					if (IsUniqueLineName(lineName, selectedLine))
					{
						selectedLine.Name = lineName;

						// refresh the name displayed in list box
						lineListBox.Items.Clear();
						foreach (LineDef lineDef in ((LineChartDef) this._chartDef).Lines)
						{
							lineListBox.Items.Add(lineDef);
						}

						lineListBox.SelectedIndex = selectedIndex;
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("GraphWizard.DuplicateLineName"),
							"Error", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
				}
			}
		}

		/// <summary>
		/// Gets the information indicating whether the given name has been used by
		/// other lines in the line chart
		/// </summary>
		/// <param name="name">the line name</param>
		/// <param name="selectedLine">the selected line</param>
		/// <returns></returns>
		private bool IsUniqueLineName(string name, LineDef selectedLine)
		{
			bool status = true;
			foreach (LineDef lineDef in ((LineChartDef) this._chartDef).Lines)
			{
				if (lineDef.Name == name && lineDef != selectedLine)
				{
					status = false;
					break;
				}
			}

			return status;
		}

		private void SaveUnnamedChartInfo()
		{
			StringBuilder builder = new StringBuilder();
			StringWriter writer = new StringWriter(builder);
			this._chartDef.Write(writer);
			int length;

			if (builder.Length > XML_BLOCK_SIZE)
			{
				// save the xml for subsequent transmitting
				this._currentXMLBuffer = builder;
				length = XML_BLOCK_SIZE;
			}
			else
			{
				this._currentXMLBuffer = null;
				length = builder.Length;
			}
			this._currentBlockNum = 0;

			// invoke the web service asynchronously
			string chartType = HandleChartUtil.GetChartTypeStr(_chartType);
			_isRequestComplete = false;
			if (_service == null)
			{
				_service = this._dataGridControl.CreateActiveXControlWebService();
			}
			_service.BeginSaveWorkingChartInfo(ConnectionString, chartType,
				builder.ToString(0, length),
				_currentBlockNum,
				new AsyncCallback(SaveUnnamedChartInfoDone),
				null);

			// launch a work in progress dialog
			this._workInProgressDialog.MaximumSteps = builder.Length / XML_BLOCK_SIZE + 1;
			this._workInProgressDialog.Value = 1;
			ShowWorkingDialog(MessageResourceManager.GetString("GraphWizard.SaveChartData"));
		}

		/// <summary>
		/// The AsyncCallback event handler for Web service method CacheChartInfoDone.
		/// </summary>
		/// <param name="res">The result</param>
		private void SaveUnnamedChartInfoDone(IAsyncResult res)
		{
			try
			{
				_service.EndSaveWorkingChartInfo(res);

				this._workInProgressDialog.PerformStep(); // move one step forward

				// It is a Worker thread, continue to send xml data if necessary
				if (_currentXMLBuffer != null)
				{
                    string chartType = HandleChartUtil.GetChartTypeStr(_chartType);

					while (true)
					{
						_currentBlockNum++;
						int start = _currentBlockNum * XML_BLOCK_SIZE;
						int length;
						if (start >= _currentXMLBuffer.Length)
						{
							_currentXMLBuffer = null;
							break;
						}
						else
						{
							if (_currentXMLBuffer.Length > (start + XML_BLOCK_SIZE))
							{
								length = XML_BLOCK_SIZE;
							}
							else
							{
								length = _currentXMLBuffer.Length - start;
							}
						}

						_service.SaveWorkingChartInfo(ConnectionString,
							chartType,
							_currentXMLBuffer.ToString(start, length),
							_currentBlockNum);

						this._workInProgressDialog.PerformStep(); // move one step forward
					}
				}

				this._chartDef.IsAltered = false;

				FireEvent();
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

		private delegate void FireEventDelegate();

		/// <summary>
		/// Display the query result data set in a data grid
		/// </summary>
		/// <param name="dataSet">The query result in DataSet</param>
		private void FireEvent()
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, fire event
				switch (this._eventType)
				{
					case EventType.ChartEvent:
						this._dataGridControl.FireGraphEvent();
						break;

					case EventType.DownloadEvent:
						this._dataGridControl.FireDownloadGraphEvent(_chartFormat.Name, _chartFormat.Suffix);
						break;
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				FireEventDelegate fireEvent = new FireEventDelegate(FireEvent);

				this.BeginInvoke(fireEvent);
			}
		}

		/// <summary>
		/// Determine if the chart name is unique
		/// </summary>
		/// <param name="name">The chart name</param>
		/// <returns>true if it is unique, false otherwise</returns>
		private bool IsChartNameUnique(string name)
		{
			bool status = true;

			if (_service == null)
			{
				_service = this._dataGridControl.CreateActiveXControlWebService();
			}

			try
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				status = _service.IsChartNameUnique(ConnectionString, name);
			}
			finally
			{
				// Restore the cursor
				Cursor.Current = this.Cursor;
			}

			return status;
		}

        /// <summary>
        /// Determine if the template name is unique
        /// </summary>
        /// <param name="name">The template name</param>
        /// <returns>true if it is unique, false otherwise</returns>
        private bool IsTemplateNameUnique(string name)
        {
            bool status = true;

            if (_service == null)
            {
                _service = this._dataGridControl.CreateActiveXControlWebService();
            }

            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                status = _service.IsTemplateNameUnique(ConnectionString, this._dataGridControl.BaseClassName, name);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }

            return status;
        }

		private void SaveNamedChartInfo()
		{
			string name = this.savedChartNameTextBox.Text;
			string desc = "";
			if (this.savedChartDescTextBox.Text != null)
			{
				desc = this.savedChartDescTextBox.Text;
			}

			StringBuilder builder = new StringBuilder();
			StringWriter writer = new StringWriter(builder);
			this._chartDef.Write(writer);
			int length;

			if (builder.Length > XML_BLOCK_SIZE)
			{
				// save the xml for subsequent transmitting
				this._currentXMLBuffer = builder;
				length = XML_BLOCK_SIZE;
			}
			else
			{
				this._currentXMLBuffer = null;
				length = builder.Length;
			}
			_currentBlockNum = 0;

            string chartType = HandleChartUtil.GetChartTypeStr(_chartType);

			// invoke the web service asynchronously
			_isRequestComplete = false;
			if (_service == null)
			{
				_service = this._dataGridControl.CreateActiveXControlWebService();
			}
			_service.BeginSaveNamedChartInfo(ConnectionString, name, desc,
				chartType, builder.ToString(0, length),
				_currentBlockNum,
				new AsyncCallback(SaveNamedChartInfoDone),
				null);

			// launch a work in progress dialog
			this._workInProgressDialog.MaximumSteps = builder.Length / XML_BLOCK_SIZE + 1;
			this._workInProgressDialog.Value = 1;
			ShowWorkingDialog(MessageResourceManager.GetString("GraphWizard.SaveChartData"));
		}

		/// <summary>
		/// The AsyncCallback event handler for Web service method SaveNamedChartInfo.
		/// </summary>
		/// <param name="res">The result</param>
		private void SaveNamedChartInfoDone(IAsyncResult res)
		{
			try
			{
				_service.EndSaveNamedChartInfo(res);

				this._workInProgressDialog.PerformStep(); // move one step forward

				// It is a Worker thread, continue to send xml data if necessary
				if (_currentXMLBuffer != null)
				{
					string name = this.savedChartNameTextBox.Text;
					string desc = "";
					if (this.savedChartDescTextBox.Text != null)
					{
						desc = this.savedChartDescTextBox.Text;
					}
                    string chartType = HandleChartUtil.GetChartTypeStr(_chartType);

					while (true)
					{
						_currentBlockNum++;
						int start = _currentBlockNum * XML_BLOCK_SIZE;
						int length;
						if (start >= _currentXMLBuffer.Length)
						{
							_currentXMLBuffer = null;
							break;
						}
						else
						{
							if (_currentXMLBuffer.Length > (start + XML_BLOCK_SIZE))
							{
								length = XML_BLOCK_SIZE;
							}
							else
							{
								length = _currentXMLBuffer.Length - start;
							}
						}

						_service.SaveNamedChartInfo(ConnectionString,
							name, desc,
							chartType,
							_currentXMLBuffer.ToString(start, length),
							_currentBlockNum);

						this._workInProgressDialog.PerformStep(); // move one step forward
					}
				}

                MessageBox.Show(MessageResourceManager.GetString("GraphWizard.SaveChartDone"));
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

        private void SaveNamedChartTemplate()
        {
            string name = this.savedChartNameTextBox.Text;
            string desc = "";
            if (this.savedChartDescTextBox.Text != null)
            {
                desc = this.savedChartDescTextBox.Text;
            }

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            this._chartDef.Write(writer);

            string chartType = HandleChartUtil.GetChartTypeStr(_chartType);

            // invoke the web service synchronously
            if (_service == null)
            {
                _service = this._dataGridControl.CreateActiveXControlWebService();
            }

            try
            {
                _service.SaveNamedChartTemplate(ConnectionString,
                    _dataGridControl.BaseClassName,
                    name, desc,
                    chartType, builder.ToString());

                MessageBox.Show(MessageResourceManager.GetString("GraphWizard.SaveTemplateDone"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

		/// <summary>
		/// Show the working dialog
		/// </summary>
		/// <param name="msg">The message to display on the working dialog</param>
		/// <remarks>It has to deal with multi-threading issue</remarks>
		internal void ShowWorkingDialog(string msg)
		{
			lock (_workInProgressDialog)
			{
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
				this.BeginInvoke(hideWorkingDialog);
			}
		}

		private void LoadChartInfos()
		{
			try
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				if (_service == null)
				{
					_service = this._dataGridControl.CreateActiveXControlWebService();
				}

				string xml = _service.GetChartInfos(ConnectionString);

				_chartInfos = new ChartInfoCollection();
				StringReader reader = new StringReader(xml);
				_chartInfos.Read(reader);
			}
			finally
			{
				// Restore the cursor
				Cursor.Current = this.Cursor;
			}
		}

        private void LoadChartTemplates()
        {
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            if (_service == null)
            {
                _service = this._dataGridControl.CreateActiveXControlWebService();
            }

            _chartTemplates = HandleChartUtil.GetChartTemplates(_service, ConnectionString, _dataGridControl.BaseClassName);
        }

        private void DisplayExistingCharts(ChartType theChartType)
        {
            if (_chartInfos == null)
            {
                LoadChartInfos();
            }

            ChartType chartType;
            ChartInfoCollection charts = new ChartInfoCollection();
            foreach (ChartInfo chartInfo in this._chartInfos)
            {
                chartType = (ChartType)Enum.Parse(typeof(ChartType), chartInfo.Type);
                if (chartType == theChartType)
                {
                    charts.Add(chartInfo);
                }
            }

            this.existingChartsListBox.Items.Clear();
            this.existingChartsListBox.DisplayMember = "Name";
            for (int i = 0; i < charts.Count; i++)
            {
                this.existingChartsListBox.Items.Add(charts[i]);
            }
        }

        private void DisplayExistingTemplates(ChartType theChartType)
        {
            if (_chartTemplates == null)
            {
                LoadChartTemplates();
            }

            ChartType chartType;
            ChartInfoCollection templates = new ChartInfoCollection();
            foreach (ChartInfo chartInfo in this._chartTemplates)
            {
                chartType = (ChartType)Enum.Parse(typeof(ChartType), chartInfo.Type);
                if (chartType == theChartType)
                {
                    templates.Add(chartInfo);
                }
            }

            this.existingChartTemplateListBox.Items.Clear();
            this.existingChartTemplateListBox.DisplayMember = "Name";
            for (int i = 0; i < templates.Count; i++)
            {
                this.existingChartTemplateListBox.Items.Add(templates[i]);
            }
        }

		#endregion

		private void wizard1_AfterSwitchPages(object sender, Newtera.DataGridActiveX.Wizard.AfterSwitchPagesEventArgs e)
		{
			// get wizard page to be displayed
			WizardPage newPage = this.wizard1.Pages[e.NewIndex];

			wizard1.CancelText = MessageResourceManager.GetString("GraphWizard.Cancel");

			if (newPage == this.lineChartWizardPage && e.NewIndex > e.OldIndex)
			{
				if (_selectedChartInfo == null)
				{
					// create a new LineChartDef object
					this._chartDef = new LineChartDef();

					// set the title
					if (this.lineChartTitleTextBox.Text != null)
					{
						this._chartDef.Title = this.lineChartTitleTextBox.Text;
					}

					ShowLineChartDef();
				}
				else
				{
					// Get the existing LineChartDef object from the database.
					GetExistingChartDef();
				}
			}
			else if (newPage == this.contourChartWizardPage && e.NewIndex > e.OldIndex)
			{
				if (_selectedChartInfo == null)
				{
					// create a new ContourChartDef object
					this._chartDef = new ContourChartDef();

					// set the title
					if (this.contourChartTitleTextBox.Text != null)
					{
						this._chartDef.Title = this.contourChartTitleTextBox.Text;
					}

					this._chartDef.Orientation = DataSeriesOrientation.ByRow; // default for contour
					ShowContourChartDef();
				}
				else
				{
					// Get the existing LineChartDef object from the database.
					GetExistingChartDef();
				}
			}
			else if (newPage == this.viewChartWizardPage)
			{
				wizard1.BackEnabled = true;
				wizard1.CancelText = MessageResourceManager.GetString("GraphWizard.Finish");
			}
		}

		private void wizard1_BeforeSwitchPages(object sender, Newtera.DataGridActiveX.Wizard.BeforeSwitchPagesEventArgs e)
		{
			// get wizard page already displayed
			WizardPage oldPage = this.wizard1.Pages[e.OldIndex];

			// check if we're going forward from options page
			if (oldPage == this.chartTypeWizardPage)
			{
				if (this.lineRadioButton.Checked)
				{
					this._chartType = ChartType.Line;
				}
				else if (this.contourRadioButton.Checked)
				{
					this._chartType = ChartType.Contour;
					e.NewIndex++; // go to the next page for contour settings
				}

                if (_selectedChartInfoType == ChartInfoType.SavedChart)
                {
					this._selectedChartInfo = (ChartInfo) this.existingChartsListBox.SelectedItem;
				}
                else if (_selectedChartInfoType == ChartInfoType.SavedTemplate)
                {
                    this._selectedChartInfo = (ChartInfo)this.existingChartTemplateListBox.SelectedItem;
                }
                else
                {
                    this._selectedChartInfo = null;
                }
			}
			else if (oldPage == this.lineChartWizardPage && e.NewIndex > e.OldIndex)
			{
				// next is clicked, check if any line are defined
				LineChartDef lineChartDef = (LineChartDef) this._chartDef;
				if (lineChartDef.Lines.Count == 0)
				{
					MessageBox.Show(MessageResourceManager.GetString("GraphWizard.MissingLines"),
						"Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);

					e.Cancel = true; // cancel the next event
				}
				else
				{
					foreach (LineDef line in lineChartDef.Lines)
					{
						// only check Y Data Series. when X Data Series is empty
						// it will be auto-generated
						if (line.YAxis.SeriesName == null)
						{
							MessageBox.Show(line.Name + " " + MessageResourceManager.GetString("GraphWizard.MissingYDef"),
								"Error", MessageBoxButtons.OK,
								MessageBoxIcon.Error);

							e.Cancel = true; // cancel next
							break;
						}
					}

					e.NewIndex++; // skip the contour page
				}
			}
			else if (oldPage == this.contourChartWizardPage && e.NewIndex < e.OldIndex)
			{
				//Jump to the chart type page
				e.NewIndex = e.NewIndex - 1;
			}
			else if (oldPage == this.viewChartWizardPage && e.NewIndex < e.OldIndex)
			{
				if (this._chartDef is LineChartDef)
				{
					// jump to the line chart def page
					e.NewIndex = e.NewIndex - 1;
				}
			}
		}

		private void wizard1_Cancel(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}

		private void viewChartButton_Click(object sender, System.EventArgs e)
		{
			if (this._dataGridControl != null)
			{
				this._eventType = EventType.ChartEvent;

				if (this._chartDef.IsAltered)
				{
					// re-generate chart info
                    HandleChartUtil.FillDataSeries(_chartDef, _dataGridControl);
					if (!this.IsForWindowsClient)
					{
                        // save the chart to server so that web client can download it from the server
						SaveUnnamedChartInfo();
					}
					else
					{
						this._chartDef.IsAltered = false;
						FireEvent();
					}
				}
				else
				{
					// nothing has been changed, the chart info has been cached on
					// server side. just fire the event
					FireEvent();
				}
			}
		}

		private void GraphWizard_Load(object sender, System.EventArgs e)
		{
			// check line chart as default type
			this.lineRadioButton.Checked = true;

			wizard1.CancelText = MessageResourceManager.GetString("GraphWizard.Cancel");

			try
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				if (_service == null)
				{
					_service = this._dataGridControl.CreateActiveXControlWebService();
				}

                _chartFormats = HandleChartUtil.GetChartFormats(_service);
				
				if (_chartFormats.Count > 0)
				{
					chartFormatComboBox.DataSource = _chartFormats;
					chartFormatComboBox.DisplayMember = "Name";
					chartFormatComboBox.SelectedIndex = 0;
				}

                if (!_isDBA)
                {
                    this.deleteTemplateButton.Enabled = false;
                    this.saveTemplateButton.Enabled = false;
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

		private void allRowsRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.allRowsRadioButton.Checked)
			{
				((LineChartDef) this._chartDef).UseSelectedRows = false;
			}
			else
			{
				((LineChartDef) this._chartDef).UseSelectedRows = true;
			}
		}

		private void byColumnsRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.byColumnsRadioButton.Checked)
			{
				((LineChartDef) this._chartDef).Orientation = DataSeriesOrientation.ByColumn;
			}
			else
			{
				((LineChartDef) this._chartDef).Orientation = DataSeriesOrientation.ByRow;
			}
		}

		private void selectXAxisButton_Click(object sender, System.EventArgs e)
		{
			SelectDataSeriesDialog dialog = new SelectDataSeriesDialog();
			dialog.DataGridControl = _dataGridControl;
			dialog.ChartDefinition = _chartDef;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				this.lineXDataSeriesTextBox.Text = dialog.SelectedSeriesCaption;
				if (this.lineListBox.SelectedIndex >= 0)
				{
					LineDef lineDef = (LineDef) this.lineListBox.SelectedItem;
				
					lineDef.XAxis.SeriesCaption = dialog.SelectedSeriesCaption;
					lineDef.XAxis.SeriesName = dialog.SelectedSeriesName;
				}
			}
		}

		private void selectYAxisButton_Click(object sender, System.EventArgs e)
		{
			SelectDataSeriesDialog dialog = new SelectDataSeriesDialog();
			dialog.DataGridControl = _dataGridControl;
			dialog.ChartDefinition = _chartDef;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				this.lineYDataSeriesTextBox.Text = dialog.SelectedSeriesCaption;
				if (this.lineListBox.SelectedIndex >= 0)
				{
					LineDef lineDef = (LineDef) this.lineListBox.SelectedItem;

					lineDef.YAxis.SeriesCaption = dialog.SelectedSeriesCaption;
					lineDef.YAxis.SeriesName = dialog.SelectedSeriesName;
				}
			}
		}

		private void lineListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.lineListBox.SelectedIndex >= 0)
			{
				// LineChartDef may contains existing lines, do not modify those lines
				LineDef selectedLine = (LineDef) this.lineListBox.SelectedItem;
				if (selectedLine.IsNew)
				{
					this.removeLineButton.Enabled = true;
					this.selectXAxisButton.Enabled = true;
					this.selectYAxisButton.Enabled = true;
					//this.lineXAxisLabelTextBox.Enabled = true;
					//this.lineYAxisLabelTextBox.Enabled = true;
				}
				else
				{
					this.removeLineButton.Enabled = true;
					this.selectXAxisButton.Enabled = false;
					this.selectYAxisButton.Enabled = false;
					//this.lineXAxisLabelTextBox.Enabled = false;
					//this.lineYAxisLabelTextBox.Enabled = false;
				}
				ShowSelectedLine(selectedLine);
			}
			else
			{
				this.removeLineButton.Enabled = false;
				this.selectXAxisButton.Enabled = false;
				this.selectYAxisButton.Enabled = false;
				this.lineXAxisLabelTextBox.Enabled = false;
				this.lineYAxisLabelTextBox.Enabled = false;
				ShowSelectedLine(null);
			}
		}

		private void addLineButton_Click(object sender, System.EventArgs e)
		{
			LineDef line = new LineDef();
			string lineName = MessageResourceManager.GetString("GraphWizard.Line") + (((LineChartDef) this._chartDef).Lines.Count + 1);
			line.Name = lineName;
			((LineChartDef) this._chartDef).Lines.Add(line);
			this.lineListBox.Items.Add(line);
			this.lineListBox.SelectedItem = line;
		}

		private void removeLineButton_Click(object sender, System.EventArgs e)
		{
			if (this.lineListBox.SelectedIndex >= 0)
			{
				((LineChartDef) this._chartDef).Lines.Remove((LineDef) this.lineListBox.SelectedItem);
				this.lineListBox.Items.RemoveAt(this.lineListBox.SelectedIndex);
				if (this.lineListBox.Items.Count > 0)
				{
					this.lineListBox.SelectedIndex = 0;
				}
			}
		}

		private void lineNameTextBox_Leave(object sender, System.EventArgs e)
		{
			RenameLineName();
		}

		private void lineNameTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) 
			{
				RenameLineName();
			}
		}

		private void lineXAxisLabelTextBox_Leave(object sender, System.EventArgs e)
		{
			if (lineListBox.SelectedIndex >= 0)
			{
				LineDef lineDef = (LineDef) this.lineListBox.SelectedItem;
				lineDef.XAxis.Label = this.lineXAxisLabelTextBox.Text;
			}
		}

		private void lineYAxisLabelTextBox_Leave(object sender, System.EventArgs e)
		{
			if (lineListBox.SelectedIndex >= 0)
			{
				LineDef lineDef = (LineDef) this.lineListBox.SelectedItem;
				lineDef.YAxis.Label = this.lineYAxisLabelTextBox.Text;
			}
		}

		private void oneXOneYRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.oneXOneYRadioButton.Checked)
			{
				((LineChartDef) this._chartDef).DisplayMethod = LineChartDisplayMethod.OneXOneY;
			}
		}

		private void oneXManyYRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.oneXManyYRadioButton.Checked)
			{
				((LineChartDef) this._chartDef).DisplayMethod = LineChartDisplayMethod.OneXManyY;
			}
		}

		private void manyXOneYRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.manyXOneYRadioButton.Checked)
			{
				((LineChartDef) this._chartDef).DisplayMethod = LineChartDisplayMethod.ManyXOneY;
			}
		}

		private void separateCoordinatesRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.separateCoordinatesRadioButton.Checked)
			{
				((LineChartDef) this._chartDef).DisplayMethod = LineChartDisplayMethod.ManyCoordinates;
			}	
		}

		private void saveChartButton_Click(object sender, System.EventArgs e)
		{
			if (savedChartNameTextBox.Text == null ||
				savedChartNameTextBox.Text.Length == 0)
			{
				MessageBox.Show(MessageResourceManager.GetString("GraphWizard.EnterChartName"),
					"Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			else 
			{
				// if it a new chart, make sure the name is unique
				if (this._selectedChartInfo == null && !IsChartNameUnique(savedChartNameTextBox.Text))
				{
					MessageBox.Show(MessageResourceManager.GetString("GraphWizard.ChartNameExist"),
						"Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);

					return;
				}

				if (this._dataGridControl != null)
				{
					if (this._chartDef.IsAltered)
					{
						// re-generate chart info
                        HandleChartUtil.FillDataSeries(_chartDef, _dataGridControl);
						SaveNamedChartInfo();
					}
				}
			}
		}

		private void savedChartTitleTextBox_Leave(object sender, System.EventArgs e)
		{
			this._chartDef.Name = savedChartNameTextBox.Text;
		}

		private void savedChartDescTextBox_Leave(object sender, System.EventArgs e)
		{
			this._chartDef.Description = savedChartDescTextBox.Text;
		}

		private void lineChartTitleTextBox_Leave(object sender, System.EventArgs e)
		{
			this._chartDef.Title = this.lineChartTitleTextBox.Text;
		}

		private void lineRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.lineRadioButton.Checked)
			{
                DisplayExistingCharts(ChartType.Line);
                DisplayExistingTemplates(ChartType.Line);
			}
		}

		private void contourRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.contourRadioButton.Checked)
			{
                DisplayExistingCharts(ChartType.Contour);
                DisplayExistingTemplates(ChartType.Contour);
			}
		}

		private void existingChartsListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.existingChartsListBox.SelectedIndex >= 0)
			{
				ChartInfo chartInfo = (ChartInfo) this.existingChartsListBox.SelectedItem;
				this.chartDescTextBox.Text = chartInfo.Description;
                this._selectedChartInfoType = ChartInfoType.SavedChart;
                this.saveTemplateButton.Enabled = false;
			}
			else
			{
				this.chartDescTextBox.Text = null;
                this._selectedChartInfoType = ChartInfoType.Unknown;
                if (_isDBA)
                {
                    this.saveTemplateButton.Enabled = true;
                }
			}
		}

		private void contourByColumnRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.contourByColumnRadioButton.Checked)
			{
				this._chartDef.Orientation = DataSeriesOrientation.ByColumn;
			}
			else
			{
				this._chartDef.Orientation = DataSeriesOrientation.ByRow;
			}
		}

		private void contourXAxisStartValueTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;
			try
			{
				if (this.contourXAxisStartValueTextBox.Text != null &&
					this.contourXAxisStartValueTextBox.Text.Length > 0)
				{
					contour.XStart = float.Parse(this.contourXAxisStartValueTextBox.Text);
				}
			}
			catch (Exception)
			{
			}
		}

		private void contourXAxisStepValueTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;
			try
			{
				if (this.contourXAxisStepValueTextBox.Text != null &&
					this.contourXAxisStepValueTextBox.Text.Length > 0)
				{
					contour.XStep = float.Parse(this.contourXAxisStepValueTextBox.Text);
				}
			}
			catch (Exception)
			{
			}
		}

		private void contourXAxisLabelTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;

			contour.XAxisLabel = this.contourXAxisLabelTextBox.Text;
		}

		private void contourYAxisStartValueTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;
			try
			{
				if (this.contourYAxisStartValueTextBox.Text != null &&
					this.contourYAxisStartValueTextBox.Text.Length > 0)
				{
					contour.YStart = float.Parse(this.contourYAxisStartValueTextBox.Text);
				}
			}
			catch (Exception)
			{
			}
		}

		private void contourYAxisStepValueTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;
			try
			{
				if (this.contourYAxisStepValueTextBox.Text != null &&
					this.contourYAxisStepValueTextBox.Text.Length > 0)
				{
					contour.YStep = float.Parse(this.contourYAxisStepValueTextBox.Text);
				}
			}
			catch (Exception)
			{
			}
		}

		private void contourYAxisLabelTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;

			contour.YAxisLabel = this.contourYAxisLabelTextBox.Text;
		}

		private void countourZAxisLabelTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;

			contour.ZAxisLabel = this.countourZAxisLabelTextBox.Text;
		}

		private void contourChartTitleTextBox_Leave(object sender, System.EventArgs e)
		{
			ContourChartDef contour = (ContourChartDef) _chartDef;

			contour.Title = this.contourChartTitleTextBox.Text;
		}

		private void contourLegengLocationComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (contourLegengLocationComboBox.SelectedIndex >= 0)
			{
				switch (contourLegengLocationComboBox.SelectedIndex)
				{
					case 0:
						_chartDef.LegendLocation = LegendLocation.Right;
						break;

					case 1:
						_chartDef.LegendLocation = LegendLocation.Left;
						break;
				}
			}
		}

		private void contourWholeAreaRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.contourWholeAreaRadioButton.Checked)
			{
				this._chartDef.UseSelectedRows = false;
			}
			else
			{
				this._chartDef.UseSelectedRows = true;
			}
		}

		private void downloadButton_Click(object sender, System.EventArgs e)
		{
			if (this._dataGridControl != null)
			{
				this._eventType = EventType.DownloadEvent;
				this._chartFormat = (ChartFormat) _chartFormats[this.chartFormatComboBox.SelectedIndex];

				if (this._chartDef.IsAltered)
				{
					// re-generate chart info
                    HandleChartUtil.FillDataSeries(_chartDef, _dataGridControl);
					if (!this.IsForWindowsClient)
					{
						SaveUnnamedChartInfo();
					}
					else
					{
						this._chartDef.IsAltered = false;
						FireEvent();
					}
				}
				else
				{
					// nothing has been changed, the chart info has been cached on
					// server side. just fire the event
					FireEvent();
				}
			}
		}

        private void saveTemplateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(savedChartNameTextBox.Text))
            {
                MessageBox.Show(MessageResourceManager.GetString("GraphWizard.EnterChartName"),
                    "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                // if it a new template, make sure the name is unique
                if (this._selectedChartInfo == null && !IsTemplateNameUnique(savedChartNameTextBox.Text))
                {
                    MessageBox.Show(MessageResourceManager.GetString("GraphWizard.TemplateNameExist"),
                        "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                if (this._dataGridControl != null)
                {
                    if (this._chartDef.IsAltered)
                    {
                        HandleChartUtil.ClearDataSeries(_chartDef); // we only need to save the chart definition, not data series
                        SaveNamedChartTemplate();
                    }
                }
            }
        }

        private void existingChartTemplateListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.existingChartTemplateListBox.SelectedIndex >= 0)
            {
                ChartInfo chartInfo = (ChartInfo)this.existingChartTemplateListBox.SelectedItem;
                if (_isDBA)
                {
                    this.deleteTemplateButton.Enabled = true;
                }
                this._selectedChartInfoType = ChartInfoType.SavedTemplate;
                if (_isDBA)
                {
                    this.saveTemplateButton.Enabled = true;
                }
                this.templateDescTextBox.Text = chartInfo.Description;
            }
            else
            {
                this.deleteTemplateButton.Enabled = false;
                this.templateDescTextBox.Text = "";
                this._selectedChartInfoType = ChartInfoType.Unknown;
                if (_isDBA)
                {
                    this.saveTemplateButton.Enabled = true;
                }
            }
        }

        private void deleteTemplateButton_Click(object sender, EventArgs e)
        {
            if (this.existingChartTemplateListBox.SelectedIndex >= 0)
            {
                ChartInfo chartInfo = (ChartInfo)this.existingChartTemplateListBox.SelectedItem;

                if (_service == null)
                {
                    _service = this._dataGridControl.CreateActiveXControlWebService();
                }

                try
                {
                    _service.DeleteChartTemplateById(chartInfo.ID);

                    this.existingChartTemplateListBox.Items.Remove(chartInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
	}

    internal enum ChartInfoType
    {
        Unknown,
        SavedTemplate,
        SavedChart
    }
}

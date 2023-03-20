using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.DataGridActiveX.Export;
using Newtera.DataGridActiveX.ActiveXControlWebService;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// ExportDialog 的摘要说明。
	/// </summary>
	public class ExportDialog : System.Windows.Forms.Form
	{
		private ActiveXControlService _service;
		private ExportTypeCollection _exportTypes;
		private ExportType _selectedType;
		private bool _exportAll;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton allRowsRadioButton;
		private System.Windows.Forms.RadioButton selectedRowsRadioButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox dataTypeComboBox;
		private System.Windows.Forms.Label label1;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ExportDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_service = null;
			_exportTypes = null;
			_selectedType = null;
			_exportAll = true; // default
		}

		/// <summary>
		/// Gets or sets the Web Service
		/// </summary>
		public ActiveXControlService WebService
		{
			get
			{
				return _service;
			}
			set
			{
				_service = value;
			}
		}

		/// <summary>
		/// Gets selected export type
		/// </summary>
		public ExportType SelectedExportType
		{
			get
			{
				return this._selectedType;
			}
		}

		/// <summary>
		/// Gets the information indicating whether to export all rows
		/// </summary>
		/// <value>True to export all rows, false to export selected rows</value>
		public bool ExportAll
		{
			get
			{
				return this._exportAll;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectedRowsRadioButton = new System.Windows.Forms.RadioButton();
            this.allRowsRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataTypeComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
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
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.dataTypeComboBox);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dataTypeComboBox
            // 
            resources.ApplyResources(this.dataTypeComboBox, "dataTypeComboBox");
            this.dataTypeComboBox.Name = "dataTypeComboBox";
            // 
            // ExportDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExportDialog";
            this.Load += new System.EventHandler(this.ExportDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (this.dataTypeComboBox.SelectedIndex >= 0 &&
				this._exportTypes != null)
			{
				_selectedType = this._exportTypes[this.dataTypeComboBox.SelectedIndex];
			}

			if (this.allRowsRadioButton.Checked)
			{
				this._exportAll = true;
			}
			else
			{
				this._exportAll = false;
			}
		}

		private void ExportDialog_Load(object sender, System.EventArgs e)
		{
			try
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				string xml = _service.GetExportTypesInXml();

				_exportTypes = new ExportTypeCollection();
				StringReader reader = new StringReader(xml);
				_exportTypes.Read(reader);

				if (_exportTypes.Count > 0)
				{
					dataTypeComboBox.DataSource = _exportTypes;
					dataTypeComboBox.DisplayMember = "Name";
					dataTypeComboBox.SelectedIndex = 0;
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
	}
}

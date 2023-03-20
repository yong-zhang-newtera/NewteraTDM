using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Mappings;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// SelectImportPackageDialog 的摘要说明。
	/// </summary>
	public class SelectImportPackageDialog : System.Windows.Forms.Form
	{
		private string _selectedPackage;
		private MappingManager _mappingManager;
		private DataSourceType _dataSourceType;
		private MappingPackageCollection _importPackages;
		private ImportWizard _importWizard;

		private System.Windows.Forms.ListBox importPackageListBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox importPackageTextBox;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectImportPackageDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_selectedPackage = null;
			_mappingManager = null;
			_dataSourceType = DataSourceType.Unknown;
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

		/// <summary>
		/// Gets or sets the selected package
		/// </summary>
		public string SelectedPackage
		{
			get
			{
				return this._selectedPackage;
			}
			set
			{
				this._selectedPackage = value;
			}
		}

		/// <summary>
		/// Gets or sets the MappingManager
		/// </summary>
		public MappingManager MappingManager
		{
			get
			{
				return this._mappingManager;
			}
			set
			{
				this._mappingManager = value;
			}
		}

		/// <summary>
		/// Gets or sets the Data Source Type
		/// </summary>
		public DataSourceType DataSourceType
		{
			get
			{
				return this._dataSourceType;
			}
			set
			{
				this._dataSourceType = value;
			}
		}

		/// <summary>
		/// Gets or sets the ImportWizard instance
		/// </summary>
		public ImportWizard ImportWizard
		{
			get
			{
				return this._importWizard;
			}
			set
			{
				this._importWizard = value;
			}
		}

		/// <summary>
		/// Delete the selected package
		/// </summary>
		private void DeleteImportPackage()
		{
			if (importPackageListBox.SelectedIndex >= 0)
			{
				MappingPackage package = (MappingPackage) _importPackages[importPackageListBox.SelectedIndex];
				this._mappingManager.RemoveMappingPackage(package);

				this._importWizard.SaveImportPackage();
				
				this.deleteButton.Enabled = false;
				this.okButton.Enabled = false;

				ShowPackageNames();
			}
		}

		/// <summary>
		/// Display the import package names in the list box
		/// </summary>
		private void ShowPackageNames()
		{
			// display the predefined import packages for the selected data type
			_importPackages = _mappingManager.GetMappingPackages(_dataSourceType);

			importPackageListBox.Items.Clear();

			foreach (MappingPackage package in _importPackages)
			{
				importPackageListBox.Items.Add(package.Name);
			}

			if (this._selectedPackage != null && this._selectedPackage.Length > 0)
			{
				importPackageListBox.SelectedItem = this._selectedPackage;
			}
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectImportPackageDialog));
            this.importPackageListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.importPackageTextBox = new System.Windows.Forms.TextBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // importPackageListBox
            // 
            resources.ApplyResources(this.importPackageListBox, "importPackageListBox");
            this.importPackageListBox.Name = "importPackageListBox";
            this.importPackageListBox.SelectedIndexChanged += new System.EventHandler(this.importPackageListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // importPackageTextBox
            // 
            resources.ApplyResources(this.importPackageTextBox, "importPackageTextBox");
            this.importPackageTextBox.Name = "importPackageTextBox";
            this.importPackageTextBox.ReadOnly = true;
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // SelectImportPackageDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.importPackageTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.importPackageListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SelectImportPackageDialog";
            this.Load += new System.EventHandler(this.SelectImportPackageDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void importPackageListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (importPackageListBox.SelectedIndex >= 0)
			{
				this._selectedPackage = (string) importPackageListBox.SelectedItem;
				MappingPackage package = (MappingPackage) _importPackages[importPackageListBox.SelectedIndex];
				importPackageTextBox.Text = package.Description;
				this.deleteButton.Enabled = true;
				this.okButton.Enabled = true;
			}
		}

		private void SelectImportPackageDialog_Load(object sender, System.EventArgs e)
		{
			ShowPackageNames();
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show(ImportExportResourceManager.GetString("ImportWizard.ConfirmDeletePackage"),
				"Confirm Dialog", MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DeleteImportPackage();
			}
		}
	}
}

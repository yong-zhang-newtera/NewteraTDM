using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// ShowSourceTableDialog 的摘要说明。
	/// </summary>
	public class ShowSourceTableDialog : System.Windows.Forms.Form
	{
		private DataTable _sourceTable;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox totalRowTextBox;
		private System.Windows.Forms.DataGrid srcTableDataGrid;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ShowSourceTableDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		public DataTable SourceDataTable
		{
			get
			{
				return _sourceTable;
			}
			set
			{
				_sourceTable = value;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ShowSourceTableDialog));
			this.okButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.totalRowTextBox = new System.Windows.Forms.TextBox();
			this.srcTableDataGrid = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.srcTableDataGrid)).BeginInit();
			this.SuspendLayout();
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
			// totalRowTextBox
			// 
			this.totalRowTextBox.AccessibleDescription = resources.GetString("totalRowTextBox.AccessibleDescription");
			this.totalRowTextBox.AccessibleName = resources.GetString("totalRowTextBox.AccessibleName");
			this.totalRowTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("totalRowTextBox.Anchor")));
			this.totalRowTextBox.AutoSize = ((bool)(resources.GetObject("totalRowTextBox.AutoSize")));
			this.totalRowTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("totalRowTextBox.BackgroundImage")));
			this.totalRowTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("totalRowTextBox.Dock")));
			this.totalRowTextBox.Enabled = ((bool)(resources.GetObject("totalRowTextBox.Enabled")));
			this.totalRowTextBox.Font = ((System.Drawing.Font)(resources.GetObject("totalRowTextBox.Font")));
			this.totalRowTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("totalRowTextBox.ImeMode")));
			this.totalRowTextBox.Location = ((System.Drawing.Point)(resources.GetObject("totalRowTextBox.Location")));
			this.totalRowTextBox.MaxLength = ((int)(resources.GetObject("totalRowTextBox.MaxLength")));
			this.totalRowTextBox.Multiline = ((bool)(resources.GetObject("totalRowTextBox.Multiline")));
			this.totalRowTextBox.Name = "totalRowTextBox";
			this.totalRowTextBox.PasswordChar = ((char)(resources.GetObject("totalRowTextBox.PasswordChar")));
			this.totalRowTextBox.ReadOnly = true;
			this.totalRowTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("totalRowTextBox.RightToLeft")));
			this.totalRowTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("totalRowTextBox.ScrollBars")));
			this.totalRowTextBox.Size = ((System.Drawing.Size)(resources.GetObject("totalRowTextBox.Size")));
			this.totalRowTextBox.TabIndex = ((int)(resources.GetObject("totalRowTextBox.TabIndex")));
			this.totalRowTextBox.Text = resources.GetString("totalRowTextBox.Text");
			this.totalRowTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("totalRowTextBox.TextAlign")));
			this.totalRowTextBox.Visible = ((bool)(resources.GetObject("totalRowTextBox.Visible")));
			this.totalRowTextBox.WordWrap = ((bool)(resources.GetObject("totalRowTextBox.WordWrap")));
			// 
			// srcTableDataGrid
			// 
			this.srcTableDataGrid.AccessibleDescription = resources.GetString("srcTableDataGrid.AccessibleDescription");
			this.srcTableDataGrid.AccessibleName = resources.GetString("srcTableDataGrid.AccessibleName");
			this.srcTableDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("srcTableDataGrid.Anchor")));
			this.srcTableDataGrid.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("srcTableDataGrid.BackgroundImage")));
			this.srcTableDataGrid.CaptionFont = ((System.Drawing.Font)(resources.GetObject("srcTableDataGrid.CaptionFont")));
			this.srcTableDataGrid.CaptionText = resources.GetString("srcTableDataGrid.CaptionText");
			this.srcTableDataGrid.CaptionVisible = false;
			this.srcTableDataGrid.DataMember = "";
			this.srcTableDataGrid.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("srcTableDataGrid.Dock")));
			this.srcTableDataGrid.Enabled = ((bool)(resources.GetObject("srcTableDataGrid.Enabled")));
			this.srcTableDataGrid.Font = ((System.Drawing.Font)(resources.GetObject("srcTableDataGrid.Font")));
			this.srcTableDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.srcTableDataGrid.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("srcTableDataGrid.ImeMode")));
			this.srcTableDataGrid.Location = ((System.Drawing.Point)(resources.GetObject("srcTableDataGrid.Location")));
			this.srcTableDataGrid.Name = "srcTableDataGrid";
			this.srcTableDataGrid.ReadOnly = true;
			this.srcTableDataGrid.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("srcTableDataGrid.RightToLeft")));
			this.srcTableDataGrid.RowHeadersVisible = false;
			this.srcTableDataGrid.Size = ((System.Drawing.Size)(resources.GetObject("srcTableDataGrid.Size")));
			this.srcTableDataGrid.TabIndex = ((int)(resources.GetObject("srcTableDataGrid.TabIndex")));
			this.srcTableDataGrid.Visible = ((bool)(resources.GetObject("srcTableDataGrid.Visible")));
			// 
			// ShowSourceTableDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.srcTableDataGrid);
			this.Controls.Add(this.totalRowTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.okButton);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ShowSourceTableDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.ShowSourceTableDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.srcTableDataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void ShowSourceTableDialog_Load(object sender, System.EventArgs e)
		{
			if (_sourceTable != null)
			{
				this.srcTableDataGrid.DataSource = _sourceTable;

				this.totalRowTextBox.Text = _sourceTable.Rows.Count.ToString();
			}
		}
	}
}

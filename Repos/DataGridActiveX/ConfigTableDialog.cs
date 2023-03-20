using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// ConfigTableDialog 的摘要说明。
	/// </summary>
	public class ConfigTableDialog : System.Windows.Forms.Form
	{
		private ColumnInfoCollection _columnInfos;

		private System.Windows.Forms.CheckedListBox columnCheckedListBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button checkAllButton;
		private System.Windows.Forms.Button uncheckAllButton;
		private System.Windows.Forms.Button upButton;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.Button downButton;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.IContainer components;

		public ConfigTableDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// Gets or sets a collection of column infos
		/// </summary>
		public ColumnInfoCollection ColumnInfos
		{
			get
			{
				return _columnInfos;
			}
			set
			{
				if (value != null)
				{
					_columnInfos = value.Clone();
				}
				else
				{
					_columnInfos = value;
				}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConfigTableDialog));
			this.columnCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.checkAllButton = new System.Windows.Forms.Button();
			this.uncheckAllButton = new System.Windows.Forms.Button();
			this.upButton = new System.Windows.Forms.Button();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.downButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// columnCheckedListBox
			// 
			this.columnCheckedListBox.AccessibleDescription = resources.GetString("columnCheckedListBox.AccessibleDescription");
			this.columnCheckedListBox.AccessibleName = resources.GetString("columnCheckedListBox.AccessibleName");
			this.columnCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("columnCheckedListBox.Anchor")));
			this.columnCheckedListBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("columnCheckedListBox.BackgroundImage")));
			this.columnCheckedListBox.ColumnWidth = ((int)(resources.GetObject("columnCheckedListBox.ColumnWidth")));
			this.columnCheckedListBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("columnCheckedListBox.Dock")));
			this.columnCheckedListBox.Enabled = ((bool)(resources.GetObject("columnCheckedListBox.Enabled")));
			this.columnCheckedListBox.Font = ((System.Drawing.Font)(resources.GetObject("columnCheckedListBox.Font")));
			this.columnCheckedListBox.HorizontalExtent = ((int)(resources.GetObject("columnCheckedListBox.HorizontalExtent")));
			this.columnCheckedListBox.HorizontalScrollbar = ((bool)(resources.GetObject("columnCheckedListBox.HorizontalScrollbar")));
			this.columnCheckedListBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("columnCheckedListBox.ImeMode")));
			this.columnCheckedListBox.IntegralHeight = ((bool)(resources.GetObject("columnCheckedListBox.IntegralHeight")));
			this.columnCheckedListBox.Location = ((System.Drawing.Point)(resources.GetObject("columnCheckedListBox.Location")));
			this.columnCheckedListBox.Name = "columnCheckedListBox";
			this.columnCheckedListBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("columnCheckedListBox.RightToLeft")));
			this.columnCheckedListBox.ScrollAlwaysVisible = ((bool)(resources.GetObject("columnCheckedListBox.ScrollAlwaysVisible")));
			this.columnCheckedListBox.Size = ((System.Drawing.Size)(resources.GetObject("columnCheckedListBox.Size")));
			this.columnCheckedListBox.TabIndex = ((int)(resources.GetObject("columnCheckedListBox.TabIndex")));
			this.columnCheckedListBox.Visible = ((bool)(resources.GetObject("columnCheckedListBox.Visible")));
			this.columnCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.columnCheckedListBox_SelectedIndexChanged);
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
			// checkAllButton
			// 
			this.checkAllButton.AccessibleDescription = resources.GetString("checkAllButton.AccessibleDescription");
			this.checkAllButton.AccessibleName = resources.GetString("checkAllButton.AccessibleName");
			this.checkAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("checkAllButton.Anchor")));
			this.checkAllButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkAllButton.BackgroundImage")));
			this.checkAllButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("checkAllButton.Dock")));
			this.checkAllButton.Enabled = ((bool)(resources.GetObject("checkAllButton.Enabled")));
			this.checkAllButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("checkAllButton.FlatStyle")));
			this.checkAllButton.Font = ((System.Drawing.Font)(resources.GetObject("checkAllButton.Font")));
			this.checkAllButton.Image = ((System.Drawing.Image)(resources.GetObject("checkAllButton.Image")));
			this.checkAllButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkAllButton.ImageAlign")));
			this.checkAllButton.ImageIndex = ((int)(resources.GetObject("checkAllButton.ImageIndex")));
			this.checkAllButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("checkAllButton.ImeMode")));
			this.checkAllButton.Location = ((System.Drawing.Point)(resources.GetObject("checkAllButton.Location")));
			this.checkAllButton.Name = "checkAllButton";
			this.checkAllButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("checkAllButton.RightToLeft")));
			this.checkAllButton.Size = ((System.Drawing.Size)(resources.GetObject("checkAllButton.Size")));
			this.checkAllButton.TabIndex = ((int)(resources.GetObject("checkAllButton.TabIndex")));
			this.checkAllButton.Text = resources.GetString("checkAllButton.Text");
			this.checkAllButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkAllButton.TextAlign")));
			this.checkAllButton.Visible = ((bool)(resources.GetObject("checkAllButton.Visible")));
			this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
			// 
			// uncheckAllButton
			// 
			this.uncheckAllButton.AccessibleDescription = resources.GetString("uncheckAllButton.AccessibleDescription");
			this.uncheckAllButton.AccessibleName = resources.GetString("uncheckAllButton.AccessibleName");
			this.uncheckAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("uncheckAllButton.Anchor")));
			this.uncheckAllButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("uncheckAllButton.BackgroundImage")));
			this.uncheckAllButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("uncheckAllButton.Dock")));
			this.uncheckAllButton.Enabled = ((bool)(resources.GetObject("uncheckAllButton.Enabled")));
			this.uncheckAllButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("uncheckAllButton.FlatStyle")));
			this.uncheckAllButton.Font = ((System.Drawing.Font)(resources.GetObject("uncheckAllButton.Font")));
			this.uncheckAllButton.Image = ((System.Drawing.Image)(resources.GetObject("uncheckAllButton.Image")));
			this.uncheckAllButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("uncheckAllButton.ImageAlign")));
			this.uncheckAllButton.ImageIndex = ((int)(resources.GetObject("uncheckAllButton.ImageIndex")));
			this.uncheckAllButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("uncheckAllButton.ImeMode")));
			this.uncheckAllButton.Location = ((System.Drawing.Point)(resources.GetObject("uncheckAllButton.Location")));
			this.uncheckAllButton.Name = "uncheckAllButton";
			this.uncheckAllButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("uncheckAllButton.RightToLeft")));
			this.uncheckAllButton.Size = ((System.Drawing.Size)(resources.GetObject("uncheckAllButton.Size")));
			this.uncheckAllButton.TabIndex = ((int)(resources.GetObject("uncheckAllButton.TabIndex")));
			this.uncheckAllButton.Text = resources.GetString("uncheckAllButton.Text");
			this.uncheckAllButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("uncheckAllButton.TextAlign")));
			this.uncheckAllButton.Visible = ((bool)(resources.GetObject("uncheckAllButton.Visible")));
			this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
			// 
			// upButton
			// 
			this.upButton.AccessibleDescription = resources.GetString("upButton.AccessibleDescription");
			this.upButton.AccessibleName = resources.GetString("upButton.AccessibleName");
			this.upButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("upButton.Anchor")));
			this.upButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("upButton.BackgroundImage")));
			this.upButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("upButton.Dock")));
			this.upButton.Enabled = ((bool)(resources.GetObject("upButton.Enabled")));
			this.upButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("upButton.FlatStyle")));
			this.upButton.Font = ((System.Drawing.Font)(resources.GetObject("upButton.Font")));
			this.upButton.Image = ((System.Drawing.Image)(resources.GetObject("upButton.Image")));
			this.upButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("upButton.ImageAlign")));
			this.upButton.ImageIndex = ((int)(resources.GetObject("upButton.ImageIndex")));
			this.upButton.ImageList = this.imageList;
			this.upButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("upButton.ImeMode")));
			this.upButton.Location = ((System.Drawing.Point)(resources.GetObject("upButton.Location")));
			this.upButton.Name = "upButton";
			this.upButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("upButton.RightToLeft")));
			this.upButton.Size = ((System.Drawing.Size)(resources.GetObject("upButton.Size")));
			this.upButton.TabIndex = ((int)(resources.GetObject("upButton.TabIndex")));
			this.upButton.Text = resources.GetString("upButton.Text");
			this.upButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("upButton.TextAlign")));
			this.upButton.Visible = ((bool)(resources.GetObject("upButton.Visible")));
			this.upButton.Click += new System.EventHandler(this.upButton_Click);
			// 
			// imageList
			// 
			this.imageList.ImageSize = ((System.Drawing.Size)(resources.GetObject("imageList.ImageSize")));
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// downButton
			// 
			this.downButton.AccessibleDescription = resources.GetString("downButton.AccessibleDescription");
			this.downButton.AccessibleName = resources.GetString("downButton.AccessibleName");
			this.downButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("downButton.Anchor")));
			this.downButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("downButton.BackgroundImage")));
			this.downButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("downButton.Dock")));
			this.downButton.Enabled = ((bool)(resources.GetObject("downButton.Enabled")));
			this.downButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("downButton.FlatStyle")));
			this.downButton.Font = ((System.Drawing.Font)(resources.GetObject("downButton.Font")));
			this.downButton.Image = ((System.Drawing.Image)(resources.GetObject("downButton.Image")));
			this.downButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("downButton.ImageAlign")));
			this.downButton.ImageIndex = ((int)(resources.GetObject("downButton.ImageIndex")));
			this.downButton.ImageList = this.imageList;
			this.downButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("downButton.ImeMode")));
			this.downButton.Location = ((System.Drawing.Point)(resources.GetObject("downButton.Location")));
			this.downButton.Name = "downButton";
			this.downButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("downButton.RightToLeft")));
			this.downButton.Size = ((System.Drawing.Size)(resources.GetObject("downButton.Size")));
			this.downButton.TabIndex = ((int)(resources.GetObject("downButton.TabIndex")));
			this.downButton.Text = resources.GetString("downButton.Text");
			this.downButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("downButton.TextAlign")));
			this.downButton.Visible = ((bool)(resources.GetObject("downButton.Visible")));
			this.downButton.Click += new System.EventHandler(this.downButton_Click);
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
			// ConfigTableDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.label1);
			this.Controls.Add(this.downButton);
			this.Controls.Add(this.upButton);
			this.Controls.Add(this.uncheckAllButton);
			this.Controls.Add(this.checkAllButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.columnCheckedListBox);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ConfigTableDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.ConfigTableDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void checkAllButton_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < this.columnCheckedListBox.Items.Count; i++)
			{
				this.columnCheckedListBox.SetItemChecked(i, true);
			}
		}

		private void uncheckAllButton_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < this.columnCheckedListBox.Items.Count; i++)
			{
				this.columnCheckedListBox.SetItemChecked(i, false);
			}
		}

		private void ConfigTableDialog_Load(object sender, System.EventArgs e)
		{
			if (ColumnInfos != null)
			{
				foreach (ColumnInfo columnInfo in ColumnInfos)
				{
					columnCheckedListBox.Items.Add(columnInfo.Caption, columnInfo.IsChecked);
				}
			}
		}

		private void upButton_Click(object sender, System.EventArgs e)
		{
			int selectedIndex = columnCheckedListBox.SelectedIndex;
			if (selectedIndex > 0)
			{
				ColumnInfo selectedColumnInfo = this._columnInfos[selectedIndex];

				// move the displayed item up
				columnCheckedListBox.Items.RemoveAt(selectedIndex);
				columnCheckedListBox.Items.Insert(selectedIndex - 1, selectedColumnInfo.Caption);
				columnCheckedListBox.SetItemChecked(selectedIndex - 1, selectedColumnInfo.IsChecked);

				// move ColumnInfo object up
				this._columnInfos.RemoveAt(selectedIndex);
				this._columnInfos.Insert(selectedIndex - 1, selectedColumnInfo);

				// selected the moved item
				columnCheckedListBox.SelectedIndex = selectedIndex - 1;
			}
		}

		private void downButton_Click(object sender, System.EventArgs e)
		{
			int selectedIndex = columnCheckedListBox.SelectedIndex;
			if (selectedIndex < (_columnInfos.Count - 1))
			{
				ColumnInfo selectedColumnInfo = this._columnInfos[selectedIndex];

				// move the displayed item down
				columnCheckedListBox.Items.RemoveAt(selectedIndex);
				columnCheckedListBox.Items.Insert(selectedIndex + 1, selectedColumnInfo.Caption);
				columnCheckedListBox.SetItemChecked(selectedIndex + 1, selectedColumnInfo.IsChecked);

				// move ColumnInfo object down
				this._columnInfos.RemoveAt(selectedIndex);
				this._columnInfos.Insert(selectedIndex + 1, selectedColumnInfo);

				// selected the moved item
				columnCheckedListBox.SelectedIndex = selectedIndex + 1;
			}
		}

		private void columnCheckedListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.upButton.Enabled = true;
			this.downButton.Enabled = true;

			if (columnCheckedListBox.SelectedIndex == 0)
			{
				this.upButton.Enabled = false;
			}
			else if (columnCheckedListBox.SelectedIndex == (ColumnInfos.Count - 1))
			{
				this.downButton.Enabled = false;
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			bool hasChecked = false;

			for (int i = 0; i < columnCheckedListBox.Items.Count; i++)
			{
				_columnInfos[i].IsChecked = columnCheckedListBox.GetItemChecked(i);
				if (_columnInfos[i].IsChecked)
				{
					hasChecked = true;
				}
			}

			if (!hasChecked)
			{
				MessageBox.Show(MessageResourceManager.GetString("DataGrid.NoColumnSelected"));
				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}
	}
}

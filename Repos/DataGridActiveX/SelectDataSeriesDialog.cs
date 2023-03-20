using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.DataGridActiveX.ChartModel;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// SelectDataSeriesDialog 的摘要说明。
	/// </summary>
	public class SelectDataSeriesDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox dataSeriesListBox;

		private IDataGridControl _dataGridControl;
		private ChartDef _chartDef;
		private ColumnInfo _columnInfo;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectDataSeriesDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_dataGridControl = null;
			_chartDef = null;
			_columnInfo = null;
		}

		/// <summary>
		/// Gets or sets the DataGridControl instance that display the data series
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
		/// Gets or sets the ChartDef instance that provides charting definition
		/// </summary>
		public ChartDef ChartDefinition
		{
			get
			{
				return _chartDef;
			}
			set
			{
				_chartDef = value;
			}
		}

		/// <summary>
		/// Gets the selected series name, null if nothing is selected
		/// </summary>
		public string SelectedSeriesName
		{
			get
			{
				if (_columnInfo != null)
				{
					return _columnInfo.Name;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the selected series caption, null if nothing is selected
		/// </summary>
		public string SelectedSeriesCaption
		{
			get
			{
				if (_columnInfo != null)
				{
					return _columnInfo.Caption;
				}
				else
				{
					return null;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SelectDataSeriesDialog));
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.dataSeriesListBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
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
			// dataSeriesListBox
			// 
			this.dataSeriesListBox.AccessibleDescription = resources.GetString("dataSeriesListBox.AccessibleDescription");
			this.dataSeriesListBox.AccessibleName = resources.GetString("dataSeriesListBox.AccessibleName");
			this.dataSeriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("dataSeriesListBox.Anchor")));
			this.dataSeriesListBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("dataSeriesListBox.BackgroundImage")));
			this.dataSeriesListBox.ColumnWidth = ((int)(resources.GetObject("dataSeriesListBox.ColumnWidth")));
			this.dataSeriesListBox.DisplayMember = "Caption";
			this.dataSeriesListBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("dataSeriesListBox.Dock")));
			this.dataSeriesListBox.Enabled = ((bool)(resources.GetObject("dataSeriesListBox.Enabled")));
			this.dataSeriesListBox.Font = ((System.Drawing.Font)(resources.GetObject("dataSeriesListBox.Font")));
			this.dataSeriesListBox.HorizontalExtent = ((int)(resources.GetObject("dataSeriesListBox.HorizontalExtent")));
			this.dataSeriesListBox.HorizontalScrollbar = ((bool)(resources.GetObject("dataSeriesListBox.HorizontalScrollbar")));
			this.dataSeriesListBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("dataSeriesListBox.ImeMode")));
			this.dataSeriesListBox.IntegralHeight = ((bool)(resources.GetObject("dataSeriesListBox.IntegralHeight")));
			this.dataSeriesListBox.ItemHeight = ((int)(resources.GetObject("dataSeriesListBox.ItemHeight")));
			this.dataSeriesListBox.Location = ((System.Drawing.Point)(resources.GetObject("dataSeriesListBox.Location")));
			this.dataSeriesListBox.Name = "dataSeriesListBox";
			this.dataSeriesListBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("dataSeriesListBox.RightToLeft")));
			this.dataSeriesListBox.ScrollAlwaysVisible = ((bool)(resources.GetObject("dataSeriesListBox.ScrollAlwaysVisible")));
			this.dataSeriesListBox.Size = ((System.Drawing.Size)(resources.GetObject("dataSeriesListBox.Size")));
			this.dataSeriesListBox.TabIndex = ((int)(resources.GetObject("dataSeriesListBox.TabIndex")));
			this.dataSeriesListBox.Visible = ((bool)(resources.GetObject("dataSeriesListBox.Visible")));
			this.dataSeriesListBox.DoubleClick += new System.EventHandler(this.dataSeriesListBox_DoubleClick);
			this.dataSeriesListBox.SelectedIndexChanged += new System.EventHandler(this.dataSeriesListBox_SelectedIndexChanged);
			// 
			// SelectDataSeriesDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.dataSeriesListBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "SelectDataSeriesDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.SelectDataSeriesDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void LoadDataSeries()
		{
			if (this._chartDef != null && this._dataGridControl != null)
			{
				if (this._chartDef.Orientation == DataSeriesOrientation.ByColumn)
				{
					foreach (ColumnInfo columnInfo in this._dataGridControl.ColumnInfos)
					{
						if (columnInfo.IsChecked)
						{
							this.dataSeriesListBox.Items.Add(columnInfo);
						}
					}
				}
				else
				{
					// show the rows as data series
					ColumnInfo columnInfo;
					DataView dataView = DataGridControl.DataView;
					for (int row = 0; row < dataView.Table.Rows.Count; row++)
					{
						columnInfo = new ColumnInfo();
						columnInfo.Caption = MessageResourceManager.GetString("GraphWizard.Row") + row;
						columnInfo.Name = row + "";
						if (!_chartDef.UseSelectedRows)
						{
							this.dataSeriesListBox.Items.Add(columnInfo);
						}
						else if (DataGridControl.TheDataGrid.IsSelected(row))
						{
							this.dataSeriesListBox.Items.Add(columnInfo);
						}
					}
				}
			}
		}

		private void SelectDataSeriesDialog_Load(object sender, System.EventArgs e)
		{
			// display the data series in the list box
			try
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				LoadDataSeries();
			}
			finally
			{
				// Restore the cursor
				Cursor.Current = this.Cursor;
			}
		}

		private void dataSeriesListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.dataSeriesListBox.SelectedIndex >= 0)
			{
				 _columnInfo = (ColumnInfo) this.dataSeriesListBox.SelectedItem;
			}
			else
			{
				_columnInfo = null;
			}
		}

		private void dataSeriesListBox_DoubleClick(object sender, System.EventArgs e)
		{
			if (this.dataSeriesListBox.SelectedIndex >= 0)
			{
				_columnInfo = (ColumnInfo) this.dataSeriesListBox.SelectedItem;
			}

			this.DialogResult = DialogResult.OK;
		}

	}
}

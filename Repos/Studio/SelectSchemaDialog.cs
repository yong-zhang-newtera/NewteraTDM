using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// SelectSchemaDialog 的摘要说明。
	/// </summary>
	public class SelectSchemaDialog : System.Windows.Forms.Form
	{
		private SchemaInfo[] _schemas = null;
		private SchemaInfo _selectedSchema = null;
		private MetaDataServiceStub _metaDataService;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete = false;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ListView listView;
		private System.ComponentModel.IContainer components;

		public SelectSchemaDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_metaDataService = new MetaDataServiceStub();
			_workInProgressDialog = new WorkInProgressDialog();
		}

		~SelectSchemaDialog()
		{
			_workInProgressDialog.Dispose();
		}

		public SchemaInfo SelectedSchema
		{
			get
			{
				return _selectedSchema;
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

		/// <summary>
		/// Show the working dialog
		/// </summary>
		/// <remarks>It has to deal with multi-threading issue</remarks>
		private void ShowWorkingDialog()
		{
			lock (_workInProgressDialog)
			{
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

		private delegate void ShowSchemaInfosDelegate(SchemaInfo[] schemas);

		/// <summary>
		/// Display the schema info array in the list view
		/// </summary>
		/// <param name="schemas">The array of schema info</param>
		private void ShowSchemaInfos(SchemaInfo[] schemas)
		{
			if (this.InvokeRequired == false)
			{
				// It is the UI thread, go ahead to show the data

				_schemas = schemas;
			
				this.listView.BeginUpdate();
				ListViewItem item;
				int index = 0;
				foreach (SchemaInfo schemaInfo in _schemas)
				{
					item = this.listView.Items.Add(schemaInfo.Name, 0);
					item.SubItems.Add(schemaInfo.Version);
					if (index == 0)
					{
						// select the first item as default
						item.Selected = true;
					}

					index++;
				}

				this.listView.EndUpdate();
			}
			else
			{
				// It is a worker thread, pass the control to the UI thread
				ShowSchemaInfosDelegate showSchemaInfos = new ShowSchemaInfosDelegate(ShowSchemaInfos);
				this.BeginInvoke(showSchemaInfos, new object[] {schemas});
			}
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SelectSchemaDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.listView = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
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
			// listView
			// 
			this.listView.AccessibleDescription = resources.GetString("listView.AccessibleDescription");
			this.listView.AccessibleName = resources.GetString("listView.AccessibleName");
			this.listView.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("listView.Alignment")));
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("listView.Anchor")));
			this.listView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listView.BackgroundImage")));
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.columnHeader3,
																					   this.columnHeader1});
			this.listView.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("listView.Dock")));
			this.listView.Enabled = ((bool)(resources.GetObject("listView.Enabled")));
			this.listView.Font = ((System.Drawing.Font)(resources.GetObject("listView.Font")));
			this.listView.FullRowSelect = true;
			this.listView.HideSelection = false;
			this.listView.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("listView.ImeMode")));
			this.listView.LabelWrap = ((bool)(resources.GetObject("listView.LabelWrap")));
			this.listView.Location = ((System.Drawing.Point)(resources.GetObject("listView.Location")));
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("listView.RightToLeft")));
			this.listView.Size = ((System.Drawing.Size)(resources.GetObject("listView.Size")));
			this.listView.SmallImageList = this.imageList1;
			this.listView.TabIndex = ((int)(resources.GetObject("listView.TabIndex")));
			this.listView.Text = resources.GetString("listView.Text");
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.Visible = ((bool)(resources.GetObject("listView.Visible")));
			this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = resources.GetString("columnHeader3.Text");
			this.columnHeader3.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader3.TextAlign")));
			this.columnHeader3.Width = ((int)(resources.GetObject("columnHeader3.Width")));
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = resources.GetString("columnHeader1.Text");
			this.columnHeader1.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader1.TextAlign")));
			this.columnHeader1.Width = ((int)(resources.GetObject("columnHeader1.Width")));
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = ((System.Drawing.Size)(resources.GetObject("imageList1.ImageSize")));
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
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
			// SelectSchemaDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.label1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "SelectSchemaDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.SelectSchemaDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void SelectSchemaDialog_Load(object sender, System.EventArgs e)
		{
            try
            {
                // invoke the web service synchronously
                ShowSchemaInfos(_metaDataService.GetSchemaInfos());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
		}

		private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_schemas != null)
			{
				ListView.SelectedIndexCollection selectedIndices = this.listView.SelectedIndices;
				if (selectedIndices.Count == 1)
				{
					_selectedSchema = _schemas[selectedIndices[0]];
				}
			}	
		}
	}
}

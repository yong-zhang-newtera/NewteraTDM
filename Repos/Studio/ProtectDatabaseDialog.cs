using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// ProtectDatabaseDialog 的摘要说明。
	/// </summary>
	public class ProtectDatabaseDialog : System.Windows.Forms.Form
	{
		private SchemaInfo[] _schemas = null;
		private MetaDataServiceStub _metaDataService;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete = false;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView schemaListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button showRolesButton;
		private System.Windows.Forms.RadioButton notProtectedRadioButton;
		private System.Windows.Forms.RadioButton protectedRadioButton;
		private System.ComponentModel.IContainer components;

		public ProtectDatabaseDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			_metaDataService = new MetaDataServiceStub();
			_workInProgressDialog = new WorkInProgressDialog();
		}

		~ProtectDatabaseDialog()
		{
			_workInProgressDialog.Dispose();
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

		/// <summary>
		/// Display the schema info array in the list view
		/// </summary>
		/// <param name="schemas">The array of schema info</param>
		private void ShowSchemaInfos(SchemaInfo[] schemas)
		{
			_schemas = schemas;
			
			this.schemaListView.BeginUpdate();
			ListViewItem item;
			int index = 0;
			foreach (SchemaInfo schemaInfo in _schemas)
			{
				item = this.schemaListView.Items.Add(schemaInfo.Name, 0);
				item.SubItems.Add(schemaInfo.Version);
				if (index == 0)
				{
					// select the first item as default
					item.Selected = true;
				}

				index++;
			}

			this.schemaListView.EndUpdate();
		}

		/// <summary>
		/// Set the protect mode according to the selection
		/// </summary>
		/// <returns>true if setting mode successfully, false otherwise.</returns>
		private bool SetProtectMode()
		{
			bool status = true;

			if (schemaListView.SelectedItems.Count == 1)
			{
				int selectedIndex = schemaListView.SelectedIndices[0];

                string connectionString = ConnectionStringBuilder.Instance.Create(_schemas[selectedIndex].Name, _schemas[selectedIndex].Version, _schemas[selectedIndex].ModifiedTime);
				if (this.protectedRadioButton.Checked)
				{
					if (this.textBox1.Text == null ||
						this.textBox1.Text.Length == 0)
					{
						MessageBox.Show(MessageResourceManager.GetString("DesignStudio.MissingRole"));
						status = false;
					}
					else
					{
						// protected mode, set the selected role
						_metaDataService.SetDBARole(connectionString, this.textBox1.Text);
					}
				}
				else
				{
					// non-protected mode, set role to null
					_metaDataService.SetDBARole(connectionString, null);
				}
			}
			
			return status;
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProtectDatabaseDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.schemaListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.applyButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.notProtectedRadioButton = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.showRolesButton = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.protectedRadioButton = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
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
			// schemaListView
			// 
			this.schemaListView.AccessibleDescription = resources.GetString("schemaListView.AccessibleDescription");
			this.schemaListView.AccessibleName = resources.GetString("schemaListView.AccessibleName");
			this.schemaListView.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("schemaListView.Alignment")));
			this.schemaListView.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("schemaListView.Anchor")));
			this.schemaListView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("schemaListView.BackgroundImage")));
			this.schemaListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.columnHeader1,
																							 this.columnHeader2});
			this.schemaListView.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("schemaListView.Dock")));
			this.schemaListView.Enabled = ((bool)(resources.GetObject("schemaListView.Enabled")));
			this.schemaListView.Font = ((System.Drawing.Font)(resources.GetObject("schemaListView.Font")));
			this.schemaListView.FullRowSelect = true;
			this.schemaListView.HideSelection = false;
			this.schemaListView.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("schemaListView.ImeMode")));
			this.schemaListView.LabelWrap = ((bool)(resources.GetObject("schemaListView.LabelWrap")));
			this.schemaListView.Location = ((System.Drawing.Point)(resources.GetObject("schemaListView.Location")));
			this.schemaListView.MultiSelect = false;
			this.schemaListView.Name = "schemaListView";
			this.schemaListView.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("schemaListView.RightToLeft")));
			this.schemaListView.Size = ((System.Drawing.Size)(resources.GetObject("schemaListView.Size")));
			this.schemaListView.SmallImageList = this.imageList1;
			this.schemaListView.TabIndex = ((int)(resources.GetObject("schemaListView.TabIndex")));
			this.schemaListView.Text = resources.GetString("schemaListView.Text");
			this.schemaListView.View = System.Windows.Forms.View.Details;
			this.schemaListView.Visible = ((bool)(resources.GetObject("schemaListView.Visible")));
			this.schemaListView.SelectedIndexChanged += new System.EventHandler(this.schemaListView_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = resources.GetString("columnHeader1.Text");
			this.columnHeader1.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader1.TextAlign")));
			this.columnHeader1.Width = ((int)(resources.GetObject("columnHeader1.Width")));
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = resources.GetString("columnHeader2.Text");
			this.columnHeader2.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader2.TextAlign")));
			this.columnHeader2.Width = ((int)(resources.GetObject("columnHeader2.Width")));
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = ((System.Drawing.Size)(resources.GetObject("imageList1.ImageSize")));
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// applyButton
			// 
			this.applyButton.AccessibleDescription = resources.GetString("applyButton.AccessibleDescription");
			this.applyButton.AccessibleName = resources.GetString("applyButton.AccessibleName");
			this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("applyButton.Anchor")));
			this.applyButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("applyButton.BackgroundImage")));
			this.applyButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("applyButton.Dock")));
			this.applyButton.Enabled = ((bool)(resources.GetObject("applyButton.Enabled")));
			this.applyButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("applyButton.FlatStyle")));
			this.applyButton.Font = ((System.Drawing.Font)(resources.GetObject("applyButton.Font")));
			this.applyButton.Image = ((System.Drawing.Image)(resources.GetObject("applyButton.Image")));
			this.applyButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("applyButton.ImageAlign")));
			this.applyButton.ImageIndex = ((int)(resources.GetObject("applyButton.ImageIndex")));
			this.applyButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("applyButton.ImeMode")));
			this.applyButton.Location = ((System.Drawing.Point)(resources.GetObject("applyButton.Location")));
			this.applyButton.Name = "applyButton";
			this.applyButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("applyButton.RightToLeft")));
			this.applyButton.Size = ((System.Drawing.Size)(resources.GetObject("applyButton.Size")));
			this.applyButton.TabIndex = ((int)(resources.GetObject("applyButton.TabIndex")));
			this.applyButton.Text = resources.GetString("applyButton.Text");
			this.applyButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("applyButton.TextAlign")));
			this.applyButton.Visible = ((bool)(resources.GetObject("applyButton.Visible")));
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
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
			// notProtectedRadioButton
			// 
			this.notProtectedRadioButton.AccessibleDescription = resources.GetString("notProtectedRadioButton.AccessibleDescription");
			this.notProtectedRadioButton.AccessibleName = resources.GetString("notProtectedRadioButton.AccessibleName");
			this.notProtectedRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("notProtectedRadioButton.Anchor")));
			this.notProtectedRadioButton.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("notProtectedRadioButton.Appearance")));
			this.notProtectedRadioButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("notProtectedRadioButton.BackgroundImage")));
			this.notProtectedRadioButton.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("notProtectedRadioButton.CheckAlign")));
			this.notProtectedRadioButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("notProtectedRadioButton.Dock")));
			this.notProtectedRadioButton.Enabled = ((bool)(resources.GetObject("notProtectedRadioButton.Enabled")));
			this.notProtectedRadioButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("notProtectedRadioButton.FlatStyle")));
			this.notProtectedRadioButton.Font = ((System.Drawing.Font)(resources.GetObject("notProtectedRadioButton.Font")));
			this.notProtectedRadioButton.Image = ((System.Drawing.Image)(resources.GetObject("notProtectedRadioButton.Image")));
			this.notProtectedRadioButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("notProtectedRadioButton.ImageAlign")));
			this.notProtectedRadioButton.ImageIndex = ((int)(resources.GetObject("notProtectedRadioButton.ImageIndex")));
			this.notProtectedRadioButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("notProtectedRadioButton.ImeMode")));
			this.notProtectedRadioButton.Location = ((System.Drawing.Point)(resources.GetObject("notProtectedRadioButton.Location")));
			this.notProtectedRadioButton.Name = "notProtectedRadioButton";
			this.notProtectedRadioButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("notProtectedRadioButton.RightToLeft")));
			this.notProtectedRadioButton.Size = ((System.Drawing.Size)(resources.GetObject("notProtectedRadioButton.Size")));
			this.notProtectedRadioButton.TabIndex = ((int)(resources.GetObject("notProtectedRadioButton.TabIndex")));
			this.notProtectedRadioButton.Text = resources.GetString("notProtectedRadioButton.Text");
			this.notProtectedRadioButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("notProtectedRadioButton.TextAlign")));
			this.notProtectedRadioButton.Visible = ((bool)(resources.GetObject("notProtectedRadioButton.Visible")));
			// 
			// groupBox1
			// 
			this.groupBox1.AccessibleDescription = resources.GetString("groupBox1.AccessibleDescription");
			this.groupBox1.AccessibleName = resources.GetString("groupBox1.AccessibleName");
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox1.Anchor")));
			this.groupBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox1.BackgroundImage")));
			this.groupBox1.Controls.Add(this.showRolesButton);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox1.Dock")));
			this.groupBox1.Enabled = ((bool)(resources.GetObject("groupBox1.Enabled")));
			this.groupBox1.Font = ((System.Drawing.Font)(resources.GetObject("groupBox1.Font")));
			this.groupBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox1.ImeMode")));
			this.groupBox1.Location = ((System.Drawing.Point)(resources.GetObject("groupBox1.Location")));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox1.RightToLeft")));
			this.groupBox1.Size = ((System.Drawing.Size)(resources.GetObject("groupBox1.Size")));
			this.groupBox1.TabIndex = ((int)(resources.GetObject("groupBox1.TabIndex")));
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = resources.GetString("groupBox1.Text");
			this.groupBox1.Visible = ((bool)(resources.GetObject("groupBox1.Visible")));
			// 
			// showRolesButton
			// 
			this.showRolesButton.AccessibleDescription = resources.GetString("showRolesButton.AccessibleDescription");
			this.showRolesButton.AccessibleName = resources.GetString("showRolesButton.AccessibleName");
			this.showRolesButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("showRolesButton.Anchor")));
			this.showRolesButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("showRolesButton.BackgroundImage")));
			this.showRolesButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("showRolesButton.Dock")));
			this.showRolesButton.Enabled = ((bool)(resources.GetObject("showRolesButton.Enabled")));
			this.showRolesButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("showRolesButton.FlatStyle")));
			this.showRolesButton.Font = ((System.Drawing.Font)(resources.GetObject("showRolesButton.Font")));
			this.showRolesButton.Image = ((System.Drawing.Image)(resources.GetObject("showRolesButton.Image")));
			this.showRolesButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("showRolesButton.ImageAlign")));
			this.showRolesButton.ImageIndex = ((int)(resources.GetObject("showRolesButton.ImageIndex")));
			this.showRolesButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("showRolesButton.ImeMode")));
			this.showRolesButton.Location = ((System.Drawing.Point)(resources.GetObject("showRolesButton.Location")));
			this.showRolesButton.Name = "showRolesButton";
			this.showRolesButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("showRolesButton.RightToLeft")));
			this.showRolesButton.Size = ((System.Drawing.Size)(resources.GetObject("showRolesButton.Size")));
			this.showRolesButton.TabIndex = ((int)(resources.GetObject("showRolesButton.TabIndex")));
			this.showRolesButton.Text = resources.GetString("showRolesButton.Text");
			this.showRolesButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("showRolesButton.TextAlign")));
			this.showRolesButton.Visible = ((bool)(resources.GetObject("showRolesButton.Visible")));
			this.showRolesButton.Click += new System.EventHandler(this.showRolesButton_Click);
			// 
			// textBox1
			// 
			this.textBox1.AccessibleDescription = resources.GetString("textBox1.AccessibleDescription");
			this.textBox1.AccessibleName = resources.GetString("textBox1.AccessibleName");
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("textBox1.Anchor")));
			this.textBox1.AutoSize = ((bool)(resources.GetObject("textBox1.AutoSize")));
			this.textBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textBox1.BackgroundImage")));
			this.textBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("textBox1.Dock")));
			this.textBox1.Enabled = ((bool)(resources.GetObject("textBox1.Enabled")));
			this.textBox1.Font = ((System.Drawing.Font)(resources.GetObject("textBox1.Font")));
			this.textBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("textBox1.ImeMode")));
			this.textBox1.Location = ((System.Drawing.Point)(resources.GetObject("textBox1.Location")));
			this.textBox1.MaxLength = ((int)(resources.GetObject("textBox1.MaxLength")));
			this.textBox1.Multiline = ((bool)(resources.GetObject("textBox1.Multiline")));
			this.textBox1.Name = "textBox1";
			this.textBox1.PasswordChar = ((char)(resources.GetObject("textBox1.PasswordChar")));
			this.textBox1.ReadOnly = true;
			this.textBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("textBox1.RightToLeft")));
			this.textBox1.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("textBox1.ScrollBars")));
			this.textBox1.Size = ((System.Drawing.Size)(resources.GetObject("textBox1.Size")));
			this.textBox1.TabIndex = ((int)(resources.GetObject("textBox1.TabIndex")));
			this.textBox1.Text = resources.GetString("textBox1.Text");
			this.textBox1.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("textBox1.TextAlign")));
			this.textBox1.Visible = ((bool)(resources.GetObject("textBox1.Visible")));
			this.textBox1.WordWrap = ((bool)(resources.GetObject("textBox1.WordWrap")));
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
			// protectedRadioButton
			// 
			this.protectedRadioButton.AccessibleDescription = resources.GetString("protectedRadioButton.AccessibleDescription");
			this.protectedRadioButton.AccessibleName = resources.GetString("protectedRadioButton.AccessibleName");
			this.protectedRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("protectedRadioButton.Anchor")));
			this.protectedRadioButton.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("protectedRadioButton.Appearance")));
			this.protectedRadioButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("protectedRadioButton.BackgroundImage")));
			this.protectedRadioButton.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("protectedRadioButton.CheckAlign")));
			this.protectedRadioButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("protectedRadioButton.Dock")));
			this.protectedRadioButton.Enabled = ((bool)(resources.GetObject("protectedRadioButton.Enabled")));
			this.protectedRadioButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("protectedRadioButton.FlatStyle")));
			this.protectedRadioButton.Font = ((System.Drawing.Font)(resources.GetObject("protectedRadioButton.Font")));
			this.protectedRadioButton.Image = ((System.Drawing.Image)(resources.GetObject("protectedRadioButton.Image")));
			this.protectedRadioButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("protectedRadioButton.ImageAlign")));
			this.protectedRadioButton.ImageIndex = ((int)(resources.GetObject("protectedRadioButton.ImageIndex")));
			this.protectedRadioButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("protectedRadioButton.ImeMode")));
			this.protectedRadioButton.Location = ((System.Drawing.Point)(resources.GetObject("protectedRadioButton.Location")));
			this.protectedRadioButton.Name = "protectedRadioButton";
			this.protectedRadioButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("protectedRadioButton.RightToLeft")));
			this.protectedRadioButton.Size = ((System.Drawing.Size)(resources.GetObject("protectedRadioButton.Size")));
			this.protectedRadioButton.TabIndex = ((int)(resources.GetObject("protectedRadioButton.TabIndex")));
			this.protectedRadioButton.Text = resources.GetString("protectedRadioButton.Text");
			this.protectedRadioButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("protectedRadioButton.TextAlign")));
			this.protectedRadioButton.Visible = ((bool)(resources.GetObject("protectedRadioButton.Visible")));
			// 
			// ProtectDatabaseDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.protectedRadioButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.notProtectedRadioButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.schemaListView);
			this.Controls.Add(this.label1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ProtectDatabaseDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.ProtectDatabaseDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (!SetProtectMode())
			{
				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}

		private void applyButton_Click(object sender, System.EventArgs e)
		{
			SetProtectMode();
		}

		private void showRolesButton_Click(object sender, System.EventArgs e)
		{
			SelectRoleDialog dialog = new SelectRoleDialog();

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				this.textBox1.Text = ConvertArrayToString(dialog.SelectedRoles);
			}
		}

		/// <summary>
		/// Convert a string array into a semi-colon separated string.
		/// </summary>
		/// <param name="values">An string array</param>
		/// <returns>A semi-colon separated string</returns>
		private string ConvertArrayToString(string[] values)
		{
			string converted = null;
			foreach (string val in values)
			{
				if (converted == null)
				{
					converted = val;
				}
				else
				{
					converted += ";";
					converted += val;
				}
			}

			return converted;
		}

		private void ProtectDatabaseDialog_Load(object sender, System.EventArgs e)
		{
			_isRequestComplete = false;

			// invoke the web service asynchronously
			SchemaInfo[] schemaInfos = _metaDataService.GetSchemaInfos();

            ShowSchemaInfos(schemaInfos);
		}

		private void schemaListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (schemaListView.SelectedItems.Count == 1)
			{
				int selectedIndex = schemaListView.SelectedIndices[0];

				// invoke the web service synchronously
                string roles = _metaDataService.GetDBARole(ConnectionStringBuilder.Instance.Create(_schemas[selectedIndex].Name, _schemas[selectedIndex].Version, _schemas[selectedIndex].ModifiedTime));

				if (roles != null)
				{
					// protected mode
					this.protectedRadioButton.Checked = true;
					this.textBox1.Text = roles;
				}
				else
				{
					// non-protected mode
					this.notProtectedRadioButton.Checked = true;
					this.textBox1.Text = null;
				}
			}
		}
	}
}

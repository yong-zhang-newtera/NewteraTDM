using System;
using System.Threading;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.MLActivities.Core;
using Newtera.MLStudio.ViewModel;
using Newtera.MLStudio.WebApi;

namespace Newtera.MLStudio.Views
{
	/// <summary>
	/// Summary description for OpenSchemaDialog.
	/// </summary>
	public class OpenSchemaDialog : System.Windows.Forms.Form
	{
		private SchemaInfo[] _schemas = null;
		private SchemaInfo _selectedSchema = null;
		private MetaDataServiceStub _metaDataService;
        private UserInfoServiceStub _userInfoService;
        private bool _isAuthenticated = false;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox idTextBox;
		private System.Windows.Forms.Label label3;
		private Newtera.MLStudio.Views.EnterTextBox pwdTextBox;
		private System.ComponentModel.IContainer components;

		public OpenSchemaDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_metaDataService = new MetaDataServiceStub();
            this._metaDataService.BaseUri = MLNameSpace.GetServerUri();
            _userInfoService = new UserInfoServiceStub();
            this._userInfoService.BaseUri = MLNameSpace.GetServerUri();
        }

		/// <summary>
		/// Gets the selected schema
		/// </summary>
		public SchemaInfo SelectedSchema
		{
			get
			{
				return _selectedSchema;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(OpenSchemaDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this.idTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.pwdTextBox = new Newtera.MLStudio.Views.EnterTextBox();
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
			// listView1
			// 
			this.listView1.AccessibleDescription = resources.GetString("listView1.AccessibleDescription");
			this.listView1.AccessibleName = resources.GetString("listView1.AccessibleName");
			this.listView1.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("listView1.Alignment")));
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("listView1.Anchor")));
			this.listView1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listView1.BackgroundImage")));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader3,
																						this.columnHeader1});
			this.listView1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("listView1.Dock")));
			this.listView1.Enabled = ((bool)(resources.GetObject("listView1.Enabled")));
			this.listView1.Font = ((System.Drawing.Font)(resources.GetObject("listView1.Font")));
			this.listView1.FullRowSelect = true;
			this.listView1.HideSelection = false;
			this.listView1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("listView1.ImeMode")));
			this.listView1.LabelWrap = ((bool)(resources.GetObject("listView1.LabelWrap")));
			this.listView1.Location = ((System.Drawing.Point)(resources.GetObject("listView1.Location")));
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("listView1.RightToLeft")));
			this.listView1.Size = ((System.Drawing.Size)(resources.GetObject("listView1.Size")));
			this.listView1.SmallImageList = this.imageList1;
			this.listView1.TabIndex = ((int)(resources.GetObject("listView1.TabIndex")));
			this.listView1.Text = resources.GetString("listView1.Text");
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.Visible = ((bool)(resources.GetObject("listView1.Visible")));
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
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
			// idTextBox
			// 
			this.idTextBox.AccessibleDescription = resources.GetString("idTextBox.AccessibleDescription");
			this.idTextBox.AccessibleName = resources.GetString("idTextBox.AccessibleName");
			this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("idTextBox.Anchor")));
			this.idTextBox.AutoSize = ((bool)(resources.GetObject("idTextBox.AutoSize")));
			this.idTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("idTextBox.BackgroundImage")));
			this.idTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("idTextBox.Dock")));
			this.idTextBox.Enabled = ((bool)(resources.GetObject("idTextBox.Enabled")));
			this.idTextBox.Font = ((System.Drawing.Font)(resources.GetObject("idTextBox.Font")));
			this.idTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("idTextBox.ImeMode")));
			this.idTextBox.Location = ((System.Drawing.Point)(resources.GetObject("idTextBox.Location")));
			this.idTextBox.MaxLength = ((int)(resources.GetObject("idTextBox.MaxLength")));
			this.idTextBox.Multiline = ((bool)(resources.GetObject("idTextBox.Multiline")));
			this.idTextBox.Name = "idTextBox";
			this.idTextBox.PasswordChar = ((char)(resources.GetObject("idTextBox.PasswordChar")));
			this.idTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("idTextBox.RightToLeft")));
			this.idTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("idTextBox.ScrollBars")));
			this.idTextBox.Size = ((System.Drawing.Size)(resources.GetObject("idTextBox.Size")));
			this.idTextBox.TabIndex = ((int)(resources.GetObject("idTextBox.TabIndex")));
			this.idTextBox.Text = resources.GetString("idTextBox.Text");
			this.idTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("idTextBox.TextAlign")));
			this.idTextBox.Visible = ((bool)(resources.GetObject("idTextBox.Visible")));
			this.idTextBox.WordWrap = ((bool)(resources.GetObject("idTextBox.WordWrap")));
			// 
			// label3
			// 
			this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
			this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
			this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
			this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
			this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
			this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
			this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
			this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
			this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
			this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
			this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
			this.label3.Name = "label3";
			this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
			this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
			this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
			this.label3.Text = resources.GetString("label3.Text");
			this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
			this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
			// 
			// pwdTextBox
			// 
			this.pwdTextBox.AccessibleDescription = resources.GetString("pwdTextBox.AccessibleDescription");
			this.pwdTextBox.AccessibleName = resources.GetString("pwdTextBox.AccessibleName");
			this.pwdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pwdTextBox.Anchor")));
			this.pwdTextBox.AutoSize = ((bool)(resources.GetObject("pwdTextBox.AutoSize")));
			this.pwdTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pwdTextBox.BackgroundImage")));
			this.pwdTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pwdTextBox.Dock")));
			this.pwdTextBox.Enabled = ((bool)(resources.GetObject("pwdTextBox.Enabled")));
			this.pwdTextBox.Font = ((System.Drawing.Font)(resources.GetObject("pwdTextBox.Font")));
			this.pwdTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pwdTextBox.ImeMode")));
			this.pwdTextBox.Location = ((System.Drawing.Point)(resources.GetObject("pwdTextBox.Location")));
			this.pwdTextBox.MaxLength = ((int)(resources.GetObject("pwdTextBox.MaxLength")));
			this.pwdTextBox.Multiline = ((bool)(resources.GetObject("pwdTextBox.Multiline")));
			this.pwdTextBox.Name = "pwdTextBox";
			this.pwdTextBox.PasswordChar = ((char)(resources.GetObject("pwdTextBox.PasswordChar")));
			this.pwdTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pwdTextBox.RightToLeft")));
			this.pwdTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("pwdTextBox.ScrollBars")));
			this.pwdTextBox.Size = ((System.Drawing.Size)(resources.GetObject("pwdTextBox.Size")));
			this.pwdTextBox.TabIndex = ((int)(resources.GetObject("pwdTextBox.TabIndex")));
			this.pwdTextBox.Text = resources.GetString("pwdTextBox.Text");
			this.pwdTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("pwdTextBox.TextAlign")));
			this.pwdTextBox.Visible = ((bool)(resources.GetObject("pwdTextBox.Visible")));
			this.pwdTextBox.WordWrap = ((bool)(resources.GetObject("pwdTextBox.WordWrap")));
			this.pwdTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pwdTextBox_KeyDown);
			

			// 
			// OpenSchemaDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.pwdTextBox);
			this.Controls.Add(this.idTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.label1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "OpenSchemaDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.OpenSchemaDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Display the schema info array in the list view
		/// </summary>
		/// <param name="schemas">The array of schema info</param>
		private void ShowSchemaInfos(SchemaInfo[] schemas)
		{
			_schemas = schemas;
			
			this.listView1.BeginUpdate();
			ListViewItem item;
			int index = 0;
			foreach (SchemaInfo schemaInfo in _schemas)
			{
				item = this.listView1.Items.Add(schemaInfo.Name, 0);
				item.SubItems.Add(schemaInfo.Version);
				if (index == 0)
				{
					// select the first item as default
					item.Selected = true;
				}

				index++;
			}

			this.listView1.EndUpdate();
		}

		/// <summary>
		/// Connect to server
		/// </summary>
		private void ConnectServer()
		{
			string userName = this.idTextBox.Text;
			string password = this.pwdTextBox.Text;

			_isAuthenticated = _userInfoService.Authenticate(userName, password);
			if (_isAuthenticated)
			{

				this.DialogResult = DialogResult.OK; // close the dialog
			}
			else
			{
				MessageBox.Show("Invalid user id or password", "Error Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
					
				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}

		private void OpenSchemaDialog_Load(object sender, System.EventArgs e)
		{
            try
            {
                // invoke the web service asynchronously
                SchemaInfo[] schemaInfos = _metaDataService.GetSchemaInfos();

                ShowSchemaInfos(schemaInfos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The model serving server isn't available.\nMake sure the server is running and the uri of the server is correct.\n Error:" + ex.Message);
                this.okButton.Enabled = false;
            }
        }

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_schemas != null)
			{
				ListView.SelectedIndexCollection selectedIndices = this.listView1.SelectedIndices;
				if (selectedIndices.Count == 1)
				{
					_selectedSchema = _schemas[selectedIndices[0]];
				}
			}		
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			ConnectServer();
		}

		private void pwdTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) 
			{
				e.Handled = true;

				ConnectServer();
			}		
		}
	}
}

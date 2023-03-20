using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Newtera.Common.Core;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for SaveToDatabaseDialog.
	/// </summary>
	public class SaveToDatabaseDialog : System.Windows.Forms.Form
	{
		private SchemaInfo[] _schemas = null;
		private SchemaInfo _selectedSchema = null;
		private MetaDataServiceStub _service;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
        private bool _isExistingSchema = false;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.TextBox versionTextBox;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.ComponentModel.IContainer components;

		public SaveToDatabaseDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_service = new MetaDataServiceStub();
			_workInProgressDialog = new WorkInProgressDialog();
		}

		~SaveToDatabaseDialog()
		{
			_workInProgressDialog.Dispose();
		}

		public SchemaInfo SelectedSchema
		{
			get
			{
				return _selectedSchema;
			}
			set
			{
				_selectedSchema = value;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the selected schema is an existing schema
        /// </summary>
        public bool IsExistingSchema
        {
            get
            {
                return _isExistingSchema;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveToDatabaseDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.versionTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
            // 
            // versionTextBox
            // 
            resources.ApplyResources(this.versionTextBox, "versionTextBox");
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.versionTextBox_Validating);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // SaveToDatabaseDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.versionTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SaveToDatabaseDialog";
            this.Load += new System.EventHandler(this.OpenSchemaDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		/// <summary>
		/// Gets the information indicating whether a given schema
		/// name and version have already existed
		/// </summary>
		/// <param name="schemaInfo">The given schema</param>
		/// <returns>true if it exists, false otherwise</returns>
		private bool IsSchemaExist(SchemaInfo schemaInfo)
		{
			bool status = false;

			if (_schemas != null)
			{
				for (int i = 0; i < _schemas.Length; i++)
				{
					if (_schemas[i].Name == schemaInfo.Name &&
						_schemas[i].Version == schemaInfo.Version)
					{
                        // get the another info of the selected schema
                        schemaInfo.ModifiedTime = _schemas[i].ModifiedTime;
                        schemaInfo.ID = _schemas[i].ID;
                        schemaInfo.Description = _schemas[i].Description;

						status = true;
						break;
					}
				}
			}

			return status;
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
			
				this.listView1.BeginUpdate();
				ListViewItem item;
				foreach (SchemaInfo schemaInfo in _schemas)
				{
					item = this.listView1.Items.Add(schemaInfo.Name, 0);
					item.SubItems.Add(schemaInfo.Version);
				}

				this.listView1.EndUpdate();
			}
			else
			{
				// It is a worker thread, pass the control to the UI thread
				ShowSchemaInfosDelegate showSchemaInfos = new ShowSchemaInfosDelegate(ShowSchemaInfos);
				this.BeginInvoke(showSchemaInfos, new object[] {schemas});
			}
		}

		private void OpenSchemaDialog_Load(object sender, System.EventArgs e)
		{
			// display the current schema name and version
			if (_selectedSchema != null)
			{
				this.nameTextBox.Text = _selectedSchema.Name;
				this.versionTextBox.Text = _selectedSchema.Version;
			}

            try
            {
                // display a text in the status bar
                ((DesignStudio)this.Owner).ShowWorkingStatus(MessageResourceManager.GetString("DesignStudio.GettingSchemas"));

                // invoke the web service synchronously
                ShowSchemaInfos(_service.GetSchemaInfos());

            }
            finally
            {
                // set the status back to ready message
                ((DesignStudio)this.Owner).ShowReadyStatus();
            }
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_schemas != null)
			{
				ListView.SelectedIndexCollection selectedIndices = this.listView1.SelectedIndices;
				if (selectedIndices.Count == 1)
				{
					SchemaInfo schemaInfo = _schemas[selectedIndices[0]];
					this.nameTextBox.Text = schemaInfo.Name;
					this.versionTextBox.Text = schemaInfo.Version;
				}
			}		
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// validate the text in nameTextBox and versionTextBox
			this.nameTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			this.versionTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			if (_selectedSchema == null)
			{
				_selectedSchema = new SchemaInfo();
			}

			_selectedSchema.Name = this.nameTextBox.Text;
			_selectedSchema.Version = this.versionTextBox.Text;

			// confirm that user wan to overrid an existing schema from a file
			if (IsSchemaExist(_selectedSchema))
			{
                _isExistingSchema = true;

                if (MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.OverwriteSchema"),
                        "Confirm Dialog", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.No)
                {
                    _selectedSchema = null; // clear the selected schema
                    this.DialogResult = DialogResult.Cancel; // close the dialog
                }
			}
		}

		private void nameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null
			if (this.nameTextBox.Text.Length == 0)
			{
				e.Cancel = true;

				this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.EnterName"));
			}
			else if (!IsValidName(this.nameTextBox.Text))
			{
				e.Cancel = true;

				this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidSchemaName"));
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}
		}

		private void versionTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the version cannot be null
			if (this.versionTextBox.Text.Length == 0)
			{
				e.Cancel = true;

				this.errorProvider.SetError(this.versionTextBox, MessageResourceManager.GetString("SchemaEditor.EnterVersion"));
			}		
		}

		private bool IsValidName(string name)
		{
			Regex regex = new Regex(@"^[a-zA-Z\u0080-\uFFFE]+[0-9]*[a-zA-Z\u0080-\uFFFE]*[0-9]*$");

			bool status = regex.IsMatch(name);

			return status;
		}
	}
}

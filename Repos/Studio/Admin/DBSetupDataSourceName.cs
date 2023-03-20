using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DBSetupDataSourceName.
	/// </summary>
	public class DBSetupDataSourceName : System.Windows.Forms.Form
	{
		private WizardButtonTag _buttonTag;
		private DBSetupWizard _wizard;
        private string _dataSourceName = null;
        private bool _editable = true;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.TextBox dsNameTextBox;
        private IContainer components;

        public DBSetupDataSourceName()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_buttonTag = WizardButtonTag.None;
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
		/// Get the tag of a button that is clicked
		/// </summary>
		/// <value>One of WizardButtonTag values</value>
		internal WizardButtonTag ButtonTag
		{
			get
			{
				return this._buttonTag;
			}
		}

		/// <summary>
		/// Gets or sets the DB Setup Wizard controller
		/// </summary>
		internal DBSetupWizard Wizard
		{
			get
			{
				return _wizard;
			}
			set
			{
				_wizard = value;
			}
		}

		/// <summary>
		/// Gets the Oracle TNS name
		/// </summary>
		public string DataSourceName
		{
			get
			{
                return _dataSourceName;
			}
            set
            {
                _dataSourceName = value;
            }
		}

        /// <summary>
        /// Indicate whether the data source name is editable
        /// </summary>
        public bool Editable
        {
            get
            {
                return _editable;
            }
            set
            {
                _editable = value;
            }
        }

		/// <summary>
		/// Gets the information indicating whether it needs to create a special
		/// database for Newtera Catalog
		/// </summary>
		/// <value>true if it needs to create a database, false otherwise.</value>
		public bool NeedCreateDatabase
		{
			get
			{
				bool status = false;
				try
				{
					AdminServiceStub service = new AdminServiceStub();

					status = service.NeedCreateDatabase(_wizard.DatabaseType, _wizard.DataSource);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Server Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}

				return status;
			}
		}

        /// <summary>
        /// call server api to create a database
        /// </summary>
        public void CreateDatabase()
        {
            AdminServiceStub service = new AdminServiceStub();

            service.CreateDatabase(_wizard.DatabaseType, _wizard.DataSource);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBSetupDataSourceName));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.dsNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.dsNameTextBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // dsNameTextBox
            // 
            resources.ApplyResources(this.dsNameTextBox, "dsNameTextBox");
            this.dsNameTextBox.Name = "dsNameTextBox";
            this.dsNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.dsNameTextBox_Validating);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HighlightText;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Tag = "1";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // backButton
            // 
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.CausesValidation = false;
            this.backButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.backButton.Name = "backButton";
            this.backButton.Tag = "2";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextButton
            // 
            resources.ApplyResources(this.nextButton, "nextButton");
            this.nextButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.nextButton.Name = "nextButton";
            this.nextButton.Tag = "3";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // finishButton
            // 
            resources.ApplyResources(this.finishButton, "finishButton");
            this.finishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.finishButton.Name = "finishButton";
            this.finishButton.Tag = "4";
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // DBSetupDataSourceName
            // 
            this.AcceptButton = this.nextButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DBSetupDataSourceName";
            this.Load += new System.EventHandler(this.DBSetupDataSourceName_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Cancel;
		}

		private void backButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Back;


		}

		private void nextButton_Click(object sender, System.EventArgs e)
		{
			// validate the text in dsNameTextBox
			this.dsNameTextBox.Focus();

			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}
			else
			{
                _dataSourceName = dsNameTextBox.Text;
                this._buttonTag = WizardButtonTag.Next;
				this._wizard.DataSource = _dataSourceName;
			}
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Finish;		
		}

		private void dsNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the data source name cannot be null and has to be valid
			if (this.dsNameTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.dsNameTextBox, "Please enter an valid data source name.");
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}			
		}

        private void DBSetupDataSourceName_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_dataSourceName))
            {
                dsNameTextBox.Text = _dataSourceName;
            }

            if (!Editable)
            {
                dsNameTextBox.Enabled = false;
            }
        }
    }
}

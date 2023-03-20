using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DBSetupDBAAccount.
	/// </summary>
	public class DBSetupDBAAccount : System.Windows.Forms.Form
	{
		private WizardButtonTag _buttonTag;
		private DBSetupWizard _wizard;
		private AdminServiceStub _service;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox dataFileDirTextBox;
		private System.Windows.Forms.ErrorProvider errorProvider;
        private IContainer components;

        public DBSetupDBAAccount()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_buttonTag = WizardButtonTag.None;
			_service = new AdminServiceStub();
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
		/// Gets the DBA User Name
		/// </summary>
		public string UserName
		{
			get
			{
				return this.nameTextBox.Text;
			}
		}

		/// <summary>
		/// Gets the DBA password
		/// </summary>
		public string UserPassword
		{
			get
			{
				return this.passwordTextBox.Text;
			}
		}

		/// <summary>
		/// Gets the data file dir
		/// </summary>
		public string DataFileDir
		{
			get
			{
				return this.dataFileDirTextBox.Text;
			}
		}

		private void CreateTablespace()
		{
			try
			{

				// invoke the admin web service synchronously
				_service.CreateTablespace(_wizard.DatabaseType, _wizard.DataSource,
					_wizard.DBAUserID, _wizard.DBAUserPassword, _wizard.DataFileDir);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBSetupDBAAccount));
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataFileDirTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
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
            this.panel1.Controls.Add(this.dataFileDirTextBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.passwordTextBox);
            this.panel1.Controls.Add(this.nameTextBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // dataFileDirTextBox
            // 
            resources.ApplyResources(this.dataFileDirTextBox, "dataFileDirTextBox");
            this.dataFileDirTextBox.Name = "dataFileDirTextBox";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.passwordTextBox_Validating);
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
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
            // DBSetupDBAAccount
            // 
            this.AcceptButton = this.nextButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DBSetupDBAAccount";
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
			// validate the text in nameTextBox
			this.nameTextBox.Focus();

			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			// validate the text in passwordTextBox
			this.passwordTextBox.Focus();

			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			this._buttonTag = WizardButtonTag.Next;
			this._wizard.DBAUserID = UserName;
			this._wizard.DBAUserPassword = UserPassword;
			this._wizard.DataFileDir = DataFileDir;

			CreateTablespace();

			this.DialogResult = DialogResult.OK;
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Finish;		
		}

		private void nameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the user name cannot be null
			if (this.nameTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.nameTextBox, "Please enter a DBA user name.");
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}

		private void passwordTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the password cannot be null
			if (this.passwordTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.passwordTextBox, "Please enter a DBA password.");
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}
	}
}

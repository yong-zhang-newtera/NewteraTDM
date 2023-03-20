using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DBSetupTNSName.
	/// </summary>
	public class DBSetupTNSName : System.Windows.Forms.Form
	{
		private WizardButtonTag _buttonTag;
		private DBSetupWizard _wizard;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tnsNameTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ErrorProvider errorProvider;
        private IContainer components;

        public DBSetupTNSName()
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
				return this.tnsNameTextBox.Text;
			}
		}

		/// <summary>
		/// Gets the information indicating whether it needs to create a special
		/// tablespace for Newtera Catalog
		/// </summary>
		/// <value>true if it needs to create a tablespace, false otherwise.</value>
		public bool NeedCreateTableSpace
		{
			get
			{
				bool status = true;

				try
				{
					AdminServiceStub service = new AdminServiceStub();
				
					status = service.NeedCreateTablespace(_wizard.DatabaseType, _wizard.DataSource);
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
		/// Gets the information indicating whether the data source specified by 
		/// the name is valid or not
		/// </summary>
		/// <param name="name">The data source name</param>
		/// <returns>true if the data source is valid, false otherwise.</returns>
		private bool ValidateDataSource(string name, out string msg)
		{
			bool status = true;
            msg = "";

			AdminServiceStub service = new AdminServiceStub();

			try
			{

				status = service.IsDataSourceValid(_wizard.DatabaseType, name);
			}
			catch (Exception ex)
			{
                msg = ex.Message;
				status = false;
			}

			return status;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBSetupTNSName));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.tnsNameTextBox = new System.Windows.Forms.TextBox();
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
            this.panel1.Controls.Add(this.tnsNameTextBox);
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
            // tnsNameTextBox
            // 
            resources.ApplyResources(this.tnsNameTextBox, "tnsNameTextBox");
            this.tnsNameTextBox.Name = "tnsNameTextBox";
            this.tnsNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.tnsNameTextBox_Validating);
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
            // DBSetupTNSName
            // 
            this.AcceptButton = this.nextButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DBSetupTNSName";
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
			// validate the text in tnsNameTextBox
			this.tnsNameTextBox.Focus();

			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}
			else
			{
				this._buttonTag = WizardButtonTag.Next;
				this._wizard.DataSource = this.DataSourceName;
			}
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Finish;		
		}

		private void tnsNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
            string msg = "";

			// the data source name cannot be null and has to be valid
			if (this.tnsNameTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.tnsNameTextBox, "Please enter an valid data source name.");
				e.Cancel = true;
			}
			else if (!ValidateDataSource(this.tnsNameTextBox.Text, out msg))
			{
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "The data source is incorrect or the database is down.";
                }

				this.errorProvider.SetError(this.tnsNameTextBox, msg);
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}			
		}
	}
}

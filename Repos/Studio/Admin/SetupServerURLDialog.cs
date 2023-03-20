using System;
using System.Net;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;

using Newtera.Common.Core;
using Newtera.Common.Config;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for SetupServerURLDialog.
	/// </summary>
	public class SetupServerURLDialog : System.Windows.Forms.Form
	{		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox serverURLTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private IContainer components;
        private string _serverUrl;

		public SetupServerURLDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            _serverUrl = null;
		}

        public string ServerURL
        {
            get
            {
                return _serverUrl;
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
		/// Gets the information indicating whether the server specified by the url
		/// is valid or not
		/// </summary>
		/// <param name="serverUrl">The server url</param>
		/// <returns>true if the server is valid, false otherwise.</returns>
		private bool ValidateSever(string serverUrl)
		{
			bool status = true;

            if (!string.IsNullOrEmpty(serverUrl))
            {
                try
                {
                    AdminServiceStub adminService = new AdminServiceStub();
                    adminService.BaseUri = serverUrl;

                    string serverVersion = adminService.GetServerVersion();
                }
                catch (Exception)
                {
                    status = false;
                }
            }

			return status;
		}

		/// <summary>
		/// Update the server base url defined in the DesignStudio config file
		/// </summary>
		private void UpdateConfig()
		{
            //Create the object
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //make changes
            config.AppSettings.Settings[NewteraNameSpace.BASE_URL].Value = _serverUrl;

            //save to apply changes
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupServerURLDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.serverURLTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // serverURLTextBox
            // 
            resources.ApplyResources(this.serverURLTextBox, "serverURLTextBox");
            this.serverURLTextBox.Name = "serverURLTextBox";
            this.serverURLTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.serverURLTextBox_Validating);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // SetupServerURLDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverURLTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SetupServerURLDialog";
            this.Load += new System.EventHandler(this.SetupServerURLDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void SetupServerURLDialog_Load(object sender, System.EventArgs e)
		{
            // get server url from config file
            //Create the object
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //make changes
            string serverUrl = config.AppSettings.Settings[NewteraNameSpace.BASE_URL].Value;
            if (string.IsNullOrEmpty(serverUrl))
            {
                try
                {
                    AdminServiceStub service = new AdminServiceStub();
                    serverUrl = service.GetServerUrl();
                }
                catch (Exception)
                {
                    serverUrl = "";
                }
            }

            this.serverURLTextBox.Text = serverUrl;
		}

		private void serverURLTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the url cannot be null and has to be correct
	        if (!ValidateSever(this.serverURLTextBox.Text))
			{
				this.errorProvider.SetError(this.serverURLTextBox, "The url is incorrect or the server of given url is down.");
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// validate the text in serverURLTextBox
			this.serverURLTextBox.Focus();

			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}
			else
			{
                _serverUrl = this.serverURLTextBox.Text.Trim();

				UpdateConfig();
			}
		}
	}
}

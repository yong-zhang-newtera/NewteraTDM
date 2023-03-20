using System;
using System.Net;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;

using Newtera.Common.Config;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
    /// <summary>
    /// Summary description for SetupServerIPandPortDialog.
    /// </summary>
    public class SetupServerIPandPortDialog : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverURLTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private IContainer components;
        private ErrorProvider errorProvider;
        private AdminServiceStub _service;

        public SetupServerIPandPortDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _service = new AdminServiceStub();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupServerIPandPortDialog));
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
            // SetupServerIPandPortDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverURLTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SetupServerIPandPortDialog";
            this.Load += new System.EventHandler(this.SetupServerIIPandPortPandPortDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void SetupServerURLDialog_Load(object sender, System.EventArgs e)
        {
            // get server url from config file

            this.serverURLTextBox.Text = "";
        }

        private void serverURLTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // the url cannot be null and has to be correct
            if (this.serverURLTextBox.Text.Length == 0)
            {
                this.errorProvider.SetError(this.serverURLTextBox, "Please enter an url.");
                e.Cancel = true;
            }
            else
            {
                string url = this.serverURLTextBox.Text.Trim();
                bool isValidUrl = Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);

                if (!isValidUrl)
                {
                    this.errorProvider.SetError(this.serverURLTextBox, "The url entered is invalid");
                    e.Cancel = true;
                }
                else
                {
                    this.errorProvider.SetError((Control)sender, null);
                }
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
                string serverUrl = this.serverURLTextBox.Text.Trim();

                try
                {
                    _service.SetServerUrl(serverUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                        "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
        }

        private void SetupServerIIPandPortPandPortDialog_Load(object sender, EventArgs e)
        {
            this.serverURLTextBox.Text = _service.GetServerUrl();
        }
    }
}

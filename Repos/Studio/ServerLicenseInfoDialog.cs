using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ServerLicenseInfoDialog.
	/// </summary>
	public class ServerLicenseInfoDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label serverIdLabel;
		private System.Windows.Forms.TextBox serverIdTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button changeLicenseButton;
		private System.Windows.Forms.TextBox licenseMsgTextBox;
        private Button detailButton;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ServerLicenseInfoDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerLicenseInfoDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.licenseMsgTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serverIdTextBox = new System.Windows.Forms.TextBox();
            this.serverIdLabel = new System.Windows.Forms.Label();
            this.changeLicenseButton = new System.Windows.Forms.Button();
            this.detailButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.licenseMsgTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.serverIdTextBox);
            this.groupBox1.Controls.Add(this.serverIdLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // licenseMsgTextBox
            // 
            resources.ApplyResources(this.licenseMsgTextBox, "licenseMsgTextBox");
            this.licenseMsgTextBox.Name = "licenseMsgTextBox";
            this.licenseMsgTextBox.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // serverIdTextBox
            // 
            resources.ApplyResources(this.serverIdTextBox, "serverIdTextBox");
            this.serverIdTextBox.Name = "serverIdTextBox";
            this.serverIdTextBox.ReadOnly = true;
            // 
            // serverIdLabel
            // 
            resources.ApplyResources(this.serverIdLabel, "serverIdLabel");
            this.serverIdLabel.Name = "serverIdLabel";
            // 
            // changeLicenseButton
            // 
            resources.ApplyResources(this.changeLicenseButton, "changeLicenseButton");
            this.changeLicenseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.changeLicenseButton.Name = "changeLicenseButton";
            this.changeLicenseButton.Click += new System.EventHandler(this.changeLicenseButton_Click);
            // 
            // detailButton
            // 
            resources.ApplyResources(this.detailButton, "detailButton");
            this.detailButton.Name = "detailButton";
            this.detailButton.UseVisualStyleBackColor = true;
            this.detailButton.Click += new System.EventHandler(this.detailButton_Click);
            // 
            // ServerLicenseInfoDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.detailButton);
            this.Controls.Add(this.changeLicenseButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ServerLicenseInfoDialog";
            this.Load += new System.EventHandler(this.ServerLicenseInfoDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void ServerLicenseInfoDialog_Load(object sender, System.EventArgs e)
		{
			try
			{
				AdminServiceStub service = new AdminServiceStub();

				string str = service.GetServerId();
				serverIdTextBox.Text = str;

				str = service.GetServerLicenseMsg();
				licenseMsgTextBox.Text = str;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void changeLicenseButton_Click(object sender, System.EventArgs e)
		{
			// only system administrator is allowed to change a license key
			LoginDialog dialog = new LoginDialog();
            dialog.CheckClient = false; // do not check if the client is registered or not

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				ChangeLicenseDialog changeLicenseDialog = new ChangeLicenseDialog();

				if (changeLicenseDialog.ShowDialog() == DialogResult.OK)
				{
					if (changeLicenseDialog.LicenseKey != null &&
						changeLicenseDialog.LicenseKey.Length > 0)
					{
						try
						{
							AdminServiceStub service = new AdminServiceStub();

							// set the license key to the server
							service.SetLicenseKey(changeLicenseDialog.LicenseKey);
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.Message, "Server Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
						}
					}
				}
			}
		}

        private void detailButton_Click(object sender, EventArgs e)
        {
            // only system administrator is allowed to view a license details
            LoginDialog dialog = new LoginDialog();
            dialog.CheckClient = false; // do not check if the client is registered or not

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ServerLicenseDetailDialog licenseDialog = new ServerLicenseDetailDialog();

                try
                {
                    AdminServiceStub service = new AdminServiceStub();

                    // get the detail msg from server
                    string msg = service.GetServerLicenseDetails();
                    licenseDialog.LicenseDetails = msg;
                    licenseDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Server Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
	}
}

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
	/// ClientLicenseInfoDialog 的摘要说明。
	/// </summary>
	public class ClientLicenseInfoDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button registerClientButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox clientIDTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox licenseMsgTextBox;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ClientLicenseInfoDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientLicenseInfoDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.licenseMsgTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.clientIDTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.registerClientButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.licenseMsgTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.clientIDTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // licenseMsgTextBox
            // 
            resources.ApplyResources(this.licenseMsgTextBox, "licenseMsgTextBox");
            this.licenseMsgTextBox.Name = "licenseMsgTextBox";
            this.licenseMsgTextBox.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // clientIDTextBox
            // 
            resources.ApplyResources(this.clientIDTextBox, "clientIDTextBox");
            this.clientIDTextBox.Name = "clientIDTextBox";
            this.clientIDTextBox.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // registerClientButton
            // 
            resources.ApplyResources(this.registerClientButton, "registerClientButton");
            this.registerClientButton.Name = "registerClientButton";
            this.registerClientButton.Click += new System.EventHandler(this.registerClientButton_Click);
            // 
            // ClientLicenseInfoDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.registerClientButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ClientLicenseInfoDialog";
            this.Load += new System.EventHandler(this.ClientLicenseInfoDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void registerClientButton_Click(object sender, System.EventArgs e)
		{
			// only system administrator is allowed to register a client
			LoginDialog dialog = new LoginDialog();
            dialog.CheckClient = false; // do not check if the client is registered or not

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				RegisterClientDialog registerClientDialog = new RegisterClientDialog();

				registerClientDialog.ShowDialog();

				ShowClientLicenseMessage();
			}
		}

		private void ClientLicenseInfoDialog_Load(object sender, System.EventArgs e)
		{
			this.clientIDTextBox.Text = NewteraNameSpace.ComputerCheckSum;

			ShowClientLicenseMessage();
		}

		private void ShowClientLicenseMessage()
		{
			try
			{
				AdminServiceStub service = new AdminServiceStub();

				string msg = service.GetClientLicenseMsg(NewteraNameSpace.DESIGN_STUDIO_NAME,
					this.clientIDTextBox.Text);
				licenseMsgTextBox.Text = msg;
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

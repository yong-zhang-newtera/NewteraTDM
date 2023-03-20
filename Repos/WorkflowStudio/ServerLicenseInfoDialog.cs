using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace WorkflowStudio
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
		private System.Windows.Forms.TextBox licenseMsgTextBox;
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
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.licenseMsgTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.serverIdTextBox);
            this.groupBox1.Controls.Add(this.serverIdLabel);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // licenseMsgTextBox
            // 
            this.licenseMsgTextBox.AccessibleDescription = null;
            this.licenseMsgTextBox.AccessibleName = null;
            resources.ApplyResources(this.licenseMsgTextBox, "licenseMsgTextBox");
            this.licenseMsgTextBox.BackgroundImage = null;
            this.licenseMsgTextBox.Font = null;
            this.licenseMsgTextBox.Name = "licenseMsgTextBox";
            this.licenseMsgTextBox.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // serverIdTextBox
            // 
            this.serverIdTextBox.AccessibleDescription = null;
            this.serverIdTextBox.AccessibleName = null;
            resources.ApplyResources(this.serverIdTextBox, "serverIdTextBox");
            this.serverIdTextBox.BackgroundImage = null;
            this.serverIdTextBox.Font = null;
            this.serverIdTextBox.Name = "serverIdTextBox";
            this.serverIdTextBox.ReadOnly = true;
            // 
            // serverIdLabel
            // 
            this.serverIdLabel.AccessibleDescription = null;
            this.serverIdLabel.AccessibleName = null;
            resources.ApplyResources(this.serverIdLabel, "serverIdLabel");
            this.serverIdLabel.Font = null;
            this.serverIdLabel.Name = "serverIdLabel";
            // 
            // ServerLicenseInfoDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.Name = "ServerLicenseInfoDialog";
            this.Load += new System.EventHandler(this.ServerLicenseInfoDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void ServerLicenseInfoDialog_Load(object sender, System.EventArgs e)
		{
			// Change the cursor to indicate that we are waiting
			Cursor.Current = Cursors.WaitCursor;

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
			finally
			{
				Cursor.Current = this.Cursor;
			}
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.Studio
{
	/// <summary>
	/// ChangeUserPwdDialog 的摘要说明。
	/// </summary>
	public class ChangeUserPwdDialog : System.Windows.Forms.Form
	{
		private string _userName;
		private string _newPwd;

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox newPwdTextBox;
		
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChangeUserPwdDialog()
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
		/// Gets or sets the user name
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
			}
		}

		/// <summary>
		/// Gets or sets the user new password
		/// </summary>
		public string NewPassword
		{
			get
			{
				return _newPwd;
			}
			set
			{
				_newPwd = value;
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeUserPwdDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.newPwdTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // newPwdTextBox
            // 
            resources.ApplyResources(this.newPwdTextBox, "newPwdTextBox");
            this.newPwdTextBox.Name = "newPwdTextBox";
            this.newPwdTextBox.TextChanged += new System.EventHandler(this.newPwdTextBox_TextChanged);
            // 
            // ChangeUserPwdDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.newPwdTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChangeUserPwdDialog";
            this.Load += new System.EventHandler(this.ChangeUserPwdDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			_userName = this.nameTextBox.Text;
			_newPwd = this.newPwdTextBox.Text;
		}

		private void ChangeUserPwdDialog_Load(object sender, System.EventArgs e)
		{
			if (_userName != null)
			{
				this.nameTextBox.Text = _userName;
			}
		}

		private void newPwdTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (this.newPwdTextBox.Text.Length > 0)
			{
				this.okButton.Enabled = true;
			}
			else
			{
				this.okButton.Enabled = false;
			}
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ChangePasswordDialog.
	/// </summary>
	public class ChangePasswordDialog : System.Windows.Forms.Form
	{
		WindowClientUserManager _userManager;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox userNameTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox oldPasswordTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox newPasswordTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox newPasswordAgainTextBox;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChangePasswordDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_userManager = new WindowClientUserManager();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.oldPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.newPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.newPasswordAgainTextBox = new System.Windows.Forms.TextBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // userNameTextBox
            // 
            resources.ApplyResources(this.userNameTextBox, "userNameTextBox");
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // oldPasswordTextBox
            // 
            resources.ApplyResources(this.oldPasswordTextBox, "oldPasswordTextBox");
            this.oldPasswordTextBox.Name = "oldPasswordTextBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // newPasswordTextBox
            // 
            resources.ApplyResources(this.newPasswordTextBox, "newPasswordTextBox");
            this.newPasswordTextBox.Name = "newPasswordTextBox";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // newPasswordAgainTextBox
            // 
            resources.ApplyResources(this.newPasswordAgainTextBox, "newPasswordAgainTextBox");
            this.newPasswordAgainTextBox.Name = "newPasswordAgainTextBox";
            // 
            // applyButton
            // 
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applyButton.Name = "applyButton";
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // ChangePasswordDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.newPasswordAgainTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.newPasswordTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.oldPasswordTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.userNameTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePasswordDialog";
            this.Load += new System.EventHandler(this.ChangePasswordDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ChangePasswordDialog_Load(object sender, System.EventArgs e)
		{
			string userName = _userManager.GetSuperUserName();

			this.userNameTextBox.Text = userName;
		}

		private void applyButton_Click(object sender, System.EventArgs e)
		{
			if (_userManager.AuthenticateSuperUser(this.userNameTextBox.Text,
				this.oldPasswordTextBox.Text))
			{
				if (this.newPasswordTextBox.Text != null &&
					this.newPasswordTextBox.Text.Length > 4)
				{
					if (this.newPasswordTextBox.Text == this.newPasswordAgainTextBox.Text)
					{
						_userManager.ChangeSuperUserPassword(
							this.userNameTextBox.Text,
							this.oldPasswordTextBox.Text,
							this.newPasswordTextBox.Text);
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("DesignStudio.PasswordDifferent"),
							"Error Dialog", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						this.DialogResult = DialogResult.None; // dimiss the OK event
					}
				}
				else
				{
					MessageBox.Show(MessageResourceManager.GetString("DesignStudio.PasswordTooShort"),
						"Error Dialog", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					this.DialogResult = DialogResult.None; // dimiss the OK event
				}
			}
			else
			{
				MessageBox.Show(MessageResourceManager.GetString("DesignStudio.InvalidPassword"),
					"Error Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}
	}
}

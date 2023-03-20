using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for AddUserDialog.
	/// </summary>
	public class AddUserDialog : System.Windows.Forms.Form
	{
		private WindowClientUserManager _userManager;
		private EnumValueCollection _allUsers;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox userNameTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox passwordAgainTextBox;
        private TextBox lastNameTextBox;
        private Label label2;
        private Label label5;
        private TextBox emailTextBox;
        private Label label6;
        private TextBox firstNameTextBox;
        private IContainer components;

		public AddUserDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_userManager = new WindowClientUserManager();
			_allUsers = null;
		}

		/// <summary>
		/// Gets or sets the all existing users
		/// </summary>
		public EnumValueCollection AllUsers
		{
			get
			{
				return _allUsers;
			}
			set
			{
				_allUsers = value;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddUserDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.passwordAgainTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.errorProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.label1.Font = null;
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.AccessibleDescription = null;
            this.userNameTextBox.AccessibleName = null;
            resources.ApplyResources(this.userNameTextBox, "userNameTextBox");
            this.userNameTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.userNameTextBox, resources.GetString("userNameTextBox.Error"));
            this.userNameTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.userNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("userNameTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.userNameTextBox, ((int)(resources.GetObject("userNameTextBox.IconPadding"))));
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.userNameTextBox_Validating);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.label3.Font = null;
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.AccessibleDescription = null;
            this.passwordTextBox.AccessibleName = null;
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.passwordTextBox, resources.GetString("passwordTextBox.Error"));
            this.passwordTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.passwordTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("passwordTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.passwordTextBox, ((int)(resources.GetObject("passwordTextBox.IconPadding"))));
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.passwordTextBox_Validating);
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.errorProvider.SetError(this.label4, resources.GetString("label4.Error"));
            this.label4.Font = null;
            this.errorProvider.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
            this.label4.Name = "label4";
            // 
            // passwordAgainTextBox
            // 
            this.passwordAgainTextBox.AccessibleDescription = null;
            this.passwordAgainTextBox.AccessibleName = null;
            resources.ApplyResources(this.passwordAgainTextBox, "passwordAgainTextBox");
            this.passwordAgainTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.passwordAgainTextBox, resources.GetString("passwordAgainTextBox.Error"));
            this.passwordAgainTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.passwordAgainTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("passwordAgainTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.passwordAgainTextBox, ((int)(resources.GetObject("passwordAgainTextBox.IconPadding"))));
            this.passwordAgainTextBox.Name = "passwordAgainTextBox";
            this.passwordAgainTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.passwordAgainTextBox_Validating);
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.errorProvider.SetError(this.okButton, resources.GetString("okButton.Error"));
            this.errorProvider.SetIconAlignment(this.okButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("okButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.okButton, ((int)(resources.GetObject("okButton.IconPadding"))));
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.errorProvider.SetError(this.cancelButton, resources.GetString("cancelButton.Error"));
            this.cancelButton.Font = null;
            this.errorProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.cancelButton, ((int)(resources.GetObject("cancelButton.IconPadding"))));
            this.cancelButton.Name = "cancelButton";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.label2.Font = null;
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            // 
            // lastNameTextBox
            // 
            this.lastNameTextBox.AccessibleDescription = null;
            this.lastNameTextBox.AccessibleName = null;
            resources.ApplyResources(this.lastNameTextBox, "lastNameTextBox");
            this.lastNameTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.lastNameTextBox, resources.GetString("lastNameTextBox.Error"));
            this.lastNameTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.lastNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lastNameTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.lastNameTextBox, ((int)(resources.GetObject("lastNameTextBox.IconPadding"))));
            this.lastNameTextBox.Name = "lastNameTextBox";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.errorProvider.SetError(this.label5, resources.GetString("label5.Error"));
            this.label5.Font = null;
            this.errorProvider.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding"))));
            this.label5.Name = "label5";
            // 
            // firstNameTextBox
            // 
            this.firstNameTextBox.AccessibleDescription = null;
            this.firstNameTextBox.AccessibleName = null;
            resources.ApplyResources(this.firstNameTextBox, "firstNameTextBox");
            this.firstNameTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.firstNameTextBox, resources.GetString("firstNameTextBox.Error"));
            this.firstNameTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.firstNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("firstNameTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.firstNameTextBox, ((int)(resources.GetObject("firstNameTextBox.IconPadding"))));
            this.firstNameTextBox.Name = "firstNameTextBox";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.errorProvider.SetError(this.label6, resources.GetString("label6.Error"));
            this.label6.Font = null;
            this.errorProvider.SetIconAlignment(this.label6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label6.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label6, ((int)(resources.GetObject("label6.IconPadding"))));
            this.label6.Name = "label6";
            // 
            // emailTextBox
            // 
            this.emailTextBox.AccessibleDescription = null;
            this.emailTextBox.AccessibleName = null;
            resources.ApplyResources(this.emailTextBox, "emailTextBox");
            this.emailTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.emailTextBox, resources.GetString("emailTextBox.Error"));
            this.emailTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.emailTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("emailTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.emailTextBox, ((int)(resources.GetObject("emailTextBox.IconPadding"))));
            this.emailTextBox.Name = "emailTextBox";
            // 
            // AddUserDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.passwordAgainTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.userNameTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddUserDialog";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		/// <summary>
		/// Gets the information indicating whether the given user name is unique
		/// </summary>
		/// <param name="name">user name</param>
		/// <returns>true if it is unique, false otherwise</returns>
		private bool ValidateUserNameUniqueness(string name)
		{
			bool status = true;

			if (_allUsers != null)
			{
				foreach (EnumValue user in _allUsers)
				{
					if (user.Value == name)
					{
						status = false;
						break;
					}
				}
			}

			return status;
		}

        /// <summary>
        /// Gets the information indicating whether the given user display text is unique
        /// </summary>
        /// <param name="name">user display text</param>
        /// <returns>true if it is unique, false otherwise</returns>
        private bool ValidateUserDisplayTextUniqueness(string text)
        {
            bool status = true;

            if (_allUsers != null)
            {
                foreach (EnumValue user in _allUsers)
                {
                    if (user.DisplayText == text)
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Due to a .net bug, an user id can't be an integer
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>true if it is a valid user id, false otherwise</returns>
        private bool IsValidUserID(string userId)
        {
            bool status = true;

            try
            {
                int.Parse(userId);
                status = false;
            }
            catch (Exception)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether user id is reserved
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>true if it is a reserved user id, false otherwise</returns>
        private bool IsReservedUserID(string userId)
        {
            bool status = false;

            if (userId.Trim() == _userManager.GetSuperUserName())
            {
                status = true;
            }

            return status;
        }

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// validate the text in the textbox
			this.userNameTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			this.passwordTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			this.passwordAgainTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

            if (!string.IsNullOrEmpty(this.lastNameTextBox.Text) ||
                !string.IsNullOrEmpty(this.firstNameTextBox.Text))
            {
                string userDisplayText = UsersListHandler.GetFormatedName(this.lastNameTextBox.Text, this.firstNameTextBox.Text);
                if (!ValidateUserDisplayTextUniqueness(userDisplayText))
                {
                    string msg = string.Format(WindowsControl.MessageResourceManager.GetString("DesignStudio.UserDisplayNameExist"), userDisplayText);
                    MessageBox.Show(msg,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            try
            {
                string[] userData = new string[3];
                userData[0] = this.lastNameTextBox.Text;
                userData[1] = this.firstNameTextBox.Text;
                userData[2] = this.emailTextBox.Text;

                _userManager.AddUser(this.userNameTextBox.Text, this.passwordTextBox.Text, userData);

                // clear the enum types created for the role list
                EnumTypeFactory.Instance.ClearEnumTypes();
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                string msg = WinClientUtil.GetOriginalMessage(ex.Message);

                MessageBox.Show(msg,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.DialogResult = DialogResult.None;
                return;
            }
		}

		private void userNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null and has to be unique
			if (this.userNameTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.userNameTextBox, WindowsControl.MessageResourceManager.GetString("DesignStudio.EnterUserId"));
				e.Cancel = true;
			}
			else if (!ValidateUserNameUniqueness(this.userNameTextBox.Text))
			{
				this.errorProvider.SetError(this.userNameTextBox, WindowsControl.MessageResourceManager.GetString("DesignStudio.UserIdExist"));
				e.Cancel = true;
			}
            else if (IsReservedUserID(this.userNameTextBox.Text))
            {
                string msg = string.Format(WindowsControl.MessageResourceManager.GetString("DesignStudio.ReservedUserId"), this.userNameTextBox.Text);
                this.errorProvider.SetError(this.userNameTextBox, msg);
                e.Cancel = true;
            }
            else if (!IsValidUserID(this.userNameTextBox.Text))
            {
                this.errorProvider.SetError(this.userNameTextBox, WindowsControl.MessageResourceManager.GetString("DesignStudio.InvalidUserId"));
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
                this.errorProvider.SetError(this.passwordTextBox, WindowsControl.MessageResourceManager.GetString("DesignStudio.EnterPassword"));
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}

		private void passwordAgainTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the password cannot be null
			if (this.passwordAgainTextBox.Text.Length == 0)
			{
                this.errorProvider.SetError(this.passwordAgainTextBox, WindowsControl.MessageResourceManager.GetString("DesignStudio.EnterPasswordAgain"));
				e.Cancel = true;
			}
			else if (this.passwordTextBox.Text != this.passwordAgainTextBox.Text)
			{
				// two passwords ain't equal
				this.errorProvider.SetError(this.passwordAgainTextBox, WindowsControl.MessageResourceManager.GetString("DesignStudio.DifferentPasswords"));
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}
	}
}

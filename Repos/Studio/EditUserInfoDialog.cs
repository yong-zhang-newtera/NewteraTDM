using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WindowsControl;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
    public partial class EditUserInfoDialog : Form
    {
        WindowClientUserManager _userManager;
        private string _userId;
        private string[] _userData;
        private EnumValueCollection _allUsers;

        public EditUserInfoDialog()
        {
            InitializeComponent();

            _userManager = new WindowClientUserManager();
        }

        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string UserID
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }

        /// <summary>
        /// Gets or sets the user data
        /// </summary>
        public string[] UserData
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        /// <summary>
        /// Get or sets a collection of user's enum values for validation purpose
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
                    if (user.DisplayText == text && user.Value != this._userId)
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }

        private void EditUserInfoDialog_Load(object sender, EventArgs e)
        {
            // display the user id and user data
            this.userNameTextBox.Text = _userId;

            if (_userData != null)
            {
                if (_userData.Length > 0)
                {
                    this.lastNameTextBox.Text = _userData[0];
                }

                if (_userData.Length > 1)
                {
                    this.firstNameTextBox.Text = _userData[1];
                }

                if (_userData.Length > 2)
                {
                    this.emailTextBox.Text = _userData[2];
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.userNameTextBox.Text))
            {
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

                _userData[0] = this.lastNameTextBox.Text;
                _userData[1] = this.firstNameTextBox.Text;
                _userData[2] = this.emailTextBox.Text;

                _userManager.ChangeUserData(
                    this.userNameTextBox.Text.Trim(),
                    _userData);

                // clear the enum types created for the role list
                EnumTypeFactory.Instance.ClearEnumTypes();
            }
        }
    }
}
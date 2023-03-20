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
    public partial class EditRoleInfo : Form
    {
        WindowClientUserManager _userManager;
        private string _roleName;
        private string _roleText;
        private EnumValueCollection _allRoles;

        public EditRoleInfo()
        {
            InitializeComponent();
            _userManager = new WindowClientUserManager();
        }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string RoleName
        {
            get
            {
                return _roleName;
            }
            set
            {
                _roleName = value;
            }
        }

        /// <summary>
        /// Gets or sets the role text
        /// </summary>
        public string RoleText
        {
            get
            {
                return _roleText;
            }
            set
            {
                _roleText = value;
            }
        }

        /// <summary>
        /// Get or sets a collection of role's enum values for validation purpose
        /// </summary>
        public EnumValueCollection AllRoles
        {
            get
            {
                return _allRoles;
            }
            set
            {
                _allRoles = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the role text is unique
        /// </summary>
        /// <param name="name">role text</param>
        /// <returns>true if it is unique, false otherwise</returns>
        private bool ValidateRoleTextUniqueness(string text)
        {
            bool status = true;

            if (_allRoles != null)
            {
                foreach (EnumValue role in _allRoles)
                {
                    if (role.DisplayText == text && role.Value != this._roleName)
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }

        private void EditRoleInfo_Load(object sender, EventArgs e)
        {
            this.roleNameTextBox.Text = _roleName;

            if (_roleText != null)
            {
                this.roleTextTextBox.Text = _roleText;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.roleTextTextBox.Text))
            {
                if (!ValidateRoleTextUniqueness(this.roleTextTextBox.Text))
                {
                    string msg = string.Format(WindowsControl.MessageResourceManager.GetString("DesignStudio.RoleDisplayNameExist"), this.roleTextTextBox.Text);
                    MessageBox.Show(msg,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.None;
                    return;
                }

                string[] roleData = new string[1];
                roleData[0] = this.roleTextTextBox.Text;
                _roleText = this.roleTextTextBox.Text;
                _userManager.ChangeRoleData(
                    this.roleNameTextBox.Text.Trim(),
                    roleData);

                // clear the enum types created for the role list
                EnumTypeFactory.Instance.ClearEnumTypes();
            }
        }
    }
}
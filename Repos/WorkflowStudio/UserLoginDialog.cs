using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;
using Newtera.WinClientCommon;
using Newtera.WorkflowStudioControl;

namespace WorkflowStudio
{
    public partial class UserLoginDialog : Form
    {
        private IUserManager _userManager;
        private bool _isAuthenticated = false;

        public UserLoginDialog()
        {
            InitializeComponent();

            _userManager = new WindowClientUserManager();
        }

        /// <summary>
        /// Login to the server
        /// </summary>
        private void Login()
        {
            string userName = this.nameTextBox.Text;
            string password = this.passwordTextBox.Text;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                _isAuthenticated = _userManager.Authenticate(userName, password);
                if (_isAuthenticated)
                {
                    // attach a custom principal object to the thread
                    CustomPrincipal.Attach(new WindowClientUserManager(), new WindowClientServerProxy(), userName);

                    this.DialogResult = DialogResult.OK; // close the dialog
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.InvalidUserLogin"), "Error Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.None; // dimiss the OK event
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // dimiss the OK event
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }
        }

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                Login();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Login();
        }
    }
}
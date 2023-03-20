using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.Principal;
using Newtera.WorkflowStudioControl;

namespace WorkflowStudio
{
    public partial class AdminLoginDialog : Form
    {
        private WindowClientUserManager _userManager;

        public AdminLoginDialog()
        {
            InitializeComponent();

            _userManager = new WindowClientUserManager();
        }

        private bool AuthenticateSuperUser()
        {
            bool authenticated = false;
            string userName = this.nameTextBox.Text;
            string password = this.passwordTextBox.Text;

            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (userName == _userManager.GetSuperUserName())
                {
                    if (_userManager.AuthenticateSuperUser(userName, password))
                    {
                        // attach a custom principal object to the thread
                        CustomPrincipal.Attach(new WindowClientUserManager(), new WindowClientServerProxy(), userName);

                        authenticated = true;
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.InvalidAdminPassword"), "Error Dialog", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                {

                    MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.InvalidAdminID"), "Error Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }

            return authenticated;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!AuthenticateSuperUser())
            {
                this.DialogResult = DialogResult.None; // dimiss the OK event
            }
        }

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (AuthenticateSuperUser())
                {
                    this.DialogResult = DialogResult.OK; // close the dialog
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;
using Newtera.WindowsControl;
using Newtera.WinClientCommon;

namespace SmartExcel2013
{
    public partial class UserLoginDialog : Form
    {
        private IUserManager _userManager;
        private bool _isAuthenticated = false;
        private bool _checkClientLicense = false; // turn off check client license from 7.0 version
        private string _userName = null;

        public UserLoginDialog()
        {
            InitializeComponent();

            _userManager = new WindowClientUserManager();
        }

        /// <summary>
        /// Gets the information indicating whether the user has been authenticated
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        /// <summary>
        /// Gets the authenticated user name
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
        }

        /// <summary>
        /// Login to the server
        /// </summary>
        private void Login()
        {
            bool isRegistered = false;

            // check if the SmartWord has been registered or not
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            try
            {

                AdminServiceStub service = new AdminServiceStub();

                if (_checkClientLicense)
                {
                    // server throws an exception if the client has not been registered
                    service.CheckInClient(NewteraNameSpace.SMART_WORD_NAME,
                        NewteraNameSpace.ComputerCheckSum);
                }

                isRegistered = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // dimiss the OK event
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }

            if (isRegistered)
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

                        _userName = userName;

                        this.DialogResult = DialogResult.OK; // close the dialog
                    }
                    else
                    {
                        MessageBox.Show(MessageResourceManager.GetString("DesignStudio.InvalidUserLogin"), "Error Dialog", MessageBoxButtons.OK,
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
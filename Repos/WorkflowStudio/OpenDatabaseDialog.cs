using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;
using Newtera.WFModel;
using Newtera.WinClientCommon;
using Newtera.WorkflowStudioControl;

namespace WorkflowStudio
{
    public partial class OpenDatabaseDialog : Form
    {
        private string _selectedProjectName;
        private string _selectedProjectVersion;
        private IUserManager _userManager;
        private WorkflowModelServiceStub _workflowService;
        private bool _isAuthenticated = false;
        private bool _lockProject = true;
        private bool _isLockObtained = false;
        private bool _isLatestVersion = false;
        private bool _checkClientLicense = false; // turn off check client license from 7.0 version
        private ProjectInfo[] _existingProjectInfos;

        public OpenDatabaseDialog()
        {
            InitializeComponent();

            _workflowService = new WorkflowModelServiceStub();
            _userManager = new WindowClientUserManager();
        }

        /// <summary>
        /// Gets the selected project name
        /// </summary>
        public string ProjectName
        {
            get
            {
                return _selectedProjectName;
            }
        }

        /// <summary>
        /// Gets the selected project version
        /// </summary>
        public string ProjectVersion
        {
            get
            {
                return _selectedProjectVersion;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether to obtain the lock
        /// for update
        /// </summary>
        public bool LockProject
        {
            get
            {
                return this._lockProject;
            }
            set
            {
                this._lockProject = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether a lock to the meta data has
        /// been obtained.
        /// </summary>
        public bool IsLockObtained
        {
            get
            {
                return this._isLockObtained;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the selected project is the latest version
        /// </summary>
        public bool IsLatestVersion
        {
            get
            {
                return this._isLatestVersion;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the login user has a role
        /// as project administrator
        /// </summary>
        public bool IsDBAUser
        {
            get
            {
                bool status = true;
                string dbaRole = null;

                if (_selectedProjectName != null && _isAuthenticated)
                {
                    try
                    {
                        // get the dba role for the schema
                        dbaRole = _workflowService.GetDBARole(ConnectionStringBuilder.Instance.Create(), this._selectedProjectName, this._selectedProjectVersion);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Server Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    // if dba role is null, then there isnt a specific role as dba
                    if (dbaRole != null)
                    {
                        // dbaRole may consists of more than one roles, break it into an array
                        string[] dbaRoles = dbaRole.Split(';');
                        string userName = this.nameTextBox.Text.Trim();

                        // get all roles of the current user
                        string[] roles = _userManager.GetRoles(userName);

                        bool isDBA = true;
                        bool found;
                        foreach (string dba in dbaRoles)
                        {
                            found = false;
                            foreach (string role in roles)
                            {
                                // super user is a dba by default
                                if (role == dba || role == Newtera.Common.Core.NewteraNameSpace.CM_SUPER_USER_ROLE)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                isDBA = false;
                                break;
                            }
                        }

                        if (!isDBA)
                        {
                            // the user does not matche all dba roles
                            status = false;
                        }
                    }
                }
                else
                {
                    status = false;
                }

                return status;
            }
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        private void ConnectServer()
        {
            bool isRegistered = false;

            // check if the workflow studio has been registered or not
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            try
            {

                AdminServiceStub service = new AdminServiceStub();

                // server throws an exception if the client has not been registered
                if (_checkClientLicense)
                {
                    service.CheckInClient(NewteraNameSpace.WORKFLOW_STUDIO_NAME,
                        NewteraNameSpace.ComputerCheckSum);
                }

                isRegistered = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
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

                        ProjectInfo selectedProjectInfo = GetProjectInfo(_selectedProjectName, _selectedProjectVersion);

                        // check to see if the project is the latest version, only the latest version can be modified
                        _isLatestVersion = _workflowService.IsLatestVersion(ConnectionStringBuilder.Instance.Create(selectedProjectInfo.ModifiedTime), _selectedProjectName, _selectedProjectVersion);

                        if (_isLatestVersion)
                        {
                            if (_lockProject)
                            {
                                // lock the meta data model for update
                                try
                                {
                                    _workflowService.LockProject(_selectedProjectName, _selectedProjectVersion, ConnectionStringBuilder.Instance.Create(selectedProjectInfo.ModifiedTime));

                                    _isLockObtained = true;
                                }
                                catch (Exception ex)
                                {
                                    _isLockObtained = false;
                                    MessageBox.Show(ex.Message,
                                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                _isLockObtained = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.NotLatestVersion"),
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

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
        }

        private ProjectInfo GetProjectInfo(string projectName, string projectVersion)
        {
            ProjectInfo projectInfo = null;
            foreach (ProjectInfo pi in _existingProjectInfos)
            {
                if (pi.Name == projectName && pi.Version == projectVersion)
                {
                    projectInfo = pi;
                    break;
                }
            }

            return projectInfo;
        }

        private void OpenDatabaseDialog_Load(object sender, EventArgs e)
        {
            if (this._lockProject)
            {
                this.lockCheckBox.Checked = true;
            }
            else
            {
                this.lockCheckBox.Checked = false;
            }

            _existingProjectInfos = _workflowService.GetExistingProjectInfos();
            ListViewItem listViewItem;
            for (int i = 0; i < _existingProjectInfos.Length; i++)
            {
                listViewItem = new ListViewItem(_existingProjectInfos[i].Name);
                listViewItem.SubItems.Add(_existingProjectInfos[i].Version);
                listViewItem.ImageIndex = 0;
                listViewItem.StateImageIndex = 0;

                if (i == 0)
                {
                    // select the first item by default
                    listViewItem.Selected = true;
                }

                this.listView.Items.Add(listViewItem);
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 1)
            {
                this._selectedProjectName = listView.SelectedItems[0].Text;
                this._selectedProjectVersion = listView.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                ConnectServer();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ConnectServer();
        }

        private void lockCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this._lockProject = lockCheckBox.Checked;
        }
    }
}
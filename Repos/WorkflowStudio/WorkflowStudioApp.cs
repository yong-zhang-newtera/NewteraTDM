/*
* @(#)WorkflowStudioApp.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;

using Newtera.WorkflowStudioControl;
using WorkflowStudio.Util;
using Newtera.Common.MetaData;
using Newtera.WFModel;
using Newtera.Activities;
using Newtera.WinClientCommon;
using Newtera.WorkflowMonitor;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.Core;

namespace WorkflowStudio
{
	public partial class WorkflowStudioApp : Form
	{
        private const string DATA_DIR = "wfdata";

        private MenuItemStates _menuItemStates;
        private string _projectFileName = null;
        private bool _isDBAUser;
        private Backup _backup;
        private MethodInvoker _backupMethod;
        private Restore _restore;
        private MethodInvoker _restoreMethod;
        private bool _isUserAuthenticated = false; // true if the user has been authenticated
        private WorkInProgressDialog _workInProgressDialog;
        private bool _isRequestComplete;

        public WorkflowStudioApp()
		{
			InitializeComponent();

            _menuItemStates = new MenuItemStates();

            _menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

            _workInProgressDialog = new WorkInProgressDialog();

            _backupMethod = null;
            _backup = null;
            _restoreMethod = null;
            _restore = null;

            // set the activity validate service to the singleton
            ActivityValidateService validateService = new ActivityValidateService();
            ActivityValidatingServiceProvider.Instance.ValidateService = validateService;

            // set windows client flag
            GlobalSettings.Instance.IsWindowClient = true;
		}

        /// <summary>
        /// Gets or sets the information indicating whether the currently loggin user
        /// has DBA role.
        /// </summary>
        public bool IsDBAUser
        {
            get
            {
                return _isDBAUser;
            }
            set
            {
                _isDBAUser = value;
            }
        }

        /// <summary>
        /// Show the working dialog
        /// </summary>
        /// <remarks>It has to deal with multi-threading issue</remarks>
        private void ShowWorkingDialog(bool cancellable, MethodInvoker cancellCallback)
        {
            lock (_workInProgressDialog)
            {
                _workInProgressDialog.EnableCancel = cancellable;
                _workInProgressDialog.CancelCallback = cancellCallback;

                // check _isRequestComplete flag in case the worker thread
                // completes the request before the working dialog is shown
                if (!_isRequestComplete && !_workInProgressDialog.Visible)
                {
                    _workInProgressDialog.ShowDialog();
                }
            }
        }

        private delegate void HideWorkingDialogDelegate();

        /// <summary>
        /// Hide the working dialog
        /// </summary>
        /// <remarks>Has to condider multi-threading issue</remarks>
        private void HideWorkingDialog()
        {
            if (this.InvokeRequired == false)
            {
                // It is the UI thread, go ahead to close the working dialog
                // lock it while updating _isRequestComplete
                lock (_workInProgressDialog)
                {
                    _workInProgressDialog.Close();
                    _isRequestComplete = true;
                }
            }
            else
            {
                // It is a worker thread, pass the control to the UI thread
                HideWorkingDialogDelegate hideWorkingDialog = new HideWorkingDialogDelegate(HideWorkingDialog);
                this.BeginInvoke(hideWorkingDialog);
            }
        }

        /// <summary>
        /// Initialize the menu item states after a project is loaded
        /// </summary>
        private void InitializeMenuItemStates()
        {
            _menuItemStates.SetState(MenuItemID.FileSaveAsFile, true);
            _menuItemStates.SetState(MenuItemID.FileSaveDatabase, true);
            _menuItemStates.SetState(MenuItemID.FileSaveFile, true);
            _menuItemStates.SetState(MenuItemID.FileSaveDatabaseAs, true);
            _menuItemStates.SetState(MenuItemID.FilePreview, true);
            _menuItemStates.SetState(MenuItemID.FilePrint, true);
            _menuItemStates.SetState(MenuItemID.FileSetup, true);
            _menuItemStates.SetState(MenuItemID.EditAdd, false);
            _menuItemStates.SetState(MenuItemID.EditCopy, true);
            _menuItemStates.SetState(MenuItemID.EditCut, true);
            _menuItemStates.SetState(MenuItemID.EditDelete, false);
            _menuItemStates.SetState(MenuItemID.EditPaste, true);
            _menuItemStates.SetState(MenuItemID.ToolUnlock, false);
            _menuItemStates.SetState(MenuItemID.ToolUnlock, false);
            _menuItemStates.SetState(MenuItemID.ToolAccessControl, true);
            _menuItemStates.SetState(MenuItemID.WorkflowCollapse, true);
            _menuItemStates.SetState(MenuItemID.WorkflowStart, false);
            _menuItemStates.SetState(MenuItemID.ViewWorkflowMonitor, false);
            _menuItemStates.SetState(MenuItemID.WorkflowValidate, true);
            _menuItemStates.SetState(MenuItemID.WorkflowExpand, true);
            _menuItemStates.SetState(MenuItemID.WorkflowZoomLevel, true);
            _menuItemStates.SetState(MenuItemID.WorkflowTool, true);
        }

        private void MenuItemStateChanged(object sender, System.EventArgs e)
        {
            StateChangedEventArgs args = (StateChangedEventArgs)e;

            // set the toolbar button states
            switch (args.ID)
            {
                case MenuItemID.FileSaveFile:
                    this.SaveMenuItem.Enabled = args.State;
                    this.saveToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.FileSaveAsFile:
                    this.SaveAsMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.FileSaveDatabase:
                    this.SaveToDatabaseMenuItem.Enabled = args.State;
                    this.saveDataBaseToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.FileSaveDatabaseAs:
                    this.SaveToDatabaseAsMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.FileSetup:
                    this.PageSetupMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.FilePreview:
                    this.PrintPreviewMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.FilePrint:
                    this.PrintMenuItem.Enabled = args.State;
                    this.printToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.EditAdd:
                    this.AddMenuItem.Enabled = args.State;
                    this.addToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.EditDelete:
                    this.DeleteMenuItem.Enabled = args.State;
                    this.deleteToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.EditCopy:
                    this.copyToolStripButton.Enabled = args.State;
                    this.CopyMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditCut:
                    this.CutMenuItem.Enabled = args.State;
                    this.cutToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.EditPaste:
                    this.PasteMenuItem.Enabled = args.State;
                    this.pasteToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.WorkflowExpand:
                    this.expandMenuItem.Enabled = args.State;
                    this.expandToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.WorkflowCollapse:
                    this.collapseMenuItem.Enabled = args.State;
                    this.collapseToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.WorkflowValidate:
                    this.validateWorkflowsMenuItem.Enabled = args.State;
                    this.validateWorkflowsToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.WorkflowStart:
                    this.startWorkflowMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.ViewWorkflowMonitor:
                    this.monitorToolStripButton.Enabled = args.State;
                    this.WorkflowMonitorMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.WorkflowZoomLevel:
                    this.zoomLevelsMenu.Enabled = args.State;
                    this.zoomLevelToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.WorkflowTool:
                    this.NavigationToolsMenu.Enabled = args.State;
                    this.zoomInNavigationMenuItem.Enabled = args.State;
                    this.zoomOutToolStripButton.Enabled = args.State;
                    this.zoomInToolStripButton.Enabled = args.State;
                    break;
                case MenuItemID.ToolLock:
                    this.LockMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.ToolUnlock:
                    this.UnlockMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.ToolAccessControl:
                    this.accessControlMenuItem.Enabled = args.State;
                    break;
            }
        }

        private void AddWorkflowModel()
        {
            AddWorkflowDialog dialog = new AddWorkflowDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.workflowStudioControl.AddWorkflowModel(dialog.WorkflowName, dialog.WorkflowType);
            }
        }

        // monitor the instances of the current selected workflow
        private void MonitorWorkflows()
        {
            // this action requires the user to be authenticated
            if (!this._isUserAuthenticated)
            {
                UserLoginDialog loginDialog = new UserLoginDialog();
                if (loginDialog.ShowDialog() == DialogResult.OK)
                {
                    this._isUserAuthenticated = true;
                }
            }

            // And project has been saved to database.
            if (this._isUserAuthenticated && this.workflowStudioControl.Project.IsLoadedFromDB)
            {
                MonitorWorkflowDialog monitorDialog = new MonitorWorkflowDialog();
                WorkflowModel workflowModel = this.workflowStudioControl.CurrentWorkflowModel;
                monitorDialog.WorkflowID = workflowModel.ID;
                monitorDialog.WorkflowModel = workflowModel;
                monitorDialog.SubWorkflowInstanceViewed += new EventHandler<SubWorkflowInstanceViewedEventArgs>(MonitorDialog_SubWorkflowInstanceViewed);
                monitorDialog.Show();

            }
        }

        /// <summary>
        /// Start a workflow instance
        /// </summary>
        private void StartWorkflow()
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                WorkflowModelServiceStub webService = new WorkflowModelServiceStub();
                string wfInstanceId = webService.StartWorkflow(ConnectionStringBuilder.Instance.Create(),
                    this.workflowStudioControl.Project.Name,
                    this.workflowStudioControl.Project.Version,
                    this.workflowStudioControl.CurrentWorkflowModel.Name);

                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.WorkflowStarted") + wfInstanceId,
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // locking failed
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// Display message on the status bar
        /// </summary>
        /// <param name="msg"></param>
        private void ShowWorkingStatus(string msg)
        {
            this.toolStripStatusLabel1.Text = msg;
        }

        /// <summary>
        /// Save the project to files
        /// </summary>
        private void SaveProjectToFile()
        {
            if (_projectFileName == null)
            {
                _projectFileName = GetSaveFileName();
            }

            if (_projectFileName != null)
            {
                // make sure that all workflow data has been loaded before saving
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    this.workflowStudioControl.Project.LoadAll();
                }
                finally
                {
                    Cursor.Current = this.Cursor;
                }

                this.workflowStudioControl.SaveProject(_projectFileName);
            }
        }

        private string GetSaveFileName()
        {
            string fileName = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "workflow project files (*.wfproj)|*.wfproj";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = false;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                if (!fileName.EndsWith(".wfproj"))
                {
                    fileName += ".wfproj";
                }
            }

            return fileName;
        }

        /// <summary>
        /// Show the corresponding tab view
        /// </summary>
        private void ShowTabView(NavigationTabView tabView, bool setMenuItemOnly)
        {
            this.ProjectMenuItem.Checked = false;
            this.PropertiesMenuItem.Checked = false;
            this.ActivitiesMenuItem.Checked = false;
            switch (tabView)
            {
                case NavigationTabView.Project:
                    this.ProjectMenuItem.Checked = true;
                    break;

                case NavigationTabView.Properties:
                    this.PropertiesMenuItem.Checked = true;
                    break;

                case NavigationTabView.Activities:
                    this.ActivitiesMenuItem.Checked = true;
                    break;
            }

            if (!setMenuItemOnly)
            {
                this.workflowStudioControl.ShowTabView(tabView);
            }
        }

        /// <summary>
        /// Save the project to the database
        /// </summary>
        private void SaveProjectToDatabase()
        {
            if (!this.workflowStudioControl.Project.IsLoadedFromDB)
            {
                // the project was not loaded from database, call "Save As"
                SaveProjectToDatabaseAs();
            }
            else
            {
                if (this.IsDBAUser)
                {
                    if (!HasRunningWorkflowInstances())
                    {
                        // save to database
                        if (MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.SaveProject"),
                            "Confirm Dialog", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            PerformSavingToDatabase();
                        }
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.HasRunningInstances"),
                                            "Confirm",
                                            MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            // save as a newer version
                            string dbaRole = GetDBARoleFromProject(); // get the dba role of the project of current version
                            IncreaseProjectVersion();
                            PerformSavingToDatabase();
                            SetDBARoleToProject(dbaRole); // set the dba role to the project of new version
                            ProjectModel projectModel = this.workflowStudioControl.Project;
                            LoadProjectFromDatabase(projectModel.Name, projectModel.Version, projectModel.IsLockObtained, true); // show the new version of project
                        }
                        else if (result == DialogResult.No)
                        {
                            // override the existing project
                            PerformSavingToDatabase();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.DBARequired"));
                }
            }
        }

        /// <summary>
        /// Save the project as an new project in the database
        /// </summary>
        private void SaveProjectToDatabaseAs()
        {
            // only system administrator is allowed to save a new project to database
            AdminLoginDialog dialog = new AdminLoginDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // admin user is a super user
                this.IsDBAUser = true;

                SaveProjectAsDialog saveAsDialog = new SaveProjectAsDialog();
                if (this.workflowStudioControl.Project != null)
                {
                    saveAsDialog.ProjectName = this.workflowStudioControl.Project.Name;
                }

                if (saveAsDialog.ShowDialog() == DialogResult.OK)
                {
                    this.workflowStudioControl.Project.Name = saveAsDialog.ProjectName;
                    this.workflowStudioControl.Project.Version = saveAsDialog.ProjectVersion;

                    // make sure that all workflow data has been loaded before saving
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        this.workflowStudioControl.Project.LoadAll();
                    }
                    finally
                    {
                        Cursor.Current = this.Cursor;
                    }

                    if (!saveAsDialog.IsOverridingExistingProject)
                    {
                        PerformSavingToDatabase();
                    }
                    else
                    {
                        // read the existing project model from database since it contains identifiers assigned
                        // to the workflows
                        WorkflowModelServiceStub webService = new WorkflowModelServiceStub();

                        string xmlString = webService.GetProject(ConnectionStringBuilder.Instance.Create(), this.workflowStudioControl.Project.Name, this.workflowStudioControl.Project.Version);
                        ProjectModel project = new ProjectModel(this.workflowStudioControl.Project.Name);
                        StringReader reader = new StringReader(xmlString);
                        project.Read(reader);

                        // set the ids to the current project and workflows
                        this.workflowStudioControl.Project.ID = project.ID;
                        // set the timestamp of project to the current time so that it tells
                        // server that this project model is newer
                        this.workflowStudioControl.Project.ModifiedTime = DateTime.Now; 
                        foreach (WorkflowModel workflowModel in this.workflowStudioControl.Project.Workflows)
                        {
                            if (project.Workflows[workflowModel.Name] != null)
                            {
                                workflowModel.ID = project.Workflows[workflowModel.Name].ID;
                            }
                        }

                        // overring an existing project, ask the admin whether to override or save as
                        // a newer version if there are workflow instances running
                        if (!DoesExistingProjectHaveRunningWorkflowInstances())
                        {
                            // save to database
                            if (MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.SaveProject"),
                                "Confirm Dialog", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                PerformSavingToDatabase();
                            }
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.HasRunningInstances"),
                                                "Confirm",
                                                MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                // save as a newer version
                                string dbaRole = GetDBARoleFromProject(); // get the dba role of the project of current version
                                IncreaseProjectVersion();
                                PerformSavingToDatabase();
                                SetDBARoleToProject(dbaRole); // set the dba role to the project of new version
                            }
                            else if (result == DialogResult.No)
                            {
                                // override the existing project
                                PerformSavingToDatabase();
                            }
                        }
                    }

                    // re-load the project from the database
                    ProjectModel projectModel = this.workflowStudioControl.Project;
                    LoadProjectFromDatabase(projectModel.Name, projectModel.Version, true, true); // show the new project
                }
            }
        }

        /// <summary>
		/// Save the current project to the database
		/// </summary>
        private void PerformSavingToDatabase()
        {
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            // display a text in the status bar
            this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.SavingProjectToDB"));

            try
            {
                // flush the workflow that is currently display at the designer
                this.workflowStudioControl.Flush();

                // make sure there are not any validation errors
                ValidationErrorCollection validationErrors = this.workflowStudioControl.ValidateWorkflows();
                if (validationErrors != null)
                {
                    ValidationErrorsDialog errorDialog = new ValidationErrorsDialog(validationErrors);
                    errorDialog.Show();

                    return;
                }

                StringBuilder projectBuilder, policyBuilder;
                StringWriter projectWriter, policyWriter;

                // save project xml to the database
                projectBuilder = new StringBuilder();
                projectWriter = new StringWriter(projectBuilder);
                this.workflowStudioControl.Project.Write(projectWriter);

                // Save the project policy right after the project model
                policyBuilder = new StringBuilder();
                policyWriter = new StringWriter(policyBuilder);
                this.workflowStudioControl.Project.Policy.Write(policyWriter);

                WorkflowModelServiceStub webService = new WorkflowModelServiceStub();

                DateTime modifiedTime = webService.SaveProject(ConnectionStringBuilder.Instance.Create(this.workflowStudioControl.Project.ModifiedTime),
                    this.workflowStudioControl.Project.Name,
                    this.workflowStudioControl.Project.Version,
                    projectBuilder.ToString(), policyBuilder.ToString());

                // saving worklfow data to the database through a database storage provider
                DatabaseStorageProvider storageProvider = new DatabaseStorageProvider(webService, this.workflowStudioControl.Project);
                foreach (WorkflowModel workflowModel in this.workflowStudioControl.Project.Workflows)
                {
                    // currently, we have no way to find out whether data in the workflow
                    // model has been changed or not, therefore, we just save the workflow
                    // data anyway.
                    storageProvider.WorkflowModel = workflowModel;
                    workflowModel.Save(storageProvider);
                }

                // read the project model from database since it contains identifiers assigned
                // to the workflows
                string xmlString = webService.GetProject(ConnectionStringBuilder.Instance.Create(), this.workflowStudioControl.Project.Name, this.workflowStudioControl.Project.Version);
                ProjectModel project = new ProjectModel(this.workflowStudioControl.Project.Name);
                StringReader reader = new StringReader(xmlString);
                project.Read(reader);

                // set the ids to the current project and workflows
                this.workflowStudioControl.Project.ID = project.ID;
                this.workflowStudioControl.Project.ModifiedTime = modifiedTime; // update the modified time
                foreach (WorkflowModel workflowModel in this.workflowStudioControl.Project.Workflows)
                {
                    if (workflowModel.ID == null || workflowModel.ID.Length == 0)
                    {
                        workflowModel.ID = project.Workflows[workflowModel.Name].ID;
                    }
                }

                // update the project tree node's tooltips
                this.workflowStudioControl.UpdateProjectTree();

                // read project access control policy
                string policyString = webService.GetProjectPolicy(ConnectionStringBuilder.Instance.Create(),
                    project.Name, project.Version);
                reader = new StringReader(policyString);
                project.Policy.Read(reader);

                this.workflowStudioControl.Project.IsLoadedFromDB = true;

                this.workflowStudioControl.Project.NeedToSave = false;

                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.ProjectSaved"),
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = this.Cursor;

                // display a text in the status bar
                this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.Ready"));
            }
        }

        /// <summary>
        /// Increase the project version by one
        /// </summary>
        private void IncreaseProjectVersion()
        {
            string version = this.workflowStudioControl.Project.Version;
            int pos = version.IndexOf('.');
            int majorNumber = int.Parse(version.Substring(0, pos));
            majorNumber++; // increase by one
            this.workflowStudioControl.Project.Version = majorNumber + ".0";
        }

        /// <summary>
        /// Get the dba role of the project of current version
        /// </summary>
        private string GetDBARoleFromProject()
        {
            string projectName = this.workflowStudioControl.Project.Name;
            string projectVersion = this.workflowStudioControl.Project.Version;

            WorkflowModelServiceStub webService = new WorkflowModelServiceStub();
            return webService.GetDBARole(ConnectionStringBuilder.Instance.Create(),
                projectName,
                projectVersion);
        }

        /// <summary>
        /// Set the dba role to the project of current version
        /// </summary>
        private void SetDBARoleToProject(string dbaRole)
        {
            string projectName = this.workflowStudioControl.Project.Name;
            string projectVersion = this.workflowStudioControl.Project.Version;

            WorkflowModelServiceStub webService = new WorkflowModelServiceStub();
            webService.SetDBARole(ConnectionStringBuilder.Instance.Create(),
                projectName,
                projectVersion,
                dbaRole);
        }

        /// <summary>
        /// Check if any of workflow models currently have instances running?
        /// </summary>
        /// <returns>true if it has instance running, false otherwise.</returns>
        private bool HasRunningWorkflowInstances()
        {
            bool status = false;

            foreach (WorkflowModel workflowModel in this.workflowStudioControl.Project.Workflows)
            {
                if (workflowModel.ID != null && workflowModel.IsAltered)
                {
                    // it is a saved workflow, check if there are any running instances
                    WorkflowModelServiceStub webService = new WorkflowModelServiceStub();

                    status = webService.HasRunningInstances(ConnectionStringBuilder.Instance.Create(),
                        this.workflowStudioControl.Project.Name,
                        this.workflowStudioControl.Project.Version,
                        workflowModel.ID);

                    if (status)
                    {
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Check if any of workflow models currently have instances running?
        /// </summary>
        /// <returns>true if it has instance running, false otherwise.</returns>
        private bool DoesExistingProjectHaveRunningWorkflowInstances()
        {
            bool status = false;
            WorkflowModelServiceStub webService;

            foreach (WorkflowModel workflowModel in this.workflowStudioControl.Project.Workflows)
            {
                if (!string.IsNullOrEmpty(workflowModel.ID))
                {
                    webService = new WorkflowModelServiceStub();

                    status = webService.HasRunningInstances(ConnectionStringBuilder.Instance.Create(),
                        this.workflowStudioControl.Project.Name,
                        this.workflowStudioControl.Project.Version,
                        workflowModel.ID);

                    if (status)
                    {
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Open a project in database
        /// </summary>
        private void OpenProjectInDatabase()
        {
            OpenDatabaseDialog openDialog = new OpenDatabaseDialog();

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                this._isUserAuthenticated = true;
                this.IsDBAUser = openDialog.IsDBAUser;
                _projectFileName = null;

                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    string projectName = openDialog.ProjectName;
                    string projectVersion = openDialog.ProjectVersion;

                    LoadProjectFromDatabase(projectName, projectVersion,
                        openDialog.IsLockObtained, openDialog.IsLatestVersion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor.Current = this.Cursor;
                }
            }
        }

        /// <summary>
        /// backup a project from the database
        /// </summary>
        private void BackupProjectFromDatabase()
        {
            SelectBackupProjectDialog selectDialog = new SelectBackupProjectDialog();

            if (selectDialog.ShowDialog() == DialogResult.OK)
            {
                string projectName = selectDialog.ProjectName;
                string projectVersion = selectDialog.ProjectVersion;

                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                // display a text in the status bar
                this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.BackupProject"));

                try
                {
                    string fileName = null;
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.InitialDirectory = "c:\\";
                    saveFileDialog.Filter = "Backup files (*.wfb)|*.wfb";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = false;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = saveFileDialog.FileName;

                        // pack the data of the selected schema into a zip file
                        string dataDir = GetToolTempDir() + WorkflowStudioApp.DATA_DIR;

                        _backup = new Backup(fileName, projectName, projectVersion, dataDir);
                        _backup.WorkingDialog = _workInProgressDialog;

                        _isRequestComplete = false;

                        // backup is a lengthy job, therefore, run the task
                        // on a worker thread so that UI thread won't be blocked.
                        _backupMethod = new MethodInvoker(RunBackupJob);

                        _backupMethod.BeginInvoke(new AsyncCallback(RunBackupJobDone), null);

                        // Show the status
                        this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.BackupProject"));

                        // launch a work in progress dialog
                        ShowWorkingDialog(true, new MethodInvoker(CancelBackupJob));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor.Current = this.Cursor;
                }
            }
        }

        /// <summary>
        /// Get the temp dir for saving tempory files
        /// </summary>
        /// <returns></returns>
        private string GetToolTempDir()
        {
            string tempToolDir = NewteraNameSpace.GetAppToolDir();

            tempToolDir += @"temp\";

            if (!Directory.Exists(tempToolDir))
            {
                Directory.CreateDirectory(tempToolDir);
            }

            return tempToolDir;
        }

        /// <summary>
        /// Run the backup job in a worker thread
        /// </summary>
        private void RunBackupJob()
        {
            if (_backup != null)
            {
                _backup.Pack();
            }
        }

        /// <summary>
        /// Cancel the backup job in a worker thread
        /// </summary>
        private void CancelBackupJob()
        {
            if (_backup != null)
            {
                _backup.IsCancelled = true;
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for RunBackupJob method.
        /// </summary>
        /// <param name="res">The result</param>
        private void RunBackupJobDone(IAsyncResult res)
        {
            try
            {
                this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.Ready"));

                _backupMethod.EndInvoke(res);

                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.BackupComplete"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
            }
        }

        /// <summary>
        /// Restore project from a backup file
        /// </summary>
        private void RestoreProjectFromFile()
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                // only system administrator is allowed to restore a project from a file
                AdminLoginDialog dialog = new AdminLoginDialog();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = null;
                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "Backup files (*.wfb)|*.wfb";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = false;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = openFileDialog.FileName;

                        string dataDir = GetToolTempDir() + WorkflowStudioApp.DATA_DIR;

                        _restore = new Restore(fileName, dataDir);
                        _restore.ConfirmCallback = new MethodInvoker(ConfirmOverrideProject);
                        _restore.WorkingDialog = this._workInProgressDialog;

                        _isRequestComplete = false;

                        // unpack and import database data is a lengthy job, therefore, run the task
                        // on a worker thread so that UI thread won't be blocked.
                        _restoreMethod = new MethodInvoker(RunRestoreJob);

                        _restoreMethod.BeginInvoke(new AsyncCallback(RunRestoreJobDone), null);

                        // Show the status
                        this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.RestoreData"));

                        // launch a work in progress dialog
                        ShowWorkingDialog(false, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// The AsyncCallback event handler for RunRestoreJob method.
        /// </summary>
        /// <param name="res">The result</param>
        private void RunRestoreJobDone(IAsyncResult res)
        {
            try
            {
                _restoreMethod.EndInvoke(res);

                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.RestoreComplete"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
            }
        }

        /// <summary>
        /// Run the restore database job in a worker thread
        /// </summary>
        private void RunRestoreJob()
        {
            if (_restore != null)
            {
                _restore.Perform();
            }
        }

        /// <summary>
        /// Confirm with user whether to override an existing project
        /// </summary>
        internal void ConfirmOverrideProject()
        {
            if (MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.OverrideProject"),
                "Confirm Dialog", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _restore.IsOverride = true;
            }
            else
            {
                _restore.IsOverride = false;
            }
        }


        /// <summary>
        /// Delete a project from the database
        /// </summary>
        private void DeleteProjectFromDatabase()
        {
            // only system administrator is allowed to delete a project from database
            AdminLoginDialog dialog = new AdminLoginDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DeleteProjectDialog deleteDialog = new DeleteProjectDialog();

                if (deleteDialog.ShowDialog() == DialogResult.OK)
                {
                    string projectName = deleteDialog.ProjectName;
                    string projectVersion = deleteDialog.ProjectVersion;

                    // Change the cursor to indicate that we are waiting
                    Cursor.Current = Cursors.WaitCursor;

                    // display a text in the status bar
                    this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.DeletingProject"));

                    try
                    {
                        WorkflowModelServiceStub webService = new WorkflowModelServiceStub();

                        webService.DeleteProject(ConnectionStringBuilder.Instance.Create(),
                            projectName, projectVersion);

                        MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.ProjectDeleted"), "Info",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor.Current = this.Cursor;

                        // display a text in the status bar
                        this.ShowWorkingStatus(MessageResourceManager.GetString("WorkflowStudioApp.Ready"));
                    }
                }
            }
        }

        /// <summary>
        /// Load the specified project from databasde
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="projectVersion">The project version</param>
        /// <param name="IsLockObtained">Is the lock of the project obtained</param>
        /// <param name="isLatestVersion">Is the project the latest version</param>
        private void LoadProjectFromDatabase(string projectName, string projectVersion, bool isLockObtained, bool isLatestVersion)
        {
            WorkflowModelServiceStub service = new WorkflowModelServiceStub();
            string modelString = service.GetProject(ConnectionStringBuilder.Instance.Create(),
                projectName, projectVersion);

            StringReader reader = new StringReader(modelString);
            ProjectModel project = new ProjectModel("");
            project.Read(reader);

            // read project access control policy
            string policyString = service.GetProjectPolicy(ConnectionStringBuilder.Instance.Create(),
                projectName, projectVersion);
            reader = new StringReader(policyString);
            project.Policy.Read(reader);

            if (PermissionChecker.Instance.HasPermission(project.Policy, project, XaclActionType.Read))
            {
                // Create a DatabaseStorageProvider to each of WorkflowModel so that
                // the various data of a workflow can be loaded from database on demand
                DatabaseStorageProvider storageProvider;
                foreach (WorkflowModel workflowModel in project.Workflows)
                {
                    storageProvider = new DatabaseStorageProvider(service, project);
                    storageProvider.WorkflowModel = workflowModel;
                    workflowModel.SourceStorageType = StorageType.Database;
                    workflowModel.DatabaseStorageProvider = storageProvider;
                }

                this.workflowStudioControl.OpenProject(project);

                project.IsLoadedFromDB = true;

                InitializeMenuItemStates(); // set menu item states

                if (isLockObtained)
                {
                    project.IsLockObtained = true;
                    _menuItemStates.SetState(MenuItemID.FileSaveDatabase, true);
                    _menuItemStates.SetState(MenuItemID.ToolLock, false);
                    _menuItemStates.SetState(MenuItemID.ToolUnlock, true);
                }
                else
                {
                    project.IsLockObtained = false;
                    _menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
                    _menuItemStates.SetState(MenuItemID.ToolLock, true);
                    _menuItemStates.SetState(MenuItemID.ToolUnlock, true);
                }

                if (!isLatestVersion)
                {
                    _menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
                    _menuItemStates.SetState(MenuItemID.ToolLock, false);
                    _menuItemStates.SetState(MenuItemID.ToolUnlock, false);
                }
            }
            else
            {
                if (isLockObtained)
                {
                    // unlock the project model on the server
                    try
                    {
                        service.UnlockProject(project.Name, project.Version, ConnectionStringBuilder.Instance.Create(), false);
                    }
                    catch (Exception)
                    {
                    }
                }

                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.NoProjectPermission"));
            }
        }

        /// <summary>
        /// Obtain a lock to the project
        /// </summary>
        private void ObtainProjectLock()
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                WorkflowModelServiceStub webService = new WorkflowModelServiceStub();
                webService.LockProject(this.workflowStudioControl.Project.Name,
                    this.workflowStudioControl.Project.Version,
                    ConnectionStringBuilder.Instance.Create(this.workflowStudioControl.Project.ModifiedTime));

                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.LockObtained"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // obtain a lock successfully
                this.workflowStudioControl.Project.IsLockObtained = true;
                _menuItemStates.SetState(MenuItemID.FileSaveDatabase, true);
                _menuItemStates.SetState(MenuItemID.ToolLock, false);
                _menuItemStates.SetState(MenuItemID.ToolUnlock, true);
            }
            catch (Exception ex)
            {
                // locking failed
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.workflowStudioControl.Project.IsLockObtained = false;
                _menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
                _menuItemStates.SetState(MenuItemID.ToolLock, true);
                _menuItemStates.SetState(MenuItemID.ToolUnlock, false);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// Release the project lock
        /// </summary>
        private void ReleaseProjectLock()
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                WorkflowModelServiceStub webService = new WorkflowModelServiceStub();
                webService.UnlockProject(this.workflowStudioControl.Project.Name,
                    this.workflowStudioControl.Project.Version,
                    ConnectionStringBuilder.Instance.Create(), true);

                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.LockReleased"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // release the lock successfully
                this.workflowStudioControl.Project.IsLockObtained = false;
                _menuItemStates.SetState(MenuItemID.FileSaveDatabase, false);
                _menuItemStates.SetState(MenuItemID.ToolLock, true);
                _menuItemStates.SetState(MenuItemID.ToolUnlock, false);
            }
            catch (Exception ex)
            {
                // locking failed
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the server's license is an
        /// evaluation license.
        /// </summary>
        /// <returns>True if it is an evaluation license, false if it is a permenant license.</returns>
        private bool IsInEvaluation()
        {
            bool isEvaluation = true;

            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                AdminServiceStub adminService = new AdminServiceStub();

                isEvaluation = adminService.IsEvaluationLicense();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }

            return isEvaluation;
        }

        /// <summary>
        /// Set access control policy to the project
        /// </summary>
        private void SetAccessControl()
        {
            AccessControlDialog dialog = new AccessControlDialog();

            dialog.Project = this.workflowStudioControl.Project;
            dialog.Policy = this.workflowStudioControl.Project.Policy;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.workflowStudioControl.Project.Policy = dialog.Policy;
                this.workflowStudioControl.Project.IsAltered = true;
                this.workflowStudioControl.Project.NeedToSave = true;
            }
        }

        /// <summary>
        /// Disconnect the server
        /// </summary>
        private void DisconnectServer()
        {
            if (this.workflowStudioControl != null &&
                this.workflowStudioControl.Project != null &&
                this.workflowStudioControl.Project.IsLockObtained)
            {
                // unlock the project on the server
                try
                {
                    WorkflowModelServiceStub webService = new WorkflowModelServiceStub();
                    webService.UnlockProject(this.workflowStudioControl.Project.Name,
                        this.workflowStudioControl.Project.Version,
                        ConnectionStringBuilder.Instance.Create(), false);
                }
                catch (Exception)
                {
                }
            }
        }

        #region event handlers

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectToFile(); 
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.CreateNewProject();

            InitializeMenuItemStates(); // set menu item states
        }

        private void SaveAsMenuItem_Click(object sender, EventArgs e)
        {
            _projectFileName = GetSaveFileName();

            SaveProjectToFile();
        }

        private void PageSetupMenuItem_Click(object sender, EventArgs e)
        {
            WorkflowView workflowView = this.workflowStudioControl.WorkflowView;
            if (workflowView != null)
            {
                WorkflowPageSetupDialog pageSetupDialog = new WorkflowPageSetupDialog(this.workflowStudioControl.WorkflowView as IServiceProvider);
                if (DialogResult.OK == pageSetupDialog.ShowDialog())
                    workflowView.PerformLayout();
            }
        }

        private void PrintPreviewMenuItem_Click(object sender, EventArgs e)
        {
            WorkflowView workflowView = this.workflowStudioControl.WorkflowView;
            if (workflowView != null)
            {
                this.workflowStudioControl.SuspendLayout();
                workflowView.PrintPreviewMode = !workflowView.PrintPreviewMode;
                PrintPreviewMenuItem.Checked = workflowView.PrintPreviewMode;
                workflowView.ClientSize = this.workflowStudioControl.ClientSize;
                this.workflowStudioControl.ResumeLayout(true);
            }
        }
        
        private void PrintMenuItem_Click(object sender, EventArgs e)
        {
            WorkflowView workflowView = this.workflowStudioControl.WorkflowView;
            if (null != workflowView)
            {
                //select printer
                PrintDialog printDialog = new System.Windows.Forms.PrintDialog();
                printDialog.AllowPrintToFile = false;
                printDialog.Document = workflowView.PrintDocument;

                try
                {
                    printDialog.ShowDialog();
                }
                catch
                {
                    string errorString = "Error selecting new printer";
                    MessageBox.Show(this, errorString, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(StandardCommands.Cut);
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(StandardCommands.Copy);
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(StandardCommands.Paste);
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(StandardCommands.Delete);
        }

        private void zoom400MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Zoom400Mode);
        }

        private void zoom300MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Zoom300Mode);
        }

        private void zoom200MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Zoom200Mode);
        }

        private void zoom150MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Zoom150Mode);
        }

        private void zoom100MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Zoom100Mode);
        }

        private void zoom75MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Zoom75Mode);
        }

        private void zoom50MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Zoom50Mode);
        }

        private void zoom10MenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.WorkflowView.Zoom = 10;
        }

        private void zoomShowAllMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.ShowAll);
        }

        private void zoomInNavigationMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.ZoomIn);
            this.zoomInToolStripButton.Checked = true;
        }

        private void zoomOutNavigationMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.ZoomOut);
            this.zoomOutToolStripButton.Checked = true;
        }

        private void panNavigationMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Pan);
        }

        private void defaultNavigationMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.DefaultFilter);
        }

        private void expandMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Expand);
        }

        private void collapseMenuItem_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.InvokeStandardCommand(WorkflowMenuCommands.Collapse);
        }

        
        //zoom level combo box handling
        private bool zoomLevelDirty = false;

        private void zoomLevelToolStripButton_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParseZoomLevelValue();
        }

        private void zoomLevelToolStripButton_Leave(object sender, EventArgs e)
        {
            if (this.zoomLevelDirty)
            {
                ParseZoomLevelValue();
                this.zoomLevelDirty = false;
            }
        }

        private void ParseZoomLevelValue()
        {
            //parse the value
            string newZoom = zoomLevelToolStripButton.Text.Trim();
            if (newZoom.EndsWith("%"))
                newZoom = newZoom.Substring(0, newZoom.Length - 1);

            if (newZoom.Length > 0)
            {
                string errorMessage = null;

                try
                {
                    this.workflowStudioControl.WorkflowView.Zoom = Convert.ToInt32(newZoom);
                }
                catch (FormatException)
                {
                    errorMessage = "Invalid Zoom Measurement";
                }
                catch
                {
                    errorMessage = "Invalid Zoom Range";
                }

                if (errorMessage != null)
                    MessageBox.Show(this, errorMessage);
            }
        }

        private void zoomLevelToolStripButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Escape)
            {
                zoomLevelDirty = true;
            }
        }

        private void zoomLevelToolStripButton_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ParseZoomLevelValue();
                zoomLevelDirty = false;
                this.workflowStudioControl.WorkflowView.Focus();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                //revert the changes back to the zoom level from the workflow
                zoomLevelToolStripButton.Text = this.workflowStudioControl.WorkflowView.Zoom + "%";
                zoomLevelDirty = false;
                this.workflowStudioControl.WorkflowView.Focus();
            }
            else
            {
                zoomLevelDirty = true;
            }
        }

        private void ValidateWorkflows()
        {
            ValidationErrorCollection errors = this.workflowStudioControl.ValidateWorkflows();
            if (errors != null)
            {
                ValidationErrorsDialog dialog = new ValidationErrorsDialog(errors);
                dialog.Show();
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.WorkflowValidated"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AddMenuItem_Click(object sender, EventArgs e)
        {
            AddWorkflowModel();
        }

        private void workflowStudioControl_AddWorkflowEvent(object sender, EventArgs e)
        {
            AddWorkflowModel();
        }

        private void OpenFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "workflow project files (*.wfproj)|*.wfproj|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.workflowStudioControl.Dock = DockStyle.Fill;

                ToolboxService toolbox = ((IServiceProvider)this.workflowStudioControl).GetService(typeof(IToolboxService)) as ToolboxService;
                if (toolbox != null)
                {
                    toolbox.BorderStyle = BorderStyle.FixedSingle;
                    toolbox.Dock = DockStyle.Fill;
                    this.toolBoxSplitContainer.Panel1.Controls.Add(toolbox);
                }

                this._projectFileName = openFileDialog.FileName;

                // create the project
                this.workflowStudioControl.OpenProject(_projectFileName);

                InitializeMenuItemStates(); // set menu item states
            }
        }

        private void OpenDatabaseMenuItem_Click(object sender, EventArgs e)
        {
            OpenProjectInDatabase();
        }

        private void ProjectMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowTabView(NavigationTabView.Project, false);
        }

        private void ActivitiesMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowTabView(NavigationTabView.Activities, false);
        }

        private void PropertiesMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowTabView(NavigationTabView.Properties, false);
        }

        private void workflowStudioControl_TabViewChanged(object sender, EventArgs e)
        {
            TabViewChangedEventArgs args = e as TabViewChangedEventArgs;
            if (args != null)
            {
                this.ShowTabView(args.ViewType, true);
            }
        }

        private void SaveToDatabaseMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectToDatabase();
        }

        private void SaveToDatabaseAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectToDatabaseAs();
        }

        private void openDatabaseToolStripButton1_Click(object sender, EventArgs e)
        {
            OpenProjectInDatabase();
        }

        private void saveDataBaseToolStripButton_Click(object sender, EventArgs e)
        {
            SaveProjectToDatabase();
        }

        private void SetupServerURLMenuItem_Click(object sender, EventArgs e)
        {
            SetupServerURLDialog dialog = new SetupServerURLDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.CorrectURL"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LockMenuItem_Click(object sender, EventArgs e)
        {
            ObtainProjectLock();
        }

        private void UnlockMenuItem_Click(object sender, EventArgs e)
        {
            ReleaseProjectLock();
        }

        private void UserGuideMenuItem_Click(object sender, EventArgs e)
        {
            string helpFile = AppDomain.CurrentDomain.BaseDirectory + MessageResourceManager.GetString("WorkflowStudioApp.OnlineHelp");
            Help.ShowHelp(this, helpFile);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog();

            aboutDialog.ShowDialog();
        }

        private void DeleteProjectMenuItem_Click(object sender, EventArgs e)
        {
            DeleteProjectFromDatabase();
        }

        private void WorkflowStudioApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.ProjectChanged"),
                "Confirm Dialog", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // unlock the project if it was locked
                this.DisconnectServer();
            }
            else
            {
                e.Cancel = true;
            }

            return;
        }

        private void monitorToolStripButton_Click(object sender, EventArgs e)
        {
            MonitorWorkflows();
        }

        private void WorkflowMonitorMenuItem_Click(object sender, EventArgs e)
        {
            MonitorWorkflows();
        }

        private void workflowStudioControl_MonitorWorkflowsEvent(object sender, EventArgs e)
        {
            MonitorWorkflows();
        }

        private void workflowStudioControl_StartWorkflowEvent(object sender, EventArgs e)
        {
            StartWorkflow();
        }

        private void startWorkflowMenuItem_Click(object sender, EventArgs e)
        {
            StartWorkflow();
        }

        private void validateWorkflowsToolStripButton_Click(object sender, EventArgs e)
        {
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                ValidateWorkflows();
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }
        }

        private void validateWorkflowsMenuItem_Click(object sender, EventArgs e)
        {
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                ValidateWorkflows();
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }
        }

        private void assignProjectAdminMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsInEvaluation())
            {
                AdminLoginDialog loginDialog = new AdminLoginDialog();

                if (loginDialog.ShowDialog() == DialogResult.OK)
                {
                    ProtectProjectDialog dialog = new ProtectProjectDialog();

                    dialog.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudio.DisabledFeature"),
                    "Error Dialog", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void serverLogMenuItem_Click(object sender, EventArgs e)
        {
            ServerErrorDialog dialog = new ServerErrorDialog();
            dialog.ShowDialog();
        }

        private void accessControlMenuItem_Click(object sender, EventArgs e)
        {
            SetAccessControl();
        }

        private void workflowStudioControl_SetAccessControlEvent(object sender, EventArgs e)
        {
            SetAccessControl();
        }

        private void addToolStripButton_Click(object sender, EventArgs e)
        {
            AddWorkflowModel();
        }

        private void deleteToolStripButton_Click(object sender, EventArgs e)
        {
            this.workflowStudioControl.DeleteWorkflowModel();
        }

        private void WorkflowStudioApp_Load(object sender, EventArgs e)
        {
             // register the menu item state change deleget
            this.workflowStudioControl.MenuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);
        }

        private void traceLogMenuItem_Click(object sender, EventArgs e)
        {
            ServerTraceLogDialog dialog = new ServerTraceLogDialog();
            dialog.ShowDialog();
        }

        private void AssignTaskSubstituteMenuItem_Click(object sender, EventArgs e)
        {
            AdminLoginDialog loginDialog = new AdminLoginDialog();

            if (loginDialog.ShowDialog() == DialogResult.OK)
            {
                SetTaskSubstituteDialog dialog = new SetTaskSubstituteDialog();

                dialog.ShowDialog();
            }
        }

        private void backupProjectMenuItem_Click(object sender, EventArgs e)
        {
            BackupProjectFromDatabase();
        }

        private void restoreProjectMenuItem_Click(object sender, EventArgs e)
        {
            RestoreProjectFromFile();
        }

        private void MonitorDialog_SubWorkflowInstanceViewed(object sender, Newtera.WorkflowMonitor.SubWorkflowInstanceViewedEventArgs e)
        {
            MonitorWorkflowDialog monitorDialog = new MonitorWorkflowDialog();
            WorkflowModel workflowModel = (WorkflowModel)this.workflowStudioControl.Project.Workflows[e.WorkflowModelName];
            if (workflowModel != null)
            {
                monitorDialog.WorkflowID = workflowModel.ID;
                monitorDialog.WorkflowModel = workflowModel;
                monitorDialog.WorkflowInstanceId = new Guid(e.WorkflowInstanceId);
                monitorDialog.SubWorkflowInstanceViewed += new EventHandler<SubWorkflowInstanceViewedEventArgs>(MonitorDialog_SubWorkflowInstanceViewed);
                monitorDialog.Show();
            }
        }

        #endregion
    }
}
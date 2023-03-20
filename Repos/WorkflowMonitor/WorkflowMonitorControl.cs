using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Xml;
using System.Threading;
using System.Windows.Forms;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using System.Workflow.ComponentModel;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.WFModel;
using Newtera.WindowsControl;
using Newtera.Activities;
using Newtera.Common.MetaData.Principal;
using Newtera.WinClientCommon;

namespace Newtera.WorkflowMonitor
{
    public partial class WorkflowMonitorControl : UserControl
    {
        private const int PageSize = 25;

        public event EventHandler ActivitySelected;

        int _pageIndex = 0;
        NewteraTrackingWorkflowInstanceCollection _displayedWorkflows = null;
        string _workflowTypeId = string.Empty;
        WorkflowModel _workflowModel = null;
        Guid _currentWorkflowInstanceId = Guid.Empty;

        DateTime _statusFromDateTime = DateTime.Now;
        DateTime _statusUntilDateTime = DateTime.Now.AddDays(1);
        WorkflowStatus _status = WorkflowStatus.Running;
        string _workflowInstanceIdToFind = null;

        bool _useCondition = false;

        private ApplicationSettings _monitorSettings;
        private ITrackingQueryService _trackingQueryService;
        private IActivityValidateService _activityValidateService;
        private IDataQueryService _dataQueryService;

        private Dictionary<string, WorkflowStatusInfo> _workflowStatusList = null;
        private Dictionary<string, ActivityStatusInfo> _activityStatusListValue = null;

        private ViewHost _workflowViewHost;

        #region public methods

        public WorkflowMonitorControl()
        {
            InitializeComponent();

            _workflowViewHost = new ViewHost(this);
            _workflowViewHost.ZoomChanged += new EventHandler<ViewHost.ZoomChangedEventArgs>(WorkflowViewHost_ZoomChanged);
            _workflowViewHost.ActivitySelected += new EventHandler(workflowViewHost_ActivitySelected);

            this.monitorSurface.Panel2.SuspendLayout();
            this.monitorSurface.Panel2.Controls.Clear();
            this.monitorSurface.Panel2.Controls.Add(_workflowViewHost);
            this.monitorSurface.Panel2.ResumeLayout(true);

            this._monitorSettings = new ApplicationSettings();
            this._workflowStatusList = new Dictionary<string, WorkflowStatusInfo>();
            this._activityStatusListValue = new Dictionary<string, ActivityStatusInfo>();

            _monitorSettings.PollingInterval = 5000;
            _monitorSettings.AutoSelectLatest = false;
        }

        /// <summary>
        /// Gets or sets the the id of the workflow to monitor
        /// </summary>
        public string WorkflowID
        {
            get
            {
                return _workflowTypeId;
            }
            set
            {
                _workflowTypeId = value;
            }
        }

        /// <summary>
        /// Gets or sets the workflow model
        /// </summary>
        public WorkflowModel WorkflowModel
        {
            get
            {
                return _workflowModel;
            }
            set
            {
                _workflowModel = value;
            }
        }

        /// <summary>
        /// Gets the workflow instance id which is currently displayed
        /// </summary>
        public Guid CurrentWorkflowInstanceId
        {
            get
            {
                return _currentWorkflowInstanceId;
            }
            set
            {
                _currentWorkflowInstanceId = value;
            }
        }

        /// <summary>
        /// Gets the monitor settings
        /// </summary>
        public ApplicationSettings MonitorSettings
        {
            get
            {
                return _monitorSettings;
            }
        }

        /// <summary>
        /// Gets or sets the tracking query serviec to get tracking data
        /// </summary>
        public ITrackingQueryService TrackingQueryService
        {
            get
            {
                return _trackingQueryService;
            }
            set
            {
                _trackingQueryService = value;
            }
        }

        /// <summary>
        /// Gets or sets the activity validate service from which to get meta-data
        /// </summary>
        public IActivityValidateService ActivityValidateService
        {
            get
            {
                return _activityValidateService;
            }
            set
            {
                _activityValidateService = value;
            }
        }

        /// <summary>
        /// Gets or sets the service that provides data related query.
        /// </summary>
        public IDataQueryService DataQueryService
        {
            get
            {
                return _dataQueryService;
            }
            set
            {
                _dataQueryService = value;
            }
        }

        /// <summary>
        /// Gets or sets the from date for query status
        /// </summary>
        public DateTime FromDate
        {
            get
            {
                return _statusFromDateTime;
            }
            set
            {
                _statusFromDateTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the to date for query status
        /// </summary>
        public DateTime ToDate
        {
            get
            {
                return _statusUntilDateTime;
            }
            set
            {
                _statusUntilDateTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the to status as query condition
        /// </summary>
        public WorkflowStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        /// <summary>
        /// Gets or sets the to workflow instance id to find
        /// </summary>
        public string WorkflowInstanceIdToFind
        {
            get
            {
                return _workflowInstanceIdToFind;
            }
            set
            {
                _workflowInstanceIdToFind = value;
            }
        }

        /// <summary>
        /// Get the selected activity that has been completed or executing
        /// </summary>
        public Activity SelectedActivity
        {
            get
            {
                Activity selected = _workflowViewHost.GetHighlightActivity();
                bool found = false;
                if (selected != null)
                {
                    // make sure the selected activity has been completed or is executing
                    for (int i = 0; i < activitiesListView.Items.Count; i++)
                    {
                        if (selected.Name == activitiesListView.Items[i].SubItems[1].Text)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        selected = null;
                    }
                }

                return selected;
            }
        }

        public void DisplayWorkflows()
        {
            if (_useCondition && !string.IsNullOrEmpty(_workflowInstanceIdToFind))
            {
                Guid workflowInstanceGuid = new Guid(_workflowInstanceIdToFind.Trim());
                DisplayWorkflows(Enum.GetName(typeof(WorkflowStatus), _status), workflowInstanceGuid, _statusFromDateTime, _statusUntilDateTime);
            }
            else if (_currentWorkflowInstanceId != null && _currentWorkflowInstanceId != Guid.Empty)
            {
                DisplayWorkflows(Enum.GetName(typeof(WorkflowStatus), _status), _currentWorkflowInstanceId, _statusFromDateTime, _statusUntilDateTime);
            }
            else
            {
                DisplayWorkflows(Enum.GetName(typeof(WorkflowStatus), _status), Guid.Empty, _statusFromDateTime, _statusUntilDateTime);
            }
        }

        public void QueryWorkflows()
        {
            _useCondition = true;
            DisplayWorkflows();
        }

        //Turn the real-time monitoring on and off
        public void Monitor(bool toggle)
        {
            if (toggle)
            {
                timer.Interval = _monitorSettings.PollingInterval;
                timer.Start();

                if (MonitorSettings.AutoSelectLatest && (workflowsListView.Items.Count > 0))
                {
                    workflowsListView.Focus();
                    workflowsListView.Items[workflowsListView.Items.Count - 1].Selected = true;
                }
            }
            else
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// Expand or collapse the displayed workflow
        /// </summary>
        /// <param name="expand">true to expand, false to collapse</param>
        public void Expand(bool expand)
        {
            _workflowViewHost.Expand(expand);
        }

        /// <summary>
        /// Zoom the displayed workflow
        /// </summary>
        /// <param name="zoom"></param>
        public void Zoom(int zoom)
        {
            if (_workflowViewHost.WorkflowView == null)
            {
                return;
            }
            else
            {
                this._workflowViewHost.WorkflowView.Zoom = zoom;
            }
        }

        /// <summary>
        /// Cancel the current selected workflow that is awaiting an event
        /// </summary>
        public void CancelWorkflow()
        {
            if (workflowsListView.SelectedIndices.Count == 1)
            {
                NewteraTrackingWorkflowInstance trackingInstance = (NewteraTrackingWorkflowInstance)this._displayedWorkflows[workflowsListView.SelectedIndices[0]];
                this._trackingQueryService.CancelWorkflow(trackingInstance.WorkflowInstanceId);

                // Set the status as terminated
                WorkflowStatusInfo workflowStatus;
                if (_workflowStatusList.TryGetValue(trackingInstance.WorkflowInstanceId.ToString(), out workflowStatus))
                {
                    workflowStatus.Status = Enum.GetName(typeof(WorkflowStatus), WorkflowStatus.Terminated);
                    workflowStatus.WorkflowListViewItem.SubItems[2].Text = TextResourceManager.GetString(workflowStatus.Status);
                    WorkflowStatusInfo workflowStatusInfo = _workflowStatusList[(workflowsListView.SelectedItems[0].SubItems[0]).Text];
                    workflowStatusInfo.Status = workflowStatus.Status;
                    this.cancelToolStripMenuItem.Enabled = false;
                    this.closeActivityToolStripMenuItem.Enabled = false;
                    this.deleteToolStripMenuItem.Enabled = true;
                    this.currentTasksToolStripMenuItem.Enabled = false;
                    this.bindingDataInstanceToolStripMenuItem.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Cancel an activity of the current selected workflow that is awaiting an event
        /// </summary>
        public void CancelActivity()
        {
            if (workflowsListView.SelectedIndices.Count == 1)
            {
                NewteraTrackingWorkflowInstance trackingInstance = (NewteraTrackingWorkflowInstance)this._displayedWorkflows[workflowsListView.SelectedIndices[0]];

                CancelActivityDialog dialog = new CancelActivityDialog();
                dialog.RootActivity = (CompositeActivity)_workflowModel.RootActivity;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.SelectedActivityName != null)
                    {
                        // Change the cursor to indicate that we are waiting
                        Cursor.Current = Cursors.WaitCursor;

                        try
                        {
                            this._trackingQueryService.CancelActivity(trackingInstance.WorkflowInstanceId, dialog.SelectedActivityName);
                        }
                        finally
                        {
                            Cursor.Current = this.Cursor;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete a workflow instance tracking data
        /// </summary>
        public void DeleteTrackingWorkflowInstance()
        {
            if (workflowsListView.SelectedIndices.Count == 1)
            {
                int index = workflowsListView.SelectedIndices[0];
                NewteraTrackingWorkflowInstance trackingInstance = (NewteraTrackingWorkflowInstance)this._displayedWorkflows[index];
                Guid deleteWorkflowInstanceId = trackingInstance.WorkflowInstanceId;
                this._trackingQueryService.DeleteTrackingWorkflowInstance(deleteWorkflowInstanceId);

                if (_currentWorkflowInstanceId != Guid.Empty &&
                    _currentWorkflowInstanceId == deleteWorkflowInstanceId)
                {
                    _currentWorkflowInstanceId = Guid.Empty;
                }

                // refresh the workflow views
                DisplayWorkflows();
            }
        }

        /// <summary>
        /// Delete all completed or terminated workflow tracking instances
        /// </summary>
        public void DeleteAllCompletedOrTerminatedTrackingWorkflowInstances()
        {
            NewteraTrackingWorkflowInstance trackingInstance;
            for (int i = 0; i < workflowsListView.Items.Count; i++)
            {
                trackingInstance = (NewteraTrackingWorkflowInstance)this._displayedWorkflows[i];
                if (trackingInstance.Status == WorkflowStatus.Completed ||
                    trackingInstance.Status == WorkflowStatus.Terminated)
                {
                    this._trackingQueryService.DeleteTrackingWorkflowInstance(trackingInstance.WorkflowInstanceId);
                }
            }

            _currentWorkflowInstanceId = Guid.Empty;

            // refresh the workflow views
            DisplayWorkflows();
        }

        /// <summary>
        /// Display the current tasks assocated with the selected workflow instance
        /// </summary>
        public void ShowCurrentTasks()
        {
            if (workflowsListView.SelectedIndices.Count == 1)
            {
                int index = workflowsListView.SelectedIndices[0];
                NewteraTrackingWorkflowInstance trackingInstance = (NewteraTrackingWorkflowInstance)this._displayedWorkflows[index];
                
                TaskInfoCollection taskInfos = this._trackingQueryService.GetTaskInfos(trackingInstance.WorkflowInstanceId);

                if (taskInfos.Count > 0)
                {
                    CurrentTasksDialog dialog = new CurrentTasksDialog();
                    dialog.TaskInfos = taskInfos;

                    dialog.ShowDialog();
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("WorkflowMonitor.NoTask"),
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Display the data instance bound to the selected workflow instance
        /// </summary>
        public void ShowBindingInstance()
        {
            if (workflowsListView.SelectedIndices.Count == 1)
            {
                int index = workflowsListView.SelectedIndices[0];
                NewteraTrackingWorkflowInstance trackingInstance = (NewteraTrackingWorkflowInstance)this._displayedWorkflows[index];

                WorkflowInstanceBindingInfo bindingInfo = _trackingQueryService.GetBindingDataInstanceInfo(trackingInstance.WorkflowInstanceId);

                if (bindingInfo != null)
                {
                    // Get the meta-data first
                    MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(bindingInfo.SchemaId);
                    if (metaData == null)
                    {
                        // Change the cursor to indicate that we are waiting
                        Cursor.Current = Cursors.WaitCursor;

                        try
                        {
                            metaData = this._activityValidateService.GetMetaData(bindingInfo.SchemaId);
                            MetaDataStore.Instance.PutMetaData(metaData);
                        }
                        finally
                        {
                            Cursor.Current = this.Cursor;
                        }
                    }

                    // get the binding instance data
                    DataViewModel instanceDataView = metaData.GetDetailedDataView(bindingInfo.DataClassName);
                    string query = instanceDataView.GetInstanceQuery(bindingInfo.DataInstanceId);

                    XmlNode xmlNode = _dataQueryService.ExecuteQuery(CreateConnectionString(metaData.SchemaModel.SchemaInfo),
                        query);
                    DataSet ds = new DataSet();

                    XmlReader xmlReader = new XmlNodeReader(xmlNode);
                    ds.ReadXml(xmlReader);

                    // show the binding instance in a dialog
                    InstanceView instanceView = new InstanceView(instanceDataView, ds);
                    DataInstanceDialog dialog = new DataInstanceDialog();
                    dialog.InstanceView = instanceView;
                    dialog.ShowDialog();
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("WorkflowMonitor.NoBindingInfo"),
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion public methods

        #region internal methods

        internal Dictionary<string, ActivityStatusInfo> ActivityStatusList
        {
            get { return _activityStatusListValue; }
        }

        //F5 refresh
        internal void ManualRefresh(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                DisplayWorkflows();
            }
        }

        #endregion

        #region protected methods

        #endregion protected methods

        #region private methods

        /// <summary>
        /// Creates a connection string to access a database via web services
        /// </summary>
        /// <param name="schemaInfo">The schema info to be accessed.</param>
        /// <returns>A connection string</returns>
        public string CreateConnectionString(SchemaInfoElement schemaInfo)
        {
            StringBuilder builder = new StringBuilder();
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName;
            if (principal != null)
            {
                userName = principal.Identity.Name;
            }
            else
            {
                throw new Exception("The user has not been authenticated.");
            }

            builder.Append("SCHEMA_NAME=").Append(schemaInfo.Name).Append(";SCHEMA_VERSION=").Append(schemaInfo.Version);
            builder.Append(";USER_ID=").Append(userName);
            if (schemaInfo.ModifiedTime != null)
            {
                builder.Append(";TIMESTAMP=").Append(schemaInfo.ModifiedTime.ToString("s"));
            }

            return builder.ToString();
        }

        private void DisplayWorkflows(string selectedWorkflowEvent, Guid workflowInstanceId, DateTime statusFrom, DateTime statusUntil)
        {
            int worklfowCount = 0;

            //Try to get all of the workflows from the tracking database
            try
            {
                if ((null != workflowInstanceId) && (Guid.Empty != workflowInstanceId))
                {
                    if (_displayedWorkflows != null)
                    {
                        _displayedWorkflows.Clear();
                    }
                    else
                    {
                        _displayedWorkflows = new NewteraTrackingWorkflowInstanceCollection();
                    }

                    NewteraTrackingWorkflowInstance trackingWorkflowInstance = _trackingQueryService.GetWorkflow(workflowInstanceId);
                    _displayedWorkflows.Add(trackingWorkflowInstance);

                    worklfowCount = _displayedWorkflows.Count;

                }
                else
                {
                    worklfowCount = _trackingQueryService.GetWorkflowCount(_workflowTypeId,
                        selectedWorkflowEvent,
                        statusFrom,
                        statusUntil,
                        _useCondition);

                    _displayedWorkflows = _trackingQueryService.GetWorkflows(_workflowTypeId,
                        selectedWorkflowEvent,
                        statusFrom,
                        statusUntil,
                        _useCondition,
                        _pageIndex,
                        PageSize);
                }

                workflowsListView.Items.Clear();
                _workflowStatusList.Clear();

                // For every workflow instance create a new WorkflowStatusInfo object and store in the _workflowStatusList
                // Also populate the workflow ListView
                foreach (NewteraTrackingWorkflowInstance trackingWorkflowInstance in _displayedWorkflows)
                {
                    ListViewItem listViewItem = new ListViewItem(new string[] {
                        trackingWorkflowInstance.WorkflowInstanceId.ToString(),
                        trackingWorkflowInstance.Initialized.ToString(),
                        TextResourceManager.GetString(trackingWorkflowInstance.Status.ToString())}, -1);

                    workflowsListView.Items.Add(listViewItem);

                    _workflowStatusList.Add(trackingWorkflowInstance.WorkflowInstanceId.ToString(),
                            new WorkflowStatusInfo(
                                trackingWorkflowInstance.WorkflowInstanceId.ToString(),
                                trackingWorkflowInstance.Status.ToString(),
                                trackingWorkflowInstance.Initialized.ToString(),
                                trackingWorkflowInstance.WorkflowInstanceId,
                                listViewItem));
                }

                //If there is at least one workflow, populate the Activities list
                if (workflowsListView.Items.Count > 0)
                {
                    this.workflowsListView.Focus();
                    ListViewItem listItem = this.workflowsListView.Items[0];
                    listItem.Focused = true;
                    listItem.Selected = true;
                    UpdateActivities();
                }
                else
                {
                    // clear activity display
                    activitiesListView.Items.Clear();
                    _activityStatusListValue.Clear();
                }

                //Display number of workflow instances
                this.workflowsLabel.Text = String.Format(TextResourceManager.GetString("WorkflowLabel"), worklfowCount);
                ShowViewHost(true);

                SetPagerButtonStatus(worklfowCount);
            }
            //Clear all of the lists and reset the UI if there are errors
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _workflowStatusList.Clear();
                workflowsListView.Items.Clear();

                _activityStatusListValue.Clear();
                activitiesListView.Items.Clear();

                this.workflowsLabel.Text = String.Format(TextResourceManager.GetString("WorkflowLabel"), "0");
                ShowViewHost(false);
            }
        }

        private void SetPagerButtonStatus(int workflowCount)
        {
            if (_pageIndex == 0)
            {
                // it's the first page
                this.prevButton.Enabled = false;
            }
            else
            {
                this.prevButton.Enabled = true;
            }

            int numberOfPages = 0;
            if (workflowCount > 0)
            {
                numberOfPages = workflowCount / PageSize + 1;
            }

            if (_pageIndex < numberOfPages - 1)
            {
                this.nextButton.Enabled = true;
            }
            else
            {
                this.nextButton.Enabled = false;
            }
        }

        //Refresh info from the database (invoked by F5 refresh and polling timer)
        private void RefreshView()
        {
            try
            {
                //Get all of the workflow instances from the database and refresh the status if changed
                _displayedWorkflows = _trackingQueryService.GetWorkflows(_workflowTypeId,
                    Enum.GetName(typeof(WorkflowStatus), _status),
                    _statusFromDateTime,
                    _statusUntilDateTime,
                    _useCondition, _pageIndex, PageSize);

                foreach (NewteraTrackingWorkflowInstance trackingWorkflowInstance in _displayedWorkflows)
                {
                    WorkflowStatusInfo workflowStatus;
                    if (_workflowStatusList.TryGetValue(trackingWorkflowInstance.WorkflowInstanceId.ToString(), out workflowStatus))
                    {
                        workflowStatus.Status = trackingWorkflowInstance.Status.ToString();
                        workflowStatus.WorkflowListViewItem.SubItems[2].Text = TextResourceManager.GetString(trackingWorkflowInstance.Status.ToString());
                    }
                    else
                    {
                        ListViewItem listViewItem = new ListViewItem(new string[] {
                            trackingWorkflowInstance.WorkflowInstanceId.ToString(),
                            trackingWorkflowInstance.Initialized.ToString(),
                            TextResourceManager.GetString(trackingWorkflowInstance.Status.ToString())}, -1);
                        workflowsListView.Items.Add(listViewItem);
                        workflowsListView.Focus();

                        _workflowStatusList.Add(trackingWorkflowInstance.WorkflowInstanceId.ToString(),
                                new WorkflowStatusInfo(
                                trackingWorkflowInstance.WorkflowInstanceId.ToString(),
                                trackingWorkflowInstance.Status.ToString(),
                                trackingWorkflowInstance.Initialized.ToString(),
                                trackingWorkflowInstance.WorkflowInstanceId,
                                listViewItem));

                        if (MonitorSettings.AutoSelectLatest)
                            listViewItem.Selected = true;
                    }
                }

                if (_displayedWorkflows.Count > 0)
                {
                    this.workflowsLabel.Text = String.Format(TextResourceManager.GetString("WorkflowLabel"),  _displayedWorkflows.Count.ToString());
                    ShowViewHost(true);
                    //Update the activity view since the selection may have changed
                    UpdateActivities();
                }
                else
                {
                    this.workflowsLabel.Text = String.Format(TextResourceManager.GetString("WorkflowLabel"), "0");
                    ShowViewHost(false);
                    this.workflowsListView.Items.Clear();
                    this.activitiesListView.Items.Clear();
                }


            }
            //Show error dialog if anything bad happen - likely a database error
            catch (Exception ex)
            {
                Monitor(false);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _workflowStatusList.Clear();
                workflowsListView.Items.Clear();

                _activityStatusListValue.Clear();
                activitiesListView.Items.Clear();

                this.workflowsLabel.Text = String.Format(TextResourceManager.GetString("WorkflowLabel"), "0");
            }

            _workflowViewHost.Refresh();
        }

        //Query the database via the databaseService and update the activities ListView and activityStatusList
        // based on the currently selected workflow instance
        private void UpdateActivities()
        {
            if (workflowsListView.SelectedItems.Count == 0)
            {
                activitiesListView.Items.Clear();
                _activityStatusListValue.Clear();
                return;
            }

            ListViewItem currentWorkflow = workflowsListView.SelectedItems[0];
            if (currentWorkflow != null)
            {
                Guid workflowInstanceId = _workflowStatusList[(currentWorkflow.SubItems[0]).Text].InstanceId;

                NewteraTrackingWorkflowInstance trackingWorkflowInstance = _trackingQueryService.GetWorkflow(workflowInstanceId);
   
                activitiesListView.Items.Clear();
                _activityStatusListValue.Clear();

                //ActivityEvents list received contain all events for activities in orders in event order
                //Walking down-up on the ActivityEvents list and keeping the last entry

                for (int index = trackingWorkflowInstance.ActivityEvents.Count; index >= 1; index--)
                {
                    NewteraActivityTrackingRecord activityTrackingRecord = (NewteraActivityTrackingRecord)trackingWorkflowInstance.ActivityEvents[index - 1];
                    if (!_activityStatusListValue.ContainsKey(activityTrackingRecord.QualifiedName))
                    {
                        ActivityStatusInfo latestActivityStatus = new ActivityStatusInfo(activityTrackingRecord.QualifiedName, activityTrackingRecord.ExecutionStatus.ToString());
                        _activityStatusListValue.Add(activityTrackingRecord.QualifiedName, latestActivityStatus);

                        string[] activitiesListViewItems = new string[] {
                            activityTrackingRecord.EventOrder.ToString(),
                            activityTrackingRecord.QualifiedName, 
                            activityTrackingRecord.ExecutionStatus.ToString()};
                        ListViewItem li = new ListViewItem(activitiesListViewItems, -1);
                        activitiesListView.Items.Add(li);
                    }
                }
                _workflowViewHost.Refresh();
            }
        }

        //Get the workflow definition from the database and load viewhost
        private void ShowWorkflowDefinition(Guid workflowInstanceId)
        {
            if (_workflowModel != null)
            {
                _workflowViewHost.OpenWorkflow(_workflowModel.RootActivity);
            }
        }

        private void ShowViewHost(Boolean show)
        {
            this._workflowViewHost.Visible = show;
            if (_displayedWorkflows == null)
                this.workflowViewErrorTextLabel.Visible = false;
            else
                this.workflowViewErrorTextLabel.Visible = _displayedWorkflows.Count.Equals(0);
        }

        #endregion

        #region event handlers

        //Sort the workflow list if the user clicks the column headers
        private void OnWorkflowsColumnClick(object sender, ColumnClickEventArgs e)
        {
            //If the column is the id column then sort numeric
            if (e.Column == 0)
                this.workflowsListView.ListViewItemSorter = new ListViewItemComparer(e.Column, false);
            else
                this.workflowsListView.ListViewItemSorter = new ListViewItemComparer(e.Column, true);
        }

        //Sort the activities list if the user clicks the column headers
        private void OnActivitiesColumnClick(object sender, ColumnClickEventArgs e)
        {
            //If the column is the id column then sort numeric
            if (e.Column == 0)
                this.activitiesListView.ListViewItemSorter = new ListViewItemComparer(e.Column, false);
            else
                this.activitiesListView.ListViewItemSorter = new ListViewItemComparer(e.Column, true);
        }

        //Change the workflow view and activities if the workflow instance selected has changed
        // It can be changed by the user or if during polling if AutoSelectLatest is true
        private void workflowsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (workflowsListView.SelectedItems.Count == 0)
            {
                this.cancelToolStripMenuItem.Enabled = false;
                this.closeActivityToolStripMenuItem.Enabled = false;
                this.deleteToolStripMenuItem.Enabled = false;
                this.currentTasksToolStripMenuItem.Enabled = false;
                this.bindingDataInstanceToolStripMenuItem.Enabled = false;
                return;
            }

            try
            {
                ListViewItem currentWorkflow = workflowsListView.SelectedItems[0];
                try
                {
                    WorkflowStatusInfo workflowStatusInfo = _workflowStatusList[(currentWorkflow.SubItems[0]).Text];
                    _currentWorkflowInstanceId = workflowStatusInfo.InstanceId;
                    ShowWorkflowDefinition(_currentWorkflowInstanceId);
                    ShowViewHost(true);

                    // set the context menu item enabling status
                    if (workflowStatusInfo.Status == Enum.GetName(typeof(WorkflowStatus), WorkflowStatus.Running) ||
                        workflowStatusInfo.Status == Enum.GetName(typeof(WorkflowStatus), WorkflowStatus.Suspended))
                    {
                        this.cancelToolStripMenuItem.Enabled = true;
                        this.deleteToolStripMenuItem.Enabled = false;
                        this.closeActivityToolStripMenuItem.Enabled = true;
                        this.currentTasksToolStripMenuItem.Enabled = true;
                        this.bindingDataInstanceToolStripMenuItem.Enabled = true;
                    }
                    else if (workflowStatusInfo.Status == Enum.GetName(typeof(WorkflowStatus), WorkflowStatus.Completed) ||
                        workflowStatusInfo.Status == Enum.GetName(typeof(WorkflowStatus), WorkflowStatus.Terminated))
                    {
                        this.cancelToolStripMenuItem.Enabled = false;
                        this.deleteToolStripMenuItem.Enabled = true;
                        this.closeActivityToolStripMenuItem.Enabled = false;
                        this.currentTasksToolStripMenuItem.Enabled = false;
                        this.bindingDataInstanceToolStripMenuItem.Enabled = false;
                    }
                }
                catch
                {
                    ShowViewHost(false);
                }

                UpdateActivities();                

                _workflowViewHost.Refresh();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void activitiesListView_Click(object sender, EventArgs e)
        {
            _workflowViewHost.HighlightActivity(
                activitiesListView.SelectedItems[0].SubItems[1].Text);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            RefreshView();
        }

        void WorkflowViewHost_ZoomChanged(object sender, ViewHost.ZoomChangedEventArgs e)
        {
            // Fires when control is resized via combo box or Fit to Size button.
            // If the dropdown is already correct, return.
            //if (e.Zoom.ToString().Equals(this.zoomToolStripComboBox.Text.Trim(new Char[] { '%' })))
            //    return;

            //Return dropdown to 100 percent
            //this.toolStripComboBoxZoom.SelectedIndex = 2;
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelWorkflow();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteTrackingWorkflowInstance();
        }

        private void currentTasksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCurrentTasks();
        }

        private void deleteAllCompletedOrTerminatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllCompletedOrTerminatedTrackingWorkflowInstances();
        }

        private void bindingDataInstanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowBindingInstance();
        }

        private void closeActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelActivity();
        }

        private void workflowViewHost_ActivitySelected(object sender, EventArgs e)
        {
            if (ActivitySelected != null)
            {
                ActivitySelected(this, e);
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            if (_pageIndex > 0)
            {
                _pageIndex--;

                _currentWorkflowInstanceId = Guid.Empty;

                DisplayWorkflows();
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            _pageIndex++;

            _currentWorkflowInstanceId = Guid.Empty;

            DisplayWorkflows();
        }

        #endregion

    }
}
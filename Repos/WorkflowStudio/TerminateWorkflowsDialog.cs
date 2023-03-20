using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.Runtime;

using Newtera.Common.Core;
using Newtera.WorkflowMonitor;
using Newtera.WFModel;

namespace WorkflowStudio
{
    public partial class TerminateWorkflowsDialog : Form
    {
        private WorkflowStatus _status = WorkflowStatus.Running;
        private ITrackingQueryService _trackingQueryService;
        string _workflowTypeId = string.Empty;

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
        /// Gets or sets the the id of the workflow to monitor
        /// </summary>
        public string WorkflowTypeID
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

        public TerminateWorkflowsDialog()
        {
            InitializeComponent();
        }

        private void TrackingQueryDialog_Load(object sender, EventArgs e)
        {
            fromDateTimePicker.Value = DateTime.Now.Subtract(new TimeSpan(365, 0, 0, 0));

            toDateTimePicker.Value = DateTime.Now;

            switch (_status)
            {
                case WorkflowStatus.Running:
                    statusComboBox.SelectedIndex = 0;
                    break;

                case WorkflowStatus.Completed:
                    statusComboBox.SelectedIndex = 1;
                    break;

                case WorkflowStatus.Terminated:
                    statusComboBox.SelectedIndex = 2;
                    break;
            }
        }

        private WorkflowStatus GetWorkflowStatus()
        {
            WorkflowStatus workflowStatus = WorkflowStatus.Running;
            switch (statusComboBox.SelectedIndex)
            {
                case 0:
                    workflowStatus = WorkflowStatus.Running;
                    break;

                case 1:
                    workflowStatus = WorkflowStatus.Completed;
                    break;

                case 2:
                    workflowStatus = WorkflowStatus.Terminated;
                    break;
            }

            return workflowStatus;
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            int worklfowCount = 0;

            //Try to get all of the workflows from the tracking database
            try
            {
                string selectedWorkflowStatus = Enum.GetName(typeof(WorkflowStatus), GetWorkflowStatus());

                worklfowCount = _trackingQueryService.GetWorkflowCount(_workflowTypeId,
                    selectedWorkflowStatus,
                    fromDateTimePicker.Value,
                    toDateTimePicker.Value,
                    true);

                string msg = string.Format(Newtera.WorkflowStudioControl.MessageResourceManager.GetString("WorkflowStudioApp.FoundWorkflows"), worklfowCount.ToString());

                if (MessageBox.Show(msg,
                        "Message Dialog", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Information) == DialogResult.OK)
                {
                    NewteraTrackingWorkflowInstanceCollection foundWorkflows = _trackingQueryService.GetWorkflows(_workflowTypeId,
                    selectedWorkflowStatus,
                    fromDateTimePicker.Value,
                    toDateTimePicker.Value,
                    true,
                    0,
                    20);

                    foreach (NewteraTrackingWorkflowInstance workflowInstance in foundWorkflows)
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            // cancel the workflow instance
                            _trackingQueryService.CancelWorkflow(workflowInstance.WorkflowInstanceId);

                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.WriteLine("Failed to cancel the workflow " + ex.Message + "\n" + ex.StackTrace);
                        }
                        finally
                        {
                            Cursor.Current = this.Cursor;
                        }

                    }

                    foreach (NewteraTrackingWorkflowInstance workflowInstance in foundWorkflows)
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            // delete the tracking instance
                            _trackingQueryService.DeleteTrackingWorkflowInstance(workflowInstance.WorkflowInstanceId);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.WriteLine("Failed to delete the workflow " + ex.Message + "\n" + ex.StackTrace);
                        }
                        finally
                        {
                            Cursor.Current = this.Cursor;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
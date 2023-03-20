using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime;

using Newtera.WFModel;
using Newtera.Activities;
using Newtera.WorkflowMonitor;

namespace WorkflowStudio
{
    public partial class MonitorWorkflowDialog : Form
    {
        internal event EventHandler<SubWorkflowInstanceViewedEventArgs> SubWorkflowInstanceViewed;

        public MonitorWorkflowDialog()
        {
            InitializeComponent();

            // create services used by the monitor
            TrackingQueryService queryService = new TrackingQueryService();
            this.workflowMonitorControl.TrackingQueryService = queryService;

            IActivityValidateService validateService = new ActivityValidateService();
            this.workflowMonitorControl.ActivityValidateService = validateService;

            DataQueryService dataQueryService = new DataQueryService();
            this.workflowMonitorControl.DataQueryService = dataQueryService;
        }

        #region public methods

        public string WorkflowID
        {
            get
            {
                return workflowMonitorControl.WorkflowID;
            }
            set
            {
                workflowMonitorControl.WorkflowID = value;
            }
        }

        public WorkflowModel WorkflowModel
        {
            get
            {
                return workflowMonitorControl.WorkflowModel;
            }
            set
            {
                workflowMonitorControl.WorkflowModel = value;
            }
        }

        public Guid WorkflowInstanceId
        {
            get
            {
                return workflowMonitorControl.CurrentWorkflowInstanceId;
            }
            set
            {
                workflowMonitorControl.CurrentWorkflowInstanceId = value;
            }
        }

        #endregion

        #region event handlers

        private void closeToolStripButton_Click(object sender, EventArgs e)
        {
            this.Close(); // close the dialog
        }

        private void settingsToolStripButton_Click(object sender, EventArgs e)
        {
            MonitorSettingsDialog dialog = new MonitorSettingsDialog();
            dialog.ApplicationSettings = this.workflowMonitorControl.MonitorSettings;

            dialog.ShowDialog();
        }

        private void monitorOnToolStripButton_Click(object sender, EventArgs e)
        {
            this.workflowMonitorControl.Monitor(true);
            this.monitorOffToolStripButton.Enabled = true;
            this.monitorOnToolStripButton.Enabled = false;
        }

        private void monitorOffToolStripButton_Click(object sender, EventArgs e)
        {
            this.workflowMonitorControl.Monitor(false);
            this.monitorOffToolStripButton.Enabled = false;
            this.monitorOnToolStripButton.Enabled = true;
        }

        private void queryToolStripButton_Click(object sender, EventArgs e)
        {
            TrackingQueryDialog dialog = new TrackingQueryDialog();
            dialog.From = workflowMonitorControl.FromDate;
            dialog.To = workflowMonitorControl.ToDate;
            dialog.Status = workflowMonitorControl.Status;
            dialog.WrokflowInstanceId = workflowMonitorControl.WorkflowInstanceIdToFind;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                workflowMonitorControl.FromDate = dialog.From;
                workflowMonitorControl.ToDate = dialog.To;
                workflowMonitorControl.Status = dialog.Status;
                workflowMonitorControl.WorkflowInstanceIdToFind = dialog.WrokflowInstanceId;

                workflowMonitorControl.QueryWorkflows();
            }
        }

        private void MonitorWorkflowDialog_Load(object sender, EventArgs e)
        {
            // Display all workflows by default
            this.workflowMonitorControl.DisplayWorkflows();
        }

  
        private void zoomToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Parse the value and set the WorkflowView zoom - set to 100% if invalid
            string newZoom = this.zoomToolStripComboBox.Text.Trim();
            if (newZoom.EndsWith("%"))
                newZoom = newZoom.Substring(0, newZoom.Length - 1);

            if (newZoom.Length > 0)
            {
                try
                {
                    int zoomNumber = Convert.ToInt32(newZoom);
                    this.workflowMonitorControl.Zoom(zoomNumber);
                }
                catch
                {
                    this.workflowMonitorControl.Zoom(100);
                }
            }
            else
            {
                this.workflowMonitorControl.Zoom(100);
            }
        }

        private void monitorToolStripButton_Click(object sender, EventArgs e)
        {
            if (SubWorkflowInstanceViewed != null)
            {
                // find the info about sub workflow instance to be mpnitored
                Guid parentWorkflowInstanceId = workflowMonitorControl.CurrentWorkflowInstanceId;
                if (parentWorkflowInstanceId != null && parentWorkflowInstanceId != Guid.Empty)
                {
                    TrackingQueryService trackingService = new TrackingQueryService();
                    IInvokeWorkflowActivity invokeWorkflowActivity = workflowMonitorControl.SelectedActivity as IInvokeWorkflowActivity;
                    string subWorkflowInstanceId = null;
                    string subWorkflowName = null;
                    WorkflowEventSubscriptionCollection eventSubscriptions = trackingService.GetWorkflowEventSubscriptions(parentWorkflowInstanceId);
                    foreach (Newtera.WFModel.WorkflowEventSubscription subscription in eventSubscriptions)
                    {
                        // NOTE: a subscription's queue name starts with the name of InvokeNewteraWorkflow activity
                        if (subscription.QueueName.StartsWith(((Activity)invokeWorkflowActivity).Name))
                        {
                            subWorkflowInstanceId = subscription.ChildWorkflowInstanceId.ToString();
                            subWorkflowName = invokeWorkflowActivity.WorkflowName;
                        }
                    }

                    if (subWorkflowInstanceId != null)
                    {
                        SubWorkflowInstanceViewed(this, new SubWorkflowInstanceViewedEventArgs(subWorkflowInstanceId, subWorkflowName));
                    }
                }
            }
        }

        private void workflowMonitorControl_ActivitySelected(object sender, EventArgs e)
        {
            if (this.workflowMonitorControl.SelectedActivity != null &&
                this.workflowMonitorControl.SelectedActivity is IInvokeWorkflowActivity)
            {
                this.monitorToolStripButton.Enabled = true;
            }
            else
            {
                this.monitorToolStripButton.Enabled = false;
            }
        }

        private void terminateToolStripButton_Click(object sender, EventArgs e)
        {
            TerminateWorkflowsDialog dialog = new TerminateWorkflowsDialog();
            dialog.Status = workflowMonitorControl.Status;
            TrackingQueryService queryService = new TrackingQueryService();
            dialog.TrackingQueryService = queryService;
            dialog.WorkflowTypeID = WorkflowID;

            dialog.ShowDialog();
        }

        #endregion event handlers

  

    }
}
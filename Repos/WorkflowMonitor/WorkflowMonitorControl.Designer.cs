namespace Newtera.WorkflowMonitor
{
    partial class WorkflowMonitorControl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkflowMonitorControl));
            this.monitorSurface = new System.Windows.Forms.SplitContainer();
            this.workflowDetails = new System.Windows.Forms.SplitContainer();
            this.pagerPanel = new System.Windows.Forms.Panel();
            this.nextButton = new System.Windows.Forms.Button();
            this.prevButton = new System.Windows.Forms.Button();
            this.workflowsListView = new System.Windows.Forms.ListView();
            this.workflowsIdHeader = new System.Windows.Forms.ColumnHeader();
            this.createTimeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.workflowsStatusHeader = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingDataInstanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentTasksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllCompletedOrTerminatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.workflowsLabel = new System.Windows.Forms.Label();
            this.activitiesListView = new System.Windows.Forms.ListView();
            this.activityIdHeader = new System.Windows.Forms.ColumnHeader();
            this.activityNameHeader = new System.Windows.Forms.ColumnHeader();
            this.activityStatusHeader = new System.Windows.Forms.ColumnHeader();
            this.activitiesLabel = new System.Windows.Forms.Label();
            this.workflowViewErrorTextLabel = new System.Windows.Forms.Label();
            this.worflowIdHeader = new System.Windows.Forms.ColumnHeader();
            this.workflowNameHeader = new System.Windows.Forms.ColumnHeader();
            this.workflowStatusHeader = new System.Windows.Forms.ColumnHeader();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.monitorSurface.Panel1.SuspendLayout();
            this.monitorSurface.Panel2.SuspendLayout();
            this.monitorSurface.SuspendLayout();
            this.workflowDetails.Panel1.SuspendLayout();
            this.workflowDetails.Panel2.SuspendLayout();
            this.workflowDetails.SuspendLayout();
            this.pagerPanel.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // monitorSurface
            // 
            this.monitorSurface.AccessibleDescription = null;
            this.monitorSurface.AccessibleName = null;
            resources.ApplyResources(this.monitorSurface, "monitorSurface");
            this.monitorSurface.BackgroundImage = null;
            this.monitorSurface.Font = null;
            this.monitorSurface.Name = "monitorSurface";
            // 
            // monitorSurface.Panel1
            // 
            this.monitorSurface.Panel1.AccessibleDescription = null;
            this.monitorSurface.Panel1.AccessibleName = null;
            resources.ApplyResources(this.monitorSurface.Panel1, "monitorSurface.Panel1");
            this.monitorSurface.Panel1.BackgroundImage = null;
            this.monitorSurface.Panel1.Controls.Add(this.workflowDetails);
            this.monitorSurface.Panel1.Font = null;
            // 
            // monitorSurface.Panel2
            // 
            this.monitorSurface.Panel2.AccessibleDescription = null;
            this.monitorSurface.Panel2.AccessibleName = null;
            resources.ApplyResources(this.monitorSurface.Panel2, "monitorSurface.Panel2");
            this.monitorSurface.Panel2.BackgroundImage = null;
            this.monitorSurface.Panel2.Controls.Add(this.workflowViewErrorTextLabel);
            this.monitorSurface.Panel2.Font = null;
            // 
            // workflowDetails
            // 
            this.workflowDetails.AccessibleDescription = null;
            this.workflowDetails.AccessibleName = null;
            resources.ApplyResources(this.workflowDetails, "workflowDetails");
            this.workflowDetails.BackgroundImage = null;
            this.workflowDetails.Font = null;
            this.workflowDetails.Name = "workflowDetails";
            // 
            // workflowDetails.Panel1
            // 
            this.workflowDetails.Panel1.AccessibleDescription = null;
            this.workflowDetails.Panel1.AccessibleName = null;
            resources.ApplyResources(this.workflowDetails.Panel1, "workflowDetails.Panel1");
            this.workflowDetails.Panel1.BackgroundImage = null;
            this.workflowDetails.Panel1.Controls.Add(this.pagerPanel);
            this.workflowDetails.Panel1.Controls.Add(this.workflowsListView);
            this.workflowDetails.Panel1.Controls.Add(this.workflowsLabel);
            this.workflowDetails.Panel1.Font = null;
            // 
            // workflowDetails.Panel2
            // 
            this.workflowDetails.Panel2.AccessibleDescription = null;
            this.workflowDetails.Panel2.AccessibleName = null;
            resources.ApplyResources(this.workflowDetails.Panel2, "workflowDetails.Panel2");
            this.workflowDetails.Panel2.BackgroundImage = null;
            this.workflowDetails.Panel2.Controls.Add(this.activitiesListView);
            this.workflowDetails.Panel2.Controls.Add(this.activitiesLabel);
            this.workflowDetails.Panel2.Font = null;
            // 
            // pagerPanel
            // 
            this.pagerPanel.AccessibleDescription = null;
            this.pagerPanel.AccessibleName = null;
            resources.ApplyResources(this.pagerPanel, "pagerPanel");
            this.pagerPanel.BackgroundImage = null;
            this.pagerPanel.Controls.Add(this.nextButton);
            this.pagerPanel.Controls.Add(this.prevButton);
            this.pagerPanel.Font = null;
            this.pagerPanel.Name = "pagerPanel";
            // 
            // nextButton
            // 
            this.nextButton.AccessibleDescription = null;
            this.nextButton.AccessibleName = null;
            resources.ApplyResources(this.nextButton, "nextButton");
            this.nextButton.BackgroundImage = null;
            this.nextButton.Font = null;
            this.nextButton.Name = "nextButton";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // prevButton
            // 
            this.prevButton.AccessibleDescription = null;
            this.prevButton.AccessibleName = null;
            resources.ApplyResources(this.prevButton, "prevButton");
            this.prevButton.BackgroundImage = null;
            this.prevButton.Font = null;
            this.prevButton.Name = "prevButton";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // workflowsListView
            // 
            this.workflowsListView.AccessibleDescription = null;
            this.workflowsListView.AccessibleName = null;
            resources.ApplyResources(this.workflowsListView, "workflowsListView");
            this.workflowsListView.BackgroundImage = null;
            this.workflowsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.workflowsIdHeader,
            this.createTimeColumnHeader,
            this.workflowsStatusHeader});
            this.workflowsListView.ContextMenuStrip = this.contextMenuStrip;
            this.workflowsListView.Font = null;
            this.workflowsListView.FullRowSelect = true;
            this.workflowsListView.HideSelection = false;
            this.workflowsListView.Name = "workflowsListView";
            this.workflowsListView.ShowGroups = false;
            this.workflowsListView.UseCompatibleStateImageBehavior = false;
            this.workflowsListView.View = System.Windows.Forms.View.Details;
            this.workflowsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnWorkflowsColumnClick);
            this.workflowsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.workflowsListView_ItemSelectionChanged);
            this.workflowsListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ManualRefresh);
            // 
            // workflowsIdHeader
            // 
            resources.ApplyResources(this.workflowsIdHeader, "workflowsIdHeader");
            // 
            // createTimeColumnHeader
            // 
            resources.ApplyResources(this.createTimeColumnHeader, "createTimeColumnHeader");
            // 
            // workflowsStatusHeader
            // 
            resources.ApplyResources(this.workflowsStatusHeader, "workflowsStatusHeader");
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.AccessibleDescription = null;
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.BackgroundImage = null;
            this.contextMenuStrip.Font = null;
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.closeActivityToolStripMenuItem,
            this.bindingDataInstanceToolStripMenuItem,
            this.currentTasksToolStripMenuItem,
            this.deleteAllCompletedOrTerminatedToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            // 
            // cancelToolStripMenuItem
            // 
            this.cancelToolStripMenuItem.AccessibleDescription = null;
            this.cancelToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.cancelToolStripMenuItem, "cancelToolStripMenuItem");
            this.cancelToolStripMenuItem.BackgroundImage = null;
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.AccessibleDescription = null;
            this.deleteToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.BackgroundImage = null;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // closeActivityToolStripMenuItem
            // 
            this.closeActivityToolStripMenuItem.AccessibleDescription = null;
            this.closeActivityToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.closeActivityToolStripMenuItem, "closeActivityToolStripMenuItem");
            this.closeActivityToolStripMenuItem.BackgroundImage = null;
            this.closeActivityToolStripMenuItem.Name = "closeActivityToolStripMenuItem";
            this.closeActivityToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.closeActivityToolStripMenuItem.Click += new System.EventHandler(this.closeActivityToolStripMenuItem_Click);
            // 
            // bindingDataInstanceToolStripMenuItem
            // 
            this.bindingDataInstanceToolStripMenuItem.AccessibleDescription = null;
            this.bindingDataInstanceToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.bindingDataInstanceToolStripMenuItem, "bindingDataInstanceToolStripMenuItem");
            this.bindingDataInstanceToolStripMenuItem.BackgroundImage = null;
            this.bindingDataInstanceToolStripMenuItem.Name = "bindingDataInstanceToolStripMenuItem";
            this.bindingDataInstanceToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.bindingDataInstanceToolStripMenuItem.Click += new System.EventHandler(this.bindingDataInstanceToolStripMenuItem_Click);
            // 
            // currentTasksToolStripMenuItem
            // 
            this.currentTasksToolStripMenuItem.AccessibleDescription = null;
            this.currentTasksToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.currentTasksToolStripMenuItem, "currentTasksToolStripMenuItem");
            this.currentTasksToolStripMenuItem.BackgroundImage = null;
            this.currentTasksToolStripMenuItem.Name = "currentTasksToolStripMenuItem";
            this.currentTasksToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.currentTasksToolStripMenuItem.Click += new System.EventHandler(this.currentTasksToolStripMenuItem_Click);
            // 
            // deleteAllCompletedOrTerminatedToolStripMenuItem
            // 
            this.deleteAllCompletedOrTerminatedToolStripMenuItem.AccessibleDescription = null;
            this.deleteAllCompletedOrTerminatedToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.deleteAllCompletedOrTerminatedToolStripMenuItem, "deleteAllCompletedOrTerminatedToolStripMenuItem");
            this.deleteAllCompletedOrTerminatedToolStripMenuItem.BackgroundImage = null;
            this.deleteAllCompletedOrTerminatedToolStripMenuItem.Name = "deleteAllCompletedOrTerminatedToolStripMenuItem";
            this.deleteAllCompletedOrTerminatedToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.deleteAllCompletedOrTerminatedToolStripMenuItem.Click += new System.EventHandler(this.deleteAllCompletedOrTerminatedToolStripMenuItem_Click);
            // 
            // workflowsLabel
            // 
            this.workflowsLabel.AccessibleDescription = null;
            this.workflowsLabel.AccessibleName = null;
            resources.ApplyResources(this.workflowsLabel, "workflowsLabel");
            this.workflowsLabel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.workflowsLabel.Font = null;
            this.workflowsLabel.Name = "workflowsLabel";
            // 
            // activitiesListView
            // 
            this.activitiesListView.AccessibleDescription = null;
            this.activitiesListView.AccessibleName = null;
            resources.ApplyResources(this.activitiesListView, "activitiesListView");
            this.activitiesListView.BackgroundImage = null;
            this.activitiesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.activityIdHeader,
            this.activityNameHeader,
            this.activityStatusHeader});
            this.activitiesListView.Font = null;
            this.activitiesListView.FullRowSelect = true;
            this.activitiesListView.HideSelection = false;
            this.activitiesListView.Name = "activitiesListView";
            this.activitiesListView.ShowGroups = false;
            this.activitiesListView.UseCompatibleStateImageBehavior = false;
            this.activitiesListView.View = System.Windows.Forms.View.Details;
            this.activitiesListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnActivitiesColumnClick);
            this.activitiesListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ManualRefresh);
            this.activitiesListView.Click += new System.EventHandler(this.activitiesListView_Click);
            // 
            // activityIdHeader
            // 
            resources.ApplyResources(this.activityIdHeader, "activityIdHeader");
            // 
            // activityNameHeader
            // 
            resources.ApplyResources(this.activityNameHeader, "activityNameHeader");
            // 
            // activityStatusHeader
            // 
            resources.ApplyResources(this.activityStatusHeader, "activityStatusHeader");
            // 
            // activitiesLabel
            // 
            this.activitiesLabel.AccessibleDescription = null;
            this.activitiesLabel.AccessibleName = null;
            resources.ApplyResources(this.activitiesLabel, "activitiesLabel");
            this.activitiesLabel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.activitiesLabel.Font = null;
            this.activitiesLabel.Name = "activitiesLabel";
            // 
            // workflowViewErrorTextLabel
            // 
            this.workflowViewErrorTextLabel.AccessibleDescription = null;
            this.workflowViewErrorTextLabel.AccessibleName = null;
            resources.ApplyResources(this.workflowViewErrorTextLabel, "workflowViewErrorTextLabel");
            this.workflowViewErrorTextLabel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.workflowViewErrorTextLabel.Font = null;
            this.workflowViewErrorTextLabel.Name = "workflowViewErrorTextLabel";
            // 
            // worflowIdHeader
            // 
            resources.ApplyResources(this.worflowIdHeader, "worflowIdHeader");
            // 
            // workflowNameHeader
            // 
            resources.ApplyResources(this.workflowNameHeader, "workflowNameHeader");
            // 
            // workflowStatusHeader
            // 
            resources.ApplyResources(this.workflowStatusHeader, "workflowStatusHeader");
            // 
            // timer
            // 
            this.timer.Interval = 5000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // WorkflowMonitorControl
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.monitorSurface);
            this.Font = null;
            this.Name = "WorkflowMonitorControl";
            this.monitorSurface.Panel1.ResumeLayout(false);
            this.monitorSurface.Panel2.ResumeLayout(false);
            this.monitorSurface.ResumeLayout(false);
            this.workflowDetails.Panel1.ResumeLayout(false);
            this.workflowDetails.Panel2.ResumeLayout(false);
            this.workflowDetails.ResumeLayout(false);
            this.pagerPanel.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer monitorSurface;
        private System.Windows.Forms.SplitContainer workflowDetails;
        private System.Windows.Forms.ColumnHeader worflowIdHeader;
        private System.Windows.Forms.ColumnHeader workflowNameHeader;
        private System.Windows.Forms.ColumnHeader workflowStatusHeader;
        private System.Windows.Forms.Label workflowsLabel;
        private System.Windows.Forms.ListView activitiesListView;
        private System.Windows.Forms.Label activitiesLabel;
        private System.Windows.Forms.ColumnHeader activityIdHeader;
        private System.Windows.Forms.ColumnHeader activityNameHeader;
        private System.Windows.Forms.ColumnHeader activityStatusHeader;
        private System.Windows.Forms.ListView workflowsListView;
        private System.Windows.Forms.ColumnHeader workflowsIdHeader;
        private System.Windows.Forms.ColumnHeader workflowsStatusHeader;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label workflowViewErrorTextLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentTasksToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader createTimeColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem deleteAllCompletedOrTerminatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bindingDataInstanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeActivityToolStripMenuItem;
        private System.Windows.Forms.Panel pagerPanel;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button prevButton;
    }
}

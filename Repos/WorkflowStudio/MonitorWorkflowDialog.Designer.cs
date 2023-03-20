namespace WorkflowStudio
{
    partial class MonitorWorkflowDialog
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorWorkflowDialog));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.settingsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.monitorOnToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.monitorOffToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.queryToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.terminateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.monitorToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.workflowMonitorControl = new Newtera.WorkflowMonitor.WorkflowMonitorControl();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripButton,
            this.toolStripSeparator2,
            this.monitorOnToolStripButton,
            this.monitorOffToolStripButton,
            this.toolStripSeparator1,
            this.closeToolStripButton,
            this.queryToolStripButton,
            this.toolStripSeparator3,
            this.terminateToolStripButton,
            this.zoomToolStripComboBox,
            this.toolStripSeparator4,
            this.monitorToolStripButton});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // settingsToolStripButton
            // 
            this.settingsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.settingsToolStripButton, "settingsToolStripButton");
            this.settingsToolStripButton.Name = "settingsToolStripButton";
            this.settingsToolStripButton.Click += new System.EventHandler(this.settingsToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // monitorOnToolStripButton
            // 
            this.monitorOnToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.monitorOnToolStripButton, "monitorOnToolStripButton");
            this.monitorOnToolStripButton.Name = "monitorOnToolStripButton";
            this.monitorOnToolStripButton.Click += new System.EventHandler(this.monitorOnToolStripButton_Click);
            // 
            // monitorOffToolStripButton
            // 
            this.monitorOffToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.monitorOffToolStripButton, "monitorOffToolStripButton");
            this.monitorOffToolStripButton.Name = "monitorOffToolStripButton";
            this.monitorOffToolStripButton.Click += new System.EventHandler(this.monitorOffToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // closeToolStripButton
            // 
            this.closeToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.closeToolStripButton, "closeToolStripButton");
            this.closeToolStripButton.Name = "closeToolStripButton";
            this.closeToolStripButton.Click += new System.EventHandler(this.closeToolStripButton_Click);
            // 
            // queryToolStripButton
            // 
            this.queryToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.queryToolStripButton, "queryToolStripButton");
            this.queryToolStripButton.Name = "queryToolStripButton";
            this.queryToolStripButton.Click += new System.EventHandler(this.queryToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // terminateToolStripButton
            // 
            this.terminateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.terminateToolStripButton, "terminateToolStripButton");
            this.terminateToolStripButton.Name = "terminateToolStripButton";
            this.terminateToolStripButton.Click += new System.EventHandler(this.terminateToolStripButton_Click);
            // 
            // zoomToolStripComboBox
            // 
            this.zoomToolStripComboBox.Items.AddRange(new object[] {
            resources.GetString("zoomToolStripComboBox.Items"),
            resources.GetString("zoomToolStripComboBox.Items1"),
            resources.GetString("zoomToolStripComboBox.Items2"),
            resources.GetString("zoomToolStripComboBox.Items3"),
            resources.GetString("zoomToolStripComboBox.Items4"),
            resources.GetString("zoomToolStripComboBox.Items5"),
            resources.GetString("zoomToolStripComboBox.Items6")});
            this.zoomToolStripComboBox.Name = "zoomToolStripComboBox";
            resources.ApplyResources(this.zoomToolStripComboBox, "zoomToolStripComboBox");
            this.zoomToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.zoomToolStripComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // monitorToolStripButton
            // 
            this.monitorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.monitorToolStripButton, "monitorToolStripButton");
            this.monitorToolStripButton.Name = "monitorToolStripButton";
            this.monitorToolStripButton.Click += new System.EventHandler(this.monitorToolStripButton_Click);
            // 
            // workflowMonitorControl
            // 
            this.workflowMonitorControl.ActivityValidateService = null;
            this.workflowMonitorControl.CurrentWorkflowInstanceId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.workflowMonitorControl.DataQueryService = null;
            resources.ApplyResources(this.workflowMonitorControl, "workflowMonitorControl");
            this.workflowMonitorControl.FromDate = new System.DateTime(2009, 11, 7, 21, 18, 51, 984);
            this.workflowMonitorControl.Name = "workflowMonitorControl";
            this.workflowMonitorControl.Status = System.Workflow.Runtime.WorkflowStatus.Running;
            this.workflowMonitorControl.ToDate = new System.DateTime(2009, 11, 8, 21, 18, 51, 984);
            this.workflowMonitorControl.TrackingQueryService = null;
            this.workflowMonitorControl.WorkflowID = "";
            this.workflowMonitorControl.WorkflowInstanceIdToFind = null;
            this.workflowMonitorControl.WorkflowModel = null;
            this.workflowMonitorControl.ActivitySelected += new System.EventHandler(this.workflowMonitorControl_ActivitySelected);
            // 
            // MonitorWorkflowDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.workflowMonitorControl);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MonitorWorkflowDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.MonitorWorkflowDialog_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private Newtera.WorkflowMonitor.WorkflowMonitorControl workflowMonitorControl;
        private System.Windows.Forms.ToolStripButton settingsToolStripButton;
        private System.Windows.Forms.ToolStripButton monitorOnToolStripButton;
        private System.Windows.Forms.ToolStripButton monitorOffToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton closeToolStripButton;
        private System.Windows.Forms.ToolStripButton queryToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton terminateToolStripButton;
        private System.Windows.Forms.ToolStripComboBox zoomToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton monitorToolStripButton;
    }
}
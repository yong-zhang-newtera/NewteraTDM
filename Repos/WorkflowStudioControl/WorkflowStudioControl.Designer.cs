namespace Newtera.WorkflowStudioControl
{
    partial class WorkflowStudioControl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkflowStudioControl));
            this.workflowStudioSplitContainer = new System.Windows.Forms.SplitContainer();
            this.navigateTabControl = new System.Windows.Forms.TabControl();
            this.projectTabPage = new System.Windows.Forms.TabPage();
            this.projectTreeView = new System.Windows.Forms.TreeView();
            this.projectContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddWorkflowStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteWorkflowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.startWorkflowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.AccessControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.monitorWorkflowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectImageList = new System.Windows.Forms.ImageList(this.components);
            this.propertiesTabPage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.propertyContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.descriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activityTypeTextBox = new System.Windows.Forms.TextBox();
            this.activitiesTabPage = new System.Windows.Forms.TabPage();
            this.activitiesPanel = new System.Windows.Forms.Panel();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.categoryComboBox = new System.Windows.Forms.ComboBox();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.workflowStudioSplitContainer.Panel1.SuspendLayout();
            this.workflowStudioSplitContainer.SuspendLayout();
            this.navigateTabControl.SuspendLayout();
            this.projectTabPage.SuspendLayout();
            this.projectContextMenuStrip.SuspendLayout();
            this.propertiesTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.propertyContextMenuStrip.SuspendLayout();
            this.activitiesTabPage.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // workflowStudioSplitContainer
            // 
            this.workflowStudioSplitContainer.AccessibleDescription = null;
            this.workflowStudioSplitContainer.AccessibleName = null;
            resources.ApplyResources(this.workflowStudioSplitContainer, "workflowStudioSplitContainer");
            this.workflowStudioSplitContainer.BackgroundImage = null;
            this.workflowStudioSplitContainer.Font = null;
            this.workflowStudioSplitContainer.Name = "workflowStudioSplitContainer";
            // 
            // workflowStudioSplitContainer.Panel1
            // 
            this.workflowStudioSplitContainer.Panel1.AccessibleDescription = null;
            this.workflowStudioSplitContainer.Panel1.AccessibleName = null;
            resources.ApplyResources(this.workflowStudioSplitContainer.Panel1, "workflowStudioSplitContainer.Panel1");
            this.workflowStudioSplitContainer.Panel1.BackgroundImage = null;
            this.workflowStudioSplitContainer.Panel1.Controls.Add(this.navigateTabControl);
            this.workflowStudioSplitContainer.Panel1.Font = null;
            // 
            // workflowStudioSplitContainer.Panel2
            // 
            this.workflowStudioSplitContainer.Panel2.AccessibleDescription = null;
            this.workflowStudioSplitContainer.Panel2.AccessibleName = null;
            resources.ApplyResources(this.workflowStudioSplitContainer.Panel2, "workflowStudioSplitContainer.Panel2");
            this.workflowStudioSplitContainer.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.workflowStudioSplitContainer.Panel2.BackgroundImage = null;
            this.workflowStudioSplitContainer.Panel2.Font = null;
            // 
            // navigateTabControl
            // 
            this.navigateTabControl.AccessibleDescription = null;
            this.navigateTabControl.AccessibleName = null;
            resources.ApplyResources(this.navigateTabControl, "navigateTabControl");
            this.navigateTabControl.BackgroundImage = null;
            this.navigateTabControl.Controls.Add(this.projectTabPage);
            this.navigateTabControl.Controls.Add(this.propertiesTabPage);
            this.navigateTabControl.Controls.Add(this.activitiesTabPage);
            this.navigateTabControl.Font = null;
            this.navigateTabControl.Name = "navigateTabControl";
            this.navigateTabControl.SelectedIndex = 0;
            this.navigateTabControl.SelectedIndexChanged += new System.EventHandler(this.navigateTabControl_SelectedIndexChanged);
            // 
            // projectTabPage
            // 
            this.projectTabPage.AccessibleDescription = null;
            this.projectTabPage.AccessibleName = null;
            resources.ApplyResources(this.projectTabPage, "projectTabPage");
            this.projectTabPage.BackgroundImage = null;
            this.projectTabPage.Controls.Add(this.projectTreeView);
            this.projectTabPage.Font = null;
            this.projectTabPage.Name = "projectTabPage";
            this.projectTabPage.UseVisualStyleBackColor = true;
            // 
            // projectTreeView
            // 
            this.projectTreeView.AccessibleDescription = null;
            this.projectTreeView.AccessibleName = null;
            resources.ApplyResources(this.projectTreeView, "projectTreeView");
            this.projectTreeView.BackgroundImage = null;
            this.projectTreeView.ContextMenuStrip = this.projectContextMenuStrip;
            this.projectTreeView.Font = null;
            this.projectTreeView.FullRowSelect = true;
            this.projectTreeView.ImageList = this.projectImageList;
            this.projectTreeView.Name = "projectTreeView";
            this.projectTreeView.ShowNodeToolTips = true;
            this.projectTreeView.ShowRootLines = false;
            this.projectTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.projectTreeView_AfterLabelEdit);
            this.projectTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.projectTreeView_AfterSelect);
            this.projectTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.projectTreeView_MouseDown);
            // 
            // projectContextMenuStrip
            // 
            this.projectContextMenuStrip.AccessibleDescription = null;
            this.projectContextMenuStrip.AccessibleName = null;
            resources.ApplyResources(this.projectContextMenuStrip, "projectContextMenuStrip");
            this.projectContextMenuStrip.BackgroundImage = null;
            this.projectContextMenuStrip.Font = null;
            this.projectContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddWorkflowStripMenuItem,
            this.DeleteWorkflowMenuItem,
            this.RenameStripMenuItem,
            this.toolStripSeparator2,
            this.startWorkflowToolStripMenuItem,
            this.toolStripMenuItem1,
            this.AccessControlToolStripMenuItem,
            this.toolStripSeparator1,
            this.monitorWorkflowsToolStripMenuItem});
            this.projectContextMenuStrip.Name = "projectContextMenuStrip";
            // 
            // AddWorkflowStripMenuItem
            // 
            this.AddWorkflowStripMenuItem.AccessibleDescription = null;
            this.AddWorkflowStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.AddWorkflowStripMenuItem, "AddWorkflowStripMenuItem");
            this.AddWorkflowStripMenuItem.BackgroundImage = null;
            this.AddWorkflowStripMenuItem.Name = "AddWorkflowStripMenuItem";
            this.AddWorkflowStripMenuItem.ShortcutKeyDisplayString = null;
            this.AddWorkflowStripMenuItem.Click += new System.EventHandler(this.AddWorkflowStripMenuItem_Click);
            // 
            // DeleteWorkflowMenuItem
            // 
            this.DeleteWorkflowMenuItem.AccessibleDescription = null;
            this.DeleteWorkflowMenuItem.AccessibleName = null;
            resources.ApplyResources(this.DeleteWorkflowMenuItem, "DeleteWorkflowMenuItem");
            this.DeleteWorkflowMenuItem.BackgroundImage = null;
            this.DeleteWorkflowMenuItem.Name = "DeleteWorkflowMenuItem";
            this.DeleteWorkflowMenuItem.ShortcutKeyDisplayString = null;
            this.DeleteWorkflowMenuItem.Click += new System.EventHandler(this.DeleteWorkflowMenuItem_Click);
            // 
            // RenameStripMenuItem
            // 
            this.RenameStripMenuItem.AccessibleDescription = null;
            this.RenameStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.RenameStripMenuItem, "RenameStripMenuItem");
            this.RenameStripMenuItem.BackgroundImage = null;
            this.RenameStripMenuItem.Name = "RenameStripMenuItem";
            this.RenameStripMenuItem.ShortcutKeyDisplayString = null;
            this.RenameStripMenuItem.Click += new System.EventHandler(this.RenameStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AccessibleDescription = null;
            this.toolStripSeparator2.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // startWorkflowToolStripMenuItem
            // 
            this.startWorkflowToolStripMenuItem.AccessibleDescription = null;
            this.startWorkflowToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.startWorkflowToolStripMenuItem, "startWorkflowToolStripMenuItem");
            this.startWorkflowToolStripMenuItem.BackgroundImage = null;
            this.startWorkflowToolStripMenuItem.Name = "startWorkflowToolStripMenuItem";
            this.startWorkflowToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.startWorkflowToolStripMenuItem.Click += new System.EventHandler(this.startWorkflowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.AccessibleDescription = null;
            this.toolStripMenuItem1.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // AccessControlToolStripMenuItem
            // 
            this.AccessControlToolStripMenuItem.AccessibleDescription = null;
            this.AccessControlToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.AccessControlToolStripMenuItem, "AccessControlToolStripMenuItem");
            this.AccessControlToolStripMenuItem.BackgroundImage = null;
            this.AccessControlToolStripMenuItem.Name = "AccessControlToolStripMenuItem";
            this.AccessControlToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.AccessControlToolStripMenuItem.Click += new System.EventHandler(this.AccessControlToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // monitorWorkflowsToolStripMenuItem
            // 
            this.monitorWorkflowsToolStripMenuItem.AccessibleDescription = null;
            this.monitorWorkflowsToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.monitorWorkflowsToolStripMenuItem, "monitorWorkflowsToolStripMenuItem");
            this.monitorWorkflowsToolStripMenuItem.BackgroundImage = null;
            this.monitorWorkflowsToolStripMenuItem.Name = "monitorWorkflowsToolStripMenuItem";
            this.monitorWorkflowsToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.monitorWorkflowsToolStripMenuItem.Click += new System.EventHandler(this.monitorWorkflowsToolStripMenuItem_Click);
            // 
            // projectImageList
            // 
            this.projectImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("projectImageList.ImageStream")));
            this.projectImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.projectImageList.Images.SetKeyName(0, "Project.bmp");
            this.projectImageList.Images.SetKeyName(1, "Sequential.bmp");
            this.projectImageList.Images.SetKeyName(2, "StateMachine.bmp");
            this.projectImageList.Images.SetKeyName(3, "WorkFlowStep.bmp");
            // 
            // propertiesTabPage
            // 
            this.propertiesTabPage.AccessibleDescription = null;
            this.propertiesTabPage.AccessibleName = null;
            resources.ApplyResources(this.propertiesTabPage, "propertiesTabPage");
            this.propertiesTabPage.BackgroundImage = null;
            this.propertiesTabPage.Controls.Add(this.panel1);
            this.propertiesTabPage.Font = null;
            this.propertiesTabPage.Name = "propertiesTabPage";
            this.propertiesTabPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.propertyGrid);
            this.panel1.Controls.Add(this.activityTypeTextBox);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // propertyGrid
            // 
            this.propertyGrid.AccessibleDescription = null;
            this.propertyGrid.AccessibleName = null;
            resources.ApplyResources(this.propertyGrid, "propertyGrid");
            this.propertyGrid.BackgroundImage = null;
            this.propertyGrid.ContextMenuStrip = this.propertyContextMenuStrip;
            this.propertyGrid.Font = null;
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.Enter += new System.EventHandler(this.propertyGrid_GotFocus);
            this.propertyGrid.SelectedObjectsChanged += new System.EventHandler(this.propertyGrid_SelectedObjectsChanged);
            // 
            // propertyContextMenuStrip
            // 
            this.propertyContextMenuStrip.AccessibleDescription = null;
            this.propertyContextMenuStrip.AccessibleName = null;
            resources.ApplyResources(this.propertyContextMenuStrip, "propertyContextMenuStrip");
            this.propertyContextMenuStrip.BackgroundImage = null;
            this.propertyContextMenuStrip.Font = null;
            this.propertyContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.toolStripMenuItem2,
            this.descriptionToolStripMenuItem});
            this.propertyContextMenuStrip.Name = "propertyContextMenuStrip";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.AccessibleDescription = null;
            this.resetToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.resetToolStripMenuItem, "resetToolStripMenuItem");
            this.resetToolStripMenuItem.BackgroundImage = null;
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.AccessibleDescription = null;
            this.toolStripMenuItem2.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // descriptionToolStripMenuItem
            // 
            this.descriptionToolStripMenuItem.AccessibleDescription = null;
            this.descriptionToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.descriptionToolStripMenuItem, "descriptionToolStripMenuItem");
            this.descriptionToolStripMenuItem.BackgroundImage = null;
            this.descriptionToolStripMenuItem.Checked = true;
            this.descriptionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.descriptionToolStripMenuItem.Name = "descriptionToolStripMenuItem";
            this.descriptionToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.descriptionToolStripMenuItem.Click += new System.EventHandler(this.descriptionToolStripMenuItem_Click);
            // 
            // activityTypeTextBox
            // 
            this.activityTypeTextBox.AccessibleDescription = null;
            this.activityTypeTextBox.AccessibleName = null;
            resources.ApplyResources(this.activityTypeTextBox, "activityTypeTextBox");
            this.activityTypeTextBox.BackgroundImage = null;
            this.activityTypeTextBox.Font = null;
            this.activityTypeTextBox.Name = "activityTypeTextBox";
            this.activityTypeTextBox.ReadOnly = true;
            // 
            // activitiesTabPage
            // 
            this.activitiesTabPage.AccessibleDescription = null;
            this.activitiesTabPage.AccessibleName = null;
            resources.ApplyResources(this.activitiesTabPage, "activitiesTabPage");
            this.activitiesTabPage.BackColor = System.Drawing.Color.LightGray;
            this.activitiesTabPage.BackgroundImage = null;
            this.activitiesTabPage.Controls.Add(this.activitiesPanel);
            this.activitiesTabPage.Controls.Add(this.filterPanel);
            this.activitiesTabPage.Font = null;
            this.activitiesTabPage.Name = "activitiesTabPage";
            // 
            // activitiesPanel
            // 
            this.activitiesPanel.AccessibleDescription = null;
            this.activitiesPanel.AccessibleName = null;
            resources.ApplyResources(this.activitiesPanel, "activitiesPanel");
            this.activitiesPanel.BackColor = System.Drawing.Color.Transparent;
            this.activitiesPanel.BackgroundImage = null;
            this.activitiesPanel.Font = null;
            this.activitiesPanel.Name = "activitiesPanel";
            // 
            // filterPanel
            // 
            this.filterPanel.AccessibleDescription = null;
            this.filterPanel.AccessibleName = null;
            resources.ApplyResources(this.filterPanel, "filterPanel");
            this.filterPanel.BackColor = System.Drawing.Color.Transparent;
            this.filterPanel.BackgroundImage = null;
            this.filterPanel.Controls.Add(this.categoryComboBox);
            this.filterPanel.Controls.Add(this.categoryLabel);
            this.filterPanel.Font = null;
            this.filterPanel.Name = "filterPanel";
            // 
            // categoryComboBox
            // 
            this.categoryComboBox.AccessibleDescription = null;
            this.categoryComboBox.AccessibleName = null;
            resources.ApplyResources(this.categoryComboBox, "categoryComboBox");
            this.categoryComboBox.BackgroundImage = null;
            this.categoryComboBox.Font = null;
            this.categoryComboBox.FormattingEnabled = true;
            this.categoryComboBox.Items.AddRange(new object[] {
            resources.GetString("categoryComboBox.Items"),
            resources.GetString("categoryComboBox.Items1"),
            resources.GetString("categoryComboBox.Items2")});
            this.categoryComboBox.Name = "categoryComboBox";
            this.categoryComboBox.SelectedIndexChanged += new System.EventHandler(this.categoryComboBox_SelectedIndexChanged);
            // 
            // categoryLabel
            // 
            this.categoryLabel.AccessibleDescription = null;
            this.categoryLabel.AccessibleName = null;
            resources.ApplyResources(this.categoryLabel, "categoryLabel");
            this.categoryLabel.Font = null;
            this.categoryLabel.Name = "categoryLabel";
            // 
            // WorkflowStudioControl
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.workflowStudioSplitContainer);
            this.Name = "WorkflowStudioControl";
            this.workflowStudioSplitContainer.Panel1.ResumeLayout(false);
            this.workflowStudioSplitContainer.ResumeLayout(false);
            this.navigateTabControl.ResumeLayout(false);
            this.projectTabPage.ResumeLayout(false);
            this.projectContextMenuStrip.ResumeLayout(false);
            this.propertiesTabPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.propertyContextMenuStrip.ResumeLayout(false);
            this.activitiesTabPage.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer workflowStudioSplitContainer;
        private System.Windows.Forms.TabControl navigateTabControl;
        private System.Windows.Forms.TabPage projectTabPage;
        private System.Windows.Forms.TabPage propertiesTabPage;
        private System.Windows.Forms.TreeView projectTreeView;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.TabPage activitiesTabPage;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.Panel activitiesPanel;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.ComboBox categoryComboBox;
        private System.Windows.Forms.ImageList projectImageList;
        private System.Windows.Forms.ContextMenuStrip projectContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem AddWorkflowStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteWorkflowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem monitorWorkflowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startWorkflowToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip propertyContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem descriptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AccessControlToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TextBox activityTypeTextBox;
        private System.Windows.Forms.Panel panel1;
    }
}

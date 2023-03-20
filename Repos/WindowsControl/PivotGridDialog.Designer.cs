namespace Newtera.WindowsControl
{
    partial class PivotGridDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PivotGridDialog));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.pivotGridControl = new DevExpress.XtraPivotGrid.PivotGridControl();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showColumnTotalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showColumnGrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRowTotalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRowGrandTotalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showColumnHeadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRowHeadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDataHeadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFilterHeadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToRTFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToXSLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToHTMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.Name = "splitContainerControl1";
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.pivotGridControl);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 216;
            // 
            // pivotGridControl
            // 
            this.pivotGridControl.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.pivotGridControl, "pivotGridControl");
            this.pivotGridControl.Name = "pivotGridControl";
            this.pivotGridControl.OptionsView.ShowCustomTotalsForSingleValues = true;
            this.pivotGridControl.OptionsView.ShowTotalsForSingleValues = true;
            this.pivotGridControl.FieldAreaChanged += new DevExpress.XtraPivotGrid.PivotFieldEventHandler(this.pivotGridControl_FieldAreaChanged);
            this.pivotGridControl.ShowingCustomizationForm += new DevExpress.XtraPivotGrid.CustomizationFormShowingEventHandler(this.pivotGridControl_ShowingCustomizationForm);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionsToolStripMenuItem,
            this.viewOptionsToolStripMenuItem,
            this.exportToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.deleteLayoutToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            resources.ApplyResources(this.actionsToolStripMenuItem, "actionsToolStripMenuItem");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // deleteLayoutToolStripMenuItem
            // 
            this.deleteLayoutToolStripMenuItem.Name = "deleteLayoutToolStripMenuItem";
            resources.ApplyResources(this.deleteLayoutToolStripMenuItem, "deleteLayoutToolStripMenuItem");
            this.deleteLayoutToolStripMenuItem.Click += new System.EventHandler(this.deleteLayoutToolStripMenuItem_Click);
            // 
            // viewOptionsToolStripMenuItem
            // 
            this.viewOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showColumnTotalsToolStripMenuItem,
            this.showColumnGrToolStripMenuItem,
            this.showRowTotalsToolStripMenuItem,
            this.showRowGrandTotalsToolStripMenuItem,
            this.showColumnHeadersToolStripMenuItem,
            this.showRowHeadersToolStripMenuItem,
            this.showDataHeadersToolStripMenuItem,
            this.showFilterHeadersToolStripMenuItem});
            this.viewOptionsToolStripMenuItem.Name = "viewOptionsToolStripMenuItem";
            resources.ApplyResources(this.viewOptionsToolStripMenuItem, "viewOptionsToolStripMenuItem");
            this.viewOptionsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewOptionsToolStripMenuItem_DropDownOpening);
            // 
            // showColumnTotalsToolStripMenuItem
            // 
            this.showColumnTotalsToolStripMenuItem.CheckOnClick = true;
            this.showColumnTotalsToolStripMenuItem.Name = "showColumnTotalsToolStripMenuItem";
            resources.ApplyResources(this.showColumnTotalsToolStripMenuItem, "showColumnTotalsToolStripMenuItem");
            this.showColumnTotalsToolStripMenuItem.Tag = "ShowColumnTotals";
            this.showColumnTotalsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showColumnTotalsToolStripMenuItem_CheckedChanged);
            // 
            // showColumnGrToolStripMenuItem
            // 
            this.showColumnGrToolStripMenuItem.Checked = true;
            this.showColumnGrToolStripMenuItem.CheckOnClick = true;
            this.showColumnGrToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showColumnGrToolStripMenuItem.Name = "showColumnGrToolStripMenuItem";
            resources.ApplyResources(this.showColumnGrToolStripMenuItem, "showColumnGrToolStripMenuItem");
            this.showColumnGrToolStripMenuItem.Tag = "ShowColumnGrandTotal";
            this.showColumnGrToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showColumnGrToolStripMenuItem_CheckedChanged);
            // 
            // showRowTotalsToolStripMenuItem
            // 
            this.showRowTotalsToolStripMenuItem.CheckOnClick = true;
            this.showRowTotalsToolStripMenuItem.Name = "showRowTotalsToolStripMenuItem";
            resources.ApplyResources(this.showRowTotalsToolStripMenuItem, "showRowTotalsToolStripMenuItem");
            this.showRowTotalsToolStripMenuItem.Tag = "ShowRowTotals";
            this.showRowTotalsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showRowTotalsToolStripMenuItem_CheckedChanged);
            // 
            // showRowGrandTotalsToolStripMenuItem
            // 
            this.showRowGrandTotalsToolStripMenuItem.CheckOnClick = true;
            this.showRowGrandTotalsToolStripMenuItem.Name = "showRowGrandTotalsToolStripMenuItem";
            resources.ApplyResources(this.showRowGrandTotalsToolStripMenuItem, "showRowGrandTotalsToolStripMenuItem");
            this.showRowGrandTotalsToolStripMenuItem.Tag = "ShowRowGrandTotals";
            this.showRowGrandTotalsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showRowGrandTotalsToolStripMenuItem_CheckedChanged);
            // 
            // showColumnHeadersToolStripMenuItem
            // 
            this.showColumnHeadersToolStripMenuItem.Checked = true;
            this.showColumnHeadersToolStripMenuItem.CheckOnClick = true;
            this.showColumnHeadersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showColumnHeadersToolStripMenuItem.Name = "showColumnHeadersToolStripMenuItem";
            resources.ApplyResources(this.showColumnHeadersToolStripMenuItem, "showColumnHeadersToolStripMenuItem");
            this.showColumnHeadersToolStripMenuItem.Tag = "ShowColumnHeaders";
            this.showColumnHeadersToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showColumnHeadersToolStripMenuItem_CheckedChanged);
            // 
            // showRowHeadersToolStripMenuItem
            // 
            this.showRowHeadersToolStripMenuItem.Checked = true;
            this.showRowHeadersToolStripMenuItem.CheckOnClick = true;
            this.showRowHeadersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showRowHeadersToolStripMenuItem.Name = "showRowHeadersToolStripMenuItem";
            resources.ApplyResources(this.showRowHeadersToolStripMenuItem, "showRowHeadersToolStripMenuItem");
            this.showRowHeadersToolStripMenuItem.Tag = "ShowRowHeaders";
            this.showRowHeadersToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showRowHeadersToolStripMenuItem_CheckedChanged);
            // 
            // showDataHeadersToolStripMenuItem
            // 
            this.showDataHeadersToolStripMenuItem.Checked = true;
            this.showDataHeadersToolStripMenuItem.CheckOnClick = true;
            this.showDataHeadersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showDataHeadersToolStripMenuItem.Name = "showDataHeadersToolStripMenuItem";
            resources.ApplyResources(this.showDataHeadersToolStripMenuItem, "showDataHeadersToolStripMenuItem");
            this.showDataHeadersToolStripMenuItem.Tag = "ShowDataHeaders";
            this.showDataHeadersToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showDataHeadersToolStripMenuItem_CheckedChanged);
            // 
            // showFilterHeadersToolStripMenuItem
            // 
            this.showFilterHeadersToolStripMenuItem.Checked = true;
            this.showFilterHeadersToolStripMenuItem.CheckOnClick = true;
            this.showFilterHeadersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showFilterHeadersToolStripMenuItem.Name = "showFilterHeadersToolStripMenuItem";
            resources.ApplyResources(this.showFilterHeadersToolStripMenuItem, "showFilterHeadersToolStripMenuItem");
            this.showFilterHeadersToolStripMenuItem.Tag = "ShowFilterHeaders";
            this.showFilterHeadersToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showFilterHeadersToolStripMenuItem_CheckedChanged);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToPDFToolStripMenuItem,
            this.exportToRTFToolStripMenuItem,
            this.exportToXSLToolStripMenuItem,
            this.exportToTextToolStripMenuItem,
            this.exportToHTMLToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            resources.ApplyResources(this.exportToolStripMenuItem, "exportToolStripMenuItem");
            // 
            // exportToPDFToolStripMenuItem
            // 
            this.exportToPDFToolStripMenuItem.Name = "exportToPDFToolStripMenuItem";
            resources.ApplyResources(this.exportToPDFToolStripMenuItem, "exportToPDFToolStripMenuItem");
            this.exportToPDFToolStripMenuItem.Click += new System.EventHandler(this.exportToPDFToolStripMenuItem_Click);
            // 
            // exportToRTFToolStripMenuItem
            // 
            this.exportToRTFToolStripMenuItem.Name = "exportToRTFToolStripMenuItem";
            resources.ApplyResources(this.exportToRTFToolStripMenuItem, "exportToRTFToolStripMenuItem");
            this.exportToRTFToolStripMenuItem.Click += new System.EventHandler(this.exportToRTFToolStripMenuItem_Click);
            // 
            // exportToXSLToolStripMenuItem
            // 
            this.exportToXSLToolStripMenuItem.Name = "exportToXSLToolStripMenuItem";
            resources.ApplyResources(this.exportToXSLToolStripMenuItem, "exportToXSLToolStripMenuItem");
            this.exportToXSLToolStripMenuItem.Click += new System.EventHandler(this.exportToXSLToolStripMenuItem_Click);
            // 
            // exportToTextToolStripMenuItem
            // 
            this.exportToTextToolStripMenuItem.Name = "exportToTextToolStripMenuItem";
            resources.ApplyResources(this.exportToTextToolStripMenuItem, "exportToTextToolStripMenuItem");
            this.exportToTextToolStripMenuItem.Click += new System.EventHandler(this.exportToTextToolStripMenuItem_Click);
            // 
            // exportToHTMLToolStripMenuItem
            // 
            this.exportToHTMLToolStripMenuItem.Name = "exportToHTMLToolStripMenuItem";
            resources.ApplyResources(this.exportToHTMLToolStripMenuItem, "exportToHTMLToolStripMenuItem");
            this.exportToHTMLToolStripMenuItem.Click += new System.EventHandler(this.exportToHTMLToolStripMenuItem_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // PivotGridDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "PivotGridDialog";
            this.Load += new System.EventHandler(this.PivotGridDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.Button cancelButton;
        private DevExpress.XtraPivotGrid.PivotGridControl pivotGridControl;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.ToolStripMenuItem viewOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToRTFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToXSLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToHTMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showColumnTotalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showColumnGrToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showRowTotalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showRowGrandTotalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showColumnHeadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showRowHeadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDataHeadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showFilterHeadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteLayoutToolStripMenuItem;
    }
}
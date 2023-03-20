namespace SmartExcel2013
{
    partial class InstanceDetailDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstanceDetailDialog));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.propertiesTabPage = new System.Windows.Forms.TabPage();
            this.instanceViewGrid = new System.Windows.Forms.PropertyGrid();
            this.attachmentTabPage = new System.Windows.Forms.TabPage();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.propertiesTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.propertiesTabPage);
            this.tabControl.Controls.Add(this.attachmentTabPage);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            // 
            // propertiesTabPage
            // 
            this.propertiesTabPage.Controls.Add(this.instanceViewGrid);
            resources.ApplyResources(this.propertiesTabPage, "propertiesTabPage");
            this.propertiesTabPage.Name = "propertiesTabPage";
            this.propertiesTabPage.UseVisualStyleBackColor = true;
            // 
            // instanceViewGrid
            // 
            resources.ApplyResources(this.instanceViewGrid, "instanceViewGrid");
            this.instanceViewGrid.Name = "instanceViewGrid";
            this.instanceViewGrid.ToolbarVisible = false;
            // 
            // attachmentTabPage
            // 
            resources.ApplyResources(this.attachmentTabPage, "attachmentTabPage");
            this.attachmentTabPage.Name = "attachmentTabPage";
            this.attachmentTabPage.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // InstanceDetailDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "InstanceDetailDialog";
            this.Load += new System.EventHandler(this.InstanceDetailDialog_Load);
            this.tabControl.ResumeLayout(false);
            this.propertiesTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage propertiesTabPage;
        private System.Windows.Forms.TabPage attachmentTabPage;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PropertyGrid instanceViewGrid;
    }
}
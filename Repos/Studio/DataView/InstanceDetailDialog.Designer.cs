namespace Newtera.Studio
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.propertiesTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.AccessibleDescription = null;
            this.tabControl.AccessibleName = null;
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.BackgroundImage = null;
            this.tabControl.Controls.Add(this.propertiesTabPage);
            this.tabControl.Font = null;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            // 
            // propertiesTabPage
            // 
            this.propertiesTabPage.AccessibleDescription = null;
            this.propertiesTabPage.AccessibleName = null;
            resources.ApplyResources(this.propertiesTabPage, "propertiesTabPage");
            this.propertiesTabPage.BackgroundImage = null;
            this.propertiesTabPage.Controls.Add(this.instanceViewGrid);
            this.propertiesTabPage.Font = null;
            this.propertiesTabPage.Name = "propertiesTabPage";
            this.propertiesTabPage.UseVisualStyleBackColor = true;
            // 
            // instanceViewGrid
            // 
            this.instanceViewGrid.AccessibleDescription = null;
            this.instanceViewGrid.AccessibleName = null;
            resources.ApplyResources(this.instanceViewGrid, "instanceViewGrid");
            this.instanceViewGrid.BackgroundImage = null;
            this.instanceViewGrid.Font = null;
            this.instanceViewGrid.Name = "instanceViewGrid";
            this.instanceViewGrid.ToolbarVisible = false;
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // InstanceDetailDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.tabControl);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "InstanceDetailDialog";
            this.Load += new System.EventHandler(this.InstanceDetailDialog_Load);
            this.tabControl.ResumeLayout(false);
            this.propertiesTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage propertiesTabPage;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PropertyGrid instanceViewGrid;
    }
}
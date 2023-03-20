namespace Newtera.WorkflowMonitor
{
    partial class DataInstanceDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataInstanceDialog));
            this.dataInstancePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.classTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // dataInstancePropertyGrid
            // 
            this.dataInstancePropertyGrid.AccessibleDescription = null;
            this.dataInstancePropertyGrid.AccessibleName = null;
            resources.ApplyResources(this.dataInstancePropertyGrid, "dataInstancePropertyGrid");
            this.dataInstancePropertyGrid.BackgroundImage = null;
            this.dataInstancePropertyGrid.Font = null;
            this.dataInstancePropertyGrid.Name = "dataInstancePropertyGrid";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
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
            // classTextBox
            // 
            this.classTextBox.AccessibleDescription = null;
            this.classTextBox.AccessibleName = null;
            resources.ApplyResources(this.classTextBox, "classTextBox");
            this.classTextBox.BackgroundImage = null;
            this.classTextBox.CausesValidation = false;
            this.classTextBox.Font = null;
            this.classTextBox.Name = "classTextBox";
            this.classTextBox.ReadOnly = true;
            // 
            // DataInstanceDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.classTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataInstancePropertyGrid);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "DataInstanceDialog";
            this.Load += new System.EventHandler(this.DataInstanceDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid dataInstancePropertyGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox classTextBox;
    }
}
namespace Newtera.Studio
{
    partial class ViewLoggingMessageDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewLoggingMessageDialog));
            this.resultDataControl = new Newtera.WindowsControl.ResultDataControl();
            this.cancelButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.deatilButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // resultDataControl
            // 
            this.resultDataControl.AccessibleDescription = null;
            this.resultDataControl.AccessibleName = null;
            resources.ApplyResources(this.resultDataControl, "resultDataControl");
            this.resultDataControl.BackgroundImage = null;
            this.resultDataControl.CurrentSlide = null;
            this.resultDataControl.Font = null;
            this.resultDataControl.MenuItemStates = null;
            this.resultDataControl.MetaData = null;
            this.resultDataControl.Name = "resultDataControl";
            this.resultDataControl.UserManager = null;
            this.resultDataControl.UserName = null;
            this.resultDataControl.RowSelectedIndexChangedEvent += new System.EventHandler(this.resultDataControl1_RowSelectedIndexChangedEvent);
            this.resultDataControl.RequestForCountEvent += new System.EventHandler(this.resultDataControl1_RequestForCountEvent);
            this.resultDataControl.RequestForDataEvent += new System.EventHandler(this.resultDataControl1_RequestForDataEvent);
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
            // searchButton
            // 
            this.searchButton.AccessibleDescription = null;
            this.searchButton.AccessibleName = null;
            resources.ApplyResources(this.searchButton, "searchButton");
            this.searchButton.BackgroundImage = null;
            this.searchButton.Font = null;
            this.searchButton.Name = "searchButton";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // deatilButton
            // 
            this.deatilButton.AccessibleDescription = null;
            this.deatilButton.AccessibleName = null;
            resources.ApplyResources(this.deatilButton, "deatilButton");
            this.deatilButton.BackgroundImage = null;
            this.deatilButton.Font = null;
            this.deatilButton.Name = "deatilButton";
            this.deatilButton.UseVisualStyleBackColor = true;
            this.deatilButton.Click += new System.EventHandler(this.deatilButton_Click);
            // 
            // ViewLoggingMessageDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.deatilButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.resultDataControl);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "ViewLoggingMessageDialog";
            this.Load += new System.EventHandler(this.ViewLoggingMessageDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Newtera.WindowsControl.ResultDataControl resultDataControl;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button deatilButton;
    }
}
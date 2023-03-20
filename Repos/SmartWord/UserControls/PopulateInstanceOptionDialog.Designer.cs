namespace SmartWord
{
    partial class PopulateInstanceOptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopulateInstanceOptionDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.entireDocumentRadioButton = new System.Windows.Forms.RadioButton();
            this.selectedNodeRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.entireDocumentRadioButton);
            this.groupBox1.Controls.Add(this.selectedNodeRadioButton);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // entireDocumentRadioButton
            // 
            this.entireDocumentRadioButton.AccessibleDescription = null;
            this.entireDocumentRadioButton.AccessibleName = null;
            resources.ApplyResources(this.entireDocumentRadioButton, "entireDocumentRadioButton");
            this.entireDocumentRadioButton.BackgroundImage = null;
            this.entireDocumentRadioButton.Checked = true;
            this.entireDocumentRadioButton.Font = null;
            this.entireDocumentRadioButton.Name = "entireDocumentRadioButton";
            this.entireDocumentRadioButton.TabStop = true;
            this.entireDocumentRadioButton.UseVisualStyleBackColor = true;
            // 
            // selectedNodeRadioButton
            // 
            this.selectedNodeRadioButton.AccessibleDescription = null;
            this.selectedNodeRadioButton.AccessibleName = null;
            resources.ApplyResources(this.selectedNodeRadioButton, "selectedNodeRadioButton");
            this.selectedNodeRadioButton.BackgroundImage = null;
            this.selectedNodeRadioButton.Font = null;
            this.selectedNodeRadioButton.Name = "selectedNodeRadioButton";
            this.selectedNodeRadioButton.UseVisualStyleBackColor = true;
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
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // PopulateInstanceOptionDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.Name = "PopulateInstanceOptionDialog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton entireDocumentRadioButton;
        private System.Windows.Forms.RadioButton selectedNodeRadioButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}
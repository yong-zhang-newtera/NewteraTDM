namespace SmartWord2013
{
    partial class SelectViewOrFamilyDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectViewOrFamilyDialog));
            this.viewRadioButton = new System.Windows.Forms.RadioButton();
            this.familyRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // viewRadioButton
            // 
            this.viewRadioButton.AccessibleDescription = null;
            this.viewRadioButton.AccessibleName = null;
            resources.ApplyResources(this.viewRadioButton, "viewRadioButton");
            this.viewRadioButton.BackgroundImage = null;
            this.viewRadioButton.Checked = true;
            this.viewRadioButton.Font = null;
            this.viewRadioButton.Name = "viewRadioButton";
            this.viewRadioButton.TabStop = true;
            this.viewRadioButton.UseVisualStyleBackColor = true;
            this.viewRadioButton.CheckedChanged += new System.EventHandler(this.viewRadioButton_CheckedChanged);
            // 
            // familyRadioButton
            // 
            this.familyRadioButton.AccessibleDescription = null;
            this.familyRadioButton.AccessibleName = null;
            resources.ApplyResources(this.familyRadioButton, "familyRadioButton");
            this.familyRadioButton.BackgroundImage = null;
            this.familyRadioButton.Font = null;
            this.familyRadioButton.Name = "familyRadioButton";
            this.familyRadioButton.UseVisualStyleBackColor = true;
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
            // 
            // SelectViewOrFamilyDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.familyRadioButton);
            this.Controls.Add(this.viewRadioButton);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.Name = "SelectViewOrFamilyDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton viewRadioButton;
        private System.Windows.Forms.RadioButton familyRadioButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}
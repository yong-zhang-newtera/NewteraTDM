namespace WorkflowStudio
{
    partial class SelectParameterBindingDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectParameterBindingDialog));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.dataInstanceTabPage = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.classListView = new System.Windows.Forms.ListView();
            this.captionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.listViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.parameterTabPage = new System.Windows.Forms.TabPage();
            this.parameterListBox = new System.Windows.Forms.ListBox();
            this.activityPropertyTabPage = new System.Windows.Forms.TabPage();
            this.propertyListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.activityComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.dataInstanceTabPage.SuspendLayout();
            this.parameterTabPage.SuspendLayout();
            this.activityPropertyTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.dataInstanceTabPage);
            this.tabControl1.Controls.Add(this.parameterTabPage);
            this.tabControl1.Controls.Add(this.activityPropertyTabPage);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // dataInstanceTabPage
            // 
            this.dataInstanceTabPage.Controls.Add(this.label3);
            this.dataInstanceTabPage.Controls.Add(this.classListView);
            resources.ApplyResources(this.dataInstanceTabPage, "dataInstanceTabPage");
            this.dataInstanceTabPage.Name = "dataInstanceTabPage";
            this.dataInstanceTabPage.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // classListView
            // 
            resources.ApplyResources(this.classListView, "classListView");
            this.classListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.captionColumnHeader,
            this.nameColumnHeader});
            this.classListView.FullRowSelect = true;
            this.classListView.HideSelection = false;
            this.classListView.MultiSelect = false;
            this.classListView.Name = "classListView";
            this.classListView.SmallImageList = this.listViewImageList;
            this.classListView.UseCompatibleStateImageBehavior = false;
            this.classListView.View = System.Windows.Forms.View.Details;
            this.classListView.SelectedIndexChanged += new System.EventHandler(this.classListView_SelectedIndexChanged);
            // 
            // captionColumnHeader
            // 
            resources.ApplyResources(this.captionColumnHeader, "captionColumnHeader");
            // 
            // nameColumnHeader
            // 
            resources.ApplyResources(this.nameColumnHeader, "nameColumnHeader");
            // 
            // listViewImageList
            // 
            this.listViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewImageList.ImageStream")));
            this.listViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewImageList.Images.SetKeyName(0, "");
            this.listViewImageList.Images.SetKeyName(1, "");
            this.listViewImageList.Images.SetKeyName(2, "");
            this.listViewImageList.Images.SetKeyName(3, "virtualproperty.GIF");
            // 
            // parameterTabPage
            // 
            this.parameterTabPage.Controls.Add(this.parameterListBox);
            resources.ApplyResources(this.parameterTabPage, "parameterTabPage");
            this.parameterTabPage.Name = "parameterTabPage";
            this.parameterTabPage.UseVisualStyleBackColor = true;
            // 
            // parameterListBox
            // 
            resources.ApplyResources(this.parameterListBox, "parameterListBox");
            this.parameterListBox.FormattingEnabled = true;
            this.parameterListBox.Name = "parameterListBox";
            this.parameterListBox.SelectedIndexChanged += new System.EventHandler(this.parameterListBox_SelectedIndexChanged);
            // 
            // activityPropertyTabPage
            // 
            this.activityPropertyTabPage.Controls.Add(this.propertyListBox);
            this.activityPropertyTabPage.Controls.Add(this.label2);
            this.activityPropertyTabPage.Controls.Add(this.activityComboBox);
            this.activityPropertyTabPage.Controls.Add(this.label1);
            resources.ApplyResources(this.activityPropertyTabPage, "activityPropertyTabPage");
            this.activityPropertyTabPage.Name = "activityPropertyTabPage";
            this.activityPropertyTabPage.UseVisualStyleBackColor = true;
            // 
            // propertyListBox
            // 
            resources.ApplyResources(this.propertyListBox, "propertyListBox");
            this.propertyListBox.FormattingEnabled = true;
            this.propertyListBox.Name = "propertyListBox";
            this.propertyListBox.SelectedIndexChanged += new System.EventHandler(this.propertyListBox_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // activityComboBox
            // 
            resources.ApplyResources(this.activityComboBox, "activityComboBox");
            this.activityComboBox.FormattingEnabled = true;
            this.activityComboBox.Name = "activityComboBox";
            this.activityComboBox.SelectedIndexChanged += new System.EventHandler(this.activityComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SelectParameterBindingDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SelectParameterBindingDialog";
            this.Load += new System.EventHandler(this.SelectParameterBindingDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.dataInstanceTabPage.ResumeLayout(false);
            this.dataInstanceTabPage.PerformLayout();
            this.parameterTabPage.ResumeLayout(false);
            this.activityPropertyTabPage.ResumeLayout(false);
            this.activityPropertyTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage dataInstanceTabPage;
        private System.Windows.Forms.TabPage parameterTabPage;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabPage activityPropertyTabPage;
        private System.Windows.Forms.ListView classListView;
        private System.Windows.Forms.ListBox parameterListBox;
        private System.Windows.Forms.ComboBox activityComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox propertyListBox;
        private System.Windows.Forms.ImageList listViewImageList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader captionColumnHeader;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
    }
}
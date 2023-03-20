namespace Newtera.WindowsControl
{
    partial class ResultDataControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultDataControl));
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.resultControlPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.countTextBox = new System.Windows.Forms.TextBox();
            this.countRowButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.nextRowButton = new System.Windows.Forms.Button();
            this.prevRowButton = new System.Windows.Forms.Button();
            this.nextPageButton = new System.Windows.Forms.Button();
            this.prevPageButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.resultControlPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            this.dataGrid1.AccessibleDescription = null;
            this.dataGrid1.AccessibleName = null;
            this.dataGrid1.AlternatingBackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.dataGrid1, "dataGrid1");
            this.dataGrid1.BackgroundImage = null;
            this.dataGrid1.CaptionFont = null;
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Name = "dataGrid1";
            this.toolTip1.SetToolTip(this.dataGrid1, resources.GetString("dataGrid1.ToolTip"));
            this.dataGrid1.Navigate += new System.Windows.Forms.NavigateEventHandler(this.dataGrid1_Navigate);
            this.dataGrid1.Click += new System.EventHandler(this.dataGrid1_Click);
            // 
            // resultControlPanel
            // 
            this.resultControlPanel.AccessibleDescription = null;
            this.resultControlPanel.AccessibleName = null;
            resources.ApplyResources(this.resultControlPanel, "resultControlPanel");
            this.resultControlPanel.BackgroundImage = null;
            this.resultControlPanel.Controls.Add(this.saveButton);
            this.resultControlPanel.Controls.Add(this.countTextBox);
            this.resultControlPanel.Controls.Add(this.countRowButton);
            this.resultControlPanel.Controls.Add(this.backButton);
            this.resultControlPanel.Controls.Add(this.nextRowButton);
            this.resultControlPanel.Controls.Add(this.prevRowButton);
            this.resultControlPanel.Controls.Add(this.nextPageButton);
            this.resultControlPanel.Controls.Add(this.prevPageButton);
            this.resultControlPanel.Font = null;
            this.resultControlPanel.Name = "resultControlPanel";
            this.toolTip1.SetToolTip(this.resultControlPanel, resources.GetString("resultControlPanel.ToolTip"));
            // 
            // saveButton
            // 
            this.saveButton.AccessibleDescription = null;
            this.saveButton.AccessibleName = null;
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.BackgroundImage = null;
            this.saveButton.Font = null;
            this.saveButton.Name = "saveButton";
            this.toolTip1.SetToolTip(this.saveButton, resources.GetString("saveButton.ToolTip"));
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // countTextBox
            // 
            this.countTextBox.AccessibleDescription = null;
            this.countTextBox.AccessibleName = null;
            resources.ApplyResources(this.countTextBox, "countTextBox");
            this.countTextBox.BackgroundImage = null;
            this.countTextBox.Font = null;
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            this.toolTip1.SetToolTip(this.countTextBox, resources.GetString("countTextBox.ToolTip"));
            // 
            // countRowButton
            // 
            this.countRowButton.AccessibleDescription = null;
            this.countRowButton.AccessibleName = null;
            resources.ApplyResources(this.countRowButton, "countRowButton");
            this.countRowButton.BackgroundImage = null;
            this.countRowButton.Font = null;
            this.countRowButton.Name = "countRowButton";
            this.toolTip1.SetToolTip(this.countRowButton, resources.GetString("countRowButton.ToolTip"));
            this.countRowButton.Click += new System.EventHandler(this.countRowButton_Click);
            // 
            // backButton
            // 
            this.backButton.AccessibleDescription = null;
            this.backButton.AccessibleName = null;
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.BackgroundImage = null;
            this.backButton.Font = null;
            this.backButton.Name = "backButton";
            this.toolTip1.SetToolTip(this.backButton, resources.GetString("backButton.ToolTip"));
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextRowButton
            // 
            this.nextRowButton.AccessibleDescription = null;
            this.nextRowButton.AccessibleName = null;
            resources.ApplyResources(this.nextRowButton, "nextRowButton");
            this.nextRowButton.BackgroundImage = null;
            this.nextRowButton.Font = null;
            this.nextRowButton.Name = "nextRowButton";
            this.toolTip1.SetToolTip(this.nextRowButton, resources.GetString("nextRowButton.ToolTip"));
            this.nextRowButton.Click += new System.EventHandler(this.nextRowButton_Click);
            // 
            // prevRowButton
            // 
            this.prevRowButton.AccessibleDescription = null;
            this.prevRowButton.AccessibleName = null;
            resources.ApplyResources(this.prevRowButton, "prevRowButton");
            this.prevRowButton.BackgroundImage = null;
            this.prevRowButton.Font = null;
            this.prevRowButton.Name = "prevRowButton";
            this.toolTip1.SetToolTip(this.prevRowButton, resources.GetString("prevRowButton.ToolTip"));
            this.prevRowButton.Click += new System.EventHandler(this.prevRowButton_Click);
            // 
            // nextPageButton
            // 
            this.nextPageButton.AccessibleDescription = null;
            this.nextPageButton.AccessibleName = null;
            resources.ApplyResources(this.nextPageButton, "nextPageButton");
            this.nextPageButton.BackgroundImage = null;
            this.nextPageButton.Font = null;
            this.nextPageButton.Name = "nextPageButton";
            this.toolTip1.SetToolTip(this.nextPageButton, resources.GetString("nextPageButton.ToolTip"));
            this.nextPageButton.Click += new System.EventHandler(this.nextPageButton_Click);
            // 
            // prevPageButton
            // 
            this.prevPageButton.AccessibleDescription = null;
            this.prevPageButton.AccessibleName = null;
            resources.ApplyResources(this.prevPageButton, "prevPageButton");
            this.prevPageButton.BackgroundImage = null;
            this.prevPageButton.Font = null;
            this.prevPageButton.Name = "prevPageButton";
            this.toolTip1.SetToolTip(this.prevPageButton, resources.GetString("prevPageButton.ToolTip"));
            this.prevPageButton.Click += new System.EventHandler(this.prevPageButton_Click);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.dataGrid1);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            // 
            // ResultDataControl
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.resultControlPanel);
            this.Font = null;
            this.Name = "ResultDataControl";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.resultControlPanel.ResumeLayout(false);
            this.resultControlPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.Panel resultControlPanel;
        private System.Windows.Forms.TextBox countTextBox;
        private System.Windows.Forms.Button countRowButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button nextRowButton;
        private System.Windows.Forms.Button prevRowButton;
        private System.Windows.Forms.Button nextPageButton;
        private System.Windows.Forms.Button prevPageButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
    }
}

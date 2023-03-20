namespace Newtera.Studio
{
    partial class DefineAutoHierarchyDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefineAutoHierarchyDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.hierarchyLevelTreeView = new System.Windows.Forms.TreeView();
            this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.levelPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.label3 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.levelNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.clearButton = new System.Windows.Forms.Button();
            this.previewButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.staticLevelTabPage1 = new System.Windows.Forms.TabPage();
            this.dynamicLevelTabPage = new System.Windows.Forms.TabPage();
            this.dynamicLevelPropertyGrid = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.staticLevelTabPage1.SuspendLayout();
            this.dynamicLevelTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // hierarchyLevelTreeView
            // 
            resources.ApplyResources(this.hierarchyLevelTreeView, "hierarchyLevelTreeView");
            this.hierarchyLevelTreeView.ImageList = this.treeViewImageList;
            this.hierarchyLevelTreeView.Name = "hierarchyLevelTreeView";
            this.hierarchyLevelTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.hierarchyLevelTreeView_AfterSelect);
            // 
            // treeViewImageList
            // 
            this.treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImageList.ImageStream")));
            this.treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewImageList.Images.SetKeyName(0, "");
            this.treeViewImageList.Images.SetKeyName(1, "");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // levelPropertyGrid
            // 
            resources.ApplyResources(this.levelPropertyGrid, "levelPropertyGrid");
            this.levelPropertyGrid.Name = "levelPropertyGrid";
            this.levelPropertyGrid.ToolbarVisible = false;
            this.levelPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.levelPropertyGrid_PropertyValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
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
            // levelNumericUpDown
            // 
            resources.ApplyResources(this.levelNumericUpDown, "levelNumericUpDown");
            this.levelNumericUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.levelNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.levelNumericUpDown.Name = "levelNumericUpDown";
            this.levelNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.levelNumericUpDown.ValueChanged += new System.EventHandler(this.levelNumericUpDown_ValueChanged);
            // 
            // clearButton
            // 
            resources.ApplyResources(this.clearButton, "clearButton");
            this.clearButton.Name = "clearButton";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // previewButton
            // 
            resources.ApplyResources(this.previewButton, "previewButton");
            this.previewButton.Name = "previewButton";
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.staticLevelTabPage1);
            this.tabControl1.Controls.Add(this.dynamicLevelTabPage);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // staticLevelTabPage1
            // 
            this.staticLevelTabPage1.Controls.Add(this.label1);
            this.staticLevelTabPage1.Controls.Add(this.label3);
            this.staticLevelTabPage1.Controls.Add(this.hierarchyLevelTreeView);
            this.staticLevelTabPage1.Controls.Add(this.levelNumericUpDown);
            this.staticLevelTabPage1.Controls.Add(this.levelPropertyGrid);
            this.staticLevelTabPage1.Controls.Add(this.label2);
            resources.ApplyResources(this.staticLevelTabPage1, "staticLevelTabPage1");
            this.staticLevelTabPage1.Name = "staticLevelTabPage1";
            this.staticLevelTabPage1.UseVisualStyleBackColor = true;
            // 
            // dynamicLevelTabPage
            // 
            this.dynamicLevelTabPage.Controls.Add(this.dynamicLevelPropertyGrid);
            resources.ApplyResources(this.dynamicLevelTabPage, "dynamicLevelTabPage");
            this.dynamicLevelTabPage.Name = "dynamicLevelTabPage";
            this.dynamicLevelTabPage.UseVisualStyleBackColor = true;
            // 
            // dynamicLevelPropertyGrid
            // 
            resources.ApplyResources(this.dynamicLevelPropertyGrid, "dynamicLevelPropertyGrid");
            this.dynamicLevelPropertyGrid.Name = "dynamicLevelPropertyGrid";
            this.dynamicLevelPropertyGrid.ToolbarVisible = false;
            // 
            // DefineAutoHierarchyDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.previewButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DefineAutoHierarchyDialog";
            this.Load += new System.EventHandler(this.DefineAutoHierarchyDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.staticLevelTabPage1.ResumeLayout(false);
            this.staticLevelTabPage1.PerformLayout();
            this.dynamicLevelTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView hierarchyLevelTreeView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PropertyGrid levelPropertyGrid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.NumericUpDown levelNumericUpDown;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button previewButton;
        private System.Windows.Forms.ImageList treeViewImageList;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage staticLevelTabPage1;
        private System.Windows.Forms.TabPage dynamicLevelTabPage;
        private System.Windows.Forms.PropertyGrid dynamicLevelPropertyGrid;
    }
}
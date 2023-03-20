namespace WorkflowStudio
{
    partial class EditDataInstanceDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditDataInstanceDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.classTextBox = new System.Windows.Forms.TextBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.parameterListViewEx = new Newtera.WindowsControl.ListViewEx();
            this.parametercolumnHeader = new System.Windows.Forms.ColumnHeader();
            this.attributeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // classTextBox
            // 
            resources.ApplyResources(this.classTextBox, "classTextBox");
            this.classTextBox.Name = "classTextBox";
            this.classTextBox.ReadOnly = true;
            // 
            // propertyGrid
            // 
            resources.ApplyResources(this.propertyGrid, "propertyGrid");
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ToolbarVisible = false;
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
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.propertyGrid);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.parameterListViewEx);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // parameterListViewEx
            // 
            this.parameterListViewEx.AllowColumnReorder = true;
            this.parameterListViewEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.parametercolumnHeader,
            this.attributeColumnHeader});
            resources.ApplyResources(this.parameterListViewEx, "parameterListViewEx");
            this.parameterListViewEx.DoubleClickActivation = false;
            this.parameterListViewEx.FullRowSelect = true;
            this.parameterListViewEx.GridLines = true;
            this.parameterListViewEx.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.parameterListViewEx.Name = "parameterListViewEx";
            this.parameterListViewEx.UseCompatibleStateImageBehavior = false;
            this.parameterListViewEx.View = System.Windows.Forms.View.Details;
            this.parameterListViewEx.SubItemClicked += new Newtera.WindowsControl.SubItemEventHandler(this.parameterListViewEx_SubItemClicked);
            this.parameterListViewEx.SubItemEndEditing += new Newtera.WindowsControl.SubItemEndEditingEventHandler(this.parameterListViewEx_SubItemEndEditing);
            // 
            // parametercolumnHeader
            // 
            resources.ApplyResources(this.parametercolumnHeader, "parametercolumnHeader");
            // 
            // attributeColumnHeader
            // 
            resources.ApplyResources(this.attributeColumnHeader, "attributeColumnHeader");
            // 
            // EditDataInstanceDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.classTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "EditDataInstanceDialog";
            this.Load += new System.EventHandler(this.EditDataInstanceDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox classTextBox;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Newtera.WindowsControl.ListViewEx parameterListViewEx;
        private System.Windows.Forms.ColumnHeader parametercolumnHeader;
        private System.Windows.Forms.ColumnHeader attributeColumnHeader;
    }
}
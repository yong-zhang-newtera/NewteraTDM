﻿namespace SmartExcel2013
{
    partial class NavigationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigationControl));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setupServerURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.smartWordLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.getAllInstancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classTreeGroupBox = new System.Windows.Forms.GroupBox();
            this.classTreeView = new System.Windows.Forms.TreeView();
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.schemaViewGroupBox = new System.Windows.Forms.GroupBox();
            this.xmlSchemaListView = new System.Windows.Forms.ListView();
            this.captionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.loadSchemaViewButton = new System.Windows.Forms.Button();
            this.loadXMLButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.databaseComboBox = new System.Windows.Forms.ComboBox();
            this.instanceGroupBox = new System.Windows.Forms.GroupBox();
            this.resultDataControl = new Newtera.WindowsControl.ResultDataControl();
            this.showInstanceButton = new System.Windows.Forms.Button();
            this.SearchButton = new System.Windows.Forms.Button();
            this.detailButton = new System.Windows.Forms.Button();
            this.removeCusomizationButton = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.classTreeGroupBox.SuspendLayout();
            this.schemaViewGroupBox.SuspendLayout();
            this.instanceGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupServerURLToolStripMenuItem,
            this.toolStripMenuItem1,
            this.smartWordLicenseToolStripMenuItem,
            this.toolStripMenuItem2,
            this.getAllInstancesToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            // 
            // setupServerURLToolStripMenuItem
            // 
            resources.ApplyResources(this.setupServerURLToolStripMenuItem, "setupServerURLToolStripMenuItem");
            this.setupServerURLToolStripMenuItem.Name = "setupServerURLToolStripMenuItem";
            this.setupServerURLToolStripMenuItem.Click += new System.EventHandler(this.setupServerURLToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // smartWordLicenseToolStripMenuItem
            // 
            resources.ApplyResources(this.smartWordLicenseToolStripMenuItem, "smartWordLicenseToolStripMenuItem");
            this.smartWordLicenseToolStripMenuItem.Name = "smartWordLicenseToolStripMenuItem";
            this.smartWordLicenseToolStripMenuItem.Click += new System.EventHandler(this.smartWordLicenseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // getAllInstancesToolStripMenuItem
            // 
            resources.ApplyResources(this.getAllInstancesToolStripMenuItem, "getAllInstancesToolStripMenuItem");
            this.getAllInstancesToolStripMenuItem.Name = "getAllInstancesToolStripMenuItem";
            this.getAllInstancesToolStripMenuItem.Click += new System.EventHandler(this.getAllInstancesToolStripMenuItem_Click);
            // 
            // classTreeGroupBox
            // 
            resources.ApplyResources(this.classTreeGroupBox, "classTreeGroupBox");
            this.classTreeGroupBox.Controls.Add(this.classTreeView);
            this.classTreeGroupBox.Name = "classTreeGroupBox";
            this.classTreeGroupBox.TabStop = false;
            // 
            // classTreeView
            // 
            resources.ApplyResources(this.classTreeView, "classTreeView");
            this.classTreeView.FullRowSelect = true;
            this.classTreeView.HideSelection = false;
            this.classTreeView.ImageList = this.treeImageList;
            this.classTreeView.Name = "classTreeView";
            this.classTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.classTreeView_AfterSelect);
            // 
            // treeImageList
            // 
            this.treeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImageList.ImageStream")));
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeImageList.Images.SetKeyName(0, "");
            this.treeImageList.Images.SetKeyName(1, "");
            this.treeImageList.Images.SetKeyName(2, "");
            this.treeImageList.Images.SetKeyName(3, "");
            this.treeImageList.Images.SetKeyName(4, "");
            this.treeImageList.Images.SetKeyName(5, "");
            this.treeImageList.Images.SetKeyName(6, "");
            this.treeImageList.Images.SetKeyName(7, "");
            this.treeImageList.Images.SetKeyName(8, "");
            this.treeImageList.Images.SetKeyName(9, "");
            this.treeImageList.Images.SetKeyName(10, "");
            this.treeImageList.Images.SetKeyName(11, "");
            this.treeImageList.Images.SetKeyName(12, "");
            this.treeImageList.Images.SetKeyName(13, "");
            this.treeImageList.Images.SetKeyName(14, "");
            // 
            // schemaViewGroupBox
            // 
            resources.ApplyResources(this.schemaViewGroupBox, "schemaViewGroupBox");
            this.schemaViewGroupBox.Controls.Add(this.xmlSchemaListView);
            this.schemaViewGroupBox.Name = "schemaViewGroupBox";
            this.schemaViewGroupBox.TabStop = false;
            // 
            // xmlSchemaListView
            // 
            resources.ApplyResources(this.xmlSchemaListView, "xmlSchemaListView");
            this.xmlSchemaListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.captionColumnHeader,
            this.nameColumnHeader});
            this.xmlSchemaListView.FullRowSelect = true;
            this.xmlSchemaListView.HideSelection = false;
            this.xmlSchemaListView.Name = "xmlSchemaListView";
            this.xmlSchemaListView.SmallImageList = this.listViewImageList;
            this.xmlSchemaListView.UseCompatibleStateImageBehavior = false;
            this.xmlSchemaListView.View = System.Windows.Forms.View.Details;
            this.xmlSchemaListView.SelectedIndexChanged += new System.EventHandler(this.attributeListView_SelectedIndexChanged);
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
            // loadSchemaViewButton
            // 
            resources.ApplyResources(this.loadSchemaViewButton, "loadSchemaViewButton");
            this.loadSchemaViewButton.Name = "loadSchemaViewButton";
            this.loadSchemaViewButton.UseVisualStyleBackColor = true;
            this.loadSchemaViewButton.Click += new System.EventHandler(this.loadSchemaViewButton_Click);
            // 
            // loadXMLButton
            // 
            resources.ApplyResources(this.loadXMLButton, "loadXMLButton");
            this.loadXMLButton.Name = "loadXMLButton";
            this.loadXMLButton.UseVisualStyleBackColor = true;
            this.loadXMLButton.Click += new System.EventHandler(this.loadXMLButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // databaseComboBox
            // 
            resources.ApplyResources(this.databaseComboBox, "databaseComboBox");
            this.databaseComboBox.FormattingEnabled = true;
            this.databaseComboBox.Name = "databaseComboBox";
            this.databaseComboBox.SelectedIndexChanged += new System.EventHandler(this.databaseComboBox_SelectedIndexChanged);
            this.databaseComboBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.databaseComboBox_MouseDown);
            // 
            // instanceGroupBox
            // 
            resources.ApplyResources(this.instanceGroupBox, "instanceGroupBox");
            this.instanceGroupBox.Controls.Add(this.resultDataControl);
            this.instanceGroupBox.Name = "instanceGroupBox";
            this.instanceGroupBox.TabStop = false;
            // 
            // resultDataControl
            // 
            resources.ApplyResources(this.resultDataControl, "resultDataControl");
            this.resultDataControl.CurrentSlide = null;
            this.resultDataControl.MenuItemStates = null;
            this.resultDataControl.MetaData = null;
            this.resultDataControl.Name = "resultDataControl";
            this.resultDataControl.ServerProxy = null;
            this.resultDataControl.UserManager = null;
            this.resultDataControl.UserName = null;
            this.resultDataControl.RowSelectedIndexChangedEvent += new System.EventHandler(this.resultDataControl_RowSelectedIndexChangedEvent);
            this.resultDataControl.RequestForCountEvent += new System.EventHandler(this.resultDataControl_RequestForCountEvent);
            this.resultDataControl.RequestForDataEvent += new System.EventHandler(this.resultDataControl_RequestForDataEvent);
            // 
            // showInstanceButton
            // 
            resources.ApplyResources(this.showInstanceButton, "showInstanceButton");
            this.showInstanceButton.Name = "showInstanceButton";
            this.showInstanceButton.UseVisualStyleBackColor = true;
            this.showInstanceButton.Click += new System.EventHandler(this.showInstanceButton_Click);
            // 
            // SearchButton
            // 
            resources.ApplyResources(this.SearchButton, "SearchButton");
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // detailButton
            // 
            resources.ApplyResources(this.detailButton, "detailButton");
            this.detailButton.Name = "detailButton";
            this.detailButton.UseVisualStyleBackColor = true;
            this.detailButton.Click += new System.EventHandler(this.detailButton_Click);
            // 
            // removeCusomizationButton
            // 
            resources.ApplyResources(this.removeCusomizationButton, "removeCusomizationButton");
            this.removeCusomizationButton.Name = "removeCusomizationButton";
            this.removeCusomizationButton.UseVisualStyleBackColor = true;
            this.removeCusomizationButton.Click += new System.EventHandler(this.removeCusomizationButton_Click);
            // 
            // NavigationControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.removeCusomizationButton);
            this.Controls.Add(this.detailButton);
            this.Controls.Add(this.instanceGroupBox);
            this.Controls.Add(this.databaseComboBox);
            this.Controls.Add(this.showInstanceButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.loadSchemaViewButton);
            this.Controls.Add(this.schemaViewGroupBox);
            this.Controls.Add(this.loadXMLButton);
            this.Controls.Add(this.classTreeGroupBox);
            this.Name = "NavigationControl";
            this.contextMenuStrip.ResumeLayout(false);
            this.classTreeGroupBox.ResumeLayout(false);
            this.schemaViewGroupBox.ResumeLayout(false);
            this.instanceGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.GroupBox classTreeGroupBox;
        private System.Windows.Forms.GroupBox schemaViewGroupBox;
        private System.Windows.Forms.Button loadSchemaViewButton;
        private System.Windows.Forms.Button loadXMLButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox databaseComboBox;
        private System.Windows.Forms.TreeView classTreeView;
        private System.Windows.Forms.ListView xmlSchemaListView;
        private System.Windows.Forms.ColumnHeader captionColumnHeader;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.GroupBox instanceGroupBox;
        private Newtera.WindowsControl.ResultDataControl resultDataControl;
        private System.Windows.Forms.Button showInstanceButton;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button detailButton;
        private System.Windows.Forms.ImageList listViewImageList;
        private System.Windows.Forms.ToolStripMenuItem setupServerURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem smartWordLicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem getAllInstancesToolStripMenuItem;
        private System.Windows.Forms.ImageList treeImageList;
        private System.Windows.Forms.Button removeCusomizationButton;
    }
}

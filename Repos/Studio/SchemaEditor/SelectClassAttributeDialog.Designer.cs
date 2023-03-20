namespace Newtera.Studio
{
    partial class SelectClassAttributeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectClassAttributeDialog));
            this.attributeListView = new System.Windows.Forms.ListView();
            this.captionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.listViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // attributeListView
            // 
            this.attributeListView.AccessibleDescription = null;
            this.attributeListView.AccessibleName = null;
            resources.ApplyResources(this.attributeListView, "attributeListView");
            this.attributeListView.BackgroundImage = null;
            this.attributeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.captionColumnHeader,
            this.nameColumnHeader});
            this.attributeListView.Font = null;
            this.attributeListView.FullRowSelect = true;
            this.attributeListView.HideSelection = false;
            this.attributeListView.MultiSelect = false;
            this.attributeListView.Name = "attributeListView";
            this.attributeListView.SmallImageList = this.listViewImageList;
            this.attributeListView.UseCompatibleStateImageBehavior = false;
            this.attributeListView.View = System.Windows.Forms.View.Details;
            this.attributeListView.SelectedIndexChanged += new System.EventHandler(this.attributeListView_SelectedIndexChanged);
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
            // SelectClassAttributeDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.attributeListView);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "SelectClassAttributeDialog";
            this.Load += new System.EventHandler(this.SelectClassAttributeDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView attributeListView;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ColumnHeader captionColumnHeader;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ImageList listViewImageList;
    }
}
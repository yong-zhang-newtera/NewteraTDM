namespace Newtera.Studio.UserControls
{
    partial class DataViewSortControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataViewSortControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sortDirectionComboBox = new System.Windows.Forms.ComboBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.captionTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.removeSortButton = new System.Windows.Forms.Button();
            this.addSortButton = new System.Windows.Forms.Button();
            this.sortAttributesListView = new System.Windows.Forms.ListView();
            this.resultCaptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.resultNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.resultClassColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.smallIconImageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sortDirectionComboBox);
            this.groupBox1.Controls.Add(this.descriptionTextBox);
            this.groupBox1.Controls.Add(this.captionTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.removeSortButton);
            this.groupBox1.Controls.Add(this.addSortButton);
            this.groupBox1.Controls.Add(this.sortAttributesListView);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // sortDirectionComboBox
            // 
            this.sortDirectionComboBox.FormattingEnabled = true;
            this.sortDirectionComboBox.Items.AddRange(new object[] {
            resources.GetString("sortDirectionComboBox.Items"),
            resources.GetString("sortDirectionComboBox.Items1")});
            resources.ApplyResources(this.sortDirectionComboBox, "sortDirectionComboBox");
            this.sortDirectionComboBox.Name = "sortDirectionComboBox";
            this.sortDirectionComboBox.SelectedIndexChanged += new System.EventHandler(this.sortDirectionComboBox_SelectedIndexChanged);
            // 
            // descriptionTextBox
            // 
            resources.ApplyResources(this.descriptionTextBox, "descriptionTextBox");
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ReadOnly = true;
            // 
            // captionTextBox
            // 
            resources.ApplyResources(this.captionTextBox, "captionTextBox");
            this.captionTextBox.Name = "captionTextBox";
            this.captionTextBox.ReadOnly = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // removeSortButton
            // 
            resources.ApplyResources(this.removeSortButton, "removeSortButton");
            this.removeSortButton.Name = "removeSortButton";
            this.removeSortButton.Click += new System.EventHandler(this.removeSortButton_Click);
            // 
            // addSortButton
            // 
            resources.ApplyResources(this.addSortButton, "addSortButton");
            this.addSortButton.Name = "addSortButton";
            this.addSortButton.Click += new System.EventHandler(this.addSortButton_Click);
            // 
            // sortAttributesListView
            // 
            resources.ApplyResources(this.sortAttributesListView, "sortAttributesListView");
            this.sortAttributesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.resultCaptionColumnHeader,
            this.resultNameColumnHeader,
            this.resultClassColumnHeader});
            this.sortAttributesListView.FullRowSelect = true;
            this.sortAttributesListView.HideSelection = false;
            this.sortAttributesListView.MultiSelect = false;
            this.sortAttributesListView.Name = "sortAttributesListView";
            this.sortAttributesListView.SmallImageList = this.smallIconImageList;
            this.sortAttributesListView.UseCompatibleStateImageBehavior = false;
            this.sortAttributesListView.View = System.Windows.Forms.View.Details;
            this.sortAttributesListView.SelectedIndexChanged += new System.EventHandler(this.sortAttributesListView_SelectedIndexChanged);
            // 
            // resultCaptionColumnHeader
            // 
            resources.ApplyResources(this.resultCaptionColumnHeader, "resultCaptionColumnHeader");
            // 
            // resultNameColumnHeader
            // 
            resources.ApplyResources(this.resultNameColumnHeader, "resultNameColumnHeader");
            // 
            // resultClassColumnHeader
            // 
            resources.ApplyResources(this.resultClassColumnHeader, "resultClassColumnHeader");
            // 
            // smallIconImageList
            // 
            this.smallIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallIconImageList.ImageStream")));
            this.smallIconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.smallIconImageList.Images.SetKeyName(0, "");
            this.smallIconImageList.Images.SetKeyName(1, "");
            this.smallIconImageList.Images.SetKeyName(2, "");
            this.smallIconImageList.Images.SetKeyName(3, "");
            this.smallIconImageList.Images.SetKeyName(4, "");
            this.smallIconImageList.Images.SetKeyName(5, "virtualproperty.GIF");
            // 
            // DataViewSortControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "DataViewSortControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView sortAttributesListView;
        private System.Windows.Forms.ColumnHeader resultCaptionColumnHeader;
        private System.Windows.Forms.ColumnHeader resultNameColumnHeader;
        private System.Windows.Forms.ColumnHeader resultClassColumnHeader;
        private System.Windows.Forms.ImageList smallIconImageList;
        private System.Windows.Forms.Button addSortButton;
        private System.Windows.Forms.Button removeSortButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox sortDirectionComboBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.TextBox captionTextBox;
    }
}

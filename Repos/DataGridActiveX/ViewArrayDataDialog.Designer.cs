namespace Newtera.DataGridActiveX
{
    partial class ViewArrayDataDialog
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
            this.dataGridControl = new Newtera.DataGridActiveX.DataGridControl();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dataGridControl
            // 
            this.dataGridControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridControl.ArrayCaption = null;
            this.dataGridControl.ArrayName = null;
            this.dataGridControl.BasePath = null;
            this.dataGridControl.BaseURL = null;
            this.dataGridControl.ClassCaption = null;
            this.dataGridControl.ClassName = null;
            this.dataGridControl.ConnectionString = null;
            this.dataGridControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridControl.InstanceId = null;
            this.dataGridControl.IsWebHosted = true;
            this.dataGridControl.Location = new System.Drawing.Point(12, 12);
            this.dataGridControl.Name = "dataGridControl";
            this.dataGridControl.RelatedClassAlias = null;
            this.dataGridControl.RelatedClassCaption = null;
            this.dataGridControl.RelatedClassName = null;
            this.dataGridControl.Size = new System.Drawing.Size(465, 287);
            this.dataGridControl.TabIndex = 0;
            this.dataGridControl.TaxonCaption = null;
            this.dataGridControl.TaxonName = null;
            this.dataGridControl.TaxonomyName = null;
            this.dataGridControl.TotalCount = 0;
            this.dataGridControl.ViewType = "CLASS";
            this.dataGridControl.XQuery = null;
            this.dataGridControl.ShowPivotGridEvent += new Newtera.DataGridActiveX.ShowPivotGridCallback(this.dataGridControl_ShowPivotGridEvent);
            this.dataGridControl.GraphEvent += new Newtera.DataGridActiveX.GraphCallback(this.dataGridControl_GraphEvent);
            this.dataGridControl.DownloadFileEvent += new Newtera.DataGridActiveX.DownloadFileCallback(this.dataGridControl_DownloadFileEvent);
            this.dataGridControl.RunAlgorithmEvent += new Newtera.DataGridActiveX.RunAlgorithmCallback(this.dataGridControl_RunAlgorithmEvent);
            this.dataGridControl.DownloadGraphEvent += new Newtera.DataGridActiveX.DownloadGraphCallback(this.dataGridControl_DownloadGraphEvent);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(402, 305);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ViewArrayDataDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 340);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.dataGridControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ViewArrayDataDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "查看数组数据";
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridControl dataGridControl;
        private System.Windows.Forms.Button cancelButton;
    }
}
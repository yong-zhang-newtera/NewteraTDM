namespace Newtera.DataGridActiveX
{
    partial class ShowTemplatesDialog
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
            this.chartTemplatesListBox = new System.Windows.Forms.ListBox();
            this.templateDescTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.viewChartButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.downLoadChartButton = new System.Windows.Forms.Button();
            this.chartFormatComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartTemplatesListBox
            // 
            this.chartTemplatesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chartTemplatesListBox.FormattingEnabled = true;
            this.chartTemplatesListBox.Location = new System.Drawing.Point(13, 13);
            this.chartTemplatesListBox.Name = "chartTemplatesListBox";
            this.chartTemplatesListBox.Size = new System.Drawing.Size(311, 121);
            this.chartTemplatesListBox.TabIndex = 0;
            this.chartTemplatesListBox.SelectedIndexChanged += new System.EventHandler(this.chartTemplatesListBox_SelectedIndexChanged);
            // 
            // templateDescTextBox
            // 
            this.templateDescTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.templateDescTextBox.Location = new System.Drawing.Point(13, 140);
            this.templateDescTextBox.Multiline = true;
            this.templateDescTextBox.Name = "templateDescTextBox";
            this.templateDescTextBox.ReadOnly = true;
            this.templateDescTextBox.Size = new System.Drawing.Size(311, 43);
            this.templateDescTextBox.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.viewChartButton);
            this.groupBox1.Location = new System.Drawing.Point(13, 189);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 49);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "在线绘图";
            // 
            // viewChartButton
            // 
            this.viewChartButton.Enabled = false;
            this.viewChartButton.Location = new System.Drawing.Point(119, 19);
            this.viewChartButton.Name = "viewChartButton";
            this.viewChartButton.Size = new System.Drawing.Size(75, 23);
            this.viewChartButton.TabIndex = 0;
            this.viewChartButton.Text = "显示图形";
            this.viewChartButton.UseVisualStyleBackColor = true;
            this.viewChartButton.Click += new System.EventHandler(this.viewChartButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.downLoadChartButton);
            this.groupBox2.Controls.Add(this.chartFormatComboBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(13, 244);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 68);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "离线绘图";
            // 
            // downLoadChartButton
            // 
            this.downLoadChartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downLoadChartButton.Enabled = false;
            this.downLoadChartButton.Location = new System.Drawing.Point(230, 28);
            this.downLoadChartButton.Name = "downLoadChartButton";
            this.downLoadChartButton.Size = new System.Drawing.Size(75, 23);
            this.downLoadChartButton.TabIndex = 2;
            this.downLoadChartButton.Text = "下载图形";
            this.downLoadChartButton.UseVisualStyleBackColor = true;
            this.downLoadChartButton.Click += new System.EventHandler(this.downLoadChartButton_Click);
            // 
            // chartFormatComboBox
            // 
            this.chartFormatComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chartFormatComboBox.FormattingEnabled = true;
            this.chartFormatComboBox.Location = new System.Drawing.Point(77, 28);
            this.chartFormatComboBox.Name = "chartFormatComboBox";
            this.chartFormatComboBox.Size = new System.Drawing.Size(147, 21);
            this.chartFormatComboBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "图形格式:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(249, 318);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ShowTemplatesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 350);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.templateDescTextBox);
            this.Controls.Add(this.chartTemplatesListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ShowTemplatesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择绘图模板";
            this.Load += new System.EventHandler(this.ShowTemplatesDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox chartTemplatesListBox;
        private System.Windows.Forms.TextBox templateDescTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button viewChartButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox chartFormatComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button downLoadChartButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.DataGridActiveX.DataGridView;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// SearchDialog ��ժҪ˵����
	/// </summary>
	public class SearchDialog : System.Windows.Forms.Form
	{
		private DataGridViewModel _dataGridView;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Panel panel1;
		private Newtera.DataGridActiveX.SearchExprBuilder searchExprBuilder;
        private Label msgLabel;
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SearchDialog()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
			_dataGridView = null;
            this.searchExprBuilder.MessageLable = this.msgLabel;
		}

		/// <summary>
		/// Gets or sets the DataGridViewModel
		/// </summary>
		public DataGridViewModel DataGridView
		{
			get
			{
				return _dataGridView;
			}
			set
			{
				_dataGridView = value;
			}
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchExprBuilder = new Newtera.DataGridActiveX.SearchExprBuilder();
            this.msgLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.searchExprBuilder);
            this.panel1.Name = "panel1";
            // 
            // searchExprBuilder
            // 
            this.searchExprBuilder.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.searchExprBuilder.DataGridView = null;
            resources.ApplyResources(this.searchExprBuilder, "searchExprBuilder");
            this.searchExprBuilder.MessageLable = null;
            this.searchExprBuilder.Name = "searchExprBuilder";
            this.searchExprBuilder.PopupClosed += new System.EventHandler(this.searchExprBuilder_PopupClosed);
            // 
            // msgLabel
            // 
            resources.ApplyResources(this.msgLabel, "msgLabel");
            this.msgLabel.ForeColor = System.Drawing.Color.Red;
            this.msgLabel.Name = "msgLabel";
            // 
            // SearchDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.msgLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchDialog";
            this.Load += new System.EventHandler(this.SearchDialog_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void SearchDialog_Load(object sender, System.EventArgs e)
		{
			if (_dataGridView != null)
			{
				this.searchExprBuilder.DataGridView = _dataGridView;
			}
		}

		private void searchExprBuilder_PopupClosed(object sender, System.EventArgs e)
		{
			this.okButton.Focus();
		}
	}
}

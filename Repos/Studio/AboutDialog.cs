using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// AboutDialog ��ժҪ˵����
	/// </summary>
	public class AboutDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.RichTextBox aboutRichTextBox;
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutDialog()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.aboutRichTextBox = new System.Windows.Forms.RichTextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // aboutRichTextBox
            // 
            resources.ApplyResources(this.aboutRichTextBox, "aboutRichTextBox");
            this.aboutRichTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.aboutRichTextBox.Name = "aboutRichTextBox";
            this.aboutRichTextBox.ReadOnly = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // AboutDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.aboutRichTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.Load += new System.EventHandler(this.AboutDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void AboutDialog_Load(object sender, System.EventArgs e)
		{
			string fileName = AppDomain.CurrentDomain.BaseDirectory + MessageResourceManager.GetString("DesignStudio.About");

			if (fileName != null)
			{
				aboutRichTextBox.LoadFile(fileName);
			}
		}
	}
}

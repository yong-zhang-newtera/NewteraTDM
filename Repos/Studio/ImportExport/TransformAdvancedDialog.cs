using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Mappings.Transform;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// TransformAdvancedDialog ��ժҪ˵����
	/// </summary>
	public class TransformAdvancedDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton oneToOneRadioButton;
		private System.Windows.Forms.RadioButton allToOneRadioButton;

		private TransformCardinal _transformCardinal = TransformCardinal.OneToOne;

		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TransformAdvancedDialog()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
			SetRadioButtonStatus();
		}

		/// <summary>
		/// Gets or sets the type of transform cardinal
		/// </summary>
		public TransformCardinal TransformCardinalType
		{
			get
			{
				return _transformCardinal;
			}
			set
			{
				_transformCardinal = value;

				SetRadioButtonStatus();
			}
		}

		/// <summary>
		/// Set the radio button status according to the TransformCardinal type
		/// </summary>
		private void SetRadioButtonStatus()
		{
			switch (TransformCardinalType)
			{
				case TransformCardinal.OneToOne:
					oneToOneRadioButton.Checked = true;
					break;

				case TransformCardinal.AllToOne:
					allToOneRadioButton.Checked = true;
					break;

				default:
					oneToOneRadioButton.Checked = true;
					break;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransformAdvancedDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.allToOneRadioButton = new System.Windows.Forms.RadioButton();
            this.oneToOneRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.allToOneRadioButton);
            this.groupBox1.Controls.Add(this.oneToOneRadioButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // allToOneRadioButton
            // 
            resources.ApplyResources(this.allToOneRadioButton, "allToOneRadioButton");
            this.allToOneRadioButton.Name = "allToOneRadioButton";
            // 
            // oneToOneRadioButton
            // 
            resources.ApplyResources(this.oneToOneRadioButton, "oneToOneRadioButton");
            this.oneToOneRadioButton.Name = "oneToOneRadioButton";
            // 
            // TransformAdvancedDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TransformAdvancedDialog";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (oneToOneRadioButton.Checked)
			{
				_transformCardinal = TransformCardinal.OneToOne;
			}
			else if (allToOneRadioButton.Checked)
			{
				_transformCardinal = TransformCardinal.AllToOne;
			}
		}

	}
}

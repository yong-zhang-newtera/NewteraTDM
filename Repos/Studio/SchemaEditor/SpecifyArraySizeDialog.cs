using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// SpecifyArraySizeDialog 的摘要说明。
	/// </summary>
	public class SpecifyArraySizeDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.RadioButton normalSizeRadioButton;
		private System.Windows.Forms.RadioButton overSizeRadioButton;

		private ArraySizeType _arraySize = ArraySizeType.NormalSize;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SpecifyArraySizeDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
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

		/// <summary>
		/// Gets the array size type
		/// </summary>
		/// <value>One of the ArraySizeType enum values.</value>
		public ArraySizeType ArraySize
		{
			get
			{
				return _arraySize;
			}
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecifyArraySizeDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.overSizeRadioButton = new System.Windows.Forms.RadioButton();
            this.normalSizeRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.overSizeRadioButton);
            this.groupBox1.Controls.Add(this.normalSizeRadioButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // overSizeRadioButton
            // 
            resources.ApplyResources(this.overSizeRadioButton, "overSizeRadioButton");
            this.overSizeRadioButton.Name = "overSizeRadioButton";
            // 
            // normalSizeRadioButton
            // 
            this.normalSizeRadioButton.Checked = true;
            resources.ApplyResources(this.normalSizeRadioButton, "normalSizeRadioButton");
            this.normalSizeRadioButton.Name = "normalSizeRadioButton";
            this.normalSizeRadioButton.TabStop = true;
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
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SpecifyArraySizeDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SpecifyArraySizeDialog";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (this.normalSizeRadioButton.Checked)
			{
				_arraySize = ArraySizeType.NormalSize;
			}
			else
			{
				_arraySize = ArraySizeType.OverSize;
			}
		}
	}
}

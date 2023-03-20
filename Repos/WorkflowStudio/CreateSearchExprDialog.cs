using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.DataView;

namespace WorkflowStudio
{
	/// <summary>
	/// Summary description for CreateSearchExprDialog.
	/// </summary>
	public class CreateSearchExprDialog : System.Windows.Forms.Form
	{
		private DataViewModel _dataView;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel panel1;
        private Newtera.WindowsControl.SearchExprBuilder searchExprBuilder;
        private Label msgLabel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CreateSearchExprDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            this.searchExprBuilder.MessageLable = this.msgLabel;
		}

		/// <summary>
		/// Clean up any resources being used.
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
		/// Gets or sets the data view
		/// </summary>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateSearchExprDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchExprBuilder = new Newtera.WindowsControl.SearchExprBuilder();
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
            this.searchExprBuilder.DataView = null;
            resources.ApplyResources(this.searchExprBuilder, "searchExprBuilder");
            this.searchExprBuilder.MessageLable = null;
            this.searchExprBuilder.Name = "searchExprBuilder";
            // 
            // msgLabel
            // 
            resources.ApplyResources(this.msgLabel, "msgLabel");
            this.msgLabel.ForeColor = System.Drawing.Color.Red;
            this.msgLabel.Name = "msgLabel";
            // 
            // CreateSearchExprDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.msgLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateSearchExprDialog";
            this.Load += new System.EventHandler(this.CreateSearchExprDialog_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void CreateSearchExprDialog_Load(object sender, System.EventArgs e)
		{
			if (_dataView != null)
			{
				this.searchExprBuilder.DataView = _dataView;
			}
		}

		private void refClassesButton_Click(object sender, System.EventArgs e)
		{
		}
	}
}

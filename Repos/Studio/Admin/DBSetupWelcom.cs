using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DBSetupWelcom.
	/// </summary>
	public class DBSetupWelcom : System.Windows.Forms.Form
	{
		private WizardButtonTag _buttonTag;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DBSetupWelcom()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_buttonTag = WizardButtonTag.None;
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
		/// Get the tag of a button that is clicked
		/// </summary>
		/// <value>One of WizardButtonTag values</value>
		internal WizardButtonTag ButtonTag
		{
			get
			{
				return this._buttonTag;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBSetupWelcom));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HighlightText;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Tag = "1";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // backButton
            // 
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.backButton.Name = "backButton";
            this.backButton.Tag = "2";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextButton
            // 
            resources.ApplyResources(this.nextButton, "nextButton");
            this.nextButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.nextButton.Name = "nextButton";
            this.nextButton.Tag = "3";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // finishButton
            // 
            resources.ApplyResources(this.finishButton, "finishButton");
            this.finishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.finishButton.Name = "finishButton";
            this.finishButton.Tag = "4";
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // DBSetupWelcom
            // 
            this.AcceptButton = this.nextButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DBSetupWelcom";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Cancel;
		}

		private void backButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Back;
		}

		private void nextButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Next;		
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Finish;		
		}
	}
}

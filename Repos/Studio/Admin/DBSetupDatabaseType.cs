using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DBSetupDatabaseType.
	/// </summary>
	public class DBSetupDatabaseType : System.Windows.Forms.Form
	{
		private WizardButtonTag _buttonTag;
		private DBSetupWizard _wizard;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DBSetupDatabaseType()
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

		/// <summary>
		/// Gets or sets the DB Setup Wizard controller
		/// </summary>
		internal DBSetupWizard Wizard
		{
			get
			{
				return _wizard;
			}
			set
			{
				_wizard = value;
			}
		}

		/// <summary>
		/// Gets the database type chosen by the user
		/// </summary>
		public string DataBaseType
		{
			get
			{
				return (string) this.comboBox1.SelectedItem;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBSetupDatabaseType));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
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
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBox1
            // 
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2")});
            this.comboBox1.Name = "comboBox1";
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
            // DBSetupDatabaseType
            // 
            this.AcceptButton = this.nextButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DBSetupDatabaseType";
            this.Load += new System.EventHandler(this.DBSetupDatabaseType_Load);
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
			this._wizard.DatabaseType = DataBaseType;
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Finish;		
		}

		private void DBSetupDatabaseType_Load(object sender, System.EventArgs e)
		{
			this.comboBox1.SelectedIndex = 0;
		}
	}
}

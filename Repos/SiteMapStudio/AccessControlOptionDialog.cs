using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.XaclModel;

namespace Newtera.SiteMapStudio
{
	/// <summary>
	/// Summary description for AccessControlOptionDialog.
	/// </summary>
	public class AccessControlOptionDialog : System.Windows.Forms.Form
	{
		private XaclConflictResolutionType _conflictResolution;

		private XaclPolicy _policy;

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton dtpRadioButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton gtpRadioButton;
		private System.ComponentModel.IContainer components;

		public AccessControlOptionDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		public XaclPolicy XaclPolicy
		{
			get
			{
				return _policy;
			}
			set
			{
				_policy = value;

				_conflictResolution = value.Setting.ConflictResolutionType;
			}
		}


		/// <summary>
		/// Gets or sets the type of Conflict resolution.
		/// </summary>
		/// <value>One of XaclConflictResolutionType values</value>
		public XaclConflictResolutionType ConflictResolution
		{
			get
			{
				return _conflictResolution;
			}
			set
			{
				_conflictResolution = value;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccessControlOptionDialog));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.gtpRadioButton = new System.Windows.Forms.RadioButton();
            this.dtpRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gtpRadioButton
            // 
            this.gtpRadioButton.AccessibleDescription = null;
            this.gtpRadioButton.AccessibleName = null;
            resources.ApplyResources(this.gtpRadioButton, "gtpRadioButton");
            this.gtpRadioButton.BackgroundImage = null;
            this.gtpRadioButton.Font = null;
            this.gtpRadioButton.Name = "gtpRadioButton";
            this.toolTip.SetToolTip(this.gtpRadioButton, resources.GetString("gtpRadioButton.ToolTip"));
            // 
            // dtpRadioButton
            // 
            this.dtpRadioButton.AccessibleDescription = null;
            this.dtpRadioButton.AccessibleName = null;
            resources.ApplyResources(this.dtpRadioButton, "dtpRadioButton");
            this.dtpRadioButton.BackgroundImage = null;
            this.dtpRadioButton.Checked = true;
            this.dtpRadioButton.Font = null;
            this.dtpRadioButton.Name = "dtpRadioButton";
            this.dtpRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.dtpRadioButton, resources.GetString("dtpRadioButton.ToolTip"));
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.gtpRadioButton);
            this.groupBox2.Controls.Add(this.dtpRadioButton);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            this.toolTip.SetToolTip(this.okButton, resources.GetString("okButton.ToolTip"));
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            this.toolTip.SetToolTip(this.cancelButton, resources.GetString("cancelButton.ToolTip"));
            // 
            // AccessControlOptionDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccessControlOptionDialog";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.AccessControlOptionDialog_Load);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void AccessControlOptionDialog_Load(object sender, System.EventArgs e)
		{
			switch (_conflictResolution)
			{
				case XaclConflictResolutionType.Dtp:
					this.dtpRadioButton.Checked = true;
					break;
				case XaclConflictResolutionType.Gtp:
					this.gtpRadioButton.Checked = true;
					break;
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (this.dtpRadioButton.Checked)
			{
				_conflictResolution = XaclConflictResolutionType.Dtp;
			}
			else if (this.gtpRadioButton.Checked)
			{
				_conflictResolution = XaclConflictResolutionType.Gtp;
			}
		}
	}
}

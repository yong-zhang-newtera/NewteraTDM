using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.XaclModel;

namespace WorkflowStudio
{
	/// <summary>
	/// Summary description for AccessControlOptionDialog.
	/// </summary>
	public class AccessControlOptionDialog : System.Windows.Forms.Form
	{
		private XaclPermissionType _readPermission;
		private XaclConflictResolutionType _conflictResolution;

		private XaclPolicy _policy;

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton dtpRadioButton;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton grantReadRadioButton;
        private System.Windows.Forms.RadioButton denyReadRadioButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.RadioButton gtpRadioButton;
        private System.Windows.Forms.RadioButton ntpRadioButton;
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

		public XaclPolicy Policy
		{
			get
			{
				return _policy;
			}
			set
			{
				_policy = value;

				_readPermission = value.Setting.DefaultReadPermission;
				_conflictResolution = value.Setting.ConflictResolutionType;
			}
		}

		/// <summary>
		/// Gets or sets the default read permission.
		/// </summary>
		/// <value>One of XaclPermissionType values</value>
		public XaclPermissionType DefaultReadPermission
		{
			get
			{
				return _readPermission;
			}
			set
			{
				_readPermission = value;
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
            this.ntpRadioButton = new System.Windows.Forms.RadioButton();
            this.gtpRadioButton = new System.Windows.Forms.RadioButton();
            this.dtpRadioButton = new System.Windows.Forms.RadioButton();
            this.denyReadRadioButton = new System.Windows.Forms.RadioButton();
            this.grantReadRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ntpRadioButton
            // 
            this.ntpRadioButton.AccessibleDescription = null;
            this.ntpRadioButton.AccessibleName = null;
            resources.ApplyResources(this.ntpRadioButton, "ntpRadioButton");
            this.ntpRadioButton.BackgroundImage = null;
            this.ntpRadioButton.Font = null;
            this.ntpRadioButton.Name = "ntpRadioButton";
            this.toolTip.SetToolTip(this.ntpRadioButton, resources.GetString("ntpRadioButton.ToolTip"));
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
            // denyReadRadioButton
            // 
            this.denyReadRadioButton.AccessibleDescription = null;
            this.denyReadRadioButton.AccessibleName = null;
            resources.ApplyResources(this.denyReadRadioButton, "denyReadRadioButton");
            this.denyReadRadioButton.BackgroundImage = null;
            this.denyReadRadioButton.Font = null;
            this.denyReadRadioButton.Name = "denyReadRadioButton";
            this.toolTip.SetToolTip(this.denyReadRadioButton, resources.GetString("denyReadRadioButton.ToolTip"));
            // 
            // grantReadRadioButton
            // 
            this.grantReadRadioButton.AccessibleDescription = null;
            this.grantReadRadioButton.AccessibleName = null;
            resources.ApplyResources(this.grantReadRadioButton, "grantReadRadioButton");
            this.grantReadRadioButton.BackgroundImage = null;
            this.grantReadRadioButton.Checked = true;
            this.grantReadRadioButton.Font = null;
            this.grantReadRadioButton.Name = "grantReadRadioButton";
            this.grantReadRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.grantReadRadioButton, resources.GetString("grantReadRadioButton.ToolTip"));
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.ntpRadioButton);
            this.groupBox2.Controls.Add(this.gtpRadioButton);
            this.groupBox2.Controls.Add(this.dtpRadioButton);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.denyReadRadioButton);
            this.panel2.Controls.Add(this.grantReadRadioButton);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            this.toolTip.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            this.toolTip.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            this.toolTip.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
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
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccessControlOptionDialog";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.AccessControlOptionDialog_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
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
				case XaclConflictResolutionType.Ntp:
					this.ntpRadioButton.Checked = true;
					break;
			}

			if (_readPermission == XaclPermissionType.Grant)
			{
				this.grantReadRadioButton.Checked = true;
			}
			else
			{
				this.denyReadRadioButton.Checked = true;
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
			else if (this.ntpRadioButton.Checked)
			{
				_conflictResolution = XaclConflictResolutionType.Ntp;
			}

			if (this.grantReadRadioButton.Checked)
			{
				_readPermission = XaclPermissionType.Grant;
			}
			else
			{
				_readPermission = XaclPermissionType.Deny;
			}
		}
	}
}

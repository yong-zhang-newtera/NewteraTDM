using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.XaclModel;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for AccessControlOptionDialog.
	/// </summary>
	public class AccessControlOptionDialog : System.Windows.Forms.Form
	{
		private XaclPermissionType _readPermission;
		private XaclPermissionType _writePermission;
		private XaclPermissionType _createPermission;
		private XaclPermissionType _deletePermission;
		private XaclPermissionType _uploadPermission;
		private XaclPermissionType _downloadPermission;
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
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.RadioButton denyWriteRadioButton;
		private System.Windows.Forms.RadioButton grantWriteRadioButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.RadioButton denyCreateRadioButton;
		private System.Windows.Forms.RadioButton grantCreateRadioButton;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.RadioButton denyDeleteRadioButton;
		private System.Windows.Forms.RadioButton grantDeleteRadioButton;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.RadioButton gtpRadioButton;
		private System.Windows.Forms.RadioButton ntpRadioButton;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.RadioButton denyDownloadRadioButton;
		private System.Windows.Forms.RadioButton grantDownloadRadioButton;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.RadioButton denyUploadRadioButton;
		private System.Windows.Forms.RadioButton grantUploadRadioButton;
		private System.Windows.Forms.Label label8;
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

				_readPermission = value.Setting.DefaultReadPermission;
				_writePermission = value.Setting.DefaultWritePermission;
				_createPermission = value.Setting.DefaultCreatePermission;
				_deletePermission = value.Setting.DefaultDeletePermission;
				_uploadPermission = value.Setting.DefaultUploadPermission;
				_downloadPermission = value.Setting.DefaultDownloadPermission;
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
		/// Gets or sets the default write permission.
		/// </summary>
		/// <value>One of XaclPermissionType values</value>
		public XaclPermissionType DefaultWritePermission
		{
			get
			{
				return _writePermission;
			}
			set
			{
				_writePermission = value;
			}
		}

		/// <summary>
		/// Gets or sets the default create permission.
		/// </summary>
		/// <value>One of XaclPermissionType values</value>
		public XaclPermissionType DefaultCreatePermission
		{
			get
			{
				return _createPermission;
			}
			set
			{
				_createPermission = value;
			}
		}

		/// <summary>
		/// Gets or sets the default delete permission.
		/// </summary>
		/// <value>One of XaclPermissionType values</value>
		public XaclPermissionType DefaultDeletePermission
		{
			get
			{
				return _deletePermission;
			}
			set
			{
				_deletePermission = value;
			}
		}

		/// <summary>
		/// Gets or sets the default upload permission.
		/// </summary>
		/// <value>One of XaclPermissionType values</value>
		public XaclPermissionType DefaultUploadPermission
		{
			get
			{
				return _uploadPermission;
			}
			set
			{
				_uploadPermission = value;
			}
		}

		/// <summary>
		/// Gets or sets the default download permission.
		/// </summary>
		/// <value>One of XaclPermissionType values</value>
		public XaclPermissionType DefaultDownloadPermission
		{
			get
			{
				return _downloadPermission;
			}
			set
			{
				_downloadPermission = value;
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
            this.denyUploadRadioButton = new System.Windows.Forms.RadioButton();
            this.grantUploadRadioButton = new System.Windows.Forms.RadioButton();
            this.denyDownloadRadioButton = new System.Windows.Forms.RadioButton();
            this.grantDownloadRadioButton = new System.Windows.Forms.RadioButton();
            this.denyDeleteRadioButton = new System.Windows.Forms.RadioButton();
            this.grantDeleteRadioButton = new System.Windows.Forms.RadioButton();
            this.denyCreateRadioButton = new System.Windows.Forms.RadioButton();
            this.grantCreateRadioButton = new System.Windows.Forms.RadioButton();
            this.denyWriteRadioButton = new System.Windows.Forms.RadioButton();
            this.grantWriteRadioButton = new System.Windows.Forms.RadioButton();
            this.denyReadRadioButton = new System.Windows.Forms.RadioButton();
            this.grantReadRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
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
            // denyUploadRadioButton
            // 
            this.denyUploadRadioButton.AccessibleDescription = null;
            this.denyUploadRadioButton.AccessibleName = null;
            resources.ApplyResources(this.denyUploadRadioButton, "denyUploadRadioButton");
            this.denyUploadRadioButton.BackgroundImage = null;
            this.denyUploadRadioButton.Font = null;
            this.denyUploadRadioButton.Name = "denyUploadRadioButton";
            this.toolTip.SetToolTip(this.denyUploadRadioButton, resources.GetString("denyUploadRadioButton.ToolTip"));
            // 
            // grantUploadRadioButton
            // 
            this.grantUploadRadioButton.AccessibleDescription = null;
            this.grantUploadRadioButton.AccessibleName = null;
            resources.ApplyResources(this.grantUploadRadioButton, "grantUploadRadioButton");
            this.grantUploadRadioButton.BackgroundImage = null;
            this.grantUploadRadioButton.Checked = true;
            this.grantUploadRadioButton.Font = null;
            this.grantUploadRadioButton.Name = "grantUploadRadioButton";
            this.grantUploadRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.grantUploadRadioButton, resources.GetString("grantUploadRadioButton.ToolTip"));
            // 
            // denyDownloadRadioButton
            // 
            this.denyDownloadRadioButton.AccessibleDescription = null;
            this.denyDownloadRadioButton.AccessibleName = null;
            resources.ApplyResources(this.denyDownloadRadioButton, "denyDownloadRadioButton");
            this.denyDownloadRadioButton.BackgroundImage = null;
            this.denyDownloadRadioButton.Font = null;
            this.denyDownloadRadioButton.Name = "denyDownloadRadioButton";
            this.toolTip.SetToolTip(this.denyDownloadRadioButton, resources.GetString("denyDownloadRadioButton.ToolTip"));
            // 
            // grantDownloadRadioButton
            // 
            this.grantDownloadRadioButton.AccessibleDescription = null;
            this.grantDownloadRadioButton.AccessibleName = null;
            resources.ApplyResources(this.grantDownloadRadioButton, "grantDownloadRadioButton");
            this.grantDownloadRadioButton.BackgroundImage = null;
            this.grantDownloadRadioButton.Checked = true;
            this.grantDownloadRadioButton.Font = null;
            this.grantDownloadRadioButton.Name = "grantDownloadRadioButton";
            this.grantDownloadRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.grantDownloadRadioButton, resources.GetString("grantDownloadRadioButton.ToolTip"));
            // 
            // denyDeleteRadioButton
            // 
            this.denyDeleteRadioButton.AccessibleDescription = null;
            this.denyDeleteRadioButton.AccessibleName = null;
            resources.ApplyResources(this.denyDeleteRadioButton, "denyDeleteRadioButton");
            this.denyDeleteRadioButton.BackgroundImage = null;
            this.denyDeleteRadioButton.Font = null;
            this.denyDeleteRadioButton.Name = "denyDeleteRadioButton";
            this.toolTip.SetToolTip(this.denyDeleteRadioButton, resources.GetString("denyDeleteRadioButton.ToolTip"));
            // 
            // grantDeleteRadioButton
            // 
            this.grantDeleteRadioButton.AccessibleDescription = null;
            this.grantDeleteRadioButton.AccessibleName = null;
            resources.ApplyResources(this.grantDeleteRadioButton, "grantDeleteRadioButton");
            this.grantDeleteRadioButton.BackgroundImage = null;
            this.grantDeleteRadioButton.Checked = true;
            this.grantDeleteRadioButton.Font = null;
            this.grantDeleteRadioButton.Name = "grantDeleteRadioButton";
            this.grantDeleteRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.grantDeleteRadioButton, resources.GetString("grantDeleteRadioButton.ToolTip"));
            // 
            // denyCreateRadioButton
            // 
            this.denyCreateRadioButton.AccessibleDescription = null;
            this.denyCreateRadioButton.AccessibleName = null;
            resources.ApplyResources(this.denyCreateRadioButton, "denyCreateRadioButton");
            this.denyCreateRadioButton.BackgroundImage = null;
            this.denyCreateRadioButton.Font = null;
            this.denyCreateRadioButton.Name = "denyCreateRadioButton";
            this.toolTip.SetToolTip(this.denyCreateRadioButton, resources.GetString("denyCreateRadioButton.ToolTip"));
            // 
            // grantCreateRadioButton
            // 
            this.grantCreateRadioButton.AccessibleDescription = null;
            this.grantCreateRadioButton.AccessibleName = null;
            resources.ApplyResources(this.grantCreateRadioButton, "grantCreateRadioButton");
            this.grantCreateRadioButton.BackgroundImage = null;
            this.grantCreateRadioButton.Checked = true;
            this.grantCreateRadioButton.Font = null;
            this.grantCreateRadioButton.Name = "grantCreateRadioButton";
            this.grantCreateRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.grantCreateRadioButton, resources.GetString("grantCreateRadioButton.ToolTip"));
            // 
            // denyWriteRadioButton
            // 
            this.denyWriteRadioButton.AccessibleDescription = null;
            this.denyWriteRadioButton.AccessibleName = null;
            resources.ApplyResources(this.denyWriteRadioButton, "denyWriteRadioButton");
            this.denyWriteRadioButton.BackgroundImage = null;
            this.denyWriteRadioButton.Font = null;
            this.denyWriteRadioButton.Name = "denyWriteRadioButton";
            this.toolTip.SetToolTip(this.denyWriteRadioButton, resources.GetString("denyWriteRadioButton.ToolTip"));
            // 
            // grantWriteRadioButton
            // 
            this.grantWriteRadioButton.AccessibleDescription = null;
            this.grantWriteRadioButton.AccessibleName = null;
            resources.ApplyResources(this.grantWriteRadioButton, "grantWriteRadioButton");
            this.grantWriteRadioButton.BackgroundImage = null;
            this.grantWriteRadioButton.Checked = true;
            this.grantWriteRadioButton.Font = null;
            this.grantWriteRadioButton.Name = "grantWriteRadioButton";
            this.grantWriteRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.grantWriteRadioButton, resources.GetString("grantWriteRadioButton.ToolTip"));
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
            this.groupBox3.Controls.Add(this.panel7);
            this.groupBox3.Controls.Add(this.panel6);
            this.groupBox3.Controls.Add(this.panel5);
            this.groupBox3.Controls.Add(this.panel4);
            this.groupBox3.Controls.Add(this.panel3);
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // panel7
            // 
            this.panel7.AccessibleDescription = null;
            this.panel7.AccessibleName = null;
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.BackgroundImage = null;
            this.panel7.Controls.Add(this.denyUploadRadioButton);
            this.panel7.Controls.Add(this.grantUploadRadioButton);
            this.panel7.Controls.Add(this.label8);
            this.panel7.Font = null;
            this.panel7.Name = "panel7";
            this.toolTip.SetToolTip(this.panel7, resources.GetString("panel7.ToolTip"));
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            this.toolTip.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // panel6
            // 
            this.panel6.AccessibleDescription = null;
            this.panel6.AccessibleName = null;
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.BackgroundImage = null;
            this.panel6.Controls.Add(this.denyDownloadRadioButton);
            this.panel6.Controls.Add(this.grantDownloadRadioButton);
            this.panel6.Controls.Add(this.label7);
            this.panel6.Font = null;
            this.panel6.Name = "panel6";
            this.toolTip.SetToolTip(this.panel6, resources.GetString("panel6.ToolTip"));
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            this.toolTip.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // panel5
            // 
            this.panel5.AccessibleDescription = null;
            this.panel5.AccessibleName = null;
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.BackgroundImage = null;
            this.panel5.Controls.Add(this.denyDeleteRadioButton);
            this.panel5.Controls.Add(this.grantDeleteRadioButton);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Font = null;
            this.panel5.Name = "panel5";
            this.toolTip.SetToolTip(this.panel5, resources.GetString("panel5.ToolTip"));
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            this.toolTip.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // panel4
            // 
            this.panel4.AccessibleDescription = null;
            this.panel4.AccessibleName = null;
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.BackgroundImage = null;
            this.panel4.Controls.Add(this.denyCreateRadioButton);
            this.panel4.Controls.Add(this.grantCreateRadioButton);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Font = null;
            this.panel4.Name = "panel4";
            this.toolTip.SetToolTip(this.panel4, resources.GetString("panel4.ToolTip"));
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            this.toolTip.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // panel3
            // 
            this.panel3.AccessibleDescription = null;
            this.panel3.AccessibleName = null;
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackgroundImage = null;
            this.panel3.Controls.Add(this.denyWriteRadioButton);
            this.panel3.Controls.Add(this.grantWriteRadioButton);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Font = null;
            this.panel3.Name = "panel3";
            this.toolTip.SetToolTip(this.panel3, resources.GetString("panel3.ToolTip"));
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            this.toolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
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
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
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

			if (_writePermission == XaclPermissionType.Grant)
			{
				this.grantWriteRadioButton.Checked = true;
			}
			else
			{
				this.denyWriteRadioButton.Checked = true;
			}

			if (_createPermission == XaclPermissionType.Grant)
			{
				this.grantCreateRadioButton.Checked = true;
			}
			else
			{
				this.denyCreateRadioButton.Checked = true;
			}

			if (_deletePermission == XaclPermissionType.Grant)
			{
				this.grantDeleteRadioButton.Checked = true;
			}
			else
			{
				this.denyDeleteRadioButton.Checked = true;
			}

			if (_uploadPermission == XaclPermissionType.Grant)
			{
				this.grantUploadRadioButton.Checked = true;
			}
			else
			{
				this.denyUploadRadioButton.Checked = true;
			}

			if (_downloadPermission == XaclPermissionType.Grant)
			{
				this.grantDownloadRadioButton.Checked = true;
			}
			else
			{
				this.denyDownloadRadioButton.Checked = true;
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

			if (this.grantWriteRadioButton.Checked)
			{
				_writePermission = XaclPermissionType.Grant;
			}
			else
			{
				_writePermission = XaclPermissionType.Deny;
			}

			if (this.grantCreateRadioButton.Checked)
			{
				_createPermission = XaclPermissionType.Grant;
			}
			else
			{
				_createPermission = XaclPermissionType.Deny;
			}

			if (this.grantDeleteRadioButton.Checked)
			{
				_deletePermission = XaclPermissionType.Grant;
			}
			else
			{
				_deletePermission = XaclPermissionType.Deny;
			}

			if (this.grantUploadRadioButton.Checked)
			{
				_uploadPermission = XaclPermissionType.Grant;
			}
			else
			{
				_uploadPermission = XaclPermissionType.Deny;
			}

			if (this.grantDownloadRadioButton.Checked)
			{
				_downloadPermission = XaclPermissionType.Grant;
			}
			else
			{
				_downloadPermission = XaclPermissionType.Deny;
			}
		}
	}
}

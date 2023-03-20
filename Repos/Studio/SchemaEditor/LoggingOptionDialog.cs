using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Logging;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for LoggingOptionDialog.
	/// </summary>
	public class LoggingOptionDialog : System.Windows.Forms.Form
	{
		private LoggingStatus _readLogStatus;
		private LoggingStatus _writeLogStatus;
		private LoggingStatus _createLogStatus;
		private LoggingStatus _deleteLogStatus;
		private LoggingStatus _uploadLogStatus;
		private LoggingStatus _downloadLogStatus;
        private LoggingStatus _importLogStatus;
        private LoggingStatus _exportLogStatus;

		private LoggingConflictResolutionType _conflictResolution;

		private LoggingPolicy _policy;

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton offtpRadioButton;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton readLogOnRadioButton;
        private System.Windows.Forms.RadioButton readLogOffRadioButton;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.RadioButton writeLogOffRadioButton;
		private System.Windows.Forms.RadioButton writeLogOnRadioButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.RadioButton createLogOffRadioButton;
		private System.Windows.Forms.RadioButton createLogOnRadioButton;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RadioButton deleteLogOffRadioButton;
		private System.Windows.Forms.RadioButton deleteLogOnRadioButton;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton ontpRadioButton;
		private System.Windows.Forms.RadioButton ntpRadioButton;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.RadioButton downloadLogOffRadioButton;
		private System.Windows.Forms.RadioButton downloadLogOnRadioButton;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.RadioButton uploadLogOffRadioButton;
		private System.Windows.Forms.RadioButton uploadLogOnRadioButton;
		private System.Windows.Forms.Label label8;
        private TabControl tabControl1;
        private TabPage conflictResolutionTabPage;
        private TabPage defaultStatusTabPage;
        private Panel panel8;
        private RadioButton importLogOffRadioButton;
        private RadioButton importLogOnRadioButton;
        private Label label9;
        private Panel panel9;
        private RadioButton exportLogOffRadioButton;
        private RadioButton exportLogOnRadioButton;
        private Label label10;
		private System.ComponentModel.IContainer components;

		public LoggingOptionDialog()
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

		public LoggingPolicy LoggingPolicy
		{
			get
			{
				return _policy;
			}
			set
			{
				_policy = value;

				_readLogStatus = value.Setting.DefaultReadLogStatus;
				_writeLogStatus = value.Setting.DefaultWriteLogStatus;
				_createLogStatus = value.Setting.DefaultCreateLogStatus;
				_deleteLogStatus = value.Setting.DefaultDeleteLogStatus;
				_uploadLogStatus = value.Setting.DefaultUploadLogStatus;
				_downloadLogStatus = value.Setting.DefaultDownloadLogStatus;
                _importLogStatus = value.Setting.DefaultImportLogStatus;
                _exportLogStatus = value.Setting.DefaultExportLogStatus;
				_conflictResolution = value.Setting.ConflictResolutionType;
			}
		}

		/// <summary>
        /// Gets or sets the default read log status.
		/// </summary>
		/// <value>One of LoggingStatus values</value>
		public LoggingStatus DefaultReadLogStatus
		{
			get
			{
				return _readLogStatus;
			}
			set
			{
				_readLogStatus = value;
			}
		}

		/// <summary>
        /// Gets or sets the default write log status.
		/// </summary>
		/// <value>One of LoggingStatus values</value>
		public LoggingStatus DefaultWriteLogStatus
		{
			get
			{
				return _writeLogStatus;
			}
			set
			{
				_writeLogStatus = value;
			}
		}

		/// <summary>
        /// Gets or sets the default create log status.
		/// </summary>
		/// <value>One of LoggingStatus values</value>
		public LoggingStatus DefaultCreateLogStatus
		{
			get
			{
				return _createLogStatus;
			}
			set
			{
				_createLogStatus = value;
			}
		}

		/// <summary>
		/// Gets or sets the default delete log status.
		/// </summary>
		/// <value>One of LoggingStatus values</value>
		public LoggingStatus DefaultDeleteLogStatus
		{
			get
			{
				return _deleteLogStatus;
			}
			set
			{
				_deleteLogStatus = value;
			}
		}

		/// <summary>
        /// Gets or sets the default upload log status.
		/// </summary>
		/// <value>One of LoggingStatus values</value>
		public LoggingStatus DefaultUploadLogStatus
		{
			get
			{
				return _uploadLogStatus;
			}
			set
			{
				_uploadLogStatus = value;
			}
		}

		/// <summary>
        /// Gets or sets the default download log status.
		/// </summary>
		/// <value>One of LoggingStatus values</value>
		public LoggingStatus DefaultDownloadLogStatus
		{
			get
			{
				return _downloadLogStatus;
			}
			set
			{
				_downloadLogStatus = value;
			}
		}

        /// <summary>
        /// Gets or sets the default import log status.
        /// </summary>
        /// <value>One of LoggingStatus values</value>
        public LoggingStatus DefaultImportLogStatus
        {
            get
            {
                return _importLogStatus;
            }
            set
            {
                _importLogStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the default export log status.
        /// </summary>
        /// <value>One of LoggingStatus values</value>
        public LoggingStatus DefaultExportLogStatus
        {
            get
            {
                return _exportLogStatus;
            }
            set
            {
                _exportLogStatus = value;
            }
        }

		/// <summary>
		/// Gets or sets the type of Conflict resolution.
		/// </summary>
		/// <value>One of LoggingConflictResolutionType values</value>
		public LoggingConflictResolutionType ConflictResolution
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoggingOptionDialog));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ntpRadioButton = new System.Windows.Forms.RadioButton();
            this.ontpRadioButton = new System.Windows.Forms.RadioButton();
            this.offtpRadioButton = new System.Windows.Forms.RadioButton();
            this.uploadLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.uploadLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.downloadLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.downloadLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.deleteLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.deleteLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.createLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.createLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.writeLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.writeLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.readLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.readLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.exportLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.exportLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.importLogOffRadioButton = new System.Windows.Forms.RadioButton();
            this.importLogOnRadioButton = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.conflictResolutionTabPage = new System.Windows.Forms.TabPage();
            this.defaultStatusTabPage = new System.Windows.Forms.TabPage();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.conflictResolutionTabPage.SuspendLayout();
            this.defaultStatusTabPage.SuspendLayout();
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
            // ontpRadioButton
            // 
            this.ontpRadioButton.AccessibleDescription = null;
            this.ontpRadioButton.AccessibleName = null;
            resources.ApplyResources(this.ontpRadioButton, "ontpRadioButton");
            this.ontpRadioButton.BackgroundImage = null;
            this.ontpRadioButton.Font = null;
            this.ontpRadioButton.Name = "ontpRadioButton";
            this.toolTip.SetToolTip(this.ontpRadioButton, resources.GetString("ontpRadioButton.ToolTip"));
            // 
            // offtpRadioButton
            // 
            this.offtpRadioButton.AccessibleDescription = null;
            this.offtpRadioButton.AccessibleName = null;
            resources.ApplyResources(this.offtpRadioButton, "offtpRadioButton");
            this.offtpRadioButton.BackgroundImage = null;
            this.offtpRadioButton.Checked = true;
            this.offtpRadioButton.Font = null;
            this.offtpRadioButton.Name = "offtpRadioButton";
            this.offtpRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.offtpRadioButton, resources.GetString("offtpRadioButton.ToolTip"));
            // 
            // uploadLogOffRadioButton
            // 
            this.uploadLogOffRadioButton.AccessibleDescription = null;
            this.uploadLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.uploadLogOffRadioButton, "uploadLogOffRadioButton");
            this.uploadLogOffRadioButton.BackgroundImage = null;
            this.uploadLogOffRadioButton.Checked = true;
            this.uploadLogOffRadioButton.Font = null;
            this.uploadLogOffRadioButton.Name = "uploadLogOffRadioButton";
            this.uploadLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.uploadLogOffRadioButton, resources.GetString("uploadLogOffRadioButton.ToolTip"));
            // 
            // uploadLogOnRadioButton
            // 
            this.uploadLogOnRadioButton.AccessibleDescription = null;
            this.uploadLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.uploadLogOnRadioButton, "uploadLogOnRadioButton");
            this.uploadLogOnRadioButton.BackgroundImage = null;
            this.uploadLogOnRadioButton.Font = null;
            this.uploadLogOnRadioButton.Name = "uploadLogOnRadioButton";
            this.toolTip.SetToolTip(this.uploadLogOnRadioButton, resources.GetString("uploadLogOnRadioButton.ToolTip"));
            // 
            // downloadLogOffRadioButton
            // 
            this.downloadLogOffRadioButton.AccessibleDescription = null;
            this.downloadLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.downloadLogOffRadioButton, "downloadLogOffRadioButton");
            this.downloadLogOffRadioButton.BackgroundImage = null;
            this.downloadLogOffRadioButton.Checked = true;
            this.downloadLogOffRadioButton.Font = null;
            this.downloadLogOffRadioButton.Name = "downloadLogOffRadioButton";
            this.downloadLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.downloadLogOffRadioButton, resources.GetString("downloadLogOffRadioButton.ToolTip"));
            // 
            // downloadLogOnRadioButton
            // 
            this.downloadLogOnRadioButton.AccessibleDescription = null;
            this.downloadLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.downloadLogOnRadioButton, "downloadLogOnRadioButton");
            this.downloadLogOnRadioButton.BackgroundImage = null;
            this.downloadLogOnRadioButton.Font = null;
            this.downloadLogOnRadioButton.Name = "downloadLogOnRadioButton";
            this.toolTip.SetToolTip(this.downloadLogOnRadioButton, resources.GetString("downloadLogOnRadioButton.ToolTip"));
            // 
            // deleteLogOffRadioButton
            // 
            this.deleteLogOffRadioButton.AccessibleDescription = null;
            this.deleteLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.deleteLogOffRadioButton, "deleteLogOffRadioButton");
            this.deleteLogOffRadioButton.BackgroundImage = null;
            this.deleteLogOffRadioButton.Checked = true;
            this.deleteLogOffRadioButton.Font = null;
            this.deleteLogOffRadioButton.Name = "deleteLogOffRadioButton";
            this.deleteLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.deleteLogOffRadioButton, resources.GetString("deleteLogOffRadioButton.ToolTip"));
            // 
            // deleteLogOnRadioButton
            // 
            this.deleteLogOnRadioButton.AccessibleDescription = null;
            this.deleteLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.deleteLogOnRadioButton, "deleteLogOnRadioButton");
            this.deleteLogOnRadioButton.BackgroundImage = null;
            this.deleteLogOnRadioButton.Font = null;
            this.deleteLogOnRadioButton.Name = "deleteLogOnRadioButton";
            this.toolTip.SetToolTip(this.deleteLogOnRadioButton, resources.GetString("deleteLogOnRadioButton.ToolTip"));
            // 
            // createLogOffRadioButton
            // 
            this.createLogOffRadioButton.AccessibleDescription = null;
            this.createLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.createLogOffRadioButton, "createLogOffRadioButton");
            this.createLogOffRadioButton.BackgroundImage = null;
            this.createLogOffRadioButton.Checked = true;
            this.createLogOffRadioButton.Font = null;
            this.createLogOffRadioButton.Name = "createLogOffRadioButton";
            this.createLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.createLogOffRadioButton, resources.GetString("createLogOffRadioButton.ToolTip"));
            // 
            // createLogOnRadioButton
            // 
            this.createLogOnRadioButton.AccessibleDescription = null;
            this.createLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.createLogOnRadioButton, "createLogOnRadioButton");
            this.createLogOnRadioButton.BackgroundImage = null;
            this.createLogOnRadioButton.Font = null;
            this.createLogOnRadioButton.Name = "createLogOnRadioButton";
            this.toolTip.SetToolTip(this.createLogOnRadioButton, resources.GetString("createLogOnRadioButton.ToolTip"));
            // 
            // writeLogOffRadioButton
            // 
            this.writeLogOffRadioButton.AccessibleDescription = null;
            this.writeLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.writeLogOffRadioButton, "writeLogOffRadioButton");
            this.writeLogOffRadioButton.BackgroundImage = null;
            this.writeLogOffRadioButton.Checked = true;
            this.writeLogOffRadioButton.Font = null;
            this.writeLogOffRadioButton.Name = "writeLogOffRadioButton";
            this.writeLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.writeLogOffRadioButton, resources.GetString("writeLogOffRadioButton.ToolTip"));
            // 
            // writeLogOnRadioButton
            // 
            this.writeLogOnRadioButton.AccessibleDescription = null;
            this.writeLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.writeLogOnRadioButton, "writeLogOnRadioButton");
            this.writeLogOnRadioButton.BackgroundImage = null;
            this.writeLogOnRadioButton.Font = null;
            this.writeLogOnRadioButton.Name = "writeLogOnRadioButton";
            this.toolTip.SetToolTip(this.writeLogOnRadioButton, resources.GetString("writeLogOnRadioButton.ToolTip"));
            // 
            // readLogOffRadioButton
            // 
            this.readLogOffRadioButton.AccessibleDescription = null;
            this.readLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.readLogOffRadioButton, "readLogOffRadioButton");
            this.readLogOffRadioButton.BackgroundImage = null;
            this.readLogOffRadioButton.Checked = true;
            this.readLogOffRadioButton.Font = null;
            this.readLogOffRadioButton.Name = "readLogOffRadioButton";
            this.readLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.readLogOffRadioButton, resources.GetString("readLogOffRadioButton.ToolTip"));
            // 
            // readLogOnRadioButton
            // 
            this.readLogOnRadioButton.AccessibleDescription = null;
            this.readLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.readLogOnRadioButton, "readLogOnRadioButton");
            this.readLogOnRadioButton.BackgroundImage = null;
            this.readLogOnRadioButton.Font = null;
            this.readLogOnRadioButton.Name = "readLogOnRadioButton";
            this.toolTip.SetToolTip(this.readLogOnRadioButton, resources.GetString("readLogOnRadioButton.ToolTip"));
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.ntpRadioButton);
            this.groupBox2.Controls.Add(this.ontpRadioButton);
            this.groupBox2.Controls.Add(this.offtpRadioButton);
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
            this.groupBox3.Controls.Add(this.panel9);
            this.groupBox3.Controls.Add(this.panel8);
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
            // panel9
            // 
            this.panel9.AccessibleDescription = null;
            this.panel9.AccessibleName = null;
            resources.ApplyResources(this.panel9, "panel9");
            this.panel9.BackgroundImage = null;
            this.panel9.Controls.Add(this.exportLogOffRadioButton);
            this.panel9.Controls.Add(this.exportLogOnRadioButton);
            this.panel9.Controls.Add(this.label10);
            this.panel9.Font = null;
            this.panel9.Name = "panel9";
            this.toolTip.SetToolTip(this.panel9, resources.GetString("panel9.ToolTip"));
            // 
            // exportLogOffRadioButton
            // 
            this.exportLogOffRadioButton.AccessibleDescription = null;
            this.exportLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.exportLogOffRadioButton, "exportLogOffRadioButton");
            this.exportLogOffRadioButton.BackgroundImage = null;
            this.exportLogOffRadioButton.Checked = true;
            this.exportLogOffRadioButton.Font = null;
            this.exportLogOffRadioButton.Name = "exportLogOffRadioButton";
            this.exportLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.exportLogOffRadioButton, resources.GetString("exportLogOffRadioButton.ToolTip"));
            // 
            // exportLogOnRadioButton
            // 
            this.exportLogOnRadioButton.AccessibleDescription = null;
            this.exportLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.exportLogOnRadioButton, "exportLogOnRadioButton");
            this.exportLogOnRadioButton.BackgroundImage = null;
            this.exportLogOnRadioButton.Font = null;
            this.exportLogOnRadioButton.Name = "exportLogOnRadioButton";
            this.toolTip.SetToolTip(this.exportLogOnRadioButton, resources.GetString("exportLogOnRadioButton.ToolTip"));
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            this.toolTip.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // panel8
            // 
            this.panel8.AccessibleDescription = null;
            this.panel8.AccessibleName = null;
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.BackgroundImage = null;
            this.panel8.Controls.Add(this.importLogOffRadioButton);
            this.panel8.Controls.Add(this.importLogOnRadioButton);
            this.panel8.Controls.Add(this.label9);
            this.panel8.Font = null;
            this.panel8.Name = "panel8";
            this.toolTip.SetToolTip(this.panel8, resources.GetString("panel8.ToolTip"));
            // 
            // importLogOffRadioButton
            // 
            this.importLogOffRadioButton.AccessibleDescription = null;
            this.importLogOffRadioButton.AccessibleName = null;
            resources.ApplyResources(this.importLogOffRadioButton, "importLogOffRadioButton");
            this.importLogOffRadioButton.BackgroundImage = null;
            this.importLogOffRadioButton.Checked = true;
            this.importLogOffRadioButton.Font = null;
            this.importLogOffRadioButton.Name = "importLogOffRadioButton";
            this.importLogOffRadioButton.TabStop = true;
            this.toolTip.SetToolTip(this.importLogOffRadioButton, resources.GetString("importLogOffRadioButton.ToolTip"));
            // 
            // importLogOnRadioButton
            // 
            this.importLogOnRadioButton.AccessibleDescription = null;
            this.importLogOnRadioButton.AccessibleName = null;
            resources.ApplyResources(this.importLogOnRadioButton, "importLogOnRadioButton");
            this.importLogOnRadioButton.BackgroundImage = null;
            this.importLogOnRadioButton.Font = null;
            this.importLogOnRadioButton.Name = "importLogOnRadioButton";
            this.toolTip.SetToolTip(this.importLogOnRadioButton, resources.GetString("importLogOnRadioButton.ToolTip"));
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            this.toolTip.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // panel7
            // 
            this.panel7.AccessibleDescription = null;
            this.panel7.AccessibleName = null;
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.BackgroundImage = null;
            this.panel7.Controls.Add(this.uploadLogOffRadioButton);
            this.panel7.Controls.Add(this.uploadLogOnRadioButton);
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
            this.panel6.Controls.Add(this.downloadLogOffRadioButton);
            this.panel6.Controls.Add(this.downloadLogOnRadioButton);
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
            this.panel5.Controls.Add(this.deleteLogOffRadioButton);
            this.panel5.Controls.Add(this.deleteLogOnRadioButton);
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
            this.panel4.Controls.Add(this.createLogOffRadioButton);
            this.panel4.Controls.Add(this.createLogOnRadioButton);
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
            this.panel3.Controls.Add(this.writeLogOffRadioButton);
            this.panel3.Controls.Add(this.writeLogOnRadioButton);
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
            this.panel2.Controls.Add(this.readLogOffRadioButton);
            this.panel2.Controls.Add(this.readLogOnRadioButton);
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
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.conflictResolutionTabPage);
            this.tabControl1.Controls.Add(this.defaultStatusTabPage);
            this.tabControl1.Font = null;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            // 
            // conflictResolutionTabPage
            // 
            this.conflictResolutionTabPage.AccessibleDescription = null;
            this.conflictResolutionTabPage.AccessibleName = null;
            resources.ApplyResources(this.conflictResolutionTabPage, "conflictResolutionTabPage");
            this.conflictResolutionTabPage.BackgroundImage = null;
            this.conflictResolutionTabPage.Controls.Add(this.groupBox2);
            this.conflictResolutionTabPage.Font = null;
            this.conflictResolutionTabPage.Name = "conflictResolutionTabPage";
            this.toolTip.SetToolTip(this.conflictResolutionTabPage, resources.GetString("conflictResolutionTabPage.ToolTip"));
            this.conflictResolutionTabPage.UseVisualStyleBackColor = true;
            // 
            // defaultStatusTabPage
            // 
            this.defaultStatusTabPage.AccessibleDescription = null;
            this.defaultStatusTabPage.AccessibleName = null;
            resources.ApplyResources(this.defaultStatusTabPage, "defaultStatusTabPage");
            this.defaultStatusTabPage.BackgroundImage = null;
            this.defaultStatusTabPage.Controls.Add(this.groupBox3);
            this.defaultStatusTabPage.Font = null;
            this.defaultStatusTabPage.Name = "defaultStatusTabPage";
            this.toolTip.SetToolTip(this.defaultStatusTabPage, resources.GetString("defaultStatusTabPage.ToolTip"));
            this.defaultStatusTabPage.UseVisualStyleBackColor = true;
            // 
            // LoggingOptionDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoggingOptionDialog";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.LoggingOptionDialog_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.conflictResolutionTabPage.ResumeLayout(false);
            this.defaultStatusTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void LoggingOptionDialog_Load(object sender, System.EventArgs e)
		{
			switch (_conflictResolution)
			{
				case LoggingConflictResolutionType.Offtp:
					this.offtpRadioButton.Checked = true;
					break;
				case LoggingConflictResolutionType.Ontp:
					this.ontpRadioButton.Checked = true;
					break;
				case LoggingConflictResolutionType.Ntp:
					this.ntpRadioButton.Checked = true;
					break;
			}

			if (_readLogStatus == LoggingStatus.On)
			{
				this.readLogOnRadioButton.Checked = true;
			}
			else
			{
				this.readLogOffRadioButton.Checked = true;
			}

			if (_writeLogStatus == LoggingStatus.On)
			{
				this.writeLogOnRadioButton.Checked = true;
			}
			else
			{
				this.writeLogOffRadioButton.Checked = true;
			}

			if (_createLogStatus == LoggingStatus.On)
			{
				this.createLogOnRadioButton.Checked = true;
			}
			else
			{
				this.createLogOffRadioButton.Checked = true;
			}

			if (_deleteLogStatus == LoggingStatus.On)
			{
				this.deleteLogOnRadioButton.Checked = true;
			}
			else
			{
				this.deleteLogOffRadioButton.Checked = true;
			}

			if (_uploadLogStatus == LoggingStatus.On)
			{
				this.uploadLogOnRadioButton.Checked = true;
			}
			else
			{
				this.uploadLogOffRadioButton.Checked = true;
			}

			if (_downloadLogStatus == LoggingStatus.On)
			{
				this.downloadLogOnRadioButton.Checked = true;
			}
			else
			{
				this.downloadLogOffRadioButton.Checked = true;
			}

            if (_importLogStatus == LoggingStatus.On)
            {
                this.importLogOnRadioButton.Checked = true;
            }
            else
            {
                this.importLogOffRadioButton.Checked = true;
            }

            if (_exportLogStatus == LoggingStatus.On)
            {
                this.exportLogOnRadioButton.Checked = true;
            }
            else
            {
                this.exportLogOffRadioButton.Checked = true;
            }
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (this.offtpRadioButton.Checked)
			{
				_conflictResolution = LoggingConflictResolutionType.Offtp;
			}
			else if (this.ontpRadioButton.Checked)
			{
				_conflictResolution = LoggingConflictResolutionType.Ontp;
			}
			else if (this.ntpRadioButton.Checked)
			{
				_conflictResolution = LoggingConflictResolutionType.Ntp;
			}

			if (this.readLogOnRadioButton.Checked)
			{
				_readLogStatus = LoggingStatus.On;
			}
			else
			{
				_readLogStatus = LoggingStatus.Off;
			}

			if (this.writeLogOnRadioButton.Checked)
			{
				_writeLogStatus = LoggingStatus.On;
			}
			else
			{
				_writeLogStatus = LoggingStatus.Off;
			}

			if (this.createLogOnRadioButton.Checked)
			{
				_createLogStatus = LoggingStatus.On;
			}
			else
			{
				_createLogStatus = LoggingStatus.Off;
			}

			if (this.deleteLogOnRadioButton.Checked)
			{
				_deleteLogStatus = LoggingStatus.On;
			}
			else
			{
				_deleteLogStatus = LoggingStatus.Off;
			}

			if (this.uploadLogOnRadioButton.Checked)
			{
				_uploadLogStatus = LoggingStatus.On;
			}
			else
			{
				_uploadLogStatus = LoggingStatus.Off;
			}

			if (this.downloadLogOnRadioButton.Checked)
			{
				_downloadLogStatus = LoggingStatus.On;
			}
			else
			{
                _downloadLogStatus = LoggingStatus.Off;
			}

            if (this.importLogOnRadioButton.Checked)
            {
                _importLogStatus = LoggingStatus.On;
            }
            else
            {
                _importLogStatus = LoggingStatus.Off;
            }

            if (this.exportLogOnRadioButton.Checked)
            {
                _exportLogStatus = LoggingStatus.On;
            }
            else
            {
                _exportLogStatus = LoggingStatus.Off;
            }
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.Web.Services.Dime;

using Newtera.Common.Core;
using Newtera.Common.Attachment;
using Newtera.Common.MetaData.FileType;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for AddAttachmentDialog.
	/// </summary>
	public class AddAttachmentDialog : System.Windows.Forms.Form
	{
        internal const int MAX_FILE_SIZE = 100; // 10 MB

		private string _instanceId;
		private string _className;
		private SchemaInfo _schemaInfo;
		private IList _attachmentItems;
		private AttachmentServiceStub _service;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private Stream _fileStream;

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TextBox filePathTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ErrorProvider infoProvider;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		public AddAttachmentDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_instanceId = null;
			_className = null;
			_schemaInfo = null;
			_attachmentItems = null;
			_service = new AttachmentServiceStub();
			_fileStream = null;
			_workInProgressDialog = new WorkInProgressDialog();
		}

		/// <summary>
		/// Destructor of AddAttachmentDialog class
		/// </summary>
		~AddAttachmentDialog()
		{
			if (_fileStream != null)
			{
				_fileStream.Close();
				_fileStream = null;
			}
			_workInProgressDialog.Dispose();
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
		/// Gets or sets the collection of list view items for attachments of an instance.
		/// </summary>
		/// <value>ListViewItemCollection object</value>
		public IList AttachmentItems
		{
			get
			{
				return _attachmentItems;
			}
			set
			{
				_attachmentItems = value;
			}
		}

		/// <summary>
		/// Gets or sets the id of an instance to which to add attachments.
		/// </summary>
		/// <value>A string representing an instance</value>
		public string InstanceId
		{
			get
			{
				return _instanceId;
			}
			set
			{
				_instanceId = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of an instance class.
		/// </summary>
		/// <value>A string representing a class name</value>
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

		/// <summary>
		/// Gets or sets the schema info.
		/// </summary>
		/// <value>A SchemaInfo object</value>
		public SchemaInfo SchemaInfo
		{
			get
			{
				return _schemaInfo;
			}
			set
			{
				_schemaInfo = value;
			}
		}

		/// <summary>
		/// Show the working dialog
		/// </summary>
		/// <remarks>It has to deal with multi-threading issue</remarks>
		private void ShowWorkingDialog()
		{
			lock (_workInProgressDialog)
			{
				// check _isRequestComplete flag in case the worker thread
				// completes the request before the working dialog is shown
				if (!_isRequestComplete && !_workInProgressDialog.Visible)
				{
					_workInProgressDialog.ShowDialog();
				}
			}
		}

		private delegate void HideWorkingDialogDelegate();

		/// <summary>
		/// Hide the working dialog
		/// </summary>
		/// <remarks>Has to condider multi-threading issue</remarks>
		private void HideWorkingDialog()
		{
			if (this.InvokeRequired == false)
			{
				// It is the UI thread, go ahead to close the working dialog
				// lock it while updating _isRequestComplete
				lock(_workInProgressDialog)
				{
                    if (_workInProgressDialog.Visible)
                    {
                        _workInProgressDialog.Close();
                    }
					_isRequestComplete = true;
				}
			}
			else
			{
				// It is a worker thread, pass the control to the UI thread
				HideWorkingDialogDelegate hideWorkingDialog = new HideWorkingDialogDelegate(HideWorkingDialog);
				this.BeginInvoke(hideWorkingDialog);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAttachmentDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
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
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // filePathTextBox
            // 
            resources.ApplyResources(this.filePathTextBox, "filePathTextBox");
            this.filePathTextBox.Name = "filePathTextBox";
            this.toolTip.SetToolTip(this.filePathTextBox, resources.GetString("filePathTextBox.ToolTip"));
            this.filePathTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.filePathTextBox_Validating);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // browseButton
            // 
            resources.ApplyResources(this.browseButton, "browseButton");
            this.browseButton.CausesValidation = false;
            this.browseButton.Name = "browseButton";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            this.toolTip.SetToolTip(this.nameTextBox, resources.GetString("nameTextBox.ToolTip"));
            this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // infoProvider
            // 
            this.infoProvider.ContainerControl = this;
            resources.ApplyResources(this.infoProvider, "infoProvider");
            // 
            // AddAttachmentDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.filePathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddAttachmentDialog";
            this.Load += new System.EventHandler(this.AddAttachmentDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Gets the information indicating whether an attachment name is
		/// unique.
		/// </summary>
		/// <param name="name">The given name</param>
		/// <returns>true if it is unique, false otherwise.</returns>
		private bool ValidateNameUniqueness(string name)
		{
			bool status = true;

			if (_attachmentItems != null)
			{
				foreach (AttachmentListViewItem item in _attachmentItems)
				{
					if (item.AttachmentInfo.Name == name)
					{
						status = false;
						break;
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Gets the information indicating whether a specified file exists.
		/// </summary>
		/// <param name="fileName">The file name</param>
		/// <returns>true if the file exists, false otherwise.</returns>
		private bool IsFileExist(string fileName)
		{
			return File.Exists(fileName);
		}

		/// <summary>
		/// Gets the type of a file based on its suffix
		/// </summary>
		/// <param name="fileName">The file name</param>
		/// <returns>A string represents a type</returns>
		private string GetMIMEType(string fileName)
		{
			FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByName(fileName);

			return fileTypeInfo.Type;
		}

		/// <summary>
		/// Get the file name from a file path
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <returns>File name</returns>
		private string GetFileName(string filePath)
		{
			int pos = filePath.LastIndexOf('\\');
			if (pos >= 0)
			{
				return filePath.Substring(pos + 1);
			}
			else
			{
				return filePath;
			}
		}

		/// <summary>
		/// If the text in nameTextBox does not have a suffix, then add a suffix
		/// according to that of file path.
		/// </summary>
		private void FixAttachmentSuffixSuffix()
		{
			int pos = this.nameTextBox.Text.IndexOf('.');
			if (pos < 0)
			{
				// missing a suffix
				pos = this.filePathTextBox.Text.LastIndexOf('.');
				if (pos > 0)
				{
					this.nameTextBox.Text = this.nameTextBox.Text + this.filePathTextBox.Text.Substring(pos);
				}
			}
		}

		#endregion

		private void browseButton_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.InitialDirectory = "c:\\" ;
			dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
			dialog.FilterIndex = 2 ;
			dialog.RestoreDirectory = true ;
	
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = dialog.FileName;
				this.filePathTextBox.Text = filePath;
				this.nameTextBox.Text = GetFileName(filePath);
			}
		}

		private void nameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null and has to be unique
			if (this.nameTextBox.Text.Length == 0)
			{
				this.nameTextBox.Text = GetFileName(this.filePathTextBox.Text);
			}
			else
			{
				FixAttachmentSuffixSuffix();
			}

			if (!ValidateNameUniqueness(this.nameTextBox.Text))
			{
				this.errorProvider.SetError(this.nameTextBox, "An attachment with the same name already exists.");
				this.infoProvider.SetError(this.nameTextBox, null);
				e.Cancel = true;
			}
			else
			{
				string tip = this.toolTip.GetToolTip((Control) sender);
				// show the info when there is text in text box
				this.errorProvider.SetError((Control) sender, null);
				this.infoProvider.SetError((Control) sender, tip);
			}		
		}

		private void filePathTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the file path cannot be null and has to represent an entity
			if (this.filePathTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.filePathTextBox, "Please enter a file path.");
				this.infoProvider.SetError(this.filePathTextBox, null);
				e.Cancel = true;
			}
			else if (!IsFileExist(this.filePathTextBox.Text))
			{
				this.errorProvider.SetError(this.filePathTextBox, "The file doesn't exist.");
				this.infoProvider.SetError(this.filePathTextBox, null);
				e.Cancel = true;
			}
			else
			{
                FileInfo fileInfo = new FileInfo(this.filePathTextBox.Text);
                long fileSize = fileInfo.Length / (1024 * 1000);
                if (fileSize > MAX_FILE_SIZE)
                {
                    this.errorProvider.SetError(this.filePathTextBox, "The file is too big to be uploaded. The limitation is 100MB.");
                    this.infoProvider.SetError(this.filePathTextBox, null);
                    e.Cancel = true;
                }
                else
                {
                    string tip = this.toolTip.GetToolTip((Control)sender);
                    // show the info when there is text in text box
                    this.errorProvider.SetError((Control)sender, null);
                    this.infoProvider.SetError((Control)sender, tip);
                }
			}				
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.None;
		
			// validate the text in filePathTextBox
			this.filePathTextBox.Focus();
			if (!this.Validate())
			{
				return;
			}

			// validate the text in nameTextBox
			this.nameTextBox.Focus();
			if (!this.Validate())
			{
				return;
			}
		}

		private void AddAttachmentDialog_Load(object sender, System.EventArgs e)
		{
			// display help providers to text boxes
			string tip = toolTip.GetToolTip(this.nameTextBox);
			if (tip.Length > 0)
			{
				infoProvider.SetError(this.nameTextBox, tip);
			}
		
			// display help providers to text boxes
			tip = toolTip.GetToolTip(this.filePathTextBox);
			if (tip.Length > 0)
			{
				infoProvider.SetError(this.filePathTextBox, tip);
			}
		}
	}
}

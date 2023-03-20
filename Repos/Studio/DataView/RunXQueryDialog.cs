using System;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for RunXQueryDialog.
	/// </summary>
	public class RunXQueryDialog : System.Windows.Forms.Form
	{
		private SchemaInfo _schemaInfo;
		private CMDataServiceStub _dataService;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button executeButton;
		private System.Windows.Forms.TextBox xqueryTextBox;
		private System.Windows.Forms.RichTextBox xmlResultTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RunXQueryDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_schemaInfo = null;
			_dataService = null;
			_workInProgressDialog = new WorkInProgressDialog();
		}

		~RunXQueryDialog()
		{
			_workInProgressDialog.Dispose();
		}

		/// <summary>
		/// Gets or sets the schema info
		/// </summary>
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
		/// Gets or sets the web service for executing a xquery
		/// </summary>
		public CMDataServiceStub DataService
		{
			get
			{
				return _dataService;
			}
			set
			{
				_dataService = value;
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunXQueryDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.xmlResultTextBox = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.xqueryTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Name = "panel1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.xmlResultTextBox);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // xmlResultTextBox
            // 
            resources.ApplyResources(this.xmlResultTextBox, "xmlResultTextBox");
            this.xmlResultTextBox.Name = "xmlResultTextBox";
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.xqueryTextBox);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // xqueryTextBox
            // 
            resources.ApplyResources(this.xqueryTextBox, "xqueryTextBox");
            this.xqueryTextBox.Name = "xqueryTextBox";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // executeButton
            // 
            resources.ApplyResources(this.executeButton, "executeButton");
            this.executeButton.Name = "executeButton";
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // RunXQueryDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "RunXQueryDialog";
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		private void ExecuteQuery()
		{
			string query = this.xqueryTextBox.Text;

			_isRequestComplete = false;

            // invoke the web service asynchronously
            XmlNode xmlNode = _dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_schemaInfo),
				query);

            XmlReader xmlReader = new XmlNodeReader(xmlNode);
            DataSet ds = new DataSet();
            ds.ReadXml(xmlReader);

            ShowQueryResult(ds.GetXml());
		}

		/// <summary>
		/// Display the query result.
		/// </summary>
		/// <param name="xml">The xml query result</param>
		private void ShowQueryResult(string xml)
		{
			// it is the UI thread, continue
			this.xmlResultTextBox.Text = xml;
		}

		#endregion

		private void executeButton_Click(object sender, System.EventArgs e)
		{
			ExecuteQuery();
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}

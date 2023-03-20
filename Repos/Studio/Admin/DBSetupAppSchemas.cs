using System;
using System.Xml;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DBSetupAppSchemas.
	/// </summary>
	public class DBSetupAppSchemas : System.Windows.Forms.Form
	{
		private WizardButtonTag _buttonTag;
		private DBSetupWizard _wizard;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private AdminServiceStub _service;
		private XmlDocument _doc;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DBSetupAppSchemas()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_buttonTag = WizardButtonTag.None;
			_wizard = null;

			_service = new AdminServiceStub();
			_workInProgressDialog = new WorkInProgressDialog();
			_doc = null;
		}

		~DBSetupAppSchemas()
		{
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
		/// Setup those application schemas that are checked
		/// </summary>
		private void SetupApplicationSchemas()
		{
			_isRequestComplete = false;

			string[] schemas = new string[this.listView1.CheckedItems.Count];
			int index = 0;
			foreach (ListViewItem item in this.listView1.CheckedItems)
			{
				schemas[index++] = item.Text;
			}

            try
            {
                // invoke the admin web service synchronously
                _service.SetupAppSchemas(schemas);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBSetupAppSchemas));
            this.panel1 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
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
            // DBSetupAppSchemas
            // 
            this.AcceptButton = this.finishButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DBSetupAppSchemas";
            this.Load += new System.EventHandler(this.DBSetupAppSchemas_Load);
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
			SetupApplicationSchemas();

			this._buttonTag = WizardButtonTag.Next;		
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Finish;
		}

		private void DBSetupAppSchemas_Load(object sender, System.EventArgs e)
		{
			try
			{
				// get a list of application schemas from server
				_doc = new XmlDocument();

				string xmlString = _service.GetAppSchemaList();

				_doc.LoadXml(xmlString);
				
				this.listView1.SuspendLayout();

				// display the application schemas in the list view
				foreach (XmlNode node in _doc.DocumentElement.ChildNodes)
				{
					if (node is XmlElement)
					{
						XmlElement element = (XmlElement) node;

						ListViewItem item = new ListViewItem(element.GetAttribute("Name"));
						string type = "Optional";
						if (element.GetAttribute("Type") == null)
						{
							type = element.GetAttribute("Type");
						}
						item.SubItems.Add(type);
						item.Checked = true;
						
						this.listView1.Items.Add(item);
					}
				}

				this.listView1.ResumeLayout();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema.Validate;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ValidateErrorDialog.
	/// </summary>
	public class ValidateErrorDialog : System.Windows.Forms.Form
	{
		private ValidateResultEntry _selectedEntry;
		private ValidateResultEntryCollection _entries;
		private string _summary;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label summaryLabel;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button showButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		public event EventHandler Accept;

		public ValidateErrorDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_selectedEntry = null;
			_entries = null;
			_summary = null;
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
		/// Gets the selected validate result entry.
		/// </summary>
		public ValidateResultEntry SelectedEntry
		{
			get
			{
				return _selectedEntry;
			}
		}

		/// <summary>
		/// Gets or sets the entries of validate result.
		/// </summary>
		public ValidateResultEntryCollection Entries
		{
			get
			{
				return _entries;
			}
			set
			{
				_entries = value;
			}
		}

		/// <summary>
		/// Gets or sets the summary of validate result.
		/// </summary>
		public string Summary
		{
			get
			{
				return _summary;
			}
			set
			{
				_summary = value;
			}
		}

		/// <summary>
		/// Display the validate result entries in the list view
		/// </summary>
		private void ShowValidateResultEntries()
		{
			if (_entries != null)
			{
				if (_summary != null)
				{
					this.summaryLabel.Text = _summary;
				}

				this.listView1.BeginUpdate();
				ListViewItem item;
				int imageIndex = 0;
				foreach (ValidateResultEntry entry in _entries)
				{
					if (entry.Type == EntryType.Error)
					{
						imageIndex = 0;
					}
					else
					{
						imageIndex = 1;
					}

					// display only image in the first column
					item = this.listView1.Items.Add("", imageIndex);
					item.SubItems.Add(entry.Message);
					item.SubItems.Add(entry.Source);
				}

				this.listView1.EndUpdate();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidateErrorDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.summaryLabel = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.showButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // summaryLabel
            // 
            resources.ApplyResources(this.summaryLabel, "summaryLabel");
            this.summaryLabel.Name = "summaryLabel";
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.FullRowSelect = true;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.SmallImageList = this.imageList1;
            this.toolTip.SetToolTip(this.listView1, resources.GetString("listView1.ToolTip"));
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // showButton
            // 
            resources.ApplyResources(this.showButton, "showButton");
            this.showButton.Name = "showButton";
            this.showButton.Click += new System.EventHandler(this.showButton_Click);
            // 
            // ValidateErrorDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.showButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.summaryLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ValidateErrorDialog";
            this.Load += new System.EventHandler(this.ValidateErrorDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ValidateErrorDialog_Load(object sender, System.EventArgs e)
		{
			ShowValidateResultEntries();
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void showButton_Click(object sender, System.EventArgs e)
		{
			// Fire the event when show button is clicked
			if (Accept != null)
			{
				Accept(this, new ShowEntryEventArgs(SelectedEntry.MetaDataElement));
			}
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_entries != null)
			{
				ListView.SelectedIndexCollection selectedIndices = this.listView1.SelectedIndices;
				if (selectedIndices.Count == 1)
				{
					_selectedEntry = _entries[selectedIndices[0]];
					this.showButton.Enabled = true;
				}
			}
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			if (_entries != null)
			{
				ListView.SelectedIndexCollection selectedIndices = this.listView1.SelectedIndices;
				if (selectedIndices.Count == 1)
				{
					_selectedEntry = _entries[selectedIndices[0]];
					this.showButton.Enabled = true;
				}

				// Fire the event when show button is clicked
				if (Accept != null)
				{
					Accept(this, new ShowEntryEventArgs(SelectedEntry.MetaDataElement));
				}
			}		
		}
	}

	public class ShowEntryEventArgs : EventArgs
	{
		public IMetaDataElement Element;

        public ShowEntryEventArgs(IMetaDataElement element) 
		{
			this.Element = element;
		}
	}
}

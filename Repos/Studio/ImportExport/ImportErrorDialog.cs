using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Mappings.Scripts;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// Summary description for ImportErrorDialog.
	/// </summary>
	public class ImportErrorDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private ScriptNodeCollection _scriptResults = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView errorListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ColumnHeader columnHeader3;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ImportErrorDialog()
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
		/// Gets or sets the meta data model.
		/// </summary>
		public MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
			}
		}

		/// <summary>
		/// Gets or sets a collection of script results that contains import error messages.
		/// </summary>
		public ScriptNodeCollection ScriptResults
		{
			get
			{
				return _scriptResults;
			}
			set
			{
				_scriptResults = value;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportErrorDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.errorListView = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // errorListView
            // 
            resources.ApplyResources(this.errorListView, "errorListView");
            this.errorListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader2});
            this.errorListView.FullRowSelect = true;
            this.errorListView.GridLines = true;
            this.errorListView.Name = "errorListView";
            this.errorListView.UseCompatibleStateImageBehavior = false;
            this.errorListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Name = "button1";
            // 
            // ImportErrorDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.button1);
            this.Controls.Add(this.errorListView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ImportErrorDialog";
            this.Load += new System.EventHandler(this.ImportErrorDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ImportErrorDialog_Load(object sender, System.EventArgs e)
		{
			ListViewItem listViewItem;
			string classCaption;

			if (_scriptResults != null)
			{
				foreach (ScriptManager scriptManager in _scriptResults)
				{
					// build list view items for errors
					foreach (ClassScript classScript in scriptManager.ClassScripts)
					{
						if (_metaData == null)
						{
							classCaption = classScript.ClassName;
						}
						else
						{
							ClassElement classElement = _metaData.SchemaModel.FindClass(classScript.ClassName);
							classCaption = classElement.Caption;
						}

						int row = 1;
						foreach (InstanceScript instanceScript in classScript.InstanceScripts)
						{
							if (!instanceScript.IsSucceeded)
							{
								listViewItem = new ListViewItem(classCaption);
								listViewItem.SubItems.Add(row.ToString());
								listViewItem.SubItems.Add(instanceScript.Message);

								this.errorListView.Items.Add(listViewItem);
							}

							row ++;
						}
					}
				}
			}
		}
	}
}

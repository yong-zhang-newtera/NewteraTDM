using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.DataGridActiveX.DataGridView;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// Summary description for ChooseSearchAttributePopup.
	/// </summary>
	public class ChooseSearchAttributePopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private DataGridViewModel _dataGridView;
		private ViewAttribute _searchAttribute;
		private System.Windows.Forms.ListView searchAttributesListView;
		private System.Windows.Forms.ImageList listViewImageList;
		private System.ComponentModel.IContainer components;

		public ChooseSearchAttributePopup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_dataGridView = null;
			_searchAttribute = null;
		}

		/// <summary>
		/// Gets the selected search attribute
		/// </summary>
		/// <value>An expression object</value>
		public object Expression
		{
			get
			{
				return _searchAttribute;
			}
			set
			{
				_searchAttribute = (ViewAttribute) value;
			}
		}

		/// <summary>
		/// Gets or sets the data view
		/// </summary>
		/// <value>A DataGridViewModel</value>
		public DataGridViewModel DataGridView
		{
			get
			{
				return _dataGridView;
			}
			set
			{
				_dataGridView = value;
			}
		}

		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the
		/// popup relative to the window.
		/// </summary>
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		/// <summary>
		/// Show the popup
		/// </summary>
		public new void Show()
		{
			base.Show();
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChooseSearchAttributePopup));
			this.searchAttributesListView = new System.Windows.Forms.ListView();
			this.listViewImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// searchAttributesListView
			// 
			this.searchAttributesListView.AccessibleDescription = resources.GetString("searchAttributesListView.AccessibleDescription");
			this.searchAttributesListView.AccessibleName = resources.GetString("searchAttributesListView.AccessibleName");
			this.searchAttributesListView.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("searchAttributesListView.Alignment")));
			this.searchAttributesListView.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("searchAttributesListView.Anchor")));
			this.searchAttributesListView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("searchAttributesListView.BackgroundImage")));
			this.searchAttributesListView.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("searchAttributesListView.Dock")));
			this.searchAttributesListView.Enabled = ((bool)(resources.GetObject("searchAttributesListView.Enabled")));
			this.searchAttributesListView.Font = ((System.Drawing.Font)(resources.GetObject("searchAttributesListView.Font")));
			this.searchAttributesListView.FullRowSelect = true;
			this.searchAttributesListView.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("searchAttributesListView.ImeMode")));
			this.searchAttributesListView.LabelWrap = ((bool)(resources.GetObject("searchAttributesListView.LabelWrap")));
			this.searchAttributesListView.LargeImageList = this.listViewImageList;
			this.searchAttributesListView.Location = ((System.Drawing.Point)(resources.GetObject("searchAttributesListView.Location")));
			this.searchAttributesListView.MultiSelect = false;
			this.searchAttributesListView.Name = "searchAttributesListView";
			this.searchAttributesListView.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("searchAttributesListView.RightToLeft")));
			this.searchAttributesListView.Size = ((System.Drawing.Size)(resources.GetObject("searchAttributesListView.Size")));
			this.searchAttributesListView.SmallImageList = this.listViewImageList;
			this.searchAttributesListView.TabIndex = ((int)(resources.GetObject("searchAttributesListView.TabIndex")));
			this.searchAttributesListView.Text = resources.GetString("searchAttributesListView.Text");
			this.searchAttributesListView.View = System.Windows.Forms.View.SmallIcon;
			this.searchAttributesListView.Visible = ((bool)(resources.GetObject("searchAttributesListView.Visible")));
			this.searchAttributesListView.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// listViewImageList
			// 
			this.listViewImageList.ImageSize = ((System.Drawing.Size)(resources.GetObject("listViewImageList.ImageSize")));
			this.listViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewImageList.ImageStream")));
			this.listViewImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ChooseSearchAttributePopup
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.searchAttributesListView);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ChooseSearchAttributePopup";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.ChooseSearchAttributePopup_Load);
			this.Deactivate += new System.EventHandler(this.ChooseSearchAttributePopup_Deactivate);
			this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Display the attributes of a class that can be used for search
		/// </summary>
		/// <param name="viewClass">The class</param>
		private void ShowClassAttributes()
		{
			AttributeElementListViewItem item;
			this.searchAttributesListView.SuspendLayout();
			this.searchAttributesListView.Items.Clear();
			this.searchAttributesListView.View = System.Windows.Forms.View.SmallIcon;

			int index = 0;
			foreach (ViewAttribute att in this._dataGridView.ResultAttributes)
			{
                if (att is ViewSimpleAttribute)
                {
                    item = new AttributeElementListViewItem(att.Caption, att);
                    item.ImageIndex = 0;

                    this.searchAttributesListView.Items.Insert(index++, item);
                }
                else if (att is ViewRelationshipAttribute)
                {
                    if (((ViewRelationshipAttribute)att).IsForeignKeyRequired)
                    {
                        item = new AttributeElementListViewItem(att.Caption, att);
                        item.ImageIndex = 1;

                        this.searchAttributesListView.Items.Insert(index++, item);
                    }
                }
			}

			this.searchAttributesListView.ResumeLayout();
		}

		#endregion

		private void ChooseSearchAttributePopup_Load(object sender, System.EventArgs e)
		{
			ShowClassAttributes();
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.searchAttributesListView.SelectedItems.Count == 1)
			{
				AttributeElementListViewItem item = (AttributeElementListViewItem) this.searchAttributesListView.SelectedItems[0];

				// create a ViewAttribute as a search attribute
                if (item.AttributeElement is ViewSimpleAttribute)
                {
                    _searchAttribute = (ViewAttribute)((ViewSimpleAttribute)item.AttributeElement).Clone();
                }
                else if (item.AttributeElement is ViewRelationshipAttribute)
                {
                    _searchAttribute = (ViewAttribute)((ViewRelationshipAttribute)item.AttributeElement).Clone();
                }
	
				// fire the accept event
				if (Accept != null)
				{
					Accept(this, EventArgs.Empty);
				}

				this.Close();
			}		
		}

		private void ChooseSearchAttributePopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}

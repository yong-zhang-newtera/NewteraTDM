using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// ArrangeElementDialog 的摘要说明。
	/// </summary>
	public class ArrangeElementDialog : System.Windows.Forms.Form
	{
		private MetaDataTreeNode _selectedTreeNode = null;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button upButton;
		private System.Windows.Forms.Button downButton;
		private System.Windows.Forms.ImageList listViewLargeImageList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.ComponentModel.IContainer components;

		public ArrangeElementDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// Gets or sets the selected tree node whose children nodes are to be arranged
		/// </summary>
		public MetaDataTreeNode SelectedTreeNode
		{
			get
			{
				return _selectedTreeNode;
			}
			set
			{
				_selectedTreeNode = value;
			}
		}

		/// <summary>
		/// 清理所有正在使用的资源。
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArrangeElementDialog));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.listViewLargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.LargeImageList = this.listViewLargeImageList;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.SmallImageList = this.listViewLargeImageList;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
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
            // listViewLargeImageList
            // 
            this.listViewLargeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewLargeImageList.ImageStream")));
            this.listViewLargeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewLargeImageList.Images.SetKeyName(0, "");
            this.listViewLargeImageList.Images.SetKeyName(1, "");
            this.listViewLargeImageList.Images.SetKeyName(2, "");
            this.listViewLargeImageList.Images.SetKeyName(3, "");
            this.listViewLargeImageList.Images.SetKeyName(4, "");
            this.listViewLargeImageList.Images.SetKeyName(5, "");
            this.listViewLargeImageList.Images.SetKeyName(6, "");
            this.listViewLargeImageList.Images.SetKeyName(7, "");
            this.listViewLargeImageList.Images.SetKeyName(8, "");
            this.listViewLargeImageList.Images.SetKeyName(9, "");
            this.listViewLargeImageList.Images.SetKeyName(10, "");
            this.listViewLargeImageList.Images.SetKeyName(11, "");
            this.listViewLargeImageList.Images.SetKeyName(12, "");
            this.listViewLargeImageList.Images.SetKeyName(13, "");
            this.listViewLargeImageList.Images.SetKeyName(14, "");
            this.listViewLargeImageList.Images.SetKeyName(15, "");
            this.listViewLargeImageList.Images.SetKeyName(16, "virtualproperty.GIF");
            this.listViewLargeImageList.Images.SetKeyName(17, "imageicon.gif");
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // upButton
            // 
            resources.ApplyResources(this.upButton, "upButton");
            this.upButton.Name = "upButton";
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            resources.ApplyResources(this.downButton, "downButton");
            this.downButton.Name = "downButton";
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // ArrangeElementDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ArrangeElementDialog";
            this.Load += new System.EventHandler(this.ArrangeElementDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void upButton_Click(object sender, System.EventArgs e)
		{
			if (this.listView1.SelectedIndices.Count == 1)
			{
				int selectedIndex = this.listView1.SelectedIndices[0];
				ListViewItem selectedItem = this.listView1.SelectedItems[0];

				// move the selected item one position upward
				this.listView1.Items.RemoveAt(selectedIndex);
				this.listView1.Items.Insert(selectedIndex - 1, selectedItem);

				this.listView1.Refresh();
			}
		}

		private void downButton_Click(object sender, System.EventArgs e)
		{
			if (this.listView1.SelectedIndices.Count == 1)
			{
				int selectedIndex = this.listView1.SelectedIndices[0];
				ListViewItem selectedItem = this.listView1.SelectedItems[0];

				// move the selected item one position downward
				this.listView1.Items.RemoveAt(selectedIndex);
				this.listView1.Items.Insert(selectedIndex + 1, selectedItem);

				this.listView1.Refresh();
			}
		}

		private void ArrangeElementDialog_Load(object sender, System.EventArgs e)
		{
			if (this.SelectedTreeNode != null)
			{
				MetaDataListViewBuilder listViewBuilder = new MetaDataListViewBuilder();

				listViewBuilder.BuildItems(this.listView1, this.SelectedTreeNode);
			}
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.listView1.SelectedIndices.Count == 1)
			{
				int selectedIndex = this.listView1.SelectedIndices[0];

				this.upButton.Enabled = true;
				this.downButton.Enabled = true;

				if (selectedIndex == 0)
				{
					this.upButton.Enabled = false;
				}
				
				if (selectedIndex == (this.listView1.Items.Count - 1))
				{
					this.downButton.Enabled = false;
				}
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// set the display positions to the IMetaDataElement
			int position = 0;

			foreach (MetaDataListViewItem listViewItem in this.listView1.Items)
			{
                if (listViewItem.TreeNode.MetaDataElement != null)
                {
                    listViewItem.TreeNode.MetaDataElement.Position = position++;
                }
                else
                {
                    // it is a folder node, set the positions of its children
                    foreach (MetaDataTreeNode child in listViewItem.TreeNode.Nodes)
                    {
                        if (child.MetaDataElement != null)
                        {
                            child.MetaDataElement.Position = position++;
                        }
                    }
                }
			}
		}
	}
}

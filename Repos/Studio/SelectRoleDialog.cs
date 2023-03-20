using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// SelectRoleDialog 的摘要说明。
	/// </summary>
	public class SelectRoleDialog : System.Windows.Forms.Form
	{
		private string[] _selectedRoles;
		private UserInfoServiceStub _userInfoService;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ImageList listViewSmallImageList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
		private System.ComponentModel.IContainer components;

		public SelectRoleDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_userInfoService = new UserInfoServiceStub();
		}

		/// <summary>
		///  Gets or sets the selected roles
		/// </summary>
		public string[] SelectedRoles
		{
			get
			{
				return _selectedRoles;
			}
			set
			{
				_selectedRoles = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectRoleDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.listViewSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // listView
            // 
            resources.ApplyResources(this.listView, "listView");
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView.FullRowSelect = true;
            this.listView.Name = "listView";
            this.listView.SmallImageList = this.listViewSmallImageList;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // listViewSmallImageList
            // 
            this.listViewSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewSmallImageList.ImageStream")));
            this.listViewSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewSmallImageList.Images.SetKeyName(0, "");
            this.listViewSmallImageList.Images.SetKeyName(1, "");
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
            // 
            // SelectRoleDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectRoleDialog";
            this.Load += new System.EventHandler(this.SelectRoleDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void SelectRoleDialog_Load(object sender, System.EventArgs e)
		{
			try
			{
                // invoke the web service synchronously
                RolesListHandler roleListHandler = new RolesListHandler();
                EnumValueCollection roles = roleListHandler.GetValues(new WindowClientUserManager());

				this.listView.SuspendLayout();

				ListViewItem item;

				foreach (EnumValue role in roles)
				{
					item = new ListViewItem(role.Value, 0);
                    item.SubItems.Add(role.DisplayText);
                    this.listView.Items.Add(item);
				}

				this.listView.ResumeLayout();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listView.SelectedItems.Count > 0)
			{
				this._selectedRoles = new string[listView.SelectedItems.Count];
				for (int i = 0; i < listView.SelectedItems.Count; i++)
				{
					this._selectedRoles[i] = listView.SelectedItems[i].Text;
				}
			}
			else
			{
				this._selectedRoles = null;
			}
		}
	}
}

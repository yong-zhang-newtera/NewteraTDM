using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// ViewRoleUsersDialog 的摘要说明。
	/// </summary>
	public class ViewRoleUsersDialog : System.Windows.Forms.Form
	{
		private string _role;
		private string[] _users;
        private EnumValueCollection _allUsers;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label roleLabel;
		private System.Windows.Forms.ListBox userListBox;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewRoleUsersDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_role = null;
			_users = null;
            _allUsers = null;
		}

		/// <summary>
		/// Gets or sets the role
		/// </summary>
		public string Role
		{
			get
			{
				return _role;
			}
			set
			{
				_role = value;
			}
		}

		/// <summary>
		/// Gets or sets the role's users
		/// </summary>
		public string[] Users
		{
			get
			{
				return _users;
			}
			set
			{
				_users = value;
			}
		}

        /// <summary>
        /// Gets or sets the enum collection for all users
        /// </summary>
        public EnumValueCollection AllUsers
        {
            get
            {
                return _allUsers;
            }
            set
            {
                _allUsers = value;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ViewRoleUsersDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.roleLabel = new System.Windows.Forms.Label();
			this.userListBox = new System.Windows.Forms.ListBox();
			this.okButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// roleLabel
			// 
			this.roleLabel.AccessibleDescription = resources.GetString("roleLabel.AccessibleDescription");
			this.roleLabel.AccessibleName = resources.GetString("roleLabel.AccessibleName");
			this.roleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("roleLabel.Anchor")));
			this.roleLabel.AutoSize = ((bool)(resources.GetObject("roleLabel.AutoSize")));
			this.roleLabel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("roleLabel.Dock")));
			this.roleLabel.Enabled = ((bool)(resources.GetObject("roleLabel.Enabled")));
			this.roleLabel.Font = ((System.Drawing.Font)(resources.GetObject("roleLabel.Font")));
			this.roleLabel.Image = ((System.Drawing.Image)(resources.GetObject("roleLabel.Image")));
			this.roleLabel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("roleLabel.ImageAlign")));
			this.roleLabel.ImageIndex = ((int)(resources.GetObject("roleLabel.ImageIndex")));
			this.roleLabel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("roleLabel.ImeMode")));
			this.roleLabel.Location = ((System.Drawing.Point)(resources.GetObject("roleLabel.Location")));
			this.roleLabel.Name = "roleLabel";
			this.roleLabel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("roleLabel.RightToLeft")));
			this.roleLabel.Size = ((System.Drawing.Size)(resources.GetObject("roleLabel.Size")));
			this.roleLabel.TabIndex = ((int)(resources.GetObject("roleLabel.TabIndex")));
			this.roleLabel.Text = resources.GetString("roleLabel.Text");
			this.roleLabel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("roleLabel.TextAlign")));
			this.roleLabel.Visible = ((bool)(resources.GetObject("roleLabel.Visible")));
			// 
			// userListBox
			// 
			this.userListBox.AccessibleDescription = resources.GetString("userListBox.AccessibleDescription");
			this.userListBox.AccessibleName = resources.GetString("userListBox.AccessibleName");
			this.userListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("userListBox.Anchor")));
			this.userListBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("userListBox.BackgroundImage")));
			this.userListBox.ColumnWidth = ((int)(resources.GetObject("userListBox.ColumnWidth")));
			this.userListBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("userListBox.Dock")));
			this.userListBox.Enabled = ((bool)(resources.GetObject("userListBox.Enabled")));
			this.userListBox.Font = ((System.Drawing.Font)(resources.GetObject("userListBox.Font")));
			this.userListBox.HorizontalExtent = ((int)(resources.GetObject("userListBox.HorizontalExtent")));
			this.userListBox.HorizontalScrollbar = ((bool)(resources.GetObject("userListBox.HorizontalScrollbar")));
			this.userListBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("userListBox.ImeMode")));
			this.userListBox.IntegralHeight = ((bool)(resources.GetObject("userListBox.IntegralHeight")));
			this.userListBox.ItemHeight = ((int)(resources.GetObject("userListBox.ItemHeight")));
			this.userListBox.Location = ((System.Drawing.Point)(resources.GetObject("userListBox.Location")));
			this.userListBox.Name = "userListBox";
			this.userListBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("userListBox.RightToLeft")));
			this.userListBox.ScrollAlwaysVisible = ((bool)(resources.GetObject("userListBox.ScrollAlwaysVisible")));
			this.userListBox.Size = ((System.Drawing.Size)(resources.GetObject("userListBox.Size")));
			this.userListBox.TabIndex = ((int)(resources.GetObject("userListBox.TabIndex")));
			this.userListBox.Visible = ((bool)(resources.GetObject("userListBox.Visible")));
			// 
			// okButton
			// 
			this.okButton.AccessibleDescription = resources.GetString("okButton.AccessibleDescription");
			this.okButton.AccessibleName = resources.GetString("okButton.AccessibleName");
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("okButton.Anchor")));
			this.okButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("okButton.BackgroundImage")));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("okButton.Dock")));
			this.okButton.Enabled = ((bool)(resources.GetObject("okButton.Enabled")));
			this.okButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("okButton.FlatStyle")));
			this.okButton.Font = ((System.Drawing.Font)(resources.GetObject("okButton.Font")));
			this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
			this.okButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.ImageAlign")));
			this.okButton.ImageIndex = ((int)(resources.GetObject("okButton.ImageIndex")));
			this.okButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("okButton.ImeMode")));
			this.okButton.Location = ((System.Drawing.Point)(resources.GetObject("okButton.Location")));
			this.okButton.Name = "okButton";
			this.okButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("okButton.RightToLeft")));
			this.okButton.Size = ((System.Drawing.Size)(resources.GetObject("okButton.Size")));
			this.okButton.TabIndex = ((int)(resources.GetObject("okButton.TabIndex")));
			this.okButton.Text = resources.GetString("okButton.Text");
			this.okButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.TextAlign")));
			this.okButton.Visible = ((bool)(resources.GetObject("okButton.Visible")));
			// 
			// ViewRoleUsersDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.userListBox);
			this.Controls.Add(this.roleLabel);
			this.Controls.Add(this.label1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ViewRoleUsersDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.ViewUserRoles_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void ViewUserRoles_Load(object sender, System.EventArgs e)
		{
			if (this._role != null)
			{
				roleLabel.Text = this._role;
			}
			else
			{
				roleLabel.Text = "";
			}

			if (this._users != null)
			{
                foreach (string user in _users)
                {
                    foreach (EnumValue enumValue in _allUsers)
                    {
                        if (enumValue.Value == user)
                        {
                            this.userListBox.Items.Add(enumValue.Value + " (" + enumValue.DisplayText + ")");
                            break;
                        }
                    }
                }
			}
		}
	}
}

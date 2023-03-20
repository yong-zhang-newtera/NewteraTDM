using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.Studio
{
	/// <summary>
	/// ViewUserRolesDialog 的摘要说明。
	/// </summary>
	public class ViewUserRolesDialog : System.Windows.Forms.Form
	{
		private string _user;
		private string[] _roles;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox roleListBox;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label userNameLabel;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewUserRolesDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_user = null;
			_roles = null;
		}

		/// <summary>
		/// Gets or sets the user name
		/// </summary>
		public string UserName
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
			}
		}

		/// <summary>
		/// Gets or sets the user's roles
		/// </summary>
		public string[] UserRoles
		{
			get
			{
				return _roles;
			}
			set
			{
				_roles = value;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ViewUserRolesDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.userNameLabel = new System.Windows.Forms.Label();
			this.roleListBox = new System.Windows.Forms.ListBox();
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
			// userNameLabel
			// 
			this.userNameLabel.AccessibleDescription = resources.GetString("userNameLabel.AccessibleDescription");
			this.userNameLabel.AccessibleName = resources.GetString("userNameLabel.AccessibleName");
			this.userNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("userNameLabel.Anchor")));
			this.userNameLabel.AutoSize = ((bool)(resources.GetObject("userNameLabel.AutoSize")));
			this.userNameLabel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("userNameLabel.Dock")));
			this.userNameLabel.Enabled = ((bool)(resources.GetObject("userNameLabel.Enabled")));
			this.userNameLabel.Font = ((System.Drawing.Font)(resources.GetObject("userNameLabel.Font")));
			this.userNameLabel.Image = ((System.Drawing.Image)(resources.GetObject("userNameLabel.Image")));
			this.userNameLabel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("userNameLabel.ImageAlign")));
			this.userNameLabel.ImageIndex = ((int)(resources.GetObject("userNameLabel.ImageIndex")));
			this.userNameLabel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("userNameLabel.ImeMode")));
			this.userNameLabel.Location = ((System.Drawing.Point)(resources.GetObject("userNameLabel.Location")));
			this.userNameLabel.Name = "userNameLabel";
			this.userNameLabel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("userNameLabel.RightToLeft")));
			this.userNameLabel.Size = ((System.Drawing.Size)(resources.GetObject("userNameLabel.Size")));
			this.userNameLabel.TabIndex = ((int)(resources.GetObject("userNameLabel.TabIndex")));
			this.userNameLabel.Text = resources.GetString("userNameLabel.Text");
			this.userNameLabel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("userNameLabel.TextAlign")));
			this.userNameLabel.Visible = ((bool)(resources.GetObject("userNameLabel.Visible")));
			// 
			// roleListBox
			// 
			this.roleListBox.AccessibleDescription = resources.GetString("roleListBox.AccessibleDescription");
			this.roleListBox.AccessibleName = resources.GetString("roleListBox.AccessibleName");
			this.roleListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("roleListBox.Anchor")));
			this.roleListBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("roleListBox.BackgroundImage")));
			this.roleListBox.ColumnWidth = ((int)(resources.GetObject("roleListBox.ColumnWidth")));
			this.roleListBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("roleListBox.Dock")));
			this.roleListBox.Enabled = ((bool)(resources.GetObject("roleListBox.Enabled")));
			this.roleListBox.Font = ((System.Drawing.Font)(resources.GetObject("roleListBox.Font")));
			this.roleListBox.HorizontalExtent = ((int)(resources.GetObject("roleListBox.HorizontalExtent")));
			this.roleListBox.HorizontalScrollbar = ((bool)(resources.GetObject("roleListBox.HorizontalScrollbar")));
			this.roleListBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("roleListBox.ImeMode")));
			this.roleListBox.IntegralHeight = ((bool)(resources.GetObject("roleListBox.IntegralHeight")));
			this.roleListBox.ItemHeight = ((int)(resources.GetObject("roleListBox.ItemHeight")));
			this.roleListBox.Location = ((System.Drawing.Point)(resources.GetObject("roleListBox.Location")));
			this.roleListBox.Name = "roleListBox";
			this.roleListBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("roleListBox.RightToLeft")));
			this.roleListBox.ScrollAlwaysVisible = ((bool)(resources.GetObject("roleListBox.ScrollAlwaysVisible")));
			this.roleListBox.Size = ((System.Drawing.Size)(resources.GetObject("roleListBox.Size")));
			this.roleListBox.TabIndex = ((int)(resources.GetObject("roleListBox.TabIndex")));
			this.roleListBox.Visible = ((bool)(resources.GetObject("roleListBox.Visible")));
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
			// ViewUserRolesDialog
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
			this.Controls.Add(this.roleListBox);
			this.Controls.Add(this.userNameLabel);
			this.Controls.Add(this.label1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ViewUserRolesDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.ViewUserRolesDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion


		private void ViewUserRolesDialog_Load(object sender, System.EventArgs e)
		{
			if (this._user != null)
			{
				userNameLabel.Text = this._user;
			}
			else
			{
				userNameLabel.Text = "";
			}

			if (this._roles != null)
			{
				this.roleListBox.DataSource = this._roles;
			}
		}
	}
}

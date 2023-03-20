using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.XaclModel;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.SiteMapStudio
{
	/// <summary>
	/// Summary description for SelectUserOrRoleDialog.
	/// </summary>
	public class SelectUserOrRoleDialog : System.Windows.Forms.Form
	{
		private UserInfoServiceStub _userInfoService;
        private EnumValueCollection _allRoles;
		private EnumValueCollection _allUsers;
		private string[] _selectedRoles;
		private string _selectedUser;

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton userRadioButton;
		private System.Windows.Forms.RadioButton roleRadioButton;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ImageList listViewLargeImageList;
		private System.Windows.Forms.ImageList listViewSmallImageList;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ContextMenu lvContextMenu;
		private System.Windows.Forms.MenuItem lvViewRolesMenuItem;
        private System.Windows.Forms.MenuItem lvViewUsersMenuItem;
        private ColumnHeader columnHeader2;
		private System.ComponentModel.IContainer components;

		public SelectUserOrRoleDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_allRoles = null;
			_allUsers = null;
			_userInfoService = new UserInfoServiceStub();
			_selectedRoles = null;
			_selectedUser = null;
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
		/// Gets the selected roles
		/// </summary>
		public string[] SelectedRoles
		{
			get
			{
				return _selectedRoles;
			}
		}

		/// <summary>
		/// Gets the selected user
		/// </summary>
		public string SelectedUser
		{
			get
			{
				return _selectedUser;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectUserOrRoleDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.userRadioButton = new System.Windows.Forms.RadioButton();
            this.roleRadioButton = new System.Windows.Forms.RadioButton();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lvContextMenu = new System.Windows.Forms.ContextMenu();
            this.lvViewRolesMenuItem = new System.Windows.Forms.MenuItem();
            this.lvViewUsersMenuItem = new System.Windows.Forms.MenuItem();
            this.listViewLargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.listViewSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.userRadioButton);
            this.groupBox1.Controls.Add(this.roleRadioButton);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // userRadioButton
            // 
            this.userRadioButton.AccessibleDescription = null;
            this.userRadioButton.AccessibleName = null;
            resources.ApplyResources(this.userRadioButton, "userRadioButton");
            this.userRadioButton.BackgroundImage = null;
            this.userRadioButton.Font = null;
            this.userRadioButton.Name = "userRadioButton";
            this.userRadioButton.CheckedChanged += new System.EventHandler(this.userRadioButton_CheckedChanged);
            // 
            // roleRadioButton
            // 
            this.roleRadioButton.AccessibleDescription = null;
            this.roleRadioButton.AccessibleName = null;
            resources.ApplyResources(this.roleRadioButton, "roleRadioButton");
            this.roleRadioButton.BackgroundImage = null;
            this.roleRadioButton.Font = null;
            this.roleRadioButton.Name = "roleRadioButton";
            // 
            // listView
            // 
            this.listView.AccessibleDescription = null;
            this.listView.AccessibleName = null;
            resources.ApplyResources(this.listView, "listView");
            this.listView.BackgroundImage = null;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView.ContextMenu = this.lvContextMenu;
            this.listView.Font = null;
            this.listView.FullRowSelect = true;
            this.listView.LargeImageList = this.listViewLargeImageList;
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
            // lvContextMenu
            // 
            this.lvContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.lvViewRolesMenuItem,
            this.lvViewUsersMenuItem});
            resources.ApplyResources(this.lvContextMenu, "lvContextMenu");
            // 
            // lvViewRolesMenuItem
            // 
            resources.ApplyResources(this.lvViewRolesMenuItem, "lvViewRolesMenuItem");
            this.lvViewRolesMenuItem.Index = 0;
            // 
            // lvViewUsersMenuItem
            // 
            resources.ApplyResources(this.lvViewUsersMenuItem, "lvViewUsersMenuItem");
            this.lvViewUsersMenuItem.Index = 1;
            // 
            // listViewLargeImageList
            // 
            this.listViewLargeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewLargeImageList.ImageStream")));
            this.listViewLargeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewLargeImageList.Images.SetKeyName(0, "");
            this.listViewLargeImageList.Images.SetKeyName(1, "");
            // 
            // listViewSmallImageList
            // 
            this.listViewSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewSmallImageList.ImageStream")));
            this.listViewSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewSmallImageList.Images.SetKeyName(0, "");
            this.listViewSmallImageList.Images.SetKeyName(1, "");
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            // 
            // SelectUserOrRoleDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "SelectUserOrRoleDialog";
            this.Load += new System.EventHandler(this.ChooseUserOrRoleDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Gets all roles and display them in the list view
		/// </summary>
		private void GetAllRoles()
		{
			// fetch the roles from server
			try
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

                // invoke the web service synchronously
                RolesListHandler roleListHandler = new RolesListHandler();
                _allRoles = roleListHandler.GetValues(new WindowClientUserManager());

                ShowRoles();
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
			finally
			{
				// Restore the cursor
				Cursor.Current = this.Cursor;
			}
		}

		private delegate void ShowRolesDelegate();

		/// <summary>
		/// Display the all available roles in the list view
		/// </summary>
		private void ShowRoles()
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue
				this.listView.SuspendLayout();
				this.listView.Items.Clear();

				// By default. add "Everyone" role
				ListViewItem item = new ListViewItem(XaclSubject.EveryOne, 0);
                item.SubItems.Add(XaclSubject.EveryOne);
				this.listView.Items.Add(item);

				if (_allRoles != null)
				{
                    foreach (EnumValue role in _allRoles)
                    {
                        item = new ListViewItem(role.Value, 1);
                        item.SubItems.Add(role.DisplayText);
                        this.listView.Items.Add(item);
                    }
				}

				this.listView.ResumeLayout();
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowRolesDelegate showRoles = new ShowRolesDelegate(ShowRoles);

				this.BeginInvoke(showRoles);
			}
		}

		/// <summary>
		/// Gets all users and display them in the list view
		/// </summary>
		private void GetAllUsers()
		{
			// fetch the users from server
			try
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				// invoke the web service synchronously
                UsersListHandler userListHandler = new UsersListHandler();
                _allUsers = userListHandler.GetValues(new WindowClientUserManager());

                ShowUsers();
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
			finally
			{
				// Restore the cursor
				Cursor.Current = this.Cursor;
			}
		}

		private delegate void ShowUsersDelegate();

		/// <summary>
		/// Display the all available users in the list view
		/// </summary>
		private void ShowUsers()
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue
				this.listView.SuspendLayout();
				this.listView.Items.Clear();

				// By default. add "Anonymous" user
				ListViewItem item = new ListViewItem(XaclSubject.AnonymousUser, 1);
                item.SubItems.Add(XaclSubject.AnonymousUser);
				this.listView.Items.Add(item);

				if (_allUsers != null)
				{
					foreach (EnumValue user in _allUsers)
					{
						item = new ListViewItem(user.Value, 1);
                        item.SubItems.Add(user.DisplayText);
						this.listView.Items.Add(item);
					}
				}

				this.listView.ResumeLayout();
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowUsersDelegate showUsers = new ShowUsersDelegate(ShowUsers);

				this.BeginInvoke(showUsers);
			}
		}

		#endregion

		private void ChooseUserOrRoleDialog_Load(object sender, System.EventArgs e)
		{
			// make role radio button checked initially
			this.userRadioButton.Checked = true;
		}

		private void userRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.roleRadioButton.Checked)
			{
				if (_allRoles == null)
				{
					GetAllRoles();
				}
				else
				{
					ShowRoles();
				}
			}
			else if (this.userRadioButton.Checked)
			{
				if (_allUsers == null)
				{
					GetAllUsers();
				}
				else
				{
					ShowUsers();
				}
			}
		}

		private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.okButton.Enabled = true;

			if (this.listView.SelectedItems.Count > 0)
			{
				if (this.roleRadioButton.Checked)
				{
					_selectedRoles = new string[this.listView.SelectedItems.Count];
					for (int i = 0; i < this.listView.SelectedItems.Count; i++)
					{
						_selectedRoles[i] = this.listView.SelectedItems[i].Text;
					}
					_selectedUser = null;
				}
				else if (this.userRadioButton.Checked)
				{
					// only allow single selection for user
					_selectedUser = this.listView.SelectedItems[0].Text;
					_selectedRoles = null;
				}
			}
			else
			{
				this._selectedRoles = null;
			}
		}
	}
}

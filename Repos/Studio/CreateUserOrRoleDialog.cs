using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.XaclModel;
using Newtera.WindowsControl;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for CreateUserOrRoleDialog.
	/// </summary>
	public class CreateUserOrRoleDialog : System.Windows.Forms.Form
	{
		private UserInfoServiceStub _userInfoService;
        private EnumValueCollection _allRoles;
		private EnumValueCollection _allUsers;
		private string[] _userRoles;
		private string[] _selectedRoles;
		private string _selectedUser;
        private string _selectedRole;
		private MenuItemStates _menuItemStates;
		private bool _isReadOnly;

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
		private System.Windows.Forms.MenuItem lvAddMenuItem;
		private System.Windows.Forms.MenuItem lvDeleteMenuItem;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem addMenuItem;
		private System.Windows.Forms.MenuItem deleteMenuItem;
		private System.Windows.Forms.MenuItem lvAssignRolesMenuItem;
		private System.Windows.Forms.MenuItem assignRolesMenuItem;
		private System.Windows.Forms.MenuItem lvChangeUserPwdMenuItem;
		private System.Windows.Forms.MenuItem changeUserPwdMenuItem;
		private System.Windows.Forms.MenuItem viewRolesMenuItem;
		private System.Windows.Forms.MenuItem viewUsersMenuItem;
		private System.Windows.Forms.MenuItem lvViewRolesMenuItem;
		private System.Windows.Forms.MenuItem lvViewUsersMenuItem;
        private MenuItem editUserInfoMenuItem;
        private MenuItem lvEditUserInfoMenuItem;
        private ColumnHeader columnHeader2;
		private System.ComponentModel.IContainer components;

		public CreateUserOrRoleDialog()
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
			_userRoles = null;
			_userInfoService = new UserInfoServiceStub();
			_selectedRoles = null;
			_selectedUser = null;
            _selectedRole = null;
			_isReadOnly = true;

			_menuItemStates = new MenuItemStates();
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
		/// Gets the menu item states
		/// </summary>
		public MenuItemStates MenuItemStates
		{
			get
			{
				return this._menuItemStates;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the dialog is
		/// read only.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			set
			{
				_isReadOnly = value;
			}
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

        /// <summary>
        /// Gets the selected role
        /// </summary>
        public string SelectedRole
        {
            get
            {
                return _selectedRole;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateUserOrRoleDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.userRadioButton = new System.Windows.Forms.RadioButton();
            this.roleRadioButton = new System.Windows.Forms.RadioButton();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lvContextMenu = new System.Windows.Forms.ContextMenu();
            this.lvAddMenuItem = new System.Windows.Forms.MenuItem();
            this.lvChangeUserPwdMenuItem = new System.Windows.Forms.MenuItem();
            this.lvAssignRolesMenuItem = new System.Windows.Forms.MenuItem();
            this.lvEditUserInfoMenuItem = new System.Windows.Forms.MenuItem();
            this.lvDeleteMenuItem = new System.Windows.Forms.MenuItem();
            this.lvViewRolesMenuItem = new System.Windows.Forms.MenuItem();
            this.lvViewUsersMenuItem = new System.Windows.Forms.MenuItem();
            this.listViewLargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.listViewSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.addMenuItem = new System.Windows.Forms.MenuItem();
            this.changeUserPwdMenuItem = new System.Windows.Forms.MenuItem();
            this.assignRolesMenuItem = new System.Windows.Forms.MenuItem();
            this.editUserInfoMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteMenuItem = new System.Windows.Forms.MenuItem();
            this.viewRolesMenuItem = new System.Windows.Forms.MenuItem();
            this.viewUsersMenuItem = new System.Windows.Forms.MenuItem();
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
            this.lvAddMenuItem,
            this.lvChangeUserPwdMenuItem,
            this.lvAssignRolesMenuItem,
            this.lvEditUserInfoMenuItem,
            this.lvDeleteMenuItem,
            this.lvViewRolesMenuItem,
            this.lvViewUsersMenuItem});
            resources.ApplyResources(this.lvContextMenu, "lvContextMenu");
            // 
            // lvAddMenuItem
            // 
            resources.ApplyResources(this.lvAddMenuItem, "lvAddMenuItem");
            this.lvAddMenuItem.Index = 0;
            this.lvAddMenuItem.Click += new System.EventHandler(this.lvAddMenuItem_Click);
            // 
            // lvChangeUserPwdMenuItem
            // 
            resources.ApplyResources(this.lvChangeUserPwdMenuItem, "lvChangeUserPwdMenuItem");
            this.lvChangeUserPwdMenuItem.Index = 1;
            this.lvChangeUserPwdMenuItem.Click += new System.EventHandler(this.lvChangeUserPwdMenuItem_Click);
            // 
            // lvAssignRolesMenuItem
            // 
            resources.ApplyResources(this.lvAssignRolesMenuItem, "lvAssignRolesMenuItem");
            this.lvAssignRolesMenuItem.Index = 2;
            this.lvAssignRolesMenuItem.Click += new System.EventHandler(this.lvAssignRolesMenuItem_Click);
            // 
            // lvEditUserInfoMenuItem
            // 
            resources.ApplyResources(this.lvEditUserInfoMenuItem, "lvEditUserInfoMenuItem");
            this.lvEditUserInfoMenuItem.Index = 3;
            this.lvEditUserInfoMenuItem.Click += new System.EventHandler(this.lvEditUserInfoMenuItem_Click);
            // 
            // lvDeleteMenuItem
            // 
            resources.ApplyResources(this.lvDeleteMenuItem, "lvDeleteMenuItem");
            this.lvDeleteMenuItem.Index = 4;
            this.lvDeleteMenuItem.Click += new System.EventHandler(this.lvDeleteMenuItem_Click);
            // 
            // lvViewRolesMenuItem
            // 
            resources.ApplyResources(this.lvViewRolesMenuItem, "lvViewRolesMenuItem");
            this.lvViewRolesMenuItem.Index = 5;
            this.lvViewRolesMenuItem.Click += new System.EventHandler(this.lvViewRolesMenuItem_Click);
            // 
            // lvViewUsersMenuItem
            // 
            resources.ApplyResources(this.lvViewUsersMenuItem, "lvViewUsersMenuItem");
            this.lvViewUsersMenuItem.Index = 6;
            this.lvViewUsersMenuItem.Click += new System.EventHandler(this.lvViewUsersMenuItem_Click);
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
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            resources.ApplyResources(this.mainMenu1, "mainMenu1");
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.addMenuItem,
            this.changeUserPwdMenuItem,
            this.assignRolesMenuItem,
            this.editUserInfoMenuItem,
            this.deleteMenuItem,
            this.viewRolesMenuItem,
            this.viewUsersMenuItem});
            // 
            // addMenuItem
            // 
            resources.ApplyResources(this.addMenuItem, "addMenuItem");
            this.addMenuItem.Index = 0;
            this.addMenuItem.Click += new System.EventHandler(this.addMenuItem_Click);
            // 
            // changeUserPwdMenuItem
            // 
            resources.ApplyResources(this.changeUserPwdMenuItem, "changeUserPwdMenuItem");
            this.changeUserPwdMenuItem.Index = 1;
            this.changeUserPwdMenuItem.Click += new System.EventHandler(this.changeUserPwdMenuItem_Click);
            // 
            // assignRolesMenuItem
            // 
            resources.ApplyResources(this.assignRolesMenuItem, "assignRolesMenuItem");
            this.assignRolesMenuItem.Index = 2;
            this.assignRolesMenuItem.Click += new System.EventHandler(this.assignRolesMenuItem_Click);
            // 
            // editUserInfoMenuItem
            // 
            resources.ApplyResources(this.editUserInfoMenuItem, "editUserInfoMenuItem");
            this.editUserInfoMenuItem.Index = 3;
            this.editUserInfoMenuItem.Click += new System.EventHandler(this.editUserInfoMenuItem_Click);
            // 
            // deleteMenuItem
            // 
            resources.ApplyResources(this.deleteMenuItem, "deleteMenuItem");
            this.deleteMenuItem.Index = 4;
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // viewRolesMenuItem
            // 
            resources.ApplyResources(this.viewRolesMenuItem, "viewRolesMenuItem");
            this.viewRolesMenuItem.Index = 5;
            this.viewRolesMenuItem.Click += new System.EventHandler(this.viewRolesMenuItem_Click);
            // 
            // viewUsersMenuItem
            // 
            resources.ApplyResources(this.viewUsersMenuItem, "viewUsersMenuItem");
            this.viewUsersMenuItem.Index = 6;
            this.viewUsersMenuItem.Click += new System.EventHandler(this.viewUsersMenuItem_Click);
            // 
            // CreateUserOrRoleDialog
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
            this.Menu = this.mainMenu1;
            this.Name = "CreateUserOrRoleDialog";
            this.Load += new System.EventHandler(this.ChooseUserOrRoleDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Initialize the menu item states
		/// </summary>
		private void InitializeMenuItemStates()
		{
			_menuItemStates.SetState(MenuItemID.EditAdd, true);
			_menuItemStates.SetState(MenuItemID.EditMap, false);
			_menuItemStates.SetState(MenuItemID.EditDelete, false);
			_menuItemStates.SetState(MenuItemID.EditModify, false);
			_menuItemStates.SetState(MenuItemID.ViewRoles, false);
			_menuItemStates.SetState(MenuItemID.ViewUsers, false);
		}

		/// <summary>
		/// Disable the menu item states for read only dialog
		/// </summary>
		private void DisableMenuItemStates()
		{
			_menuItemStates.SetState(MenuItemID.EditAdd, false);
			_menuItemStates.SetState(MenuItemID.EditMap, false);
			_menuItemStates.SetState(MenuItemID.EditDelete, false);
			_menuItemStates.SetState(MenuItemID.EditModify, false);
		}

		/// <summary>
		/// Change the enable state of menu items
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemStateChanged(object sender, System.EventArgs e)
		{
			StateChangedEventArgs args = (StateChangedEventArgs) e;

			// set the toolbar button states
			switch (args.ID)
			{
				case MenuItemID.EditAdd:
					this.addMenuItem.Enabled = args.State;
					this.lvAddMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditDelete:
					this.deleteMenuItem.Enabled = args.State;
					this.lvDeleteMenuItem.Enabled = args.State;					
					break;
				case MenuItemID.EditMap:
					this.assignRolesMenuItem.Enabled = args.State;
					this.lvAssignRolesMenuItem.Enabled = args.State;
					break;
				case MenuItemID.EditModify:
					this.changeUserPwdMenuItem.Enabled = args.State;
					this.lvChangeUserPwdMenuItem.Enabled = args.State;
                    this.editUserInfoMenuItem.Enabled = args.State;
                    this.lvEditUserInfoMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewRoles:
					this.viewRolesMenuItem.Enabled = args.State;
					this.lvViewRolesMenuItem.Enabled = args.State;
					break;
				case MenuItemID.ViewUsers:
					this.viewUsersMenuItem.Enabled = args.State;
					this.lvViewUsersMenuItem.Enabled = args.State;
					break;
			}
		}

		/// <summary>
		/// Gets all roles and display them in the list view
		/// </summary>
		private void GetAllRoles()
		{
			// fetch the roles from server
			try
			{
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

				if (!IsReadOnly)
				{
					InitializeMenuItemStates();
				}
				else
				{
					DisableMenuItemStates();
				}
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

				if (!IsReadOnly)
				{
					InitializeMenuItemStates();
				}
				else
				{
					DisableMenuItemStates();
				}
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowUsersDelegate showUsers = new ShowUsersDelegate(ShowUsers);

				this.BeginInvoke(showUsers);
			}
		}

		/// <summary>
		/// Launch the dialog to assign roles to an user
		/// </summary>
		private void ShowAssignRolesDialog()
		{
			try
			{
				if (_allRoles == null)
				{
                    RolesListHandler roleListHandler = new RolesListHandler();
                    _allRoles = roleListHandler.GetValues(new WindowClientUserManager());
				}

				_userRoles = _userInfoService.GetRoles(SelectedUser, null);

				AssignUserRolesDialog dialog = new AssignUserRolesDialog();

				dialog.UserName = SelectedUser;
				dialog.AllRoles = _allRoles;
				dialog.UserRoles = _userRoles;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					string[] selectedRoles = dialog.SelectedRoles;

					// add a mapping for each added role
					foreach (string role in selectedRoles)
					{
						if (IsAddedRole(role, _userRoles))
						{
							_userInfoService.AddUserRoleMapping(SelectedUser, role);
						}
					}

					// delete the mappings for deleted roles
					if (_userRoles != null)
					{
						foreach (string role in _userRoles)
						{
							if (IsDeletedRole(role, selectedRoles))
							{
								_userInfoService.DeleteUserRoleMapping(SelectedUser, role);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Launch the dialog to an user's password.
		/// </summary>
		private void ShowChangeUserPassword()
		{
			try
			{
				ChangeUserPwdDialog dialog = new ChangeUserPwdDialog();

				dialog.UserName = SelectedUser;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					_userInfoService.ChangeUserPassword(dialog.UserName,
						"NONE", dialog.NewPassword);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Gets the information indicating whether a given role is added to an user
		/// </summary>
		/// <param name="role">The given role</param>
		/// <param name="userRoles">The exitsing user roles</param>
		/// <returns>true if it is added, false oterwise</returns>
		private bool IsAddedRole(string role, string[] userRoles)
		{
			bool status = true;

			if (userRoles != null)
			{
				foreach (string userRole in userRoles)
				{
					if (role == userRole)
					{
						status = false;
						break;
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Gets the information indicating whether a given role is deleted from an user
		/// </summary>
		/// <param name="role">The given role</param>
		/// <param name="userRoles">The selected user roles</param>
		/// <returns>true if it is deleted, false oterwise</returns>
		private bool IsDeletedRole(string role, string[] userRoles)
		{
			bool status = true;

			if (userRoles != null)
			{
				foreach (string userRole in userRoles)
				{
					if (role == userRole)
					{
						status = false;
						break;
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Display a dialog to add a role
		/// </summary>
		private void showAddRoleDialog()
		{
			AddRoleDialog dialog = new AddRoleDialog();

			dialog.AllRoles = this._allRoles;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				GetAllRoles();
			}
		}

		/// <summary>
		/// Display a dialog to add an user
		/// </summary>
		private void showAddUserDialog()
		{
			AddUserDialog dialog = new AddUserDialog();

			dialog.AllUsers = this._allUsers;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				GetAllUsers();
			}
		}

		/// <summary>
		/// Delete the selected user
		/// </summary>
		private void deleteUser()
		{
			try
			{
                if (MessageBox.Show(WindowsControl.MessageResourceManager.GetString("DesignStudio.DeleteUser"),
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    _userInfoService.DeleteUser(this.SelectedUser);

                    GetAllUsers();

                    EnumTypeFactory.Instance.ClearEnumTypes(); // clear the cahed enums created for the users
                }
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// delete the selected role
		/// </summary>
		private void deleteRole()
		{
			try
			{
                if (MessageBox.Show(WindowsControl.MessageResourceManager.GetString("DesignStudio.DeleteRole"),
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int i = 0; i < this.SelectedRoles.Length; i++)
                    {
                        _userInfoService.DeleteRole(this.SelectedRoles[i]);
                    }

                    GetAllRoles();

                    EnumTypeFactory.Instance.ClearEnumTypes(); // clear the cahed enums created for the roles
                }
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// View the roles of the selected user
		/// </summary>
		private void ViewRoles()
		{
			try
			{
				string[] roles = _userInfoService.GetRoles(SelectedUser, null);

				ViewUserRolesDialog dialog = new ViewUserRolesDialog();

				dialog.UserName = SelectedUser;
				dialog.UserRoles = roles;

				dialog.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// View the users of the selected role
		/// </summary>
		private void ViewUsers()
		{
			try
			{
				string selectedRole = null;
				if (SelectedRoles.Length == 1)
				{
					selectedRole = SelectedRoles[0];
				}

				string[] users = null;
				
				if (selectedRole != null)
				{
					users = _userInfoService.GetUsers(selectedRole);
				}

				ViewRoleUsersDialog dialog = new ViewRoleUsersDialog();

				dialog.Role = selectedRole;
				dialog.Users = users;
                dialog.AllUsers = _allUsers;

				dialog.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

        /// <summary>
        /// Edit the selected user info
        /// </summary>
        private void EditUser()
        {
            try
            {

                string[] userData = _userInfoService.GetUserData(this.SelectedUser);

                EditUserInfoDialog dialog = new EditUserInfoDialog();
                dialog.UserID = this.SelectedUser;
                dialog.UserData = userData;
                dialog.AllUsers = _allUsers;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // update the user name display
                    string displayText = UsersListHandler.GetFormatedName(dialog.UserData[0], dialog.UserData[1]);
                    if (string.IsNullOrEmpty(displayText))
                    {
                        displayText = this.SelectedUser;
                    }

                    foreach (ListViewItem listViewItem in this.listView.Items)
                    {
                        if (listViewItem.Text == this.SelectedUser)
                        {
                            listViewItem.SubItems[1].Text = displayText;
                            break;
                        }
                    }

                    foreach (EnumValue enumValue in _allUsers)
                    {
                        if (enumValue.Value == this.SelectedUser)
                        {
                            enumValue.DisplayText = displayText;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Edit the selected role info
        /// </summary>
        private void EditRole()
        {
            try
            {
                EditRoleInfo dialog = new EditRoleInfo();
                dialog.RoleName = this.SelectedRole;
                foreach (EnumValue role in _allRoles)
                {
                    if (role.Value == this.SelectedRole)
                    {
                        dialog.RoleText = role.DisplayText;
                        break;
                    }
                }

                dialog.AllRoles = _allRoles;
                string displayText ;

                if (SelectedRole != null &&
                    dialog.ShowDialog() == DialogResult.OK)
                {
                    // update the role display text
                    if (string.IsNullOrEmpty(dialog.RoleText))
                    {
                        displayText = this.SelectedUser;
                    }
                    else
                    {
                        displayText = dialog.RoleText;
                    }

                    foreach (ListViewItem listViewItem in this.listView.Items)
                    {
                        if (listViewItem.Text == this.SelectedRole)
                        {
                            listViewItem.SubItems[1].Text = displayText;
                            break;
                        }
                    }

                    foreach (EnumValue enumValue in _allRoles)
                    {
                        if (enumValue.Value == this.SelectedRole)
                        {
                            enumValue.DisplayText = displayText;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

		#endregion

		private void ChooseUserOrRoleDialog_Load(object sender, System.EventArgs e)
		{
			// Register the menu item state change event handler
			_menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

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

                    if (this.listView.SelectedItems.Count == 1)
                    {
                        _selectedRole = this.listView.SelectedItems[0].Text;
                    }
                    else
                    {
                        _selectedRole = null;
                    }
					_selectedUser = null;
				}
				else if (this.userRadioButton.Checked)
				{
					// only allow single selection for user
					_selectedUser = this.listView.SelectedItems[0].Text;
					_selectedRoles = null;
                    _selectedRole = null;
				}
			}
			else
			{
				this._selectedRoles = null;
			}

			// set the button states, enable operation for the single selection
			if (this.listView.SelectedItems.Count == 1 && !_isReadOnly)
			{
				if (this.roleRadioButton.Checked)
				{
					// disable this button for role
					this._menuItemStates.SetState(MenuItemID.EditMap, false);
					this._menuItemStates.SetState(MenuItemID.EditModify, true);
					this._menuItemStates.SetState(MenuItemID.ViewRoles, false);
					this._menuItemStates.SetState(MenuItemID.ViewUsers, true);
					if (_selectedRoles[0] != XaclSubject.EveryOne)
					{
						this._menuItemStates.SetState(MenuItemID.EditDelete, true);
					}
					else
					{
						this._menuItemStates.SetState(MenuItemID.EditDelete, false);
					}
				}
				else
				{
					this._menuItemStates.SetState(MenuItemID.ViewRoles, true);
					this._menuItemStates.SetState(MenuItemID.ViewUsers, false);
                    if (this._selectedUser != XaclSubject.AnonymousUser)
                    {
                        this._menuItemStates.SetState(MenuItemID.EditMap, true);
                        this._menuItemStates.SetState(MenuItemID.EditDelete, true);
                        this._menuItemStates.SetState(MenuItemID.EditModify, true);
                    }
                    else
                    {
                        this._menuItemStates.SetState(MenuItemID.EditMap, false);
                        this._menuItemStates.SetState(MenuItemID.EditDelete, false);
                        this._menuItemStates.SetState(MenuItemID.EditModify, false);
                    }
				}
			}
			else
			{
				this._menuItemStates.SetState(MenuItemID.EditMap, false);
				this._menuItemStates.SetState(MenuItemID.EditDelete, false);
				this._menuItemStates.SetState(MenuItemID.EditModify, false);
			}

			if (this.listView.SelectedItems.Count == 1)
			{
				if (this.roleRadioButton.Checked)
				{
					this._menuItemStates.SetState(MenuItemID.ViewRoles, false);
					if (_selectedRoles[0] != XaclSubject.EveryOne)
					{
						this._menuItemStates.SetState(MenuItemID.ViewUsers, true);
					}
					else
					{
						this._menuItemStates.SetState(MenuItemID.ViewUsers, false);
					}
				}
				else
				{
					if (SelectedUser != XaclSubject.AnonymousUser)
					{
						this._menuItemStates.SetState(MenuItemID.ViewRoles, true);
					}
					else
					{
						this._menuItemStates.SetState(MenuItemID.ViewRoles, false);
					}
					this._menuItemStates.SetState(MenuItemID.ViewUsers, false);
				}
			}
			else
			{
				this._menuItemStates.SetState(MenuItemID.ViewRoles, false);
				this._menuItemStates.SetState(MenuItemID.ViewUsers, false);
			}
		}

		private void addMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.roleRadioButton.Checked)
			{
				showAddRoleDialog();
			}
			else
			{
				showAddUserDialog();
			}
		}

		private void lvAddMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.roleRadioButton.Checked)
			{
				showAddRoleDialog();
			}
			else
			{
				showAddUserDialog();
			}		
		}

		private void deleteMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.roleRadioButton.Checked)
			{
				deleteRole();
			}
			else
			{
				deleteUser();;
			}		
		}

		private void lvDeleteMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.roleRadioButton.Checked)
			{
				deleteRole();
			}
			else
			{
				deleteUser();;
			}		
		}

		private void assignRolesMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowAssignRolesDialog();
		}

		private void lvAssignRolesMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowAssignRolesDialog();		
		}

		private void lvChangeUserPwdMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowChangeUserPassword();
		}

		private void changeUserPwdMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowChangeUserPassword();
		}

		private void viewRolesMenuItem_Click(object sender, System.EventArgs e)
		{
			ViewRoles();
		}

		private void viewUsersMenuItem_Click(object sender, System.EventArgs e)
		{
			ViewUsers();
		}

		private void lvViewRolesMenuItem_Click(object sender, System.EventArgs e)
		{
			ViewRoles();
		}

		private void lvViewUsersMenuItem_Click(object sender, System.EventArgs e)
		{
			ViewUsers();
		}

        private void lvEditUserInfoMenuItem_Click(object sender, EventArgs e)
        {
            if (this.roleRadioButton.Checked)
            {
                EditRole();
            }
            else
            {
                EditUser();
            }
        }

        private void editUserInfoMenuItem_Click(object sender, EventArgs e)
        {
            if (this.roleRadioButton.Checked)
            {
                EditRole();
            }
            else
            {
                EditUser();
            }
        }
	}
}

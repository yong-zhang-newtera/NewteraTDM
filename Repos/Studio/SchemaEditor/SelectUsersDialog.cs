using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
    /// Summary description for SelectUsersDialog.
	/// </summary>
	public class SelectUsersDialog : System.Windows.Forms.Form
	{
		private EnumValueCollection _allUsers = null;
		private StringCollection _selectedUsers = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox allUsersListBox;
		private System.Windows.Forms.ListBox selectedUsersListBox;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectUsersDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            _selectedUsers = new StringCollection();
            _allUsers = new EnumValueCollection();
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
		/// Gets or sets the selected users.
		/// </summary>
		public StringCollection Users
		{
			get
			{
				return _selectedUsers;
			}
            set
            {
                _selectedUsers = value;
            }
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectUsersDialog));
            this.allUsersListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.selectedUsersListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // allUsersListBox
            // 
            resources.ApplyResources(this.allUsersListBox, "allUsersListBox");
            this.allUsersListBox.Name = "allUsersListBox";
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
            // selectedUsersListBox
            // 
            resources.ApplyResources(this.selectedUsersListBox, "selectedUsersListBox");
            this.selectedUsersListBox.Name = "selectedUsersListBox";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // delButton
            // 
            resources.ApplyResources(this.delButton, "delButton");
            this.delButton.Name = "delButton";
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // SelectUsersDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.selectedUsersListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.allUsersListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectUsersDialog";
            this.Load += new System.EventHandler(this.SelectUsersDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ShowItems()
		{
			this.allUsersListBox.BeginUpdate();

			this.allUsersListBox.Items.Clear();

			foreach (EnumValue usr in this._allUsers)
			{
				this.allUsersListBox.Items.Add(usr.Value + " (" + usr.DisplayText + ")");
			}

			this.allUsersListBox.EndUpdate();

			this.selectedUsersListBox.BeginUpdate();

			this.selectedUsersListBox.Items.Clear();

            bool found;
			foreach (string user in this._selectedUsers)
			{
                found = false;
                foreach (EnumValue userEnum in _allUsers)
                {
                    if (userEnum.Value == user)
                    {
                        found = true;
                        this.selectedUsersListBox.Items.Add(userEnum.Value + " (" + userEnum.DisplayText + ")");
                        break;
                    }
                }

                if (!found)
                {
                    this.selectedUsersListBox.Items.Add(user + " (Unknown)");
                }
			}

			this.selectedUsersListBox.EndUpdate();
		}

		private void SelectUsersDialog_Load(object sender, System.EventArgs e)
		{
            WindowClientUserManager userManager = new WindowClientUserManager();
            UsersListHandler userListHandler = new UsersListHandler();

            _allUsers = userListHandler.GetValues(userManager);

            ShowItems();
		}

        private void addButton_Click(object sender, System.EventArgs e)
		{
			int index = this.allUsersListBox.SelectedIndex;

			if (index >= 0 && index < _allUsers.Count)
			{
				EnumValue usr = _allUsers[index];
                if (!ContainsUser(_selectedUsers, usr.Value))
                {
                    _selectedUsers.Add(usr.Value);
                    this.selectedUsersListBox.Items.Add(usr.Value + " (" + usr.DisplayText + ")");
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.UserExist"), "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
			}
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			int index = this.selectedUsersListBox.SelectedIndex;

			if (index >= 0 && index < _selectedUsers.Count)
			{
				_selectedUsers.RemoveAt(index);
                this.selectedUsersListBox.Items.RemoveAt(index);
			}		
		}

        private bool ContainsUser(StringCollection users, string user)
        {
            bool status = false;

            foreach (string temp in users)
            {
                if (user == temp)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for AssignUserRolesDialog.
	/// </summary>
	public class AssignUserRolesDialog : System.Windows.Forms.Form
	{
		private string _user;
		private string[] _userRoles;
        private EnumValueCollection _allRoles;
		private string[] _selectedRoles;

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label userLabel;
		private System.Windows.Forms.ListView rolesListView;
		private System.Windows.Forms.ColumnHeader rolesColumnHeader;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AssignUserRolesDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_user = null;
			_userRoles = null;
			_allRoles = null;
			_selectedRoles = null;
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
		/// Gets or sets the user to which to assign roles
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
		/// Gets or sets the roles of the given user
		/// </summary>
		public string[] UserRoles
		{
			get
			{
				return _userRoles;
			}
			set
			{
				_userRoles = value;
			}
		}

		/// <summary>
		/// Gets or sets the all available roles
		/// </summary>
        public EnumValueCollection AllRoles
		{
			get
			{
				return _allRoles;
			}
			set
			{
				_allRoles = value;
			}
		}

		/// <summary>
		/// Gets the selected roles
		/// </summary>
		/// <value>A array of selected roles.</value>
		public string[] SelectedRoles
		{
			get
			{
				return _selectedRoles;
			}
		}

		/// <summary>
		/// Gets the information indicating whether a given role
		/// has been assigned to an user
		/// </summary>
		/// <value>true if it is assigned, false otherwise.</value>
		private bool IsRoleAssigned(string name)
		{
			bool status = false;

			if (_userRoles != null)
			{
				foreach (string role in this._userRoles)
				{
					if (role == name)
					{
						status = true;
						break;
					}
				}
			}

			return status;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssignUserRolesDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.userLabel = new System.Windows.Forms.Label();
            this.rolesListView = new System.Windows.Forms.ListView();
            this.rolesColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // userLabel
            // 
            resources.ApplyResources(this.userLabel, "userLabel");
            this.userLabel.Name = "userLabel";
            // 
            // rolesListView
            // 
            resources.ApplyResources(this.rolesListView, "rolesListView");
            this.rolesListView.CheckBoxes = true;
            this.rolesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.rolesColumnHeader});
            this.rolesListView.Name = "rolesListView";
            this.rolesListView.UseCompatibleStateImageBehavior = false;
            this.rolesListView.View = System.Windows.Forms.View.Details;
            // 
            // rolesColumnHeader
            // 
            resources.ApplyResources(this.rolesColumnHeader, "rolesColumnHeader");
            // 
            // AssignUserRolesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.rolesListView);
            this.Controls.Add(this.userLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssignUserRolesDialog";
            this.Load += new System.EventHandler(this.AssignUserRolesDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void AssignUserRolesDialog_Load(object sender, System.EventArgs e)
		{
		
			this.userLabel.Text = _user;

			// show all roles in the list view
			if (_allRoles != null)
			{
				foreach (EnumValue role in _allRoles)
				{
					ListViewItem item = new ListViewItem(role.Value);
                    item.SubItems.Add(role.DisplayText);

					if (IsRoleAssigned(role.Value))
					{
						item.Checked = true;
					}

					this.rolesListView.Items.Add(item);
				}
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			_selectedRoles = new string[this.rolesListView.CheckedItems.Count];

			int index = 0;
			foreach (ListViewItem item in this.rolesListView.CheckedItems)
			{
				_selectedRoles[index++] = item.Text;
			}
		}
	}
}

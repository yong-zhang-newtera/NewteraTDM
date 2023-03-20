using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ChooseAttributeUpdatedDialog.
	/// </summary>
	public class SelectRolesDialog : System.Windows.Forms.Form
	{
		private StringCollection _allRoles = null;
		private StringCollection _selectedRoles = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox allRolesListBox;
		private System.Windows.Forms.ListBox selectedRolesListBox;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectRolesDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            _selectedRoles = new StringCollection();
            _allRoles = new StringCollection();
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
		/// Gets or sets the selected roles.
		/// </summary>
		public StringCollection Roles
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectRolesDialog));
            this.allRolesListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.selectedRolesListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // allRolesListBox
            // 
            resources.ApplyResources(this.allRolesListBox, "allRolesListBox");
            this.allRolesListBox.Name = "allRolesListBox";
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
            // selectedRolesListBox
            // 
            resources.ApplyResources(this.selectedRolesListBox, "selectedRolesListBox");
            this.selectedRolesListBox.Name = "selectedRolesListBox";
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
            // SelectRolesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.selectedRolesListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.allRolesListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectRolesDialog";
            this.Load += new System.EventHandler(this.SelectRolesDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ShowItems()
		{
			this.allRolesListBox.BeginUpdate();

			this.allRolesListBox.Items.Clear();

			foreach (string usr in this._allRoles)
			{
				this.allRolesListBox.Items.Add(usr);
			}

			this.allRolesListBox.EndUpdate();

			this.selectedRolesListBox.BeginUpdate();

			this.selectedRolesListBox.Items.Clear();

			foreach (string role in this._selectedRoles)
			{
                this.selectedRolesListBox.Items.Add(role);
			}

			this.selectedRolesListBox.EndUpdate();
		}

		private void SelectRolesDialog_Load(object sender, System.EventArgs e)
		{
            WindowClientUserManager userManager = new WindowClientUserManager();
            string[] allRoles = userManager.GetAllRoles();

            for (int i = 0; i < allRoles.Length; i++)
            {
                if (!ContainsRole(_selectedRoles, allRoles[i]))
                {
                    _allRoles.Add(allRoles[i]);
                }
            }
            ShowItems();
		}

        private void addButton_Click(object sender, System.EventArgs e)
		{
			int index = this.allRolesListBox.SelectedIndex;

			if (index >= 0 && index < _allRoles.Count)
			{
				string usr = _allRoles[index];
                _allRoles.RemoveAt(index);
                this.allRolesListBox.Items.RemoveAt(index);
				_selectedRoles.Add(usr);
                this.selectedRolesListBox.Items.Add(usr);
			}
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			int index = this.selectedRolesListBox.SelectedIndex;

			if (index >= 0 && index < _selectedRoles.Count)
			{
                string usr = _selectedRoles[index];
				_selectedRoles.RemoveAt(index);
                this.selectedRolesListBox.Items.RemoveAt(index);
                _allRoles.Add(usr);
                this.allRolesListBox.Items.Add(usr);
			}		
		}

        private bool ContainsRole(StringCollection roles, string role)
        {
            bool status = false;

            foreach (string temp in roles)
            {
                if (role == temp)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }
	}
}

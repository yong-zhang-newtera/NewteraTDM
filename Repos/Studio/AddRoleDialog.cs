using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for AddRoleDialog.
	/// </summary>
	public class AddRoleDialog : System.Windows.Forms.Form
	{
		private WindowClientUserManager _userManager;
        private EnumValueCollection _allRoles;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TextBox roleNameTextBox;
        private RadioButton unitRadioButton;
        private RadioButton functionRadioButton;
        private Label label3;
        private Label label2;
        private TextBox roleTextTextBox;
        private IContainer components;

		public AddRoleDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_userManager = new WindowClientUserManager();
			_allRoles = null;
		}

		/// <summary>
		/// Gets or sets the all existing roles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddRoleDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.roleNameTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.functionRadioButton = new System.Windows.Forms.RadioButton();
            this.unitRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.roleTextTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // roleNameTextBox
            // 
            resources.ApplyResources(this.roleNameTextBox, "roleNameTextBox");
            this.errorProvider.SetIconAlignment(this.roleNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("roleNameTextBox.IconAlignment"))));
            this.roleNameTextBox.Name = "roleNameTextBox";
            this.roleNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.roleNameTextBox_Validating);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.errorProvider.SetIconAlignment(this.okButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("okButton.IconAlignment"))));
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.errorProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment"))));
            this.cancelButton.Name = "cancelButton";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // functionRadioButton
            // 
            resources.ApplyResources(this.functionRadioButton, "functionRadioButton");
            this.functionRadioButton.Checked = true;
            this.errorProvider.SetIconAlignment(this.functionRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("functionRadioButton.IconAlignment"))));
            this.functionRadioButton.Name = "functionRadioButton";
            this.functionRadioButton.TabStop = true;
            this.functionRadioButton.UseVisualStyleBackColor = true;
            // 
            // unitRadioButton
            // 
            resources.ApplyResources(this.unitRadioButton, "unitRadioButton");
            this.errorProvider.SetIconAlignment(this.unitRadioButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("unitRadioButton.IconAlignment"))));
            this.unitRadioButton.Name = "unitRadioButton";
            this.unitRadioButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // roleTextTextBox
            // 
            resources.ApplyResources(this.roleTextTextBox, "roleTextTextBox");
            this.roleTextTextBox.Name = "roleTextTextBox";
            // 
            // AddRoleDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.roleTextTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.unitRadioButton);
            this.Controls.Add(this.functionRadioButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.roleNameTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddRoleDialog";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		/// <summary>
		/// Gets the information indicating whether the given role name is unique
		/// </summary>
		/// <param name="name">role name</param>
		/// <returns>true if it is unique, false otherwise</returns>
		private bool ValidateRoleNameUniqueness(string name)
		{
			bool status = true;

			if (_allRoles != null)
			{
                foreach (EnumValue role in _allRoles)
				{
					if (role.Value == name)
					{
						status = false;
						break;
					}
				}
			}

			return status;
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// validate the text in the textbox
			this.roleNameTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

            string type = "Function";
            if (this.unitRadioButton.Checked)
            {
                type = "Unit";
            }

            string[] roleData = new string[2];
            roleData[0] = this.roleTextTextBox.Text;
            roleData[1] = type;
            _userManager.AddRole(this.roleNameTextBox.Text, roleData);

            // clear the enum types created for the role list
            EnumTypeFactory.Instance.ClearEnumTypes();
		}

		private void roleNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null and has to be unique
			if (this.roleNameTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.roleNameTextBox, "Please enter a role name.");
				e.Cancel = true;
			}
			else if (!ValidateRoleNameUniqueness(this.roleNameTextBox.Text))
			{
				this.errorProvider.SetError(this.roleNameTextBox, "The role has already existed.");
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}

        private void AddRoleDialog_Load(object sender, EventArgs e)
        {

        }
	}
}

using System;
using System.Threading;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Principal;
using Newtera.WindowsControl;

namespace SmartWord2013
{
	/// <summary>
	/// Summary description for AdminLoginDialog.
	/// </summary>
	public class AdminLoginDialog : System.Windows.Forms.Form
	{
		private WindowClientUserManager _userManager;
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox idTextBox;
		private System.Windows.Forms.Label label3;
		private Newtera.WindowsControl.EnterTextBox pwdTextBox;
		private System.ComponentModel.IContainer components;

		public AdminLoginDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_userManager = new WindowClientUserManager();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminLoginDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pwdTextBox = new Newtera.WindowsControl.EnterTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // idTextBox
            // 
            this.idTextBox.AccessibleDescription = null;
            this.idTextBox.AccessibleName = null;
            resources.ApplyResources(this.idTextBox, "idTextBox");
            this.idTextBox.BackgroundImage = null;
            this.idTextBox.Font = null;
            this.idTextBox.Name = "idTextBox";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // pwdTextBox
            // 
            this.pwdTextBox.AccessibleDescription = null;
            this.pwdTextBox.AccessibleName = null;
            resources.ApplyResources(this.pwdTextBox, "pwdTextBox");
            this.pwdTextBox.BackgroundImage = null;
            this.pwdTextBox.Font = null;
            this.pwdTextBox.Name = "pwdTextBox";
            this.pwdTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pwdTextBox_KeyDown);
            // 
            // AdminLoginDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.pwdTextBox);
            this.Controls.Add(this.idTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdminLoginDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		#region controller code

		private bool AuthenticateSuperUser(object sender, System.EventArgs e)
		{
			bool authenticated = false;
			string userName = this.idTextBox.Text;
			string password = this.pwdTextBox.Text;

			// Change the cursor to indicate that we are waiting
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				if (userName == _userManager.GetSuperUserName())
				{
					if (_userManager.AuthenticateSuperUser(userName, password))
					{
						// attach a custom principal object to the thread
						CustomPrincipal.Attach(new WindowClientUserManager(), new WindowClientServerProxy(), userName);

						authenticated = true;
					}
					else
					{
						MessageBox.Show(MessageResourceManager.GetString("DesignStudio.InvalidAdminPassword"), "Error Dialog", MessageBoxButtons.OK,
							MessageBoxIcon.Error);						
					}
				}
				else
				{
					
					MessageBox.Show(MessageResourceManager.GetString("DesignStudio.InvalidAdminID"), "Error Dialog", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
			finally
			{
				Cursor.Current = this.Cursor;
			}

			return authenticated;
		}

		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (!AuthenticateSuperUser(sender, e))
			{
				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}

		private void pwdTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) 
			{
				e.Handled = true;
				if (AuthenticateSuperUser(sender, e))
				{
					this.DialogResult = DialogResult.OK; // close the dialog
				}
			}
		}
	}
}

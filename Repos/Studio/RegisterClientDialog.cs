using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// RegisterClientDialog 的摘要说明。
	/// </summary>
	public class RegisterClientDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox clientIDTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox registeredClientsListBox;
		private System.Windows.Forms.Button RegisterButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RegisterClientDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterClientDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.clientIDTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.registeredClientsListBox = new System.Windows.Forms.ListBox();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // clientIDTextBox
            // 
            this.clientIDTextBox.AccessibleDescription = null;
            this.clientIDTextBox.AccessibleName = null;
            resources.ApplyResources(this.clientIDTextBox, "clientIDTextBox");
            this.clientIDTextBox.BackgroundImage = null;
            this.clientIDTextBox.Font = null;
            this.clientIDTextBox.Name = "clientIDTextBox";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // registeredClientsListBox
            // 
            this.registeredClientsListBox.AccessibleDescription = null;
            this.registeredClientsListBox.AccessibleName = null;
            resources.ApplyResources(this.registeredClientsListBox, "registeredClientsListBox");
            this.registeredClientsListBox.BackgroundImage = null;
            this.registeredClientsListBox.Font = null;
            this.registeredClientsListBox.Name = "registeredClientsListBox";
            // 
            // RegisterButton
            // 
            this.RegisterButton.AccessibleDescription = null;
            this.RegisterButton.AccessibleName = null;
            resources.ApplyResources(this.RegisterButton, "RegisterButton");
            this.RegisterButton.BackgroundImage = null;
            this.RegisterButton.Font = null;
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.AccessibleDescription = null;
            this.removeButton.AccessibleName = null;
            resources.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.BackgroundImage = null;
            this.removeButton.Font = null;
            this.removeButton.Name = "removeButton";
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
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
            // RegisterClientDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.RegisterButton);
            this.Controls.Add(this.registeredClientsListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clientIDTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "RegisterClientDialog";
            this.Load += new System.EventHandler(this.RegisterClientDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void RegisterButton_Click(object sender, System.EventArgs e)
		{
			if (this.clientIDTextBox.Text != null &&
				this.clientIDTextBox.Text.Length > 0)
			{
				try
				{
					AdminServiceStub service = new AdminServiceStub();

                    string machineName = System.Environment.MachineName;
                    if (machineName == null)
                    {
                        machineName = "Unknown";
                    }

					service.RegisterClient(NewteraNameSpace.DESIGN_STUDIO_NAME,
                        this.clientIDTextBox.Text, machineName);

                    MessageBox.Show(MessageResourceManager.GetString("DesignStudio.ClientAdded"),
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Server Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}

				ShowRegisteredClients();
			}
		}

		private void removeButton_Click(object sender, System.EventArgs e)
		{
			if (registeredClientsListBox.SelectedIndex >= 0)
			{
				try
				{
					string clientId = (string) registeredClientsListBox.SelectedItem;
					AdminServiceStub service = new AdminServiceStub();

					service.UnregisterClient(NewteraNameSpace.DESIGN_STUDIO_NAME,
						clientId);

                    MessageBox.Show(MessageResourceManager.GetString("DesignStudio.ClientRemoved"),
                                    "Info",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Server Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}

				ShowRegisteredClients();
			}
		}

		private void RegisterClientDialog_Load(object sender, System.EventArgs e)
		{
			this.clientIDTextBox.Text = NewteraNameSpace.ComputerCheckSum;

			ShowRegisteredClients();
		}

		/// <summary>
		/// Show the registered clients in the list box
		/// </summary>
		private void ShowRegisteredClients()
		{
			try
			{
				AdminServiceStub service = new AdminServiceStub();

				// clear the list box
				this.registeredClientsListBox.Items.Clear();

				// get a list of registered client ids
				string[] clientIds = service.GetRegisteredClients(NewteraNameSpace.DESIGN_STUDIO_NAME);
				for (int i = 0; i < clientIds.Length; i++)
				{
					this.registeredClientsListBox.Items.Add(clientIds[i]);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}
	}
}

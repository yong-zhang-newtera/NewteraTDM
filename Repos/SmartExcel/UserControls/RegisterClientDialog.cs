using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using SmartExcel.AdminWebService;
using Newtera.WindowsControl;

namespace SmartExcel
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
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // clientIDTextBox
            // 
            resources.ApplyResources(this.clientIDTextBox, "clientIDTextBox");
            this.clientIDTextBox.Name = "clientIDTextBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // registeredClientsListBox
            // 
            resources.ApplyResources(this.registeredClientsListBox, "registeredClientsListBox");
            this.registeredClientsListBox.Name = "registeredClientsListBox";
            // 
            // RegisterButton
            // 
            resources.ApplyResources(this.RegisterButton, "RegisterButton");
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // removeButton
            // 
            resources.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.Name = "removeButton";
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // RegisterClientDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.RegisterButton);
            this.Controls.Add(this.registeredClientsListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clientIDTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
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
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				try
				{
					AdminService service = new AdminService();

                    string machineName = System.Net.Dns.GetHostName();
                    if (machineName == null)
                    {
                        machineName = "Unknown";
                    }

					service.RegisterClient(NewteraNameSpace.SMART_WORD_NAME,
                        this.clientIDTextBox.Text, machineName);
                    MessageBox.Show(MessageResourceManager.GetString("SmartWord.ClientAdded"),
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
				finally
				{
					Cursor.Current = this.Cursor;
				}

				ShowRegisteredClients();
			}
		}

		private void removeButton_Click(object sender, System.EventArgs e)
		{
			if (registeredClientsListBox.SelectedIndex >= 0)
			{
				// Change the cursor to indicate that we are waiting
				Cursor.Current = Cursors.WaitCursor;

				try
				{
					string clientId = (string) registeredClientsListBox.SelectedItem;
					AdminService service = new AdminService();

					service.UnregisterClient(NewteraNameSpace.SMART_WORD_NAME,
						clientId);
                    MessageBox.Show(MessageResourceManager.GetString("SmartWord.ClientRemoved"),
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
				finally
				{
					Cursor.Current = this.Cursor;
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
			// Change the cursor to indicate that we are waiting
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				AdminService service = new AdminService();

				// clear the list box
				this.registeredClientsListBox.Items.Clear();

				// get a list of registered client ids
				string[] clientIds = service.GetRegisteredClients(NewteraNameSpace.SMART_WORD_NAME);
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
			finally
			{
				Cursor.Current = this.Cursor;
			}
		}
	}
}

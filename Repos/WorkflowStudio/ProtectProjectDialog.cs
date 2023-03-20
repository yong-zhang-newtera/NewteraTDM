using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;

using Newtera.Common.MetaData.Principal;
using Newtera.WinClientCommon;
using Newtera.WorkflowStudioControl;
using Newtera.WFModel;

namespace WorkflowStudio
{
	/// <summary>
	/// ProtectProjectDialog 的摘要说明。
	/// </summary>
	public class ProtectProjectDialog : System.Windows.Forms.Form
	{
        private WorkflowModelServiceStub _workflowModelService;
        private System.Windows.Forms.ListView projectListView;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button showRolesButton;
		private System.Windows.Forms.RadioButton notProtectedRadioButton;
		private System.Windows.Forms.RadioButton protectedRadioButton;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
		private System.ComponentModel.IContainer components;

		public ProtectProjectDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			_workflowModelService = new WorkflowModelServiceStub();
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

		/// <summary>
		/// Display the project names in the list view
		/// </summary>
		/// <param name="projectInfos">The array of project info</param>
		private void ShowProjectInfos(ProjectInfo[] projects)
		{			
			this.projectListView.BeginUpdate();
			ListViewItem item;
			int index = 0;
            foreach (ProjectInfo projectInfo in projects)
			{
                item = this.projectListView.Items.Add(projectInfo.Name, 0);
                item.SubItems.Add(projectInfo.Version);
				if (index == 0)
				{
					// select the first item as default
					item.Selected = true;
				}

				index++;
			}

			this.projectListView.EndUpdate();
		}

		/// <summary>
		/// Set the protect mode according to the selection
		/// </summary>
		/// <returns>true if setting mode successfully, false otherwise.</returns>
		private bool SetProtectMode()
		{
			bool status = true;

			if (projectListView.SelectedItems.Count == 1)
			{
				string projectName = projectListView.SelectedItems[0].Text;
                string projectVersion = projectListView.SelectedItems[0].SubItems[1].Text;

				string connectionString = ConnectionStringBuilder.Instance.Create();
				if (this.protectedRadioButton.Checked)
				{
					if (this.textBox1.Text == null ||
						this.textBox1.Text.Length == 0)
					{
						MessageBox.Show(MessageResourceManager.GetString("WorkflowStudio.MissingRole"));
						status = false;
					}
					else
					{
						// protected mode, set the selected role
						_workflowModelService.SetDBARole(connectionString, projectName,
                            projectVersion,
                            this.textBox1.Text);
					}
				}
				else
				{
					// non-protected mode, set role to null
					_workflowModelService.SetDBARole(connectionString, projectName, projectVersion, null);
				}
			}
			
			return status;
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtectProjectDialog));
            this.projectListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.notProtectedRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.showRolesButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.protectedRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectListView
            // 
            this.projectListView.AccessibleDescription = null;
            this.projectListView.AccessibleName = null;
            resources.ApplyResources(this.projectListView, "projectListView");
            this.projectListView.BackgroundImage = null;
            this.projectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.projectListView.Font = null;
            this.projectListView.FullRowSelect = true;
            this.projectListView.HideSelection = false;
            this.projectListView.MultiSelect = false;
            this.projectListView.Name = "projectListView";
            this.projectListView.SmallImageList = this.imageList1;
            this.projectListView.UseCompatibleStateImageBehavior = false;
            this.projectListView.View = System.Windows.Forms.View.Details;
            this.projectListView.SelectedIndexChanged += new System.EventHandler(this.schemaListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Project.bmp");
            // 
            // applyButton
            // 
            this.applyButton.AccessibleDescription = null;
            this.applyButton.AccessibleName = null;
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.BackgroundImage = null;
            this.applyButton.Font = null;
            this.applyButton.Name = "applyButton";
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
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
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // notProtectedRadioButton
            // 
            this.notProtectedRadioButton.AccessibleDescription = null;
            this.notProtectedRadioButton.AccessibleName = null;
            resources.ApplyResources(this.notProtectedRadioButton, "notProtectedRadioButton");
            this.notProtectedRadioButton.BackgroundImage = null;
            this.notProtectedRadioButton.Font = null;
            this.notProtectedRadioButton.Name = "notProtectedRadioButton";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.showRolesButton);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // showRolesButton
            // 
            this.showRolesButton.AccessibleDescription = null;
            this.showRolesButton.AccessibleName = null;
            resources.ApplyResources(this.showRolesButton, "showRolesButton");
            this.showRolesButton.BackgroundImage = null;
            this.showRolesButton.Font = null;
            this.showRolesButton.Name = "showRolesButton";
            this.showRolesButton.Click += new System.EventHandler(this.showRolesButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.AccessibleDescription = null;
            this.textBox1.AccessibleName = null;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.BackgroundImage = null;
            this.textBox1.Font = null;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // protectedRadioButton
            // 
            this.protectedRadioButton.AccessibleDescription = null;
            this.protectedRadioButton.AccessibleName = null;
            resources.ApplyResources(this.protectedRadioButton, "protectedRadioButton");
            this.protectedRadioButton.BackgroundImage = null;
            this.protectedRadioButton.Font = null;
            this.protectedRadioButton.Name = "protectedRadioButton";
            // 
            // ProtectProjectDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.protectedRadioButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.notProtectedRadioButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.projectListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "ProtectProjectDialog";
            this.Load += new System.EventHandler(this.ProtectDatabaseDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (!SetProtectMode())
			{
				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}

		private void applyButton_Click(object sender, System.EventArgs e)
		{
			SetProtectMode();
		}

		private void showRolesButton_Click(object sender, System.EventArgs e)
		{
			SelectRolesDialog dialog = new SelectRolesDialog();
            StringCollection roles = GetShownRoles();
            dialog.Roles = roles;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				this.textBox1.Text = ConvertArrayToString(dialog.Roles);
			}
		}

		/// <summary>
		/// Convert a string array into a semi-colon separated string.
		/// </summary>
		/// <param name="values">An string array</param>
		/// <returns>A semi-colon separated string</returns>
		private string ConvertArrayToString(StringCollection values)
		{
			string converted = null;
			foreach (string val in values)
			{
				if (converted == null)
				{
					converted = val;
				}
				else
				{
					converted += ";";
					converted += val;
				}
			}

			return converted;
		}

        private StringCollection GetShownRoles()
        {
            StringCollection roles = new StringCollection();

            if (this.textBox1.Text != null && this.textBox1.Text.Length > 0)
            {
                string[] values = this.textBox1.Text.Split(';');
                foreach (string role in values)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }

		private void ProtectDatabaseDialog_Load(object sender, System.EventArgs e)
		{
			// invoke the web service asynchronously
			ProjectInfo[] projects = _workflowModelService.GetExistingProjectInfos();

            ShowProjectInfos(projects);
		}

		private void schemaListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (projectListView.SelectedItems.Count == 1)
			{
                string selectedProjectName = projectListView.SelectedItems[0].Text;
                string selectedProjectVersion = projectListView.SelectedItems[0].SubItems[1].Text;

				// invoke the web service synchronously
                string roles = _workflowModelService.GetDBARole(ConnectionStringBuilder.Instance.Create(), selectedProjectName, selectedProjectVersion);

				if (roles != null)
				{
					// protected mode
					this.protectedRadioButton.Checked = true;
					this.textBox1.Text = roles;
				}
				else
				{
					// non-protected mode
					this.notProtectedRadioButton.Checked = true;
					this.textBox1.Text = null;
				}
			}
		}
	}
}

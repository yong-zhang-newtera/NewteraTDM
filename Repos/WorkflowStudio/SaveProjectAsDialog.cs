using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WorkflowStudioControl;
using Newtera.WinClientCommon;
using Newtera.WFModel;

namespace WorkflowStudio
{
    public partial class SaveProjectAsDialog : Form
    {
        private bool _isOverridingExistingProject = false;
        private string _projectName = null;
        private string _projectVersion = "1.0"; // default
        private ProjectInfo[] _existingProjectInfos = null;

        public SaveProjectAsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the name of project to be saved
        /// </summary>
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of project to be saved
        /// </summary>
        public string ProjectVersion
        {
            get
            {
                return _projectVersion;
            }
        }

        /// <summary>
        /// Gets information indicating whether it is overriding an existing project
        /// </summary>
        public bool IsOverridingExistingProject
        {
            get
            {
                return _isOverridingExistingProject;
            }
        }

        private ProjectInfo GetExistingProject(string projectName)
        {
            ProjectInfo existingProject = null;

            if (_existingProjectInfos != null)
            {
                foreach (ProjectInfo projectInfo in this._existingProjectInfos)
                {
                    if (projectInfo.Name.ToUpper() == projectName.ToUpper())
                    {
                        if (existingProject == null)
                        {
                            existingProject = projectInfo;
                        }
                        else if (string.Compare(projectInfo.Version, existingProject.Version) > 0)
                        {
                            // use the latest version
                            existingProject = projectInfo;
                        }
                    }
                }
            }

            return existingProject;
        }

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                this.nameTextBox.Focus();
                if (!this.Validate())
                {
                    return;
                }
                else
                {
                    _projectName = nameTextBox.Text;
                    ProjectInfo existingProject = GetExistingProject(_projectName);
                    if (existingProject != null)
                    {
                        _projectVersion = existingProject.Version;
                        _isOverridingExistingProject = true;
                    }
                    else
                    {
                        _isOverridingExistingProject = false;
                    }

                    this.DialogResult = DialogResult.OK; // close the dialog
                }
            }		
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // validate the text in nameTextBox
            this.nameTextBox.Focus();
            if (!this.Validate())
            {
                this.DialogResult = DialogResult.None;
                return;
            }
            else
            {
                _projectName = nameTextBox.Text;
                ProjectInfo existingProject = GetExistingProject(_projectName);
                if (existingProject != null)
                {
                    _projectVersion = existingProject.Version;
                    _isOverridingExistingProject = true;
                }
                else
                {
                    _isOverridingExistingProject = false;
                }
            }
        }

        private void SaveProjectAsDialog_Load(object sender, EventArgs e)
        {
            // show the list of the existing projects
            WorkflowModelServiceStub service = new WorkflowModelServiceStub();

            _existingProjectInfos = service.GetExistingProjectInfos();
            ListViewItem listViewItem;
            for (int i = 0; i < _existingProjectInfos.Length; i++)
            {
                listViewItem = new ListViewItem(_existingProjectInfos[i].Name);
                listViewItem.ImageIndex = 0;
                listViewItem.StateImageIndex = 0;
                listViewItem.SubItems.Add(_existingProjectInfos[i].Version);

                this.listView1.Items.Add(listViewItem);
            }

            // set the default name
            if (_projectName != null)
            {
                this.nameTextBox.Text = _projectName;
            }
        }

        private void nameTextBox_Validating(object sender, CancelEventArgs e)
        {
            // the name cannot be null
            if (this.nameTextBox.Text.Length == 0)
            {
                this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("WorkflowStudio.EnterName"));
                e.Cancel = true;
            }
            else
            {
                this.errorProvider.SetError((Control)sender, null);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 1)
            {
                this.nameTextBox.Text = listView1.SelectedItems[0].Text;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WFModel;
using Newtera.WorkflowStudioControl;
using Newtera.WinClientCommon;

namespace WorkflowStudio
{
    public partial class DeleteProjectDialog : Form
    {
        private string _projectName = null;
        private string _projectVersion = null;
        private ProjectInfo[] _existingProjectInfos = null;

        public DeleteProjectDialog()
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
            set
            {
                _projectVersion = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count == 1)
            {
                _projectName = this.listView1.SelectedItems[0].Text;
                _projectVersion = this.listView1.SelectedItems[0].SubItems[1].Text;
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
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count == 1)
            {
                this.okButton.Enabled = true;
            }
        }
    }
}
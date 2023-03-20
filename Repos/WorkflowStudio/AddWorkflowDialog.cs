using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WFModel;
using Newtera.WorkflowStudioControl;

namespace WorkflowStudio
{
    public partial class AddWorkflowDialog : Form
    {
        private string _name;
        private WorkflowType _workflowType;

        public AddWorkflowDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets name of the workflow to be added
        /// </summary>
        public string WorkflowName
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets type of the workflow to be added
        /// </summary>
        public WorkflowType WorkflowType
        {
            get
            {
                return _workflowType;
            }
        }

        private void SetWorkflowType()
        {
            this._name = this.nameTextBox.Text;
            if (this.listView1.SelectedIndices.Count == 1)
            {
                ListViewItem selectedItem = this.listView1.SelectedItems[0];

                string typeStr = selectedItem.SubItems[1].Text;
                switch (typeStr)
                {
                    case "Sequential":
                        _workflowType = WorkflowType.Sequential;
                        break;

                    case "StateMachine":
                        _workflowType = WorkflowType.StateMachine;
                        break;

                    case "Wizard":
                        _workflowType = WorkflowType.Wizard;
                        break;

                    default:
                        _workflowType = WorkflowType.Sequential;
                        break;
                }
            }
            else
            {
                _workflowType = WorkflowType.Sequential; // default type
            }
        }

        private void AddWorkflowDialog_Load(object sender, EventArgs e)
        {
            this.listView1.Items[0].Selected = true; // select the first item by default
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

            this.SetWorkflowType();
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
                    this.SetWorkflowType();

                    this.DialogResult = DialogResult.OK; // close the dialog
                }
            }		
        }
    }
}
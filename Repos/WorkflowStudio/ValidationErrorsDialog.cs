using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.ComponentModel.Compiler;

namespace WorkflowStudio
{
    public partial class ValidationErrorsDialog : Form
    {
        private ValidationErrorCollection _errors;

        public ValidationErrorsDialog(ValidationErrorCollection errors)
        {
            InitializeComponent();

            _errors = errors;
        }

        private void ValidationErrorsDialog_Load(object sender, EventArgs e)
        {
            foreach (ValidationError error in _errors)
            {
                ListViewItem listViewItem = new ListViewItem(new string[] {
                        error.UserData["Workflow"].ToString(),
                        error.ErrorText}, -1);

                this.errorListView.Items.Add(listViewItem);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
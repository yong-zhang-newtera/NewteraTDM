using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WFModel;
using Newtera.Activities;
using Newtera.WindowsControl;

namespace WorkflowStudio
{
    public partial class ActionCodeEditDialog : Form
    {
        private const int MAX_CODE_LENGTH = 2000;
        private string _actionCode;
        private string _bindingSchemaId = null;
        private string _bindingClassName = null;

        public ActionCodeEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the action code
        /// </summary>
        public string ActionCode
        {
            get
            {
                return _actionCode;
            }
            set
            {
                _actionCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the class name that is bound to the workflow
        /// </summary>
        public string BindingClassName
        {
            get
            {
                return _bindingClassName;
            }
            set
            {
                _bindingClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema id of the binding class
        /// </summary>
        public string BindingSchemaId
        {
            get
            {
                return _bindingSchemaId;
            }
            set
            {
                _bindingSchemaId = value;
            }
        }

        private bool CompileActionCode()
        {
            bool status = true;
            string errorMessage = null;

            if (_bindingClassName != null && _bindingSchemaId != null)
            {
                if (!string.IsNullOrEmpty(this.codeRichTextBox.Text))
                {
                    if (this.codeRichTextBox.Text.Length <= MAX_CODE_LENGTH)
                    {
                        if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidActionCode(this.codeRichTextBox.Text, _bindingSchemaId, _bindingClassName, out errorMessage))
                        {
                            MessageBox.Show(Newtera.WorkflowStudioControl.MessageResourceManager.GetString("WorkflowStudioApp.ActionCodeOK"), "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(errorMessage, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            status = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(Newtera.WorkflowStudioControl.MessageResourceManager.GetString("WorkflowStudioApp.ActionCodeTooLong"), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        status = false;
                    }
                }
            }
            else
            {
                MessageBox.Show(Newtera.WorkflowStudioControl.MessageResourceManager.GetString("WorkflowStudioApp.NoBindingClass"), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                status = false;
            }

            return status;
        }

        private void compileButton_Click(object sender, EventArgs e)
        {
            CompileActionCode();
        }

        private void ActionCodeEditDialog_Load(object sender, EventArgs e)
        {
            if (_actionCode != null)
            {
                this.codeRichTextBox.Text = _actionCode;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (CompileActionCode())
            {
                _actionCode = this.codeRichTextBox.Text;
            }
            else
            {
                // there is error, dismiss the ok event
                this.DialogResult = DialogResult.None;
                return;
            }
        }
    }
}
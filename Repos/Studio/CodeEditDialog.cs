using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
    public partial class CodeEditDialog : Form
    {
        private const int MAX_CODE_LENGTH = 5000;
        private string _code;
        private string _bindingSchemaId = null;
        private string _bindingClassName = null;
        private MetaDataServiceStub _metaDataService = null;

        public CodeEditDialog()
        {
            InitializeComponent();

            _metaDataService = new MetaDataServiceStub();
        }

        /// <summary>
        /// Gets the code
        /// </summary>
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
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

        private bool CompileCode()
        {
            bool status = true;
            string msg = "";
            string errorMessage = null;

            if (!string.IsNullOrEmpty(this.codeRichTextBox.Text))
            {
                if (this.codeRichTextBox.Text.Length <= MAX_CODE_LENGTH)
                {
                    msg = _metaDataService.ValidateMethodCode(ConnectionStringBuilder.Instance.Create(),
                    this.codeRichTextBox.Text, _bindingSchemaId, _bindingClassName);

                    if (!string.IsNullOrEmpty(msg))
                    {
                        // there are errors
                        errorMessage = msg;
                        status = false;
                    }
                    else
                    {
                        // there are no errors
                        errorMessage = "";
                        status = true;
                    }

                    if (status)
                    {
                        MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.MethodCodeOK"), "Info",
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
                    MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.MethodCodeTooLong"), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    status = false;
                }
            }

            return status;
        }

        private void compileButton_Click(object sender, EventArgs e)
        {
            CompileCode();
        }

        private void CodeEditDialog_Load(object sender, EventArgs e)
        {
            if (_code != null)
            {
                this.codeRichTextBox.Text = _code;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (CompileCode())
            {
                _code = this.codeRichTextBox.Text;
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
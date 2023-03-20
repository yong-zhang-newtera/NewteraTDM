using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkflowStudio
{
    public partial class EnterNameDialog : Form
    {
        private string _enterName = null;

        public EnterNameDialog()
        {
            InitializeComponent();
        }

        public string EnterName
        {
            get
            {
                return _enterName;
            }
            set
            {
                _enterName = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.nameTextBox.Text))
            {
                this.DialogResult = DialogResult.None;
                return;
            }
            else
            {
                _enterName = this.nameTextBox.Text;
            }
        }

        private void EnterNameDialog_Load(object sender, EventArgs e)
        {
            if (_enterName != null)
            {
                this.nameTextBox.Text = _enterName;
            }
        }

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.nameTextBox.Text == null ||
                    this.nameTextBox.Text.Length == 0)
                {
                    return;
                }
                else
                {
                    _enterName = this.nameTextBox.Text;
                }

                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
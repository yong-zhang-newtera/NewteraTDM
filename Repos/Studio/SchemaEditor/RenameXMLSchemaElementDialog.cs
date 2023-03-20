using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.XMLSchemaView;

namespace Newtera.Studio
{
    public partial class RenameXMLSchemaElementDialog : Form
    {
        private string _elementName;
        private XMLSchemaModel _xmlSchemaModel;

        public RenameXMLSchemaElementDialog()
        {
            InitializeComponent();
        }

        public string ElementName
        {
            get
            {
                return _elementName;
            }
            set
            {
                _elementName = value;
            }
        }

        public XMLSchemaModel SchemaModel
        {
            get
            {
                return _xmlSchemaModel;
            }
            set
            {
                _xmlSchemaModel = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // validate the text in the textbox
            this.elementNameTextBox.Focus();
            if (!this.Validate())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            _elementName = this.elementNameTextBox.Text;
        }

        private void RenameXMLSchemaElementDialog_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_elementName))
            {
                this.elementNameTextBox.Text = _elementName;
            }
        }

        private void elementNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            // the name cannot be null and has to be unique
            if (this.elementNameTextBox.Text.Length == 0)
            {
                this.errorProvider.SetError(this.elementNameTextBox, "You must enter a name.");
                e.Cancel = true;
            }
            else if (!ValidateRoleNameUniqueness(this.elementNameTextBox.Text))
            {
                this.errorProvider.SetError(this.elementNameTextBox, "The name already exists.");
                e.Cancel = true;
            }
            else
            {
                this.errorProvider.SetError((Control)sender, null);
            }	
        }

        /// <summary>
        /// Gets the information indicating whether the given name is unique among the xml schema
        /// </summary>
        /// <param name="name">element name</param>
        /// <returns>true if it is unique, false otherwise</returns>
        private bool ValidateRoleNameUniqueness(string name)
        {
            bool status = true;

            if (_xmlSchemaModel != null)
            {
                foreach (XMLSchemaComplexType complexType in _xmlSchemaModel.ComplexTypes)
                {
                    if (complexType.Caption == name)
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }
    }
}
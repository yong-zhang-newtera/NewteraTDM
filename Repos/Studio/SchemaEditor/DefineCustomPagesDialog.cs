using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
    public partial class DefineCustomPagesDialog : Form
    {
        private SchemaModelElementCollection _customPages;
        private SchemaModel _schemaModel;
        private string _className;

        public DefineCustomPagesDialog()
        {
            InitializeComponent();
        }

        public SchemaModelElementCollection CustomPages
        {
            get
            {
                return _customPages;
            }
            set
            {
                _customPages = value;
            }
        }

        public SchemaModel SchemaModel
        {
            get
            {
                return _schemaModel;
            }
            set
            {
                _schemaModel = value;
            }
        }

        /// <summary>
        /// Set or gets the name of a class that owns the custom pages
        /// </summary>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        private void BindList()
        {
            if (_customPages != null)
            {
                this.listBox1.Items.Clear();

                foreach (CustomPageElement element in _customPages)
                {
                    this.listBox1.Items.Add(element.Caption);
                }
            }
        }

        private bool IsCustomPagesValid()
        {
            bool status = true;

            if (_customPages != null)
            {
                foreach (CustomPageElement element in _customPages)
                {
                    if (string.IsNullOrEmpty(element.URL))
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }

        private void DefineCustomPagesDialog_Load(object sender, EventArgs e)
        {
            if (_customPages != null)
            {
                BindList();

                if (_customPages.Count > 0)
                {
                    this.listBox1.SelectedIndex = 0;
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string name = "Page" + _customPages.Count;
            CustomPageElement element = new CustomPageElement(name);
            element.SchemaModel = _schemaModel;
            element.MasetrClassName = _className;
            _customPages.Add(element);
            BindList();
            this.listBox1.SelectedIndex = _customPages.Count - 1;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0)
            {
                _customPages.RemoveAt(this.listBox1.SelectedIndex);

                BindList();

                if (_customPages.Count > 0)
                {
                    this.listBox1.SelectedIndex = 0;
                }
                else
                {
                    this.propertyGrid1.SelectedObject = null;
                }
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex > 0)
            {
                int pos = this.listBox1.SelectedIndex;

                CustomPageElement selectedElement = (CustomPageElement)_customPages[pos];
                _customPages.Remove(selectedElement);
                _customPages.Insert(pos - 1, selectedElement);

                BindList();

                this.listBox1.SelectedIndex = pos - 1;
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0 && this.listBox1.SelectedIndex < (_customPages.Count - 1))
            {
                int pos = this.listBox1.SelectedIndex;

                CustomPageElement selectedElement = (CustomPageElement)_customPages[pos];
                _customPages.Remove(selectedElement);
                _customPages.Insert(pos + 1, selectedElement);

                BindList();

                this.listBox1.SelectedIndex = pos + 1;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string msg;

            // make sure that page definitions are valid
            if (_customPages != null)
            {
                if (!IsCustomPagesValid())
                {
                    msg = Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InvalidCustomPages");

                    MessageBox.Show(msg,
                        "Error Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0)
            {
                this.propertyGrid1.SelectedObject = _customPages[this.listBox1.SelectedIndex];
                this.removeButton.Enabled = true;

                if (this.listBox1.SelectedIndex == 0)
                {
                    this.upButton.Enabled = false;
                }
                else
                {
                    this.upButton.Enabled = true;
                }

                if (this.listBox1.SelectedIndex >= _customPages.Count - 1)
                {
                    this.downButton.Enabled = false;
                }
                else
                {
                    this.downButton.Enabled = true;
                } 
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == "Caption")
            {
                int pos = this.listBox1.SelectedIndex;

                BindList();

                this.listBox1.SelectedIndex = pos;
            }
        }
    }
}
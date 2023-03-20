using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.SiteMap;

namespace Newtera.SiteMapStudio
{
    public partial class DefineStateParametersDialog : Form
    {
        private SiteMapNodeCollection _parameters;

        public DefineStateParametersDialog()
        {
            InitializeComponent();
        }

        public SiteMapNodeCollection StateParameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        private void BindList()
        {
            if (_parameters != null)
            {
                this.listBox1.Items.Clear();

                foreach (StateParameter element in _parameters)
                {
                    this.listBox1.Items.Add(element.Name);
                }
            }
        }

        private bool IsStateParametersValid()
        {
            bool status = true;

            if (_parameters != null)
            {
                foreach (StateParameter element in _parameters)
                {
                    if (string.IsNullOrEmpty(element.Name) || string.IsNullOrEmpty(element.Value))
                    {
                        status = false;
                        break;
                    }
                }
            }

            return status;
        }

        private void DefineStateParametersDialog_Load(object sender, EventArgs e)
        {
            if (_parameters != null)
            {
                BindList();

                if (_parameters.Count > 0)
                {
                    this.listBox1.SelectedIndex = 0;
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            StateParameter element = new StateParameter();
            element.Name = "param";
            _parameters.Add(element);
            BindList();
            this.listBox1.SelectedIndex = _parameters.Count - 1;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0)
            {
                _parameters.RemoveAt(this.listBox1.SelectedIndex);

                BindList();

                if (_parameters.Count > 0)
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

                StateParameter selectedElement = (StateParameter)_parameters[pos];
                _parameters.Remove(selectedElement);
                _parameters.Insert(pos - 1, selectedElement);

                BindList();

                this.listBox1.SelectedIndex = pos - 1;
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0 && this.listBox1.SelectedIndex < (_parameters.Count - 1))
            {
                int pos = this.listBox1.SelectedIndex;

                StateParameter selectedElement = (StateParameter)_parameters[pos];
                _parameters.Remove(selectedElement);
                _parameters.Insert(pos + 1, selectedElement);

                BindList();

                this.listBox1.SelectedIndex = pos + 1;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string msg;

            // make sure that page definitions are valid
            if (_parameters != null)
            {
                if (!IsStateParametersValid())
                {
                    msg = MessageResourceManager.GetString("SiteMapModel.InvalidStateParameters");

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
                this.propertyGrid1.SelectedObject = _parameters[this.listBox1.SelectedIndex];
                this.removeButton.Enabled = true;

                if (this.listBox1.SelectedIndex == 0)
                {
                    this.upButton.Enabled = false;
                }
                else
                {
                    this.upButton.Enabled = true;
                }

                if (this.listBox1.SelectedIndex >= _parameters.Count - 1)
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
            if (e.ChangedItem.PropertyDescriptor.Name == "Value" ||
                e.ChangedItem.PropertyDescriptor.Name == "Name")
            {
                int pos = this.listBox1.SelectedIndex;

                BindList();

                this.listBox1.SelectedIndex = pos;
            }
        }
    }
}
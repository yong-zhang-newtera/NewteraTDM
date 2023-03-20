using System;
using System.Collections;
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
    public partial class DefineInputParameterDialog : Form
    {
        private IList _inputParameters;

        public DefineInputParameterDialog()
        {
            InitializeComponent();

            _inputParameters = null;
        }

        /// <summary>
        /// Gets or sets the input parameters
        /// </summary>
        public IList InputParameters
        {
            get
            {
                if (_inputParameters == null)
                {
                    _inputParameters = new ArrayList();
                }

                return _inputParameters;
            }
            set
            {
                _inputParameters = value;
            }
        }

        /// <summary>
        /// Add a new parameter
        /// </summary>
        private void AddParameter()
        {
            if (!string.IsNullOrEmpty(nameEnterTextBox.Text))
            {
                if (!IsParameterExist(nameEnterTextBox.Text))
                {
                    InputParameter parameter = new InputParameter(nameEnterTextBox.Text);
                    InputParameters.Add(parameter);
                    int pos = parametersListBox.Items.Add(parameter.Name);
                    parametersListBox.SelectedIndex = pos; // make the new item selected
                }
                else
                {
                    // show the error
                    string msg = string.Format(MessageResourceManager.GetString("WorkflowStudio.ParameterExist"), nameEnterTextBox.Text);
                    MessageBox.Show(msg,
                        "Error Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudio.NullParameterName"),
                    "Error Dialog", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets information indicating whether a parameter of a given name already exists or not
        /// </summary>
        /// <param name="parameterName">parameter name</param>
        /// <returns>true if exists, false otherwsie</returns>
        private bool IsParameterExist(string parameterName)
        {
            bool status = false;

            if (InputParameters != null)
            {
                foreach (InputParameter param in InputParameters)
                {
                    if (param.Name == parameterName)
                    {
                        status = true;
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Delete a parameter
        /// </summary>
        private void DelParameter()
        {
            if (parametersListBox.SelectedIndex >= 0)
            {
                InputParameters.RemoveAt(this.parametersListBox.SelectedIndex);

                parametersListBox.Items.RemoveAt(this.parametersListBox.SelectedIndex);

                if (this.InputParameters.Count > 0)
                {
                    this.parametersListBox.SelectedIndex = 0; // select the first item
                }
                else
                {
                    this.propertyGrid1.SelectedObject = null;
                }
            }   
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddParameter();
        }

        private void delButton_Click(object sender, EventArgs e)
        {
            DelParameter();
        }

        private void DefineInputParameterDialog_Load(object sender, EventArgs e)
        {
            foreach (InputParameter param in InputParameters)
            {
                parametersListBox.Items.Add(param.Name);
            }

            if (parametersListBox.Items.Count > 0)
            {
                parametersListBox.SelectedIndex = 0; // select first item
            }
        }

        private void parametersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (parametersListBox.SelectedIndex >= 0)
            {
                InputParameter parameter = (InputParameter)InputParameters[parametersListBox.SelectedIndex];
                propertyGrid1.SelectedObject = parameter;
            }
            else
            {
                propertyGrid1.SelectedObject = null;
            }
        }

        private void nameEnterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddParameter();
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }
    }
}
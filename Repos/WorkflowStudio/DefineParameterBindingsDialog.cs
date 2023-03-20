using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.ComponentModel;

using Newtera.WFModel;
using Newtera.WorkflowStudioControl;

namespace WorkflowStudio
{
    public partial class DefineParameterBindingsDialog : Form
    {
        private IList _inputParameters;

        public DefineParameterBindingsDialog()
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

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }
    }
}
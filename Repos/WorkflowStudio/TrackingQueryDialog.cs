using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.Runtime;

namespace WorkflowStudio
{
    public partial class TrackingQueryDialog : Form
    {
        private DateTime _from;
        private DateTime _to;
        private WorkflowStatus _status = WorkflowStatus.Running;
        private string _workflowInstanceId;

        public DateTime From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        public DateTime To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }

        public WorkflowStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public string WrokflowInstanceId
        {
            get
            {
                return _workflowInstanceId;
            }
            set
            {
                _workflowInstanceId = value;
            }
        }

        public TrackingQueryDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this._from =  this.fromDateTimePicker.Value;
            this._to = this.toDateTimePicker.Value;
            if (this.statusComboBox.SelectedIndex >= 0)
            {
                switch (statusComboBox.SelectedIndex)
                {
                    case 0:
                        _status = WorkflowStatus.Running;
                        break;

                    case 1:
                        _status = WorkflowStatus.Completed;
                        break;

                    case 2:
                        _status = WorkflowStatus.Suspended;

                        break;

                    case 3:
                        _status = WorkflowStatus.Terminated;

                        break;
                }
            }

            _workflowInstanceId = this.workflowInstanceIDTextBox.Text;
        }

        private void TrackingQueryDialog_Load(object sender, EventArgs e)
        {
            fromDateTimePicker.Value = _from;

            toDateTimePicker.Value = _to;

            switch (_status)
            {
                case WorkflowStatus.Running:
                    statusComboBox.SelectedIndex = 0;
                    break;

                case WorkflowStatus.Completed:
                    statusComboBox.SelectedIndex = 1;
                    break;

                case WorkflowStatus.Suspended:
                    statusComboBox.SelectedIndex = 2;
                    break;

                case WorkflowStatus.Terminated:
                    statusComboBox.SelectedIndex = 3;
                    break;
            }
        }
    }
}
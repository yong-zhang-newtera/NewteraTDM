using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WFModel;

namespace Newtera.WorkflowMonitor
{
    public partial class CurrentTasksDialog : Form
    {
        private TaskInfoCollection _taskInfos = null;

        public CurrentTasksDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a list of TaskInfo
        /// </summary>
        public TaskInfoCollection TaskInfos
        {
            get
            {
                return _taskInfos;
            }
            set
            {
                _taskInfos = value;
            }
        }

        private void CurrentTasksDialog_Load(object sender, EventArgs e)
        {
            if (_taskInfos != null)
            {
                this.dataGridView.DataSource = _taskInfos;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.DataView;

namespace Newtera.Studio
{
    public partial class InstanceDetailDialog : Form
    {
        private InstanceView _instanceView;

        public InstanceDetailDialog()
        {
            InitializeComponent();
        }

        public InstanceView InstanceView
        {
            get
            {
                return _instanceView;
            }
            set
            {
                _instanceView = value;
            }
        }

        private void InstanceDetailDialog_Load(object sender, EventArgs e)
        {
            if (_instanceView != null)
            {
                this.instanceViewGrid.SelectedObject = _instanceView;
            }
        }

        private void insertInstanceButton_Click(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Validate;

namespace Newtera.WorkflowMonitor
{
    public partial class DataInstanceDialog : Form
    {
        private InstanceView _instanceView;

        public DataInstanceDialog()
        {
            InitializeComponent();

            _instanceView = null;
        }

        /// <summary>
		/// Gets or sets the instance view of the new instance
		/// </summary>
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

        private void DataInstanceDialog_Load(object sender, EventArgs e)
        {
            if (_instanceView != null)
            {
                this.classTextBox.Text = _instanceView.DataView.BaseClass.Caption;

                this.dataInstancePropertyGrid.SelectedObject = _instanceView;
            }
        }
    }
}
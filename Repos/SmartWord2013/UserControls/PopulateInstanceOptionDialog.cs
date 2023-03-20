using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartWord2013
{
    public partial class PopulateInstanceOptionDialog : Form
    {
        private bool _toSelectedNode = false;

        /// <summary>
        /// Gets the information indicating whether to populate the data to the selected node
        /// </summary>
        /// <value>true if populating to the selected node only, false populating to the entire document.</value>
        public bool IsPopulateToSelectedNode
        {
            get
            {
                return _toSelectedNode;
            }
        }

        public PopulateInstanceOptionDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (this.selectedNodeRadioButton.Checked)
            {
                this._toSelectedNode = true;
            }
            else
            {
                this._toSelectedNode = false;
            }
        }
    }
}
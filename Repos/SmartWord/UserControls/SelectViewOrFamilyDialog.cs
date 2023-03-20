using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartWord
{
    public partial class SelectViewOrFamilyDialog : Form
    {
        private string _nodeName = ThisDocument.ViewNodeName;

        public SelectViewOrFamilyDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the node name to be inserted
        /// </summary>
        public string InsertNodeName
        {
            get
            {
                return _nodeName;
            }
        }

        private void viewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (viewRadioButton.Checked)
            {
                _nodeName = ThisDocument.ViewNodeName;
            }
            else
            {
                _nodeName = ThisDocument.FamilyNodeName;
            }
        }
    }
}
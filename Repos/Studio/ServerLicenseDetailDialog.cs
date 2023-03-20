using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Newtera.Studio
{
    public partial class ServerLicenseDetailDialog : Form
    {
        private string _text = null;

        public ServerLicenseDetailDialog()
        {
            InitializeComponent();
        }

        public string LicenseDetails
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        private void ServerLicenseDetailDialog_Load(object sender, EventArgs e)
        {
            licenseTextBox.Text = _text;
        }
    }
}
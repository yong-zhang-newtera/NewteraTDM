using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using System.Text.RegularExpressions;

using Newtera.MLActivities.Core;

namespace Newtera.MLActivities.Controls
{
    /// <summary>
    /// ExperimentIdDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ExperimentIdDialog : Window
    {
        private string experimentId;

        public ExperimentIdDialog()
        {
            InitializeComponent();

        }

        public string ExperimentId
        {
            get
            {
                return experimentId;
            }
            set
            {
                this.experimentId = value;
            }
        }

        private void btnDialogOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtExperimentId.Text))
            {
                MessageBox.Show("Please enter an experiment id.");
            }
            else if (!ValidateIDString(this.txtExperimentId.Text))
            {
                MessageBox.Show("ExperimentId starts with an english letter and consists of only english letters, digits, hyphen, or undercore.");

                this.DialogResult = false;
            }
            else if (IsExperimentExists(this.txtExperimentId.Text))
            {
                MessageBox.Show(this.txtExperimentId.Text + " has been taken, please pick a different one.");

                this.DialogResult = false;
            }
            else
            {
                this.experimentId = this.txtExperimentId.Text.Trim();

                this.DialogResult = true;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //show initial value
            this.txtExperimentId.Text = this.experimentId;
        }

        private bool ValidateIDString(string id)
        {
            Regex regex = new Regex(@"^[a-zA-Z]+[0-9]*[a-zA-Z_\-]*[0-9]*$");

            bool status = regex.IsMatch(id);

            return status;
        }

        private bool IsExperimentExists(string id)
        {
            string experimentDir = MLNameSpace.GetHomeDir() + @"\" + MLNameSpace.EXPERIMENT_DIR + @"\" + id;
            if (Directory.Exists(experimentDir))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

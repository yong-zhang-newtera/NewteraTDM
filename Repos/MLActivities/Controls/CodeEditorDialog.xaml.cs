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
using ICSharpCode.AvalonEdit;

using Newtera.MLActivities.Core;

namespace Newtera.MLActivities.Controls
{
    /// <summary>
    /// CodeEditorDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CodeEditorDialog : Window
    {
        public CodeEditorDialog()
        {
            InitializeComponent();

        }

        public string Code
        {
            get
            {
                return (txtModelCode.Text == null ? "" : txtModelCode.Text); ;
            }
            set
            {
                txtModelCode.Text = value;
            }
        }

        private void btnDialogOK_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCode(this.txtModelCode.Text))
            {
                MessageBox.Show("The code is incorrect");

                this.DialogResult = false;
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string templateDir = GettemplateDir();

            this.lbxTemplate.Items.Add("None");

            DirectoryInfo dir = new DirectoryInfo(templateDir);
            FileInfo[] Files = dir.GetFiles("*.txt"); //Getting templates files
            foreach (FileInfo file in Files)
            {
                int pos = file.Name.IndexOf('.');
                if (pos > 0)
                {
                    this.lbxTemplate.Items.Add(file.Name.Substring(0, pos));
                }
            }
        }

        /// <summary>
        /// Validate the code
        /// </summary>
        /// <param name="code">The model code</param>
        /// <returns>true if the code is valid, false otherwise.</returns>
        private bool ValidateCode(string code)
        {
            bool status = true;

            return status;
        }

        private void lbxTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lbxTemplate.SelectedIndex > 0)
            {
                string templatePath = GettemplateDir() + @"\" + this.lbxTemplate.SelectedItem.ToString() + ".txt";
                string templateCode = File.ReadAllText(templatePath);
                if (!string.IsNullOrEmpty(txtModelCode.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure to override the content?", "Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        txtModelCode.Text = templateCode;
                    }
                }
                else
                {
                    txtModelCode.Text = templateCode;
                }
            }
        }

        private string GettemplateDir()
        {
            string templateDir = MLNameSpace.GetHomeDir() + @"\" + MLNameSpace.NETWORK_TEMPLATE_DIR;

            return templateDir;
        }
    }
}

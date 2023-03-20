using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using Newtera.MLActivities.Core;
using Newtera.MLStudio.WebApi;
using Newtera.MLStudio.Utilities;

namespace Newtera.MLStudio.Views
{
    /// <summary>
    /// SetupServerUrlDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SetupServerUrlDialog : Window
    {
        public SetupServerUrlDialog()
        {
            InitializeComponent();

        }

        public string ServerUri
        {
            get
            {
                return (txtServerUri.Text == null ? "" : txtServerUri.Text.Trim()); ;
            }
        }

        private void btnDialogOK_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSeverUrl(this.txtServerUri.Text))
            {
                MessageBox.Show("The server uri is incorrect");

                this.DialogResult = false;
            }
            else
            {
                UpdateConfig();

                MessageBox.Show("The setup of server uri is completed. ");

                this.DialogResult = true;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //Create the object
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //show initial value
            this.txtServerUri.Text = config.AppSettings.Settings[MLNameSpace.BASE_URL].Value;
        }

        /// <summary>
        /// Gets the information indicating whether the server specified by the url
        /// is valid or not
        /// </summary>
        /// <param name="serverUrl">The server url</param>
        /// <returns>true if the server is valid, false otherwise.</returns>
        private bool ValidateSeverUrl(string serverUri)
        {
            bool status = true;

            if (!string.IsNullOrEmpty(serverUri))
            {
                try
                {
                    MLModelServiceStub mlModelService = new MLModelServiceStub();
                    mlModelService.BaseUri = serverUri;

                    string serverVersion = mlModelService.GetServerVersion();
                }
                catch (Exception)
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Update the server base url defined in the DesignStudio config file
        /// </summary>
        private void UpdateConfig()
        {
            //Create the object
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //make changes
            config.AppSettings.Settings[MLNameSpace.BASE_URL].Value = this.txtServerUri.Text.Trim();

            //save to apply changes
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}

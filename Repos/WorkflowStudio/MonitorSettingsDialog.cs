using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WorkflowMonitor;

namespace WorkflowStudio
{
    public partial class MonitorSettingsDialog : Form
    {
        private ApplicationSettings monitorSettings = null;

        public MonitorSettingsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the application settings
        /// </summary>
        public ApplicationSettings ApplicationSettings
        {
            get
            {
                return monitorSettings;
            }
            set
            {
                monitorSettings = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int previousPollingInterval = monitorSettings.PollingInterval;

            //Try to convert pollling interval to an int32 - if an error, revert to previous setting
            try
            {
                monitorSettings.AutoSelectLatest = AutoSelectLatestCheckBox.Checked;
                int pollingInterval = System.Convert.ToInt32(this.PollingIntervalTextBox.Text);
                if (pollingInterval > 0)
                {
                    monitorSettings.PollingInterval = pollingInterval;
                }
            }
            catch
            {
                monitorSettings.PollingInterval = previousPollingInterval;
            }
        }

        private void MonitorSettingsDialog_Load(object sender, EventArgs e)
        {
            if (monitorSettings != null)
            {
                this.PollingIntervalTextBox.Text = monitorSettings.PollingInterval.ToString();
                AutoSelectLatestCheckBox.Checked = monitorSettings.AutoSelectLatest;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.Config;
using Newtera.Common.Core;
using Newtera.WinClientCommon;

namespace Newtera.SiteMapStudio
{
    public partial class FulltextSettingsDialog : Form
    {
        private const string INDEX_METHOD_KEY = "method";
        private const string INDEX_METHOD_AUTO = "auto";
        private const string INDEX_METHOD_MANUAL = "manual";
        private const string INDEX_INTERVAL_KEY = "interval";
        private const string INDEX_ENABLED = "enabled";
        private const string INDEX_DEPTH_KEY = "depth";
        private const string INDEX_START_TIME_KEY = "startTime";

        private FullTextIndexConfig _config;
        private Newtera.Common.Core.SchemaInfo[] _schemaInfos;

        public FulltextSettingsDialog()
        {
            _config = null;
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // save the full-text settings
            MetaDataServiceStub metaDataService = new MetaDataServiceStub();

            // set the index method
            _config.SetAppSetting(FulltextSettingsDialog.INDEX_METHOD_KEY, this.methodComboBox.SelectedItem.ToString());

            // set index interval
            _config.SetAppSetting(FulltextSettingsDialog.INDEX_INTERVAL_KEY, this.intervalTextBox.Text);

            // set crawling depth
            _config.SetAppSetting(FulltextSettingsDialog.INDEX_DEPTH_KEY, this.depthComboBox.SelectedItem.ToString());

            // set crawling start time
            _config.SetAppSetting(FulltextSettingsDialog.INDEX_START_TIME_KEY, this.startTimeComboBox.SelectedItem.ToString());

            for (int i = 0; i < this.schemaListView.Items.Count; i++)
            {
                if (this.schemaListView.Items[i].Checked)
                {
                    _config.SetAppSetting(_schemaInfos[i].NameAndVersion, FulltextSettingsDialog.INDEX_ENABLED);
                }
                else
                {
                    _config.RemoveAppSetting(_schemaInfos[i].NameAndVersion);
                }
            }

            string xmlString = _config.ToXML();

            metaDataService.SetFullTextSettings(xmlString);

            MessageBox.Show(MessageResourceManager.GetString("SiteMapStudioApp.RestartProcess"), "Information Dialog", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void FulltextSettingsDialog_Load(object sender, EventArgs e)
        {
            MetaDataServiceStub metaDataService = new MetaDataServiceStub();
            string xml = metaDataService.GetFullTextSettings();
            _config = new FullTextIndexConfig(xml);
            Newtera.Common.Core.SchemaInfo schemaInfo;

            // set the index method
            string method = _config.GetAppSetting(FulltextSettingsDialog.INDEX_METHOD_KEY);
            if (!string.IsNullOrEmpty(method) && method == FulltextSettingsDialog.INDEX_METHOD_AUTO)
            {
                this.methodComboBox.SelectedItem = FulltextSettingsDialog.INDEX_METHOD_AUTO;
            }
            else
            {
                this.methodComboBox.SelectedItem = FulltextSettingsDialog.INDEX_METHOD_MANUAL;
            }

            // set index interval
            string interval = _config.GetAppSetting(FulltextSettingsDialog.INDEX_INTERVAL_KEY);
            if (!string.IsNullOrEmpty(interval))
            {
                try
                {
                    this.intervalTextBox.Text = interval.Trim();
                }
                catch (Exception)
                {
                    this.intervalTextBox.Text = "7"; // default to 7 day
                }
            }
            else
            {
                this.intervalTextBox.Text = "7"; // default to 7 day
            }

            // set starttime
            string startTime = _config.GetAppSetting(FulltextSettingsDialog.INDEX_START_TIME_KEY);
            if (!string.IsNullOrEmpty(startTime))
            {
                this.startTimeComboBox.SelectedItem = startTime;
            }
            else
            {
                this.startTimeComboBox.SelectedItem = "0"; // default to 0
            }

            // set crawling depth
            string depth = _config.GetAppSetting(FulltextSettingsDialog.INDEX_DEPTH_KEY);
            if (!string.IsNullOrEmpty(depth))
            {
                this.depthComboBox.SelectedItem = depth;
            }
            else
            {
                this.depthComboBox.SelectedItem = "1"; // default to 1
            }

            this.schemaListView.SuspendLayout();
            Newtera.Common.Core.SchemaInfo[] schemaInfos = metaDataService.GetSchemaInfos();
            _schemaInfos = new Newtera.Common.Core.SchemaInfo[schemaInfos.Length];
            for (int i = 0; i < schemaInfos.Length; i++)
            {
                schemaInfo = new Newtera.Common.Core.SchemaInfo();
                schemaInfo.Name = schemaInfos[i].Name;
                schemaInfo.Version = schemaInfos[i].Version;
                _schemaInfos[i] = schemaInfo;
                ListViewItem item = new ListViewItem(schemaInfo.Name);
                item.SubItems.Add(schemaInfo.Version);
                if (_config.GetAppSetting(schemaInfo.NameAndVersion) != null)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }

                this.schemaListView.Items.Add(item);
            }

            this.schemaListView.ResumeLayout();

        }
    }
}
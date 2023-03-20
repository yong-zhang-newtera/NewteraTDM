using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.Mappings;

namespace Newtera.Studio
{
    public partial class SelectMappingPackageDialog : Form
    {
        private MappingPackageCollection _mappingPackages;
        private MappingPackage _selectedMappingPackage;

        public SelectMappingPackageDialog(MappingPackageCollection mappingPackages)
        {
            _mappingPackages = mappingPackages;
            _selectedMappingPackage = null;

            InitializeComponent();
        }

        public MappingPackage SelectedMappingPackage
        {
            get
            {
                return _selectedMappingPackage;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }

        private void SelectMappingPackageDialog_Load(object sender, EventArgs e)
        {
            if (_mappingPackages != null)
            {
                ListViewItem item;
                this.mappingPackageListView.SuspendLayout();
                this.mappingPackageListView.Items.Clear();

                foreach (MappingPackage mp in _mappingPackages)
                {
                    item = new ListViewItem(mp.Name);
                    item.ImageIndex = 0;
                    item.SubItems.Add(mp.DataSourceType.ToString());

                    this.mappingPackageListView.Items.Add(item);
                }

                this.mappingPackageListView.ResumeLayout();
            }
        }

        private void mappingPackageListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.mappingPackageListView.SelectedIndices.Count == 1)
            {
                int index = this.mappingPackageListView.SelectedIndices[0];

                _selectedMappingPackage = (MappingPackage)_mappingPackages[index];
            }
        }
    }
}
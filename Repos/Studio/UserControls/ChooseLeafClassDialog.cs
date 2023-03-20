using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;

namespace Newtera.Studio.UserControls
{
    public partial class ChooseLeafClassDialog : Form
    {
        private ClassElement _parentClass;
        private string _selectedLeafClassName = null;
        private string _selectedLeafClassCaption = null;

        public ChooseLeafClassDialog()
        {
            InitializeComponent();
        }

        public ClassElement ParentClass
        {
            get
            {
                return _parentClass;
            }
            set
            {
                _parentClass = value;
            }
        }

        public string SelectedLeafClassName
        {
            get
            {
                return _selectedLeafClassName;
            }
            set
            {
                _selectedLeafClassName = value;
            }
        }

        public string SelectedLeafClassCaption
        {
            get
            {
                return _selectedLeafClassCaption;
            }
            set
            {
                _selectedLeafClassCaption = value;
            }
        }

        private void ChooseLeafClassDialog_Load(object sender, EventArgs e)
        {
            SchemaModelElementCollection leafClasses = _parentClass.GetLeafClasses();
            foreach (ClassElement leafClass in leafClasses)
            {
                ListViewItem listViewItem = new ListViewItem(leafClass.Caption);
                listViewItem.SubItems.Add(leafClass.Name);

                this.listView1.Items.Add(listViewItem);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                ListViewItem item = (ListViewItem)this.listView1.SelectedItems[0];

                _selectedLeafClassCaption = item.Text;
                _selectedLeafClassName = item.SubItems[1].Text;
            }
        }
    }
}
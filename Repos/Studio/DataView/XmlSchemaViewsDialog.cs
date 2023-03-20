using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.XMLSchemaView;

namespace Newtera.Studio
{
    public partial class XmlSchemaViewsDialog : Form
    {
        private string _xmlSchemaViewName;
        private string _baseClassName;
        private MetaDataModel _metaData;

        public XmlSchemaViewsDialog()
        {
            InitializeComponent();
        }

        public MetaDataModel MetaData
        {
            get
            {
                return _metaData;
            }
            set
            {
                _metaData = value;
            }
        }

        public string BaseClassName
        {
            get
            {
                return _baseClassName;
            }
            set
            {
                _baseClassName = value;
            }
        }

        public string SelectedXMLSchemaName
        {
            get
            {
                return _xmlSchemaViewName;
            }
            set
            {
                _xmlSchemaViewName = value;
            }
        }

        private void XmlSchemaViewsDialog_Load(object sender, EventArgs e)
        {
            // it is the UI thread, continue
            this.xmlSchemaListView.SuspendLayout();

            ListViewItem item;

            foreach (XMLSchemaModel xmlSchemaModel in _metaData.XMLSchemaViews)
            {
                if (xmlSchemaModel.RootElement.ElementType == _baseClassName)
                {
                    item = new ListViewItem(xmlSchemaModel.Caption);
                    item.SubItems.Add(xmlSchemaModel.Name);
                    this.xmlSchemaListView.Items.Add(item);
                }
            }

            this.xmlSchemaListView.ResumeLayout();
        }

        private void xmlSchemaListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.okButton.Enabled = true;

            if (this.xmlSchemaListView.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)this.xmlSchemaListView.SelectedItems[0];

                _xmlSchemaViewName = item.SubItems[1].Text;
            }
            else
            {
                this._xmlSchemaViewName = null;
            }
        }
    }
}
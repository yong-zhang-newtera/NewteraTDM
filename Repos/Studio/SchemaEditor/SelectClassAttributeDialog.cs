using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
    public partial class SelectClassAttributeDialog : Form
    {
        private string _selectedAttributeName = null;
        private string _selectedAttributeCaption = null;
        private DataType _selectedAttributeType = DataType.Unknown;
        private MetaDataModel _metaData;
        private ClassElement _classElement;

        public SelectClassAttributeDialog(MetaDataModel metaData, ClassElement classElement)
        {
            _metaData = metaData;
            _classElement = classElement;

            InitializeComponent();
        }

        /// <summary>
        /// Gets the name of the selected attribute
        /// </summary>
        public string SelectedAttributeName
        {
            get
            {
                return _selectedAttributeName;
            }
        }

        /// <summary>
        /// Gets the caption of the selected attribute
        /// </summary>
        public string SelectedAttributeCaption
        {
            get
            {
                return _selectedAttributeCaption;
            }
        }

        /// <summary>
        /// Gets the data type of the selected attribute
        /// </summary>
        public DataType SelectedAttributeDataType
        {
            get
            {
                return _selectedAttributeType;
            }
        }

        private void SelectClassAttributeDialog_Load(object sender, EventArgs e)
        {
            DataViewModel dataView = _metaData.GetDetailedDataView(_classElement.Name);
            if (dataView != null)
            {
                ResultAttributeCollection attributes = dataView.ResultAttributes;

                ResultAttributeListViewItem item;
                
                this.attributeListView.SuspendLayout();
                this.attributeListView.Items.Clear();

                foreach (IDataViewElement result in dataView.ResultAttributes)
                {
                    if (result.ElementType != ElementType.RelationshipAttribute)
                    {
                        item = new ResultAttributeListViewItem(result.Caption, result);
                        item.ImageIndex = 0;
                        item.SubItems.Add(result.Name);

                        this.attributeListView.Items.Add(item);
                    }
                }

                this.attributeListView.ResumeLayout();
            }
        }

        private void attributeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.attributeListView.SelectedIndices.Count == 1)
            {
                ResultAttributeListViewItem item = (ResultAttributeListViewItem)this.attributeListView.SelectedItems[0];
                AttributeElementBase attributeElement = (AttributeElementBase)item.ResultAttribute.GetSchemaModelElement();
                _selectedAttributeName = attributeElement.Name;
                _selectedAttributeCaption = attributeElement.Caption;
                _selectedAttributeType = attributeElement.DataType;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
        }
    }
}
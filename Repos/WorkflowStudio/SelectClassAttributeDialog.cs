using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.ComponentModel;

using Newtera.Activities;
using Newtera.WFModel;
using Newtera.WorkflowStudioControl;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace WorkflowStudio
{
    public partial class SelectClassAttributeDialog : Form
    {
        private string _selectedAttributeName = null;
        private string _selectedAttributeCaption = null;
        private ParameterDataType _parameterType = ParameterDataType.Unknown;
        private InstanceView _instanceView = null;

        public SelectClassAttributeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the name of the selected attribute
        /// </summary>
        public string SelectedAttributeName
        {
            get
            {
                return _selectedAttributeName;
            }
            set
            {
                _selectedAttributeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the caption of the selected attribute
        /// </summary>
        public string SelectedAttributeCaption
        {
            get
            {
                return _selectedAttributeCaption;
            }
            set
            {
                _selectedAttributeCaption = value;
            }
        }

        /// <summary>
        /// Gets or sets the data type of the parameter
        /// </summary>
        public ParameterDataType DataType
        {
            get
            {
                return _parameterType;
            }
            set
            {
                _parameterType = value;
            }
        }

        private void SelectClassAttributeDialog_Load(object sender, EventArgs e)
        {
            Activity currentActivity = ActivityCache.Instance.CurrentActivity;
            INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(currentActivity);
            if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(rootActivity.SchemaId))
            {
                if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(rootActivity.SchemaId, rootActivity.ClassName))
                {
                    string bindingClassName = rootActivity.ClassName;
                    MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(rootActivity.SchemaId);

                    DataViewModel dataView = metaData.GetDetailedDataView(bindingClassName);
                    if (dataView != null)
                    {
                        _instanceView = new InstanceView(dataView);

                        ResultAttributeCollection attributes = dataView.ResultAttributes;

                        ResultAttributeListViewItem item;
                        ResultAttributeListViewItem selectedItem = null;
                        this.attributeListView.SuspendLayout();
                        this.attributeListView.Items.Clear();

                        foreach (IDataViewElement result in dataView.ResultAttributes)
                        {
                            item = new ResultAttributeListViewItem(result.Caption, result);

                            if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.SimpleAttribute)
                            {
                                item.ImageIndex = 0;
                            }
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.ArrayAttribute)
                            {
                                item.ImageIndex = 2;
                            }
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.RelationshipAttribute)
                            {
                                RelationshipAttributeElement relationship =  result.GetSchemaModelElement() as RelationshipAttributeElement;
                                if (relationship != null && relationship.IsForeignKeyRequired)
                                {
                                    item.ImageIndex = 1;
                                }
                                else
                                {
                                    continue; // only show the relationship attribute with foreign key
                                }
                            }
                            else
                            {
                                continue;
                            }
                            item.SubItems.Add(result.Name);

                            if (_selectedAttributeName != null &&
                                _selectedAttributeName == result.Name)
                            {
                                selectedItem = item;
                            }

                            this.attributeListView.Items.Add(item);
                        }

                        // select the item and make sure the selected item is visible
                        if (selectedItem != null)
                        {
                            selectedItem.Selected = true;
                            selectedItem.EnsureVisible();
                        }

                        this.attributeListView.ResumeLayout();
                    }
                }
            }
        }

        private void attributeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.attributeListView.SelectedIndices.Count == 1)
            {
                ResultAttributeListViewItem item = (ResultAttributeListViewItem)this.attributeListView.SelectedItems[0];
                _selectedAttributeName = item.ResultAttribute.Name;
                _selectedAttributeCaption = item.ResultAttribute.Caption;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (_selectedAttributeName != null)
            {
                InstanceAttributePropertyDescriptor pd = (InstanceAttributePropertyDescriptor)_instanceView.GetProperties(null)[_selectedAttributeName];
                Type targetType = ActivityUtil.GetClassAttributeType(pd);
                if (!ActivityUtil.CanConvertTo(_parameterType, targetType))
                {
                    string msg = string.Format(Newtera.WorkflowStudioControl.MessageResourceManager.GetString("WorkflowStudioApp.DataTypeMismatch"), targetType.Name);
                    MessageBox.Show(msg,
                        "Error Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
        }
    }
}
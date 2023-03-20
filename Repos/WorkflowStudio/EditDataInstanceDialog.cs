using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.ComponentModel;

using Newtera.Common.MetaData.DataView;
using Newtera.WorkflowStudioControl;
using Newtera.Activities;
using Newtera.WFModel;

namespace WorkflowStudio
{
    public partial class EditDataInstanceDialog : Form
    {
        private InstanceView _instanceView;
        private string[] _boundAttributeNames;
        private string _existingInsertStatement;

        public EditDataInstanceDialog()
        {
            InitializeComponent();

            _boundAttributeNames = null;
            _existingInsertStatement = null;
        }

        public InstanceView InstanceView
        {
            get
            {
                return _instanceView;
            }
            set
            {
                _instanceView = value;
            }
        }

        public string ExistingInsertStatement
        {
            get
            {
                return _existingInsertStatement;
            }
            set
            {
                _existingInsertStatement = value;
            }
        }

        private int GetParameterIndex(ListViewItem theItem)
        {
            int index = 0;

            foreach (ListViewItem item in this.parameterListViewEx.Items)
            {
                if (item == theItem)
                {
                    break;
                }
                else
                {
                    index++;
                }
            }

            return index;
        }

        private void EditDataInstanceDialog_Load(object sender, EventArgs e)
        {
            if (_instanceView != null)
            {
                this.classTextBox.Text = _instanceView.DataView.BaseClass.Caption;

                if (!string.IsNullOrEmpty(_existingInsertStatement))
                {
                    QueryStatementUtil.SetDataInstanceAttributeValues(_instanceView, _existingInsertStatement);
                }

                this.propertyGrid.SelectedObject = _instanceView;

                // show the parameters
                Activity currentActivity = ActivityCache.Instance.CurrentActivity;
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(currentActivity);
                if (rootActivity.InputParameters != null &&
                    rootActivity.InputParameters.Count > 0)
                {
                    _boundAttributeNames = new string[rootActivity.InputParameters.Count];

                    ListViewItem listViewItem;
                    int index = 0;
                    foreach (InputParameter parameter in rootActivity.InputParameters)
                    {
                        listViewItem = new ListViewItem(parameter.Name);
                        listViewItem.SubItems.Add("..."); // Attribute Name

                        parameterListViewEx.Items.Add(listViewItem);
                        _boundAttributeNames[index] = null;

                        index++;
                    }

                    if (!string.IsNullOrEmpty(_existingInsertStatement))
                    {
                        // show the exiting parameter bindings
                        QueryStatementUtil.SetInputParameterBindingValues(parameterListViewEx, _boundAttributeNames, _instanceView, _existingInsertStatement);
                    }
                }
            }
        }

        private void parameterListViewEx_SubItemClicked(object sender, Newtera.WindowsControl.SubItemEventArgs e)
        {
            if (e.SubItem == 1)
            {
                SelectClassAttributeDialog dialog = new SelectClassAttributeDialog();

                Activity currentActivity = ActivityCache.Instance.CurrentActivity;
                INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(currentActivity);
                int index = GetParameterIndex(e.Item);

                dialog.DataType = ((InputParameter)rootActivity.InputParameters[index]).DataType;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // update the listview display text
                    if (dialog.SelectedAttributeCaption != null &&
                        dialog.SelectedAttributeCaption != e.Item.SubItems[1].Text)
                    {
                        e.Item.SubItems[1].Text = dialog.SelectedAttributeCaption;
                        _boundAttributeNames[index] = dialog.SelectedAttributeName;
                    }
                }
            }
        }

        private void parameterListViewEx_SubItemEndEditing(object sender, Newtera.WindowsControl.SubItemEndEditingEventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Set the paramter name as a value to the bound attribute
            if (_boundAttributeNames != null)
            {
                for (int i = 0; i < _boundAttributeNames.Length; i++)
                {
                    string attribteName = _boundAttributeNames[i];
                    if (attribteName != null)
                    {
                        // set the parameter to the instance without verifying the value, otherwise setting to an enum attribute will fail
                        _instanceView.InstanceData.SetAttributeStringValue(attribteName, "{" + this.parameterListViewEx.Items[i].Text + "}", false);
                    }
                }
            }
        }
    }
}
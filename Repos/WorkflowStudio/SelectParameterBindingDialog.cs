using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Workflow.ComponentModel;

using Newtera.WinClientCommon;
using Newtera.WindowsControl;
using Newtera.WFModel;
using Newtera.Activities;
using Newtera.WorkflowStudioControl;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;

namespace WorkflowStudio
{
    public partial class SelectParameterBindingDialog : Form
    {
        private ParameterBindingInfo _bindingInfo;
        private INewteraWorkflow _rootActivity;
        private ParameterDataType _dataType;
        private InstanceView _instanceView;

        public SelectParameterBindingDialog()
        {
            InitializeComponent();
            _rootActivity = null;
            _dataType = ParameterDataType.Unknown;
        }

        /// <summary>
        /// Gets or sets the parameter binding
        /// </summary>
        public ParameterBindingInfo ParameterBinding
        {
            get
            {
                return _bindingInfo;
            }
            set
            {
                _bindingInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the expected data type
        /// </summary>
        public ParameterDataType DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
            }
        }

        /// <summary>
        /// Display the attributes of the class that is bound to the workflow model if exists.
        /// </summary>
        /// <returns>true if a binding in class attributes is matched, false otherwsie</returns>
        private bool ShowBindingClassAttributes()
        {
            bool matched = false;

            if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidSchemaId(_rootActivity.SchemaId))
            {
                if (ActivityValidatingServiceProvider.Instance.ValidateService.IsValidClassName(_rootActivity.SchemaId, _rootActivity.ClassName))
                {
                    string bindingClassName = _rootActivity.ClassName;
                    MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(_rootActivity.SchemaId);

                    DataViewModel dataView = metaData.GetDetailedDataView(bindingClassName);
                    if (dataView != null)
                    {
                        _instanceView = new InstanceView(dataView);

                        ResultAttributeCollection attributes = dataView.ResultAttributes;

                        ResultAttributeListViewItem item;
                        ResultAttributeListViewItem selectedItem = null;
                        this.classListView.SuspendLayout();
                        this.classListView.Items.Clear();

                        foreach (IDataViewElement result in attributes)
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
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.VirtualAttribute)
                            {
                                item.ImageIndex = 3;
                            }
                            /*
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.RelationshipAttribute)
                            {
                                item.ImageIndex = 1;
                            }
                             */
                            else
                            {
                                continue;
                            }

                            item.SubItems.Add(result.Name);

                            if (_bindingInfo != null &&
                                _bindingInfo.SourceType == SourceType.DataInstance &&
                                !string.IsNullOrEmpty(_bindingInfo.Path) &&
                                _bindingInfo.Path == result.Name)
                            {
                                selectedItem = item;
                            }

                            this.classListView.Items.Add(item);
                        }

                        // select the item and make sure the selected item is visible
                        if (selectedItem != null)
                        {
                            selectedItem.Selected = true;
                            selectedItem.EnsureVisible();
                            matched = true;
                        }

                        this.classListView.ResumeLayout();
                    }
                }
            }

            return matched;
        }

        /// <summary>
        /// Display the input parameters of the workflow that is the root activity of the
        /// invoking activity.
        /// </summary>
        /// <returns>true if a binding in parameters is matched, false otherwsie</returns>
        private bool ShowInputParameters()
        {
            bool matched = false;

            if (_rootActivity.InputParameters != null &&
                _rootActivity.InputParameters.Count > 0)
            {
                int index = 0;
                int selectedIndex = -1;
                foreach (InputParameter parameter in _rootActivity.InputParameters)
                {
                    this.parameterListBox.Items.Add(parameter.Name);
                    if (_bindingInfo != null &&
                        _bindingInfo.SourceType == SourceType.Parameter &&
                        _bindingInfo.Path == parameter.Name)
                    {
                        selectedIndex = index;
                    }

                    index++;
                }

                if (selectedIndex >= 0)
                {
                    this.parameterListBox.SelectedIndex = selectedIndex;
                    matched = true;
                }
            }

            return matched;
        }

        /// <summary>
        /// Display all the activities of the workflow that is the root activity of the
        /// invoking activity.
        /// </summary>
        /// <returns>true if a binding in activity properties is matched, false otherwsie</returns>
        private bool ShowActivities()
        {
            bool matched = false;
            Activity currentActivity = ActivityCache.Instance.CurrentActivity;

            int index = 0;
            int selectedIndex = -1;
            while (currentActivity != null)
            {
                this.activityComboBox.Items.Add(currentActivity.Name);
                if (_bindingInfo != null &&
                    _bindingInfo.SourceType == SourceType.Activity &&
                    !string.IsNullOrEmpty(_bindingInfo.Path) &&
                    IsSameActivity(_bindingInfo.Path, currentActivity.Name))
                {
                    selectedIndex = index;
                }

                currentActivity = currentActivity.Parent;
                index++;
            }

            if (selectedIndex >= 0)
            {
                this.activityComboBox.SelectedIndex = selectedIndex;
                matched = true;
            }
            else
            {
                this.activityComboBox.SelectedIndex = 0; // default
            }

            return matched;
        }

        /// <summary>
        /// Display the public properties of the activity which is shown in activityComboBox.
        /// </summary>
        /// <returns>true if a binding in activity properties is matched, false otherwsie</returns>
        private bool ShowActivityProperties()
        {
            bool matched = false;

            this.propertyListBox.Items.Clear();

            if (activityComboBox.SelectedIndex >= 0)
            {
                string selectedActivityName = (string) activityComboBox.SelectedItem;
                Activity selectedActivity = ((Activity)_rootActivity).GetActivityByName(selectedActivityName);
                if (selectedActivity != null)
                {

                    int index = 0;
                    int selectedIndex = -1;
                    PropertyInfo[] properties = selectedActivity.GetType().GetProperties();
                    foreach (PropertyInfo propertyInfo in properties)
                    {
                        if (propertyInfo.CanRead)
                        {
                            propertyListBox.Items.Add(propertyInfo.Name);

                            if (_bindingInfo != null &&
                                _bindingInfo.SourceType == SourceType.Activity &&
                                !string.IsNullOrEmpty(_bindingInfo.Path) &&
                                IsSameProperty(_bindingInfo.Path, propertyInfo.Name))
                            {
                                selectedIndex = index;
                            }

                            index++;
                        }
                    }

                    if (selectedIndex >= 0)
                    {
                        this.propertyListBox.SelectedIndex = selectedIndex;
                        matched = true;
                    }
                }
            }

            return matched;
        }

        /// <summary>
        /// Gets the information indicating whether the path of a parameter binding refers
        /// to an activity of given name
        /// </summary>
        /// <param name="path">Path of parameter binding</param>
        /// <param name="activityName">An activity name</param>
        /// <returns>true if the path refers to the activity, false otherwise</returns>
        private bool IsSameActivity(string path, string activityName)
        {
            bool status = false;

            string aName = ParameterBindingInfo.GetActivityName(path);

            if (aName == activityName)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the path of a parameter binding refers
        /// to a property of an activity.
        /// </summary>
        /// <param name="path">Path of parameter binding</param>
        /// <param name="activityName">An property name</param>
        /// <returns>true if the path refers to the property, false otherwise</returns>
        private bool IsSameProperty(string path, string propertyName)
        {
            bool status = false;

            string aName = ParameterBindingInfo.GetPropertyName(path);

            if (aName == propertyName)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets the selected DataViewElement
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <returns></returns>
        private InstanceAttributePropertyDescriptor GetSelectedAttributeDescriptor(ParameterBindingInfo bindingInfo)
        {
            InstanceAttributePropertyDescriptor pd = null;

            if (_instanceView != null)
            {
                pd = (InstanceAttributePropertyDescriptor)_instanceView.GetProperties(null)[bindingInfo.Path];
            }

            return pd;
        }

        /// <summary>
        /// Gets the selected InputParameter
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <returns></returns>
        private InputParameter GetSelectedParameter(ParameterBindingInfo bindingInfo)
        {
            InputParameter parameter = null;

            if (_rootActivity.InputParameters != null)
            {
                foreach (InputParameter param in _rootActivity.InputParameters)
                {
                    if (param.Name == bindingInfo.Path)
                    {
                        parameter = param;
                        break;
                    }
                }
            }

            return parameter;
        }

        /// <summary>
        /// Gets the selected activity property
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <returns></returns>
        private PropertyInfo GetSelectedActivityProperty(ParameterBindingInfo bindingInfo)
        {
            PropertyInfo property = null;

            string activityName = ParameterBindingInfo.GetActivityName(bindingInfo.Path);
            string propertyName = ParameterBindingInfo.GetPropertyName(bindingInfo.Path);

            Activity activity = ((Activity)_rootActivity).GetActivityByName(activityName);
            if (activity != null)
            {
                property = activity.GetType().GetProperty(propertyName);
            }

            return property;
        }

        #region event handlers

        private void SelectParameterBindingDialog_Load(object sender, EventArgs e)
        {
            Activity currentActivity = ActivityCache.Instance.CurrentActivity;
            _rootActivity = ActivityUtil.GetRootActivity(currentActivity);
            int tabIndex = 0;
            // display attributes of binding data class
            if (ShowBindingClassAttributes())
            {
                this.tabControl1.SelectedIndex = tabIndex;
            }

            tabIndex++;

            // display the input paramters of the worklfow that is root of the invoking activity
            if (ShowInputParameters())
            {
                this.tabControl1.SelectedIndex = tabIndex;
            }

            tabIndex++;
            // display the activities defined in the root workflow
            if (ShowActivities())
            {
                this.tabControl1.SelectedIndex = tabIndex;
            }
        }

        private void classListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (classListView.SelectedIndices.Count == 1)
            {
                _bindingInfo.SourceType = SourceType.DataInstance;
                _bindingInfo.Path = classListView.SelectedItems[0].SubItems[1].Text; // path is attribute name
                this.okButton.Enabled = true;
            }
        }

        private void parameterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.parameterListBox.SelectedIndex >= 0)
            {
                _bindingInfo.SourceType = SourceType.Parameter;
                _bindingInfo.Path = (string)parameterListBox.SelectedItem;

                this.okButton.Enabled = true;
            }
        }

        private void activityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowActivityProperties();
        }

        private void propertyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (propertyListBox.SelectedIndex >= 0)
            {
                StringBuilder builder = new StringBuilder();

                _bindingInfo.SourceType = SourceType.Activity;

                builder.Append((string)activityComboBox.SelectedItem);
                builder.Append(ParameterBindingInfo.SEPARATOR).Append((string)propertyListBox.SelectedItem);

                _bindingInfo.Path = builder.ToString();

                this.okButton.Enabled = true;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (_bindingInfo != null)
            {
                Type sourceType = typeof(string);

                switch (_bindingInfo.SourceType)
                {
                    case SourceType.DataInstance:

                        sourceType = ActivityUtil.GetClassAttributeType(GetSelectedAttributeDescriptor(_bindingInfo));
                       
                        break;

                    case SourceType.Parameter:

                        sourceType = ActivityUtil.GetParameterType(GetSelectedParameter(_bindingInfo));

                        break;

                    case SourceType.Activity:

                        sourceType = ActivityUtil.GetActivityPropertyType(GetSelectedActivityProperty(_bindingInfo));
                        
                        break;
                }

                if (!ActivityUtil.CanConvertFrom(sourceType, _dataType))
                {
                    string msg = string.Format(Newtera.WorkflowStudioControl.MessageResourceManager.GetString("WorkflowStudioApp.DataTypeMismatch"), Enum.GetName(typeof(ParameterDataType), _dataType));
                    MessageBox.Show(msg,
                        "Error Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
        }

        #endregion
    }
}
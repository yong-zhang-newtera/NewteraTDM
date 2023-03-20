using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WFModel;
using Newtera.WorkflowStudioControl;

namespace WorkflowStudio
{
    public partial class DefineCustomActionDialog : Form
    {
        private CustomActionCollection _customActions;
        private string _bindingSchemaId = null;
        private string _bindingClassName = null;

        public DefineCustomActionDialog()
        {
            InitializeComponent();

            _customActions = null;
        }

        /// <summary>
        /// Gets or sets the class name that is bound to the workflow
        /// </summary>
        public string BindingClassName
        {
            get
            {
                return _bindingClassName;
            }
            set
            {
                _bindingClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema id of the binding class
        /// </summary>
        public string BindingSchemaId
        {
            get
            {
                return _bindingSchemaId;
            }
            set
            {
                _bindingSchemaId = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom actions
        /// </summary>
        public CustomActionCollection CustomActions
        {
            get
            {
                if (_customActions == null)
                {
                    _customActions = new CustomActionCollection();
                }

                return _customActions;
            }
            set
            {
                _customActions = value;
                if (_customActions != null)
                {
                    foreach (CustomAction customAction in _customActions)
                    {
                        customAction.SchemaId = BindingSchemaId;
                        customAction.ClassName = BindingClassName;
                    }
                }
            }
        }

        /// <summary>
        /// Add a custom action
        /// </summary>
        private void AddCustomAction()
        {
            if (!string.IsNullOrEmpty(nameEnterTextBox.Text))
            {
                if (!IsCustomActionExist(nameEnterTextBox.Text))
                {
                    CustomAction customAction = new CustomAction(nameEnterTextBox.Text);
                    customAction.SchemaId = BindingSchemaId;
                    customAction.ClassName = BindingClassName;
                    this.CustomActions.Add(customAction);
                    int pos = customActionsListBox.Items.Add(customAction.Name);
                    customActionsListBox.SelectedIndex = pos; // make the new item selected
                }
                else
                {
                    // show the error
                    string msg = string.Format(MessageResourceManager.GetString("WorkflowStudio.CustomActionExist"), nameEnterTextBox.Text);
                    MessageBox.Show(msg,
                        "Error Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudio.NullCustomActionName"),
                    "Error Dialog", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets information indicating whether a custom action of a given name already exists or not
        /// </summary>
        /// <param name="customActionName">Custom action name</param>
        /// <returns>true if exists, false otherwsie</returns>
        private bool IsCustomActionExist(string customActionName)
        {
            bool status = false;

            if (CustomActions != null)
            {
                foreach (CustomAction customAction in CustomActions)
                {
                    if (customAction.Name == customActionName)
                    {
                        status = true;
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Delete a custom action
        /// </summary>
        private void DelCustomAction()
        {
            if (customActionsListBox.SelectedIndex >= 0)
            {
                CustomActions.RemoveAt(this.customActionsListBox.SelectedIndex);

                customActionsListBox.Items.RemoveAt(this.customActionsListBox.SelectedIndex);

                if (this.CustomActions.Count > 0)
                {
                    this.customActionsListBox.SelectedIndex = 0; // select the first item
                }
                else
                {
                    this.propertyGrid1.SelectedObject = null;
                }
            }
        }

        private void DefineCustomActionDialog_Load(object sender, EventArgs e)
        {
            foreach (CustomAction customAction in CustomActions)
            {
                customActionsListBox.Items.Add(customAction.Name);
            }

            if (customActionsListBox.Items.Count > 0)
            {
                customActionsListBox.SelectedIndex = 0; // select first item
            }
        }

        private void customActionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customActionsListBox.SelectedIndex >= 0)
            {
                CustomAction customAction = (CustomAction)CustomActions[customActionsListBox.SelectedIndex];
                propertyGrid1.SelectedObject = customAction;
            }
            else
            {
                propertyGrid1.SelectedObject = null;
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

        }

        private void nameEnterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddCustomAction();
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddCustomAction();
        }

        private void delButton_Click(object sender, EventArgs e)
        {
            DelCustomAction();
        }
    }
}
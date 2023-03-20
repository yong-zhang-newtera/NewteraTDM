using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Newtera.WFModel;
using Newtera.Common.MetaData.XaclModel;
using Newtera.WorkflowStudioControl;

namespace WorkflowStudio
{
    public partial class AccessControlDialog : Form
    {
        private ProjectModel _project;
        private XaclPolicy _xaclPolicy;

        public AccessControlDialog()
        {
            InitializeComponent();
            _xaclPolicy = new XaclPolicy();
        }

        /// <summary>
        /// Gets or sets the ProjectModel instance
        /// </summary>
        public ProjectModel Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;
            }
        }

        /// <summary>
        /// Gets or sets the project policy instance
        /// </summary>
        public XaclPolicy Policy
        {
            get
            {
                return _xaclPolicy;
            }
            set
            {
                // copy the policy so that we can modify it without changing the original one
                StringBuilder builder = new StringBuilder();
                StringWriter writer = new StringWriter(builder);
                value.Write(writer);

                _xaclPolicy = new XaclPolicy();
                StringReader reader = new StringReader(builder.ToString());
                _xaclPolicy.Read(reader);
            }
        }

        /// <summary>
        /// Add an xacl rule
        /// </summary>
        /// <param name="user">The user for the rule</param>
        /// <param name="roles">The roles for the rule</param>
        private void AddXaclRule(String user, StringCollection roles)
        {
            if (_project != null)
            {
                XaclObject obj = new XaclObject(_project.ToXPath());

                XaclSubject subject = new XaclSubject();
                if (user != null)
                {
                    subject.Uid = user;
                }
                else if (roles != null)
                {
                    foreach (string role in roles)
                    {
                        subject.AddRole(role);
                    }
                }

                XaclRule rule = new XaclRule(subject, obj.Href);
                if (!_xaclPolicy.IsRuleExist(obj, rule))
                {
                    _xaclPolicy.AddRule(obj, rule);
                    XaclRuleListViewItem listViewItem = new XaclRuleListViewItem(obj, rule);
                    listViewItem.Selected = true;
                    this.listView1.Items.Add(listViewItem);
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioApp.RuleExists"),
                        "Information Dialog", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Remove an XaclRule from currently displayed rules
        /// </summary>
        /// <param name="listViewItem">The list view item for the rule</param>
        private void RemoveXaclRule(XaclRuleListViewItem listViewItem)
        {
            _xaclPolicy.RemoveRule(listViewItem.Object, listViewItem.Rule);

            this.listView1.Items.Remove(listViewItem);
        }

        /// <summary>
        /// Display and bind the detail of a rule to UI controls
        /// </summary>
        /// <param name="rule">The rule</param>
        private void ShowXaclRuleDetail(XaclRule rule)
        {
            XaclActionCollection actions = rule.Actions;

            foreach (XaclAction action in actions)
            {
                switch (action.ActionType)
                {
                    case XaclActionType.Read:
                        if (action.Permission == XaclPermissionType.Grant)
                        {
                            this.readAllowRadioButton.Checked = true;
                            this.readDenyRadioButton.Checked = false;
                        }
                        else
                        {
                            this.readAllowRadioButton.Checked = false;
                            this.readDenyRadioButton.Checked = true;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Set a permission to a rule of a certain action type
        /// </summary>
        /// <param name="rule">The rule</param>
        /// <param name="type">The action type</param>
        /// <param name="isAllowed">IsAllowed</param>
        private void SetXaclRulePermission(XaclRule rule, XaclActionType actionType, bool isAllowed)
        {
            XaclActionCollection actions = rule.Actions;

            foreach (XaclAction action in actions)
            {
                if (action.ActionType == actionType)
                {
                    if (isAllowed)
                    {
                        action.Permission = XaclPermissionType.Grant;
                    }
                    else
                    {
                        action.Permission = XaclPermissionType.Deny;
                    }
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                XaclRuleListViewItem selectedItem = (XaclRuleListViewItem)this.listView1.SelectedItems[0];

                RemoveXaclRule(selectedItem);

                this.deleteButton.Enabled = false;
            }		
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                XaclRuleListViewItem selectedItem = (XaclRuleListViewItem)this.listView1.SelectedItems[0];

                ShowXaclRuleDetail(selectedItem.Rule);

                this.deleteButton.Enabled = true;
            }
        }

        private void readAllowRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                XaclRuleListViewItem selectedItem = (XaclRuleListViewItem)this.listView1.SelectedItems[0];

                SetXaclRulePermission(selectedItem.Rule, XaclActionType.Read, readAllowRadioButton.Checked);
            }
        }

        private void addRolesButton_Click(object sender, EventArgs e)
        {
            SelectRolesDialog dialog = new SelectRolesDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.Roles.Count > 0)
                {
                    this.AddXaclRule(null, dialog.Roles);
                }
            }
        }

        private void addUsersButton_Click(object sender, EventArgs e)
        {
            SelectUsersDialog dialog = new SelectUsersDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.Users.Count > 0)
                {
                    foreach (string user in dialog.Users)
                    {
                        AddXaclRule(user, null);
                    }
                }
            }
        }

        private void AccessControlDialog_Load(object sender, EventArgs e)
        {
            // display the rules in the policy
            this.listView1.SuspendLayout();

            //InitializeXaclRuleControls();
            XaclRuleListViewItem listViewItem;

            if (_project != null && _xaclPolicy != null)
            {
                XaclObject obj = new XaclObject(_project.ToXPath());

                XaclRuleCollection rules = _xaclPolicy.GetRules(obj);
                int index = 0;
                foreach (XaclRule rule in rules)
                {
                    listViewItem = new XaclRuleListViewItem(obj, rule);
                    if (index == 0)
                    {
                        listViewItem.Selected = true;
                    }

                    this.listView1.Items.Add(listViewItem);

                    index++;
                }
            }

            this.listView1.ResumeLayout();
        }

        private void advancedButton_Click(object sender, EventArgs e)
        {
            AccessControlOptionDialog dialog = new AccessControlOptionDialog();
            dialog.Policy = _xaclPolicy;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _xaclPolicy.Setting.ConflictResolutionType = dialog.ConflictResolution;
                _xaclPolicy.Setting.DefaultReadPermission = dialog.DefaultReadPermission;
            }
        }
    }
}
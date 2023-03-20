using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using System.Workflow.ComponentModel;

using Newtera.WindowsControl;
using Newtera.Activities;

namespace Newtera.WorkflowMonitor
{
    public partial class CancelActivityDialog : Form
    {
        private string _selectedActivityName;
        private CompositeActivity _rootActivity;

        public CancelActivityDialog()
        {
            InitializeComponent();

            _selectedActivityName = null;
        }

        public CompositeActivity RootActivity
        {
            get
            {
                return _rootActivity;
            }
            set
            {
                _rootActivity = value;
            }
        }

        public string SelectedActivityName
        {
            get
            {
                return _selectedActivityName;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (this.activityNameComboBox.SelectedIndex >= 0)
            {

                _selectedActivityName = this.activityNameComboBox.SelectedValue.ToString();
            }
            else
            {
                _selectedActivityName = null;
            }
        }

        private void CancelActivityDialog_Load(object sender, EventArgs e)
        {
            if (_rootActivity != null)
            {
                StringCollection activityNameCollection = new StringCollection();
                // travel all the child activities, and collect the names of HandleNewteraEventActivity
                // and display the names in the dropdown list
                FindActivities(_rootActivity, activityNameCollection);

                this.activityNameComboBox.DataSource = activityNameCollection;
                if (activityNameCollection.Count > 0)
                {
                    this.activityNameComboBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Find all HandleNewteraEventActivity children in the composite activity, and keep the name
        /// in the StringCollection. This method is called recursively.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="activityNameCollection"></param>
        private void FindActivities(CompositeActivity parent, StringCollection activityNameCollection)
        {
            foreach (Activity child in parent.Activities)
            {
                if (child is CompositeActivity)
                {
                    // call it recursively
                    FindActivities((CompositeActivity)child, activityNameCollection);
                }
                else if (child is HandleNewteraEventActivity)
                {
                    activityNameCollection.Add(child.Name);
                }
                else if (child is HandleGroupTaskEventActivity)
                {
                    activityNameCollection.Add(child.Name);
                }
                else if (child is HandleWorkflowEventActivity)
                {
                    activityNameCollection.Add(child.Name);
                }
            }
        }
    }
}
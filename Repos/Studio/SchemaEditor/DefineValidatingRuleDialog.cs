using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.Rules;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
    public partial class DefineValidatingRuleDialog : Form
    {
        private RuleDef _ruleDef = null;

        public DefineValidatingRuleDialog()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Gets or sets the rule definition defined by the dialog
        /// </summary>
        internal RuleDef RuleDef
        {
            get
            {
                return _ruleDef;
            }
            set
            {
                _ruleDef = value;
            }
        }

        private void DefineValidatingRuleDialog_Load(object sender, EventArgs e)
        {
            if (_ruleDef != null)
            {
                this.propertyGrid.SelectedObject = _ruleDef;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // make sure that rule definition is completed
            if (_ruleDef.Condition == null ||
                _ruleDef.ThenAction == null)
            {
                MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.IncompleteRule"),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.DialogResult = DialogResult.None; // cancel the OK event
                return;
            }
        }
    }
}
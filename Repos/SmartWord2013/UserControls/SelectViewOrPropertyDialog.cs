using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartWord2013
{
    public partial class SelectViewOrPropertyDialog : Form
    {
        private RadioButton viewRadioButton;
        private RadioButton propertyRadioButton;
        private Button okButton;
        private Button cancelButton;
        private string _nodeName = ThisDocument.ViewNodeName;

        public SelectViewOrPropertyDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the node name to be inserted
        /// </summary>
        public string InsertNodeName
        {
            get
            {
                return _nodeName;
            }
        }

        private void viewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (viewRadioButton.Checked)
            {
                _nodeName = ThisDocument.ViewNodeName;
            }
            else
            {
                _nodeName = ThisDocument.PropertyNodeName;
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectViewOrPropertyDialog));
            this.viewRadioButton = new System.Windows.Forms.RadioButton();
            this.propertyRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // viewRadioButton
            // 
            resources.ApplyResources(this.viewRadioButton, "viewRadioButton");
            this.viewRadioButton.Checked = true;
            this.viewRadioButton.Name = "viewRadioButton";
            this.viewRadioButton.TabStop = true;
            this.viewRadioButton.UseVisualStyleBackColor = true;
            this.viewRadioButton.CheckedChanged += new System.EventHandler(this.viewRadioButton_CheckedChanged);
            // 
            // propertyRadioButton
            // 
            resources.ApplyResources(this.propertyRadioButton, "propertyRadioButton");
            this.propertyRadioButton.Name = "propertyRadioButton";
            this.propertyRadioButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // SelectViewOrPropertyDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.propertyRadioButton);
            this.Controls.Add(this.viewRadioButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectViewOrPropertyDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
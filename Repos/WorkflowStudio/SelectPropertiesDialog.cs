using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
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
	/// <summary>
	/// Summary description for SelectPropertiesDialog.
	/// </summary>
	public class SelectPropertiesDialog : System.Windows.Forms.Form
	{
        private const string NAME_PAIR_SEPARATOR = ":";

        ResultAttributeCollection _allProperties;
		private StringCollection _selectedProperties = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox allPropertiesListBox;
        private System.Windows.Forms.ListBox selectedPropertiesListBox;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
        private CheckBox allowManulUpdateCheckBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectPropertiesDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            _selectedProperties = new StringCollection();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Gets or sets the selected properties.
		/// </summary>
		public StringCollection FormProperties
		{
			get
			{
				return _selectedProperties;
			}
            set
            {
                _selectedProperties = value;
            }
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectPropertiesDialog));
            this.allPropertiesListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.selectedPropertiesListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.allowManulUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // allPropertiesListBox
            // 
            resources.ApplyResources(this.allPropertiesListBox, "allPropertiesListBox");
            this.allPropertiesListBox.Name = "allPropertiesListBox";
            this.allPropertiesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // selectedPropertiesListBox
            // 
            resources.ApplyResources(this.selectedPropertiesListBox, "selectedPropertiesListBox");
            this.selectedPropertiesListBox.Name = "selectedPropertiesListBox";
            this.selectedPropertiesListBox.SelectedIndexChanged += new System.EventHandler(this.selectedPropertiesListBox_SelectedIndexChanged);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // delButton
            // 
            resources.ApplyResources(this.delButton, "delButton");
            this.delButton.Name = "delButton";
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // allowManulUpdateCheckBox
            // 
            resources.ApplyResources(this.allowManulUpdateCheckBox, "allowManulUpdateCheckBox");
            this.allowManulUpdateCheckBox.Checked = true;
            this.allowManulUpdateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allowManulUpdateCheckBox.Name = "allowManulUpdateCheckBox";
            this.allowManulUpdateCheckBox.UseVisualStyleBackColor = true;
            this.allowManulUpdateCheckBox.CheckedChanged += new System.EventHandler(this.allowManulUpdateCheckBox_CheckedChanged);
            // 
            // SelectPropertiesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.allowManulUpdateCheckBox);
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.selectedPropertiesListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.allPropertiesListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectPropertiesDialog";
            this.Load += new System.EventHandler(this.SelectUsersDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ShowItems()
		{
			this.allPropertiesListBox.BeginUpdate();

			this.allPropertiesListBox.Items.Clear();

            foreach (IDataViewElement property in this._allProperties)
			{
				this.allPropertiesListBox.Items.Add(property.Caption);
			}

			this.allPropertiesListBox.EndUpdate();

			this.selectedPropertiesListBox.BeginUpdate();

			this.selectedPropertiesListBox.Items.Clear();

            StringCollection deletedProperties = new StringCollection();
            string truePropertyName;
            foreach (string propertyName in this._selectedProperties)
            {
                truePropertyName = GetPropertyName(propertyName);
                IDataViewElement property = _allProperties[truePropertyName];
                if (property != null)
                {
                    this.selectedPropertiesListBox.Items.Add(property.Caption);
                }
                else
                {
                    // the property selected previously has been removed from the binding class
                    deletedProperties.Add(truePropertyName);
                }
            }

            // remove the deleted properties
            foreach (string deletedProperty in deletedProperties)
            {
                _selectedProperties.Remove(deletedProperty);
            }

			this.selectedPropertiesListBox.EndUpdate();
		}

        private bool AllowManualUpdate(string propertyName)
        {
            bool status = true;

            int pos = propertyName.IndexOf(NAME_PAIR_SEPARATOR);

            if (pos > 0 && propertyName.Substring(pos + 1).Trim() == "r")
            {
                status = false;
            }

            return status;
        }

        private string GetPropertyName(string name)
        {
            string propertyName = name;

            int pos = name.IndexOf(NAME_PAIR_SEPARATOR);
            if (pos > 0)
            {
                propertyName = name.Substring(0, pos);
            }

            return propertyName;
        }

		private void SelectUsersDialog_Load(object sender, System.EventArgs e)
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
                        ResultAttributeCollection attributes = dataView.ResultAttributes;
                        this.allPropertiesListBox.Items.Clear();
                        _allProperties = new ResultAttributeCollection();
                        foreach (IDataViewElement result in attributes)
                        {
                            if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.SimpleAttribute)
                            {
                                _allProperties.Add(result);
                            }
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.ArrayAttribute)
                            {
                                _allProperties.Add(result);
                            }
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.ImageAttribute)
                            {
                                _allProperties.Add(result);
                            }
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.VirtualAttribute)
                            {
                                _allProperties.Add(result);
                            }
                            else if (result.ElementType == Newtera.Common.MetaData.DataView.ElementType.RelationshipAttribute)
                            {
                                RelationshipAttributeElement relationship = result.GetSchemaModelElement() as RelationshipAttributeElement;
                                if (relationship != null)
                                {
                                    if (relationship.IsForeignKeyRequired)
                                    {
                                        _allProperties.Add(result);
                                    }
                                    else if (relationship.LinkedClass.IsJunction && relationship.Usage == DefaultViewUsage.Included)
                                    {
                                        _allProperties.Add(result); // many-to-many relationship
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            ShowItems();
		}

        private void addButton_Click(object sender, System.EventArgs e)
		{
            ListBox.SelectedIndexCollection selectedIndices = this.allPropertiesListBox.SelectedIndices;

            if (selectedIndices.Count > 0)
            {
                foreach (int index in selectedIndices)
                {
                    IDataViewElement property = _allProperties[index];
                    if (!ContainsProperty(_selectedProperties, property.Name))
                    {
                        _selectedProperties.Add(property.Name);
                        this.selectedPropertiesListBox.Items.Add(property.Caption);
                    }
                    else
                    {
                        MessageBox.Show(property.Caption + Newtera.WorkflowStudioControl.MessageResourceManager.GetString("WorkflowStudio.PropertyExist"), "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                allPropertiesListBox.ClearSelected();
            }
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			int index = this.selectedPropertiesListBox.SelectedIndex;

			if (index >= 0 && index < _selectedProperties.Count)
			{
				_selectedProperties.RemoveAt(index);
                this.selectedPropertiesListBox.Items.RemoveAt(index);
			}		
		}

        private bool ContainsProperty(StringCollection properties, string propertyName)
        {
            bool status = false;
            string tempName;
            
            foreach (string temp in properties)
            {
                tempName = GetPropertyName(temp);
                if (propertyName == tempName)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        private void selectedPropertiesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.selectedPropertiesListBox.SelectedIndex;

            if (index >= 0 && index < _selectedProperties.Count)
            {
                string propertyName = _selectedProperties[index];
                if (AllowManualUpdate(propertyName))
                {
                    this.allowManulUpdateCheckBox.Checked = true;
                }
                else
                {
                    this.allowManulUpdateCheckBox.Checked = false;
                }
            }	
        }

        private void allowManulUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int index = this.selectedPropertiesListBox.SelectedIndex;

            if (index >= 0 && index < _selectedProperties.Count)
            {
                string propertyName = _selectedProperties[index];
                if (this.allowManulUpdateCheckBox.Checked)
                {
                    if (!AllowManualUpdate(propertyName))
                    {
                        // remove the :r at the end
                        int pos = propertyName.IndexOf(NAME_PAIR_SEPARATOR);
                        _selectedProperties[index] = propertyName.Substring(0, pos);
                    }
                }
                else
                {
                    if (AllowManualUpdate(propertyName))
                    {
                        // add :r at the end
                        _selectedProperties[index] += NAME_PAIR_SEPARATOR + "r";
                    }
                }
            }
        }
	}
}

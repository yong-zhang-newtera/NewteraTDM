using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ChooseAttributeUpdatedDialog.
	/// </summary>
	public class ChooseAttributesUpdatedDialog : System.Windows.Forms.Form
	{
		private ClassElement _classElement = null;
		private StringCollection _attributes = null;
		private StringCollection _attributeUpdated = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox attributeListBox;
		private System.Windows.Forms.ListBox selectedListBox;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseAttributesUpdatedDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            _attributeUpdated = new StringCollection();
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
		/// Gets or sets the class element that the primary keys are defined for.
		/// </summary>
		public ClassElement ClassElement
		{
			get
			{
				return _classElement;
			}
			set
			{
				_classElement = value;

                _attributes = new StringCollection();

                ClassElement currentClass = value;
                while (currentClass != null)
                {
                    foreach (SimpleAttributeElement attr in currentClass.SimpleAttributes)
                    {
                        if (!IsSelectedAttribute(attr.Name))
                        {
                            _attributes.Add(attr.Name);
                        }
                    }

                    currentClass = currentClass.ParentClass;
                }
			}
		}

		/// <summary>
		/// Gets or sets the selected attributes.
		/// </summary>
		public StringCollection AttributesUpdated
		{
			get
			{
				return _attributeUpdated;
			}
            set
            {
                // duplicate the string collection so that it makes the property editor's
                // value change event to happen
                if (value != null)
                {
                    foreach (string attr in value)
                    {
                        _attributeUpdated.Add(attr);
                    }
                }
            }
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseAttributesUpdatedDialog));
            this.attributeListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.selectedListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // attributeListBox
            // 
            resources.ApplyResources(this.attributeListBox, "attributeListBox");
            this.attributeListBox.Name = "attributeListBox";
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
            // selectedListBox
            // 
            resources.ApplyResources(this.selectedListBox, "selectedListBox");
            this.selectedListBox.Name = "selectedListBox";
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
            // ChooseAttributesUpdatedDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.selectedListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.attributeListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChooseAttributesUpdatedDialog";
            this.Load += new System.EventHandler(this.ChooseAttributesUpdatedDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ShowItems()
		{
            SimpleAttributeElement attribute;

			this.attributeListBox.BeginUpdate();

			this.attributeListBox.Items.Clear();

			foreach (string attributeName in this._attributes)
			{
                attribute = _classElement.FindInheritedSimpleAttribute(attributeName);
                if (attribute != null)
                {
                    this.attributeListBox.Items.Add(attribute.Caption);
                }
			}

			this.attributeListBox.EndUpdate();

			this.selectedListBox.BeginUpdate();

			this.selectedListBox.Items.Clear();

			foreach (string attributeName in this._attributeUpdated)
			{
                attribute = _classElement.FindInheritedSimpleAttribute(attributeName);
                if (attribute != null)
                {
                    this.selectedListBox.Items.Add(attribute.Caption);
                }
			}

			this.selectedListBox.EndUpdate();
		}

        private bool IsSelectedAttribute(string attributeName)
        {
            bool found = false;

            foreach (string name in _attributeUpdated)
            {
                if (attributeName == name)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

		private void ChooseAttributesUpdatedDialog_Load(object sender, System.EventArgs e)
		{
			if (_classElement != null)
			{
				ShowItems();
			}
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			int index = this.attributeListBox.SelectedIndex;

			if (index >= 0 && index < _attributes.Count)
			{
				string attributeName = _attributes[index];
				_attributes.RemoveAt(index);
                _attributeUpdated.Add(attributeName);
				
				ShowItems();
			}
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			int index = this.selectedListBox.SelectedIndex;

			if (index >= 0 && index < _attributeUpdated.Count)
			{
                string attributeName = _attributeUpdated[index];
				_attributeUpdated.RemoveAt(index);
                _attributes.Add(attributeName);

				ShowItems();
			}		
		}
	}
}

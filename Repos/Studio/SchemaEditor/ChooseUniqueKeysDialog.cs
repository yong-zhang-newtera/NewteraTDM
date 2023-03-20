using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ChooseUniqueKeysDialog.
	/// </summary>
	public class ChooseUniqueKeysDialog : System.Windows.Forms.Form
	{
		private ClassElement _classElement = null;
		private SchemaModelElementCollection _attributes = null;
		private SchemaModelElementCollection _uniqueKeys = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox attributeListBox;
		private System.Windows.Forms.ListBox ukListBox;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseUniqueKeysDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			}
		}

		/// <summary>
		/// Gets the selected primary keys.
		/// </summary>
		public SchemaModelElementCollection UniqueKeys
		{
			get
			{
				return _uniqueKeys;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseUniqueKeysDialog));
            this.attributeListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ukListBox = new System.Windows.Forms.ListBox();
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
            // ukListBox
            // 
            resources.ApplyResources(this.ukListBox, "ukListBox");
            this.ukListBox.Name = "ukListBox";
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
            // ChooseUniqueKeysDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.ukListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.attributeListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChooseUniqueKeysDialog";
            this.Load += new System.EventHandler(this.ChoosePrimaryKeysDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ShowItems()
		{
			this.attributeListBox.BeginUpdate();

			this.attributeListBox.Items.Clear();

            foreach (AttributeElementBase attribute in this._attributes)
			{
				this.attributeListBox.Items.Add(attribute.Caption);
			}

			this.attributeListBox.EndUpdate();

			this.ukListBox.BeginUpdate();

			this.ukListBox.Items.Clear();

            foreach (AttributeElementBase attribute in this._uniqueKeys)
			{
				this.ukListBox.Items.Add(attribute.Caption);
			}

			this.ukListBox.EndUpdate();
		}

		private void ChoosePrimaryKeysDialog_Load(object sender, System.EventArgs e)
		{
			if (_classElement != null)
			{
				_attributes = new SchemaModelElementCollection();

                MetaDataElementSortedList sortedList = new MetaDataElementSortedList();

                ClassElement currentClass = _classElement;
                SchemaModelElementCollection localAttributes;
                
                int level = 10; // estimate of maximum levels of inheritance
                while (currentClass != null)
                {
                    level--;
                    if (level < 0)
                    {
                        level = 0;
                    }

                    localAttributes = currentClass.SimpleAttributes;
                    if (localAttributes != null)
                    {
                        foreach (SimpleAttributeElement attr in localAttributes)
                        {
                            // attributes of parent appears first
                            sortedList.Add(level * 1000 + attr.Position, attr);
                        }
                    }

                    localAttributes = currentClass.RelationshipAttributes;
                    if (localAttributes != null)
                    {
                        foreach (RelationshipAttributeElement attr in localAttributes)
                        {
                            // only add relationship with local foreign keys
                            if (attr.IsForeignKeyRequired)
                            {
                                // attributes of parent appears first
                                sortedList.Add(level * 1000 + attr.Position, attr);
                            }
                        }
                    }

                    currentClass = currentClass.ParentClass;
                }

                // Display the attributes that is not part of the unique keys and
                // is not auto-incremental
                foreach (AttributeElementBase attribute in sortedList.Values)
				{
                    if (!_classElement.UniqueKeys.Contains(attribute))
                    {
                        if (attribute is SimpleAttributeElement &&
                            !((SimpleAttributeElement)attribute).IsAutoIncrement)
                        {
                            _attributes.Add(attribute);
                        }
                        else
                        {
                            _attributes.Add(attribute);
                        }
                    }
				}

				_uniqueKeys = new SchemaModelElementCollection();
                foreach (AttributeElementBase uk in _classElement.UniqueKeys)
				{
					_uniqueKeys.Add(uk);
				}

				ShowItems();
			}
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			int index = this.attributeListBox.SelectedIndex;

			if (index >= 0 && index < _attributes.Count)
			{
                AttributeElementBase attribute = (AttributeElementBase)_attributes[index];

				_attributes.RemoveAt(index);
				_uniqueKeys.Add(attribute);
				
				ShowItems();
			}
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			int index = this.ukListBox.SelectedIndex;

			if (index >= 0 && index < _uniqueKeys.Count)
			{
                AttributeElementBase attribute = (AttributeElementBase)_uniqueKeys[index];
				_uniqueKeys.RemoveAt(index);
				_attributes.Add(attribute);

				ShowItems();
			}
        }
	}
}

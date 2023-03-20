using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ChoosePrimaryKeysDialog.
	/// </summary>
	public class ChoosePrimaryKeysDialog : System.Windows.Forms.Form
	{
		private ClassElement _classElement = null;
		private SchemaModelElementCollection _attributes = null;
		private SchemaModelElementCollection _primaryKeys = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox attributeListBox;
		private System.Windows.Forms.ListBox pkListBox;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChoosePrimaryKeysDialog()
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
		public SchemaModelElementCollection PrimaryKeys
		{
			get
			{
				return _primaryKeys;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoosePrimaryKeysDialog));
            this.attributeListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pkListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // attributeListBox
            // 
            this.attributeListBox.AccessibleDescription = null;
            this.attributeListBox.AccessibleName = null;
            resources.ApplyResources(this.attributeListBox, "attributeListBox");
            this.attributeListBox.BackgroundImage = null;
            this.attributeListBox.Font = null;
            this.attributeListBox.Name = "attributeListBox";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // pkListBox
            // 
            this.pkListBox.AccessibleDescription = null;
            this.pkListBox.AccessibleName = null;
            resources.ApplyResources(this.pkListBox, "pkListBox");
            this.pkListBox.BackgroundImage = null;
            this.pkListBox.Font = null;
            this.pkListBox.Name = "pkListBox";
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            // 
            // addButton
            // 
            this.addButton.AccessibleDescription = null;
            this.addButton.AccessibleName = null;
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.BackgroundImage = null;
            this.addButton.Font = null;
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // delButton
            // 
            this.delButton.AccessibleDescription = null;
            this.delButton.AccessibleName = null;
            resources.ApplyResources(this.delButton, "delButton");
            this.delButton.BackgroundImage = null;
            this.delButton.Font = null;
            this.delButton.Name = "delButton";
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // ChoosePrimaryKeysDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.pkListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.attributeListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.Name = "ChoosePrimaryKeysDialog";
            this.Load += new System.EventHandler(this.ChoosePrimaryKeysDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ShowItems()
		{
			this.attributeListBox.BeginUpdate();

			this.attributeListBox.Items.Clear();

			foreach (SimpleAttributeElement attribute in this._attributes)
			{
				this.attributeListBox.Items.Add(attribute.Caption);
			}

			this.attributeListBox.EndUpdate();

			this.pkListBox.BeginUpdate();

			this.pkListBox.Items.Clear();

			foreach (SimpleAttributeElement attribute in this._primaryKeys)
			{
				this.pkListBox.Items.Add(attribute.Caption);
			}

			this.pkListBox.EndUpdate();
		}

		private void ChoosePrimaryKeysDialog_Load(object sender, System.EventArgs e)
		{
			if (_classElement != null)
			{
				_attributes = new SchemaModelElementCollection();
				foreach (SimpleAttributeElement attribute in _classElement.SimpleAttributes)
				{
					if (!attribute.IsPrimaryKey)
					{
						_attributes.Add(attribute);
					}
				}

				_primaryKeys = new SchemaModelElementCollection();
				foreach (SimpleAttributeElement pk in _classElement.PrimaryKeys)
				{
					_primaryKeys.Add(pk);
				}

				ShowItems();
			}
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			int index = this.attributeListBox.SelectedIndex;

			if (index >= 0 && index < _attributes.Count)
			{
				SimpleAttributeElement attribute = (SimpleAttributeElement) _attributes[index];

                if (attribute.IsUnique)
                {
                    MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.UKNotAllowedForPK"));
                }
                else if (attribute.DataType == DataType.Boolean ||
                    attribute.DataType == DataType.DateTime ||
                    attribute.DataType == DataType.Text ||
                    attribute.DataType == DataType.Date)
                {
                    MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.BadTypesForPK"));
                }
                else
                {
                    _attributes.RemoveAt(index);
                    _primaryKeys.Add(attribute);

                    ShowItems();
                }
			}
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			int index = this.pkListBox.SelectedIndex;

			if (index >= 0 && index < _primaryKeys.Count)
			{
				SimpleAttributeElement attribute = (SimpleAttributeElement) _primaryKeys[index];

				_primaryKeys.RemoveAt(index);
				_attributes.Add(attribute);

				ShowItems();
			}
        }
	}
}

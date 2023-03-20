using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.WindowsControl;

namespace Newtera.Studio.UserControls
{
	/// <summary>
	/// Summary description for ChooseSortAttributeDialog.
	/// </summary>
	public class ChooseSortAttributeDialog : System.Windows.Forms.Form
	{
		private DataViewModel _dataView;
		private DataViewElementCollection _sortAttributes;
        private int _currentSelectedIndex = 0;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ImageList listViewImageList;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ColumnHeader attributeCaptionColumnHeader;
		private System.Windows.Forms.ColumnHeader attributeNameColumnHeader;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ColumnHeader resultCaptionColumnHeader;
		private System.Windows.Forms.ColumnHeader resultNameColumnHeader;
		private System.Windows.Forms.ListView classAttributesListView;
		private System.Windows.Forms.ListView sortAttributesListView;
		private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
        private ComboBox classesComboBox;
        private Label label1;
		private System.ComponentModel.IContainer components;

		public ChooseSortAttributeDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_dataView = null;
			_sortAttributes = new DataViewElementCollection();
		}

		/// <summary>
		/// Gets or sets the data view
		/// </summary>
		/// <value>A DataViewModel</value>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
                if (_sortAttributes != null)
                {
                    _sortAttributes.DataView = value;
                }
			}
		}

		public DataViewElementCollection SortAttributes
		{
			get
			{
				return _sortAttributes;
			}
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseSortAttributeDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.listViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.classesComboBox = new System.Windows.Forms.ComboBox();
            this.classAttributesListView = new System.Windows.Forms.ListView();
            this.attributeCaptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.attributeNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.sortAttributesListView = new System.Windows.Forms.ListView();
            this.resultCaptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.resultNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
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
            // listViewImageList
            // 
            this.listViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewImageList.ImageStream")));
            this.listViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewImageList.Images.SetKeyName(0, "");
            this.listViewImageList.Images.SetKeyName(1, "");
            this.listViewImageList.Images.SetKeyName(2, "");
            this.listViewImageList.Images.SetKeyName(3, "");
            this.listViewImageList.Images.SetKeyName(4, "virtualproperty.GIF");
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.classesComboBox);
            this.groupBox1.Controls.Add(this.classAttributesListView);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // classesComboBox
            // 
            this.classesComboBox.AccessibleDescription = null;
            this.classesComboBox.AccessibleName = null;
            resources.ApplyResources(this.classesComboBox, "classesComboBox");
            this.classesComboBox.BackgroundImage = null;
            this.classesComboBox.Font = null;
            this.classesComboBox.Name = "classesComboBox";
            this.classesComboBox.SelectedIndexChanged += new System.EventHandler(this.classesComboBox_SelectedIndexChanged);
            // 
            // classAttributesListView
            // 
            this.classAttributesListView.AccessibleDescription = null;
            this.classAttributesListView.AccessibleName = null;
            resources.ApplyResources(this.classAttributesListView, "classAttributesListView");
            this.classAttributesListView.BackgroundImage = null;
            this.classAttributesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.attributeCaptionColumnHeader,
            this.attributeNameColumnHeader});
            this.classAttributesListView.Font = null;
            this.classAttributesListView.FullRowSelect = true;
            this.classAttributesListView.HideSelection = false;
            this.classAttributesListView.LargeImageList = this.listViewImageList;
            this.classAttributesListView.Name = "classAttributesListView";
            this.classAttributesListView.SmallImageList = this.listViewImageList;
            this.classAttributesListView.UseCompatibleStateImageBehavior = false;
            this.classAttributesListView.View = System.Windows.Forms.View.Details;
            this.classAttributesListView.SelectedIndexChanged += new System.EventHandler(this.classAttributesListView_SelectedIndexChanged);
            // 
            // attributeCaptionColumnHeader
            // 
            resources.ApplyResources(this.attributeCaptionColumnHeader, "attributeCaptionColumnHeader");
            // 
            // attributeNameColumnHeader
            // 
            resources.ApplyResources(this.attributeNameColumnHeader, "attributeNameColumnHeader");
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
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.downButton);
            this.groupBox2.Controls.Add(this.upButton);
            this.groupBox2.Controls.Add(this.sortAttributesListView);
            this.groupBox2.Controls.Add(this.delButton);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // downButton
            // 
            this.downButton.AccessibleDescription = null;
            this.downButton.AccessibleName = null;
            resources.ApplyResources(this.downButton, "downButton");
            this.downButton.BackgroundImage = null;
            this.downButton.Font = null;
            this.downButton.Name = "downButton";
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // upButton
            // 
            this.upButton.AccessibleDescription = null;
            this.upButton.AccessibleName = null;
            resources.ApplyResources(this.upButton, "upButton");
            this.upButton.BackgroundImage = null;
            this.upButton.Font = null;
            this.upButton.Name = "upButton";
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // sortAttributesListView
            // 
            this.sortAttributesListView.AccessibleDescription = null;
            this.sortAttributesListView.AccessibleName = null;
            resources.ApplyResources(this.sortAttributesListView, "sortAttributesListView");
            this.sortAttributesListView.BackgroundImage = null;
            this.sortAttributesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.resultCaptionColumnHeader,
            this.resultNameColumnHeader});
            this.sortAttributesListView.Font = null;
            this.sortAttributesListView.FullRowSelect = true;
            this.sortAttributesListView.HideSelection = false;
            this.sortAttributesListView.LargeImageList = this.listViewImageList;
            this.sortAttributesListView.MultiSelect = false;
            this.sortAttributesListView.Name = "sortAttributesListView";
            this.sortAttributesListView.SmallImageList = this.listViewImageList;
            this.sortAttributesListView.UseCompatibleStateImageBehavior = false;
            this.sortAttributesListView.View = System.Windows.Forms.View.Details;
            this.sortAttributesListView.SelectedIndexChanged += new System.EventHandler(this.sortAttributesListView_SelectedIndexChanged);
            // 
            // resultCaptionColumnHeader
            // 
            resources.ApplyResources(this.resultCaptionColumnHeader, "resultCaptionColumnHeader");
            // 
            // resultNameColumnHeader
            // 
            resources.ApplyResources(this.resultNameColumnHeader, "resultNameColumnHeader");
            // 
            // ChooseSortAttributeDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "ChooseSortAttributeDialog";
            this.Load += new System.EventHandler(this.ChooseSortAttributeDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		private void ShowClassAttributes(DataClass dataClass)
		{
			AttributeElementListViewItem item;
			this.classAttributesListView.SuspendLayout();
			this.classAttributesListView.Items.Clear();

			ClassElement classElement = (ClassElement) dataClass.GetSchemaModelElement();

			// display order of attributes of class hierarchy from top-down order
            // only simple attributes can be used as sort attributes
			while (classElement != null)
			{
				int index = 0;
				foreach (SimpleAttributeElement att in classElement.SimpleAttributes)
				{
                    if (att.DataType != DataType.Text)
                    {
                        item = new AttributeElementListViewItem(att.Caption, att);
                        item.ImageIndex = 0;
                        item.SubItems.Add(att.Name);

                        this.classAttributesListView.Items.Insert(index, item);
                        index++;
                    }
				}

				classElement = classElement.ParentClass;
			}

			this.classAttributesListView.ResumeLayout();

			this.addButton.Enabled = false;
		}

		private void ShowSortAttributes()
		{
			ResultAttributeListViewItem item;
			this.sortAttributesListView.SuspendLayout();
			this.sortAttributesListView.Items.Clear();

			bool selected = false;
			foreach (IDataViewElement attribute in _dataView.SortBy.SortAttributes)
			{
				_sortAttributes.Add(attribute);

                item = new ResultAttributeListViewItem(attribute.Caption, attribute);
				item.ImageIndex = 0;
                item.SubItems.Add(attribute.Name);

				if (!selected)
				{
					item.Selected = true;
					selected = true;
				}

				this.sortAttributesListView.Items.Add(item);
			}

			this.sortAttributesListView.ResumeLayout();
		}

		#endregion

		private void ChooseSortAttributeDialog_Load(object sender, System.EventArgs e)
		{
			if (_dataView != null)
			{
                ShowClassesInComboBox();

                // set combox to display the first item
                this.classesComboBox.SelectedIndex = _currentSelectedIndex;

                ShowClassAttributes(_dataView.BaseClass);

				// Show existing result attributes of a data view
				ShowSortAttributes();
			}
		}

        /// <summary>
        /// Display the base class and referenced classes of a data view to
        /// the combo box
        /// </summary>
        private void ShowClassesInComboBox()
        {
            DataClassComboBoxItem item;

            this.classesComboBox.Items.Clear();

            item = new DataClassComboBoxItem(_dataView.BaseClass);
            this.classesComboBox.Items.Add(item);

            foreach (DataClass refClass in _dataView.ReferencedClasses)
            {
                item = new DataClassComboBoxItem(refClass);
                this.classesComboBox.Items.Add(item);
            }
        }

		/// <summary>
		/// Gets the information indicating whether an attribute of the given name
		/// has already existed in the result list.
		/// </summary>
		/// <returns>true if it exists, false otherwise.</returns>
		private bool IsAttributeExist(AttributeElementBase attribute)
		{
			bool status = false;

			foreach (DataViewElementBase attr in _sortAttributes)
			{
				if (attr.Name == attribute.Name)
				{
					status = true;
					break;
				}
			}

			return status;
		}

		private void classAttributesListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.classAttributesListView.SelectedItems.Count > 0)
			{
				this.addButton.Enabled = true;
			}
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			ResultAttributeListViewItem attributeItem = null;

			foreach (AttributeElementListViewItem item in this.classAttributesListView.SelectedItems)
			{
				if (!IsAttributeExist(item.AttributeElement))
				{
                    DataClass currentDataClass = ((DataClassComboBoxItem)this.classesComboBox.SelectedItem).DataClass;

					IDataViewElement sortAttribute = null;

					if (item.AttributeElement is SimpleAttributeElement)
					{
                        sortAttribute = new SortAttribute(item.AttributeElement.Name, currentDataClass.Alias);
                        sortAttribute.Caption = item.AttributeElement.Caption;
                        sortAttribute.Description = item.AttributeElement.Description;

                        attributeItem = new ResultAttributeListViewItem(sortAttribute.Caption, sortAttribute);
                        attributeItem.ImageIndex = 0;
                        attributeItem.SubItems.Add(sortAttribute.Name);
					}

                    if (sortAttribute != null)
					{
                        _sortAttributes.Add(sortAttribute);
                        this.sortAttributesListView.Items.Add(attributeItem);
					}
				}
				else
				{
					string msg = String.Format(MessageResourceManager.GetString("DesignStudio.DuplicateSortAttribute"), item.AttributeElement.Name);
					MessageBox.Show(msg, "Info", MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
		}

		private void sortAttributesListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.sortAttributesListView.SelectedItems.Count == 1)
			{
				int selectedIndex = this.sortAttributesListView.SelectedIndices[0];
				if (selectedIndex > 0)
				{
					this.upButton.Enabled = true;
				}
				else
				{
					this.upButton.Enabled = false;
				}

				if (selectedIndex < this.sortAttributesListView.Items.Count - 1)
				{
					this.downButton.Enabled = true;
				}
				else
				{
					this.downButton.Enabled = false;
				}

				this.delButton.Enabled = true;
			}
			else
			{
				this.upButton.Enabled = false;
				this.downButton.Enabled = false;
				this.delButton.Enabled = false;
			}
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			if (this.sortAttributesListView.SelectedItems.Count == 1)
			{
				ResultAttributeListViewItem item = (ResultAttributeListViewItem) this.sortAttributesListView.SelectedItems[0];

				_sortAttributes.Remove(item.ResultAttribute);

				this.sortAttributesListView.Items.Remove(item);
			}
		}

		private void upButton_Click(object sender, System.EventArgs e)
		{
			if (this.sortAttributesListView.SelectedItems.Count == 1)
			{
				// move the result up a slot
				ResultAttributeListViewItem item = (ResultAttributeListViewItem) this.sortAttributesListView.SelectedItems[0];

				int pos = this.sortAttributesListView.Items.IndexOf(item);

				if (pos > 0)
				{
					this.sortAttributesListView.Items.RemoveAt(pos);
					item.Selected = true;
					this.sortAttributesListView.Items.Insert(pos - 1, item);
					_sortAttributes.RemoveAt(pos);
					_sortAttributes.Insert(pos - 1, item.ResultAttribute);
				}
			}
		}

		private void downButton_Click(object sender, System.EventArgs e)
		{
			if (this.sortAttributesListView.SelectedItems.Count == 1)
			{
				// move the result down a slot
				ResultAttributeListViewItem item = (ResultAttributeListViewItem) this.sortAttributesListView.SelectedItems[0];

				int pos = this.sortAttributesListView.Items.IndexOf(item);

				if (pos < this.sortAttributesListView.Items.Count)
				{
					this.sortAttributesListView.Items.RemoveAt(pos);
					item.Selected = true;
					this.sortAttributesListView.Items.Insert(pos + 1, item);

					_sortAttributes.RemoveAt(pos);
					_sortAttributes.Insert(pos + 1, item.ResultAttribute);
				}
			}		
		}

        private void classesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.classesComboBox.SelectedIndex >= 0)
            {
                DataClassComboBoxItem selectedItem = (DataClassComboBoxItem)this.classesComboBox.SelectedItem;

                _currentSelectedIndex = this.classesComboBox.SelectedIndex;

                ShowClassAttributes(selectedItem.DataClass);
            }
        }
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.WindowsControl;

namespace SmartExcel
{
	/// <summary>
	/// Summary description for ChooseReferencedClassDialog.
	/// </summary>
	public class ChooseReferencedClassDialog : System.Windows.Forms.Form
	{
		private DataViewModel _dataView;
		private ListViewItem _baseClassListViewItem;

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ListView existingClassesListView;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView linkedClassesListView;
		private System.Windows.Forms.ColumnHeader classAliasColumnHeader;
		private System.Windows.Forms.ColumnHeader classCaptionColumnHeader;
		private System.Windows.Forms.ColumnHeader linkedClassCaptionColumnHeader;
		private System.Windows.Forms.ColumnHeader relationshipCaptionColumnHeader;
		private System.Windows.Forms.ColumnHeader relationshipTypeColumnHeader;
		private System.Windows.Forms.ImageList listViewSmallImageList;
		private System.Windows.Forms.Button delButton;
		private System.Windows.Forms.Button addButton;
		private System.ComponentModel.IContainer components;

		public ChooseReferencedClassDialog()
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
		/// Gets or sets the data view being created.
		/// </summary>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseReferencedClassDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.existingClassesListView = new System.Windows.Forms.ListView();
            this.classCaptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.classAliasColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.listViewSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkedClassesListView = new System.Windows.Forms.ListView();
            this.linkedClassCaptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.relationshipCaptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.relationshipTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.delButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
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
            // existingClassesListView
            // 
            resources.ApplyResources(this.existingClassesListView, "existingClassesListView");
            this.existingClassesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.classCaptionColumnHeader,
            this.classAliasColumnHeader});
            this.existingClassesListView.FullRowSelect = true;
            this.existingClassesListView.LargeImageList = this.listViewSmallImageList;
            this.existingClassesListView.MultiSelect = false;
            this.existingClassesListView.Name = "existingClassesListView";
            this.existingClassesListView.SmallImageList = this.listViewSmallImageList;
            this.existingClassesListView.UseCompatibleStateImageBehavior = false;
            this.existingClassesListView.View = System.Windows.Forms.View.Details;
            this.existingClassesListView.SelectedIndexChanged += new System.EventHandler(this.existingClassesListView_SelectedIndexChanged);
            // 
            // classCaptionColumnHeader
            // 
            resources.ApplyResources(this.classCaptionColumnHeader, "classCaptionColumnHeader");
            // 
            // classAliasColumnHeader
            // 
            resources.ApplyResources(this.classAliasColumnHeader, "classAliasColumnHeader");
            // 
            // listViewSmallImageList
            // 
            this.listViewSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewSmallImageList.ImageStream")));
            this.listViewSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewSmallImageList.Images.SetKeyName(0, "");
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
            // linkedClassesListView
            // 
            resources.ApplyResources(this.linkedClassesListView, "linkedClassesListView");
            this.linkedClassesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.linkedClassCaptionColumnHeader,
            this.relationshipCaptionColumnHeader,
            this.relationshipTypeColumnHeader});
            this.linkedClassesListView.FullRowSelect = true;
            this.linkedClassesListView.LargeImageList = this.listViewSmallImageList;
            this.linkedClassesListView.MultiSelect = false;
            this.linkedClassesListView.Name = "linkedClassesListView";
            this.linkedClassesListView.SmallImageList = this.listViewSmallImageList;
            this.linkedClassesListView.UseCompatibleStateImageBehavior = false;
            this.linkedClassesListView.View = System.Windows.Forms.View.Details;
            this.linkedClassesListView.SelectedIndexChanged += new System.EventHandler(this.linkedClassesListView_SelectedIndexChanged);
            // 
            // linkedClassCaptionColumnHeader
            // 
            resources.ApplyResources(this.linkedClassCaptionColumnHeader, "linkedClassCaptionColumnHeader");
            // 
            // relationshipCaptionColumnHeader
            // 
            resources.ApplyResources(this.relationshipCaptionColumnHeader, "relationshipCaptionColumnHeader");
            // 
            // relationshipTypeColumnHeader
            // 
            resources.ApplyResources(this.relationshipTypeColumnHeader, "relationshipTypeColumnHeader");
            // 
            // delButton
            // 
            resources.ApplyResources(this.delButton, "delButton");
            this.delButton.Name = "delButton";
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // ChooseReferencedClassDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.linkedClassesListView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.existingClassesListView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ChooseReferencedClassDialog";
            this.Load += new System.EventHandler(this.ChooseReferencedClassDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		private MetaDataTreeNode FindTreeNode(MetaDataTreeNode root, ClassElement classElement)
		{
			foreach (MetaDataTreeNode node in root.Nodes)
			{
				if (classElement == node.MetaDataElement)
				{
					return node;
				}
				else
				{
					MetaDataTreeNode found = FindTreeNode(node, classElement);
					if (found != null)
					{
						return found;
					}
				}
			}

			return null;
		}

		private void ShowExistingClasses()
		{
			this.existingClassesListView.SuspendLayout();
			this.existingClassesListView.Items.Clear();

			// add an item for base class as first item
			ListViewItem item = new RefClassListViewItem(_dataView.BaseClass.Caption, _dataView.BaseClass);
			item.ImageIndex = 0;
			item.SubItems.Add(_dataView.BaseClass.Alias);

			// select the first item by default
			item.Selected = true;

			_baseClassListViewItem = item;

			this.existingClassesListView.Items.Add(item);

			// Add items for the referenced classes
			foreach (DataClass refClass in _dataView.ReferencedClasses)
			{
				item = new RefClassListViewItem(refClass.Caption, refClass);
				item.ImageIndex = 0;
				item.SubItems.Add(refClass.Alias);

				this.existingClassesListView.Items.Add(item);
			}

			this.existingClassesListView.ResumeLayout();
		}

		#endregion

		private void ChooseReferencedClassDialog_Load(object sender, System.EventArgs e)
		{
			if (_dataView != null && _dataView.BaseClass != null)
			{
				ShowExistingClasses();
			}		
		}

		private void existingClassesListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// display the linked classes of the selected class in the existing class list
			if (this.existingClassesListView.SelectedItems.Count == 1)
			{
				RefClassListViewItem item = (RefClassListViewItem) this.existingClassesListView.SelectedItems[0];

				DataClass selectedClass = item.ReferencedClass;
				if (selectedClass.Type == DataClassType.ReferencedClass)
				{
					this.delButton.Enabled = true;
				}
				else
				{
					this.delButton.Enabled = false;
				}

				ClassElement classElement = (ClassElement) selectedClass.GetSchemaModelElement();

				this.linkedClassesListView.SuspendLayout();
				this.linkedClassesListView.Items.Clear();

				while (classElement != null)
				{
					foreach (RelationshipAttributeElement relationship in classElement.RelationshipAttributes)
					{
						ClassElement linkedClass = relationship.LinkedClass;

                        LinkedClassListViewItem item1 = new LinkedClassListViewItem(linkedClass.Caption, linkedClass, selectedClass, relationship);
						item1.ImageIndex = 0;
						item1.SubItems.Add(relationship.Caption);
						item1.SubItems.Add(Enum.GetName(typeof(RelationshipType), relationship.Type));

						this.linkedClassesListView.Items.Add(item1);
					}

					// go up class hierarchy
					classElement = classElement.ParentClass;
				}

				this.linkedClassesListView.ResumeLayout();
			}
		}

		private void linkedClassesListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.linkedClassesListView.SelectedItems.Count == 1)
			{
				this.addButton.Enabled = true;
			}					
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			if (this.linkedClassesListView.SelectedItems.Count == 1)
			{
				LinkedClassListViewItem listViewItem = (LinkedClassListViewItem) this.linkedClassesListView.SelectedItems[0];
                DataClass referringClass = listViewItem.ReferringClass;
				RelationshipAttributeElement relationship = listViewItem.RelationshipAttribute;

				// check to see if the class has been added as a referenced class
				bool isAdded = false;
				foreach (DataClass refClass in _dataView.ReferencedClasses)
				{
                    if (refClass.ReferringClassAlias == referringClass.Alias &&
                        refClass.ReferringRelationshipName == relationship.Name)
					{
						isAdded = true;
					}
				}

				if (!isAdded)
				{
					// create a referenced class and add it to data view
					// Add the linked class as a referenced class to data view
                    DataClass referencedClass = new DataClass(relationship.LinkedClassAlias,
						relationship.LinkedClassName, DataClassType.ReferencedClass);
                    referencedClass.ReferringClassAlias = referringClass.Alias;
                    referencedClass.ReferringRelationshipName = relationship.Name;
					referencedClass.Caption = relationship.LinkedClass.Caption;
					_dataView.ReferencedClasses.Add(referencedClass);

					// refresh the existing class list view
					ShowExistingClasses();
				}
				else
				{
					MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.ReferenceClassAdded"), "Notify Dialog",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			// remove the selected referenced class
			if (this.existingClassesListView.SelectedItems.Count == 1)
			{
				RefClassListViewItem item = (RefClassListViewItem) this.existingClassesListView.SelectedItems[0];

				DataClass selectedClass = item.ReferencedClass;
				if (selectedClass.Type == DataClassType.ReferencedClass)
				{
					_dataView.ReferencedClasses.Remove(selectedClass);

					// select the base class item in the list
					_baseClassListViewItem.Selected = true;

					// refresh the existing class list view
					ShowExistingClasses();
				}
			}
		}
	}
}

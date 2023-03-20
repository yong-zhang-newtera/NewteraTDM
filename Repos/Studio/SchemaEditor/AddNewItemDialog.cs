using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.Mappings;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for AddNewItemDialog.
	/// </summary>
	public class AddNewItemDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private MetaDataTreeNode _selectedTreeNode = null;
		private NewItemListViewItem _selectedListViewItem = null;
		private IMetaDataElement _metaDataElement = null;

		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ImageList listViewImageList;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ErrorProvider infoProvider;
		private System.Windows.Forms.ToolTip toolTip;
        private Newtera.WindowsControl.EnterTextBox captionTextBox;
		private System.ComponentModel.IContainer components;

		public AddNewItemDialog()
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

		public MetaDataModel MetaDataModel
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
			}
		}

		public MetaDataTreeNode SelectedTreeNode
		{
			get
			{
				return _selectedTreeNode;
			}
			set
			{
				_selectedTreeNode = value;
			}
		}

		public IMetaDataElement NewMetaDataElement
		{
			get
			{
				return _metaDataElement;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddNewItemDialog));
            this.listView1 = new System.Windows.Forms.ListView();
            this.listViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.captionTextBox = new Newtera.WindowsControl.EnterTextBox();
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.HideSelection = false;
            this.listView1.LargeImageList = this.listViewImageList;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.SmallImageList = this.listViewImageList;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // listViewImageList
            // 
            this.listViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewImageList.ImageStream")));
            this.listViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewImageList.Images.SetKeyName(0, "");
            this.listViewImageList.Images.SetKeyName(1, "");
            this.listViewImageList.Images.SetKeyName(2, "");
            this.listViewImageList.Images.SetKeyName(3, "");
            this.listViewImageList.Images.SetKeyName(4, "");
            this.listViewImageList.Images.SetKeyName(5, "");
            this.listViewImageList.Images.SetKeyName(6, "");
            this.listViewImageList.Images.SetKeyName(7, "");
            this.listViewImageList.Images.SetKeyName(8, "");
            this.listViewImageList.Images.SetKeyName(9, "");
            this.listViewImageList.Images.SetKeyName(10, "");
            this.listViewImageList.Images.SetKeyName(11, "");
            this.listViewImageList.Images.SetKeyName(12, "virtualproperty.GIF");
            this.listViewImageList.Images.SetKeyName(13, "imageicon.gif");
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
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            this.toolTip.SetToolTip(this.nameTextBox, resources.GetString("nameTextBox.ToolTip"));
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // captionTextBox
            // 
            resources.ApplyResources(this.captionTextBox, "captionTextBox");
            this.captionTextBox.Name = "captionTextBox";
            this.toolTip.SetToolTip(this.captionTextBox, resources.GetString("captionTextBox.ToolTip"));
            this.captionTextBox.TextChanged += new System.EventHandler(this.captionTextBox_TextChanged);
            this.captionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.captionTextBox_KeyDown);
            this.captionTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.captionTextBox_Validating);
            // 
            // infoProvider
            // 
            this.infoProvider.ContainerControl = this;
            resources.ApplyResources(this.infoProvider, "infoProvider");
            // 
            // AddNewItemDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.captionTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "AddNewItemDialog";
            this.Load += new System.EventHandler(this.AddNewItemDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private IMetaDataElement CreateElement()
		{
			IMetaDataElement element = null;

			switch (_selectedListViewItem.Type)
			{
				case Newtera.WinClientCommon.NodeType.ClassNode:
					element = _metaData.SchemaModel.CreateClass(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.SimpleAttributeNode:
					element = new SimpleAttributeElement(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.RelationshipAttributeNode:
					element = new RelationshipAttributeElement(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.ArrayAttributeNode:
					element = new ArrayAttributeElement(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.VirtualAttributeNode:
                    element = new VirtualAttributeElement(this.nameTextBox.Text);
                    break;
                case Newtera.WinClientCommon.NodeType.ImageAttributeNode:
                    element = new ImageAttributeElement(this.nameTextBox.Text);
                    break;
                case Newtera.WinClientCommon.NodeType.EnumConstraintNode:
					element = new EnumElement(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.RangeConstraintNode:
					element = new RangeElement(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.PatternConstraintNode:
					element = new PatternElement(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.ListConstraintNode:
					element = new ListElement(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.TaxonomyNode:
					element = new TaxonomyModel(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.TaxonNode:
					element = new TaxonNode(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.DataViewNode:
					element = new DataViewModel(this.nameTextBox.Text, _metaData.SchemaInfo, _metaData.SchemaModel);
					break;
                case Newtera.WinClientCommon.NodeType.SelectorNode:
					element = new Selector(this.nameTextBox.Text);
					break;
                case Newtera.WinClientCommon.NodeType.XMLSchemaView:
                    element = new XMLSchemaModel(this.nameTextBox.Text);
                    break;
			}

			if (element != null)
			{
				element.Caption = this.captionTextBox.Text;
			}
			
			return element;
		}

		private bool ValidateNameString(string name)
		{
			Regex regex = new Regex(@"^[a-zA-Z]+[0-9]*[a-zA-Z]*[0-9]*$");

			bool status = regex.IsMatch(name);

			return status;
		}

		private bool IsReservedName(string name)
		{
			bool status = false;

			if (name.ToUpper() == "TYPE" || name.ToUpper() == "OBJ_ID" ||
                name.ToUpper() == "ATTACHMENTS" || name.ToUpper() == "PERMISSION")
			{
				status = true;
			}

			return status;
		}

		/// <summary>
		/// Gets the information indicating whether the given name is used as class name.
		/// </summary>
		/// <param name="ownerClass">The owner class</param>
		/// <param name="name">The name</param>
		/// <returns>true if it is a class name, false otherwise</returns>
		private bool IsClassName(ClassElement ownerClass, string name)
		{
			bool status = false;

			ClassElement theClass = ownerClass;
			while (theClass != null)
			{
				if (theClass.Name == name)
				{
					status = true;
					break;
				}

				theClass = theClass.ParentClass;
			}

			return status;
		}

		private bool ValidateNameUniqueness(string name)
		{
			bool status = true;

			switch (_selectedListViewItem.Type)
			{
                case Newtera.WinClientCommon.NodeType.ClassNode:

					if (_metaData.SchemaModel.FindClass(name) != null)
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateClassName"));
					}
					else if(_selectedTreeNode.MetaDataElement is ClassElement)
					{
						ClassElement parentClass = (ClassElement) _selectedTreeNode.MetaDataElement;
						if (parentClass.FindAttribute(name, SearchDirection.TwoWay) != null)
						{
							status = false;
							this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.IsAttributeName"));
						}
					}

					break;
                case Newtera.WinClientCommon.NodeType.SimpleAttributeNode:
                case Newtera.WinClientCommon.NodeType.RelationshipAttributeNode:
                case Newtera.WinClientCommon.NodeType.ArrayAttributeNode:
                case Newtera.WinClientCommon.NodeType.VirtualAttributeNode:
                case Newtera.WinClientCommon.NodeType.ImageAttributeNode:
					ClassElement ownerClass = (ClassElement) _selectedTreeNode.MetaDataElement;
					if (ownerClass.FindAttribute(name, SearchDirection.TwoWay) != null)
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateAttributeName"));
					}
					else if (IsReservedName(name))
					{
						status = false;
						string msg = String.Format(MessageResourceManager.GetString("SchemaEditor.ReservedWord"), name);
						this.errorProvider.SetError(this.nameTextBox, msg);
					}
					else if (IsClassName(ownerClass, name))
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.NameTakenByClass"));
					}

					break;
                case Newtera.WinClientCommon.NodeType.EnumConstraintNode:
                case Newtera.WinClientCommon.NodeType.RangeConstraintNode:
                case Newtera.WinClientCommon.NodeType.PatternConstraintNode:
                case Newtera.WinClientCommon.NodeType.ListConstraintNode:
					if (_metaData.SchemaModel.FindConstraint(name) != null)
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateConstraintName"));
					}
					break;
                case Newtera.WinClientCommon.NodeType.TaxonomyNode:
					if (_metaData.Taxonomies[name] != null)
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateTaxonomyName"));
					}
					break;
                case Newtera.WinClientCommon.NodeType.TaxonNode:
					TaxonomyModel taxonomy;
					if (_selectedTreeNode.MetaDataElement is TaxonomyModel)
					{
						taxonomy = (TaxonomyModel) _selectedTreeNode.MetaDataElement;
					}
					else
					{
						taxonomy = ((TaxonNode) _selectedTreeNode.MetaDataElement).OwnerTaxonomy;
					}

					// make sure that the name has not been used by any node in the
					// given taxonomy
					if (taxonomy.FindNode(name) != null)
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateTaxonName"));
					}

					break;

                case Newtera.WinClientCommon.NodeType.DataViewNode:
					if (_metaData.DataViews[name] != null)
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateDataViewName"));
					}
					break;
                case Newtera.WinClientCommon.NodeType.SelectorNode:
					if (_metaData.SelectorManager.IsSelectorExist(name))
					{
						status = false;
						this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateSelectorName"));
					}
					break;

                case Newtera.WinClientCommon.NodeType.XMLSchemaView:
                    if (_metaData.XMLSchemaViews[name] != null)
                    {
                        status = false;
                        this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateXMLSchemaViewName"));
                    }
                    break;
			}

			return status;
		}
		
		private void AddNewItemDialog_Load(object sender, System.EventArgs e)
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MetaDataListViewBuilder));

			this.listView1.BeginUpdate();
			this.listView1.Items.Clear();
			
			ListViewItem item;
			if (SelectedTreeNode != null)
			{
				switch (SelectedTreeNode.Type)
				{
                    case Newtera.WinClientCommon.NodeType.ClassesFolder:
                        item = new NewItemListViewItem(resources.GetString("ClassType.Text"), Newtera.WinClientCommon.NodeType.ClassNode);
						item.ImageIndex = 0;
						item.Selected = true;
						this.listView1.Items.Add(item);

						break;

                    case Newtera.WinClientCommon.NodeType.CategoryFolder:
                        item = new NewItemListViewItem(resources.GetString("ClassType.Text"), Newtera.WinClientCommon.NodeType.ClassNode);
                        item.ImageIndex = 0;
                        item.Selected = true;
                        this.listView1.Items.Add(item);

                        break;
                    case Newtera.WinClientCommon.NodeType.ConstraintsFolder:
                        item = new NewItemListViewItem(resources.GetString("EnumType.Text"), Newtera.WinClientCommon.NodeType.EnumConstraintNode);
						item.ImageIndex = 3;
						item.Selected = true;
						this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("RangeType.Text"), Newtera.WinClientCommon.NodeType.RangeConstraintNode);
						item.ImageIndex = 4;
						this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("PatternType.Text"), Newtera.WinClientCommon.NodeType.PatternConstraintNode);
						item.ImageIndex = 5;
						this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("ListType.Text"), Newtera.WinClientCommon.NodeType.ListConstraintNode);
						item.ImageIndex = 10; // add an icon for list constraint
						this.listView1.Items.Add(item);

						break;

                    case Newtera.WinClientCommon.NodeType.TaxonomiesFolder:
                        item = new NewItemListViewItem(resources.GetString("TaxonomyType.Text"), Newtera.WinClientCommon.NodeType.TaxonomyNode);
						item.ImageIndex = 6;
						item.Selected = true;
						this.listView1.Items.Add(item);

						break;

                    case Newtera.WinClientCommon.NodeType.ClassNode:
                        item = new NewItemListViewItem(resources.GetString("SimpleAttributeType.Text"), Newtera.WinClientCommon.NodeType.SimpleAttributeNode);
						item.ImageIndex = 1;
						item.Selected = true;
						this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("RelationshipAttributeType.Text"), Newtera.WinClientCommon.NodeType.RelationshipAttributeNode);
						item.ImageIndex = 2;
						this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("ArrayAttributeType.Text"), Newtera.WinClientCommon.NodeType.ArrayAttributeNode);
						item.ImageIndex = 9;
						this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("VirtualAttributeType.Text"), Newtera.WinClientCommon.NodeType.VirtualAttributeNode);
                        item.ImageIndex = 12;
                        this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("ImageAttributeType.Text"), Newtera.WinClientCommon.NodeType.ImageAttributeNode);
                        item.ImageIndex = 13;
                        this.listView1.Items.Add(item);

                        item = new NewItemListViewItem(resources.GetString("ClassType.Text"), Newtera.WinClientCommon.NodeType.ClassNode);
						item.ImageIndex = 0;
						this.listView1.Items.Add(item);

						break;
                    case Newtera.WinClientCommon.NodeType.TaxonomyNode:
                    case Newtera.WinClientCommon.NodeType.TaxonNode:
                        item = new NewItemListViewItem(resources.GetString("TaxonType.Text"), Newtera.WinClientCommon.NodeType.TaxonNode);
						item.ImageIndex = 7;
						item.Selected = true;
						this.listView1.Items.Add(item);

						// create an unique name for the taxon node by default
						this.nameTextBox.Text = CreateUniqueTaxonName(SelectedTreeNode);

						break;

                    case Newtera.WinClientCommon.NodeType.DataViewsFolder:
                        item = new NewItemListViewItem(resources.GetString("DataViewType.Text"), Newtera.WinClientCommon.NodeType.DataViewNode);
						item.ImageIndex = 8;
						item.Selected = true;
						this.listView1.Items.Add(item);

						break;

                    case Newtera.WinClientCommon.NodeType.SelectorsFolder:
                        item = new NewItemListViewItem(resources.GetString("SelectorType.Text"), Newtera.WinClientCommon.NodeType.SelectorNode);
						item.ImageIndex = 11;
						item.Selected = true;
						this.listView1.Items.Add(item);

						break;

                    case Newtera.WinClientCommon.NodeType.XMLSchemaViewsFolder:
                        item = new NewItemListViewItem(resources.GetString("XMLSchemaView.Text"), Newtera.WinClientCommon.NodeType.XMLSchemaView);
                        item.ImageIndex = 7;
                        item.Selected = true;
                        this.listView1.Items.Add(item);

                        break;
				}
			}

			this.listView1.EndUpdate();

			// display help providers to text boxes
			string tip = toolTip.GetToolTip(this.nameTextBox);
			if (tip.Length > 0)
			{
				infoProvider.SetError(this.nameTextBox, tip);
			}
			tip = toolTip.GetToolTip(this.captionTextBox);
			if (tip.Length > 0)
			{
				infoProvider.SetError(this.captionTextBox, tip);
			}		
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.listView1.SelectedItems.Count == 1)
			{
				this._selectedListViewItem = (NewItemListViewItem) this.listView1.SelectedItems[0];
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// validate the text in nameTextBox and captionTextBox
			this.nameTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			this.captionTextBox.Focus();
			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			this._metaDataElement = CreateElement();
		}

		private void nameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null and has to be unique

                if (this.nameTextBox.Text.Length == 0)
                {
                    this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.EnterName"));
                    this.infoProvider.SetError(this.nameTextBox, null);
                    e.Cancel = true;
                }
                else if (!ValidateNameString(this.nameTextBox.Text))
                {
                    this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidName"));
                    this.infoProvider.SetError(this.nameTextBox, null);
                    e.Cancel = true;
                }
                else if (!ValidateNameUniqueness(this.nameTextBox.Text))
                {
                    this.infoProvider.SetError(this.nameTextBox, null);
                    e.Cancel = true;
                }
                else
                {
                    string tip = this.toolTip.GetToolTip((Control)sender);
                    // show the info when there is text in text box
                    this.errorProvider.SetError((Control)sender, null);
                    this.infoProvider.SetError((Control)sender, tip);
                }
		}

		private void nameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// fill the caption automatically if it has not been modified
			if (((this.captionTextBox.Text.Length + 1) == this.nameTextBox.Text.Length &&
				this.captionTextBox.Text == this.nameTextBox.Text.Substring(0, this.captionTextBox.Text.Length)) ||
				((this.captionTextBox.Text.Length - 1) == this.nameTextBox.Text.Length && 
				this.nameTextBox.Text == this.captionTextBox.Text.Substring(0, this.nameTextBox.Text.Length)))
			{
				this.captionTextBox.Text = this.nameTextBox.Text;
			}
		}

		private void captionTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the caption cannot be null
            if (this.captionTextBox.Text.Length == 0)
            {
                this.errorProvider.SetError(this.captionTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidCaption"));
                this.infoProvider.SetError(this.captionTextBox, null);
                e.Cancel = true;
            }
            else if (this.captionTextBox.Text.Contains("'") || this.captionTextBox.Text.Contains("\"") ||
                this.captionTextBox.Text.Contains("\u2018"))
            {
                this.errorProvider.SetError(this.captionTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidCaptionWithSepcialChars"));
                this.infoProvider.SetError(this.captionTextBox, null);
                e.Cancel = true;
            }
		}

		private void captionTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) 
			{
				// validate the text in nameTextBox and captionTextBox
				this.nameTextBox.Focus();
				if (!this.Validate())
				{
					return;
				}

				this.captionTextBox.Focus();
				if (!this.Validate())
				{
					return;
				}

				this._metaDataElement = CreateElement();

				this.DialogResult = DialogResult.OK;
			}
		}

		/// <summary>
		/// Create an unique name for a child taxon node in a taxonomy tree.
		/// </summary>
		/// <param name="parentNode">The parent node.</param>
		/// <returns>A string representing an unique name.</returns>
		private string CreateUniqueTaxonName(MetaDataTreeNode parentNode)
		{
			string nodeName = "Node";
			string tempName;
			int id;

			switch (parentNode.Type)
			{
                case Newtera.WinClientCommon.NodeType.TaxonomyNode:
					TaxonomyModel taxonomy = (TaxonomyModel) parentNode.MetaDataElement;
					id = taxonomy.ChildrenNodes.Count;

					while (true)
					{
						id++; // increase the id by one
						tempName = nodeName + id;

						// make sure the node name has not been taken
						if (taxonomy.FindNode(tempName) == null)
						{
							// found an unique name
							nodeName = tempName;
							break;
						}
					}

					break;

                case Newtera.WinClientCommon.NodeType.TaxonNode:
					TaxonNode taxon = (TaxonNode) parentNode.MetaDataElement;
					nodeName = taxon.Name;
					id = taxon.ChildrenNodes.Count;

					while (true)
					{
						id++; // increase the id by one
						tempName = nodeName + id;

						// make sure the node name has not been taken
						if (taxon.OwnerTaxonomy.FindNode(tempName) == null)
						{
							// found an unique name
							nodeName = tempName;
							break;
						}
					}

					break;
			}

			return nodeName;
		}

        private void captionTextBox_TextChanged(object sender, EventArgs e)
        {

        }
	}

	/// <summary>
	/// Represent an item in the list view
	/// </summary>
	internal class NewItemListViewItem : ListViewItem
	{
		private Newtera.WinClientCommon.NodeType _nodeType;

        public NewItemListViewItem(string text, Newtera.WinClientCommon.NodeType nodeType)
            : base(text)
		{
			_nodeType = nodeType;
		}

        public Newtera.WinClientCommon.NodeType Type
		{
			get
			{
				return _nodeType;
			}
			set
			{
				_nodeType = value;
			}
		}
	}
}
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.WindowsControl;

namespace Newtera.Studio.UserControls
{
	/// <summary>
	/// Summary description for DataViewResultBuilder.
	/// </summary>
	public class DataViewResultBuilder : System.Windows.Forms.UserControl
	{
		private DataViewModel _dataView;

		private System.Windows.Forms.GroupBox resultGroupBox;
		private System.Windows.Forms.TextBox resultDescriptionTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox resultFunctionTypeComboBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox resultCaptionTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button removeResultButton;
		private System.Windows.Forms.Button addResultsButton;
		private System.Windows.Forms.ListView resultsListView;
		private System.Windows.Forms.ColumnHeader resultCaptionColumnHeader;
		private System.Windows.Forms.ColumnHeader resultNameColumnHeader;
		private System.Windows.Forms.ImageList smallIconImageList;
		private System.Windows.Forms.ErrorProvider errorProvider;
        private ColumnHeader resultClassColumnHeader;
		private System.ComponentModel.IContainer components;

		public event EventHandler ResultAttributesChanged;

		public DataViewResultBuilder()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
		/// Gets or sets the data view model
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

		/// <summary>
		/// Display the result attributes of a dataview
		/// </summary>
		public void DisplayResultAttributes()
		{
			if (_dataView != null)
			{
				ShowResultAttributes();
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataViewResultBuilder));
            this.resultGroupBox = new System.Windows.Forms.GroupBox();
            this.resultDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.resultFunctionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.resultCaptionTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.removeResultButton = new System.Windows.Forms.Button();
            this.addResultsButton = new System.Windows.Forms.Button();
            this.resultsListView = new System.Windows.Forms.ListView();
            this.resultCaptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.resultNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.resultClassColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.smallIconImageList = new System.Windows.Forms.ImageList(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.resultGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // resultGroupBox
            // 
            this.resultGroupBox.AccessibleDescription = null;
            this.resultGroupBox.AccessibleName = null;
            resources.ApplyResources(this.resultGroupBox, "resultGroupBox");
            this.resultGroupBox.BackgroundImage = null;
            this.resultGroupBox.Controls.Add(this.resultDescriptionTextBox);
            this.resultGroupBox.Controls.Add(this.label3);
            this.resultGroupBox.Controls.Add(this.resultFunctionTypeComboBox);
            this.resultGroupBox.Controls.Add(this.label8);
            this.resultGroupBox.Controls.Add(this.resultCaptionTextBox);
            this.resultGroupBox.Controls.Add(this.label2);
            this.resultGroupBox.Controls.Add(this.removeResultButton);
            this.resultGroupBox.Controls.Add(this.addResultsButton);
            this.resultGroupBox.Controls.Add(this.resultsListView);
            this.errorProvider.SetError(this.resultGroupBox, resources.GetString("resultGroupBox.Error"));
            this.resultGroupBox.Font = null;
            this.errorProvider.SetIconAlignment(this.resultGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("resultGroupBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.resultGroupBox, ((int)(resources.GetObject("resultGroupBox.IconPadding"))));
            this.resultGroupBox.Name = "resultGroupBox";
            this.resultGroupBox.TabStop = false;
            // 
            // resultDescriptionTextBox
            // 
            this.resultDescriptionTextBox.AccessibleDescription = null;
            this.resultDescriptionTextBox.AccessibleName = null;
            resources.ApplyResources(this.resultDescriptionTextBox, "resultDescriptionTextBox");
            this.resultDescriptionTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.resultDescriptionTextBox, resources.GetString("resultDescriptionTextBox.Error"));
            this.resultDescriptionTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.resultDescriptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("resultDescriptionTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.resultDescriptionTextBox, ((int)(resources.GetObject("resultDescriptionTextBox.IconPadding"))));
            this.resultDescriptionTextBox.Name = "resultDescriptionTextBox";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.label3.Font = null;
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // resultFunctionTypeComboBox
            // 
            this.resultFunctionTypeComboBox.AccessibleDescription = null;
            this.resultFunctionTypeComboBox.AccessibleName = null;
            resources.ApplyResources(this.resultFunctionTypeComboBox, "resultFunctionTypeComboBox");
            this.resultFunctionTypeComboBox.BackgroundImage = null;
            this.errorProvider.SetError(this.resultFunctionTypeComboBox, resources.GetString("resultFunctionTypeComboBox.Error"));
            this.resultFunctionTypeComboBox.Font = null;
            this.errorProvider.SetIconAlignment(this.resultFunctionTypeComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("resultFunctionTypeComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.resultFunctionTypeComboBox, ((int)(resources.GetObject("resultFunctionTypeComboBox.IconPadding"))));
            this.resultFunctionTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("resultFunctionTypeComboBox.Items"),
            resources.GetString("resultFunctionTypeComboBox.Items1"),
            resources.GetString("resultFunctionTypeComboBox.Items2"),
            resources.GetString("resultFunctionTypeComboBox.Items3"),
            resources.GetString("resultFunctionTypeComboBox.Items4")});
            this.resultFunctionTypeComboBox.Name = "resultFunctionTypeComboBox";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.errorProvider.SetError(this.label8, resources.GetString("label8.Error"));
            this.label8.Font = null;
            this.errorProvider.SetIconAlignment(this.label8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label8.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label8, ((int)(resources.GetObject("label8.IconPadding"))));
            this.label8.Name = "label8";
            // 
            // resultCaptionTextBox
            // 
            this.resultCaptionTextBox.AccessibleDescription = null;
            this.resultCaptionTextBox.AccessibleName = null;
            resources.ApplyResources(this.resultCaptionTextBox, "resultCaptionTextBox");
            this.resultCaptionTextBox.BackgroundImage = null;
            this.errorProvider.SetError(this.resultCaptionTextBox, resources.GetString("resultCaptionTextBox.Error"));
            this.resultCaptionTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.resultCaptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("resultCaptionTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.resultCaptionTextBox, ((int)(resources.GetObject("resultCaptionTextBox.IconPadding"))));
            this.resultCaptionTextBox.Name = "resultCaptionTextBox";
            this.resultCaptionTextBox.TextChanged += new System.EventHandler(this.resultCaptionTextBox_TextChanged);
            this.resultCaptionTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.resultCaptionTextBox_Validating);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.label2.Font = null;
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            // 
            // removeResultButton
            // 
            this.removeResultButton.AccessibleDescription = null;
            this.removeResultButton.AccessibleName = null;
            resources.ApplyResources(this.removeResultButton, "removeResultButton");
            this.removeResultButton.BackgroundImage = null;
            this.errorProvider.SetError(this.removeResultButton, resources.GetString("removeResultButton.Error"));
            this.removeResultButton.Font = null;
            this.errorProvider.SetIconAlignment(this.removeResultButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("removeResultButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.removeResultButton, ((int)(resources.GetObject("removeResultButton.IconPadding"))));
            this.removeResultButton.Name = "removeResultButton";
            this.removeResultButton.Click += new System.EventHandler(this.removeResultButton_Click);
            // 
            // addResultsButton
            // 
            this.addResultsButton.AccessibleDescription = null;
            this.addResultsButton.AccessibleName = null;
            resources.ApplyResources(this.addResultsButton, "addResultsButton");
            this.addResultsButton.BackgroundImage = null;
            this.errorProvider.SetError(this.addResultsButton, resources.GetString("addResultsButton.Error"));
            this.addResultsButton.Font = null;
            this.errorProvider.SetIconAlignment(this.addResultsButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addResultsButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.addResultsButton, ((int)(resources.GetObject("addResultsButton.IconPadding"))));
            this.addResultsButton.Name = "addResultsButton";
            this.addResultsButton.Click += new System.EventHandler(this.addResultsButton_Click);
            // 
            // resultsListView
            // 
            this.resultsListView.AccessibleDescription = null;
            this.resultsListView.AccessibleName = null;
            resources.ApplyResources(this.resultsListView, "resultsListView");
            this.resultsListView.BackgroundImage = null;
            this.resultsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.resultCaptionColumnHeader,
            this.resultNameColumnHeader,
            this.resultClassColumnHeader});
            this.errorProvider.SetError(this.resultsListView, resources.GetString("resultsListView.Error"));
            this.resultsListView.Font = null;
            this.resultsListView.FullRowSelect = true;
            this.errorProvider.SetIconAlignment(this.resultsListView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("resultsListView.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.resultsListView, ((int)(resources.GetObject("resultsListView.IconPadding"))));
            this.resultsListView.LargeImageList = this.smallIconImageList;
            this.resultsListView.MultiSelect = false;
            this.resultsListView.Name = "resultsListView";
            this.resultsListView.SmallImageList = this.smallIconImageList;
            this.resultsListView.UseCompatibleStateImageBehavior = false;
            this.resultsListView.View = System.Windows.Forms.View.Details;
            this.resultsListView.SelectedIndexChanged += new System.EventHandler(this.resultsListView_SelectedIndexChanged);
            // 
            // resultCaptionColumnHeader
            // 
            resources.ApplyResources(this.resultCaptionColumnHeader, "resultCaptionColumnHeader");
            // 
            // resultNameColumnHeader
            // 
            resources.ApplyResources(this.resultNameColumnHeader, "resultNameColumnHeader");
            // 
            // resultClassColumnHeader
            // 
            resources.ApplyResources(this.resultClassColumnHeader, "resultClassColumnHeader");
            // 
            // smallIconImageList
            // 
            this.smallIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallIconImageList.ImageStream")));
            this.smallIconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.smallIconImageList.Images.SetKeyName(0, "");
            this.smallIconImageList.Images.SetKeyName(1, "");
            this.smallIconImageList.Images.SetKeyName(2, "");
            this.smallIconImageList.Images.SetKeyName(3, "");
            this.smallIconImageList.Images.SetKeyName(4, "");
            this.smallIconImageList.Images.SetKeyName(5, "virtualproperty.GIF");
            this.smallIconImageList.Images.SetKeyName(6, "imageicon.gif");
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // DataViewResultBuilder
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.resultGroupBox);
            this.errorProvider.SetError(this, resources.GetString("$this.Error"));
            this.errorProvider.SetIconAlignment(this, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("$this.IconAlignment"))));
            this.errorProvider.SetIconPadding(this, ((int)(resources.GetObject("$this.IconPadding"))));
            this.Name = "DataViewResultBuilder";
            this.resultGroupBox.ResumeLayout(false);
            this.resultGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Fire an event for number of result attributes change
		/// </summary>
		private void FireResultAttributeChangedEvent()
		{
			if (ResultAttributesChanged != null)
			{
				ResultAttributesChanged(this, new EventArgs());
			}
		}

		private void ShowResultAttributes()
		{
            string ownerClassCaption = "";
			ResultAttributeListViewItem item;
			this.resultsListView.SuspendLayout();
			this.resultsListView.Items.Clear();

			bool selected = false;
			foreach (IDataViewElement result in _dataView.ResultAttributes)
			{
                ownerClassCaption = "";
				item = new ResultAttributeListViewItem(result.Caption, result);
				if (result.ElementType == ElementType.SimpleAttribute)
				{
					item.ImageIndex = 1;
                    SimpleAttributeElement simpleAttribute = (SimpleAttributeElement) result.GetSchemaModelElement();
                    if (simpleAttribute != null)
                    {
                        ownerClassCaption = simpleAttribute.OwnerClass.Caption;
                    }
				}
				else if (result.ElementType == ElementType.ArrayAttribute)
				{
					item.ImageIndex = 4;
                    ArrayAttributeElement arrayAttribute = (ArrayAttributeElement) result.GetSchemaModelElement();
                    if (arrayAttribute != null)
                    {
                        ownerClassCaption = arrayAttribute.OwnerClass.Caption;
                    }
				}
                else if (result.ElementType == ElementType.VirtualAttribute)
                {
                    item.ImageIndex = 5;
                    VirtualAttributeElement virtualAttribute = (VirtualAttributeElement) result.GetSchemaModelElement();
                    if (virtualAttribute != null)
                    {
                        ownerClassCaption = virtualAttribute.OwnerClass.Caption;
                    }
                }
                else if (result.ElementType == ElementType.ImageAttribute)
                {
                    item.ImageIndex = 6;
                    ImageAttributeElement imageAttribute = (ImageAttributeElement)result.GetSchemaModelElement();
                    if (imageAttribute != null)
                    {
                        ownerClassCaption = imageAttribute.OwnerClass.Caption;
                    }
                }
				else if (result.ElementType == ElementType.RelationshipAttribute)
				{
					if (((DataRelationshipAttribute) result).Function != null)
					{
						item.ImageIndex = 2;
					}
					else
					{
						item.ImageIndex = 2;
					}

                    RelationshipAttributeElement relationshipAttribute = (RelationshipAttributeElement) result.GetSchemaModelElement();
                    if (relationshipAttribute != null)
                    {
                        ownerClassCaption = relationshipAttribute.OwnerClass.Caption;
                    }
				}
				else
				{
					item.ImageIndex = 1;
				}
				item.SubItems.Add(result.Name);
                item.SubItems.Add(ownerClassCaption);

				if (!selected)
				{
					item.Selected = true;
					selected = true;
				}

				this.resultsListView.Items.Add(item);
			}

			this.resultsListView.ResumeLayout();

			FireResultAttributeChangedEvent();
		}

		private void ClearResultDisplay()
		{
			this.resultDescriptionTextBox.Text = "";
			this.resultCaptionTextBox.Text = "";
			this.resultFunctionTypeComboBox.Text = "";
			this.resultFunctionTypeComboBox.Enabled = false;
		}

        private bool IsReferencedClassExist(DataViewModel dataView, DataClass referencedClass)
        {
            bool status = false;
            foreach (DataClass refClass in dataView.ReferencedClasses)
            {
                if (refClass.Alias == referencedClass.Alias)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

		#endregion

		private void addResultsButton_Click(object sender, System.EventArgs e)
		{
			ChooseResultAttributeDialog dialog = new ChooseResultAttributeDialog();
			dialog.DataView = _dataView;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_dataView.ResultAttributes.Clear();

				foreach (IDataViewElement result in dialog.ResultAttributes)
				{
                    if (result is DataRelationshipAttribute)
                    {
                        // add the class referenced by the relationship as a referenced class to data view
                        DataClass referencedClass = ((DataRelationshipAttribute)result).ReferencedDataClass;
                        if (!IsReferencedClassExist(_dataView, referencedClass))
                        {
                            _dataView.ReferencedClasses.Add(referencedClass);
                        }
                    }

					_dataView.ResultAttributes.Add(result);
				}

				ShowResultAttributes();
			}
		}

		private void removeResultButton_Click(object sender, System.EventArgs e)
		{
			if (this.resultsListView.SelectedItems.Count == 1)
			{
				ResultAttributeListViewItem item = (ResultAttributeListViewItem) this.resultsListView.SelectedItems[0];

				_dataView.ResultAttributes.Remove(item.ResultAttribute);

				this.resultsListView.Items.Remove(item);

				this.removeResultButton.Enabled = false;

                // remove the reference classe that doesn't have any result attributes left
                foreach (DataClass refClass in _dataView.ReferencedClasses)
                {
                    bool found = false;

                    foreach (IDataViewElement result in _dataView.ResultAttributes)
                    {
                        string ownerClassAlias = null;
                        if (result.ElementType == ElementType.SimpleAttribute)
                        {
                            DataSimpleAttribute simpleAttribute = (DataSimpleAttribute)result;
                            ownerClassAlias = simpleAttribute.OwnerClassAlias;
                        }
                        else if (result.ElementType == ElementType.ArrayAttribute)
                        {
                            DataArrayAttribute arrayAttribute = (DataArrayAttribute)result;
                            ownerClassAlias = arrayAttribute.OwnerClassAlias;
                        }
                        else if (result.ElementType == ElementType.VirtualAttribute)
                        {
                            DataVirtualAttribute virtualAttribute = (DataVirtualAttribute)result;
                            ownerClassAlias = virtualAttribute.OwnerClassAlias;
                        }
                        else if (result.ElementType == ElementType.ImageAttribute)
                        {
                            DataImageAttribute imageAttribute = (DataImageAttribute)result;
                            ownerClassAlias = imageAttribute.OwnerClassAlias;
                        }
                        else if (result.ElementType == ElementType.RelationshipAttribute)
                        {
                            DataRelationshipAttribute relationshipAttribute = (DataRelationshipAttribute)result;
                            ownerClassAlias = relationshipAttribute.OwnerClassAlias;
                        }

                        if (ownerClassAlias == refClass.Alias)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        // found a reference class that doesn't have any attributes left
                        // remove it
                        _dataView.ReferencedClasses.Remove(refClass);
                        break;
                    }
                }

				FireResultAttributeChangedEvent();
			}
		}

		private void resultsListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.resultsListView.SelectedItems.Count == 1)
			{
				this.removeResultButton.Enabled = true;
				ResultAttributeListViewItem item = (ResultAttributeListViewItem) this.resultsListView.SelectedItems[0];
				
				this.resultCaptionTextBox.DataBindings.Clear();
				this.resultCaptionTextBox.DataBindings.Add("Text", item.ResultAttribute, "Caption");
				this.resultDescriptionTextBox.DataBindings.Clear();
				this.resultDescriptionTextBox.DataBindings.Add("Text", item.ResultAttribute, "Description");
				if (item.ResultAttribute.ElementType == ElementType.SimpleAttribute ||
					item.ResultAttribute.ElementType == ElementType.ArrayAttribute ||
                    item.ResultAttribute.ElementType == ElementType.VirtualAttribute ||
                    item.ResultAttribute.ElementType == ElementType.ImageAttribute)
				{
					this.resultFunctionTypeComboBox.Text = "";
					this.resultFunctionTypeComboBox.Enabled = false;
				}
				else if (item.ResultAttribute.ElementType == ElementType.RelationshipAttribute)
				{
					RelationshipAttributeElement relationshipElement = (RelationshipAttributeElement) item.ResultAttribute.GetSchemaModelElement();

					// enable the function combo box if the relationship type is one-to-many
					if (relationshipElement.Type == RelationshipType.OneToMany)
					{
						this.resultFunctionTypeComboBox.Enabled = true;
						this.resultFunctionTypeComboBox.DataBindings.Clear();
						this.resultFunctionTypeComboBox.DataBindings.Add("Text", item.ResultAttribute, "Function");
					}
					else
					{
						this.resultFunctionTypeComboBox.Text = "";
						this.resultFunctionTypeComboBox.Enabled = false;
					}
				}
			}
			else
			{
				this.removeResultButton.Enabled = false;
				ClearResultDisplay();
			}
		}

		private void resultCaptionTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the Caption cannot be null and has to be unique
			if (this.resultsListView.SelectedItems.Count == 1 &&
				this.resultCaptionTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.resultCaptionTextBox, "Caption can not be empty.");
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}
		}

		private void resultCaptionTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// change the caption display in the result attributes list view
			if (this.resultsListView.SelectedItems.Count == 1)
			{
				ResultAttributeListViewItem item = (ResultAttributeListViewItem) this.resultsListView.SelectedItems[0];

				item.Text = this.resultCaptionTextBox.Text;
			}		
		}
	}
}

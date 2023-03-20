using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;

namespace Newtera.WindowsControl
{
	/// <summary>
	/// Summary description for ChooseSearchAttributePopup.
	/// </summary>
	public class ChooseSearchAttributePopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private DataViewModel _dataView;
		private IDataViewElement _expression;

		private System.Windows.Forms.ComboBox classesComboBox;
		private System.Windows.Forms.ListView searchAttributesListView;
        private System.Windows.Forms.ImageList listViewImageList;
		private System.ComponentModel.IContainer components;

		public ChooseSearchAttributePopup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_dataView = null;
			_expression = null;
		}

		/// <summary>
		/// Gets the selected search attribute
		/// </summary>
		/// <value>An expression object</value>
		public object Expression
		{
			get
			{
				return _expression;
			}
			set
			{
				_expression = (IDataViewElement) value;
			}
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
			}
		}

		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the
		/// popup relative to the window.
		/// </summary>
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		/// <summary>
		/// Show the popup
		/// </summary>
		public new void Show()
		{
			base.Show();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseSearchAttributePopup));
            this.classesComboBox = new System.Windows.Forms.ComboBox();
            this.searchAttributesListView = new System.Windows.Forms.ListView();
            this.listViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // classesComboBox
            // 
            resources.ApplyResources(this.classesComboBox, "classesComboBox");
            this.classesComboBox.Name = "classesComboBox";
            this.classesComboBox.SelectedIndexChanged += new System.EventHandler(this.classesComboBox_SelectedIndexChanged);
            // 
            // searchAttributesListView
            // 
            resources.ApplyResources(this.searchAttributesListView, "searchAttributesListView");
            this.searchAttributesListView.AutoArrange = false;
            this.searchAttributesListView.FullRowSelect = true;
            this.searchAttributesListView.MultiSelect = false;
            this.searchAttributesListView.Name = "searchAttributesListView";
            this.searchAttributesListView.SmallImageList = this.listViewImageList;
            this.searchAttributesListView.UseCompatibleStateImageBehavior = false;
            this.searchAttributesListView.View = System.Windows.Forms.View.List;
            this.searchAttributesListView.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // listViewImageList
            // 
            this.listViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listViewImageList.ImageStream")));
            this.listViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.listViewImageList.Images.SetKeyName(0, "");
            this.listViewImageList.Images.SetKeyName(1, "");
            this.listViewImageList.Images.SetKeyName(2, "");
            // 
            // ChooseSearchAttributePopup
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.searchAttributesListView);
            this.Controls.Add(this.classesComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChooseSearchAttributePopup";
            this.Deactivate += new System.EventHandler(this.ChooseSearchAttributePopup_Deactivate);
            this.Load += new System.EventHandler(this.ChooseSearchAttributePopup_Load);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Display the attributes of a class that can be used for search
		/// </summary>
		/// <param name="dataClass">The class</param>
		private void ShowClassAttributes(DataClass dataClass)
		{
			AttributeElementListViewItem item;
            this.searchAttributesListView.Items.Clear();
			this.searchAttributesListView.SuspendLayout();
			
			//this.searchAttributesListView.View = System.Windows.Forms.View.SmallIcon;

			ClassElement classElement = (ClassElement) dataClass.GetSchemaModelElement();

			// display order of attributes of class hierarchy from top-down order
			// only simple attributes and relationship attributes of certain types
            // are allowed in search
			while (classElement != null)
			{
				int index = 0;
				foreach (SimpleAttributeElement att in classElement.SimpleAttributes)
				{
                    if (!att.IsHistoryEdit)
                    {
                        item = new AttributeElementListViewItem(att.Caption, att);
                        item.ImageIndex = 0;

                        this.searchAttributesListView.Items.Insert(index, item);
                        index++;
                    }
				}

                foreach (RelationshipAttributeElement relationship in classElement.RelationshipAttributes)
                {
                    if (relationship.IsForeignKeyRequired)
                    {
                        item = new AttributeElementListViewItem(relationship.Caption, relationship);
                        item.ImageIndex = 1;

                        this.searchAttributesListView.Items.Insert(index, item);
                        index++;
                    }
                }

                foreach (ArrayAttributeElement att in classElement.ArrayAttributes)
                {
                    item = new AttributeElementListViewItem(att.Caption, att);
                    item.ImageIndex = 2;

                    this.searchAttributesListView.Items.Insert(index, item);
                    index++;
                }

				classElement = classElement.ParentClass;
			}

			this.searchAttributesListView.ResumeLayout();
		}

        /// <summary>
        /// Display the functions available for using in a condition
        /// </summary>
        private void ShowFunctions()
        {
            FunctionListViewItem item;
            IFunctionElement functionElement;
            this.searchAttributesListView.Items.Clear();
            this.searchAttributesListView.SuspendLayout();

            // display functions available for use in a condition expression
            int index = 0;
            foreach (string name in FunctionFactory.CONDITION_FUNCTIONS)
            {
                functionElement = FunctionFactory.Instance.Create(name);
                item = new FunctionListViewItem(name, functionElement);
                item.ImageIndex = 2;

                this.searchAttributesListView.Items.Insert(index, item);
                index++;
            }

            this.searchAttributesListView.ResumeLayout();
        }

		/// <summary>
		/// Display the base class and referenced classes of a data view to
		/// the combo box
		/// </summary>
		private void ShowClassesInComboBox()
		{
			DataClassComboBoxItem item;

			this.classesComboBox.Items.Clear();

            // add base class
			item = new DataClassComboBoxItem(_dataView.BaseClass);
			this.classesComboBox.Items.Add(item);

            // add referenced classes
			foreach (DataClass refClass in _dataView.ReferencedClasses)
			{
				item = new DataClassComboBoxItem(refClass);
				this.classesComboBox.Items.Add(item);
			}

            // add function item
            this.classesComboBox.Items.Add(new ConditionalFunctionComboBoxItem());
		}

		#endregion

		private void ChooseSearchAttributePopup_Load(object sender, System.EventArgs e)
		{
			if (_dataView != null)
			{
				ShowClassesInComboBox();

				// set combox to display the first item
				this.classesComboBox.SelectedIndex = 0;
			}
		}

		private void classesComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DataClassComboBoxItem dataClassItem = this.classesComboBox.SelectedItem as DataClassComboBoxItem;
            ConditionalFunctionComboBoxItem functionItem = this.classesComboBox.SelectedItem as ConditionalFunctionComboBoxItem;

            if (dataClassItem != null)
            {
                ShowClassAttributes(dataClassItem.DataClass);
            }
            else if (functionItem != null)
            {
                ShowFunctions();
            }
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.searchAttributesListView.SelectedItems.Count == 1)
			{
                AttributeElementListViewItem attributeListItem = this.searchAttributesListView.SelectedItems[0] as AttributeElementListViewItem;
                FunctionListViewItem functionListItem = this.searchAttributesListView.SelectedItems[0] as FunctionListViewItem;

                if (attributeListItem != null)
                {
                    // selected item represents a simple attribute or relationship
                    DataClass currentDataClass = ((DataClassComboBoxItem)this.classesComboBox.SelectedItem).DataClass;

                    if (attributeListItem.AttributeElement is SimpleAttributeElement)
                    {
                        _expression = new DataSimpleAttribute(attributeListItem.AttributeElement.Name, currentDataClass.Alias);
                    }
                    else if (attributeListItem.AttributeElement is ArrayAttributeElement)
                    {
                        _expression = new DataArrayAttribute(attributeListItem.AttributeElement.Name, currentDataClass.Alias);
                    }
                    else
                    {
                        // it is a relationship attribute
                        RelationshipAttributeElement relationElement = (RelationshipAttributeElement)attributeListItem.AttributeElement;
                        _expression = new DataRelationshipAttribute(relationElement.Name, currentDataClass.Alias,
                            relationElement.LinkedClassName);
                    }
                    _expression.DataView = _dataView;
                    _expression.Caption = attributeListItem.AttributeElement.Caption;
                    _expression.Description = attributeListItem.AttributeElement.Description;

                    // fire the accept event
                    if (Accept != null)
                    {
                        Accept(this, EventArgs.Empty);
                    }

                    this.Close();
                }
                else if (functionListItem != null)
                {
                    // selected item represents a function
                    _expression = (IDataViewElement)functionListItem.FunctionElement;

                    // fire the accept event
                    if (Accept != null)
                    {
                        Accept(this, EventArgs.Empty);
                    }

                    this.Close();
                }
			}		
		}

		private void ChooseSearchAttributePopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}

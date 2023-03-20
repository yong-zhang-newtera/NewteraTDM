using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.WindowsControl;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// CreateClassDialog 的摘要说明。
	/// </summary>
	public class CreateClassDialog : System.Windows.Forms.Form
	{
		private DataTable _srcDataTable;
		private MetaDataModel _metaData;
		private Control[] Editors;
		private ClassElement _parentClass;
		private SchemaModelElementCollection _attributes;
		private int _colIndex;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
        private Newtera.WindowsControl.ListViewEx attributeListViewEx;
		private System.Windows.Forms.ColumnHeader includedColumnHeader;
		private System.Windows.Forms.ColumnHeader nameColumnHeader;
		private System.Windows.Forms.ColumnHeader captionColumnHeader;
		private System.Windows.Forms.ColumnHeader sampleColumnHeader;
		private System.Windows.Forms.ColumnHeader attributeTypeColumnHeader;
		private System.Windows.Forms.ColumnHeader valueTypeColumnHeader;
		private System.Windows.Forms.ColumnHeader settingColumnHeader;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ErrorProvider infoProvider;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.TextBox captionTextBox;
		private System.Windows.Forms.TextBox attrNameTextBox;
		private System.Windows.Forms.TextBox attrCaptionTextBox;
		private System.Windows.Forms.ComboBox attrTypeComboBox;
		private System.Windows.Forms.ComboBox valueTypeComboBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox parentClassTextBox;
		private System.Windows.Forms.Button inferDataTypeButton;
		private System.Windows.Forms.Button uncheckAllButton;
		private System.Windows.Forms.Button checkAllButton;
		private System.ComponentModel.IContainer components;

		public CreateClassDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_srcDataTable = null;
			_metaData = null;
			_parentClass = null;
			_attributes = new SchemaModelElementCollection();
			_colIndex = 0;
		}

		/// <summary>
		/// Gets or sets the DataTable object that contains data imported from an
		/// source file.
		/// </summary>
		public DataTable SourceDataTable
		{
			get
			{
				return _srcDataTable;
			}
			set
			{
				_srcDataTable = value;
			}
		}

		/// <summary>
		/// Gets or sets the meta data model from which to get classes
		/// </summary>
		public MetaDataModel MetaData
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

		/// <summary>
		/// Gets or sets the parent class of the created subclass.
		/// </summary>
		public ClassElement ParentClass
		{
			get
			{
				return _parentClass;
			}
			set
			{
				_parentClass = value;
			}
		}

		/// <summary>
		/// Gets the class name
		/// </summary>
		public string ClassName
		{
			get
			{
				return this.nameTextBox.Text.Trim();
			}
		}

		/// <summary>
		/// Gets the class caption
		/// </summary>
		public string ClassCaption
		{
			get
			{
				return this.captionTextBox.Text.Trim();
			}
		}

		/// <summary>
		/// Gets the checked attributes for the new class
		/// </summary>
		public SchemaModelElementCollection Attributes
		{
			get
			{
				SchemaModelElementCollection attributes = new SchemaModelElementCollection();
				for (int i = 0; i < this._attributes.Count; i++)
				{
					if (this.attributeListViewEx.Items[i].Checked)
					{
						attributes.Add(this._attributes[i]);
					}
				}

				return attributes;
			}
		}

		/// <summary>
		/// 清理所有正在使用的资源。
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateClassDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.captionTextBox = new System.Windows.Forms.TextBox();
            this.attributeListViewEx = new Newtera.WindowsControl.ListViewEx();
            this.includedColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.captionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.sampleColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.attributeTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.valueTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.settingColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.attrNameTextBox = new System.Windows.Forms.TextBox();
            this.attrCaptionTextBox = new System.Windows.Forms.TextBox();
            this.attrTypeComboBox = new System.Windows.Forms.ComboBox();
            this.valueTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.parentClassTextBox = new System.Windows.Forms.TextBox();
            this.inferDataTypeButton = new System.Windows.Forms.Button();
            this.uncheckAllButton = new System.Windows.Forms.Button();
            this.checkAllButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            this.toolTip.SetToolTip(this.nameTextBox, resources.GetString("nameTextBox.ToolTip"));
            this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // captionTextBox
            // 
            resources.ApplyResources(this.captionTextBox, "captionTextBox");
            this.captionTextBox.Name = "captionTextBox";
            this.toolTip.SetToolTip(this.captionTextBox, resources.GetString("captionTextBox.ToolTip"));
            this.captionTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.captionTextBox_Validating);
            // 
            // attributeListViewEx
            // 
            this.attributeListViewEx.AllowColumnReorder = true;
            resources.ApplyResources(this.attributeListViewEx, "attributeListViewEx");
            this.attributeListViewEx.CheckBoxes = true;
            this.attributeListViewEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.includedColumnHeader,
            this.nameColumnHeader,
            this.captionColumnHeader,
            this.sampleColumnHeader,
            this.attributeTypeColumnHeader,
            this.valueTypeColumnHeader,
            this.settingColumnHeader});
            this.attributeListViewEx.DoubleClickActivation = false;
            this.attributeListViewEx.FullRowSelect = true;
            this.attributeListViewEx.GridLines = true;
            this.attributeListViewEx.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.attributeListViewEx.Name = "attributeListViewEx";
            this.attributeListViewEx.UseCompatibleStateImageBehavior = false;
            this.attributeListViewEx.View = System.Windows.Forms.View.Details;
            this.attributeListViewEx.SubItemClicked += new Newtera.WindowsControl.SubItemEventHandler(this.attributeListViewEx_SubItemClicked);
            this.attributeListViewEx.SubItemEndEditing += new Newtera.WindowsControl.SubItemEndEditingEventHandler(this.attributeListViewEx_SubItemEndEditing);
            // 
            // includedColumnHeader
            // 
            resources.ApplyResources(this.includedColumnHeader, "includedColumnHeader");
            // 
            // nameColumnHeader
            // 
            resources.ApplyResources(this.nameColumnHeader, "nameColumnHeader");
            // 
            // captionColumnHeader
            // 
            resources.ApplyResources(this.captionColumnHeader, "captionColumnHeader");
            // 
            // sampleColumnHeader
            // 
            resources.ApplyResources(this.sampleColumnHeader, "sampleColumnHeader");
            // 
            // attributeTypeColumnHeader
            // 
            resources.ApplyResources(this.attributeTypeColumnHeader, "attributeTypeColumnHeader");
            // 
            // valueTypeColumnHeader
            // 
            resources.ApplyResources(this.valueTypeColumnHeader, "valueTypeColumnHeader");
            // 
            // settingColumnHeader
            // 
            resources.ApplyResources(this.settingColumnHeader, "settingColumnHeader");
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // attrNameTextBox
            // 
            resources.ApplyResources(this.attrNameTextBox, "attrNameTextBox");
            this.attrNameTextBox.Name = "attrNameTextBox";
            // 
            // attrCaptionTextBox
            // 
            resources.ApplyResources(this.attrCaptionTextBox, "attrCaptionTextBox");
            this.attrCaptionTextBox.Name = "attrCaptionTextBox";
            // 
            // attrTypeComboBox
            // 
            resources.ApplyResources(this.attrTypeComboBox, "attrTypeComboBox");
            this.attrTypeComboBox.Name = "attrTypeComboBox";
            // 
            // valueTypeComboBox
            // 
            resources.ApplyResources(this.valueTypeComboBox, "valueTypeComboBox");
            this.valueTypeComboBox.Name = "valueTypeComboBox";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // parentClassTextBox
            // 
            resources.ApplyResources(this.parentClassTextBox, "parentClassTextBox");
            this.parentClassTextBox.Name = "parentClassTextBox";
            this.parentClassTextBox.ReadOnly = true;
            // 
            // inferDataTypeButton
            // 
            resources.ApplyResources(this.inferDataTypeButton, "inferDataTypeButton");
            this.inferDataTypeButton.Name = "inferDataTypeButton";
            this.inferDataTypeButton.Click += new System.EventHandler(this.inferDataTypeButton_Click);
            // 
            // uncheckAllButton
            // 
            resources.ApplyResources(this.uncheckAllButton, "uncheckAllButton");
            this.uncheckAllButton.Name = "uncheckAllButton";
            this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
            // 
            // checkAllButton
            // 
            resources.ApplyResources(this.checkAllButton, "checkAllButton");
            this.checkAllButton.Name = "checkAllButton";
            this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
            // 
            // infoProvider
            // 
            this.infoProvider.ContainerControl = this;
            resources.ApplyResources(this.infoProvider, "infoProvider");
            // 
            // CreateClassDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkAllButton);
            this.Controls.Add(this.uncheckAllButton);
            this.Controls.Add(this.inferDataTypeButton);
            this.Controls.Add(this.parentClassTextBox);
            this.Controls.Add(this.attrCaptionTextBox);
            this.Controls.Add(this.attrNameTextBox);
            this.Controls.Add(this.captionTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.valueTypeComboBox);
            this.Controls.Add(this.attrTypeComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.attributeListViewEx);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CreateClassDialog";
            this.Load += new System.EventHandler(this.CreateClassDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region controller

		private bool ValidateNameString(string name)
		{
			// name must be consists of english letter, followed by digits
			Regex regex = new Regex(@"^[a-zA-Z]+[0-9]*[a-zA-Z]*[0-9]*$");

			bool status = regex.IsMatch(name);

			return status;
		}

		private bool ValidateNameUniqueness(string name)
		{
			bool status = true;

			if (_metaData.SchemaModel.FindClass(name) != null)
			{
				status = false;
				this.errorProvider.SetError(this.nameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateClassName"));
			}
					
			return status;
		}

		private string GetUniqueAttributeName()
		{
			string attrName = "Col" + _colIndex++;

			while (true)
			{
				// search the parent class to see if the name already exists.
				if (this.ParentClass != null && 
					this.ParentClass.FindAttribute(attrName, SearchDirection.TwoWay) != null)
				{
					// found a dupliacte name, get a new name by increase the index
					attrName = "Col" + _colIndex++;
				}
				else
				{
					break;
				}
			}

			return attrName;
		}

		private bool ValidateAttributes()
		{
			bool status = true;
			string error = null;

			int totalAttributes = 0;

			// check for validity and uniqueness of attribute names first
			for (int i = 0; i < this._attributes.Count; i++)
			{
				AttributeElementBase attribute = (AttributeElementBase) this._attributes[i];

				if (!this.attributeListViewEx.Items[i].Checked)
				{
					continue; // ignore the unchecked attributes
				}
				else
				{
					totalAttributes++;
				}

				if (attribute.Name == null || attribute.Name.Length == 0)
				{
					status = false;
					error = MessageResourceManager.GetString("SchemaEditor.EnterName");
				}

				// check the validities of attribue names
				if (!ValidateNameString(attribute.Name))
				{
					status = false;
					error = attribute.Name + " : " + MessageResourceManager.GetString("SchemaEditor.InvalidName");
				}

				// check uniqueness in the parent class first
				if (this.ParentClass != null && 
					this.ParentClass.FindAttribute(attribute.Name, SearchDirection.TwoWay) != null)
				{
					status = false;
					error = attribute.Name + " : " + MessageResourceManager.GetString("SchemaEditor.DulicateAttributeName");
				}

				// check the uniqueness within the same class
				for (int j = 0; j < this._attributes.Count; j++)
				{
					if (i != j)
					{
						AttributeElementBase attr = (AttributeElementBase) this._attributes[j];
						if (attr.Name == attribute.Name)
						{
							status = false;
							error = attribute.Name + " : " + MessageResourceManager.GetString("SchemaEditor.DulicateAttributeName");
							break;
						}
					}
				}

				// check for the captions of attributes
				if (attribute.Caption == null || attribute.Caption.Length == 0)
				{
					status = false;
					error = MessageResourceManager.GetString("SchemaEditor.InvalidCaption");
				}

				// check for the data type
				if (attribute is SimpleAttributeElement)
				{
					if (((SimpleAttributeElement) attribute).DataType == DataType.Unknown)
					{
						status = false;
						error = attribute.Name + " " + MessageResourceManager.GetString("Import.UnknownDataType");
					}
				}
				else if (attribute is ArrayAttributeElement)
				{
					if (((ArrayAttributeElement) attribute).ElementDataType == DataType.Unknown)
					{
						status = false;
						error = attribute.Name + " " + MessageResourceManager.GetString("Import.UnknownDataType");
					}
				}

				if (!status)
				{
					break;
				}
			}

			if (!status)
			{
				MessageBox.Show(error, "Error Dialog",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			else if (totalAttributes > NewteraNameSpace.ATTRIBUTES_PER_CLASS)
			{
				// exceeding maximum number of attribute per class
				status = false;

				MessageBox.Show(MessageResourceManager.GetString("Import.AttributesOverflow") + NewteraNameSpace.ATTRIBUTES_PER_CLASS, "Error Dialog",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}

			return status;
		}

		private int GetAttributeIndex(ListViewItem theItem)
		{
			int index = 0;

			foreach (ListViewItem item in this.attributeListViewEx.Items)
			{
				if (item == theItem)
				{
					break;
				}
				else
				{
					index ++;
				}
			}

			return index;
		}

		private void ChangeAttributeName(int attributeIndex, string name)
		{
			if (attributeIndex < this._attributes.Count)
			{
				AttributeElementBase attribute = (AttributeElementBase) this._attributes[attributeIndex];
				
				if (attribute.Name != name)
				{
					attribute.Name = name;
				}
			}
		}

		private void ChangeAttributeCaption(int attributeIndex, string caption)
		{
			if (attributeIndex < this._attributes.Count)
			{
				AttributeElementBase attribute = (AttributeElementBase) this._attributes[attributeIndex];
				
				attribute.Caption = caption;
			}
		}

		private void ChangeAttributeType(int attributeIndex, string typeName)
		{
			if (attributeIndex < this._attributes.Count)
			{
				AttributeElementBase attribute = (AttributeElementBase) this._attributes[attributeIndex];
				
				AttributeType attributeType = (AttributeType) Enum.Parse(typeof(AttributeType), typeName);
				
				switch (attributeType)
				{
					case AttributeType.Simple:
						if (attribute is ArrayAttributeElement)
						{
							// change the attribute from array to simple
							ArrayAttributeElement arrayAttribute = (ArrayAttributeElement) attribute;

							SimpleAttributeElement simpleAttribute = new SimpleAttributeElement(arrayAttribute.Name);
							simpleAttribute.Caption = arrayAttribute.Caption;
							simpleAttribute.DataType = arrayAttribute.ElementDataType;

							// replace the array attribute in the list
							this._attributes.Insert(attributeIndex, simpleAttribute);
							this._attributes.RemoveAt(attributeIndex + 1);
						}

						break;
					case AttributeType.Array:
						if (attribute is SimpleAttributeElement)
						{
							SpecifyArraySizeDialog dialog = new SpecifyArraySizeDialog();
							ArraySizeType arraySize = ArraySizeType.NormalSize;
							if (dialog.ShowDialog() == DialogResult.OK)
							{
								arraySize = dialog.ArraySize;
							}

							// change the attribute from simple to array
							SimpleAttributeElement simpleAttribute = (SimpleAttributeElement) attribute;

							ArrayAttributeElement arrayAttribute = new ArrayAttributeElement(simpleAttribute.Name);
							arrayAttribute.Caption = simpleAttribute.Caption;
							arrayAttribute.ElementDataType = simpleAttribute.DataType;
							arrayAttribute.ArraySize = arraySize;

							// replace the simple attribute in the list
							this._attributes.Insert(attributeIndex, arrayAttribute);
							this._attributes.RemoveAt(attributeIndex + 1);
						}

						break;
				}
			}
		}

		private void ChangeAttributeDataType(int attributeIndex, string dataTypeName)
		{
			if (attributeIndex < this._attributes.Count)
			{
				AttributeElementBase attribute = (AttributeElementBase) this._attributes[attributeIndex];
				
				DataType dataType = (DataType) Enum.Parse(typeof(DataType), dataTypeName);
				if (attribute is SimpleAttributeElement)
				{
					((SimpleAttributeElement) attribute).DataType = dataType;
				}
				else if (attribute is ArrayAttributeElement)
				{
					((ArrayAttributeElement) attribute).ElementDataType = dataType;
				}
			}
		}

		/// <summary>
		/// Infer the column data type using the column values
		/// </summary>
		/// <param name="colIndex">The column index</param>
		/// <returns>One of the DataType enum values.</returns>
		private DataType InferDataType(int colIndex)
		{
			DataType dataType = DataType.Unknown;
			int typePriority = 0;

			string dataVal;

			for (int i = 0; i < _srcDataTable.Rows.Count; i++)
			{
				dataVal = _srcDataTable.Rows[i][colIndex].ToString();

				if (dataVal != null)
				{
					// try to match with int32
					try
					{
						int val = Int32.Parse(dataVal);
						if (typePriority <= 0)
						{
							dataType = DataType.Integer;
							typePriority = 0;
						}
						continue;
					}
					catch (Exception)
					{
					}

					try
					{
						// try to match with int64
						long val = Int64.Parse(dataVal);
						if (typePriority < 1)
						{
							dataType = DataType.BigInteger;
							typePriority = 1;
						}
						continue;
					}
					catch (Exception)
					{
					}

					try
					{
						// try to match with float
						float val = Single.Parse(dataVal);
						if (typePriority < 2)
						{
							dataType = DataType.Float;
							typePriority = 2;
						}
						
						continue;
					}
					catch (Exception)
					{
					}

					try
					{
						// try to match with double
						double val = Double.Parse(dataVal);
						if (typePriority < 3)
						{
							dataType = DataType.Double;
							typePriority = 3;
						}

						continue;
					}
					catch (Exception)
					{
					}

					// final resort
					dataType = DataType.String;

					// String is the most general type,
					// no need to check other values.
					break;
				}
			}

			return dataType;
		}

		#endregion

		private void CreateClassDialog_Load(object sender, System.EventArgs e)
		{
			if (_srcDataTable != null && _srcDataTable.Columns.Count > 0 &&
				_srcDataTable.Rows.Count > 0)
			{
				ListViewItem listViewItem;
				string attrName;
				string dataVal;
				SimpleAttributeElement attributeElement;
				inferDataTypeButton.Enabled = true;

				// create a list view item for each column in the data table
				for (int i = 0; i < _srcDataTable.Columns.Count; i++)
				{
					// create an unique attribute name
					attrName = GetUniqueAttributeName();

					listViewItem = new ListViewItem();
					listViewItem.Checked = true; // selected by default
					listViewItem.SubItems.Add(attrName); // Attribute Name
					listViewItem.SubItems.Add(_srcDataTable.Columns[i].ColumnName); // Attribute Caption
					dataVal = _srcDataTable.Rows[0][i].ToString();
					if (dataVal.Length > 200)
					{
						dataVal = dataVal.Substring(0, 200);
					}
					listViewItem.SubItems.Add(dataVal); // sample data
					listViewItem.SubItems.Add("Simple");
					listViewItem.SubItems.Add("Unknown");
					listViewItem.SubItems.Add("...");

					attributeListViewEx.Items.Add(listViewItem);

					// create a SimpleAttributeElement initially
					attributeElement = new SimpleAttributeElement(attrName);
					attributeElement.Caption = _srcDataTable.Columns[i].ColumnName;
					attributeElement.Usage = DefaultViewUsage.Excluded;
					this._attributes.Add(attributeElement);

					Editors = new Control[] {
												null,					// for column 0
												attrNameTextBox,		// for column 1
												attrCaptionTextBox,		// for column 2
												null,					// for column 3
												attrTypeComboBox,		// for column 4
												valueTypeComboBox,		// for column 5
												null					// for column 6
											};
			
					// Immediately accept the new value once the value of the control has changed
					// (for example, the comboBox)
					attrTypeComboBox.SelectedIndexChanged += new EventHandler(control_SelectedValueChanged);
					valueTypeComboBox.SelectedIndexChanged += new EventHandler(control_SelectedValueChanged);
				}
			}

			// build attribute type enum
			string[] enums = Enum.GetNames(typeof(AttributeType));
			for (int j = 0; j < enums.Length; j++)
			{
				attrTypeComboBox.Items.Add(enums[j]);
			}

			// build value type enum strings
			enums = Enum.GetNames(typeof(DataType));
			for (int j = 0; j < enums.Length; j++)
			{
				valueTypeComboBox.Items.Add(enums[j]);
			}

			// show the parent class
			if (this.ParentClass != null)
			{
				this.parentClassTextBox.Text = this.ParentClass.Caption;
			}

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


		private void control_SelectedValueChanged(object sender, System.EventArgs e)
		{
			attributeListViewEx.EndEditing(true);
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
				string tip = this.toolTip.GetToolTip((Control) sender);
				// show the info when there is text in text box
				this.errorProvider.SetError((Control) sender, null);
				this.infoProvider.SetError((Control) sender, tip);
			}
		}

        private void captionTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(this.captionTextBox.Text))
            {
                this.errorProvider.SetError(this.captionTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidCaption"));
                this.infoProvider.SetError(this.captionTextBox, null);
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

			// validate the settings of attributes
			if (!ValidateAttributes())
			{
				this.DialogResult = DialogResult.None;
				return;
			}
		}

        private void attributeListViewEx_SubItemClicked(object sender, Newtera.WindowsControl.SubItemEventArgs e)
		{
			if (e.SubItem == 1 ||
				e.SubItem == 2 ||
				e.SubItem == 4 ||
				e.SubItem == 5)
			{
				attributeListViewEx.StartEditing(Editors[e.SubItem], e.Item, e.SubItem);
			}
			else if (e.SubItem == 6)
			{
				int index = GetAttributeIndex(e.Item);

				if (index < this._attributes.Count)
				{
					AttributePropertyDialog dialog = new AttributePropertyDialog();
					dialog.AttributeElement = ((AttributeElementBase) this._attributes[index]);
					
					dialog.ShowDialog();

					// update the listview display text
					if (dialog.AttributeElement.Caption != e.Item.SubItems[2].Text)
					{
						e.Item.SubItems[2].Text = dialog.AttributeElement.Caption;
					}

					string dataTypeStr = null;
					if (dialog.AttributeElement is SimpleAttributeElement)
					{
						dataTypeStr = Enum.GetName(typeof(DataType),
							dialog.AttributeElement.DataType);
					}
					else if (dialog.AttributeElement is ArrayAttributeElement)
					{
						dataTypeStr = Enum.GetName(typeof(DataType),
							((ArrayAttributeElement) dialog.AttributeElement).ElementDataType);
					}
					if (dataTypeStr != null &&
						e.Item.SubItems[5].Text != dataTypeStr)
					{
						e.Item.SubItems[5].Text = dataTypeStr;
					}
				}
			}
		}

        private void attributeListViewEx_SubItemEndEditing(object sender, Newtera.WindowsControl.SubItemEndEditingEventArgs e)
		{
			int index = GetAttributeIndex(e.Item);

			string text = e.DisplayText;
			switch (e.SubItem)
			{
				case 1:
					
					ChangeAttributeName(index, text);
					break;

				case 2:
					ChangeAttributeCaption(index, text);
					break;

				case 4:
					ChangeAttributeType(index, text);
					break;

				case 5:
					ChangeAttributeDataType(index, text);
					break;
			}
		}

		private void inferDataTypeButton_Click(object sender, System.EventArgs e)
		{
			string dataTypeStr;
			DataType dataType = DataType.Unknown;

			// Infer attribute data type from the data value
			for (int i = 0; i < _srcDataTable.Columns.Count; i++)
			{
				if (_attributes[i] is SimpleAttributeElement)
				{
					dataType = InferDataType(i);

					((SimpleAttributeElement) _attributes[i]).DataType = dataType;
				}
				else if (_attributes[i] is ArrayAttributeElement)
				{
					// choose String Type for an array by default
					dataType = DataType.String;

					((ArrayAttributeElement) _attributes[i]).ElementDataType = dataType;
				}

				dataTypeStr = Enum.GetName(typeof(DataType), dataType);

				if (dataTypeStr != null && attributeListViewEx.Items[i].SubItems[5].Text != dataTypeStr)
				{
					attributeListViewEx.Items[i].SubItems[5].Text = dataTypeStr;
				}
			}
		}

		private void uncheckAllButton_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < this._attributes.Count; i++)
			{
				this.attributeListViewEx.Items[i].Checked = false;
			}
		}

		private void checkAllButton_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < this._attributes.Count; i++)
			{
				this.attributeListViewEx.Items[i].Checked = true;
			}
		}
	}

	internal enum AttributeType
	{
		Simple,
		Array
	}
}

/*
* @(#)TableStyleFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
    using System.Collections;
	using System.Drawing;
	using System.Windows.Forms;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A factory that create a DataGridTableStyle instance based on a data view.
	/// </summary>
	/// <version>  1.0.1 05 Oct 2003</version>
	/// <author>  Yong Zhang</author>
	public class TableStyleFactory
	{
		private const int MaxColumnWidth = 500;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static TableStyleFactory theFactory;

        private IImageGetterFactory _imageGetterFactory;
        private bool _hasInlineEditColumns = false;
		
		static TableStyleFactory()
		{
			theFactory = new TableStyleFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private TableStyleFactory()
		{
            _imageGetterFactory = null;
		}

		/// <summary>
		/// Gets the TableStyleFactory instance.
		/// </summary>
		/// <returns> The TableStyleFactory instance.</returns>
		static public TableStyleFactory Instance
		{
			get
			{
				return theFactory;
			}
		}

        /// <summary>
        /// Gets or sets the ImageGetterFactory, set by window clients, such as DesignStudio etc.
        /// </summary>
        public IImageGetterFactory ImageGetterFactory
        {
            get
            {
                return _imageGetterFactory;
            }
            set
            {
                _imageGetterFactory = value;
            }
        }

        /// <summary>
        /// Gets info indicates whether the DataGridTableStyle created contains columns for
        /// inline edit
        /// </summary>
        public bool HasInlineEditColumns
        {
            get
            {
                return _hasInlineEditColumns;
            }
        }

		/// <summary>
		/// Create a DataGridTableStyle for the given data view
		/// </summary>
		/// <param name="dataView">The data view</param>
		/// <returns>A DataGridTableStyle</returns>
		public DataGridTableStyle Create(DataViewModel dataView)
		{
			return Create(dataView, null);
		}

		/// <summary>
		/// Create a DataGridTableStyle for the given data view
		/// </summary>
		/// <param name="dataView">The data view</param>
		/// <returns>A DataGridTableStyle</returns>
		public DataGridTableStyle Create(DataViewModel dataView, Image image)
		{
			DataGridTableStyle tableStyle = new DataGridTableStyle();

			// mapping the table style to the name of data view's base class
			tableStyle.MappingName = dataView.BaseClass.ClassName;
			tableStyle.AlternatingBackColor = Color.LightGray;
			DataGridColumnStyle columnStyle;
            EnumValueCollection enumValues;

			// Create an DataGridImageColumn to show an image as attachment icon
			if (image != null)
			{
				columnStyle = new DataGridImageColumn();
				((DataGridImageColumn) columnStyle).ColumnImage = image; // default image
                ((DataGridImageColumn)columnStyle).ImageGetter = new AttachmentColumnImageGetter(image);
				columnStyle.MappingName = NewteraNameSpace.ATTACHMENTS;
				columnStyle.HeaderText = "";
				columnStyle.ReadOnly = true;
				columnStyle.Width = image.Width + 2;
				tableStyle.GridColumnStyles.Add(columnStyle);
			}

            _hasInlineEditColumns = false;
			foreach (IDataViewElement resultAttribute in dataView.ResultAttributes)
			{
				// Create a column style for each result of DataSimpleAttribute or DataVirtualAttribute type
				if (resultAttribute is DataSimpleAttribute ||
                    resultAttribute is DataVirtualAttribute ||
                    resultAttribute is DataImageAttribute)
				{
                    SchemaModelElement schemaModelElement = resultAttribute.GetSchemaModelElement();
                    SimpleAttributeElement simpleAttribute = schemaModelElement as SimpleAttributeElement;
                    VirtualAttributeElement virtualAttribute = schemaModelElement as VirtualAttributeElement;
                    ImageAttributeElement imageAttribute = schemaModelElement as ImageAttributeElement;

                    if (simpleAttribute != null)
                    {
                        if (simpleAttribute.Constraint != null &&
                            simpleAttribute.Constraint is EnumElement &&
                            ((EnumElement)simpleAttribute.Constraint).DisplayMode == EnumDisplayMode.Image)
                        {
                            // display the image for the enum value
                            columnStyle = new DataGridImageColumn();
                            ((DataGridImageColumn)columnStyle).ColumnImage = image; // default image
                            ((DataGridImageColumn)columnStyle).ImageGetter = new EnumColumnImageGetter((EnumElement)simpleAttribute.Constraint);
                        }
                        else if (simpleAttribute.InlineEditEnabled &&
                                simpleAttribute.Constraint != null &&
                                simpleAttribute.Constraint is EnumElement)
                        {
                            // show combobox with enum values
                            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
                            enumValues = ((EnumElement) simpleAttribute.Constraint).Values;
                            comboBoxColumn.ComboBoxControl.DataSource = enumValues;
                            comboBoxColumn.ComboBoxControl.DisplayMember = "DisplayText";
                            comboBoxColumn.ComboBoxControl.ValueMember = "DisplayText";
                            tableStyle.PreferredRowHeight = comboBoxColumn.ComboBoxControl.Height;
                            columnStyle = comboBoxColumn;
                        }
                        else if (simpleAttribute.InlineEditEnabled &&
                                simpleAttribute.Constraint != null &&
                                simpleAttribute.Constraint is ListElement)
                        {
                            // show combobox with enum values
                            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
                            enumValues = EnumTypeFactory.Instance.GetEnumValues(simpleAttribute, (ListElement)simpleAttribute.Constraint);
                            comboBoxColumn.ComboBoxControl.DataSource = enumValues;
                            comboBoxColumn.ComboBoxControl.DisplayMember = "DisplayText";
                            comboBoxColumn.ComboBoxControl.ValueMember = "DisplayText";
                            tableStyle.PreferredRowHeight = comboBoxColumn.ComboBoxControl.Height;
                            columnStyle = comboBoxColumn;
                        }
                        else if (simpleAttribute.InlineEditEnabled &&
                                simpleAttribute.DataType == DataType.Boolean)
                        {
                            // show combobox with boolean values
                            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn();
                            comboBoxColumn.ComboBoxControl.DataSource = GetBooleanValues();
                            comboBoxColumn.ComboBoxControl.DisplayMember = "DisplayText";
                            comboBoxColumn.ComboBoxControl.ValueMember = "DisplayText";
                            tableStyle.PreferredRowHeight = comboBoxColumn.ComboBoxControl.Height;
                            columnStyle = comboBoxColumn;
                        }
                        else
                        {
                            // show text box
                            columnStyle = new DataGridTextBoxColumn();
                        }

                        if (!resultAttribute.IsReadOnly &&
                            simpleAttribute.InlineEditEnabled)
                        {
                            _hasInlineEditColumns = true;
                        }
                    }
                    else if (virtualAttribute != null &&
                        virtualAttribute.Constraint != null &&
                        virtualAttribute.Constraint is EnumElement &&
                        ((EnumElement)virtualAttribute.Constraint).DisplayMode == EnumDisplayMode.Image)
                    {
                        // display the image for the enum value
                        columnStyle = new DataGridImageColumn();
                        ((DataGridImageColumn)columnStyle).ColumnImage = image; // default image
                        if (_imageGetterFactory != null)
                        {
                            ((DataGridImageColumn)columnStyle).ImageGetter = _imageGetterFactory.Create(ImageGetterType.EnumColumnImageGetter, virtualAttribute.Constraint, dataView.SchemaInfo);
                        }
                    }
                    else if (imageAttribute != null)
                    {
                        // display the thumbnail image
                        columnStyle = new DataGridImageColumn();
                        ((DataGridImageColumn)columnStyle).ColumnImage = image; // default image
                        if (_imageGetterFactory != null)
                        {
                            ((DataGridImageColumn)columnStyle).ImageGetter = _imageGetterFactory.Create(ImageGetterType.ImageAttributeImageGetter, imageAttribute, dataView.SchemaInfo);
                        }
                    }
                    else
                    {
                        // display as text by default
                        columnStyle = new DataGridTextBoxColumn();
                    }

					columnStyle.MappingName = resultAttribute.Name;
					columnStyle.HeaderText = resultAttribute.Caption;
					columnStyle.NullText = "";

                    if (!resultAttribute.IsReadOnly &&
                        simpleAttribute != null &&
                        simpleAttribute.InlineEditEnabled)
                    {
                        columnStyle.ReadOnly = false;
                    }
                    else
                    {
                        columnStyle.ReadOnly = true;
                    }
					
					tableStyle.GridColumnStyles.Add(columnStyle);
				}
			}

			return tableStyle;
		}

        /// <summary>
        /// Gets a list of IComboBoxItem for boolean values
        /// </summary>
        /// <returns>A IList of IComboBoxItem object</returns>
        public IList GetBooleanValues()
        {
            ArrayList values = new ArrayList();
            BooleanValue val = new BooleanValue(LocaleInfo.Instance.None, LocaleInfo.Instance.None);
            values.Add(val);
            val = new BooleanValue(LocaleInfo.Instance.True, LocaleInfo.Instance.True);
            values.Add(val);
            val = new BooleanValue(LocaleInfo.Instance.False, LocaleInfo.Instance.False);
            values.Add(val);

            return values;
        }
	}
}
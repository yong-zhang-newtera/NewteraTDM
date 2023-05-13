/*
* @(#)ArrayAttributeElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Collections.Specialized;
	using System.Collections;
	using System.Xml.Schema;
	using System.ComponentModel;
	using System.Drawing.Design;
	using Newtera.Common.Core;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// ArrayAttributeElement represents an attribute that store values for one or two
	/// dimentional array.
	/// </summary>
	/// <example>
	/// <!--Xml Schema representation:
	/// <xsd:element name="coordinates" type="xsd:string" psd:dimension="2" psd:elementType="int" psd:id="0"/>
	/// -->
	/// </example>
	/// <version>1.0.1 10 Aug 2004</version>
	/// <author> Yong Zhang </author>
	public class ArrayAttributeElement : AttributeElementBase
	{
		/// <summary>
		/// The maximum length of a Normal size array.
		/// </summary>
		public const int MAX_COLUMN_LENGTH = 4000;

		public const string DELIMETER = ";";

		private string _category = null;
		private string _section = null;
		private bool _isRequired = false;
        private bool _isSingleRow = false;
		private DataType _type = DataType.String;
		private int _dimension = 1; // default value
		private int _columns = 1; // default value
		private StringCollection _columnTitles = null;
		private DataType _elementType = DataType.Unknown;
        private bool _isFullTextSearchAttribute = false;
        private string _keywordFormat = null;

		private ArraySizeType _arraySize = ArraySizeType.NormalSize; // default value

		/// <summary>
		/// Initializing ArrayAttributeElement object.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		public ArrayAttributeElement(string name) : base(name)
		{
			_columnTitles = new StringCollection();
		}

		/// <summary>
		/// Initializing ArrayAttributeElement object.
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal ArrayAttributeElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
		}

		/// <summary>
		/// Gets or sets information indicating whether the attribute is required.
		/// </summary>
		/// <value> return true if attribute is required, false otherwise. The default is false.</value>
		/// 
		/// <remarks>
		/// Using the XML schema's attribute minOccurs to determine whether this 
		/// attribute is required. If minOccurs="1" means this attribute is 
		/// required.
		/// </remarks>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Is the attribute value required?"),
		DefaultValueAttribute(false)
		]		
		public bool IsRequired
		{
			get
			{
				return _isRequired;
			}
			set
			{
				_isRequired = value;
			}
		}

		/// <summary>
		/// Gets or sets the dimension of the array.
		/// </summary>
		/// <value>1 or 2 only</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The dimension of the array"),
		DefaultValueAttribute(1)
		]		
		public int Dimension
		{
			get
			{
				return _dimension;
			}
			set
			{
				if (value < 1 || value > 2)
				{
					_dimension = 1; // default is 1
				}
				else
				{
					_dimension = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets information indicating whether the array has a single row
        /// </summary>
        /// <value> return true if the array has single row, false otherwise. The default is false.</value>
        /// 
        [
        CategoryAttribute("System"),
        DescriptionAttribute("Is a single-row array?"),
        DefaultValueAttribute(false)
        ]
        public bool IsSingleRow
        {
            get
            {
                return _isSingleRow;
            }
            set
            {
                _isSingleRow = value;
            }
        }

		/// <summary>
		/// Gets or sets data type of the array elements.
		/// </summary>
		/// <value>The one of the DataType enum values.</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The data type of the array elements")
		]	
		public DataType ElementDataType
		{
			get
			{
				return _elementType;
			}
			set
			{
				// DataTye.Text is used internaly for Full-Text Search only
				if (value != DataType.Text)
				{
					_elementType = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the number of columns in an array.
		/// </summary>
		/// <value>an integer</value>
		/// <remarks>One column for one-dimension array</remarks>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The number of columns of the array"),
		DefaultValueAttribute(1)
		]		
		public int ColumnCount
		{
			get
			{
				if (_dimension == 1)
				{
					_columns = 1;
				}

				return _columns;
			}
			set
			{
				if (_dimension == 1)
				{
					_columns = 1;
				}
                else if (value > 0)
                {
                    _columns = value;
                }
                else
                {
                    _columns = 1; // default
                }
			}
		}

		/// <summary>
		/// Gets or sets the column titles of an array.
		/// </summary>
		/// <value>A StringCollection</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The column titles of the array"),
		EditorAttribute("Newtera.Studio.ArrayColumnCollectionEditor, Studio", typeof(UITypeEditor)),
		]		
		public StringCollection ColumnTitles
		{
			get
			{
				return _columnTitles;
			}
			set
			{
				_columnTitles = value;

				// set the array dimension and column numbers automatically
				if (value.Count > 1)
				{
					this.Dimension = 2;
					this.ColumnCount = value.Count;
				}
				else if (value.Count == 1)
				{
					this.Dimension = 1;
				}

				FireValueChangedEvent(value); // fire a Value changed event
			}
		}

		/// <summary>
		/// Gets or sets type of array size.
		/// </summary>
		/// <value>The one of the ArraySizeType enum values.</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("The type of array size. NormalSize has a limit of 4000 characters."),
		ReadOnlyAttribute(true)
		]	
		public ArraySizeType ArraySize
		{
			get
			{
				return _arraySize;
			}
			set
			{
				_arraySize = value;
			}
		}

		/// <summary>
		/// Gets or sets data type of the attribute.
		/// </summary>
		/// <value>The data type for array attribute is String</value>
		[BrowsableAttribute(false)]		
		public override DataType DataType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

        /// <summary>
        /// Gets or sets information indicating whether to invoke call back function code defined for the class
        /// </summary>
        /// <remarks>Invalid for array attribute</remarks>
        [BrowsableAttribute(false)]
        public virtual bool InvokeCallback
        {
            get
            {
                return base.InvokeCallback;
            }
            set
            {
                base.InvokeCallback = value;
            }
        }

        /// <summary>
        /// Gets column length of the attribute.
        /// </summary>
        /// <value>The maximum column length</value>
        [BrowsableAttribute(false)]		
		public int ColumnLength
		{
			get
			{
				return MAX_COLUMN_LENGTH;
			}
		}

        /// <summary>
        /// Gets or sets section of the attribute.
        /// </summary>
        /// <value>
        /// A string of section name.
        /// </value>
        [BrowsableAttribute(false)]
        public string Section
		{
			get
			{
				return _section;
			}
			set
			{
				_section = value;
			}
		}

        /// <summary>
        /// Gets or sets section of the attribute.
        /// </summary>
        /// <value>
        /// A string of section name.
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The category of the array elements")
        ]
        public string Category
		{
			get
			{
				return _category;
			}
			set
			{
				_category = value;
			}
		}

        /// <summary>
        /// Gets or sets information indicating whether this attribute is good for
        /// full-text search. This method is used by the full-text indexer to 
        /// determine whether to include the content of this attribute as part
        /// of full-text index.
        /// </summary>
        /// <value>
        /// true if it is good for full-text search, false otherwise. Default is false.
        /// </value>
        [
        CategoryAttribute("Index"),
        DescriptionAttribute("Is the attribute value good for full-text search?"),
        DefaultValueAttribute(false)
        ]
        public bool IsFullTextSearchAttribute
		{
            get
            {
                return _isFullTextSearchAttribute;
            }
            set
            {
				_isFullTextSearchAttribute = value;
            }
        }

		/// <summary>
		/// Gets default value for array element
		/// </summary>
		/// <value>
		/// A default value string.
		/// </value>
		[BrowsableAttribute(false)]			
		public string DefaultValue
		{
			get
			{
				string defaultValue = "";

				switch (ElementDataType)
				{
					case DataType.Integer:
					case DataType.BigInteger:
					case DataType.Byte:
						defaultValue = "0";
						break;
					case DataType.Float:
					case DataType.Double:
					case DataType.Decimal:
						defaultValue = "0.0";
						break;
					case DataType.Date:
					case DataType.DateTime:
						defaultValue = "0000-00-00";
						break;
					case DataType.Boolean:
						defaultValue = "true";
						break;
				}

				return defaultValue;
			}
		}


		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the SchemaModelElement
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
			if (_xpath == null)
			{
                _xpath = this.Parent.ToXPath() + "/" + this.Name + NewteraNameSpace.ATTRIBUTE_SUFFIX;
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the SchemaModelElement
		/// </summary>
		/// <returns>The parent of the SchemaModelElement</returns>
		public override IXaclObject Parent
		{
			get
			{
				// the parent is the owner class element
				return this.OwnerClass;
			}
		}

		/// <summary>
		/// Return a  of children of the SchemaModelElement
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
		public override IEnumerator GetChildren()
		{
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Gets the column title of the given index.
		/// </summary>
		/// <param name="colIndex">Column index</param>
		/// <returns>A column Title</returns>
		public string GetColumnTitle(int colIndex)
		{
			string colTitle;

			if (ColumnTitles.Count > colIndex)
			{
				colTitle = ColumnTitles[colIndex];
			}
			else
			{
				colTitle = "Column_" + colIndex;
			}

			return colTitle;
		}

		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitArrayAttributeElement(this);
		}
		
		/// <summary>
		/// Return the name of the ArrayAttributeElement
		/// </summary>
		protected override string ElementName
		{
			get
			{
				return ((XmlSchemaElement) XmlSchemaElement).Name;
			}
		}
		
		/// <summary>
		/// Create xml schema element as an internal representation
		/// of Schema Model element.
		/// </summary>
		/// <returns> Return an XmlSchemaAnnotated object</returns>
		protected override XmlSchemaAnnotated CreateXmlSchemaElement(string name)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = name;

			return xmlSchemaElement;
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		/// <remarks>
		/// Example of simple attributes in xml schema
		/// 
		/// </remarks>
		internal override void Unmarshal()
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			// Set _section member
			_section = this.GetNewteraAttributeValue(NewteraNameSpace.SECTION);

			// Set _category member
			_category = this.GetNewteraAttributeValue(NewteraNameSpace.CATEGORY);

			// Set _dimension member
			_dimension = int.Parse(this.GetNewteraAttributeValue(NewteraNameSpace.DIMENSION));

			// Set _columns member
			string val = this.GetNewteraAttributeValue(NewteraNameSpace.COLUMN_COUNT);
			if (val != null && val.Length > 0)
			{
				_columns = int.Parse(val);
			}
			else
			{
				_columns = 1;
			}

			// Set _columnTitles member
			_columnTitles = ConvertToCollection(this.GetNewteraAttributeValue(NewteraNameSpace.COLUMN_TITLES));

			// Set _elementType member
			string typeString = this.GetNewteraAttributeValue(NewteraNameSpace.ELEMENT_TYPE);
			if (typeString != null && typeString.Length > 0)
			{
				_elementType = DataTypeConverter.ConvertToTypeEnum(typeString);
			}

			// Set _arraySize member
			string sizeString = this.GetNewteraAttributeValue(NewteraNameSpace.ARRAY_SIZE);
			if (sizeString != null && sizeString.Length > 0)
			{
				_arraySize = (ArraySizeType) Enum.Parse(typeof(ArraySizeType), sizeString);
			}
			else
			{
				_arraySize = ArraySizeType.NormalSize;
			}

            // Set _isSingleRow member
            string singleRowString = this.GetNewteraAttributeValue(NewteraNameSpace.SINGLE_ROW);
            if (!string.IsNullOrEmpty(singleRowString) && singleRowString == "true")
            {
                _isSingleRow = true;
            }
            else
            {
                _isSingleRow = false;
            }

			// Set isRequired member
			_isRequired = (xmlSchemaElement.MinOccurs > 0 ? true : false);

            // set isGoodForFullTextSearch member
            string status = GetNewteraAttributeValue(NewteraNameSpace.GOOD_FOR_FULL_TEXT);
			_isFullTextSearchAttribute = (status != null && status == "true" ? true : false);

            // set _keywordFormat
            _keywordFormat = this.GetNewteraAttributeValue(NewteraNameSpace.KEYWORD_FORMAT);
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <remarks>
		/// Notice that we write physical value of a member to xml schema.
		/// For example, _isUnique member holds a physical value, but IsUnique
		/// property holds logical value. Logical value may be different from
		/// physical value. When an attribute is a primary key, even _isUnique
		/// member holds a physical value of false, IsUnique property will return
		/// true. Therefore, make sure that only physical values get written to
		/// xml schema. The same is true when reading value from a xml schema in
		/// Unmarshal process.
		/// </remarks>
		internal override void Marshal()
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) XmlSchemaElement;

			// Write _section member
			if (_section != null && _section.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.SECTION, _section);	
			}

			// Write _category member
			if (_category != null && _category.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.CATEGORY, _category);	
			}

			// Write _dimension
			SetNewteraAttributeValue(NewteraNameSpace.DIMENSION, _dimension.ToString());

			// Write _columns
			SetNewteraAttributeValue(NewteraNameSpace.COLUMN_COUNT, _columns.ToString());

			// Write _columnTitles
			if (_columnTitles.Count > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.COLUMN_TITLES, ConvertToString(_columnTitles));
			}

			// Write _elementType member
			if (_elementType != DataType.Unknown)
			{
				SetNewteraAttributeValue(NewteraNameSpace.ELEMENT_TYPE, DataTypeConverter.ConvertToTypeString(_elementType));	
			}

			// Write _arraySize member
			if (_arraySize != ArraySizeType.NormalSize)
			{
				SetNewteraAttributeValue(NewteraNameSpace.ARRAY_SIZE, Enum.GetName(typeof(ArraySizeType), _arraySize));	
			}

            // Write _isSingleRow member
            if (_isSingleRow)
            {
                SetNewteraAttributeValue(NewteraNameSpace.SINGLE_ROW, "true");	             
            }

            // Write IsGoodForFullTextSearch member
            if (_isFullTextSearchAttribute)
            {
                SetNewteraAttributeValue(NewteraNameSpace.GOOD_FOR_FULL_TEXT, "true");
            }

            // Write _keywordFOrmat member
            if (!string.IsNullOrEmpty(_keywordFormat))
            {
                SetNewteraAttributeValue(NewteraNameSpace.KEYWORD_FORMAT, _keywordFormat);
            }

			// Write IsRequired member
			if (_isRequired)
			{
				xmlSchemaElement.MinOccurs = 1;
			}
			else
			{
				xmlSchemaElement.MinOccurs = 0;
			}

			xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(DataTypeConverter.ConvertToTypeString(_type), "http://www.w3.org/2003/XMLSchema");

			// Always to call this last
			base.Marshal();
		}

		/// <summary>
		/// Convert a comma separated column title string into a StringCollection object
		/// </summary>
		/// <param name="colTitles">A comma separated column title string</param>
		/// <returns>A StringCollection object</returns>
		private StringCollection ConvertToCollection(string colTitles)
		{
			StringCollection columnTitles = new StringCollection();

			if (colTitles != null && colTitles.Length > 0)
			{
				Regex regex = new Regex(DELIMETER);
				string[] values = regex.Split(colTitles);
				columnTitles.AddRange(values);
			}

			return columnTitles;
		}

		/// <summary>
		/// Convert a StringCollection object into a comma separated column title string.
		/// </summary>
		/// <param name="columnTitles">A StringCollection object</param>
		/// <returns>A comma separated column title string</returns>
		private string ConvertToString(StringCollection columnTitles)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < columnTitles.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(DELIMETER);
				}

				builder.Append(columnTitles[i]);
			}

			return builder.ToString();
		}
	}

	/// <summary>
	/// Specify the size of an array as indication to the database builder to
	/// pick appropriate type of storage for storing the array value.
	/// </summary>
	public enum ArraySizeType
	{
		/// <summary>
		/// The array size is less than 4000 characters, including array element
		/// delimiters. Declare an array of normal size is more efficient in terms
		/// of database retrieval and updates
		/// </summary>
		NormalSize,
		/// <summary>
		/// The array size is over 4000 characters, including array element
		/// delimiters. Declare an array of over size could make query slower.
		/// </summary>
		OverSize
	}
}
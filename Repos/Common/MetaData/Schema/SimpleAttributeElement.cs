/*
* @(#)SimpleAttributeElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
	using System.Collections;
    using System.Text;
    using System.Web;
    using System.Threading;
	using System.Xml.Schema;
	using System.ComponentModel;
	using System.Drawing.Design;
    using System.Runtime.Remoting;
    using System.Security.Principal;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// SimpleAttributeElement represents a simple attribute in a class.
	/// </summary>
	/// 
	/// <!--
	/// Xml Schema representation:
	///  
	/// <xsd:element name="orderNum" type="xsd:positiveInteger" psd:key="true" psd:id="0"/>
	/// or
	/// <xsd:element name="Address" psd:id="0">
	/// <xsd:simpleType>
	/// <xsd:restriction base="xsd:string">
	/// <xsd:maxLength value="100"/>
	/// </xsd:restriction>
	/// </xsd:simpleType>
	/// </xsd:element>
	/// -->
	/// 
	/// <version>      1.0.1 11 Jul 2003
	/// </version>
	/// <author> Yong Zhang </author>
	public class SimpleAttributeElement:AttributeElementBase
	{
        public const string SYS_TIME = "systime";
        public const string UID = "uid";

        internal const string OPT_EQUALS = "EQ";
        internal const string OPT_NOT_EQUALS = "NE";
        internal const string OPT_LESS_THAN = "LT";
        internal const string OPT_GREATER_THAN = "GT";
        internal const string OPT_LESS_THAN_EQUALS = "LE";
        internal const string OPT_GREATER_THAN_EQUALS = "GE";
        internal const string OPT_LIKE = "LK";

		private string _category = null;
		private string _section = null;
		private DefaultViewUsage _usage = DefaultViewUsage.Included;
		private bool _isRequired = false;
		private bool _isAutoIncrement = false;
		private bool _isUnique = false;
		private bool _isPrimaryKey = false;
		private CaseStyle _caseStyle = CaseStyle.CaseSensitive;
		private DataType _type = DataType.Unknown;
        private DataType _oldType = DataType.Unknown; // run-time member
		private int _minLength = 0;
		private int _maxLength = 100;
		private string _defaultValue = null;
        private string _dataSource = null;
        private string _displayFormat = null;
        private string _inputMask = null;
        private bool _isHistoryEdit = false;
        private bool _isRichText = false;
		private bool _isMultipleLined = false;
		private bool _isIndexed = false;
		private bool _isFullTextSearchAttribute = false;
        private ConstraintElementBase _refConstraint = null;
        private string _autoValueGenerator = null;
		private int _rows = 1;
        private string _operator = null;
        private bool _inlineEditEnabled = false;
        private bool _isReadOnly = false;
        private bool _allowManualUpdate = true;
        private bool _showAsProgressBar = false;
        private bool _showUpdateHistory = false;
        private bool _isEncrypted = false;
        private string _cascadedAttributeName = null;
        private string _parentAttributeName = null;
        private ConstraintUsage _constraintUsage = ConstraintUsage.Restriction; // default
        private string _lastIndexedTime = null; // run-time value
        private IAttributeValueGenerator _valueGenerator = null; // run-time value
        private string _keywordFormat = null;

        /// <summary>
        /// Create an instance of the custom value generator as specified.
        /// </summary>
        /// <param name="generatorDef">The definition of the value generator</param>
        /// <returns>An instance of value generator of interface IAttributeValueGenerator, null if failed to create the instance.</returns>
        public static IAttributeValueGenerator CreateGenerator(string generatorDef)
        {
            IAttributeValueGenerator generator = null;

            if (!string.IsNullOrEmpty(generatorDef))
            {
                int index = generatorDef.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = generatorDef.Substring(0, index).Trim();
                    assemblyName = generatorDef.Substring(index + 1).Trim();
                }
                else
                {
                    className = generatorDef.Trim();
                }

                try
                {

                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    generator = (IAttributeValueGenerator)obj.Unwrap();
                }
                catch
                {
                    generator = null;
                }
            }

            return generator;
        }
		
		/// <summary>
		/// Initializing SimpleAttributeElement object.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		public SimpleAttributeElement(string name) : base(name)
		{
		}

		/// <summary>
		/// Initializing SimpleAttributeElement object.
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal SimpleAttributeElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
		}

		/// <summary>
		/// Gets or sets the information indicating whether the attribute is browsable.
		/// </summary>
		/// <value>true if it is browsable, false otherwise, default is true</value>
		/// <remarks>If browsable is false, it won't appear as result field of a data view,
		/// but it still can be used as a search field</remarks>
		public override bool IsBrowsable
		{
			get
			{
				if (this.IsRequired)
				{
					return true; // required attribute alwayse browsable
				}
				else
				{
					return base.IsBrowsable;
				}
			}
			set
			{
				base.IsBrowsable = value;
			}
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
				if (IsPrimaryKey)
				{
					return true;
				}
				else
				{
					return _isRequired;
				}
			}
			set
			{
                _isRequired = value;
			}
		}

		/// <summary>
		/// Gets or sets the constraint referenced by the attribute.
		/// </summary>
		/// <value> The referenced constraint</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The constraint specification to the attribute value"),
			DefaultValueAttribute(null),
			EditorAttribute("Newtera.Studio.ConstraintPropertyEditor, Studio", typeof(UITypeEditor)),
			TypeConverterAttribute("Newtera.Studio.ConstraintPropertyConverter, Studio")
		]		
		public ConstraintElementBase Constraint
		{
			get
			{
				return _refConstraint;
			}
			set
			{
				_refConstraint = value;
			}
		}

		/// <summary>
		/// Gets or sets information to indicate whether the attribute is auto-increment.
		/// </summary>
		/// <value>
		/// true if it is auto-increment; otherwise false. default is false.
		/// </value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("Is the attribute value auto-increased?"),
			DefaultValueAttribute(false)
		]		
		public bool IsAutoIncrement
		{
			get
			{
				return _isAutoIncrement;
			}
			set
			{
                _isAutoIncrement = value;
			}
		}

        /// <summary>
        /// Gets or sets the class definition that generate an unique value for auto-incremental attribute.
        /// </summary>
        /// <value>
        /// A fully-qualified generator class name, including namespace and class name.
        /// for example, MyLib.TestIDGenerator, MyLib
        /// </value>
        [BrowsableAttribute(false)]
        public string AutoValueGenerator
        {
            get
            {
                return _autoValueGenerator;
            }
            set
            {
                _autoValueGenerator = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the attribute has a custom value generator
        /// </summary>
        [BrowsableAttribute(false)]	
        public bool HasCustomValueGenerator
        {
            get
            {
                if (string.IsNullOrEmpty(this.AutoValueGenerator))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

		/// <summary>
		/// Gets or sets information to indicate whether value of the attribute is unique.
		/// </summary>
		/// <value> true if it is unique, otherwise false. Default is false.
		/// </value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("Is the attribute value unique?"),
			DefaultValueAttribute(false)
		]		
		public bool IsUnique
		{
			get
			{
				if (IsPrimaryKey)
				{
					// If it is a primary key, then it is considered to be unique
					return true;
				}
				else
				{
					return _isUnique;
				}
			}
			set
			{
                _isUnique = value;
			}
		}

		/// <summary>
		/// Gets or sets information to indicate whether the attribute is a primary key.
		/// </summary>
		/// <value> true if it is a primary key, otherwise false. Default is false.
		/// </value>
        /*
		[
			CategoryAttribute("System"),
			ReadOnlyAttribute(true),
			DescriptionAttribute("Is the attribute a primary key? A primary key is selected at a Class level.")
		]
        */
        [BrowsableAttribute(false)]
        public bool IsPrimaryKey
		{
			get
			{
				return _isPrimaryKey;
			}
			set
			{
				_isPrimaryKey = value;
			}
		}

        /// <summary>
        /// Gets or sets information indicating whether the attribute can be edited within a datagrid.
        /// </summary>
        /// <value> return true if attribute can have inline editing, false otherwise. The default is false.</value>
        /// 
        /// <remarks>
        /// Some UI can use this value to determine whether the attribute values can be edited within a datagrid
        /// </remarks>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Is Inine Editing enabled for the attribute?"),
            DefaultValueAttribute(false)
        ]
        public bool InlineEditEnabled
        {
            get
            {
                if (!_allowManualUpdate)
                {
                    return false;
                }
                else
                {
                    return _inlineEditEnabled;
                }
            }
            set
            {
                _inlineEditEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether the attribute can be updated manually.
        /// </summary>
        /// <value> return true if attribute can be updated manually through user interface, false
        /// indicating that the attribute can be updated in programs instead of user intefaces.
        /// The default is true.</value>
        /// 
        /// <remarks>
        /// This setting only be effective on Web user interface.
        /// </remarks>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Is it allowed to update attribute manually?"),
            DefaultValueAttribute(true)
        ]
        public bool AllowManualUpdate
        {
            get
            {
                return _allowManualUpdate;
            }
            set
            {
                _allowManualUpdate = value;
            }
        }

		/// <summary>
		/// Gets or sets data type of the attribute.
		/// </summary>
		/// <value>One of DataType enum values. default is String</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("Data type of the attribute value"),
			DefaultValueAttribute(DataType.String)
		]		
		public override DataType DataType
		{
			get
			{
                // If there is a constraint, data type is derived from constraint
                if (Constraint != null)
                {
                    return Constraint.DataType;
                }
                else
                {
                    return _type;
                }
			}
			set
			{
                if ((this.IsHistoryEdit || this.IsRichText) && value != DataType.Text)
                {
                    return; // do not allow change a historyedit, rich text attribute to a type other then Text
                }

                _oldType = _type;

				_type = value;
			}
		}

		/// <summary>
		/// Gets or sets minimum length of attribute value. 
		/// </summary>
		/// <value>The minimum length of attribute value. Default is 0</value>
		/// <remarks>It is applicable to an attribute of string type.</remarks>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The minimum length of the attribute value"),
			DefaultValueAttribute(0)
		]		
		public int MinLength
		{
			get
			{
				return _minLength;
			}
			set
			{
				_minLength = value;
			}
		}

		/// <summary>
		/// Gets or sets maximum length of attribute value. 
		/// </summary>
		/// <value>The maximum length of attribute value. Default is 1024</value>
		/// <remarks>It is applicable to an attribute of string type.</remarks>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The maximum length of the attribute value"),
			DefaultValueAttribute(100)
		]		
		public int MaxLength
		{
			get
			{
				return _maxLength;
			}
			set
			{
				_maxLength = value;
			}
		}

		/// <summary>
		/// Gets the actual database column length.
		/// </summary>
		/// <value>The actual length for database column, -1 if it is not applicable</value>
		[BrowsableAttribute(false)]	
		public int ColumnLength
		{
			get
			{
				int length = -1;

				switch (this.DataType)
				{
					case DataType.String:
						if (Constraint is EnumElement)
						{
							EnumElement enumConstraint = (EnumElement) Constraint;

							// when is multiple selection, we need to allow
							// column space to store multiple values
							if (enumConstraint.IsMultipleSelection)
							{
								length = MaxLength * enumConstraint.Values.Count + enumConstraint.Values.Count;
							}
							else
							{
								length = MaxLength;
							}
						}
						else
						{
							length = MaxLength;
						}

						break;
				}

				return length;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the default value is system time.
		/// </summary>
		/// <value>True if it uses system time as default, false otherwise. Default is false.</value>
		[BrowsableAttribute(false)]	
		public bool IsSystemTimeDefault
		{
			get
			{
                if (DefaultValue != null && DefaultValue == SYS_TIME)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets the information indicating whether the default value is unique id function.
        /// </summary>
        /// <value>True if it uses unique id function as default, false otherwise. Default is false.</value>
        [BrowsableAttribute(false)]
        public bool IsUidDefault
        {
            get
            {
                if (DefaultValue != null && DefaultValue == UID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the value of simple attribute is multiple-choice
        /// </summary>
        /// <value> True if it is multiple choice, false otherwise</value>
        [BrowsableAttribute(false)]	
		public bool IsMultipleChoice
		{
			get
			{
				if (Constraint != null &&
					Constraint is EnumElement &&
					((EnumElement) Constraint).IsMultipleSelection)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets or sets default value of an attribute.
        /// </summary>
        /// <value> The default value.
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Gets or sets of a default value for the attribute"),
            DefaultValueAttribute(null)
        ]
        public string DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				_defaultValue = value;
			}
		}

        /// <summary>
        /// Gets or sets name of a class whose primary keys are source of this attribute's values.
        /// </summary>
        /// <value> The class namedefault value.
        /// </value>
        /*
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Name of a class whose primary keys are source of this attribute's values"),
            DefaultValueAttribute(null),
            TypeConverterAttribute("Newtera.Common.MetaData.Schema.ClassNameConverter"),
            EditorAttribute("Newtera.Studio.ClassNamePropertyEditor, Studio", typeof(UITypeEditor)),
        ]
        */
        [BrowsableAttribute(false)]
        public string DataSourceName
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating when the full-text index was built last time
        /// searchable.
        /// </summary>
        /// <value>
        /// </value>
        /*
        [
            CategoryAttribute("Index"),
            DescriptionAttribute("Last time the full-text index was built"),
            ReadOnlyAttribute(true)
        ]
        */
        [BrowsableAttribute(false)]
        public string LastIndexedTime
        {
            get
            {
                return _lastIndexedTime;
            }
            set
            {
                _lastIndexedTime = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether value of the attribute rich text.
        /// </summary>
        /// <value>
        /// true if it is rich text, false otherwise. Default is false.
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Is value of the attribute rich text?"),
            DefaultValueAttribute(false)
        ]
        public bool IsRichText
        {
            get
            {
                return _isRichText;
            }
            set
            {
                _isRichText = value;

                if (value)
                {
                    DataType = DataType.Text; // Use Text to store rich text
                    IsMultipleLined = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether this attribute is for edit history.
        /// </summary>
        /// <value>
        /// true if it is editing hsitory, false otherwise. Default is false.
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Is the attribute for editing history?"),
            DefaultValueAttribute(false)
        ]
        public bool IsHistoryEdit
        {
            get
            {
                return _isHistoryEdit;
            }
            set
            {
                _isHistoryEdit = value;

                if (value)
                {
                    DataType = DataType.Text; // Use Text to store edit history
                    IsMultipleLined = true;
                }
            }
        }

		/// <summary>
		/// Gets or sets information indicating whether this attribute is for
		/// full-text search. This method is used by the full-text indexer to 
		/// determine whether to include the content of this attribute as part
		/// of full-text index.
		/// </summary>
		/// <value>
		/// true if it is a full-text search attribute, false otherwise. Default is false.
		/// </value>
		[
		CategoryAttribute("Index"),
		DescriptionAttribute("Is a full-text search attribute?"),
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
		/// Gets or sets information indicating whether the attribute is multiple-lined.
		/// </summary>
		/// <value> true if the attribute is multiple-lined, false, otherwise. Default is false.
		/// </value>
		[
			CategoryAttribute("Appearance"),
			DescriptionAttribute("Is the attribute value displayed in multiple lines?"),
			DefaultValueAttribute(false)
		]		
		public bool IsMultipleLined
		{
			get
			{
				return _isMultipleLined;
			}
			set
			{
				_isMultipleLined = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of rows for a multiple-lined property.
		/// </summary>
		/// <value> an integer</value>
		[
		    CategoryAttribute("Appearance"),
		    DescriptionAttribute("Number of rows displayed for multiple line property"),
		    DefaultValueAttribute(1)
		]		
		public int Rows
		{
			get
			{
				if (!IsMultipleLined)
				{
					return 1; // for single lined
				}
				else
				{
					if (_rows <= 1)
					{
						return 5; // default rows for a multiple line
					}
					else
					{
						return _rows;
					}
				}
			}
			set
			{
				if (value > 50)
				{
					_rows = 50; // maximum row is 50
				}
				else
				{
					_rows = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets information indicating whether the attribute is shown as a progress bar
        /// </summary>
        /// <value> true if the attribute is shown as progress bar, false, otherwise. Default is false.
        /// </value>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Is the attribute value displayed as a progress bar?"),
            DefaultValueAttribute(false)
        ]
        public bool ShowAsProgressBar
        {
            get
            {
                return _showAsProgressBar;
            }
            set
            {
                _showAsProgressBar = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether to show update history of the attribute
        /// </summary>
        /// <value> true if the attribute's update history is shown, false, otherwise. Default is false.
        /// </value>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Is update history of the attribute shown?"),
            DefaultValueAttribute(false)
        ]
        public bool ShowUpdateHistory
        {
            get
            {
                return _showUpdateHistory;
            }
            set
            {
                _showUpdateHistory = value;
            }
        }


        /// <summary>
        /// Gets or sets information indicating whether the attribute's value is stored as encrypted
        /// </summary>
        /// <value> true if the attribute'value is stored as encrypted, false, otherwise. Default is false.
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Is the attribute value stored as encrypted?"),
            DefaultValueAttribute(false)
        ]
        public bool IsEncrypted
        {
            get
            {
                if (DataType != DataType.String)
                {
                    return false;
                }
                else
                {
                    return _isEncrypted;
                }
            }
            set
            {
                _isEncrypted = value;
            }
        }

        /// <summary>
        /// Gets or sets display format string which is confirming to C# string.Format(format)
        /// </summary>
        /// <value> A string representing input mask </value>
        /*
        [
             CategoryAttribute("Appearance"),
             DescriptionAttribute("Display Format"),
             DefaultValueAttribute(null)
        ]
        */
        [BrowsableAttribute(false)]
        public string DisplayFormatString
        {
            get
            {
                return _displayFormat;
            }
            set
            {
                _displayFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets input mask
        /// </summary>
        /// <value> A string representing input mask </value>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Input mask"),
            DefaultValueAttribute(null)
        ]
        public string InputMask
        {
            get
            {
                return _inputMask;
            }
            set
            {
                _inputMask = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether the attribute is multiple-lined.
        /// </summary>
        /// <value> true if the attribute is multiple-lined, false, otherwise. Default is false.
        /// </value>
        [BrowsableAttribute(false)]	
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
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
        /// Gets or sets usage of the attribute.
        /// </summary>
        /// <value>
        /// A value of DefaultViewUsage enum values.
        /// </value>
        [
        CategoryAttribute("DefaultView"),
		DescriptionAttribute("Describe whether the attribute is part of the default data view."),
		DefaultValueAttribute(DefaultViewUsage.Included)
		]		
		public DefaultViewUsage Usage
		{
			get
			{
				return _usage;
			}
			set
			{
				_usage = value;
			}
		}

        /// <summary>
        /// Gets or sets usage of the attribute.
        /// </summary>
        /// <value>
        /// A value of AttributeUsage enum values.
        /// </value>
        [
            CategoryAttribute("DefaultView"),
            DescriptionAttribute("Describe the default operaror for a search attribute in the default view."),
            DefaultValueAttribute("="),
            EditorAttribute("Newtera.Studio.OperatorPropertyEditor, Studio", typeof(UITypeEditor)),
        ]
        public string Operator
        {
            get
            {
                if (string.IsNullOrEmpty(_operator))
                {
                    return MetaDataModel.OPT_EQUALS;
                }
                else
                {
                    return _operator;
                }
            }
            set
            {
                _operator = value;
            }
        }

        /// <summary>
        /// Gets or sets category of the attribute.
        /// </summary>
        /// <value>
        /// A string of category name.
        /// </value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The category of the simple elements")
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
		/// Gets or sets information whether the attribute is indexed.
		/// </summary>
		/// <value> true if it is indexed, false, otherwise. Default is false. 
		/// </value>
		[
			CategoryAttribute("Index"),
			DescriptionAttribute("Is the attribute value indexed by database?"),
			DefaultValueAttribute(false)
		]			
		public bool IsIndexed
		{
			get
			{
				if (IsPrimaryKey)
				{
					// when it is primary key, it is indexed automatically
					return true;
				}
				else
				{
					return _isIndexed;
				}
			}
			set
			{
				_isIndexed = value;
			}
		}

		/// <summary>
		/// Gets or sets case style of the attribute.
		/// </summary>
		/// <value>One of upper, lower, caseSensitive, caseInsensitive values.</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The case style of the attribute value"),
			DefaultValueAttribute(CaseStyle.CaseSensitive)
		]			
		public CaseStyle CaseStyle
		{
			get
			{
				return _caseStyle;
			}
			set
			{
				_caseStyle = value;
			}
		}

        /// <summary>
        /// Gets or sets names of cascaded attributes. The value of this attribute determines the
        /// list of values available for the cascade attribute.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The values of a cascaded attribute is determined by the value of this attribute."),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.CascadedAttributePropertyEditor, Studio", typeof(UITypeEditor)),
            TypeConverterAttribute("Newtera.Studio.CascadedAttributePropertyConverter, Studio")
        ]
        public string CascadedAttributes
        {
            get
            {
                return _cascadedAttributeName;
            }
            set
            {
                _cascadedAttributeName = value;
            }
        }

        /// <summary>
        /// Gets or sets name of a parent attribute whose value affects the values of a list constraint bound to this attribute
        /// </summary>
        /*
        [CategoryAttribute("System")]
        [DescriptionAttribute("Specify a parent attribute whose value affects the values of a list constraint bound to this attribute")]
        [DefaultValueAttribute(null)]
        [TypeConverterAttribute("Newtera.Studio.AttributeNameConverter, Studio")]
        [EditorAttribute("Newtera.Studio.AttributeNamePropertyEditor, Studio", typeof(UITypeEditor))]
        */
        [BrowsableAttribute(false)]
        public string ParentAttribute
        {
            get
            {
                return _parentAttributeName;
            }
            set
            {
                _parentAttributeName = value;
            }
        }

        /// <summary>
        /// Gets or sets constraint usage of the attribute.
        /// </summary>
        /// <value>
        /// A value of ConstraintUsage enum values.
        /// </value>
        [
        CategoryAttribute("System"),
        BrowsableAttribute(false),
        DescriptionAttribute("Indicate the use of the constraint associated with the attribute"),
        DefaultValueAttribute(ConstraintUsage.Restriction)
        ]
        public ConstraintUsage ConstraintUsage
        {
            get
            {
                return _constraintUsage;
            }
            set
            {
                _constraintUsage = value;
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
        /// Reset the data type to the previous one
        /// </summary>
        public void ResetDataType()
        {
            _type = _oldType;
        }

        /// <summary>
        /// Gets a filter value from the the session or hashtable for the list constraint associated with the attribute.
        /// </summary>
        /// <returns>The filter value, can be null</returns>
        public string GetListFilterValue()
        {
            string filterValue = null;

            Newtera.Common.MetaData.Principal.CustomPrincipal customPrincipal = Thread.CurrentPrincipal as Newtera.Common.MetaData.Principal.CustomPrincipal;

            if (customPrincipal != null && customPrincipal.IsServerSide)
            {
                // The client is a web application, get the filter value from the session saved by a web application
                filterValue = customPrincipal.GetUserDataString(ListElement.CreateListFilterKey(OwnerClass.SchemaModel.MetaData.SchemaInfo.NameAndVersion, OwnerClass.Name, Name));
            }
            else
            {
                // it is a windows application, get the value from a hashtable
                filterValue = (string)ListElement.ListFilterTable[ListElement.CreateListFilterKey(OwnerClass.SchemaModel.MetaData.SchemaInfo.NameAndVersion, OwnerClass.Name, Name)];

            }

            return filterValue;
        }

        /// <summary>
        /// Sets a filter value to the the session or hashtable for the list constraint associated with the attribute.
        /// </summary>
        /// <param name="filterValue">The filter value</param>
        public void SetListFilterValue(string filterValue)
        {
            Newtera.Common.MetaData.Principal.CustomPrincipal customPrincipal = Thread.CurrentPrincipal as Newtera.Common.MetaData.Principal.CustomPrincipal;

            if (customPrincipal != null && customPrincipal.IsServerSide)
            {
                // The client is a web application, get the filter value from the session saved by a web application
                customPrincipal.SetUserData(ListElement.CreateListFilterKey(OwnerClass.SchemaModel.MetaData.SchemaInfo.NameAndVersion, OwnerClass.Name, Name), filterValue);
            }
            else
            {
                // it is a windows application, get the value from a hashtable
                ListElement.ListFilterTable[ListElement.CreateListFilterKey(OwnerClass.SchemaModel.MetaData.SchemaInfo.NameAndVersion, OwnerClass.Name, Name)] = filterValue;

            }
        }

		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitSimpleAttributeElement(this);
		}

        /// <summary>
        /// Get an instance of the auto value generator if exists
        /// </summary>
        /// <returns>An instance of value generator of interface IAttributeValueGenerator, can be null</returns>
        public IAttributeValueGenerator GetAutoValueGenerator()
        {
            lock (this)
            {
                if (_valueGenerator == null)
                {
                    _valueGenerator = SimpleAttributeElement.CreateGenerator(AutoValueGenerator);
                }

                return _valueGenerator;
            }
        }
		
		/// <summary>
		/// Return the name of the SimpleAttributeElement
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
        /// Create an enum type name for the enum constraint associated with the simple attribute
        /// </summary>
        /// <returns>An unique enum type name</returns>
        public string CreateEnumTypeName()
        {
            lock (this)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Newtera.Common.Types.").Append(this.SchemaModel.SchemaInfo.Name).Append(this.SchemaModel.SchemaInfo.Version);
                if (this.Constraint != null && this.Constraint is IEnumConstraint)
                {
                    builder.Append(this.Constraint.EnumTypeName);

                    if (((IEnumConstraint)this.Constraint).IsUserBased)
                    {
                        // if the constraint's values is user based, add user name as part of the enum type
                        string userName = Thread.CurrentPrincipal.Identity.Name;
                        builder.Append(userName);
                    }
                    else if (((IEnumConstraint)this.Constraint).IsConditionBased)
                    {
                        // if the constraint's values is condition based,
                        // filter value as part of enum type
                        string filterValue = GetListFilterValue();
                        if (filterValue != null)
                        {
                            builder.Append(filterValue);
                        }
                    }
                }
                return builder.ToString();
            }           
        }

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		/// <!--
		/// Example of simple attributes in xml schema
		///	<xsd:simpleType psd:constraint="enumeration" name="State" psd:description="">
		///	<xsd:restriction base="xsd:string">
		///		<xsd:enumeration value="CA" />
		///		<xsd:enumeration value="NY" />
		///		<xsd:enumeration value="NE" />
		///		<xsd:enumeration value="LA" />
		///		<xsd:enumeration value="NJ" />
		///	</xsd:restriction>
		///	</xsd:simpleType>
		///	<xsd:simpleType psd:constraint="pattern" name = "Number" psd:description="">
		///	<xsd:restriction base="xsd:string">
		///		<xsd:pattern value="[A-Z]-\d{6}" />
		///		<xsd:minLength value="0" />
		///		<xsd:maxLength value="8" />
		///	</xsd:restriction>
		///	</xsd:simpleType>
		///
		///	<xsd:complexType name="Customer" psd:displayName="Customer" psd:order="3" psd:id="1459">
		///	<xsd:sequence>
		///		<xsd:element name="Number" type="Number" minOccurs="1" maxOccurs="1" psd:displayName="Name" psd:order="0" psd:key="true" psd:id="8477" />
		///		<xsd:element name="Name" type="xsd:string" minOccurs="1" maxOccurs="1" psd:displayName="Name" psd:order="0" psd:key="true" psd:id="8479" />
		///		<xsd:element name="Address" type="xsd:string" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="Address" psd:order="1" psd:id="8480" />
		///		<xsd:element name="City" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="City" psd:order="2" psd:id="8481">
		///			<xsd:simpleType>
		///			<xsd:restriction base="xsd:string">
		///				<xsd:maxLength value="20" />
		///			</xsd:restriction>
		///			</xsd:simpleType>
		///		</xsd:element>
		///		<xsd:element name="State" type="State" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="State" psd:order="3" psd:id="8482">
		///	</xsd:sequence>
		///	<xsd:attribute name="obj_id" type="xsd:ID" />
		///	</xsd:complexType>
		/// 
		/// -->
		internal override void Unmarshal()
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) XmlSchemaElement;
            string status;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

			// Set _section member
			_section = this.GetNewteraAttributeValue(NewteraNameSpace.SECTION);

			// Set _category member
			_category = this.GetNewteraAttributeValue(NewteraNameSpace.CATEGORY);

			// Set isRequired member
			_isRequired = (xmlSchemaElement.MinOccurs > 0 ? true : false);

			// Set the case style member
			this._caseStyle = ConvertToCaseStyleEnum(GetNewteraAttributeValue(NewteraNameSpace.CASE_STYLE));

			// Set referenced constraint
			XmlQualifiedName qualifiedName = xmlSchemaElement.SchemaTypeName;
			// check to see if it is a constraint name
			ConstraintElementBase constraint = SchemaModel.FindConstraint(qualifiedName.Name);
			if (constraint != null)
			{
				// It is a name of a constraint
				_refConstraint = constraint;
				_type = constraint.DataType;
			}

			// Set isUnique member
			status = GetNewteraAttributeValue(NewteraNameSpace.UNIQUE);
			this._isUnique = (status != null && status == "true" ? true : false);
			
			// Set isPrimaryKey
			status = GetNewteraAttributeValue(NewteraNameSpace.KEY);
			_isPrimaryKey = (status != null && status == "true" ? true : false);

            // Set InlineEditEnabled member
            status = GetNewteraAttributeValue(NewteraNameSpace.INLINE_EDIT);
            this._inlineEditEnabled = (status != null && status == "true" ? true : false);

            // Set AllowManualUpdate member, default to true
            status = GetNewteraAttributeValue(NewteraNameSpace.MANUAL_UPDATE);
            this._allowManualUpdate = (status != null && status == "false" ? false : true);

            // Set IsReadOnly member
            status = GetNewteraAttributeValue(NewteraNameSpace.READ_ONLY);
            this._isReadOnly = (status != null && status == "true" ? true : false);

			// set minLength and maxLength members.
			XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) xmlSchemaElement.SchemaType;
			if (simpleType != null)
			{
				XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;
				_type = DataTypeConverter.ConvertToTypeEnum(restriction.BaseTypeName.Name);
				foreach (XmlSchemaFacet facet in restriction.Facets)
				{
					if (facet is XmlSchemaMinLengthFacet)
					{
						_minLength = int.Parse(facet.Value);
					}
					else if (facet is XmlSchemaMaxLengthFacet)
					{
						_maxLength = int.Parse(facet.Value);
					}
				}
			}

			// The data type can be derived from a simple type or a constraint
			// from previous action
			if (_type == DataType.Unknown)
			{
				// set DataType member
				qualifiedName = xmlSchemaElement.SchemaTypeName;
				_type = DataTypeConverter.ConvertToTypeEnum(qualifiedName.Name);
			}

			// Set defaultValue member
			_defaultValue = xmlSchemaElement.DefaultValue;

            // Set isAutoIncrement member
            status = GetNewteraAttributeValue(NewteraNameSpace.AUTO_INCREMENT);
            IsAutoIncrement = (status != null && status == "true" ? true : false);

            // Set _autoValueGenerator member
            _autoValueGenerator = this.GetNewteraAttributeValue(NewteraNameSpace.VALUE_GENERATOR);

            // set IsRichText member
            status = GetNewteraAttributeValue(NewteraNameSpace.RICH_TEXT);
            IsRichText = (status != null && status == "true" ? true : false);

            // set IsHistoryEdit member
            status = GetNewteraAttributeValue(NewteraNameSpace.HISTORY_EDIT);
            IsHistoryEdit = (status != null && status == "true" ? true : false);

			// set isGoodForFullTextSearch member
			status = GetNewteraAttributeValue(NewteraNameSpace.GOOD_FOR_FULL_TEXT);
            _isFullTextSearchAttribute = (status != null && status == "true" ? true : false);

            // Set isMultipleLined member
            status = this.GetNewteraAttributeValue(NewteraNameSpace.MULTILINE);
			_isMultipleLined = (status != null && status == "true" ? true : false);

			// Set _rows member
			status = GetNewteraAttributeValue(NewteraNameSpace.ROWS);
			if (status != null && status.Length > 0)
			{
				_rows = int.Parse(status);
			}

            // Set ShowAsProgressBar member
            status = this.GetNewteraAttributeValue(NewteraNameSpace.SHOW_AS_PROGRESS_BAR);
            _showAsProgressBar = (status != null && status == "true" ? true : false);

            // Set ShowUpdateHistory member
            status = this.GetNewteraAttributeValue(NewteraNameSpace.SHOW_UPDATE_HISTORY);
            _showUpdateHistory = (status != null && status == "true" ? true : false);

			// Set _usage member
			string usgeString = this.GetNewteraAttributeValue(NewteraNameSpace.USAGE);
			if (usgeString != null && usgeString.Length > 0)
			{
                AttributeUsage  val = (AttributeUsage) Enum.Parse(typeof(AttributeUsage), usgeString);
                if (val == AttributeUsage.Both || val == AttributeUsage.Result)
                {
                    _usage = DefaultViewUsage.Included;
                }
                else
                {
                    _usage = DefaultViewUsage.Excluded;
                }
			}
			else
			{
				_usage = DefaultViewUsage.Included; // default value is included
			}

            // Set _operator member
            string opString = this.GetNewteraAttributeValue(NewteraNameSpace.OPERATOR);
            if (!string.IsNullOrEmpty(opString))
            {
                _operator = ConvertToOperatorDisplay(opString);
            }
            else
            {
                _operator = null; // null is for equals operator
            }

            // set _dataSource
            _dataSource = this.GetNewteraAttributeValue(NewteraNameSpace.DATA_SOURCE);

            // set _displayFormat
            _displayFormat = this.GetNewteraAttributeValue(NewteraNameSpace.DISPLAY_FORMAT);

            // set _inputMask
            _inputMask = this.GetNewteraAttributeValue(NewteraNameSpace.INPUT_MASK);

            // set _keywordFormat
            _keywordFormat = this.GetNewteraAttributeValue(NewteraNameSpace.KEYWORD_FORMAT);

            // Set _cascadeAttribute member
            _cascadedAttributeName = this.GetNewteraAttributeValue(NewteraNameSpace.CASCADE_ATTRIBUTE);

            // Set _parentAttributeName member
            _parentAttributeName = this.GetNewteraAttributeValue(NewteraNameSpace.PARENT_ATTRIBUTE);

			// Set isIndexed member
			status = this.GetNewteraAttributeValue(NewteraNameSpace.INDEX);
			_isIndexed = (status != null && status == "true" ? true : false);

            // Set lastIndexedTime member
            _lastIndexedTime = this.GetNewteraAttributeValue(NewteraNameSpace.INDEXED_TIME);

            // Set isMultipleLined member
            status = this.GetNewteraAttributeValue(NewteraNameSpace.IS_ENCRYPTED);
            _isEncrypted = (status != null && status == "true" ? true : false);

            // Set _constraintUsage member
            string constraintUsgeString = this.GetNewteraAttributeValue(NewteraNameSpace.CONSTRAINT_USAGE);
            if (!string.IsNullOrEmpty(constraintUsgeString))
            {
                _constraintUsage = (ConstraintUsage)Enum.Parse(typeof(ConstraintUsage), constraintUsgeString);
            }
            else
            {
                _constraintUsage = ConstraintUsage.Restriction; // default is for restricting attribute values
            }
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

			// Write IsRequired member
			if (_isRequired)
			{
				xmlSchemaElement.MinOccurs = 1;
			}
			else
			{
				xmlSchemaElement.MinOccurs = 0;
			}

			// Write the case style member if it isn't a default value
			if (_caseStyle != CaseStyle.CaseSensitive)
			{
				SetNewteraAttributeValue(NewteraNameSpace.CASE_STYLE, ConvertToCaseStyleString(CaseStyle));	
			}

			// Do not write the Referenced constraint since it is marshalled in
			// SchemaModel

			// Write IsAutoIncrement member, default is false
			if (this._isAutoIncrement)
			{
				SetNewteraAttributeValue(NewteraNameSpace.AUTO_INCREMENT, "true");	
			}

            // Write _autoValueGenerator member
            if (!string.IsNullOrEmpty(_autoValueGenerator))
            {
                SetNewteraAttributeValue(NewteraNameSpace.VALUE_GENERATOR, _autoValueGenerator);
            }

			// Set IsUnique member
			if (_isUnique)
			{
				SetNewteraAttributeValue(NewteraNameSpace.UNIQUE, "true");

				// add an unique constraint for the attribute
				SchemaModel.SchemaBody.AddUniqueConstraint(this);
			}

			// Set IsPrimaryKey member
			if (_isPrimaryKey)
			{
				SetNewteraAttributeValue(NewteraNameSpace.KEY, "true");	
			}

            // Set InlineEditEnabled member
            if (_inlineEditEnabled)
            {
                SetNewteraAttributeValue(NewteraNameSpace.INLINE_EDIT, "true");
            }

            // Set AllowManualUpdate member
            if (!_allowManualUpdate)
            {
                SetNewteraAttributeValue(NewteraNameSpace.MANUAL_UPDATE, "false");
            }

            // Set IsReadOnly member
            if (_isReadOnly)
            {
                SetNewteraAttributeValue(NewteraNameSpace.READ_ONLY, "true");
            }

			// Write constraint member
			if (_refConstraint != null)
			{
				// using constraint as a type
				xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(_refConstraint.Name);
			}
			else
			{
				xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(DataTypeConverter.ConvertToTypeString(_type), "http://www.w3.org/2003/XMLSchema");
			}

			// Write minLength and maxLength if the attribute is string type
			if (_type == DataType.String)
			{
				// Create a XmlSchemaSimpleType object for minLength and maxLength
				// constraint
				xmlSchemaElement.SchemaType = CreateMinMaxLengthSimpleType(_minLength, _maxLength);
			}
			
			// Write defaultValue member
			if (_defaultValue != null && _defaultValue.Length > 0)
			{
				xmlSchemaElement.DefaultValue = _defaultValue;
			}

			// Write IsGoodForFullTextSearch member
			if (_isFullTextSearchAttribute)
			{
				SetNewteraAttributeValue(NewteraNameSpace.GOOD_FOR_FULL_TEXT, "true");	
			}

            // Write IsRichText member
            if (_isRichText)
            {
                SetNewteraAttributeValue(NewteraNameSpace.RICH_TEXT, "true");
            }

            // Write IsHistoryEdit member
            if (_isHistoryEdit)
            {
                SetNewteraAttributeValue(NewteraNameSpace.HISTORY_EDIT, "true");
            }

			// Write IsMultipleLined member
			if (_isMultipleLined)
			{
				SetNewteraAttributeValue(NewteraNameSpace.MULTILINE, "true");
			}

			// Write _rows member, 1 is default number
			if (_rows > 1)
			{
				SetNewteraAttributeValue(NewteraNameSpace.ROWS, _rows.ToString());
			}

            // Write ShowAsProgressBar member
            if (_showAsProgressBar)
            {
                SetNewteraAttributeValue(NewteraNameSpace.SHOW_AS_PROGRESS_BAR, "true");
            }

            // Write ShowUpdateHistory member
            if (_showUpdateHistory)
            {
                SetNewteraAttributeValue(NewteraNameSpace.SHOW_UPDATE_HISTORY, "true");
            }

			// Write IsIndexed member
			if (_isIndexed)
			{
				SetNewteraAttributeValue(NewteraNameSpace.INDEX, "true");
			}

			// Write _usage member
			if (_usage != DefaultViewUsage.Included)
			{
				SetNewteraAttributeValue(NewteraNameSpace.USAGE, Enum.GetName(typeof(AttributeUsage), AttributeUsage.None));	
			}

            // Write _dataSource member
            if (!string.IsNullOrEmpty(_dataSource))
            {
                SetNewteraAttributeValue(NewteraNameSpace.DATA_SOURCE, _dataSource);
            }

            // Write _displayFormat member
            if (!string.IsNullOrEmpty(_displayFormat))
            {
                SetNewteraAttributeValue(NewteraNameSpace.DISPLAY_FORMAT, _displayFormat);
            }

            // Write _inputMask member
            if (!string.IsNullOrEmpty(_inputMask))
            {
                SetNewteraAttributeValue(NewteraNameSpace.INPUT_MASK, _inputMask);
            }

            // Write _keywordFOrmat member
            if (!string.IsNullOrEmpty(_keywordFormat))
            {
                SetNewteraAttributeValue(NewteraNameSpace.KEYWORD_FORMAT, _keywordFormat);
            }

            // Write _operator member
            if (!string.IsNullOrEmpty(_operator))
            {
                SetNewteraAttributeValue(NewteraNameSpace.OPERATOR, ConvertToOperatorValue(_operator));
            }

            // Write _cascadeAttribute member
            if (!string.IsNullOrEmpty(_cascadedAttributeName))
            {
                SetNewteraAttributeValue(NewteraNameSpace.CASCADE_ATTRIBUTE, _cascadedAttributeName);
            }

            // Write _parentAttributeName member
            if (!string.IsNullOrEmpty(_parentAttributeName))
            {
                SetNewteraAttributeValue(NewteraNameSpace.PARENT_ATTRIBUTE, _parentAttributeName);
            }

            // Write _lastIndexedTime member
            if (!string.IsNullOrEmpty(_lastIndexedTime))
            {
                SetNewteraAttributeValue(NewteraNameSpace.INDEXED_TIME, _lastIndexedTime);
            }

            // Write IsEncrypted
            if (IsEncrypted)
            {
                SetNewteraAttributeValue(NewteraNameSpace.IS_ENCRYPTED, "true");
            }

            // Write _constraintUsage member
            if (_constraintUsage != ConstraintUsage.Restriction)
            {
                SetNewteraAttributeValue(NewteraNameSpace.CONSTRAINT_USAGE, Enum.GetName(typeof(ConstraintUsage), _constraintUsage));
            }

			// Always to call this last
			base.Marshal();
		}

		/// <summary>
		/// Create a XmlSchemaSimpleType object for minLength and maxLength facets.
		/// </summary>
		/// <param name="minLength">minimum length</param>
		/// <param name="maxLength">maxinum length</param>
		/// <returns>The XmlSchemaSimpleType object</returns>
		private XmlSchemaSimpleType CreateMinMaxLengthSimpleType(int minLength, int maxLength)
		{
			// create a simple type for the minLength
			XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
			simpleType.Content = restriction;
			XmlSchemaFacet facet = new XmlSchemaMinLengthFacet();
			facet.Value = minLength.ToString();
			restriction.Facets.Add(facet);
			facet = new XmlSchemaMaxLengthFacet();
			facet.Value = maxLength.ToString();
			restriction.Facets.Add(facet);

			return simpleType;
		}

		/// <summary>
		/// Convert case style string to an enumeration value
		/// </summary>
		/// <param name="caseStyle">Case style string</param>
		/// <returns>One of CaseStyle enum values</returns>
		private CaseStyle ConvertToCaseStyleEnum(string caseStyle)
		{
			CaseStyle caseStyleEnum = CaseStyle.CaseSensitive;

            if (!string.IsNullOrEmpty(caseStyle))
            {
                switch (caseStyle)
                {
                    case NewteraNameSpace.CASE_INSENSITIVE:
                        caseStyleEnum = CaseStyle.CaseInsensitive;
                        break;
                    case NewteraNameSpace.CASE_SENSITIVE:
                        caseStyleEnum = CaseStyle.CaseSensitive;
                        break;
                    case NewteraNameSpace.CASE_UPPER:
                        caseStyleEnum = CaseStyle.Upper;
                        break;
                    case NewteraNameSpace.CASE_LOWER:
                        caseStyleEnum = CaseStyle.Lower;
                        break;
                }
            }

			return caseStyleEnum;
		}

		/// <summary>
		/// Convert case style enum value to a string representation
		/// </summary>
		/// <param name="caseStyle">Case style enum value</param>
		/// <returns>The string representation</returns>
		private string ConvertToCaseStyleString(CaseStyle caseStyle)
		{
			string caseStyleStr = NewteraNameSpace.CASE_SENSITIVE;

			switch (caseStyle)
			{
				case CaseStyle.CaseInsensitive:
					caseStyleStr = NewteraNameSpace.CASE_INSENSITIVE;
					break;
				case CaseStyle.CaseSensitive:
					caseStyleStr = NewteraNameSpace.CASE_SENSITIVE;
					break;
				case CaseStyle.Upper:
					caseStyleStr = NewteraNameSpace.CASE_UPPER;
					break;
				case CaseStyle.Lower:
					caseStyleStr = NewteraNameSpace.CASE_LOWER;
					break;
			}

			return caseStyleStr;
		}

        /// <summary>
        /// Convert an operator value to the display text
        /// </summary>
        /// <param name="val">operator value</param>
        /// <returns>The operator display</returns>
        private string ConvertToOperatorDisplay(string val)
        {
            string display = MetaDataModel.OPT_EQUALS;

            switch (val)
            {
                case OPT_EQUALS:
                    display = MetaDataModel.OPT_EQUALS;
                    break;
                case OPT_NOT_EQUALS:
                    display = MetaDataModel.OPT_NOT_EQUALS;
                    break;
                case OPT_LESS_THAN:
                    display = MetaDataModel.OPT_LESS_THAN;
                    break;
                case OPT_GREATER_THAN:
                    display = MetaDataModel.OPT_GREATER_THAN;
                    break;
                case OPT_LESS_THAN_EQUALS:
                    display = MetaDataModel.OPT_LESS_THAN_EQUALS;
                    break;
                case OPT_GREATER_THAN_EQUALS:
                    display = MetaDataModel.OPT_GREATER_THAN_EQUALS;
                    break;
                case OPT_LIKE:
                    display = MetaDataModel.OPT_LIKE;
                    break;
            }

            return display;
        }

        /// <summary>
        /// Convert an operator display text to its value
        /// </summary>
        /// <param name="text">operator display text</param>
        /// <returns>The operator val</returns>
        private string ConvertToOperatorValue(string text)
        {
            string val = OPT_EQUALS;

            switch (text)
            {
                case MetaDataModel.OPT_EQUALS:
                    val = OPT_EQUALS;
                    break;
                case MetaDataModel.OPT_NOT_EQUALS:
                    val = OPT_NOT_EQUALS;
                    break;
                case MetaDataModel.OPT_LESS_THAN:
                    val = OPT_LESS_THAN;
                    break;
                case MetaDataModel.OPT_GREATER_THAN:
                    val = OPT_GREATER_THAN;
                    break;
                case MetaDataModel.OPT_LESS_THAN_EQUALS:
                    val = OPT_LESS_THAN_EQUALS;
                    break;
                case MetaDataModel.OPT_GREATER_THAN_EQUALS:
                    val = OPT_GREATER_THAN_EQUALS;
                    break;
                case MetaDataModel.OPT_LIKE:
                    val = OPT_LIKE;
                    break;
            }

            return val;
        }
	}
}
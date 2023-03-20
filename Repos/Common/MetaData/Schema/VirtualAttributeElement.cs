/*
* @(#)VirtualAttributeElement.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Xml.Schema;
	using System.ComponentModel;
	using System.Drawing.Design;
    using System.Reflection;
    using System.CodeDom.Compiler;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.Schema.Generator;

	/// <summary>
	/// VirtualAttributeElement represents a virtual attribute in a class. A virtual attribute
    /// doesn't correspondes a database table column. It's value is the result of computing based on
    /// values of othere attributes. Virtual attribute is used as result column only, it cannot be
    /// used in any search functioality.
	/// </summary>
	/// 
	/// <version>      1.0.1 25 May 2007
	/// </version>
	public class VirtualAttributeElement : AttributeElementBase
	{
		private string _category = null;
		private string _section = null;
		private DefaultViewUsage _usage = DefaultViewUsage.Included;
		private DataType _type = DataType.Unknown;
		private string _defaultValue = null;
		private ConstraintElementBase _refConstraint = null;
        private bool _isHtml = false;
        private string _classType = null;
        private string _code = "";
        private Assembly _assembly = null; // runtime variable
		
		/// <summary>
		/// Initializing VirtualAttributeElement object.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		public VirtualAttributeElement(string name) : base(name)
		{
		}

		/// <summary>
		/// Initializing VirtualAttributeElement object.
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal VirtualAttributeElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
		}

        /// <summary>
        /// Gets or sets the constraint referenced by the attribute.
        /// </summary>
        /// <value> The referenced constraint</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The code that calculates value of the virtual attribute"),
            EditorAttribute("Newtera.Studio.CodePropertyEditor, Studio", typeof(UITypeEditor)),
        ]
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (value != null)
                {
                    _code = value;
                }
                else
                {
                    _code = "";
                }
            }
        }

        /// <summary>
        /// Gets or sets the class type of the formula
        /// </summary>
        [BrowsableAttribute(false)]
        public string ClassType
        {
            get
            {
                return _classType;
            }
            set
            {
                _classType = value;
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
		/// Gets or sets data type of the attribute.
		/// </summary>
		/// <value>One of DataType enum values. default is String</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("Data type of the attribute value"),
			DefaultValueAttribute(DataType.Unknown)
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
				// DataType.Text is used internaly for full-text search only
				if (value != DataType.Text)
				{
					_type = value;
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
        [BrowsableAttribute(false)]
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
        CategoryAttribute("Appearance"),
        DescriptionAttribute("Describe whether the attribute is part of the default view."),
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
        /// Gets or sets section of the attribute.
        /// </summary>
        /// <value>
        /// A string of section name.
        /// </value>
        [BrowsableAttribute(false)]
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
        /// Gets or sets information indicating whether the attribute is shown as html code
        /// </summary>
        /// <value> true if the attribute is shown as html code, false, otherwise. Default is false.
        /// </value>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Is the attribute value displayed as a html?"),
            DefaultValueAttribute(false)
        ]
        public bool IsHtml
        {
            get
            {
                return _isHtml;
            }
            set
            {
                _isHtml = value;
            }
        }

        /// <summary>
        /// Gets or sets the creator that creates a customized web control for the attribute
        /// </summary>
        /// <remarks>Invalid for virtual attribute</remarks>
        [BrowsableAttribute(false)]
        public override string UIControlCreator
        {
            get
            {
                return base.UIControlCreator;
            }
            set
            {
                base.UIControlCreator = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether to invoke call back function code defined for the class
        /// </summary>
        /// <remarks>Invalid for virtual attribute</remarks>
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
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
            visitor.VisitVirtualAttributeElement(this);
		}

        /// <summary>
        /// Create an IFormula object for the virtual attribute
        /// </summary>
        /// <returns>An IFormula object</returns>
        public IFormula CreateFormula()
        {
            // compile the code defined for the virtual attribute into an assembly
            if (_assembly == null)
            {
                _assembly = CompileCode();
            }

            string typeName = NewteraNameSpace.FORMULA_NAME_SPACE + "." + this.ClassType;

            return (IFormula)_assembly.CreateInstance(typeName);
        }

        /// <summary>
        /// Compile the code defined in a virtual attribute into an assembly
        /// and place the assembly in the memory
        /// </summary>
        /// <returns>The compiled assembly</returns>
        private Assembly CompileCode()
        {
            Assembly assembly = null;
            ScriptLanguage language = ScriptLanguage.CSharp; // TODO, handle the scripts in different languages

            // compile the code into an assembly in memory
            string libPath = NewteraNameSpace.GetAppHomeDir() + @"bin\";
            CompilerResults cr = CodeStubGenerator.Instance.CompileFromSource(language, this.Code, libPath);

            if (cr.Errors.Count > 0)
            {
                throw new SchemaModelException("There are errors while compiling the code of the virtual attribute " + this.Caption + " with error message " + cr.Errors[0].ErrorText);
            }
            else
            {
                assembly = cr.CompiledAssembly;
            }

            return assembly;
        }
		
		/// <summary>
		/// Return the name of the VirtualAttributeElement
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
		internal override void Unmarshal()
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) XmlSchemaElement;

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

            // Set _code member, code string is stored as an annotation
            _code = this.GetNewteraAppInfoContent(NewteraNameSpace.CODE);

            // Set _classType member
            _classType = this.GetNewteraAttributeValue(NewteraNameSpace.CLASS_TYPE);

			// Set _section member
			_section = this.GetNewteraAttributeValue(NewteraNameSpace.SECTION);

			// Set _category member
			_category = this.GetNewteraAttributeValue(NewteraNameSpace.CATEGORY);

            // Set _usage member
            string usgeString = this.GetNewteraAttributeValue(NewteraNameSpace.USAGE);
            if (usgeString != null && usgeString.Length > 0)
            {
                AttributeUsage val = (AttributeUsage)Enum.Parse(typeof(AttributeUsage), usgeString);
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

            // Set SHOW_AS_HTML member
            string status = this.GetNewteraAttributeValue(NewteraNameSpace.SHOW_AS_HTML);
            _isHtml = (status != null && status == "true" ? true : false);

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
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		internal override void Marshal()
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) XmlSchemaElement;

            // Write _code member
            if (_code != null)
            {
                SetNewteraAppInfoContent(NewteraNameSpace.CODE, _code);
            }

            // Write _classType member
            if (!string.IsNullOrEmpty(_classType))
            {
                SetNewteraAttributeValue(NewteraNameSpace.CLASS_TYPE, _classType);
            }

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

            // Write _usage member
            if (_usage != DefaultViewUsage.Included)
            {
                SetNewteraAttributeValue(NewteraNameSpace.USAGE, Enum.GetName(typeof(AttributeUsage), AttributeUsage.None));
            }

            // Write ShowAsProgressBar member
            if (_isHtml)
            {
                SetNewteraAttributeValue(NewteraNameSpace.SHOW_AS_HTML, "true");
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

			// Write defaultValue member
			if (_defaultValue != null && _defaultValue.Length > 0)
			{
				xmlSchemaElement.DefaultValue = _defaultValue;
			}

			// Always to call this last
			base.Marshal();
		}
	}
}
/*
* @(#)ImageAttributeElement.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
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
	/// ImageAttributeElement represents an attribute that store an image.
	/// </summary>
	/// <version>1.0.1 04 Jul 2008</version>
	public class ImageAttributeElement : AttributeElementBase
	{
		private string _category = null;
		private string _section = null;
        private DefaultViewUsage _usage = DefaultViewUsage.Included;
        private int _height = 400;
        private int _width = 600;
        private int _thumbnailHeight = 16;
        private int _thumbnailWidth = 16;

		/// <summary>
		/// Initializing ImageAttributeElement object.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		public ImageAttributeElement(string name) : base(name)
		{
		}

		/// <summary>
		/// Initializing ImageAttributeElement object.
		/// </summary>
		/// <param name="xmlSchemaElement">The xml schema element</param>
		internal ImageAttributeElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
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
        /// Gets or sets maximum height of displayed image.
        /// </summary>
        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("The maximum height of image."),
        DefaultValueAttribute(400)
        ]
        public int MaximumHeight
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        /// <summary>
        /// Gets or sets maximum width of displayed image.
        /// </summary>
        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("The maximum width of image."),
        DefaultValueAttribute(600)
        ]
        public int MaximumWidth
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Gets or sets height of thumbnail image.
        /// </summary>
        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("The height of thumbnail."),
        DefaultValueAttribute(16)
        ]
        public int ThumbnailHeight
        {
            get
            {
                return _thumbnailHeight;
            }
            set
            {
                _thumbnailHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets width of thumbnail image.
        /// </summary>
        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("The width of thumbnail."),
        DefaultValueAttribute(16)
        ]
        public int ThumbnailWidth
        {
            get
            {
                return _thumbnailWidth;
            }
            set
            {
                _thumbnailWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether to invoke call back function code defined for the class
        /// </summary>
        /// <value> return true if to invoke callback, false otherwise. The default is false.</value>
        [BrowsableAttribute(false)]
        public override bool InvokeCallback
        {
            get
            {
                return false;
            }
            set
            {
              
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
                return DataType.String;
            }
            set
            {
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
                return 100;
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
			visitor.VisitImageAttributeElement(this);
		}
		
		/// <summary>
		/// Return the name of the ImageAttributeElement
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

            // Set _height and _width member
            string str = this.GetNewteraAttributeValue(NewteraNameSpace.HEIGHT);
            if (!string.IsNullOrEmpty(str))
            {
                _height = int.Parse(str);
            }

            str = this.GetNewteraAttributeValue(NewteraNameSpace.WIDTH);
            if (!string.IsNullOrEmpty(str))
            {
                _width = int.Parse(str);
            }

            str = this.GetNewteraAttributeValue(NewteraNameSpace.THEIGHT);
            if (!string.IsNullOrEmpty(str))
            {
                _thumbnailHeight = int.Parse(str);
            }

            str = this.GetNewteraAttributeValue(NewteraNameSpace.TWIDTH);
            if (!string.IsNullOrEmpty(str))
            {
                _thumbnailWidth = int.Parse(str);
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

            // Write _usage member
            if (_usage != DefaultViewUsage.Included)
            {
                SetNewteraAttributeValue(NewteraNameSpace.USAGE, Enum.GetName(typeof(AttributeUsage), AttributeUsage.None));
            }

            // Write _height and _width
            SetNewteraAttributeValue(NewteraNameSpace.HEIGHT, _height.ToString());

            SetNewteraAttributeValue(NewteraNameSpace.WIDTH, _width.ToString());

            SetNewteraAttributeValue(NewteraNameSpace.THEIGHT, _thumbnailHeight.ToString());

            SetNewteraAttributeValue(NewteraNameSpace.TWIDTH, _thumbnailWidth.ToString());

			// Always to call this last
			base.Marshal();
		}
    }
}
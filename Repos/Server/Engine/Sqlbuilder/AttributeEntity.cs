/*
* @(#)AttributeEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;
    using System.Text.RegularExpressions;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// An AttributeEntity object represents a simple or array attribute of a class in an object-relational
	/// data model. An attribute is owned by one and only one class and stores
	/// information about how an attribute is mapped to a relational database column.
	/// </summary>
	/// <version>  	1.0.0 08 Jul 2003 </version>
	/// <author> Yong Zhang</author>
	public abstract class AttributeEntity : DBEntity
	{
		/* private instance variables */
		private AttributeElementBase _attributeElement; // The meta model element for attribute
		private string _searchValue; // A search value
		private string _operator; // The boolean operator for the attribute
		private DataType _type; // The attribute type
		private bool _isReferenced;
        private ClassEntity _baseClass; // the base class
		
		/// <summary>
		/// Initializes a new instance of the AttributeEntity class
		/// </summary>
		/// <param name="element">the attribute element in meta model</param>
		public AttributeEntity(AttributeElementBase element) : base()
		{
			_attributeElement = element;
			_searchValue = null;
			_operator = null;
			_type = DataType.Unknown;
			_isReferenced = false; // default value
            _baseClass = null;
		}

		/// <summary>
		/// Gets the attribute name
		/// </summary>
		/// <value> the attribute name</value>
		public override string Name
		{
			get
			{
				return _attributeElement.Name;
			}
		}

		/// <summary>
		/// Gets type of attribute
		/// </summary>
		/// <value> the attribute type </value>
		public override DataType Type
		{
			get
			{
				if (_type == DataType.Unknown)
				{
					_type = _attributeElement.DataType;
				}
				
				return _type;
			}
			
		}

        /// <summary>
        /// Gets or sets the base class entity. Base class entity represents the leaf class of
        /// the class entity inheritance tree
        /// </summary>
        /// <value> The base class entity</value>
        public ClassEntity BaseClassEntity
        {
            get
            {
                return _baseClass;
            }
            set
            {
                _baseClass = value;
            }
        }

		/// <summary>
		/// Get SchemaModel element for this AttributeEntity object. metaData element
		/// provides meta data information about this attribute.
		/// </summary>
		/// <returns> a AttributeElementBase object</returns>
		public AttributeElementBase SchemaModelElement
		{
			get
			{
				return _attributeElement;
			}
		}

		/// <summary>
		/// Get the DB column name for the attribute. The column name is provided by meta model
		/// element
		/// </summary>
		/// <value> the column name for the attribute</returns>
		public override string ColumnName
		{
			get
			{
				return _attributeElement.ColumnName;
			}	
		}

        /// <summary>
        /// Gets information  indicating whether the attribute is browsable
        /// element
        /// </summary>
        /// <value>True if browsable, false otherwise</returns>
        public bool IsBrowsable
        {
            get
            {
                return _attributeElement.IsBrowsable;
            }
        }

        /// <summary>
        /// Gets the case style for the attribute. The case style is provided by schema model
        /// element.
        /// </summary>
        /// <value> One of the CaseStyle enum values.</returns>
        public abstract CaseStyle CaseStyle {get;}

		/// <summary>
		/// Gets or sets information indicating whether the attribute is required.
		/// </summary>
		/// <value> return true if attribute is required, false otherwise. The default is false.</value>
		public abstract bool IsRequired {get;}

		/// <summary>
		/// Gets or sets information to indicate whether the attribute is auto-increment.
		/// </summary>
		/// <value>
		/// true if it is auto-increment; otherwise false. default is false.
		/// </value>
		public abstract bool IsAutoIncrement {get;}

        /// <summary>
        /// Gets information indicating whether this attribute is history edit
        /// searchable.
        /// </summary>
        /// <value>
        /// true if it is history edit, false otherwise. Default is false.
        /// </value>
        public abstract bool IsHistoryEdit { get;}

        /// <summary>
        /// Gets information indicating whether this attribute is rich text
        /// searchable.
        /// </summary>
        /// <value>
        /// true if it is rich text, false otherwise. Default is false.
        /// </value>
        public abstract bool IsRichText { get; }

        /// <summary>
        /// Gets information indicating whether this attribute has an input mask.
        /// </summary>
        /// <value>
        /// true if it has an input mask, false otherwise.
        /// </value>
        public virtual bool HasInputMask
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute has a display format.
        /// </summary>
        /// <value>
        /// true if it has a display format, false otherwise.
        /// </value>
        public virtual bool HasDisplayFormat
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute's value is encrypted.
        /// </summary>
        /// <value>
        /// true if it's value is encrypted, false otherwise.
        /// </value>
        public virtual bool IsEncrypted
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Gets the information indicating whether this attribute has enum constraint
		/// </summary>
		/// <value>true if this attribute has enum constraint, false otherwise.</value>
		public abstract bool IsEnum {get;}

        /// <summary>
        /// Gets the enum values of an enum constraint
        /// </summary>
        /// <value>A collection of enum values.</value>
        public virtual EnumValueCollection EnumValues
        {
            get
            {
                return new EnumValueCollection();
            }
        }

		/// <summary>
		/// Gets the information indicating whether this attribute has enumerated
		/// values with multiple selection
		/// </summary>
		/// <value>true if this attribute has enmerated values with multiple selections, false otherwise.</value>
        public virtual bool IsMultipleChoice
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Gets the default value of the attribute.
		/// </summary>
		/// <value> the default value </value>
		public abstract string DefaultValue {get;}

		/// <summary>
		/// Gets or sets the attribute's searchValue.
		/// </summary>
		/// <value> The search value</value>
		public string SearchValue
		{
			get
			{
				return _searchValue;
			}
			set
			{
				_searchValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the operator.
		/// </summary>
		/// <value> The operator</value>
		/// <example>such as "=", "<", ">", "<=", ">=", etc</example>
		public string Operator
		{
			get
			{
				return _operator;
			}
			set
			{
				_operator = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the attribute has been
		/// referenced explicitly in a XQuery.
		/// </summary>
		/// <value>true if it has been referenced, false otherwise.</value>
		public bool IsReferenced
		{
			get
			{
				return _isReferenced;
			}
			set
			{
				_isReferenced = value;
			}
		}
		
		/// <summary>
		/// Accept a EntityVistor to visit itself.
		/// </summary>
		/// <param name="visitor">the visiting visitor</param>
		public override void Accept(EntityVisitor visitor)
		{
			// visit itself
			visitor.VisitAttribute(this);
		}
		
		/// <summary>
		/// Gets information if the attribute has a default value.
		/// </summary>
		/// <returns>
		/// true if it has, otherwise, return false
		/// </returns>
		public abstract bool HasDefaultValue();

        /// <summary>
        /// Gets information if the attribute has an unique id as default value.
        /// </summary>
        /// <returns>
        /// true if it has, otherwise, return false
        /// </returns>
        public virtual bool HasUidAsDefault()
        {
            return false;
        }

        /// <summary>
        /// Convert an enum value to its display text.
        /// </summary>
        /// <param name="val">An enum value.</param>
        /// <returns>The corresponding display text for the enum value</returns>
        public virtual string ConvertToEnumDisplayText(string val)
        {
            return EnumValues.ConvertToEnumDisplayText(val);
        }

		/// <summary>
		/// Convert an enum display text to its value.
		/// </summary>
		/// <param name="text">An enum display text.</param>
		/// <returns>The corresponding enum value</returns>
        public virtual string ConvertToEnumValue(string text)
        {
            return EnumValues.ConvertToEnumValue(text);
        }

		/// <summary>
		/// Convert an integer whose bits representing multiple enum values to a "|" separated string.
		/// </summary>
		/// <param name="val">An integer whose bits representing multiple enum values.</param>
		/// <returns>a "|" separated enum value string</returns>
        public virtual string ConvertToEnumString(int val)
        {
            int bit = 1; // initialize it to the first enum value
            string converted = "";
            if (IsMultipleChoice)
            {
                EnumValueCollection enumValues = this.EnumValues;
                foreach (EnumValue enumValue in enumValues)
                {
                    int result = val & bit;
                    if (result != 0)
                    {
                        if (converted.Length > 0)
                        {
                            converted = converted + EnumElement.SEPARATOR + enumValue.DisplayText;
                        }
                        else
                        {
                            converted = enumValue.DisplayText;
                        }
                    }

                    bit = bit << 1; // shift the bit to next enum value
                }
            }

            return converted;
        }

		/// <summary>
		/// Convert a "|" separated enum value string to an integer whose bits representing multiple enum values.
		/// </summary>
		/// <param name="val">a "|" separated enum value string</param>
		/// <returns>An integer whose bits representing multiple enum values.</returns>
        public virtual int ConvertToEnumInteger(string val)
        {
            int converted = 0;

            if (IsMultipleChoice)
            {
                int bit = 1; // initialize it to the first enum value
                string[] values = GetStringArray(val);
                EnumValueCollection enumValues = this.EnumValues;
                foreach (EnumValue enumValue in enumValues)
                {
                    if (Contains(enumValue.DisplayText, values))
                    {
                        converted = converted | bit;
                    }

                    bit = bit << 1; // shift the bit to next enum value
                }
            }

            return converted;
        }

        /// <summary>
        /// Convert a string into a masked string using a mask.
        /// </summary>
        /// <param name="val">unmasked string</param>
        /// <returns>The masked string</returns>
        public virtual String ConvertToMaskedString(string val)
        {
            return val;
        }

        /// <summary>
        /// Convert a masked string into an unmasked string using a mask.
        /// </summary>
        /// <param name="val">masked string</param>
        /// <returns>The unmasked string</returns>
        public virtual String ConvertToUnmaskedString(string val)
        {
            return val;
        }

        /// <summary>
        /// Format a numeric value
        /// </summary>
        /// <param name="val">A numeric value</param>
        /// <returns>The formated string</returns>
        public virtual String FormatValue(string val)
        {
            return val;
        }

        /// <summary>
        /// Convert to an encrypted string.
        /// </summary>
        /// <param name="val">a string</param>
        /// <returns>The encrypted string</returns>
        public virtual String ConvertToEncrytedString(string val)
        {
            return val;
        }

        /// <summary>
        /// Convert to a decrypted string.
        /// </summary>
        /// <param name="val">a string</param>
        /// <returns>The decrypted string</returns>
        public virtual String ConvertToDecryptedString(string val)
        {
            return val;
        }

        /// <summary>
        /// convert a string of "|' separated enum values into an array of strings.
        /// </summary>
        /// <param name="val">a string of "|" separated enum values</param>
        /// <returns>An array of strings</returns>
        private string[] GetStringArray(string val)
        {
            if (val.Length > 0)
            {
                Regex exp = new Regex(EnumElement.SEPARATOR);

                return exp.Split(val);
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Gets the information indicating whether a string exists in an array of strings.
        /// </summary>
        /// <param name="val">The given string</param>
        /// <param name="values">An array of strings</param>
        /// <returns>true if it exists, false otherwise.</returns>
        private bool Contains(string val, string[] values)
        {
            bool status = false;

            for (int i = 0; i < values.Length; i++)
            {
                if (val == values[i])
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the AttributeEntity
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
                // the base class is the leaf class of the inheritance tree
                return BaseClassEntity;
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
	}
}
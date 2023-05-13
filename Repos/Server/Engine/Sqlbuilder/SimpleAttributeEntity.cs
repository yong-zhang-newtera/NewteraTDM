/*
* @(#)SimpleAttributeEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
    using System.Text;
	using System.Collections;

    using Newtera.Server.DB;
    using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// An SimpleAttributeEntity object represents a simple attribute of a class in an object-relational
	/// data model.
	/// </summary>
	/// <version>  	1.0.0 08 Aug 2004 </version>
	public class SimpleAttributeEntity : AttributeEntity
	{
        private char[] PlaceHolderChars = new char[]{'0', '9', 'A', 'a', 'L', 'l'};
        private const string WildChar = "%";

		/* private instance variables */
		private SimpleAttributeElement _attributeElement; // The meta model element for attribute
		
		/// <summary>
		/// Initializes a new instance of the SimpleAttributeEntity class
		/// </summary>
		/// <param name="element">the attribute element in meta model</param>
		public SimpleAttributeEntity(SimpleAttributeElement element) : base(element)
		{
			_attributeElement = element;
		}

		/// <summary>
		/// Gets the case style for the attribute. The case style is provided by schema model
		/// element.
		/// </summary>
		/// <value> One of the CaseStyle enum values.</returns>
		public override CaseStyle CaseStyle
		{
			get
			{
				return _attributeElement.CaseStyle;
			}	
		}

		/// <summary>
		/// Gets or sets information indicating whether the attribute is required.
		/// </summary>
		/// <value> return true if attribute is required, false otherwise. The default is false.</value>
		public override bool IsRequired
		{
			get
			{
				return _attributeElement.IsRequired;
			}
		}

		/// <summary>
		/// Gets or sets information to indicate whether the attribute is auto-increment.
		/// </summary>
		/// <value>
		/// true if it is auto-increment; otherwise false. default is false.
		/// </value>
		public override bool IsAutoIncrement
		{
			get
			{
				return _attributeElement.IsAutoIncrement;
			}
		}

        /// <summary>
        /// Gets information indicating whether this attribute is history edit attribute.
        /// </summary>
        /// <value>
        /// true if it is history edit, false otherwise. Default is false.
        /// </value>
        public override bool IsHistoryEdit
        {
            get
            {
                return _attributeElement.IsHistoryEdit;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute is rich text attribute.
        /// </summary>
        /// <value>
        /// true if it is rich text, false otherwise. Default is false.
        /// </value>
        public override bool IsRichText
        {
            get
            {
                return _attributeElement.IsRichText;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute has an input mask.
        /// </summary>
        /// <value>
        /// true if it has an input mask, false otherwise.
        /// </value>
        public override bool HasInputMask
        {
            get
            {
                bool status = false;
                if (_attributeElement.InputMask != null &&
                    _attributeElement.InputMask.Trim().Length > 0)
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute has a display format.
        /// </summary>
        /// <value>
        /// true if it has a display format, false otherwise.
        /// </value>
        public override bool HasDisplayFormat
        {
            get
            {
                bool status = false;
                if (_attributeElement.DisplayFormatString != null &&
                    _attributeElement.DisplayFormatString.Trim().Length > 0)
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute's value is encrypted.
        /// </summary>
        /// <value>
        /// true if it's value is encrypted, false otherwise.
        /// </value>
        public override bool IsEncrypted
        {
            get
            {
                return _attributeElement.IsEncrypted;
            }
        }

		/// <summary>
		/// Gets the information indicating whether this attribute has enum constraint
		/// </summary>
		/// <value>true if this attribute has enum constraint, false otherwise.</value>
		public override bool IsEnum
		{
			get
			{
				if (_attributeElement.Constraint != null &&
					_attributeElement.Constraint is IEnumConstraint)
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
		/// Gets the information indicating whether this attribute has enumerated
		/// values with multiple selection
		/// </summary>
		/// <value>true if this attribute has enmerated values with multiple selections, false otherwise.</value>
		public override bool IsMultipleChoice
		{
			get
			{
				if (_attributeElement.IsMultipleChoice)
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
		/// Gets the default value of the attribute.
		/// </summary>
		/// <value> the default value </value>
		public override string DefaultValue
		{
			get
			{
				string defaultValue = _attributeElement.DefaultValue;
				if (defaultValue != null && defaultValue.Equals(""))
				{
					defaultValue = null;
				}
				
				return defaultValue;
			}
		}

        /// <summary>
        /// Gets information if the attribute has an unique id as default value.
        /// </summary>
        /// <returns>
        /// true if it has, otherwise, return false
        /// </returns>
        public override bool HasUidAsDefault()
        {
            bool hasUIDAsDefault = false;
            string defaultValue = _attributeElement.DefaultValue;
            if (_attributeElement.IsUidDefault)
            {
                hasUIDAsDefault = true;
            }

            return hasUIDAsDefault;
        }

        /// <summary>
        /// Gets information if the attribute has a default value.
        /// </summary>
        /// <returns>
        /// true if it has, otherwise, return false
        /// </returns>
        public override bool HasDefaultValue()
		{
			bool hasDefaultValue = true;
			string defaultValue = _attributeElement.DefaultValue;
			if (string.IsNullOrEmpty(defaultValue))
			{
				hasDefaultValue = false;
			}
			
			return hasDefaultValue;
		}

        /// <summary>
        /// Gets the enum values of an enum constraint
        /// </summary>
        /// <value>A collection of enum values.</value>
        public override EnumValueCollection EnumValues
        {
            get
            {
                if (IsEnum)
                {
                    if (_attributeElement.Constraint is ListElement)
                    {
                        // try to get the enum values from a cache since it may be very
                        // expensive to get enum values from list hangdler
                        EnumValueCollection enumValues = EnumTypeFactory.Instance.GetEnumValues(_attributeElement, (ListElement)_attributeElement.Constraint);

                        return enumValues;
                    }
                    else
                    {
                        return ((IEnumConstraint)_attributeElement.Constraint).Values;
                    }
                }
                else
                {
                    return new EnumValueCollection();
                }
            }
        }

        /// <summary>
        /// Convert a string into a masked string using a mask.
        /// </summary>
        /// <param name="val">unmasked string</param>
        /// <returns>The masked string</returns>
        public override String ConvertToMaskedString(string val)
        {
            StringBuilder builder = new StringBuilder();
            string mask = _attributeElement.InputMask;
            int pos = 0;
            if (!string.IsNullOrEmpty(val))
            {
                for (int i = 0; i < mask.Length; i++)
                {
                    if (IsPlaceHolderChar(mask[i]))
                    {
                        if (pos < val.Length)
                        {
                            builder.Append(val[pos++]);
                        }
                    }
                    else
                    {
                        builder.Append(mask[i]);
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert a masked string into an unmasked string using a mask.
        /// </summary>
        /// <param name="val">masked string</param>
        /// <returns>The unmasked string</returns>
        public override String ConvertToUnmaskedString(string val)
        {
            StringBuilder builder = new StringBuilder();
            Hashtable formattingChars = new Hashtable();

            string mask = _attributeElement.InputMask;
            int pos = 0;
            if (!string.IsNullOrEmpty(val))
            {
                for (int i = 0; i < mask.Length; i++)
                {
                    if (IsPlaceHolderChar(mask[i]))
                    {
                        if (pos < val.Length)
                        {
                            builder.Append(val[pos++]);
                        }
                    }
                    else
                    {
                        if (formattingChars[mask[i]] == null)
                        {
                            formattingChars[mask[i]] = "1"; // remember the formatting chars
                        }
                        pos++;
                    }
                }

                // if the val ends with a wildchar, remove the formatting chars from the value
                if (val.EndsWith(WildChar))
                {
                    builder = new StringBuilder();
                    for (int i = 0; i < val.Length; i++)
                    {
                        if (formattingChars[val[i]] == null)
                        {
                            builder.Append(val[i]);
                        }
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert to an encrypted string.
        /// </summary>
        /// <param name="val">a string</param>
        /// <returns>The encrypted string</returns>
        public override String ConvertToEncrytedString(string val)
        {
            // encrypt the val before storing it to database
            string encrypted = val;

            if (!string.IsNullOrEmpty(val))
            {
                encrypted = TextDeEncryptor.Instance.Encrypt(val);
            }

            return encrypted;
        }

        /// <summary>
        /// Convert to a decrypted string.
        /// </summary>
        /// <param name="val">a string</param>
        /// <returns>The decrypted string</returns>
        public override String ConvertToDecryptedString(string val)
        {
            string decrypted = val;

            if (!string.IsNullOrEmpty(val))
            {
                try
                {
                    decrypted = TextDeEncryptor.Instance.Decrypt(val);
                }
                catch (Exception)
                {
                    // value stored previously may not be encrypted, therefore,
                    // return the original val
                    decrypted = val;
                }
            }

            return decrypted;
        }

        /// <summary>
        /// Format a numeric value
        /// </summary>
        /// <param name="val">A numeric value</param>
        /// <returns>The formated string</returns>
        public override String FormatValue(string val)
        {
            string formatedVal = val;
            
            string displayFormat = _attributeElement.DisplayFormatString;
            try
            {
                switch (_attributeElement.DataType)
                {
                    case DataType.Integer:
                        formatedVal = int.Parse(val).ToString(displayFormat);
                        break;
                    case DataType.Float:
                        formatedVal = float.Parse(val).ToString(displayFormat);

                        break;

                    case DataType.Double:
                        formatedVal = double.Parse(val).ToString(displayFormat);

                        break;

                    case DataType.Decimal:
                        formatedVal = decimal.Parse(val).ToString(displayFormat);

                        break;
                }
            }
            catch (Exception)
            {
                formatedVal = val;
            }

            return formatedVal;
        }

        private bool IsPlaceHolderChar(char c)
        {
            bool status = false;

            foreach (char placeHolderChar in PlaceHolderChars)
            {
                if (c == placeHolderChar)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }
	}
}
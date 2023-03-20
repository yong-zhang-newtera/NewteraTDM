/*
* @(#)VirtualAttributeEntity.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;

    using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// An VirtualAttributeEntity object represents a virtual attribute of a class in an object-relational
	/// data model.
	/// </summary>
	/// <version>  	1.0.0 27 May 2007 </version>
	public class VirtualAttributeEntity : AttributeEntity
	{
		/* private instance variables */
		private VirtualAttributeElement _attributeElement; // The meta model element for attribute
		
		/// <summary>
		/// Initializes a new instance of the VirtualAttributeEntity class
		/// </summary>
		/// <param name="element">the attribute element in meta model</param>
		public VirtualAttributeEntity(VirtualAttributeElement element) : base(element)
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
                return CaseStyle.CaseInsensitive;
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
                return false;
            }
        }

        /// <summary>
        /// Gets or sets information to indicate whether the attribute is auto-increment.
        /// </summary>
        /// <value>
        /// always false.
        /// </value>
        public override bool IsAutoIncrement
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute is full-text
        /// searchable.
        /// </summary>
        /// <value>
        /// Alwayse false.
        /// </value>
        public override bool IsFullTextSearchable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute is history edit attribute.
        /// </summary>
        /// <value>
        /// Alwayse false.
        /// </value>
        public override bool IsHistoryEdit
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets information indicating whether this attribute is rich text attribute.
        /// </summary>
        /// <value>
        /// Alwayse false.
        /// </value>
        public override bool IsRichText
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
        /// Gets information if the attribute has a default value.
        /// </summary>
        /// <returns>
        /// true if it has, otherwise, return false
        /// </returns>
        public override bool HasDefaultValue()
        {
            bool hasDefaultValue = true;
            string defaultValue = _attributeElement.DefaultValue;
            if (defaultValue == null || defaultValue.Equals(""))
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
	}
}
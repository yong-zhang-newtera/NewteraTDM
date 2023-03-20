/*
* @(#)ImageAttributeEntity.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// An ImageAttributeEntity object represents an image attribute of a class in an object-relational
	/// data model.
	/// </summary>
	/// <version>  	1.0.0 04 Jul 2008 </version>
	public class ImageAttributeEntity : AttributeEntity
	{
		/* private instance variables */
		private ImageAttributeElement _attributeElement; // The meta model element for attribute
		
		/// <summary>
		/// Initializes a new instance of the ImageAttributeEntity class
		/// </summary>
		/// <param name="element">the attribute element in meta model</param>
		public ImageAttributeEntity(ImageAttributeElement element) : base(element)
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
				return false;
			}
		}

		/// <summary>
		/// Gets the default value of the attribute.
		/// </summary>
		/// <value> the null value </value>
		public override string DefaultValue
		{
			get
			{	
				return null;
			}
		}
		
		/// <summary>
		/// Gets information if the attribute has a default value.
		/// </summary>
		/// <returns>
		/// alwayse false
		/// </returns>
		public override bool HasDefaultValue()
		{	
			return false;
		}

		/// <summary>
		/// Convert an enum value to its display text.
		/// </summary>
		/// <param name="val">An enum value.</param>
		/// <returns>The corresponding display text for the enum value</returns>
		public override string ConvertToEnumDisplayText(string val)
		{
			return "";
		}

		/// <summary>
		/// Convert an enum display text to its value.
		/// </summary>
		/// <param name="text">An enum display text.</param>
		/// <returns>The corresponding enum value</returns>
		public override string ConvertToEnumValue(string text)
		{
			return "";
		}

		/// <summary>
		/// Convert an integer whose bits representing multiple enum values to a "|" separated string.
		/// </summary>
		/// <param name="val">An integer whose bits representing multiple enum values.</param>
		/// <returns>a "|" separated enum value string</returns>
		public override string ConvertToEnumString(int val)
		{
			return "";
		}

		/// <summary>
		/// Convert a "|" separated enum value string to an integer whose bits representing multiple enum values.
		/// </summary>
		/// <param name="val">a "|" separated enum value string</param>
		/// <returns>An integer whose bits representing multiple enum values.</returns>
		public override int ConvertToEnumInteger(string val)
		{
			return 0;
		}
	}
}
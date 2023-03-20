/*
* @(#)ImageEditElement.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Represents an element for DataImageAttribute appeared in InlinedXml clause
	/// for editing purpose.
	/// </summary>
	/// <version>  	1.0.0 04 Jul 2008</version>
	internal class ImageEditElement : QueryElementBase
	{
		private DataImageAttribute _imageAttribute;

		/// <summary>
		/// Initiating an instance of ImageEditElement class
		/// </summary>
        /// <param name="imageAttribute">The image attribute</param>
        public ImageEditElement(DataImageAttribute imageAttribute)
            : base()
		{
            _imageAttribute = imageAttribute;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();
			bool isNullValue = false;

            if (string.IsNullOrEmpty(_imageAttribute.AttributeValue))
			{
				isNullValue = true;
			}

            query.Append("<").Append(_imageAttribute.Name);

			if (isNullValue)
			{
				query.Append(" xsi:nil=\"true\"/>\n");
			}
			else
			{
                query.Append(">").Append(EscapeChars(_imageAttribute.AttributeValue));
                query.Append("</").Append(_imageAttribute.Name).Append(">\n");
			}

			return query.ToString();
		}
	}
}
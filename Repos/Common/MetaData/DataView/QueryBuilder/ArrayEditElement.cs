/*
* @(#)ArrayEditElement.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Represents an element for DataArrayAttribute appeared in InlinedXml clause
	/// for editing purpose.
	/// </summary>
	/// <version>  	1.0.0 11 Aug 2004</version>
	/// <author>  Yong Zhang </author>
	internal class ArrayEditElement : QueryElementBase
	{
		private DataArrayAttribute _arrayAttribute;

		/// <summary>
		/// Initiating an instance of ArrayEditElement class
		/// </summary>
		/// <param name="simpleAttribute">The simple attribute</param>
		public ArrayEditElement(DataArrayAttribute simpleAttribute) : base()
		{
			_arrayAttribute = simpleAttribute;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();
			bool isNullValue = false;

			if (string.IsNullOrEmpty(_arrayAttribute.AttributeValue))
			{
				isNullValue = true;
			}

			query.Append("<").Append(_arrayAttribute.Name);

			if (isNullValue)
			{
				query.Append(" xsi:nil=\"true\"/>\n");
			}
			else
			{
				query.Append(">").Append(EscapeChars(_arrayAttribute.AttributeValue));
				query.Append("</").Append(_arrayAttribute.Name).Append(">\n");
			}

			return query.ToString();
		}
	}
}
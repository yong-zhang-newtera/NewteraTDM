/*
* @(#)SimpleEditElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Represents an element for DataSimpleAttribute appeared in InlinedXml clause
	/// for editing purpose.
	/// </summary>
	/// <version>  	1.0.0 11 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class SimpleEditElement : QueryElementBase
	{
		private DataSimpleAttribute _simpleAttribute;

		/// <summary>
		/// Initiating an instance of SimpleEditElement class
		/// </summary>
		/// <param name="simpleAttribute">The simple attribute</param>
		public SimpleEditElement(DataSimpleAttribute simpleAttribute) : base()
		{
			_simpleAttribute = simpleAttribute;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();
			bool isNullValue = false;

			if (string.IsNullOrEmpty(_simpleAttribute.AttributeValue))
			{
				isNullValue = true;
			}

			query.Append("<").Append(_simpleAttribute.Name);

			if (isNullValue)
			{
				query.Append(" xsi:nil=\"true\"/>\n");
			}
			else
			{
				query.Append(">").Append(EscapeChars(_simpleAttribute.AttributeValue));
				query.Append("</").Append(_simpleAttribute.Name).Append(">\n");
			}

			return query.ToString();
		}
	}
}
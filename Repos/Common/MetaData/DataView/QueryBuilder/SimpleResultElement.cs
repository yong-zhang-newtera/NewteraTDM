/*
* @(#)SimpleResultElement.cs
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
	/// Represents a root xml element in return clause of a XQuery.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class SimpleResultElement : QueryElementBase
	{
		private DataSimpleAttribute _simpleAttribute;

		/// <summary>
		/// Initiating an instance of SimpleResultElement class
		/// </summary>
		/// <param name="simpleAttribute">The attribute</param>
		public SimpleResultElement(DataSimpleAttribute simpleAttribute) : base()
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

			if (_simpleAttribute.HasFunction)
			{
				query.Append("<").Append(_simpleAttribute.Caption).Append(">");

			}

			query.Append("{").Append(((IQueryElement) _simpleAttribute).ToXQuery()).Append("}");

			if (_simpleAttribute.HasFunction)
			{
				query.Append("</").Append(_simpleAttribute.Caption).Append(">");
			}

			query.Append("\n");

			return query.ToString();
		}
	}
}
/*
* @(#)RelationshipEditElement.cs
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
	/// Represents an element for DataRelationshipAttribute appeared in InlinedXml clause
	/// for editing purpose.
	/// </summary>
	/// <version>  	1.0.0 11 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class RelationshipEditElement : QueryElementBase
	{
		private DataRelationshipAttribute _relationshipAttribute;

		/// <summary>
		/// Initiating an instance of RelationshipEditElement class
		/// </summary>
		/// <param name="relationshipAttribute">The relationship attribute</param>
		public RelationshipEditElement(DataRelationshipAttribute relationshipAttribute) : base()
		{
			_relationshipAttribute = relationshipAttribute;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();
			bool isNullValue = false;

			// any of the primary key value is empty, then relationship
			// value is considered null
			foreach (DataSimpleAttribute pk in _relationshipAttribute.PrimaryKeys)
			{
				if (string.IsNullOrEmpty(pk.AttributeValue))
				{
					isNullValue = true;
					break;
				}
			}

			query.Append("<").Append(_relationshipAttribute.Name);

			if (isNullValue)
			{
				query.Append(" xsi:nil=\"true\"/>\n");
			}
			else
			{
				query.Append(">\n");
				foreach (IQueryElement child in Children)
				{
					query.Append("  ").Append(child.ToXQuery());
				}
				query.Append("</").Append(_relationshipAttribute.Name).Append(">\n");
			}

			return query.ToString();
		}
	}
}
/*
* @(#)ReturnClause.cs
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
	/// Represents a return clause element in XQuery.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class ReturnClause : QueryElementBase
	{
		/// <summary>
		/// Initiating an instance of ReturnClause class
		/// </summary>
		public ReturnClause() : base()
		{
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("return").Append("\n");

			foreach (IQueryElement child in Children)
			{
				query.Append(child.ToXQuery()).Append(" ");
			}

			return query.ToString();
		}
	}
}
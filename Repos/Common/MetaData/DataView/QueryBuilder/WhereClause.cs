/*
* @(#)WhereClause.cs
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
	/// Represents a Where clause element in XQuery.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class WhereClause : QueryElementBase
	{
		/// <summary>
		/// Initiating an instance of WhereClause class
		/// </summary>
		public WhereClause() : base()
		{
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			string xquery = null;

			// Where clause has a single child representing the search filter
			// if the search filter is empty, do not generate where clause
			if (Children.Count == 1)
			{
				IQueryElement filter = Children[0];

				string expression = filter.ToXQuery();

				if (expression != null)
				{
					StringBuilder query = new StringBuilder();

					query.Append("where").Append("\n").Append(expression).Append(" \n");

					xquery = query.ToString();
				}
			}

			return xquery;
		}
	}
}
/*
* @(#)OrderByClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A OrderByClause class is a composite class whose children make up
	/// the elements of a ORDER BY clause.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class OrderByClause : SQLComposite
	{
		/// <summary>
		/// Initiating a ORDER BY clause object
		/// </summary>
		public OrderByClause() : base()
		{
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("ORDER BY ");
			
			// listing columns
			SQLElementCollection children = Children;

			for (int i = 0; i < children.Count; i++)
			{
				builder.Append(((SQLElement) children[i]).ToSQL());
				// add comma separator if necessary
				if (i < children.Count - 1)
				{
					builder.Append(", ");
				}
			}
			
			builder.Append(" ");
			
			return builder.ToString();
		}
	}
}
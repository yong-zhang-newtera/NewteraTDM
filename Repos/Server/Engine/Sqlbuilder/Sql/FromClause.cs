/*
* @(#)FromClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A FromClause class is a composite class whose children make up
	/// the elements of a FROM clause.
	/// </summary>
	/// <version>  	1.0.0 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class FromClause : SQLComposite
	{
		/// <summary>
		/// Initiating a FROM clause object
		/// </summary>
		public FromClause() : base()
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
			builder.Append("FROM ");
			
			// listing table names
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
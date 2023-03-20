/*
* @(#)ANDExpression.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A Node for AND operator in a SQL
	/// </summary>
	/// <version> 1.0.1 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class ANDExpression : SQLComposite
	{
		/// <summary>
		/// Initiating an ANDExpression
		/// </summary>
		public ANDExpression() : base()
		{
		}
		
		/// <summary>
		/// Returns a string by concatenating the strings of children
		/// </summary>
		/// <returns> string representation of this object</returns>
		public override string ToSQL()
		{
			StringBuilder builder = new StringBuilder();
			SQLElementCollection children = Children;
			
			foreach (SQLElement child in children)
			{
				if (builder.Length > 0)
				{
					builder.Append(" AND ").Append(child.ToSQL());
				}
				else
				{
					builder.Append(child.ToSQL());
				}
			}
			
			return builder.ToString();
		}
	}
}
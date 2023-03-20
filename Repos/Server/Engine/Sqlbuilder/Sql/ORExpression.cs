/*
* @(#)ORExpression.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A Node for OR operator in SQL
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class ORExpression : SQLComposite
	{
		/// <summary>
		/// Initiating an ORExpression
		/// </summary>
		public ORExpression() : base()
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
			
			foreach (SQLElement child in this.Children)
			{
				if (builder.Length > 0)
				{
					builder.Append(" OR ").Append(child.ToSQL());
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
/*
* @(#)NOTExpression.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A Node for NOT operator in SQL
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003
	/// </version>
	/// <author> Yong Zhang</author>
	public class NOTExpression : SQLComposite
	{
		/// <summary> 
		/// Initiating a NOTExpression object
		/// </summary>
		public NOTExpression() : base()
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
			SQLElementCollection children = Children;
			StringBuilder builder = new StringBuilder();
			builder.Append(" ");
			
			// This should have only one child        
			builder.Append("( NOT ").Append(children[0].ToSQL()).Append(")");
			
			return builder.ToString();
		}
	}
}
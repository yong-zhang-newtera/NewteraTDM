/*
* @(#)EnclosedExpression.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A Enclosed expression put parenthesis around an expression
	/// </summary>
	/// <version>  	1.0.1 06 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class EnclosedExpression:SQLElement
	{
		private SQLElement _enclosed;
		
		/// <summary>
		/// Initiating an EnclosedExpression object
		/// </summary>
		public EnclosedExpression() : base()
		{
			_enclosed = null;
		}
		
		/// <summary>
		/// Initiating an EnclosedExpression object
		/// </summary>
		/// <param name="expr">the enclosed expression</param>
		public EnclosedExpression(SQLElement expr) : base()
		{
			_enclosed = expr;
		}

		/// <summary>
		/// Sets the Enclosed Expression
		/// </summary>
		/// <value>the SQLElement</value>
		public virtual SQLElement Expression
		{
			set
			{
				_enclosed = value;
			}
		}		
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			if (_enclosed != null)
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("(").Append(_enclosed.ToSQL()).Append(")");
				return builder.ToString();
			}
			else
			{
				return "";
			}
		}
	}
}
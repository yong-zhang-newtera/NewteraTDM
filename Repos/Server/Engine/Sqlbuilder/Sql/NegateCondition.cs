/*
* @(#)NegateCondition.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A NegateCondition negates the value of enclosed condition
	/// </summary>
	/// <version>  	1.0.1 04 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class NegateCondition : SQLElement
	{
		private SQLElement _enclosed;
		
		/// <summary>
		/// Initiating a NegateCondition object
		/// </summary>
		public NegateCondition() : base()
		{
			_enclosed = null;
		}
		
		/// <summary>
		/// Initiating a NegateCondition object
		/// </summary>
		/// <param name="expr">the enclosed expression</param>
		public NegateCondition(SQLElement condition) : base()
		{
			_enclosed = condition;
		}
		
		/// <summary>
		/// Sets an enclosed condition object
		/// </summary>
		/// <param name="expr">the enclosed condition</param>
		public SQLElement Condition
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
				builder.Append("NOT(").Append(_enclosed.ToSQL()).Append(")");
				return builder.ToString();
			}
			else
			{
				return "";
			}
		}
	}
}
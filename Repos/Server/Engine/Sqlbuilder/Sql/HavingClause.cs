/*
* @(#)HavingClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

	/// <summary>
	/// A HavingClause class is a composite class whose children make up
	/// the elements of a HAVING clause.
	/// </summary>
	/// <version>  	1.0.0 12 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class HavingClause : SQLComposite
	{
		/* Private members */
		private SQLElement _condition;
		
		/// <summary>
		/// Initiating a HAVING clause object
		/// </summary>
		/// <param name="the">condition in the HAVING clause </param>
		public HavingClause(SQLElement condition) : base()
		{
			_condition = condition;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			return "HAVING " + _condition.ToSQL();
		}
	}
}
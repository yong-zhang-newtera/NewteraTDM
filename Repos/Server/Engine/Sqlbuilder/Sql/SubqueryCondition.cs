/*
* @(#)SubqueryCondition.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

	/// <summary>
	/// A SubqueryCondition class is a composite class whose children make up
	/// a subquery condition in the WHERE clause.
	/// </summary>
	/// <version>  	1.0.1 14 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class SubqueryCondition : SQLElement
	{
		// private instance members
		private SQLElement _fieldSet;
		private SQLElement _subquery;
		
		/// <summary>
		/// Initiating Constructor for a SubqueryCondition object
		/// </summary>
		/// <param name="fieldSet">the fields that refer to a subquery.</param>
		/// <param name="subquery">the subquery itself</param>
		public SubqueryCondition(SQLElement fieldSet, SQLElement subquery) : base()
		{
			_fieldSet = fieldSet;
			_subquery = subquery;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			return _fieldSet.ToSQL() + " in (" + _subquery.ToSQL() + ")";
		}
	}
}
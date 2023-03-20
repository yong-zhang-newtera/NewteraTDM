/*
* @(#)SQLStatement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

	/// <summary>
	/// A SQLStatement class is a special composite class whose children make up
	/// a entire SQL statement.
	/// </summary>
	/// <version>  	1.0.1 14 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class SQLStatement : SQLComposite
	{
		private SQLElement _fromClause;
		
		/// <summary>
		/// Initiating a SQLStatement object
		/// </summary>
		public SQLStatement() : base()
		{
			_fromClause = null;
		}

		/// <summary>
		/// Gets the FROM clause from the SQL statement
		/// </summary>
		/// <returns>
		/// the FROM clause SQLElement
		/// </returns>
		public SQLElement FromClause
		{
			get
			{
				return _fromClause;
			}
		}
		
		/// <summary>
		/// Add the FROM clause to the SQL statement
		/// </summary>
		/// <param name="fromClause">the FROM clause</param>
		public void AddFromClause(SQLElement fromClause)
		{
			_fromClause = fromClause;
			Add(fromClause);
		}
	}
}
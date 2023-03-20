/*
* @(#)DeleteClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;
	
	/// <summary>
	/// A DeleteClause class is a composite class whose children make up
	/// the elements of a DELETE clause.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003</version>
	/// <author>  Yong Zhang </author>
	public class DeleteClause : SQLComposite
	{
		// private instance members
		private SQLElement _tableName;
		
		/// <summary>
		/// Initiating a DeleteClause
		/// </summary>
		/// <param name="tableName">the table name in a delete clause</param>
		public DeleteClause(SQLElement tableName):base()
		{
			_tableName = tableName;
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
			builder.Append("DELETE FROM ").Append(_tableName.ToSQL()).Append(" ");

			return builder.ToString();
		}
	}
}
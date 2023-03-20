/*
* @(#)InsertClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A InsertClause class is a composite class whose children make up
	/// the elements of a INSERT clause.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class InsertClause : SQLComposite
	{
		// private instance members
		private SQLElement _tableName;
		
		/// <summary>
		/// Initaiting a INSERT clause object
		/// </summary>
		/// <param name="tableName">the table name</param>
		public InsertClause(SQLElement tableName) : base()
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
			builder.Append("INSERT INTO ").Append(_tableName.ToSQL()).Append(" (");
			
			// listing of set fields
			SQLElementCollection children = Children;

			for (int i = 0; i < children.Count; i++)
			{
				builder.Append(((SQLElement) children[i]).ToSQL());
				// add comma separator if necessary
				if (i < children.Count -1)
				{
					builder.Append(", ");
				}
			}
			
			builder.Append(") ");
			
			return builder.ToString();
		}
	}
}
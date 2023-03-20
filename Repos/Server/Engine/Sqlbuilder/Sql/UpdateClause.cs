/*
* @(#)UpdateClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A UpdateClause class is a composite class whose children make up
	/// the elements of an UPDATE clause.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003 </version>
	/// <author> Yong Zhang</author>
	public class UpdateClause : SQLComposite
	{
		
		// private instance members
		private SQLElement _tableName;
		
		/// <summary>
		/// Initiating a UPDATE clause object.
		/// </summary>
		/// <param name="tableName">name of the update table.</param>
		public UpdateClause(SQLElement tableName) : base()
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
			
			builder.Append("UPDATE ").Append(_tableName.ToSQL()).Append(" SET ");
			
			// listing of set fields
			SQLElementCollection children = Children;

			for (int i = 0; i < children.Count; i++)
			{
				builder.Append(((SQLElement) children[i]).ToSQL());
				// add comma separator if necessary
				if (i < children.Count - 1)
				{
					builder.Append(", ");
				}
			}
			
			builder.Append(" ");
			
			return builder.ToString();
		}
	}
}
/*
* @(#)WhereClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A WhereClause class is a composite class whose children make up
	/// the elements of a WHERE clause.
	/// </summary>
	/// <version>  	1.0.1 14 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class WhereClause : SQLComposite
	{
		// private instance members
		private string _clause;
		
		/// <summary>
		/// Initiating a WHERE clause object
		/// </summary>
		public WhereClause() : base()
		{
			_clause = null;
		}
		
		/// <summary>
		/// Initiating a WHERE clause object
		/// </summary>
		/// <param name="whereStr">the where clause in string form</param>
		public WhereClause(string whereStr) : base()
		{
			_clause = whereStr;
		}

		/// <summary> set a WHERE clause string to object.
		/// 
		/// </summary>
		/// <param name="clause">the where clause.
		/// 
		/// </param>
		public virtual string Clause
		{
			set
			{
				_clause = value;
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
			StringBuilder builder = new StringBuilder();
			
			/*
			* if there is where clause string, use it.
			* otherwise, turn the children into a string.
			*/
			if (_clause != null)
			{
				builder.Append(_clause);
			}
			else
			{
				SQLElementCollection children = Children;

				for (int i = 0; i < children.Count; i++)
				{
					builder.Append(((SQLElement) children[i]).ToSQL());
					// add comma separator if necessary
					if (i < children.Count - 1)
					{
						builder.Append(" AND ");
					}
				}
				
				if (builder.Length > 0)
				{
					builder.Insert(0, "WHERE ").Append(" ");
				}
			}
			
			return builder.ToString();
		}
	}
}
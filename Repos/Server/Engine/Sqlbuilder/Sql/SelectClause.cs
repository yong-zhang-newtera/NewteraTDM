/*
* @(#)SelectClause.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A SelectClause class is a composite class whose children make up
	/// the elements of a SELECT clause.
	/// </summary>
	/// <version>  	1.0.1 14 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class SelectClause : SQLComposite
	{
		// private instance declarartion
		private bool _isDistinct;
		
		/// <summary>
		/// Initiating a SELECT clause
		/// </summary>
		public SelectClause() : base()
		{
			_isDistinct = false;
		}
		
		/// <summary>
		/// Initiating a SELECT clause
		/// </summary>
		/// <param name="isDistinct">a indicator for DISTINCT.</param>
		public SelectClause(bool isDistinct) : base()
		{
			_isDistinct = isDistinct;
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
			builder.Append("SELECT ");
			
			if (_isDistinct)
			{
				builder.Append("DISTINCT ");
			}
			
			// listing columns
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
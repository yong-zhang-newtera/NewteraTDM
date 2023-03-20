/*
* @(#)OrderByField.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

	/// <summary>
	/// A OrderByField is a leaf class which is defined for a field name
	/// that appears in ORDER BY clauses.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class OrderByField : SQLElement
	{
		// private instance members
		private string _tableAlias;
		private string _fieldName;
		private bool _isAcending;
		
		/// <summary>
		/// Initiating an OrderByField object
		/// </summary>
		/// <param name="fieldName">the name of a field</param>
		/// <param name="tableAlias">an alias of owning table</param>
		/// <param name="isAcending">true if sort direction is ascending.</param>
		public OrderByField(string fieldName, string tableAlias, bool isAscending) : base()
		{
			_fieldName = fieldName;
			_tableAlias = tableAlias;
			_isAcending = isAscending;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			
			// if the table alias has not been assigned, try to get it from its entity
			if (_tableAlias == null && ClassEntity != null && ClassEntity.Alias != null)
			{
				_tableAlias = ClassEntity.Alias;
			}
			
			if (_tableAlias != null)
			{
				buf.Append(_tableAlias).Append(".").Append(_fieldName);
			}
			else
			{
				buf.Append(_fieldName);
			}
			
			if (_isAcending)
			{
                buf.Append(" ").Append(SQLElement.SORT_ASCEND);
            }
			else
			{
				buf.Append(" ").Append(SQLElement.SORT_DESCEND);
			}
			
			return buf.ToString();
		}
	}
}
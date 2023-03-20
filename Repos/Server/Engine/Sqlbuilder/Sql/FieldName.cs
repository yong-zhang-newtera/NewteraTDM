/*
* @(#)FieldName.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;
	
	/// <summary>
	/// A FieldName is a leaf class which is defined for a field name
	/// that may appear in SELECT, GROUP BY, and ORDER BY clauses.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003</version>
	/// <author>  		Yong Zhang </author>
	public class FieldName : SQLElement
	{
		// private instance members
		private string _tableAlias;
		private string _fieldName;
		private string _fieldAlias;
		
		/// <summary>
		/// Initiating a FieldName object
		/// </summary>
		/// <param name="fieldName">the name of a field</param>
		public FieldName(string fieldName) : this(fieldName, null, null)
		{
		}
		
		/// <summary>
		/// Initiating a FieldName object
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		public FieldName(string fieldName, string tableAlias) : this(fieldName, tableAlias, null)
		{
		}
		
		/// <summary>
		/// Initiating a FieldName object
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		/// <param name="fieldAlias">is an alias of the field</param>
		public FieldName(string fieldName, string tableAlias, string fieldAlias) : base()
		{
			_fieldName = fieldName;
			_tableAlias = tableAlias;
			_fieldAlias = fieldAlias;
		}
		
		/// <summary>
		/// Gets or sets the alias of table
		/// </summary>
		/// <value> alias of the table </value>
		public string TableAlias
		{
			get
			{
				return _tableAlias;
			}
			set
			{
				_tableAlias = value;
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

			// if the table alias has not been assigned, try to get it from its entity
			if (_tableAlias == null && ClassEntity != null && ClassEntity.Alias != null)
			{
				_tableAlias = ClassEntity.Alias;
			}
			
			if (_tableAlias != null && _fieldAlias != null)
			{
				builder.Append(_tableAlias).Append(".").Append(_fieldName).Append(" ").Append(_fieldAlias);
				return builder.ToString();
			}
			else if (_tableAlias != null)
			{
				builder.Append(_tableAlias).Append(".").Append(_fieldName);
				return builder.ToString();
			}
			else if (_fieldAlias != null)
			{
				builder.Append(_fieldName).Append(" ").Append(_fieldAlias);
				return builder.ToString();
			}
			else
			{
				return _fieldName;
			}
		}
	}
}
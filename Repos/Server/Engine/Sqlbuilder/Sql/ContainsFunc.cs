/*
* @(#)ContainsFunc.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;
	using Newtera.Server.DB;

	/// <summary>
	/// A ContainsFunc class is responsible for outputting the SQL text used for expressing
	/// a condition for full-text search
	/// </summary>
	/// <version>  	1.0.1 21 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class ContainsFunc : SQLElement
	{	
		// private declarations
		private SQLElement _field;
		private string _value;
		private string _label;
		private DatabaseType _dbType;
		private bool _isFullText;
		
		/// <summary>
		/// Initiating Contains function object
		/// </summary>
		/// <param name="field">a table column name.</param>
		/// <param name="value">the search value in expression.</param>
		/// <param name="dataProvider">the database provider.</param>
		/// <param name="isFullText">the flag indicates whenther to use full-text search function</param>
		public ContainsFunc(SQLElement field, string val, IDataProvider dataProvider, bool isFullText) : base()
		{
			_field = field;
			_value = val;
			_label = null;
			_isFullText = isFullText;
			_dbType = dataProvider.DatabaseType;
		}
		
		/// <summary>
		/// Initiating Contains function object
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="value">the search value in expression</param>
		/// <param name="label">the lable of the function</param>
		/// <param name="dataProvider">the database provider</param>
		/// <param name="isFullText">the flag indicates whenther to use full-text search function.</param>
		public ContainsFunc(SQLElement field, string val, string label, IDataProvider dataProvider, bool isFullText) : base()
		{
			_field = field;
			_value = val;
			_label = label;
			_isFullText = isFullText;
			_dbType = dataProvider.DatabaseType;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			if (_isFullText)
			{
				return SymbolLookupFactory.Instance.Create(_dbType).GetContainsFunc(_field.ToSQL(), _value, _label);
			}
			else
			{
				StringBuilder builder = new StringBuilder();
				// if the attribute doesn't have full-text index, use LIKE operator instead
                string value = _value;
                if (value != null)
                {
                    value = value.Replace('*', '%');
                }

				builder.Append(_field.ToSQL()).Append(" LIKE '%").Append(value).Append("%'");
				return  builder.ToString();
			}
		}
	}
}
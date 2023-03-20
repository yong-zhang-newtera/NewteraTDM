/*
* @(#)LikeFunc.cs
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
	/// A LikeFunc class is responsible for outputting string compare SQL syntax
	/// </summary>
	/// <version>  	1.0.0 03 Jul 2003</version>
	/// <author> Yong Zhang</author>
	public class LikeFunc : SQLElement
	{
		// private declarations
		private SQLElement _field;
		private string _value;
		private DatabaseType _dbType;
		
		/// <summary>
		/// Initiating a LikeFunc object
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="value">the search value in expression.</param>
		/// <param name="dataProvider">the database provider.</param>
		public LikeFunc(SQLElement field, string val, IDataProvider dataProvider) : base()
		{
			_field = field;
			_value = val;
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
			StringBuilder builder = new StringBuilder();
			builder.Append(_field.ToSQL()).Append(" LIKE '").Append(_value).Append("'");
			return builder.ToString();
		}
	}
}
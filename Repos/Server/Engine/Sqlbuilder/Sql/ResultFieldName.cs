/*
* @(#)ResultFieldName.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A ResultFieldName is a leaf class which is defined for a field name
	/// that appears in SELECT clauses.
	/// </summary>
	/// <version>  	1.0.0 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class ResultFieldName:SQLElement
	{
		// private instance members
		private string _tableAlias;
		private string _fieldName;
		private string _fieldAlias;
		private DataType _type;
		private string _function;
		private SymbolLookup _lookup;
		
		/// <summary>
		/// Initiating a ResultFieldName object.
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table.</param>
		/// <param name="type">the type of field.</param>
		/// <param name="dataProvider">the database provider </param>
		public ResultFieldName(string fieldName, string tableAlias, DataType type, IDataProvider dataProvider)
		{
			Initialize(fieldName, tableAlias, type, null, null, dataProvider);
		}
		
		/// <summary>
		/// Initiating a ResultFieldName object.
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		/// <param name="type">the type of field.</param>
		/// <param name="function">an aggregate function applied to the field</param>
		/// <param name="dataProvider">the database provider</param>
		public ResultFieldName(string fieldName, string tableAlias, DataType type, string function, IDataProvider dataProvider)
		{
			Initialize(fieldName, tableAlias, type, function, null, dataProvider);
		}
		
		/// <summary>
		/// Initiating a ResultFieldName object.
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		/// <param name="type">the type of field.</param>
		/// <param name="function">an aggregate function applied to the field</param>
		/// <param name="dataProvider">the database provider</param>
		public ResultFieldName(string fieldName, string tableAlias, DataType type, string function, string fieldAlias, IDataProvider dataProvider)
		{
			Initialize(fieldName, tableAlias, type, function, fieldAlias, dataProvider);
		}
		
		/// <summary>
		/// Initializing a ResultFieldName object
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		/// <param name="type">the type of field.</param>
		/// <param name="function">an aggregate function applied to the field</param>
		/// <param name="dataProvider">the database provider</param>
		private void Initialize(string fieldName, string tableAlias, DataType type, string function, string fieldAlias, IDataProvider dataProvider)
		{
			_fieldName = fieldName;
			_fieldAlias = fieldAlias;
			_tableAlias = tableAlias;
			_type = type;
			_function = function;
			
			_lookup = SymbolLookupFactory.Instance.Create(dataProvider.DatabaseType);
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			string fieldExp;
			
			// if the table alias has not been assigned, try to get it from its class entity
			if (_tableAlias == null && ClassEntity != null && ClassEntity.Alias != null)
			{
				_tableAlias = ClassEntity.Alias;
			}
			
			if (_tableAlias != null)
			{
				fieldExp = _tableAlias + "." + _fieldName;
			}
			else
			{
				fieldExp = _fieldName;
			}
			
			// adjust for date/time formate
			string fmt = null;
			switch (_type)
			{
				
				case DataType.Date: 
					fmt = LocaleInfo.Instance.DateFormat;
					fieldExp = _lookup.GetCharFunc4Date(fieldExp, fmt);
                    _fieldAlias = _fieldName;
					break;
				
				case DataType.DateTime: 
					fieldExp = _lookup.GetCharFunc4Timestamp(fieldExp, LocaleInfo.Instance.DateFormat, LocaleInfo.Instance.TimeFormat);
                    _fieldAlias = _fieldName;
					break;
				
				case DataType.Boolean: 
					fieldExp = _lookup.GetDecodeFunc(fieldExp, LocaleInfo.Instance.True, LocaleInfo.Instance.False);
                    _fieldAlias = _fieldName;
					break;
				}
			
			// Apply for aggregate function
			if (_function != null)
			{
				fieldExp = _function + "(" + fieldExp + ")";
			}
			
			// Apply field alias
            /*
			if (_fieldAlias != null)
			{
				fieldExp = fieldExp + " " + _fieldAlias;
			}
            */
			
			return fieldExp;
		}
	}
}
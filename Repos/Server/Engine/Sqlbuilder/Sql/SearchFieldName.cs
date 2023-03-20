/*
* @(#)SearchFieldName.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A SearchFieldName is a leaf class which is defined for a field name
	/// that appear in WHERE clauses.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class SearchFieldName : SQLElement
	{
		// private instance members
		private string _tableAlias;
		private string _fieldName;
		private CaseStyle _caseStyle;
		private DataType _fieldType;
		private SymbolLookup _lookup;
		
		/// <summary>
		/// Initiating a SearchFieldName object
		/// </summary>
		/// <param name="fieldName">the name of a field </param>
		/// <param name="dataProvider">the database provider</param>
		public SearchFieldName(string fieldName, IDataProvider dataProvider) : base()
		{
			Initialize(fieldName, null, CaseStyle.CaseSensitive, DataType.String, dataProvider);
		}
		
		/// <summary>
		/// Initiating a SearchFieldName object
		/// </summary>
		/// <param name="fieldName">the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		/// <param name="dataProvider">the database provider</param>
		public SearchFieldName(string fieldName, string tableAlias, IDataProvider dataProvider):base()
		{
			Initialize(fieldName, tableAlias, CaseStyle.CaseSensitive, DataType.String, dataProvider);
		}
		
		/// <summary>
		/// Initiating a SearchFieldName object
		/// </summary>
		/// <param name="fieldName">is the name of a field.</param>
		/// <param name="tableAlias">is an alias of owning table.</param>
		/// <param name="caseStyle">the case style of the field.</param>
		/// <param name="dataProvider">the database provider</param>
		public SearchFieldName(string fieldName, string tableAlias, CaseStyle caseStyle, IDataProvider dataProvider):base()
		{
			Initialize(fieldName, tableAlias, caseStyle, DataType.String, dataProvider);
		}
		
		/// <summary>
		/// Initiating a SearchFieldName object
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		/// <param name="caseStyle">the case style of the field</param>
		/// <param name="fieldType">is the type of the field</param>
		/// <param name="dataProvider">the database provider</param>
		public SearchFieldName(string fieldName, string tableAlias, CaseStyle caseStyle, DataType type, IDataProvider dataProvider):base()
		{
			Initialize(fieldName, tableAlias, caseStyle, type, dataProvider);
		}
		
		/// <summary>
		/// Initialize a SearchFieldName object
		/// </summary>
		/// <param name="fieldName">is the name of a field</param>
		/// <param name="tableAlias">is an alias of owning table</param>
		/// <param name="caseStyle">the case style of the field</param>
		/// <param name="type">the type of the field</param>
		/// <param name="dataProvider">the database provider</param>
		private void Initialize(string fieldName, string tableAlias, CaseStyle caseStyle, DataType type, IDataProvider dataProvider)
		{
			_fieldName = fieldName;
			_tableAlias = tableAlias;
			_caseStyle = caseStyle;
			_fieldType = type;
			
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
			string finalName = "";
			
			// if the table alias has not been assigned, try to get it from its class entity
			if (_tableAlias == null && ClassEntity != null && ClassEntity.Alias != null)
			{
				_tableAlias = ClassEntity.Alias;
			}
			
			if (_tableAlias != null)
			{
				finalName = _tableAlias + "." + _fieldName;
			}
			else
			{
				finalName = _fieldName;
			}
			
			if (_caseStyle == CaseStyle.CaseInsensitive && _fieldType != DataType.Date && _fieldType != DataType.DateTime)
			{
				
				finalName = _lookup.GetUpperFunc(finalName);
			}
			
			return finalName;
		}
	}
}
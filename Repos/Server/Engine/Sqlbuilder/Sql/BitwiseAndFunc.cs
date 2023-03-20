/*
* @(#)BitwiseAndFunc.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A BitwiseAndFunc is a leaf class which is defined for a field name
	/// that appear in WHERE clauses.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2004 </version>
	/// <author> Yong Zhang </author>
	public class BitwiseAndFunc : SQLElement
	{
		// private instance members
		private string _tableAlias;
		private string _fieldName;
		private string _operand;
		private SymbolLookup _lookup;
		
		/// <summary>
		/// Initiating an instance of  BitwiseAndFunc class
		/// </summary>
		/// <param name="fieldName">is the name of a field.</param>
		/// <param name="tableAlias">is an alias of owning table.</param>
		/// <param name="operand">the right operand of bitwise and.</param>
		/// <param name="dataProvider">the database provider</param>
		public BitwiseAndFunc(string fieldName, string tableAlias, string operand, IDataProvider dataProvider):base()
		{
			_fieldName = fieldName;
			_tableAlias = tableAlias;
			_operand = operand;
			
			_lookup = SymbolLookupFactory.Instance.Create(dataProvider.DatabaseType);
		}
		
		/// <summary>
		/// Gets or sets the right operand of the bitwise AND function
		/// </summary>
		/// <value>A operand</value>
		public string RightOperand
		{
			get
			{
				return _operand;
			}
			set
			{
				_operand = value;
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
				
			finalName = _lookup.GetBitwiseAndFunc(finalName, _operand);
			
			return finalName;
		}
	}
}
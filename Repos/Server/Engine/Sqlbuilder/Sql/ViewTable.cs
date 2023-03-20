/*
* @(#)ViewTable.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

	/// <summary>
	/// A ViewTable is used to output text for a view for an aggregate function to form
	/// a part of FROM clause.
	/// </summary>
	/// <version> 1.0.1 27 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class ViewTable:SQLElement
	{
		// private instance members
		private SQLElement _view;
		private string _alias;
		
		/// <summary>
		/// Initiating a ViewTable object.
		/// </summary>
		/// <param name="sqlElement">a SQL element for an aggregate function view</param>
		/// <param name="alias">alias given to the view</param>
		public ViewTable(SQLElement view, string alias) : base()
		{
			_view = view;
			_alias = alias;
		}

		/// <summary>
		/// Gets or sets the alias of table.
		/// </summary>
		/// <returns> alias of the table</returns>
		/// <value> alias of the table</value>
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
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
			// if the table alias is null, try to get it from its class entity
			if (_alias == null && ClassEntity != null && ClassEntity.Alias != null)
			{
				_alias = ClassEntity.Alias;
			}
			
			if (_alias == null)
			{
				return _view.ToSQL();
			}
			else
			{
				return "(" + _view.ToSQL() + ") " + _alias;
			}
		}
		
		
	}
}
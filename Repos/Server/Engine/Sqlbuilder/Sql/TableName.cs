/*
* @(#)TableName.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

	/// <summary>
	/// A TableName is a leaf class which is defined for a table name
	/// in a TableList of FROM clause.
	/// </summary>
	/// <version>  	1.0.0 14 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class TableName : SQLElement, IJoinElement
	{
		// private instance members
		private string _name;
		private string _alias;
		private int _size;
		
		/// <summary>
		/// Initiating a TableName object
		/// </summary>
		/// <param name="name">name of the table </param>
		public TableName(string name) : base()
		{
			_name = name;
			_alias = null;
			_size = 0;
		}
		
		/// <summary>
		/// Initiating a TableName object
		/// </summary>
		/// <param name="name">name of the table</param>
		/// <param name="alias">alias of the table</param>
		public TableName(string name, string alias) : base()
		{
			_name = name;
			_alias = alias;
			_size = 0;
		}
		
		/// <summary>
		/// Initiating a TableName object
		/// </summary>
		/// <param name="name">name of the table</param>
		/// <param name="alias">alias of the table</param>
		/// <param name="size">size of the table.</param>
		public TableName(string name, string alias, int size) : base()
		{
			_name = name;
			_alias = alias;
			_size = size;
		}

		/// <summary>
		/// Gets the name of table
		/// </summary>
		/// <value> name of the table</returns>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets or sets the alias of table
		/// </summary>
		/// <value> alias of the table </value>
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
		/// Gets or sets the size of table
		/// </summary>
		/// <value> size of the table </value>
		public int Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}			
		}

		#region IJoinElement

		/// <summary>
		/// Gets the information indicating whether the table's alias equals to
		/// the given alias.
		/// </summary>
		/// <param name="alias">The alias</param>
		/// <returns>True if the inner join chain contains it, false otherwise.</returns>
		public bool ContainsAlias(string alias)
		{
			bool status = false;

			if (_alias != null && _alias == alias)
			{
				status = true;
			}

			return status;
		}

		#endregion
				
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
				return _name;
			}
			else
			{
				return _name + " " + _alias;
			}
		}
	}
}
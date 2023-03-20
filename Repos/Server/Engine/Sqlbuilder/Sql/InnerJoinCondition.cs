/*
* @(#)InnerJoinCondition.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using Newtera.Server.DB;

	/// <summary>
	/// An InnerJoinCondition class is a composite class whose children make up
	/// an inner-join condition in the FROM clause.
	/// </summary>
	/// <version>  	1.0.0 09 Jun 2005</version>
	/// <author> Yong Zhang </author>
	public class InnerJoinCondition : SQLElement, IJoinElement
	{		
		// private instance members
		private SQLElement _leftTable;
		private SQLElement _rightTable;
		private SQLElement _leftExpr;
		private SQLElement _rightExpr;
		private IDataProvider _dataProvider;
		private SymbolLookup _lookup;
		
		/// <summary>
		/// Initiating an InnerJoinCondition object
		/// </summary>
		/// <param name="leftTable">the table element represent left side of join table.</param>
		/// <param name="rightTable">the table element represent right side of join table .</param>
		/// <param name="leftExpr">the expression on the left side of join.</param>
		/// <param name="rightExpr">the expression on the right side of join.</param>
		/// <param name="dataProvider">The data provider.</param>
		public InnerJoinCondition(SQLElement leftTable, SQLElement rightTable, SQLElement leftExpr, SQLElement rightExpr, IDataProvider dataProvider) : base()
		{
			_leftTable = leftTable;
			_rightTable = rightTable;
			_leftExpr = leftExpr;
			_rightExpr = rightExpr;
			_dataProvider = dataProvider;
			_lookup = SymbolLookupFactory.Instance.Create(dataProvider.DatabaseType);
		}
		
		/// <summary>
		/// Gets the left table object.
		/// </summary>
		/// <value>the left table object</value>
		public SQLElement LeftTable
		{
			get
			{
				return _leftTable;
			}
		}

		/// <summary>
		/// Gets the right table object
		/// </summary>
		/// <value>the right table object</value>
		public SQLElement RightTable
		{
			get
			{
				return _rightTable;
			}
		}

		/// <summary>
		/// Gets the left operand.
		/// </summary>
		/// <value>the left operand of outer join.</value>
		public SQLElement LeftOperand
		{
			get
			{
				return _leftExpr;
			}
		}

		/// <summary>
		/// Gets the right operand
		/// </summary>
		/// <value>the right operand of outer join</value>
		public SQLElement RightOperand
		{
			get
			{
				return _rightExpr;
			}
		}

		#region IJoinElement

		/// <summary>
		/// Gets the information indicating whether the inner join chain contains a
		/// class with the provided alias.
		/// </summary>
		/// <param name="alias">The alias</param>
		/// <returns>True if the inner join chain contains it, false otherwise.</returns>
		public bool ContainsAlias(string alias)
		{
			bool status = false;

			if (_leftTable != null)
			{
				status = ((IJoinElement) _leftTable).ContainsAlias(alias);
			}

			if (!status)
			{
				// search downward
				if (_rightTable != null)
				{
					status = ((IJoinElement) _rightTable).ContainsAlias(alias);
				}
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
			string joinStr = null;
			if (_rightTable != null)
			{
				joinStr = _lookup.GetInnerJoin(_leftTable.ToSQL(), _rightTable.ToSQL(), _leftExpr.ToSQL(), _rightExpr.ToSQL());
			}
			else if (_leftTable != null)
			{
				joinStr = _leftTable.ToSQL();
			}
			
			return joinStr;
		}
		}
}
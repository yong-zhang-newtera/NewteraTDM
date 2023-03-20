/*
* @(#)OuterJoinCondition.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using Newtera.Server.DB;

	/// <summary>
	/// An OuterJoinCondition class is a composite class whose children make up
	/// an outer-join condition in the WHERE clause.
	/// </summary>
	/// <version>  	1.0.0 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class OuterJoinCondition : SQLElement, IJoinElement
	{		
		// private instance members
		private SQLElement _leftTable;
		private SQLElement _rightTable;
		private SQLElement _leftExpr;
		private SQLElement _rightExpr;
		private JoinType _type;
		private IDataProvider _dataProvider;
		private SymbolLookup _lookup;
		
		/// <summary>
		/// Initiating an OuterJoinCondition object
		/// </summary>
		/// <param name="leftTable">the table element represent left side of join table.</param>
		/// <param name="rightTable">the table element represent right side of join table .</param>
		/// <param name="leftExpr">the expression on the left side of join.</param>
		/// <param name="rightExpr">the expression on the right side of join.</param>
		/// <param name="type">the type of outer join (right, left, full).</param>
		public OuterJoinCondition(SQLElement leftTable, SQLElement rightTable, SQLElement leftExpr, SQLElement rightExpr, JoinType type, IDataProvider dataProvider) : base()
		{
			_leftTable = leftTable;
			_rightTable = rightTable;
			_leftExpr = leftExpr;
			_rightExpr = rightExpr;
			_type = type;
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

		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			string joinStr = "";
			switch (_type)
			{
				
				case JoinType.RightJoin: 
					joinStr = _lookup.GetRightOuterJoin(_leftTable.ToSQL(), _rightTable.ToSQL(), _leftExpr.ToSQL(), _rightExpr.ToSQL());
					break;
				
				case JoinType.LeftJoin: 
					joinStr = _lookup.GetLeftOuterJoin(_leftTable.ToSQL(), _rightTable.ToSQL(), _leftExpr.ToSQL(), _rightExpr.ToSQL());
					break;
				
				case JoinType.FullJoin: 
					joinStr = _lookup.GetFullOuterJoin(_leftTable.ToSQL(), _rightTable.ToSQL(), _leftExpr.ToSQL(), _rightExpr.ToSQL());
					break;
				}
			
			return joinStr;
		}

		#region IJoinElememnt

		/// <summary>
		/// Gets the information indicating whether the outer join chain contains a
		/// class with the provided alias.
		/// </summary>
		/// <param name="alias">The alias</param>
		/// <returns>True if the outer join chain contains it, false otherwise.</returns>
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
	}

	/// <summary>
	/// Describes the options for join type
	/// </summary>
	public enum JoinType
	{
		RightJoin,
		LeftJoin,
		FullJoin
	}
}
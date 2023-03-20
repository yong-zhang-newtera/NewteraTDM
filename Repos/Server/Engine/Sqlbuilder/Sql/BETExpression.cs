/*
* @(#)BETExpression.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// A Node for BETWEEN operator in a SQL
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class BETExpression : SQLElement
	{
		// private declarations
		private SQLElement _field;
		private SQLElement _lvalue;
		private SQLElement _rvalue;
		private DataType _type;
		
		/// <summary>
		/// Initiating a BETExpression object
		/// </summary>
		/// <param name="field">the column object</param>
		/// <param name="lvalue">the left value object</param>
		/// <param name="rvalue">the right vale object </param>
		public BETExpression(SQLElement field, SQLElement lvalue, SQLElement rvalue) : base()
		{
			_field = field;
			_lvalue = lvalue;
			_rvalue = rvalue;
		}
		
		/// <summary>
		/// Initiating a BETExpression object
		/// </summary>
		/// <param name="field">the column object</param>
		/// <param name="lvalue">the left value object</param>
		/// <param name="rvalue">the right vale object</param>
		/// <param name="type">the type of column </param>
		public BETExpression(SQLElement field, SQLElement lvalue, SQLElement rvalue, DataType type) : base()
		{
			_field = field;
			_lvalue = lvalue;
			_rvalue = rvalue;
			_type = type;
		}
		
		/// <summary>
		/// Return the BETExpression in string.
		/// </summary>
		/// <returns> string representation of this object.</returns>
		public override string ToSQL()
		{
			string leftStr = _lvalue.ToSQL();
			string rightStr = _rvalue.ToSQL();

			if (leftStr.Equals("IS NULL"))
			{
				leftStr = "NULL";
			}
			
			if (rightStr.Equals("IS NULL"))
			{
				rightStr = "NULL";
			}
			
			StringBuilder builder = new StringBuilder();
			
			if (SQLElement.IsNumericType(_type))
			{
				float val1 = (float) System.Single.Parse(leftStr);
				float val2 = (float) System.Single.Parse(rightStr);
				builder.Append(_field.ToSQL()).Append(" ");
				builder.Append("BETWEEN ").Append((float) (val1 - 0.0000005));
				builder.Append(" AND ").Append((float) (val2 + 0.0000003));
			}
			else
			{
				builder.Append(_field.ToSQL()).Append(" BETWEEN ").Append(leftStr).Append(" AND ").Append(rightStr);
			}
			
			return builder.ToString();
		}
	}
}
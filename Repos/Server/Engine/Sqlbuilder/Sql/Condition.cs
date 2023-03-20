/*
* @(#)Condition.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Server.DB;
	
	/// <summary>
	/// A Condition class is a composite class whose children represent
	/// conditions in the WHERE clause or HAVING clause.
	/// </summary>
	/// <version>  	1.0.1 12 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class Condition : SQLElement
	{
		private DataType _type = DataType.String;
		private SQLElement _leftOperand;
		private SQLElement _rightOperand;
		private string _operator;
        private IDataProvider _dataProvider;
		
		/// <summary>
		/// Initiating a Condition object
		/// </summary>
		/// <param name="operator">the relational operator</param>
		public Condition(string relOperator) : base()
		{
			_leftOperand = null;
			_rightOperand = null;
			_operator = relOperator;
            _dataProvider = null;
		}

        /// <summary>
        /// Initiating a Condition object
        /// </summary>
        /// <param name="operator">the relational operator</param>
        public Condition(string relOperator, IDataProvider dataProvider)
            : base()
        {
            _leftOperand = null;
            _rightOperand = null;
            _operator = relOperator;
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Initiating a Condition object.
        /// </summary>
        /// <param name="leftOperand">the expression on left side of condition</param>
        /// <param name="operator">the relational operator</param>
        /// <param name="rightOperand">the expression on right side of condition</param>
        public Condition(SQLElement leftOperand, string relOperator, SQLElement rightOperand)
            : base()
        {
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
            _operator = relOperator;
            _dataProvider = null;
            if (_operator != null && _operator == SQLElement.OPT_LIKE &&
                _rightOperand != null && _rightOperand is SearchValue)
            {
                ((SearchValue)_rightOperand).IsPattern = true;
            }
        }
		
		/// <summary>
		/// Initiating a Condition object.
		/// </summary>
		/// <param name="leftOperand">the expression on left side of condition</param>
		/// <param name="operator">the relational operator</param>
		/// <param name="rightOperand">the expression on right side of condition</param>
        public Condition(SQLElement leftOperand, string relOperator, SQLElement rightOperand, IDataProvider dataProvider)
            : base()
		{
			_leftOperand = leftOperand;
			_rightOperand = rightOperand;
			_operator = relOperator;
            _dataProvider = dataProvider;
            if (_operator != null && _operator == SQLElement.OPT_LIKE &&
                _rightOperand != null && _rightOperand is SearchValue)
            {
                ((SearchValue)_rightOperand).IsPattern = true;
            }
		}
		
		/// <summary>
		/// Initiating a Condition object.
		/// </summary>
		/// <param name="leftOperand">the expression on left side of condition.</param>
		/// <param name="relOperator">the relational operator.</param>
		/// <param name="rightOperand">the expression on right side of condition.</param>
		/// <param name="type">the type of value.</param>
		public Condition(SQLElement leftOperand, string relOperator, SQLElement rightOperand, DataType type) : base()
		{
			_leftOperand = leftOperand;
			_rightOperand = rightOperand;
			_operator = relOperator;
            _dataProvider = null;
			_type = type;
            if (_operator != null && _operator == SQLElement.OPT_LIKE &&
                _rightOperand != null && _rightOperand is SearchValue)
            {
                ((SearchValue)_rightOperand).IsPattern = true;
            }
		}

		/// <summary>
		/// Gets the operator of the condition.
		/// </summary>
		/// <value>The operator of the condition.</value>
		public string SQLOperator
		{
			get
			{
				return _operator;
			}
		}

		/// <summary>
		/// Gets or sets the left operand of the condition.
		/// </summary>
		/// <value>A SQLElement object</value>
		public SQLElement LeftOperand
		{
			get
			{
				return _leftOperand;
			}
			set
			{
				_leftOperand = value;
			}
		}

		/// <summary>
		/// Gets or sets the right operand of the condition.
		/// </summary>
		/// <value>A SQLElement object</value>
		public SQLElement RightOperand
		{
			get
			{
				return _rightOperand;
			}
			set
			{
				_rightOperand = value;
                if (_operator != null && _operator == SQLElement.OPT_LIKE &&
                    value != null && value is SearchValue)
                {
                    ((SearchValue)value).IsPattern = true;
                }
			}
		}

		/// <summary>
		/// Gets or sets type of right operand.
		/// </summary>
		/// <value>One of DataType enum values</value>
		public DataType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
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
			StringBuilder builder = new StringBuilder();

			if (_rightOperand is NullValue)
			{
				if (_operator == SQLElement.OPT_EQUALS)
				{
					builder.Append(_leftOperand.ToSQL()).Append(" IS NULL");
				}
				else
				{
					// treat all other operators as not null
					builder.Append(_leftOperand.ToSQL()).Append(" IS NOT NULL");
				}
			}
			else if (SQLElement.IsNumericType(Type) && SQLOperator == SQLElement.OPT_EQUALS)
			{
				string tmpstr = _rightOperand.ToSQL();
				double val = System.Double.Parse(tmpstr);
				builder.Append(_leftOperand.ToSQL()).Append(" ");
				builder.Append("BETWEEN ").Append((double) (val - 0.0000005)).Append(" AND ").Append((double) (val + 0.0000003));
			}
            else if (SQLOperator == SQLElement.OPT_NEQ &&
                _dataProvider != null &&
                !string.IsNullOrEmpty(SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType).NegateFunc))
            {
                // use Negate function so that null values will be treated as not equals
                builder.Append(SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType).NegateFunc).Append("(");
                builder.Append(_leftOperand.ToSQL()).Append(" ").Append(SQLElement.OPT_EQUALS).Append(" ").Append(_rightOperand.ToSQL()).Append(")");
            }
            else
            {
                builder.Append(_leftOperand.ToSQL()).Append(" ").Append(SQLOperator).Append(" ").Append(_rightOperand.ToSQL());
            }
			
			return builder.ToString();
		}
	}
}
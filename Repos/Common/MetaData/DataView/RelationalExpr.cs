/*
* @(#)RelationalExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a logical expressions
	/// </summary>
	/// <version> 1.0.0 14 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class RelationalExpr : BinaryExpr
	{
		/// <summary>
		/// Operator of relational expression
		/// </summary>
		public static string[] Operators = new string[] {"=", "!=", "<", ">", "<=", ">=", "like", "is null", "is not null"};

		/// <summary>
		/// Convert an operator string to a ElementType
		/// </summary>
		/// <param name="op">An operator string</param>
		/// <returns>A ElementType</returns>
		public static ElementType ConvertToElementType(string op)
		{
			ElementType type = ElementType.Unknown;

			switch (op)
			{
				case "=":
					type = ElementType.Equals;
					break;
				case "!=":
					type = ElementType.NotEquals;
					break;
				case "<":
					type = ElementType.LessThan;
					break;
				case ">":
					type = ElementType.GreaterThan;
					break;
				case "<=":
					type = ElementType.LessThanEquals;
					break;
				case ">=":
					type = ElementType.GreaterThanEquals;
					break;
                case "like":
                    type = ElementType.Like;
                    break;
                case "is null":
                    type = ElementType.IsNull;
                    break;
                case "is not null":
                    type = ElementType.IsNotNull;
                    break;
			}

			return type;
		}

		/// <summary>
		/// Initiate an instance of RelationalExpr object.
		/// </summary>
		public RelationalExpr(ElementType type) : base()
		{
			if (type == ElementType.Equals || type == ElementType.NotEquals ||
				type == ElementType.LessThan || type == ElementType.GreaterThan ||
				type == ElementType.LessThanEquals || type == ElementType.GreaterThanEquals ||
                type == ElementType.Like ||
                type == ElementType.IsNull || type == ElementType.IsNotNull)
			{
				_type = type;
				Name = Operator;
			}
			else
			{
				_type = ElementType.Unknown;
			}
		}

		/// <summary>
		/// Initiate an instance of RelationalExpr object.
		/// </summary>
		public RelationalExpr(ElementType type, IDataViewElement left, IDataViewElement right) : base(left, right)
		{
			if (type == ElementType.Equals || type == ElementType.NotEquals ||
				type == ElementType.LessThan || type == ElementType.GreaterThan ||
				type == ElementType.LessThanEquals || type == ElementType.GreaterThanEquals ||
                type == ElementType.Like ||
                type == ElementType.IsNull || type == ElementType.IsNotNull)
			{
				_type = type;
				Name = Operator;
			}
			else
			{
				_type = ElementType.Unknown;
			}
		}

		/// <summary>
		/// Initiate an instance of RelationalExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal RelationalExpr(XmlElement parent) : base(parent)
		{
		}

        /// <summary>
        /// Gets the information indicating whether the binary expression has a value(s) as
        /// the right expression of the operator.
        /// </summary>
        public override bool HasValue
        {
            get
            {
                bool status = false;

                switch (ElementType)
                {
                    case ElementType.IsNull:
                    case ElementType.IsNotNull:
                        status = true;
                        break;
                    case ElementType.Equals:
                    case ElementType.NotEquals:
                    case ElementType.LessThan:
                    case ElementType.GreaterThan:
                    case ElementType.LessThanEquals:
                    case ElementType.GreaterThanEquals:
                    case ElementType.Like:
                        IParameter parameter = this.Right as IParameter;
                        if (parameter != null && parameter.HasValue)
                        {
                            status = true;
                        }
                        break;
                }

                return status;
            }
        }

		/// <summary>
		/// Gets the XQuery operator.
		/// </summary>
		/// <value>An XQuery operator</value>
		public override string Operator
		{
			get
			{
				string op = "";

				switch (_type)
				{
					case ElementType.Equals:
						op = RelationalExpr.Operators[0];
						break;
					case ElementType.NotEquals:
						op = RelationalExpr.Operators[1];
						break;
					case ElementType.LessThan:
						op = RelationalExpr.Operators[2];
						break;
					case ElementType.GreaterThan:
						op = RelationalExpr.Operators[3];
						break;
					case ElementType.LessThanEquals:
						op = RelationalExpr.Operators[4];
						break;
					case ElementType.GreaterThanEquals:
						op = RelationalExpr.Operators[5];
						break;
                    case ElementType.Like:
                        op = RelationalExpr.Operators[6];
                        break;
                    case ElementType.IsNull:
                        op = RelationalExpr.Operators[7];
                        break;
                    case ElementType.IsNotNull:
                        op = RelationalExpr.Operators[8];
                        break;
				}

				return op;
			}
			set
			{
				_type = RelationalExpr.ConvertToElementType(value);

				// fire an event for operator change
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
            string xquery = null;

            if (SubstituteExpression == null)
            {
                string leftExpr = null;
                string rightExpr = null;
                IQueryElement leftElement = _left as IQueryElement;
                IQueryElement rightElement = _right as IQueryElement;

                if (leftElement != null)
                {
                    leftExpr = leftElement.ToXQuery();
                }

                if (rightElement != null)
                {
                    rightExpr = rightElement.ToXQuery();
                }

                if (leftExpr != null)
                {
                    StringBuilder query = new StringBuilder();

                    if (rightExpr != null)
                    {
                        query.Append(leftExpr).Append(" ").Append(Operator).Append(" ").Append(rightExpr);

                        xquery = query.ToString();
                    }
                    else
                    {
                        // could be IsNull or IsNotNull operators
                        if (_type == ElementType.IsNull)
                        {
                            // special handling for "is null"
                            query.Append(leftExpr).Append(" = null");

                            xquery = query.ToString();
                        }
                        else if (_type == ElementType.IsNotNull)
                        {
                            // special handling for "is not null"
                            query.Append(leftExpr).Append(" != null");

                            xquery = query.ToString();
                        }
                    }
                }
            }
            else
            {
                IQueryElement queryElement = SubstituteExpression as IQueryElement;
                if (queryElement != null)
                {
                    xquery = queryElement.ToXQuery();
                }
            }

			return xquery;
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return " " + this.Operator + " ";
		}
	}
}
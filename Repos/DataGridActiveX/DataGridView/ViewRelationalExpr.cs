/*
* @(#)ViewRelationalExpr.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a logical expressions
	/// </summary>
	/// <version> 1.0.0 14 May 2006 </version>
	///
	public class ViewRelationalExpr : ViewBinaryExpr
	{
		/// <summary>
		/// Operator of relational expression
		/// </summary>
		public static string[] Operators = new string[] {"=", "!=", "<", ">", "<=", ">=", "like", "is null", "is not null"};

		/// <summary>
		/// Convert an operator string to a ViewElementType
		/// </summary>
		/// <param name="op">An operator string</param>
		/// <returns>A ViewElementType</returns>
		public static ViewElementType ConvertToElementType(string op)
		{
			ViewElementType type = ViewElementType.Unknown;

			switch (op)
			{
				case "=":
					type = ViewElementType.Equals;
					break;
				case "!=":
					type = ViewElementType.NotEquals;
					break;
				case "<":
					type = ViewElementType.LessThan;
					break;
				case ">":
					type = ViewElementType.GreaterThan;
					break;
				case "<=":
					type = ViewElementType.LessThanEquals;
					break;
				case ">=":
					type = ViewElementType.GreaterThanEquals;
					break;
                case "like":
                    type = ViewElementType.Like;
                    break;
                case "is null":
                    type = ViewElementType.IsNull;
                    break;
                case "is not null":
                    type = ViewElementType.IsNotNull;
                    break;
			}

			return type;
		}

		/// <summary>
		/// Initiate an instance of ViewRelationalExpr object.
		/// </summary>
		public ViewRelationalExpr(ViewElementType type) : base()
		{
			if (type == ViewElementType.Equals || type == ViewElementType.NotEquals ||
				type == ViewElementType.LessThan || type == ViewElementType.GreaterThan ||
				type == ViewElementType.LessThanEquals || type == ViewElementType.GreaterThanEquals ||
                type == ViewElementType.Like ||
                type == ViewElementType.IsNull || type == ViewElementType.IsNotNull)
			{
				_type = type;
				Name = Operator;
			}
			else
			{
				_type = ViewElementType.Unknown;
			}
		}

		/// <summary>
		/// Initiate an instance of ViewRelationalExpr object.
		/// </summary>
		public ViewRelationalExpr(ViewElementType type, IDataGridViewElement left, IDataGridViewElement right) : base(left, right)
		{
			if (type == ViewElementType.Equals || type == ViewElementType.NotEquals ||
				type == ViewElementType.LessThan || type == ViewElementType.GreaterThan ||
				type == ViewElementType.LessThanEquals || type == ViewElementType.GreaterThanEquals ||
                type == ViewElementType.Like ||
                type == ViewElementType.IsNull || type == ViewElementType.IsNotNull)
			{
				_type = type;
				Name = Operator;
			}
			else
			{
				_type = ViewElementType.Unknown;
			}
		}

		/// <summary>
		/// Initiate an instance of ViewRelationalExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal ViewRelationalExpr(XmlElement parent) : base(parent)
		{
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
					case ViewElementType.Equals:
						op = ViewRelationalExpr.Operators[0];
						break;
					case ViewElementType.NotEquals:
						op = ViewRelationalExpr.Operators[1];
						break;
					case ViewElementType.LessThan:
						op = ViewRelationalExpr.Operators[2];
						break;
					case ViewElementType.GreaterThan:
						op = ViewRelationalExpr.Operators[3];
						break;
					case ViewElementType.LessThanEquals:
						op = ViewRelationalExpr.Operators[4];
						break;
					case ViewElementType.GreaterThanEquals:
						op = ViewRelationalExpr.Operators[5];
						break;
                    case ViewElementType.Like:
                        op = ViewRelationalExpr.Operators[6];
                        break;
                    case ViewElementType.IsNull:
                        op = ViewRelationalExpr.Operators[7];
                        break;
                    case ViewElementType.IsNotNull:
                        op = ViewRelationalExpr.Operators[8];
                        break;
				}

				return op;
			}
			set
			{
				_type = ViewRelationalExpr.ConvertToElementType(value);
			}
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
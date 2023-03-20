/*
* @(#)LogicalExpr.cs
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
	/// Represents a logical expression (And/Or)
	/// </summary>
	/// <version> 1.0.0 14 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class LogicalExpr : BinaryExpr
	{
		/// <summary>
		/// Operators of logical expressions
		/// </summary>
		public static string[] Operators = new string[] {"and", "or"};

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
				case "and":
					type = ElementType.And;
					break;
				case "or":
					type = ElementType.Or;
					break;
			}

			return type;
		}

		/// <summary>
		/// Initiate an instance of LogicalExpr object.
		/// </summary>
		public LogicalExpr(ElementType type, IDataViewElement left, IDataViewElement right) : base(left, right)
		{
			if (type == ElementType.And || type == ElementType.Or)
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
		/// Initiate an instance of LogicalExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal LogicalExpr(XmlElement parent) : base(parent)
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
					case ElementType.And:
						op = LogicalExpr.Operators[0];
						break;
					case ElementType.Or:
						op = LogicalExpr.Operators[1];
						break;
				}

				return op;
			}
			set
			{
				_type = LogicalExpr.ConvertToElementType(value);

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

			if (leftExpr != null && rightExpr != null)
			{
				StringBuilder query = new StringBuilder();

				query.Append(leftExpr).Append(" ").Append(Operator).Append(" ").Append(rightExpr);

				xquery = query.ToString();
			}
			else if (leftExpr != null)
			{
				xquery = leftExpr;
			}
			else if (rightExpr != null)
			{
				xquery = rightExpr;
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
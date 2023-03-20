/*
* @(#)ViewLogicalExpr.cs
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
	/// Represents a logical expression (And/Or)
	/// </summary>
	/// <version> 1.0.0 14 May 2006 </version>
	///
	public class ViewLogicalExpr : ViewBinaryExpr
	{
		/// <summary>
		/// Operators of logical expressions
		/// </summary>
		public static string[] Operators = new string[] {"and", "or"};

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
				case "and":
					type = ViewElementType.And;
					break;
				case "or":
					type = ViewElementType.Or;
					break;
			}

			return type;
		}

		/// <summary>
		/// Initiate an instance of ViewLogicalExpr object.
		/// </summary>
		public ViewLogicalExpr(ViewElementType type, IDataGridViewElement left, IDataGridViewElement right) : base(left, right)
		{
			if (type == ViewElementType.And || type == ViewElementType.Or)
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
		/// Initiate an instance of ViewLogicalExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal ViewLogicalExpr(XmlElement parent) : base(parent)
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
					case ViewElementType.And:
						op = ViewLogicalExpr.Operators[0];
						break;
					case ViewElementType.Or:
						op = ViewLogicalExpr.Operators[1];
						break;
				}

				return op;
			}
			set
			{
				_type = ViewLogicalExpr.ConvertToElementType(value);
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
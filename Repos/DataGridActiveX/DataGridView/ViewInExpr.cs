/*
* @(#)ViewInExpr.cs
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
	/// Represents a In expression (In/NotIn)
	/// </summary>
	/// <version> 1.0.0 14 May 2006 </version>
	///
	public class ViewInExpr : ViewBinaryExpr
	{
		/// <summary>
		/// Operators of In expression
		/// </summary>
		public static string[] Operators = new string[] {"in", "not in"};

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
				case "in":
					type = ViewElementType.In;
					break;
				case "not in":
					type = ViewElementType.NotIn;
					break;
			}

			return type;
		}

		/// <summary>
		/// Initiate an instance of ViewInExpr object.
		/// </summary>
		public ViewInExpr(ViewElementType type) : base()
		{
			if (type == ViewElementType.In || type == ViewElementType.NotIn)
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
		/// Initiate an instance of ViewInExpr object.
		/// </summary>
		public ViewInExpr(ViewElementType type, IDataGridViewElement left, IDataGridViewElement right) : base(left, right)
		{
			if (type == ViewElementType.In || type == ViewElementType.NotIn)
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
		/// Initiate an instance of ViewInExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal ViewInExpr(XmlElement parent) : base(parent)
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
					case ViewElementType.In:
						op = ViewInExpr.Operators[0];
						break;
					case ViewElementType.NotIn:
						op = ViewInExpr.Operators[1];
						break;
				}

				return op;
			}
			set
			{
				_type = ViewInExpr.ConvertToElementType(value);
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
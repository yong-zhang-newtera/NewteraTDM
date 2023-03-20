/*
* @(#)ContainsFunc.cs
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
	/// Represents a Contains function
	/// </summary>
	/// <version> 1.0.0 14 Mar 2004 </version>
	/// <author> Yong Zhang</author>
	public class ContainsFunc : BinaryExpr
	{
		/// <summary>
		/// Initiate an instance of ContainsFunc object.
		/// </summary>
		public ContainsFunc() : base()
		{
			_type = ElementType.Contains;
			Name = Operator;
		}

		/// <summary>
		/// Initiate an instance of ContainsFunc object.
		/// </summary>
		public ContainsFunc(IDataViewElement left, IDataViewElement right) : base(left, right)
		{
			_type = ElementType.Contains;
			Name = Operator;
		}

		/// <summary>
		/// Initiate an instance of ContainsFunc class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal ContainsFunc(XmlElement parent) : base(parent)
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
				return "contains";
			}
			set
			{
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

				query.Append(Operator).Append("(").Append(leftExpr).Append(", ").Append(rightExpr).Append(") ");

				xquery = query.ToString();
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
/*
* @(#)ViewContainsFunc.cs
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
	/// Represents a Contains function
	/// </summary>
	/// <version> 1.0.0 14 May 2006 </version>
	public class ViewContainsFunc : ViewBinaryExpr
	{
		/// <summary>
		/// Initiate an instance of ViewContainsFunc object.
		/// </summary>
		public ViewContainsFunc() : base()
		{
			_type = ViewElementType.Contains;
			Name = Operator;
		}

		/// <summary>
		/// Initiate an instance of ViewContainsFunc object.
		/// </summary>
		public ViewContainsFunc(IDataGridViewElement left, IDataGridViewElement right) : base(left, right)
		{
			_type = ViewElementType.Contains;
			Name = Operator;
		}

		/// <summary>
		/// Initiate an instance of ViewContainsFunc class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal ViewContainsFunc(XmlElement parent) : base(parent)
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
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return " " + this.Operator + " ";
		}
	}
}
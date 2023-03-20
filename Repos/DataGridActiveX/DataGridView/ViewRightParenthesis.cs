/*
* @(#)ViewRightParenthesis.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a left parenthesis in a search filter expression.
	/// Only used to construct flatten search filter expression.
	/// </summary>
	/// <version>1.0.1 05 May 2006</version>
	
	public class ViewRightParenthesis : DataGridViewElementBase
	{
		IDataGridViewElement _parent;

		/// <summary>
		/// Initiating an instance of ViewRightParenthesis class
		/// </summary>
		public ViewRightParenthesis() : base()
		{
			_parent = null;
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.RightParenthesis;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
		}
		
		/// <summary>
		/// Gets or sets the parent expression
		/// </summary>
		public IDataGridViewElement Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return ")";
		}
	}
}
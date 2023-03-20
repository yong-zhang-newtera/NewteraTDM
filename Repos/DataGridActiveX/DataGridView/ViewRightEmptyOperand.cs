/*
* @(#)ViewRightEmptyOperand.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a left empty operand of a binary expression.
	/// Only used to construct flatten search filter expression for the purpose of
	/// a class view building.
	/// </summary>
	/// <version>1.0.1 21 May 2006</version>
	public class ViewRightEmptyOperand : DataGridViewElementBase
	{
		private ViewBinaryExpr _parent;

		/// <summary>
		/// Initiating an instance of ViewRightEmptyOperand class
		/// </summary>
		public ViewRightEmptyOperand() : base()
		{
			_parent = null;
		}
		
		/// <summary>
		/// Gets or sets the parent of right operand
		/// </summary>
		/// <value>The binary expression of right operand</value>
		public ViewBinaryExpr Parent
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
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.RightEmptyOperand;
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
			return "?";
		}
	}
}
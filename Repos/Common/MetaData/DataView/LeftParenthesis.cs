/*
* @(#)LeftParenthesis.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a left parenthesis in a search filter expression.
	/// Only used to construct flatten search filter expression.
	/// </summary>
	/// <version>1.0.1 05 Nov 2003</version>
	/// <author>Yong Zhang</author>
	public class LeftParenthesis : DataViewElementBase
	{
		/// <summary>
		/// Initiating an instance of LeftParenthesis class
		/// </summary>
		public LeftParenthesis() : base()
		{
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.LeftParenthesis;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
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
			return "(";
		}
	}
}
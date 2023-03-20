/*
* @(#)ViewAttributeCollection.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of Result Attributes.
	/// </summary>
	/// <version>1.0.1 29 May 2006</version>
	public class ViewAttributeCollection : DataGridViewElementCollection
	{
		/// <summary>
		/// Initiating an instance of ViewAttributeCollection class
		/// </summary>
		public ViewAttributeCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ViewAttributeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ViewAttributeCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.ResultAttributes;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			if (visitor.VisitSimpleAttributes(this))
			{
				foreach (IDataGridViewElement element in List)
				{
					element.Accept(visitor);
				}
			}
		}
	}
}
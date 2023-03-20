/*
* @(#)ViewParameterCollection.cs
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
	/// Represents a collection of parameters in "in" or "not in" expressions.
	/// </summary>
	/// <version>1.0.1 29 May 2006</version>
	
	public class ViewParameterCollection : DataGridViewElementCollection
	{
		/// <summary>
		/// Initiating an instance of ViewParameterCollection class
		/// </summary>
		public ViewParameterCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ViewParameterCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ViewParameterCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.Parameters;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			if (visitor.VisitParametersBegin(this))
			{
				foreach (IDataGridViewElement element in List)
				{
					element.Accept(visitor);
				}
			}

			visitor.VisitParametersEnd(this);
		}
	}
}
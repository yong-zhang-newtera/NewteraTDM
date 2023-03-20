/*
* @(#)ReferencedClassCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of referenced class.
	/// </summary>
	/// <version>1.0.1 29 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class ReferencedClassCollection : DataViewElementCollection
	{
		/// <summary>
		/// Initiating an instance of ReferencedClassCollection class
		/// </summary>
		public ReferencedClassCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ReferencedClassCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ReferencedClassCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.ReferencedClasses;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			if (visitor.VisitReferencedClasses(this))
			{
				foreach (IDataViewElement element in List)
				{
					element.Accept(visitor);
				}
			}
		}
	}
}
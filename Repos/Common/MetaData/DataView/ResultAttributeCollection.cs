/*
* @(#)ResultAttributeCollection.cs
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
	/// Represents a collection of Result Attributes.
	/// </summary>
	/// <version>1.0.1 29 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class ResultAttributeCollection : DataViewElementCollection
	{
		/// <summary>
		/// Initiating an instance of ResultAttributeCollection class
		/// </summary>
		public ResultAttributeCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ResultAttributeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ResultAttributeCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.ResultAttributes;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			if (visitor.VisitResultAttributes(this))
			{
				foreach (IDataViewElement element in List)
				{
					element.Accept(visitor);
				}
			}
		}
	}
}
/*
* @(#)NewteraActivityTrackingRecordCollection.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of NewteraActivityTrackingRecord instances.
	/// </summary>
	/// <version>1.0.0 3 Jan 2006</version>
	public class NewteraActivityTrackingRecordCollection : WFModelElementCollection
	{
		/// <summary>
		/// Initiating an instance of NewteraActivityTrackingRecordCollection class
		/// </summary>
		public NewteraActivityTrackingRecordCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of NewteraActivityTrackingRecordCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal NewteraActivityTrackingRecordCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.ActivityTrackingRecordCollection;
			}
		}

		/// <summary>
        /// Accept a visitor of IWFModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
			if (visitor.VisitActivityTrackingRecordCollection(this))
			{
				foreach (IWFModelElement element in List)
				{
					element.Accept(visitor);
				}
			}
		}
	}
}
/*
* @(#)SubjectEntryCollection.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of UserEntry instances.
	/// </summary>
	/// <version>1.0.0 25 Oct 2008</version>
	public class SubjectEntryCollection : WFModelElementCollection
	{
		/// <summary>
		/// Initiating an instance of SubjectEntryCollection class
		/// </summary>
		public SubjectEntryCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of SubjectEntryCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SubjectEntryCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.SubjectEntries;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
			if (visitor.VisitSubjectEntryCollection(this))
			{
				foreach (IWFModelElement element in List)
				{
					element.Accept(visitor);
				}
			}
		}
	}
}
/*
* @(#)WorkflowModelCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of WorkflowModel instances.
	/// </summary>
	/// <version>1.0.0 8 Dec 2006</version>
	/// <author>Yong Zhang</author>
	public class WorkflowModelCollection : WFModelElementCollection
	{
		/// <summary>
		/// Initiating an instance of WorkflowModelCollection class
		/// </summary>
		public WorkflowModelCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of WorkflowModelCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal WorkflowModelCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.Workflows;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
			if (visitor.VisitWorkflowModelCollection(this))
			{
				foreach (IWFModelElement element in List)
				{
					element.Accept(visitor);
				}
			}
		}
	}
}
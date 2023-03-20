/*
* @(#)EventGroupCollection.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of EventGroup instances.
	/// </summary>
	/// <version>1.0.0 22 Dec 2006</version>
	public class EventGroupCollection : EventNodeCollection
	{
		/// <summary>
		/// Initiating an instance of EventGroupCollection class
		/// </summary>
		public EventGroupCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of EventGroupCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal EventGroupCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of EventNodeType values</value>
		public override EventNodeType NodeType
		{
			get
			{
                return EventNodeType.EventGroupCollection;
			}
		}

        /// <summary>
        /// Accept a visitor of IEventNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IEventNodeVisitor visitor)
        {
            if (visitor.VisitEventGroupCollection(this))
            {
                foreach (EventGroup element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}
/*
* @(#)EventCollection.cs
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
	/// Represents a collection of EventDef instances.
	/// </summary>
	/// <version>1.0.0 22 Dec 2006</version>
	public class EventCollection : EventNodeCollection
	{
		/// <summary>
		/// Initiating an instance of EventCollection class
		/// </summary>
		public EventCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of EventCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal EventCollection(XmlElement xmlElement) : base(xmlElement)
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
				return EventNodeType.EventCollection;
			}
		}

        /// <summary>
        /// Accept a visitor of IEventNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IEventNodeVisitor visitor)
        {
            if (visitor.VisitEventCollection(this))
            {
                foreach (EventDef element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}
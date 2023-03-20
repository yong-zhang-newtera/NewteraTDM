/*
* @(#)SubscriberCollection.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of Subscriber instances.
	/// </summary>
	/// <version>1.0.0 16 Sep 2013</version>
	public class SubscriberCollection : SubscriberNodeCollection
	{
		/// <summary>
		/// Initiating an instance of SubscriberCollection class
		/// </summary>
		public SubscriberCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of SubscriberCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SubscriberCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of SubscriberNodeType values</value>
		public override SubscriberNodeType NodeType
		{
			get
			{
				return SubscriberNodeType.SubscriberCollection;
			}
		}

        /// <summary>
        /// Accept a visitor of ISubscriberNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ISubscriberNodeVisitor visitor)
        {
            if (visitor.VisitSubscriberCollection(this))
            {
                foreach (Subscriber element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}
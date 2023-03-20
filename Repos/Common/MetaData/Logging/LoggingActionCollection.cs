/*
* @(#)LoggingActionCollection.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of logging actions.
	/// </summary>
	/// <version>1.0.1 04 Jan 2009</version>
	public class LoggingActionCollection : LoggingNodeCollection
	{
		/// <summary>
		/// Initiating an instance of LoggingActionCollection class
		/// </summary>
		public LoggingActionCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of LoggingActionCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingActionCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Actions;
			}
		}

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            if (visitor.VisitLoggingActionCollection(this))
            {
                foreach (ILoggingNode element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}
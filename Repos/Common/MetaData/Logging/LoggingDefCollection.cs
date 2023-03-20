/*
* @(#)LoggingDefCollection.cs
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
	/// Represents a collection of logging defitions.
	/// </summary>
	/// <version>1.0.1 10 Dec 2003</version>
	public class LoggingDefCollection : LoggingNodeCollection
	{
		/// <summary>
		/// Initiating an instance of LoggingDefCollection class
		/// </summary>
		public LoggingDefCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of LoggingDefCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingDefCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.Definitions;
			}
		}

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            if (visitor.VisitLoggingDefCollection(this))
            {
                foreach (ILoggingNode element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}
/*
* @(#)XaclRuleCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of xacl rules.
	/// </summary>
	/// <version>1.0.1 10 Dec 2003</version>
	/// <author>Yong Zhang</author>
	public class XaclRuleCollection : XaclNodeCollection
	{
		/// <summary>
		/// Initiating an instance of XaclRuleCollection class
		/// </summary>
		public XaclRuleCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of XaclRuleCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XaclRuleCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.Rules;
			}
		}

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXaclNodeVisitor visitor)
        {
            if (visitor.VisitXaclRuleCollection(this))
            {
                foreach (IXaclNode element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}